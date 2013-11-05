

Public Module Constants
    Public Function GuidIsCreature(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_UNIT Then Return True
        Return False
    End Function

    Public Function GuidIsPet(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_PET Then Return True
        Return False
    End Function

    Public Function GuidIsItem(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_ITEM Then Return True
        Return False
    End Function

    Public Function GuidIsGameObject(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_GAMEOBJECT Then Return True
        Return False
    End Function

    Public Function GuidIsDnyamicObject(ByVal guid As ULong) As Boolean
        If GuidHigh2(GUID) = GUID_DYNAMICOBJECT Then Return True
        Return False
    End Function

    Public Function GuidIsTransport(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_TRANSPORT Then Return True
        Return False
    End Function

    Public Function GuidIsMoTransport(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_MO_TRANSPORT Then Return True
        Return False
    End Function

    Public Function GuidIsCorpse(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_CORPSE Then Return True
        Return False
    End Function

    Public Function GuidIsPlayer(ByVal guid As ULong) As Boolean
        If GuidHigh2(guid) = GUID_PLAYER Then Return True
        Return False
    End Function

    Public Function GuidHigh2(ByVal guid As ULong) As ULong
        Return (guid And GUID_MASK_HIGH)
    End Function

    Public Function GuidHigh(ByVal guid As ULong) As UInteger
        Return CUInt((guid And GUID_MASK_HIGH) >> 32UL)
    End Function

    Public Function GuidLow(ByVal guid As ULong) As UInteger
        Return (guid And GUID_MASK_LOW)
    End Function

    'TODO: Needs numic values, not packets
    Public Const GUID_ITEM As ULong = &H4000000000000000UL
    Public Const GUID_CONTAINER As ULong = &H4000000000000000UL
    Public Const GUID_PLAYER As ULong = &H0UL
    Public Const GUID_GAMEOBJECT As ULong = &HF110000000000000UL
    Public Const GUID_TRANSPORT As ULong = &HF120000000000000UL
    Public Const GUID_UNIT As ULong = &HF130000000000000UL
    Public Const GUID_PET As ULong = &HF140000000000000UL
    Public Const GUID_DYNAMICOBJECT As ULong = &HF100000000000000UL
    Public Const GUID_CORPSE As ULong = &HF101000000000000UL
    Public Const GUID_MO_TRANSPORT As ULong = &H1FC0000000000000UL
    Public Const GUID_MASK_LOW As UInteger = &HFFFFFFFFUI
    Public Const GUID_MASK_HIGH As ULong = &HFFFFFFFF00000000UL

    Public Const DEFAULT_DISTANCE_VISIBLE As Single = 155.8
    Public Const DEFAULT_DISTANCE_DETECTION As Single = 7

    Public Const DAY As Single = 86400.0F

    'TODO: Is this correct? The amount of time since last pvp action until you go out of combat again
    Public Const DEFAULT_PVP_COMBAT_TIME As Integer = 6000 ' 6 seconds

    Public Const DEFAULT_LOCK_TIMEOUT As Integer = 2000
    Public Const DEFAULT_INSTANCE_EXPIRE_TIME As Integer = 3600              '1 hour
    Public Const DEFAULT_BATTLEFIELD_EXPIRE_TIME As Integer = 3600 * 24      '24 hours

    Public SERVER_CONFIG_DISABLED_CLASSES() As Boolean = {False, False, False, False, False, False, False, False, False, True, False}
    Public SERVER_CONFIG_DISABLED_RACES() As Boolean = {False, False, False, False, False, False, False, False, True, False, False}

    Public Const UNIT_NORMAL_WALK_SPEED As Single = 2.5F
    Public Const UNIT_NORMAL_RUN_SPEED As Single = 7.0F
    Public Const UNIT_NORMAL_SWIM_SPEED As Single = 4.722222F
    Public Const UNIT_NORMAL_SWIM_BACK_SPEED As Single = 2.5F
    Public Const UNIT_NORMAL_WALK_BACK_SPEED As Single = 4.5F
    Public Const UNIT_NORMAL_TURN_RATE As Single = Math.PI
    Public Const UNIT_NORMAL_TAXI_SPEED As Single = 32.0F

    Public Const PLAYER_VISIBLE_ITEM_SIZE As Integer = 12
    Public Const PLAYER_SKILL_INFO_SIZE As Integer = 384 - 1
    Public Const PLAYER_EXPLORED_ZONES_SIZE As Integer = 64 - 1

    Public Const FIELD_MASK_SIZE_PLAYER As Integer = ((EPlayerFields.PLAYER_END + 32) \ 32) * 32
    Public Const FIELD_MASK_SIZE_UNIT As Integer = ((EUnitFields.UNIT_END + 32) \ 32) * 32
    Public Const FIELD_MASK_SIZE_GAMEOBJECT As Integer = ((EGameObjectFields.GAMEOBJECT_END + 32) \ 32) * 32
    Public Const FIELD_MASK_SIZE_DYNAMICOBJECT As Integer = ((EDynamicObjectFields.DYNAMICOBJECT_END + 32) \ 32) * 32
    Public Const FIELD_MASK_SIZE_ITEM As Integer = ((EContainerFields.CONTAINER_END + 32) \ 32) * 32
    Public Const FIELD_MASK_SIZE_CORPSE As Integer = ((ECorpseFields.CORPSE_END + 32) \ 32) * 32

    Public Enum SocialList As Byte
        MAX_FRIENDS_ON_LIST = 50
        MAX_IGNORES_ON_LIST = 25
    End Enum

    Public Enum FriendStatus As Byte
        FRIEND_STATUS_OFFLINE = 0
        FRIEND_STATUS_ONLINE = 1
        FRIEND_STATUS_AFK = 2
        FRIEND_STATUS_UNK3 = 3
        FRIEND_STATUS_DND = 4
    End Enum

    Public Enum FriendResult As Byte
        FRIEND_DB_ERROR = &H0
        FRIEND_LIST_FULL = &H1
        FRIEND_ONLINE = &H2
        FRIEND_OFFLINE = &H3
        FRIEND_NOT_FOUND = &H4
        FRIEND_REMOVED = &H5
        FRIEND_ADDED_ONLINE = &H6
        FRIEND_ADDED_OFFLINE = &H7
        FRIEND_ALREADY = &H8
        FRIEND_SELF = &H9
        FRIEND_ENEMY = &HA
        FRIEND_IGNORE_FULL = &HB
        FRIEND_IGNORE_SELF = &HC
        FRIEND_IGNORE_NOT_FOUND = &HD
        FRIEND_IGNORE_ALREADY = &HE
        FRIEND_IGNORE_ADDED = &HF
        FRIEND_IGNORE_REMOVED = &H10
    End Enum

    Public Enum SocialFlag As Byte
        SOCIAL_FLAG_FRIEND = &H1
        SOCIAL_FLAG_IGNORED = &H2
    End Enum

    Public Enum EquipmentSlots As Byte '19 slots total
        EQUIPMENT_SLOT_START = 0
        EQUIPMENT_SLOT_HEAD = 0
        EQUIPMENT_SLOT_NECK = 1
        EQUIPMENT_SLOT_SHOULDERS = 2
        EQUIPMENT_SLOT_BODY = 3
        EQUIPMENT_SLOT_CHEST = 4
        EQUIPMENT_SLOT_WAIST = 5
        EQUIPMENT_SLOT_LEGS = 6
        EQUIPMENT_SLOT_FEET = 7
        EQUIPMENT_SLOT_WRISTS = 8
        EQUIPMENT_SLOT_HANDS = 9
        EQUIPMENT_SLOT_FINGER1 = 10
        EQUIPMENT_SLOT_FINGER2 = 11
        EQUIPMENT_SLOT_TRINKET1 = 12
        EQUIPMENT_SLOT_TRINKET2 = 13
        EQUIPMENT_SLOT_BACK = 14
        EQUIPMENT_SLOT_MAINHAND = 15
        EQUIPMENT_SLOT_OFFHAND = 16
        EQUIPMENT_SLOT_RANGED = 17
        EQUIPMENT_SLOT_TABARD = 18
        EQUIPMENT_SLOT_END = 19
    End Enum

    Public Enum InventorySlots As Byte '4 Slots
        INVENTORY_SLOT_BAG_START = 19
        INVENTORY_SLOT_BAG_1 = 19
        INVENTORY_SLOT_BAG_2 = 20
        INVENTORY_SLOT_BAG_3 = 21
        INVENTORY_SLOT_BAG_4 = 22
        INVENTORY_SLOT_BAG_END = 23
    End Enum

    Public Enum InventoryPackSlots As Byte  '16 Slots
        INVENTORY_SLOT_ITEM_START = 23
        INVENTORY_SLOT_ITEM_1 = 23
        INVENTORY_SLOT_ITEM_2 = 24
        INVENTORY_SLOT_ITEM_3 = 25
        INVENTORY_SLOT_ITEM_4 = 26
        INVENTORY_SLOT_ITEM_5 = 27
        INVENTORY_SLOT_ITEM_6 = 28
        INVENTORY_SLOT_ITEM_7 = 29
        INVENTORY_SLOT_ITEM_8 = 30
        INVENTORY_SLOT_ITEM_9 = 31
        INVENTORY_SLOT_ITEM_10 = 32
        INVENTORY_SLOT_ITEM_11 = 33
        INVENTORY_SLOT_ITEM_12 = 34
        INVENTORY_SLOT_ITEM_13 = 35
        INVENTORY_SLOT_ITEM_14 = 36
        INVENTORY_SLOT_ITEM_15 = 37
        INVENTORY_SLOT_ITEM_16 = 38
        INVENTORY_SLOT_ITEM_END = 39
    End Enum

    Public Enum BankItemSlots As Byte  '29 Slots
        BANK_SLOT_ITEM_START = 39
        BANK_SLOT_ITEM_1 = 39
        BANK_SLOT_ITEM_2 = 40
        BANK_SLOT_ITEM_3 = 41
        BANK_SLOT_ITEM_4 = 42
        BANK_SLOT_ITEM_5 = 43
        BANK_SLOT_ITEM_6 = 44
        BANK_SLOT_ITEM_7 = 45
        BANK_SLOT_ITEM_8 = 46
        BANK_SLOT_ITEM_9 = 47
        BANK_SLOT_ITEM_10 = 48
        BANK_SLOT_ITEM_11 = 49
        BANK_SLOT_ITEM_12 = 50
        BANK_SLOT_ITEM_13 = 51
        BANK_SLOT_ITEM_14 = 52
        BANK_SLOT_ITEM_15 = 53
        BANK_SLOT_ITEM_16 = 54
        BANK_SLOT_ITEM_17 = 55
        BANK_SLOT_ITEM_18 = 56
        BANK_SLOT_ITEM_19 = 57
        BANK_SLOT_ITEM_20 = 58
        BANK_SLOT_ITEM_21 = 59
        BANK_SLOT_ITEM_22 = 60
        BANK_SLOT_ITEM_23 = 61
        BANK_SLOT_ITEM_24 = 62
        BANK_SLOT_ITEM_END = 63
    End Enum

    Public Enum BankBagSlots As Byte  '7 Slots
        BANK_SLOT_BAG_START = 63
        BANK_SLOT_BAG_1 = 63
        BANK_SLOT_BAG_2 = 64
        BANK_SLOT_BAG_3 = 65
        BANK_SLOT_BAG_4 = 66
        BANK_SLOT_BAG_5 = 67
        BANK_SLOT_BAG_6 = 68
        BANK_SLOT_BAG_END = 69
    End Enum

    Public Enum BuyBackSlots As Byte  '12 Slots
        BUYBACK_SLOT_START = 69
        BUYBACK_SLOT_1 = 69
        BUYBACK_SLOT_2 = 70
        BUYBACK_SLOT_3 = 71
        BUYBACK_SLOT_4 = 72
        BUYBACK_SLOT_5 = 73
        BUYBACK_SLOT_6 = 74
        BUYBACK_SLOT_7 = 75
        BUYBACK_SLOT_8 = 76
        BUYBACK_SLOT_9 = 77
        BUYBACK_SLOT_10 = 78
        BUYBACK_SLOT_11 = 79
        BUYBACK_SLOT_12 = 80
        BUYBACK_SLOT_END = 81
    End Enum

    Public Enum KeyRingSlots As Byte  '32 Slots?
        KEYRING_SLOT_START = 81
        KEYRING_SLOT_1 = 81
        KEYRING_SLOT_2 = 82
        KEYRING_SLOT_31 = 112
        KEYRING_SLOT_32 = 113
        KEYRING_SLOT_END = 113
    End Enum

    Public Enum QuestInfo As Byte
        QUEST_OBJECTIVES_COUNT = 4
        QUEST_REWARD_CHOICES_COUNT = 5
        QUEST_REWARDS_COUNT = 4
        QUEST_DEPLINK_COUNT = 10
        QUEST_SLOTS = 24
        QUEST_SHARING_DISTANCE = 10
    End Enum

    Public Enum TimeConstant
        MINUTE = 60
        HOUR = MINUTE * 60
        DAY = HOUR * 24
        WEEK = DAY * 7
        MONTH = DAY * 30
        YEAR = MONTH * 12
        IN_MILLISECONDS = 1000
    End Enum

    Public Enum AuthCMD As Byte
        CMD_AUTH_LOGON_CHALLENGE = &H0
        CMD_AUTH_LOGON_PROOF = &H1
        CMD_AUTH_RECONNECT_CHALLENGE = &H2
        CMD_AUTH_RECONNECT_PROOF = &H3
        CMD_AUTH_REALMLIST = &H10
        CMD_XFER_INITIATE = &H30
        CMD_XFER_DATA = &H31
        CMD_XFER_ACCEPT = &H32
        CMD_XFER_RESUME = &H33
        CMD_XFER_CANCEL = &H34
    End Enum

    Public Enum AuthSrv As Byte
        CMD_GRUNT_CONN_PONG = &H11
        CMD_GRUNT_PROVESESSION = &H21
    End Enum

    Public Enum AuthResult As Byte
        WOW_SUCCESS = &H0
        WOW_FAIL_BANNED = &H3
        WOW_FAIL_UNKNOWN_ACCOUNT = &H4
        WOW_FAIL_INCORRECT_PASSWORD = &H5
        WOW_FAIL_ALREADY_ONLINE = &H6
        WOW_FAIL_NO_TIME = &H7
        WOW_FAIL_DB_BUSY = &H8
        WOW_FAIL_VERSION_INVALID = &H9
        WOW_FAIL_VERSION_UPDATE = &HA
        WOW_FAIL_INVALID_SERVER = &HB
        WOW_FAIL_SUSPENDED = &HC
        WOW_FAIL_FAIL_NOACCESS = &HD
        WOW_SUCCESS_SURVEY = &HE
        WOW_FAIL_PARENTCONTROL = &HF
        WOW_FAIL_LOCKED_ENFORCED = &H10
        WOW_FAIL_TRIAL_ENDED = &H11
        WOW_FAIL_ANTI_INDULGENCE = &H13
        WOW_FAIL_EXPIRED = &H14
        WOW_FAIL_NO_GAME_ACCOUNT = &H15
        WOW_FAIL_CHARGEBACK = &H16
        WOW_FAIL_GAME_ACCOUNT_LOCKED = &H18
        WOW_FAIL_UNLOCKABLE_LOCK = &H19
        WOW_FAIL_CONVERSION_REQUIRED = &H20
        WOW_FAIL_DISCONNECTED = &HFF
    End Enum

    Public Enum LoginResponse As Byte
        LOGIN_OK = &HC
        LOGIN_VERSION_MISMATCH = &H14
        LOGIN_UNKNOWN_ACCOUNT = &H15
        LOGIN_WAIT_QUEUE = &H1B
    End Enum

    Public Enum CharResponse As Byte
        CHAR_LIST_FAILED = &H2C
        CHAR_CREATE_SUCCESS = &H2E
        CHAR_CREATE_ERROR = &H2F
        CHAR_CREATE_FAILED = &H30
        CHAR_CREATE_NAME_IN_USE = &H31
        CHAR_CREATE_DISABLED = &H32
        CHAR_CREATE_PVP_TEAMS_VIOLATION = &H33
        CHAR_CREATE_SERVER_LIMIT = &H34
        CHAR_CREATE_ACCOUNT_LIMIT = &H35
        CHAR_DELETE_SUCCESS = &H39
        CHAR_DELETE_FAILED = &H3A
        CHAR_LOGIN_NO_WORLD = &H3D
        CHAR_LOGIN_FAILED = &H40
        CHAR_NAME_INVALID_CHARACTER = &H46
    End Enum

    Public Enum ATLoginFlags As Byte
        AT_LOGIN_NONE = &H0
        AT_LOGIN_RENAME = &H1
        AT_LOGIN_RESET_SPELLS = &H2
        AT_LOGIN_RESET_TALENTS = &H4
        AT_LOGIN_FIRST = &H20
    End Enum

    Public Enum Genders As Byte
        GENDER_MALE = 0
        GENDER_FEMALE = 1
    End Enum

    Public Enum Classes As Byte
        CLASS_WARRIOR = 1
        CLASS_PALADIN = 2
        CLASS_HUNTER = 3
        CLASS_ROGUE = 4
        CLASS_PRIEST = 5
        CLASS_SHAMAN = 7
        CLASS_MAGE = 8
        CLASS_WARLOCK = 9
        CLASS_DRUID = 11
    End Enum

    Public Enum Races As Byte
        RACE_HUMAN = 1
        RACE_ORC = 2
        RACE_DWARF = 3
        RACE_NIGHT_ELF = 4
        RACE_UNDEAD = 5
        RACE_TAUREN = 6
        RACE_GNOME = 7
        RACE_TROLL = 8
    End Enum

    <Flags()> _
    Public Enum PlayerFlags As Integer
        PLAYER_FLAGS_GROUP_LEADER = &H1
        PLAYER_FLAGS_AFK = &H2
        PLAYER_FLAGS_DND = &H4
        PLAYER_FLAGS_GM = &H8                        'GM Prefix
        PLAYER_FLAGS_DEAD = &H10
        PLAYER_FLAGS_RESTING = &H20
        PLAYER_FLAGS_UNK7 = &H40                    'Admin Prefix?
        PLAYER_FLAGS_FFA_PVP = &H80
        PLAYER_FLAGS_CONTESTED_PVP = &H100
        PLAYER_FLAGS_IN_PVP = &H200
        PLAYER_FLAGS_HIDE_HELM = &H400
        PLAYER_FLAGS_HIDE_CLOAK = &H800
        PLAYER_FLAGS_PARTIAL_PLAY_TIME = &H1000
        PLAYER_FLAGS_IS_OUT_OF_BOUNDS = &H4000      'Out of Bounds
        PLAYER_FLAGS_UNK15 = &H8000                 'Dev Prefix?
        PLAYER_FLAGS_SANCTUARY = &H10000
        PLAYER_FLAGS_NO_PLAY_TIME = &H2000
        PLAYER_FLAGS_PVP_TIMER = &H40000
    End Enum

    Public Enum PlayerHonorRank As Byte
        RANK_NONE = 0
        RANK_PARIAH = 1
        RANK_OUTLAW = 2
        RANK_EXILED = 3
        RANK_DISHONORED = 4
        RANK_A_PRIVATE = 5
        RANK_H_SCOUT = 5
        RANK_A_CORPORAL = 6
        RANK_H_GRUNT = 6
        RANK_A_SERGEANT = 7
        RANK_H_SERGEANT = 7
        RANK_A_MASTER_SERGEANT = 78
        RANK_H_SENIOR_SERGEANT = 8
        RANK_A_SERGEANT_MAJOR = 9
        RANK_H_FIRST_SERGEANT = 9
        RANK_A_KNIGHT = 10
        RANK_H_STONE_GUARD = 10
        RANK_A_KNIGHT_LIEUTENANT = 11
        RANK_H_BLOOD_GUARD = 11
        RANK_A_KNIGHT_CAPTAIN = 12
        RANK_H_LEGIONNAIRE = 12
        RANK_A_KNIGHT_CHAMPION = 13
        RANK_H_CENTURION = 13
        RANK_A_LIEUTENANT = 14
        RANK_H_COMMANDER_CHAMPION = 14
        RANK_A_COMMANDER = 15
        RANK_H_LIEUTENANT_GENERAL = 15
        RANK_A_MARSHAL = 16
        RANK_H_GENERAL = 16
        RANK_A_FIELD_MARSHAL = 17
        RANK_H_WARLORD = 17
        RANK_A_GRAND_MARSHAL = 18
        RANK_H_HIGH_WARLORD = 18
    End Enum

    Public Enum DamageTypes As Byte
        DMG_PHYSICAL = 0
        DMG_HOLY = 1
        DMG_FIRE = 2
        DMG_NATURE = 3
        DMG_FROST = 4
        DMG_SHADOW = 5
        DMG_ARCANE = 6
    End Enum

    <Flags()> _
    Public Enum DamageMasks As Integer
        DMG_NORMAL = &H0
        DMG_PHYSICAL = &H1
        DMG_HOLY = &H2
        DMG_FIRE = &H4
        DMG_NATURE = &H8
        DMG_FROST = &H10
        DMG_SHADOW = &H20
        DMG_ARCANE = &H40
    End Enum

    Public Enum SpellLogTypes As Integer
        NON_MELEE = 0
    End Enum

    Public Enum StandStates As Byte
        STANDSTATE_STAND = 0
        STANDSTATE_SIT = 1
        STANDSTATE_SIT_CHAIR = 2
        STANDSTATE_SLEEP = 3
        STANDSTATE_SIT_LOW_CHAIR = 4
        STANDSTATE_SIT_MEDIUM_CHAIR = 5
        STANDSTATE_SIT_HIGH_CHAIR = 6
        STANDSTATE_DEAD = 7
        STANDSTATE_KNEEL = 8
    End Enum

    Public Enum HonorRank As Byte
        NoRank = 0
        Pariah = 1
        Outlaw = 2
        Exiled = 3
        Dishonored = 4
        Private_ = 5
        Corporal = 6
        Sergeant = 7
        MasterSergeant = 8
        SergeantMajor = 9
        Knight = 10
        KnightLieutenant = 11
        KnightCaptain = 12
        KnightChampion = 13
        LieutenantCommander = 14
        Commander = 15
        Marshal = 16
        FieldMarshal = 17
        GrandMarshal = 18
        Leader = 19
    End Enum

    Public Enum XPSTATE As Byte
        Normal = 2
        Rested = 1
    End Enum

    Public Enum ReputationRank As Byte
        Hated = 0
        Hostile = 1
        Unfriendly = 2
        Neutral = 3
        Friendly = 4
        Honored = 5
        Revered = 6
        Exalted = 7
    End Enum

    Public Enum ReputationPoints
        MIN = Integer.MinValue
        Hated = -42000
        Hostile = -6000
        Unfriendly = -3000
        Friendly = 3000
        Neutral = 0
        Honored = 9000
        Revered = 21000
        Exalted = 42000
        MAX = 43000
    End Enum

    Enum POI_ICON
        ICON_POI_0 = 0                                         ' Grey ?
        ICON_POI_1 = 1                                         ' Red ?
        ICON_POI_2 = 2                                         ' Blue ?
        ICON_POI_BWTOMB = 3                                    ' Blue and White Tomb Stone
        ICON_POI_HOUSE = 4                                     ' House
        ICON_POI_TOWER = 5                                     ' Tower
        ICON_POI_REDFLAG = 6                                   ' Red Flag with Yellow !
        ICON_POI_TOMB = 7                                      ' Tomb Stone
        ICON_POI_BWTOWER = 8                                   ' Blue and White Tower
        ICON_POI_REDTOWER = 9                                  ' Red Tower
        ICON_POI_BLUETOWER = 10                                ' Blue Tower
        ICON_POI_RWTOWER = 11                                  ' Red and White Tower
        ICON_POI_REDTOMB = 12                                  ' Red Tomb Stone
        ICON_POI_RWTOMB = 13                                   ' Red and White Tomb Stone
        ICON_POI_BLUETOMB = 14                                 ' Blue Tomb Stone
        ICON_POI_NOTHING = 15                                  ' NOTHING
        ICON_POI_16 = 16                                       ' Red ?
        ICON_POI_17 = 17                                       ' Grey ?
        ICON_POI_18 = 18                                       ' Blue ?
        ICON_POI_19 = 19                                       ' Red and White ?
        ICON_POI_20 = 20                                       ' Red ?
        ICON_POI_GREYLOGS = 21                                 ' Grey Wood Logs
        ICON_POI_BWLOGS = 22                                   ' Blue and White Wood Logs
        ICON_POI_BLUELOGS = 23                                 ' Blue Wood Logs
        ICON_POI_RWLOGS = 24                                   ' Red and White Wood Logs
        ICON_POI_REDLOGS = 25                                  ' Red Wood Logs
        ICON_POI_26 = 26                                       ' Grey ?
        ICON_POI_27 = 27                                       ' Blue and White ?
        ICON_POI_28 = 28                                       ' Blue ?
        ICON_POI_29 = 29                                       ' Red and White ?
        ICON_POI_30 = 30                                       ' Red ?
        ICON_POI_GREYHOUSE = 31                                ' Grey House
        ICON_POI_BWHOUSE = 32                                  ' Blue and White House
        ICON_POI_BLUEHOUSE = 33                                ' Blue House
        ICON_POI_RWHOUSE = 34                                  ' Red and White House
        ICON_POI_REDHOUSE = 35                                 ' Red House
        ICON_POI_GREYHORSE = 36                                ' Grey Horse
        ICON_POI_BWHORSE = 37                                  ' Blue and White Horse
        ICON_POI_BLUEHORSE = 38                                ' Blue Horse
        ICON_POI_RWHORSE = 39                                  ' Red and White Horse
        ICON_POI_REDHORSE = 40                                 ' Red Horse
    End Enum

    <Flags()> _
    Public Enum GroupType As Byte
        PARTY = 0
        RAID = 1
    End Enum

    <Flags()> _
    Public Enum GroupMemberOnlineStatus
        MEMBER_STATUS_OFFLINE = &H0
        MEMBER_STATUS_ONLINE = &H1
        MEMBER_STATUS_PVP = &H2
        MEMBER_STATUS_DEAD = &H4            ' dead (health=0)
        MEMBER_STATUS_GHOST = &H8           ' ghost (health=1)
        MEMBER_STATUS_PVP_FFA = &H10        ' pvp ffa
        MEMBER_STATUS_UNK3 = &H20           ' unknown
        MEMBER_STATUS_AFK = &H40            ' afk flag
        MEMBER_STATUS_DND = &H80            ' dnd flag
    End Enum

    Public Enum GroupDungeonDifficulty As Byte
        DIFFICULTY_NORMAL = 0
        DIFFICULTY_HEROIC = 1
    End Enum

    Public Enum GroupLootMethod As Byte
        LOOT_FREE_FOR_ALL = 0
        LOOT_ROUND_ROBIN = 1
        LOOT_MASTER = 2
        LOOT_GROUP = 3
        LOOT_NEED_BEFORE_GREED = 4
    End Enum

    Public Enum GroupLootThreshold As Byte
        Uncommon = 2
        Rare = 3
        Epic = 4
    End Enum

    Public Enum LANGUAGES As Integer
        LANG_GLOBAL = 0
        LANG_UNIVERSAL = 0
        LANG_ORCISH = 1
        LANG_DARNASSIAN = 2
        LANG_TAURAHE = 3
        LANG_DWARVISH = 6
        LANG_COMMON = 7
        LANG_DEMONIC = 8
        LANG_TITAN = 9
        LANG_DRACONIC = 11
        LANG_KALIMAG = 12
        LANG_GNOMISH = 13
        LANG_TROLL = 14
        LANG_GUTTERSPEAK = 33
    End Enum

    Public Enum ChatMsg As Integer
        CHAT_MSG_SAY = &H0
        CHAT_MSG_PARTY = &H1
        CHAT_MSG_RAID = &H2
        CHAT_MSG_GUILD = &H3
        CHAT_MSG_OFFICER = &H4
        CHAT_MSG_YELL = &H5
        CHAT_MSG_WHISPER = &H6
        CHAT_MSG_WHISPER_INFORM = &H7
        CHAT_MSG_EMOTE = &H8
        CHAT_MSG_TEXT_EMOTE = &H9
        CHAT_MSG_SYSTEM = &HA
        CHAT_MSG_MONSTER_SAY = &HB
        CHAT_MSG_MONSTER_YELL = &HC
        CHAT_MSG_MONSTER_EMOTE = &HD
        CHAT_MSG_CHANNEL = &HE
        CHAT_MSG_CHANNEL_JOIN = &HF
        CHAT_MSG_CHANNEL_LEAVE = &H10
        CHAT_MSG_CHANNEL_LIST = &H11
        CHAT_MSG_CHANNEL_NOTICE = &H12
        CHAT_MSG_CHANNEL_NOTICE_USER = &H13
        CHAT_MSG_AFK = &H14
        CHAT_MSG_DND = &H15
        CHAT_MSG_IGNORED = &H16
        CHAT_MSG_SKILL = &H17
        CHAT_MSG_LOOT = &H18
        CHAT_MSG_RAID_LEADER = &H57
        CHAT_MSG_RAID_WARNING = &H58
    End Enum

    <Flags()> _
    Public Enum ChatFlag As Byte
        FLAGS_NONE = 0
        FLAGS_AFK = 1
        FLAGS_DND = 2
        FLAGS_GM = 3
    End Enum

    Public Enum DynamicFlags   'Dynamic flags for units
        'Unit has blinking stars effect showing lootable
        UNIT_DYNFLAG_LOOTABLE = &H1
        'Shows marked unit as small red dot on radar
        UNIT_DYNFLAG_TRACK_UNIT = &H2
        'Gray mob title marks that mob is tagged by another player
        UNIT_DYNFLAG_OTHER_TAGGER = &H4
        'Blocks player character from moving
        UNIT_DYNFLAG_ROOTED = &H8
        'Shows infos like Damage and Health of the enemy
        UNIT_DYNFLAG_SPECIALINFO = &H10
        'Unit falls on the ground and shows like dead
        UNIT_DYNFLAG_DEAD = &H20
    End Enum

    <Flags()> _
    Public Enum UnitFlags   'Flags for units
        UNIT_FLAG_NONE = &H0
        UNIT_FLAG_UNK1 = &H1
        UNIT_FLAG_NOT_ATTACKABLE = &H2                                              'Unit is not attackable
        UNIT_FLAG_DISABLE_MOVE = &H4                                                'Unit is frozen, rooted or stunned
        UNIT_FLAG_ATTACKABLE = &H8                                                  'Unit becomes temporarily hostile, shows in red, allows attack
        UNIT_FLAG_RENAME = &H10
        UNIT_FLAG_RESTING = &H20
        UNIT_FLAG_UNK5 = &H40
        UNIT_FLAG_NOT_ATTACKABLE_1 = &H80                                           'Unit cannot be attacked by player, shows no attack cursor
        UNIT_FLAG_UNK6 = &H100
        UNIT_FLAG_UNK7 = &H200
        UNIT_FLAG_NON_PVP_PLAYER = UNIT_FLAG_ATTACKABLE + UNIT_FLAG_NOT_ATTACKABLE_1 'Unit cannot be attacked by player, shows in blue
        UNIT_FLAG_LOOTING = &H400
        UNIT_FLAG_PET_IN_COMBAT = &H800
        UNIT_FLAG_PVP = &H1000
        UNIT_FLAG_SILENCED = &H2000
        UNIT_FLAG_DEAD = &H4000
        UNIT_FLAG_UNK11 = &H8000
        UNIT_FLAG_ROOTED = &H10000
        UNIT_FLAG_PACIFIED = &H20000
        UNIT_FLAG_STUNTED = &H40000
        UNIT_FLAG_IN_COMBAT = &H80000
        UNIT_FLAG_TAXI_FLIGHT = &H100000
        UNIT_FLAG_DISARMED = &H200000
        UNIT_FLAG_CONFUSED = &H400000
        UNIT_FLAG_FLEEING = &H800000
        UNIT_FLAG_UNK21 = &H1000000
        UNIT_FLAG_NOT_SELECTABLE = &H2000000
        UNIT_FLAG_SKINNABLE = &H4000000
        UNIT_FLAG_MOUNT = &H8000000
        UNIT_FLAG_UNK25 = &H10000000
        UNIT_FLAG_UNK26 = &H20000000
        UNIT_FLAG_SKINNABLE_AND_DEAD = UNIT_FLAG_SKINNABLE + UNIT_FLAG_DEAD
        UNIT_FLAG_SPIRITHEALER = UNIT_FLAG_UNK21 + UNIT_FLAG_NOT_ATTACKABLE + UNIT_FLAG_DISABLE_MOVE + UNIT_FLAG_RESTING + UNIT_FLAG_UNK5
        UNIT_FLAG_SHEATHE = &H40000000
    End Enum

    <Flags()> _
    Public Enum NPCFlags
        UNIT_NPC_FLAG_NONE = &H0
        UNIT_NPC_FLAG_GOSSIP = &H1
        UNIT_NPC_FLAG_QUESTGIVER = &H2
        UNIT_NPC_FLAG_VENDOR = &H4
        UNIT_NPC_FLAG_TAXIVENDOR = &H8
        UNIT_NPC_FLAG_TRAINER = &H10
        UNIT_NPC_FLAG_SPIRITHEALER = &H20
        UNIT_NPC_FLAG_GUARD = &H40
        UNIT_NPC_FLAG_INNKEEPER = &H80
        UNIT_NPC_FLAG_BANKER = &H100
        UNIT_NPC_FLAG_PETITIONER = &H200
        UNIT_NPC_FLAG_TABARDVENDOR = &H400
        UNIT_NPC_FLAG_BATTLEFIELDPERSON = &H800
        UNIT_NPC_FLAG_AUCTIONEER = &H1000
        UNIT_NPC_FLAG_STABLE = &H2000
        UNIT_NPC_FLAG_ARMORER = &H4000
    End Enum

    Public Enum TrainerTypes
        TRAINER_TYPE_CLASS = 0
        TRAINER_TYPE_MOUNTS = 1
        TRAINER_TYPE_TRADESKILLS = 2
        TRAINER_TYPE_PETS = 3
    End Enum

    Public Enum UNIT_TYPE
        NONE = 0
        BEAST = 1
        DRAGONKIN = 2
        DEMON = 3
        ELEMENTAL = 4
        GIANT = 5
        UNDEAD = 6
        HUMANOID = 7
        CRITTER = 8
        MECHANICAL = 9
        NOT_SPECIFIED = 10
        TOTEM = 11
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

    Public Enum InvalidReason
        DontHaveReq = 0
        DontHaveReqItems = 19
        DontHaveReqMoney = 21
        NotAvailableRace = 6
        NotEnoughLevel = 1
        ReadyHaveThatQuest = 13
        ReadyHaveTimedQuest = 12
    End Enum

    Public Enum Attributes
        Agility = 3
        Health = 1
        Iq = 5
        Mana = 0
        Spirit = 6
        Stamina = 7
        Strenght = 4
    End Enum

    Public Enum Slots
        ' Fields
        Back = 14
        BackpackEnd = 39
        BackpackStart = 23
        Bag1 = 19
        Bag2 = 20
        Bag3 = 21
        Bag4 = 22
        BagsEnd = 261
        BagsStart = 81
        BankBagsEnd = 70
        BankBagsStart = 63
        BankEnd = 67
        BankStart = 39
        BuybackEnd = 81
        BuybackStart = 69
        Chest = 4
        Feet = 7
        FingerLeft = 10
        FingerRight = 11
        Hands = 9
        Head = 0
        ItemsEnd = 261
        Legs = 6
        MainHand = 15
        Neck = 1
        None = -1
        OffHand = 16
        Ranged = 17
        Shirt = 3
        Shoulders = 2
        Tabard = 18
        TrinketLeft = 12
        TrinketRight = 13
        Waist = 5
        Wrists = 8
    End Enum

    Public Enum EnviromentalDamage
        DAMAGE_EXHAUSTED = 0
        DAMAGE_DROWNING = 1
        DAMAGE_FALL = 2
        DAMAGE_LAVA = 3
        DAMAGE_SLIME = 4
        DAMAGE_FIRE = 5
    End Enum

    Public Enum MapTypes As Integer
        MAP_COMMON = 0
        MAP_INSTANCE = 1
        MAP_RAID = 2
        MAP_BATTLEGROUND = 3
    End Enum

    Public Enum BattlefieldType
        TYPE_BATTLEGROUND = 3
    End Enum

    Public Enum BattlefieldMapType As Byte
        BATTLEGROUND_AlteracValley = 1
        BATTLEGROUND_WarsongGulch = 2
        BATTLEGROUND_ArathiBasin = 3
    End Enum

    Public Enum GuildEvent As Byte
        PROMOTION = 0           'uint8(2), string(name), string(rankName)
        DEMOTION = 1            'uint8(2), string(name), string(rankName)
        MOTD = 2                'uint8(1), string(text)                                             'Guild message of the day: <text>
        JOINED = 3              'uint8(1), string(name)                                             '<name> has joined the guild.
        LEFT = 4                'uint8(1), string(name)                                             '<name> has left the guild.
        REMOVED = 5             '??
        LEADER_IS = 6           'uint8(1), string(name                                              '<name> is the leader of your guild.
        LEADER_CHANGED = 7      'uint8(2), string(oldLeaderName), string(newLeaderName) 
        DISBANDED = 8           'uint8(0)                                                           'Your guild has been disbanded.
        TABARDCHANGE = 9        '??
        SIGNED_ON = 12
        SIGNED_OFF = 13
    End Enum

    'Quest System Enums
    Public Enum QuestgiverStatusFlag As Integer
        DIALOG_STATUS_NONE = 0                  ' There aren't any quests available. - No Mark
        DIALOG_STATUS_UNAVAILABLE = 1           ' Quest available and your leve isn't enough. - Gray Quotation ! Mark
        DIALOG_STATUS_CHAT = 2                  ' Quest available it shows a talk baloon. - No Mark
        DIALOG_STATUS_INCOMPLETE = 3            ' Quest isn't finished yet. - Gray Question ? Mark
        DIALOG_STATUS_REWARD_REP = 4            ' Rewards rep? :P
        DIALOG_STATUS_AVAILABLE = 5             ' Quest available, and your level is enough. - Yellow Quotation ! Mark
        DIALOG_STATUS_REWARD = 6                ' Quest has been finished. - Yellow dot on the minimap
    End Enum

    <Flags()> _
    Public Enum QuestObjectiveFlag 'These flags are custom and are only used for MangosVB
        QUEST_OBJECTIVE_KILL = 1 'You have to kill creatures
        QUEST_OBJECTIVE_EXPLORE = 2 'You have to explore an area
        QUEST_OBJECTIVE_ESCORT = 4 'You have to escort someone
        QUEST_OBJECTIVE_EVENT = 8 'Something is required to happen (escort may be included in this one)
        QUEST_OBJECTIVE_CAST = 16 'You will have to cast a spell on a creature or a gameobject (spells on gameobjects are f.ex opening)
        QUEST_OBJECTIVE_ITEM = 32 'You have to recieve some items to deliver
        QUEST_OBJECTIVE_EMOTE = 64 'You do an emote to a creature
    End Enum

    <Flags()> _
    Public Enum QuestSpecialFlag As Integer
        QUEST_SPECIALFLAGS_NONE = 0
        QUEST_SPECIALFLAGS_DELIVER = 1
        QUEST_SPECIALFLAGS_EXPLORE = 2
        QUEST_SPECIALFLAGS_SPEAKTO = 4
        QUEST_SPECIALFLAGS_KILLORCAST = 8
        QUEST_SPECIALFLAGS_TIMED = 16
        '32 is unknown
        QUEST_SPECIALFLAGS_REPUTATION = 64
    End Enum

    Public Enum QuestInvalidError
        'SMSG_QUESTGIVER_QUEST_INVALID
        '   uint32 invalidReason

        INVALIDREASON_DONT_HAVE_REQ = 0                     'You don't meet the requirements for that quest
        INVALIDREASON_DONT_HAVE_LEVEL = 1                   'You are not high enough level for that quest.
        INVALIDREASON_DONT_HAVE_RACE = 6                    'That quest is not available to your race
        INVALIDREASON_COMPLETED_QUEST = 7                   'You have already completed this quest
        INVALIDREASON_HAVE_TIMED_QUEST = 12                 'You can only be on one timed quest at a time
        INVALIDREASON_HAVE_QUEST = 13                       'You are already on that quest
        INVALIDREASON_DONT_HAVE_EXP_ACCOUNT = 16            '??????
        INVALIDREASON_DONT_HAVE_REQ_ITEMS = 21  'Changed for 2.1.3  'You don't have the required items with you. Check storage.
        INVALIDREASON_DONT_HAVE_REQ_MONEY = 23              'You don't have enough money for that quest
        INVALIDREASON_REACHED_DAILY_LIMIT = 26              'You have completed xx daily quests today
        INVALIDREASON_UNKNOW27 = 27                         'You can not complete quests once you have reached tired time ???
    End Enum

    Public Enum QuestFailedReason
        'SMSG_QUESTGIVER_QUEST_FAILED
        '		uint32 questID
        '		uint32 failedReason

        FAILED_INVENTORY_FULL = 4       '0x04: '%s failed: Inventory is full.'
        FAILED_DUPE_ITEM = &H11         '0x11: '%s failed: Duplicate item found.'
        FAILED_INVENTORY_FULL2 = &H31   '0x31: '%s failed: Inventory is full.'
        FAILED_NOREASON = 0       '0x00: '%s failed.'
    End Enum

    Public Enum QuestPartyPushError As Byte
        QUEST_PARTY_MSG_SHARRING_QUEST = 0
        QUEST_PARTY_MSG_CANT_TAKE_QUEST = 1
        QUEST_PARTY_MSG_ACCEPT_QUEST = 2
        QUEST_PARTY_MSG_REFUSE_QUEST = 3
        QUEST_PARTY_MSG_TO_FAR = 4
        QUEST_PARTY_MSG_BUSY = 5
        QUEST_PARTY_MSG_LOG_FULL = 6
        QUEST_PARTY_MSG_HAVE_QUEST = 7
        QUEST_PARTY_MSG_FINISH_QUEST = 8
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

#Region "NPC Constants"
    Public Enum SELL_ERROR As Byte
        SELL_ERR_CANT_FIND_ITEM = 1
        SELL_ERR_CANT_SELL_ITEM = 2
        SELL_ERR_CANT_FIND_VENDOR = 3
    End Enum

    Public Enum BUY_ERROR As Byte
        'SMSG_BUY_FAILED error
        '0: cant find item
        '1: item already selled
        '2: not enought money
        '4: seller(dont Like u)
        '5: distance too far
        '8: cant carry more
        '11: level(require)
        '12: reputation(require)

        BUY_ERR_CANT_FIND_ITEM = 0
        BUY_ERR_ITEM_ALREADY_SOLD = 1
        BUY_ERR_NOT_ENOUGHT_MONEY = 2
        BUY_ERR_SELLER_DONT_LIKE_YOU = 4
        BUY_ERR_DISTANCE_TOO_FAR = 5
        BUY_ERR_CANT_CARRY_MORE = 8
        BUY_ERR_LEVEL_REQUIRE = 11
        BUY_ERR_REPUTATION_REQUIRE = 12
    End Enum

    Public Enum Gossip_Option
        GOSSIP_OPTION_NONE = 0                                 'UNIT_NPC_FLAG_NONE              = 0
        GOSSIP_OPTION_GOSSIP = 1                               'UNIT_NPC_FLAG_GOSSIP            = 1
        GOSSIP_OPTION_QUESTGIVER = 2                           'UNIT_NPC_FLAG_QUESTGIVER        = 2
        GOSSIP_OPTION_VENDOR = 3                               'UNIT_NPC_FLAG_VENDOR            = 4
        GOSSIP_OPTION_TAXIVENDOR = 4                           'UNIT_NPC_FLAG_FLIGHTMASTER        = 8
        GOSSIP_OPTION_TRAINER = 5                              'UNIT_NPC_FLAG_TRAINER           = 16
        GOSSIP_OPTION_SPIRITHEALER = 6                         'UNIT_NPC_FLAG_SPIRITHEALER      = 32
        GOSSIP_OPTION_GUARD = 7                                'UNIT_NPC_FLAG_GUARD		        = 64
        GOSSIP_OPTION_INNKEEPER = 8                            'UNIT_NPC_FLAG_INNKEEPER         = 128
        GOSSIP_OPTION_BANKER = 9                               'UNIT_NPC_FLAG_BANKER            = 256
        GOSSIP_OPTION_ARENACHARTER = 10                         'UNIT_NPC_FLAG_ARENACHARTER     = 262144
        GOSSIP_OPTION_TABARDVENDOR = 11                        'UNIT_NPC_FLAG_TABARDVENDOR      = 1024
        GOSSIP_OPTION_BATTLEFIELD = 12                         'UNIT_NPC_FLAG_BATTLEFIELDPERSON = 2048
        GOSSIP_OPTION_AUCTIONEER = 13                          'UNIT_NPC_FLAG_AUCTIONEER        = 4096
        GOSSIP_OPTION_STABLEPET = 14                           'UNIT_NPC_FLAG_STABLE            = 8192
        GOSSIP_OPTION_ARMORER = 15                             'UNIT_NPC_FLAG_REPAIR           = 16384
        GOSSIP_OPTION_TALENTWIPE = 16
    End Enum

    Public Enum Gossip_Guard
        GOSSIP_GUARD_BANK = 32
        GOSSIP_GUARD_RIDE = 33
        GOSSIP_GUARD_GUILD = 34
        GOSSIP_GUARD_INN = 35
        GOSSIP_GUARD_MAIL = 36
        GOSSIP_GUARD_AUCTION = 37
        GOSSIP_GUARD_WEAPON = 38
        GOSSIP_GUARD_STABLE = 39
        GOSSIP_GUARD_BATTLE = 40
        GOSSIP_GUARD_SPELLTRAINER = 41
        GOSSIP_GUARD_SKILLTRAINER = 42
    End Enum

    Public Enum Gossip_Guard_Spell
        GOSSIP_GUARD_SPELL_WARRIOR = 64
        GOSSIP_GUARD_SPELL_PALADIN = 65
        GOSSIP_GUARD_SPELL_HUNTER = 66
        GOSSIP_GUARD_SPELL_ROGUE = 67
        GOSSIP_GUARD_SPELL_PRIEST = 68
        GOSSIP_GUARD_SPELL_UNKNOWN1 = 69
        GOSSIP_GUARD_SPELL_SHAMAN = 70
        GOSSIP_GUARD_SPELL_MAGE = 71
        GOSSIP_GUARD_SPELL_WARLOCK = 72
        GOSSIP_GUARD_SPELL_UNKNOWN2 = 73
        GOSSIP_GUARD_SPELL_DRUID = 74
    End Enum

    Public Enum Gossip_Guard_Skill
        GOSSIP_GUARD_SKILL_ALCHEMY = 80
        GOSSIP_GUARD_SKILL_BLACKSMITH = 81
        GOSSIP_GUARD_SKILL_COOKING = 82
        GOSSIP_GUARD_SKILL_ENCHANT = 83
        GOSSIP_GUARD_SKILL_FIRSTAID = 84
        GOSSIP_GUARD_SKILL_FISHING = 85
        GOSSIP_GUARD_SKILL_HERBALISM = 86
        GOSSIP_GUARD_SKILL_LEATHER = 87
        GOSSIP_GUARD_SKILL_MINING = 88
        GOSSIP_GUARD_SKILL_SKINNING = 89
        GOSSIP_GUARD_SKILL_TAILORING = 90
        GOSSIP_GUARD_SKILL_ENGINERING = 91
    End Enum
#End Region

#Region "Items.Constants"
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

    Enum SpellFailure As Byte
        SELL_ERR_CANT_FIND_ITEM = 1
        SELL_ERR_CANT_SELL_ITEM = 2
        SELL_ERR_CANT_FIND_VENDOR = 3
    End Enum

    Public Enum ITEM_DAMAGE_TYPE As Byte
        NORMAL_DAMAGE = 0
        HOLY_DAMAGE = 1
        FIRE_DAMAGE = 2
        NATURE_DAMAGE = 3
        FROST_DAMAGE = 4
        SHADOW_DAMAGE = 5
        ARCANE_DAMAGE = 6
    End Enum

    Public Enum ITEM_QUALITY_NAMES As Byte
        ITEM_QUALITY_POOR_GREY = 0
        ITEM_QUALITY_NORMAL_WHITE = 1
        ITEM_QUALITY_UNCOMMON_GREEN = 2
        ITEM_QUALITY_RARE_BLUE = 3
        ITEM_QUALITY_EPIC_PURPLE = 4
        ITEM_QUALITY_LEGENDARY_ORANGE = 5
        ITEM_QUALITY_ARTIFACT_LIGHT_YELLOW = 6
        ITEM_QUALITY_HEIRLOOM = 7
    End Enum

    Public Enum ITEM_STAT_TYPE As Byte
        HEALTH = 1
        UNKNOWN = 2
        AGILITY = 3
        STRENGTH = 4
        INTELLECT = 5
        SPIRIT = 6
        STAMINA = 7
        DEFENCE = 12
        DODGE = 13
        PARRY = 14
        BLOCK = 15
        MELEEHITRATING = 16
        RANGEDHITRATING = 17
        SPELLHITRATING = 18
        MELEECRITRATING = 19
        RANGEDCRITRATING = 20
        SPELLCRITRATING = 21
        MELEEHITAVOIDANCE = 22
        RANGEDHITAVOIDANCE = 23
        SPELLHITAVOIDANCE = 24
        MELEECRITAVOIDANCE = 25
        RANGEDCRITAVOIDANCE = 26
        SPELLCRITAVOIDANCE = 26
        MELEEHASTERATING = 28
        RANGEDHASTERATING = 29
        SPELLHASTERATING = 30
        HITRATING = 31
        HITCRITRATING = 32
        HITAVOIDANCE = 33
        HITCRITAVOIDANCE = 34
        RESILIENCE = 35
        HITHASTERATING = 36
    End Enum

    Public Enum ITEM_SPELLTRIGGER_TYPE As Byte
        USE = 0
        ON_EQUIP = 1
        CHANCE_ON_HIT = 2
        SOULSTONE = 4
        NO_DELAY_USE = 5
        LEARN_SPELL = 6
    End Enum

    Public Enum ITEM_BONDING_TYPE As Byte
        NO_BIND = 0
        BIND_WHEN_PICKED_UP = 1
        BIND_WHEN_EQUIPED = 2
        BIND_WHEN_USED = 3
        BIND_UNK_QUESTITEM1 = 4
        BIND_UNK_QUESTITEM2 = 5
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

    'Got them from ItemSubClass.dbc
    Public Enum ITEM_CLASS As Byte
        ITEM_CLASS_CONSUMABLE = 0
        ITEM_CLASS_CONTAINER = 1
        ITEM_CLASS_WEAPON = 2
        ITEM_CLASS_JEWELRY = 3
        ITEM_CLASS_ARMOR = 4
        ITEM_CLASS_REAGENT = 5
        ITEM_CLASS_PROJECTILE = 6
        ITEM_CLASS_TRADE_GOODS = 7
        ITEM_CLASS_GENERIC = 8
        ITEM_CLASS_BOOK = 9
        ITEM_CLASS_MONEY = 10
        ITEM_CLASS_QUIVER = 11
        ITEM_CLASS_QUEST = 12
        ITEM_CLASS_KEY = 13
        ITEM_CLASS_PERMANENT = 14
        ITEM_CLASS_JUNK = 15
    End Enum

    Public Enum ITEM_SUBCLASS As Byte
        ' Consumable
        ITEM_SUBCLASS_CONSUMABLE = 0
        ITEM_SUBCLASS_FOOD = 1
        ITEM_SUBCLASS_LIQUID = 2
        ITEM_SUBCLASS_POTION = 3
        ITEM_SUBCLASS_SCROLL = 4
        ITEM_SUBCLASS_BANDAGE = 5
        ITEM_SUBCLASS_HEALTHSTONE = 6
        ITEM_SUBCLASS_COMBAT_EFFECT = 7

        ' Container
        ITEM_SUBCLASS_BAG = 0
        ITEM_SUBCLASS_SOUL_BAG = 1
        ITEM_SUBCLASS_HERB_BAG = 2
        ITEM_SUBCLASS_ENCHANTING_BAG = 3
        ITEM_SUBCLASS_ENGINEERING_BAG = 4
        ITEM_SUBCLASS_GEM_BAG = 5
        ITEM_SUBCLASS_MINNING_BAG = 6
        ITEM_SUBCLASS_LEATHERWORKING_BAG = 7

        ' Weapon
        ITEM_SUBCLASS_AXE = 0
        ITEM_SUBCLASS_TWOHAND_AXE = 1
        ITEM_SUBCLASS_BOW = 2
        ITEM_SUBCLASS_GUN = 3
        ITEM_SUBCLASS_MACE = 4
        ITEM_SUBCLASS_TWOHAND_MACE = 5
        ITEM_SUBCLASS_POLEARM = 6
        ITEM_SUBCLASS_SWORD = 7
        ITEM_SUBCLASS_TWOHAND_SWORD = 8
        ITEM_SUBCLASS_WEAPON_obsolete = 9
        ITEM_SUBCLASS_STAFF = 10
        ITEM_SUBCLASS_WEAPON_EXOTIC = 11
        ITEM_SUBCLASS_WEAPON_EXOTIC2 = 12
        ITEM_SUBCLASS_FIST_WEAPON = 13
        ITEM_SUBCLASS_MISC_WEAPON = 14
        ITEM_SUBCLASS_DAGGER = 15
        ITEM_SUBCLASS_THROWN = 16
        ITEM_SUBCLASS_SPEAR = 17
        ITEM_SUBCLASS_CROSSBOW = 18
        ITEM_SUBCLASS_WAND = 19
        ITEM_SUBCLASS_FISHING_POLE = 20

        ' Gem
        ITEM_SUBCLASS_RED = 0
        ITEM_SUBCLASS_BLUE = 1
        ITEM_SUBCLASS_YELLOW = 2
        ITEM_SUBCLASS_PURPLE = 3
        ITEM_SUBCLASS_GREEN = 4
        ITEM_SUBCLASS_ORANGE = 5
        ITEM_SUBCLASS_META = 6
        ITEM_SUBCLASS_SIMPLE = 7
        ITEM_SUBCLASS_PRISMATIC = 8

        ' Armor
        ITEM_SUBCLASS_MISC = 0
        ITEM_SUBCLASS_CLOTH = 1
        ITEM_SUBCLASS_LEATHER = 2
        ITEM_SUBCLASS_MAIL = 3
        ITEM_SUBCLASS_PLATE = 4
        ITEM_SUBCLASS_BUCKLER = 5
        ITEM_SUBCLASS_SHIELD = 6
        ITEM_SUBCLASS_LIBRAM = 7
        ITEM_SUBCLASS_IDOL = 8
        ITEM_SUBCLASS_TOTEM = 9

        ' Projectile
        ITEM_SUBCLASS_WAND_obslete = 0
        ITEM_SUBCLASS_BOLT_obslete = 1
        ITEM_SUBCLASS_ARROW = 2
        ITEM_SUBCLASS_BULLET = 3
        ITEM_SUBCLASS_THROWN_obslete = 4

        ' Trade goods
        ITEM_SUBCLASS_TRADE_GOODS = 0
        ITEM_SUBCLASS_PARTS = 1
        ITEM_SUBCLASS_EXPLOSIVES = 2
        ITEM_SUBCLASS_DEVICES = 3
        ITEM_SUBCLASS_GEMS = 4
        ITEM_SUBCLASS_CLOTHS = 5
        ITEM_SUBCLASS_LEATHERS = 6
        ITEM_SUBCLASS_METAL_AND_STONE = 7
        ITEM_SUBCLASS_MEAT = 8
        ITEM_SUBCLASS_HERB = 9
        ITEM_SUBCLASS_ELEMENTAL = 10
        ITEM_SUBCLASS_OTHERS = 11
        ITEM_SUBCLASS_ENCHANTANTS = 12
        ITEM_SUBCLASS_MATERIALS = 13

        ' Recipe
        ITEM_SUBCLASS_BOOK = 0
        ITEM_SUBCLASS_LEATHERWORKING = 1
        ITEM_SUBCLASS_TAILORING = 2
        ITEM_SUBCLASS_ENGINEERING = 3
        ITEM_SUBCLASS_BLACKSMITHING = 4
        ITEM_SUBCLASS_COOKING = 5
        ITEM_SUBCLASS_ALCHEMY = 6
        ITEM_SUBCLASS_FIRST_AID = 7
        ITEM_SUBCLASS_ENCHANTING = 8
        ITEM_SUBCLASS_FISNING = 9
        ITEM_SUBCLASS_JEWELCRAFTING = 10

        ' Quiver
        ITEM_SUBCLASS_QUIVER0_obslete = 0
        ITEM_SUBCLASS_QUIVER1_obslete = 1
        ITEM_SUBCLASS_QUIVER = 2
        ITEM_SUBCLASS_AMMO_POUCH = 3

        ' Keys
        ITEM_SUBCLASS_KEY = 0
        ITEM_SUBCLASS_LOCKPICK = 1

        ' Misc
        ITEM_SUBCLASS_JUNK = 0
        ITEM_SUBCLASS_REAGENT = 1
        ITEM_SUBCLASS_PET = 2
        ITEM_SUBCLASS_HOLIDAY = 3
        ITEM_SUBCLASS_OTHER = 4
        ITEM_SUBCLASS_MOUNT = 5
    End Enum

    <Flags()> _
    Public Enum ITEM_FLAGS As Integer
        ITEM_FLAGS_BINDED = &H1
        ITEM_FLAGS_CONJURED = &H2
        ITEM_FLAGS_OPENABLE = &H4
        ITEM_FLAGS_WRAPPED = &H8
        ITEM_FLAGS_WRAPPER = &H200 ' used or not used wrapper
        ITEM_FLAGS_PARTY_LOOT = &H800 ' determines if item is party loot or not
        ITEM_FLAGS_CHARTER = &H2000 ' arena/guild charter
        ITEM_FLAGS_THROWABLE = &H400000 ' not used in game for check trow possibility, only for item in game tooltip
        ITEM_FLAGS_SPECIALUSE = &H800000
    End Enum

    Public Enum ITEM_BAG As Integer
        NONE = 0
        ARROW = 1
        BULLET = 2
        SOUL_SHARD = 3
        HERB = 6
        ENCHANTING = 7
        ENGINEERING = 8
        KEYRING = 9
        JEWELCRAFTING = 10
        MINNING = 11
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
#End Region

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

#Region "Global.Constants"

    Public ReadOnly WorldServerStatus() As String = {"ONLINE/G", "ONLINE/R", "OFFLINE "}
    'Public ConsoleColor As New ConsoleColor
    '1.12.1 - 5875
    '1.12.2 - 6005

    Public Const RequiredVersion1 As Integer = 1
    Public Const RequiredVersion2 As Integer = 12
    Public Const RequiredVersion3 As Integer = 1
    Public Const RequiredBuildLow As Integer = 5875
    Public Const RequiredBuildHigh As Integer = 5875
    Public Const ConnectionSleepTime As Integer = 100
#End Region

    Public Enum GameObjectType As Byte
        GAMEOBJECT_TYPE_DOOR = 0
        GAMEOBJECT_TYPE_BUTTON = 1
        GAMEOBJECT_TYPE_QUESTGIVER = 2
        GAMEOBJECT_TYPE_CHEST = 3
        GAMEOBJECT_TYPE_BINDER = 4
        GAMEOBJECT_TYPE_GENERIC = 5
        GAMEOBJECT_TYPE_TRAP = 6
        GAMEOBJECT_TYPE_CHAIR = 7
        GAMEOBJECT_TYPE_SPELL_FOCUS = 8
        GAMEOBJECT_TYPE_TEXT = 9
        GAMEOBJECT_TYPE_GOOBER = 10
        GAMEOBJECT_TYPE_TRANSPORT = 11
        GAMEOBJECT_TYPE_AREADAMAGE = 12
        GAMEOBJECT_TYPE_CAMERA = 13
        GAMEOBJECT_TYPE_MAP_OBJECT = 14
        GAMEOBJECT_TYPE_MO_TRANSPORT = 15
        GAMEOBJECT_TYPE_DUEL_ARBITER = 16
        GAMEOBJECT_TYPE_FISHINGNODE = 17
        GAMEOBJECT_TYPE_RITUAL = 18
        GAMEOBJECT_TYPE_MAILBOX = 19
        GAMEOBJECT_TYPE_AUCTIONHOUSE = 20
        GAMEOBJECT_TYPE_GUARDPOST = 21
        GAMEOBJECT_TYPE_SPELLCASTER = 22
        GAMEOBJECT_TYPE_MEETINGSTONE = 23
        GAMEOBJECT_TYPE_FLAGSTAND = 24
        GAMEOBJECT_TYPE_FISHINGHOLE = 25
        GAMEOBJECT_TYPE_FLAGDROP = 26
        GAMEOBJECT_TYPE_MINI_GAME = 27
        GAMEOBJECT_TYPE_LOTTERY_KIOSK = 28
        GAMEOBJECT_TYPE_CAPTURE_POINT = 29
        GAMEOBJECT_TYPE_AURA_GENERATOR = 30
        GAMEOBJECT_TYPE_DUNGEON_DIFFICULTY = 31
        GAMEOBJECT_TYPE_BARBER_CHAIR = 32
        GAMEOBJECT_TYPE_DESTRUCTIBLE_BUILDING = 33
        GAMEOBJECT_TYPE_GUILD_BANK = 34
        GAMEOBJECT_TYPE_TRAP_DOOR = 35
    End Enum

    <Flags()> _
    Public Enum GameObjectFlags As Byte
        GO_FLAG_IN_USE = &H1                        'disables interaction while animated
        GO_FLAG_LOCKED = &H2                        'require key, spell, event, etc to be opened. Makes "Locked" appear in tooltip
        GO_FLAG_INTERACT_COND = &H4                 'cannot interact (condition to interact)
        GO_FLAG_TRANSPORT = &H8                     'any kind of transport? Object can transport (elevator, boat, car)
        GO_FLAG_UNK1 = &H10                         '
        GO_FLAG_NODESPAWN = &H20                    'never despawn, typically for doors, they just change state
        GO_FLAG_TRIGGERED = &H40                    'typically, summoned objects. Triggered by spell or other events
    End Enum

    Public Const GUILD_RANK_MAX As Integer = 9 'Max Ranks Per Guild
    Public Const GUILD_RANK_MIN As Integer = 5 'Min Ranks Per Guild

    'Default Guild Ranks
    'TODO: Set the ranks during guild creation
    Public Enum GuildDefaultRanks As Byte
        GR_GUILDMASTER = 0
        GR_OFFICER = 1
        GR_VETERAN = 2
        GR_MEMBER = 3
        GR_INITIATE = 4
    End Enum

    'Helping Subs
    Public Enum GuildCommand As Byte
        GUILD_CREATE_S = &H0
        GUILD_INVITE_S = &H1
        GUILD_QUIT_S = &H2
        GUILD_FOUNDER_S = &HC
    End Enum

    Public Enum GuildError As Byte
        GUILD_PLAYER_NO_MORE_IN_GUILD = &H0
        GUILD_INTERNAL = &H1
        GUILD_ALREADY_IN_GUILD = &H2
        ALREADY_IN_GUILD = &H3
        INVITED_TO_GUILD = &H4
        ALREADY_INVITED_TO_GUILD = &H5
        GUILD_NAME_INVALID = &H6
        GUILD_NAME_EXISTS = &H7
        GUILD_LEADER_LEAVE = &H8
        GUILD_PERMISSIONS = &H8
        GUILD_PLAYER_NOT_IN_GUILD = &H9
        GUILD_PLAYER_NOT_IN_GUILD_S = &HA
        GUILD_PLAYER_NOT_FOUND = &HB
        GUILD_NOT_ALLIED = &HC
    End Enum

    Public Enum PetitionSignError As Integer
        PETITIONSIGN_OK = 0                     ':Closes the window
        PETITIONSIGN_ALREADY_SIGNED = 1         'You have already signed that guild charter
        PETITIONSIGN_ALREADY_IN_GUILD = 2       'You are already in a guild
        PETITIONSIGN_CANT_SIGN_OWN = 3          'You can's sign own guild charter
        PETITIONSIGN_NOT_SERVER = 4             'That player is not from your server
    End Enum

    Public Enum PetitionTurnInError As Integer
        PETITIONTURNIN_OK = 0                   ':Closes the window
        PETITIONTURNIN_ALREADY_IN_GUILD = 2     'You are already in a guild
        PETITIONTURNIN_NEED_MORE_SIGNATURES = 4 'You need more signatures
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

    Public Enum Guards As Integer
        Stormwind_Guard = 1423
        Stormwind_City_Guard = 68
        Stormwind_City_Patroller = 1976
        Darnassus_Sentinel = 4262
        Undercity_Guardian = 5624
        Teldrassil_Sentinel = 3571
        Shield_of_Velen = 20674
        Orgrimmar_Grunt = 3296
        Bluffwatcher = 3084
        Brave_Wildrunner = 3222
        Brave_Cloudmane = 3224
        Brave_Darksky = 3220
        Brave_Leaping_Deer = 3219
        Brave_Dawn_Eagle = 3217
        Brave_Strongbash = 3215
        Brave_Swiftwind = 3218
        Brave_Rockhorn = 3221
        Brave_Rainchaser = 3223
        Brave_IronHorn = 3212
        Razor_Hill_Grunt = 5953
        Deathguard_Lundmark = 5725
        Deathguard_Terrence = 1738
        Deathguard_Burgess = 1652
        Deathguard_Cyrus = 1746
        Deathguard_Morris = 1745
        Deathguard_Lawrence = 1743
        Deathguard_Mort = 1744
        Deathguard_Dillinger = 1496
        Deathguard_Bartholomew = 1742
        Ironforge_Guard = 5595
        Ironforge_Mountaineer = 727
    End Enum

    'TODO: Need to fix this for all to have values
    Public Enum Gossips As Integer
        Thunderbluff = 0
        Darnassus
        DunMorogh
        Durotar
        ElwynnForest
        Ironforge
        Mulgore
        Orgrimmar
        Stormwind
        Teldrassil
        Tirisfall
        Undercity
    End Enum

    Public Const GOSSIP_TEXT_BANK As String = "The Bank"
    Public Const GOSSIP_TEXT_WINDRIDER As String = "Wind rider master"
    Public Const GOSSIP_TEXT_GRYPHON As String = "Gryphon Master"
    Public Const GOSSIP_TEXT_BATHANDLER As String = "Bat Handler"
    Public Const GOSSIP_TEXT_HIPPOGRYPH As String = "Hippogryph Master"
    Public Const GOSSIP_TEXT_FLIGHTMASTER As String = "Flight Master"
    Public Const GOSSIP_TEXT_AUCTIONHOUSE As String = "Auction House"
    Public Const GOSSIP_TEXT_GUILDMASTER As String = "Guild Master"
    Public Const GOSSIP_TEXT_INN As String = "The Inn"
    Public Const GOSSIP_TEXT_MAILBOX As String = "Mailbox"
    Public Const GOSSIP_TEXT_STABLEMASTER As String = "Stable Master"
    Public Const GOSSIP_TEXT_WEAPONMASTER As String = "Weapons Trainer"
    Public Const GOSSIP_TEXT_BATTLEMASTER As String = "Battlemaster"
    Public Const GOSSIP_TEXT_CLASSTRAINER As String = "Class Trainer"
    Public Const GOSSIP_TEXT_PROFTRAINER As String = "Profession Trainer"
    Public Const GOSSIP_TEXT_OFFICERS As String = "The officers` lounge"

    Public Const GOSSIP_TEXT_ALTERACVALLEY As String = "Alterac Valley"
    Public Const GOSSIP_TEXT_ARATHIBASIN As String = "Arathi Basin"
    Public Const GOSSIP_TEXT_WARSONGULCH As String = "Warsong Gulch"

    Public Const GOSSIP_TEXT_IRONFORGE_BANK As String = "Bank of Ironforge"
    Public Const GOSSIP_TEXT_STORMWIND_BANK As String = "Bank of Stormwind"
    Public Const GOSSIP_TEXT_DEEPRUNTRAM As String = "Deeprun Tram"
    Public Const GOSSIP_TEXT_ZEPPLINMASTER As String = "Zeppelin master"
    Public Const GOSSIP_TEXT_FERRY As String = "Rut'theran Ferry"

    Public Const GOSSIP_TEXT_DRUID As String = "Druid"
    Public Const GOSSIP_TEXT_HUNTER As String = "Hunter"
    Public Const GOSSIP_TEXT_PRIEST As String = "Priest"
    Public Const GOSSIP_TEXT_ROGUE As String = "Rogue"
    Public Const GOSSIP_TEXT_WARRIOR As String = "Warrior"
    Public Const GOSSIP_TEXT_PALADIN As String = "Paladin"
    Public Const GOSSIP_TEXT_SHAMAN As String = "Shaman"
    Public Const GOSSIP_TEXT_MAGE As String = "Mage"
    Public Const GOSSIP_TEXT_WARLOCK As String = "Warlock"

    Public Const GOSSIP_TEXT_ALCHEMY As String = "Alchemy"
    Public Const GOSSIP_TEXT_BLACKSMITHING As String = "Blacksmithing"
    Public Const GOSSIP_TEXT_COOKING As String = "Cooking"
    Public Const GOSSIP_TEXT_ENCHANTING As String = "Enchanting"
    Public Const GOSSIP_TEXT_ENGINEERING As String = "Engineering"
    Public Const GOSSIP_TEXT_FIRSTAID As String = "First Aid"
    Public Const GOSSIP_TEXT_HERBALISM As String = "Herbalism"
    Public Const GOSSIP_TEXT_LEATHERWORKING As String = "Leatherworking"
    Public Const GOSSIP_TEXT_POISONS As String = "Poisons"
    Public Const GOSSIP_TEXT_TAILORING As String = "Tailoring"
    Public Const GOSSIP_TEXT_MINING As String = "Mining"
    Public Const GOSSIP_TEXT_FISHING As String = "Fishing"
    Public Const GOSSIP_TEXT_SKINNING As String = "Skinning"

    Public Enum AreaTeam As Integer
        AREATEAM_NONE = 0
        AREATEAM_ALLY = 2
        AREATEAM_HORDE = 4
    End Enum

    Public Enum AreaFlag As Integer
        AREA_FLAG_SNOW = &H1                ' snow (only Dun Morogh, Naxxramas, Razorfen Downs and Winterspring)
        AREA_FLAG_UNK1 = &H2                ' unknown, (only Naxxramas and Razorfen Downs)
        AREA_FLAG_UNK2 = &H4                ' Only used on development map
        AREA_FLAG_SLAVE_CAPITAL = &H8       ' slave capital city flag?
        AREA_FLAG_UNK3 = &H10               ' unknown
        AREA_FLAG_SLAVE_CAPITAL2 = &H20     ' slave capital city flag?
        AREA_FLAG_UNK4 = &H40               ' many zones have this flag
        AREA_FLAG_ARENA = &H80              ' arena, both instanced and world arenas
        AREA_FLAG_CAPITAL = &H100           ' main capital city flag
        AREA_FLAG_CITY = &H200              ' only for one zone named "City" (where it located?)
        AREA_FLAG_SANCTUARY = &H800         ' sanctuary area (PvP disabled)
        AREA_FLAG_NEED_FLY = &H1000         ' only Netherwing Ledge, Socrethar's Seat, Tempest Keep, The Arcatraz, The Botanica, The Mechanar, Sorrow Wing Point, Dragonspine Ridge, Netherwing Mines, Dragonmaw Base Camp, Dragonmaw Skyway
        AREA_FLAG_UNUSED1 = &H2000          ' not used now (no area/zones with this flag set in 2.4.2)
        AREA_FLAG_PVP = &H8000              ' pvp objective area? (Death's Door also has this flag although it's no pvp object area)
        AREA_FLAG_ARENA_INSTANCE = &H10000  ' used by instanced arenas only
        AREA_FLAG_UNUSED2 = &H20000         ' not used now (no area/zones with this flag set in 2.4.2)
        AREA_FLAG_UNK5 = &H40000            ' just used for Amani Pass, Hatchet Hills
        AREA_FLAG_LOWLEVEL = &H100000       ' used for some starting areas with area_level <=15
    End Enum

    'VMAPS
    Public Const VMAP_MAGIC As String = "VMAP_2.0"
    Public Const VMAP_MAX_CAN_FALL_DISTANCE As Single = 10.0F
    Public Const VMAP_INVALID_HEIGHT As Single = -100000.0F 'for check
    Public Const VMAP_INVALID_HEIGHT_VALUE As Single = -200000.0F 'real assigned value in unknown height case

    'MAPS
    Public Const SIZE As Single = 533.3333F
    Public Const RESOLUTION_WATER As Integer = 128 - 1
    Public Const RESOLUTION_FLAGS As Integer = 16 - 1
    Public Const RESOLUTION_TERRAIN As Integer = 16 - 1

    Public Enum TransferAbortReason As Short
        TRANSFER_ABORT_MAX_PLAYERS = &H1                ' Transfer Aborted: instance is full
        TRANSFER_ABORT_NOT_FOUND = &H2                  ' Transfer Aborted: instance not found
        TRANSFER_ABORT_TOO_MANY_INSTANCES = &H3         ' You have entered too many instances recently.
        TRANSFER_ABORT_ZONE_IN_COMBAT = &H5             ' Unable to zone in while an encounter is in progress.
        TRANSFER_ABORT_INSUF_EXPAN_LVL1 = &H106         ' You must have TBC expansion installed to access this area.
        TRANSFER_ABORT_DIFFICULTY1 = &H7                ' Normal difficulty mode is not available for %s.
        TRANSFER_ABORT_DIFFICULTY2 = &H107              ' Heroic difficulty mode is not available for %s.
        TRANSFER_ABORT_DIFFICULTY3 = &H207              ' Epic difficulty mode is not available for %s.
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

    Public Const groundFlagsMask As Integer = &HFFFFFFFF And Not (MovementFlags.MOVEMENTFLAG_LEFT Or MovementFlags.MOVEMENTFLAG_RIGHT Or MovementFlags.MOVEMENTFLAG_BACKWARD Or MovementFlags.MOVEMENTFLAG_FORWARD Or MovementFlags.MOVEMENTFLAG_WALK)
    Public Const movementFlagsMask As Integer = MovementFlags.MOVEMENTFLAG_FORWARD Or MovementFlags.MOVEMENTFLAG_BACKWARD Or MovementFlags.MOVEMENTFLAG_STRAFE_LEFT Or _
    MovementFlags.MOVEMENTFLAG_STRAFE_RIGHT Or MovementFlags.MOVEMENTFLAG_PITCH_UP Or MovementFlags.MOVEMENTFLAG_PITCH_DOWN Or MovementFlags.MOVEMENTFLAG_JUMPING Or _
    MovementFlags.MOVEMENTFLAG_FALLING Or MovementFlags.MOVEMENTFLAG_SWIMMING Or MovementFlags.MOVEMENTFLAG_SPLINE
    Public Const TurningFlagsMask As Integer = MovementFlags.MOVEMENTFLAG_LEFT Or MovementFlags.MOVEMENTFLAG_RIGHT
    Public Const movementOrTurningFlagsMask As Integer = movementFlagsMask Or TurningFlagsMask

    Public Const ITEM_SLOT_NULL As Byte = 255
    Public Const ITEM_BAG_NULL As Long = -1

    Public Const PETITION_GUILD_PRICE As Integer = 1000
    Public Const PETITION_GUILD As Integer = 5863       'Guild Charter, ItemFlags = &H2000
    Public Const GUILD_TABARD_ITEM As Integer = 5976

    Public Enum TransportStates As Byte
        TRANSPORT_MOVE_TO_DOCK = 0
        TRANSPORT_DOCKED
        TRANSPORT_MOVE_NEXT_MAP
    End Enum

    Public Const CREATURE_MAX_SPELLS As Integer = 4
    Public Const MAX_OWNER_DIS As Integer = 100

    Public Enum PetType As Byte
        SUMMON_PET = 0
        HUNTER_PET = 1
        GUARDIAN_PET = 2
        MINI_PET = 3
    End Enum

    Public Enum PetSaveType As Integer
        PET_SAVE_DELETED = -1
        PET_SAVE_CURRENT = 0
        PET_SAVE_IN_STABLE_1 = 1
        PET_SAVE_IN_STABLE_2 = 2
        PET_SAVE_NO_SLOT = 3
    End Enum

    Public Enum HappinessState As Byte
        UNHAPPY = 1
        CONTENT = 2
        HAPPY = 3
    End Enum

    Public Enum LoyaltyState As Byte
        REBELLIOUS = 1
        UNRULY = 2
        SUBMISSIVE = 3
        DEPENDABLE = 4
        FAITHFUL = 5
        BEST_FRIEND = 6
    End Enum

    Public Enum PetSpellState As Byte
        SPELL_UNCHANGED = 0
        SPELL_CHANGED = 1
        SPELL_NEW = 2
        SPELL_REMOVED = 3
    End Enum

    Public Enum ActionFeedback As Byte
        FEEDBACK_NONE = 0
        FEEDBACK_PET_DEAD = 1
        FEEDBACK_NO_TARGET = 2
        FEEDBACK_CANT_ATT = 3
    End Enum

    Public Enum PetTalk As Byte
        PET_TALK_SPECIAL_SPELL = 0
        PET_TALK_ATTACK = 1
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

    Public Const SPELL_DURATION_INFINITE As Integer = -1
    Public Const MAX_AURA_EFFECTs_VISIBLE As Integer = 48                  '48 AuraSlots (32 buff, 16 debuff)
    Public Const MAX_AURA_EFFECTs_PASSIVE As Integer = 192
    Public Const MAX_AURA_EFFECTs As Integer = MAX_AURA_EFFECTs_VISIBLE + MAX_AURA_EFFECTs_PASSIVE
    Public Const MAX_AURA_EFFECT_FLAGs As Integer = MAX_AURA_EFFECTs_VISIBLE \ 8
    Public Const MAX_AURA_EFFECT_LEVELSs As Integer = MAX_AURA_EFFECTs_VISIBLE \ 4
    Public Const MAX_POSITIVE_AURA_EFFECTs As Integer = 32
    Public Const MAX_NEGATIVE_AURA_EFFECTs As Integer = MAX_AURA_EFFECTs_VISIBLE - MAX_POSITIVE_AURA_EFFECTs

    Public Enum CurrentSpellTypes
        CURRENT_MELEE_SPELL = 0
        CURRENT_GENERIC_SPELL = 1
        CURRENT_AUTOREPEAT_SPELL = 2
        CURRENT_CHANNELED_SPELL = 3
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

    Public Function GetShapeshiftModel(ByVal form As ShapeshiftForm, ByVal race As Races, ByVal model As Integer) As Integer
        Select Case form
            Case ShapeshiftForm.FORM_CAT
                If race = Races.RACE_NIGHT_ELF Then Return 892
                If race = Races.RACE_TAUREN Then Return 8571
            Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                If race = Races.RACE_NIGHT_ELF Then Return 2281
                If race = Races.RACE_TAUREN Then Return 2289
            Case ShapeshiftForm.FORM_MOONKIN
                If race = Races.RACE_NIGHT_ELF Then Return 15374
                If race = Races.RACE_TAUREN Then Return 15375
            Case ShapeshiftForm.FORM_TRAVEL
                Return 632
            Case ShapeshiftForm.FORM_AQUA
                Return 2428
            Case ShapeshiftForm.FORM_FLIGHT
                If race = Races.RACE_NIGHT_ELF Then Return 20857
                If race = Races.RACE_TAUREN Then Return 20872
            Case ShapeshiftForm.FORM_SWIFT
                If race = Races.RACE_NIGHT_ELF Then Return 21243
                If race = Races.RACE_TAUREN Then Return 21244
            Case ShapeshiftForm.FORM_GHOUL
                If race = Races.RACE_NIGHT_ELF Then Return 10045 Else Return model
            Case ShapeshiftForm.FORM_CREATUREBEAR
                Return 902
            Case ShapeshiftForm.FORM_GHOSTWOLF
                Return 4613
            Case ShapeshiftForm.FORM_TREE
                Return 864
            Case ShapeshiftForm.FORM_SPIRITOFREDEMPTION
                Return 12824
            Case Else
                Return model
                'Case ShapeshiftForm.FORM_CREATURECAT
                'Case ShapeshiftForm.FORM_AMBIENT
                'Case ShapeshiftForm.FORM_SHADOW
        End Select
    End Function

    Public Function GetShapeshiftManaType(ByVal form As ShapeshiftForm, ByVal manaType As ManaTypes) As ManaTypes
        Select Case form
            Case ShapeshiftForm.FORM_CAT, ShapeshiftForm.FORM_STEALTH
                Return ManaTypes.TYPE_ENERGY
            Case ShapeshiftForm.FORM_AQUA, ShapeshiftForm.FORM_TRAVEL, ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_TREE, _
                 ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_MOONKIN, ShapeshiftForm.FORM_SPIRITOFREDEMPTION, ShapeshiftForm.FORM_FLIGHT, ShapeshiftForm.FORM_SWIFT
                Return ManaTypes.TYPE_MANA
            Case ShapeshiftForm.FORM_BEAR, ShapeshiftForm.FORM_DIREBEAR
                Return ManaTypes.TYPE_RAGE
            Case Else
                Return manaType
        End Select
    End Function
End Module
