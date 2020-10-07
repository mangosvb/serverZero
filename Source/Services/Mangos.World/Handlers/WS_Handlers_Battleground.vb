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
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Server

Namespace Handlers

    Public Class WS_Handlers_Battleground
        Public Sub On_CMSG_BATTLEMASTER_HELLO(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim GUID As ULong = packet.GetUInt64

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEMASTER_HELLO [{2:X}]", client.IP, client.Port, GUID)

            If _WorldServer.WORLD_CREATUREs.ContainsKey(GUID) = False Then Exit Sub
            If (_WorldServer.WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_BATTLEFIELDPERSON) = 0 Then Exit Sub
            If _WS_DBCDatabase.Battlemasters.ContainsKey(_WorldServer.WORLD_CREATUREs(GUID).ID) = False Then Exit Sub

            Dim BGType As Byte = _WS_DBCDatabase.Battlemasters(_WorldServer.WORLD_CREATUREs(GUID).ID)
            If _WS_DBCDatabase.Battlegrounds.ContainsKey(BGType) = False Then Exit Sub

            If _WS_DBCDatabase.Battlegrounds(BGType).MinLevel > client.Character.Level OrElse _WS_DBCDatabase.Battlegrounds(BGType).MaxLevel < client.Character.Level Then
                _Functions.SendMessageNotification(client, "You don't meet Battleground level requirements")
                Exit Sub
            End If

            'DONE: Send list
            Dim response As New Packets.PacketClass(OPCODES.SMSG_BATTLEFIELD_LIST)
            Try
                response.AddUInt64(client.Character.GUID)
                response.AddInt32(BGType)

                'If BGType = 6 Then          'Arenas
                'response.AddInt8(5)     'Unk
                'response.AddInt32(0)    'Unk
                'Else
                Dim Battlegrounds As List(Of Integer) = _WorldServer.ClsWorldServer.Cluster.BattlefieldList(BGType)
                response.AddInt8(0)                     'Unk
                response.AddInt32(Battlegrounds.Count)  'Number of BG Instances

                For Each Instance As Integer In Battlegrounds
                    response.AddInt32(Instance)
                Next
                'End If

                client.Send(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        Public Sub On_MSG_BATTLEGROUND_PLAYER_POSITIONS(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass) 'Not finished yet.. long ways to go!
            packet.GetUInt32()
        End Sub

        'Not Implement:
        'MSG_BATTLEGROUND_PLAYER_POSITIONS
        'MSG_PVP_LOG_DATA
        'CMSG_AREA_SPIRIT_HEALER_QUERY
        'CMSG_AREA_SPIRIT_HEALER_QUEUE
        'CMSG_REPORT_PVP_AFK

    End Class
End Namespace