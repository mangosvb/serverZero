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
Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Cluster.Globals
Imports Mangos.Cluster.Server
Imports Mangos.Common.Enums.Global

Namespace Handlers

    Public Class WC_Handlers_Tickets

        Public Sub On_CMSG_BUG(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 14 Then Exit Sub
            packet.GetInt16()
            Dim suggestion As SuggestionType = packet.GetInt32
            Dim cLength As Integer = packet.GetInt32
            Dim cString As String = _Functions.EscapeString(packet.GetString)
            If (packet.Data.Length - 1) < (14 + cString.Length + 5) Then Exit Sub
            Dim tLength As Integer = packet.GetInt32
            Dim tString As String = _Functions.EscapeString(packet.GetString)

            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUG [2]", client.IP, client.Port, suggestion)
            _WorldCluster.Log.WriteLine(LogType.INFORMATION, "Bug report [{0}:{1} Lengths:{2}, {3}] " & cString & vbCrLf & tString, cLength.ToString(), tLength.ToString())
        End Sub

        'ERR_TICKET_ALREADY_EXISTS
        'ERR_TICKET_CREATE_ERROR
        'ERR_TICKET_UPDATE_ERROR
        'ERR_TICKET_DB_ERROR
        'ERR_TICKET_NO_TEXT

        Private Enum GMTicketGetResult
            GMTICKET_AVAILABLE = 6
            GMTICKET_NOTICKET = 10
        End Enum
        Public Sub On_CMSG_GMTICKET_GETTICKET(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_GETTICKET", client.IP, client.Port)

            Dim SMSG_GMTICKET_GETTICKET As New Packets.PacketClass(OPCODES.SMSG_GMTICKET_GETTICKET)
            Dim MySQLResult As New DataTable
            _WorldCluster.CharacterDatabase.Query(String.Format("SELECT * FROM characters_tickets WHERE char_guid = {0};", client.Character.Guid), MySQLResult)
            If MySQLResult.Rows.Count > 0 Then
                SMSG_GMTICKET_GETTICKET.AddInt32(GMTicketGetResult.GMTICKET_AVAILABLE)
                SMSG_GMTICKET_GETTICKET.AddString(MySQLResult.Rows(0).Item("ticket_text"))
            Else
                SMSG_GMTICKET_GETTICKET.AddInt32(GMTicketGetResult.GMTICKET_NOTICKET)
            End If
            client.Send(SMSG_GMTICKET_GETTICKET)
            SMSG_GMTICKET_GETTICKET.Dispose()

            Dim SMSG_QUERY_TIME_RESPONSE As New Packets.PacketClass(OPCODES.SMSG_QUERY_TIME_RESPONSE)
            SMSG_QUERY_TIME_RESPONSE.AddInt32(_NativeMethods.timeGetTime("")) 'GetTimestamp(Now))
            client.Send(SMSG_QUERY_TIME_RESPONSE)
            SMSG_QUERY_TIME_RESPONSE.Dispose()
        End Sub

        Private Enum GMTicketCreateResult
            GMTICKET_ALREADY_HAVE = 1
            GMTICKET_CREATE_OK = 2
        End Enum

        Public Sub On_CMSG_GMTICKET_CREATE(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()
            Dim ticket_map As UInteger = packet.GetUInt32()
            Dim ticket_x As Single = packet.GetFloat()
            Dim ticket_y As Single = packet.GetFloat()
            Dim ticket_z As Single = packet.GetFloat()
            Dim ticket_text As String = _Functions.EscapeString(packet.GetString)

            Dim MySQLResult As New DataTable
            _WorldCluster.CharacterDatabase.Query(String.Format("SELECT * FROM characters_tickets WHERE char_guid = {0};", client.Character.Guid), MySQLResult)

            Dim SMSG_GMTICKET_CREATE As New Packets.PacketClass(OPCODES.SMSG_GMTICKET_CREATE)
            If MySQLResult.Rows.Count > 0 Then
                _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_CREATE", client.IP, client.Port)
                SMSG_GMTICKET_CREATE.AddInt32(GMTicketCreateResult.GMTICKET_ALREADY_HAVE)
            Else
                _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_CREATE [{2}]", client.IP, client.Port, ticket_text)
                _WorldCluster.CharacterDatabase.Update(String.Format("INSERT INTO characters_tickets (char_guid, ticket_text, ticket_x, ticket_y, ticket_z, ticket_map) VALUES ({0} , ""{1}"", {2}, {3}, {4}, {5});", client.Character.Guid, ticket_text, Trim(Str(ticket_x)), Trim(Str(ticket_y)), Trim(Str(ticket_z)), ticket_map))
                SMSG_GMTICKET_CREATE.AddInt32(GMTicketCreateResult.GMTICKET_CREATE_OK)
            End If
            client.Send(SMSG_GMTICKET_CREATE)
            SMSG_GMTICKET_CREATE.Dispose()
        End Sub

        Private Enum GMTicketSystemStatus
            GMTICKET_SYSTEMSTATUS_ENABLED = 1
            GMTICKET_SYSTEMSTATUS_DISABLED = 2
            GMTICKET_SYSTEMSTATUS_SURVEY = 3
        End Enum

        Public Sub On_CMSG_GMTICKET_SYSTEMSTATUS(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_SYSTEMSTATUS", client.IP, client.Port)

            Dim SMSG_GMTICKET_SYSTEMSTATUS As New Packets.PacketClass(OPCODES.SMSG_GMTICKET_SYSTEMSTATUS)
            SMSG_GMTICKET_SYSTEMSTATUS.AddInt32(GMTicketSystemStatus.GMTICKET_SYSTEMSTATUS_SURVEY)
            client.Send(SMSG_GMTICKET_SYSTEMSTATUS)
            SMSG_GMTICKET_SYSTEMSTATUS.Dispose()
        End Sub

        Private Enum GMTicketDeleteResult
            GMTICKET_DELETE_SUCCESS = 9
        End Enum

        Public Sub On_CMSG_GMTICKET_DELETETICKET(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_DELETETICKET", client.IP, client.Port)
            _WorldCluster.CharacterDatabase.Update(String.Format("DELETE FROM characters_tickets WHERE char_guid = {0};", client.Character.Guid))

            Dim SMSG_GMTICKET_DELETETICKET As New Packets.PacketClass(OPCODES.SMSG_GMTICKET_DELETETICKET)
            SMSG_GMTICKET_DELETETICKET.AddInt32(GMTicketDeleteResult.GMTICKET_DELETE_SUCCESS)
            client.Send(SMSG_GMTICKET_DELETETICKET)
            SMSG_GMTICKET_DELETETICKET.Dispose()
        End Sub

        Public Sub On_CMSG_GMTICKET_UPDATETEXT(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            If (packet.Data.Length - 1) < 7 Then Exit Sub
            packet.GetInt16()
            Dim ticket_text As String = _Functions.EscapeString(packet.GetString)
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GMTICKET_UPDATETEXT [{2}]", client.IP, client.Port, ticket_text)
            _WorldCluster.CharacterDatabase.Update(String.Format("UPDATE characters_tickets SET char_guid={0}, ticket_text=""{1}"";", client.Character.Guid, ticket_text))
        End Sub

        Public Sub On_CMSG_WHOIS(ByRef packet As Packets.PacketClass, ByRef client As WC_Network.ClientClass)
            packet.GetInt16()

            Dim Name As String = packet.GetString
            _WorldCluster.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_WHOIS [{2}]", client.IP, client.Port, Name)

            Dim response As New Packets.PacketClass(OPCODES.SMSG_WHOIS)
            response.AddString("This feature is not available yet.")
            client.Send(response)
            response.Dispose()
        End Sub

    End Class
End Namespace