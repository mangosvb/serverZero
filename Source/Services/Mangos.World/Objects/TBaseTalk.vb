Imports Mangos.Common

Public Class TBaseTalk
    Public Overridable Sub OnGossipHello(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)

    End Sub
    Public Overridable Sub OnGossipSelect(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal selected As Integer)

    End Sub
    Public Overridable Function OnQuestStatus(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong) As Integer
        Return QuestEnum.QuestgiverStatusFlag.DIALOG_STATUS_NONE
    End Function

    Public Overridable Function OnQuestHello(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong) As Boolean
        Return True
    End Function
End Class