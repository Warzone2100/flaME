Public Class ctrlPlayerNum
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public tsbNumber(10) As ToolStripButton

    Private _SelectedPlayerNum As Integer = -1
    Public Property SelectedPlayerNum As Integer
        Get
            Return _SelectedPlayerNum
        End Get
        Set(ByVal value As Integer)
            If value > 10 Then
                Stop
                Exit Property
            End If
            If value = _SelectedPlayerNum Then
                Exit Property
            End If
            ChangeSelected(value)
        End Set
    End Property

    Public Event SelectedPlayerNumChanged()

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim A As Integer
        Dim B As Integer
        Dim ButtonsPerRow As Integer = 5

        For A = 0 To ButtonsPerRow - 1
            tsbNumber(A) = New ToolStripButton
            tsbNumber(A).DisplayStyle = ToolStripItemDisplayStyle.Text
            tsbNumber(A).Text = A
            tsbNumber(A).AutoToolTip = False
            tsbNumber(A).Tag = A
            AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum1.Items.Add(tsbNumber(A))

            B = A + ButtonsPerRow
            tsbNumber(B) = New ToolStripButton
            tsbNumber(B).DisplayStyle = ToolStripItemDisplayStyle.Text
            tsbNumber(B).Text = B
            tsbNumber(B).AutoToolTip = False
            tsbNumber(B).Tag = B
            AddHandler tsbNumber(B).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum2.Items.Add(tsbNumber(B))
        Next

        A = 10

        tsbNumber(A) = New ToolStripButton
        tsbNumber(A).DisplayStyle = ToolStripItemDisplayStyle.Text
        tsbNumber(A).Text = A
        tsbNumber(A).Tag = A
        tsbNumber(A).AutoToolTip = False
        AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
        tsPlayerNum2.Items.Add(tsbNumber(A))

        Width = 24 * 6
        Height = 25 * 2
    End Sub

    Private Sub tsbNumber_Clicked(ByVal sender As Object, ByVal e As EventArgs)
        Dim tsb As ToolStripButton = sender
        Dim Num As Integer = tsb.Tag

        If Num = _SelectedPlayerNum Then
            Exit Sub
        End If

        ChangeSelected(Num)
    End Sub

    Private Sub ChangeSelected(ByVal Num As Integer)

        If _SelectedPlayerNum >= 0 Then
            tsbNumber(_SelectedPlayerNum).Checked = False
        End If
        _SelectedPlayerNum = Num
        If _SelectedPlayerNum >= 0 Then
            tsbNumber(_SelectedPlayerNum).Checked = True
        End If

        RaiseEvent SelectedPlayerNumChanged()
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.tsPlayerNum1 = New System.Windows.Forms.ToolStrip()
        Me.tsPlayerNum2 = New System.Windows.Forms.ToolStrip()
        Me.SuspendLayout()
        '
        'tsPlayerNum1
        '
        Me.tsPlayerNum1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsPlayerNum1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsPlayerNum1.Location = New System.Drawing.Point(0, 0)
        Me.tsPlayerNum1.Name = "tsPlayerNum1"
        Me.tsPlayerNum1.Size = New System.Drawing.Size(56, 25)
        Me.tsPlayerNum1.TabIndex = 0
        Me.tsPlayerNum1.Text = "ToolStrip1"
        '
        'tsPlayerNum2
        '
        Me.tsPlayerNum2.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsPlayerNum2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsPlayerNum2.Location = New System.Drawing.Point(0, 25)
        Me.tsPlayerNum2.Name = "tsPlayerNum2"
        Me.tsPlayerNum2.Size = New System.Drawing.Size(56, 25)
        Me.tsPlayerNum2.TabIndex = 1
        Me.tsPlayerNum2.Text = "ToolStrip1"
        '
        'ctrlPlayerNum
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.tsPlayerNum2)
        Me.Controls.Add(Me.tsPlayerNum1)
        Me.Name = "ctrlPlayerNum"
        Me.Size = New System.Drawing.Size(56, 50)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tsPlayerNum1 As System.Windows.Forms.ToolStrip
    Friend WithEvents tsPlayerNum2 As System.Windows.Forms.ToolStrip
#End If
End Class