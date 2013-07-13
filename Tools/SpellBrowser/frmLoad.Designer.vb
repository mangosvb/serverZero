<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLoad
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLoad))
        Me.lblAction = New System.Windows.Forms.Label
        Me.pbAction = New System.Windows.Forms.ProgressBar
        Me.SuspendLayout()
        '
        'lblAction
        '
        Me.lblAction.AutoSize = True
        Me.lblAction.Location = New System.Drawing.Point(12, 9)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(37, 13)
        Me.lblAction.TabIndex = 0
        Me.lblAction.Text = "Action"
        '
        'pbAction
        '
        Me.pbAction.Location = New System.Drawing.Point(12, 25)
        Me.pbAction.Name = "pbAction"
        Me.pbAction.Size = New System.Drawing.Size(375, 18)
        Me.pbAction.TabIndex = 1
        '
        'frmLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(399, 54)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbAction)
        Me.Controls.Add(Me.lblAction)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmLoad"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Loading Spell Browser..."
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblAction As System.Windows.Forms.Label
    Friend WithEvents pbAction As System.Windows.Forms.ProgressBar
End Class
