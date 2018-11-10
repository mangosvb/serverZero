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

Imports System.IO
Imports System.Text

Namespace NewLogging
    Public NotInheritable Class LogFile

        Private ReadOnly logStream As FileStream

        Public Sub New(ByVal directory As String, ByVal file As String)
            MyBase.New
            logStream = New FileStream("{directory}/{DateTime.Now:yyyy-MM-dd_hh-mm-ss}_{file}", FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, True)
        End Sub

        Public Sub Write(ByVal logMessage As String)
            Dim logBytes = Encoding.UTF8.GetBytes("{logMessage}" & vbLf)
            logStream.Write(logBytes, 0, logBytes.Length)
            logStream.Flush()
        End Sub

        Public Sub Dispose()
            logStream.Close()
        End Sub
    End Class
End Namespace
