' 
' Copyright (C) 2011 SpuriousZero <http://www.spuriousemu.com/>
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

Imports System
Imports System.IO
Imports SpuriousZero.Common
Imports SpuriousZero.Common.BaseWriter
Imports SpuriousZero.WorldServer
Imports SpuriousZero.Common.DBC

Public Module WS_DBCLoad

#Region "Spells"
    Public Sub InitializeSpellRadius()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellRadius.dbc")

            Dim radiusID As Integer
            Dim radiusValue As Single
            Dim radiusValue2 As Single

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                radiusID = tmpDBC.Item(i, 0)
                radiusValue = tmpDBC.Item(i, 1, DBC.DBCValueType.DBC_FLOAT)
                radiusValue2 = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_FLOAT) ' May be needed in the future

                SpellRadius(radiusID) = radiusValue
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellRadius initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellRadius missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub InitializeSpellCastTime()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellCastTimes.dbc")

            Dim spellCastID As Integer
            Dim spellCastTimeS As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                spellCastID = tmpDBC.Item(i, 0)
                spellCastTimeS = tmpDBC.Item(i, 1)

                SpellCastTime(spellCastID) = spellCastTimeS
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellCastTimes initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellCastTimes missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub InitializeSpellRange()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellRange.dbc")

            Dim spellRangeIndex As Integer
            Dim spellRangeMin As Single
            Dim spellRangeMax As Single

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                spellRangeIndex = tmpDBC.Item(i, 0)
                spellRangeMin = tmpDBC.Item(i, 1, DBC.DBCValueType.DBC_FLOAT) ' Added back may be needed in the future
                spellRangeMax = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_FLOAT)

                SpellRange(spellRangeIndex) = spellRangeMax
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellRanges initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellRanges missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub InitializeSpellShapeShift()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellShapeshiftForm.dbc")

            Dim ID As Integer
            Dim Flags1 As Integer
            Dim CreatureType As Integer
            Dim AttackSpeed As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                ID = tmpDBC.Item(i, 0)
                Flags1 = tmpDBC.Item(i, 11)
                CreatureType = tmpDBC.Item(i, 12)
                AttackSpeed = tmpDBC.Item(i, 13)

                SpellShapeShiftForm.Add(New TSpellShapeshiftForm(ID, Flags1, CreatureType, AttackSpeed))
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellShapeshiftForms initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellShapeshiftForms missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub InitializeSpellFocusObject()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellFocusObject.dbc")

            Dim spellFocusIndex As Integer
            Dim spellFocusObjectName As String

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                spellFocusIndex = tmpDBC.Item(i, 0)
                spellFocusObjectName = tmpDBC.Item(i, 1, DBC.DBCValueType.DBC_STRING)

                SpellFocusObject(spellFocusIndex) = spellFocusObjectName
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellFocusObjects initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellFocusObjects missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub InitializeSpellDuration()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellDuration.dbc")

            Dim SpellDurationIndex As Integer
            Dim SpellDurationValue As Integer
            Dim SpellDurationValue2 As Integer
            Dim SpellDurationValue3 As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                SpellDurationIndex = tmpDBC.Item(i, 0)
                SpellDurationValue = tmpDBC.Item(i, 1)
                SpellDurationValue2 = tmpDBC.Item(i, 2) ' May be needed in the future
                SpellDurationValue3 = tmpDBC.Item(i, 3) ' May be needed in the future

                SpellDuration(SpellDurationIndex) = SpellDurationValue
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellDurations initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellDurations missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeSpells()
        Try
            Dim SpellDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Spell.dbc")
            'Console.WriteLine("[" & Format(TimeOfDay, "HH:mm:ss") & "] " & SpellDBC.GetFileInformation)
            Log.WriteLine(LogType.INFORMATION, "DBC: Initializing Spells - This may take a few moments....")

            Dim i As Long
            Dim ID As Integer
            For i = 0 To SpellDBC.Rows - 1
                Try
                    ID = SpellDBC.Item(i, 0)
                    SPELLs(ID) = New SpellInfo
                    SPELLs(ID).ID = ID
                    SPELLs(ID).School = SpellDBC.Item(i, 1)
                    SPELLs(ID).Category = SpellDBC.Item(i, 2)
                    ' 3 = Not Used
                    SPELLs(ID).DispellType = SpellDBC.Item(i, 4)
                    SPELLs(ID).Mechanic = SpellDBC.Item(i, 5)
                    SPELLs(ID).Attributes = SpellDBC.Item(i, 6)
                    SPELLs(ID).AttributesEx = SpellDBC.Item(i, 7)
                    SPELLs(ID).AttributesEx2 = SpellDBC.Item(i, 8)
                    ' AttributesEx3 = SpellDBC.Item(i, 9)
                    ' AttributesEx4 = SpellDBC.Item(i, 10)
                    SPELLs(ID).RequredCasterStance = SpellDBC.Item(i, 11) ' RequiredShapeShift
                    SPELLs(ID).ShapeshiftExclude = SpellDBC.Item(i, 12)
                    SPELLs(ID).Target = SpellDBC.Item(i, 13)
                    SPELLs(ID).TargetCreatureType = SpellDBC.Item(i, 14)
                    SPELLs(ID).FocusObjectIndex = SpellDBC.Item(i, 15)
                    SPELLs(ID).CasterAuraState = SpellDBC.Item(i, 16)
                    SPELLs(ID).TargetAuraState = SpellDBC.Item(i, 17)
                    SPELLs(ID).SpellCastTimeIndex = SpellDBC.Item(i, 18)
                    SPELLs(ID).SpellCooldown = SpellDBC.Item(i, 19)
                    SPELLs(ID).CategoryCooldown = SpellDBC.Item(i, 20)
                    SPELLs(ID).interruptFlags = SpellDBC.Item(i, 21)
                    SPELLs(ID).auraInterruptFlags = SpellDBC.Item(i, 22)
                    SPELLs(ID).channelInterruptFlags = SpellDBC.Item(i, 23)
                    SPELLs(ID).procFlags = SpellDBC.Item(i, 24)
                    SPELLs(ID).procChance = SpellDBC.Item(i, 25)
                    SPELLs(ID).procCharges = SpellDBC.Item(i, 26)
                    SPELLs(ID).maxLevel = SpellDBC.Item(i, 27)
                    SPELLs(ID).baseLevel = SpellDBC.Item(i, 28)
                    SPELLs(ID).spellLevel = SpellDBC.Item(i, 29)
                    SPELLs(ID).DurationIndex = SpellDBC.Item(i, 30)
                    SPELLs(ID).powerType = SpellDBC.Item(i, 31)
                    SPELLs(ID).manaCost = SpellDBC.Item(i, 32)
                    SPELLs(ID).manaCostPerlevel = SpellDBC.Item(i, 33)
                    SPELLs(ID).manaPerSecond = SpellDBC.Item(i, 34)
                    SPELLs(ID).manaPerSecondPerLevel = SpellDBC.Item(i, 35)
                    SPELLs(ID).rangeIndex = SpellDBC.Item(i, 36)
                    SPELLs(ID).Speed = SpellDBC.Item(i, 37, DBC.DBCValueType.DBC_FLOAT)
                    SPELLs(ID).modalNextSpell = SpellDBC.Item(i, 38) ' Not Used
                    SPELLs(ID).maxStack = SpellDBC.Item(i, 39)
                    SPELLs(ID).Totem(0) = SpellDBC.Item(i, 40)
                    SPELLs(ID).Totem(1) = SpellDBC.Item(i, 41)

                    '-CORRECT-
                    SPELLs(ID).Reagents(0) = SpellDBC.Item(i, 42)
                    SPELLs(ID).Reagents(1) = SpellDBC.Item(i, 43)
                    SPELLs(ID).Reagents(2) = SpellDBC.Item(i, 44)
                    SPELLs(ID).Reagents(3) = SpellDBC.Item(i, 45)
                    SPELLs(ID).Reagents(4) = SpellDBC.Item(i, 46)
                    SPELLs(ID).Reagents(5) = SpellDBC.Item(i, 47)
                    SPELLs(ID).Reagents(6) = SpellDBC.Item(i, 48)
                    SPELLs(ID).Reagents(7) = SpellDBC.Item(i, 49)

                    SPELLs(ID).ReagentsCount(0) = SpellDBC.Item(i, 50)
                    SPELLs(ID).ReagentsCount(1) = SpellDBC.Item(i, 51)
                    SPELLs(ID).ReagentsCount(2) = SpellDBC.Item(i, 52)
                    SPELLs(ID).ReagentsCount(3) = SpellDBC.Item(i, 53)
                    SPELLs(ID).ReagentsCount(4) = SpellDBC.Item(i, 54)
                    SPELLs(ID).ReagentsCount(5) = SpellDBC.Item(i, 55)
                    SPELLs(ID).ReagentsCount(6) = SpellDBC.Item(i, 56)
                    SPELLs(ID).ReagentsCount(7) = SpellDBC.Item(i, 57)
                    '-/CORRECT-

                    SPELLs(ID).EquippedItemClass = SpellDBC.Item(i, 58) 'Value
                    SPELLs(ID).EquippedItemSubClass = SpellDBC.Item(i, 59) 'Mask
                    SPELLs(ID).EquippedItemInventoryType = SpellDBC.Item(i, 60) 'Mask

                    For j As Integer = 0 To 2
                        If CInt(SpellDBC.Item(i, 61 + j)) <> 0 Then
                            SPELLs(ID).SpellEffects(j) = New SpellEffect(SPELLs(ID))

                            SPELLs(ID).SpellEffects(j).ID = SpellDBC.Item(i, 61 + j)
                            SPELLs(ID).SpellEffects(j).valueDie = SpellDBC.Item(i, 64 + j)
                            SPELLs(ID).SpellEffects(j).diceBase = SpellDBC.Item(i, 67 + j)
                            SPELLs(ID).SpellEffects(j).dicePerLevel = SpellDBC.Item(i, 70 + j, DBCValueType.DBC_FLOAT)
                            SPELLs(ID).SpellEffects(j).valuePerLevel = SpellDBC.Item(i, 73 + j, DBC.DBCValueType.DBC_FLOAT)
                            SPELLs(ID).SpellEffects(j).valueBase = SpellDBC.Item(i, 76 + j)
                            SPELLs(ID).SpellEffects(j).Mechanic = SpellDBC.Item(i, 79 + j)
                            SPELLs(ID).SpellEffects(j).implicitTargetA = SpellDBC.Item(i, 82 + j)
                            SPELLs(ID).SpellEffects(j).implicitTargetB = SpellDBC.Item(i, 85 + j)
                            SPELLs(ID).SpellEffects(j).RadiusIndex = SpellDBC.Item(i, 88 + j) ' spellradius.dbc
                            SPELLs(ID).SpellEffects(j).ApplyAuraIndex = SpellDBC.Item(i, 91 + j)
                            SPELLs(ID).SpellEffects(j).Amplitude = SpellDBC.Item(i, 94 + j)
                            SPELLs(ID).SpellEffects(j).MultipleValue = SpellDBC.Item(i, 97 + j)
                            SPELLs(ID).SpellEffects(j).ChainTarget = SpellDBC.Item(i, 100 + j)
                            SPELLs(ID).SpellEffects(j).ItemType = SpellDBC.Item(i, 103 + j)
                            SPELLs(ID).SpellEffects(j).MiscValue = SpellDBC.Item(i, 106 + j)
                            SPELLs(ID).SpellEffects(j).TriggerSpell = SpellDBC.Item(i, 109 + j)
                            SPELLs(ID).SpellEffects(j).valuePerComboPoint = SpellDBC.Item(i, 112 + j)
                        Else
                            SPELLs(ID).SpellEffects(j) = Nothing
                        End If
                    Next

                    SPELLs(ID).SpellVisual = SpellDBC.Item(i, 115)
                    '116 = Always zero? - SpellVisual2 - Not Used
                    SPELLs(ID).SpellIconID = SpellDBC.Item(i, 117)
                    SPELLs(ID).ActiveIconID = SpellDBC.Item(i, 118)
                    '119 = spellPriority
                    SPELLs(ID).Name = SpellDBC.Item(i, 120, DBCValueType.DBC_STRING)
                    '121 = Always zero?
                    '122 = Always zero?
                    '123 = Always zero?
                    '124 = Always zero?
                    '125 = Always zero?
                    '126 = Always zero?
                    '127 = Always zero?
                    '128 = Always zero?
                    SPELLs(ID).Rank = SpellDBC.Item(i, 129, DBCValueType.DBC_STRING)
                    '130 = Always zero?
                    '131 = Always zero?
                    '132 = Always zero?
                    '133 = Always zero?
                    '134 = Always zero?
                    '135 = Always zero?
                    '136 = Always zero?
                    '137 = RankFlags
                    '138 = Description - Not Used
                    '139 = Always zero?
                    '140 = Always zero?
                    '141 = Always zero?
                    '142 = Always zero?
                    '143 = Always zero?
                    '144 = Always zero?
                    '145 = Always zero?
                    '146 = DescriptionFlags - Not Used
                    '147 = ToolTip - Not USed
                    '148 = Always zero?
                    '149 = Always zero?
                    '150 = Always zero?
                    '151 = Always zero?
                    '152 = Always zero?
                    '153 = Always zero?
                    '154 = Always zero?
                    '155 = ToolTipFlags - Not Used
                    SPELLs(ID).manaCostPercent = SpellDBC.Item(i, 156)
                    SPELLs(ID).StartRecoveryCategory = SpellDBC.Item(i, 157)
                    SPELLs(ID).StartRecoveryTime = SpellDBC.Item(i, 158)
                    SPELLs(ID).AffectedTargetLevel = SpellDBC.Item(i, 159)
                    SPELLs(ID).SpellFamilyName = SpellDBC.Item(i, 160)
                    SPELLs(ID).SpellFamilyFlags = SpellDBC.Item(i, 161) ' ClassFamilyMask SpellFamilyFlags;                   // 161+162
                    SPELLs(ID).MaxTargets = SpellDBC.Item(i, 163)
                    SPELLs(ID).DamageType = SpellDBC.Item(i, 164) ' defenseType
                    'SPELLs(ID).PreventionType = SpellDBC.Item(i, 165)
                    '166 = StanceBarOrder - Not Used

                    For j As Integer = 0 To 2
                        If SPELLs(ID).SpellEffects(j) IsNot Nothing Then
                            SPELLs(ID).SpellEffects(j).DamageMultiplier = SpellDBC.Item(i, 167 + j, DBCValueType.DBC_FLOAT)
                        End If
                    Next

                    '170 = MinFactionId - Not Used
                    '171 = MinReputation - Not Used
                    '172 = RequiredAuraVision - Not Used

                    SPELLs(ID).InitCustomAttributes()

                Catch e As Exception
                    Log.WriteLine(LogType.FAILED, "Line {0} caused error: {1}", i, e.ToString)
                End Try

            Next i

            SpellDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Spells initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Spells missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeSpellChains()
        Try
            Dim SpellChainQuery As New DataTable
            WorldDatabase.Query("SELECT spell_id, prev_spell FROM spell_chain", SpellChainQuery)

            For Each SpellChain As DataRow In SpellChainQuery.Rows
                SpellChains.Add(CInt(SpellChain.Item("spell_id")), CInt(SpellChain.Item("prev_spell")))
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} SpellChains initialized.", SpellChainQuery.Rows.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : SpellChains missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Taxi"
    Public Sub InitializeTaxiNodes()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\TaxiNodes.dbc")

            Dim taxiPosX As Single
            Dim taxiPosY As Single
            Dim taxiPosZ As Single
            Dim taxiMapID As Integer
            Dim taxiNode As Integer
            Dim taxiMountType_Horde As Integer
            Dim taxiMountType_Alliance As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                taxiNode = tmpDBC.Item(i, 0)
                taxiMapID = tmpDBC.Item(i, 1)
                taxiPosX = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_FLOAT)
                taxiPosY = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_FLOAT)
                taxiPosZ = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_FLOAT)
                taxiMountType_Horde = tmpDBC.Item(i, 14)
                taxiMountType_Alliance = tmpDBC.Item(i, 15)

                If Config.Maps.Contains(taxiMapID.ToString) Then
                    TaxiNodes.Add(taxiNode, New TTaxiNode(taxiPosX, taxiPosY, taxiPosZ, taxiMapID, taxiMountType_Horde, taxiMountType_Alliance))
                End If
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} TaxiNodes initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : TaxiNodes missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeTaxiPaths()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\TaxiPath.dbc")

            Dim taxiNode As Integer
            Dim taxiFrom As Integer
            Dim taxiTo As Integer
            Dim taxiPrice As Integer
            Dim i As Integer = 0

            For i = 0 To tmpDBC.Rows - 1
                taxiNode = tmpDBC.Item(i, 0)
                taxiFrom = tmpDBC.Item(i, 1)
                taxiTo = tmpDBC.Item(i, 2)
                taxiPrice = tmpDBC.Item(i, 3)

                TaxiPaths.Add(taxiNode, New TTaxiPath(taxiFrom, taxiTo, taxiPrice))

            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} TaxiPaths initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : TaxiPath missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeTaxiPathNodes()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\TaxiPathNode.dbc")

            Dim taxiNode As Integer
            Dim taxiPath As Integer
            Dim taxiSeq As Integer
            Dim taxiMapID As Integer
            Dim taxiPosX As Single
            Dim taxiPosY As Single
            Dim taxiPosZ As Single
            Dim taxiAction As Integer
            Dim taxiWait As Integer
            Dim i As Integer = 0

            For i = 0 To tmpDBC.Rows - 1
                taxiNode = tmpDBC.Item(i, 0)
                taxiPath = tmpDBC.Item(i, 1)
                taxiSeq = tmpDBC.Item(i, 2)
                taxiMapID = tmpDBC.Item(i, 3)
                taxiPosX = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_FLOAT)
                taxiPosY = tmpDBC.Item(i, 5, DBC.DBCValueType.DBC_FLOAT)
                taxiPosZ = tmpDBC.Item(i, 6, DBC.DBCValueType.DBC_FLOAT)
                taxiAction = tmpDBC.Item(i, 7)
                taxiWait = tmpDBC.Item(i, 8)

                If Config.Maps.Contains(taxiMapID.ToString) Then
                    If TaxiPathNodes.ContainsKey(taxiPath) = False Then
                        TaxiPathNodes.Add(taxiPath, New Dictionary(Of Integer, TTaxiPathNode))
                    End If
                    TaxiPathNodes(taxiPath).Add(taxiSeq, New TTaxiPathNode(taxiPosX, taxiPosY, taxiPosZ, taxiMapID, taxiPath, taxiSeq, taxiAction, taxiWait))
                End If
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} TaxiPathNodes initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : TaxiPathNode missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "GraveYards"
    Public Sub InitializeGraveyards()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\WorldSafeLocs.dbc")

            Dim locationPosX As Single
            Dim locationPosY As Single
            Dim locationPosZ As Single
            Dim locationMapID As Integer
            Dim locationIndex As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                locationIndex = tmpDBC.Item(i, 0)
                locationMapID = tmpDBC.Item(i, 1)
                locationPosX = tmpDBC.Item(i, 2, DBC.DBCValueType.DBC_FLOAT)
                locationPosY = tmpDBC.Item(i, 3, DBC.DBCValueType.DBC_FLOAT)
                locationPosZ = tmpDBC.Item(i, 4, DBC.DBCValueType.DBC_FLOAT)

                If Config.Maps.Contains(locationMapID.ToString) Then
                    Graveyards.Add(locationIndex, New TGraveyard(locationPosX, locationPosY, locationPosZ, locationMapID))
                End If
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Graveyards initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : WorldSafeLocs missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Skills"
    Public Sub InitializeSkillLines()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SkillLine.dbc")

            Dim skillID As Integer
            Dim skillLine As Integer
            'Dim skillUnk1 As Integer
            Dim skillName As String
            Dim skillDescription As String
            Dim skillSpellIcon As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                skillID = tmpDBC.Item(i, 0)
                skillLine = tmpDBC.Item(i, 1) ' Type or Category?
                'skillUnk1 = tmpDBC.Item(i, 2) ' May be needed in the future
                'skillName = tmpDBC.Item(i, 3) ' May be needed in the future
                skillName = tmpDBC.Item(i, 3, DBCValueType.DBC_STRING)
                skillDescription = tmpDBC.Item(i, 12, DBCValueType.DBC_STRING)
                skillSpellIcon = tmpDBC.Item(i, 21)

                SkillLines(skillID) = skillLine
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SkillLines initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SkillLines missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeSkillLineAbility()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SkillLineAbility.dbc")

            Dim tmpSkillLineAbility As New TSkillLineAbility

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                tmpSkillLineAbility = New TSkillLineAbility

                tmpSkillLineAbility.ID = tmpDBC.Item(i, 0)
                tmpSkillLineAbility.SkillID = tmpDBC.Item(i, 1)
                tmpSkillLineAbility.SpellID = tmpDBC.Item(i, 2)
                tmpSkillLineAbility.Unknown1 = tmpDBC.Item(i, 3) ' May be needed in the future
                tmpSkillLineAbility.Unknown2 = tmpDBC.Item(i, 4) ' May be needed in the future
                tmpSkillLineAbility.Unknown3 = tmpDBC.Item(i, 5) ' May be needed in the future
                tmpSkillLineAbility.Unknown4 = tmpDBC.Item(i, 6) ' May be needed in the future
                tmpSkillLineAbility.Required_Skill_Value = tmpDBC.Item(i, 7)
                tmpSkillLineAbility.Forward_SpellID = tmpDBC.Item(i, 8)
                tmpSkillLineAbility.Unknown5 = tmpDBC.Item(i, 9) ' May be needed in the future
                tmpSkillLineAbility.Max_Value = tmpDBC.Item(i, 10)
                tmpSkillLineAbility.Min_Value = tmpDBC.Item(i, 11)

                SkillLineAbility.Add(tmpSkillLineAbility.ID, tmpSkillLineAbility)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SkillLineAbilitys initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SkillLineAbility missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

