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

Public Module WC_Handlers_Social

#Region "Framework"

    Public Sub LoadIgnoreList(ByRef objCharacter As CharacterObject)
        'DONE: Query DB
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_social WHERE char_guid = {0} AND flags = {1};", objCharacter.GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)

        'DONE: Add to list
        For Each r As DataRow In q.Rows
            objCharacter.IgnoreList.Add(CType(r.Item("guid"), ULong))
        Next
    End Sub

    Public Sub SendFriendList(ByRef client As ClientClass, ByRef Character As CharacterObject)
        'DONE: Query DB
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_social WHERE char_guid = {0} AND (flags & {1}) > 0;", Character.GUID, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Integer)), q)

        'DONE: Make the packet
        Dim SMSG_FRIEND_LIST As New PacketClass(OPCODES.SMSG_FRIEND_LIST)
        If q.Rows.Count > 0 Then
            SMSG_FRIEND_LIST.AddInt8(q.Rows.Count)

            For Each r As DataRow In q.Rows
                Dim GUID As ULong = r.Item("guid")
                SMSG_FRIEND_LIST.AddUInt64(GUID)                    'Player GUID
                If CHARACTERs.ContainsKey(GUID) AndAlso CHARACTERs(GUID).IsInWorld Then
                    'If CType(CHARACTERs(guid), CharacterObject).DND Then
                    '    SMSG_FRIEND_LIST.AddInt8(FriendStatus.FRIEND_STATUS_DND)
                    'ElseIf CType(CHARACTERs(guid), CharacterObject).AFK Then
                    '    SMSG_FRIEND_LIST.AddInt8(FriendStatus.FRIEND_STATUS_AFK)
                    'Else
                    SMSG_FRIEND_LIST.AddInt8(FriendStatus.FRIEND_STATUS_ONLINE)
                    'End If
                    SMSG_FRIEND_LIST.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Zone)    'Area
                    SMSG_FRIEND_LIST.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Level)   'Level
                    SMSG_FRIEND_LIST.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Classe)  'Class
                Else
                    SMSG_FRIEND_LIST.AddInt8(FriendStatus.FRIEND_STATUS_OFFLINE)
                End If
            Next
        Else
            SMSG_FRIEND_LIST.AddInt8(0)
        End If

        client.Send(SMSG_FRIEND_LIST)
        SMSG_FRIEND_LIST.Dispose()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_LIST", client.IP, client.Port)
    End Sub

    Public Sub SendIgnoreList(ByRef client As ClientClass, ByRef Character As CharacterObject)
        'DONE: Query DB
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_social WHERE char_guid = {0} AND (flags & {1}) > 0;", Character.GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Integer)), q)

        'DONE: Make the packet
        Dim SMSG_IGNORE_LIST As New PacketClass(OPCODES.SMSG_IGNORE_LIST)
        If q.Rows.Count > 0 Then
            SMSG_IGNORE_LIST.AddInt8(q.Rows.Count)

            For Each r As DataRow In q.Rows
                SMSG_IGNORE_LIST.AddUInt64(CType(r.Item("guid"), ULong))                    'Player GUID
            Next
        Else
            SMSG_IGNORE_LIST.AddInt8(0)
        End If

        client.Send(SMSG_IGNORE_LIST)
        SMSG_IGNORE_LIST.Dispose()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_IGNORE_LIST", client.IP, client.Port)
    End Sub

    Public Sub NotifyFriendStatus(ByRef objCharacter As CharacterObject, ByVal s As FriendStatus)
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters_social WHERE guid = {0} AND (flags & {1}) > 0;", objCharacter.GUID, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Integer)), q)

        'DONE: Send "Friend offline/online"
        Dim friendpacket As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
        friendpacket.AddInt8(s)
        friendpacket.AddUInt64(objCharacter.GUID)
        For Each r As DataRow In q.Rows
            Dim GUID As ULong = r.Item("char_guid")
            If CHARACTERs.ContainsKey(GUID) AndAlso CHARACTERs(GUID).Client IsNot Nothing Then
                CHARACTERs(GUID).Client.SendMultiplyPackets(friendpacket)
            End If
        Next
        friendpacket.Dispose()
    End Sub

#End Region

