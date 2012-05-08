
Public Class frmWarnings

    Public Shared Images As New ImageList

    Public Sub New(Result As clsResult, WindowTitle As String)
        InitializeComponent()

        Icon = ProgramIcon

        Text = WindowTitle

        tvwWarnings.StateImageList = Images
        Result.MakeNodes(tvwWarnings.Nodes)
        tvwWarnings.ExpandAll()
    End Sub
End Class
