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

Public Module GlobalEnum
    Public Enum ServerDb As Byte
        None = 0
        Realm = 1
        Character = 2
        World = 3
    End Enum

    Public Enum CorpseType
        CORPSE_BONES = 0
        CORPSE_RESURRECTABLE_PVE = 1
        CORPSE_RESURRECTABLE_PVP = 2
    End Enum

    Public Enum TrainerTypes
        TRAINER_TYPE_CLASS = 0
        TRAINER_TYPE_MOUNTS = 1
        TRAINER_TYPE_TRADESKILLS = 2
        TRAINER_TYPE_PETS = 3
    End Enum

    Public Enum CREATURE_FAMILY As Integer
        NONE = 0
        WOLF = 1
        CAT = 2
        SPIDER = 3
        BEAR = 4
        BOAR = 5
        CROCILISK = 6
        CARRION_BIRD = 7
        CRAB = 8
        GORILLA = 9
        RAPTOR = 11
        TALLSTRIDER = 12
        FELHUNTER = 15
        VOIDWALKER = 16
        SUCCUBUS = 17
        DOOMGUARD = 19
        SCORPID = 20
        TURTLE = 21
        IMP = 23
        BAT = 24
        HYENA = 25
        OWL = 26
        WIND_SERPENT = 27
        REMOTE_CONTROL = 28
    End Enum

    Public Enum CREATURE_ELITE As Integer
        NORMAL = 0
        ELITE = 1
        RAREELITE = 2
        WORLDBOSS = 3
        RARE = 4
    End Enum

    Public Enum MonsterSayEvents
        MONSTER_SAY_EVENT_COMBAT = 0
    End Enum

    Public Enum AIState
        AI_DO_NOTHING
        AI_DEAD
        AI_MOVING_TO_SPAWN
        AI_ATTACKING
        AI_MOVE_FOR_ATTACK
        AI_MOVING
        AI_WANDERING
        AI_RESPAWN
    End Enum

    Public Enum EnvironmentalDamage
        DAMAGE_EXHAUSTED = 0
        DAMAGE_DROWNING = 1
        DAMAGE_FALL = 2
        DAMAGE_LAVA = 3
        DAMAGE_SLIME = 4
        DAMAGE_FIRE = 5
    End Enum

    Public Enum BattlefieldType
        TYPE_BATTLEGROUND = 3
    End Enum

    Public Enum BattlefieldMapType As Byte
        BATTLEGROUND_AlteracValley = 1
        BATTLEGROUND_WarsongGulch = 2
        BATTLEGROUND_ArathiBasin = 3
    End Enum

    Public Enum BattlegroundStatus
        STATUS_CLEAR = 0
        STATUS_WAIT_QUEUE = 1
        STATUS_WAIT_JOIN = 2
        STATUS_IN_PROGRESS = 3
    End Enum

    'indexes of BattlemasterList.dbc 
    'This did not exist in Vanilla, Revisit in future!
    Public Enum BattleGroundTypeId As Byte
        BATTLEGROUND_TYPE_NONE = 0
        BATTLEGROUND_AV = 1
        BATTLEGROUND_WS = 2
        BATTLEGROUND_AB = 3
    End Enum

    Public Enum ActivateTaxiReplies As Byte
        ERR_TAXIOK = 0
        ERR_TAXIUNSPECIFIEDSERVERERROR = 1
        ERR_TAXINOSUCHPATH = 2
        ERR_TAXINOTENOUGHMONEY = 3
        ERR_TAXITOOFARAWAY = 4
        ERR_TAXINOVENDORNEARBY = 5
        ERR_TAXINOTVISITED = 6
        ERR_TAXIPLAYERBUSY = 7
        ERR_TAXIPLAYERALREADYMOUNTED = 8
        ERR_TAXIPLAYERSHAPESHIFTED = 9
        ERR_TAXIPLAYERMOVING = 10
        ERR_TAXISAMENODE = 11
        ERR_TAXINOTSTANDING = 12
    End Enum

    Enum InventoryChangeFailure As Byte
        EQUIP_ERR_OK = 0
        EQUIP_ERR_YOU_MUST_REACH_LEVEL_N = 1
        EQUIP_ERR_SKILL_ISNT_HIGH_ENOUGH = 2
        EQUIP_ERR_ITEM_DOESNT_GO_TO_SLOT = 3
        EQUIP_ERR_BAG_FULL = 4
        EQUIP_ERR_NONEMPTY_BAG_OVER_OTHER_BAG = 5
        EQUIP_ERR_CANT_TRADE_EQUIP_BAGS = 6
        EQUIP_ERR_ONLY_AMMO_CAN_GO_HERE = 7
        EQUIP_ERR_NO_REQUIRED_PROFICIENCY = 8
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE = 9
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM = 10
        EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM2 = 11
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE2 = 12
        EQUIP_ERR_CANT_EQUIP_WITH_TWOHANDED = 13
        EQUIP_ERR_CANT_DUAL_WIELD = 14
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG = 15
        EQUIP_ERR_ITEM_DOESNT_GO_INTO_BAG2 = 16
        EQUIP_ERR_CANT_CARRY_MORE_OF_THIS = 17
        EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE3 = 18
        EQUIP_ERR_ITEM_CANT_STACK = 19
        EQUIP_ERR_ITEM_CANT_BE_EQUIPPED = 20
        EQUIP_ERR_ITEMS_CANT_BE_SWAPPED = 21
        EQUIP_ERR_SLOT_IS_EMPTY = 22
        EQUIP_ERR_ITEM_NOT_FOUND = 23
        EQUIP_ERR_CANT_DROP_SOULBOUND = 24
        EQUIP_ERR_OUT_OF_RANGE = 25
        EQUIP_ERR_TRIED_TO_SPLIT_MORE_THAN_COUNT = 26
        EQUIP_ERR_COULDNT_SPLIT_ITEMS = 27
        EQUIP_ERR_MISSING_REAGENT = 28
        EQUIP_ERR_NOT_ENOUGH_MONEY = 29
        EQUIP_ERR_NOT_A_BAG = 30
        EQUIP_ERR_CAN_ONLY_DO_WITH_EMPTY_BAGS = 31
        EQUIP_ERR_DONT_OWN_THAT_ITEM = 32
        EQUIP_ERR_CAN_EQUIP_ONLY1_QUIVER = 33
        EQUIP_ERR_MUST_PURCHASE_THAT_BAG_SLOT = 34
        EQUIP_ERR_TOO_FAR_AWAY_FROM_BANK = 35
        EQUIP_ERR_ITEM_LOCKED = 36
        EQUIP_ERR_YOU_ARE_STUNNED = 37
        EQUIP_ERR_YOU_ARE_DEAD = 38
        EQUIP_ERR_CANT_DO_RIGHT_NOW = 39
        EQUIP_ERR_BAG_FULL2 = 40
        EQUIP_ERR_CAN_EQUIP_ONLY1_QUIVER2 = 41
        EQUIP_ERR_CAN_EQUIP_ONLY1_AMMOPOUCH = 42
        EQUIP_ERR_STACKABLE_CANT_BE_WRAPPED = 43
        EQUIP_ERR_EQUIPPED_CANT_BE_WRAPPED = 44
        EQUIP_ERR_WRAPPED_CANT_BE_WRAPPED = 45
        EQUIP_ERR_BOUND_CANT_BE_WRAPPED = 46
        EQUIP_ERR_UNIQUE_CANT_BE_WRAPPED = 47
        EQUIP_ERR_BAGS_CANT_BE_WRAPPED = 48
        EQUIP_ERR_ALREADY_LOOTED = 49
        EQUIP_ERR_INVENTORY_FULL = 50
        EQUIP_ERR_BANK_FULL = 51
        EQUIP_ERR_ITEM_IS_CURRENTLY_SOLD_OUT = 52
        EQUIP_ERR_BAG_FULL3 = 53
        EQUIP_ERR_ITEM_NOT_FOUND2 = 54
        EQUIP_ERR_ITEM_CANT_STACK2 = 55
        EQUIP_ERR_BAG_FULL4 = 56
        EQUIP_ERR_ITEM_SOLD_OUT = 57
        EQUIP_ERR_OBJECT_IS_BUSY = 58
        EQUIP_ERR_NONE = 59
        EQUIP_ERR_CANT_DO_IN_COMBAT = 60
        EQUIP_CANT_DO_WHILE_DISARMED = 61
        EQUIP_ERR_BAG_FULL6 = 62
        EQUIP_ITEM_RANK_NOT_ENOUGH = 63
        EQUIP_ITEM_REPUTATION_NOT_ENOUGH = 64
        EQUIP_MORE_THAN1_SPECIAL_BAG = 65
    End Enum

    Enum BuyFailure As Byte
        BUY_ERR_CANT_FIND_ITEM = 0
        BUY_ERR_ITEM_ALREADY_SOLD = 1
        BUY_ERR_NOT_ENOUGHT_MONEY = 2
        BUY_ERR_SELLER_DONT_LIKE_YOU = 4
        BUY_ERR_DISTANCE_TOO_FAR = 5
        BUY_ERR_CANT_CARRY_MORE = 8
        BUY_ERR_LEVEL_REQUIRE = 11
        BUY_ERR_REPUTATION_REQUIRE = 12
    End Enum

    Public Enum SHEATHE_TYPE As Byte
        SHEATHETYPE_NONE = 0
        SHEATHETYPE_MAINHAND = 1
        SHEATHETYPE_OFFHAND = 2
        SHEATHETYPE_LARGEWEAPONLEFT = 3
        SHEATHETYPE_LARGEWEAPONRIGHT = 4
        SHEATHETYPE_HIPWEAPONLEFT = 5
        SHEATHETYPE_HIPWEAPONRIGHT = 6
        SHEATHETYPE_SHIELD = 7
    End Enum

    Public Enum SHEATHE_SLOT As Byte
        SHEATHE_NONE = 0
        SHEATHE_WEAPON = 1
        SHEATHE_RANGED = 2
    End Enum

    Public Enum INVENTORY_TYPES As Byte
        INVTYPE_NON_EQUIP = &H0
        INVTYPE_HEAD = &H1
        INVTYPE_NECK = &H2
        INVTYPE_SHOULDERS = &H3
        INVTYPE_BODY = &H4           ' cloth robes only
        INVTYPE_CHEST = &H5
        INVTYPE_WAIST = &H6
        INVTYPE_LEGS = &H7
        INVTYPE_FEET = &H8
        INVTYPE_WRISTS = &H9
        INVTYPE_HANDS = &HA
        INVTYPE_FINGER = &HB
        INVTYPE_TRINKET = &HC
        INVTYPE_WEAPON = &HD
        INVTYPE_SHIELD = &HE
        INVTYPE_RANGED = &HF
        INVTYPE_CLOAK = &H10
        INVTYPE_TWOHAND_WEAPON = &H11
        INVTYPE_BAG = &H12
        INVTYPE_TABARD = &H13
        INVTYPE_ROBE = &H14
        INVTYPE_WEAPONMAINHAND = &H15
        INVTYPE_WEAPONOFFHAND = &H16
        INVTYPE_HOLDABLE = &H17
        INVTYPE_AMMO = &H18
        INVTYPE_THROWN = &H19
        INVTYPE_RANGEDRIGHT = &H1A
        INVTYPE_SLOT_ITEM = &H1B
        INVTYPE_RELIC = &H1C
        NUM_INVENTORY_TYPES = &H1D
    End Enum

    Public Enum EnchantSlots As Byte
        ENCHANTMENT_PERM = 0
        ENCHANTMENT_TEMP = 1
        ENCHANTMENT_BONUS = 2
        MAX_INSPECT = 3
        ENCHANTMENT_PROP_SLOT_1 = 3 'used with RandomSuffix
        ENCHANTMENT_PROP_SLOT_2 = 4 'used with RandomSuffix
        ENCHANTMENT_PROP_SLOT_3 = 5 'used with RandomSuffix and RandomProperty
        ENCHANTMENT_PROP_SLOT_4 = 6 'used with RandomProperty
        ENCHANTMENT_PROP_SLOT_5 = 7 'used with RandomProperty
        MAX_ENCHANTS = 8
    End Enum

    Public Enum AccountState As Byte
        'RealmServ Error Codes
        LOGIN_OK = &H0
        LOGIN_FAILED = &H1          'Unable to connect
        LOGIN_BANNED = &H3          'This World of Warcraft account has been closed and is no longer in service -- Please check the registered email address of this account for further information.
        LOGIN_UNKNOWN_ACCOUNT = &H4 'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_BAD_PASS = &H5        'The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account, see www.worldofwarcraft.com for more information.
        LOGIN_ALREADYONLINE = &H6   'This account is already logged into World of Warcraft.  Please check the spelling and try again.
        LOGIN_NOTIME = &H7          'You have used up your prepaid time for this account. Please purchase more to continue playing.
        LOGIN_DBBUSY = &H8          'Could not log in to World of Warcraft at this time.  Please try again later.
        LOGIN_BADVERSION = &H9      'Unable to validate game version.  This may be caused by file corruption or the interference of another program.  Please visit www.blizzard.com/support/wow/ for more information and possible solutions to this issue.
        LOGIN_DOWNLOADFILE = &HA
        LOGIN_SUSPENDED = &HC       'This World Of Warcraft account has been temporarily suspended. Please go to http://www.wow-europe.com/en/misc/banned.html for further information.
        LOGIN_PARENTALCONTROL = &HF 'Access to this account has been blocked by parental controls.  Your settings may be changed in your account preferences at http://www.worldofwarcraft.com.
    End Enum

    Public Enum ChangeSpeedType As Byte
        RUN = 1
        RUNBACK = 2
        SWIM = 3
        SWIMBACK = 4
        TURNRATE = 5
    End Enum

    Public Enum MirrorTimer As Byte
        FIRE = 5
        SLIME = 4
        LAVA = 3
        FALLING = 2
        DROWNING = 1
        FATIGUE = 0
    End Enum

    Public Enum TradeStatus As Byte
        TRADE_TARGET_UNAVIABLE = 0              '"[NAME] is busy"
        TRADE_STATUS_OK = 1                     'BEGIN TRADE
        TRADE_TRADE_WINDOW_OPEN = 2             'OPEN TRADE WINDOW
        TRADE_STATUS_CANCELED = 3               '"Trade canceled"
        TRADE_STATUS_COMPLETE = 4               'TRADE COMPLETE
        TRADE_TARGET_UNAVIABLE2 = 5             '"[NAME] is busy"
        TRADE_TARGET_MISSING = 6                'SOUND: I dont have a target
        TRADE_STATUS_UNACCEPT = 7               'BACK TRADE
        TRADE_COMPLETE = 8                      '"Trade Complete"
        TRADE_UNK2 = 9
        TRADE_TARGET_TOO_FAR = 10               '"Trade target is too far away"
        TRADE_TARGET_DIFF_FACTION = 11          '"Trade is not party of your alliance"
        TRADE_TRADE_WINDOW_CLOSE = 12           'CLOSE TRADE WINDOW
        TRADE_UNK3 = 13
        TRADE_TARGET_IGNORING = 14              '"[NAME] is ignoring you"
        TRADE_STUNNED = 15                      '"You are stunned"
        TRADE_TARGET_STUNNED = 16               '"Target is stunned"
        TRADE_DEAD = 17                         '"You cannot do that when you are dead"
        TRADE_TARGET_DEAD = 18                  '"You cannot trade with dead players"
        TRADE_LOGOUT = 19                       '"You are loging out"
        TRADE_TARGET_LOGOUT = 20                '"The player is loging out"
        TRADE_TRIAL_ACCOUNT = 21                '"Trial accounts cannot perform that action"
        TRADE_STATUS_ONLY_CONJURED = 22         '"You can only trade conjured items... (cross realm BG related)."
    End Enum

    Public Enum ResetFailedReason As UInteger
        INSTANCE_RESET_FAILED_ZONING = 0
        INSTANCE_RESET_FAILED_OFFLINE = 1
        INSTANCE_RESET_FAILED = 2
        INSTANCE_RESET_SUCCESS = 3
    End Enum

    Public Enum RaidInstanceMessage As UInteger
        RAID_INSTANCE_WARNING_HOURS = 1         ' WARNING! %s is scheduled to reset in %d hour(s).
        RAID_INSTANCE_WARNING_MIN = 2           ' WARNING! %s is scheduled to reset in %d minute(s)!
        RAID_INSTANCE_WARNING_MIN_SOON = 3      ' WARNING! %s is scheduled to reset in %d minute(s). Please exit the zone or you will be returned to your bind location!
        RAID_INSTANCE_WELCOME = 4               ' Welcome to %s. This raid instance is scheduled to reset in %s.
    End Enum

    Public Enum AreaTeam As Integer
        AREATEAM_NONE = 0
        AREATEAM_ALLY = 2
        AREATEAM_HORDE = 4
    End Enum

    Public Enum MenuIcon As Integer
        MENUICON_GOSSIP = &H0
        MENUICON_VENDOR = &H1
        MENUICON_TAXI = &H2
        MENUICON_TRAINER = &H3
        MENUICON_HEALER = &H4
        MENUICON_BINDER = &H5
        MENUICON_BANKER = &H6
        MENUICON_PETITION = &H7
        MENUICON_TABARD = &H8
        MENUICON_BATTLEMASTER = &H9
        MENUICON_AUCTIONER = &HA
        MENUICON_GOSSIP2 = &HB
        MENUICON_GOSSIP3 = &HC
    End Enum

    Public Enum ManaTypes As Integer
        TYPE_MANA = 0
        TYPE_RAGE = 1
        TYPE_FOCUS = 2
        TYPE_ENERGY = 3
        TYPE_HAPPINESS = 4
        TYPE_HEALTH = -2
    End Enum

    Public Enum ForceRestrictionFlags As Byte
        RESTRICT_RENAME = &H1
        RESTRICT_BILLING = &H2
        RESTRICT_TRANSFER = &H4
        RESTRICT_HIDECLOAK = &H8
        RESTRICT_HIDEHELM = &H10
    End Enum

    Public Enum MovementFlags As Integer
        MOVEMENTFLAG_NONE = &H0
        MOVEMENTFLAG_FORWARD = &H1
        MOVEMENTFLAG_BACKWARD = &H2
        MOVEMENTFLAG_STRAFE_LEFT = &H4
        MOVEMENTFLAG_STRAFE_RIGHT = &H8
        MOVEMENTFLAG_LEFT = &H10
        MOVEMENTFLAG_RIGHT = &H20
        MOVEMENTFLAG_PITCH_UP = &H40
        MOVEMENTFLAG_PITCH_DOWN = &H80

        MOVEMENTFLAG_WALK = &H100
        MOVEMENTFLAG_JUMPING = &H2000
        MOVEMENTFLAG_FALLING = &H4000
        MOVEMENTFLAG_SWIMMING = &H200000
        MOVEMENTFLAG_ONTRANSPORT = &H2000000
        MOVEMENTFLAG_SPLINE = &H4000000
    End Enum

    Public Enum TransportStates As Byte
        TRANSPORT_MOVE_TO_DOCK = 0
        TRANSPORT_DOCKED
        TRANSPORT_MOVE_NEXT_MAP
    End Enum

    Public Enum LootType As Byte
        LOOTTYPE_CORPSE = 1
        LOOTTYPE_PICKPOCKETING = 2
        LOOTTYPE_FISHING = 3
        LOOTTYPE_DISENCHANTING = 4
        LOOTTYPE_SKINNING = 6
    End Enum

    Private Enum LootState As UInteger
        NORMAL = 0
        PASSING = 1
    End Enum

    Public Enum LockKeyType As Byte
        LOCK_KEY_NONE = 0
        LOCK_KEY_ITEM = 1
        LOCK_KEY_SKILL = 2
    End Enum

    Public Enum LockType As Byte
        LOCKTYPE_PICKLOCK = 1
        LOCKTYPE_HERBALISM = 2
        LOCKTYPE_MINING = 3
        LOCKTYPE_DISARM_TRAP = 4
        LOCKTYPE_OPEN = 5
        LOCKTYPE_TREASURE = 6
        LOCKTYPE_CALCIFIED_ELVEN_GEMS = 7
        LOCKTYPE_CLOSE = 8
        LOCKTYPE_ARM_TRAP = 9
        LOCKTYPE_QUICK_OPEN = 10
        LOCKTYPE_QUICK_CLOSE = 11
        LOCKTYPE_OPEN_TINKERING = 12
        LOCKTYPE_OPEN_KNEELING = 13
        LOCKTYPE_OPEN_ATTACKING = 14
        LOCKTYPE_GAHZRIDIAN = 15
        LOCKTYPE_BLASTING = 16
        LOCKTYPE_SLOW_OPEN = 17
        LOCKTYPE_SLOW_CLOSE = 18
        LOCKTYPE_FISHING = 19
    End Enum

    Public Enum ConditionType                   ' value1       value2  for the Condition enumed
        CONDITION_NONE = 0                      ' 0            0
        CONDITION_AURA = 1                      ' spell_id     effindex
        CONDITION_ITEM = 2                      ' item_id      count
        CONDITION_ITEM_EQUIPPED = 3             ' item_id      0
        CONDITION_ZONEID = 4                    ' zone_id      0
        CONDITION_REPUTATION_RANK = 5           ' faction_id   min_rank
        CONDITION_TEAM = 6                      ' player_team  0,      (469 - Alliance 67 - Horde)
        CONDITION_SKILL = 7                     ' skill_id     skill_value
        CONDITION_QUESTREWARDED = 8             ' quest_id     0
        CONDITION_QUESTTAKEN = 9                ' quest_id     0,      for condition true while quest active.
        CONDITION_AD_COMMISSION_AURA = 10       ' 0            0,      for condition true while one from AD ñommission aura active
        CONDITION_NO_AURA = 11                  ' spell_id     effindex
        CONDITION_ACTIVE_EVENT = 12             ' event_id
        CONDITION_INSTANCE_DATA = 13            ' entry        data
    End Enum

    Public Enum TradeSkill As Integer
        TRADESKILL_ALCHEMY = 1
        TRADESKILL_BLACKSMITHING = 2
        TRADESKILL_COOKING = 3
        TRADESKILL_ENCHANTING = 4
        TRADESKILL_ENGINEERING = 5
        TRADESKILL_FIRSTAID = 6
        TRADESKILL_HERBALISM = 7
        TRADESKILL_LEATHERWORKING = 8
        TRADESKILL_POISONS = 9
        TRADESKILL_TAILORING = 10
        TRADESKILL_MINING = 11
        TRADESKILL_FISHING = 12
        TRADESKILL_SKINNING = 13
    End Enum

    Public Enum TradeSkillLevel As Integer
        TRADESKILL_LEVEL_NONE = 0
        TRADESKILL_LEVEL_APPRENTICE = 1
        TRADESKILL_LEVEL_JOURNEYMAN = 2
        TRADESKILL_LEVEL_EXPERT = 3
        TRADESKILL_LEVEL_ARTISAN = 4
        TRADESKILL_LEVEL_MASTER = 5
    End Enum

    Public Enum TargetType
        AllCharacters = -2
        AllMobiles = -3
        Enemy = -1
        [Friend] = 1
        GameObj = 4
        Neutral = 0
        Party = 2
        Pet = 3
    End Enum

    Public Enum TrackableCreatures
        All = 128
        Beast = 1
        Critter = 8
        Demon = 3
        Dragonkin = 2
        Elemental = 4
        Giant = 5
        Humanoid = 7
        Mechanical = 9
        Undead = 6
    End Enum

    Public Enum TrackableResources
        ElvenGems = 7
        GahzRidian = 15
        Herbs = 2
        Minerals = 3
        Treasure = 6
    End Enum

    Public Enum AuraTickFlags As Byte
        FLAG_PERIODIC_DAMAGE = &H2
        FLAG_PERIODIC_TRIGGER_SPELL = &H4
        FLAG_PERIODIC_HEAL = &H8
        FLAG_PERIODIC_LEECH = &H10
        FLAG_PERIODIC_ENERGIZE = &H20
    End Enum

    Public Enum AuraFlags
        AFLAG_NONE = &H0
        AFLAG_VISIBLE = &H1
        AFLAG_EFF_INDEX_1 = &H2
        AFLAG_EFF_INDEX_2 = &H4
        AFLAG_NOT_GUID = &H8
        AFLAG_CANCELLABLE = &H10
        AFLAG_HAS_DURATION = &H20
        AFLAG_UNK2 = &H40
        AFLAG_NEGATIVE = &H80
        AFLAG_POSITIVE = &H1F
        AFLAG_MASK = &HFF
    End Enum

    Public Enum ShapeshiftForm As Byte
        FORM_NORMAL = 0

        FORM_CAT = 1
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

    Public Enum SummonType
        SUMMON_TYPE_CRITTER = 41
        SUMMON_TYPE_GUARDIAN = 61
        SUMMON_TYPE_TOTEM_SLOT1 = 63
        SUMMON_TYPE_WILD = 64
        SUMMON_TYPE_POSESSED = 65
        SUMMON_TYPE_DEMON = 66
        SUMMON_TYPE_SUMMON = 67
        SUMMON_TYPE_TOTEM_SLOT2 = 81
        SUMMON_TYPE_TOTEM_SLOT3 = 82
        SUMMON_TYPE_TOTEM_SLOT4 = 83
        SUMMON_TYPE_TOTEM = 121
        SUMMON_TYPE_UNKNOWN3 = 181
        SUMMON_TYPE_UNKNOWN4 = 187
        SUMMON_TYPE_UNKNOWN1 = 247
        SUMMON_TYPE_UNKNOWN5 = 307
        SUMMON_TYPE_CRITTER2 = 407
        SUMMON_TYPE_UNKNOWN6 = 409
        SUMMON_TYPE_UNKNOWN2 = 427
        SUMMON_TYPE_POSESSED2 = 428
    End Enum

    Public Enum TempSummonType
        TEMPSUMMON_TIMED_OR_DEAD_DESPAWN = 1             'despawns after a specified time OR when the creature disappears
        TEMPSUMMON_TIMED_OR_CORPSE_DESPAWN = 2             'despawns after a specified time OR when the creature dies
        TEMPSUMMON_TIMED_DESPAWN = 3             'despawns after a specified time
        TEMPSUMMON_TIMED_DESPAWN_OUT_OF_COMBAT = 4             'despawns after a specified time after the creature is out of combat
        TEMPSUMMON_CORPSE_DESPAWN = 5             'despawns instantly after death
        TEMPSUMMON_CORPSE_TIMED_DESPAWN = 6             'despawns after a specified time after death
        TEMPSUMMON_DEAD_DESPAWN = 7             'despawns when the creature disappears
        TEMPSUMMON_MANUAL_DESPAWN = 8              'despawns when UnSummon() is called
    End Enum

    Public Enum LogType
        NETWORK                 'Network code debugging
        DEBUG                   'Packets processing
        INFORMATION             'User information
        USER                    'User actions
        SUCCESS                 'Normal operation
        WARNING                 'Warning
        FAILED                  'Processing Error
        CRITICAL                'Application Error
        DATABASE                'Database Error
    End Enum

    Public Enum WDBValueType
        WDB_STRING
        WDB_INTEGER
        WDB_BYTE
        WDB_FLOAT
    End Enum

    <CLSCompliant(False)>
    Public Enum MpqFileFlags As Long
        MPQ_Changed = 1                     '&H00000001
        MPQ_Protected = 2                   '&H00000002
        MPQ_CompressedPK = 256              '&H00000100
        MPQ_CompressedMulti = 512           '&H00000200
        MPQ_Compressed = 65280              '&H0000FF00
        MPQ_Encrypted = 65536               '&H00010000
        MPQ_FixSeed = 131072                '&H00020000
        MPQ_SingleUnit = 16777216           '&H01000000
        MPQ_Unknown_02000000 = 33554432     '&H02000000 - The file is only 1 byte long and its name is a hash
        MPQ_FileHasMetadata = 67108864      '&H04000000 - Indicates the file has associted metadata.
        MPQ_Exists = 2147483648             '&H80000000
    End Enum

    Public Enum DBCValueType
        DBC_STRING
        DBC_INTEGER
        DBC_FLOAT
    End Enum

    Public Enum SuggestionType As Integer
        TYPE_BUG_REPORT = 0
        TYPE_SUGGESTION = 1
    End Enum

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

    Public Enum ProcFlags
        PROC_FLAG_NONE = &H0                            ' None
        PROC_FLAG_HIT_MELEE = &H1                       ' On melee hit
        PROC_FLAG_STRUCK_MELEE = &H2                    ' On being struck melee
        PROC_FLAG_KILL_XP_GIVER = &H4                   ' On kill target giving XP or honor
        PROC_FLAG_SPECIAL_DROP = &H8                    '
        PROC_FLAG_DODGE = &H10                          ' On dodge melee attack
        PROC_FLAG_PARRY = &H20                          ' On parry melee attack
        PROC_FLAG_BLOCK = &H40                          ' On block attack
        PROC_FLAG_TOUCH = &H80                          ' On being touched (for bombs, probably?)
        PROC_FLAG_TARGET_LOW_HEALTH = &H100             ' On deal damage to enemy with 20% or less health
        PROC_FLAG_LOW_HEALTH = &H200                    ' On health dropped below 20%
        PROC_FLAG_STRUCK_RANGED = &H400                 ' On being struck ranged
        PROC_FLAG_HIT_SPECIAL = &H800                   ' (!)Removed, may be reassigned in future
        PROC_FLAG_CRIT_MELEE = &H1000                   ' On crit melee
        PROC_FLAG_STRUCK_CRIT_MELEE = &H2000            ' On being critically struck in melee
        PROC_FLAG_CAST_SPELL = &H4000                   ' On cast spell
        PROC_FLAG_TAKE_DAMAGE = &H8000                  ' On take damage
        PROC_FLAG_CRIT_SPELL = &H10000                  ' On crit spell
        PROC_FLAG_HIT_SPELL = &H20000                   ' On hit spell
        PROC_FLAG_STRUCK_CRIT_SPELL = &H40000           ' On being critically struck by a spell
        PROC_FLAG_HIT_RANGED = &H80000                  ' On getting ranged hit
        PROC_FLAG_STRUCK_SPELL = &H100000               ' On being struck by a spell
        PROC_FLAG_TRAP = &H200000                       ' On trap activation (?)
        PROC_FLAG_CRIT_RANGED = &H400000                ' On getting ranged crit
        PROC_FLAG_STRUCK_CRIT_RANGED = &H800000         ' On being critically struck by a ranged attack
        PROC_FLAG_RESIST_SPELL = &H1000000              ' On resist enemy spell
        PROC_FLAG_TARGET_RESISTS = &H2000000            ' On enemy resisted spell
        PROC_FLAG_TARGET_DODGE_OR_PARRY = &H4000000     ' On enemy dodges/parries
        PROC_FLAG_HEAL = &H8000000                      ' On heal
        PROC_FLAG_CRIT_HEAL = &H10000000                ' On critical healing effect
        PROC_FLAG_HEALED = &H20000000                   ' On healing
        PROC_FLAG_TARGET_BLOCK = &H40000000             ' On enemy blocks
        PROC_FLAG_MISS = &H80000000                     ' On miss melee attack
    End Enum

    Public Enum WeaponAttackType As Byte
        BASE_ATTACK = 0
        OFF_ATTACK = 1
        RANGED_ATTACK = 2
    End Enum

    Public Enum SwingTypes As Byte
        NOSWING = 0
        SINGLEHANDEDSWING = 1
        TWOHANDEDSWING = 2
    End Enum

    Public Enum AttackVictimState As Integer
        VICTIMSTATE_UNKNOWN1 = 0
        VICTIMSTATE_NORMAL = 1
        VICTIMSTATE_DODGE = 2
        VICTIMSTATE_PARRY = 3
        VICTIMSTATE_UNKNOWN2 = 4
        VICTIMSTATE_BLOCKS = 5
        VICTIMSTATE_EVADES = 6
        VICTIMSTATE_IS_IMMUNE = 7
        VICTIMSTATE_DEFLECTS = 8
    End Enum

    Public Enum AttackHitState As Integer
        HIT_UNARMED = HITINFO_NORMALSWING
        HIT_NORMAL = HITINFO_HITANIMATION
        HIT_NORMAL_OFFHAND = HITINFO_HITANIMATION + HITINFO_LEFTSWING
        HIT_MISS = HITINFO_MISS
        HIT_MISS_OFFHAND = HITINFO_MISS + HITINFO_LEFTSWING
        HIT_CRIT = HITINFO_CRITICALHIT
        HIT_CRIT_OFFHAND = HITINFO_CRITICALHIT + HITINFO_LEFTSWING
        HIT_RESIST = HITINFO_RESIST
        HIT_CRUSHING_BLOW = HITINFO_CRUSHING
        HIT_GLANCING_BLOW = HITINFO_GLANCING

        HITINFO_NORMALSWING = &H0
        HITINFO_UNK = &H1
        HITINFO_HITANIMATION = &H2
        HITINFO_LEFTSWING = &H4
        HITINFO_RANGED = &H8
        HITINFO_MISS = &H10
        HITINFO_ABSORB = &H20
        HITINFO_RESIST = &H40
        HITINFO_UNK2 = &H100
        HITINFO_CRITICALHIT = &H200
        HITINFO_BLOCK = &H800
        HITINFO_UNK3 = &H2000
        HITINFO_CRUSHING = &H8000
        HITINFO_GLANCING = &H10000
        HITINFO_NOACTION = &H10000
        HITINFO_SWINGNOHITSOUND = &H80000
    End Enum

    Public Enum InvisibilityLevel As Byte
        VISIBLE = 0
        STEALTH = 1
        INIVISIBILITY = 2
        DEAD = 3
        GM = 4
    End Enum

    Public Enum TotemType As Byte
        TOTEM_PASSIVE = 0
        TOTEM_ACTIVE = 1
        TOTEM_STATUE = 2
    End Enum

    Public Enum AuraAction As Byte
        AURA_ADD
        AURA_UPDATE
        AURA_REMOVE
        AURA_REMOVEBYDURATION
    End Enum

End Module
