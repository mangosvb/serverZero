﻿'
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

Imports mangosVB.Shared

Public Module WS_Handlers_Gamemaster

    Public Sub On_CMSG_WORLD_TELEPORT(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_WORLD_TELEPORT", client.IP, client.Port)

        If client.Access >= AccessLevel.GameMaster Then
            packet.GetInt16()
            Dim Time As Integer = packet.GetInt32
            Dim Map As UInteger = packet.GetUInt32
            Dim X As Single = packet.GetFloat
            Dim Y As Single = packet.GetFloat
            Dim Z As Single = packet.GetFloat
            Dim O As Single = packet.GetFloat

            client.Character.Teleport(X, Y, Z, O, Map)
        Else
            'Do we need to notify client that he is using GM command... i think no :)
        End If
    End Sub

End Module