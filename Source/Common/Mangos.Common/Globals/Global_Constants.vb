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

Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals

Namespace Global
    Public Class Global_Constants
        Public ReadOnly RevisionDbCharactersVersion As Integer = 21
        Public ReadOnly RevisionDbCharactersStructure As Integer = 0
        Public ReadOnly RevisionDbCharactersContent As Integer = 0

        Public ReadOnly RevisionDbMangosVersion As Integer = 21
        Public ReadOnly RevisionDbMangosStructure As Integer = 1
        Public ReadOnly RevisionDbMangosContent As Integer = 0

        Public ReadOnly RevisionDbMangosVersion As Integer = 21
        Public ReadOnly RevisionDbMangosStructure As Integer = 3
        Public ReadOnly RevisionDbMangosContent As Integer = 1

        Public ReadOnly GROUP_SUBGROUPSIZE As Integer = 5  '(MAX_RAID_SIZE / MAX_GROUP_SIZE)
        Public ReadOnly GROUP_SIZE As Integer = 5          'Normal Group Size/More then 5, it's a raid group
        Public ReadOnly GROUP_RAIDSIZE As Integer = 40     'Max Raid Size

        Public ReadOnly GUID_ITEM As ULong = &H4000000000000000UL
        Public ReadOnly GUID_CONTAINER As ULong = &H4000000000000000UL
        Public ReadOnly GUID_PLAYER As ULong = &H0UL
        Public ReadOnly GUID_GAMEOBJECT As ULong = &HF110000000000000UL
        Public ReadOnly GUID_TRANSPORT As ULong = &HF120000000000000UL
        Public ReadOnly GUID_UNIT As ULong = &HF130000000000000UL
        Public ReadOnly GUID_PET As ULong = &HF140000000000000UL
        Public ReadOnly GUID_DYNAMICOBJECT As ULong = &HF100000000000000UL
        Public ReadOnly GUID_CORPSE As ULong = &HF101000000000000UL
        Public ReadOnly GUID_MO_TRANSPORT As ULong = &H1FC0000000000000UL
        Public ReadOnly GUID_MASK_LOW As UInteger = &HFFFFFFFFUI
        Public ReadOnly GUID_MASK_HIGH As ULong = &HFFFFFFFF00000000UL

        Public ReadOnly DEFAULT_DISTANCE_VISIBLE As Single = 155.8
        Public ReadOnly DEFAULT_DISTANCE_DETECTION As Single = 7

        'TODO: Is this correct? The amount of time since last pvp action until you go out of combat again
        Public ReadOnly DEFAULT_PVP_COMBAT_TIME As Integer = 6000 ' 6 seconds

        Public ReadOnly DEFAULT_LOCK_TIMEOUT As Integer = 2000
        Public ReadOnly DEFAULT_INSTANCE_EXPIRE_TIME As Integer = 3600              '1 hour
        Public ReadOnly DEFAULT_BATTLEFIELD_EXPIRE_TIME As Integer = 3600 * 24      '24 hours

        Public SERVER_CONFIG_DISABLED_CLASSES() As Boolean = {False, False, False, False, False, False, False, False, False, True, False}
        Public SERVER_CONFIG_DISABLED_RACES() As Boolean = {False, False, False, False, False, False, False, False, True, False, False}

        Public ReadOnly UNIT_NORMAL_WALK_SPEED As Single = 2.5F
        Public ReadOnly UNIT_NORMAL_RUN_SPEED As Single = 7.0F
        Public ReadOnly UNIT_NORMAL_SWIM_SPEED As Single = 4.722222F
        Public ReadOnly UNIT_NORMAL_SWIM_BACK_SPEED As Single = 2.5F
        Public ReadOnly UNIT_NORMAL_WALK_BACK_SPEED As Single = 4.5F
        Public ReadOnly UNIT_NORMAL_TURN_RATE As Single = Math.PI
        Public ReadOnly UNIT_NORMAL_TAXI_SPEED As Single = 32.0F

        Public ReadOnly PLAYER_VISIBLE_ITEM_SIZE As Integer = 12
        Public ReadOnly PLAYER_SKILL_INFO_SIZE As Integer = 384 - 1
        Public ReadOnly PLAYER_EXPLORED_ZONES_SIZE As Integer = 64 - 1

        Public ReadOnly FIELD_MASK_SIZE_PLAYER As Integer = ((EPlayerFields.PLAYER_END + 32) \ 32) * 32
        Public ReadOnly FIELD_MASK_SIZE_UNIT As Integer = ((EUnitFields.UNIT_END + 32) \ 32) * 32
        Public ReadOnly FIELD_MASK_SIZE_GAMEOBJECT As Integer = ((EGameObjectFields.GAMEOBJECT_END + 32) \ 32) * 32
        Public ReadOnly FIELD_MASK_SIZE_DYNAMICOBJECT As Integer = ((EDynamicObjectFields.DYNAMICOBJECT_END + 32) \ 32) * 32
        Public ReadOnly FIELD_MASK_SIZE_ITEM As Integer = ((EContainerFields.CONTAINER_END + 32) \ 32) * 32
        Public ReadOnly FIELD_MASK_SIZE_CORPSE As Integer = ((ECorpseFields.CORPSE_END + 32) \ 32) * 32

        Public ReadOnly WorldServerStatus() As String = {"ONLINE/G", "ONLINE/R", "OFFLINE "}
        'Public ConsoleColor As New ConsoleColor
        '1.12.1 - 5875
        '1.12.2 - 6005
        '1.12.3 - 6141

        'New Auto Detection Build
        Public ReadOnly Required_Build_1_12_1 As Integer = 5875
        Public ReadOnly Required_Build_1_12_2 As Integer = 6005
        Public ReadOnly Required_Build_1_12_3 As Integer = 6141
        Public ReadOnly ConnectionSleepTime As Integer = 100

        Public ReadOnly GUILD_RANK_MAX As Integer = 9 'Max Ranks Per Guild
        Public ReadOnly GUILD_RANK_MIN As Integer = 5 'Min Ranks Per Guild

        Public ReadOnly GOSSIP_TEXT_BANK As String = "The Bank"
        Public ReadOnly GOSSIP_TEXT_WINDRIDER As String = "Wind rider master"
        Public ReadOnly GOSSIP_TEXT_GRYPHON As String = "Gryphon Master"
        Public ReadOnly GOSSIP_TEXT_BATHANDLER As String = "Bat Handler"
        Public ReadOnly GOSSIP_TEXT_HIPPOGRYPH As String = "Hippogryph Master"
        Public ReadOnly GOSSIP_TEXT_FLIGHTMASTER As String = "Flight Master"
        Public ReadOnly GOSSIP_TEXT_AUCTIONHOUSE As String = "Auction House"
        Public ReadOnly GOSSIP_TEXT_GUILDMASTER As String = "Guild Master"
        Public ReadOnly GOSSIP_TEXT_INN As String = "The Inn"
        Public ReadOnly GOSSIP_TEXT_MAILBOX As String = "Mailbox"
        Public ReadOnly GOSSIP_TEXT_STABLEMASTER As String = "Stable Master"
        Public ReadOnly GOSSIP_TEXT_WEAPONMASTER As String = "Weapons Trainer"
        Public ReadOnly GOSSIP_TEXT_BATTLEMASTER As String = "Battlemaster"
        Public ReadOnly GOSSIP_TEXT_CLASSTRAINER As String = "Class Trainer"
        Public ReadOnly GOSSIP_TEXT_PROFTRAINER As String = "Profession Trainer"
        Public ReadOnly GOSSIP_TEXT_OFFICERS As String = "The officers` lounge"

        Public ReadOnly GOSSIP_TEXT_ALTERACVALLEY As String = "Alterac Valley"
        Public ReadOnly GOSSIP_TEXT_ARATHIBASIN As String = "Arathi Basin"
        Public ReadOnly GOSSIP_TEXT_WARSONGULCH As String = "Warsong Gulch"

        Public ReadOnly GOSSIP_TEXT_IRONFORGE_BANK As String = "Bank of Ironforge"
        Public ReadOnly GOSSIP_TEXT_STORMWIND_BANK As String = "Bank of Stormwind"
        Public ReadOnly GOSSIP_TEXT_DEEPRUNTRAM As String = "Deeprun Tram"
        Public ReadOnly GOSSIP_TEXT_ZEPPLINMASTER As String = "Zeppelin master"
        Public ReadOnly GOSSIP_TEXT_FERRY As String = "Rut'theran Ferry"

        Public ReadOnly GOSSIP_TEXT_DRUID As String = "Druid"
        Public ReadOnly GOSSIP_TEXT_HUNTER As String = "Hunter"
        Public ReadOnly GOSSIP_TEXT_PRIEST As String = "Priest"
        Public ReadOnly GOSSIP_TEXT_ROGUE As String = "Rogue"
        Public ReadOnly GOSSIP_TEXT_WARRIOR As String = "Warrior"
        Public ReadOnly GOSSIP_TEXT_PALADIN As String = "Paladin"
        Public ReadOnly GOSSIP_TEXT_SHAMAN As String = "Shaman"
        Public ReadOnly GOSSIP_TEXT_MAGE As String = "Mage"
        Public ReadOnly GOSSIP_TEXT_WARLOCK As String = "Warlock"

        Public ReadOnly GOSSIP_TEXT_ALCHEMY As String = "Alchemy"
        Public ReadOnly GOSSIP_TEXT_BLACKSMITHING As String = "Blacksmithing"
        Public ReadOnly GOSSIP_TEXT_COOKING As String = "Cooking"
        Public ReadOnly GOSSIP_TEXT_ENCHANTING As String = "Enchanting"
        Public ReadOnly GOSSIP_TEXT_ENGINEERING As String = "Engineering"
        Public ReadOnly GOSSIP_TEXT_FIRSTAID As String = "First Aid"
        Public ReadOnly GOSSIP_TEXT_HERBALISM As String = "Herbalism"
        Public ReadOnly GOSSIP_TEXT_LEATHERWORKING As String = "Leatherworking"
        Public ReadOnly GOSSIP_TEXT_POISONS As String = "Poisons"
        Public ReadOnly GOSSIP_TEXT_TAILORING As String = "Tailoring"
        Public ReadOnly GOSSIP_TEXT_MINING As String = "Mining"
        Public ReadOnly GOSSIP_TEXT_FISHING As String = "Fishing"
        Public ReadOnly GOSSIP_TEXT_SKINNING As String = "Skinning"

        'VMAPS
        Public ReadOnly VMAP_MAGIC As String = "VMAP_2.0"
        Public ReadOnly VMAP_MAX_CAN_FALL_DISTANCE As Single = 10.0F
        Public ReadOnly VMAP_INVALID_HEIGHT As Single = -100000.0F 'for check
        Public ReadOnly VMAP_INVALID_HEIGHT_VALUE As Single = -200000.0F 'real assigned value in unknown height case

        'MAPS
        Public ReadOnly SIZE As Single = 533.3333F
        Public ReadOnly RESOLUTION_WATER As Integer = 128 - 1
        Public ReadOnly RESOLUTION_FLAGS As Integer = 16 - 1
        Public ReadOnly RESOLUTION_TERRAIN As Integer = 16 - 1

        Public ReadOnly groundFlagsMask As Integer = &HFFFFFFFF And Not (MovementFlags.MOVEMENTFLAG_LEFT Or MovementFlags.MOVEMENTFLAG_RIGHT Or MovementFlags.MOVEMENTFLAG_BACKWARD Or MovementFlags.MOVEMENTFLAG_FORWARD Or MovementFlags.MOVEMENTFLAG_WALK)
        Public ReadOnly movementFlagsMask As Integer = MovementFlags.MOVEMENTFLAG_FORWARD Or MovementFlags.MOVEMENTFLAG_BACKWARD Or MovementFlags.MOVEMENTFLAG_STRAFE_LEFT Or
                                                    MovementFlags.MOVEMENTFLAG_STRAFE_RIGHT Or MovementFlags.MOVEMENTFLAG_PITCH_UP Or MovementFlags.MOVEMENTFLAG_PITCH_DOWN Or MovementFlags.MOVEMENTFLAG_JUMPING Or
                                                    MovementFlags.MOVEMENTFLAG_FALLING Or MovementFlags.MOVEMENTFLAG_SWIMMING Or MovementFlags.MOVEMENTFLAG_SPLINE
        Public ReadOnly TurningFlagsMask As Integer = MovementFlags.MOVEMENTFLAG_LEFT Or MovementFlags.MOVEMENTFLAG_RIGHT
        Public ReadOnly movementOrTurningFlagsMask As Integer = movementFlagsMask Or TurningFlagsMask

        Public ReadOnly ITEM_SLOT_NULL As Byte = 255
        Public ReadOnly ITEM_BAG_NULL As Long = -1

        Public ReadOnly PETITION_GUILD_PRICE As Integer = 1000
        Public ReadOnly PETITION_GUILD As Integer = 5863       'Guild Charter, ItemFlags = &H2000
        Public ReadOnly GUILD_TABARD_ITEM As Integer = 5976

        Public ReadOnly CREATURE_MAX_SPELLS As Integer = 4
        Public ReadOnly MAX_OWNER_DIS As Integer = 100

        Public ReadOnly SPELL_DURATION_INFINITE As Integer = -1
        Public ReadOnly MAX_AURA_EFFECTs_VISIBLE As Integer = 48                  '48 AuraSlots (32 buff, 16 debuff)
        Public ReadOnly MAX_AURA_EFFECTs_PASSIVE As Integer = 192
        Public ReadOnly MAX_AURA_EFFECTs As Integer = MAX_AURA_EFFECTs_VISIBLE + MAX_AURA_EFFECTs_PASSIVE
        Public ReadOnly MAX_AURA_EFFECT_FLAGs As Integer = MAX_AURA_EFFECTs_VISIBLE \ 8
        Public ReadOnly MAX_AURA_EFFECT_LEVELSs As Integer = MAX_AURA_EFFECTs_VISIBLE \ 4
        Public ReadOnly MAX_POSITIVE_AURA_EFFECTs As Integer = 32
        Public ReadOnly MAX_NEGATIVE_AURA_EFFECTs As Integer = MAX_AURA_EFFECTs_VISIBLE - MAX_POSITIVE_AURA_EFFECTs

        Public ReadOnly UINT32_MAX As Integer = &HFFFFFFFF
        Public ReadOnly UINT32_MIN As Integer = 0

        Public ReadOnly MpqId As Long = 441536589
        Public ReadOnly MpqHeaderSize As Long = 32
    End Class
End Namespace