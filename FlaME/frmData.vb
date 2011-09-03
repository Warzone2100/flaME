Public Class frmData
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Public ObjectDataPathSet As New ctrlPathSet("Object Data Directories")
    Public TilesetsPathSet As New ctrlPathSet("Tilesets Directories")

    Private IsDialog As Boolean = True

    Public Sub New()
        InitializeComponent()

        TilesetsPathSet.Dock = DockStyle.Fill
        ObjectDataPathSet.Dock = DockStyle.Fill

        TableLayoutPanel1.Controls.Add(TilesetsPathSet, 0, 0)
        TableLayoutPanel1.Controls.Add(ObjectDataPathSet, 0, 1)
    End Sub

    Public Sub HideButtons()

        btnQuit.Visible = False
        btnContinue.Visible = False
        IsDialog = False
    End Sub

    Private Sub btnQuit_Click(sender As System.Object, e As System.EventArgs) Handles btnQuit.Click

        DialogResult = Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnContinue_Click(sender As System.Object, e As System.EventArgs) Handles btnContinue.Click

        DialogResult = Windows.Forms.DialogResult.OK
    End Sub

    Private Sub frmData_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If Not IsDialog Then
            e.Cancel = True
            Hide()
        End If
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.btnContinue = New System.Windows.Forms.Button()
        Me.btnQuit = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnContinue
        '
        Me.btnContinue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnContinue.Location = New System.Drawing.Point(350, 6)
        Me.btnContinue.Margin = New System.Windows.Forms.Padding(6)
        Me.btnContinue.Name = "btnContinue"
        Me.btnContinue.Size = New System.Drawing.Size(148, 36)
        Me.btnContinue.TabIndex = 0
        Me.btnContinue.Text = "Continue"
        Me.btnContinue.UseCompatibleTextRendering = True
        Me.btnContinue.UseVisualStyleBackColor = True
        '
        'btnQuit
        '
        Me.btnQuit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnQuit.Location = New System.Drawing.Point(6, 6)
        Me.btnQuit.Margin = New System.Windows.Forms.Padding(6)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(148, 36)
        Me.btnQuit.TabIndex = 1
        Me.btnQuit.Text = "Quit"
        Me.btnQuit.UseCompatibleTextRendering = True
        Me.btnQuit.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(504, 422)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnQuit, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnContinue, 2, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 374)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(504, 48)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'frmData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(504, 422)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "frmData"
        Me.Text = "Data Directories"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents btnContinue As System.Windows.Forms.Button
    Public WithEvents btnQuit As System.Windows.Forms.Button
    Public WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
#End If
End Class