#Region "Handlers"

    Public Sub On_CMSG_WHO(ByRef packet As PacketClass, ByRef client As ClientClass)
        packet.GetInt16()
        Dim LevelMinimum As UInteger = packet.GetUInt32()       '0
        Dim LevelMaximum As UInteger = packet.GetUInt32()       '100
        Dim NamePlayer As String = EscapeString(packet.GetString())
        Dim NameGuild As String = EscapeString(packet.GetString())
        Dim MaskRace As UInteger = packet.GetUInt32()
        Dim MaskClass As UInteger = packet.GetUInt32()
        Dim ZonesCount As UInteger = packet.GetUInt32()         'Limited to 10
        If ZonesCount > 10 Then Exit Sub
        Dim Zones As New List(Of UInteger)
        For i As Integer = 1 To ZonesCount
            Zones.Add(packet.GetUInt32)
        Next
        Dim StringsCount As UInteger = packet.GetUInt32         'Limited to 4
        If StringsCount > 4 Then Exit Sub
        Dim Strings As New List(Of String)
        For i As Integer = 1 To StringsCount
            Strings.Add(UCase(EscapeString(packet.GetString())))
        Next

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_WHO [P:'{2}' G:'{3}' L:{4}-{5} C:{6:X} R:{7:X}]", client.IP, client.Port, NamePlayer, NameGuild, LevelMinimum, LevelMaximum, MaskClass, MaskRace)

        'TODO: Don't show GMs?
        Dim results As New List(Of ULong)
        CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
        For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
            If Not objCharacter.Value.IsInWorld Then Continue For
            If (GetCharacterSide(objCharacter.Value.Race) <> GetCharacterSide(Client.Character.Race)) AndAlso client.Character.Access < AccessLevel.GameMaster Then Continue For
            If NamePlayer <> "" AndAlso UCase(objCharacter.Value.Name).IndexOf(UCase(NamePlayer)) = -1 Then Continue For
            If NameGuild <> "" AndAlso (objCharacter.Value.Guild Is Nothing OrElse UCase(objCharacter.Value.Guild.Name).IndexOf(UCase(NameGuild)) = -1) Then Continue For
            If objCharacter.Value.Level < LevelMinimum Then Continue For
            If objCharacter.Value.Level > LevelMaximum Then Continue For
            If ZonesCount > 0 AndAlso Zones.Contains(objCharacter.Value.Zone) = False Then Continue For
            If StringsCount > 0 Then
                Dim PassedStrings As Boolean = True
                For Each StringValue As String In Strings
                    If UCase(objCharacter.Value.Name).IndexOf(StringValue) <> -1 Then Continue For
                    If UCase(GetRaceName(objCharacter.Value.Race)) = StringValue Then Continue For
                    If UCase(GetClassName(objCharacter.Value.Classe)) = StringValue Then Continue For
                    If objCharacter.Value.Guild IsNot Nothing AndAlso UCase(objCharacter.Value.Guild.Name).IndexOf(StringValue) <> -1 Then Continue For
                    'TODO: Look for zone name
                    PassedStrings = False
                    Exit For
                Next
                If PassedStrings = False Then Continue For
            End If

            'DONE: List first 49 characters (like original)
            If results.Count > 49 Then Exit For

            results.Add(objCharacter.Value.GUID)
        Next

        Dim response As New PacketClass(OPCODES.SMSG_WHO)
        response.AddInt32(results.Count)
        response.AddInt32(results.Count)

        For Each GUID As ULong In results
            response.AddString(CHARACTERs(GUID).Name)           'Name
            If CHARACTERs(GUID).Guild IsNot Nothing Then
                response.AddString(CHARACTERs(GUID).Guild.Name) 'Guild Name
            Else
                response.AddString("")                          'Guild Name
            End If
            response.AddInt32(CHARACTERs(GUID).Level)           'Level
            response.AddInt32(CHARACTERs(GUID).Classe)          'Class
            response.AddInt32(CHARACTERs(GUID).Race)            'Race
            response.AddInt32(CHARACTERs(GUID).Zone)            'Zone ID
        Next
        CHARACTERs_Lock.ReleaseReaderLock()

        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_ADD_FRIEND(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()

        Dim response As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
        Dim name As String = packet.GetString()
        Dim GUID As ULong = 0
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ADD_FRIEND [{2}]", client.IP, client.Port, name)

        'DONE: Get GUID from DB
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT char_guid, char_race FROM characters WHERE char_name = ""{0}"";", name), q)

        If q.Rows.Count > 0 Then
            GUID = CType(q.Rows(0).Item("char_guid"), Long)
            Dim FriendSide As Boolean = GetCharacterSide(CType(q.Rows(0).Item("char_race"), Byte))

            q.Clear()
            CharacterDatabase.Query(String.Format("SELECT flags FROM characters_social WHERE flags = {0}", CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)), q)
            Dim NumberOfFriends As Integer = q.Rows.Count
            q.Clear()
            CharacterDatabase.Query(String.Format("SELECT flags FROM characters_social WHERE char_guid = {0} AND guid = {1} AND flags = {2};", client.Character.GUID, GUID, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)), q)

            If GUID = client.Character.GUID Then
                response.AddInt8(FriendResult.FRIEND_SELF)
                response.AddUInt64(GUID)
            ElseIf q.Rows.Count > 0 Then
                response.AddInt8(FriendResult.FRIEND_ALREADY)
                response.AddUInt64(GUID)
            ElseIf NumberOfFriends >= SocialList.MAX_FRIENDS_ON_LIST Then
                response.AddInt8(FriendResult.FRIEND_LIST_FULL)
                response.AddUInt64(GUID)
            ElseIf GetCharacterSide(Client.Character.Race) <> FriendSide Then
                response.AddInt8(FriendResult.FRIEND_ENEMY)
                response.AddUInt64(GUID)
            ElseIf CHARACTERs.ContainsKey(GUID) Then
                response.AddInt8(FriendResult.FRIEND_ADDED_ONLINE)
                response.AddUInt64(GUID)
                response.AddString(name)
                If CType(CHARACTERs(GUID), CharacterObject).DND Then
                    response.AddInt8(FriendStatus.FRIEND_STATUS_DND)
                ElseIf CType(CHARACTERs(GUID), CharacterObject).AFK Then
                    response.AddInt8(FriendStatus.FRIEND_STATUS_AFK)
                Else
                    response.AddInt8(FriendStatus.FRIEND_STATUS_ONLINE)
                End If
                response.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Zone)
                response.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Level)
                response.AddInt32(CType(CHARACTERs(GUID), CharacterObject).Classe)
                CharacterDatabase.Update(String.Format("INSERT INTO characters_social (char_guid, guid, flags) VALUES ({0}, {1}, {2});", client.Character.GUID, GUID, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)))
            Else
                response.AddInt8(FriendResult.FRIEND_ADDED_OFFLINE)
                response.AddUInt64(GUID)
                response.AddString(name)
                CharacterDatabase.Update(String.Format("INSERT INTO characters_social (char_guid, guid, flags) VALUES ({0}, {1}, {2});", client.Character.GUID, GUID, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)))
            End If
        Else
            response.AddInt8(FriendResult.FRIEND_NOT_FOUND)
            response.AddUInt64(GUID)
        End If

        client.Send(response)
        response.Dispose()
        q.Dispose()
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_STATUS", client.IP, client.Port)
    End Sub

    Public Sub On_CMSG_ADD_IGNORE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 6 Then Exit Sub
        packet.GetInt16()
        Dim response As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
        Dim name As String = packet.GetString()
        Dim GUID As ULong = 0
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ADD_IGNORE [{2}]", client.IP, client.Port, name)

        'DONE: Get GUID from DB
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE char_name = ""{0}"";", name), q)

        If q.Rows.Count > 0 Then
            GUID = CType(q.Rows(0).Item("char_guid"), Long)
            q.Clear()
            CharacterDatabase.Query(String.Format("SELECT flags FROM characters_social WHERE flags = {0}", CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)
            Dim NumberOfFriends As Integer = q.Rows.Count
            q.Clear()
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_social WHERE char_guid = {0} AND guid = {1} AND flags = {2};", client.Character.GUID, GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)

            If GUID = client.Character.GUID Then
                response.AddInt8(FriendResult.FRIEND_IGNORE_SELF)
                response.AddUInt64(GUID)
            ElseIf q.Rows.Count > 0 Then
                response.AddInt8(FriendResult.FRIEND_IGNORE_ALREADY)
                response.AddUInt64(GUID)
            ElseIf NumberOfFriends >= SocialList.MAX_IGNORES_ON_LIST Then
                response.AddInt8(FriendResult.FRIEND_IGNORE_ALREADY)
                response.AddUInt64(GUID)
            Else
                response.AddInt8(FriendResult.FRIEND_IGNORE_ADDED)
                response.AddUInt64(GUID)

                CharacterDatabase.Update(String.Format("INSERT INTO characters_social (char_guid, guid, flags) VALUES ({0}, {1}, {2});", client.Character.GUID, GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)))
                client.Character.IgnoreList.Add(GUID)
            End If
        Else
            response.AddInt8(FriendResult.FRIEND_IGNORE_NOT_FOUND)
            response.AddUInt64(GUID)
        End If

        client.Send(response)
        response.Dispose()
        q.Dispose()
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_STATUS", client.IP, client.Port)
    End Sub

    Public Sub On_CMSG_DEL_FRIEND(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_DEL_FRIEND", client.IP, client.Port)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim response As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
        Dim GUID As ULong = packet.GetUInt64()

        Try
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT flags FROM characters_social WHERE char_guid = {0} AND guid = {1};", client.Character.GUID, GUID), q)

            If q.Rows.Count > 0 Then
                Dim flags As Integer = CType(q.Rows(0).Item("flags"), Integer)
                Dim newFlags As Integer = (flags And (Not SocialFlag.SOCIAL_FLAG_FRIEND))
                If (newFlags And (SocialFlag.SOCIAL_FLAG_FRIEND Or SocialFlag.SOCIAL_FLAG_IGNORED)) = 0 Then
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_social WHERE guid = {1} AND char_guid = {0};", client.Character.GUID, GUID))
                Else
                    CharacterDatabase.Update(String.Format("UPDATE characters_social SET flags = {2} WHERE guid = {1} AND char_guid = {0};", client.Character.GUID, GUID, newFlags))
                End If
                response.AddInt8(FriendResult.FRIEND_REMOVED)
            Else
                response.AddInt8(FriendResult.FRIEND_NOT_FOUND)
            End If
        Catch
            response.AddInt8(FriendResult.FRIEND_DB_ERROR)
        End Try

        response.AddUInt64(GUID)

        client.Send(response)
        response.Dispose()
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_STATUS", client.IP, client.Port)
    End Sub

    Public Sub On_CMSG_DEL_IGNORE(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_DEL_IGNORE", client.IP, client.Port)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim response As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
        Dim GUID As ULong = packet.GetUInt64()

        Try
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT flags FROM characters_social WHERE char_guid = {0} AND guid = {1};", client.Character.GUID, GUID), q)

            If q.Rows.Count > 0 Then
                Dim flags As Integer = CType(q.Rows(0).Item("flags"), Integer)
                Dim newFlags As Integer = (flags And (Not SocialFlag.SOCIAL_FLAG_IGNORED))
                If (newFlags And (SocialFlag.SOCIAL_FLAG_FRIEND Or SocialFlag.SOCIAL_FLAG_IGNORED)) = 0 Then
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_social WHERE guid = {1} AND char_guid = {0};", client.Character.GUID, GUID))
                Else
                    CharacterDatabase.Update(String.Format("UPDATE characters_social SET flags = {2} WHERE guid = {1} AND char_guid = {0};", client.Character.GUID, GUID, newFlags))
                End If
                response.AddInt8(FriendResult.FRIEND_IGNORE_REMOVED)
            Else
                response.AddInt8(FriendResult.FRIEND_IGNORE_NOT_FOUND)
            End If
        Catch
            response.AddInt8(FriendResult.FRIEND_DB_ERROR)
        End Try
        response.AddUInt64(GUID)

        client.Send(response)
        response.Dispose()
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_STATUS", client.IP, client.Port)
    End Sub

    Public Sub On_CMSG_FRIEND_LIST(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_FRIEND_LIST", client.IP, client.Port)
        SendFriendList(Client, client.Character)
        SendIgnoreList(Client, client.Character)
    End Sub

#End Region

End Module