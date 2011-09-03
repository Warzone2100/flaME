Public Class frmWZLoad
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.lstMap = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'lstMap
        '
        Me.lstMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstMap.FormattingEnabled = True
        Me.lstMap.ItemHeight = 16
        Me.lstMap.Location = New System.Drawing.Point(0, 0)
        Me.lstMap.Margin = New System.Windows.Forms.Padding(4)
        Me.lstMap.Name = "lstMap"
        Me.lstMap.Size = New System.Drawing.Size(619, 315)
        Me.lstMap.TabIndex = 1
        '
        'frmWZLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(619, 315)
        Me.Controls.Add(Me.lstMap)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmWZLoad"
        Me.Text = "frmWZLoad"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents lstMap As System.Windows.Forms.ListBox
#End If
End Class