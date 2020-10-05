Namespace Enums.Spell
    Public Enum SpellInterruptFlags As Integer
        SPELL_INTERRUPT_FLAG_MOVEMENT = &H1 ' why need this for instant?
        SPELL_INTERRUPT_FLAG_PUSH_BACK = &H2 ' push back
        SPELL_INTERRUPT_FLAG_INTERRUPT = &H4 ' interrupt
        SPELL_INTERRUPT_FLAG_AUTOATTACK = &H8 ' no
        SPELL_INTERRUPT_FLAG_DAMAGE = &H10  ' _complete_ interrupt on direct damage?
    End Enum
End NameSpace