'
' Copyright (C) 2013 - 2018 getMaNGOS <https://getmangos.eu>
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Public Module Spell_Stuff

#Region "Spells.SpellEffects"
    Enum ManaTypes As Integer
        TYPE_MANA = 0
        TYPE_RAGE = 1
        TYPE_FOCUS = 2
        TYPE_ENERGY = 3
        TYPE_HAPPINESS = 4
        TYPE_HEALTH = -2
    End Enum

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
        SPELL_ATTR_MOVEMENT_IMPAIRING = &H40000000 'Effect movement in a negative way
        SPELL_ATTR_CANT_REMOVE = &H80000000 'Positive Aura but cannot right click to remove
    End Enum
    Public Enum SpellAttributesEx As Integer
        SPELL_ATTR_EX_DRAIN_ALL_POWER = &H2 'use all power (Only paladin Lay of Hands and Bunyanize)
        SPELL_ATTR_EX_CHANNELED_1 = &H4 'channeled 1
        SPELL_ATTR_EX_NOT_BREAK_STEALTH = &H20 'Not break stealth
        SPELL_ATTR_EX_CHANNELED_2 = &H40 'channeled 2
        SPELL_ATTR_EX_NEGATIVE = &H80 'negative spell?
        SPELL_ATTR_EX_NOT_IN_COMBAT_TARGET = &H100 'Spell req target not to be in combat state
        SPELL_ATTR_EX_NOT_PASSIVE = &H400 'not passive? (if this flag is set and SPELL_PASSIVE is set in Attributes it shouldn't be counted as a passive?)
        SPELL_ATTR_EX_DISPEL_AURAS_ON_IMMUNITY = &H8000 'remove auras on immunity
        SPELL_ATTR_EX_UNAFFECTED_BY_SCHOOL_IMMUNE = &H10000 'unaffected by school immunity
        SPELL_ATTR_EX_REQ_COMBO_POINTS1 = &H100000 'Req combo points on target
        SPELL_ATTR_EX_REQ_COMBO_POINTS2 = &H400000 'Req combo points on target
    End Enum
    Public Enum SpellAttributesEx2 As Integer
        SPELL_ATTR_EX2_AUTO_SHOOT = &H20 'Auto Shoot?
        SPELL_ATTR_EX2_HEALTH_FUNNEL = &H800 'Health funnel pets?
        SPELL_ATTR_EX2_NOT_NEED_SHAPESHIFT = &H80000 'does not necessarly need shapeshift
        SPELL_ATTR_EX2_CANT_CRIT = &H20000000 'Spell can't crit
    End Enum

    Public Enum SpellImplicitTargets As Byte
        TARGET_NOTHING = 0

        TARGET_SELF = 1
        TARGET_RANDOM_ENEMY_CHAIN_IN_AREA = 2           'Only one spell has this one, but regardless, it's a target type after all
        TARGET_PET = 5
        TARGET_CHAIN_DAMAGE = 6
        TARGET_AREAEFFECT_CUSTOM = 8
        TARGET_INNKEEPER_COORDINATES = 9                'Used in teleport to innkeeper spells
        TARGET_ALL_ENEMY_IN_AREA = 15
        TARGET_ALL_ENEMY_IN_AREA_INSTANT = 16
        TARGET_TABLE_X_Y_Z_COORDINATES = 17             'Used in teleport spells and some other
        TARGET_EFFECT_SELECT = 18                       'Highly depends on the spell effect
        TARGET_AROUND_CASTER_PARTY = 20
        TARGET_SELECTED_FRIEND = 21
        TARGET_AROUND_CASTER_ENEMY = 22                 'Used only in TargetA, target selection dependent from TargetB
        TARGET_SELECTED_GAMEOBJECT = 23
        TARGET_INFRONT = 24
        TARGET_DUEL_VS_PLAYER = 25                      'Used when part of spell is casted on another target
        TARGET_GAMEOBJECT_AND_ITEM = 26
        TARGET_MASTER = 27      'not tested
        TARGET_AREA_EFFECT_ENEMY_CHANNEL = 28
        TARGET_ALL_FRIENDLY_UNITS_AROUND_CASTER = 30    'In TargetB used only with TARGET_ALL_AROUND_CASTER and in self casting range in TargetA
        TARGET_ALL_FRIENDLY_UNITS_IN_AREA = 31
        TARGET_MINION = 32                              'Summons your pet to you.
        TARGET_ALL_PARTY = 33
        TARGET_ALL_PARTY_AROUND_CASTER_2 = 34           'Used in Tranquility
        TARGET_SINGLE_PARTY = 35
        TARGET_AREAEFFECT_PARTY = 37                    'Power infuses the target's party, increasing their Shadow resistance by $s1 for $d.
        TARGET_SCRIPT = 38
        TARGET_SELF_FISHING = 39                        'Equip a fishing pole and find a body of water to fish.
        TARGET_TOTEM_EARTH = 41
        TARGET_TOTEM_WATER = 42
        TARGET_TOTEM_AIR = 43
        TARGET_TOTEM_FIRE = 44
        TARGET_CHAIN_HEAL = 45
        TARGET_DYNAMIC_OBJECT = 47
        TARGET_AREA_EFFECT_SELECTED = 53                'Inflicts $s1 Fire damage to all enemies in a selected area.
        TARGET_UNK54 = 54
        TARGET_RANDOM_RAID_MEMBER = 56
        TARGET_SINGLE_FRIEND_2 = 57
        TARGET_AREAEFFECT_PARTY_AND_CLASS = 61
        TARGET_DUELVSPLAYER_COORDINATES = 63
        TARGET_BEHIND_VICTIM = 65                       ' uses in teleport behind spells
        TARGET_SINGLE_ENEMY = 77
        TARGET_SELF2 = 87
        TARGET_NONCOMBAT_PET = 90
    End Enum

    Public Enum ShapeshiftForm As Byte
        FORM_NORMAL = 0

        FORM_CAT = 1
        FORM_TREE = 2
        FORM_TRAVEL = 3
        FORM_AQUA = 4
        FORM_BEAR = 5
        FORM_AMBIENT = 6
        FORM_GHOUL = 7
        FORM_DIREBEAR = 8
        FORM_CREATUREBEAR = 14
        FORM_CREATURECAT = 15
        FORM_GHOSTWOLF = 16
        FORM_BATTLESTANCE = 17
        FORM_DEFENSIVESTANCE = 18
        FORM_BERSERKERSTANCE = 19
        FORM_SWIFT = 27
        FORM_SHADOW = 28
        FORM_FLIGHT = 29
        FORM_STEALTH = 30
        FORM_MOONKIN = 31
        FORM_SPIRITOFREDEMPTION = 32
    End Enum

    Public Enum SpellEffects_Names As Integer
        SPELL_EFFECT_NOTHING = 0
        SPELL_EFFECT_INSTAKILL = 1
        SPELL_EFFECT_SCHOOL_DAMAGE = 2
        SPELL_EFFECT_DUMMY = 3
        SPELL_EFFECT_PORTAL_TELEPORT = 4
        SPELL_EFFECT_TELEPORT_UNITS = 5
        SPELL_EFFECT_APPLY_AURA = 6
        SPELL_EFFECT_ENVIRONMENTAL_DAMAGE = 7
        SPELL_EFFECT_MANA_DRAIN = 8
        SPELL_EFFECT_HEALTH_LEECH = 9
        SPELL_EFFECT_HEAL = 10
        SPELL_EFFECT_BIND = 11
        SPELL_EFFECT_PORTAL = 12
        SPELL_EFFECT_RITUAL_BASE = 13
        SPELL_EFFECT_RITUAL_SPECIALIZE = 14
        SPELL_EFFECT_RITUAL_ACTIVATE_PORTAL = 15
        SPELL_EFFECT_QUEST_COMPLETE = 16
        SPELL_EFFECT_WEAPON_DAMAGE_NOSCHOOL = 17
        SPELL_EFFECT_RESURRECT = 18
        SPELL_EFFECT_ADD_EXTRA_ATTACKS = 19
        SPELL_EFFECT_DODGE = 20
        SPELL_EFFECT_EVADE = 21
        SPELL_EFFECT_PARRY = 22
        SPELL_EFFECT_BLOCK = 23
        SPELL_EFFECT_CREATE_ITEM = 24
        SPELL_EFFECT_WEAPON = 25
        SPELL_EFFECT_DEFENSE = 26
        SPELL_EFFECT_PERSISTENT_AREA_AURA = 27
        SPELL_EFFECT_SUMMON = 28
        SPELL_EFFECT_LEAP = 29
        SPELL_EFFECT_ENERGIZE = 30
        SPELL_EFFECT_WEAPON_PERCENT_DAMAGE = 31
        SPELL_EFFECT_TRIGGER_MISSILE = 32
        SPELL_EFFECT_OPEN_LOCK = 33
        SPELL_EFFECT_SUMMON_MOUNT_OBSOLETE = 34
        SPELL_EFFECT_APPLY_AREA_AURA = 35
        SPELL_EFFECT_LEARN_SPELL = 36
        SPELL_EFFECT_SPELL_DEFENSE = 37
        SPELL_EFFECT_DISPEL = 38
        SPELL_EFFECT_LANGUAGE = 39
        SPELL_EFFECT_DUAL_WIELD = 40
        SPELL_EFFECT_SUMMON_WILD = 41
        SPELL_EFFECT_SUMMON_GUARDIAN = 42
        SPELL_EFFECT_TELEPORT_UNITS_FACE_CASTER = 43
        SPELL_EFFECT_SKILL_STEP = 44
        SPELL_EFFECT_UNDEFINED_45 = 45
        SPELL_EFFECT_SPAWN = 46
        SPELL_EFFECT_TRADE_SKILL = 47
        SPELL_EFFECT_STEALTH = 48
        SPELL_EFFECT_DETECT = 49
        SPELL_EFFECT_SUMMON_OBJECT = 50
        SPELL_EFFECT_FORCE_CRITICAL_HIT = 51
        SPELL_EFFECT_GUARANTEE_HIT = 52
        SPELL_EFFECT_ENCHANT_ITEM = 53
        SPELL_EFFECT_ENCHANT_ITEM_TEMPORARY = 54
        SPELL_EFFECT_TAMECREATURE = 55
        SPELL_EFFECT_SUMMON_PET = 56
        SPELL_EFFECT_LEARN_PET_SPELL = 57
        SPELL_EFFECT_WEAPON_DAMAGE = 58
        SPELL_EFFECT_OPEN_LOCK_ITEM = 59
        SPELL_EFFECT_PROFICIENCY = 60
        SPELL_EFFECT_SEND_EVENT = 61
        SPELL_EFFECT_POWER_BURN = 62
        SPELL_EFFECT_THREAT = 63
        SPELL_EFFECT_TRIGGER_SPELL = 64
        SPELL_EFFECT_HEALTH_FUNNEL = 65
        SPELL_EFFECT_POWER_FUNNEL = 66
        SPELL_EFFECT_HEAL_MAX_HEALTH = 67
        SPELL_EFFECT_INTERRUPT_CAST = 68
        SPELL_EFFECT_DISTRACT = 69
        SPELL_EFFECT_PULL = 70
        SPELL_EFFECT_PICKPOCKET = 71
        SPELL_EFFECT_ADD_FARSIGHT = 72
        SPELL_EFFECT_SUMMON_POSSESSED = 73
        SPELL_EFFECT_SUMMON_TOTEM = 74
        SPELL_EFFECT_HEAL_MECHANICAL = 75
        SPELL_EFFECT_SUMMON_OBJECT_WILD = 76
        SPELL_EFFECT_SCRIPT_EFFECT = 77
        SPELL_EFFECT_ATTACK = 78
        SPELL_EFFECT_SANCTUARY = 79
        SPELL_EFFECT_ADD_COMBO_POINTS = 80
        SPELL_EFFECT_CREATE_HOUSE = 81
        SPELL_EFFECT_BIND_SIGHT = 82
        SPELL_EFFECT_DUEL = 83
        SPELL_EFFECT_STUCK = 84
        SPELL_EFFECT_SUMMON_PLAYER = 85
        SPELL_EFFECT_ACTIVATE_OBJECT = 86
        SPELL_EFFECT_SUMMON_TOTEM_SLOT1 = 87
        SPELL_EFFECT_SUMMON_TOTEM_SLOT2 = 88
        SPELL_EFFECT_SUMMON_TOTEM_SLOT3 = 89
        SPELL_EFFECT_SUMMON_TOTEM_SLOT4 = 90
        SPELL_EFFECT_THREAT_ALL = 91
        SPELL_EFFECT_ENCHANT_HELD_ITEM = 92
        SPELL_EFFECT_SUMMON_PHANTASM = 93
        SPELL_EFFECT_SELF_RESURRECT = 94
        SPELL_EFFECT_SKINNING = 95
        SPELL_EFFECT_CHARGE = 96
        SPELL_EFFECT_SUMMON_CRITTER = 97
        SPELL_EFFECT_KNOCK_BACK = 98
        SPELL_EFFECT_DISENCHANT = 99
        SPELL_EFFECT_INEBRIATE = 100
        SPELL_EFFECT_FEED_PET = 101
        SPELL_EFFECT_DISMISS_PET = 102
        SPELL_EFFECT_REPUTATION = 103
        SPELL_EFFECT_SUMMON_OBJECT_SLOT1 = 104
        SPELL_EFFECT_SUMMON_OBJECT_SLOT2 = 105
        SPELL_EFFECT_SUMMON_OBJECT_SLOT3 = 106
        SPELL_EFFECT_SUMMON_OBJECT_SLOT4 = 107
        SPELL_EFFECT_DISPEL_MECHANIC = 108
        SPELL_EFFECT_SUMMON_DEAD_PET = 109
        SPELL_EFFECT_DESTROY_ALL_TOTEMS = 110
        SPELL_EFFECT_DURABILITY_DAMAGE = 111
        SPELL_EFFECT_SUMMON_DEMON = 112
        SPELL_EFFECT_RESURRECT_NEW = 113
        SPELL_EFFECT_ATTACK_ME = 114
        SPELL_EFFECT_DURABILITY_DAMAGE_PCT = 115
        SPELL_EFFECT_SKIN_PLAYER_CORPSE = 116
        SPELL_EFFECT_SPIRIT_HEAL = 117
        SPELL_EFFECT_SKILL = 118
        SPELL_EFFECT_APPLY_AURA_NEW = 119
        SPELL_EFFECT_TELEPORT_GRAVEYARD = 120
        SPELL_EFFECT_ADICIONAL_DMG = 121
        SPELL_EFFECT_122 = 122
        SPELL_EFFECT_123 = 123
        SPELL_EFFECT_PLAYER_PULL = 124
        SPELL_EFFECT_REDUCE_THREAT_PERCENT = 125
        SPELL_EFFECT_STEAL_BENEFICIAL_BUFF = 126
        SPELL_EFFECT_PROSPECTING = 127
        SPELL_EFFECT_APPLY_AREA_AURA_FRIEND = 128
        SPELL_EFFECT_APPLY_AREA_AURA_ENEMY = 129
        SPELL_EFFECT_REDIRECT_THREAT = 130
        SPELL_EFFECT_131 = 131
        SPELL_EFFECT_132 = 132
        SPELL_EFFECT_UNLEARN_SPECIALIZATION = 133
        SPELL_EFFECT_KILL_CREDIT = 134
        SPELL_EFFECT_135 = 135
        SPELL_EFFECT_HEAL_PCT = 136
        SPELL_EFFECT_ENERGIZE_PCT = 137
        SPELL_EFFECT_138 = 138
        SPELL_EFFECT_139 = 139
        SPELL_EFFECT_FORCE_CAST = 140
        SPELL_EFFECT_141 = 141
        SPELL_EFFECT_TRIGGER_SPELL_WITH_VALUE = 142
        SPELL_EFFECT_APPLY_AREA_AURA_OWNER = 143
        SPELL_EFFECT_144 = 144
        SPELL_EFFECT_145 = 145
        SPELL_EFFECT_146 = 146
        SPELL_EFFECT_QUEST_FAIL = 147
        SPELL_EFFECT_148 = 148
        SPELL_EFFECT_149 = 149
        SPELL_EFFECT_150 = 150
        SPELL_EFFECT_TRIGGER_SPELL_2 = 151
        SPELL_EFFECT_152 = 152
        SPELL_EFFECT_153 = 153
    End Enum
    Public Enum AuraEffects_Names As Integer
        SPELL_AURA_NONE = 0
        SPELL_AURA_BIND_SIGHT = 1
        SPELL_AURA_MOD_POSSESS = 2
        SPELL_AURA_PERIODIC_DAMAGE = 3
        SPELL_AURA_DUMMY = 4
        SPELL_AURA_MOD_CONFUSE = 5
        SPELL_AURA_MOD_CHARM = 6
        SPELL_AURA_MOD_FEAR = 7
        SPELL_AURA_PERIODIC_HEAL = 8
        SPELL_AURA_MOD_ATTACKSPEED = 9
        SPELL_AURA_MOD_THREAT = 10
        SPELL_AURA_MOD_TAUNT = 11
        SPELL_AURA_MOD_STUN = 12
        SPELL_AURA_MOD_DAMAGE_DONE = 13
        SPELL_AURA_MOD_DAMAGE_TAKEN = 14
        SPELL_AURA_DAMAGE_SHIELD = 15
        SPELL_AURA_MOD_STEALTH = 16
        SPELL_AURA_MOD_DETECT = 17
        SPELL_AURA_MOD_INVISIBILITY = 18
        SPELL_AURA_MOD_INVISIBILITY_DETECTION = 19
        SPELL_AURA_OBS_MOD_HEALTH = 20                         '2021 unofficial
        SPELL_AURA_OBS_MOD_MANA = 21
        SPELL_AURA_MOD_RESISTANCE = 22
        SPELL_AURA_PERIODIC_TRIGGER_SPELL = 23
        SPELL_AURA_PERIODIC_ENERGIZE = 24
        SPELL_AURA_MOD_PACIFY = 25
        SPELL_AURA_MOD_ROOT = 26
        SPELL_AURA_MOD_SILENCE = 27
        SPELL_AURA_REFLECT_SPELLS = 28
        SPELL_AURA_MOD_STAT = 29
        SPELL_AURA_MOD_SKILL = 30
        SPELL_AURA_MOD_INCREASE_SPEED = 31
        SPELL_AURA_MOD_INCREASE_MOUNTED_SPEED = 32
        SPELL_AURA_MOD_DECREASE_SPEED = 33
        SPELL_AURA_MOD_INCREASE_HEALTH = 34
        SPELL_AURA_MOD_INCREASE_ENERGY = 35
        SPELL_AURA_MOD_SHAPESHIFT = 36
        SPELL_AURA_EFFECT_IMMUNITY = 37
        SPELL_AURA_STATE_IMMUNITY = 38
        SPELL_AURA_SCHOOL_IMMUNITY = 39
        SPELL_AURA_DAMAGE_IMMUNITY = 40
        SPELL_AURA_DISPEL_IMMUNITY = 41
        SPELL_AURA_PROC_TRIGGER_SPELL = 42
        SPELL_AURA_PROC_TRIGGER_DAMAGE = 43
        SPELL_AURA_TRACK_CREATURES = 44
        SPELL_AURA_TRACK_RESOURCES = 45
        SPELL_AURA_MOD_PARRY_SKILL = 46
        SPELL_AURA_MOD_PARRY_PERCENT = 47
        SPELL_AURA_MOD_DODGE_SKILL = 48
        SPELL_AURA_MOD_DODGE_PERCENT = 49
        SPELL_AURA_MOD_BLOCK_SKILL = 50
        SPELL_AURA_MOD_BLOCK_PERCENT = 51
        SPELL_AURA_MOD_CRIT_PERCENT = 52
        SPELL_AURA_PERIODIC_LEECH = 53
        SPELL_AURA_MOD_HIT_CHANCE = 54
        SPELL_AURA_MOD_SPELL_HIT_CHANCE = 55
        SPELL_AURA_TRANSFORM = 56
        SPELL_AURA_MOD_SPELL_CRIT_CHANCE = 57
        SPELL_AURA_MOD_INCREASE_SWIM_SPEED = 58
        SPELL_AURA_MOD_DAMAGE_DONE_CREATURE = 59
        SPELL_AURA_MOD_PACIFY_SILENCE = 60
        SPELL_AURA_MOD_SCALE = 61
        SPELL_AURA_PERIODIC_HEALTH_FUNNEL = 62
        SPELL_AURA_PERIODIC_MANA_FUNNEL = 63
        SPELL_AURA_PERIODIC_MANA_LEECH = 64
        SPELL_AURA_MOD_CASTING_SPEED = 65
        SPELL_AURA_FEIGN_DEATH = 66
        SPELL_AURA_MOD_DISARM = 67
        SPELL_AURA_MOD_STALKED = 68
        SPELL_AURA_SCHOOL_ABSORB = 69
        SPELL_AURA_EXTRA_ATTACKS = 70
        SPELL_AURA_MOD_SPELL_CRIT_CHANCE_SCHOOL = 71
        SPELL_AURA_MOD_POWER_COST_SCHOOL_PCT = 72
        SPELL_AURA_MOD_POWER_COST_SCHOOL = 73
        SPELL_AURA_REFLECT_SPELLS_SCHOOL = 74
        SPELL_AURA_MOD_LANGUAGE = 75
        SPELL_AURA_FAR_SIGHT = 76
        SPELL_AURA_MECHANIC_IMMUNITY = 77
        SPELL_AURA_MOUNTED = 78
        SPELL_AURA_MOD_DAMAGE_PERCENT_DONE = 79
        SPELL_AURA_MOD_PERCENT_STAT = 80
        SPELL_AURA_SPLIT_DAMAGE_PCT = 81
        SPELL_AURA_WATER_BREATHING = 82
        SPELL_AURA_MOD_BASE_RESISTANCE = 83
        SPELL_AURA_MOD_REGEN = 84
        SPELL_AURA_MOD_POWER_REGEN = 85
        SPELL_AURA_CHANNEL_DEATH_ITEM = 86
        SPELL_AURA_MOD_DAMAGE_PERCENT_TAKEN = 87
        SPELL_AURA_MOD_HEALTH_REGEN_PERCENT = 88
        SPELL_AURA_PERIODIC_DAMAGE_PERCENT = 89
        SPELL_AURA_MOD_RESIST_CHANCE = 90
        SPELL_AURA_MOD_DETECT_RANGE = 91
        SPELL_AURA_PREVENTS_FLEEING = 92
        SPELL_AURA_MOD_UNATTACKABLE = 93
        SPELL_AURA_INTERRUPT_REGEN = 94
        SPELL_AURA_GHOST = 95
        SPELL_AURA_SPELL_MAGNET = 96
        SPELL_AURA_MANA_SHIELD = 97
        SPELL_AURA_MOD_SKILL_TALENT = 98
        SPELL_AURA_MOD_ATTACK_POWER = 99
        SPELL_AURA_AURAS_VISIBLE = 100
        SPELL_AURA_MOD_RESISTANCE_PCT = 101
        SPELL_AURA_MOD_MELEE_ATTACK_POWER_VERSUS = 102
        SPELL_AURA_MOD_TOTAL_THREAT = 103
        SPELL_AURA_WATER_WALK = 104
        SPELL_AURA_FEATHER_FALL = 105
        SPELL_AURA_HOVER = 106
        SPELL_AURA_ADD_FLAT_MODIFIER = 107
        SPELL_AURA_ADD_PCT_MODIFIER = 108
        SPELL_AURA_ADD_TARGET_TRIGGER = 109
        SPELL_AURA_MOD_POWER_REGEN_PERCENT = 110
        SPELL_AURA_ADD_CASTER_HIT_TRIGGER = 111
        SPELL_AURA_OVERRIDE_CLASS_SCRIPTS = 112
        SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN = 113
        SPELL_AURA_MOD_RANGED_DAMAGE_TAKEN_PCT = 114
        SPELL_AURA_MOD_HEALING = 115
        SPELL_AURA_MOD_REGEN_DURING_COMBAT = 116
        SPELL_AURA_MOD_MECHANIC_RESISTANCE = 117
        SPELL_AURA_MOD_HEALING_PCT = 118
        SPELL_AURA_SHARE_PET_TRACKING = 119
        SPELL_AURA_UNTRACKABLE = 120
        SPELL_AURA_EMPATHY = 121
        SPELL_AURA_MOD_OFFHAND_DAMAGE_PCT = 122
        SPELL_AURA_MOD_TARGET_RESISTANCE = 123
        SPELL_AURA_MOD_RANGED_ATTACK_POWER = 124
        SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN = 125
        SPELL_AURA_MOD_MELEE_DAMAGE_TAKEN_PCT = 126
        SPELL_AURA_RANGED_ATTACK_POWER_ATTACKER_BONUS = 127
        SPELL_AURA_MOD_POSSESS_PET = 128
        SPELL_AURA_MOD_SPEED_ALWAYS = 129
        SPELL_AURA_MOD_MOUNTED_SPEED_ALWAYS = 130
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_VERSUS = 131
        SPELL_AURA_MOD_INCREASE_ENERGY_PERCENT = 132
        SPELL_AURA_MOD_INCREASE_HEALTH_PERCENT = 133
        SPELL_AURA_MOD_MANA_REGEN_INTERRUPT = 134
        SPELL_AURA_MOD_HEALING_DONE = 135
        SPELL_AURA_MOD_HEALING_DONE_PERCENT = 136
        SPELL_AURA_MOD_TOTAL_STAT_PERCENTAGE = 137
        SPELL_AURA_MOD_HASTE = 138
        SPELL_AURA_FORCE_REACTION = 139
        SPELL_AURA_MOD_RANGED_HASTE = 140
        SPELL_AURA_MOD_RANGED_AMMO_HASTE = 141
        SPELL_AURA_MOD_BASE_RESISTANCE_PCT = 142
        SPELL_AURA_MOD_RESISTANCE_EXCLUSIVE = 143
        SPELL_AURA_SAFE_FALL = 144
        SPELL_AURA_CHARISMA = 145
        SPELL_AURA_PERSUADED = 146
        SPELL_AURA_ADD_CREATURE_IMMUNITY = 147
        SPELL_AURA_RETAIN_COMBO_POINTS = 148
        SPELL_AURA_RESIST_PUSHBACK = 149                      '    Resist Pushback
        SPELL_AURA_MOD_SHIELD_BLOCKVALUE_PCT = 150
        SPELL_AURA_TRACK_STEALTHED = 151                      '    Track Stealthed
        SPELL_AURA_MOD_DETECTED_RANGE = 152                    '    Mod Detected Range
        SPELL_AURA_SPLIT_DAMAGE_FLAT = 153                     '    Split Damage Flat
        SPELL_AURA_MOD_STEALTH_LEVEL = 154                     '    Stealth Level Modifier
        SPELL_AURA_MOD_WATER_BREATHING = 155                   '    Mod Water Breathing
        SPELL_AURA_MOD_REPUTATION_GAIN = 156                   '    Mod Reputation Gain
        SPELL_AURA_PET_DAMAGE_MULTI = 157                      '    Mod Pet Damage
        SPELL_AURA_MOD_SHIELD_BLOCKVALUE = 158
        SPELL_AURA_NO_PVP_CREDIT = 159
        SPELL_AURA_MOD_AOE_AVOIDANCE = 160
        SPELL_AURA_MOD_HEALTH_REGEN_IN_COMBAT = 161
        SPELL_AURA_POWER_BURN_MANA = 162
        SPELL_AURA_MOD_CRIT_DAMAGE_BONUS_MELEE = 163
        SPELL_AURA_164 = 164
        SPELL_AURA_MELEE_ATTACK_POWER_ATTACKER_BONUS = 165
        SPELL_AURA_MOD_ATTACK_POWER_PCT = 166
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_PCT = 167
        SPELL_AURA_MOD_DAMAGE_DONE_VERSUS = 168
        SPELL_AURA_MOD_CRIT_PERCENT_VERSUS = 169
        SPELL_AURA_DETECT_AMORE = 170
        SPELL_AURA_MOD_SPEED_NOT_STACK = 171
        SPELL_AURA_MOD_MOUNTED_SPEED_NOT_STACK = 172
        SPELL_AURA_ALLOW_CHAMPION_SPELLS = 173
        SPELL_AURA_MOD_SPELL_DAMAGE_OF_STAT_PERCENT = 174      ' by defeult intelect dependent from SPELL_AURA_MOD_SPELL_HEALING_OF_STAT_PERCENT
        SPELL_AURA_MOD_SPELL_HEALING_OF_STAT_PERCENT = 175
        SPELL_AURA_SPIRIT_OF_REDEMPTION = 176
        SPELL_AURA_AOE_CHARM = 177
        SPELL_AURA_MOD_DEBUFF_RESISTANCE = 178
        SPELL_AURA_MOD_ATTACKER_SPELL_CRIT_CHANCE = 179
        SPELL_AURA_MOD_FLAT_SPELL_DAMAGE_VERSUS = 180
        SPELL_AURA_MOD_FLAT_SPELL_CRIT_DAMAGE_VERSUS = 181     ' unused - possible flat spell crit damage versus
        SPELL_AURA_MOD_RESISTANCE_OF_STAT_PERCENT = 182
        SPELL_AURA_MOD_CRITICAL_THREAT = 183
        SPELL_AURA_MOD_ATTACKER_MELEE_HIT_CHANCE = 184
        SPELL_AURA_MOD_ATTACKER_RANGED_HIT_CHANCE = 185
        SPELL_AURA_MOD_ATTACKER_SPELL_HIT_CHANCE = 186
        SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_CHANCE = 187
        SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_CHANCE = 188
        SPELL_AURA_MOD_RATING = 189
        SPELL_AURA_MOD_FACTION_REPUTATION_GAIN = 190
        SPELL_AURA_USE_NORMAL_MOVEMENT_SPEED = 191
        SPELL_AURA_HASTE_MELEE = 192
        SPELL_AURA_MELEE_SLOW = 193
        SPELL_AURA_MOD_DEPRICATED_1 = 194                     ' not used now old SPELL_AURA_MOD_SPELL_DAMAGE_OF_INTELLECT
        SPELL_AURA_MOD_DEPRICATED_2 = 195                     ' not used now old SPELL_AURA_MOD_SPELL_HEALING_OF_INTELLECT
        SPELL_AURA_MOD_COOLDOWN = 196                          ' only 24818 Noxious Breath
        SPELL_AURA_MOD_ATTACKER_SPELL_AND_WEAPON_CRIT_CHANCE = 197
        SPELL_AURA_MOD_ALL_WEAPON_SKILLS = 198
        SPELL_AURA_MOD_INCREASES_SPELL_PCT_TO_HIT = 199
        SPELL_AURA_MOD_XP_PCT = 200
        SPELL_AURA_FLY = 201
        SPELL_AURA_IGNORE_COMBAT_RESULT = 202
        SPELL_AURA_MOD_ATTACKER_MELEE_CRIT_DAMAGE = 203
        SPELL_AURA_MOD_ATTACKER_RANGED_CRIT_DAMAGE = 204
        SPELL_AURA_205 = 205                                   ' unused
        SPELL_AURA_MOD_SPEED_MOUNTED = 206                     ' ? used in strange spells
        SPELL_AURA_MOD_INCREASE_FLIGHT_SPEED = 207
        SPELL_AURA_MOD_SPEED_FLIGHT = 208
        SPELL_AURA_MOD_FLIGHT_SPEED_ALWAYS = 209
        SPELL_AURA_210 = 210                                   ' unused
        SPELL_AURA_MOD_FLIGHT_SPEED_NOT_STACK = 211
        SPELL_AURA_MOD_RANGED_ATTACK_POWER_OF_STAT_PERCENT = 212
        SPELL_AURA_MOD_RAGE_FROM_DAMAGE_DEALT = 213
        SPELL_AURA_214 = 214
        SPELL_AURA_ARENA_PREPARATION = 215
        SPELL_AURA_HASTE_SPELLS = 216
        SPELL_AURA_217 = 217
        SPELL_AURA_HASTE_RANGED = 218
        SPELL_AURA_MOD_MANA_REGEN_FROM_STAT = 219
        SPELL_AURA_MOD_RATING_FROM_STAT = 220
        SPELL_AURA_221 = 221
        SPELL_AURA_222 = 222
        SPELL_AURA_223 = 223
        SPELL_AURA_224 = 224
        SPELL_AURA_PRAYER_OF_MENDING = 225
        SPELL_AURA_PERIODIC_DUMMY = 226
        SPELL_AURA_227 = 227
        SPELL_AURA_DETECT_STEALTH = 228
        SPELL_AURA_MOD_AOE_DAMAGE_AVOIDANCE = 229
        SPELL_AURA_230 = 230
        SPELL_AURA_231 = 231
        SPELL_AURA_MECHANIC_DURATION_MOD = 232
        SPELL_AURA_233 = 233
        SPELL_AURA_MECHANIC_DURATION_MOD_NOT_STACK = 234
        SPELL_AURA_MOD_DISPEL_RESIST = 235
        SPELL_AURA_236 = 236
        SPELL_AURA_MOD_SPELL_DAMAGE_OF_ATTACK_POWER = 237
        SPELL_AURA_MOD_SPELL_HEALING_OF_ATTACK_POWER = 238
        SPELL_AURA_MOD_SCALE_2 = 239
        SPELL_AURA_MOD_EXPERTISE = 240
        SPELL_AURA_241 = 241
        SPELL_AURA_MOD_SPELL_DAMAGE_FROM_HEALING = 242
        SPELL_AURA_243 = 243
        SPELL_AURA_244 = 244
        SPELL_AURA_MOD_DURATION_OF_MAGIC_EFFECTS = 245
        SPELL_AURA_246 = 246
        SPELL_AURA_247 = 247
        SPELL_AURA_MOD_COMBAT_RESULT_CHANCE = 248
        SPELL_AURA_249 = 249
        SPELL_AURA_MOD_INCREASE_HEALTH_2 = 250
        SPELL_AURA_MOD_ENEMY_DODGE = 251
        SPELL_AURA_252 = 252
        SPELL_AURA_253 = 253
        SPELL_AURA_254 = 254
        SPELL_AURA_255 = 255
        SPELL_AURA_256 = 256
        SPELL_AURA_257 = 257
        SPELL_AURA_258 = 258
        SPELL_AURA_259 = 259
        SPELL_AURA_260 = 260
        SPELL_AURA_261 = 261
    End Enum
