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
Imports System.Reflection
Imports System.Collections.Generic
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common.NativeMethods

#Region "WS.Commands.Attributes"

<AttributeUsage(AttributeTargets.Method, Inherited:=False, AllowMultiple:=True)> _
Public Class ChatCommandAttribute
    Inherits Attribute

    Private Command As String = ""
    Private CommandHelp As String = "No information available."
    Private CommandAccess As AccessLevel = AccessLevel.GameMaster

    Public Sub New(ByVal cmdName As String, Optional ByVal cmdHelp As String = "No information available.", Optional ByVal cmdAccess As AccessLevel = AccessLevel.GameMaster)
        Command = cmdName
        CommandHelp = cmdHelp
        CommandAccess = cmdAccess
    End Sub

    Public Property cmdName() As String
        Get
            Return Command
        End Get
        Set(ByVal Value As String)
            Command = Value
        End Set
    End Property

    Public Property cmdHelp() As String
        Get
            Return CommandHelp
        End Get
        Set(ByVal Value As String)
            CommandHelp = Value
        End Set
    End Property

    Public Property cmdAccess() As AccessLevel
        Get
            Return CommandAccess
        End Get
        Set(ByVal Value As AccessLevel)
            CommandAccess = Value
        End Set
    End Property

End Class
#End Region

Public Module WS_Commands

