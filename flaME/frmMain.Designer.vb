<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrKey = New System.Windows.Forms.Timer(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TabControl = New System.Windows.Forms.TabControl()
        Me.tpTextures = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.chkTextureOrientationRandomize = New System.Windows.Forms.CheckBox()
        Me.btnTextureFlipX = New System.Windows.Forms.Button()
        Me.btnTextureClockwise = New System.Windows.Forms.Button()
        Me.btnTextureAnticlockwise = New System.Windows.Forms.Button()
        Me.chkSetTextureOrientation = New System.Windows.Forms.CheckBox()
        Me.chkSetTexture = New System.Windows.Forms.CheckBox()
        Me.tabTextureBrushShape = New System.Windows.Forms.TabControl()
        Me.TabPage37 = New System.Windows.Forms.TabPage()
        Me.TabPage38 = New System.Windows.Forms.TabPage()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.cmbTileset = New System.Windows.Forms.ComboBox()
        Me.nudTextureBrushRadius = New System.Windows.Forms.NumericUpDown()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.chkTileNumbers = New System.Windows.Forms.CheckBox()
        Me.chkTileTypes = New System.Windows.Forms.CheckBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.cmbTileType = New System.Windows.Forms.ComboBox()
        Me.tpAutoTexture = New System.Windows.Forms.TabPage()
        Me.tabAutoTextureBrushShape = New System.Windows.Forms.TabControl()
        Me.TabPage35 = New System.Windows.Forms.TabPage()
        Me.TabPage36 = New System.Windows.Forms.TabPage()
        Me.chkAutoTexSetHeight = New System.Windows.Forms.CheckBox()
        Me.chkCliffTris = New System.Windows.Forms.CheckBox()
        Me.btnMapTexturer = New System.Windows.Forms.Button()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.rdoAutoRoadLine = New System.Windows.Forms.RadioButton()
        Me.btnAutoTextureRemove = New System.Windows.Forms.Button()
        Me.btnAutoRoadRemove = New System.Windows.Forms.Button()
        Me.rdoAutoRoadPlace = New System.Windows.Forms.RadioButton()
        Me.lstAutoRoad = New System.Windows.Forms.ListBox()
        Me.rdoAutoTexturePlace = New System.Windows.Forms.RadioButton()
        Me.rdoAutoTextureFill = New System.Windows.Forms.RadioButton()
        Me.nudAutoCliffBrushRadius = New System.Windows.Forms.NumericUpDown()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.rdoAutoCliffBrush = New System.Windows.Forms.RadioButton()
        Me.rdoAutoCliffRemove = New System.Windows.Forms.RadioButton()
        Me.nudAutoTextureRadius = New System.Windows.Forms.NumericUpDown()
        Me.txtAutoCliffSlope = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstAutoTexture = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tpHeight = New System.Windows.Forms.TabPage()
        Me.btnHeightsMultiplySelection = New System.Windows.Forms.Button()
        Me.btnHeightOffsetSelection = New System.Windows.Forms.Button()
        Me.tabHeightBrushShape = New System.Windows.Forms.TabControl()
        Me.TabPage33 = New System.Windows.Forms.TabPage()
        Me.TabPage34 = New System.Windows.Forms.TabPage()
        Me.tabHeightSetR = New System.Windows.Forms.TabControl()
        Me.TabPage25 = New System.Windows.Forms.TabPage()
        Me.TabPage26 = New System.Windows.Forms.TabPage()
        Me.TabPage27 = New System.Windows.Forms.TabPage()
        Me.TabPage28 = New System.Windows.Forms.TabPage()
        Me.TabPage29 = New System.Windows.Forms.TabPage()
        Me.TabPage30 = New System.Windows.Forms.TabPage()
        Me.TabPage31 = New System.Windows.Forms.TabPage()
        Me.TabPage32 = New System.Windows.Forms.TabPage()
        Me.tabHeightSetL = New System.Windows.Forms.TabControl()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.TabPage11 = New System.Windows.Forms.TabPage()
        Me.TabPage12 = New System.Windows.Forms.TabPage()
        Me.TabPage17 = New System.Windows.Forms.TabPage()
        Me.TabPage18 = New System.Windows.Forms.TabPage()
        Me.TabPage19 = New System.Windows.Forms.TabPage()
        Me.TabPage20 = New System.Windows.Forms.TabPage()
        Me.txtHeightSetR = New System.Windows.Forms.TextBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtHeightOffset = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtHeightMultiply = New System.Windows.Forms.TextBox()
        Me.txtHeightChangeRate = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.rdoHeightChange = New System.Windows.Forms.RadioButton()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.nudHeightBrushRadius = New System.Windows.Forms.NumericUpDown()
        Me.btnMultiplier = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtMultiplier = New System.Windows.Forms.TextBox()
        Me.txtSmoothRadius = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtSmoothRate = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.rdoHeightSmooth = New System.Windows.Forms.RadioButton()
        Me.rdoHeightSet = New System.Windows.Forms.RadioButton()
        Me.txtHeightSetL = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tpAutoHeight = New System.Windows.Forms.TabPage()
        Me.btnGenerator = New System.Windows.Forms.Button()
        Me.btnWaterTri = New System.Windows.Forms.Button()
        Me.btnReinterpretTerrain = New System.Windows.Forms.Button()
        Me.btnAutoTri = New System.Windows.Forms.Button()
        Me.tpResize = New System.Windows.Forms.TabPage()
        Me.btnSelResize = New System.Windows.Forms.Button()
        Me.btnResize = New System.Windows.Forms.Button()
        Me.txtOffsetY = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtOffsetX = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtSizeY = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtSizeX = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.tpObjects = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.lstDroids = New System.Windows.Forms.ListBox()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.lstStructures = New System.Windows.Forms.ListBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.txtPlayerNum = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.lstFeatures = New System.Windows.Forms.ListBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.tpObject = New System.Windows.Forms.TabPage()
        Me.txtObjectPriority = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.txtObjectID = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.txtObjectPlayer = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.txtObjectRotation = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.lblObjectType = New System.Windows.Forms.Label()
        Me.txtObjectName = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.txtObjesdss = New System.Windows.Forms.Label()
        Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.tsTools = New System.Windows.Forms.ToolStrip()
        Me.tsbGateways = New System.Windows.Forms.ToolStripButton()
        Me.tsbDrawAutotexture = New System.Windows.Forms.ToolStripButton()
        Me.tsbDrawTileOrientation = New System.Windows.Forms.ToolStripButton()
        Me.tsFile = New System.Windows.Forms.ToolStrip()
        Me.tsbSave = New System.Windows.Forms.ToolStripButton()
        Me.tsSelection = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.tsbSelection = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionCopy = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionPasteOptions = New System.Windows.Forms.ToolStripDropDownButton()
        Me.menuRotateUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.menuSelPasteHeights = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuSelPasteTextures = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuSelPasteUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuSelPasteGateways = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuSelPasteDeleteUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuSelPasteDeleteGateways = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsbSelectionPaste = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionRotateAnticlockwise = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionRotateClockwise = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionFlipX = New System.Windows.Forms.ToolStripButton()
        Me.tsbSelectionObjects = New System.Windows.Forms.ToolStripButton()
        Me.tsMinimap = New System.Windows.Forms.ToolStrip()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.menuMiniShowTex = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuMiniShowHeight = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuMiniShowUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuMiniShowGateways = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuMinimapSize = New System.Windows.Forms.ToolStripComboBox()
        Me.pnlView = New System.Windows.Forms.Panel()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.menuMain = New System.Windows.Forms.MenuStrip()
        Me.menuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.NewMapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MapFMEToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.MapLNDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.menuExportMapTileTypes = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MinimapBMPToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportHeightmapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.menuImportTileTypes = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.menuImportMapCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MapWZToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UndoLimitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutosaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuAutosaveEnabled = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuAutosaveInterval = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuAutosaveChanges = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.menuAutosaveOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.CursorModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuPointerPlane = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuPointerDirect = New System.Windows.Forms.ToolStripMenuItem()
        Me.DisplayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuFont = New System.Windows.Forms.ToolStripMenuItem()
        Me.tmrTool = New System.Windows.Forms.Timer(Me.components)
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.TabPage13 = New System.Windows.Forms.TabPage()
        Me.TabPage14 = New System.Windows.Forms.TabPage()
        Me.TabPage15 = New System.Windows.Forms.TabPage()
        Me.TabPage16 = New System.Windows.Forms.TabPage()
        Me.TabPage21 = New System.Windows.Forms.TabPage()
        Me.TabPage22 = New System.Windows.Forms.TabPage()
        Me.TabPage23 = New System.Windows.Forms.TabPage()
        Me.TabPage24 = New System.Windows.Forms.TabPage()
        Me.FontDialog = New System.Windows.Forms.FontDialog()
        Me.ilInterface = New System.Windows.Forms.ImageList(Me.components)
        Me.menuMiniShowCliffs = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControl.SuspendLayout()
        Me.tpTextures.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.tabTextureBrushShape.SuspendLayout()
        CType(Me.nudTextureBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel6.SuspendLayout()
        Me.tpAutoTexture.SuspendLayout()
        Me.tabAutoTextureBrushShape.SuspendLayout()
        CType(Me.nudAutoCliffBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudAutoTextureRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpHeight.SuspendLayout()
        Me.tabHeightBrushShape.SuspendLayout()
        Me.tabHeightSetR.SuspendLayout()
        Me.tabHeightSetL.SuspendLayout()
        CType(Me.nudHeightBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpAutoHeight.SuspendLayout()
        Me.tpResize.SuspendLayout()
        Me.tpObjects.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.tpObject.SuspendLayout()
        Me.TableLayoutPanel7.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.tsTools.SuspendLayout()
        Me.tsFile.SuspendLayout()
        Me.tsSelection.SuspendLayout()
        Me.tsMinimap.SuspendLayout()
        Me.menuMain.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrKey
        '
        Me.tmrKey.Enabled = True
        Me.tmrKey.Interval = 30
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(4, 35)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(4)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabControl)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control
        Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel7)
        Me.SplitContainer1.Size = New System.Drawing.Size(1288, 616)
        Me.SplitContainer1.SplitterDistance = 422
        Me.SplitContainer1.TabIndex = 0
        '
        'TabControl
        '
        Me.TabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.TabControl.Controls.Add(Me.tpTextures)
        Me.TabControl.Controls.Add(Me.tpAutoTexture)
        Me.TabControl.Controls.Add(Me.tpHeight)
        Me.TabControl.Controls.Add(Me.tpAutoHeight)
        Me.TabControl.Controls.Add(Me.tpResize)
        Me.TabControl.Controls.Add(Me.tpObjects)
        Me.TabControl.Controls.Add(Me.tpObject)
        Me.TabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl.ItemSize = New System.Drawing.Size(72, 22)
        Me.TabControl.Location = New System.Drawing.Point(0, 0)
        Me.TabControl.Margin = New System.Windows.Forms.Padding(0)
        Me.TabControl.Multiline = True
        Me.TabControl.Name = "TabControl"
        Me.TabControl.Padding = New System.Drawing.Point(0, 0)
        Me.TabControl.SelectedIndex = 0
        Me.TabControl.Size = New System.Drawing.Size(418, 612)
        Me.TabControl.TabIndex = 0
        '
        'tpTextures
        '
        Me.tpTextures.Controls.Add(Me.TableLayoutPanel6)
        Me.tpTextures.Location = New System.Drawing.Point(4, 51)
        Me.tpTextures.Margin = New System.Windows.Forms.Padding(0)
        Me.tpTextures.Name = "tpTextures"
        Me.tpTextures.Padding = New System.Windows.Forms.Padding(4)
        Me.tpTextures.Size = New System.Drawing.Size(410, 557)
        Me.tpTextures.TabIndex = 0
        Me.tpTextures.Text = "Textures"
        Me.tpTextures.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 1
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.Panel5, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.Panel6, 0, 2)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(4, 4)
        Me.TableLayoutPanel6.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 3
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(402, 549)
        Me.TableLayoutPanel6.TabIndex = 8
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.chkTextureOrientationRandomize)
        Me.Panel5.Controls.Add(Me.btnTextureFlipX)
        Me.Panel5.Controls.Add(Me.btnTextureClockwise)
        Me.Panel5.Controls.Add(Me.btnTextureAnticlockwise)
        Me.Panel5.Controls.Add(Me.chkSetTextureOrientation)
        Me.Panel5.Controls.Add(Me.chkSetTexture)
        Me.Panel5.Controls.Add(Me.tabTextureBrushShape)
        Me.Panel5.Controls.Add(Me.Label21)
        Me.Panel5.Controls.Add(Me.Label11)
        Me.Panel5.Controls.Add(Me.cmbTileset)
        Me.Panel5.Controls.Add(Me.nudTextureBrushRadius)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel5.Location = New System.Drawing.Point(4, 4)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(394, 150)
        Me.Panel5.TabIndex = 0
        '
        'chkTextureOrientationRandomize
        '
        Me.chkTextureOrientationRandomize.AutoSize = True
        Me.chkTextureOrientationRandomize.Location = New System.Drawing.Point(281, 110)
        Me.chkTextureOrientationRandomize.Margin = New System.Windows.Forms.Padding(4)
        Me.chkTextureOrientationRandomize.Name = "chkTextureOrientationRandomize"
        Me.chkTextureOrientationRandomize.Size = New System.Drawing.Size(95, 21)
        Me.chkTextureOrientationRandomize.TabIndex = 44
        Me.chkTextureOrientationRandomize.Text = "Randomize"
        Me.chkTextureOrientationRandomize.UseCompatibleTextRendering = True
        Me.chkTextureOrientationRandomize.UseVisualStyleBackColor = True
        '
        'btnTextureFlipX
        '
        Me.btnTextureFlipX.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTextureFlipX.Image = CType(resources.GetObject("btnTextureFlipX.Image"), System.Drawing.Image)
        Me.btnTextureFlipX.Location = New System.Drawing.Point(237, 103)
        Me.btnTextureFlipX.Margin = New System.Windows.Forms.Padding(0)
        Me.btnTextureFlipX.Name = "btnTextureFlipX"
        Me.btnTextureFlipX.Size = New System.Drawing.Size(32, 30)
        Me.btnTextureFlipX.TabIndex = 43
        Me.btnTextureFlipX.UseCompatibleTextRendering = True
        Me.btnTextureFlipX.UseVisualStyleBackColor = True
        '
        'btnTextureClockwise
        '
        Me.btnTextureClockwise.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTextureClockwise.Image = CType(resources.GetObject("btnTextureClockwise.Image"), System.Drawing.Image)
        Me.btnTextureClockwise.Location = New System.Drawing.Point(197, 103)
        Me.btnTextureClockwise.Margin = New System.Windows.Forms.Padding(0)
        Me.btnTextureClockwise.Name = "btnTextureClockwise"
        Me.btnTextureClockwise.Size = New System.Drawing.Size(32, 30)
        Me.btnTextureClockwise.TabIndex = 42
        Me.btnTextureClockwise.UseCompatibleTextRendering = True
        Me.btnTextureClockwise.UseVisualStyleBackColor = True
        '
        'btnTextureAnticlockwise
        '
        Me.btnTextureAnticlockwise.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTextureAnticlockwise.Image = CType(resources.GetObject("btnTextureAnticlockwise.Image"), System.Drawing.Image)
        Me.btnTextureAnticlockwise.Location = New System.Drawing.Point(157, 103)
        Me.btnTextureAnticlockwise.Margin = New System.Windows.Forms.Padding(0)
        Me.btnTextureAnticlockwise.Name = "btnTextureAnticlockwise"
        Me.btnTextureAnticlockwise.Size = New System.Drawing.Size(32, 30)
        Me.btnTextureAnticlockwise.TabIndex = 41
        Me.btnTextureAnticlockwise.UseCompatibleTextRendering = True
        Me.btnTextureAnticlockwise.UseVisualStyleBackColor = True
        '
        'chkSetTextureOrientation
        '
        Me.chkSetTextureOrientation.AutoSize = True
        Me.chkSetTextureOrientation.Checked = True
        Me.chkSetTextureOrientation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSetTextureOrientation.Location = New System.Drawing.Point(25, 110)
        Me.chkSetTextureOrientation.Margin = New System.Windows.Forms.Padding(4)
        Me.chkSetTextureOrientation.Name = "chkSetTextureOrientation"
        Me.chkSetTextureOrientation.Size = New System.Drawing.Size(116, 21)
        Me.chkSetTextureOrientation.TabIndex = 40
        Me.chkSetTextureOrientation.Text = "Set Orientation"
        Me.chkSetTextureOrientation.UseCompatibleTextRendering = True
        Me.chkSetTextureOrientation.UseVisualStyleBackColor = True
        '
        'chkSetTexture
        '
        Me.chkSetTexture.AutoSize = True
        Me.chkSetTexture.Checked = True
        Me.chkSetTexture.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSetTexture.Location = New System.Drawing.Point(25, 81)
        Me.chkSetTexture.Margin = New System.Windows.Forms.Padding(4)
        Me.chkSetTexture.Name = "chkSetTexture"
        Me.chkSetTexture.Size = New System.Drawing.Size(96, 21)
        Me.chkSetTexture.TabIndex = 39
        Me.chkSetTexture.Text = "Set Texture"
        Me.chkSetTexture.UseCompatibleTextRendering = True
        Me.chkSetTexture.UseVisualStyleBackColor = True
        '
        'tabTextureBrushShape
        '
        Me.tabTextureBrushShape.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabTextureBrushShape.Controls.Add(Me.TabPage37)
        Me.tabTextureBrushShape.Controls.Add(Me.TabPage38)
        Me.tabTextureBrushShape.ItemSize = New System.Drawing.Size(64, 24)
        Me.tabTextureBrushShape.Location = New System.Drawing.Point(213, 49)
        Me.tabTextureBrushShape.Margin = New System.Windows.Forms.Padding(0)
        Me.tabTextureBrushShape.Multiline = True
        Me.tabTextureBrushShape.Name = "tabTextureBrushShape"
        Me.tabTextureBrushShape.Padding = New System.Drawing.Point(0, 0)
        Me.tabTextureBrushShape.SelectedIndex = 0
        Me.tabTextureBrushShape.Size = New System.Drawing.Size(160, 32)
        Me.tabTextureBrushShape.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabTextureBrushShape.TabIndex = 38
        '
        'TabPage37
        '
        Me.TabPage37.Location = New System.Drawing.Point(4, 28)
        Me.TabPage37.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage37.Name = "TabPage37"
        Me.TabPage37.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage37.Size = New System.Drawing.Size(152, 0)
        Me.TabPage37.TabIndex = 0
        Me.TabPage37.Text = "Circular"
        Me.TabPage37.UseVisualStyleBackColor = True
        '
        'TabPage38
        '
        Me.TabPage38.Location = New System.Drawing.Point(4, 28)
        Me.TabPage38.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage38.Name = "TabPage38"
        Me.TabPage38.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage38.Size = New System.Drawing.Size(152, 0)
        Me.TabPage38.TabIndex = 1
        Me.TabPage38.Text = "Square"
        Me.TabPage38.UseVisualStyleBackColor = True
        '
        'Label21
        '
        Me.Label21.Location = New System.Drawing.Point(21, 17)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(64, 20)
        Me.Label21.TabIndex = 8
        Me.Label21.Text = "Tileset:"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label21.UseCompatibleTextRendering = True
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(47, 49)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(64, 20)
        Me.Label11.TabIndex = 5
        Me.Label11.Text = "Radius"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label11.UseCompatibleTextRendering = True
        '
        'cmbTileset
        '
        Me.cmbTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTileset.FormattingEnabled = True
        Me.cmbTileset.Items.AddRange(New Object() {"Arizona", "Urban", "Rocky Mountains"})
        Me.cmbTileset.Location = New System.Drawing.Point(103, 16)
        Me.cmbTileset.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbTileset.Name = "cmbTileset"
        Me.cmbTileset.Size = New System.Drawing.Size(161, 24)
        Me.cmbTileset.TabIndex = 0
        '
        'nudTextureBrushRadius
        '
        Me.nudTextureBrushRadius.DecimalPlaces = 1
        Me.nudTextureBrushRadius.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.nudTextureBrushRadius.Location = New System.Drawing.Point(119, 49)
        Me.nudTextureBrushRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.nudTextureBrushRadius.Maximum = New Decimal(New Integer() {512, 0, 0, 0})
        Me.nudTextureBrushRadius.Name = "nudTextureBrushRadius"
        Me.nudTextureBrushRadius.Size = New System.Drawing.Size(75, 22)
        Me.nudTextureBrushRadius.TabIndex = 7
        Me.nudTextureBrushRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.chkTileNumbers)
        Me.Panel6.Controls.Add(Me.chkTileTypes)
        Me.Panel6.Controls.Add(Me.Label20)
        Me.Panel6.Controls.Add(Me.cmbTileType)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel6.Location = New System.Drawing.Point(4, 474)
        Me.Panel6.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(394, 71)
        Me.Panel6.TabIndex = 2
        '
        'chkTileNumbers
        '
        Me.chkTileNumbers.Location = New System.Drawing.Point(175, 37)
        Me.chkTileNumbers.Margin = New System.Windows.Forms.Padding(4)
        Me.chkTileNumbers.Name = "chkTileNumbers"
        Me.chkTileNumbers.Size = New System.Drawing.Size(185, 28)
        Me.chkTileNumbers.TabIndex = 3
        Me.chkTileNumbers.Text = "Display Tile Numbers"
        Me.chkTileNumbers.UseCompatibleTextRendering = True
        Me.chkTileNumbers.UseVisualStyleBackColor = True
        '
        'chkTileTypes
        '
        Me.chkTileTypes.Location = New System.Drawing.Point(8, 37)
        Me.chkTileTypes.Margin = New System.Windows.Forms.Padding(4)
        Me.chkTileTypes.Name = "chkTileTypes"
        Me.chkTileTypes.Size = New System.Drawing.Size(159, 28)
        Me.chkTileTypes.TabIndex = 2
        Me.chkTileTypes.Text = "Display Tile Types"
        Me.chkTileTypes.UseCompatibleTextRendering = True
        Me.chkTileTypes.UseVisualStyleBackColor = True
        '
        'Label20
        '
        Me.Label20.Location = New System.Drawing.Point(4, 6)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(91, 26)
        Me.Label20.TabIndex = 1
        Me.Label20.Text = "Tile Type:"
        Me.Label20.UseCompatibleTextRendering = True
        '
        'cmbTileType
        '
        Me.cmbTileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTileType.Enabled = False
        Me.cmbTileType.FormattingEnabled = True
        Me.cmbTileType.Location = New System.Drawing.Point(103, 4)
        Me.cmbTileType.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbTileType.Name = "cmbTileType"
        Me.cmbTileType.Size = New System.Drawing.Size(151, 24)
        Me.cmbTileType.TabIndex = 0
        '
        'tpAutoTexture
        '
        Me.tpAutoTexture.AutoScroll = True
        Me.tpAutoTexture.Controls.Add(Me.tabAutoTextureBrushShape)
        Me.tpAutoTexture.Controls.Add(Me.chkAutoTexSetHeight)
        Me.tpAutoTexture.Controls.Add(Me.chkCliffTris)
        Me.tpAutoTexture.Controls.Add(Me.btnMapTexturer)
        Me.tpAutoTexture.Controls.Add(Me.Label29)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoRoadLine)
        Me.tpAutoTexture.Controls.Add(Me.btnAutoTextureRemove)
        Me.tpAutoTexture.Controls.Add(Me.btnAutoRoadRemove)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoRoadPlace)
        Me.tpAutoTexture.Controls.Add(Me.lstAutoRoad)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoTexturePlace)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoTextureFill)
        Me.tpAutoTexture.Controls.Add(Me.nudAutoCliffBrushRadius)
        Me.tpAutoTexture.Controls.Add(Me.Label17)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoCliffBrush)
        Me.tpAutoTexture.Controls.Add(Me.rdoAutoCliffRemove)
        Me.tpAutoTexture.Controls.Add(Me.nudAutoTextureRadius)
        Me.tpAutoTexture.Controls.Add(Me.txtAutoCliffSlope)
        Me.tpAutoTexture.Controls.Add(Me.Label1)
        Me.tpAutoTexture.Controls.Add(Me.lstAutoTexture)
        Me.tpAutoTexture.Controls.Add(Me.Label3)
        Me.tpAutoTexture.Controls.Add(Me.Label2)
        Me.tpAutoTexture.Location = New System.Drawing.Point(4, 51)
        Me.tpAutoTexture.Margin = New System.Windows.Forms.Padding(4)
        Me.tpAutoTexture.Name = "tpAutoTexture"
        Me.tpAutoTexture.Size = New System.Drawing.Size(410, 557)
        Me.tpAutoTexture.TabIndex = 2
        Me.tpAutoTexture.Text = "Terrain Painter"
        Me.tpAutoTexture.UseVisualStyleBackColor = True
        '
        'tabAutoTextureBrushShape
        '
        Me.tabAutoTextureBrushShape.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabAutoTextureBrushShape.Controls.Add(Me.TabPage35)
        Me.tabAutoTextureBrushShape.Controls.Add(Me.TabPage36)
        Me.tabAutoTextureBrushShape.ItemSize = New System.Drawing.Size(64, 24)
        Me.tabAutoTextureBrushShape.Location = New System.Drawing.Point(179, 10)
        Me.tabAutoTextureBrushShape.Margin = New System.Windows.Forms.Padding(0)
        Me.tabAutoTextureBrushShape.Multiline = True
        Me.tabAutoTextureBrushShape.Name = "tabAutoTextureBrushShape"
        Me.tabAutoTextureBrushShape.Padding = New System.Drawing.Point(0, 0)
        Me.tabAutoTextureBrushShape.SelectedIndex = 0
        Me.tabAutoTextureBrushShape.Size = New System.Drawing.Size(160, 32)
        Me.tabAutoTextureBrushShape.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabAutoTextureBrushShape.TabIndex = 37
        '
        'TabPage35
        '
        Me.TabPage35.Location = New System.Drawing.Point(4, 28)
        Me.TabPage35.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage35.Name = "TabPage35"
        Me.TabPage35.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage35.Size = New System.Drawing.Size(152, 0)
        Me.TabPage35.TabIndex = 0
        Me.TabPage35.Text = "Circular"
        Me.TabPage35.UseVisualStyleBackColor = True
        '
        'TabPage36
        '
        Me.TabPage36.Location = New System.Drawing.Point(4, 28)
        Me.TabPage36.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage36.Name = "TabPage36"
        Me.TabPage36.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage36.Size = New System.Drawing.Size(152, 0)
        Me.TabPage36.TabIndex = 1
        Me.TabPage36.Text = "Square"
        Me.TabPage36.UseVisualStyleBackColor = True
        '
        'chkAutoTexSetHeight
        '
        Me.chkAutoTexSetHeight.Location = New System.Drawing.Point(96, 215)
        Me.chkAutoTexSetHeight.Margin = New System.Windows.Forms.Padding(4)
        Me.chkAutoTexSetHeight.Name = "chkAutoTexSetHeight"
        Me.chkAutoTexSetHeight.Size = New System.Drawing.Size(127, 21)
        Me.chkAutoTexSetHeight.TabIndex = 36
        Me.chkAutoTexSetHeight.Text = "Set Height"
        Me.chkAutoTexSetHeight.UseCompatibleTextRendering = True
        Me.chkAutoTexSetHeight.UseVisualStyleBackColor = True
        '
        'chkCliffTris
        '
        Me.chkCliffTris.Checked = True
        Me.chkCliffTris.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCliffTris.Location = New System.Drawing.Point(161, 489)
        Me.chkCliffTris.Margin = New System.Windows.Forms.Padding(4)
        Me.chkCliffTris.Name = "chkCliffTris"
        Me.chkCliffTris.Size = New System.Drawing.Size(127, 21)
        Me.chkCliffTris.TabIndex = 35
        Me.chkCliffTris.Text = "Set Tris"
        Me.chkCliffTris.UseCompatibleTextRendering = True
        Me.chkCliffTris.UseVisualStyleBackColor = True
        '
        'btnMapTexturer
        '
        Me.btnMapTexturer.Location = New System.Drawing.Point(14, 283)
        Me.btnMapTexturer.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMapTexturer.Name = "btnMapTexturer"
        Me.btnMapTexturer.Size = New System.Drawing.Size(160, 30)
        Me.btnMapTexturer.TabIndex = 34
        Me.btnMapTexturer.Text = "Entire Map Painter"
        Me.btnMapTexturer.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.Location = New System.Drawing.Point(11, 328)
        Me.Label29.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(107, 20)
        Me.Label29.TabIndex = 33
        Me.Label29.Text = "Road Type:"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'rdoAutoRoadLine
        '
        Me.rdoAutoRoadLine.AutoSize = True
        Me.rdoAutoRoadLine.Location = New System.Drawing.Point(11, 423)
        Me.rdoAutoRoadLine.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoRoadLine.Name = "rdoAutoRoadLine"
        Me.rdoAutoRoadLine.Size = New System.Drawing.Size(58, 21)
        Me.rdoAutoRoadLine.TabIndex = 32
        Me.rdoAutoRoadLine.Text = "Lines"
        Me.rdoAutoRoadLine.UseCompatibleTextRendering = True
        Me.rdoAutoRoadLine.UseVisualStyleBackColor = True
        '
        'btnAutoTextureRemove
        '
        Me.btnAutoTextureRemove.Location = New System.Drawing.Point(178, 177)
        Me.btnAutoTextureRemove.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAutoTextureRemove.Name = "btnAutoTextureRemove"
        Me.btnAutoTextureRemove.Size = New System.Drawing.Size(85, 30)
        Me.btnAutoTextureRemove.TabIndex = 31
        Me.btnAutoTextureRemove.Text = "Erase"
        Me.btnAutoTextureRemove.UseVisualStyleBackColor = True
        '
        'btnAutoRoadRemove
        '
        Me.btnAutoRoadRemove.Location = New System.Drawing.Point(85, 396)
        Me.btnAutoRoadRemove.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAutoRoadRemove.Name = "btnAutoRoadRemove"
        Me.btnAutoRoadRemove.Size = New System.Drawing.Size(85, 30)
        Me.btnAutoRoadRemove.TabIndex = 30
        Me.btnAutoRoadRemove.Text = "Erase"
        Me.btnAutoRoadRemove.UseVisualStyleBackColor = True
        '
        'rdoAutoRoadPlace
        '
        Me.rdoAutoRoadPlace.AutoSize = True
        Me.rdoAutoRoadPlace.Location = New System.Drawing.Point(11, 394)
        Me.rdoAutoRoadPlace.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoRoadPlace.Name = "rdoAutoRoadPlace"
        Me.rdoAutoRoadPlace.Size = New System.Drawing.Size(59, 21)
        Me.rdoAutoRoadPlace.TabIndex = 29
        Me.rdoAutoRoadPlace.Text = "Sides"
        Me.rdoAutoRoadPlace.UseCompatibleTextRendering = True
        Me.rdoAutoRoadPlace.UseVisualStyleBackColor = True
        '
        'lstAutoRoad
        '
        Me.lstAutoRoad.FormattingEnabled = True
        Me.lstAutoRoad.ItemHeight = 16
        Me.lstAutoRoad.Location = New System.Drawing.Point(11, 350)
        Me.lstAutoRoad.Margin = New System.Windows.Forms.Padding(4)
        Me.lstAutoRoad.Name = "lstAutoRoad"
        Me.lstAutoRoad.ScrollAlwaysVisible = True
        Me.lstAutoRoad.Size = New System.Drawing.Size(159, 36)
        Me.lstAutoRoad.TabIndex = 27
        '
        'rdoAutoTexturePlace
        '
        Me.rdoAutoTexturePlace.AutoSize = True
        Me.rdoAutoTexturePlace.Checked = True
        Me.rdoAutoTexturePlace.Location = New System.Drawing.Point(11, 214)
        Me.rdoAutoTexturePlace.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoTexturePlace.Name = "rdoAutoTexturePlace"
        Me.rdoAutoTexturePlace.Size = New System.Drawing.Size(59, 21)
        Me.rdoAutoTexturePlace.TabIndex = 26
        Me.rdoAutoTexturePlace.TabStop = True
        Me.rdoAutoTexturePlace.Text = "Place"
        Me.rdoAutoTexturePlace.UseCompatibleTextRendering = True
        Me.rdoAutoTexturePlace.UseVisualStyleBackColor = True
        '
        'rdoAutoTextureFill
        '
        Me.rdoAutoTextureFill.AutoSize = True
        Me.rdoAutoTextureFill.Location = New System.Drawing.Point(11, 244)
        Me.rdoAutoTextureFill.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoTextureFill.Name = "rdoAutoTextureFill"
        Me.rdoAutoTextureFill.Size = New System.Drawing.Size(43, 21)
        Me.rdoAutoTextureFill.TabIndex = 25
        Me.rdoAutoTextureFill.Text = "Fill"
        Me.rdoAutoTextureFill.UseCompatibleTextRendering = True
        Me.rdoAutoTextureFill.UseVisualStyleBackColor = True
        '
        'nudAutoCliffBrushRadius
        '
        Me.nudAutoCliffBrushRadius.DecimalPlaces = 1
        Me.nudAutoCliffBrushRadius.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.nudAutoCliffBrushRadius.Location = New System.Drawing.Point(117, 542)
        Me.nudAutoCliffBrushRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.nudAutoCliffBrushRadius.Maximum = New Decimal(New Integer() {512, 0, 0, 0})
        Me.nudAutoCliffBrushRadius.Name = "nudAutoCliffBrushRadius"
        Me.nudAutoCliffBrushRadius.Size = New System.Drawing.Size(75, 22)
        Me.nudAutoCliffBrushRadius.TabIndex = 24
        Me.nudAutoCliffBrushRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudAutoCliffBrushRadius.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'Label17
        '
        Me.Label17.Location = New System.Drawing.Point(32, 542)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(75, 20)
        Me.Label17.TabIndex = 23
        Me.Label17.Text = "Radius"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label17.UseCompatibleTextRendering = True
        '
        'rdoAutoCliffBrush
        '
        Me.rdoAutoCliffBrush.AutoSize = True
        Me.rdoAutoCliffBrush.Location = New System.Drawing.Point(11, 486)
        Me.rdoAutoCliffBrush.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoCliffBrush.Name = "rdoAutoCliffBrush"
        Me.rdoAutoCliffBrush.Size = New System.Drawing.Size(88, 21)
        Me.rdoAutoCliffBrush.TabIndex = 22
        Me.rdoAutoCliffBrush.Text = "Cliff Brush"
        Me.rdoAutoCliffBrush.UseCompatibleTextRendering = True
        Me.rdoAutoCliffBrush.UseVisualStyleBackColor = True
        '
        'rdoAutoCliffRemove
        '
        Me.rdoAutoCliffRemove.AutoSize = True
        Me.rdoAutoCliffRemove.Location = New System.Drawing.Point(11, 516)
        Me.rdoAutoCliffRemove.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoAutoCliffRemove.Name = "rdoAutoCliffRemove"
        Me.rdoAutoCliffRemove.Size = New System.Drawing.Size(102, 21)
        Me.rdoAutoCliffRemove.TabIndex = 21
        Me.rdoAutoCliffRemove.Text = "Cliff Remove"
        Me.rdoAutoCliffRemove.UseCompatibleTextRendering = True
        Me.rdoAutoCliffRemove.UseVisualStyleBackColor = True
        '
        'nudAutoTextureRadius
        '
        Me.nudAutoTextureRadius.DecimalPlaces = 1
        Me.nudAutoTextureRadius.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.nudAutoTextureRadius.Location = New System.Drawing.Point(96, 10)
        Me.nudAutoTextureRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.nudAutoTextureRadius.Maximum = New Decimal(New Integer() {512, 0, 0, 0})
        Me.nudAutoTextureRadius.Name = "nudAutoTextureRadius"
        Me.nudAutoTextureRadius.Size = New System.Drawing.Size(75, 22)
        Me.nudAutoTextureRadius.TabIndex = 20
        Me.nudAutoTextureRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudAutoTextureRadius.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'txtAutoCliffSlope
        '
        Me.txtAutoCliffSlope.Location = New System.Drawing.Point(117, 462)
        Me.txtAutoCliffSlope.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAutoCliffSlope.Name = "txtAutoCliffSlope"
        Me.txtAutoCliffSlope.Size = New System.Drawing.Size(52, 22)
        Me.txtAutoCliffSlope.TabIndex = 7
        Me.txtAutoCliffSlope.Text = "35"
        Me.txtAutoCliffSlope.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(11, 462)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 20)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Cliff Angle"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label1.UseCompatibleTextRendering = True
        '
        'lstAutoTexture
        '
        Me.lstAutoTexture.FormattingEnabled = True
        Me.lstAutoTexture.ItemHeight = 16
        Me.lstAutoTexture.Location = New System.Drawing.Point(11, 59)
        Me.lstAutoTexture.Margin = New System.Windows.Forms.Padding(4)
        Me.lstAutoTexture.Name = "lstAutoTexture"
        Me.lstAutoTexture.ScrollAlwaysVisible = True
        Me.lstAutoTexture.Size = New System.Drawing.Size(159, 148)
        Me.lstAutoTexture.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(11, 39)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(107, 20)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Ground Type"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label3.UseCompatibleTextRendering = True
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(11, 10)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Radius"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label2.UseCompatibleTextRendering = True
        '
        'tpHeight
        '
        Me.tpHeight.AutoScroll = True
        Me.tpHeight.Controls.Add(Me.btnHeightsMultiplySelection)
        Me.tpHeight.Controls.Add(Me.btnHeightOffsetSelection)
        Me.tpHeight.Controls.Add(Me.tabHeightBrushShape)
        Me.tpHeight.Controls.Add(Me.tabHeightSetR)
        Me.tpHeight.Controls.Add(Me.tabHeightSetL)
        Me.tpHeight.Controls.Add(Me.txtHeightSetR)
        Me.tpHeight.Controls.Add(Me.Label27)
        Me.tpHeight.Controls.Add(Me.Label10)
        Me.tpHeight.Controls.Add(Me.txtHeightOffset)
        Me.tpHeight.Controls.Add(Me.Label9)
        Me.tpHeight.Controls.Add(Me.txtHeightMultiply)
        Me.tpHeight.Controls.Add(Me.txtHeightChangeRate)
        Me.tpHeight.Controls.Add(Me.Label18)
        Me.tpHeight.Controls.Add(Me.rdoHeightChange)
        Me.tpHeight.Controls.Add(Me.Label16)
        Me.tpHeight.Controls.Add(Me.nudHeightBrushRadius)
        Me.tpHeight.Controls.Add(Me.btnMultiplier)
        Me.tpHeight.Controls.Add(Me.Label8)
        Me.tpHeight.Controls.Add(Me.txtMultiplier)
        Me.tpHeight.Controls.Add(Me.txtSmoothRadius)
        Me.tpHeight.Controls.Add(Me.Label7)
        Me.tpHeight.Controls.Add(Me.txtSmoothRate)
        Me.tpHeight.Controls.Add(Me.Label6)
        Me.tpHeight.Controls.Add(Me.rdoHeightSmooth)
        Me.tpHeight.Controls.Add(Me.rdoHeightSet)
        Me.tpHeight.Controls.Add(Me.txtHeightSetL)
        Me.tpHeight.Controls.Add(Me.Label5)
        Me.tpHeight.Controls.Add(Me.Label4)
        Me.tpHeight.Location = New System.Drawing.Point(4, 51)
        Me.tpHeight.Margin = New System.Windows.Forms.Padding(4)
        Me.tpHeight.Name = "tpHeight"
        Me.tpHeight.Padding = New System.Windows.Forms.Padding(4)
        Me.tpHeight.Size = New System.Drawing.Size(410, 557)
        Me.tpHeight.TabIndex = 1
        Me.tpHeight.Text = "Height"
        Me.tpHeight.UseVisualStyleBackColor = True
        '
        'btnHeightsMultiplySelection
        '
        Me.btnHeightsMultiplySelection.Location = New System.Drawing.Point(121, 383)
        Me.btnHeightsMultiplySelection.Margin = New System.Windows.Forms.Padding(4)
        Me.btnHeightsMultiplySelection.Name = "btnHeightsMultiplySelection"
        Me.btnHeightsMultiplySelection.Size = New System.Drawing.Size(75, 30)
        Me.btnHeightsMultiplySelection.TabIndex = 38
        Me.btnHeightsMultiplySelection.Text = "Do"
        Me.btnHeightsMultiplySelection.UseVisualStyleBackColor = True
        '
        'btnHeightOffsetSelection
        '
        Me.btnHeightOffsetSelection.Location = New System.Drawing.Point(121, 452)
        Me.btnHeightOffsetSelection.Margin = New System.Windows.Forms.Padding(4)
        Me.btnHeightOffsetSelection.Name = "btnHeightOffsetSelection"
        Me.btnHeightOffsetSelection.Size = New System.Drawing.Size(75, 30)
        Me.btnHeightOffsetSelection.TabIndex = 37
        Me.btnHeightOffsetSelection.Text = "Do"
        Me.btnHeightOffsetSelection.UseVisualStyleBackColor = True
        '
        'tabHeightBrushShape
        '
        Me.tabHeightBrushShape.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabHeightBrushShape.Controls.Add(Me.TabPage33)
        Me.tabHeightBrushShape.Controls.Add(Me.TabPage34)
        Me.tabHeightBrushShape.ItemSize = New System.Drawing.Size(64, 24)
        Me.tabHeightBrushShape.Location = New System.Drawing.Point(187, 10)
        Me.tabHeightBrushShape.Margin = New System.Windows.Forms.Padding(0)
        Me.tabHeightBrushShape.Multiline = True
        Me.tabHeightBrushShape.Name = "tabHeightBrushShape"
        Me.tabHeightBrushShape.Padding = New System.Drawing.Point(0, 0)
        Me.tabHeightBrushShape.SelectedIndex = 0
        Me.tabHeightBrushShape.Size = New System.Drawing.Size(160, 32)
        Me.tabHeightBrushShape.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabHeightBrushShape.TabIndex = 36
        '
        'TabPage33
        '
        Me.TabPage33.Location = New System.Drawing.Point(4, 28)
        Me.TabPage33.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage33.Name = "TabPage33"
        Me.TabPage33.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage33.Size = New System.Drawing.Size(152, 0)
        Me.TabPage33.TabIndex = 0
        Me.TabPage33.Text = "Circular"
        Me.TabPage33.UseVisualStyleBackColor = True
        '
        'TabPage34
        '
        Me.TabPage34.Location = New System.Drawing.Point(4, 28)
        Me.TabPage34.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage34.Name = "TabPage34"
        Me.TabPage34.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage34.Size = New System.Drawing.Size(152, 0)
        Me.TabPage34.TabIndex = 1
        Me.TabPage34.Text = "Square"
        Me.TabPage34.UseVisualStyleBackColor = True
        '
        'tabHeightSetR
        '
        Me.tabHeightSetR.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabHeightSetR.Controls.Add(Me.TabPage25)
        Me.tabHeightSetR.Controls.Add(Me.TabPage26)
        Me.tabHeightSetR.Controls.Add(Me.TabPage27)
        Me.tabHeightSetR.Controls.Add(Me.TabPage28)
        Me.tabHeightSetR.Controls.Add(Me.TabPage29)
        Me.tabHeightSetR.Controls.Add(Me.TabPage30)
        Me.tabHeightSetR.Controls.Add(Me.TabPage31)
        Me.tabHeightSetR.Controls.Add(Me.TabPage32)
        Me.tabHeightSetR.ItemSize = New System.Drawing.Size(28, 20)
        Me.tabHeightSetR.Location = New System.Drawing.Point(29, 169)
        Me.tabHeightSetR.Margin = New System.Windows.Forms.Padding(0)
        Me.tabHeightSetR.Multiline = True
        Me.tabHeightSetR.Name = "tabHeightSetR"
        Me.tabHeightSetR.Padding = New System.Drawing.Point(0, 0)
        Me.tabHeightSetR.SelectedIndex = 0
        Me.tabHeightSetR.Size = New System.Drawing.Size(439, 25)
        Me.tabHeightSetR.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabHeightSetR.TabIndex = 35
        '
        'TabPage25
        '
        Me.TabPage25.Location = New System.Drawing.Point(4, 24)
        Me.TabPage25.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage25.Name = "TabPage25"
        Me.TabPage25.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage25.Size = New System.Drawing.Size(431, 0)
        Me.TabPage25.TabIndex = 0
        Me.TabPage25.Text = "1"
        Me.TabPage25.UseVisualStyleBackColor = True
        '
        'TabPage26
        '
        Me.TabPage26.Location = New System.Drawing.Point(4, 24)
        Me.TabPage26.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage26.Name = "TabPage26"
        Me.TabPage26.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage26.Size = New System.Drawing.Size(431, 0)
        Me.TabPage26.TabIndex = 1
        Me.TabPage26.Text = "2"
        Me.TabPage26.UseVisualStyleBackColor = True
        '
        'TabPage27
        '
        Me.TabPage27.Location = New System.Drawing.Point(4, 24)
        Me.TabPage27.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage27.Name = "TabPage27"
        Me.TabPage27.Size = New System.Drawing.Size(431, 0)
        Me.TabPage27.TabIndex = 2
        Me.TabPage27.Text = "3"
        Me.TabPage27.UseVisualStyleBackColor = True
        '
        'TabPage28
        '
        Me.TabPage28.Location = New System.Drawing.Point(4, 24)
        Me.TabPage28.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage28.Name = "TabPage28"
        Me.TabPage28.Size = New System.Drawing.Size(431, 0)
        Me.TabPage28.TabIndex = 3
        Me.TabPage28.Text = "4"
        Me.TabPage28.UseVisualStyleBackColor = True
        '
        'TabPage29
        '
        Me.TabPage29.Location = New System.Drawing.Point(4, 24)
        Me.TabPage29.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage29.Name = "TabPage29"
        Me.TabPage29.Size = New System.Drawing.Size(431, 0)
        Me.TabPage29.TabIndex = 4
        Me.TabPage29.Text = "5"
        Me.TabPage29.UseVisualStyleBackColor = True
        '
        'TabPage30
        '
        Me.TabPage30.Location = New System.Drawing.Point(4, 24)
        Me.TabPage30.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage30.Name = "TabPage30"
        Me.TabPage30.Size = New System.Drawing.Size(431, 0)
        Me.TabPage30.TabIndex = 5
        Me.TabPage30.Text = "6"
        Me.TabPage30.UseVisualStyleBackColor = True
        '
        'TabPage31
        '
        Me.TabPage31.Location = New System.Drawing.Point(4, 24)
        Me.TabPage31.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage31.Name = "TabPage31"
        Me.TabPage31.Size = New System.Drawing.Size(431, 0)
        Me.TabPage31.TabIndex = 6
        Me.TabPage31.Text = "7"
        Me.TabPage31.UseVisualStyleBackColor = True
        '
        'TabPage32
        '
        Me.TabPage32.Location = New System.Drawing.Point(4, 24)
        Me.TabPage32.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage32.Name = "TabPage32"
        Me.TabPage32.Size = New System.Drawing.Size(431, 0)
        Me.TabPage32.TabIndex = 7
        Me.TabPage32.Text = "8"
        Me.TabPage32.UseVisualStyleBackColor = True
        '
        'tabHeightSetL
        '
        Me.tabHeightSetL.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabHeightSetL.Controls.Add(Me.TabPage9)
        Me.tabHeightSetL.Controls.Add(Me.TabPage10)
        Me.tabHeightSetL.Controls.Add(Me.TabPage11)
        Me.tabHeightSetL.Controls.Add(Me.TabPage12)
        Me.tabHeightSetL.Controls.Add(Me.TabPage17)
        Me.tabHeightSetL.Controls.Add(Me.TabPage18)
        Me.tabHeightSetL.Controls.Add(Me.TabPage19)
        Me.tabHeightSetL.Controls.Add(Me.TabPage20)
        Me.tabHeightSetL.ItemSize = New System.Drawing.Size(28, 20)
        Me.tabHeightSetL.Location = New System.Drawing.Point(29, 107)
        Me.tabHeightSetL.Margin = New System.Windows.Forms.Padding(0)
        Me.tabHeightSetL.Multiline = True
        Me.tabHeightSetL.Name = "tabHeightSetL"
        Me.tabHeightSetL.Padding = New System.Drawing.Point(0, 0)
        Me.tabHeightSetL.SelectedIndex = 0
        Me.tabHeightSetL.Size = New System.Drawing.Size(439, 25)
        Me.tabHeightSetL.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.tabHeightSetL.TabIndex = 34
        '
        'TabPage9
        '
        Me.TabPage9.Location = New System.Drawing.Point(4, 24)
        Me.TabPage9.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage9.Size = New System.Drawing.Size(431, 0)
        Me.TabPage9.TabIndex = 0
        Me.TabPage9.Text = "1"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'TabPage10
        '
        Me.TabPage10.Location = New System.Drawing.Point(4, 24)
        Me.TabPage10.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPage10.Size = New System.Drawing.Size(431, 0)
        Me.TabPage10.TabIndex = 1
        Me.TabPage10.Text = "2"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'TabPage11
        '
        Me.TabPage11.Location = New System.Drawing.Point(4, 24)
        Me.TabPage11.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage11.Name = "TabPage11"
        Me.TabPage11.Size = New System.Drawing.Size(431, 0)
        Me.TabPage11.TabIndex = 2
        Me.TabPage11.Text = "3"
        Me.TabPage11.UseVisualStyleBackColor = True
        '
        'TabPage12
        '
        Me.TabPage12.Location = New System.Drawing.Point(4, 24)
        Me.TabPage12.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage12.Name = "TabPage12"
        Me.TabPage12.Size = New System.Drawing.Size(431, 0)
        Me.TabPage12.TabIndex = 3
        Me.TabPage12.Text = "4"
        Me.TabPage12.UseVisualStyleBackColor = True
        '
        'TabPage17
        '
        Me.TabPage17.Location = New System.Drawing.Point(4, 24)
        Me.TabPage17.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage17.Name = "TabPage17"
        Me.TabPage17.Size = New System.Drawing.Size(431, 0)
        Me.TabPage17.TabIndex = 4
        Me.TabPage17.Text = "5"
        Me.TabPage17.UseVisualStyleBackColor = True
        '
        'TabPage18
        '
        Me.TabPage18.Location = New System.Drawing.Point(4, 24)
        Me.TabPage18.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage18.Name = "TabPage18"
        Me.TabPage18.Size = New System.Drawing.Size(431, 0)
        Me.TabPage18.TabIndex = 5
        Me.TabPage18.Text = "6"
        Me.TabPage18.UseVisualStyleBackColor = True
        '
        'TabPage19
        '
        Me.TabPage19.Location = New System.Drawing.Point(4, 24)
        Me.TabPage19.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage19.Name = "TabPage19"
        Me.TabPage19.Size = New System.Drawing.Size(431, 0)
        Me.TabPage19.TabIndex = 6
        Me.TabPage19.Text = "7"
        Me.TabPage19.UseVisualStyleBackColor = True
        '
        'TabPage20
        '
        Me.TabPage20.Location = New System.Drawing.Point(4, 24)
        Me.TabPage20.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPage20.Name = "TabPage20"
        Me.TabPage20.Size = New System.Drawing.Size(431, 0)
        Me.TabPage20.TabIndex = 7
        Me.TabPage20.Text = "8"
        Me.TabPage20.UseVisualStyleBackColor = True
        '
        'txtHeightSetR
        '
        Me.txtHeightSetR.Location = New System.Drawing.Point(121, 140)
        Me.txtHeightSetR.Margin = New System.Windows.Forms.Padding(4)
        Me.txtHeightSetR.Name = "txtHeightSetR"
        Me.txtHeightSetR.Size = New System.Drawing.Size(73, 22)
        Me.txtHeightSetR.TabIndex = 31
        Me.txtHeightSetR.Text = "#"
        Me.txtHeightSetR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label27
        '
        Me.Label27.Location = New System.Drawing.Point(11, 140)
        Me.Label27.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(99, 20)
        Me.Label27.TabIndex = 30
        Me.Label27.Text = "RMB Height"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label27.UseCompatibleTextRendering = True
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(15, 431)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(209, 17)
        Me.Label10.TabIndex = 29
        Me.Label10.Text = "Offset Heights of Selection"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtHeightOffset
        '
        Me.txtHeightOffset.Location = New System.Drawing.Point(19, 455)
        Me.txtHeightOffset.Margin = New System.Windows.Forms.Padding(4)
        Me.txtHeightOffset.Name = "txtHeightOffset"
        Me.txtHeightOffset.Size = New System.Drawing.Size(89, 22)
        Me.txtHeightOffset.TabIndex = 27
        Me.txtHeightOffset.Text = "0"
        Me.txtHeightOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(11, 359)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(209, 20)
        Me.Label9.TabIndex = 25
        Me.Label9.Text = "Multiply Heights of Selection"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label9.UseCompatibleTextRendering = True
        '
        'txtHeightMultiply
        '
        Me.txtHeightMultiply.Location = New System.Drawing.Point(19, 386)
        Me.txtHeightMultiply.Margin = New System.Windows.Forms.Padding(4)
        Me.txtHeightMultiply.Name = "txtHeightMultiply"
        Me.txtHeightMultiply.Size = New System.Drawing.Size(89, 22)
        Me.txtHeightMultiply.TabIndex = 24
        Me.txtHeightMultiply.Text = "1"
        Me.txtHeightMultiply.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtHeightChangeRate
        '
        Me.txtHeightChangeRate.Location = New System.Drawing.Point(96, 246)
        Me.txtHeightChangeRate.Margin = New System.Windows.Forms.Padding(4)
        Me.txtHeightChangeRate.Name = "txtHeightChangeRate"
        Me.txtHeightChangeRate.Size = New System.Drawing.Size(73, 22)
        Me.txtHeightChangeRate.TabIndex = 23
        Me.txtHeightChangeRate.Text = "8"
        Me.txtHeightChangeRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label18
        '
        Me.Label18.Location = New System.Drawing.Point(11, 246)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(75, 20)
        Me.Label18.TabIndex = 22
        Me.Label18.Text = "Rate"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label18.UseCompatibleTextRendering = True
        '
        'rdoHeightChange
        '
        Me.rdoHeightChange.AutoSize = True
        Me.rdoHeightChange.Location = New System.Drawing.Point(11, 217)
        Me.rdoHeightChange.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoHeightChange.Name = "rdoHeightChange"
        Me.rdoHeightChange.Size = New System.Drawing.Size(73, 21)
        Me.rdoHeightChange.TabIndex = 21
        Me.rdoHeightChange.Text = "Change"
        Me.rdoHeightChange.UseCompatibleTextRendering = True
        Me.rdoHeightChange.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.Location = New System.Drawing.Point(121, 49)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(75, 21)
        Me.Label16.TabIndex = 20
        Me.Label16.Text = "(0-255)"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label16.UseCompatibleTextRendering = True
        '
        'nudHeightBrushRadius
        '
        Me.nudHeightBrushRadius.DecimalPlaces = 1
        Me.nudHeightBrushRadius.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.nudHeightBrushRadius.Location = New System.Drawing.Point(96, 10)
        Me.nudHeightBrushRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.nudHeightBrushRadius.Maximum = New Decimal(New Integer() {512, 0, 0, 0})
        Me.nudHeightBrushRadius.Name = "nudHeightBrushRadius"
        Me.nudHeightBrushRadius.Size = New System.Drawing.Size(75, 22)
        Me.nudHeightBrushRadius.TabIndex = 19
        Me.nudHeightBrushRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.nudHeightBrushRadius.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'btnMultiplier
        '
        Me.btnMultiplier.Enabled = False
        Me.btnMultiplier.Location = New System.Drawing.Point(303, 516)
        Me.btnMultiplier.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMultiplier.Name = "btnMultiplier"
        Me.btnMultiplier.Size = New System.Drawing.Size(75, 30)
        Me.btnMultiplier.TabIndex = 16
        Me.btnMultiplier.Text = "Change"
        Me.btnMultiplier.UseVisualStyleBackColor = True
        Me.btnMultiplier.Visible = False
        '
        'Label8
        '
        Me.Label8.Enabled = False
        Me.Label8.Location = New System.Drawing.Point(213, 489)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(160, 20)
        Me.Label8.TabIndex = 15
        Me.Label8.Text = "Height Multiplier"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label8.UseCompatibleTextRendering = True
        Me.Label8.Visible = False
        '
        'txtMultiplier
        '
        Me.txtMultiplier.Enabled = False
        Me.txtMultiplier.Location = New System.Drawing.Point(217, 516)
        Me.txtMultiplier.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMultiplier.Name = "txtMultiplier"
        Me.txtMultiplier.Size = New System.Drawing.Size(73, 22)
        Me.txtMultiplier.TabIndex = 13
        Me.txtMultiplier.Text = "2"
        Me.txtMultiplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtMultiplier.Visible = False
        '
        'txtSmoothRadius
        '
        Me.txtSmoothRadius.Enabled = False
        Me.txtSmoothRadius.Location = New System.Drawing.Point(311, 250)
        Me.txtSmoothRadius.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSmoothRadius.Name = "txtSmoothRadius"
        Me.txtSmoothRadius.Size = New System.Drawing.Size(73, 22)
        Me.txtSmoothRadius.TabIndex = 12
        Me.txtSmoothRadius.Text = "1.5"
        Me.txtSmoothRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtSmoothRadius.Visible = False
        '
        'Label7
        '
        Me.Label7.Enabled = False
        Me.Label7.Location = New System.Drawing.Point(225, 250)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(75, 20)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "Radius"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label7.UseCompatibleTextRendering = True
        Me.Label7.Visible = False
        '
        'txtSmoothRate
        '
        Me.txtSmoothRate.Location = New System.Drawing.Point(100, 311)
        Me.txtSmoothRate.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSmoothRate.Name = "txtSmoothRate"
        Me.txtSmoothRate.Size = New System.Drawing.Size(73, 22)
        Me.txtSmoothRate.TabIndex = 10
        Me.txtSmoothRate.Text = "3"
        Me.txtSmoothRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(15, 311)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(75, 20)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Rate"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label6.UseCompatibleTextRendering = True
        '
        'rdoHeightSmooth
        '
        Me.rdoHeightSmooth.AutoSize = True
        Me.rdoHeightSmooth.Location = New System.Drawing.Point(11, 286)
        Me.rdoHeightSmooth.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoHeightSmooth.Name = "rdoHeightSmooth"
        Me.rdoHeightSmooth.Size = New System.Drawing.Size(72, 21)
        Me.rdoHeightSmooth.TabIndex = 8
        Me.rdoHeightSmooth.Text = "Smooth"
        Me.rdoHeightSmooth.UseCompatibleTextRendering = True
        Me.rdoHeightSmooth.UseVisualStyleBackColor = True
        '
        'rdoHeightSet
        '
        Me.rdoHeightSet.AutoSize = True
        Me.rdoHeightSet.Checked = True
        Me.rdoHeightSet.Location = New System.Drawing.Point(11, 49)
        Me.rdoHeightSet.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoHeightSet.Name = "rdoHeightSet"
        Me.rdoHeightSet.Size = New System.Drawing.Size(46, 21)
        Me.rdoHeightSet.TabIndex = 7
        Me.rdoHeightSet.TabStop = True
        Me.rdoHeightSet.Text = "Set"
        Me.rdoHeightSet.UseCompatibleTextRendering = True
        Me.rdoHeightSet.UseVisualStyleBackColor = True
        '
        'txtHeightSetL
        '
        Me.txtHeightSetL.Location = New System.Drawing.Point(121, 79)
        Me.txtHeightSetL.Margin = New System.Windows.Forms.Padding(4)
        Me.txtHeightSetL.Name = "txtHeightSetL"
        Me.txtHeightSetL.Size = New System.Drawing.Size(73, 22)
        Me.txtHeightSetL.TabIndex = 6
        Me.txtHeightSetL.Text = "#"
        Me.txtHeightSetL.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(11, 79)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(99, 20)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "LMB Height"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label5.UseCompatibleTextRendering = True
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(11, 10)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 20)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Radius"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'tpAutoHeight
        '
        Me.tpAutoHeight.Controls.Add(Me.btnGenerator)
        Me.tpAutoHeight.Controls.Add(Me.btnWaterTri)
        Me.tpAutoHeight.Controls.Add(Me.btnReinterpretTerrain)
        Me.tpAutoHeight.Controls.Add(Me.btnAutoTri)
        Me.tpAutoHeight.Location = New System.Drawing.Point(4, 51)
        Me.tpAutoHeight.Margin = New System.Windows.Forms.Padding(4)
        Me.tpAutoHeight.Name = "tpAutoHeight"
        Me.tpAutoHeight.Size = New System.Drawing.Size(410, 557)
        Me.tpAutoHeight.TabIndex = 3
        Me.tpAutoHeight.Text = "Misc"
        Me.tpAutoHeight.UseVisualStyleBackColor = True
        '
        'btnGenerator
        '
        Me.btnGenerator.Location = New System.Drawing.Point(11, 122)
        Me.btnGenerator.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGenerator.Name = "btnGenerator"
        Me.btnGenerator.Size = New System.Drawing.Size(223, 30)
        Me.btnGenerator.TabIndex = 3
        Me.btnGenerator.Text = "Generator"
        Me.btnGenerator.UseCompatibleTextRendering = True
        Me.btnGenerator.UseVisualStyleBackColor = True
        '
        'btnWaterTri
        '
        Me.btnWaterTri.Location = New System.Drawing.Point(11, 47)
        Me.btnWaterTri.Margin = New System.Windows.Forms.Padding(4)
        Me.btnWaterTri.Name = "btnWaterTri"
        Me.btnWaterTri.Size = New System.Drawing.Size(223, 30)
        Me.btnWaterTri.TabIndex = 2
        Me.btnWaterTri.Text = "Water Triangle Correction"
        Me.btnWaterTri.UseCompatibleTextRendering = True
        Me.btnWaterTri.UseVisualStyleBackColor = True
        '
        'btnReinterpretTerrain
        '
        Me.btnReinterpretTerrain.Location = New System.Drawing.Point(11, 84)
        Me.btnReinterpretTerrain.Margin = New System.Windows.Forms.Padding(4)
        Me.btnReinterpretTerrain.Name = "btnReinterpretTerrain"
        Me.btnReinterpretTerrain.Size = New System.Drawing.Size(223, 30)
        Me.btnReinterpretTerrain.TabIndex = 1
        Me.btnReinterpretTerrain.Text = "Reinterpret Terrain"
        Me.btnReinterpretTerrain.UseCompatibleTextRendering = True
        Me.btnReinterpretTerrain.UseVisualStyleBackColor = True
        '
        'btnAutoTri
        '
        Me.btnAutoTri.Location = New System.Drawing.Point(11, 10)
        Me.btnAutoTri.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAutoTri.Name = "btnAutoTri"
        Me.btnAutoTri.Size = New System.Drawing.Size(223, 30)
        Me.btnAutoTri.TabIndex = 0
        Me.btnAutoTri.Text = "Set All Triangles"
        Me.btnAutoTri.UseCompatibleTextRendering = True
        Me.btnAutoTri.UseVisualStyleBackColor = True
        '
        'tpResize
        '
        Me.tpResize.Controls.Add(Me.btnSelResize)
        Me.tpResize.Controls.Add(Me.btnResize)
        Me.tpResize.Controls.Add(Me.txtOffsetY)
        Me.tpResize.Controls.Add(Me.Label15)
        Me.tpResize.Controls.Add(Me.txtOffsetX)
        Me.tpResize.Controls.Add(Me.Label14)
        Me.tpResize.Controls.Add(Me.txtSizeY)
        Me.tpResize.Controls.Add(Me.Label13)
        Me.tpResize.Controls.Add(Me.txtSizeX)
        Me.tpResize.Controls.Add(Me.Label12)
        Me.tpResize.Location = New System.Drawing.Point(4, 51)
        Me.tpResize.Margin = New System.Windows.Forms.Padding(4)
        Me.tpResize.Name = "tpResize"
        Me.tpResize.Size = New System.Drawing.Size(410, 557)
        Me.tpResize.TabIndex = 4
        Me.tpResize.Text = "Resize"
        Me.tpResize.UseVisualStyleBackColor = True
        '
        'btnSelResize
        '
        Me.btnSelResize.Location = New System.Drawing.Point(21, 175)
        Me.btnSelResize.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSelResize.Name = "btnSelResize"
        Me.btnSelResize.Size = New System.Drawing.Size(180, 30)
        Me.btnSelResize.TabIndex = 17
        Me.btnSelResize.Text = "Resize To Selection"
        Me.btnSelResize.UseCompatibleTextRendering = True
        Me.btnSelResize.UseVisualStyleBackColor = True
        '
        'btnResize
        '
        Me.btnResize.Location = New System.Drawing.Point(21, 138)
        Me.btnResize.Margin = New System.Windows.Forms.Padding(4)
        Me.btnResize.Name = "btnResize"
        Me.btnResize.Size = New System.Drawing.Size(148, 30)
        Me.btnResize.TabIndex = 16
        Me.btnResize.Text = "Resize"
        Me.btnResize.UseCompatibleTextRendering = True
        Me.btnResize.UseVisualStyleBackColor = True
        '
        'txtOffsetY
        '
        Me.txtOffsetY.Location = New System.Drawing.Point(117, 98)
        Me.txtOffsetY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtOffsetY.Name = "txtOffsetY"
        Me.txtOffsetY.Size = New System.Drawing.Size(52, 22)
        Me.txtOffsetY.TabIndex = 15
        Me.txtOffsetY.Text = "0"
        '
        'Label15
        '
        Me.Label15.Location = New System.Drawing.Point(11, 98)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(96, 20)
        Me.Label15.TabIndex = 14
        Me.Label15.Text = "Offset Y"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label15.UseCompatibleTextRendering = True
        '
        'txtOffsetX
        '
        Me.txtOffsetX.Location = New System.Drawing.Point(117, 69)
        Me.txtOffsetX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtOffsetX.Name = "txtOffsetX"
        Me.txtOffsetX.Size = New System.Drawing.Size(52, 22)
        Me.txtOffsetX.TabIndex = 13
        Me.txtOffsetX.Text = "0"
        '
        'Label14
        '
        Me.Label14.Location = New System.Drawing.Point(11, 69)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(96, 20)
        Me.Label14.TabIndex = 12
        Me.Label14.Text = "Offset X"
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label14.UseCompatibleTextRendering = True
        '
        'txtSizeY
        '
        Me.txtSizeY.Location = New System.Drawing.Point(117, 39)
        Me.txtSizeY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSizeY.Name = "txtSizeY"
        Me.txtSizeY.Size = New System.Drawing.Size(52, 22)
        Me.txtSizeY.TabIndex = 11
        Me.txtSizeY.Text = "0"
        '
        'Label13
        '
        Me.Label13.Location = New System.Drawing.Point(11, 39)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(96, 20)
        Me.Label13.TabIndex = 10
        Me.Label13.Text = "Size Y"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label13.UseCompatibleTextRendering = True
        '
        'txtSizeX
        '
        Me.txtSizeX.Location = New System.Drawing.Point(117, 10)
        Me.txtSizeX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSizeX.Name = "txtSizeX"
        Me.txtSizeX.Size = New System.Drawing.Size(52, 22)
        Me.txtSizeX.TabIndex = 9
        Me.txtSizeX.Text = "0"
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(11, 10)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(96, 20)
        Me.Label12.TabIndex = 8
        Me.Label12.Text = "Size X"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label12.UseCompatibleTextRendering = True
        '
        'tpObjects
        '
        Me.tpObjects.AutoScroll = True
        Me.tpObjects.Controls.Add(Me.TableLayoutPanel1)
        Me.tpObjects.Location = New System.Drawing.Point(4, 51)
        Me.tpObjects.Margin = New System.Windows.Forms.Padding(4)
        Me.tpObjects.Name = "tpObjects"
        Me.tpObjects.Size = New System.Drawing.Size(410, 557)
        Me.tpObjects.TabIndex = 5
        Me.tpObjects.Text = "Place Objects"
        Me.tpObjects.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel4, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel3, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(410, 557)
        Me.TableLayoutPanel1.TabIndex = 16
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 1
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.Panel4, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.lstDroids, 0, 1)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(4, 418)
        Me.TableLayoutPanel4.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 2
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(402, 135)
        Me.TableLayoutPanel4.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.Label31)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(4, 4)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(394, 51)
        Me.Panel4.TabIndex = 14
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(12, 21)
        Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(160, 20)
        Me.Label31.TabIndex = 15
        Me.Label31.Text = "Droid Templates"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label31.UseCompatibleTextRendering = True
        '
        'lstDroids
        '
        Me.lstDroids.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDroids.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstDroids.FormattingEnabled = True
        Me.lstDroids.ItemHeight = 17
        Me.lstDroids.Location = New System.Drawing.Point(4, 63)
        Me.lstDroids.Margin = New System.Windows.Forms.Padding(4)
        Me.lstDroids.Name = "lstDroids"
        Me.lstDroids.ScrollAlwaysVisible = True
        Me.lstDroids.Size = New System.Drawing.Size(394, 68)
        Me.lstDroids.TabIndex = 10
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 1
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.Panel3, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.lstStructures, 0, 1)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(4, 275)
        Me.TableLayoutPanel3.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 2
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59.0!))
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(402, 135)
        Me.TableLayoutPanel3.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.Label30)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(4, 4)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(394, 51)
        Me.Panel3.TabIndex = 13
        '
        'Label30
        '
        Me.Label30.Location = New System.Drawing.Point(12, 21)
        Me.Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(160, 20)
        Me.Label30.TabIndex = 14
        Me.Label30.Text = "Structures"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label30.UseCompatibleTextRendering = True
        '
        'lstStructures
        '
        Me.lstStructures.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstStructures.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstStructures.FormattingEnabled = True
        Me.lstStructures.ItemHeight = 17
        Me.lstStructures.Location = New System.Drawing.Point(4, 63)
        Me.lstStructures.Margin = New System.Windows.Forms.Padding(4)
        Me.lstStructures.Name = "lstStructures"
        Me.lstStructures.ScrollAlwaysVisible = True
        Me.lstStructures.Size = New System.Drawing.Size(394, 68)
        Me.lstStructures.TabIndex = 12
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Label32)
        Me.Panel1.Controls.Add(Me.txtPlayerNum)
        Me.Panel1.Controls.Add(Me.Label22)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(4, 4)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(402, 120)
        Me.Panel1.TabIndex = 0
        '
        'Label32
        '
        Me.Label32.Location = New System.Drawing.Point(16, 82)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(219, 38)
        Me.Label32.TabIndex = 16
        Me.Label32.Text = "Players 8 to 10 only work with master versions of Warzone."
        Me.Label32.UseCompatibleTextRendering = True
        '
        'txtPlayerNum
        '
        Me.txtPlayerNum.Location = New System.Drawing.Point(253, 11)
        Me.txtPlayerNum.Margin = New System.Windows.Forms.Padding(4)
        Me.txtPlayerNum.Name = "txtPlayerNum"
        Me.txtPlayerNum.Size = New System.Drawing.Size(52, 22)
        Me.txtPlayerNum.TabIndex = 15
        Me.txtPlayerNum.Text = "0"
        Me.txtPlayerNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtPlayerNum.Visible = False
        '
        'Label22
        '
        Me.Label22.Location = New System.Drawing.Point(16, 10)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(85, 25)
        Me.Label22.TabIndex = 14
        Me.Label22.Text = "Player"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label22.UseCompatibleTextRendering = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.lstFeatures, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Panel2, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(4, 132)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(402, 135)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'lstFeatures
        '
        Me.lstFeatures.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFeatures.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstFeatures.FormattingEnabled = True
        Me.lstFeatures.ItemHeight = 17
        Me.lstFeatures.Location = New System.Drawing.Point(4, 63)
        Me.lstFeatures.Margin = New System.Windows.Forms.Padding(4)
        Me.lstFeatures.Name = "lstFeatures"
        Me.lstFeatures.ScrollAlwaysVisible = True
        Me.lstFeatures.Size = New System.Drawing.Size(394, 68)
        Me.lstFeatures.TabIndex = 5
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Label19)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(4, 4)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(394, 51)
        Me.Panel2.TabIndex = 6
        '
        'Label19
        '
        Me.Label19.Location = New System.Drawing.Point(12, 21)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(160, 20)
        Me.Label19.TabIndex = 10
        Me.Label19.Text = "Features"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label19.UseCompatibleTextRendering = True
        '
        'tpObject
        '
        Me.tpObject.Controls.Add(Me.txtObjectPriority)
        Me.tpObject.Controls.Add(Me.Label33)
        Me.tpObject.Controls.Add(Me.Label26)
        Me.tpObject.Controls.Add(Me.txtObjectID)
        Me.tpObject.Controls.Add(Me.Label25)
        Me.tpObject.Controls.Add(Me.txtObjectPlayer)
        Me.tpObject.Controls.Add(Me.Label28)
        Me.tpObject.Controls.Add(Me.txtObjectRotation)
        Me.tpObject.Controls.Add(Me.Label23)
        Me.tpObject.Controls.Add(Me.lblObjectType)
        Me.tpObject.Controls.Add(Me.txtObjectName)
        Me.tpObject.Controls.Add(Me.Label24)
        Me.tpObject.Controls.Add(Me.txtObjesdss)
        Me.tpObject.Location = New System.Drawing.Point(4, 51)
        Me.tpObject.Margin = New System.Windows.Forms.Padding(4)
        Me.tpObject.Name = "tpObject"
        Me.tpObject.Size = New System.Drawing.Size(410, 557)
        Me.tpObject.TabIndex = 6
        Me.tpObject.Text = "Object Properties"
        Me.tpObject.UseVisualStyleBackColor = True
        '
        'txtObjectPriority
        '
        Me.txtObjectPriority.Location = New System.Drawing.Point(86, 186)
        Me.txtObjectPriority.Margin = New System.Windows.Forms.Padding(4)
        Me.txtObjectPriority.Name = "txtObjectPriority"
        Me.txtObjectPriority.Size = New System.Drawing.Size(66, 22)
        Me.txtObjectPriority.TabIndex = 35
        Me.txtObjectPriority.Text = "#"
        Me.txtObjectPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label33
        '
        Me.Label33.Location = New System.Drawing.Point(15, 183)
        Me.Label33.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(63, 25)
        Me.Label33.TabIndex = 34
        Me.Label33.Text = "Priority:"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label33.UseCompatibleTextRendering = True
        '
        'Label26
        '
        Me.Label26.Location = New System.Drawing.Point(161, 122)
        Me.Label26.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(88, 25)
        Me.Label26.TabIndex = 33
        Me.Label26.Text = "(0-359)"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label26.UseCompatibleTextRendering = True
        '
        'txtObjectID
        '
        Me.txtObjectID.Location = New System.Drawing.Point(111, 154)
        Me.txtObjectID.Margin = New System.Windows.Forms.Padding(4)
        Me.txtObjectID.Name = "txtObjectID"
        Me.txtObjectID.Size = New System.Drawing.Size(41, 22)
        Me.txtObjectID.TabIndex = 31
        Me.txtObjectID.Text = "#"
        Me.txtObjectID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label25
        '
        Me.Label25.Location = New System.Drawing.Point(28, 154)
        Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(75, 25)
        Me.Label25.TabIndex = 30
        Me.Label25.Text = "ID:"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label25.UseCompatibleTextRendering = True
        '
        'txtObjectPlayer
        '
        Me.txtObjectPlayer.Enabled = False
        Me.txtObjectPlayer.Location = New System.Drawing.Point(336, 60)
        Me.txtObjectPlayer.Margin = New System.Windows.Forms.Padding(4)
        Me.txtObjectPlayer.Name = "txtObjectPlayer"
        Me.txtObjectPlayer.Size = New System.Drawing.Size(41, 22)
        Me.txtObjectPlayer.TabIndex = 29
        Me.txtObjectPlayer.Text = "0"
        Me.txtObjectPlayer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtObjectPlayer.Visible = False
        '
        'Label28
        '
        Me.Label28.Location = New System.Drawing.Point(11, 59)
        Me.Label28.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(53, 25)
        Me.Label28.TabIndex = 28
        Me.Label28.Text = "Player:"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label28.UseCompatibleTextRendering = True
        '
        'txtObjectRotation
        '
        Me.txtObjectRotation.Location = New System.Drawing.Point(111, 122)
        Me.txtObjectRotation.Margin = New System.Windows.Forms.Padding(4)
        Me.txtObjectRotation.Name = "txtObjectRotation"
        Me.txtObjectRotation.Size = New System.Drawing.Size(41, 22)
        Me.txtObjectRotation.TabIndex = 25
        Me.txtObjectRotation.Text = "#"
        Me.txtObjectRotation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label23
        '
        Me.Label23.Location = New System.Drawing.Point(28, 121)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(75, 25)
        Me.Label23.TabIndex = 21
        Me.Label23.Text = "Rotation:"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label23.UseCompatibleTextRendering = True
        '
        'lblObjectType
        '
        Me.lblObjectType.Location = New System.Drawing.Point(11, 30)
        Me.lblObjectType.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblObjectType.Name = "lblObjectType"
        Me.lblObjectType.Size = New System.Drawing.Size(304, 26)
        Me.lblObjectType.TabIndex = 20
        Me.lblObjectType.Text = "Object Type"
        Me.lblObjectType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblObjectType.UseCompatibleTextRendering = True
        '
        'txtObjectName
        '
        Me.txtObjectName.Enabled = False
        Me.txtObjectName.Location = New System.Drawing.Point(248, 224)
        Me.txtObjectName.Margin = New System.Windows.Forms.Padding(4)
        Me.txtObjectName.Name = "txtObjectName"
        Me.txtObjectName.Size = New System.Drawing.Size(137, 22)
        Me.txtObjectName.TabIndex = 19
        Me.txtObjectName.Text = "NONAME"
        Me.txtObjectName.Visible = False
        '
        'Label24
        '
        Me.Label24.Location = New System.Drawing.Point(11, 10)
        Me.Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(107, 20)
        Me.Label24.TabIndex = 18
        Me.Label24.Text = "Type:"
        Me.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label24.UseCompatibleTextRendering = True
        '
        'txtObjesdss
        '
        Me.txtObjesdss.Location = New System.Drawing.Point(248, 204)
        Me.txtObjesdss.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.txtObjesdss.Name = "txtObjesdss"
        Me.txtObjesdss.Size = New System.Drawing.Size(107, 20)
        Me.txtObjesdss.TabIndex = 16
        Me.txtObjesdss.Text = "Name:"
        Me.txtObjesdss.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.txtObjesdss.UseCompatibleTextRendering = True
        Me.txtObjesdss.Visible = False
        '
        'TableLayoutPanel7
        '
        Me.TableLayoutPanel7.ColumnCount = 1
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.Controls.Add(Me.Panel7, 0, 0)
        Me.TableLayoutPanel7.Controls.Add(Me.pnlView, 0, 1)
        Me.TableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel7.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel7.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
        Me.TableLayoutPanel7.RowCount = 2
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.Size = New System.Drawing.Size(858, 612)
        Me.TableLayoutPanel7.TabIndex = 2
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.tsTools)
        Me.Panel7.Controls.Add(Me.tsFile)
        Me.Panel7.Controls.Add(Me.tsSelection)
        Me.Panel7.Controls.Add(Me.tsMinimap)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel7.Location = New System.Drawing.Point(0, 0)
        Me.Panel7.Margin = New System.Windows.Forms.Padding(0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(858, 30)
        Me.Panel7.TabIndex = 0
        '
        'tsTools
        '
        Me.tsTools.Dock = System.Windows.Forms.DockStyle.None
        Me.tsTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsTools.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbGateways, Me.tsbDrawAutotexture, Me.tsbDrawTileOrientation})
        Me.tsTools.Location = New System.Drawing.Point(372, 2)
        Me.tsTools.Name = "tsTools"
        Me.tsTools.Size = New System.Drawing.Size(72, 25)
        Me.tsTools.TabIndex = 2
        '
        'tsbGateways
        '
        Me.tsbGateways.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbGateways.Image = CType(resources.GetObject("tsbGateways.Image"), System.Drawing.Image)
        Me.tsbGateways.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbGateways.Name = "tsbGateways"
        Me.tsbGateways.Size = New System.Drawing.Size(23, 22)
        Me.tsbGateways.Text = "Gateways"
        '
        'tsbDrawAutotexture
        '
        Me.tsbDrawAutotexture.CheckOnClick = True
        Me.tsbDrawAutotexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDrawAutotexture.Image = CType(resources.GetObject("tsbDrawAutotexture.Image"), System.Drawing.Image)
        Me.tsbDrawAutotexture.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDrawAutotexture.Name = "tsbDrawAutotexture"
        Me.tsbDrawAutotexture.Size = New System.Drawing.Size(23, 22)
        Me.tsbDrawAutotexture.Text = "Display Painted Texture Markers"
        '
        'tsbDrawTileOrientation
        '
        Me.tsbDrawTileOrientation.CheckOnClick = True
        Me.tsbDrawTileOrientation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDrawTileOrientation.Image = CType(resources.GetObject("tsbDrawTileOrientation.Image"), System.Drawing.Image)
        Me.tsbDrawTileOrientation.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDrawTileOrientation.Name = "tsbDrawTileOrientation"
        Me.tsbDrawTileOrientation.Size = New System.Drawing.Size(23, 22)
        Me.tsbDrawTileOrientation.Text = "Display Texture Orientations"
        '
        'tsFile
        '
        Me.tsFile.Dock = System.Windows.Forms.DockStyle.None
        Me.tsFile.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbSave})
        Me.tsFile.Location = New System.Drawing.Point(453, 2)
        Me.tsFile.Name = "tsFile"
        Me.tsFile.Size = New System.Drawing.Size(26, 25)
        Me.tsFile.TabIndex = 3
        '
        'tsbSave
        '
        Me.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSave.Enabled = False
        Me.tsbSave.Image = CType(resources.GetObject("tsbSave.Image"), System.Drawing.Image)
        Me.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSave.Name = "tsbSave"
        Me.tsbSave.Size = New System.Drawing.Size(23, 22)
        Me.tsbSave.Text = "Quick Save FME"
        '
        'tsSelection
        '
        Me.tsSelection.Dock = System.Windows.Forms.DockStyle.None
        Me.tsSelection.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsSelection.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.tsbSelection, Me.tsbSelectionCopy, Me.tsbSelectionPasteOptions, Me.tsbSelectionPaste, Me.tsbSelectionRotateAnticlockwise, Me.tsbSelectionRotateClockwise, Me.tsbSelectionFlipX, Me.tsbSelectionObjects})
        Me.tsSelection.Location = New System.Drawing.Point(98, 0)
        Me.tsSelection.Name = "tsSelection"
        Me.tsSelection.Size = New System.Drawing.Size(266, 25)
        Me.tsSelection.TabIndex = 0
        Me.tsSelection.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(73, 22)
        Me.ToolStripLabel1.Text = "Selection:"
        '
        'tsbSelection
        '
        Me.tsbSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelection.Image = CType(resources.GetObject("tsbSelection.Image"), System.Drawing.Image)
        Me.tsbSelection.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelection.Name = "tsbSelection"
        Me.tsbSelection.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelection.Text = "Selection Tool"
        '
        'tsbSelectionCopy
        '
        Me.tsbSelectionCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionCopy.Image = CType(resources.GetObject("tsbSelectionCopy.Image"), System.Drawing.Image)
        Me.tsbSelectionCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionCopy.Name = "tsbSelectionCopy"
        Me.tsbSelectionCopy.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionCopy.Text = "Copy Selection"
        '
        'tsbSelectionPasteOptions
        '
        Me.tsbSelectionPasteOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionPasteOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuRotateUnits, Me.ToolStripSeparator10, Me.menuSelPasteHeights, Me.menuSelPasteTextures, Me.menuSelPasteUnits, Me.menuSelPasteGateways, Me.menuSelPasteDeleteUnits, Me.menuSelPasteDeleteGateways})
        Me.tsbSelectionPasteOptions.Image = CType(resources.GetObject("tsbSelectionPasteOptions.Image"), System.Drawing.Image)
        Me.tsbSelectionPasteOptions.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionPasteOptions.Name = "tsbSelectionPasteOptions"
        Me.tsbSelectionPasteOptions.Size = New System.Drawing.Size(29, 22)
        Me.tsbSelectionPasteOptions.Text = "Paste Options"
        '
        'menuRotateUnits
        '
        Me.menuRotateUnits.CheckOnClick = True
        Me.menuRotateUnits.Name = "menuRotateUnits"
        Me.menuRotateUnits.Size = New System.Drawing.Size(244, 24)
        Me.menuRotateUnits.Text = "Rotate Units"
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(241, 6)
        '
        'menuSelPasteHeights
        '
        Me.menuSelPasteHeights.Checked = True
        Me.menuSelPasteHeights.CheckOnClick = True
        Me.menuSelPasteHeights.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuSelPasteHeights.Name = "menuSelPasteHeights"
        Me.menuSelPasteHeights.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteHeights.Text = "Paste Heights"
        '
        'menuSelPasteTextures
        '
        Me.menuSelPasteTextures.Checked = True
        Me.menuSelPasteTextures.CheckOnClick = True
        Me.menuSelPasteTextures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuSelPasteTextures.Name = "menuSelPasteTextures"
        Me.menuSelPasteTextures.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteTextures.Text = "Paste Textures"
        '
        'menuSelPasteUnits
        '
        Me.menuSelPasteUnits.CheckOnClick = True
        Me.menuSelPasteUnits.Name = "menuSelPasteUnits"
        Me.menuSelPasteUnits.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteUnits.Text = "Paste Units"
        '
        'menuSelPasteGateways
        '
        Me.menuSelPasteGateways.CheckOnClick = True
        Me.menuSelPasteGateways.Name = "menuSelPasteGateways"
        Me.menuSelPasteGateways.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteGateways.Text = "Paste Gateways"
        '
        'menuSelPasteDeleteUnits
        '
        Me.menuSelPasteDeleteUnits.CheckOnClick = True
        Me.menuSelPasteDeleteUnits.Name = "menuSelPasteDeleteUnits"
        Me.menuSelPasteDeleteUnits.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteDeleteUnits.Text = "Delete Existing Units"
        '
        'menuSelPasteDeleteGateways
        '
        Me.menuSelPasteDeleteGateways.CheckOnClick = True
        Me.menuSelPasteDeleteGateways.Name = "menuSelPasteDeleteGateways"
        Me.menuSelPasteDeleteGateways.Size = New System.Drawing.Size(244, 24)
        Me.menuSelPasteDeleteGateways.Text = "Delete Existing Gateways"
        '
        'tsbSelectionPaste
        '
        Me.tsbSelectionPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionPaste.Image = CType(resources.GetObject("tsbSelectionPaste.Image"), System.Drawing.Image)
        Me.tsbSelectionPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionPaste.Name = "tsbSelectionPaste"
        Me.tsbSelectionPaste.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionPaste.Text = "Paste To Selection"
        '
        'tsbSelectionRotateAnticlockwise
        '
        Me.tsbSelectionRotateAnticlockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionRotateAnticlockwise.Image = CType(resources.GetObject("tsbSelectionRotateAnticlockwise.Image"), System.Drawing.Image)
        Me.tsbSelectionRotateAnticlockwise.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionRotateAnticlockwise.Name = "tsbSelectionRotateAnticlockwise"
        Me.tsbSelectionRotateAnticlockwise.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionRotateAnticlockwise.Text = "Rotate Copy Anticlockwise"
        '
        'tsbSelectionRotateClockwise
        '
        Me.tsbSelectionRotateClockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionRotateClockwise.Image = CType(resources.GetObject("tsbSelectionRotateClockwise.Image"), System.Drawing.Image)
        Me.tsbSelectionRotateClockwise.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionRotateClockwise.Name = "tsbSelectionRotateClockwise"
        Me.tsbSelectionRotateClockwise.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionRotateClockwise.Text = "Rotate Copy Clockwise"
        '
        'tsbSelectionFlipX
        '
        Me.tsbSelectionFlipX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionFlipX.Image = CType(resources.GetObject("tsbSelectionFlipX.Image"), System.Drawing.Image)
        Me.tsbSelectionFlipX.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionFlipX.Name = "tsbSelectionFlipX"
        Me.tsbSelectionFlipX.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionFlipX.Text = "Flip Copy Horizontally"
        '
        'tsbSelectionObjects
        '
        Me.tsbSelectionObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionObjects.Image = CType(resources.GetObject("tsbSelectionObjects.Image"), System.Drawing.Image)
        Me.tsbSelectionObjects.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionObjects.Name = "tsbSelectionObjects"
        Me.tsbSelectionObjects.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionObjects.Text = "Select Units"
        '
        'tsMinimap
        '
        Me.tsMinimap.Dock = System.Windows.Forms.DockStyle.None
        Me.tsMinimap.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.tsMinimap.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton1})
        Me.tsMinimap.Location = New System.Drawing.Point(0, 0)
        Me.tsMinimap.Name = "tsMinimap"
        Me.tsMinimap.Size = New System.Drawing.Size(84, 27)
        Me.tsMinimap.TabIndex = 1
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuMiniShowTex, Me.menuMiniShowHeight, Me.menuMiniShowCliffs, Me.menuMiniShowUnits, Me.menuMiniShowGateways, Me.menuMinimapSize})
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(81, 24)
        Me.ToolStripDropDownButton1.Text = "Minimap"
        '
        'menuMiniShowTex
        '
        Me.menuMiniShowTex.Checked = True
        Me.menuMiniShowTex.CheckOnClick = True
        Me.menuMiniShowTex.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuMiniShowTex.Name = "menuMiniShowTex"
        Me.menuMiniShowTex.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowTex.Text = "Show Textures"
        '
        'menuMiniShowHeight
        '
        Me.menuMiniShowHeight.Checked = True
        Me.menuMiniShowHeight.CheckOnClick = True
        Me.menuMiniShowHeight.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuMiniShowHeight.Name = "menuMiniShowHeight"
        Me.menuMiniShowHeight.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowHeight.Text = "Show Heights"
        '
        'menuMiniShowUnits
        '
        Me.menuMiniShowUnits.Checked = True
        Me.menuMiniShowUnits.CheckOnClick = True
        Me.menuMiniShowUnits.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuMiniShowUnits.Name = "menuMiniShowUnits"
        Me.menuMiniShowUnits.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowUnits.Text = "Show Objects"
        '
        'menuMiniShowGateways
        '
        Me.menuMiniShowGateways.CheckOnClick = True
        Me.menuMiniShowGateways.Name = "menuMiniShowGateways"
        Me.menuMiniShowGateways.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowGateways.Text = "Show Gateways"
        '
        'menuMinimapSize
        '
        Me.menuMinimapSize.Items.AddRange(New Object() {"64", "96", "128", "160", "192", "256", "384"})
        Me.menuMinimapSize.Name = "menuMinimapSize"
        Me.menuMinimapSize.Size = New System.Drawing.Size(121, 28)
        Me.menuMinimapSize.Text = "160"
        '
        'pnlView
        '
        Me.pnlView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlView.Location = New System.Drawing.Point(0, 30)
        Me.pnlView.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlView.Name = "pnlView"
        Me.pnlView.Size = New System.Drawing.Size(858, 582)
        Me.pnlView.TabIndex = 1
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.FileName = "OpenFileDialog1"
        '
        'menuMain
        '
        Me.menuMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.menuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuFile, Me.OptionsToolStripMenuItem})
        Me.menuMain.Location = New System.Drawing.Point(0, 0)
        Me.menuMain.Name = "menuMain"
        Me.menuMain.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
        Me.menuMain.Size = New System.Drawing.Size(1296, 31)
        Me.menuMain.TabIndex = 0
        Me.menuMain.Text = "MenuStrip1"
        '
        'menuFile
        '
        Me.menuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewMapToolStripMenuItem, Me.ToolStripSeparator3, Me.OpenToolStripMenuItem, Me.ToolStripSeparator2, Me.SaveToolStripMenuItem, Me.ToolStripSeparator1, Me.ToolStripMenuItem4, Me.ToolStripMenuItem2, Me.MapWZToolStripMenuItem, Me.ToolStripSeparator4, Me.CloseToolStripMenuItem})
        Me.menuFile.Name = "menuFile"
        Me.menuFile.Size = New System.Drawing.Size(44, 27)
        Me.menuFile.Text = "File"
        '
        'NewMapToolStripMenuItem
        '
        Me.NewMapToolStripMenuItem.Name = "NewMapToolStripMenuItem"
        Me.NewMapToolStripMenuItem.Size = New System.Drawing.Size(177, 24)
        Me.NewMapToolStripMenuItem.Text = "New Map"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(174, 6)
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(177, 24)
        Me.OpenToolStripMenuItem.Text = "Open..."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(174, 6)
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MapFMEToolStripMenuItem, Me.ToolStripSeparator5, Me.MapLNDToolStripMenuItem, Me.ToolStripSeparator6, Me.menuExportMapTileTypes, Me.ToolStripMenuItem1, Me.MinimapBMPToolStripMenuItem, Me.ToolStripMenuItem3})
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(177, 24)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'MapFMEToolStripMenuItem
        '
        Me.MapFMEToolStripMenuItem.Name = "MapFMEToolStripMenuItem"
        Me.MapFMEToolStripMenuItem.Size = New System.Drawing.Size(214, 24)
        Me.MapFMEToolStripMenuItem.Text = "Map..."
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(211, 6)
        '
        'MapLNDToolStripMenuItem
        '
        Me.MapLNDToolStripMenuItem.Name = "MapLNDToolStripMenuItem"
        Me.MapLNDToolStripMenuItem.Size = New System.Drawing.Size(214, 24)
        Me.MapLNDToolStripMenuItem.Text = "Export Map LND..."
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(211, 6)
        '
        'menuExportMapTileTypes
        '
        Me.menuExportMapTileTypes.Name = "menuExportMapTileTypes"
        Me.menuExportMapTileTypes.Size = New System.Drawing.Size(214, 24)
        Me.menuExportMapTileTypes.Text = "Export Tile Types..."
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(211, 6)
        '
        'MinimapBMPToolStripMenuItem
        '
        Me.MinimapBMPToolStripMenuItem.Name = "MinimapBMPToolStripMenuItem"
        Me.MinimapBMPToolStripMenuItem.Size = New System.Drawing.Size(214, 24)
        Me.MinimapBMPToolStripMenuItem.Text = "Minimap Bitmap..."
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(214, 24)
        Me.ToolStripMenuItem3.Text = "Heightmap Bitmap..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(174, 6)
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ImportHeightmapToolStripMenuItem, Me.ToolStripSeparator7, Me.menuImportTileTypes, Me.ToolStripSeparator9, Me.menuImportMapCopy})
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(177, 24)
        Me.ToolStripMenuItem4.Text = "Import"
        '
        'ImportHeightmapToolStripMenuItem
        '
        Me.ImportHeightmapToolStripMenuItem.Name = "ImportHeightmapToolStripMenuItem"
        Me.ImportHeightmapToolStripMenuItem.Size = New System.Drawing.Size(250, 24)
        Me.ImportHeightmapToolStripMenuItem.Text = "Heightmap..."
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(247, 6)
        '
        'menuImportTileTypes
        '
        Me.menuImportTileTypes.Name = "menuImportTileTypes"
        Me.menuImportTileTypes.Size = New System.Drawing.Size(250, 24)
        Me.menuImportTileTypes.Text = "Tile Types..."
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(247, 6)
        '
        'menuImportMapCopy
        '
        Me.menuImportMapCopy.Name = "menuImportMapCopy"
        Me.menuImportMapCopy.Size = New System.Drawing.Size(250, 24)
        Me.menuImportMapCopy.Text = "Map as Copied Segment..."
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(174, 6)
        '
        'MapWZToolStripMenuItem
        '
        Me.MapWZToolStripMenuItem.Name = "MapWZToolStripMenuItem"
        Me.MapWZToolStripMenuItem.Size = New System.Drawing.Size(177, 24)
        Me.MapWZToolStripMenuItem.Text = "Compile Map..."
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(174, 6)
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(177, 24)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UndoLimitToolStripMenuItem, Me.AutosaveToolStripMenuItem, Me.CursorModeToolStripMenuItem, Me.DisplayToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(73, 27)
        Me.OptionsToolStripMenuItem.Text = "Options"
        '
        'UndoLimitToolStripMenuItem
        '
        Me.UndoLimitToolStripMenuItem.Name = "UndoLimitToolStripMenuItem"
        Me.UndoLimitToolStripMenuItem.Size = New System.Drawing.Size(168, 24)
        Me.UndoLimitToolStripMenuItem.Text = "Undo Limit..."
        '
        'AutosaveToolStripMenuItem
        '
        Me.AutosaveToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuAutosaveEnabled, Me.menuAutosaveInterval, Me.menuAutosaveChanges, Me.ToolStripSeparator8, Me.menuAutosaveOpen})
        Me.AutosaveToolStripMenuItem.Name = "AutosaveToolStripMenuItem"
        Me.AutosaveToolStripMenuItem.Size = New System.Drawing.Size(168, 24)
        Me.AutosaveToolStripMenuItem.Text = "Autosave"
        '
        'menuAutosaveEnabled
        '
        Me.menuAutosaveEnabled.Checked = True
        Me.menuAutosaveEnabled.CheckOnClick = True
        Me.menuAutosaveEnabled.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuAutosaveEnabled.Name = "menuAutosaveEnabled"
        Me.menuAutosaveEnabled.Size = New System.Drawing.Size(210, 24)
        Me.menuAutosaveEnabled.Text = "Enabled"
        '
        'menuAutosaveInterval
        '
        Me.menuAutosaveInterval.Name = "menuAutosaveInterval"
        Me.menuAutosaveInterval.Size = New System.Drawing.Size(210, 24)
        Me.menuAutosaveInterval.Text = "Minimum Interval..."
        '
        'menuAutosaveChanges
        '
        Me.menuAutosaveChanges.Name = "menuAutosaveChanges"
        Me.menuAutosaveChanges.Size = New System.Drawing.Size(210, 24)
        Me.menuAutosaveChanges.Text = "Minimum Changes..."
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(207, 6)
        '
        'menuAutosaveOpen
        '
        Me.menuAutosaveOpen.Name = "menuAutosaveOpen"
        Me.menuAutosaveOpen.Size = New System.Drawing.Size(210, 24)
        Me.menuAutosaveOpen.Text = "Open Autosave..."
        '
        'CursorModeToolStripMenuItem
        '
        Me.CursorModeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuPointerPlane, Me.menuPointerDirect})
        Me.CursorModeToolStripMenuItem.Name = "CursorModeToolStripMenuItem"
        Me.CursorModeToolStripMenuItem.Size = New System.Drawing.Size(168, 24)
        Me.CursorModeToolStripMenuItem.Text = "Pointer Mode"
        '
        'menuPointerPlane
        '
        Me.menuPointerPlane.Name = "menuPointerPlane"
        Me.menuPointerPlane.Size = New System.Drawing.Size(118, 24)
        Me.menuPointerPlane.Text = "Plane"
        '
        'menuPointerDirect
        '
        Me.menuPointerDirect.Name = "menuPointerDirect"
        Me.menuPointerDirect.Size = New System.Drawing.Size(118, 24)
        Me.menuPointerDirect.Text = "Direct"
        '
        'DisplayToolStripMenuItem
        '
        Me.DisplayToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuFont})
        Me.DisplayToolStripMenuItem.Name = "DisplayToolStripMenuItem"
        Me.DisplayToolStripMenuItem.Size = New System.Drawing.Size(168, 24)
        Me.DisplayToolStripMenuItem.Text = "Display"
        '
        'menuFont
        '
        Me.menuFont.Name = "menuFont"
        Me.menuFont.Size = New System.Drawing.Size(116, 24)
        Me.menuFont.Text = "Font..."
        '
        'tmrTool
        '
        Me.tmrTool.Enabled = True
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 1
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.menuMain, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.SplitContainer1, 0, 1)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel5.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 2
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(1296, 655)
        Me.TableLayoutPanel5.TabIndex = 1
        '
        'TabPage13
        '
        Me.TabPage13.Location = New System.Drawing.Point(4, 24)
        Me.TabPage13.Name = "TabPage13"
        Me.TabPage13.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage13.Size = New System.Drawing.Size(193, 0)
        Me.TabPage13.TabIndex = 0
        Me.TabPage13.Text = "1"
        Me.TabPage13.UseVisualStyleBackColor = True
        '
        'TabPage14
        '
        Me.TabPage14.Location = New System.Drawing.Point(4, 24)
        Me.TabPage14.Name = "TabPage14"
        Me.TabPage14.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage14.Size = New System.Drawing.Size(193, 0)
        Me.TabPage14.TabIndex = 1
        Me.TabPage14.Text = "2"
        Me.TabPage14.UseVisualStyleBackColor = True
        '
        'TabPage15
        '
        Me.TabPage15.Location = New System.Drawing.Point(4, 24)
        Me.TabPage15.Name = "TabPage15"
        Me.TabPage15.Size = New System.Drawing.Size(193, 0)
        Me.TabPage15.TabIndex = 2
        Me.TabPage15.Text = "3"
        Me.TabPage15.UseVisualStyleBackColor = True
        '
        'TabPage16
        '
        Me.TabPage16.Location = New System.Drawing.Point(4, 24)
        Me.TabPage16.Name = "TabPage16"
        Me.TabPage16.Size = New System.Drawing.Size(193, 0)
        Me.TabPage16.TabIndex = 3
        Me.TabPage16.Text = "4"
        Me.TabPage16.UseVisualStyleBackColor = True
        '
        'TabPage21
        '
        Me.TabPage21.Location = New System.Drawing.Point(4, 24)
        Me.TabPage21.Name = "TabPage21"
        Me.TabPage21.Size = New System.Drawing.Size(193, 0)
        Me.TabPage21.TabIndex = 4
        Me.TabPage21.Text = "5"
        Me.TabPage21.UseVisualStyleBackColor = True
        '
        'TabPage22
        '
        Me.TabPage22.Location = New System.Drawing.Point(4, 24)
        Me.TabPage22.Name = "TabPage22"
        Me.TabPage22.Size = New System.Drawing.Size(193, 0)
        Me.TabPage22.TabIndex = 5
        Me.TabPage22.Text = "6"
        Me.TabPage22.UseVisualStyleBackColor = True
        '
        'TabPage23
        '
        Me.TabPage23.Location = New System.Drawing.Point(4, 24)
        Me.TabPage23.Name = "TabPage23"
        Me.TabPage23.Size = New System.Drawing.Size(193, 0)
        Me.TabPage23.TabIndex = 6
        Me.TabPage23.Text = "7"
        Me.TabPage23.UseVisualStyleBackColor = True
        '
        'TabPage24
        '
        Me.TabPage24.Location = New System.Drawing.Point(4, 24)
        Me.TabPage24.Name = "TabPage24"
        Me.TabPage24.Size = New System.Drawing.Size(193, 0)
        Me.TabPage24.TabIndex = 7
        Me.TabPage24.Text = "8"
        Me.TabPage24.UseVisualStyleBackColor = True
        '
        'FontDialog
        '
        Me.FontDialog.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'ilInterface
        '
        Me.ilInterface.ImageStream = CType(resources.GetObject("ilInterface.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ilInterface.TransparentColor = System.Drawing.Color.Transparent
        Me.ilInterface.Images.SetKeyName(0, "displayautotexture.png")
        Me.ilInterface.Images.SetKeyName(1, "drawtileorientation.png")
        Me.ilInterface.Images.SetKeyName(2, "gateways.png")
        Me.ilInterface.Images.SetKeyName(3, "objectsselect.png")
        Me.ilInterface.Images.SetKeyName(4, "save.png")
        Me.ilInterface.Images.SetKeyName(5, "selection.png")
        Me.ilInterface.Images.SetKeyName(6, "selectioncopy.png")
        Me.ilInterface.Images.SetKeyName(7, "selectionflipx.png")
        Me.ilInterface.Images.SetKeyName(8, "selectionpaste.png")
        Me.ilInterface.Images.SetKeyName(9, "selectionpasteoptions.png")
        Me.ilInterface.Images.SetKeyName(10, "selectionrotateanticlockwise.png")
        Me.ilInterface.Images.SetKeyName(11, "selectionrotateclockwise.png")
        '
        'menuMiniShowCliffs
        '
        Me.menuMiniShowCliffs.CheckOnClick = True
        Me.menuMiniShowCliffs.Name = "menuMiniShowCliffs"
        Me.menuMiniShowCliffs.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowCliffs.Text = "Show Cliffs"
        '
        'frmMain
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1296, 655)
        Me.Controls.Add(Me.TableLayoutPanel5)
        Me.MainMenuStrip = Me.menuMain
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmMain"
        Me.Text = "flaME"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabControl.ResumeLayout(False)
        Me.tpTextures.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.tabTextureBrushShape.ResumeLayout(False)
        CType(Me.nudTextureBrushRadius, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel6.ResumeLayout(False)
        Me.tpAutoTexture.ResumeLayout(False)
        Me.tpAutoTexture.PerformLayout()
        Me.tabAutoTextureBrushShape.ResumeLayout(False)
        CType(Me.nudAutoCliffBrushRadius, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudAutoTextureRadius, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpHeight.ResumeLayout(False)
        Me.tpHeight.PerformLayout()
        Me.tabHeightBrushShape.ResumeLayout(False)
        Me.tabHeightSetR.ResumeLayout(False)
        Me.tabHeightSetL.ResumeLayout(False)
        CType(Me.nudHeightBrushRadius, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpAutoHeight.ResumeLayout(False)
        Me.tpResize.ResumeLayout(False)
        Me.tpResize.PerformLayout()
        Me.tpObjects.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.tpObject.ResumeLayout(False)
        Me.tpObject.PerformLayout()
        Me.TableLayoutPanel7.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.tsTools.ResumeLayout(False)
        Me.tsTools.PerformLayout()
        Me.tsFile.ResumeLayout(False)
        Me.tsFile.PerformLayout()
        Me.tsSelection.ResumeLayout(False)
        Me.tsSelection.PerformLayout()
        Me.tsMinimap.ResumeLayout(False)
        Me.tsMinimap.PerformLayout()
        Me.menuMain.ResumeLayout(False)
        Me.menuMain.PerformLayout()
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel5.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrKey As System.Windows.Forms.Timer
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents tpTextures As System.Windows.Forms.TabPage
    Friend WithEvents tpHeight As System.Windows.Forms.TabPage
    Friend WithEvents tpAutoTexture As System.Windows.Forms.TabPage
    Friend WithEvents lstAutoTexture As System.Windows.Forms.ListBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbTileset As System.Windows.Forms.ComboBox
    Friend WithEvents txtAutoCliffSlope As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tpAutoHeight As System.Windows.Forms.TabPage
    Friend WithEvents btnAutoTri As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtHeightSetL As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtSmoothRate As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents rdoHeightSmooth As System.Windows.Forms.RadioButton
    Friend WithEvents rdoHeightSet As System.Windows.Forms.RadioButton
    Friend WithEvents txtSmoothRadius As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnMultiplier As System.Windows.Forms.Button
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtMultiplier As System.Windows.Forms.TextBox
    Friend WithEvents tmrTool As System.Windows.Forms.Timer
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents tpResize As System.Windows.Forms.TabPage
    Friend WithEvents txtOffsetY As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtOffsetX As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtSizeY As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtSizeX As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents btnResize As System.Windows.Forms.Button
    Friend WithEvents nudTextureBrushRadius As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudHeightBrushRadius As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudAutoTextureRadius As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents rdoAutoCliffRemove As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAutoCliffBrush As System.Windows.Forms.RadioButton
    Friend WithEvents nudAutoCliffBrushRadius As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents HeightmapBMPToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents rdoAutoTextureFill As System.Windows.Forms.RadioButton
    Friend WithEvents txtHeightChangeRate As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents rdoHeightChange As System.Windows.Forms.RadioButton
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtHeightOffset As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtHeightMultiply As System.Windows.Forms.TextBox
    Friend WithEvents rdoAutoTexturePlace As System.Windows.Forms.RadioButton
    Friend WithEvents rdoAutoRoadPlace As System.Windows.Forms.RadioButton
    Friend WithEvents lstAutoRoad As System.Windows.Forms.ListBox
    Friend WithEvents btnAutoRoadRemove As System.Windows.Forms.Button
    Friend WithEvents btnAutoTextureRemove As System.Windows.Forms.Button
    Friend WithEvents menuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents menuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewMapToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MapLNDToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MapFMEToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MinimapBMPToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tpObjects As System.Windows.Forms.TabPage
    Friend WithEvents lstDroids As System.Windows.Forms.ListBox
    Friend WithEvents lstFeatures As System.Windows.Forms.ListBox
    Friend WithEvents lstStructures As System.Windows.Forms.ListBox
    Friend WithEvents txtPlayerNum As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents tpObject As System.Windows.Forms.TabPage
    Friend WithEvents lblObjectType As System.Windows.Forms.Label
    Friend WithEvents txtObjectName As System.Windows.Forms.TextBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents txtObjesdss As System.Windows.Forms.Label
    Friend WithEvents txtObjectRotation As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents txtObjectPlayer As System.Windows.Forms.TextBox
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents rdoAutoRoadLine As System.Windows.Forms.RadioButton
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents btnMapTexturer As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents MapWZToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UndoLimitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnSelResize As System.Windows.Forms.Button
    Friend WithEvents tsSelection As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripLabel1 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tsbSelection As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbSelectionCopy As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbSelectionPasteOptions As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents menuSelPasteHeights As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuSelPasteTextures As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuSelPasteUnits As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuSelPasteDeleteUnits As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsbSelectionPaste As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbSelectionRotateAnticlockwise As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbSelectionRotateClockwise As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsMinimap As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents menuMiniShowTex As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuMiniShowHeight As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuMiniShowUnits As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuMinimapSize As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents menuMiniShowGateways As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsTools As System.Windows.Forms.ToolStrip
    Friend WithEvents tsbGateways As System.Windows.Forms.ToolStripButton
    Friend WithEvents TableLayoutPanel6 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents cmbTileType As System.Windows.Forms.ComboBox
    Friend WithEvents chkTileTypes As System.Windows.Forms.CheckBox
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menuExportMapTileTypes As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImportHeightmapToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuImportTileTypes As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents chkCliffTris As System.Windows.Forms.CheckBox
    Friend WithEvents AutosaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuAutosaveInterval As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuAutosaveChanges As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuAutosaveEnabled As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menuAutosaveOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsFile As System.Windows.Forms.ToolStrip
    Friend WithEvents tsbSave As System.Windows.Forms.ToolStripButton
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents chkAutoTexSetHeight As System.Windows.Forms.CheckBox
    Friend WithEvents txtObjectID As System.Windows.Forms.TextBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel7 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents pnlView As System.Windows.Forms.Panel
    Friend WithEvents btnReinterpretTerrain As System.Windows.Forms.Button
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents txtHeightSetR As System.Windows.Forms.TextBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents tabHeightSetL As System.Windows.Forms.TabControl
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage11 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage12 As System.Windows.Forms.TabPage
    Friend WithEvents tabHeightSetR As System.Windows.Forms.TabControl
    Friend WithEvents TabPage25 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage26 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage27 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage28 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage29 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage30 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage31 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage32 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage17 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage18 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage19 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage20 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage13 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage14 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage15 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage16 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage21 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage22 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage23 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage24 As System.Windows.Forms.TabPage
    Friend WithEvents tabHeightBrushShape As System.Windows.Forms.TabControl
    Friend WithEvents TabPage33 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage34 As System.Windows.Forms.TabPage
    Friend WithEvents tabAutoTextureBrushShape As System.Windows.Forms.TabControl
    Friend WithEvents TabPage35 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage36 As System.Windows.Forms.TabPage
    Friend WithEvents tabTextureBrushShape As System.Windows.Forms.TabControl
    Friend WithEvents TabPage37 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage38 As System.Windows.Forms.TabPage
    Private WithEvents TabControl As System.Windows.Forms.TabControl
    Friend WithEvents tsbSelectionObjects As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menuImportMapCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsbDrawAutotexture As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnWaterTri As System.Windows.Forms.Button
    Friend WithEvents tsbSelectionFlipX As System.Windows.Forms.ToolStripButton
    Friend WithEvents CursorModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuPointerPlane As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuPointerDirect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnHeightOffsetSelection As System.Windows.Forms.Button
    Friend WithEvents btnHeightsMultiplySelection As System.Windows.Forms.Button
    Friend WithEvents tsbDrawTileOrientation As System.Windows.Forms.ToolStripButton
    Friend WithEvents chkTextureOrientationRandomize As System.Windows.Forms.CheckBox
    Friend WithEvents btnTextureFlipX As System.Windows.Forms.Button
    Friend WithEvents btnTextureClockwise As System.Windows.Forms.Button
    Friend WithEvents btnTextureAnticlockwise As System.Windows.Forms.Button
    Friend WithEvents chkSetTextureOrientation As System.Windows.Forms.CheckBox
    Friend WithEvents chkSetTexture As System.Windows.Forms.CheckBox
    Friend WithEvents DisplayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuFont As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FontDialog As System.Windows.Forms.FontDialog
    Friend WithEvents chkTileNumbers As System.Windows.Forms.CheckBox
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents txtObjectPriority As System.Windows.Forms.TextBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents menuRotateUnits As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents menuSelPasteGateways As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuSelPasteDeleteGateways As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ilInterface As System.Windows.Forms.ImageList
    Friend WithEvents btnGenerator As System.Windows.Forms.Button
    Friend WithEvents menuMiniShowCliffs As System.Windows.Forms.ToolStripMenuItem
End Class
