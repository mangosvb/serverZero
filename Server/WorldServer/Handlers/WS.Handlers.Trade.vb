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

Imports System.Threading
Imports System.Net.Sockets
Imports System.Xml.Serialization
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common


Public Module WS_Handlers_Trade


    Public Class TTradeInfo
        Implements IDisposable

        Public ID As Integer = 0
        Public Trader As CharacterObject = Nothing
        Public Target As CharacterObject = Nothing

        Public TraderSlots() As Integer = {-1, -1, -1, -1, -1, -1, -1}
        Public TraderGold As UInteger = 0
        Public TraderAccept As Boolean = False

        Public TargetSlots() As Integer = {-1, -1, -1, -1, -1, -1, -1}
        Public TargetGold As UInteger = 0
        Public TargetAccept As Boolean = False

        Public Sub New(ByRef Trader_ As CharacterObject, ByRef Target_ As CharacterObject)
            Trader = Trader_
            Target = Target_
            Trader.tradeInfo = Me
            Target.tradeInfo = Me
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Trader.tradeInfo = Nothing
                Target.tradeInfo = Nothing
                Trader = Nothing
                Target = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Sub SendTradeUpdateToTrader()
            If Trader Is Nothing Then Exit Sub

            Dim packet As New PacketClass(OPCODES.SMSG_TRADE_STATUS_EXTENDED)
            Try
                packet.AddInt8(1)               'giving(0x00) or receiving(0x01) 
                packet.AddInt32(7)              'Slots for Character 1
                packet.AddInt32(7)              'Slots for Character 2
                packet.AddUInt32(TargetGold)     'Gold
                packet.AddInt32(0)

                Dim i As Integer
                For i = 0 To 6
                    packet.AddInt8(i)
                    If TargetSlots(i) > 0 Then
                        Dim mySlot As Byte = TargetSlots(i) And &HFF
                        Dim myBag As Byte = TargetSlots(i) >> 8
                        Dim myItem As ItemObject = Nothing

                        If myBag = 0 Then myItem = Target.Items(mySlot) Else myItem = Target.Items(myBag).Items(mySlot)

                        packet.AddInt32(myItem.ItemEntry)
                        packet.AddInt32(myItem.ItemInfo.Model)
                        packet.AddInt32(myItem.StackCount)              'ITEM_FIELD_STACK_COUNT
                        packet.AddInt32(0)                              'Unk.. probably gift=1, created_by=0?
                        packet.AddUInt64(myItem.GiftCreatorGUID)        'ITEM_FIELD_GIFTCREATOR
                        If myItem.Enchantments.ContainsKey(EnchantSlots.ENCHANTMENT_PERM) Then
                            packet.AddInt32(myItem.Enchantments(EnchantSlots.ENCHANTMENT_PERM).ID)
                        Else
                            packet.AddInt32(0)                          'ITEM_FIELD_ENCHANTMENT
                        End If
                        packet.AddUInt64(myItem.CreatorGUID)            'ITEM_FIELD_CREATOR
                        packet.AddInt32(myItem.ChargesLeft)             'ITEM_FIELD_SPELL_CHARGES
                        packet.AddInt32(0)                              'ITEM_FIELD_PROPERTY_SEED
                        packet.AddInt32(myItem.RandomProperties)        'ITEM_FIELD_RANDOM_PROPERTIES_ID 
                        packet.AddInt32(myItem.ItemInfo.Flags)          'ITEM_FIELD_FLAGS 
                        packet.AddInt32(myItem.ItemInfo.Durability)     'ITEM_FIELD_MAXDURABILITY 
                        packet.AddInt32(myItem.Durability)              'ITEM_FIELD_DURABILITY 
                    Else
                        Dim j As Integer
                        For j = 0 To 14
                            packet.AddInt32(0)
                        Next j
                    End If
                Next i


                Trader.Client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub
        Public Sub SendTradeUpdateToTarget()
            If Target Is Nothing Then Exit Sub

            Dim packet As New PacketClass(OPCODES.SMSG_TRADE_STATUS_EXTENDED)
            Try
                packet.AddInt8(1)               'giving(0x00) or receiving(0x01) 
                packet.AddInt32(7)              'Slots for Character 1
                packet.AddInt32(7)              'Slots for Character 2
                packet.AddUInt32(TraderGold)     'Gold
                packet.AddInt32(0)

                Dim i As Integer
                For i = 0 To 6
                    packet.AddInt8(i)
                    If TraderSlots(i) > 0 Then
                        Dim mySlot As Byte = TraderSlots(i) And &HFF
                        Dim myBag As Byte = TraderSlots(i) >> 8
                        Dim myItem As ItemObject = Nothing

                        If myBag = 0 Then myItem = Trader.Items(mySlot) Else myItem = Trader.Items(myBag).Items(mySlot)

                        packet.AddInt32(myItem.ItemEntry)
                        packet.AddInt32(myItem.ItemInfo.Model)
                        packet.AddInt32(myItem.StackCount)              'ITEM_FIELD_STACK_COUNT
                        packet.AddInt32(0)                              'Unk.. probably gift=1, created_by=0?
                        packet.AddUInt64(myItem.GiftCreatorGUID)        'ITEM_FIELD_GIFTCREATOR
                        If myItem.Enchantments.ContainsKey(EnchantSlots.ENCHANTMENT_PERM) Then
                            packet.AddInt32(myItem.Enchantments(EnchantSlots.ENCHANTMENT_PERM).ID)
                        Else
                            packet.AddInt32(0)                          'ITEM_FIELD_ENCHANTMENT
                        End If
                        packet.AddUInt64(myItem.CreatorGUID)            'ITEM_FIELD_CREATOR
                        packet.AddInt32(myItem.ChargesLeft)             'ITEM_FIELD_SPELL_CHARGES
                        packet.AddInt32(0)                              'ITEM_FIELD_PROPERTY_SEED
                        packet.AddInt32(myItem.RandomProperties)        'ITEM_FIELD_RANDOM_PROPERTIES_ID 
                        packet.AddInt32(myItem.ItemInfo.Flags)          'ITEM_FIELD_FLAGS 
                        packet.AddInt32(myItem.ItemInfo.Durability)     'ITEM_FIELD_MAXDURABILITY 
                        packet.AddInt32(myItem.Durability)              'ITEM_FIELD_DURABILITY 
                    Else
                        Dim j As Integer
                        For j = 0 To 14
                            packet.AddInt32(0)
                        Next j
                    End If
                Next i


                Target.Client.Send(packet)
            Finally
                packet.Dispose()
            End Try
        End Sub
        Public Sub DoTrade(ByRef Who As CharacterObject)

            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_STATUS_COMPLETE)
            If Trader Is Who Then
                Target.Client.SendMultiplyPackets(response)
                TraderAccept = True
            Else
                Trader.Client.SendMultiplyPackets(response)
                TargetAccept = True
            End If
            response.Dispose()

            If TargetAccept AndAlso TraderAccept Then DoTrade()
        End Sub

        Private Sub DoTrade()
            Dim TargetReqItems As Byte = 0
            Dim TraderReqItems As Byte = 0

            For i As Byte = 0 To 5
                If TraderSlots(i) > 0 Then TargetReqItems += 1
                If TargetSlots(i) > 0 Then TraderReqItems += 1
            Next

            Try
                'DONE: Check free slots
                If Target.ItemFREESLOTS < TargetReqItems Then
                    Dim responseUnAccept As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                    responseUnAccept.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
                    Target.Client.SendMultiplyPackets(responseUnAccept)
                    TraderAccept = False
                    Trader.Client.SendMultiplyPackets(responseUnAccept)
                    TraderAccept = False
                    responseUnAccept.Dispose()

                    Dim responseNoSlot As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    responseNoSlot.AddInt8(InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL)
                    responseNoSlot.AddUInt64(0)
                    responseNoSlot.AddUInt64(0)
                    responseNoSlot.AddInt8(0)
                    Target.Client.Send(responseNoSlot)
                    responseNoSlot.Dispose()
                    Exit Sub
                End If

                If Trader.ItemFREESLOTS < TraderReqItems Then
                    Dim responseUnAccept As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                    responseUnAccept.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
                    Target.Client.SendMultiplyPackets(responseUnAccept)
                    TraderAccept = False
                    Trader.Client.SendMultiplyPackets(responseUnAccept)
                    TargetAccept = False
                    responseUnAccept.Dispose()

                    Dim responseNoSlot As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                    responseNoSlot.AddInt8(InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL)
                    responseNoSlot.AddUInt64(0)
                    responseNoSlot.AddUInt64(0)
                    responseNoSlot.AddInt8(0)
                    Trader.Client.Send(responseNoSlot)
                    responseNoSlot.Dispose()
                    Exit Sub
                End If


                'DONE: Trade gold
                If TargetGold > 0 Or TraderGold > 0 Then
                    Trader.Copper = Trader.Copper - TraderGold + TargetGold
                    Target.Copper = Target.Copper + TraderGold - TargetGold
                    Trader.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Trader.Copper)
                    Target.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Target.Copper)
                End If

                'TODO: For item set ITEM_FIELD_GIFTCREATOR
                'DONE: Item trading
                If TargetReqItems > 0 Or TraderReqItems > 0 Then
                    For i As Byte = 0 To 5
                        If TraderSlots(i) > 0 Then
                            Dim mySlot As Byte = TraderSlots(i) And &HFF
                            Dim myBag As Byte = TraderSlots(i) >> 8
                            Dim myItem As ItemObject = Nothing
                            If myBag = 0 Then myItem = Trader.Items(mySlot) Else myItem = Trader.Items(myBag).Items(mySlot)

                            'DONE: Disable trading of quest items
                            If myItem.ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_QUEST Then
                                'DONE: Swap items
                                myItem.OwnerGUID = Target.GUID
                                If Target.ItemADD(myItem) Then Trader.ItemREMOVE(myBag, mySlot, False, False)
                            End If
                        End If
                        If TargetSlots(i) > 0 Then
                            Dim mySlot As Byte = TargetSlots(i) And &HFF
                            Dim myBag As Byte = TargetSlots(i) >> 8
                            Dim myItem As ItemObject = Nothing
                            If myBag = 0 Then myItem = Target.Items(mySlot) Else myItem = Target.Items(myBag).Items(mySlot)

                            'DONE: Disable trading of quest items
                            If myItem.ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_QUEST Then
                                'DONE: Swap items
                                myItem.OwnerGUID = Trader.GUID
                                If Trader.ItemADD(myItem) Then Target.ItemREMOVE(myBag, mySlot, False, False)
                            End If
                        End If
                    Next
                End If


                Trader.SendCharacterUpdate(True)
                Target.SendCharacterUpdate(True)

                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                response.AddInt32(TradeStatus.TRADE_COMPLETE)
                Target.Client.SendMultiplyPackets(response)
                Trader.Client.SendMultiplyPackets(response)
                response.Dispose()
                Me.Dispose()

            Catch e As Exception
                Log.WriteLine(LogType.FAILED, "Error doing trade: {0}{1}", vbNewLine, e.ToString)
            End Try
        End Sub
    End Class
    Private Enum TradeStatus
        TRADE_TARGET_UNAVIABLE = 0              '"[NAME] is busy"
        TRADE_STATUS_OK = 1                     'BEGIN TRADE
        TRADE_TRADE_WINDOW_OPEN = 2             'OPEN TRADE WINDOW
        TRADE_STATUS_CANCELED = 3               '"Trade canceled"
        TRADE_STATUS_COMPLETE = 4               'TRADE COMPLETE
        TRADE_TARGET_UNAVIABLE2 = 5             '"[NAME] is busy"
        TRADE_TARGET_MISSING = 6                'SOUND: I dont have a target
        TRADE_STATUS_UNACCEPT = 7               'BACK TRADE
        TRADE_COMPLETE = 8                      '"Trade Complete"
        TRADE_UNK2 = 9
        TRADE_TARGET_TOO_FAR = 10               '"Trade target is too far away"
        TRADE_TARGET_DIFF_FACTION = 11          '"Trade is not party of your alliance"
        TRADE_TRADE_WINDOW_CLOSE = 12           'CLOSE TRADE WINDOW
        TRADE_UNK3 = 13
        TRADE_TARGET_IGNORING = 14              '"[NAME] is ignoring you"
        TRADE_STUNNED = 15                      '"You are stunned"
        TRADE_TARGET_STUNNED = 16               '"Target is stunned"
        TRADE_DEAD = 17                         '"You cannot do that when you are dead"
        TRADE_TARGET_DEAD = 18                  '"You cannot trade with dead players"
        TRADE_LOGOUT = 19                       '"You are loging out"
        TRADE_TARGET_LOGOUT = 20                '"The player is loging out"
        TRADE_TRIAL_ACCOUNT = 21                '"Trial accounts cannot perform that action"
        TRADE_STATUS_ONLY_CONJURED = 22         '"You can only trade conjured items... (cross realm BG related)."
    End Enum

    Public Sub On_CMSG_CANCEL_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If Client Is Nothing Then Exit Sub
        If Client.Character Is Nothing Then Exit Sub

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_TRADE", Client.IP, Client.Port)

        If Client.Character.tradeInfo IsNot Nothing Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_STATUS_CANCELED)
            If Client.Character.tradeInfo.Target IsNot Nothing Then Client.Character.tradeInfo.Target.Client.SendMultiplyPackets(response)
            If Client.Character.tradeInfo.Trader IsNot Nothing Then Client.Character.tradeInfo.Trader.Client.SendMultiplyPackets(response)
            response.Dispose()

            Client.Character.tradeInfo.Dispose()
        End If
    End Sub
    Public Sub On_CMSG_SET_TRADE_GOLD(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim gold As UInteger = packet.GetUInt32()

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_TRADE_GOLD [gold={2}]", Client.IP, Client.Port, gold)

        If Client.Character.tradeInfo Is Nothing Then Exit Sub
        If Client.Character.tradeInfo.Trader Is Client.Character Then
            Client.Character.tradeInfo.TraderGold = gold
            Client.Character.tradeInfo.SendTradeUpdateToTarget()
        Else
            Client.Character.tradeInfo.TargetGold = gold
            Client.Character.tradeInfo.SendTradeUpdateToTrader()
        End If
    End Sub
    Public Sub On_CMSG_SET_TRADE_ITEM(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim slot As Byte = packet.GetInt8
        Dim myBag As Byte = packet.GetInt8
        Dim mySlot As Byte = packet.GetInt8
        If myBag = 255 Then myBag = 0
        If slot > 6 Then Exit Sub
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_TRADE_ITEM [slot={2} myBag={3} mySlot={4}]", Client.IP, Client.Port, slot, myBag, mySlot)

        If Client.Character.tradeInfo Is Nothing Then Exit Sub
        If Client.Character.tradeInfo.Trader Is Client.Character Then
            Client.Character.tradeInfo.TraderSlots(slot) = (CType(myBag, Integer) << 8) + mySlot
            Client.Character.tradeInfo.SendTradeUpdateToTarget()
        Else
            Client.Character.tradeInfo.TargetSlots(slot) = (CType(myBag, Integer) << 8) + mySlot
            Client.Character.tradeInfo.SendTradeUpdateToTrader()
        End If
    End Sub
    Public Sub On_CMSG_CLEAR_TRADE_ITEM(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim slot As Byte = packet.GetInt8
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CLEAR_TRADE_ITEM [slot={2}]", Client.IP, Client.Port, slot)

        If Client.Character.tradeInfo.Trader Is Client.Character Then
            Client.Character.tradeInfo.TraderSlots(slot) = -1
            Client.Character.tradeInfo.SendTradeUpdateToTarget()
        Else
            Client.Character.tradeInfo.TargetSlots(slot) = -1
            Client.Character.tradeInfo.SendTradeUpdateToTrader()
        End If

    End Sub
    Public Sub On_CMSG_INITIATE_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim targetGUID As ULong = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_INITIATE_TRADE [Trader={2} Target={3}]", Client.IP, Client.Port, Client.Character.GUID, targetGUID)

        If Client.Character.DEAD = True Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_DEAD)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        ElseIf Client.Character.LogoutTimer IsNot Nothing Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_LOGOUT)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        ElseIf (Client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED) Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_STUNNED)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If CHARACTERs.ContainsKey(targetGUID) = False Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_MISSING)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        ElseIf CHARACTERs(targetGUID).DEAD = True Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_DEAD)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        ElseIf CHARACTERs(targetGUID).LogoutTimer IsNot Nothing Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_LOGOUT)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        ElseIf (CHARACTERs(targetGUID).cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED) Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_STUNNED)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If Not Client.Character.tradeInfo Is Nothing Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_UNAVIABLE)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If Not CType(CHARACTERs(targetGUID), CharacterObject).tradeInfo Is Nothing Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_UNAVIABLE2)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If CType(CHARACTERs(targetGUID), CharacterObject).Side <> Client.Character.Side Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_DIFF_FACTION)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If GetDistance(CType(Client.Character, CharacterObject), CHARACTERs(targetGUID)) > 30.0F Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TARGET_TOO_FAR)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        If Client.Character.Access = AccessLevel.Trial Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TRIAL_ACCOUNT)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If
        If CHARACTERs(targetGUID).Access = AccessLevel.Trial Then
            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            response.AddInt32(TradeStatus.TRADE_TRIAL_ACCOUNT)
            Client.Send(response)
            response.Dispose()
            Exit Sub
        End If

        Dim tmpTradeInfo As New TTradeInfo(Client.Character, CHARACTERs(targetGUID))
        Dim response_ok As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
        response_ok.AddInt32(TradeStatus.TRADE_STATUS_OK)
        response_ok.AddUInt64(Client.Character.GUID)
        Client.Character.tradeInfo.Target.Client.Send(response_ok)
        response_ok.Dispose()
    End Sub

    Public Sub On_CMSG_BEGIN_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BEGIN_TRADE", Client.IP, Client.Port)

        Client.Character.tradeInfo.ID += 1

        Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
        response.AddInt32(TradeStatus.TRADE_TRADE_WINDOW_OPEN)
        response.AddInt32(Client.Character.tradeInfo.ID)
        Client.Character.tradeInfo.Trader.Client.SendMultiplyPackets(response)
        Client.Character.tradeInfo.Target.Client.SendMultiplyPackets(response)
        response.Dispose()
    End Sub
    Public Sub On_CMSG_UNACCEPT_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_UNACCEPT_TRADE", Client.IP, Client.Port)

        Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
        response.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
        If Client.Character.tradeInfo.Trader Is Client.Character Then
            Client.Character.tradeInfo.Target.Client.SendMultiplyPackets(response)
            Client.Character.tradeInfo.TraderAccept = False
        Else
            Client.Character.tradeInfo.Trader.Client.SendMultiplyPackets(response)
            Client.Character.tradeInfo.TargetAccept = False
        End If
        response.Dispose()
    End Sub
    Public Sub On_CMSG_ACCEPT_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACCEPT_TRADE", Client.IP, Client.Port)
        Client.Character.tradeInfo.DoTrade(Client.Character)
    End Sub

    Public Sub On_CMSG_IGNORE_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_IGNORE_TRADE", Client.IP, Client.Port)
    End Sub
    Public Sub On_CMSG_BUSY_TRADE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUSY_TRADE", Client.IP, Client.Port)
    End Sub


End Module