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
Imports WorldCluster.Server

Namespace Handlers

    Public Module WC_Handlers_Misc

        Public Sub On_CMSG_QUERY_TIME(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_QUERY_TIME", client.IP, client.Port)
            Dim response As New PacketClass(OPCODES.SMSG_QUERY_TIME_RESPONSE)
            response.AddInt32(TimeGetTime("")) 'GetTimestamp(Now))
            client.Send(response)
            response.Dispose()
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_QUERY_TIME_RESPONSE", client.IP, client.Port)
        End Sub

        Public Sub On_CMSG_NEXT_CINEMATIC_CAMERA(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_NEXT_CINEMATIC_CAMERA", client.IP, client.Port)
        End Sub

        Public Sub On_CMSG_COMPLETE_CINEMATIC(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_COMPLETE_CINEMATIC", client.IP, client.Port)
        End Sub

        Public Sub On_CMSG_PLAYED_TIME(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_NAME_QUERY", client.IP, client.Port)

            Dim response As New PacketClass(OPCODES.SMSG_PLAYED_TIME)
            response.AddInt32(1)
            response.AddInt32(1)
            client.Send(response)
            response.Dispose()
        End Sub

        Public Sub On_CMSG_NAME_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64()
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_NAME_QUERY [GUID={2:X}]", client.IP, client.Port, GUID)

            If GuidIsPlayer(GUID) AndAlso CHARACTERs.ContainsKey(GUID) Then
                Dim SMSG_NAME_QUERY_RESPONSE As New PacketClass(OPCODES.SMSG_NAME_QUERY_RESPONSE)
                SMSG_NAME_QUERY_RESPONSE.AddUInt64(GUID)
                SMSG_NAME_QUERY_RESPONSE.AddString(CHARACTERs(GUID).Name)
                SMSG_NAME_QUERY_RESPONSE.AddInt32(CHARACTERs(GUID).Race)
                SMSG_NAME_QUERY_RESPONSE.AddInt32(CHARACTERs(GUID).Gender)
                SMSG_NAME_QUERY_RESPONSE.AddInt32(CHARACTERs(GUID).Classe)
                SMSG_NAME_QUERY_RESPONSE.AddInt8(0)
                client.Send(SMSG_NAME_QUERY_RESPONSE)
                SMSG_NAME_QUERY_RESPONSE.Dispose()
                Exit Sub
            Else
                'DONE: Send it to the world server if it wasn't found in the cluster
                Try
                    client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
                Catch
                    WorldServer.Disconnect("NULL", New Integer() {client.Character.Map})
                End Try
            End If
        End Sub

        Public Sub On_CMSG_INSPECT(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_INSPECT [GUID={2:X}]", client.IP, client.Port, GUID)
        End Sub

        Public Sub On_MSG_MOVE_HEARTBEAT(ByRef packet As PacketClass, ByRef client As ClientClass)
            Try
                client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
            Catch
                WorldServer.Disconnect("NULL", New Integer() {client.Character.Map})
                Exit Sub
            End Try

            'DONE: Save location on cluster
            client.Character.PositionX = packet.GetFloat '(15)
            client.Character.PositionY = packet.GetFloat
            client.Character.PositionZ = packet.GetFloat

            'DONE: Sync your location to other party / raid members
            If client.Character.IsInGroup Then
                Dim statsPacket As New PacketClass(OPCODES.MSG_NULL_ACTION)
                statsPacket.Data = client.Character.GetWorld.GroupMemberStats(client.Character.GUID, PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POSITION + PartyMemberStatsFlag.GROUP_UPDATE_FLAG_ZONE)
                client.Character.Group.BroadcastToOutOfRange(statsPacket, client.Character)
                statsPacket.Dispose()
            End If
        End Sub

        Public Sub On_CMSG_CANCEL_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            If client.Character IsNot Nothing AndAlso client.Character.IsInWorld Then
                Try
                    client.Character.GetWorld.ClientPacket(client.Index, packet.Data)
                Catch
                    WorldServer.Disconnect("NULL", New Integer() {client.Character.Map})
                End Try
            Else
                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_TRADE", client.IP, client.Port)
            End If
        End Sub

        Public Sub On_CMSG_LOGOUT_CANCEL(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LOGOUT_CANCEL", client.IP, client.Port)
        End Sub

        'Public Sub On_CMSG_MOVE_TIME_SKIPPED(ByRef packet As PacketClass, ByRef client As ClientClass)
        '    packet.GetUInt64()
        '    packet.GetUInt32()
        '    Dim WC_MsTime As Integer = msTime()
        '    Dim ClientTimeDelay As Integer = MsTime - msTime()
        '    Dim MoveTime As Integer = (msTime() - (msTime() - ClientTimeDelay)) + 50 + msTime()
        '    packet.AddInt32(MoveTime, 10)
        '    Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOVE_TIME_SKIPPED", client.IP, client.Port)
        'End Sub

    End Module
End Namespace