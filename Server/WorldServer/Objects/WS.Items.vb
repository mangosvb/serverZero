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
Imports System.Runtime.CompilerServices

Public Module WS_Items

#Region "WS.Items.Constants"

    Private ReadOnly ItemWeaponSkills() As Integer = New Integer() {SKILL_IDs.SKILL_AXES,
                                                                    SKILL_IDs.SKILL_TWO_HANDED_AXES,
                                                                    SKILL_IDs.SKILL_BOWS,
                                                                    SKILL_IDs.SKILL_GUNS,
                                                                    SKILL_IDs.SKILL_MACES,
                                                                    SKILL_IDs.SKILL_TWO_HANDED_MACES,
                                                                    SKILL_IDs.SKILL_POLEARMS,
                                                                    SKILL_IDs.SKILL_SWORDS,
                                                                    SKILL_IDs.SKILL_TWO_HANDED_SWORDS, 0,
                                                                    SKILL_IDs.SKILL_STAVES, 0, 0, 0, 0,
                                                                    SKILL_IDs.SKILL_DAGGERS,
                                                                    SKILL_IDs.SKILL_THROWN,
                                                                    SKILL_IDs.SKILL_SPEARS,
                                                                    SKILL_IDs.SKILL_CROSSBOWS,
                                                                    SKILL_IDs.SKILL_WANDS,
                                                                    SKILL_IDs.SKILL_FISHING}

    Private ReadOnly _
        ItemArmorSkills() As Integer = New Integer() _
        {0, SKILL_IDs.SKILL_CLOTH, SKILL_IDs.SKILL_LEATHER, SKILL_IDs.SKILL_MAIL, SKILL_IDs.SKILL_PLATE_MAIL, 0,
         SKILL_IDs.SKILL_SHIELD, 0, 0, 0}

#End Region

