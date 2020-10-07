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
Imports Mangos.Common.Enums.Global
Imports Mangos.World.Server

Namespace DataStores

    Public Class WS_DBCDatabase

#Region "Emotes"
        Public EmotesState As New Dictionary(Of Integer, Integer)
        Public EmotesText As New Dictionary(Of Integer, Integer)
#End Region

#Region "SkillLines"
        Public SkillLines As New Dictionary(Of Integer, Integer)

        Public SkillLineAbility As New Dictionary(Of Integer, TSkillLineAbility)
        Public Class TSkillLineAbility
            Public ID As Integer
            Public SkillID As Integer
            Public SpellID As Integer
            Public Unknown1 As Integer
            Public Unknown2 As Integer
            Public Unknown3 As Integer
            Public Unknown4 As Integer
            Public Required_Skill_Value As Integer ' For Trade Skill, Not For Training
            Public Forward_SpellID As Integer
            Public Unknown5 As Integer
            Public Max_Value As Integer
            Public Min_Value As Integer
        End Class

#End Region

#Region "Taxi"
        Public TaxiNodes As New Dictionary(Of Integer, TTaxiNode)
        Public TaxiPaths As New Dictionary(Of Integer, TTaxiPath)
        Public TaxiPathNodes As New Dictionary(Of Integer, Dictionary(Of Integer, TTaxiPathNode))
        Public Class TTaxiNode
            Public x As Single
            Public y As Single
            Public z As Single
            Public MapID As Integer
            Public HordeMount As Integer = 0
            Public AllianceMount As Integer = 0

            Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMapID As Integer, ByVal pHMount As Integer, ByVal pAMount As Integer)
                x = px
                y = py
                z = pz
                MapID = pMapID
                HordeMount = pHMount
                AllianceMount = pAMount
            End Sub
        End Class

        Public Class TTaxiPath
            Public TFrom As Integer
            Public TTo As Integer
            Public Price As Integer

            Public Sub New(ByVal pFrom As Integer, ByVal pTo As Integer, ByVal pPrice As Integer)
                TFrom = pFrom
                TTo = pTo
                Price = pPrice
            End Sub
        End Class

        Public Class TTaxiPathNode
            Public Path As Integer
            Public Seq As Integer
            Public MapID As Integer
            Public x As Single
            Public y As Single
            Public z As Single
            Public action As Integer
            Public waittime As Integer

            Public Sub New(ByVal px As Single, ByVal py As Single, ByVal pz As Single, ByVal pMapID As Integer, ByVal pPath As Integer, ByVal pSeq As Integer, ByVal pAction As Integer, ByVal pWaittime As Integer)
                x = px
                y = py
                z = pz
                MapID = pMapID
                Path = pPath
                Seq = pSeq
                action = pAction
                waittime = pWaittime
            End Sub

        End Class

        Public Function GetNearestTaxi(ByVal x As Single, ByVal y As Single, ByVal map As Integer) As Integer
            Dim minDistance As Single = 1.0E+8F
            Dim selectedTaxiNode As Integer = 0
            Dim tmp As Single

            For Each TaxiNode As KeyValuePair(Of Integer, TTaxiNode) In TaxiNodes
                If TaxiNode.Value.MapID = map Then
                    tmp = _WS_Combat.GetDistance(x, TaxiNode.Value.x, y, TaxiNode.Value.y)
                    If tmp < minDistance Then
                        minDistance = tmp
                        selectedTaxiNode = TaxiNode.Key
                    End If
                End If
            Next
            Return selectedTaxiNode
        End Function
#End Region

#Region "Talents"
        Public TalentsTab As New Dictionary(Of Integer, Integer)(30)
        Public Talents As New Dictionary(Of Integer, TalentInfo)(500)
        Public Class TalentInfo
            Public TalentID As Integer
            Public TalentTab As Integer
            Public Row As Integer
            Public Col As Integer
            Public RankID(4) As Integer
            Public RequiredTalent(2) As Integer
            Public RequiredPoints(2) As Integer
        End Class
