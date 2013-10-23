'
' Copyright (C) 2013 getMaNGOS <http://www.getMangos.co.uk>
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
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common
Imports mangosVB.Common.SQL

Public Module WS_DBCDatabase

#Region "Emotes"
    Public EmotesState As New Dictionary(Of Integer, Integer)
    Public EmotesText As New Dictionary(Of Integer, Integer)

    Public Enum Emotes As Integer
        'Auto generated from Emotes.dbc
        STATE_WORK_NOSHEATHE_MINING = 233
        ONESHOT_READYRIFLE = 213
        STATE_READYRIFLE = 214
        STATE_WORK_NOSHEATHE_CHOPWOOD = 234
        STATE_READY1H = 333
        STATE_SUBMERGED = 373
        ONESHOT_NONE = 0
        ONESHOT_TALK = 1
        ONESHOT_BOW = 2
        ONESHOT_WAVE = 3
        ONESHOT_CHEER = 4
        ONESHOT_EXCLAMATION = 5
        ONESHOT_QUESTION = 6
        ONESHOT_EAT = 7
        STATE_SPELLPRECAST = 193
        ONESHOT_LAND = 293
        STATE_DANCE = 10
        ONESHOT_LAUGH = 11
        STATE_SLEEP = 12
        STATE_SIT = 13
        ONESHOT_RUDE = 14
        ONESHOT_ROAR = 15
        ONESHOT_KNEEL = 16
        ONESHOT_KISS = 17
        ONESHOT_CRY = 18
        ONESHOT_CHICKEN = 19
        ONESHOT_BEG = 20
        ONESHOT_APPLAUD = 21
        ONESHOT_SHOUT = 22
        ONESHOT_FLEX = 23
        ONESHOT_SHY = 24
        ONESHOT_POINT = 25
        STATE_STAND = 26
        STATE_READYUNARMED = 27
        STATE_WORK = 28
        STATE_POINT = 29
        STATE_NONE = 30
        ONESHOT_WOUND = 33
        ONESHOT_WOUNDCRITICAL = 34
        ONESHOT_ATTACKUNARMED = 35
        ONESHOT_ATTACK1H = 36
        ONESHOT_ATTACK2HTIGHT = 37
        ONESHOT_ATTACK2HLOOSE = 38
        ONESHOT_PARRYUNARMED = 39
        ONESHOT_PARRYSHIELD = 43
        ONESHOT_READYUNARMED = 44
        ONESHOT_READY1H = 45
        ONESHOT_READYBOW = 48
        ONESHOT_SPELLPRECAST = 50
        ONESHOT_SPELLCAST = 51
        ONESHOT_BATTLEROAR = 53
        ONESHOT_SPECIALATTACK1H = 54
        ONESHOT_KICK = 60
        ONESHOT_ATTACKTHROWN = 61
        STATE_STUN = 64
        STATE_DEAD = 65
        ONESHOT_SALUTE = 66
        STATE_KNEEL = 68
        STATE_USESTANDING = 69
        ONESHOT_WAVE_NOSHEATHE = 70
        ONESHOT_CHEER_NOSHEATHE = 71
        ONESHOT_EAT_NOSHEATHE = 92
        STATE_STUN_NOSHEATHE = 93
        ONESHOT_DANCE = 94
        ONESHOT_SALUTE_NOSHEATH = 113
        STATE_USESTANDING_NOSHEATHE = 133
        ONESHOT_LAUGH_NOSHEATHE = 153
        STATE_WORK_NOSHEATHE = 173
        zzOLDONESHOT_LIFTOFF = 253
        ONESHOT_LIFTOFF = 254
        ONESHOT_YES = 273
        ONESHOT_NO = 274
        ONESHOT_TRAIN = 275
        STATE_AT_EASE = 313
        STATE_SPELLKNEELSTART = 353
        ONESHOT_SUBMERGE = 374
        STATE_CANNIBALIZE = 398
    End Enum

    Public Enum EmoteStates As Integer
        ANIM_STAND = &H0
        ANIM_DEATH = &H1
        ANIM_SPELL = &H2
        ANIM_STOP = &H3
        ANIM_WALK = &H4
        ANIM_RUN = &H5
        ANIM_DEAD = &H6
        ANIM_RISE = &H7
        ANIM_STANDWOUND = &H8
        ANIM_COMBATWOUND = &H9
        ANIM_COMBATCRITICAL = &HA
        ANIM_SHUFFLE_LEFT = &HB
        ANIM_SHUFFLE_RIGHT = &HC
        ANIM_WALK_BACKWARDS = &HD
        ANIM_STUN = &HE
        ANIM_HANDS_CLOSED = &HF
        ANIM_ATTACKUNARMED = &H10
        ANIM_ATTACK1H = &H11
        ANIM_ATTACK2HTIGHT = &H12
        ANIM_ATTACK2HLOOSE = &H13
        ANIM_PARRYUNARMED = &H14
        ANIM_PARRY1H = &H15
        ANIM_PARRY2HTIGHT = &H16
        ANIM_PARRY2HLOOSE = &H17
        ANIM_PARRYSHIELD = &H18
        ANIM_READYUNARMED = &H19
        ANIM_READY1H = &H1A
        ANIM_READY2HTIGHT = &H1B
        ANIM_READY2HLOOSE = &H1C
        ANIM_READYBOW = &H1D
        ANIM_DODGE = &H1E
        ANIM_SPELLPRECAST = &H1F
        ANIM_SPELLCAST = &H20
        ANIM_SPELLCASTAREA = &H21
        ANIM_NPCWELCOME = &H22
        ANIM_NPCGOODBYE = &H23
        ANIM_BLOCK = &H24
        ANIM_JUMPSTART = &H25
        ANIM_JUMP = &H26
        ANIM_JUMPEND = &H27
        ANIM_FALL = &H28
        ANIM_SWIMIDLE = &H29
        ANIM_SWIM = &H2A
        ANIM_SWIM_LEFT = &H2B
        ANIM_SWIM_RIGHT = &H2C
        ANIM_SWIM_BACKWARDS = &H2D
        ANIM_ATTACKBOW = &H2E
        ANIM_FIREBOW = &H2F
        ANIM_READYRIFLE = &H30
        ANIM_ATTACKRIFLE = &H31
        ANIM_LOOT = &H32
        ANIM_SPELL_PRECAST_DIRECTED = &H33
        ANIM_SPELL_PRECAST_OMNI = &H34
        ANIM_SPELL_CAST_DIRECTED = &H35
        ANIM_SPELL_CAST_OMNI = &H36
        ANIM_SPELL_BATTLEROAR = &H37
        ANIM_SPELL_READYABILITY = &H38
        ANIM_SPELL_SPECIAL1H = &H39
        ANIM_SPELL_SPECIAL2H = &H3A
        ANIM_SPELL_SHIELDBASH = &H3B
        ANIM_EMOTE_TALK = &H3C
        ANIM_EMOTE_EAT = &H3D
        ANIM_EMOTE_WORK = &H3E
        ANIM_EMOTE_USE_STANDING = &H3F
        ANIM_EMOTE_EXCLAMATION = &H40
        ANIM_EMOTE_QUESTION = &H41
        ANIM_EMOTE_BOW = &H42
        ANIM_EMOTE_WAVE = &H43
        ANIM_EMOTE_CHEER = &H44
        ANIM_EMOTE_DANCE = &H45
        ANIM_EMOTE_LAUGH = &H46
        ANIM_EMOTE_SLEEP = &H47
        ANIM_EMOTE_SIT_GROUND = &H48
        ANIM_EMOTE_RUDE = &H49
        ANIM_EMOTE_ROAR = &H4A
        ANIM_EMOTE_KNEEL = &H4B
        ANIM_EMOTE_KISS = &H4C
        ANIM_EMOTE_CRY = &H4D
        ANIM_EMOTE_CHICKEN = &H4E
        ANIM_EMOTE_BEG = &H4F
        ANIM_EMOTE_APPLAUD = &H50
        ANIM_EMOTE_SHOUT = &H51
        ANIM_EMOTE_FLEX = &H52
        ANIM_EMOTE_SHY = &H53
        ANIM_EMOTE_POINT = &H54
        ANIM_ATTACK1HPIERCE = &H55
        ANIM_ATTACK2HLOOSEPIERCE = &H56
        ANIM_ATTACKOFF = &H57
        ANIM_ATTACKOFFPIERCE = &H58
        ANIM_SHEATHE = &H59
        ANIM_HIPSHEATHE = &H5A
        ANIM_MOUNT = &H5B
        ANIM_RUN_LEANRIGHT = &H5C
        ANIM_RUN_LEANLEFT = &H5D
        ANIM_MOUNT_SPECIAL = &H5E
        ANIM_KICK = &H5F
        ANIM_SITDOWN = &H60
        ANIM_SITTING = &H61
        ANIM_SITUP = &H62
        ANIM_SLEEPDOWN = &H63
        ANIM_SLEEPING = &H64
        ANIM_SLEEPUP = &H65
        ANIM_SITCHAIRLOW = &H66
        ANIM_SITCHAIRMEDIUM = &H67
        ANIM_SITCHAIRHIGH = &H68
        ANIM_LOADBOW = &H69
        ANIM_LOADRIFLE = &H6A
        ANIM_ATTACKTHROWN = &H6B
        ANIM_READYTHROWN = &H6C
        ANIM_HOLDBOW = &H6D
        ANIM_HOLDRIFLE = &H6E
        ANIM_HOLDTHROWN = &H6F
        ANIM_LOADTHROWN = &H70
        ANIM_EMOTE_SALUTE = &H71
        ANIM_KNEELDOWN = &H72
        ANIM_KNEELING = &H73
        ANIM_KNEELUP = &H74
        ANIM_ATTACKUNARMEDOFF = &H75
        ANIM_SPECIALUNARMED = &H76
        ANIM_STEALTHWALK = &H77
        ANIM_STEALTHSTAND = &H78
        ANIM_KNOCKDOWN = &H79
        ANIM_EATING = &H7A
        ANIM_USESTANDINGLOOP = &H7B
        ANIM_CHANNELCASTDIRECTED = &H7C
        ANIM_CHANNELCASTOMNI = &H7D
        ANIM_WHIRLWIND = &H7E
        ANIM_BIRTH = &H7F
        ANIM_USESTANDINGSTART = &H80
        ANIM_USESTANDINGEND = &H81
        ANIM_HOWL = &H82
        ANIM_DROWN = &H83
        ANIM_DROWNED = &H84
        ANIM_FISHINGCAST = &H85
        ANIM_FISHINGLOOP = &H86
    End Enum
