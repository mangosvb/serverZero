'
' Copyright (objCharacter) 2013 getMaNGOS <http://www.getMangos.co.uk>
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
Imports mangosVB.Common
Imports mangosVB.Common.BaseWriter


Public Module WC_Group


    Public Const GROUP_SUBGROUPSIZE As Integer = 5
    Public Const GROUP_SIZE As Integer = 4
    Public Const GROUP_RAIDSIZE As Integer = 39

    'Used as counter for unique Group.ID
    Private GroupCounter As Long = 1

    Public GROUPs As New Dictionary(Of Long, Group)
    Public Class Group
        Implements IDisposable


        Public ID As Long
        Public Type As GroupType = GroupType.PARTY
        Public DungeonDifficulty As GroupDungeonDifficulty = GroupDungeonDifficulty.DIFFICULTY_NORMAL
        Public LootMaster As Byte
        Public LootMethod As GroupLootMethod = GroupLootMethod.LOOT_GROUP
        Public LootThreshold As GroupLootThreshold = GroupLootThreshold.Uncommon

        Public Leader As Byte
        Public Members(GROUP_SIZE) As CharacterObject
        Public TargetIcons(7) As ULong


        Public Sub New(ByRef objCharacter As CharacterObject)
            ID = Interlocked.Increment(GroupCounter)
            GROUPs.Add(ID, Me)

            Members(0) = objCharacter
            Members(1) = Nothing
            Members(2) = Nothing
            Members(3) = Nothing
            Members(4) = Nothing

            Leader = 0
            LootMaster = 255

            objCharacter.Group = Me
            objCharacter.GroupAssistant = False

            objCharacter.GetWorld.ClientSetGroup(objCharacter.Client.Index, ID)
        End Sub


