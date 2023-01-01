'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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

Namespace Objects
    Public Class GossipMenu
        Public Sub AddMenu(ByVal menu As String, Optional ByVal icon As Byte = 0, Optional ByVal isCoded As Byte = 0, Optional ByVal cost As Integer = 0, Optional ByVal WarningMessage As String = "")
            Icons.Add(icon)
            Menus.Add(menu)
            Coded.Add(isCoded)
            Costs.Add(cost)
            WarningMessages.Add(WarningMessage)
        End Sub
        Public Icons As New ArrayList
        Public Menus As New ArrayList
        Public Coded As New ArrayList
        Public Costs As New ArrayList
        Public WarningMessages As New ArrayList
    End Class
End NameSpace