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

Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums.Chat
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Misc

Namespace Handlers

    Public Class WC_Handlers_Chat

        Public Sub On_CMSG_CHAT_IGNORED(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()

            Dim guid As ULong = packet.GetUInt64
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHAT_IGNORED [0x{2}]", client.IP, client.Port, guid)

            If _WorldCluster.CHARACTERs.ContainsKey(guid) Then
                Dim response As Packets.PacketClass = _Functions.BuildChatMessage(client.Character.Guid, "", ChatMsg.CHAT_MSG_IGNORED, LANGUAGES.LANG_UNIVERSAL, 0, "")
                _WorldCluster.CHARACTERs(guid).Client.Send(response)
                response.Dispose()
            End If
        End Sub

        Public Sub On_CMSG_MESSAGECHAT(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 14 Then Exit Sub
            packet.GetInt16()

            Dim msgType As ChatMsg = packet.GetInt32()
            Dim msgLanguage As LANGUAGES = packet.GetInt32()
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MESSAGECHAT [{2}:{3}]", client.IP, client.Port, msgType, msgLanguage)

            Select Case msgType

                Case ChatMsg.CHAT_MSG_CHANNEL
                    Dim channel As String = packet.GetString()
                    If (packet.Data.Length - 1) < (14 + channel.Length) Then Exit Sub
                    Dim message As String = packet.GetString()

                    'DONE: Broadcast to all
                    If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(channel) Then
                        _WS_Handler_Channels.CHAT_CHANNELs(channel).Say(message, msgLanguage, client.Character)
                    End If
                    Exit Sub

                Case ChatMsg.CHAT_MSG_WHISPER
                    Dim toUser As String = _Functions.CapitalizeName(packet.GetString())
                    If (packet.Data.Length - 1) < (14 + toUser.Length) Then Exit Sub
                    Dim message As String = packet.GetString()

                    'DONE: Handle admin/gm commands
                    'If ToUser = "Warden" AndAlso client.Character.Access > 0 Then
                    '    client.Character.GetWorld.ClientPacket(Client.Index, packet.Data)
                    '    Exit Sub
                    'End If

                    'DONE: Send whisper MSG to receiver
                    Dim guid As ULong = 0
                    _WorldCluster.CHARACTERs_Lock.AcquireReaderLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                    For Each character As KeyValuePair(Of ULong, WcHandlerCharacter.CharacterObject) In _WorldCluster.CHARACTERs
                        If _CommonFunctions.UppercaseFirstLetter(character.Value.Name) = _CommonFunctions.UppercaseFirstLetter(toUser) Then
                            guid = character.Value.Guid
                            Exit For
                        End If
                    Next
                    _WorldCluster.CHARACTERs_Lock.ReleaseReaderLock()

                    If guid > 0 AndAlso _WorldCluster.CHARACTERs.ContainsKey(guid) Then
                        'DONE: Check if ignoring
                        If _WorldCluster.CHARACTERs(guid).IgnoreList.Contains(client.Character.Guid) AndAlso client.Character.Access < AccessLevel.GameMaster Then
                            'Client.Character.SystemMessage(String.Format("{0} is ignoring you.", ToUser))
                            client.Character.SendChatMessage(guid, "", ChatMsg.CHAT_MSG_IGNORED, LANGUAGES.LANG_UNIVERSAL, "")
                        Else
                            'To message
                            client.Character.SendChatMessage(guid, message, ChatMsg.CHAT_MSG_WHISPER_INFORM, msgLanguage, "")
                            If _WorldCluster.CHARACTERs(guid).DND = False OrElse client.Character.Access >= AccessLevel.GameMaster Then
                                'From message
                                _WorldCluster.CHARACTERs(guid).SendChatMessage(client.Character.Guid, message, ChatMsg.CHAT_MSG_WHISPER, msgLanguage, "")
                            Else
                                'DONE: Send the DND message
                                client.Character.SendChatMessage(guid, _WorldCluster.CHARACTERs(guid).AfkMessage, ChatMsg.CHAT_MSG_DND, msgLanguage, "")
                            End If

                            'DONE: Send the AFK message
                            If _WorldCluster.CHARACTERs(guid).AFK Then client.Character.SendChatMessage(guid, _WorldCluster.CHARACTERs(guid).AfkMessage, ChatMsg.CHAT_MSG_AFK, msgLanguage, "")
                        End If
                    Else
                        Dim smsgChatPlayerNotFound As New Packets.PacketClass(OPCODES.SMSG_CHAT_PLAYER_NOT_FOUND)
                        smsgChatPlayerNotFound.AddString(toUser)
                        client.Send(smsgChatPlayerNotFound)
                        smsgChatPlayerNotFound.Dispose()
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_PARTY, ChatMsg.CHAT_MSG_RAID, ChatMsg.CHAT_MSG_RAID_LEADER, ChatMsg.CHAT_MSG_RAID_WARNING
                    Dim message As String = packet.GetString()

                    'DONE: Check in group
                    If Not client.Character.IsInGroup Then
                        Exit Select
                    End If

                    'DONE: Broadcast to party
                    client.Character.Group.SendChatMessage(client.Character, message, msgLanguage, msgType)
                    Exit Select

                Case ChatMsg.CHAT_MSG_AFK
                    Dim message As String = packet.GetString()
                    'TODO: Can not be used while in combat!
                    If message = "" OrElse client.Character.AFK = False Then
                        If client.Character.AFK = False Then
                            If message = "" Then message = "Away From Keyboard"
                            client.Character.AfkMessage = message
                        End If
                        client.Character.AFK = Not client.Character.AFK
                        If client.Character.AFK AndAlso client.Character.DND Then
                            client.Character.DND = False
                        End If
                        If client.Character.AFK Then
                            client.Character.ChatFlag = ChatFlag.FLAGS_AFK
                        Else
                            client.Character.ChatFlag = ChatFlag.FLAGS_NONE
                        End If
                        'DONE: Pass the packet to the world server so it also knows about it
                        client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_DND
                    Dim message As String = packet.GetString()
                    If message = "" OrElse client.Character.DND = False Then
                        If client.Character.DND = False Then
                            If message = "" Then message = "Do Not Disturb"
                            client.Character.AfkMessage = message
                        End If
                        client.Character.DND = Not client.Character.DND
                        If client.Character.DND AndAlso client.Character.AFK Then
                            client.Character.AFK = False
                        End If
                        If client.Character.DND Then
                            client.Character.ChatFlag = ChatFlag.FLAGS_DND
                        Else
                            client.Character.ChatFlag = ChatFlag.FLAGS_NONE
                        End If
                        'DONE: Pass the packet to the world server so it also knows about it
                        client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_SAY, ChatMsg.CHAT_MSG_YELL, ChatMsg.CHAT_MSG_EMOTE
                    client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
                    Exit Select

                Case ChatMsg.CHAT_MSG_GUILD
                    Dim message As String = packet.GetString()

                    'DONE: Broadcast to guild
                    _WC_Guild.BroadcastChatMessageGuild(client.Character, message, msgLanguage, client.Character.Guild.ID)
                    Exit Select

                Case ChatMsg.CHAT_MSG_OFFICER
                    Dim message As String = packet.GetString()

                    'DONE: Broadcast to officer chat
                    _WC_Guild.BroadcastChatMessageOfficer(client.Character, message, msgLanguage, client.Character.Guild.ID)
                    Exit Select

                Case Else
                    _WorldCluster.Log.WriteLine(LogType.FAILED, "[{0}:{1}] Unknown chat message [msgType={2}, msgLanguage={3}]", client.IP, client.Port, msgType, msgLanguage)
                    _Packets.DumpPacket(packet.Data, client)
            End Select

        End Sub

        Public Sub On_CMSG_JOIN_CHANNEL(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim channelName As String = packet.GetString()
            Dim password As String = packet.GetString()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_JOIN_CHANNEL [{2}]", client.IP, client.Port, channelName)

            If Not _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(channelName) Then
                'The New does a an add to the .Containskey collection above
                Dim newChannel As New WS_Handler_Channels.ChatChannelClass(channelName)
            End If
            _WS_Handler_Channels.CHAT_CHANNELs(channelName).Join(client.Character, password)
        End Sub

        Public Sub On_CMSG_LEAVE_CHANNEL(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LEAVE_CHANNEL [{2}]", client.IP, client.Port, ChannelName)

            ChannelName = ChannelName
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).Part(client.Character)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_LIST(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString()

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_LIST [{2}]", client.IP, client.Port, ChannelName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).List(client.Character)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_PASSWORD(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelNewPassword As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_PASSWORD [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelNewPassword)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetPassword(client.Character, ChannelNewPassword)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_SET_OWNER(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelNewOwner As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_SET_OWNER [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelNewOwner)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                If _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).CanSetOwner(client.Character, ChannelNewOwner) Then
                    For Each GUID As ULong In _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).Joined.ToArray
                        If _WorldCluster.CHARACTERs(GUID).Name.ToUpper = ChannelNewOwner.ToUpper Then
                            _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetOwner(_WorldCluster.CHARACTERs(GUID))
                            Exit For
                        End If
                    Next
                End If
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_OWNER(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_OWNER [{2}]", client.IP, client.Port, ChannelName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).GetOwner(client.Character)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_MODERATOR(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelUser As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_MODERATOR [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelUser)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetModerator(client.Character, ChannelUser)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_UNMODERATOR(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelUser As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_UNMODERATOR [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelUser)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetUnModerator(client.Character, ChannelUser)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_MUTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelUser As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_MUTE [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelUser)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetMute(client.Character, ChannelUser)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_UNMUTE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            Dim ChannelUser As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_UNMUTE [{2}, {3}]", client.IP, client.Port, ChannelName, ChannelUser)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetUnMute(client.Character, ChannelUser)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_INVITE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            If (packet.Data.Length - 1) < 6 + ChannelName.Length + 1 Then Exit Sub
            Dim PlayerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_INVITE [{2}, {3}]", client.IP, client.Port, ChannelName, PlayerName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).Invite(client.Character, PlayerName)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_KICK(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            If (packet.Data.Length - 1) < 6 + ChannelName.Length + 1 Then Exit Sub
            Dim PlayerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_KICK [{2}, {3}]", client.IP, client.Port, ChannelName, PlayerName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).Kick(client.Character, PlayerName)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_ANNOUNCEMENTS(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_ANNOUNCEMENTS [{2}]", client.IP, client.Port, ChannelName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetAnnouncements(client.Character)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_BAN(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            If (packet.Data.Length - 1) < 6 + ChannelName.Length + 1 Then Exit Sub
            Dim PlayerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_BAN [{2}, {3}]", client.IP, client.Port, ChannelName, PlayerName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).Ban(client.Character, PlayerName)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_UNBAN(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString
            If (packet.Data.Length - 1) < 6 + ChannelName.Length + 1 Then Exit Sub
            Dim PlayerName As String = _Functions.CapitalizeName(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_UNBAN [{2}, {3}]", client.IP, client.Port, ChannelName, PlayerName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).UnBan(client.Character, PlayerName)
            End If
        End Sub

        Public Sub On_CMSG_CHANNEL_MODERATE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ChannelName As String = packet.GetString

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHANNEL_MODERATE [{2}]", client.IP, client.Port, ChannelName)

            'ChannelName = ChannelName.ToUpper
            If _WS_Handler_Channels.CHAT_CHANNELs.ContainsKey(ChannelName) Then
                _WS_Handler_Channels.CHAT_CHANNELs(ChannelName).SetModeration(client.Character)
            End If
        End Sub

    End Class
End Namespace