#End Region

#Region "Factions"
        Public Const FACTION_TEMPLATES_COUNT As Integer = 2074

        Public CharRaces As New Dictionary(Of Integer, TCharRace)
        Public Class TCharRace
            Public FactionID As Short
            Public ModelMale As Integer
            Public ModelFemale As Integer
            Public TeamID As Byte
            Public TaxiMask As UInteger
            Public CinematicID As Integer
            Public RaceName As String

            Public Sub New(ByVal Faction As Short, ByVal ModelM As Integer, ByVal ModelF As Integer, ByVal Team As Byte, ByVal Taxi As UInteger, ByVal Cinematic As Integer, ByVal Name As String)
                FactionID = Faction
                ModelMale = ModelM
                ModelFemale = ModelF
                TeamID = Team
                TaxiMask = Taxi
                CinematicID = Cinematic
                RaceName = Name
            End Sub
        End Class

        Public CharClasses As New Dictionary(Of Integer, TCharClass)
        Public Class TCharClass
            Public CinematicID As Integer

            Public Sub New(ByVal Cinematic As Integer)
                CinematicID = Cinematic
            End Sub
        End Class

        Public FactionInfo As New Dictionary(Of Integer, TFaction)
        Public Class TFaction
            Public ID As Short
            Public VisibleID As Short
            Public flags(3) As Short
            Public rep_stats(3) As Integer
            Public rep_flags(3) As Byte

            Public Sub New(ByVal Id_ As Short, ByVal VisibleID_ As Short, ByVal flags1 As Integer, ByVal flags2 As Integer, ByVal flags3 As Integer, ByVal flags4 As Integer, ByVal rep_stats1 As Integer, ByVal rep_stats2 As Integer, ByVal rep_stats3 As Integer, ByVal rep_stats4 As Integer, ByVal rep_flags1 As Integer, ByVal rep_flags2 As Integer, ByVal rep_flags3 As Integer, ByVal rep_flags4 As Integer)
                ID = Id_
                VisibleID = VisibleID_
                flags(0) = flags1
                flags(1) = flags2
                flags(2) = flags3
                flags(3) = flags4
                rep_stats(0) = rep_stats1
                rep_stats(1) = rep_stats2
                rep_stats(2) = rep_stats3
                rep_stats(3) = rep_stats4
                rep_flags(0) = rep_flags1
                rep_flags(1) = rep_flags2
                rep_flags(2) = rep_flags3
                rep_flags(3) = rep_flags4
            End Sub
        End Class

        Public FactionTemplatesInfo As New Dictionary(Of Integer, TFactionTemplate)
        Public Class TFactionTemplate
            Public FactionID As Integer
            Public ourMask As UInteger
            Public friendMask As UInteger
            Public enemyMask As UInteger
            Public enemyFaction1 As Integer
            Public enemyFaction2 As Integer
            Public enemyFaction3 As Integer
            Public enemyFaction4 As Integer
            Public friendFaction1 As Integer
            Public friendFaction2 As Integer
            Public friendFaction3 As Integer
            Public friendFaction4 As Integer
        End Class
#End Region

#Region "Spells"
        Public SpellShapeShiftForm As New List(Of TSpellShapeshiftForm)
        Public Class TSpellShapeshiftForm
            Public ID As Integer = 0
            Public Flags1 As Integer = 0
            Public CreatureType As Integer
            Public AttackSpeed As Integer

            Public Sub New(ByVal ID_ As Integer, ByVal Flags1_ As Integer, ByVal CreatureType_ As Integer, ByVal AttackSpeed_ As Integer)
                ID = ID_
                Flags1 = Flags1_
                CreatureType = CreatureType_
                AttackSpeed = AttackSpeed_
            End Sub
        End Class

        Public Function FindShapeshiftForm(ByVal ID As Integer) As TSpellShapeshiftForm
            For Each Form As TSpellShapeshiftForm In SpellShapeShiftForm
                If Form.ID = ID Then
                    Return Form
                End If
            Next

            Return Nothing
        End Function

        Public gtOCTRegenHP As New List(Of Single)
        Public gtOCTRegenMP As New List(Of Single)
        Public gtRegenHPPerSpt As New List(Of Single)
        Public gtRegenMPPerSpt As New List(Of Single)
