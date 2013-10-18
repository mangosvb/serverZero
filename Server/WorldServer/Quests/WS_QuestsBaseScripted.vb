Public Class WS_QuestsBaseScripted
    Inherits WS_QuestsBase
    Public Overridable Sub OnQuestStart(ByRef c As CharacterObject)
    End Sub
    Public Overridable Sub OnQuestComplete(ByRef c As CharacterObject)
    End Sub
    Public Overridable Sub OnQuestCancel(ByRef c As CharacterObject)
    End Sub

    Public Overridable Sub OnQuestItem(ByRef c As CharacterObject, ByVal ItemID As Integer, ByVal ItemCount As Integer)
    End Sub
    Public Overridable Sub OnQuestKill(ByRef c As CharacterObject, ByRef Creature As CreatureObject)
    End Sub
    Public Overridable Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal SpellID As Integer)
    End Sub
    Public Overridable Sub OnQuestCastSpell(ByRef c As CharacterObject, ByRef GameObject As GameObjectObject, ByVal SpellID As Integer)
    End Sub
    Public Overridable Sub OnQuestExplore(ByRef c As CharacterObject, ByVal AreaID As Integer)
    End Sub
    Public Overridable Sub OnQuestEmote(ByRef c As CharacterObject, ByRef Creature As CreatureObject, ByVal EmoteID As Integer)
    End Sub
End Class