#End Region

#Region "SkillLines"
    Public Enum SKILL_LineCategory
        ATTRIBUTES = 5
        WEAPON_SKILLS = 6
        CLASS_SKILLS = 7
        ARMOR_PROFICIENCES = 8
        SECONDARY_SKILLS = 9
        LANGUAGES = 10
        PROFESSIONS = 11
        NOT_DISPLAYED = 12
    End Enum

    Public Enum SKILL_IDs As Integer
        SKILL_NONE = 0
        SKILL_FROST = 6
        SKILL_FIRE = 8
        SKILL_ARMS = 26
        SKILL_COMBAT = 38
        SKILL_SUBTLETY = 39
        SKILL_POISONS = 40
        SKILL_SWORDS = 43                  ' Higher weapon skill increases your chance to hit.
        SKILL_AXES = 44                    ' Higher weapon skill increases your chance to hit.
        SKILL_BOWS = 45                    ' Higher weapon skill increases your chance to hit.
        SKILL_GUNS = 46                    ' Higher weapon skill increases your chance to hit.
        SKILL_BEAST_MASTERY = 50
        SKILL_SURVIVAL = 51
        SKILL_MACES = 54                   ' Higher weapon skill increases your chance to hit.
        SKILL_TWO_HANDED_SWORDS = 55       ' Higher weapon skill increases your chance to hit.
        SKILL_HOLY = 56
        SKILL_SHADOW_MAGIC = 78
        SKILL_DEFENSE = 95                 ' Higher defense makes you harder to hit and makes monsters less likely to land a crushing blow.
        SKILL_LANGUAGE_COMMON = 98
        SKILL_DWARVEN_RACIAL = 101
        SKILL_LANGUAGE_ORCISH = 109
        SKILL_LANGUAGE_DWARVEN = 111
        SKILL_LANGUAGE_DARNASSIAN = 113
        SKILL_LANGUAGE_TAURAHE = 115
        SKILL_DUAL_WIELD = 118
        SKILL_TAUREN_RACIAL = 124
        SKILL_ORC_RACIAL = 125
        SKILL_NIGHT_ELF_RACIAL = 126
        SKILL_FIRST_AID = 129               ' Higher first aid skill allows you to learn higher level first aid abilities.  First aid abilities can be found on trainers around the world as well as from quests and as drops from monsters.
        SKILL_FERAL_COMBAT = 134
        SKILL_STAVES = 136                  ' Higher weapon skill increases your chance to hit.
        SKILL_LANGUAGE_THALASSIAN = 137
        SKILL_LANGUAGE_DRACONIC = 138
        SKILL_LANGUAGE_DEMON_TONGUE = 139
        SKILL_LANGUAGE_TITAN = 140
        SKILL_LANGUAGE_OLD_TONGUE = 141
        SKILL_SURVIVAL_1 = 142
        SKILL_HORSE_RIDING = 148
        SKILL_WOLF_RIDING = 149
        SKILL_TIGER_RIDING = 150
        SKILL_RAM_RIDING = 152
        SKILL_SWIMMING = 155
        SKILL_TWO_HANDED_MACES = 160        ' Higher weapon skill increases your chance to hit.
        SKILL_UNARMED = 162                 ' Higher skill increases your chance to hit.
        SKILL_MARKSMANSHIP = 163
        SKILL_BLACKSMITHING = 164           ' Higher smithing skill allows you to learn higher level smithing plans.  Blacksmithing plans can be found on trainers around the world as well as from quests and monsters.
        SKILL_LEATHERWORKING = 165          ' Higher leatherworking skill allows you to learn higher level leatherworking patterns.  Leatherworking patterns can be found on trainers around the world as well as from quests and monsters.
        SKILL_ALCHEMY = 171                 ' Higher alchemy skill allows you to learn higher level alchemy recipes.  Alchemy recipes can be found on trainers around the world as well as from quests and monsters.
        SKILL_TWO_HANDED_AXES = 172         ' Higher weapon skill increases your chance to hit.
        SKILL_DAGGERS = 173                 ' Higher weapon skill increases your chance to hit.
        SKILL_THROWN = 176                  ' Higher weapon skill increases your chance to hit.
        SKILL_HERBALISM = 182               ' Higher herbalism skill allows you to harvest more difficult herbs around the world.  If you cannot harvest a specific herb
        SKILL_GENERIC_DND = 183
        SKILL_RETRIBUTION = 184
        SKILL_COOKING = 185                 ' Higher cooking skill allows you to learn higher level cooking recipes.  Recipes can be found on trainers around the world as well as from quests and as drops from monsters.
        SKILL_MINING = 186                  ' Higher mining skill allows you to harvest more difficult minerals nodes around the world.  If you cannot harvest a specific mineral
        SKILL_PET_IMP = 188
        SKILL_PET_FELHUNTER = 189
        SKILL_TAILORING = 197               ' Higher tailoring skill allows you to learn higher level tailoring patterns.  Tailoring patterns can be found on trainers around the world as well as from quests and monsters.
        SKILL_ENGINEERING = 202             ' Higher engineering skill allows you to learn higher level engineering schematics.  Schematics can be found on trainers around the world as well as from quests and monsters.
        SKILL_PET_SPIDER = 203
        SKILL_PET_VOIDWALKER = 204
        SKILL_PET_SUCCUBUS = 205
        SKILL_PET_INFERNAL = 206
        SKILL_PET_DOOMGUARD = 207
        SKILL_PET_WOLF = 208
        SKILL_PET_CAT = 209
        SKILL_PET_BEAR = 210
        SKILL_PET_BOAR = 211
        SKILL_PET_CROCILISK = 212
        SKILL_PET_CARRION_BIRD = 213
        SKILL_PET_CRAB = 214
        SKILL_PET_GORILLA = 215
        SKILL_PET_RAPTOR = 217
        SKILL_PET_TALLSTRIDER = 218
        SKILL_RACIAL_UNDEAD = 220
        SKILL_WEAPON_TALENTS = 222
        SKILL_CROSSBOWS = 226               ' Higher weapon skill increases your chance to hit.
        SKILL_SPEARS = 227
        SKILL_WANDS = 228
        SKILL_POLEARMS = 229                ' Higher weapon skill increases your chance to hit.
        SKILL_PET_SCORPID = 236
        SKILL_ARCANE = 237
        SKILL_PET_TURTLE = 251
        SKILL_ASSASSINATION = 253
        SKILL_FURY = 256
        SKILL_PROTECTION = 257
        SKILL_BEAST_TRAINING = 261
        SKILL_PROTECTION_1 = 267
        SKILL_PET_TALENTS = 270
        SKILL_PLATE_MAIL = 293              ' Allows the wearing of plate armor.
        SKILL_LANGUAGE_GNOMISH = 313
        SKILL_LANGUAGE_TROLL = 315
        SKILL_ENCHANTING = 333              ' Higher enchanting skill allows you to learn more powerful forumulas.  Formulas can be found on trainers around the world as well as from quests and monsters.
        SKILL_DEMONOLOGY = 354
        SKILL_AFFLICTION = 355
        SKILL_FISHING = 356                 ' Higher fishing skill increases your chance of catching fish in bodies of water around the world.  If you are having trouble catching fish in a given area
        SKILL_ENHANCEMENT = 373
        SKILL_RESTORATION = 374
        SKILL_ELEMENTAL_COMBAT = 375
        SKILL_SKINNING = 393                ' Higher skinning skill allows you to skin hides from higher level monsters around the world.    Once your skill is above 100
        SKILL_MAIL = 413                    ' Allows the wearing of mail armor.
        SKILL_LEATHER = 414                 ' Allows the wearing of leather armor.
        SKILL_CLOTH = 415                   ' Allows the wearing of cloth armor.
        SKILL_SHIELD = 433                  ' Allows the use of shields.
        SKILL_FIST_WEAPONS = 473            ' Allows for the use of fist weapons.  Chance to hit is determined by the Unarmed skill.
        SKILL_RAPTOR_RIDING = 533
        SKILL_MECHANOSTRIDER_PILOTING = 553
        SKILL_UNDEAD_HORSEMANSHIP = 554
        SKILL_RESTORATION_1 = 573
        SKILL_BALANCE = 574
        SKILL_DESTRUCTION = 593
        SKILL_HOLY_1 = 594
        SKILL_DISCIPLINE = 613
        SKILL_LOCKPICKING = 633
        SKILL_PET_BAT = 653
        SKILL_PET_HYENA = 654
        SKILL_PET_OWL = 655
        SKILL_PET_WIND_SERPENT = 656
        SKILL_LANGUAGE_GUTTERSPEAK = 673
        SKILL_KODO_RIDING = 713
        SKILL_RACIAL_TROLL = 733
        SKILL_RACIAL_GNOME = 753
        SKILL_RACIAL_HUMAN = 754
        SKILL_JEWELCRAFTING = 755
        SKILL_RACIAL_BLOODELF = 756
        SKILL_PET_EVENT_REMOTE_CONTROL = 758
        SKILL_LANGUAGE_DRAENEI = 759
        SKILL_RACIAL_DRAENEI = 760
        SKILL_PET_FELGUARD = 761
        SKILL_RIDING = 762
        SKILL_PET_DRAGONHAWK = 763
        SKILL_PET_NETHER_RAY = 764
        SKILL_PET_SPOREBAT = 765
        SKILL_PET_WARP_STALKER = 766
        SKILL_PET_RAVAGER = 767
        SKILL_PET_SERPENT = 768
        SKILL_INTERNAL = 769
    End Enum

    Public SkillLines As New Dictionary(Of Integer, Integer)

    Public SkillLineAbility As New Dictionary(Of Integer, TSkillLineAbility)
    Public Class TSkillLineAbility
        Public ID As Integer
        Public SkillID As Integer
        Public SpellID As Integer
        Public Unknown1 As Integer
        Public Unknown2 As Integer
        Public Unknown3 As Integer
        Public Unknown4 As Integer
        Public Required_Skill_Value As Integer ' For Trade Skill, Not For Training
        Public Forward_SpellID As Integer
        Public Unknown5 As Integer
        Public Max_Value As Integer
        Public Min_Value As Integer
    End Class

