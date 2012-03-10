Public Class frmOptions

    Private DisplayFont As Font

    Private MinimapCliffColour As clsRGBA_sng
    Private clrMinimapCliffs As ctrlColour
    Private MinimapSelectedObjectColour As clsRGBA_sng
    Private clrMinimapSelectedObjects As ctrlColour

    Public Sub New()
        InitializeComponent()

        Icon = ProgramIcon

        txtAutosaveChanges.Text = InvariantToString_sng(Settings.AutoSaveMinChanges)
        txtAutosaveInterval.Text = InvariantToString_sng(Settings.AutoSaveMinInterval_s)
        cbxAutosaveCompression.Checked = Settings.AutoSaveCompress
        cbxAutosaveEnabled.Checked = Settings.AutoSaveEnabled
        cbxAskDirectories.Checked = Settings.DirectoriesPrompt
        cbxPointerDirect.Checked = Settings.DirectPointer
        DisplayFont = Settings.DisplayFont
        UpdateDisplayFontLabel()
        txtFOV.Text = InvariantToString_dbl(Settings.FOVDefault)

        MinimapCliffColour = New clsRGBA_sng(Settings.MinimapCliffColour)
        clrMinimapCliffs = New ctrlColour(MinimapCliffColour)
        pnlMinimapCliffColour.Controls.Add(clrMinimapCliffs)

        MinimapSelectedObjectColour = New clsRGBA_sng(Settings.MinimapSelectedObjectsColour)
        clrMinimapSelectedObjects = New ctrlColour(MinimapSelectedObjectColour)
        pnlMinimapSelectedObjectColour.Controls.Add(clrMinimapSelectedObjects)

        txtMinimapSize.Text = InvariantToString_int(Settings.MinimapSize)
        cbxMinimapObjectColours.Checked = Settings.MinimapTeamColours
        cbxMinimapTeamColourFeatures.Checked = Settings.MinimapTeamColoursExceptFeatures
        cbxMipmaps.Checked = Settings.Mipmaps
        cbxMipmapsHardware.Checked = Settings.MipmapsHardware
        txtUndoSteps.Text = InvariantToString_sng(Settings.UndoLimit)
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        Dim NewSettings As New clsSettings
        Dim dblTemp As Double

        If InvariantParse_dbl(txtAutosaveChanges.Text, dblTemp) Then
            NewSettings.AutoSaveMinChanges = CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#))
        End If
        If InvariantParse_dbl(txtAutosaveInterval.Text, dblTemp) Then
            NewSettings.AutoSaveMinInterval_s = CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#))
        End If
        NewSettings.AutoSaveCompress = cbxAutosaveCompression.Checked
        NewSettings.AutoSaveEnabled = cbxAutosaveEnabled.Checked
        NewSettings.DirectoriesPrompt = cbxAskDirectories.Checked
        NewSettings.DirectPointer = cbxPointerDirect.Checked
        NewSettings.DisplayFont = DisplayFont
        If InvariantParse_dbl(txtFOV.Text, dblTemp) Then
            NewSettings.FOVDefault = Clamp_dbl(dblTemp, 0.00005#, 0.005#)
        End If
        NewSettings.MinimapCliffColour = MinimapCliffColour
        NewSettings.MinimapSelectedObjectsColour = MinimapSelectedObjectColour
        If InvariantParse_dbl(txtMinimapSize.Text, dblTemp) Then
            NewSettings.MinimapSize = CInt(Clamp_dbl(dblTemp, 0.0#, 512.0#))
        End If
        NewSettings.MinimapTeamColours = cbxMinimapObjectColours.Checked
        NewSettings.MinimapTeamColoursExceptFeatures = cbxMinimapTeamColourFeatures.Checked
        NewSettings.Mipmaps = cbxMipmaps.Checked
        NewSettings.MipmapsHardware = cbxMipmapsHardware.Checked
        If InvariantParse_dbl(txtUndoSteps.Text, dblTemp) Then
            NewSettings.UndoLimit = CUInt(Clamp_dbl(dblTemp, 0.0#, 8192.0#))
        End If

        UpdateSettings(NewSettings)

        Dim Map As clsMap = frmMainInstance.MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater()
        End If
        frmMainInstance.View_DrawViewLater()

        AllowClose = True
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click

        AllowClose = True
        Close()
    End Sub

    Private Sub btnFont_Click(sender As System.Object, e As System.EventArgs) Handles btnFont.Click
        Dim FontDialog As New Windows.Forms.FontDialog

		Dim Result As DialogResult
		Try 'mono 267 has crashed here
			FontDialog.Font = DisplayFont
			FontDialog.FontMustExist = True
            Result = FontDialog.ShowDialog(Me)
        Catch ex As Exception
            Result = DialogResult.Cancel
        End Try
        If Result = Windows.Forms.DialogResult.OK Then
            DisplayFont = FontDialog.Font
            UpdateDisplayFontLabel()
        End If
    End Sub

    Private Sub btnAutosaveOpen_Click(sender As System.Object, e As System.EventArgs) Handles btnAutosaveOpen.Click

        frmMainInstance.Load_Autosave_Prompt()
    End Sub

    Private Sub btnDirectories_Click(sender As System.Object, e As System.EventArgs) Handles btnDirectories.Click

        frmDataInstance.Show()
    End Sub

    Private AllowClose As Boolean = False

    Private Sub frmOptions_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If AllowClose Then
            frmOptionsInstance = Nothing
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub UpdateDisplayFontLabel()

        lblFont.Text = DisplayFont.FontFamily.Name & " " & DisplayFont.SizeInPoints & " "
        If DisplayFont.Bold Then
            lblFont.Text &= "B"
        End If
        If DisplayFont.Italic Then
            lblFont.Text &= "I"
        End If
    End Sub
End Class