Imports System
Imports System.Threading
Imports MangosVB.WorldServer
Imports mangosVB.Common

Namespace Scripts
    Public Class CreatureAI_Lord_Cobrahn
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const SLUMBER_CD As Integer = 10000
        Private Const Lightning_Bolt_CD As Integer = 6000
        Private Const Poison_CD As Integer = 9000

        Private Const Cobrahn_Serpent_Form_Spell As Integer = 7965
        Private Const Poison_Spell As Integer = 744
        Private Const Slumber_Spell As Integer = 8040
        Private Const Healing_Spell As Integer = 23381
        Private Const Spell_Serpent_Form As Integer = 8041 'Not sure how this will work. 
        Private Const Spell_Lightning_Bolt As Integer = 9532
        Public NextPoison As Integer = 0
        Public NextLightningBolt As Integer = 0
        Public NextSerpentTransform As Integer = 0
        Public NextSlumber As Integer = 0
        Public NextHealingTouch As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnEnterCombat()
            MyBase.OnEnterCombat()
            aiCreature.SendChatMessage("You will never wake the dreamer!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL) 'If you can do anything, then go serpent form.
        End Sub

        Public Overrides Sub OnThink()
            NextLightningBolt -= AI_UPDATE
            NextSlumber -= AI_UPDATE
            NextPoison -= AI_UPDATE
            NextSerpentTransform -= AI_UPDATE

            If NextLightningBolt <= 0 Then
                NextLightningBolt = Lightning_Bolt_CD
                aiCreature.CastSpell(Spell_Lightning_Bolt, aiTarget) 'Lightning bolt on current target.
            End If

            If NextSlumber <= 0 Then
                NextSlumber = SLUMBER_CD
                aiCreature.CastSpell(Slumber_Spell, aiCreature.GetRandomTarget)
            End If

            If NextPoison <= 0 Then
                NextPoison = Poison_CD
                aiCreature.CastSpell(Poison_Spell, aiTarget) 'Should this be random target?
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

        Public Sub CastPoison()
            For i As Integer = 2 To 3
                Dim target As BaseUnit = aiCreature
                If target Is Nothing Then Exit Sub
            Next
            aiCreature.CastSpell(Poison_Spell, aiTarget)
        End Sub

        'This may not work, unsure on how to add two health conditions.
        Public Sub OnHealthChange2(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 45 Then
                Try
                    aiCreature.CastSpellOnSelf(Cobrahn_Serpent_Form_Spell)
                Catch ex As Exception
                    aiCreature.SendChatMessage("I have failed to cast Serpent Form on myself. This is a problem. Please report this issue to the developers of MaNGOS VB.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            End If
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