#End Region
#Region "Taxi"
    Public TaxiNodes As New Dictionary(Of Integer, TTaxiNode)
    Public TaxiPaths As New Dictionary(Of Integer, TTaxiPath)
    Public TaxiPathNodes As New Dictionary(Of Integer, Dictionary(Of Integer, TTaxiPathNode))
    Public Class TTaxiNode
        Public x As Single
        Public y As Single
        Public z As Single
        Public MapID As Integer
        Public HordeMount As Integer = 0
        Public AllianceMount As Integer = 0

        Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMapID As Integer, ByVal pHMount As Integer, ByVal pAMount As Integer)
            x = px
            y = py
            z = pz
            MapID = pMapID
            HordeMount = pHMount
            AllianceMount = pAMount
        End Sub
    End Class

    Public Class TTaxiPath
        Public TFrom As Integer
        Public TTo As Integer
        Public Price As Integer

        Public Sub New(ByVal pFrom As Integer, ByVal pTo As Integer, ByVal pPrice As Integer)
            TFrom = pFrom
            TTo = pTo
            Price = pPrice
        End Sub
    End Class

    Public Class TTaxiPathNode
        Public Path As Integer
        Public Seq As Integer
        Public MapID As Integer
        Public x As Single
        Public y As Single
        Public z As Single
        Public action As Integer
        Public waittime As Integer

        Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMapID As Integer, ByVal pPath As Integer, ByVal pSeq As Integer, ByVal pAction As Integer, ByVal pWaittime As Integer)
            x = px
            y = py
            z = pz
            MapID = pMapID
            Path = pPath
            Seq = pSeq
            action = pAction
            waittime = pWaittime
        End Sub

    End Class

    Public Function GetNearestTaxi(ByVal x As Single, ByVal y As Single, ByVal map As Integer) As Integer
        Dim minDistance As Single = 99999999.0F
        Dim selectedTaxiNode As Integer = 0
        Dim tmp As Single

        For Each TaxiNode As KeyValuePair(Of Integer, TTaxiNode) In TaxiNodes
            If TaxiNode.Value.MapID = map Then
                tmp = GetDistance(x, TaxiNode.Value.x, y, TaxiNode.Value.y)
                If tmp < minDistance Then
                    minDistance = tmp
                    selectedTaxiNode = TaxiNode.Key
                End If
            End If
        Next
        Return selectedTaxiNode
    End Function
#End Region

#Region "Talents"
    Public TalentsTab As New Dictionary(Of Integer, Integer)(30)
    Public Talents As New Dictionary(Of Integer, TalentInfo)(500)
    Public Class TalentInfo
        Public TalentID As Integer
        Public TalentTab As Integer
        Public Row As Integer
        Public Col As Integer
        Public RankID(4) As Integer
        Public RequiredTalent(2) As Integer
        Public RequiredPoints(2) As Integer
    End Class
#End Region

