
Imports mangosVB.Shared
Imports mangosVB.WorldServer

'Summon implementation isn't yet supported.
'Sand trap not implemented into script, need to make a gameobject I assume.
Namespace Creatures
    Public Class CreatureAI_Kurinnax
        Inherits BossAI
        Private Const AI_UPDATE As Integer = 1000
        Private Const wound_cooldown As Integer = 8000
        'private const summon_player_cooldown As Integer = 5000 
        'Private Const summon_player_cooldown2 As Integer = 5001
        'Not sure if has correct core support or on cooldowns.
        Private Const Thrash_Cooldown As Integer = 9000
        Private Const Wide_Slash_Cooldown As Integer = 10000

        Private Const Spell_Frenzy As Integer = 26527
        Private Const Spell_Mortal_Wound As Integer = 25646
        ' Private Const Spell_Summon_1 As Integer = 20477 'Unused until we figure out how this works and what it does.
        ' Private Const Spell_Summon_2 As Integer = 26446 'Same as above, unused until more is figured out.
        Private Const spell_Thrash As Integer = 3391
        Private Const Spell_Wide_Slash As Integer = 25814


        Public phase As Integer = 0
        Public Next_Mortal_Wound As Integer = 0
        Public Next_Thrash As Integer = 0
        Public Next_Wide_Slash As Integer = 0
        ' Public Next_Summon_1 As Integer = 0
        ' Public Next_Summon_2 As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            phase = 0
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = (700)
        End Sub

        Public Overrides Sub OnEnterCombat()
            If phase > 1 Then Exit Sub
            MyBase.OnEnterCombat()
            AllowedAttack = True
            aiCreature.Flying = False
            'ReinitSpells()
        End Sub

        Public Overrides Sub OnLeaveCombat(Optional Reset As Boolean = True)
            MyBase.OnLeaveCombat(Reset)
            AllowedAttack = True

            phase = 0

        End Sub

        Public Overrides Sub OnThink()
            MyBase.OnThink()
            If phase < 1 Then Exit Sub

            If (phase = 1 OrElse phase = 3) Then

            End If
            Next_Mortal_Wound -= AI_UPDATE
            Next_Thrash -= AI_UPDATE
            Next_Wide_Slash -= AI_UPDATE


            If Next_Mortal_Wound <= 0 Then
                Next_Mortal_Wound = wound_cooldown
                Try
                    aiCreature.CastSpell(Spell_Mortal_Wound, aiTarget)
                Catch Ex As Exception
                    Log.WriteLine(LogType.WARNING, "Mortal Wound failed to cast!")
                End Try
            End If
            If Next_Thrash <= 1 Then
                Next_Thrash = Thrash_Cooldown
                aiCreature.CastSpell(spell_Thrash, aiTarget)

            End If
            If Next_Wide_Slash <= 2 Then
                Next_Wide_Slash = Wide_Slash_Cooldown
                aiCreature.CastSpell(spell_Thrash, aiTarget)
            End If
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer)
            MyBase.OnHealthChange(Percent)
            If phase = 1 Then
                If Percent <= 20 Then
                    aiCreature.CastSpellOnSelf(Spell_Frenzy)
                End If
            End If
        End Sub
    End Class
End Namespace