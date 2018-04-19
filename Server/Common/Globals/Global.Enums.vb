'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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

'TODO: Reorganize this file.

Public Module Global_Enums
    Public Enum ExpansionLevel As Byte
        NORMAL = 0      'Vanilla
    End Enum

    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Developer = 3
        Admin = 4
    End Enum

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

    <Flags()>
    Public Enum CharacterFlagState
        CHARACTER_FLAG_NONE = &H0
        CHARACTER_FLAG_UNK1 = &H1
        CHARACTER_FLAG_UNK2 = &H2
        CHARACTER_FLAG_LOCKED_FOR_TRANSFER = &H4                    'Character Locked for Paid Character Transfer
        CHARACTER_FLAG_UNK4 = &H8
        CHARACTER_FLAG_UNK5 = &H10
        CHARACTER_FLAG_UNK6 = &H20
        CHARACTER_FLAG_UNK7 = &H40
        CHARACTER_FLAG_UNK8 = &H80
        CHARACTER_FLAG_UNK9 = &H100
        CHARACTER_FLAG_UNK10 = &H200
        CHARACTER_FLAG_HIDE_HELM = &H400
        CHARACTER_FLAG_HIDE_CLOAK = &H800
        CHARACTER_FLAG_UNK13 = &H1000
        CHARACTER_FLAG_GHOST = &H2000                               'Player is ghost in char selection screen
        CHARACTER_FLAG_RENAME = &H4000                              'On login player will be asked to change name
        CHARACTER_FLAG_UNK16 = &H8000
        CHARACTER_FLAG_UNK17 = &H10000
        CHARACTER_FLAG_UNK18 = &H20000
        CHARACTER_FLAG_UNK19 = &H40000
        CHARACTER_FLAG_UNK20 = &H80000
        CHARACTER_FLAG_UNK21 = &H100000
        CHARACTER_FLAG_UNK22 = &H200000
        CHARACTER_FLAG_UNK23 = &H400000
        CHARACTER_FLAG_UNK24 = &H800000
        CHARACTER_FLAG_LOCKED_BY_BILLING = &H1000000
        CHARACTER_FLAG_DECLINED = &H2000000
        CHARACTER_FLAG_UNK27 = &H4000000
        CHARACTER_FLAG_UNK28 = &H8000000
        CHARACTER_FLAG_UNK29 = &H10000000
        CHARACTER_FLAG_UNK30 = &H20000000
        CHARACTER_FLAG_UNK31 = &H40000000
        CHARACTER_FLAG_UNK32 = &H80000000
    End Enum

    Public Enum LogoutResponseCode As Byte
        LOGOUT_RESPONSE_ACCEPTED = &H0
        LOGOUT_RESPONSE_DENIED = &HC
    End Enum

    Public Enum ATLoginFlags As Byte
        AT_LOGIN_NONE = &H0
        AT_LOGIN_RENAME = &H1
        AT_LOGIN_RESET_SPELLS = &H2
        AT_LOGIN_RESET_TALENTS = &H4
        AT_LOGIN_FIRST = &H20
    End Enum

    Public Enum CorpseType
        CORPSE_BONES = 0
        CORPSE_RESURRECTABLE_PVE = 1
        CORPSE_RESURRECTABLE_PVP = 2
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

    Public Enum PartyCommand As Byte
        PARTY_OP_INVITE = 0
        PARTY_OP_LEAVE = 2
    End Enum

    Public Enum PartyCommandResult As Byte
        INVITE_OK = 0                   'You have invited [name] to join your group.
        INVITE_NOT_FOUND = 1            'Cannot find [name].
        INVITE_NOT_IN_YOUR_PARTY = 2    '[name] is not in your party.
        INVITE_NOT_IN_YOUR_INSTANCE = 3 '[name] is not in your instance.
        INVITE_PARTY_FULL = 4           'Your party is full.
        INVITE_ALREADY_IN_GROUP = 5     '[name] is already in group.
        INVITE_NOT_IN_PARTY = 6         'You aren't in party.
        INVITE_NOT_LEADER = 7           'You are not the party leader.
        INVITE_NOT_SAME_SIDE = 8        'gms - Target is not part of your alliance.
        INVITE_IGNORED = 9              'Test is ignoring you.
        INVITE_RESTRICTED = 13
    End Enum

    Private Enum PromoteToMain As Byte
        MainTank = 0
        MainAssist = 1
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

    Public Enum AuctionHouses As Integer
        AUCTION_UNDEFINED = 0
        AUCTION_ALLIANCE = 2
        AUCTION_HORDE = 6
        AUCTION_NEUTRAL = 7
        AUCTION_STORMWIND = 1
        AUCTION_IRONFORGE = 2
        AUCTION_DARNASSYS = 3
        AUCTION_UNDERCITY = 4
        AUCTION_THUNDER_BLUFF = 5
        AUCTION_ORGRIMMAR = 6
        AUCTION_BLACKWATER = 7
    End Enum

    Public Enum AuctionAction As Integer
        AUCTION_SELL_ITEM = 0
        AUCTION_CANCEL = 1
        AUCTION_PLACE_BID = 2
    End Enum

    Public Enum AuctionError As Integer
        AUCTION_OK = 0
        AUCTION_INTERNAL_ERROR = 2
        AUCTION_NOT_ENOUGHT_MONEY = 3
        CANNOT_BID_YOUR_AUCTION_ERROR = 10
    End Enum

    'Auction Mail Format:
    '
    'Outbid
    '       Subject -> ItemID:0:0
    '       Body    -> ""
    '       Money returned
    'Auction won
    '       Subject -> ItemID:0:1
    '       Body    -> FFFFFFFF:Bid:Buyout
    '       Item received    
    'Auction Successful
    '       Subject -> ItemID:0:2
    '       Body    -> FFFFFFFF:Bid:Buyout:0:0
    '       Money received   
    'Auction Canceled
    '       Subject -> ItemID:0:4
    '       Body    -> ""
    '       Item returned
    Public Enum MailAuctionAction As Integer
        OUTBID = 0
        AUCTION_WON = 1
        AUCTION_SUCCESSFUL = 2
        AUCTION_CANCELED = 3
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

    Public Enum EnvironmentalDamage
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

    Public Enum GuildRankRights
        GR_RIGHT_EMPTY = &H40
        GR_RIGHT_GCHATLISTEN = &H41
        GR_RIGHT_GCHATSPEAK = &H42
        GR_RIGHT_OFFCHATLISTEN = &H44
        GR_RIGHT_OFFCHATSPEAK = &H48
        GR_RIGHT_PROMOTE = &HC0
        GR_RIGHT_DEMOTE = &H140
        GR_RIGHT_INVITE = &H50
        GR_RIGHT_REMOVE = &H60
        GR_RIGHT_SETMOTD = &H1040
        GR_RIGHT_EPNOTE = &H2040
        GR_RIGHT_VIEWOFFNOTE = &H4040
        GR_RIGHT_EOFFNOTE = &H8040
        GR_RIGHT_ALL = &HF1FF
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

    'Default Guild Ranks
    'TODO: Set the ranks during guild creation
    Public Enum GuildDefaultRanks As Byte
        GR_GUILDMASTER = 0
        GR_OFFICER = 1
        GR_VETERAN = 2
        GR_MEMBER = 3
        GR_INITIATE = 4
    End Enum

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
        ITEM_SUBCLASS_FISHING = 9
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

    Public Enum TransferAbortReason As Short
        TRANSFER_ABORT_MAX_PLAYERS = &H1                ' Transfer Aborted: instance is full
        TRANSFER_ABORT_NOT_FOUND = &H2                  ' Transfer Aborted: instance not found
        TRANSFER_ABORT_TOO_MANY_INSTANCES = &H3         ' You have entered too many instances recently.
        TRANSFER_ABORT_ZONE_IN_COMBAT = &H5             ' Unable to zone in while an encounter is in progress.
        TRANSFER_ABORT_INSUF_EXPAN_LVL1 = &H106         ' You must have TBC expansion installed to access this area.
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

    <CLSCompliant(False)> _
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

    <Flags()>
    Public Enum ChatChannelsFlags
        FLAG_NONE = &H0
        FLAG_INITIAL = &H1              ' General, Trade, LocalDefense, LFG
        FLAG_ZONE_DEP = &H2             ' General, Trade, LocalDefense, GuildRecruitment
        FLAG_GLOBAL = &H4               ' WorldDefense
        FLAG_TRADE = &H8                ' Trade
        FLAG_CITY_ONLY = &H10           ' Trade, GuildRecruitment
        FLAG_CITY_ONLY2 = &H20          ' Trade, GuildRecruitment
        FLAG_DEFENSE = &H10000          ' LocalDefense, WorldDefense
        FLAG_GUILD_REQ = &H20000        ' GuildRecruitment
        FLAG_LFG = &H40000              ' LookingForGroup
    End Enum

    <Flags()>
    Public Enum CHANNEL_FLAG As Byte
        'General                  0x18 = 0x10 | 0x08
        'Trade                    0x3C = 0x20 | 0x10 | 0x08 | 0x04
        'LocalDefence             0x18 = 0x10 | 0x08
        'GuildRecruitment         0x38 = 0x20 | 0x10 | 0x08
        'LookingForGroup          0x50 = 0x40 | 0x10

        CHANNEL_FLAG_NONE = &H0
        CHANNEL_FLAG_CUSTOM = &H1
        CHANNEL_FLAG_UNK1 = &H2
        CHANNEL_FLAG_TRADE = &H4
        CHANNEL_FLAG_NOT_LFG = &H8
        CHANNEL_FLAG_GENERAL = &H10
        CHANNEL_FLAG_CITY = &H20
        CHANNEL_FLAG_LFG = &H40
    End Enum

    <Flags()>
    Public Enum CHANNEL_USER_FLAG As Byte
        CHANNEL_FLAG_NONE = &H0
        CHANNEL_FLAG_OWNER = &H1
        CHANNEL_FLAG_MODERATOR = &H2
        CHANNEL_FLAG_MUTED = &H4
        CHANNEL_FLAG_CUSTOM = &H10
    End Enum

    Public Enum CHANNEL_NOTIFY_FLAGS
        CHANNEL_JOINED = 0                      ' %s joined channel.
        CHANNEL_LEFT = 1                        ' %s left channel.
        CHANNEL_YOU_JOINED = 2                  ' Joined Channel: [%s]
        CHANNEL_YOU_LEFT = 3                    ' Left Channel: [%s]
        CHANNEL_WRONG_PASS = 4                  ' Wrong password for %s.
        CHANNEL_NOT_ON = 5                      ' Not on channel %s.
        CHANNEL_NOT_MODERATOR = 6               ' Not a moderator of %s.
        CHANNEL_SET_PASSWORD = 7                ' [%s] Password changed by %s.
        CHANNEL_CHANGE_OWNER = 8                ' [%s] Owner changed to %s.
        CHANNEL_NOT_ON_FOR_NAME = 9             ' [%s] Player %s was not found.
        CHANNEL_NOT_OWNER = &HA                 ' [%s] You are not the channel owner.
        CHANNEL_WHO_OWNER = &HB                 ' [%s] Channel owner is %s.
        CHANNEL_MODE_CHANGE = &HC               '
        CHANNEL_ENABLE_ANNOUNCE = &HD           ' [%s] Channel announcements enabled by %s.
        CHANNEL_DISABLE_ANNOUNCE = &HE          ' [%s] Channel announcements disabled by %s.
        CHANNEL_MODERATED = &HF                 ' [%s] Channel moderation enabled by %s.
        CHANNEL_UNMODERATED = &H10              ' [%s] Channel moderation disabled by %s.
        CHANNEL_YOUCANTSPEAK = &H11             ' [%s] You do not have permission to speak.
        CHANNEL_KICKED = &H12                   ' [%s] Player %s kicked by %s.
        CHANNEL_YOU_ARE_BANNED = &H13           ' [%s] You are banned from that channel.
        CHANNEL_BANNED = &H14                   ' [%s] Player %s banned by %s.
        CHANNEL_UNBANNED = &H15                 ' [%s] Player %s unbanned by %s.
        CHANNEL_NOT_BANNED = &H16               ' [%s] Player %s is not banned.
        CHANNEL_ALREADY_ON = &H17               ' [%s] Player %s is already on the channel.
        CHANNEL_INVITED = &H18                  ' %s has invited you to join the channel '%s'
        CHANNEL_INVITED_WRONG_FACTION = &H19    ' Target is in the wrong alliance for %s.
        CHANNEL_WRONG_FACTION = &H1A            ' Wrong alliance for %s.
        CHANNEL_INVALID_NAME = &H1B             ' Invalid channel name
        CHANNEL_NOT_MODERATED = &H1C            ' %s is not moderated
        CHANNEL_PLAYER_INVITED = &H1D           ' [%s] You invited %s to join the channel
        CHANNEL_PLAYER_INVITE_BANNED = &H1E     ' [%s] %s has been banned.
        CHANNEL_THROTTLED = &H1F                ' [%s] The number of messages that can be sent to this channel is limited, please wait to send another message.
        CHANNEL_NOT_IN_AREA = &H20              ' [%s] You are not in the correct area for this channel.
        CHANNEL_NOT_IN_LFG = &H21               ' [%s] You must be queued in looking for group before joining this channel.
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

    Public Enum MaievResponse As Byte
        MAIEV_RESPONSE_FAILED_OR_MISSING = &H0          'The module was either currupt or not in the cache request transfer
        MAIEV_RESPONSE_SUCCESS = &H1                    'The module was in the cache and loaded successfully
        MAIEV_RESPONSE_RESULT = &H2
        MAIEV_RESPONSE_HASH = &H4
    End Enum
    Public Enum MaievOpcode As Byte
        MAIEV_MODULE_INFORMATION = 0
        MAIEV_MODULE_TRANSFER = 1
        MAIEV_MODULE_RUN = 2
        MAIEV_MODULE_UNK = 3
        MAIEV_MODULE_SEED = 5
    End Enum

    Public Enum CheckTypes As Byte
        MEM_CHECK = 0
        PAGE_CHECK_A_B = 1
        MPQ_CHECK = 2
        LUA_STR_CHECK = 3
        DRIVER_CHECK = 4
        TIMING_CHECK = 5
        PROC_CHECK = 6
        MODULE_CHECK = 7
    End Enum

    Public Enum Axis As Integer
        X_AXIS = 0
        Y_AXIS = 1
        Z_AXIS = 2
        DETECT_AXIS = -1
    End Enum

    Public Enum InvisibilityLevel As Byte
        VISIBLE = 0
        STEALTH = 1
        INIVISIBILITY = 2
        DEAD = 3
        GM = 4
    End Enum

    Public Enum GameObjectLootState As Byte
        DOOR_OPEN = 0
        DOOR_CLOSED = 1
        LOOT_UNAVIABLE = 0
        LOOT_UNLOOTED = 1
        LOOT_LOOTED = 2
    End Enum

    Public Enum TotemType As Byte
        TOTEM_PASSIVE = 0
        TOTEM_ACTIVE = 1
        TOTEM_STATUE = 2
    End Enum

    Public Enum MailResult
        MAIL_SENT = 0
        MAIL_MONEY_REMOVED = 1
        MAIL_ITEM_REMOVED = 2
        MAIL_RETURNED = 3
        MAIL_DELETED = 4
        MAIL_MADE_PERMANENT = 5
    End Enum

    Public Enum MailSentError
        NO_ERROR = 0
        BAG_FULL = 1
        CANNOT_SEND_TO_SELF = 2
        NOT_ENOUGHT_MONEY = 3
        CHARACTER_NOT_FOUND = 4
        NOT_YOUR_ALLIANCE = 5
        INTERNAL_ERROR = 6
    End Enum

    Public Enum MailReadInfo As Byte
        Unread = 0
        Read = 1
        Auction = 4
        COD = 8
    End Enum

    Public Enum MailTypeInfo As Byte
        NORMAL = 0
        GMMAIL = 1
        AUCTION = 2
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

    Public Enum AuraAction As Byte
        AURA_ADD
        AURA_UPDATE
        AURA_REMOVE
        AURA_REMOVEBYDURATION
    End Enum

    Public Enum WeatherSounds As Integer
        WEATHER_SOUND_NOSOUND = 0
        WEATHER_SOUND_RAINLIGHT = 8533
        WEATHER_SOUND_RAINMEDIUM = 8534
        WEATHER_SOUND_RAINHEAVY = 8535
        WEATHER_SOUND_SNOWLIGHT = 8536
        WEATHER_SOUND_SNOWMEDIUM = 8537
        WEATHER_SOUND_SNOWHEAVY = 8538
        WEATHER_SOUND_SANDSTORMLIGHT = 8556
        WEATHER_SOUND_SANDSTORMMEDIUM = 8557
        WEATHER_SOUND_SANDSTORMHEAVY = 8558
    End Enum

    Public Enum WeatherType As Integer
        WEATHER_FINE = 0
        WEATHER_RAIN = 1
        WEATHER_SNOW = 2
        WEATHER_SANDSTORM = 3
    End Enum


End Module