#Region "Factions"
    Public Const FACTION_TEMPLATES_COUNT As Integer = 2074
    Enum FactionTemplates
        ' Fields
        None = 0
        PLAYERHuman = 1
        PLAYEROrc = 2
        PLAYERDwarf = 3
        PLAYERNightElf = 4
        PLAYERUndead = 5
        PLAYERTauren = 6
        Creature = 7
        Escortee = 10
        Stormwind = 11
        Stormwind_2 = 12
        Monster = 14
        Creature_2 = 15
        Monster_2 = 16
        DefiasBrotherhood = 17
        Murloc = 18
        GnollRedridge = 19
        GnollRiverpaw = 20
        UndeadScourge = 21
        BeastSpider = 22
        GnomereganExiles = 23
        Worgen = 24
        Kobold = 25
        Kobold_2 = 26
        DefiasBrotherhood_2 = 27
        TrollBloodscalp = 28
        Orgrimmar = 29
        TrollSkullsplitter = 30
        Prey = 31
        BeastWolf = 32
        Escortee_2 = 33
        DefiasBrotherhood_3 = 34
        Friendly = 35
        Trogg = 36
        TrollFrostmane = 37
        BeastWolf_2 = 38
        GnollShadowhide = 39
        OrcBlackrock = 40
        Villian = 41
        Victim = 42
        Villian_2 = 43
        BeastBear = 44
        Ogre = 45
        KurzensMercenaries = 46
        VentureCompany = 47
        BeastRaptor = 48
        Basilisk = 49
        DragonflightGreen = 50
        LostOnes = 51
        GizlocksDummy = 52
        HumanNightWatch = 53
        DarkIronDwarves = 54
        Ironforge = 55
        HumanNightWatch_2 = 56
        Ironforge_2 = 57
        Creature_3 = 58
        Trogg_2 = 59
        DragonflightRed = 60
        GnollMosshide = 61
        OrcDragonmaw = 62
        GnomeLeper = 63
        GnomereganExiles_2 = 64
        Orgrimmar_2 = 65
        Leopard = 66
        ScarletCrusade = 67
        Undercity = 68
        Ratchet = 69
        GnollRothide = 70
        Undercity_2 = 71
        BeastGorilla = 72
        BeastCarrionBird = 73
        Naga = 74
        Dalaran = 76
        ForlornSpirit = 77
        Darkhowl = 78
        Darnassus = 79
        Darnassus_2 = 80
        Grell = 81
        Furbolg = 82
        HordeGeneric = 83
        AllianceGeneric = 84
        Orgrimmar_3 = 85
        GizlocksCharm = 86
        Syndicate = 87
        HillsbradMilitia = 88
        ScarletCrusade_2 = 89
        Demon = 90
        Elemental = 91
        Spirit = 92
        Monster_3 = 93
        Treasure = 94
        GnollMudsnout = 95
        HIllsbradSouthshoreMayor = 96
        Syndicate_2 = 97
        Undercity_3 = 98
        Victim_2 = 99
        Treasure_2 = 100
        Treasure_3 = 101
        Treasure_4 = 102
        DragonflightBlack = 103
        ThunderBluff = 104
        ThunderBluff_2 = 105
        HordeGeneric_2 = 106
        TrollFrostmane_2 = 107
        Syndicate_3 = 108
        QuilboarRazormane2 = 109
        QuilboarRazormane2_2 = 110
        QuilboarBristleback = 111
        QuilboarBristleback_2 = 112
        Escortee_3 = 113
        Treasure_5 = 114
        PLAYERGnome = 115
        PLAYERTroll = 116
        Undercity_4 = 118
        BloodsailBuccaneers = 119
        BootyBay = 120
        BootyBay_2 = 121
        Ironforge_3 = 122
        Stormwind_3 = 123
        Darnassus_3 = 124
        Orgrimmar_4 = 125
        DarkspearTrolls = 126
        Villian_3 = 127
        Blackfathom = 128
        Makrura = 129
        CentaurKolkar = 130
        CentaurGalak = 131
        GelkisClanCentaur = 132
        MagramClanCentaur = 133
        Maraudine = 134
        Monster_4 = 148
        Theramore = 149
        Theramore_2 = 150
        Theramore_3 = 151
        QuilboarRazorfen = 152
        QuilboarRazorfen_2 = 153
        QuilboarDeathshead = 154
        Enemy = 168
        Ambient = 188
        Creature_4 = 189
        Ambient_2 = 190
        NethergardeCaravan = 208
        NethergardeCaravan_2 = 209
        AllianceGeneric_2 = 210
        SouthseaFreebooters = 230
        Escortee_4 = 231
        Escortee_5 = 232
        UndeadScourge_2 = 233
        Escortee_6 = 250
        WailingCaverns = 270
        Escortee_7 = 290
        Silithid = 310
        Silithid_2 = 311
        BeastSpider_2 = 312
        WailingCaverns_2 = 330
        Blackfathom_2 = 350
        ArmiesOfCThun = 370
        SilvermoonRemnant = 371
        BootyBay_3 = 390
        Basilisk_2 = 410
        BeastBat = 411
        TheDefilers = 412
        Scorpid = 413
        TimbermawHold = 414
        Titan = 415
        Titan_2 = 416
        TaskmasterFizzule = 430
        WailingCaverns_3 = 450
        Titan_3 = 470
        Ravenholdt = 471
        Syndicate_4 = 472
        Ravenholdt_2 = 473
        Gadgetzan = 474
        Gadgetzan_2 = 475
        GnomereganBug = 494
        Escortee_8 = 495
        Harpy = 514
        AllianceGeneric_3 = 534
        BurningBlade = 554
        ShadowsilkPoacher = 574
        SearingSpider = 575
        Trogg_3 = 594
        Victim_3 = 614
        Monster_5 = 634
        CenarionCircle = 635
        TimbermawHold_2 = 636
        Ratchet_2 = 637
        TrollWitherbark = 654
        CentaurKolkar_2 = 655
        DarkIronDwarves_2 = 674
        AllianceGeneric_4 = 694
        HydraxianWaterlords = 695
        HordeGeneric_3 = 714
        DarkIronDwarves_3 = 734
        GoblinDarkIronBarPatron = 735
        GoblinDarkIronBarPatron_2 = 736
        DarkIronDwarves_4 = 754
        Escortee_9 = 774
        Escortee_10 = 775
        BroodOfNozdormu = 776
        MightOfKalimdor = 777
        Giant = 778
        ArgentDawn = 794
        TrollVilebranch = 795
        ArgentDawn_2 = 814
        Elemental_2 = 834
        Everlook = 854
        Everlook_2 = 855
        WintersaberTrainers = 874
        GnomereganExiles_3 = 875
        DarkspearTrolls_2 = 876
        DarkspearTrolls_3 = 877
        Theramore_4 = 894
        TrainingDummy = 914
        FurbolgUncorrupted = 934
        Demon_2 = 954
        UndeadScourge_3 = 974
        CenarionCircle_2 = 994
        ThunderBluff_3 = 995
        CenarionCircle_3 = 996
        ShatterspearTrolls = 1014
        ShatterspearTrolls_2 = 1015
        HordeGeneric_4 = 1034
        AllianceGeneric_5 = 1054
        AllianceGeneric_6 = 1055
        Orgrimmar_5 = 1074
        Theramore_5 = 1075
        Darnassus_4 = 1076
        Theramore_6 = 1077
        Stormwind_4 = 1078
        Friendly_2 = 1080
        Elemental_3 = 1081
        BeastBoar = 1094
        TrainingDummy_2 = 1095
        Theramore_7 = 1096
        Darnassus_5 = 1097
        DragonflightBlackBait = 1114
        Undercity_5 = 1134
        Undercity_6 = 1154
        Orgrimmar_6 = 1174
        BattlegroundNeutral = 1194
        FrostwolfClan = 1214
        FrostwolfClan_2 = 1215
        StormpikeGuard = 1216
        StormpikeGuard_2 = 1217
        SulfuronFirelords = 1234
        SulfuronFirelords_2 = 1235
        SulfuronFirelords_3 = 1236
        CenarionCircle_4 = 1254
        Creature_5 = 1274
        Creature_6 = 1275
        Gizlock = 1294
        HordeGeneric_5 = 1314
        AllianceGeneric_7 = 1315
        StormpikeGuard_3 = 1334
        FrostwolfClan_3 = 1335
        ShenDralar = 1354
        ShenDralar_2 = 1355
        OgreCaptainKromcrush = 1374
        Treasure_6 = 1375
        DragonflightBlack_2 = 1394
        SilithidAttackers = 1395
        SpiritGuideAlliance = 1414
        SpiritGuideHorde = 1415
        Jaedenar = 1434
        Victim_4 = 1454
        ThoriumBrotherhood = 1474
        ThoriumBrotherhood_2 = 1475
        HordeGeneric_6 = 1494
        HordeGeneric_7 = 1495
        HordeGeneric_8 = 1496
        SilverwingSentinels = 1514
        WarsongOutriders = 1515
        StormpikeGuard_4 = 1534
        FrostwolfClan_4 = 1554
        DarkmoonFaire = 1555
        ZandalarTribe = 1574
        Stormwind_5 = 1575
        SilvermoonRemnant_2 = 1576
        TheLeagueOfArathor = 1577
        Darnassus_6 = 1594
        Orgrimmar_7 = 1595
        StormpikeGuard_5 = 1596
        FrostwolfClan_5 = 1597
        TheDefilers_2 = 1598
        TheLeagueOfArathor_2 = 1599
        Darnassus_7 = 1600
        BroodOfNozdormu_2 = 1601
        SilvermoonCity = 1602
        SilvermoonCity_2 = 1603
        SilvermoonCity_3 = 1604
        DragonflightBronze = 1605
        Creature_7 = 1606
        Creature_8 = 1607
        CenarionCircle_5 = 1608
        PLAYERBloodElf = 1610
        Ironforge_4 = 1611
        Orgrimmar_8 = 1612
        MightOfKalimdor_2 = 1613
        Monster_6 = 1614
        SteamwheedleCartel = 1615
        RCObjects = 1616
        RCEnemies = 1617
        Ironforge_5 = 1618
        Orgrimmar_9 = 1619
        Enemy_2 = 1620
        Blue = 1621
        Red = 1622
        Tranquillien = 1623
        ArgentDawn_3 = 1624
        ArgentDawn_4 = 1625
        UndeadScourge_4 = 1626
        Farstriders = 1627
        Tranquillien_2 = 1628
        PLAYERDraenei = 1629
        ScourgeInvaders = 1630
        ScourgeInvaders_2 = 1634
        SteamwheedleCartel_2 = 1635
        Farstriders_2 = 1636
        Farstriders_3 = 1637
        Exodar = 1638
        Exodar_2 = 1639
        Exodar_3 = 1640
        WarsongOutriders_2 = 1641
        SilverwingSentinels_2 = 1642
        TrollForest = 1643
        TheSonsOfLothar = 1644
        TheSonsOfLothar_2 = 1645
        Exodar_4 = 1646
        Exodar_5 = 1647
        TheSonsOfLothar_3 = 1648
        TheSonsOfLothar_4 = 1649
        TheMagHar = 1650
        TheMagHar_2 = 1651
        TheMagHar_3 = 1652
        TheMagHar_4 = 1653
        Exodar_6 = 1654
        Exodar_7 = 1655
        SilvermoonCity_4 = 1656
        SilvermoonCity_5 = 1657
        SilvermoonCity_6 = 1658
        CenarionExpedition = 1659
        CenarionExpedition_2 = 1660
        CenarionExpedition_3 = 1661
        FelOrc = 1662
        FelOrcGhost = 1663
        SonsOfLotharGhosts = 1664
        HonorHold = 1666
        HonorHold_2 = 1667
        Thrallmar = 1668
        Thrallmar_2 = 1669
        Thrallmar_3 = 1670
        HonorHold_3 = 1671
        TestFaction1 = 1672
        ToWoWFlag = 1673
        TestFaction4 = 1674
        TestFaction3 = 1675
        ToWoWFlagTriggerHordeDND = 1676
        ToWoWFlagTriggerAllianceDND = 1677
        Ethereum = 1678
        Broken = 1679
        Elemental_4 = 1680
        EarthElemental = 1681
        FightingRobots = 1682
        ActorGood = 1683
        ActorEvil = 1684
        StillpineFurbolg = 1685
        StillpineFurbolg_2 = 1686
        CrazedOwlkin = 1687
        ChessAlliance = 1688
        ChessHorde = 1689
        ChessAlliance_2 = 1690
        ChessHorde_2 = 1691
        MonsterSpar = 1692
        MonsterSparBuddy = 1693
        Exodar_8 = 1694
        SilvermoonCity_7 = 1695
        TheVioletEye = 1696
        FelOrc_2 = 1697
        Exodar_9 = 1698
        Exodar_10 = 1699
        Exodar_11 = 1700
        Sunhawks = 1701
        Sunhawks_2 = 1702
        TrainingDummy_3 = 1703
        FelOrc_3 = 1704
        FelOrc_4 = 1705
        FungalGiant = 1706
        Sporeggar = 1707
        Sporeggar_2 = 1708
        Sporeggar_3 = 1709
        CenarionExpedition_4 = 1710
        MonsterPredator = 1711
        MonsterPrey = 1712
        MonsterPrey_2 = 1713
        Sunhawks_3 = 1714
        VoidAnomaly = 1715
        HyjalDefenders = 1716
        HyjalDefenders_2 = 1717
        HyjalDefenders_3 = 1718
        HyjalDefenders_4 = 1719
        HyjalInvaders = 1720
        Kurenai = 1721
        Kurenai_2 = 1722
        Kurenai_3 = 1723
        Kurenai_4 = 1724
        EarthenRing = 1725
        EarthenRing_2 = 1726
        EarthenRing_3 = 1727
        CenarionExpedition_5 = 1728
        Thrallmar_4 = 1729
        TheConsortium = 1730
        TheConsortium_2 = 1731
        AllianceGeneric_8 = 1732
        AllianceGeneric_9 = 1733
        HordeGeneric_9 = 1734
        HordeGeneric_10 = 1735
        MonsterSparBuddy_2 = 1736
        HonorHold_4 = 1737
        Arakkoa = 1738
        ZangarmarshBannerAlliance = 1739
        ZangarmarshBannerHorde = 1740
        TheShaTar = 1741
        ZangarmarshBannerNeutral = 1742
        TheAldor = 1743
        TheScryers = 1744
        SilvermoonCity_8 = 1745
        TheScryers_2 = 1746
        CavernsOfTimeThrall = 1747
        CavernsOfTimeDurnholde = 1748
        CavernsOfTimeSouthshoreGuards = 1749
        ShadowCouncilCovert = 1750
        Monster_7 = 1751
        DarkPortalAttackerLegion = 1752
        DarkPortalAttackerLegion_2 = 1753
        DarkPortalAttackerLegion_3 = 1754
        DarkPortalDefenderAlliance = 1755
        DarkPortalDefenderAlliance_2 = 1756
        DarkPortalDefenderAlliance_3 = 1757
        DarkPortalDefenderHorde = 1758
        DarkPortalDefenderHorde_2 = 1759
        DarkPortalDefenderHorde_3 = 1760
        InciterTrigger = 1761
        InciterTrigger2 = 1762
        InciterTrigger3 = 1763
        InciterTrigger4 = 1764
        InciterTrigger5 = 1765
        ArgentDawn_5 = 1766
        ArgentDawn_6 = 1767
        Demon_3 = 1768
        Demon_4 = 1769
        ActorGood_2 = 1770
        ActorEvil_2 = 1771
        ManaCreature = 1772
        KhadgarsServant = 1773
        Friendly_3 = 1774
        TheShaTar_2 = 1775
        TheAldor_2 = 1776
        TheAldor_3 = 1777
        TheScaleOfTheSands = 1778
        KeepersOfTime = 1779
        BladespireClan = 1780
        BloodmaulClan = 1781
        BladespireClan_2 = 1782
        BloodmaulClan_2 = 1783
        BladespireClan_3 = 1784
        BloodmaulClan_3 = 1785
        Demon_5 = 1786
        Monster_8 = 1787
        TheConsortium_3 = 1788
        Sunhawks_4 = 1789
        BladespireClan_4 = 1790
        BloodmaulClan_4 = 1791
        FelOrc_5 = 1792
        Sunhawks_5 = 1793
        Protectorate = 1794
        Protectorate_2 = 1795
        Ethereum_2 = 1796
        Protectorate_3 = 1797
        ArcaneAnnihilatorDNR = 1798
        EthereumSparbuddy = 1799
        Ethereum_3 = 1800
        Horde = 1801
        Alliance = 1802
        Ambient_3 = 1803
        Ambient_4 = 1804
        TheAldor_4 = 1805
        Friendly_4 = 1806
        Protectorate_4 = 1807
        KirinVarBelmara = 1808
        KirinVarCohlien = 1809
        KirinVarDathric = 1810
        KirinVarLuminrath = 1811
        Friendly_5 = 1812
        ServantOfIllidan = 1813
        MonsterSparBuddy_3 = 1814
        BeastWolf_3 = 1815
        Friendly_6 = 1816
        LowerCity = 1818
        AllianceGeneric_10 = 1819
        AshtongueDeathsworn = 1820
        SpiritsOfShadowmoon1 = 1821
        SpiritsOfShadowmoon2 = 1822
        Ethereum_4 = 1823
        Netherwing = 1824
        Demon_6 = 1825
        ServantOfIllidan_2 = 1826
        Wyrmcult = 1827
        Treant = 1828
        LeotherasDemonI = 1829
        LeotherasDemonII = 1830
        LeotherasDemonIII = 1831
        LeotherasDemonIV = 1832
        LeotherasDemonV = 1833
        Azaloth = 1834
        HordeGeneric_11 = 1835
        TheConsortium_4 = 1836
        Sporeggar_4 = 1837
        TheScryers_3 = 1838
        RockFlayer = 1839
        FlayerHunter = 1840
        ShadowmoonShade = 1841
        LegionCommunicator = 1842
        ServantOfIllidan_3 = 1843
        TheAldor_5 = 1844
        TheScryers_4 = 1845
        RavenswoodAncients = 1846
        MonsterSpar_2 = 1847
        MonsterSparBuddy_4 = 1848
        ServantOfIllidan_4 = 1849
        Netherwing_2 = 1850
        LowerCity_2 = 1851
        ChessFriendlyToAllChess = 1852
        ServantOfIllidan_5 = 1853
        TheAldor_6 = 1854
        TheScryers_5 = 1855
        ShaTariSkyguard = 1856
        Friendly_7 = 1857
        AshtongueDeathsworn_2 = 1858
        Maiev = 1859
        SkettisShadowyArakkoa = 1860
        SkettisArakkoa = 1862
        OrcDragonmaw_2 = 1863
        DragonmawEnemy = 1864
        OrcDragonmaw_3 = 1865
        AshtongueDeathsworn_3 = 1866
        Maiev_2 = 1867
        MonsterSparBuddy_5 = 1868
        Arakkoa_2 = 1869
        ShaTariSkyguard_2 = 1870
        SkettisArakkoa_2 = 1871
        OgriLa = 1872
        RockFlayer_2 = 1873
        OgriLa_2 = 1874
        TheAldor_7 = 1875
        TheScryers_6 = 1876
        OrcDragonmaw_4 = 1877
        Frenzy = 1878
        SkyguardEnemy = 1879
        OrcDragonmaw_5 = 1880
        SkettisArakkoa_3 = 1881
        ServantOfIllidan_6 = 1882
        TheramoreDeserter = 1883
        TrollAmani = 1890
        DarkmoonFaire_2 = 1896
        BeastRaptor_2 = 1909
        CTFFlags = 1913
        Maiev_3 = 1916
        RamRacingPowerupDND = 1930
        RamRacingTrapDND = 1931
        ActorGood_3 = 1934
        ActorGood_4 = 1935
        CraigsSquirrels = 1936
        CraigsSquirrels_2 = 1937
        CraigsSquirrels_3 = 1938
        CraigsSquirrels_4 = 1939
        CraigsSquirrels_5 = 1940
        CraigsSquirrels_6 = 1941
        CraigsSquirrels_7 = 1942
        CraigsSquirrels_8 = 1943
        CraigsSquirrels_9 = 1944
        CraigsSquirrels_10 = 1945
        Darnassus_8 = 1951
        HolidayWaterBarrel = 1952
        ShatteredSunOffensive = 1956
        ShatteredSunOffensive_2 = 1957
        ActorEvil_3 = 1958
        ActorEvil_4 = 1959
        ShatteredSunOffensive_3 = 1960
        FightingVanityPet = 1961
        UndeadScourge_5 = 1962
        Demon_7 = 1963
        UndeadScourge_6 = 1964
        MonsterSpar_3 = 1965
        Murloc_2 = 1966
        ShatteredSunOffensive_4 = 1967
        MonsterForceReaction = 1971
        ObjectForceReaction = 1972
        Ambient_5 = 1990
        Monster_9 = 1992
        CTFFlags_2 = 1995
        CTFFlags_3 = 1997
        HolidayMonster = 1998
        MonsterPredator_2 = 2029
        CavernsOfTimeDurnholde_2 = 2074
    End Enum                    'FactionTemplate.dbc       'Used in CREATUREs Database as Faction

    Public Enum TReaction As Byte
        HOSTILE = 0
        NEUTRAL = 1
        FRIENDLY = 2
        FIGHT_SUPPORT = 3
    End Enum

    Enum Factions
        None = 0
        PLAYERHuman = 1
        PLAYEROrc = 2
        PLAYERDwarf = 3
        PLAYERNightElf = 4
        PLAYERUndead = 5
        PLAYERTauren = 6
        Creature = 7
        PLAYERGnome = 8
        PLAYERTroll = 9
        Monster = 14
        DefiasBrotherhood = 15
        GnollRiverpaw = 16
        GnollRedridge = 17
        GnollShadowhide = 18
        Murloc = 19
        UndeadScourge = 20
        BootyBay = 21
        BeastSpider = 22
        BeastBoar = 23
        Worgen = 24
        Kobold = 25
        TrollBloodscalp = 26
        TrollSkullsplitter = 27
        Prey = 28
        BeastWolf = 29
        Friendly = 31
        Trogg = 32
        TrollFrostmane = 33
        OrcBlackrock = 34
        Villian = 35
        Victim = 36
        BeastBear = 37
        Ogre = 38
        KurzensMercenaries = 39
        Escortee = 40
        VentureCompany = 41
        BeastRaptor = 42
        Basilisk = 43
        DragonflightGreen = 44
        LostOnes = 45
        Ironforge = 47
        DarkIronDwarves = 48
        HumanNightWatch = 49
        DragonflightRed = 50
        GnollMosshide = 51
        OrcDragonmaw = 52
        GnomeLeper = 53
        GnomereganExiles = 54
        Leopard = 55
        ScarletCrusade = 56
        GnollRothide = 57
        BeastGorilla = 58
        ThoriumBrotherhood = 59
        Naga = 60
        Dalaran = 61
        ForlornSpirit = 62
        Darkhowl = 63
        Grell = 64
        Furbolg = 65
        HordeGeneric = 66
        Horde = 67
        Undercity = 68
        Darnassus = 69
        Syndicate = 70
        HillsbradMilitia = 71
        Stormwind = 72
        Demon = 73
        Elemental = 74
        Spirit = 75
        Orgrimmar = 76
        Treasure = 77
        GnollMudsnout = 78
        HIllsbradSouthshoreMayor = 79
        DragonflightBlack = 80
        ThunderBluff = 81
        TrollWitherbark = 82
        QuilboarBristleback = 85
        BloodsailBuccaneers = 87
        Blackfathom = 88
        Makrura = 89
        CentaurKolkar = 90
        CentaurGalak = 91
        GelkisClanCentaur = 92
        MagramClanCentaur = 93
        Maraudine = 94
        Theramore = 108
        QuilboarRazorfen = 109
        QuilboarRazormane2 = 110
        QuilboarDeathshead = 111
        Enemy = 128
        Ambient = 148
        NethergardeCaravan = 168
        SteamwheedleCartel = 169
        AllianceGeneric = 189
        WailingCaverns = 229
        Silithid = 249
        SilvermoonRemnant = 269
        ZandalarTribe = 270
        Scorpid = 309
        BeastBat = 310
        Titan = 311
        TaskmasterFizzule = 329
        Ravenholdt = 349
        Gadgetzan = 369
        GnomereganBug = 389
        Harpy = 409
        BurningBlade = 429
        ShadowsilkPoacher = 449
        SearingSpider = 450
        Alliance = 469
        Ratchet = 470
        GoblinDarkIronBarPatron = 489
        TheLeagueOfArathor = 509
        TheDefilers = 510
        Giant = 511
        ArgentDawn = 529
        DarkspearTrolls = 530
        DragonflightBronze = 531
        TrollVilebranch = 572
        SouthseaFreebooters = 573
        FurbolgUncorrupted = 575
        TimbermawHold = 576
        Everlook = 577
        WintersaberTrainers = 589
        CenarionCircle = 609
        ShatterspearTrolls = 629
        BeastCarrionBird = 669
        TrainingDummy = 679
        DragonflightBlackBait = 689
        BattlegroundNeutral = 709
        FrostwolfClan = 729
        StormpikeGuard = 730
        HydraxianWaterlords = 749
        SulfuronFirelords = 750
        GizlocksDummy = 769
        GizlocksCharm = 770
        Gizlock = 771
        SpiritGuideAlliance = 790
        ShenDralar = 809
        OgreCaptainKromcrush = 829
        SpiritGuideHorde = 849
        Jaedenar = 869
        WarsongOutriders = 889
        SilverwingSentinels = 890
        DarkmoonFaire = 909
        BroodOfNozdormu = 910
        SilvermoonCity = 911
        MightOfKalimdor = 912
        PLAYERBloodElf = 914
        ArmiesOfCThun = 915
        SilithidAttackers = 916
        RCEnemies = 918
        RCObjects = 919
        Red = 920
        Blue = 921
        Tranquillien = 922
        Farstriders = 923
        PLAYERDraenei = 927
        ScourgeInvaders = 928
        BloodmaulClan = 929
        Exodar = 930
        TheAldor = 932
        TheConsortium = 933
        TheScryers = 934
        TheShaTar = 935
        TrollForest = 937
        TheSonsOfLothar = 940
        TheMagHar = 941
        CenarionExpedition = 942
        FelOrc = 943
        FelOrcGhost = 944
        SonsOfLotharGhosts = 945
        HonorHold = 946
        Thrallmar = 947
        TestFaction1 = 949
        ToWoWFlag = 950
        ToWoWFlagTriggerAllianceDND = 951
        TestFaction3 = 952
        TestFaction4 = 953
        ToWoWFlagTriggerHordeDND = 954
        Broken = 955
        Ethereum = 956
        EarthElemental = 957
        FightingRobots = 958
        ActorGood = 959
        ActorEvil = 960
        StillpineFurbolg = 961
        CrazedOwlkin = 962
        ChessAlliance = 963
        ChessHorde = 964
        MonsterSpar = 965
        MonsterSparBuddy = 966
        TheVioletEye = 967
        Sunhawks = 968
        Sporeggar = 970
        FungalGiant = 971
        MonsterPredator = 973
        MonsterPrey = 974
        VoidAnomaly = 975
        HyjalDefenders = 976
        HyjalInvaders = 977
        Kurenai = 978
        EarthenRing = 979
        Arakkoa = 981
        ZangarmarshBannerAlliance = 982
        ZangarmarshBannerHorde = 983
        ZangarmarshBannerNeutral = 984
        CavernsOfTimeThrall = 985
        CavernsOfTimeDurnholde = 986
        CavernsOfTimeSouthshoreGuards = 987
        ShadowCouncilCovert = 988
        KeepersOfTime = 989
        TheScaleOfTheSands = 990
        DarkPortalDefenderAlliance = 991
        DarkPortalDefenderHorde = 992
        DarkPortalAttackerLegion = 993
        InciterTrigger = 994
        InciterTrigger2 = 995
        InciterTrigger3 = 996
        InciterTrigger4 = 997
        InciterTrigger5 = 998
        ManaCreature = 999
        KhadgarsServant = 1000
        BladespireClan = 1001
        EthereumSparbuddy = 1002
        Protectorate = 1003
        ArcaneAnnihilatorDNR = 1004
        KirinVarDathric = 1006
        KirinVarBelmara = 1007
        KirinVarLuminrath = 1008
        KirinVarCohlien = 1009
        ServantOfIllidan = 1010
        LowerCity = 1011
        AshtongueDeathsworn = 1012
        SpiritsOfShadowmoon1 = 1013
        SpiritsOfShadowmoon2 = 1014
        Netherwing = 1015
        Wyrmcult = 1016
        Treant = 1017
        LeotherasDemonI = 1018
        LeotherasDemonII = 1019
        LeotherasDemonIII = 1020
        LeotherasDemonIV = 1021
        LeotherasDemonV = 1022
        Azaloth = 1023
        RockFlayer = 1024
        FlayerHunter = 1025
        ShadowmoonShade = 1026
        LegionCommunicator = 1027
        RavenswoodAncients = 1028
        ChessFriendlyToAllChess = 1029
        ShaTariSkyguard = 1031
        Maiev = 1033
        SkettisShadowyArakkoa = 1034
        SkettisArakkoa = 1035
        DragonmawEnemy = 1036
        OgriLa = 1038
        Frenzy = 1041
        SkyguardEnemy = 1042
        TheramoreDeserter = 1044
        TrollAmani = 1049
        CTFFlags = 1059
        RamRacingPowerupDND = 1069
        RamRacingTrapDND = 1070
        CraigsSquirrels = 1071
        HolidayWaterBarrel = 1074
        ShatteredSunOffensive = 1077
        FightingVanityPet = 1078
        MonsterForceReaction = 1080
        ObjectForceReaction = 1081
        HolidayMonster = 1087
    End Enum        'Faction.dbc

    Public Enum FactionMasks
        FACTION_MASK_PLAYER = 1     'any player
        FACTION_MASK_ALLIANCE = 2   'player or creature from alliance team
        FACTION_MASK_HORDE = 4      'player or creature from horde team
        FACTION_MASK_MONSTER = 8    'aggressive creature from monster team
    End Enum

    Public CharRaces As New Dictionary(Of Integer, TCharRace)
    Public Class TCharRace
        Public FactionID As Short
        Public ModelMale As Integer
        Public ModelFemale As Integer
        Public TeamID As Byte
        Public TaxiMask As UInteger
        Public CinematicID As Integer
        Public RaceName As String

        Public Sub New(ByVal Faction As Short, ByVal ModelM As Integer, ByVal ModelF As Integer, ByVal Team As Byte, ByVal Taxi As UInteger, ByVal Cinematic As Integer, ByVal Name As String)
            FactionID = Faction
            ModelMale = ModelM
            ModelFemale = ModelF
            TeamID = Team
            TaxiMask = Taxi
            CinematicID = Cinematic
            RaceName = Name
        End Sub
    End Class

    Public CharClasses As New Dictionary(Of Integer, TCharClass)
    Public Class TCharClass
        Public CinematicID As Integer

        Public Sub New(ByVal Cinematic As Integer)
            CinematicID = Cinematic
        End Sub
    End Class

    Public FactionInfo As New Dictionary(Of Integer, TFaction)
    Public Class TFaction
        Public ID As Short
        Public VisibleID As Short
        Public flags(3) As Short
        Public rep_stats(3) As Integer
        Public rep_flags(3) As Byte

        Public Sub New(ByVal Id_ As Short, ByVal VisibleID_ As Short, ByVal flags1 As Integer, ByVal flags2 As Integer, ByVal flags3 As Integer, ByVal flags4 As Integer, ByVal rep_stats1 As Integer, ByVal rep_stats2 As Integer, ByVal rep_stats3 As Integer, ByVal rep_stats4 As Integer, ByVal rep_flags1 As Integer, ByVal rep_flags2 As Integer, ByVal rep_flags3 As Integer, ByVal rep_flags4 As Integer)
            ID = Id_
            VisibleID = VisibleID_
            flags(0) = flags1
            flags(1) = flags2
            flags(2) = flags3
            flags(3) = flags4
            rep_stats(0) = rep_stats1
            rep_stats(1) = rep_stats2
            rep_stats(2) = rep_stats3
            rep_stats(3) = rep_stats4
            rep_flags(0) = rep_flags1
            rep_flags(1) = rep_flags2
            rep_flags(2) = rep_flags3
            rep_flags(3) = rep_flags4
        End Sub
    End Class

    Public FactionTemplatesInfo As New Dictionary(Of Integer, TFactionTemplate)
    Public Class TFactionTemplate
        Public FactionID As Integer
        Public ourMask As UInteger
        Public friendMask As UInteger
        Public enemyMask As UInteger
        Public enemyFaction1 As Integer
        Public enemyFaction2 As Integer
        Public enemyFaction3 As Integer
        Public enemyFaction4 As Integer
        Public friendFaction1 As Integer
        Public friendFaction2 As Integer
        Public friendFaction3 As Integer
        Public friendFaction4 As Integer
    End Class
