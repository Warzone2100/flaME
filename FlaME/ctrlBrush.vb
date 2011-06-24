Public Class ctrlBrush
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Private Brush As clsBrush

    Public Sub New(ByVal NewBrush As clsBrush)
        InitializeComponent()

#If Mono <> 0.0# Then
        AddHandler nudRadius.ValueChanged, AddressOf nudRadius_LostFocus
#End If

        Brush = NewBrush

        UpdateControlValues()
    End Sub

    Public Sub UpdateControlValues()

        nudRadius.Enabled = False
        tabShape.Enabled = False

        If Brush Is Nothing Then
            Exit Sub
        End If

        nudRadius.Value = Brush.Radius
        Select Case Brush.Shape
            Case clsBrush.enumShape.Circle
                tabShape.SelectedIndex = 0
            Case clsBrush.enumShape.Square
                tabShape.SelectedIndex = 1
        End Select
        nudRadius.Enabled = True
        tabShape.Enabled = True
    End Sub

    Private Sub nudRadius_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles nudRadius.LostFocus
        If Not nudRadius.Enabled Then
            Exit Sub
        End If

        nudRadius.Enabled = False

        Dim NewRadius As Double = Val(nudRadius.Value)
        If NewRadius < 0.0# Then
            NewRadius = 0.0#
            nudRadius.Value = NewRadius
        ElseIf NewRadius > 512.0# Then
            NewRadius = 512.0#
            nudRadius.Value = NewRadius
        End If

        Brush.Radius = NewRadius

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