#End Region
#Region "Locks"
    Public Sub InitializeLocks()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Lock.dbc")

            Dim lockID As Integer
            Dim keyType(4) As Byte
            Dim key(4) As Integer
            Dim reqMining As Integer
            Dim reqLockSkill As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                lockID = tmpDBC.Item(i, 0)
                keyType(0) = CByte(tmpDBC.Item(i, 1))
                keyType(1) = CByte(tmpDBC.Item(i, 2))
                keyType(2) = CByte(tmpDBC.Item(i, 3))
                keyType(3) = CByte(tmpDBC.Item(i, 4))
                keyType(4) = CByte(tmpDBC.Item(i, 5))
                key(0) = tmpDBC.Item(i, 9)
                key(1) = tmpDBC.Item(i, 10)
                key(2) = tmpDBC.Item(i, 11)
                key(3) = tmpDBC.Item(i, 12)
                key(4) = tmpDBC.Item(i, 13)
                reqMining = tmpDBC.Item(i, 17) ' Not sure about this one leaving it like it is
                reqLockSkill = tmpDBC.Item(i, 17)

                Locks(lockID) = New TLock(keyType, key, reqMining, reqLockSkill)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Locks initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Locks missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "AreaTable"
    Public Sub InitializeAreaTable()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\AreaTable.dbc")

            Dim areaID As Integer
            'Dim areaMapID As Integer
            Dim areaExploreFlag As Integer
            Dim areaLevel As Integer
            Dim areaZone As Integer
            Dim areaZoneType As Integer
            'Dim areaEXP As Integer
            'Dim areaTeam As Integer
            'Dim areaName As String

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                areaID = tmpDBC.Item(i, 0)
                'areaMapID = tmpDBC.Item(i, 1) ' May be needed in the future
                areaZone = tmpDBC.Item(i, 2)
                areaExploreFlag = tmpDBC.Item(i, 3)
                areaZoneType = tmpDBC.Item(i, 4) ' 312 For Cities - Flags
                ' 5        m_SoundProviderPref
                ' 6        m_SoundProviderPrefUnderwater
                ' 7        m_AmbienceID
                'areaEXP = tmpDBC.Item(i, 8) ' May be needed in the future - m_ZoneMusic
                ' 9        m_IntroSound
                areaLevel = tmpDBC.Item(i, 10)
                'areaName = tmpDBC.Item(i, 11) ' May be needed in the future
                ' 19 string flags
                'areaTeam = tmpDBC.Item(i, 20)
                ' 24 = LiquidTypeOverride

                If areaLevel > 255 Then areaLevel = 255
                If areaLevel < 0 Then areaLevel = 0

                AreaTable(areaExploreFlag) = New TArea
                AreaTable(areaExploreFlag).ID = areaID
                AreaTable(areaExploreFlag).Level = areaLevel
                'AreaTable(areaExploreFlag).Name = areaName
                AreaTable(areaExploreFlag).Zone = areaZone
                AreaTable(areaExploreFlag).ZoneType = areaZoneType
                'AreaTable(areaExploreFlag).Team = areaTeam
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Areas initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : AreaTable missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Emotes"
    Public Sub InitializeEmotes()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Emotes.dbc")
            Dim EmoteID As Integer
            Dim EmoteState As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                EmoteID = tmpDBC.Item(i, 0)
                EmoteState = tmpDBC.Item(i, 4)

                If EmoteID <> 0 Then EmotesState(EmoteID) = EmoteState
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Emotes initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Emotes missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeEmotesText()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\EmotesText.dbc")
            Dim textEmoteID As Integer
            Dim EmoteID As Integer
            'Dim EmoteID2 As Integer
            'Dim EmoteID3 As Integer
            'Dim EmoteID4 As Integer
            'Dim EmoteID5 As Integer
            'Dim EmoteID6 As Integer

            Dim i As Integer = 0
            For i = 0 To tmpDBC.Rows - 1
                textEmoteID = tmpDBC.Item(i, 0)
                EmoteID = tmpDBC.Item(i, 2)
                'EmoteID2 = tmpDBC.Item(i, 3) ' May be needed in the future
                'EmoteID3 = tmpDBC.Item(i, 4) ' May be needed in the future
                'EmoteID4 = tmpDBC.Item(i, 5) ' May be needed in the future
                'EmoteID5 = tmpDBC.Item(i, 7) ' May be needed in the future
                'EmoteID6 = tmpDBC.Item(i, 8) ' May be needed in the future

                If EmoteID <> 0 Then EmotesText(textEmoteID) = EmoteID
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} EmotesText initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : EmotesText missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Factions"
    Public Sub InitializeFactions()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Faction.dbc")

            Dim factionID As Integer
            Dim factionFlag As Integer
            Dim Flags(3) As Integer
            Dim ReputationStats(3) As Integer
            Dim ReputationFlags(3) As Integer
            Dim factionTeam As Integer
            'Dim factionName As String

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                factionID = tmpDBC.Item(i, 0)
                factionFlag = tmpDBC.Item(i, 1)
                Flags(0) = tmpDBC.Item(i, 2)
                Flags(1) = tmpDBC.Item(i, 3)
                Flags(2) = tmpDBC.Item(i, 4)
                Flags(3) = tmpDBC.Item(i, 5)
                ReputationStats(0) = tmpDBC.Item(i, 10)
                ReputationStats(1) = tmpDBC.Item(i, 11)
                ReputationStats(2) = tmpDBC.Item(i, 12)
                ReputationStats(3) = tmpDBC.Item(i, 13)
                ReputationFlags(0) = tmpDBC.Item(i, 14)
                ReputationFlags(1) = tmpDBC.Item(i, 15)
                ReputationFlags(2) = tmpDBC.Item(i, 16)
                ReputationFlags(3) = tmpDBC.Item(i, 17)
                factionTeam = tmpDBC.Item(i, 18)
                'factionName = tmpDBC.Item(i, 19) ' May be needed in the future

                FactionInfo(factionID) = New WS_DBCDatabase.TFaction(factionID, factionFlag, _
                   Flags(0), Flags(1), Flags(2), Flags(3), _
                   ReputationStats(0), ReputationStats(1), ReputationStats(2), ReputationStats(3), _
                   ReputationFlags(0), ReputationFlags(1), ReputationFlags(2), ReputationFlags(3))
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Factions initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Factions missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeFactionTemplates()
        Try
            Dim i As Integer

            'Loading from DBC
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\FactionTemplate.dbc")

            Dim templateID As Integer

            For i = 0 To tmpDBC.Rows - 1
                templateID = tmpDBC.Item(i, 0)
                FactionTemplatesInfo.Add(templateID, New TFactionTemplate)
                FactionTemplatesInfo(templateID).FactionID = tmpDBC.Item(i, 1)
                FactionTemplatesInfo(templateID).ourMask = tmpDBC.Item(i, 3)
                FactionTemplatesInfo(templateID).friendMask = tmpDBC.Item(i, 4)
                FactionTemplatesInfo(templateID).enemyMask = tmpDBC.Item(i, 5)
                FactionTemplatesInfo(templateID).enemyFaction1 = tmpDBC.Item(i, 6)
                FactionTemplatesInfo(templateID).enemyFaction2 = tmpDBC.Item(i, 7)
                FactionTemplatesInfo(templateID).enemyFaction3 = tmpDBC.Item(i, 8)
                FactionTemplatesInfo(templateID).enemyFaction4 = tmpDBC.Item(i, 9)
                FactionTemplatesInfo(templateID).friendFaction1 = tmpDBC.Item(i, 10)
                FactionTemplatesInfo(templateID).friendFaction2 = tmpDBC.Item(i, 11)
                FactionTemplatesInfo(templateID).friendFaction3 = tmpDBC.Item(i, 12)
                FactionTemplatesInfo(templateID).friendFaction4 = tmpDBC.Item(i, 13)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} FactionTemplates initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : FactionsTemplates missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeCharRaces()
        Try
            Dim i As Integer

            'Loading from DBC
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ChrRaces.dbc")

            Dim raceID As Integer
            Dim factionID As Integer
            Dim modelM As Integer
            Dim modelF As Integer
            Dim teamID As Integer '1 = Horde / 7 = Alliance
            Dim taxiMask As UInteger
            Dim cinematicID As Integer
            Dim name As String

            For i = 0 To tmpDBC.Rows - 1
                raceID = tmpDBC.Item(i, 0)
                factionID = tmpDBC.Item(i, 2)
                modelM = tmpDBC.Item(i, 4)
                modelF = tmpDBC.Item(i, 5)
                teamID = tmpDBC.Item(i, 8)
                taxiMask = tmpDBC.Item(i, 14)
                cinematicID = tmpDBC.Item(i, 16)
                name = tmpDBC.Item(i, 17, DBCValueType.DBC_STRING)

                CharRaces(CByte(raceID)) = New TCharRace(CShort(factionID), modelM, modelF, CByte(teamID), taxiMask, cinematicID, name)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} CharRaces initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : CharRaces missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub InitializeCharClasses()
        Try
            Dim i As Integer

            'Loading from DBC
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ChrClasses.dbc")

            Dim classID As Integer
            Dim cinematicID As Integer

            For i = 0 To tmpDBC.Rows - 1
                classID = tmpDBC.Item(i, 0)
                cinematicID = tmpDBC.Item(i, 5)

                CharClasses(CByte(classID)) = New TCharClass(cinematicID)
            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} CharClasses initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : CharRaces missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "DurabilityCosts"
    Public Sub InitializeDurabilityCosts()
        Try
            Dim tmpDBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\DurabilityCosts.dbc")

            Dim itemBroken As Integer
            Dim itemType As Integer
            Dim itemPrice As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                itemBroken = tmpDBC.Item(i, 0)

                For itemType = 1 To tmpDBC.Columns - 1
                    itemPrice = tmpDBC.Item(i, itemType)
                    DurabilityCosts(itemBroken, itemType - 1) = itemPrice
                Next

            Next i

            tmpDBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} DurabilityCosts initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : DurabilityCosts missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Talents"
    Public Sub LoadTalentDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\Talent.dbc")

            Dim tmpInfo As TalentInfo

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                tmpInfo = New TalentInfo

                tmpInfo.TalentID = DBC.Item(i, 0)
                tmpInfo.TalentTab = DBC.Item(i, 1)
                tmpInfo.Row = DBC.Item(i, 2)
                tmpInfo.Col = DBC.Item(i, 3)
                tmpInfo.RankID(0) = DBC.Item(i, 4)
                tmpInfo.RankID(1) = DBC.Item(i, 5)
                tmpInfo.RankID(2) = DBC.Item(i, 6)
                tmpInfo.RankID(3) = DBC.Item(i, 7)
                tmpInfo.RankID(4) = DBC.Item(i, 8)

                tmpInfo.RequiredTalent(0) = DBC.Item(i, 13) ' dependson
                'tmpInfo.RequiredTalent(1) = DBC.Item(i, 14) ' ???
                'tmpInfo.RequiredTalent(2) = DBC.Item(i, 15) ' ???
                tmpInfo.RequiredPoints(0) = DBC.Item(i, 16) ' dependsonrank
                'tmpInfo.RequiredPoints(1) = DBC.Item(i, 17) ' ???
                'tmpInfo.RequiredPoints(2) = DBC.Item(i, 18) ' ???

                Talents.Add(tmpInfo.TalentID, tmpInfo)
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Talents initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : Talents missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub LoadTalentTabDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\TalentTab.dbc")

            Dim TalentTab As Integer
            Dim TalentMask As Integer
            Dim TalentTabPage As Integer

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                TalentTab = DBC.Item(i, 0)
                TalentMask = DBC.Item(i, 12)
                TalentTabPage = DBC.Item(i, 13) ' May be needed in the future

                TalentsTab.Add(TalentTab, TalentMask)
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} Talent tabs initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : TalentTab missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "AuctionHouse"
    Public Sub LoadAuctionHouseDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\AuctionHouse.dbc")

            Dim AHId As Integer
            Dim unk As Integer
            Dim fee As Integer
            Dim tax As Integer

            'What the hell is this doing? o_O

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                AHId = DBC.Item(i, 0)
                unk = DBC.Item(i, 1)
                fee = DBC.Item(i, 2)
                tax = DBC.Item(i, 3)

                AuctionID = AHId
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} AuctionHouses initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : AuctionHouse missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

