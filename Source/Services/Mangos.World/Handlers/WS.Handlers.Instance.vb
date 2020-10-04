'
' Copyright (C) 2013-2020 getMaNGOS <https://getmangos.eu>
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

Imports System.Data
Imports Mangos.Common

Public Module WS_Handlers_Instance

    Public Sub InstanceMapUpdate()
        Dim q As New DataTable
        Dim t As UInteger = GetTimestamp(Now)
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_instances WHERE expire < {0};", t), q)

        For Each r As DataRow In q.Rows
            If Maps.ContainsKey(r.Item("map")) Then InstanceMapExpire(r.Item("map"), r.Item("instance"))
        Next
    End Sub
    Public Function InstanceMapCreate(ByVal Map As UInteger) As UInteger
        Dim q As New DataTable

        'TODO: Save instance IDs in MAP class, using current way it may happen 2 groups to be in same instance
        CharacterDatabase.Query(String.Format("SELECT MAX(instance) FROM characters_instances WHERE map = {0};", Map), q)
        If q.Rows(0).Item(0) IsNot DBNull.Value Then
            Return CInt(q.Rows(0).Item(0)) + 1
        Else
            Return 0
        End If
    End Function
    Public Sub InstanceMapSpawn(ByVal Map As UInteger, ByVal Instance As UInteger)
        'DONE: Load map data
        For x As Short = 0 To 63
            For y As Short = 0 To 63
                If Maps(Map).TileUsed(x, y) = False AndAlso IO.File.Exists(String.Format("maps\{0}{1}{2}.map", Format(Map, "000"), Format(x, "00"), Format(y, "00"))) Then
                    Log.WriteLine(LogType.INFORMATION, "Loading map [{2}: {0},{1}]...", x, y, Map)
                    Maps(Map).TileUsed(x, y) = True
                    Maps(Map).Tiles(x, y) = New TMapTile(x, y, Map)
                End If

                If (Maps(Map).Tiles(x, y) IsNot Nothing) Then
                    'DONE: Spawn the instance
                    LoadSpawns(x, y, Map, Instance)
                End If
            Next
        Next
    End Sub
    Public Sub InstanceMapExpire(ByVal Map As UInteger, ByVal Instance As UInteger)
        Dim empty As Boolean = True

        Try
            'DONE: Check for players
            For x As Short = 0 To 63
                For y As Short = 0 To 63
                    If Maps(Map).Tiles(x, y) IsNot Nothing Then
                        For Each GUID As ULong In Maps(Map).Tiles(x, y).PlayersHere.ToArray
                            If CHARACTERs(GUID).instance = Instance Then
                                empty = False
                                Exit For
                            End If
                        Next
                    End If
                    If Not empty Then Exit For
                Next
                If Not empty Then Exit For
            Next

            If empty Then
                'DONE: Delete the instance if there are no players
                CharacterDatabase.Update(String.Format("DELETE FROM characters_instances WHERE instance = {0} AND map = {1};", Instance, Map))
                CharacterDatabase.Update(String.Format("DELETE FROM characters_instances_group WHERE instance = {0} AND map = {1};", Instance, Map))

                'DONE: Delete spawned things
                For x As Short = 0 To 63
                    For y As Short = 0 To 63
                        If Maps(Map).Tiles(x, y) IsNot Nothing Then
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).CreaturesHere.ToArray
                                If WORLD_CREATUREs(GUID).instance = Instance Then WORLD_CREATUREs(GUID).Destroy()
                            Next
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).GameObjectsHere.ToArray
                                If WORLD_GAMEOBJECTs(GUID).instance = Instance Then WORLD_GAMEOBJECTs(GUID).Destroy(WORLD_GAMEOBJECTs(GUID))
                            Next
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).CorpseObjectsHere.ToArray
                                If WORLD_CORPSEOBJECTs(GUID).instance = Instance Then WORLD_CORPSEOBJECTs(GUID).Destroy()
                            Next
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).DynamicObjectsHere.ToArray
                                If WORLD_DYNAMICOBJECTs(GUID).instance = Instance Then WORLD_DYNAMICOBJECTs(GUID).Delete()
                            Next
                        End If

                    Next
                Next
            Else
                'DONE: Extend the expire time
                CharacterDatabase.Update(String.Format("UPDATE characters_instances SET expire = {2} WHERE instance = {0} AND map = {1};", Instance, Map, GetTimestamp(Now) + Maps(Map).ResetTime()))
                CharacterDatabase.Update(String.Format("UPDATE characters_instances_group SET expire = {2} WHERE instance = {0} AND map = {1};", Instance, Map, GetTimestamp(Now) + Maps(Map).ResetTime()))

                'DONE: Respawn the instance if there are players
                For x As Short = 0 To 63
                    For y As Short = 0 To 63
                        If Maps(Map).Tiles(x, y) IsNot Nothing Then
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).CreaturesHere.ToArray
                                If WORLD_CREATUREs(GUID).instance = Instance Then WORLD_CREATUREs(GUID).Respawn()
                            Next
                            For Each GUID As ULong In Maps(Map).Tiles(x, y).GameObjectsHere.ToArray
                                If WORLD_GAMEOBJECTs(GUID).instance = Instance Then WORLD_GAMEOBJECTs(GUID).Respawn(WORLD_GAMEOBJECTs(GUID))
                            Next
                        End If
                    Next
                Next
            End If

        Catch ex As Exception
            Log.WriteLine(LogType.CRITICAL, "Error expiring map instance.{0}{1}", Environment.NewLine, ex.ToString)
        End Try
    End Sub

    Public Sub InstanceMapEnter(ByVal objCharacter As CharacterObject)
        If Maps(objCharacter.MapID).Type = MapTypes.MAP_COMMON Then
            objCharacter.instance = 0

