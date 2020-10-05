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
Imports Mangos.Common.Enums
Imports Mangos.World.DataStores
Imports Mangos.World.Globals

Namespace Player

    Public Module WS_Player_Initializator
        Public DEFAULT_MAX_LEVEL As Integer = 60 'Max Player Level
        Public XPTable(DEFAULT_MAX_LEVEL) As Integer 'Max XPTable Level from Database

        Public Function CalculateStartingLIFE(ByRef objCharacter As CharacterObject, ByVal baseLIFE As Integer) As Integer
            If (objCharacter.Stamina.Base < 20) Then
                Return baseLIFE + (objCharacter.Stamina.Base - 20)
            Else
                Return baseLIFE + 10 * (objCharacter.Stamina.Base - 20)
            End If
        End Function

        Public Function CalculateStartingMANA(ByRef objCharacter As CharacterObject, ByVal baseMANA As Integer) As Integer
            If (objCharacter.Intellect.Base < 20) Then
                Return baseMANA + (objCharacter.Intellect.Base - 20)
            Else
                Return baseMANA + 15 * (objCharacter.Intellect.Base - 20)
            End If
        End Function

        Private Function gainStat(ByVal level As Integer, ByVal a3 As Double, ByVal a2 As Double, ByVal a1 As Double, ByVal a0 As Double) As Integer
            Return CType(Math.Round(a3 * level * level * level + a2 * level * level + a1 * level + a0), Integer) -
                   CType(Math.Round(a3 * (level - 1) * (level - 1) * (level - 1) + a2 * (level - 1) * (level - 1) + a1 * (level - 1) + a0), Integer)
        End Function

        Public Sub CalculateOnLevelUP(ByRef objCharacter As CharacterObject)
            Dim baseInt As Integer = objCharacter.Intellect.Base
            'Dim baseStr As Integer = objCharacter.Strength.Base
            'Dim baseSta As Integer = objCharacter.Stamina.Base
            Dim baseSpi As Integer = objCharacter.Spirit.Base
            Dim baseAgi As Integer = objCharacter.Agility.Base
            'Dim baseMana As Integer = objCharacter.Mana.Maximum
            Dim baseLife As Integer = objCharacter.Life.Maximum

            Select Case objCharacter.Classe
                Case PlayerEnum.Classes.CLASS_DRUID
                    If objCharacter.Level <= 17 Then
                        objCharacter.Life.Base += 17
                    Else
                        objCharacter.Life.Base += objCharacter.Level
                    End If
                    If objCharacter.Level <= 25 Then
                        objCharacter.Mana.Base += 20 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 45
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000021, 0.003009, 0.486493, -0.400003)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000038, 0.005145, 0.871006, -0.832029)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000041, 0.00044, 0.512076, -1.000317)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000023, 0.003345, 0.56005, -0.562058)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000059, 0.004044, 1.04, -1.488504)

                Case Classes.CLASS_HUNTER
                    If objCharacter.Level <= 13 Then
                        objCharacter.Life.Base += 17
                    Else
                        objCharacter.Life.Base += objCharacter.Level + 4
                    End If
                    If objCharacter.Level <= 27 Then
                        objCharacter.Mana.Base += 18 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 45
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000022, 0.0018, 0.407867, -0.550889)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.00002, 0.003007, 0.505215, -0.500642)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.00004, 0.007416, 1.125108, -1.003045)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000031, 0.00448, 0.78004, -0.800471)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000017, 0.003803, 0.536846, -0.490026)

                Case Classes.CLASS_MAGE
                    If objCharacter.Level <= 25 Then
                        objCharacter.Life.Base += 15
                    Else
                        objCharacter.Life.Base += objCharacter.Level - 8
                    End If
                    If objCharacter.Level <= 27 Then
                        objCharacter.Mana.Base += 23 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 51
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000002, 0.001003, 0.10089, -0.076055)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.00004, 0.007416, 1.125108, -1.003045)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000008, 0.001001, 0.16319, -0.06428)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000006, 0.002031, 0.27836, -0.340077)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000039, 0.006981, 1.09009, -1.00607)

                Case Classes.CLASS_PALADIN
                    If objCharacter.Level <= 14 Then
                        objCharacter.Life.Base += 18
                    Else
                        objCharacter.Life.Base += objCharacter.Level + 4
                    End If
                    If objCharacter.Level <= 25 Then
                        objCharacter.Mana.Base += 17 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 42
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000037, 0.005455, 0.940039, -1.00009)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000023, 0.003345, 0.56005, -0.562058)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.00002, 0.003007, 0.505215, -0.500642)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000038, 0.005145, 0.871006, -0.832029)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000032, 0.003025, 0.61589, -0.640307)

                Case Classes.CLASS_PRIEST
                    If objCharacter.Level <= 22 Then
                        objCharacter.Life.Base += 15
                    Else
                        objCharacter.Life.Base += objCharacter.Level - 6
                    End If
                    If objCharacter.Level <= 33 Then
                        objCharacter.Mana.Base += 22 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 54
                    End If
                    If objCharacter.Level = 34 Then objCharacter.Mana.Base += 15
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000008, 0.001001, 0.16319, -0.06428)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000039, 0.006981, 1.09009, -1.00607)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000022, 0.000022, 0.260756, -0.494)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000024, 0.000981, 0.364935, -0.5709)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.00004, 0.007416, 1.125108, -1.003045)

                Case Classes.CLASS_ROGUE
                    If objCharacter.Level <= 15 Then
                        objCharacter.Life.Base += 17
                    Else
                        objCharacter.Life.Base += objCharacter.Level + 2
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000025, 0.00417, 0.654096, -0.601491)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000008, 0.001001, 0.16319, -0.06428)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000038, 0.007834, 1.191028, -1.20394)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000032, 0.003025, 0.61589, -0.640307)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000024, 0.000981, 0.364935, -0.5709)

                Case Classes.CLASS_SHAMAN
                    If objCharacter.Level <= 16 Then
                        objCharacter.Life.Base += 17
                    Else
                        objCharacter.Life.Base += objCharacter.Level + 1
                    End If
                    If objCharacter.Level <= 32 Then
                        objCharacter.Mana.Base += 19 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 52
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000035, 0.003641, 0.73431, -0.800626)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000031, 0.00448, 0.78004, -0.800471)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000022, 0.0018, 0.407867, -0.550889)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.00002, 0.00603, 0.80957, -0.80922)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000038, 0.005145, 0.871006, -0.832029)

                Case Classes.CLASS_WARLOCK
                    If objCharacter.Level <= 17 Then
                        objCharacter.Life.Base += 15
                    Else
                        objCharacter.Life.Base += objCharacter.Level - 2
                    End If
                    If objCharacter.Level <= 30 Then
                        objCharacter.Mana.Base += 21 + objCharacter.Level
                    Else
                        objCharacter.Mana.Base += 51
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000006, 0.002031, 0.27836, -0.340077)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000059, 0.004044, 1.04, -1.488504)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000024, 0.000981, 0.364935, -0.5709)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000021, 0.003009, 0.486493, -0.400003)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.00004, 0.006404, 1.038791, -1.039076)

                Case Classes.CLASS_WARRIOR
                    If objCharacter.Level <= 14 Then
                        objCharacter.Life.Base += 19
                    Else
                        objCharacter.Life.Base += objCharacter.Level + 10
                    End If
                    objCharacter.Strength.Base += gainStat(objCharacter.Level, 0.000039, 0.006902, 1.08004, -1.051701)
                    objCharacter.Intellect.Base += gainStat(objCharacter.Level, 0.000002, 0.001003, 0.10089, -0.076055)
                    objCharacter.Agility.Base += gainStat(objCharacter.Level, 0.000022, 0.0046, 0.655333, -0.600356)
                    objCharacter.Stamina.Base += gainStat(objCharacter.Level, 0.000059, 0.004044, 1.04, -1.488504)
                    objCharacter.Spirit.Base += gainStat(objCharacter.Level, 0.000006, 0.002031, 0.27836, -0.340077)
            End Select

            'Calculate new spi/int gain
            If objCharacter.Agility.Base <> baseAgi Then objCharacter.Resistances(DamageTypes.DMG_PHYSICAL).Base += (objCharacter.Agility.Base - baseAgi) * 2
            If objCharacter.Spirit.Base <> baseSpi Then objCharacter.Life.Base += 10 * (objCharacter.Spirit.Base - baseSpi)
            If objCharacter.Intellect.Base <> baseInt AndAlso objCharacter.ManaType = ManaTypes.TYPE_MANA Then objCharacter.Mana.Base += 15 * (objCharacter.Intellect.Base - baseInt)

            objCharacter.Damage.Minimum += 1
            objCharacter.RangedDamage.Minimum += 1
            objCharacter.Damage.Maximum += 1
            objCharacter.RangedDamage.Maximum += 1
            If objCharacter.Level > 9 Then objCharacter.TalentPoints += 1

            For Each Skill As KeyValuePair(Of Integer, TSkill) In objCharacter.Skills
                If SkillLines(Skill.Key) = SKILL_LineCategory.WEAPON_SKILLS Then
                    CType(Skill.Value, TSkill).Base += 5
                End If
            Next
        End Sub

        Public Function GetClassManaType(ByVal Classe As Classes) As ManaTypes
            Select Case Classe
                Case Classes.CLASS_DRUID, Classes.CLASS_HUNTER, Classes.CLASS_MAGE, Classes.CLASS_PALADIN, Classes.CLASS_PRIEST, Classes.CLASS_SHAMAN, Classes.CLASS_WARLOCK
                    Return ManaTypes.TYPE_MANA
                Case Classes.CLASS_ROGUE
                    Return ManaTypes.TYPE_ENERGY
                Case Classes.CLASS_WARRIOR
                    Return ManaTypes.TYPE_RAGE
                Case Else
                    Return ManaTypes.TYPE_MANA
            End Select
        End Function

        Public Sub InitializeReputations(ByRef objCharacter As CharacterObject)
            For i As Byte = 0 To 63
                objCharacter.Reputation(i) = New TReputation With {
                    .Value = 0,
                    .Flags = 0
                    }

                For Each tmpFactionInfo As KeyValuePair(Of Integer, WS_DBCDatabase.TFaction) In FactionInfo
                    If tmpFactionInfo.Value.VisibleID = i Then
                        For j As Byte = 0 To 3
                            If HaveFlag(tmpFactionInfo.Value.flags(j), objCharacter.Race - 1) Then
                                objCharacter.Reputation(i).Flags = tmpFactionInfo.Value.rep_flags(j)
                                objCharacter.Reputation(i).Value = tmpFactionInfo.Value.rep_stats(j)
                                Exit For
                            End If
                        Next
                        Exit For
                    End If
                Next
            Next
        End Sub
    End Module
End NameSpace