#End Region

#Region "Spells"
    Public SpellShapeShiftForm As New List(Of TSpellShapeshiftForm)
    Public Class TSpellShapeshiftForm
        Public ID As Integer = 0
        Public Flags1 As Integer = 0
        Public CreatureType As Integer
        Public AttackSpeed As Integer

        Public Sub New(ByVal ID_ As Integer, ByVal Flags1_ As Integer, ByVal CreatureType_ As Integer, ByVal AttackSpeed_ As Integer)
            ID = ID_
            Flags1 = Flags1_
            CreatureType = CreatureType_
            AttackSpeed = AttackSpeed_
        End Sub
    End Class

    Public Function FindShapeshiftForm(ByVal ID As Integer) As TSpellShapeshiftForm
        For Each Form As TSpellShapeshiftForm In SpellShapeShiftForm
            If Form.ID = ID Then
                Return Form
            End If
        Next

        Return Nothing
    End Function

    Public gtOCTRegenHP As New List(Of Single)
    Public gtOCTRegenMP As New List(Of Single)
    Public gtRegenHPPerSpt As New List(Of Single)
    Public gtRegenMPPerSpt As New List(Of Single)
#End Region

#Region "Items"
    Public Const DurabilityCosts_MAX As Integer = 300
    Public DurabilityCosts(DurabilityCosts_MAX, 28) As Short

    Public SpellItemEnchantments As New Dictionary(Of Integer, TSpellItemEnchantment)
    Public Class TSpellItemEnchantment
        Public Type(2) As Integer
        Public Amount(2) As Integer
        Public SpellID(2) As Integer
        Public AuraID As Integer
        Public Slot As Integer
        'Public EnchantmentConditions As Integer

        Public Sub New(ByVal Types() As Integer, ByVal Amounts() As Integer, ByVal SpellIDs() As Integer, ByVal AuraID_ As Integer, ByVal Slot_ As Integer) ', ByVal EnchantmentConditions_ As Integer)
            For i As Byte = 0 To 2
                Type(i) = Types(i)
                Amount(i) = Amounts(i)
                SpellID(i) = SpellIDs(i)
            Next
            AuraID = AuraID_
            Slot = Slot_
            'EnchantmentConditions = EnchantmentConditions_
        End Sub
    End Class

    Public ItemSet As New Dictionary(Of Integer, TItemSet)
    Public Class TItemSet
        Public ID As Integer ' 0
        Public Name As String ' 1
        Public ItemID(7) As Integer ' 10-17
        Public SpellID(7) As Integer ' 66-73
        Public ItemCount(7) As Integer ' 74-81
        Public Required_Skill_ID As Integer ' 82
        Public Required_Skill_Value As Integer ' 83

        Public Sub New(ByVal Name_ As String, ByVal ItemID_() As Integer, ByVal SpellID_() As Integer, ByVal ItemCount_() As Integer, ByVal Required_Skill_ID_ As Integer, ByVal Required_Skill_Value_ As Integer)
            For i As Byte = 0 To 7
                SpellID(i) = SpellID_(i)
                ItemID(i) = ItemID_(i)
                ItemCount(i) = ItemCount_(i)
            Next
            Name = Name_
            Required_Skill_ID = Required_Skill_ID_
            Required_Skill_Value = Required_Skill_Value_
        End Sub
    End Class

    Public ItemDisplayInfo As New Dictionary(Of Integer, TItemDisplayInfo)
    Public Class TItemDisplayInfo
        Public ID As Integer
        Public RandomPropertyChance As Integer
        Public Unknown As Integer
    End Class

    Public ItemRandomPropertiesInfo As New Dictionary(Of Integer, TItemRandomPropertiesInfo)
    Public Class TItemRandomPropertiesInfo
        Public ID As Integer
        Public Enchant_ID(3) As Integer
    End Class

