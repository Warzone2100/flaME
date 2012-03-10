Public Class ctrlColour

    Private Colour As clsRGB_sng
    Private ColourColor As Color

    Private ColourBoxGraphics As Graphics

    Public Sub New(ByVal NewColour As clsRGB_sng)
        InitializeComponent()

        If NewColour Is Nothing Then
            Stop
            Visible = False
            Exit Sub
        End If

        Colour = NewColour
        Dim Red As Integer = CInt(Clamp_dbl(Colour.Red * 255.0#, 0.0#, 255.0#))
        Dim Green As Integer = CInt(Clamp_dbl(Colour.Green * 255.0#, 0.0#, 255.0#))
        Dim Blue As Integer = CInt(Clamp_dbl(Colour.Blue * 255.0#, 0.0#, 255.0#))
        ColourColor = ColorTranslator.FromOle(OSRGB(Red, Green, Blue))

        Select Case Colour.ColourType
            Case clsRGB_sng.enumType.RGB
                nudAlpha.Visible = False
            Case clsRGB_sng.enumType.RGBA
                nudAlpha.Value = CDec(CType(Colour, clsRGBA_sng).Alpha)
                AddHandler nudAlpha.ValueChanged, AddressOf nudAlpha_Changed
                AddHandler nudAlpha.Leave, AddressOf nudAlpha_Changed
            Case Else
                Stop
        End Select

        ColourBoxGraphics = pnlColour.CreateGraphics

        ColourBoxRedraw()
    End Sub

    Private Sub SelectColour(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pnlColour.Click
        Dim ColourSelect As New Windows.Forms.ColorDialog

        ColourSelect.Color = ColourColor
        Dim Result As DialogResult = ColourSelect.ShowDialog()
        If Result <> DialogResult.OK Then
            Exit Sub
        End If
        ColourColor = ColourSelect.Color
        Colour.Red = CSng(ColourColor.R / 255.0#)
        Colour.Green = CSng(ColourColor.G / 255.0#)
        Colour.Blue = CSng(ColourColor.B / 255.0#)
        ColourBoxRedraw()
    End Sub

    Private Sub nudAlpha_Changed(ByVal sender As Object, ByVal e As EventArgs)

        CType(Colour, clsRGBA_sng).Alpha = nudAlpha.Value
    End Sub

    Private Sub pnlColour_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles pnlColour.Paint

        ColourBoxRedraw()
    End Sub

    Private Sub ColourBoxRedraw()

        ColourBoxGraphics.Clear(ColourColor)
    End Sub
End Class