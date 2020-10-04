'
' Copyright (C) 2013-2019 getMaNGOS <https://getmangos.eu>
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

Public Module PlayerEnum

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

    Public Enum KeyRingSlots As Byte  '32 Slots?
        KEYRING_SLOT_START = 81
        KEYRING_SLOT_1 = 81
        KEYRING_SLOT_2 = 82
        KEYRING_SLOT_31 = 112
        KEYRING_SLOT_32 = 113
        KEYRING_SLOT_END = 113
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

    <Flags()>
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

    <Flags()>
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

    Public Enum Attributes
        Agility = 3
        Health = 1
        Iq = 5
        Mana = 0
        Spirit = 6
        Stamina = 7
        Strenght = 4
    End Enum

End Module
