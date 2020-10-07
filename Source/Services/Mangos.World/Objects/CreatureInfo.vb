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

Imports System.Data
Imports Mangos.Common
Imports Mangos.Common.Enums.Faction
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Unit
Imports Mangos.World.Globals
Imports Mangos.World.Gossip

Namespace Objects

'WARNING: Use only with CREATUREsDatabase()
    Public Class CreatureInfo
        Implements IDisposable
        Public Sub New(ByVal CreatureID As Integer)
            Me.New()
            Id = CreatureID
            _WorldServer.CREATURESDatabase.Add(Id, Me)

            'DONE: Load Item Data from MySQL
            Dim MySQLQuery As New DataTable
            _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM creature_template LEFT JOIN creature_template_spells ON creature_template.entry = creature_template_spells.`entry` WHERE creature_template.entry = {0};", CreatureID), MySQLQuery)

            If MySQLQuery.Rows.Count = 0 Then
                _WorldServer.Log.WriteLine(LogType.FAILED, "CreatureID {0} not found in SQL database.", CreatureID)
                found_ = False
                'Throw New ApplicationException(String.Format("CreatureID {0} not found in SQL database.", CreatureID))
                Exit Sub
            End If
            found_ = True

            ModelA1 = MySQLQuery.Rows(0).Item("modelid1")
            ModelA2 = MySQLQuery.Rows(0).Item("Modelid2")
            ModelH1 = MySQLQuery.Rows(0).Item("modelid3")
            ModelH2 = MySQLQuery.Rows(0).Item("modelid4")
            Name = MySQLQuery.Rows(0).Item("name")
            Try
                SubName = MySQLQuery.Rows(0).Item("subname")
            Catch
                SubName = ""
            End Try
            Size = MySQLQuery.Rows(0).Item("scale")

            MinLife = MySQLQuery.Rows(0).Item("MinLevelHealth")
            MaxLife = MySQLQuery.Rows(0).Item("MaxLevelHealth")
            MinMana = MySQLQuery.Rows(0).Item("MinLevelMana")
            MaxMana = MySQLQuery.Rows(0).Item("MaxLevelMana")
            ManaType = ManaTypes.TYPE_MANA
            Faction = MySQLQuery.Rows(0).Item("factionAlliance")
            'TODO: factionHorde?
            Elite = MySQLQuery.Rows(0).Item("rank")
            Damage.Maximum = MySQLQuery.Rows(0).Item("MaxMeleeDmg")
            RangedDamage.Maximum = MySQLQuery.Rows(0).Item("MaxRangedDmg")
            Damage.Minimum = MySQLQuery.Rows(0).Item("MinMeleeDmg")
            RangedDamage.Minimum = MySQLQuery.Rows(0).Item("MinRangedDmg")

            AttackPower = MySQLQuery.Rows(0).Item("MeleeAttackPower")
            RangedAttackPower = MySQLQuery.Rows(0).Item("RangedAttackPower")

            'TODO: What exactly is the speed column in the DB? It sure as hell wasn't runspeed :P
            WalkSpeed = MySQLQuery.Rows(0).Item("SpeedWalk")
            RunSpeed = MySQLQuery.Rows(0).Item("SpeedRun")
            BaseAttackTime = MySQLQuery.Rows(0).Item("MeleeBaseAttackTime")
            BaseRangedAttackTime = MySQLQuery.Rows(0).Item("RangedBaseAttackTime")

            cNpcFlags = MySQLQuery.Rows(0).Item("NpcFlags")
            DynFlags = MySQLQuery.Rows(0).Item("DynamicFlags")
            cFlags = MySQLQuery.Rows(0).Item("UnitFlags")

            TypeFlags = MySQLQuery.Rows(0).Item("CreatureTypeFlags")
            CreatureType = MySQLQuery.Rows(0).Item("CreatureType")
            CreatureFamily = MySQLQuery.Rows(0).Item("Family")
            LevelMin = MySQLQuery.Rows(0).Item("MinLevel")
            LevelMax = MySQLQuery.Rows(0).Item("MaxLevel")

            TrainerType = MySQLQuery.Rows(0).Item("TrainerType")
            TrainerSpell = MySQLQuery.Rows(0).Item("TrainerSpell")
            Classe = MySQLQuery.Rows(0).Item("TrainerClass")
            Race = MySQLQuery.Rows(0).Item("TrainerRace")
            Leader = MySQLQuery.Rows(0).Item("RacialLeader")

            If (Not IsDBNull(MySQLQuery.Rows(0).Item("spell1"))) Then
                Spells(0) = MySQLQuery.Rows(0).Item("spell1")
            Else
                Spells(0) = 0
            End If

            If (Not IsDBNull(MySQLQuery.Rows(0).Item("spell2"))) Then
                Spells(1) = MySQLQuery.Rows(0).Item("spell2")
            Else
                Spells(1) = 0
            End If

            If (Not IsDBNull(MySQLQuery.Rows(0).Item("spell3"))) Then
                Spells(2) = MySQLQuery.Rows(0).Item("spell3")
            Else
                Spells(2) = 0
            End If

            If (Not IsDBNull(MySQLQuery.Rows(0).Item("spell4"))) Then
                Spells(3) = MySQLQuery.Rows(0).Item("spell4")
            Else
                Spells(3) = 0
            End If

            PetSpellDataID = MySQLQuery.Rows(0).Item("PetSpellDataId")

            LootID = MySQLQuery.Rows(0).Item("LootId")
            SkinLootID = MySQLQuery.Rows(0).Item("SkinningLootId")
            PocketLootID = MySQLQuery.Rows(0).Item("PickpocketLootId")
            MinGold = MySQLQuery.Rows(0).Item("MinLootGold")
            MaxGold = MySQLQuery.Rows(0).Item("MaxLootGold")

            Resistances(0) = MySQLQuery.Rows(0).Item("Armor")
            Resistances(1) = MySQLQuery.Rows(0).Item("ResistanceHoly")
            Resistances(2) = MySQLQuery.Rows(0).Item("ResistanceFire")
            Resistances(3) = MySQLQuery.Rows(0).Item("ResistanceNature")
            Resistances(4) = MySQLQuery.Rows(0).Item("ResistanceFrost")
            Resistances(5) = MySQLQuery.Rows(0).Item("ResistanceShadow")
            Resistances(6) = MySQLQuery.Rows(0).Item("ResistanceArcane")

            'InhabitType
            'RegenHealth
            EquipmentID = MySQLQuery.Rows(0).Item("EquipmentTemplateId")
            MechanicImmune = MySQLQuery.Rows(0).Item("SchoolImmuneMask")
            'flags_extra

            'AIScriptSource = MySQLQuery.Rows(0).Item("ScriptName")

            If IO.File.Exists("scripts\gossip\" & _Functions.FixName(Name) & ".vb") Then
                Dim tmpScript As New ScriptedObject("scripts\gossip\" & _Functions.FixName(Name) & ".vb", "", True)
                TalkScript = tmpScript.InvokeConstructor("TalkScript")
                tmpScript.Dispose()
            Else
                If (cNpcFlags And NPCFlags.UNIT_NPC_FLAG_TRAINER) Then
                    TalkScript = New WS_NPCs.TDefaultTalk
                ElseIf (cNpcFlags And NPCFlags.UNIT_NPC_FLAG_GUARD) Then
                    TalkScript = New WS_GuardGossip.TGuardTalk
                ElseIf cNpcFlags = 0 Then
                    TalkScript = Nothing
                ElseIf cNpcFlags = NPCFlags.UNIT_NPC_FLAG_GOSSIP Then
                    TalkScript = New WS_NPCs.TDefaultTalk
                Else
                    TalkScript = New WS_NPCs.TDefaultTalk
                End If
            End If
        End Sub

        Public Sub New()
            Damage.Minimum = (0.8F * BaseAttackTime / 1000.0F) * (LevelMin * 10.0F)
            Damage.Maximum = (1.2F * BaseAttackTime / 1000.0F) * (LevelMax * 10.0F)
        End Sub

#Region "IDisposable Support"
        Private _disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposedValue Then
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                _WorldServer.CREATURESDatabase.Remove(Id)
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

        Public ReadOnly Property Life() As Integer
            Get
                Return _WorldServer.Rnd.Next(MinLife, MaxLife)
            End Get
        End Property

        Public ReadOnly Property Mana() As Integer
            Get
                Return _WorldServer.Rnd.Next(MinMana, MaxMana)
            End Get
        End Property

        Public ReadOnly Property GetRandomModel() As Integer
            Get
                Dim modelIDs(3) As Integer
                Dim current As Integer = 0
                If ModelA1 Then
                    modelIDs(current) = ModelA1
                    current += 1
                End If
                If ModelA2 Then
                    modelIDs(current) = ModelA2
                    current += 1
                End If
                If ModelH1 Then
                    modelIDs(current) = ModelH1
                    current += 1
                End If
                If ModelH2 Then
                    modelIDs(current) = ModelH2
                    current += 1
                End If
                If current = 0 Then Return 0
                Return modelIDs(_WorldServer.Rnd.Next(0, current))
            End Get
        End Property

        Public ReadOnly Property GetFirstModel() As Integer
            Get
                If ModelA1 Then
                    Return ModelA1
                ElseIf ModelA2 Then
                    Return ModelA2
                ElseIf ModelH1 Then
                    Return ModelH1
                ElseIf ModelH2 Then
                    Return ModelH2
                Else
                    Return 0
                End If
            End Get
        End Property

        Private found_ As Boolean = False

        Public Id As Integer = 0
        Public Name As String = "MISSING_CREATURE_INFO"
        Public SubName As String = ""
        Public Size As Single = 1
        Public ModelA1 As Integer = 262
        Public ModelA2 As Integer = 0
        Public ModelH1 As Integer = 0
        Public ModelH2 As Integer = 0
        Public MinLife As Integer = 1
        Public MaxLife As Integer = 1
        Public MinMana As Integer = 1
        Public MaxMana As Integer = 1
        Public ManaType As Byte = 0
        Public Faction As Short = FactionTemplates.None
        Public CreatureType As Byte = UNIT_TYPE.NONE
        Public CreatureFamily As Byte = CREATURE_FAMILY.NONE
        Public Elite As Byte = CREATURE_ELITE.NORMAL
        Public HonorRank As Byte = 0
        Public Damage As New WS_Items.TDamage
        Public RangedDamage As New WS_Items.TDamage
        Public AttackPower As Integer = 0
        Public RangedAttackPower As Integer = 0
        Public Resistances() As Integer = {0, 0, 0, 0, 0, 0, 0}

        Public WalkSpeed As Single = _Global_Constants.UNIT_NORMAL_WALK_SPEED
        Public RunSpeed As Single = _Global_Constants.UNIT_NORMAL_RUN_SPEED
        Public BaseAttackTime As Short = 2000
        Public BaseRangedAttackTime As Short = 2000

        Public cNpcFlags As Integer
        Public DynFlags As Integer
        Public cFlags As Integer
        Public TypeFlags As UInteger
        Public LevelMin As Byte = 1
        Public LevelMax As Byte = 1
        Public Leader As Byte = 0

        Public TrainerType As Integer
        Public TrainerSpell As Integer = 0
        Public Classe As Byte = 0
        Public Race As Byte = 0

        Public PetSpellDataID As Integer = 0
        Public Spells() As Integer = {0, 0, 0, 0}

        Public LootID As Integer = 0
        Public SkinLootID As Integer = 0
        Public PocketLootID As Integer = 0
        Public MinGold As UInteger = 0
        Public MaxGold As UInteger = 0

        Public EquipmentID As Integer = 0
        Public MechanicImmune As UInteger = 0UI

        Public UnkFloat1 As Single = 1
        Public UnkFloat2 As Single = 1

        Public AIScriptSource As String = ""

        Public TalkScript As TBaseTalk = Nothing
    End Class
End NameSpace