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
Public Class BaseWriter
    Implements IDisposable

    Public Enum LogType
        NETWORK                 'Network code debugging
        DEBUG                   'Packets processing
        INFORMATION             'User information
        USER                    'User actions
        SUCCESS                 'Normal operation
        WARNING                 'Warning
        FAILED                  'Processing Error
        CRITICAL                'Application Error
        DATABASE                'Database Error
    End Enum
    Public L() As Char = {"N", "D", "I", "U", "S", "W", "F", "C", "DB"}

    Public LogLevel As LogType = LogType.NETWORK

    Public Sub New()
    End Sub

#Region "IDisposable Support"
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
#End Region

    Public Overridable Sub Write(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
    End Sub
    Public Overridable Sub WriteLine(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
    End Sub
    Public Overridable Function ReadLine() As String
        Return Console.ReadLine()
    End Function

    Public Sub PrintDiagnosticTest()
        WriteLine(LogType.NETWORK, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.DEBUG, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.INFORMATION, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.USER, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.SUCCESS, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.WARNING, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.FAILED, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.CRITICAL, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.DATABASE, "{0}:************************* TEST *************************", 1)
    End Sub
    Public Shared Sub CreateLog(ByVal LogType As String, ByVal LogConfig As String, ByRef Log As BaseWriter)
        Try
            Select Case UCase(LogType)
                Case "COLORCONSOLE"
                    Log = New ColoredConsoleWriter
                Case "CONSOLE"
                    Log = New ConsoleWriter
                Case "FILE"
                    Log = New FileWriter(LogConfig)
                Case "TELNET"
                    Dim info As String() = Split(LogConfig, ":")
                    Log = New TelnetWriter(System.Net.IPAddress.Parse(info(0)), info(1))
                Case "IRC"
                    Dim info As String() = Split(LogConfig, ":")
                    Dim server As String = info(0)
                    Dim port As String = info(1)
                    Dim nick As String = info(2)
                    Dim channel As String = info(3)
                    Log = New IrcWriter(server, port, nick, channel)
            End Select
        Catch e As Exception
            Console.WriteLine("[{0}] Error creating log output!" & vbNewLine & e.ToString, Format(TimeOfDay, "hh:mm:ss"))
        End Try
    End Sub

End Class
