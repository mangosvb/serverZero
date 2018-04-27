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

    Public Enum KeyRingSlots As Byte  '32 Slots?
        KEYRING_SLOT_START = 81
        KEYRING_SLOT_1 = 81
        KEYRING_SLOT_2 = 82
        KEYRING_SLOT_31 = 112
        KEYRING_SLOT_32 = 113
        KEYRING_SLOT_END = 113
    End Enum

End Module
