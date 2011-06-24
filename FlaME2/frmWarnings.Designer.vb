<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWarnings
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
        Me.lstWarnings = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'lstWarnings
        '
        Me.lstWarnings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstWarnings.FormattingEnabled = True
        Me.lstWarnings.ItemHeight = 16
        Me.lstWarnings.Location = New System.Drawing.Point(0, 0)
        Me.lstWarnings.Margin = New System.Windows.Forms.Padding(4)
        Me.lstWarnings.Name = "lstWarnings"
        Me.lstWarnings.Size = New System.Drawing.Size(324, 222)
        Me.lstWarnings.TabIndex = 1
        '
        'frmWarnings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(324, 222)
        Me.Controls.Add(Me.lstWarnings)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "frmWarnings"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstWarnings As System.Windows.Forms.ListBox
End Class