#End Region

#Region "Items"
        Public Const DurabilityCosts_MAX As Integer = 300
        Public DurabilityCosts(DurabilityCosts_MAX, 28) As Short

        Public SpellItemEnchantments As New Dictionary(Of Integer, TSpellItemEnchantment)
        Public Class TSpellItemEnchantment
            Public Type(2) As Integer
            Public Amount(2) As Integer
            Public SpellID(2) As Integer
            Public AuraID As Integer
            Public Slot As Integer
            'Public EnchantmentConditions As Integer

            Public Sub New(ByVal Types() As Integer, ByVal Amounts() As Integer, ByVal SpellIDs() As Integer, ByVal AuraID_ As Integer, ByVal Slot_ As Integer) ', ByVal EnchantmentConditions_ As Integer)
                For i As Byte = 0 To 2
                    Type(i) = Types(i)
                    Amount(i) = Amounts(i)
                    SpellID(i) = SpellIDs(i)
                Next
                AuraID = AuraID_
                Slot = Slot_
                'EnchantmentConditions = EnchantmentConditions_
            End Sub
        End Class

        Public ItemSet As New Dictionary(Of Integer, TItemSet)
        Public Class TItemSet
            Public ID As Integer ' 0
            Public Name As String ' 1
            Public ItemID(7) As Integer ' 10-17
            Public SpellID(7) As Integer ' 66-73
            Public ItemCount(7) As Integer ' 74-81
            Public Required_Skill_ID As Integer ' 82
            Public Required_Skill_Value As Integer ' 83

            Public Sub New(ByVal Name_ As String, ByVal ItemID_() As Integer, ByVal SpellID_() As Integer, ByVal ItemCount_() As Integer, ByVal Required_Skill_ID_ As Integer, ByVal Required_Skill_Value_ As Integer)
                For i As Byte = 0 To 7
                    SpellID(i) = SpellID_(i)
                    ItemID(i) = ItemID_(i)
                    ItemCount(i) = ItemCount_(i)
                Next
                Name = Name_
                Required_Skill_ID = Required_Skill_ID_
                Required_Skill_Value = Required_Skill_Value_
            End Sub
        End Class

        Public ItemDisplayInfo As New Dictionary(Of Integer, TItemDisplayInfo)
        Public Class TItemDisplayInfo
            Public ID As Integer
            Public RandomPropertyChance As Integer
            Public Unknown As Integer
        End Class

        Public ItemRandomPropertiesInfo As New Dictionary(Of Integer, TItemRandomPropertiesInfo)
        Public Class TItemRandomPropertiesInfo
            Public ID As Integer
            Public Enchant_ID(3) As Integer
        End Class

#End Region

#Region "XPTable"
        ''' <summary>
        ''' Initializes the xp lookup table from db.
        ''' </summary>
        ''' <returns></returns>
        Private Sub InitializeXpTableFromDb()
            Dim result As DataTable = Nothing
            Dim dbLvl As Integer
            Dim dbXp As Long
            Try
                _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM player_xp_for_level order by lvl;"), result)
                If result.Rows.Count > 0 Then
                    For Each row As DataRow In result.Rows
                        dbLvl = row.Item("lvl")
                        dbXp = row.Item("xp_for_next_level")
                        _WS_Player_Initializator.XPTable(dbLvl) = dbXp
                    Next
                End If
                _WorldServer.Log.WriteLine(LogType.INFORMATION, "Initalizing: XPTable initialized.")
            Catch ex As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "XPTable initialization failed.")
            End Try
        End Sub
