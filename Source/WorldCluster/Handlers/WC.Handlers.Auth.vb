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
Imports System.Net.Sockets
Imports System.Threading
Imports mangosVB.Common
Imports mangosVB.Common.Globals
Imports mangosVB.Shared
Imports WorldCluster.Globals
Imports WorldCluster.Server

Namespace Handlers

    Public Module WC_Handlers_Auth

        Const REQUIRED_BUILD_LOW As Integer = 5875 ' 1.12.1
        Const REQUIRED_BUILD_HIGH As Integer = 6141

        Public Sub SendLoginOk(ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_AUTH_SESSION [{2}]", client.IP, client.Port, client.Account)

            Thread.Sleep(500)

            Dim response As New PacketClass(OPCODES.SMSG_AUTH_RESPONSE)
            response.AddInt8(LoginResponse.LOGIN_OK)
            response.AddInt32(0)
            response.AddInt8(2) 'BillingPlanFlags
            response.AddUInt32(0) 'BillingTimeRested
            client.Send(response)
        End Sub

        Public Sub On_CMSG_AUTH_SESSION(ByRef packet As PacketClass, ByRef client As ClientClass)
            'Log.WriteLine(LogType.DEBUG, "[{0}] [{1}:{2}] CMSG_AUTH_SESSION", Format(TimeOfDay, "hh:mm:ss"), client.IP, client.Port)

            packet.GetInt16()
            Dim clientVersion As Integer = packet.GetInt32
            Dim clientSessionID As Integer = packet.GetInt32
            Dim clientAccount As String = packet.GetString
            Dim clientSeed As Integer = packet.GetInt32
            Dim clientHash(19) As Byte
            For i As Integer = 0 To 19
                clientHash(i) = packet.GetInt8
            Next
            Dim clientAddOnsSize As Integer = packet.GetInt32

            'DONE: Set client.Account
            Dim tmp As String = clientAccount

            'DONE: Kick if existing
            For Each tmpClientEntry As KeyValuePair(Of UInteger, ClientClass) In CLIENTs
                If Not tmpClientEntry.Value Is Nothing Then
                    If tmpClientEntry.Value.Account = tmp Then
                        If Not tmpClientEntry.Value.Character Is Nothing Then
                            tmpClientEntry.Value.Character.Dispose()
                            tmpClientEntry.Value.Character = Nothing
                        End If
                        Try
                            tmpClientEntry.Value.Socket.Shutdown(SocketShutdown.Both)
                        Catch
                            tmpClientEntry.Value.Socket.Close()
                        End Try
                    End If
                End If
            Next
            client.Account = tmp

            'DONE: Set client.SS_Hash
            Dim result As New DataTable
            Dim query As String
            query = "SELECT sessionkey, gmlevel FROM account WHERE username = '" & client.Account & "';"
            AccountDatabase.Query(query, result)
            If result.Rows.Count > 0 Then
                tmp = result.Rows(0).Item("sessionkey")
                client.Access = result.Rows(0).Item("gmlevel")
            Else
                Log.WriteLine(LogType.USER, "[{0}:{1}] AUTH_UNKNOWN_ACCOUNT: Account not in DB!", client.IP, client.Port)
                Dim response_unk_acc As New PacketClass(OPCODES.SMSG_AUTH_RESPONSE)
                response_unk_acc.AddInt8(AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT)
                client.Send(response_unk_acc)
                Exit Sub
            End If
            ReDim client.SS_Hash(39)
            For i As Integer = 0 To Len(tmp) - 1 Step 2
                client.SS_Hash(i \ 2) = Val("&H" & Mid(tmp, i + 1, 2))
            Next
            client.Encryption = True

            'DONE: Disconnect clients trying to enter with an invalid build
            If clientVersion < REQUIRED_BUILD_LOW OrElse clientVersion > REQUIRED_BUILD_HIGH Then
                Dim invalid_version As New PacketClass(OPCODES.SMSG_AUTH_RESPONSE)
                invalid_version.AddInt8(AuthResult.WOW_FAIL_VERSION_INVALID)
                client.Send(invalid_version)
                Exit Sub
            End If

            'TODO: Make sure the correct client connected
            'Dim temp() As Byte = System.Text.Encoding.ASCII.GetBytes(clientAccount)
            'temp = Concat(temp, BitConverter.GetBytes(0))
            'temp = Concat(temp, BitConverter.GetBytes(clientSeed))
            'temp = Concat(temp, BitConverter.GetBytes(client.Index))
            'temp = Concat(temp, client.SS_Hash)
            'Dim ShaDigest() As Byte = New System.Security.Cryptography.SHA1Managed().ComputeHash(temp)
            'Log.WriteLine(LogType.DEBUG, "Client Hash: {0}", BitConverter.ToString(clientHash).Replace("-", ""))
            'Log.WriteLine(LogType.DEBUG, "Server Hash: {0}", BitConverter.ToString(ShaDigest).Replace("-", ""))
            'For i As Integer = 0 To 19
            '    If clientHash(i) <> ShaDigest(i) Then
            '        Dim responseFail As New PacketClass(OPCODES.SMSG_AUTH_RESPONSE)
            '        responseFail.AddInt8(AuthResponseCodes.AUTH_FAILED)
            '        client.Send(responseFail)
            '        Exit Sub
            '    End If
            'Next

            'DONE: If server full then queue, If GM/Admin let in
            If CLIENTs.Count > _config.ServerPlayerLimit And client.Access <= AccessLevel.Player Then
                ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf client.EnQueue))
            Else
                SendLoginOk(client)
            End If

            'DONE: Addons info reading
            Dim decompressBuffer(packet.Data.Length - packet.Offset) As Byte
            Array.Copy(packet.Data, packet.Offset, decompressBuffer, 0, packet.Data.Length - packet.Offset)
            packet.Data = DeCompress(decompressBuffer)
            packet.Offset = 0
            'DumpPacket(packet.Data)

            Dim AddOnsNames As New List(Of String)
            Dim AddOnsHashes As New List(Of UInteger)
            'Dim AddOnsConsoleWrite As String = String.Format("[{0}:{1}] Client addons loaded:", client.IP, client.Port)
            While packet.Offset < clientAddOnsSize
                AddOnsNames.Add(packet.GetString)
                AddOnsHashes.Add(packet.GetUInt32)
                packet.GetInt32() 'Unk7
                packet.GetInt8() 'Unk6
                'AddOnsConsoleWrite &= String.Format("{0}{1} AddOnName: [{2,-30}], AddOnHash: [{3:X}]", vbNewLine, vbTab, AddOnsNames(AddOnsNames.Count - 1), AddOnsHashes(AddOnsHashes.Count - 1))
            End While
            'Log.WriteLine(LogType.DEBUG, AddOnsConsoleWrite)

            'DONE: Build mysql addons query
            'Not needed already - in 1.11 addons list is removed.

            'DONE: Send packet
            Dim addOnsEnable As New PacketClass(OPCODES.SMSG_ADDON_INFO)
            For i As Integer = 0 To AddOnsNames.Count - 1
                If IO.File.Exists(String.Format("interface\{0}.pub", AddOnsNames(i))) AndAlso (AddOnsHashes(i) <> &H1C776D01UI) Then
                    'We have hash data
                    addOnsEnable.AddInt8(2)                    'AddOn Type [1-enabled, 0-banned, 2-blizzard]
                    addOnsEnable.AddInt8(1)                    'Unk

                    Dim fs As New IO.FileStream(String.Format("interface\{0}.pub", AddOnsNames(i)), IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 258, IO.FileOptions.SequentialScan)
                    Dim fb(256) As Byte
                    fs.Read(fb, 0, 257)

                    'NOTE: Read from file
                    addOnsEnable.AddByteArray(fb)
                    addOnsEnable.AddInt32(0)
                    addOnsEnable.AddInt8(0)
                Else
                    'We don't have hash data or already sent to client
                    addOnsEnable.AddInt8(2)                    'AddOn Type [1-enabled, 0-banned, 2-blizzard]
                    addOnsEnable.AddInt8(1)                    'Unk
                    addOnsEnable.AddInt32(0)
                    addOnsEnable.AddInt16(0)
                End If
            Next
            client.Send(addOnsEnable)
            addOnsEnable.Dispose()
        End Sub

        Public Sub On_CMSG_PING(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()

            Dim response As New PacketClass(OPCODES.SMSG_PONG)
            response.AddInt32(packet.GetInt32)
            client.Send(response)

            If Not client.Character Is Nothing Then
                client.Character.Latency = packet.GetInt32
            End If

            'Log.WriteLine(LogType.NETWORK, "[{0}:{1}] SMSG_PONG [{2}]", client.IP, client.Port, client.Character.Latency)
        End Sub

        Public Sub On_CMSG_UPDATE_ACCOUNT_DATA(ByRef packet As PacketClass, ByRef client As ClientClass)
            Try
                If (packet.Data.Length - 1) < 13 Then Exit Sub
                packet.GetInt16()
                Dim DataID As UInteger = packet.GetUInt32
                Dim UncompressedSize As UInteger = packet.GetUInt32

                Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_UPDATE_ACCOUNT_DATA [ID={2} Size={3}]", client.IP, client.Port, DataID, UncompressedSize)
                If DataID > 7 Then Exit Sub

                'TODO: How does Mangos Zero Handle the Account Data For the Character?
                'Dim AccData As New DataTable
                'AccountDatabase.Query(String.Format("SELECT account_id FROM accounts WHERE username = ""{0}"";", client.Account), AccData)
                'If AccData.Rows.Count = 0 Then
                '    Log.WriteLine(LogType.WARNING, "[{0}:{1}] CMSG_UPDATE_ACCOUNT_DATA [Account ID not found]", client.IP, client.Port)
                '    Exit Sub
                'End If

                'Dim AccID As Integer = CType(AccData.Rows(0).Item("account_id"), Integer)
                'AccData.Clear()

                'DONE: Clear the entry
                'If UncompressedSize = 0 Then
                '    AccountDatabase.Update(String.Format("UPDATE `account_data` SET `account_data{0}`='' WHERE `account_id`={1}", DataID, AccID))
                '    Exit Sub
                'End If

                'DONE: Can not handle more than 65534 bytes
                'If UncompressedSize >= 65534 Then
                '    Log.WriteLine(LogType.WARNING, "[{0}:{1}] CMSG_UPDATE_ACCOUNT_DATA [Invalid uncompressed size]", client.IP, client.Port)
                '    Exit Sub
                'End If

                Dim ReceivedPacketSize As Integer = packet.Data.Length - packet.Offset
                'Dim dataStr As String
                'DONE: Check if it's compressed, if so, decompress it
                'If UncompressedSize > ReceivedPacketSize Then
                '    Dim compressedBuffer(ReceivedPacketSize - 1) As Byte
                '    Array.Copy(packet.Data, packet.Offset, compressedBuffer, 0, compressedBuffer.Length)
                '
                '    dataStr = ToHex(DeCompress(compressedBuffer))
                'Else
                '    dataStr = ToHex(packet.Data, packet.Offset)
                'End If

                'AccountDatabase.Update(String.Format("UPDATE `account_data` SET `account_data{0}`={2} WHERE `account_id`={1};", DataID, AccID, dataStr))

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error while updating account data.{0}", vbNewLine & e.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_REQUEST_ACCOUNT_DATA(ByRef packet As PacketClass, ByRef client As ClientClass)
            If (packet.Data.Length - 1) < 9 Then Exit Sub
            packet.GetInt16()
            Dim DataID As UInteger = packet.GetUInt32
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_REQUEST_ACCOUNT_DATA [ID={2}]", client.IP, client.Port, DataID)
            If DataID > 7 Then Exit Sub

            Dim FoundData As Boolean = False
            'Dim AccData As New DataTable
            'AccountDatabase.Query(String.Format("SELECT account_id FROM accounts WHERE username = ""{0}"";", client.Account), AccData)
            'If AccData.Rows.Count > 0 Then
            '    Dim AccID As Integer = CType(AccData.Rows(0).Item("account_id"), Integer)
            '
            '    AccData.Clear()
            '    AccountDatabase.Query(String.Format("SELECT `account_data{1}` FROM account_data WHERE account_id = {0}", AccID, DataID), AccData)
            '    If AccData.Rows.Count > 0 Then FoundData = True
            'End If

            Dim response As New PacketClass(OPCODES.SMSG_UPDATE_ACCOUNT_DATA)
            response.AddUInt32(DataID)

            'If FoundData = False Then
            response.AddInt32(0) 'Uncompressed buffer length
            'Else
            'Dim AccountData() As Byte = AccData.Rows(0).Item("account_data" & DataID)
            'If AccountData.Length > 0 Then
            '    response.AddInt32(AccountData.Length) 'Uncompressed buffer length
            'DONE: Compress buffer if it's longer than 200 bytes
            'If AccountData.Length > 200 Then
            '    Dim CompressedBuffer() As Byte = Compress(AccountData, 0, AccountData.Length)
            '    response.AddByteArray(CompressedBuffer)
            'Else
            '    response.AddByteArray(AccountData)
            'End If
            'Else
            '    response.AddInt32(0) 'Uncompressed buffer length
            'End If
            'End If

            client.Send(response)
            response.Dispose()
        End Sub

        Public Sub On_CMSG_CHAR_ENUM(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHAR_ENUM", client.IP, client.Port)

            'DONE: Query Characters DB
            Dim response As New PacketClass(OPCODES.SMSG_CHAR_ENUM)
            Dim MySQLQuery As New DataTable
            Dim Account_ID As Integer

            Try
                AccountDatabase.Query(String.Format("SELECT id FROM account WHERE username = '{0}';", client.Account), MySQLQuery)
                Account_ID = MySQLQuery.Rows(0).Item("id")
                MySQLQuery.Clear()
                CharacterDatabase.Query(String.Format("SELECT * FROM characters WHERE account_id = '{0}' ORDER BY char_guid;", Account_ID), MySQLQuery)

                'DONE: Make The Packet
                response.AddInt8(MySQLQuery.Rows.Count)
                For i As Integer = 0 To MySQLQuery.Rows.Count - 1
                    Dim DEAD As Boolean = False
                    Dim DeadMySQLQuery As New DataTable
                    CharacterDatabase.Query(String.Format("SELECT COUNT(*) FROM corpse WHERE player = {0};", MySQLQuery.Rows(i).Item("char_guid")), DeadMySQLQuery)
                    If CInt(DeadMySQLQuery.Rows(0).Item(0)) > 0 Then DEAD = True
                    Dim PetQuery As New DataTable
                    CharacterDatabase.Query(String.Format("SELECT modelid, level, entry FROM character_pet WHERE owner = '{0}';", MySQLQuery.Rows(i).Item("char_guid")), PetQuery)

                    response.AddInt64(MySQLQuery.Rows(i).Item("char_guid"))
                    response.AddString(MySQLQuery.Rows(i).Item("char_name"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_race"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_class"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_gender"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_skin"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_face"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_hairStyle"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_hairColor"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_facialHair"))
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_level"))
                    response.AddInt32(MySQLQuery.Rows(i).Item("char_zone_id"))
                    response.AddInt32(MySQLQuery.Rows(i).Item("char_map_id"))
                    response.AddSingle(MySQLQuery.Rows(i).Item("char_positionX"))
                    response.AddSingle(MySQLQuery.Rows(i).Item("char_positionY"))
                    response.AddSingle(MySQLQuery.Rows(i).Item("char_positionZ"))
                    response.AddInt32(MySQLQuery.Rows(i).Item("char_guildId"))

                    Dim playerState As UInteger = CharacterFlagState.CHARACTER_FLAG_NONE
                    Dim ForceRestrictions As UInteger = MySQLQuery.Rows(i).Item("force_restrictions")
                    If (ForceRestrictions And ForceRestrictionFlags.RESTRICT_TRANSFER) Then
                        playerState += CharacterFlagState.CHARACTER_FLAG_LOCKED_FOR_TRANSFER
                    End If
                    If (ForceRestrictions And ForceRestrictionFlags.RESTRICT_BILLING) Then
                        playerState += CharacterFlagState.CHARACTER_FLAG_LOCKED_BY_BILLING
                    End If
                    If (ForceRestrictions And ForceRestrictionFlags.RESTRICT_RENAME) Then
                        playerState += CharacterFlagState.CHARACTER_FLAG_RENAME
                    End If
                    If DEAD Then
                        playerState += CharacterFlagState.CHARACTER_FLAG_GHOST
                    End If

                    response.AddUInt32(playerState)
                    response.AddInt8(MySQLQuery.Rows(i).Item("char_restState"))

                    Dim PetModel As Integer = 0
                    Dim PetLevel As Integer = 0
                    Dim PetFamily As Integer = 0

                    If PetQuery.Rows.Count > 0 Then
                        PetModel = PetQuery.Rows(0).Item("modelid")
                        PetLevel = PetQuery.Rows(0).Item("level")
                        Dim PetFamilyQuery As New DataTable
                        WorldDatabase.Query(String.Format("SELECT family FROM creature_template WHERE entry = '{0}'", PetQuery.Rows(0).Item("entry")), PetFamilyQuery)
                        PetFamily = PetFamilyQuery.Rows(0).Item("family")
                    End If

                    response.AddInt32(PetModel)
                    response.AddInt32(PetLevel)
                    response.AddInt32(PetFamily)

                    'DONE: Get items
                    Dim GUID As Long = MySQLQuery.Rows(i).Item("char_guid")
                    Dim ItemsMySQLQuery As New DataTable
                    Dim characterDB As String = CharacterDatabase.SqldbName
                    Dim worldDB As String = WorldDatabase.SqldbName
                    CharacterDatabase.Query(String.Format("SELECT item_slot, displayid, inventorytype FROM " & characterDB & ".characters_inventory, " & worldDB & ".item_template WHERE item_bag = {0} AND item_slot <> 255 AND entry = item_id  ORDER BY item_slot;", GUID), ItemsMySQLQuery)

                    Dim e As IEnumerator = ItemsMySQLQuery.Rows.GetEnumerator
                    e.Reset()
                    e.MoveNext()
                    Dim r As DataRow = e.Current

                    'DONE: Add model info
                    For slot As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END '- 1
                        If r Is Nothing OrElse CInt(r.Item("item_slot")) <> slot Then
                            'No equiped item in this slot
                            response.AddInt32(0) 'Item Model
                            response.AddInt8(0)  'Item Slot
                        Else
                            'DONE: Do not show helmet or cloak
                            If ((ForceRestrictions And ForceRestrictionFlags.RESTRICT_HIDECLOAK) AndAlso CByte(r.Item("item_slot")) = EquipmentSlots.EQUIPMENT_SLOT_BACK) OrElse
                               ((ForceRestrictions And ForceRestrictionFlags.RESTRICT_HIDEHELM) AndAlso CByte(r.Item("item_slot")) = EquipmentSlots.EQUIPMENT_SLOT_HEAD) Then
                                response.AddInt32(0) 'Item Model
                                response.AddInt8(0)  'Item Slot
                            Else
                                response.AddInt32(r.Item("displayid"))          'Item Model
                                response.AddInt8(r.Item("inventorytype"))       'Item Slot
                            End If

                            e.MoveNext()
                            r = e.Current
                        End If
                    Next
                Next i

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "[{0}:{1}] Unable to enum characters. [{2}]", client.IP, client.Port, e.Message)
                'TODO: Find what opcode officials use
                response = New PacketClass(OPCODES.SMSG_CHAR_CREATE)
                response.AddInt8(CharResponse.CHAR_LIST_FAILED)
            End Try

            client.Send(response)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CHAR_ENUM", client.IP, client.Port)
        End Sub

        Public Sub On_CMSG_CHAR_DELETE(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHAR_DELETE", client.IP, client.Port)

            Dim response As New PacketClass(OPCODES.SMSG_CHAR_DELETE)
            packet.GetInt16()
            Dim guid As ULong = packet.GetUInt64()

            Try
                Dim q As New DataTable

                'Done: Fixed packet manipulation protection
                AccountDatabase.Query(String.Format("SELECT id FROM account WHERE username = ""{0}"";", client.Account), q)
                If q.Rows.Count = 0 Then
                    Exit Sub
                End If

                CharacterDatabase.Query(String.Format("SELECT char_guid FROM characters WHERE account_id = ""{0}"" AND char_guid = ""{1}"";", q.Rows(0).Item("id"), guid), q)
                If q.Rows.Count = 0 Then
                    response.AddInt8(AuthResult.WOW_FAIL_BANNED)
                    client.Send(response)
                    Ban_Account(client.Account, "Packet Manipulation/Character Deletion")
                    client.Delete()
                    Exit Sub
                End If
                q.Clear()

                CharacterDatabase.Query(String.Format("SELECT item_guid FROM characters_inventory WHERE item_bag = {0};", guid), q)
                For Each row As DataRow In q.Rows
                    'DONE: Delete items
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_guid = ""{0}"";", row.Item("item_guid")))
                    'DONE: Delete items in bags
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_inventory WHERE item_bag = ""{0}"";", CULng(row.Item("item_guid")) + GUID_ITEM))
                Next
                CharacterDatabase.Query(String.Format("SELECT item_guid FROM characters_inventory WHERE item_owner = {0};", guid), q)
                q.Clear()

                CharacterDatabase.Query(String.Format("SELECT mail_id FROM characters_mail WHERE mail_receiver = ""{0}"";", guid), q)
                For Each row As DataRow In q.Rows
                    'TODO: Return mails?
                    'DONE: Delete mails
                    CharacterDatabase.Update(String.Format("DELETE FROM characters_mail WHERE mail_id = ""{0}"";", row.Item("mail_id")))
                    'DONE: Delete mail items
                    CharacterDatabase.Update(String.Format("DELETE FROM mail_items WHERE mail_id = ""{0}"";", row.Item("mail_id")))
                Next
                CharacterDatabase.Update(String.Format("DELETE FROM characters WHERE char_guid = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM characters_honor WHERE char_guid = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM characters_quests WHERE char_guid = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM character_social WHERE guid = '{0}' OR friend = '{0}';", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM characters_spells WHERE guid = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM petitions WHERE petition_owner = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM auctionhouse WHERE auction_owner = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM characters_tickets WHERE char_guid = ""{0}"";", guid))
                CharacterDatabase.Update(String.Format("DELETE FROM corpse WHERE guid = ""{0}"";", guid))
                q.Clear()

                CharacterDatabase.Query(String.Format("SELECT guild_id FROM guilds WHERE guild_leader = ""{0}"";", guid), q)
                If q.Rows.Count > 0 Then
                    CharacterDatabase.Update(String.Format("UPDATE characters SET char_guildid = 0, char_guildrank = 0, char_guildpnote = '', charguildoffnote = '' WHERE char_guildid = ""{0}"";", q.Rows(0).Item("guild_id")))
                    CharacterDatabase.Update(String.Format("DELETE FROM guild WHERE guild_id = ""{0}"";", q.Rows(0).Item("guild_id")))
                End If
                response.AddInt8(CharResponse.CHAR_DELETE_SUCCESS) ' Changed in 1.12.x client branch?
            Catch e As Exception
                response.AddInt8(CharResponse.CHAR_DELETE_FAILED)
            End Try

            client.Send(response)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CHAR_DELETE [{2:X}]", client.IP, client.Port, guid)
        End Sub

        Public Sub On_CMSG_CHAR_RENAME(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim GUID As Long = packet.GetInt64()
            Dim Name As String = packet.GetString
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHAR_RENAME [{2}:{3}]", client.IP, client.Port, GUID, Name)

            Dim ErrCode As Byte = ATLoginFlags.AT_LOGIN_RENAME

            'DONE: Check for existing name
            Dim q As New DataTable
            CharacterDatabase.Query(String.Format("SELECT char_name FROM characters WHERE char_name LIKE ""{0}"";", Name), q)
            If q.Rows.Count > 0 Then
                ErrCode = CharResponse.CHAR_CREATE_NAME_IN_USE
            End If

            'DONE: Do the rename
            If ErrCode = ATLoginFlags.AT_LOGIN_RENAME Then CharacterDatabase.Update(String.Format("UPDATE characters SET char_name = ""{1}"", force_restrictions = 0 WHERE char_guid = {0};", GUID, Name))

            'DONE: Send response
            Dim response As New PacketClass(OPCODES.SMSG_CHAR_RENAME)
            response.AddInt8(ErrCode)
            client.Send(response)
            response.Dispose()

            On_CMSG_CHAR_ENUM(Nothing, client)
        End Sub

        Public Sub On_CMSG_CHAR_CREATE(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()

            Dim Name As String = packet.GetString

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CHAR_CREATE [{2}]", client.IP, client.Port, Name)

            Dim Race As Byte = packet.GetInt8
            Dim Classe As Byte = packet.GetInt8
            Dim Gender As Byte = packet.GetInt8
            Dim Skin As Byte = packet.GetInt8
            Dim Face As Byte = packet.GetInt8
            Dim HairStyle As Byte = packet.GetInt8
            Dim HairColor As Byte = packet.GetInt8
            Dim FacialHair As Byte = packet.GetInt8
            Dim OutfitId As Byte = packet.GetInt8

            Dim result As Integer = CharResponse.CHAR_CREATE_DISABLED

            'Try to pass the packet to one of World Servers
            Try
                If WorldServer.Worlds.ContainsKey(0) Then
                    result = WorldServer.Worlds(0).ClientCreateCharacter(client.Account, Name, Race, Classe, Gender, Skin, Face, HairStyle, HairColor, FacialHair, OutfitId)
                ElseIf WorldServer.Worlds.ContainsKey(1) Then
                    result = WorldServer.Worlds(1).ClientCreateCharacter(client.Account, Name, Race, Classe, Gender, Skin, Face, HairStyle, HairColor, FacialHair, OutfitId)
                End If
            Catch ex As Exception
                result = CharResponse.CHAR_CREATE_ERROR
                Log.WriteLine(LogType.FAILED, "[{0}:{1}] Character creation failed!{2}{3}", client.IP, client.Port, vbNewLine, ex.ToString)
            End Try

            Dim response As New PacketClass(OPCODES.SMSG_CHAR_CREATE)
            response.AddInt8(result)
            client.Send(response)
        End Sub

        Public Sub On_CMSG_PLAYER_LOGIN(ByRef packet As PacketClass, ByRef client As ClientClass)
            Dim GUID As ULong = 0
            packet.GetInt16()               'int16 unknown
            GUID = packet.GetUInt64()       'uint64 guid
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PLAYER_LOGIN [0x{2:X}]", client.IP, client.Port, GUID)

            If client.Character Is Nothing Then
                client.Character = New CharacterObject(GUID, client)
            Else
                If client.Character.GUID <> GUID Then
                    client.Character.Dispose()
                    client.Character = New CharacterObject(GUID, client)
                Else
                    client.Character.ReLoad()
                End If
            End If

            If WorldServer.InstanceCheck(client, client.Character.Map) Then
                client.Character.GetWorld.ClientConnect(client.Index, client.GetClientInfo)
                client.Character.IsInWorld = True
                client.Character.GetWorld.ClientLogin(client.Index, client.Character.GUID)

                client.Character.OnLogin()
            Else
                Log.WriteLine(LogType.FAILED, "[{0:000000}] Unable to login: WORLD SERVER DOWN", client.Index)

                client.Character.Dispose()
                client.Character = Nothing
                Dim r As New PacketClass(OPCODES.SMSG_CHARACTER_LOGIN_FAILED)
                Try
                    r.AddInt8(CharResponse.CHAR_LOGIN_NO_WORLD)
                    client.Send(r)
                Catch ex As Exception
                    Log.WriteLine(LogType.FAILED, "[{0:000000}] Unable to login: {1}", client.Index, ex.ToString)

                    client.Character.Dispose()
                    client.Character = Nothing

                    Dim a As New PacketClass(OPCODES.SMSG_CHARACTER_LOGIN_FAILED)
                    Try
                        a.AddInt8(CharResponse.CHAR_LOGIN_FAILED)
                        client.Send(a)
                    Finally
                        r.Dispose()
                    End Try
                End Try
            End If
        End Sub

        'Leak is with in this code. Needs a rewrite to correct the leak. This only effects the CPU Usage.
        'Happens when the client disconnects from the server.
        Public Sub On_CMSG_PLAYER_LOGOUT(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PLAYER_LOGOUT", client.IP, client.Port)
            client.Character.OnLogout()

            client.Character.GetWorld.ClientDisconnect(client.Index) 'Likely the cause of it
            client.Character.Dispose()
            client.Character = Nothing
        End Sub

        Public Sub On_MSG_MOVE_WORLDPORT_ACK(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_MOVE_WORLDPORT_ACK", client.IP, client.Port)

            Try
                If Not WorldServer.InstanceCheck(client, client.Character.Map) Then Exit Sub

                If client.Character.IsInWorld Then
                    'Inside server transfer
                    client.Character.GetWorld.ClientLogin(client.Index, client.Character.GUID)
                Else
                    'Inter-server transfer
                    client.Character.ReLoad()

                    client.Character.GetWorld.ClientConnect(client.Index, client.GetClientInfo)
                    client.Character.IsInWorld = True
                    client.Character.GetWorld.ClientLogin(client.Index, client.Character.GUID)
                End If
            Catch ex As Exception
                Log.WriteLine(LogType.CRITICAL, "{0}", ex.ToString)
            End Try
        End Sub

    End Module
End Namespace