#If DEBUG Then
            objCharacter.SystemMessage(SetColor("You are not in instance.", 0, 0, 255))
#End If
        Else
            'DONE: Instances expire check
            InstanceMapUpdate()

            Dim q As New DataTable

            'DONE: Check if player is already saved to instance
            CharacterDatabase.Query(String.Format("SELECT * FROM characters_instances WHERE char_guid = {0} AND map = {1};", objCharacter.GUID, objCharacter.MapID), q)
            If q.Rows.Count > 0 Then
                'Character is saved to instance
                objCharacter.instance = q.Rows(0).Item("instance")
#If DEBUG Then
                objCharacter.SystemMessage(SetColor(String.Format("You are in instance #{0}, map {1}", objCharacter.instance, objCharacter.MapID), 0, 0, 255))
#End If
                SendInstanceMessage(objCharacter.client, objCharacter.MapID, q.Rows(0).Item("expire") - GetTimestamp(Now))
                Exit Sub
            End If

            'DONE: Check if group is already in instance
            If objCharacter.IsInGroup Then
                CharacterDatabase.Query(String.Format("SELECT * FROM characters_instances_group WHERE group_id = {0} AND map = {1};", objCharacter.Group.ID, objCharacter.MapID), q)

                If q.Rows.Count > 0 Then
                    'Group is saved to instance
                    objCharacter.instance = q.Rows(0).Item("instance")
#If DEBUG Then
                    objCharacter.SystemMessage(SetColor(String.Format("You are in instance #{0}, map {1}", objCharacter.instance, objCharacter.MapID), 0, 0, 255))
#End If
                    SendInstanceMessage(objCharacter.client, objCharacter.MapID, q.Rows(0).Item("expire") - GetTimestamp(Now))
                    Exit Sub
                End If
            End If

            'DONE Create new instance
            Dim instanceNewID As Integer = InstanceMapCreate(objCharacter.MapID)
            Dim instanceNewResetTime As Integer = GetTimestamp(Now) + Maps(objCharacter.MapID).ResetTime()

            'Set instance
            objCharacter.instance = instanceNewID

            If objCharacter.IsInGroup Then
                'Set group in the same instance
                CharacterDatabase.Update(String.Format("INSERT INTO characters_instances_group (group_id, map, instance, expire) VALUES ({0}, {1}, {2}, {3});", objCharacter.Group.ID, objCharacter.MapID, instanceNewID, instanceNewResetTime))
            End If

            InstanceMapSpawn(objCharacter.MapID, instanceNewID)

