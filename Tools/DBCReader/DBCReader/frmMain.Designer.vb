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
        Me.components = New System.ComponentModel.Container()
        Me.txtFile = New System.Windows.Forms.TextBox()
        Me.cmdBrowse = New System.Windows.Forms.Button()
        Me.DBCData = New System.Windows.Forms.ListView()
        Me.ProgressBar = New System.Windows.Forms.ProgressBar()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.cmbColumn = New System.Windows.Forms.ComboBox()
        Me.txtQuery = New System.Windows.Forms.TextBox()
        Me.cmdSearch = New System.Windows.Forms.Button()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtFile
        '
        Me.txtFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFile.Enabled = False
        Me.txtFile.Location = New System.Drawing.Point(2, 8)
        Me.txtFile.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtFile.Name = "txtFile"
        Me.txtFile.Size = New System.Drawing.Size(781, 23)
        Me.txtFile.TabIndex = 1
        '
        'cmdBrowse
        '
        Me.cmdBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowse.Location = New System.Drawing.Point(791, 7)
        Me.cmdBrowse.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cmdBrowse.Name = "cmdBrowse"
        Me.cmdBrowse.Size = New System.Drawing.Size(104, 23)
        Me.cmdBrowse.TabIndex = 2
        Me.cmdBrowse.Text = "Browse"
        Me.cmdBrowse.UseVisualStyleBackColor = True
        '
        'DBCData
        '
        Me.DBCData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DBCData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DBCData.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.DBCData.FullRowSelect = True
        Me.DBCData.GridLines = True
        Me.DBCData.HideSelection = False
        Me.DBCData.Location = New System.Drawing.Point(2, 68)
        Me.DBCData.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.DBCData.Name = "DBCData"
        Me.DBCData.Size = New System.Drawing.Size(892, 437)
        Me.DBCData.TabIndex = 3
        Me.DBCData.UseCompatibleStateImageBehavior = False
        Me.DBCData.View = System.Windows.Forms.View.Details
        '
        'ProgressBar
        '
        Me.ProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar.Location = New System.Drawing.Point(2, 509)
        Me.ProgressBar.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Size = New System.Drawing.Size(891, 23)
        Me.ProgressBar.Step = 2
        Me.ProgressBar.TabIndex = 4
        '
        'cmbColumn
        '
        Me.cmbColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbColumn.FormattingEnabled = True
        Me.cmbColumn.Location = New System.Drawing.Point(2, 37)
        Me.cmbColumn.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cmbColumn.Name = "cmbColumn"
        Me.cmbColumn.Size = New System.Drawing.Size(177, 23)
        Me.cmbColumn.TabIndex = 5
        '
        'txtQuery
        '
        Me.txtQuery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtQuery.Location = New System.Drawing.Point(187, 38)
        Me.txtQuery.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.txtQuery.Name = "txtQuery"
        Me.txtQuery.Size = New System.Drawing.Size(597, 23)
        Me.txtQuery.TabIndex = 6
        '
        'cmdSearch
        '
        Me.cmdSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSearch.Location = New System.Drawing.Point(791, 38)
        Me.cmdSearch.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cmdSearch.Name = "cmdSearch"
        Me.cmdSearch.Size = New System.Drawing.Size(104, 23)
        Me.cmdSearch.TabIndex = 7
        Me.cmdSearch.Text = "Search"
        Me.cmdSearch.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(898, 533)
        Me.Controls.Add(Me.cmdSearch)
        Me.Controls.Add(Me.txtQuery)
        Me.Controls.Add(Me.cmbColumn)
        Me.Controls.Add(Me.ProgressBar)
        Me.Controls.Add(Me.DBCData)
        Me.Controls.Add(Me.cmdBrowse)
        Me.Controls.Add(Me.txtFile)
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Name = "frmMain"
        Me.Text = "DBC (Database Code) Reader"
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtFile As System.Windows.Forms.TextBox
    Friend WithEvents cmdBrowse As System.Windows.Forms.Button
    Friend WithEvents DBCData As System.Windows.Forms.ListView
    Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents BindingSource1 As System.Windows.Forms.BindingSource
    Friend WithEvents cmbColumn As System.Windows.Forms.ComboBox
    Friend WithEvents txtQuery As System.Windows.Forms.TextBox
    Friend WithEvents cmdSearch As System.Windows.Forms.Button

End Class
