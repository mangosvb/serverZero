Public Class frmDbcCompare

    Private IndexChanged As Boolean = False

    Public Sub UpdateCompare()
        lbSpells.Items.Clear()
        lvDBC.Items.Clear()
        For Each spell As Integer In CompareSpells
            lbSpells.Items.Add(SPELLs(spell).Name & " [" & spell & "]")
            Dim tmpItem As ListViewItem = lvDBC.Items.Add(spell)
            tmpItem.SubItems.Add(SPELLs(spell).School)
            tmpItem.SubItems.Add(SPELLs(spell).Category)
            tmpItem.SubItems.Add(SPELLs(spell).DispellType)
            tmpItem.SubItems.Add(SPELLs(spell).Mechanic)
            tmpItem.SubItems.Add(SPELLs(spell).unk1)
            tmpItem.SubItems.Add(SPELLs(spell).Attributes)
            tmpItem.SubItems.Add(SPELLs(spell).AttributesEx)
            tmpItem.SubItems.Add(SPELLs(spell).AttributesEx2)
            tmpItem.SubItems.Add(SPELLs(spell).RequredCasterStance)
            tmpItem.SubItems.Add(SPELLs(spell).ShapeshiftExclude)
            tmpItem.SubItems.Add(SPELLs(spell).Target)
            tmpItem.SubItems.Add(SPELLs(spell).TargetCreatureType)
            tmpItem.SubItems.Add(SPELLs(spell).FocusObjectIndex)
            tmpItem.SubItems.Add(SPELLs(spell).CasterAuraState)
            tmpItem.SubItems.Add(SPELLs(spell).TargetAuraState)
            tmpItem.SubItems.Add(SPELLs(spell).SpellCastTimeIndex)
            tmpItem.SubItems.Add(SPELLs(spell).SpellCooldown)
            tmpItem.SubItems.Add(SPELLs(spell).CategoryCooldown)
            tmpItem.SubItems.Add(SPELLs(spell).interruptFlags)
            tmpItem.SubItems.Add(SPELLs(spell).auraInterruptFlags)
            tmpItem.SubItems.Add(SPELLs(spell).channelInterruptFlags)
            tmpItem.SubItems.Add(SPELLs(spell).procFlags)
            tmpItem.SubItems.Add(SPELLs(spell).procChance)
            tmpItem.SubItems.Add(SPELLs(spell).procCharges)
            tmpItem.SubItems.Add(SPELLs(spell).maxLevel)
            tmpItem.SubItems.Add(SPELLs(spell).baseLevel)
            tmpItem.SubItems.Add(SPELLs(spell).spellLevel)
            tmpItem.SubItems.Add(SPELLs(spell).DurationIndex)
            tmpItem.SubItems.Add(SPELLs(spell).powerType)
            tmpItem.SubItems.Add(SPELLs(spell).manaCost)
            tmpItem.SubItems.Add(SPELLs(spell).manaCostPerlevel)
            tmpItem.SubItems.Add(SPELLs(spell).manaPerSecond)
            tmpItem.SubItems.Add(SPELLs(spell).manaPerSecondPerLevel)
            tmpItem.SubItems.Add(SPELLs(spell).rangeIndex)
            tmpItem.SubItems.Add(SPELLs(spell).Speed)
            tmpItem.SubItems.Add(SPELLs(spell).modalNextSpell)
            tmpItem.SubItems.Add(SPELLs(spell).maxStack)
            tmpItem.SubItems.Add(SPELLs(spell).Totem(0))
            tmpItem.SubItems.Add(SPELLs(spell).Totem(1))

            tmpItem.SubItems.Add(SPELLs(spell).Reagents(0))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(1))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(2))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(3))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(4))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(5))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(6))
            tmpItem.SubItems.Add(SPELLs(spell).Reagents(7))

            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(0))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(1))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(2))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(3))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(4))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(5))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(6))
            tmpItem.SubItems.Add(SPELLs(spell).ReagentsCount(7))

            tmpItem.SubItems.Add(SPELLs(spell).EquippedItemClass)
            tmpItem.SubItems.Add(SPELLs(spell).EquippedItemSubClass)

            'TODO: Spell effects
            For i As Integer = 0 To 17
                For j As Integer = 0 To 2
                    If SPELLs(spell).SpellEffects(j) IsNot Nothing Then
                        Select Case i
                            Case 0
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).ID)
                            Case 1
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).valueDie)
                            Case 2
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).diceBase)
                            Case 3
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).dicePerLevel)
                            Case 4
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).valuePerLevel)
                            Case 5
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).valueBase)
                            Case 6
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).mechanic)
                            Case 7
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).implicitTargetA)
                            Case 8
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).implicitTargetB)
                            Case 9
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).RadiusIndex)
                            Case 10
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).ApplyAuraIndex)
                            Case 11
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).Amplitude)
                            Case 12
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).MultipleValue)
                            Case 13
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).ChainTarget)
                            Case 14
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).ItemType)
                            Case 15
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).MiscValue)
                            Case 16
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).TriggerSpell)
                            Case 17
                                tmpItem.SubItems.Add(SPELLs(spell).SpellEffects(j).valuePerComboPoint)
                        End Select
                    Else
                        tmpItem.SubItems.Add("-")
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub frmDbcCompare_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        UpdateCompare()
    End Sub

    Private Sub lbSpells_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbSpells.SelectedIndexChanged
        If IndexChanged Then IndexChanged = False : Exit Sub
        If lbSpells.SelectedIndex < 0 Then Exit Sub
        IndexChanged = True
        lvDBC.SelectedIndices.Clear()
        IndexChanged = True
        lvDBC.Items(lbSpells.SelectedIndex).Selected = True
    End Sub

    Private Sub lvDBC_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvDBC.SelectedIndexChanged
        If IndexChanged Then IndexChanged = False : Exit Sub
        If lvDBC.SelectedIndices.Count = 0 Then Exit Sub
        IndexChanged = True
        lbSpells.SelectedIndex = lvDBC.SelectedIndices(0)
    End Sub
End Class