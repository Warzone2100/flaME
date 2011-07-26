Imports ICSharpCode.SharpZipLib

Partial Public Class frmMain
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Public View As ctrlMapView

    Public TextureView As ctrlTextureView

    Public lstFeatures_Objects() As clsUnitType
    Public lstStructures_Objects() As clsUnitType
    Public lstDroids_Objects() As clsUnitType

    Public cboBody_Objects() As clsBody
    Public cboPropulsion_Objects() As clsPropulsion
    Public cboTurret_Objects() As clsTurret

    Public HeightSetPalette(7) As Byte

    Public SelectedObjectType As clsUnitType

    Public WithEvents NewPlayerNum As ctrlPlayerNum
    Public WithEvents ObjectPlayerNum As ctrlPlayerNum

    Public WithEvents ctrlTextureBrush As ctrlBrush
    Public WithEvents ctrlTerrainBrush As ctrlBrush
    Public WithEvents ctrlCliffRemoveBrush As ctrlBrush
    Public WithEvents ctrlHeightBrush As ctrlBrush

    Public Sub New()

        Select Case Environment.OSVersion.Platform
            Case PlatformID.Unix
                OSPathSeperator = "/"c
            Case PlatformID.MacOSX
                OSPathSeperator = "/"c
            Case PlatformID.Win32NT
                OSPathSeperator = "\"c
            Case PlatformID.Win32S
                OSPathSeperator = "\"c
            Case PlatformID.Win32Windows
                OSPathSeperator = "\"c
            Case PlatformID.WinCE
                OSPathSeperator = "\"c
            Case Else
                OSPathSeperator = "\"c
        End Select

        'these depend on ospathseperator
        SetProgramSubDirs()
        SetDataSubDirs()

        LoadInterfaceIcons()

        InitializeComponent() 'required for monodevelop too, depends on subdirs being set

        NewPlayerNum = New ctrlPlayerNum
        ObjectPlayerNum = New ctrlPlayerNum

        View = New ctrlMapView
        TextureView = New ctrlTextureView
    End Sub

#If MonoDevelop <> 0.0# Then
    Public InterfaceImage_DisplayAutoTexture As Bitmap
    Public InterfaceImage_DrawTileOrientation As Bitmap
    Public InterfaceImage_QuickSave As Bitmap
    Public InterfaceImage_Selection As Bitmap
    Public InterfaceImage_ObjectSelect As Bitmap
    Public InterfaceImage_SelectionCopy As Bitmap
    Public InterfaceImage_SelectionFlipX As Bitmap
    Public InterfaceImage_SelectionRotateClockwise As Bitmap
    Public InterfaceImage_SelectionRotateCounterClockwise As Bitmap
    Public InterfaceImage_SelectionPaste As Bitmap
    Public InterfaceImage_SelectionPasteOptions As Bitmap
    Public InterfaceImage_Gateways As Bitmap
#End If

    Private Sub LoadInterfaceIcons()

#If MonoDevelop <> 0.0# Then
        LoadBitmap(InterfaceImagesPath & "displayautotexture.png",InterfaceImage_DisplayAutoTexture )
        LoadBitmap(InterfaceImagesPath & "drawtileorientation.png", InterfaceImage_DrawTileOrientation)
        LoadBitmap(InterfaceImagesPath & "save.png", InterfaceImage_QuickSave)
        LoadBitmap(InterfaceImagesPath & "selection.png", InterfaceImage_Selection)
        LoadBitmap(InterfaceImagesPath & "objectsselect.png", InterfaceImage_ObjectSelect)
        LoadBitmap(InterfaceImagesPath & "selectioncopy.png", InterfaceImage_SelectionCopy)
        LoadBitmap(InterfaceImagesPath & "selectionflipx.png", InterfaceImage_SelectionFlipX)
        LoadBitmap(InterfaceImagesPath & "selectionrotateclockwise.png", InterfaceImage_SelectionRotateClockwise)
        LoadBitmap(InterfaceImagesPath & "selectionrotateanticlockwise.png", InterfaceImage_SelectionRotateCounterClockwise)
        LoadBitmap(InterfaceImagesPath & "selectionpaste.png", InterfaceImage_SelectionPaste)
        LoadBitmap(InterfaceImagesPath & "selectionpasteoptions.png", InterfaceImage_SelectionPasteOptions)
        LoadBitmap(InterfaceImagesPath & "gateways.png", InterfaceImage_Gateways)
