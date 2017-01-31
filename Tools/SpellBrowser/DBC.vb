'
' Copyright (C) 2013 - 2017 getMaNGOS <http://www.getmangos.eu>
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

Imports mangosVB.Common
Imports mangosVB.Common.DBC

Public Module DBC_Stuff
    Public Sub LoadDBCs()
        frmLoad.lblAction.Text = "Loading spells..."
        frmLoad.pbAction.Value = 0
        LoadSpells()

        frmLoad.lblAction.Text = "Loading spell ranges..."
        frmLoad.pbAction.Value = 0
        LoadSpellRanges()

        frmLoad.lblAction.Text = "Loading spell casttimes..."
        frmLoad.pbAction.Value = 0
        LoadSpellCasttimes()

        frmLoad.lblAction.Text = "Loading spell durations..."
        frmLoad.pbAction.Value = 0
        LoadSpellDurations()

        frmLoad.lblAction.Text = "Loading spell radius's..."
        frmLoad.pbAction.Value = 0
        LoadSpellRadius()

        frmLoad.lblAction.Text = "Loading spell icons..."
        frmLoad.pbAction.Value = 0
        LoadSpellIcons()

        frmLoad.lblAction.Text = "Downloading spell icons..."
        frmLoad.pbAction.Value = 0
        DownloadIcons()

        frmLoad.lblAction.Text = "Fixing spell descriptions..."
        frmLoad.pbAction.Value = 0
        FixUpDescs()
    End Sub

    Private Sub LoadSpells()
        Dim SpellDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\Spell.dbc")

        Dim i As Long
        Dim ID As Integer
        For i = 0 To SpellDBC.Rows - 1
            Try
                ID = SpellDBC.Item(i, 0)
                SPELLs(ID) = New SpellInfo
                SPELLs(ID).ID = ID
                SPELLs(ID).School = SpellDBC.Item(i, 1)
                SPELLs(ID).Category = SpellDBC.Item(i, 2)
                SPELLs(ID).DispellType = SpellDBC.Item(i, 3)
                SPELLs(ID).Mechanic = SpellDBC.Item(i, 4)
                SPELLs(ID).unk1 = SpellDBC.Item(i, 5)
                SPELLs(ID).Attributes = SpellDBC.Item(i, 6)
                SPELLs(ID).AttributesEx = SpellDBC.Item(i, 7)
                SPELLs(ID).AttributesEx2 = SpellDBC.Item(i, 8)
                SPELLs(ID).RequredCasterStance = SpellDBC.Item(i, 9) ' RequiredShapeShift
                SPELLs(ID).ShapeshiftExclude = SpellDBC.Item(i, 10)
                SPELLs(ID).Target = SpellDBC.Item(i, 11)
                SPELLs(ID).TargetCreatureType = SpellDBC.Item(i, 12)
                SPELLs(ID).FocusObjectIndex = SpellDBC.Item(i, 13)
                SPELLs(ID).CasterAuraState = SpellDBC.Item(i, 14)
                SPELLs(ID).TargetAuraState = SpellDBC.Item(i, 15)
                SPELLs(ID).SpellCastTimeIndex = SpellDBC.Item(i, 16)
                SPELLs(ID).SpellCooldown = SpellDBC.Item(i, 17)
                SPELLs(ID).CategoryCooldown = SpellDBC.Item(i, 18)
                SPELLs(ID).interruptFlags = SpellDBC.Item(i, 19)
                SPELLs(ID).auraInterruptFlags = SpellDBC.Item(i, 20)
                SPELLs(ID).channelInterruptFlags = SpellDBC.Item(i, 21)
                SPELLs(ID).procFlags = SpellDBC.Item(i, 22)
                SPELLs(ID).procChance = SpellDBC.Item(i, 23)
                SPELLs(ID).procCharges = SpellDBC.Item(i, 24)
                SPELLs(ID).maxLevel = SpellDBC.Item(i, 25)
                SPELLs(ID).baseLevel = SpellDBC.Item(i, 26)
                SPELLs(ID).spellLevel = SpellDBC.Item(i, 27)
                SPELLs(ID).DurationIndex = SpellDBC.Item(i, 28)
                SPELLs(ID).powerType = SpellDBC.Item(i, 29)
                SPELLs(ID).manaCost = SpellDBC.Item(i, 30)
                SPELLs(ID).manaCostPerlevel = SpellDBC.Item(i, 31)
                SPELLs(ID).manaPerSecond = SpellDBC.Item(i, 32)
                SPELLs(ID).manaPerSecondPerLevel = SpellDBC.Item(i, 33)
                SPELLs(ID).rangeIndex = SpellDBC.Item(i, 34)
                SPELLs(ID).Speed = SpellDBC.Item(i, 35, DBCValueType.DBC_FLOAT)
                SPELLs(ID).modalNextSpell = SpellDBC.Item(i, 36)
                SPELLs(ID).maxStack = SpellDBC.Item(i, 37)
                SPELLs(ID).Totem(0) = SpellDBC.Item(i, 38)
                SPELLs(ID).Totem(1) = SpellDBC.Item(i, 39)

                '-CORRECT-
                SPELLs(ID).Reagents(0) = SpellDBC.Item(i, 40)
                SPELLs(ID).Reagents(1) = SpellDBC.Item(i, 41)
                SPELLs(ID).Reagents(2) = SpellDBC.Item(i, 42)
                SPELLs(ID).Reagents(3) = SpellDBC.Item(i, 43)
                SPELLs(ID).Reagents(4) = SpellDBC.Item(i, 44)
                SPELLs(ID).Reagents(5) = SpellDBC.Item(i, 45)
                SPELLs(ID).Reagents(6) = SpellDBC.Item(i, 46)
                SPELLs(ID).Reagents(7) = SpellDBC.Item(i, 47)

                SPELLs(ID).ReagentsCount(0) = SpellDBC.Item(i, 48)
                SPELLs(ID).ReagentsCount(1) = SpellDBC.Item(i, 49)
                SPELLs(ID).ReagentsCount(2) = SpellDBC.Item(i, 50)
                SPELLs(ID).ReagentsCount(3) = SpellDBC.Item(i, 51)
                SPELLs(ID).ReagentsCount(4) = SpellDBC.Item(i, 52)
                SPELLs(ID).ReagentsCount(5) = SpellDBC.Item(i, 53)
                SPELLs(ID).ReagentsCount(6) = SpellDBC.Item(i, 54)
                SPELLs(ID).ReagentsCount(7) = SpellDBC.Item(i, 55)
                '-/CORRECT-

                SPELLs(ID).EquippedItemClass = SpellDBC.Item(i, 56)
                SPELLs(ID).EquippedItemSubClass = SpellDBC.Item(i, 57) 'Mask
                SPELLs(ID).EquippedItemInventoryType = SpellDBC.Item(i, 58) 'Mask

                For j As Integer = 0 To 2
                    If CInt(SpellDBC.Item(i, 58 + j)) <> 0 Then
                        SPELLs(ID).SpellEffects(j) = New SpellEffect

                        SPELLs(ID).SpellEffects(j).ID = SpellDBC.Item(i, 58 + j)
                        SPELLs(ID).SpellEffects(j).valueDie = SpellDBC.Item(i, 61 + j)
                        SPELLs(ID).SpellEffects(j).diceBase = SpellDBC.Item(i, 64 + j)
                        SPELLs(ID).SpellEffects(j).dicePerLevel = SpellDBC.Item(i, 67 + j, DBCValueType.DBC_FLOAT)
                        SPELLs(ID).SpellEffects(j).valuePerLevel = SpellDBC.Item(i, 70 + j, DBCValueType.DBC_FLOAT)
                        SPELLs(ID).SpellEffects(j).valueBase = SpellDBC.Item(i, 73 + j)
                        SPELLs(ID).SpellEffects(j).mechanic = SpellDBC.Item(i, 76 + j)
                        SPELLs(ID).SpellEffects(j).implicitTargetA = SpellDBC.Item(i, 79 + j)
                        SPELLs(ID).SpellEffects(j).implicitTargetB = SpellDBC.Item(i, 82 + j)
                        SPELLs(ID).SpellEffects(j).RadiusIndex = SpellDBC.Item(i, 85 + j)
                        SPELLs(ID).SpellEffects(j).ApplyAuraIndex = SpellDBC.Item(i, 88 + j)
                        SPELLs(ID).SpellEffects(j).Amplitude = SpellDBC.Item(i, 91 + j)
                        SPELLs(ID).SpellEffects(j).MultipleValue = SpellDBC.Item(i, 94 + j)
                        SPELLs(ID).SpellEffects(j).ChainTarget = SpellDBC.Item(i, 97 + j)
                        SPELLs(ID).SpellEffects(j).ItemType = SpellDBC.Item(i, 100 + j)
                        SPELLs(ID).SpellEffects(j).MiscValue = SpellDBC.Item(i, 103 + j)
                        SPELLs(ID).SpellEffects(j).TriggerSpell = SpellDBC.Item(i, 106 + j)
                        SPELLs(ID).SpellEffects(j).valuePerComboPoint = SpellDBC.Item(i, 109 + j)
                    Else
                        SPELLs(ID).SpellEffects(j) = Nothing
                    End If
                Next

                '112 = SpellVisual?
                '113 = Always zero?
                SPELLs(ID).SpellIconID = SpellDBC.Item(i, 114)
                '115 = Some sort of flag? Very rarely set. 90+% are 0.
                '116 = 0 or 50?
                SPELLs(ID).Name = SpellDBC.Item(i, 117, DBCValueType.DBC_STRING)
                '118 = Always zero?
                '119 = Always zero?
                '120 = Always zero?
                '121 = Always zero?
                '122 = Always zero?
                '123 = Always zero?
                '124 = Always zero?
                '125 = Some sort of flag with a lot of bits set (Sometimes same as 134, 143 and 152?)
                If SpellDBC.Item(i, 126) > 0 Then SPELLs(ID).Rank = SpellDBC.Item(i, 126, DBCValueType.DBC_STRING)
                '127 = Always zero?
                '128 = Always zero?
                '129 = Always zero?
                '130 = Always zero?
                '131 = Always zero?
                '132 = Always zero?
                '133 = Always zero?
                '134 = Some sort of flag with a lot of bits set (Sometimes same as 125, 143 and 152?)
                If SpellDBC.Item(i, 135) > 0 Then SPELLs(ID).Description = SpellDBC.Item(i, 135, DBCValueType.DBC_STRING)
                '136 = Always zero?
                '137 = Always zero?
                '138 = Always zero?
                '139 = Always zero?
                '140 = Always zero?
                '141 = Always zero?
                '142 = Always zero?
                '143 = Some sort of flag with a lot of bits set (Sometimes same as 125, 134 and 152?)
                If SpellDBC.Item(i, 144) > 0 Then SPELLs(ID).BuffDesc = SpellDBC.Item(i, 144, DBCValueType.DBC_STRING)
                '145 = Always zero?
                '146 = Always zero?
                '147 = Always zero?
                '148 = Always zero?
                '149 = Always zero?
                '150 = Always zero?
                '151 = Always zero?
                '152 = Some sort of flag with a lot of bits set (Sometimes same as 125, 134 and 143?)
                SPELLs(ID).manaCostPercent = SpellDBC.Item(i, 153)
                '154 = uint32 StartRecoveryCategory; 154  (133 = Global Cooldown?)
                '155 = uint32 StartRecoveryTime; 155 (Global Cooldown = 1500ms?)
                SPELLs(ID).AffectedTargetLevel = SpellDBC.Item(i, 156)
                '157 = SpellFamilyName
                '158 = SpellFamilyFlags
                '159 = Always zero?
                SPELLs(ID).MaxTargets = SpellDBC.Item(i, 160)
                SPELLs(ID).DamageType = SpellDBC.Item(i, 161)
                '162 = 0, 1 or 2? Sometimes same as 161
                '163 = uint32    DmgClass?

                For j As Integer = 0 To 2
                    If SPELLs(ID).SpellEffects(j) IsNot Nothing Then
                        SPELLs(ID).SpellEffects(j).DamageMultiplier = SpellDBC.Item(i, 164 + j, DBCValueType.DBC_FLOAT)
                    End If
                Next

                '167 = Some sort of flag
                '168 = Always zero?
                '169 = Always zero?
                '170 = Always zero?

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / SpellDBC.Rows) * 100)
                    Application.DoEvents()
                End If

            Catch e As Exception
                MsgBox(String.Format("Line {0} caused error: {1}", i, e.ToString))
            End Try
        Next
    End Sub

    Public Sub LoadSpellRanges()
        Try
            Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\SpellRange.dbc")

            Dim spellRangeIndex As Integer
            Dim spellRangeMin As Single
            Dim spellRangeMax As Single

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                spellRangeIndex = tmpDBC.Item(i, 0)
                spellRangeMin = tmpDBC.Item(i, 1, DBCValueType.DBC_FLOAT) ' Added back may be needed in the future
                spellRangeMax = tmpDBC.Item(i, 2, DBCValueType.DBC_FLOAT)

                SpellRange(spellRangeIndex) = spellRangeMax

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / tmpDBC.Rows) * 100)
                    Application.DoEvents()
                End If
            Next i

            tmpDBC.Dispose()
        Catch e As System.IO.DirectoryNotFoundException
            MsgBox("DBC File : SpellRanges missing.")
        End Try
    End Sub

    Public Sub LoadSpellCasttimes()
        Try
            Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\SpellCastTimes.dbc")

            Dim spellCastID As Integer
            Dim spellCastTimeS As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                spellCastID = tmpDBC.Item(i, 0)
                spellCastTimeS = tmpDBC.Item(i, 1)

                SpellCastTime(spellCastID) = spellCastTimeS

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / tmpDBC.Rows) * 100)
                    Application.DoEvents()
                End If
            Next i

            tmpDBC.Dispose()
        Catch e As System.IO.DirectoryNotFoundException
            MsgBox("DBC File : SpellCastTimes missing.")
        End Try
    End Sub

    Public Sub LoadSpellDurations()
        Try
            Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\SpellDuration.dbc")

            Dim SpellDurationIndex As Integer
            Dim SpellDurationValue As Integer

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                SpellDurationIndex = tmpDBC.Item(i, 0)
                SpellDurationValue = tmpDBC.Item(i, 1)

                SpellDuration(SpellDurationIndex) = SpellDurationValue

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / tmpDBC.Rows) * 100)
                    Application.DoEvents()
                End If
            Next i

            tmpDBC.Dispose()
        Catch e As System.IO.DirectoryNotFoundException
            MsgBox("DBC File : SpellDurations missing.")
        End Try
    End Sub

    Public Sub LoadSpellRadius()
        Try
            Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\SpellRadius.dbc")

            Dim radiusID As Integer
            Dim radiusValue As Single

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                radiusID = tmpDBC.Item(i, 0)
                radiusValue = tmpDBC.Item(i, 1, DBCValueType.DBC_FLOAT)

                SpellRadius(radiusID) = radiusValue

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / tmpDBC.Rows) * 100)
                    Application.DoEvents()
                End If
            Next i

            tmpDBC.Dispose()
        Catch e As System.IO.DirectoryNotFoundException
            MsgBox("DBC File : SpellRadius missing.")
        End Try
    End Sub

    Public Sub LoadSpellIcons()
        Try
            Dim tmpDBC As DBC.BufferedDbc = New DBC.BufferedDbc("dbc\SpellIcon.dbc")

            Dim iconID As Integer
            Dim iconStr As String

            Dim i As Integer
            For i = 0 To tmpDBC.Rows - 1
                iconID = tmpDBC.Item(i, 0)
                iconStr = CStr(tmpDBC.Item(i, 1, DBCValueType.DBC_STRING)).Replace("Interface\Icons\", "").Replace("Spells\Icon\", "")

                SpellIcon(iconID) = iconStr

                If (i Mod 50) = 0 Then
                    frmLoad.pbAction.Value = Fix(((i + 1) / tmpDBC.Rows) * 100)
                    Application.DoEvents()
                End If
            Next i

            tmpDBC.Dispose()
        Catch e As System.IO.DirectoryNotFoundException
            MsgBox("DBC File : SpellIcon missing.")
        End Try
    End Sub

    Public Sub DownloadIcons()
        Dim FailedIcons As New List(Of String)

        If System.IO.Directory.Exists("icons") = False Then
            System.IO.Directory.CreateDirectory("icons")
        Else
            If MsgBox("Do you wish to download icons you might miss?", MsgBoxStyle.YesNo, "Download Icons") = MsgBoxResult.No Then Exit Sub
        End If

        Dim Downloaded As Integer = 0
        For Each Icon As KeyValuePair(Of Integer, String) In SpellIcon
            If System.IO.File.Exists("icons\" & Icon.Value & ".jpg") = False Then
                frmLoad.lblAction.Text = "Downloading icon: " & Icon.Value

                Try
                    My.Computer.Network.DownloadFile("http://static.wowhead.com/images/icons/large/" & Icon.Value.ToLower & ".jpg", "icons\" & Icon.Value & ".jpg")
                Catch ex As Net.WebException
                    FailedIcons.Add(Icon.Value)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
            Downloaded += 1

            If (Downloaded Mod 20) = 0 Then
                frmLoad.pbAction.Value = Fix((Downloaded / SpellIcon.Count) * 100)
                Application.DoEvents()
            End If
        Next

        frmLoad.pbAction.Value = 100
        If FailedIcons.Count > 0 Then
            Dim tmpStr As String = FailedIcons.Count & " icons could not be downloaded."
            For Each Icon As String In FailedIcons
                tmpStr &= vbNewLine & "- " & Icon
            Next
            MsgBox(tmpStr)
        End If
    End Sub

    Public Sub FixUpDescs()
        Dim Fixed As Integer = 0
        For Each Spell As KeyValuePair(Of Integer, SpellInfo) In SPELLs
            Spell.Value.Description = FixDescription(Spell.Key, Spell.Value.Description)
            Spell.Value.BuffDesc = FixDescription(Spell.Key, Spell.Value.BuffDesc)

            Fixed += 1
            If (Fixed Mod 20) = 0 Then
                frmLoad.pbAction.Value = Fix((Fixed / SPELLs.Count) * 100)
                Application.DoEvents()
            End If
        Next
    End Sub

    Public Function GetDuration(ByVal DurationIndex) As String
        If SpellDuration.ContainsKey(DurationIndex) = False Then Return ""
        If SpellDuration(DurationIndex) < 60000 Then Return String.Format("{0} sec", Fix(CDbl(SpellDuration(DurationIndex)) / 1000.0))
        Return String.Format("{0:0} min", Fix(CDbl(SpellDuration(DurationIndex)) / 60000.0))
    End Function

    Public Function GetRadius(ByVal RadiusIndex) As String
        If SpellRadius.ContainsKey(RadiusIndex) = False Then Return ""
        Return SpellRadius(RadiusIndex)
    End Function

    Public Function GetSpellPowerType(ByVal SpellID As Integer) As String
        Select Case SPELLs(SpellID).powerType
            Case ManaTypes.TYPE_MANA
                Return "mana"
            Case ManaTypes.TYPE_RAGE
                Return "rage"
            Case ManaTypes.TYPE_ENERGY
                Return "energy"
            Case ManaTypes.TYPE_HAPPINESS
                Return "happiness"
            Case ManaTypes.TYPE_FOCUS
                Return "focus"
            Case ManaTypes.TYPE_HEALTH
                Return "health"
            Case Else
                Return "mana"
        End Select
    End Function

    Public Function GetSpellManacost(ByVal SpellID As Integer) As String
        If (SPELLs(SpellID).AttributesEx And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then Return "Uses 100% " & GetSpellPowerType(SpellID)
        If SPELLs(SpellID).manaCostPercent > 0 Then Return SPELLs(SpellID).manaCostPercent & "% of base " & GetSpellPowerType(SpellID)
        If SPELLs(SpellID).manaPerSecond > 0 Then Return SPELLs(SpellID).manaPerSecond & " " & GetSpellPowerType(SpellID) & " per sec"
        If SPELLs(SpellID).manaCost > 0 Then Return SPELLs(SpellID).manaCost & " " & GetSpellPowerType(SpellID)
        Return ""
    End Function

    Public Function GetSpellCasttime(ByVal CastTimeIndex As Integer) As String
        If SpellCastTime.ContainsKey(CastTimeIndex) = False Then Return ""
        If SpellCastTime(CastTimeIndex) = 0 Then Return "Instant cast"
        If SpellCastTime(CastTimeIndex) < 60000 Then Return String.Format("{0:0.##} sec cast", (CDbl(SpellCastTime(CastTimeIndex)) / 1000.0))
        Return String.Format("{0:0.##} min cast", (CDbl(SpellCastTime(CastTimeIndex)) / 60000.0))
    End Function

    Public Function GetSpellCooldown(ByVal Cooldown As Integer) As String
        If Cooldown <= 0 Then Return ""
        If Cooldown < 60000 Then Return Cooldown & " sec cooldown"
        If Cooldown < 3600000 Then Return String.Format("{0:0.##} min cooldown", (CDbl(Cooldown) / 60000.0))
        Return String.Format("{0:0.##} hour cooldown", (CDbl(Cooldown) / 3600000.0))
    End Function

    Public Function GetSpellRange(ByVal RangeIndex As Integer) As String
        If SpellRange.ContainsKey(RangeIndex) = False Then Return ""
        If SpellRange(RangeIndex) = 0.0F Then Return ""
        If SpellRange(RangeIndex) = 50000.0F Then Return "Unlimited range"
        If SpellRange(RangeIndex) = 5.0F Then Return "Melee range"
        Return SpellRange(RangeIndex) & " yd range"
    End Function

    Public Function FixDescription(ByVal SpellID As Integer, ByVal Description As String) As String
        Description = Description.Replace("$d", GetDuration(SPELLs(SpellID).DurationIndex))
        Description = Description.Replace("$n", SPELLs(SpellID).procCharges)
        Description = Description.Replace("$h", SPELLs(SpellID).procChance)

        Dim tmpString As String = ""
        Dim tmpString2 As String = ""
        Dim tmpPos As Integer = Description.IndexOf("$l")
        Dim tmpPos2 As Integer = 0
        If tmpPos <> -1 Then
            tmpPos += 2
            tmpString = Description.Substring(tmpPos, Description.IndexOf(";", tmpPos) - tmpPos)
            Dim tmpSplit() As String = tmpString.Split(":")
            If Description.Substring(tmpPos - 5, 3) = " 1 " Then
                Description = Description.Replace("1 $l" & tmpString & ";", tmpSplit(0))
            Else
                Description = Description.Replace("$l" & tmpString & ";", tmpSplit(1))
            End If
        End If

        tmpPos = Description.IndexOf("$g")
        tmpPos2 = 0
        If tmpPos <> -1 Then
            tmpPos += 2
            tmpString = Description.Substring(tmpPos, Description.IndexOf(";", tmpPos) - tmpPos)
            Dim tmpSplit() As String = tmpString.Split(":")
            Description = Description.Replace("$g" & tmpString & ";", tmpSplit(0) & "/" & tmpSplit(1))
        End If

        For i As Integer = 1 To 3
            If SPELLs(SpellID).SpellEffects(i - 1) IsNot Nothing Then
                Description = Description.Replace("$s" & i, SPELLs(SpellID).SpellEffects(i - 1).GetValue)
                Description = Description.Replace("$m" & i, SPELLs(SpellID).SpellEffects(i - 1).GetValue)
                Description = Description.Replace("$o" & i, SPELLs(SpellID).SpellEffects(i - 1).GetDotValue(SpellID))
                Description = Description.Replace("$t" & i, Fix(SPELLs(SpellID).SpellEffects(i - 1).Amplitude / 1000))
                Description = Description.Replace("$a" & i, GetRadius(SPELLs(SpellID).SpellEffects(i - 1).RadiusIndex))
                Description = Description.Replace("$q" & i, SPELLs(SpellID).SpellEffects(i - 1).MiscValue)
                Description = Description.Replace("$x" & i, SPELLs(SpellID).SpellEffects(i - 1).ChainTarget)
            End If

            tmpPos = Description.IndexOf("$")
            Do While tmpPos <> -1
                tmpPos += 1
                tmpPos2 = Description.IndexOf(".", tmpPos)
                If Description.IndexOf(" ", tmpPos) = -1 AndAlso tmpPos2 = -1 Then
                    If tmpPos2 = -1 Then
                        tmpString = Description.Substring(tmpPos).Replace("%", "")
                    Else
                        tmpString = Description.Substring(tmpPos, tmpPos2 - tmpPos).Replace("%", "")
                    End If
                Else
                    If tmpPos2 = -1 OrElse (tmpPos2 > Description.IndexOf(" ", tmpPos) AndAlso Description.IndexOf(" ", tmpPos) <> -1) Then
                        tmpString = Description.Substring(tmpPos, Description.IndexOf(" ", tmpPos) - tmpPos).Replace("%", "")
                    Else
                        tmpString = Description.Substring(tmpPos, tmpPos2 - tmpPos).Replace("%", "")
                    End If
                End If

                If tmpString.Length > 2 AndAlso tmpString.EndsWith("s" & i) Then
                    tmpString = tmpString.Substring(0, tmpString.IndexOf("s" & i))
                    If tmpString(0) = "/"c Then
                        Dim tmpSplit() As String = tmpString.Split(";", 2, StringSplitOptions.None)
                        tmpString2 = tmpSplit(0).Substring(1)
                        Dim tmpSpellID As Integer = SpellID
                        If tmpSplit.Length > 1 AndAlso IsNumeric(tmpSplit(1)) Then
                            tmpSpellID = CInt(tmpSplit(1))
                        End If
                        If SPELLs(tmpSpellID).SpellEffects(i - 1) IsNot Nothing Then Description = Description.Replace("$" & tmpString & "s" & i, SPELLs(tmpSpellID).SpellEffects(i - 1).GetValue(CInt(tmpString2)))
                    ElseIf tmpString(0) = "*"c Then
                        Dim tmpSplit() As String = tmpString.Split(";", 2, StringSplitOptions.None)
                        tmpString2 = tmpSplit(0).Substring(1)
                        Dim tmpSpellID As Integer = SpellID
                        If tmpSplit.Length > 1 AndAlso IsNumeric(tmpSplit(1)) Then
                            tmpSpellID = CInt(tmpSplit(1))
                        End If
                        If SPELLs(tmpSpellID).SpellEffects(i - 1) IsNot Nothing Then Description = Description.Replace("$" & tmpString & "s" & i, SPELLs(tmpSpellID).SpellEffects(i - 1).GetValue(, CInt(tmpString2)))
                    Else
                        tmpString2 = tmpString
                        If tmpString.IndexOf("/") <> -1 Then
                            tmpString = tmpString.Substring(0, tmpString.IndexOf("/"))
                        ElseIf tmpString.IndexOf("*") <> -1 Then
                            tmpString = tmpString.Substring(0, tmpString.IndexOf("*"))
                        End If
                        If SPELLs(CInt(tmpString)).SpellEffects(i - 1) IsNot Nothing Then Description = Description.Replace("$" & tmpString2 & "s" & i, SPELLs(CInt(tmpString)).SpellEffects(i - 1).GetValue)
                    End If
                ElseIf tmpString.Length > 1 AndAlso tmpString.EndsWith("d") Then
                    tmpString = tmpString.Substring(0, tmpString.IndexOf("d"))
                    Description = Description.Replace("$" & tmpString & "d", GetDuration(SPELLs(CInt(tmpString)).DurationIndex))
                ElseIf tmpString.Length > 2 AndAlso tmpString.EndsWith("q" & i) Then
                    tmpString = tmpString.Substring(0, tmpString.IndexOf("q" & i))
                    If SPELLs(CInt(tmpString)).SpellEffects(i - 1) IsNot Nothing Then Description = Description.Replace("$" & tmpString & "q" & i, SPELLs(CInt(tmpString)).SpellEffects(i - 1).MiscValue)
                ElseIf tmpString.Length > 2 AndAlso tmpString.EndsWith("a" & i) Then
                    tmpString = tmpString.Substring(0, tmpString.IndexOf("a" & i))
                    If SPELLs(CInt(tmpString)).SpellEffects(i - 1) IsNot Nothing Then Description = Description.Replace("$" & tmpString & "a" & i, GetRadius(SPELLs(CInt(tmpString)).SpellEffects(i - 1).RadiusIndex))
                End If

                tmpPos = Description.IndexOf("$", tmpPos)
            Loop
        Next
        Return Description
    End Function
End Module