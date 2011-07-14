Public Class frmSplash
#If MonoDevelop <> 0.0# Then
	Inherits Form
#End If

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent() 'required for monodevelop too

        ' Add any initialization after the InitializeComponent() call.
        Text = ProgramName & " " & ProgramVersionNumber & " Loading"
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplash))
        Me.SuspendLayout()
        '
        'frmSplash
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(396, 197)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmSplash"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.TransparencyKey = System.Drawing.Color.White
        Me.ResumeLayout(False)

    End Sub
#End If
End Class