#Region "WS.Commands.Framework"

    Public Const SystemGUID As ULong = Integer.MaxValue
    Public Const SystemNAME As String = "System"
    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Developer = 3
        Admin = 4
    End Enum

    Public ChatCommands As New Dictionary(Of String, ChatCommand)
    Public Class ChatCommand
        Public CommandHelp As String
        Public CommandAccess As AccessLevel = AccessLevel.GameMaster
        Public CommandDelegate As ChatCommandDelegate
    End Class

    Public Delegate Function ChatCommandDelegate(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean

    Public Sub RegisterChatCommands()
        For Each tmpModule As Type In [Assembly].GetExecutingAssembly.GetTypes
            For Each tmpMethod As MethodInfo In tmpModule.GetMethods
                Dim infos() As ChatCommandAttribute = tmpMethod.GetCustomAttributes(GetType(ChatCommandAttribute), True)

                If infos.Length <> 0 Then
                    For Each info As ChatCommandAttribute In infos
                        Dim cmd As New ChatCommand
                        cmd.CommandHelp = info.cmdHelp
                        cmd.CommandAccess = info.cmdAccess
                        cmd.CommandDelegate = ChatCommandDelegate.CreateDelegate(GetType(ChatCommandDelegate), tmpMethod)

                        ChatCommands.Add(UCase(info.cmdName), cmd)
#If DEBUG Then
                        Log.WriteLine(LogType.INFORMATION, "Command found: {0}", UCase(info.cmdName))
#End If
                    Next
                End If
            Next
        Next

    End Sub

    Public Sub OnCommand(ByRef client As ClientClass, ByVal Message As String)
        Try
            'DONE: Find the command
            Dim tmp() As String = Split(Message, " ", 2)
            Dim Command As ChatCommand = Nothing
            If ChatCommands.ContainsKey(UCase(tmp(0))) Then
                Command = ChatCommands(UCase(tmp(0)))
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
            ElseIf Not Command.CommandDelegate(Client.Character, Arguments) Then
                client.Character.CommandResponse(Command.CommandHelp)
            Else
                Log.WriteLine(LogType.USER, "[{0}:{1}] {2} used command: {3}", client.IP, client.Port, Name, Message)
            End If

        Catch err As Exception
            Log.WriteLine(LogType.FAILED, "[{0}:{1}] Client command caused error! {3}{2}", client.IP, client.Port, err.ToString, vbNewLine)
            client.Character.CommandResponse(String.Format("Your command caused error:" & vbNewLine & " [{0}]", err.Message))
        End Try
    End Sub

#End Region

#Region "WS.Commands.InternalCommands"

    <ChatCommandAttribute("Help", "HELP <CMD>" & vbNewLine & "Displays usage information about command, if no command specified - displays list of available commands.", AccessLevel.GameMaster)> _
    Public Function Help(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Trim(Message) <> "" Then
            Dim Command As ChatCommand = CType(ChatCommands(Trim(UCase(Message))), ChatCommand)
            If Command Is Nothing Then
                objCharacter.CommandResponse("Unknown command.")
            ElseIf Command.CommandAccess > objCharacter.Access Then
                objCharacter.CommandResponse("This command is not available for your access level.")
            Else
                objCharacter.CommandResponse(Command.CommandHelp)
            End If
        Else
            Dim cmdList As String = "Listing available commands:" & vbNewLine
            For Each Command As KeyValuePair(Of String, ChatCommand) In ChatCommands
                If CType(Command.Value, ChatCommand).CommandAccess <= objCharacter.Access Then cmdList += UCase(Command.Key) & vbNewLine '", "
            Next
            cmdList += vbNewLine + "Use HELP <CMD> for usage information about particular command."
            objCharacter.CommandResponse(cmdList)
        End If

        Return True
    End Function

    Dim x As Integer = 0
    Dim currentSpError As SpellFailedReason = SpellFailedReason.SPELL_NO_ERROR
    <ChatCommandAttribute("SpellFailedMSG", "SPELLFAILEDMSG <optional ID> - Sends test spell failed message.", AccessLevel.Developer)> _
    Public Function cmdSpellFailed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then
            currentSpError += 1
        Else
            currentSpError = Message
        End If
        SendCastResult(currentSpError, objCharacter.Client, 133)
        objCharacter.CommandResponse(String.Format("Sent spell failed message:{2} {0} = {1}", currentSpError, CType(currentSpError, Integer), vbNewLine))
        Return True
    End Function

    Dim currentInvError As InventoryChangeFailure = InventoryChangeFailure.EQUIP_ERR_OK
    <ChatCommandAttribute("InvFailedMSG", "INVFAILEDMSG <optional ID> - Sends test inventory failed message.", AccessLevel.Developer)> _
    Public Function cmdInventoryFailed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then
            currentInvError += 1
        Else
            currentInvError = Message
        End If
        Dim response As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
        response.AddInt8(currentInvError)
        response.AddUInt64(0)
        response.AddUInt64(0)
        response.AddInt8(0)
        objCharacter.Client.Send(response)
        response.Dispose()
        objCharacter.CommandResponse(String.Format("Sent spell failed message:{2} {0} = {1}", currentInvError, CType(currentInvError, Integer), vbNewLine))
        Return True
    End Function

    Dim currentInstanceResetError As ResetFailedReason = ResetFailedReason.INSTANCE_RESET_FAILED_ZONING
    <ChatCommandAttribute("InstanceResetFailedMSG", "INSTANCERESETFAILEDMSG <optional ID> - Sends test inventory failed message.", AccessLevel.Developer)> _
    Public Function cmdInstanceResetFailedReason(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then
            currentInstanceResetError += 1
        Else
            currentInstanceResetError = Message
        End If
        SendResetInstanceFailed(objCharacter.Client, objCharacter.MapID, currentInstanceResetError)
        objCharacter.CommandResponse(String.Format("Sent instance failed message:{2} {0} = {1}", currentInstanceResetError, CType(currentInstanceResetError, Integer), vbNewLine))
        Return True
    End Function

    <ChatCommandAttribute("CastSpell", "CASTSPELL <SpellID> <Target> - Selected unit will start casting spell. Target can be ME or SELF.", AccessLevel.Developer)> _
    Public Function cmdCastSpellMe(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp As String() = Split(Message, " ", 2)
        If tmp.Length < 2 Then Return False
        Dim SpellID As Integer = tmp(0)
        Dim Target As String = UCase(tmp(1))

        If GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
            Select Case Target
                Case "ME"
                    WORLD_CREATUREs(objCharacter.TargetGUID).CastSpell(SpellID, objCharacter)
                Case "SELF"
                    WORLD_CREATUREs(objCharacter.TargetGUID).CastSpell(SpellID, CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject))
            End Select
        ElseIf GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            Select Case Target
                Case "ME"
                    Dim Targets As New SpellTargets
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

    <ChatCommandAttribute("Control", "CONTROL - Takes or removes control over the selected unit.", AccessLevel.Admin)> _
    Public Function cmdControl(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.MindControl IsNot Nothing Then
            If TypeOf objCharacter.MindControl Is CharacterObject Then
                Dim packet1 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                packet1.AddPackGUID(objCharacter.MindControl.GUID)
                packet1.AddInt8(1)
                CType(objCharacter.MindControl, CharacterObject).Client.Send(packet1)
                packet1.Dispose()
            End If

            Dim packet3 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
            packet3.AddPackGUID(objCharacter.MindControl.GUID)
            packet3.AddInt8(0)
            objCharacter.Client.Send(packet3)
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
            CHARACTERs(objCharacter.TargetGUID).Client.Send(packet1)
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
        objCharacter.Client.Send(packet2)
        packet2.Dispose()

        objCharacter.cUnitFlags = objCharacter.cUnitFlags Or UnitFlags.UNIT_FLAG_UNK21
        objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, objCharacter.TargetGUID)
        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
        objCharacter.SendCharacterUpdate(False)

        objCharacter.CommandResponse("Taken control over a unit.")

        Return True
    End Function

    <ChatCommandAttribute("CreateGuild", "CreateGuild <Name> - Creates a guild.", AccessLevel.GameMaster)> _
    Public Function cmdCreateGuild(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        'TODO: Creating guilds must be done in the cluster

        'Dim GuildName As String = Message

        'Dim MySQLQuery As New DataTable
        'CharacterDatabase.Query(String.Format("INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES ('{0}', {1}, {2}, {3}, {4}); SELECT guild_id FROM guilds WHERE guild_name = '{0}';", GuildName, objCharacter.GUID, Now.Year, Now.Month, Now.Day), MySQLQuery)

        'AddCharacterToGuild(objCharacter, MySQLQuery.Rows(0).Item("guild_id"), 0)
        Return True
    End Function

    <ChatCommandAttribute("Cast", "CAST <SpellID> - You will start casting spell on selected target.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("Save", "SAVE - Saves selected character.", AccessLevel.GameMaster)> _
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

    <ChatCommandAttribute("Spawns", "SPAWNS - Tells you the spawn in memory information.", AccessLevel.Developer)> _
    Public Function cmdSpawns(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        objCharacter.CommandResponse("Spawns loaded in server memory:")
        objCharacter.CommandResponse("-------------------------------")
        objCharacter.CommandResponse("Creatures: " & WORLD_CREATUREs.Count)
        objCharacter.CommandResponse("GameObjects: " & WORLD_GAMEOBJECTs.Count)

        Return True
    End Function

    <ChatCommandAttribute("Near", "NEAR - Tells you the near objects count.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("SetWaterWalk", "SETWATERWALK <TRUE/FALSE> - Enables/Disables walking over water for selected target.", AccessLevel.Developer)> _
    Public Function cmdSetWaterWalk(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            If UCase(Message) = "TRUE" Then
                CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SetWaterWalk()
            ElseIf UCase(Message) = "FALSE" Then
                CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SetLandWalk()
            Else
                Return False
            End If
        Else
            objCharacter.CommandResponse("Select target is not character!")
            Return True
        End If
    End Function

    <ChatCommandAttribute("AI", "AI <ENABLE/DISABLE> - Enables/Disables AI updating.", AccessLevel.Developer)> _
    Public Function cmdAI(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If UCase(Message) = "ENABLE" Then
            AIManager.AIManagerTimer.Change(TAIManager.UPDATE_TIMER, TAIManager.UPDATE_TIMER)
            objCharacter.CommandResponse("AI is enabled.")
        ElseIf UCase(Message) = "DISABLE" Then
            AIManager.AIManagerTimer.Change(Timeout.Infinite, Timeout.Infinite)
            objCharacter.CommandResponse("AI is disabled.")
        Else
            Return False
        End If

        Return True
    End Function

    <ChatCommandAttribute("AIState", "AIState - Shows debug information about AI state of selected creature.", AccessLevel.Developer)> _
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
                objCharacter.CommandResponse(String.Format("Information for creature [{0}]:{1}ai = {2}{1}state = {3}{1}maxdist = {4}", .Name, vbNewLine, .aiScript.ToString, .aiScript.State.ToString, .MaxDistance))
                objCharacter.CommandResponse("Hate table:")
                For Each u As KeyValuePair(Of BaseUnit, Integer) In .aiScript.aiHateTable
                    objCharacter.CommandResponse(String.Format("{0:X} = {1} hate", u.Key.GUID, u.Value))
                Next
            End With
        End If

        Return True
    End Function

    <ChatCommandAttribute("Broadcast", "BROADCAST <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdBroadcast(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False

        Broadcast(Message)

        Return True
    End Function

    <ChatCommandAttribute("GameMessage", "GAMEMESSAGE <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdGameMessage(ByRef objCharacter As CharacterObject, ByVal Text As String) As Boolean
        If Text = "" Then Return False

        Dim packet As New PacketClass(OPCODES.SMSG_AREA_TRIGGER_MESSAGE)
        packet.AddInt32(0)
        packet.AddString(Text)
        packet.AddInt8(0)

        packet.UpdateLength()
        ClsWorldServer.Cluster.Broadcast(packet.Data)
        packet.Dispose()
        Return True
    End Function

    <ChatCommandAttribute("ServerMessage", "SERVERMESSAGE <TYPE> <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
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

    <ChatCommandAttribute("NotifyMessage", "NOTIFYMESSAGE <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdNotificationMessage(ByRef objCharacter As CharacterObject, ByVal Text As String) As Boolean
        If Text = "" Then Return False

        Dim packet As New PacketClass(OPCODES.SMSG_NOTIFICATION)
        packet.AddString(Text)

        packet.UpdateLength()
        ClsWorldServer.Cluster.Broadcast(packet.Data)
        packet.Dispose()

        Return True
    End Function

    <ChatCommandAttribute("Say", "SAY <TEXT> - Target Player / NPC will say this.", AccessLevel.GameMaster)> _
    Public Function cmdSay(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        If objCharacter.TargetGUID = 0 Then Return False

        If GuidIsPlayer(objCharacter.TargetGUID) Then
            CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SendChatMessage(CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject), Message, ChatMsg.CHAT_MSG_SAY, LANGUAGES.LANG_UNIVERSAL, , True)
        ElseIf GuidIsCreature(objCharacter.TargetGUID) Then
            CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject).SendChatMessage(Message, ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL, objCharacter.GUID)
        Else
            Return False
        End If

        Return True
    End Function

    <ChatCommandAttribute("ResetFactions", "RESETFACTIONS - Resets reputation standings.", AccessLevel.Admin)> _
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

    <ChatCommandAttribute("SetDurability", "SETDURABILITY <PERCENT> - Set all the target's itemdurability to a certain percentage.", AccessLevel.Admin)> _
    Public Function cmdSetDurability(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim Percent As Integer = 0
        If Integer.TryParse(Message, Percent) = True Then
            If Percent < 0 Then Percent = 0
            If Percent > 100 Then Percent = 100
            Dim sngPercent As Single = CSng(Percent / 100)

            If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                For i As Byte = EQUIPMENT_SLOT_START To EQUIPMENT_SLOT_END - 1
                    If CHARACTERs(objCharacter.TargetGUID).Items.ContainsKey(i) Then
                        CHARACTERs(objCharacter.TargetGUID).Items(i).ModifyToDurability(sngPercent, CHARACTERs(objCharacter.TargetGUID).Client)
                    End If
                Next
            Else
                For i As Byte = EQUIPMENT_SLOT_START To EQUIPMENT_SLOT_END - 1
                    If objCharacter.Items.ContainsKey(i) Then
                        objCharacter.Items(i).ModifyToDurability(sngPercent, objCharacter.Client)
                    End If
                Next
            End If

            Return True
        End If
        Return False
    End Function

    <ChatCommandAttribute("GetMax", "GETMAX - Get all spells and skills maxed out for your level.", AccessLevel.Admin)> _
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

    <ChatCommandAttribute("SetLevel", "SETLEVEL <LEVEL> - Set the level of selected character.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("AddXP", "ADDXP <XP> - Add X experience points to selected character.", AccessLevel.Admin)> _
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

    <ChatCommandAttribute("AddRestedXP", "ADDRESTEDXP <XP> - Add X rested bonus experience points to selected character.", AccessLevel.Admin)> _
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

    <ChatCommandAttribute("AddTP", "ADDTP <POINTs> - Add X talent points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddTP(ByRef objCharacter As CharacterObject, ByVal tTP As String) As Boolean
        If IsNumeric(tTP) = False Then Return False

        Dim TP As Integer = tTP

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).TalentPoints += TP
            CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, CType(CHARACTERs(objCharacter.TargetGUID).TalentPoints, Integer))
            CHARACTERs(objCharacter.TargetGUID).SaveCharacter()
        Else
            objCharacter.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("AddHonor", "ADDHONOR <POINTs> - Add X honor points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddHonor(ByRef objCharacter As CharacterObject, ByVal tHONOR As String) As Boolean
        If IsNumeric(tHONOR) = False Then Return False

        Dim Honor As Integer = tHONOR

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).HonorPoints += Honor
            'CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, CHARACTERs(objCharacter.TargetGUID).HonorCurrency)
            CHARACTERs(objCharacter.TargetGUID).SendCharacterUpdate(False)
        Else
            objCharacter.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("EditUnitFlag", "EDITUNITFLAG <UNITFLAG> - Change your unitflag.", AccessLevel.Developer)> _
    Public Function cmdEditUnitflag(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            objCharacter.cUnitFlags = Val("&H" & Message.Replace("0x", ""))
        Else
            objCharacter.cUnitFlags = Message
        End If
        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, objCharacter.cUnitFlags)
        objCharacter.SendCharacterUpdate()

        Return True
    End Function

    <ChatCommandAttribute("EditPlayerFlag", "EDITPLAYERFLAG <PLAYERFLAG> - Change your PLAYER_FLAGS.", AccessLevel.Developer)> _
    Public Function cmdEditPlayerflag(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            objCharacter.cPlayerFlags = Val("&H" & Message.Replace("0x", ""))
        Else
            objCharacter.cPlayerFlags = Message
        End If
        objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, objCharacter.cPlayerFlags)
        objCharacter.SendCharacterUpdate()

        Return True
    End Function

    <ChatCommandAttribute("EditPlayerFieldBytes", "EDITPLAYERFIELDBYTES <PLAYERFIELDBYTES> - Change your PLAYER_FIELD_BYTES.", AccessLevel.Developer)> _
    Public Function cmdEditPlayerFieldBytes(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            objCharacter.cPlayerFieldBytes = Val("&H" & Message.Replace("0x", ""))
        Else
            objCharacter.cPlayerFieldBytes = Message
        End If
        objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES, objCharacter.cPlayerFieldBytes)
        objCharacter.SendCharacterUpdate()

        Return True
    End Function

    <ChatCommandAttribute("GroupUpdate", "GROUPUPDATE - Get a groupupdate for selected player.", AccessLevel.Developer)> _
    Public Function cmdGroupUpdate(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).GroupUpdate(PartyMemberStatsFlag.GROUP_UPDATE_FULL)
            Return True
        End If

        Return False
    End Function

    <ChatCommandAttribute("PlaySound", "PLAYSOUND - Plays a specific sound for every player around you.", AccessLevel.Developer)> _
    Public Function cmdPlaySound(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim soundID As Integer = 0

        If Integer.TryParse(Message, soundID) = False Then Return False

        objCharacter.SendPlaySound(soundID)

        Return True
    End Function

    <ChatCommandAttribute("CombatList", "COMBATLIST - Lists everyone in your targets combatlist.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("CooldownList", "COOLDOWNLIST - Lists all cooldowns of your target.", AccessLevel.GameMaster)> _
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
                        sCooldowns &= "* Spell: " & Spell.Key & " - TimeLeft: " & GetTimeLeftString(timeLeft) & " sec" & " - Item: " & Spell.Value.CooldownItem & vbNewLine
                    End If
                End If
            Next
            objCharacter.CommandResponse(sCooldowns)
        Else
            objCharacter.CommandResponse("*Cooldowns not supported for creatures yet*")
        End If

        Return True
    End Function

    <ChatCommandAttribute("ClearCooldowns", "CLEARCOOLDOWNS - Clears all cooldowns of your target.", AccessLevel.Admin)> _
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
                CType(targetUnit, CharacterObject).Client.Send(packet)
                packet.Dispose()
            Next
        Else
            objCharacter.CommandResponse("Cooldowns are not supported for creatures yet.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("StartCheck", "STARTCHECK - Initialize Warden anti-cheat engine for selected character.", AccessLevel.Developer)> _
    Public Function cmdStartCheck(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
#If WARDEN Then
        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            MaievInit(CHARACTERs(objCharacter.TargetGUID))
        Else
            objCharacter.CommandResponse("No player target selected.")
        End If
#Else
        objCharacter.CommandResponse("Warden is not active.")
#End If

        Return True
    End Function

    <ChatCommandAttribute("SendCheck", "SENDCHECK - Sends a Warden anti-cheat check packet to the selected character.", AccessLevel.Developer)> _
    Public Function cmdSendCheck(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
#If WARDEN Then
        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            MaievSendCheck(CHARACTERs(objCharacter.TargetGUID))
        Else
            objCharacter.CommandResponse("No player target selected.")
        End If
#Else
        objCharacter.CommandResponse("Warden is not active.")
#End If

        Return True
    End Function

    <ChatCommandAttribute("GetSpeed", "GETSPEED - Displays all current speed.", AccessLevel.GameMaster)> _
    Public Function cmdGetSpeed(ByRef objCharacter As CharacterObject, ByVal tCopper As String) As Boolean
        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("WalkSpeed: " & CHARACTERs(objCharacter.TargetGUID).WalkSpeed)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("RunSpeed:" & CHARACTERs(objCharacter.TargetGUID).RunSpeed)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("RunBackSpeed:" & CHARACTERs(objCharacter.TargetGUID).RunBackSpeed)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("SwimSpeed:" & CHARACTERs(objCharacter.TargetGUID).SwimSpeed)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("SwimBackSpeed:" & CHARACTERs(objCharacter.TargetGUID).SwimBackSpeed)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("Turnrate:" & CHARACTERs(objCharacter.TargetGUID).TurnRate)
        Else
            objCharacter.CommandResponse("WalkSpeed: " & objCharacter.WalkSpeed)
            objCharacter.CommandResponse("RunSpeed:" & objCharacter.RunSpeed)
            objCharacter.CommandResponse("RunBackSpeed:" & objCharacter.RunBackSpeed)
            objCharacter.CommandResponse("SwimSpeed:" & objCharacter.SwimSpeed)
            objCharacter.CommandResponse("SwimBackSpeed:" & objCharacter.SwimBackSpeed)
            objCharacter.CommandResponse("Turnrate:" & objCharacter.TurnRate)
        End If

        Return True
    End Function

    <ChatCommandAttribute("GetAP", "GETAP - Displays attack power.", AccessLevel.GameMaster)> _
    Public Function cmdGetAttackPower(ByRef objCharacter As CharacterObject, ByVal tCopper As String) As Boolean
        If objCharacter.TargetGUID <> 0 AndAlso GuidIsPlayer(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("AttackPower: " & CHARACTERs(objCharacter.TargetGUID).AttackPower)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("AttackPowerMods: " & CHARACTERs(objCharacter.TargetGUID).AttackPowerMods)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("RangedAttackPower: " & CHARACTERs(objCharacter.TargetGUID).AttackPowerRanged)
            CHARACTERs(objCharacter.TargetGUID).CommandResponse("RangedAttackPowerMods: " & CHARACTERs(objCharacter.TargetGUID).AttackPowerModsRanged)
        Else
            objCharacter.CommandResponse("AttackPower: " & objCharacter.AttackPower)
            objCharacter.CommandResponse("AttackPowerMods: " & objCharacter.AttackPowerMods)
            objCharacter.CommandResponse("RangedAttackPower: " & objCharacter.AttackPowerRanged)
            objCharacter.CommandResponse("RangedAttackPowerMods: " & objCharacter.AttackPowerModsRanged)
        End If

        Return True
    End Function

    <ChatCommandAttribute("GetDPS", "GETDPS - Tells you about damage info.", AccessLevel.Developer)> _
    Public Function cmdGetDPS(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        objCharacter.CommandResponse("Ammo ID: " & objCharacter.AmmoID)
        objCharacter.CommandResponse("Ammo DPS: " & objCharacter.AmmoDPS)
        objCharacter.CommandResponse("Ammo Mod: " & objCharacter.AmmoMod)
        CalculateMinMaxDamage(objCharacter, WeaponAttackType.RANGED_ATTACK)

        Return True
    End Function

    <ChatCommandAttribute("AddItem", "ADDITEM <ID> <optional COUNT> - Add Y items with id X to selected character.", AccessLevel.GameMaster)> _
    Public Function cmdAddItem(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        If tmp.Length < 1 Then Return False

        Dim id As Integer = tmp(0)
        Dim Count As Integer = 1
        If tmp.Length = 2 Then Count = tmp(1)
        If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            Dim newItem As New ItemObject(id, objCharacter.TargetGUID)
            newItem.StackCount = Count
            If CHARACTERs(objCharacter.TargetGUID).ItemADD(newItem) Then
                CHARACTERs(objCharacter.TargetGUID).LogLootItem(newItem, Count, True, False)
            Else
                newItem.Delete()
            End If
        Else
            Dim newItem As New ItemObject(id, objCharacter.GUID)
            newItem.StackCount = Count
            If objCharacter.ItemADD(newItem) Then
                objCharacter.LogLootItem(newItem, Count, False, True)
            Else
                newItem.Delete()
            End If
        End If

        Return True
    End Function

    <ChatCommandAttribute("AddItemSet", "ADDITEMSET <ID> - Add the items in the item set with id X to selected character.", AccessLevel.GameMaster)> _
    Public Function cmdAddItemSet(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        If tmp.Length < 1 Then Return False

        Dim id As Integer = tmp(0)

        If ItemSet.ContainsKey(id) Then
            If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                For Each item As Integer In ItemSet(id).ItemID
                    Dim newItem As New ItemObject(item, objCharacter.TargetGUID)
                    newItem.StackCount = 1
                    If CHARACTERs(objCharacter.TargetGUID).ItemADD(newItem) Then
                        CHARACTERs(objCharacter.TargetGUID).LogLootItem(newItem, 1, False, True)
                    Else
                        newItem.Delete()
                    End If
                Next
            Else
                For Each item As Integer In ItemSet(id).ItemID
                    Dim newItem As New ItemObject(item, objCharacter.GUID)
                    newItem.StackCount = 1
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

    <ChatCommandAttribute("AddMoney", "ADDMONEY <XP> - Add X copper yours.", AccessLevel.GameMaster)> _
    Public Function cmdAddMoney(ByRef objCharacter As CharacterObject, ByVal tCopper As String) As Boolean
        If tCopper = "" Then Return False

        Dim Copper As ULong = tCopper

        If CType(objCharacter.Copper, ULong) + Copper > UInteger.MaxValue Then
            objCharacter.Copper = UInteger.MaxValue
        Else
            objCharacter.Copper += Copper
        End If

        objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, objCharacter.Copper)
        objCharacter.SendCharacterUpdate(False)

        Return True
    End Function

    <ChatCommandAttribute("LearnSkill", "LearnSkill <ID> <CURRENT> <MAX> - Add skill id X with value Y of Z to selected character.", AccessLevel.Developer)> _
    Public Function cmdLearnSkill(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            Dim tmp() As String
            tmp = Split(Trim(Message), " ")

            Dim SkillID As Integer = tmp(0)
            Dim Current As Int16 = tmp(1)
            Dim Maximum As Int16 = tmp(2)

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

    <ChatCommandAttribute("LearnSpell", "LearnSpell <ID> - Add spell X to selected character.", AccessLevel.Developer)> _
    Public Function cmdLearnSpell(ByRef objCharacter As CharacterObject, ByVal tID As String) As Boolean
        If tID = "" Then Return False

        Dim ID As Integer
        If Integer.TryParse(tID, ID) = False OrElse ID < 0 Then Return False
        If SPELLs.ContainsKey(ID) = False Then
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

    <ChatCommandAttribute("UnlearnSpell", "UnlearnSpell <ID> - Remove spell X from selected character.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("ShowTaxi", "SHOWTAXI - Unlock all taxi locations.", AccessLevel.Developer)> _
    Public Function cmdShowTaxi(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        objCharacter.TaxiZones.SetAll(True)
        Return True
    End Function

    <ChatCommandAttribute("SET", "SET <INDEX> <VALUE> - Set update value (A9).", AccessLevel.Developer)> _
    Public Function cmdSetUpdateField(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim tmp() As String = Split(Message, " ", 2)

        SetUpdateValue(objCharacter.TargetGUID, tmp(0), tmp(1), objCharacter.Client)
        Return True
    End Function

    <ChatCommandAttribute("SetRunSpeed", "SETRUNSPEED <VALUE> - Change your run speed.", AccessLevel.GameMaster)> _
    Public Function cmdSetRunSpeed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        objCharacter.ChangeSpeedForced(CharacterObject.ChangeSpeedType.RUN, Message)
        objCharacter.CommandResponse("Your RunSpeed is changed to " & Message)
        Return True
    End Function

    <ChatCommandAttribute("SetSwimSpeed", "SETSWIMSPEED <VALUE> - Change your swim speed.", AccessLevel.GameMaster)> _
    Public Function cmdSetSwimSpeed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        objCharacter.ChangeSpeedForced(CharacterObject.ChangeSpeedType.SWIM, Message)
        objCharacter.CommandResponse("Your SwimSpeed is changed to " & Message)
        Return True
    End Function

    <ChatCommandAttribute("SetRunBackSpeed", "SETRUNBACKSPEED <VALUE> - Change your run back speed.", AccessLevel.GameMaster)> _
    Public Function cmdSetRunBackSpeed(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        objCharacter.ChangeSpeedForced(CharacterObject.ChangeSpeedType.SWIMBACK, Message)
        objCharacter.CommandResponse("Your RunBackSpeed is changed to " & Message)
        Return True
    End Function

    <ChatCommandAttribute("SetReputation", "SETREPUTATION <FACTION> <VALUE> - Change your reputation standings.", AccessLevel.GameMaster)> _
    Public Function cmdSetReputation(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim tmp() As String = Split(Message, " ", 2)
        objCharacter.SetReputation(tmp(0), tmp(1))
        objCharacter.CommandResponse("You have set your reputation with [" & tmp(0) & "] to [" & tmp(1) & "]")
        Return True
    End Function

    <ChatCommandAttribute("Model", "MODEL <ID> - Will morph you into specified model ID.", AccessLevel.GameMaster)> _
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

    <ChatCommandAttribute("Mount", "MOUNT <ID> - Will mount you to specified model ID.", AccessLevel.GameMaster)> _
    Public Function cmdMount(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim value As Integer = 0
        If Integer.TryParse(Message, value) = False OrElse value < 0 Then Return False

        objCharacter.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, value)
        objCharacter.SendCharacterUpdate()
        Return True
    End Function

    <ChatCommandAttribute("Hurt", "HURT - Hurt selected character.", AccessLevel.GameMaster)> _
    Public Function cmdHurt(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).Life.Current -= CHARACTERs(objCharacter.TargetGUID).Life.Maximum * 0.1
            CHARACTERs(objCharacter.TargetGUID).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Life.Current)
            CHARACTERs(objCharacter.TargetGUID).SendCharacterUpdate()
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("Vulnerable", "VULNERABLE - Changes the selected characters vulnerability (ON/OFF).", AccessLevel.GameMaster)> _
    Public Function cmdVulnerable(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Message = Message.ToUpper()
        Dim Enabled As Boolean = False
        If Message = "ON" OrElse Message = "1" Then
            Enabled = False
        ElseIf Message = "OFF" OrElse Message = "0" Then
            Enabled = True
        Else
            Return False
        End If

        If objCharacter.TargetGUID = 0 OrElse GuidIsPlayer(objCharacter.TargetGUID) = False OrElse CHARACTERs.ContainsKey(objCharacter.TargetGUID) = False Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Invulnerable = Enabled

        Return True
    End Function

    <ChatCommandAttribute("Root", "ROOT - Instantly root selected character.", AccessLevel.GameMaster)> _
    Public Function cmdRoot(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SetMoveRoot()
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("UnRoot", "UNROOT - Instantly unroot selected character.", AccessLevel.GameMaster)> _
    Public Function cmdUnRoot(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SetMoveUnroot()
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("Revive", "REVIVE - Instantly revive selected character.", AccessLevel.GameMaster)> _
    Public Function cmdRevive(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CharacterResurrect(CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject))
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("GoToGraveyard", "GOTOGRAVEYARD - Instantly teleports selected character to nearest graveyard.", AccessLevel.GameMaster)> _
    Public Function cmdGoToGraveyard(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            AllGraveYards.GoToNearestGraveyard(CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject), False, True)
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("GoToStart", "GOTOSTART <RACE> - Instantly teleports selected character to specified race start location.", AccessLevel.GameMaster)> _
    Public Function cmdGoToStart(ByRef objCharacter As CharacterObject, ByVal StringRace As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            Dim Info As New DataTable
            Dim Character As CharacterObject = CHARACTERs(objCharacter.TargetGUID)
            Dim Race As Races

            Select Case UCase(StringRace)
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
            Character.Teleport(Info.Rows(0).Item("position_x"), Info.Rows(0).Item("position_y"), Info.Rows(0).Item("position_z"), 0, Info.Rows(0).Item("map"))
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("Summon", "SUMMON <NAME> - Instantly teleports the player to you.", AccessLevel.GameMaster)> _
    Public Function cmdSummon(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
        Dim GUID As ULong = GetGUID(CapitalizeName(Name))
        If CHARACTERs.ContainsKey(GUID) Then
            If objCharacter.OnTransport IsNot Nothing Then
                CType(CHARACTERs(GUID), CharacterObject).OnTransport = objCharacter.OnTransport
                CType(CHARACTERs(GUID), CharacterObject).Transfer(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
            Else
                CType(CHARACTERs(GUID), CharacterObject).Teleport(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
            End If
            Return True
        Else
            objCharacter.CommandResponse("Player not found.")
            Return True
        End If
    End Function

    <ChatCommandAttribute("Appear", "APPEAR <NAME> - Instantly teleports you to the player.", AccessLevel.GameMaster)> _
    Public Function cmdAppear(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
        Dim GUID As ULong = GetGUID(CapitalizeName(Name))
        If CHARACTERs.ContainsKey(GUID) Then
            With CType(CHARACTERs(GUID), CharacterObject)
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

    <ChatCommandAttribute("VmapTest", "VMAPTEST - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        If Config.VMapsEnabled Then
            Dim target As BaseUnit = Nothing
            If objCharacter.TargetGUID > 0 Then
                If GuidIsPlayer(objCharacter.TargetGUID) AndAlso CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                    target = CHARACTERs(objCharacter.TargetGUID)
                ElseIf GuidIsCreature(objCharacter.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
                    target = WORLD_CREATUREs(objCharacter.TargetGUID)
                    WORLD_CREATUREs(objCharacter.TargetGUID).SetToRealPosition()
                End If
            End If

            Dim timeStart As Integer = timeGetTime("")

            Dim height As Single = GetVMapHeight(objCharacter.MapID, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ + 2.0F)

            Dim isInLOS As Boolean = False
            If target IsNot Nothing Then
                isInLOS = IsInLineOfSight(objCharacter, target)
            End If

            Dim timeTaken As Integer = timeGetTime("") - timeStart

            If height = VMAP_INVALID_HEIGHT_VALUE Then
                objCharacter.CommandResponse(String.Format("Unable to retrieve VMap height for your location."))
            Else
                objCharacter.CommandResponse(String.Format("Your Z: {0}  VMap Z: {1}", objCharacter.positionZ, height))
            End If

            If target IsNot Nothing Then
                objCharacter.CommandResponse(String.Format("Target in line of sight: {0}", isInLOS))
            End If

            objCharacter.CommandResponse(String.Format("Vmap functionality ran under [{0} ms].", timeTaken))
        Else
            objCharacter.CommandResponse("Vmaps is not enabled.")
        End If
#Else
        objCharacter.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function

    <ChatCommandAttribute("VmapTest2", "VMAPTEST2 - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest2(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        If Config.VMapsEnabled Then
            If objCharacter.TargetGUID = 0UL OrElse GuidIsCreature(objCharacter.TargetGUID) = False OrElse WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) = False Then
                objCharacter.CommandResponse("You must target a creature first.")
            Else
                WORLD_CREATUREs(objCharacter.TargetGUID).SetToRealPosition()

                Dim resX As Single = 0.0F
                Dim resY As Single = 0.0F
                Dim resZ As Single = 0.0F
                Dim result As Boolean = GetObjectHitPos(objCharacter, WORLD_CREATUREs(objCharacter.TargetGUID), resX, resY, resZ, -1.0F)

                If result = False Then
                    objCharacter.CommandResponse("You teleported without any problems.")
                Else
                    objCharacter.CommandResponse("You teleported by hitting something.")
                End If

                objCharacter.orientation = GetOrientation(objCharacter.positionX, WORLD_CREATUREs(objCharacter.TargetGUID).positionX, objCharacter.positionY, WORLD_CREATUREs(objCharacter.TargetGUID).positionY)
                resZ = GetVMapHeight(objCharacter.MapID, resX, resY, resZ + 2.0F)
                objCharacter.Teleport(resX, resY, resZ, objCharacter.orientation, objCharacter.MapID)
            End If
        Else
            objCharacter.CommandResponse("Vmaps is not enabled.")
        End If
#Else
        objCharacter.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function

    <ChatCommandAttribute("VmapTest3", "VMAPTEST3 - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest3(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        Dim CellMap As UInteger = objCharacter.MapID
        Dim CellX As Byte = GetMapTileX(objCharacter.positionX)
        Dim CellY As Byte = GetMapTileY(objCharacter.positionY)

        Dim fileName As String = String.Format("{0}_{1}_{2}.vmdir", Format(CellMap, "000"), Format(CellX, "00"), Format(CellY, "00"))
        If Not IO.File.Exists("vmaps\" & fileName) Then
            objCharacter.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
            fileName = String.Format("{0}.vmdir", Format(CellMap, "000"))
        End If

        If Not IO.File.Exists("vmaps\" & fileName) Then
            objCharacter.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
        Else
            objCharacter.CommandResponse(String.Format("VMap file [{0}] found!", fileName))
            Dim map As TMap = Maps(CellMap)
            fileName = Trim(IO.File.ReadAllText("vmaps\" & fileName))

            objCharacter.CommandResponse(String.Format("Full file: '{0}'", fileName))
            If fileName.Contains(vbLf) Then
                fileName = fileName.Substring(0, fileName.IndexOf(vbLf))
            End If

            objCharacter.CommandResponse(String.Format("First line: '{0}'", fileName))
            Dim newModelLoaded As Boolean = False
            If fileName.Length > 0 AndAlso IO.File.Exists("vmaps\" & fileName) Then
                objCharacter.CommandResponse(String.Format("VMap file [{0}] found!", fileName))

                If Maps(CellMap).ContainsModelContainer(fileName) Then
                    objCharacter.CommandResponse(String.Format("VMap ModelContainer is loaded!"))
                Else
                    objCharacter.CommandResponse(String.Format("VMap ModelContainer is NOT loaded!"))
                End If
            Else
                objCharacter.CommandResponse(String.Format("VMap file [{0}] not found!", fileName))
            End If
        End If
#Else
        objCharacter.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function

    <ChatCommandAttribute("LineOfSight", "LINEOFSIGHT <ON/OFF> - Enables/Disables line of sight calculation.", AccessLevel.Developer)> _
    Public Function cmdLineOfSight(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message.ToUpper = "ON" Then
            Config.LineOfSightEnabled = True
            objCharacter.CommandResponse("Line of Sight Calculation is now Enabled.")
        ElseIf Message.ToUpper = "OFF" Then
            Config.LineOfSightEnabled = False
            objCharacter.CommandResponse("Line of Sight Calculation is now Disabled.")
        Else
            Return False
        End If
        Return True
    End Function

    <ChatCommandAttribute("GPS", "GPS - Tells you where you are located.", AccessLevel.GameMaster)> _
    Public Function cmdGPS(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        objCharacter.CommandResponse("X: " & objCharacter.positionX)
        objCharacter.CommandResponse("Y: " & objCharacter.positionY)
        objCharacter.CommandResponse("Z: " & objCharacter.positionZ)
        objCharacter.CommandResponse("Orientation: " & objCharacter.orientation)
        objCharacter.CommandResponse("Map: " & objCharacter.MapID)
        Return True
    End Function

    <ChatCommandAttribute("SetInstance", "SETINSTANCE <ID> - Sets you into another instance.", AccessLevel.GameMaster)> _
    Public Function cmdSetInstance(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim instanceID As Integer = 0
        If Integer.TryParse(Message, instanceID) = False Then Return False
        If instanceID < 0 OrElse instanceID > 400000 Then Return False

        objCharacter.instance = instanceID
        Return True
    End Function

    <ChatCommandAttribute("Port", "PORT <X> <Y> <Z> <ORIENTATION> <MAP> - Teleports Character To Given Coordinates.", AccessLevel.GameMaster)> _
    Public Function cmdPort(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False

        Dim tmp() As String
        tmp = Message.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

        If tmp.Length <> 5 Then Return False

        Dim posX As Single = CSng(tmp(0))
        Dim posY As Single = CSng(tmp(1))
        Dim posZ As Single = CSng(tmp(2))
        Dim posO As Single = CSng(tmp(3))
        Dim posMap As Integer = CSng(tmp(4))

        objCharacter.Teleport(posX, posY, posZ, posO, posMap)
        Return True
    End Function

    ''' <summary>
    ''' Teleports the player to a location.
    ''' </summary>
    ''' <param name="objCharacter">The objCharacter.</param>
    ''' <param name="location">The location. Use PortByName list to get a list of locations (can use * as wildcard).</param>
    ''' <returns></returns>
    <ChatCommandAttribute("PortByName", "PORT <LocationName> - Teleports Character To The LocationName Location. Use PortByName list to get a list of locations (can use * as wildcard).", AccessLevel.GameMaster)> _
    Public Function CmdPortByName(ByRef objCharacter As CharacterObject, ByVal location As String) As Boolean

        If location = "" Then Return False

        Dim posX As Single '= 0
        Dim posY As Single '= 0
        Dim posZ As Single '= 0
        Dim posO As Single '= 0
        Dim posMap As Integer '= 0

        If UCase(location) = "LIST" Then
            Dim cmdList As String = "Listing of available locations:" & vbNewLine

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

                posX = CType(mySqlQuery.Rows(0).Item("position_x"), Single)
                posY = CType(mySqlQuery.Rows(0).Item("position_y"), Single)
                posZ = CType(mySqlQuery.Rows(0).Item("position_z"), Single)
                posO = CType(mySqlQuery.Rows(0).Item("orientation"), Single)
                posMap = CType(mySqlQuery.Rows(0).Item("map"), Integer)
                objCharacter.Teleport(posX, posY, posZ, posO, posMap)
            Else
                Dim cmdList As String = "Listing of matching locations:" & vbNewLine

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

    <ChatCommandAttribute("Slap", "SLAP <DAMAGE> - Slap target creature or player for X damage.", AccessLevel.Admin)> _
    Public Function cmdSlap(ByRef objCharacter As CharacterObject, ByVal tDamage As String) As Boolean
        Dim Damage As Integer = tDamage

        If GuidIsCreature(objCharacter.TargetGUID) Then
            CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject).DealDamage(Damage)
        ElseIf GuidIsPlayer(objCharacter.TargetGUID) Then
            CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).DealDamage(Damage)
            CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).SystemMessage(objCharacter.Name & " slaps you for " & Damage & " damage.")
        Else
            objCharacter.CommandResponse("Not supported target selected.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("Kick", "KICK <optional NAME> - Kick selected player or character with name specified if found.", AccessLevel.GameMaster)> _
    Public Function cmdKick(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            'DONE: Kick by selection
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                'DONE: Kick gracefully
                objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Client.Character.Name, CHARACTERs(objCharacter.TargetGUID).Name)
                CHARACTERs(objCharacter.TargetGUID).Logout()
            Else
                objCharacter.CommandResponse(String.Format("Character GUID=[{0}] not found.", objCharacter.TargetGUID))
            End If

        Else

            'DONE: Kick by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    'DONE: Kick gracefully
                    Character.Value.Logout()
                    objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(Character.Value, CharacterObject).Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Client.Character.Name, Name)
                    Return True
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            objCharacter.CommandResponse(String.Format("Character [{0:X}] not found.", Name))

        End If
        Return True
    End Function

    <ChatCommandAttribute("KickReason", "KICKREASON <TEXT> - Display message for 2 seconds and kick selected player.", AccessLevel.GameMaster)> _
    Public Function cmdKickReason(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("No target selected.")
        Else
            SystemMessage(String.Format("Character [{0}] kicked form server.{3}Reason: {1}{3}GameMaster: [{2}].", SetColor(CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Name, 255, 0, 0), SetColor(Message, 255, 0, 0), SetColor(objCharacter.Name, 255, 0, 0), vbNewLine))
            Thread.Sleep(2000)

            cmdKick(objCharacter, "")
        End If

        Return True
    End Function

    <ChatCommandAttribute("Disconnect", "DISCONNECT <optional NAME> - Disconnects selected player or character with name specified if found.", AccessLevel.GameMaster)> _
    Public Function cmdDisconnect(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            'DONE: Kick by selection
            If objCharacter.TargetGUID = 0 Then
                objCharacter.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
                objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Client.Character.Name, CHARACTERs(objCharacter.TargetGUID).Name)
                CType(CHARACTERs(objCharacter.TargetGUID), CharacterObject).Client.Disconnect()
            Else
                objCharacter.CommandResponse(String.Format("Character GUID=[{0}] not found.", objCharacter.TargetGUID))
            End If

        Else

            'DONE: Kick by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    objCharacter.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(Character.Value, CharacterObject).Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Client.Character.Name, Name)
                    CType(Character.Value, CharacterObject).Client.Disconnect()
                    Return True
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            objCharacter.CommandResponse(String.Format("Character [{0:X}] not found.", Name))

        End If
        Return True
    End Function

    <ChatCommandAttribute("ForceRename", "FORCERENAME - Force selected player to change his name next time on char enum.", AccessLevel.GameMaster)> _
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

    <ChatCommandAttribute("BanChar", "BANCHAR - Selected player won't be able to login next time with this character.", AccessLevel.GameMaster)> _
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

    <ChatCommandAttribute("Ban", "BAN <ACCOUNT> - Ban specified account from server.", AccessLevel.GameMaster)> _
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
                AccountDatabase.Update(String.Format("INSERT INTO `account_banned` VALUES ('{0}', UNIX_TIMESTAMP({1}), UNIX_TIMESTAMP({2}), '{3}', '{4}', active = 1);", accountID, Format(Now, "yyyy-MM-dd HH:mm:ss"), "0000-00-00 00:00:00", objCharacter.Name, "No Reason Specified."))
                AccountDatabase.Update(String.Format("INSERT INTO `ip_banned` VALUES ('{0}', UNIX_TIMESTAMP({1}), UNIX_TIMESTAMP({2}), '{3}', '{4}');", IP, Format(Now, "yyyy-MM-dd HH:mm:ss"), "0000-00-00 00:00:00", objCharacter.Name, "No Reason Specified."))
                objCharacter.CommandResponse(String.Format("Account [{0}] banned.", Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] banned by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Name, Name)
            End If
        Else
            objCharacter.CommandResponse(String.Format("Account [{0}] not found.", Name))
        End If

        Return True
    End Function

    <ChatCommandAttribute("UnBan", "UNBAN <ACCOUNT> - Remove ban of specified account from server.", AccessLevel.GameMaster)> _
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
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] unbanned by [{2}].", objCharacter.Client.IP.ToString, objCharacter.Client.Port, objCharacter.Name, Name)
            End If
        Else
            objCharacter.CommandResponse(String.Format("Account [{0}] not found.", Name))
        End If

        Return True
    End Function

    <ChatCommandAttribute("Info", "INFO <optional NAME> - Show account information for selected target or character with name specified if found.", AccessLevel.GameMaster)> _
    Public Function cmdInfo(ByRef objCharacter As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            Dim GUID As ULong = objCharacter.TargetGUID

            'DONE: Info by selection
            If CHARACTERs.ContainsKey(GUID) Then
                objCharacter.CommandResponse(String.Format("Information for character [{0}]:{1}account = {2}{1}ip = {3}{1}guid = {4:X}{1}access = {5}{1}boundingradius = {6}{1}combatreach = {7}", _
                CHARACTERs(GUID).Name, vbNewLine, _
                CHARACTERs(GUID).Client.Account, _
                CHARACTERs(GUID).Client.IP.ToString, _
                CHARACTERs(GUID).GUID, _
                CHARACTERs(GUID).Access, _
                CHARACTERs(GUID).BoundingRadius, _
                CHARACTERs(GUID).CombatReach))
            ElseIf WORLD_CREATUREs.ContainsKey(GUID) Then
                objCharacter.CommandResponse(String.Format("Information for creature [{0}]:{1}id = {2}{1}guid = {3:X}{1}model = {4}{1}boundingradius = {11}{1}combatreach = {12}{1}ai = {5}{1}his reaction = {6}{1}guard = {7}{1}waypoint = {10}{1}damage = {8}-{9}", _
                WORLD_CREATUREs(GUID).Name, vbNewLine, _
                WORLD_CREATUREs(GUID).ID, _
                GUID, _
                WORLD_CREATUREs(GUID).Model, _
                WORLD_CREATUREs(GUID).aiScript.GetType().ToString, _
                objCharacter.GetReaction(WORLD_CREATUREs(GUID).Faction), _
                WORLD_CREATUREs(GUID).isGuard, _
                WORLD_CREATUREs(GUID).CreatureInfo.Damage.Minimum, WORLD_CREATUREs(GUID).CreatureInfo.Damage.Maximum, _
                (WORLD_CREATUREs(GUID).MoveType = 2), _
                WORLD_CREATUREs(GUID).BoundingRadius, _
                WORLD_CREATUREs(GUID).CombatReach))
            ElseIf WORLD_GAMEOBJECTs.ContainsKey(GUID) Then
                objCharacter.CommandResponse(String.Format("Information for gameobject [{0}]:{1}id = {2}{1}guid = {3:X}{1}model = {4}", _
                WORLD_GAMEOBJECTs(GUID).Name, vbNewLine, _
                WORLD_GAMEOBJECTs(GUID).ID, _
                GUID, _
                GAMEOBJECTSDatabase(WORLD_GAMEOBJECTs(GUID).ID).Model))
            Else
                objCharacter.CommandResponse(String.Format("Information about yourself.{0}guid = {1:X}{0}model = {2}{0}mount = {3}", _
                vbNewLine, objCharacter.GUID, objCharacter.Model, objCharacter.Mount))
            End If

        Else

            'DONE: Info by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    objCharacter.CommandResponse(String.Format("Information for character [{0}]:{1}account = {2}{1}ip = {3}{1}guid = {4}{1}access = {5}", _
                    CType(Character.Value, CharacterObject).Name, vbNewLine, _
                    CType(Character.Value, CharacterObject).Client.Account, _
                    CType(Character.Value, CharacterObject).Client.IP.ToString, _
                    CType(Character.Value, CharacterObject).GUID, _
                    CType(Character.Value, CharacterObject).Access))
                    Exit Function
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            objCharacter.CommandResponse(String.Format("Character [{0}] not found.", Name))

        End If

        Return True
    End Function

    <ChatCommandAttribute("Where", "WHERE - Display your position information.", AccessLevel.GameMaster)> _
    Public Function cmdWhere(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        objCharacter.SystemMessage(String.Format("Coords: x={0}, y={1}, z={2}, or={3}, map={4}", objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID))
        objCharacter.SystemMessage(String.Format("Cell: {0},{1} SubCell: {2},{3}", GetMapTileX(objCharacter.positionX), GetMapTileY(objCharacter.positionY), GetSubMapTileX(objCharacter.positionX), GetSubMapTileY(objCharacter.positionY)))
        objCharacter.SystemMessage(String.Format("ZCoords: {0} AreaFlag: {1} WaterLevel={2}", GetZCoord(objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.MapID), GetAreaFlag(objCharacter.positionX, objCharacter.positionY, objCharacter.MapID), GetWaterLevel(objCharacter.positionX, objCharacter.positionY, objCharacter.MapID)))
        objCharacter.ZoneCheck()
        objCharacter.SystemMessage(String.Format("ZoneID: {0}", objCharacter.ZoneID))
#If ENABLE_PPOINTS Then
        objCharacter.SystemMessage(String.Format("ZCoords_PP: {0}", GetZCoord_PP(objCharacter.positionX, objCharacter.positionY, objCharacter.MapID)))
#End If

        Return True
    End Function

    <ChatCommandAttribute("SetGM", "SETGM <FLAG> <INVISIBILITY> - Toggles gameMaster status. You can use values like On/Off/1/0.", AccessLevel.GameMaster)> _
    Public Function cmdSetGM(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        Dim value1 As String = tmp(0)
        Dim value2 As String = tmp(1)

        'setFaction(35);
        'SetFlag(PLAYER_BYTES_2, 0x8);

        'Commnad: .setgm <gmflag:0/1/off/on> <invisibility:0/1/off/on>
        If value1 = "0" Or UCase(value1) = "OFF" Then
            objCharacter.GM = False
            objCharacter.CommandResponse("GameMaster Flag turned off.")
        Else
            objCharacter.GM = True
            objCharacter.CommandResponse("GameMaster Flag turned on.")
        End If
        If value2 = "0" Or UCase(value2) = "OFF" Then
            objCharacter.Invisibility = InvisibilityLevel.VISIBLE
            objCharacter.CanSeeInvisibility = InvisibilityLevel.VISIBLE
            objCharacter.CommandResponse("GameMaster Invisibility turned off.")
        Else
            objCharacter.Invisibility = InvisibilityLevel.GM
            objCharacter.CanSeeInvisibility = InvisibilityLevel.GM
            objCharacter.CommandResponse("GameMaster Invisibility turned on.")
        End If
        objCharacter.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, objCharacter.cPlayerFlags)
        objCharacter.SendCharacterUpdate()
        UpdateCell(objCharacter)

        Return True
    End Function

    <ChatCommandAttribute("SetWeather", "SETWEATHER <TYPE> <INTENSITY> - Change weather in current zone. Intensity is float value!", AccessLevel.Developer)> _
    Public Function cmdSetWeather(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        Dim Type As Integer = tmp(0)
        Dim Intensity As Single = tmp(1)

        If WeatherZones.ContainsKey(objCharacter.ZoneID) = False Then
            objCharacter.CommandResponse("No weather for this zone is found!")
        Else
            WeatherZones(objCharacter.ZoneID).CurrentWeather = Type
            WeatherZones(objCharacter.ZoneID).Intensity = Intensity
            SendWeather(objCharacter.ZoneID, objCharacter.Client)
        End If

        Return True
    End Function

    <ChatCommandAttribute("Remove", "REMOVE <ID> - Delete selected creature or gameobject.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("Turn", "TURN - Selected creature or game object will turn to your position.", AccessLevel.Developer)> _
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

            CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject).TurnTo(objCharacter.positionX, objCharacter.positionY)

        ElseIf GuidIsGameObject(objCharacter.TargetGUID) Then
            'DONE: Turn GO
            If Not WORLD_GAMEOBJECTs.ContainsKey(objCharacter.TargetGUID) Then
                objCharacter.CommandResponse("Selected target is not game object!")
                Return True
            End If

            CType(WORLD_GAMEOBJECTs(objCharacter.TargetGUID), GameObjectObject).TurnTo(objCharacter.positionX, objCharacter.positionY)

            Dim q As New DataTable
            Dim GUID As ULong = objCharacter.TargetGUID - GUID_GAMEOBJECT

            objCharacter.CommandResponse("Object rotation will be visible when the object is reloaded!")

        End If

        Return True
    End Function

    <ChatCommandAttribute("AddNPC", "ADDNPC <ID> - Spawn creature at your position.", AccessLevel.Developer)> _
    Public Function cmdAddCreature(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean

        Dim tmpCr As CreatureObject = New CreatureObject(CType(Message, Integer), objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
        tmpCr.AddToWorld()
        objCharacter.CommandResponse("Creature [" & tmpCr.Name & "] spawned.")

        Return True
    End Function

    <ChatCommandAttribute("NPCFlood", "NPCFLOOD <Amount> - Spawn a number of creatures at your position.", AccessLevel.Developer)> _
    Public Function cmdCreatureFlood(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False OrElse CInt(Message) <= 0 Then Return False
        For i As Integer = 1 To CInt(Message)
            Dim tmpCreature As New CreatureObject(7385, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation, objCharacter.MapID)
            tmpCreature.CreatedBy = objCharacter.GUID
            tmpCreature.CreatedBySpell = 10673
            tmpCreature.aiScript = New DefaultAI(tmpCreature)
            tmpCreature.AddToWorld()
        Next

        Return True
    End Function

    <ChatCommandAttribute("Come", "COME - Selected creature will come to your position.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("Kill", "KILL - Selected creature or character will die.", AccessLevel.GameMaster)> _
    Public Function cmdKillCreature(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean
        If objCharacter.TargetGUID = 0 Then
            objCharacter.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(objCharacter.TargetGUID) Then
            CHARACTERs(objCharacter.TargetGUID).Die(objCharacter)
            Return True
        ElseIf WORLD_CREATUREs.ContainsKey(objCharacter.TargetGUID) Then
            CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject).DealDamage(CType(WORLD_CREATUREs(objCharacter.TargetGUID), CreatureObject).Life.Maximum)
            Return True
        End If
        Return False
    End Function

    <ChatCommandAttribute("TargetGo", "TARGETGO - Nearest game object will be selected.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("ActivateGo", "ACTIVATEGO - Activates your targetted game object.", AccessLevel.Developer)> _
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

    <ChatCommandAttribute("AddGameObject", "ADDGAMEOBJECT <ID> - Spawn game object at your position.", AccessLevel.Developer)> _
    Public Function cmdAddGameObject(ByRef objCharacter As CharacterObject, ByVal Message As String) As Boolean

        Dim tmpGO As GameObjectObject = New GameObjectObject(CType(Message, Integer), objCharacter.MapID, objCharacter.positionX, objCharacter.positionY, objCharacter.positionZ, objCharacter.orientation)
        tmpGO.Rotations(2) = Math.Sin(tmpGO.orientation / 2)
        tmpGO.Rotations(3) = Math.Cos(tmpGO.orientation / 2)
        tmpGO.AddToWorld()

        objCharacter.CommandResponse(String.Format("GameObject [{0}][{1:X}] spawned.", tmpGO.Name, tmpGO.GUID))

        Return True
    End Function

    <ChatCommandAttribute("CreateAccount", "CreateAccount <Name> <Password> <Email> - Add a new account using Name, Password, and Email.", AccessLevel.Admin)> _
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

    <ChatCommandAttribute("ChangePassword", "ChangePassword <Name> <Password> - Changes the password of an account.", AccessLevel.Admin)> _
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
            Dim targetLevel As AccessLevel = CType(result.Rows(0).Item("gmlevel"), AccessLevel)
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

    <ChatCommandAttribute("SetAccess", "SetAccess <Name> <AccessLevel> - Sets the account to a specific access level.", AccessLevel.Admin)> _
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

        Dim newLevel As AccessLevel = CType(aLevel, AccessLevel)
        If newLevel >= objCharacter.Access Then
            objCharacter.CommandResponse("You cannot set access levels to your own or above your own access level.")
            Return True
        End If

        AccountDatabase.Query("SELECT id, gmlevel FROM account WHERE username = """ & aName & """;", result)
        If result.Rows.Count = 0 Then
            objCharacter.CommandResponse(String.Format("Account [{0}] does not exist.", aName))
        Else
            Dim targetLevel As AccessLevel = CType(result.Rows(0).Item("gmlevel"), AccessLevel)
            If targetLevel >= objCharacter.Access Then
                objCharacter.CommandResponse("You cannot set access levels to accounts with the same or a higher access level than yourself.")
            Else
                AccountDatabase.Update(String.Format("UPDATE account SET gmlevel={0} WHERE id={1}", CByte(newLevel), result.Rows(0).Item("id")))
                objCharacter.CommandResponse(String.Format("Account [{0}] now has access level [{1}].", aName, newLevel))
            End If
        End If
        Return True
    End Function

#End Region

#Region "WS.Commands.InternalCommands.HelperSubs"

    Public Function GetGUID(ByVal Name As String) As ULong
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE char_name = ""{0}"";", Name), MySQLQuery)

        If MySQLQuery.Rows.Count > 0 Then
            Return CType(MySQLQuery.Rows(0).Item("char_guid"), ULong)
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