#End Region
#Region "Items"
    Public Sub LoadSpellItemEnchantments()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\SpellItemEnchantment.dbc")

            Dim ID As Integer
            Dim Type(2) As Integer
            Dim Amount(2) As Integer
            Dim SpellID(2) As Integer
            Dim AuraID As Integer
            Dim Slot As Integer
            'Dim EnchantmentConditions As Integer

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                ID = DBC.Item(i, 0)
                Type(0) = DBC.Item(i, 1)
                Type(1) = DBC.Item(i, 2)
                'Type(2) = DBC.Item(i, 3)
                Amount(0) = DBC.Item(i, 4)
                Amount(1) = DBC.Item(i, 7)
                'Amount(2) = DBC.Item(i, 6)
                SpellID(0) = DBC.Item(i, 10)
                SpellID(1) = DBC.Item(i, 11)
                'SpellID(2) = DBC.Item(i, 12)
                AuraID = DBC.Item(i, 22)
                Slot = DBC.Item(i, 23)
                'EnchantmentConditions = DBC.Item(i, 23) ' TODO: Correct?

                SpellItemEnchantments.Add(ID, New TSpellItemEnchantment(Type, Amount, SpellID, AuraID, Slot)) ', EnchantmentConditions))
            Next

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} SpellItemEnchantments initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : SpellItemEnchantments missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub LoadItemSet()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ItemSet.dbc")

            Dim ID As Integer
            Dim Name As String
            Dim ItemID(7) As Integer
            Dim SpellID(7) As Integer
            Dim ItemCount(7) As Integer
            Dim Required_Skill_ID As Integer
            Dim Required_Skill_Value As Integer

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                ID = DBC.Item(i, 0)
                Name = DBC.Item(i, 1, DBCValueType.DBC_STRING)
                ItemID(0) = DBC.Item(i, 10) ' 10 - 26
                ItemID(1) = DBC.Item(i, 11)
                ItemID(2) = DBC.Item(i, 12)
                ItemID(3) = DBC.Item(i, 13)
                ItemID(4) = DBC.Item(i, 14)
                ItemID(5) = DBC.Item(i, 15)
                ItemID(6) = DBC.Item(i, 16)
                ItemID(7) = DBC.Item(i, 17)
                'SpellID(0) = DBC.Item(i, 27) ' 27 - 34
                'SpellID(1) = DBC.Item(i, 28)
                'SpellID(2) = DBC.Item(i, 29)
                'SpellID(3) = DBC.Item(i, 30)
                'SpellID(4) = DBC.Item(i, 31)
                'SpellID(5) = DBC.Item(i, 32)
                'SpellID(6) = DBC.Item(i, 33)
                'SpellID(7) = DBC.Item(i, 34)
                'ItemCount(0) = DBC.Item(i, 35) ' Items To Trigger Spell? 
                'ItemCount(1) = DBC.Item(i, 36)
                'ItemCount(2) = DBC.Item(i, 37)
                'ItemCount(3) = DBC.Item(i, 38)
                'ItemCount(4) = DBC.Item(i, 39)
                'ItemCount(5) = DBC.Item(i, 40)
                'ItemCount(6) = DBC.Item(i, 41)
                'ItemCount(7) = DBC.Item(i, 42)
                'Required_Skill_ID = DBC.Item(i, 43)
                'Required_Skill_Value = DBC.Item(i, 44)

                ItemSet.Add(ID, New TItemSet(Name, ItemID, SpellID, ItemCount, Required_Skill_ID, Required_Skill_Value))
            Next

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} ItemSets initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : ItemSet missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub LoadItemDisplayInfoDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ItemDisplayInfo.dbc")

            Dim tmpItemDisplayInfo As TItemDisplayInfo

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                tmpItemDisplayInfo = New TItemDisplayInfo

                tmpItemDisplayInfo.ID = DBC.Item(i, 0)
                tmpItemDisplayInfo.RandomPropertyChance = DBC.Item(i, 11)
                tmpItemDisplayInfo.Unknown = DBC.Item(i, 22)

                ItemDisplayInfo.Add(tmpItemDisplayInfo.ID, tmpItemDisplayInfo)
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} ItemDisplayInfos initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : ItemDisplayInfo missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub LoadItemRandomPropertiesDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\ItemRandomProperties.dbc")

            Dim tmpInfo As TItemRandomPropertiesInfo

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                tmpInfo = New TItemRandomPropertiesInfo

                tmpInfo.ID = DBC.Item(i, 0)
                tmpInfo.Enchant_ID(0) = DBC.Item(i, 2)
                tmpInfo.Enchant_ID(1) = DBC.Item(i, 3)
                tmpInfo.Enchant_ID(2) = DBC.Item(i, 4)

                ItemRandomPropertiesInfo.Add(tmpInfo.ID, tmpInfo)
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} ItemRandomProperties initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : ItemRandomProperties missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

