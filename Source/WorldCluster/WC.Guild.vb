'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
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

Imports mangosVB.Common
Imports mangosVB.Common.Globals
Imports WorldCluster.Globals
Imports WorldCluster.Handlers

Public Module WC_Guild

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

        Public Sub New(ByVal GuildID As UInteger)
            ID = GuildID

            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query("SELECT * FROM guilds WHERE guild_id = " & ID & ";", MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then Throw New ApplicationException("GuildID " & ID & " not found in database.")
            Dim GuildInfo As DataRow = MySQLQuery.Rows(0)

            Name = GuildInfo.Item("guild_name")
            Leader = GuildInfo.Item("guild_leader")
            Motd = GuildInfo.Item("guild_MOTD")
            EmblemStyle = GuildInfo.Item("guild_tEmblemStyle")
            EmblemColor = GuildInfo.Item("guild_tEmblemColor")
            BorderStyle = GuildInfo.Item("guild_tBorderStyle")
            BorderColor = GuildInfo.Item("guild_tBorderColor")
            BackgroundColor = GuildInfo.Item("guild_tBackgroundColor")

            cYear = GuildInfo.Item("guild_cYear")
            cMonth = GuildInfo.Item("guild_cMonth")
            cDay = GuildInfo.Item("guild_cDay")

            For i As Integer = 0 To 9
                Ranks(i) = GuildInfo.Item("guild_rank" & i)
                RankRights(i) = GuildInfo.Item("guild_rank" & i & "_Rights")
            Next

            MySQLQuery.Clear()
            CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_guildId = " & ID & ";", MySQLQuery)
            For Each MemberInfo As DataRow In MySQLQuery.Rows
                Members.Add(MemberInfo.Item("char_guid"))
            Next

            GUILDs.Add(ID, Me)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                GUILDs.Remove(ID)
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
    Public Sub AddCharacterToGuild(ByRef objCharacter As WcHandlerCharacter.CharacterObject, ByVal GuildID As Integer, Optional ByVal GuildRank As Integer = 4)
        CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = {2}, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", GuildID, objCharacter.GUID, GuildRank))

        If GUILDs.ContainsKey(GuildID) = False Then
            Dim tmpGuild As New Guild(GuildID)
        End If

        objCharacter.Guild = GUILDs(GuildID)
        objCharacter.Guild.Members.Add(objCharacter.GUID)
        objCharacter.GuildRank = GuildRank

        objCharacter.SendGuildUpdate()
    End Sub

    Public Sub AddCharacterToGuild(ByVal GUID As ULong, ByVal GuildID As Integer, Optional ByVal GuildRank As Integer = 4)
        CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = {2}, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", GuildID, GUID, GuildRank))
    End Sub

    Public Sub RemoveCharacterFromGuild(ByRef objCharacter As CharacterObject)
        CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = 0, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", 0, objCharacter.GUID))

        objCharacter.Guild.Members.Remove(objCharacter.GUID)
        objCharacter.Guild = Nothing
        objCharacter.GuildRank = 0
        objCharacter.SendGuildUpdate()
    End Sub

    Public Sub RemoveCharacterFromGuild(ByVal GUID As ULong)
        CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildId = {0}, char_guildRank = 0, char_guildOffNote = '', char_guildPNote = '' WHERE char_guid = {1};", 0, GUID))
    End Sub

    Public Sub BroadcastChatMessageGuild(ByRef Sender As CharacterObject, ByVal Message As String, ByVal Language As LANGUAGES, ByVal GuildID As Integer)
        'DONE: Check for guild member
        If Not Sender.IsInGuild Then
            SendGuildResult(Sender.client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
            Exit Sub
        End If

        'DONE: Check for rights to speak
        If Not Sender.IsGuildRightSet(GuildRankRights.GR_RIGHT_GCHATSPEAK) Then
            SendGuildResult(Sender.client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
            Exit Sub
        End If

        'DONE: Build packet
        Dim packet As Packets.PacketClass = BuildChatMessage(Sender.GUID, Message, ChatMsg.CHAT_MSG_GUILD, Language, Sender.ChatFlag)

        'DONE: Send message to everyone
        Dim tmpArray() As ULong = Sender.Guild.Members.ToArray
        For Each Member As ULong In tmpArray
            If CHARACTERs.ContainsKey(Member) Then
                If CHARACTERs(Member).IsGuildRightSet(GuildRankRights.GR_RIGHT_GCHATLISTEN) Then
                    CHARACTERs(Member).client.SendMultiplyPackets(packet)
                End If
            End If
        Next

        packet.Dispose()
    End Sub

    Public Sub BroadcastChatMessageOfficer(ByRef Sender As CharacterObject, ByVal Message As String, ByVal Language As LANGUAGES, ByVal GuildID As Integer)
        'DONE: Check for guild member
        If Not Sender.IsInGuild Then
            SendGuildResult(Sender.client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
            Exit Sub
        End If

        'DONE: Check for rights to speak
        If Not Sender.IsGuildRightSet(GuildRankRights.GR_RIGHT_OFFCHATSPEAK) Then
            SendGuildResult(Sender.client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
            Exit Sub
        End If

        'DONE: Build packet
        Dim packet As PacketClass = BuildChatMessage(Sender.GUID, Message, ChatMsg.CHAT_MSG_OFFICER, Language, Sender.ChatFlag)

        'DONE: Send message to everyone
        Dim tmpArray() As ULong = Sender.Guild.Members.ToArray
        For Each Member As ULong In tmpArray
            If CHARACTERs.ContainsKey(Member) Then
                If CHARACTERs(Member).IsGuildRightSet(GuildRankRights.GR_RIGHT_OFFCHATLISTEN) Then
                    CHARACTERs(Member).client.SendMultiplyPackets(packet)
                End If
            End If
        Next

        packet.Dispose()
    End Sub

    Public Sub SendGuildQuery(ByRef client As ClientClass, ByVal GuildID As UInteger)
        If GuildID = 0 Then Exit Sub
        'WARNING: This opcode is used also in character enum, so there must not be used any references to CharacterObject, only ClientClass

        'DONE: Load the guild if it doesn't exist in the memory
        If GUILDs.ContainsKey(GuildID) = False Then
            Dim tmpGuild As New Guild(GuildID)
        End If

        Dim response As New PacketClass(OPCODES.SMSG_GUILD_QUERY_RESPONSE)
        response.AddUInt32(GuildID)
        response.AddString(GUILDs(GuildID).Name)
        For i As Integer = 0 To 9
            response.AddString(GUILDs(GuildID).Ranks(i))
        Next
        response.AddInt32(GUILDs(GuildID).EmblemStyle)
        response.AddInt32(GUILDs(GuildID).EmblemColor)
        response.AddInt32(GUILDs(GuildID).BorderStyle)
        response.AddInt32(GUILDs(GuildID).BorderColor)
        response.AddInt32(GUILDs(GuildID).BackgroundColor)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub SendGuildRoster(ByRef objCharacter As CharacterObject)
        If Not objCharacter.IsInGuild Then Exit Sub

        'DONE: Count the ranks
        Dim guildRanksCount As Byte = 0
        For i As Integer = 0 To 9
            If objCharacter.Guild.Ranks(i) <> "" Then guildRanksCount += 1
        Next

        'DONE: Count the members
        Dim Members As New DataTable
        CharacterDatabase.Query("SELECT char_online, char_guid, char_name, char_class, char_level, char_zone_id, char_logouttime, char_guildRank, char_guildPNote, char_guildOffNote FROM characters WHERE char_guildId = " & objCharacter.Guild.ID & ";", Members)

        Dim response As New PacketClass(OPCODES.SMSG_GUILD_ROSTER)
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
                Dim DaysOffline As Single = (GetTimestamp(Now) - CUInt(Members.Rows(i).Item("char_logouttime"))) / DateInterval.Day
                response.AddSingle(DaysOffline) 'Days offline
                response.AddString(Members.Rows(i).Item("char_guildPNote"))
                If Officer Then
                    response.AddString(Members.Rows(i).Item("char_guildOffNote"))
                Else
                    response.AddInt8(0)
                End If
            End If
        Next

        objCharacter.client.Send(response)
        response.Dispose()
    End Sub

    Public Sub SendGuildResult(ByRef client As ClientClass, ByVal Command As GuildCommand, ByVal Result As GuildError, Optional ByVal Text As String = "")
        Dim response As New PacketClass(OPCODES.SMSG_GUILD_COMMAND_RESULT)
        response.AddInt32(Command)
        response.AddString(Text)
        response.AddInt32(Result)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub NotifyGuildStatus(ByRef objCharacter As CharacterObject, ByVal Status As GuildEvent)
        If objCharacter.Guild Is Nothing Then Exit Sub

        Dim statuspacket As New PacketClass(OPCODES.SMSG_GUILD_EVENT)
        statuspacket.AddInt8(Status)
        statuspacket.AddInt8(1)
        statuspacket.AddString(objCharacter.Name)
        statuspacket.AddInt8(0)
        statuspacket.AddInt8(0)
        statuspacket.AddInt8(0)
        BroadcastToGuild(statuspacket, objCharacter.Guild, objCharacter.GUID)
        statuspacket.Dispose()
    End Sub

    Public Sub BroadcastToGuild(ByRef Packet As PacketClass, ByRef Guild As Guild, Optional ByRef NotTo As ULong = 0)
        Dim tmpArray() As ULong = Guild.Members.ToArray()
        For Each Member As ULong In tmpArray
            If Member = NotTo Then Continue For
            If CHARACTERs.ContainsKey(Member) Then
                CHARACTERs(Member).client.SendMultiplyPackets(Packet)
            End If
        Next
    End Sub

    'Members Options
    Public Sub SendGuildMOTD(ByRef objCharacter As CharacterObject)
        If objCharacter.IsInGuild Then
            If objCharacter.Guild.Motd <> "" Then
                Dim response As New PacketClass(OPCODES.SMSG_GUILD_EVENT)
                response.AddInt8(GuildEvent.MOTD)
                response.AddInt8(1)
                response.AddString(objCharacter.Guild.Motd)
                objCharacter.client.Send(response)
                response.Dispose()
            End If
        End If
    End Sub

End Module