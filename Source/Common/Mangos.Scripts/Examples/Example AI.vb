Imports Mangos.Shared
Imports Mangos.WorldServer

'Example AI for combat. 
'TODO: Fix AoE spells on AIs and then insert it as an example into this.
Namespace Examples
    Public Class CreatureAI
        Inherits BossAI

        Private Const AI_UPDATE As Integer = 1000
        Private Const Knockdown_CD As Integer = 5000 'Cooldown for spells get listed under a private constant. The timer is followed in milliseconds.

        Private Const Spell_Knockdown As Integer = 16790 'The spell is defined here. This is the equivilent of a MaNGOS C++ enumerator. This should work for NPCs and healing spells too.
        Private Const Frost_Armor As Integer = 7302 'Self buff.

        Public NextWaypoint As Integer = 0
        Public NextKnockdown As Integer = 0 'This will be called later, this is only needed along with the CD if you plan to have it recasted.
        Public CurrentWaypoint As Integer = 0

        Public Sub New(ByRef Creature As CreatureObject) 'The following under this are very self explanatory, on spawn the creature will not move by itself nor fly. It will be visible from far away. This can be changed.
            MyBase.New(Creature)
            AllowedMove = False
            Creature.Flying = False
            Creature.VisibleDistance = 700
        End Sub

        Public Overrides Sub OnThink()

            NextKnockdown -= AI_UPDATE 'The update is required for the AI to actually go on with anything.

            If NextKnockdown <= 0 Then 'This is the cooldown definition, this is not required to be used on a non-reused spell I believe.
                NextKnockdown = Knockdown_CD
                aiCreature.CastSpell(Spell_Knockdown, aiTarget) 'aiTarget can be changed to select a random target by making it "aiCreature.GetRandomTarget".
            End If
        End Sub

        Public Sub CastKnockdown() 'This is where the spell is brought into actual usage. 
            For i As Integer = 0 To 3 ' I believe this number is capped by the amount of spells from 0-any number. We'll make it 0 to 3 here just to be safe. You should do the same.
                Dim Target As BaseUnit = aiCreature
                If Target Is Nothing Then Exit Sub 'If no player is targetted, don't cast knockdown.
                aiCreature.CastSpell(Spell_Knockdown, aiTarget) 'The casting of the spell. This will be casted on the selected target as defined previously.
            Next
        End Sub

        Public Overrides Sub OnEnterCombat() 'Override sub is self explanatory. This is on the AIs entering of combat. Put anything here, for this we'll put a self buff.
            MyBase.OnEnterCombat()
            aiCreature.SendChatMessage("I have been pulled, so I send this message if I am inserted within the entercombat sub.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL) 'ChatMsg refers to the chat type, this can be changed to say, bg or anything else. Lang_Global is a global langauge that ALL can understand, this can be changed to any language. (Draenei, gnomish, etc.)
            aiCreature.CastSpellOnSelf(Frost_Armor) 'Casts frost armor upon self when entering combat.
        End Sub

        Public Overrides Sub OnDeath() 'Same as enter combat, except casting on self during death will probably lead to a core crash.
            MyBase.OnDeath()
            aiCreature.SendChatMessage("Y u do dat. I'm dead and tell you this because my sent chat message was set under the OnDeath() sub.", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
        End Sub

        Public Overrides Sub OnHealthChange(Percent As Integer) 'gets alot more tricky here. Just follow along and you'll understand.
            MyBase.OnHealthChange(Percent)
            If Percent <= 50 Then 'If health is equal to or less than 50, it'll continue with the code. Otherwise it won't.
                aiCreature.SendChatMessage("You have reduced me to 50% health. I will try harder now to ensure your death. Good job on inserting this into the onhealthchange percent sub though. That's why I'm talking! =]", ChatMsg.CHAT_MSG_YELL, LANGUAGES.LANG_GLOBAL)
            End If
        End Sub
    End Class
End Namespace
