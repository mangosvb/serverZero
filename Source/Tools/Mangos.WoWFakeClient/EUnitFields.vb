﻿'
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

Public Enum EUnitFields
    UNIT_FIELD_CHARM = EObjectFields.OBJECT_END + &H0                             ' 0x006 - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_SUMMON = EObjectFields.OBJECT_END + &H2                            ' 0x008 - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_CHARMEDBY = EObjectFields.OBJECT_END + &H4                         ' 0x00A - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_SUMMONEDBY = EObjectFields.OBJECT_END + &H6                        ' 0x00C - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_CREATEDBY = EObjectFields.OBJECT_END + &H8                         ' 0x00E - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_TARGET = EObjectFields.OBJECT_END + &HA                            ' 0x010 - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_PERSUADED = EObjectFields.OBJECT_END + &HC                         ' 0x012 - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_CHANNEL_OBJECT = EObjectFields.OBJECT_END + &HE                    ' 0x014 - Size: 2 - Type: GUID - Flags: PUBLIC
    UNIT_FIELD_HEALTH = EObjectFields.OBJECT_END + &H10                           ' 0x016 - Size: 1 - Type: INT - Flags: DYNAMIC
    UNIT_FIELD_POWER1 = EObjectFields.OBJECT_END + &H11                           ' 0x017 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_POWER2 = EObjectFields.OBJECT_END + &H12                           ' 0x018 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_POWER3 = EObjectFields.OBJECT_END + &H13                           ' 0x019 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_POWER4 = EObjectFields.OBJECT_END + &H14                           ' 0x01A - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_POWER5 = EObjectFields.OBJECT_END + &H15                           ' 0x01B - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MAXHEALTH = EObjectFields.OBJECT_END + &H16                        ' 0x01C - Size: 1 - Type: INT - Flags: DYNAMIC
    UNIT_FIELD_MAXPOWER1 = EObjectFields.OBJECT_END + &H17                        ' 0x01D - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MAXPOWER2 = EObjectFields.OBJECT_END + &H18                        ' 0x01E - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MAXPOWER3 = EObjectFields.OBJECT_END + &H19                        ' 0x01F - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MAXPOWER4 = EObjectFields.OBJECT_END + &H1A                        ' 0x020 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MAXPOWER5 = EObjectFields.OBJECT_END + &H1B                        ' 0x021 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_LEVEL = EObjectFields.OBJECT_END + &H1C                            ' 0x022 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_FACTIONTEMPLATE = EObjectFields.OBJECT_END + &H1D                  ' 0x023 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_BYTES_0 = EObjectFields.OBJECT_END + &H1E                          ' 0x024 - Size: 1 - Type: BYTES - Flags: PUBLIC
    UNIT_VIRTUAL_ITEM_SLOT_DISPLAY = EObjectFields.OBJECT_END + &H1F              ' 0x025 - Size: 3 - Type: INT - Flags: PUBLIC
    UNIT_VIRTUAL_ITEM_INFO = EObjectFields.OBJECT_END + &H22                      ' 0x028 - Size: 6 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_FLAGS = EObjectFields.OBJECT_END + &H28                            ' 0x02E - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_AURA = EObjectFields.OBJECT_END + &H29                             ' 0x02F - Size: 48 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_AURAFLAGS = EObjectFields.OBJECT_END + &H59                        ' 0x05F - Size: 6 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_AURALEVELS = EObjectFields.OBJECT_END + &H5F                       ' 0x065 - Size: 12 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_AURAAPPLICATIONS = EObjectFields.OBJECT_END + &H6B                 ' 0x071 - Size: 12 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_AURASTATE = EObjectFields.OBJECT_END + &H77                        ' 0x07D - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_BASEATTACKTIME = EObjectFields.OBJECT_END + &H78                   ' 0x07E - Size: 2 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_OFFHANDATTACKTIME = EObjectFields.OBJECT_END + &H79
    UNIT_FIELD_RANGEDATTACKTIME = EObjectFields.OBJECT_END + &H7A                 ' 0x080 - Size: 1 - Type: INT - Flags: PRIVATE
    UNIT_FIELD_BOUNDINGRADIUS = EObjectFields.OBJECT_END + &H7B                   ' 0x081 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    UNIT_FIELD_COMBATREACH = EObjectFields.OBJECT_END + &H7C                      ' 0x082 - Size: 1 - Type: FLOAT - Flags: PUBLIC
    UNIT_FIELD_DISPLAYID = EObjectFields.OBJECT_END + &H7D                        ' 0x083 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_NATIVEDISPLAYID = EObjectFields.OBJECT_END + &H7E                  ' 0x084 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MOUNTDISPLAYID = EObjectFields.OBJECT_END + &H7F                   ' 0x085 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_MINDAMAGE = EObjectFields.OBJECT_END + &H80                        ' 0x086 - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY + UNK3
    UNIT_FIELD_MAXDAMAGE = EObjectFields.OBJECT_END + &H81                        ' 0x087 - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY + UNK3
    UNIT_FIELD_MINOFFHANDDAMAGE = EObjectFields.OBJECT_END + &H82                 ' 0x088 - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY + UNK3
    UNIT_FIELD_MAXOFFHANDDAMAGE = EObjectFields.OBJECT_END + &H83                 ' 0x089 - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY + UNK3
    UNIT_FIELD_BYTES_1 = EObjectFields.OBJECT_END + &H84                          ' 0x08A - Size: 1 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_PETNUMBER = EObjectFields.OBJECT_END + &H85                        ' 0x08B - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_PET_NAME_TIMESTAMP = EObjectFields.OBJECT_END + &H86               ' 0x08C - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_FIELD_PETEXPERIENCE = EObjectFields.OBJECT_END + &H87                    ' 0x08D - Size: 1 - Type: INT - Flags: OWNER_ONLY
    UNIT_FIELD_PETNEXTLEVELEXP = EObjectFields.OBJECT_END + &H88                  ' 0x08E - Size: 1 - Type: INT - Flags: OWNER_ONLY
    UNIT_DYNAMIC_FLAGS = EObjectFields.OBJECT_END + &H89                          ' 0x08F - Size: 1 - Type: INT - Flags: DYNAMIC
    UNIT_CHANNEL_SPELL = EObjectFields.OBJECT_END + &H8A                          ' 0x090 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_MOD_CAST_SPEED = EObjectFields.OBJECT_END + &H8B                         ' 0x091 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_CREATED_BY_SPELL = EObjectFields.OBJECT_END + &H8C                       ' 0x092 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_NPC_FLAGS = EObjectFields.OBJECT_END + &H8D                              ' 0x093 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_NPC_EMOTESTATE = EObjectFields.OBJECT_END + &H8E                         ' 0x094 - Size: 1 - Type: INT - Flags: PUBLIC
    UNIT_TRAINING_POINTS = EObjectFields.OBJECT_END + &H8F                        ' 0x095 - Size: 1 - Type: TWO_SHORT - Flags: OWNER_ONLY
    UNIT_FIELD_STAT0 = EObjectFields.OBJECT_END + &H90                            ' 0x096 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_STAT1 = EObjectFields.OBJECT_END + &H91                            ' 0x097 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_STAT2 = EObjectFields.OBJECT_END + &H92                            ' 0x098 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_STAT3 = EObjectFields.OBJECT_END + &H93                            ' 0x099 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_STAT4 = EObjectFields.OBJECT_END + &H94                            ' 0x09A - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_RESISTANCES = EObjectFields.OBJECT_END + &H95                      ' 0x09B - Size: 7 - Type: INT - Flags: PRIVATE + OWNER_ONLY + UNK3
    UNIT_FIELD_BASE_MANA = EObjectFields.OBJECT_END + &H9C                        ' 0x0A2 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_BASE_HEALTH = EObjectFields.OBJECT_END + &H9D                      ' 0x0A3 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_BYTES_2 = EObjectFields.OBJECT_END + &H9E                          ' 0x0A4 - Size: 1 - Type: BYTES - Flags: PUBLIC
    UNIT_FIELD_ATTACK_POWER = EObjectFields.OBJECT_END + &H9F                     ' 0x0A5 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_ATTACK_POWER_MODS = EObjectFields.OBJECT_END + &HA0                ' 0x0A6 - Size: 1 - Type: TWO_SHORT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_ATTACK_POWER_MULTIPLIER = EObjectFields.OBJECT_END + &HA1          ' 0x0A7 - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_RANGED_ATTACK_POWER = EObjectFields.OBJECT_END + &HA2              ' 0x0A8 - Size: 1 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_RANGED_ATTACK_POWER_MODS = EObjectFields.OBJECT_END + &HA3         ' 0x0A9 - Size: 1 - Type: TWO_SHORT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER = EObjectFields.OBJECT_END + &HA4   ' 0x0AA - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_MINRANGEDDAMAGE = EObjectFields.OBJECT_END + &HA5                  ' 0x0AB - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_MAXRANGEDDAMAGE = EObjectFields.OBJECT_END + &HA6                  ' 0x0AC - Size: 1 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_POWER_COST_MODIFIER = EObjectFields.OBJECT_END + &HA7              ' 0x0AD - Size: 7 - Type: INT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_POWER_COST_MULTIPLIER = EObjectFields.OBJECT_END + &HAE            ' 0x0B4 - Size: 7 - Type: FLOAT - Flags: PRIVATE + OWNER_ONLY
    UNIT_FIELD_PADDING = EObjectFields.OBJECT_END + &HB5                          ' 0x0BB - Size: 1 - Type: INT - Flags: NONE
    UNIT_END = EObjectFields.OBJECT_END + &HB6                                    ' 0x0BC

    UNIT_FIELD_STRENGTH = UNIT_FIELD_STAT0
    UNIT_FIELD_AGILITY = UNIT_FIELD_STAT1
    UNIT_FIELD_STAMINA = UNIT_FIELD_STAT2
    UNIT_FIELD_SPIRIT = UNIT_FIELD_STAT3
    UNIT_FIELD_INTELLECT = UNIT_FIELD_STAT4
End Enum