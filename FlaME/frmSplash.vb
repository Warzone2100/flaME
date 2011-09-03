Public Class frmSplash
#If MonoDevelop <> 0.0# Then
	Inherits Form
#End If

    Public Sub New()
        InitializeComponent()

        Text = ProgramName & " " & ProgramVersionNumber & " Loading"
    End Sub

#If MonoDevelop <> 0.0# Then
    'not used in MonoDevelop
#End If
End Class