#End Region

#Region "Battlemasters"

        Public Battlemasters As New Dictionary(Of Integer, Byte)
        Public Sub InitializeBattlemasters()
            Dim MySQLQuery As New DataTable
            _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM battlemaster_entry"), MySQLQuery)

            For Each row As DataRow In MySQLQuery.Rows
                Battlemasters.Add(row.Item("entry"), row.Item("bg_template"))
            Next

            _WorldServer.Log.WriteLine(LogType.INFORMATION, "World: {0} Battlemasters Loaded.", MySQLQuery.Rows.Count)
        End Sub

#End Region

#Region "Battlegrounds"

        Public Battlegrounds As New Dictionary(Of Byte, TBattleground)
        Public Sub InitializeBattlegrounds()
            Dim entry As Byte

            Dim mySqlQuery As New DataTable
            _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM battleground_template"), mySqlQuery)

            For Each row As DataRow In mySqlQuery.Rows
                entry = row.Item("id")
                Battlegrounds.Add(entry, New TBattleground)

                '            Battlegrounds(Entry).Map = row.Item("Map")
                Battlegrounds(entry).MinPlayersPerTeam = row.Item("MinPlayersPerTeam")
                Battlegrounds(entry).MaxPlayersPerTeam = row.Item("MaxPlayersPerTeam")
                Battlegrounds(entry).MinLevel = row.Item("MinLvl")
                Battlegrounds(entry).MaxLevel = row.Item("MaxLvl")
                Battlegrounds(entry).AllianceStartLoc = row.Item("AllianceStartLoc")
                Battlegrounds(entry).AllianceStartO = row.Item("AllianceStartO")
                Battlegrounds(entry).HordeStartLoc = row.Item("HordeStartLoc")
                Battlegrounds(entry).HordeStartO = row.Item("HordeStartO")
            Next

            _WorldServer.Log.WriteLine(LogType.INFORMATION, "World: {0} Battlegrounds Loaded.", mySqlQuery.Rows.Count)
        End Sub

        Public Class TBattleground
            '        Public Map As Integer
            Public MinPlayersPerTeam As Byte
            Public MaxPlayersPerTeam As Byte
            Public MinLevel As Byte
            Public MaxLevel As Byte
            Public AllianceStartLoc As Single
            Public AllianceStartO As Single
            Public HordeStartLoc As Single
            Public HordeStartO As Single
        End Class

#End Region

#Region "TeleportCoords"
        Public TeleportCoords As New Dictionary(Of Integer, TTeleportCoords)
        Public Sub InitializeTeleportCoords()
            Dim SpellID As Integer

            Dim MySQLQuery As New DataTable
            _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM spells_teleport_coords"), MySQLQuery)

            For Each row As DataRow In MySQLQuery.Rows
                SpellID = row.Item("id")
                TeleportCoords.Add(SpellID, New TTeleportCoords)

                TeleportCoords(SpellID).Name = row.Item("name")
                TeleportCoords(SpellID).MapID = row.Item("mapId")
                TeleportCoords(SpellID).PosX = row.Item("position_x")
                TeleportCoords(SpellID).PosY = row.Item("position_y")
                TeleportCoords(SpellID).PosZ = row.Item("position_z")
            Next

            _WorldServer.Log.WriteLine(LogType.INFORMATION, "World: {0} Teleport Coords Loaded.", MySQLQuery.Rows.Count)
        End Sub

        Public Class TTeleportCoords
            Public Name As String
            Public MapID As UInteger
            Public PosX As Single
            Public PosY As Single
            Public PosZ As Single
        End Class
#End Region

