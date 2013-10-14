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
Imports mangosVB.Common
Imports mangosVB.Common.NativeMethods

Public Module WS_Pets

#Region "WS.Pets.Framework"

    Public Const CREATURE_MAX_SPELLS As Integer = 4
    Public Const MAX_OWNER_DIS As Integer = 100

    Public LevelUpLoyalty(6) As Integer
    Public LevelStartLoyalty(6) As Integer

    Public Sub InitializeLevelUpLoyalty()
        WS_Pets.LevelUpLoyalty(0) = 0
        WS_Pets.LevelUpLoyalty(1) = 5500
        WS_Pets.LevelUpLoyalty(2) = 11500
        WS_Pets.LevelUpLoyalty(3) = 17000
        WS_Pets.LevelUpLoyalty(4) = 23500
        WS_Pets.LevelUpLoyalty(5) = 31000
        WS_Pets.LevelUpLoyalty(6) = 39500
    End Sub

    Public Sub InitializeLevelStartLoyalty()
        WS_Pets.LevelStartLoyalty(0) = 0
        WS_Pets.LevelStartLoyalty(1) = 2000
        WS_Pets.LevelStartLoyalty(2) = 4500
        WS_Pets.LevelStartLoyalty(3) = 7000
        WS_Pets.LevelStartLoyalty(4) = 10000
        WS_Pets.LevelStartLoyalty(5) = 13500
        WS_Pets.LevelStartLoyalty(6) = 17500
    End Sub

    Public Enum PetType As Byte
        SUMMON_PET = 0
        HUNTER_PET = 1
        GUARDIAN_PET = 2
        MINI_PET = 3
    End Enum

    Public Enum PetSaveType As Integer
        PET_SAVE_DELETED = -1
        PET_SAVE_CURRENT = 0
        PET_SAVE_IN_STABLE_1 = 1
        PET_SAVE_IN_STABLE_2 = 2
        PET_SAVE_NO_SLOT = 3
    End Enum

    Public Enum HappinessState As Byte
        UNHAPPY = 1
        CONTENT = 2
        HAPPY = 3
    End Enum

    Public Enum LoyaltyState As Byte
        REBELLIOUS = 1
        UNRULY = 2
        SUBMISSIVE = 3
        DEPENDABLE = 4
        FAITHFUL = 5
        BEST_FRIEND = 6
    End Enum

    Public Enum PetSpellState As Byte
        SPELL_UNCHANGED = 0
        SPELL_CHANGED = 1
        SPELL_NEW = 2
        SPELL_REMOVED = 3
    End Enum

    Public Enum ActionFeedback As Byte
        FEEDBACK_NONE = 0
        FEEDBACK_PET_DEAD = 1
        FEEDBACK_NO_TARGET = 2
        FEEDBACK_CANT_ATT = 3
    End Enum

    Public Enum PetTalk As Byte
        PET_TALK_SPECIAL_SPELL = 0
        PET_TALK_ATTACK = 1
    End Enum