#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Dim packet As PacketClass

                If Type = GroupType.RAID Then
                    packet = New PacketClass(OPCODES.SMSG_GROUP_LIST)
                    packet.AddInt16(0)          'GroupType 0:Party 1:Raid
                    packet.AddInt32(0)          'GroupCount
                Else
                    packet = New PacketClass(OPCODES.SMSG_GROUP_DESTROYED)
                End If

                For i As Byte = 0 To Members.Length - 1
                    If Not Members(i) Is Nothing Then
                        Members(i).Group = Nothing
                        If Not Members(i).Client Is Nothing Then
                            Members(i).Client.SendMultiplyPackets(packet)
                            Members(i).GetWorld.ClientSetGroup(Members(i).Client.Index, -1)
                        End If
                        Members(i) = Nothing
                    End If
                Next
                packet.Dispose()

                WorldServer.GroupSendUpdate(ID)
                GROUPs.Remove(ID)
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

        Public Sub Join(ByRef objCharacter As CharacterObject)
            For i As Byte = 0 To Members.Length - 1
                If Members(i) Is Nothing Then
                    Members(i) = objCharacter
                    objCharacter.Group = Me
                    objCharacter.GroupAssistant = False
                    Exit For
                End If
            Next

            WorldServer.GroupSendUpdate(ID)

            objCharacter.GetWorld.ClientSetGroup(objCharacter.Client.Index, ID)

            SendGroupList()
        End Sub
        Public Sub Leave(ByRef objCharacter As CharacterObject)
            If GetMembersCount() = 2 Then
                Dispose()
                Exit Sub
            End If

            For i As Byte = 0 To Members.Length - 1
                If Members(i) Is objCharacter Then

                    objCharacter.Group = Nothing
                    Members(i) = Nothing

                    'DONE: If current is leader then choose new
                    If i = Leader Then
                        NewLeader()
                    End If

                    'DONE: If current is lootMaster then choose new
                    If i = LootMaster Then LootMaster = Leader

                    If objCharacter.Client IsNot Nothing Then
                        Dim packet As New PacketClass(OPCODES.SMSG_GROUP_UNINVITE)
                        objCharacter.Client.Send(packet)
                        packet.Dispose()
                    End If

                    Exit For
                End If
            Next

            WorldServer.GroupSendUpdate(ID)

            objCharacter.GetWorld.ClientSetGroup(objCharacter.Client.Index, -1)

            CheckMembers()
        End Sub
        Public Sub CheckMembers()
            If GetMembersCount() < 2 Then Dispose() Else SendGroupList()
        End Sub
        Public Sub NewLeader(Optional ByVal Leaver As CharacterObject = Nothing)
            Dim ChosenMember As Byte = 255
            Dim NewLootMaster As Boolean = False
            For i As Byte = 0 To Members.Length - 1
                If Members(i) IsNot Nothing AndAlso Members(i).Client IsNot Nothing Then
                    If Leaver IsNot Nothing AndAlso Leaver Is Members(i) Then
                        If i = LootMaster Then NewLootMaster = True
                        If ChosenMember <> 255 Then Exit For
                    Else
                        If Members(i).GroupAssistant AndAlso ChosenMember = 255 Then
                            ChosenMember = i
                        ElseIf ChosenMember = 255 Then
                            ChosenMember = i
                        End If
                    End If
                End If
            Next

            If ChosenMember <> 255 Then
                Leader = ChosenMember
                If NewLootMaster Then LootMaster = Leader
                Dim response As New PacketClass(OPCODES.SMSG_GROUP_SET_LEADER)
                response.AddString(Members(Leader).Name)
                Broadcast(response)
                response.Dispose()

                WorldServer.GroupSendUpdate(ID)
            End If
        End Sub

        Public ReadOnly Property IsFull() As Boolean
            Get
                For i As Byte = 0 To Members.Length - 1
                    If Members(i) Is Nothing Then Return False
                Next
                Return True
            End Get
        End Property


        Public Sub ConvertToRaid()
            ReDim Preserve Members(GROUP_RAIDSIZE)
            For i As Byte = GROUP_SIZE + 1 To GROUP_RAIDSIZE
                Members(i) = Nothing
            Next

            Type = GroupType.RAID
        End Sub

        Public Sub SetLeader(ByRef objCharacter As CharacterObject)
            For i As Byte = 0 To Members.Length
                If Members(i) Is objCharacter Then
                    Leader = i
                    Exit For
                End If
            Next

            Dim packet As New PacketClass(OPCODES.SMSG_GROUP_SET_LEADER)
            packet.AddString(objCharacter.Name)
            Broadcast(packet)
            packet.Dispose()

            WorldServer.GroupSendUpdate(ID)

            SendGroupList()
        End Sub
        Public Sub SetLootMaster(ByRef objCharacter As CharacterObject)
            LootMaster = Leader
            For i As Byte = 0 To Members.Length - 1
                If Members(i) Is objCharacter Then
                    LootMaster = i
                    Exit For
                End If
            Next i
            SendGroupList()
        End Sub
        Public Sub SetLootMaster(ByRef GUID As ULong)
            LootMaster = 255
            For i As Byte = 0 To Members.Length - 1
                If (Not Members(i) Is Nothing) AndAlso (Members(i).GUID = GUID) Then
                    LootMaster = i
                    Exit For
                End If
            Next i
            SendGroupList()
        End Sub
        Public Function GetLeader() As CharacterObject
            Return Members(Leader)
        End Function
        Public Function GetLootMaster() As CharacterObject
            Return Members(Leader)
        End Function
        Public Function GetMembersCount() As Byte
            Dim count As Byte = 0
            For i As Byte = 0 To Members.Length - 1
                If Not Members(i) Is Nothing Then count += 1
            Next i
            Return count
        End Function
        Public Function GetMembers() As ULong()
            Dim list As New List(Of ULong)
            For i As Byte = 0 To Members.Length - 1
                If Not Members(i) Is Nothing Then list.Add(Members(i).GUID)
            Next i

            Return list.ToArray
        End Function



        Public Sub Broadcast(ByRef packet As PacketClass)
            For i As Byte = 0 To Members.Length - 1
                If Members(i) IsNot Nothing AndAlso Members(i).Client IsNot Nothing Then Members(i).Client.SendMultiplyPackets(packet)
            Next
        End Sub
        Public Sub BroadcastToOther(ByRef packet As PacketClass, ByRef objCharacter As CharacterObject)
            For i As Byte = 0 To Members.Length - 1
                If (Not Members(i) Is Nothing) AndAlso (Members(i) IsNot objCharacter) AndAlso (Members(i).Client IsNot Nothing) Then Members(i).Client.SendMultiplyPackets(packet)
            Next
        End Sub
        Public Sub BroadcastToOutOfRange(ByRef packet As PacketClass, ByRef objCharacter As CharacterObject)
            For i As Byte = 0 To Members.Length - 1
                If Members(i) IsNot Nothing AndAlso Members(i) IsNot objCharacter AndAlso Members(i).Client IsNot Nothing Then
                    If objCharacter.Map <> Members(i).Map OrElse Math.Sqrt((objCharacter.PositionX - Members(i).PositionX) ^ 2 + (objCharacter.PositionY - Members(i).PositionY) ^ 2) > DEFAULT_DISTANCE_VISIBLE Then
                        Members(i).Client.SendMultiplyPackets(packet)
                    End If
                End If
            Next
        End Sub


        Public Sub SendGroupList()
            Dim GroupCount As Byte = GetMembersCount()

            For i As Byte = 0 To Members.Length - 1
                If Not Members(i) Is Nothing Then

                    Dim packet As New PacketClass(OPCODES.SMSG_GROUP_LIST)
                    packet.AddInt8(Type)                                    'GroupType 0:Party 1:Raid
                    Dim MemberFlags As Byte = Int(i / GROUP_SUBGROUPSIZE)
                    'If Members(i).GroupAssistant Then MemberFlags = MemberFlags Or &H1
                    packet.AddInt8(MemberFlags)
                    packet.AddInt32(GroupCount - 1)

                    For j As Byte = 0 To Members.Length - 1
                        If (Not Members(j) Is Nothing) AndAlso (Not Members(j) Is Members(i)) Then
                            packet.AddString(Members(j).Name)
                            packet.AddUInt64(Members(j).GUID)
                            If Members(j).IsInWorld Then
                                packet.AddInt8(1)                           'CharOnline?
                            Else
                                packet.AddInt8(0)                           'CharOnline?
                            End If
                            MemberFlags = Int(j / GROUP_SUBGROUPSIZE)
                            'If Members(j).GroupAssistant Then MemberFlags = MemberFlags Or &H1
                            packet.AddInt8(MemberFlags)
                        End If
                    Next
                    packet.AddUInt64(Members(Leader).GUID)
                    packet.AddInt8(LootMethod)
                    If LootMaster <> 255 Then packet.AddUInt64(Members(LootMaster).GUID) Else packet.AddUInt64(0)
                    packet.AddInt8(LootThreshold)
                    packet.AddInt16(0)

                    If Members(i).Client IsNot Nothing Then
                        Members(i).Client.Send(packet)
                    End If
                    packet.Dispose()
                End If
            Next
        End Sub
        Public Sub SendChatMessage(ByRef Sender As CharacterObject, ByVal Message As String, ByVal Language As LANGUAGES, ByVal Type As ChatMsg)
            Dim packet As PacketClass = BuildChatMessage(Sender.GUID, Message, Type, Language, Sender.ChatFlag)

            Broadcast(packet)
            packet.Dispose()
        End Sub

    End Class




End Module
