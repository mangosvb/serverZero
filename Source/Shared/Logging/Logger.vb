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

Imports System.Collections.Generic
Imports System.Threading

Namespace NewLogging
    Public Class Logger
        'Public Shared LogTypeInfo As Dictionary(Of LogTypes, Tuple) = New Dictionary(Of LogTypes, Tuple)() {, LogTypes.None, Create(ConsoleColor.White, "")}

        Public Shared Info As Dictionary(Of LogTypes, Tuple)

        Public Shared Debug As Dictionary(Of LogTypes, Tuple)

        Public Shared Create As Dictionary(Of LogTypes, Tuple)

        Public Shared Trace As Dictionary(Of LogTypes, Tuple)

        Public Shared Warning As Dictionary(Of LogTypes, Tuple)

        Public Shared _Error As Dictionary(Of LogTypes, Tuple)

        Public Shared Panic As Dictionary(Of LogTypes, Tuple)

        Public Shared Unknown As Dictionary(Of LogTypes, Tuple)

        ' No volatile support for properties, let's use a private backing field.
        'Public Property LogTypes As LogTypes
        'End Property
    End Class

    'Public Sub Start(Optional ByVal logFile As LogFile = Nothing)
    '    Dim logQueue As String
    '    Dim isLogging = True

    '    While isLogging
    '        ' Do nothing if logging is turned off (LogTypes.None) & the log queue is empty, but continue the loop.
    '        If ((LogTypes = LogTypes.None) _
    '            OrElse Not logQueue.TryTake(Tuple, Log)) Then
    '            'TODO: Warning!!! continue If
    '        End If

    '        ' LogTypes.None is also used for empty/simple log lines (without timestamp, etc.).
    '        If (Log.Item1 <> LogTypes.None) Then
    '            Console.ForegroundColor = ConsoleColor.White
    '            Console.Write("{log.Item2} |")
    '            Console.ForegroundColor = LogTypeInfo(Log.Item1).Item1
    '            Console.Write(LogTypeInfo(Log.Item1).Item2)
    '            Console.ForegroundColor = ConsoleColor.White
    '            Console.WriteLine("| {log.Item3}")
    '            logFile?.WriteAsync("{log.Item2} |{LogTypeInfo[log.Item1].Item2}| {log.Item3}")
    '        Else
    '            Console.WriteLine(Log.Item3)
    '            logFile?.WriteAsync(Log.Item3)
    '        End If
    '    End While

    '    'Warning!!! Lambda constructs are not supported
    '    Dim logThread = New Thread() >= {}
    '    logThread.IsBackground = True
    '    logThread.Start()
    '    isLogging = (logThread.ThreadState = ThreadState.Running)
    'End Sub

    'Public Sub Stop()
    'End Sub

    'Public Sub Message(ByVal logType As LogTypes, ByVal text As String)
    'End Sub

    'Public Sub NewLine()
    'End Sub

    'Public Sub WaitForKey()
    'End Sub

    'Public Sub Clear()
    'End Sub

    'Private Sub SetLogger(ByVal type As LogTypes, ByVal text As String)
    '    If ((LogTypes And type) _
    '                        = type) Then
    '        If (type = LogTypes.None) Then
    '            logQueue.Add(Tuple.Create(type, "", text))
    '        Else
    '            logQueue.Add(Tuple.Create(type, Now.ToString("T"), text))
    '        End If
    '    End If
    'End Sub
End Namespace
