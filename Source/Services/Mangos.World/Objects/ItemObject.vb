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

'WARNING: Use only with ITEMs()
Imports System.Data
Imports System.Runtime.CompilerServices
Imports Mangos.Common
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Item
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Enums.Spell
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Loots
Imports Mangos.World.Player
Imports Mangos.World.Server
Imports Mangos.World.Spells

Namespace Objects

    Public NotInheritable Class ItemObject
        Implements IDisposable

        Public ReadOnly Property ItemInfo() As WS_Items.ItemInfo
            Get
                Return _WorldServer.ITEMDatabase(ItemEntry)
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
        Public ReadOnly Enchantments As New Dictionary(Of Byte, WS_Items.TEnchantmentInfo)

        Private _loot As WS_Loot.LootObject = Nothing

        'WARNING: Containers cannot hold itemText value
        Public ItemText As Integer = 0

        <MethodImpl(MethodImplOptions.Synchronized)>
        Private Function GetNewGUID() As ULong
            _WorldServer.itemGuidCounter += 1
            GetNewGUID = _WorldServer.itemGuidCounter
        End Function


        Public Sub FillAllUpdateFlags(ByRef update As Packets.UpdateClass)
            If ItemInfo.ContainerSlots > 0 Then
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_CONTAINER + ObjectType.TYPE_OBJECT)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ItemEntry)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 1.0F)

                update.SetUpdateFlag(EItemFields.ITEM_FIELD_OWNER, OwnerGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_CONTAINED, OwnerGUID)
                If CreatorGUID > 0 Then update.SetUpdateFlag(EItemFields.ITEM_FIELD_CREATOR, CreatorGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_GIFTCREATOR, GiftCreatorGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_STACK_COUNT, StackCount)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURATION, 0)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_ITEM_TEXT_ID, ItemText)

                update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_NUM_SLOTS, ItemInfo.ContainerSlots)
                'DONE: Here list in bag items
                For i As Byte = 0 To 35
                    If Items.ContainsKey(i) Then
                        update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_SLOT_1 + i * 2, CType(Items(i).GUID, Long))
                    Else
                        update.SetUpdateFlag(EContainerFields.CONTAINER_FIELD_SLOT_1 + i * 2, 0)
                    End If
                Next
            Else
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_ITEM + ObjectType.TYPE_OBJECT)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, ItemEntry)
                update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 1.0F)

                update.SetUpdateFlag(EItemFields.ITEM_FIELD_OWNER, OwnerGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_CONTAINED, OwnerGUID)
                If CreatorGUID > 0 Then update.SetUpdateFlag(EItemFields.ITEM_FIELD_CREATOR, CreatorGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_GIFTCREATOR, GiftCreatorGUID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_STACK_COUNT, StackCount)
                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURATION, 0)
                For i As Integer = 0 To 4
                    If _
                        ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE OrElse
                        ItemInfo.Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.NO_DELAY_USE Then
                        update.SetUpdateFlag(EItemFields.ITEM_FIELD_SPELL_CHARGES + i, ChargesLeft)
                    Else
                        update.SetUpdateFlag(EItemFields.ITEM_FIELD_SPELL_CHARGES + i, -1)
                    End If
                Next
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)

                'Update.SetUpdateFlag(EItemFields.ITEM_FIELD_PROPERTY_SEED, 0)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_RANDOM_PROPERTIES_ID, RandomProperties)

                For Each enchant As KeyValuePair(Of Byte, WS_Items.TEnchantmentInfo) In Enchantments
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3, enchant.Value.ID)
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3 + 1, enchant.Value.Duration)
                    update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3 + 2, enchant.Value.Charges)
                Next

                update.SetUpdateFlag(EItemFields.ITEM_FIELD_ITEM_TEXT_ID, ItemText)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURABILITY, Durability)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_MAXDURABILITY, _WorldServer.ITEMDatabase(ItemEntry).Durability)
            End If
        End Sub

        Public Sub SendContainedItemsUpdate(ByRef client As WS_Network.ClientClass,
                                            Optional ByVal updatetype As Integer =
                                               ObjectUpdateType.UPDATETYPE_CREATE_OBJECT)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(Items.Count)      'Operations.Count
            packet.AddInt8(0)

            For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_ITEM)
                item.Value.FillAllUpdateFlags(tmpUpdate)
                tmpUpdate.AddToPacket(packet, updatetype, item.Value)
                tmpUpdate.Dispose()
            Next

            client.Send(packet)
        End Sub

        Private Sub InitializeBag()
            If _WorldServer.ITEMDatabase(ItemEntry).IsContainer Then
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
        '        If Items.Count = _WorldServer.ITEMDatabase(ItemEntry).ContainerSlots Then Return True Else Return False
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
                If _WorldServer.CHARACTERs.ContainsKey(OwnerGUID) = False Then Return 255

                With _WorldServer.CHARACTERs(OwnerGUID)
                    For i As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_END - 1
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
                If _WorldServer.CHARACTERs.ContainsKey(OwnerGUID) = False Then Return -1

                With _WorldServer.CHARACTERs(OwnerGUID)
                    For i As Byte = EquipmentSlots.EQUIPMENT_SLOT_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END - 1
                        If .Items.ContainsKey(i) Then
                            If .Items(i) Is Me Then Return i
                        End If
                    Next

                    For i As Byte = KeyRingSlots.KEYRING_SLOT_START To KeyRingSlots.KEYRING_SLOT_END - 1
                        If .Items.ContainsKey(i) Then
                            If .Items(i) Is Me Then Return i
                        End If
                    Next

                    For i As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                        If .Items.ContainsKey(i) Then
                            For j As Byte = 0 To .Items(i).ItemInfo.ContainerSlots - 1
                                If .Items(i).Items.ContainsKey(j) Then
                                    If .Items(i).Items(j) Is Me Then Return j
                                End If
                            Next
                        End If
                    Next
                End With

                Return -1
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
            _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM item_loot WHERE entry = {0};", ItemEntry), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then Return False

            _loot = New WS_Loot.LootObject(GUID, LootType.LOOTTYPE_CORPSE)
            Dim template As WS_Loot.LootTemplate = _WS_Loot.LootTemplates_Item.GetLoot(ItemEntry)
            If template IsNot Nothing Then
                template.Process(_loot, 0)
            End If

            _loot.LootOwner = 0

            Return True
        End Function

        Public Sub New(ByVal guidVal As ULong, Optional ByVal owner As WS_PlayerData.CharacterObject = Nothing,
                       Optional ByVal equipped As Boolean = False)
            'DONE: Get from SQLDB
            Dim mySqlQuery As New DataTable
            _WorldServer.CharacterDatabase.Query(
                String.Format("SELECT * FROM characters_inventory WHERE item_guid = ""{0}"";", guidVal), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then _
                Err.Raise(1, "ItemObject.New", String.Format("itemGuid {0} not found in SQL database!", guidVal))

            GUID = mySqlQuery.Rows(0).Item("item_guid") + _Global_Constants.GUID_ITEM
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
                        Enchantments.Add(tmp2(0), New WS_Items.TEnchantmentInfo(tmp2(1), tmp2(2), tmp2(3)))
                        'DONE: Add the bonuses to the character
                        If equipped Then AddEnchantBonus(tmp2(0), owner)
                    End If
                Next i
            End If

            'DONE: Load ItemID in cashe if not loaded
            If _WorldServer.ITEMDatabase.ContainsKey(ItemEntry) = False Then
                'TODO: This needs to actually do something
                Dim tmpItem As New WS_Items.ItemInfo(ItemEntry)
            End If

            InitializeBag()

            'DONE: Get Items
            mySqlQuery.Clear()
            _WorldServer.CharacterDatabase.Query(String.Format("SELECT * FROM characters_inventory WHERE item_bag = {0};", GUID),
                                    mySqlQuery)
            For Each row As DataRow In mySqlQuery.Rows
                If row.Item("item_slot") <> _Global_Constants.ITEM_SLOT_NULL Then
                    Dim tmpItem As New ItemObject(CType(row.Item("item_guid"), Long))
                    Items(row.Item("item_slot")) = tmpItem
                End If
            Next

            _WorldServer.WORLD_ITEMs.Add(GUID, Me)
        End Sub

        Public Sub New(ByVal itemId As Integer, ByVal owner As ULong)
            'DONE: Load ItemID in cashe if not loaded
            Try
                If _WorldServer.ITEMDatabase.ContainsKey(itemId) = False Then
                    'TODO: This needs to actually do something
                    Dim tmpItem As New WS_Items.ItemInfo(itemId)
                End If
                ItemEntry = itemId
                OwnerGUID = owner
                Durability = _WorldServer.ITEMDatabase(ItemEntry).Durability

                For i As Integer = 0 To 4
                    If _
                        _WorldServer.ITEMDatabase(ItemEntry).Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.USE OrElse
                        _WorldServer.ITEMDatabase(ItemEntry).Spells(i).SpellTrigger = ITEM_SPELLTRIGGER_TYPE.NO_DELAY_USE Then
                        If _WorldServer.ITEMDatabase(ItemEntry).Spells(i).SpellCharges <> 0 Then
                            ChargesLeft = _WorldServer.ITEMDatabase(ItemEntry).Spells(i).SpellCharges
                            Exit For
                        End If
                    End If
                Next i

                'DONE: Create new GUID
                GUID = GetNewGUID()
                InitializeBag()
                SaveAsNew()

                _WorldServer.WORLD_ITEMs.Add(GUID, Me)
            Catch Ex As Exception
                _WorldServer.Log.WriteLine(LogType.WARNING, "Duplicate Key Warning ITEMID:{0} OWNERGUID:{1}", itemId, owner)
            End Try
        End Sub

        Private Sub SaveAsNew()
            'DONE: Save to SQL
            Dim tmpCmd As String = "INSERT INTO characters_inventory (item_guid"
            Dim tmpValues As String = " VALUES (" & GUID - _Global_Constants.GUID_ITEM
            tmpCmd &= ", item_owner"
            tmpValues = tmpValues & ", """ & OwnerGUID & """"
            tmpCmd &= ", item_creator"
            tmpValues = tmpValues & ", " & CreatorGUID
            tmpCmd &= ", item_giftCreator"
            tmpValues = tmpValues & ", " & GiftCreatorGUID
            tmpCmd &= ", item_stackCount"
            tmpValues = tmpValues & ", " & StackCount
            tmpCmd &= ", item_durability"
            tmpValues = tmpValues & ", " & Durability
            tmpCmd &= ", item_chargesLeft"
            tmpValues = tmpValues & ", " & ChargesLeft
            tmpCmd &= ", item_random_properties"
            tmpValues = tmpValues & ", " & RandomProperties
            tmpCmd &= ", item_id"
            tmpValues = tmpValues & ", " & ItemEntry
            tmpCmd &= ", item_flags"
            tmpValues = tmpValues & ", " & _flags

            'DONE: Saving enchanments
            Dim temp As New ArrayList
            For Each enchantment As KeyValuePair(Of Byte, WS_Items.TEnchantmentInfo) In Enchantments
                temp.Add(String.Format("{0}:{1}:{2}:{3}", enchantment.Key, enchantment.Value.ID,
                                       enchantment.Value.Duration, enchantment.Value.Charges))
            Next
            tmpCmd &= ", item_enchantment"
            tmpValues = tmpValues & ", '" & Join(temp.ToArray, " ") & "'"
            tmpCmd &= ", item_textId"
            tmpValues = tmpValues & ", " & ItemText

            tmpCmd = tmpCmd & ") " & tmpValues & ");"
            _WorldServer.CharacterDatabase.Update(tmpCmd)
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
            For Each enchantment As KeyValuePair(Of Byte, WS_Items.TEnchantmentInfo) In Enchantments
                temp.Add(String.Format("{0}:{1}:{2}:{3}", enchantment.Key, enchantment.Value.ID,
                                       enchantment.Value.Duration, enchantment.Value.Charges))
            Next
            tmp = tmp & ", item_enchantment=""" & Join(temp.ToArray, " ") & """"
            tmp = tmp & ", item_textId=" & ItemText

            tmp = tmp & " WHERE item_guid = """ & (GUID - _Global_Constants.GUID_ITEM) & """;"

            _WorldServer.CharacterDatabase.Update(tmp)

            If _WorldServer.ITEMDatabase(ItemEntry).IsContainer() AndAlso saveAll Then
                For Each item As KeyValuePair(Of Byte, ItemObject) In Items
                    item.Value.Save()
                Next
            End If
        End Sub

        Public Sub Delete()
            'DONE: Check if item is petition
            If _
                ItemEntry = _Global_Constants.PETITION_GUILD Then _
                _WorldServer.CharacterDatabase.Update("DELETE FROM petitions WHERE petition_itemGuid = " & GUID - _Global_Constants.GUID_ITEM & ";")
            _WorldServer.CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_guid = {0}", GUID - _Global_Constants.GUID_ITEM))

            If _WorldServer.ITEMDatabase(ItemEntry).IsContainer() Then
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
                _WorldServer.WORLD_ITEMs.Remove(GUID)

                If _WorldServer.ITEMDatabase(ItemEntry).IsContainer() Then
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

        Public Sub ModifyDurability(ByVal percent As Single, ByRef client As WS_Network.ClientClass)
            If _WorldServer.ITEMDatabase(ItemEntry).Durability > 0 Then
                Durability -= Fix(_WorldServer.ITEMDatabase(ItemEntry).Durability * percent)
                If Durability < 0 Then Durability = 0
                If Durability > _WorldServer.ITEMDatabase(ItemEntry).Durability Then Durability = _WorldServer.ITEMDatabase(ItemEntry).Durability
                UpdateDurability(client)
            End If
        End Sub

        Public Sub ModifyToDurability(ByVal percent As Single, ByRef client As WS_Network.ClientClass)
            If _WorldServer.ITEMDatabase(ItemEntry).Durability > 0 Then
                Durability = Fix(_WorldServer.ITEMDatabase(ItemEntry).Durability * percent)
                If Durability < 0 Then Durability = 0
                If Durability > _WorldServer.ITEMDatabase(ItemEntry).Durability Then Durability = _WorldServer.ITEMDatabase(ItemEntry).Durability
                UpdateDurability(client)
            End If
        End Sub

        Private Sub UpdateDurability(ByRef client As WS_Network.ClientClass)
            Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)      'Operations.Count
            packet.AddInt8(0)
            Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_ITEM)
            tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_DURABILITY, Durability)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)
            tmpUpdate.Dispose()
            client.Send(packet)
        End Sub

        Public ReadOnly Property GetDurabulityCost() As UInteger
            Get
                Try
                    Dim lostDurability As Integer = _WorldServer.ITEMDatabase(ItemEntry).Durability - Durability
                    If lostDurability > _WS_DBCDatabase.DurabilityCosts_MAX Then lostDurability = _WS_DBCDatabase.DurabilityCosts_MAX
                    Dim subClass As Integer = 0
                    If ItemInfo.ObjectClass = ITEM_CLASS.ITEM_CLASS_WEAPON Then subClass = ItemInfo.SubClass Else _
                        subClass = ItemInfo.SubClass + 21
                    Dim durabilityCost As UInteger =
                            (lostDurability * ((_WS_DBCDatabase.DurabilityCosts(ItemInfo.Level, subClass) / 40) * 100))
                    _WorldServer.Log.WriteLine(LogType.DEBUG, "Durability cost: {0}", durabilityCost)
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
            Enchantments.Add(slot, New WS_Items.TEnchantmentInfo(id, duration, charges))
            'DONE: Add the bonuses to the character if it's equipped
            AddEnchantBonus(slot)
        End Sub

        Public Sub AddEnchantBonus(ByVal slot As Byte, Optional ByRef objCharacter As WS_PlayerData.CharacterObject = Nothing)
            If objCharacter Is Nothing Then
                If _WorldServer.CHARACTERs.ContainsKey(OwnerGUID) = False Then Exit Sub
                objCharacter = _WorldServer.CHARACTERs(OwnerGUID)
            End If
            If objCharacter IsNot Nothing AndAlso _WS_DBCDatabase.SpellItemEnchantments.ContainsKey(Enchantments(slot).ID) Then
                For i As Byte = 0 To 2
                    If _WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i) <> 0 Then
                        If _WS_Spells.SPELLs.ContainsKey(_WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i)) Then
                            Dim spellInfo As WS_Spells.SpellInfo
                            spellInfo = _WS_Spells.SPELLs(_WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i))
                            For j As Byte = 0 To 2
                                If Not (spellInfo.SpellEffects(j) Is Nothing) Then
                                    Select Case spellInfo.SpellEffects(j).ID
                                        Case SpellEffects_Names.SPELL_EFFECT_APPLY_AURA
                                            _WS_Spells.AURAs(spellInfo.SpellEffects(j).ApplyAuraIndex).Invoke(objCharacter, objCharacter,
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
            If _WorldServer.CHARACTERs.ContainsKey(OwnerGUID) AndAlso _WS_DBCDatabase.SpellItemEnchantments.ContainsKey(Enchantments(slot).ID) Then
                For i As Byte = 0 To 2
                    If _WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i) <> 0 Then
                        If _WS_Spells.SPELLs.ContainsKey(_WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i)) Then
                            Dim spellInfo As WS_Spells.SpellInfo
                            spellInfo = _WS_Spells.SPELLs(_WS_DBCDatabase.SpellItemEnchantments(Enchantments(slot).ID).SpellID(i))
                            For j As Byte = 0 To 2
                                If Not (spellInfo.SpellEffects(j) Is Nothing) Then
                                    Select Case spellInfo.SpellEffects(j).ID
                                        Case SpellEffects_Names.SPELL_EFFECT_APPLY_AURA
                                            _WS_Spells.AURAs(spellInfo.SpellEffects(j).ApplyAuraIndex).Invoke(_WorldServer.CHARACTERs(OwnerGUID),
                                                                                                   _WorldServer.CHARACTERs(OwnerGUID),
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
            If Enchantments.ContainsKey(slot) = False Then Exit Sub
            'DONE: Remove the bonuses from the character
            RemoveEnchantBonus(slot)
            'DONE: Remove the enchant
            Enchantments.Remove(slot)
            'DONE: Send the update to the client about it
            If _WorldServer.CHARACTERs.ContainsKey(OwnerGUID) Then
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_ITEM)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3), 0)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3) + 1, 0)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3) + 2, 0)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

                _WorldServer.CHARACTERs(OwnerGUID).client.Send(packet)
                packet.Dispose()
                tmpUpdate.Dispose()
            End If
        End Sub

        Public Sub SoulbindItem(Optional ByRef client As WS_Network.ClientClass = Nothing)
            If (_flags And ITEM_FLAGS.ITEM_FLAGS_BINDED) = ITEM_FLAGS.ITEM_FLAGS_BINDED Then Exit Sub

            'DONE: Setting the flag
            _flags = _flags Or ITEM_FLAGS.ITEM_FLAGS_BINDED
            Save()

            'DONE: Sending update to character
            If Not client Is Nothing Then
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)      'Operations.Count
                packet.AddInt8(0)

                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_ITEM)
                tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_FLAGS, _flags)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

                client.Send(packet)
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
End Namespace