#Region "MonterSayCombat"
        'Public Sub InitializeMonsterSayCombat()
        '    ' Load the MonsterSayCombat Hashtable.
        '    Dim Entry As Integer = 0
        '    Dim EventNo As Integer = 0
        '    Dim Chance As Single = 0.0F
        '    Dim Language As Integer = 0
        '    Dim Type As Integer = 0
        '    Dim MonsterName As String = ""
        '    Dim Text0 As String = ""
        '    Dim Text1 As String = ""
        '    Dim Text2 As String = ""
        '    Dim Text3 As String = ""
        '    Dim Text4 As String = ""
        '    Dim Count As Integer = 0

        '    Dim MySQLQuery As New DataTable
        '    _WorldServer.WorldDatabase.Query(String.Format("SELECT * FROM npc_monstersay"), MySQLQuery)
        '    For Each MonsterRow As DataRow In MySQLQuery.Rows
        '        Count = Count + 1
        '        Entry = MonsterRow.Item("entry")
        '        EventNo = MonsterRow.Item("event")
        '        Chance = MonsterRow.Item("chance")
        '        Language = MonsterRow.Item("language")
        '        Type = MonsterRow.Item("type")
        '        If Not MonsterRow.Item("monstername") Is DBNull.Value Then
        '            MonsterName = MonsterRow.Item("monstername")
        '        Else
        '            MonsterName = ""
        '        End If

        '        If Not MonsterRow.Item("text0") Is DBNull.Value Then
        '            Text0 = MonsterRow.Item("text0")
        '        Else
        '            Text0 = ""
        '        End If

        '        If Not MonsterRow.Item("text1") Is DBNull.Value Then
        '            Text1 = MonsterRow.Item("text1")
        '        Else
        '            Text1 = ""
        '        End If

        '        If Not MonsterRow.Item("text2") Is DBNull.Value Then
        '            Text2 = MonsterRow.Item("text2")
        '        Else
        '            Text2 = ""
        '        End If

        '        If Not MonsterRow.Item("text3") Is DBNull.Value Then
        '            Text3 = MonsterRow("text3")
        '        Else
        '            Text3 = ""
        '        End If

        '        If Not MonsterRow.Item("text4") Is DBNull.Value Then
        '            Text4 = MonsterRow("text4")
        '        Else
        '            Text4 = ""
        '        End If

        '        If EventNo = MonsterSayEvents.MONSTER_SAY_EVENT_COMBAT Then
        '            MonsterSayCombat(Entry) = New TMonsterSayCombat(Entry, EventNo, Chance, Language, Type, MonsterName, Text0, Text1, Text2, Text3, Text4)
        '        End If

        '    Next

        '    _WorldServer.Log.WriteLine(LogType.INFORMATION, "World: {0} Monster Say(s) Loaded.", Count)

        'End Sub
#End Region

