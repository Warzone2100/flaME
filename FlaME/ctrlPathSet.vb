Public Class ctrlPathSet
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public Property SelectedNum As Integer
        Get
            Return lstPaths.SelectedIndex
        End Get
        Set(value As Integer)
            lstPaths.SelectedIndex = value
        End Set
    End Property

    Public ReadOnly Property SelectedPath As String
        Get
            If lstPaths.SelectedIndex < 0 Then
                Return Nothing
            Else
                Return CStr(lstPaths.Items.Item(lstPaths.SelectedIndex))
            End If
        End Get
    End Property

    Public ReadOnly Property GetPaths As String()
        Get
            Dim Paths(lstPaths.Items.Count - 1) As String
            Dim A As Integer
            For A = 0 To lstPaths.Items.Count - 1
                Paths(A) = CStr(lstPaths.Items.Item(A))
            Next
            Return Paths
        End Get
    End Property

    Public Sub New(ByVal Title As String)
        InitializeComponent()

        gbxTitle.Text = Title
    End Sub

    Public Sub SetPaths(ByVal NewPaths() As String)
        Dim A As Integer

        lstPaths.Items.Clear()
        For A = 0 To NewPaths.GetUpperBound(0)
            lstPaths.Items.Add(NewPaths(A))
        Next
    End Sub

    Private Sub btnAdd_Click(sender As System.Object, e As System.EventArgs) Handles btnAdd.Click
        Dim DirSelect As New Windows.Forms.FolderBrowserDialog

        If DirSelect.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        lstPaths.Items.Add(CStr(DirSelect.SelectedPath))
        lstPaths.SelectedIndex = lstPaths.Items.Count - 1
    End Sub

    Private Sub btnRemove_Click(sender As System.Object, e As System.EventArgs) Handles btnRemove.Click

        If lstPaths.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstPaths.Items.RemoveAt(lstPaths.SelectedIndex)
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.gbxTitle = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lstPaths = New System.Windows.Forms.ListBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.gbxTitle.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbxTitle
        '
        Me.gbxTitle.Controls.Add(Me.TableLayoutPanel1)
        Me.gbxTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbxTitle.Location = New System.Drawing.Point(0, 0)
        Me.gbxTitle.Margin = New System.Windows.Forms.Padding(0)
        Me.gbxTitle.Name = "gbxTitle"
        Me.gbxTitle.Padding = New System.Windows.Forms.Padding(8)
        Me.gbxTitle.Size = New System.Drawing.Size(486, 180)
        Me.gbxTitle.TabIndex = 0
        Me.gbxTitle.TabStop = False
        Me.gbxTitle.Text = "Path Set Title"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lstPaths, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 1, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(8, 23)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(470, 149)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'lstPaths
        '
        Me.lstPaths.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstPaths.FormattingEnabled = True
        Me.lstPaths.ItemHeight = 16
        Me.lstPaths.Location = New System.Drawing.Point(3, 3)
        Me.lstPaths.Name = "lstPaths"
        Me.lstPaths.Size = New System.Drawing.Size(368, 143)
        Me.lstPaths.TabIndex = 2
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnRemove, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnAdd, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(374, 0)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 3
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(96, 149)
        Me.TableLayoutPanel2.TabIndex = 3
        '
        'btnRemove
        '
        Me.btnRemove.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnRemove.Location = New System.Drawing.Point(3, 43)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(90, 34)
        Me.btnRemove.TabIndex = 4
        Me.btnRemove.Text = "Remove"
        Me.btnRemove.UseCompatibleTextRendering = True
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnAdd.Location = New System.Drawing.Point(3, 3)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(90, 34)
        Me.btnAdd.TabIndex = 3
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseCompatibleTextRendering = True
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'ctrlPathSet
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.gbxTitle)
        Me.Name = "ctrlPathSet"
        Me.Size = New System.Drawing.Size(486, 180)
        Me.gbxTitle.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents gbxTitle As System.Windows.Forms.GroupBox
    Public WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents btnRemove As System.Windows.Forms.Button
    Public WithEvents btnAdd As System.Windows.Forms.Button
    Private WithEvents lstPaths As System.Windows.Forms.ListBox
#End If
End Class