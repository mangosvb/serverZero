

Public Module Constants
    Public Function GuidIsCreature(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_UNIT Then Return True
        Return False
    End Function
    Public Function GuidIsPet(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_PET Then Return True
        Return False
    End Function
    Public Function GuidIsItem(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_ITEM Then Return True
        Return False
    End Function
    Public Function GuidIsGameObject(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_GAMEOBJECT Then Return True
        Return False
    End Function
    Public Function GuidIsDnyamicObject(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_DYNAMICOBJECT Then Return True
        Return False
    End Function
    Public Function GuidIsTransport(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_TRANSPORT Then Return True
        Return False
    End Function
    Public Function GuidIsMoTransport(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_MO_TRANSPORT Then Return True
        Return False
    End Function
    Public Function GuidIsCorpse(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_CORPSE Then Return True
        Return False
    End Function
    Public Function GuidIsPlayer(ByVal GUID As ULong) As Boolean
        If GuidHIGH2(GUID) = GUID_PLAYER Then Return True
        Return False
    End Function
    Public Function GuidHIGH2(ByVal GUID As ULong) As ULong
        Return (GUID And GUID_MASK_HIGH)
    End Function
    Public Function GuidHIGH(ByVal GUID As ULong) As UInteger
        Return CUInt((GUID And GUID_MASK_HIGH) >> 32UL)
    End Function
    Public Function GuidLOW(ByVal GUID As ULong) As UInteger
        Return (GUID And GUID_MASK_LOW)
    End Function

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

    Public Const MAX_FRIENDS_ON_LIST As Byte = 50
    Public Const MAX_IGNORES_ON_LIST As Byte = 25
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

    Public Const EQUIPMENT_SLOT_START As Byte = 0
    Public Const EQUIPMENT_SLOT_HEAD As Byte = 0
    Public Const EQUIPMENT_SLOT_NECK As Byte = 1
    Public Const EQUIPMENT_SLOT_SHOULDERS As Byte = 2
    Public Const EQUIPMENT_SLOT_BODY As Byte = 3
    Public Const EQUIPMENT_SLOT_CHEST As Byte = 4
    Public Const EQUIPMENT_SLOT_WAIST As Byte = 5
    Public Const EQUIPMENT_SLOT_LEGS As Byte = 6
    Public Const EQUIPMENT_SLOT_FEET As Byte = 7
    Public Const EQUIPMENT_SLOT_WRISTS As Byte = 8
    Public Const EQUIPMENT_SLOT_HANDS As Byte = 9
    Public Const EQUIPMENT_SLOT_FINGER1 As Byte = 10
    Public Const EQUIPMENT_SLOT_FINGER2 As Byte = 11
    Public Const EQUIPMENT_SLOT_TRINKET1 As Byte = 12
    Public Const EQUIPMENT_SLOT_TRINKET2 As Byte = 13
    Public Const EQUIPMENT_SLOT_BACK As Byte = 14
    Public Const EQUIPMENT_SLOT_MAINHAND As Byte = 15
    Public Const EQUIPMENT_SLOT_OFFHAND As Byte = 16
    Public Const EQUIPMENT_SLOT_RANGED As Byte = 17
    Public Const EQUIPMENT_SLOT_TABARD As Byte = 18
    Public Const EQUIPMENT_SLOT_END As Byte = 19

    Public Const INVENTORY_SLOT_BAG_START As Byte = 19
    Public Const INVENTORY_SLOT_BAG_1 As Byte = 19
    Public Const INVENTORY_SLOT_BAG_2 As Byte = 20
    Public Const INVENTORY_SLOT_BAG_3 As Byte = 21
    Public Const INVENTORY_SLOT_BAG_4 As Byte = 22
    Public Const INVENTORY_SLOT_BAG_END As Byte = 23

    Public Const INVENTORY_SLOT_ITEM_START As Byte = 23
    Public Const INVENTORY_SLOT_ITEM_1 As Byte = 23
    Public Const INVENTORY_SLOT_ITEM_2 As Byte = 24
    Public Const INVENTORY_SLOT_ITEM_3 As Byte = 25
    Public Const INVENTORY_SLOT_ITEM_4 As Byte = 26
    Public Const INVENTORY_SLOT_ITEM_5 As Byte = 27
    Public Const INVENTORY_SLOT_ITEM_6 As Byte = 28
    Public Const INVENTORY_SLOT_ITEM_7 As Byte = 29
    Public Const INVENTORY_SLOT_ITEM_8 As Byte = 30
    Public Const INVENTORY_SLOT_ITEM_9 As Byte = 31
    Public Const INVENTORY_SLOT_ITEM_10 As Byte = 32
    Public Const INVENTORY_SLOT_ITEM_11 As Byte = 33
    Public Const INVENTORY_SLOT_ITEM_12 As Byte = 34
    Public Const INVENTORY_SLOT_ITEM_13 As Byte = 35
    Public Const INVENTORY_SLOT_ITEM_14 As Byte = 36
    Public Const INVENTORY_SLOT_ITEM_15 As Byte = 37
    Public Const INVENTORY_SLOT_ITEM_16 As Byte = 38
    Public Const INVENTORY_SLOT_ITEM_END As Byte = 39

    Public Const BANK_SLOT_ITEM_START As Byte = 39
    Public Const BANK_SLOT_ITEM_1 As Byte = 39
    Public Const BANK_SLOT_ITEM_2 As Byte = 40
    Public Const BANK_SLOT_ITEM_3 As Byte = 41
    Public Const BANK_SLOT_ITEM_4 As Byte = 42
    Public Const BANK_SLOT_ITEM_5 As Byte = 43
    Public Const BANK_SLOT_ITEM_6 As Byte = 44
    Public Const BANK_SLOT_ITEM_7 As Byte = 45
    Public Const BANK_SLOT_ITEM_8 As Byte = 46
    Public Const BANK_SLOT_ITEM_9 As Byte = 47
    Public Const BANK_SLOT_ITEM_10 As Byte = 48
    Public Const BANK_SLOT_ITEM_11 As Byte = 49
    Public Const BANK_SLOT_ITEM_12 As Byte = 50
    Public Const BANK_SLOT_ITEM_13 As Byte = 51
    Public Const BANK_SLOT_ITEM_14 As Byte = 52
    Public Const BANK_SLOT_ITEM_15 As Byte = 53
    Public Const BANK_SLOT_ITEM_16 As Byte = 54
    Public Const BANK_SLOT_ITEM_17 As Byte = 55
    Public Const BANK_SLOT_ITEM_18 As Byte = 56
    Public Const BANK_SLOT_ITEM_19 As Byte = 57
    Public Const BANK_SLOT_ITEM_20 As Byte = 58
    Public Const BANK_SLOT_ITEM_21 As Byte = 59
    Public Const BANK_SLOT_ITEM_22 As Byte = 60
    Public Const BANK_SLOT_ITEM_23 As Byte = 61
    Public Const BANK_SLOT_ITEM_24 As Byte = 62
    Public Const BANK_SLOT_ITEM_END As Byte = 63

    Public Const BANK_SLOT_BAG_START As Byte = 63
    Public Const BANK_SLOT_BAG_1 As Byte = 63
    Public Const BANK_SLOT_BAG_2 As Byte = 64
    Public Const BANK_SLOT_BAG_3 As Byte = 65
    Public Const BANK_SLOT_BAG_4 As Byte = 66
    Public Const BANK_SLOT_BAG_5 As Byte = 67
    Public Const BANK_SLOT_BAG_6 As Byte = 68
    Public Const BANK_SLOT_BAG_END As Byte = 69

    Public Const BUYBACK_SLOT_START As Byte = 69
    Public Const BUYBACK_SLOT_1 As Byte = 69
    Public Const BUYBACK_SLOT_2 As Byte = 70
    Public Const BUYBACK_SLOT_3 As Byte = 71
    Public Const BUYBACK_SLOT_4 As Byte = 72
    Public Const BUYBACK_SLOT_5 As Byte = 73
    Public Const BUYBACK_SLOT_6 As Byte = 74
    Public Const BUYBACK_SLOT_7 As Byte = 75
    Public Const BUYBACK_SLOT_8 As Byte = 76
    Public Const BUYBACK_SLOT_9 As Byte = 77
    Public Const BUYBACK_SLOT_10 As Byte = 78
    Public Const BUYBACK_SLOT_11 As Byte = 79
    Public Const BUYBACK_SLOT_12 As Byte = 80
    Public Const BUYBACK_SLOT_END As Byte = 81

    Public Const KEYRING_SLOT_START As Byte = 81
    Public Const KEYRING_SLOT_1 As Byte = 81
    Public Const KEYRING_SLOT_2 As Byte = 82
    Public Const KEYRING_SLOT_31 As Byte = 112
    Public Const KEYRING_SLOT_32 As Byte = 113
    Public Const KEYRING_SLOT_END As Byte = 113

    Public Const QUEST_OBJECTIVES_COUNT As Integer = 3
    Public Const QUEST_REWARD_CHOICES_COUNT As Integer = 5
    Public Const QUEST_REWARDS_COUNT As Integer = 3
    Public Const QUEST_DEPLINK_COUNT As Integer = 9
    Public Const QUEST_SLOTS As Integer = 24
    Public Const QUEST_SHARING_DISTANCE As Integer = 10



    Public Enum AuthLoginCodes
        CHAR_LOGIN_FAILED = 0                       'Login failed
        CHAR_LOGIN_NO_WORLD = 1                     'World server is down
        CHAR_LOGIN_DUPLICATE_CHARACTER = 2          'A character with that name already exists
        CHAR_LOGIN_NO_INSTANCES = 3                 'No instance servers are available
        CHAR_LOGIN_DISABLED = 4                     'Login for that race and/or class is currently disabled
        CHAR_LOGIN_NO_CHARACTER = 5                 'Character not found
        CHAR_LOGIN_LOCKED_FOR_TRANSFER = 6
        CHAR_LOGIN_LOCKED_BY_BILLING = 7
    End Enum
    Public Enum AuthResponseCodes
        RESPONSE_SUCCESS = &H0                      'Success
        RESPONSE_FAILURE = &H1                      'Failure
        RESPONSE_CANCELLED = &H2                    'Cancelled
        RESPONSE_DISCONNECTED = &H3                 'Disconnected from server
        RESPONSE_FAILED_TO_CONNECT = &H4            'Failed to connect
        RESPONSE_CONNECTED = &H5                    'Connected
        RESPONSE_VERSION_MISMATCH = &H6             'Wrong client version

        CSTATUS_CONNECTING = &H7                    'Connecting to server...
        CSTATUS_NEGOTIATING_SECURITY = &H8          'Negotiating Security
        CSTATUS_NEGOTIATION_COMPLETE = &H9          'Security negotiation complete
        CSTATUS_NEGOTIATION_FAILED = &HA            'Security negotiation failed
        CSTATUS_AUTHENTICATING = &HB                'Authenticating

        AUTH_OK = &HC                               'Authentication successful
        AUTH_FAILED = &HD                           'Authentication failed
        AUTH_LOGIN_UNAVAILABLE = &HE                'Login unavailable
        AUTH_BAD_SERVER_PROOF = &HF                 'Server is not valid
        AUTH_UNAVAILABLE = &H10                     'System unavailable - please try again later
        AUTH_SYSTEM_ERROR = &H11                    'System error
        AUTH_BILLING_ERROR = &H12                   'Billing system error
        AUTH_BILLING_EXPIRED = &H13                 'Account billing has expired
        AUTH_VERSION_MISMATCH = &H14                'Wrong client version
        AUTH_UNKNOWN_ACCOUNT = &H15                 'Unknown account
        AUTH_INCORRECT_PASSWORD = &H16              'Incorrect password
        AUTH_SESSION_EXPIRED = &H17                 'Session expired
        AUTH_SERVER_SHUTTING_DOWN = &H18            'Server shutting down
        AUTH_ALREADY_LOGGING_IN = &H19              'Already logging in
        AUTH_LOGIN_SERVER_NOT_FOUND = &H1A          'Invalid login server
        AUTH_WAIT_QUEUE = &H1B                      'Position in queue - 0
        AUTH_BANNED = &H1C                          'This account has been banned
        AUTH_ALREADY_ONLINE = &H1D                  'This character is still logged on
        AUTH_NO_TIME = &H1E                         'Your WoW subscription has expired
        AUTH_DB_BUSY = &H1F                         'This session has timed out
        AUTH_SUSPENDED = &H20                       'This account has been temporarily suspended
        AUTH_PARENTAL_CONTROL = &H21                'Access to this account blocked by parental controls 

        REALM_LIST_IN_PROGRESS = &H22               'Retrieving realm list
        REALM_LIST_SUCCESS = &H23                   'Realm list retrieved
        REALM_LIST_FAILED = &H24                    'Unable to connect to realm list server
        REALM_LIST_INVALID = &H25                   'Invalid realm list
        REALM_LIST_REALM_NOT_FOUND = &H26           'Realm is down

        ACCOUNT_CREATE_IN_PROGRESS = &H27           'Creating account
        ACCOUNT_CREATE_SUCCESS = &H28               'Account created
        ACCOUNT_CREATE_FAILED = &H29                'Account creation failed

        CHAR_LIST_RETRIEVED = &H2A                  'Retrieving character list
        CHAR_LIST_SUCCESS = &H2B                    'Character list retrieved
        CHAR_LIST_FAILED = &H2C                     'Error retrieving character list

        CHAR_CREATE_IN_PROGRESS = &H2D              'Creating character
        CHAR_CREATE_SUCCESS = &H2E                  'Character created
        CHAR_CREATE_ERROR = &H2F                    'Error creating character
        CHAR_CREATE_FAILED = &H30                   'Character creation failed
        CHAR_CREATE_NAME_IN_USE = &H31              'That name is unavailable
        CHAR_CREATE_DISABLED = &H32                 'Creation of that race/class is disabled
        CHAR_CREATE_PVP_TEAMS_VIOLATION = &H33      'You cannot have both horde and alliance character at pvp realm
        CHAR_CREATE_SERVER_LIMIT = &H34             'You already have maximum number of characters
        CHAR_CREATE_ACCOUNT_LIMIT = &H35            'You already have maximum number of characters
        CHAR_CREATE_SERVER_QUEUE = &H36             'The server is currently queued
        CHAR_CREATE_ONLY_EXISTING = &H37            'Only players who have characters on this realm..

        CHAR_DELETE_IN_PROGRESS = &H38              'Deleting character
        CHAR_DELETE_SUCCESS = &H39                  'Character deleted
        CHAR_DELETE_FAILED = &H3A                   'Char deletion failed

        CHAR_LOGIN_IN_PROGRESS = &H3B               'Entering the World of Warcraft
        CHAR_LOGIN_SUCCESS = &H3C                   'Login successful
        CHAR_LOGIN_NO_WORLD = &H3D                  'World server is down
        CHAR_LOGIN_DUPLICATE_CHARACTER = &H3E       'A character with that name already exists
        CHAR_LOGIN_NO_INSTANCES = &H3F              'No instance servers are available
        CHAR_LOGIN_FAILED = &H40                    'Login failed
        CHAR_LOGIN_DISABLED = &H41                  'Login for that race and/or class is currently disabled
        CHAR_LOGIN_NO_CHARACTER = &H42              'Character not found

        CHAR_NAME_NO_NAME = &H43                    'Enter a name for your character
        CHAR_NAME_TOO_SHORT = &H44                  'Names must be atleast 2 characters long
        CHAR_NAME_TOO_LONG = &H45                   'Names must be no more then 12 characters
        CHAR_NAME_INVALID_CHARACTER = &H46          'Names can only contain letters
        CHAR_NAME_MIXED_LANGUAGES = &H47            'Names must contain only one language
        CHAR_NAME_PROFANE = &H48                    'That name contains mature language
        CHAR_NAME_RESERVED = &H49                   'That name is unavailable
        CHAR_NAME_INVALID_APOSTROPHE = &H4A         'You cannot use an apostrophe
        CHAR_NAME_MULTIPLE_APOSTROPHES = &H4B       'You can only have one apostrophe
        CHAR_NAME_THREE_CONSECUTIVE = &H4C          'You cannot use the same letter three times consecutively
        CHAR_NAME_INVALID_SPACE = &H4D              'You cannot use space as the first or last character of your name
    End Enum

#Region "Player.Enums"


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
        PLAYER_FLAG_GROUP_LEADER = &H1
        PLAYER_FLAG_AFK = &H2
        PLAYER_FLAG_DND = &H4
        PLAYER_FLAG_GM = &H8
        PLAYER_FLAG_DEAD = &H10
        PLAYER_FLAG_RESTING = &H20
        PLAYER_FLAG_UNKNOWN1 = &H40
        PLAYER_FLAG_FREE_FOR_ALL_PVP = &H80
        PLAYER_FLAGS_CONTESTED_PVP = &H100
        PLAYER_FLAG_PVP_TOGGLE = &H200
        PLAYER_FLAG_HIDE_HELM = &H400
        PLAYER_FLAG_HIDE_CLOAK = &H800
        PLAYER_FLAG_NEED_REST_3_HOURS = &H1000
        PLAYER_FLAG_NEED_REST_5_HOURS = &H2000
        PLAYER_FLAG_PVP = &H40000
    End Enum
    Public Enum PlayerHonorRank As Byte
        RANK_NONE = 0

        RANK_PARIAH = 1
        RANK_OUTLAW = 2
        RANK_EXILED = 3
        RANK_DISHONORED = 4

        RANK_A_RIVATE = 5
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

#End Region
#Region "Player.Groups"

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
        MEMBER_STATUS_UNK0 = &H4            ' dead? (health=0)
        MEMBER_STATUS_UNK1 = &H8            ' ghost? (health=1)
        MEMBER_STATUS_UNK2 = &H10           ' never seen
        MEMBER_STATUS_UNK3 = &H20           ' never seen
        MEMBER_STATUS_UNK4 = &H40           ' appears with dead and ghost flags
        MEMBER_STATUS_UNK5 = &H80           ' never seen
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

#End Region
#Region "Player.Chat"

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
        FLAG_NONE = 0
        FLAG_AFK = 1
        FLAG_DND = 2
        FLAG_GM = 3
    End Enum

#End Region
#Region "Object.Flags"

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

    Public Enum UnitFlags   'Flags for units
        UNIT_FLAG_NONE = &H0
        UNIT_FLAG_UNK1 = &H1
        UNIT_FLAG_NOT_ATTACKABLE = &H2                                                  'Unit is not attackable
        UNIT_FLAG_DISABLE_MOVE = &H4                                                    'Unit is frozen, rooted or stunned
        UNIT_FLAG_ATTACKABLE = &H8                                                      'Unit becomes temporarily hostile, shows in red, allows attack
        UNIT_FLAG_RENAME = &H10
        UNIT_FLAG_RESTING = &H20
        UNIT_FLAG_UNK5 = &H40
        UNIT_FLAG_NOT_ATTACKABLE_1 = &H80                                               'Unit cannot be attacked by player, shows no attack cursor
        UNIT_FLAG_UNK6 = &H100
        UNIT_FLAG_UNK7 = &H200
        UNIT_FLAG_NON_PVP_PLAYER = UNIT_FLAG_ATTACKABLE + UNIT_FLAG_NOT_ATTACKABLE_1    'Unit cannot be attacked by player, shows in blue
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

#End Region
#Region "Creatures.Types"

    Public Enum TrainerTypes
        TRAINER_TYPE_CLASS = 0
        TRAINER_TYPE_MOUNTS = 1
        TRAINER_TYPE_TRADESKILLS = 2
        TRAINER_TYPE_PETS = 3
    End Enum
    Public Enum UNIT_TYPE
        NOUNITTYPE = 0
        BEAST = 1
        DRAGONKIN = 2
        DEMON = 3
        ELEMENTAL = 4
        GIANT = 5
        UNDEAD = 6
        HUMANOID = 7
        CRITTER = 8
        MECHANICAL = 9
        MOUNT = 10
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
    End Enum

    Public Enum CREATURE_ELITE As Integer
        NORMAL = 0
        ELITE = 1
        RAREELITE = 2
        WORLDBOSS = 3
        RARE = 4
    End Enum

#End Region

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
        BATTLEGROUND_EyeOfTheStorm = 7
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

    Public Enum QuestObjectiveFlag 'These flags are custom and are only used for MangosVB
        QUEST_OBJECTIVE_KILL = 1 'You have to kill creatures
        QUEST_OBJECTIVE_EXPLORE = 2 'You have to explore an area
        QUEST_OBJECTIVE_ESCORT = 4 'You have to escort someone
        QUEST_OBJECTIVE_EVENT = 8 'Something is required to happen (escort may be included in this one)
        QUEST_OBJECTIVE_CAST = 16 'You will have to cast a spell on a creature or a gameobject (spells on gameobjects are f.ex opening)
        QUEST_OBJECTIVE_ITEM = 32 'You have to recieve some items to deliver
        QUEST_OBJECTIVE_EMOTE = 64 'You do an emote to a creature
    End Enum

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

    'RealmServ OP Codes
    Public Const CMD_AUTH_LOGON_CHALLENGE As Integer = &H0
    Public Const CMD_AUTH_LOGON_PROOF As Integer = &H1
    Public Const CMD_AUTH_RECONNECT_CHALLENGE As Integer = &H2
    Public Const CMD_AUTH_RECONNECT_PROOF As Integer = &H3
    Public Const CMD_AUTH_UPDATESRV As Integer = &H4
    Public Const CMD_AUTH_REALMLIST As Integer = &H10

    'UpdateServ OP Codes
    Public Const CMD_XFER_INITIATE As Integer = &H30  'client? from server
    Public Const CMD_XFER_DATA As Integer = &H31      'client? from server
    Public Const CMD_XFER_ACCEPT As Integer = &H32    'not official name, from client
    Public Const CMD_XFER_RESUME As Integer = &H33    'not official name, from client
    Public Const CMD_XFER_CANCEL As Integer = &H34    'not official name, from client

    'Unknown
    Public Const CMD_GRUNT_AUTH_CHALLENGE As Integer = &H0    'server
    Public Const CMD_GRUNT_AUTH_VERIFY As Integer = &H2       'server
    Public Const CMD_GRUNT_CONN_PING As Integer = &H10        'server
    Public Const CMD_GRUNT_CONN_PONG As Integer = &H11        'server
    Public Const CMD_GRUNT_HELLO As Integer = &H20            'server
    Public Const CMD_GRUNT_PROVESESSION As Integer = &H21     'server
    Public Const CMD_GRUNT_KICK As Integer = &H24             'server



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
End Module