#End If
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If LoseMapQuestion() Then
            Dim Result As clsResult = Settings_Write()
        Else
            e.Cancel = True
        End If
    End Sub

    Private InitializeDone As Boolean = False

    Public Sub Initialize(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If InitializeDone Then
            Exit Sub
        End If
        If Not (View.IsGLInitialized And TextureView.IsGLInitialized) Then
            Exit Sub
        End If

        InitializeDone = True

        InitializeDelay.Enabled = False
        RemoveHandler InitializeDelay.Tick, AddressOf Initialize
        InitializeDelay.Dispose()
        InitializeDelay = Nothing

        NewPlayerNum.Left = 112
        NewPlayerNum.Top = 10
        Panel1.Controls.Add(NewPlayerNum)

        ObjectPlayerNum.Left = 72
        ObjectPlayerNum.Top = 60
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

        PlayerColour(0).Red = 0.0F
        PlayerColour(0).Green = 96.0F / 255.0F
        PlayerColour(0).Blue = 0.0F
        PlayerColour(1).Red = 160.0F / 255.0F
        PlayerColour(1).Green = 112.0F / 255.0F
        PlayerColour(1).Blue = 0.0F
        PlayerColour(2).Red = 128.0F / 255.0F
        PlayerColour(2).Green = 128.0F / 255.0F
        PlayerColour(2).Blue = 128.0F / 255.0F
        PlayerColour(3).Red = 0.0F
        PlayerColour(3).Green = 0.0F
        PlayerColour(3).Blue = 0.0F
        PlayerColour(4).Red = 128.0F / 255.0F
        PlayerColour(4).Green = 0.0F
        PlayerColour(4).Blue = 0.0F
        PlayerColour(5).Red = 32.0F / 255.0F
        PlayerColour(5).Green = 48.0F / 255.0F
        PlayerColour(5).Blue = 96.0F / 255.0F
        PlayerColour(6).Red = 144.0F / 255.0F
        PlayerColour(6).Green = 0.0F
        PlayerColour(6).Blue = 112 / 255.0F
        PlayerColour(7).Red = 0.0F
        PlayerColour(7).Green = 128.0F / 255.0F
        PlayerColour(7).Blue = 128.0F / 255.0F
        PlayerColour(8).Red = 128.0F / 255.0F
        PlayerColour(8).Green = 192.0F / 255.0F
        PlayerColour(8).Blue = 0.0F
        PlayerColour(9).Red = 176.0F / 255.0F
        PlayerColour(9).Green = 112.0F / 255.0F
        PlayerColour(9).Blue = 112.0F / 255.0F
        PlayerColour(10).Red = 224.0F / 255.0F
        PlayerColour(10).Green = 224.0F / 255.0F
        PlayerColour(10).Blue = 224.0F / 255.0F
        PlayerColour(11).Red = 32.0F / 255.0F
        PlayerColour(11).Green = 32.0F / 255.0F
        PlayerColour(11).Blue = 255.0F / 255.0F
        PlayerColour(12).Red = 0.0F
        PlayerColour(12).Green = 160.0F / 255.0F
        PlayerColour(12).Blue = 0.0F
        PlayerColour(13).Red = 64.0F / 255.0F
        PlayerColour(13).Green = 0.0F
        PlayerColour(13).Blue = 0.0F
        PlayerColour(14).Red = 16.0F / 255.0F
        PlayerColour(14).Green = 0.0F
        PlayerColour(14).Blue = 64.0F / 255.0F
        PlayerColour(15).Red = 64.0F / 255.0F
        PlayerColour(15).Green = 96.0F / 255.0F
        PlayerColour(15).Blue = 0.0F

        View.BGColor.Red = 0.75F
        View.BGColor.Green = 0.5F
        View.BGColor.Blue = 0.25F

        Dim SettingsLoadResult As clsResult = Settings_Load()
        InitializeResult.AppendAsWarning(SettingsLoadResult, "Load settings: ")

        If menuDirectoriesPrompt.Checked Then
            If frmDataInstance.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                End
            End If
        End If

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

        Main_Map = New clsMap(1, 1)

        SetMenuPointerModeChecked()

        Objects_Update()

        SelectedObject_Changed()
        Terrain_Selection_Changed()

        View.Dock = DockStyle.Fill
        pnlView.Controls.Add(View)

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
        Dim A As Integer
        For A = 0 To 7
            tabHeightSetL.TabPages(A).Text = HeightSetPalette(A)
            tabHeightSetR.TabPages(A).Text = HeightSetPalette(A)
        Next
        tabHeightSetL.SelectedIndex = 1
        tabHeightSetR.SelectedIndex = 0
        tabHeightSetL_SelectedIndexChanged(Nothing, Nothing)
        tabHeightSetR_SelectedIndexChanged(Nothing, Nothing)

        View.ViewAngleSetToDefault()

        View.ViewPos.Y = 3072

        NewMap()
        Main_Map.RandomizeTileOrientations()
        Main_Map.ShadowSector_CreateAll()
        Main_Map.SectorGraphicsChange.Update_Graphics()
        If My.Application.CommandLineArgs.Count >= 1 Then
            If Load_MainMap(My.Application.CommandLineArgs(0)) Then
                Main_Map.MinimapMakeLater()
            End If
        End If

        View.DrawView_SetEnabled(True)

        TextureView.Dock = DockStyle.Fill
        TableLayoutPanel6.Controls.Add(TextureView, 0, 1)

        Tool = enumTool.Texture_Brush

        TextureView.DrawView_SetEnabled(True)

        WindowState = FormWindowState.Maximized
#If MonoDevelop = 0.0# Then
        frmSplashInstance.Hide()
        Show()
#End If

        ShowWarnings(InitializeResult, "Startup Result")
    End Sub

    Private InitializeDelay As Timer

    Private InitializeResult As New clsResult

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#If MonoDevelop = 0.0# Then
        Hide()

        Try
            ProgramIcon = New Icon(My.Application.Info.DirectoryPath & OSPathSeperator & "flaME.ico")
        Catch ex As Exception
            InitializeResult.Warning_Add(ProgramName & " icon is missing: " & ex.Message)
        End Try
        Icon = ProgramIcon
        frmCompileInstance.Icon = ProgramIcon
        frmMapTexturerInstance.Icon = ProgramIcon
        frmGeneratorInstance.Icon = ProgramIcon
        frmSplashInstance.Icon = ProgramIcon
        frmDataInstance.Icon = ProgramIcon

        frmSplashInstance.Show()
#End If

        InitializeDelay = New Timer
        AddHandler InitializeDelay.Tick, AddressOf Initialize
        InitializeDelay.Interval = 50
        InitializeDelay.Enabled = True
    End Sub

    Private Sub Me_LostFocus(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.LostFocus

        ViewKeyDown_Clear()
    End Sub

    Private Sub tmrKey_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrKey.Tick
        If Not InitializeDone Then
            Exit Sub
        End If
        If View Is Nothing Then
            Exit Sub
        End If

        Dim XYZ_dbl As sXYZ_dbl
        Dim Rate As Double
        Dim Move As Double
        Dim Zoom As Double
        Dim PanRate As Double
        Dim AnglePY As sAnglePY
        Dim matrixA(8) As Double
        Dim matrixB(8) As Double
        Dim ViewAngleChange As sXYZ_dbl
        Dim ViewPosChange As sXYZ_int
        Dim AngleChanged As Boolean

        Rate = Rate_Get()

        Zoom = tmrKey.Interval * Rate * 0.002#
        Move = tmrKey.Interval * Rate * View.FOVMultiplier * (View.GLSize.X + View.GLSize.Y) * Math.Max(Math.Abs(View.ViewPos.Y), 512.0#) / 2048.0#

        If Control_View_Zoom_In.Active Then
            View.FOV_Scale_2E_Change(-Zoom)
        End If
        If Control_View_Zoom_Out.Active Then
            View.FOV_Scale_2E_Change(Zoom)
        End If

        If View.ViewMoveType = ctrlMapView.enumView_Move_Type.Free Then
            ViewPosChange = New sXYZ_int
            If Control_View_Move_Forward.Active Then
                VectorForwardRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Backward.Active Then
                VectorBackwardRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Left.Active Then
                VectorLeftRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Right.Active Then
                VectorRightRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Up.Active Then
                VectorUpRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Down.Active Then
                VectorDownRotationByMatrix(View.ViewAngleMatrix, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If

            ViewAngleChange = New sXYZ_dbl
            PanRate = View.FieldOfViewY / 16.0# * Rate
            If Control_View_Left.Active Then
                VectorForwardRotationByMatrix(View.ViewAngleMatrix, Rate * 5.0# * RadOf1Deg, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Right.Active Then
                VectorBackwardRotationByMatrix(View.ViewAngleMatrix, Rate * 5.0# * RadOf1Deg, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Backward.Active Then
                VectorLeftRotationByMatrix(View.ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Forward.Active Then
                VectorRightRotationByMatrix(View.ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Roll_Left.Active Then
                VectorDownRotationByMatrix(View.ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Roll_Right.Active Then
                VectorUpRotationByMatrix(View.ViewAngleMatrix, PanRate, XYZ_dbl)
                ViewAngleChange.X += XYZ_dbl.X
                ViewAngleChange.Y += XYZ_dbl.Y
                ViewAngleChange.Z += XYZ_dbl.Z
            End If

            If ViewPosChange.X <> 0.0# Or ViewPosChange.Y <> 0.0# Or ViewPosChange.Z <> 0.0# Then
                View.ViewPosChange(ViewPosChange)
            End If
            'do rotation
            If ViewAngleChange.X <> 0.0# Or ViewAngleChange.Y <> 0.0# Or ViewAngleChange.Z <> 0.0# Then
                GetAnglePY(ViewAngleChange, AnglePY)
                MatrixSetToPY(matrixA, AnglePY)
                MatrixRotationAroundAxis(View.ViewAngleMatrix, matrixA, GetDist(ViewAngleChange), matrixB)
                View.ViewAngleSet_Rotate(matrixB)
            End If
        ElseIf View.ViewMoveType = ctrlMapView.enumView_Move_Type.RTS Then
            ViewPosChange = New sXYZ_int

            MatrixToPY(View.ViewAngleMatrix, AnglePY)
            MatrixSetToYAngle(matrixA, AnglePY.Yaw)
            If Control_View_Move_Forward.Active Then
                VectorForwardRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Backward.Active Then
                VectorBackwardRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Left.Active Then
                VectorLeftRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Right.Active Then
                VectorRightRotationByMatrix(matrixA, Move, XYZ_dbl)
                ViewPosChange.X += XYZ_dbl.X
                ViewPosChange.Y += XYZ_dbl.Y
                ViewPosChange.Z += XYZ_dbl.Z
            End If
            If Control_View_Move_Up.Active Then
                ViewPosChange.Y += Move
            End If
            If Control_View_Move_Down.Active Then
                ViewPosChange.Y -= Move
            End If

            AngleChanged = False

            If View.RTSOrbit Then
                PanRate = Rate / 32.0#
            Else
                PanRate = View.FieldOfViewY / 16.0# * Rate
            End If
            If View.RTSOrbit Then
                If Control_View_Forward.Active Then
                    AnglePY.Pitch = Clamp(AnglePY.Pitch + PanRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If Control_View_Backward.Active Then
                    AnglePY.Pitch = Clamp(AnglePY.Pitch - PanRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If Control_View_Left.Active Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw + PanRate)
                    AngleChanged = True
                End If
                If Control_View_Right.Active Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw - PanRate)
                    AngleChanged = True
                End If
            Else
                If Control_View_Forward.Active Then
                    AnglePY.Pitch = Clamp(AnglePY.Pitch - PanRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If Control_View_Backward.Active Then
                    AnglePY.Pitch = Clamp(AnglePY.Pitch + PanRate, -RadOf90Deg + 0.03125# * RadOf1Deg, RadOf90Deg - 0.03125# * RadOf1Deg)
                    AngleChanged = True
                End If
                If Control_View_Left.Active Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw - PanRate)
                    AngleChanged = True
                End If
                If Control_View_Right.Active Then
                    AnglePY.Yaw = AngleClamp(AnglePY.Yaw + PanRate)
                    AngleChanged = True
                End If
            End If

            'Dim HeightChange As Double
            'HeightChange = Map.Terrain_Height_Get(view.View_Pos.X + ViewPosChange.X, view.View_Pos.Z + ViewPosChange.Z) - Map.Terrain_Height_Get(view.View_Pos.X, view.View_Pos.Z)

            'ViewPosChange.Y = ViewPosChange.Y + HeightChange

            If ViewPosChange.X <> 0.0# Or ViewPosChange.Y <> 0.0# Or ViewPosChange.Z <> 0.0# Then
                View.ViewPosChange(ViewPosChange)
            End If
            If AngleChanged Then
                MatrixSetToPY(matrixA, AnglePY)
                View.ViewAngleSet_Rotate(matrixA)
            End If
        End If
    End Sub

    Sub Controls_Set_Default()
        Dim A As Integer

        For A = 0 To InputControlCount - 1
            InputControls(A).SetToDefault()
        Next
    End Sub

    Function Rate_Get() As Double

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

        Control_Anticlockwise = InputControl_Create()
        With Control_Anticlockwise
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

        Controls_Set_Default()
    End Sub

    Function InputControl_Create() As clsInputControl

        ReDim Preserve InputControls(InputControlCount)
        InputControls(InputControlCount) = New clsInputControl
        InputControl_Create = InputControls(InputControlCount)
        InputControlCount += 1
    End Function

    Private Function Load_MainMap(ByVal Path As String) As Boolean
        Dim NewMap As clsMap = Nothing
        Dim Result As New clsResult
        Dim InterfaceOptions As clsMap.clsInterfaceOptions = Nothing

        Result = Load_Map(Path, NewMap, InterfaceOptions)

        If Result.HasProblems Then
            If NewMap IsNot Nothing Then
                NewMap.Deallocate()
            End If
        Else
            Main_Map.Deallocate()

            Main_Map = NewMap

            If InterfaceOptions Is Nothing Then
                InterfaceOptions = New clsMap.clsInterfaceOptions
            End If
            Map_Changed(InterfaceOptions)
        End If
        ShowWarnings(Result, "Load Map")
        Return Not Result.HasProblems
    End Function

    Public Sub Map_Changed(ByVal InterfaceOptions As clsMap.clsInterfaceOptions)

        Resize_Update()
        Map_Changed_Tileset()
        PainterTerrains_Refresh(-1, -1)

        Main_Map.SectorAll_GL_Update()
        View.ViewAngleSetToDefault()
        View.LookAtTile(New sXY_int(Math.Floor(Main_Map.TerrainSize.X / 2.0#), Math.Floor(Main_Map.TerrainSize.Y / 2.0#)))

        tsbSave.Enabled = False
        SetInterface(InterfaceOptions)
        NewPlayerNum.SetButtonUnitGroups(Main_Map)
        ObjectPlayerNum.SetButtonUnitGroups(Main_Map)
        Title_Text_Update()

        TextureView.ScrollUpdate()

        TextureView.DrawViewLater()
        View_DrawViewLater()
    End Sub

    Private Function Load_Map(ByVal Path As String, ByRef ResultMap As clsMap, ByRef InterfaceOptions As clsMap.clsInterfaceOptions) As clsResult
        Load_Map = New clsResult

        Dim SplitPath As New sSplitPath(Path)
        Dim Result As sResult
        Dim ReturnResult As New clsResult

        ResultMap = New clsMap

        If SplitPath.FileExtension = "fmap" Then
            ReturnResult.Append(ResultMap.Load_FMap(Path, InterfaceOptions), "Load FMap: ")
            ResultMap.PathInfo = New clsMap.clsPathInfo(Path, True)
        ElseIf SplitPath.FileExtension = "fme" Or SplitPath.FileExtension = "wzme" Then
            ReturnResult.Append(ResultMap.Load_FME(Path, InterfaceOptions), "")
            ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
        ElseIf SplitPath.FileExtension = "wz" Then
            ReturnResult.Append(ResultMap.Load_WZ(Path), "Load WZ: ")
            ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
        ElseIf SplitPath.FileExtension = "lnd" Then
            Result = ResultMap.Load_LND(Path)
            If Not Result.Success Then
                ReturnResult.Problem_Add("Load LND: " & Result.Problem)
            End If
            ResultMap.PathInfo = New clsMap.clsPathInfo(Path, False)
        Else
            ReturnResult.Problem_Add("File extension not recognised.")
        End If

        Return ReturnResult
    End Function

    Function Load_Map_As_Copy(ByVal Path As String) As clsResult
        Dim NewMap As clsMap = Nothing
        Dim Result As clsResult

        Result = Load_Map(Path, NewMap, Nothing)

        If Result.HasProblems Then
            If NewMap IsNot Nothing Then
                NewMap.Deallocate()
            End If
        Else
            If Copied_Map IsNot Nothing Then
                Copied_Map.Deallocate()
                Copied_Map = Nothing
            End If

            Copied_Map = NewMap
        End If

        Return Result
    End Function

    Sub Load_Map_Prompt()

        If LoseMapQuestion() Then
            Dim Dialog As New OpenFileDialog
            Dialog.InitialDirectory = Main_Map.GetDirectory
            Dialog.FileName = ""
            Dialog.Filter = "Warzone Map Files (*.fmap, *.fme, *.wz, *.lnd)|*.fmap;*.fme;*.wz;*.lnd|All Files (*.*)|*.*"
            If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
                Exit Sub
            End If
            Load_MainMap(Dialog.FileName)
        End If
    End Sub

    Sub Load_Heightmap_Prompt()
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

            Main_Map.Terrain_Resize(0, 0, HeightmapBitmap.Width - 1, HeightmapBitmap.Height - 1)
            Dim X As Integer
            Dim Y As Integer
            Dim PixelColor As Color

            For Y = 0 To HeightmapBitmap.Height - 1
                For X = 0 To HeightmapBitmap.Width - 1
                    PixelColor = HeightmapBitmap.GetPixel(X, Y)
                    Main_Map.TerrainVertex(X, Y).Height = Int((CInt(PixelColor.R) + PixelColor.G + PixelColor.B) / 3.0# + 0.5#)
                Next
            Next
            Main_Map.UnitHeight_Update_All()
            Main_Map.SectorAll_GL_Update()
            Main_Map.SectorAll_Set_Changed()

            Main_Map.UndoStepCreate("Import Heightmap")

            Resize_Update()

            View_DrawViewLater()
            Exit Sub
        Else
            MsgBox("Failed to load image: " & Result.Problem)
        End If
Error_Exit:
    End Sub

    Sub Load_TTP_Prompt()
        Dim Dialog As New OpenFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "TTP Files (*.ttp)|*.ttp|All Files (*.*)|*.*"
        If Not Dialog.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult = Main_Map.Load_TTP(Dialog.FileName)
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

    Sub Map_Changed_Tileset()
        Dim A As Integer

        For A = 0 To TilesetCount - 1
            If Tilesets(A) Is Main_Map.Tileset Then
                Exit For
            End If
        Next
        If A = TilesetCount Then
            cboTileset.SelectedIndex = -1
        Else
            cboTileset.SelectedIndex = A
        End If

        SetBackgroundColour()
    End Sub

    Public Sub SetBackgroundColour()

        If Main_Map.Tileset Is Tileset_Arizona Then
            View.BGColor.Red = 204.0# / 255.0#
            View.BGColor.Green = 149.0# / 255.0#
            View.BGColor.Blue = 70.0# / 255.0#
        ElseIf Main_Map.Tileset Is Tileset_Urban Then
            View.BGColor.Red = 118.0# / 255.0#
            View.BGColor.Green = 165.0# / 255.0#
            View.BGColor.Blue = 203.0# / 255.0#
        ElseIf Main_Map.Tileset Is Tileset_Rockies Then
            View.BGColor.Red = 182.0# / 255.0#
            View.BGColor.Green = 225.0# / 255.0#
            View.BGColor.Blue = 236.0# / 255.0#
        Else
            View.BGColor.Red = 0.5F
            View.BGColor.Green = 0.5F
            View.BGColor.Blue = 0.5F
        End If
    End Sub

    Private Sub cboTileset_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTileset.SelectedIndexChanged
        Dim NewTileset As clsTileset

        If cboTileset.SelectedIndex < 0 Then
            NewTileset = Nothing
        Else
            NewTileset = Tilesets(cboTileset.SelectedIndex)
        End If
        If NewTileset IsNot Main_Map.Tileset Then
            Main_Map.Tileset = NewTileset
            If Main_Map.Tileset IsNot Nothing Then
                SelectedTextureNum = Math.Min(0, Main_Map.Tileset.TileCount - 1)
            End If
            Main_Map.TileType_Reset()

            Main_Map.SetPainterToDefaults()
            PainterTerrains_Refresh(-1, -1)

            SetBackgroundColour()

            Main_Map.SectorAll_GL_Update()
            View_DrawViewLater()
            TextureView.ScrollUpdate()
            TextureView.DrawViewLater()
        End If
    End Sub

    Sub PainterTerrains_Refresh(ByVal Terrain_NewSelectedIndex As Integer, ByVal Road_NewSelectedIndex As Integer)

        lstAutoTexture.Items.Clear()
        lstAutoRoad.Items.Clear()
        Dim A As Integer
        For A = 0 To Main_Map.Painter.TerrainCount - 1
            lstAutoTexture.Items.Add(Main_Map.Painter.Terrains(A).Name)
        Next
        For A = 0 To Main_Map.Painter.RoadCount - 1
            lstAutoRoad.Items.Add(Main_Map.Painter.Roads(A).Name)
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
            ElseIf rdoAutoCliffBrush.Checked Or rdoAutoCliffRemove.Checked Then
                Tool = enumTool.AutoCliff
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

        Dim X As Integer
        Dim Y As Integer

        Dim difA As Double
        Dim difB As Double

        Dim NewTri As Boolean

        'tri set to the direction where the diagonal edge will be the flattest, so that cliff edges are level
        For Y = 0 To Main_Map.TerrainSize.Y - 1
            For X = 0 To Main_Map.TerrainSize.X - 1
                difA = Math.Abs(CDbl(Main_Map.TerrainVertex(X + 1, Y + 1).Height) - Main_Map.TerrainVertex(X, Y).Height)
                difB = Math.Abs(CDbl(Main_Map.TerrainVertex(X, Y + 1).Height) - Main_Map.TerrainVertex(X + 1, Y).Height)
                If difA = difB Then
                    If Rnd() >= 0.5F Then
                        NewTri = False
                    Else
                        NewTri = True
                    End If
                ElseIf difA < difB Then
                    NewTri = False
                Else
                    NewTri = True
                End If
                If Not Main_Map.TerrainTiles(X, Y).Tri = NewTri Then
                    Main_Map.TerrainTiles(X, Y).Tri = NewTri
                End If
            Next X
        Next Y

        Main_Map.SectorAll_Set_Changed()
        Main_Map.SectorAll_GL_Update()

        Main_Map.UndoStepCreate("Set All Triangles")

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
        If Not InitializeDone Then
            Exit Sub
        End If

        If View IsNot Nothing Then
            If Tool = enumTool.Height_Smooth_Brush Then
                If View.GetMouseOverTerrain IsNot Nothing Then
                    If View.MouseLeftIsDown Then
                        View.Apply_HeightSmoothing(Clamp(Val(txtSmoothRate.Text) * tmrTool.Interval / 1000.0#, 0.0#, 1.0#))
                    End If
                End If
            ElseIf Tool = enumTool.Height_Change_Brush Then
                If View.GetMouseOverTerrain IsNot Nothing Then
                    If View.MouseLeftIsDown Then
                        View.Apply_Height_Change(Clamp(Val(txtHeightChangeRate.Text), -255.0#, 255.0#), cbxHeightChangeFade.Checked)
                    ElseIf View.MouseRightIsDown Then
                        View.Apply_Height_Change(Clamp(-Val(txtHeightChangeRate.Text), -255.0#, 255.0#), cbxHeightChangeFade.Checked)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub btnResize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResize.Click
        Dim NewSize As sXY_int
        Dim Offset As sXY_int
        Dim Max As Double = MapMaxSize

        NewSize.X = Clamp(Val(txtSizeX.Text), 1.0#, Max)
        NewSize.Y = Clamp(Val(txtSizeY.Text), 1.0#, Max)
        Offset.X = Clamp(Val(txtOffsetX.Text), -Max, Max)
        Offset.Y = Clamp(Val(txtOffsetY.Text), -Max, Max)

        Map_Resize(Offset, NewSize)
    End Sub

    Public Sub Map_Resize(ByVal Offset As sXY_int, ByVal NewSize As sXY_int)

        If MsgBox("Resizing can't be undone. Continue?", vbOKCancel + vbQuestion, "") <> MsgBoxResult.Ok Then
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

        View.MouseOver = Nothing
        Main_Map.Terrain_Resize(Offset.X, Offset.Y, NewSize.X, NewSize.Y)

        Resize_Update()

        Main_Map.SectorAll_Set_Changed()
        Main_Map.SectorAll_GL_Update()

        View_DrawViewLater()
    End Sub

    Sub Resize_Update()

        txtSizeX.Text = Main_Map.TerrainSize.X
        txtSizeY.Text = Main_Map.TerrainSize.Y
        txtOffsetX.Text = "0"
        txtOffsetY.Text = "0"

        frmMapTexturerInstance.Map_Size_Refresh()
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

    Sub Save_LND_Prompt()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Editworld Files (*.lnd)|*.lnd"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim Result As clsResult
        Result = Main_Map.Write_LND(Dialog.FileName, True)

        ShowWarnings(Result, "Save LND")
    End Sub

    Private Sub Save_FMap_Prompt()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = Main_Map.GetDirectory
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
        Result = Main_Map.Write_FMap(Dialog.FileName, True, True)
        If Not Result.HasProblems Then
            Main_Map.PathInfo = New clsMap.clsPathInfo(Dialog.FileName, True)
            tsbSave.Enabled = False
            Title_Text_Update()
        End If
        ShowWarnings(Result, "Save FMap")
    End Sub

    Private Sub Save_FME_Prompt()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " FME Map Files (*.fme)|*.fme"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim strScavenger As String = InputBox("Enter the player number for scavenger units:")
        Dim ScavengerNum As Byte = CByte(Clamp(Val(strScavenger), CDbl(Byte.MinValue), 10.0#))
        Dim Result As sResult
        Result = Main_Map.Write_FME(Dialog.FileName, True, ScavengerNum)
        If Not Result.Success Then
            MsgBox("Unable to save FME: " & Result.Problem)
        End If
    End Sub

    'Public Function PromptFileFMEVersion(ByVal Path As String) As Boolean

    '    Dim File As New clsReadFile
    '    If Not File.Begin(Path).Success Then
    '        Return True
    '    End If
    '    Dim GotVersion As Boolean
    '    Dim Version As UInteger
    '    GotVersion = File.Get_U32(Version)
    '    File.Close()
    '    If GotVersion Then
    '        If Version <> SaveVersion Then
    '            If MsgBox("The file you are overwriting uses a different format to this version of " & ProgramName & ". You may only be able to open the file in this version if you continue. Do you want to overwrite?", vbOKCancel + vbQuestion, ProgramName) <> MsgBoxResult.Ok Then
    '                Return False
    '            End If
    '        End If
    '    End If
    '    Return True
    'End Function

    Sub Save_FMap_Quick()

        If Main_Map.PathInfo Is Nothing Then
            Save_FMap_Prompt()
        ElseIf Main_Map.PathInfo.IsFMap Then
            'If Not PromptFileFMEVersion(Map.LastFMESaveInfo.Path) Then
            '    Save_FME_Prompt()
            '    Exit Sub
            'End If

            Dim Result As clsResult = Main_Map.Write_FMap(Main_Map.PathInfo.Path, True, True)
            If Not Result.HasProblems Then
                tsbSave.Enabled = False
            End If
            ShowWarnings(Result, "Quick Save FMap")
        Else
            Save_FMap_Prompt()
        End If
    End Sub

    Sub Compile_Map_Prompt()

        frmCompileInstance.Show()
        frmCompileInstance.Activate()
    End Sub

    Sub PromptSave_Minimap()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Bitmap File (*.bmp)|*.bmp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Main_Map.Write_MinimapFile(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the minimap bitmap: " & Result.Problem)
        End If
    End Sub

    Sub Save_Heightmap_Prompt()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Bitmap Files (*.bmp)|*.bmp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Main_Map.Write_Heightmap(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the heightmap bitmap: " & Result.Problem)
        End If
    End Sub

    Sub PromptSave_TTP()
        Dim Dialog As New SaveFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "TTP Files (*.ttp)|*.ttp"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As sResult
        Result = Main_Map.Write_TTP(Dialog.FileName, True)
        If Not Result.Success Then
            MsgBox("There was a problem saving the tile types: " & Result.Problem)
        End If
    End Sub

    Sub New_Prompt()

        If LoseMapQuestion() Then
            NewMap()
        End If
    End Sub

    Sub NewMap()

        Main_Map.Deallocate()

        Main_Map = New clsMap(64, 64)

        Dim InterfaceOptions As New clsMap.clsInterfaceOptions
        Map_Changed(InterfaceOptions)
    End Sub

    Private Sub rdoAutoCliffRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoCliffRemove.Click

        If rdoAutoCliffRemove.Checked Then
            Tool = enumTool.AutoCliff
            lstAutoTexture.SelectedIndex = -1
            rdoAutoCliffBrush.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoTexturePlace.Checked = False
            rdoAutoRoadPlace.Checked = False
            rdoAutoRoadLine.Checked = False
            rdoRoadRemove.Checked = False
        End If
    End Sub

    Private Sub rdoAutoCliffBrush_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoCliffBrush.Click

        If rdoAutoCliffBrush.Checked Then
            Tool = enumTool.AutoCliff
            rdoAutoCliffRemove.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoTexturePlace.Checked = False
            rdoAutoRoadPlace.Checked = False
            rdoAutoRoadLine.Checked = False
            rdoRoadRemove.Checked = False
        End If
    End Sub

    Private Sub MinimapBMPToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MinimapBMPToolStripMenuItem.Click

        PromptSave_Minimap()
    End Sub

    Private Sub FMEToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuSaveFMap.Click

        Save_FMap_Prompt()
    End Sub

    Private Sub MapWZToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapWZToolStripMenuItem.Click

        Compile_Map_Prompt()
    End Sub

    Private Sub rdoAutoTextureFill_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoTextureFill.Click

        If rdoAutoTextureFill.Checked Then
            Tool = enumTool.AutoTexture_Fill
            rdoAutoCliffRemove.Checked = False
            rdoAutoCliffBrush.Checked = False
            rdoAutoTexturePlace.Checked = False
            rdoAutoRoadLine.Checked = False
            rdoRoadRemove.Checked = False
        End If
    End Sub

    Private Sub btnHeightOffsetSelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHeightOffsetSelection.Click
        If (Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing) Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim Offset As Double
        Dim SectorChange As clsMap.clsSectorGraphicsChange = Main_Map.SectorGraphicsChange
        Dim StartXY As sXY_int
        Dim FinishXY As sXY_int
        Dim Pos As sXY_int

        Offset = Clamp(Val(txtHeightOffset.Text), -255.0#, 255.0#)
        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
        For Y = StartXY.Y To FinishXY.Y
            For X = StartXY.X To FinishXY.X
                Main_Map.TerrainVertex(X, Y).Height = Math.Round(Clamp(Main_Map.TerrainVertex(X, Y).Height + Offset, 0.0#, 255.0#))
                Pos.X = X
                Pos.Y = Y
                SectorChange.Vertex_And_Normals_Changed(Pos)
            Next
        Next

        SectorChange.Update_Graphics_And_UnitHeights()
        Main_Map.UndoStepCreate("Selection Heights Offset")

        View_DrawViewLater()
    End Sub

    Private Sub rdoAutoTexturePlace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoTexturePlace.Click

        If rdoAutoTexturePlace.Checked Then
            Tool = enumTool.AutoTexture_Place
            rdoAutoCliffRemove.Checked = False
            rdoAutoCliffBrush.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoRoadPlace.Checked = False
            rdoAutoRoadLine.Checked = False
            rdoRoadRemove.Checked = False
        End If
    End Sub

    Private Sub rdoAutoRoadPlace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoRoadPlace.Click

        If rdoAutoRoadPlace.Checked Then
            Tool = enumTool.AutoRoad_Place
            rdoAutoCliffRemove.Checked = False
            rdoAutoCliffBrush.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoTexturePlace.Checked = False
        End If
    End Sub

    Private Sub rdoAutoRoadLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoAutoRoadLine.Click

        If rdoAutoRoadLine.Checked Then
            Tool = enumTool.AutoRoad_Line
            If Main_Map IsNot Nothing Then
                Main_Map.Selected_Tile_A = Nothing
                Main_Map.Selected_Tile_B = Nothing
            End If
            rdoAutoCliffRemove.Checked = False
            rdoAutoCliffBrush.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoTexturePlace.Checked = False
            rdoAutoRoadPlace.Checked = False
            rdoRoadRemove.Checked = False
        End If
    End Sub

    Private Sub rdoRoadRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoRoadRemove.Click

        If rdoRoadRemove.Checked Then
            Tool = enumTool.AutoRoad_Remove
            rdoAutoCliffRemove.Checked = False
            rdoAutoCliffBrush.Checked = False
            rdoAutoTextureFill.Checked = False
            rdoAutoTexturePlace.Checked = False
            rdoAutoRoadPlace.Checked = False
            rdoAutoRoadLine.Checked = False
        End If
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
        Main_Map.MinimapMakeLater() 'for unit highlight
        View_DrawViewLater()
    End Sub

    Private Sub lstStructures_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstStructures.SelectedIndexChanged

        If lstStructures.SelectedIndex >= 0 Then
            Tool = enumTool.ObjectPlace
            SelectedObjectType = lstStructures_Objects(lstStructures.SelectedIndex)
            lstFeatures.SelectedIndex = -1
            lstDroids.SelectedIndex = -1
        End If
        Main_Map.MinimapMakeLater() 'for unit highlight
        View_DrawViewLater()
    End Sub

    Private Sub lstDroids_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstDroids.SelectedIndexChanged

        If lstDroids.SelectedIndex >= 0 Then
            Tool = enumTool.ObjectPlace
            SelectedObjectType = lstDroids_Objects(lstDroids.SelectedIndex)
            lstFeatures.SelectedIndex = -1
            lstStructures.SelectedIndex = -1
        End If
        Main_Map.MinimapMakeLater() 'for unit highlight
        View_DrawViewLater()
    End Sub

    Sub Objects_Update()
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

    Private Sub txtObjectPlayer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If Not txtObjectPlayer.Enabled Then Exit Sub
        'If Map Is Nothing Then Exit Sub
        'If Map.Selected_Unit Is Nothing Then Exit Sub

        'Dim Unit_New As New clsMap.clsUnit(Map.Selected_Unit)
        'Dim ID As UInteger = Map.Selected_Unit.ID
        'Unit_New.Player_Num = Clamp(Val(txtObjectPlayer.Text), 0.0#, 7.0#)
        'Map.Unit_Remove_StoreChange(Map.Selected_Unit.Num)
        'Map.Unit_Add_StoreChange(Unit_New, ID)
        'Map.Selected_Unit = Unit_New
        'Object_Selected_Changed()
        'Map.Undo_Step_Make("Object Player Changed")
        'view_draw()
    End Sub

    Private Sub txtObjectName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If Not txtObjectName.Enabled Then Exit Sub
        'If Map Is Nothing Then Exit Sub
        'If Map.numSelected_Units Is Nothing Then Exit Sub

        'Dim Unit_New As New clsMap.clsUnit(Map.Selected_Unit)
        'Dim ID As UInteger = Map.Selected_Unit.ID
        'Unit_New.Name = txtObjectName.Text
        'Map.Unit_Remove_StoreChange(Map.Selected_Unit.Num)
        'Map.Unit_Add_StoreChange(Unit_New, ID)
        'Map.Selected_Unit = Unit_New
        'Object_Selected_Changed()
        'Map.Undo_Step_Make("Object Name Changed")
        'view_draw()
    End Sub

    Private Sub txtObjectRotation_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectRotation.LostFocus
        If Not txtObjectRotation.Enabled Then Exit Sub
        If txtObjectRotation.Text = "" Then Exit Sub
        If Main_Map Is Nothing Then Exit Sub
        If Main_Map.SelectedUnitCount <= 0 Then Exit Sub
        Dim Angle As Integer = Clamp(Val(txtObjectRotation.Text), 0.0#, 359.0#)
        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim ID As UInteger
        Dim A As Integer

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change rotation of multiple objects?", MsgBoxStyle.OkCancel, "") <> MsgBoxResult.Ok Then
                txtObjectRotation.Enabled = False
                txtObjectRotation.Text = ""
                txtObjectRotation.Enabled = True
                Exit Sub
            End If
        End If

        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone

        For A = 0 To OldUnits.GetUpperBound(0)
            NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            NewUnits(A).Rotation = Angle
            Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
            Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
            ErrorIDChange(ID, NewUnits(A), "txtObjectRotation_LostFocus")
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        Main_Map.SectorGraphicsChange.Update_Graphics()
        SelectedObject_Changed()
        Main_Map.UndoStepCreate("Object Rotated")
        View_DrawViewLater()
    End Sub

    Sub SelectedObject_Changed()

        lblObjectType.Enabled = False
        ObjectPlayerNum.Enabled = False
        txtObjectRotation.Enabled = False
        txtObjectID.Enabled = False
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
        If Main_Map.SelectedUnitCount > 1 Then
            lblObjectType.Text = "Multiple objects"
            Dim A As Integer
            Dim tmpUnitGroup As clsMap.clsUnitGroup = Main_Map.SelectedUnits(0).UnitGroup
            For A = 1 To Main_Map.SelectedUnitCount - 1
                If Main_Map.SelectedUnits(A).UnitGroup IsNot tmpUnitGroup Then
                    Exit For
                End If
            Next
            If A = Main_Map.SelectedUnitCount Then
                ObjectPlayerNum.SelectedUnitGroup = tmpUnitGroup
            Else
                ObjectPlayerNum.SelectedUnitGroup = Nothing
            End If
            txtObjectRotation.Text = ""
            txtObjectID.Text = ""
            lblObjectType.Enabled = True
            ObjectPlayerNum.Enabled = True
            txtObjectRotation.Enabled = True
            txtObjectPriority.Text = ""
            txtObjectPriority.Enabled = True
            txtObjectHealth.Text = ""
            txtObjectHealth.Enabled = True
            'design
            For A = 0 To Main_Map.SelectedUnitCount - 1
                If Main_Map.SelectedUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    If CType(Main_Map.SelectedUnits(A).Type, clsDroidDesign).IsTemplate Then
                        Exit For
                    End If
                End If
            Next
            If A < Main_Map.SelectedUnitCount Then
                btnDroidToDesign.Enabled = True
            End If

            For A = 0 To Main_Map.SelectedUnitCount - 1
                If Main_Map.SelectedUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    If Not CType(Main_Map.SelectedUnits(A).Type, clsDroidDesign).IsTemplate Then
                        Exit For
                    End If
                End If
            Next
            If A < Main_Map.SelectedUnitCount Then
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
        ElseIf Main_Map.SelectedUnitCount = 1 Then
            Dim A As Integer
            With Main_Map.SelectedUnits(0)
                lblObjectType.Text = .Type.GetDisplayText
                ObjectPlayerNum.SelectedUnitGroup = .UnitGroup
                txtObjectRotation.Text = CStr(.Rotation)
                txtObjectID.Text = CStr(.ID)
                txtObjectPriority.Text = CStr(.SavePriority)
                txtObjectHealth.Text = .Health * 100.0#
                lblObjectType.Enabled = True
                ObjectPlayerNum.Enabled = True
                txtObjectRotation.Enabled = True
                'txtObjectID.Enabled = True 'no known need to change IDs
                txtObjectPriority.Enabled = True
                txtObjectHealth.Enabled = True
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
        Else
            lblObjectType.Text = ""
            ObjectPlayerNum.SelectedUnitGroup = Nothing
            txtObjectRotation.Text = ""
            txtObjectID.Text = ""
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
        End If
        'this steals focus, so give it back
        View.OpenGLControl.Focus()
    End Sub

    Private Sub btnMapTexturer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMapTexturer.Click

        frmMapTexturerInstance.Show()
        frmMapTexturerInstance.Activate()
    End Sub

    Private Sub UndoLimitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UndoLimitToolStripMenuItem.Click

        Dim BoxResult As String = InputBox("Enter number of undo steps to store:", "", Undo_Limit)
        If BoxResult <> "" Then
            Undo_Limit = Clamp(Val(BoxResult), 0.0#, 8192.0#)
        End If
    End Sub

    Private Sub tsbSelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelection.Click

        Tool = enumTool.Terrain_Select
    End Sub

    Sub Terrain_Selection_Changed()
        Dim EnableCopy As Boolean = (Tool = enumTool.Terrain_Select)

        If Main_Map Is Nothing Then
            EnableCopy = False
        Else
            If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
                EnableCopy = False
            End If
        End If

        tsbSelectionCopy.Enabled = EnableCopy
    End Sub

    Private Sub tsbSelectionCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionCopy.Click

        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        If Copied_Map IsNot Nothing Then
            Copied_Map.Deallocate()
        End If
        Dim Area As sXY_int
        Dim Start As sXY_int
        Dim Finish As sXY_int
        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, Start, Finish)
        Area.X = Finish.X - Start.X
        Area.Y = Finish.Y - Start.Y
        Copied_Map = New clsMap(Main_Map, Start, Area)
    End Sub

    Private Sub tsbSelectionPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionPaste.Click

        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
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
        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, Start, Finish)
        Area.X = Finish.X - Start.X
        Area.Y = Finish.Y - Start.Y
        Main_Map.Map_Insert(Copied_Map, Start, Area, menuSelPasteHeights.Checked, menuSelPasteTextures.Checked, menuSelPasteUnits.Checked, menuSelPasteDeleteUnits.Checked, menuSelPasteGateways.Checked, menuSelPasteDeleteGateways.Checked)

        SelectedObject_Changed()
        Main_Map.UndoStepCreate("Paste")

        View_DrawViewLater()
    End Sub

    Private Sub btnSelResize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelResize.Click

        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
            MsgBox("You haven't selected anything.", vbOKOnly, "")
            Exit Sub
        End If

        Dim Start As sXY_int
        Dim Finish As sXY_int
        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, Start, Finish)
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

        Copied_Map.Rotate_Clockwise(frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub tsbSelectionRotateAnticlockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionRotateAnticlockwise.Click

        If Copied_Map Is Nothing Then
            MsgBox("Nothing to rotate.")
            Exit Sub
        End If

        Copied_Map.Rotate_CounterClockwise(frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub menuMiniShowTex_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowTex.Click

        Main_Map.MinimapMakeLater()
    End Sub

    Private Sub menuMiniShowHeight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowHeight.Click

        Main_Map.MinimapMakeLater()
    End Sub

    Private Sub menuMiniShowUnits_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowUnits.Click

        Main_Map.MinimapMakeLater()
    End Sub

    Private Function NoTile_Texture_Load() As clsResult
        Dim ReturnResult As New clsResult

        Dim tmpBitmap As Bitmap = Nothing

        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "notile.png", tmpBitmap).Success Then
            ReturnResult.AppendAsWarning(BitmapIsGLCompatible(tmpBitmap), "notile.png compatability: ")
            GLTexture_NoTile = BitmapGLTexture(tmpBitmap, View.OpenGLControl, False, False)
        End If
        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "overflow.png", tmpBitmap).Success Then
            ReturnResult.AppendAsWarning(BitmapIsGLCompatible(tmpBitmap), "overflow.png compatability: ")
            GLTexture_OverflowTile = BitmapGLTexture(tmpBitmap, View.OpenGLControl, False, False)
        End If

        Return ReturnResult
    End Function

    Private Sub tsbGateways_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbGateways.Click

        If Tool = enumTool.Gateways Then
            View.Draw_Gateways = False
            Tool = enumTool.None
            tsbGateways.Checked = False
        Else
            View.Draw_Gateways = True
            Tool = enumTool.Gateways
            tsbGateways.Checked = True
        End If
        If Main_Map IsNot Nothing Then
            Main_Map.Selected_Tile_A = Nothing
            Main_Map.Selected_Tile_B = Nothing
            View_DrawViewLater()
        End If
    End Sub

    Private Sub tsbDrawAutotexture_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDrawAutotexture.Click

        If View IsNot Nothing Then
            If View.Draw_VertexTerrain <> tsbDrawAutotexture.Checked Then
                View.Draw_VertexTerrain = tsbDrawAutotexture.Checked
                View_DrawViewLater()
            End If
        End If
    End Sub

    Private Sub tsbDrawTileOrientation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDrawTileOrientation.Click

        If View IsNot Nothing Then
            If DisplayTileOrientation <> tsbDrawTileOrientation.Checked Then
                DisplayTileOrientation = tsbDrawTileOrientation.Checked
                View_DrawViewLater()
                TextureView.DrawViewLater()
            End If
        End If
    End Sub

    Private Sub menuMiniShowGateways_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuMiniShowGateways.Click

        Main_Map.MinimapMakeLater()
    End Sub

    Private Sub menuMiniShowCliffs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuMiniShowCliffs.Click

        Main_Map.MinimapMakeLater()
    End Sub

    Private Sub cmbTileType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTileType.SelectedIndexChanged
        If Not cboTileType.Enabled Then Exit Sub
        If Main_Map Is Nothing Then Exit Sub
        If cboTileType.SelectedIndex < 0 Then Exit Sub
        If SelectedTextureNum < 0 Or SelectedTextureNum >= Main_Map.Tileset.TileCount Then Exit Sub

        Main_Map.Tile_TypeNum(SelectedTextureNum) = cboTileType.SelectedIndex

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

    Private Sub MinimumIntervalToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAutosaveInterval.Click

        Dim BoxResult As String = InputBox("Enter minimum interval between autosaves in seconds:", "", AutoSave_MinInterval_s)
        If BoxResult <> "" Then
            AutoSave_MinInterval_s = Clamp(Val(BoxResult), 1.0#, UInteger.MaxValue - 1.0#)
        End If
    End Sub

    Private Sub MinimumChangesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAutosaveChanges.Click

        Dim BoxResult As String = InputBox("Enter minimum number of changes before an autosave:", "", AutoSave_MinChanges)
        If BoxResult <> "" Then
            AutoSave_MinChanges = Clamp(Val(BoxResult), 1.0#, UInteger.MaxValue - 1.0#)
        End If
    End Sub

    Private Sub menuAutosaveOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuAutosaveOpen.Click

        Load_Autosave_Prompt()
    End Sub

    Private Sub Load_Autosave_Prompt()

        If Not IO.Directory.Exists(AutoSavePath) Then
            MsgBox("Autosave directory does not exist. There are no autosaves.", MsgBoxStyle.OkOnly, "")
            Exit Sub
        End If
        If LoseMapQuestion() Then
            Dim Dialog As New OpenFileDialog

            Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            Dialog.FileName = ""
            Dialog.Filter = ProgramName & " Files (*.fmap, *.fme)|*.fmap;*.fme|All Files (*.*)|*.*"
            Dialog.InitialDirectory = AutoSavePath
            If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
                Exit Sub
            End If
            Load_MainMap(Dialog.FileName)
        End If
    End Sub

    Private Sub tsbSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSave.Click

        Save_FMap_Quick()
    End Sub

    Private Sub btnGenerator_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerator.Click

        frmGeneratorInstance.Show()
        frmGeneratorInstance.Activate()
    End Sub

    Private Sub Title_Text_Update()
        Dim MapFileTitle As String

        If Main_Map.PathInfo Is Nothing Then
            MapFileTitle = "Unsaved map"
            tsbSave.ToolTipText = "Save FMap..."
        Else
            Dim SplitPath As New sSplitPath(Main_Map.PathInfo.Path)
            If Main_Map.PathInfo.IsFMap Then
                MapFileTitle = SplitPath.FileTitleWithoutExtension
                tsbSave.ToolTipText = "Quick save FMap to " & ControlChars.Quote & Main_Map.PathInfo.Path & ControlChars.Quote
            Else
                MapFileTitle = SplitPath.FileTitle
                tsbSave.ToolTipText = "Save FMap..."
            End If
        End If

        Text = MapFileTitle & " - " & ProgramName & " " & ProgramVersionNumber
    End Sub

    Private Function Settings_Load() As clsResult
        Dim ReturnResult As New clsResult

        UnitLabelFontSize = 20.0F
        SetFont(New Font("Verdana", 1.0F, FontStyle.Bold))

        Dim File_Settings As New clsReadFile
        If File_Settings.Begin(SettingsPath).Success Then
            ReturnResult.Append(Read_Settings(File_Settings), "Read settings.ini: ")
            File_Settings.Close()
        Else
#If Portable = 0.0# Then
            Dim File_OldSettings As New clsReadFile
            If File_OldSettings.Begin(OldSettingsPath).Success Then
                Read_OldSettings(File_OldSettings)
                File_OldSettings.Close()
            End If
#End If
        End If

        Return ReturnResult
    End Function

    Public Class clsSettings

        Public FontFamily As String = ""
        Public FontBold As Boolean = True
        Public FontItalic As Boolean = False
        Public AutoSaveEnabled As Boolean = True
        Public AutoSaveCompress As Boolean
        Public DirectoriesPrompt As Boolean = True

        Public TilesetsPaths(-1) As String
        Public TilesetsPathCount As Integer = 0
        Public ObjectDataPaths(-1) As String
        Public ObjectDataPathCount As Integer = 0

        Public DefaultTilesetPathNum As Integer = -1
        Public DefaultObjectDataPathNum As Integer = -1

        Public Function Translate_INI(ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "directpointer"
                    Try
                        DirectPointer = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontfamily"
                    FontFamily = INIProperty.Value
                Case "fontbold"
                    Try
                        FontBold = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontitalic"
                    Try
                        FontItalic = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontsize"
                    Dim sngTemp As Single
                    Try
                        sngTemp = CSng(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If sngTemp <= 0.0F Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    UnitLabelFontSize = sngTemp
                Case "minimapsize"
                    Dim Size As Integer
                    Try
                        Size = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If Size < 0 Or Size > MinimapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    MinimapSize = Size
                Case "undolimit"
                    Try
                        Undo_Limit = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosave"
                    Try
                        AutoSaveEnabled = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosavemininterval"
                    Try
                        AutoSave_MinInterval_s = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosaveminchanges"
                    Try
                        AutoSave_MinChanges = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosaveminchanges"
                    Try
                        AutoSave_MinChanges = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosavecompress"
                    Try
                        AutoSaveCompress = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "tilesetspath"
                    ReDim Preserve TilesetsPaths(TilesetsPathCount)
                    TilesetsPaths(TilesetsPathCount) = INIProperty.Value
                    TilesetsPathCount += 1
                Case "objectdatapath"
                    ReDim Preserve ObjectDataPaths(ObjectDataPathCount)
                    ObjectDataPaths(ObjectDataPathCount) = INIProperty.Value
                    ObjectDataPathCount += 1
                Case "defaulttilesetspathnum"
                    Try
                        DefaultTilesetPathNum = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "defaultobjectdatapathnum"
                    Try
                        DefaultObjectDataPathNum = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "directoriesprompt"
                    Try
                        DirectoriesPrompt = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_Settings(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim INISection As New clsINIRead.clsSection
        ReturnResult.Append(INISection.ReadFile(File), "")
        Dim NewSettings As New clsSettings
        ReturnResult.Append(INISection.Translate(AddressOf NewSettings.Translate_INI), "")

        menuAutosaveEnabled.Checked = NewSettings.AutoSaveEnabled
        menuAutosaveCompress.Checked = NewSettings.AutoSaveCompress
        menuDirectoriesPrompt.Checked = NewSettings.DirectoriesPrompt

        If NewSettings.FontFamily <> "" Then
            Dim tmpFontStyle As Integer = FontStyle.Regular
            If NewSettings.FontBold Then
                tmpFontStyle += FontStyle.Bold
            End If
            If NewSettings.FontItalic Then
                tmpFontStyle += FontStyle.Italic
            End If
            Dim tmpFont As New Font(NewSettings.FontFamily, 1.0F, CType(tmpFontStyle, FontStyle))
            SetFont(tmpFont)
        End If

        frmDataInstance.TilesetsPathSet.SetPaths(NewSettings.TilesetsPaths)
        If NewSettings.DefaultTilesetPathNum >= -1 And NewSettings.DefaultTilesetPathNum < NewSettings.TilesetsPathCount Then
            frmDataInstance.TilesetsPathSet.SelectedNum = NewSettings.DefaultTilesetPathNum
        End If
        frmDataInstance.ObjectDataPathSet.SetPaths(NewSettings.ObjectDataPaths)
        If NewSettings.DefaultObjectDataPathNum >= -1 And NewSettings.DefaultObjectDataPathNum < NewSettings.ObjectDataPathCount Then
            frmDataInstance.ObjectDataPathSet.SelectedNum = NewSettings.DefaultObjectDataPathNum
        End If

        Return ReturnResult
    End Function

    Private Function Read_OldSettings(ByVal File As clsReadFile) As Boolean
        Read_OldSettings = False

        Dim uintTemp As UInteger
        Dim byteTemp As Byte
        Dim strTemp As String = ""
        Dim Version As UInteger

        If Not File.Get_U32(Version) Then Exit Function
        If Version <> 4UI And Version <> 5UI Then Exit Function
        If Not File.Get_U32(uintTemp) Then Exit Function
        Undo_Limit = uintTemp
        If Not File.Get_U32(uintTemp) Then Exit Function
        AutoSave_MinInterval_s = uintTemp
        If Not File.Get_U32(uintTemp) Then Exit Function
        AutoSave_MinChanges = uintTemp
        If Not File.Get_U8(byteTemp) Then Exit Function
        menuAutosaveEnabled.Checked = (byteTemp > 0)
        If Not File.Get_U8(byteTemp) Then Exit Function
        DirectPointer = (byteTemp > 0)
        If Not File.Get_Text_VariableLength(strTemp) Then Exit Function
        Dim BoldByte As Byte
        Dim ItalicByte As Byte
        If Version = 5UI Then
            If Not File.Get_U8(BoldByte) Then Exit Function
            If Not File.Get_U8(ItalicByte) Then Exit Function
        End If
        Dim tmpFontStyle As Integer = FontStyle.Regular
        If BoldByte Then
            tmpFontStyle += FontStyle.Bold
        End If
        If ItalicByte Then
            tmpFontStyle += FontStyle.Italic
        End If
        Dim tmpFont As New Font(strTemp, 1.0F, CType(tmpFontStyle, FontStyle))
        SetFont(tmpFont)
        If Not File.Get_F32(UnitLabelFontSize) Then Exit Function

        Read_OldSettings = True
    End Function

    Private Sub SetFont(ByVal NewFont As Font)

        If UnitLabelFont IsNot Nothing Then
            UnitLabelFont.Deallocate()
        End If
        If TextureViewFont IsNot Nothing Then
            TextureViewFont.Deallocate()
        End If
        UnitLabelFont = View.CreateGLFont(NewFont)
        TextureViewFont = TextureView.CreateGLFont(NewFont)
    End Sub

    Private Function Settings_Write() As clsResult
        Dim ReturnResult As New clsResult

#If Portable = 0.0# Then
        If Not IO.Directory.Exists(MyDocumentsProgramPath) Then
            Try
                IO.Directory.CreateDirectory(MyDocumentsProgramPath)
            Catch ex As Exception
                ReturnResult.Problem_Add("Unable to create folder " & ControlChars.Quote & MyDocumentsProgramPath & ControlChars.Quote & ": " & ex.Message)
                Return ReturnResult
            End Try
        End If
#End If

        Dim INI_Settings As clsINIWrite = CreateINIWriteFile()

        ReturnResult.Append(Data_Settings(INI_Settings), "Compile settings.ini: ")

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        ReturnResult.Append(INI_Settings.File.WriteFile(SettingsPath, True), "Write settings.ini: ")

        Return ReturnResult
    End Function

    Private Function Data_Settings(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult

        File.Property_Append("DirectPointer", DirectPointer)
        If UnitLabelFont IsNot Nothing Then
            File.Property_Append("FontFamily", UnitLabelFont.BaseFont.FontFamily.Name)
            File.Property_Append("FontBold", UnitLabelFont.BaseFont.Bold)
            File.Property_Append("FontItalic", UnitLabelFont.BaseFont.Italic)
            File.Property_Append("FontSize", UnitLabelFontSize)
        End If
        File.Property_Append("MinimapSize", MinimapSize)
        File.Property_Append("UndoLimit", Undo_Limit)
        File.Property_Append("AutoSave", menuAutosaveEnabled.Checked)
        File.Property_Append("AutoSaveMinInterval", AutoSave_MinInterval_s)
        File.Property_Append("AutoSaveMinChanges", AutoSave_MinChanges)
        File.Property_Append("AutoSaveCompress", menuAutosaveCompress.Checked)
        File.Property_Append("DirectoriesPrompt", menuDirectoriesPrompt.Checked)
        Dim A As Integer
        Dim Paths() As String
        Paths = frmDataInstance.TilesetsPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("TilesetsPath", Paths(A))
        Next
        A = frmDataInstance.TilesetsPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultTilesetsPathNum", A)
        End If
        Paths = frmDataInstance.ObjectDataPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("ObjectDataPath", Paths(A))
        Next
        A = frmDataInstance.ObjectDataPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultObjectDataPathNum", A)
        End If

        Return ReturnResult
    End Function

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

        If MsgBox("Are you sure?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "Reinterpret Terrain") <> MsgBoxResult.Ok Then
            Exit Sub
        End If

        Main_Map.Terrain_Interpret(New sXY_int(0, 0), New sXY_int(Main_Map.TerrainSize.X - 1, Main_Map.TerrainSize.Y - 1))

        Main_Map.SectorAll_Set_Changed()
        Main_Map.SectorAll_GL_Update()

        Main_Map.UndoStepCreate("Interpret Terrain")
    End Sub

    Private Sub tabPlayerNum_SelectedIndexChanged() Handles ObjectPlayerNum.SelectedUnitGroupChanged
        ObjectPlayerNum.Focus() 'so that the rotation textbox and anything else loses focus, and performs its effects

        If Not ObjectPlayerNum.Enabled Then Exit Sub
        If Main_Map Is Nothing Then Exit Sub
        If Main_Map.SelectedUnitCount <= 0 Then Exit Sub
        If ObjectPlayerNum.SelectedUnitGroup Is Nothing Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change player of multiple objects?", MsgBoxStyle.OkCancel, "") <> MsgBoxResult.Ok Then
                ObjectPlayerNum.Enabled = False
                ObjectPlayerNum.SelectedUnitGroup = Nothing
                ObjectPlayerNum.Enabled = True
                Exit Sub
            End If
        End If

        Dim A As Integer
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone 'copy the array because it will change as units are removed
        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim ID As UInteger

        For A = 0 To OldUnits.GetUpperBound(0)
            NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            NewUnits(A).UnitGroup = ObjectPlayerNum.SelectedUnitGroup
            Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
            Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
            ErrorIDChange(ID, NewUnits(A), "ObjectPlayerNum_SelectedPlayerNumChanged")
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        Main_Map.UndoStepCreate("Object Player Changed")
        View_DrawViewLater()
    End Sub

    Private Sub txtHeightSetL_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHeightSetL.LostFocus
        Dim tmpHeight As Byte

        tmpHeight = Clamp(Val(txtHeightSetL.Text), 0.0#, 255.0#)
        HeightSetPalette(tabHeightSetL.SelectedIndex) = tmpHeight
        If tabHeightSetR.SelectedIndex = tabHeightSetL.SelectedIndex Then
            tabHeightSetR_SelectedIndexChanged(Nothing, Nothing)
        End If
        tabHeightSetL.TabPages(tabHeightSetL.SelectedIndex).Text = tmpHeight
        tabHeightSetR.TabPages(tabHeightSetL.SelectedIndex).Text = tmpHeight
    End Sub

    Private Sub txtHeightSetR_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtHeightSetR.LostFocus
        Dim tmpHeight As Byte

        tmpHeight = Clamp(Val(txtHeightSetR.Text), 0.0#, 255.0#)
        HeightSetPalette(tabHeightSetR.SelectedIndex) = Clamp(Val(txtHeightSetR.Text), 0.0#, 255.0#)
        If tabHeightSetL.SelectedIndex = tabHeightSetR.SelectedIndex Then
            tabHeightSetL_SelectedIndexChanged(Nothing, Nothing)
        End If
        tabHeightSetL.TabPages(tabHeightSetR.SelectedIndex).Text = tmpHeight
        tabHeightSetR.TabPages(tabHeightSetR.SelectedIndex).Text = tmpHeight
    End Sub

    Private Sub tabHeightSetL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabHeightSetL.SelectedIndexChanged

        txtHeightSetL.Text = HeightSetPalette(tabHeightSetL.SelectedIndex)
    End Sub

    Private Sub tabHeightSetR_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabHeightSetR.SelectedIndexChanged

        txtHeightSetR.Text = HeightSetPalette(tabHeightSetR.SelectedIndex)
    End Sub

    Private Sub tsbSelectionObjects_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionObjects.Click
        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim A As Integer

        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, Start, Finish)
        For A = 0 To Main_Map.UnitCount - 1
            If PosIsWithinTileArea(Main_Map.Units(A).Pos.Horizontal, Start, Finish) Then
                Main_Map.SelectedUnit_Add(Main_Map.Units(A))
            End If
        Next

        SelectedObject_Changed()
        Tool = enumTool.None
        View_DrawViewLater()
    End Sub

    Private Sub menuImportMapCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuImportMapCopy.Click
        Dim Dialog As New OpenFileDialog

        Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Dialog.FileName = ""
        Dialog.Filter = "Warzone Map Files (*.fmap, *.fme, *.wz, *.lnd)|*.fmap;*.fme;*.wz;*.lnd|All Files (*.*)|*.*"
        If Dialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim Result As clsResult
        Result = Load_Map_As_Copy(Dialog.FileName)
        ShowWarnings(Result, "Load Map As Copy")
    End Sub

    Public Sub View_DrawViewLater()

        If View IsNot Nothing Then
            View.DrawViewLater()
        End If
    End Sub

    Private Sub btnWaterTri_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWaterTri.Click

        Main_Map.WaterTriCorrection()
        Main_Map.SectorGraphicsChange.Update_Graphics()
        Main_Map.UndoStepCreate("Water Triangle Correction")
        View_DrawViewLater()
    End Sub

    Private Sub tsbSelectionFlipX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionFlipX.Click

        If Copied_Map Is Nothing Then
            MsgBox("Nothing to flip.")
            Exit Sub
        End If

        Copied_Map.Rotate_FlipX(frmMainInstance.PasteRotateObjects)
    End Sub

    Public Sub SetMenuPointerModeChecked()

        If DirectPointer Then
            menuPointerPlane.Checked = False
            menuPointerDirect.Checked = True
        Else
            menuPointerPlane.Checked = True
            menuPointerDirect.Checked = False
        End If
    End Sub

    Private Sub menuPointerPlane_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPointerPlane.Click

        DirectPointer = False
        SetMenuPointerModeChecked()
    End Sub

    Private Sub menuPointerDirect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuPointerDirect.Click

        DirectPointer = True
        SetMenuPointerModeChecked()
    End Sub

    Private Sub btnHeightsMultiplySelection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHeightsMultiplySelection.Click
        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim Multiplier As Double
        Dim SectorChange As clsMap.clsSectorGraphicsChange = Main_Map.SectorGraphicsChange
        Dim StartXY As sXY_int
        Dim FinishXY As sXY_int
        Dim Pos As sXY_int

        Multiplier = Clamp(Val(txtHeightMultiply.Text), 0.0#, 255.0#)
        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
        For Y = StartXY.Y To FinishXY.Y
            For X = StartXY.X To FinishXY.X
                Main_Map.TerrainVertex(X, Y).Height = Math.Round(Clamp(Main_Map.TerrainVertex(X, Y).Height * Multiplier, 0.0#, 255.0#))
                Pos.X = X
                Pos.Y = Y
                SectorChange.Vertex_And_Normals_Changed(Pos)
            Next
        Next

        SectorChange.Update_Graphics_And_UnitHeights()
        Main_Map.UndoStepCreate("Selection Heights Multiply")

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
            TextureOrientation.ResultZFlip = Not TextureOrientation.ResultZFlip
        Else
            TextureOrientation.ResultXFlip = Not TextureOrientation.ResultXFlip
        End If

        TextureView.DrawViewLater()
    End Sub

    Private Sub menuUnitLabelFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuFont.Click

        If UnitLabelFont IsNot Nothing Then
            FontDialog.Font = New Font(UnitLabelFont.BaseFont.FontFamily, UnitLabelFontSize, UnitLabelFont.BaseFont.Style)
        End If
        If FontDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        If FontDialog.Font Is Nothing Then
            Exit Sub
        End If
        SetFont(FontDialog.Font)
        UnitLabelFontSize = FontDialog.Font.SizeInPoints
    End Sub

    Private Function LoadTilesets(ByVal TilesetsPath As String) As clsResult
        LoadTilesets = New clsResult

        Dim TilesetDirs() As String
        Try
            TilesetDirs = IO.Directory.GetDirectories(TilesetsPath)
        Catch ex As Exception
            LoadTilesets.Problem_Add("Error reading tilesets directory: " & ex.Message)
            Exit Function
        End Try
        Dim A As Integer

        If TilesetDirs Is Nothing Then
            TilesetCount = 0
            ReDim Tilesets(-1)
            Exit Function
        End If

        TilesetCount = 0
        ReDim Tilesets(TilesetDirs.GetUpperBound(0))

        Dim Result As clsResult
        Dim Path As String

        For A = 0 To TilesetDirs.GetUpperBound(0)
            Path = TilesetDirs(A)
            Tilesets(TilesetCount) = New clsTileset
            Result = Tilesets(TilesetCount).LoadDirectory(Path)
            LoadTilesets.AppendAsWarning(Result, "Loading tileset directory " & ControlChars.Quote & Path & ControlChars.Quote & ": ")
            If Not Result.HasProblems Then
                Tilesets(TilesetCount).Num = TilesetCount
                TilesetCount += 1
            End If
        Next

        ReDim Preserve Tilesets(TilesetCount - 1)

        Dim tmpTileset As clsTileset

        For A = 0 To TilesetCount - 1
            tmpTileset = Tilesets(A)
            If tmpTileset.Name = "tertilesc1hw" Then
                tmpTileset.Name = "Arizona"
                Tileset_Arizona = tmpTileset
                tmpTileset.IsOriginal = True
            ElseIf tmpTileset.Name = "tertilesc2hw" Then
                tmpTileset.Name = "Urban"
                Tileset_Urban = tmpTileset
                tmpTileset.IsOriginal = True
            ElseIf tmpTileset.Name = "tertilesc3hw" Then
                tmpTileset.Name = "Rocky Mountains"
                Tileset_Rockies = tmpTileset
                tmpTileset.IsOriginal = True
            End If
        Next

        If Tileset_Arizona Is Nothing Then
            LoadTilesets.Warning_Add("Arizona tileset is missing.")
        End If
        If Tileset_Urban Is Nothing Then
            LoadTilesets.Warning_Add("Urban tileset is missing.")
        End If
        If Tileset_Rockies Is Nothing Then
            LoadTilesets.Warning_Add("Rocky Mountains tileset is missing.")
        End If
    End Function

    Private Sub lstAutoTexture_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoTexture.SelectedIndexChanged

        If lstAutoTexture.SelectedIndex < 0 Then
            SelectedTerrain = Nothing
        ElseIf lstAutoTexture.SelectedIndex < Main_Map.Painter.TerrainCount Then
            SelectedTerrain = Main_Map.Painter.Terrains(lstAutoTexture.SelectedIndex)
        Else
            Stop
            SelectedTerrain = Nothing
        End If
    End Sub

    Private Sub lstAutoRoad_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstAutoRoad.SelectedIndexChanged

        If lstAutoRoad.SelectedIndex < 0 Then
            SelectedRoad = Nothing
        ElseIf lstAutoRoad.SelectedIndex < Main_Map.Painter.RoadCount Then
            SelectedRoad = Main_Map.Painter.Roads(lstAutoRoad.SelectedIndex)
        Else
            Stop
            SelectedRoad = Nothing
        End If
    End Sub

    Private Sub txtObjectID_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectID.LostFocus
        If Not txtObjectID.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <> 1 Then
            Exit Sub
        End If

        '    Dim NewID As UInteger = Clamp(Val(txtObjectRot.Text), 0.0#, CDbl(UInteger.MaxValue))

        '    Dim A As Integer

        '    For A = 0 To Map.UnitCount - 1
        '        If Map.Units(A).ID = NewID Then
        '            Exit For
        '        End If
        '    Next
        '    If A < Map.UnitCount Then
        '        MsgBox("ID is already in use by " & Map.Units(A).Type.Code & " at " & Map.Units(A).Pos.X & ", " & Map.Units(A).Pos.Z & ".", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        '        txtObjectID.Text = Map.SelectedUnits(0).ID
        '        Exit Sub
        '    End If

        '    If txtObjectID.Text <> CStr(NewID) Then
        '        MsgBox("Entered text was not a valid ID number.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        '        txtObjectID.Text = Map.SelectedUnits(0).ID
        '        Exit Sub
        '    End If

        '    Dim NewUnit As clsMap.clsUnit
        '    Dim OldUnit As clsMap.clsUnit = Map.SelectedUnits(0)

        '    NewUnit = New clsMap.clsUnit(OldUnit)
        '    Map.Unit_Remove_StoreChange(OldUnit.Num)
        '    Map.Unit_Add_StoreChange(NewUnit, NewID)

        '    Map.Selected_Units_Clear()
        '    Map.SelectedUnit_Add(NewUnit)
        '    Selected_Object_Changed()
        '    Map.Undo_Step_Make("Object ID Changed")
        '    DrawView()
    End Sub

    Private Sub txtObjectpriority_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectPriority.LostFocus
        If Not txtObjectPriority.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If
        Dim Priority As Integer = CInt(Clamp(Val(txtObjectPriority.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue)))
        If txtObjectPriority.Text <> CStr(Priority) Then
            If txtObjectPriority.Text <> "" Then
                MsgBox("Entered text is not a valid number.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
                txtObjectPriority.Text = ""
            End If
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change priority of multiple objects?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                txtObjectPriority.Enabled = False
                txtObjectPriority.Text = ""
                txtObjectPriority.Enabled = True
                Exit Sub
            End If
        ElseIf Main_Map.SelectedUnitCount = 1 Then
            If Priority = Main_Map.SelectedUnits(0).SavePriority Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim A As Integer

        For A = 0 To OldUnits.GetUpperBound(0)
            NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            NewUnits(A).SavePriority = Priority
            Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
            Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
            ErrorIDChange(ID, NewUnits(A), "ObjectPriority_LostFocus")
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        Main_Map.UndoStepCreate("Object Priority Changed")
        View_DrawViewLater()
    End Sub

    Private Sub txtObjectHealth_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObjectHealth.LostFocus
        If Not txtObjectHealth.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If
        Dim Health As Double = CInt(Clamp(Val(txtObjectHealth.Text), 1.0#, 100.0#)) / 100.0#

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change health of multiple objects?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                txtObjectHealth.Enabled = False
                txtObjectHealth.Text = ""
                txtObjectHealth.Enabled = True
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim A As Integer

        For A = 0 To OldUnits.GetUpperBound(0)
            NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            NewUnits(A).Health = Health
            Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
            Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
            ErrorIDChange(ID, NewUnits(A), "ObjectHealth_LostFocus")
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        Main_Map.UndoStepCreate("Object Health Changed")
        View_DrawViewLater()
    End Sub

    Private Sub btnDroidToDesign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDroidToDesign.Click
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change design of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        Else
            If MsgBox("Change design of a droid?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "btnDroidToDesign_Click")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Template Removed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change body of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.Body = cboBody_Objects(cboDroidBody.SelectedIndex)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "cboDroidBody_SelectedIndexChanged")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Body Changed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change propulsion of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.Propulsion = cboPropulsion_Objects(cboDroidPropulsion.SelectedIndex)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "cboDroidPropulsion_SelectedIndexChanged")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Propulsion Changed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change turret of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.Turret1 = cboTurret_Objects(cboDroidTurret1.SelectedIndex)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "cboDroidTurret1_SelectedIndexChanged")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Turret Changed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change turret of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.Turret2 = cboTurret_Objects(cboDroidTurret2.SelectedIndex)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "cboDroidTurret2_SelectedIndexChanged")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Turret Changed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change turret of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.Turret3 = cboTurret_Objects(cboDroidTurret3.SelectedIndex)
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "cboDroidTurret3_SelectedIndexChanged")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Turret Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub rdoDroidTurret0_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret0.CheckedChanged
        If Not rdoDroidTurret0.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret0.Checked Then
            Exit Sub
        End If

        rdoDroidTurret1.Checked = False
        rdoDroidTurret2.Checked = False
        rdoDroidTurret3.Checked = False

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(0)
    End Sub

    Private Sub rdoDroidTurret1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret1.CheckedChanged
        If Not rdoDroidTurret1.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret1.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret2.Checked = False
        rdoDroidTurret3.Checked = False

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(1)
    End Sub

    Private Sub rdoDroidTurret2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret2.CheckedChanged
        If Not rdoDroidTurret2.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret2.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret1.Checked = False
        rdoDroidTurret3.Checked = False

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(2)
    End Sub

    Private Sub rdoDroidTurret3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoDroidTurret3.CheckedChanged
        If Not rdoDroidTurret2.Enabled Then
            Exit Sub
        End If
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Not rdoDroidTurret3.Checked Then
            Exit Sub
        End If

        rdoDroidTurret0.Checked = False
        rdoDroidTurret1.Checked = False
        rdoDroidTurret2.Checked = False

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change number of turrets of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetTurretCount(3)
    End Sub

    Private Sub SelectedObjects_SetTurretCount(ByVal Count As Integer)

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.TurretCount = Count
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "SelectedObjects_SetTurretCount")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Number Of Turrets Changed")
            View_DrawViewLater()
        End If
    End Sub

    Private Sub SelectedObjects_SetDroidType(ByVal NewType As clsDroidDesign.clsTemplateDroidType)

        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
        Dim OldUnits() As clsMap.clsUnit = Main_Map.SelectedUnits.Clone
        Dim ID As UInteger
        Dim OldDroidType As clsDroidDesign
        Dim NewDroidType As clsDroidDesign
        Dim A As Integer
        Dim Changed As Boolean = False
        Dim DoUnit As Boolean

        For A = 0 To OldUnits.GetUpperBound(0)
            If OldUnits(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                DoUnit = Not OldDroidType.IsTemplate
            Else
                DoUnit = False
            End If
            If DoUnit Then
                Changed = True
                NewUnits(A) = New clsMap.clsUnit(OldUnits(A))
                ID = OldUnits(A).ID
                NewDroidType = New clsDroidDesign
                NewUnits(A).Type = NewDroidType
                OldDroidType = CType(OldUnits(A).Type, clsDroidDesign)
                NewDroidType.CopyDesign(OldDroidType)
                NewDroidType.TemplateDroidType = NewType
                NewDroidType.UpdateAttachments()
                Main_Map.Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
                Main_Map.Unit_Add_StoreChange(NewUnits(A), ID)
                ErrorIDChange(ID, NewUnits(A), "SelectedObjects_SetDroidType")
            Else
                NewUnits(A) = OldUnits(A)
            End If
        Next
        Main_Map.SelectedUnits_Clear()
        Main_Map.SelectedUnit_Add(NewUnits)
        SelectedObject_Changed()
        If Changed Then
            Main_Map.UndoStepCreate("Object Body Changed")
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
        If Main_Map Is Nothing Then
            Exit Sub
        End If
        If Main_Map.SelectedUnitCount <= 0 Then
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change type of multiple droids?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question, "") <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        SelectedObjects_SetDroidType(TemplateDroidTypes(cboDroidType.SelectedIndex))
    End Sub

    Public Sub SetInterface(ByVal InterfaceOptions As clsMap.clsInterfaceOptions)

        frmCompileInstance.cbxAutoScrollLimits.Checked = InterfaceOptions.AutoScrollLimits
        frmCompileInstance.txtCampTime.Text = InterfaceOptions.CampaignGameTime
        frmCompileInstance.cboCampType.SelectedIndex = InterfaceOptions.CampaignGameType
        frmCompileInstance.txtAuthor.Text = InterfaceOptions.CompileMultiAuthor
        frmCompileInstance.cboLicense.Text = InterfaceOptions.CompileMultiLicense
        frmCompileInstance.txtName.Text = InterfaceOptions.CompileName
        frmCompileInstance.txtMultiPlayers.Text = InterfaceOptions.CompileMultiPlayers
        frmCompileInstance.cbxNewPlayerFormat.Checked = InterfaceOptions.CompileMultiXPlayers
        frmCompileInstance.txtScrollMinX.Text = InterfaceOptions.ScrollMin.X
        frmCompileInstance.txtScrollMinY.Text = InterfaceOptions.ScrollMin.Y
        frmCompileInstance.txtScrollMaxX.Text = InterfaceOptions.ScrollMax.X
        frmCompileInstance.txtScrollMaxY.Text = InterfaceOptions.ScrollMax.Y

        frmCompileInstance.AutoScrollLimits_Update()
    End Sub

    Public Enum enumTextureTerrainAction As Byte
        Ignore
        Reinterpret
        Remove
    End Enum

    Public TextureTerrainAction As enumTextureTerrainAction = enumTextureTerrainAction.Reinterpret

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

    Private Sub MinimapSizeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MinimapSizeToolStripMenuItem.Click

        Dim Result As String = InputBox("Enter minimap size.", "", MinimapSize)
        If Result Is Nothing Or Result = "" Then
            Exit Sub
        End If
        MinimapSize = Clamp(Val(Result), 0.0#, MinimapMaxSize)
        View_DrawViewLater()
    End Sub

    Private Sub btnPlayerSelectObjects_Click(sender As System.Object, e As System.EventArgs) Handles btnPlayerSelectObjects.Click
        Dim A As Integer

        If Not Control_Unit_Multiselect.Active Then
            Main_Map.SelectedUnits_Clear()
        End If

        Dim tmpUnitGroup As clsMap.clsUnitGroup = NewPlayerNum.SelectedUnitGroup

        For A = 0 To Main_Map.UnitCount - 1
            If Main_Map.Units(A).UnitGroup Is tmpUnitGroup Then
                If Main_Map.Units(A).Map_SelectedUnitNum < 0 Then
                    Main_Map.SelectedUnit_Add(Main_Map.Units(A))
                End If
            End If
        Next

        View_DrawViewLater()
    End Sub

    Private Sub menuSaveFME_Click(sender As System.Object, e As System.EventArgs) Handles menuSaveFME.Click

        Save_FME_Prompt()
    End Sub
End Class