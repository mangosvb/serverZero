' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
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
Imports SpuriousZero.Common.BaseWriter


#Region "WS.Commands.Attributes"
<AttributeUsage(AttributeTargets.Method, Inherited:=False, AllowMultiple:=True)> _
    Public Class ChatCommandAttribute
    Inherits System.Attribute

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


    Public Const WardenGUID As ULong = Integer.MaxValue
    Public Const WardenNAME As String = "Warden"
    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Admin = 3
        Developer = 4
    End Enum



    Public ChatCommands As New Dictionary(Of String, ChatCommand)
    Public Class ChatCommand
        Public CommandHelp As String
        Public CommandAccess As AccessLevel = AccessLevel.GameMaster
        Public CommandDelegate As ChatCommandDelegate
    End Class
    Public Delegate Function ChatCommandDelegate(ByRef c As CharacterObject, ByVal Message As String) As Boolean


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
                        Log.WriteLine(SpuriousZero.Common.BaseWriter.LogType.INFORMATION, "Command found: {0}", UCase(info.cmdName))
#End If
                    Next
                End If
            Next
        Next

    End Sub
    Public Sub OnCommand(ByRef Client As ClientClass, ByVal Message As String)
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
            Dim Name As String = Client.Character.Name


            If Command Is Nothing Then
                Client.Character.CommandResponse("Unknown command.")
            ElseIf Command.CommandAccess > Client.Character.Access Then
                Client.Character.CommandResponse("This command is not available for your access level.")
            ElseIf Not Command.CommandDelegate(Client.Character, Arguments) Then
                Client.Character.CommandResponse(Command.CommandHelp)
            Else
                Log.WriteLine(LogType.USER, "[{0}:{1}] {2} used command: {3}", Client.IP, Client.Port, Name, Message)
            End If

        Catch err As Exception
            Log.WriteLine(LogType.FAILED, "[{0}:{1}] Client command caused error! {3}{2}", Client.IP, Client.Port, err.ToString, vbNewLine)
            Client.Character.CommandResponse(String.Format("Your command caused error:" & vbNewLine & " [{0}]", err.Message))
        End Try
    End Sub


