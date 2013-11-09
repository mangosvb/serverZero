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
Imports System.IO

Public Class Logger
    Implements IDisposable

    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        _disposedValue = True
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Overridable Sub Write(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
    End Sub

    Public Overridable Sub WriteLine(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
    End Sub

    Public Overridable Function ReadLine() As String
        Return Console.ReadLine()
    End Function

    Public Shared Sub CreateLog(ByVal LogType As String, ByVal LogConfig As String, ByRef Log As Logger)
        Try
            Select Case UCase(LogType)
                Case "CONSOLE"
                    Log = New ConsoleWriter
                Case "FILE"
                    Log = New LogWriter(LogConfig)
                Case "TELNET"
                    Dim info As String() = Split(LogConfig, ":")
                    Log = New TelnetWriter(Net.IPAddress.Parse(info(0)), info(1))
            End Select
        Catch e As Exception
            Console.WriteLine("[{0}] Error creating log output!" & vbNewLine & e.ToString, Format(TimeOfDay, "hh:mm:ss"))
        End Try
    End Sub

End Class

Public Class LogWriter
    Inherits Logger

    Protected Output As StreamWriter
    Protected LastDate As Date = #1/1/2007#
    Protected Filename As String = ""

    Protected Sub CreateNewFile()
        LastDate = Now.Date
        Output = New StreamWriter(String.Format("{0}-{1}.log", Filename, Format(LastDate, "yyyy-MM-dd")), True)
        Output.AutoFlush = True

        WriteLine(LogType.INFORMATION, "Log started successfully.")
    End Sub

    Public Sub New(ByVal filename_ As String)
        Filename = filename_
        CreateNewFile()
    End Sub

    Private _disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not _disposedValue Then
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
            Output.Close()
        End If
        _disposedValue = True
    End Sub

    Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return
        If LastDate <> Now.Date Then CreateNewFile()

        Output.Write(formatStr, arg)
    End Sub

    Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return
        If LastDate <> Now.Date Then CreateNewFile()

        Output.WriteLine(L(type) & ":[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr, arg)
    End Sub

End Class

Public Class ConsoleWriter
    Inherits Logger

    Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        Console.Write(formatStr, arg)
    End Sub
    Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        Console.WriteLine(L(type) & ":" & "[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr, arg)
    End Sub

End Class