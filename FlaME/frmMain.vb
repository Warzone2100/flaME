Imports ICSharpCode.SharpZipLib
Imports System.Security.Permissions
Imports OpenTK.Graphics.OpenGL

Partial Public Class frmMain
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Public Class clsMaps
        Inherits ConnectedList(Of clsMap, frmMain)

        Private _MainMap As clsMap

        Public Overrides Sub Add(ByVal NewItem As ConnectedListItem(Of clsMap, frmMain))

            Dim NewMap As clsMap = NewItem.Item

            If Not NewMap.ReadyForUserInput Then
                NewMap.InitializeUserInput()
            End If

            NewMap.MapView_TabPage = New TabPage
            NewMap.MapView_TabPage.Tag = NewMap

            NewMap.SetTabText()

            MyBase.Add(NewItem)

            Owner.MapView.UpdateTabs()
        End Sub

        Public Overrides Sub Remove(ByVal Position As Integer)

            Dim Map As clsMap = Item(Position)

            MyBase.Remove(Position)

            If Map Is _MainMap Then
                Dim NewNum As Integer = Math.Min(Owner.MapView.tabMaps.SelectedIndex, ItemCount - 1)
                If NewNum < 0 Then
                    MainMap = Nothing
                Else
                    MainMap = Item(NewNum)
                End If
            End If

            Map.MapView_TabPage.Tag = Nothing
            Map.MapView_TabPage = Nothing

            Owner.MapView.UpdateTabs()
        End Sub

        Public Property MainMap As clsMap
            Get
                Return _MainMap
            End Get
            Set(value As clsMap)
                If value Is _MainMap Then
                    Exit Property
                End If
                Dim MainForm As frmMain = Owner
                MainForm.MainMapBeforeChanged()
                If value Is Nothing Then
                    _MainMap = Nothing
                Else
                    If value.frmMainLink.Source IsNot Owner Then
                        MsgBox("Error: Assigning map to wrong main form.")
                        _MainMap = Nothing
                    Else
                        _MainMap = value
                    End If
                End If
                MainForm.MainMapAfterChanged()
            End Set
        End Property

        Public Sub New(ByVal Owner As frmMain)
            MyBase.New(Owner)

            MaintainOrder = True
        End Sub
    End Class
    Private _LoadedMaps As New clsMaps(Me)

    Public MapView As ctrlMapView
    Public TextureView As ctrlTextureView

    Public lstFeatures_Objects() As clsUnitType
    Public lstStructures_Objects() As clsUnitType
    Public lstDroids_Objects() As clsUnitType

    Public cboBody_Objects() As clsBody
    Public cboPropulsion_Objects() As clsPropulsion
    Public cboTurret_Objects() As clsTurret

    Public HeightSetPalette(7) As Byte

    Public SelectedObjectType As clsUnitType

    Public TextureTerrainAction As enumTextureTerrainAction = enumTextureTerrainAction.Reinterpret

    Public FillCliffAction As enumFillCliffAction = enumFillCliffAction.Ignore

    Public WithEvents tmrKey As Timer
    Public WithEvents tmrTool As Timer

    Public WithEvents NewPlayerNum As ctrlPlayerNum
    Public WithEvents ObjectPlayerNum As ctrlPlayerNum

    Public WithEvents ctrlTextureBrush As ctrlBrush
    Public WithEvents ctrlTerrainBrush As ctrlBrush
    Public WithEvents ctrlCliffRemoveBrush As ctrlBrush
    Public WithEvents ctrlHeightBrush As ctrlBrush

    Public Sub New()
        InitializeComponent() 'required for monodevelop too

        PlatformPathSeparator = IO.Path.DirectorySeparatorChar

        'these depend on ospathseperator
        SetProgramSubDirs()
        SetDataSubDirs()

        tmrKey = New Timer
        tmrKey.Interval = 30

        tmrTool = New Timer
        tmrTool.Interval = 100

        NewPlayerNum = New ctrlPlayerNum
        ObjectPlayerNum = New ctrlPlayerNum

        MapView = New ctrlMapView(Me)
        TextureView = New ctrlTextureView(Me)

        frmGeneratorInstance = New frmGenerator(Me)
    End Sub

    Private InterfaceImage_DisplayAutoTexture As Bitmap
    Private InterfaceImage_DrawTileOrientation As Bitmap
    Private InterfaceImage_QuickSave As Bitmap
    Private InterfaceImage_Selection As Bitmap
    Private InterfaceImage_ObjectSelect As Bitmap
    Private InterfaceImage_SelectionCopy As Bitmap
    Private InterfaceImage_SelectionFlipX As Bitmap
    Private InterfaceImage_SelectionRotateClockwise As Bitmap
    Private InterfaceImage_SelectionRotateCounterClockwise As Bitmap
    Private InterfaceImage_SelectionPaste As Bitmap
    Private InterfaceImage_SelectionPasteOptions As Bitmap
    Private InterfaceImage_Gateways As Bitmap

    Private Function LoadInterfaceIcons() As clsResult
        Dim ReturnResult As New clsResult

        LoadInterfaceImage(InterfaceImagesPath & "displayautotexture.png", InterfaceImage_DisplayAutoTexture, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "drawtileorientation.png", InterfaceImage_DrawTileOrientation, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "save.png", InterfaceImage_QuickSave, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selection.png", InterfaceImage_Selection, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "objectsselect.png", InterfaceImage_ObjectSelect, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectioncopy.png", InterfaceImage_SelectionCopy, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectionflipx.png", InterfaceImage_SelectionFlipX, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectionrotateclockwise.png", InterfaceImage_SelectionRotateClockwise, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectionrotateanticlockwise.png", InterfaceImage_SelectionRotateCounterClockwise, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectionpaste.png", InterfaceImage_SelectionPaste, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "selectionpasteoptions.png", InterfaceImage_SelectionPasteOptions, ReturnResult)
        LoadInterfaceImage(InterfaceImagesPath & "gateways.png", InterfaceImage_Gateways, ReturnResult)

        tsbDrawAutotexture.Image = InterfaceImage_DisplayAutoTexture
        tsbDrawTileOrientation.Image = InterfaceImage_DrawTileOrientation
        tsbSave.Image = InterfaceImage_QuickSave
        tsbSelection.Image = InterfaceImage_Selection
        tsbSelectionObjects.Image = InterfaceImage_ObjectSelect
        tsbSelectionCopy.Image = InterfaceImage_SelectionCopy
        tsbSelectionFlipX.Image = InterfaceImage_SelectionFlipX
        tsbSelectionRotateClockwise.Image = InterfaceImage_SelectionRotateClockwise
        tsbSelectionRotateCounterClockwise.Image = InterfaceImage_SelectionRotateCounterClockwise
        tsbSelectionPaste.Image = InterfaceImage_SelectionPaste
        tsbSelectionPasteOptions.Image = InterfaceImage_SelectionPasteOptions
        tsbGateways.Image = InterfaceImage_Gateways
        btnTextureAnticlockwise.Image = InterfaceImage_SelectionRotateCounterClockwise
        btnTextureClockwise.Image = InterfaceImage_SelectionRotateClockwise
        btnTextureFlipX.Image = InterfaceImage_SelectionFlipX

        Return ReturnResult
    End Function

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim A As Integer

        For A = 0 To _LoadedMaps.ItemCount - 1
            If _LoadedMaps.Item(A).ChangedSinceSave Then
                Exit For
            End If
        Next
        If A < _LoadedMaps.ItemCount Then
            If MsgBox("Lose any unsaved changes to open maps?", CType(MsgBoxStyle.Question + MsgBoxStyle.OkCancel, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        Dim Result As clsResult = Settings_Write()
    End Sub

#If Mono = 0.0# Then
    Public Class clsSplashScreen

        Public Form As New frmSplash

        Public Sub New()

            Form.Icon = ProgramIcon
        End Sub
    End Class

    Private Sub ShowThreadedSplashScreen()
        Dim SplashScreen As New clsSplashScreen

        SplashScreen.Form.Show()
        SplashScreen.Form.Activate()
        Do While Not ProgramInitializeFinished
            Application.DoEvents()
            Threading.Thread.Sleep(200)
        Loop
        SplashScreen.Form.Close()
    End Sub
#End If

    Public Sub Initialize(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If ProgramInitialized Then
            Stop
            Exit Sub
        End If
        If Not (MapView.IsGLInitialized And TextureView.IsGLInitialized) Then
            Exit Sub
        End If

#If Mono = 0.0# Then
        Dim SplashThread As New Threading.Thread(AddressOf ShowThreadedSplashScreen)
        SplashThread.IsBackground = True
        SplashThread.Start()
#End If

        Dim A As Integer

        ProgramInitialized = True

        InitializeDelay.Enabled = False
        RemoveHandler InitializeDelay.Tick, AddressOf Initialize
        InitializeDelay.Dispose()
        InitializeDelay = Nothing

        InitializeResult.AppendAsWarning(LoadInterfaceIcons(), "Interface icons:")

        Matrix3D.MatrixSetToPY(SunAngleMatrix, New Matrix3D.AnglePY(-22.5# * RadOf1Deg, 157.5# * RadOf1Deg))

        NewPlayerNum.Left = 112
        NewPlayerNum.Top = 10
        Panel1.Controls.Add(NewPlayerNum)

        ObjectPlayerNum.Left = 72
        ObjectPlayerNum.Top = 60
        ObjectPlayerNum.Target = New clsMap.clsUnitGroupContainer
        AddHandler ObjectPlayerNum.Target.Changed, AddressOf tabPlayerNum_SelectedIndexChanged
        Panel14.Controls.Add(ObjectPlayerNum)

        ctrlTextureBrush = New ctrlBrush(TextureBrush)
        pnlTextureBrush.Controls.Add(ctrlTextureBrush)

        ctrlTerrainBrush = New ctrlBrush(TerrainBrush)
        pnlTerrainBrush.Controls.Add(ctrlTerrainBrush)

        ctrlCliffRemoveBrush = New ctrlBrush(CliffBrush)
        pnlCliffRemoveBrush.Controls.Add(ctrlCliffRemoveBrush)

        ctrlHeightBrush = New ctrlBrush(HeightBrush)
        pnlHeightSetBrush.Controls.Add(ctrlHeightBrush)

        Randomize()

        CreateControls()

        CreateTileTypes()

        For A = 0 To 15
            PlayerColour(A) = New clsPlayer
        Next
        PlayerColour(0).Colour.Red = 0.0F
        PlayerColour(0).Colour.Green = 96.0F / 255.0F
        PlayerColour(0).Colour.Blue = 0.0F
        PlayerColour(1).Colour.Red = 160.0F / 255.0F
        PlayerColour(1).Colour.Green = 112.0F / 255.0F
        PlayerColour(1).Colour.Blue = 0.0F
        PlayerColour(2).Colour.Red = 128.0F / 255.0F
        PlayerColour(2).Colour.Green = 128.0F / 255.0F
        PlayerColour(2).Colour.Blue = 128.0F / 255.0F
        PlayerColour(3).Colour.Red = 0.0F
        PlayerColour(3).Colour.Green = 0.0F
        PlayerColour(3).Colour.Blue = 0.0F
        PlayerColour(4).Colour.Red = 128.0F / 255.0F
        PlayerColour(4).Colour.Green = 0.0F
        PlayerColour(4).Colour.Blue = 0.0F
        PlayerColour(5).Colour.Red = 32.0F / 255.0F
        PlayerColour(5).Colour.Green = 48.0F / 255.0F
        PlayerColour(5).Colour.Blue = 96.0F / 255.0F
        PlayerColour(6).Colour.Red = 144.0F / 255.0F
        PlayerColour(6).Colour.Green = 0.0F
        PlayerColour(6).Colour.Blue = 112 / 255.0F
        PlayerColour(7).Colour.Red = 0.0F
        PlayerColour(7).Colour.Green = 128.0F / 255.0F
        PlayerColour(7).Colour.Blue = 128.0F / 255.0F
        PlayerColour(8).Colour.Red = 128.0F / 255.0F
        PlayerColour(8).Colour.Green = 192.0F / 255.0F
        PlayerColour(8).Colour.Blue = 0.0F
        PlayerColour(9).Colour.Red = 176.0F / 255.0F
        PlayerColour(9).Colour.Green = 112.0F / 255.0F
        PlayerColour(9).Colour.Blue = 112.0F / 255.0F
        PlayerColour(10).Colour.Red = 224.0F / 255.0F
        PlayerColour(10).Colour.Green = 224.0F / 255.0F
        PlayerColour(10).Colour.Blue = 224.0F / 255.0F
        PlayerColour(11).Colour.Red = 32.0F / 255.0F
        PlayerColour(11).Colour.Green = 32.0F / 255.0F
        PlayerColour(11).Colour.Blue = 255.0F / 255.0F
        PlayerColour(12).Colour.Red = 0.0F
        PlayerColour(12).Colour.Green = 160.0F / 255.0F
        PlayerColour(12).Colour.Blue = 0.0F
        PlayerColour(13).Colour.Red = 64.0F / 255.0F
        PlayerColour(13).Colour.Green = 0.0F
        PlayerColour(13).Colour.Blue = 0.0F
        PlayerColour(14).Colour.Red = 16.0F / 255.0F
        PlayerColour(14).Colour.Green = 0.0F
        PlayerColour(14).Colour.Blue = 64.0F / 255.0F
        PlayerColour(15).Colour.Red = 64.0F / 255.0F
        PlayerColour(15).Colour.Green = 96.0F / 255.0F
        PlayerColour(15).Colour.Blue = 0.0F
        For A = 0 To 15
            PlayerColour(A).CalcMinimapColour()
        Next

        MinimapFeatureColour.Red = 0.5F
        MinimapFeatureColour.Green = 0.5F
        MinimapFeatureColour.Blue = 0.5F

        Dim SettingsLoadResult As clsResult = Settings_Load()
        InitializeResult.AppendAsWarning(SettingsLoadResult, "Load settings: ")

        If Settings.DirectoriesPrompt Then
            If frmDataInstance.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                End
            End If
        End If
        frmDataInstance.HideButtons()

        Dim TilesetsPath As String = frmDataInstance.TilesetsPathSet.SelectedPath
        If TilesetsPath IsNot Nothing And TilesetsPath <> "" Then
            InitializeResult.AppendAsWarning(LoadTilesets(EndWithPathSeperator(TilesetsPath)), "Load tilesets: ")
        End If

        cboTileset_Update(-1)

        InitializeResult.AppendAsWarning(NoTile_Texture_Load(), "Load special textures: ")
        cboTileType_Update()

        CreateTemplateDroidTypes() 'do before loading data

        Dim ObjectDataPath As String = frmDataInstance.ObjectDataPathSet.SelectedPath
        If ObjectDataPath IsNot Nothing And ObjectDataPath <> "" Then
            InitializeResult.AppendAsWarning(DataLoad(EndWithPathSeperator(ObjectDataPath)), "Load object data: ")
        End If

        CreateGeneratorTilesets()
        CreatePainterArizona()
        CreatePainterUrban()
        CreatePainterRockies()

        Objects_Update()

        MapView.Dock = DockStyle.Fill
        pnlView.Controls.Add(MapView)

        VisionRadius_2E = 10
        VisionRadius_2E_Changed()

        HeightSetPalette(0) = 0
        HeightSetPalette(1) = 85
        HeightSetPalette(2) = 170
        HeightSetPalette(3) = 255
        HeightSetPalette(4) = 64
        HeightSetPalette(5) = 128
        HeightSetPalette(6) = 192
        HeightSetPalette(7) = 255
        For A = 0 To 7
            tabHeightSetL.TabPages(A).Text = InvariantToString_byte(HeightSetPalette(A))
            tabHeightSetR.TabPages(A).Text = InvariantToString_byte(HeightSetPalette(A))
        Next
        tabHeightSetL.SelectedIndex = 1
        tabHeightSetR.SelectedIndex = 0
        tabHeightSetL_SelectedIndexChanged(Nothing, Nothing)
        tabHeightSetR_SelectedIndexChanged(Nothing, Nothing)

        If CommandLinePaths.GetUpperBound(0) >= 0 Then
            Dim LoadResult As New clsResult
            For A = 0 To CommandLinePaths.GetUpperBound(0)
                LoadResult.Append(LoadMap(CommandLinePaths(A)), "")
            Next
            ShowWarnings(LoadResult, "Load Startup Command-line Maps")
        End If

        TextureView.Dock = DockStyle.Fill
        TableLayoutPanel6.Controls.Add(TextureView, 0, 1)

        Tool = enumTool.Texture_Brush

        MainMapAfterChanged()

        MapView.DrawView_SetEnabled(True)
        TextureView.DrawView_SetEnabled(True)

        WindowState = FormWindowState.Maximized
        Show()

        tmrKey.Enabled = True
        tmrTool.Enabled = True

        ShowWarnings(InitializeResult, "Startup Result")

        ProgramInitializeFinished = True
    End Sub

    Private InitializeDelay As Timer

    Private InitializeResult As New clsResult

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If MonoDevelop = 0.0# Then
        Hide()
#End If

        Try
            ProgramIcon = New Icon(My.Application.Info.DirectoryPath & PlatformPathSeparator & "flaME.ico")
        Catch ex As Exception
            InitializeResult.Warning_Add(ProgramName & " icon is missing: " & ex.Message)
        End Try
        Icon = ProgramIcon
        frmGeneratorInstance.Icon = ProgramIcon
        frmDataInstance.Icon = ProgramIcon

        InitializeDelay = New Timer
        AddHandler InitializeDelay.Tick, AddressOf Initialize
        InitializeDelay.Interval = 50
        InitializeDelay.Enabled = True
    End Sub

    Private Sub Me_LostFocus(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Leave

        ViewKeyDown_Clear()
    End Sub

    Private Sub tmrKey_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrKey.Tick
        If Not ProgramInitialized Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Rate As Double
        Dim Zoom As Double
        Dim Move As Double
        Dim Roll As Double
        Dim Pan As Double
        Dim OrbitRate As Double

        Rate = frmMainInstance.Rate_Get()

        Zoom = frmMainInstance.tmrKey.Interval * Rate * 0.002#
        Move = frmMainInstance.tmrKey.Interval * Rate / 2048.0#
        Roll = Rate * 5.0# * RadOf1Deg
        Pan = Rate / 16.0#
        OrbitRate = Rate / 32.0#

        Map.ViewInfo.TimedActions(Zoom, Move, Pan, Roll, OrbitRate)

        If Map.CheckMessages() Then
            View_DrawViewLater()
        End If
    End Sub

    Public Sub Controls_Set_Default()
        Dim A As Integer

        For A = 0 To InputControlCount - 1
            InputControls(A).SetToDefault()
        Next
    End Sub

    Public Function Rate_Get() As Double

        If Control_Fast.Active Then
            If Control_Slow.Active Then
                Return 8.0#
            Else
                Return 4.0#
            End If
        ElseIf Control_Slow.Active Then
            Return 0.25#
        Else
            Return 1.0#
        End If
    End Function

    Private Sub CreateControls()

        'interface controls

        Control_Deselect = InputControl_Create()
        With Control_Deselect
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Escape)
        End With

        'selected unit controls

        Control_Unit_Move = InputControl_Create()
        With Control_Unit_Move
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.M)
        End With

        Control_Unit_Delete = InputControl_Create()
        With Control_Unit_Delete
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Delete)
        End With

        Control_Unit_Multiselect = InputControl_Create()
        With Control_Unit_Multiselect
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.ShiftKey)
        End With

        'generalised controls

        Control_Slow = InputControl_Create()
        With Control_Slow
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.R)
        End With

        Control_Fast = InputControl_Create()
        With Control_Fast
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F)
        End With

        'picker controls

        Control_Picker = InputControl_Create()
        With Control_Picker
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.ControlKey)
        End With

        'view controls

        Control_View_Textures = InputControl_Create()
        With Control_View_Textures
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F5)
        End With

        Control_View_Lighting = InputControl_Create()
        With Control_View_Lighting
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F8)
        End With

        Control_View_Wireframe = InputControl_Create()
        With Control_View_Wireframe
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F6)
        End With

        Control_View_Units = InputControl_Create()
        With Control_View_Units
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F7)
        End With

        Control_View_ScriptMarkers = InputControl_Create()
        With Control_View_ScriptMarkers
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F4)
        End With

        Control_View_Move_Type = InputControl_Create()
        With Control_View_Move_Type
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F1)
        End With

        Control_View_Rotate_Type = InputControl_Create()
        With Control_View_Rotate_Type
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.F2)
        End With

        Control_View_Move_Left = InputControl_Create()
        With Control_View_Move_Left
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.A)
        End With

        Control_View_Move_Right = InputControl_Create()
        With Control_View_Move_Right
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.D)
        End With

        Control_View_Move_Forward = InputControl_Create()
        With Control_View_Move_Forward
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.W)
        End With

        Control_View_Move_Backward = InputControl_Create()
        With Control_View_Move_Backward
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.S)
        End With

        Control_View_Move_Up = InputControl_Create()
        With Control_View_Move_Up
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.E)
        End With

        Control_View_Move_Down = InputControl_Create()
        With Control_View_Move_Down
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.C)
        End With

        Control_View_Zoom_In = InputControl_Create()
        With Control_View_Zoom_In
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Home)
        End With

        Control_View_Zoom_Out = InputControl_Create()
        With Control_View_Zoom_Out
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.End)
        End With

        Control_View_Left = InputControl_Create()
        With Control_View_Left
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Left)
        End With

        Control_View_Right = InputControl_Create()
        With Control_View_Right
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Right)
        End With

        Control_View_Forward = InputControl_Create()
        With Control_View_Forward
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Up)
        End With

        Control_View_Backward = InputControl_Create()
        With Control_View_Backward
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Down)
        End With

        Control_View_Up = InputControl_Create()
        With Control_View_Up
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.PageUp)
        End With

        Control_View_Down = InputControl_Create()
        With Control_View_Down
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.PageDown)
        End With

        Control_View_Roll_Left = InputControl_Create()
        With Control_View_Roll_Left
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.OemOpenBrackets)
        End With

        Control_View_Roll_Right = InputControl_Create()
        With Control_View_Roll_Right
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.OemCloseBrackets)
        End With

        Control_View_Reset = InputControl_Create()
        With Control_View_Reset
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.Back)
        End With

        'texture controls

        Control_CounterClockwise = InputControl_Create()
        With Control_CounterClockwise
            .DefaultKeys = New clsInputControl.clsKeyCombo(188) ',<
        End With

        Control_Clockwise = InputControl_Create()
        With Control_Clockwise
            .DefaultKeys = New clsInputControl.clsKeyCombo(190) '.>
        End With

        Control_Texture_Flip = InputControl_Create()
        With Control_Texture_Flip
            .DefaultKeys = New clsInputControl.clsKeyCombo(191) '/?
        End With

        Control_Tri_Flip = InputControl_Create()
        With Control_Tri_Flip
            .DefaultKeys = New clsInputControl.clsKeyCombo(220) '\|
        End With

        Control_Gateway_Delete = InputControl_Create()
        With Control_Gateway_Delete
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.ShiftKey)
        End With

        'undo controls

        Control_Undo = InputControl_Create()
        With Control_Undo
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.ControlKey, Keys.Z)
        End With

        Control_Redo = InputControl_Create()
        With Control_Redo
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.ControlKey, Keys.Y)
        End With

        Control_ScriptPosition = InputControl_Create()
        With Control_ScriptPosition
            .DefaultKeys = New clsInputControl.clsKeyCombo(Keys.P)
        End With

        Controls_Set_Default()
    End Sub

    Public Function InputControl_Create() As clsInputControl

        ReDim Preserve InputControls(InputControlCount)
        InputControls(InputControlCount) = New clsInputControl
        InputControl_Create = InputControls(InputControlCount)
        InputControlCount += 1
    End Function

    Private LoadMapDialog As New OpenFileDialog

    Public Sub Load_Map_Prompt()

        LoadMapDialog.FileName = ""
        LoadMapDialog.Filter = "Warzone Map Files (*.fmap, *.fme, *.wz, *.gam, *.lnd)|*.fmap;*.fme;*.wz;*.gam;*.lnd|All Files (*.*)|*.*"
        If LoadMapDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        ShowWarnings(LoadMap(LoadMapDialog.FileName), "Load map")
    End Sub

    Public Sub Load_Heightmap_Prompt()
        Dim Dialog As New OpenFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Image Files (*.bmp, *.png)|*.bmp;*.png|All Files (*.*)|*.*"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim HeightmapBitmap As Bitmap = Nothing
        Dim Result As sResult = LoadBitmap(Dialog.FileName, HeightmapBitmap)
        If Result.Success Then
            Dim NewMap As New clsMap(New sXY_int(HeightmapBitmap.Width - 1, HeightmapBitmap.Height - 1))
            Dim X As Integer
            Dim Y As Integer
            Dim PixelColor As Color

            For Y = 0 To HeightmapBitmap.Height - 1
                For X = 0 To HeightmapBitmap.Width - 1
                    PixelColor = HeightmapBitmap.GetPixel(X, Y)
                    NewMap.Terrain.Vertices(X, Y).Height = CByte(Math.Min(Math.Round((CInt(PixelColor.R) + PixelColor.G + PixelColor.B) / 3.0#), Byte.MaxValue))
                Next
            Next

            NewMainMap(NewMap)

            NewMap.Update()

            View_DrawViewLater()
            Exit Sub
        Else
            MsgBox("Failed to load image: " & Result.Problem)
        End If
Error_Exit:
    End Sub

    Public Sub Load_TTP_Prompt()
        Dim Dialog As New OpenFileDialog

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "TTP Files (*.ttp)|*.ttp|All Files (*.*)|*.*"
        If Not Dialog.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult = Map.Load_TTP(Dialog.FileName)
        If Result.Success Then
            TextureView.DrawViewLater()
        Else
            MsgBox("Importing tile types failed: " & Result.Problem)
        End If
    End Sub

    Public Sub cboTileset_Update(ByVal NewSelectedIndex As Integer)
        Dim A As Integer

        cboTileset.Items.Clear()
        For A = 0 To TilesetCount - 1
            cboTileset.Items.Add(Tilesets(A).Name)
        Next
        cboTileset.SelectedIndex = NewSelectedIndex
    End Sub

    Public Sub MainMapTilesetChanged()
        Dim A As Integer
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            cboTileset.SelectedIndex = -1
            Exit Sub
        End If

        For A = 0 To TilesetCount - 1
            If Tilesets(A) Is Map.Tileset Then
                Exit For
            End If
        Next
        If A = TilesetCount Then
            cboTileset.SelectedIndex = -1
        Else
            cboTileset.SelectedIndex = A
        End If
    End Sub

    Private Sub cboTileset_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTileset.SelectedIndexChanged
        Dim NewTileset As clsTileset
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If cboTileset.SelectedIndex < 0 Then
            NewTileset = Nothing
        Else
            NewTileset = Tilesets(cboTileset.SelectedIndex)
        End If
        If NewTileset IsNot Map.Tileset Then
            Map.Tileset = NewTileset
            If Map.Tileset IsNot Nothing Then
                SelectedTextureNum = Math.Min(0, Map.Tileset.TileCount - 1)
            End If
            Map.TileType_Reset()

            Map.SetPainterToDefaults()
            PainterTerrains_Refresh(-1, -1)

            Map.SectorGraphicsChanges.SetAllChanged()
            Map.Update()
            Map.MinimapMakeLater()
            View_DrawViewLater()
            TextureView.ScrollUpdate()
            TextureView.DrawViewLater()
        End If
    End Sub

    Public Sub PainterTerrains_Refresh(ByVal Terrain_NewSelectedIndex As Integer, ByVal Road_NewSelectedIndex As Integer)

        lstAutoTexture.Items.Clear()
        lstAutoRoad.Items.Clear()

        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If

        Dim A As Integer
        For A = 0 To Map.Painter.TerrainCount - 1
            lstAutoTexture.Items.Add(Map.Painter.Terrains(A).Name)
        Next
        For A = 0 To Map.Painter.RoadCount - 1
            lstAutoRoad.Items.Add(Map.Painter.Roads(A).Name)
        Next
        lstAutoTexture.SelectedIndex = Terrain_NewSelectedIndex
        lstAutoRoad.SelectedIndex = Road_NewSelectedIndex
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl.SelectedIndexChanged

        If TabControl.SelectedTab Is tpTextures Then
            Tool = enumTool.Texture_Brush
            TextureView.DrawViewLater()
        ElseIf TabControl.SelectedTab Is tpHeight Then
            If rdoHeightSet.Checked Then
                Tool = enumTool.Height_Set_Brush
            ElseIf rdoHeightSmooth.Checked Then
                Tool = enumTool.Height_Smooth_Brush
            ElseIf rdoHeightChange.Checked Then
                Tool = enumTool.Height_Change_Brush
            End If
        ElseIf TabControl.SelectedTab Is tpAutoTexture Then
            If rdoAutoTexturePlace.Checked Then
                Tool = enumTool.AutoTexture_Place
            ElseIf rdoAutoRoadPlace.Checked Then
                Tool = enumTool.AutoRoad_Place
            ElseIf rdoCliffTriBrush.Checked Then
                Tool = enumTool.CliffTriangle
            ElseIf rdoAutoCliffBrush.Checked Then
                Tool = enumTool.AutoCliff
            ElseIf rdoAutoCliffRemove.Checked Then
                Tool = enumTool.AutoCliffRemove
            ElseIf rdoAutoTextureFill.Checked Then
                Tool = enumTool.AutoTexture_Fill
            ElseIf rdoAutoRoadLine.Checked Then
                Tool = enumTool.AutoRoad_Line
            ElseIf rdoRoadRemove.Checked Then
                Tool = enumTool.AutoRoad_Remove
            Else
                Tool = enumTool.None
            End If
        Else
            Tool = enumTool.None
        End If
    End Sub

    Private Sub btnAutoTri_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutoTri.Click

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim ApplyAutoTri As New clsMap.clsApplyAutoTri
        ApplyAutoTri.Map = Map
        Dim AllTiles As New clsMap.clsPointChanges(Map.Terrain.TileSize)
        AllTiles.SetAllChanged()
        AllTiles.PerformTool(ApplyAutoTri)

        Map.Update()

        Map.UndoStepCreate("Set All Triangles")

        View_DrawViewLater()
    End Sub

    Private Sub rdoHeightSet_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoHeightSet.Click

        If rdoHeightSet.Checked Then
            rdoHeightSmooth.Checked = False
            rdoHeightChange.Checked = False

            Tool = enumTool.Height_Set_Brush
        End If
    End Sub

    Private Sub rdoHeightChange_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoHeightChange.Click

        If rdoHeightChange.Checked Then
            rdoHeightSet.Checked = False
            rdoHeightSmooth.Checked = False

            Tool = enumTool.Height_Change_Brush
        End If
    End Sub

    Private Sub rdoHeightSmooth_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoHeightSmooth.Click

        If rdoHeightSmooth.Checked Then
            rdoHeightSet.Checked = False
            rdoHeightChange.Checked = False

            Tool = enumTool.Height_Smooth_Brush
        End If
    End Sub

    Private Sub tmrTool_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrTool.Tick
        If Not ProgramInitialized Then
            Exit Sub
        End If

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.TimedTools()
    End Sub

    Private Sub btnResize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResize.Click
        Dim NewSize As sXY_int
        Dim Offset As sXY_int
        Dim Max As Double = MapMaxSize

        If Not InvariantParse_int(txtSizeX.Text, NewSize.X) Then
            Exit Sub
        End If
        If Not InvariantParse_int(txtSizeY.Text, NewSize.Y) Then
            Exit Sub
        End If
        If Not InvariantParse_int(txtOffsetX.Text, Offset.X) Then
            Exit Sub
        End If
        If Not InvariantParse_int(txtOffsetY.Text, Offset.Y) Then
            Exit Sub
        End If

        Map_Resize(Offset, NewSize)
    End Sub

    Public Sub Map_Resize(ByVal Offset As sXY_int, ByVal NewSize As sXY_int)

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If MsgBox("Resizing can't be undone. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
            Exit Sub
        End If

        If NewSize.X < 1 Or NewSize.Y < 1 Then
            MsgBox("Map sizes must be at least 1.", MsgBoxStyle.OkOnly, "")
            Exit Sub
        End If
        If NewSize.X > WZMapMaxSize Or NewSize.Y > WZMapMaxSize Then
            If MsgBox("Warzone doesn't support map sizes above " & WZMapMaxSize & ". Continue anyway?", MsgBoxStyle.YesNo, "") <> MsgBoxResult.Yes Then
                Exit Sub
            End If
        End If

        Map.Terrain_Resize(Offset, NewSize)

        Resize_Update()
        ScriptMarkerLists_Update()

        Map.SectorGraphicsChanges.SetAllChanged()

        Map.Update()

        View_DrawViewLater()
    End Sub

    Public Sub Resize_Update()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            txtSizeX.Text = ""
            txtSizeY.Text = ""
            txtOffsetX.Text = ""
            txtOffsetY.Text = ""
        Else
            txtSizeX.Text = InvariantToString_int(Map.Terrain.TileSize.X)
            txtSizeY.Text = InvariantToString_int(Map.Terrain.TileSize.Y)
            txtOffsetX.Text = "0"
            txtOffsetY.Text = "0"
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseToolStripMenuItem.Click

        Close()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click

        Load_Map_Prompt()
    End Sub

    Private Sub LNDToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapLNDToolStripMenuItem.Click

        Save_LND_Prompt()
    End Sub

    Public Sub Save_LND_Prompt()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Editworld Files (*.lnd)|*.lnd"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim Result As clsResult
        Result = Map.Write_LND(Dialog.FileName, True)

        ShowWarnings(Result, "Save LND")
    End Sub

    Private Sub Save_FMap_Prompt()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = Map.GetDirectory
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " Map Files (*.fmap)|*.fmap"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        'If Not PromptFileFMEVersion(Dialog.FileName) Then
        '    Save_FME_Prompt()
        '    Exit Sub
        'End If
        Dim Result As clsResult
        Result = Map.Write_FMap(Dialog.FileName, True, True)
        If Not Result.HasProblems Then
            Map.PathInfo = New clsMap.clsPathInfo(Dialog.FileName, True)
            tsbSave.Enabled = False
            Map.ChangedSinceSave = False
            Title_Text_Update()
        End If
        ShowWarnings(Result, "Save FMap")
    End Sub

    Private Sub Save_FME_Prompt()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " FME Map Files (*.fme)|*.fme"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim strScavenger As String = InputBox("Enter the player number for scavenger units:")
        Dim ScavengerNum As Byte
        If Not InvariantParse_byte(strScavenger, ScavengerNum) Then
            MsgBox("Unable to save FME: entered scavenger number is not a number.")
            Exit Sub
        End If
        ScavengerNum = Math.Min(ScavengerNum, CByte(10))
        Dim Result As sResult
        Result = Map.Write_FME(Dialog.FileName, True, ScavengerNum)
        If Not Result.Success Then
            MsgBox("Unable to save FME: " & Result.Problem)
        End If
    End Sub

    Public Sub Save_FMap_Quick()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.PathInfo Is Nothing Then
            Save_FMap_Prompt()
        ElseIf Map.PathInfo.IsFMap Then
            Dim Result As clsResult = Map.Write_FMap(Map.PathInfo.Path, True, True)
            If Not Result.HasProblems Then
                tsbSave.Enabled = False
                Map.ChangedSinceSave = False
            End If
            ShowWarnings(Result, "Quick Save FMap")
        Else
            Save_FMap_Prompt()
        End If
    End Sub

    Public Sub Save_Minimap_Prompt()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Bitmap File (*.bmp)|*.bmp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Map.Write_MinimapFile(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the minimap bitmap: " & Result.Problem)
        End If
    End Sub

    Public Sub Save_Heightmap_Prompt()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Bitmap File (*.bmp)|*.bmp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Map.Write_Heightmap(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the heightmap bitmap: " & Result.Problem)
        End If
    End Sub

    Public Sub PromptSave_TTP()

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "TTP Files (*.ttp)|*.ttp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Map.Write_TTP(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the tile types: " & Result.Problem)
        End If
    End Sub

    Public Sub New_Prompt()

        NewMap()
    End Sub

    Public Sub NewMap()

        Dim NewMap As New clsMap(New sXY_int(64, 64))
        NewMainMap(NewMap)

        NewMap.RandomizeTileOrientations()
        NewMap.Update()
    End Sub

    Private Sub rdoAutoCliffRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoCliffRemove.Click

        Tool = enumTool.AutoCliffRemove
    End Sub

    Private Sub rdoAutoCliffBrush_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoCliffBrush.Click

        Tool = enumTool.AutoCliff
    End Sub

    Private Sub MinimapBMPToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MinimapBMPToolStripMenuItem.Click

        Save_Minimap_Prompt()
    End Sub

    Private Sub FMEToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSaveFMap.Click

        Save_FMap_Prompt()
    End Sub

    Private Sub MapWZToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapWZToolStripMenuItem.Click

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.CompileScreen Is Nothing Then
            Dim NewCompile As frmCompile = frmCompile.Create(Map)
            NewCompile.Show()
        Else
            Map.CompileScreen.Activate()
        End If
    End Sub

    Private Sub rdoAutoTextureFill_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoTextureFill.Click

        Tool = enumTool.AutoTexture_Fill
    End Sub

    Private Sub btnHeightOffsetSelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHeightOffsetSelection.Click

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If (Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing) Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim Offset As Double
        Dim StartXY As sXY_int
        Dim FinishXY As sXY_int
        Dim Pos As sXY_int

        If Not InvariantParse_dbl(txtHeightOffset.Text, Offset) Then
            Exit Sub
        End If

        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
        For Y = StartXY.Y To FinishXY.Y
            For X = StartXY.X To FinishXY.X
                Map.Terrain.Vertices(X, Y).Height = CByte(Math.Round(Clamp_dbl(Map.Terrain.Vertices(X, Y).Height + Offset, Byte.MinValue, Byte.MaxValue)))
                Pos.X = X
                Pos.Y = Y
                Map.SectorGraphicsChanges.VertexAndNormalsChanged(Pos)
                Map.SectorUnitHeightsChanges.VertexChanged(Pos)
                Map.SectorTerrainUndoChanges.VertexChanged(Pos)
            Next
        Next

        Map.Update()

        Map.UndoStepCreate("Selection Heights Offset")

        View_DrawViewLater()
    End Sub

    Private Sub rdoAutoTexturePlace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoTexturePlace.Click

        Tool = enumTool.AutoTexture_Place
    End Sub

    Private Sub rdoAutoRoadPlace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoRoadPlace.Click

        Tool = enumTool.AutoRoad_Place
    End Sub

    Private Sub rdoAutoRoadLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoRoadLine.Click

        Tool = enumTool.AutoRoad_Line
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.Selected_Tile_A = Nothing
            Map.Selected_Tile_B = Nothing
        End If
    End Sub

    Private Sub rdoRoadRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoRoadRemove.Click

        Tool = enumTool.AutoRoad_Remove
    End Sub

    Private Sub btnAutoRoadRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutoRoadRemove.Click

        lstAutoRoad.SelectedIndex = -1
    End Sub

    Private Sub btnAutoTextureRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutoTextureRemove.Click

        lstAutoTexture.SelectedIndex = -1
    End Sub

    Private Sub NewMapToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewMapToolStripMenuItem.Click

        New_Prompt()
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click

        Save_Heightmap_Prompt()
    End Sub

    Private Sub lstFeatures_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstFeatures.SelectedIndexChanged

        If lstFeatures.SelectedIndex >= 0 Then
            Tool = enumTool.ObjectPlace
            SelectedObjectType = lstFeatures_Objects(lstFeatures.SelectedIndex)
            lstStructures.SelectedIndex = -1
            lstDroids.SelectedIndex = -1
        End If
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater() 'for unit highlight
            View_DrawViewLater()
        End If
    End Sub

    Private Sub lstStructures_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstStructures.SelectedIndexChanged

        If lstStructures.SelectedIndex >= 0 Then
            Tool = enumTool.ObjectPlace
            SelectedObjectType = lstStructures_Objects(lstStructures.SelectedIndex)
            lstFeatures.SelectedIndex = -1
            lstDroids.SelectedIndex = -1
        End If
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater() 'for unit highlight
            View_DrawViewLater()
        End If
    End Sub

    Private Sub lstDroids_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstDroids.SelectedIndexChanged

        If lstDroids.SelectedIndex >= 0 Then
            Tool = enumTool.ObjectPlace
            SelectedObjectType = lstDroids_Objects(lstDroids.SelectedIndex)
            lstFeatures.SelectedIndex = -1
            lstStructures.SelectedIndex = -1
        End If
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater() 'for unit highlight
            View_DrawViewLater()
        End If
    End Sub

    Public Sub Objects_Update()
        Dim A As Integer
        Dim tmpDroid As clsDroidDesign
        Dim tmpTemplate As clsDroidTemplate
        Dim tmpFeature As clsFeatureType
        Dim tmpStructure As clsStructureType

        lstFeatures.Items.Clear()
        lstStructures.Items.Clear()
        lstDroids.Items.Clear()
        For A = 0 To UnitTypeCount - 1
            If UnitTypes(A).Type = clsUnitType.enumType.Feature Then
                tmpFeature = CType(UnitTypes(A), clsFeatureType)
                lstFeatures.Items.Add(tmpFeature.Code & " (" & tmpFeature.Name & ")")
                ReDim Preserve lstFeatures_Objects(lstFeatures.Items.Count - 1)
                lstFeatures_Objects(lstFeatures.Items.Count - 1) = tmpFeature
            ElseIf UnitTypes(A).Type = clsUnitType.enumType.PlayerDroid Then
                tmpDroid = CType(UnitTypes(A), clsDroidDesign)
                If tmpDroid.IsTemplate Then
                    tmpTemplate = CType(tmpDroid, clsDroidTemplate)
                    If tmpTemplate.Code <> "ConstructorDroid" Then
                        lstDroids.Items.Add(tmpTemplate.Code & " (" & tmpTemplate.Name & ")")
                        ReDim Preserve lstDroids_Objects(lstDroids.Items.Count - 1)
                        lstDroids_Objects(lstDroids.Items.Count - 1) = tmpTemplate
                    End If
                End If
            ElseIf UnitTypes(A).Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(UnitTypes(A), clsStructureType)
                lstStructures.Items.Add(tmpStructure.Code & " (" & tmpStructure.Name & ")")
                ReDim Preserve lstStructures_Objects(lstStructures.Items.Count - 1)
                lstStructures_Objects(lstStructures.Items.Count - 1) = UnitTypes(A)
            End If
        Next

        Dim tmpBody As clsBody
        Dim tmpPropulsion As clsPropulsion
        Dim tmpTurret As clsTurret
        Dim strTemp As String
        Dim B As Integer

        cboDroidBody.Items.Clear()
        ReDim cboBody_Objects(BodyCount - 1)
        B = 0
        For A = 0 To BodyCount - 1
            tmpBody = Bodies(A)
            If tmpBody.Designable Or True Then
                cboDroidBody.Items.Add("(" & tmpBody.Name & ") " & tmpBody.Code)
                cboBody_Objects(B) = tmpBody
                B += 1
            End If
        Next
        ReDim Preserve cboBody_Objects(B - 1)

        cboDroidPropulsion.Items.Clear()
        ReDim cboPropulsion_Objects(PropulsionCount - 1)
        B = 0
        For A = 0 To PropulsionCount - 1
            tmpPropulsion = Propulsions(A)
            If tmpPropulsion.Designable Or True Then
                cboDroidPropulsion.Items.Add("(" & tmpPropulsion.Name & ") " & tmpPropulsion.Code)
                cboPropulsion_Objects(B) = tmpPropulsion
                B += 1
            End If
        Next
        ReDim Preserve cboPropulsion_Objects(B - 1)

        cboDroidTurret1.Items.Clear()
        cboDroidTurret2.Items.Clear()
        cboDroidTurret3.Items.Clear()
        ReDim cboTurret_Objects(WeaponCount + ConstructCount + RepairCount + SensorCount + BrainCount - 1)
        B = 0
        For A = 0 To WeaponCount - 1
            tmpTurret = Weapons(A)
            If tmpTurret.Designable Or True Then
                strTemp = "(Weapon - " & tmpTurret.Name & ") " & tmpTurret.Code
                cboDroidTurret1.Items.Add(strTemp)
                cboDroidTurret2.Items.Add(strTemp)
                cboDroidTurret3.Items.Add(strTemp)
                cboTurret_Objects(B) = tmpTurret
                B += 1
            End If
        Next
        For A = 0 To ConstructCount - 1
            tmpTurret = Constructs(A)
            If tmpTurret.Designable Or True Then
                strTemp = "(Construct - " & tmpTurret.Name & ") " & tmpTurret.Code
                cboDroidTurret1.Items.Add(strTemp)
                cboDroidTurret2.Items.Add(strTemp)
                cboDroidTurret3.Items.Add(strTemp)
                cboTurret_Objects(B) = tmpTurret
                B += 1
            End If
        Next
        For A = 0 To RepairCount - 1
            tmpTurret = Repairs(A)
            If tmpTurret.Designable Or True Then
                strTemp = "(Repair - " & tmpTurret.Name & ") " & tmpTurret.Code
                cboDroidTurret1.Items.Add(strTemp)
                cboDroidTurret2.Items.Add(strTemp)
                cboDroidTurret3.Items.Add(strTemp)
                cboTurret_Objects(B) = tmpTurret
                B += 1
            End If
        Next
        For A = 0 To SensorCount - 1
            tmpTurret = Sensors(A)
            If tmpTurret.Designable Or True Then
                strTemp = "(Sensor - " & tmpTurret.Name & ") " & tmpTurret.Code
                cboDroidTurret1.Items.Add(strTemp)
                cboDroidTurret2.Items.Add(strTemp)
                cboDroidTurret3.Items.Add(strTemp)
                cboTurret_Objects(B) = tmpTurret
                B += 1
            End If
        Next
        For A = 0 To BrainCount - 1
            tmpTurret = Brains(A)
            If tmpTurret.Designable Or True Then
                strTemp = "(Brain - " & tmpTurret.Name & ") " & tmpTurret.Code
                cboDroidTurret1.Items.Add(strTemp)
                cboDroidTurret2.Items.Add(strTemp)
                cboDroidTurret3.Items.Add(strTemp)
                cboTurret_Objects(B) = tmpTurret
                B += 1
            End If
        Next
        ReDim Preserve cboTurret_Objects(B - 1)

        cboDroidType.Items.Clear()
        For A = 0 To TemplateDroidTypeCount - 1
            cboDroidType.Items.Add(TemplateDroidTypes(A).Name)
        Next
    End Sub

    Private Sub txtObjectRotation_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectRotation.Leave
        If Not txtObjectRotation.Enabled Then
            Exit Sub
        End If
        If txtObjectRotation.Text = "" Then
            Exit Sub
        End If

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        Dim Angle As Integer
        If Not InvariantParse_int(txtObjectRotation.Text, Angle) Then
            'MsgBox("Invalid rotation value.", CType(MsgBoxStyle.OkOnly + MsgBoxStyle.Information, MsgBoxStyle), "")
            'SelectedObject_Changed()
            'todo
            Exit Sub
        End If

        Angle = Clamp_int(Angle, 0, 359)

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change rotation of multiple objects?", MsgBoxStyle.OkCancel, "") <> MsgBoxResult.Ok Then
                'SelectedObject_Changed()
                Exit Sub
            End If
        End If

        Dim ObjectRotation As New clsMap.clsObjectRotation
        ObjectRotation.Map = Map
        ObjectRotation.Angle = Angle
        Map.SelectedUnitsAction(ObjectRotation)

        Map.Update()
        SelectedObject_Changed()
        Map.UndoStepCreate("Object Rotated")
        View_DrawViewLater()
    End Sub

    Public Sub SelectedObject_Changed()
        Dim ClearControls As Boolean
        Dim Map As clsMap = MainMap

        lblObjectType.Enabled = False
        ObjectPlayerNum.Enabled = False
        txtObjectRotation.Enabled = False
        txtObjectID.Enabled = False
        txtObjectLabel.Enabled = False
        txtObjectPriority.Enabled = False
        txtObjectHealth.Enabled = False
        btnDroidToDesign.Enabled = False
        cboDroidType.Enabled = False
        cboDroidBody.Enabled = False
        cboDroidPropulsion.Enabled = False
        cboDroidTurret1.Enabled = False
        cboDroidTurret2.Enabled = False
        cboDroidTurret3.Enabled = False
        rdoDroidTurret0.Enabled = False
        rdoDroidTurret1.Enabled = False
        rdoDroidTurret2.Enabled = False
        rdoDroidTurret3.Enabled = False
        If Map Is Nothing Then
            ClearControls = True
        ElseIf Map.SelectedUnits.ItemCount = 0 Then
            ClearControls = True
        End If
        If ClearControls Then
            lblObjectType.Text = ""
            ObjectPlayerNum.Target.Item = Nothing
            txtObjectRotation.Text = ""
            txtObjectID.Text = ""
            txtObjectLabel.Text = ""
            txtObjectPriority.Text = ""
            txtObjectHealth.Text = ""
            cboDroidType.SelectedIndex = -1
            cboDroidBody.SelectedIndex = -1
            cboDroidPropulsion.SelectedIndex = -1
            cboDroidTurret1.SelectedIndex = -1
            cboDroidTurret2.SelectedIndex = -1
            cboDroidTurret3.SelectedIndex = -1
            rdoDroidTurret0.Checked = False
            rdoDroidTurret1.Checked = False
            rdoDroidTurret2.Checked = False
            rdoDroidTurret3.Checked = False
        ElseIf Map.SelectedUnits.ItemCount > 1 Then
            lblObjectType.Text = "Multiple objects"
            Dim A As Integer
            Dim tmpUnitGroup As clsMap.clsUnitGroup = Map.SelectedUnits.Item(0).UnitGroup
            For A = 1 To Map.SelectedUnits.ItemCount - 1
                If Map.SelectedUnits.Item(A).UnitGroup IsNot tmpUnitGroup Then
                    Exit For
                End If
            Next
            If A = Map.SelectedUnits.ItemCount Then
                ObjectPlayerNum.Target.Item = tmpUnitGroup
            Else
                ObjectPlayerNum.Target.Item = Nothing
            End If
            txtObjectRotation.Text = ""
            txtObjectID.Text = ""
            txtObjectLabel.Text = ""
            lblObjectType.Enabled = True
            ObjectPlayerNum.Enabled = True
            txtObjectRotation.Enabled = True
            txtObjectPriority.Text = ""
            txtObjectPriority.Enabled = True
            txtObjectHealth.Text = ""
            txtObjectHealth.Enabled = True
            'design
            For A = 0 To Map.SelectedUnits.ItemCount - 1
                If Map.SelectedUnits.Item(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    If CType(Map.SelectedUnits.Item(A).Type, clsDroidDesign).IsTemplate Then
                        Exit For
                    End If
                End If
            Next
            If A < Map.SelectedUnits.ItemCount Then
                btnDroidToDesign.Enabled = True
            End If

            For A = 0 To Map.SelectedUnits.ItemCount - 1
                If Map.SelectedUnits.Item(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    If Not CType(Map.SelectedUnits.Item(A).Type, clsDroidDesign).IsTemplate Then
                        Exit For
                    End If
                End If
            Next
            If A < Map.SelectedUnits.ItemCount Then
                cboDroidType.SelectedIndex = -1
                cboDroidBody.SelectedIndex = -1
                cboDroidPropulsion.SelectedIndex = -1
                cboDroidTurret1.SelectedIndex = -1
                cboDroidTurret2.SelectedIndex = -1
                cboDroidTurret3.SelectedIndex = -1
                rdoDroidTurret1.Checked = False
                rdoDroidTurret2.Checked = False
                rdoDroidTurret3.Checked = False
                cboDroidType.Enabled = True
                cboDroidBody.Enabled = True
                cboDroidPropulsion.Enabled = True
                cboDroidTurret1.Enabled = True
                cboDroidTurret2.Enabled = True
                cboDroidTurret3.Enabled = True
                rdoDroidTurret0.Enabled = True
                rdoDroidTurret1.Enabled = True
                rdoDroidTurret2.Enabled = True
                rdoDroidTurret3.Enabled = True
            End If
        ElseIf Map.SelectedUnits.ItemCount = 1 Then
            Dim A As Integer
            With Map.SelectedUnits.Item(0)
                lblObjectType.Text = .Type.GetDisplayText
                ObjectPlayerNum.Target.Item = .UnitGroup
                txtObjectRotation.Text = InvariantToString_int(.Rotation)
                txtObjectID.Text = InvariantToString_sng(.ID)
                txtObjectPriority.Text = InvariantToString_int(.SavePriority)
                txtObjectHealth.Text = InvariantToString_dbl(.Health * 100.0#)
                lblObjectType.Enabled = True
                ObjectPlayerNum.Enabled = True
                txtObjectRotation.Enabled = True
                'txtObjectID.Enabled = True 'no known need to change IDs
                txtObjectPriority.Enabled = True
                txtObjectHealth.Enabled = True
                Dim LabelEnabled As Boolean = True
                If .Type.Type = clsUnitType.enumType.PlayerStructure Then
                    Dim tmpType As clsStructureType.enumStructureType = CType(.Type, clsStructureType).StructureType
                    If tmpType = clsStructureType.enumStructureType.FactoryModule Or _
                        tmpType = clsStructureType.enumStructureType.PowerModule Or _
                        tmpType = clsStructureType.enumStructureType.ResearchModule Then
                        LabelEnabled = False
                    End If
                End If
                If LabelEnabled Then
                    txtObjectLabel.Text = .Label
                    txtObjectLabel.Enabled = True
                Else
                    txtObjectLabel.Text = ""
                End If
                Dim ClearDesignControls As Boolean = False
                If .Type.Type = clsUnitType.enumType.PlayerDroid Then
                    Dim tmpDroid As clsDroidDesign = CType(.Type, clsDroidDesign)
                    If tmpDroid.IsTemplate Then
                        btnDroidToDesign.Enabled = True
                        ClearDesignControls = True
                    Else
                        If tmpDroid.TemplateDroidType Is Nothing Then
                            cboDroidType.SelectedIndex = -1
                        Else
                            cboDroidType.SelectedIndex = tmpDroid.TemplateDroidType.Num
                        End If

                        If tmpDroid.Body Is Nothing Then
                            cboDroidBody.SelectedIndex = -1
                        Else
                            For A = 0 To cboDroidBody.Items.Count - 1
                                If cboBody_Objects(A) Is tmpDroid.Body Then
                                    Exit For
                                End If
                            Next
                            If A < 0 Then

                            ElseIf A < cboDroidBody.Items.Count Then
                                cboDroidBody.SelectedIndex = A
                            Else
                                cboDroidBody.SelectedIndex = -1
                                cboDroidBody.Text = tmpDroid.Body.Code
                            End If
                        End If

                        If tmpDroid.Propulsion Is Nothing Then
                            cboDroidPropulsion.SelectedIndex = -1
                        Else
                            For A = 0 To cboDroidPropulsion.Items.Count - 1
                                If cboPropulsion_Objects(A) Is tmpDroid.Propulsion Then
                                    Exit For
                                End If
                            Next
                            If A < cboDroidPropulsion.Items.Count Then
                                cboDroidPropulsion.SelectedIndex = A
                            Else
                                cboDroidPropulsion.SelectedIndex = -1
                                cboDroidPropulsion.Text = tmpDroid.Propulsion.Code
                            End If
                        End If

                        If tmpDroid.Turret1 Is Nothing Then
                            cboDroidTurret1.SelectedIndex = -1
                        Else
                            For A = 0 To cboDroidTurret1.Items.Count - 1
                                If cboTurret_Objects(A) Is tmpDroid.Turret1 Then
                                    Exit For
                                End If
                            Next
                            If A < cboDroidTurret1.Items.Count Then
                                cboDroidTurret1.SelectedIndex = A
                            Else
                                cboDroidTurret1.SelectedIndex = -1
                                cboDroidTurret1.Text = tmpDroid.Turret1.Code
                            End If
                        End If

                        If tmpDroid.Turret2 Is Nothing Then
                            cboDroidTurret2.SelectedIndex = -1
                        Else
                            For A = 0 To cboDroidTurret2.Items.Count - 1
                                If cboTurret_Objects(A) Is tmpDroid.Turret2 Then
                                    Exit For
                                End If
                            Next
                            If A < cboDroidTurret2.Items.Count Then
                                cboDroidTurret2.SelectedIndex = A
                            Else
                                cboDroidTurret2.SelectedIndex = -1
                                cboDroidTurret2.Text = tmpDroid.Turret2.Code
                            End If
                        End If

                        If tmpDroid.Turret3 Is Nothing Then
                            cboDroidTurret3.SelectedIndex = -1
                        Else
                            For A = 0 To cboDroidTurret3.Items.Count - 1
                                If cboTurret_Objects(A) Is tmpDroid.Turret3 Then
                                    Exit For
                                End If
                            Next
                            If A < cboDroidTurret3.Items.Count Then
                                cboDroidTurret3.SelectedIndex = A
                            Else
                                cboDroidTurret3.SelectedIndex = -1
                                cboDroidTurret3.Text = tmpDroid.Turret3.Code
                            End If
                        End If

                        If tmpDroid.TurretCount = 3 Then
                            rdoDroidTurret0.Checked = False
                            rdoDroidTurret1.Checked = False
                            rdoDroidTurret2.Checked = False
                            rdoDroidTurret3.Checked = True
                        ElseIf tmpDroid.TurretCount = 2 Then
                            rdoDroidTurret0.Checked = False
                            rdoDroidTurret1.Checked = False
                            rdoDroidTurret2.Checked = True
                            rdoDroidTurret3.Checked = False
                        ElseIf tmpDroid.TurretCount = 1 Then
                            rdoDroidTurret0.Checked = False
                            rdoDroidTurret1.Checked = True
                            rdoDroidTurret2.Checked = False
                            rdoDroidTurret3.Checked = False
                        ElseIf tmpDroid.TurretCount = 0 Then
                            rdoDroidTurret0.Checked = True
                            rdoDroidTurret1.Checked = False
                            rdoDroidTurret2.Checked = False
                            rdoDroidTurret3.Checked = False
                        Else
                            rdoDroidTurret0.Checked = False
                            rdoDroidTurret1.Checked = False
                            rdoDroidTurret2.Checked = False
                            rdoDroidTurret3.Checked = False
                        End If

                        cboDroidType.Enabled = True
                        cboDroidBody.Enabled = True
                        cboDroidPropulsion.Enabled = True
                        cboDroidTurret1.Enabled = True
                        cboDroidTurret2.Enabled = True
                        cboDroidTurret3.Enabled = True
                        rdoDroidTurret0.Enabled = True
                        rdoDroidTurret1.Enabled = True
                        rdoDroidTurret2.Enabled = True
                        rdoDroidTurret3.Enabled = True
                    End If
                Else
                    ClearDesignControls = True
                End If
                If ClearDesignControls Then
                    cboDroidType.SelectedIndex = -1
                    cboDroidBody.SelectedIndex = -1
                    cboDroidPropulsion.SelectedIndex = -1
                    cboDroidTurret1.SelectedIndex = -1
                    cboDroidTurret2.SelectedIndex = -1
                    cboDroidTurret3.SelectedIndex = -1
                    rdoDroidTurret1.Checked = False
                    rdoDroidTurret2.Checked = False
                    rdoDroidTurret3.Checked = False
                End If
            End With
        End If
    End Sub

    Private Sub tsbSelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelection.Click

        Tool = enumTool.Terrain_Select
    End Sub

    Private Sub tsbSelectionCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionCopy.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        If Copied_Map IsNot Nothing Then
            Copied_Map.Deallocate()
        End If
        Dim Area As sXY_int
        Dim Start As sXY_int
        Dim Finish As sXY_int
        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish)
        Area.X = Finish.X - Start.X
        Area.Y = Finish.Y - Start.Y
        Copied_Map = New clsMap(Map, Start, Area)
    End Sub

    Private Sub tsbSelectionPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionPaste.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        If Copied_Map Is Nothing Then
            MsgBox("Nothing to paste.")
            Exit Sub
        End If
        If Not (menuSelPasteHeights.Checked Or menuSelPasteTextures.Checked Or menuSelPasteUnits.Checked Or menuSelPasteDeleteUnits.Checked Or menuSelPasteGateways.Checked Or menuSelPasteDeleteGateways.Checked) Then
            Exit Sub
        End If
        Dim Area As sXY_int
        Dim Start As sXY_int
        Dim Finish As sXY_int
        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish)
        Area.X = Finish.X - Start.X
        Area.Y = Finish.Y - Start.Y
        Map.Map_Insert(Copied_Map, Start, Area, menuSelPasteHeights.Checked, menuSelPasteTextures.Checked, menuSelPasteUnits.Checked, menuSelPasteDeleteUnits.Checked, menuSelPasteGateways.Checked, menuSelPasteDeleteGateways.Checked)

        SelectedObject_Changed()
        Map.UndoStepCreate("Paste")

        View_DrawViewLater()
    End Sub

    Private Sub btnSelResize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelResize.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing Then
            MsgBox("You haven't selected anything.", MsgBoxStyle.OkOnly, "")
            Exit Sub
        End If

        Dim Start As sXY_int
        Dim Finish As sXY_int
        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish)
        Dim Area As sXY_int
        Area.X = Finish.X - Start.X
        Area.Y = Finish.Y - Start.Y

        Map_Resize(Start, Area)
    End Sub

    Private Sub tsbSelectionRotateClockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionRotateClockwise.Click

        If Copied_Map Is Nothing Then
            MsgBox("Nothing to rotate.")
            Exit Sub
        End If

        Copied_Map.Rotate(Orientation_Clockwise, frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub tsbSelectionRotateAnticlockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionRotateCounterClockwise.Click

        If Copied_Map Is Nothing Then
            MsgBox("Nothing to rotate.")
            Exit Sub
        End If

        Copied_Map.Rotate(Orientation_CounterClockwise, frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub menuMiniShowTex_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowTex.Click
        UpdateMinimap()
    End Sub

    Private Sub menuMiniShowHeight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowHeight.Click
        UpdateMinimap()
    End Sub

    Private Sub menuMiniShowUnits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowUnits.Click
        UpdateMinimap()
    End Sub

    Private Function NoTile_Texture_Load() As clsResult
        Dim ReturnResult As New clsResult

        Dim tmpBitmap As Bitmap = Nothing

        Dim BitmapTextureArgs As sBitmapGLTexture

        BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
        BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
        BitmapTextureArgs.TextureNum = 0
        BitmapTextureArgs.MipMapLevel = 0

        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "notile.png", tmpBitmap).Success Then
            ReturnResult.AppendAsWarning(BitmapIsGLCompatible(tmpBitmap), "notile.png compatability: ")
            BitmapTextureArgs.Texture = tmpBitmap
            BitmapTextureArgs.Perform()
            GLTexture_NoTile = BitmapTextureArgs.TextureNum
        End If
        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "overflow.png", tmpBitmap).Success Then
            ReturnResult.AppendAsWarning(BitmapIsGLCompatible(tmpBitmap), "overflow.png compatability: ")
            BitmapTextureArgs.Texture = tmpBitmap
            BitmapTextureArgs.Perform()
            GLTexture_OverflowTile = BitmapTextureArgs.TextureNum
        End If

        Return ReturnResult
    End Function

    Private Sub tsbGateways_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbGateways.Click

        If Tool = enumTool.Gateways Then
            Draw_Gateways = False
            Tool = enumTool.None
            tsbGateways.Checked = False
        Else
            Draw_Gateways = True
            Tool = enumTool.Gateways
            tsbGateways.Checked = True
        End If
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.Selected_Tile_A = Nothing
            Map.Selected_Tile_B = Nothing
            View_DrawViewLater()
        End If
    End Sub

    Private Sub tsbDrawAutotexture_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDrawAutotexture.Click

        If MapView IsNot Nothing Then
            If Draw_VertexTerrain <> tsbDrawAutotexture.Checked Then
                Draw_VertexTerrain = tsbDrawAutotexture.Checked
                View_DrawViewLater()
            End If
        End If
    End Sub

    Private Sub tsbDrawTileOrientation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDrawTileOrientation.Click

        If MapView IsNot Nothing Then
            If DisplayTileOrientation <> tsbDrawTileOrientation.Checked Then
                DisplayTileOrientation = tsbDrawTileOrientation.Checked
                View_DrawViewLater()
                TextureView.DrawViewLater()
            End If
        End If
    End Sub

    Private Sub menuMiniShowGateways_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowGateways.Click
        UpdateMinimap()
    End Sub

    Private Sub menuMiniShowCliffs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuMiniShowCliffs.Click
        UpdateMinimap()
    End Sub

    Private Sub UpdateMinimap()
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.MinimapMakeLater()
    End Sub

    Private Sub cmbTileType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTileType.SelectedIndexChanged
        If Not cboTileType.Enabled Then
            Exit Sub
        End If

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If cboTileType.SelectedIndex < 0 Then
            Exit Sub
        End If
        If SelectedTextureNum < 0 Or SelectedTextureNum >= Map.Tileset.TileCount Then Exit Sub

        Map.Tile_TypeNum(SelectedTextureNum) = CByte(cboTileType.SelectedIndex)

        TextureView.DrawViewLater()
    End Sub

    Private Sub chkTileTypes_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbxTileTypes.CheckedChanged

        TextureView.DisplayTileTypes = cbxTileTypes.Checked
        TextureView.DrawViewLater()
    End Sub

    Private Sub chkTileNumbers_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbxTileNumbers.CheckedChanged

        TextureView.DisplayTileNumbers = cbxTileNumbers.Checked
        TextureView.DrawViewLater()
    End Sub

    Private Sub cboTileType_Update()
        Dim A As Integer

        cboTileType.Items.Clear()
        For A = 0 To TileTypeCount - 1
            cboTileType.Items.Add(TileTypes(A).Name)
        Next
    End Sub

    Private Sub CreateTileTypes()
        Dim NewTileType As sTileType

        With NewTileType
            .Name = "Sand"
            .DisplayColour.Red = 1.0F
            .DisplayColour.Green = 1.0F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Sandy Brush"
            .DisplayColour.Red = 0.5F
            .DisplayColour.Green = 0.5F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Rubble"
            .DisplayColour.Red = 0.25F
            .DisplayColour.Green = 0.25F
            .DisplayColour.Blue = 0.25F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Green Mud"
            .DisplayColour.Red = 0.0F
            .DisplayColour.Green = 0.5F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Red Brush"
            .DisplayColour.Red = 1.0F
            .DisplayColour.Green = 0.0F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Pink Rock"
            .DisplayColour.Red = 1.0F
            .DisplayColour.Green = 0.5F
            .DisplayColour.Blue = 0.5F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Road"
            .DisplayColour.Red = 0.0F
            .DisplayColour.Green = 0.0F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Water"
            .DisplayColour.Red = 0.0F
            .DisplayColour.Green = 0.0F
            .DisplayColour.Blue = 1.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Cliff Face"
            .DisplayColour.Red = 0.5F
            .DisplayColour.Green = 0.5F
            .DisplayColour.Blue = 0.5F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Baked Earth"
            .DisplayColour.Red = 0.5F
            .DisplayColour.Green = 0.0F
            .DisplayColour.Blue = 0.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Sheet Ice"
            .DisplayColour.Red = 1.0F
            .DisplayColour.Green = 1.0F
            .DisplayColour.Blue = 1.0F
        End With
        TileType_Add(NewTileType)

        With NewTileType
            .Name = "Slush"
            .DisplayColour.Red = 0.75F
            .DisplayColour.Green = 0.75F
            .DisplayColour.Blue = 0.75F
        End With
        TileType_Add(NewTileType)
    End Sub

    Private Sub menuExportMapTileTypes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuExportMapTileTypes.Click

        PromptSave_TTP()
    End Sub

    Private Sub ImportHeightmapToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportHeightmapToolStripMenuItem.Click

        Load_Heightmap_Prompt()
    End Sub

    Private Sub menuImportTileTypes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuImportTileTypes.Click

        Load_TTP_Prompt()
    End Sub

    Private Sub tsbSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSave.Click

        Save_FMap_Quick()
    End Sub

    Private Sub btnGenerator_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerator.Click

        frmGeneratorInstance.Show()
        frmGeneratorInstance.Activate()
    End Sub

    Public Sub Title_Text_Update()
        Dim Map As clsMap = MainMap
        Dim MapFileTitle As String

        If Map Is Nothing Then
            MapFileTitle = "No Map"
            tsbSave.ToolTipText = "No Map"
        Else
            If Map.PathInfo Is Nothing Then
                MapFileTitle = "Unsaved map"
                tsbSave.ToolTipText = "Save FMap..."
            Else
                Dim SplitPath As New sSplitPath(Map.PathInfo.Path)
                If Map.PathInfo.IsFMap Then
                    MapFileTitle = SplitPath.FileTitleWithoutExtension
                    tsbSave.ToolTipText = "Quick save FMap to " & ControlChars.Quote & Map.PathInfo.Path & ControlChars.Quote
                Else
                    MapFileTitle = SplitPath.FileTitle
                    tsbSave.ToolTipText = "Save FMap..."
                End If
            End If
            Map.SetTabText()
        End If

        Text = MapFileTitle & " - " & ProgramName & " " & ProgramVersionNumber
    End Sub

    Private Sub lstAutoTexture_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoTexture.Click
        If Not lstAutoTexture.Enabled Then
            Exit Sub
        End If

        If Not (Tool = enumTool.AutoTexture_Place Or Tool = enumTool.AutoTexture_Fill) Then
            rdoAutoTexturePlace.Checked = True
            rdoAutoTexturePlace_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub lstAutoRoad_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoRoad.Click

        If Not (Tool = enumTool.AutoRoad_Place Or Tool = enumTool.AutoRoad_Line) Then
            rdoAutoRoadLine.Checked = True
            rdoAutoRoadLine_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnReinterpretTerrain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReinterpretTerrain.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If MsgBox("Are you sure?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "Reinterpret Terrain") <> MsgBoxResult.Ok Then
            Exit Sub
        End If

        Map.TerrainInterpretChanges.SetAllChanged()

        Map.Update()

        Map.UndoStepCreate("Interpret Terrain")
    End Sub

    Private Sub tabPlayerNum_SelectedIndexChanged()
        'ObjectPlayerNum.Focus() 'so that the rotation textbox and anything else loses focus, and performs its effects

        If Not ObjectPlayerNum.Enabled Then
            Exit Sub
        End If

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If
        If ObjectPlayerNum.Target.Item Is Nothing Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change player of multiple objects?", MsgBoxStyle.OkCancel, "") <> MsgBoxResult.Ok Then
                'SelectedObject_Changed()
                'todo
                Exit Sub
            End If
        End If

        Dim ObjectUnitGroup As New clsMap.clsObjectUnitGroup
        ObjectUnitGroup.Map = Map
        ObjectUnitGroup.UnitGroup = ObjectPlayerNum.Target.Item
        Map.SelectedUnitsAction(ObjectUnitGroup)

        SelectedObject_Changed()
        Map.UndoStepCreate("Object Player Changed")
        If Settings.MinimapTeamColours Then
            Map.MinimapMakeLater()
        End If
        View_DrawViewLater()
    End Sub

    Private Sub txtHeightSetL_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHeightSetL.Leave
        Dim tmpHeight As Byte
        Dim tmpText As String
        Dim dblTemp As Double

        If Not InvariantParse_dbl(txtHeightSetL.Text, dblTemp) Then
            Exit Sub
        End If
        tmpHeight = CByte(Clamp_dbl(dblTemp, Byte.MinValue, Byte.MaxValue))
        HeightSetPalette(tabHeightSetL.SelectedIndex) = tmpHeight
        If tabHeightSetL.SelectedIndex = tabHeightSetL.SelectedIndex Then
            tabHeightSetL_SelectedIndexChanged(Nothing, Nothing)
        End If
        tmpText = InvariantToString_byte(tmpHeight)
        tabHeightSetL.TabPages(tabHeightSetL.SelectedIndex).Text = tmpText
        tabHeightSetR.TabPages(tabHeightSetL.SelectedIndex).Text = tmpText
    End Sub

    Private Sub txtHeightSetR_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHeightSetR.Leave
        Dim tmpHeight As Byte
        Dim tmpText As String
        Dim dblTemp As Double

        If Not InvariantParse_dbl(txtHeightSetL.Text, dblTemp) Then
            Exit Sub
        End If
        tmpHeight = CByte(Clamp_dbl(dblTemp, Byte.MinValue, Byte.MaxValue))
        HeightSetPalette(tabHeightSetR.SelectedIndex) = tmpHeight
        If tabHeightSetL.SelectedIndex = tabHeightSetR.SelectedIndex Then
            tabHeightSetL_SelectedIndexChanged(Nothing, Nothing)
        End If
        tmpText = InvariantToString_byte(tmpHeight)
        tabHeightSetL.TabPages(tabHeightSetR.SelectedIndex).Text = tmpText
        tabHeightSetR.TabPages(tabHeightSetR.SelectedIndex).Text = tmpText
    End Sub

    Private Sub tabHeightSetL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabHeightSetL.SelectedIndexChanged

        txtHeightSetL.Text = InvariantToString_byte(HeightSetPalette(tabHeightSetL.SelectedIndex))
    End Sub

    Private Sub tabHeightSetR_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabHeightSetR.SelectedIndexChanged

        txtHeightSetR.Text = InvariantToString_byte(HeightSetPalette(tabHeightSetR.SelectedIndex))
    End Sub

    Private Sub tsbSelectionObjects_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionObjects.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim A As Integer

        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, Start, Finish)
        For A = 0 To Map.Units.ItemCount - 1
            If PosIsWithinTileArea(Map.Units.Item(A).Pos.Horizontal, Start, Finish) Then
                If Not Map.Units.Item(A).MapSelectedUnitLink.IsConnected Then
                    Map.Units.Item(A).MapSelectedUnitLink.Connect(Map.SelectedUnits)
                End If
            End If
        Next

        SelectedObject_Changed()
        Tool = enumTool.None
        View_DrawViewLater()
    End Sub

    Public Sub View_DrawViewLater()

        If MapView IsNot Nothing Then
            MapView.DrawViewLater()
        End If
    End Sub

    Private Sub btnWaterTri_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWaterTri.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.WaterTriCorrection()

        Map.Update()

        Map.UndoStepCreate("Water Triangle Correction")

        View_DrawViewLater()
    End Sub

    Private Sub tsbSelectionFlipX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionFlipX.Click

        If Copied_Map Is Nothing Then
            MsgBox("Nothing to flip.")
            Exit Sub
        End If

        Copied_Map.Rotate(Orientation_FlipX, frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub btnHeightsMultiplySelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHeightsMultiplySelection.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.Selected_Area_VertexA Is Nothing Or Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim Multiplier As Double
        Dim StartXY As sXY_int
        Dim FinishXY As sXY_int
        Dim Pos As sXY_int
        Dim dblTemp As Double

        If Not InvariantParse_dbl(txtHeightMultiply.Text, dblTemp) Then
            Exit Sub
        End If
        Multiplier = Clamp_dbl(dblTemp, 0.0#, 255.0#)
        XY_Reorder(Map.Selected_Area_VertexA.XY, Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
        For Y = StartXY.Y To FinishXY.Y
            For X = StartXY.X To FinishXY.X
                Map.Terrain.Vertices(X, Y).Height = CByte(Math.Round(Clamp_dbl(Map.Terrain.Vertices(X, Y).Height * Multiplier, Byte.MinValue, Byte.MaxValue)))
                Pos.X = X
                Pos.Y = Y
                Map.SectorGraphicsChanges.VertexAndNormalsChanged(Pos)
                Map.SectorUnitHeightsChanges.VertexChanged(Pos)
                Map.SectorTerrainUndoChanges.VertexChanged(Pos)
            Next
        Next

        Map.Update()

        Map.UndoStepCreate("Selection Heights Multiply")

        View_DrawViewLater()
    End Sub

    Private Sub btnTextureAnticlockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextureAnticlockwise.Click

        TextureOrientation.RotateAnticlockwise()

        TextureView.DrawViewLater()
    End Sub

    Private Sub btnTextureClockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextureClockwise.Click

        TextureOrientation.RotateClockwise()

        TextureView.DrawViewLater()
    End Sub

    Private Sub btnTextureFlipX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextureFlipX.Click

        If TextureOrientation.SwitchedAxes Then
            TextureOrientation.ResultYFlip = Not TextureOrientation.ResultYFlip
        Else
            TextureOrientation.ResultXFlip = Not TextureOrientation.ResultXFlip
        End If

        TextureView.DrawViewLater()
    End Sub

    Private Sub lstAutoTexture_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoTexture.SelectedIndexChanged
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If lstAutoTexture.SelectedIndex < 0 Then
            SelectedTerrain = Nothing
        ElseIf lstAutoTexture.SelectedIndex < Map.Painter.TerrainCount Then
            SelectedTerrain = Map.Painter.Terrains(lstAutoTexture.SelectedIndex)
        Else
            Stop
            SelectedTerrain = Nothing
        End If
    End Sub

    Private Sub lstAutoRoad_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoRoad.SelectedIndexChanged
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If lstAutoRoad.SelectedIndex < 0 Then
            SelectedRoad = Nothing
        ElseIf lstAutoRoad.SelectedIndex < Map.Painter.RoadCount Then
            SelectedRoad = Map.Painter.Roads(lstAutoRoad.SelectedIndex)
        Else
            Stop
            SelectedRoad = Nothing
        End If
    End Sub

    Private Sub txtObjectPriority_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectPriority.Leave
        If Not txtObjectPriority.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If
        Dim Priority As Integer
        If Not InvariantParse_int(txtObjectPriority.Text, Priority) Then
            'MsgBox("Entered text is not a valid number.", CType(MsgBoxStyle.OkOnly + MsgBoxStyle.Information, MsgBoxStyle), "")
            'SelectedObject_Changed()
            'todo
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change priority of multiple objects?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                'SelectedObject_Changed()
                'todo
                Exit Sub
            End If
        ElseIf Map.SelectedUnits.ItemCount = 1 Then
            If Priority = Map.SelectedUnits.Item(0).SavePriority Then
                Exit Sub
            End If
        End If

        Dim ObjectPriority As New clsMap.clsObjectPriority
        ObjectPriority.Map = Map
        ObjectPriority.Priority = Priority
        Map.SelectedUnitsAction(ObjectPriority)

        SelectedObject_Changed()
        Map.UndoStepCreate("Object Priority Changed")
        View_DrawViewLater()
    End Sub

    Private Sub txtObjectHealth_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectHealth.Leave
        If Not txtObjectHealth.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        Dim Health As Double
        If Not InvariantParse_dbl(txtObjectHealth.Text, Health) Then
            'SelectedObject_Changed()
            'todo
            Exit Sub
        End If

        Health = Clamp_dbl(Health, 1.0#, 100.0#) / 100.0#

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change health of multiple objects?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                'SelectedObject_Changed()
                'todo
                Exit Sub
            End If
        End If

        Dim ObjectHealth As New clsMap.clsObjectHealth
        ObjectHealth.Map = Map
        ObjectHealth.Health = Health
        Map.SelectedUnitsAction(ObjectHealth)

        SelectedObject_Changed()
        Map.UndoStepCreate("Object Health Changed")
        View_DrawViewLater()
    End Sub

    Private Sub btnDroidToDesign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDroidToDesign.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change design of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        Else
            If MsgBox("Change design of a droid?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectTemplateToDesign As New clsMap.clsObjectTemplateToDesign
        ObjectTemplateToDesign.Map = Map
        Map.SelectedUnitsAction(ObjectTemplateToDesign)

        SelectedObject_Changed()
        If ObjectTemplateToDesign.ActionPerformed Then
            Map.UndoStepCreate("Object Template Removed")
            View_DrawViewLater()
        End If
    End Sub

    Public PasteRotateObjects As enumObjectRotateMode = enumObjectRotateMode.Walls

    Private Sub menuRotateUnits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuRotateUnits.Click

        PasteRotateObjects = enumObjectRotateMode.All
        menuRotateUnits.Checked = True
        menuRotateWalls.Checked = False
        menuRotateNothing.Checked = False
    End Sub

    Private Sub menuRotateWalls_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuRotateWalls.Click

        PasteRotateObjects = enumObjectRotateMode.Walls
        menuRotateUnits.Checked = False
        menuRotateWalls.Checked = True
        menuRotateNothing.Checked = False
    End Sub

    Private Sub menuRotateNothing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuRotateNothing.Click

        PasteRotateObjects = enumObjectRotateMode.None
        menuRotateUnits.Checked = False
        menuRotateWalls.Checked = False
        menuRotateNothing.Checked = True
    End Sub

    Private Sub cboDroidBody_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidBody.SelectedIndexChanged
        If Not cboDroidBody.Enabled Then
            Exit Sub
        End If
        If cboDroidBody.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change body of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectBody As New clsMap.clsObjectBody
        ObjectBody.Map = Map
        ObjectBody.Body = cboBody_Objects(cboDroidBody.SelectedIndex)
        Map.SelectedUnitsAction(ObjectBody)

        SelectedObject_Changed()
        If ObjectBody.ActionPerformed Then
            Map.UndoStepCreate("Object Body Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub cboDroidPropulsion_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidPropulsion.SelectedIndexChanged
        If Not cboDroidPropulsion.Enabled Then
            Exit Sub
        End If
        If cboDroidPropulsion.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change propulsion of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectPropulsion As New clsMap.clsObjectPropulsion
        ObjectPropulsion.Map = Map
        ObjectPropulsion.Propulsion = cboPropulsion_Objects(cboDroidPropulsion.SelectedIndex)
        Map.SelectedUnitsAction(ObjectPropulsion)

        SelectedObject_Changed()
        If ObjectPropulsion.ActionPerformed Then
            Map.UndoStepCreate("Object Body Changed")
            View_DrawViewLater()
        End If
        SelectedObject_Changed()
        If ObjectPropulsion.ActionPerformed Then
            Map.UndoStepCreate("Object Propulsion Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub cboDroidTurret1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidTurret1.SelectedIndexChanged
        If Not cboDroidTurret1.Enabled Then
            Exit Sub
        End If
        If cboDroidTurret1.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change turret of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectTurret As New clsMap.clsObjectTurret
        ObjectTurret.Map = Map
        ObjectTurret.Turret = cboTurret_Objects(cboDroidTurret1.SelectedIndex)
        ObjectTurret.TurretNum = 0
        Map.SelectedUnitsAction(ObjectTurret)

        SelectedObject_Changed()
        If ObjectTurret.ActionPerformed Then
            Map.UndoStepCreate("Object Turret Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub cboDroidTurret2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidTurret2.SelectedIndexChanged
        If Not cboDroidTurret2.Enabled Then
            Exit Sub
        End If
        If cboDroidTurret2.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change turret of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectTurret As New clsMap.clsObjectTurret
        ObjectTurret.Map = Map
        ObjectTurret.Turret = cboTurret_Objects(cboDroidTurret2.SelectedIndex)
        ObjectTurret.TurretNum = 1
        Map.SelectedUnitsAction(ObjectTurret)

        SelectedObject_Changed()
        If ObjectTurret.ActionPerformed Then
            Map.UndoStepCreate("Object Turret Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub cboDroidTurret3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidTurret3.SelectedIndexChanged
        If Not cboDroidTurret3.Enabled Then
            Exit Sub
        End If
        If cboDroidTurret3.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change turret of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim ObjectTurret As New clsMap.clsObjectTurret
        ObjectTurret.Map = Map
        ObjectTurret.Turret = cboTurret_Objects(cboDroidTurret3.SelectedIndex)
        ObjectTurret.TurretNum = 2
        Map.SelectedUnitsAction(ObjectTurret)

        SelectedObject_Changed()
        If ObjectTurret.ActionPerformed Then
            Map.UndoStepCreate("Object Turret Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub rdoDroidTurret0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret0.CheckedChanged
        If Not rdoDroidTurret0.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret0.Checked Then
            Exit Sub
        End If

        rdoDroidTurret1.Checked = False
        rdoDroidTurret2.Checked = False
        rdoDroidTurret3.Checked = False

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(0)
    End Sub

    Private Sub rdoDroidTurret1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret1.CheckedChanged
        If Not rdoDroidTurret1.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret1.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret2.Checked = False
        rdoDroidTurret3.Checked = False

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(1)
    End Sub

    Private Sub rdoDroidTurret2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret2.CheckedChanged
        If Not rdoDroidTurret2.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret2.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret1.Checked = False
        rdoDroidTurret3.Checked = False

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(2)
    End Sub

    Private Sub rdoDroidTurret3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret3.CheckedChanged
        If Not rdoDroidTurret2.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret3.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret1.Checked = False
        rdoDroidTurret2.Checked = False

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(3)
    End Sub

    Private Sub SelectedObjects_SetTurretCount(ByVal Count As Byte)

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim ObjectTurretCount As New clsMap.clsObjectTurretCount
        ObjectTurretCount.Map = Map
        ObjectTurretCount.TurretCount = Count
        Map.SelectedUnitsAction(ObjectTurretCount)

        SelectedObject_Changed()
        If ObjectTurretCount.ActionPerformed Then
            Map.UndoStepCreate("Object Number Of Turrets Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub SelectedObjects_SetDroidType(ByVal NewType As clsDroidDesign.clsTemplateDroidType)

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim ObjectDroidType As New clsMap.clsObjectDroidType
        ObjectDroidType.Map = Map
        ObjectDroidType.DroidType = NewType
        Map.SelectedUnitsAction(ObjectDroidType)

        SelectedObject_Changed()
        If ObjectDroidType.ActionPerformed Then
            Map.UndoStepCreate("Object Number Of Turrets Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub cboDroidType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDroidType.SelectedIndexChanged
        If Not cboDroidType.Enabled Then
            Exit Sub
        End If
        If cboDroidType.SelectedIndex < 0 Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <= 0 Then
            Exit Sub
        End If

        If Map.SelectedUnits.ItemCount > 1 Then
            If MsgBox("Change type of multiple droids?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle), "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetDroidType(TemplateDroidTypes(cboDroidType.SelectedIndex))
    End Sub

    Private Sub rdoTextureIgnoreTerrain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoTextureIgnoreTerrain.Click

        If rdoTextureIgnoreTerrain.Checked Then
            TextureTerrainAction = enumTextureTerrainAction.Ignore
            rdoTextureReinterpretTerrain.Checked = False
            rdoTextureRemoveTerrain.Checked = False
        End If
    End Sub

    Private Sub rdoTextureReinterpretTerrain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoTextureReinterpretTerrain.Click

        If rdoTextureReinterpretTerrain.Checked Then
            TextureTerrainAction = enumTextureTerrainAction.Reinterpret
            rdoTextureIgnoreTerrain.Checked = False
            rdoTextureRemoveTerrain.Checked = False
        End If
    End Sub

    Private Sub rdoTextureRemoveTerrain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoTextureRemoveTerrain.Click

        If rdoTextureRemoveTerrain.Checked Then
            TextureTerrainAction = enumTextureTerrainAction.Remove
            rdoTextureIgnoreTerrain.Checked = False
            rdoTextureReinterpretTerrain.Checked = False
        End If
    End Sub

    Private Sub btnPlayerSelectObjects_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayerSelectObjects.Click

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Not Control_Unit_Multiselect.Active Then
            Map.SelectedUnits.Clear()
        End If

        Dim tmpUnitGroup As clsMap.clsUnitGroup = Map.SelectedUnitGroup.Item
        Dim A As Integer

        For A = 0 To Map.Units.ItemCount - 1
            If Map.Units.Item(A).UnitGroup Is tmpUnitGroup Then
                If Not Map.Units.Item(A).MapSelectedUnitLink.IsConnected Then
                    Map.Units.Item(A).MapSelectedUnitLink.Connect(Map.SelectedUnits)
                End If
            End If
        Next

        View_DrawViewLater()
    End Sub

    Private Sub menuSaveFME_Click(sender As System.Object, e As System.EventArgs) Handles menuSaveFME.Click

        Save_FME_Prompt()
    End Sub

    Private Sub menuOptions_Click(sender As System.Object, e As System.EventArgs) Handles menuOptions.Click

        If frmOptionsInstance IsNot Nothing Then
            frmOptionsInstance.Activate()
            Exit Sub
        End If
        frmOptionsInstance = New frmOptions
        frmOptionsInstance.Show()
    End Sub

    Private Sub rdoCliffTriBrush_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoCliffTriBrush.CheckedChanged

        Tool = enumTool.CliffTriangle
    End Sub

    Private Sub LoadInterfaceImage(ByVal ImagePath As String, ByRef ResultBitmap As Bitmap, ByVal Result As clsResult)
        Dim tmpResult As sResult

        ResultBitmap = Nothing
        tmpResult = LoadBitmap(ImagePath, ResultBitmap)
        If Not tmpResult.Success Then
            Result.Warning_Add("Unable to load image " & ControlChars.Quote & ImagePath & ControlChars.Quote)
        End If
    End Sub

    Public Sub MainMapAfterChanged()

        Dim Map As clsMap = MainMap

        MapView.UpdateTabs()

        SelectedTerrain = Nothing
        SelectedRoad = Nothing

        Resize_Update()
        MainMapTilesetChanged()
        PainterTerrains_Refresh(-1, -1)
        ScriptMarkerLists_Update()

        NewPlayerNum.Enabled = False
        ObjectPlayerNum.Enabled = False
        If Map IsNot Nothing Then
            Map.CheckMessages()
            Map.ViewInfo.FOV_Calc()
            Map.SectorGraphicsChanges.SetAllChanged()
            Map.Update()
            Map.MinimapMakeLater()
            tsbSave.Enabled = Map.ChangedSinceSave
            NewPlayerNum.SetMap(Map)
            NewPlayerNum.Target = Map.SelectedUnitGroup
            ObjectPlayerNum.SetMap(Map)
            AddHandler MainMap.Changed, AddressOf MainMap_Modified
        Else
            tsbSave.Enabled = False
            NewPlayerNum.SetMap(Nothing)
            NewPlayerNum.Target = Nothing
            ObjectPlayerNum.SetMap(Nothing)
        End If
        NewPlayerNum.Enabled = True
        ObjectPlayerNum.Enabled = True

        SelectedObject_Changed()

        Title_Text_Update()

        TextureView.ScrollUpdate()

        TextureView.DrawViewLater()
        View_DrawViewLater()
    End Sub

    Public Sub MainMapBeforeChanged()
        Dim Map As clsMap = MainMap

        MapView.OpenGLControl.Focus() 'take focus from controls to trigger their lostfocuses

        If Map Is Nothing Then
            Exit Sub
        End If

        RemoveHandler MainMap.Changed, AddressOf MainMap_Modified

        If Map.ReadyForUserInput Then
            Map.SectorAll_GLLists_Delete()
            Map.SectorGraphicsChanges.SetAllChanged()
        End If
    End Sub

    Private Sub MainMap_Modified()

        tsbSave.Enabled = True
    End Sub

    Public Sub ObjectPicker(ByVal UnitType As clsUnitType)

        Tool = enumTool.ObjectPlace
        lstFeatures.SelectedIndex = -1
        lstStructures.SelectedIndex = -1
        lstDroids.SelectedIndex = -1
        SelectedObjectType = UnitType
        Dim Map As clsMap = MainMap
        If Map IsNot Nothing Then
            Map.MinimapMakeLater() 'for unit highlight
            View_DrawViewLater()
        End If
    End Sub

    Public Sub TerrainPicker()
        Dim Map As clsMap = MainMap

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Vertex As sXY_int = MouseOverTerrain.Vertex.Normal
        Dim A As Integer

        lstAutoTexture.Enabled = False
        For A = 0 To lstAutoTexture.Items.Count - 1
            If Map.Painter.Terrains(A) Is Map.Terrain.Vertices(Vertex.X, Vertex.Y).Terrain Then
                lstAutoTexture.SelectedIndex = A
                Exit For
            End If
        Next
        If A = lstAutoTexture.Items.Count Then
            lstAutoTexture.SelectedIndex = -1
        End If
        lstAutoTexture.Enabled = True
        SelectedTerrain = Map.Terrain.Vertices(Vertex.X, Vertex.Y).Terrain
    End Sub

    Public Sub TexturePicker()
        Dim Map As clsMap = MainMap

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        If Map.Tileset IsNot Nothing Then
            If Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.TextureNum < Map.Tileset.TileCount Then
                SelectedTextureNum = Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.TextureNum
                TextureView.DrawViewLater()
            End If
        End If
    End Sub

    Public Sub HeightPickerL()
        Dim Map As clsMap = MainMap

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        txtHeightSetL.Text = InvariantToString_byte(Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height)
        txtHeightSetL.Focus()
        MapView.OpenGLControl.Focus()
    End Sub

    Public Sub HeightPickerR()
        Dim Map As clsMap = MainMap

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()
        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        txtHeightSetR.Text = InvariantToString_byte(Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height)
        txtHeightSetR.Focus()
        MapView.OpenGLControl.Focus()
    End Sub

    Private Sub OpenGL_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub OpenGL_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop

        Dim Paths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim A As Integer
        Dim Result As New clsResult

        For A = 0 To Paths.GetUpperBound(0)
            Result.Append(LoadMap(Paths(A)), New sSplitPath(Paths(A)).FileTitle & ": ")
        Next
        ShowWarnings(Result, "Load Drag-dropped Maps")
    End Sub

    Private Sub btnFlatOil_Click(sender As System.Object, e As System.EventArgs) Handles btnFlatOil.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If UnitType_OilResource Is Nothing Then
            MsgBox("Unable. Oil resource is not loaded.")
            Exit Sub
        End If

        Dim A As Integer
        Dim tmpUnit As clsMap.clsUnit
        Dim OilList As New SimpleClassList(Of clsMap.clsUnit)
        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If tmpUnit.Type Is UnitType_OilResource Then
                OilList.Add(tmpUnit)
            End If
        Next
        Dim FlattenTool As New clsMap.clsObjectFlattenTerrain
        OilList.PerformTool(FlattenTool)

        Map.Update()
        Map.UndoStepCreate("Flatten Under Oil")
    End Sub

    Private Sub btnFlatStructures_Click(sender As Object, e As EventArgs) Handles btnFlatStructures.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim A As Integer
        Dim tmpUnit As clsMap.clsUnit
        Dim StructureList As New SimpleClassList(Of clsMap.clsUnit)
        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                StructureList.Add(tmpUnit)
            End If
        Next
        Dim FlattenTool As New clsMap.clsObjectFlattenTerrain
        StructureList.PerformTool(FlattenTool)

        Map.Update()
        Map.UndoStepCreate("Flatten Under Structures")
    End Sub

    Private Sub btnFlatSelected_Click(sender As Object, e As EventArgs) Handles btnFlatSelected.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim FlattenTool As New clsMap.clsObjectFlattenTerrain
        Map.SelectedUnits.GetItemsAsSimpleClassList.PerformTool(FlattenTool)

        Map.Update()
        Map.UndoStepCreate("Flatten Under Structures")
    End Sub

    Private Sub rdoFillCliffIgnore_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoFillCliffIgnore.CheckedChanged

        FillCliffAction = enumFillCliffAction.Ignore
    End Sub

    Private Sub rdoFillCliffStopBefore_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoFillCliffStopBefore.CheckedChanged

        FillCliffAction = enumFillCliffAction.StopBefore
    End Sub

    Private Sub rdoFillCliffStopAfter_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoFillCliffStopAfter.CheckedChanged

        FillCliffAction = enumFillCliffAction.StopAfter
    End Sub

    Private Sub btnScriptAreaCreate_Click(sender As System.Object, e As System.EventArgs) Handles btnScriptAreaCreate.Click
        If Not btnScriptAreaCreate.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If

        If Map.Selected_Area_VertexA Is Nothing Then
            MsgBox("Select something first.")
            Exit Sub
        End If
        If Map.Selected_Area_VertexB Is Nothing Then
            MsgBox("Select something first.")
            Exit Sub
        End If

        Dim NewArea As clsMap.clsScriptArea = clsMap.clsScriptArea.Create(Map)
        If NewArea Is Nothing Then
            MsgBox("Error: Creating area failed.")
            Exit Sub
        End If

        NewArea.SetPositions(New sXY_int(Map.Selected_Area_VertexA.X * TerrainGridSpacing, _
        Map.Selected_Area_VertexA.Y * TerrainGridSpacing), _
        New sXY_int(Map.Selected_Area_VertexB.X * TerrainGridSpacing, _
        Map.Selected_Area_VertexB.Y * TerrainGridSpacing))

        ScriptMarkerLists_Update()

        Map.SetChanged() 'todo: remove if areas become undoable
        View_DrawViewLater()
    End Sub

    Private lstScriptPositions_Objects(-1) As clsMap.clsScriptPosition
    Private lstScriptAreas_Objects(-1) As clsMap.clsScriptArea

    Public Sub ScriptMarkerLists_Update()
        Dim Map As clsMap = MainMap

        lstScriptPositions.Enabled = False
        lstScriptAreas.Enabled = False

        lstScriptPositions.Items.Clear()
        lstScriptAreas.Items.Clear()

        If Map Is Nothing Then
            _SelectedScriptMarker = Nothing
            Exit Sub
        End If

        Dim A As Integer
        Dim B As Integer
        Dim tmpPosition As clsMap.clsScriptPosition
        Dim tmpArea As clsMap.clsScriptArea
        Dim NewSelectedScriptMarker As Object = Nothing

        ReDim lstScriptPositions_Objects(Map.ScriptPositions.ItemCount - 1)
        ReDim lstScriptAreas_Objects(Map.ScriptAreas.ItemCount - 1)

        For A = 0 To Map.ScriptPositions.ItemCount - 1
            tmpPosition = Map.ScriptPositions.Item(A)
            B = lstScriptPositions.Items.Add(tmpPosition.Label)
            lstScriptPositions_Objects(B) = tmpPosition
            If tmpPosition Is _SelectedScriptMarker Then
                NewSelectedScriptMarker = tmpPosition
                lstScriptPositions.SelectedIndex = B
            End If
        Next

        For A = 0 To Map.ScriptAreas.ItemCount - 1
            tmpArea = Map.ScriptAreas.Item(A)
            B = lstScriptAreas.Items.Add(tmpArea.Label)
            lstScriptAreas_Objects(B) = tmpArea
            If tmpArea Is _SelectedScriptMarker Then
                NewSelectedScriptMarker = tmpArea
                lstScriptAreas.SelectedIndex = B
            End If
        Next

        lstScriptPositions.Enabled = True
        lstScriptAreas.Enabled = True

        _SelectedScriptMarker = NewSelectedScriptMarker

        SelectedScriptMarker_Update()
    End Sub

    Private _SelectedScriptMarker As Object
    Public ReadOnly Property SelectedScriptMarker As Object
        Get
            Return _SelectedScriptMarker
        End Get
    End Property

    Private Sub lstScriptPositions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lstScriptPositions.SelectedIndexChanged
        If Not lstScriptPositions.Enabled Then
            Exit Sub
        End If

        _SelectedScriptMarker = lstScriptPositions_Objects(lstScriptPositions.SelectedIndex)

        lstScriptAreas.Enabled = False
        lstScriptAreas.SelectedIndex = -1
        lstScriptAreas.Enabled = True

        SelectedScriptMarker_Update()

        View_DrawViewLater()
    End Sub

    Private Sub lstScriptAreas_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lstScriptAreas.SelectedIndexChanged
        If Not lstScriptAreas.Enabled Then
            Exit Sub
        End If

        _SelectedScriptMarker = lstScriptAreas_Objects(lstScriptAreas.SelectedIndex)

        lstScriptPositions.Enabled = False
        lstScriptPositions.SelectedIndex = -1
        lstScriptPositions.Enabled = True

        SelectedScriptMarker_Update()

        View_DrawViewLater()
    End Sub

    Public Sub SelectedScriptMarker_Update()

        txtScriptMarkerLabel.Enabled = False
        txtScriptMarkerX.Enabled = False
        txtScriptMarkerY.Enabled = False
        txtScriptMarkerX2.Enabled = False
        txtScriptMarkerY2.Enabled = False
        txtScriptMarkerLabel.Text = ""
        txtScriptMarkerX.Text = ""
        txtScriptMarkerY.Text = ""
        txtScriptMarkerX2.Text = ""
        txtScriptMarkerY2.Text = ""
        If _SelectedScriptMarker IsNot Nothing Then
            If TypeOf _SelectedScriptMarker Is clsMap.clsScriptPosition Then
                Dim tmpPosition As clsMap.clsScriptPosition = CType(_SelectedScriptMarker, clsMap.clsScriptPosition)
                txtScriptMarkerLabel.Text = tmpPosition.Label
                txtScriptMarkerX.Text = InvariantToString_int(tmpPosition.PosX)
                txtScriptMarkerY.Text = InvariantToString_int(tmpPosition.PosY)
                txtScriptMarkerLabel.Enabled = True
                txtScriptMarkerX.Enabled = True
                txtScriptMarkerY.Enabled = True
            ElseIf TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
                Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
                txtScriptMarkerLabel.Text = tmpArea.Label
                txtScriptMarkerX.Text = InvariantToString_int(tmpArea.PosAX)
                txtScriptMarkerY.Text = InvariantToString_int(tmpArea.PosAY)
                txtScriptMarkerX2.Text = InvariantToString_int(tmpArea.PosBX)
                txtScriptMarkerY2.Text = InvariantToString_int(tmpArea.PosBY)
                txtScriptMarkerLabel.Enabled = True
                txtScriptMarkerX.Enabled = True
                txtScriptMarkerY.Enabled = True
                txtScriptMarkerX2.Enabled = True
                txtScriptMarkerY2.Enabled = True
            End If
        End If
    End Sub

    Private Sub btnScriptMarkerRemove_Click(sender As System.Object, e As System.EventArgs) Handles btnScriptMarkerRemove.Click
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Number As Integer
        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptPosition Then
            Dim tmpPosition As clsMap.clsScriptPosition = CType(_SelectedScriptMarker, clsMap.clsScriptPosition)
            Number = tmpPosition.ParentMap.ArrayPosition
            tmpPosition.Deallocate()
            If Map.ScriptPositions.ItemCount > 0 Then
                _SelectedScriptMarker = Map.ScriptPositions.Item(Clamp_int(Number, 0, Map.ScriptPositions.ItemCount - 1))
            Else
                _SelectedScriptMarker = Nothing
            End If
        ElseIf TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            Number = tmpArea.ParentMap.ArrayPosition
            tmpArea.Deallocate()
            If Map.ScriptAreas.ItemCount > 0 Then
                _SelectedScriptMarker = Map.ScriptAreas.Item(Clamp_int(Number, 0, Map.ScriptAreas.ItemCount - 1))
            Else
                _SelectedScriptMarker = Nothing
            End If
        End If

        ScriptMarkerLists_Update()

        View_DrawViewLater()
    End Sub

    Private Sub txtScriptMarkerLabel_LostFocus(sender As System.Object, e As System.EventArgs) Handles txtScriptMarkerLabel.Leave
        If Not txtScriptMarkerLabel.Enabled Then
            Exit Sub
        End If
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        Dim Result As sResult
        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptPosition Then
            Dim tmpPosition As clsMap.clsScriptPosition = CType(_SelectedScriptMarker, clsMap.clsScriptPosition)
            If tmpPosition.Label = txtScriptMarkerLabel.Text Then
                Exit Sub
            End If
            Result = tmpPosition.SetLabel(txtScriptMarkerLabel.Text)
        ElseIf TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            If tmpArea.Label = txtScriptMarkerLabel.Text Then
                Exit Sub
            End If
            Result = tmpArea.SetLabel(txtScriptMarkerLabel.Text)
        Else
            Exit Sub
        End If

        If Not Result.Success Then
            MsgBox("Unable to change label: " & Result.Problem)
            SelectedScriptMarker_Update()
            Exit Sub
        End If

        ScriptMarkerLists_Update()
    End Sub

    Private Sub txtScriptMarkerX_LostFocus(sender As System.Object, e As System.EventArgs) Handles txtScriptMarkerX.Leave
        If Not txtScriptMarkerX.Enabled Then
            Exit Sub
        End If
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptPosition Then
            Dim tmpPosition As clsMap.clsScriptPosition = CType(_SelectedScriptMarker, clsMap.clsScriptPosition)
            InvariantParse_int(txtScriptMarkerX.Text, tmpPosition.PosX)
        ElseIf TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            InvariantParse_int(txtScriptMarkerX.Text, tmpArea.PosAX)
        Else
            MsgBox("Error: unhandled type.")
        End If

        SelectedScriptMarker_Update()
        View_DrawViewLater()
    End Sub

    Private Sub txtScriptMarkerY_LostFocus(sender As System.Object, e As System.EventArgs) Handles txtScriptMarkerY.Leave
        If Not txtScriptMarkerY.Enabled Then
            Exit Sub
        End If
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptPosition Then
            Dim tmpPosition As clsMap.clsScriptPosition = CType(_SelectedScriptMarker, clsMap.clsScriptPosition)
            InvariantParse_int(txtScriptMarkerY.Text, tmpPosition.PosY)
        ElseIf TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            InvariantParse_int(txtScriptMarkerY.Text, tmpArea.PosAY)
        Else
            MsgBox("Error: unhandled type.")
        End If

        SelectedScriptMarker_Update()
        View_DrawViewLater()
    End Sub

    Private Sub txtScriptMarkerX2_LostFocus(sender As System.Object, e As System.EventArgs) Handles txtScriptMarkerX2.Leave
        If Not txtScriptMarkerX2.Enabled Then
            Exit Sub
        End If
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            InvariantParse_int(txtScriptMarkerX2.Text, tmpArea.PosBX)
        Else
            MsgBox("Error: unhandled type.")
        End If

        SelectedScriptMarker_Update()
        View_DrawViewLater()
    End Sub

    Private Sub txtScriptMarkerY2_LostFocus(sender As System.Object, e As System.EventArgs) Handles txtScriptMarkerY2.Leave
        If Not txtScriptMarkerY2.Enabled Then
            Exit Sub
        End If
        If _SelectedScriptMarker Is Nothing Then
            Exit Sub
        End If

        If TypeOf _SelectedScriptMarker Is clsMap.clsScriptArea Then
            Dim tmpArea As clsMap.clsScriptArea = CType(_SelectedScriptMarker, clsMap.clsScriptArea)
            InvariantParse_int(txtScriptMarkerY2.Text, tmpArea.PosBY)
        Else
            MsgBox("Error: unhandled type.")
        End If

        SelectedScriptMarker_Update()
        View_DrawViewLater()
    End Sub

    Private Sub txtObjectLabel_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectLabel.Leave
        If Not txtObjectLabel.Enabled Then
            Exit Sub
        End If
        Dim Map As clsMap = MainMap
        If Map Is Nothing Then
            Exit Sub
        End If
        If Map.SelectedUnits.ItemCount <> 1 Then
            Exit Sub
        End If

        If txtObjectLabel.Text = Map.SelectedUnits.Item(0).Label Then
            Exit Sub
        End If

        Dim OldUnit As clsMap.clsUnit = Map.SelectedUnits.Item(0)
        Dim ResultUnit As clsMap.clsUnit = New clsMap.clsUnit(OldUnit, Map)
        Map.UnitSwap(OldUnit, ResultUnit)
        Dim Result As sResult = ResultUnit.SetLabel(txtObjectLabel.Text)
        If Not Result.Success Then
            MsgBox("Unable to set label: " & Result.Problem)
        End If

        Map.SelectedUnits.Clear()
        ResultUnit.MapSelect()

        SelectedObject_Changed()

        Map.UndoStepCreate("Object Label Changed")
        View_DrawViewLater()
    End Sub

    Public Sub NewMainMap(ByVal NewMap As clsMap)

        NewMap.frmMainLink.Connect(_LoadedMaps)
        SetMainMap(NewMap)
    End Sub

    Public ReadOnly Property MainMap As clsMap
        Get
            Return _LoadedMaps.MainMap
        End Get
    End Property

    Public ReadOnly Property LoadedMaps As clsMaps
        Get
            Return _LoadedMaps
        End Get
    End Property

    Public Sub SetMainMap(ByVal Map As clsMap)

        _LoadedMaps.MainMap = Map
    End Sub

    Public Function LoadMap(ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult
        Dim ResultB As sResult
        Dim SplitPath As New sSplitPath(Path)
        Dim ResultMap As New clsMap
        Dim Extension As String = SplitPath.FileExtension.ToLower

        Select Case Extension
            Case "fmap"
                ReturnResult.Append(ResultMap.Load_FMap(Path), "Load FMap: ")
                ResultMap.PathInfo = New clsMap.clsPathInfo(Path, True)
            Case "fme", "wzme"
                ReturnResult.Append(ResultMap.Load_FME(Path), "")
                ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
            Case "wz"
                ReturnResult.Append(ResultMap.Load_WZ(Path), "Load WZ: ")
                ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
            Case "gam"
                ReturnResult.Append(ResultMap.Load_Game(Path), "Load Game: ")
                ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
            Case "lnd"
                ResultB = ResultMap.Load_LND(Path)
                If Not ResultB.Success Then
                    ReturnResult.Problem_Add("Load LND: " & ResultB.Problem)
                End If
                ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
            Case Else
                ReturnResult.Problem_Add("File extension not recognised.")
        End Select

        If ReturnResult.HasProblems Then
            ResultMap.Deallocate()
        Else
            NewMainMap(ResultMap)
        End If

        Return ReturnResult
    End Function

    Public Sub Load_Autosave_Prompt()

        If Not IO.Directory.Exists(AutoSavePath) Then
            MsgBox("Autosave directory does not exist. There are no autosaves.", MsgBoxStyle.OkOnly, "")
            Exit Sub
        End If
        Dim Dialog As New OpenFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " Files (*.fmap, *.fme)|*.fmap;*.fme|All Files (*.*)|*.*"
        Dialog.InitialDirectory = AutoSavePath
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As clsResult = LoadMap(Dialog.FileName)
        ShowWarnings(Result, "Load Map")
    End Sub

    Private Sub btnAlignObjects_Click(sender As System.Object, e As System.EventArgs) Handles btnAlignObjects.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim AlignTool As New clsMap.clsObjectAlignment
        AlignTool.Map = Map
        Map.SelectedUnits.GetItemsAsSimpleClassList.PerformTool(AlignTool)

        Map.Update()
        Map.UndoStepCreate("Align Objects")
    End Sub
End Class