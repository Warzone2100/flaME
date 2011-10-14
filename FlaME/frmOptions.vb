Public Class frmOptions
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Private DisplayFont As Font

    Private MinimapCliffColour As clsRGBA_sng
    Private clrMinimapCliffs As ctrlColour
    Private MinimapSelectedObjectColour As clsRGBA_sng
    Private clrMinimapSelectedObjects As ctrlColour

    Public Sub New()
        InitializeComponent()

        Icon = ProgramIcon

        txtAutosaveChanges.Text = InvariantToString_sng(Settings.AutoSave_MinChanges)
        txtAutosaveInterval.Text = InvariantToString_sng(Settings.AutoSave_MinInterval_s)
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
        txtUndoSteps.Text = InvariantToString_sng(Settings.Undo_Limit)
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        Dim NewSettings As New clsSettings
        Dim dblTemp As Double

        If InvariantParse_dbl(txtAutosaveChanges.Text, dblTemp) Then
            NewSettings.AutoSave_MinChanges = CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#))
        End If
        If InvariantParse_dbl(txtAutosaveInterval.Text, dblTemp) Then
            NewSettings.AutoSave_MinInterval_s = CUInt(Clamp_dbl(dblTemp, 1.0#, CDbl(UInteger.MaxValue) - 1.0#))
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
        If InvariantParse_dbl(txtUndoSteps.Text, dblTemp) Then
            NewSettings.Undo_Limit = CUInt(Clamp_dbl(dblTemp, 0.0#, 8192.0#))
        End If

        UpdateSettings(NewSettings)

        Dim Map As clsMap = MainMap
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

        Load_Autosave_Prompt()
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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.txtFOV = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.cbxPointerDirect = New System.Windows.Forms.CheckBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.txtMinimapSize = New System.Windows.Forms.TextBox()
        Me.cbxMinimapTeamColourFeatures = New System.Windows.Forms.CheckBox()
        Me.cbxMinimapObjectColours = New System.Windows.Forms.CheckBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblFont = New System.Windows.Forms.Label()
        Me.btnFont = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtAutosaveInterval = New System.Windows.Forms.TextBox()
        Me.txtAutosaveChanges = New System.Windows.Forms.TextBox()
        Me.btnAutosaveOpen = New System.Windows.Forms.Button()
        Me.cbxAutosaveCompression = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbxAutosaveEnabled = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtUndoSteps = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.btnDirectories = New System.Windows.Forms.Button()
        Me.cbxAskDirectories = New System.Windows.Forms.CheckBox()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.pnlMinimapCliffColour = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.pnlMinimapSelectedObjectColour = New System.Windows.Forms.Panel()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(637, 329)
        Me.TabControl1.TabIndex = 35
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox7)
        Me.TabPage1.Controls.Add(Me.GroupBox6)
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.GroupBox2)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Controls.Add(Me.GroupBox3)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(629, 300)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "General"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.txtFOV)
        Me.GroupBox7.Controls.Add(Me.Label4)
        Me.GroupBox7.Location = New System.Drawing.Point(316, 232)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(304, 50)
        Me.GroupBox7.TabIndex = 44
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Field Of View"
        Me.GroupBox7.UseCompatibleTextRendering = True
        '
        'txtFOV
        '
        Me.txtFOV.Location = New System.Drawing.Point(159, 15)
        Me.txtFOV.Margin = New System.Windows.Forms.Padding(4)
        Me.txtFOV.Name = "txtFOV"
        Me.txtFOV.Size = New System.Drawing.Size(138, 22)
        Me.txtFOV.TabIndex = 25
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 18)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(105, 20)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Default Multiplier"
        Me.Label4.UseCompatibleTextRendering = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.cbxPointerDirect)
        Me.GroupBox6.Location = New System.Drawing.Point(316, 176)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(304, 50)
        Me.GroupBox6.TabIndex = 43
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Pointer"
        Me.GroupBox6.UseCompatibleTextRendering = True
        '
        'cbxPointerDirect
        '
        Me.cbxPointerDirect.AutoSize = True
        Me.cbxPointerDirect.Location = New System.Drawing.Point(7, 22)
        Me.cbxPointerDirect.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxPointerDirect.Name = "cbxPointerDirect"
        Me.cbxPointerDirect.Size = New System.Drawing.Size(62, 21)
        Me.cbxPointerDirect.TabIndex = 40
        Me.cbxPointerDirect.Text = "Direct"
        Me.cbxPointerDirect.UseCompatibleTextRendering = True
        Me.cbxPointerDirect.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label6)
        Me.GroupBox5.Controls.Add(Me.pnlMinimapSelectedObjectColour)
        Me.GroupBox5.Controls.Add(Me.Label5)
        Me.GroupBox5.Controls.Add(Me.pnlMinimapCliffColour)
        Me.GroupBox5.Controls.Add(Me.txtMinimapSize)
        Me.GroupBox5.Controls.Add(Me.cbxMinimapTeamColourFeatures)
        Me.GroupBox5.Controls.Add(Me.cbxMinimapObjectColours)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Location = New System.Drawing.Point(316, 7)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(304, 163)
        Me.GroupBox5.TabIndex = 42
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Minimap"
        Me.GroupBox5.UseCompatibleTextRendering = True
        '
        'txtMinimapSize
        '
        Me.txtMinimapSize.Location = New System.Drawing.Point(159, 15)
        Me.txtMinimapSize.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMinimapSize.Name = "txtMinimapSize"
        Me.txtMinimapSize.Size = New System.Drawing.Size(61, 22)
        Me.txtMinimapSize.TabIndex = 25
        '
        'cbxMinimapTeamColourFeatures
        '
        Me.cbxMinimapTeamColourFeatures.AutoSize = True
        Me.cbxMinimapTeamColourFeatures.Location = New System.Drawing.Point(147, 42)
        Me.cbxMinimapTeamColourFeatures.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMinimapTeamColourFeatures.Name = "cbxMinimapTeamColourFeatures"
        Me.cbxMinimapTeamColourFeatures.Size = New System.Drawing.Size(120, 21)
        Me.cbxMinimapTeamColourFeatures.TabIndex = 41
        Me.cbxMinimapTeamColourFeatures.Text = "Except features"
        Me.cbxMinimapTeamColourFeatures.UseCompatibleTextRendering = True
        Me.cbxMinimapTeamColourFeatures.UseVisualStyleBackColor = True
        '
        'cbxMinimapObjectColours
        '
        Me.cbxMinimapObjectColours.AutoSize = True
        Me.cbxMinimapObjectColours.Location = New System.Drawing.Point(8, 42)
        Me.cbxMinimapObjectColours.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxMinimapObjectColours.Name = "cbxMinimapObjectColours"
        Me.cbxMinimapObjectColours.Size = New System.Drawing.Size(131, 21)
        Me.cbxMinimapObjectColours.TabIndex = 40
        Me.cbxMinimapObjectColours.Text = "Use team colours"
        Me.cbxMinimapObjectColours.UseCompatibleTextRendering = True
        Me.cbxMinimapObjectColours.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 18)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(31, 20)
        Me.Label3.TabIndex = 26
        Me.Label3.Text = "Size"
        Me.Label3.UseCompatibleTextRendering = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblFont)
        Me.GroupBox4.Controls.Add(Me.btnFont)
        Me.GroupBox4.Location = New System.Drawing.Point(6, 231)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(304, 62)
        Me.GroupBox4.TabIndex = 41
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Display Font"
        Me.GroupBox4.UseCompatibleTextRendering = True
        '
        'lblFont
        '
        Me.lblFont.Location = New System.Drawing.Point(8, 27)
        Me.lblFont.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFont.Name = "lblFont"
        Me.lblFont.Size = New System.Drawing.Size(182, 29)
        Me.lblFont.TabIndex = 39
        Me.lblFont.Text = "Current font"
        Me.lblFont.UseCompatibleTextRendering = True
        '
        'btnFont
        '
        Me.btnFont.Location = New System.Drawing.Point(208, 21)
        Me.btnFont.Name = "btnFont"
        Me.btnFont.Size = New System.Drawing.Size(89, 29)
        Me.btnFont.TabIndex = 38
        Me.btnFont.Text = "Select"
        Me.btnFont.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtAutosaveInterval)
        Me.GroupBox2.Controls.Add(Me.txtAutosaveChanges)
        Me.GroupBox2.Controls.Add(Me.btnAutosaveOpen)
        Me.GroupBox2.Controls.Add(Me.cbxAutosaveCompression)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.cbxAutosaveEnabled)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(6, 63)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(304, 107)
        Me.GroupBox2.TabIndex = 37
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Autosave"
        Me.GroupBox2.UseCompatibleTextRendering = True
        '
        'txtAutosaveInterval
        '
        Me.txtAutosaveInterval.Location = New System.Drawing.Point(140, 74)
        Me.txtAutosaveInterval.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAutosaveInterval.Name = "txtAutosaveInterval"
        Me.txtAutosaveInterval.Size = New System.Drawing.Size(61, 22)
        Me.txtAutosaveInterval.TabIndex = 25
        '
        'txtAutosaveChanges
        '
        Me.txtAutosaveChanges.Location = New System.Drawing.Point(140, 51)
        Me.txtAutosaveChanges.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAutosaveChanges.Name = "txtAutosaveChanges"
        Me.txtAutosaveChanges.Size = New System.Drawing.Size(61, 22)
        Me.txtAutosaveChanges.TabIndex = 22
        '
        'btnAutosaveOpen
        '
        Me.btnAutosaveOpen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnAutosaveOpen.Location = New System.Drawing.Point(208, 71)
        Me.btnAutosaveOpen.Name = "btnAutosaveOpen"
        Me.btnAutosaveOpen.Size = New System.Drawing.Size(89, 29)
        Me.btnAutosaveOpen.TabIndex = 39
        Me.btnAutosaveOpen.Text = "Open Map"
        Me.btnAutosaveOpen.UseVisualStyleBackColor = True
        '
        'cbxAutosaveCompression
        '
        Me.cbxAutosaveCompression.AutoSize = True
        Me.cbxAutosaveCompression.Location = New System.Drawing.Point(140, 19)
        Me.cbxAutosaveCompression.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAutosaveCompression.Name = "cbxAutosaveCompression"
        Me.cbxAutosaveCompression.Size = New System.Drawing.Size(130, 21)
        Me.cbxAutosaveCompression.TabIndex = 27
        Me.cbxAutosaveCompression.Text = "Use compression"
        Me.cbxAutosaveCompression.UseCompatibleTextRendering = True
        Me.cbxAutosaveCompression.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(125, 20)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "Number of changes:"
        Me.Label2.UseCompatibleTextRendering = True
        '
        'cbxAutosaveEnabled
        '
        Me.cbxAutosaveEnabled.AutoSize = True
        Me.cbxAutosaveEnabled.Location = New System.Drawing.Point(7, 22)
        Me.cbxAutosaveEnabled.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAutosaveEnabled.Name = "cbxAutosaveEnabled"
        Me.cbxAutosaveEnabled.Size = New System.Drawing.Size(76, 21)
        Me.cbxAutosaveEnabled.TabIndex = 3
        Me.cbxAutosaveEnabled.Text = "Enabled"
        Me.cbxAutosaveEnabled.UseCompatibleTextRendering = True
        Me.cbxAutosaveEnabled.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 74)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 20)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "Time interval:"
        Me.Label1.UseCompatibleTextRendering = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtUndoSteps)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(304, 51)
        Me.GroupBox1.TabIndex = 36
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Undo"
        Me.GroupBox1.UseCompatibleTextRendering = True
        '
        'txtUndoSteps
        '
        Me.txtUndoSteps.Location = New System.Drawing.Point(158, 15)
        Me.txtUndoSteps.Margin = New System.Windows.Forms.Padding(4)
        Me.txtUndoSteps.Name = "txtUndoSteps"
        Me.txtUndoSteps.Size = New System.Drawing.Size(61, 22)
        Me.txtUndoSteps.TabIndex = 22
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(7, 18)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(143, 20)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Maximum stored steps:"
        Me.Label11.UseCompatibleTextRendering = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.btnDirectories)
        Me.GroupBox3.Controls.Add(Me.cbxAskDirectories)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 176)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(304, 52)
        Me.GroupBox3.TabIndex = 40
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Warzone Data"
        Me.GroupBox3.UseCompatibleTextRendering = True
        '
        'btnDirectories
        '
        Me.btnDirectories.Location = New System.Drawing.Point(208, 17)
        Me.btnDirectories.Name = "btnDirectories"
        Me.btnDirectories.Size = New System.Drawing.Size(89, 29)
        Me.btnDirectories.TabIndex = 40
        Me.btnDirectories.Text = "Modify"
        Me.btnDirectories.UseVisualStyleBackColor = True
        '
        'cbxAskDirectories
        '
        Me.cbxAskDirectories.AutoSize = True
        Me.cbxAskDirectories.Location = New System.Drawing.Point(8, 22)
        Me.cbxAskDirectories.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxAskDirectories.Name = "cbxAskDirectories"
        Me.cbxAskDirectories.Size = New System.Drawing.Size(134, 21)
        Me.cbxAskDirectories.TabIndex = 39
        Me.cbxAskDirectories.Text = "Ask for directories"
        Me.cbxAskDirectories.UseCompatibleTextRendering = True
        Me.cbxAskDirectories.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(549, 347)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 29)
        Me.btnCancel.TabIndex = 39
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(443, 347)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 29)
        Me.btnSave.TabIndex = 40
        Me.btnSave.Text = "Accept"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'pnlMinimapCliffColour
        '
        Me.pnlMinimapCliffColour.Location = New System.Drawing.Point(132, 67)
        Me.pnlMinimapCliffColour.Name = "pnlMinimapCliffColour"
        Me.pnlMinimapCliffColour.Size = New System.Drawing.Size(164, 29)
        Me.pnlMinimapCliffColour.TabIndex = 42
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 67)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 20)
        Me.Label5.TabIndex = 43
        Me.Label5.Text = "Cliff Colour"
        Me.Label5.UseCompatibleTextRendering = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(8, 100)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(99, 20)
        Me.Label6.TabIndex = 45
        Me.Label6.Text = "Object Highlight"
        Me.Label6.UseCompatibleTextRendering = True
        '
        'pnlMinimapSelectedObjectColour
        '
        Me.pnlMinimapSelectedObjectColour.Location = New System.Drawing.Point(132, 100)
        Me.pnlMinimapSelectedObjectColour.Name = "pnlMinimapSelectedObjectColour"
        Me.pnlMinimapSelectedObjectColour.Size = New System.Drawing.Size(164, 29)
        Me.pnlMinimapSelectedObjectColour.TabIndex = 44
        '
        'frmOptions
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(659, 382)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOptions"
        Me.Text = "Options"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents TabControl1 As System.Windows.Forms.TabControl
    Public WithEvents TabPage1 As System.Windows.Forms.TabPage
    Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Public WithEvents Label11 As System.Windows.Forms.Label
    Public WithEvents txtUndoSteps As System.Windows.Forms.TextBox
    Public WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Public WithEvents cbxAutosaveCompression As System.Windows.Forms.CheckBox
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents txtAutosaveInterval As System.Windows.Forms.TextBox
    Public WithEvents cbxAutosaveEnabled As System.Windows.Forms.CheckBox
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents txtAutosaveChanges As System.Windows.Forms.TextBox
    Public WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Public WithEvents lblFont As System.Windows.Forms.Label
    Public WithEvents btnFont As System.Windows.Forms.Button
    Public WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Public WithEvents cbxAskDirectories As System.Windows.Forms.CheckBox
    Public WithEvents btnCancel As System.Windows.Forms.Button
    Public WithEvents btnSave As System.Windows.Forms.Button
    Public WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Public WithEvents cbxMinimapObjectColours As System.Windows.Forms.CheckBox
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents txtMinimapSize As System.Windows.Forms.TextBox
    Public WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents txtFOV As System.Windows.Forms.TextBox
    Public WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Public WithEvents cbxPointerDirect As System.Windows.Forms.CheckBox
    Public WithEvents btnAutosaveOpen As System.Windows.Forms.Button
    Public WithEvents btnDirectories As System.Windows.Forms.Button
    Public WithEvents cbxMinimapTeamColourFeatures As System.Windows.Forms.CheckBox
    Friend WithEvents pnlMinimapCliffColour As System.Windows.Forms.Panel
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents pnlMinimapSelectedObjectColour As System.Windows.Forms.Panel
#End If
End Class