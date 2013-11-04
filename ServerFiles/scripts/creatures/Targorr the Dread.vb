Imports System
Imports System.Threading
Imports mangosVB.WorldServer
Imports mangosVB.Common

Namespace Scripts
    Public Class CreatureAI_Targorr_the_Dread
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const ThrashCD As Integer = 7000
        Private Const FrenzyCD As Integer = 90000 'This should never be reused.

        Private Const Spell_Frenzy As Integer = 8599
        Private Const Spell_Thrash As Integer = 3391

        Public NextThrash As Integer = 0
        Public NextWaypoint As Integer = 0
        Public NextAcid As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()

            NextThrash -= AI_UPDATE

            If NextThrash <= 0 Then
                NextThrash = ThrashCD
                aiCreature.CastSpellOnSelf(Spell_Thrash) 'Should be cast on self. Correct me if wrong.
            End If
        End Sub

        Public Sub CastThrash()
            For i As Integer = 0 To 0
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpellOnSelf(Spell_Thrash)
                Catch ex As Exception
                    aiCreature.SendChatMessage("AI was unable to cast Thrash on himself. Please report this to a developer.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 40 Then
                Try
                    aiCreature.CastSpellOnSelf(Spell_Frenzy)
                Catch ex As Exception
                    aiCreature.SendChatMessage("AI was unable to cast Frenzy on himself. Please report this to a developer.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            End If
        End Sub
    End Class
End Namespace