#End Region

#Region "XPTable"
    ''' <summary>
    ''' Initializes the xp lookup table from db.
    ''' </summary>
    ''' <returns></returns>
    Private Sub InitializeXpTableFromDb()
        Dim result As DataTable = Nothing
        Dim dbLvl As Integer
        Dim dbXp As Long
        Try
            WorldDatabase.Query([String].Format("SELECT * FROM player_xp_for_level order by lvl;"), result)
            If result.Rows.Count > 0 Then
                For Each row As DataRow In result.Rows
                    dbLvl = row.Item("lvl")
                    dbXp = row.Item("xp_for_next_level")
                    XPTable(dbLvl) = dbXp
                Next
            End If
            Log.WriteLine(LogType.INFORMATION, "Initalizing: XPTable initialized.")
        Catch ex As Exception
            Log.WriteLine(LogType.FAILED, "XPTable initialization failed.")
        End Try
    End Sub
#End Region

#Region "Battlemasters"

    Public Battlemasters As New Dictionary(Of Integer, Byte)
    Public Sub InitializeBattlemasters()
        Dim MySQLQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM battlemaster_entry"), MySQLQuery)

        For Each row As DataRow In MySQLQuery.Rows
            Battlemasters.Add(CInt(row.Item("entry")), CByte(row.Item("bg_template")))
        Next

        Log.WriteLine(LogType.INFORMATION, "World: {0} Battlemasters Loaded.", MySQLQuery.Rows.Count)
    End Sub

