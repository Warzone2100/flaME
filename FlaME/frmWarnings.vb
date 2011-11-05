Public Class frmWarnings
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Public Sub New(ByVal Result As clsResult, ByVal WindowTitle As String)
        InitializeComponent()

        Icon = ProgramIcon

        Text = WindowTitle

        Dim A As Integer
        lstWarnings.Items.Clear()
        For A = 0 To Result.Problems.ItemCount - 1
            lstWarnings.Items.Add("(Problem) " & Result.Problems.Item(A))
        Next
        For A = 0 To Result.Warnings.ItemCount - 1
            lstWarnings.Items.Add("(Warning) " & Result.Warnings.Item(A))
        Next
    End Sub

#If MonoDevelop <> 0.0# Then
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
        Me.Name = "frmWarnings"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents lstWarnings As System.Windows.Forms.ListBox
#End If
End Class