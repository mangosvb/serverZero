Namespace Enums.Spell
    Public Enum SpellAttributes As Integer
        SPELL_ATTR_NONE = &H0
        SPELL_ATTR_RANGED = &H2
        SPELL_ATTR_NEXT_ATTACK = &H4
        SPELL_ATTR_IS_ABILITY = &H8 'Means this is not a "magic spell"
        SPELL_ATTR_UNK1 = &H10
        SPELL_ATTR_IS_TRADE_SKILL = &H20
        SPELL_ATTR_PASSIVE = &H40
        SPELL_ATTR_NO_VISIBLE_AURA = &H80 'Does not show in buff/debuff pane. normally passive buffs
        SPELL_ATTR_UNK2 = &H100
        SPELL_ATTR_TEMP_WEAPON_ENCH = &H200
        SPELL_ATTR_NEXT_ATTACK2 = &H400
        SPELL_ATTR_UNK3 = &H800
        SPELL_ATTR_ONLY_DAYTIME = &H1000 'Only usable at day
        SPELL_ATTR_ONLY_NIGHT = &H2000 'Only usable at night
        SPELL_ATTR_ONLY_INDOOR = &H4000 'Only usable indoors
        SPELL_ATTR_ONLY_OUTDOOR = &H8000 'Only usable outdoors
        SPELL_ATTR_NOT_WHILE_SHAPESHIFTED = &H10000 'Not while shapeshifted
        SPELL_ATTR_REQ_STEALTH = &H20000 'Requires stealth
        SPELL_ATTR_UNK4 = &H40000
        SPELL_ATTR_SCALE_DMG_LVL = &H80000 'Scale the damage with the caster's level
        SPELL_ATTR_STOP_ATTACK = &H100000 'Stop attack after use this spell (and not begin attack if use)
        SPELL_ATTR_CANT_BLOCK = &H200000 'This attack cannot be dodged, blocked, or parried
        SPELL_ATTR_UNK5 = &H400000
        SPELL_ATTR_WHILE_DEAD = &H800000 'Castable while dead
        SPELL_ATTR_WHILE_MOUNTED = &H1000000 'Castable while mounted
        SPELL_ATTR_COOLDOWN_AFTER_FADE = &H2000000 'Activate and start cooldown after aura fade or remove summoned creature or go
        SPELL_ATTR_UNK6 = &H4000000
        SPELL_ATTR_WHILE_SEATED = &H8000000 'Castable while seated
        SPELL_ATTR_NOT_WHILE_COMBAT = &H10000000 'Set for all spells that are not allowed during combat
        SPELL_ATTR_IGNORE_IMMUNE = &H20000000 'Ignore all immune effects
        SPELL_ATTR_BREAKABLE_BY_DAMAGE = &H40000000 'Breakable by damage
        SPELL_ATTR_CANT_REMOVE = &H80000000 'Positive Aura but cannot right click to remove
    End Enum
End NameSpace