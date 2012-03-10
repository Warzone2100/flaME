Public Class frmWarnings

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
End Class