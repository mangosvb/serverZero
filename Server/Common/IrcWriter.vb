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
Imports Meebey.SmartIrc4net
Imports System.Threading

Public Class IrcWriter
    Inherits BaseWriter

    Dim conn As IrcClient
    Dim message As String = Nothing
    Public Sub New(ByVal server As String, ByVal port As Integer, ByVal nick As String, ByVal channel As String)
        conn = New IrcClient
        conn.AutoReconnect = True
        conn.AutoRejoin = True
        conn.AutoRelogin = True
        conn.SendDelay = 200
        conn.ActiveChannelSyncing = True

        AddHandler conn.OnError, AddressOf OnErrorMessage
        AddHandler conn.OnQueryMessage, AddressOf OnQueryMessage
#If DEBUG Then
        AddHandler conn.OnRawMessage, AddressOf OnRawMessage
#End If

        conn.CtcpVersion = "MaNGOSvb Log Bot"
        conn.Connect(server, port)
        conn.Login(nick, "MaNGOSvb Log Bot")
        conn.RfcJoin(channel)
        conn.SendMessage(SendType.Action, channel, " starts logging")

        Dim t As New Thread(AddressOf conn.Listen)
        t.Start()
    End Sub
    Public Overrides Sub Dispose()
        conn.Disconnect()
    End Sub

    Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        For Each Channel As String In conn.JoinedChannels
            conn.SendMessage(SendType.Message, Channel, String.Format(formatStr, arg))
        Next
    End Sub
    Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        For Each Channel As String In conn.JoinedChannels
            conn.SendMessage(SendType.Message, Channel, String.Format(formatStr, arg))
        Next
    End Sub
    Public Overrides Function ReadLine() As String
        While (message Is Nothing)
            System.Threading.Thread.Sleep(100)
        End While

        Dim msg As String = message
        message = Nothing
        Return msg
    End Function

    Public Sub OnQueryMessage(ByVal sender As Object, ByVal e As IrcEventArgs)
        Console.WriteLine(e.Data.Message)
        message = e.Data.Message
    End Sub
    Public Sub OnRawMessage(ByVal sender As Object, ByVal e As IrcEventArgs)
        Console.WriteLine(e.Data.RawMessage)
    End Sub
    Public Sub OnErrorMessage(ByVal sender As Object, ByVal e As ErrorEventArgs)
        Console.WriteLine(e.ErrorMessage)
    End Sub

End Class