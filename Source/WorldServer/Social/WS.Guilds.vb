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
Imports mangosVB.Common.Globals
Imports mangosVB.Shared

Public Module WS_Guilds

#Region "WS.Guilds.Petition"
    Public Sub SendPetitionActivate(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
        If WORLD_CREATUREs.ContainsKey(cGUID) = False Then Exit Sub
        Dim Count As Byte = 3
        If WORLD_CREATUREs(cGUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_VENDOR Then
            Count = 1
        End If

        Dim packet As New PacketClass(OPCODES.SMSG_PETITION_SHOWLIST)
        packet.AddUInt64(cGUID)
        packet.AddInt8(1)

        If Count = 1 Then
            packet.AddInt32(1) 'Index
            packet.AddInt32(PETITION_GUILD)
            packet.AddInt32(16161) 'Charter display ID
            packet.AddInt32(PETITION_GUILD_PRICE)
            packet.AddInt32(0) 'Unknown
            packet.AddInt32(9) 'Required signatures
        End If

        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub On_CMSG_PETITION_SHOWLIST(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SHOWLIST [GUID={2:X}]", client.IP, client.Port, GUID)

        SendPetitionActivate(client.Character, GUID)
    End Sub

    Public Sub On_CMSG_PETITION_BUY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 26 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        packet.GetInt64()
        packet.GetInt32()
        Dim Name As String = packet.GetString
        If (packet.Data.Length - 1) < 26 + Name.Length + 5 * 8 + 2 + 1 + 4 + 4 Then Exit Sub
        packet.GetInt64()
        packet.GetInt64()
        packet.GetInt64()
        packet.GetInt64()
        packet.GetInt64()
        packet.GetInt16()
        packet.GetInt8()
        Dim Index As Integer = packet.GetInt32
        packet.GetInt32()
        If WORLD_CREATUREs.ContainsKey(GUID) = False OrElse (WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_PETITIONER) = 0 Then Exit Sub

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_BUY [GuildName={2}]", client.IP, client.Port, Name)

        Dim CharterID As Integer = 0
        Dim CharterPrice As Integer = 0
        If client.Character.GuildID <> 0 Then Exit Sub
        CharterID = PETITION_GUILD
        CharterPrice = PETITION_GUILD_PRICE

        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT guild_id FROM guilds WHERE guild_name = '{0}'", Name), q)
        If q.Rows.Count > 0 Then
            SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_NAME_EXISTS, Name)
        End If
        q.Clear()
        If ValidateGuildName(Name) = False Then
            SendGuildResult(client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_NAME_INVALID, Name)
        End If

        If ITEMDatabase.ContainsKey(CharterID) = False Then
            Dim response As New PacketClass(OPCODES.SMSG_BUY_FAILED)
            response.AddUInt64(GUID)
            response.AddInt32(CharterID)
            response.AddInt8(BUY_ERROR.BUY_ERR_CANT_FIND_ITEM)
            client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If client.Character.Copper < CharterPrice Then
            Dim response As New PacketClass(OPCODES.SMSG_BUY_FAILED)
            response.AddUInt64(GUID)
            response.AddInt32(CharterID)
            response.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
            client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        client.Character.Copper -= CharterPrice
        client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, client.Character.Copper)
        client.Character.SendCharacterUpdate(False)

        'Client.Character.AddItem(PETITION_GUILD)
        Dim tmpItem As New ItemObject(CharterID, client.Character.GUID) With {
            .StackCount = 1
        }
        tmpItem.AddEnchantment(tmpItem.GUID - GUID_ITEM, 0, 0, 0)

        If client.Character.ItemADD(tmpItem) Then
            'Save petition into database
            CharacterDatabase.Update(String.Format("INSERT INTO petitions (petition_id, petition_itemGuid, petition_owner, petition_name, petition_type, petition_signedMembers) VALUES ({0}, {0}, {1}, '{2}', {3}, 0);", tmpItem.GUID - GUID_ITEM, client.Character.GUID - GUID_PLAYER, Name, 9))
        Else
            'No free inventory slot
            tmpItem.Delete()
        End If
    End Sub

    Public Sub SendPetitionSignatures(ByRef objCharacter As CharacterObject, ByVal iGUID As ULong)
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM petitions WHERE petition_itemGuid = " & iGUID - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        Dim response As New PacketClass(OPCODES.SMSG_PETITION_SHOW_SIGNATURES)
        response.AddUInt64(iGUID)                                                        'itemGuid
        response.AddUInt64(MySQLQuery.Rows(0).Item("petition_owner"))                    'GuildOwner
        response.AddInt32(MySQLQuery.Rows(0).Item("petition_id"))                        'PetitionGUID
        response.AddInt8(MySQLQuery.Rows(0).Item("petition_signedMembers"))              'PlayersSigned

        For i As Byte = 1 To MySQLQuery.Rows(0).Item("petition_signedMembers")
            response.AddUInt64(MySQLQuery.Rows(0).Item("petition_signedMember" & i))     'SignedGUID
            response.AddInt32(0)                                                         'Unk
        Next

        objCharacter.client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_PETITION_SHOW_SIGNATURES(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SHOW_SIGNATURES [GUID={2:X}]", client.IP, client.Port, GUID)

        SendPetitionSignatures(client.Character, GUID)
    End Sub

    Public Sub On_CMSG_PETITION_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim PetitionGUID As Integer = packet.GetInt32
        Dim itemGuid As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_QUERY [pGUID={3} iGUID={2:X}]", client.IP, client.Port, itemGuid, PetitionGUID)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM petitions WHERE petition_itemGuid = " & itemGuid - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        Dim response As New PacketClass(OPCODES.SMSG_PETITION_QUERY_RESPONSE)
        response.AddInt32(MySQLQuery.Rows(0).Item("petition_id"))               'PetitionGUID
        response.AddUInt64(MySQLQuery.Rows(0).Item("petition_owner"))           'GuildOwner
        response.AddString(MySQLQuery.Rows(0).Item("petition_name"))            'GuildName
        response.AddInt8(0)         'Unk1
        If CByte(MySQLQuery.Rows(0).Item("petition_type")) = 9 Then
            response.AddInt32(9)
            response.AddInt32(9)
            response.AddInt32(0) 'bypass client - side limitation, a different value is needed here for each petition
        Else
            response.AddInt32(CByte(MySQLQuery.Rows(0).Item("petition_type")) - 1)
            response.AddInt32(CByte(MySQLQuery.Rows(0).Item("petition_type")) - 1)
            response.AddInt32(CByte(MySQLQuery.Rows(0).Item("petition_type"))) 'bypass client - side limitation, a different value is needed here for each petition
        End If
        '9x int32
        response.AddInt32(0)
        response.AddInt32(0)
        response.AddInt32(0)
        response.AddInt32(0)
        response.AddInt16(0)
        response.AddInt32(0)
        response.AddInt32(0)
        response.AddInt32(0)
        response.AddInt32(0)
        If CByte(MySQLQuery.Rows(0).Item("petition_type")) = 9 Then
            response.AddInt32(0)
        Else
            response.AddInt32(1)
        End If
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_MSG_PETITION_RENAME(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim itemGuid As ULong = packet.GetUInt64
        Dim NewName As String = packet.GetString

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_PETITION_RENAME [NewName={3} GUID={2:X}]", client.IP, client.Port, itemGuid, NewName)

        CharacterDatabase.Update("UPDATE petitions SET petition_name = '" & NewName & "' WHERE petition_itemGuid = " & itemGuid - GUID_ITEM & ";")

        'DONE: Update client-side name information
        Dim response As New PacketClass(OPCODES.MSG_PETITION_RENAME)
        response.AddUInt64(itemGuid)
        response.AddString(NewName)
        response.AddInt32(itemGuid - GUID_ITEM)
        client.Send(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_OFFER_PETITION(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim PetitionType As Integer = packet.GetInt32
        Dim itemGuid As ULong = packet.GetUInt64
        Dim GUID As ULong = packet.GetUInt64
        If CHARACTERs.ContainsKey(GUID) = False Then Exit Sub
        'If CHARACTERs(GUID).IgnoreList.Contains(Client.Character.GUID) Then Exit Sub
        If CHARACTERs(GUID).IsHorde <> client.Character.IsHorde Then Exit Sub

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_OFFER_PETITION [GUID={2:X} Petition={3}]", client.IP, client.Port, GUID, itemGuid)

        SendPetitionSignatures(CHARACTERs(GUID), itemGuid)
    End Sub

    Public Sub On_CMSG_PETITION_SIGN(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim itemGuid As ULong = packet.GetUInt64
        Dim Unk As Integer = packet.GetInt8

        'TODO: Check if the player already has signed

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SIGN [GUID={2:X} Unk={3}]", client.IP, client.Port, itemGuid, Unk)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT petition_signedMembers, petition_owner FROM petitions WHERE petition_itemGuid = " & itemGuid - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        CharacterDatabase.Update("UPDATE petitions SET petition_signedMembers = petition_signedMembers + 1, petition_signedMember" & (MySQLQuery.Rows(0).Item("petition_signedMembers") + 1) & " = " & client.Character.GUID & " WHERE petition_itemGuid = " & itemGuid - GUID_ITEM & ";")

        'DONE: Send result to both players
        Dim response As New PacketClass(OPCODES.SMSG_PETITION_SIGN_RESULTS)
        response.AddUInt64(itemGuid)
        response.AddUInt64(client.Character.GUID)
        response.AddInt32(PetitionSignError.PETITIONSIGN_OK)
        client.SendMultiplyPackets(response)
        If CHARACTERs.ContainsKey(MySQLQuery.Rows(0).Item("petition_owner")) Then CHARACTERs(MySQLQuery.Rows(0).Item("petition_owner")).client.SendMultiplyPackets(response)
        response.Dispose()
    End Sub

    Public Sub On_MSG_PETITION_DECLINE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim itemGuid As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_PETITION_DECLINE [GUID={2:X}]", client.IP, client.Port, itemGuid)

        'DONE: Get petition owner
        Dim q As New DataTable
        CharacterDatabase.Query("SELECT petition_owner FROM petitions WHERE petition_itemGuid = " & itemGuid - GUID_ITEM & " LIMIT 1;", q)

        'DONE: Send message to player
        Dim response As New PacketClass(OPCODES.MSG_PETITION_DECLINE)
        response.AddUInt64(client.Character.GUID)
        If q.Rows.Count > 0 AndAlso CHARACTERs.ContainsKey(q.Rows(0).Item("petition_owner")) Then CHARACTERs(q.Rows(0).Item("petition_owner")).client.SendMultiplyPackets(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_TURN_IN_PETITION(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim itemGuid As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TURN_IN_PETITION [GUID={2:X}]", client.IP, client.Port, itemGuid)

        client.Character.ItemREMOVE(itemGuid, True, True)
    End Sub

#End Region

#Region "WS.Guilds.Handlers"

    'Basic Tabard Framework
    Public Sub SendTabardActivate(ByRef objCharacter As CharacterObject, ByVal cGUID As ULong)
        Dim packet As New PacketClass(OPCODES.MSG_TABARDVENDOR_ACTIVATE)
        packet.AddUInt64(cGUID)
        objCharacter.client.Send(packet)
        packet.Dispose()
    End Sub

    Public Sub On_MSG_TABARDVENDOR_ACTIVATE(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_TABARDVENDOR_ACTIVATE [GUID={2}]", client.IP, client.Port, GUID)

        SendTabardActivate(client.Character, GUID)
    End Sub

    Public Function GetGuildBankTabPrice(ByVal TabID As Byte) As Integer
        Select Case TabID
            Case 0
                Return 100
            Case 1
                Return 250
            Case 2
                Return 500
            Case 3
                Return 1000
            Case 4
                Return 2500
            Case 5
                Return 5000
            Case Else
                Return 0
        End Select
    End Function

    Public Sub SendGuildResult(ByRef client As ClientClass, ByVal Command As GuildCommand, ByVal Result As GuildError, Optional ByVal Text As String = "")
        Dim response As New PacketClass(OPCODES.SMSG_GUILD_COMMAND_RESULT)
        response.AddInt32(Command)
        response.AddString(Text)
        response.AddInt32(Result)
        client.Send(response)
        response.Dispose()
    End Sub

#End Region

End Module