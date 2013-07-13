<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDbcCompare
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDbcCompare))
        Me.lvDBC = New System.Windows.Forms.ListView
        Me.chID = New System.Windows.Forms.ColumnHeader
        Me.chSchool = New System.Windows.Forms.ColumnHeader
        Me.chCategory = New System.Windows.Forms.ColumnHeader
        Me.chDispellType = New System.Windows.Forms.ColumnHeader
        Me.chMechanic = New System.Windows.Forms.ColumnHeader
        Me.chUnk1 = New System.Windows.Forms.ColumnHeader
        Me.chAttributes = New System.Windows.Forms.ColumnHeader
        Me.chAttributesEx = New System.Windows.Forms.ColumnHeader
        Me.chAttributesEx2 = New System.Windows.Forms.ColumnHeader
        Me.chRequredCasterStance = New System.Windows.Forms.ColumnHeader
        Me.chShapeshiftExclude = New System.Windows.Forms.ColumnHeader
        Me.chTarget = New System.Windows.Forms.ColumnHeader
        Me.chTargetCreatureType = New System.Windows.Forms.ColumnHeader
        Me.chFocusObjectIndex = New System.Windows.Forms.ColumnHeader
        Me.chCasterAuraState = New System.Windows.Forms.ColumnHeader
        Me.chTargetAuraState = New System.Windows.Forms.ColumnHeader
        Me.chSpellCastTimeIndex = New System.Windows.Forms.ColumnHeader
        Me.chSpellCooldown = New System.Windows.Forms.ColumnHeader
        Me.chCategoryCooldown = New System.Windows.Forms.ColumnHeader
        Me.chInterruptFlags = New System.Windows.Forms.ColumnHeader
        Me.chAuraInterruptFlags = New System.Windows.Forms.ColumnHeader
        Me.chChannelInterruptFlags = New System.Windows.Forms.ColumnHeader
        Me.chProcFlags = New System.Windows.Forms.ColumnHeader
        Me.chProcChance = New System.Windows.Forms.ColumnHeader
        Me.chProcCharges = New System.Windows.Forms.ColumnHeader
        Me.chMaxLevel = New System.Windows.Forms.ColumnHeader
        Me.chBaseLevel = New System.Windows.Forms.ColumnHeader
        Me.chSpellLevel = New System.Windows.Forms.ColumnHeader
        Me.chDurationIndex = New System.Windows.Forms.ColumnHeader
        Me.chPowerType = New System.Windows.Forms.ColumnHeader
        Me.chManaCost = New System.Windows.Forms.ColumnHeader
        Me.chManaCostPerlevel = New System.Windows.Forms.ColumnHeader
        Me.chManaPerSecond = New System.Windows.Forms.ColumnHeader
        Me.chManaPerSecondPerLevel = New System.Windows.Forms.ColumnHeader
        Me.chRangeIndex = New System.Windows.Forms.ColumnHeader
        Me.chSpeed = New System.Windows.Forms.ColumnHeader
        Me.chModalNextSpell = New System.Windows.Forms.ColumnHeader
        Me.chMaxStack = New System.Windows.Forms.ColumnHeader
        Me.chTotem1 = New System.Windows.Forms.ColumnHeader
        Me.chTotem2 = New System.Windows.Forms.ColumnHeader
        Me.chReagents1 = New System.Windows.Forms.ColumnHeader
        Me.chReagents2 = New System.Windows.Forms.ColumnHeader
        Me.chReagents3 = New System.Windows.Forms.ColumnHeader
        Me.chReagents4 = New System.Windows.Forms.ColumnHeader
        Me.chReagents5 = New System.Windows.Forms.ColumnHeader
        Me.chReagents6 = New System.Windows.Forms.ColumnHeader
        Me.chReagents7 = New System.Windows.Forms.ColumnHeader
        Me.chReagents8 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount1 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount2 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount3 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount4 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount5 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount6 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount7 = New System.Windows.Forms.ColumnHeader
        Me.chReagentsCount8 = New System.Windows.Forms.ColumnHeader
        Me.chEquippedItemClass = New System.Windows.Forms.ColumnHeader
        Me.chEquippedItemSubClass = New System.Windows.Forms.ColumnHeader
        Me.chEffectID1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectID2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectID3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueDie1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueDie2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueDie3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDiceBase1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDiceBase2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDiceBase3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDicePerLevel1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDicePerLevel2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectDicePerLevel3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerLevel1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerLevel2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerLevel3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueBase1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueBase2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValueBase3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMechanic1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMechanic2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMechanic3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetA1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetA2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetA3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetB1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetB2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectImplicitTargetB3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectRadiusIndex1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectRadiusIndex2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectRadiusIndex3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectApplyAuraIndex1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectApplyAuraIndex2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectApplyAuraIndex3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectAmplitude1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectAmplitude2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectAmplitude3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMultipleValue1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMultipleValue2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMultipleValue3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectChainTarget1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectChainTarget2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectChainTarget3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectItemType1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectItemType2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectItemType3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMiscValue1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMiscValue2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectMiscValue3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectTriggerSpell1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectTriggerSpell2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectTriggerSpell3 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerComboPoint1 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerComboPoint2 = New System.Windows.Forms.ColumnHeader
        Me.chEffectValuePerComboPoint3 = New System.Windows.Forms.ColumnHeader
        Me.lbSpells = New System.Windows.Forms.ListBox
        Me.SuspendLayout()
        '
        'lvDBC
        '
        Me.lvDBC.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvDBC.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chID, Me.chSchool, Me.chCategory, Me.chDispellType, Me.chMechanic, Me.chUnk1, Me.chAttributes, Me.chAttributesEx, Me.chAttributesEx2, Me.chRequredCasterStance, Me.chShapeshiftExclude, Me.chTarget, Me.chTargetCreatureType, Me.chFocusObjectIndex, Me.chCasterAuraState, Me.chTargetAuraState, Me.chSpellCastTimeIndex, Me.chSpellCooldown, Me.chCategoryCooldown, Me.chInterruptFlags, Me.chAuraInterruptFlags, Me.chChannelInterruptFlags, Me.chProcFlags, Me.chProcChance, Me.chProcCharges, Me.chMaxLevel, Me.chBaseLevel, Me.chSpellLevel, Me.chDurationIndex, Me.chPowerType, Me.chManaCost, Me.chManaCostPerlevel, Me.chManaPerSecond, Me.chManaPerSecondPerLevel, Me.chRangeIndex, Me.chSpeed, Me.chModalNextSpell, Me.chMaxStack, Me.chTotem1, Me.chTotem2, Me.chReagents1, Me.chReagents2, Me.chReagents3, Me.chReagents4, Me.chReagents5, Me.chReagents6, Me.chReagents7, Me.chReagents8, Me.chReagentsCount1, Me.chReagentsCount2, Me.chReagentsCount3, Me.chReagentsCount4, Me.chReagentsCount5, Me.chReagentsCount6, Me.chReagentsCount7, Me.chReagentsCount8, Me.chEquippedItemClass, Me.chEquippedItemSubClass, Me.chEffectID1, Me.chEffectID2, Me.chEffectID3, Me.chEffectValueDie1, Me.chEffectValueDie2, Me.chEffectValueDie3, Me.chEffectDiceBase1, Me.chEffectDiceBase2, Me.chEffectDiceBase3, Me.chEffectDicePerLevel1, Me.chEffectDicePerLevel2, Me.chEffectDicePerLevel3, Me.chEffectValuePerLevel1, Me.chEffectValuePerLevel2, Me.chEffectValuePerLevel3, Me.chEffectValueBase1, Me.chEffectValueBase2, Me.chEffectValueBase3, Me.chEffectMechanic1, Me.chEffectMechanic2, Me.chEffectMechanic3, Me.chEffectImplicitTargetA1, Me.chEffectImplicitTargetA2, Me.chEffectImplicitTargetA3, Me.chEffectImplicitTargetB1, Me.chEffectImplicitTargetB2, Me.chEffectImplicitTargetB3, Me.chEffectRadiusIndex1, Me.chEffectRadiusIndex2, Me.chEffectRadiusIndex3, Me.chEffectApplyAuraIndex1, Me.chEffectApplyAuraIndex2, Me.chEffectApplyAuraIndex3, Me.chEffectAmplitude1, Me.chEffectAmplitude2, Me.chEffectAmplitude3, Me.chEffectMultipleValue1, Me.chEffectMultipleValue2, Me.chEffectMultipleValue3, Me.chEffectChainTarget1, Me.chEffectChainTarget2, Me.chEffectChainTarget3, Me.chEffectItemType1, Me.chEffectItemType2, Me.chEffectItemType3, Me.chEffectMiscValue1, Me.chEffectMiscValue2, Me.chEffectMiscValue3, Me.chEffectTriggerSpell1, Me.chEffectTriggerSpell2, Me.chEffectTriggerSpell3, Me.chEffectValuePerComboPoint1, Me.chEffectValuePerComboPoint2, Me.chEffectValuePerComboPoint3})
        Me.lvDBC.FullRowSelect = True
        Me.lvDBC.Location = New System.Drawing.Point(152, 12)
        Me.lvDBC.Name = "lvDBC"
        Me.lvDBC.Size = New System.Drawing.Size(662, 454)
        Me.lvDBC.TabIndex = 0
        Me.lvDBC.UseCompatibleStateImageBehavior = False
        Me.lvDBC.View = System.Windows.Forms.View.Details
        '
        'chID
        '
        Me.chID.Text = "ID"
        '
        'chSchool
        '
        Me.chSchool.Text = "School"
        '
        'chCategory
        '
        Me.chCategory.Text = "Category"
        '
        'chDispellType
        '
        Me.chDispellType.Text = "DispellType"
        '
        'chMechanic
        '
        Me.chMechanic.Text = "Mechanic"
        '
        'chUnk1
        '
        Me.chUnk1.Text = "Unk1"
        '
        'chAttributes
        '
        Me.chAttributes.Text = "Attributes"
        '
        'chAttributesEx
        '
        Me.chAttributesEx.Text = "AttributesEx"
        '
        'chAttributesEx2
        '
        Me.chAttributesEx2.Text = "AttributesEx2"
        '
        'chRequredCasterStance
        '
        Me.chRequredCasterStance.Text = "RequredCasterStance"
        '
        'chShapeshiftExclude
        '
        Me.chShapeshiftExclude.Text = "ShapeshiftExclude"
        '
        'chTarget
        '
        Me.chTarget.Text = "Target"
        '
        'chTargetCreatureType
        '
        Me.chTargetCreatureType.Text = "TargetCreatureType"
        '
        'chFocusObjectIndex
        '
        Me.chFocusObjectIndex.Text = "FocusObjectIndex"
        '
        'chCasterAuraState
        '
        Me.chCasterAuraState.Text = "CasterAuraState"
        '
        'chTargetAuraState
        '
        Me.chTargetAuraState.Text = "TargetAuraState"
        '
        'chSpellCastTimeIndex
        '
        Me.chSpellCastTimeIndex.Text = "SpellCastTimeIndex"
        '
        'chSpellCooldown
        '
        Me.chSpellCooldown.Text = "SpellCooldown"
        '
        'chCategoryCooldown
        '
        Me.chCategoryCooldown.Text = "CategoryCooldown"
        '
        'chInterruptFlags
        '
        Me.chInterruptFlags.Text = "InterruptFlags"
        '
        'chAuraInterruptFlags
        '
        Me.chAuraInterruptFlags.Text = "AuraInterruptFlags"
        '
        'chChannelInterruptFlags
        '
        Me.chChannelInterruptFlags.Text = "ChannelInterruptFlags"
        '
        'chProcFlags
        '
        Me.chProcFlags.Text = "ProcFlags"
        '
        'chProcChance
        '
        Me.chProcChance.Text = "ProcChance"
        '
        'chProcCharges
        '
        Me.chProcCharges.Text = "ProcCharges"
        '
        'chMaxLevel
        '
        Me.chMaxLevel.Text = "MaxLevel"
        '
        'chBaseLevel
        '
        Me.chBaseLevel.Text = "BaseLevel"
        '
        'chSpellLevel
        '
        Me.chSpellLevel.Text = "SpellLevel"
        '
        'chDurationIndex
        '
        Me.chDurationIndex.Text = "DurationIndex"
        '
        'chPowerType
        '
        Me.chPowerType.Text = "PowerType"
        '
        'chManaCost
        '
        Me.chManaCost.Text = "ManaCost"
        '
        'chManaCostPerlevel
        '
        Me.chManaCostPerlevel.Text = "ManaCostPerlevel"
        '
        'chManaPerSecond
        '
        Me.chManaPerSecond.Text = "ManaPerSecond"
        '
        'chManaPerSecondPerLevel
        '
        Me.chManaPerSecondPerLevel.Text = "ManaPerSecondPerLevel"
        '
        'chRangeIndex
        '
        Me.chRangeIndex.Text = "RangeIndex"
        '
        'chSpeed
        '
        Me.chSpeed.Text = "Speed"
        '
        'chModalNextSpell
        '
        Me.chModalNextSpell.Text = "ModalNextSpell"
        '
        'chMaxStack
        '
        Me.chMaxStack.Text = "MaxStack"
        '
        'chTotem1
        '
        Me.chTotem1.Text = "Totem1"
        '
        'chTotem2
        '
        Me.chTotem2.Text = "Totem2"
        '
        'chReagents1
        '
        Me.chReagents1.Text = "Reagents1"
        '
        'chReagents2
        '
        Me.chReagents2.Text = "Reagents2"
        '
        'chReagents3
        '
        Me.chReagents3.Text = "Reagents3"
        '
        'chReagents4
        '
        Me.chReagents4.Text = "Reagents4"
        '
        'chReagents5
        '
        Me.chReagents5.Text = "Reagents5"
        '
        'chReagents6
        '
        Me.chReagents6.Text = "Reagents6"
        '
        'chReagents7
        '
        Me.chReagents7.Text = "Reagents7"
        '
        'chReagents8
        '
        Me.chReagents8.Text = "Reagents8"
        '
        'chReagentsCount1
        '
        Me.chReagentsCount1.Text = "ReagentsCount1"
        '
        'chReagentsCount2
        '
        Me.chReagentsCount2.Text = "ReagentsCount2"
        '
        'chReagentsCount3
        '
        Me.chReagentsCount3.Text = "ReagentsCount3"
        '
        'chReagentsCount4
        '
        Me.chReagentsCount4.Text = "ReagentsCount4"
        '
        'chReagentsCount5
        '
        Me.chReagentsCount5.Text = "ReagentsCount5"
        '
        'chReagentsCount6
        '
        Me.chReagentsCount6.Text = "ReagentsCount6"
        '
        'chReagentsCount7
        '
        Me.chReagentsCount7.Text = "ReagentsCount7"
        '
        'chReagentsCount8
        '
        Me.chReagentsCount8.Text = "ReagentsCount8"
        '
        'chEquippedItemClass
        '
        Me.chEquippedItemClass.Text = "EquippedItemClass"
        '
        'chEquippedItemSubClass
        '
        Me.chEquippedItemSubClass.Text = "EquippedItemSubClass"
        '
        'chEffectID1
        '
        Me.chEffectID1.Text = "EffectID1"
        '
        'chEffectID2
        '
        Me.chEffectID2.Text = "EffectID2"
        '
        'chEffectID3
        '
        Me.chEffectID3.Text = "EffectID3"
        '
        'chEffectValueDie1
        '
        Me.chEffectValueDie1.Text = "EffectValueDie1"
        '
        'chEffectValueDie2
        '
        Me.chEffectValueDie2.Text = "EffectValueDie2"
        '
        'chEffectValueDie3
        '
        Me.chEffectValueDie3.Text = "EffectValueDie3"
        '
        'chEffectDiceBase1
        '
        Me.chEffectDiceBase1.Text = "EffectDiceBase1"
        '
        'chEffectDiceBase2
        '
        Me.chEffectDiceBase2.Text = "EffectDiceBase2"
        '
        'chEffectDiceBase3
        '
        Me.chEffectDiceBase3.Text = "EffectDiceBase3"
        '
        'chEffectDicePerLevel1
        '
        Me.chEffectDicePerLevel1.Text = "EffectDicePerLevel1"
        '
        'chEffectDicePerLevel2
        '
        Me.chEffectDicePerLevel2.Text = "EffectDicePerLevel2"
        '
        'chEffectDicePerLevel3
        '
        Me.chEffectDicePerLevel3.Text = "EffectDicePerLevel"
        '
        'chEffectValuePerLevel1
        '
        Me.chEffectValuePerLevel1.Text = "EffectValuePerLevel1"
        '
        'chEffectValuePerLevel2
        '
        Me.chEffectValuePerLevel2.Text = "EffectValuePerLevel2"
        '
        'chEffectValuePerLevel3
        '
        Me.chEffectValuePerLevel3.Text = "EffectValuePerLevel3"
        '
        'chEffectValueBase1
        '
        Me.chEffectValueBase1.Text = "EffectValueBase1"
        '
        'chEffectValueBase2
        '
        Me.chEffectValueBase2.Text = "EffectValueBase2"
        '
        'chEffectValueBase3
        '
        Me.chEffectValueBase3.Text = "EffectValueBase3"
        '
        'chEffectMechanic1
        '
        Me.chEffectMechanic1.Text = "EffectMechanic1"
        '
        'chEffectMechanic2
        '
        Me.chEffectMechanic2.Text = "EffectMechanic2"
        '
        'chEffectMechanic3
        '
        Me.chEffectMechanic3.Text = "EffectMechanic3"
        '
        'chEffectImplicitTargetA1
        '
        Me.chEffectImplicitTargetA1.Text = "EffectImplicitTargetA1"
        '
        'chEffectImplicitTargetA2
        '
        Me.chEffectImplicitTargetA2.Text = "EffectImplicitTargetA2"
        '
        'chEffectImplicitTargetA3
        '
        Me.chEffectImplicitTargetA3.Text = "EffectImplicitTargetA3"
        '
        'chEffectImplicitTargetB1
        '
        Me.chEffectImplicitTargetB1.Text = "EffectImplicitTargetB1"
        '
        'chEffectImplicitTargetB2
        '
        Me.chEffectImplicitTargetB2.Text = "EffectImplicitTargetB2"
        '
        'chEffectImplicitTargetB3
        '
        Me.chEffectImplicitTargetB3.Text = "EffectImplicitTargetB3"
        '
        'chEffectRadiusIndex1
        '
        Me.chEffectRadiusIndex1.Text = "EffectRadiusIndex1"
        '
        'chEffectRadiusIndex2
        '
        Me.chEffectRadiusIndex2.Text = "EffectRadiusIndex2"
        '
        'chEffectRadiusIndex3
        '
        Me.chEffectRadiusIndex3.Text = "EffectRadiusIndex3"
        '
        'chEffectApplyAuraIndex1
        '
        Me.chEffectApplyAuraIndex1.Text = "EffectApplyAuraIndex1"
        '
        'chEffectApplyAuraIndex2
        '
        Me.chEffectApplyAuraIndex2.Text = "EffectApplyAuraIndex2"
        '
        'chEffectApplyAuraIndex3
        '
        Me.chEffectApplyAuraIndex3.Text = "EffectApplyAuraIndex3"
        '
        'chEffectAmplitude1
        '
        Me.chEffectAmplitude1.Text = "EffectAmplitude1"
        '
        'chEffectAmplitude2
        '
        Me.chEffectAmplitude2.Text = "EffectAmplitude2"
        '
        'chEffectAmplitude3
        '
        Me.chEffectAmplitude3.Text = "EffectAmplitude3"
        '
        'chEffectMultipleValue1
        '
        Me.chEffectMultipleValue1.Text = "EffectMultipleValue1"
        '
        'chEffectMultipleValue2
        '
        Me.chEffectMultipleValue2.Text = "EffectMultipleValue2"
        '
        'chEffectMultipleValue3
        '
        Me.chEffectMultipleValue3.Text = "EffectMultipleValue3"
        '
        'chEffectChainTarget1
        '
        Me.chEffectChainTarget1.Text = "EffectChainTarget1"
        '
        'chEffectChainTarget2
        '
        Me.chEffectChainTarget2.Text = "EffectChainTarget2"
        '
        'chEffectChainTarget3
        '
        Me.chEffectChainTarget3.Text = "EffectChainTarget3"
        '
        'chEffectItemType1
        '
        Me.chEffectItemType1.Text = "EffectItemType1"
        '
        'chEffectItemType2
        '
        Me.chEffectItemType2.Text = "EffectItemType2"
        '
        'chEffectItemType3
        '
        Me.chEffectItemType3.Text = "EffectItemType3"
        '
        'chEffectMiscValue1
        '
        Me.chEffectMiscValue1.Text = "EffectMiscValue1"
        '
        'chEffectMiscValue2
        '
        Me.chEffectMiscValue2.Text = "EffectMiscValue2"
        '
        'chEffectMiscValue3
        '
        Me.chEffectMiscValue3.Text = "EffectMiscValue3"
        '
        'chEffectTriggerSpell1
        '
        Me.chEffectTriggerSpell1.Text = "EffectTriggerSpell1"
        '
        'chEffectTriggerSpell2
        '
        Me.chEffectTriggerSpell2.Text = "EffectTriggerSpell2"
        '
        'chEffectTriggerSpell3
        '
        Me.chEffectTriggerSpell3.Text = "EffectTriggerSpell3"
        '
        'chEffectValuePerComboPoint1
        '
        Me.chEffectValuePerComboPoint1.Text = "EffectValuePerComboPoint1"
        '
        'chEffectValuePerComboPoint2
        '
        Me.chEffectValuePerComboPoint2.Text = "EffectValuePerComboPoint2"
        '
        'chEffectValuePerComboPoint3
        '
        Me.chEffectValuePerComboPoint3.Text = "EffectValuePerComboPoint3"
        '
        'lbSpells
        '
        Me.lbSpells.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lbSpells.FormattingEnabled = True
        Me.lbSpells.Location = New System.Drawing.Point(12, 32)
        Me.lbSpells.Name = "lbSpells"
        Me.lbSpells.Size = New System.Drawing.Size(134, 433)
        Me.lbSpells.TabIndex = 1
        '
        'frmDbcCompare
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(826, 478)
        Me.Controls.Add(Me.lbSpells)
        Me.Controls.Add(Me.lvDBC)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmDbcCompare"
        Me.Text = "Compare Spell DBC entries"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lvDBC As System.Windows.Forms.ListView
    Friend WithEvents lbSpells As System.Windows.Forms.ListBox
    Friend WithEvents chID As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSchool As System.Windows.Forms.ColumnHeader
    Friend WithEvents chCategory As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDispellType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chMechanic As System.Windows.Forms.ColumnHeader
    Friend WithEvents chUnk1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAttributes As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAttributesEx As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAttributesEx2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chRequredCasterStance As System.Windows.Forms.ColumnHeader
    Friend WithEvents chShapeshiftExclude As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTarget As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTargetCreatureType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chFocusObjectIndex As System.Windows.Forms.ColumnHeader
    Friend WithEvents chCasterAuraState As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTargetAuraState As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSpellCastTimeIndex As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSpellCooldown As System.Windows.Forms.ColumnHeader
    Friend WithEvents chCategoryCooldown As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInterruptFlags As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAuraInterruptFlags As System.Windows.Forms.ColumnHeader
    Friend WithEvents chChannelInterruptFlags As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProcFlags As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProcChance As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProcCharges As System.Windows.Forms.ColumnHeader
    Friend WithEvents chMaxLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chBaseLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSpellLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDurationIndex As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPowerType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chManaCost As System.Windows.Forms.ColumnHeader
    Friend WithEvents chManaCostPerlevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chManaPerSecond As System.Windows.Forms.ColumnHeader
    Friend WithEvents chManaPerSecondPerLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chRangeIndex As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSpeed As System.Windows.Forms.ColumnHeader
    Friend WithEvents chModalNextSpell As System.Windows.Forms.ColumnHeader
    Friend WithEvents chMaxStack As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTotem1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTotem2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagents8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chReagentsCount8 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEquippedItemClass As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEquippedItemSubClass As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectID1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectID2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectID3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueDie1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueDie2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueDie3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDiceBase1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDiceBase2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDiceBase3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDicePerLevel1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDicePerLevel2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectDicePerLevel3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerLevel1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerLevel2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerLevel3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueBase1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueBase2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValueBase3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetA1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetA2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetA3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetB1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetB2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectImplicitTargetB3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectRadiusIndex1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectRadiusIndex2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectRadiusIndex3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMechanic1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMechanic2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMechanic3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectApplyAuraIndex1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectApplyAuraIndex2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectApplyAuraIndex3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectAmplitude1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectAmplitude2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectAmplitude3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMultipleValue1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMultipleValue2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMultipleValue3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectChainTarget1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectChainTarget2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectChainTarget3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectItemType1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectItemType2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectItemType3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMiscValue1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMiscValue2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectMiscValue3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectTriggerSpell1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectTriggerSpell2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectTriggerSpell3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerComboPoint1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerComboPoint2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEffectValuePerComboPoint3 As System.Windows.Forms.ColumnHeader
End Class
