<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSpellInfo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSpellInfo))
        Me.picBorder = New System.Windows.Forms.PictureBox
        Me.picIcon = New System.Windows.Forms.PictureBox
        Me.pDescription = New System.Windows.Forms.Panel
        Me.lblDescription = New System.Windows.Forms.Label
        Me.pBuffDesc = New System.Windows.Forms.Panel
        Me.lblBuffDesc = New System.Windows.Forms.Label
        Me.gbGeneral = New System.Windows.Forms.GroupBox
        Me.lblCooldown = New System.Windows.Forms.Label
        Me.lblRange = New System.Windows.Forms.Label
        Me.lblCastTime = New System.Windows.Forms.Label
        Me.lblManaCost = New System.Windows.Forms.Label
        Me.lblRank = New System.Windows.Forms.Label
        Me.lblName = New System.Windows.Forms.Label
        Me.gbFlags = New System.Windows.Forms.GroupBox
        Me.lblSpellAttributesEx2 = New System.Windows.Forms.Label
        Me.lblSpellAttributesEx = New System.Windows.Forms.Label
        Me.lblSpellAttributes = New System.Windows.Forms.Label
        Me.lblAttributesEx2 = New System.Windows.Forms.Label
        Me.lblAttributesEx = New System.Windows.Forms.Label
        Me.lblAttributes = New System.Windows.Forms.Label
        Me.gbEffect1 = New System.Windows.Forms.GroupBox
        Me.lblEffectTargets1 = New System.Windows.Forms.Label
        Me.lblEffectItem1 = New System.Windows.Forms.Label
        Me.lblEffectAmplitude1 = New System.Windows.Forms.Label
        Me.lblEffectTrigger1 = New System.Windows.Forms.Label
        Me.lblEffectRadius1 = New System.Windows.Forms.Label
        Me.lblEffectAura1 = New System.Windows.Forms.Label
        Me.lblEffectMisc1 = New System.Windows.Forms.Label
        Me.lblEffectValue1 = New System.Windows.Forms.Label
        Me.lblEffectName1 = New System.Windows.Forms.Label
        Me.gbEffect2 = New System.Windows.Forms.GroupBox
        Me.lblEffectTargets2 = New System.Windows.Forms.Label
        Me.lblEffectItem2 = New System.Windows.Forms.Label
        Me.lblEffectAmplitude2 = New System.Windows.Forms.Label
        Me.lblEffectTrigger2 = New System.Windows.Forms.Label
        Me.lblEffectRadius2 = New System.Windows.Forms.Label
        Me.lblEffectAura2 = New System.Windows.Forms.Label
        Me.lblEffectMisc2 = New System.Windows.Forms.Label
        Me.lblEffectValue2 = New System.Windows.Forms.Label
        Me.lblEffectName2 = New System.Windows.Forms.Label
        Me.gbEffect3 = New System.Windows.Forms.GroupBox
        Me.lblEffectTargets3 = New System.Windows.Forms.Label
        Me.lblEffectItem3 = New System.Windows.Forms.Label
        Me.lblEffectAmplitude3 = New System.Windows.Forms.Label
        Me.lblEffectTrigger3 = New System.Windows.Forms.Label
        Me.lblEffectRadius3 = New System.Windows.Forms.Label
        Me.lblEffectAura3 = New System.Windows.Forms.Label
        Me.lblEffectMisc3 = New System.Windows.Forms.Label
        Me.lblEffectValue3 = New System.Windows.Forms.Label
        Me.lblEffectName3 = New System.Windows.Forms.Label
        Me.gbDetails = New System.Windows.Forms.GroupBox
        Me.lblDetails = New System.Windows.Forms.Label
        Me.lblEffectChain1 = New System.Windows.Forms.Label
        Me.lblEffectChain2 = New System.Windows.Forms.Label
        Me.lblEffectChain3 = New System.Windows.Forms.Label
        CType(Me.picBorder, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pDescription.SuspendLayout()
        Me.pBuffDesc.SuspendLayout()
        Me.gbGeneral.SuspendLayout()
        Me.gbFlags.SuspendLayout()
        Me.gbEffect1.SuspendLayout()
        Me.gbEffect2.SuspendLayout()
        Me.gbEffect3.SuspendLayout()
        Me.gbDetails.SuspendLayout()
        Me.SuspendLayout()
        '
        'picBorder
        '
        Me.picBorder.Image = CType(resources.GetObject("picBorder.Image"), System.Drawing.Image)
        Me.picBorder.InitialImage = Nothing
        Me.picBorder.Location = New System.Drawing.Point(12, 12)
        Me.picBorder.Name = "picBorder"
        Me.picBorder.Size = New System.Drawing.Size(68, 68)
        Me.picBorder.TabIndex = 0
        Me.picBorder.TabStop = False
        '
        'picIcon
        '
        Me.picIcon.InitialImage = Nothing
        Me.picIcon.Location = New System.Drawing.Point(18, 18)
        Me.picIcon.Name = "picIcon"
        Me.picIcon.Size = New System.Drawing.Size(56, 56)
        Me.picIcon.TabIndex = 7
        Me.picIcon.TabStop = False
        '
        'pDescription
        '
        Me.pDescription.AutoSize = True
        Me.pDescription.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pDescription.BackgroundImage = CType(resources.GetObject("pDescription.BackgroundImage"), System.Drawing.Image)
        Me.pDescription.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pDescription.Controls.Add(Me.lblDescription)
        Me.pDescription.Location = New System.Drawing.Point(12, 338)
        Me.pDescription.Name = "pDescription"
        Me.pDescription.Padding = New System.Windows.Forms.Padding(0, 0, 8, 8)
        Me.pDescription.Size = New System.Drawing.Size(100, 32)
        Me.pDescription.TabIndex = 10
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.BackColor = System.Drawing.Color.Transparent
        Me.lblDescription.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDescription.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(209, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblDescription.Location = New System.Drawing.Point(8, 8)
        Me.lblDescription.MaximumSize = New System.Drawing.Size(360, 300)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(81, 16)
        Me.lblDescription.TabIndex = 9
        Me.lblDescription.Text = "Description"
        '
        'pBuffDesc
        '
        Me.pBuffDesc.AutoSize = True
        Me.pBuffDesc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pBuffDesc.BackgroundImage = CType(resources.GetObject("pBuffDesc.BackgroundImage"), System.Drawing.Image)
        Me.pBuffDesc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pBuffDesc.Controls.Add(Me.lblBuffDesc)
        Me.pBuffDesc.Location = New System.Drawing.Point(118, 338)
        Me.pBuffDesc.Name = "pBuffDesc"
        Me.pBuffDesc.Padding = New System.Windows.Forms.Padding(0, 0, 8, 8)
        Me.pBuffDesc.Size = New System.Drawing.Size(85, 32)
        Me.pBuffDesc.TabIndex = 11
        '
        'lblBuffDesc
        '
        Me.lblBuffDesc.AutoSize = True
        Me.lblBuffDesc.BackColor = System.Drawing.Color.Transparent
        Me.lblBuffDesc.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBuffDesc.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(209, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblBuffDesc.Location = New System.Drawing.Point(8, 8)
        Me.lblBuffDesc.MaximumSize = New System.Drawing.Size(360, 300)
        Me.lblBuffDesc.Name = "lblBuffDesc"
        Me.lblBuffDesc.Size = New System.Drawing.Size(66, 16)
        Me.lblBuffDesc.TabIndex = 9
        Me.lblBuffDesc.Text = "BuffDesc"
        '
        'gbGeneral
        '
        Me.gbGeneral.Controls.Add(Me.lblCooldown)
        Me.gbGeneral.Controls.Add(Me.lblRange)
        Me.gbGeneral.Controls.Add(Me.lblCastTime)
        Me.gbGeneral.Controls.Add(Me.lblManaCost)
        Me.gbGeneral.Controls.Add(Me.lblRank)
        Me.gbGeneral.Controls.Add(Me.lblName)
        Me.gbGeneral.Location = New System.Drawing.Point(86, 6)
        Me.gbGeneral.Name = "gbGeneral"
        Me.gbGeneral.Size = New System.Drawing.Size(299, 85)
        Me.gbGeneral.TabIndex = 13
        Me.gbGeneral.TabStop = False
        Me.gbGeneral.Text = "General"
        '
        'lblCooldown
        '
        Me.lblCooldown.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCooldown.Location = New System.Drawing.Point(153, 61)
        Me.lblCooldown.Name = "lblCooldown"
        Me.lblCooldown.Size = New System.Drawing.Size(139, 13)
        Me.lblCooldown.TabIndex = 12
        Me.lblCooldown.Text = "Cooldown"
        Me.lblCooldown.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblRange
        '
        Me.lblRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRange.Location = New System.Drawing.Point(181, 39)
        Me.lblRange.Name = "lblRange"
        Me.lblRange.Size = New System.Drawing.Size(111, 13)
        Me.lblRange.TabIndex = 11
        Me.lblRange.Text = "Range"
        Me.lblRange.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblCastTime
        '
        Me.lblCastTime.AutoSize = True
        Me.lblCastTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCastTime.Location = New System.Drawing.Point(6, 61)
        Me.lblCastTime.Name = "lblCastTime"
        Me.lblCastTime.Size = New System.Drawing.Size(59, 13)
        Me.lblCastTime.TabIndex = 10
        Me.lblCastTime.Text = "CastTime"
        '
        'lblManaCost
        '
        Me.lblManaCost.AutoSize = True
        Me.lblManaCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblManaCost.Location = New System.Drawing.Point(6, 39)
        Me.lblManaCost.Name = "lblManaCost"
        Me.lblManaCost.Size = New System.Drawing.Size(63, 13)
        Me.lblManaCost.TabIndex = 9
        Me.lblManaCost.Text = "ManaCost"
        '
        'lblRank
        '
        Me.lblRank.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRank.Location = New System.Drawing.Point(211, 19)
        Me.lblRank.Name = "lblRank"
        Me.lblRank.Size = New System.Drawing.Size(81, 13)
        Me.lblRank.TabIndex = 8
        Me.lblRank.Text = "Rank"
        Me.lblRank.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblName.Location = New System.Drawing.Point(6, 19)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(67, 13)
        Me.lblName.TabIndex = 7
        Me.lblName.Text = "SpellName"
        '
        'gbFlags
        '
        Me.gbFlags.Controls.Add(Me.lblSpellAttributesEx2)
        Me.gbFlags.Controls.Add(Me.lblSpellAttributesEx)
        Me.gbFlags.Controls.Add(Me.lblSpellAttributes)
        Me.gbFlags.Controls.Add(Me.lblAttributesEx2)
        Me.gbFlags.Controls.Add(Me.lblAttributesEx)
        Me.gbFlags.Controls.Add(Me.lblAttributes)
        Me.gbFlags.Location = New System.Drawing.Point(391, 6)
        Me.gbFlags.Name = "gbFlags"
        Me.gbFlags.Size = New System.Drawing.Size(372, 85)
        Me.gbFlags.TabIndex = 14
        Me.gbFlags.TabStop = False
        Me.gbFlags.Text = "Flags"
        '
        'lblSpellAttributesEx2
        '
        Me.lblSpellAttributesEx2.AutoSize = True
        Me.lblSpellAttributesEx2.Location = New System.Drawing.Point(98, 61)
        Me.lblSpellAttributesEx2.Name = "lblSpellAttributesEx2"
        Me.lblSpellAttributesEx2.Size = New System.Drawing.Size(33, 13)
        Me.lblSpellAttributesEx2.TabIndex = 19
        Me.lblSpellAttributesEx2.Text = "None"
        '
        'lblSpellAttributesEx
        '
        Me.lblSpellAttributesEx.AutoSize = True
        Me.lblSpellAttributesEx.Location = New System.Drawing.Point(91, 39)
        Me.lblSpellAttributesEx.Name = "lblSpellAttributesEx"
        Me.lblSpellAttributesEx.Size = New System.Drawing.Size(33, 13)
        Me.lblSpellAttributesEx.TabIndex = 18
        Me.lblSpellAttributesEx.Text = "None"
        '
        'lblSpellAttributes
        '
        Me.lblSpellAttributes.AutoSize = True
        Me.lblSpellAttributes.Location = New System.Drawing.Point(77, 19)
        Me.lblSpellAttributes.Name = "lblSpellAttributes"
        Me.lblSpellAttributes.Size = New System.Drawing.Size(33, 13)
        Me.lblSpellAttributes.TabIndex = 17
        Me.lblSpellAttributes.Text = "None"
        '
        'lblAttributesEx2
        '
        Me.lblAttributesEx2.AutoSize = True
        Me.lblAttributesEx2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAttributesEx2.Location = New System.Drawing.Point(6, 61)
        Me.lblAttributesEx2.Name = "lblAttributesEx2"
        Me.lblAttributesEx2.Size = New System.Drawing.Size(86, 13)
        Me.lblAttributesEx2.TabIndex = 15
        Me.lblAttributesEx2.Text = "AttributesEx2:"
        '
        'lblAttributesEx
        '
        Me.lblAttributesEx.AutoSize = True
        Me.lblAttributesEx.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAttributesEx.Location = New System.Drawing.Point(6, 39)
        Me.lblAttributesEx.Name = "lblAttributesEx"
        Me.lblAttributesEx.Size = New System.Drawing.Size(79, 13)
        Me.lblAttributesEx.TabIndex = 14
        Me.lblAttributesEx.Text = "AttributesEx:"
        '
        'lblAttributes
        '
        Me.lblAttributes.AutoSize = True
        Me.lblAttributes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAttributes.Location = New System.Drawing.Point(6, 19)
        Me.lblAttributes.Name = "lblAttributes"
        Me.lblAttributes.Size = New System.Drawing.Size(65, 13)
        Me.lblAttributes.TabIndex = 13
        Me.lblAttributes.Text = "Attributes:"
        '
        'gbEffect1
        '
        Me.gbEffect1.Controls.Add(Me.lblEffectChain1)
        Me.gbEffect1.Controls.Add(Me.lblEffectTargets1)
        Me.gbEffect1.Controls.Add(Me.lblEffectItem1)
        Me.gbEffect1.Controls.Add(Me.lblEffectAmplitude1)
        Me.gbEffect1.Controls.Add(Me.lblEffectTrigger1)
        Me.gbEffect1.Controls.Add(Me.lblEffectRadius1)
        Me.gbEffect1.Controls.Add(Me.lblEffectAura1)
        Me.gbEffect1.Controls.Add(Me.lblEffectMisc1)
        Me.gbEffect1.Controls.Add(Me.lblEffectValue1)
        Me.gbEffect1.Controls.Add(Me.lblEffectName1)
        Me.gbEffect1.Location = New System.Drawing.Point(12, 97)
        Me.gbEffect1.Name = "gbEffect1"
        Me.gbEffect1.Size = New System.Drawing.Size(373, 116)
        Me.gbEffect1.TabIndex = 15
        Me.gbEffect1.TabStop = False
        Me.gbEffect1.Text = "Spell Effect 1"
        '
        'lblEffectTargets1
        '
        Me.lblEffectTargets1.AutoSize = True
        Me.lblEffectTargets1.Location = New System.Drawing.Point(6, 98)
        Me.lblEffectTargets1.Name = "lblEffectTargets1"
        Me.lblEffectTargets1.Size = New System.Drawing.Size(71, 13)
        Me.lblEffectTargets1.TabIndex = 8
        Me.lblEffectTargets1.Text = "EffectTargets"
        '
        'lblEffectItem1
        '
        Me.lblEffectItem1.Location = New System.Drawing.Point(258, 79)
        Me.lblEffectItem1.Name = "lblEffectItem1"
        Me.lblEffectItem1.Size = New System.Drawing.Size(108, 13)
        Me.lblEffectItem1.TabIndex = 7
        Me.lblEffectItem1.Text = "EffectItem"
        Me.lblEffectItem1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAmplitude1
        '
        Me.lblEffectAmplitude1.AutoSize = True
        Me.lblEffectAmplitude1.Location = New System.Drawing.Point(6, 78)
        Me.lblEffectAmplitude1.Name = "lblEffectAmplitude1"
        Me.lblEffectAmplitude1.Size = New System.Drawing.Size(81, 13)
        Me.lblEffectAmplitude1.TabIndex = 6
        Me.lblEffectAmplitude1.Text = "EffectAmplitude"
        '
        'lblEffectTrigger1
        '
        Me.lblEffectTrigger1.Location = New System.Drawing.Point(230, 58)
        Me.lblEffectTrigger1.Name = "lblEffectTrigger1"
        Me.lblEffectTrigger1.Size = New System.Drawing.Size(136, 13)
        Me.lblEffectTrigger1.TabIndex = 5
        Me.lblEffectTrigger1.Text = "EffectTrigger"
        Me.lblEffectTrigger1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectRadius1
        '
        Me.lblEffectRadius1.Location = New System.Drawing.Point(262, 37)
        Me.lblEffectRadius1.Name = "lblEffectRadius1"
        Me.lblEffectRadius1.Size = New System.Drawing.Size(104, 13)
        Me.lblEffectRadius1.TabIndex = 4
        Me.lblEffectRadius1.Text = "EffectRadius"
        Me.lblEffectRadius1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAura1
        '
        Me.lblEffectAura1.AutoSize = True
        Me.lblEffectAura1.Location = New System.Drawing.Point(6, 58)
        Me.lblEffectAura1.Name = "lblEffectAura1"
        Me.lblEffectAura1.Size = New System.Drawing.Size(57, 13)
        Me.lblEffectAura1.TabIndex = 3
        Me.lblEffectAura1.Text = "EffectAura"
        '
        'lblEffectMisc1
        '
        Me.lblEffectMisc1.Location = New System.Drawing.Point(288, 16)
        Me.lblEffectMisc1.Name = "lblEffectMisc1"
        Me.lblEffectMisc1.Size = New System.Drawing.Size(78, 13)
        Me.lblEffectMisc1.TabIndex = 2
        Me.lblEffectMisc1.Text = "EffectMisc"
        Me.lblEffectMisc1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectValue1
        '
        Me.lblEffectValue1.AutoSize = True
        Me.lblEffectValue1.Location = New System.Drawing.Point(6, 38)
        Me.lblEffectValue1.Name = "lblEffectValue1"
        Me.lblEffectValue1.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectValue1.TabIndex = 1
        Me.lblEffectValue1.Text = "EffectValue"
        '
        'lblEffectName1
        '
        Me.lblEffectName1.AutoSize = True
        Me.lblEffectName1.Location = New System.Drawing.Point(6, 16)
        Me.lblEffectName1.Name = "lblEffectName1"
        Me.lblEffectName1.Size = New System.Drawing.Size(63, 13)
        Me.lblEffectName1.TabIndex = 0
        Me.lblEffectName1.Text = "EffectName"
        '
        'gbEffect2
        '
        Me.gbEffect2.Controls.Add(Me.lblEffectChain2)
        Me.gbEffect2.Controls.Add(Me.lblEffectTargets2)
        Me.gbEffect2.Controls.Add(Me.lblEffectItem2)
        Me.gbEffect2.Controls.Add(Me.lblEffectAmplitude2)
        Me.gbEffect2.Controls.Add(Me.lblEffectTrigger2)
        Me.gbEffect2.Controls.Add(Me.lblEffectRadius2)
        Me.gbEffect2.Controls.Add(Me.lblEffectAura2)
        Me.gbEffect2.Controls.Add(Me.lblEffectMisc2)
        Me.gbEffect2.Controls.Add(Me.lblEffectValue2)
        Me.gbEffect2.Controls.Add(Me.lblEffectName2)
        Me.gbEffect2.Location = New System.Drawing.Point(391, 97)
        Me.gbEffect2.Name = "gbEffect2"
        Me.gbEffect2.Size = New System.Drawing.Size(372, 116)
        Me.gbEffect2.TabIndex = 16
        Me.gbEffect2.TabStop = False
        Me.gbEffect2.Text = "Spell Effect 2"
        '
        'lblEffectTargets2
        '
        Me.lblEffectTargets2.AutoSize = True
        Me.lblEffectTargets2.Location = New System.Drawing.Point(6, 98)
        Me.lblEffectTargets2.Name = "lblEffectTargets2"
        Me.lblEffectTargets2.Size = New System.Drawing.Size(71, 13)
        Me.lblEffectTargets2.TabIndex = 16
        Me.lblEffectTargets2.Text = "EffectTargets"
        '
        'lblEffectItem2
        '
        Me.lblEffectItem2.Location = New System.Drawing.Point(258, 79)
        Me.lblEffectItem2.Name = "lblEffectItem2"
        Me.lblEffectItem2.Size = New System.Drawing.Size(108, 13)
        Me.lblEffectItem2.TabIndex = 15
        Me.lblEffectItem2.Text = "EffectItem"
        Me.lblEffectItem2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAmplitude2
        '
        Me.lblEffectAmplitude2.AutoSize = True
        Me.lblEffectAmplitude2.Location = New System.Drawing.Point(6, 78)
        Me.lblEffectAmplitude2.Name = "lblEffectAmplitude2"
        Me.lblEffectAmplitude2.Size = New System.Drawing.Size(81, 13)
        Me.lblEffectAmplitude2.TabIndex = 14
        Me.lblEffectAmplitude2.Text = "EffectAmplitude"
        '
        'lblEffectTrigger2
        '
        Me.lblEffectTrigger2.Location = New System.Drawing.Point(230, 58)
        Me.lblEffectTrigger2.Name = "lblEffectTrigger2"
        Me.lblEffectTrigger2.Size = New System.Drawing.Size(136, 13)
        Me.lblEffectTrigger2.TabIndex = 13
        Me.lblEffectTrigger2.Text = "EffectTrigger"
        Me.lblEffectTrigger2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectRadius2
        '
        Me.lblEffectRadius2.Location = New System.Drawing.Point(262, 38)
        Me.lblEffectRadius2.Name = "lblEffectRadius2"
        Me.lblEffectRadius2.Size = New System.Drawing.Size(104, 13)
        Me.lblEffectRadius2.TabIndex = 12
        Me.lblEffectRadius2.Text = "EffectRadius"
        Me.lblEffectRadius2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAura2
        '
        Me.lblEffectAura2.AutoSize = True
        Me.lblEffectAura2.Location = New System.Drawing.Point(6, 58)
        Me.lblEffectAura2.Name = "lblEffectAura2"
        Me.lblEffectAura2.Size = New System.Drawing.Size(57, 13)
        Me.lblEffectAura2.TabIndex = 11
        Me.lblEffectAura2.Text = "EffectAura"
        '
        'lblEffectMisc2
        '
        Me.lblEffectMisc2.Location = New System.Drawing.Point(288, 16)
        Me.lblEffectMisc2.Name = "lblEffectMisc2"
        Me.lblEffectMisc2.Size = New System.Drawing.Size(78, 13)
        Me.lblEffectMisc2.TabIndex = 10
        Me.lblEffectMisc2.Text = "EffectMisc"
        Me.lblEffectMisc2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectValue2
        '
        Me.lblEffectValue2.AutoSize = True
        Me.lblEffectValue2.Location = New System.Drawing.Point(6, 38)
        Me.lblEffectValue2.Name = "lblEffectValue2"
        Me.lblEffectValue2.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectValue2.TabIndex = 9
        Me.lblEffectValue2.Text = "EffectValue"
        '
        'lblEffectName2
        '
        Me.lblEffectName2.AutoSize = True
        Me.lblEffectName2.Location = New System.Drawing.Point(6, 16)
        Me.lblEffectName2.Name = "lblEffectName2"
        Me.lblEffectName2.Size = New System.Drawing.Size(63, 13)
        Me.lblEffectName2.TabIndex = 8
        Me.lblEffectName2.Text = "EffectName"
        '
        'gbEffect3
        '
        Me.gbEffect3.Controls.Add(Me.lblEffectChain3)
        Me.gbEffect3.Controls.Add(Me.lblEffectTargets3)
        Me.gbEffect3.Controls.Add(Me.lblEffectItem3)
        Me.gbEffect3.Controls.Add(Me.lblEffectAmplitude3)
        Me.gbEffect3.Controls.Add(Me.lblEffectTrigger3)
        Me.gbEffect3.Controls.Add(Me.lblEffectRadius3)
        Me.gbEffect3.Controls.Add(Me.lblEffectAura3)
        Me.gbEffect3.Controls.Add(Me.lblEffectMisc3)
        Me.gbEffect3.Controls.Add(Me.lblEffectValue3)
        Me.gbEffect3.Controls.Add(Me.lblEffectName3)
        Me.gbEffect3.Location = New System.Drawing.Point(12, 219)
        Me.gbEffect3.Name = "gbEffect3"
        Me.gbEffect3.Size = New System.Drawing.Size(373, 116)
        Me.gbEffect3.TabIndex = 17
        Me.gbEffect3.TabStop = False
        Me.gbEffect3.Text = "Spell Effect 3"
        '
        'lblEffectTargets3
        '
        Me.lblEffectTargets3.AutoSize = True
        Me.lblEffectTargets3.Location = New System.Drawing.Point(8, 98)
        Me.lblEffectTargets3.Name = "lblEffectTargets3"
        Me.lblEffectTargets3.Size = New System.Drawing.Size(71, 13)
        Me.lblEffectTargets3.TabIndex = 16
        Me.lblEffectTargets3.Text = "EffectTargets"
        '
        'lblEffectItem3
        '
        Me.lblEffectItem3.Location = New System.Drawing.Point(258, 78)
        Me.lblEffectItem3.Name = "lblEffectItem3"
        Me.lblEffectItem3.Size = New System.Drawing.Size(108, 13)
        Me.lblEffectItem3.TabIndex = 15
        Me.lblEffectItem3.Text = "EffectItem"
        Me.lblEffectItem3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAmplitude3
        '
        Me.lblEffectAmplitude3.AutoSize = True
        Me.lblEffectAmplitude3.Location = New System.Drawing.Point(7, 78)
        Me.lblEffectAmplitude3.Name = "lblEffectAmplitude3"
        Me.lblEffectAmplitude3.Size = New System.Drawing.Size(81, 13)
        Me.lblEffectAmplitude3.TabIndex = 14
        Me.lblEffectAmplitude3.Text = "EffectAmplitude"
        '
        'lblEffectTrigger3
        '
        Me.lblEffectTrigger3.Location = New System.Drawing.Point(230, 58)
        Me.lblEffectTrigger3.Name = "lblEffectTrigger3"
        Me.lblEffectTrigger3.Size = New System.Drawing.Size(136, 13)
        Me.lblEffectTrigger3.TabIndex = 13
        Me.lblEffectTrigger3.Text = "EffectTrigger"
        Me.lblEffectTrigger3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectRadius3
        '
        Me.lblEffectRadius3.Location = New System.Drawing.Point(263, 38)
        Me.lblEffectRadius3.Name = "lblEffectRadius3"
        Me.lblEffectRadius3.Size = New System.Drawing.Size(104, 13)
        Me.lblEffectRadius3.TabIndex = 12
        Me.lblEffectRadius3.Text = "EffectRadius"
        Me.lblEffectRadius3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectAura3
        '
        Me.lblEffectAura3.AutoSize = True
        Me.lblEffectAura3.Location = New System.Drawing.Point(7, 58)
        Me.lblEffectAura3.Name = "lblEffectAura3"
        Me.lblEffectAura3.Size = New System.Drawing.Size(57, 13)
        Me.lblEffectAura3.TabIndex = 11
        Me.lblEffectAura3.Text = "EffectAura"
        '
        'lblEffectMisc3
        '
        Me.lblEffectMisc3.Location = New System.Drawing.Point(288, 16)
        Me.lblEffectMisc3.Name = "lblEffectMisc3"
        Me.lblEffectMisc3.Size = New System.Drawing.Size(78, 13)
        Me.lblEffectMisc3.TabIndex = 10
        Me.lblEffectMisc3.Text = "EffectMisc"
        Me.lblEffectMisc3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectValue3
        '
        Me.lblEffectValue3.AutoSize = True
        Me.lblEffectValue3.Location = New System.Drawing.Point(7, 38)
        Me.lblEffectValue3.Name = "lblEffectValue3"
        Me.lblEffectValue3.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectValue3.TabIndex = 9
        Me.lblEffectValue3.Text = "EffectValue"
        '
        'lblEffectName3
        '
        Me.lblEffectName3.AutoSize = True
        Me.lblEffectName3.Location = New System.Drawing.Point(7, 16)
        Me.lblEffectName3.Name = "lblEffectName3"
        Me.lblEffectName3.Size = New System.Drawing.Size(63, 13)
        Me.lblEffectName3.TabIndex = 8
        Me.lblEffectName3.Text = "EffectName"
        '
        'gbDetails
        '
        Me.gbDetails.Controls.Add(Me.lblDetails)
        Me.gbDetails.Location = New System.Drawing.Point(391, 219)
        Me.gbDetails.Name = "gbDetails"
        Me.gbDetails.Size = New System.Drawing.Size(372, 116)
        Me.gbDetails.TabIndex = 18
        Me.gbDetails.TabStop = False
        Me.gbDetails.Text = "Details"
        '
        'lblDetails
        '
        Me.lblDetails.Location = New System.Drawing.Point(6, 16)
        Me.lblDetails.Name = "lblDetails"
        Me.lblDetails.Size = New System.Drawing.Size(360, 95)
        Me.lblDetails.TabIndex = 0
        Me.lblDetails.Text = "SpellDetails"
        '
        'lblEffectChain1
        '
        Me.lblEffectChain1.Location = New System.Drawing.Point(304, 98)
        Me.lblEffectChain1.Name = "lblEffectChain1"
        Me.lblEffectChain1.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectChain1.TabIndex = 9
        Me.lblEffectChain1.Text = "EffectChain"
        Me.lblEffectChain1.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectChain2
        '
        Me.lblEffectChain2.Location = New System.Drawing.Point(304, 98)
        Me.lblEffectChain2.Name = "lblEffectChain2"
        Me.lblEffectChain2.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectChain2.TabIndex = 17
        Me.lblEffectChain2.Text = "EffectChain"
        Me.lblEffectChain2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblEffectChain3
        '
        Me.lblEffectChain3.Location = New System.Drawing.Point(304, 98)
        Me.lblEffectChain3.Name = "lblEffectChain3"
        Me.lblEffectChain3.Size = New System.Drawing.Size(62, 13)
        Me.lblEffectChain3.TabIndex = 17
        Me.lblEffectChain3.Text = "EffectChain"
        Me.lblEffectChain3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'frmSpellInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(775, 442)
        Me.Controls.Add(Me.gbDetails)
        Me.Controls.Add(Me.gbEffect3)
        Me.Controls.Add(Me.gbEffect2)
        Me.Controls.Add(Me.gbEffect1)
        Me.Controls.Add(Me.gbFlags)
        Me.Controls.Add(Me.gbGeneral)
        Me.Controls.Add(Me.pBuffDesc)
        Me.Controls.Add(Me.pDescription)
        Me.Controls.Add(Me.picIcon)
        Me.Controls.Add(Me.picBorder)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmSpellInfo"
        Me.Text = "Spell Info"
        CType(Me.picBorder, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pDescription.ResumeLayout(False)
        Me.pDescription.PerformLayout()
        Me.pBuffDesc.ResumeLayout(False)
        Me.pBuffDesc.PerformLayout()
        Me.gbGeneral.ResumeLayout(False)
        Me.gbGeneral.PerformLayout()
        Me.gbFlags.ResumeLayout(False)
        Me.gbFlags.PerformLayout()
        Me.gbEffect1.ResumeLayout(False)
        Me.gbEffect1.PerformLayout()
        Me.gbEffect2.ResumeLayout(False)
        Me.gbEffect2.PerformLayout()
        Me.gbEffect3.ResumeLayout(False)
        Me.gbEffect3.PerformLayout()
        Me.gbDetails.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents picBorder As System.Windows.Forms.PictureBox
    Friend WithEvents picIcon As System.Windows.Forms.PictureBox
    Friend WithEvents pDescription As System.Windows.Forms.Panel
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents pBuffDesc As System.Windows.Forms.Panel
    Friend WithEvents lblBuffDesc As System.Windows.Forms.Label
    Friend WithEvents gbGeneral As System.Windows.Forms.GroupBox
    Friend WithEvents lblCooldown As System.Windows.Forms.Label
    Friend WithEvents lblRange As System.Windows.Forms.Label
    Friend WithEvents lblCastTime As System.Windows.Forms.Label
    Friend WithEvents lblManaCost As System.Windows.Forms.Label
    Friend WithEvents lblRank As System.Windows.Forms.Label
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents gbFlags As System.Windows.Forms.GroupBox
    Friend WithEvents lblAttributes As System.Windows.Forms.Label
    Friend WithEvents lblAttributesEx2 As System.Windows.Forms.Label
    Friend WithEvents lblAttributesEx As System.Windows.Forms.Label
    Friend WithEvents lblSpellAttributesEx2 As System.Windows.Forms.Label
    Friend WithEvents lblSpellAttributesEx As System.Windows.Forms.Label
    Friend WithEvents lblSpellAttributes As System.Windows.Forms.Label
    Friend WithEvents gbEffect1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblEffectMisc1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectValue1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectName1 As System.Windows.Forms.Label
    Friend WithEvents gbEffect2 As System.Windows.Forms.GroupBox
    Friend WithEvents gbEffect3 As System.Windows.Forms.GroupBox
    Friend WithEvents lblEffectAura1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTrigger1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectRadius1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectAmplitude1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectItem1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectItem2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectAmplitude2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTrigger2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectRadius2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectAura2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectMisc2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectValue2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectName2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectItem3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectAmplitude3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTrigger3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectRadius3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectAura3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectMisc3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectValue3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectName3 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTargets1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTargets2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectTargets3 As System.Windows.Forms.Label
    Friend WithEvents gbDetails As System.Windows.Forms.GroupBox
    Friend WithEvents lblDetails As System.Windows.Forms.Label
    Friend WithEvents lblEffectChain1 As System.Windows.Forms.Label
    Friend WithEvents lblEffectChain2 As System.Windows.Forms.Label
    Friend WithEvents lblEffectChain3 As System.Windows.Forms.Label
End Class
