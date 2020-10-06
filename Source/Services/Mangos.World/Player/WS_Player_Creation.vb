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
Imports Mangos.Common.Enums.Character
Imports Mangos.Common.Enums.Global
Imports Mangos.Common.Enums.Misc
Imports Mangos.World.DataStores
Imports Mangos.World.Globals
Imports Mangos.World.Objects

Namespace Player

    Public Module WS_Player_Creation
        Public Function CreateCharacter(ByVal Account As String, ByVal Name As String, ByVal Race As Byte, ByVal Classe As Byte, ByVal Gender As Byte, ByVal Skin As Byte, ByVal Face As Byte, ByVal HairStyle As Byte, ByVal HairColor As Byte, ByVal FacialHair As Byte, ByVal OutfitID As Byte) As Integer
            Dim Character As New CharacterObject
            Dim MySQLQuery As New DataTable

            'DONE: Make name capitalized as on official
            Character.Name = CapitalizeName(Name)
            Character.Race = Race
            Character.Classe = Classe
            Character.Gender = Gender
            Character.Skin = Skin
            Character.Face = Face
            Character.HairStyle = HairStyle
            Character.HairColor = HairColor
            Character.FacialHair = FacialHair

            'DONE: Query Access Level and Account ID
            AccountDatabase.Query(String.Format("SELECT id, gmlevel FROM account WHERE username = ""{0}"";", Account), MySQLQuery)
            Dim Account_ID As Integer = MySQLQuery.Rows(0).Item("id")
            Dim Account_Access As AccessLevel = MySQLQuery.Rows(0).Item("gmlevel")
            Character.Access = Account_Access

            If Not ValidateName(Character.Name) Then
                Return CharResponse.CHAR_NAME_INVALID_CHARACTER
            End If

            'DONE: Name In Use
            Try
                MySQLQuery.Clear()
                CharacterDatabase.Query(String.Format("SELECT char_name FROM characters WHERE char_name = ""{0}"";", Character.Name), MySQLQuery)
                If MySQLQuery.Rows.Count > 0 Then
                    Return CharResponse.CHAR_CREATE_NAME_IN_USE
                End If
            Catch
                Return CharResponse.CHAR_CREATE_FAILED
            End Try

            'DONE: Check for disabled class/race, only for non GM/Admin
            If (_Global_Constants.SERVER_CONFIG_DISABLED_CLASSES(Character.Classe - 1) = True) OrElse (_Global_Constants.SERVER_CONFIG_DISABLED_RACES(Character.Race - 1) = True) AndAlso Account_Access < AccessLevel.GameMaster Then
                Return CharResponse.CHAR_CREATE_DISABLED
            End If

            'DONE: Check for both horde and alliance
            'TODO: Only if it's a pvp realm
            If Account_Access <= AccessLevel.Player Then
                MySQLQuery.Clear()
                CharacterDatabase.Query(String.Format("SELECT char_race FROM characters WHERE account_id = ""{0}"" LIMIT 1;", Account_ID), MySQLQuery)
                If MySQLQuery.Rows.Count > 0 Then
                    If Character.IsHorde <> GetCharacterSide(MySQLQuery.Rows(0).Item("char_race")) Then
                        Return CharResponse.CHAR_CREATE_PVP_TEAMS_VIOLATION
                    End If
                End If
            End If

            'DONE: Check for MAX characters limit on this realm
            MySQLQuery.Clear()
            CharacterDatabase.Query(String.Format("SELECT char_name FROM characters WHERE account_id = ""{0}"";", Account_ID), MySQLQuery)
            If MySQLQuery.Rows.Count >= 10 Then
                Return CharResponse.CHAR_CREATE_SERVER_LIMIT
            End If

            'DONE: Check for max characters in total on all realms
            MySQLQuery.Clear()
            CharacterDatabase.Query(String.Format("SELECT char_name FROM characters WHERE account_id = ""{0}"";", Account_ID), MySQLQuery)
            If MySQLQuery.Rows.Count >= 10 Then
                Return CharResponse.CHAR_CREATE_ACCOUNT_LIMIT
            End If

            'DONE: Generate GUID, MySQL Auto generation
            'DONE: Create Char
            Try
                InitializeReputations(Character)
                CreateCharacter(Character)
                Character.SaveAsNewCharacter(Account_ID)
                CreateCharacterSpells(Character)
                CreateCharacterItems(Character)

                'Log.WriteLine(LogType.DEBUG, "[{0}:{1}] SMSG_CHAR_CREATE [{2}]", client.IP, client.Port, Character.Name)
            Catch err As Exception
                Log.WriteLine(LogType.FAILED, "Error initializing character! {0} {1}", Environment.NewLine, err.ToString)
                Return CharResponse.CHAR_CREATE_FAILED
            Finally
                Character.Dispose()
            End Try

            Return CharResponse.CHAR_CREATE_SUCCESS
        End Function

        Public Sub CreateCharacter(ByRef objCharacter As CharacterObject)
            Dim CreateInfo As New DataTable
            Dim CreateInfoBars As New DataTable
            Dim CreateInfoSkills As New DataTable
            Dim LevelStats As New DataTable
            Dim ClassLevelStats As New DataTable

            Dim ButtonPos As Integer = 0

            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo WHERE race = {0} AND class = {1};", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer)), CreateInfo)
            If CreateInfo.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in playercreateinfo table for Race: {0}, Class: {1}", objCharacter.Race, objCharacter.Classe)
            End If

            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo_action WHERE race = {0} AND class = {1} ORDER BY button;", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer)), CreateInfoBars)
            If CreateInfoBars.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in playercreateinfo_action table for Race: {0}, Class: {1}", objCharacter.Race, objCharacter.Classe)
            End If

            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo_skill WHERE race = {0} AND class = {1};", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer)), CreateInfoSkills)
            If CreateInfoSkills.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in playercreateinfo_skill table for Race: {0}, Class: {1}", objCharacter.Race, objCharacter.Classe)
            End If

            WorldDatabase.Query(String.Format("SELECT * FROM player_levelstats WHERE race = {0} AND class = {1} AND level = {2};", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer), CType(objCharacter.Level, Integer)), LevelStats)
            If LevelStats.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in player_levelstats table for Race: {0}, Class: {1}, Level: {2}", objCharacter.Race, objCharacter.Classe, objCharacter.Level)
            End If

            WorldDatabase.Query(String.Format("SELECT * FROM player_classlevelstats WHERE class = {0} AND level = {1};", CType(objCharacter.Classe, Integer), CType(objCharacter.Level, Integer)), ClassLevelStats)
            If ClassLevelStats.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in player_classlevelstats table for Class: {0}, Level: {1}", objCharacter.Classe, objCharacter.Level)
            End If

            ' Initialize Character Variables
            objCharacter.Copper = 0
            objCharacter.XP = 0
            objCharacter.Size = 1.0F
            objCharacter.Life.Base = 0
            objCharacter.Life.Current = 0
            objCharacter.Mana.Base = 0
            objCharacter.Mana.Current = 0
            objCharacter.Rage.Current = 0
            objCharacter.Rage.Base = 0
            objCharacter.Energy.Current = 0
            objCharacter.Energy.Base = 0
            objCharacter.ManaType = GetClassManaType(objCharacter.Classe)

            ' Set Character Create Information
            objCharacter.Model = GetRaceModel(objCharacter.Race, objCharacter.Gender)

            objCharacter.Faction = CharRaces(objCharacter.Race).FactionID
            objCharacter.MapID = CreateInfo.Rows(0).Item("map")
            objCharacter.ZoneID = CreateInfo.Rows(0).Item("zone")
            objCharacter.positionX = CreateInfo.Rows(0).Item("position_x")
            objCharacter.positionY = CreateInfo.Rows(0).Item("position_y")
            objCharacter.positionZ = CreateInfo.Rows(0).Item("position_z")
            objCharacter.orientation = CreateInfo.Rows(0).Item("orientation")
            objCharacter.bindpoint_map_id = objCharacter.MapID
            objCharacter.bindpoint_zone_id = objCharacter.ZoneID
            objCharacter.bindpoint_positionX = objCharacter.positionX
            objCharacter.bindpoint_positionY = objCharacter.positionY
            objCharacter.bindpoint_positionZ = objCharacter.positionZ
            objCharacter.Strength.Base = LevelStats.Rows(0).Item("str")
            objCharacter.Agility.Base = LevelStats.Rows(0).Item("agi")
            objCharacter.Stamina.Base = LevelStats.Rows(0).Item("sta")
            objCharacter.Intellect.Base = LevelStats.Rows(0).Item("inte")
            objCharacter.Spirit.Base = LevelStats.Rows(0).Item("spi")
            objCharacter.Life.Base = ClassLevelStats.Rows(0).Item("basehp")
            objCharacter.Life.Current = objCharacter.Life.Maximum

            Select Case objCharacter.ManaType
                Case ManaTypes.TYPE_MANA
                    objCharacter.Mana.Base = ClassLevelStats.Rows(0).Item("basemana")
                    objCharacter.Mana.Current = objCharacter.Mana.Maximum
                Case ManaTypes.TYPE_RAGE
                    objCharacter.Rage.Base = ClassLevelStats.Rows(0).Item("basemana")
                    objCharacter.Rage.Current = 0
                Case ManaTypes.TYPE_ENERGY
                    objCharacter.Energy.Base = ClassLevelStats.Rows(0).Item("basemana")
                    objCharacter.Energy.Current = 0
            End Select

            'TODO: Get damage min and maximum
            objCharacter.Damage.Minimum = 5
            objCharacter.Damage.Maximum = 10

            ' Set Player Create Skills
            For Each SkillRow As DataRow In CreateInfoSkills.Rows
                objCharacter.LearnSkill(SkillRow.Item("Skill"), SkillRow.Item("SkillMin"), SkillRow.Item("SkillMax"))
            Next

            ' Set Player Taxi Zones
            For i As Integer = 0 To 31
                If (CharRaces(objCharacter.Race).TaxiMask And (1 << i)) Then
                    objCharacter.TaxiZones.Set(i + 1, True)
                End If
            Next

            ' Set Player Create Action Buttons
            For Each BarRow As DataRow In CreateInfoBars.Rows
                If BarRow.Item("action") > 0 Then
                    ButtonPos = BarRow.Item("button")
                    objCharacter.ActionButtons(ButtonPos) = New TActionButton(BarRow.Item("action"), BarRow.Item("type"), 0)
                End If
            Next

        End Sub

        Public Sub CreateCharacterSpells(ByRef objCharacter As CharacterObject)
            Dim CreateInfoSpells As New DataTable

            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo_spell WHERE race = {0} AND class = {1};", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer)), CreateInfoSpells)
            If CreateInfoSpells.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in playercreateinfo_spell table Race: {0}, Class: {1}", objCharacter.Race, objCharacter.Classe)
            End If

            ' Set Player Create Spells
            For Each SpellRow As DataRow In CreateInfoSpells.Rows
                objCharacter.LearnSpell(SpellRow.Item("Spell"))
            Next
        End Sub

        Public Sub CreateCharacterItems(ByRef objCharacter As CharacterObject)

            Dim CreateInfoItems As New DataTable
            WorldDatabase.Query(String.Format("SELECT * FROM playercreateinfo_item WHERE race = {0} AND class = {1};", CType(objCharacter.Race, Integer), CType(objCharacter.Classe, Integer)), CreateInfoItems)
            If CreateInfoItems.Rows.Count <= 0 Then
                Log.WriteLine(LogType.FAILED, "No information found in playercreateinfo_item table for Race: {0}, Class: {1}", objCharacter.Race, objCharacter.Classe)
            End If

            ' Set Player Create Items
            Dim Items As New Dictionary(Of Integer, Integer)
            Dim Used As New List(Of Integer)

            For Each ItemRow As DataRow In CreateInfoItems.Rows
                Items.Add(ItemRow.Item("itemid"), ItemRow.Item("amount"))
            Next

            'First add bags
            For Each Item As KeyValuePair(Of Integer, Integer) In Items
                If ITEMDatabase.ContainsKey(Item.Key) = False Then
                    Dim newItem As New WS_Items.ItemInfo(Item.Key)
                    'The New does a an add to the .Containskey collection above
                End If

                If ITEMDatabase(Item.Key).ContainerSlots > 0 Then
                    Dim Slots() As Byte = ITEMDatabase(Item.Key).GetSlots
                    For Each tmpSlot As Byte In Slots
                        If Not objCharacter.Items.ContainsKey(tmpSlot) Then
                            objCharacter.ItemADD(Item.Key, 0, tmpSlot, Item.Value)
                            Used.Add(Item.Key)
                            Exit For
                        End If
                    Next
                End If
            Next

            'Then add the rest of the items
            For Each Item As KeyValuePair(Of Integer, Integer) In Items
                If Used.Contains(Item.Key) Then Continue For

                Dim Slots() As Byte = ITEMDatabase(Item.Key).GetSlots
                For Each tmpSlot As Byte In Slots
                    If Not objCharacter.Items.ContainsKey(tmpSlot) Then
                        objCharacter.ItemADD(Item.Key, 0, tmpSlot, Item.Value)
                        GoTo NextItem
                    End If
                Next
                objCharacter.ItemADD(Item.Key, 255, 255, Item.Value)
                NextItem:
            Next

        End Sub
    End Module
End NameSpace