Public Class frmWZLoad

    Public Class clsOutput
        Public Result As Integer
    End Class

    Public lstMap_MapName() As String

    Public Output As clsOutput

    Public Structure sMapNameList
        Public Names() As String
    End Structure

    Public Sub New(ByRef MapNames As sMapNameList, ByVal NewOutput As clsOutput, ByVal FormTitle As String)
        InitializeComponent()

        Icon = ProgramIcon

        Output = NewOutput
        Output.Result = -1

        Dim A As Integer

        lstMap.Items.Clear()
        lstMap_MapName = MapNames.Names
        For A = 0 To MapNames.Names.GetUpperBound(0)
            lstMap.Items.Add(MapNames.Names(A))
        Next

        Text = FormTitle
    End Sub

    Private Sub lstMaps_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstMap.DoubleClick

        If lstMap.SelectedIndex >= 0 Then
            Output.Result = lstMap.SelectedIndex
            Close()
        End If
    End Sub
End Class