'
' Copyright (C) 2013-2021 getMaNGOS <https://getmangos.eu>
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

Imports Mangos.World.Objects
Imports Mangos.World.Player

Namespace Quests
    Public Class WS_QuestsBaseScripted
        Inherits WS_QuestsBase
        Public Overridable Sub OnQuestStart(ByRef objCharacter As WS_PlayerData.CharacterObject)
        End Sub
        Public Overridable Sub OnQuestComplete(ByRef objCharacter As WS_PlayerData.CharacterObject)
        End Sub
        Public Overridable Sub OnQuestCancel(ByRef objCharacter As WS_PlayerData.CharacterObject)
        End Sub

        Public Overridable Sub OnQuestItem(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal ItemID As Integer, ByVal ItemCount As Integer)
        End Sub
        Public Overridable Sub OnQuestKill(ByRef objCharacter As WS_PlayerData.CharacterObject, ByRef Creature As WS_Creatures.CreatureObject)
        End Sub
        Public Overridable Sub OnQuestCastSpell(ByRef objCharacter As WS_PlayerData.CharacterObject, ByRef Creature As WS_Creatures.CreatureObject, ByVal SpellID As Integer)
        End Sub
        Public Overridable Sub OnQuestCastSpell(ByRef objCharacter As WS_PlayerData.CharacterObject, ByRef GameObject As WS_GameObjects.GameObjectObject, ByVal SpellID As Integer)
        End Sub
        Public Overridable Sub OnQuestExplore(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal AreaID As Integer)
        End Sub
        Public Overridable Sub OnQuestEmote(ByRef objCharacter As WS_PlayerData.CharacterObject, ByRef Creature As WS_Creatures.CreatureObject, ByVal EmoteID As Integer)
        End Sub
    End Class
End Namespace