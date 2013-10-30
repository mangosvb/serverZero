'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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

Imports mangosVB.Common.BaseWriter


Public Module WC_Handlers_Battleground


    Public Sub On_CMSG_BATTLEFIELD_PORT(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()

        Dim Unk1 As Byte = packet.GetInt8
        Dim Unk2 As Byte = packet.GetInt8                   'unk, can be 0x0 (may be if was invited?) and 0x1
        Dim MapType As UInteger = packet.GetInt32           'type id from dbc
        Dim ID As UInteger = packet.GetUInt16               'ID
        Dim Action As Byte = packet.GetInt8                 'enter battle 0x1, leave queue 0x0

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEFIELD_PORT [MapType: {2}, Action: {3}, Unk1: {4}, Unk2: {5}, ID: {6}]", Client.IP, Client.Port, MapType, Action, Unk1, Unk2, ID)

        If Action = 0 Then
            BATTLEFIELDs(ID).Leave(Client.Character)
        Else
            BATTLEFIELDs(ID).Join(Client.Character)
        End If
    End Sub
    Public Sub On_CMSG_LEAVE_BATTLEFIELD(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()

        Dim Unk1 As Byte = packet.GetInt8
        Dim Unk2 As Byte = packet.GetInt8
        Dim MapType As UInteger = packet.GetInt32
        Dim ID As UInteger = packet.GetUInt16

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LEAVE_BATTLEFIELD [MapType: {2}, Unk1: {3}, Unk2: {4}, ID: {5}]", Client.IP, Client.Port, MapType, Unk1, Unk2, ID)

        BATTLEFIELDs(ID).Leave(Client.Character)
    End Sub
    Public Sub On_CMSG_BATTLEMASTER_JOIN(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 16 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim MapType As UInteger = packet.GetInt32
        Dim Intance As UInteger = packet.GetInt32
        Dim AsGroup As Byte = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BATTLEMASTER_JOIN [MapType: {2}, Instance: {3}, Group: {4}]", Client.IP, Client.Port, MapType, Intance, AsGroup)

        GetBattlefield(MapType, Client.Character.Level).Enqueue(Client.Character)
    End Sub
    
End Module
