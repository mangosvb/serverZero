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
Imports Mangos.Common
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Globals
Imports Mangos.World.AI
Imports Mangos.World.Globals
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Handlers

    Public Class WS_Handlers_Misc

        'Public Function SelectMonsterSay(ByVal MonsterID As Integer) As String
        '    ' Select Random Text Field From Monster Say HashTable(s)
        '    ' TODO: Allow This To Work With Different Monster Say Events Besides Combat
        '    Dim TextCount As Integer = 0
        '    Dim RandomText As Integer = 0

        '    If Trim((MonsterSayCombat(MonsterID)).Text0) <> "" Then TextCount += 1
        '    If Trim((MonsterSayCombat(MonsterID)).Text1) <> "" Then TextCount += 1
        '    If Trim((MonsterSayCombat(MonsterID)).Text2) <> "" Then TextCount += 1
        '    If Trim((MonsterSayCombat(MonsterID)).Text3) <> "" Then TextCount += 1
        '    If Trim((MonsterSayCombat(MonsterID)).Text4) <> "" Then TextCount += 1

        '    RandomText = Rnd.Next(1, TextCount + 1)

        '    SelectMonsterSay = ""

        '    Select Case RandomText
        '        Case 1
        '            SelectMonsterSay = (MonsterSayCombat(MonsterID)).Text0
        '        Case 2
        '            SelectMonsterSay = (MonsterSayCombat(MonsterID)).Text1
        '        Case 3
        '            SelectMonsterSay = (MonsterSayCombat(MonsterID)).Text2
        '        Case 4
        '            SelectMonsterSay = (MonsterSayCombat(MonsterID)).Text3
        '        Case 5
        '            SelectMonsterSay = (MonsterSayCombat(MonsterID)).Text4
        '    End Select

        'End Function

        Public Sub On_CMSG_NAME_QUERY(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Try
                If (packet.Data.Length - 1) < 13 Then Exit Sub
                packet.GetInt16()
                Dim GUID As ULong = packet.GetUInt64()
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_NAME_QUERY [GUID={2:X}]", client.IP, client.Port, GUID)
                Dim SMSG_NAME_QUERY_RESPONSE As New Packets.PacketClass(OPCODES.SMSG_NAME_QUERY_RESPONSE)

                'RESERVED For Warden Bot
                If GUID = _WS_Commands.SystemGUID Then
                    Try
                        SMSG_NAME_QUERY_RESPONSE.AddUInt64(GUID)
                        SMSG_NAME_QUERY_RESPONSE.AddString(_WS_Commands.SystemNAME)
                        SMSG_NAME_QUERY_RESPONSE.AddInt32(1)
                        SMSG_NAME_QUERY_RESPONSE.AddInt32(1)
                        SMSG_NAME_QUERY_RESPONSE.AddInt32(1)
                        client.Send(SMSG_NAME_QUERY_RESPONSE)
                    Finally
                        SMSG_NAME_QUERY_RESPONSE.Dispose()
                    End Try
                    Exit Sub
                End If

                'Asking for player name
                If _CommonGlobalFunctions.GuidIsPlayer(GUID) Then
                    If _WorldServer.CHARACTERs.ContainsKey(GUID) = True Then
                        Try
                            SMSG_NAME_QUERY_RESPONSE.AddUInt64(GUID)
                            SMSG_NAME_QUERY_RESPONSE.AddString(_WorldServer.CHARACTERs(GUID).Name)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(_WorldServer.CHARACTERs(GUID).Race)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(_WorldServer.CHARACTERs(GUID).Gender)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(_WorldServer.CHARACTERs(GUID).Classe)
                            client.Send(SMSG_NAME_QUERY_RESPONSE)
                        Finally
                            SMSG_NAME_QUERY_RESPONSE.Dispose()
                        End Try
                        Exit Sub
                    Else
                        Dim MySQLQuery As New DataTable
                        _WorldServer.CharacterDatabase.Query(String.Format("SELECT char_name, char_race, char_class, char_gender FROM characters WHERE char_guid = ""{0}"";", GUID), MySQLQuery)

                        If MySQLQuery.Rows.Count > 0 Then
                            Try
                                SMSG_NAME_QUERY_RESPONSE.AddUInt64(GUID)
                                SMSG_NAME_QUERY_RESPONSE.AddString(CType(MySQLQuery.Rows(0).Item("char_name"), String))
                                SMSG_NAME_QUERY_RESPONSE.AddInt32(MySQLQuery.Rows(0).Item("char_race"))
                                SMSG_NAME_QUERY_RESPONSE.AddInt32(MySQLQuery.Rows(0).Item("char_gender"))
                                SMSG_NAME_QUERY_RESPONSE.AddInt32(MySQLQuery.Rows(0).Item("char_class"))
                                client.Send(SMSG_NAME_QUERY_RESPONSE)
                            Finally
                                SMSG_NAME_QUERY_RESPONSE.Dispose()
                            End Try
                        Else
                            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_NAME_QUERY_RESPONSE [Character GUID={2:X} not found]", client.IP, client.Port, GUID)
                        End If

                        MySQLQuery.Dispose()
                        Exit Sub
                    End If
                End If

                'Asking for creature name (only used in quests?)
                If _CommonGlobalFunctions.GuidIsCreature(GUID) Then
                    If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
                        Try
                            SMSG_NAME_QUERY_RESPONSE.AddUInt64(GUID)
                            SMSG_NAME_QUERY_RESPONSE.AddString(_WorldServer.WORLD_CREATUREs(GUID).Name)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(0)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(0)
                            SMSG_NAME_QUERY_RESPONSE.AddInt32(0)
                            client.Send(SMSG_NAME_QUERY_RESPONSE)
                        Finally
                            SMSG_NAME_QUERY_RESPONSE.Dispose()
                        End Try
                    Else
                        _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_NAME_QUERY_RESPONSE [Creature GUID={2:X} not found]", client.IP, client.Port, GUID)
                    End If
                End If
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.CRITICAL, "Error at name query.{0}", Environment.NewLine & e.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_TUTORIAL_FLAG(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim Flag As Integer = packet.GetInt32()
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TUTORIAL_FLAG [flag={2}]", client.IP, client.Port, Flag)

            client.Character.TutorialFlags((Flag \ 8)) = client.Character.TutorialFlags((Flag \ 8)) + (1 << 7 - (Flag Mod 8))
            client.Character.SaveCharacter()
        End Sub

        Public Sub On_CMSG_TUTORIAL_CLEAR(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TUTORIAL_CLEAR", client.IP, client.Port)

            For i As Integer = 0 To 31
                client.Character.TutorialFlags(i) = 255
            Next
            client.Character.SaveCharacter()
        End Sub

        Public Sub On_CMSG_TUTORIAL_RESET(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TUTORIAL_RESET", client.IP, client.Port)

            For i As Integer = 0 To 31
                client.Character.TutorialFlags(i) = 0
            Next
            client.Character.SaveCharacter()
        End Sub

        Public Sub On_CMSG_TOGGLE_HELM(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TOGGLE_HELM", client.IP, client.Port)

            If (client.Character.cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_HELM) Then
                client.Character.cPlayerFlags = client.Character.cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_HIDE_HELM)
            Else
                client.Character.cPlayerFlags = client.Character.cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_HIDE_HELM
            End If

            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
            client.Character.SendCharacterUpdate(True)
        End Sub

        Public Sub On_CMSG_TOGGLE_CLOAK(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TOGGLE_CLOAK", client.IP, client.Port)

            If (client.Character.cPlayerFlags And PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK) Then
                client.Character.cPlayerFlags = client.Character.cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK)
            Else
                client.Character.cPlayerFlags = client.Character.cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_HIDE_CLOAK
            End If

            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
            client.Character.SendCharacterUpdate(True)
        End Sub

        Public Sub On_CMSG_SET_ACTIONBAR_TOGGLES(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim ActionBar As Byte = packet.GetInt8

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_ACTIONBAR_TOGGLES [{2:X}]", client.IP, client.Port, ActionBar)

            client.Character.cPlayerFieldBytes = (client.Character.cPlayerFieldBytes And &HFFF0FFFF) Or (ActionBar << 16)

            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES, client.Character.cPlayerFieldBytes)
            client.Character.SendCharacterUpdate(True)
        End Sub

        Public Sub On_CMSG_MOUNTSPECIAL_ANIM(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOUNTSPECIAL_ANIM", client.IP, client.Port)

            Dim response As New Packets.PacketClass(OPCODES.SMSG_MOUNTSPECIAL_ANIM)
            Try
                response.AddPackGUID(client.Character.GUID)
                client.Character.SendToNearPlayers(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub On_CMSG_EMOTE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim emoteID As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_EMOTE [{2}]", client.IP, client.Port, emoteID)

            Dim response As New Packets.PacketClass(OPCODES.SMSG_EMOTE)
            Try
                response.AddInt32(emoteID)
                response.AddUInt64(client.Character.GUID)
                client.Character.SendToNearPlayers(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub On_CMSG_TEXT_EMOTE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 21 Then Exit Sub
            packet.GetInt16()
            Dim TextEmote As Integer = packet.GetInt32
            Dim Unk As Integer = packet.GetInt32
            Dim GUID As ULong = packet.GetUInt64

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TEXT_EMOTE [TextEmote={2} Unk={3}]", client.IP, client.Port, TextEmote, Unk)

            If _CommonGlobalFunctions.GuidIsCreature(GUID) AndAlso _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
                'DONE: Some quests needs emotes being done
                _WorldServer.ALLQUESTS.OnQuestDoEmote(client.Character, _WorldServer.WORLD_CREATUREs(GUID), TextEmote)

                'DONE: Doing emotes to guards
                If _WorldServer.WORLD_CREATUREs(GUID).aiScript IsNot Nothing AndAlso (TypeOf _WorldServer.WORLD_CREATUREs(GUID).aiScript Is WS_Creatures_AI.GuardAI) Then
                    CType(_WorldServer.WORLD_CREATUREs(GUID).aiScript, WS_Creatures_AI.GuardAI).OnEmote(TextEmote)
                End If
            End If

            'DONE: Send Emote animation
            If _WS_DBCDatabase.EmotesText.ContainsKey(TextEmote) Then
                If _WS_DBCDatabase.EmotesState(_WS_DBCDatabase.EmotesText(TextEmote)) = 0 Then
                    client.Character.DoEmote(_WS_DBCDatabase.EmotesText(TextEmote))
                ElseIf _WS_DBCDatabase.EmotesState(_WS_DBCDatabase.EmotesText(TextEmote)) = 2 Then
                    client.Character.cEmoteState = _WS_DBCDatabase.EmotesText(TextEmote)
                    client.Character.SetUpdateFlag(EUnitFields.UNIT_NPC_EMOTESTATE, client.Character.cEmoteState)
                    client.Character.SendCharacterUpdate(True)
                End If
            End If

            'DONE: Find Creature/Player with the recv GUID
            Dim secondName As String = ""
            If GUID > 0 Then
                If _WorldServer.CHARACTERs.ContainsKey(GUID) Then
                    secondName = _WorldServer.CHARACTERs(GUID).Name
                ElseIf _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) Then
                    secondName = _WorldServer.WORLD_CREATUREs(GUID).Name
                End If
            End If

            Dim SMSG_TEXT_EMOTE As New Packets.PacketClass(OPCODES.SMSG_TEXT_EMOTE)
            Try
                SMSG_TEXT_EMOTE.AddUInt64(client.Character.GUID)
                SMSG_TEXT_EMOTE.AddInt32(TextEmote)
                SMSG_TEXT_EMOTE.AddInt32(&HFF)
                SMSG_TEXT_EMOTE.AddInt32(secondName.Length + 1)
                SMSG_TEXT_EMOTE.AddString(secondName)
                client.Character.SendToNearPlayers(SMSG_TEXT_EMOTE)
            Finally
                SMSG_TEXT_EMOTE.Dispose()
            End Try
        End Sub

        Public Sub On_MSG_CORPSE_QUERY(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If client.Character.corpseGUID = 0 Then Exit Sub

            'TODO: Find out the proper structure of this packet or at least what is wrong with instances!

            'DONE: Send corpse coords
            Dim MSG_CORPSE_QUERY As New Packets.PacketClass(OPCODES.MSG_CORPSE_QUERY)
            Try
                MSG_CORPSE_QUERY.AddInt8(1)
                ''MSG_CORPSE_QUERY.AddInt32(Client.Character.corpseMapID)
                MSG_CORPSE_QUERY.AddInt32(client.Character.MapID) ' Without changing this from the above line, the corpse pointer on the minimap did not show
                ' when I was on a different map at least.
                MSG_CORPSE_QUERY.AddSingle(client.Character.corpsePositionX)
                MSG_CORPSE_QUERY.AddSingle(client.Character.corpsePositionY)
                MSG_CORPSE_QUERY.AddSingle(client.Character.corpsePositionZ)
                ''If client.Character.corpseMapID <> client.Character.MapID Then
                ''    MSG_CORPSE_QUERY.AddInt32(0)                '1-Normal 0-Corpse in instance
                ''Else
                ''    MSG_CORPSE_QUERY.AddInt32(1)                '1-Normal 0-Corpse in instance
                ''End If
                MSG_CORPSE_QUERY.AddInt32(client.Character.corpseMapID) ' This change from the above lines, gets rid of the "You must enter the instance to recover your corpse."
                ' message when you did not die in an instance, although I did not see it when I did die in the instance either, but I did rez upon reentrance into the instance.
                client.Send(MSG_CORPSE_QUERY)
            Finally
                MSG_CORPSE_QUERY.Dispose()
            End Try

            'DONE: Send ping on minimap
            Dim MSG_MINIMAP_PING As New Packets.PacketClass(OPCODES.MSG_MINIMAP_PING)
            Try
                MSG_MINIMAP_PING.AddUInt64(client.Character.corpseGUID)
                MSG_MINIMAP_PING.AddSingle(client.Character.corpsePositionX)
                MSG_MINIMAP_PING.AddSingle(client.Character.corpsePositionY)
                client.Send(MSG_MINIMAP_PING)
            Finally
                MSG_MINIMAP_PING.Dispose()
            End Try
        End Sub

        Public Sub On_CMSG_REPOP_REQUEST(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_REPOP_REQUEST [GUID={2:X}]", client.IP, client.Port, client.Character.GUID)
            If client.Character.repopTimer IsNot Nothing Then
                client.Character.repopTimer.Dispose()
                client.Character.repopTimer = Nothing
            End If
            CharacterRepop(client)
        End Sub

        Public Sub On_CMSG_RECLAIM_CORPSE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_RECLAIM_CORPSE [GUID={2:X}]", client.IP, client.Port, GUID)

            CharacterResurrect(client.Character)
        End Sub

        Public Sub CharacterRepop(ByRef client As WS_Network.ClientClass)
            Try
                'DONE: Make really dead
                With client.Character
                    .Mana.Current = 0
                    .Rage.Current = 0
                    .Energy.Current = 0
                    .Life.Current = 1
                    .DEAD = True
                    .cUnitFlags = &H8
                    .cDynamicFlags = 0
                    .cPlayerFlags = client.Character.cPlayerFlags Or PlayerFlags.PLAYER_FLAGS_DEAD
                End With
                _Functions.SendCorpseReclaimDelay(client, client.Character, 30)

                'DONE: Clear some things like spells, flags and timers
                client.Character.StopMirrorTimer(MirrorTimer.FATIGUE)
                client.Character.StopMirrorTimer(MirrorTimer.DROWNING)
                If client.Character.underWaterTimer IsNot Nothing Then
                    client.Character.underWaterTimer.Dispose()
                    client.Character.underWaterTimer = Nothing
                End If

                'DONE: Spawn Corpse
                Dim myCorpse As New WS_Corpses.CorpseObject(client.Character)
                myCorpse.Save()
                myCorpse.AddToWorld()

                'DONE: Update to see only dead
                client.Character.Invisibility = InvisibilityLevel.DEAD
                client.Character.CanSeeInvisibility = InvisibilityLevel.DEAD
                _WS_CharMovement.UpdateCell(client.Character)

                'DONE: Remove all auras
                For i As Integer = 0 To _Global_Constants.MAX_AURA_EFFECTs - 1
                    If client.Character.ActiveSpells(i) IsNot Nothing Then client.Character.RemoveAura(i, client.Character.ActiveSpells(i).SpellCaster)
                Next

                'DONE: Ghost aura
                client.Character.SetWaterWalk()
                client.Character.SetMoveUnroot()
                If client.Character.Race = Races.RACE_NIGHT_ELF Then
                    client.Character.ApplySpell(20584)
                Else
                    client.Character.ApplySpell(8326)
                End If

                client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, 1)
                client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_POWER1 + client.Character.ManaType, 0)
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
                client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, client.Character.cUnitFlags)
                client.Character.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, client.Character.cDynamicFlags)

                client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, &H1000000)       'Set standing so always be standing
                client.Character.SendCharacterUpdate()

                'DONE: Get closest graveyard
                _WorldServer.AllGraveYards.GoToNearestGraveyard(client.Character, False, True)
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Error on repop: {0}", e.ToString)
            End Try
        End Sub

        Public Sub CharacterResurrect(ByRef Character As WS_PlayerData.CharacterObject)
            If Character.repopTimer IsNot Nothing Then
                Character.repopTimer.Dispose()
                Character.repopTimer = Nothing
            End If

            'DONE: Make really alive
            Character.Mana.Current = 0
            Character.Rage.Current = 0
            Character.Energy.Current = 0
            Character.Life.Current = Character.Life.Maximum / 2
            Character.DEAD = False
            Character.cPlayerFlags = Character.cPlayerFlags And (Not PlayerFlags.PLAYER_FLAGS_DEAD)
            Character.cUnitFlags = &H8
            Character.cDynamicFlags = 0

            'DONE: Update to see only alive
            Character.InvisibilityReset()
            _WS_CharMovement.UpdateCell(Character)
            Character.SetLandWalk()

            If Character.Race = Races.RACE_NIGHT_ELF Then
                Character.RemoveAuraBySpell(20584)
            Else
                Character.RemoveAuraBySpell(8326)
            End If

            Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, Character.Life.Current)
            Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, Character.cPlayerFlags)
            Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Character.cUnitFlags)
            Character.SetUpdateFlag(EUnitFields.UNIT_DYNAMIC_FLAGS, Character.cDynamicFlags)
            Character.SendCharacterUpdate()

            'DONE: Spawn Bones, Delete Corpse
            If Character.corpseGUID <> 0 Then
                If _WorldServer.WORLD_CORPSEOBJECTs.ContainsKey(Character.corpseGUID) Then
                    _WorldServer.WORLD_CORPSEOBJECTs(Character.corpseGUID).ConvertToBones()
                Else
                    _WorldServer.Log.WriteLine(LogType.DEBUG, "Corpse wasn't found [{0}]!", Character.corpseGUID - _Global_Constants.GUID_CORPSE)

                    'DONE: Delete from database
                    _WorldServer.CharacterDatabase.Update(String.Format("DELETE FROM corpse WHERE player = ""{0}"";", Character.GUID))

                    'TODO: Turn the corpse into bones on the server it is located at!
                End If
                Character.corpseGUID = 0
                Character.corpseMapID = 0
                Character.corpsePositionX = 0
                Character.corpsePositionY = 0
                Character.corpsePositionZ = 0
            End If
        End Sub

        Public Sub On_CMSG_TOGGLE_PVP(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TOGGLE_PVP", client.IP, client.Port)

            client.Character.isPvP = Not client.Character.isPvP
            client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, client.Character.cUnitFlags)
            client.Character.SendCharacterUpdate()
        End Sub
        Public Sub On_MSG_INSPECT_HONOR_STATS(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_INSPECT_HONOR_STATS [{2:X}]", client.IP, client.Port, GUID)
            If _WorldServer.CHARACTERs.ContainsKey(GUID) = False Then Exit Sub

            Dim response As New Packets.PacketClass(OPCODES.MSG_INSPECT_HONOR_STATS)
            Try
                response.AddUInt64(GUID)

                _WorldServer.CHARACTERs_Lock.AcquireReaderLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                response.AddInt8(_WorldServer.CHARACTERs(GUID).HonorRank)                                                            'Highest Rank
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorKillsToday + CInt(_WorldServer.CHARACTERs(GUID).DishonorKillsToday) << 16)   'PLAYER_FIELD_SESSION_KILLS                - Today Honorable and Dishonorable Kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorKillsYesterday)                                                 'PLAYER_FIELD_YESTERDAY_KILLS              - Yesterday Honorable Kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorKillsLastWeek)                                                  'PLAYER_FIELD_LAST_WEEK_KILLS              - Last Week Honorable Kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorKillsThisWeek)                                                  'PLAYER_FIELD_THIS_WEEK_KILLS              - This Week Honorable kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorKillsLifeTime)                                                  'PLAYER_FIELD_LIFETIME_HONORABLE_KILLS     - Lifetime Honorable Kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).DishonorKillsLifeTime)                                               'PLAYER_FIELD_LIFETIME_DISHONORABLE_KILLS  - Lifetime Dishonorable Kills
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorPointsYesterday)                                                'PLAYER_FIELD_YESTERDAY_CONTRIBUTION       - Yesterday Honor
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorPointsLastWeek)                                                 'PLAYER_FIELD_LAST_WEEK_CONTRIBUTION       - Last Week Honor
                response.AddInt32(_WorldServer.CHARACTERs(GUID).HonorPointsThisWeek)                                                 'PLAYER_FIELD_THIS_WEEK_CONTRIBUTION       - This Week Honor
                response.AddInt32(_WorldServer.CHARACTERs(GUID).StandingLastWeek)                                                    'PLAYER_FIELD_LAST_WEEK_RANK               - Last Week Standing
                response.AddInt8(_WorldServer.CHARACTERs(GUID).HonorHighestRank)                                                     '?!
                _WorldServer.CHARACTERs_Lock.ReleaseReaderLock()

                client.Send(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub On_CMSG_MOVE_FALL_RESET(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOVE_FALL_RESET", client.IP, client.Port)
            _Packets.DumpPacket(packet.Data)
        End Sub
        Public Sub On_CMSG_BATTLEFIELD_STATUS(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEFIELD_STATUS", client.IP, client.Port)
        End Sub
        Public Sub On_CMSG_SET_ACTIVE_MOVER(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_ACTIVE_MOVER [GUID={2:X}]", client.IP, client.Port, GUID)
        End Sub
        Public Sub On_CMSG_MEETINGSTONE_INFO(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MEETINGSTONE_INFO", client.IP, client.Port)
        End Sub

        Public Sub On_CMSG_SET_FACTION_ATWAR(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim faction As Integer = packet.GetInt32
            Dim enabled As Byte = packet.GetInt8
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_FACTION_ATWAR [faction={2:X} enabled={3}]", client.IP, client.Port, faction, enabled)
            If enabled > 1 Then Exit Sub

            If enabled = 1 Then
                client.Character.Reputation(faction).Flags = client.Character.Reputation(faction).Flags Or 2
            Else
                client.Character.Reputation(faction).Flags = client.Character.Reputation(faction).Flags And (Not 2)
            End If

            Dim response As New Packets.PacketClass(OPCODES.SMSG_SET_FACTION_STANDING)
            Try
                response.AddInt32(client.Character.Reputation(faction).Flags)
                response.AddInt32(faction)
                response.AddInt32(client.Character.Reputation(faction).Value)
                client.Send(response)
            Finally
                response.Dispose()
            End Try
        End Sub
        Public Sub On_CMSG_SET_FACTION_INACTIVE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim faction As Integer = packet.GetInt32
            Dim enabled As Byte = packet.GetInt8
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_FACTION_INACTIVE [faction={2:X} enabled={3}]", client.IP, client.Port, faction, enabled)
            If enabled > 1 Then Exit Sub
        End Sub
        Public Sub On_CMSG_SET_WATCHED_FACTION(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim faction As Integer = packet.GetInt32
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_WATCHED_FACTION [faction={2:X}]", client.IP, client.Port, faction)
            If faction = -1 Then faction = &HFF
            If faction < 0 OrElse faction > 255 Then Exit Sub

            client.Character.WatchedFactionIndex = faction
            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_WATCHED_FACTION_INDEX, faction)
            client.Character.SendCharacterUpdate(False)
        End Sub

        Public Sub On_MSG_PVP_LOG_DATA(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            ' TODO: Implement this packet - As far as I know, this packet only applys if the character is in a battleground.
            If (Not _WS_Maps.Maps(client.Character.MapID).IsBattleGround) Then Return
        End Sub

    End Class
End Namespace