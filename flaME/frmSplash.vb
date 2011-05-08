Public Class frmSplash
#If MonoDevelop <> 0.0# Then
	Inherits Form
#End If

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent() 'required for monodevelop too

        ' Add any initialization after the InitializeComponent() call.
        Text = "flaME " & ProgramVersionNumber & " Loading"
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'frmSplash
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.BackgroundImage = New Bitmap("interface/splash.png") 'needs to be changed to interface path variable
        Me.ClientSize = New System.Drawing.Size(616, 316)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmSplash"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = ""
        Me.TransparencyKey = System.Drawing.Color.White
        Me.ResumeLayout(False)
    End Sub
#End If
End Class