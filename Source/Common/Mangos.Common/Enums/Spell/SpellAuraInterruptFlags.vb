Namespace Enums.Spell
    Public Enum SpellAuraInterruptFlags As Integer
        AURA_INTERRUPT_FLAG_HOSTILE_SPELL_INFLICTED = &H1 'removed when recieving a hostile spell?
        AURA_INTERRUPT_FLAG_DAMAGE = &H2 'removed by any damage
        AURA_INTERRUPT_FLAG_CC = &H4 'removed by crowd control
        AURA_INTERRUPT_FLAG_MOVE = &H8 'removed by any movement
        AURA_INTERRUPT_FLAG_TURNING = &H10 'removed by any turning
        AURA_INTERRUPT_FLAG_ENTER_COMBAT = &H20 'removed by entering combat
        AURA_INTERRUPT_FLAG_NOT_MOUNTED = &H40 'removed by unmounting
        AURA_INTERRUPT_FLAG_SLOWED = &H80 'removed by being slowed
        AURA_INTERRUPT_FLAG_NOT_UNDERWATER = &H100 'removed by leaving water
        AURA_INTERRUPT_FLAG_NOT_SHEATHED = &H200 'removed by unsheathing
        AURA_INTERRUPT_FLAG_TALK = &H400 'removed by action to NPC
        AURA_INTERRUPT_FLAG_USE = &H800 'removed by action to GameObject
        AURA_INTERRUPT_FLAG_START_ATTACK = &H1000 'removed by attacking
        AURA_INTERRUPT_FLAG_UNK4 = &H2000
        AURA_INTERRUPT_FLAG_UNK5 = &H4000
        AURA_INTERRUPT_FLAG_CAST_SPELL = &H8000 'removed at spell cast
        AURA_INTERRUPT_FLAG_UNK6 = &H10000
        AURA_INTERRUPT_FLAG_MOUNTING = &H20000 'removed by mounting
        AURA_INTERRUPT_FLAG_NOT_SEATED = &H40000 'removed by standing up
        AURA_INTERRUPT_FLAG_CHANGE_MAP = &H80000 'leaving map/getting teleported
        AURA_INTERRUPT_FLAG_INVINCIBLE = &H100000 'removed when invicible
        AURA_INTERRUPT_FLAG_STEALTH = &H200000 'removed by stealth
        AURA_INTERRUPT_FLAG_UNK7 = &H400000
    End Enum
End NameSpace