#End Region

#Region "Spells.Info"
    Public SPELLs As New Dictionary(Of Integer, SpellInfo)(29000)
    Public SpellCastTime As New Dictionary(Of Integer, Integer)
    Public SpellRadius As New Dictionary(Of Integer, Single)
    Public SpellRange As New Dictionary(Of Integer, Single)
    Public SpellDuration As New Dictionary(Of Integer, Integer)
    Public SpellIcon As New Dictionary(Of Integer, String)

    Public OpenSpells As New Dictionary(Of Integer, frmSpellInfo)
    Public CompareSpells As New List(Of Integer)

    Public Class SpellInfo
        Public ID As Integer = 0
        Public School As Integer = 0
        Public Category As Integer = 0
        Public DispellType As Integer = 0
        Public Mechanic As Integer = 0

        Public Attributes As Integer = 0
        Public AttributesEx As Integer = 0
        Public AttributesEx2 As Integer = 0
        Public RequredCasterStance As Integer = 0
        Public ShapeshiftExclude As Integer = 0
        Public Target As Integer = 0
        Public TargetCreatureType As Integer = 0
        Public FocusObjectIndex As Integer = 0
        Public FacingCasterFlags As Integer = 0
        Public CasterAuraState As Integer = 0
        Public TargetAuraState As Integer = 0
        Public ExcludeCasterAuraState As Integer = 0
        Public ExcludeTargetAuraState As Integer = 0

        Public SpellCastTimeIndex As Integer = 0
        Public CategoryCooldown As Integer = 0
        Public SpellCooldown As Integer = 0

        Public interruptFlags As Integer = 0
        Public auraInterruptFlags As Integer = 0
        Public channelInterruptFlags As Integer = 0
        Public procFlags As Integer = 0
        Public procChance As Integer = 0
        Public procCharges As Integer = 0
        Public maxLevel As Integer = 0
        Public baseLevel As Integer = 0
        Public spellLevel As Integer = 0
        Public maxStack As Integer = 0

        Public DurationIndex As Integer = 0

        Public powerType As Integer = 0
        Public manaCost As Integer = 0
        Public manaCostPerlevel As Integer = 0
        Public manaPerSecond As Integer = 0
        Public manaPerSecondPerLevel As Integer = 0
        Public manaCostPercent As Integer = 0

        Public rangeIndex As Integer = 0
        Public Speed As Single = 0
        Public modalNextSpell As Integer = 0

        Public Totem() As Integer = {0, 0}
        Public TotemCategory() As Integer = {0, 0}
        Public Reagents() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}
        Public ReagentsCount() As Integer = {0, 0, 0, 0, 0, 0, 0, 0}

        Public EquippedItemClass As Integer = 0
        Public EquippedItemSubClass As Integer = 0
        Public EquippedItemInventoryType As Integer = 0

        Public SpellEffects() As SpellEffect = {Nothing, Nothing, Nothing}

        Public MaxTargets As Integer = 0
        Public RequiredAreaID As Integer = 0

        Public SpellVisual As Integer = 0
        Public SpellPriority As Integer = 0
        Public AffectedTargetLevel As Integer = 0
        Public SpellIconID As Integer = 0
        Public ActiveIconID As Integer = 0
        Public SpellNameFlag As Integer = 0
        Public Rank As String = ""
        Public RankFlags As Integer = 0
        Public StartRecoveryCategory As Integer = 0
        Public StartRecoveryTime As Integer = 0
        Public SpellFamilyName As Integer = 0
        Public SpellFamilyFlags As Integer = 0
        'Public MaxAffectedTargets As Integer = 0
        Public DamageType As Integer = 0
        Public Name As String = ""
        Public Description As String = ""
        Public BuffDesc As String = ""

        Public unk1 As Integer = 0

        Public Sub New()

        End Sub
    End Class

    Public Class SpellEffect
        Public ID As SpellEffects_Names = SpellEffects_Names.SPELL_EFFECT_NOTHING

        Public diceSides As Integer = 0
        Public diceBase As Integer = 0
        Public dicePerLevel As Single = 0
        Public valueBase As Integer = 0
        Public valueDie As Integer = 0
        Public valuePerLevel As Integer = 0
        Public valuePerComboPoint As Integer = 0
        Public implicitTargetA As Integer = 0
        Public implicitTargetB As Integer = 0
        Public mechanic As Integer = 0

        Public RadiusIndex As Integer = 0
        Public ApplyAuraIndex As Integer = 0

        Public Amplitude As Integer = 0
        Public MultipleValue As Integer = 0
        Public ChainTarget As Integer = 0
        Public ItemType As Integer = 0
        Public MiscValue As Integer = 0
        Public TriggerSpell As Integer = 0
        Public DamageMultiplier As Single = 1

        Public Sub New()

        End Sub

        Public ReadOnly Property GetValue(Optional ByVal Divide As Integer = 1, Optional ByVal Multiply As Integer = 1) As String
            Get
                Dim Min As Integer = Math.Abs(valueBase + 1) / Divide * Multiply
                Dim Max As Integer = Math.Abs(valueBase + valueDie) / Divide * Multiply
                If Min >= Max Then
                    Return Min.ToString
                Else
                    Return Min & " to " & Max
                End If
            End Get
        End Property

        Public ReadOnly Property GetDotValue(ByVal SpellID As Integer) As String
            Get
                If Amplitude <= 0 Then Return "0"
                Dim Duration As Integer = 0
                If SpellDuration.ContainsKey(SPELLs(SpellID).DurationIndex) Then Duration = SpellDuration(SPELLs(SpellID).DurationIndex)
                If Duration <= 0 Then Return "0"
                Dim Min As Integer = Math.Abs((valueBase + 1) * (Duration / Amplitude))
                Dim Max As Integer = Math.Abs((valueBase + valueDie) * (Duration / Amplitude))

                If Min = Max Then
                    Return Min.ToString
                Else
                    Return Min & " to " & Max
                End If
            End Get
        End Property

        Public ReadOnly Property GetTargets() As String
            Get
                Dim tmpStr As String = CType(implicitTargetA, SpellImplicitTargets).ToString() & " (" & implicitTargetA & ")"
                tmpStr &= " + " & CType(implicitTargetB, SpellImplicitTargets).ToString() & " (" & implicitTargetB & ")"
                Return tmpStr
            End Get
        End Property
    End Class
#End Region

End Module