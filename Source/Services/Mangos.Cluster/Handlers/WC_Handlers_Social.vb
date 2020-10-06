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
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Misc
Imports Mangos.Common.Enums.Social

Namespace Handlers

    Public Module WC_Handlers_Social

#Region "Framework"

        Public Sub LoadIgnoreList(ByRef objCharacter As CharacterObject)
            'DONE: Query DB
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM character_social WHERE guid = {0} AND flags = {1};", objCharacter.Guid, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)

            'DONE: Add to list
            For Each r As DataRow In q.Rows
                objCharacter.IgnoreList.Add(r.Item("friend"))
            Next
        End Sub

        Public Sub SendFriendList(ByRef client As ClientClass, ByRef character As CharacterObject)
            'DONE: Query DB
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM character_social WHERE guid = {0} AND (flags & {1}) > 0;", character.Guid, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Integer)), q)

            'DONE: Make the packet
            Dim smsgFriendList As New PacketClass(OPCODES.SMSG_FRIEND_LIST)
            If q.Rows.Count > 0 Then
                smsgFriendList.AddInt8(q.Rows.Count)

                For Each r As DataRow In q.Rows
                    Dim guid As ULong = r.Item("friend")
                    smsgFriendList.AddUInt64(guid)                    'Player GUID
                    If CHARACTERs.ContainsKey(guid) AndAlso CHARACTERs(guid).IsInWorld Then
                        If CHARACTERs(guid).DND Then
                            smsgFriendList.AddInt8(FriendStatus.FRIEND_STATUS_DND)
                        ElseIf CHARACTERs(guid).AFK Then
                            smsgFriendList.AddInt8(FriendStatus.FRIEND_STATUS_AFK)
                        Else
                            smsgFriendList.AddInt8(FriendStatus.FRIEND_STATUS_ONLINE)
                        End If
                        smsgFriendList.AddInt32(CHARACTERs(guid).Zone)    'Area
                        smsgFriendList.AddInt32(CHARACTERs(guid).Level)   'Level
                        smsgFriendList.AddInt32(CHARACTERs(guid).Classe)  'Class
                    Else
                        smsgFriendList.AddInt8(FriendStatus.FRIEND_STATUS_OFFLINE)
                    End If
                Next
            Else
                smsgFriendList.AddInt8(0)
            End If

            client.Send(smsgFriendList)
            smsgFriendList.Dispose()

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_FRIEND_LIST", client.IP, client.Port)
        End Sub

        Public Sub SendIgnoreList(ByRef client As ClientClass, ByRef character As CharacterObject)
            'DONE: Query DB
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM character_social WHERE guid = {0} AND (flags & {1}) > 0;", character.Guid, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Integer)), q)

            'DONE: Make the packet
            Dim smsgIgnoreList As New PacketClass(OPCODES.SMSG_IGNORE_LIST)
            If q.Rows.Count > 0 Then
                smsgIgnoreList.AddInt8(q.Rows.Count)

                For Each r As DataRow In q.Rows
                    smsgIgnoreList.AddUInt64(r.Item("friend"))                    'Player GUID
                Next
            Else
                smsgIgnoreList.AddInt8(0)
            End If

            client.Send(smsgIgnoreList)
            smsgIgnoreList.Dispose()

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_IGNORE_LIST", client.IP, client.Port)
        End Sub

        Public Sub NotifyFriendStatus(ByRef objCharacter As CharacterObject, s As FriendStatus)
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT guid FROM character_social WHERE friend = {0} AND (flags & {1}) > 0;", objCharacter.Guid, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Integer)), q)

            'DONE: Send "Friend offline/online"
            Dim friendpacket As New PacketClass(OPCODES.SMSG_FRIEND_STATUS)
            friendpacket.AddInt8(s)
            friendpacket.AddUInt64(objCharacter.Guid)
            For Each r As DataRow In q.Rows
                Dim guid As ULong = r.Item("guid")
                If CHARACTERs.ContainsKey(guid) AndAlso CHARACTERs(guid).Client IsNot Nothing Then
                    CHARACTERs(guid).Client.SendMultiplyPackets(friendpacket)
                End If
            Next
            friendpacket.Dispose()
        End Sub

#End Region

