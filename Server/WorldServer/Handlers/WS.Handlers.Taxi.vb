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
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports mangosVB.Common.BaseWriter
Imports mangosVB.Common.NativeMethods


Public Module WS_Handlers_Taxi


    Private Enum ActivateTaxiReplies As Byte
        ERR_TAXIOK = 0
        ERR_TAXIUNSPECIFIEDSERVERERROR = 1
        ERR_TAXINOSUCHPATH = 2
        ERR_TAXINOTENOUGHMONEY = 3
        ERR_TAXITOOFARAWAY = 4
        ERR_TAXINOVENDORNEARBY = 5
        ERR_TAXINOTVISITED = 6
        ERR_TAXIPLAYERBUSY = 7
        ERR_TAXIPLAYERALREADYMOUNTED = 8
        ERR_TAXIPLAYERSHAPESHIFTED = 9
        ERR_TAXIPLAYERMOVING = 10
        ERR_TAXISAMENODE = 11
        ERR_TAXINOTSTANDING = 12
    End Enum
    Private Sub SendActivateTaxiReply(ByRef Client As ClientClass, ByVal Reply As ActivateTaxiReplies)
        Dim TaxiFailed As New PacketClass(OPCODES.SMSG_ACTIVATETAXIREPLY)
        TaxiFailed.AddInt32(Reply)
        Client.Send(TaxiFailed)
        TaxiFailed.Dispose()
    End Sub

    Public Sub SendTaxiStatus(ByRef c As CharacterObject, ByVal cGUID As ULong)
        If WORLD_CREATUREs.ContainsKey(cGUID) = False Then Exit Sub

        Dim currentTaxi As Integer = GetNearestTaxi(WORLD_CREATUREs(cGUID).positionX, WORLD_CREATUREs(cGUID).positionY, WORLD_CREATUREs(cGUID).MapID)

        Dim SMSG_TAXINODE_STATUS As New PacketClass(OPCODES.SMSG_TAXINODE_STATUS)
        SMSG_TAXINODE_STATUS.AddUInt64(cGUID)
        If c.TaxiZones.Item(currentTaxi) = False Then SMSG_TAXINODE_STATUS.AddInt8(0) Else SMSG_TAXINODE_STATUS.AddInt8(1)
        c.Client.Send(SMSG_TAXINODE_STATUS)
        SMSG_TAXINODE_STATUS.Dispose()
    End Sub

    Public Sub SendTaxiMenu(ByRef c As CharacterObject, ByVal cGUID As ULong)
        If WORLD_CREATUREs.ContainsKey(cGUID) = False Then Exit Sub

        Dim currentTaxi As Integer = GetNearestTaxi(WORLD_CREATUREs(cGUID).positionX, WORLD_CREATUREs(cGUID).positionY, WORLD_CREATUREs(cGUID).MapID)

        If c.TaxiZones.Item(currentTaxi) = False Then
            c.TaxiZones.Set(currentTaxi, True)

            Dim SMSG_NEW_TAXI_PATH As New PacketClass(OPCODES.SMSG_NEW_TAXI_PATH)
            c.Client.Send(SMSG_NEW_TAXI_PATH)
            SMSG_NEW_TAXI_PATH.Dispose()

            Dim SMSG_TAXINODE_STATUS As New PacketClass(OPCODES.SMSG_TAXINODE_STATUS)
            SMSG_TAXINODE_STATUS.AddUInt64(cGUID)
            SMSG_TAXINODE_STATUS.AddInt8(1)
            c.Client.Send(SMSG_TAXINODE_STATUS)
            SMSG_TAXINODE_STATUS.Dispose()
            Exit Sub
        End If


        Dim SMSG_SHOWTAXINODES As New PacketClass(OPCODES.SMSG_SHOWTAXINODES)
        SMSG_SHOWTAXINODES.AddInt32(1)
        SMSG_SHOWTAXINODES.AddUInt64(cGUID)
        SMSG_SHOWTAXINODES.AddInt32(currentTaxi)
        SMSG_SHOWTAXINODES.AddBitArray(c.TaxiZones, 8 * 4)
        c.Client.Send(SMSG_SHOWTAXINODES)
        SMSG_SHOWTAXINODES.Dispose()
    End Sub

    Public Sub On_CMSG_TAXINODE_STATUS_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TAXINODE_STATUS_QUERY [taxiGUID={2:X}]", Client.IP, Client.Port, GUID)
        If WORLD_CREATUREs.ContainsKey(GUID) = False Then Exit Sub

        SendTaxiStatus(Client.Character, GUID)
    End Sub
    Public Sub On_CMSG_TAXIQUERYAVAILABLENODES(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TAXIQUERYAVAILABLENODES [taxiGUID={2:X}]", Client.IP, Client.Port, GUID)
        If WORLD_CREATUREs.ContainsKey(GUID) = False Then Exit Sub
        If (WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then Exit Sub 'NPC is not a taxi vendor

        SendTaxiMenu(Client.Character, GUID)
    End Sub
    Public Sub On_CMSG_ACTIVATETAXI(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim GUID As ULong = packet.GetUInt64
        Dim srcNode As Integer = packet.GetInt32
        Dim dstNode As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACTIVATETAXI [taxiGUID={2:X} srcNode={3} dstNode={4}]", Client.IP, Client.Port, GUID, srcNode, dstNode)

        If WORLD_CREATUREs.ContainsKey(GUID) = False OrElse (WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOVENDORNEARBY)
            Exit Sub
        End If
        If Not Client.Character.LogoutTimer Is Nothing Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERBUSY)
            Exit Sub
        End If
        If (Client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_DISABLE_MOVE) Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOTSTANDING)
            Exit Sub
        End If
        If Client.Character.ShapeshiftForm > 0 AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BERSERKERSTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BATTLESTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_DEFENSIVESTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_SHADOW Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERSHAPESHIFTED)
            Exit Sub
        End If
        If Client.Character.Mount <> 0 Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERALREADYMOUNTED)
            Exit Sub
        End If

        If TaxiNodes.ContainsKey(srcNode) = False OrElse TaxiNodes.ContainsKey(dstNode) = False Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
            Exit Sub
        End If

        Dim Mount As Integer = 0
        If Client.Character.Side Then
            If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).HordeMount) = False Then Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).HordeMount)
            Mount = CREATURESDatabase(TaxiNodes(srcNode).HordeMount).ModelA1
        Else
            If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).AllianceMount) = False Then Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).AllianceMount)
            Mount = CREATURESDatabase(TaxiNodes(srcNode).AllianceMount).ModelA2
        End If
        If Mount = 0 Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIUNSPECIFIEDSERVERERROR)
            Exit Sub
        End If

        'DONE: Reputation discount
        Dim DiscountMod As Single = Client.Character.GetDiscountMod(WORLD_CREATUREs(GUID).Faction)
        Dim TotalCost As Integer
        For Each TaxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
            If TaxiPath.Value.TFrom = srcNode AndAlso TaxiPath.Value.TTo = dstNode Then
                TotalCost += TaxiPath.Value.Price * DiscountMod
                Exit For
            End If
        Next

        'DONE: Check if we have enough money
        If Client.Character.Copper < TotalCost Then
            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
            Exit Sub
        End If
        Client.Character.Copper -= TotalCost

        Client.Character.TaxiNodes.Clear()
        Client.Character.TaxiNodes.Enqueue(srcNode)
        Client.Character.TaxiNodes.Enqueue(dstNode)

        SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIOK)

        'DONE: Mount up, disable move and spell casting
        TaxiTake(Client.Character, Mount)
        TaxiMove(Client.Character, DiscountMod)
    End Sub
    Public Sub On_CMSG_ACTIVATETAXI_FAR(ByRef packet As PacketClass, ByRef Client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Try
            Dim GUID As ULong = packet.GetUInt64
            Dim TotalCost As Integer = packet.GetInt32
            Dim NodeCount As Integer = packet.GetInt32
            If NodeCount <= 0 Then Exit Sub
            If (packet.Data.Length - 1) < (21 + (4 * NodeCount)) Then Exit Sub

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACTIVATETAXI_FAR [taxiGUID={2:X} TotalCost={3} NodeCount={4}]", Client.IP, Client.Port, GUID, TotalCost, NodeCount)

            If WORLD_CREATUREs.ContainsKey(GUID) = False OrElse (WORLD_CREATUREs(GUID).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOVENDORNEARBY)
                Exit Sub
            End If
            If Not Client.Character.LogoutTimer Is Nothing Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERBUSY)
                Exit Sub
            End If
            If (Client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_DISABLE_MOVE) Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOTSTANDING)
                Exit Sub
            End If
            If Client.Character.ShapeshiftForm > 0 AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BERSERKERSTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BATTLESTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_DEFENSIVESTANCE AndAlso Client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_SHADOW Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERSHAPESHIFTED)
                Exit Sub
            End If
            If Client.Character.Mount <> 0 Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIPLAYERALREADYMOUNTED)
                Exit Sub
            End If
            If NodeCount < 1 Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
                Exit Sub
            End If
            If Client.Character.Copper < TotalCost Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
                Exit Sub
            End If


            'DONE: Load nodes
            Dim Nodes As New List(Of Integer)
            For i As Integer = 0 To NodeCount - 1
                Nodes.Add(packet.GetInt32)
            Next
            Dim srcNode As Integer = Nodes(0)
            Dim dstNode As Integer = Nodes(1)


            For Each Node As Integer In Client.Character.TaxiNodes
                If Not TaxiNodes.ContainsKey(Node) Then
                    SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
                    Exit Sub
                End If
            Next

            Dim Mount As Integer = 0
            If Client.Character.Side Then
                If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).HordeMount) = False Then Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).HordeMount)
                Mount = CREATURESDatabase(TaxiNodes(srcNode).HordeMount).GetFirstModel
            Else
                If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).AllianceMount) = False Then Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).AllianceMount)
                Mount = CREATURESDatabase(TaxiNodes(srcNode).AllianceMount).GetFirstModel
            End If
            If Mount = 0 Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIUNSPECIFIEDSERVERERROR)
                Exit Sub
            End If

            'DONE: Reputation discount
            Dim DiscountMod As Single = Client.Character.GetDiscountMod(WORLD_CREATUREs(GUID).Faction)
            TotalCost = 0
            For Each TaxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
                If TaxiPath.Value.TFrom = srcNode AndAlso TaxiPath.Value.TTo = dstNode Then
                    TotalCost += TaxiPath.Value.Price * DiscountMod
                    Exit For
                End If
            Next

            'DONE: Check if we have enough money
            If Client.Character.Copper < TotalCost Then
                SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
                Exit Sub
            End If
            Client.Character.Copper -= TotalCost

            Client.Character.TaxiNodes.Clear()
            For Each Node As Integer In Nodes
                Client.Character.TaxiNodes.Enqueue(Node)
            Next

            SendActivateTaxiReply(Client, ActivateTaxiReplies.ERR_TAXIOK)

            'DONE: Mount up, disable move and spell casting
            TaxiTake(Client.Character, Mount)
            TaxiMove(Client.Character, DiscountMod)

        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error when taking a long taxi.{0}", vbNewLine & e.ToString)
        End Try
    End Sub
    Public Sub On_CMSG_MOVE_SPLINE_DONE(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOVE_SPLINE_DONE", Client.IP, Client.Port)
    End Sub


    Private Sub TaxiLand(ByVal Character As CharacterObject)
        Character.TaxiNodes.Clear()
        Character.Mount = 0
        Character.cUnitFlags = Character.cUnitFlags And (Not UnitFlags.UNIT_FLAG_DISABLE_MOVE)
        Character.cUnitFlags = Character.cUnitFlags And (Not UnitFlags.UNIT_FLAG_TAXI_FLIGHT)
        Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Character.Mount)
        Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Character.cUnitFlags)
        Character.SendCharacterUpdate()
    End Sub
    Private Sub TaxiTake(ByVal Character As CharacterObject, ByVal Mount As Integer)
        Character.Mount = Mount
        Character.cUnitFlags = Character.cUnitFlags Or UnitFlags.UNIT_FLAG_DISABLE_MOVE
        Character.cUnitFlags = Character.cUnitFlags Or UnitFlags.UNIT_FLAG_TAXI_FLIGHT
        Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, Character.Mount)
        Character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, Character.cUnitFlags)
        Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Character.Copper)
        Character.SendCharacterUpdate()
    End Sub
    Public Sub TaxiMove(ByVal Character As CharacterObject, ByVal DiscountMod As Single)
        Dim FlagFirstNode As Boolean = True
        Dim LastX As Single
        Dim LastY As Single
        Dim LastZ As Single
        Dim MoveDistance As Single
        Dim TotalDistance As Single

        Dim WaypointPaths As New List(Of Integer)
        Dim WaypointNodes As New Dictionary(Of Integer, TTaxiPathNode)


        Try
            'DONE: Generate paths
            Dim srcNode As Integer
            Dim dstNode As Integer = Character.TaxiNodes.Dequeue
            While Character.TaxiNodes.Count > 0
                srcNode = dstNode
                dstNode = Character.TaxiNodes.Dequeue

                For Each TaxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
                    If TaxiPath.Value.TFrom = srcNode AndAlso TaxiPath.Value.TTo = dstNode Then
                        WaypointPaths.Add(TaxiPath.Key)
                        Exit For
                    End If
                Next
            End While


            'DONE: Do move on paths
            For Each Path As Integer In WaypointPaths
                If FlagFirstNode Then
                    'DONE: Don't tax first node, it is already taxed
                    FlagFirstNode = False
                Else
                    'DONE: Remove the money for this flight
                    Dim Price As Integer = TaxiPaths(Path).Price * DiscountMod
                    If Character.Copper < Price Then Exit For
                    Character.Copper -= Price
                    Character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, Character.Copper)
                    Character.SendCharacterUpdate(False)
                    Console.WriteLine("Paying {0}", Price)
                End If

                LastX = Character.positionX
                LastY = Character.positionY
                LastZ = Character.positionZ
                TotalDistance = 0

                'DONE: Get the waypoints
                WaypointNodes.Clear()
                For Each TaxiPathNode As KeyValuePair(Of Integer, TTaxiPathNode) In TaxiPathNodes(Path)
                    WaypointNodes.Add(TaxiPathNode.Value.Seq, TaxiPathNode.Value)

                    TotalDistance += GetDistance(LastX, TaxiPathNode.Value.x, LastY, TaxiPathNode.Value.y, LastZ, TaxiPathNode.Value.z)
                    LastX = TaxiPathNode.Value.x
                    LastY = TaxiPathNode.Value.y
                    LastZ = TaxiPathNode.Value.z
                Next

                LastX = Character.positionX
                LastY = Character.positionY
                LastZ = Character.positionZ

                'Send move packet for player
                Dim SMSG_MONSTER_MOVE As New PacketClass(OPCODES.SMSG_MONSTER_MOVE)
                SMSG_MONSTER_MOVE.AddPackGUID(Character.GUID)
                SMSG_MONSTER_MOVE.AddSingle(Character.positionX)
                SMSG_MONSTER_MOVE.AddSingle(Character.positionY)
                SMSG_MONSTER_MOVE.AddSingle(Character.positionZ)
                SMSG_MONSTER_MOVE.AddInt32(timeGetTime(""))
                SMSG_MONSTER_MOVE.AddInt8(0)
                SMSG_MONSTER_MOVE.AddInt32(&H300)                           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                SMSG_MONSTER_MOVE.AddInt32(Fix(TotalDistance / UNIT_NORMAL_TAXI_SPEED * 1000))   'Time
                SMSG_MONSTER_MOVE.AddInt32(WaypointNodes.Count)             'Points Count
                For j As Integer = 0 To WaypointNodes.Count - 1
                    SMSG_MONSTER_MOVE.AddSingle(WaypointNodes(j).x)         'First Point X
                    SMSG_MONSTER_MOVE.AddSingle(WaypointNodes(j).y)         'First Point Y
                    SMSG_MONSTER_MOVE.AddSingle(WaypointNodes(j).z)         'First Point Z
                Next
                Character.Client.Send(SMSG_MONSTER_MOVE)
                SMSG_MONSTER_MOVE.Dispose()


                For i As Integer = 0 To WaypointNodes.Count - 1
                    MoveDistance = GetDistance(LastX, WaypointNodes(i).x, LastY, WaypointNodes(i).y, LastZ, WaypointNodes(i).z)

                    LastX = WaypointNodes(i).x
                    LastY = WaypointNodes(i).y
                    LastZ = WaypointNodes(i).z

                    'Send move packet for other players
                    Dim p As New PacketClass(OPCODES.SMSG_MONSTER_MOVE)
                    p.AddPackGUID(Character.GUID)
                    p.AddSingle(Character.positionX)
                    p.AddSingle(Character.positionY)
                    p.AddSingle(Character.positionZ)
                    p.AddInt32(timeGetTime(""))
                    p.AddInt8(0)
                    p.AddInt32(&H300)                           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                    p.AddInt32(Fix(TotalDistance / UNIT_NORMAL_TAXI_SPEED * 1000))   'Time
                    p.AddInt32(WaypointNodes.Count)             'Points Count
                    For j As Integer = i To WaypointNodes.Count - 1
                        p.AddSingle(WaypointNodes(j).x)         'First Point X
                        p.AddSingle(WaypointNodes(j).y)         'First Point Y
                        p.AddSingle(WaypointNodes(j).z)         'First Point Z
                    Next
                    Character.SendToNearPlayers(p, , False)
                    p.Dispose()

                    'Wait move to complete
                    Thread.Sleep(Fix(MoveDistance / UNIT_NORMAL_TAXI_SPEED * 1000))

                    'Update character postion
                    TotalDistance -= MoveDistance
                    Character.positionX = LastX
                    Character.positionY = LastY
                    Character.positionZ = LastZ
                    MoveCell(Character)
                    UpdateCell(Character)
                Next

            Next

        Catch ex As Exception
            Log.WriteLine(LogType.FAILED, "Error on flight: {0}", ex.ToString)
        End Try

        Character.Save()
        TaxiLand(Character)
    End Sub

End Module
