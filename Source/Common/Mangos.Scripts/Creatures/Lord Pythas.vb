Imports Mangos.Shared
Imports Mangos.WorldServer

Namespace Creatures
    Public Class CreatureAI_Lord_Pythas
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const SLUMBER_CD As Integer = 10000
        Private Const Lightning_Bolt_CD As Integer = 6000
        Private Const Thunder_Clap_CD As Integer = 9000

        Private Const Thunderclap_Spell As Integer = 8147
        Private Const Slumber_Spell As Integer = 8040
        Private Const Healing_Spell As Integer = 23381
        Private Const Spell_Serpent_Form As Integer = 8041 'Not sure how this will work. 
        Private Const Spell_Lightning_Bolt As Integer = 9532
        Public NextWaypoint As Integer = 0
        Public NextThunderClap As Integer = 0
        Public NextLightningBolt As Integer = 0
        Public NextSlumber As Integer = 0
        Public CurrentWaypoint As Integer = 0
        Public NextHealingTouch As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub
        Public Overrides Sub OnEnterCombat()
            MyBase.OnEnterCombat()
            aiCreature.SendChatMessage("The coils of death... Will crush you.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL) 'If you can do anything, then go serpent form.
        End Sub
        Public Overrides Sub OnThink()

            NextLightningBolt -= AI_UPDATE
            NextHealingTouch -= AI_UPDATE
            NextSlumber -= AI_UPDATE
            NextThunderClap -= AI_UPDATE

            If NextLightningBolt <= 0 Then
                NextLightningBolt = Lightning_Bolt_CD
                aiCreature.CastSpell(Spell_Lightning_Bolt, aiTarget) 'Lightning bolt on current target.
            End If
            If NextSlumber <= 0 Then
                NextSlumber = SLUMBER_CD
                aiCreature.CastSpell(Slumber_Spell, aiCreature.GetRandomTarget)
            End If
            If NextThunderClap <= 0 Then
                NextThunderClap = Thunder_Clap_CD
                aiCreature.CastSpell(Thunderclap_Spell, aiTarget) 'TODO: Make this an AoE cast.
            End If
        End Sub

        Public Sub CastLightning()
            For i As Integer = 0 To 3
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                aiCreature.CastSpell(Spell_Lightning_Bolt, aiTarget)
            Next
        End Sub
        Public Sub CastSlumber()
            For i As Integer = 1 To 3
                Dim target As BaseUnit = aiCreature.GetRandomTarget
                If target Is Nothing Then Exit Sub
            Next
            aiCreature.CastSpell(Slumber_Spell, aiCreature.GetRandomTarget)
        End Sub
        Public Sub CastThunderClap()
            For i As Integer = 2 To 3
                Dim target As BaseUnit = aiCreature
                If target Is Nothing Then Exit Sub
            Next
            aiCreature.CastSpell(Thunder_Clap_CD, aiTarget)
        End Sub
        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 10 Then
                Try
                    aiCreature.CastSpellOnSelf(Healing_Spell)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I was unable to cast healing touch on myself. This is a problem. Please report this to the developers.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            End If
        End Sub
    End Class
End Namespace