#End Region

#Region "Battlegrounds"

    Public Battlegrounds As New Dictionary(Of Byte, TBattleground)
    Public Sub InitializeBattlegrounds()
        Dim Entry As Byte

        Dim MySQLQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM battleground_template"), MySQLQuery)

        For Each row As DataRow In MySQLQuery.Rows
            Entry = row.Item("id")
            Battlegrounds.Add(Entry, New TBattleground)

            Battlegrounds(Entry).Map = row.Item("Map")
            Battlegrounds(Entry).MinPlayersPerTeam = row.Item("MinPlayersPerTeam")
            Battlegrounds(Entry).MaxPlayersPerTeam = row.Item("MaxPlayersPerTeam")
            Battlegrounds(Entry).MinLevel = row.Item("MinLvl")
            Battlegrounds(Entry).MaxLevel = row.Item("MaxLvl")
            Battlegrounds(Entry).AllianceStartLoc = row.Item("AllianceStartLoc")
            Battlegrounds(Entry).AllianceStartO = row.Item("AllianceStartO")
            Battlegrounds(Entry).HordeStartLoc = row.Item("HordeStartLoc")
            Battlegrounds(Entry).HordeStartO = row.Item("HordeStartO")
        Next

        Log.WriteLine(LogType.INFORMATION, "World: {0} Battlegrounds Loaded.", MySQLQuery.Rows.Count)
    End Sub

    Public Class TBattleground
        Public Map As Integer
        Public MinPlayersPerTeam As Byte
        Public MaxPlayersPerTeam As Byte
        Public MinLevel As Byte
        Public MaxLevel As Byte
        Public AllianceStartLoc As Single
        Public AllianceStartO As Single
        Public HordeStartLoc As Single
        Public HordeStartO As Single
    End Class

#End Region

#Region "TeleportCoords"
    Public TeleportCoords As New Dictionary(Of Integer, TTeleportCoords)
    Public Sub InitializeTeleportCoords()
        Dim SpellID As Integer

        Dim MySQLQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM spells_teleport_coords"), MySQLQuery)

        For Each row As DataRow In MySQLQuery.Rows
            SpellID = row.Item("id")
            TeleportCoords.Add(SpellID, New TTeleportCoords)

            TeleportCoords(SpellID).Name = row.Item("name")
            TeleportCoords(SpellID).MapID = row.Item("mapId")
            TeleportCoords(SpellID).PosX = row.Item("position_x")
            TeleportCoords(SpellID).PosY = row.Item("position_y")
            TeleportCoords(SpellID).PosZ = row.Item("position_z")
        Next

        Log.WriteLine(LogType.INFORMATION, "World: {0} Teleport Coords Loaded.", MySQLQuery.Rows.Count)
    End Sub

    Public Class TTeleportCoords
        Public Name As String
        Public MapID As UInteger
        Public PosX As Single
        Public PosY As Single
        Public PosZ As Single
    End Class
