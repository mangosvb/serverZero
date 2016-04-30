'
' Copyright (C) 2013 - 2015 getMaNGOS <http://www.getmangos.eu>
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

Public Class frmSpellInfo

    Private SpellID As Integer = 0

    Private Sub frmSpellInfo_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If OpenSpells.ContainsKey(SpellID) Then OpenSpells.Remove(SpellID)
    End Sub

    Public Sub New(ByVal ID As Integer)
        InitializeComponent()

        SpellID = ID

        Me.Text = "Spell Info - " & SPELLs(SpellID).Name

        If SPELLs(SpellID).SpellIconID > 0 AndAlso SpellIcon.ContainsKey(SPELLs(SpellID).SpellIconID) Then
            Dim IconPath As String = "icons\" & SpellIcon(SPELLs(SpellID).SpellIconID) & ".jpg"
            If System.IO.File.Exists(IconPath) Then
                picIcon.Image = Image.FromFile(IconPath)
            End If
        End If

        lblName.Text = SPELLs(SpellID).Name
        lblManaCost.Text = GetSpellManacost(SpellID)
        lblCastTime.Text = GetSpellCasttime(SPELLs(SpellID).SpellCastTimeIndex)
        lblRank.Text = SPELLs(SpellID).Rank
        lblRange.Text = GetSpellRange(SPELLs(SpellID).rangeIndex)
        lblCooldown.Text = GetSpellCooldown(SPELLs(SpellID).SpellCooldown)
        lblDescription.Text = SPELLs(SpellID).Description
        lblBuffDesc.Text = SPELLs(SpellID).BuffDesc

        pBuffDesc.Location = New Point(pDescription.Location.X + pDescription.Width, pBuffDesc.Location.Y)

        lblSpellAttributes.Text = GetFlags(SPELLs(SpellID).Attributes)
        lblSpellAttributesEx.Text = GetFlags(SPELLs(SpellID).AttributesEx)
        lblSpellAttributesEx2.Text = GetFlags(SPELLs(SpellID).AttributesEx2)

        If SPELLs(SpellID).SpellEffects(0) IsNot Nothing Then
            lblEffectName1.Text = String.Format("{0}", SPELLs(SpellID).SpellEffects(0).ID)
            lblEffectValue1.Text = "Value: " & SPELLs(SpellID).SpellEffects(0).GetValue
            lblEffectAura1.Text = "No aura"
            If SPELLs(SpellID).SpellEffects(0).ApplyAuraIndex > 0 Then lblEffectAura1.Text = String.Format("{0}", CType(SPELLs(SpellID).SpellEffects(0).ApplyAuraIndex, AuraEffects_Names))
            lblEffectAmplitude1.Text = "Amplitude: " & SPELLs(SpellID).SpellEffects(0).Amplitude
            lblEffectMisc1.Text = "Misc: " & SPELLs(SpellID).SpellEffects(0).MiscValue
            lblEffectRadius1.Text = "Radius: " & GetRadius(SPELLs(SpellID).SpellEffects(0).RadiusIndex)
            lblEffectTrigger1.Text = "Trigger spell: None"
            If SPELLs(SpellID).SpellEffects(0).TriggerSpell > 0 Then lblEffectTrigger1.Text = "Trigger spell: " & SPELLs(SPELLs(SpellID).SpellEffects(0).TriggerSpell).Name
            lblEffectItem1.Text = "Item: " & SPELLs(SpellID).SpellEffects(0).ItemType
            lblEffectTargets1.Text = SPELLs(SpellID).SpellEffects(0).GetTargets
            lblEffectChain1.Text = SPELLs(SpellID).SpellEffects(0).ChainTarget
        Else
            lblEffectName1.Text = "None"
            lblEffectValue1.Text = ""
            lblEffectAura1.Text = ""
            lblEffectAmplitude1.Text = ""
            lblEffectMisc1.Text = ""
            lblEffectRadius1.Text = ""
            lblEffectTrigger1.Text = ""
            lblEffectItem1.Text = ""
            lblEffectTargets1.Text = ""
            lblEffectChain1.Text = ""
        End If

        If SPELLs(SpellID).SpellEffects(1) IsNot Nothing Then
            lblEffectName2.Text = String.Format("{0}", SPELLs(SpellID).SpellEffects(1).ID)
            lblEffectValue2.Text = "Value: " & SPELLs(SpellID).SpellEffects(1).GetValue
            lblEffectAura2.Text = "No aura"
            If SPELLs(SpellID).SpellEffects(1).ApplyAuraIndex > 0 Then lblEffectAura2.Text = String.Format("{0}", CType(SPELLs(SpellID).SpellEffects(1).ApplyAuraIndex, AuraEffects_Names))
            lblEffectAmplitude2.Text = "Amplitude: " & SPELLs(SpellID).SpellEffects(0).Amplitude
            lblEffectMisc2.Text = "Misc: " & SPELLs(SpellID).SpellEffects(1).MiscValue
            lblEffectRadius2.Text = "Radius: " & GetRadius(SPELLs(SpellID).SpellEffects(1).RadiusIndex)
            lblEffectTrigger2.Text = "Trigger spell: None"
            If SPELLs(SpellID).SpellEffects(1).TriggerSpell > 0 Then lblEffectTrigger2.Text = "Trigger spell: " & SPELLs(SPELLs(SpellID).SpellEffects(1).TriggerSpell).Name
            lblEffectItem2.Text = "Item: " & SPELLs(SpellID).SpellEffects(1).ItemType
            lblEffectTargets2.Text = SPELLs(SpellID).SpellEffects(1).GetTargets
            lblEffectChain2.Text = SPELLs(SpellID).SpellEffects(1).ChainTarget
        Else
            lblEffectName2.Text = "None"
            lblEffectValue2.Text = ""
            lblEffectAura2.Text = ""
            lblEffectAmplitude2.Text = ""
            lblEffectMisc2.Text = ""
            lblEffectRadius2.Text = ""
            lblEffectTrigger2.Text = ""
            lblEffectItem2.Text = ""
            lblEffectTargets2.Text = ""
            lblEffectChain2.Text = ""
        End If

        If SPELLs(SpellID).SpellEffects(2) IsNot Nothing Then
            lblEffectName3.Text = String.Format("{0}", SPELLs(SpellID).SpellEffects(2).ID)
            lblEffectValue3.Text = "Value: " & SPELLs(SpellID).SpellEffects(2).GetValue
            lblEffectAura3.Text = "No aura"
            If SPELLs(SpellID).SpellEffects(2).ApplyAuraIndex > 0 Then lblEffectAura3.Text = String.Format("{0}", CType(SPELLs(SpellID).SpellEffects(2).ApplyAuraIndex, AuraEffects_Names))
            lblEffectAmplitude3.Text = "Amplitude: " & SPELLs(SpellID).SpellEffects(2).Amplitude
            lblEffectMisc3.Text = "Misc: " & SPELLs(SpellID).SpellEffects(2).MiscValue
            lblEffectRadius3.Text = "Radius: " & GetRadius(SPELLs(SpellID).SpellEffects(2).RadiusIndex)
            lblEffectTrigger3.Text = "Trigger spell: None"
            If SPELLs(SpellID).SpellEffects(2).TriggerSpell > 0 Then lblEffectTrigger3.Text = "Trigger spell: " & SPELLs(SPELLs(SpellID).SpellEffects(2).TriggerSpell).Name
            lblEffectItem3.Text = "Item: " & SPELLs(SpellID).SpellEffects(2).ItemType
            lblEffectTargets3.Text = SPELLs(SpellID).SpellEffects(2).GetTargets
            lblEffectChain3.Text = SPELLs(SpellID).SpellEffects(2).ChainTarget
        Else
            lblEffectName3.Text = "None"
            lblEffectValue3.Text = ""
            lblEffectAura3.Text = ""
            lblEffectAmplitude3.Text = ""
            lblEffectMisc3.Text = ""
            lblEffectRadius3.Text = ""
            lblEffectTrigger3.Text = ""
            lblEffectItem3.Text = ""
            lblEffectTargets3.Text = ""
            lblEffectChain3.Text = ""
        End If

        If lblManaCost.Text = "" Then
            lblManaCost.Text = lblRange.Text
            lblRange.Text = ""
        End If
        If lblManaCost.Text = "" Then
            lblManaCost.Text = lblManaCost.Text
            lblManaCost.Text = ""
        End If
        If lblRange.Text = "" Then
            lblRange.Text = lblCooldown.Text
            lblCooldown.Text = ""
        End If

        Dim SpellDetails As New List(Of String)
        GetAttributes(SpellDetails, SpellID)
        GetAttributesEx(SpellDetails, SpellID)
        GetAttributesEx2(SpellDetails, SpellID)

        SpellDetails.Sort()
        lblDetails.Text = GetList(SpellDetails)
    End Sub

    Private Function GetFlags(ByVal Mask As UInteger) As String
        If Mask = 0 Then Return "None"
        Dim tmpStr As String = ""

        For i As UInteger = 0 To 31
            If (Mask And (1UI << i)) Then
                If tmpStr <> "" Then tmpStr &= ", "
                tmpStr &= i.ToString
            End If
        Next

        Return tmpStr
    End Function

    Private Function GetList(ByRef SpellDetails As List(Of String)) As String
        Dim tmpStr As String = ""
        For Each Spell As String In SpellDetails
            If tmpStr <> "" Then tmpStr &= ", "
            tmpStr &= Spell
        Next

        Return tmpStr
    End Function

    Private Sub GetAttributes(ByRef SpellDetails As List(Of String), ByVal SpellID As Integer)
        Dim Attributes As Integer = SPELLs(SpellID).Attributes
        If (Attributes And SpellAttributes.SPELL_ATTR_NEXT_ATTACK) OrElse (Attributes And SpellAttributes.SPELL_ATTR_NEXT_ATTACK2) Then SpellDetails.Add("On Next Attack")
        If (Attributes And SpellAttributes.SPELL_ATTR_PASSIVE) Then SpellDetails.Add("Passive")
        If (Attributes And SpellAttributes.SPELL_ATTR_RANGED) Then SpellDetails.Add("Ranged")
        If (Attributes And SpellAttributes.SPELL_ATTR_REQ_STEALTH) Then SpellDetails.Add("Requires Stealth")
        If (Attributes And SpellAttributes.SPELL_ATTR_STOP_ATTACK) Then SpellDetails.Add("Stops Attack")
        If (Attributes And SpellAttributes.SPELL_ATTR_WHILE_DEAD) Then SpellDetails.Add("Useable While Dead")
        If (Attributes And SpellAttributes.SPELL_ATTR_WHILE_MOUNTED) Then SpellDetails.Add("Useable While Mounted")
        If (Attributes And SpellAttributes.SPELL_ATTR_WHILE_SEATED) Then SpellDetails.Add("Useable While Seated")
        If (Attributes And SpellAttributes.SPELL_ATTR_MOVEMENT_IMPAIRING) Then SpellDetails.Add("Movement Impairing")
        If (Attributes And SpellAttributes.SPELL_ATTR_CANT_BLOCK) Then SpellDetails.Add("Can't Block")
        If (Attributes And SpellAttributes.SPELL_ATTR_CANT_REMOVE) Then SpellDetails.Add("Can't Remove")
        If (Attributes And SpellAttributes.SPELL_ATTR_COOLDOWN_AFTER_FADE) Then SpellDetails.Add("Cooldown After Fade")
        If (Attributes And SpellAttributes.SPELL_ATTR_IGNORE_IMMUNE) Then SpellDetails.Add("Ignore Immunity")
        If (Attributes And SpellAttributes.SPELL_ATTR_IS_ABILITY) Then SpellDetails.Add("Is Ability")
        If (Attributes And SpellAttributes.SPELL_ATTR_IS_TRADE_SKILL) Then SpellDetails.Add("Is Trade Skill")
        If (Attributes And SpellAttributes.SPELL_ATTR_NO_VISIBLE_AURA) Then SpellDetails.Add("Has No Visible Aura")
        If (Attributes And SpellAttributes.SPELL_ATTR_NOT_WHILE_COMBAT) Then SpellDetails.Add("Can't Be Used In Combat")
        If (Attributes And SpellAttributes.SPELL_ATTR_NOT_WHILE_SHAPESHIFTED) Then SpellDetails.Add("Can't Be Used While Shapeshifted")
        If (Attributes And SpellAttributes.SPELL_ATTR_ONLY_DAYTIME) Then SpellDetails.Add("Useable Only Daytime")
        If (Attributes And SpellAttributes.SPELL_ATTR_ONLY_NIGHT) Then SpellDetails.Add("Useable Only Night")
        If (Attributes And SpellAttributes.SPELL_ATTR_ONLY_INDOOR) Then SpellDetails.Add("Useable Only Indoor")
        If (Attributes And SpellAttributes.SPELL_ATTR_ONLY_OUTDOOR) Then SpellDetails.Add("Useable Only Outdoor")
        If (Attributes And SpellAttributes.SPELL_ATTR_SCALE_DMG_LVL) Then SpellDetails.Add("Scales Damage With Level")
        If (Attributes And SpellAttributes.SPELL_ATTR_TEMP_WEAPON_ENCH) Then SpellDetails.Add("Temporary Weapon Enchant")
    End Sub

    Private Sub GetAttributesEx(ByRef SpellDetails As List(Of String), ByVal SpellID As Integer)
        Dim Attributes As Integer = SPELLs(SpellID).AttributesEx
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_CHANNELED_1) OrElse (Attributes And SpellAttributesEx.SPELL_ATTR_EX_CHANNELED_2) Then SpellDetails.Add("Channeled")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_DISPEL_AURAS_ON_IMMUNITY) Then SpellDetails.Add("Dispel Auras On Immunity")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_DRAIN_ALL_POWER) Then SpellDetails.Add("Drains All Power")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_NEGATIVE) Then SpellDetails.Add("Is Negative")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_NOT_BREAK_STEALTH) Then SpellDetails.Add("Doesn't Break Stealth")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_NOT_IN_COMBAT_TARGET) Then SpellDetails.Add("Can't Be Used On Target In Combat")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_NOT_PASSIVE) Then SpellDetails.Add("Not Passive")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_REQ_COMBO_POINTS1) OrElse (Attributes And SpellAttributesEx.SPELL_ATTR_EX_REQ_COMBO_POINTS2) Then SpellDetails.Add("Requires Combopoints")
        If (Attributes And SpellAttributesEx.SPELL_ATTR_EX_UNAFFECTED_BY_SCHOOL_IMMUNE) Then SpellDetails.Add("Unaffected By School Immunity")
    End Sub

    Private Sub GetAttributesEx2(ByRef SpellDetails As List(Of String), ByVal SpellID As Integer)
        Dim Attributes As Integer = SPELLs(SpellID).AttributesEx2
        If (Attributes And SpellAttributesEx2.SPELL_ATTR_EX2_AUTO_SHOOT) Then SpellDetails.Add("Auto-Shoot")
        If (Attributes And SpellAttributesEx2.SPELL_ATTR_EX2_CANT_CRIT) Then SpellDetails.Add("Can't Crit")
        If (Attributes And SpellAttributesEx2.SPELL_ATTR_EX2_HEALTH_FUNNEL) Then SpellDetails.Add("Health Funnel")
        If (Attributes And SpellAttributesEx2.SPELL_ATTR_EX2_NOT_NEED_SHAPESHIFT) Then SpellDetails.Add("Doesn't Need Shapeshift")
    End Sub

    Private Sub frmSpellInfo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.Shift AndAlso e.KeyCode = Keys.C Then
            If CompareSpells.Contains(SpellID) = False Then
                CompareSpells.Add(SpellID)
                frmDbcCompare.UpdateCompare()
            End If
        End If
    End Sub
End Class