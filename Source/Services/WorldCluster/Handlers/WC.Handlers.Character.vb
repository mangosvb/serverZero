'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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
Imports System.Data
Imports System.Reflection
Imports MangosVB.Common
Imports MangosVB.Common.Globals
Imports MangosVB.Shared
Imports WorldCluster.Globals
Imports WorldCluster.Server

Namespace Handlers

    Public Module WcHandlerCharacter

        Class CharacterObject
            Implements IDisposable

            Public Guid As ULong
            Public Client As ClientClass

            Public IsInWorld As Boolean = False
            Public Map As UInteger
            Public Zone As UInteger
            Public PositionX As Single
            Public PositionY As Single
            Public PositionZ As Single

            Public Access As AccessLevel
            Public Name As String
            Public Level As Integer
            Public Race As Races
            Public Classe As Classes
            Public Gender As Byte
            Public Time As Date = Now()
            Public Latency As Integer = 0

            Public IgnoreList As New List(Of ULong)
            Public JoinedChannels As New List(Of String)

            Public AFK As Boolean
            Public DND As Boolean
            Public AfkMessage As String

            Public GuildInvited As UInteger = 0
            Public GuildInvitedBy As ULong = 0
            Public Guild As Guild = Nothing
            Public GuildRank As Byte = 0

            Public Group As Group = Nothing
            Public GroupAssistant As Boolean = False
            Public GroupInvitedFlag As Boolean = False
            Public ReadOnly Property IsInGroup() As Boolean
                Get
                    Return (Group IsNot Nothing) AndAlso (GroupInvitedFlag = False)
                End Get
            End Property

            Public ReadOnly Property IsGroupLeader() As Boolean
                Get
                    If Group Is Nothing Then Return False
                    Return (Group.Members(Group.Leader) Is Me)
                End Get
            End Property

            Public ReadOnly Property IsInRaid() As Boolean
                Get
                    Return ((Group IsNot Nothing) AndAlso (Group.Type = GroupType.RAID))
                End Get
            End Property

            Public ReadOnly Property IsInGuild() As Boolean
                Get
                    Return (Guild IsNot Nothing)
                End Get
            End Property

            Public ReadOnly Property IsGuildLeader() As Boolean
                Get
                    Return ((Guild IsNot Nothing) AndAlso Guild.Leader = Guid)
                End Get
            End Property

            Public ReadOnly Property IsGuildRightSet(ByVal Rights As UInteger) As Boolean
                Get
                    Return ((Guild IsNot Nothing) AndAlso (Guild.RankRights(GuildRank) And Rights) = Rights)
                End Get
            End Property

            Public ReadOnly Property Side() As Boolean
                Get
                    Select Case Race
                        Case Races.RACE_DWARF, Races.RACE_GNOME, Races.RACE_HUMAN, Races.RACE_NIGHT_ELF
                            Return False
                        Case Else
                            Return True
                    End Select
                End Get
            End Property

            Public ReadOnly Property GetWorld() As IWorld
                Get
                    Return WorldServer.Worlds(Map)
                End Get
            End Property

            Public Sub ReLoad()
                'DONE: Get character info from DB
                Dim MySQLQuery As New DataTable
                CharacterDatabase.Query(String.Format("SELECT * FROM characters WHERE char_guid = {0};", Guid), MySQLQuery)
                If MySQLQuery.Rows.Count > 0 Then
                    Race = CType(MySQLQuery.Rows(0).Item("char_race"), Byte)
                    Classe = CType(MySQLQuery.Rows(0).Item("char_class"), Byte)
                    Gender = MySQLQuery.Rows(0).Item("char_gender")

                    Name = CType(MySQLQuery.Rows(0).Item("char_name"), String)
                    Level = CType(MySQLQuery.Rows(0).Item("char_level"), Byte)

                    Zone = MySQLQuery.Rows(0).Item("char_zone_id")
                    Map = MySQLQuery.Rows(0).Item("char_map_id")

                    PositionX = MySQLQuery.Rows(0).Item("char_positionX")
                    PositionY = MySQLQuery.Rows(0).Item("char_positionY")
                    PositionZ = MySQLQuery.Rows(0).Item("char_positionZ")

                    'DONE: Get guild info
                    Dim GuildID As UInteger = MySQLQuery.Rows(0).Item("char_guildId")
                    If GuildID > 0 Then
                        If GUILDs.ContainsKey(GuildID) = False Then
                            Dim tmpGuild As New Guild(GuildID)
                            Guild = tmpGuild
                        Else
                            Guild = GUILDs(GuildID)
                        End If
                        GuildRank = MySQLQuery.Rows(0).Item("char_guildRank")
                    End If
                Else
                    Log.WriteLine(LogType.DATABASE, "Failed to load expected results from:")
                    Log.WriteLine(LogType.DATABASE, String.Format("SELECT * FROM characters WHERE char_guid = {0};", Guid))
                End If

            End Sub

            Public Sub New(ByVal g As ULong, ByRef objCharacter As ClientClass)
                Guid = g
                Client = objCharacter

                ReLoad()
                Access = Client.Access

                LoadIgnoreList(Me)

                CHARACTERs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                CHARACTERs.Add(Guid, Me)
                CHARACTERs_Lock.ReleaseWriterLock()
            End Sub

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    Client = Nothing

                    'DONE: Update character status in database
                    CharacterDatabase.Update(String.Format("UPDATE characters SET char_online = 0, char_logouttime = '{1}' WHERE char_guid = '{0}';", Guid, GetTimestamp(Now)))

                    'NOTE: Don't leave group on normal disconnect, only on logout
                    If IsInGroup Then
                        'DONE: Tell the group the member is offline
                        Dim response As PacketClass = BuildPartyMemberStatsOffline(Guid)
                        Group.Broadcast(response)
                        response.Dispose()

                        'DONE: Set new leader and loot master
                        Group.NewLeader(Me)
                        Group.SendGroupList()
                    End If

                    'DONE: Notify friends for logout
                    NotifyFriendStatus(Me, FriendResult.FRIEND_OFFLINE)

                    'DONE: Notify guild for logout
                    If IsInGuild Then
                        NotifyGuildStatus(Me, GuildEvent.SIGNED_OFF)
                    End If

                    'DONE: Leave chat
                    While JoinedChannels.Count > 0
                        If CHAT_CHANNELs.ContainsKey(JoinedChannels(0)) Then
                            CHAT_CHANNELs(JoinedChannels(0)).Part(Me)
                        Else
                            JoinedChannels.RemoveAt(0)
                        End If
                    End While

                    CHARACTERs_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                    CHARACTERs.Remove(Guid)
                    CHARACTERs_Lock.ReleaseWriterLock()
                End If
                _disposedValue = True
            End Sub

            ' This code added by Visual Basic to correctly implement the disposable pattern.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

            Public Sub Transfer(ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single, ByVal thisMap As Integer)
                Dim p As New PacketClass(OPCODES.SMSG_TRANSFER_PENDING)
                p.AddInt32(thisMap)
                Client.Send(p)
                p.Dispose()

                'Actions Here
                IsInWorld = False
                GetWorld.ClientDisconnect(Client.Index)

                CharacterDatabase.Update(String.Format("UPDATE characters SET char_positionX = {0}, char_positionY = {1}, char_positionZ = {2}, char_orientation = {3}, char_map_id = {4} WHERE char_guid = {5};",
                                                       Trim(Str(posX)), Trim(Str(posY)), Trim(Str(posZ)), Trim(Str(ori)), thisMap, Guid))

                'Do global transfer
                WorldServer.ClientTransfer(Client.Index, posX, posY, posZ, ori, thisMap)
            End Sub

            Public Sub Transfer(ByVal posX As Single, ByVal posY As Single, ByVal posZ As Single, ByVal ori As Single)
                Dim p As New PacketClass(OPCODES.SMSG_TRANSFER_PENDING)
                p.AddInt32(Map)
                Client.Send(p)
                p.Dispose()

                'Actions Here
                IsInWorld = False
                GetWorld.ClientDisconnect(Client.Index)

                CharacterDatabase.Update(String.Format("UPDATE characters SET char_positionX = {0}, char_positionY = {1}, char_positionZ = {2}, char_orientation = {3}, char_map_id = {4} WHERE char_guid = {5};",
                                                       Trim(Str(posX)), Trim(Str(posY)), Trim(Str(posZ)), Trim(Str(ori)), Map, Guid))

                'Do global transfer
                WorldServer.ClientTransfer(Client.Index, posX, posY, posZ, ori, Map)
            End Sub
            'Login
            Public Sub OnLogin()
                'DONE: Update character status in database
                CharacterDatabase.Update("UPDATE characters SET char_online = 1 WHERE char_guid = " & Guid & ";")

                'DONE: SMSG_ACCOUNT_DATA_MD5
                SendAccountMD5(Client, Me)

                'DONE: SMSG_TRIGGER_CINEMATIC
                Dim q As New DataTable
                CharacterDatabase.Query(String.Format("SELECT char_moviePlayed FROM characters WHERE char_guid = {0} AND char_moviePlayed = 0;", Guid), q)
                If q.Rows.Count > 0 Then
                    CharacterDatabase.Update("UPDATE characters SET char_moviePlayed = 1 WHERE char_guid = " & Guid & ";")
                    SendTriggerCinematic(Client, Me)
                End If

                'DONE: SMSG_LOGIN_SETTIMESPEED
                SendGameTime(Client, Me)

                'DONE: Server Message Of The Day
                SendMessageMOTD(Client, "Welcome to World of Warcraft.")
                SendMessageMOTD(Client, String.Format("This server is using {0} v.{1}", SetColor("[mangosVB]", 255, 0, 0), [Assembly].GetExecutingAssembly().GetName().Version))

                'DONE: Guild Message Of The Day
                SendGuildMOTD(Me)

                'DONE: Social lists
                SendFriendList(Client, Me)
                SendIgnoreList(Client, Me)

                'DONE: Send "Friend online"
                NotifyFriendStatus(Me, FriendResult.FRIEND_ONLINE)

                'DONE: Send online notify for guild
                NotifyGuildStatus(Me, GuildEvent.SIGNED_ON)

                'DONE: Put back character in group if disconnected
                For Each tmpGroup As KeyValuePair(Of Long, Group) In GROUPs
                    For i As Byte = 0 To tmpGroup.Value.Members.Length - 1
                        If tmpGroup.Value.Members(i) IsNot Nothing AndAlso tmpGroup.Value.Members(i).Guid = Guid Then
                            tmpGroup.Value.Members(i) = Me
                            tmpGroup.Value.SendGroupList()

                            Dim response As New PacketClass(0) With {
                                    .Data = GetWorld.GroupMemberStats(Guid, 0)
                                    }
                            tmpGroup.Value.BroadcastToOther(response, Me)
                            response.Dispose()
                            Exit Sub
                        End If
                    Next
                Next
            End Sub

            Public Sub OnLogout()
                'DONE: Leave group
                If IsInGroup Then
                    Group.Leave(Me)
                End If

                'DONE: Leave chat
                While JoinedChannels.Count > 0
                    If CHAT_CHANNELs.ContainsKey(JoinedChannels(0)) Then
                        CHAT_CHANNELs(JoinedChannels(0)).Part(Me)
                    Else
                        JoinedChannels.RemoveAt(0)
                    End If
                End While
            End Sub

            Public Sub SendGuildUpdate()
                Dim GuildID As UInteger = 0
                If Guild IsNot Nothing Then GuildID = Guild.ID
                GetWorld.GuildUpdate(Guid, GuildID, GuildRank)
            End Sub

            'Chat
            Public ChatFlag As ChatFlag = ChatFlag.FLAGS_NONE

            Public Sub SendChatMessage(thisguid As ULong, message As String, msgType As ChatMsg, msgLanguage As Integer, channelName As String)
                If thisguid = 0 Then thisguid = Guid
                If channelName = "" Then channelName = "Global"
                Dim msgChatFlag As ChatFlag = ChatFlag
                If msgType = ChatMsg.CHAT_MSG_WHISPER_INFORM OrElse msgType = ChatMsg.CHAT_MSG_WHISPER Then msgChatFlag = CHARACTERs(thisguid).ChatFlag
                Dim packet As PacketClass = BuildChatMessage(thisguid, message, msgType, msgLanguage, msgChatFlag, channelName)
                Client.Send(packet)
                packet.Dispose()
            End Sub
        End Class

        Public Function GetCharacterGUIDByName(ByVal Name As String) As ULong
            Dim GUID As ULong = 0

            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            For Each objCharacter As KeyValuePair(Of ULong, CharacterObject) In CHARACTERs
                If UppercaseFirstLetter(objCharacter.Value.Name) = UppercaseFirstLetter(Name) Then
                    GUID = objCharacter.Value.Guid
                    Exit For
                End If
            Next
            CHARACTERs_Lock.ReleaseReaderLock()

            If GUID = 0 Then
                Dim q As New DataTable
                CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE char_name = ""{0}"";", EscapeString(Name)), q)

                If q.Rows.Count > 0 Then
                    Return q.Rows(0).Item("char_guid")
                Else
                    Return 0
                End If
            Else
                Return GUID
            End If
        End Function

        Public Function GetCharacterNameByGUID(ByVal GUID As String) As String
            If CHARACTERs.ContainsKey(GUID) Then
                Return CHARACTERs(GUID).Name
            Else
                Dim q As New DataTable
                CharacterDatabase.Query(String.Format("SELECT char_name FROM characters WHERE char_guid = ""{0}"";", GUID), q)

                If q.Rows.Count > 0 Then
                    Return CType(q.Rows(0).Item("char_name"), String)
                Else
                    Return ""
                End If
            End If
        End Function

    End Module
End Namespace