#End Region
#Region "WS.Pets.TypeDef"

    Public Class PetObject
        Inherits CreatureObject

        Public PetName As String = ""
        Public Renamed As Boolean = False

        Public Owner As BaseUnit = Nothing

        Public FollowOwner As Boolean = True

        Public Command As Byte = 7
        Public State As Byte = 6
        Public Spells As ArrayList

        Public XP As Integer = 0

        Public Sub New(ByVal GUID_ As ULong, ByVal CreatureID As Integer)
            MyBase.New(GUID_, CreatureID)

        End Sub

        Public Sub Spawn()
            Me.AddToWorld()

            If TypeOf Owner Is CharacterObject Then
                CType(Owner, CharacterObject).GroupUpdateFlag = CType(Owner, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_PET
            End If

            SendPetInitialize(Owner, Me)
        End Sub

        Public Sub Hide()
            Me.RemoveFromWorld()

            If Not TypeOf Owner Is CharacterObject Then Exit Sub

            CType(Owner, CharacterObject).GroupUpdateFlag = CType(Owner, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_PET

            Dim packet As New PacketClass(OPCODES.SMSG_PET_SPELLS)
            packet.AddUInt64(0)
            CType(Owner, CharacterObject).Client.Send(packet)
            packet.Dispose()
        End Sub

    End Class

#End Region
#Region "WS.Pets.Handlers"

    Public Sub On_CMSG_PET_NAME_QUERY(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim PetNumber As Integer = packet.GetInt32()
        Dim PetGUID As ULong = packet.GetUInt64()

        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_NAME_QUERY [Number={0} GUID={1:X}", PetNumber, PetGUID)

        SendPetNameQuery(Client, PetGUID, PetNumber)
    End Sub

    Public Sub On_CMSG_REQUEST_PET_INFO(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_REQUEST_PET_INFO")

        DumpPacket(packet.Data, Client, 6)
    End Sub

    Public Sub On_CMSG_PET_ACTION(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim PetGUID As ULong = packet.GetUInt64()
        Dim SpellID As UShort = packet.GetUInt16()
        Dim SpellFlag As UShort = packet.GetUInt16()
        Dim TargetGUID As ULong = packet.GetUInt64()

        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_ACTION [GUID={0:X} Spell={1} Flag={2:X} Target={3:X}]", PetGUID, SpellID, SpellFlag, TargetGUID)
    End Sub

    Public Sub On_CMSG_PET_CANCEL_AURA(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_CANCEL_AURA")

        DumpPacket(packet.Data, Client, 6)
    End Sub

    Public Sub On_CMSG_PET_ABANDON(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim PetGUID As ULong = packet.GetUInt64()

        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_ABANDON [GUID={0:X}]", PetGUID)
    End Sub

    Public Sub On_CMSG_PET_RENAME(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim PetGUID As ULong = packet.GetUInt64()
        Dim PetName As String = packet.GetString()

        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_RENAME [GUID={0:X} Name={1}]", PetGUID, PetName)
    End Sub

    Public Sub On_CMSG_PET_SET_ACTION(ByRef packet As PacketClass, ByRef Client As ClientClass)
        packet.GetInt16()
        Dim PetGUID As ULong = packet.GetUInt64()
        Dim Position As Integer = packet.GetInt32()
        Dim SpellID As UShort = packet.GetUInt16()
        Dim ActionState As Short = packet.GetInt16()

        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_SET_ACTION [GUID={0:X} Pos={1} Spell={2} Action={3}]", PetGUID, Position, SpellID, ActionState)
    End Sub

    Public Sub On_CMSG_PET_SPELL_AUTOCAST(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_SPELL_AUTOCAST")

        DumpPacket(packet.Data, Client, 6)
    End Sub

    Public Sub On_CMSG_PET_STOP_ATTACK(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_STOP_ATTACK")

        DumpPacket(packet.Data, Client, 6)
    End Sub

    Public Sub On_CMSG_PET_UNLEARN(ByRef packet As PacketClass, ByRef Client As ClientClass)
        Log.WriteLine(BaseWriter.LogType.DEBUG, "CMSG_PET_UNLEARN")

        DumpPacket(packet.Data, Client, 6)
    End Sub

    Public Sub SendPetNameQuery(ByRef Client As ClientClass, ByVal PetGUID As ULong, ByVal PetNumber As Integer)
        If WORLD_CREATUREs.ContainsKey(PetGUID) = False Then Exit Sub
        If Not TypeOf WORLD_CREATUREs(PetGUID) Is PetObject Then Exit Sub

        Dim response As New PacketClass(OPCODES.SMSG_PET_NAME_QUERY_RESPONSE)
        response.AddInt32(PetNumber)
        response.AddString(CType(WORLD_CREATUREs(PetGUID), PetObject).PetName) 'Pet name
        response.AddInt32(timeGetTime) 'Pet name timestamp
        Client.Send(response)
        response.Dispose()
    End Sub

#End Region
#Region "WS.Pets.AI"

    'TODO: Fix the pet AI
    Public Class PetAI
        Inherits DefaultAI

        Public Sub New(ByRef Creature As CreatureObject)
            MyBase.New(Creature)
            AllowedMove = False
        End Sub

    End Class

#End Region
#Region "WS.Pets.Owner"
    Public Sub LoadPet(ByRef c As CharacterObject)
        If c.Pet IsNot Nothing Then Exit Sub

        Dim PetQuery As New DataTable
        CharacterDatabase.Query(String.Format("SELECT * FROM characters_pets WHERE pet_owner = '{0}';", c.GUID), PetQuery)
        If PetQuery.Rows.Count = 0 Then Exit Sub
        Dim PetInfo As DataRow = PetQuery.Rows(0)

        c.Pet = New PetObject(CULng(PetInfo.Item("pet_guid")) + GUID_PET, CInt(PetInfo.Item("pet_entry")))
        c.Pet.Owner = c
        c.Pet.SummonedBy = c.GUID
        c.Pet.CreatedBy = c.GUID
        c.Pet.Level = PetInfo.Item("pet_level")
        c.Pet.XP = PetInfo.Item("pet_xp")

        c.Pet.PetName = PetInfo.Item("pet_name")
        If CByte(PetInfo.Item("pet_renamed")) = 0 Then
            c.Pet.Renamed = False
        Else
            c.Pet.Renamed = True
        End If

        c.Pet.Faction = c.Faction

        c.Pet.positionX = c.positionX
        c.Pet.positionY = c.positionY
        c.Pet.positionZ = c.positionZ
        c.Pet.MapID = c.MapID

        Log.WriteLine(BaseWriter.LogType.DEBUG, "Loaded pet [{0}] for character [{1}].", c.Pet.GUID, c.GUID)
    End Sub

    Public Sub SendPetInitialize(ByRef Caster As CharacterObject, ByRef Pet As BaseUnit)
        If TypeOf Pet Is CreatureObject Then
            'TODO: Get spells
        ElseIf TypeOf Pet Is CharacterObject Then
            'TODO: Get spells. How do we know which spells to take?
        End If

        Dim Command As UShort = 7
        Dim State As UShort = 6
        Dim AddList As Byte = 0

        If TypeOf Pet Is PetObject Then
            Command = CType(Pet, PetObject).Command
            State = CType(Pet, PetObject).State
        End If

        Dim packet As New PacketClass(OPCODES.SMSG_PET_SPELLS)
        packet.AddUInt64(Pet.GUID)
        packet.AddInt32(0)
        packet.AddInt32(&H1010000)
        packet.AddInt16(2)
        packet.AddInt16(Command << 8)
        packet.AddInt16(1)
        packet.AddInt16(Command << 8)
        packet.AddInt16(0)
        packet.AddInt16(Command << 8)

        For i As Integer = 0 To 3
            packet.AddInt16(0) 'SpellID?
            packet.AddInt16(0) '0xC100?
        Next

        packet.AddInt16(2)
        packet.AddInt16(State << 8)
        packet.AddInt16(1)
        packet.AddInt16(State << 8)
        packet.AddInt16(0)
        packet.AddInt16(State << 8)

        packet.AddInt8(AddList)

        'Something based on AddList

        packet.AddInt8(1)
        packet.AddInt32(&H6010)
        packet.AddInt32(0)
        packet.AddInt32(0)
        packet.AddInt16(0)

        Caster.Client.Send(packet)
        packet.Dispose()
    End Sub
#End Region

End Module

