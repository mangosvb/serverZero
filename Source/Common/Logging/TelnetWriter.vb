'
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

Imports System.Threading
Imports System.Net.Sockets

Imports mangosVB.Shared

'Using this logging type, you can watch logs with ordinary telnet client.
'Writing commands requires client, which don't send every key typed.

Namespace Logging
    Public Class TelnetWriter
        Inherits BaseWriter

        Protected Conn As TcpListener
        Protected Socket As Socket = Nothing
        Protected Const SleepTime As Integer = 1000

        Public Sub New(host As Net.IPAddress, port As Integer)
            Conn = New TcpListener(host, port)
            Conn.Start()
            ThreadPool.QueueUserWorkItem(AddressOf ConnWaitListen)
        End Sub

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Conn.Stop()
                Conn = Nothing
                Socket.Close()
            End If
            _disposedValue = True
        End Sub

        Public Overrides Sub Write(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return
            If Socket Is Nothing Then Return

            Try
                Socket.Send(Text.Encoding.UTF8.GetBytes(String.Format(formatStr, arg).ToCharArray))
            Catch
                Socket = Nothing
            End Try
        End Sub
        Public Overrides Sub WriteLine(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return
            If Socket Is Nothing Then Return

            Try
                Socket.Send(Text.Encoding.UTF8.GetBytes(String.Format(L(type) & ":" & "[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr & vbNewLine, arg).ToCharArray))
            Catch
                Socket = Nothing
            End Try
        End Sub
        Public Overrides Function ReadLine() As String
            While (Socket Is Nothing) OrElse (Socket.Available = 0)
                Thread.Sleep(SleepTime)
            End While

            Dim buffer(Socket.Available) As Byte
            Socket.Receive(buffer)
            Return Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length)
        End Function

        Protected Sub ConnWaitListen(state As Object)
            Do While (Not Conn Is Nothing)
                Thread.Sleep(SleepTime)
                If Conn.Pending() Then
                    Socket = Conn.AcceptSocket
                End If
            Loop
        End Sub

    End Class
End Namespace
