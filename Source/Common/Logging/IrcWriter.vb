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
Imports Meebey.SmartIrc4net
Imports System.Threading

Namespace Logging
    Public Class IrcWriter
        Inherits BaseWriter

        Protected Conn As IrcClient
        Dim _message As String = Nothing

        Public Sub New(server As String, port As Integer, nick As String, channel As String)
            Conn = New IrcClient With {
                .AutoReconnect = True,
                .AutoRejoin = True,
                .AutoRelogin = True,
                .SendDelay = 200,
                .ActiveChannelSyncing = True
            }

            AddHandler Conn.OnError, AddressOf OnErrorMessage
            AddHandler Conn.OnQueryMessage, AddressOf OnQueryMessage
#If DEBUG Then
            AddHandler Conn.OnRawMessage, AddressOf OnRawMessage
#End If

            Conn.CtcpVersion = "MaNGOSvb Log Bot"
            Conn.Connect(server, port)
            Conn.Login(nick, "MaNGOSvb Log Bot")
            Conn.RfcJoin(channel)
            Conn.SendMessage(SendType.Action, channel, " starts logging")

            Dim t As New Thread(AddressOf Conn.Listen)
            t.Start()
        End Sub

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Conn.Disconnect()
            End If
            _disposedValue = True
        End Sub

        Public Overrides Sub Write(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            For Each channel As String In Conn.JoinedChannels
                Conn.SendMessage(SendType.Message, channel, String.Format(formatStr, arg))
            Next
        End Sub
        Public Overrides Sub WriteLine(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            For Each channel As String In Conn.JoinedChannels
                Conn.SendMessage(SendType.Message, channel, String.Format(formatStr, arg))
            Next
        End Sub
        Public Overrides Function ReadLine() As String
            While (_message Is Nothing)
                Thread.Sleep(100)
            End While

            Dim msg As String = _message
            _message = Nothing
            Return msg
        End Function

        Public Sub OnQueryMessage(sender As Object, e As IrcEventArgs)
            Console.WriteLine(e.Data.Message)
            _message = e.Data.Message
        End Sub
        Public Sub OnRawMessage(sender As Object, e As IrcEventArgs)
            Console.WriteLine(e.Data.RawMessage)
        End Sub
        Public Sub OnErrorMessage(sender As Object, e As ErrorEventArgs)
            Console.WriteLine(e.ErrorMessage)
        End Sub

    End Class
End Namespace
