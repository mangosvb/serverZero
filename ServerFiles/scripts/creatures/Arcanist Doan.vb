Imports System
Imports System.Threading
Imports mangosVB.WorldServer

Namespace Scripts
    Public Class CreatureAI
        Inherits BossAI
        'AI TODO: Confirm AI sounds, fix AoE casts. Rest of the spell issues would be core problems.
        Private Const AI_UPDATE As Integer = 1000
        Private Const ARCANE_BUBBLE_CD As Integer = 500000 'This should never be recasted.
        Private Const DETONATION_CD As Integer = 500000 'This should never be recasted.
        Private Const Arcane_Explosion_CD As Integer = 6000
        Private Const Silence_CD As Integer = 9000
        Private Const Polymorph_CD As Integer = 15000

        Private Const SPELL_POLYMORPH As Integer = 13323
        Private Const SPELL_SILENCE As Integer = 8988
        Private Const SPELL_DETONATION As Integer = 9435
        Private Const SPELL_ARCANE_BUBBLE As Integer = 9438
        Private Const SPELL_ARCANE_EXPLOSION As Integer = 34517

        Public NextDetonation As Integer = 0 'Again, this should never be reused.
        Public NextArcaneBubble As Integer = 0 'Again, this should never be reused.
        Public NextExplosion As Integer = 0
        Public NextPolymorph As Integer = 0
        Public NextSilence As Integer = 0
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

            NextExplosion -= AI_UPDATE
            NextPolymorph -= AI_UPDATE
            NextSilence -= AI_UPDATE
            NextArcaneBubble -= AI_UPDATE
            NextDetonation -= AI_UPDATE

            If NextExplosion <= 0 Then  'Need to implement better AoE handling. This may be the reason for lack of boss casts with AoE spells.
                NextExplosion = Arcane_Explosion_CD
                aiCreature.CastSpell(SPELL_ARCANE_EXPLOSION, aiTarget)
            End If

            If NextPolymorph <= 0 Then
                NextPolymorph = Polymorph_CD
                aiCreature.CastSpell(SPELL_POLYMORPH, aiCreature.GetRandomTarget)
            End If

            If NextSilence <= 0 Then
                NextSilence = Silence_CD
                aiCreature.CastSpell(SPELL_SILENCE, aiCreature.GetRandomTarget)
            End If
            'No need to handle Detonation or Bubble here.
        End Sub
        Public Sub CastExplosion()
            For i As Integer = 0 To 2
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(SPELL_ARCANE_EXPLOSION, aiTarget)
                Catch ex As Exception
                    aiCreature.SendChatMessage("Failed to cast Arcane Explosion. Please report this to a developer!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub
        Public Sub CastPolymorph()
            For i As Integer = 1 To 2
                Dim target As BaseUnit = aiCreature.GetRandomTarget 'Finally learned how random target functions work. 
                If target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(SPELL_POLYMORPH, aiCreature.GetRandomTarget) 'Might not properly work.
                Catch ex As Exception
                    aiCreature.SendChatMessage("I was unable to cast polymorph. Please report this to a developer!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
                End Try
            Next
        End Sub
        Public Sub CastSilence()
            For i As Integer = 2 To 2
                Dim target As BaseUnit = aiCreature.GetRandomTarget
                If target Is Nothing Then Exit Sub
                Try
                    aiCreature.CastSpell(SPELL_SILENCE, aiCreature.GetRandomTarget)
                Catch ex As Exception

                End Try
            Next
        End Sub
        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If Percent <= 50 Then
                aiCreature.MoveToInstant(148.403458, -429.035919, 18.485929, 0.002225)
                aiCreature.CastSpellOnSelf(SPELL_ARCANE_BUBBLE)
            End If

            If aiCreature.HaveAura(SPELL_ARCANE_BUBBLE) Then 'Workaround for casting detonation.
                aiCreature.CastSpell(SPELL_DETONATION, aiTarget)
                aiCreature.SendPlaySound(5843, True) 'Should play sound when he says "Burn in righteous fire!"
                aiCreature.SendChatMessage("Burn in righteous fire!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
            End If
        End Sub
        Public Overrides Sub OnEnterCombat()
            MyBase.OnEnterCombat()
            aiCreature.SendChatMessage("You will not defile these mysteries!", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
            aiCreature.SendPlaySound(5842, True)
        End Sub
    End Class
End Namespace