#Region "Creatures"
        Public CreatureGossip As New Dictionary(Of ULong, Integer)

        Public CreaturesFamily As New Dictionary(Of Integer, CreatureFamilyInfo)
        Public Class CreatureFamilyInfo
            Public ID As Integer
            Public Unknown1 As Integer
            Public Unknown2 As Integer
            Public PetFoodID As Integer
            Public Name As String
        End Class

        Public CreatureMovement As New Dictionary(Of Integer, Dictionary(Of Integer, CreatureMovePoint))
        Public Class CreatureMovePoint
            Public x As Single
            Public y As Single
            Public z As Single
            Public waittime As Integer
            Public moveflag As Integer
            Public action As Integer
            Public actionchance As Integer

            Public Sub New(ByVal PosX As Single, ByVal PosY As Single, ByVal PosZ As Single, ByVal Wait As Integer, ByVal MoveFlag As Integer, ByVal Action As Integer, ByVal ActionChance As Integer)
                x = PosX
                y = PosY
                z = PosZ
                waittime = Wait
                Me.moveflag = MoveFlag
                Me.action = Action
                Me.actionchance = ActionChance
            End Sub
        End Class

        Public CreatureEquip As New Dictionary(Of Integer, CreatureEquipInfo)
        Public Class CreatureEquipInfo
            Public EquipModel(2) As Integer
            Public EquipInfo(2) As UInteger
            Public EquipSlot(2) As Integer

            Public Sub New(ByVal EquipModel1 As Integer, ByVal EquipModel2 As Integer, ByVal EquipModel3 As Integer, ByVal EquipInfo1 As UInteger, ByVal EquipInfo2 As UInteger, ByVal EquipInfo3 As UInteger, ByVal EquipSlot1 As Integer, ByVal EquipSlot2 As Integer, ByVal EquipSlot3 As Integer)
                EquipModel(0) = EquipModel1
                EquipModel(1) = EquipModel2
                EquipModel(2) = EquipModel3
                EquipInfo(0) = EquipInfo1
                EquipInfo(1) = EquipInfo2
                EquipInfo(2) = EquipInfo3
                EquipSlot(0) = EquipSlot1
                EquipSlot(1) = EquipSlot2
                EquipSlot(2) = EquipSlot3
            End Sub
        End Class

        Public CreatureModel As New Dictionary(Of Integer, CreatureModelInfo)
        Public Class CreatureModelInfo
            Public BoundingRadius As Single
            Public CombatReach As Single
            Public Gender As Byte
            Public ModelIDOtherGender As Integer

            Public Sub New(ByVal BoundingRadius As Single, ByVal CombatReach As Single, ByVal Gender As Byte, ByVal ModelIDOtherGender As Integer)
                Me.BoundingRadius = BoundingRadius
                Me.CombatReach = CombatReach
                Me.Gender = Gender
                Me.ModelIDOtherGender = ModelIDOtherGender
            End Sub
        End Class
#End Region

