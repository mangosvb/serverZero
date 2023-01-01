'
' Copyright (C) 2013-2023 getMaNGOS <https://getmangos.eu>
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

Imports System.Threading
Imports Mangos.Common
Imports Mangos.Common.Enums.Authentication
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Item
Imports Mangos.Common.Enums.Player
Imports Mangos.Common.Enums.Spell
Imports Mangos.Common.Enums.Unit
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Handlers

#Region "WS.CharMangment.Handlers"
    Public Class CharManagementHandler

        Public Sub On_CMSG_SET_ACTION_BUTTON(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 10 Then Exit Sub
            packet.GetInt16()
            Dim button As Byte = packet.GetInt8 '(6)
            Dim action As UShort = packet.GetUInt16 '(7)
            Dim actionMisc As Byte = packet.GetInt8 '(9)
            Dim actionType As Byte = packet.GetInt8 '(10)

            If action = 0 Then
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] MSG_SET_ACTION_BUTTON [Remove action from button {2}]", client.IP, client.Port, button)
                client.Character.ActionButtons.Remove(button)
            ElseIf actionType = 64 Then
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_ACTION_BUTTON [Added Macro {2} into button {3}]", client.IP, client.Port, action, button)
            ElseIf actionType = 128 Then
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_ACTION_BUTTON [Added Item {2} into button {3}]", client.IP, client.Port, action, button)
            Else
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_SET_ACTION_BUTTON [Added Action {2}:{4}:{5} into button {3}]", client.IP, client.Port, action, button, actionType, actionMisc)
            End If
            client.Character.ActionButtons(button) = New WS_PlayerHelper.TActionButton(action, actionType, actionMisc)
        End Sub

        Public Sub On_CMSG_LOGOUT_REQUEST(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LOGOUT_REQUEST", client.IP, client.Port)
            client.Character.Save()

            'TODO: Lose Invisibility

            'DONE: Can't log out in combat
            If client.Character.IsInCombat Then
                Dim LOGOUT_RESPONSE_DENIED As New Packets.PacketClass(OPCODES.SMSG_LOGOUT_RESPONSE)
                Try
                    LOGOUT_RESPONSE_DENIED.AddInt32(0)
                    LOGOUT_RESPONSE_DENIED.AddInt8(LogoutResponseCode.LOGOUT_RESPONSE_DENIED)
                    client.Send(LOGOUT_RESPONSE_DENIED)
                Finally
                    LOGOUT_RESPONSE_DENIED.Dispose()
                End Try
                Exit Sub
            End If

            If Not client.Character.positionZ > (_WS_Maps.GetZCoord(client.Character.positionX, client.Character.positionY, client.Character.positionZ, client.Character.MapID) + 10) Then
                'DONE: Initialize packet
                Dim UpdateData As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                Dim SMSG_UPDATE_OBJECT As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                Try
                    SMSG_UPDATE_OBJECT.AddInt32(1)      'Operations.Count
                    SMSG_UPDATE_OBJECT.AddInt8(0)

                    'DONE: Disable Turn
                    client.Character.cUnitFlags = client.Character.cUnitFlags Or UnitFlags.UNIT_FLAG_STUNTED
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, client.Character.cUnitFlags)

                    'DONE: StandState -> Sit
                    client.Character.StandState = StandStates.STANDSTATE_SIT
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, client.Character.cBytes1)

                    'DONE: Send packet
                    UpdateData.AddToPacket(SMSG_UPDATE_OBJECT, ObjectUpdateType.UPDATETYPE_VALUES, (client.Character))
                    client.Character.SendToNearPlayers(SMSG_UPDATE_OBJECT)
                Finally
                    SMSG_UPDATE_OBJECT.Dispose()
                End Try

                Dim packetACK As New Packets.PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                Try
                    packetACK.AddInt8(StandStates.STANDSTATE_SIT)
                    client.Send(packetACK)
                Finally
                    packetACK.Dispose()
                End Try
            End If

            'DONE: Let the client to exit
            Dim SMSG_LOGOUT_RESPONSE As New Packets.PacketClass(OPCODES.SMSG_LOGOUT_RESPONSE)
            Try
                SMSG_LOGOUT_RESPONSE.AddInt32(0)
                SMSG_LOGOUT_RESPONSE.AddInt8(LogoutResponseCode.LOGOUT_RESPONSE_ACCEPTED)     'Logout Accepted
                client.Send(SMSG_LOGOUT_RESPONSE)
            Finally
                SMSG_LOGOUT_RESPONSE.Dispose()
            End Try
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGOUT_RESPONSE", client.IP, client.Port)

            'DONE: While logout, the player can't move
            client.Character.SetMoveRoot()

            'DONE: If the player is resting, then it's instant logout
            client.Character.ZoneCheck()
            If client.Character.isResting Then
                client.Character.Logout()
            Else
                client.Character.LogoutTimer = New Timer(AddressOf client.Character.Logout, Nothing, 20000, Timeout.Infinite)
            End If
        End Sub

        Public Sub On_CMSG_LOGOUT_CANCEL(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            Try
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_LOGOUT_CANCEL", client.IP, client.Port)
                If client Is Nothing Then Exit Sub
                If client.Character Is Nothing Then Exit Sub
                If client.Character.LogoutTimer Is Nothing Then Exit Sub
                Try
                    client.Character.LogoutTimer.Dispose()
                    client.Character.LogoutTimer = Nothing
                Catch
                End Try

                'DONE: Initialize packet
                Dim UpdateData As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_PLAYER)
                Dim SMSG_UPDATE_OBJECT As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                Try
                    SMSG_UPDATE_OBJECT.AddInt32(1)      'Operations.Count
                    SMSG_UPDATE_OBJECT.AddInt8(0)

                    'DONE: Enable turn
                    client.Character.cUnitFlags = client.Character.cUnitFlags And (Not UnitFlags.UNIT_FLAG_STUNTED)
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, client.Character.cUnitFlags)

                    'DONE: StandState -> Stand
                    client.Character.StandState = StandStates.STANDSTATE_STAND
                    UpdateData.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, client.Character.cBytes1)

                    'DONE: Send packet
                    UpdateData.AddToPacket(SMSG_UPDATE_OBJECT, ObjectUpdateType.UPDATETYPE_VALUES, (client.Character))
                    client.Send(SMSG_UPDATE_OBJECT)
                Finally
                    SMSG_UPDATE_OBJECT.Dispose()
                End Try

                Dim packetACK As New Packets.PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
                Try
                    packetACK.AddInt8(StandStates.STANDSTATE_STAND)
                    client.Send(packetACK)
                Finally
                    packetACK.Dispose()
                End Try

                'DONE: Stop client logout
                Dim SMSG_LOGOUT_CANCEL_ACK As New Packets.PacketClass(OPCODES.SMSG_LOGOUT_CANCEL_ACK)
                Try
                    client.Send(SMSG_LOGOUT_CANCEL_ACK)
                Finally
                    SMSG_LOGOUT_CANCEL_ACK.Dispose()
                End Try
                _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_LOGOUT_CANCEL_ACK", client.IP, client.Port)

                'DONE: Enable moving
                client.Character.SetMoveUnroot()
            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.CRITICAL, "Error while trying to cancel logout.{0}", Environment.NewLine & e.ToString)
            End Try
        End Sub

        Public Sub On_CMSG_STANDSTATECHANGE(ByRef packet As Packets.PacketClass, ByRef client As WS_Network.ClientClass)
            If (packet.Data.Length - 1) < 6 Then Exit Sub
            packet.GetInt16()

            Dim StandState As Byte = packet.GetInt8

            If StandState = StandStates.STANDSTATE_STAND Then
                client.Character.RemoveAurasByInterruptFlag(SpellAuraInterruptFlags.AURA_INTERRUPT_FLAG_NOT_SEATED)
            End If

            client.Character.StandState = StandState
            client.Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_BYTES_1, client.Character.cBytes1)
            client.Character.SendCharacterUpdate()

            Dim packetACK As New Packets.PacketClass(OPCODES.SMSG_STANDSTATE_CHANGE_ACK)
            Try
                packetACK.AddInt8(StandState)
                client.Send(packetACK)
            Finally
                packetACK.Dispose()
            End Try
            _WorldServer.Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_STANDSTATECHANGE [{2}]", client.IP, client.Port, client.Character.StandState)
        End Sub

        Public Function CanUseAmmo(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal AmmoID As Integer) As InventoryChangeFailure
            If objCharacter.DEAD Then Return InventoryChangeFailure.EQUIP_ERR_YOU_ARE_DEAD
            If _WorldServer.ITEMDatabase.ContainsKey(AmmoID) = False Then Return InventoryChangeFailure.EQUIP_ERR_ITEM_NOT_FOUND
            If _WorldServer.ITEMDatabase(AmmoID).InventoryType <> INVENTORY_TYPES.INVTYPE_AMMO Then Return InventoryChangeFailure.EQUIP_ERR_ONLY_AMMO_CAN_GO_HERE
            If _WorldServer.ITEMDatabase(AmmoID).AvailableClasses <> 0 AndAlso (_WorldServer.ITEMDatabase(AmmoID).AvailableClasses And objCharacter.ClassMask) = 0 Then Return InventoryChangeFailure.EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM
            If _WorldServer.ITEMDatabase(AmmoID).AvailableRaces <> 0 AndAlso (_WorldServer.ITEMDatabase(AmmoID).AvailableRaces And objCharacter.RaceMask) = 0 Then Return InventoryChangeFailure.EQUIP_ERR_YOU_CAN_NEVER_USE_THAT_ITEM

            If _WorldServer.ITEMDatabase(AmmoID).ReqSkill <> 0 Then
                If objCharacter.HaveSkill(_WorldServer.ITEMDatabase(AmmoID).ReqSkill) = False Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
                If objCharacter.HaveSkill(_WorldServer.ITEMDatabase(AmmoID).ReqSkill, _WorldServer.ITEMDatabase(AmmoID).ReqSkillRank) = False Then Return InventoryChangeFailure.EQUIP_ERR_SKILL_ISNT_HIGH_ENOUGH
            End If
            If _WorldServer.ITEMDatabase(AmmoID).ReqSpell <> 0 Then
                If objCharacter.HaveSpell(_WorldServer.ITEMDatabase(AmmoID).ReqSpell) = False Then Return InventoryChangeFailure.EQUIP_ERR_NO_REQUIRED_PROFICIENCY
            End If
            If _WorldServer.ITEMDatabase(AmmoID).ReqLevel > objCharacter.Level Then Return InventoryChangeFailure.EQUIP_ERR_YOU_MUST_REACH_LEVEL_N
            If objCharacter.HavePassiveAura(46699) Then Return InventoryChangeFailure.EQUIP_ERR_BAG_FULL6 'Required no ammoe

            Return InventoryChangeFailure.EQUIP_ERR_OK
        End Function

        Public Function CheckAmmoCompatibility(ByRef objCharacter As WS_PlayerData.CharacterObject, ByVal AmmoID As Integer) As Boolean
            If _WorldServer.ITEMDatabase.ContainsKey(AmmoID) = False Then Return False
            If objCharacter.Items.ContainsKey(EquipmentSlots.EQUIPMENT_SLOT_RANGED) = False OrElse objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).IsBroken Then Return False
            If objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.ObjectClass <> ITEM_CLASS.ITEM_CLASS_WEAPON Then Return False

            Select Case objCharacter.Items(EquipmentSlots.EQUIPMENT_SLOT_RANGED).ItemInfo.SubClass
                Case ITEM_SUBCLASS.ITEM_SUBCLASS_BOW, ITEM_SUBCLASS.ITEM_SUBCLASS_CROSSBOW
                    If _WorldServer.ITEMDatabase(AmmoID).SubClass <> ITEM_SUBCLASS.ITEM_SUBCLASS_ARROW Then Return False
                Case ITEM_SUBCLASS.ITEM_SUBCLASS_GUN
                    If _WorldServer.ITEMDatabase(AmmoID).SubClass <> ITEM_SUBCLASS.ITEM_SUBCLASS_BULLET Then Return False
                Case Else
                    Return False
            End Select

            Return True
        End Function

    End Class
#End Region
End Namespace