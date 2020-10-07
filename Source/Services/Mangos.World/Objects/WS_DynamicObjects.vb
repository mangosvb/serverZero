﻿'
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

Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.World.Globals
Imports Mangos.World.Handlers
Imports Mangos.World.Maps
Imports Mangos.World.Server
Imports Mangos.World.Spells

Namespace Objects

    Public Class WS_DynamicObjects
        Private Function GetNewGUID() As ULong
            _WorldServer.DynamicObjectsGUIDCounter += 1
            GetNewGUID = _WorldServer.DynamicObjectsGUIDCounter
        End Function

        Public Class DynamicObjectObject
            Inherits WS_Base.BaseObject
            Implements IDisposable

            Public SpellID As Integer = 0
            Public Effects As New List(Of WS_Spells.SpellEffect)
            Public Duration As Integer = 0
            Public Radius As Single = 0
            Public Caster As WS_Base.BaseUnit
            Public CastTime As Integer = 0
            Public Bytes As Integer = 1

#Region "IDisposable Support"
            Private _disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not _disposedValue Then
                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.
                    _WorldServer.WORLD_DYNAMICOBJECTs.Remove(GUID)
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

            Public Sub New(ByRef Caster_ As WS_Base.BaseUnit, ByVal SpellID_ As Integer, ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Duration_ As Integer, ByVal Radius_ As Single)
                GUID = _WS_DynamicObjects.GetNewGUID()
                _WorldServer.WORLD_DYNAMICOBJECTs.Add(GUID, Me)

                Caster = Caster_
                SpellID = SpellID_
                positionX = PosX
                positionY = PosY
                positionZ = PosZ
                orientation = 0
                MapID = Caster.MapID
                instance = Caster.instance
                Duration = Duration_
                Radius = Radius_

                CastTime = _NativeMethods.timeGetTime("")
            End Sub

            Public Sub FillAllUpdateFlags(ByRef Update As Packets.UpdateClass)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_GUID, GUID)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_TYPE, ObjectType.TYPE_DYNAMICOBJECT + ObjectType.TYPE_OBJECT)
                Update.SetUpdateFlag(EObjectFields.OBJECT_FIELD_SCALE_X, 0.5F * Radius)

                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_CASTER, Caster.GUID)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_BYTES, Bytes)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_SPELLID, SpellID)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_RADIUS, Radius)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_POS_X, positionX)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_POS_Y, positionY)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_POS_Z, positionZ)
                Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_FACING, orientation)
                'Update.SetUpdateFlag(EDynamicObjectFields.DYNAMICOBJECT_CASTTIME, CastTime)
            End Sub

            Public Sub AddToWorld()
                _WS_Maps.GetMapTile(positionX, positionY, CellX, CellY)
                If _WS_Maps.Maps(MapID).Tiles(CellX, CellY) Is Nothing Then _WS_CharMovement.MAP_Load(CellX, CellY, MapID)
                Try
                    _WS_Maps.Maps(MapID).Tiles(CellX, CellY).DynamicObjectsHere.Add(GUID)
                Catch
                    _WorldServer.Log.WriteLine(LogType.WARNING, "AddToWorld failed MapId: {0} Tile XY: {1} {2} GUID: {3}", MapID, CellX, CellY, GUID)
                    Exit Sub
                End Try

                Dim list() As ULong
                'DONE: Sending to players in nearby cells
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_UPDATE_OBJECT)
                packet.AddInt32(1)
                packet.AddInt8(0)
                Dim tmpUpdate As New Packets.UpdateClass(_Global_Constants.FIELD_MASK_SIZE_DYNAMICOBJECT)
                FillAllUpdateFlags(tmpUpdate)
                tmpUpdate.AddToPacket(packet, ObjectUpdateType.UPDATETYPE_CREATE_OBJECT_SELF, Me)
                tmpUpdate.Dispose()

                For i As Short = -1 To 1
                    For j As Short = -1 To 1
                        If (CellX + i) >= 0 AndAlso (CellX + i) <= 63 AndAlso (CellY + j) >= 0 AndAlso (CellY + j) <= 63 AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j) IsNot Nothing AndAlso _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j).PlayersHere.Count > 0 Then
                            With _WS_Maps.Maps(MapID).Tiles(CellX + i, CellY + j)
                                list = .PlayersHere.ToArray
                                For Each plGUID As ULong In list
                                    If _WorldServer.CHARACTERs.ContainsKey(plGUID) AndAlso _WorldServer.CHARACTERs(plGUID).CanSee(Me) Then
                                        _WorldServer.CHARACTERs(plGUID).client.SendMultiplyPackets(packet)
                                        _WorldServer.CHARACTERs(plGUID).dynamicObjectsNear.Add(GUID)
                                        SeenBy.Add(plGUID)
                                    End If
                                Next
                            End With
                        End If
                    Next
                Next

                packet.Dispose()

            End Sub
            Public Sub RemoveFromWorld()
                _WS_Maps.GetMapTile(positionX, positionY, CellX, CellY)
                _WS_Maps.Maps(MapID).Tiles(CellX, CellY).DynamicObjectsHere.Remove(GUID)

                'DONE: Remove the dynamic object from players that can see the object
                For Each plGUID As ULong In SeenBy.ToArray
                    If _WorldServer.CHARACTERs(plGUID).dynamicObjectsNear.Contains(GUID) Then
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.AcquireWriterLock(_Global_Constants.DEFAULT_LOCK_TIMEOUT)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving.Add(GUID)
                        _WorldServer.CHARACTERs(plGUID).guidsForRemoving_Lock.ReleaseWriterLock()

                        _WorldServer.CHARACTERs(plGUID).dynamicObjectsNear.Remove(GUID)
                    End If
                Next
            End Sub

            Public Sub AddEffect(ByVal EffectInfo As WS_Spells.SpellEffect)
                Effects.Add(EffectInfo)
            End Sub

            Public Sub RemoveEffect(ByVal EffectInfo As WS_Spells.SpellEffect)
                Effects.Remove(EffectInfo)
            End Sub

            Public Function Update() As Boolean
                'DONE: Remove if caster doesn't exist
                If Caster Is Nothing Then
                    Return True
                End If

                'DONE: Tick down
                Dim DeleteThis As Boolean = False
                If Duration > WS_TimerBasedEvents.TSpellManager.UPDATE_TIMER Then
                    Duration -= WS_TimerBasedEvents.TSpellManager.UPDATE_TIMER
                Else
                    DeleteThis = True
                End If

                'DONE: Do the spell
                For Each Effect As WS_Spells.SpellEffect In Effects
                    If Effect.GetRadius = 0 Then
                        If Effect.Amplitude = 0 OrElse ((_WS_Spells.SPELLs(SpellID).GetDuration - Duration) Mod Effect.Amplitude) = 0 Then
                            _WS_Spells.AURAs(Effect.ApplyAuraIndex).Invoke(Caster, Me, Effect, SpellID, 1, AuraAction.AURA_UPDATE)
                        End If
                    Else
                        Dim Targets As List(Of WS_Base.BaseUnit) = _WS_Spells.GetEnemyAtPoint(Caster, positionX, positionY, positionZ, Effect.GetRadius)
                        For Each Target As WS_Base.BaseUnit In Targets
                            If Effect.Amplitude = 0 OrElse ((_WS_Spells.SPELLs(SpellID).GetDuration - Duration) Mod Effect.Amplitude) = 0 Then
                                _WS_Spells.AURAs(Effect.ApplyAuraIndex).Invoke(Target, Me, Effect, SpellID, 1, AuraAction.AURA_UPDATE)
                            End If
                        Next
                    End If
                Next

                'DONE: Remove when done
                If DeleteThis Then
                    Caster.dynamicObjects.Remove(Me)
                    Return True
                End If

                Return False
            End Function

            Public Sub Spawn()
                AddToWorld()

                'DONE: Send spawn animation
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_GAMEOBJECT_SPAWN_ANIM)
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
                packet.Dispose()
            End Sub

            Public Sub Delete()
                'DONE: Remove the reference between the dynamic object and the caster
                If Caster IsNot Nothing AndAlso Caster.dynamicObjects.Contains(Me) Then Caster.dynamicObjects.Remove(Me)

                'DONE: Send despawn animation
                Dim packet As New Packets.PacketClass(OPCODES.SMSG_GAMEOBJECT_DESPAWN_ANIM)
                packet.AddUInt64(GUID)
                SendToNearPlayers(packet)
                packet.Dispose()

                RemoveFromWorld()
                Dispose()
            End Sub
        End Class
    End Class
End Namespace