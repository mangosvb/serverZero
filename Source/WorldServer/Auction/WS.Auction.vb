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


Module WS_Auction


#Region "WS.Auction.Constants"
    Public AuctionID As Integer
    Public AuctionFee As Integer
    Public AuctionTax As Integer

    Public Function GetAuctionSide(ByVal GUID As ULong) As AuctionHouses
        If Config.GlobalAuction Then Return AuctionHouses.AUCTION_UNDEFINED
        Select Case WORLD_CREATUREs(GUID).CreatureInfo.Faction
            Case 29, 68, 104
                Return AuctionHouses.AUCTION_HORDE
            Case 12, 55, 79
                Return AuctionHouses.AUCTION_ALLIANCE
            Case Else
                Return AuctionHouses.AUCTION_NEUTRAL
        End Select
    End Function

    Public Function GetAuctionDeposit(ByVal GUID As ULong, ByVal Price As Integer, ByVal ItemCount As Integer, ByVal Time As Integer) As Integer
        If ItemCount = 0 Then ItemCount = 1

        Select Case GetAuctionSide(GUID)
            Case AuctionHouses.AUCTION_NEUTRAL, AuctionHouses.AUCTION_BLACKWATER
                Return Fix(0.25F * Price * ItemCount * (Time / 120))
            Case AuctionHouses.AUCTION_UNDEFINED
                Return 0
            Case Else
                Return Fix(0.05F * Price * ItemCount * (Time / 120))
        End Select
    End Function

    Public Sub AuctionCreateMail(ByVal MailAction As MailAuctionAction, ByVal AuctionLocation As AuctionHouses, ByVal ReceiverGUID As ULong, ByVal ItemID As Integer, ByRef packet As PacketClass)
        Dim queryString As String = "INSERT INTO characters_mail ("
        Dim valuesString As String = ") VALUES ("
        Dim MailID As Integer = packet.GetInt32

        queryString += "mail_sender,"
        valuesString += CType(AuctionLocation, Integer).ToString
        queryString += "mail_receiver,"
        valuesString += ReceiverGUID.ToString
        queryString += "mail_type,"
        valuesString += "2"
        queryString += "mail_stationary,"
        valuesString += "62"
        queryString += "mail_subject,"
        valuesString += ItemID.ToString & ":0:" & CType(MailAction, Integer).ToString
        queryString += "mail_body,"
        valuesString += ""

        'queryString += "mail_item_id,"
        'valuesString += ""
        'queryString += "mail_item_guid,"
        'valuesString += ""
        'queryString += "mail_item_count,"
        'valuesString += ""

        queryString += "mail_money,"
        valuesString += ""
        queryString += "mail_COD,"
        valuesString += "0"
        queryString += "mail_time,"
        valuesString += "30"
        queryString += "mail_read"
        valuesString += "0);"

        CharacterDatabase.Update(queryString & valuesString)
    End Sub

