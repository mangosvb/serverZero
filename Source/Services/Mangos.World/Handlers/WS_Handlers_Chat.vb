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

Imports Mangos.Common.Enums
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Handlers

    Public Module WS_Handlers_Chat

        Public Function GetChatFlag(ByVal objCharacter As WS_PlayerData.CharacterObject) As Byte
            If objCharacter.GM Then
                Return ChatEnum.ChatFlag.FLAGS_GM
            ElseIf objCharacter.AFK Then
                Return ChatFlag.FLAGS_AFK
            ElseIf objCharacter.DND Then
                Return ChatFlag.FLAGS_DND
            Else
                Return 0
            End If
        End Function

        Public Sub On_CMSG_MESSAGECHAT(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MESSAGECHAT", client.IP, client.Port)
            If (packet.Data.Length - 1) < 14 Then Exit Sub
            packet.GetInt16()

            Dim msgType As ChatMsg = packet.GetInt32()
            Dim msgLanguage As LANGUAGES = packet.GetInt32()
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MESSAGECHAT [{2}:{3}]", client.IP, client.Port, msgType, msgLanguage)

            'TODO: Check if we really are able to speak this language!

            'DONE: Changing language
            If client.Character.Spell_Language <> -1 Then msgLanguage = client.Character.Spell_Language

            Select Case msgType
                Case ChatMsg.CHAT_MSG_SAY, ChatMsg.CHAT_MSG_YELL, ChatMsg.CHAT_MSG_EMOTE, ChatMsg.CHAT_MSG_WHISPER
                    Dim Message As String = packet.GetString()
                    'Handle admin/gm commands
                    If Message.StartsWith(Config.CommandCharacter) AndAlso client.Character.Access > AccessLevel.Player Then
                        Message = Message.Remove(0, 1) ' Remove Command Start Character From Message
                        Dim toCommand As PacketClass = BuildChatMessage(SystemGUID, Message, ChatMsg.CHAT_MSG_SYSTEM, LANGUAGES.LANG_UNIVERSAL)
                        Try
                            client.Send(toCommand)
                        Finally
                            toCommand.Dispose()
                        End Try
                        OnCommand(client, Message)
                        Exit Sub
                    Else
                        client.Character.SendChatMessage(client.Character, Message, msgType, msgLanguage, "", True)
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_AFK
                    Dim Message As String = packet.GetString()
                    If (Message = "" OrElse client.Character.AFK = False) AndAlso client.Character.IsInCombat = False Then
                        client.Character.AFK = Not client.Character.AFK
                        If client.Character.AFK AndAlso client.Character.DND Then
                            client.Character.DND = False
                        End If
                        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
                        client.Character.SendCharacterUpdate()
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_DND
                    Dim Message As String = packet.GetString()
                    If Message = "" OrElse client.Character.DND = False Then
                        client.Character.DND = Not client.Character.DND
                        If client.Character.DND AndAlso client.Character.AFK Then
                            client.Character.AFK = False
                        End If
                        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FLAGS, client.Character.cPlayerFlags)
                        client.Character.SendCharacterUpdate()
                    End If
                    Exit Select

                Case ChatMsg.CHAT_MSG_CHANNEL, ChatMsg.CHAT_MSG_PARTY, ChatMsg.CHAT_MSG_RAID, ChatMsg.CHAT_MSG_RAID_LEADER, ChatMsg.CHAT_MSG_RAID_WARNING
                    Log.WriteLine(LogType.WARNING, "This chat message type should not be here!")
                    Exit Select

                Case Else
                    Log.WriteLine(LogType.FAILED, "[{0}:{1}] Unknown chat message [msgType={2}, msgLanguage={3}]", client.IP, client.Port, msgType, msgLanguage)
                    DumpPacket(packet.Data, client)
            End Select

        End Sub

    End Module
End NameSpace