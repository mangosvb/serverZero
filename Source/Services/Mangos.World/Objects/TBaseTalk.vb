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

Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Quest
Imports Mangos.World.Player

Namespace Objects

    Public Class TBaseTalk
        Public Overridable Sub OnGossipHello(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal cGUID As ULong)

        End Sub
        Public Overridable Sub OnGossipSelect(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal selected As Integer)

        End Sub
        Public Overridable Function OnQuestStatus(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong) As Integer
            Return QuestgiverStatusFlag.DIALOG_STATUS_NONE
        End Function

        Public Overridable Function OnQuestHello(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong) As Boolean
            Return True
        End Function
    End Class
End NameSpace