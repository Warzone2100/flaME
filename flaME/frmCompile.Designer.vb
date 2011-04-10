<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCompile
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.rdoMulti = New System.Windows.Forms.RadioButton()
        Me.rdoCamp = New System.Windows.Forms.RadioButton()
        Me.txtMultiPlayers = New System.Windows.Forms.TextBox()
        Me.btnCompile = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCampTime = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtCampMaxX = New System.Windows.Forms.TextBox()
        Me.txtCampMaxY = New System.Windows.Forms.TextBox()
        Me.txtCampMinY = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtCampMinX = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.txtAuthor = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cmbLicense = New System.Windows.Forms.ComboBox()
        Me.chkNewPlayerFormat = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cmbCampType = New System.Windows.Forms.ComboBox()
        Me.SuspendLayout()
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(84, 15)
        Me.txtName.Margin = New System.Windows.Forms.Padding(4)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(140, 22)
        Me.txtName.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(25, 18)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(21, 95)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 17)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Players:"
        '
        'rdoMulti
        '
        Me.rdoMulti.AutoSize = True
        Me.rdoMulti.Location = New System.Drawing.Point(16, 63)
        Me.rdoMulti.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoMulti.Name = "rdoMulti"
        Me.rdoMulti.Size = New System.Drawing.Size(97, 21)
        Me.rdoMulti.TabIndex = 3
        Me.rdoMulti.TabStop = True
        Me.rdoMulti.Text = "Multiplayer"
        Me.rdoMulti.UseVisualStyleBackColor = True
        '
        'rdoCamp
        '
        Me.rdoCamp.AutoSize = True
        Me.rdoCamp.Location = New System.Drawing.Point(16, 191)
        Me.rdoCamp.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoCamp.Name = "rdoCamp"
        Me.rdoCamp.Size = New System.Drawing.Size(92, 21)
        Me.rdoCamp.TabIndex = 4
        Me.rdoCamp.TabStop = True
        Me.rdoCamp.Text = "Campaign"
        Me.rdoCamp.UseVisualStyleBackColor = True
        '
        'txtMultiPlayers
        '
        Me.txtMultiPlayers.Location = New System.Drawing.Point(84, 91)
        Me.txtMultiPlayers.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMultiPlayers.Name = "txtMultiPlayers"
        Me.txtMultiPlayers.Size = New System.Drawing.Size(68, 22)
        Me.txtMultiPlayers.TabIndex = 5
        '
        'btnCompile
        '
        Me.btnCompile.Enabled = False
        Me.btnCompile.Location = New System.Drawing.Point(255, 366)
        Me.btnCompile.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCompile.Name = "btnCompile"
        Me.btnCompile.Size = New System.Drawing.Size(128, 30)
        Me.btnCompile.TabIndex = 10
        Me.btnCompile.Text = "Compile"
        Me.btnCompile.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(32, 226)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(43, 17)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Time:"
        '
        'txtCampTime
        '
        Me.txtCampTime.Location = New System.Drawing.Point(84, 223)
        Me.txtCampTime.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampTime.Name = "txtCampTime"
        Me.txtCampTime.Size = New System.Drawing.Size(68, 22)
        Me.txtCampTime.TabIndex = 11
        Me.txtCampTime.Text = "2"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(168, 226)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(44, 17)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Type:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(13, 270)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 17)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Scroll Limits:"
        '
        'txtCampMaxX
        '
        Me.txtCampMaxX.Location = New System.Drawing.Point(217, 298)
        Me.txtCampMaxX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMaxX.Name = "txtCampMaxX"
        Me.txtCampMaxX.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMaxX.TabIndex = 15
        Me.txtCampMaxX.Text = "maxX"
        '
        'txtCampMaxY
        '
        Me.txtCampMaxY.Location = New System.Drawing.Point(217, 329)
        Me.txtCampMaxY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMaxY.Name = "txtCampMaxY"
        Me.txtCampMaxY.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMaxY.TabIndex = 18
        Me.txtCampMaxY.Text = "maxY"
        '
        'txtCampMinY
        '
        Me.txtCampMinY.Location = New System.Drawing.Point(136, 329)
        Me.txtCampMinY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMinY.Name = "txtCampMinY"
        Me.txtCampMinY.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMinY.TabIndex = 22
        Me.txtCampMinY.Text = "minY"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(108, 298)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(18, 17)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "x:"
        '
        'txtCampMinX
        '
        Me.txtCampMinX.Location = New System.Drawing.Point(136, 298)
        Me.txtCampMinX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMinX.Name = "txtCampMinX"
        Me.txtCampMinX.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMinX.TabIndex = 20
        Me.txtCampMinX.Text = "minX"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(108, 270)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(67, 17)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Minimum:"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(193, 270)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(70, 17)
        Me.Label12.TabIndex = 25
        Me.Label12.Text = "Maximum:"
        '
        'txtAuthor
        '
        Me.txtAuthor.Location = New System.Drawing.Point(84, 123)
        Me.txtAuthor.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAuthor.Name = "txtAuthor"
        Me.txtAuthor.Size = New System.Drawing.Size(123, 22)
        Me.txtAuthor.TabIndex = 27
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(25, 127)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(54, 17)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Author:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(17, 159)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(61, 17)
        Me.Label13.TabIndex = 28
        Me.Label13.Text = "License:"
        '
        'cmbLicense
        '
        Me.cmbLicense.FormattingEnabled = True
        Me.cmbLicense.Items.AddRange(New Object() {"GPL 2+", "CC BY 3.0 + GPL v2+", "CC BY-SA 3.0 + GPL v2+", "CC0"})
        Me.cmbLicense.Location = New System.Drawing.Point(84, 155)
        Me.cmbLicense.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbLicense.Name = "cmbLicense"
        Me.cmbLicense.Size = New System.Drawing.Size(172, 24)
        Me.cmbLicense.TabIndex = 29
        '
        'chkNewPlayerFormat
        '
        Me.chkNewPlayerFormat.Location = New System.Drawing.Point(161, 90)
        Me.chkNewPlayerFormat.Margin = New System.Windows.Forms.Padding(4)
        Me.chkNewPlayerFormat.Name = "chkNewPlayerFormat"
        Me.chkNewPlayerFormat.Size = New System.Drawing.Size(221, 25)
        Me.chkNewPlayerFormat.TabIndex = 30
        Me.chkNewPlayerFormat.Text = "X Player Support (Beta)"
        Me.chkNewPlayerFormat.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(108, 333)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(19, 17)
        Me.Label7.TabIndex = 31
        Me.Label7.Text = "y:"
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(265, 155)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(132, 41)
        Me.Label8.TabIndex = 32
        Me.Label8.Text = "Select from the list or type another."
        '
        'cmbCampType
        '
        Me.cmbCampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCampType.FormattingEnabled = True
        Me.cmbCampType.Items.AddRange(New Object() {"Initial scenario state", "Scenario scroll area expansion", "Stand alone mission"})
        Me.cmbCampType.Location = New System.Drawing.Point(221, 223)
        Me.cmbCampType.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbCampType.Name = "cmbCampType"
        Me.cmbCampType.Size = New System.Drawing.Size(160, 24)
        Me.cmbCampType.TabIndex = 33
        '
        'frmCompile
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(399, 402)
        Me.Controls.Add(Me.cmbCampType)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.chkNewPlayerFormat)
        Me.Controls.Add(Me.cmbLicense)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtAuthor)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.txtCampMinY)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtCampMinX)
        Me.Controls.Add(Me.txtCampMaxY)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtCampMaxX)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtCampTime)
        Me.Controls.Add(Me.btnCompile)
        Me.Controls.Add(Me.txtMultiPlayers)
        Me.Controls.Add(Me.rdoCamp)
        Me.Controls.Add(Me.rdoMulti)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmCompile"
        Me.Text = "Compile Map"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents rdoMulti As System.Windows.Forms.RadioButton
    Friend WithEvents rdoCamp As System.Windows.Forms.RadioButton
    Friend WithEvents txtMultiPlayers As System.Windows.Forms.TextBox
    Friend WithEvents btnCompile As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtCampTime As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtCampMaxX As System.Windows.Forms.TextBox
    Friend WithEvents txtCampMaxY As System.Windows.Forms.TextBox
    Friend WithEvents txtCampMinY As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtCampMinX As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents txtAuthor As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents cmbLicense As System.Windows.Forms.ComboBox
    Friend WithEvents chkNewPlayerFormat As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbCampType As System.Windows.Forms.ComboBox
End Class
