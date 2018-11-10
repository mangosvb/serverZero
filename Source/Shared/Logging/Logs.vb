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

Namespace NewLogging
    Public Class Log
        Private Shared ReadOnly logger As Logger

        'Public Shared Sub Start(ByVal logTypes As LogTypes, Optional ByVal logFile As LogFile = Nothing)
        '    New Logger() {LogTypes=logTypes}
        '    logger?.Start(logFile)
        'End Sub

        'Public Shared Sub Stop()
        '    logger?.Stop
        '    logger = Nothing
        'End Sub

        'Public Shared Sub Resume(ByVal logTypes As LogTypes)
        'End Sub

        Public Shared Sub Pause()
        End Sub

        Public Shared Sub Message(ByVal logType As LogTypes, ByVal text As String)
        End Sub

        Public Shared Sub NewLine()
        End Sub

        Public Shared Sub WaitForKey()
        End Sub

        Public Shared Sub Clear()
        End Sub
    End Class
End Namespace