#End Region
#Region "WS.Auction.Framework"
    Public Sub SendShowAuction(ByRef objCharacter As CharacterObject, ByVal GUID As ULong)
        Dim packet As New PacketClass(OPCODES.MSG_AUCTION_HELLO)
        packet.AddUInt64(GUID)
        packet.AddInt32(GetAuctionSide(GUID))          'AuctionID (on this is based the fees shown in client side)
        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub AuctionListAddItem(ByRef packet As PacketClass, ByRef Row As DataRow)
        packet.AddUInt32(Row.Item("auction_id"))
        Dim itemId As UInt32 = Row.Item("auction_itemId")
        packet.AddUInt32(itemId)

        Dim item As ItemInfo
        If ITEMDatabase.ContainsKey(itemId) Then
            item = ITEMDatabase(itemId)
        Else
            item = New ItemInfo(itemId)
        End If

        packet.AddUInt32(0)                                        ' PERM_ENCHANMENT_SLOT (Not sure if we have to do anything here)

        packet.AddUInt32(item.RandomProp)                          'Item Random Property ID
        packet.AddUInt32(item.RandomSuffix)                        'SuffixFactor

        packet.AddUInt32(Row.Item("auction_itemCount"))            'Item Count
        packet.AddInt32(item.Spells(0).SpellCharges)               'Item Spell Charges
        packet.AddUInt64(Row.Item("auction_owner"))                'Bid Owner
        packet.AddUInt32(Row.Item("auction_bid"))                  'Bid Price
        packet.AddUInt32(Fix(Row.Item("auction_bid") * 0.1F) + 1)  'Bid Step
        packet.AddUInt32(Row.Item("auction_buyout"))               'Bid Buyout
        packet.AddUInt32(Row.Item("auction_timeleft") * 1000)      'Bid Timeleft (in ms)
        packet.AddUInt64(Row.Item("auction_bidder"))               'Bidder GUID
        packet.AddUInt32(Row.Item("auction_bid"))                  'Bidder Current Bid
    End Sub

    Public Sub SendAuctionCommandResult(ByRef client As ClientClass, ByVal AuctionID As Integer, ByVal AuctionAction As AuctionAction, ByVal AuctionError As AuctionError, ByVal BidError As Integer)
        Dim response As New PacketClass(OPCODES.SMSG_AUCTION_COMMAND_RESULT)
        response.AddInt32(AuctionID)
        response.AddInt32(AuctionAction)
        response.AddInt32(AuctionError)
        'If AuctionError <> AuctionError.AUCTION_OK AndAlso AuctionAction <> AuctionAction.AUCTION_SELL_ITEM Then
        response.AddInt32(BidError)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub SendAuctionBidderNotification(ByRef objCharacter As CharacterObject)
        'Displays: "Outbid on <Item>."

        Dim packet As New PacketClass(OPCODES.SMSG_AUCTION_BIDDER_NOTIFICATION)
        packet.AddInt32(0)          'Location
        packet.AddInt32(0)          'AutionID
        packet.AddUInt64(0)          'BidderGUID
        packet.AddInt32(0)          'BidSum
        packet.AddInt32(0)          'Diff
        packet.AddInt32(0)          'ItemID
        packet.AddInt32(0)          'RandomProperyID
        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendAuctionOwnerNotification(ByRef objCharacter As CharacterObject)
        'Displays: "Your auction of <Item> sold."

        Dim packet As New PacketClass(OPCODES.SMSG_AUCTION_OWNER_NOTIFICATION)
        packet.AddInt32(0)          'AutionID
        packet.AddInt32(0)          'Bid
        packet.AddInt32(0)
        packet.AddInt32(0)
        packet.AddInt32(0)
        packet.AddInt32(0)          'ItemID
        packet.AddInt32(0)          'RandomProperyID
        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendAuctionRemovedNotification(ByRef objCharacter As CharacterObject)
        'Displays: "Auction of <Item> canceled by the seller."

        Dim packet As New PacketClass(OPCODES.SMSG_AUCTION_REMOVED_NOTIFICATION)
        packet.AddInt32(0)          'AutionID
        packet.AddInt32(0)          'ItemID
        packet.AddInt32(0)          'RandomProperyID
        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub SendAuctionListOwnerItems(ByRef client As ClientClass)
        Dim response As New PacketClass(OPCODES.SMSG_AUCTION_OWNER_LIST_RESULT)
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM auctionhouse WHERE auction_owner = " & client.Character.GUID & ";", MySQLQuery)
        If MySQLQuery.Rows.Count > 50 Then
            response.AddInt32(50)                               'Count
        Else
            response.AddInt32(MySQLQuery.Rows.Count)            'Count
        End If

        Dim count As Integer = 0
        For Each Row As DataRow In MySQLQuery.Rows
            AuctionListAddItem(response, Row)
            count += 1
            If count = 50 Then Exit For
        Next
        response.AddInt32(MySQLQuery.Rows.Count)            'AllCount
        client.Send(response)
        response.Dispose()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_AUCTION_OWNER_LIST_RESULT", client.IP, client.Port)
    End Sub

    Public Sub SendAuctionListBidderItems(ByRef client As ClientClass)
        Dim response As New PacketClass(OPCODES.SMSG_AUCTION_BIDDER_LIST_RESULT)
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM auctionhouse WHERE auction_bidder = " & client.Character.GUID & ";", MySQLQuery)
        If MySQLQuery.Rows.Count > 50 Then
            response.AddInt32(50)                               'Count
        Else
            response.AddInt32(MySQLQuery.Rows.Count)            'Count
        End If

        Dim count As Integer = 0
        For Each Row As DataRow In MySQLQuery.Rows
            AuctionListAddItem(response, Row)
            count += 1
            If count = 50 Then Exit For
        Next
        response.AddInt32(MySQLQuery.Rows.Count)            'AllCount
        client.Send(response)
        response.Dispose()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_AUCTION_BIDDER_LIST_RESULT", client.IP, client.Port)
    End Sub


