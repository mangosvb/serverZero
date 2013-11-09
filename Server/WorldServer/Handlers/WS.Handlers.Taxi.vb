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
Imports System.Collections.Generic
Imports mangosVB.Common.Logger
Imports mangosVB.Common.NativeMethods
Imports mangosVB.Common

Public Module WS_Handlers_Taxi
    ''' <summary>
    ''' Sends the activate taxi reply.
    ''' </summary>
    ''' <param name="client">The client.</param>
    ''' <param name="reply">The reply.</param>
    ''' <returns></returns>
    Private Sub SendActivateTaxiReply(ByRef client As ClientClass, ByVal reply As ActivateTaxiReplies)
        Dim taxiFailed As New PacketClass(OPCODES.SMSG_ACTIVATETAXIREPLY)
        Try
            taxiFailed.AddInt32(reply)
            client.Send(taxiFailed)
        Finally
            taxiFailed.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the taxi status.
    ''' </summary>
    ''' <param name="objCharacter">The objCharacter.</param>
    ''' <param name="cGuid">The objCharacter GUID.</param>
    ''' <returns></returns>
    Private Sub SendTaxiStatus(ByRef objCharacter As CharacterObject, ByVal cGuid As ULong)
        If WORLD_CREATUREs.ContainsKey(cGuid) = False Then Exit Sub

        Dim currentTaxi As Integer = GetNearestTaxi(WORLD_CREATUREs(cGuid).positionX, WORLD_CREATUREs(cGuid).positionY, WORLD_CREATUREs(cGuid).MapID)

        Dim SMSG_TAXINODE_STATUS As New PacketClass(OPCODES.SMSG_TAXINODE_STATUS)
        Try
            SMSG_TAXINODE_STATUS.AddUInt64(cGuid)
            If objCharacter.TaxiZones.Item(currentTaxi) = False Then SMSG_TAXINODE_STATUS.AddInt8(0) Else SMSG_TAXINODE_STATUS.AddInt8(1)
            objCharacter.Client.Send(SMSG_TAXINODE_STATUS)
        Finally
            SMSG_TAXINODE_STATUS.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Sends the taxi menu.
    ''' </summary>
    ''' <param name="objCharacter">The objCharacter.</param>
    ''' <param name="cGuid">The objCharacter GUID.</param>
    ''' <returns></returns>
    Public Sub SendTaxiMenu(ByRef objCharacter As CharacterObject, ByVal cGuid As ULong)
        If WORLD_CREATUREs.ContainsKey(cGuid) = False Then Exit Sub

        Dim currentTaxi As Integer = GetNearestTaxi(WORLD_CREATUREs(cGuid).positionX, WORLD_CREATUREs(cGuid).positionY, WORLD_CREATUREs(cGuid).MapID)

        If objCharacter.TaxiZones.Item(currentTaxi) = False Then
            objCharacter.TaxiZones.Set(currentTaxi, True)

            Dim SMSG_NEW_TAXI_PATH As New PacketClass(OPCODES.SMSG_NEW_TAXI_PATH)
            Try
                objCharacter.Client.Send(SMSG_NEW_TAXI_PATH)
            Finally
                SMSG_NEW_TAXI_PATH.Dispose()
            End Try

            Dim SMSG_TAXINODE_STATUS As New PacketClass(OPCODES.SMSG_TAXINODE_STATUS)
            Try
                SMSG_TAXINODE_STATUS.AddUInt64(cGuid)
                SMSG_TAXINODE_STATUS.AddInt8(1)
                objCharacter.Client.Send(SMSG_TAXINODE_STATUS)
            Finally
                SMSG_TAXINODE_STATUS.Dispose()
            End Try
            Exit Sub
        End If

        Dim SMSG_SHOWTAXINODES As New PacketClass(OPCODES.SMSG_SHOWTAXINODES)
        Try
            SMSG_SHOWTAXINODES.AddInt32(1)
            SMSG_SHOWTAXINODES.AddUInt64(cGuid)
            SMSG_SHOWTAXINODES.AddInt32(currentTaxi)
            SMSG_SHOWTAXINODES.AddBitArray(objCharacter.TaxiZones, 8 * 4)
            objCharacter.Client.Send(SMSG_SHOWTAXINODES)
        Finally
            SMSG_SHOWTAXINODES.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Called when [CMSG_TAXINODE_STATUS_QUERY] is received.
    ''' </summary>
    ''' <param name="packet">The packet.</param>
    ''' <param name="client">The client.</param>
    ''' <returns></returns>
    Public Sub On_CMSG_TAXINODE_STATUS_QUERY(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong
        guid = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TAXINODE_STATUS_QUERY [taxiGUID={2:X}]", client.IP, client.Port, guid)
        If WORLD_CREATUREs.ContainsKey(guid) = False Then Exit Sub

        SendTaxiStatus(client.Character, guid)
    End Sub

    ''' <summary>
    ''' Called when [CMSG_TAXIQUERYAVAILABLENODES] is received.
    ''' </summary>
    ''' <param name="packet">The packet.</param>
    ''' <param name="client">The client.</param>
    ''' <returns></returns>
    Public Sub On_CMSG_TAXIQUERYAVAILABLENODES(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 13 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong
        guid = packet.GetUInt64
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_TAXIQUERYAVAILABLENODES [taxiGUID={2:X}]", client.IP, client.Port, guid)
        If WORLD_CREATUREs.ContainsKey(guid) = False Then Exit Sub
        If (WORLD_CREATUREs(guid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then Exit Sub 'NPC is not a taxi vendor

        SendTaxiMenu(client.Character, guid)
    End Sub

    ''' <summary>
    ''' Called when [CMSG_ACTIVATETAXI] is received.
    ''' </summary>
    ''' <param name="packet">The packet.</param>
    ''' <param name="client">The client.</param>
    ''' <returns></returns>
    Public Sub On_CMSG_ACTIVATETAXI(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Dim guid As ULong
        guid = packet.GetUInt64
        Dim srcNode As Integer = packet.GetInt32
        Dim dstNode As Integer = packet.GetInt32

        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACTIVATETAXI [taxiGUID={2:X} srcNode={3} dstNode={4}]", client.IP, client.Port, guid, srcNode, dstNode)

        If WORLD_CREATUREs.ContainsKey(guid) = False OrElse (WORLD_CREATUREs(guid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOVENDORNEARBY)
            Exit Sub
        End If
        If Not client.Character.LogoutTimer Is Nothing Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERBUSY)
            Exit Sub
        End If
        If (client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_DISABLE_MOVE) Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOTSTANDING)
            Exit Sub
        End If
        If client.Character.ShapeshiftForm > 0 AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BERSERKERSTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BATTLESTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_DEFENSIVESTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_SHADOW Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERSHAPESHIFTED)
            Exit Sub
        End If
        If client.Character.Mount <> 0 Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERALREADYMOUNTED)
            Exit Sub
        End If

        If TaxiNodes.ContainsKey(srcNode) = False OrElse TaxiNodes.ContainsKey(dstNode) = False Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
            Exit Sub
        End If

        Dim mount As Integer '= 0
        If client.Character.IsHorde Then
            If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).HordeMount) = False Then
                Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).HordeMount)
                mount = tmpCr.GetFirstModel
            Else
                mount = CREATURESDatabase(TaxiNodes(srcNode).HordeMount).ModelA1
            End If
        Else
            If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).AllianceMount) = False Then
                Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).AllianceMount)
                mount = tmpCr.GetFirstModel
            Else
                mount = CREATURESDatabase(TaxiNodes(srcNode).AllianceMount).ModelA2
            End If
        End If
        If mount = 0 Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIUNSPECIFIEDSERVERERROR)
            Exit Sub
        End If

        'DONE: Reputation discount
        Dim discountMod As Single
        discountMod = client.Character.GetDiscountMod(WORLD_CREATUREs(guid).Faction)
        Dim totalCost As Integer
        For Each taxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
            If taxiPath.Value.TFrom = srcNode AndAlso taxiPath.Value.TTo = dstNode Then
                totalCost += taxiPath.Value.Price * discountMod
                Exit For
            End If
        Next

        'DONE: Check if we have enough money
        If client.Character.Copper < totalCost Then
            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
            Exit Sub
        End If
        client.Character.Copper -= totalCost

        client.Character.TaxiNodes.Clear()
        client.Character.TaxiNodes.Enqueue(srcNode)
        client.Character.TaxiNodes.Enqueue(dstNode)

        SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIOK)

        'DONE: Mount up, disable move and spell casting
        TaxiTake(client.Character, mount)
        TaxiMove(client.Character, discountMod)
    End Sub

    ''' <summary>
    ''' Called when [CMSG_ACTIVATETAXI_FAR] is received.
    ''' </summary>
    ''' <param name="packet">The packet.</param>
    ''' <param name="client">The client.</param>
    ''' <returns></returns>
    Public Sub On_CMSG_ACTIVATETAXI_FAR(ByRef packet As PacketClass, ByRef client As ClientClass)
        If (packet.Data.Length - 1) < 21 Then Exit Sub
        packet.GetInt16()
        Try
            Dim guid As ULong
            guid = packet.GetUInt64
            Dim totalCost As Integer
            totalCost = packet.GetInt32
            Dim nodeCount As Integer
            nodeCount = packet.GetInt32
            If nodeCount <= 0 Then Exit Sub
            If (packet.Data.Length - 1) < (21 + (4 * nodeCount)) Then Exit Sub

            Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_ACTIVATETAXI_FAR [taxiGUID={2:X} TotalCost={3} NodeCount={4}]", client.IP, client.Port, guid, totalCost, nodeCount)

            If WORLD_CREATUREs.ContainsKey(guid) = False OrElse (WORLD_CREATUREs(guid).CreatureInfo.cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TAXIVENDOR) = 0 Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOVENDORNEARBY)
                Exit Sub
            End If
            If Not client.Character.LogoutTimer Is Nothing Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERBUSY)
                Exit Sub
            End If
            If (client.Character.cUnitFlags And UnitFlags.UNIT_FLAG_DISABLE_MOVE) Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOTSTANDING)
                Exit Sub
            End If
            If client.Character.ShapeshiftForm > 0 AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BERSERKERSTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_BATTLESTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_DEFENSIVESTANCE AndAlso client.Character.ShapeshiftForm <> ShapeshiftForm.FORM_SHADOW Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERSHAPESHIFTED)
                Exit Sub
            End If
            If client.Character.Mount <> 0 Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIPLAYERALREADYMOUNTED)
                Exit Sub
            End If
            If nodeCount < 1 Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
                Exit Sub
            End If
            If client.Character.Copper < totalCost Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
                Exit Sub
            End If

            'DONE: Load nodes
            Dim nodes As New List(Of Integer)
            For i As Integer = 0 To nodeCount - 1
                nodes.Add(packet.GetInt32)
            Next
            Dim srcNode As Integer = nodes(0)
            Dim dstNode As Integer = nodes(1)

            For Each node As Integer In client.Character.TaxiNodes
                If Not TaxiNodes.ContainsKey(node) Then
                    SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOSUCHPATH)
                    Exit Sub
                End If
            Next

            Dim mount As Integer '= 0
            If client.Character.IsHorde Then
                If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).HordeMount) = False Then
                    'TODO: This was here for a reason, i'm guessing to correct the line below.but it is never used
                    Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).HordeMount)
                    mount = tmpCr.GetFirstModel
                Else
                    mount = CREATURESDatabase(TaxiNodes(srcNode).HordeMount).GetFirstModel
                End If
            Else
                If CREATURESDatabase.ContainsKey(TaxiNodes(srcNode).AllianceMount) = False Then
                    'TODO: This was here for a reason, i'm guessing to correct the line below.but it is never used
                    Dim tmpCr As CreatureInfo = New CreatureInfo(TaxiNodes(srcNode).AllianceMount)
                    mount = tmpCr.GetFirstModel
                Else
                    mount = CREATURESDatabase(TaxiNodes(srcNode).AllianceMount).GetFirstModel
                End If
            End If
            If mount = 0 Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIUNSPECIFIEDSERVERERROR)
                Exit Sub
            End If

            'DONE: Reputation discount
            Dim discountMod As Single = client.Character.GetDiscountMod(WORLD_CREATUREs(guid).Faction)
            totalCost = 0
            For Each taxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
                If taxiPath.Value.TFrom = srcNode AndAlso taxiPath.Value.TTo = dstNode Then
                    totalCost += taxiPath.Value.Price * discountMod
                    Exit For
                End If
            Next

            'DONE: Check if we have enough money
            If client.Character.Copper < totalCost Then
                SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXINOTENOUGHMONEY)
                Exit Sub
            End If
            client.Character.Copper -= totalCost

            client.Character.TaxiNodes.Clear()
            For Each node As Integer In nodes
                client.Character.TaxiNodes.Enqueue(node)
            Next

            SendActivateTaxiReply(client, ActivateTaxiReplies.ERR_TAXIOK)

            'DONE: Mount up, disable move and spell casting
            TaxiTake(client.Character, mount)
            TaxiMove(client.Character, discountMod)

        Catch e As Exception
            Log.WriteLine(LogType.CRITICAL, "Error when taking a long taxi.{0}", vbNewLine & e.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Called when [CMSG_MOVE_SPLINE_DONE] is received.
    ''' </summary>
    ''' <param name="packet">The packet.</param>
    ''' <param name="Client">The client.</param>
    ''' <returns></returns>
    Public Sub On_CMSG_MOVE_SPLINE_DONE(ByRef packet As PacketClass, ByRef client As ClientClass)
        Log.WriteLine(LogType.DEBUG, "[{0}:{1}] CMSG_MOVE_SPLINE_DONE", client.IP, client.Port)
    End Sub

    ''' <summary>
    ''' Lands the Taxi.
    ''' </summary>
    ''' <param name="character">The character.</param>
    ''' <returns></returns>
    Private Sub TaxiLand(ByVal character As CharacterObject)
        character.TaxiNodes.Clear()
        character.Mount = 0
        character.cUnitFlags = character.cUnitFlags And (Not UnitFlags.UNIT_FLAG_DISABLE_MOVE)
        character.cUnitFlags = character.cUnitFlags And (Not UnitFlags.UNIT_FLAG_TAXI_FLIGHT)
        character.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, character.Mount)
        character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, character.cUnitFlags)
        character.SendCharacterUpdate()
    End Sub

    ''' <summary>
    ''' Takes the Taxi.
    ''' </summary>
    ''' <param name="character">The character.</param>
    ''' <param name="mount">The mount.</param>
    ''' <returns></returns>
    Private Sub TaxiTake(ByVal character As CharacterObject, ByVal mount As Integer)
        character.Mount = mount
        character.cUnitFlags = character.cUnitFlags Or UnitFlags.UNIT_FLAG_DISABLE_MOVE
        character.cUnitFlags = character.cUnitFlags Or UnitFlags.UNIT_FLAG_TAXI_FLIGHT
        character.SetUpdateFlag(EUnitFields.UNIT_FIELD_MOUNTDISPLAYID, character.Mount)
        character.SetUpdateFlag(EUnitFields.UNIT_FIELD_FLAGS, character.cUnitFlags)
        character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, character.Copper)
        character.SendCharacterUpdate()
    End Sub

    ''' <summary>
    ''' Moves the Taxi.
    ''' </summary>
    ''' <param name="character">The character.</param>
    ''' <param name="discountMod">The discount mod.</param>
    ''' <returns></returns>
    Private Sub TaxiMove(ByVal character As CharacterObject, ByVal discountMod As Single)
        Dim flagFirstNode As Boolean
        flagFirstNode = True
        Dim lastX As Single
        Dim lastY As Single
        Dim lastZ As Single
        Dim moveDistance As Single
        Dim totalDistance As Single

        Dim waypointPaths As New List(Of Integer)
        Dim waypointNodes As New Dictionary(Of Integer, TTaxiPathNode)

        Try
            'DONE: Generate paths
            Dim srcNode As Integer
            Dim dstNode As Integer = character.TaxiNodes.Dequeue
            While character.TaxiNodes.Count > 0
                srcNode = dstNode
                dstNode = character.TaxiNodes.Dequeue

                For Each taxiPath As KeyValuePair(Of Integer, TTaxiPath) In TaxiPaths
                    If taxiPath.Value.TFrom = srcNode AndAlso taxiPath.Value.TTo = dstNode Then
                        waypointPaths.Add(taxiPath.Key)
                        Exit For
                    End If
                Next
            End While

            'DONE: Do move on paths
            For Each path As Integer In waypointPaths
                If flagFirstNode Then
                    'DONE: Don't tax first node, it is already taxed
                    flagFirstNode = False
                Else
                    'DONE: Remove the money for this flight
                    Dim price As Integer = TaxiPaths(path).Price * discountMod
                    If character.Copper < price Then Exit For
                    character.Copper -= price
                    character.SetUpdateFlag(EPlayerFields.PLAYER_FIELD_COINAGE, character.Copper)
                    character.SendCharacterUpdate(False)
                    Console.WriteLine("Paying {0}", price)
                End If

                lastX = character.positionX
                lastY = character.positionY
                lastZ = character.positionZ
                totalDistance = 0

                'DONE: Get the waypoints
                waypointNodes.Clear()
                For Each taxiPathNode As KeyValuePair(Of Integer, TTaxiPathNode) In TaxiPathNodes(path)
                    waypointNodes.Add(taxiPathNode.Value.Seq, taxiPathNode.Value)

                    totalDistance += GetDistance(lastX, taxiPathNode.Value.x, lastY, taxiPathNode.Value.y, lastZ, taxiPathNode.Value.z)
                    lastX = taxiPathNode.Value.x
                    lastY = taxiPathNode.Value.y
                    lastZ = taxiPathNode.Value.z
                Next

                lastX = character.positionX
                lastY = character.positionY
                lastZ = character.positionZ

                'Send move packet for player
                Dim SMSG_MONSTER_MOVE As New PacketClass(OPCODES.SMSG_MONSTER_MOVE)
                Try
                    SMSG_MONSTER_MOVE.AddPackGUID(character.GUID)
                    SMSG_MONSTER_MOVE.AddSingle(character.positionX)
                    SMSG_MONSTER_MOVE.AddSingle(character.positionY)
                    SMSG_MONSTER_MOVE.AddSingle(character.positionZ)
                    SMSG_MONSTER_MOVE.AddInt32(timeGetTime(""))
                    SMSG_MONSTER_MOVE.AddInt8(0)
                    SMSG_MONSTER_MOVE.AddInt32(&H300)                           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                    SMSG_MONSTER_MOVE.AddInt32(Fix(totalDistance / UNIT_NORMAL_TAXI_SPEED * 1000))   'Time
                    SMSG_MONSTER_MOVE.AddInt32(waypointNodes.Count)             'Points Count
                    For j As Integer = 0 To waypointNodes.Count - 1
                        SMSG_MONSTER_MOVE.AddSingle(waypointNodes(j).x)         'First Point X
                        SMSG_MONSTER_MOVE.AddSingle(waypointNodes(j).y)         'First Point Y
                        SMSG_MONSTER_MOVE.AddSingle(waypointNodes(j).z)         'First Point Z
                    Next
                    character.Client.Send(SMSG_MONSTER_MOVE)
                Finally
                    SMSG_MONSTER_MOVE.Dispose()
                End Try

                For i As Integer = 0 To waypointNodes.Count - 1
                    moveDistance = GetDistance(lastX, waypointNodes(i).x, lastY, waypointNodes(i).y, lastZ, waypointNodes(i).z)

                    lastX = waypointNodes(i).x
                    lastY = waypointNodes(i).y
                    lastZ = waypointNodes(i).z

                    'Send move packet for other players
                    Dim p As New PacketClass(OPCODES.SMSG_MONSTER_MOVE)
                    Try
                        p.AddPackGUID(character.GUID)
                        p.AddSingle(character.positionX)
                        p.AddSingle(character.positionY)
                        p.AddSingle(character.positionZ)
                        p.AddInt32(timeGetTime(""))
                        p.AddInt8(0)
                        p.AddInt32(&H300)                           'Flags [0x0 - Walk, 0x100 - Run, 0x200 - Waypoint, 0x300 - Fly]
                        p.AddInt32(Fix(totalDistance / UNIT_NORMAL_TAXI_SPEED * 1000))   'Time
                        p.AddInt32(waypointNodes.Count)             'Points Count
                        For j As Integer = i To waypointNodes.Count - 1
                            p.AddSingle(waypointNodes(j).x)         'First Point X
                            p.AddSingle(waypointNodes(j).y)         'First Point Y
                            p.AddSingle(waypointNodes(j).z)         'First Point Z
                        Next
                        character.SendToNearPlayers(p, , False)
                    Finally
                        p.Dispose()
                    End Try

                    'Wait move to complete
                    Thread.Sleep(Fix(moveDistance / UNIT_NORMAL_TAXI_SPEED * 1000))

                    'Update character postion
                    totalDistance -= moveDistance
                    character.positionX = lastX
                    character.positionY = lastY
                    character.positionZ = lastZ
                    MoveCell(character)
                    UpdateCell(character)
                Next

            Next

        Catch ex As Exception
            Log.WriteLine(LogType.FAILED, "Error on flight: {0}", ex.ToString)
        End Try

        character.Save()
        TaxiLand(character)
    End Sub

End Module