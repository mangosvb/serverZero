'
' Copyright (C) 2013 - 2014 getMaNGOS <http://www.getmangos.eu>
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

Public Class frmMain

    Private FilterText As String = ""
    Private FilterID As Integer = 0
    Private FilterFromLevel As Integer = 0
    Private FilterToLevel As Integer = 0
    Private FilterAttributes As UInteger = 0
    Private FilterAttributesEx As UInteger = 0
    Private FilterAttributesEx2 As UInteger = 0
    Private LastSelected As Integer = 0

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        frmLoad.Show()

        LoadDBCs()

        frmLoad.Close()
    End Sub

    Private Sub cmdFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFilter.Click
        FilterText = txtName.Text.ToUpper()
        If Integer.TryParse(txtID.Text, FilterID) = False Then
            FilterID = 0
            txtID.Text = ""
        End If
        If Integer.TryParse(txtFromLvl.Text, FilterFromLevel) = False OrElse FilterFromLevel < 0 OrElse FilterFromLevel > 255 Then
            FilterFromLevel = 0
            txtFromLvl.Text = "0"
        End If
        If Integer.TryParse(txtToLvl.Text, FilterToLevel) = False OrElse FilterToLevel < 0 OrElse FilterToLevel > 255 Then
            FilterToLevel = 0
            txtToLvl.Text = "0"
        End If
        If UInteger.TryParse(txtAttributes.Text, FilterAttributes) = False AndAlso ParseHex(txtAttributes.Text, FilterAttributes) = False Then
            FilterAttributes = UInteger.MaxValue
            txtAttributes.Text = ""
        End If
        If UInteger.TryParse(txtAttributesEx.Text, FilterAttributesEx) = False AndAlso ParseHex(txtAttributesEx.Text, FilterAttributesEx) = False Then
            FilterAttributesEx = UInteger.MaxValue
            txtAttributesEx.Text = ""
        End If
        If UInteger.TryParse(txtAttributesEx2.Text, FilterAttributesEx2) = False AndAlso ParseHex(txtAttributesEx2.Text, FilterAttributesEx2) = False Then
            FilterAttributesEx2 = UInteger.MaxValue
            txtAttributesEx2.Text = ""
        End If

        ListSpells()
    End Sub

    Private Sub cmdReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReset.Click
        FilterText = ""
        FilterID = 0
        txtID.Text = ""
        FilterFromLevel = 0
        txtFromLvl.Text = "0"
        FilterToLevel = 0
        txtToLvl.Text = "0"
        txtAttributes.Text = ""
        txtAttributesEx.Text = ""
        txtAttributesEx2.Text = ""

        ListSpells()
    End Sub

    Private Function ParseHex(ByVal s As String, ByRef result As UInteger)
        s = s.ToUpper()
        If s.StartsWith("&H") = False AndAlso s.StartsWith("0X") = False Then Return False
        If s.Length < 3 OrElse s.Length > 10 Then Return False

        For i As Integer = 2 To s.Length - 1
            If (s(i) < "0"c OrElse s(i) > "9"c) AndAlso (s(i) < "A"c OrElse s(i) > "F"c) Then Return False
        Next

        result = CUInt("&H" & s.Substring(2))

        Return True
    End Function

    Private Sub ListSpells()
        lvSpells.Items.Clear()

        If FilterID > 0 Then
            If SPELLs.ContainsKey(FilterID) = False Then Exit Sub
            With lvSpells.Items.Add(FilterID.ToString)
                .SubItems.Add(SPELLs(FilterID).Name)
                .SubItems.Add(SPELLs(FilterID).Description.Replace(vbNewLine, " "))
                .SubItems.Add(SPELLs(FilterID).spellLevel)
            End With
            Exit Sub
        End If

        frmLoad.Show()
        frmLoad.lblAction.Text = "Filtering..."
        frmLoad.pbAction.Value = 0
        Application.DoEvents()

        Dim i As Integer = 0, j As Integer = 0
        For Each Spell As KeyValuePair(Of Integer, SpellInfo) In SPELLs
            i += 1
            If FilterFromLevel > 0 AndAlso Spell.Value.spellLevel < FilterFromLevel Then Continue For
            If FilterToLevel > 0 AndAlso Spell.Value.spellLevel > FilterToLevel Then Continue For
            If FilterText <> "" AndAlso Spell.Value.Name.ToUpper.IndexOf(FilterText) = -1 AndAlso Spell.Value.Description.ToUpper.IndexOf(FilterText) = -1 Then Continue For
            If FilterAttributes < UInteger.MaxValue AndAlso (Spell.Value.Attributes And FilterAttributes) <> FilterAttributes Then Continue For
            If FilterAttributesEx < UInteger.MaxValue AndAlso (Spell.Value.AttributesEx And FilterAttributesEx) <> FilterAttributesEx Then Continue For
            If FilterAttributesEx2 < UInteger.MaxValue AndAlso (Spell.Value.AttributesEx2 And FilterAttributesEx2) <> FilterAttributesEx2 Then Continue For
            With lvSpells.Items.Add(Spell.Key.ToString)
                .SubItems.Add(Spell.Value.Name)
                .SubItems.Add(Spell.Value.Description.Replace(vbNewLine, " "))
                .SubItems.Add(Spell.Value.spellLevel)
            End With
            j += 1

            If (i Mod 100) Then
                frmLoad.pbAction.Value = Fix((i / SPELLs.Count) * 100)
            End If
        Next

        lblStatus.Text = "Listing spells: " & j

        Application.DoEvents()
        frmLoad.Close()
    End Sub

    Private Sub lvSpells_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvSpells.DoubleClick
        If lvSpells.SelectedItems.Count = 0 Then Exit Sub

        Dim SpellID As Integer = CInt(lvSpells.SelectedItems(0).Text)
        If OpenSpells.ContainsKey(SpellID) Then
            OpenSpells(SpellID).BringToFront()
        Else
            OpenSpells.Add(SpellID, New frmSpellInfo(SpellID))
            OpenSpells(SpellID).Show()
        End If
    End Sub

    Private Sub lvSpells_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lvSpells.MouseMove
        Dim hoverItem As ListViewItem = lvSpells.GetItemAt(e.X, e.Y)
        If hoverItem IsNot Nothing Then
            Dim SpellID As Integer = CInt(hoverItem.Text)
            If LastSelected <> SpellID Then
                LastSelected = SpellID
                lblSpellName.Text = SPELLs(SpellID).Name
                lblSpellRank.Text = SPELLs(SpellID).Rank
                lblSpellCasttime.Text = GetSpellCasttime(SPELLs(SpellID).SpellCastTimeIndex)
                lblSpellCooldown.Text = GetSpellCooldown(SPELLs(SpellID).SpellCooldown)
                lblSpellManacost.Text = GetSpellManacost(SpellID)
                lblSpellRange.Text = GetSpellRange(SPELLs(SpellID).rangeIndex)
                lblSpellDescription.Text = SPELLs(SpellID).Description

                lblSpellRange.Visible = True
                lblSpellCooldown.Visible = True
                lblSpellDescription.Location = New Point(lblSpellDescription.Location.X, 89)
                If lblSpellManacost.Text = "" Then
                    If lblSpellRange.Text = "" Then
                        If lblSpellCasttime.Text = "" Then
                            If lblSpellCooldown.Text = "" Then
                                lblSpellRange.Visible = False
                                lblSpellCooldown.Visible = False
                                lblSpellDescription.Location = New Point(lblSpellDescription.Location.X, 42)
                            Else
                                lblSpellRange.Text = lblSpellCooldown.Text
                                lblSpellCooldown.Text = ""
                                lblSpellCooldown.Visible = False
                                lblSpellDescription.Location = New Point(lblSpellDescription.Location.X, 66)
                            End If
                        Else
                            lblSpellManacost.Text = lblSpellCasttime.Text
                            lblSpellCasttime.Text = ""

                            If lblSpellCooldown.Text = "" Then
                                lblSpellDescription.Location = New Point(lblSpellDescription.Location.X, 66)
                            End If
                        End If
                    Else
                        lblSpellManacost.Text = lblSpellRange.Text
                        lblSpellRange.Text = ""
                    End If
                End If
            End If

            pSpellInfo.Location = New Point(lvSpells.Location.X + e.X + 10, lvSpells.Location.Y + e.Y + 10)
            pSpellInfo.Visible = True
        Else
            LastSelected = 0
            pSpellInfo.Visible = False
        End If
    End Sub

    Private Sub frmMain_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        LastSelected = 0
        pSpellInfo.Visible = False
    End Sub

    Private Sub pSpellInfo_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pSpellInfo.MouseMove
        pSpellInfo.Location = New Point(pSpellInfo.Location.X + e.X + 10, pSpellInfo.Location.Y + e.Y + 10)
    End Sub

    Private Sub txtName_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtName.KeyDown
        If e.KeyCode = Keys.Enter Then
            cmdFilter_Click(txtName, New EventArgs())
        End If
    End Sub

    Private Sub cmdOpenCompare_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOpenCompare.Click
        frmDbcCompare.Show()
    End Sub
End Class