#Region "WS.Items.TypeDef"


    'WARNING: Use only with ITEMDatabase()
    Public Class ItemInfo
        Implements IDisposable
        Private _found As Boolean = False

        Private Sub New()
            Damage(0) = New TDamage
            Damage(1) = New TDamage
            Damage(2) = New TDamage
            Damage(3) = New TDamage
            Damage(4) = New TDamage
            Spells(0) = New TItemSpellInfo
            Spells(1) = New TItemSpellInfo
            Spells(2) = New TItemSpellInfo
            Spells(3) = New TItemSpellInfo
            Spells(4) = New TItemSpellInfo
        End Sub

        Public Sub New(ByVal itemId As Integer)
            Me.New()
            Id = itemId
            ITEMDatabase.Add(Id, Me)

            'DONE: Load Item Data from MySQL
            Dim mySqlQuery As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM item_template WHERE entry = {0};", itemId), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then
                Log.WriteLine(BaseWriter.LogType.FAILED,
                              "ItemID {0} not found in SQL database! Loading default ""Unknown Item"" info.", itemId)
                _found = False
                Exit Sub
            End If
            _found = True

            Model = mySqlQuery.Rows(0).Item("displayid")
            Name = mySqlQuery.Rows(0).Item("name")
            Quality = mySqlQuery.Rows(0).Item("quality") _
            '0=Grey-Poor 1=White-Common 2=Green-Uncommon 3=Blue-Rare 4=Purple-Epic 5=Orange-Legendary 6=Red-Artifact
            Material = mySqlQuery.Rows(0).Item("Material") _
            '-1=Consumables 1=Metal 2=Wood 3=Liquid 4=Jewelry 5=Chain 6=Plate 7=Cloth 8=Leather 
            Durability = mySqlQuery.Rows(0).Item("MaxDurability")
            Sheath = mySqlQuery.Rows(0).Item("sheath")
            Bonding = mySqlQuery.Rows(0).Item("bonding")
            BuyCount = mySqlQuery.Rows(0).Item("buycount")
            BuyPrice = mySqlQuery.Rows(0).Item("buyprice")
            SellPrice = mySqlQuery.Rows(0).Item("sellprice")

            'Item's Characteristics
            Id = mySqlQuery.Rows(0).Item("entry")
            Flags = mySqlQuery.Rows(0).Item("flags")
            ObjectClass = mySqlQuery.Rows(0).Item("class")
            SubClass = mySqlQuery.Rows(0).Item("subclass")
            InventoryType = mySqlQuery.Rows(0).Item("inventorytype")
            Level = mySqlQuery.Rows(0).Item("itemlevel")

            AvailableClasses = BitConverter.ToUInt32(BitConverter.GetBytes(mySqlQuery.Rows(0).Item("allowableclass")), 0)
            AvailableRaces = BitConverter.ToUInt32(BitConverter.GetBytes(mySqlQuery.Rows(0).Item("allowablerace")), 0)
            ReqLevel = mySqlQuery.Rows(0).Item("requiredlevel")
            ReqSkill = mySqlQuery.Rows(0).Item("RequiredSkill")
            ReqSkillRank = mySqlQuery.Rows(0).Item("RequiredSkillRank")
            'ReqSkillSubRank = MySQLQuery.Rows(0).Item("RequiredSkillSubRank")
            ReqSpell = mySqlQuery.Rows(0).Item("requiredspell")
            ReqFaction = mySqlQuery.Rows(0).Item("RequiredReputationFaction")
            ReqFactionLevel = mySqlQuery.Rows(0).Item("RequiredReputationRank")
            ReqHonorRank = mySqlQuery.Rows(0).Item("requiredhonorrank")
            ReqHonorRank2 = mySqlQuery.Rows(0).Item("RequiredCityRank")

            'Special items
            AmmoType = mySqlQuery.Rows(0).Item("ammo_type")
            PageText = mySqlQuery.Rows(0).Item("PageText")
            Stackable = mySqlQuery.Rows(0).Item("stackable")
            Unique = mySqlQuery.Rows(0).Item("maxcount")
            Description = mySqlQuery.Rows(0).Item("description")
            Block = mySqlQuery.Rows(0).Item("block")
            ItemSet = mySqlQuery.Rows(0).Item("itemset")
            PageMaterial = mySqlQuery.Rows(0).Item("PageMaterial") _
            'The background of the page window: 1=Parchment 2=Stone 3=Marble 4=Silver 5=Bronze                
            StartQuest = mySqlQuery.Rows(0).Item("startquest")
            ContainerSlots = mySqlQuery.Rows(0).Item("ContainerSlots")
            LanguageID = mySqlQuery.Rows(0).Item("LanguageID")
            BagFamily = mySqlQuery.Rows(0).Item("BagFamily")

            Delay = mySqlQuery.Rows(0).Item("delay")
            Range = mySqlQuery.Rows(0).Item("RangedModRange")

            Damage(0).Minimum = mySqlQuery.Rows(0).Item("dmg_min1")
            Damage(0).Maximum = mySqlQuery.Rows(0).Item("dmg_max1")
            Damage(0).Type = mySqlQuery.Rows(0).Item("dmg_type1")
            Damage(1).Minimum = mySqlQuery.Rows(0).Item("dmg_min2")
            Damage(1).Maximum = mySqlQuery.Rows(0).Item("dmg_max2")
            Damage(1).Type = mySqlQuery.Rows(0).Item("dmg_type2")
            Damage(2).Minimum = mySqlQuery.Rows(0).Item("dmg_min3")
            Damage(2).Maximum = mySqlQuery.Rows(0).Item("dmg_max3")
            Damage(2).Type = mySqlQuery.Rows(0).Item("dmg_type3")
            Damage(3).Minimum = mySqlQuery.Rows(0).Item("dmg_min4")
            Damage(3).Maximum = mySqlQuery.Rows(0).Item("dmg_max4")
            Damage(3).Type = mySqlQuery.Rows(0).Item("dmg_type4")
            Damage(4).Minimum = mySqlQuery.Rows(0).Item("dmg_min5")
            Damage(4).Maximum = mySqlQuery.Rows(0).Item("dmg_max5")
            Damage(4).Type = mySqlQuery.Rows(0).Item("dmg_type5")

            Resistances(DamageTypes.DMG_PHYSICAL) = mySqlQuery.Rows(0).Item("armor")        'Armor
            Resistances(DamageTypes.DMG_HOLY) = mySqlQuery.Rows(0).Item("holy_res")          'Holy
            Resistances(DamageTypes.DMG_FIRE) = mySqlQuery.Rows(0).Item("fire_res")          'Fire
            Resistances(DamageTypes.DMG_NATURE) = mySqlQuery.Rows(0).Item("nature_res")      'Nature
            Resistances(DamageTypes.DMG_FROST) = mySqlQuery.Rows(0).Item("frost_res")        'Frost
            Resistances(DamageTypes.DMG_SHADOW) = mySqlQuery.Rows(0).Item("shadow_res")      'Shadow
            Resistances(DamageTypes.DMG_ARCANE) = mySqlQuery.Rows(0).Item("arcane_res")      'Arcane

            Spells(0).SpellID = mySqlQuery.Rows(0).Item("spellid_1")
            Spells(0).SpellTrigger = mySqlQuery.Rows(0).Item("spelltrigger_1") _
            '0="Use:" 1="Equip:" 2="Chance on Hit:"
            Spells(0).SpellCharges = mySqlQuery.Rows(0).Item("spellcharges_1") _
            '0=Doesn't disappear after use -1=Disappears after use
            Spells(0).SpellCooldown = mySqlQuery.Rows(0).Item("spellcooldown_1")
            Spells(0).SpellCategory = mySqlQuery.Rows(0).Item("spellcategory_1")
            Spells(0).SpellCategoryCooldown = mySqlQuery.Rows(0).Item("spellcategorycooldown_1")
            Spells(1).SpellID = mySqlQuery.Rows(0).Item("spellid_2")
            Spells(1).SpellTrigger = mySqlQuery.Rows(0).Item("spelltrigger_2") _
            '0="Use:" 1="Equip:" 2="Chance on Hit:"
            Spells(1).SpellCharges = mySqlQuery.Rows(0).Item("spellcharges_2") _
            '0=Doesn't disappear after use -1=Disappears after use
            Spells(1).SpellCooldown = mySqlQuery.Rows(0).Item("spellcooldown_2")
            Spells(1).SpellCategory = mySqlQuery.Rows(0).Item("spellcategory_2")
            Spells(1).SpellCategoryCooldown = mySqlQuery.Rows(0).Item("spellcategorycooldown_2")
            Spells(2).SpellID = mySqlQuery.Rows(0).Item("spellid_3")
            Spells(2).SpellTrigger = mySqlQuery.Rows(0).Item("spelltrigger_3") _
            '0="Use:" 1="Equip:" 2="Chance on Hit:"
            Spells(2).SpellCharges = mySqlQuery.Rows(0).Item("spellcharges_3") _
            '0=Doesn't disappear after use -1=Disappears after use
            Spells(2).SpellCooldown = mySqlQuery.Rows(0).Item("spellcooldown_3")
            Spells(2).SpellCategory = mySqlQuery.Rows(0).Item("spellcategory_3")
            Spells(2).SpellCategoryCooldown = mySqlQuery.Rows(0).Item("spellcategorycooldown_3")
            Spells(3).SpellID = mySqlQuery.Rows(0).Item("spellid_4")
            Spells(3).SpellTrigger = mySqlQuery.Rows(0).Item("spelltrigger_4") _
            '0="Use:" 1="Equip:" 2="Chance on Hit:"
            Spells(3).SpellCharges = mySqlQuery.Rows(0).Item("spellcharges_4") _
            '0=Doesn't disappear after use -1=Disappears after use
            Spells(3).SpellCooldown = mySqlQuery.Rows(0).Item("spellcooldown_4")
            Spells(3).SpellCategory = mySqlQuery.Rows(0).Item("spellcategory_4")
            Spells(3).SpellCategoryCooldown = mySqlQuery.Rows(0).Item("spellcategorycooldown_4")
            Spells(4).SpellID = mySqlQuery.Rows(0).Item("spellid_5")
            Spells(4).SpellTrigger = mySqlQuery.Rows(0).Item("spelltrigger_5") _
            '0="Use:" 1="Equip:" 2="Chance on Hit:"
            Spells(4).SpellCharges = mySqlQuery.Rows(0).Item("spellcharges_5") _
            '0=Doesn't disappear after use -1=Disappears after use
            Spells(4).SpellCooldown = mySqlQuery.Rows(0).Item("spellcooldown_5")
            Spells(4).SpellCategory = mySqlQuery.Rows(0).Item("spellcategory_5")
            Spells(4).SpellCategoryCooldown = mySqlQuery.Rows(0).Item("spellcategorycooldown_5")

            'Unknown
            LockID = mySqlQuery.Rows(0).Item("lockid")
            'Extra = MySQLQuery.Rows(0).Item("Extra")

            ItemBonusStatType(0) = mySqlQuery.Rows(0).Item("stat_type1")
            ItemBonusStatValue(0) = mySqlQuery.Rows(0).Item("stat_value1")
            ItemBonusStatType(1) = mySqlQuery.Rows(0).Item("stat_type2")
            ItemBonusStatValue(1) = mySqlQuery.Rows(0).Item("stat_value2")
            ItemBonusStatType(2) = mySqlQuery.Rows(0).Item("stat_type3")
            ItemBonusStatValue(2) = mySqlQuery.Rows(0).Item("stat_value3")
            ItemBonusStatType(3) = mySqlQuery.Rows(0).Item("stat_type4")
            ItemBonusStatValue(3) = mySqlQuery.Rows(0).Item("stat_value4")
            ItemBonusStatType(4) = mySqlQuery.Rows(0).Item("stat_type5")
            ItemBonusStatValue(4) = mySqlQuery.Rows(0).Item("stat_value5")
            ItemBonusStatType(5) = mySqlQuery.Rows(0).Item("stat_type6")
            ItemBonusStatValue(5) = mySqlQuery.Rows(0).Item("stat_value6")
            ItemBonusStatType(6) = mySqlQuery.Rows(0).Item("stat_type7")
            ItemBonusStatValue(6) = mySqlQuery.Rows(0).Item("stat_value7")
            ItemBonusStatType(7) = mySqlQuery.Rows(0).Item("stat_type8")
            ItemBonusStatValue(7) = mySqlQuery.Rows(0).Item("stat_value8")
            ItemBonusStatType(8) = mySqlQuery.Rows(0).Item("stat_type9")
            ItemBonusStatValue(8) = mySqlQuery.Rows(0).Item("stat_value9")
            ItemBonusStatType(9) = mySqlQuery.Rows(0).Item("stat_type10")
            ItemBonusStatValue(9) = mySqlQuery.Rows(0).Item("stat_value10")

            'RandomProp = MySQLQuery.Rows(0).Item("randomprop")
            'RandomSuffix = MySQLQuery.Rows(0).Item("randomsuffix") ' Not sure about this one
            ZoneNameID = mySqlQuery.Rows(0).Item("area")
            'MapID = MySQLQuery.Rows(0).Item("mapid")
            'TotemCategory = MySQLQuery.Rows(0).Item("TotemCategory")
            _reqDisenchantSkill = mySqlQuery.Rows(0).Item("DisenchantID")
            'ArmorDamageModifier = MySQLQuery.Rows(0).Item("armorDamageModifier")
            'ExistingDuration = MySQLQuery.Rows(0).Item("ExistingDuration")

            'DONE: Internal database fixers
            If Stackable = 0 Then Stackable = 1
        End Sub

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                ITEMDatabase.Remove(Id)
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

        'Item's visuals
        Public ReadOnly Model As Integer = 0
        Public ReadOnly Name As String = "Unknown Item"
        Public ReadOnly Quality As Integer = 0
        Public ReadOnly Material As Integer = 0
        Public ReadOnly Durability As Integer = 0
        Public ReadOnly Sheath As SHEATHE_TYPE = 0
        Public ReadOnly Bonding As Integer = 0
        Public ReadOnly BuyCount As Integer = 0
        Public ReadOnly BuyPrice As Integer = 0
        Public ReadOnly SellPrice As Integer = 0

        'Item's Characteristics
        Public ReadOnly Id As Integer = 0
        Public ReadOnly Flags As Integer = 0
        Public ReadOnly ObjectClass As ITEM_CLASS = 0
        Public ReadOnly SubClass As ITEM_SUBCLASS = 0
        Public ReadOnly InventoryType As INVENTORY_TYPES = 0
        Public ReadOnly Level As Integer = 0

        Public ReadOnly AvailableClasses As UInteger = 0
        Public ReadOnly AvailableRaces As UInteger = 0
        Public ReadOnly ReqLevel As Integer = 0
        Public ReadOnly ReqSkill As Integer = 0
        Public ReadOnly ReqSkillRank As Integer = 0
        Public ReqSkillSubRank As Integer
        Public ReadOnly ReqSpell As Integer = 0
        Public ReadOnly ReqFaction As Integer = 0
        Public ReadOnly ReqFactionLevel As Integer = 0
        Public ReadOnly ReqHonorRank As Integer = 0
        Public ReadOnly ReqHonorRank2 As Integer = 0

        'Special items
        Public ReadOnly AmmoType As Integer = 0
        Public ReadOnly PageText As Integer = 0
        Public ReadOnly Stackable As Integer = 1
        Public ReadOnly Unique As Integer = 0
        Public ReadOnly Description As String = ""
        Public ReadOnly Block As Integer = 0
        Public ReadOnly ItemSet As Integer = 0
        Public ReadOnly PageMaterial As Integer = 0
        Public ReadOnly StartQuest As Integer = 0
        Public ReadOnly ContainerSlots As Integer = 0
        Public ReadOnly LanguageID As Integer = 0
        Public ReadOnly BagFamily As ITEM_BAG = 0

        'Item's bonuses
        Public ReadOnly Delay As Integer = 0
        Public ReadOnly Range As Single = 0
        Public ReadOnly Damage(4) As TDamage
        Public ReadOnly Resistances() As Integer = {0, 0, 0, 0, 0, 0, 0}
        Public ReadOnly ItemBonusStatType() As Integer = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Public ReadOnly ItemBonusStatValue() As Integer = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        'Item's Spells
        Public ReadOnly Spells(4) As TItemSpellInfo
        Private _reqDisenchantSkill As Integer = - 1
        Public ArmorDamageModifier As Single = 0
        Public ExistingDuration As Integer = 0

        'Other
        Public Unk2 As Integer = 0
        Public ReadOnly LockID As Integer = 0
        Public Extra As Integer = 0
        Public Area As Integer = 0
        Public ReadOnly ZoneNameID As Integer = 0
        Public MapID As Integer = 0
        Public TotemCategory As Integer = 0
        Public RandomProp As Integer = 0
        Public RandomSuffix As Integer = 0

        Public ReadOnly Property IsContainer() As Boolean
            Get
                If ContainerSlots > 0 Then Return True Else Return False
            End Get
        End Property

        Public ReadOnly Property GetSlots() As Byte()
            Get
                Select Case InventoryType
                    Case INVENTORY_TYPES.INVTYPE_HEAD
                        Return New Byte() {EQUIPMENT_SLOT_HEAD}
                    Case INVENTORY_TYPES.INVTYPE_NECK
                        Return New Byte() {EQUIPMENT_SLOT_NECK}
                    Case INVENTORY_TYPES.INVTYPE_SHOULDERS
                        Return New Byte() {EQUIPMENT_SLOT_SHOULDERS}
                    Case INVENTORY_TYPES.INVTYPE_BODY
                        Return New Byte() {EQUIPMENT_SLOT_BODY}
                    Case INVENTORY_TYPES.INVTYPE_CHEST
                        Return New Byte() {EQUIPMENT_SLOT_CHEST}
                    Case INVENTORY_TYPES.INVTYPE_ROBE
                        Return New Byte() {EQUIPMENT_SLOT_CHEST}
                    Case INVENTORY_TYPES.INVTYPE_WAIST
                        Return New Byte() {EQUIPMENT_SLOT_WAIST}
                    Case INVENTORY_TYPES.INVTYPE_LEGS
                        Return New Byte() {EQUIPMENT_SLOT_LEGS}
                    Case INVENTORY_TYPES.INVTYPE_FEET
                        Return New Byte() {EQUIPMENT_SLOT_FEET}
                    Case INVENTORY_TYPES.INVTYPE_WRISTS
                        Return New Byte() {EQUIPMENT_SLOT_WRISTS}
                    Case INVENTORY_TYPES.INVTYPE_HANDS
                        Return New Byte() {EQUIPMENT_SLOT_HANDS}
                    Case INVENTORY_TYPES.INVTYPE_FINGER
                        Return New Byte() {EQUIPMENT_SLOT_FINGER1, EQUIPMENT_SLOT_FINGER2}
                    Case INVENTORY_TYPES.INVTYPE_TRINKET
                        Return New Byte() {EQUIPMENT_SLOT_TRINKET1, EQUIPMENT_SLOT_TRINKET2}
                    Case INVENTORY_TYPES.INVTYPE_CLOAK
                        Return New Byte() {EQUIPMENT_SLOT_BACK}
                    Case INVENTORY_TYPES.INVTYPE_WEAPON
                        Return New Byte() {EQUIPMENT_SLOT_MAINHAND, EQUIPMENT_SLOT_OFFHAND}
                    Case INVENTORY_TYPES.INVTYPE_SHIELD
                        Return New Byte() {EQUIPMENT_SLOT_OFFHAND}
                    Case INVENTORY_TYPES.INVTYPE_RANGED
                        Return New Byte() {EQUIPMENT_SLOT_RANGED}
                    Case INVENTORY_TYPES.INVTYPE_TWOHAND_WEAPON
                        Return New Byte() {EQUIPMENT_SLOT_MAINHAND}
                    Case INVENTORY_TYPES.INVTYPE_TABARD
                        Return New Byte() {EQUIPMENT_SLOT_TABARD}
                    Case INVENTORY_TYPES.INVTYPE_WEAPONMAINHAND
                        Return New Byte() {EQUIPMENT_SLOT_MAINHAND}
                    Case INVENTORY_TYPES.INVTYPE_WEAPONOFFHAND
                        Return New Byte() {EQUIPMENT_SLOT_OFFHAND}
                    Case INVENTORY_TYPES.INVTYPE_HOLDABLE
                        Return New Byte() {EQUIPMENT_SLOT_OFFHAND}
                    Case INVENTORY_TYPES.INVTYPE_THROWN
                        Return New Byte() {EQUIPMENT_SLOT_RANGED}
                    Case INVENTORY_TYPES.INVTYPE_RANGEDRIGHT
                        Return New Byte() {EQUIPMENT_SLOT_RANGED}
                    Case INVENTORY_TYPES.INVTYPE_BAG
                        Return _
                            New Byte() _
                                {INVENTORY_SLOT_BAG_1, INVENTORY_SLOT_BAG_2, INVENTORY_SLOT_BAG_3, INVENTORY_SLOT_BAG_4}
                    Case INVENTORY_TYPES.INVTYPE_RELIC
                        Return New Byte() {EQUIPMENT_SLOT_RANGED}
                    Case Else
                        Return New Byte() {}
                End Select
            End Get
        End Property

        Public ReadOnly Property GetReqSkill() As Integer
            Get
                If ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then Return ItemWeaponSkills(SubClass)
                If ObjectClass = ITEM_CLASS.ITEM_CLASS_ARMOR Then Return ItemArmorSkills(SubClass)
                Return 0
            End Get
        End Property

        Public ReadOnly Property GetReqSpell() As Short
            Get
                Select Case ObjectClass
                    Case ITEM_CLASS.ITEM_CLASS_WEAPON
                        Select Case SubClass
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_MISC_WEAPON
                                Return 0
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_AXE
                                Return 196
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_AXE
                                Return 197
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_BOW
                                Return 264
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_GUN
                                Return 266
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_MACE
                                Return 198
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_MACE
                                Return 199
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_POLEARM
                                Return 200
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_SWORD
                                Return 201
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_TWOHAND_SWORD
                                Return 202
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_STAFF
                                Return 227
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_WEAPON_EXOTIC
                                Return 262
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_WEAPON_EXOTIC2
                                Return 263
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_FIST_WEAPON
                                Return 15590
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_DAGGER
                                Return 1180
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_THROWN
                                Return 2567
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_SPEAR
                                Return 3386
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW
                                Return 5011
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_WAND
                                Return 5009
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_FISHING_POLE
                                Return 7738
                        End Select
                    Case ITEM_CLASS.ITEM_CLASS_ARMOR
                        Select Case SubClass
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_MISC
                                Return 0
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_CLOTH
                                Return 9078
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_LEATHER
                                Return 9077
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_MAIL
                                Return 8737
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_PLATE
                                Return 750
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_SHIELD
                                Return 9116
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_BUCKLER
                                Return 9124
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_LIBRAM
                                Return 27762
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_TOTEM
                                Return 27763
                            Case ITEM_SUBCLASS.ITEM_SUBCLASS_IDOL
                                Return 27764
                        End Select
                    Case Else
                        Return 0
                End Select
            End Get
        End Property
    End Class

    'WARNING: Use only with ITEMs()
    Public NotInheritable Class ItemObject
        Implements IDisposable

        Public ReadOnly Property ItemInfo() As ItemInfo
            Get
                Return ITEMDatabase(ItemEntry)
            End Get
        End Property

        Public ReadOnly ItemEntry As Integer
        Public GUID As ULong
        Public OwnerGUID As ULong
        Public ReadOnly GiftCreatorGUID As ULong = 0
        Public ReadOnly CreatorGUID As ULong

        Public StackCount As Integer = 1
        Public Durability As Integer = 1
        Public ChargesLeft As Integer = 0
        Private _flags As Integer = 0
        Public Items As Dictionary(Of Byte, ItemObject) = Nothing
        Public ReadOnly RandomProperties As Integer = 0
        Public SuffixFactor As Integer = 0
        Public ReadOnly Enchantments As New Dictionary(Of Byte, TEnchantmentInfo)

        Private _loot As LootObject = Nothing

        'WARNING: Containers cannot hold itemText value
        Public ItemText As Integer = 0

        Public Sub FillAllUpdateFlags(ByRef update As UpdateClass)
            If ItemInfo.ContainerSlots > 0 Then
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_CONTAINER + ObjectType.TYPE_OBJECT)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ItemEntry)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 1.0F)

                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_OWNER, OwnerGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_CONTAINED, OwnerGUID)
                If CreatorGUID > 0 Then Update.SetUpdateFlag(EItemFields.ITEM_FIELD_CREATOR, CreatorGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_GIFTCREATOR, GiftCreatorGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_STACK_COUNT, StackCount)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURATION, 0)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_ITEM_TEXT_ID, ItemText)

                Update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_NUM_SLOTS, ItemInfo.ContainerSlots)
                'DONE: Here list in bag items
                Dim i As Byte
                For i = 0 To 35
                    If Items.ContainsKey(i) Then
                        Update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_SLOT_1 + i*2, CType(Items(i).GUID, Long))
                    Else
                        Update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_SLOT_1 + i*2, 0)
                    End If
                Next
            Else
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_ITEM + ObjectType.TYPE_OBJECT)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ItemEntry)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 1.0F)

                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_OWNER, OwnerGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_CONTAINED, OwnerGUID)
                If CreatorGUID > 0 Then Update.SetUpdateFlag(EItemFields.ITEM_FIELD_CREATOR, CreatorGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_GIFTCREATOR, GiftCreatorGUID)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_STACK_COUNT, StackCount)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURATION, 0)
                For i As Integer = 0 To 4
                    If _
                        ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE OrElse
                        ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.NO_DELAY_USE Then
                        Update.SetUpdateFlag(EItemFields.ITEM_FIELD_SPELL_CHARGES + i, ChargesLeft)
                    Else
                        Update.SetUpdateFlag(EItemFields.ITEM_FIELD_SPELL_CHARGES + i, - 1)
                    End If
                Next
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)

                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_PROPERTY_SEED, 0)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_RANDOM_PROPERTIES_ID, RandomProperties)

                For Each enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Enchantments
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key*3, enchant.Value.ID)
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key*3 + 1, enchant.Value.Duration)
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key*3 + 2, enchant.Value.Charges)
                Next

                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_ITEM_TEXT_ID, ItemText)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURABILITY, Durability)
                Update.SetUpdateFlag(EItemFields.ITEM_FIELD_MAXDURABILITY, ITEMDatabase(ItemEntry).Durability)
            End If
        End Sub

        Public Sub SendContainedItemsUpdate(ByRef client As ClientClass,
                                            Optional ByVal updatetype As Integer =
                                               ObjectUpdateType.UPDATETYPE_CREATE_OBJECT)
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(Items.Count)      'Operations.Count
            packet.AddInt8(0)

            For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                item.Value.FillAllUpdateFlags(tmpUpdate)
                tmpUpdate.AddToPacket(packet, updatetype, item.Value)
                tmpUpdate.Dispose()
            Next

            client.Send(packet)
        End Sub

        Private Sub InitializeBag()
            If ITEMDatabase(ItemEntry).IsContainer Then
                Items = New Dictionary(Of Byte, ItemObject)
            Else
                Items = Nothing
            End If
        End Sub

        Public ReadOnly Property IsFree() As Boolean
            Get
                If Items.Count > 0 Then Return False Else Return True
            End Get
        End Property
        'Public ReadOnly Property IsFull() As Boolean
        '    Get
        '        If Items.Count = ITEMDatabase(ItemEntry).ContainerSlots Then Return True Else Return False
        '    End Get
        'End Property
        'Public ReadOnly Property IsEquipped() As Boolean
        '    Get
        '        Dim srcBag As Byte = GetBagSlot
        '        Dim srcSlot As Integer = GetSlot
        '        If srcBag = 255 AndAlso srcSlot < EQUIPMENT_SLOT_END AndAlso srcSlot >= 0 Then Return True
        '        Return False
        '    End Get
        'End Property
        Public ReadOnly Property IsRanged() As Boolean
            Get
                Return _
                    (ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON AndAlso
                     (ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_BOW OrElse
                      ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW OrElse
                      ItemInfo.SubClass = ITEM_SUBCLASS.ITEM_SUBCLASS_GUN))
            End Get
        End Property

        Public ReadOnly Property GetBagSlot() As Byte
            Get
                If CHARACTERs.ContainsKey(OwnerGUID) = False Then Return 255

                With CHARACTERs(OwnerGUID)
                    For i As Byte = INVENTORY_SLOT_BAG_1 To INVENTORY_SLOT_BAG_END - 1
                        If .Items.ContainsKey(i) Then
                            For j As Byte = 0 To .Items(i).ItemInfo.ContainerSlots - 1
                                If .Items(i).Items.ContainsKey(j) Then
                                    If .Items(i).Items(j) Is Me Then Return i
                                End If
                            Next
                        End If
                    Next
                End With

                Return 255
            End Get
        End Property

        Public ReadOnly Property GetSlot() As Integer
            Get
                If CHARACTERs.ContainsKey(OwnerGUID) = False Then Return - 1

                With CHARACTERs(OwnerGUID)
                    For i As Byte = EQUIPMENT_SLOT_START To INVENTORY_SLOT_ITEM_END - 1
                        If .Items.ContainsKey(i) Then
                            If .Items(i) Is Me Then Return i
                        End If
                    Next

                    For i As Byte = KEYRING_SLOT_START To KEYRING_SLOT_END - 1
                        If .Items.ContainsKey(i) Then
                            If .Items(i) Is Me Then Return i
                        End If
                    Next

                    For i As Byte = INVENTORY_SLOT_BAG_1 To INVENTORY_SLOT_BAG_END - 1
                        If .Items.ContainsKey(i) Then
                            For j As Byte = 0 To .Items(i).ItemInfo.ContainerSlots - 1
                                If .Items(i).Items.ContainsKey(j) Then
                                    If .Items(i).Items(j) Is Me Then Return j
                                End If
                            Next
                        End If
                    Next
                End With

                Return - 1
            End Get
        End Property

        Public ReadOnly Property GetSkill() As Integer
            Get
                Return ItemInfo.GetReqSkill
            End Get
        End Property

        Public Function GenerateLoot() As Boolean
            If Not _loot Is Nothing Then Return True

            'DONE: Loot generation
            Dim mySqlQuery As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM item_loot WHERE entry = {0};", ItemEntry), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then Return False

            _loot = New LootObject(GUID, LootType.LOOTTYPE_CORPSE)
            Dim template As LootTemplate = LootTemplates_Item.GetLoot(ItemEntry)
            If template IsNot Nothing Then
                template.Process(_loot, 0)
            End If

            _loot.LootOwner = 0

            Return True
        End Function

        Public Sub New(ByVal guidVal As ULong, Optional ByVal owner As CharacterObject = Nothing,
                       Optional ByVal equipped As Boolean = False)
            'DONE: Get from SQLDB
            Dim mySqlQuery As New DataTable
            CharacterDatabase.Query(
                String.Format("SELECT * FROM characters_inventory WHERE item_guid = ""{0}"";", guidVal), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then _
                Err.Raise(1, "ItemObject.New", String.Format("itemGuid {0} not found in SQL database!", guidVal))

            GUID = mySqlQuery.Rows(0).Item("item_guid") + GUID_ITEM
            CreatorGUID = mySqlQuery.Rows(0).Item("item_creator")
            OwnerGUID = mySqlQuery.Rows(0).Item("item_owner")
            GiftCreatorGUID = mySqlQuery.Rows(0).Item("item_giftCreator")
            StackCount = mySqlQuery.Rows(0).Item("item_stackCount")
            Durability = mySqlQuery.Rows(0).Item("item_durability")
            ChargesLeft = mySqlQuery.Rows(0).Item("item_chargesLeft")
            RandomProperties = mySqlQuery.Rows(0).Item("item_random_properties")
            ItemEntry = mySqlQuery.Rows(0).Item("item_id")
            _flags = mySqlQuery.Rows(0).Item("item_flags")
            ItemText = mySqlQuery.Rows(0).Item("item_textId")

            'DONE: Intitialize enchantments - Saved as STRING like "Slot1:ID1:Duration:Charges Slot2:ID2:Duration:Charges Slot3:ID3:Duration:Charges"
            Dim tmp() As String = Split(CType(mySqlQuery.Rows(0).Item("item_enchantment"), String), " ")
            If tmp.Length > 0 Then
                For i As Integer = 0 To tmp.Length - 1
                    If Trim(tmp(i)) <> "" Then
                        Dim tmp2() As String
                        tmp2 = Split(tmp(i), ":")
                        'DONE: Add the enchantment
                        Enchantments.Add(tmp2(0), New TEnchantmentInfo(tmp2(1), tmp2(2), tmp2(3)))
                        'DONE: Add the bonuses to the character
                        If equipped Then AddEnchantBonus(tmp2(0), owner)
                    End If
                Next i
            End If


            'DONE: Load ItemID in cashe if not loaded
            If ITEMDatabase.ContainsKey(ItemEntry) = False Then
                'TODO: This needs to actually do something
                Dim tmpItem As New ItemInfo(ItemEntry)
            End If

            InitializeBag()

            'DONE: Get Items
            mySqlQuery.Clear()
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_inventory WHERE item_bag = {0};", GUID),
                                    mySqlQuery)
            For Each row As DataRow In mySqlQuery.Rows
                If row.Item("item_slot") <> ITEM_SLOT_NULL Then
                    Dim tmpItem As New ItemObject(CType(row.Item("item_guid"), Long))
                    Items(CType(row.Item("item_slot"), Byte)) = tmpItem
                End If
            Next

            WORLD_ITEMs.Add(GUID, Me)
        End Sub

        Public Sub New(ByVal itemId As Integer, ByVal owner As ULong)
            'DONE: Load ItemID in cashe if not loaded
            If ITEMDatabase.ContainsKey(itemId) = False Then
                'TODO: This needs to actually do something
                Dim tmpItem As New ItemInfo(itemId)
            End If
            ItemEntry = itemId
            OwnerGUID = Owner
            Durability = ITEMDatabase(ItemEntry).Durability

            For i As Integer = 0 To 4
                If _
                    ITEMDatabase(ItemEntry).Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE OrElse
                    ITEMDatabase(ItemEntry).Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.NO_DELAY_USE Then
                    If ITEMDatabase(ItemEntry).Spells(i).SpellCharges <> 0 Then
                        ChargesLeft = ITEMDatabase(ItemEntry).Spells(i).SpellCharges
                        Exit For
                    End If
                End If
            Next i

            'DONE: Create new GUID 
            GUID = GetNewGUID()
            InitializeBag()
            SaveAsNew()

            WORLD_ITEMs.Add(GUID, Me)
        End Sub

        Private Sub SaveAsNew()
            'DONE: Save to SQL
            Dim tmpCmd As String = "INSERT INTO characters_inventory (item_guid"
            Dim tmpValues As String = " VALUES (" & GUID - GUID_ITEM
            tmpCmd = tmpCmd & ", item_owner"
            tmpValues = tmpValues & ", """ & OwnerGUID & """"
            tmpCmd = tmpCmd & ", item_creator"
            tmpValues = tmpValues & ", " & CreatorGUID
            tmpCmd = tmpCmd & ", item_giftCreator"
            tmpValues = tmpValues & ", " & GiftCreatorGUID
            tmpCmd = tmpCmd & ", item_stackCount"
            tmpValues = tmpValues & ", " & StackCount
            tmpCmd = tmpCmd & ", item_durability"
            tmpValues = tmpValues & ", " & Durability
            tmpCmd = tmpCmd & ", item_chargesLeft"
            tmpValues = tmpValues & ", " & ChargesLeft
            tmpCmd = tmpCmd & ", item_random_properties"
            tmpValues = tmpValues & ", " & RandomProperties
            tmpCmd = tmpCmd & ", item_id"
            tmpValues = tmpValues & ", " & ItemEntry
            tmpCmd = tmpCmd & ", item_flags"
            tmpValues = tmpValues & ", " & _flags


            'DONE: Saving enchanments
            Dim temp As New ArrayList
            For Each enchantment As KeyValuePair(Of Byte, TEnchantmentInfo) In Enchantments
                temp.Add(String.Format("{0}:{1}:{2}:{3}", enchantment.Key, enchantment.Value.ID,
                                       enchantment.Value.Duration, enchantment.Value.Charges))
            Next
            tmpCmd = tmpCmd & ", item_enchantment"
            tmpValues = tmpValues & ", '" & Join(temp.ToArray, " ") & "'"
            tmpCmd = tmpCmd & ", item_textId"
            tmpValues = tmpValues & ", " & ItemText


            tmpCmd = tmpCmd & ") " & tmpValues & ");"
            CharacterDatabase.Update(tmpCmd)
        End Sub

        Public Sub Save(Optional ByVal saveAll As Boolean = True)
            Dim tmp As String = "UPDATE characters_inventory SET"

            tmp = tmp & " item_owner=""" & OwnerGUID & """"
            tmp = tmp & ", item_creator=" & CreatorGUID
            tmp = tmp & ", item_giftCreator=" & GiftCreatorGUID
            tmp = tmp & ", item_stackCount=" & StackCount
            tmp = tmp & ", item_durability=" & Durability
            tmp = tmp & ", item_chargesLeft=" & ChargesLeft
            tmp = tmp & ", item_random_properties=" & RandomProperties
            tmp = tmp & ", item_flags=" & _flags


            'DONE: Saving enchanments
            Dim temp As New ArrayList
            For Each enchantment As KeyValuePair(Of Byte, TEnchantmentInfo) In Enchantments
                temp.Add(String.Format("{0}:{1}:{2}:{3}", enchantment.Key, enchantment.Value.ID,
                                       enchantment.Value.Duration, enchantment.Value.Charges))
            Next
            tmp = tmp & ", item_enchantment=""" & Join(temp.ToArray, " ") & """"
            tmp = tmp & ", item_textId=" & ItemText


            tmp = tmp & " WHERE item_guid = """ & (GUID - GUID_ITEM) & """;"

            CharacterDatabase.Update(tmp)

            If ITEMDatabase(ItemEntry).IsContainer() AndAlso saveAll Then
                For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                    item.Value.Save()
                Next
            End If
        End Sub

        Public Sub Delete()
            'DONE: Check if item is petition
            If _
                ItemEntry = PETITION_GUILD OrElse ItemEntry = PETITION_2v2 OrElse ItemEntry = PETITION_3v3 OrElse
                ItemEntry = PETITION_5v5 Then _
                CharacterDatabase.Update("DELETE FROM petitions WHERE petition_itemGuid = " & GUID - GUID_ITEM & ";")

            CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_guid = {0}",
                                                   GUID - GUID_ITEM))

            If ITEMDatabase(ItemEntry).IsContainer() Then
                For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                    item.Value.Delete()
                Next
            End If
            Dispose()
        End Sub

#Region "IDisposable Support"

        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Private Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                WORLD_ITEMs.Remove(GUID)

                If ITEMDatabase(ItemEntry).IsContainer() Then
                    For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                        item.Value.Dispose()
                    Next
                End If
                If Not IsNothing(_loot) Then _loot.Dispose()
            End If
            _disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region

        Public Function IsBroken() As Boolean
            Return (Durability = 0) AndAlso (ItemInfo.Durability > 0)
        End Function

        Public Sub ModifyDurability(ByVal percent As Single, ByRef client As ClientClass)
            If ITEMDatabase(ItemEntry).Durability > 0 Then
                Durability -= Fix(ITEMDatabase(ItemEntry).Durability*percent)
                If Durability < 0 Then Durability = 0
                If Durability > ITEMDatabase(ItemEntry).Durability Then Durability = ITEMDatabase(ItemEntry).Durability
                UpdateDurability(Client)
            End If
        End Sub

        Public Sub ModifyToDurability(ByVal percent As Single, ByRef client As ClientClass)
            If ITEMDatabase(ItemEntry).Durability > 0 Then
                Durability = Fix(ITEMDatabase(ItemEntry).Durability*percent)
                If Durability < 0 Then Durability = 0
                If Durability > ITEMDatabase(ItemEntry).Durability Then Durability = ITEMDatabase(ItemEntry).Durability
                UpdateDurability(Client)
            End If
        End Sub

        Private Sub UpdateDurability(ByRef client As ClientClass)
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)      'Operations.Count
            packet.AddInt8(0)
            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
            tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_DURABILITY, Durability)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            client.Send(packet)
        End Sub

        Public ReadOnly Property GetDurabulityCost() As UInteger
            Get
                Try
                    Dim lostDurability As Integer = ITEMDatabase(ItemEntry).Durability - Durability
                    If lostDurability > DurabilityCosts_MAX Then lostDurability = DurabilityCosts_MAX
                    Dim subClass As Integer = 0
                    If ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then subClass = ItemInfo.SubClass Else _
                        subClass = ItemInfo.SubClass + 21
                    Dim durabilityCost As UInteger =
                            (lostDurability * ((DurabilityCosts(ItemInfo.Level, subClass) / 40) * 100))
                    Log.WriteLine(BaseWriter.LogType.DEBUG, "Durability cost: {0}", durabilityCost)
                    Return durabilityCost
                Catch
                    Return 0
                End Try
            End Get
        End Property

        Public Sub AddEnchantment(ByVal id As Integer, ByVal slot As Byte, Optional ByVal duration As Integer = 0,
                                  Optional ByVal charges As Integer = 0)
            'DONE: Replace if an enchant already is placed in this slot
            If Enchantments.ContainsKey(slot) Then RemoveEnchantment(slot)
            'DONE: Add the enchantment
            Enchantments.Add(slot, New TEnchantmentInfo(ID, duration, charges))
            'DONE: Add the bonuses to the character if it's equipped
            AddEnchantBonus(slot)
        End Sub

        Public Sub AddEnchantBonus(ByVal slot As Byte, Optional ByRef c As CharacterObject = Nothing)
            If c Is Nothing Then
                If CHARACTERs.ContainsKey(OwnerGUID) = False Then Exit Sub
                c = CHARACTERs(OwnerGUID)
            End If
            If c IsNot Nothing AndAlso SpellItemEnchantments.ContainsKey(Enchantments(Slot).ID) Then
                For i As Byte = 0 To 2
                    If SpellItemEnchantments(Enchantments(Slot).ID).SpellID(i) <> 0 Then
                        If SPELLs.ContainsKey(SpellItemEnchantments(Enchantments(Slot).ID).SpellID(i)) Then
                            Dim spellInfo As SpellInfo
                            spellInfo = SPELLs(SpellItemEnchantments(Enchantments(Slot).ID).SpellID(i))
                            For j As Byte = 0 To 2
                                If Not (spellInfo.SpellEffects(j) Is Nothing) Then
                                    Select Case spellInfo.SpellEffects(j).ID
                                        Case SpellEffects_Names.SPELL_EFFECT_APPLY_AURA
                                            AURAs(spellInfo.SpellEffects(j).ApplyAuraIndex).Invoke(c, c,
                                                                                                   spellInfo.
                                                                                                      SpellEffects(j),
                                                                                                   spellInfo.ID, 1,
                                                                                                   AuraAction.AURA_ADD)
                                    End Select
                                End If
                            Next j
                        End If
                    End If
                Next
            End If
        End Sub

        Public Sub RemoveEnchantBonus(ByVal slot As Byte)
            If CHARACTERs.ContainsKey(OwnerGUID) AndAlso SpellItemEnchantments.ContainsKey(Enchantments(Slot).ID) Then
                For i As Byte = 0 To 2
                    If SpellItemEnchantments(Enchantments(Slot).ID).SpellID(i) <> 0 Then
                        If SPELLs.ContainsKey(SpellItemEnchantments(Enchantments(Slot).ID).SpellID(i)) Then
                            Dim spellInfo As SpellInfo
                            spellInfo = SPELLs(SpellItemEnchantments(Enchantments(slot).ID).SpellID(i))
                            For j As Byte = 0 To 2
                                If Not (spellInfo.SpellEffects(j) Is Nothing) Then
                                    Select Case spellInfo.SpellEffects(j).ID
                                        Case SpellEffects_Names.SPELL_EFFECT_APPLY_AURA
                                            AURAs(spellInfo.SpellEffects(j).ApplyAuraIndex).Invoke(CHARACTERs(OwnerGUID),
                                                                                                   CHARACTERs(OwnerGUID),
                                                                                                   spellInfo.
                                                                                                      SpellEffects(j),
                                                                                                   spellInfo.ID, 1,
                                                                                                   AuraAction.
                                                                                                      AURA_REMOVE)
                                    End Select
                                End If
                            Next j
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub RemoveEnchantment(ByVal slot As Byte)
            If Enchantments.ContainsKey(Slot) = False Then Exit Sub
            'DONE: Remove the bonuses from the character
            RemoveEnchantBonus(Slot)
            'DONE: Remove the enchant
            Enchantments.Remove(Slot)
            'DONE: Send the update to the client about it
            If CHARACTERs.ContainsKey(OwnerGUID) Then
                Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (Slot * 3), 0)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (Slot * 3) + 1, 0)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (Slot * 3) + 2, 0)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

                CHARACTERs(OwnerGUID).Client.Send(packet)
                packet.Dispose()
                tmpUpdate.Dispose()
            End If
        End Sub

        Public Sub SoulbindItem(Optional ByRef client As ClientClass = Nothing)
            If (_flags And ITEM_FLAGS.ITEM_FLAGS_BINDED) = ITEM_FLAGS.ITEM_FLAGS_BINDED Then Exit Sub

            'DONE: Setting the flag
            _flags = _flags Or ITEM_FLAGS.ITEM_FLAGS_BINDED
            Save()

            'DONE: Sending update to character
            If Not Client Is Nothing Then
                Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

                Client.Send(packet)
                packet.Dispose()
                tmpUpdate.Dispose()
            End If
        End Sub

        Public ReadOnly Property IsSoulBound() As Boolean
            Get
                Return ((_flags And ITEM_FLAGS.ITEM_FLAGS_BINDED) = ITEM_FLAGS.ITEM_FLAGS_BINDED)
            End Get
        End Property
    End Class

    Public Class TDamage
        Public Minimum As Single = 0
        Public Maximum As Single = 0
        Public Type As Integer = DamageTypes.DMG_PHYSICAL
    End Class

    Public Class TEnchantmentInfo
        Public ReadOnly ID As Integer = 0
        Public ReadOnly Duration As Integer = 0
        Public ReadOnly Charges As Integer = 0

        Public Sub New(ByVal ID_ As Integer, Optional ByVal Duration_ As Integer = 0,
                       Optional ByVal Charges_ As Integer = 0)
            ID = ID_
            Duration = Duration_
            Charges = Charges_
        End Sub
    End Class

    Public Class TItemSpellInfo
        Public SpellID As Integer = 0
        Public SpellTrigger As ITEM_SPELLTRIGGER_TYPE = 0
        Public SpellCharges As Integer = - 1
        Public SpellCooldown As Integer = 0
        Public SpellCategory As Integer = 0
        Public SpellCategoryCooldown As Integer = 0
    End Class


#End Region

#Region "WS.Items.Handlers"

    <MethodImpl(MethodImplOptions.Synchronized)>
    Private Function GetNewGUID() As ULong
        itemGuidCounter += 1
        GetNewGUID = itemGuidCounter
    End Function

    Public Function LoadItemByGUID(ByVal guid As ULong, Optional ByVal owner As CharacterObject = Nothing,
                                   Optional ByVal equipped As Boolean = False) As ItemObject
        If WORLD_ITEMs.ContainsKey(guid + GUID_ITEM) Then
            Return WORLD_ITEMs(guid + GUID_ITEM)
        End If

        Dim tmpItem As New ItemObject(guid, Owner, equipped)
        Return tmpItem
    End Function

    Public Sub SendItemInfo(ByRef client As ClientClass, ByVal itemID As Integer)
        Dim response As New PacketClass(OPCODES.SMSG_ITEM_QUERY_SINGLE_RESPONSE)

        Dim item As ItemInfo

        If ITEMDatabase.ContainsKey(ItemID) = False Then
            item = New ItemInfo(ItemID)
        Else
            item = ITEMDatabase(ItemID)
        End If
        Dim i As Integer

        response.AddInt32(item.Id)
        response.AddInt32(item.ObjectClass)
        If item.ObjectClass = ITEM_CLASS.ITEM_CLASS_CONSUMABLE Then
            response.AddInt32(0)
        Else
            response.AddInt32(item.SubClass)
        End If
        response.AddString(item.Name)
        response.AddInt8(0)     'Item.Name2
        response.AddInt8(0)     'Item.Name3
        response.AddInt8(0)     'Item.Name4

        response.AddInt32(item.Model)
        response.AddInt32(item.Quality)
        response.AddInt32(item.Flags)
        response.AddInt32(item.BuyPrice)
        response.AddInt32(item.SellPrice)
        response.AddInt32(item.InventoryType)
        response.AddUInt32(item.AvailableClasses)
        response.AddUInt32(item.AvailableRaces)
        response.AddInt32(item.Level)
        response.AddInt32(item.ReqLevel)
        response.AddInt32(item.ReqSkill)
        response.AddInt32(item.ReqSkillRank)
        response.AddInt32(item.ReqSpell)
        response.AddInt32(item.ReqHonorRank)
        response.AddInt32(item.ReqHonorRank2) _
        'RequiredCityRank           [1 - Protector of Stormwind, 2 - Overlord of Orgrimmar, 3 - Thane of Ironforge, 4 - High Sentinel of Darnassus, 5 - Deathlord of the Undercity, 6 - Chieftan of Thunderbluff, 7 - Avenger of Gnomeregan, 8 - Voodoo Boss of Senjin]
        response.AddInt32(item.ReqFaction)          'RequiredReputationFaction
        response.AddInt32(item.ReqFactionLevel)     'RequiredRaputationRank
        response.AddInt32(item.Unique) ' Was stackable
        response.AddInt32(item.Stackable)
        response.AddInt32(item.ContainerSlots)

        For i = 0 To 9
            response.AddInt32(item.ItemBonusStatType(i))
            response.AddInt32(item.ItemBonusStatValue(i))
        Next

        For i = 0 To 4
            response.AddSingle(item.Damage(i).Minimum)
            response.AddSingle(item.Damage(i).Maximum)
            response.AddInt32(item.Damage(i).Type)
        Next
        For i = 0 To 6
            response.AddInt32(item.Resistances(i))
        Next

        response.AddInt32(item.Delay)
        response.AddInt32(item.AmmoType)
        response.AddSingle(item.Range)          'itemRangeModifier (Ranged Weapons = 100.0, Fishing Poles = 3.0)

        For i = 0 To 4
            If SPELLs.ContainsKey(item.Spells(i).SpellID) = False Then
                response.AddInt32(0)
                response.AddInt32(0)
                response.AddInt32(0)
                response.AddInt32(-1)
                response.AddInt32(0)
                response.AddInt32(-1)
            Else
                response.AddInt32(item.Spells(i).SpellID)
                response.AddInt32(item.Spells(i).SpellTrigger)
                response.AddInt32(item.Spells(i).SpellCharges)
                If item.Spells(i).SpellCooldown > 0 OrElse item.Spells(i).SpellCategoryCooldown > 0 Then
                    response.AddInt32(item.Spells(i).SpellCooldown)
                    response.AddInt32(item.Spells(i).SpellCategory)
                    response.AddInt32(item.Spells(i).SpellCategoryCooldown)
                Else
                    response.AddInt32(SPELLs(item.Spells(i).SpellID).SpellCooldown)
                    response.AddInt32(SPELLs(item.Spells(i).SpellID).Category)
                    response.AddInt32(SPELLs(item.Spells(i).SpellID).CategoryCooldown)
                End If
            End If
        Next

        response.AddInt32(item.Bonding)
        response.AddString(item.Description)
        response.AddInt32(item.PageText)
        response.AddInt32(item.LanguageID)
        response.AddInt32(item.PageMaterial)
        response.AddInt32(item.StartQuest)
        response.AddInt32(item.LockID)
        response.AddInt32(item.Material)
        response.AddInt32(item.Sheath)
        response.AddInt32(item.Extra)
        response.AddInt32(item.Block)
        response.AddInt32(item.ItemSet)
        response.AddInt32(item.Durability)
        response.AddInt32(item.ZoneNameID)
        response.AddInt32(item.MapID) ' Added in 1.12.1 client branch
        response.AddInt32(item.BagFamily)

        'response.AddInt32(Item.TotemCategory)
        'response.AddInt32(Item.ReqDisenchantSkill)
        'response.AddInt32(Item.ArmorDamageModifier)
        'response.AddInt32(Item.ExistingDuration)

        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_ITEM_QUERY_SINGLE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim itemID As Integer = packet.GetInt32

        SendItemInfo(Client, itemID)
    End Sub

    Public Sub On_CMSG_ITEM_NAME_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim itemID As Integer = packet.GetInt32

        Dim item As ItemInfo

        If ITEMDatabase.ContainsKey(itemID) = False Then
            item = New ItemInfo(itemID)
        Else
            item = ITEMDatabase(itemID)
        End If

        Dim response As New PacketClass(OPCODES.SMSG_ITEM_NAME_QUERY_RESPONSE)
        response.AddInt32(itemID)
        response.AddString(item.Name)
        response.AddInt32(item.InventoryType)
        Client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_SWAP_INV_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 7 Then Exit Sub
        packet.GetInt16()
        Dim srcSlot As Byte = packet.GetInt8
        Dim dstSlot As Byte = packet.GetInt8
        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_SWAP_INV_ITEM [srcSlot=0:{2}, dstSlot=0:{3}]", Client.IP,
                      Client.Port, srcSlot, dstSlot)

        Client.Character.ItemSWAP(0, srcSlot, 0, dstSlot)
    End Sub

    Public Sub On_CMSG_AUTOEQUIP_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 7 Then Exit Sub
        Try
            packet.GetInt16()
            Dim srcBag As Byte = packet.GetInt8
            Dim srcSlot As Byte = packet.GetInt8
            If srcBag = 255 Then srcBag = 0
            Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_AUTOEQUIP_ITEM [srcSlot={3}:{2}]", Client.IP,
                          Client.Port, srcSlot, srcBag)

            Dim errCode As Byte = InventoryChangeFailure.EQUIP_ERR_ITEM_CANT_BE_EQUIPPED

            'DONE: Check owner of the item
            If Client.Character.ItemGET(srcBag, srcSlot).OwnerGUID <> Client.Character.GUID Then
                errCode = InventoryChangeFailure.EQUIP_ERR_DONT_OWN_THAT_ITEM
            Else

                If srcBag = 0 AndAlso Client.Character.Items.ContainsKey(srcSlot) Then
                    Dim slots() As Byte = Client.Character.Items(srcSlot).ItemInfo.GetSlots
                    For Each tmpSlot As Byte In slots
                        If Not Client.Character.Items.ContainsKey(tmpSlot) Then
                            Client.Character.ItemSWAP(srcBag, srcSlot, 0, tmpSlot)
                            errCode = InventoryChangeFailure.EQUIP_ERR_OK
                            Exit For
                        Else
                            errCode = InventoryChangeFailure.EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE
                        End If
                    Next
                    If errCode = InventoryChangeFailure.EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE Then
                        For Each tmpSlot As Byte In slots
                            Client.Character.ItemSWAP(srcBag, srcSlot, 0, tmpSlot)
                            errCode = InventoryChangeFailure.EQUIP_ERR_OK
                            Exit For
                        Next
                    End If
                ElseIf srcBag > 0 Then
                    Dim slots() As Byte = Client.Character.Items(srcBag).Items(srcSlot).ItemInfo.GetSlots
                    For Each tmpSlot As Byte In slots
                        If Not Client.Character.Items.ContainsKey(tmpSlot) Then
                            Client.Character.ItemSWAP(srcBag, srcSlot, 0, tmpSlot)
                            errCode = InventoryChangeFailure.EQUIP_ERR_OK
                            Exit For
                        Else
                            errCode = InventoryChangeFailure.EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE
                        End If
                    Next
                    If errCode = InventoryChangeFailure.EQUIP_ERR_NO_EQUIPMENT_SLOT_AVAILABLE Then
                        For Each tmpSlot As Byte In slots
                            Client.Character.ItemSWAP(srcBag, srcSlot, 0, tmpSlot)
                            errCode = InventoryChangeFailure.EQUIP_ERR_OK
                            Exit For
                        Next
                    End If
                Else
                    errCode = InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND
                End If

            End If

            If errCode <> InventoryChangeFailure.EQUIP_ERR_OK Then
                Dim response As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                response.AddInt8(errCode)
                response.AddUInt64(Client.Character.ItemGetGUID(srcBag, srcSlot))
                response.AddUInt64(0)
                response.AddInt8(0)
                Client.Send(response)
                response.Dispose()
            End If
        Catch err As Exception
            Log.WriteLine(BaseWriter.LogType.FAILED, "[{0}:{1}] Unable to equip item. {2}{3}", Client.IP, Client.Port,
                          vbNewLine, err.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_AUTOSTORE_BAG_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 8 Then Exit Sub
        packet.GetInt16()
        Dim srcBag As Byte = packet.GetInt8
        Dim srcSlot As Byte = packet.GetInt8
        Dim dstBag As Byte = packet.GetInt8
        If srcBag = 255 Then srcBag = 0
        If dstBag = 255 Then dstBag = 0
        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_AUTOSTORE_BAG_ITEM [srcSlot={3}:{2}, dstBag={4}]",
                      Client.IP, Client.Port, srcSlot, srcBag, dstBag)

        If Client.Character.ItemADD_AutoBag(WORLD_ITEMs(Client.Character.ItemGetGUID(srcBag, srcSlot)), dstBag) Then
            Client.Character.ItemREMOVE(srcBag, srcSlot, False, True)
            SendInventoryChangeFailure(Client.Character, InventoryChangeFailure.EQUIP_ERR_OK,
                                       Client.Character.ItemGetGUID(srcBag, srcSlot), 0)
        End If
    End Sub

    Public Sub On_CMSG_SWAP_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim dstBag As Byte = packet.GetInt8
        Dim dstSlot As Byte = packet.GetInt8
        Dim srcBag As Byte = packet.GetInt8
        Dim srcSlot As Byte = packet.GetInt8
        If dstBag = 255 Then dstBag = 0
        If srcBag = 255 Then srcBag = 0

        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_SWAP_ITEM [srcSlot={4}:{2}, dstSlot={5}:{3}]", Client.IP,
                      Client.Port, srcSlot, dstSlot, srcBag, dstBag)
        Client.Character.ItemSWAP(srcBag, srcSlot, dstBag, dstSlot)
    End Sub

    Public Sub On_CMSG_SPLIT_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 10 Then Exit Sub
        packet.GetInt16()
        Dim srcBag As Byte = packet.GetInt8
        Dim srcSlot As Byte = packet.GetInt8
        Dim dstBag As Byte = packet.GetInt8
        Dim dstSlot As Byte = packet.GetInt8
        Dim count As Byte = packet.GetInt8
        If dstBag = 255 Then dstBag = 0
        If srcBag = 255 Then srcBag = 0

        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_SPLIT_ITEM [srcSlot={3}:{2}, dstBag={5}:{4}, count={6}]",
                      Client.IP, Client.Port, srcSlot, srcBag, dstSlot, dstBag, count)
        If srcBag = dstBag AndAlso srcSlot = dstSlot Then Return
        If count > 0 Then Client.Character.ItemSPLIT(srcBag, srcSlot, dstBag, dstSlot, count)
    End Sub

    Public Sub On_CMSG_READ_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 7 Then Exit Sub
        packet.GetInt16()
        Dim srcBag As Byte = packet.GetInt8
        Dim srcSlot As Byte = packet.GetInt8
        If srcBag = 255 Then srcBag = 0
        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_READ_ITEM [srcSlot={3}:{2}]", Client.IP, Client.Port,
                      srcSlot, srcBag)

        'TODO: If InCombat/Dead
        Dim opcode As Short = OPCODES.SMSG_READ_ITEM_FAILED
        Dim guid As ULong = 0

        If srcBag = 0 Then
            If client.Character.Items.ContainsKey(srcSlot) Then
                opcode = OPCODES.SMSG_READ_ITEM_OK
                If client.Character.Items(srcSlot).ItemInfo.PageText > 0 Then _
                    guid = client.Character.Items(srcSlot).GUID
            End If
        Else
            If client.Character.Items.ContainsKey(srcBag) Then
                If client.Character.Items(srcBag).Items.ContainsKey(srcSlot) Then
                    opcode = OPCODES.SMSG_READ_ITEM_OK
                    If client.Character.Items(srcBag).Items(srcSlot).ItemInfo.PageText > 0 Then _
                        guid = client.Character.Items(srcBag).Items(srcSlot).GUID
                End If
            End If
        End If

        If guid <> 0 Then
            Dim response As New PacketClass(opcode)
            response.AddUInt64(guid)
            client.Send(response)
            response.Dispose()
        End If
    End Sub

    Public Sub On_CMSG_PAGE_TEXT_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim pageID As Integer = packet.GetInt32
        Dim itemGuid As ULong = packet.GetUInt64
        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_PAGE_TEXT_QUERY [pageID={2}, itemGuid={3:X}]", Client.IP,
                      Client.Port, pageID, itemGuid)

        Dim mySqlQuery As New DataTable
        WorldDatabase.Query(String.Format("SELECT * FROM itempages WHERE entry = ""{0}"";", pageID), mySqlQuery)

        Dim response As New PacketClass(OPCODES.SMSG_PAGE_TEXT_QUERY_RESPONSE)
        response.AddInt32(pageID)
        If mySqlQuery.Rows.Count <> 0 Then response.AddString(mySqlQuery.Rows(0).Item("text")) Else _
            response.AddString("Page " & pageID & " not found! Please report this to database devs.")
        If mySqlQuery.Rows.Count <> 0 Then response.AddInt32(mySqlQuery.Rows(0).Item("next_page")) Else _
            response.AddInt32(0)
        Client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_WRAP_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim giftBag As Byte = packet.GetInt8
        Dim giftSlot As Byte = packet.GetInt8
        Dim itemBag As Byte = packet.GetInt8
        Dim itemSlot As Byte = packet.GetInt8
        If giftBag = 255 Then giftBag = 0
        If itemBag = 255 Then itemBag = 0

        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_WRAP_ITEM [{2}:{3} -> {4}{5}]", Client.IP, Client.Port,
                      giftBag, giftSlot, itemBag, itemSlot)

        Dim gift As ItemObject = Client.Character.ItemGET(giftBag, giftSlot)
        Dim item As ItemObject = Client.Character.ItemGET(itemBag, itemSlot)

        If gift Is Nothing Or item Is Nothing Then
            SendInventoryChangeFailure(Client.Character, InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND, 0, 0)
        End If


        'if(item==gift)                                          // not possable with pacjket from real client
        '{
        '    _player->SendEquipError( EQUIP_ERR_WRAPPED_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        'if(item->IsEquipped())
        '{
        '    _player->SendEquipError( EQUIP_ERR_EQUIPPED_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        'if(item->GetUInt64Value(ITEM_FIELD_GIFTCREATOR)) // HasFlag(ITEM_FIELD_FLAGS, 8);
        '{
        '    _player->SendEquipError( EQUIP_ERR_WRAPPED_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        'if(item->IsBag())
        '{
        '    _player->SendEquipError( EQUIP_ERR_BAGS_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        'if(item->IsSoulBound() || item->GetProto()->Class == ITEM_CLASS_QUEST)
        '{
        '    _player->SendEquipError( EQUIP_ERR_BOUND_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        'if(item->GetMaxStackCount() != 1)
        '{
        '    _player->SendEquipError( EQUIP_ERR_STACKABLE_CANT_BE_WRAPPED, item, NULL );
        '    return;
        '}

        '//if(item->IsUnique()) // need figure out unique item flags...
        '//{
        '//    _player->SendEquipError( EQUIP_ERR_UNIQUE_CANT_BE_WRAPPED, item, NULL );
        '//    return;
        '//}

        'sDatabase.BeginTransaction();
        'sDatabase.PExecute("INSERT INTO `character_gifts` VALUES ('%u', '%u', '%u', '%u')", GUID_LOPART(item->GetOwnerGUID()), item->GetGUIDLow(), item->GetEntry(), item->GetUInt32Value(ITEM_FIELD_FLAGS));
        'item->SetUInt32Value(OBJECT_FIELD_ENTRY, gift->GetUInt32Value(OBJECT_FIELD_ENTRY));
        'item->SetUInt64Value(ITEM_FIELD_GIFTCREATOR, _player->GetGUID());
        'item->SetUInt32Value(ITEM_FIELD_FLAGS, 8); // wrapped ?
        'item->SetState(ITEM_CHANGED, _player);

        'if(item->GetState()==ITEM_NEW)                          // save new item, to have alway for `character_gifts` record in `item_template`
        '    item->SaveToDB();
        'sDatabase.CommitTransaction();

        'uint32 count = 1;
        '_player->DestroyItemCount(gift, count, true);
    End Sub

    Public Sub On_CMSG_DESTROYITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 8 Then Exit Sub
        Try
            packet.GetInt16()
            Dim srcBag As Byte = packet.GetInt8
            Dim srcSlot As Byte = packet.GetInt8
            Dim count As Byte = packet.GetInt8
            If srcBag = 255 Then srcBag = 0
            Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_DESTROYITEM [srcSlot={3}:{2}  count={4}]", Client.IP,
                          Client.Port, srcSlot, srcBag, count)

            If srcBag = 0 Then
                If Client.Character.Items.ContainsKey(srcSlot) = False Then Exit Sub
                'DONE: Fire quest event to check for if this item is required for quest
                'NOTE: Not only quest items are needed for quests
                ALLQUESTS.OnQuestItemRemove(Client.Character, Client.Character.Items(srcSlot).ItemEntry, count)

                If count = 0 Or count >= Client.Character.Items(srcSlot).StackCount Then
                    If srcSlot < INVENTORY_SLOT_BAG_END Then _
                        Client.Character.UpdateRemoveItemStats(Client.Character.Items(srcSlot), srcSlot)
                    Client.Character.ItemREMOVE(srcBag, srcSlot, True, True)
                Else
                    Client.Character.Items(srcSlot).StackCount -= count
                    Client.Character.SendItemUpdate(Client.Character.Items(srcSlot))
                    Client.Character.Items(srcSlot).Save()
                End If

            Else
                If Client.Character.Items.ContainsKey(srcBag) = False Then Exit Sub
                If Client.Character.Items(srcBag).Items.ContainsKey(srcSlot) = False Then Exit Sub
                'DONE: Fire quest event to check for if this item is required for quest
                'NOTE: Not only quest items are needed for quests
                ALLQUESTS.OnQuestItemRemove(Client.Character, Client.Character.Items(srcBag).Items(srcSlot).ItemEntry,
                                            count)

                If count = 0 Or count >= Client.Character.Items(srcBag).Items(srcSlot).StackCount Then
                    Client.Character.ItemREMOVE(srcBag, srcSlot, True, True)
                Else
                    Client.Character.Items(srcBag).Items(srcSlot).StackCount -= count
                    Client.Character.SendItemUpdate(Client.Character.Items(srcBag).Items(srcSlot))
                    Client.Character.Items(srcBag).Items(srcSlot).Save()
                End If
            End If

        Catch e As Exception
            Log.WriteLine(BaseWriter.LogType.DEBUG, "Error destroying item.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_USE_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        Try
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim bag As Byte = packet.GetInt8
            If bag = 255 Then bag = 0
            Dim slot As Byte = packet.GetInt8
            Dim tmp3 As Byte = packet.GetInt8

            Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_USE_ITEM [bag={2} slot={3} tmp3={4}]", Client.IP,
                          Client.Port, bag, slot, tmp3)
            If (Client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_TAXI_FLIGHT) Then Exit Sub _
            'Don't allow item usage when on a taxi

            Dim itemGuid As ULong = Client.Character.ItemGetGUID(bag, slot)
            If WORLD_ITEMs.ContainsKey(itemGuid) = False Then
                SendInventoryChangeFailure(Client.Character, InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND, 0, 0)
                Exit Sub
            End If
            Dim itemInfo As ItemInfo = WORLD_ITEMs(itemGuid).ItemInfo

            'DONE: Check if the item can be used in combat
            Dim InstantCast As Boolean = False

            For i As Byte = 0 To 4
                If SPELLs.ContainsKey(itemInfo.Spells(i).SpellID) Then
                    If ((Client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_IN_COMBAT) = UnitFlags.UNIT_FLAG_IN_COMBAT) _
                        Then
                        If _
                            (CType(SPELLs(itemInfo.Spells(i).SpellID), SpellInfo).Attributes And
                             SpellAttributes.SPELL_ATTR_NOT_WHILE_COMBAT) Then
                            SendInventoryChangeFailure(Client.Character,
                                                       InventoryChangeFailure.EQUIP_ERR_CANT_DO_IN_COMBAT, itemGuid, 0)
                            Exit Sub
                        End If
                    End If
                End If
            Next

            If Client.Character.DEAD = True Then
                SendInventoryChangeFailure(Client.Character, InventoryChangeFailure.EQUIP_ERR_YOU_ARE_DEAD, itemGuid, 0)
                Exit Sub
            End If

            If itemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_CONSUMABLE Then
                'DONE: Bind item to player
                If _
                    WORLD_ITEMs(itemGuid).ItemInfo.Bonding = ITEM_BONDING_TYPE.BIND_WHEN_USED AndAlso
                    WORLD_ITEMs(itemGuid).IsSoulBound = False Then WORLD_ITEMs(itemGuid).SoulbindItem(Client)
            End If

            'DONE: Read spell targets
            Dim targets As New SpellTargets
            targets.ReadTargets(packet, Client.Character)

            For i As Byte = 0 To 4
                If _
                    itemInfo.Spells(i).SpellID > 0 AndAlso
                    (itemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE OrElse
                     itemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.NO_DELAY_USE) Then
                    If SPELLs.ContainsKey(itemInfo.Spells(i).SpellID) Then
                        'DONE: If there's no more charges
                        If itemInfo.Spells(i).SpellCharges > 0 AndAlso WORLD_ITEMs(itemGuid).ChargesLeft = 0 Then
                            SendCastResult(SpellFailedReason.SPELL_FAILED_NO_CHARGES_REMAIN, Client,
                                           itemInfo.Spells(i).SpellID)
                            Exit Sub
                        End If

                        Dim tmpSpell As _
                                New CastSpellParameters(targets, Client.Character, itemInfo.Spells(i).SpellID,
                                                        WORLD_ITEMs(itemGuid), InstantCast)

                        Dim castResult As Byte = SpellFailedReason.SPELL_NO_ERROR
                        Try
                            castResult = SPELLs(itemInfo.Spells(i).SpellID).CanCast(Client.Character, targets, True)

                            'Only instant cast send ERR_OK for cast result?
                            If castResult = SpellFailedReason.SPELL_NO_ERROR Then
                                'DONE: Enqueue spell casting function
                                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf tmpSpell.Cast))
                            Else
                                SendCastResult(castResult, Client, itemInfo.Spells(i).SpellID)
                            End If

                        Catch e As Exception
                            Log.WriteLine(BaseWriter.LogType.DEBUG, "Error casting spell {0}.{1}",
                                          itemInfo.Spells(i).SpellID, vbNewLine & e.ToString)
                            SendCastResult(castResult, Client, itemInfo.Spells(i).SpellID)
                        End Try
                        Exit Sub
                    End If
                End If
            Next

        Catch ex As Exception
            Log.WriteLine(BaseWriter.LogType.CRITICAL, "Error while using a item.{0}", vbNewLine & ex.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_OPEN_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 7 Then Exit Sub
        packet.GetInt16()
        Dim bag As Byte = packet.GetInt8
        If bag = 255 Then bag = 0
        Dim slot As Byte = packet.GetInt8

        Log.WriteLine(BaseWriter.LogType.DEBUG, "[{0}:{1}] CMSG_OPEN_ITEM [bag={2} slot={3}]", Client.IP, Client.Port,
                      bag, slot)

        Dim itemGuid As ULong = 0
        If bag = 0 Then
            itemGuid = Client.Character.Items(slot).GUID
        Else
            itemGuid = Client.Character.Items(bag).Items(slot).GUID
        End If
        If itemGuid = 0 OrElse WORLD_ITEMs.ContainsKey(itemGuid) = False Then Exit Sub

        If WORLD_ITEMs(itemGuid).GenerateLoot Then
            LootTable(itemGuid).SendLoot(Client)
            Exit Sub
        End If

        SendEmptyLoot(itemGuid, LootType.LOOTTYPE_CORPSE, Client)
    End Sub


    Public Sub SendInventoryChangeFailure(ByRef c As CharacterObject, ByVal errorCode As InventoryChangeFailure,
                                          ByVal guid1 As ULong, ByVal guid2 As ULong)
        Dim packet As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
        packet.AddInt8(errorCode)

        If errorCode = InventoryChangeFailure.EQUIP_ERR_YOU_MUST_REACH_LEVEL_N Then
            packet.AddInt32(WORLD_ITEMs(guid1).ItemInfo.ReqLevel)
        End If

        packet.AddUInt64(guid1)
        packet.AddUInt64(GUID2)
        packet.AddInt8(0)
        c.Client.Send(packet)
        packet.Dispose()
    End Sub

    'Public Sub SendEnchantmentLog(ByRef c As CharacterObject, ByVal iGUID As ULong, ByVal iEntry As Integer,
    '                              ByVal iSpellID As Integer)
    '    Dim packet As New PacketClass(OPCODES.SMSG_ENCHANTMENTLOG)
    '    packet.AddUInt64(iGUID)
    '    packet.AddUInt64(c.GUID)
    '    packet.AddInt32(iEntry)
    '    packet.AddInt32(iSpellID)
    '    packet.AddInt8(0)
    '    c.Client.Send(packet)
    '    packet.Dispose()
    'End Sub

    'Public Sub SendEnchantmentTimeUpdate(ByRef c As CharacterObject, ByVal iGUID As ULong, ByVal iSlot As Integer,
    '                                     ByVal iTime As Integer)
    '    Dim packet As New PacketClass(OPCODES.SMSG_ITEM_ENCHANT_TIME_UPDATE)
    '    packet.AddUInt64(iGUID)
    '    packet.AddInt32(iSlot)
    '    packet.AddInt32(iTime)
    '    c.Client.Send(packet)
    '    packet.Dispose()
    'End Sub


#End Region
End Module
