'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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
Imports System.Data
Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Globals
Imports Mangos.World.Auction
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Handlers
Imports Mangos.World.Player
Imports Mangos.World.Quests
Imports Mangos.World.Server
Imports Mangos.World.Social
Imports Mangos.World.Spells

Namespace Objects

    Public Module WS_NPCs
        Private Const DbcBankBagSlotsMax As Integer = 12
        Private ReadOnly DbcBankBagSlotPrices(DbcBankBagSlotsMax) As Integer

#Region "Trainers"
        ''' <summary>
        ''' Called when [CMSG_TRAINER_LIST] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_TRAINER_LIST(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong
            guid = packet.GetUInt64

            Log.WriteLine(GlobalEnum.LogType.DEBUG, "[{0}:{1}] CMSG_TRAINER_LIST [GUID={2}]", client.IP, client.Port, guid)

            SendTrainerList(client.Character, guid)
        End Sub

        ''' <summary>
        ''' Called when [CMSG_TRAINER_BUY_SPELL] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_TRAINER_BUY_SPELL(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 17 Then Exit Sub
            packet.GetInt16()
            Dim cGuid As ULong
            cGuid = packet.GetUInt64
            Dim spellID As Integer
            spellID = packet.GetInt32

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TRAINER_BUY_SPELL [GUID={2} Spell={3}]", client.IP, client.Port, cGuid, spellID)
            If WORLD_CREATUREs.ContainsKey(cGuid) = False OrElse (WORLD_CREATUREs(cGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TRAINER) = 0 Then Exit Sub

            Dim mySqlQuery As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM npc_trainer WHERE entry = {0} AND spell = {1};", WORLD_CREATUREs(cGuid).ID, spellID), mySqlQuery)
            If mySqlQuery.Rows.Count = 0 Then Exit Sub

            Dim spellInfo As WS_Spells.SpellInfo
            spellInfo = WS_Spells.SPELLs(spellID)
            If spellInfo.SpellEffects(0) IsNot Nothing AndAlso spellInfo.SpellEffects(0).TriggerSpell > 0 Then spellInfo = WS_Spells.SPELLs(spellInfo.SpellEffects(0).TriggerSpell)

            'DONE: Check requirements
            Dim reqLevel As Byte
            reqLevel = mySqlQuery.Rows(0).Item("reqlevel")
            If reqLevel = 0 Then reqLevel = spellInfo.spellLevel
            Dim spellCost As UInteger
            spellCost = mySqlQuery.Rows(0).Item("spellcost")
            Dim reqSpell As Integer
            reqSpell = 0
            If SpellChains.ContainsKey(spellInfo.ID) Then
                reqSpell = SpellChains(spellInfo.ID)
            End If

            If client.Character.HaveSpell(spellInfo.ID) Then Exit Sub
            If client.Character.Copper < spellCost Then Exit Sub
            If client.Character.Level < reqLevel Then Exit Sub
            If reqSpell > 0 AndAlso client.Character.HaveSpell(reqSpell) = False Then Exit Sub
            If CInt(mySqlQuery.Rows(0).Item("reqskill")) > 0 AndAlso client.Character.HaveSkill(mySqlQuery.Rows(0).Item("reqskill"), mySqlQuery.Rows(0).Item("reqskillvalue")) = False Then Exit Sub

            'TODO: Check proffessions - only alowed to learn 2!
            Try
                'DONE: Get the money
                client.Character.Copper -= spellCost
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                client.Character.SendCharacterUpdate(False)

                'DONE: Cast the spell
                Dim spellTargets As New SpellTargets
                spellTargets.SetTarget_UNIT(client.Character)

                Dim tmpCaster As BaseUnit
                If spellInfo.SpellVisual = 222 Then
                    tmpCaster = client.Character
                Else
                    tmpCaster = WORLD_CREATUREs(cGuid)
                End If

                Dim castParams As New CastSpellParameters(spellTargets, tmpCaster, spellID, True)
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))

                WORLD_CREATUREs(cGuid).MoveToInstant(WORLD_CREATUREs(cGuid).positionX, WORLD_CREATUREs(cGuid).positionY, WORLD_CREATUREs(cGuid).positionZ, WORLD_CREATUREs(cGuid).SpawnO)

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Training Spell Error: Unable to cast spell. [{0}:{1}]", Environment.NewLine, e.ToString)

                'TODO: Fix this opcode
                'Dim errorPacket As New PacketClass(OPCODES.SMSG_TRAINER_BUY_FAILED)
                'errorPacket.AddUInt64(cGUID)
                'errorPacket.AddInt32(SpellID)
                'Client.Send(errorPacket)
                'errorPacket.Dispose()
            End Try

            'DONE: Send response
            Dim response As PacketClass
            response = New PacketClass(OPCODES.SMSG_TRAINER_BUY_SUCCEEDED)
            Try
                response.AddUInt64(cGuid)
                response.AddInt32(spellID)
                client.Send(response)
            Finally
                response.Dispose()
            End Try
        End Sub

        ''' <summary>
        ''' Sends the trainer list.
        ''' </summary>
        ''' <param name="objCharacter">The objCharacter.</param>
        ''' <param name="cGuid">The objCharacter GUID.</param>
        ''' <returns></returns>
        Private Sub SendTrainerList(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal cGuid As ULong)
            'DONE: Query the database and sort spells
            'Dim NeedToLearn As Boolean = False
            'Dim noTrainID As Integer = 0
            Dim spellSqlQuery As DataTable
            spellSqlQuery = New DataTable
            'Dim npcTextSQLQuery As New DataTable
            Dim creatureInfo As CreatureInfo
            creatureInfo = WORLD_CREATUREs(cGuid).CreatureInfo
            Dim spellsList As List(Of DataRow)
            spellsList = New List(Of DataRow)

            If (creatureInfo.Classe = 0 OrElse creatureInfo.Classe = objCharacter.Classe) AndAlso (creatureInfo.Race = 0 OrElse (creatureInfo.Race = objCharacter.Race OrElse objCharacter.GetReputation(creatureInfo.Faction) = ReputationRank.Exalted)) Then
                WorldDatabase.Query(String.Format("SELECT * FROM npc_trainer WHERE entry = {0};", WORLD_CREATUREs(cGuid).ID), spellSqlQuery)

                For Each sellRow As DataRow In spellSqlQuery.Rows
                    spellsList.Add(sellRow)
                Next
            End If

            'DONE: Build the packet
            Dim packet As New PacketClass(OPCODES.SMSG_TRAINER_LIST)

            packet.AddUInt64(cGuid)
            packet.AddInt32(creatureInfo.TrainerType)
            packet.AddInt32(spellsList.Count)              'Trains Length

            'DONE: Discount on reputation
            Dim discountMod As Single = objCharacter.GetDiscountMod(WORLD_CREATUREs(cGuid).Faction)

            Dim spellID As Integer
            For Each sellRow As DataRow In spellsList
                spellID = sellRow.Item("spell")
                If WS_Spells.SPELLs.ContainsKey(spellID) = False Then Continue For
                Dim spellInfo As SpellInfo = WS_Spells.SPELLs(spellID)
                If spellInfo.SpellEffects(0) IsNot Nothing AndAlso spellInfo.SpellEffects(0).TriggerSpell > 0 Then spellInfo = WS_Spells.SPELLs(spellInfo.SpellEffects(0).TriggerSpell)

                Dim reqSpell As Integer = 0
                If SpellChains.ContainsKey(spellInfo.ID) Then
                    reqSpell = SpellChains(spellInfo.ID)
                End If

                Dim spellLevel As Byte = sellRow.Item("reqlevel")
                If spellLevel = 0 Then spellLevel = spellInfo.spellLevel

                'CanLearn (0):Green (1):Red (2):Gray
                Dim canLearnFlag As Byte = 0
                If objCharacter.HaveSpell(spellInfo.ID) Then
                    'NOTE: Already have that spell
                    canLearnFlag = 2

                ElseIf objCharacter.Level >= spellLevel Then

                    If reqSpell > 0 AndAlso objCharacter.HaveSpell(reqSpell) = False Then
                        canLearnFlag = 1
                    End If

                    If canLearnFlag = 0 AndAlso (CType(sellRow.Item("reqskill"), Integer) <> 0) Then
                        If (CType(sellRow.Item("reqskillvalue"), Integer) <> 0) Then
                            If objCharacter.HaveSkill(sellRow.Item("reqskill"), sellRow.Item("reqskillvalue")) = False Then
                                canLearnFlag = 1
                            End If
                        End If
                    End If
                Else
                    'NOTE: Doesn't meet requirements, cannot learn that spell
                    canLearnFlag = 1
                End If

                'TODO: Check if the spell is a profession
                Dim isProf As Integer = 0
                If spellInfo.SpellEffects(1) IsNot Nothing AndAlso spellInfo.SpellEffects(1).ID = SpellEffects_Names.SPELL_EFFECT_SKILL_STEP Then
                    isProf = 1
                End If

                packet.AddInt32(spellID) 'SpellID
                packet.AddInt8(canLearnFlag)
                packet.AddInt32(CType(sellRow.Item("spellcost"), Integer) * discountMod)              'SpellCost
                packet.AddInt32(0)
                packet.AddInt32(isProf) 'Profession
                packet.AddInt8(spellLevel)
                packet.AddInt32(sellRow.Item("reqskill"))          'Required Skill
                packet.AddInt32(sellRow.Item("reqskillvalue"))    'Required Skill Value
                packet.AddInt32(reqSpell)          'Required Spell
                packet.AddInt32(0)
                packet.AddInt32(0)
            Next

            packet.AddString("Hello! Ready for some training?") ' Trainer UI message?

            objCharacter.client.Send(packet)
            packet.Dispose()
        End Sub

#End Region
#Region "Merchants"
        ''' <summary>
        ''' Called when [CMSG_LIST_INVENTORY] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_LIST_INVENTORY(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong
            guid = packet.GetUInt64
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LIST_INVENTORY [GUID={2:X}]", client.IP, client.Port, guid)
            If WORLD_CREATUREs.ContainsKey(guid) = False OrElse (WORLD_CREATUREs(guid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_VENDOR) = 0 Then Exit Sub
            If WORLD_CREATUREs(guid).Evade Then Exit Sub

            WORLD_CREATUREs(guid).StopMoving()
            SendListInventory(client.Character, guid)
        End Sub

        ''' <summary>
        ''' Called when [CMSG_SELL_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_SELL_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 22 Then Exit Sub
            packet.GetInt16()
            Dim vendorGuid As ULong
            vendorGuid = packet.GetUInt64
            Dim itemGuid As ULong
            itemGuid = packet.GetUInt64
            Dim count As Byte = packet.GetInt8
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SELL_ITEM [vendorGuid={2:X} itemGuid={3:X} Count={4}]", client.IP, client.Port, vendorGuid, itemGuid, count)

            Try
                If itemGuid = 0 OrElse WORLD_ITEMs.ContainsKey(itemGuid) = False Then
                    Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                    Try
                        okPckt.AddUInt64(vendorGuid)
                        okPckt.AddUInt64(itemGuid)
                        okPckt.AddInt8(SELL_ERROR.SELL_ERR_CANT_FIND_ITEM)
                        client.Send(okPckt)
                    Finally
                        okPckt.Dispose()
                    End Try
                    Exit Sub
                End If
                'DONE: You can't sell someone else's items
                If WORLD_ITEMs(itemGuid).OwnerGUID <> client.Character.GUID Then
                    Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                    Try
                        okPckt.AddUInt64(vendorGuid)
                        okPckt.AddUInt64(itemGuid)
                        okPckt.AddInt8(SELL_ERROR.SELL_ERR_CANT_FIND_ITEM)
                        client.Send(okPckt)
                    Finally
                        okPckt.Dispose()
                    End Try
                    Exit Sub
                End If
                If Not WORLD_CREATUREs.ContainsKey(vendorGuid) Then
                    Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                    Try
                        okPckt.AddUInt64(vendorGuid)
                        okPckt.AddUInt64(itemGuid)
                        okPckt.AddInt8(SELL_ERROR.SELL_ERR_CANT_FIND_VENDOR)
                        client.Send(okPckt)
                    Finally
                        okPckt.Dispose()
                    End Try
                    Exit Sub
                End If
                'DONE: Can't sell quest items
                If (ITEMDatabase(WORLD_ITEMs(itemGuid).ItemEntry).SellPrice = 0) Or (ITEMDatabase(WORLD_ITEMs(itemGuid).ItemEntry).ObjectClass = ITEM_CLASS.ITEM_CLASS_QUEST) Then
                    Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                    Try
                        okPckt.AddUInt64(vendorGuid)
                        okPckt.AddUInt64(itemGuid)
                        okPckt.AddInt8(SELL_ERROR.SELL_ERR_CANT_SELL_ITEM)
                        client.Send(okPckt)
                    Finally
                        okPckt.Dispose()
                    End Try
                    Exit Sub
                End If
                'DONE: Can't cheat and sell items that are located in the buyback
                For i As Byte = BuyBackSlots.BUYBACK_SLOT_START To BuyBackSlots.BUYBACK_SLOT_END - 1
                    If client.Character.Items.ContainsKey(i) AndAlso client.Character.Items(i).GUID = itemGuid Then
                        Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                        okPckt.AddUInt64(vendorGuid)
                        okPckt.AddUInt64(itemGuid)
                        okPckt.AddInt8(SELL_ERROR.SELL_ERR_CANT_FIND_ITEM)
                        client.Send(okPckt)
                        okPckt.Dispose()
                        Exit Sub
                    End If
                Next

                If count < 1 Then count = WORLD_ITEMs(itemGuid).StackCount
                If WORLD_ITEMs(itemGuid).StackCount > count Then
                    WORLD_ITEMs(itemGuid).StackCount -= count
                    Dim tmpItem As ItemObject = LoadItemByGUID(itemGuid) 'Lets create a new stack to place in the buyback
                    itemGuidCounter += 1 'Get a new GUID for our new stack
                    tmpItem.GUID = itemGuidCounter
                    tmpItem.StackCount = count
                    client.Character.ItemADD_BuyBack(tmpItem)

                    client.Character.Copper += (ITEMDatabase(WORLD_ITEMs(itemGuid).ItemEntry).SellPrice * count)
                    client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                    client.Character.SendItemUpdate(WORLD_ITEMs(itemGuid))
                    WORLD_ITEMs(itemGuid).Save(False)
                Else
                    'DONE: Move item to buyback
                    'TODO: Remove items that expire in the buyback, in mangos it seems like they use 30 hours until it's removed.

                    For Each item As KeyValuePair(Of Byte, ItemObject) In client.Character.Items
                        If item.Value.GUID = itemGuid Then
                            client.Character.Copper += (ITEMDatabase(item.Value.ItemEntry).SellPrice * item.Value.StackCount)
                            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                            If item.Key < InventorySlots.INVENTORY_SLOT_BAG_END Then client.Character.UpdateRemoveItemStats(item.Value, item.Key)

                            client.Character.ItemREMOVE(item.Value.GUID, False, True)
                            client.Character.ItemADD_BuyBack(item.Value)

                            Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                            okPckt.AddUInt64(vendorGuid)
                            okPckt.AddUInt64(itemGuid)
                            okPckt.AddInt8(0)
                            client.Send(okPckt)
                            okPckt.Dispose()
                            Exit Sub
                        End If
                    Next

                    For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_4
                        If client.Character.Items.ContainsKey(bag) Then
                            For Each item As KeyValuePair(Of Byte, ItemObject) In client.Character.Items(bag).Items
                                If item.Value.GUID = itemGuid Then
                                    client.Character.Copper += (ITEMDatabase(item.Value.ItemEntry).SellPrice * item.Value.StackCount)
                                    client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                                    client.Character.ItemREMOVE(item.Value.GUID, False, True)
                                    client.Character.ItemADD_BuyBack(item.Value)

                                    Dim okPckt As New PacketClass(OPCODES.SMSG_SELL_ITEM)
                                    okPckt.AddUInt64(vendorGuid)
                                    okPckt.AddUInt64(itemGuid)
                                    okPckt.AddInt8(0)
                                    client.Send(okPckt)
                                    okPckt.Dispose()
                                    Exit Sub
                                End If
                            Next
                        End If
                    Next
                End If

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error selling item: {0}{1}", Environment.NewLine, e.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BUY_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BUY_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 19 Then Exit Sub
            packet.GetInt16()
            Dim vendorGuid As ULong = packet.GetUInt64
            Dim itemID As Integer = packet.GetInt32
            Dim count As Byte = packet.GetInt8
            Dim slot As Byte = packet.GetInt8       '??
            If WORLD_CREATUREs.ContainsKey(vendorGuid) = False OrElse ((WORLD_CREATUREs(vendorGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_ARMORER) = 0 AndAlso (WORLD_CREATUREs(vendorGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_VENDOR) = 0) Then Exit Sub
            If ITEMDatabase.ContainsKey(itemID) = False Then Exit Sub
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUY_ITEM [vendorGuid={2:X} ItemID={3} Count={4} Slot={5}]", client.IP, client.Port, vendorGuid, itemID, count, slot)

            'TODO: Make sure that the vendor sells the item!

            'DONE: No count cheating
            If count > ITEMDatabase(itemID).Stackable Then count = ITEMDatabase(itemID).Stackable
            If count = 0 Then count = 1

            'DONE: Can't buy quest items
            If ITEMDatabase(itemID).ObjectClass = ITEM_CLASS.ITEM_CLASS_QUEST Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                Try
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(itemID)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_SELLER_DONT_LIKE_YOU)
                    client.Send(errorPckt)
                Finally
                    errorPckt.Dispose()
                End Try
                Exit Sub
            End If

            Dim itemPrice As Integer = 0
            If count * ITEMDatabase(itemID).BuyCount > ITEMDatabase(itemID).Stackable Then count = (ITEMDatabase(itemID).Stackable / ITEMDatabase(itemID).BuyCount)

            'DONE: Reputation discount
            Dim discountMod As Single = client.Character.GetDiscountMod(WORLD_CREATUREs(vendorGuid).Faction)
            itemPrice = ITEMDatabase(itemID).BuyPrice * discountMod
            If client.Character.Copper < (itemPrice * count) Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                Try
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(itemID)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
                    client.Send(errorPckt)
                Finally
                    errorPckt.Dispose()
                End Try
                Exit Sub
            End If

            client.Character.Copper -= (itemPrice * count)
            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

            client.Character.SendCharacterUpdate(False)

            Dim tmpItem As New ItemObject(itemID, client.Character.GUID) With {
                    .StackCount = count * ITEMDatabase(itemID).BuyCount
                    }

            'TODO: Remove one count of the item from the vendor if it's not unlimited

            If Not client.Character.ItemADD(tmpItem) Then
                tmpItem.Delete()
                client.Character.Copper += itemPrice
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
            Else
                Dim okPckt As New PacketClass(OPCODES.SMSG_BUY_ITEM)
                okPckt.AddUInt64(vendorGuid)
                okPckt.AddInt32(itemID)
                okPckt.AddInt32(count)
                client.Send(okPckt)
                okPckt.Dispose()
            End If
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BUY_ITEM_IN_SLOT] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BUY_ITEM_IN_SLOT(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 27 Then Exit Sub
            packet.GetInt16()
            Dim vendorGuid As ULong = packet.GetUInt64
            Dim itemID As Integer = packet.GetInt32
            Dim clientGuid As ULong = packet.GetUInt64
            Dim slot As Byte = packet.GetInt8
            Dim count As Byte = packet.GetInt8
            If WORLD_CREATUREs.ContainsKey(vendorGuid) = False OrElse ((WORLD_CREATUREs(vendorGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_ARMORER) = 0 AndAlso (WORLD_CREATUREs(vendorGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_VENDOR) = 0) Then Exit Sub
            If ITEMDatabase.ContainsKey(itemID) = False Then Exit Sub
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUY_ITEM_IN_SLOT [vendorGuid={2:X} ItemID={3} Count={4} Slot={5}]", client.IP, client.Port, vendorGuid, itemID, count, slot)

            'DONE: No count cheating
            If count > ITEMDatabase(itemID).Stackable Then count = ITEMDatabase(itemID).Stackable

            'DONE: Can't buy quest items
            If ITEMDatabase(itemID).ObjectClass = ITEM_CLASS.ITEM_CLASS_QUEST Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                errorPckt.AddUInt64(vendorGuid)
                errorPckt.AddInt32(itemID)
                errorPckt.AddInt8(BUY_ERROR.BUY_ERR_SELLER_DONT_LIKE_YOU)
                client.Send(errorPckt)
                errorPckt.Dispose()
                Exit Sub
            End If

            Dim itemPrice As Integer = 0

            'DONE: Reputation discount
            Dim discountMod As Single = client.Character.GetDiscountMod(WORLD_CREATUREs(vendorGuid).Faction)
            itemPrice = ITEMDatabase(itemID).BuyPrice * discountMod

            If client.Character.Copper < (itemPrice * count) Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                errorPckt.AddUInt64(vendorGuid)
                errorPckt.AddInt32(itemID)
                errorPckt.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
                client.Send(errorPckt)
                errorPckt.Dispose()
                Exit Sub
            End If

            Dim errCode As Byte = 0
            Dim bag As Byte = 0

            If clientGuid = client.Character.GUID Then
                'Store in inventory
                bag = 0
                If client.Character.Items.ContainsKey(slot) Then
                    Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(itemID)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_CANT_CARRY_MORE)
                    client.Send(errorPckt)
                    errorPckt.Dispose()
                    Exit Sub
                End If
            Else
                'Store in bag
                Dim i As Byte
                For i = InventorySlots.INVENTORY_SLOT_BAG_1 To InventorySlots.INVENTORY_SLOT_BAG_4
                    If client.Character.Items(i).GUID = clientGuid Then
                        bag = i
                        Exit For
                    End If
                Next
                If bag = 0 Then
                    Dim okPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                    okPckt.AddUInt64(vendorGuid)
                    okPckt.AddInt32(itemID)
                    okPckt.AddInt8(BUY_ERROR.BUY_ERR_CANT_FIND_ITEM)
                    client.Send(okPckt)
                    okPckt.Dispose()
                    Exit Sub
                End If
                If client.Character.Items(bag).Items.ContainsKey(slot) Then
                    Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(itemID)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_CANT_CARRY_MORE)
                    client.Send(errorPckt)
                    errorPckt.Dispose()
                    Exit Sub
                End If
            End If

            Dim tmpItem As New ItemObject(itemID, client.Character.GUID) With {
                    .StackCount = count
                    }

            errCode = client.Character.ItemCANEQUIP(tmpItem, bag, slot)
            If errCode <> InventoryChangeFailure.EQUIP_ERR_OK Then
                If errCode <> InventoryChangeFailure.EQUIP_ERR_YOU_MUST_REACH_LEVEL_N Then
                    Dim errorPckt As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    errorPckt.AddInt8(errCode)
                    errorPckt.AddUInt64(0)
                    errorPckt.AddUInt64(0)
                    errorPckt.AddInt8(0)
                    client.Send(errorPckt)
                    errorPckt.Dispose()
                End If
                tmpItem.Delete()
                Exit Sub
            Else
                client.Character.Copper -= (itemPrice * count)
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                If Not client.Character.ItemSETSLOT(tmpItem, slot, bag) Then
                    tmpItem.Delete()
                    client.Character.Copper += itemPrice
                    client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                Else
                    Dim okPckt As New PacketClass(OPCODES.SMSG_BUY_ITEM)
                    okPckt.AddUInt64(vendorGuid)
                    okPckt.AddInt32(itemID)
                    okPckt.AddInt32(count)
                    client.Send(okPckt)
                    okPckt.Dispose()
                End If
                client.Character.SendCharacterUpdate(False)
            End If
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BUYBACK_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BUYBACK_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 17 Then Exit Sub
            packet.GetInt16()
            Dim vendorGuid As ULong = packet.GetUInt64
            Dim slot As Integer = packet.GetInt32
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUYBACK_ITEM [vendorGuid={2:X} Slot={3}]", client.IP, client.Port, vendorGuid, slot)

            'TODO: If item is not located in your buyback you can't buy it back (this checking below doesn't work)
            If slot < BuyBackSlots.BUYBACK_SLOT_START OrElse slot >= BuyBackSlots.BUYBACK_SLOT_END OrElse client.Character.Items.ContainsKey(slot) = False Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                Try
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(0)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_CANT_FIND_ITEM)
                    client.Send(errorPckt)
                Finally
                    errorPckt.Dispose()
                End Try
                Exit Sub
            End If
            'DONE: Check if you can afford it
            Dim tmpItem As ItemObject = client.Character.Items(slot)
            If client.Character.Copper < (tmpItem.ItemInfo.SellPrice * tmpItem.StackCount) Then
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                Try
                    errorPckt.AddUInt64(vendorGuid)
                    errorPckt.AddInt32(tmpItem.ItemEntry)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
                    client.Send(errorPckt)
                Finally
                    errorPckt.Dispose()
                End Try
                Exit Sub
            End If

            'DONE: Move item to the inventory, if it's unable to do that tell the client that the bags are full
            client.Character.ItemREMOVE(tmpItem.GUID, False, True)
            If client.Character.ItemADD_AutoSlot(tmpItem) Then
                Dim eSlot As Byte = slot - BuyBackSlots.BUYBACK_SLOT_START
                client.Character.Copper -= (tmpItem.ItemInfo.SellPrice * tmpItem.StackCount)
                client.Character.BuyBackTimeStamp(eSlot) = 0
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BUYBACK_TIMESTAMP_1 + eSlot, 0)
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_BUYBACK_PRICE_1 + eSlot, 0)
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

                client.Character.SendCharacterUpdate()
            Else
                SendInventoryChangeFailure(client.Character, InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL, 0, 0)
                client.Character.ItemSETSLOT(tmpItem, 0, slot)
            End If
        End Sub

        ''' <summary>
        ''' Called when [CMSG_REPAIR_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_REPAIR_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 21 Then Exit Sub
            packet.GetInt16()
            Dim vendorGuid As ULong = packet.GetUInt64
            Dim itemGuid As ULong = packet.GetUInt64
            If WORLD_CREATUREs.ContainsKey(vendorGuid) = False OrElse (WORLD_CREATUREs(vendorGuid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_ARMORER) = 0 Then Exit Sub
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_REPAIR_ITEM [vendorGuid={2:X} itemGuid={3:X}]", client.IP, client.Port, vendorGuid, itemGuid)

            'DONE: Reputation discount
            Dim discountMod As Single = client.Character.GetDiscountMod(WORLD_CREATUREs(vendorGuid).Faction)
            Dim price As UInteger = 0

            If itemGuid <> 0 Then
                price = (WORLD_ITEMs(itemGuid).GetDurabulityCost * discountMod)
                If client.Character.Copper >= price Then
                    client.Character.Copper -= price
                    client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                    client.Character.SendCharacterUpdate(False)

                    WORLD_ITEMs(itemGuid).ModifyToDurability(100.0F, client)
                End If
            Else
                For i As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                    If client.Character.Items.ContainsKey(i) Then
                        price = (client.Character.Items(i).GetDurabulityCost * discountMod)

                        If client.Character.Copper >= price Then
                            client.Character.Copper -= price
                            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                            client.Character.SendCharacterUpdate(False)

                            client.Character.Items(i).ModifyToDurability(100.0F, client)
                        Else
                            Continue For
                        End If
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Sends the list inventory.
        ''' </summary>
        ''' <param name="objCharacter">The obj char.</param>
        ''' <param name="guid">The GUID.</param>
        ''' <returns></returns>
        Private Sub SendListInventory(ByRef objCharacter As CharacterObject, ByVal guid As ULong)
            Try
                Dim packet As New PacketClass(OPCODES.SMSG_LIST_INVENTORY)
                packet.AddUInt64(guid)

                Dim mySqlQuery As New DataTable
                WorldDatabase.Query(String.Format("SELECT * FROM npc_vendor WHERE entry = {0};", WORLD_CREATUREs(guid).ID), mySqlQuery)
                Dim dataPos As Integer = packet.Data.Length
                packet.AddInt8(0) 'Will be updated later

                Dim i As Byte = 0
                Dim itemID As Integer
                For Each sellRow As DataRow In mySqlQuery.Rows
                    itemID = sellRow.Item("item")
                    'DONE: You will now only see items for your class
                    If ITEMDatabase.ContainsKey(itemID) = False Then
                        Dim tmpItem As New ItemInfo(itemID)
                        'The New does a an add to the .Containskey collection above
                    End If

                    If (ITEMDatabase(itemID).AvailableClasses = 0 OrElse (ITEMDatabase(itemID).AvailableClasses And objCharacter.ClassMask)) Then
                        i += 1
                        packet.AddInt32(-1) 'i-1
                        packet.AddInt32(itemID)
                        packet.AddInt32(ITEMDatabase(itemID).Model)

                        'AviableCount
                        If sellRow.Item("maxcount") <= 0 Then
                            packet.AddInt32(-1)
                        Else
                            packet.AddInt32(sellRow.Item("maxcount"))
                        End If

                        'DONE: Discount on reputation
                        Dim discountMod As Single = objCharacter.GetDiscountMod(WORLD_CREATUREs(guid).Faction)
                        packet.AddInt32(ITEMDatabase(itemID).BuyPrice * discountMod)
                        packet.AddInt32(-1) 'Durability
                        packet.AddInt32(ITEMDatabase(itemID).BuyCount)
                    End If
                Next

                If i > 0 Then packet.AddInt8(i, dataPos)
                objCharacter.client.Send(packet)
                packet.Dispose()
            Catch e As Exception
                Log.WriteLine(LogType.DEBUG, "Error while listing inventory.{0}", Environment.NewLine & e.ToString)
            End Try
        End Sub

#End Region
#Region "Banker"
        ''' <summary>
        ''' Called when [CMSG_AUTOBANK_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_AUTOBANK_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 7 Then Exit Sub
            packet.GetInt16()
            Dim srcBag As Byte = packet.GetInt8
            Dim srcSlot As Byte = packet.GetInt8
            If srcBag = 255 Then srcBag = 0

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUTOBANK_ITEM [srcSlot={2}:{3}]", client.IP, client.Port, srcBag, srcSlot)

            For dstSlot As Byte = BankItemSlots.BANK_SLOT_ITEM_START To BankItemSlots.BANK_SLOT_ITEM_END
                If Not client.Character.Items.ContainsKey(dstSlot) Then
                    client.Character.ItemSWAP(srcBag, srcSlot, 0, dstSlot)
                    Exit Sub
                End If
            Next

            For dstBag As Byte = BankBagSlots.BANK_SLOT_BAG_START To BankBagSlots.BANK_SLOT_BAG_END - 1
                If client.Character.Items.ContainsKey(dstBag) Then
                    If client.Character.Items(dstBag).ItemInfo.IsContainer Then
                        For dstSlot As Byte = 0 To client.Character.Items(dstBag).ItemInfo.ContainerSlots - 1
                            If Not client.Character.Items(dstBag).Items.ContainsKey(dstSlot) Then
                                client.Character.ItemSWAP(srcBag, srcSlot, dstBag, dstSlot)
                                ' Not sure, but we probably have to send the "EQUIP_ERR_OK = 0," packet to play the "moving sound".
                                Exit Sub
                            End If
                        Next
                    End If
                End If
            Next

            ' If you ever get here, send error packet. I think it should be "EQUIP_ERR_INVENTORY_FULL = 50, // ERR_INV_FULL" here.
        End Sub

        ''' <summary>
        ''' Called when [CMSG_AUTOSTORE_BANK_ITEM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_AUTOSTORE_BANK_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 7 Then Exit Sub
            packet.GetInt16()
            Dim srcBag As Byte = packet.GetInt8
            Dim srcSlot As Byte = packet.GetInt8
            If srcBag = 255 Then srcBag = 0

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUTOSTORE_BANK_ITEM [srcSlot={2}:{3}]", client.IP, client.Port, srcBag, srcSlot)

            For dstSlot As Byte = InventoryPackSlots.INVENTORY_SLOT_ITEM_START To InventoryPackSlots.INVENTORY_SLOT_ITEM_END
                If Not client.Character.Items.ContainsKey(dstSlot) Then
                    client.Character.ItemSWAP(srcBag, srcSlot, 0, dstSlot)
                    Exit Sub
                End If
            Next

            For bag As Byte = InventorySlots.INVENTORY_SLOT_BAG_START To InventorySlots.INVENTORY_SLOT_BAG_END - 1
                If client.Character.Items.ContainsKey(bag) Then
                    If client.Character.Items(bag).ItemInfo.IsContainer Then
                        For dstSlot As Byte = 0 To client.Character.Items(bag).ItemInfo.ContainerSlots - 1
                            If Not client.Character.Items(bag).Items.ContainsKey(dstSlot) Then
                                client.Character.ItemSWAP(srcBag, srcSlot, bag, dstSlot)
                                ' Not sure, but we probably have to send the "EQUIP_ERR_OK = 0," packet to play the "moving sound".
                                Exit Sub
                            End If
                        Next
                    End If
                End If
            Next

            ' If you ever get here, send error packet I think it should be "EQUIP_ERR_BAG_FULL3 = 53, // ERR_BAG_FULL" here.
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BUY_BANK_SLOT] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BUY_BANK_SLOT(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUY_BANK_SLOT", client.IP, client.Port)

            If client.Character.Items_AvailableBankSlots < DbcBankBagSlotsMax AndAlso
               client.Character.Copper >= DbcBankBagSlotPrices(client.Character.Items_AvailableBankSlots) Then
                client.Character.Copper -= DbcBankBagSlotPrices(client.Character.Items_AvailableBankSlots)
                client.Character.Items_AvailableBankSlots += 1

                CharacterDatabase.Update(String.Format("UPDATE characters SET char_bankSlots = {0}, char_copper = {1};", client.Character.Items_AvailableBankSlots, client.Character.Copper))

                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_BYTES_2, client.Character.cPlayerBytes2)
                client.Character.SendCharacterUpdate(False)
            Else
                Dim errorPckt As New PacketClass(OPCODES.SMSG_BUY_FAILED)
                Try
                    errorPckt.AddUInt64(0)
                    errorPckt.AddInt32(0)
                    errorPckt.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
                    client.Send(errorPckt)
                Finally
                    errorPckt.Dispose()
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BANKER_ACTIVATE] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BANKER_ACTIVATE(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BANKER_ACTIVATE [GUID={2:X}]", client.IP, client.Port, guid)

            SendShowBank(client.Character, guid)
        End Sub

        ''' <summary>
        ''' Sends the opcode to show the bank.
        ''' </summary>
        ''' <param name="objCharacter">The objCharacter.</param>
        ''' <param name="guid">The GUID.</param>
        ''' <returns></returns>
        Private Sub SendShowBank(ByRef objCharacter As CharacterObject, ByVal guid As ULong)
            Dim packet As New PacketClass(OPCODES.SMSG_SHOW_BANK)
            Try
                packet.AddUInt64(guid)
                objCharacter.client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

#End Region
#Region "Other"
        ''' <summary>
        ''' Sends the bind point confirm.
        ''' </summary>
        ''' <param name="objCharacter">The obj char.</param>
        ''' <param name="guid">The GUID.</param>
        ''' <returns></returns>
        Private Sub SendBindPointConfirm(ByRef objCharacter As CharacterObject, ByVal guid As ULong)
            objCharacter.SendGossipComplete()
            objCharacter.ZoneCheck()
            Dim packet As New PacketClass(OPCODES.SMSG_BINDER_CONFIRM)
            Try
                packet.AddUInt64(guid)
                packet.AddInt32(objCharacter.ZoneID)
                objCharacter.client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        ''' <summary>
        ''' Called when [CMSG_BINDER_ACTIVATE] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_CMSG_BINDER_ACTIVATE(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 13 Then Exit Sub
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BINDER_ACTIVATE [binderGUID={2:X}]", client.IP, client.Port, guid)

            If WORLD_CREATUREs.ContainsKey(guid) = False Then Exit Sub

            client.Character.SendGossipComplete()

            Dim spellTargets As New SpellTargets
            spellTargets.SetTarget_UNIT(client.Character)
            Dim castParams As New CastSpellParameters(spellTargets, WORLD_CREATUREs(guid), 3286, True)
            ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf castParams.Cast))
        End Sub

        ''' <summary>
        ''' Sends the talent wipe confirm.
        ''' </summary>
        ''' <param name="objCharacter">The obj char.</param>
        ''' <param name="cost">The cost.</param>
        ''' <returns></returns>
        Private Sub SendTalentWipeConfirm(ByRef objCharacter As CharacterObject, ByVal cost As Integer)
            Dim packet As New PacketClass(OPCODES.MSG_TALENT_WIPE_CONFIRM)
            Try
                packet.AddUInt64(objCharacter.GUID)
                packet.AddInt32(cost)
                objCharacter.client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub

        ''' <summary>
        ''' Called when [MSG_TALENT_WIPE_CONFIRM] is received.
        ''' </summary>
        ''' <param name="packet">The packet.</param>
        ''' <param name="client">The client.</param>
        ''' <returns></returns>
        Public Sub On_MSG_TALENT_WIPE_CONFIRM(ByRef packet As PacketClass, ByRef client As ClientClass)
            Try
                packet.GetInt16()
                Dim guid As ULong = packet.GetPackGuid

                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_TALENT_WIPE_CONFIRM [GUID={2:X}]", client.IP, client.Port, guid)
                If client.Character.Level < 10 Then Exit Sub

                'DONE: Removing all talents
                For Each talentInfo As KeyValuePair(Of Integer, WS_DBCDatabase.TalentInfo) In Talents
                    For i As Integer = 0 To 4
                        If talentInfo.Value.RankID(i) <> 0 Then
                            If client.Character.HaveSpell(talentInfo.Value.RankID(i)) Then
                                client.Character.UnLearnSpell(talentInfo.Value.RankID(i))
                            End If
                        End If
                    Next i
                Next

                'DONE: Reset Talentpoints to Level - 9
                client.Character.TalentPoints = client.Character.Level - 9
                client.Character.SetUpdateFlag(EPlayerFields.PLAYER_CHARACTER_POINTS1, client.Character.TalentPoints)
                client.Character.SendCharacterUpdate(True)

                'DONE: Use spell 14867
                Dim SMSG_SPELL_START As New PacketClass(OPCODES.SMSG_SPELL_START)
                Try
                    SMSG_SPELL_START.AddPackGUID(client.Character.GUID)
                    SMSG_SPELL_START.AddPackGUID(guid)
                    SMSG_SPELL_START.AddInt16(14867)
                    SMSG_SPELL_START.AddInt16(0)
                    SMSG_SPELL_START.AddInt16(&HF)
                    SMSG_SPELL_START.AddInt32(0)
                    SMSG_SPELL_START.AddInt16(0)
                    client.Send(SMSG_SPELL_START)
                Finally
                    SMSG_SPELL_START.Dispose()
                End Try

                Dim SMSG_SPELL_GO As New PacketClass(OPCODES.SMSG_SPELL_GO)
                Try
                    SMSG_SPELL_GO.AddPackGUID(client.Character.GUID)
                    SMSG_SPELL_GO.AddPackGUID(guid)
                    SMSG_SPELL_GO.AddInt16(14867)
                    SMSG_SPELL_GO.AddInt16(0)
                    SMSG_SPELL_GO.AddInt8(&HD)
                    SMSG_SPELL_GO.AddInt8(&H1)
                    SMSG_SPELL_GO.AddInt8(&H1)
                    SMSG_SPELL_GO.AddUInt64(client.Character.GUID)
                    SMSG_SPELL_GO.AddInt32(0)
                    SMSG_SPELL_GO.AddInt16(&H200)
                    SMSG_SPELL_GO.AddInt16(0)
                    client.Send(SMSG_SPELL_GO)
                Finally
                    SMSG_SPELL_GO.Dispose()
                End Try

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error unlearning talents: {0}{1}", Environment.NewLine, e.ToString)
            End Try
        End Sub

#End Region
#Region "Default Menu"
        Public Class TDefaultTalk
            Inherits TBaseTalk

            ''' <summary>
            ''' Called when [gossip hello].
            ''' </summary>
            ''' <param name="objCharacter">The obj char.</param>
            ''' <param name="cGuid">The objCharacter GUID.</param>
            ''' <returns></returns>
            Public Overrides Sub OnGossipHello(ByRef objCharacter As CharacterObject, ByVal cGuid As ULong)
                Dim textID As Integer = 0

                Dim npcMenu As New GossipMenu

                objCharacter.TalkMenuTypes.Clear()

                Dim creatureInfo As CreatureInfo = WORLD_CREATUREs(cGuid).CreatureInfo
                Try
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_VENDOR) OrElse (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_ARMORER) Then
                        npcMenu.AddMenu("Let me browse your goods.", MenuIcon.MENUICON_VENDOR)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_VENDOR)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) Then
                        npcMenu.AddMenu("I want to continue my journey.", MenuIcon.MENUICON_TAXI)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TAXIVENDOR)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TRAINER) Then
                        If creatureInfo.TrainerType = TrainerTypes.TRAINER_TYPE_CLASS Then
                            If creatureInfo.Classe <> objCharacter.Classe Then
                                Select Case creatureInfo.Classe
                                    Case Classes.CLASS_DRUID
                                        textID = 4913
                                    Case Classes.CLASS_HUNTER
                                        textID = 10090
                                    Case Classes.CLASS_MAGE
                                        textID = 328
                                    Case Classes.CLASS_PALADIN
                                        textID = 1635
                                    Case Classes.CLASS_PRIEST
                                        textID = 4436
                                    Case Classes.CLASS_ROGUE
                                        textID = 4797
                                    Case Classes.CLASS_SHAMAN
                                        textID = 5003
                                    Case Classes.CLASS_WARLOCK
                                        textID = 5836
                                    Case Classes.CLASS_WARRIOR
                                        textID = 4985
                                End Select

                                objCharacter.SendGossip(cGuid, textID)
                                Exit Sub
                            Else
                                npcMenu.AddMenu("I am interested in " & GetClassName(objCharacter.Classe) & " training.", MenuIcon.MENUICON_TRAINER)
                                objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TRAINER)
                                If objCharacter.Level >= 10 Then
                                    npcMenu.AddMenu("I want to unlearn all my talents.", MenuIcon.MENUICON_GOSSIP)
                                    objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TALENTWIPE)
                                End If
                            End If
                        ElseIf creatureInfo.TrainerType = TrainerTypes.TRAINER_TYPE_MOUNTS Then
                            If creatureInfo.Race > 0 AndAlso creatureInfo.Race <> objCharacter.Race AndAlso objCharacter.GetReputation(creatureInfo.Faction) < ReputationRank.Exalted Then
                                Select Case creatureInfo.Race
                                    Case Races.RACE_DWARF
                                        textID = 5865
                                    Case Races.RACE_GNOME
                                        textID = 4881
                                    Case Races.RACE_HUMAN
                                        textID = 5861
                                    Case Races.RACE_NIGHT_ELF
                                        textID = 5862
                                    Case Races.RACE_ORC
                                        textID = 5863
                                    Case Races.RACE_TAUREN
                                        textID = 5864
                                    Case Races.RACE_TROLL
                                        textID = 5816
                                    Case Races.RACE_UNDEAD
                                        textID = 624
                                End Select

                                objCharacter.SendGossip(cGuid, textID)
                                Exit Sub
                            Else
                                npcMenu.AddMenu("I am interested in mount training.", MenuIcon.MENUICON_TRAINER)
                                objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TRAINER)
                            End If
                        ElseIf creatureInfo.TrainerType = TrainerTypes.TRAINER_TYPE_TRADESKILLS Then
                            If creatureInfo.TrainerSpell > 0 AndAlso objCharacter.HaveSpell(creatureInfo.TrainerSpell) = False Then
                                textID = 11031
                                objCharacter.SendGossip(cGuid, textID)
                                Exit Sub
                            Else
                                npcMenu.AddMenu("I am interested in professions training.", MenuIcon.MENUICON_TRAINER)
                                objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TRAINER)
                            End If
                        ElseIf creatureInfo.TrainerType = TrainerTypes.TRAINER_TYPE_PETS Then
                            If objCharacter.Classe <> Classes.CLASS_HUNTER Then
                                textID = 3620
                                objCharacter.SendGossip(cGuid, textID)
                                Exit Sub
                            Else
                                npcMenu.AddMenu("I am interested in pet training.", MenuIcon.MENUICON_TRAINER)
                                objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TRAINER)
                            End If
                        End If
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_SPIRITHEALER) Then
                        textID = 580
                        npcMenu.AddMenu("Return me to life", MenuIcon.MENUICON_GOSSIP)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_SPIRITHEALER)
                    End If
                    'UNIT_NPC_FLAG_GUARD
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_INNKEEPER) Then
                        npcMenu.AddMenu("Make this inn your home.", MenuIcon.MENUICON_BINDER)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_INNKEEPER)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_BANKER) Then
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_BANKER)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_PETITIONER) Then
                        npcMenu.AddMenu("I am interested in guilds.", MenuIcon.MENUICON_PETITION)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_ARENACHARTER)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TABARDVENDOR) Then
                        npcMenu.AddMenu("I want to purchase a tabard.", MenuIcon.MENUICON_TABARD)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_TABARDVENDOR)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_BATTLEFIELDPERSON) Then
                        npcMenu.AddMenu("My blood hungers for battle.", MenuIcon.MENUICON_BATTLEMASTER)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_BATTLEFIELD)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_AUCTIONEER) Then
                        npcMenu.AddMenu("Wanna auction something?", MenuIcon.MENUICON_AUCTIONER)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_AUCTIONEER)
                    End If
                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_STABLE) Then
                        npcMenu.AddMenu("Let me check my pet.", MenuIcon.MENUICON_VENDOR)
                        objCharacter.TalkMenuTypes.Add(Gossip_Option.GOSSIP_OPTION_STABLEPET)
                    End If

                    If textID = 0 Then textID = WORLD_CREATUREs(cGuid).NPCTextID

                    If (creatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_QUESTGIVER) = NPCFlags.UNIT_NPC_FLAG_QUESTGIVER Then

                        Dim qMenu As QuestMenu = ALLQUESTS.GetQuestMenu(objCharacter, cGuid)
                        If qMenu.IDs.Count = 0 AndAlso npcMenu.Menus.Count = 0 Then Exit Sub

                        If npcMenu.Menus.Count = 0 Then ' If we only have quests to list
                            If qMenu.IDs.Count = 1 Then ' If we only have one quest to list, we direct the client directly to it
                                Dim questID As Integer = qMenu.IDs(0)
                                If Not ALLQUESTS.IsValidQuest(questID) Then
                                    'TODO: Another chunk that doesn't do anything but should
                                    Dim tmpQuest As New WS_QuestInfo(questID)
                                End If
                                Dim status As QuestgiverStatusFlag = qMenu.Icons(0)
                                If status = QuestgiverStatusFlag.DIALOG_STATUS_INCOMPLETE Then
                                    For i As Integer = 0 To QuestInfo.QUEST_SLOTS
                                        If objCharacter.TalkQuests(i) IsNot Nothing AndAlso objCharacter.TalkQuests(i).ID = questID Then
                                            'Load quest data
                                            objCharacter.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                                            ALLQUESTS.SendQuestRequireItems(objCharacter.client, objCharacter.TalkCurrentQuest, cGuid, objCharacter.TalkQuests(i))
                                            Exit For
                                        End If
                                    Next
                                Else
                                    objCharacter.TalkCurrentQuest = ALLQUESTS.ReturnQuestInfoById(questID)
                                    ALLQUESTS.SendQuestDetails(objCharacter.client, objCharacter.TalkCurrentQuest, cGuid, True)
                                End If
                            Else ' There were more than one quest to list
                                ALLQUESTS.SendQuestMenu(objCharacter, cGuid, "I have some tasks for you, $N.", qMenu)
                            End If
                        Else ' We have to list both gossip options and quests
                            objCharacter.SendGossip(cGuid, textID, npcMenu, qMenu)
                        End If
                    Else
                        objCharacter.SendGossip(cGuid, textID, npcMenu)
                    End If

                Catch ex As Exception
                    ' Stop
                End Try
            End Sub

            ''' <summary>
            ''' Called when [gossip select].
            ''' </summary>
            ''' <param name="objCharacter">The objCharacter.</param>
            ''' <param name="cGUID">The objCharacter GUID.</param>
            ''' <param name="selected">The selected.</param>
            ''' <returns></returns>
            Public Overrides Sub OnGossipSelect(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong, ByVal selected As Integer)
                Select Case objCharacter.TalkMenuTypes(selected)
                    Case Gossip_Option.GOSSIP_OPTION_SPIRITHEALER
                        If objCharacter.DEAD = True Then
                            Dim response As New PacketClass(OPCODES.SMSG_SPIRIT_HEALER_CONFIRM)
                            Try
                                response.AddUInt64(cGUID)
                                objCharacter.client.Send(response)
                            Finally
                                response.Dispose()
                            End Try

                            objCharacter.SendGossipComplete()
                        End If

                    Case Gossip_Option.GOSSIP_OPTION_VENDOR, Gossip_Option.GOSSIP_OPTION_ARMORER, Gossip_Option.GOSSIP_OPTION_STABLEPET
                        SendListInventory(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_TRAINER
                        SendTrainerList(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_TAXIVENDOR
                        SendTaxiMenu(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_INNKEEPER
                        SendBindPointConfirm(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_BANKER
                        SendShowBank(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_ARENACHARTER
                        SendPetitionActivate(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_TABARDVENDOR
                        SendTabardActivate(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_AUCTIONEER
                        SendShowAuction(objCharacter, cGUID)
                    Case Gossip_Option.GOSSIP_OPTION_TALENTWIPE
                        SendTalentWipeConfirm(objCharacter, 0)
                    Case Gossip_Option.GOSSIP_OPTION_GOSSIP
                        objCharacter.SendTalking(WORLD_CREATUREs(cGUID).NPCTextID)
                    Case Gossip_Option.GOSSIP_OPTION_QUESTGIVER
                        'NOTE: This may stay unused
                        Dim qMenu As QuestMenu = ALLQUESTS.GetQuestMenu(objCharacter, cGUID)
                        ALLQUESTS.SendQuestMenu(objCharacter, cGUID, "I have some tasks for you, $N.", qMenu)
                End Select
                ''c.SendGossipComplete()
            End Sub
        End Class
#End Region
    End Module
End NameSpace