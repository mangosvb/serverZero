'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
'
' This program is free software. You can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation. either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY. Without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program. If not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'

Namespace Enums
    Public Module SpellEnum

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

        Public Enum SpellCastState As Byte
            SPELL_STATE_NULL = 0
            SPELL_STATE_PREPARING = 1
            SPELL_STATE_CASTING = 2
            SPELL_STATE_FINISHED = 3
            SPELL_STATE_IDLE = 4
        End Enum

        Public Enum SpellSchoolMask As Integer
            SPELL_SCHOOL_MASK_NONE = &H0
            SPELL_SCHOOL_MASK_NORMAL = &H1
            SPELL_SCHOOL_MASK_HOLY = &H2
            SPELL_SCHOOL_MASK_FIRE = &H4
            SPELL_SCHOOL_MASK_NATURE = &H8
            SPELL_SCHOOL_MASK_FROST = &H10
            SPELL_SCHOOL_MASK_SHADOW = &H20
            SPELL_SCHOOL_MASK_ARCANE = &H40
            SPELL_SCHOOL_MASK_SPELL = (SPELL_SCHOOL_MASK_FIRE Or SPELL_SCHOOL_MASK_NATURE Or SPELL_SCHOOL_MASK_FROST Or SPELL_SCHOOL_MASK_SHADOW Or SPELL_SCHOOL_MASK_ARCANE)
            SPELL_SCHOOL_MASK_MAGIC = (SPELL_SCHOOL_MASK_HOLY Or SPELL_SCHOOL_MASK_SPELL)
            SPELL_SCHOOL_MASK_ALL = (SPELL_SCHOOL_MASK_NORMAL Or SPELL_SCHOOL_MASK_MAGIC)
        End Enum

        Public Enum SpellInterruptFlags As Integer
            SPELL_INTERRUPT_FLAG_MOVEMENT = &H1 ' why need this for instant?
            SPELL_INTERRUPT_FLAG_PUSH_BACK = &H2 ' push back
            SPELL_INTERRUPT_FLAG_INTERRUPT = &H4 ' interrupt
            SPELL_INTERRUPT_FLAG_AUTOATTACK = &H8 ' no
            SPELL_INTERRUPT_FLAG_DAMAGE = &H10  ' _complete_ interrupt on direct damage?
        End Enum

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

        Public Enum SpellAuraProcFlags As Integer
            AURA_PROC_NULL = &H0
            AURA_PROC_ON_ANY_HOSTILE_ACTION = &H1
            AURA_PROC_ON_GAIN_EXPIERIENCE = &H2
            AURA_PROC_ON_MELEE_ATTACK = &H4
            AURA_PROC_ON_CRIT_HIT_VICTIM = &H8
            AURA_PROC_ON_CAST_SPELL = &H10
            AURA_PROC_ON_PHYSICAL_ATTACK_VICTIM = &H20
            AURA_PROC_ON_RANGED_ATTACK = &H40
            AURA_PROC_ON_RANGED_CRIT_ATTACK = &H80
            AURA_PROC_ON_PHYSICAL_ATTACK = &H100
            AURA_PROC_ON_MELEE_ATTACK_VICTIM = &H200
            AURA_PROC_ON_SPELL_HIT = &H400
            AURA_PROC_ON_RANGED_CRIT_ATTACK_VICTIM = &H800
            AURA_PROC_ON_CRIT_ATTACK = &H1000
            AURA_PROC_ON_RANGED_ATTACK_VICTIM = &H2000
            AURA_PROC_ON_PRE_DISPELL_AURA_VICTIM = &H4000
            AURA_PROC_ON_SPELL_LAND_VICTIM = &H8000
            AURA_PROC_ON_CAST_SPECIFIC_SPELL = &H10000
            AURA_PROC_ON_SPELL_HIT_VICTIM = &H20000
            AURA_PROC_ON_SPELL_CRIT_HIT_VICTIM = &H40000
            AURA_PROC_ON_TARGET_DIE = &H80000
            AURA_PROC_ON_ANY_DAMAGE_VICTIM = &H100000
            AURA_PROC_ON_TRAP_TRIGGER = &H200000                'triggers on trap activation
            AURA_PROC_ON_AUTO_SHOT_HIT = &H400000
            AURA_PROC_ON_ABSORB = &H800000
            AURA_PROC_ON_RESIST_VICTIM = &H1000000
            AURA_PROC_ON_DODGE_VICTIM = &H2000000
            AURA_PROC_ON_DIE = &H4000000
            AURA_PROC_REMOVEONUSE = &H8000000                   'remove AURA_PROChcharge only when it is used
            AURA_PROC_MISC = &H10000000                          'our custom flag to decide if AURA_PROC dmg or shield
            AURA_PROC_ON_BLOCK_VICTIM = &H20000000
            AURA_PROC_ON_SPELL_CRIT_HIT = &H40000000
            AURA_PROC_TARGET_SELF = &H80000000                   'our custom flag to decide if AURA_PROC target is self or victim
        End Enum

        Public Enum SpellAuraStates As Integer
            AURASTATE_FLAG_DODGE_BLOCK = 1
            AURASTATE_FLAG_HEALTH20 = 2
            AURASTATE_FLAG_BERSERK = 4
            AURASTATE_FLAG_JUDGEMENT = 16
            AURASTATE_FLAG_PARRY = 64
            AURASTATE_FLAG_LASTKILLWITHHONOR = 512
            AURASTATE_FLAG_CRITICAL = 1024
            AURASTATE_FLAG_HEALTH35 = 4096
            AURASTATE_FLAG_IMMOLATE = 8192
            AURASTATE_FLAG_REJUVENATE = 16384
            AURASTATE_FLAG_POISON = 32768
        End Enum

        Public Enum SpellProcFlags As Byte
            PROC_ON_DAMAGE_RECEIVED = 3
        End Enum

        Public Enum SpellCastFlags As Integer
            CAST_FLAG_RANGED = &H20
            CAST_FLAG_ITEM_CASTER = &H100
            CAST_FLAG_EXTRA_MSG = &H400
        End Enum

        Public Enum SpellDamageType As Byte
            SPELL_DMG_TYPE_NONE = 0
            SPELL_DMG_TYPE_MAGIC = 1
            SPELL_DMG_TYPE_MELEE = 2
            SPELL_DMG_TYPE_RANGED = 3
        End Enum

        Public Enum SpellType As Byte
            SPELL_TYPE_NONMELEE = 0
            SPELL_TYPE_DOT = 1
            SPELL_TYPE_HEAL = 2
            SPELL_TYPE_HEALDOT = 3
        End Enum

        Public Enum SpellModOp
            SPELLMOD_DAMAGE = 0
            SPELLMOD_DURATION = 1
            SPELLMOD_THREAT = 2
            SPELLMOD_EFFECT1 = 3
            SPELLMOD_CHARGES = 4
            SPELLMOD_RANGE = 5
            SPELLMOD_RADIUS = 6
            SPELLMOD_CRITICAL_CHANCE = 7
            SPELLMOD_ALL_EFFECTS = 8
            SPELLMOD_NOT_LOSE_CASTING_TIME = 9
            SPELLMOD_CASTING_TIME = 10
            SPELLMOD_COOLDOWN = 11
            SPELLMOD_EFFECT2 = 12
            ' spellmod 13 unused
            SPELLMOD_COST = 14
            SPELLMOD_CRIT_DAMAGE_BONUS = 15
            SPELLMOD_RESIST_MISS_CHANCE = 16
            SPELLMOD_JUMP_TARGETS = 17
            SPELLMOD_CHANCE_OF_SUCCESS = 18                   ' Only used with SPELL_AURA_ADD_FLAT_MODIFIER and affects proc spells
            SPELLMOD_ACTIVATION_TIME = 19
            SPELLMOD_EFFECT_PAST_FIRST = 20
            SPELLMOD_CASTING_TIME_OLD = 21
            SPELLMOD_DOT = 22
            SPELLMOD_EFFECT3 = 23
            SPELLMOD_SPELL_BONUS_DAMAGE = 24
            ' spellmod 25 unused
            SPELLMOD_FREQUENCY_OF_SUCCESS = 26                   ' Only used with SPELL_AURA_ADD_PCT_MODIFIER and affects used on proc spells
            SPELLMOD_MULTIPLE_VALUE = 27
            SPELLMOD_RESIST_DISPEL_CHANCE = 28
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
            SPELL_ATTR_BREAKABLE_BY_DAMAGE = &H40000000 'Breakable by damage
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

        Public Enum SpellAttributesCustom As UInteger
            SPELL_ATTR_CU_CONE_BACK = &H1
            SPELL_ATTR_CU_CONE_LINE = &H2
            SPELL_ATTR_CU_SHARE_DAMAGE = &H4
            SPELL_ATTR_CU_AURA_HOT = &H8
            SPELL_ATTR_CU_AURA_DOT = &H10
            SPELL_ATTR_CU_AURA_CC = &H20
            SPELL_ATTR_CU_AURA_SPELL = &H40
            SPELL_ATTR_CU_DIRECT_DAMAGE = &H80
            SPELL_ATTR_CU_CHARGE = &H100
            SPELL_ATTR_CU_LINK_CAST = &H200
            SPELL_ATTR_CU_LINK_HIT = &H400
            SPELL_ATTR_CU_LINK_AURA = &H800
            SPELL_ATTR_CU_LINK_REMOVE = &H1000
            SPELL_ATTR_CU_MOVEMENT_IMPAIR = &H2000
        End Enum

        Public Enum SpellCastTargetFlags As Integer
            TARGET_FLAG_SELF = &H0
            TARGET_FLAG_UNIT = &H2
            TARGET_FLAG_ITEM = &H10
            TARGET_FLAG_SOURCE_LOCATION = &H20
            TARGET_FLAG_DEST_LOCATION = &H40
            TARGET_FLAG_OBJECT_UNK = &H80
            TARGET_FLAG_PVP_CORPSE = &H200
            TARGET_FLAG_OBJECT = &H800
            TARGET_FLAG_TRADE_ITEM = &H1000
            TARGET_FLAG_STRING = &H2000
            TARGET_FLAG_UNK1 = &H4000
            TARGET_FLAG_CORPSE = &H8000
            TARGET_FLAG_UNK2 = &H10000
        End Enum

        Public Enum SpellMissInfo As Byte
            SPELL_MISS_NONE = 0
            SPELL_MISS_MISS = 1
            SPELL_MISS_RESIST = 2
            SPELL_MISS_DODGE = 3
            SPELL_MISS_PARRY = 4
            SPELL_MISS_BLOCK = 5
            SPELL_MISS_EVADE = 6
            SPELL_MISS_IMMUNE = 7
            SPELL_MISS_IMMUNE2 = 8
            SPELL_MISS_DEFLECT = 9
            SPELL_MISS_ABSORB = 10
            SPELL_MISS_REFLECT = 11
        End Enum

        Public Enum SpellFailedReason As Byte
            SPELL_FAILED_AFFECTING_COMBAT = &H0                             ' 0x000
            SPELL_FAILED_ALREADY_AT_FULL_HEALTH = &H1                       ' 0x001
            SPELL_FAILED_ALREADY_AT_FULL_POWER = &H2                        ' 0x002
            SPELL_FAILED_ALREADY_BEING_TAMED = &H3                          ' 0x003
            SPELL_FAILED_ALREADY_HAVE_CHARM = &H4                           ' 0x004
            SPELL_FAILED_ALREADY_HAVE_SUMMON = &H5                          ' 0x005
            SPELL_FAILED_ALREADY_OPEN = &H6                                 ' 0x006
            'SPELL_FAILED_AURA_BOUNCED = &H7                                ' 0x007
            SPELL_FAILED_MORE_POWERFUL_SPELL_ACTIVE = &H7                   ' 0x007
            'SPELL_FAILED_AUTOTRACK_INTERRUPTED = &H8                       ' 0x008 ' old commented CAST_FAIL_FAILED = 8,-> 29
            SPELL_FAILED_BAD_IMPLICIT_TARGETS = &H9                         ' 0x009
            SPELL_FAILED_BAD_TARGETS = &HA                                  ' 0x00A
            SPELL_FAILED_CANT_BE_CHARMED = &HB                              ' 0x00B
            SPELL_FAILED_CANT_BE_DISENCHANTED = &HC                         ' 0x00C
            SPELL_FAILED_CANT_BE_PROSPECTED = &HD
            SPELL_FAILED_CANT_CAST_ON_TAPPED = &HE                          ' 0x00D
            SPELL_FAILED_CANT_DUEL_WHILE_INVISIBLE = &HF                    ' 0x00E
            SPELL_FAILED_CANT_DUEL_WHILE_STEALTHED = &H10                   ' 0x00F
            'SPELL_FAILED_CANT_STEALTH = &H10                               ' 0x010
            'SPELL_FAILED_CASTER_AURASTATE = &H11                           ' 0x011
            SPELL_FAILED_CANT_TOO_CLOSE_TO_ENEMY = &H11
            SPELL_FAILED_CANT_DO_THAT_YET = &H12
            SPELL_FAILED_CASTER_DEAD = &H13                                 ' 0x012
            SPELL_FAILED_CHARMED = &H14                                     ' 0x013
            SPELL_FAILED_CHEST_IN_USE = &H15                                ' 0x014
            SPELL_FAILED_CONFUSED = &H16                                    ' 0x015
            SPELL_FAILED_DONT_REPORT = &H17                                 ' 0x016 ' [-ZERO] need check
            SPELL_FAILED_EQUIPPED_ITEM = &H18                               ' 0x017
            SPELL_FAILED_EQUIPPED_ITEM_CLASS = &H19                         ' 0x018
            SPELL_FAILED_EQUIPPED_ITEM_CLASS_MAINHAND = &H1A                ' 0x019
            SPELL_FAILED_EQUIPPED_ITEM_CLASS_OFFHAND = &H1B                 ' 0x01A
            SPELL_FAILED_ERROR = &H1C                                       ' 0x01B
            SPELL_FAILED_FIZZLE = &H1D                                      ' 0x01C
            SPELL_FAILED_FLEEING = &H1E                                     ' 0x01D
            SPELL_FAILED_FOOD_LOWLEVEL = &H1F                               ' 0x01E
            SPELL_FAILED_HIGHLEVEL = &H20                                   ' 0x01F
            'SPELL_FAILED_HUNGER_SATIATED = &H21                            ' 0x020
            SPELL_FAILED_IMMUNE = &H22                                      ' 0x021
            SPELL_FAILED_INTERRUPTED = &H23                                 ' 0x022
            SPELL_FAILED_INTERRUPTED_COMBAT = &H24                          ' 0x023
            SPELL_FAILED_ITEM_ALREADY_ENCHANTED = &H25                      ' 0x024
            SPELL_FAILED_ITEM_GONE = &H26                                   ' 0x025
            SPELL_FAILED_ITEM_NOT_FOUND = &H27                              ' 0x026
            SPELL_FAILED_ITEM_NOT_READY = &H28                              ' 0x027
            SPELL_FAILED_LEVEL_REQUIREMENT = &H29                           ' 0x028
            SPELL_FAILED_LINE_OF_SIGHT = &H2A                               ' 0x029
            SPELL_FAILED_LOWLEVEL = &H2B                                    ' 0x02A
            SPELL_FAILED_LOW_CASTLEVEL = &H2C                               ' 0x02B
            SPELL_FAILED_MAINHAND_EMPTY = &H2D                              ' 0x02C
            SPELL_FAILED_MOVING = &H2E                                      ' 0x02D
            SPELL_FAILED_NEED_AMMO = &H2F                                   ' 0x02E
            SPELL_FAILED_NEED_REQUIRES_SOMETHING = &H30                     ' 0x02F
            SPELL_FAILED_NEED_EXOTIC_AMMO = &H31                            ' 0x030
            SPELL_FAILED_NOPATH = &H32                                      ' 0x031
            SPELL_FAILED_NOT_BEHIND = &H33                                  ' 0x032
            SPELL_FAILED_NOT_FISHABLE = &H34                                ' 0x033
            SPELL_FAILED_NOT_HERE = &H35                                    ' 0x034
            SPELL_FAILED_NOT_INFRONT = &H36                                 ' 0x035
            SPELL_FAILED_NOT_IN_CONTROL = &H37                              ' 0x036
            SPELL_FAILED_NOT_KNOWN = &H38                                   ' 0x037
            SPELL_FAILED_NOT_MOUNTED = &H39                                 ' 0x038
            SPELL_FAILED_NOT_ON_TAXI = &H3A                                 ' 0x039
            SPELL_FAILED_NOT_ON_TRANSPORT = &H3B                            ' 0x03A
            SPELL_FAILED_NOT_READY = &H3C                                   ' 0x03B
            SPELL_FAILED_NOT_SHAPESHIFT = &H3D                              ' 0x03C
            SPELL_FAILED_NOT_STANDING = &H3E                                ' 0x03D
            SPELL_FAILED_NOT_TRADEABLE = &H3F                               ' 0x03E ' rogues trying "enchant" other's weapon with poison
            SPELL_FAILED_NOT_TRADING = &H40                                 ' 0x03F ' CAST_FAIL_CANT_ENCHANT_TRADE_ITEM
            SPELL_FAILED_NOT_UNSHEATHED = &H41                              ' 0x040 ' yellow text
            SPELL_FAILED_NOT_WHILE_GHOST = &H42                             ' 0x041
            SPELL_FAILED_NO_AMMO = &H43                                     ' 0x042
            SPELL_FAILED_NO_CHARGES_REMAIN = &H44                           ' 0x043
            SPELL_FAILED_NO_CHAMPION = &H45                                 ' 0x044 ' CAST_FAIL_NOT_SELECT
            SPELL_FAILED_NO_COMBO_POINTS = &H46                             ' 0x045
            SPELL_FAILED_NO_DUELING = &H47                                  ' 0x046
            SPELL_FAILED_NO_ENDURANCE = &H48                                ' 0x047
            SPELL_FAILED_NO_FISH = &H49                                     ' 0x048
            SPELL_FAILED_NO_ITEMS_WHILE_SHAPESHIFTED = &H4A                 ' 0x049
            SPELL_FAILED_NO_MOUNTS_ALLOWED = &H4B                           ' 0x04A
            SPELL_FAILED_NO_PET = &H4C                                      ' 0x04B
            SPELL_FAILED_NO_POWER = &H4D                                    ' 0x04C ' CAST_FAIL_NOT_ENOUGH_MANA
            SPELL_FAILED_NOTHING_TO_DISPEL = &H4E                           ' 0x04D
            SPELL_FAILED_NOTHING_TO_STEAL = &H4F
            SPELL_FAILED_ONLY_ABOVEWATER = &H50                             ' 0x04E ' CAST_FAIL_CANT_USE_WHILE_SWIMMING
            SPELL_FAILED_ONLY_DAYTIME = &H51                                ' 0x04F
            SPELL_FAILED_ONLY_INDOORS = &H52                                ' 0x050
            SPELL_FAILED_ONLY_MOUNTED = &H53                                ' 0x051
            SPELL_FAILED_ONLY_NIGHTTIME = &H54                              ' 0x052
            SPELL_FAILED_ONLY_OUTDOORS = &H55                               ' 0x053
            SPELL_FAILED_ONLY_SHAPESHIFT = &H56                             ' 0x054
            SPELL_FAILED_ONLY_STEALTHED = &H57                              ' 0x055
            SPELL_FAILED_ONLY_UNDERWATER = &H58                             ' 0x056 ' CAST_FAIL_CAN_ONLY_USE_WHILE_SWIMMING
            SPELL_FAILED_OUT_OF_RANGE = &H59                                ' 0x057
            SPELL_FAILED_PACIFIED = &H5S                                    ' 0x058
            SPELL_FAILED_POSSESSED = &H5B                                   ' 0x059
            'SPELL_FAILED_REAGENTS = &H5C                                   ' 0x05A ' [-ZERO] not in 1.12
            SPELL_FAILED_REQUIRES_AREA = &H5D                               ' 0x05B ' CAST_FAIL_YOU_NEED_TO_BE_IN_XXX
            SPELL_FAILED_REQUIRES_SPELL_FOCUS = &H5E                        ' 0x05C ' CAST_FAIL_REQUIRES_XXX
            SPELL_FAILED_ROOTED = &H5F                                      ' 0x05D ' CAST_FAIL_UNABLE_TO_MOVE
            SPELL_FAILED_SILENCED = &H60                                    ' 0x05E
            SPELL_FAILED_SPELL_IN_PROGRESS = &H61                           ' 0x05F
            SPELL_FAILED_SPELL_LEARNED = &H62                               ' 0x060
            SPELL_FAILED_SPELL_UNAVAILABLE = &H63                           ' 0x061
            SPELL_FAILED_STUNNED = &H64                                     ' 0x062
            SPELL_FAILED_TARGETS_DEAD = &H65                                ' 0x063
            SPELL_FAILED_TARGET_AFFECTING_COMBAT = &H66                     ' 0x064
            SPELL_FAILED_TARGET_AURASTATE = &H67                            ' 0x065 ' CAST_FAIL_CANT_DO_THAT_YET_2
            SPELL_FAILED_TARGET_DUELING = &H68                              ' 0x066
            SPELL_FAILED_TARGET_ENEMY = &H69                                ' 0x067
            SPELL_FAILED_TARGET_ENRAGED = &H6A                              ' 0x068 ' CAST_FAIL_TARGET_IS_TOO_ENRAGED_TO_CHARM
            SPELL_FAILED_TARGET_FRIENDLY = &H6B                             ' 0x069
            SPELL_FAILED_TARGET_IN_COMBAT = &H6C                            ' 0x06A
            SPELL_FAILED_TARGET_IS_PLAYER = &H6D                            ' 0x06B
            SPELL_FAILED_TARGET_NOT_DEAD = &H6E                             ' 0x06C
            SPELL_FAILED_TARGET_NOT_IN_PARTY = &H6F                         ' 0x06D
            SPELL_FAILED_TARGET_NOT_LOOTED = &H70                           ' 0x06E ' CAST_FAIL_CREATURE_MUST_BE_LOOTED_FIRST
            SPELL_FAILED_TARGET_NOT_PLAYER = &H71                           ' 0x06F
            SPELL_FAILED_TARGET_NO_POCKETS = &H72                           ' 0x070 ' CAST_FAIL_NOT_ITEM_TO_STEAL
            SPELL_FAILED_TARGET_NO_WEAPONS = &H73                           ' 0x071
            SPELL_FAILED_TARGET_UNSKINNABLE = &H74                          ' 0x072
            SPELL_FAILED_THIRST_SATIATED = &H75                             ' 0x073
            SPELL_FAILED_TOO_CLOSE = &H76                                   ' 0x074
            SPELL_FAILED_TOO_MANY_OF_ITEM = &H77                            ' 0x075
            'SPELL_FAILED_TOTEMS = &H78                                     ' 0x076 ' [-ZERO] not in 1.12
            SPELL_FAILED_TRAINING_POINTS = &H79                             ' 0x077
            SPELL_FAILED_TRY_AGAIN = &H7A                                   ' 0x078 ' CAST_FAIL_FAILED_ATTEMPT
            SPELL_FAILED_UNIT_NOT_BEHIND = &H7B                             ' 0x079
            SPELL_FAILED_UNIT_NOT_INFRONT = &H7C                            ' 0x07A
            SPELL_FAILED_WRONG_PET_FOOD = &H7D                              ' 0x07B
            SPELL_FAILED_NOT_WHILE_FATIGUED = &H7E                          ' 0x07C
            SPELL_FAILED_TARGET_NOT_IN_INSTANCE = &H7F                      ' 0x07D ' CAST_FAIL_TARGET_MUST_BE_IN_THIS_INSTANCE
            SPELL_FAILED_NOT_WHILE_TRADING = &H80                           ' 0x07E
            SPELL_FAILED_TARGET_NOT_IN_RAID = &H81                          ' 0x07F
            SPELL_FAILED_DISENCHANT_WHILE_LOOTING = &H82                    ' 0x080
            SPELL_FAILED_PROSPECT_WHILE_LOOTING = &H83
            'SPELL_FAILED_PROSPECT_NEED_MORE = &H85
            SPELL_FAILED_TARGET_FREEFORALL = &H85                           ' 0x081
            SPELL_FAILED_NO_EDIBLE_CORPSES = &H86                           ' 0x082
            SPELL_FAILED_ONLY_BATTLEGROUNDS = &H87                          ' 0x083
            SPELL_FAILED_TARGET_NOT_GHOST = &H88                            ' 0x084
            SPELL_FAILED_TOO_MANY_SKILLS = &H89                             ' 0x085 ' CAST_FAIL_YOUR_PET_CANT_LEARN_MORE_SKILLS
            SPELL_FAILED_CANT_USE_NEW_ITEM = &H8A                           ' 0x086
            SPELL_FAILED_WRONG_WEATHER = &H8B                               ' 0x087 ' CAST_FAIL_CANT_DO_IN_THIS_WEATHER
            SPELL_FAILED_DAMAGE_IMMUNE = &H8C                               ' 0x088 ' CAST_FAIL_CANT_DO_IN_IMMUNE
            SPELL_FAILED_PREVENTED_BY_MECHANIC = &H8D                       ' 0x089 ' CAST_FAIL_CANT_DO_IN_XXX
            SPELL_FAILED_PLAY_TIME = &H8E                                   ' 0x08A ' CAST_FAIL_GAME_TIME_OVER
            SPELL_FAILED_REPUTATION = &H8F                                  ' 0x08B
            SPELL_FAILED_MIN_SKILL = &H90
            SPELL_FAILED_UNKNOWN = &H91                                     ' 0x08C
            SPELL_NO_ERROR = &HFF                                           ' 0x0FF
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

        Public Enum SpellFamilyNames As Integer
            SPELLFAMILY_GENERIC = 0
            SPELLFAMILY_MAGE = 3
            SPELLFAMILY_WARRIOR = 4
            SPELLFAMILY_WARLOCK = 5
            SPELLFAMILY_PRIEST = 6
            SPELLFAMILY_DRUID = 7
            SPELLFAMILY_ROGUE = 8
            SPELLFAMILY_HUNTER = 9
            SPELLFAMILY_PALADIN = 10
            SPELLFAMILY_SHAMAN = 11
            SPELLFAMILY_POTION = 13
        End Enum

        Enum SpellFailure As Byte
            SELL_ERR_CANT_FIND_ITEM = 1
            SELL_ERR_CANT_SELL_ITEM = 2
            SELL_ERR_CANT_FIND_VENDOR = 3
        End Enum

        Public Enum CurrentSpellTypes
            CURRENT_MELEE_SPELL = 0
            CURRENT_GENERIC_SPELL = 1
            CURRENT_AUTOREPEAT_SPELL = 2
            CURRENT_CHANNELED_SPELL = 3
        End Enum

    End Module
End NameSpace