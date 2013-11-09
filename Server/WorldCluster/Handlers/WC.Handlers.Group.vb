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

Imports mangosVB.Common.Logger
Imports mangosVB.Common

Public Module WC_Handlers_Group

    Public Sub On_CMSG_REQUEST_RAID_INFO(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_REQUEST_RAID_INFO", client.IP, client.Port)

        Dim q As New DataTable
        If client.Character IsNot Nothing Then
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_instances WHERE char_guid = {0};", client.Character.GUID), q)
        End If

        Dim response As New PacketClass(OPCODES.SMSG_RAID_INSTANCE_INFO)
        response.AddInt32(q.Rows.Count)                                 'Instances Counts

        Dim i As Integer = 0
        For Each r As DataRow In q.Rows
            response.AddUInt32(r.Item("map"))                               'MapID
            response.AddUInt32(CInt(r.Item("expire")) - GetTimestamp(Now))  'TimeLeft
            response.AddUInt32(r.Item("instance"))                          'InstanceID
            'TODO: Is this is a counter, shouldn't it be counting ?
            response.AddUInt32(i)                                           'Counter
        Next
        client.Send(response)
        response.Dispose()

    End Sub

    Public Enum PartyCommand As Byte
        PARTY_OP_INVITE = 0
        PARTY_OP_LEAVE = 2
    End Enum

    Public Enum PartyCommandResult As Byte
        INVITE_OK = 0                   'You have invited [name] to join your group.
        INVITE_NOT_FOUND = 1            'Cannot find [name].
        INVITE_NOT_IN_YOUR_PARTY = 2    '[name] is not in your party.
        INVITE_NOT_IN_YOUR_INSTANCE = 3 '[name] is not in your instance.
        INVITE_PARTY_FULL = 4           'Your party is full.
        INVITE_ALREADY_IN_GROUP = 5     '[name] is already in group.
        INVITE_NOT_IN_PARTY = 6         'You aren't in party.
        INVITE_NOT_LEADER = 7           'You are not the party leader.
        INVITE_NOT_SAME_SIDE = 8        'gms - Target is not part of your alliance.
        INVITE_IGNORED = 9              'Test is ignoring you.
        INVITE_RESTRICTED = 13
    End Enum

    Public Sub SendPartyResult(ByVal objCharacter As ClientClass, ByVal Name As String, ByVal operation As PartyCommand, ByVal result As PartyCommandResult)
        Dim response As New PacketClass(OPCODES.SMSG_PARTY_COMMAND_RESULT)
        response.AddInt32(operation)
        response.AddString(Name)
        response.AddInt32(result)
        objCharacter.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_GROUP_INVITE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim Name As String = CapitalizeName(packet.GetString)

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_INVITE [{2}]", client.IP, client.Port, Name)

        Dim GUID As ULong = 0
        CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
        For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
            If UCase(Character.Value.Name) = UCase(Name) Then
                GUID = Character.Value.GUID
                Exit For
            End If
        Next
        CHARACTERs_Lock.ReleaseReaderLock()

        Dim errCode As PartyCommandResult = PartyCommandResult.INVITE_OK
        'TODO: InBattlegrounds: INVITE_RESTRICTED
        If GUID = 0 Then
            errCode = PartyCommandResult.INVITE_NOT_FOUND
        ElseIf CHARACTERs(GUID).IsInWorld = False Then
            errCode = PartyCommandResult.INVITE_NOT_FOUND
        ElseIf GetCharacterSide(CHARACTERs(GUID).Race) <> GetCharacterSide(Client.Character.Race) Then
            errCode = PartyCommandResult.INVITE_NOT_SAME_SIDE
        ElseIf CHARACTERs(GUID).IsInGroup Then
            errCode = PartyCommandResult.INVITE_ALREADY_IN_GROUP
            Dim denied As New PacketClass(OPCODES.SMSG_GROUP_INVITE)
            denied.AddInt8(0)
            denied.AddString(Client.Character.Name)
            CHARACTERs(GUID).Client.Send(denied)
            denied.Dispose()
        ElseIf CHARACTERs(GUID).IgnoreList.Contains(Client.Character.GUID) Then
            errCode = PartyCommandResult.INVITE_IGNORED
        Else
            If Not client.Character.IsInGroup Then
                Dim g As New Group(Client.Character)
                CHARACTERs(GUID).Group = client.Character.Group
                CHARACTERs(GUID).GroupInvitedFlag = True
            Else
                If client.Character.Group.IsFull Then
                    errCode = PartyCommandResult.INVITE_PARTY_FULL
                ElseIf client.Character.IsGroupLeader = False AndAlso client.Character.GroupAssistant = False Then
                    errCode = PartyCommandResult.INVITE_NOT_LEADER
                Else
                    CHARACTERs(GUID).Group = client.Character.Group
                    CHARACTERs(GUID).GroupInvitedFlag = True
                End If
            End If

        End If

        SendPartyResult(Client, Name, PartyCommand.PARTY_OP_INVITE, errCode)

        If errCode = PartyCommandResult.INVITE_OK Then
            Dim invited As New PacketClass(OPCODES.SMSG_GROUP_INVITE)
            invited.AddInt8(1)
            invited.AddString(Client.Character.Name)
            CHARACTERs(GUID).Client.Send(invited)
            invited.Dispose()
        End If
    End Sub

    Public Sub On_CMSG_GROUP_CANCEL(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_CANCEL", client.IP, client.Port)
    End Sub

    Public Sub On_CMSG_GROUP_ACCEPT(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_ACCEPT", client.IP, client.Port)
        If client.Character.GroupInvitedFlag AndAlso Not client.Character.Group.IsFull Then
            client.Character.Group.Join(Client.Character)
        Else
            SendPartyResult(Client, client.Character.Name, PartyCommand.PARTY_OP_INVITE, PartyCommandResult.INVITE_PARTY_FULL)
            client.Character.Group = Nothing
        End If

        client.Character.GroupInvitedFlag = False
    End Sub

    Public Sub On_CMSG_GROUP_DECLINE(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_DECLINE", client.IP, client.Port)
        If client.Character.GroupInvitedFlag Then
            Dim response As New PacketClass(OPCODES.SMSG_GROUP_DECLINE)
            response.AddString(Client.Character.Name)
            client.Character.Group.GetLeader.Client.Send(response)
            response.Dispose()

            client.Character.Group.CheckMembers()
            client.Character.Group = Nothing
            client.Character.GroupInvitedFlag = False
        End If
    End Sub

    Public Sub On_CMSG_GROUP_DISBAND(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_DISBAND", client.IP, client.Port)

        If client.Character.IsInGroup Then
            'TODO: InBattlegrounds: INVITE_RESTRICTED
            If client.Character.Group.GetMembersCount > 2 Then
                client.Character.Group.Leave(Client.Character)
            Else
                client.Character.Group.Dispose()
            End If
        End If
    End Sub

    Public Sub On_CMSG_GROUP_UNINVITE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim Name As String = packet.GetString

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_UNINVITE [{2}]", client.IP, client.Port, Name)

        Dim GUID As ULong = 0
        CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
        For Each Character As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
            If UCase(Character.Value.Name) = UCase(Name) Then
                GUID = Character.Value.GUID
                Exit For
            End If
        Next
        CHARACTERs_Lock.ReleaseReaderLock()

        'TODO: InBattlegrounds: INVITE_RESTRICTED
        If GUID = 0 Then
            SendPartyResult(Client, Name, PartyCommand.PARTY_OP_LEAVE, PartyCommandResult.INVITE_NOT_FOUND)
        ElseIf Not client.Character.IsGroupLeader Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_LEAVE, PartyCommandResult.INVITE_NOT_LEADER)
        Else
            client.Character.Group.Leave(CHARACTERs(GUID))
        End If

    End Sub

    Public Sub On_CMSG_GROUP_UNINVITE_GUID(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_UNINVITE_GUID [0x{2:X}]", client.IP, client.Port, GUID)

        'TODO: InBattlegrounds: INVITE_RESTRICTED
        If GUID = 0 Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_LEAVE, PartyCommandResult.INVITE_NOT_FOUND)
        ElseIf CHARACTERs.ContainsKey(GUID) = False Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_LEAVE, PartyCommandResult.INVITE_NOT_FOUND)
        ElseIf Not client.Character.IsGroupLeader Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_LEAVE, PartyCommandResult.INVITE_NOT_LEADER)
        Else
            client.Character.Group.Leave(CHARACTERs(GUID))
        End If
    End Sub

    Public Sub On_CMSG_GROUP_SET_LEADER(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim Name As String = packet.GetString()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_SET_LEADER [Name={2}]", client.IP, client.Port, Name)

        Dim GUID As ULong = GetCharacterGUIDByName(Name)
        If GUID = 0 Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_INVITE, PartyCommandResult.INVITE_NOT_FOUND)
        ElseIf CHARACTERs.ContainsKey(GUID) = False Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_INVITE, PartyCommandResult.INVITE_NOT_FOUND)
        ElseIf Not client.Character.IsGroupLeader Then
            SendPartyResult(Client, client.Character.Name, PartyCommand.PARTY_OP_INVITE, PartyCommandResult.INVITE_NOT_LEADER)
        Else
            client.Character.Group.SetLeader(CHARACTERs(GUID))
        End If
    End Sub

    Public Sub On_CMSG_GROUP_RAID_CONVERT(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_RAID_CONVERT", client.IP, client.Port)

        If client.Character.IsInGroup Then
            SendPartyResult(Client, "", PartyCommand.PARTY_OP_INVITE, PartyCommandResult.INVITE_OK)

            client.Character.Group.ConvertToRaid()
            client.Character.Group.SendGroupList()

            WorldServer.GroupSendUpdate(Client.Character.Group.ID)
        End If
    End Sub

    Public Sub On_CMSG_GROUP_CHANGE_SUB_GROUP(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim name As String = packet.GetString
        If (packet.Data.Length - 1) < (6 + name.Length + 1) Then Exit Sub
        Dim subGroup As Byte = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_CHANGE_SUB_GROUP [{2}:{3}]", client.IP, client.Port, name, subGroup)

        If client.Character.IsInGroup Then
            Dim j As Integer

            For j = subGroup * GROUP_SUBGROUPSIZE To ((subGroup + 1) * GROUP_SUBGROUPSIZE - 1)
                If client.Character.Group.Members(j) Is Nothing Then
                    Exit For
                End If
            Next

            For i As Integer = 0 To client.Character.Group.Members.Length - 1
                If (Not client.Character.Group.Members(i) Is Nothing) AndAlso client.Character.Group.Members(i).Name = name Then
                    client.Character.Group.Members(j) = client.Character.Group.Members(i)
                    client.Character.Group.Members(i) = Nothing
                    If client.Character.Group.Leader = i Then client.Character.Group.Leader = j
                    client.Character.Group.SendGroupList()
                    Exit For
                End If
            Next
        End If
    End Sub

    Public Sub On_CMSG_GROUP_SWAP_SUB_GROUP(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim name1 As String = packet.GetString
        If (packet.Data.Length - 1) < (6 + name1.Length + 1) Then Exit Sub
        Dim name2 As String = packet.GetString

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GROUP_SWAP_SUB_GROUP [{2}:{3}]", client.IP, client.Port, name1, name2)

        If client.Character.IsInGroup Then
            Dim j As Integer

            For j = 0 To client.Character.Group.Members.Length - 1
                If (Not client.Character.Group.Members(j) Is Nothing) AndAlso client.Character.Group.Members(j).Name = name2 Then
                    Exit For
                End If
            Next

            For i As Integer = 0 To client.Character.Group.Members.Length - 1
                If (Not client.Character.Group.Members(i) Is Nothing) AndAlso client.Character.Group.Members(i).Name = name1 Then
                    Dim tmpPlayer As CharacterObject = client.Character.Group.Members(j)
                    client.Character.Group.Members(j) = client.Character.Group.Members(i)
                    client.Character.Group.Members(i) = tmpPlayer
                    tmpPlayer = Nothing

                    If client.Character.Group.Leader = i Then
                        client.Character.Group.Leader = j
                    ElseIf client.Character.Group.Leader = j Then
                        client.Character.Group.Leader = i
                    End If

                    client.Character.Group.SendGroupList()
                    Exit For
                End If
            Next
        End If
    End Sub

    Public Sub On_CMSG_LOOT_METHOD(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim Method As Integer = packet.GetInt32
        Dim Master As ULong = packet.GetUInt64
        Dim Threshold As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LOOT_METHOD [Method={2}, Master=0x{3:X}, Threshold={4}]", client.IP, client.Port, Method, Master, Threshold)

        If Not client.Character.IsGroupLeader Then
            Exit Sub
        End If

        client.Character.Group.SetLootMaster(Master)
        client.Character.Group.LootMethod = Method
        client.Character.Group.LootThreshold = Threshold
        client.Character.Group.SendGroupList()

        WorldServer.GroupSendUpdateLoot(Client.Character.Group.ID)
    End Sub

    Public Sub On_MSG_MINIMAP_PING(ByRef packet As PacketClass, ByRef client As ClientClass)
        packet.GetInt16()
        Dim x As Single = packet.GetFloat
        Dim y As Single = packet.GetFloat

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_MINIMAP_PING [{2}:{3}]", client.IP, client.Port, x, y)

        If client.Character.IsInGroup Then
            Dim response As New PacketClass(OPCODES.MSG_MINIMAP_PING)
            response.AddUInt64(Client.Character.GUID)
            response.AddSingle(x)
            response.AddSingle(y)
            client.Character.Group.Broadcast(response)
            response.Dispose()
        End If

    End Sub

    Public Sub On_MSG_RANDOM_ROLL(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim minRoll As Integer = packet.GetInt32
        Dim maxRoll As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_RANDOM_ROLL [min={2} max={3}]", client.IP, client.Port, minRoll, maxRoll)

        Dim response As New PacketClass(OPCODES.MSG_RANDOM_ROLL)
        response.AddInt32(minRoll)
        response.AddInt32(maxRoll)
        response.AddInt32(Rnd.Next(minRoll, maxRoll))
        response.AddUInt64(Client.Character.GUID)
        If client.Character.IsInGroup Then
            client.Character.Group.Broadcast(response)
        Else
            client.SendMultiplyPackets(response)
        End If
        response.Dispose()
    End Sub

    Public Sub On_MSG_RAID_READY_CHECK(ByRef packet As PacketClass, ByRef client As ClientClass)

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_RAID_READY_CHECK", client.IP, client.Port)

        If client.Character.IsGroupLeader Then
            client.Character.Group.BroadcastToOther(packet, client.Character)
        Else
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim result As Byte = packet.GetInt8

            If result = 0 Then
                'DONE: Not ready
                client.Character.Group.GetLeader.Client.Send(packet)
            Else
                'DONE: Ready
                Dim response As New PacketClass(OPCODES.MSG_RAID_READY_CHECK)
                response.AddUInt64(Client.Character.GUID)
                client.Character.Group.GetLeader.Client.Send(response)
                response.Dispose()
            End If
        End If
    End Sub

    Public Sub On_MSG_RAID_ICON_TARGET(ByRef packet As PacketClass, ByRef client As ClientClass)
        If packet.Data.Length < 7 Then Exit Sub 'Too short packet
        If client.Character.Group Is Nothing Then Exit Sub
        packet.GetInt16()
        Dim icon As Byte = packet.GetInt8()

        If icon = 255 Then
            'DONE: Send icon target list
            Dim response As New PacketClass(OPCODES.MSG_RAID_ICON_TARGET)
            response.AddInt8(1) 'Target list
            For i As Byte = 0 To 7
                If client.Character.Group.TargetIcons(i) = 0 Then Continue For

                response.AddInt8(i)
                response.AddUInt64(Client.Character.Group.TargetIcons(i))
            Next
            client.Send(response)
            response.Dispose()
        Else
            If icon > 7 Then Exit Sub 'Not a valid icon
            If packet.Data.Length < 15 Then Exit Sub 'Too short packet
            Dim GUID As ULong = packet.GetUInt64()

            'DONE: Set the raid icon target
            client.Character.Group.TargetIcons(icon) = GUID

            Dim response As New PacketClass(OPCODES.MSG_RAID_ICON_TARGET)
            response.AddInt8(0) 'Set target
            response.AddInt8(icon)
            response.AddUInt64(GUID)
            client.Character.Group.Broadcast(response)
            response.Dispose()
        End If
    End Sub

    Private Enum PromoteToMain As Byte
        MainTank = 0
        MainAssist = 1
    End Enum

    Public Sub On_CMSG_REQUEST_PARTY_MEMBER_STATS(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_REQUEST_PARTY_MEMBER_STATS [{2:X}]", client.IP, client.Port, GUID)

        If Not CHARACTERs.ContainsKey(GUID) Then
            'Character is offline
            Dim response As PacketClass = BuildPartyMemberStatsOffline(GUID)
            client.Send(response)
            response.Dispose()
        ElseIf CHARACTERs(GUID).IsInWorld = False Then
            'Character is offline (not in world)
            Dim response As PacketClass = BuildPartyMemberStatsOffline(GUID)
            client.Send(response)
            response.Dispose()
        Else
            'Request information from WorldServer
            Dim response As New PacketClass(0)
            response.Data = CHARACTERs(GUID).GetWorld.GroupMemberStats(GUID, 0)
            client.Send(response)
            response.Dispose()
        End If
    End Sub

End Module