'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Imports Mangos.Common
Imports Mangos.Common.Enums

Namespace Handlers

    <AttributeUsage(AttributeTargets.Method, Inherited:=False, AllowMultiple:=True)>
    Public Class ChatCommandAttribute
        Inherits Attribute

        Public Sub New(ByVal cmdName As String, Optional ByVal cmdHelp As String = "No information available.", Optional ByVal cmdAccess As MiscEnum.AccessLevel = AccessLevel.GameMaster)
            Me.cmdName = cmdName
            Me.cmdHelp = cmdHelp
            Me.cmdAccess = cmdAccess
        End Sub

        Public Property cmdName() As String = ""
        Public Property cmdHelp() As String = "No information available."
        Public Property cmdAccess() As AccessLevel = AccessLevel.GameMaster

    End Class
End NameSpace