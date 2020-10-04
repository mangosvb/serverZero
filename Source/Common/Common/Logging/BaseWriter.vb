'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getMangos.co.uk>
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

Namespace Logging

    Public Class BaseWriter
        Implements IDisposable

        Public L() As Char = {"N", "D", "I", "U", "S", "W", "F", "C", "DB"}

        Public LogLevel As LogType = LogType.NETWORK

        Public Sub New()
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
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

        ''' <summary>
        ''' Writes the text to the console, typically does not privide a carridge return. (Overridable)
        ''' </summary>
        ''' <param name="type">The type.</param>
        ''' <param name="format">The format.</param>
        ''' <param name="arg">The arg.</param>
        ''' <returns></returns>
        Public Overridable Sub Write(type As LogType, format As String, ByVal ParamArray arg() As Object)
        End Sub

        ''' <summary>
        ''' Writes the line to the console. (Overridable)
        ''' </summary>
        ''' <param name="type">The type.</param>
        ''' <param name="format">The format.</param>
        ''' <param name="arg">The arg.</param>
        ''' <returns></returns>
        Public Overridable Sub WriteLine(type As LogType, format As String, ByVal ParamArray arg() As Object)
        End Sub

        ''' <summary>
        ''' Reads the line from the console. (Overridable)
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function ReadLine() As String
            Return Console.ReadLine()
        End Function

        ''' <summary>
        ''' Prints the diagnostic test for all the different settings - do not remove.
        ''' </summary>
        ''' <returns></returns>
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

        ''' <summary>
        ''' Creates the log instance.
        ''' </summary>
        ''' <param name="logType">Type of the log.</param>
        ''' <param name="logConfig">The log config.</param>
        ''' <param name="log">The log.</param>
        ''' <returns></returns>
        Public Shared Sub CreateLog(logType As String, logConfig As String, ByRef log As BaseWriter)
            Try
                Select Case UppercaseFirstLetter(logType)
                    Case "COLORCONSOLE"
                        log = New ColoredConsoleWriter
                    Case "CONSOLE"
                        log = New ConsoleWriter
                    Case "FILE"
                        log = New FileWriter(logConfig)
                End Select
            Catch e As Exception
                Console.WriteLine("[{0}] Error creating log output!" & vbNewLine & e.ToString, Format(TimeOfDay, "hh:mm:ss"))
            End Try
        End Sub

    End Class
End Namespace