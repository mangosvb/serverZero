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

Imports System.Threading
Imports Mangos.Common.Enums.Global

Namespace Logging
    Public Class ConsoleWriter
        Inherits BaseWriter

        Public Overrides Sub Write(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            Console.Write(formatStr, arg)
        End Sub
        Public Overrides Sub WriteLine(type As LogType, formatStr As String, ByVal ParamArray arg() As Object)
            If LogLevel > type Then Return

            Console.WriteLine(L(type) & ":" & "[" & Format(TimeOfDay, "hh:mm:ss") & "] " & formatStr, arg)
        End Sub

        Public Overrides Function ReadLine() As String
            Thread.Sleep(TimeSpan.FromMinutes(1))
            Return "info"
        End Function

    End Class
End Namespace