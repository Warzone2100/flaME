Public Class ctrlPlayerNum
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public tsbNumber(10) As ToolStripButton

    Private _Target As clsMap.clsUnitGroupContainer

    Public Const ScavButtonNum As Integer = 10

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
            tsbNumber(A).Text = InvariantToString_int(A)
            tsbNumber(A).AutoToolTip = False
            AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum1.Items.Add(tsbNumber(A))

            B = A + ButtonsPerRow
            tsbNumber(B) = New ToolStripButton
            tsbNumber(B).DisplayStyle = ToolStripItemDisplayStyle.Text
            tsbNumber(B).Text = InvariantToString_int(B)
            tsbNumber(B).AutoToolTip = False
            AddHandler tsbNumber(B).Click, AddressOf tsbNumber_Clicked
            tsPlayerNum2.Items.Add(tsbNumber(B))
        Next

        A = 10

        tsbNumber(A) = New ToolStripButton
        tsbNumber(A).DisplayStyle = ToolStripItemDisplayStyle.Text
        tsbNumber(A).Text = "S"
        tsbNumber(A).AutoToolTip = False
        AddHandler tsbNumber(A).Click, AddressOf tsbNumber_Clicked
        tsPlayerNum2.Items.Add(tsbNumber(A))

        Width = 24 * 6
        Height = 25 * 2
    End Sub

    Private Sub tsbNumber_Clicked(ByVal sender As Object, ByVal e As EventArgs)

        If _Target Is Nothing Then
            Exit Sub
        End If

        Dim tsb As ToolStripButton = CType(sender, ToolStripButton)
        Dim tmpUnitGroup As clsMap.clsUnitGroup = CType(tsb.Tag, clsMap.clsUnitGroup)

        _Target.Item = tmpUnitGroup
    End Sub

    Public Property Target As clsMap.clsUnitGroupContainer
        Get
            Return _Target
        End Get
        Set(value As clsMap.clsUnitGroupContainer)
            If value Is _Target Then
                Exit Property
            End If
            If _Target IsNot Nothing Then
                RemoveHandler _Target.Changed, AddressOf SelectedChanged
            End If
            _Target = value
            If _Target IsNot Nothing Then
                AddHandler _Target.Changed, AddressOf SelectedChanged
            End If
            SelectedChanged()
        End Set
    End Property

    Private Sub SelectedChanged()

        Dim A As Integer
        Dim tmpUnitGroup As clsMap.clsUnitGroup

        If _Target Is Nothing Then
            tmpUnitGroup = Nothing
        Else
            tmpUnitGroup = _Target.Item
        End If

        If tmpUnitGroup Is Nothing Then
            For A = 0 To 10
                tsbNumber(A).Checked = False
            Next
        Else
            For A = 0 To 10
                tsbNumber(A).Checked = (CType(tsbNumber(A).Tag, clsMap.clsUnitGroup) Is tmpUnitGroup)
            Next
        End If
    End Sub

    Public Sub SetMap(ByVal NewMap As clsMap)
        Dim A As Integer

        If NewMap Is Nothing Then
            For A = 0 To PlayerCountMax - 1
                tsbNumber(A).Tag = Nothing
            Next
            tsbNumber(ScavButtonNum).Tag = Nothing
        Else
            For A = 0 To PlayerCountMax - 1
                tsbNumber(A).Tag = NewMap.UnitGroups(A)
            Next
            tsbNumber(ScavButtonNum).Tag = NewMap.ScavengerUnitGroup
        End If
        SelectedChanged()
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
    Public WithEvents tsPlayerNum1 As System.Windows.Forms.ToolStrip
    Public WithEvents tsPlayerNum2 As System.Windows.Forms.ToolStrip
#End If
End Class