#End Region

#Region "MonterSayCombat"
    Public Sub InitializeMonsterSayCombat()
        ' Load the MonsterSayCombat Hashtable.
        Dim Entry As Integer = 0
        Dim EventNo As Integer = 0
        Dim Chance As Single = 0.0F
        Dim Language As Integer = 0
        Dim Type As Integer = 0
        Dim MonsterName As String = ""
        Dim Text0 As String = ""
        Dim Text1 As String = ""
        Dim Text2 As String = ""
        Dim Text3 As String = ""
        Dim Text4 As String = ""
        Dim Count As Integer = 0

        Dim MySQLQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM npc_monstersay"), MySQLQuery)
        For Each MonsterRow As DataRow In MySQLQuery.Rows
            Count = Count + 1
            Entry = MonsterRow.Item("entry")
            EventNo = MonsterRow.Item("event")
            Chance = MonsterRow.Item("chance")
            Language = MonsterRow.Item("language")
            Type = MonsterRow.Item("type")
            If Not MonsterRow.Item("monstername") Is DBNull.Value Then
                MonsterName = MonsterRow.Item("monstername")
            Else
                MonsterName = ""
            End If

            If Not MonsterRow.Item("text0") Is DBNull.Value Then
                Text0 = MonsterRow.Item("text0")
            Else
                Text0 = ""
            End If

            If Not MonsterRow.Item("text1") Is DBNull.Value Then
                Text1 = MonsterRow.Item("text1")
            Else
                Text1 = ""
            End If

            If Not MonsterRow.Item("text2") Is DBNull.Value Then
                Text2 = MonsterRow.Item("text2")
            Else
                Text2 = ""
            End If

            If Not MonsterRow.Item("text3") Is DBNull.Value Then
                Text3 = MonsterRow("text3")
            Else
                Text3 = ""
            End If

            If Not MonsterRow.Item("text4") Is DBNull.Value Then
                Text4 = MonsterRow("text4")
            Else
                Text4 = ""
            End If

            If EventNo = MonsterSayEvents.MONSTER_SAY_EVENT_COMBAT Then
                MonsterSayCombat(Entry) = New TMonsterSayCombat(Entry, EventNo, Chance, Language, Type, MonsterName, Text0, Text1, Text2, Text3, Text4)
            End If

        Next

        Log.WriteLine(LogType.INFORMATION, "World: {0} Monster Say(s) Loaded.", Count)

    End Sub
#End Region

#Region "Creatures"
    Public CreatureGossip As New Dictionary(Of ULong, Integer)

    Public CreaturesFamily As New Dictionary(Of Integer, CreatureFamilyInfo)
    Public Class CreatureFamilyInfo
        Public ID As Integer
        Public Unknown1 As Integer
        Public Unknown2 As Integer
        Public PetFoodID As Integer
        Public Name As String
    End Class

    Public CreatureMovement As New Dictionary(Of Integer, Dictionary(Of Integer, CreatureMovePoint))
    Public Class CreatureMovePoint
        Public x As Single
        Public y As Single
        Public z As Single
        Public waittime As Integer
        Public moveflag As Integer
        Public action As Integer
        Public actionchance As Integer

        Public Sub New(ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Wait As Integer, ByVal MoveFlag As Integer, ByVal Action As Integer, ByVal ActionChance As Integer)
            Me.x = PosX
            Me.y = PosY
            Me.z = PosZ
            Me.waittime = Wait
            Me.moveflag = MoveFlag
            Me.action = Action
            Me.actionchance = ActionChance
        End Sub
    End Class

    Public CreatureEquip As New Dictionary(Of Integer, CreatureEquipInfo)
    Public Class CreatureEquipInfo
        Public EquipModel(2) As Integer
        Public EquipInfo(2) As UInteger
        Public EquipSlot(2) As Integer

        Public Sub New(ByVal EquipModel1 As Integer, ByVal EquipModel2 As Integer, ByVal EquipModel3 As Integer, ByVal EquipInfo1 As UInteger, ByVal EquipInfo2 As UInteger, ByVal EquipInfo3 As UInteger, ByVal EquipSlot1 As Integer, ByVal EquipSlot2 As Integer, ByVal EquipSlot3 As Integer)
            EquipModel(0) = EquipModel1
            EquipModel(1) = EquipModel2
            EquipModel(2) = EquipModel3
            EquipInfo(0) = EquipInfo1
            EquipInfo(1) = EquipInfo2
            EquipInfo(2) = EquipInfo3
            EquipSlot(0) = EquipSlot1
            EquipSlot(1) = EquipSlot2
            EquipSlot(2) = EquipSlot3
        End Sub
    End Class

    Public CreatureModel As New Dictionary(Of Integer, CreatureModelInfo)
    Public Class CreatureModelInfo
        Public BoundingRadius As Single
        Public CombatReach As Single
        Public Gender As Byte
        Public ModelIDOtherGender As Integer

        Public Sub New(ByVal BoundingRadius As Single, ByVal CombatReach As Single, ByVal Gender As Byte, ByVal ModelIDOtherGender As Integer)
            Me.BoundingRadius = BoundingRadius
            Me.CombatReach = CombatReach
            Me.Gender = Gender
            Me.ModelIDOtherGender = ModelIDOtherGender
        End Sub
    End Class
#End Region

#Region "Other"

    Public Sub InitializeInternalDatabase()

        InitializeLoadDBCs()

        InitializeSpellDB()

        RegisterChatCommands()

        Try
            Regenerator = New TRegenerator
            AIManager = New TAIManager
            SpellManager = New TSpellManager
            CharacterSaver = New TCharacterSaver
            WeatherChanger = New TWeatherChanger

            Log.WriteLine(LogType.INFORMATION, "World: Loading Maps and Spawns....")

            'DONE: Initializing Counters
            Dim MySQLQuery As New DataTable
            Try
                CharacterDatabase.Query(String.Format("SELECT MAX(item_guid) FROM characters_inventory;"), MySQLQuery)
                If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                    ItemGUIDCounter = MySQLQuery.Rows(0).Item(0) + GUID_ITEM
                Else
                    ItemGUIDCounter = 0 + GUID_ITEM
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "World: Failed loading characters_inventory....")
            End Try
            MySQLQuery = New DataTable
            Try
                WorldDatabase.Query(String.Format("SELECT MAX(spawn_id) FROM spawns_creatures;"), MySQLQuery)
                If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                    CreatureGUIDCounter = MySQLQuery.Rows(0).Item(0) + GUID_UNIT
                Else
                    CreatureGUIDCounter = 0 + GUID_UNIT
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "World: Failed loading spawns_creatures....")
            End Try

            MySQLQuery = New DataTable
            Try
                WorldDatabase.Query(String.Format("SELECT MAX(spawn_id) FROM spawns_gameobjects;"), MySQLQuery)
                If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                    GameObjectsGUIDCounter = MySQLQuery.Rows(0).Item(0) + GUID_GAMEOBJECT
                Else
                    GameObjectsGUIDCounter = 0 + GUID_GAMEOBJECT
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "World: Failed loading spawn_gameobjects....")
            End Try

            MySQLQuery = New DataTable
            Try
                CharacterDatabase.Query(String.Format("SELECT MAX(corpse_guid) FROM tmpspawnedcorpses"), MySQLQuery)
                If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                    CorpseGUIDCounter = MySQLQuery.Rows(0).Item(0) + GUID_CORPSE
                Else
                    CorpseGUIDCounter = 0 + GUID_CORPSE
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.FAILED, "World: Failed loading tmpspawnedcorpses....")
            End Try

        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Internal database initialization failed! [{0}]{1}{2}", e.Message, vbNewLine, e.ToString)
        End Try
    End Sub

    Public Sub InitializeLoadDBCs()
        InitializeMaps()
        InitializeXpTableFromDb()
        InitializeEmotes()
        InitializeEmotesText()
        InitializeAreaTable()
        InitializeFactions()
        InitializeFactionTemplates()
        InitializeCharRaces()
        InitializeCharClasses()
        InitializeSkillLines()
        InitializeSkillLineAbility()
        InitializeLocks()
        'AllGraveYards.InitializeGraveyards()
        InitializeTaxiNodes()
        InitializeTaxiPaths()
        InitializeTaxiPathNodes()
        InitializeDurabilityCosts()
        LoadSpellItemEnchantments()
        LoadItemSet()
        LoadItemDisplayInfoDBC()
        LoadItemRandomPropertiesDBC()
        LoadTalentDBC()
        LoadTalentTabDBC()
        LoadAuctionHouseDBC()
        LoadLootStores()
        LoadWeather()

        InitializeBattlemasters()
        InitializeBattlegrounds()
        InitializeTeleportCoords()
        InitializeMonsterSayCombat()
        LoadCreatureFamilyDBC()

        InitializeSpellRadius()
        InitializeSpellDuration()
        InitializeSpellCastTime()
        InitializeSpellRange()
        InitializeSpellFocusObject()
        InitializeSpells()
        InitializeSpellShapeShift()
        InitializeSpellChains()

        LoadCreatureGossip()
        LoadCreatureMovements()
        LoadCreatureEquipTable()
        LoadCreatureModelInfo()
        LoadQuestStartersAndFinishers()

        'LoadTransports()

    End Sub

#End Region
End Module
