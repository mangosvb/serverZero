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

Imports System.Security.Cryptography

Public Module WS_Auth
    Public Sub On_SMSG_PONG(ByRef Packet As PacketClass)
        Dim SequenceID As UInteger = Packet.GetUInt32
        Dim Latency As Integer = timeGetTime - PingSent
        If SequenceID = CurrentPing AndAlso Latency >= 0 Then
            CurrentLatency = Latency
        End If
    End Sub

    Public Sub On_SMSG_AUTH_CHALLENGE(ByRef Packet As PacketClass)
        Console.WriteLine("[{0}][World] Received Auth Challenge.", Format(TimeOfDay, "HH:mm:ss"))

        InitWarden()

        ServerSeed = Packet.GetUInt32

        Dim temp() As Byte = Text.Encoding.ASCII.GetBytes(Account.ToCharArray)
        temp = Concat(temp, BitConverter.GetBytes(0))
        temp = Concat(temp, BitConverter.GetBytes(ClientSeed))
        temp = Concat(temp, BitConverter.GetBytes(ServerSeed))
        temp = Concat(temp, SS_Hash)
        Dim algorithm1 As New SHA1Managed
        Dim ShaDigest() As Byte = algorithm1.ComputeHash(temp)

        Decoding = True

        Randomize()
        ClientSeed = UInteger.MaxValue * Rnd()
        Dim Response As New PacketClass(OPCODES.CMSG_AUTH_SESSION)
        Response.AddInt32(Revision)
        Response.AddInt32(0) 'SessionID?
        Response.AddString(Account.ToUpper)
        Response.AddUInt32(ClientSeed)
        Response.AddByteArray(ShaDigest)
        Response.AddInt32(0) 'Addon size
        Send(Response)
        Response.Dispose()

        Encoding = True
    End Sub

    Public Sub On_SMSG_AUTH_RESPONSE(ByRef Packet As PacketClass)
        Console.WriteLine("[{0}][World] Received Auth Response.", Format(TimeOfDay, "HH:mm:ss"))
        Dim ErrorCode As Byte = Packet.GetInt8()

        Select Case ErrorCode
            Case &HC
                Console.WriteLine("[{0}][World] Auth succeeded.", Format(TimeOfDay, "HH:mm:ss"))

                Dim Response As New PacketClass(OPCODES.CMSG_CHAR_ENUM)
                Send(Response)
                Response.Dispose()
            Case &H15
                Console.WriteLine("[{0}][World] Auth Challenge failed.", Format(TimeOfDay, "HH:mm:ss"))
                Worldserver.Disconnect()
            Case Else
                Console.WriteLine("[{0}][World] Unknown Auth Response error [{1}].", Format(TimeOfDay, "HH:mm:ss"), ErrorCode)
                Worldserver.Disconnect()
        End Select
    End Sub

    Public Sub On_SMSG_CHAR_ENUM(ByRef Packet As PacketClass)
        Console.WriteLine("[{0}][World] Received Character List.", Format(TimeOfDay, "HH:mm:ss"))

        Dim NumChars As Byte = Packet.GetInt8

        If NumChars > 0 Then
            For i As Byte = 1 To NumChars
                Dim GUID As ULong = Packet.GetUInt64
                Dim Name As String = Packet.GetString
                Dim Race As Byte = Packet.GetInt8
                Dim Classe As Byte = Packet.GetInt8
                Dim Gender As Byte = Packet.GetInt8
                Dim Skin As Byte = Packet.GetInt8
                Dim Face As Byte = Packet.GetInt8
                Dim HairStyle As Byte = Packet.GetInt8
                Dim HairColor As Byte = Packet.GetInt8
                Dim FacialHair As Byte = Packet.GetInt8
                Dim Level As Byte = Packet.GetInt8
                Dim Zone As Integer = Packet.GetInt32
                Dim Map As Integer = Packet.GetInt32
                Dim PosX As Single = Packet.GetFloat
                Dim PosY As Single = Packet.GetFloat
                Dim PosZ As Single = Packet.GetFloat
                Dim GuildID As UInteger = Packet.GetUInt32
                Dim PlayerState As UInteger = Packet.GetUInt32
                Dim RestState As Byte = Packet.GetInt8
                Dim PetInfoID As UInteger = Packet.GetUInt32
                Dim PetLevel As UInteger = Packet.GetUInt32
                Dim PetFamilyID As UInteger = Packet.GetUInt32

                Console.WriteLine("[{0}][World] Logging in with character [{1}].", Format(TimeOfDay, "HH:mm:ss"), Name)

                CharacterGUID = GUID
                Dim Response As New PacketClass(OPCODES.CMSG_PLAYER_LOGIN)
                Response.AddUInt64(GUID)
                Send(Response)
                Response.Dispose()
                Exit For

                'Skip the equipment
                Packet.Offset += (20 * 9)
            Next
        Else
            Console.WriteLine("[{0}][World] No characters found.", Format(TimeOfDay, "HH:mm:ss"))
        End If
    End Sub
End Module
