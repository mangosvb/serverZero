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
        Me.btnExtractUpdateFields = New System.Windows.Forms.Button
        Me.btnExtractOpcodes = New System.Windows.Forms.Button
        Me.btnExtractSpellFailedReasons = New System.Windows.Forms.Button
        Me.btnExtractChatTypes = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btnExtractUpdateFields
        '
        Me.btnExtractUpdateFields.Location = New System.Drawing.Point(12, 74)
        Me.btnExtractUpdateFields.Name = "btnExtractUpdateFields"
        Me.btnExtractUpdateFields.Size = New System.Drawing.Size(161, 56)
        Me.btnExtractUpdateFields.TabIndex = 0
        Me.btnExtractUpdateFields.Text = "Extract UPDATE FIELDs"
        Me.btnExtractUpdateFields.UseVisualStyleBackColor = True
        '
        'btnExtractOpcodes
        '
        Me.btnExtractOpcodes.Location = New System.Drawing.Point(12, 12)
        Me.btnExtractOpcodes.Name = "btnExtractOpcodes"
        Me.btnExtractOpcodes.Size = New System.Drawing.Size(161, 56)
        Me.btnExtractOpcodes.TabIndex = 1
        Me.btnExtractOpcodes.Text = "Extract OPCODEs"
        Me.btnExtractOpcodes.UseVisualStyleBackColor = True
        '
        'btnExtractSpellFailedReasons
        '
        Me.btnExtractSpellFailedReasons.Location = New System.Drawing.Point(12, 136)
        Me.btnExtractSpellFailedReasons.Name = "btnExtractSpellFailedReasons"
        Me.btnExtractSpellFailedReasons.Size = New System.Drawing.Size(161, 56)
        Me.btnExtractSpellFailedReasons.TabIndex = 2
        Me.btnExtractSpellFailedReasons.Text = "Extract SpellFailedReasons"
        Me.btnExtractSpellFailedReasons.UseVisualStyleBackColor = True
        '
        'btnExtractChatTypes
        '
        Me.btnExtractChatTypes.Location = New System.Drawing.Point(12, 198)
        Me.btnExtractChatTypes.Name = "btnExtractChatTypes"
        Me.btnExtractChatTypes.Size = New System.Drawing.Size(161, 56)
        Me.btnExtractChatTypes.TabIndex = 3
        Me.btnExtractChatTypes.Text = "Extract ChatTypes"
        Me.btnExtractChatTypes.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(188, 263)
        Me.Controls.Add(Me.btnExtractChatTypes)
        Me.Controls.Add(Me.btnExtractSpellFailedReasons)
        Me.Controls.Add(Me.btnExtractOpcodes)
        Me.Controls.Add(Me.btnExtractUpdateFields)
        Me.Name = "frmMain"
        Me.Text = "WoW Extractor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnExtractUpdateFields As System.Windows.Forms.Button
    Friend WithEvents btnExtractOpcodes As System.Windows.Forms.Button
    Friend WithEvents btnExtractSpellFailedReasons As System.Windows.Forms.Button
    Friend WithEvents btnExtractChatTypes As System.Windows.Forms.Button

End Class
