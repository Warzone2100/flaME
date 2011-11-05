Public Class ctrlColour
#If MonoDevelop <> 0.0# Then
	Inherits UserControl
#End If

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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.pnlColour = New System.Windows.Forms.Panel()
        Me.nudAlpha = New System.Windows.Forms.NumericUpDown()
        Me.SuspendLayout()
        '
        'pnlColour
        '
        Me.pnlColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlColour.Location = New System.Drawing.Point(0, 0)
        Me.pnlColour.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlColour.Name = "pnlColour"
        Me.pnlColour.Size = New System.Drawing.Size(51, 24)
        Me.pnlColour.TabIndex = 1
        '
        'nudAlpha
        '
        Me.nudAlpha.DecimalPlaces = 2
        Me.nudAlpha.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.nudAlpha.Location = New System.Drawing.Point(54, 0)
        Me.nudAlpha.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudAlpha.Name = "nudAlpha"
        Me.nudAlpha.Size = New System.Drawing.Size(50, 22)
        Me.nudAlpha.TabIndex = 2
        Me.nudAlpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ctrlColour
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.nudAlpha)
        Me.Controls.Add(Me.pnlColour)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlColour"
        Me.Size = New System.Drawing.Size(211, 39)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlColour As System.Windows.Forms.Panel
    Friend WithEvents nudAlpha As System.Windows.Forms.NumericUpDown
#End If
End Class