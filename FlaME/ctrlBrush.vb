Public Class ctrlBrush
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Private Brush As clsBrush

    Public Sub New(ByVal NewBrush As clsBrush)
        InitializeComponent()

		Brush = NewBrush

		UpdateControlValues()

		AddHandler nudRadius.LostFocus, AddressOf nudRadius_LostFocus
        AddHandler nudRadius.ValueChanged, AddressOf nudRadius_LostFocus
    End Sub

    Public Sub UpdateControlValues()

        nudRadius.Enabled = False
        tabShape.Enabled = False

        If Brush Is Nothing Then
            Exit Sub
        End If

        nudRadius.Value = CDec(Brush.Radius)
        Select Case Brush.Shape
            Case clsBrush.enumShape.Circle
                tabShape.SelectedIndex = 0
            Case clsBrush.enumShape.Square
                tabShape.SelectedIndex = 1
        End Select
        nudRadius.Enabled = True
        tabShape.Enabled = True
    End Sub

    Private Sub nudRadius_LostFocus(ByVal sender As Object, ByVal e As EventArgs) 
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
            If NewRadius < 0.0# Then
                NewRadius = 0.0#
                nudRadius.Value = CDec(NewRadius)
            ElseIf NewRadius > 512.0# Then
                NewRadius = 512.0#
                nudRadius.Value = CDec(NewRadius)
            End If
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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.tabShape = New System.Windows.Forms.TabControl()
        Me.TabPage37 = New System.Windows.Forms.TabPage()
        Me.TabPage38 = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.nudRadius = New System.Windows.Forms.NumericUpDown()
        Me.tabShape.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabShape
        '
        Me.tabShape.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabShape.Controls.Add(Me.TabPage37)
        Me.tabShape.Controls.Add(Me.TabPage38)
        Me.tabShape.ItemSize = New System.Drawing.Size(64, 24)
        Me.tabShape.Location = New System.Drawing.Point(139, 0)
        Me.tabShape.Margin = New System.Windows.Forms.Padding(0)
        Me.tabShape.Multiline = True
        Me.tabShape.Name = "tabShape"
        Me.tabShape.Padding = New System.Drawing.Point(0, 0)
        Me.tabShape.SelectedIndex = 0
        Me.tabShape.Size = New System.Drawing.Size(161, 32)
        Me.tabShape.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabShape.TabIndex = 41
        '
        'TabPage37
        '
        Me.TabPage37.Location = New System.Drawing.Point(4, 28)
        Me.TabPage37.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage37.Name = "TabPage37"
        Me.TabPage37.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage37.Size = New System.Drawing.Size(153, 0)
        Me.TabPage37.TabIndex = 0
        Me.TabPage37.Text = "Circular"
        Me.TabPage37.UseVisualStyleBackColor = True
        '
        'TabPage38
        '
        Me.TabPage38.Location = New System.Drawing.Point(4, 28)
        Me.TabPage38.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage38.Name = "TabPage38"
        Me.TabPage38.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage38.Size = New System.Drawing.Size(153, 0)
        Me.TabPage38.TabIndex = 1
        Me.TabPage38.Text = "Square"
        Me.TabPage38.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(52, 20)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "Radius"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label1.UseCompatibleTextRendering = True
        '
        'nudRadius
        '
        Me.nudRadius.DecimalPlaces = 1
        Me.nudRadius.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.nudRadius.Location = New System.Drawing.Point(60, 0)
        Me.nudRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.nudRadius.Maximum = New Decimal(New Integer() {512, 0, 0, 0})
        Me.nudRadius.Name = "nudRadius"
        Me.nudRadius.Size = New System.Drawing.Size(75, 22)
        Me.nudRadius.TabIndex = 40
        Me.nudRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ctrlBrush
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tabShape)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.nudRadius)
        Me.Name = "ctrlBrush"
        Me.Size = New System.Drawing.Size(334, 35)
        Me.tabShape.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents tabShape As System.Windows.Forms.TabControl
    Public WithEvents TabPage37 As System.Windows.Forms.TabPage
    Public WithEvents TabPage38 As System.Windows.Forms.TabPage
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents nudRadius As System.Windows.Forms.NumericUpDown
#End If
End Class