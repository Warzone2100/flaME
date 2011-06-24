Imports ICSharpCode.SharpZipLib

Public Class frmMain
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

        InitializeComponent() 'required for monodevelop too, depends on subdirs being set

        NewPlayerNum = New ctrlPlayerNum
        ObjectPlayerNum = New ctrlPlayerNum

        View = New ctrlMapView
        TextureView = New ctrlTextureView
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If LoseMapQuestion() Then
            Settings_Write()
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
        NewPlayerNum.SelectedPlayerNum = 0
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

        InitializeResult.AppendAsWarning(LoadTilesets(), "Load tilesets: ")

        cmbTileset_CreateItems(-1)

        NoTile_Texture_Load()
        cmbTileType_Refresh()

        CreateTemplateDroidTypes() 'do before loading data

        InitializeResult.AppendAsWarning(DataLoad(ObjectDataPath), "Load object data: ")

        CreateGeneratorTilesets()
        CreatePainterArizona()
        CreatePainterUrban()
        CreatePainterRockies()

        Main_Map = New clsMap(1, 1)

        InitializeResult.AppendAsWarning(Settings_Load(), "Load settings: ")

        SetMenuPointerModeChecked()

        Objects_Update()

        SelectedObject_Changed()
        Copied_Map_Changed()
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
        Main_Map.SectorGraphicsChange.Update_Graphics()

        View.DrawView_SetEnabled(True)

        TextureView.Dock = DockStyle.Fill
        TableLayoutPanel6.Controls.Add(TextureView, 0, 1)

        Tool = enumTool.Texture_Brush

        If My.Application.CommandLineArgs.Count >= 1 Then
            Dim Path As String = My.Application.CommandLineArgs(0)

            Load_MainMap(Path)
        End If

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
        Move = tmrKey.Interval * Rate * Math.Max(Math.Abs(View.ViewPos.Y), 512.0#) / 1536.0#

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

            PanRate = View.FieldOfViewY / 16.0# * Rate
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

    Private Sub Load_MainMap(ByVal Path As String)
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
    End Sub

    Public Sub Map_Changed(ByVal InterfaceOptions As clsMap.clsInterfaceOptions)

        Resize_Update()
        cmbTileset_Refresh()
        PainterTerrains_Refresh(-1, -1)

        Main_Map.SectorAll_GL_Update()
        View.ViewAngleSetToDefault()
        View.LookAtTile(New sXY_int(Math.Floor(Main_Map.TerrainSize.X / 2.0#), Math.Floor(Main_Map.TerrainSize.Y / 2.0#)))

        tsbSave.Enabled = False
        SetInterface(InterfaceOptions)
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
            ReturnResult.Append(ResultMap.Load_FMap(Path, InterfaceOptions), "Load Fmap: ")
            ResultMap.LastFMapSaveInfo = New clsMap.clsFMapSaveInfo(Path)
        ElseIf SplitPath.FileExtension = "fme" Or SplitPath.FileExtension = "wzme" Then
            ReturnResult.Append(ResultMap.Load_FME(Path, InterfaceOptions), "")
        ElseIf SplitPath.FileExtension = "wz" Then
            ReturnResult.Append(ResultMap.Load_WZ(Path), "Load WZ: ")
        ElseIf SplitPath.FileExtension = "lnd" Then
            Result = ResultMap.Load_LND(Path)
            If Not Result.Success Then
                ReturnResult.Problem_Add("Load LND: " & Result.Problem)
            End If
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

            Copied_Map_Changed()
        End If

        Return Result
    End Function

    Sub Load_Map_Prompt()

        If LoseMapQuestion() Then
            Dim Dialog As New OpenFileDialog
            If Main_Map.LastFMapSaveInfo Is Nothing Then
                Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            Else
                Dialog.InitialDirectory = Main_Map.LastFMapSaveInfo.Path
            End If
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

    Public Sub cmbTileset_CreateItems(ByVal NewSelectedIndex As Integer)
        Dim A As Integer

        cboTileset.Items.Clear()
        For A = 0 To TilesetCount - 1
            cboTileset.Items.Add(Tilesets(A).Name)
        Next
        cboTileset.SelectedIndex = NewSelectedIndex
    End Sub

    Sub cmbTileset_Refresh()
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

    Private Sub cmbTileset_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTileset.SelectedIndexChanged
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
        Dim Z As Integer

        Dim difA As Double
        Dim difB As Double

        Dim NewTri As Boolean

        'tri set to the direction where the diagonal edge will be the flattest, so that cliff edges are level
        For Z = 0 To Main_Map.TerrainSize.Y - 1
            For X = 0 To Main_Map.TerrainSize.X - 1
                difA = Math.Abs(CDbl(Main_Map.TerrainVertex(X + 1, Z + 1).Height) - Main_Map.TerrainVertex(X, Z).Height)
                difB = Math.Abs(CDbl(Main_Map.TerrainVertex(X, Z + 1).Height) - Main_Map.TerrainVertex(X + 1, Z).Height)
                If difA = difB Then
                    If CInt(Int(Rnd() * 2.0F)) = 0 Then
                        NewTri = False
                    Else
                        NewTri = True
                    End If
                ElseIf difA < difB Then
                    NewTri = False
                Else
                    NewTri = True
                End If
                If Not Main_Map.TerrainTiles(X, Z).Tri = NewTri Then
                    Main_Map.TerrainTiles(X, Z).Tri = NewTri
                End If
            Next X
        Next Z

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
        Dim Max As Double = MaxMapTileSize

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
        If NewSize.X > 250 Or NewSize.Y > 250 Then
            If MsgBox("Warzone doesn't support map sizes above 250. Continue anyway?", MsgBoxStyle.YesNo, "") <> MsgBoxResult.Yes Then
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

        If Main_Map.LastFMapSaveInfo Is Nothing Then
            Dialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            Dialog.InitialDirectory = Main_Map.LastFMapSaveInfo.Path
        End If
        Dialog.FileName = ""
        Dialog.Filter = ProgramName & " Map Files (*.fmap)|*.fmap"
        If Dialog.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            'If Not PromptFileFMEVersion(Dialog.FileName) Then
            '    Save_FME_Prompt()
            '    Exit Sub
            'End If
            Dim Result As clsResult
            Result = Main_Map.Write_FMap(Dialog.FileName, True)
            If Not Result.HasProblems Then
                Main_Map.LastFMapSaveInfo = New clsMap.clsFMapSaveInfo(Dialog.FileName)
                tsbSave.Enabled = False
                Title_Text_Update()
            End If
            ShowWarnings(Result, "Save FMap")
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

        If Main_Map.LastFMapSaveInfo Is Nothing Then
            Save_FMap_Prompt()
        Else
            'If Not PromptFileFMEVersion(Map.LastFMESaveInfo.Path) Then
            '    Save_FME_Prompt()
            '    Exit Sub
            'End If

            Dim Result As clsResult = Main_Map.Write_FMap(Main_Map.LastFMapSaveInfo.Path, True)
            If Not Result.HasProblems Then
                tsbSave.Enabled = False
            End If
            ShowWarnings(Result, "Quick Save FMap")
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
        Result = Main_Map.Write_HeightmapBMP(Dialog.FileName, True)
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

    Private Sub FMEToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MapFMEToolStripMenuItem.Click

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
            Dim PlayerNum As Integer
            PlayerNum = Main_Map.SelectedUnits(0).PlayerNum
            For A = 1 To Main_Map.SelectedUnitCount - 1
                If Main_Map.SelectedUnits(A).PlayerNum <> PlayerNum Then
                    Exit For
                End If
            Next
            If A = Main_Map.SelectedUnitCount Then
                ObjectPlayerNum.SelectedPlayerNum = PlayerNum
            Else
                ObjectPlayerNum.SelectedPlayerNum = -1
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
                ObjectPlayerNum.SelectedPlayerNum = .PlayerNum
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
            ObjectPlayerNum.SelectedPlayerNum = -1
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

    Sub Copied_Map_Changed()

        If Copied_Map Is Nothing Then
            tsbSelectionPaste.Enabled = False
            tsbSelectionRotateClockwise.Enabled = False
            tsbSelectionRotateAnticlockwise.Enabled = False
        Else
            tsbSelectionPaste.Enabled = True
            tsbSelectionRotateClockwise.Enabled = True
            tsbSelectionRotateAnticlockwise.Enabled = True
        End If
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

        Copied_Map_Changed()
    End Sub

    Private Sub tsbSelectionPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionPaste.Click

        If Main_Map.Selected_Area_VertexA Is Nothing Or Main_Map.Selected_Area_VertexB Is Nothing Then
            Exit Sub
        End If
        If Copied_Map Is Nothing Then
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
            Exit Sub
        End If

        Copied_Map.Rotate_Clockwise(frmMainInstance.PasteRotateObjects)
    End Sub

    Private Sub tsbSelectionRotateAnticlockwise_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbSelectionRotateAnticlockwise.Click

        If Copied_Map Is Nothing Then
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

    Sub NoTile_Texture_Load()
        Dim tmpBitmap As Bitmap = Nothing

        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "notile.png", tmpBitmap).Success Then
            GLTexture_NoTile = BitmapGLTexture(tmpBitmap, View.OpenGLControl, False, False)
        End If
        If LoadBitmap(EndWithPathSeperator(My.Application.Info.DirectoryPath) & "overflow.png", tmpBitmap).Success Then
            GLTexture_OverflowTile = BitmapGLTexture(tmpBitmap, View.OpenGLControl, False, False)
        End If
    End Sub

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

    Private Sub cmbTileType_Refresh()
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

    Sub Title_Text_Update()
        Dim MapFileTitle As String

        If Main_Map.LastFMapSaveInfo Is Nothing Then
            MapFileTitle = "Unsaved map"
            tsbSave.ToolTipText = "Save FMap"
        Else
            Dim SplitPath As New sSplitPath(Main_Map.LastFMapSaveInfo.Path)
            MapFileTitle = SplitPath.FileTitleWithoutExtension
            tsbSave.ToolTipText = "Quick save FMap to " & ControlChars.Quote & Main_Map.LastFMapSaveInfo.Path & ControlChars.Quote
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
            Dim File_OldSettings As New clsReadFile
            If File_OldSettings.Begin(OldSettingsPath).Success Then
                Read_OldSettings(File_OldSettings)
                File_OldSettings.Close()
            End If
        End If

        Return ReturnResult
    End Function

    Public Class clsSettings

        Public FontFamily As String = ""
        Public FontBold As Boolean = True
        Public FontItalic As Boolean = False
        Public AutoSaveEnabled As Boolean

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
                    If Size < 0 Or Size > MaxMinimapSize Then
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

    Private Sub tabPlayerNum_SelectedIndexChanged() Handles ObjectPlayerNum.SelectedPlayerNumChanged
        ObjectPlayerNum.Focus() 'so that the rotation textbox and anything else loses focus, and performs its effects

        If Not ObjectPlayerNum.Enabled Then Exit Sub
        If Main_Map Is Nothing Then Exit Sub
        If Main_Map.SelectedUnitCount <= 0 Then Exit Sub
        If ObjectPlayerNum.SelectedPlayerNum < 0 Or ObjectPlayerNum.SelectedPlayerNum > 10 Then
            Stop
            Exit Sub
        End If

        If Main_Map.SelectedUnitCount > 1 Then
            If MsgBox("Change player of multiple objects?", MsgBoxStyle.OkCancel, "") <> MsgBoxResult.Ok Then
                ObjectPlayerNum.Enabled = False
                ObjectPlayerNum.SelectedPlayerNum = -1
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
            NewUnits(A).PlayerNum = ObjectPlayerNum.SelectedPlayerNum
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

    Private Function LoadTilesets() As clsResult
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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrKey = New System.Windows.Forms.Timer()
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
        Me.menuRotateWalls = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuRotateNothing = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.tmrTool = New System.Windows.Forms.Timer()
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
        Me.chkInvalidTiles = New System.Windows.Forms.CheckBox()
        Me.btnGenerator = New System.Windows.Forms.Button()
        Me.menuMiniShowCliffs = New System.Windows.Forms.ToolStripMenuItem()
        'CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControl.SuspendLayout()
        Me.tpTextures.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.tabTextureBrushShape.SuspendLayout()
        'CType(Me.nudTextureBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel6.SuspendLayout()
        Me.tpAutoTexture.SuspendLayout()
        Me.tabAutoTextureBrushShape.SuspendLayout()
        'CType(Me.nudAutoCliffBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        'CType(Me.nudAutoTextureRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpHeight.SuspendLayout()
        Me.tabHeightBrushShape.SuspendLayout()
        Me.tabHeightSetR.SuspendLayout()
        Me.tabHeightSetL.SuspendLayout()
        'CType(Me.nudHeightBrushRadius, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.TabControl.ItemSize = New System.Drawing.Size(72, 28)
        Me.TabControl.Location = New System.Drawing.Point(0, 0)
        Me.TabControl.Margin = New System.Windows.Forms.Padding(0)
        Me.TabControl.Multiline = True
        Me.TabControl.Name = "TabControl"
        Me.TabControl.Padding = New System.Drawing.Point(0,0)
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
        Me.tpTextures.Text = "Textures "
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
        Me.btnTextureFlipX.Image = New Bitmap(InterfaceImagesPath & "selectionflipx.png")
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
        Me.btnTextureClockwise.Image = New Bitmap(InterfaceImagesPath & "selectionrotateclockwise.png")
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
        Me.btnTextureAnticlockwise.Image = New Bitmap(InterfaceImagesPath & "selectionrotateanticlockwise.png")
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
        Me.tpAutoTexture.Controls.Add(Me.chkInvalidTiles)
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
        Me.tpAutoTexture.Text = " Terrain Painter "
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
        Me.tpHeight.Text = " Height "
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
        Me.tpAutoHeight.Text = " Misc "
        Me.tpAutoHeight.UseVisualStyleBackColor = True
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
        Me.tpResize.Text = " Resize "
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
        Me.tpObjects.Text = " Place Objects "
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
        Me.tpObject.Text = " Object Properties "
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
        Me.tsbGateways.Image = New Bitmap(InterfaceImagesPath & "gateways.png")
        Me.tsbGateways.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbGateways.Name = "tsbGateways"
        Me.tsbGateways.Size = New System.Drawing.Size(23, 22)
        Me.tsbGateways.Text = "Gateways"
        '
        'tsbDrawAutotexture
        '
        Me.tsbDrawAutotexture.CheckOnClick = True
        Me.tsbDrawAutotexture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDrawAutotexture.Image = New Bitmap(InterfaceImagesPath & "displayautotexture.png")
        Me.tsbDrawAutotexture.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbDrawAutotexture.Name = "tsbDrawAutotexture"
        Me.tsbDrawAutotexture.Size = New System.Drawing.Size(23, 22)
        Me.tsbDrawAutotexture.Text = "Display Painted Texture Markers"
        '
        'tsbDrawTileOrientation
        '
        Me.tsbDrawTileOrientation.CheckOnClick = True
        Me.tsbDrawTileOrientation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDrawTileOrientation.Image = New Bitmap(InterfaceImagesPath & "drawtileorientation.png")
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
        Me.tsbSave.Image = New Bitmap(InterfaceImagesPath & "save.png")
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
        Me.tsbSelection.Image = New Bitmap(InterfaceImagesPath & "selection.png")
        Me.tsbSelection.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelection.Name = "tsbSelection"
        Me.tsbSelection.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelection.Text = "Selection Tool"
        '
        'tsbSelectionCopy
        '
        Me.tsbSelectionCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionCopy.Image = New Bitmap(InterfaceImagesPath & "selectioncopy.png")
        Me.tsbSelectionCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionCopy.Name = "tsbSelectionCopy"
        Me.tsbSelectionCopy.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionCopy.Text = "Copy Selection"
        '
        'tsbSelectionPasteOptions
        '
        Me.tsbSelectionPasteOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionPasteOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuRotateUnits, Me.menuRotateWalls, Me.menuRotateNothing, Me.ToolStripSeparator10, Me.menuSelPasteHeights, Me.menuSelPasteTextures, Me.menuSelPasteUnits, Me.menuSelPasteGateways, Me.menuSelPasteDeleteUnits, Me.menuSelPasteDeleteGateways})
        Me.tsbSelectionPasteOptions.Image = New Bitmap(InterfaceImagesPath & "selectionpasteoptions.png")
        Me.tsbSelectionPasteOptions.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionPasteOptions.Name = "tsbSelectionPasteOptions"
        Me.tsbSelectionPasteOptions.Size = New System.Drawing.Size(29, 22)
        Me.tsbSelectionPasteOptions.Text = "Paste Options"
        '
        'menuRotateUnits
        '
        Me.menuRotateUnits.Name = "menuRotateUnits"
        Me.menuRotateUnits.Size = New System.Drawing.Size(244, 24)
        Me.menuRotateUnits.Text = "Rotate All Objects"
        '
        'menuRotateWalls
        '
        Me.menuRotateWalls.Checked = True
        Me.menuRotateWalls.CheckState = System.Windows.Forms.CheckState.Checked
        Me.menuRotateWalls.Name = "menuRotateWalls"
        Me.menuRotateWalls.Size = New System.Drawing.Size(244, 24)
        Me.menuRotateWalls.Text = "Rotate Walls Only"
        '
        'menuRotateNothing
        '
        Me.menuRotateNothing.Name = "menuRotateNothing"
        Me.menuRotateNothing.Size = New System.Drawing.Size(244, 24)
        Me.menuRotateNothing.Text = "No Object Rotation"
        '
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
        Me.tsbSelectionPaste.Image = New Bitmap(InterfaceImagesPath & "selectionpaste.png")
        Me.tsbSelectionPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionPaste.Name = "tsbSelectionPaste"
        Me.tsbSelectionPaste.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionPaste.Text = "Paste To Selection"
        '
        'tsbSelectionRotateAnticlockwise
        '
        Me.tsbSelectionRotateAnticlockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionRotateAnticlockwise.Image = New Bitmap(InterfaceImagesPath & "selectionrotateanticlockwise.png")
        Me.tsbSelectionRotateAnticlockwise.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionRotateAnticlockwise.Name = "tsbSelectionRotateAnticlockwise"
        Me.tsbSelectionRotateAnticlockwise.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionRotateAnticlockwise.Text = "Rotate Copy Anticlockwise"
        '
        'tsbSelectionRotateClockwise
        '
        Me.tsbSelectionRotateClockwise.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionRotateClockwise.Image = New Bitmap(InterfaceImagesPath & "selectionrotateclockwise.png")
        Me.tsbSelectionRotateClockwise.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionRotateClockwise.Name = "tsbSelectionRotateClockwise"
        Me.tsbSelectionRotateClockwise.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionRotateClockwise.Text = "Rotate Copy Clockwise"
        '
        'tsbSelectionFlipX
        '
        Me.tsbSelectionFlipX.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionFlipX.Image = New Bitmap(InterfaceImagesPath & "selectionflipx.png")
        Me.tsbSelectionFlipX.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbSelectionFlipX.Name = "tsbSelectionFlipX"
        Me.tsbSelectionFlipX.Size = New System.Drawing.Size(23, 22)
        Me.tsbSelectionFlipX.Text = "Flip Copy Horizontally"
        '
        'tsbSelectionObjects
        '
        Me.tsbSelectionObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbSelectionObjects.Image = New Bitmap(InterfaceImagesPath & "objectsselect.png")
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
        Me.menuFont.Size = New System.Drawing.Size(152, 24)
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
        'menuMiniShowCliffs
        '
        Me.menuMiniShowCliffs.CheckOnClick = True
        Me.menuMiniShowCliffs.Name = "menuMiniShowCliffs"
        Me.menuMiniShowCliffs.Size = New System.Drawing.Size(181, 24)
        Me.menuMiniShowCliffs.Text = "Show Cliffs"
        '
        'chkInvalidTiles
        '
        Me.chkInvalidTiles.Checked = True
        Me.chkInvalidTiles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkInvalidTiles.Location = New System.Drawing.Point(183, 59)
        Me.chkInvalidTiles.Margin = New System.Windows.Forms.Padding(4)
        Me.chkInvalidTiles.Name = "chkInvalidTiles"
        Me.chkInvalidTiles.Size = New System.Drawing.Size(152, 21)
        Me.chkInvalidTiles.TabIndex = 38
        Me.chkInvalidTiles.Text = "Make Invalid Tiles"
        Me.chkInvalidTiles.UseCompatibleTextRendering = True
        Me.chkInvalidTiles.UseVisualStyleBackColor = True
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
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
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
    Friend WithEvents btnGenerator As System.Windows.Forms.Button
    Friend WithEvents menuMiniShowCliffs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chkInvalidTiles As System.Windows.Forms.CheckBox
    Friend WithEvents menuRotateWalls As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents menuRotateNothing As System.Windows.Forms.ToolStripMenuItem
#End If

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
        MinimapSize = Clamp(Val(Result), 0.0#, MaxMinimapSize)
        View_DrawViewLater()
    End Sub
End Class