#Region "Other"

        Public Sub InitializeInternalDatabase()

            InitializeLoadDBCs()

            _WS_Spells.InitializeSpellDB()

            _WS_Commands.RegisterChatCommands()

            Try
                _WS_TimerBasedEvents.Regenerator = New WS_TimerBasedEvents.TRegenerator
                _WS_TimerBasedEvents.AIManager = New WS_TimerBasedEvents.TAIManager
                _WS_TimerBasedEvents.SpellManager = New WS_TimerBasedEvents.TSpellManager
                _WS_TimerBasedEvents.CharacterSaver = New WS_TimerBasedEvents.TCharacterSaver
                _WS_TimerBasedEvents.WeatherChanger = New WS_TimerBasedEvents.TWeatherChanger

                _WorldServer.Log.WriteLine(LogType.INFORMATION, "World: Loading Maps and Spawns....")

                'DONE: Initializing Counters
                Dim MySQLQuery As New DataTable
                Try
                    _WorldServer.CharacterDatabase.Query(String.Format("SELECT MAX(item_guid) FROM characters_inventory;"), MySQLQuery)
                    If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                        _WorldServer.itemGuidCounter = MySQLQuery.Rows(0).Item(0) + _Global_Constants.GUID_ITEM
                    Else
                        _WorldServer.itemGuidCounter = 0 + _Global_Constants.GUID_ITEM
                    End If
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "World: Failed loading characters_inventory....")
                End Try
                MySQLQuery = New DataTable
                Try
                    _WorldServer.WorldDatabase.Query(String.Format("SELECT MAX(guid) FROM creature;"), MySQLQuery)
                    If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                        _WorldServer.CreatureGUIDCounter = MySQLQuery.Rows(0).Item(0) + _Global_Constants.GUID_UNIT
                    Else
                        _WorldServer.CreatureGUIDCounter = 0 + _Global_Constants.GUID_UNIT
                    End If
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "World: Failed loading creatures....")
                End Try

                MySQLQuery = New DataTable
                Try
                    _WorldServer.WorldDatabase.Query(String.Format("SELECT MAX(guid) FROM gameobject;"), MySQLQuery)
                    If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                        _WorldServer.GameObjectsGUIDCounter = MySQLQuery.Rows(0).Item(0) + _Global_Constants.GUID_GAMEOBJECT
                    Else
                        _WorldServer.GameObjectsGUIDCounter = 0 + _Global_Constants.GUID_GAMEOBJECT
                    End If
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "World: Failed loading gameobjects....")
                End Try

                MySQLQuery = New DataTable
                Try
                    _WorldServer.CharacterDatabase.Query(String.Format("SELECT MAX(guid) FROM corpse"), MySQLQuery)
                    If Not MySQLQuery.Rows(0).Item(0) Is DBNull.Value Then
                        _WorldServer.CorpseGUIDCounter = MySQLQuery.Rows(0).Item(0) + _Global_Constants.GUID_CORPSE
                    Else
                        _WorldServer.CorpseGUIDCounter = 0 + _Global_Constants.GUID_CORPSE
                    End If
                Catch ex As Exception
                    _WorldServer.Log.WriteLine(LogType.FAILED, "World: Failed loading corpse....")
                End Try

            Catch e As Exception
                _WorldServer.Log.WriteLine(LogType.FAILED, "Internal database initialization failed! [{0}]{1}{2}", e.Message, Environment.NewLine, e.ToString)
            End Try
        End Sub

        Public Sub InitializeLoadDBCs()
            _WS_Maps.InitializeMaps()
            InitializeXpTableFromDb()
            _WS_DBCLoad.InitializeEmotes()
            _WS_DBCLoad.InitializeEmotesText()
            _WS_DBCLoad.InitializeAreaTable()
            _WS_DBCLoad.InitializeFactions()
            _WS_DBCLoad.InitializeFactionTemplates()
            _WS_DBCLoad.InitializeCharRaces()
            _WS_DBCLoad.InitializeCharClasses()
            _WS_DBCLoad.InitializeSkillLines()
            _WS_DBCLoad.InitializeSkillLineAbility()
            _WS_DBCLoad.InitializeLocks()
            '_WorldServer.AllGraveYards.InitializeGraveyards()
            _WS_DBCLoad.InitializeTaxiNodes()
            _WS_DBCLoad.InitializeTaxiPaths()
            _WS_DBCLoad.InitializeTaxiPathNodes()
            _WS_DBCLoad.InitializeDurabilityCosts()
            _WS_DBCLoad.LoadSpellItemEnchantments()
            _WS_DBCLoad.LoadItemSet()
            _WS_DBCLoad.LoadItemDisplayInfoDbc()
            _WS_DBCLoad.LoadItemRandomPropertiesDbc()
            _WS_DBCLoad.LoadTalentDbc()
            _WS_DBCLoad.LoadTalentTabDbc()
            _WS_DBCLoad.LoadAuctionHouseDbc()
            _WS_DBCLoad.LoadLootStores()
            _WS_DBCLoad.LoadWeather()

            InitializeBattlemasters()
            InitializeBattlegrounds()
            InitializeTeleportCoords()
            'InitializeMonsterSayCombat()
            _WS_DBCLoad.LoadCreatureFamilyDbc()

            _WS_DBCLoad.InitializeSpellRadius()
            _WS_DBCLoad.InitializeSpellDuration()
            _WS_DBCLoad.InitializeSpellCastTime()
            _WS_DBCLoad.InitializeSpellRange()
            _WS_DBCLoad.InitializeSpellFocusObject()
            _WS_DBCLoad.InitializeSpells()
            _WS_DBCLoad.InitializeSpellShapeShift()
            _WS_DBCLoad.InitializeSpellChains()

            _WS_DBCLoad.LoadCreatureGossip()
            _WS_DBCLoad.LoadCreatureMovements()
            _WS_DBCLoad.LoadCreatureEquipTable()
            _WS_DBCLoad.LoadCreatureModelInfo()
            _WS_DBCLoad.LoadQuestStartersAndFinishers()

            'LoadTransports()

        End Sub

#End Region
    End Class
End Namespace