#End Region
#Region "WS.Auction.Handlers"

    Public Sub On_MSG_AUCTION_HELLO(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_AUCTION_HELLO [GUID={2}]", client.IP, client.Port, guid)

        SendShowAuction(client.Character, guid)
    End Sub

    Public Sub On_CMSG_AUCTION_SELL_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 33 Then Exit Sub
        packet.GetInt16()
        Dim cGUID As ULong = packet.GetUInt64
        Dim iGUID As ULong = packet.GetUInt64
        Dim Bid As Integer = packet.GetInt32
        Dim Buyout As Integer = packet.GetInt32
        Dim Time As Integer = packet.GetInt32

        'DONE: Calculate deposit with time in hours
        Dim Deposit As Integer = GetAuctionDeposit(cGUID, WORLD_ITEMs(iGUID).ItemInfo.SellPrice, WORLD_ITEMs(iGUID).StackCount, Time)

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_SELL_ITEM [Bid={2} BuyOut={3} Time={4}]", client.IP, client.Port, Bid, Buyout, Time)

        'DONE: Convert time in seconds left
        Time = Time * 60

        'DONE: Check if item is bag with items
        If WORLD_ITEMs(iGUID).ItemInfo.IsContainer AndAlso Not WORLD_ITEMs(iGUID).IsFree Then
            SendAuctionCommandResult(client, 0, AuctionAction.AUCTION_SELL_ITEM, AuctionError.CANNOT_BID_YOUR_AUCTION_ERROR, 0)
            Return
        End If
        'DONE: Check deposit
        If client.Character.Copper < Deposit Then
            SendAuctionCommandResult(client, 0, AuctionAction.AUCTION_SELL_ITEM, AuctionError.AUCTION_NOT_ENOUGHT_MONEY, 0)
            Return
        End If

        'DONE: Get 5% deposit per 2h in auction (http://www.wowwiki.com/Formulas:Auction_House)
        client.Character.Copper -= Deposit

        'DONE: Remove item from inventory
        client.Character.ItemREMOVE(iGUID, False, True)

        'DONE: Add auction entry into table
        CharacterDatabase.Update(String.Format("INSERT INTO auctionhouse (auction_bid, auction_buyout, auction_timeleft, auction_bidder, auction_owner, auction_itemId, auction_itemGuid, auction_itemCount) VALUES ({0},{1},{2},{3},{4},{5},{6},{7});", Bid, Buyout, Time, 0, client.Character.GUID, WORLD_ITEMs(iGUID).ItemEntry, iGUID - GUID_ITEM, WORLD_ITEMs(iGUID).StackCount))

        'DONE: Send result packet
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT auction_id FROM auctionhouse WHERE auction_itemGuid = " & iGUID - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        SendAuctionCommandResult(client, MySQLQuery.Rows(0).Item("auction_id"), AuctionAction.AUCTION_SELL_ITEM, AuctionError.AUCTION_OK, 0)

        'NOTE: Not needed, client would request it
        'SendAuctionListOwnerItems(Client)
    End Sub

    Public Sub On_CMSG_AUCTION_REMOVE_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim AuctionID As Integer = packet.GetInt32
        Dim MailTime As Integer = GetTimestamp(Now) + (86400 * 30)

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_REMOVE_ITEM [GUID={2} AuctionID={3}]", client.IP, client.Port, GUID, AuctionID)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM auctionhouse WHERE auction_id = " & AuctionID & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub


        'DONE: Return item to owner
        CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read) VALUES ({0},{1},{2},62,'{3}','{4}',{5},{6},{7},{8});", AuctionID, MySQLQuery.Rows(0).Item("auction_owner"), 2, MySQLQuery.Rows(0).Item("auction_itemId") & ":0:4", "", 0, 0, MailTime, 0))

        Dim MailQuery As New DataTable
        CharacterDatabase.Query("SELECT mail_id FROM characters_mail WHERE mail_receiver = " & MySQLQuery.Rows(0).Item("auction_owner") & ";", MailQuery)
        Dim MailID As Integer = MailQuery.Rows(0).Item("mail_id")

        CharacterDatabase.Update(String.Format("INSERT INTO mail_items (mail_id, item_guid) VALUES ({0},{1});", MailID, MySQLQuery.Rows(0).Item("auction_itemGuid")))
        'DONE: Return money to bidder
        If MySQLQuery.Rows(0).Item("auction_bidder") <> 0 Then CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read, mail_type, mail_stationary) VALUES ({0},{1},'{2}','{3}',{4},{5},{6},{7},{8},62);", 0, MySQLQuery.Rows(0).Item("auction_bidder"), MySQLQuery.Rows(0).Item("auction_itemId") & ":0:4", "", MySQLQuery.Rows(0).Item("auction_bid"), 0, 30, 0, 2))

        'DONE: Remove from auction table
        CharacterDatabase.Update("DELETE FROM auctionhouse WHERE auction_id = " & AuctionID & ";")

        SendAuctionCommandResult(client, AuctionID, AuctionAction.AUCTION_CANCEL, AuctionError.AUCTION_OK, 0)
        'WS_Mail.SendNotify(Client) 'Notifies the client that they have mail
        'NOTE: Not needed, client would request it
        'SendAuctionListOwnerItems(Client)
    End Sub

    Public Sub On_CMSG_AUCTION_PLACE_BID(ByRef packet As PacketClass, ByRef client As ClientClass)
        packet.GetInt16()
        Dim cGUID As ULong = packet.GetUInt64
        Dim AuctionID As Integer = packet.GetInt32
        Dim Bid As Integer = packet.GetInt32
        Dim MailTime As Integer = GetTimestamp(Now) + (86400 * 30)


        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_PLACE_BID [AuctionID={2} Bid={3}]", client.IP, client.Port, AuctionID, Bid)

        If client.Character.Copper < Bid Then Exit Sub


        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM auctionhouse WHERE auction_id = " & AuctionID & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub
        If Bid < MySQLQuery.Rows(0).Item("auction_bid") Then Exit Sub

        If MySQLQuery.Rows(0).Item("auction_bidder") <> 0 Then
            'DONE: Send outbid mail
            CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read) VALUES ({0},{1},{2},62,'{3}','{4}',{5},{6},{7},{8});", AuctionID, MySQLQuery.Rows(0).Item("auction_bidder"), 2, MySQLQuery.Rows(0).Item("auction_itemId") & ":0:0", "", MySQLQuery.Rows(0).Item("auction_bid"), 0, MailTime, 0))
        End If

        If Bid = MySQLQuery.Rows(0).Item("auction_buyout") Then
            'Do buyout
            Dim bodyText As String
            Dim buffer As Byte()

            'DONE: Send auction succ to owner (PurchasedBy:SalePrice:BuyoutPrice:Deposit:AuctionHouseCut)
            buffer = BitConverter.GetBytes(CType(client.Character.GUID, Long))
            Array.Reverse(buffer)
            bodyText = BitConverter.ToString(buffer).Replace("-", "") & ":" & Bid & ":" & MySQLQuery.Rows(0).Item("auction_buyout") & ":0:0"
            CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read) VALUES ({0},{1},{2},62,'{3}','{4}',{5},{6},{7},{8});", AuctionID, MySQLQuery.Rows(0).Item("auction_owner"), 2, MySQLQuery.Rows(0).Item("auction_itemId") & ":0:2", bodyText, MySQLQuery.Rows(0).Item("auction_bid"), 0, MailTime, 0))

            'DONE: Send auction won to bidder with item (SoldBy:SalePrice:BuyoutPrice)
            buffer = BitConverter.GetBytes(CType(MySQLQuery.Rows(0).Item("auction_owner"), Long))
            Array.Reverse(buffer)
            bodyText = BitConverter.ToString(buffer).Replace("-", "") & ":" & Bid & ":" & MySQLQuery.Rows(0).Item("auction_buyout")
            CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read) VALUES ({0},{1},{2},62,'{3}','{4}',{5},{6},{7},{8});", AuctionID, client.Character.GUID, 2, MySQLQuery.Rows(0).Item("auction_itemId") & ":0:1", bodyText, 0, 0, MailTime, 0))

            Dim MailQuery As New DataTable
            CharacterDatabase.Query("SELECT mail_id FROM characters_mail WHERE mail_receiver = " & client.Character.GUID & ";", MailQuery)
            Dim MailID As Integer = MailQuery.Rows(0).Item("mail_id")

            CharacterDatabase.Update(String.Format("INSERT INTO mail_items (mail_id, item_guid) VALUES ({0},{1});", MailID, MySQLQuery.Rows(0).Item("auction_itemGuid")))

            'DONE: Remove auction
            CharacterDatabase.Update("DELETE FROM auctionhouse WHERE auction_id = " & AuctionID & ";")
            'WS_Mail.SendNotify(Client) 'Notifies the Client that they have mail
        Else
            'Do bid
            'NOTE: Here is using external timer or web page script to count what time is left and to do the actual buy

            'DONE: Set bidder in auction table, update bid value
            CharacterDatabase.Update(String.Format("UPDATE auctionhouse SET auction_bidder = {1}, auction_bid = {2} WHERE auction_id = {0};", AuctionID, client.Character.GUID, Bid))
        End If

        client.Character.Copper -= Bid
        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
        client.Character.SendCharacterUpdate(False)

        'DONE: Send result packet
        SendAuctionCommandResult(client, MySQLQuery.Rows(0).Item("auction_id"), AuctionAction.AUCTION_PLACE_BID, AuctionError.AUCTION_OK, 0)

        'NOTE: Not needed, client would request it
        'SendAuctionListBidderItems(Client)
    End Sub

    Public Sub On_CMSG_AUCTION_LIST_ITEMS(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 18 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim Unk1 As Integer = packet.GetInt32           'Always 0... may be page?
        Dim Name As String = packet.GetString
        If (packet.Data.Length - 1) < (18 + Name.Length + 1 + 1 + 4 + 4 + 4 + 4 + 1) Then Exit Sub
        Dim LevelMIN As Byte = packet.GetInt8           '0 if not used
        Dim LevelMAX As Byte = packet.GetInt8           '0 if not used

        Dim itemSlot As Integer = packet.GetInt32       '&H FF FF FF FF
        Dim itemClass As Integer = packet.GetInt32      '&H FF FF FF FF
        Dim itemSubClass As Integer = packet.GetInt32   '&H FF FF FF FF
        Dim itemQuality As Integer = packet.GetInt32    '&H FF FF FF FF

        Dim mustBeUsable As Integer = packet.GetInt8

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_LIST_ITEMS [{2} ({3}-{4})]", client.IP, client.Port, Name, LevelMIN, LevelMAX)

        Dim response As New PacketClass(OPCODES.SMSG_AUCTION_LIST_RESULT)
        Dim QueryString As String = "SELECT auctionhouse.* FROM " & CharacterDatabase.SQLDBName & ".auctionhouse, " & WorldDatabase.SQLDBName & ".item_template WHERE item_template.entry = auctionhouse.auction_itemId"
        If Name <> "" Then QueryString += " AND item_template.name LIKE '%" & Name & "%'"
        If LevelMIN <> 0 Then QueryString += " AND item_template.itemlevel > " & (LevelMIN - 1)
        If LevelMAX <> 0 Then QueryString += " AND item_template.itemlevel < " & (LevelMAX + 1)
        If itemSlot <> -1 Then QueryString += " AND item_template.inventoryType = " & itemSlot
        If itemClass <> -1 Then QueryString += " AND item_template.class = " & itemClass
        If itemSubClass <> -1 Then QueryString += " AND item_template.subclass = " & itemSubClass
        If itemQuality <> -1 Then QueryString += " AND item_template.quality = " & itemQuality

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query(QueryString & ";", MySQLQuery)
        If MySQLQuery.Rows.Count > 32 Then
            response.AddInt32(32)                               'Count
        Else
            response.AddInt32(MySQLQuery.Rows.Count)            'Count
        End If

        Dim count As Integer = 0
        For Each Row As DataRow In MySQLQuery.Rows
            AuctionListAddItem(response, Row)
            count += 1
            If count = 32 Then Exit For
        Next
        response.AddInt32(MySQLQuery.Rows.Count)            'AllCount
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_AUCTION_LIST_OWNER_ITEMS(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_LIST_OWNER_ITEMS [GUID={2:X}]", client.IP, client.Port, GUID)

        SendAuctionListOwnerItems(client)

    End Sub

    Public Sub On_CMSG_AUCTION_LIST_BIDDER_ITEMS(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim Unk As Long = packet.GetInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUCTION_LIST_BIDDER_ITEMS [GUID={2:X} UNK={3}]", client.IP, client.Port, GUID, Unk)

        SendAuctionListBidderItems(client)
    End Sub
#End Region
End Module
