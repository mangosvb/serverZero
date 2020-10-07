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
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Guild
Imports Mangos.Common

Namespace Handlers

    Public Class WC_Handlers_Guild

        Public Sub On_CMSG_GUILD_QUERY(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim guildId As UInteger = packet.GetUInt32

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_QUERY [{2}]", client.IP, client.Port, guildId)

            _WC_Guild.SendGuildQuery(client, guildId)
        End Sub

        Public Sub On_CMSG_GUILD_ROSTER(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            'packet.GetInt16()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_ROSTER", client.IP, client.Port)

            _WC_Guild.SendGuildRoster(client.Character)
        End Sub

        Public Sub On_CMSG_GUILD_CREATE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim guildName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_CREATE [{2}]", client.IP, client.Port, guildName)

            If client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_ALREADY_IN_GUILD)
                Exit Sub
            End If

            'DONE: Create guild data
            Dim MySQLQuery As New DataTable
            _WorldCluster.CharacterDatabase.Query(String.Format("INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES (""{0}"", {1}, {2}, {3}, {4}); SELECT guild_id FROM guilds WHERE guild_name = ""{0}"";", guildName, client.Character.Guid, Now.Year - 2006, Now.Month, Now.Day), MySQLQuery)

            _WC_Guild.AddCharacterToGuild(client.Character, MySQLQuery.Rows(0).Item("guild_id"), 0)
        End Sub

        Public Sub On_CMSG_GUILD_INFO(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_INFO", client.IP, client.Port)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            End If

            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_INFO)
            response.AddString(client.Character.Guild.Name)
            response.AddInt32(client.Character.Guild.cDay)
            response.AddInt32(client.Character.Guild.cMonth)
            response.AddInt32(client.Character.Guild.cYear)
            response.AddInt32(0)
            response.AddInt32(0)
            client.Send(response)
            response.Dispose()
        End Sub

        Public Sub On_CMSG_GUILD_RANK(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 14 Then Exit Sub
            packet.GetInt16()
            Dim rankID As Integer = packet.GetInt32
            Dim rankRights As UInteger = packet.GetUInt32
            Dim rankName As String = packet.GetString.Replace("""", "_").Replace("'", "_")

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_RANK [{2}:{3}:{4}]", client.IP, client.Port, rankID, rankRights, rankName)
            If rankID < 0 OrElse rankID > 9 Then Exit Sub

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            client.Character.Guild.Ranks(rankID) = rankName
            client.Character.Guild.RankRights(rankID) = rankRights

            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_rank{1} = ""{2}"", guild_rank{1}_Rights = {3} WHERE guild_id = {0};", client.Character.Guild.ID, rankID, rankName, rankRights))

            _WC_Guild.SendGuildQuery(client, client.Character.Guild.ID)
            _WC_Guild.SendGuildRoster(client.Character)
        End Sub

        Public Sub On_CMSG_GUILD_ADD_RANK(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim NewRankName As String = packet.GetString().Replace("""", "_").Replace("'", "_")

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_ADD_RANK [{2}]", client.IP, client.Port, NewRankName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            ElseIf _Functions.ValidateGuildName(NewRankName) = False Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_INTERNAL)
                Exit Sub
            End If

            For i As Integer = 0 To 9
                If client.Character.Guild.Ranks(i) = "" Then
                    client.Character.Guild.Ranks(i) = NewRankName
                    client.Character.Guild.RankRights(i) = GuildRankRights.GR_RIGHT_GCHATLISTEN Or GuildRankRights.GR_RIGHT_GCHATSPEAK
                    _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_rank{1} = '{2}', guild_rank{1}_Rights = '{3}' WHERE guild_id = {0};", client.Character.Guild.ID, i, NewRankName, client.Character.Guild.RankRights(i)))

                    _WC_Guild.SendGuildQuery(client, client.Character.Guild.ID)
                    _WC_Guild.SendGuildRoster(client.Character)
                    Exit Sub
                End If
            Next

            _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_INTERNAL)
        End Sub

        Public Sub On_CMSG_GUILD_DEL_RANK(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_DEL_RANK", client.IP, client.Port)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'TODO: Check if someone in the guild is the rank we're removing?
            'TODO: Can we really remove all ranks?
            For i As Integer = 9 To 0 Step -1
                If client.Character.Guild.Ranks(i) <> "" Then
                    _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_rank{1} = '{2}', guild_rank{1}_Rights = '{3}' WHERE guild_id = {0};", client.Character.Guild.ID, i, "", 0))

                    _WC_Guild.SendGuildQuery(client, client.Character.Guild.ID)
                    _WC_Guild.SendGuildRoster(client.Character)
                    Exit Sub
                End If
            Next

            _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_INTERNAL)
        End Sub

        Public Sub On_CMSG_GUILD_LEADER(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_LEADER [{2}]", client.IP, client.Port, playerName)
            If playerName.Length < 2 Then Exit Sub
            playerName = _Functions.CapitalizeName(playerName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Find new leader's GUID
            Dim MySQLQuery As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid, char_guildId, char_guildrank FROM characters WHERE char_name = '" & playerName & "';", MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            ElseIf CUInt(MySQLQuery.Rows(0).Item("char_guildId")) <> client.Character.Guild.ID Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD_S, playerName)
                Exit Sub
            End If
            Dim PlayerGUID As ULong = MySQLQuery.Rows(0).Item("char_guid")

            client.Character.GuildRank = 1 'Officer
            client.Character.SendGuildUpdate()
            If _WorldCluster.CHARACTERs.ContainsKey(PlayerGUID) Then
                _WorldCluster.CHARACTERs(PlayerGUID).GuildRank = 0
                _WorldCluster.CHARACTERs(PlayerGUID).SendGuildUpdate()
            End If
            client.Character.Guild.Leader = PlayerGUID
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_leader = ""{1}"" WHERE guild_id = {0};", client.Character.Guild.ID, PlayerGUID))
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildRank = {0} WHERE char_guid = {1};", 0, PlayerGUID))
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildRank = {0} WHERE char_guid = {1};", client.Character.GuildRank, client.Character.Guid))

            'DONE: Send notify message
            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.LEADER_CHANGED)
            response.AddInt8(2)
            response.AddString(client.Character.Name)
            response.AddString(playerName)
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()
        End Sub

        Public Sub On_MSG_SAVE_GUILD_EMBLEM(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If packet.Data.Length < 34 Then Exit Sub
            packet.GetInt16()
            Dim unk0 As Integer = packet.GetInt32
            Dim unk1 As Integer = packet.GetInt32
            Dim tEmblemStyle As Integer = packet.GetInt32
            Dim tEmblemColor As Integer = packet.GetInt32
            Dim tBorderStyle As Integer = packet.GetInt32
            Dim tBorderColor As Integer = packet.GetInt32
            Dim tBackgroundColor As Integer = packet.GetInt32

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_SAVE_GUILD_EMBLEM [{2},{3}] [{4}:{5}:{6}:{7}:{8}]", client.IP, client.Port, unk0, unk1, tEmblemStyle, tEmblemColor, tBorderStyle, tBorderColor, tBackgroundColor)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub

                'TODO: Check if you have enough money
                'ElseIf client.Character.Copper < 100000 Then
                '    SendInventoryChangeFailure(Client.Character, InventoryChangeFailure.EQUIP_ERR_NOT_ENOUGH_MONEY, 0, 0)
                '    Exit Sub
            End If

            client.Character.Guild.EmblemStyle = tEmblemStyle
            client.Character.Guild.EmblemColor = tEmblemColor
            client.Character.Guild.BorderStyle = tBorderStyle
            client.Character.Guild.BorderColor = tBorderColor
            client.Character.Guild.BackgroundColor = tBackgroundColor

            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_tEmblemStyle = {1}, guild_tEmblemColor = {2}, guild_tBorderStyle = {3}, guild_tBorderColor = {4}, guild_tBackgroundColor = {5} WHERE guild_id = {0};", client.Character.Guild.ID, tEmblemStyle, tEmblemColor, tBorderStyle, tBorderColor, tBackgroundColor))

            _WC_Guild.SendGuildQuery(client, client.Character.Guild.ID)

            Dim packet_event As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            packet_event.AddInt8(GuildEvent.TABARDCHANGE)
            packet_event.AddInt32(client.Character.Guild.ID)
            _WC_Guild.BroadcastToGuild(packet_event, client.Character.Guild)
            packet_event.Dispose()

            'TODO: This tabard design costs 10g!
            'Client.Character.Copper -= 100000
            'Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
            'Client.Character.SendCharacterUpdate(False)
        End Sub

        Public Sub On_CMSG_GUILD_DISBAND(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            'packet.GetInt16()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_DISBAND", client.IP, client.Port)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Clear all members
            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.DISBANDED)
            response.AddInt8(0)

            Dim GuildID As Integer = client.Character.Guild.ID

            Dim tmpArray() As ULong = client.Character.Guild.Members.ToArray
            For Each Member As ULong In tmpArray
                If _WorldCluster.CHARACTERs.ContainsKey(Member) Then
                    _WC_Guild.RemoveCharacterFromGuild(_WorldCluster.CHARACTERs(Member))
                    _WorldCluster.CHARACTERs(Member).Client.SendMultiplyPackets(response)
                Else
                    _WC_Guild.RemoveCharacterFromGuild(Member)
                End If
            Next

            _WC_Guild.GUILDs(GuildID).Dispose()

            response.Dispose()

            'DONE: Delete guild information
            _WorldCluster.CharacterDatabase.Update("DELETE FROM guilds WHERE guild_id = " & GuildID & ";")
        End Sub

        Public Sub On_CMSG_GUILD_MOTD(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            'Isn't the client even sending a null terminator for the motd if it's empty?
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim Motd As String = ""
            If packet.Length <> 4 Then Motd = packet.GetString.Replace("""", "_").Replace("'", "_")

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_MOTD", client.IP, client.Port)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_SETMOTD) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            client.Character.Guild.Motd = Motd
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE guilds SET guild_MOTD = '{1}' WHERE guild_id = '{0}';", client.Character.Guild.ID, Motd))

            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.MOTD)
            response.AddInt8(1)
            response.AddString(Motd)

            'DONE: Send message to everyone in the guild
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)

            response.Dispose()
        End Sub

        Public Sub On_CMSG_GUILD_SET_OFFICER_NOTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = packet.GetString
            If (packet.Data.Length - 1) < (6 + playerName.Length + 1) Then Exit Sub
            Dim Note As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_SET_OFFICER_NOTE [{2}]", client.IP, client.Port, playerName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_EOFFNOTE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildOffNote = ""{1}"" WHERE char_name = ""{0}"";", playerName, Note.Replace("""", "_").Replace("'", "_")))

            _WC_Guild.SendGuildRoster(client.Character)
        End Sub

        Public Sub On_CMSG_GUILD_SET_PUBLIC_NOTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = packet.GetString
            If (packet.Data.Length - 1) < (6 + playerName.Length + 1) Then Exit Sub
            Dim Note As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_SET_PUBLIC_NOTE [{2}]", client.IP, client.Port, playerName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_EPNOTE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildPNote = ""{1}"" WHERE char_name = ""{0}"";", playerName, Note.Replace("""", "_").Replace("'", "_")))

            _WC_Guild.SendGuildRoster(client.Character)
        End Sub

        Public Sub On_CMSG_GUILD_REMOVE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = packet.GetString.Replace("'", "_").Replace("""", "_")

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_REMOVE [{2}]", client.IP, client.Port, playerName)
            If playerName.Length < 2 Then Exit Sub
            playerName = _Functions.CapitalizeName(playerName)

            'DONE: Player1 checks
            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_REMOVE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Find player2's guid
            Dim q As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_name = '" & playerName & "';", q)

            'DONE: Removed checks
            If q.Rows.Count = 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            ElseIf Not _WorldCluster.CHARACTERs.ContainsKey(q.Rows(0).Item("char_guid")) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            End If

            Dim objCharacter As CharacterObject = _WorldCluster.CHARACTERs(q.Rows(0).Item("char_guid"))

            If objCharacter.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_QUIT_S, GuildError.GUILD_LEADER_LEAVE)
                Exit Sub
            End If

            'DONE: Send guild event
            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.REMOVED)
            response.AddInt8(2)
            response.AddString(playerName)
            response.AddString(objCharacter.Name)
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()

            _WC_Guild.RemoveCharacterFromGuild(objCharacter)
        End Sub

        Public Sub On_CMSG_GUILD_PROMOTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_PROMOTE [{2}]", client.IP, client.Port, playerName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_PROMOTE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Find promoted player's guid
            Dim q As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_name = '" & playerName.Replace("'", "_") & "';", q)

            'DONE: Promoted checks
            If q.Rows.Count = 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_NAME_INVALID)
                Exit Sub
            ElseIf Not _WorldCluster.CHARACTERs.ContainsKey(q.Rows(0).Item("char_guid")) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            End If
            Dim objCharacter As CharacterObject = _WorldCluster.CHARACTERs(q.Rows(0).Item("char_guid"))
            If objCharacter.Guild.ID <> client.Character.Guild.ID Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD_S, playerName)
                Exit Sub
            ElseIf objCharacter.GuildRank <= client.Character.GuildRank Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            ElseIf objCharacter.GuildRank = _Global_Constants.GUILD_RANK_MIN Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_INTERNAL)
                Exit Sub
            End If

            'DONE: Do the real update
            objCharacter.GuildRank -= 1
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildRank = {0} WHERE char_guid = {1};", objCharacter.GuildRank, objCharacter.Guid))
            objCharacter.SendGuildUpdate()

            'DONE: Send event to guild
            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.PROMOTION)
            response.AddInt8(3)
            response.AddString(objCharacter.Name)
            response.AddString(playerName)
            response.AddString(client.Character.Guild.Ranks(objCharacter.GuildRank))
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()
        End Sub

        Public Sub On_CMSG_GUILD_DEMOTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_DEMOTE [{2}]", client.IP, client.Port, playerName)

            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_PROMOTE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Find demoted player's guid
            Dim q As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_name = '" & playerName.Replace("'", "_") & "';", q)

            'DONE: Demoted checks
            If q.Rows.Count = 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_NAME_INVALID)
                Exit Sub
            ElseIf Not _WorldCluster.CHARACTERs.ContainsKey(q.Rows(0).Item("char_guid")) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            End If

            Dim objCharacter As CharacterObject = _WorldCluster.CHARACTERs(q.Rows(0).Item("char_guid"))
            If objCharacter.Guild.ID <> client.Character.Guild.ID Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD_S, playerName)
                Exit Sub
            ElseIf objCharacter.GuildRank <= client.Character.GuildRank Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            ElseIf objCharacter.GuildRank = _Global_Constants.GUILD_RANK_MAX Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_INTERNAL)
                Exit Sub
            End If

            'DONE: Max defined rank check
            If Trim(client.Character.Guild.Ranks(objCharacter.GuildRank + 1)) = "" Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_INTERNAL)
                Exit Sub
            End If

            'DONE: Do the real update
            objCharacter.GuildRank += 1
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildRank = {0} WHERE char_guid = {1};", objCharacter.GuildRank, objCharacter.Guid))
            objCharacter.SendGuildUpdate()

            'DONE: Send event to guild
            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.DEMOTION)
            response.AddInt8(3)
            response.AddString(objCharacter.Name)
            response.AddString(playerName)
            response.AddString(client.Character.Guild.Ranks(objCharacter.GuildRank))
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()
        End Sub

        'User Options
        Public Sub On_CMSG_GUILD_INVITE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim playerName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_INVITE [{2}]", client.IP, client.Port, playerName)

            'DONE: Inviter checks
            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf Not client.Character.IsGuildRightSet(GuildRankRights.GR_RIGHT_INVITE) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PERMISSIONS)
                Exit Sub
            End If

            'DONE: Find invited player's guid
            Dim q As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT char_guid FROM characters WHERE char_name = '" & playerName.Replace("'", "_") & "';", q)

            'DONE: Invited checks
            If q.Rows.Count = 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_NAME_INVALID)
                Exit Sub
            ElseIf Not _WorldCluster.CHARACTERs.ContainsKey(q.Rows(0).Item("char_guid")) Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_PLAYER_NOT_FOUND, playerName)
                Exit Sub
            End If

            Dim objCharacter As CharacterObject = _WorldCluster.CHARACTERs(q.Rows(0).Item("char_guid"))
            If objCharacter.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.ALREADY_IN_GUILD, playerName)
                Exit Sub
            ElseIf objCharacter.Side <> client.Character.Side Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.GUILD_NOT_ALLIED, playerName)
                Exit Sub
            ElseIf objCharacter.GuildInvited <> 0 Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_INVITE_S, GuildError.ALREADY_INVITED_TO_GUILD, playerName)
                Exit Sub
            End If

            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_INVITE)
            response.AddString(client.Character.Name)
            response.AddString(client.Character.Guild.Name)
            objCharacter.Client.Send(response)
            response.Dispose()

            objCharacter.GuildInvited = client.Character.Guild.ID
            objCharacter.GuildInvitedBy = client.Character.Guid
        End Sub

        Public Sub On_CMSG_GUILD_ACCEPT(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If client.Character.GuildInvited = 0 Then Throw New ApplicationException("Character accepting guild invitation whihtout being invited.")

            _WC_Guild.AddCharacterToGuild(client.Character, client.Character.GuildInvited)
            client.Character.GuildInvited = 0

            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.JOINED)
            response.AddInt8(1)
            response.AddString(client.Character.Name)
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()

            _WC_Guild.SendGuildRoster(client.Character)
            _WC_Guild.SendGuildMOTD(client.Character)
        End Sub

        Public Sub On_CMSG_GUILD_DECLINE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            client.Character.GuildInvited = 0

            If _WorldCluster.CHARACTERs.ContainsKey(CType(client.Character.GuildInvitedBy, Long)) Then
                Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_DECLINE)
                response.AddString(client.Character.Name)
                _WorldCluster.CHARACTERs(CType(client.Character.GuildInvitedBy, Long)).Client.Send(response)
                response.Dispose()
            End If
        End Sub

        Public Sub On_CMSG_GUILD_LEAVE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            'packet.GetInt16()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GUILD_LEAVE", client.IP, client.Port)

            'DONE: Checks
            If Not client.Character.IsInGuild Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_PLAYER_NOT_IN_GUILD)
                Exit Sub
            ElseIf client.Character.IsGuildLeader Then
                _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_QUIT_S, GuildError.GUILD_LEADER_LEAVE)
                Exit Sub
            End If

            _WC_Guild.RemoveCharacterFromGuild(client.Character)
            _WC_Guild.SendGuildResult(client, GuildCommand.GUILD_QUIT_S, GuildError.GUILD_PLAYER_NO_MORE_IN_GUILD, client.Character.Name)

            Dim response As New Packets.PacketClass(OPCODES.SMSG_GUILD_EVENT)
            response.AddInt8(GuildEvent.LEFT)
            response.AddInt8(1)
            response.AddString(client.Character.Name)
            _WC_Guild.BroadcastToGuild(response, client.Character.Guild)
            response.Dispose()
        End Sub

        Public Sub On_CMSG_TURN_IN_PETITION(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim itemGuid As ULong = packet.GetUInt64

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TURN_IN_PETITION [GUID={2:X}]", client.IP, client.Port, itemGuid)

            'DONE: Get info
            Dim q As New DataTable
            _WorldCluster.CharacterDatabase.Query("SELECT * FROM petitions WHERE petition_itemGuid = " & itemGuid - _Global_Constants.GUID_ITEM & " LIMIT 1;", q)
            If q.Rows.Count = 0 Then Exit Sub
            Dim Type As Byte = q.Rows(0).Item("petition_type")
            Dim Name As String = q.Rows(0).Item("petition_name")

            'DONE: Check if already in guild
            If Type = 9 AndAlso client.Character.IsInGuild Then
                Dim response As New Packets.PacketClass(OPCODES.SMSG_TURN_IN_PETITION_RESULTS)
                response.AddInt32(PetitionTurnInError.PETITIONTURNIN_ALREADY_IN_GUILD)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            'DONE: Check required signs
            Dim RequiredSigns As Byte = 9
            If CType(q.Rows(0).Item("petition_signedMembers"), Integer) < RequiredSigns Then
                Dim response As New Packets.PacketClass(OPCODES.SMSG_TURN_IN_PETITION_RESULTS)
                response.AddInt32(PetitionTurnInError.PETITIONTURNIN_NEED_MORE_SIGNATURES)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            Dim q2 As New DataTable

            'DONE: Create guild and add members
            _WorldCluster.CharacterDatabase.Query(String.Format("INSERT INTO guilds (guild_name, guild_leader, guild_cYear, guild_cMonth, guild_cDay) VALUES ('{0}', {1}, {2}, {3}, {4}); SELECT guild_id FROM guilds WHERE guild_name = '{0}';", Name, client.Character.Guid, Now.Year - 2006, Now.Month, Now.Day), q2)

            _WC_Guild.AddCharacterToGuild(client.Character, q2.Rows(0).Item("guild_id"), 0)

            'DONE: Adding 9 more signed _WorldCluster.CHARACTERs
            For i As Byte = 1 To 9
                If _WorldCluster.CHARACTERs.ContainsKey(q.Rows(0).Item("petition_signedMember" & i)) Then
                    _WC_Guild.AddCharacterToGuild(_WorldCluster.CHARACTERs(q.Rows(0).Item("petition_signedMember" & i)), q2.Rows(0).Item("guild_id"))
                Else
                    _WC_Guild.AddCharacterToGuild(CType(q.Rows(0).Item("petition_signedMember" & i), ULong), q2.Rows(0).Item("guild_id"))
                End If
            Next

            'DONE: Delete guild charter item, on the world server
            client.Character.GetWorld.ClientPacket(client.Index, packet.Data)

            Dim success As New Packets.PacketClass(OPCODES.SMSG_TURN_IN_PETITION_RESULTS)
            success.AddInt32(0) 'Okay
            client.Send(success)
            success.Dispose()
        End Sub

    End Class
End Namespace