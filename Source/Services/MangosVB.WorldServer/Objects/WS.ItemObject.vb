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

'WARNING: Use only with ITEMs()

Imports System.Data
Imports MangosVB.Common
Imports System.Runtime.CompilerServices
Imports MangosVB.Common.Globals
Imports MangosVB.Shared

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

    <MethodImpl(MethodImplOptions.Synchronized)>
    Private Function GetNewGUID() As ULong
        itemGuidCounter += 1
        GetNewGUID = itemGuidCounter
    End Function


    Public Sub FillAllUpdateFlags(ByRef update As UpdateClass)
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

            For Each enchant As KeyValuePair(Of Byte, TEnchantmentInfo) In Enchantments
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3, enchant.Value.ID)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3 + 1, enchant.Value.Duration)
                update.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + enchant.Key * 3 + 2, enchant.Value.Charges)
            Next

            update.SetUpdateFlag(EItemFields.ITEM_FIELD_ITEM_TEXT_ID, ItemText)
            update.SetUpdateFlag(EItemFields.ITEM_FIELD_DURABILITY, Durability)
            update.SetUpdateFlag(EItemFields.ITEM_FIELD_MAXDURABILITY, ITEMDatabase(ItemEntry).Durability)
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
            If CHARACTERs.ContainsKey(OwnerGUID) = False Then Return -1

            With CHARACTERs(OwnerGUID)
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
                Items(row.Item("item_slot")) = tmpItem
            End If
        Next

        WORLD_ITEMs.Add(GUID, Me)
    End Sub

    Public Sub New(ByVal itemId As Integer, ByVal owner As ULong)
        'DONE: Load ItemID in cashe if not loaded
        Try
            If ITEMDatabase.ContainsKey(itemId) = False Then
                'TODO: This needs to actually do something
                Dim tmpItem As New ItemInfo(itemId)
            End If
            ItemEntry = itemId
            OwnerGUID = owner
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
        Catch Ex As Exception
            Log.WriteLine(LogType.WARNING, "Duplicate Key Warning ITEMID:{0} OWNERGUID:{1}", itemId, owner)
        End Try
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
            ItemEntry = PETITION_GUILD Then _
            CharacterDatabase.Update("DELETE FROM petitions WHERE petition_itemGuid = " & GUID - GUID_ITEM & ";")
        CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_guid = {0}", GUID - GUID_ITEM))

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
            Durability -= Fix(ITEMDatabase(ItemEntry).Durability * percent)
            If Durability < 0 Then Durability = 0
            If Durability > ITEMDatabase(ItemEntry).Durability Then Durability = ITEMDatabase(ItemEntry).Durability
            UpdateDurability(client)
        End If
    End Sub

    Public Sub ModifyToDurability(ByVal percent As Single, ByRef client As ClientClass)
        If ITEMDatabase(ItemEntry).Durability > 0 Then
            Durability = Fix(ITEMDatabase(ItemEntry).Durability * percent)
            If Durability < 0 Then Durability = 0
            If Durability > ITEMDatabase(ItemEntry).Durability Then Durability = ITEMDatabase(ItemEntry).Durability
            UpdateDurability(client)
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
                Log.WriteLine(LogType.DEBUG, "Durability cost: {0}", durabilityCost)
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
        Enchantments.Add(slot, New TEnchantmentInfo(id, duration, charges))
        'DONE: Add the bonuses to the character if it's equipped
        AddEnchantBonus(slot)
    End Sub

    Public Sub AddEnchantBonus(ByVal slot As Byte, Optional ByRef objCharacter As CharacterObject = Nothing)
        If objCharacter Is Nothing Then
            If CHARACTERs.ContainsKey(OwnerGUID) = False Then Exit Sub
            objCharacter = CHARACTERs(OwnerGUID)
        End If
        If objCharacter IsNot Nothing AndAlso SpellItemEnchantments.ContainsKey(Enchantments(slot).ID) Then
            For i As Byte = 0 To 2
                If SpellItemEnchantments(Enchantments(slot).ID).SpellID(i) <> 0 Then
                    If SPELLs.ContainsKey(SpellItemEnchantments(Enchantments(slot).ID).SpellID(i)) Then
                        Dim spellInfo As SpellInfo
                        spellInfo = SPELLs(SpellItemEnchantments(Enchantments(slot).ID).SpellID(i))
                        For j As Byte = 0 To 2
                            If Not (spellInfo.SpellEffects(j) Is Nothing) Then
                                Select Case spellInfo.SpellEffects(j).ID
                                    Case SpellEffects_Names.SPELL_EFFECT_APPLY_AURA
                                        AURAs(spellInfo.SpellEffects(j).ApplyAuraIndex).Invoke(objCharacter, objCharacter,
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
        If CHARACTERs.ContainsKey(OwnerGUID) AndAlso SpellItemEnchantments.ContainsKey(Enchantments(slot).ID) Then
            For i As Byte = 0 To 2
                If SpellItemEnchantments(Enchantments(slot).ID).SpellID(i) <> 0 Then
                    If SPELLs.ContainsKey(SpellItemEnchantments(Enchantments(slot).ID).SpellID(i)) Then
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
        If Enchantments.ContainsKey(slot) = False Then Exit Sub
        'DONE: Remove the bonuses from the character
        RemoveEnchantBonus(slot)
        'DONE: Remove the enchant
        Enchantments.Remove(slot)
        'DONE: Send the update to the client about it
        If CHARACTERs.ContainsKey(OwnerGUID) Then
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)      'Operations.Count
            packet.AddInt8(0)

            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
            tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3), 0)
            tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3) + 1, 0)
            tmpUpdate.SetUpdateFlag(EItemFields.ITEM_FIELD_ENCHANTMENT + (slot * 3) + 2, 0)
            tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

            CHARACTERs(OwnerGUID).client.Send(packet)
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
        If Not client Is Nothing Then
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            packet.AddInt32(1)      'Operations.Count
            packet.AddInt8(0)

            Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_ITEM)
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
