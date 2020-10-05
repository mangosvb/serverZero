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
Imports mangosVB.Common.Globals
Imports mangosVB.Shared

Public Module WS_Mail

#Region "WS.Mail.Constants"

    Public Const ITEM_MAILTEXT_ITEMID As Integer = 889

#End Region

#Region "WS.Mail.Handlers"

    Public Sub On_CMSG_MAIL_RETURN_TO_SENDER(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_RETURN_TO_SENDER [MailID={2}]", client.IP, client.Port, MailID)

        'A = 1
        'B = 2
        'A = A + B '3
        'B = A - B '3-2=1
        'A = A - B '3-1=2

        Dim MailTime As Integer = GetTimestamp(Now) + (TimeConstant.DAY * 30) 'Set expiredate to today + 30 days
        CharacterDatabase.Update(String.Format("UPDATE characters_mail SET mail_time = {1}, mail_read = 0, mail_receiver = (mail_receiver + mail_sender), mail_sender = (mail_receiver - mail_sender), mail_receiver = (mail_receiver - mail_sender) WHERE mail_id = {0};", MailID, MailTime))

        Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
        response.AddInt32(MailID)
        response.AddInt32(MailResult.MAIL_RETURNED)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_MAIL_DELETE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_DELETE [MailID={2}]", client.IP, client.Port, MailID)

        CharacterDatabase.Update(String.Format("DELETE FROM characters_mail WHERE mail_id = {0};", MailID))

        Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
        response.AddInt32(MailID)
        response.AddInt32(MailResult.MAIL_DELETED)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_MAIL_MARK_AS_READ(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_MARK_AS_READ [MailID={2}]", client.IP, client.Port, MailID)
        Dim MailTime As Integer = GetTimestamp(Now) + (TimeConstant.DAY * 3) 'Set expiredate to today + 3 days
        CharacterDatabase.Update(String.Format("UPDATE characters_mail SET mail_read = 1, mail_time = {1} WHERE mail_id = {0} AND mail_read < 2;", MailID, MailTime))
    End Sub

    Public Sub On_MSG_QUERY_NEXT_MAIL_TIME(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_QUERY_NEXT_MAIL_TIME", client.IP, client.Port)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query(String.Format("SELECT COUNT(*) FROM characters_mail WHERE mail_read = 0 AND mail_receiver = {0} AND mail_time > {1};", client.Character.GUID, GetTimestamp(Now)), MySQLQuery)
        If MySQLQuery.Rows(0).Item(0) > 0 Then
            Dim response As New PacketClass(OPCODES.MSG_QUERY_NEXT_MAIL_TIME)
            response.AddInt32(0)
            client.Send(response)
            response.Dispose()
        Else
            Dim response As New PacketClass(OPCODES.MSG_QUERY_NEXT_MAIL_TIME)
            response.AddInt8(0)
            response.AddInt8(&HC0)
            response.AddInt8(&HA8)
            response.AddInt8(&HC7)
            client.Send(response)
            response.Dispose()
        End If
    End Sub

    Public Sub On_CMSG_GET_MAIL_LIST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_GET_MAIL_LIST [GUID={2:X}]", client.IP, client.Port, GameObjectGUID)

        Try
            'Done: Check for old mails, and delete those that have expired
            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT mail_id FROM characters_mail WHERE mail_time < {0};", GetTimestamp(Now)), MySQLQuery)
            If MySQLQuery.Rows.Count > 0 Then
                For i As Byte = 0 To MySQLQuery.Rows.Count - 1
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_mail WHERE mail_id = {0};", MySQLQuery.Rows(i).Item("mail_id")))
                Next
            End If

            CharacterDatabase.Query(String.Format("SELECT * FROM characters_mail WHERE mail_receiver = {0};", client.Character.GUID), MySQLQuery)

            Dim response As New PacketClass(OPCODES.SMSG_MAIL_LIST_RESULT)
            response.AddInt8(MySQLQuery.Rows.Count)

            Dim tmpItem As ItemObject
            If MySQLQuery.Rows.Count > 0 Then
                For i As Byte = 0 To MySQLQuery.Rows.Count - 1
                    response.AddInt32(MySQLQuery.Rows(i).Item("mail_id"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("mail_type"))

                    Select Case MySQLQuery.Rows(i).Item("mail_type")
                        Case MailTypeInfo.NORMAL
                            response.AddUInt64(MySQLQuery.Rows(i).Item("mail_sender"))
                        Case MailTypeInfo.AUCTION
                            response.AddInt32(MySQLQuery.Rows(i).Item("mail_sender")) 'creature/gameobject entry, auction id
                    End Select

                    response.AddString(MySQLQuery.Rows(i).Item("mail_subject"))

                    If MySQLQuery.Rows(i).Item("mail_body") <> "" Then
                        response.AddInt32(MySQLQuery.Rows(i).Item("mail_id")) 'ItemtextID?
                    Else
                        response.AddInt32(0)
                    End If

                    response.AddInt32(0) '2  = Gift
                    response.AddInt32(MySQLQuery.Rows(i).Item("mail_stationary")) '41/62 = Mail Background

                    If CType(MySQLQuery.Rows(i).Item("item_guid"), ULong) > 0 Then
                        tmpItem = LoadItemByGUID(MySQLQuery.Rows(i).Item("item_guid"))
                        response.AddInt32(tmpItem.ItemEntry)

                        If tmpItem.Enchantments.ContainsKey(EnchantSlots.ENCHANTMENT_PERM) Then
                            packet.AddInt32(tmpItem.Enchantments(EnchantSlots.ENCHANTMENT_PERM).ID)
                        Else
                            packet.AddInt32(0)                                      'Permanent enchant
                        End If

                        response.AddInt32(tmpItem.RandomProperties)                 'Item random property
                        response.AddInt32(0)                                        'Item suffix factor
                        response.AddInt8(tmpItem.StackCount)                        'Item count
                        response.AddInt32(tmpItem.ChargesLeft)                      'Spell Charges
                        response.AddInt32(tmpItem.ItemInfo.Durability)              'Durability Max
                        response.AddInt32(tmpItem.Durability)                       'Durability Min
                    Else
                        response.AddInt32(0)
                        response.AddInt32(0)
                        response.AddInt32(0)
                        response.AddInt32(0)
                        response.AddInt8(0)
                        response.AddInt32(0)
                        response.AddInt32(0)
                        response.AddInt32(0)
                    End If

                    response.AddUInt32(MySQLQuery.Rows(i).Item("mail_money"))    'Money on delivery
                    response.AddUInt32(MySQLQuery.Rows(i).Item("mail_COD"))      'Money as COD
                    response.AddInt32(MySQLQuery.Rows(i).Item("mail_read"))
                    response.AddSingle(((CType(MySQLQuery.Rows(i).Item("mail_time"), UInteger) - GetTimestamp(Now)) / TimeConstant.DAY))
                    response.AddInt32(0) 'Mail template ID
                Next
            End If

            client.Send(response)
            response.Dispose()

        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error getting mail list: {0}{1}", Environment.NewLine, e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_MAIL_TAKE_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_TAKE_ITEM [MailID={2}]", client.IP, client.Port, MailID)

        Try
            'DONE: Check if it's the receiver that is trying to get the item
            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT mail_cod, mail_sender, item_guid FROM characters_mail WHERE mail_id = {0} AND mail_receiver = {1};", MailID, client.Character.GUID), MySQLQuery)
            If MySQLQuery.Rows.Count = 0 Then 'The mail didn't exit, wrong owner trying to get someone elses item?
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(MailID)
                response.AddInt32(MailResult.MAIL_ITEM_REMOVED)
                response.AddInt32(MailSentError.INTERNAL_ERROR)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            'DONE: Check for COD
            If MySQLQuery.Rows(0).Item("mail_cod") <> 0 Then
                If client.Character.Copper < MySQLQuery.Rows(0).Item("mail_cod") Then
                    Dim noMoney As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                    noMoney.AddInt32(MailID)
                    noMoney.AddInt32(MailResult.MAIL_SENT)
                    noMoney.AddInt32(MailSentError.NOT_ENOUGHT_MONEY)
                    client.Send(noMoney)
                    noMoney.Dispose()
                    Exit Sub
                Else
                    'DONE: Pay COD and save
                    client.Character.Copper -= MySQLQuery.Rows(0).Item("mail_cod")
                    CharacterDatabase.Update(String.Format("UPDATE characters_mail SET mail_cod = 0 WHERE mail_id = {0};", MailID))

                    'DONE: Send COD to sender
                    'TODO: Edit text to be more blizzlike
                    Dim MailTime As Integer = GetTimestamp(Now) + (TimeConstant.DAY * 30) 'Set expiredate to today + 30 days
                    CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_subject, mail_body, mail_item_guid, mail_money, mail_COD, mail_time, mail_read, mail_type) VALUES 
                        ({0},{1},'{2}','{3}',{4},{5},{6},{7},{8},{9});", client.Character.GUID, MySQLQuery.Rows(0).Item("mail_sender"), "", "", 0, MySQLQuery.Rows(0).Item("mail_cod"), 0, MailTime, MailReadInfo.COD, 0))
                End If
            End If

            'DONE: Get Item
            If MySQLQuery.Rows(0).Item("item_guid") = 0 Then 'The item doesn't exist?
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(MailID)
                response.AddInt32(MailResult.MAIL_ITEM_REMOVED)
                response.AddInt32(MailSentError.INTERNAL_ERROR)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            Dim tmpItem As ItemObject = LoadItemByGUID(MySQLQuery.Rows(0).Item("item_guid"))
            tmpItem.OwnerGUID = client.Character.GUID
            tmpItem.Save()

            'DONE: Send error message if no slots
            If client.Character.ItemADD(tmpItem) Then
                CharacterDatabase.Update(String.Format("UPDATE characters_mail SET item_guid = 0 WHERE mail_id = {0};", MailID))
                CharacterDatabase.Update(String.Format("DELETE FROM mail_items WHERE mail_id = {0};", MailID))

                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(MailID)
                response.AddInt32(MailResult.MAIL_ITEM_REMOVED)
                response.AddInt32(MailSentError.NO_ERROR)
                client.Send(response)
                response.Dispose()
            Else
                tmpItem.Dispose()

                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(MailID)
                response.AddInt32(MailResult.MAIL_ITEM_REMOVED)
                response.AddInt32(MailSentError.BAG_FULL)
                client.Send(response)
                response.Dispose()
            End If
            client.Character.Save()

        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error getting item from mail: {0}{1}", Environment.NewLine, e.ToString)
        End Try
    End Sub

    Public Sub On_CMSG_MAIL_TAKE_MONEY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_TAKE_MONEY [MailID={2}]", client.IP, client.Port, MailID)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query(String.Format("SELECT mail_money FROM characters_mail WHERE mail_id = {0}; UPDATE characters_mail SET mail_money = 0 WHERE mail_id = {0};", MailID), MySQLQuery)
        If (client.Character.Copper + CLng(MySQLQuery.Rows(0).Item("mail_money"))) > UInteger.MaxValue Then
            client.Character.Copper = UInteger.MaxValue
        Else
            client.Character.Copper += MySQLQuery.Rows(0).Item("mail_money")
        End If
        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

        Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
        response.AddInt32(MailID)
        response.AddInt32(MailResult.MAIL_MONEY_REMOVED)
        response.AddInt32(0)
        client.Send(response)
        response.Dispose()

        client.Character.SaveCharacter()
    End Sub

    Public Sub On_CMSG_ITEM_TEXT_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 9 Then Exit Sub
        packet.GetInt16()
        Dim MailID As Integer = packet.GetInt32
        'Dim GameObjectGUID as ulong = packet.GetuInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ITEM_TEXT_QUERY [MailID={2}]", client.IP, client.Port, MailID)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query(String.Format("SELECT mail_body FROM characters_mail WHERE mail_id = {0};", MailID), MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        Dim response As New PacketClass(OPCODES.SMSG_ITEM_TEXT_QUERY_RESPONSE)
        response.AddInt32(MailID)
        response.AddString(MySQLQuery.Rows(0).Item("mail_body"))
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_MAIL_CREATE_TEXT_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim MailID As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MAIL_CREATE_TEXT_ITEM [MailID={2}]", client.IP, client.Port, MailID)

        'DONE: Create Item with ITEM_FIELD_ITEM_TEXT_ID = MailID
        Dim tmpItem As New ItemObject(ITEM_MAILTEXT_ITEMID, client.Character.GUID) With {
            .ItemText = MailID
        }
        If Not client.Character.ItemADD(tmpItem) Then
            Dim response As New PacketClass(OPCODES.SMSG_ITEM_TEXT_QUERY_RESPONSE)
            response.AddInt32(MailID)
            response.AddInt32(0)
            response.AddInt32(1)
            client.Send(response)
            response.Dispose()

            tmpItem.Delete()
        Else
            client.Character.SendItemUpdate(tmpItem)
        End If
    End Sub

    Public Sub On_CMSG_SEND_MAIL(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim GameObjectGUID As ULong = packet.GetUInt64
        Dim Receiver As String = packet.GetString
        If (packet.Data.Length - 1) < (14 + Receiver.Length + 1) Then Exit Sub
        Dim Subject As String = packet.GetString
        If (packet.Data.Length - 1) < (14 + Receiver.Length + 2 + Subject.Length) Then Exit Sub
        Dim Body As String = packet.GetString
        If (packet.Data.Length - 1) < (14 + Receiver.Length + 2 + Subject.Length + Body.Length + 4 + 4 + 1) Then Exit Sub
        packet.GetInt32()
        packet.GetInt32()
        Dim itemGuid As ULong = packet.GetUInt64()
        Dim Money As UInteger = packet.GetUInt32()
        Dim COD As UInteger = packet.GetUInt32()

        Try
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SEND_MAIL [Receiver={2} Subject={3}]", client.IP, client.Port, Receiver, Subject)

            Dim MySQLQuery As New DataTable
            CharacterDatabase.Query("SELECT char_guid, char_race FROM characters WHERE char_name Like '" & Receiver & "';", MySQLQuery)

            If MySQLQuery.Rows.Count = 0 Then
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(0)
                response.AddInt32(MailResult.MAIL_SENT)
                response.AddInt32(MailSentError.CHARACTER_NOT_FOUND)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If
            Dim ReceiverGUID As ULong = MySQLQuery.Rows(0).Item("char_guid")
            Dim ReceiverSide As Boolean = GetCharacterSide(MySQLQuery.Rows(0).Item("char_race"))

            If client.Character.GUID = ReceiverGUID Then
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(0)
                response.AddInt32(MailResult.MAIL_SENT)
                response.AddInt32(MailSentError.CANNOT_SEND_TO_SELF)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            If client.Character.Copper < (Money + 30) Then
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(0)
                response.AddInt32(MailResult.MAIL_SENT)
                response.AddInt32(MailSentError.NOT_ENOUGHT_MONEY)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            'Lets check so that the receiver doesn't have a full inbox
            CharacterDatabase.Query(String.Format("SELECT mail_id FROM characters_mail WHERE mail_receiver = {0}", ReceiverGUID), MySQLQuery)
            If MySQLQuery.Rows.Count >= 100 Then
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(0)
                response.AddInt32(MailResult.MAIL_SENT)
                response.AddInt32(MailSentError.INTERNAL_ERROR)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            'You can only send mails to characters with your same faction, but GMs can ofc
            If client.Access >= AccessLevel.GameMaster AndAlso client.Character.IsHorde <> ReceiverSide Then
                Dim response As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
                response.AddInt32(0)
                response.AddInt32(MailResult.MAIL_SENT)
                response.AddInt32(MailSentError.NOT_YOUR_ALLIANCE)
                client.Send(response)
                response.Dispose()
                Exit Sub
            End If

            'Check if the item exists
            If client.Character.ItemGETByGUID(itemGuid) Is Nothing Then itemGuid = 0

            client.Character.Copper -= 30 + Money
            client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)

            Dim MailTime As Integer = GetTimestamp(Now) + (TimeConstant.DAY * 30) 'Add 30 days to the current date/time
            CharacterDatabase.Update(String.Format("INSERT INTO characters_mail (mail_sender, mail_receiver, mail_type, mail_stationary, mail_subject, mail_body, mail_money, mail_COD, mail_time, mail_read, item_guid) VALUES
                ({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},{10});", client.Character.GUID, ReceiverGUID, 0, 41, Subject.Replace("'", "`"), Body.Replace("'", "`"), Money, COD, MailTime, CType(MailReadInfo.Unread, Byte), itemGuid = GUID_ITEM))

            If itemGuid > 0 Then client.Character.ItemREMOVE(itemGuid, False, True)

            'Tell the client we succeded
            Dim sendOK As New PacketClass(OPCODES.SMSG_SEND_MAIL_RESULT)
            sendOK.AddInt32(0)
            sendOK.AddInt32(MailResult.MAIL_SENT)
            sendOK.AddInt32(MailSentError.NO_ERROR)
            client.Send(sendOK)
            sendOK.Dispose()

            CHARACTERs_Lock.AcquireReaderLock(DEFAULT_LOCK_TIMEOUT)
            If CHARACTERs.ContainsKey(ReceiverGUID) Then
                Dim response As New PacketClass(OPCODES.SMSG_RECEIVED_MAIL)
                response.AddInt32(0)
                CHARACTERs(ReceiverGUID).client.Send(response)
                response.Dispose()
            End If
            CHARACTERs_Lock.ReleaseReaderLock()
        Catch e As Exception
            Log.WriteLine(LogType.FAILED, "Error sending mail: {0}{1}", Environment.NewLine, e.ToString)
        End Try
    End Sub

    'Public Sub SendNotify(ByRef client As ClientClass)
    '    Dim packet As New PacketClass(OPCODES.SMSG_RECEIVED_MAIL)
    '    packet.GetInt32() '(0)
    '    client.Send(packet)
    'End Sub

#End Region

End Module