#End Region
#Region "WS.Commands.InternalCommands"


    <ChatCommandAttribute("Help", "HELP <CMD>" & vbNewLine & "Displays usage information about command, if no command specified - displays list of available commands.")> _
    Public Function Help(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Trim(Message) <> "" Then
            Dim Command As ChatCommand = CType(ChatCommands(Trim(UCase(Message))), ChatCommand)
            If Command Is Nothing Then
                c.CommandResponse("Unknown command.")
            ElseIf Command.CommandAccess > c.Access Then
                c.CommandResponse("This command is not available for your access level.")
            Else
                c.CommandResponse(Command.CommandHelp)
            End If
        Else
            Dim cmdList As String = "Listing available commands:" & vbNewLine
            For Each Command As KeyValuePair(Of String, ChatCommand) In ChatCommands
                If CType(Command.Value, ChatCommand).CommandAccess <= c.Access Then cmdList += UCase(Command.Key) & ", "
            Next
            cmdList += vbNewLine + "Use HELP <CMD> for usage information about particular command."
            c.CommandResponse(cmdList)
        End If

        Return True
    End Function

    '****************************************** DEVELOPER COMMANDs *************************************************
    Dim x As Integer = 0
    <ChatCommandAttribute("Test", "This is test command used for debugging. Do not use if you don't know what it does!", AccessLevel.Developer)> _
    Public Function cmdTest(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        WORLD_GAMEOBJECTs(c.TargetGUID).orientation = 0
        WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(0) = 0
        WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(1) = 0
        WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(2) = 0
        WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(3) = 0

        Select Case x
            Case 0
                WORLD_GAMEOBJECTs(c.TargetGUID).orientation = c.orientation
            Case 1
                WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(0) = c.orientation
            Case 2
                WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(1) = c.orientation
            Case 3
                WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(2) = c.orientation
            Case 4
                WORLD_GAMEOBJECTs(c.TargetGUID).Rotations(3) = c.orientation
        End Select

        x += 1
        If x = 5 Then x = 0


        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
        packet.AddInt32(1)
        packet.AddInt8(0)
        Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_GAMEOBJECT)
        WORLD_GAMEOBJECTs(c.TargetGUID).FillAllUpdateFlags(tmpUpdate, c)
        tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(WORLD_GAMEOBJECTs(c.TargetGUID), GameObjectObject))
        tmpUpdate.Dispose()

        c.Client.Send(packet)

        packet.Dispose()

        Return True
    End Function
    Dim currentSpError As SpellFailedReason = SpellFailedReason.SPELL_NO_ERROR
    <ChatCommandAttribute("SpellFailedMSG", "SPELLFAILEDMSG <optional ID> - Sends test spell failed message.", AccessLevel.Developer)> _
    Public Function cmdSpellFailed(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then
            currentSpError += 1
        Else
            currentSpError = Message
        End If
        SendCastResult(currentSpError, c.Client, 133)
        c.CommandResponse(String.Format("Sent spell failed message:{2} {0} = {1}", currentSpError, CType(currentSpError, Integer), vbNewLine))
        Return True
    End Function
    Dim currentInvError As InventoryChangeFailure = InventoryChangeFailure.EQUIP_ERR_OK
    <ChatCommandAttribute("InvFailedMSG", "INVFAILEDMSG <optional ID> - Sends test inventory failed message.", AccessLevel.Developer)> _
    Public Function cmdInventoryFailed(ByRef c As CharacterObject, ByVal Message As String) As Boolean
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
        c.Client.Send(response)
        response.Dispose()
        c.CommandResponse(String.Format("Sent spell failed message:{2} {0} = {1}", currentInvError, CType(currentInvError, Integer), vbNewLine))
        Return True
    End Function
    Dim currentInstanceResetError As ResetFailedReason = ResetFailedReason.INSTANCE_RESET_FAILED_ZONING
    <ChatCommandAttribute("InstanceResetFailedMSG", "INSTANCERESETFAILEDMSG <optional ID> - Sends test inventory failed message.", AccessLevel.Developer)> _
    Public Function cmdInstanceResetFailedReason(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then
            currentInstanceResetError += 1
        Else
            currentInstanceResetError = Message
        End If
        SendResetInstanceFailed(c.Client, c.MapID, currentInstanceResetError)
        c.CommandResponse(String.Format("Sent instance failed message:{2} {0} = {1}", currentInstanceResetError, CType(currentInstanceResetError, Integer), vbNewLine))
        Return True
    End Function

    <ChatCommandAttribute("CastSpell", "CASTSPELL <SpellID> <Target> - Selected unit will start casting spell. Target can be ME or SELF.", AccessLevel.Developer)> _
    Public Function cmdCastSpellMe(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp As String() = Split(Message, " ", 2)
        If tmp.Length < 2 Then Return False
        Dim SpellID As Integer = tmp(0)
        Dim Target As String = UCase(tmp(1))

        If GuidIsCreature(c.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            Select Case Target
                Case "ME"
                    WORLD_CREATUREs(c.TargetGUID).CastSpell(SpellID, c)
                Case "SELF"
                    WORLD_CREATUREs(c.TargetGUID).CastSpell(SpellID, CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject))
            End Select
        ElseIf GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            Select Case Target
                Case "ME"
                    Dim Targets As New SpellTargets
                    Targets.SetTarget_UNIT(c)
                    Dim castParams As New CastSpellParameters(Targets, CHARACTERs(c.TargetGUID), SpellID)
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
                Case "SELF"
                    CHARACTERs(c.TargetGUID).CastOnSelf(SpellID)
            End Select
        Else
            c.CommandResponse(String.Format("GUID=[{0:X}] not found or unsupported.", c.TargetGUID))
        End If

        Return True
    End Function
    <ChatCommandAttribute("Control", "CONTROL - Takes or removes control over the selected unit.", AccessLevel.Admin)> _
    Public Function cmdControl(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.MindControl IsNot Nothing Then
            If TypeOf c.MindControl Is CharacterObject Then
                Dim packet1 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
                packet1.AddPackGUID(c.MindControl.GUID)
                packet1.AddInt8(1)
                CType(c.MindControl, CharacterObject).Client.Send(packet1)
                packet1.Dispose()
            End If

            Dim packet3 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
            packet3.AddPackGUID(c.MindControl.GUID)
            packet3.AddInt8(0)
            c.Client.Send(packet3)
            packet3.Dispose()

            c.cUnitFlags = c.cUnitFlags And (Not UnitFlags.UNIT_FLAG_UNK21)
            c.SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, 0)
            c.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, c.cUnitFlags)
            c.SendCharacterUpdate(False)

            c.MindControl = Nothing

            c.CommandResponse("Removed control over the unit.")
            Return True
        End If

        If GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            Dim packet1 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
            packet1.AddPackGUID(c.TargetGUID)
            packet1.AddInt8(0)
            CHARACTERs(c.TargetGUID).Client.Send(packet1)
            packet1.Dispose()

            c.MindControl = CHARACTERs(c.TargetGUID)
        ElseIf GuidIsCreature(c.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            c.MindControl = WORLD_CREATUREs(c.TargetGUID)
        Else
            c.CommandResponse("You need a target.")
            Return True
        End If

        Dim packet2 As New PacketClass(OPCODES.SMSG_DEATH_NOTIFY_OBSOLETE)
        packet2.AddPackGUID(c.TargetGUID)
        packet2.AddInt8(1)
        c.Client.Send(packet2)
        packet2.Dispose()

        c.cUnitFlags = c.cUnitFlags Or UnitFlags.UNIT_FLAG_UNK21
        c.SetUpdateFlag(EPlayerFields.PLAYER_FARSIGHT, c.TargetGUID)
        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, c.cUnitFlags)
        c.SendCharacterUpdate(False)

        c.CommandResponse("Taken control over a unit.")

        Return True
    End Function
    <ChatCommandAttribute("CreateGuild", "CreateGuild <Name> - Creates a guild.", AccessLevel.Developer)> _
    Public Function cmdCreateGuild(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        'TODO: Creating guilds must be done in the cluster

        'Dim GuildName As String = Message

        'Dim MySQLQuery As New DataTable
        'Database.Query(String.Format("INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES ('{0}', {1}, {2}, {3}, {4}); SELECT guild_id FROM guilds WHERE guild_name = '{0}';", GuildName, c.GUID, Now.Year, Now.Month, Now.Day), MySQLQuery)

        'AddCharacterToGuild(c, MySQLQuery.Rows(0).Item("guild_id"), 0)
        Return True
    End Function
    <ChatCommandAttribute("Cast", "CAST <SpellID> - You will start casting spell on selected target.", AccessLevel.Developer)> _
    Public Function cmdCastSpell(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp As String() = Split(Message, " ", 2)
        Dim SpellID As Integer = tmp(0)

        If GuidIsCreature(c.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            Dim Targets As New SpellTargets
            Targets.SetTarget_UNIT(WORLD_CREATUREs(c.TargetGUID))
            Dim castParams As New CastSpellParameters(Targets, c, SpellID)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

            c.CommandResponse("You are now casting [" & SpellID & "] at [" & WORLD_CREATUREs(c.TargetGUID).Name & "].")
        ElseIf GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            Dim Targets As New SpellTargets
            Targets.SetTarget_UNIT(CHARACTERs(c.TargetGUID))
            Dim castParams As New CastSpellParameters(Targets, c, SpellID)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

            c.CommandResponse("You are now casting [" & SpellID & "] at [" & CHARACTERs(c.TargetGUID).Name & "].")
        Else
            c.CommandResponse(String.Format("GUID=[{0:X}] not found or unsupported.", c.TargetGUID))
        End If

        Return True
    End Function
    <ChatCommandAttribute("Save", "SAVE - Saves selected character.", AccessLevel.GameMaster)> _
    Public Function cmdSave(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).Save()
            CHARACTERs(c.TargetGUID).CommandResponse(String.Format("Character {0} saved.", CHARACTERs(c.TargetGUID).Name))
        Else
            c.Save()
            c.CommandResponse(String.Format("Character {0} saved.", c.Name))
        End If
        
        Return True
    End Function
    <ChatCommandAttribute("AddWardenToParty", "This command will add the command bot to you group.", AccessLevel.Developer)> _
    Public Function cmdAddWardenToParty(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        'Dim Warden As New CharacterObject
        'Warden.Name = WardenNAME
        'Warden.GUID = WardenGUID
        'Warden.Client = New ClientClass
        'Warden.Client.DEBUG_CONNECTION = True

        'c.Party = New BaseParty(c)
        'c.Party.AddCharacter(Warden)

        c.CommandResponse("This command is disabled for now")
        Return True
    End Function
    <ChatCommandAttribute("Spawns", "SPAWNS - Tells you the spawn in memory information.", AccessLevel.Developer)> _
    Public Function cmdSpawns(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.CommandResponse("Spawns loaded in server memory:")
        c.CommandResponse("-------------------------------")
        c.CommandResponse("Creatures: " & WORLD_CREATUREs.Count)
        c.CommandResponse("GameObjects: " & WORLD_GAMEOBJECTs.Count)

        Return True
    End Function
    <ChatCommandAttribute("Near", "NEAR - Tells you the near objects count.", AccessLevel.Developer)> _
    Public Function cmdNear(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.CommandResponse("Near objects:")
        c.CommandResponse("-------------------------------")
        c.CommandResponse("Players: " & c.playersNear.Count)
        c.CommandResponse("Creatures: " & c.creaturesNear.Count)
        c.CommandResponse("GameObjects: " & c.gameObjectsNear.Count)
        c.CommandResponse("Corpses: " & c.corpseObjectsNear.Count)
        c.CommandResponse("-------------------------------")
        c.CommandResponse("You are seen by: " & c.SeenBy.Count)
        Return True
    End Function

    <ChatCommandAttribute("SetWaterWalk", "SETWATERWALK <TRUE/FALSE> - Enables/Disables walking over water for selected target.", AccessLevel.Developer)> _
    Public Function cmdSetWaterWalk(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            If UCase(Message) = "TRUE" Then
                CType(CHARACTERs(c.TargetGUID), CharacterObject).SetWaterWalk()
            ElseIf UCase(Message) = "FALSE" Then
                CType(CHARACTERs(c.TargetGUID), CharacterObject).SetLandWalk()
            Else
                Return False
            End If
        Else
            c.CommandResponse("Select target is not character!")
            Return True
        End If
    End Function
    <ChatCommandAttribute("AI", "AI <ENABLE/DISABLE> - Enables/Disables AI updating.", AccessLevel.Developer)> _
    Public Function cmdAI(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If UCase(Message) = "ENABLE" Then
            AIManager.AIManagerTimer.Change(TAIManager.UPDATE_TIMER, TAIManager.UPDATE_TIMER)
            c.CommandResponse("AI is enabled.")
        ElseIf UCase(Message) = "DISABLE" Then
            AIManager.AIManagerTimer.Change(Timeout.Infinite, Timeout.Infinite)
            c.CommandResponse("AI is disabled.")
        Else
            Return False
        End If

        Return True
    End Function
    <ChatCommandAttribute("AIState", "AIState - Shows debug information about AI state of selected creature.", AccessLevel.Developer)> _
    Public Function cmdAIState(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Exit Function
        End If
        If Not WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            c.CommandResponse("Selected target is not creature!")
            Exit Function
        End If

        If WORLD_CREATUREs(c.TargetGUID).aiScript Is Nothing Then
            c.CommandResponse("This creature doesn't have AI")
        Else
            With WORLD_CREATUREs(c.TargetGUID)
                c.CommandResponse(String.Format("Information for creature [{0}]:{1}ai = {2}{1}state = {3}{1}maxdist = {4}", .Name, vbNewLine, .aiScript.ToString, .aiScript.State.ToString, .MaxDistance))
                c.CommandResponse("Hate table:")
                For Each u As KeyValuePair(Of BaseUnit, Integer) In .aiScript.aiHateTable
                    c.CommandResponse(String.Format("{0:X} = {1} hate", u.Key.GUID, u.Value))
                Next
            End With
        End If

        Return True
    End Function



    '****************************************** CHAT COMMANDs ******************************************************
    <ChatCommandAttribute("Broadcast", "BROADCAST <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdBroadcast(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False

        Broadcast(Message)

        Return True
    End Function
    <ChatCommandAttribute("MSGGame", "MSGGAME <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster), _
     ChatCommandAttribute("GameMessage", "GAMEMESSAGE <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdGameMessage(ByRef c As CharacterObject, ByVal Text As String) As Boolean
        If Text = "" Then Return False

        Dim packet As New PacketClass(OPCODES.SMSG_AREA_TRIGGER_MESSAGE)
        packet.AddInt32(0)
        packet.AddString(Text)
        packet.AddInt8(0)

        packet.UpdateLength()
        WS.Cluster.Broadcast(packet.Data)
        packet.Dispose()
        Return True
    End Function
    <ChatCommandAttribute("MSGServer", "MSGSERVER <TYPE> <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster), _
     ChatCommandAttribute("ServerMessage", "SERVERMESSAGE <TYPE> <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdServerMessage(ByRef c As CharacterObject, ByVal Message As String) As Boolean
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
        WS.Cluster.Broadcast(packet.Data)
        packet.Dispose()

        Return True
    End Function
    <ChatCommandAttribute("MSGNotify", "MSGNOTIFY <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster), _
     ChatCommandAttribute("NotifyMessage", "NOTIFYMESSAGE <TEXT> - Send text message to all players on the server.", AccessLevel.GameMaster)> _
    Public Function cmdNotificationMessage(ByRef c As CharacterObject, ByVal Text As String) As Boolean
        If Text = "" Then Return False

        Dim packet As New PacketClass(OPCODES.SMSG_NOTIFICATION)
        packet.AddString(Text)

        packet.UpdateLength()
        WS.Cluster.Broadcast(packet.Data)
        packet.Dispose()

        Return True
    End Function
    <ChatCommandAttribute("Say", "SAY <TEXT> - Target Player / NPC will say this.", AccessLevel.GameMaster)> _
    Public Function cmdSay(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        If c.TargetGUID = 0 Then Return False

        If GuidIsPlayer(c.TargetGUID) Then
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SendChatMessage(CType(CHARACTERs(c.TargetGUID), CharacterObject), Message, ChatMsg.CHAT_MSG_SAY, LANGUAGES.LANG_UNIVERSAL, , True)
        ElseIf GuidIsCreature(c.TargetGUID) Then
            CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject).SendChatMessage(Message, ChatMsg.CHAT_MSG_MONSTER_SAY, LANGUAGES.LANG_UNIVERSAL, c.GUID)
        Else
            Return False
        End If

        Return True
    End Function


    '****************************************** DEBUG COMMANDs ******************************************************
    <ChatCommandAttribute("ResetFactions", "RESETFACTIONS - Resets reputation standings.", AccessLevel.Admin)> _
    Public Function cmdResetFactions(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            InitializeReputations(CHARACTERs(c.TargetGUID))
            CHARACTERs(c.TargetGUID).SaveCharacter()
        Else
            InitializeReputations(c)
            c.SaveCharacter()
        End If
        Return True
    End Function
    <ChatCommandAttribute("SetDurability", "SETDURABILITY <PERCENT> - Set all the target's itemdurability to a certain percentage.", AccessLevel.Admin)> _
    Public Function cmdSetDurability(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim Percent As Integer = 0
        If Integer.TryParse(Message, Percent) = True Then
            If Percent < 0 Then Percent = 0
            If Percent > 100 Then Percent = 100
            Dim sngPercent As Single = CSng(Percent / 100)

            If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
                For i As Byte = EQUIPMENT_SLOT_START To EQUIPMENT_SLOT_END - 1
                    If CHARACTERs(c.TargetGUID).Items.ContainsKey(i) Then
                        CHARACTERs(c.TargetGUID).Items(i).ModifyToDurability(sngPercent, CHARACTERs(c.TargetGUID).Client)
                    End If
                Next
            Else
                For i As Byte = EQUIPMENT_SLOT_START To EQUIPMENT_SLOT_END - 1
                    If c.Items.ContainsKey(i) Then
                        c.Items(i).ModifyToDurability(sngPercent, c.Client)
                    End If
                Next
            End If

            Return True
        End If
        Return False
    End Function
    <ChatCommandAttribute("GetMax", "GETMAX - Get all spells and skills maxed out for your level.", AccessLevel.Admin)> _
    Public Function cmdGetMax(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        'DONE: Max out all skills you know
        For Each skill As KeyValuePair(Of Integer, TSkill) In c.Skills
            skill.Value.Current = skill.Value.Maximum
            c.SetUpdateFlag(EPlayerFields.PLAYER_SKILL_INFO_1_1 + c.SkillsPositions(skill.Key) * 3 + 1, c.Skills(skill.Key).GetSkill)
        Next
        c.SendCharacterUpdate(False)

        'TODO: Add all spells

        Return True
    End Function
    <ChatCommandAttribute("SetLevel", "SETLEVEL <LEVEL> - Set the level of selected character.", AccessLevel.Admin)> _
    Public Function cmdSetLevel(ByRef c As CharacterObject, ByVal tLevel As String) As Boolean
        If IsNumeric(tLevel) = False Then Return False

        Dim Level As Integer = tLevel
        If Level > MAX_LEVEL Then Level = MAX_LEVEL
        If Level > 255 Then Level = 255

        If CHARACTERs.ContainsKey(c.TargetGUID) = False Then
            c.CommandResponse("Target not found or not character.")
            Return True
        End If

        CHARACTERs(c.TargetGUID).SetLevel(Level)

        Return True
    End Function
    <ChatCommandAttribute("AddXP", "ADDXP <XP> - Add X experience points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddXP(ByRef c As CharacterObject, ByVal tXP As String) As Boolean
        If IsNumeric(tXP) = False Then Return False

        Dim XP As Integer = tXP

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).AddXP(XP, 0, 0, True)
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("AddRestedXP", "ADDRESTEDXP <XP> - Add X rested bonus experience points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddRestedXP(ByRef c As CharacterObject, ByVal tXP As String) As Boolean
        If IsNumeric(tXP) = False Then Return False

        Dim XP As Integer = tXP

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).RestBonus += XP
            CHARACTERs(c.TargetGUID).RestState = XPSTATE.Rested

            CHARACTERs(c.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_REST_STATE_EXPERIENCE, CHARACTERs(c.TargetGUID).RestBonus)
            CHARACTERs(c.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, CHARACTERs(c.TargetGUID).cPlayerBytes2)
            CHARACTERs(c.TargetGUID).SendCharacterUpdate()
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("AddTP", "ADDTP <POINTs> - Add X talent points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddTP(ByRef c As CharacterObject, ByVal tTP As String) As Boolean
        If IsNumeric(tTP) = False Then Return False

        Dim TP As Integer = tTP

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).TalentPoints += TP
            CHARACTERs(c.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, CType(CHARACTERs(c.TargetGUID).TalentPoints, Integer))
            CHARACTERs(c.TargetGUID).SaveCharacter()
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("AddHonor", "ADDHONOR <POINTs> - Add X honor points to selected character.", AccessLevel.Admin)> _
    Public Function cmdAddHonor(ByRef c As CharacterObject, ByVal tHONOR As String) As Boolean
        If IsNumeric(tHONOR) = False Then Return False

        Dim Honor As Integer = tHONOR

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).HonorPoints += Honor
            'CHARACTERs(c.TargetGUID).SetUpdateFlag(EPlayerFields.PLAYER_FIELD_HONOR_CURRENCY, CHARACTERs(c.TargetGUID).HonorCurrency)
            CHARACTERs(c.TargetGUID).SendCharacterUpdate(False)
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("EditUnitFlag", "EDITUNITFLAG <UNITFLAG> - Change your unitflag.", AccessLevel.Developer)> _
    Public Function cmdEditUnitflag(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            c.cUnitFlags = Val("&H" & Message.Replace("0x", ""))
        Else
            c.cUnitFlags = Message
        End If
        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, c.cUnitFlags)
        c.SendCharacterUpdate()

        Return True
    End Function
    <ChatCommandAttribute("EditPlayerFlag", "EDITPLAYERFLAG <PLAYERFLAG> - Change your PLAYER_FLAGS.", AccessLevel.Developer)> _
    Public Function cmdEditPlayerflag(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            c.cPlayerFlags = Val("&H" & Message.Replace("0x", ""))
        Else
            c.cPlayerFlags = Message
        End If
        c.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, c.cPlayerFlags)
        c.SendCharacterUpdate()

        Return True
    End Function
    <ChatCommandAttribute("EditPlayerFieldBytes", "EDITPLAYERFIELDBYTES <PLAYERFIELDBYTES> - Change your PLAYER_FIELD_BYTES.", AccessLevel.Developer)> _
    Public Function cmdEditPlayerFieldBytes(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False AndAlso InStr(Message, "0x") = 0 Then Return False
        If InStr(Message, "0x") > 0 Then
            c.cPlayerFieldBytes = Val("&H" & Message.Replace("0x", ""))
        Else
            c.cPlayerFieldBytes = Message
        End If
        c.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BYTES, c.cPlayerFieldBytes)
        c.SendCharacterUpdate()

        Return True
    End Function
    <ChatCommandAttribute("GroupUpdate", "GROUPUPDATE - Get a groupupdate for selected player.", AccessLevel.Developer)> _
    Public Function cmdGroupUpdate(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).GroupUpdate(PartyMemberStatsFlag.GROUP_UPDATE_FULL)
            Return True
        End If

        Return False
    End Function

    <ChatCommandAttribute("PlaySound", "PLAYSOUND - Plays a specific sound for every player around you.", AccessLevel.Developer)> _
    Public Function cmdPlaySound(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim soundID As Integer = 0

        If Integer.TryParse(Message, soundID) = False Then Return False

        c.SendPlaySound(soundID)

        Return True
    End Function

    <ChatCommandAttribute("CombatList", "COMBATLIST - Lists everyone in your targets combatlist.", AccessLevel.Developer)> _
    Public Function cmdCombatList(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim combatList() As ULong = {}
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) Then
            combatList = CHARACTERs(c.TargetGUID).inCombatWith.ToArray()
        Else
            combatList = c.inCombatWith.ToArray()
        End If

        c.CommandResponse("Combat List (" & combatList.Length & "):")
        For Each Guid As ULong In combatList
            c.CommandResponse(String.Format("* {0:X}", Guid))
        Next

        Return True
    End Function

    <ChatCommandAttribute("CooldownList", "COOLDOWNLIST - Lists all cooldowns of your target.", AccessLevel.GameMaster)> _
    Public Function cmdCooldownList(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim targetUnit As BaseUnit = Nothing
        If GuidIsPlayer(c.TargetGUID) Then
            If CHARACTERs.ContainsKey(c.TargetGUID) Then targetUnit = CHARACTERs(c.TargetGUID)
        ElseIf GuidIsCreature(c.TargetGUID) Then
            If WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then targetUnit = WORLD_CREATUREs(c.TargetGUID)
        End If
        If targetUnit Is Nothing Then
            targetUnit = c
        End If

        If targetUnit Is c Then
            c.CommandResponse("Listing your cooldowns:")
        Else
            c.CommandResponse("Listing cooldowns for [" & targetUnit.UnitName & "]:")
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
            c.CommandResponse(sCooldowns)
        Else
            c.CommandResponse("*Cooldowns not supported for creatures yet*")
        End If

        Return True
    End Function

    <ChatCommandAttribute("ClearCooldowns", "CLEARCOOLDOWNS - Clears all cooldowns of your target.", AccessLevel.Admin)> _
    Public Function cmdClearCooldowns(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim targetUnit As BaseUnit = Nothing
        If GuidIsPlayer(c.TargetGUID) Then
            If CHARACTERs.ContainsKey(c.TargetGUID) Then targetUnit = CHARACTERs(c.TargetGUID)
        ElseIf GuidIsCreature(c.TargetGUID) Then
            If WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then targetUnit = WORLD_CREATUREs(c.TargetGUID)
        End If
        If targetUnit Is Nothing Then
            targetUnit = c
        End If

        If TypeOf targetUnit Is CharacterObject Then
            Dim timeNow As UInteger = GetTimestamp(Now)
            Dim cooldownSpells As New List(Of Integer)
            For Each Spell As KeyValuePair(Of Integer, CharacterSpell) In CType(targetUnit, CharacterObject).Spells
                If Spell.Value.Cooldown > 0UI Then
                    Spell.Value.Cooldown = 0UI
                    Spell.Value.CooldownItem = 0UI
                    CharacterDatabase.Update(String.Format("UPDATE characters_spells SET cooldown={2}, cooldownitem={3} WHERE guid = {0} AND spellid = {1};", c.GUID, Spell.Key, 0, 0))
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
            c.CommandResponse("Cooldowns are not supported for creatures yet.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("StartCheck", "STARTCHECK - Initialize Warden anti-cheat engine for selected character.", AccessLevel.Developer)> _
   Public Function cmdStartCheck(ByRef c As CharacterObject, ByVal Message As String) As Boolean
#If WARDEN Then
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            MaievInit(CHARACTERs(c.TargetGUID))
        Else
            c.CommandResponse("No player target selected.")
        End If
#Else
        c.CommandResponse("Warden is not active.")
#End If

        Return True
    End Function
    <ChatCommandAttribute("SendCheck", "SENDCHECK - Sends a Warden anti-cheat check packet to the selected character.", AccessLevel.Developer)> _
   Public Function cmdSendCheck(ByRef c As CharacterObject, ByVal Message As String) As Boolean
#If WARDEN Then
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            MaievSendCheck(CHARACTERs(c.TargetGUID))
        Else
            c.CommandResponse("No player target selected.")
        End If
#Else
        c.CommandResponse("Warden is not active.")
#End If

        Return True
    End Function

    <ChatCommandAttribute("GetSpeed", "GETSPEED - Displays all current speed.", AccessLevel.GameMaster)> _
    Public Function cmdGetSpeed(ByRef c As CharacterObject, ByVal tCopper As String) As Boolean
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).CommandResponse("WalkSpeed: " & CHARACTERs(c.TargetGUID).WalkSpeed)
            CHARACTERs(c.TargetGUID).CommandResponse("RunSpeed:" & CHARACTERs(c.TargetGUID).RunSpeed)
            CHARACTERs(c.TargetGUID).CommandResponse("RunBackSpeed:" & CHARACTERs(c.TargetGUID).RunBackSpeed)
            CHARACTERs(c.TargetGUID).CommandResponse("SwimSpeed:" & CHARACTERs(c.TargetGUID).SwimSpeed)
            CHARACTERs(c.TargetGUID).CommandResponse("SwimBackSpeed:" & CHARACTERs(c.TargetGUID).SwimBackSpeed)
            CHARACTERs(c.TargetGUID).CommandResponse("Turnrate:" & CHARACTERs(c.TargetGUID).TurnRate)
        Else
            c.CommandResponse("WalkSpeed: " & c.WalkSpeed)
            c.CommandResponse("RunSpeed:" & c.RunSpeed)
            c.CommandResponse("RunBackSpeed:" & c.RunBackSpeed)
            c.CommandResponse("SwimSpeed:" & c.SwimSpeed)
            c.CommandResponse("SwimBackSpeed:" & c.SwimBackSpeed)
            c.CommandResponse("Turnrate:" & c.TurnRate)
        End If

        Return True
    End Function
    <ChatCommandAttribute("GetAP", "GETAP - Displays attack power.", AccessLevel.GameMaster)> _
    Public Function cmdGetAttackPower(ByRef c As CharacterObject, ByVal tCopper As String) As Boolean
        If c.TargetGUID <> 0 AndAlso GuidIsPlayer(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).CommandResponse("AttackPower: " & CHARACTERs(c.TargetGUID).AttackPower)
            CHARACTERs(c.TargetGUID).CommandResponse("AttackPowerMods: " & CHARACTERs(c.TargetGUID).AttackPowerMods)
            CHARACTERs(c.TargetGUID).CommandResponse("RangedAttackPower: " & CHARACTERs(c.TargetGUID).AttackPowerRanged)
            CHARACTERs(c.TargetGUID).CommandResponse("RangedAttackPowerMods: " & CHARACTERs(c.TargetGUID).AttackPowerModsRanged)
        Else
            c.CommandResponse("AttackPower: " & c.AttackPower)
            c.CommandResponse("AttackPowerMods: " & c.AttackPowerMods)
            c.CommandResponse("RangedAttackPower: " & c.AttackPowerRanged)
            c.CommandResponse("RangedAttackPowerMods: " & c.AttackPowerModsRanged)
        End If

        Return True
    End Function
    <ChatCommandAttribute("GetDPS", "GETDPS - Tells you about damage info.", AccessLevel.Developer)> _
    Public Function cmdGetDPS(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.CommandResponse("Ammo ID: " & c.AmmoID)
        c.CommandResponse("Ammo DPS: " & c.AmmoDPS)
        c.CommandResponse("Ammo Mod: " & c.AmmoMod)
        CalculateMinMaxDamage(c, WeaponAttackType.RANGED_ATTACK)

        Return True
    End Function

    <ChatCommandAttribute("AddItem", "ADDITEM <ID> <optional COUNT> - Add Y items with id X to selected character.")> _
    Public Function cmdAddItem(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        If tmp.Length < 1 Then Return False

        Dim id As Integer = tmp(0)
        Dim Count As Integer = 1
        If tmp.Length = 2 Then Count = tmp(1)
        If GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
            Dim newItem As New ItemObject(id, c.TargetGUID)
            newItem.StackCount = Count
            If CHARACTERs(c.TargetGUID).ItemADD(newItem) Then
                CHARACTERs(c.TargetGUID).LogLootItem(newItem, Count, True, False)
            Else
                newItem.Delete()
            End If
        Else
            Dim newItem As New ItemObject(id, c.GUID)
            newItem.StackCount = Count
            If c.ItemADD(newItem) Then
                c.LogLootItem(newItem, Count, False, True)
            Else
                newItem.Delete()
            End If
        End If

        Return True
    End Function
    <ChatCommandAttribute("AddItemSet", "ADDITEMSET <ID> - Add the items in the item set with id X to selected character.")> _
    Public Function cmdAddItemSet(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        If tmp.Length < 1 Then Return False

        Dim id As Integer = tmp(0)

        If ItemSet.ContainsKey(id) Then
            If GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
                For Each item As Integer In ItemSet(id).ItemID
                    Dim newItem As New ItemObject(item, c.TargetGUID)
                    newItem.StackCount = 1
                    If CHARACTERs(c.TargetGUID).ItemADD(newItem) Then
                        CHARACTERs(c.TargetGUID).LogLootItem(newItem, 1, False, True)
                    Else
                        newItem.Delete()
                    End If
                Next
            Else
                For Each item As Integer In ItemSet(id).ItemID
                    Dim newItem As New ItemObject(item, c.GUID)
                    newItem.StackCount = 1
                    If c.ItemADD(newItem) Then
                        c.LogLootItem(newItem, 1, False, True)
                    Else
                        newItem.Delete()
                    End If
                Next
            End If
        End If

        Return True
    End Function
    <ChatCommandAttribute("AddMoney", "ADDMONEY <XP> - Add X copper yours.")> _
    Public Function cmdAddMoney(ByRef c As CharacterObject, ByVal tCopper As String) As Boolean
        If tCopper = "" Then Return False

        Dim Copper As ULong = tCopper

        If CType(c.Copper, ULong) + Copper > UInteger.MaxValue Then
            c.Copper = UInteger.MaxValue
        Else
            c.Copper += Copper
        End If

        c.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, c.Copper)
        c.SendCharacterUpdate(False)

        Return True
    End Function
    <ChatCommandAttribute("LearnSkill", "LearnSkill <ID> <CURRENT> <MAX> - Add skill id X with value Y of Z to selected character.")> _
    Public Function cmdLearnSkill(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False


        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            Dim tmp() As String
            tmp = Split(Trim(Message), " ")

            Dim SkillID As Integer = tmp(0)
            Dim Current As Int16 = tmp(1)
            Dim Maximum As Int16 = tmp(2)

            If CHARACTERs(c.TargetGUID).Skills.ContainsKey(SkillID) Then
                CType(CHARACTERs(c.TargetGUID).Skills(SkillID), TSkill).Base = Maximum
                CType(CHARACTERs(c.TargetGUID).Skills(SkillID), TSkill).Current = Current
            Else
                CHARACTERs(c.TargetGUID).LearnSkill(SkillID, Current, Maximum)
            End If

            CHARACTERs(c.TargetGUID).FillAllUpdateFlags()
            CHARACTERs(c.TargetGUID).SendUpdate()
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("LearnSpell", "LearnSpell <ID> - Add spell X to selected character.")> _
    Public Function cmdLearnSpell(ByRef c As CharacterObject, ByVal tID As String) As Boolean
        If tID = "" Then Return False

        Dim ID As Integer
        If Integer.TryParse(tID, ID) = False OrElse ID < 0 Then Return False
        If SPELLs.ContainsKey(ID) = False Then
            c.CommandResponse("You tried learning a spell that did not exist.")
            Exit Function
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).LearnSpell(ID)
            If c.TargetGUID = c.GUID Then
                c.CommandResponse("You learned spell: " & ID)
            Else
                c.CommandResponse(CHARACTERs(c.TargetGUID).Name & " has learned spell: " & ID)
            End If
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("UnlearnSpell", "UnlearnSpell <ID> - Remove spell X from selected character.")> _
    Public Function cmdUnlearnSpell(ByRef c As CharacterObject, ByVal tID As String) As Boolean
        If tID = "" Then Return False

        Dim ID As Integer = tID

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).UnLearnSpell(ID)
            If c.TargetGUID = c.GUID Then
                c.CommandResponse("You unlearned spell: " & ID)
            Else
                c.CommandResponse(CHARACTERs(c.TargetGUID).Name & " has unlearned spell: " & ID)
            End If
        Else
            c.CommandResponse("Target not found or not character.")
        End If

        Return True
    End Function

    <ChatCommandAttribute("ShowTaxi", "SHOWTAXI - Unlock all taxi locations.")> _
    Public Function cmdShowTaxi(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.TaxiZones.SetAll(True)
        Return True
    End Function
    <ChatCommandAttribute("SET", "SET <INDEX> <VALUE> - Set update value (A9).")> _
    Public Function cmdSetUpdateField(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim tmp() As String = Split(Message, " ", 2)

        SetUpdateValue(c.TargetGUID, tmp(0), tmp(1), c.Client)
        Return True
    End Function
    <ChatCommandAttribute("SetRunSpeed", "SETRUNSPEED <VALUE> - Change your run speed.")> _
    Public Function cmdSetRunSpeed(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        c.ChangeSpeedForced(WS_CharManagment.CharacterObject.ChangeSpeedType.RUN, Message)
        c.CommandResponse("Your RunSpeed is changed to " & Message)
        Return True
    End Function
    <ChatCommandAttribute("SetSwimSpeed", "SETSWIMSPEED <VALUE> - Change your swim speed.")> _
    Public Function cmdSetSwimSpeed(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        c.ChangeSpeedForced(WS_CharManagment.CharacterObject.ChangeSpeedType.SWIM, Message)
        c.CommandResponse("Your SwimSpeed is changed to " & Message)
        Return True
    End Function
    <ChatCommandAttribute("SetRunBackSpeed", "SETRUNBACKSPEED <VALUE> - Change your run back speed.")> _
    Public Function cmdSetRunBackSpeed(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        c.ChangeSpeedForced(WS_CharManagment.CharacterObject.ChangeSpeedType.SWIMBACK, Message)
        c.CommandResponse("Your RunBackSpeed is changed to " & Message)
        Return True
    End Function
    <ChatCommandAttribute("SetReputation", "SETREPUTATION <FACTION> <VALUE> - Change your reputation standings.", AccessLevel.GameMaster)> _
    Public Function cmdSetReputation(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim tmp() As String = Split(Message, " ", 2)
        c.SetReputation(tmp(0), tmp(1))
        c.CommandResponse("You have set your reputation with [" & tmp(0) & "] to [" & tmp(1) & "]")
        Return True
    End Function

    <ChatCommandAttribute("Model", "MODEL <ID> - Will morph you into specified model ID.", AccessLevel.GameMaster)> _
    Public Function cmdModel(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim value As Integer = 0
        If Integer.TryParse(Message, value) = False OrElse value < 0 Then Return False

        If CreatureModel.ContainsKey(value) Then
            c.BoundingRadius = CreatureModel(value).BoundingRadius
            c.CombatReach = CreatureModel(value).CombatReach
        End If

        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_BOUNDINGRADIUS, c.BoundingRadius)
        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_COMBATREACH, c.CombatReach)
        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_DISPLAYID, value)
        c.SendCharacterUpdate()
        Return True
    End Function

    <ChatCommandAttribute("Mount", "MOUNT <ID> - Will mount you to specified model ID.", AccessLevel.GameMaster)> _
    Public Function cmdMount(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim value As Integer = 0
        If Integer.TryParse(Message, value) = False OrElse value < 0 Then Return False

        c.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, value)
        c.SendCharacterUpdate()
        Return True
    End Function

    <ChatCommandAttribute("Hurt", "HURT - Hurt selected character.", AccessLevel.GameMaster)> _
    Public Function cmdHurt(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CType(CHARACTERs(c.TargetGUID), CharacterObject).Life.Current -= CType(CHARACTERs(c.TargetGUID), CharacterObject).Life.Maximum * 0.1
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SetUpdateFlag(EUnitFields.UNIT_FIELD_HEALTH, CType(CHARACTERs(c.TargetGUID), CharacterObject).Life.Current)
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SendCharacterUpdate()
            Return True
        End If

        Return True
    End Function
    <ChatCommandAttribute("Vulnerable", "VULNERABLE - Changes the selected characters vulnerability (ON/OFF).", AccessLevel.GameMaster)> _
    Public Function cmdVulnerable(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Message = Message.ToUpper()
        Dim Enabled As Boolean = False
        If Message = "ON" OrElse Message = "1" Then
            Enabled = False
        ElseIf Message = "OFF" OrElse Message = "0" Then
            Enabled = True
        Else
            Return False
        End If

        If c.TargetGUID = 0 OrElse GuidIsPlayer(c.TargetGUID) = False OrElse CHARACTERs.ContainsKey(c.TargetGUID) = False Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        CType(CHARACTERs(c.TargetGUID), CharacterObject).Invulnerable = Enabled

        Return True
    End Function
    <ChatCommandAttribute("Root", "ROOT - Instantly root selected character.", AccessLevel.GameMaster)> _
    Public Function cmdRoot(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SetMoveRoot()
            Return True
        End If

        Return True
    End Function
    <ChatCommandAttribute("UnRoot", "UNROOT - Instantly unroot selected character.", AccessLevel.GameMaster)> _
    Public Function cmdUnRoot(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SetMoveUnroot()
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("Revive", "REVIVE - Instantly revive selected character.", AccessLevel.GameMaster)> _
    Public Function cmdRevive(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CharacterResurrect(CType(CHARACTERs(c.TargetGUID), CharacterObject))
            Return True
        End If

        Return True
    End Function

    <ChatCommandAttribute("GoToGraveyard", "GOTOGRAVEYARD - Instantly teleports selected character to nearest graveyard.", AccessLevel.GameMaster)> _
    Public Function cmdGoToGraveyard(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            GoToNearestGraveyard(CType(CHARACTERs(c.TargetGUID), CharacterObject))
            Return True
        End If

        Return True
    End Function
    <ChatCommandAttribute("GoToStart", "GOTOSTART <RACE> - Instantly teleports selected character to specified race start location.", AccessLevel.GameMaster)> _
    Public Function cmdGoToStart(ByRef c As CharacterObject, ByVal StringRace As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            Dim Info As New DataTable
            Dim Character As CharacterObject = CHARACTERs(c.TargetGUID)
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
                    c.CommandResponse("Unknown race. Use DR,BE,DW,GN,HU,NE,OR,TA,TR,UN for race.")
                    Return True
            End Select

            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo WHERE race = {0};", CType(Race, Integer)), Info)
            Character.Teleport(Info.Rows(0).Item("positionX"), Info.Rows(0).Item("positionY"), Info.Rows(0).Item("positionZ"), 0, Info.Rows(0).Item("mapID"))
            Return True
        End If

        Return True
    End Function
    <ChatCommandAttribute("Summon", "SUMMON <NAME> - Instantly teleports the player to you.", AccessLevel.GameMaster)> _
    Public Function cmdSummon(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        Dim GUID As ULong = GetGUID(CapitalizeName(Name))
        If CHARACTERs.ContainsKey(GUID) Then
            If c.OnTransport IsNot Nothing Then
                CType(CHARACTERs(GUID), CharacterObject).OnTransport = c.OnTransport
                CType(CHARACTERs(GUID), CharacterObject).Transfer(c.positionX, c.positionY, c.positionZ, c.orientation, c.MapID)
            Else
                CType(CHARACTERs(GUID), CharacterObject).Teleport(c.positionX, c.positionY, c.positionZ, c.orientation, c.MapID)
            End If
            Return True
        Else
            c.CommandResponse("Player not found.")
            Return True
        End If
    End Function
    <ChatCommandAttribute("Appear", "APPEAR <NAME> - Instantly teleports you to the player.", AccessLevel.GameMaster)> _
    Public Function cmdAppear(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        Dim GUID As ULong = GetGUID(CapitalizeName(Name))
        If CHARACTERs.ContainsKey(GUID) Then
            With CType(CHARACTERs(GUID), CharacterObject)
                If .OnTransport IsNot Nothing Then
                    c.OnTransport = .OnTransport
                    c.Transfer(.positionX, .positionY, .positionZ, .orientation, .MapID)
                Else
                    c.Teleport(.positionX, .positionY, .positionZ, .orientation, .MapID)
                End If
            End With
            Return True
        Else
            c.CommandResponse("Player not found.")
            Return True
        End If
    End Function

    <ChatCommandAttribute("VmapTest", "VMAPTEST - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest(ByRef c As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        If Config.VMapsEnabled Then
            Dim target As BaseUnit = Nothing
            If c.TargetGUID > 0 Then
                If GuidIsPlayer(c.TargetGUID) AndAlso CHARACTERs.ContainsKey(c.TargetGUID) Then
                    target = CHARACTERs(c.TargetGUID)
                ElseIf GuidIsCreature(c.TargetGUID) AndAlso WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
                    target = WORLD_CREATUREs(c.TargetGUID)
                    WORLD_CREATUREs(c.TargetGUID).SetToRealPosition()
                End If
            End If

            Dim timeStart As Integer = timeGetTime

            Dim height As Single = GetVMapHeight(c.MapID, c.positionX, c.positionY, c.positionZ + 2.0F)

            Dim isInLOS As Boolean = False
            If target IsNot Nothing Then
                isInLOS = IsInLineOfSight(c, target)
            End If

            Dim timeTaken As Integer = timeGetTime - timeStart

            If height = VMAP_INVALID_HEIGHT_VALUE Then
                c.CommandResponse(String.Format("Unable to retrieve VMap height for your location."))
            Else
                c.CommandResponse(String.Format("Your Z: {0}  VMap Z: {1}", c.positionZ, height))
            End If

            If target IsNot Nothing Then
                c.CommandResponse(String.Format("Target in line of sight: {0}", isInLOS))
            End If

            c.CommandResponse(String.Format("Vmap functionality ran under [{0} ms].", timeTaken))
        Else
            c.CommandResponse("Vmaps is not enabled.")
        End If
#Else
        c.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function
    <ChatCommandAttribute("VmapTest2", "VMAPTEST2 - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest2(ByRef c As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        If Config.VMapsEnabled Then
            If c.TargetGUID = 0UL OrElse GuidIsCreature(c.TargetGUID) = False OrElse WORLD_CREATUREs.ContainsKey(c.TargetGUID) = False Then
                c.CommandResponse("You must target a creature first.")
            Else
                WORLD_CREATUREs(c.TargetGUID).SetToRealPosition()

                Dim resX As Single = 0.0F
                Dim resY As Single = 0.0F
                Dim resZ As Single = 0.0F
                Dim result As Boolean = GetObjectHitPos(c, WORLD_CREATUREs(c.TargetGUID), resX, resY, resZ, -1.0F)

                If result = False Then
                    c.CommandResponse("You teleported without any problems.")
                Else
                    c.CommandResponse("You teleported by hitting something.")
                End If

                c.orientation = GetOrientation(c.positionX, WORLD_CREATUREs(c.TargetGUID).positionX, c.positionY, WORLD_CREATUREs(c.TargetGUID).positionY)
                resZ = GetVMapHeight(c.MapID, resX, resY, resZ + 2.0F)
                c.Teleport(resX, resY, resZ, c.orientation, c.MapID)
            End If
        Else
            c.CommandResponse("Vmaps is not enabled.")
        End If
#Else
        c.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function
    <ChatCommandAttribute("VmapTest3", "VMAPTEST3 - Tests VMAP functionality.", AccessLevel.Developer)> _
    Public Function cmdVmapTest3(ByRef c As CharacterObject, ByVal Message As String) As Boolean
#If VMAPS Then
        Dim CellMap As UInteger = c.MapID
        Dim CellX As Byte = GetMapTileX(c.positionX)
        Dim CellY As Byte = GetMapTileY(c.positionY)

        Dim fileName As String = String.Format("{0}_{1}_{2}.vmdir", Format(CellMap, "000"), Format(CellX, "00"), Format(CellY, "00"))
        If Not System.IO.File.Exists("vmaps\" & fileName) Then
            c.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
            fileName = String.Format("{0}.vmdir", Format(CellMap, "000"))
        End If

        If Not System.IO.File.Exists("vmaps\" & fileName) Then
            c.CommandResponse(String.Format("VMap file [{0}] not found", fileName))
        Else
            c.CommandResponse(String.Format("VMap file [{0}] found!", fileName))
            Dim map As TMap = Maps(CellMap)
            fileName = Trim(System.IO.File.ReadAllText("vmaps\" & fileName))

            c.CommandResponse(String.Format("Full file: '{0}'", fileName))
            If fileName.Contains(vbLf) Then
                fileName = fileName.Substring(0, fileName.IndexOf(vbLf))
            End If

            c.CommandResponse(String.Format("First line: '{0}'", fileName))
            Dim newModelLoaded As Boolean = False
            If fileName.Length > 0 AndAlso System.IO.File.Exists("vmaps\" & fileName) Then
                c.CommandResponse(String.Format("VMap file [{0}] found!", fileName))

                If Maps(CellMap).ContainsModelContainer(fileName) Then
                    c.CommandResponse(String.Format("VMap ModelContainer is loaded!"))
                Else
                    c.CommandResponse(String.Format("VMap ModelContainer is NOT loaded!"))
                End If
            Else
                c.CommandResponse(String.Format("VMap file [{0}] not found!", fileName))
            End If
        End If
#Else
        c.CommandResponse("Vmaps is not enabled.")
#End If
        Return True
    End Function
    <ChatCommandAttribute("LineOfSight", "LINEOFSIGHT <ON/OFF> - Enables/Disables line of sight calculation.", AccessLevel.Developer)> _
    Public Function cmdLineOfSight(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message.ToUpper = "ON" Then
            Config.LineOfSightEnabled = True
            c.CommandResponse("Line of Sight Calculation is now Enabled.")
        ElseIf Message.ToUpper = "OFF" Then
            Config.LineOfSightEnabled = False
            c.CommandResponse("Line of Sight Calculation is now Disabled.")
        Else
            Return False
        End If
        Return True
    End Function

    <ChatCommandAttribute("GPS", "GPS - Tells you where you are located.", AccessLevel.GameMaster)> _
    Public Function cmdGPS(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.CommandResponse("X: " & c.positionX)
        c.CommandResponse("Y: " & c.positionY)
        c.CommandResponse("Z: " & c.positionZ)
        c.CommandResponse("Orientation: " & c.orientation)
        c.CommandResponse("Map: " & c.MapID)
        Return True
    End Function
    <ChatCommandAttribute("SetInstance", "SETINSTANCE <ID> - Sets you into another instance.", AccessLevel.GameMaster)> _
    Public Function cmdSetInstance(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim instanceID As Integer = 0
        If Integer.TryParse(Message, instanceID) = False Then Return False
        If instanceID < 0 OrElse instanceID > 400000 Then Return False

        c.instance = instanceID
        Return True
    End Function
    <ChatCommandAttribute("Port", "PORT <X> <Y> <Z> <ORIENTATION> <MAP> - Teleports Character To Given Coordinates.", AccessLevel.GameMaster)> _
    Public Function cmdPort(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False

        Dim tmp() As String
        tmp = Message.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

        If tmp.Length <> 5 Then Return False

        Dim posX As Single = CSng(tmp(0))
        Dim posY As Single = CSng(tmp(1))
        Dim posZ As Single = CSng(tmp(2))
        Dim posO As Single = CSng(tmp(3))
        Dim posMap As Integer = CSng(tmp(4))

        c.Teleport(posX, posY, posZ, posO, posMap)
        Return True
    End Function
    <ChatCommandAttribute("PortByName", "PORT <LocationName> - Teleports Character To The LocationName Location. Use PortByName list to get a list of locations.", AccessLevel.GameMaster)> _
    Public Function cmdPortByName(ByRef c As CharacterObject, ByVal location As String) As Boolean

        If location = "" Then Return False

        Dim posX As Single = 0
        Dim posY As Single = 0
        Dim posZ As Single = 0
        Dim posO As Single = 0
        Dim posMap As Integer = 0

        If UCase(location) = "LIST" Then
            Dim cmdList As String = "Listing of available locations:" & vbNewLine

            Dim ListSQLQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM world_cmdteleports", ListSQLQuery)

            For Each LocationRow As DataRow In ListSQLQuery.Rows
                cmdList += LocationRow.Item("name") & ", "
            Next
            c.CommandResponse(cmdList)
            Return True
        End If

        location = location.Replace("'", "").Replace(" ", "")
        Dim MySQLQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM world_cmdteleports WHERE name = '{0}' LIMIT 1;", location), MySQLQuery)

        If MySQLQuery.Rows.Count > 0 Then
            posX = CType(MySQLQuery.Rows(0).Item("positionX"), Single)
            posY = CType(MySQLQuery.Rows(0).Item("positionY"), Single)
            posZ = CType(MySQLQuery.Rows(0).Item("positionZ"), Single)
            posMap = CType(MySQLQuery.Rows(0).Item("MapId"), Integer)
            c.Teleport(posX, posY, posZ, posO, posMap)
        Else
            c.CommandResponse(String.Format("Location {0} NOT found in Database", location))
        End If


        Return True
    End Function


    '****************************************** ACCOUNT MANAGMENT COMMANDs ******************************************
    <ChatCommandAttribute("Slap", "SLAP <DAMAGE> - Slap target creature or player for X damage.")> _
    Public Function cmdSlap(ByRef c As CharacterObject, ByVal tDamage As String) As Boolean
        Dim Damage As Integer = tDamage

        If GuidIsCreature(c.TargetGUID) Then
            CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject).DealDamage(Damage)
        ElseIf GuidIsPlayer(c.TargetGUID) Then
            CType(CHARACTERs(c.TargetGUID), CharacterObject).DealDamage(Damage)
            CType(CHARACTERs(c.TargetGUID), CharacterObject).SystemMessage(c.Name & " slaps you for " & Damage & " damage.")
        Else
            c.CommandResponse("Not supported target selected.")
        End If

        Return True
    End Function
    <ChatCommandAttribute("Kick", "KICK <optional NAME> - Kick selected player or character with name specified if found.")> _
    Public Function cmdKick(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            'DONE: Kick by selection
            If c.TargetGUID = 0 Then
                c.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(c.TargetGUID) Then
                'DONE: Kick gracefully
                c.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(CHARACTERs(c.TargetGUID), CharacterObject).Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Client.Character.Name, CHARACTERs(c.TargetGUID).Name)
                CHARACTERs(c.TargetGUID).Logout()
            Else
                c.CommandResponse(String.Format("Character GUID=[{0}] not found.", c.TargetGUID))
            End If

        Else

            'DONE: Kick by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    'DONE: Kick gracefully
                    Character.Value.Logout()
                    c.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(Character.Value, CharacterObject).Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Client.Character.Name, Name)
                    Return True
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            c.CommandResponse(String.Format("Character [{0:X}] not found.", Name))

        End If
        Return True
    End Function
    <ChatCommandAttribute("KickReason", "KICKREASON <TEXT> - Display message for 2 seconds and kick selected player.")> _
    Public Function cmdKickReason(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("No target selected.")
        Else
            SystemMessage(String.Format("Character [{0}] kicked form server.{3}Reason: {1}{3}GameMaster: [{2}].", SetColor(CType(CHARACTERs(c.TargetGUID), CharacterObject).Name, 255, 0, 0), SetColor(Message, 255, 0, 0), SetColor(c.Name, 255, 0, 0), vbNewLine))
            Thread.Sleep(2000)

            cmdKick(c, "")
        End If

        Return True
    End Function
    <ChatCommandAttribute("Disconnect", "DISCONNECT <optional NAME> - Disconnects selected player or character with name specified if found.")> _
    Public Function cmdDisconnect(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            'DONE: Kick by selection
            If c.TargetGUID = 0 Then
                c.CommandResponse("No target selected.")
            ElseIf CHARACTERs.ContainsKey(c.TargetGUID) Then
                c.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(CHARACTERs(c.TargetGUID), CharacterObject).Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Client.Character.Name, CHARACTERs(c.TargetGUID).Name)
                CType(CHARACTERs(c.TargetGUID), CharacterObject).Client.Disconnect()
            Else
                c.CommandResponse(String.Format("Character GUID=[{0}] not found.", c.TargetGUID))
            End If

        Else

            'DONE: Kick by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    c.CommandResponse(String.Format("Character [{0}] kicked form server.", CType(Character.Value, CharacterObject).Name))
                    Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Character [{3}] kicked by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Client.Character.Name, Name)
                    CType(Character.Value, CharacterObject).Client.Disconnect()
                    Return True
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            c.CommandResponse(String.Format("Character [{0:X}] not found.", Name))

        End If
        Return True
    End Function

    <ChatCommandAttribute("ForceRename", "FORCERENAME - Force selected player to change his name next time on char enum.")> _
    Public Function cmdForceRename(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("No target selected.")
        ElseIf CHARACTERs.ContainsKey(c.TargetGUID) Then
            CharacterDatabase.Update(String.Format("UPDATE characters SET force_restrictions = 1 WHERE char_guid = {0};", c.TargetGUID))
            c.CommandResponse("Player will be asked to change his name on next logon.")
        Else
            c.CommandResponse(String.Format("Character GUID=[{0:X}] not found.", c.TargetGUID))
        End If

        Return True
    End Function
    <ChatCommandAttribute("BanChar", "BANCHAR - Selected player won't be able to login next time with this character.")> _
    Public Function cmdBanChar(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("No target selected.")
        ElseIf CHARACTERs.ContainsKey(c.TargetGUID) Then
            CharacterDatabase.Update(String.Format("UPDATE characters SET force_restrictions = 2 WHERE char_guid = {0};", c.TargetGUID))
            c.CommandResponse("Character disabled.")
        Else
            c.CommandResponse(String.Format("Character GUID=[{0:X}] not found.", c.TargetGUID))
        End If

        Return True
    End Function

    <ChatCommandAttribute("Ban", "BAN <ACCOUNT> - Ban specified account from server.")> _
    Public Function cmdBan(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then Return False

        Dim result As New DataTable
        AccountDatabase.Query("SELECT banned FROM accounts WHERE account = """ & Name & """;", result)
        If result.Rows.Count > 0 Then
            If result.Rows(0).Item("banned") = 1 Then
                c.CommandResponse(String.Format("Account [{0}] already banned.", Name))
            Else
                AccountDatabase.Update("UPDATE accounts SET banned = 1 WHERE account = """ & Name & """;")
                c.CommandResponse(String.Format("Account [{0}] banned.", Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] banned by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Name, Name)
            End If
        Else
            c.CommandResponse(String.Format("Account [{0}] not found.", Name))
        End If

        Return True
    End Function
    <ChatCommandAttribute("UnBan", "UNBAN <ACCOUNT> - Remove ban of specified account from server.")> _
    Public Function cmdUnBan(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then Return False

        Dim result As New DataTable
        AccountDatabase.Query("SELECT banned FROM accounts WHERE account = """ & Name & """;", result)
        If result.Rows.Count > 0 Then
            If result.Rows(0).Item("banned") = 0 Then
                c.CommandResponse(String.Format("Account [{0}] is not banned.", Name))
            Else
                AccountDatabase.Update("UPDATE accounts SET banned = 0 WHERE account = """ & Name & """;")
                c.CommandResponse(String.Format("Account [{0}] unbanned.", Name))
                Log.WriteLine(LogType.INFORMATION, "[{0}:{1}] Account [{3}] unbanned by [{2}].", c.Client.IP.ToString, c.Client.Port, c.Name, Name)
            End If
        Else
            c.CommandResponse(String.Format("Account [{0}] not found.", Name))
        End If

        Return True
    End Function

    <ChatCommandAttribute("Info", "INFO <optional NAME> - Show account information for selected target or character with name specified if found.")> _
    Public Function cmdInfo(ByRef c As CharacterObject, ByVal Name As String) As Boolean
        If Name = "" Then

            Dim GUID As ULong = c.TargetGUID

            'DONE: Info by selection
            If CHARACTERs.ContainsKey(GUID) Then
                c.CommandResponse(String.Format("Information for character [{0}]:{1}account = {2}{1}ip = {3}{1}guid = {4:X}{1}access = {5}{1}boundingradius = {6}{1}combatreach = {7}", _
                CHARACTERs(GUID).Name, vbNewLine, _
                CHARACTERs(GUID).Client.Account, _
                CHARACTERs(GUID).Client.IP.ToString, _
                CHARACTERs(GUID).GUID, _
                CHARACTERs(GUID).Access, _
                CHARACTERs(GUID).BoundingRadius, _
                CHARACTERs(GUID).CombatReach))
            ElseIf WORLD_CREATUREs.ContainsKey(GUID) Then
                c.CommandResponse(String.Format("Information for creature [{0}]:{1}id = {2}{1}guid = {3:X}{1}model = {4}{1}boundingradius = {11}{1}combatreach = {12}{1}ai = {5}{1}his reaction = {6}{1}guard = {7}{1}waypoint = {10}{1}damage = {8}-{9}", _
                WORLD_CREATUREs(GUID).Name, vbNewLine, _
                WORLD_CREATUREs(GUID).ID, _
                GUID, _
                WORLD_CREATUREs(GUID).Model, _
                WORLD_CREATUREs(GUID).aiScript.GetType().ToString, _
                c.GetReaction(WORLD_CREATUREs(GUID).Faction), _
                WORLD_CREATUREs(GUID).isGuard, _
                WORLD_CREATUREs(GUID).CreatureInfo.Damage.Minimum, WORLD_CREATUREs(GUID).CreatureInfo.Damage.Maximum, _
                (WORLD_CREATUREs(GUID).MoveType = 2), _
                WORLD_CREATUREs(GUID).BoundingRadius, _
                WORLD_CREATUREs(GUID).CombatReach))
            ElseIf WORLD_GAMEOBJECTs.ContainsKey(GUID) Then
                c.CommandResponse(String.Format("Information for gameobject [{0}]:{1}id = {2}{1}guid = {3:X}{1}model = {4}", _
                WORLD_GAMEOBJECTs(GUID).Name, vbNewLine, _
                WORLD_GAMEOBJECTs(GUID).ID, _
                GUID, _
                GAMEOBJECTSDatabase(WORLD_GAMEOBJECTs(GUID).ID).Model))
            Else
                c.CommandResponse(String.Format("Information about yourself.{0}guid = {1:X}{0}model = {2}{0}mount = {3}", _
                vbNewLine, c.GUID, c.Model, c.Mount))
            End If

        Else

            'DONE: Info by name
            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UCase(CType(Character.Value, CharacterObject).Name) = Name Then
                    CHARACTERs_Lock.ReleaseReaderLock()
                    c.CommandResponse(String.Format("Information for character [{0}]:{1}account = {2}{1}ip = {3}{1}guid = {4}{1}access = {5}", _
                    CType(Character.Value, CharacterObject).Name, vbNewLine, _
                    CType(Character.Value, CharacterObject).Client.Account, _
                    CType(Character.Value, CharacterObject).Client.IP.ToString, _
                    CType(Character.Value, CharacterObject).GUID, _
                    CType(Character.Value, CharacterObject).Access))
                    Exit Function
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()
            c.CommandResponse(String.Format("Character [{0}] not found.", Name))

        End If

        Return True
    End Function
    <ChatCommandAttribute("Where", "WHERE - Display your position information.")> _
    Public Function cmdWhere(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        c.SystemMessage(String.Format("Coords: x={0}, y={1}, z={2}, or={3}, map={4}", c.positionX, c.positionY, c.positionZ, c.orientation, c.MapID))
        c.SystemMessage(String.Format("Cell: {0},{1} SubCell: {2},{3}", GetMapTileX(c.positionX), GetMapTileY(c.positionY), GetSubMapTileX(c.positionX), GetSubMapTileY(c.positionY)))
        c.SystemMessage(String.Format("ZCoords: {0} AreaFlag: {1} WaterLevel={2}", GetZCoord(c.positionX, c.positionY, c.positionZ, c.MapID), GetAreaFlag(c.positionX, c.positionY, c.MapID), GetWaterLevel(c.positionX, c.positionY, c.MapID)))
        c.ZoneCheck()
        c.SystemMessage(String.Format("ZoneID: {0}", c.ZoneID))
#If ENABLE_PPOINTS Then
        c.SystemMessage(String.Format("ZCoords_PP: {0}", GetZCoord_PP(c.positionX, c.positionY, c.MapID)))
#End If

        Return True
    End Function


    '****************************************** MISC COMMANDs *******************************************************
    <ChatCommandAttribute("SetGM", "SETGM <FLAG> <INVISIBILITY> - Toggles gameMaster status. You can use values like On/Off/1/0.")> _
    Public Function cmdSetGM(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        Dim value1 As String = tmp(0)
        Dim value2 As String = tmp(1)

        'setFaction(35);
        'SetFlag(PLAYER_BYTES_2, 0x8);

        'Commnad: .setgm <gmflag:0/1/off/on> <invisibility:0/1/off/on>
        If value1 = "0" Or UCase(value1) = "OFF" Then
            c.GM = False
            c.CommandResponse("GameMaster Flag turned off.")
        Else
            c.GM = True
            c.CommandResponse("GameMaster Flag turned on.")
        End If
        If value2 = "0" Or UCase(value2) = "OFF" Then
            c.Invisibility = InvisibilityLevel.VISIBLE
            c.CanSeeInvisibility = InvisibilityLevel.VISIBLE
            c.CommandResponse("GameMaster Invisibility turned off.")
        Else
            c.Invisibility = InvisibilityLevel.GM
            c.CanSeeInvisibility = InvisibilityLevel.GM
            c.CommandResponse("GameMaster Invisibility turned on.")
        End If
        c.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, c.cPlayerFlags)
        c.SendCharacterUpdate()
        UpdateCell(c)

        Return True
    End Function
    <ChatCommandAttribute("SetWeather", "SETWEATHER <TYPE> <INTENSITY> - Change weather in current zone. Intensity is float value!")> _
    Public Function cmdSetWeather(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim tmp() As String = Split(Message, " ", 2)
        Dim Type As Integer = tmp(0)
        Dim Intensity As Single = tmp(1)

        If WeatherZones.ContainsKey(c.ZoneID) = False Then
            c.CommandResponse("No weather for this zone is found!")
        Else
            WeatherZones(c.ZoneID).CurrentWeather = Type
            WeatherZones(c.ZoneID).Intensity = Intensity
            SendWeather(c.ZoneID, c.Client)
        End If

        Return True
    End Function


    '****************************************** SPAWNING COMMANDs ***************************************************
    <ChatCommandAttribute("Del", "DEL <ID> - Delete selected creature or gameobject."), _
     ChatCommandAttribute("Delete", "DELETE <ID> - Delete selected creature or gameobject."), _
     ChatCommandAttribute("Remove", "REMOVE <ID> - Delete selected creature or gameobject.")> _
    Public Function cmdDeleteObject(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If GuidIsCreature(c.TargetGUID) Then
            'DONE: Delete creature
            If Not WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
                c.CommandResponse("Selected target is not creature!")
                Return True
            End If

            WORLD_CREATUREs(c.TargetGUID).Destroy()
            c.CommandResponse("Creature deleted.")

        ElseIf GuidIsGameObject(c.TargetGUID) Then
            'DONE: Delete GO
            If Not WORLD_GAMEOBJECTs.ContainsKey(c.TargetGUID) Then
                c.CommandResponse("Selected target is not game object!")
                Return True
            End If

            WORLD_GAMEOBJECTs(c.TargetGUID).Destroy()
            c.CommandResponse("Game object deleted.")

        End If




        Return True
    End Function
    <ChatCommandAttribute("Turn", "TURN - Selected creature or game object will turn to your position.")> _
    Public Function cmdTurnObject(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If GuidIsCreature(c.TargetGUID) Then
            'DONE: Turn creature
            If Not WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
                c.CommandResponse("Selected target is not creature!")
                Return True
            End If

            CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject).TurnTo(c.positionX, c.positionY)

        ElseIf GuidIsGameObject(c.TargetGUID) Then
            'DONE: Turn GO
            If Not WORLD_GAMEOBJECTs.ContainsKey(c.TargetGUID) Then
                c.CommandResponse("Selected target is not game object!")
                Return True
            End If

            CType(WORLD_GAMEOBJECTs(c.TargetGUID), GameObjectObject).TurnTo(c.positionX, c.positionY)

            Dim q As New DataTable
            Dim GUID As ULong = c.TargetGUID - GUID_GAMEOBJECT

            c.CommandResponse("Object rotation will be visible when the object is reloaded!")

        End If

        Return True
    End Function

    <ChatCommandAttribute("AddNPC", "ADDNPC <ID> - Spawn creature at your position."), _
     ChatCommandAttribute("AddCreature", "ADDCREATURE <ID> - Spawn creature at your position.")> _
    Public Function cmdAddCreature(ByRef c As CharacterObject, ByVal Message As String) As Boolean

        Dim tmpCr As CreatureObject = New CreatureObject(CType(Message, Integer), c.positionX, c.positionY, c.positionZ, c.orientation, c.MapID)
        tmpCr.AddToWorld()
        c.CommandResponse("Creature [" & tmpCr.Name & "] spawned.")

        Return True
    End Function
    <ChatCommandAttribute("NPCFlood", "NPCFLOOD <Amount> - Spawn a number of creatures at your position.")> _
    Public Function cmdCreatureFlood(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If IsNumeric(Message) = False OrElse CInt(Message) <= 0 Then Return False
        For i As Integer = 1 To CInt(Message)
            Dim tmpCreature As New CreatureObject(7385, c.positionX, c.positionY, c.positionZ, c.orientation, c.MapID)
            tmpCreature.CreatedBy = c.GUID
            tmpCreature.CreatedBySpell = 10673
            tmpCreature.AddToWorld()
        Next

        Return True
    End Function
    <ChatCommandAttribute("Come", "COME - Selected creature will come to your position.")> _
    Public Function cmdComeCreature(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If
        If Not WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            c.CommandResponse("Selected target is not creature!")
            Return True
        End If

        Dim creature As CreatureObject = WORLD_CREATUREs(c.TargetGUID)

        If creature.aiScript IsNot Nothing AndAlso creature.aiScript.InCombat() Then
            c.CommandResponse("Creature is in combat. It has to be out of combat first.")
            Return True
        End If

        creature.SetToRealPosition(True)
        Dim MoveTime As Integer = creature.MoveTo(c.positionX, c.positionY, c.positionZ, c.orientation)
        If creature.aiScript IsNot Nothing Then
            creature.aiScript.Pause(MoveTime) 'Make sure it doesn't do anything in this period
        End If

        Return True
    End Function
    <ChatCommandAttribute("Kill", "KILL - Selected creature or character will die.")> _
    Public Function cmdKillCreature(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If c.TargetGUID = 0 Then
            c.CommandResponse("Select target first!")
            Return True
        End If

        If CHARACTERs.ContainsKey(c.TargetGUID) Then
            CHARACTERs(c.TargetGUID).Die(c)
            Return True
        ElseIf WORLD_CREATUREs.ContainsKey(c.TargetGUID) Then
            CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject).DealDamage(CType(WORLD_CREATUREs(c.TargetGUID), CreatureObject).Life.Maximum)
            Return True
        End If
        Return False
    End Function

    <ChatCommandAttribute("TargetGo", "TARGETGO - Nearest game object will be selected.")> _
    Public Function cmdTargetGameObject(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        Dim targetGO As GameObjectObject = GetClosestGameobject(c)

        If targetGO Is Nothing Then
            c.CommandResponse("Could not find any near objects.")
        Else
            Dim distance As Single = GetDistance(targetGO, c)
            c.CommandResponse(String.Format("Selected [{0}][{1}] game object at distance {2}.", targetGO.ID, targetGO.Name, distance))
        End If

        Return True
    End Function

    <ChatCommandAttribute("ActivateGo", "ACTIVATEGO - Activates your targetted game object.")> _
    Public Function cmdActivateGameObject(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If WORLD_GAMEOBJECTs.ContainsKey(c.TargetGUID) = False Then Return False

        If WORLD_GAMEOBJECTs(c.TargetGUID).State = GameObjectLootState.DOOR_CLOSED Then
            WORLD_GAMEOBJECTs(c.TargetGUID).State = GameObjectLootState.DOOR_OPEN
            WORLD_GAMEOBJECTs(c.TargetGUID).SetState(GameObjectLootState.DOOR_OPEN)
        Else
            WORLD_GAMEOBJECTs(c.TargetGUID).State = GameObjectLootState.DOOR_CLOSED
            WORLD_GAMEOBJECTs(c.TargetGUID).SetState(GameObjectLootState.DOOR_CLOSED)
        End If

        c.CommandResponse(String.Format("Activated game object [{0}] to state [{1}].", WORLD_GAMEOBJECTs(c.TargetGUID).Name, WORLD_GAMEOBJECTs(c.TargetGUID).State))

        Return True
    End Function

    <ChatCommandAttribute("AddGO", "ADDGO <ID> - Spawn game object at your position."), _
     ChatCommandAttribute("AddGameObject", "ADDGAMEOBJECT <ID> - Spawn game object at your position.")> _
    Public Function cmdAddGameObject(ByRef c As CharacterObject, ByVal Message As String) As Boolean

        Dim tmpGO As GameObjectObject = New GameObjectObject(CType(Message, Integer), c.MapID, c.positionX, c.positionY, c.positionZ, c.orientation)
        tmpGO.Rotations(2) = Math.Sin(tmpGO.orientation / 2)
        tmpGO.Rotations(3) = Math.Cos(tmpGO.orientation / 2)
        tmpGO.AddToWorld()

        c.CommandResponse(String.Format("GameObject [{0}][{1:X}] spawned.", tmpGO.Name, tmpGO.GUID))

        Return True
    End Function
    <ChatCommandAttribute("CreateAccount", "CreateAccount <Name> <Password> <Email> - Add a new account using Name, Password, and Email.")> _
    Public Function cmdCreateAccount(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim result As New DataTable
        Dim acct() As String
        acct = Split(Trim(Message), " ")
        If acct.Length <> 3 Then Return False

        Dim aName As String = acct(0)
        Dim aPassword As String = acct(1)
        Dim aEmail As String = acct(2)
        AccountDatabase.Query("SELECT account FROM accounts WHERE account = """ & aName & """;", result)
        If result.Rows.Count > 0 Then
            c.CommandResponse(String.Format("Account [{0}] already exists.", aName))
        Else
            Dim passwordStr() As Byte = System.Text.Encoding.ASCII.GetBytes(aName.ToUpper & ":" & aPassword.ToUpper)
            Dim passwordHash() As Byte = New System.Security.Cryptography.SHA1Managed().ComputeHash(passwordStr)
            Dim hashStr As String = BitConverter.ToString(passwordHash).Replace("-", "")

            AccountDatabase.Insert(String.Format("INSERT INTO accounts (account, password, email, joindate, last_ip) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')", aName, hashStr, aEmail, Format(Now, "yyyy-MM-dd"), "0.0.0.0"))
            c.CommandResponse(String.Format("Account [{0}] has been created.", aName))
        End If
        Return True
    End Function
    <ChatCommandAttribute("ChangePassword", "ChangePassword <Name> <Password> - Changes the password of an account.")> _
    Public Function cmdChangePassword(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim result As New DataTable
        Dim acct() As String
        acct = Split(Trim(Message), " ")
        If acct.Length <> 2 Then Return False

        Dim aName As String = acct(0)
        Dim aPassword As String = acct(1)

        AccountDatabase.Query("SELECT account_id, plevel FROM accounts WHERE account = """ & aName & """;", result)
        If result.Rows.Count = 0 Then
            c.CommandResponse(String.Format("Account [{0}] does not exist.", aName))
        Else
            Dim targetLevel As AccessLevel = CType(result.Rows(0).Item("plevel"), AccessLevel)
            If targetLevel >= c.Access Then
                c.CommandResponse("You cannot change password for accounts with the same or a higher access level than yourself.")
            Else
                Dim passwordStr() As Byte = System.Text.Encoding.ASCII.GetBytes(aName.ToUpper & ":" & aPassword.ToUpper)
                Dim passwordHash() As Byte = New System.Security.Cryptography.SHA1Managed().ComputeHash(passwordStr)
                Dim hashStr As String = BitConverter.ToString(passwordHash).Replace("-", "")

                AccountDatabase.Update(String.Format("UPDATE accounts SET password='{0}' WHERE account_id={1}", hashStr, result.Rows(0).Item("account_id")))
                c.CommandResponse(String.Format("Account [{0}] now has a new password [{1}].", aName, aPassword))
            End If
        End If
        Return True
    End Function
    <ChatCommandAttribute("SetAccess", "SetAccess <Name> <AccessLevel> - Sets the account to a specific access level.")> _
    Public Function cmdSetAccess(ByRef c As CharacterObject, ByVal Message As String) As Boolean
        If Message = "" Then Return False
        Dim result As New DataTable
        Dim acct() As String
        acct = Split(Trim(Message), " ")
        If acct.Length <> 2 Then Return False

        Dim aName As String = acct(0)
        Dim aLevel As Byte
        If Byte.TryParse(acct(1), aLevel) = False Then Return False

        If aLevel < AccessLevel.Trial OrElse aLevel > AccessLevel.Developer Then
            c.CommandResponse(String.Format("Not a valid access level. Must be in the range {0}-{1}.", CByte(AccessLevel.Trial), CByte(AccessLevel.Developer)))
            Return True
        End If

        Dim newLevel As AccessLevel = CType(aLevel, AccessLevel)
        If newLevel >= c.Access Then
            c.CommandResponse("You cannot set access levels to your own or above your own access level.")
            Return True
        End If

        AccountDatabase.Query("SELECT account_id, plevel FROM accounts WHERE account = """ & aName & """;", result)
        If result.Rows.Count = 0 Then
            c.CommandResponse(String.Format("Account [{0}] does not exist.", aName))
        Else
            Dim targetLevel As AccessLevel = CType(result.Rows(0).Item("plevel"), AccessLevel)
            If targetLevel >= c.Access Then
                c.CommandResponse("You cannot set access levels to accounts with the same or a higher access level than yourself.")
            Else
                AccountDatabase.Update(String.Format("UPDATE accounts SET plevel={0} WHERE account_id={1}", CByte(newLevel), result.Rows(0).Item("account_id")))
                c.CommandResponse(String.Format("Account [{0}] now has access level [{1}].", newLevel))
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
        WS.Cluster.Broadcast(packet.Data)
        packet.Dispose()
    End Sub
    Public Function SetUpdateValue(ByVal GUID As ULong, ByVal Index As Integer, ByVal Value As Integer, ByVal Client As ClientClass) As Boolean
        Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
        packet.AddInt32(1)      'Operations.Count
        packet.AddInt8(0)
        Dim UpdateData As New UpdateClass
        UpdateData.SetUpdateFlag(Index, Value)

        If GuidIsCreature(GUID) Then
            UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(WORLD_CREATUREs(GUID), CreatureObject))
        ElseIf GuidIsPlayer(GUID) Then
            If GUID = Client.Character.GUID Then
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(CHARACTERs(GUID), CharacterObject))
            Else
                UpdateData.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, CType(CHARACTERs(GUID), CharacterObject))
            End If
        End If

        Client.Send(packet)
        packet.Dispose()
        UpdateData.Dispose()
    End Function


#End Region


End Module


