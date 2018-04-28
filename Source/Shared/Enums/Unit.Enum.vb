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

Public Module UnitEnum

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

    <Flags()>
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

    <Flags()>
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

End Module
