Public Class ctrlBrush

    Private Brush As clsBrush

    Public Sub New(ByVal NewBrush As clsBrush)
        InitializeComponent()

		Brush = NewBrush

		UpdateControlValues()

        AddHandler nudRadius.ValueChanged, AddressOf nudRadius_Changed
        AddHandler nudRadius.Leave, AddressOf nudRadius_Changed
    End Sub

    Public Sub UpdateControlValues()

        nudRadius.Enabled = False
        tabShape.Enabled = False

        If Brush Is Nothing Then
            Exit Sub
        End If

        nudRadius.Value = CDec(Clamp_dbl(Brush.Radius, CDbl(nudRadius.Minimum), CDbl(nudRadius.Maximum)))
        Select Case Brush.Shape
            Case clsBrush.enumShape.Circle
                tabShape.SelectedIndex = 0
            Case clsBrush.enumShape.Square
                tabShape.SelectedIndex = 1
        End Select
        nudRadius.Enabled = True
        tabShape.Enabled = True
    End Sub

    Private Sub nudRadius_Changed(ByVal sender As Object, ByVal e As EventArgs)
        If Not nudRadius.Enabled Then
            Exit Sub
        End If

        nudRadius.Enabled = False

        Dim NewRadius As Double
        Dim Converted As Boolean = False
        Try
            NewRadius = CDbl(nudRadius.Value)
            Converted = True
        Catch ex As Exception

        End Try
        If Converted Then
            Brush.Radius = NewRadius
        End If

        nudRadius.Enabled = True
    End Sub

    Private Sub tabShape_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabShape.SelectedIndexChanged
        If Not tabShape.Enabled Then
            Exit Sub
        End If

        Select Case tabShape.SelectedIndex
            Case 0
                Brush.Shape = clsBrush.enumShape.Circle
            Case 1
                Brush.Shape = clsBrush.enumShape.Square
        End Select
    End Sub
End Class