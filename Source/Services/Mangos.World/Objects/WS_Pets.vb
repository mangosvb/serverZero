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

Imports System.Data
Imports Mangos.Common
Imports Mangos.Common.Enums
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Globals
Imports Mangos.World.AI
Imports Mangos.World.Globals
Imports Mangos.World.Player
Imports Mangos.World.Server

Namespace Objects

    Public Module WS_Pets

#Region "WS.Pets.Framework"

        Public LevelUpLoyalty(6) As Integer
        Public LevelStartLoyalty(6) As Integer

        Public Sub InitializeLevelUpLoyalty()
            LevelUpLoyalty(0) = 0
            LevelUpLoyalty(1) = 5500
            LevelUpLoyalty(2) = 11500
            LevelUpLoyalty(3) = 17000
            LevelUpLoyalty(4) = 23500
            LevelUpLoyalty(5) = 31000
            LevelUpLoyalty(6) = 39500
        End Sub

        Public Sub InitializeLevelStartLoyalty()
            LevelStartLoyalty(0) = 0
            LevelStartLoyalty(1) = 2000
            LevelStartLoyalty(2) = 4500
            LevelStartLoyalty(3) = 7000
            LevelStartLoyalty(4) = 10000
            LevelStartLoyalty(5) = 13500
            LevelStartLoyalty(6) = 17500
        End Sub

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
                AddToWorld()

                If TypeOf Owner Is WS_PlayerData.CharacterObject Then
                    CType(Owner, CharacterObject).GroupUpdateFlag = CType(Owner, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_PET
                End If

                SendPetInitialize(Owner, Me)
            End Sub

            Public Sub Hide()
                RemoveFromWorld()

                If Not TypeOf Owner Is CharacterObject Then Exit Sub

                CType(Owner, CharacterObject).GroupUpdateFlag = CType(Owner, CharacterObject).GroupUpdateFlag Or PartyMemberStatsFlag.GROUP_UPDATE_PET

                Dim packet As New Packets.PacketClass(OPCODES.SMSG_PET_SPELLS)
                packet.AddUInt64(0)
                CType(Owner, CharacterObject).client.Send(packet)
                packet.Dispose()
            End Sub

        End Class

#End Region
#Region "WS.Pets.Handlers"

        Public Sub On_CMSG_PET_NAME_QUERY(ByRef packet As PacketClass, ByRef client As WS_Network.ClientClass)
            packet.GetInt16()
            Dim PetNumber As Integer = packet.GetInt32()
            Dim PetGUID As ULong = packet.GetUInt64()

            Log.WriteLine(LogType.DEBUG, "CMSG_PET_NAME_QUERY [Number={0} GUID={1:X}", PetNumber, PetGUID)

            SendPetNameQuery(client, PetGUID, PetNumber)
        End Sub

        Public Sub On_CMSG_REQUEST_PET_INFO(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "CMSG_REQUEST_PET_INFO")

            DumpPacket(packet.Data, client, 6)
        End Sub

        Public Sub On_CMSG_PET_ACTION(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim PetGUID As ULong = packet.GetUInt64()
            Dim SpellID As UShort = packet.GetUInt16()
            Dim SpellFlag As UShort = packet.GetUInt16()
            Dim TargetGUID As ULong = packet.GetUInt64()

            Log.WriteLine(LogType.DEBUG, "CMSG_PET_ACTION [GUID={0:X} Spell={1} Flag={2:X} Target={3:X}]", PetGUID, SpellID, SpellFlag, TargetGUID)
        End Sub

        Public Sub On_CMSG_PET_CANCEL_AURA(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "CMSG_PET_CANCEL_AURA")

            DumpPacket(packet.Data, client, 6)
        End Sub

        Public Sub On_CMSG_PET_ABANDON(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim PetGUID As ULong = packet.GetUInt64()

            Log.WriteLine(LogType.DEBUG, "CMSG_PET_ABANDON [GUID={0:X}]", PetGUID)
        End Sub

        Public Sub On_CMSG_PET_RENAME(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim PetGUID As ULong = packet.GetUInt64()
            Dim PetName As String = packet.GetString()

            Log.WriteLine(LogType.DEBUG, "CMSG_PET_RENAME [GUID={0:X} Name={1}]", PetGUID, PetName)
        End Sub

        Public Sub On_CMSG_PET_SET_ACTION(ByRef packet As PacketClass, ByRef client As ClientClass)
            packet.GetInt16()
            Dim PetGUID As ULong = packet.GetUInt64()
            Dim Position As Integer = packet.GetInt32()
            Dim SpellID As UShort = packet.GetUInt16()
            Dim ActionState As Short = packet.GetInt16()

            Log.WriteLine(LogType.DEBUG, "CMSG_PET_SET_ACTION [GUID={0:X} Pos={1} Spell={2} Action={3}]", PetGUID, Position, SpellID, ActionState)
        End Sub

        Public Sub On_CMSG_PET_SPELL_AUTOCAST(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "CMSG_PET_SPELL_AUTOCAST")

            DumpPacket(packet.Data, client, 6)
        End Sub

        Public Sub On_CMSG_PET_STOP_ATTACK(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "CMSG_PET_STOP_ATTACK")

            DumpPacket(packet.Data, client, 6)
        End Sub

        Public Sub On_CMSG_PET_UNLEARN(ByRef packet As PacketClass, ByRef client As ClientClass)
            Log.WriteLine(LogType.DEBUG, "CMSG_PET_UNLEARN")

            DumpPacket(packet.Data, client, 6)
        End Sub

        Public Sub SendPetNameQuery(ByRef client As ClientClass, ByVal PetGUID As ULong, ByVal PetNumber As Integer)
            If WORLD_CREATUREs.ContainsKey(PetGUID) = False Then Exit Sub
            If Not TypeOf WORLD_CREATUREs(PetGUID) Is PetObject Then Exit Sub

            Dim response As New PacketClass(OPCODES.SMSG_PET_NAME_QUERY_RESPONSE)
            response.AddInt32(PetNumber)
            response.AddString(CType(WORLD_CREATUREs(PetGUID), PetObject).PetName) 'Pet name
            response.AddInt32(_NativeMethods.timeGetTime("")) 'Pet name timestamp
            client.Send(response)
            response.Dispose()
        End Sub

#End Region
#Region "WS.Pets.AI"

        'TODO: Fix the pet AI
        Public Class PetAI
            Inherits WS_Creatures_AI.DefaultAI

            Public Sub New(ByRef Creature As CreatureObject)
                MyBase.New(Creature)
                AllowedMove = False
            End Sub

        End Class

#End Region
#Region "WS.Pets.Owner"
        Public Sub LoadPet(ByRef objCharacter As CharacterObject)
            If objCharacter.Pet IsNot Nothing Then Exit Sub

            Dim PetQuery As New DataTable
            CharacterDatabase.Query(String.Format("SELECT * FROM character_pet WHERE owner = '{0}';", objCharacter.GUID), PetQuery)
            If PetQuery.Rows.Count = 0 Then Exit Sub
            Dim PetInfo As DataRow = PetQuery.Rows(0)

            objCharacter.Pet = New PetObject(CULng(PetInfo.Item("id")) + _Global_Constants.GUID_PET, PetInfo.Item("entry")) With {
                .Owner = objCharacter,
                .SummonedBy = objCharacter.GUID,
                .CreatedBy = objCharacter.GUID,
                .Level = PetInfo.Item("level"),
                .XP = PetInfo.Item("exp"),
                .PetName = PetInfo.Item("name")
                }

            If CByte(PetInfo.Item("renamed")) = 0 Then
                objCharacter.Pet.Renamed = False
            Else
                objCharacter.Pet.Renamed = True
            End If

            objCharacter.Pet.Faction = objCharacter.Faction

            objCharacter.Pet.positionX = objCharacter.positionX
            objCharacter.Pet.positionY = objCharacter.positionY
            objCharacter.Pet.positionZ = objCharacter.positionZ
            objCharacter.Pet.MapID = objCharacter.MapID

            Log.WriteLine(LogType.DEBUG, "Loaded pet [{0}] for character [{1}].", objCharacter.Pet.GUID, objCharacter.GUID)
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
End NameSpace