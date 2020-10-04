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

Imports Mangos.Common
Imports Mangos.Common.Globals
Imports Mangos.Shared

Public Module WS_Group
    Public ReadOnly Groups As New Dictionary(Of Long, Group)
    Private _lastLooter As ULong = 0

    Public NotInheritable Class Group
        Implements IDisposable

        Public ReadOnly ID As Long
        Public Type As GroupType = GroupType.PARTY
        Public DungeonDifficulty As GroupDungeonDifficulty = GroupDungeonDifficulty.DIFFICULTY_NORMAL
        Public LootMethod As GroupLootMethod = GroupLootMethod.LOOT_GROUP
        Public LootThreshold As GroupLootThreshold = GroupLootThreshold.Uncommon
        Public Leader As ULong

        Public LocalMembers As List(Of ULong)
        Public LocalLootMaster As CharacterObject

        Public Sub New(ByVal groupID As Long)
            ID = groupID
            Groups.Add(ID, Me)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Private Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Groups.Remove(ID)
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

        ''' <summary>
        ''' Broadcasts the specified p.
        ''' </summary>
        ''' <param name="p">The p.</param>
        ''' <returns></returns>
        Public Sub Broadcast(ByVal p As PacketClass)
            p.UpdateLength()
            ClsWorldServer.Cluster.BroadcastGroup(ID, p.Data)
        End Sub

        ''' <summary>
        ''' Gets the next looter.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetNextLooter() As CharacterObject
            Dim nextIsLooter As Boolean = False
            Dim nextLooterFound As Boolean = False

            For Each guid As ULong In LocalMembers
                If nextIsLooter Then
                    _lastLooter = guid
                    nextLooterFound = True
                    Exit For
                End If

                If guid = _lastLooter Then nextIsLooter = True
            Next

            If Not nextLooterFound Then
                _lastLooter = LocalMembers.Item(0)
            End If

            Return CHARACTERs(_lastLooter)
        End Function

        ''' <summary>
        ''' Gets the members count.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetMembersCount() As Integer
            Return LocalMembers.Count
        End Function
    End Class

    ''' <summary>
    ''' Builds the party member stats.
    ''' </summary>
    ''' <param name="objCharacter">The objCharacter.</param>
    ''' <param name="flag">The flag.</param>
    ''' <returns></returns>
    Function BuildPartyMemberStats(ByRef objCharacter As CharacterObject, ByVal flag As UInteger) As PacketClass
        Dim opCode As OPCODES = OPCODES.SMSG_PARTY_MEMBER_STATS
        If flag = PartyMemberStatsFlag.GROUP_UPDATE_FULL OrElse flag = PartyMemberStatsFlag.GROUP_UPDATE_FULL_PET Then
            opCode = OPCODES.SMSG_PARTY_MEMBER_STATS_FULL

            If objCharacter.ManaType <> ManaTypes.TYPE_MANA Then flag = flag Or PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POWER_TYPE
        End If

        Dim packet As New PacketClass(opCode)
        packet.AddPackGUID(objCharacter.GUID)
        packet.AddUInt32(flag)

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_STATUS) Then
            Dim memberFlags As Byte = PartyMemberStatsStatus.STATUS_ONLINE
            If objCharacter.isPvP Then memberFlags = memberFlags Or PartyMemberStatsStatus.STATUS_PVP
            If objCharacter.DEAD Then memberFlags = memberFlags Or PartyMemberStatsStatus.STATUS_DEAD
            packet.AddInt8(memberFlags)
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_HP) Then packet.AddUInt16(objCharacter.Life.Current)
        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_HP) Then packet.AddUInt16(objCharacter.Life.Maximum)
        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POWER_TYPE) Then packet.AddInt8(objCharacter.ManaType)
        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_CUR_POWER) Then
            If objCharacter.ManaType = ManaTypes.TYPE_RAGE Then
                packet.AddUInt16(objCharacter.Rage.Current)
            ElseIf objCharacter.ManaType = ManaTypes.TYPE_ENERGY Then
                packet.AddUInt16(objCharacter.Energy.Current)
            Else
                packet.AddUInt16(objCharacter.Mana.Current)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_MAX_POWER) Then
            If objCharacter.ManaType = ManaTypes.TYPE_RAGE Then
                packet.AddUInt16(objCharacter.Rage.Maximum)
            ElseIf objCharacter.ManaType = ManaTypes.TYPE_ENERGY Then
                packet.AddUInt16(objCharacter.Energy.Maximum)
            Else
                packet.AddUInt16(objCharacter.Mana.Maximum)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_LEVEL) Then packet.AddUInt16(objCharacter.Level)
        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_ZONE) Then packet.AddUInt16(objCharacter.ZoneID)
        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_POSITION) Then
            packet.AddInt16(Fix(objCharacter.positionX))
            packet.AddInt16(Fix(objCharacter.positionY))
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_AURAS) Then
            Dim auraMask As ULong = 0
            Dim auraPos As Integer = packet.Data.Length
            packet.AddUInt64(0) 'AuraMask (is set after the loop)
            For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                If Not objCharacter.ActiveSpells(i) Is Nothing Then
                    auraMask = auraMask Or (CULng(1) << CULng(i))
                    packet.AddUInt16(objCharacter.ActiveSpells(i).SpellID)
                    packet.AddInt8(1) 'Stack Count?
                End If
            Next
            packet.AddUInt64(auraMask, auraPos) 'Set the AuraMask
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_GUID) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt64(objCharacter.Pet.GUID)
            Else
                packet.AddInt64(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_NAME) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddString(objCharacter.Pet.PetName)
            Else
                packet.AddString("")
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_MODEL_ID) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt16(objCharacter.Pet.Model)
            Else
                packet.AddInt16(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_CUR_HP) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt16(objCharacter.Pet.Life.Current)
            Else
                packet.AddInt16(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_MAX_HP) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt16(objCharacter.Pet.Life.Maximum)
            Else
                packet.AddInt16(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_POWER_TYPE) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddInt8(ManaTypes.TYPE_FOCUS)
            Else
                packet.AddInt8(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_CUR_POWER) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt16(objCharacter.Pet.Mana.Current)
            Else
                packet.AddInt16(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_MAX_POWER) Then
            If objCharacter.Pet IsNot Nothing Then
                packet.AddUInt16(objCharacter.Pet.Mana.Maximum)
            Else
                packet.AddInt16(0)
            End If
        End If

        If (flag And PartyMemberStatsFlag.GROUP_UPDATE_FLAG_PET_AURAS) Then
            If objCharacter.Pet IsNot Nothing Then
                Dim auraMask As ULong = 0
                Dim auraPos As Integer = packet.Data.Length
                packet.AddUInt64(0) 'AuraMask (is set after the loop)
                For i As Integer = 0 To MAX_AURA_EFFECTs_VISIBLE - 1
                    If Not objCharacter.Pet.ActiveSpells(i) Is Nothing Then
                        auraMask = auraMask Or (CULng(1) << CULng(i))
                        packet.AddUInt16(objCharacter.Pet.ActiveSpells(i).SpellID)
                        packet.AddInt8(1) 'Stack Count?
                    End If
                Next
                packet.AddUInt64(auraMask, auraPos) 'Set the AuraMask
            Else
                packet.AddInt64(0)
            End If
        End If

        Return packet
    End Function
End Module