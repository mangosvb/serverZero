'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Handlers
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums.Chat
Imports Mangos.Common.Enums.Guild
Imports Mangos.Common.Enums.Misc

Public Class WC_Guild

#Region "WC.Guild.Guild"
    Public GUILDs As New Dictionary(Of UInteger, Guild)
    Public Class Guild
        Implements IDisposable

        Public ID As UInteger
        Public Name As String
        Public Leader As ULong
        Public Motd As String
        Public Info As String
        Public Members As New List(Of ULong)
        Public Ranks(9) As String
        Public RankRights(9) As UInteger
        Public EmblemStyle As Byte
        Public EmblemColor As Byte
        Public BorderStyle As Byte
        Public BorderColor As Byte
        Public BackgroundColor As Byte

        Public cYear As Short
        Public cMonth As Byte
        Public cDay As Byte

        Public Sub New(guildId As UInteger)
            ID = guildId

            Dim mySqlQuery As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT * FROM guilds WHERE guild_id = " & ID & ";", mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then Throw New ApplicationException("GuildID " & ID & " not found in database.")
            Dim guildInfo As DataRow = mySqlQuery.Rows(0)

            Name = guildInfo.Item("guild_name")
            Leader = guildInfo.Item("guild_leader")
            Motd = guildInfo.Item("guild_MOTD")
            EmblemStyle = guildInfo.Item("guild_tEmblemStyle")
            EmblemColor = guildInfo.Item("guild_tEmblemColor")
            BorderStyle = guildInfo.Item("guild_tBorderStyle")
            BorderColor = guildInfo.Item("guild_tBorderColor")
            BackgroundColor = guildInfo.Item("guild_tBackgroundColor")

            cYear = guildInfo.Item("guild_cYear")
            cMonth = guildInfo.Item("guild_cMonth")
            cDay = guildInfo.Item("guild_cDay")

            For i As Integer = 0 To 9
                Ranks(i) = guildInfo.Item("guild_rank" & i)
                RankRights(i) = guildInfo.Item("guild_rank" & i & "_Rights")
            Next

            mySqlQuery.Clear()
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_guildId = " & ID & ";", mySqlQuery)
            For Each memberInfo As DataRow In mySqlQuery.Rows
                Members.Add(memberInfo.Item("char_guid"))
            Next

            _WC_Guild.GUILDs.Add(ID, Me)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                _WC_Guild.GUILDs.Remove(ID)
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
    End Class

