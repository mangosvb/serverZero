'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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

Imports System.Runtime.CompilerServices
Imports mangosVB.Common.BaseWriter

Public Module WS_Corpses
    'WARNING: Use only with WORLD_GAMEOBJECTs()
    Public Class CorpseObject
        Inherits BaseObject
        Implements IDisposable

        Public DynFlags As Integer = 0
        Public Flags As Integer = 0
        Public Owner As ULong = 0
        Public Bytes1 As Integer = 0
        Public Bytes2 As Integer = 0
        Public Model As Integer = 0
        Public Guild As Integer = 0
        Public Items(EquipmentSlots.EQUIPMENT_SLOT_END - 1) As Integer

        Public Sub FillAllUpdateFlags(ByRef Update As UpdateClass)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, CType(ObjectType.TYPE_CORPSE + ObjectType.TYPE_OBJECT, Integer))
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_ENTRY, 0)
            Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 1.0F)

            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_OWNER, Owner)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_FACING, orientation)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_POS_X, positionX)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_POS_Y, positionY)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_POS_Z, positionZ)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_DISPLAY_ID, Model)

            For i As Integer = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_ITEM + i, Items(i))
            Next

            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_BYTES_1, Bytes1)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_BYTES_2, Bytes2)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_GUILD, Guild)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_FLAGS, Flags)
            Update.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_DYNAMIC_FLAGS, DynFlags)

        End Sub

        Public Sub ConvertToBones()
            'DONE: Delete from database
            CharacterDatabase.Update(String.Format("DELETE FROM corpse WHERE player = ""{0}"";", Owner))

            Flags = 5
            Owner = 0
            For i As Integer = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                Items(i) = 0
            Next

            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                packet.AddInt32(1)
                packet.AddInt8(0)
                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_CORPSE)
                Try
                    tmpUpdate.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_OWNER, 0)
                    tmpUpdate.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_FLAGS, 5)
                    For i As Integer = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                        tmpUpdate.SetUpdateFlag(ECorpseFields.CORPSE_FIELD_ITEM + i, 0)
                    Next
                    tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_VALUES, Me)

                    SendToNearPlayers(packet)
                Finally
                    tmpUpdate.Dispose()
                End Try
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub Save()
            'Only for creating New Character
            Dim tmpCmd As String = "INSERT INTO corpse (guid"
            Dim tmpValues As String = " VALUES (" & (GUID - GUID_CORPSE)

            tmpCmd = tmpCmd & ", player"
            tmpValues = tmpValues & ", " & Owner

            tmpCmd = tmpCmd & ", position_x"
            tmpValues = tmpValues & ", " & Trim(Str(positionX))
            tmpCmd = tmpCmd & ", position_y"
            tmpValues = tmpValues & ", " & Trim(Str(positionY))
            tmpCmd = tmpCmd & ", position_z"
            tmpValues = tmpValues & ", " & Trim(Str(positionZ))
            tmpCmd = tmpCmd & ", map"
            tmpValues = tmpValues & ", " & MapID
            tmpCmd = tmpCmd & ", instance"
            tmpValues = tmpValues & ", " & instance
            tmpCmd = tmpCmd & ", orientation"
            tmpValues = tmpValues & ", " & Trim(Str(orientation))
            tmpCmd = tmpCmd & ", time"
            tmpValues = tmpValues & ", UNIX_TIMESTAMP()"
            tmpCmd = tmpCmd & ", corpse_type"
            tmpValues = tmpValues & ", " & CorpseType

            'tmpCmd = tmpCmd & ", corpse_bytes1"
            'tmpValues = tmpValues & ", " & Bytes1
            'tmpCmd = tmpCmd & ", corpse_bytes2"
            'tmpValues = tmpValues & ", " & Bytes2
            'tmpCmd = tmpCmd & ", corpse_model"
            'tmpValues = tmpValues & ", " & Model
            'tmpCmd = tmpCmd & ", corpse_guild"
            'tmpValues = tmpValues & ", " & Guild

            'Dim temp(EquipmentSlots.EQUIPMENT_SLOT_END - 1) As String
            'For i As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
            '    temp(i) = Items(i)
            'Next
            'tmpCmd = tmpCmd & ", corpse_items"
            'tmpValues = tmpValues & ", """ & Join(temp, " ") & """"

            tmpCmd = tmpCmd & ") " & tmpValues & ");"
            CharacterDatabase.Update(tmpCmd)
        End Sub
        Public Sub Destroy()
            Dim packet As New PacketClass(OPCODES.SMSG_DESTROY_OBJECT)
            Try
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
            Finally
                packet.Dispose()
            End Try
            Me.Dispose()
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Me.RemoveFromWorld()
                WORLD_CORPSEOBJECTs.Remove(GUID)
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

        Public Sub New(ByRef Character As CharacterObject)
            'WARNING: Use only for spawning new object
            GUID = GetNewGUID()
            Bytes1 = (CType(Character.Race, Integer) << 8) + (CType(Character.Gender, Integer) << 16) + (CType(Character.Skin, Integer) << 24)
            Bytes2 = Character.Face + (CType(Character.HairStyle, Integer) << 8) + (CType(Character.HairColor, Integer) << 16) + (CType(Character.FacialHair, Integer) << 24)
            Model = Character.Model
            positionX = Character.positionX
            positionY = Character.positionY
            positionZ = Character.positionZ
            orientation = Character.orientation
            MapID = Character.MapID
            Owner = Character.GUID

            Character.corpseGUID = GUID
            Character.corpsePositionX = positionX
            Character.corpsePositionY = positionY
            Character.corpsePositionZ = positionZ
            Character.corpseMapID = MapID

            'TODO: The Corpse Type May Need to be Set Differently (Perhaps using Player Extra Flags)?
            If (Character.isPvP) Then
                Character.corpseCorpseType = CorpseType.CORPSE_RESURRECTABLE_PVP
            Else
                Character.corpseCorpseType = CorpseType.CORPSE_RESURRECTABLE_PVE
            End If

            Character.corpseCorpseType = CorpseType

            For i As Byte = 0 To EquipmentSlots.EQUIPMENT_SLOT_END - 1
                If Character.Items.ContainsKey(i) Then
                    Items(i) = Character.Items(i).ItemInfo.Model + (CType(Character.Items(i).ItemInfo.InventoryType, Integer) << 24)
                Else
                    Items(i) = 0
                End If
            Next

            Flags = 4

            WORLD_CORPSEOBJECTs.Add(GUID, Me)
        End Sub

        Public Sub New(ByVal cGUID As ULong, Optional ByRef Info As DataRow = Nothing)
            'WARNING: Use only for loading from DB
            If Info Is Nothing Then
                Dim MySQLQuery As New DataTable
                CharacterDatabase.Query(String.Format("SELECT * FROM corpse WHERE guid = {0};", cGUID), MySQLQuery)
                If MySQLQuery.Rows.Count > 0 Then
                    Info = MySQLQuery.Rows(0)
                Else
                    Log.WriteLine(LogType.FAILED, "Corpse not found in database. [corpseGUID={0:X}]", cGUID)
                    Return
                End If
            End If

            positionX = Info.Item("position_x")
            positionY = Info.Item("position_y")
            positionZ = Info.Item("position_z")
            orientation = Info.Item("orientation")

            MapID = Info.Item("map")
            instance = Info.Item("instance")

            Owner = Info.Item("player")
            CorpseType = Info.Item("corpse_type")
            'Bytes1 = Info.Item("corpse_bytes1")
            'Bytes2 = Info.Item("corpse_bytes2")
            'Model = Info.Item("corpse_model")
            'Guild = Info.Item("corpse_guild")

            'Dim tmp() As String
            'tmp = Split(CType(Info.Item("corpse_items"), String), " ")
            'For i As Integer = 0 To tmp.Length - 1
            '    Items(i) = tmp(i)
            'Next i

            Flags = 4

            GUID = cGUID + GUID_CORPSE
            WORLD_CORPSEOBJECTs.Add(GUID, Me)
        End Sub

        Public Sub AddToWorld()
            GetMapTile(positionX, positionY, CellX, CellY)
            If Maps(MapID).Tiles(CellX, CellY) Is Nothing Then MAP_Load(CellX, CellY, MapID)
            Maps(MapID).Tiles(CellX, CellY).CorpseObjectsHere.Add(GUID)

            Dim list() As ULong
            'DONE: Sending to players in nearby cells
            Dim packet As New PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
            Try
                Dim tmpUpdate As New UpdateClass(FIELD_MASK_SIZE_CORPSE)
                Try
                    packet.AddInt32(1)
                    packet.AddInt8(0)
                    FillAllUpdateFlags(tmpUpdate)
                    tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT, Me)
                Finally
                    tmpUpdate.Dispose()
                End Try

                For i As Short = -1 To 1
                    For j As Short = -1 To 1
                        If (CellX + i) >= 0 AndAlso (CellX + i) <= 63 AndAlso (CellY + j) >= 0 AndAlso (CellY + j) <= 63 AndAlso Maps(MapID).Tiles(CellX + i, CellY + j) IsNot Nothing AndAlso Maps(MapID).Tiles(CellX + i, CellY + j).PlayersHere.Count > 0 Then
                            With Maps(MapID).Tiles(CellX + i, CellY + j)
                                list = .PlayersHere.ToArray
                                For Each plGUID As ULong In list
                                    If CHARACTERs.ContainsKey(plGUID) AndAlso CHARACTERs(plGUID).CanSee(Me) Then
                                        CHARACTERs(plGUID).Client.SendMultiplyPackets(packet)
                                        CHARACTERs(plGUID).corpseObjectsNear.Add(GUID)
                                        SeenBy.Add(plGUID)
                                    End If
                                Next
                            End With
                        End If
                    Next
                Next
            Finally
                packet.Dispose()
            End Try
        End Sub

        Public Sub RemoveFromWorld()
            GetMapTile(positionX, positionY, CellX, CellY)
            Maps(MapID).Tiles(CellX, CellY).CorpseObjectsHere.Remove(GUID)

            Dim list() As ULong

            'DONE: Removing from players in <CENTER> Cell wich can see it
            If Maps(MapID).Tiles(CellX, CellY).PlayersHere.Count > 0 Then
                With Maps(MapID).Tiles(CellX, CellY)
                    list = .PlayersHere.ToArray
                    For Each plGUID As ULong In list
                        If CHARACTERs(plGUID).corpseObjectsNear.Contains(GUID) Then
                            CHARACTERs(plGUID).guidsForRemoving_Lock.AcquireWriterLock(DEFAULT_LOCK_TIMEOUT)
                            CHARACTERs(plGUID).guidsForRemoving.Add(GUID)
                            CHARACTERs(plGUID).guidsForRemoving_Lock.ReleaseWriterLock()

                            CHARACTERs(plGUID).corpseObjectsNear.Remove(GUID)
                        End If
                    Next
                End With
            End If
        End Sub
    End Class

    <MethodImplAttribute(MethodImplOptions.Synchronized)> _
    Private Function GetNewGUID() As ULong
        CorpseGUIDCounter += 1
        GetNewGUID = CorpseGUIDCounter
    End Function

End Module