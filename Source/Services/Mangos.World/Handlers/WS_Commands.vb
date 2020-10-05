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
Imports System.Reflection
Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Globals
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Spells
Imports Mangos.World.Weather

Namespace Handlers

    Public Module WS_Commands

        Public Const SystemGUID As ULong = Integer.MaxValue
        Public Const SystemNAME As String = "System"

        Public ChatCommands As New Dictionary(Of String, ChatCommand)
        Public Class ChatCommand
            Public CommandHelp As String
            Public CommandAccess As MiscEnum.AccessLevel = AccessLevel.GameMaster
            Public CommandDelegate As ChatCommandDelegate
        End Class

        Public Delegate Function ChatCommandDelegate(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal Message As String) As Boolean

        Public Sub RegisterChatCommands()
            For Each tmpModule As Type In [Assembly].GetExecutingAssembly.GetTypes
                For Each tmpMethod As MethodInfo In tmpModule.GetMethods
                    Dim infos() As ChatCommandAttribute = tmpMethod.GetCustomAttributes(GetType(ChatCommandAttribute), True)

                    If infos.Length <> 0 Then
                        For Each info As ChatCommandAttribute In infos
                            Dim cmd As New ChatCommand With {
                                    .CommandHelp = info.cmdHelp,
                                    .CommandAccess = info.cmdAccess,
                                    .CommandDelegate = [Delegate].CreateDelegate(GetType(ChatCommandDelegate), tmpMethod)
                                    }

                            ChatCommands.Add(UppercaseFirstLetter(info.cmdName), cmd)
                        Next
                    End If
                Next
            Next

        End Sub

        Public Sub OnCommand(ByRef client As WS_Network.ClientClass, ByVal Message As String)
            Try
                'DONE: Find the command
                Dim tmp() As String = Split(Message, " ", 2)
                Dim Command As ChatCommand = Nothing
                If ChatCommands.ContainsKey(UppercaseFirstLetter(tmp(0))) Then
                    Command = ChatCommands(UppercaseFirstLetter(tmp(0)))
                End If

                'DONE: Build argument string
                Dim Arguments As String = ""
                If tmp.Length = 2 Then Arguments = Trim(tmp(1))

                'DONE: Get character name (there can be no character after the command)
                Dim Name As String = client.Character.Name

                If Command Is Nothing Then
                    client.Character.CommandResponse("Unknown command.")
                ElseIf Command.CommandAccess > client.Character.Access Then
                    client.Character.CommandResponse("This command is not available for your access level.")
                ElseIf Not Command.CommandDelegate(client.Character, Arguments) Then
                    client.Character.CommandResponse(Command.CommandHelp)
                Else
                    Log.WriteLine(LogType.USER, "[{0}:{1}] {2} used command: {3}", client.IP, client.Port, Name, Message)
                End If

            Catch err As Exception
                Log.WriteLine(LogType.FAILED, "[{0}:{1}] Client command caused error! {3}{2}", client.IP, client.Port, err.ToString, Environment.NewLine)
                client.Character.CommandResponse(String.Format("Your command caused error:" & Environment.NewLine & " [{0}]", err.Message))
            End Try
        End Sub

        'Help Command
        <ChatCommand("help", "help #command\r\nDisplays usage information about command, if no command specified - displays list of available commands.", AccessLevel.GameMaster)>
        Public Function Help(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Trim(Message) <> "" Then
                Dim Command As ChatCommand = ChatCommands(Trim(UppercaseFirstLetter(Message)))
                If Command Is Nothing Then
                    objCharacter.CommandResponse("Unknown command.")
                ElseIf Command.CommandAccess > objCharacter.Access Then
                    objCharacter.CommandResponse("This command is not available for your access level.")
                Else
                    objCharacter.CommandResponse(Command.CommandHelp)
                End If
            Else
                Dim cmdList As String = "Listing available commands:" & Environment.NewLine
                For Each Command As KeyValuePair(Of String, ChatCommand) In ChatCommands
                    If Command.Value.CommandAccess <= objCharacter.Access Then cmdList += UppercaseFirstLetter(Command.Key) & Environment.NewLine '", "
                Next
                cmdList += Environment.NewLine + "Use help #command for usage information about particular command."
                objCharacter.CommandResponse(cmdList)
            End If

            Return True
        End Function

        'CastSpell Command
        <ChatCommand("castspell", "castspell #spellid #target - Selected unit will start casting spell. Target can be ME or SELF.", AccessLevel.Developer)>
        Public Function cmdCastSpellMe(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp As String() = Split(Message, " ", 2)
            If tmp.Length < 2 Then Return False
            Dim SpellID As Integer = tmp(0)
            Dim Target As String = UppercaseFirstLetter(tmp(1))

            If GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                Select Case Target
                    Case "ME"
                        WORLD_CREATUREs(objCharacter.TargetGUID).CastSpell(SpellID, objCharacter)
                    Case "SELF"
                        WORLD_CREATUREs(objCharacter.TargetGUID).CastSpell(SpellID, WORLD_CREATUREs(objCharacter.TargetGUID))
                End Select
            ElseIf GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Select Case Target
                    Case "ME"
                        Dim Targets As New WS_Spells.SpellTargets
                        Targets.SetTarget_UNIT(objCharacter)
                        Dim castParams As New CastSpellParameters(Targets, CHARACTERs(objCharacter.TargetGUID), SpellID)
                        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                    Case "SELF"
                        CHARACTERs(objCharacter.TargetGUID).CastOnSelf(SpellID)
                End Select
            Else
                objCharacter.CommandResponse(String.Format("GUID=[{0:X}] not found or unsupported.", objCharacter.TargetGUID))
            End If

            Return True
        End Function

        'Control Command
        <ChatCommand("control", "control - Takes or removes control over the selected unit.", AccessLevel.Admin)>
        Public Function cmdControl(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.MindControl IsNot Nothing Then
                If TypeOf objCharacter.MindControl Is CharacterObject Then
                    Dim packet1 As New Packets.PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                    packet1.AddPackGUID(objCharacter.MindControl.GUID)
                    packet1.AddInt8(1)
                    CType(objCharacter.MindControl, CharacterObject).client.Send(packet1)
                    packet1.Dispose()
                End If

                Dim packet3 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                packet3.AddPackGUID(objCharacter.MindControl.GUID)
                packet3.AddInt8(0)
                objCharacter.client.Send(packet3)
                packet3.Dispose()

                objCharacter.cUnitFlags = objCharacter.cUnitFlags And (Not UnitFlags.UNIT_FLAG_UNK21)
                objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, 0)
                objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
                objCharacter.SendCharacterUpdate(False)

                objCharacter.MindControl = Nothing

                objCharacter.CommandResponse("Removed control over the unit.")
                Return True
            End If

            If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Dim packet1 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                packet1.AddPackGUID(objCharacter.TargetGUID)
                packet1.AddInt8(0)
                CHARACTERs(objCharacter.TargetGUID).client.Send(packet1)
                packet1.Dispose()

                objCharacter.MindControl = CHARACTERs(objCharacter.TargetGUID)
            ElseIf GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                objCharacter.MindControl = WORLD_CREATUREs(objCharacter.TargetGUID)
            Else
                objCharacter.CommandResponse("You need a target.")
                Return True
            End If

            Dim packet2 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
            packet2.AddPackGUID(objCharacter.TargetGUID)
            packet2.AddInt8(1)
            objCharacter.client.Send(packet2)
            packet2.Dispose()

            objCharacter.cUnitFlags = objCharacter.cUnitFlags Or UnitFlags.UNIT_FLAG_UNK21
            objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, objCharacter.TargetGUID)
            objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
            objCharacter.SendCharacterUpdate(False)

            objCharacter.CommandResponse("Taken control over a unit.")

            Return True
        End Function

        'CreateGuild Command - Needs to be implemented
        <ChatCommand("createguild", "createguild #guildname - Creates a guild.", AccessLevel.Developer)>
        Public Function cmdCreateGuild(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            'TODO: Creating guilds must be done in the cluster

            'Dim GuildName As String = Message

            'Dim MySQLQuery As New DataTable
            'CharacterDatabase.Query(String.Format("INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES ('{0}', {1}, {2}, {3}, {4}); SELECT guild_id FROM guilds WHERE guild_name = '{0}';", GuildName, objCharacter.GUID, Now.Year, Now.Month, Now.Day), MySQLQuery)

            'AddCharacterToGuild(objCharacter, MySQLQuery.Rows(0).Item("guild_id"), 0)
            Return True
        End Function

        'Cast Command
        <ChatCommand("cast", "cast #spellid - You will start casting spell on selected target.", AccessLevel.Developer)>
        Public Function cmdCastSpell(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp As String() = Split(Message, " ", 2)
            Dim SpellID As Integer = tmp(0)

            If GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                Dim Targets As New SpellTargets
                Targets.SetTarget_UNIT(WORLD_CREATUREs(objCharacter.TargetGUID))
                Dim castParams As New CastSpellParameters(Targets, objCharacter, SpellID)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

                objCharacter.CommandResponse("You are now casting [" & SpellID & "] at [" & WORLD_CREATUREs(objCharacter.TargetGUID).Name & "].")
            ElseIf GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Dim Targets As New SpellTargets
                Targets.SetTarget_UNIT(CHARACTERs(objCharacter.TargetGUID))
                Dim castParams As New CastSpellParameters(Targets, objCharacter, SpellID)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

                objCharacter.CommandResponse("You are now casting [" & SpellID & "] at [" & CHARACTERs(objCharacter.TargetGUID).Name & "].")
            Else
                objCharacter.CommandResponse(String.Format("GUID=[{0:X}] not found or unsupported.", objCharacter.TargetGUID))
            End If

            Return True
        End Function

        'Save Command
        <ChatCommand("save", "save - Saves selected character.", AccessLevel.Developer)>
        Public Function cmdSave(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).Save()
                CHARACTERs(objCharacter.TargetGUID).CommandResponse(String.Format("Character {0} saved.", CHARACTERs(objCharacter.TargetGUID).Name))
            Else
                objCharacter.Save()
                objCharacter.CommandResponse(String.Format("Character {0} saved.", objCharacter.Name))
            End If

            Return True
        End Function

        'SpawnData Command
        <ChatCommand("spawndata", "spawndata - Tells you the spawn in memory information.", AccessLevel.Developer)>
        Public Function cmdSpawns(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            objCharacter.CommandResponse("Spawns loaded in server memory:")
            objCharacter.CommandResponse("-------------------------------")
            objCharacter.CommandResponse("Creatures: " & WORLD_CREATUREs.Count)
            objCharacter.CommandResponse("GameObjects: " & WORLD_GAMEOBJECTs.Count)

            Return True
        End Function

        'GobjectNear Command
        <ChatCommand("gobjectnear", "gobjectnear - Tells you the near objects count.", AccessLevel.Developer)>
        Public Function cmdNear(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            objCharacter.CommandResponse("Near objects:")
            objCharacter.CommandResponse("-------------------------------")
            objCharacter.CommandResponse("Players: " & objCharacter.playersNear.Count)
            objCharacter.CommandResponse("Creatures: " & objCharacter.creaturesNear.Count)
            objCharacter.CommandResponse("GameObjects: " & objCharacter.gameObjectsNear.Count)
            objCharacter.CommandResponse("Corpses: " & objCharacter.corpseObjectsNear.Count)
            objCharacter.CommandResponse("-------------------------------")
            objCharacter.CommandResponse("You are seen by: " & objCharacter.SeenBy.Count)
            Return True
        End Function

        'NpcAI Command
        <ChatCommand("npcai", "npcai #enable/disable - Enables/Disables  Creature AI updating.", AccessLevel.Developer)>
        Public Function cmdAI(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If UppercaseFirstLetter(Message) = "ENABLE" Then
                AIManager.AIManagerTimer.Change(TAIManager.UPDATE_TIMER, TAIManager.UPDATE_TIMER)
                objCharacter.CommandResponse("AI is enabled.")
            ElseIf UppercaseFirstLetter(Message) = "DISABLE" Then
                AIManager.AIManagerTimer.Change(Timeout.Infinite, Timeout.Infinite)
                objCharacter.CommandResponse("AI is disabled.")
            Else
                Return False
            End If

            Return True
        End Function

        'NpcAIState Command
        <ChatCommand("npcaistate", "npcaistate - Shows debug information about AI state of selected creature.", AccessLevel.Developer)>
        Public Function cmdAIState(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Exit Function
            End If
            If Not WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                objCharacter.CommandResponse("Selected target is not creature!")
                Exit Function
            End If

            If WORLD_CREATUREs(objCharacter.TargetGUID).aiScript Is Nothing Then
                objCharacter.CommandResponse("This creature doesn't have AI")
            Else
                With WORLD_CREATUREs(objCharacter.TargetGUID)
                    objCharacter.CommandResponse(String.Format("Information for creature [{0}]:{1}ai = {2}{1}state = {3}{1}maxdist = {4}", .Name, Environment.NewLine, .aiScript.ToString, .aiScript.State.ToString, .MaxDistance))
                    objCharacter.CommandResponse("Hate table:")
                    For Each u As KeyValuePair(Of WS_Base.BaseUnit, Integer) In .aiScript.aiHateTable
                        objCharacter.CommandResponse(String.Format("{0:X} = {1} hate", u.Key.GUID, u.Value))
                    Next
                End With
            End If

            Return True
        End Function

        'ServerMessage Command
        <ChatCommand("servermessage", "servermessage #type #text - Send text message to all players on the server.", AccessLevel.GameMaster)>
        Public Function cmdServerMessage(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            '1,"[SERVER] Shutdown in %s"
            '2,"[SERVER] Restart in %s"
            '3,"%s"
            '4,"[SERVER] Shutdown cancelled"
            '5,"[SERVER] Restart cancelled"

            Dim tmp() As String = Split(Message, " ", 2)
            If tmp.Length <> 2 Then Return False
            Dim Type As Integer = tmp(0)
            Dim Text As String = tmp(1)

            Dim packet As New PacketClass(OPCODES.SMSG_SERVER_MESSAGE)
            packet.AddInt32(Type)
            packet.AddString(Text)

            packet.UpdateLength()
            ClsWorldServer.Cluster.Broadcast(packet.Data)
            packet.Dispose()

            Return True
        End Function

        'NotifyMessage Command
        <ChatCommand("notifymessage", "notify #message - Send text message to all players on the server.", AccessLevel.GameMaster)>
        Public Function cmdNotificationMessage(ByRef objCharacter As CharacterObject, ByVal Text As String) As Boolean
            If Text = "" Then Return False

            Dim packet As New PacketClass(OPCODES.SMSG_NOTIFICATION)
            packet.AddString(Text)

            packet.UpdateLength()
            ClsWorldServer.Cluster.Broadcast(packet.Data)
            packet.Dispose()

            Return True
        End Function

        'Say Command
        <ChatCommand("say", "say #text - Target NPC will say this.", AccessLevel.GameMaster)>
        Public Function cmdSay(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            If objCharacter.TargetGUID = 0 Then Return False

            If GuidIsCreature(objCharacter.TargetGUID) Then
                WORLD_CREATUREs(objCharacter.TargetGUID).SendChatMessage(Message, ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL, objCharacter.GUID)
            Else
                Return False
            End If

            Return True
        End Function

        'ResetFactions Command
        <ChatCommand("resetfactions", "resetfactions - Resets character reputation standings.", AccessLevel.Admin)>
        Public Function cmdResetFactions(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                InitializeReputations(CHARACTERs(objCharacter.TargetGUID))
                CHARACTERs(objCharacter.TargetGUID).SaveCharacter()
            Else
                InitializeReputations(objCharacter)
                objCharacter.SaveCharacter()
            End If
            Return True
        End Function

        'SkillMaster Command
        <ChatCommand("skillmaster", "skillmaster - Get all spells and skills maxed out for your level.", AccessLevel.Developer)>
        Public Function cmdGetMax(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            'DONE: Max out all skills you know
            For Each skill As KeyValuePair(Of Integer, TSkill) In objCharacter.Skills
                skill.Value.Current = skill.Value.Maximum
                objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + objCharacter.SkillsPositions(skill.Key) * 3 + 1, objCharacter.Skills(skill.Key).GetSkill)
            Next
            objCharacter.SendCharacterUpdate(False)

            'TODO: Add all spells

            Return True
        End Function

        'SetLevel Command
        <ChatCommand("setlevel", "setlevel #level - Set the level of selected character.", AccessLevel.Developer)>
        Public Function cmdSetLevel(ByRef objCharacter As CharacterObject, ByVal tLevel As String) As Boolean
            If IsNumeric(tLevel) = False Then Return False

            Dim Level As Integer = tLevel
            If Level > DEFAULT_MAX_LEVEL Then Level = DEFAULT_MAX_LEVEL
            If Level > 60 Then Level = 60

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) = False Then
                objCharacter.CommandResponse("Target not found or not character.")
                Return True
            End If

            CHARACTERs(objCharacter.TargetGUID).SetLevel(Level)

            Return True
        End Function

        'AddXp Command
        <ChatCommand("addxp", "addxp #amount - Add X experience points to selected character.", AccessLevel.Developer)>
        Public Function cmdAddXP(ByRef objCharacter As CharacterObject, ByVal tXP As String) As Boolean
            If IsNumeric(tXP) = False Then Return False

            Dim XP As Integer = tXP

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).AddXP(XP, 0, 0, True)
            Else
                objCharacter.CommandResponse("Target not found or not character.")
            End If

            Return True
        End Function

        'AddRestedXp Command
        <ChatCommand("addrestedxp", "addrestedxp #amount - Add X rested bonus experience points to selected character.", AccessLevel.Developer)>
        Public Function cmdAddRestedXP(ByRef objCharacter As CharacterObject, ByVal tXP As String) As Boolean
            If IsNumeric(tXP) = False Then Return False

            Dim XP As Integer = tXP

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).RestBonus += XP
                CHARACTERs(objCharacter.TargetGUID).RestState = XPSTATE.Rested

                CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_REST_STATE_EXPERIENCE, CHARACTERs(objCharacter.TargetGUID).RestBonus)
                CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, CHARACTERs(objCharacter.TargetGUID).cPlayerBytes2)
                CHARACTERs(objCharacter.TargetGUID).SendCharacterUpdate()
            Else
                objCharacter.CommandResponse("Target not found or not character.")
            End If

            Return True
        End Function

        'AddHonor Command - Disabled: missing packet data
        '<ChatCommand("addhonor", "addhonor #amount - Add select amount of honor points to selected character.", AccessLevel.Admin)>
        'Public Function cmdAddHonor(ByRef objCharacter As CharacterObject, ByVal tHONOR As String) As Boolean
        '    If IsNumeric(tHONOR) = False Then Return False

        '    Dim Honor As Integer = tHONOR

        '    If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
        '        CHARACTERs(objCharacter.TargetGUID).HonorPoints += Honor
        '        'CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, CHARACTERs(objCharacter.TargetGUID).HonorCurrency)
        '        CHARACTERs(objCharacter.TargetGUID).SendCharacterUpdate(False)
        '    Else
        '        objCharacter.CommandResponse("Target not found or not character.")
        '    End If

        '    Return True
        'End Function

        'PlaySound Command
        <ChatCommand("playsound", "playsound - Plays a specific sound for every player around you.", AccessLevel.Developer)>
        Public Function cmdPlaySound(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim soundID As Integer = 0

            If Integer.TryParse(Message, soundID) = False Then Return False

            objCharacter.SendPlaySound(soundID)

            Return True
        End Function

        'CombatList Command
        <ChatCommand("combatlist", "combatlist - Lists everyone in your targets combatlist.", AccessLevel.Developer)>
        Public Function cmdCombatList(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim combatList() As ULong = {}
            If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) Then
                combatList = CHARACTERs(objCharacter.TargetGUID).inCombatWith.ToArray()
            Else
                combatList = objCharacter.inCombatWith.ToArray()
            End If

            objCharacter.CommandResponse("Combat List (" & combatList.Length & "):")
            For Each Guid As ULong In combatList
                objCharacter.CommandResponse(String.Format("* {0:X}", Guid))
            Next

            Return True
        End Function

        'CoolDownList Command
        <ChatCommand("cooldownlist", "cooldownlist - Lists all cooldowns of your target.", AccessLevel.GameMaster)>
        Public Function cmdCooldownList(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim targetUnit As BaseUnit = Nothing
            If GuidIsPlayer(objCharacter.TargetGUID) Then
                If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then targetUnit = CHARACTERs(objCharacter.TargetGUID)
            ElseIf GuidIsCreature(objCharacter.TargetGUID) Then
                If WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then targetUnit = WORLD_CREATUREs(objCharacter.TargetGUID)
            End If
            If targetUnit Is Nothing Then
                targetUnit = objCharacter
            End If

            If targetUnit Is objCharacter Then
                objCharacter.CommandResponse("Listing your cooldowns:")
            Else
                objCharacter.CommandResponse("Listing cooldowns for [" & targetUnit.UnitName & "]:")
            End If

            If TypeOf targetUnit Is CharacterObject Then
                Dim sCooldowns As String = ""
                Dim timeNow As UInteger = GetTimestamp(Now)
                For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In CType(targetUnit, CharacterObject).Spells
                    If Spell.Value.Cooldown > 0UI Then
                        Dim timeLeft As UInteger = 0
                        If timeNow < Spell.Value.Cooldown Then timeLeft = (Spell.Value.Cooldown - timeNow)
                        If timeLeft > 0 Then
                            sCooldowns &= "* Spell: " & Spell.Key & " - TimeLeft: " & GetTimeLeftString(timeLeft) & " sec" & " - Item: " & Spell.Value.CooldownItem & Environment.NewLine
                        End If
                    End If
                Next
                objCharacter.CommandResponse(sCooldowns)
            Else
                objCharacter.CommandResponse("*Cooldowns not supported for creatures yet*")
            End If

            Return True
        End Function

        'ClearCoolDowns Command
        <ChatCommand("clearcooldowns", "clearcooldowns - Clears all cooldowns of your target.", AccessLevel.Developer)>
        Public Function cmdClearCooldowns(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim targetUnit As BaseUnit = Nothing
            If GuidIsPlayer(objCharacter.TargetGUID) Then
                If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then targetUnit = CHARACTERs(objCharacter.TargetGUID)
            ElseIf GuidIsCreature(objCharacter.TargetGUID) Then
                If WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then targetUnit = WORLD_CREATUREs(objCharacter.TargetGUID)
            End If
            If targetUnit Is Nothing Then
                targetUnit = objCharacter
            End If

            If TypeOf targetUnit Is CharacterObject Then
                Dim timeNow As UInteger = GetTimestamp(Now)
                Dim cooldownSpells As New List(Of Integer)
                For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In CType(targetUnit, CharacterObject).Spells
                    If Spell.Value.Cooldown > 0UI Then
                        Spell.Value.Cooldown = 0UI
                        Spell.Value.CooldownItem = 0UI
                        CharacterDatabase.Update(String.Format("UPDATE characters_spells SET cooldown={2}, cooldownitem={3} WHERE guid = {0} AND spellid = {1};", objCharacter.GUID, Spell.Key, 0, 0))
                        cooldownSpells.Add(Spell.Key)
                    End If
                Next

                For Each SpellID As Integer In cooldownSpells
                    Dim packet As New PacketClass(OPCODES.SMSG_CLEAR_COOLDOWN)
                    packet.AddInt32(SpellID)
                    packet.AddUInt64(targetUnit.GUID)
                    CType(targetUnit, CharacterObject).client.Send(packet)
                    packet.Dispose()
                Next
            Else
                objCharacter.CommandResponse("Cooldowns are not supported for creatures yet.")
            End If

            Return True
        End Function

        'Disabled till warden is finished
        '    <ChatCommand("StartCheck", "STARTCHECK - Initialize Warden anti-cheat engine for selected character.", AccessLevel.Developer)>
        '    Public Function cmdStartCheck(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        '#If WARDEN Then
        '        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
        '            MaievInit(CHARACTERs(objCharacter.TargetGUID))
        '        Else
        '            objCharacter.CommandResponse("No player target selected.")
        '        End If
        '#Else
        '        objCharacter.CommandResponse("Warden is not active.")
        '#End If

        '        Return True
        '    End Function

        '    <ChatCommand("SendCheck", "SENDCHECK - Sends a Warden anti-cheat check packet to the selected character.", AccessLevel.Developer)>
        '    Public Function cmdSendCheck(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        '#If WARDEN Then
        '        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
        '            MaievSendCheck(CHARACTERs(objCharacter.TargetGUID))
        '        Else
        '            objCharacter.CommandResponse("No player target selected.")
        '        End If
        '#Else
        '        objCharacter.CommandResponse("Warden is not active.")
        '#End If

        '        Return True
        '    End Function

        'Additem Command
        <ChatCommand("additem", "additem #itemid #count (optional) - Add chosen items with item amount to selected character.", AccessLevel.GameMaster)>
        Public Function cmdAddItem(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp() As String = Split(Message, " ", 2)
            If tmp.Length < 1 Then Return False

            Dim id As Integer = tmp(0)
            Dim Count As Integer = 1
            If tmp.Length = 2 Then Count = tmp(1)
            If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Dim newItem As New ItemObject(id, objCharacter.TargetGUID) With {
                        .StackCount = Count
                        }

                If CHARACTERs(objCharacter.TargetGUID).ItemADD(newItem) Then
                    CHARACTERs(objCharacter.TargetGUID).LogLootItem(newItem, Count, True, False)
                Else
                    newItem.Delete()
                End If
            Else
                Dim newItem As New ItemObject(id, objCharacter.GUID) With {
                        .StackCount = Count
                        }

                If objCharacter.ItemADD(newItem) Then
                    objCharacter.LogLootItem(newItem, Count, False, True)
                Else
                    newItem.Delete()
                End If
            End If

            Return True
        End Function

        'AddItemSet
        <ChatCommand("additemset", "additemset #item - Add the items in the item set with id X to selected character.", AccessLevel.GameMaster)>
        Public Function cmdAddItemSet(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp() As String = Split(Message, " ", 2)
            If tmp.Length < 1 Then Return False

            Dim id As Integer = tmp(0)

            If ItemSet.ContainsKey(id) Then
                If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                    For Each item As Integer In ItemSet(id).ItemID
                        Dim newItem As New ItemObject(item, objCharacter.TargetGUID) With {
                                .StackCount = 1
                                }

                        If CHARACTERs(objCharacter.TargetGUID).ItemADD(newItem) Then
                            CHARACTERs(objCharacter.TargetGUID).LogLootItem(newItem, 1, False, True)
                        Else
                            newItem.Delete()
                        End If
                    Next
                Else
                    For Each item As Integer In ItemSet(id).ItemID
                        Dim newItem As New ItemObject(item, objCharacter.GUID) With {
                                .StackCount = 1
                                }

                        If objCharacter.ItemADD(newItem) Then
                            objCharacter.LogLootItem(newItem, 1, False, True)
                        Else
                            newItem.Delete()
                        End If
                    Next
                End If
            End If

            Return True
        End Function

        'Addmoney Command
        'ToDo: Add method of Copper, Silver or Gold in the command.
        '      Max Gold in Vanilla?
        <ChatCommand("addmoney", "addmoney #amount - Add chosen copper to your character or selected character.", AccessLevel.GameMaster)>
        Public Function cmdAddMoney(ByRef objCharacter As CharacterObject, ByVal tCopper As String) As Boolean
            If tCopper = "" Then Return False

            Dim Copper As ULong = tCopper

            If objCharacter.Copper + Copper > UInteger.MaxValue Then
                objCharacter.Copper = UInteger.MaxValue
            Else
                objCharacter.Copper += Copper
            End If

            objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, objCharacter.Copper)
            objCharacter.SendCharacterUpdate(False)

            Return True
        End Function

        'LearnSkill Command
        <ChatCommand("learnskill", "learnskill #id #current #max - Add skill id X with value Y of Z to selected character.", AccessLevel.Developer)>
        Public Function cmdLearnSkill(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Dim tmp() As String
                tmp = Split(Trim(Message), " ")

                Dim SkillID As Integer = tmp(0)
                Dim Current As Short = tmp(1)
                Dim Maximum As Short = tmp(2)

                If CHARACTERs(objCharacter.TargetGUID).Skills.ContainsKey(SkillID) Then
                    CType(CHARACTERs(objCharacter.TargetGUID).Skills(SkillID), TSkill).Base = Maximum
                    CType(CHARACTERs(objCharacter.TargetGUID).Skills(SkillID), TSkill).Current = Current
                Else
                    CHARACTERs(objCharacter.TargetGUID).LearnSkill(SkillID, Current, Maximum)
                End If

                CHARACTERs(objCharacter.TargetGUID).FillAllUpdateFlags()
                CHARACTERs(objCharacter.TargetGUID).SendUpdate()
            Else
                objCharacter.CommandResponse("Target not found or not character.")
            End If

            Return True
        End Function

        'LearnSpell Command
        <ChatCommand("learnSpell", "learnSpell #id - Add chosen spell to selected character.", AccessLevel.Developer)>
        Public Function cmdLearnSpell(ByRef objCharacter As CharacterObject, ByVal tID As String) As Boolean
            If tID = "" Then Return False

            Dim ID As Integer
            If Integer.TryParse(tID, ID) = False OrElse ID < 0 Then Return False
            If WS_Spells.SPELLs.ContainsKey(ID) = False Then
                objCharacter.CommandResponse("You tried learning a spell that did not exist.")
                Exit Function
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).LearnSpell(ID)
                If objCharacter.TargetGUID = objCharacter.GUID Then
                    objCharacter.CommandResponse("You learned spell: " & ID)
                Else
                    objCharacter.CommandResponse(CHARACTERs(objCharacter.TargetGUID).Name & " has learned spell: " & ID)
                End If
            Else
                objCharacter.CommandResponse("Target not found or not character.")
            End If

            Return True
        End Function

        'UnlearnSpell Command
        <ChatCommand("unlearnspell", "unlearnspell #id - Remove chosen spell from selected character.", AccessLevel.Developer)>
        Public Function cmdUnlearnSpell(ByRef objCharacter As CharacterObject, ByVal tID As String) As Boolean
            If tID = "" Then Return False

            Dim ID As Integer = tID

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).UnLearnSpell(ID)
                If objCharacter.TargetGUID = objCharacter.GUID Then
                    objCharacter.CommandResponse("You unlearned spell: " & ID)
                Else
                    objCharacter.CommandResponse(CHARACTERs(objCharacter.TargetGUID).Name & " has unlearned spell: " & ID)
                End If
            Else
                objCharacter.CommandResponse("Target not found or not character.")
            End If

            Return True
        End Function

        'ShowTaxi Command
        <ChatCommand("showtaxi", "showtaxi - Unlock all taxi locations.", AccessLevel.Developer)>
        Public Function cmdShowTaxi(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            objCharacter.TaxiZones.SetAll(True)
            Return True
        End Function

        'SetCharacterSpeed Command
        <ChatCommand("setcharacterspeed", "setcharacterspeed #value - Change your character travel speed.", AccessLevel.GameMaster)>
        Public Function cmdSetCharacterSpeed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            objCharacter.ChangeSpeedForced(ChangeSpeedType.RUN, Message)
            objCharacter.CommandResponse("Your RunSpeed is changed to " & Message)

            objCharacter.ChangeSpeedForced(ChangeSpeedType.SWIM, Message)
            objCharacter.CommandResponse("Your SwimSpeed is changed to " & Message)

            objCharacter.ChangeSpeedForced(ChangeSpeedType.SWIMBACK, Message)
            objCharacter.CommandResponse("Your RunBackSpeed is changed to " & Message)
            Return True
        End Function

        'SetReputation Command
        <ChatCommand("setreputation", "setreputation #faction #value - Change your reputation standings.", AccessLevel.GameMaster)>
        Public Function cmdSetReputation(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            Dim tmp() As String = Split(Message, " ", 2)
            objCharacter.SetReputation(tmp(0), tmp(1))
            objCharacter.CommandResponse("You have set your reputation with [" & tmp(0) & "] to [" & tmp(1) & "]")
            Return True
        End Function

        'ChangeModel Command
        <ChatCommand("changemodel", "changemodel #id - Will morph you into specified model ID.", AccessLevel.GameMaster)>
        Public Function cmdModel(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim value As Integer = 0
            If Integer.TryParse(Message, value) = False OrElse value < 0 Then Return False

            If CreatureModel.ContainsKey(value) Then
                objCharacter.BoundingRadius = CreatureModel(value).BoundingRadius
                objCharacter.CombatReach = CreatureModel(value).CombatReach
            End If

            objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_BOUNDINGRADIUS, objCharacter.BoundingRadius)
            objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_COMBATREACH, objCharacter.CombatReach)
            objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, value)
            objCharacter.SendCharacterUpdate()
            Return True
        End Function

        'Mount Command
        <ChatCommand("mount", "mount #id - Will mount you to specified model ID.", AccessLevel.GameMaster)>
        Public Function cmdMount(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim value As Integer = 0
            If Integer.TryParse(Message, value) = False OrElse value < 0 Then Return False

            objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, value)
            objCharacter.SendCharacterUpdate()
            Return True
        End Function

        'Hurt Command - Wait what?
        <ChatCommand("hurt", "hurt - Hurts a selected character.", AccessLevel.GameMaster)>
        Public Function cmdHurt(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).Life.Current -= CHARACTERs(objCharacter.TargetGUID).Life.Maximum * 0.1
                CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CHARACTERs(objCharacter.TargetGUID).Life.Current)
                CHARACTERs(objCharacter.TargetGUID).SendCharacterUpdate()
                Return True
            End If

            Return True
        End Function

        'Root Command
        <ChatCommand("root", "root - Instantly root selected character.", AccessLevel.GameMaster)>
        Public Function cmdRoot(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).SetMoveRoot()
                Return True
            End If

            Return True
        End Function

        'Unroot Command
        <ChatCommand("unroot", "unroot - Instantly unroot selected character.", AccessLevel.GameMaster)>
        Public Function cmdUnRoot(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).SetMoveUnroot()
                Return True
            End If

            Return True
        End Function

        'Revive Command
        <ChatCommand("revive", "revive - Instantly revive selected character.", AccessLevel.GameMaster)>
        Public Function cmdRevive(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CharacterResurrect(CHARACTERs(objCharacter.TargetGUID))
                Return True
            End If

            Return True
        End Function

        'GoToGY Command
        <ChatCommand("gotogy", "gotogy - Instantly teleports selected character to nearest graveyard.", AccessLevel.GameMaster)>
        Public Function cmdGoToGraveyard(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                AllGraveYards.GoToNearestGraveyard(CHARACTERs(objCharacter.TargetGUID), False, True)
                Return True
            End If

            Return True
        End Function

        'ToStart Command
        <ChatCommand("tostart", "tostart #race - Instantly teleports selected character to specified race start location.", AccessLevel.GameMaster)>
        Public Function cmdGoToStart(ByRef objCharacter As CharacterObject, ByVal StringRace As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                Dim Info As New DataTable
                Dim Character As CharacterObject = CHARACTERs(objCharacter.TargetGUID)
                Dim Race As Races

                Select Case UppercaseFirstLetter(StringRace)
                    Case "DWARF", "DW"
                        Race = Races.RACE_DWARF
                    Case "GNOME", "GN"
                        Race = Races.RACE_GNOME
                    Case "HUMAN", "HU"
                        Race = Races.RACE_HUMAN
                    Case "NIGHTELF", "NE"
                        Race = Races.RACE_NIGHT_ELF
                    Case "ORC", "OR"
                        Race = Races.RACE_ORC
                    Case "TAUREN", "TA"
                        Race = Races.RACE_TAUREN
                    Case "TROLL", "TR"
                        Race = Races.RACE_TROLL
                    Case "UNDEAD", "UN"
                        Race = Races.RACE_UNDEAD
                    Case Else
                        objCharacter.CommandResponse("Unknown race. Use DW, GN, HU, NE, OR, TA, TR, UN for race.")
                        Return True
                End Select

                WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo WHERE race = {0};", CType(Race, Integer)), Info)
                Character.Teleport(Info.Rows(0).Item("position_x"), Info.Rows(0).Item("position_y"), Info.Rows(0).Item("position_z"), Info.Rows(0).Item("orientation"), Info.Rows(0).Item("map"))
                Return True
            End If

            Return True
        End Function

        'Summon Command
        <ChatCommand("summon", "summon #name - Instantly teleports the player to you.", AccessLevel.GameMaster)>
        Public Function cmdSummon(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
            Dim GUID As ULong = GetGUID(CapitalizeName(Name))
            If CHARACTERs.ContainsKey(GUID) Then
                If objCharacter.OnTransport IsNot Nothing Then
                    CType(CHARACTERs(GUID), CharacterObject).OnTransport = objCharacter.OnTransport
                    CHARACTERs(GUID).Transfer(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
                Else
                    CHARACTERs(GUID).Teleport(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
                End If
                Return True
            Else
                objCharacter.CommandResponse("Player not found.")
                Return True
            End If
        End Function

        'Appear Command
        <ChatCommand("appear", "appear #name - Instantly teleports you to the player.", AccessLevel.GameMaster)>
        Public Function cmdAppear(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
            Dim GUID As ULong = GetGUID(CapitalizeName(Name))
            If CHARACTERs.ContainsKey(GUID) Then
                With CHARACTERs(GUID)
                    If .OnTransport IsNot Nothing Then
                        objCharacter.OnTransport = .OnTransport
                        objCharacter.Transfer(.positionX, .positionY, .positionZ, .orientation, .MapID)
                    Else
                        objCharacter.Teleport(.positionX, .positionY, .positionZ, .orientation, .MapID)
                    End If
                End With
                Return True
            Else
                objCharacter.CommandResponse("Player not found.")
                Return True
            End If
        End Function

        '    <ChatCommand("VmapTest", "VMAPTEST - Tests VMAP functionality.", AccessLevel.Developer)>
        '    Public Function cmdVmapTest(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        '#If VMAPS Then
        '        If Config.VMapsEnabled Then
        '            Dim target As BaseUnit = Nothing
        '            If objCharacter.TargetGUID > 0 Then
        '                If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
        '                    target = CHARACTERs(objCharacter.TargetGUID)
        '                ElseIf GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
        '                    target = WORLD_CREATUREs(objCharacter.TargetGUID)
        '                    WORLD_CREATUREs(objCharacter.TargetGUID).SetToRealPosition()
        '                End If
        '            End If

        '            Dim timeStart As Integer = timeGetTime("")

        '            Dim height As Single = GetVMapHeight(objCharacter.MapID, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ + 2.0F)

        '            Dim isInLOS As Boolean = False
        '            If target IsNot Nothing Then
        '                isInLOS = IsInLineOfSight(objCharacter, target)
        '            End If

        '            Dim timeTaken As Integer = timeGetTime("") - timeStart

        '            If height = VMAP_INVALID_HEIGHT_VALUE Then
        '                objCharacter.CommandResponse(String.Format("Unable to retrieve VMap height for your location."))
        '            Else
        '                objCharacter.CommandResponse(String.Format("Your Z: {0}  VMap Z: {1}", objCharacter.positionZ, height))
        '            End If

        '            If target IsNot Nothing Then
        '                objCharacter.CommandResponse(String.Format("Target in line of sight: {0}", isInLOS))
        '            End If

        '            objCharacter.CommandResponse(String.Format("Vmap functionality ran under [{0} ms].", timeTaken))
        '        Else
        '            objCharacter.CommandResponse("Vmaps is not enabled.")
        '        End If
        '#Else
        '        objCharacter.CommandResponse("Vmaps is not enabled.")
        '#End If
        '        Return True
        '    End Function

        '    <ChatCommand("VmapTest2", "VMAPTEST2 - Tests VMAP functionality.", AccessLevel.Developer)>
        '    Public Function cmdVmapTest2(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        '#If VMAPS Then
        '        If Config.VMapsEnabled Then
        '            If objCharacter.TargetGUID = 0UL OrElse GuidIsCreature(objCharacter.TargetGUID) = False OrElse WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) = False Then
        '                objCharacter.CommandResponse("You must target a creature first.")
        '            Else
        '                WORLD_CREATUREs(objCharacter.TargetGUID).SetToRealPosition()

        '                Dim resX As Single = 0.0F
        '                Dim resY As Single = 0.0F
        '                Dim resZ As Single = 0.0F
        '                Dim result As Boolean = GetObjectHitPos(objCharacter, WORLD_CREATUREs(objCharacter.TargetGUID), resX, resY, resZ, -1.0F)

        '                If result = False Then
        '                    objCharacter.CommandResponse("You teleported without any problems.")
        '                Else
        '                    objCharacter.CommandResponse("You teleported by hitting something.")
        '                End If

        '                objCharacter.orientation = GetOrientation(objCharacter.positionX, WORLD_CREATUREs(objCharacter.TargetGUID).positionX, objCharacter.positionY, WORLD_CREATUREs(objCharacter.TargetGUID).positionY)
        '                resZ = GetVMapHeight(objCharacter.MapID, resX, resY, resZ + 2.0F)
        '                objCharacter.Teleport(resX, resY, resZ, objCharacter.orientation, objCharacter.MapID)
        '            End If
        '        Else
        '            objCharacter.CommandResponse("Vmaps is not enabled.")
        '        End If
        '#Else
        '        objCharacter.CommandResponse("Vmaps is not enabled.")
        '#End If
        '        Return True
        '    End Function

        '    <ChatCommand("VmapTest3", "VMAPTEST3 - Tests VMAP functionality.", AccessLevel.Developer)>
        '    Public Function cmdVmapTest3(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        '#If VMAPS Then
        '        Dim CellMap As UInteger = objCharacter.MapID
        '        Dim CellX As Byte = GetMapTileX(objCharacter.positionX)
        '        Dim CellY As Byte = GetMapTileY(objCharacter.positionY)

        '        Dim fileName As String = String.Format("{0}_{1}_{2}.vmdir", Format(CellMap, "000"), Format(CellX, "00"), Format(CellY, "00"))
        '        If Not IO.File.Exists("vmaps\" & fileName) Then
        '            objCharacter.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
        '            fileName = String.Format("{0}.vmdir", Format(CellMap, "000"))
        '        End If

        '        If Not IO.File.Exists("vmaps\" & fileName) Then
        '            objCharacter.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
        '        Else
        '            objCharacter.CommandResponse(String.Format("VMap file [{0}] found!", fileName))
        '            Dim map As TMap = Maps(CellMap)
        '            fileName = Trim(IO.File.ReadAllText("vmaps\" & fileName))

        '            objCharacter.CommandResponse(String.Format("Full file: '{0}'", fileName))
        '            If fileName.Contains(vbLf) Then
        '                fileName = fileName.Substring(0, fileName.IndexOf(vbLf))
        '            End If

        '            objCharacter.CommandResponse(String.Format("First line: '{0}'", fileName))
        '            Dim newModelLoaded As Boolean = False
        '            If fileName.Length > 0 AndAlso IO.File.Exists("vmaps\" & fileName) Then
        '                objCharacter.CommandResponse(String.Format("VMap file [{0}] found!", fileName))

        '                If Maps(CellMap).ContainsModelContainer(fileName) Then
        '                    objCharacter.CommandResponse(String.Format("VMap ModelContainer is loaded!"))
        '                Else
        '                    objCharacter.CommandResponse(String.Format("VMap ModelContainer is NOT loaded!"))
        '                End If
        '            Else
        '                objCharacter.CommandResponse(String.Format("VMap file [{0}] not found!", fileName))
        '            End If
        '        End If
        '#Else
        '        objCharacter.CommandResponse("Vmaps is not enabled.")
        '#End If
        '        Return True
        '    End Function

        'LOS Command
        <ChatCommand("los", "los #on/off - Enables/Disables line of sight calculation.", AccessLevel.Developer)>
        Public Function cmdLineOfSight(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message.ToUpper = "on" Then
                Config.LineOfSightEnabled = True
                objCharacter.CommandResponse("Line of Sight Calculation is now Enabled.")
            ElseIf Message.ToUpper = "on" Then
                Config.LineOfSightEnabled = False
                objCharacter.CommandResponse("Line of Sight Calculation is now Disabled.")
            Else
                Return False
            End If
            Return True
        End Function

        'GPS Command
        <ChatCommand("gps", "gps - Tells you where you are located.", AccessLevel.GameMaster)>
        Public Function cmdGPS(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            objCharacter.CommandResponse("X: " & objCharacter.positionX)
            objCharacter.CommandResponse("Y: " & objCharacter.positionY)
            objCharacter.CommandResponse("Z: " & objCharacter.positionZ)
            objCharacter.CommandResponse("Orientation: " & objCharacter.orientation)
            objCharacter.CommandResponse("Map: " & objCharacter.MapID)
            Return True
        End Function

        'SetInstance Command
        <ChatCommand("SetInstance", "SETINSTANCE <ID> - Sets you into another instance.", AccessLevel.Admin)>
        Public Function cmdSetInstance(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim instanceID As Integer = 0
            If Integer.TryParse(Message, instanceID) = False Then Return False
            If instanceID < 0 OrElse instanceID > 400000 Then Return False

            objCharacter.instance = instanceID
            Return True
        End Function

        'Port Command
        <ChatCommand("port", "port #x #y #z #orientation #map - Teleports Character To Given Coordinates.", AccessLevel.GameMaster)>
        Public Function cmdPort(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False

            Dim tmp() As String
            tmp = Message.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

            If tmp.Length <> 5 Then Return False

            Dim posX As Single = tmp(0)
            Dim posY As Single = tmp(1)
            Dim posZ As Single = tmp(2)
            Dim posO As Single = tmp(3)
            Dim posMap As Integer = CSng(tmp(4))

            objCharacter.Teleport(posX, posY, posZ, posO, posMap)
            Return True
        End Function

        'Teleport Command
        <ChatCommand("teleport", "teleport #locationname - Teleports character to given location name.", AccessLevel.GameMaster)>
        Public Function CmdPortByName(ByRef objCharacter As CharacterObject, ByVal location As String) As Boolean

            If location = "" Then Return False

            Dim posX As Single '= 0
            Dim posY As Single '= 0
            Dim posZ As Single '= 0
            Dim posO As Single '= 0
            Dim posMap As Integer '= 0

            If UppercaseFirstLetter(location) = "LIST" Then
                Dim cmdList As String = "Listing of available locations:" & Environment.NewLine

                Dim listSqlQuery As New DataTable
                WorldDatabase.Query("SELECT * FROM game_tele order by name", listSqlQuery)

                For Each locationRow As DataRow In listSqlQuery.Rows
                    cmdList += locationRow.Item("name") & ", "
                Next
                objCharacter.CommandResponse(cmdList)
                Return True
            End If

            location = location.Replace("'", "").Replace(" ", "")
            location = location.Replace(";", "") 'Some SQL Safety added

            Dim mySqlQuery As New DataTable
            If location.Contains("*") Then
                location = location.Replace("*", "")
                WorldDatabase.Query(String.Format("SELECT * FROM game_tele WHERE name like '{0}%' order by name;", location), mySqlQuery)
            Else
                WorldDatabase.Query(String.Format("SELECT * FROM game_tele WHERE name = '{0}' order by name LIMIT 1;", location), mySqlQuery)
            End If
            If mySqlQuery.Rows.Count > 0 Then
                If mySqlQuery.Rows.Count = 1 Then

                    posX = mySqlQuery.Rows(0).Item("position_x")
                    posY = mySqlQuery.Rows(0).Item("position_y")
                    posZ = mySqlQuery.Rows(0).Item("position_z")
                    posO = mySqlQuery.Rows(0).Item("orientation")
                    posMap = mySqlQuery.Rows(0).Item("map")
                    objCharacter.Teleport(posX, posY, posZ, posO, posMap)
                Else
                    Dim cmdList As String = "Listing of matching locations:" & Environment.NewLine

                    For Each locationRow As DataRow In mySqlQuery.Rows
                        cmdList += locationRow.Item("name") & ", "
                    Next
                    objCharacter.CommandResponse(cmdList)
                    Return True
                End If
            Else
                objCharacter.CommandResponse(String.Format("Location {0} NOT found in Database", location))
            End If
            Return True
        End Function

        'Kick Command
        <ChatCommand("kick", "kick #name (optional) - Kick selected player or character with name specified if found.", AccessLevel.GameMaster)>
        Public Function cmdKick(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
            If Name = "" Then

                'DONE: Kick by selection
                If objCharacter.TargetGUID = 0 Then
                    objCharacter.CommandResponse("No target selected.")
                ElseIf CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                    'DONE: Kick gracefully
                    objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", CHARACTERs(objCharacter.TargetGUID).Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.client.IP.ToString, objCharacter.client.Port, objCharacter.client.Character.Name, CHARACTERs(objCharacter.TargetGUID).Name)
                    CHARACTERs(objCharacter.TargetGUID).Logout()
                Else
                    objCharacter.CommandResponse(String.Format("Character GUID=[{0}] not found.", objCharacter.TargetGUID))
                End If

            Else

                'DONE: Kick by name
                CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
                For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                    If UppercaseFirstLetter(Character.Value.Name) = Name Then
                        CHARACTERs_Lock.ReleaseReaderLock()
                        'DONE: Kick gracefully
                        Character.Value.Logout()
                        objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", Character.Value.Name))
                        Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.client.IP.ToString, objCharacter.client.Port, objCharacter.client.Character.Name, Name)
                        Return True
                    End If
                Next
                CHARACTERs_Lock.ReleaseReaderLock()
                objCharacter.CommandResponse(String.Format("Character [{0:X}] not found.", Name))

            End If
            Return True
        End Function

        'ForceRename
        'ToDo: Add option to use a player name as well
        <ChatCommand("forcerename", "forcerename - Force selected player to change his name next time on char enum.", AccessLevel.GameMaster)>
        Public Function cmdForceRename(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CharacterDatabase.Update(String.Format("UPDATE characters SET force_restrictions = 1 WHERE char_guid = {0};", objCharacter.TargetGUID))
                objCharacter.CommandResponse("Player will be asked to change his name on next logon.")
            Else
                objCharacter.CommandResponse(String.Format("Character GUID=[{0:X}] not found.", objCharacter.TargetGUID))
            End If

            Return True
        End Function

        'BanCharacter Command
        'ToDo: Add option to use a player name as well
        <ChatCommand("bancharacter", "bancharacter - Selected player won't be able to login next time with this character.", AccessLevel.GameMaster)>
        Public Function cmdBanChar(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CharacterDatabase.Update(String.Format("UPDATE characters SET force_restrictions = 2 WHERE char_guid = {0};", objCharacter.TargetGUID))
                objCharacter.CommandResponse("Character disabled.")
            Else
                objCharacter.CommandResponse(String.Format("Character GUID=[{0:X}] not found.", objCharacter.TargetGUID))
            End If

            Return True
        End Function

        'BanAccount Comand
        <ChatCommand("banaccount", "banaccount #account - Ban specified account from server.", AccessLevel.GameMaster)>
        Public Function cmdBan(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
            'TODO: Allow Reason For BAN to be Specified, and Inserted.
            If Name = "" Then Return False

            Dim account As New DataTable
            AccountDatabase.Query("SELECT id, last_ip FROM account WHERE username = """ & Name & """;", account)
            Dim accountID As ULong = account.Rows(0).Item("id")
            Dim IP As Integer = account.Rows(0).Item("last_ip")

            Dim result As New DataTable
            AccountDatabase.Query("SELECT active FROM account_banned WHERE id = " & accountID & ";", result)
            If result.Rows.Count > 0 Then
                If result.Rows(0).Item("active") = 1 Then
                    objCharacter.CommandResponse(String.Format("Account [{0}] already banned.", Name))
                Else
                    'TODO: We May Want To Allow Account and IP to be Banned Separately
                    AccountDatabase.Update(String.Format("INSERT INTO `account_banned` VALUES ('{0}', UNIX_TIMESTAMP({1}), UNIX_TIMESTAMP({2}), '{3}', '{4}', active = 1);", accountID, Format(Now, "yyyy-MM-dd hh:mm:ss"), "0000-00-00 00:00:00", objCharacter.Name, "No Reason Specified."))
                    AccountDatabase.Update(String.Format("INSERT INTO `ip_banned` VALUES ('{0}', UNIX_TIMESTAMP({1}), UNIX_TIMESTAMP({2}), '{3}', '{4}');", IP, Format(Now, "yyyy-MM-dd hh:mm:ss"), "0000-00-00 00:00:00", objCharacter.Name, "No Reason Specified."))
                    objCharacter.CommandResponse(String.Format("Account [{0}] banned.", Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] banned by [{2}].", objCharacter.client.IP.ToString, objCharacter.client.Port, objCharacter.Name, Name)
                End If
            Else
                objCharacter.CommandResponse(String.Format("Account [{0}] not found.", Name))
            End If

            Return True
        End Function

        'UnBan Command
        <ChatCommand("unban", "unban #account - Remove ban of specified account from server.", AccessLevel.Admin)>
        Public Function cmdUnBan(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
            If Name = "" Then Return False

            Dim account As New DataTable
            AccountDatabase.Query("SELECT id, last_ip FROM account WHERE username = """ & Name & """;", account)
            Dim accountID As ULong = account.Rows(0).Item("id")
            Dim IP As Integer = account.Rows(0).Item("last_ip")

            Dim result As New DataTable
            AccountDatabase.Query("SELECT active FROM account_banned WHERE id = '" & accountID & "';", result)
            If result.Rows.Count > 0 Then
                If result.Rows(0).Item("active") = 0 Then
                    objCharacter.CommandResponse(String.Format("Account [{0}] is not banned.", Name))
                Else
                    'TODO: Do we want to update the account_banned, ip_banned tables or DELETE the records?
                    AccountDatabase.Update("UPDATE account_banned SET active = 0 WHERE id = '" & accountID & "';")
                    AccountDatabase.Update(String.Format("DELETE FROM `ip_banned` WHERE `ip` = '{0}';", IP))
                    objCharacter.CommandResponse(String.Format("Account [{0}] unbanned.", Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] unbanned by [{2}].", objCharacter.client.IP.ToString, objCharacter.client.Port, objCharacter.Name, Name)
                End If
            Else
                objCharacter.CommandResponse(String.Format("Account [{0}] not found.", Name))
            End If

            Return True
        End Function

        'SetGM Command - not really working as it should right now
        <ChatCommand("setgm", "set gm #flag #invisibility - Toggles gameMaster status. You can use values like On/Off.", AccessLevel.GameMaster)>
        Public Function cmdSetGM(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp() As String = Split(Message, " ", 2)
            Dim value1 As String = tmp(0)
            Dim value2 As String = tmp(1)

            'setFaction(35);
            'SetFlag(PLAYER_BYTES_2, 0x8);

            'Commnad: .setgm <gmflag:off/on> <invisibility:off/on>
            If UppercaseFirstLetter(value1) = "off" Then
                objCharacter.GM = False
                objCharacter.CommandResponse("GameMaster Flag turned off.")
            ElseIf UppercaseFirstLetter(value1) = "on" Then
                objCharacter.GM = True
                objCharacter.CommandResponse("GameMaster Flag turned on.")
            End If

            If UppercaseFirstLetter(value2) = "off" Then
                objCharacter.Invisibility = InvisibilityLevel.VISIBLE
                objCharacter.CanSeeInvisibility = InvisibilityLevel.VISIBLE
                objCharacter.CommandResponse("GameMaster Invisibility turned off.")
            ElseIf UppercaseFirstLetter(value1) = "on" Then
                objCharacter.Invisibility = InvisibilityLevel.GM
                objCharacter.CanSeeInvisibility = InvisibilityLevel.GM
                objCharacter.CommandResponse("GameMaster Invisibility turned on.")
            End If

            objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, objCharacter.cPlayerFlags)
            objCharacter.SendCharacterUpdate()
            UpdateCell(objCharacter)

            Return True
        End Function

        'SetWeather Command
        <ChatCommand("setweather", "setweather #type #intensity - Change weather in current zone. Intensity is float value!", AccessLevel.Developer)>
        Public Function cmdSetWeather(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim tmp() As String = Split(Message, " ", 2)
            Dim Type As Integer = tmp(0)
            Dim Intensity As Single = tmp(1)

            If WeatherZones.ContainsKey(objCharacter.ZoneID) = False Then
                objCharacter.CommandResponse("No weather for this zone is found!")
            Else
                WeatherZones(objCharacter.ZoneID).CurrentWeather = Type
                WeatherZones(objCharacter.ZoneID).Intensity = Intensity
                SendWeather(objCharacter.ZoneID, objCharacter.client)
            End If

            Return True
        End Function

        'Remove Command
        'ToDo: Needs to be split in two commands
        <ChatCommand("remove", "remove #id - Delete selected creature or gameobject.", AccessLevel.Developer)>
        Public Function cmdDeleteObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If GuidIsCreature(objCharacter.TargetGUID) Then
                'DONE: Delete creature
                If Not WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                    objCharacter.CommandResponse("Selected target is not creature!")
                    Return True
                End If

                WORLD_CREATUREs(objCharacter.TargetGUID).Destroy()
                objCharacter.CommandResponse("Creature deleted.")

            ElseIf GuidIsGameObject(objCharacter.TargetGUID) Then
                'DONE: Delete GO
                If Not WORLD_GAMEOBJECTs.ContainsKey(objCharacter.TargetGUID) Then
                    objCharacter.CommandResponse("Selected target is not game object!")
                    Return True
                End If

                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).Destroy(WORLD_GAMEOBJECTs(objCharacter.TargetGUID))
                objCharacter.CommandResponse("Game object deleted.")

            End If

            Return True
        End Function

        'Turn Command
        'ToDo: Needs to be split in two commands
        <ChatCommand("turn", "turn - Selected creature or game object will turn to your position.", AccessLevel.Developer)>
        Public Function cmdTurnObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If GuidIsCreature(objCharacter.TargetGUID) Then
                'DONE: Turn creature
                If Not WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                    objCharacter.CommandResponse("Selected target is not creature!")
                    Return True
                End If

                WORLD_CREATUREs(objCharacter.TargetGUID).TurnTo(objCharacter.positionX, objCharacter.positionY)

            ElseIf GuidIsGameObject(objCharacter.TargetGUID) Then
                'DONE: Turn GO
                If Not WORLD_GAMEOBJECTs.ContainsKey(objCharacter.TargetGUID) Then
                    objCharacter.CommandResponse("Selected target is not game object!")
                    Return True
                End If

                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).TurnTo(objCharacter.positionX, objCharacter.positionY)

                Dim q As New DataTable
                Dim GUID As ULong = objCharacter.TargetGUID - GUID_GAMEOBJECT

                objCharacter.CommandResponse("Object rotation will be visible when the object is reloaded!")

            End If

            Return True
        End Function

        'AddNpc Command
        <ChatCommand("npcadd", "npcadd #id - Spawn creature at your position.", AccessLevel.Developer)>
        Public Function cmdAddCreature(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean

            Dim tmpCr As CreatureObject = New CreatureObject(Message, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
            tmpCr.AddToWorld()
            objCharacter.CommandResponse("Creature [" & tmpCr.Name & "] spawned.")

            Return True
        End Function

        'NpcCome Command
        <ChatCommand("npccome", "npccome - Selected creature will come to your position.", AccessLevel.Developer)>
        Public Function cmdComeCreature(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If
            If Not WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                objCharacter.CommandResponse("Selected target is not creature!")
                Return True
            End If

            Dim creature As CreatureObject = WORLD_CREATUREs(objCharacter.TargetGUID)

            If creature.aiScript IsNot Nothing AndAlso creature.aiScript.InCombat() Then
                objCharacter.CommandResponse("Creature is in combat. It has to be out of combat first.")
                Return True
            End If

            creature.SetToRealPosition(True)
            Dim MoveTime As Integer = creature.MoveTo(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation)
            If creature.aiScript IsNot Nothing Then
                creature.aiScript.Pause(MoveTime) 'Make sure it doesn't do anything in this period
            End If

            Return True
        End Function

        'Kill Command
        <ChatCommand("kill", "kill - Selected creature or character will die.", AccessLevel.GameMaster)>
        Public Function cmdKillCreature(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("Select target first!")
                Return True
            End If

            If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                CHARACTERs(objCharacter.TargetGUID).Die(objCharacter)
                Return True
            ElseIf WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                WORLD_CREATUREs(objCharacter.TargetGUID).DealDamage(WORLD_CREATUREs(objCharacter.TargetGUID).Life.Maximum)
                Return True
            End If
            Return False
        End Function

        'ObjectTarget Command
        <ChatCommand("gobjecttarget", "gobjecttarget - Nearest game object will be selected.", AccessLevel.Developer)>
        Public Function cmdTargetGameObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            Dim targetGO As GameObjectObject = GetClosestGameobject(objCharacter)

            If targetGO Is Nothing Then
                objCharacter.CommandResponse("Could not find any near objects.")
            Else
                Dim distance As Single = GetDistance(targetGO, objCharacter)
                objCharacter.CommandResponse(String.Format("Selected [{0}][{1}] game object at distance {2}.", targetGO.ID, targetGO.Name, distance))
            End If

            Return True
        End Function

        'ActiveGameObject Command
        <ChatCommand("activatego", "activatego - Activates your targetted game object.", AccessLevel.Developer)>
        Public Function cmdActivateGameObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If WORLD_GAMEOBJECTs.ContainsKey(objCharacter.TargetGUID) = False Then Return False

            If WORLD_GAMEOBJECTs(objCharacter.TargetGUID).State = GameObjectLootState.DOOR_CLOSED Then
                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).State = GameObjectLootState.DOOR_OPEN
                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).SetState(GameObjectLootState.DOOR_OPEN)
            Else
                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).State = GameObjectLootState.DOOR_CLOSED
                WORLD_GAMEOBJECTs(objCharacter.TargetGUID).SetState(GameObjectLootState.DOOR_CLOSED)
            End If

            objCharacter.CommandResponse(String.Format("Activated game object [{0}] to state [{1}].", WORLD_GAMEOBJECTs(objCharacter.TargetGUID).Name, WORLD_GAMEOBJECTs(objCharacter.TargetGUID).State))

            Return True
        End Function

        'GameObjectAdd Command
        <ChatCommand("gobjectadd", "gobjectadd #id - Spawn game object at your position.", AccessLevel.Developer)>
        Public Function cmdAddGameObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean

            Dim tmpGO As GameObjectObject = New GameObjectObject(Message, objCharacter.MapID, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation)
            tmpGO.Rotations(2) = Math.Sin(tmpGO.orientation / 2)
            tmpGO.Rotations(3) = Math.Cos(tmpGO.orientation / 2)
            tmpGO.AddToWorld()

            objCharacter.CommandResponse(String.Format("GameObject [{0}][{1:X}] spawned.", tmpGO.Name, tmpGO.GUID))

            Return True
        End Function

        'CreateAccount Command
        <ChatCommand("createaccount", "createaccount #account #password #email - Add a New account using Name, Password, And Email.", AccessLevel.Admin)>
        Public Function cmdCreateAccount(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            Dim result As New DataTable
            Dim acct() As String
            acct = Split(Trim(Message), " ")
            If acct.Length <> 3 Then Return False

            Dim aName As String = acct(0)
            Dim aPassword As String = acct(1)
            Dim aEmail As String = acct(2)
            AccountDatabase.Query("SELECT username FROM account WHERE username = """ & aName & """;", result)
            If result.Rows.Count > 0 Then
                objCharacter.CommandResponse(String.Format("Account [{0}] already exists.", aName))
            Else
                Dim passwordStr() As Byte = Text.Encoding.ASCII.GetBytes(aName.ToUpper & ":" & aPassword.ToUpper)
                Dim passwordHash() As Byte = New Security.Cryptography.SHA1Managed().ComputeHash(passwordStr)
                Dim hashStr As String = BitConverter.ToString(passwordHash).Replace("-", "")

                AccountDatabase.Insert(String.Format("INSERT INTO account (username, sha_pass_hash, email, joindate, last_ip) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", aName, hashStr, aEmail, Format(Now, "yyyy-MM-dd"), "0.0.0.0"))
                objCharacter.CommandResponse(String.Format("Account [{0}] has been created.", aName))
            End If
            Return True
        End Function

        'ChangePassword Command
        <ChatCommand("changepassword", "changepassword #account #password - Changes the password of an account.", AccessLevel.Admin)>
        Public Function cmdChangePassword(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            Dim result As New DataTable
            Dim acct() As String
            acct = Split(Trim(Message), " ")
            If acct.Length <> 2 Then Return False

            Dim aName As String = acct(0)
            Dim aPassword As String = acct(1)

            AccountDatabase.Query("SELECT id, gmlevel FROM account WHERE username = """ & aName & """;", result)
            If result.Rows.Count = 0 Then
                objCharacter.CommandResponse(String.Format("Account [{0}] does not exist.", aName))
            Else
                Dim targetLevel As AccessLevel = result.Rows(0).Item("gmlevel")
                If targetLevel >= objCharacter.Access Then
                    objCharacter.CommandResponse("You cannot change password for accounts with the same or a higher access level than yourself.")
                Else
                    Dim passwordStr() As Byte = Text.Encoding.ASCII.GetBytes(aName.ToUpper & ":" & aPassword.ToUpper)
                    Dim passwordHash() As Byte = New Security.Cryptography.SHA1Managed().ComputeHash(passwordStr)
                    Dim hashStr As String = BitConverter.ToString(passwordHash).Replace("-", "")

                    AccountDatabase.Update(String.Format("UPDATE account SET password='{0}' WHERE id={1}", hashStr, result.Rows(0).Item("id")))
                    objCharacter.CommandResponse(String.Format("Account [{0}] now has a new password [{1}].", aName, aPassword))
                End If
            End If
            Return True
        End Function

        'SetAccess Command
        <ChatCommand("setaccess", "setaccess #account #level - Sets the account to a specific access level.", AccessLevel.Admin)>
        Public Function cmdSetAccess(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
            If Message = "" Then Return False
            Dim result As New DataTable
            Dim acct() As String
            acct = Split(Trim(Message), " ")
            If acct.Length <> 2 Then Return False

            Dim aName As String = acct(0)
            Dim aLevel As Byte
            If Byte.TryParse(acct(1), aLevel) = False Then Return False

            If aLevel < AccessLevel.Trial OrElse aLevel > AccessLevel.Developer Then
                objCharacter.CommandResponse(String.Format("Not a valid access level. Must be in the range {0}-{1}.", CByte(AccessLevel.Trial), CByte(AccessLevel.Developer)))
                Return True
            End If

            Dim newLevel As AccessLevel = aLevel
            If newLevel >= objCharacter.Access Then
                objCharacter.CommandResponse("You cannot set access levels to your own or above your own access level.")
                Return True
            End If

            AccountDatabase.Query("SELECT id, gmlevel FROM account WHERE username = """ & aName & """;", result)
            If result.Rows.Count = 0 Then
                objCharacter.CommandResponse(String.Format("Account [{0}] does not exist.", aName))
            Else
                Dim targetLevel As AccessLevel = result.Rows(0).Item("gmlevel")
                If targetLevel >= objCharacter.Access Then
                    objCharacter.CommandResponse("You cannot set access levels to accounts with the same or a higher access level than yourself.")
                Else
                    AccountDatabase.Update(String.Format("UPDATE account SET gmlevel={0} WHERE id={1}", CByte(newLevel), result.Rows(0).Item("id")))
                    objCharacter.CommandResponse(String.Format("Account [{0}] now has access level [{1}].", aName, newLevel))
                End If
            End If
            Return True
        End Function

#Region "WS.Commands.InternalCommands.HelperSubs"

        Public Function GetGUID(ByVal Name As String) As ULong
            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE char_name = ""{0}"";", Name), MySQLQuery)

            If MySQLQuery.Rows.Count > 0 Then
                Return MySQLQuery.Rows(0).Item("char_guid")
            Else
                Return 0
            End If
        End Function

        Public Sub SystemMessage(ByVal Message As String)
            Dim packet As PacketClass = BuildChatMessage(0, "System Message: " & Message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL, 0, "")

            packet.UpdateLength()
            ClsWorldServer.Cluster.Broadcast(packet.Data)
            packet.Dispose()
        End Sub

        Public Function SetUpdateValue(ByVal GUID As ULong, ByVal Index As Integer, ByVal Value As Integer, ByVal client As ClientClass) As Boolean
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)      'Operations.Count
            packet.AddInt8(0)
            Dim UpdateData As New UpdateClass
            UpdateData.SetUpdateFlag(Index, Value)

            If GuidIsCreature(GUID) Then
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, WORLD_CREATUREs(GUID))
            ElseIf GuidIsPlayer(GUID) Then
                If GUID = client.Character.GUID Then
                    UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CHARACTERs(GUID))
                Else
                    UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CHARACTERs(GUID))
                End If
            End If

            client.Send(packet)
            packet.Dispose()
            UpdateData.Dispose()
        End Function

#End Region

    End Module
End NameSpace