#End Region
#Region "Creatures"
    Public Sub LoadCreatureGossip()
        Try
            Dim GossipQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM npc_gossip;", GossipQuery)

            Dim GUID As ULong
            For Each Gossip As DataRow In GossipQuery.Rows
                GUID = Gossip.Item("npc_guid")
                If CreatureGossip.ContainsKey(GUID) = False Then
                    CreatureGossip.Add(GUID, Gossip.Item("textid"))
                End If
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} creature gossips initialized.", CreatureGossip.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : Waypoint_Data missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub

    Public Sub LoadCreatureFamilyDBC()
        Try
            Dim DBC As DBC.BufferedDBC = New DBC.BufferedDBC("dbc\CreatureFamily.dbc")

            Dim tmpInfo As CreatureFamilyInfo

            Dim i As Integer = 0
            For i = 0 To DBC.Rows - 1
                tmpInfo = New CreatureFamilyInfo

                tmpInfo.ID = DBC.Item(i, 0)
                tmpInfo.Unknown1 = DBC.Item(i, 5)
                tmpInfo.Unknown2 = DBC.Item(i, 6)
                tmpInfo.PetFoodID = DBC.Item(i, 7)
                tmpInfo.Name = DBC.Item(i, 12, DBCValueType.DBC_STRING)

                CreaturesFamily.Add(tmpInfo.ID, tmpInfo)
            Next i

            DBC.Dispose()
            Log.WriteLine(LogType.INFORMATION, "DBC: {0} CreatureFamilys initialized.", i)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("DBC File : CreatureFamily missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub LoadCreatureMovements()
        Try
            Dim MovementsQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM waypoint_data ORDER BY id, point;", MovementsQuery)

            Dim ID As Integer
            For Each Movement As DataRow In MovementsQuery.Rows
                ID = CType(Movement.Item("id"), Integer)
                If CreatureMovement.ContainsKey(ID) = False Then
                    CreatureMovement.Add(ID, New Dictionary(Of Integer, CreatureMovePoint))
                End If
                CreatureMovement(ID).Add(Movement.Item("point"), New CreatureMovePoint(Movement.Item("position_x"), Movement.Item("position_y"), Movement.Item("position_z"), Movement.Item("delay"), Movement.Item("move_flag"), Movement.Item("action"), Movement.Item("action_chance")))
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} creature movements for {1} creatures initialized.", MovementsQuery.Rows.Count, CreatureMovement.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : Waypoint_Data missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub LoadCreatureEquipTable()
        Try
            Dim EquipQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM creature_equip_template;", EquipQuery)

            Dim Entry As Integer
            For Each EquipInfo As DataRow In EquipQuery.Rows
                Entry = CType(EquipInfo.Item("entry"), Integer)
                If CreatureEquip.ContainsKey(Entry) Then Continue For
                CreatureEquip.Add(Entry, New CreatureEquipInfo(EquipInfo.Item("equipmodel1"), EquipInfo.Item("equipmodel2"), EquipInfo.Item("equipmodel3"), EquipInfo.Item("equipinfo1"), EquipInfo.Item("equipinfo2"), EquipInfo.Item("equipinfo3"), EquipInfo.Item("equipslot1"), EquipInfo.Item("equipslot2"), EquipInfo.Item("equipslot3")))
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} creature equips initialized.", EquipQuery.Rows.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : Creature_Equip_Template missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
    Public Sub LoadCreatureModelInfo()
        Try
            Dim ModelQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM creature_model_info;", ModelQuery)

            Dim Entry As Integer
            For Each ModelInfo As DataRow In ModelQuery.Rows
                Entry = CType(ModelInfo.Item("modelid"), Integer)
                If CreatureModel.ContainsKey(Entry) Then Continue For
                CreatureModel.Add(Entry, New CreatureModelInfo(ModelInfo.Item("bounding_radius"), ModelInfo.Item("combat_reach"), ModelInfo.Item("gender"), ModelInfo.Item("modelid_other_gender")))
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} creature models initialized.", ModelQuery.Rows.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : Creature_Model_Info missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Quests"
    Public Sub LoadQuestStartersAndFinishers()
        Dim questStarters As New DataTable
        WorldDatabase.Query("SELECT * FROM creature_questrelation;", questStarters)

        For Each starter As DataRow In questStarters.Rows
            Dim entry As Integer = CInt(starter.Item("id"))
            Dim quest As Integer = CInt(starter.Item("quest"))
            If CreatureQuestStarters.ContainsKey(entry) = False Then CreatureQuestStarters.Add(entry, New List(Of Integer))
            CreatureQuestStarters(entry).Add(quest)
        Next

        Dim questStartersAmount As Integer = questStarters.Rows.Count
        questStarters.Clear()
        WorldDatabase.Query("SELECT * FROM gameobject_questrelation;", questStarters)
        For Each starter As DataRow In questStarters.Rows
            Dim entry As Integer = CInt(starter.Item("id"))
            Dim quest As Integer = CInt(starter.Item("quest"))
            If GameobjectQuestStarters.ContainsKey(entry) = False Then GameobjectQuestStarters.Add(entry, New List(Of Integer))
            GameobjectQuestStarters(entry).Add(quest)
        Next

        questStartersAmount += questStarters.Rows.Count
        Log.WriteLine(LogType.INFORMATION, "Database: {0} queststarters initated for {1} creatures and {2} gameobjects.", questStartersAmount, CreatureQuestStarters.Count, GameobjectQuestStarters.Count)
        questStarters.Clear()

        Dim questFinishers As New DataTable
        WorldDatabase.Query("SELECT * FROM creature_involvedrelation;", questFinishers)

        For Each starter As DataRow In questFinishers.Rows
            Dim entry As Integer = CInt(starter.Item("id"))
            Dim quest As Integer = CInt(starter.Item("quest"))
            If CreatureQuestFinishers.ContainsKey(entry) = False Then CreatureQuestFinishers.Add(entry, New List(Of Integer))
            CreatureQuestFinishers(entry).Add(quest)
        Next

        Dim questFinishersAmount As Integer = questFinishers.Rows.Count
        questFinishers.Clear()
        WorldDatabase.Query("SELECT * FROM gameobject_involvedrelation;", questFinishers)
        For Each starter As DataRow In questFinishers.Rows
            Dim entry As Integer = CInt(starter.Item("id"))
            Dim quest As Integer = CInt(starter.Item("quest"))
            If GameobjectQuestFinishers.ContainsKey(entry) = False Then GameobjectQuestFinishers.Add(entry, New List(Of Integer))
            GameobjectQuestFinishers(entry).Add(quest)
        Next

        questFinishersAmount += questFinishers.Rows.Count
        Log.WriteLine(LogType.INFORMATION, "Database: {0} questfinishers initated for {1} creatures and {2} gameobjects.", questFinishersAmount, CreatureQuestFinishers.Count, GameobjectQuestFinishers.Count)
        questFinishers.Clear()
    End Sub
