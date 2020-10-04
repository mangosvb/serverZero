'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Imports MangosVB.Common.Globals
Imports MangosVB.Shared
Imports MangosVB.WorldCluster.DataStores
Imports MangosVB.WorldCluster.Globals

Namespace Handlers

    Public Module WS_Handler_Channels

        Public CHAT_CHANNELs As New Dictionary(Of String, ChatChannelClass)
        Private CHAT_CHANNELs_Counter As Long = 1
        Private Function GetNexyChatChannelID() As Long
            Return Threading.Interlocked.Increment(CHAT_CHANNELs_Counter)
        End Function

        Public Class ChatChannelClass
            Implements IDisposable

            'This is server-side ID
            Public ID As Long = 0

            'These are channel identificators
            Public ChannelIndex As Integer
            Public ChannelFlags As Byte
            Public ChannelName As String

            Public Password As String = ""
            Public Announce As Boolean = True
            Public Moderate As Boolean = True

            Public Joined As New List(Of ULong)
            Public Joined_Mode As New Dictionary(Of ULong, Byte)

            Public Banned As New List(Of ULong)
            Public Moderators As New List(Of ULong)
            Public Muted As New List(Of ULong)
            Public Owner As ULong = 0

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    CHAT_CHANNELs.Remove(ChannelName.ToUpper)
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

            Public Sub New(name As String)
                ID = GetNexyChatChannelID()
                ChannelIndex = 0
                ChannelName = name
                ChannelFlags = CHANNEL_FLAG.CHANNEL_FLAG_NONE

                CHAT_CHANNELs.Add(ChannelName, Me)

                Dim sZone As String = name.Substring(name.IndexOf(" - ", StringComparison.Ordinal) + 3)
                For Each chatChannel As KeyValuePair(Of Integer, ChatChannelInfo) In ChatChannelsInfo
                    If chatChannel.Value.Name.Replace("%s", sZone).ToUpper() = name.ToUpper() Then
                        ChannelIndex = chatChannel.Key
                        Exit For
                    End If
                Next

                If ChatChannelsInfo.ContainsKey(ChannelIndex) Then
                    'Default channel
                    ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_GENERAL
                    Announce = False
                    Moderate = False

                    With ChatChannelsInfo(ChannelIndex)
                        If .Flags And ChatChannelsFlags.FLAG_TRADE Then
                            ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_TRADE
                        End If
                        If .Flags And ChatChannelsFlags.FLAG_CITY_ONLY2 Then
                            ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_CITY
                        End If
                        If .Flags And ChatChannelsFlags.FLAG_LFG Then
                            ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_LFG
                        Else
                            ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_NOT_LFG
                        End If
                    End With
                Else
                    'Custom channel
                    ChannelFlags = ChannelFlags Or CHANNEL_FLAG.CHANNEL_FLAG_CUSTOM
                End If
            End Sub

            Public Sub Say(message As String, msgLang As Integer, ByRef character As CharacterObject)
                If Muted.Contains(character.Guid) Then
                    Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOUCANTSPEAK, character.Guid, Nothing, Nothing)
                    character.Client.Send(p)
                    p.Dispose()
                    Exit Sub
                ElseIf Not Joined.Contains(character.Guid) Then
                    Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, character.Guid, Nothing, Nothing)
                    character.Client.Send(p)
                    p.Dispose()
                    Exit Sub
                Else
                    Dim packet As PacketClass = BuildChatMessage(character.Guid, message, ChatMsg.CHAT_MSG_CHANNEL, msgLang, character.ChatFlag, ChannelName)
                    Broadcast(packet)
                    packet.Dispose()
                    Log.WriteLine(LogType.USER, "[{0}:{1}] SMSG_MESSAGECHAT [{2}: <{3}> {4}]", character.Client.IP, character.Client.Port, ChannelName, character.Name, message)
                End If
            End Sub

            Public Overridable Sub Join(ByRef character As CharacterObject, clientPassword As String)
                'DONE: Check if Already joined
                If Joined.Contains(character.Guid) Then

                    If character.JoinedChannels.Contains(0) Then
                        Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_ALREADY_ON, character.Guid, Nothing, Nothing)
                        character.Client.Send(p)
                        p.Dispose()
                        Exit Sub
                    End If
                End If

                'DONE: Check if banned
                If Banned.Contains(character.Guid) Then
                    Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_ARE_BANNED, character.Guid, Nothing, Nothing)
                    character.Client.Send(p)
                    p.Dispose()
                    Exit Sub
                End If

                'DONE: Check for password
                If Password <> "" Then
                    If Password <> clientPassword Then
                        Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_WRONG_PASS, character.Guid, Nothing, Nothing)
                        character.Client.Send(p)
                        p.Dispose()
                        Exit Sub
                    End If
                End If

                'DONE: {0} Joined channel
                If Announce Then
                    Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_JOINED, character.Guid, Nothing, Nothing)
                    Broadcast(response)
                    response.Dispose()
                End If

                'DONE: You Joined channel
                Dim response2 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_JOINED, character.Guid, Nothing, Nothing)
                character.Client.Send(response2)
                response2.Dispose()

                If Joined.Contains(character.Guid) = False Then
                    Joined.Add(character.Guid)
                End If

                If Joined_Mode.ContainsKey(character.Guid) = False Then
                    Joined_Mode.Add(character.Guid, CHANNEL_USER_FLAG.CHANNEL_FLAG_NONE)
                End If

                If character.JoinedChannels.Contains(ChannelName) = False Then
                    character.JoinedChannels.Add(ChannelName)
                End If

                'DONE: If new channel, set owner
                If HaveFlags(ChannelFlags, CHANNEL_FLAG.CHANNEL_FLAG_CUSTOM) AndAlso Owner = 0 Then
                    SetOwner(character)
                End If

                'DONE: Update flags
                Dim modes As Byte = CHANNEL_USER_FLAG.CHANNEL_FLAG_NONE
                If Muted.Contains(character.Guid) Then
                    modes = modes Or CHANNEL_USER_FLAG.CHANNEL_FLAG_MUTED
                End If
                If Moderators.Contains(character.Guid) Then
                    modes = modes Or CHANNEL_USER_FLAG.CHANNEL_FLAG_MODERATOR
                End If
                If Owner = character.Guid Then
                    modes = modes Or CHANNEL_USER_FLAG.CHANNEL_FLAG_OWNER
                End If
                Joined_Mode(character.Guid) = modes
            End Sub

            Public Overridable Sub Part(ByRef Character As CharacterObject)
                'DONE: Check if not on this channel
                If Not Joined.Contains(Character.Guid) Then
                    If Character.Client IsNot Nothing Then
                        Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                        Character.Client.Send(p)
                        p.Dispose()
                    End If
                    Exit Sub
                End If

                'DONE: You Left channel
                If Character.Client IsNot Nothing Then
                    Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_LEFT, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(p)
                    p.Dispose()
                End If

                Joined.Remove(Character.Guid)
                Joined_Mode.Remove(Character.Guid)
                Character.JoinedChannels.Remove(ChannelName)

                'DONE: {0} Left channel
                If Announce Then
                    Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_LEFT, Character.Guid, Nothing, Nothing)
                    Broadcast(response)
                    response.Dispose()
                End If

                'DONE: Set new owner
                If HaveFlags(ChannelFlags, CHANNEL_FLAG.CHANNEL_FLAG_CUSTOM) AndAlso Owner = Character.Guid AndAlso Joined.Count > 0 Then
                    Dim tmp As IEnumerator = Joined.GetEnumerator()
                    tmp.MoveNext()
                    SetOwner(CHARACTERs(tmp.Current))
                End If

                'DONE: If free and not global - clear channel
                If HaveFlags(ChannelFlags, CHANNEL_FLAG.CHANNEL_FLAG_CUSTOM) AndAlso Joined.Count = 0 Then
                    CHAT_CHANNELs.Remove(ChannelName)
                    Dispose()
                End If
            End Sub

            Public Overridable Sub Kick(ByRef Character As CharacterObject, ByVal Name As String)
                Dim VictimGUID As ULong = GetCharacterGUIDByName(Name)

                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not CHARACTERs.ContainsKey(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Joined.Contains(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    'DONE: You Left channel
                    Dim packet1 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_LEFT, Character.Guid, Nothing, Nothing)
                    CHARACTERs(VictimGUID).client.Send(packet1)
                    packet1.Dispose()

                    Joined.Remove(VictimGUID)
                    Joined_Mode.Remove(VictimGUID)
                    CHARACTERs(VictimGUID).JoinedChannels.Remove(ChannelName.ToUpper)

                    'DONE: [%s] Player %s kicked by %s.
                    Dim packet2 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_KICKED, VictimGUID, Character.Guid, Nothing)
                    Broadcast(packet2)
                    packet2.Dispose()
                End If
            End Sub

            Public Overridable Sub Ban(ByRef Character As CharacterObject, ByVal Name As String)
                Dim VictimGUID As ULong = GetCharacterGUIDByName(Name)

                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not CHARACTERs.ContainsKey(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Joined.Contains(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Banned.Contains(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_PLAYER_INVITE_BANNED, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Banned.Add(VictimGUID)

                    'DONE: [%s] Player %s banned by %s.
                    Dim packet2 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_BANNED, VictimGUID, Character.Guid, Nothing)
                    Broadcast(packet2)
                    packet2.Dispose()

                    Joined.Remove(VictimGUID)
                    Joined_Mode.Remove(VictimGUID)
                    CHARACTERs(VictimGUID).JoinedChannels.Remove(ChannelName.ToUpper)

                    'DONE: You Left channel
                    Dim packet1 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_LEFT, Character.Guid, Nothing, Nothing)
                    CHARACTERs(VictimGUID).client.Send(packet1)
                    packet1.Dispose()
                End If
            End Sub

            Public Overridable Sub UnBan(ByRef Character As CharacterObject, ByVal Name As String)
                Dim VictimGUID As ULong = GetCharacterGUIDByName(Name)

                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not CHARACTERs.ContainsKey(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Banned.Contains(VictimGUID) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_BANNED, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Banned.Remove(VictimGUID)

                    'DONE: [%s] Player %s unbanned by %s.
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_UNBANNED, VictimGUID, Character.Guid, Nothing)
                    Broadcast(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub List(ByRef Character As CharacterObject)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Dim packet As New PacketClass(OPCODES.SMSG_CHANNEL_LIST)
                    packet.AddInt8(0)                   'ChannelType
                    packet.AddString(ChannelName)       'ChannelName
                    packet.AddInt8(ChannelFlags)        'ChannelFlags
                    packet.AddInt32(Joined.Count)

                    For Each GUID As ULong In Joined
                        packet.AddUInt64(GUID)
                        packet.AddInt8(Joined_Mode(GUID))
                    Next
                    Character.Client.Send(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub Invite(ByRef Character As CharacterObject, ByVal Name As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Dim GUID As ULong = GetCharacterGUIDByName(Name)

                    If Not CHARACTERs.ContainsKey(GUID) Then
                        Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                        Character.Client.Send(packet)
                        packet.Dispose()
                    ElseIf GetCharacterSide(CHARACTERs(GUID).Race) <> GetCharacterSide(Character.Race) Then
                        Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_INVITED_WRONG_FACTION, Character.Guid, Nothing, Nothing)
                        Character.Client.Send(packet)
                        packet.Dispose()
                    ElseIf Joined.Contains(GUID) Then
                        Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_ALREADY_ON, GUID, Nothing, Nothing)
                        Character.Client.Send(packet)
                        packet.Dispose()
                    ElseIf Banned.Contains(GUID) Then
                        Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_PLAYER_INVITE_BANNED, GUID, Nothing, Name)
                        Character.Client.Send(packet)
                        packet.Dispose()
                    ElseIf CHARACTERs(GUID).IgnoreList.Contains(Character.Guid) Then
                        '?
                    Else
                        Dim packet1 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_PLAYER_INVITED, Character.Guid, Nothing, CHARACTERs(GUID).Name)
                        Character.Client.Send(packet1)
                        packet1.Dispose()

                        Dim packet2 As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_INVITED, Character.Guid, Nothing, Nothing)
                        CHARACTERs(GUID).client.Send(packet2)
                        packet2.Dispose()
                    End If
                End If
            End Sub

            Public Function CanSetOwner(ByRef Character As CharacterObject, ByVal Name As String) As Boolean
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                    Return False
                End If
                If Owner <> Character.Guid Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_OWNER, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                    Return False
                End If

                For Each GUID As ULong In Joined.ToArray
                    If CHARACTERs(GUID).Name.ToUpper = Name.ToUpper Then
                        Return True
                    End If
                Next

                Dim p As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Nothing)
                Character.Client.Send(p)
                p.Dispose()
                Return False
            End Function

            Public Sub GetOwner(ByRef Character As CharacterObject)
                Dim p As PacketClass

                If Not Joined.Contains(Character.Guid) Then
                    p = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                ElseIf Owner > 0 Then
                    p = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_WHO_OWNER, Character.Guid, Nothing, CHARACTERs(Owner).Name)
                Else
                    p = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_WHO_OWNER, Character.Guid, Nothing, "Nobody")
                End If

                Character.Client.Send(p)
                p.Dispose()
            End Sub

            Public Sub SetOwner(ByRef Character As CharacterObject)
                If Joined_Mode.ContainsKey(Owner) Then
                    Joined_Mode(Owner) = Joined_Mode(Owner) And Not CHANNEL_USER_FLAG.CHANNEL_FLAG_OWNER
                End If
                Joined_Mode(Character.Guid) = Joined_Mode(Character.Guid) Or CHANNEL_USER_FLAG.CHANNEL_FLAG_OWNER

                Owner = Character.Guid
                If Not Moderators.Contains(Owner) Then Moderators.Add(Owner)

                Dim p As PacketClass
                p = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_CHANGE_OWNER, Character.Guid, Nothing, Nothing)
                Broadcast(p)
                p.Dispose()
            End Sub

            Public Sub SetAnnouncements(ByRef Character As CharacterObject)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Announce = Not Announce
                    Dim packet As PacketClass
                    If Announce Then
                        packet = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_ENABLE_ANNOUNCE, Character.Guid, Nothing, Nothing)
                    Else
                        packet = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_DISABLE_ANNOUNCE, Character.Guid, Nothing, Nothing)
                    End If
                    Broadcast(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetModeration(ByRef Character As CharacterObject)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Character.Guid <> Owner Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_OWNER, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Moderate = Not Moderate
                    Dim packet As PacketClass
                    If Announce Then
                        packet = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_MODERATED, Character.Guid, Nothing, Nothing)
                    Else
                        packet = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_UNMODERATED, Character.Guid, Nothing, Nothing)
                    End If
                    Broadcast(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetPassword(ByRef Character As CharacterObject, ByVal NewPassword As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else
                    Password = NewPassword

                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_SET_PASSWORD, Character.Guid, Nothing, Nothing)
                    Broadcast(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetModerator(ByRef Character As CharacterObject, ByVal Name As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else

                    For Each GUID As ULong In Joined.ToArray
                        If CHARACTERs(GUID).Name.ToUpper = Name.ToUpper Then
                            Dim flags As Byte = Joined_Mode(GUID)
                            Joined_Mode(GUID) = Joined_Mode(GUID) Or CHANNEL_USER_FLAG.CHANNEL_FLAG_MODERATOR

                            If Not Moderators.Contains(GUID) Then Moderators.Add(GUID)

                            Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_MODE_CHANGE, GUID, flags, Nothing)
                            Broadcast(response)
                            response.Dispose()
                            Exit Sub
                        End If
                    Next

                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetUnModerator(ByRef Character As CharacterObject, ByVal Name As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else

                    For Each GUID As ULong In Joined.ToArray
                        If CHARACTERs(GUID).Name.ToUpper = Name.ToUpper Then
                            Dim flags As Byte = Joined_Mode(GUID)
                            Joined_Mode(GUID) = Joined_Mode(GUID) And Not CHANNEL_USER_FLAG.CHANNEL_FLAG_MODERATOR

                            Moderators.Remove(GUID)

                            Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_MODE_CHANGE, GUID, flags, Nothing)
                            Broadcast(response)
                            response.Dispose()
                            Exit Sub
                        End If
                    Next

                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetMute(ByRef Character As CharacterObject, ByVal Name As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else

                    For Each GUID As ULong In Joined.ToArray
                        If CHARACTERs(GUID).Name.ToUpper = Name.ToUpper Then
                            Dim flags As Byte = Joined_Mode(GUID)
                            Joined_Mode(GUID) = Joined_Mode(GUID) Or CHANNEL_USER_FLAG.CHANNEL_FLAG_MUTED

                            If Not Muted.Contains(GUID) Then Muted.Add(GUID)

                            Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_MODE_CHANGE, GUID, flags, Nothing)
                            Broadcast(response)
                            response.Dispose()
                            Exit Sub
                        End If
                    Next

                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub SetUnMute(ByRef Character As CharacterObject, ByVal Name As String)
                If Not Joined.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                ElseIf Not Moderators.Contains(Character.Guid) Then
                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR, Character.Guid, Nothing, Nothing)
                    Character.Client.Send(packet)
                    packet.Dispose()
                Else

                    For Each GUID As ULong In Joined.ToArray
                        If CHARACTERs(GUID).Name.ToUpper = Name.ToUpper Then
                            Dim flags As Byte = Joined_Mode(GUID)
                            Joined_Mode(GUID) = Joined_Mode(GUID) And Not CHANNEL_USER_FLAG.CHANNEL_FLAG_MUTED

                            Muted.Remove(GUID)

                            Dim response As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_MODE_CHANGE, GUID, flags, Nothing)
                            Broadcast(response)
                            response.Dispose()
                            Exit Sub
                        End If
                    Next

                    Dim packet As PacketClass = BuildChannelNotify(CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, Character.Guid, Nothing, Name)
                    Character.Client.Send(packet)
                    packet.Dispose()
                End If
            End Sub

            Public Sub Broadcast(ByRef p As PacketClass)
                For Each GUID As ULong In Joined.ToArray
                    CHARACTERs(GUID).client.SendMultiplyPackets(p)
                Next
            End Sub

            Public Sub Save()
                'TODO: Saving into database
            End Sub

            Public Sub Load()
                'TODO: Loading from database
            End Sub

            Protected Function BuildChannelNotify(ByVal Notify As CHANNEL_NOTIFY_FLAGS, ByVal GUID1 As ULong, ByVal GUID2 As ULong, ByVal Name As String) As PacketClass
                Dim response As New PacketClass(OPCODES.SMSG_CHANNEL_NOTIFY)
                response.AddInt8(Notify)
                response.AddString(ChannelName)

                Select Case Notify
                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_WRONG_PASS, CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON, CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATOR,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_OWNER, CHANNEL_NOTIFY_FLAGS.CHANNEL_YOUCANTSPEAK, CHANNEL_NOTIFY_FLAGS.CHANNEL_INVITED_WRONG_FACTION,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_INVALID_NAME, CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_MODERATED, CHANNEL_NOTIFY_FLAGS.CHANNEL_THROTTLED,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_IN_AREA, CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_IN_LFG
                        'No extra fields

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_JOINED, CHANNEL_NOTIFY_FLAGS.CHANNEL_LEFT, CHANNEL_NOTIFY_FLAGS.CHANNEL_SET_PASSWORD,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_CHANGE_OWNER, CHANNEL_NOTIFY_FLAGS.CHANNEL_ENABLE_ANNOUNCE, CHANNEL_NOTIFY_FLAGS.CHANNEL_DISABLE_ANNOUNCE,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_MODERATED, CHANNEL_NOTIFY_FLAGS.CHANNEL_UNMODERATED, CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_ARE_BANNED,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_ALREADY_ON, CHANNEL_NOTIFY_FLAGS.CHANNEL_INVITED
                        response.AddUInt64(GUID1)

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_KICKED, CHANNEL_NOTIFY_FLAGS.CHANNEL_BANNED, CHANNEL_NOTIFY_FLAGS.CHANNEL_UNBANNED
                        response.AddUInt64(GUID1)           'Victim
                        response.AddUInt64(GUID2)           'Moderator

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_ON_FOR_NAME, CHANNEL_NOTIFY_FLAGS.CHANNEL_WHO_OWNER, CHANNEL_NOTIFY_FLAGS.CHANNEL_PLAYER_INVITED,
                        CHANNEL_NOTIFY_FLAGS.CHANNEL_PLAYER_INVITE_BANNED, CHANNEL_NOTIFY_FLAGS.CHANNEL_NOT_BANNED
                        response.AddString(Name)

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_JOINED
                        response.AddInt8(ChannelFlags)
                        response.AddUInt64(ChannelIndex)

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_YOU_LEFT
                        response.AddUInt64(ChannelIndex)

                    Case CHANNEL_NOTIFY_FLAGS.CHANNEL_MODE_CHANGE
                        response.AddUInt64(GUID1)
                        response.AddInt8(GUID2)                     'Old Player Flags
                        response.AddInt8(Joined_Mode(GUID1))        'New Player Flags

                    Case Else
                        Log.WriteLine(LogType.WARNING, "Probably wrong channel function used for SendChannelNotify({0})", Notify)
                End Select
                Return response
            End Function

        End Class

    End Module
End Namespace