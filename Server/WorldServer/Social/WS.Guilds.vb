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

Imports mangosVB.Common.BaseWriter


Public Module WS_Guilds

#Region "WS.Guilds.Constants"


    Public Const PETITION_GUILD_PRICE As Integer = 1000
    Public Const PETITION_GUILD As Integer = 5863       'Guild Charter, ItemFlags = &H2000
    Public Const PETITION_2v2_PRICE As Integer = 800000
    Public Const PETITION_2v2 As Integer = 23560
    Public Const PETITION_3v3_PRICE As Integer = 1200000
    Public Const PETITION_3v3 As Integer = 23561
    Public Const PETITION_5v5_PRICE As Integer = 2000000
    Public Const PETITION_5v5 As Integer = 23562

    Public Const TABARD_ITEM As Integer = 5976


#End Region
#Region "WS.Guilds.Petition"

    'ERR_PETITION_FULL
    'ERR_PETITION_NOT_SAME_SERVER
    'ERR_PETITION_NOT_ENOUGH_SIGNATURES
    'ERR_PETITION_CREATOR
    'ERR_PETITION_IN_GUILD
    'ERR_PETITION_ALREADY_SIGNED
    'ERR_PETITION_DECLINED_S
    'ERR_PETITION_SIGNED_S
    'ERR_PETITION_SIGNED
    'ERR_PETITION_OFFERED_S
    Public Enum PetitionSignError As Integer
        PETITIONSIGN_OK = 0                     ':Closes the window
        PETITIONSIGN_ALREADY_SIGNED = 1         'You have already signed that guild charter
        PETITIONSIGN_ALREADY_IN_GUILD = 2       'You are already in a guild
        PETITIONSIGN_CANT_SIGN_OWN = 3          'You can's sign own guild charter
        PETITIONSIGN_NOT_SERVER = 4             'That player is not from your server
    End Enum
    Public Enum PetitionTurnInError As Integer
        PETITIONTURNIN_OK = 0                   ':Closes the window
        PETITIONTURNIN_ALREADY_IN_GUILD = 2     'You are already in a guild
        PETITIONTURNIN_NEED_MORE_SIGNATURES = 4 'You need more signatures
    End Enum

    Public Sub SendPetitionActivate(ByRef c As CharacterObject, ByVal cGUID As ULong)
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
        Else
            packet.AddInt32(1) 'Index
            packet.AddInt32(PETITION_2v2)
            packet.AddInt32(16161) 'Charter display ID
            packet.AddInt32(PETITION_2v2_PRICE)
            packet.AddInt32(2) 'Unknown
            packet.AddInt32(2) 'Required signatures

            packet.AddInt32(2) 'Index
            packet.AddInt32(PETITION_3v3)
            packet.AddInt32(16161) 'Charter display ID
            packet.AddInt32(PETITION_3v3_PRICE)
            packet.AddInt32(3) 'Unknown
            packet.AddInt32(3) 'Required signatures

            packet.AddInt32(3) 'Index
            packet.AddInt32(PETITION_5v5)
            packet.AddInt32(16161) 'Charter display ID
            packet.AddInt32(PETITION_5v5_PRICE)
            packet.AddInt32(5) 'Unknown
            packet.AddInt32(5) 'Required signatures
        End If

        c.Client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub On_CMSG_PETITION_SHOWLIST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SHOWLIST [GUID={2:X}]", Client.IP, Client.Port, GUID)

        SendPetitionActivate(Client.Character, GUID)
    End Sub
    Public Sub On_CMSG_PETITION_BUY(ByRef packet As PacketClass, ByRef Client As ClientClass)
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

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_BUY [GuildName={2}]", Client.IP, Client.Port, Name)

        Dim CharterID As Integer = 0
        Dim CharterPrice As Integer = 0
        If Client.Character.GuildID <> 0 Then Exit Sub
        CharterID = PETITION_GUILD
        CharterPrice = PETITION_GUILD_PRICE

        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT guild_id FROM guilds WHERE guild_name = '{0}'", Name), q)
        If q.Rows.Count > 0 Then
            SendGuildResult(Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_NAME_EXISTS, Name)
        End If
        q.Clear()
        If ValidateGuildName(Name) = False Then
            SendGuildResult(Client, GuildCommand.GUILD_CREATE_S, GuildError.GUILD_NAME_INVALID, Name)
        End If

        If ITEMDatabase.ContainsKey(CharterID) = False Then
            Dim response As New PacketClass(OPCODES.SMSG_BUY_FAILED)
            response.AddUInt64(GUID)
            response.AddInt32(CharterID)
            response.AddInt8(BUY_ERROR.BUY_ERR_CANT_FIND_ITEM)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If
        If Client.Character.Copper < CharterPrice Then
            Dim response As New PacketClass(OPCODES.SMSG_BUY_FAILED)
            response.AddUInt64(GUID)
            response.AddInt32(CharterID)
            response.AddInt8(BUY_ERROR.BUY_ERR_NOT_ENOUGHT_MONEY)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        Client.Character.Copper -= CharterPrice
        Client.Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Client.Character.Copper)
        Client.Character.SendCharacterUpdate(False)

        'Client.Character.AddItem(PETITION_ITEM)
        Dim tmpItem As New ItemObject(CharterID, Client.Character.GUID)
        tmpItem.StackCount = 1
        tmpItem.AddEnchantment(tmpItem.GUID - GUID_ITEM, 0, 0, 0)
        If Client.Character.ItemADD(tmpItem) Then
            'Save petition into database
            CharacterDatabase.Update(String.Format("INSERT INTO petitions (petition_id, petition_itemGuid, petition_owner, petition_name, petition_type, petition_signedMembers) VALUES ({0}, {0}, {1}, '{2}', {3}, 0);", tmpItem.GUID - GUID_ITEM, Client.Character.GUID - GUID_PLAYER, Name, 9))
        Else
            'No free inventory slot
            tmpItem.Delete()
        End If
    End Sub

    Public Sub SendPetitionSignatures(ByRef c As CharacterObject, ByVal iGUID As ULong)
        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM petitions WHERE petition_itemGuid = " & iGUID - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        Dim response As New PacketClass(OPCODES.SMSG_PETITION_SHOW_SIGNATURES)
        response.AddUInt64(iGUID)                                                        'ItemGUID
        response.AddUInt64(MySQLQuery.Rows(0).Item("petition_owner"))                    'GuildOwner
        response.AddInt32(MySQLQuery.Rows(0).Item("petition_id"))                       'PetitionGUID
        response.AddInt8(MySQLQuery.Rows(0).Item("petition_signedMembers"))             'PlayersSigned

        For i As Byte = 1 To MySQLQuery.Rows(0).Item("petition_signedMembers")
            response.AddUInt64(MySQLQuery.Rows(0).Item("petition_signedMember" & i))                     'SignedGUID
            response.AddInt32(0)                                                                        'Unk
        Next

        c.Client.Send(response)
        response.Dispose()
    End Sub
    Public Sub On_CMSG_PETITION_SHOW_SIGNATURES(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SHOW_SIGNATURES [GUID={2:X}]", Client.IP, Client.Port, GUID)

        SendPetitionSignatures(Client.Character, GUID)
    End Sub
    Public Sub On_CMSG_PETITION_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 17 Then Exit Sub
        packet.GetInt16()
        Dim PetitionGUID As Integer = packet.GetInt32
        Dim ItemGUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_QUERY [pGUID={3} iGUID={2:X}]", Client.IP, Client.Port, ItemGUID, PetitionGUID)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT * FROM petitions WHERE petition_itemGuid = " & ItemGUID - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub




        Dim response As New PacketClass(OPCODES.SMSG_PETITION_QUERY_RESPONSE)
        response.AddInt32(MySQLQuery.Rows(0).Item("petition_id"))               'PetitionGUID
        response.AddUInt64(MySQLQuery.Rows(0).Item("petition_owner"))            'GuildOwner
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
        Client.Send(response)
        response.Dispose()
    End Sub


    Public Sub On_MSG_PETITION_RENAME(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim ItemGUID As ULong = packet.GetUInt64
        Dim NewName As String = packet.GetString

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_PETITION_RENAME [NewName={3} GUID={2:X}]", Client.IP, Client.Port, ItemGUID, NewName)

        CharacterDatabase.Update("UPDATE petitions SET petition_name = '" & NewName & "' WHERE petition_itemGuid = " & ItemGUID - GUID_ITEM & ";")

        'DONE: Update client-side name information
        Dim response As New PacketClass(OPCODES.MSG_PETITION_RENAME)
        response.AddUInt64(ItemGUID)
        response.AddString(NewName)
        response.AddInt32(ItemGUID - GUID_ITEM)
        Client.Send(response)
        response.Dispose()
    End Sub
    Public Sub On_CMSG_OFFER_PETITION(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim PetitionType As Integer = packet.GetInt32
        Dim ItemGUID As ULong = packet.GetUInt64
        Dim GUID As ULong = packet.GetUInt64
        If CHARACTERs.ContainsKey(GUID) = False Then Exit Sub
        'If CHARACTERs(GUID).IgnoreList.Contains(Client.Character.GUID) Then Exit Sub
        If CHARACTERs(GUID).IsHorde <> Client.Character.IsHorde Then Exit Sub

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_OFFER_PETITION [GUID={2:X} Petition={3}]", Client.IP, Client.Port, GUID, ItemGUID)

        SendPetitionSignatures(CHARACTERs(GUID), ItemGUID)
    End Sub
    Public Sub On_CMSG_PETITION_SIGN(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 14 Then Exit Sub
        packet.GetInt16()
        Dim ItemGUID As ULong = packet.GetUInt64
        Dim Unk As Integer = packet.GetInt8

        'TODO: Check if the player already has signed

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_PETITION_SIGN [GUID={2:X} Unk={3}]", Client.IP, Client.Port, ItemGUID, Unk)

        Dim MySQLQuery As New DataTable
        CharacterDatabase.Query("SELECT petition_signedMembers, petition_owner FROM petitions WHERE petition_itemGuid = " & ItemGUID - GUID_ITEM & ";", MySQLQuery)
        If MySQLQuery.Rows.Count = 0 Then Exit Sub

        CharacterDatabase.Update("UPDATE petitions SET petition_signedMembers = petition_signedMembers + 1, petition_signedMember" & (MySQLQuery.Rows(0).Item("petition_signedMembers") + 1) & " = " & Client.Character.GUID & " WHERE petition_itemGuid = " & ItemGUID - GUID_ITEM & ";")

        'DONE: Send result to both players
        Dim response As New PacketClass(OPCODES.SMSG_PETITION_SIGN_RESULTS)
        response.AddUInt64(ItemGUID)
        response.AddUInt64(Client.Character.GUID)
        response.AddInt32(PetitionSignError.PETITIONSIGN_OK)
        Client.SendMultiplyPackets(response)
        If CHARACTERs.ContainsKey(CType(MySQLQuery.Rows(0).Item("petition_owner"), ULong)) Then CHARACTERs(CType(MySQLQuery.Rows(0).Item("petition_owner"), ULong)).Client.SendMultiplyPackets(response)
        response.Dispose()
    End Sub
    Public Sub On_MSG_PETITION_DECLINE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim ItemGUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_PETITION_DECLINE [GUID={2:X}]", Client.IP, Client.Port, ItemGUID)

        'DONE: Get petition owner
        Dim q As New DataTable
        CharacterDatabase.Query("SELECT petition_owner FROM petitions WHERE petition_itemGuid = " & ItemGUID - GUID_ITEM & " LIMIT 1;", q)

        'DONE: Send message to player
        Dim response As New PacketClass(OPCODES.MSG_PETITION_DECLINE)
        response.AddUInt64(Client.Character.GUID)
        If q.Rows.Count > 0 AndAlso CHARACTERs.ContainsKey(CType(q.Rows(0).Item("petition_owner"), ULong)) Then CHARACTERs(CType(q.Rows(0).Item("petition_owner"), ULong)).Client.SendMultiplyPackets(response)
        response.Dispose()
    End Sub

    Public Sub On_CMSG_TURN_IN_PETITION(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim ItemGUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TURN_IN_PETITION [GUID={2:X}]", Client.IP, Client.Port, ItemGUID)

        Client.Character.ItemREMOVE(ItemGUID, True, True)
    End Sub

#End Region
#Region "WS.Guilds.Handlers"

    'Basic Tabard Framework
    Public Sub SendTabardActivate(ByRef c As CharacterObject, ByVal cGUID As ULong)
        Dim packet As New PacketClass(OPCODES.MSG_TABARDVENDOR_ACTIVATE)
        packet.AddUInt64(cGUID)
        c.Client.Send(packet)
        packet.Dispose()
    End Sub
    Public Sub On_MSG_TABARDVENDOR_ACTIVATE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_TABARDVENDOR_ACTIVATE [GUID={2}]", Client.IP, Client.Port, GUID)

        SendTabardActivate(Client.Character, GUID)
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

    'Helping Subs
    Public Enum GuildCommand As Byte
        GUILD_CREATE_S = &H0
        GUILD_INVITE_S = &H1
        GUILD_QUIT_S = &H2
        GUILD_FOUNDER_S = &HC
    End Enum
    Public Enum GuildError As Byte
        GUILD_PLAYER_NO_MORE_IN_GUILD = &H0
        GUILD_INTERNAL = &H1
        GUILD_ALREADY_IN_GUILD = &H2
        ALREADY_IN_GUILD = &H3
        INVITED_TO_GUILD = &H4
        ALREADY_INVITED_TO_GUILD = &H5
        GUILD_NAME_INVALID = &H6
        GUILD_NAME_EXISTS = &H7
        GUILD_LEADER_LEAVE = &H8
        GUILD_PERMISSIONS = &H8
        GUILD_PLAYER_NOT_IN_GUILD = &H9
        GUILD_PLAYER_NOT_IN_GUILD_S = &HA
        GUILD_PLAYER_NOT_FOUND = &HB
        GUILD_NOT_ALLIED = &HC
    End Enum

    Public Sub SendGuildResult(ByRef Client As ClientClass, ByVal Command As GuildCommand, ByVal Result As GuildError, Optional ByVal Text As String = "")
        Dim response As New PacketClass(OPCODES.SMSG_GUILD_COMMAND_RESULT)
        response.AddInt32(Command)
        response.AddString(Text)
        response.AddInt32(Result)
        Client.Send(response)
        response.Dispose()
    End Sub

#End Region

End Module
