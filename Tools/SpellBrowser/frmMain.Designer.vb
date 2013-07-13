<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.lvSpells = New System.Windows.Forms.ListView
        Me.chID = New System.Windows.Forms.ColumnHeader
        Me.chName = New System.Windows.Forms.ColumnHeader
        Me.chDescription = New System.Windows.Forms.ColumnHeader
        Me.chLevel = New System.Windows.Forms.ColumnHeader
        Me.lblFilter = New System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.TextBox
        Me.txtID = New System.Windows.Forms.TextBox
        Me.txtFromLvl = New System.Windows.Forms.TextBox
        Me.txtToLvl = New System.Windows.Forms.TextBox
        Me.lblID = New System.Windows.Forms.Label
        Me.lblLevel = New System.Windows.Forms.Label
        Me.lblLvlSeparator = New System.Windows.Forms.Label
        Me.cmdFilter = New System.Windows.Forms.Button
        Me.cmdReset = New System.Windows.Forms.Button
        Me.pSpellInfo = New System.Windows.Forms.Panel
        Me.lblSpellDescription = New System.Windows.Forms.Label
        Me.lblSpellRange = New System.Windows.Forms.Label
        Me.lblSpellCooldown = New System.Windows.Forms.Label
        Me.lblSpellCasttime = New System.Windows.Forms.Label
        Me.lblSpellManacost = New System.Windows.Forms.Label
        Me.lblSpellRank = New System.Windows.Forms.Label
        Me.lblSpellName = New System.Windows.Forms.Label
        Me.lblAttributes = New System.Windows.Forms.Label
        Me.txtAttributes = New System.Windows.Forms.TextBox
        Me.lblAttributesEx = New System.Windows.Forms.Label
        Me.lblAttributesEx2 = New System.Windows.Forms.Label
        Me.txtAttributesEx = New System.Windows.Forms.TextBox
        Me.txtAttributesEx2 = New System.Windows.Forms.TextBox
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.lblStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.cmdOpenCompare = New System.Windows.Forms.Button
        Me.pSpellInfo.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lvSpells
        '
        Me.lvSpells.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvSpells.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chID, Me.chName, Me.chDescription, Me.chLevel})
        Me.lvSpells.FullRowSelect = True
        Me.lvSpells.GridLines = True
        Me.lvSpells.Location = New System.Drawing.Point(12, 78)
        Me.lvSpells.MultiSelect = False
        Me.lvSpells.Name = "lvSpells"
        Me.lvSpells.Size = New System.Drawing.Size(732, 481)
        Me.lvSpells.TabIndex = 0
        Me.lvSpells.UseCompatibleStateImageBehavior = False
        Me.lvSpells.View = System.Windows.Forms.View.Details
        '
        'chID
        '
        Me.chID.Text = "ID"
        Me.chID.Width = 65
        '
        'chName
        '
        Me.chName.Text = "Name"
        Me.chName.Width = 156
        '
        'chDescription
        '
        Me.chDescription.Text = "Description"
        Me.chDescription.Width = 441
        '
        'chLevel
        '
        Me.chLevel.Text = "Level"
        Me.chLevel.Width = 64
        '
        'lblFilter
        '
        Me.lblFilter.AutoSize = True
        Me.lblFilter.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFilter.Location = New System.Drawing.Point(12, 9)
        Me.lblFilter.Name = "lblFilter"
        Me.lblFilter.Size = New System.Drawing.Size(39, 13)
        Me.lblFilter.TabIndex = 1
        Me.lblFilter.Text = "Filter:"
        '
        'txtName
        '
        Me.txtName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtName.Location = New System.Drawing.Point(15, 25)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(211, 20)
        Me.txtName.TabIndex = 2
        '
        'txtID
        '
        Me.txtID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtID.Location = New System.Drawing.Point(259, 25)
        Me.txtID.MaxLength = 8
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(62, 20)
        Me.txtID.TabIndex = 3
        '
        'txtFromLvl
        '
        Me.txtFromLvl.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFromLvl.Location = New System.Drawing.Point(369, 25)
        Me.txtFromLvl.MaxLength = 2
        Me.txtFromLvl.Name = "txtFromLvl"
        Me.txtFromLvl.Size = New System.Drawing.Size(19, 20)
        Me.txtFromLvl.TabIndex = 4
        Me.txtFromLvl.Text = "0"
        '
        'txtToLvl
        '
        Me.txtToLvl.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtToLvl.Location = New System.Drawing.Point(410, 25)
        Me.txtToLvl.MaxLength = 2
        Me.txtToLvl.Name = "txtToLvl"
        Me.txtToLvl.Size = New System.Drawing.Size(19, 20)
        Me.txtToLvl.TabIndex = 5
        Me.txtToLvl.Text = "0"
        '
        'lblID
        '
        Me.lblID.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblID.AutoSize = True
        Me.lblID.Location = New System.Drawing.Point(232, 28)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(21, 13)
        Me.lblID.TabIndex = 6
        Me.lblID.Text = "ID:"
        '
        'lblLevel
        '
        Me.lblLevel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLevel.AutoSize = True
        Me.lblLevel.Location = New System.Drawing.Point(327, 28)
        Me.lblLevel.Name = "lblLevel"
        Me.lblLevel.Size = New System.Drawing.Size(36, 13)
        Me.lblLevel.TabIndex = 7
        Me.lblLevel.Text = "Level:"
        '
        'lblLvlSeparator
        '
        Me.lblLvlSeparator.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLvlSeparator.AutoSize = True
        Me.lblLvlSeparator.Location = New System.Drawing.Point(394, 28)
        Me.lblLvlSeparator.Name = "lblLvlSeparator"
        Me.lblLvlSeparator.Size = New System.Drawing.Size(10, 13)
        Me.lblLvlSeparator.TabIndex = 8
        Me.lblLvlSeparator.Text = "-"
        '
        'cmdFilter
        '
        Me.cmdFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdFilter.Location = New System.Drawing.Point(644, 52)
        Me.cmdFilter.Name = "cmdFilter"
        Me.cmdFilter.Size = New System.Drawing.Size(100, 19)
        Me.cmdFilter.TabIndex = 9
        Me.cmdFilter.Text = "Filter"
        Me.cmdFilter.UseVisualStyleBackColor = True
        '
        'cmdReset
        '
        Me.cmdReset.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdReset.Location = New System.Drawing.Point(644, 28)
        Me.cmdReset.Name = "cmdReset"
        Me.cmdReset.Size = New System.Drawing.Size(100, 19)
        Me.cmdReset.TabIndex = 10
        Me.cmdReset.Text = "Reset"
        Me.cmdReset.UseVisualStyleBackColor = True
        '
        'pSpellInfo
        '
        Me.pSpellInfo.AutoSize = True
        Me.pSpellInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pSpellInfo.BackColor = System.Drawing.Color.Transparent
        Me.pSpellInfo.BackgroundImage = CType(resources.GetObject("pSpellInfo.BackgroundImage"), System.Drawing.Image)
        Me.pSpellInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pSpellInfo.Controls.Add(Me.lblSpellDescription)
        Me.pSpellInfo.Controls.Add(Me.lblSpellRange)
        Me.pSpellInfo.Controls.Add(Me.lblSpellCooldown)
        Me.pSpellInfo.Controls.Add(Me.lblSpellCasttime)
        Me.pSpellInfo.Controls.Add(Me.lblSpellManacost)
        Me.pSpellInfo.Controls.Add(Me.lblSpellRank)
        Me.pSpellInfo.Controls.Add(Me.lblSpellName)
        Me.pSpellInfo.Location = New System.Drawing.Point(284, 159)
        Me.pSpellInfo.MaximumSize = New System.Drawing.Size(389, 403)
        Me.pSpellInfo.MinimumSize = New System.Drawing.Size(389, 67)
        Me.pSpellInfo.Name = "pSpellInfo"
        Me.pSpellInfo.Padding = New System.Windows.Forms.Padding(0, 0, 0, 14)
        Me.pSpellInfo.Size = New System.Drawing.Size(389, 125)
        Me.pSpellInfo.TabIndex = 12
        Me.pSpellInfo.Visible = False
        '
        'lblSpellDescription
        '
        Me.lblSpellDescription.AutoSize = True
        Me.lblSpellDescription.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellDescription.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellDescription.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(209, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblSpellDescription.Location = New System.Drawing.Point(14, 89)
        Me.lblSpellDescription.MaximumSize = New System.Drawing.Size(360, 300)
        Me.lblSpellDescription.MinimumSize = New System.Drawing.Size(360, 22)
        Me.lblSpellDescription.Name = "lblSpellDescription"
        Me.lblSpellDescription.Size = New System.Drawing.Size(360, 22)
        Me.lblSpellDescription.TabIndex = 6
        Me.lblSpellDescription.Text = "Description."
        '
        'lblSpellRange
        '
        Me.lblSpellRange.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellRange.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellRange.ForeColor = System.Drawing.Color.White
        Me.lblSpellRange.Location = New System.Drawing.Point(197, 42)
        Me.lblSpellRange.Name = "lblSpellRange"
        Me.lblSpellRange.Size = New System.Drawing.Size(189, 18)
        Me.lblSpellRange.TabIndex = 3
        Me.lblSpellRange.Text = "1 yd range"
        Me.lblSpellRange.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblSpellCooldown
        '
        Me.lblSpellCooldown.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellCooldown.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellCooldown.ForeColor = System.Drawing.Color.White
        Me.lblSpellCooldown.Location = New System.Drawing.Point(197, 66)
        Me.lblSpellCooldown.Name = "lblSpellCooldown"
        Me.lblSpellCooldown.Size = New System.Drawing.Size(189, 18)
        Me.lblSpellCooldown.TabIndex = 5
        Me.lblSpellCooldown.Text = "1 min cooldown"
        Me.lblSpellCooldown.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblSpellCasttime
        '
        Me.lblSpellCasttime.AutoSize = True
        Me.lblSpellCasttime.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellCasttime.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellCasttime.ForeColor = System.Drawing.Color.White
        Me.lblSpellCasttime.Location = New System.Drawing.Point(13, 66)
        Me.lblSpellCasttime.Name = "lblSpellCasttime"
        Me.lblSpellCasttime.Size = New System.Drawing.Size(68, 18)
        Me.lblSpellCasttime.TabIndex = 4
        Me.lblSpellCasttime.Text = "Instant"
        '
        'lblSpellManacost
        '
        Me.lblSpellManacost.AutoSize = True
        Me.lblSpellManacost.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellManacost.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellManacost.ForeColor = System.Drawing.Color.White
        Me.lblSpellManacost.Location = New System.Drawing.Point(13, 42)
        Me.lblSpellManacost.Name = "lblSpellManacost"
        Me.lblSpellManacost.Size = New System.Drawing.Size(69, 18)
        Me.lblSpellManacost.TabIndex = 2
        Me.lblSpellManacost.Text = "1 mana"
        '
        'lblSpellRank
        '
        Me.lblSpellRank.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellRank.Font = New System.Drawing.Font("Verdana", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellRank.ForeColor = System.Drawing.Color.FromArgb(CType(CType(157, Byte), Integer), CType(CType(157, Byte), Integer), CType(CType(157, Byte), Integer))
        Me.lblSpellRank.Location = New System.Drawing.Point(291, 12)
        Me.lblSpellRank.Name = "lblSpellRank"
        Me.lblSpellRank.Size = New System.Drawing.Size(95, 23)
        Me.lblSpellRank.TabIndex = 1
        Me.lblSpellRank.Text = "Rank 1"
        Me.lblSpellRank.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblSpellName
        '
        Me.lblSpellName.AutoSize = True
        Me.lblSpellName.BackColor = System.Drawing.Color.Transparent
        Me.lblSpellName.Font = New System.Drawing.Font("Verdana", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpellName.ForeColor = System.Drawing.Color.White
        Me.lblSpellName.Location = New System.Drawing.Point(12, 12)
        Me.lblSpellName.Name = "lblSpellName"
        Me.lblSpellName.Size = New System.Drawing.Size(58, 23)
        Me.lblSpellName.TabIndex = 0
        Me.lblSpellName.Text = "Spell"
        '
        'lblAttributes
        '
        Me.lblAttributes.AutoSize = True
        Me.lblAttributes.Location = New System.Drawing.Point(12, 55)
        Me.lblAttributes.Name = "lblAttributes"
        Me.lblAttributes.Size = New System.Drawing.Size(54, 13)
        Me.lblAttributes.TabIndex = 13
        Me.lblAttributes.Text = "Attributes:"
        '
        'txtAttributes
        '
        Me.txtAttributes.Location = New System.Drawing.Point(72, 52)
        Me.txtAttributes.MaxLength = 10
        Me.txtAttributes.Name = "txtAttributes"
        Me.txtAttributes.Size = New System.Drawing.Size(67, 20)
        Me.txtAttributes.TabIndex = 14
        '
        'lblAttributesEx
        '
        Me.lblAttributesEx.AutoSize = True
        Me.lblAttributesEx.Location = New System.Drawing.Point(145, 55)
        Me.lblAttributesEx.Name = "lblAttributesEx"
        Me.lblAttributesEx.Size = New System.Drawing.Size(66, 13)
        Me.lblAttributesEx.TabIndex = 15
        Me.lblAttributesEx.Text = "AttributesEx:"
        '
        'lblAttributesEx2
        '
        Me.lblAttributesEx2.AutoSize = True
        Me.lblAttributesEx2.Location = New System.Drawing.Point(290, 55)
        Me.lblAttributesEx2.Name = "lblAttributesEx2"
        Me.lblAttributesEx2.Size = New System.Drawing.Size(72, 13)
        Me.lblAttributesEx2.TabIndex = 16
        Me.lblAttributesEx2.Text = "AttributesEx2:"
        '
        'txtAttributesEx
        '
        Me.txtAttributesEx.Location = New System.Drawing.Point(217, 52)
        Me.txtAttributesEx.MaxLength = 10
        Me.txtAttributesEx.Name = "txtAttributesEx"
        Me.txtAttributesEx.Size = New System.Drawing.Size(67, 20)
        Me.txtAttributesEx.TabIndex = 17
        '
        'txtAttributesEx2
        '
        Me.txtAttributesEx2.Location = New System.Drawing.Point(362, 52)
        Me.txtAttributesEx2.MaxLength = 10
        Me.txtAttributesEx2.Name = "txtAttributesEx2"
        Me.txtAttributesEx2.Size = New System.Drawing.Size(67, 20)
        Me.txtAttributesEx2.TabIndex = 18
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 562)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(756, 22)
        Me.StatusStrip1.TabIndex = 19
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblStatus
        '
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 17)
        '
        'cmdOpenCompare
        '
        Me.cmdOpenCompare.Location = New System.Drawing.Point(644, 3)
        Me.cmdOpenCompare.Name = "cmdOpenCompare"
        Me.cmdOpenCompare.Size = New System.Drawing.Size(100, 19)
        Me.cmdOpenCompare.TabIndex = 20
        Me.cmdOpenCompare.Text = "Open Compare"
        Me.cmdOpenCompare.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(756, 584)
        Me.Controls.Add(Me.cmdOpenCompare)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.txtAttributesEx2)
        Me.Controls.Add(Me.txtAttributesEx)
        Me.Controls.Add(Me.lblAttributesEx2)
        Me.Controls.Add(Me.lblAttributesEx)
        Me.Controls.Add(Me.txtAttributes)
        Me.Controls.Add(Me.lblAttributes)
        Me.Controls.Add(Me.pSpellInfo)
        Me.Controls.Add(Me.cmdReset)
        Me.Controls.Add(Me.cmdFilter)
        Me.Controls.Add(Me.lblLvlSeparator)
        Me.Controls.Add(Me.lblLevel)
        Me.Controls.Add(Me.lblID)
        Me.Controls.Add(Me.txtToLvl)
        Me.Controls.Add(Me.txtFromLvl)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.lblFilter)
        Me.Controls.Add(Me.lvSpells)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "Spell Browser v0.1 by UniX"
        Me.pSpellInfo.ResumeLayout(False)
        Me.pSpellInfo.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvSpells As System.Windows.Forms.ListView
    Friend WithEvents lblFilter As System.Windows.Forms.Label
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents txtFromLvl As System.Windows.Forms.TextBox
    Friend WithEvents txtToLvl As System.Windows.Forms.TextBox
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents lblLevel As System.Windows.Forms.Label
    Friend WithEvents lblLvlSeparator As System.Windows.Forms.Label
    Friend WithEvents cmdFilter As System.Windows.Forms.Button
    Friend WithEvents cmdReset As System.Windows.Forms.Button
    Friend WithEvents chID As System.Windows.Forms.ColumnHeader
    Friend WithEvents chName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents chLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents pSpellInfo As System.Windows.Forms.Panel
    Friend WithEvents lblSpellName As System.Windows.Forms.Label
    Friend WithEvents lblSpellRank As System.Windows.Forms.Label
    Friend WithEvents lblSpellManacost As System.Windows.Forms.Label
    Friend WithEvents lblSpellDescription As System.Windows.Forms.Label
    Friend WithEvents lblSpellCooldown As System.Windows.Forms.Label
    Friend WithEvents lblSpellCasttime As System.Windows.Forms.Label
    Friend WithEvents lblSpellRange As System.Windows.Forms.Label
    Friend WithEvents lblAttributes As System.Windows.Forms.Label
    Friend WithEvents txtAttributes As System.Windows.Forms.TextBox
    Friend WithEvents lblAttributesEx As System.Windows.Forms.Label
    Friend WithEvents lblAttributesEx2 As System.Windows.Forms.Label
    Friend WithEvents txtAttributesEx As System.Windows.Forms.TextBox
    Friend WithEvents txtAttributesEx2 As System.Windows.Forms.TextBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents lblStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents cmdOpenCompare As System.Windows.Forms.Button

End Class
