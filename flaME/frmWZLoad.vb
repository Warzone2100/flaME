Public Class frmWZLoad
#If OS <> "Windows" Then
    Inherits Form
#End If

    Class clsOutput
        Public Result As Integer
    End Class

    Public lstMap_MapName() As String

    Public Output As clsOutput

    Sub New(ByRef MapNames() As String, ByVal NewOutput As clsOutput)
        InitializeComponent()

        Output = NewOutput
        Output.Result = -1

        Dim A As Integer

        lstMap.Items.Clear()
        lstMap_MapName = MapNames
        For A = 0 To MapNames.GetUpperBound(0)
            lstMap.Items.Add(MapNames(A))
        Next
    End Sub

    Private Sub lstMaps_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstMap.DoubleClick

        If lstMap.SelectedIndex >= 0 Then
            Output.Result = lstMap.SelectedIndex
            Close()
        End If
    End Sub

#If OS <> "Windows" Then
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWZLoad))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstMap = New System.Windows.Forms.ListBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(4, 0)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(611, 30)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select map to load:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lstMap
        '
        Me.lstMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstMap.FormattingEnabled = True
        Me.lstMap.ItemHeight = 16
        Me.lstMap.Location = New System.Drawing.Point(4, 34)
        Me.lstMap.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.lstMap.Name = "lstMap"
        Me.lstMap.Size = New System.Drawing.Size(611, 277)
        Me.lstMap.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lstMap, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(619, 315)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'frmWZLoad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(619, 315)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "frmWZLoad"
        Me.Text = "frmWZLoad"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lstMap As System.Windows.Forms.ListBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
#End If
End Class