#End Region
#Region "Transports"
    Public Sub LoadTransports()
        Try
            Dim TransportQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM transports", TransportQuery)

            For Each Transport As DataRow In TransportQuery.Rows
                Dim TransportEntry As Integer = Transport.Item("entry")
                Dim TransportName As String = Transport.Item("name")
                Dim TransportPeriod As Integer = Transport.Item("period")

                Dim newTransport As New TransportObject(TransportEntry, TransportName, TransportPeriod)
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} Transports initialized.", TransportQuery.Rows.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : TransportQuery missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region
#Region "Loot"
    Public Sub LoadLootStores()
        LootTemplates_Creature = New LootStore("creature_loot")
        LootTemplates_Disenchant = New LootStore("disenchant_loot")
        LootTemplates_Fishing = New LootStore("fishing_loot")
        LootTemplates_Gameobject = New LootStore("gameobject_loot")
        LootTemplates_Item = New LootStore("item_loot")
        LootTemplates_Pickpocketing = New LootStore("pickpocketing_loot")
        LootTemplates_QuestMail = New LootStore("quest_mail_loot")
        LootTemplates_Reference = New LootStore("reference_loot")
        LootTemplates_Skinning = New LootStore("skinning_loot")
    End Sub
#End Region
#Region "Weather"
    Public Sub LoadWeather()
        Try
            Dim WeatherQuery As New DataTable
            WorldDatabase.Query("SELECT * FROM game_weather;", WeatherQuery)

            For Each Weather As DataRow In WeatherQuery.Rows
                Dim Zone As Integer = Weather.Item("zone")

                If WeatherZones.ContainsKey(Zone) = False Then
                    Dim ZoneChanges As New WeatherZone(Zone)
                    ZoneChanges.Seasons(0) = New WeatherSeasonChances(Weather.Item("spring_rain_chance"), Weather.Item("spring_snow_chance"), Weather.Item("spring_storm_chance"))
                    ZoneChanges.Seasons(1) = New WeatherSeasonChances(Weather.Item("summer_rain_chance"), Weather.Item("summer_snow_chance"), Weather.Item("summer_storm_chance"))
                    ZoneChanges.Seasons(2) = New WeatherSeasonChances(Weather.Item("fall_rain_chance"), Weather.Item("fall_snow_chance"), Weather.Item("fall_storm_chance"))
                    ZoneChanges.Seasons(3) = New WeatherSeasonChances(Weather.Item("winter_rain_chance"), Weather.Item("winter_snow_chance"), Weather.Item("winter_storm_chance"))
                    WeatherZones.Add(Zone, ZoneChanges)
                End If
            Next

            Log.WriteLine(LogType.INFORMATION, "Database: {0} Weather zones initialized.", WeatherQuery.Rows.Count)
        Catch e As System.IO.DirectoryNotFoundException
            Console.ForegroundColor = System.ConsoleColor.DarkRed
            Console.WriteLine("Database : TransportQuery missing.")
            Console.ForegroundColor = System.ConsoleColor.Gray
        End Try
    End Sub
#End Region

End Module