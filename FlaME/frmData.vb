Public Class frmData

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
End Class