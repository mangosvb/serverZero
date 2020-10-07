'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Imports System.Data
Imports System.Runtime.CompilerServices
Imports Mangos.Common
Imports Mangos.Common.Enums.Faction
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Enums.Spell
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.AI
Imports Mangos.World.Globals
Imports Mangos.World.Maps
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Spells
Imports Mangos.World.Weather

Namespace Handlers

    Public Class WS_CharMovement

#Region "WS.CharacterMovement.MovementHandlers"
        Private Const PId2 As Single = Math.PI / 2
        Private Const PIx2 As Single = 2 * Math.PI

        Public Sub OnMovementPacket(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()

            If client.Character.MindControl IsNot Nothing Then
                OnControlledMovementPacket(packet, client.Character.MindControl, client.Character)
                Exit Sub
            End If

            client.Character.charMovementFlags = packet.GetInt32()
            Dim Time As UInteger = packet.GetUInt32()
            client.Character.positionX = packet.GetFloat()
            client.Character.positionY = packet.GetFloat()
            client.Character.positionZ = packet.GetFloat()
            client.Character.orientation = packet.GetFloat()

            'DONE: If character is falling below the world
            If client.Character.positionZ < -500.0F Then
                _WorldServer.AllGraveYards.GoToNearestGraveyard(client.Character, False, True)
                Exit Sub
            End If

            If client.Character.Pet IsNot Nothing Then
                If client.Character.Pet.FollowOwner Then
                    Dim angle As Single = client.Character.orientation - PId2
                    If angle < 0 Then angle += PIx2

                    client.Character.Pet.SetToRealPosition()

                    Dim tmpX As Single = client.Character.positionX + Math.Cos(angle) * 2.0F
                    Dim tmpY As Single = client.Character.positionY + Math.Sin(angle) * 2.0F
                    client.Character.Pet.MoveTo(tmpX, tmpY, client.Character.positionZ, client.Character.orientation, True)
                End If
            End If

#If ENABLE_PPOINTS Then
        If (client.Character.charMovementFlags And groundFlagsMask) = 0 AndAlso _
           Math.Abs(GetZCoord(client.Character.positionX, client.Character.positionY, client.Character.positionZ, client.Character.MapID) - client.Character.positionZ) > PPOINT_LIMIT Then
            _WorldServer.Log.WriteLine(LogType.DEBUG, "PPoints: {0} [MapZ = {1}]", client.Character.positionZ, GetZCoord(client.Character.positionX, client.Character.positionY, client.Character.MapID))
            SetZCoord_PP(client.Character.positionX, client.Character.positionY, client.Character.MapID, client.Character.positionZ)
        End If
#End If

            If (client.Character.charMovementFlags And MovementFlags.MOVEMENTFLAG_ONTRANSPORT) Then
                Dim transportGUID As ULong = packet.GetUInt64
                Dim transportX As Single = packet.GetFloat
                Dim transportY As Single = packet.GetFloat
                Dim transportZ As Single = packet.GetFloat
                Dim transportO As Single = packet.GetFloat

                client.Character.transportX = transportX
                client.Character.transportY = transportY
                client.Character.transportZ = transportZ
                client.Character.transportO = transportO

                'DONE: Boarding transport
                If client.Character.OnTransport Is Nothing Then
                    If _CommonGlobalFunctions.GuidIsMoTransport(transportGUID) AndAlso _WorldServer.WORLD_TRANSPORTs.ContainsKey(transportGUID) Then
                        client.Character.OnTransport = _WorldServer.WORLD_TRANSPORTs(transportGUID)

                        'DONE: Unmount when boarding
                        client.Character.RemoveAurasOfType(AuraEffects_Names.SPELL_AURA_MOUNTED)

                        CType(client.Character.OnTransport, WS_Transports.TransportObject).AddPassenger(client.Character)
                    ElseIf _CommonGlobalFunctions.GuidIsTransport(transportGUID) AndAlso _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(transportGUID) Then
                        client.Character.OnTransport = _WorldServer.WORLD_GAMEOBJECTs(transportGUID)
                    End If
                End If
            ElseIf client.Character.OnTransport IsNot Nothing Then
                'DONE: Unboarding transport
                If TypeOf client.Character.OnTransport Is WS_Transports.TransportObject Then
                    CType(client.Character.OnTransport, WS_Transports.TransportObject).RemovePassenger(client.Character)
                End If
                client.Character.OnTransport = Nothing
            End If

            If (client.Character.charMovementFlags And (MovementFlags.MOVEMENTFLAG_SWIMMING)) Then
                Dim swimAngle As Single = packet.GetFloat
                '#If DEBUG Then
                '                Console.WriteLine("[{0}] [{1}:{2}] Client swim angle:{3}", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port, swimAngle)
                '#End If
            End If

            packet.GetInt32() 'Fall time

            If (client.Character.charMovementFlags And MovementFlags.MOVEMENTFLAG_JUMPING) Then
                Dim airTime As UInteger = packet.GetUInt32
                Dim sinAngle As Single = packet.GetFloat
                Dim cosAngle As Single = packet.GetFloat
                Dim xySpeed As Single = packet.GetFloat
                '#If DEBUG Then
                '                Console.WriteLine("[{0}] [{1}:{2}] Client jump: 0x{3:X} {4} {5} {6}", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port, unk, sinAngle, cosAngle, xySpeed)
                '#End If
            End If

            If (client.Character.charMovementFlags And MovementFlags.MOVEMENTFLAG_SPLINE) Then
                Dim unk1 As Single = packet.GetFloat
            End If

            If client.Character.exploreCheckQueued_ AndAlso (Not client.Character.DEAD) Then
                Dim exploreFlag As Integer = _WS_Maps.GetAreaFlag(client.Character.positionX, client.Character.positionY, client.Character.MapID)

                'DONE: Checking Explore System
                If exploreFlag <> &HFFFF Then
                    Dim areaFlag As Integer = exploreFlag Mod 32
                    Dim areaFlagOffset As Byte = exploreFlag \ 32

                    If Not _Functions.HaveFlag(client.Character.ZonesExplored(areaFlagOffset), areaFlag) Then
                        _Functions.SetFlag(client.Character.ZonesExplored(areaFlagOffset), areaFlag, True)

                        Dim GainedXP As Integer = _WS_Maps.AreaTable(exploreFlag).Level * 10
                        GainedXP = _WS_Maps.AreaTable(exploreFlag).Level * 10

                        Dim SMSG_EXPLORATION_EXPERIENCE As New Packets.PacketClass(OPCODES.SMSG_EXPLORATION_EXPERIENCE)
                        SMSG_EXPLORATION_EXPERIENCE.AddInt32(_WS_Maps.AreaTable(exploreFlag).ID)
                        SMSG_EXPLORATION_EXPERIENCE.AddInt32(GainedXP)
                        client.Send(SMSG_EXPLORATION_EXPERIENCE)
                        SMSG_EXPLORATION_EXPERIENCE.Dispose()

                        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_EXPLORED_ZONES_1 + areaFlagOffset, client.Character.ZonesExplored(areaFlagOffset))
                        client.Character.AddXP(GainedXP, 0, 0, True)

                        'DONE: Fire quest event to check for if this area is used in explore area quest
                        _WorldServer.ALLQUESTS.OnQuestExplore(client.Character, exploreFlag)
                    End If
                End If
            End If

            'If character is moving
            If client.Character.isMoving Then
                'DONE: Stop emotes if moving
                If client.Character.cEmoteState > 0 Then
                    client.Character.cEmoteState = 0
                    client.Character.SetUpdateFlag(EUnitFields.UNIT_NPC_EMOTESTATE, client.Character.cEmoteState)
                    client.Character.SendCharacterUpdate(True)
                End If

                'DONE: Stop casting
                If client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL) IsNot Nothing Then
                    With client.Character.spellCasted(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
                        If .Finished = False And (_WS_Spells.SPELLs(.SpellID).interruptFlags And SpellInterruptFlags.SPELL_INTERRUPT_FLAG_MOVEMENT) Then
                            client.Character.FinishSpell(CurrentSpellTypes.CURRENT_GENERIC_SPELL)
                        End If
                    End With
                End If

                client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_MOVE)
            End If

            'If character is turning
            If client.Character.isTurning Then
                client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TURNING)
            End If

            'DONE: Movement time calculation
            'TODO: PROPERLY MOVE THIS OVER TO THE CMSG_MOVE_TIME_SKIPPED OPCODE, Reference @ LN 406
            Dim MsTime As Integer = _WS_Network.MsTime()
            Dim ClientTimeDelay As Integer = MsTime - Time
            Dim MoveTime As Integer = (Time - (MsTime - ClientTimeDelay)) + 500 + MsTime
            packet.AddInt32(MoveTime, 10)

            'DONE: Send to nearby players
            Dim response As New Packets.PacketClass(packet.OpCode)
            response.AddPackGUID(client.Character.GUID)
            Dim tempArray(packet.Data.Length - 6) As Byte
            Array.Copy(packet.Data, 6, tempArray, 0, packet.Data.Length - 6)
            response.AddByteArray(tempArray)
            client.Character.SendToNearPlayers(response, , False)
            response.Dispose()

            'NOTE: They may slow the movement down so let's do them after the packet is sent
            'DONE: Remove auras that requires you to not move
            If client.Character.isMoving Then
                client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_MOVE)
            End If
            'DONE: Remove auras that requires you to not turn
            If client.Character.isTurning Then
                client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_TURNING)
            End If
        End Sub

        Public Sub OnControlledMovementPacket(ByRef packet As Packets.PacketClass, ByRef Controlled As WS_Base.BaseUnit, ByRef Controller As WS_PlayerData.CharacterObject)
            Dim MovementFlags As Integer = packet.GetInt32()
            Dim Time As UInteger = packet.GetUInt32()
            Dim PositionX As Single = packet.GetFloat()
            Dim PositionY As Single = packet.GetFloat()
            Dim PositionZ As Single = packet.GetFloat()
            Dim Orientation As Single = packet.GetFloat()

            If TypeOf Controlled Is WS_PlayerData.CharacterObject Then
                With CType(Controlled, WS_PlayerData.CharacterObject)
                    .charMovementFlags = MovementFlags
                    .positionX = PositionX
                    .positionY = PositionY
                    .positionZ = PositionZ
                    .orientation = Orientation
                End With
            ElseIf TypeOf Controlled Is WS_Creatures.CreatureObject Then
                With CType(Controlled, WS_Creatures.CreatureObject)
                    .positionX = PositionX
                    .positionY = PositionY
                    .positionZ = PositionZ
                    .orientation = Orientation
                End With
            End If

            'DONE: Movement time calculation
            Dim MsTime As Integer = _WS_Network.MsTime
            Dim ClientTimeDelay As Integer = MsTime - Time
            Dim MoveTime As Integer = (Time - (MsTime - ClientTimeDelay)) + 500 + MsTime
            packet.AddInt32(MoveTime, 10)

            'DONE: Send to nearby players
            Dim response As New Packets.PacketClass(packet.OpCode)
            response.AddPackGUID(Controlled.GUID)
            Dim tempArray(packet.Data.Length - 6) As Byte
            Array.Copy(packet.Data, 6, tempArray, 0, packet.Data.Length - 6)
            response.AddByteArray(tempArray)
            Controlled.SendToNearPlayers(response, Controller.GUID)
            response.Dispose()
        End Sub

        Public Sub OnStartSwim(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            OnMovementPacket(packet, client)

            If client.Character.positionZ < _WS_Maps.GetWaterLevel(client.Character.positionX, client.Character.positionY, client.Character.MapID) Then
                If (client.Character.underWaterTimer Is Nothing) AndAlso (Not client.Character.underWaterBreathing) AndAlso (Not client.Character.DEAD) Then
                    client.Character.underWaterTimer = New WS_PlayerHelper.TDrowningTimer(client.Character)
                End If
            Else
                If client.Character.underWaterTimer IsNot Nothing Then
                    client.Character.underWaterTimer.Dispose()
                    client.Character.underWaterTimer = Nothing
                End If
            End If
        End Sub

        Public Sub OnStopSwim(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If client.Character.underWaterTimer IsNot Nothing Then
                client.Character.underWaterTimer.Dispose()
                client.Character.underWaterTimer = Nothing
            End If

            OnMovementPacket(packet, client)
        End Sub

        Public Sub OnChangeSpeed(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            If GUID <> client.Character.GUID Then Exit Sub 'Skip it, it's not our packet

            packet.GetInt32()
            Dim flags As Integer = packet.GetInt32()
            Dim time As Integer = packet.GetInt32()
            client.Character.positionX = packet.GetFloat()
            client.Character.positionY = packet.GetFloat()
            client.Character.positionZ = packet.GetFloat()
            client.Character.orientation = packet.GetFloat()

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

            Try
                'DONE: Anti hack
                'This doesn't even work anyway, If i'm correct this is suppose to detect when some ones speed changed via abnormal method's and DC the offender.
                'However how would this even work against speeding up the process it's self?
                'At the moment, this just flat out does not work.
                If client.Character.antiHackSpeedChanged_ <= 0 Then
                    Try
                        client.Character.Logout(Nothing)
                        Exit Sub
                    Catch ex As Exception
                        _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] CHEAT: Possible speed hack detected!", client.IP, client.Port)
                    End Try
                End If
            Catch ex As Exception
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] {3} [{2}]", client.IP, client.Port, newSpeed, packet.OpCode)
            End Try

            'DONE: Update speed value and create packet
            client.Character.antiHackSpeedChanged_ -= 1
            Select Case packet.OpCode
                Case OPCODES.CMSG_FORCE_RUN_SPEED_CHANGE_ACK
                    client.Character.RunSpeed = newSpeed
                Case OPCODES.CMSG_FORCE_RUN_BACK_SPEED_CHANGE_ACK
                    client.Character.RunBackSpeed = newSpeed
                Case OPCODES.CMSG_FORCE_SWIM_BACK_SPEED_CHANGE_ACK
                    client.Character.SwimBackSpeed = newSpeed
                Case OPCODES.CMSG_FORCE_SWIM_SPEED_CHANGE_ACK
                    client.Character.SwimSpeed = newSpeed
                Case OPCODES.CMSG_FORCE_TURN_RATE_CHANGE_ACK
                    client.Character.TurnRate = newSpeed
            End Select
        End Sub

        Public Sub SendAreaTriggerMessage(ByRef client As WS_Network.ClientClass, ByVal Text As String)
            Dim p As New Packets.PacketClass(OPCODES.SMSG_AREA_TRIGGER_MESSAGE)
            p.AddInt32(Text.Length)
            p.AddString(Text)
            client.Send(p)
            p.Dispose()
        End Sub

        Public Sub On_CMSG_AREATRIGGER(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Try
                If (packet.Data.Length - 1) < 9 Then Exit Sub
                packet.GetInt16()
                Dim triggerID As Integer = packet.GetInt32
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AREATRIGGER [triggerID={2}]", client.IP, client.Port, triggerID)

                'TODO: Check if in combat?

                Dim q As New DataTable

                'DONE: Handling quest triggers
                q.Clear()
                _WorldServer.WorldDatabase.Query(String.Format("SELECT entry, quest FROM quest_relations WHERE actor=2 and role=0 and entry = {0};", triggerID), q)
                If q.Rows.Count > 0 Then
                    _WorldServer.ALLQUESTS.OnQuestExplore(client.Character, triggerID)
                    Exit Sub
                End If

                'TODO: Handling tavern triggers
                q.Clear()
                _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM areatrigger_tavern WHERE id = {0};", triggerID), q)
                If q.Rows.Count > 0 Then
                    client.Character.cPlayerFlags = client.Character.cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_RESTING
                    client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
                    client.Character.SendCharacterUpdate(True)
                    Exit Sub
                End If

                'DONE: Handling teleport triggers
                q.Clear()
                _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM areatrigger_teleport WHERE id = {0};", triggerID), q)
                If q.Rows.Count > 0 Then
                    Dim posX As Single = q.Rows(0).Item("target_position_x")
                    Dim posY As Single = q.Rows(0).Item("target_position_y")
                    Dim posZ As Single = q.Rows(0).Item("target_position_z")
                    Dim ori As Single = q.Rows(0).Item("target_orientation")
                    Dim tMap As Integer = q.Rows(0).Item("target_map")
                    Dim reqLevel As Byte = q.Rows(0).Item("required_level")

                    If client.Character.DEAD Then
                        If client.Character.corpseMapID = tMap Then
                            _WS_Handlers_Misc.CharacterResurrect(client.Character)
                        Else
                            _WorldServer.AllGraveYards.GoToNearestGraveyard(client.Character, False, True)
                            Exit Sub
                        End If
                    End If

                    If reqLevel <> 0 AndAlso client.Character.Level < reqLevel Then
                        SendAreaTriggerMessage(client, "Your level is too low")
                        Exit Sub
                    End If


                    If posX <> 0 And posY <> 0 And posZ <> 0 Then
                        client.Character.Teleport(posX, posY, posZ, ori, tMap)
                    End If
                    Exit Sub
                End If

                'DONE: Handling all other scripted triggers
                If Not IsNothing(_WorldServer.AreaTriggers) Then
                    If _WorldServer.AreaTriggers.ContainsMethod("AreaTriggers", String.Format("HandleAreaTrigger_{0}", triggerID)) Then
                        _WorldServer.AreaTriggers.InvokeFunction("AreaTriggers", String.Format("HandleAreaTrigger_{0}", triggerID), New Object() {client.Character.GUID})
                    Else
                        _WorldServer.Log.WriteLine(LogType.WARNING, "[{0}:{1}] AreaTrigger [{2}] not found!", client.IP, client.Port, triggerID)
                    End If
                End If
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.CRITICAL, "Error when entering areatrigger.{0}", Environment.NewLine & e.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_MOVE_TIME_SKIPPED(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            'TODO: Figure out why this is causing a freeze everytime the packet is called, Reference @ LN 180

            'packet.GetUInt64()
            'packet.GetUInt32()
            'Dim MsTime As Integer = WS_Network.msTime()
            'Dim ClientTimeDelay As Integer = MsTime - MsTime
            'Dim MoveTime As Integer = (MsTime - (MsTime - ClientTimeDelay)) + 500 + MsTime
            'packet.AddInt32(MoveTime, 10)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOVE_TIME_SKIPPED", client.IP, client.Port)
        End Sub
        Public Sub On_MSG_MOVE_FALL_LAND(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Try
                OnMovementPacket(packet, client)

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
                If FallTime > 1100 AndAlso (Not client.Character.DEAD) AndAlso client.Character.positionZ > _WS_Maps.GetWaterLevel(client.Character.positionX, client.Character.positionY, client.Character.MapID) Then
                    If client.Character.HaveAuraType(AuraEffects_Names.SPELL_AURA_FEATHER_FALL) = False Then
                        Dim safe_fall As Integer = client.Character.GetAuraModifier(AuraEffects_Names.SPELL_AURA_SAFE_FALL)
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
                            Dim FallDamage As Integer = (FallPerc * FallPerc - 1) / 9 * client.Character.Life.Maximum

                            If FallDamage > 0 Then
                                'Prevent the fall damage to be more than your maximum health
                                If FallDamage > client.Character.Life.Maximum Then FallDamage = client.Character.Life.Maximum
                                'Deal the damage
                                client.Character.LogEnvironmentalDamage(EnvironmentalDamage.DAMAGE_FALL, FallDamage)
                                client.Character.DealDamage(FallDamage)

#If DEBUG Then
                                _WorldServer.Log.WriteLine(LogType.USER, "[{0}:{1}] Client fall time: {2}  Damage: {3}", client.IP, client.Port, FallTime, FallDamage)
#End If
                            End If
                        End If
                    End If
                End If

                If Not client.Character.underWaterTimer Is Nothing Then
                    client.Character.underWaterTimer.Dispose()
                    client.Character.underWaterTimer = Nothing
                End If

                If Not client.Character.LogoutTimer Is Nothing Then
                    'DONE: Initialize packet
                    Dim UpdateData As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                    Dim SMSG_UPDATE_OBJECT As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                    Try
                        SMSG_UPDATE_OBJECT.AddInt32(1)      'Operations.Count
                        SMSG_UPDATE_OBJECT.AddInt8(0)

                        'DONE: Disable Turn
                        client.Character.cUnitFlags = client.Character.cUnitFlags Or UnitFlags.UNIT_FLAG_STUNTED
                        UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, client.Character.cUnitFlags)
                        'DONE: StandState -> Sit
                        client.Character.StandState = StandStates.STANDSTATE_SIT
                        UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, client.Character.cBytes1)

                        'DONE: Send packet
                        UpdateData.AddToPacket(SMSG_UPDATE_OBJECT, ObjectUpdateType.UPDATETYPE_VALUES, client.Character)
                        client.Send(SMSG_UPDATE_OBJECT)
                    Finally
                        SMSG_UPDATE_OBJECT.Dispose()
                    End Try

                    Dim packetACK As New Packets.PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                    Try
                        packetACK.AddInt8(StandStates.STANDSTATE_SIT)
                        client.Send(packetACK)
                    Finally
                        packetACK.Dispose()
                    End Try
                End If
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.DEBUG, "Error when falling.{0}", Environment.NewLine & e.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_ZONEUPDATE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim newZone As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ZONEUPDATE [newZone={2}]", client.IP, client.Port, newZone)
            client.Character.ZoneID = newZone
            client.Character.exploreCheckQueued_ = True

            client.Character.ZoneCheck()

            'DONE: Update zone on cluster
            _WorldServer.ClsWorldServer.Cluster.ClientUpdate(client.Index, client.Character.ZoneID, client.Character.Level)

            'DONE: Send weather
            If _WS_Weather.WeatherZones.ContainsKey(newZone) Then
                _WS_Weather.SendWeather(newZone, client)
            End If
        End Sub

        Public Sub On_MSG_MOVE_HEARTBEAT(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            OnMovementPacket(packet, client)

            If client.Character.CellX <> _WS_Maps.GetMapTileX(client.Character.positionX) Or client.Character.CellY <> _WS_Maps.GetMapTileY(client.Character.positionY) Then
                MoveCell(client.Character)
            End If
            UpdateCell(client.Character)

            client.Character.GroupUpdateFlag = client.Character.GroupUpdateFlag Or Globals.Functions.PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POSITION

            client.Character.ZoneCheck()

            'DONE: Check for out of continent - coordinates from WorldMapContinent.dbc
            If _WS_Maps.IsOutsideOfMap((client.Character)) Then
                If client.Character.outsideMapID_ = False Then
                    client.Character.outsideMapID_ = True
                    client.Character.StartMirrorTimer(MirrorTimer.FATIGUE, 30000)
                End If
            Else
                If client.Character.outsideMapID_ = True Then
                    client.Character.outsideMapID_ = False
                    client.Character.StopMirrorTimer(MirrorTimer.FATIGUE)
                End If
            End If

            'DONE: Duel check
            If client.Character.IsInDuel Then _WS_Spells.CheckDuelDistance(client.Character)

            'DONE: Aggro range
            For Each cGUID As ULong In client.Character.creaturesNear.ToArray
                If _WorldServer.WORLD_CREATUREs.ContainsKey(cGUID) AndAlso _WorldServer.WORLD_CREATUREs(cGUID).aiScript IsNot Nothing AndAlso ((TypeOf _WorldServer.WORLD_CREATUREs(cGUID).aiScript Is WS_Creatures_AI.DefaultAI) OrElse (TypeOf _WorldServer.WORLD_CREATUREs(cGUID).aiScript Is WS_Creatures_AI.GuardAI)) Then
                    If _WorldServer.WORLD_CREATUREs(cGUID).IsDead = False AndAlso _WorldServer.WORLD_CREATUREs(cGUID).aiScript.InCombat() = False Then
                        If client.Character.inCombatWith.Contains(cGUID) Then Continue For
                        If client.Character.GetReaction(_WorldServer.WORLD_CREATUREs(cGUID).Faction) = TReaction.HOSTILE AndAlso _WS_Combat.GetDistance(_WorldServer.WORLD_CREATUREs(cGUID), client.Character) <= _WorldServer.WORLD_CREATUREs(cGUID).AggroRange(client.Character) Then
                            _WorldServer.WORLD_CREATUREs(cGUID).aiScript.OnGenerateHate(client.Character, 1)
                            client.Character.AddToCombat(_WorldServer.WORLD_CREATUREs(cGUID))
                            _WorldServer.WORLD_CREATUREs(cGUID).aiScript.State = AIState.AI_ATTACKING
                            _WorldServer.WORLD_CREATUREs(cGUID).aiScript.DoThink()
                        End If
                    End If
                End If
            Next

            'DONE: Creatures that are following you will have a more smooth movement
            For Each CombatUnit As ULong In client.Character.inCombatWith.ToArray
                If _CommonGlobalFunctions.GuidIsCreature(CombatUnit) AndAlso _WorldServer.WORLD_CREATUREs.ContainsKey(CombatUnit) AndAlso _WorldServer.WORLD_CREATUREs(CombatUnit).aiScript IsNot Nothing Then
                    With _WorldServer.WORLD_CREATUREs(CombatUnit)
                        If (Not .aiScript.aiTarget Is Nothing) AndAlso .aiScript.aiTarget Is client.Character Then
                            .SetToRealPosition() 'Make sure it moves from it's location and not from where it was already heading before this
                            .aiScript.State = AIState.AI_MOVE_FOR_ATTACK
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
                        If _WS_Maps.Maps(Map).TileUsed(x + i, y + j) = False Then
                            _WorldServer.Log.WriteLine(LogType.INFORMATION, "Loading map [{2}: {0},{1}]...", x + i, y + j, Map)
                            _WS_Maps.Maps(Map).TileUsed(x + i, y + j) = True
                            _WS_Maps.Maps(Map).Tiles(x + i, y + j) = New WS_Maps.TMapTile(x + i, y + j, Map)
                            'DONE: Load spawns
                            _WS_Maps.LoadSpawns(x + i, y + j, Map, 0)
                        End If
                    End If
                Next
            Next
            GC.Collect()
        End Sub

        Public Sub MAP_UnLoad(ByVal x As Byte, ByVal y As Byte, ByVal Map As Integer)
            If _WS_Maps.Maps(Map).Tiles(x, y).PlayersHere.Count = 0 Then
                _WorldServer.Log.WriteLine(LogType.INFORMATION, "Unloading map [{2}: {0},{1}]...", x, y, Map)
                _WS_Maps.Maps(Map).Tiles(x, y).Dispose()
                _WS_Maps.Maps(Map).Tiles(x, y) = Nothing
            End If
        End Sub

        Public Sub AddToWorld(ByRef Character As WS_PlayerData.CharacterObject)
            _WS_Maps.GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)

            'DONE: Dynamic map loading
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then MAP_Load(Character.CellX, Character.CellY, Character.MapID)

            'DONE: Cleanig
            'myPacket.Dispose()
            _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Add(Character.GUID)

            'DONE: Send all creatures and gameobjects to the client
            UpdateCell(Character)

            'DONE: Add the pet to the world as well
            If Character.Pet IsNot Nothing Then
                Character.Pet.Spawn()
            End If
        End Sub

        Public Sub RemoveFromWorld(ByRef Character As WS_PlayerData.CharacterObject)
            If Not _WS_Maps.Maps.ContainsKey(Character.MapID) Then Return

            If Not _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then
                'DONE: Remove character from map
                Try
                    _WS_Maps.GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)
                    _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Remove(Character.GUID)
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "Error removing character {0} from map", Character.Name)
                End Try
            End If

            Dim list() As ULong

            'DONE: Removing from players wich can see it
            list = Character.SeenBy.ToArray
            For Each GUID As ULong In list

                If _WorldServer.CHARACTERs(GUID).playersNear.Contains(Character.GUID) Then
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving.Add(Character.GUID)
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving_Lock.ReleaseWriterLock()

                    _WorldServer.CHARACTERs(GUID).playersNear.Remove(Character.GUID)
                End If
                'DONE: Fully clean
                _WorldServer.CHARACTERs(GUID).SeenBy.Remove(Character.GUID)

            Next
            Character.playersNear.Clear()
            Character.SeenBy.Clear()

            'DONE: Removing from creatures wich can see it
            list = Character.creaturesNear.ToArray
            For Each GUID As ULong In list

                If _WorldServer.WORLD_CREATUREs(GUID).SeenBy.Contains(Character.GUID) Then
                    _WorldServer.WORLD_CREATUREs(GUID).SeenBy.Remove(Character.GUID)
                End If
            Next
            Character.creaturesNear.Clear()

            'DONE: Removing from gameObjects wich can see it
            list = Character.gameObjectsNear.ToArray
            For Each GUID As ULong In list
                If _CommonGlobalFunctions.GuidIsMoTransport(GUID) Then
                    If _WorldServer.WORLD_TRANSPORTs(GUID).SeenBy.Contains(Character.GUID) Then
                        _WorldServer.WORLD_TRANSPORTs(GUID).SeenBy.Remove(Character.GUID)
                    End If
                Else
                    If _WorldServer.WORLD_GAMEOBJECTs(GUID).SeenBy.Contains(Character.GUID) Then
                        _WorldServer.WORLD_GAMEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                    End If
                End If
            Next
            Character.gameObjectsNear.Clear()

            'DONE: Removing from corpseObjects wich can see it
            list = Character.corpseObjectsNear.ToArray
            For Each GUID As ULong In list

                If _WorldServer.WORLD_CORPSEOBJECTs(GUID).SeenBy.Contains(Character.GUID) Then
                    _WorldServer.WORLD_CORPSEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                End If
            Next
            Character.corpseObjectsNear.Clear()

            'DONE: Remove the pet from the world as well
            If Character.Pet IsNot Nothing Then
                Character.Pet.Hide()
            End If
        End Sub

        Public Sub MoveCell(ByRef Character As WS_PlayerData.CharacterObject)
            Dim oldX As Byte = Character.CellX
            Dim oldY As Byte = Character.CellY
            _WS_Maps.GetMapTile(Character.positionX, Character.positionY, Character.CellX, Character.CellY)

            'Map Loading
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then MAP_Load(Character.CellX, Character.CellY, Character.MapID)

            'TODO: Fix map unloading

            If Character.CellX <> oldX Or Character.CellY <> oldY Then
                _WS_Maps.Maps(Character.MapID).Tiles(oldX, oldY).PlayersHere.Remove(Character.GUID)
                'MAP_UnLoad(oldX, oldY)
                _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Add(Character.GUID)
            End If
        End Sub

        Public Sub UpdateCell(ByRef Character As WS_PlayerData.CharacterObject)
            'Dim start As Integer = _NativeMethods.timeGetTime("")
            Dim list() As ULong

            'DONE: Remove players,creatures,objects if dist is >
            list = Character.playersNear.ToArray
            For Each GUID As ULong In list
                If Not Character.CanSee(_WorldServer.CHARACTERs(GUID)) Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    _WorldServer.CHARACTERs(GUID).SeenBy.Remove(Character.GUID)
                    Character.playersNear.Remove(GUID)
                End If
                'Remove me for him
                If (Not _WorldServer.CHARACTERs(GUID).CanSee(Character)) AndAlso Character.SeenBy.Contains(GUID) Then
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving.Add(Character.GUID)
                    _WorldServer.CHARACTERs(GUID).guidsForRemoving_Lock.ReleaseWriterLock()

                    Character.SeenBy.Remove(GUID)
                    _WorldServer.CHARACTERs(GUID).playersNear.Remove(Character.GUID)
                End If
            Next

            list = Character.creaturesNear.ToArray
            For Each GUID As ULong In list
                If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) = False OrElse Character.CanSee(_WorldServer.WORLD_CREATUREs(GUID)) = False Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    _WorldServer.WORLD_CREATUREs(GUID).SeenBy.Remove(Character.GUID)
                    Character.creaturesNear.Remove(GUID)
                End If
            Next

            list = Character.gameObjectsNear.ToArray
            For Each GUID As ULong In list
                If _CommonGlobalFunctions.GuidIsMoTransport(GUID) Then
                    If Not Character.CanSee(_WorldServer.WORLD_TRANSPORTs(GUID)) Then
                        Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        Character.guidsForRemoving.Add(GUID)
                        Character.guidsForRemoving_Lock.ReleaseWriterLock()

                        _WorldServer.WORLD_TRANSPORTs(GUID).SeenBy.Remove(Character.GUID)
                        Character.gameObjectsNear.Remove(GUID)
                    End If
                Else
                    If Not Character.CanSee(_WorldServer.WORLD_GAMEOBJECTs(GUID)) Then
                        Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        Character.guidsForRemoving.Add(GUID)
                        Character.guidsForRemoving_Lock.ReleaseWriterLock()

                        _WorldServer.WORLD_GAMEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                        Character.gameObjectsNear.Remove(GUID)
                    End If
                End If
            Next

            list = Character.dynamicObjectsNear.ToArray
            For Each GUID As ULong In list
                If Not Character.CanSee(_WorldServer.WORLD_DYNAMICOBJECTs(GUID)) Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    _WorldServer.WORLD_DYNAMICOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                    Character.dynamicObjectsNear.Remove(GUID)
                End If
            Next

            list = Character.corpseObjectsNear.ToArray
            For Each GUID As ULong In list

                If Not Character.CanSee(_WorldServer.WORLD_CORPSEOBJECTs(GUID)) Then
                    Character.guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    Character.guidsForRemoving.Add(GUID)
                    Character.guidsForRemoving_Lock.ReleaseWriterLock()

                    _WorldServer.WORLD_CORPSEOBJECTs(GUID).SeenBy.Remove(Character.GUID)
                    Character.corpseObjectsNear.Remove(GUID)
                End If
            Next

            'DONE: Add new if dist is <
            Dim CellXAdd As Short = -1
            Dim CellYAdd As Short = -1
            If _WS_Maps.GetSubMapTileX(Character.positionX) > 32 Then CellXAdd = 1
            If _WS_Maps.GetSubMapTileX(Character.positionY) > 32 Then CellYAdd = 1
            If (Character.CellX + CellXAdd) > 63 Or (Character.CellX + CellXAdd) < 0 Then CellXAdd = 0
            If (Character.CellY + CellYAdd) > 63 Or (Character.CellY + CellYAdd) < 0 Then CellYAdd = 0

            'DONE: Load cell if needed
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY) Is Nothing Then
                MAP_Load(Character.CellX, Character.CellY, Character.MapID)
            End If
            'DONE: Sending near creatures and gameobjects in <CENTER CELL>
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).CreaturesHere.Count > 0 OrElse _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).GameObjectsHere.Count > 0 Then
                UpdateCreaturesAndGameObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
            End If
            'DONE: Sending near players in <CENTER CELL>
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).PlayersHere.Count > 0 Then
                UpdatePlayersInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
            End If
            'DONE: Sending near corpseobjects in <CENTER CELL>
            If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY).CorpseObjectsHere.Count > 0 Then
                UpdateCorpseObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY), Character)
            End If

            If CellXAdd <> 0 Then
                'DONE: Load cell if needed
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY) Is Nothing Then
                    MAP_Load(Character.CellX + CellXAdd, Character.CellY, Character.MapID)
                End If
                'DONE: Sending near creatures and gameobjects in <LEFT/RIGHT CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).CreaturesHere.Count > 0 OrElse _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).GameObjectsHere.Count > 0 Then
                    UpdateCreaturesAndGameObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
                End If
                'DONE: Sending near players in <LEFT/RIGHT CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).PlayersHere.Count > 0 Then
                    UpdatePlayersInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
                End If
                'DONE: Sending near corpseobjects in <LEFT/RIGHT CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY).CorpseObjectsHere.Count > 0 Then
                    UpdateCorpseObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY), Character)
                End If
            End If

            If CellYAdd <> 0 Then
                'DONE: Load cell if needed
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd) Is Nothing Then
                    MAP_Load(Character.CellX, Character.CellY + CellYAdd, Character.MapID)
                End If
                'DONE: Sending near creatures and gameobjects in <TOP/BOTTOM CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).CreaturesHere.Count > 0 OrElse _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).GameObjectsHere.Count > 0 Then
                    UpdateCreaturesAndGameObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
                End If
                'DONE: Sending near players in <TOP/BOTTOM CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).PlayersHere.Count > 0 Then
                    UpdatePlayersInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
                End If
                'DONE: Sending near corpseobjects in <TOP/BOTTOM CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd).CorpseObjectsHere.Count > 0 Then
                    UpdateCorpseObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX, Character.CellY + CellYAdd), Character)
                End If
            End If

            If CellYAdd <> 0 AndAlso CellXAdd <> 0 Then
                'DONE: Load cell if needed
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd) Is Nothing Then
                    MAP_Load(Character.CellX + CellXAdd, Character.CellY + CellYAdd, Character.MapID)
                End If
                'DONE: Sending near creatures and gameobjects in <CORNER CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).CreaturesHere.Count > 0 OrElse _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).GameObjectsHere.Count > 0 Then
                    UpdateCreaturesAndGameObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
                End If
                'DONE: Sending near players in <LEFT/RIGHT CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).PlayersHere.Count > 0 Then
                    UpdatePlayersInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
                End If
                'DONE: Sending near corpseobjects in <LEFT/RIGHT CELL>
                If _WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd).CorpseObjectsHere.Count > 0 Then
                    UpdateCorpseObjectsInCell(_WS_Maps.Maps(Character.MapID).Tiles(Character.CellX + CellXAdd, Character.CellY + CellYAdd), Character)
                End If
            End If

            Character.SendOutOfRangeUpdate()
            '_WorldServer.Log.WriteLine(LogType.DEBUG, "Update: {0}ms", _NativeMethods.timeGetTime("") - start)
        End Sub

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub UpdatePlayersInCell(ByRef MapTile As WS_Maps.TMapTile, ByRef Character As WS_PlayerData.CharacterObject)
            Dim list() As ULong

            With MapTile
                list = .PlayersHere.ToArray
                For Each GUID As ULong In list

                    'DONE: Send to me
                    If Not _WorldServer.CHARACTERs(GUID).SeenBy.Contains(Character.GUID) Then
                        If Character.CanSee(_WorldServer.CHARACTERs(GUID)) Then
                            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            packet.AddInt32(1)
                            packet.AddInt8(0)
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                            _WorldServer.CHARACTERs(GUID).FillAllUpdateFlags(tmpUpdate)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.CHARACTERs(GUID))
                            tmpUpdate.Dispose()
                            Character.client.Send(packet)
                            packet.Dispose()

                            _WorldServer.CHARACTERs(GUID).SeenBy.Add(Character.GUID)
                            Character.playersNear.Add(GUID)
                        End If
                    End If
                    'DONE: Send to him
                    If Not Character.SeenBy.Contains(GUID) Then
                        If _WorldServer.CHARACTERs(GUID).CanSee(Character) Then
                            Dim myPacket As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            myPacket.AddInt32(1)
                            myPacket.AddInt8(0)
                            Dim myTmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                            Character.FillAllUpdateFlags(myTmpUpdate)
                            myTmpUpdate.AddToPacket(myPacket, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Character)
                            myTmpUpdate.Dispose()
                            _WorldServer.CHARACTERs(GUID).client.Send(myPacket)
                            myPacket.Dispose()

                            Character.SeenBy.Add(GUID)
                            _WorldServer.CHARACTERs(GUID).playersNear.Add(Character.GUID)
                        End If
                    End If
                Next
            End With
        End Sub

        Public Sub UpdateCreaturesAndGameObjectsInCell(ByRef MapTile As WS_Maps.TMapTile, ByRef Character As WS_PlayerData.CharacterObject)
            Dim list() As ULong
            Dim packet As New Packets.UpdatePacketClass

            With MapTile
                list = .CreaturesHere.ToArray
                For Each GUID As ULong In list
                    If Not Character.creaturesNear.Contains(GUID) AndAlso _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
                        If Character.CanSee(_WorldServer.WORLD_CREATUREs(GUID)) Then
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_UNIT)
                            _WorldServer.WORLD_CREATUREs(GUID).FillAllUpdateFlags(tmpUpdate)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_CREATUREs(GUID))
                            tmpUpdate.Dispose()

                            Character.creaturesNear.Add(GUID)
                            _WorldServer.WORLD_CREATUREs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                Next

                list = .GameObjectsHere.ToArray
                For Each GUID As ULong In list
                    If Not Character.gameObjectsNear.Contains(GUID) Then
                        If _CommonGlobalFunctions.GuidIsMoTransport(GUID) Then
                            If Character.CanSee(_WorldServer.WORLD_TRANSPORTs(GUID)) Then
                                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_GAMEOBJECT)
                                _WorldServer.WORLD_TRANSPORTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_TRANSPORTs(GUID))
                                tmpUpdate.Dispose()

                                Character.gameObjectsNear.Add(GUID)
                                _WorldServer.WORLD_TRANSPORTs(GUID).SeenBy.Add(Character.GUID)
                            End If
                        Else
                            If Character.CanSee(_WorldServer.WORLD_GAMEOBJECTs(GUID)) Then
                                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_GAMEOBJECT)
                                _WorldServer.WORLD_GAMEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_GAMEOBJECTs(GUID))
                                tmpUpdate.Dispose()

                                Character.gameObjectsNear.Add(GUID)

                                _WorldServer.WORLD_GAMEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                            End If
                        End If
                    End If
                Next

                list = .DynamicObjectsHere.ToArray
                For Each GUID As ULong In list
                    If Not Character.dynamicObjectsNear.Contains(GUID) Then
                        If Character.CanSee(_WorldServer.WORLD_DYNAMICOBJECTs(GUID)) Then
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_DYNAMICOBJECT)
                            _WorldServer.WORLD_DYNAMICOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF, _WorldServer.WORLD_DYNAMICOBJECTs(GUID))
                            tmpUpdate.Dispose()

                            Character.dynamicObjectsNear.Add(GUID)

                            _WorldServer.WORLD_DYNAMICOBJECTs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                Next
            End With

            'DONE: Send creatures, game objects and dynamic objects in the same packet
            If packet.UpdatesCount > 0 Then
                packet.CompressUpdatePacket()
                Character.client.Send(packet)
            End If
            packet.Dispose()
        End Sub

        Public Sub UpdateCreaturesInCell(ByRef MapTile As WS_Maps.TMapTile, ByRef Character As WS_PlayerData.CharacterObject)
            Dim list() As ULong

            With MapTile
                list = .CreaturesHere.ToArray
                For Each GUID As ULong In list

                    If Not Character.creaturesNear.Contains(GUID) Then
                        If Character.CanSee(_WorldServer.WORLD_CREATUREs(GUID)) Then
                            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            packet.AddInt32(1)
                            packet.AddInt8(0)
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_UNIT)
                            _WorldServer.WORLD_CREATUREs(GUID).FillAllUpdateFlags(tmpUpdate)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_CREATUREs(GUID))
                            tmpUpdate.Dispose()
                            Character.client.Send(packet)
                            packet.Dispose()

                            Character.creaturesNear.Add(GUID)
                            _WorldServer.WORLD_CREATUREs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                Next
            End With
        End Sub

        Public Sub UpdateGameObjectsInCell(ByRef MapTile As WS_Maps.TMapTile, ByRef Character As WS_PlayerData.CharacterObject)
            With MapTile

                Dim list() As ULong

                list = .GameObjectsHere.ToArray
                For Each GUID As ULong In list

                    If Not Character.gameObjectsNear.Contains(GUID) Then
                        If _CommonGlobalFunctions.GuidIsGameObject(GUID) AndAlso _WorldServer.WORLD_GAMEOBJECTs.ContainsKey(GUID) AndAlso Character.CanSee(_WorldServer.WORLD_GAMEOBJECTs(GUID)) Then
                            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            packet.AddInt32(1)
                            packet.AddInt8(0)
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_GAMEOBJECT)
                            _WorldServer.WORLD_GAMEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate, Character)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_GAMEOBJECTs(GUID))
                            tmpUpdate.Dispose()
                            Character.client.Send(packet)
                            packet.Dispose()

                            Character.gameObjectsNear.Add(GUID)

                            _WorldServer.WORLD_GAMEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                Next

            End With
        End Sub

        Public Sub UpdateCorpseObjectsInCell(ByRef MapTile As WS_Maps.TMapTile, ByRef Character As WS_PlayerData.CharacterObject)
            With MapTile

                Dim list() As ULong

                list = .CorpseObjectsHere.ToArray
                For Each GUID As ULong In list

                    If Not Character.corpseObjectsNear.Contains(GUID) Then
                        If Character.CanSee(_WorldServer.WORLD_CORPSEOBJECTs(GUID)) Then
                            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                            packet.AddInt32(1)
                            packet.AddInt8(0)
                            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_CORPSE)
                            _WorldServer.WORLD_CORPSEOBJECTs(GUID).FillAllUpdateFlags(tmpUpdate)
                            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, _WorldServer.WORLD_CORPSEOBJECTs(GUID))
                            tmpUpdate.Dispose()
                            Character.client.Send(packet)
                            packet.Dispose()

                            Character.corpseObjectsNear.Add(GUID)
                            _WorldServer.WORLD_CORPSEOBJECTs(GUID).SeenBy.Add(Character.GUID)
                        End If
                    End If
                Next

            End With
        End Sub

#End Region

    End Class
End Namespace