#End Region

    'Basic Guild Framework
    Public Sub AddCharacterToGuild(ByRef objCharacter As WcHandlerCharacter.CharacterObject, guildId As Integer, Optional ByVal guildRank As Integer = 4)
        _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = {2}, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", guildId, objCharacter.Guid, guildRank))

        If GUILDs.ContainsKey(guildId) = False Then
            Dim tmpGuild As New Guild(guildId)
            GUILDs.Add(guildId, tmpGuild)
        End If

        objCharacter.Guild = GUILDs(guildId)
        objCharacter.Guild.Members.Add(objCharacter.Guid)
        objCharacter.GuildRank = guildRank

        objCharacter.SendGuildUpdate()
    End Sub

    Public Sub AddCharacterToGuild(guid As ULong, guildId As Integer, Optional ByVal guildRank As Integer = 4)
        _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = {2}, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", guildId, guid, guildRank))
    End Sub

    Public Sub RemoveCharacterFromGuild(ByRef objCharacter As WcHandlerCharacter.CharacterObject)
        _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = 0, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", 0, objCharacter.Guid))

        objCharacter.Guild.Members.Remove(objCharacter.Guid)
        objCharacter.Guild = Nothing
        objCharacter.GuildRank = 0
        objCharacter.SendGuildUpdate()
    End Sub

    Public Sub RemoveCharacterFromGuild(guid As ULong)
        _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = 0, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", 0, guid))
    End Sub

    Public Sub BroadcastChatMessageGuild(ByRef sender As WcHandlerCharacter.CharacterObject, message As String, language As LANGUAGES, guildId As Integer)
        'DONE: Check for guild member
        If Not sender.IsInGuild Then
            SendGuildResult(sender.Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
            Exit Sub
        End If

        'DONE: Check for rights to speak
        If Not sender.IsGuildRightSet(GuildRankRights.GR_RIGHT_GCHATSPEAK) Then
            SendGuildResult(sender.Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
            Exit Sub
        End If

        'DONE: Build packet
        Dim packet As Packets.PacketClass = _Functions.BuildChatMessage(sender.Guid, message, ChatMsg.CHAT_MSG_GUILD, language, sender.ChatFlag)

        'DONE: Send message to everyone
        Dim tmpArray() As ULong = sender.Guild.Members.ToArray
        For Each member As ULong In tmpArray
            If _WorldCluster.CHARACTERs.ContainsKey(member) Then
                If _WorldCluster.CHARACTERs(member).IsGuildRightSet(GuildRankRights.GR_RIGHT_GCHATLISTEN) Then
                    _WorldCluster.CHARACTERs(member).Client.SendMultiplyPackets(packet)
                End If
            End If
        Next

        packet.Dispose()
    End Sub

    Public Sub BroadcastChatMessageOfficer(ByRef sender As WcHandlerCharacter.CharacterObject, message As String, language As LANGUAGES, guildId As Integer)
        'DONE: Check for guild member
        If Not sender.IsInGuild Then
            SendGuildResult(sender.Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
            Exit Sub
        End If

        'DONE: Check for rights to speak
        If Not sender.IsGuildRightSet(GuildRankRights.GR_RIGHT_OFFCHATSPEAK) Then
            SendGuildResult(sender.Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
            Exit Sub
        End If

        'DONE: Build packet
        Dim packet As Packets.PacketClass = _Functions.BuildChatMessage(sender.Guid, message, ChatMsg.CHAT_MSG_OFFICER, language, sender.ChatFlag)

        'DONE: Send message to everyone
        Dim tmpArray() As ULong = sender.Guild.Members.ToArray
        For Each member As ULong In tmpArray
            If _WorldCluster.CHARACTERs.ContainsKey(member) Then
                If _WorldCluster.CHARACTERs(member).IsGuildRightSet(GuildRankRights.GR_RIGHT_OFFCHATLISTEN) Then
                    _WorldCluster.CHARACTERs(member).Client.SendMultiplyPackets(packet)
                End If
            End If
        Next

        packet.Dispose()
    End Sub

    Public Sub SendGuildQuery(ByRef client As WC_Network.ClientClass, guildId As UInteger)
        If guildId = 0 Then Exit Sub
        'WARNING: This opcode is used also in character enum, so there must not be used any references to CharacterObject, only ClientClass

        'DONE: Load the guild if it doesn't exist in the memory
        If GUILDs.ContainsKey(guildId) = False Then
            Dim tmpGuild As New Guild(guildId)
            GUILDs.Add(guildId, tmpGuild)
        End If

        Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_QUERY_RESPONSE)
        response.AddUInt32(guildId)
        response.AddString(GUILDs(guildId).Name)
        For i As Integer = 0 To 9
            response.AddString(GUILDs(guildId).Ranks(i))
        Next
        response.AddInt32(GUILDs(guildId).EmblemStyle)
        response.AddInt32(GUILDs(guildId).EmblemColor)
        response.AddInt32(GUILDs(guildId).BorderStyle)
        response.AddInt32(GUILDs(guildId).BorderColor)
        response.AddInt32(GUILDs(guildId).BackgroundColor)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub SendGuildRoster(ByRef objCharacter As WcHandlerCharacter.CharacterObject)
        If Not objCharacter.IsInGuild Then Exit Sub

        'DONE: Count the ranks
        Dim guildRanksCount As Byte = 0
        For i As Integer = 0 To 9
            If objCharacter.Guild.Ranks(i) <> "" Then guildRanksCount += 1
        Next

        'DONE: Count the members
        Dim Members As New DataTable
        _WorldCluster.CharacterDatabase.Query("SELECT char_online, char_guid, char_name, char_class, char_level, char_zone_id, char_logouttime, char_guildRank, char_guildPNote, char_guildOffNote FROM characters WHERE char_guildId = " & objCharacter.Guild.ID & ";", Members)

        Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_ROSTER)
        response.AddInt32(Members.Rows.Count)
        response.AddString(objCharacter.Guild.Motd)
        response.AddString(objCharacter.Guild.Info)
        response.AddInt32(guildRanksCount)
        For i As Integer = 0 To 9
            If objCharacter.Guild.Ranks(i) <> "" Then
                response.AddUInt32(objCharacter.Guild.RankRights(i))
            End If
        Next

        Dim Officer As Boolean = objCharacter.IsGuildRightSet(GuildRankRights.GR_RIGHT_VIEWOFFNOTE)
        For i As Integer = 0 To Members.Rows.Count - 1
            If CByte(Members.Rows(i).Item("char_online")) = 1 Then
                response.AddUInt64(Members.Rows(i).Item("char_guid"))
                response.AddInt8(1)                         'OnlineFlag
                response.AddString(Members.Rows(i).Item("char_name"))
                response.AddInt32(Members.Rows(i).Item("char_guildRank"))
                response.AddInt8(Members.Rows(i).Item("char_level"))
                response.AddInt8(Members.Rows(i).Item("char_class"))
                response.AddInt32(Members.Rows(i).Item("char_zone_id"))
                response.AddString(Members.Rows(i).Item("char_guildPNote"))
                If Officer Then
                    response.AddString(Members.Rows(i).Item("char_guildOffNote"))
                Else
                    response.AddInt8(0)
                End If
            Else
                response.AddUInt64(Members.Rows(i).Item("char_guid"))
                response.AddInt8(0)                         'OfflineFlag
                response.AddString(Members.Rows(i).Item("char_name"))
                response.AddInt32(Members.Rows(i).Item("char_guildRank"))
                response.AddInt8(Members.Rows(i).Item("char_level"))
                response.AddInt8(Members.Rows(i).Item("char_class"))
                response.AddInt32(Members.Rows(i).Item("char_zone_id"))
                '0 = < 1 hour / 0.1 = 2.4 hours / 1 = 24 hours (1 day)
                '(Time logged out / 86400) = Days offline
                Dim DaysOffline As Single = (_Functions.GetTimestamp(Now) - CUInt(Members.Rows(i).Item("char_logouttime"))) / DateInterval.Day
                response.AddSingle(DaysOffline) 'Days offline
                response.AddString(Members.Rows(i).Item("char_guildPNote"))
                If Officer Then
                    response.AddString(Members.Rows(i).Item("char_guildOffNote"))
                Else
                    response.AddInt8(0)
                End If
            End If
        Next

        objCharacter.Client.Send(response)
        response.Dispose()
    End Sub

    Public Sub SendGuildResult(ByRef client As WC_Network.ClientClass, command As GuildCommand, result As GuildError, Optional ByVal text As String = "")
        Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_COMMAND_RESULT)
        response.AddInt32(command)
        response.AddString(text)
        response.AddInt32(result)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub NotifyGuildStatus(ByRef objCharacter As WcHandlerCharacter.CharacterObject, status As GuildEvent)
        If objCharacter.Guild Is Nothing Then Exit Sub

        Dim statuspacket As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
        statuspacket.AddInt8(status)
        statuspacket.AddInt8(1)
        statuspacket.AddString(objCharacter.Name)
        statuspacket.AddInt8(0)
        statuspacket.AddInt8(0)
        statuspacket.AddInt8(0)
        BroadcastToGuild(statuspacket, objCharacter.Guild, objCharacter.Guid)
        statuspacket.Dispose()
    End Sub

    Public Sub BroadcastToGuild(ByRef packet As Packets.PacketClass, ByRef guild As Guild, Optional ByRef notTo As ULong = 0)
        Dim tmpArray() As ULong = guild.Members.ToArray()
        For Each member As ULong In tmpArray
            If member = notTo Then Continue For
            If _WorldCluster.CHARACTERs.ContainsKey(member) Then
                _WorldCluster.CHARACTERs(member).Client.SendMultiplyPackets(packet)
            End If
        Next
    End Sub

    'Members Options
    Public Sub SendGuildMOTD(ByRef objCharacter As WcHandlerCharacter.CharacterObject)
        If objCharacter.IsInGuild Then
            If objCharacter.Guild.Motd <> "" Then
                Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
                response.AddInt8(GuildEvent.MOTD)
                response.AddInt8(1)
                response.AddString(objCharacter.Guild.Motd)
                objCharacter.Client.Send(response)
                response.Dispose()
            End If
        End If
    End Sub

End Class