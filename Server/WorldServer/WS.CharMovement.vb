'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Threading
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter

Module WS_CharMovement

#Region "WS.CharacterMovement.MovementHandlers"

    Private Const PId2 As Single = Math.PI / 2
    Private Const PIx2 As Single = 2 * Math.PI

    Public Enum MovementFlags As Integer
        MOVEMENTFLAG_NONE = &H0
        MOVEMENTFLAG_FORWARD = &H1
        MOVEMENTFLAG_BACKWARD = &H2
        MOVEMENTFLAG_STRAFE_LEFT = &H4
        MOVEMENTFLAG_STRAFE_RIGHT = &H8
        MOVEMENTFLAG_LEFT = &H10
        MOVEMENTFLAG_RIGHT = &H20
        MOVEMENTFLAG_PITCH_UP = &H40
        MOVEMENTFLAG_PITCH_DOWN = &H80

        MOVEMENTFLAG_WALK = &H100
        MOVEMENTFLAG_JUMPING = &H2000
        MOVEMENTFLAG_FALLING = &H4000
        MOVEMENTFLAG_SWIMMING = &H200000
        MOVEMENTFLAG_ONTRANSPORT = &H2000000
        MOVEMENTFLAG_SPLINE = &H4000000
    End Enum

    Public Sub OnMovementPacket(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()

        If Client.Character.MindControl IsNot Nothing Then
            OnControlledMovementPacket(packet, Client.Character.MindControl, Client.Character)
            Exit Sub
        End If

        Client.Character.movementFlags = packet.GetInt32()
        Dim Time As UInteger = packet.GetUInt32()
        Client.Character.positionX = packet.GetFloat()
        Client.Character.positionY = packet.GetFloat()
        Client.Character.positionZ = packet.GetFloat()
        Client.Character.orientation = packet.GetFloat()

        'DONE: If character is falling below the world
        If Client.Character.positionZ < -500.0F Then
            AllGraveYards.GoToNearestGraveyard(Client.Character, False, True)
            Exit Sub
        End If

        If Client.Character.Pet IsNot Nothing Then
            If Client.Character.Pet.FollowOwner Then
                Dim angle As Single = Client.Character.orientation - PId2
                If angle < 0 Then angle += PIx2

                Client.Character.Pet.SetToRealPosition()

                Dim tmpX As Single = Client.Character.positionX + Math.Cos(angle) * 2.0F
                Dim tmpY As Single = Client.Character.positionY + Math.Sin(angle) * 2.0F
                Client.Character.Pet.MoveTo(tmpX, tmpY, Client.Character.positionZ, Client.Character.orientation, True)
            End If
        End If

#If ENABLE_PPOINTS Then
        If (Client.Character.movementFlags And groundFlagsMask) = 0 AndAlso _
           Math.Abs(GetZCoord(Client.Character.positionX, Client.Character.positionY, Client.Character.positionZ, Client.Character.MapID) - Client.Character.positionZ) > PPOINT_LIMIT Then
            Log.WriteLine(LogType.DEBUG, "PPoints: {0} [MapZ = {1}]", Client.Character.positionZ, GetZCoord(Client.Character.positionX, Client.Character.positionY, Client.Character.MapID))
            SetZCoord_PP(Client.Character.positionX, Client.Character.positionY, Client.Character.MapID, Client.Character.positionZ)
        End If
#End If


        If (Client.Character.movementFlags And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
            Dim transportGUID As ULong = packet.GetUInt64
            Dim transportX As Single = packet.GetFloat
            Dim transportY As Single = packet.GetFloat
            Dim transportZ As Single = packet.GetFloat
            Dim transportO As Single = packet.GetFloat

            Client.Character.transportX = transportX
            Client.Character.transportY = transportY
            Client.Character.transportZ = transportZ
            Client.Character.transportO = transportO

            'DONE: Boarding transport
            If Client.Character.OnTransport Is Nothing Then
                If GuidIsMoTransport(transportGUID) AndAlso WORLD_TRANSPORTs.ContainsKey(transportGUID) Then
                    Client.Character.OnTransport = WORLD_TRANSPORTs(transportGUID)

                    'DONE: Unmount when boarding
                    Client.Character.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOUNTED)

                    CType(Client.Character.OnTransport, TransportObject).AddPassenger(Client.Character)
                ElseIf GuidIsTransport(transportGUID) AndAlso WORLD_GAMEOBJECTs.ContainsKey(transportGUID) Then
                    Client.Character.OnTransport = WORLD_GAMEOBJECTs(transportGUID)
                End If
            End If
        ElseIf Client.Character.OnTransport IsNot Nothing Then
            'DONE: Unboarding transport
            If TypeOf Client.Character.OnTransport Is TransportObject Then
                CType(Client.Character.OnTransport, TransportObject).RemovePassenger(Client.Character)
            End If
            Client.Character.OnTransport = Nothing
        End If

        If (Client.Character.movementFlags And (MovementFlags.MOVEMENTFLAG_SWIMMING)) Then
            Dim swimAngle As Single = packet.GetFloat
            '#If DEBUG Then
            '                Console.WriteLine("[{0}] [{1}:{2}] Client swim angle:{3}", Format(TimeOfDay, "HH:mm:ss"), Client.IP, Client.Port, swimAngle)
            '#End If
        End If

        packet.GetInt32() 'Fall time

        If (Client.Character.movementFlags And MovementFlags.MOVEMENTFLAG_JUMPING) Then
            Dim airTime As UInteger = packet.GetUInt32
            Dim sinAngle As Single = packet.GetFloat
            Dim cosAngle As Single = packet.GetFloat
            Dim xySpeed As Single = packet.GetFloat
            '#If DEBUG Then
            '                Console.WriteLine("[{0}] [{1}:{2}] Client jump: 0x{3:X} {4} {5} {6}", Format(TimeOfDay, "HH:mm:ss"), Client.IP, Client.Port, unk, sinAngle, cosAngle, xySpeed)
            '#End If
        End If

        If (Client.Character.movementFlags And MovementFlags.MOVEMENTFLAG_SPLINE) Then
            Dim unk1 As Single = packet.GetFloat
        End If

        If Client.Character.exploreCheckQueued_ AndAlso (Not Client.Character.DEAD) Then
            Dim exploreFlag As Integer = GetAreaFlag(Client.Character.positionX, Client.Character.positionY, Client.Character.MapID)

            'DONE: Checking Explore System
            If exploreFlag <> &HFFFF Then
                Dim areaFlag As Integer = exploreFlag Mod 32
                Dim areaFlagOffset As Byte = exploreFlag \ 32

                If Not HaveFlag(Client.Character.ZonesExplored(areaFlagOffset), areaFlag) Then
                    SetFlag(Client.Character.ZonesExplored(areaFlagOffset), areaFlag, True)

                    Dim GainedXP As Integer = AreaTable(exploreFlag).Level * 10
                    GainedXP = CInt(AreaTable(exploreFlag).Level) * 10

                    Dim SMSG_EXPLORATION_EXPERIENCE As New PacketClass(OPCODES.SMSG_EXPLORATION_EXPERIENCE)
                    SMSG_EXPLORATION_EXPERIENCE.AddInt32(AreaTable(exploreFlag).ID)
                    SMSG_EXPLORATION_EXPERIENCE.AddInt32(GainedXP)
                    Client.Send(SMSG_EXPLORATION_EXPERIENCE)
                    SMSG_EXPLORATION_EXPERIENCE.Dispose()

                    Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_EXPLORED_ZONES_1 + areaFlagOffset, Client.Character.ZonesExplored(areaFlagOffset))
                    Client.Character.AddXP(GainedXP, 0, 0, True)

                    'DONE: Fire quest event to check for if this area is used in explore area quest
                    ALLQUESTS.OnQuestExplore(Client.Character, exploreFlag)
                End If
            End If
        End If

        'If character is moving
        If Client.Character.isMoving Then
            'DONE: Stop emotes if moving
            If Client.Character.cEmoteState > 0 Then
                Client.Character.cEmoteState = 0
                Client.Character.SetUpdateFlag(EUnitFields.UNIT_NPC_EMOTESTATE, Client.Character.cEmoteState)
                Client.Character.SendCharacterUpdate(True)
            End If

            'DONE: Stop casting
            If Client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing Then
                With Client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
                    If .Finished = False And (SPELLs(.SpellID).interruptFlags And SpellInterruptFlags.SPELL_INTERRUPT_FLAG_MOVEMENT) Then
                        Client.Character.FinishSpell(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
                    End If
                End With
            End If

            Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_MOVE)
        End If

        'If character is turning
        If Client.Character.isTurning Then
            Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TURNING)
        End If

        'DONE: Movement time calculation
        Dim MsTime As Integer = WS_Network.msTime()
        Dim ClientTimeDelay As Integer = MsTime - Time
        Dim MoveTime As Integer = (Time - (MsTime - ClientTimeDelay)) + 500 + MsTime
        packet.AddInt32(MoveTime, 10)

        'DONE: Send to nearby players
        Dim response As New PacketClass(packet.OpCode)
        response.AddPackGUID(Client.Character.GUID)
        Dim tempArray(packet.Data.Length - 6) As Byte
        Array.Copy(packet.Data, 6, tempArray, 0, packet.Data.Length - 6)
        response.AddByteArray(tempArray)
        Client.Character.SendToNearPlayers(response, , False)
        response.Dispose()

        'NOTE: They may slow the movement down so let's do them after the packet is sent
        'DONE: Remove auras that requires you to not move
        If Client.Character.isMoving Then
            Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_MOVE)
        End If
        'DONE: Remove auras that requires you to not turn
        If Client.Character.isTurning Then
            Client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TURNING)
        End If
    End Sub
    Public Sub OnControlledMovementPacket(ByRef packet As PacketClass, ByRef Controlled As BaseUnit, ByRef Controller As CharacterObject)
        Dim MovementFlags As Integer = packet.GetInt32()
        Dim Time As UInteger = packet.GetUInt32()
        Dim PositionX As Single = packet.GetFloat()
        Dim PositionY As Single = packet.GetFloat()
        Dim PositionZ As Single = packet.GetFloat()
        Dim Orientation As Single = packet.GetFloat()

        If TypeOf Controlled Is CharacterObject Then
            With CType(Controlled, CharacterObject)
                .movementFlags = MovementFlags
                .positionX = PositionX
                .positionY = PositionY
                .positionZ = PositionZ
                .orientation = Orientation
            End With
        ElseIf TypeOf Controlled Is CreatureObject Then
            With CType(Controlled, CreatureObject)
                .positionX = PositionX
                .positionY = PositionY
                .positionZ = PositionZ
                .orientation = Orientation
            End With
        End If

        'DONE: Movement time calculation
        Dim MsTime As Integer = WS_Network.msTime
        Dim ClientTimeDelay As Integer = MsTime - Time
        Dim MoveTime As Integer = (Time - (MsTime - ClientTimeDelay)) + 500 + MsTime
        packet.AddInt32(MoveTime, 10)

        'DONE: Send to nearby players
        Dim response As New PacketClass(packet.OpCode)
        response.AddPackGUID(Controlled.GUID)
        Dim tempArray(packet.Data.Length - 6) As Byte
        Array.Copy(packet.Data, 6, tempArray, 0, packet.Data.Length - 6)
        response.AddByteArray(tempArray)
        Controlled.SendToNearPlayers(response, Controller.GUID)
        response.Dispose()
    End Sub
    Public Sub OnStartSwim(ByRef packet As PacketClass, ByRef Client As ClientClass)
        OnMovementPacket(packet, Client)

        If Client.Character.positionZ < GetWaterLevel(Client.Character.positionX, Client.Character.positionY, Client.Character.MapID) Then
            If (Client.Character.underWaterTimer Is Nothing) AndAlso (Not Client.Character.underWaterBreathing) AndAlso (Not Client.Character.DEAD) Then
                Client.Character.underWaterTimer = New TDrowningTimer(Client.Character)
            End If
        Else
            If Client.Character.underWaterTimer IsNot Nothing Then
                Client.Character.underWaterTimer.Dispose()
                Client.Character.underWaterTimer = Nothing
            End If
        End If
    End Sub
    Public Sub OnStopSwim(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If Client.Character.underWaterTimer IsNot Nothing Then
            Client.Character.underWaterTimer.Dispose()
            Client.Character.underWaterTimer = Nothing
        End If

        OnMovementPacket(packet, Client)
    End Sub

    Public Sub OnChangeSpeed(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        If GUID <> Client.Character.GUID Then Exit Sub 'Skip it, it's not our packet

        packet.GetInt32()
        Dim flags As Integer = packet.GetInt32()
        Dim time As Integer = packet.GetInt32()
        Client.Character.positionX = packet.GetFloat()
        Client.Character.positionY = packet.GetFloat()
        Client.Character.positionZ = packet.GetFloat()
        Client.Character.orientation = packet.GetFloat()

        If (flags And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
            packet.GetInt64() 'GUID
            packet.GetFloat() 'X
            packet.GetFloat() 'Y
            packet.GetFloat() 'Z
            packet.GetFloat() 'O
        End If
        If (flags And (MovementFlags.MOVEMENTFLAG_SWIMMING)) Then
            packet.GetFloat() 'angle
        End If

        Dim falltime As Single = packet.GetInt32()

        If (flags And MovementFlags.MOVEMENTFLAG_JUMPING) Then
            packet.GetFloat() 'unk
            packet.GetFloat() 'sin angle
            packet.GetFloat() 'cos angle
            packet.GetFloat() 'xyz speed
        End If

        Dim newSpeed As Single = packet.GetFloat()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] {3} [{2}]", Client.IP, Client.Port, newSpeed, packet.OpCode)

        'DONE: Anti hack
        If Client.Character.antiHackSpeedChanged_ <= 0 Then
            Log.WriteLine(LogType.WARNING, "[{0}:{1}] CHEAT: Possible speed hack detected!", Client.IP, Client.Port)
            Client.Character.Logout(Nothing)
            Exit Sub
        End If

        'DONE: Update speed value and create packet
        Client.Character.antiHackSpeedChanged_ -= 1
        Select Case packet.OpCode
            Case OPCODES.CMSG_FORCE_RUN_SPEED_CHANGE_ACK
                Client.Character.RunSpeed = newSpeed
            Case OPCODES.CMSG_FORCE_RUN_BACK_SPEED_CHANGE_ACK
                Client.Character.RunBackSpeed = newSpeed
            Case OPCODES.CMSG_FORCE_SWIM_BACK_SPEED_CHANGE_ACK
                Client.Character.SwimBackSpeed = newSpeed
            Case OPCODES.CMSG_FORCE_SWIM_SPEED_CHANGE_ACK
                Client.Character.SwimSpeed = newSpeed
            Case OPCODES.CMSG_FORCE_TURN_RATE_CHANGE_ACK
                Client.Character.TurnRate = newSpeed
        End Select
    End Sub

    Public Sub SendAreaTriggerMessage(ByRef Client As ClientClass, ByVal Text As String)
        Dim p As New PacketClass(OPCODES.SMSG_AREA_TRIGGER_MESSAGE)
        p.AddInt32(Text.Length)
        p.AddString(Text)
        Client.Send(p)
        p.Dispose()
    End Sub
    Public Sub On_CMSG_AREATRIGGER(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim triggerID As Integer = packet.GetInt32
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AREATRIGGER [triggerID={2}]", Client.IP, Client.Port, triggerID)

            'TODO: Check if in combat?

            Dim q As New DataTable

            'DONE: Handling quest triggers
            q.Clear()
            WorldDatabase.Query(String.Format("SELECT * FROM areatrigger_involvedrelation WHERE id = {0};", triggerID), q)
            If q.Rows.Count > 0 Then
                ALLQUESTS.OnQuestExplore(Client.Character, triggerID)
                Exit Sub
            End If

            'TODO: Handling tavern triggers
            q.Clear()
            WorldDatabase.Query(String.Format("SELECT * FROM areatrigger_tavern WHERE id = {0};", triggerID), q)
            If q.Rows.Count > 0 Then
                Client.Character.cPlayerFlags = Client.Character.cPlayerFlags Or PlayerFlags.PLAYER_FLAG_RESTING
                Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, Client.Character.cPlayerFlags)
                Client.Character.SendCharacterUpdate(True)
                Exit Sub
            End If

            'DONE: Handling teleport triggers
            q.Clear()
            WorldDatabase.Query(String.Format("SELECT * FROM areatrigger_teleport WHERE id = {0};", triggerID), q)
            If q.Rows.Count > 0 Then
                If Client.Character.DEAD Then
                    If Client.Character.corpseMapID = q.Rows(0).Item("target_map") Then
                        CharacterResurrect(Client.Character)
                    Else
                        AllGraveYards.GoToNearestGraveyard(Client.Character, False, True)
                        Exit Sub
                    End If
                End If

                If q.Rows(0).Item("required_level") <> 0 AndAlso Client.Character.Level < q.Rows(0).Item("required_level") Then
                    SendAreaTriggerMessage(Client, "Your level is too low")
                    Exit Sub
                End If

                If CSng(q.Rows(0).Item("target_position_x")) <> 0 OrElse CSng(q.Rows(0).Item("target_position_y")) <> 0 OrElse CSng(q.Rows(0).Item("target_position_z")) <> 0 Then
                    Client.Character.Teleport(q.Rows(0).Item("target_position_x"), q.Rows(0).Item("target_position_y"), q.Rows(0).Item("target_position_z"), _
                                              q.Rows(0).Item("target_orientation"), q.Rows(0).Item("target_map"))
                End If
                Exit Sub
            End If


            'DONE: Handling all other scripted triggers
            If AreaTriggers.ContainsMethod("AreaTriggers", String.Format("HandleAreaTrigger_{0}", triggerID)) Then
                AreaTriggers.InvokeFunction("AreaTriggers", String.Format("HandleAreaTrigger_{0}", triggerID), New Object() {Client.Character.GUID})
            Else
                Log.WriteLine(LogType.WARNING, "[{0}:{1}] AreaTrigger [{2}] not found!", Client.IP, Client.Port, triggerID)
            End If
        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error when entering areatrigger.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_MSG_MOVE_FALL_LAND(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Try
            OnMovementPacket(packet, Client)

            packet.Offset = 6
            Dim movFlags As Integer = packet.GetInt32()
            packet.GetUInt32()
            packet.GetFloat()
            packet.GetFloat()
            packet.GetFloat()
            packet.GetFloat()
            If (movFlags And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
                packet.GetUInt64()
                packet.GetFloat()
                packet.GetFloat()
                packet.GetFloat()
                packet.GetFloat()
            End If
            If (movFlags And (MovementFlags.MOVEMENTFLAG_SWIMMING)) Then
                packet.GetFloat()
            End If
            Dim FallTime As Integer = packet.GetInt32()

            'DONE: If FallTime > 1100 and not Dead
            If FallTime > 1100 AndAlso (Not Client.Character.DEAD) AndAlso Client.Character.positionZ > GetWaterLevel(Client.Character.positionX, Client.Character.positionY, Client.Character.MapID) Then
                If Client.Character.HaveAuraType(AuraEffects_Names.SPELL_AURA_FEATHER_FALL) = False Then
                    Dim safe_fall As Integer = Client.Character.GetAuraModifier(AuraEffects_Names.SPELL_AURA_SAFE_FALL)
                    If safe_fall > 0 Then
                        If FallTime > (safe_fall * 10) Then
                            FallTime -= (safe_fall * 10)
                        Else
                            FallTime = 0
                        End If
                    End If
                    If FallTime > 1100 Then
                        'DONE: Caluclate fall damage
                        Dim FallPerc As Single = FallTime / 1100
                        Dim FallDamage As Integer = (FallPerc * FallPerc - 1) / 9 * Client.Character.Life.Maximum

                        If FallDamage > 0 Then
                            'Prevent the fall damage to be more than your maximum health
                            If FallDamage > Client.Character.Life.Maximum Then FallDamage = Client.Character.Life.Maximum
                            'Deal the damage
                            Client.Character.LogEnvironmentalDamage(EnviromentalDamage.DAMAGE_FALL, FallDamage)
                            Client.Character.DealDamage(FallDamage)

#If DEBUG Then
                            Log.WriteLine(LogType.USER, "[{0}:{1}] Client fall time: {2}  Damage: {3}", Client.IP, Client.Port, FallTime, FallDamage)
#End If
                        End If
                    End If
                End If
            End If

            If Not Client.Character.underWaterTimer Is Nothing Then
                Client.Character.underWaterTimer.Dispose()
                Client.Character.underWaterTimer = Nothing
            End If
            If Not Client.Character.LogoutTimer Is Nothing Then
                'DONE: Initialize packet
                Dim UpdateData As New UpdateClass
                Dim SMSG_UPDATE_OBJECT As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                SMSG_UPDATE_OBJECT.AddInt32(1)      'Operations.Count
                SMSG_UPDATE_OBJECT.AddInt8(0)

                'DONE: Disable Turn
                Client.Character.cUnitFlags = Client.Character.cUnitFlags Or UnitFlags.UNIT_FLAG_STUNTED
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Client.Character.cUnitFlags)
                'DONE: StandState -> Sit
                Client.Character.StandState = StandStates.STANDSTATE_SIT
                UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, Client.Character.cBytes1)

                'DONE: Send packet
                UpdateData.AddToPacket(SMSG_UPDATE_OBJECT, ObjectUpdateType.UPDATETYPE_VALUES, CType(Client.Character, CharacterObject))
                Client.Send(SMSG_UPDATE_OBJECT)
                SMSG_UPDATE_OBJECT.Dispose()

                Dim packetACK As New PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                packetACK.AddInt8(StandStates.STANDSTATE_SIT)
                Client.Send(packetACK)
                packetACK.Dispose()
            End If
        Catch e As Exception
            Log.WriteLine(LogType.DEBUG, "Error when falling.{0}", vbNewLine & e.ToString)
        End Try
    End Sub
    Public Sub On_CMSG_ZONEUPDATE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim newZone As Integer = packet.GetInt32
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ZONEUPDATE [newZone={2}]", Client.IP, Client.Port, newZone)
        Client.Character.ZoneID = newZone
        Client.Character.exploreCheckQueued_ = True

        Client.Character.ZoneCheck()

        'DONE: Update zone on cluster
        WorldServer.Cluster.ClientUpdate(Client.Index, Client.Character.ZoneID, Client.Character.Level)

        'DONE: Send weather
        If WeatherZones.ContainsKey(newZone) Then
            SendWeather(newZone, Client)
        End If
    End Sub
    Public Sub On_MSG_MOVE_HEARTBEAT(ByRef packet As PacketClass, ByRef Client As ClientClass)
        OnMovementPacket(packet, Client)

        If Client.Character.CellX <> GetMapTileX(Client.Character.positionX) Or Client.Character.CellY <> GetMapTileY(Client.Character.positionY) Then
            MoveCell(Client.Character)
        End If
        UpdateCell(Client.Character)

        Client.Character.GroupUpdateFlag = Client.Character.GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POSITION

        Client.Character.ZoneCheck()

        'DONE: Check for out of continent - coordinates from WorldMapContinent.dbc
        If IsOutsideOfMap(CType(Client.Character, CharacterObject)) Then
            If Client.Character.outsideMapID_ = False Then
                Client.Character.outsideMapID_ = True
                Client.Character.StartMirrorTimer(MirrorTimer.FATIGUE, 30000)
            End If
        Else
            If Client.Character.outsideMapID_ = True Then
                Client.Character.outsideMapID_ = False
                Client.Character.StopMirrorTimer(MirrorTimer.FATIGUE)
            End If
        End If

        'DONE: Duel check
        If Client.Character.IsInDuel Then CheckDuelDistance(Client.Character)

        'DONE: Aggro range
        For Each cGUID As ULong In Client.Character.creaturesNear.ToArray
            If WORLD_CREATUREs.ContainsKey(cGUID) AndAlso WORLD_CREATUREs(cGUID).aiScript IsNot Nothing AndAlso ((TypeOf WORLD_CREATUREs(cGUID).aiScript Is DefaultAI) OrElse (TypeOf WORLD_CREATUREs(cGUID).aiScript Is GuardAI)) Then
                If WORLD_CREATUREs(cGUID).isDead = False AndAlso WORLD_CREATUREs(cGUID).aiScript.InCombat() = False Then
                    If Client.Character.inCombatWith.Contains(cGUID) Then Continue For
                    If Client.Character.GetReaction(WORLD_CREATUREs(cGUID).Faction) = TReaction.HOSTILE AndAlso GetDistance(WORLD_CREATUREs(cGUID), Client.Character) <= WORLD_CREATUREs(cGUID).AggroRange(Client.Character) Then
                        WORLD_CREATUREs(cGUID).aiScript.OnGenerateHate(Client.Character, 1)
                        Client.Character.AddToCombat(WORLD_CREATUREs(cGUID))
                        WORLD_CREATUREs(cGUID).aiScript.State = TBaseAI.AIState.AI_ATTACKING
                        WORLD_CREATUREs(cGUID).aiScript.DoThink()
                    End If
                End If
            End If
        Next

        'DONE: Creatures that are following you will have a more smooth movement
        For Each CombatUnit As ULong In Client.Character.inCombatWith.ToArray
            If GuidIsCreature(CombatUnit) AndAlso WORLD_CREATUREs.ContainsKey(CombatUnit) AndAlso CType(WORLD_CREATUREs(CombatUnit), CreatureObject).aiScript IsNot Nothing Then
                With CType(WORLD_CREATUREs(CombatUnit), CreatureObject)
                    If (Not .aiScript.aiTarget Is Nothing) AndAlso .aiScript.aiTarget Is Client.Character Then
                        .SetToRealPosition() 'Make sure it moves from it's location and not from where it was already heading before this
                        .aiScript.State = TBaseAI.AIState.AI_MOVE_FOR_ATTACK
                        .aiScript.DoMove()
                    End If
                End With
            End If
        Next
    End Sub


#End Region
#Region "WS.CharacterMovement.CellFramework"


    Public Sub MAP_Load(ByVal x As Byte, ByVal y As Byte, ByVal Map As UInteger)
        For i As Short = -1 To 1
            For j As Short = -1 To 1
                If x + i > -1 AndAlso x + i < 64 AndAlso y + j > -1 AndAlso y + j < 64 Then
                    If Maps(Map).TileUsed(x + i, y + j) = False Then
                        Log.WriteLine(LogType.INFORMATION, "Loading map [{2}: {0},{1}]...", x + i, y + j, Map)
                        Maps(Map).TileUsed(x + i, y + j) = True
                        Maps(Map).Tiles(x + i, y + j) = New TMapTile(x + i, y + j, Map)
                        'DONE: Load spawns
                        LoadSpawns(x + i, y + j, Map, 0)
                    End If
                End If
            Next
        Next
        GC.Collect()
    End Sub
    Public Sub MAP_UnLoad(ByVal x As Byte, ByVal y As Byte, ByVal Map As Integer)
        If Maps(Map).Tiles(x, y).PlayersHere.Count = 0 Then
            Log.WriteLine(LogType.INFORMATION, "Unloading map [{2}: {0},{1}]...", x, y, Map)
            Maps(Map).Tiles(x, y).Dispose()
            Maps(Map).Tiles(x, y) = Nothing
        End If
    End Sub

    Public Sub AddToWorld(ByRef Character As CharacterObject)
        GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)

        'DONE: Dynamic map loading
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then MAP_Load(Character.CellX, Character.CellY, Character.MapID)

        'DONE: Cleanig
        'myPacket.Dispose()
        Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Add(Character.GUID)

        'DONE: Send all creatures and gameobjects to the client
        UpdateCell(Character)

        'DONE: Add the pet to the world as well
        If Character.Pet IsNot Nothing Then
            Character.Pet.Spawn()
        End If
    End Sub
    Public Sub RemoveFromWorld(ByRef Character As CharacterObject)
        If Not Maps.ContainsKey(Character.MapID) Then Return

        If Not Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then
            'DONE: Remove character from map
            GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)
            Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Remove(Character.GUID)
        End If

        Dim list() As ULong

        'DONE: Removing from players wich can see it
        list = Character.SeenBy.ToArray
        For Each GUID As ULong In list

            If CHARACTERs(GUID).playersNear.Contains(Character.GUID) Then
                CHARACTERs(GUID).guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                CHARACTERs(GUID).guidsForRemoving.Add(Character.GUID)
                CHARACTERs(GUID).guidsForRemoving_Lock.ReleaseWriterLock()

                CHARACTERs(GUID).playersNear.Remove(Character.GUID)
            End If
            'DONE: Fully clean
            CHARACTERs(GUID).SeenBy.Remove(Character.GUID)

        Next
        Character.playersNear.Clear()
        Character.SeenBy.Clear()


        'DONE: Removing from creatures wich can see it
        list = Character.creaturesNear.ToArray
        For Each GUID As ULong In list

            If WORLD_CREATUREs(GUID).SeenBy.Contains(Character.GUID) Then
                WORLD_CREATUREs(GUID).SeenBy.Remove(Character.GUID)
            End If
        Next
        Character.creaturesNear.Clear()

        'DONE: Removing from gameObjects wich can see it
        list = Character.gameObjectsNear.ToArray
        For Each GUID As ULong In list
            If GuidIsMoTransport(GUID) Then
                If WORLD_TRANSPORTs(GUID).SeenBy.Contains(Character.GUID) Then
                    WORLD_TRANSPORTs(GUID).SeenBy.Remove(Character.GUID)
                End If
            Else
                If WORLD_GAMEOBJECTs(GUID).SeenBy.Contains(Character.GUID) Then
                    WORLD_GAMEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                End If
            End If
        Next
        Character.gameObjectsNear.Clear()

        'DONE: Removing from corpseObjects wich can see it
        list = Character.corpseObjectsNear.ToArray
        For Each GUID As ULong In list

            If WORLD_CORPSEOBJECTs(GUID).SeenBy.Contains(Character.GUID) Then
                WORLD_CORPSEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
            End If
        Next
        Character.corpseObjectsNear.Clear()

        'DONE: Remove the pet from the world as well
        If Character.Pet IsNot Nothing Then
            Character.Pet.Hide()
        End If
    End Sub
    Public Sub MoveCell(ByRef Character As CharacterObject)
        Dim oldX As Byte = Character.CellX
        Dim oldY As Byte = Character.CellY
        GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)

        'Map Loading
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then MAP_Load(Character.CellX, Character.CellY, Character.MapID)

        'TODO: Fix map unloading

        If Character.CellX <> oldX Or Character.CellY <> oldY Then
            Maps(Character.MapID).Tiles(oldX, oldY).PlayersHere.Remove(Character.GUID)
            'MAP_UnLoad(oldX, oldY)
            Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Add(Character.GUID)
        End If
    End Sub
    Public Sub UpdateCell(ByRef Character As CharacterObject)
        'Dim start As Integer = timeGetTime("")
        Dim list() As ULong

        'DONE: Remove players,creatures,objects if dist is >
        list = Character.playersNear.ToArray
        For Each GUID As ULong In list
            If Not Character.CanSee(CHARACTERs(GUID)) Then
                Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                Character.guidsForRemoving.Add(GUID)
                Character.guidsForRemoving_Lock.ReleaseWriterLock()

                CHARACTERs(GUID).SeenBy.Remove(Character.GUID)
                Character.playersNear.Remove(GUID)
            End If
            'Remove me for him
            If (Not CHARACTERs(GUID).CanSee(Character)) AndAlso Character.SeenBy.Contains(GUID) Then
                CHARACTERs(GUID).guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                CHARACTERs(GUID).guidsForRemoving.Add(Character.GUID)
                CHARACTERs(GUID).guidsForRemoving_Lock.ReleaseWriterLock()

                Character.SeenBy.Remove(GUID)
                CHARACTERs(GUID).playersNear.Remove(Character.GUID)
            End If
        Next

        list = Character.creaturesNear.ToArray
        For Each GUID As ULong In list
            If WORLD_CREATUREs.ContainsKey(GUID) = False OrElse Character.CanSee(WORLD_CREATUREs(GUID)) = False Then
                Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                Character.guidsForRemoving.Add(GUID)
                Character.guidsForRemoving_Lock.ReleaseWriterLock()

                WORLD_CREATUREs(GUID).SeenBy.Remove(Character.GUID)
                Character.creaturesNear.Remove(GUID)
            End If
        Next

        list = Character.gameObjectsNear.ToArray
        For Each GUID As ULong In list
            If GuidIsMoTransport(GUID) Then
                If Not Character.CanSee(WORLD_TRANSPORTs(GUID)) Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    WORLD_TRANSPORTs(GUID).SeenBy.Remove(Character.GUID)
                    Character.gameObjectsNear.Remove(GUID)
                End If
            Else
                If Not Character.CanSee(WORLD_GAMEOBJECTs(GUID)) Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    WORLD_GAMEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                    Character.gameObjectsNear.Remove(GUID)
                End If
            End If
        Next

        list = Character.dynamicObjectsNear.ToArray
        For Each GUID As ULong In list
            If Not Character.CanSee(WORLD_DYNAMICOBJECTs(GUID)) Then
                Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                Character.guidsForRemoving.Add(GUID)
                Character.guidsForRemoving_Lock.ReleaseWriterLock()

                WORLD_DYNAMICOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                Character.dynamicObjectsNear.Remove(GUID)
            End If
        Next

        list = Character.corpseObjectsNear.ToArray
        For Each GUID As ULong In list

            If Not Character.CanSee(WORLD_CORPSEOBJECTs(GUID)) Then
                Character.guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                Character.guidsForRemoving.Add(GUID)
                Character.guidsForRemoving_Lock.ReleaseWriterLock()

                WORLD_CORPSEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                Character.corpseObjectsNear.Remove(GUID)
            End If
        Next

        'DONE: Add new if dist is <
        Dim CellXAdd As Short = -1
        Dim CellYAdd As Short = -1
        If GetSubMapTileX(Character.positionX) > 32 Then CellXAdd = 1
        If GetSubMapTileX(Character.positionY) > 32 Then CellYAdd = 1
        If (Character.CellX + CellXAdd) > 63 Or (Character.CellX + CellXAdd) < 0 Then CellXAdd = 0
        If (Character.CellY + CellYAdd) > 63 Or (Character.CellY + CellYAdd) < 0 Then CellYAdd = 0

        'DONE: Load cell if needed
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then
            MAP_Load(Character.CellX, Character.CellY, Character.MapID)
        End If
        'DONE: Sending near creatures and gameobjects in <CENTER CELL>
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).CreaturesHere.Count > 0 OrElse Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).GameObjectsHere.Count > 0 Then
            UpdateCreaturesAndGameObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
        End If
        'DONE: Sending near players in <CENTER CELL>
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Count > 0 Then
            UpdatePlayersInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
        End If
        'DONE: Sending near corpseobjects in <CENTER CELL>
        If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).CorpseObjectsHere.Count > 0 Then
            UpdateCorpseObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
        End If


        If CellXAdd <> 0 Then
            'DONE: Load cell if needed
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY) Is Nothing Then
                MAP_Load(Character.CellX + CellXAdd, Character.CellY, Character.MapID)
            End If
            'DONE: Sending near creatures and gameobjects in <LEFT/RIGHT CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).CreaturesHere.Count > 0 OrElse Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).GameObjectsHere.Count > 0 Then
                UpdateCreaturesAndGameObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
            End If
            'DONE: Sending near players in <LEFT/RIGHT CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).PlayersHere.Count > 0 Then
                UpdatePlayersInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
            End If
            'DONE: Sending near corpseobjects in <LEFT/RIGHT CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).CorpseObjectsHere.Count > 0 Then
                UpdateCorpseObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
            End If
        End If


        If CellYAdd <> 0 Then
            'DONE: Load cell if needed
            If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd) Is Nothing Then
                MAP_Load(Character.CellX, Character.CellY + CellYAdd, Character.MapID)
            End If
            'DONE: Sending near creatures and gameobjects in <TOP/BOTTOM CELL>
            If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).CreaturesHere.Count > 0 OrElse Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).GameObjectsHere.Count > 0 Then
                UpdateCreaturesAndGameObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
            End If
            'DONE: Sending near players in <TOP/BOTTOM CELL>
            If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).PlayersHere.Count > 0 Then
                UpdatePlayersInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
            End If
            'DONE: Sending near corpseobjects in <TOP/BOTTOM CELL>
            If Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).CorpseObjectsHere.Count > 0 Then
                UpdateCorpseObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
            End If
        End If


        If CellYAdd <> 0 AndAlso CellXAdd <> 0 Then
            'DONE: Load cell if needed
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd) Is Nothing Then
                MAP_Load(Character.CellX + CellXAdd, Character.CellY + CellYAdd, Character.MapID)
            End If
            'DONE: Sending near creatures and gameobjects in <CORNER CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).CreaturesHere.Count > 0 OrElse Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).GameObjectsHere.Count > 0 Then
                UpdateCreaturesAndGameObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
            End If
            'DONE: Sending near players in <LEFT/RIGHT CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).PlayersHere.Count > 0 Then
                UpdatePlayersInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
            End If
            'DONE: Sending near corpseobjects in <LEFT/RIGHT CELL>
            If Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).CorpseObjectsHere.Count > 0 Then
                UpdateCorpseObjectsInCell(Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
            End If
        End If

        Character.SendOutOfRangeUpdate()
        'Log.WriteLine(LogType.DEBUG, "Update: {0}ms", timeGetTime("") - start)
    End Sub

    <MethodImplAttribute(MethodImplOptions.Synchronized)> _
    Public Sub UpdatePlayersInCell(ByRef MapTile As TMapTile, ByRef Character As CharacterObject)
        Dim list() As ULong

        With MapTile
            list = .PlayersHere.ToArray
            For Each GUID As ULong In list

                'DONE: Send to me
                If Not CHARACTERs(GUID).SeenBy.Contains(Character.GUID) Then
                    If Character.CanSee(CHARACTERs(GUID)) Then
                        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                        packet.AddInt32(1)
                        packet.AddInt8(0)
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_PLAYER)
                        CHARACTERs(GUID).FillAllUpdateFlags(tmpUpdate)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(CHARACTERs(GUID), CharacterObject))
                        tmpUpdate.Dispose()
                        Character.Client.Send(packet)
                        packet.Dispose()

                        CHARACTERs(GUID).SeenBy.Add(Character.GUID)
                        Character.playersNear.Add(GUID)
                    End If
                End If
                'DONE: Send to him
                If Not Character.SeenBy.Contains(GUID) Then
                    If CHARACTERs(GUID).CanSee(Character) Then
                        Dim myPacket As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                        myPacket.AddInt32(1)
                        myPacket.AddInt8(0)
                        Dim myTmpUpdate As New UpdateClass(FIELD_MASK_SIZE_PLAYER)
                        Character.FillAllUpdateFlags(myTmpUpdate)
                        myTmpUpdate.AddToPacket(myPacket, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Character)
                        myTmpUpdate.Dispose()
                        CHARACTERs(GUID).Client.Send(myPacket)
                        myPacket.Dispose()

                        Character.SeenBy.Add(GUID)
                        CHARACTERs(GUID).playersNear.Add(Character.GUID)
                    End If
                End If
            Next
        End With
    End Sub
    Public Sub UpdateCreaturesAndGameObjectsInCell(ByRef MapTile As TMapTile, ByRef Character As CharacterObject)
        Dim list() As ULong
        Dim packet As New UpdatePacketClass

        With MapTile
            list = .CreaturesHere.ToArray
            For Each GUID As ULong In list
                If Not Character.creaturesNear.Contains(GUID) AndAlso WORLD_CREATUREs.ContainsKey(GUID) Then
                    If Character.CanSee(WORLD_CREATUREs(GUID)) Then
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_UNIT)
                        WORLD_CREATUREs(GUID).FillAllUpdateFlags(tmpUpdate)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, WORLD_CREATUREs(GUID))
                        tmpUpdate.Dispose()

                        Character.creaturesNear.Add(GUID)
                        WORLD_CREATUREs(GUID).SeenBy.Add(Character.GUID)
                    End If
                End If
            Next

            list = .GameObjectsHere.ToArray
            For Each GUID As ULong In list
                If Not Character.gameObjectsNear.Contains(GUID) Then
                    If GuidIsMoTransport(GUID) Then
                        If Character.CanSee(WORLD_TRANSPORTs(GUID)) Then
                            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                            WORLD_TRANSPORTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, WORLD_TRANSPORTs(GUID))
                            tmpUpdate.Dispose()

                            Character.gameObjectsNear.Add(GUID)
                            WORLD_TRANSPORTs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    Else
                        If Character.CanSee(WORLD_GAMEOBJECTs(GUID)) Then
                            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                            WORLD_GAMEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, WORLD_GAMEOBJECTs(GUID))
                            tmpUpdate.Dispose()

                            Character.gameObjectsNear.Add(GUID)

                            WORLD_GAMEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                End If
            Next

            list = .DynamicObjectsHere.ToArray
            For Each GUID As ULong In list
                If Not Character.dynamicObjectsNear.Contains(GUID) Then
                    If Character.CanSee(WORLD_DYNAMICOBJECTs(GUID)) Then
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_DYNAMICOBJECT)
                        WORLD_DYNAMICOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF, WORLD_DYNAMICOBJECTs(GUID))
                        tmpUpdate.Dispose()

                        Character.dynamicObjectsNear.Add(GUID)

                        WORLD_DYNAMICOBJECTs(GUID).SeenBy.Add(Character.GUID)
                    End If
                End If
            Next
        End With

        'DONE: Send creatures, game objects and dynamic objects in the same packet
        If packet.UpdatesCount > 0 Then
            packet.CompressUpdatePacket()
            Character.Client.Send(packet)
        End If
        packet.Dispose()
    End Sub
    Public Sub UpdateCreaturesInCell(ByRef MapTile As TMapTile, ByRef Character As CharacterObject)
        Dim list() As ULong

        With MapTile
            List = .CreaturesHere.ToArray
            For Each GUID As ULong In List

                If Not Character.creaturesNear.Contains(GUID) Then
                    If Character.CanSee(WORLD_CREATUREs(GUID)) Then
                        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                        packet.AddInt32(1)
                        packet.AddInt8(0)
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_UNIT)
                        WORLD_CREATUREs(GUID).FillAllUpdateFlags(tmpUpdate)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(WORLD_CREATUREs(GUID), CreatureObject))
                        tmpUpdate.Dispose()
                        Character.Client.Send(packet)
                        packet.Dispose()

                        Character.creaturesNear.Add(GUID)
                        WORLD_CREATUREs(GUID).SeenBy.Add(Character.GUID)
                    End If
                End If
            Next
        End With
    End Sub
    Public Sub UpdateGameObjectsInCell(ByRef MapTile As TMapTile, ByRef Character As CharacterObject)
        With MapTile

            Dim list() As ULong

            List = .GameObjectsHere.ToArray
            For Each GUID As ULong In List

                If Not Character.gameObjectsNear.Contains(GUID) Then
                    If GuidIsGameObject(GUID) AndAlso WORLD_GAMEOBJECTs.ContainsKey(GUID) AndAlso Character.CanSee(WORLD_GAMEOBJECTs(GUID)) Then
                        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                        packet.AddInt32(1)
                        packet.AddInt8(0)
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
                        WORLD_GAMEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(WORLD_GAMEOBJECTs(GUID), GameObjectObject))
                        tmpUpdate.Dispose()
                        Character.Client.Send(packet)
                        packet.Dispose()

                        Character.gameObjectsNear.Add(GUID)

                        WORLD_GAMEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                    End If
                End If
            Next

        End With
    End Sub
    Public Sub UpdateCorpseObjectsInCell(ByRef MapTile As TMapTile, ByRef Character As CharacterObject)
        With MapTile

            Dim list() As ULong

            List = .CorpseObjectsHere.ToArray
            For Each GUID As ULong In List

                If Not Character.corpseObjectsNear.Contains(GUID) Then
                    If Character.CanSee(WORLD_CORPSEOBJECTs(GUID)) Then
                        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                        packet.AddInt32(1)
                        packet.AddInt8(0)
                        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_CORPSE)
                        WORLD_CORPSEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate)
                        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, CType(WORLD_CORPSEOBJECTs(GUID), CorpseObject))
                        tmpUpdate.Dispose()
                        Character.Client.Send(packet)
                        packet.Dispose()

                        Character.corpseObjectsNear.Add(GUID)
                        WORLD_CORPSEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                    End If
                End If
            Next

        End With
    End Sub


#End Region

End Module
