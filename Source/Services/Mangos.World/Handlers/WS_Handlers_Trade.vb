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

Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Item
Imports Mangos.Common.Enums.Misc
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Objects
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Handlers

    Public Module WS_Handlers_Trade

        Public Class TTradeInfo
            Implements IDisposable

            Public ID As Integer = 0
            Public Trader As WS_PlayerData.CharacterObject = Nothing
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
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    Trader.tradeInfo = Nothing
                    Target.tradeInfo = Nothing
                    Trader = Nothing
                    Target = Nothing
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

            Public Sub SendTradeUpdateToTrader()
                If Trader Is Nothing Then Exit Sub

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_TRADE_STATUS_EXTENDED)
                Try
                    packet.AddInt8(1)               'giving(0x00) or receiving(0x01)
                    packet.AddInt32(7)              'Slots for Character 1
                    packet.AddInt32(7)              'Slots for Character 2
                    packet.AddUInt32(TargetGold)     'Gold
                    packet.AddInt32(0)

                    For i As Integer = 0 To 6
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

                    Trader.client.Send(packet)
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

                    For i As Integer = 0 To 6
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

                    Target.client.Send(packet)
                Finally
                    packet.Dispose()
                End Try
            End Sub
            Public Sub DoTrade(ByRef Who As CharacterObject)

                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_STATUS_COMPLETE)
                    If Trader Is Who Then
                        Target.client.SendMultiplyPackets(response)
                        TraderAccept = True
                    Else
                        Trader.client.SendMultiplyPackets(response)
                        TargetAccept = True
                    End If
                Finally
                    response.Dispose()
                End Try

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
                        Try
                            responseUnAccept.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
                            Target.client.SendMultiplyPackets(responseUnAccept)
                            TraderAccept = False
                            Trader.client.SendMultiplyPackets(responseUnAccept)
                            TraderAccept = False
                        Finally
                            responseUnAccept.Dispose()
                        End Try

                        Dim responseNoSlot As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                        Try
                            responseNoSlot.AddInt8(InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL)
                            responseNoSlot.AddUInt64(0)
                            responseNoSlot.AddUInt64(0)
                            responseNoSlot.AddInt8(0)
                            Target.client.Send(responseNoSlot)
                        Finally
                            responseNoSlot.Dispose()
                        End Try
                        Exit Sub
                    End If

                    If Trader.ItemFREESLOTS < TraderReqItems Then
                        Dim responseUnAccept As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                        Try
                            responseUnAccept.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
                            Target.client.SendMultiplyPackets(responseUnAccept)
                            TraderAccept = False
                            Trader.client.SendMultiplyPackets(responseUnAccept)
                            TargetAccept = False
                        Finally
                            responseUnAccept.Dispose()
                        End Try

                        Dim responseNoSlot As New PacketClass(OPCODES.SMSG_INVENTORY_CHANGE_FAILURE)
                        Try
                            responseNoSlot.AddInt8(InventoryChangeFailure.EQUIP_ERR_INVENTORY_FULL)
                            responseNoSlot.AddUInt64(0)
                            responseNoSlot.AddUInt64(0)
                            responseNoSlot.AddInt8(0)
                            Trader.client.Send(responseNoSlot)
                        Finally
                            responseNoSlot.Dispose()
                        End Try
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
                    Try
                        response.AddInt32(TradeStatus.TRADE_COMPLETE)
                        Target.client.SendMultiplyPackets(response)
                        Trader.client.SendMultiplyPackets(response)
                    Finally
                        response.Dispose()
                        Dispose()
                    End Try

                Catch e As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "Error doing trade: {0}{1}", Environment.NewLine, e.ToString)
                End Try
            End Sub
        End Class


        Public Sub On_CMSG_CANCEL_TRADE(ByRef packet As PacketClass, ByRef client As WS_Network.ClientClass)
            If client Is Nothing Then Exit Sub
            If client.Character Is Nothing Then Exit Sub

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CANCEL_TRADE", client.IP, client.Port)

            If client.Character.tradeInfo IsNot Nothing Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_STATUS_CANCELED)
                    If client.Character.tradeInfo.Target IsNot Nothing Then client.Character.tradeInfo.Target.client.SendMultiplyPackets(response)
                    If client.Character.tradeInfo.Trader IsNot Nothing Then client.Character.tradeInfo.Trader.client.SendMultiplyPackets(response)
                Finally
                    response.Dispose()
                End Try

                client.Character.tradeInfo.Dispose()
            End If
        End Sub
        Public Sub On_CMSG_SET_TRADE_GOLD(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim gold As UInteger = packet.GetUInt32()

            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_TRADE_GOLD [gold={2}]", client.IP, client.Port, gold)

            If client.Character.tradeInfo Is Nothing Then Exit Sub
            If client.Character.tradeInfo.Trader Is client.Character Then
                client.Character.tradeInfo.TraderGold = gold
                client.Character.tradeInfo.SendTradeUpdateToTarget()
            Else
                client.Character.tradeInfo.TargetGold = gold
                client.Character.tradeInfo.SendTradeUpdateToTrader()
            End If
        End Sub
        Public Sub On_CMSG_SET_TRADE_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim slot As Byte = packet.GetInt8
            Dim myBag As Byte = packet.GetInt8
            Dim mySlot As Byte = packet.GetInt8
            If myBag = 255 Then myBag = 0
            If slot > 6 Then Exit Sub
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_TRADE_ITEM [slot={2} myBag={3} mySlot={4}]", client.IP, client.Port, slot, myBag, mySlot)

            If client.Character.tradeInfo Is Nothing Then Exit Sub
            If client.Character.tradeInfo.Trader Is client.Character Then
                client.Character.tradeInfo.TraderSlots(slot) = (CType(myBag, Integer) << 8) + mySlot
                client.Character.tradeInfo.SendTradeUpdateToTarget()
            Else
                client.Character.tradeInfo.TargetSlots(slot) = (CType(myBag, Integer) << 8) + mySlot
                client.Character.tradeInfo.SendTradeUpdateToTrader()
            End If
        End Sub
        Public Sub On_CMSG_CLEAR_TRADE_ITEM(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim slot As Byte = packet.GetInt8
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_CLEAR_TRADE_ITEM [slot={2}]", client.IP, client.Port, slot)

            If client.Character.tradeInfo.Trader Is client.Character Then
                client.Character.tradeInfo.TraderSlots(slot) = -1
                client.Character.tradeInfo.SendTradeUpdateToTarget()
            Else
                client.Character.tradeInfo.TargetSlots(slot) = -1
                client.Character.tradeInfo.SendTradeUpdateToTrader()
            End If

        End Sub
        Public Sub On_CMSG_INITIATE_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim targetGUID As ULong = packet.GetUInt64
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_INITIATE_TRADE [Trader={2} Target={3}]", client.IP, client.Port, client.Character.GUID, targetGUID)

            If client.Character.DEAD = True Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_DEAD)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            ElseIf client.Character.LogoutTimer IsNot Nothing Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_LOGOUT)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            ElseIf (client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED) Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_STUNNED)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If _WorldServer.CHARACTERs.ContainsKey(targetGUID) = False Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_MISSING)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            ElseIf _WorldServer.CHARACTERs(targetGUID).DEAD = True Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_DEAD)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            ElseIf _WorldServer.CHARACTERs(targetGUID).LogoutTimer IsNot Nothing Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_LOGOUT)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            ElseIf (_WorldServer.CHARACTERs(targetGUID).cUnitFlags And UnitFlags.UNIT_FLAG_STUNTED) Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_STUNNED)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If Not client.Character.tradeInfo Is Nothing Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_UNAVIABLE)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If Not _WorldServer.CHARACTERs(targetGUID).tradeInfo Is Nothing Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_UNAVIABLE2)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If _WorldServer.CHARACTERs(targetGUID).IsHorde <> client.Character.IsHorde Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_DIFF_FACTION)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If GetDistance(client.Character, _WorldServer.CHARACTERs(targetGUID)) > 30.0F Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TARGET_TOO_FAR)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            If client.Character.Access = AccessLevel.Trial Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TRIAL_ACCOUNT)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If
            If _WorldServer.CHARACTERs(targetGUID).Access = AccessLevel.Trial Then
                Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
                Try
                    response.AddInt32(TradeStatus.TRADE_TRIAL_ACCOUNT)
                    client.Send(response)
                Finally
                    response.Dispose()
                End Try
                Exit Sub
            End If

            'TODO: Another of these currently 'DO NOTHING' lines, needs to be implemented correctly
            Dim tmpTradeInfo As New TTradeInfo(client.Character, _WorldServer.CHARACTERs(targetGUID))

            Dim response_ok As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            Try
                response_ok.AddInt32(TradeStatus.TRADE_STATUS_OK)
                response_ok.AddUInt64(client.Character.GUID)
                client.Character.tradeInfo.Target.client.Send(response_ok)
            Finally
                response_ok.Dispose()
            End Try
        End Sub

        Public Sub On_CMSG_BEGIN_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BEGIN_TRADE", client.IP, client.Port)

            client.Character.tradeInfo.ID += 1

            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            Try
                response.AddInt32(TradeStatus.TRADE_TRADE_WINDOW_OPEN)
                response.AddInt32(client.Character.tradeInfo.ID)
                client.Character.tradeInfo.Trader.client.SendMultiplyPackets(response)
                client.Character.tradeInfo.Target.client.SendMultiplyPackets(response)
            Finally
                response.Dispose()
            End Try
        End Sub
        Public Sub On_CMSG_UNACCEPT_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_UNACCEPT_TRADE", client.IP, client.Port)

            Dim response As New PacketClass(OPCODES.SMSG_TRADE_STATUS)
            Try
                response.AddInt32(TradeStatus.TRADE_STATUS_UNACCEPT)
                If client.Character.tradeInfo.Trader Is client.Character Then
                    client.Character.tradeInfo.Target.client.SendMultiplyPackets(response)
                    client.Character.tradeInfo.TraderAccept = False
                Else
                    client.Character.tradeInfo.Trader.client.SendMultiplyPackets(response)
                    client.Character.tradeInfo.TargetAccept = False
                End If
            Finally
                response.Dispose()
            End Try
        End Sub
        Public Sub On_CMSG_ACCEPT_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACCEPT_TRADE", client.IP, client.Port)
            client.Character.tradeInfo.DoTrade(client.Character)
        End Sub

        Public Sub On_CMSG_IGNORE_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_IGNORE_TRADE", client.IP, client.Port)
        End Sub
        Public Sub On_CMSG_BUSY_TRADE(ByRef packet As PacketClass, ByRef client As ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_BUSY_TRADE", client.IP, client.Port)
        End Sub

    End Module
End NameSpace