#If DEBUG Then
            objCharacter.SystemMessage(SetColor(String.Format("You are in instance #{0}, map {1}", objCharacter.instance, objCharacter.MapID), 0, 0, 255))
#End If
            SendInstanceMessage(objCharacter.client, objCharacter.MapID, GetTimestamp(Now) - instanceNewResetTime)
        End If
    End Sub
    Public Sub InstanceUpdate(ByVal Map As UInteger, ByVal Instance As UInteger, ByVal Cleared As UInteger)
        'NOTE: This should be used when a boss is killed, since he and his units will no longer spawn, raid instances only
        'TODO: Save everybody to the instance at the first kill
        'TODO: Save the instance to the database
    End Sub
    Public Sub InstanceMapLeave(ByVal objChar As CharacterObject)
        'TODO: Start teleport timer
    End Sub

    'SMSG_INSTANCE_DIFFICULTY
    Public Sub SendResetInstanceSuccess(ByRef client As ClientClass, ByVal Map As UInteger)
        'Dim p As New PacketClass(OPCODES.SMSG_INSTANCE_RESET)
        'p.AddUInt32(Map)
        'Client.Send(p)
        'p.Dispose()
    End Sub
    Public Sub SendResetInstanceFailed(ByRef client As ClientClass, ByVal Map As UInteger, ByVal Reason As ResetFailedReason)
        'Dim p As New PacketClass(OPCODES.SMSG_INSTANCE_RESET)
        'p.AddUInt32(Reason)
        'p.AddUInt32(Map)
        'Client.Send(p)
        'p.Dispose()
    End Sub
    Public Sub SendResetInstanceFailedNotify(ByRef client As ClientClass, ByVal Map As UInteger)
        'Dim p As New PacketClass(OPCODES.SMSG_RESET_FAILED_NOTIFY)
        'p.AddUInt32(Map)
        'Client.Send(p)
        'p.Dispose()
    End Sub

    Private Sub SendUpdateInstanceOwnership(ByRef client As ClientClass, ByVal Saved As UInteger)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_UPDATE_INSTANCE_OWNERSHIP", client.IP, client.Port)

        'Dim p As New PacketClass(OPCODES.SMSG_UPDATE_INSTANCE_OWNERSHIP)
        'p.AddUInt32(Saved)                  'True/False if have been saved
        'Client.Send(p)
        'p.Dispose()
    End Sub
    Private Sub SendUpdateLastInstance(ByRef client As ClientClass, ByVal Map As UInteger)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_UPDATE_LAST_INSTANCE", client.IP, client.Port)

        'Dim p As New PacketClass(OPCODES.SMSG_UPDATE_LAST_INSTANCE)
        'p.AddUInt32(Map)
        'Client.Send(p)
        'p.Dispose()
    End Sub
    Public Sub SendInstanceSaved(ByVal Character As CharacterObject)
        Dim q As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_instances WHERE char_guid = {0};", Character.GUID), q)

        SendUpdateInstanceOwnership(Character.client, q.Rows.Count > 0)

        For Each r As DataRow In q.Rows
            SendUpdateLastInstance(Character.client, r.Item("map"))
        Next
    End Sub

    Public Sub SendInstanceMessage(ByRef client As ClientClass, ByVal Map As UInteger, ByVal Time As Integer)
        Dim Type As RaidInstanceMessage

        If Time < 0 Then
            Type = RaidInstanceMessage.RAID_INSTANCE_WELCOME
            Time = -Time
        ElseIf Time > 60 AndAlso Time < 3600 Then
            Type = RaidInstanceMessage.RAID_INSTANCE_WARNING_MIN
        ElseIf Time > 3600 Then
            Type = RaidInstanceMessage.RAID_INSTANCE_WARNING_HOURS
        ElseIf Time < 60 Then
            Type = RaidInstanceMessage.RAID_INSTANCE_WARNING_MIN_SOON
        End If

        'Dim p As New PacketClass(OPCODES.SMSG_RAID_INSTANCE_MESSAGE)
        'p.AddUInt32(Type)
        'p.AddUInt32(Map)
        'p.AddUInt32(Time)
        'Client.Send(p)
        'p.Dispose()
    End Sub

End Module