Imports MangosVB.Common
Imports MangosVB.Shared
Imports MangosVB.WorldServer

Namespace Creatures
    Public Class CreatureAI_Druid_of_the_Fang
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const SLUMBER_CD As Integer = 10000 '- Unable to implement this as for the time being due to threat issues in the core.
        Private Const Healing_Touch_CD As Integer = 20000
        Private Const Serpent_Form_CD As Integer = 40000
        Private Const Lightning_Bolt_CD As Integer = 6000

        Private Const Slumber_Spell As Integer = 8040
        Private Const Healing_Spell As Integer = 23381
        Private Const Spell_Serpent_Form As Integer = 8041 'Not sure how this will work. 
        Private Const Spell_Lightning_Bolt As Integer = 9532
        Public NextWaypoint As Integer = 0
        Public NextLightningBolt As Integer = 0
        Public NextSerpentForm As Integer = 0
        Public NextHealingTouch As Integer = 0
        Public NextSlumber As Integer = 0
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnThink()

            NextLightningBolt -= AI_UPDATE
            NextSerpentForm -= AI_UPDATE
            NextHealingTouch -= AI_UPDATE
            NextSlumber -= AI_UPDATE

            If NextLightningBolt <= 0 Then
                NextLightningBolt = Lightning_Bolt_CD
                aiCreature.CastSpell(Spell_Lightning_Bolt, aiTarget) 'Lightning bolt on current target.
            End If
        End Sub

        Public Sub CastLightning()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Lightning_Bolt, aiTarget)
            Next
        End Sub

        Public Overrides Sub OnHealthChange(ByVal Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 30 Then
                Try
                    aiCreature.CastSpellOnSelf(Spell_Serpent_Form)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast Serpent Form. This is a problem. Please report this to the developers.", ChatMsg.CHAT_MSG_MONSTER_YELL, LANGUAGES.LANG_UNIVERSAL)
                End Try
            End If
        End Sub
    End Class
End Namespace
