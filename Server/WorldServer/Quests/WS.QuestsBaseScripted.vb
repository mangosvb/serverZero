Public Class WS_QuestsBaseScripted
    Inherits WS_QuestsBase
    Public Overridable Sub OnQuestStart(ByRef objCharacter As CharacterObject)
    End Sub
    Public Overridable Sub OnQuestComplete(ByRef objCharacter As CharacterObject)
    End Sub
    Public Overridable Sub OnQuestCancel(ByRef objCharacter As CharacterObject)
    End Sub

    Public Overridable Sub OnQuestItem(ByRef objCharacter As CharacterObject, ByVal ItemID As Integer, ByVal ItemCount As Integer)
    End Sub
    Public Overridable Sub OnQuestKill(ByRef objCharacter As CharacterObject, ByRef Creature As CreatureObject)
    End Sub
    Public Overridable Sub OnQuestCastSpell(ByRef objCharacter As CharacterObject, ByRef Creature As CreatureObject, ByVal SpellID As Integer)
    End Sub
    Public Overridable Sub OnQuestCastSpell(ByRef objCharacter As CharacterObject, ByRef GameObject As GameObjectObject, ByVal SpellID As Integer)
    End Sub
    Public Overridable Sub OnQuestExplore(ByRef objCharacter As CharacterObject, ByVal AreaID As Integer)
    End Sub
    Public Overridable Sub OnQuestEmote(ByRef objCharacter As CharacterObject, ByRef Creature As CreatureObject, ByVal EmoteID As Integer)
    End Sub
End Class