#Region "Handlers"

        Public Sub On_CMSG_WHO(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim levelMinimum As UInteger = packet.GetUInt32()       '0
            Dim levelMaximum As UInteger = packet.GetUInt32()       '100
            Dim namePlayer As String = EscapeString(packet.GetString())
            Dim nameGuild As String = EscapeString(packet.GetString())
            Dim maskRace As UInteger = packet.GetUInt32()
            Dim maskClass As UInteger = packet.GetUInt32()
            Dim zonesCount As UInteger = packet.GetUInt32()         'Limited to 10
            If zonesCount > 10 Then Exit Sub
            Dim zones As New List(Of UInteger)
            For i As Integer = 1 To zonesCount
                zones.Add(packet.GetUInt32)
            Next
            Dim stringsCount As UInteger = packet.GetUInt32         'Limited to 4
            If stringsCount > 4 Then Exit Sub
            Dim strings As New List(Of String)
            For i As Integer = 1 To stringsCount
                strings.Add(_CommonFunctions.UppercaseFirstLetter(EscapeString(packet.GetString())))
            Next

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_WHO [P:'{2}' G:'{3}' L:{4}-{5} C:{6:X} R:{7:X}]", client.IP, client.Port, namePlayer, nameGuild, levelMinimum, levelMaximum, maskClass, maskRace)

            'TODO: Don't show GMs?
            Dim results As New List(Of ULong)
            CHARACTERs_Lock.AcquireReaderLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
            For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If Not objCharacter.Value.IsInWorld Then Continue For
                If (GetCharacterSide(objCharacter.Value.Race) <> GetCharacterSide(client.Character.Race)) AndAlso client.Character.Access < AccessLevel.GameMaster Then Continue For
                If namePlayer <> "" AndAlso _CommonFunctions.UppercaseFirstLetter(objCharacter.Value.Name).IndexOf(_CommonFunctions.UppercaseFirstLetter(namePlayer), StringComparison.Ordinal) = -1 Then Continue For
                If nameGuild <> "" AndAlso (objCharacter.Value.Guild Is Nothing OrElse _CommonFunctions.UppercaseFirstLetter(objCharacter.Value.Guild.Name).IndexOf(_CommonFunctions.UppercaseFirstLetter(nameGuild), StringComparison.Ordinal) = -1) Then Continue For
                If objCharacter.Value.Level < levelMinimum Then Continue For
                If objCharacter.Value.Level > levelMaximum Then Continue For
                If zonesCount > 0 AndAlso zones.Contains(objCharacter.Value.Zone) = False Then Continue For
                If stringsCount > 0 Then
                    Dim passedStrings As Boolean = True
                    For Each stringValue As String In strings
                        If _CommonFunctions.UppercaseFirstLetter(objCharacter.Value.Name).IndexOf(stringValue, StringComparison.Ordinal) <> -1 Then Continue For
                        If _CommonFunctions.UppercaseFirstLetter(GetRaceName(objCharacter.Value.Race)) = stringValue Then Continue For
                        If _CommonFunctions.UppercaseFirstLetter(GetClassName(objCharacter.Value.Classe)) = stringValue Then Continue For
                        If objCharacter.Value.Guild IsNot Nothing AndAlso _CommonFunctions.UppercaseFirstLetter(objCharacter.Value.Guild.Name).IndexOf(stringValue, StringComparison.Ordinal) <> -1 Then Continue For
                        'TODO: Look for zone name
                        passedStrings = False
                        Exit For
                    Next
                    If passedStrings = False Then Continue For
                End If

                'DONE: List first 49 characters (like original)
                If results.Count > 49 Then Exit For

                results.Add(objCharacter.Value.Guid)
            Next

            Dim response As New PacketClass(OPCODES.SMSG_WHO)
            response.AddInt32(results.Count)
            response.AddInt32(results.Count)

            For Each guid As ULong In results
                response.AddString(CHARACTERs(guid).Name)           'Name
                If CHARACTERs(guid).Guild IsNot Nothing Then
                    response.AddString(CHARACTERs(guid).Guild.Name) 'Guild Name
                Else
                    response.AddString("")                          'Guild Name
                End If
                response.AddInt32(CHARACTERs(guid).Level)           'Level
                response.AddInt32(CHARACTERs(guid).Classe)          'Class
                response.AddInt32(CHARACTERs(guid).Race)            'Race
                response.AddInt32(CHARACTERs(guid).Zone)            'Zone ID
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
            Dim guid As ULong = 0
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ADD_FRIEND [{2}]", client.IP, client.Port, name)

            'DONE: Get GUID from DB
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT char_guid, char_race FROM characters WHERE char_name = ""{0}"";", name), q)

            If q.Rows.Count > 0 Then
                guid = CType(q.Rows(0).Item("char_guid"), Long)
                Dim FriendSide As Boolean = GetCharacterSide(q.Rows(0).Item("char_race"))

                q.Clear()
                CharacterDatabase.Query(String.Format("SELECT flags FROM character_social WHERE flags = {0}", CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)), q)
                Dim NumberOfFriends As Integer = q.Rows.Count
                q.Clear()
                CharacterDatabase.Query(String.Format("SELECT flags FROM character_social WHERE guid = {0} AND friend = {1} AND flags = {2};", client.Character.Guid, guid, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)), q)

                If guid = client.Character.Guid Then
                    response.AddInt8(FriendResult.FRIEND_SELF)
                    response.AddUInt64(guid)
                ElseIf q.Rows.Count > 0 Then
                    response.AddInt8(FriendResult.FRIEND_ALREADY)
                    response.AddUInt64(guid)
                ElseIf NumberOfFriends >= SocialList.MAX_FRIENDS_ON_LIST Then
                    response.AddInt8(FriendResult.FRIEND_LIST_FULL)
                    response.AddUInt64(guid)
                ElseIf GetCharacterSide(client.Character.Race) <> FriendSide Then
                    response.AddInt8(FriendResult.FRIEND_ENEMY)
                    response.AddUInt64(guid)
                ElseIf CHARACTERs.ContainsKey(guid) Then
                    response.AddInt8(FriendResult.FRIEND_ADDED_ONLINE)
                    response.AddUInt64(guid)
                    response.AddString(name)
                    If CHARACTERs(guid).DND Then
                        response.AddInt8(FriendStatus.FRIEND_STATUS_DND)
                    ElseIf CHARACTERs(guid).AFK Then
                        response.AddInt8(FriendStatus.FRIEND_STATUS_AFK)
                    Else
                        response.AddInt8(FriendStatus.FRIEND_STATUS_ONLINE)
                    End If
                    response.AddInt32(CHARACTERs(guid).Zone)
                    response.AddInt32(CHARACTERs(guid).Level)
                    response.AddInt32(CHARACTERs(guid).Classe)
                    CharacterDatabase.Update(String.Format("INSERT INTO character_social (guid, friend, flags) VALUES ({0}, {1}, {2});", client.Character.Guid, guid, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)))
                Else
                    response.AddInt8(FriendResult.FRIEND_ADDED_OFFLINE)
                    response.AddUInt64(guid)
                    response.AddString(name)
                    CharacterDatabase.Update(String.Format("INSERT INTO character_social (guid, friend, flags) VALUES ({0}, {1}, {2});", client.Character.Guid, guid, CType(SocialFlag.SOCIAL_FLAG_FRIEND, Byte)))
                End If
            Else
                response.AddInt8(FriendResult.FRIEND_NOT_FOUND)
                response.AddUInt64(guid)
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
                CharacterDatabase.Query(String.Format("SELECT flags FROM character_social WHERE flags = {0}", CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)
                Dim NumberOfFriends As Integer = q.Rows.Count
                q.Clear()
                CharacterDatabase.Query(String.Format("SELECT * FROM character_social WHERE guid = {0} AND friend = {1} AND flags = {2};", client.Character.Guid, GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)), q)

                If GUID = client.Character.Guid Then
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

                    CharacterDatabase.Update(String.Format("INSERT INTO character_social (guid, friend, flags) VALUES ({0}, {1}, {2});", client.Character.Guid, GUID, CType(SocialFlag.SOCIAL_FLAG_IGNORED, Byte)))
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
                CharacterDatabase.Query(String.Format("SELECT flags FROM character_social WHERE guid = {0} AND friend = {1};", client.Character.Guid, GUID), q)

                If q.Rows.Count > 0 Then
                    Dim flags As Integer = q.Rows(0).Item("flags")
                    Dim newFlags As Integer = (flags And (Not SocialFlag.SOCIAL_FLAG_FRIEND))
                    If (newFlags And (SocialFlag.SOCIAL_FLAG_FRIEND Or SocialFlag.SOCIAL_FLAG_IGNORED)) = 0 Then
                        CharacterDatabase.Update(String.Format("DELETE FROM character_social WHERE friend = {1} AND guid = {0};", client.Character.Guid, GUID))
                    Else
                        CharacterDatabase.Update(String.Format("UPDATE character_social SET flags = {2} WHERE friend = {1} AND guid = {0};", client.Character.Guid, GUID, newFlags))
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
                CharacterDatabase.Query(String.Format("SELECT flags FROM character_social WHERE guid = {0} AND friend = {1};", client.Character.Guid, GUID), q)

                If q.Rows.Count > 0 Then
                    Dim flags As Integer = q.Rows(0).Item("flags")
                    Dim newFlags As Integer = (flags And (Not SocialFlag.SOCIAL_FLAG_IGNORED))
                    If (newFlags And (SocialFlag.SOCIAL_FLAG_FRIEND Or SocialFlag.SOCIAL_FLAG_IGNORED)) = 0 Then
                        CharacterDatabase.Update(String.Format("DELETE FROM character_social WHERE friend = {1} AND guid = {0};", client.Character.Guid, GUID))
                    Else
                        CharacterDatabase.Update(String.Format("UPDATE character_social SET flags = {2} WHERE friend = {1} AND guid = {0};", client.Character.Guid, GUID, newFlags))
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
            SendFriendList(client, client.Character)
            SendIgnoreList(client, client.Character)
        End Sub

#End Region

    End Module
End Namespace