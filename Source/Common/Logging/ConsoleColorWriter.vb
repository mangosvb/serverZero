'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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
Imports System.Runtime.CompilerServices

'Using this logging type, all logs are displayed in console.
'Writting commands is done trought console.

Namespace Logging
    Public Class ColoredConsoleWriter
        Inherits BaseWriter

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            Select Case type
                Case LogType.NETWORK
                    Console.ForegroundColor = ConsoleColor.DarkGray
                Case LogType.DEBUG
                    Console.ForegroundColor = ConsoleColor.Gray
                Case LogType.INFORMATION
                    Console.ForegroundColor = ConsoleColor.White
                Case LogType.USER
                    Console.ForegroundColor = ConsoleColor.Blue
                Case LogType.SUCCESS
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                Case LogType.WARNING
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case LogType.FAILED
                    Console.ForegroundColor = ConsoleColor.Red
                Case LogType.CRITICAL
                    Console.ForegroundColor = ConsoleColor.DarkRed
                Case LogType.DATABASE
                    Console.ForegroundColor = ConsoleColor.DarkMagenta
            End Select

            If arg Is Nothing Then
                Console.Write(formatStr)
            Else
                Console.Write(formatStr, arg)
            End If
            Console.ForegroundColor = ConsoleColor.Gray
        End Sub

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            Select Case type
                Case LogType.NETWORK
                    Console.ForegroundColor = ConsoleColor.DarkGray
                Case LogType.DEBUG
                    Console.ForegroundColor = ConsoleColor.Gray
                Case LogType.INFORMATION
                    Console.ForegroundColor = ConsoleColor.White
                Case LogType.USER
                    Console.ForegroundColor = ConsoleColor.Blue
                Case LogType.SUCCESS
                    Console.ForegroundColor = ConsoleColor.DarkGreen
                Case LogType.WARNING
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case LogType.FAILED
                    Console.ForegroundColor = ConsoleColor.Red
                Case LogType.CRITICAL
                    Console.ForegroundColor = ConsoleColor.DarkRed
                Case LogType.DATABASE
                    Console.ForegroundColor = ConsoleColor.DarkMagenta
            End Select

            If arg Is Nothing Then
                Console.WriteLine("[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr)
            Else
                Console.WriteLine("[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr, arg)
            End If
            Console.ForegroundColor = ConsoleColor.Gray
        End Sub

    End Class
End Namespace