Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class ctrlMapView
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public DrawPending As Boolean

    Public OpenGLControl As OpenTK.GLControl

    Public GLSize As sXY_int
    Public GLSize_XPerY As Single

    Public Class clsMouseOver
        Public ScreenPos As sXY_int
        Public Class clsOverTerrain
            Public Pos As sWorldPos
            Public Units(-1) As clsMap.clsUnit
            Public UnitCount As Integer
            Public Tile As clsBrush.sPosNum
            Public Vertex As clsBrush.sPosNum
            Public Triangle As Boolean
            Public Side_Num As sXY_int
            Public Side_IsV As Boolean

            Public Sub Unit_FindRemove(ByVal UnitToRemove As clsMap.clsUnit)
                Dim A As Integer

                A = 0
                Do While A < UnitCount
                    If Units(A) Is UnitToRemove Then
                        Unit_Remove(A)
                    Else
                        A += 1
                    End If
                Loop
            End Sub

            Public Sub Units_Clear()

                ReDim Preserve Units(-1)
                UnitCount = 0
            End Sub

            Public Sub Unit_Add(ByVal UnitToAdd As clsMap.clsUnit)

                ReDim Preserve Units(UnitCount)
                Units(UnitCount) = UnitToAdd
                UnitCount += 1
            End Sub

            Public Sub Unit_Remove(ByVal Num As Integer)

                UnitCount -= 1
                If Num <> UnitCount Then
                    Units(Num) = Units(UnitCount)
                End If
                ReDim Preserve Units(UnitCount - 1)
            End Sub
        End Class
        Public OverTerrain As clsOverTerrain
    End Class
    Public MouseOver As clsMouseOver

    Public Class clsMouseDown
        Public Class clsOverTerrain
            Public DownPos As sWorldPos
        End Class
        Public OverTerrain As clsOverTerrain
        Public Class clsOverMinimap
            Public DownPos As sXY_int
        End Class
        Public OverMinimap As clsOverMinimap
    End Class
    Public MouseLeftDown As clsMouseDown
    Public MouseRightDown As clsMouseDown
 
    Public Tiles_Per_Minimap_Pixel As Double

    Public Draw_TileTextures As Boolean = True
    Public Enum enumDrawLighting As Byte
        Off
        Half
        Normal
    End Enum
    Public Draw_Lighting As enumDrawLighting = enumDrawLighting.Half
    Public Draw_TileWireframe As Boolean
    Public Draw_Units As Boolean = True
    Public Draw_VertexTerrain As Boolean
    Public Draw_Gateways As Boolean

    Public BGColor As sRGB_sng

    Enum enumView_Move_Type As Byte
        Free
        RTS
    End Enum
    Public ViewMoveType As enumView_Move_Type = enumView_Move_Type.RTS
    Public RTSOrbit As Boolean = True
    Public ViewPos As sXYZ_int
    Public ViewAngleMatrix As New Matrix3D.Matrix3D
    Public ViewAngleMatrix_Inverted As New Matrix3D.Matrix3D
    Public ViewAngleRPY As Matrix3D.AngleRPY
    Public FieldOfViewY As Single

    Public SunAngleMatrix As New Matrix3D.Matrix3D

    Public FOVMultiplierExponent As Double
    Public FOVMultiplier As Double

    Public DrawView_Enabled As Boolean = False

    Public VisionSectors As New clsBrush(0.0#, clsBrush.enumShape.Circle)

    Private GLInitializeDelayTimer As Timer
    Public IsGLInitialized As Boolean = False

    Private WithEvents tmrDraw As Timer
    Private WithEvents tmrDrawDelay As Timer

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ListSelect = New ContextMenuStrip
        UndoMessageTimer = New Timer

        ' Add any initialization after the InitializeComponent() call.
        OpenGLControl = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(32), 24, 0))
        OpenGLControl.MakeCurrent() 'mono version fails without this
        pnlDraw.Controls.Add(OpenGLControl)

        Matrix3D.MatrixSetToPY(SunAngleMatrix, New Matrix3D.AnglePY(-22.5# * RadOf1Deg, 157.5# * RadOf1Deg))

        GLInitializeDelayTimer = New Timer
        GLInitializeDelayTimer.Interval = 1
        AddHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Enabled = True

        tmrDraw = New Timer
        tmrDraw.Interval = 1

        tmrDrawDelay = New Timer
        tmrDrawDelay.Interval = 30

        UndoMessageTimer.Interval = 4000
    End Sub

    Public Sub OpenGL_Size_Calc()

        OpenGLControl.Width = pnlDraw.Width
        OpenGLControl.Height = pnlDraw.Height
        Viewport_Resize()
    End Sub

    Public Sub DrawView_SetEnabled(ByVal Value As Boolean)

        If Value Then
            If Not DrawView_Enabled Then
                DrawView_Enabled = True
                DrawViewLater()
            End If
        Else
            tmrDraw.Enabled = False
            DrawView_Enabled = False
        End If
    End Sub

    Public Sub DrawViewLater()

        DrawPending = True
        If Not tmrDrawDelay.Enabled Then
            tmrDraw.Enabled = True
        End If
    End Sub

    Private Sub tmrDraw_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDraw.Tick

        tmrDraw.Enabled = False
        If DrawPending Then
            DrawView()
            DrawPending = False
            tmrDrawDelay.Enabled = True
        End If
    End Sub

    Public Sub FOV_Calc()

        FieldOfViewY = CSng(Clamp_dbl(Math.Atan(GLSize.Y * FOVMultiplier / 2.0#) * 2.0#, RadOf1Deg, 179.0# * RadOf1Deg))

        DrawViewLater()
    End Sub

    Private Sub GLInitialize(ByVal sender As Object, ByVal e As EventArgs)

        IsGLInitialized = True

        GLInitializeDelayTimer.Enabled = False
        RemoveHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Dispose()
        GLInitializeDelayTimer = Nothing

        OpenGL_Size_Calc()

        AddHandler OpenGLControl.MouseDown, AddressOf OpenGL_MouseDown
        AddHandler OpenGLControl.MouseUp, AddressOf OpenGL_MouseUp
        AddHandler OpenGLControl.MouseWheel, AddressOf OpenGL_MouseWheel
        AddHandler OpenGLControl.MouseMove, AddressOf OpenGL_MouseMove
        AddHandler OpenGLControl.MouseEnter, AddressOf OpenGL_MouseEnter
        AddHandler OpenGLControl.MouseLeave, AddressOf OpenGL_MouseLeave
        AddHandler OpenGLControl.Resize, AddressOf OpenGL_Resize
        AddHandler OpenGLControl.LostFocus, AddressOf OpenGL_LostFocus
        AddHandler OpenGLControl.PreviewKeyDown, AddressOf OpenGL_KeyDown
        AddHandler OpenGLControl.KeyUp, AddressOf OpenGL_KeyUp
        AddHandler OpenGLControl.Paint, AddressOf OpenGL_Paint

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Enable(EnableCap.DepthTest)

        Dim ambient(3) As Single
        Dim specular(3) As Single
        Dim diffuse(3) As Single

        ambient(0) = 0.333333343F
        ambient(1) = 0.333333343F
        ambient(2) = 0.333333343F
        ambient(3) = 1.0F
        specular(0) = 0.6666667F
        specular(1) = 0.6666667F
        specular(2) = 0.6666667F
        specular(3) = 1.0F
        diffuse(0) = 0.75F
        diffuse(1) = 0.75F
        diffuse(2) = 0.75F
        diffuse(3) = 1.0F
        GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse)
        GL.Light(LightName.Light0, LightParameter.Specular, specular)
        GL.Light(LightName.Light0, LightParameter.Ambient, ambient)

        ambient(0) = 0.25F
        ambient(1) = 0.25F
        ambient(2) = 0.25F
        ambient(3) = 1.0F
        specular(0) = 0.5F
        specular(1) = 0.5F
        specular(2) = 0.5F
        specular(3) = 1.0F
        diffuse(0) = 0.5625F
        diffuse(1) = 0.5625F
        diffuse(2) = 0.5625F
        diffuse(3) = 1.0F
        GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse)
        GL.Light(LightName.Light1, LightParameter.Specular, specular)
        GL.Light(LightName.Light1, LightParameter.Ambient, ambient)

        Dim mat_diffuse(3) As Single
        Dim mat_specular(3) As Single
        Dim mat_ambient(3) As Single
        Dim mat_shininess(0) As Single

        mat_specular(0) = 0.0F
        mat_specular(1) = 0.0F
        mat_specular(2) = 0.0F
        mat_specular(3) = 0.0F
        mat_ambient(0) = 1.0F
        mat_ambient(1) = 1.0F
        mat_ambient(2) = 1.0F
        mat_ambient(3) = 1.0F
        mat_diffuse(0) = 1.0F
        mat_diffuse(1) = 1.0F
        mat_diffuse(2) = 1.0F
        mat_diffuse(3) = 1.0F
        mat_shininess(0) = 0.0F

        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, mat_ambient)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, mat_specular)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, mat_diffuse)
        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, mat_shininess)

        FOV_Multiplier_Set(0.0005#)
    End Sub

    Public Sub Viewport_Resize()

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If
        GL.Viewport(0, 0, GLSize.X, GLSize.Y)

        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Flush()
        OpenGLControl.SwapBuffers()
        Refresh()

        DrawViewLater()
    End Sub

    Public Sub FOV_Scale_2E_Set(ByVal Power As Double)

        FOVMultiplierExponent = Power
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent
        FOV_Calc()
    End Sub

    Public Sub FOV_Scale_2E_Change(ByVal Power_Change As Double)

        FOVMultiplierExponent += Power_Change
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent
        FOV_Calc()
    End Sub

    Public Sub FOV_Set(ByVal Radians As Double)

        FOVMultiplier = Math.Tan(Radians / 2.0#) / GLSize.Y * 2.0#
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
        FOV_Calc()
    End Sub

    Public Sub FOV_Multiplier_Set(ByVal Value As Double)

        FOVMultiplier = Value
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
        FOV_Calc()
    End Sub

    Private Sub DrawView()

        If Not (DrawView_Enabled And IsGLInitialized) Then
            Exit Sub
        End If

        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim Footprint As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim ColourA As sRGBA_sng
        Dim ColourB As sRGBA_sng
        Dim ShowMinimapViewPosBox As Boolean
        Dim ViewCorner0 As sXY_dbl
        Dim ViewCorner1 As sXY_dbl
        Dim ViewCorner2 As sXY_dbl
        Dim ViewCorner3 As sXY_dbl
        Dim dblTemp As Double
        Dim dblTemp2 As Double
        Dim Vertex0 As Matrix3D.XYZ_dbl
        Dim Vertex1 As Matrix3D.XYZ_dbl
        Dim Vertex2 As Matrix3D.XYZ_dbl
        Dim Vertex3 As Matrix3D.XYZ_dbl
        Dim ScreenPos As sXY_int
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim WorldPos As sWorldPos
        Dim PosA As sXY_dbl
        Dim PosB As sXY_dbl
        Dim PosC As sXY_dbl
        Dim PosD As sXY_dbl
        Dim MinimapSizeXY As sXY_int
        Dim tmpUnit As clsMap.clsUnit
        Dim StartXY As sXY_int
        Dim FinishXY As sXY_int
        Dim DrawIt As Boolean
        Dim DrawCentre As sXY_dbl
        Dim DrawCentreSector As clsBrush.sPosNum
        Dim SelectionLabel As New clsTextLabel
        Dim light_position(3) As Single
        Dim matrixA As New Matrix3D.Matrix3D
        Dim matrixB As New Matrix3D.Matrix3D
        Dim MapAction As clsMap.clsAction

        Dim DrawObjects As New clsMap.clsDrawSectorObjects
        DrawObjects.MapView = Me
        DrawObjects.Map = Main_Map
        DrawObjects.UnitTextLabels = New clsTextLabels(64)
        DrawObjects.Start()

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        GL.Enable(EnableCap.DepthTest)
        GL.ClearColor(BGColor.Red, BGColor.Green, BGColor.Blue, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreatePerspectiveFieldOfView(FieldOfViewY, GLSize_XPerY, 8.0F, 65536.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        Matrix3D.MatrixRotationByMatrix(ViewAngleMatrix_Inverted, SunAngleMatrix, matrixB)
        Matrix3D.VectorForwardsRotationByMatrix(matrixB, XYZ_dbl)
        light_position(0) = CSng(XYZ_dbl.X)
        light_position(1) = CSng(XYZ_dbl.Y)
        light_position(2) = CSng(-XYZ_dbl.Z)
        light_position(3) = 0.0F
        GL.Light(LightName.Light0, LightParameter.Position, light_position)
        GL.Light(LightName.Light1, LightParameter.Position, light_position)

        GL.Disable(EnableCap.Light0)
        GL.Disable(EnableCap.Light1)
        If Draw_Lighting <> enumDrawLighting.Off Then
            If Draw_Lighting = enumDrawLighting.Half Then
                GL.Enable(EnableCap.Light0)
            ElseIf Draw_Lighting = enumDrawLighting.Normal Then
                GL.Enable(EnableCap.Light1)
            End If
            GL.Enable(EnableCap.Lighting)
        Else
            GL.Disable(EnableCap.Lighting)
        End If

        dblTemp = 127.5# * Main_Map.HeightMultiplier
        If ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, 0, dblTemp, ViewCorner0) _
        And ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, 0, dblTemp, ViewCorner1) _
        And ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, GLSize.Y, dblTemp, ViewCorner2) _
        And ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, GLSize.Y, dblTemp, ViewCorner3) Then
            ShowMinimapViewPosBox = True
        Else
            ShowMinimapViewPosBox = False
        End If

        If ScreenXY_Get_ViewPlanePos(New sXY_int(CInt(GLSize.X / 2.0#), CInt(GLSize.Y / 2.0#)), dblTemp, DrawCentre) Then
            XYZ_dbl.X = DrawCentre.X - ViewPos.X
            XYZ_dbl.Z = DrawCentre.Y - ViewPos.Z
            dblTemp2 = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z)
            If dblTemp2 > VisionRadius * 2.0# Then
                DrawCentre.X = ViewPos.X + XYZ_dbl.X * VisionRadius * 2.0# / dblTemp2
                DrawCentre.Y = ViewPos.Z + XYZ_dbl.Z * VisionRadius * 2.0# / dblTemp2
            End If
        Else
            Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, XYZ_dbl)
            dblTemp2 = VisionRadius * 2.0# / Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z)
            DrawCentre.X = ViewPos.X + XYZ_dbl.X * dblTemp2
            DrawCentre.Y = ViewPos.Z + XYZ_dbl.Z * dblTemp2
        End If
        DrawCentre.X = Clamp_dbl(DrawCentre.X, 0.0#, Main_Map.Terrain.TileSize.X * TerrainGridSpacing - 1.0#)
        DrawCentre.Y = Clamp_dbl(-DrawCentre.Y, 0.0#, Main_Map.Terrain.TileSize.Y * TerrainGridSpacing - 1.0#)
        DrawCentreSector.Normal = Main_Map.GetPosSectorNum(New sXY_int(CInt(DrawCentre.X), CInt(DrawCentre.Y)))
        DrawCentreSector.Alignment = Main_Map.GetPosSectorNum(New sXY_int(CInt(DrawCentre.X - SectorTileSize * TerrainGridSpacing / 2.0#), CInt(DrawCentre.Y - SectorTileSize * TerrainGridSpacing / 2.0#)))

        GL.Rotate(AngleClamp(-ViewAngleRPY.Roll) / RadOf1Deg, 0.0F, 0.0F, 1.0F)
        GL.Rotate(ViewAngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
        GL.Rotate(ViewAngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
        GL.Translate(-ViewPos.X, -ViewPos.Y, ViewPos.Z)

        GL.Enable(EnableCap.CullFace)

        If Draw_TileTextures Then
            GL.Color3(1.0F, 1.0F, 1.0F)
            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
            MapAction = New clsMap.clsDrawCallTerrain
            MapAction.Map = Main_Map
            VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector)
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)
        GL.Disable(EnableCap.Lighting)

        If Draw_TileWireframe Then
            GL.Color3(0.0F, 1.0F, 0.0F)
            GL.LineWidth(1.0F)
            Dim DrawCallTerrainWireframe As New clsMap.clsDrawCallTerrainWireframe
            DrawCallTerrainWireframe.Map = Main_Map
            VisionSectors.PerformActionMapSectors(DrawCallTerrainWireframe, DrawCentreSector)
        End If

        'draw tile orientation markers

        If DisplayTileOrientation Then

            GL.Disable(EnableCap.CullFace)

            GL.Begin(BeginMode.Triangles)
            GL.Color3(1.0F, 1.0F, 0.0F)
            MapAction = New clsMap.clsDrawTileOrientation
            MapAction.Map = Main_Map
            VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector)
            GL.End()

            GL.Enable(EnableCap.CullFace)
        End If

        'draw painted texture terrain type markers

        Dim RGB_sng As sRGB_sng

        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If Draw_VertexTerrain Then
            GL.LineWidth(1.0F)
            Dim DrawVertexTerran As New clsMap.clsDrawVertexTerrain
            DrawVertexTerran.Map = Main_Map
            DrawVertexTerran.ViewAngleMatrix = ViewAngleMatrix
            VisionSectors.PerformActionMapSectors(DrawVertexTerran, DrawCentreSector)
        End If

        SelectionLabel.Text = ""

        If Main_Map.Selected_Area_VertexA IsNot Nothing Then
            DrawIt = False
            If Main_Map.Selected_Area_VertexB IsNot Nothing Then
                'area is selected
                XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
                XYZ_dbl.X = Main_Map.Selected_Area_VertexB.X * TerrainGridSpacing - ViewPos.X
                XYZ_dbl.Z = -Main_Map.Selected_Area_VertexB.Y * TerrainGridSpacing - ViewPos.Z
                XYZ_dbl.Y = Main_Map.GetVertexAltitude(Main_Map.Selected_Area_VertexB.XY) - ViewPos.Y
                DrawIt = True
            ElseIf Tool = enumTool.Terrain_Select Then
                If MouseOverTerrain IsNot Nothing Then
                    'selection is changing under pointer
                    XY_Reorder(Main_Map.Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                    XYZ_dbl.X = MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing - ViewPos.X
                    XYZ_dbl.Z = -MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing - ViewPos.Z
                    XYZ_dbl.Y = Main_Map.GetVertexAltitude(MouseOverTerrain.Vertex.Normal) - ViewPos.Y
                    DrawIt = True
                End If
            End If
            If DrawIt Then
                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                If Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos) Then
                    If ScreenPos.X >= 0 And ScreenPos.X <= GLSize.X And ScreenPos.Y >= 0 And ScreenPos.Y <= GLSize.Y Then
                        SelectionLabel.Colour.Red = 1.0F
                        SelectionLabel.Colour.Green = 1.0F
                        SelectionLabel.Colour.Blue = 1.0F
                        SelectionLabel.Colour.Alpha = 1.0F
                        SelectionLabel.TextFont = UnitLabelFont
                        SelectionLabel.SizeY = Settings.DisplayFont.SizeInPoints
                        SelectionLabel.Pos = ScreenPos
                        SelectionLabel.Text = FinishXY.X - StartXY.X & "x" & FinishXY.Y - StartXY.Y
                    End If
                End If
                GL.LineWidth(3.0F)
                For X = StartXY.X To FinishXY.X - 1
                    Vertex0.X = X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X, StartXY.Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                    Vertex1.X = (X + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X + 1, StartXY.Y).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For X = StartXY.X To FinishXY.X - 1
                    Vertex0.X = X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X, FinishXY.Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                    Vertex1.X = (X + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X + 1, FinishXY.Y).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For Y = StartXY.Y To FinishXY.Y - 1
                    Vertex0.X = StartXY.X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(StartXY.X, Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y * TerrainGridSpacing
                    Vertex1.X = StartXY.X * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(StartXY.X, Y + 1).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For Y = StartXY.Y To FinishXY.Y - 1
                    Vertex0.X = FinishXY.X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(FinishXY.X, Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y * TerrainGridSpacing
                    Vertex1.X = FinishXY.X * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(FinishXY.X, Y + 1).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
            End If
        End If

        If Tool = enumTool.Terrain_Select Then
            If MouseOverTerrain IsNot Nothing Then
                'draw mouseover vertex
                GL.LineWidth(3.0F)

                Vertex0.X = MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing
                Vertex0.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * Main_Map.HeightMultiplier
                Vertex0.Z = -MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing
                GL.Begin(BeginMode.Lines)
                GL.Color3(1.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
                GL.End()
            End If
        End If

        If Draw_Gateways Then
            GL.LineWidth(2.0F)
            For A = 0 To Main_Map.GatewayCount - 1
                If Main_Map.Gateways(A).PosA.X = Main_Map.Gateways(A).PosB.X Then
                    If Main_Map.Gateways(A).PosA.Y <= Main_Map.Gateways(A).PosB.Y Then
                        C = Main_Map.Gateways(A).PosA.Y
                        D = Main_Map.Gateways(A).PosB.Y
                    Else
                        C = Main_Map.Gateways(A).PosB.Y
                        D = Main_Map.Gateways(A).PosA.Y
                    End If
                    X2 = Main_Map.Gateways(A).PosA.X
                    For Y2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -Y2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -Y2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                        Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                        Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.LineLoop)
                        GL.Color3(0.75F, 1.0F, 0.0F)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                        GL.End()
                    Next
                ElseIf Main_Map.Gateways(A).PosA.Y = Main_Map.Gateways(A).PosB.Y Then
                    If Main_Map.Gateways(A).PosA.X <= Main_Map.Gateways(A).PosB.X Then
                        C = Main_Map.Gateways(A).PosA.X
                        D = Main_Map.Gateways(A).PosB.X
                    Else
                        C = Main_Map.Gateways(A).PosB.X
                        D = Main_Map.Gateways(A).PosA.X
                    End If
                    Y2 = Main_Map.Gateways(A).PosA.Y
                    For X2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -Y2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -Y2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                        Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                        Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.LineLoop)
                        GL.Color3(0.75F, 1.0F, 0.0F)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                        GL.End()
                    Next
                Else
                    'draw invalid gateways as red tile borders
                    X2 = Main_Map.Gateways(A).PosA.X
                    Y2 = Main_Map.Gateways(A).PosA.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(1.0F, 0.0F, 0.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    X2 = Main_Map.Gateways(A).PosB.X
                    Y2 = Main_Map.Gateways(A).PosB.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(1.0F, 0.0F, 0.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()
                End If
            Next
        End If

        If MouseOverTerrain IsNot Nothing Then

            If Tool = enumTool.None Then
                If Main_Map.Unit_Selected_Area_VertexA IsNot Nothing Then
                    'selection is changing under pointer
                    XY_Reorder(Main_Map.Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                    GL.LineWidth(2.0F)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(X, StartXY.Y).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(X + 1, StartXY.Y).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(X, FinishXY.Y).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(X + 1, FinishXY.Y).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Y = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = StartXY.X * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(StartXY.X, Y).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -Y * TerrainGridSpacing
                        Vertex1.X = StartXY.X * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(StartXY.X, Y + 1).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Y = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = FinishXY.X * TerrainGridSpacing
                        Vertex0.Y = Main_Map.Terrain.Vertices(FinishXY.X, Y).Height * Main_Map.HeightMultiplier
                        Vertex0.Z = -Y * TerrainGridSpacing
                        Vertex1.X = FinishXY.X * TerrainGridSpacing
                        Vertex1.Y = Main_Map.Terrain.Vertices(FinishXY.X, Y + 1).Height * Main_Map.HeightMultiplier
                        Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                Else
                    GL.LineWidth(2.0F)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Begin(BeginMode.Lines)
                    GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0#, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0#)
                    GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0#, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0#)
                    GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0#, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0#)
                    GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0#, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0#)
                    GL.End()
                End If
            End If

            If Tool = enumTool.AutoRoad_Place Then
                GL.LineWidth(2.0F)

                If MouseOverTerrain.Side_IsV Then
                    Vertex0.X = MouseOverTerrain.Side_Num.X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = MouseOverTerrain.Side_Num.X * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y + 1).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -(MouseOverTerrain.Side_Num.Y + 1) * TerrainGridSpacing
                Else
                    Vertex0.X = MouseOverTerrain.Side_Num.X * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = (MouseOverTerrain.Side_Num.X + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Side_Num.X + 1, MouseOverTerrain.Side_Num.Y).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                End If

                GL.Begin(BeginMode.Lines)
                GL.Color3(0.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.End()
            ElseIf Tool = enumTool.AutoRoad_Line Or Tool = enumTool.Gateways Then
                GL.LineWidth(2.0F)

                If Main_Map.Selected_Tile_A IsNot Nothing Then
                    X2 = Main_Map.Selected_Tile_A.X
                    Y2 = Main_Map.Selected_Tile_A.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    If MouseOverTerrain.Tile.Normal.X = Main_Map.Selected_Tile_A.X Then
                        If MouseOverTerrain.Tile.Normal.Y <= Main_Map.Selected_Tile_A.Y Then
                            A = MouseOverTerrain.Tile.Normal.Y
                            B = Main_Map.Selected_Tile_A.Y
                        Else
                            A = Main_Map.Selected_Tile_A.Y
                            B = MouseOverTerrain.Tile.Normal.Y
                        End If
                        X2 = Main_Map.Selected_Tile_A.X
                        For Y2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                            Vertex0.Z = -Y2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                            Vertex1.Z = -Y2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                            Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                            Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                            GL.Begin(BeginMode.LineLoop)
                            GL.Color3(0.0F, 1.0F, 1.0F)
                            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                            GL.End()
                        Next
                    ElseIf MouseOverTerrain.Tile.Normal.Y = Main_Map.Selected_Tile_A.Y Then
                        If MouseOverTerrain.Tile.Normal.X <= Main_Map.Selected_Tile_A.X Then
                            A = MouseOverTerrain.Tile.Normal.X
                            B = Main_Map.Selected_Tile_A.X
                        Else
                            A = Main_Map.Selected_Tile_A.X
                            B = MouseOverTerrain.Tile.Normal.X
                        End If
                        Y2 = Main_Map.Selected_Tile_A.Y
                        For X2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                            Vertex0.Z = -Y2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                            Vertex1.Z = -Y2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                            Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                            Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                            GL.Begin(BeginMode.LineLoop)
                            GL.Color3(0.0F, 1.0F, 1.0F)
                            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                            GL.End()
                        Next
                    End If
                Else
                    X2 = MouseOverTerrain.Tile.Normal.X
                    Y2 = MouseOverTerrain.Tile.Normal.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Main_Map.Terrain.Vertices(X2, Y2).Height * Main_Map.HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2).Height * Main_Map.HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Main_Map.Terrain.Vertices(X2, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Main_Map.Terrain.Vertices(X2 + 1, Y2 + 1).Height * Main_Map.HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()
                End If
            End If

            'draw mouseover tiles

            Dim ToolBrush As clsBrush

            Select Case Tool
                Case enumTool.Texture_Brush
                    ToolBrush = TextureBrush
                Case enumTool.AutoCliff
                    ToolBrush = CliffBrush
                Case enumTool.AutoCliffRemove
                    ToolBrush = CliffBrush
                Case enumTool.AutoRoad_Remove
                    ToolBrush = CliffBrush
                Case Else
                    ToolBrush = Nothing
            End Select

            If ToolBrush IsNot Nothing Then
                GL.LineWidth(2.0F)
                Dim DrawTileOutline As New clsMap.clsDrawTileOutline
                DrawTileOutline.Map = Main_Map
                DrawTileOutline.Colour.Red = 0.0F
                DrawTileOutline.Colour.Green = 1.0F
                DrawTileOutline.Colour.Blue = 1.0F
                DrawTileOutline.Colour.Alpha = 1.0F
                ToolBrush.PerformActionMapTiles(DrawTileOutline, MouseOverTerrain.Tile)
            End If

            'draw mouseover vertex
            If Tool = enumTool.AutoTexture_Fill Then
                GL.LineWidth(2.0F)

                Vertex0.X = MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing
                Vertex0.Y = Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * Main_Map.HeightMultiplier
                Vertex0.Z = -MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing
                GL.Begin(BeginMode.Lines)
                GL.Color3(0.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
                GL.End()
            End If

            Select Case Tool
                Case enumTool.AutoTexture_Place
                    ToolBrush = TerrainBrush
                Case enumTool.Height_Set_Brush
                    ToolBrush = HeightBrush
                Case enumTool.Height_Change_Brush
                    ToolBrush = HeightBrush
                Case enumTool.Height_Smooth_Brush
                    ToolBrush = HeightBrush
                Case Else
                    ToolBrush = Nothing
            End Select

            If ToolBrush IsNot Nothing Then
                GL.LineWidth(2.0F)
                Dim DrawVertexMarker As New clsMap.clsDrawVertexMarker
                DrawVertexMarker.Map = Main_Map
                DrawVertexMarker.Colour.Red = 0.0F
                DrawVertexMarker.Colour.Green = 1.0F
                DrawVertexMarker.Colour.Blue = 1.0F
                DrawVertexMarker.Colour.Alpha = 1.0F
                ToolBrush.PerformActionMapVertices(DrawVertexMarker, MouseOverTerrain.Vertex)
            End If
        End If

        GL.Enable(EnableCap.DepthTest)

        GL.Disable(EnableCap.CullFace)

        GL.LoadIdentity()
        GL.Rotate(AngleClamp(-ViewAngleRPY.Roll) / RadOf1Deg, 0.0F, 0.0F, 1.0F)
        GL.Rotate(ViewAngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
        GL.Rotate(ViewAngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)

        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)

        If Draw_Units Then
            GL.Color3(1.0F, 1.0F, 1.0F)
            GL.Enable(EnableCap.Texture2D)
            VisionSectors.PerformActionMapSectors(DrawObjects, DrawCentreSector)
            GL.Disable(EnableCap.Texture2D)
        End If

        If MouseOverTerrain IsNot Nothing Then
            GL.Enable(EnableCap.Texture2D)
            If Tool = enumTool.ObjectPlace Then
                If frmMainInstance.SelectedObjectType IsNot Nothing Then
                    WorldPos = Main_Map.TileAligned_Pos_From_MapPos(MouseOverTerrain.Pos.Horizontal, frmMainInstance.SelectedObjectType.GetFootprint)
                    GL.PushMatrix()
                    GL.Translate(WorldPos.Horizontal.X - ViewPos.X, WorldPos.Altitude - ViewPos.Y + 2.0#, ViewPos.Z + WorldPos.Horizontal.Y)
                    frmMainInstance.SelectedObjectType.GLDraw(0.0F)
                    GL.PopMatrix()
                End If
            End If
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)

        'draw unit selection

        GL.Begin(BeginMode.Quads)
        For A = 0 To Main_Map.SelectedUnitCount - 1
            tmpUnit = Main_Map.SelectedUnits(A)
            XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - ViewPos.X
            XYZ_dbl.Y = tmpUnit.Pos.Altitude - ViewPos.Y
            XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - ViewPos.Z
            Footprint = tmpUnit.Type.GetFootprint
            RGB_sng = Main_Map.GetUnitGroupColour(tmpUnit.UnitGroup)
            ColourA = New sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F)
            ColourB = New sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.75F)
            DrawUnitRectangle(XYZ_dbl, Footprint, 8.0#, ColourA, ColourB)
        Next
        If MouseOverTerrain IsNot Nothing Then
            For A = 0 To MouseOverTerrain.UnitCount - 1
                If MouseOverTerrain.Units(A) IsNot Nothing And Tool = enumTool.None Then
                    tmpUnit = MouseOverTerrain.Units(A)
                    XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - ViewPos.X
                    XYZ_dbl.Y = tmpUnit.Pos.Altitude - ViewPos.Y
                    XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - ViewPos.Z
                    RGB_sng = Main_Map.GetUnitGroupColour(tmpUnit.UnitGroup)
                    GL.Color4((0.5F + RGB_sng.Red) / 1.5F, (0.5F + RGB_sng.Green) / 1.5F, (0.5F + RGB_sng.Blue) / 1.5F, 0.75F)
                    Footprint = tmpUnit.Type.GetFootprint
                    ColourA = New sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F)
                    ColourB = New sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.875F)
                    DrawUnitRectangle(XYZ_dbl, Footprint, 16.0#, ColourA, ColourB)
                End If
            Next
        End If
        GL.End()

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, CSng(GLSize.X), CSng(GLSize.Y), 0.0F, -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        GL.Enable(EnableCap.Texture2D)

        DrawObjects.UnitTextLabels.Draw()
        SelectionLabel.Draw()

        GL.Disable(EnableCap.Texture2D)

        GL.Disable(EnableCap.Blend)

        'draw minimap

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, CSng(0.0F), CSng(GLSize.Y), -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        If Main_Map.Minimap_Texture_Size > 0 And Main_Map.Minimap_GLTexture > 0 Then
            dblTemp = Settings.MinimapSize
            Tiles_Per_Minimap_Pixel = Math.Sqrt(Main_Map.Terrain.TileSize.X * Main_Map.Terrain.TileSize.X + Main_Map.Terrain.TileSize.Y * Main_Map.Terrain.TileSize.Y) / (RootTwo * dblTemp)

            MinimapSizeXY.X = CInt(Main_Map.Terrain.TileSize.X / Tiles_Per_Minimap_Pixel)
            MinimapSizeXY.Y = CInt(Main_Map.Terrain.TileSize.Y / Tiles_Per_Minimap_Pixel)

            GL.Translate(0.0F, GLSize.Y - MinimapSizeXY.Y, 0.0F)

            XYZ_dbl.X = Main_Map.Terrain.TileSize.X / Main_Map.Minimap_Texture_Size
            XYZ_dbl.Z = Main_Map.Terrain.TileSize.Y / Main_Map.Minimap_Texture_Size

            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Decal)
            GL.BindTexture(TextureTarget.Texture2D, Main_Map.Minimap_GLTexture)

            GL.Begin(BeginMode.Quads)

            GL.TexCoord2(0.0F, 0.0F)
            GL.Vertex2(0.0F, MinimapSizeXY.Y)

            GL.TexCoord2(XYZ_dbl.X, 0.0F)
            GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y)

            GL.TexCoord2(XYZ_dbl.X, XYZ_dbl.Z)
            GL.Vertex2(MinimapSizeXY.X, 0.0F)

            GL.TexCoord2(0.0F, XYZ_dbl.Z)
            GL.Vertex2(0.0F, 0.0F)

            GL.End()

            GL.Disable(EnableCap.Texture2D)

            'draw minimap border

            GL.LineWidth(1.0F)
            GL.Begin(BeginMode.Lines)
            GL.Color3(0.75F, 0.75F, 0.75F)
            GL.Vertex2(MinimapSizeXY.X, 0.0F)
            GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y)
            GL.Vertex2(0.0F, 0.0F)
            GL.Vertex2(MinimapSizeXY.X, 0.0F)
            GL.End()

            'draw minimap view pos box

            If ShowMinimapViewPosBox Then
                dblTemp = TerrainGridSpacing * Tiles_Per_Minimap_Pixel

                PosA.X = ViewCorner0.X / dblTemp
                PosA.Y = MinimapSizeXY.Y + ViewCorner0.Y / dblTemp
                PosB.X = ViewCorner1.X / dblTemp
                PosB.Y = MinimapSizeXY.Y + ViewCorner1.Y / dblTemp
                PosC.X = ViewCorner2.X / dblTemp
                PosC.Y = MinimapSizeXY.Y + ViewCorner2.Y / dblTemp
                PosD.X = ViewCorner3.X / dblTemp
                PosD.Y = MinimapSizeXY.Y + ViewCorner3.Y / dblTemp

                GL.LineWidth(1.0F)
                GL.Begin(BeginMode.LineLoop)
                GL.Color3(1.0F, 1.0F, 1.0F)
                GL.Vertex2(PosA.X, PosA.Y)
                GL.Vertex2(PosB.X, PosB.Y)
                GL.Vertex2(PosC.X, PosC.Y)
                GL.Vertex2(PosD.X, PosD.Y)
                GL.End()
            End If

            If Main_Map.Selected_Area_VertexA IsNot Nothing Then
                DrawIt = False
                If Main_Map.Selected_Area_VertexB IsNot Nothing Then
                    'area is selected
                    XY_Reorder(Main_Map.Selected_Area_VertexA.XY, Main_Map.Selected_Area_VertexB.XY, StartXY, FinishXY)
                    DrawIt = True
                ElseIf Tool = enumTool.Terrain_Select Then
                    If MouseOverTerrain IsNot Nothing Then
                        'selection is changing under mouse
                        XY_Reorder(Main_Map.Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                        DrawIt = True
                    End If
                End If
                If DrawIt Then
                    GL.LineWidth(1.0F)
                    PosA.X = StartXY.X / Tiles_Per_Minimap_Pixel
                    PosA.Y = MinimapSizeXY.Y - StartXY.Y / Tiles_Per_Minimap_Pixel
                    PosB.X = FinishXY.X / Tiles_Per_Minimap_Pixel
                    PosB.Y = MinimapSizeXY.Y - StartXY.Y / Tiles_Per_Minimap_Pixel
                    PosC.X = FinishXY.X / Tiles_Per_Minimap_Pixel
                    PosC.Y = MinimapSizeXY.Y - FinishXY.Y / Tiles_Per_Minimap_Pixel
                    PosD.X = StartXY.X / Tiles_Per_Minimap_Pixel
                    PosD.Y = MinimapSizeXY.Y - FinishXY.Y / Tiles_Per_Minimap_Pixel
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex2(PosA.X, PosA.Y)
                    GL.Vertex2(PosB.X, PosB.Y)
                    GL.Vertex2(PosC.X, PosC.Y)
                    GL.Vertex2(PosD.X, PosD.Y)
                    GL.End()
                End If
            End If
        End If

        GL.Flush()
        OpenGLControl.SwapBuffers()

        Refresh()
    End Sub

    Public Sub ViewPosSet(ByVal NewViewPos As sXYZ_int)

        ViewPos.X = Clamp_int(NewViewPos.X, -Main_Map.Terrain.TileSize.X * TerrainGridSpacing - 2048, Main_Map.Terrain.TileSize.X * TerrainGridSpacing * 2 + 2048)
        ViewPos.Z = Clamp_int(NewViewPos.Z, -Main_Map.Terrain.TileSize.Y * TerrainGridSpacing * 2 - 2048, Main_Map.Terrain.TileSize.X * TerrainGridSpacing + 2048)
        ViewPos.Y = Clamp_int(NewViewPos.Y, CInt(Math.Ceiling(Main_Map.GetTerrainHeight(New sXY_int(ViewPos.X, -ViewPos.Z)))) + 16, 49152)

        DrawViewLater()
    End Sub

    Public Sub ViewPosChange(ByVal Displacement As sXYZ_int)

        ViewPos.X = Clamp_int(ViewPos.X + Displacement.X, -Main_Map.Terrain.TileSize.X * TerrainGridSpacing - 2048, Main_Map.Terrain.TileSize.X * TerrainGridSpacing * 2 + 2048)
        ViewPos.Z = Clamp_int(ViewPos.Z + Displacement.Z, -Main_Map.Terrain.TileSize.Y * TerrainGridSpacing * 2 - 2048, Main_Map.Terrain.TileSize.X * TerrainGridSpacing + 2048)
        ViewPos.Y = Clamp_int(ViewPos.Y + Displacement.Y, CInt(Math.Ceiling(Main_Map.GetTerrainHeight(New sXY_int(ViewPos.X, -ViewPos.Z)))) + 16, 49152)

        DrawViewLater()
    End Sub

    Public Sub ViewAngleSet(ByVal NewMatrix As Matrix3D.Matrix3D)

        Matrix3D.MatrixCopy(NewMatrix, ViewAngleMatrix)
        Matrix3D.MatrixNormalize(ViewAngleMatrix)
        Matrix3D.MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)
        Matrix3D.MatrixToRPY(ViewAngleMatrix, ViewAngleRPY)
    End Sub

    Public Sub ViewAngleSetToDefault()

        Dim matrixA As New Matrix3D.Matrix3D
        Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
        ViewAngleSet(matrixA)
    End Sub

    Public Sub ViewAngleSet_Rotate(ByVal NewMatrix As Matrix3D.Matrix3D)
        Dim Flag As Boolean
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        'Dim XYZ_lng As sXYZ_lng
        Dim XY_dbl As sXY_dbl

        If ViewMoveType = enumView_Move_Type.RTS And RTSOrbit Then
            Flag = True
            'If ScreenXY_Get_TerrainPos(CInt(Int(GLSize.X / 2.0#)), CInt(Int(GLSize.Y / 2.0#)), XYZ_lng) Then
            '    XYZ_dbl.X = XYZ_lng.X
            '    XYZ_dbl.Y = XYZ_lng.Y
            '    XYZ_dbl.Z = XYZ_lng.Z
            'Else
            If ScreenXY_Get_ViewPlanePos_ForwardDownOnly(CInt(Int(GLSize.X / 2.0#)), CInt(Int(GLSize.Y / 2.0#)), 127.5#, XY_dbl) Then
                XYZ_dbl.X = XY_dbl.X
                XYZ_dbl.Y = 127.5#
                XYZ_dbl.Z = -XY_dbl.Y
            Else
                Flag = False
            End If
            'End If
        Else
            Flag = False
        End If

        Matrix3D.MatrixToRPY(NewMatrix, ViewAngleRPY)
        If Flag Then
            If ViewAngleRPY.Pitch < RadOf1Deg * 10.0# Then
                ViewAngleRPY.Pitch = RadOf1Deg * 10.0#
            End If
        End If
        Matrix3D.MatrixSetToRPY(ViewAngleMatrix, ViewAngleRPY)
        Matrix3D.MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)

        If Flag Then
            XYZ_dbl2.X = ViewPos.X
            XYZ_dbl2.Y = ViewPos.Y
            XYZ_dbl2.Z = -ViewPos.Z
            MoveToViewTerrainPosFromDistance(XYZ_dbl, GetDist_XYZ_dbl(XYZ_dbl, XYZ_dbl2))
        End If

        DrawViewLater()
    End Sub

    Public Sub View_Rotate(ByVal ChangeMatrix As Matrix3D.Matrix3D)
        Dim matrixA As New Matrix3D.Matrix3D

        Matrix3D.MatrixRotationByMatrix(ViewAngleMatrix, ChangeMatrix, matrixA)
        Matrix3D.MatrixCopy(matrixA, ViewAngleMatrix)
        Matrix3D.MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)

        DrawViewLater()
    End Sub

    Public Sub OpenGL_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        MouseOver = New clsMouseOver
        MouseOver.ScreenPos.X = e.X
        MouseOver.ScreenPos.Y = e.Y

        MouseOver_Pos_Calc()
    End Sub

    Public Sub MouseOver_Pos_Calc()
        Dim XY_dbl As sXY_dbl
        Dim A As Integer
        Dim Flag As Boolean
        Dim Footprint As sXY_int
        Dim MouseLeftDownOverMinimap As clsMouseDown.clsOverMinimap = GetMouseLeftDownOverMinimap()

        If MouseLeftDownOverMinimap IsNot Nothing Then
            If MouseOver Is Nothing Then

            ElseIf IsViewPosOverMinimap(MouseOver.ScreenPos) Then
                Dim Pos As New sXY_int(CInt(Int(MouseOver.ScreenPos.X * Tiles_Per_Minimap_Pixel)), CInt(Int(MouseOver.ScreenPos.Y * Tiles_Per_Minimap_Pixel)))
                Main_Map.TileNumClampToMap(Pos)
                LookAtTile(Pos)
            End If
        Else
            Dim MouseOverTerrain As New clsMouseOver.clsOverTerrain
            Flag = False
            If Settings.DirectPointer Then
                If ScreenXY_Get_TerrainPos(MouseOver.ScreenPos, MouseOverTerrain.Pos) Then
                    If Main_Map.PosIsOnMap(MouseOverTerrain.Pos.Horizontal) Then
                        Flag = True
                    End If
                End If
            Else
                MouseOverTerrain.Pos.Altitude = CInt(255.0# / 2.0# * Main_Map.HeightMultiplier)
                If ScreenXY_Get_ViewPlanePos(MouseOver.ScreenPos, MouseOverTerrain.Pos.Altitude, XY_dbl) Then
                    MouseOverTerrain.Pos.Horizontal.X = CInt(XY_dbl.X)
                    MouseOverTerrain.Pos.Horizontal.Y = CInt(-XY_dbl.Y)
                    If Main_Map.PosIsOnMap(MouseOverTerrain.Pos.Horizontal) Then
                        MouseOverTerrain.Pos.Altitude = CInt(Main_Map.GetTerrainHeight(MouseOverTerrain.Pos.Horizontal))
                        Flag = True
                    End If
                End If
            End If
            If Flag Then
                MouseOver.OverTerrain = MouseOverTerrain
                MouseOverTerrain.Tile.Normal.X = CInt(Int(MouseOverTerrain.Pos.Horizontal.X / TerrainGridSpacing))
                MouseOverTerrain.Tile.Normal.Y = CInt(Int(MouseOverTerrain.Pos.Horizontal.Y / TerrainGridSpacing))
                MouseOverTerrain.Vertex.Normal.X = CInt(Math.Round(MouseOverTerrain.Pos.Horizontal.X / TerrainGridSpacing))
                MouseOverTerrain.Vertex.Normal.Y = CInt(Math.Round(MouseOverTerrain.Pos.Horizontal.Y / TerrainGridSpacing))
                MouseOverTerrain.Tile.Alignment = MouseOverTerrain.Vertex.Normal
                MouseOverTerrain.Vertex.Alignment = New sXY_int(MouseOverTerrain.Tile.Normal.X + 1, MouseOverTerrain.Tile.Normal.Y + 1)
                MouseOverTerrain.Triangle = Main_Map.GetTerrainTri(MouseOverTerrain.Pos.Horizontal)
                XY_dbl.X = MouseOverTerrain.Pos.Horizontal.X - MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing
                XY_dbl.Y = MouseOverTerrain.Pos.Horizontal.Y - MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing
                If Math.Abs(XY_dbl.Y) <= Math.Abs(XY_dbl.X) Then
                    MouseOverTerrain.Side_IsV = False
                    MouseOverTerrain.Side_Num.X = MouseOverTerrain.Tile.Normal.X
                    MouseOverTerrain.Side_Num.Y = MouseOverTerrain.Vertex.Normal.Y
                Else
                    MouseOverTerrain.Side_IsV = True
                    MouseOverTerrain.Side_Num.X = MouseOverTerrain.Vertex.Normal.X
                    MouseOverTerrain.Side_Num.Y = MouseOverTerrain.Tile.Normal.Y
                End If
                Dim SectorNum As sXY_int = Main_Map.GetPosSectorNum(MouseOverTerrain.Pos.Horizontal)
                Dim tmpUnit As clsMap.clsUnit
                For A = 0 To Main_Map.Sectors(SectorNum.X, SectorNum.Y).UnitCount - 1
                    tmpUnit = Main_Map.Sectors(SectorNum.X, SectorNum.Y).Units(A)
                    XY_dbl.X = tmpUnit.Pos.Horizontal.X - MouseOverTerrain.Pos.Horizontal.X
                    XY_dbl.Y = tmpUnit.Pos.Horizontal.Y - MouseOverTerrain.Pos.Horizontal.Y
                    Footprint = tmpUnit.Type.GetFootprint
                    If Math.Abs(XY_dbl.X) <= Math.Max(Footprint.X / 2.0#, 0.5#) * TerrainGridSpacing _
                    And Math.Abs(XY_dbl.Y) <= Math.Max(Footprint.Y / 2.0#, 0.5#) * TerrainGridSpacing Then
                        MouseOverTerrain.Unit_Add(tmpUnit)
                    End If
                Next

                If MouseLeftDown IsNot Nothing Then
                    Select Case Tool
                        Case enumTool.AutoTexture_Place
                            Apply_Terrain()
                            If frmMainInstance.cbxAutoTexSetHeight.Checked Then
                                Apply_Height_Set(TerrainBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                            End If
                        Case enumTool.Height_Set_Brush
                            Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                        Case enumTool.Texture_Brush
                            Apply_Texture()
                        Case enumTool.CliffTriangle
                            Apply_CliffTriangle(False)
                        Case enumTool.AutoCliff
                            Apply_Cliff()
                        Case enumTool.AutoCliffRemove
                            Apply_Cliff_Remove()
                        Case enumTool.AutoRoad_Place
                            Apply_Road()
                        Case enumTool.AutoRoad_Remove
                            Apply_Road_Remove()
                    End Select
                End If
                If MouseRightDown IsNot Nothing Then
                    Select Case Tool
                        Case enumTool.Height_Set_Brush
                            If MouseLeftDown Is Nothing Then
                                Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                            End If
                        Case enumTool.CliffTriangle
                            Apply_CliffTriangle(True)
                    End Select
                End If
            End If
        End If
        Pos_Display_Update()
        DrawViewLater()
    End Sub

    Public Sub Pos_Display_Update()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            lblTile.Text = ""
            lblVertex.Text = ""
            lblPos.Text = ""
        Else
            lblTile.Text = "Tile x:" & MouseOverTerrain.Tile.Normal.X & ", y:" & MouseOverTerrain.Tile.Normal.Y
            lblVertex.Text = "Vertex  x:" & MouseOverTerrain.Vertex.Normal.X & ", y:" & MouseOverTerrain.Vertex.Normal.Y & ", alt:" & Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * Main_Map.HeightMultiplier & " (" & Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height & "x" & Main_Map.HeightMultiplier & ")"
            lblPos.Text = "Pos x:" & MouseOverTerrain.Pos.Horizontal.X & ", y:" & MouseOverTerrain.Pos.Horizontal.Y & ", alt:" & MouseOverTerrain.Pos.Altitude & ", slope: " & Math.Round(Main_Map.GetTerrainSlopeAngle(MouseOverTerrain.Pos.Horizontal) / RadOf1Deg * 10.0#) / 10.0# & "°"
        End If
    End Sub

    Public Sub OpenGL_LostFocus(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        Main_Map.SuppressMinimap = False

        MouseOver = Nothing
        MouseLeftDown = Nothing
        MouseRightDown = Nothing

        ViewKeyDown_Clear()
    End Sub

    Public Sub LookAtTile(ByVal TileNum As sXY_int)
        Dim Pos As sXY_int

        Pos.X = CInt((TileNum.X + 0.5#) * TerrainGridSpacing)
        Pos.Y = CInt((TileNum.Y + 0.5#) * TerrainGridSpacing)
        LookAtPos(Pos)
    End Sub

    Public Sub LookAtPos(ByVal Horizontal As sXY_int)
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_int As sXYZ_int
        Dim dblTemp As Double
        Dim A As Integer
        Dim matrixA As New Matrix3D.Matrix3D
        Dim AnglePY As Matrix3D.AnglePY

        Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, XYZ_dbl)
        dblTemp = Main_Map.GetTerrainHeight(Horizontal)
        A = CInt(Math.Ceiling(dblTemp)) + 128
        If ViewPos.Y < A Then
            ViewPos.Y = A
        End If
        If XYZ_dbl.Y > -0.33333333333333331# Then
            XYZ_dbl.Y = -0.33333333333333331#
            Matrix3D.VectorToPY(XYZ_dbl, AnglePY)
            Matrix3D.MatrixSetToPY(matrixA, AnglePY)
            ViewAngleSet(matrixA)
        End If
        dblTemp = (ViewPos.Y - dblTemp) / XYZ_dbl.Y

        XYZ_int.X = CInt(Horizontal.X + dblTemp * XYZ_dbl.X)
        XYZ_int.Y = ViewPos.Y
        XYZ_int.Z = CInt(-Horizontal.Y + dblTemp * XYZ_dbl.Z)

        ViewPosSet(XYZ_int)
    End Sub

    Public Sub MoveToViewTerrainPosFromDistance(ByVal TerrainPos As Matrix3D.XYZ_dbl, ByVal Distance As Double)
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_int As sXYZ_int

        Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, XYZ_dbl)

        XYZ_int.X = CInt(TerrainPos.X - XYZ_dbl.X * Distance)
        XYZ_int.Y = CInt(TerrainPos.Y - XYZ_dbl.Y * Distance)
        XYZ_int.Z = CInt(-TerrainPos.Z - XYZ_dbl.Z * Distance)

        ViewPosSet(XYZ_int)
    End Sub

    Public Function GetMouseOverTerrain() As clsMouseOver.clsOverTerrain

        If MouseOver Is Nothing Then
            Return Nothing
        Else
            Return MouseOver.OverTerrain
        End If
    End Function

    Public Function GetMouseLeftDownOverTerrain() As clsMouseDown.clsOverTerrain

        If MouseLeftDown Is Nothing Then
            Return Nothing
        Else
            Return MouseLeftDown.OverTerrain
        End If
    End Function

    Public Function GetMouseRightDownOverTerrain() As clsMouseDown.clsOverTerrain

        If MouseRightDown Is Nothing Then
            Return Nothing
        Else
            Return MouseRightDown.OverTerrain
        End If
    End Function

    Public Function GetMouseLeftDownOverMinimap() As clsMouseDown.clsOverMinimap

        If MouseLeftDown Is Nothing Then
            Return Nothing
        Else
            Return MouseLeftDown.OverMinimap
        End If
    End Function

    Public Function GetMouseRightDownOverMinimap() As clsMouseDown.clsOverMinimap

        If MouseRightDown Is Nothing Then
            Return Nothing
        Else
            Return MouseRightDown.OverMinimap
        End If
    End Function

    Public Sub Apply_Terrain()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyVertexTerrain As New clsMap.clsAction.clsApplyVertexTerrain
        ApplyVertexTerrain.Map = Main_Map
        ApplyVertexTerrain.VertexTerrain = SelectedTerrain
        TerrainBrush.PerformActionMapVertices(ApplyVertexTerrain, MouseOverTerrain.Vertex)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Road()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Side_Num As sXY_int = MouseOverTerrain.Side_Num
        Dim TileNum As sXY_int

        If MouseOverTerrain.Side_IsV Then
            If Main_Map.Terrain.SideV(Side_Num.X, Side_Num.Y).Road IsNot SelectedRoad Then

                Main_Map.Terrain.SideV(Side_Num.X, Side_Num.Y).Road = SelectedRoad

                If Side_Num.X > 0 Then
                    TileNum.X = Side_Num.X - 1
                    TileNum.Y = Side_Num.Y
                    Main_Map.AutoTextureChanges.TileChanged(TileNum)
                    Main_Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Main_Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If
                If Side_Num.X < Main_Map.Terrain.TileSize.X Then
                    TileNum = Side_Num
                    Main_Map.AutoTextureChanges.TileChanged(TileNum)
                    Main_Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Main_Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If

                Main_Map.Update()

                Main_Map.UndoStepCreate("Road Side")

                DrawViewLater()
            End If
        Else
            If Main_Map.Terrain.SideH(Side_Num.X, Side_Num.Y).Road IsNot SelectedRoad Then

                Main_Map.Terrain.SideH(Side_Num.X, Side_Num.Y).Road = SelectedRoad

                If Side_Num.Y > 0 Then
                    TileNum.X = Side_Num.X
                    TileNum.Y = Side_Num.Y - 1
                    Main_Map.AutoTextureChanges.TileChanged(TileNum)
                    Main_Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Main_Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If
                If Side_Num.Y < Main_Map.Terrain.TileSize.X Then
                    TileNum = Side_Num
                    Main_Map.AutoTextureChanges.TileChanged(TileNum)
                    Main_Map.SectorGraphicsChanges.TileChanged(TileNum)
                    Main_Map.SectorTerrainUndoChanges.TileChanged(TileNum)
                End If

                Main_Map.Update()

                Main_Map.UndoStepCreate("Road Side")

                DrawViewLater()
            End If
        End If
    End Sub

    Public Sub Apply_Terrain_Fill()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim FillType As clsPainter.clsTerrain
        Dim ReplaceType As clsPainter.clsTerrain
        Dim StartVertex As sXY_int = MouseOverTerrain.Vertex.Normal

        FillType = SelectedTerrain
        ReplaceType = Main_Map.Terrain.Vertices(StartVertex.X, StartVertex.Y).Terrain
        If FillType Is ReplaceType Then
            Exit Sub 'otherwise will cause endless loop
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim SourceOfFill(524288) As sXY_int
        Dim SourceOfFillCount As Integer
        Dim SourceOfFill_Num As Integer
        Dim VertexNum As sXY_int
        Dim MoveCount As Integer
        Dim RemainingCount As Integer
        Dim MoveOffset As Integer

        SourceOfFill(0).X = StartVertex.X
        SourceOfFill(0).Y = StartVertex.Y
        SourceOfFillCount = 1
        SourceOfFill_Num = 0

        X = SourceOfFill(SourceOfFill_Num).X
        Y = SourceOfFill(SourceOfFill_Num).Y
        Main_Map.Terrain.Vertices(X, Y).Terrain = FillType
        VertexNum.X = X
        VertexNum.Y = Y
        Main_Map.SectorGraphicsChanges.VertexChanged(VertexNum)
        Main_Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
        Main_Map.AutoTextureChanges.VertexChanged(VertexNum)
        Do While SourceOfFill_Num < SourceOfFillCount
            X = SourceOfFill(SourceOfFill_Num).X + 1
            Y = SourceOfFill(SourceOfFill_Num).Y
            If X >= 0 And X <= Main_Map.Terrain.TileSize.X _
            And Y >= 0 And Y <= Main_Map.Terrain.TileSize.Y Then
                If Main_Map.Terrain.Vertices(X, Y).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Y
                    SourceOfFillCount += 1

                    Main_Map.Terrain.Vertices(X, Y).Terrain = FillType

                    VertexNum.X = X
                    VertexNum.Y = Y
                    Main_Map.SectorGraphicsChanges.VertexChanged(VertexNum)
                    Main_Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
                    Main_Map.AutoTextureChanges.VertexChanged(VertexNum)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X - 1
            Y = SourceOfFill(SourceOfFill_Num).Y
            If X >= 0 And X <= Main_Map.Terrain.TileSize.X _
            And Y >= 0 And Y <= Main_Map.Terrain.TileSize.Y Then
                If Main_Map.Terrain.Vertices(X, Y).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Y
                    SourceOfFillCount += 1

                    Main_Map.Terrain.Vertices(X, Y).Terrain = FillType

                    VertexNum.X = X
                    VertexNum.Y = Y
                    Main_Map.SectorGraphicsChanges.VertexChanged(VertexNum)
                    Main_Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
                    Main_Map.AutoTextureChanges.VertexChanged(VertexNum)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X
            Y = SourceOfFill(SourceOfFill_Num).Y + 1
            If X >= 0 And X <= Main_Map.Terrain.TileSize.X _
            And Y >= 0 And Y <= Main_Map.Terrain.TileSize.Y Then
                If Main_Map.Terrain.Vertices(X, Y).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Y
                    SourceOfFillCount += 1

                    Main_Map.Terrain.Vertices(X, Y).Terrain = FillType

                    VertexNum.X = X
                    VertexNum.Y = Y
                    Main_Map.SectorGraphicsChanges.VertexChanged(VertexNum)
                    Main_Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
                    Main_Map.AutoTextureChanges.VertexChanged(VertexNum)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X
            Y = SourceOfFill(SourceOfFill_Num).Y - 1
            If X >= 0 And X <= Main_Map.Terrain.TileSize.X _
            And Y >= 0 And Y <= Main_Map.Terrain.TileSize.Y Then
                If Main_Map.Terrain.Vertices(X, Y).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Y
                    SourceOfFillCount += 1

                    Main_Map.Terrain.Vertices(X, Y).Terrain = FillType

                    VertexNum.X = X
                    VertexNum.Y = Y
                    Main_Map.SectorGraphicsChanges.VertexChanged(VertexNum)
                    Main_Map.SectorTerrainUndoChanges.VertexChanged(VertexNum)
                    Main_Map.AutoTextureChanges.VertexChanged(VertexNum)
                End If
            End If

            SourceOfFill_Num += 1

            If SourceOfFill_Num >= 131072 Then
                RemainingCount = SourceOfFillCount - SourceOfFill_Num
                MoveCount = Math.Min(SourceOfFill_Num, RemainingCount)
                MoveOffset = SourceOfFillCount - MoveCount
                For A = 0 To MoveCount - 1
                    SourceOfFill(A) = SourceOfFill(MoveOffset + A)
                Next
                SourceOfFillCount -= SourceOfFill_Num
                SourceOfFill_Num = 0
                If SourceOfFillCount * 3 < SourceOfFill.GetUpperBound(0) + 1 Then
                    ReDim Preserve SourceOfFill(SourceOfFillCount * 2 + 1)
                End If
            End If
        Loop

        Main_Map.Update()

        Main_Map.UndoStepCreate("Ground Fill")

        DrawViewLater()
    End Sub

    Public Sub Apply_Texture()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyTexture As New clsMap.clsAction.clsApplyTexture
        ApplyTexture.Map = Main_Map
        ApplyTexture.TextureNum = SelectedTextureNum
        ApplyTexture.SetTexture = frmMainInstance.chkSetTexture.Checked
        ApplyTexture.Orientation = TextureOrientation
        ApplyTexture.RandomOrientation = frmMainInstance.chkTextureOrientationRandomize.Checked
        ApplyTexture.SetOrientation = frmMainInstance.chkSetTextureOrientation.Checked
        ApplyTexture.TerrainAction = frmMainInstance.TextureTerrainAction
        TextureBrush.PerformActionMapTiles(ApplyTexture, MouseOverTerrain.Tile)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_CliffTriangle(ByVal Remove As Boolean)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        If Remove Then
            Dim ApplyCliffTriangleRemove As New clsMap.clsAction.clsApplyCliffTriangleRemove
            ApplyCliffTriangleRemove.Map = Main_Map
            ApplyCliffTriangleRemove.PosNum = MouseOverTerrain.Tile.Normal
            ApplyCliffTriangleRemove.Triangle = MouseOverTerrain.Triangle
            ApplyCliffTriangleRemove.ActionPerform()
        Else
            Dim ApplyCliffTriangle As New clsMap.clsAction.clsApplyCliffTriangle
            ApplyCliffTriangle.Map = Main_Map
            ApplyCliffTriangle.PosNum = MouseOverTerrain.Tile.Normal
            ApplyCliffTriangle.Triangle = MouseOverTerrain.Triangle
            ApplyCliffTriangle.ActionPerform()
        End If

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Cliff()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyCliff As New clsMap.clsAction.clsApplyCliff
        ApplyCliff.Map = Main_Map
        ApplyCliff.Angle = Clamp_dbl(Val(frmMainInstance.txtAutoCliffSlope.Text) * RadOf1Deg, 0.0#, RadOf90Deg)
        ApplyCliff.SetTris = frmMainInstance.cbxCliffTris.Checked
        CliffBrush.PerformActionMapTiles(ApplyCliff, MouseOverTerrain.Tile)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Cliff_Remove()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyCliffRemove As New clsMap.clsAction.clsApplyCliffRemove
        ApplyCliffRemove.Map = Main_Map
        CliffBrush.PerformActionMapTiles(ApplyCliffRemove, MouseOverTerrain.Tile)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Road_Remove()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyRoadRemove As New clsMap.clsAction.clsApplyRoadRemove
        ApplyRoadRemove.Map = Main_Map
        HeightBrush.PerformActionMapTiles(ApplyRoadRemove, MouseOverTerrain.Tile)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Texture_Clockwise()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.RotateClockwise()
        Main_Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Main_Map.SectorGraphicsChanges.TileChanged(Tile)
        Main_Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Main_Map.Update()

        Main_Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Public Sub Apply_Texture_CounterClockwise()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.RotateAnticlockwise()
        Main_Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Main_Map.SectorGraphicsChanges.TileChanged(Tile)
        Main_Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Main_Map.Update()

        Main_Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Public Sub Apply_Texture_FlipX()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.ResultXFlip = Not Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation.ResultXFlip
        Main_Map.TileTextureChangeTerrainAction(Tile, frmMainInstance.TextureTerrainAction)

        Main_Map.SectorGraphicsChanges.TileChanged(Tile)
        Main_Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Main_Map.Update()

        Main_Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Public Sub Apply_Tri_Flip()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Tri = Not Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Tri

        Main_Map.SectorGraphicsChanges.TileChanged(Tile)
        Main_Map.SectorTerrainUndoChanges.TileChanged(Tile)

        Main_Map.Update()

        Main_Map.UndoStepCreate("Triangle Flip")

        DrawViewLater()
    End Sub

    Public Sub Apply_HeightSmoothing(ByVal Ratio As Double)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightSmoothing As New clsMap.clsAction.clsApplyHeightSmoothing
        ApplyHeightSmoothing.Map = Main_Map
        ApplyHeightSmoothing.Ratio = Ratio
        Dim Radius As Integer = CInt(Math.Ceiling(HeightBrush.Radius))
        Dim PosNum As sXY_int = HeightBrush.GetPosNum(MouseOverTerrain.Vertex)
        ApplyHeightSmoothing.Offset.X = Clamp_int(PosNum.X - Radius, 0, Main_Map.Terrain.TileSize.X)
        ApplyHeightSmoothing.Offset.Y = Clamp_int(PosNum.Y - Radius, 0, Main_Map.Terrain.TileSize.Y)
        Dim PosEnd As sXY_int
        PosEnd.X = Clamp_int(PosNum.X + Radius, 0, Main_Map.Terrain.TileSize.X)
        PosEnd.Y = Clamp_int(PosNum.Y + Radius, 0, Main_Map.Terrain.TileSize.Y)
        ApplyHeightSmoothing.AreaTileSize.X = PosEnd.X - ApplyHeightSmoothing.Offset.X
        ApplyHeightSmoothing.AreaTileSize.Y = PosEnd.Y - ApplyHeightSmoothing.Offset.Y
        ApplyHeightSmoothing.Start()
        HeightBrush.PerformActionMapVertices(ApplyHeightSmoothing, MouseOverTerrain.Vertex)
        ApplyHeightSmoothing.Finish()

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Height_Change(ByVal Rate As Double)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightChange As New clsMap.clsAction.clsApplyHeightChange
        ApplyHeightChange.Map = Main_Map
        ApplyHeightChange.Rate = Rate
        ApplyHeightChange.UseEffect = frmMainInstance.cbxHeightChangeFade.Checked
        HeightBrush.PerformActionMapVertices(ApplyHeightChange, MouseOverTerrain.Vertex)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Public Sub Apply_Height_Set(ByVal Brush As clsBrush, ByVal Height As Byte)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim ApplyHeightSet As New clsMap.clsAction.clsApplyHeightSet
        ApplyHeightSet.Map = Main_Map
        ApplyHeightSet.Height = Height
        Brush.PerformActionMapVertices(ApplyHeightSet, MouseOverTerrain.Vertex)

        Main_Map.Update()

        DrawViewLater()
    End Sub

    Private WithEvents ListSelect As ContextMenuStrip
    Private ListSelectIsPicker As Boolean
    Private ListSelectItems(-1) As ToolStripItem

    Private Sub ListSelect_Click(ByVal Sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles ListSelect.ItemClicked
        Dim tmpButton As ToolStripItem = e.ClickedItem
        Dim tmpUnit As clsMap.clsUnit = CType(tmpButton.Tag, clsMap.clsUnit)

        If ListSelectIsPicker Then
            ObjectPicker(tmpUnit.Type)
        Else
            If tmpUnit.Map_SelectedUnitNum < 0 Then
                Main_Map.SelectedUnit_Add(tmpUnit)
            Else
                Main_Map.SelectedUnit_Remove(tmpUnit.Map_SelectedUnitNum)
            End If
            frmMainInstance.SelectedObject_Changed()
            DrawViewLater()
        End If
    End Sub

    Private Sub ListSelect_Close(ByVal sender As Object, ByVal e As ToolStripDropDownClosedEventArgs) Handles ListSelect.Closed
        Dim A As Integer

        For A = 0 To ListSelectItems.GetUpperBound(0)
            ListSelectItems(A).Tag = Nothing
            ListSelectItems(A).Dispose()
        Next
        ListSelect.Items.Clear()
        ReDim ListSelectItems(-1)

        ViewKeyDown_Clear()
    End Sub

    Private Sub OpenGL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim ScreenPos As sXY_int
        Dim NewUnitType As clsUnitType
        Dim NewUnit As clsMap.clsUnit
        Dim A As Integer

        Main_Map.SuppressMinimap = True

        ScreenPos.X = e.X
        ScreenPos.Y = e.Y
        If e.Button = Windows.Forms.MouseButtons.Left Then
            MouseLeftDown = New clsMouseDown
            If IsViewPosOverMinimap(ScreenPos) Then
                MouseLeftDown.OverMinimap = New clsMouseDown.clsOverMinimap
                MouseLeftDown.OverMinimap.DownPos = ScreenPos
                Dim Pos As New sXY_int(CInt(Int(ScreenPos.X * Tiles_Per_Minimap_Pixel)), CInt(Int(ScreenPos.Y * Tiles_Per_Minimap_Pixel)))
                Main_Map.TileNumClampToMap(Pos)
                LookAtTile(Pos)
            Else
                Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()
                If MouseOverTerrain IsNot Nothing Then
                    MouseLeftDown.OverTerrain = New clsMouseDown.clsOverTerrain
                    MouseLeftDown.OverTerrain.DownPos = MouseOverTerrain.Pos
                    Select Case Tool
                        Case enumTool.None
                            If Control_Picker.Active Then
                                If MouseOverTerrain.UnitCount > 0 Then
                                    If MouseOverTerrain.UnitCount = 1 Then
                                        ObjectPicker(MouseOverTerrain.Units(0).Type)
                                    Else
                                        ListSelect.Close()
                                        ListSelect.Items.Clear()
                                        ReDim ListSelectItems(MouseOverTerrain.UnitCount - 1)
                                        For A = 0 To MouseOverTerrain.UnitCount - 1
                                            ListSelectItems(A) = New ToolStripButton(MouseOverTerrain.Units(A).Type.GetDisplayText)
                                            ListSelectItems(A).Tag = MouseOverTerrain.Units(A)
                                            ListSelect.Items.Add(ListSelectItems(A))
                                        Next
                                        ListSelectIsPicker = True
                                        ListSelect.Show(Me, New Drawing.Point(MouseOver.ScreenPos.X, MouseOver.ScreenPos.Y))
                                    End If
                                End If
                            Else
                                If Not Control_Unit_Multiselect.Active Then
                                    Main_Map.SelectedUnits_Clear()
                                End If
                                frmMainInstance.SelectedObject_Changed()
                                Main_Map.Unit_Selected_Area_VertexA = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                                DrawViewLater()
                            End If
                        Case enumTool.AutoTexture_Place
                            If Main_Map.Tileset IsNot Nothing Then
                                If Control_Picker.Active Then
                                    TerrainPicker()
                                Else
                                    Apply_Terrain()
                                    If frmMainInstance.cbxAutoTexSetHeight.Checked Then
                                        Apply_Height_Set(TerrainBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                                    End If
                                End If
                            End If
                        Case enumTool.Height_Set_Brush
                            If Control_Picker.Active Then
                                HeightPickerL()
                            Else
                                Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                            End If
                        Case enumTool.Texture_Brush
                            If Main_Map.Tileset IsNot Nothing Then
                                If Control_Picker.Active Then
                                    TexturePicker()
                                Else
                                    Apply_Texture()
                                End If
                            End If
                        Case enumTool.CliffTriangle
                            Apply_CliffTriangle(False)
                        Case enumTool.AutoCliff
                            Apply_Cliff()
                        Case enumTool.AutoCliffRemove
                            Apply_Cliff_Remove()
                        Case enumTool.AutoTexture_Fill
                            If Main_Map.Tileset IsNot Nothing Then
                                If Control_Picker.Active Then
                                    TerrainPicker()
                                Else
                                    Apply_Terrain_Fill()
                                    DrawViewLater()
                                End If
                            End If
                        Case enumTool.AutoRoad_Place
                            If Main_Map.Tileset IsNot Nothing Then
                                Apply_Road()
                            End If
                        Case enumTool.AutoRoad_Line
                            If Main_Map.Tileset IsNot Nothing Then
                                Apply_Road_Line_Selection()
                            End If
                        Case enumTool.AutoRoad_Remove
                            Apply_Road_Remove()
                        Case enumTool.ObjectPlace
                            If frmMainInstance.SelectedObjectType IsNot Nothing And frmMainInstance.NewPlayerNum.SelectedUnitGroup IsNot Nothing Then
                                NewUnitType = frmMainInstance.SelectedObjectType
                                NewUnit = New clsMap.clsUnit
                                NewUnit.UnitGroup = frmMainInstance.NewPlayerNum.SelectedUnitGroup
                                NewUnit.Pos = Main_Map.TileAligned_Pos_From_MapPos(MouseOverTerrain.Pos.Horizontal, NewUnitType.GetFootprint)
                                If frmMainInstance.cbxObjectRandomRotation.Checked Then
                                    NewUnit.Rotation = CInt(Int(Rnd() * 360.0#))
                                Else
                                    NewUnit.Rotation = 0
                                End If
                                NewUnit.Type = NewUnitType
                                Main_Map.Unit_Add_StoreChange(NewUnit)
                                Main_Map.UndoStepCreate("Place Object")
                                Main_Map.Update()
                                Main_Map.MinimapMakeLater()
                                DrawViewLater()
                            End If
                        Case enumTool.Terrain_Select
                            If Main_Map.Selected_Area_VertexA Is Nothing Then
                                Main_Map.Selected_Area_VertexA = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                                DrawViewLater()
                            ElseIf Main_Map.Selected_Area_VertexB Is Nothing Then
                                Main_Map.Selected_Area_VertexB = New clsXY_int(MouseOverTerrain.Vertex.Normal)
                                DrawViewLater()
                                frmMainInstance.Terrain_Selection_Changed()
                            Else
                                Main_Map.Selected_Area_VertexA = Nothing
                                Main_Map.Selected_Area_VertexB = Nothing
                                frmMainInstance.Terrain_Selection_Changed()
                                DrawViewLater()
                            End If
                        Case enumTool.Gateways
                            Apply_Gateway()
                    End Select
                ElseIf Tool = enumTool.None Then
                    Main_Map.SelectedUnits_Clear()
                    frmMainInstance.SelectedObject_Changed()
                End If
            End If
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            MouseRightDown = New clsMouseDown
            If IsViewPosOverMinimap(ScreenPos) Then
                MouseRightDown.OverMinimap = New clsMouseDown.clsOverMinimap
                MouseRightDown.OverMinimap.DownPos = ScreenPos
            Else
                Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()
                If MouseOverTerrain IsNot Nothing Then
                    MouseRightDown.OverTerrain = New clsMouseDown.clsOverTerrain
                    MouseRightDown.OverTerrain.DownPos = MouseOverTerrain.Pos
                End If
            End If
            Select Case Tool
                Case enumTool.AutoRoad_Line
                    Main_Map.Selected_Tile_A = Nothing
                    DrawViewLater()
                Case enumTool.Terrain_Select
                    Main_Map.Selected_Area_VertexA = Nothing
                    Main_Map.Selected_Area_VertexB = Nothing
                    frmMainInstance.Terrain_Selection_Changed()
                    DrawViewLater()
                Case enumTool.CliffTriangle
                    Apply_CliffTriangle(True)
                Case enumTool.Gateways
                    Main_Map.Selected_Tile_A = Nothing
                    Main_Map.Selected_Tile_B = Nothing
                    DrawViewLater()
                Case enumTool.Height_Set_Brush
                    If Control_Picker.Active Then
                        HeightPickerR()
                    Else
                        Apply_Height_Set(HeightBrush, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                    End If
            End Select
        End If
    End Sub

    Private Sub OpenGL_KeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
        Dim matrixA As New Matrix3D.Matrix3D
        Dim A As Integer
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        IsViewKeyDown.Keys(e.KeyCode) = True

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next

        If e.KeyCode = Keys.F12 Then InputBox("", "", My.Application.Info.DirectoryPath)
        If Control_Undo.Active Then
            If Main_Map.Undo_Pos > 0 Then
                DisplayUndoMessage("Undid: " & Main_Map.Undos(Main_Map.Undo_Pos - 1).Name)
                Main_Map.Undo_Perform()
                DrawViewLater()
            Else
                DisplayUndoMessage("Nothing to undo")
            End If
        End If
        If Control_Redo.Active Then
            If Main_Map.Undo_Pos < Main_Map.UndoCount Then
                DisplayUndoMessage("Redid: " & Main_Map.Undos(Main_Map.Undo_Pos).Name)
                Main_Map.Redo_Perform()
                DrawViewLater()
            Else
                DisplayUndoMessage("Nothing to redo")
            End If
        End If
        If IsViewKeyDown.Keys(Keys.ControlKey) Then
            If e.KeyCode = Keys.D1 Then
                VisionRadius_2E = 7
            ElseIf e.KeyCode = Keys.D2 Then
                VisionRadius_2E = 8
            ElseIf e.KeyCode = Keys.D3 Then
                VisionRadius_2E = 9
            ElseIf e.KeyCode = Keys.D4 Then
                VisionRadius_2E = 10
            ElseIf e.KeyCode = Keys.D5 Then
                VisionRadius_2E = 11
            ElseIf e.KeyCode = Keys.D6 Then
                VisionRadius_2E = 12
            ElseIf e.KeyCode = Keys.D7 Then
                VisionRadius_2E = 13
            ElseIf e.KeyCode = Keys.D8 Then
                VisionRadius_2E = 14
            ElseIf e.KeyCode = Keys.D9 Then
                VisionRadius_2E = 15
            ElseIf e.KeyCode = Keys.D0 Then
                VisionRadius_2E = 16
            End If
            VisionRadius_2E_Changed()
        End If

        If Control_View_Move_Type.Active Then
            If ViewMoveType = ctrlMapView.enumView_Move_Type.Free Then
                ViewMoveType = ctrlMapView.enumView_Move_Type.RTS
            ElseIf ViewMoveType = ctrlMapView.enumView_Move_Type.RTS Then
                ViewMoveType = ctrlMapView.enumView_Move_Type.Free
            End If
        End If
        If Control_View_Rotate_Type.Active Then
            RTSOrbit = Not RTSOrbit
        End If
        If Control_View_Reset.Active Then
            FOV_Multiplier_Set(Settings.FOVDefault)
            If ViewMoveType = ctrlMapView.enumView_Move_Type.Free Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                ViewAngleSet_Rotate(matrixA)
            ElseIf ViewMoveType = ctrlMapView.enumView_Move_Type.RTS Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                ViewAngleSet_Rotate(matrixA)
            End If
        End If
        If Control_View_Textures.Active Then
            Draw_TileTextures = Not Draw_TileTextures
            DrawViewLater()
        End If
        If Control_View_Wireframe.Active Then
            Draw_TileWireframe = Not Draw_TileWireframe
            DrawViewLater()
        End If
        If Control_View_Units.Active Then
            Draw_Units = Not Draw_Units
            Main_Map.SectorGraphicsChanges.SetAllChanged()
            Main_Map.Update()
            DrawViewLater()
        End If
        If Control_View_Lighting.Active Then
            If Draw_Lighting = enumDrawLighting.Off Then
                Draw_Lighting = enumDrawLighting.Half
            ElseIf Draw_Lighting = enumDrawLighting.Half Then
                Draw_Lighting = enumDrawLighting.Normal
            ElseIf Draw_Lighting = enumDrawLighting.Normal Then
                Draw_Lighting = enumDrawLighting.Off
            End If
            DrawViewLater()
        End If
        If Tool = enumTool.Texture_Brush Then
            If MouseOverTerrain IsNot Nothing Then
                If Control_Clockwise.Active Then
                    Apply_Texture_Clockwise()
                End If
                If Control_Anticlockwise.Active Then
                    Apply_Texture_CounterClockwise()
                End If
                If Control_Texture_Flip.Active Then
                    Apply_Texture_FlipX()
                End If
                If Control_Tri_Flip.Active Then
                    Apply_Tri_Flip()
                End If
            End If
        End If
        If Tool = enumTool.None Then
            If Control_Unit_Delete.Active Then
                If Main_Map.SelectedUnitCount > 0 Then
                    Dim OldUnits As New clsMap.sUnitArray
                    Main_Map.SelectedUnits_Copy(OldUnits)
                    For A = 0 To OldUnits.Units.GetUpperBound(0)
                        Main_Map.Unit_Remove_StoreChange(OldUnits.Units(A).Map_UnitNum)
                    Next
                    Main_Map.SelectedUnits_Clear()
                    frmMainInstance.SelectedObject_Changed()
                    Main_Map.UndoStepCreate("Object Deleted")
                    Main_Map.Update()
                    Main_Map.MinimapMakeLater()
                    DrawViewLater()
                End If
            End If
            If Control_Unit_Move.Active Then
                If MouseOverTerrain IsNot Nothing Then
                    If Main_Map.SelectedUnitCount > 0 Then
                        Dim Centre As sXY_dbl
                        For A = 0 To Main_Map.SelectedUnitCount - 1
                            Centre.X += Main_Map.SelectedUnits(A).Pos.Horizontal.X
                            Centre.Y += Main_Map.SelectedUnits(A).Pos.Horizontal.Y
                        Next
                        Centre.X /= Main_Map.SelectedUnitCount
                        Centre.Y /= Main_Map.SelectedUnitCount
                        Dim UnitsToMove(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
                        Dim UnitsToMoveID(Main_Map.SelectedUnitCount - 1) As UInteger
                        For A = 0 To Main_Map.SelectedUnitCount - 1
                            UnitsToMove(A) = Main_Map.SelectedUnits(A)
                            UnitsToMoveID(A) = UnitsToMove(A).ID
                        Next
                        Dim NewUnits(Main_Map.SelectedUnitCount - 1) As clsMap.clsUnit
                        Dim tmpUnit As clsMap.clsUnit
                        Dim NewPos As sXY_int
                        For A = 0 To Main_Map.SelectedUnitCount - 1
                            tmpUnit = New clsMap.clsUnit(UnitsToMove(A))
                            NewUnits(A) = tmpUnit
                            NewPos.X = MouseOverTerrain.Pos.Horizontal.X + UnitsToMove(A).Pos.Horizontal.X - CInt(Centre.X)
                            NewPos.Y = MouseOverTerrain.Pos.Horizontal.Y + UnitsToMove(A).Pos.Horizontal.Y - CInt(Centre.Y)
                            tmpUnit.Pos = Main_Map.TileAligned_Pos_From_MapPos(NewPos, tmpUnit.Type.GetFootprint)
                            Main_Map.Unit_Remove_StoreChange(UnitsToMove(A).Map_UnitNum)
                            Main_Map.UnitID_Add_StoreChange(tmpUnit, UnitsToMoveID(A))
                            ErrorIDChange(UnitsToMoveID(A), tmpUnit, "OpenGL_KeyDown->UnitMove")
                        Next
                        Main_Map.SelectedUnits_Add(NewUnits)
                        Main_Map.UndoStepCreate("Objects Moved")
                        Main_Map.Update()
                        Main_Map.MinimapMakeLater()
                        frmMainInstance.SelectedObject_Changed()
                        DrawViewLater()
                    End If
                End If
            End If
            If Control_Clockwise.Active Then
                If Main_Map.SelectedUnitCount = 1 Then
                    Dim NewUnit As New clsMap.clsUnit(Main_Map.SelectedUnits(0))
                    Dim ID As UInteger = Main_Map.SelectedUnits(0).ID
                    Main_Map.Unit_Remove_StoreChange(Main_Map.SelectedUnits(0).Map_UnitNum)
                    NewUnit.Rotation -= 90
                    If NewUnit.Rotation < 0 Then
                        NewUnit.Rotation += 360
                    End If
                    Main_Map.UnitID_Add_StoreChange(NewUnit, ID)
                    ErrorIDChange(ID, NewUnit, "OpenGL_KeyDown->UnitClockwise")
                    Main_Map.SelectedUnit_Add(NewUnit)
                    Main_Map.Update()
                    frmMainInstance.SelectedObject_Changed()
                    Main_Map.UndoStepCreate("Object Rotated")
                    DrawViewLater()
                End If
            End If
            If Control_Anticlockwise.Active Then
                If Main_Map.SelectedUnitCount = 1 Then
                    Dim NewUnit As New clsMap.clsUnit(Main_Map.SelectedUnits(0))
                    Dim ID As UInteger = Main_Map.SelectedUnits(0).ID
                    Main_Map.Unit_Remove_StoreChange(Main_Map.SelectedUnits(0).Map_UnitNum)
                    NewUnit.Rotation += 90
                    If NewUnit.Rotation > 359 Then
                        NewUnit.Rotation -= 360
                    End If
                    Main_Map.UnitID_Add_StoreChange(NewUnit, ID)
                    ErrorIDChange(ID, NewUnit, "OpenGL_KeyDown->UnitCounterClockwise")
                    Main_Map.Update()
                    Main_Map.SelectedUnit_Add(NewUnit)
                    frmMainInstance.SelectedObject_Changed()
                    Main_Map.UndoStepCreate("Object Rotated")
                    DrawViewLater()
                End If
            End If
        End If

        If Control_Deselect.Active Then
            Tool = enumTool.None
            DrawViewLater()
        End If
    End Sub

    Private Sub OpenGL_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        Dim A As Integer

        IsViewKeyDown.Keys(e.KeyCode) = False

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Public Sub Apply_Road_Line_Selection()
        Dim MouseOverTerrian As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrian Is Nothing Then
            Exit Sub
        End If

        Dim Num As Integer
        Dim A As Integer
        Dim B As Integer
        Dim Tile As sXY_int = MouseOverTerrian.Tile.Normal
        Dim SideNum As sXY_int

        If Main_Map.Selected_Tile_A IsNot Nothing Then
            If Tile.X = Main_Map.Selected_Tile_A.X Then
                If Tile.Y <= Main_Map.Selected_Tile_A.Y Then
                    A = Tile.Y
                    B = Main_Map.Selected_Tile_A.Y
                Else
                    A = Main_Map.Selected_Tile_A.Y
                    B = Tile.Y
                End If
                For Num = A + 1 To B
                    Main_Map.Terrain.SideH(Main_Map.Selected_Tile_A.X, Num).Road = SelectedRoad
                    SideNum.X = Main_Map.Selected_Tile_A.X
                    SideNum.Y = Num
                    Main_Map.AutoTextureChanges.SideHChanged(SideNum)
                    Main_Map.SectorGraphicsChanges.SideHChanged(SideNum)
                    Main_Map.SectorTerrainUndoChanges.SideHChanged(SideNum)
                Next

                Main_Map.Update()

                Main_Map.UndoStepCreate("Road Line")

                Main_Map.Selected_Tile_A = Nothing
                DrawViewLater()
            ElseIf Tile.Y = Main_Map.Selected_Tile_A.Y Then
                If Tile.X <= Main_Map.Selected_Tile_A.X Then
                    A = Tile.X
                    B = Main_Map.Selected_Tile_A.X
                Else
                    A = Main_Map.Selected_Tile_A.X
                    B = Tile.X
                End If
                For Num = A + 1 To B
                    Main_Map.Terrain.SideV(Num, Main_Map.Selected_Tile_A.Y).Road = SelectedRoad
                    SideNum.X = Num
                    SideNum.Y = Main_Map.Selected_Tile_A.Y
                    Main_Map.AutoTextureChanges.SideVChanged(SideNum)
                    Main_Map.SectorGraphicsChanges.SideVChanged(SideNum)
                    Main_Map.SectorTerrainUndoChanges.SideVChanged(SideNum)
                Next

                Main_Map.Update()

                Main_Map.UndoStepCreate("Road Line")

                Main_Map.Selected_Tile_A = Nothing
                DrawViewLater()
            Else

            End If
        Else
            Main_Map.Selected_Tile_A = New clsXY_int(Tile)
        End If
    End Sub

    Public Function IsViewPosOverMinimap(ByVal Pos As sXY_int) As Boolean

        If Pos.X >= 0 And Pos.X < Main_Map.Terrain.TileSize.X / Tiles_Per_Minimap_Pixel _
            And Pos.Y >= 0 And Pos.Y < Main_Map.Terrain.TileSize.Y / Tiles_Per_Minimap_Pixel Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub OpenGL_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        Main_Map.SuppressMinimap = False

        If e.Button = Windows.Forms.MouseButtons.Left Then
            If GetMouseLeftDownOverMinimap() IsNot Nothing Then

            Else
                Select Case Tool
                    Case enumTool.AutoTexture_Place
                        Main_Map.UndoStepCreate("Ground Painted")
                    Case enumTool.CliffTriangle
                        Main_Map.UndoStepCreate("Cliff Triangles")
                    Case enumTool.AutoCliff
                        Main_Map.UndoStepCreate("Cliff Brush")
                    Case enumTool.AutoCliffRemove
                        Main_Map.UndoStepCreate("Cliff Remove Brush")
                    Case enumTool.Height_Change_Brush
                        Main_Map.UndoStepCreate("Height Change")
                    Case enumTool.Height_Set_Brush
                        Main_Map.UndoStepCreate("Height Set")
                    Case enumTool.Height_Smooth_Brush
                        Main_Map.UndoStepCreate("Height Smooth")
                    Case enumTool.Texture_Brush
                        Main_Map.UndoStepCreate("Texture")
                    Case enumTool.AutoRoad_Remove
                        Main_Map.UndoStepCreate("Road Remove")
                    Case enumTool.None
                        If Main_Map.Unit_Selected_Area_VertexA IsNot Nothing Then
                            If MouseOverTerrain IsNot Nothing Then
                                SelectUnits(Main_Map.Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal)
                            End If
                            Main_Map.Unit_Selected_Area_VertexA = Nothing
                        End If
                End Select
            End If
            MouseLeftDown = Nothing
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            If GetMouseRightDownOverMinimap() IsNot Nothing Then

            Else
                Select Case Tool
                    Case enumTool.Height_Change_Brush
                        Main_Map.UndoStepCreate("Height Change")
                    Case enumTool.Height_Set_Brush
                        Main_Map.UndoStepCreate("Height Set")
                End Select
            End If
            MouseRightDown = Nothing
        End If
    End Sub

    Private Sub SelectUnits(ByVal VertexA As sXY_int, ByVal VertexB As sXY_int)
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()
        Dim A As Integer
        Dim SectorNum As sXY_int
        Dim tmpUnit As clsMap.clsUnit
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim StartPos As sXY_int
        Dim FinishPos As sXY_int
        Dim StartVertex As sXY_int
        Dim FinishVertex As sXY_int

        If Math.Abs(VertexA.X - VertexB.X) <= 1 And _
          Math.Abs(VertexA.Y - VertexB.Y) <= 1 And _
          MouseOverTerrain IsNot Nothing Then
            If MouseOverTerrain.UnitCount > 0 Then
                If MouseOverTerrain.UnitCount = 1 Then
                    If MouseOverTerrain.Units(0).Map_SelectedUnitNum < 0 Then
                        Main_Map.SelectedUnit_Add(MouseOverTerrain.Units(0))
                    Else
                        Main_Map.SelectedUnit_Remove(MouseOverTerrain.Units(0).Map_SelectedUnitNum)
                    End If
                Else
                    ListSelect.Close()
                    ListSelect.Items.Clear()
                    ReDim ListSelectItems(MouseOverTerrain.UnitCount - 1)
                    For A = 0 To MouseOverTerrain.UnitCount - 1
                        If MouseOverTerrain.Units(A).Type Is Nothing Then
                            ListSelectItems(A) = New ToolStripButton("<nothing>")
                        Else
                            ListSelectItems(A) = New ToolStripButton(MouseOverTerrain.Units(A).Type.GetDisplayText)
                        End If
                        ListSelectItems(A).Tag = MouseOverTerrain.Units(A)
                        ListSelect.Items.Add(ListSelectItems(A))
                    Next
                    ListSelectIsPicker = False
                    ListSelect.Show(Me, New Drawing.Point(MouseOver.ScreenPos.X, MouseOver.ScreenPos.Y))
                End If
            End If
        Else
            XY_Reorder(VertexA, VertexB, StartVertex, FinishVertex)
            StartPos.X = StartVertex.X * TerrainGridSpacing
            StartPos.Y = StartVertex.Y * TerrainGridSpacing
            FinishPos.X = FinishVertex.X * TerrainGridSpacing
            FinishPos.Y = FinishVertex.Y * TerrainGridSpacing
            SectorStart.X = Math.Min(CInt(Int(StartVertex.X / SectorTileSize)), Main_Map.SectorCount.X - 1)
            SectorStart.Y = Math.Min(CInt(Int(StartVertex.Y / SectorTileSize)), Main_Map.SectorCount.Y - 1)
            SectorFinish.X = Math.Min(CInt(Int(FinishVertex.X / SectorTileSize)), Main_Map.SectorCount.X - 1)
            SectorFinish.Y = Math.Min(CInt(Int(FinishVertex.Y / SectorTileSize)), Main_Map.SectorCount.Y - 1)
            For SectorNum.Y = SectorStart.Y To SectorFinish.Y
                For SectorNum.X = SectorStart.X To SectorFinish.X
                    For A = 0 To Main_Map.Sectors(SectorNum.X, SectorNum.Y).UnitCount - 1
                        tmpUnit = Main_Map.Sectors(SectorNum.X, SectorNum.Y).Units(A)
                        If tmpUnit.Pos.Horizontal.X >= StartPos.X And tmpUnit.Pos.Horizontal.Y >= StartPos.Y And _
                            tmpUnit.Pos.Horizontal.X <= FinishPos.X And tmpUnit.Pos.Horizontal.Y <= FinishPos.Y Then
                            Main_Map.SelectedUnit_Add(tmpUnit)
                        End If
                    Next
                Next
            Next
        End If
        frmMainInstance.SelectedObject_Changed()
        DrawViewLater()
    End Sub

    Private Sub tmrDrawDelay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDrawDelay.Tick

        If DrawPending Then
            DrawPending = False
            DrawView()
        Else
            tmrDrawDelay.Enabled = False
        End If
    End Sub

    Private Sub pnlDraw_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlDraw.Resize

        If OpenGLControl IsNot Nothing Then
            OpenGL_Size_Calc()
        End If
    End Sub

    Public Sub View_Radius_Set(ByVal Radius As Double)

        VisionSectors.Radius = Radius / (TerrainGridSpacing * SectorTileSize)
    End Sub

    Public Function Pos_Get_Screen_XY(ByVal Pos As Matrix3D.XYZ_dbl, ByRef Result As sXY_int) As Boolean

        If Pos.Z <= 0.0# Then
            Return False
        End If

        Try
            Dim RatioZ_px As Double = 1.0# / (FOVMultiplier * Pos.Z)
            Result.X = CInt(GLSize.X / 2.0# + (Pos.X * RatioZ_px))
            Result.Y = CInt(GLSize.Y / 2.0# - (Pos.Y * RatioZ_px))
            Return True
        Catch

        End Try

        Return False
    End Function

    Public Function ScreenXY_Get_ViewPlanePos(ByVal ScreenPos As sXY_int, ByVal PlaneHeight As Double, ByRef ResultPos As sXY_dbl) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp2 As Double

        Try
            'convert screen pos to vector of one pos unit
            dblTemp2 = FOVMultiplier
            XYZ_dbl.X = (ScreenPos.X - GLSize.X / 2.0#) * dblTemp2
            XYZ_dbl.Y = (GLSize.Y / 2.0# - ScreenPos.Y) * dblTemp2
            XYZ_dbl.Z = 1.0#
            'factor in the view angle
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
            'get distance to cover the height
            dblTemp = (PlaneHeight - ViewPos.Y) / XYZ_dbl2.Y
            ResultPos.X = ViewPos.X + XYZ_dbl2.X * dblTemp
            ResultPos.Y = ViewPos.Z + XYZ_dbl2.Z * dblTemp
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_TerrainPos(ByVal ScreenPos As sXY_int, ByRef ResultPos As sWorldPos) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim TerrainViewVector As Matrix3D.XYZ_dbl
        Dim X As Integer
        Dim Y As Integer
        Dim LimitA As sXY_dbl
        Dim LimitB As sXY_dbl
        Dim Min As sXY_int
        Dim Max As sXY_int
        Dim TriGradientX As Double
        Dim TriGradientZ As Double
        Dim TriHeightOffset As Double
        Dim Dist As Double
        Dim BestPos As Matrix3D.XYZ_dbl
        Dim BestDist As Double
        Dim Dif As Matrix3D.XYZ_dbl
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim TilePos As sXY_dbl
        Dim TerrainViewPos As Matrix3D.XYZ_dbl

        Try

            TerrainViewPos.X = ViewPos.X
            TerrainViewPos.Y = ViewPos.Y
            TerrainViewPos.Z = -ViewPos.Z

            'convert screen pos to vector of one pos unit
            XYZ_dbl.X = (ScreenPos.X - GLSize.X / 2.0#) * FOVMultiplier
            XYZ_dbl.Y = (GLSize.Y / 2.0# - ScreenPos.Y) * FOVMultiplier
            XYZ_dbl.Z = 1.0#
            'rotate the vector so that it points forward and level
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, TerrainViewVector)
            TerrainViewVector.Y = -TerrainViewVector.Y 'get the amount of looking down, not up
            TerrainViewVector.Z = -TerrainViewVector.Z 'convert to terrain coordinates from view coordinates
            'get range of possible tiles
            dblTemp = (TerrainViewPos.Y - 255 * Main_Map.HeightMultiplier) / TerrainViewVector.Y
            LimitA.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitA.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            dblTemp = TerrainViewPos.Y / TerrainViewVector.Y
            LimitB.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitB.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            Min.X = Math.Max(CInt(Int(Math.Min(LimitA.X, LimitB.X) / TerrainGridSpacing)), 0)
            Min.Y = Math.Max(CInt(Int(Math.Min(LimitA.Y, LimitB.Y) / TerrainGridSpacing)), 0)
            Max.X = Math.Min(CInt(Int(Math.Max(LimitA.X, LimitB.X) / TerrainGridSpacing)), Main_Map.Terrain.TileSize.X - 1)
            Max.Y = Math.Min(CInt(Int(Math.Max(LimitA.Y, LimitB.Y) / TerrainGridSpacing)), Main_Map.Terrain.TileSize.Y - 1)
            'find the nearest valid tile to the view
            BestDist = Double.MaxValue
            BestPos.X = Double.NaN
            BestPos.Y = Double.NaN
            BestPos.Z = Double.NaN
            For Y = Min.Y To Max.Y
                For X = Min.X To Max.X

                    TilePos.X = X * TerrainGridSpacing
                    TilePos.Y = Y * TerrainGridSpacing

                    If Main_Map.Terrain.Tiles(X, Y).Tri Then
                        TriHeightOffset = Main_Map.Terrain.Vertices(X, Y).Height * Main_Map.HeightMultiplier
                        TriGradientX = Main_Map.Terrain.Vertices(X + 1, Y).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Main_Map.Terrain.Vertices(X, Y + 1).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ <= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Main_Map.Terrain.Vertices(X + 1, Y + 1).Height * Main_Map.HeightMultiplier
                        TriGradientX = Main_Map.Terrain.Vertices(X, Y + 1).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Main_Map.Terrain.Vertices(X + 1, Y).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + TriGradientZ + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ >= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                    Else

                        TriHeightOffset = Main_Map.Terrain.Vertices(X + 1, Y).Height * Main_Map.HeightMultiplier
                        TriGradientX = Main_Map.Terrain.Vertices(X, Y).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Main_Map.Terrain.Vertices(X + 1, Y + 1).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ <= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Main_Map.Terrain.Vertices(X, Y + 1).Height * Main_Map.HeightMultiplier
                        TriGradientX = Main_Map.Terrain.Vertices(X + 1, Y + 1).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Main_Map.Terrain.Vertices(X, Y).Height * Main_Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientZ + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Y
                        If InTileZ >= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            Dist = Dif.GetMagnitude
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                    End If
                Next
            Next

            If BestPos.X = Double.NaN Then
                Return False
            End If

            ResultPos.Horizontal.X = CInt(BestPos.X)
            ResultPos.Altitude = CInt(BestPos.Y)
            ResultPos.Horizontal.Y = CInt(BestPos.Z)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_ViewPlanePos_ForwardDownOnly(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByVal PlaneHeight As Double, ByRef ResultPos As sXY_dbl) As Boolean
        Dim dblTemp As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp2 As Double

        If ViewPos.Y < PlaneHeight Then
            Return False
        End If

        Try

            'convert screen pos to vector of one pos unit
            dblTemp2 = FOVMultiplier
            XYZ_dbl.X = (ScreenX - GLSize.X / 2.0#) * dblTemp2
            XYZ_dbl.Y = (GLSize.Y / 2.0# - ScreenY) * dblTemp2
            XYZ_dbl.Z = 1.0#
            'factor in the view angle
            Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
            'get distance to cover the height
            If XYZ_dbl2.Y > 0.0# Then
                Return False
            End If
            dblTemp = (PlaneHeight - ViewPos.Y) / XYZ_dbl2.Y
            ResultPos.X = ViewPos.X + XYZ_dbl2.X * dblTemp
            ResultPos.Y = ViewPos.Z + XYZ_dbl2.Z * dblTemp
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Sub Apply_Gateway()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        If Control_Gateway_Delete.Active Then
            Dim A As Integer
            Dim Low As sXY_int
            Dim High As sXY_int
            A = 0
            Do While A < Main_Map.GatewayCount
                XY_Reorder(Main_Map.Gateways(A).PosA, Main_Map.Gateways(A).PosB, Low, High)
                If Low.X <= Tile.X _
                And High.X >= Tile.X _
                And Low.Y <= Tile.Y _
                And High.Y >= Tile.Y Then
                    Main_Map.Gateway_Remove(A)
                    Main_Map.UndoStepCreate("Gateway Delete")
                    Main_Map.MinimapMakeLater()
                    DrawViewLater()
                    Exit Do
                End If
                A += 1
            Loop
        Else
            If Main_Map.Selected_Tile_A Is Nothing Then
                Main_Map.Selected_Tile_A = New clsXY_int(Tile)
                DrawViewLater()
            ElseIf Tile.X = Main_Map.Selected_Tile_A.X Or Tile.Y = Main_Map.Selected_Tile_A.Y Then
                If Main_Map.Gateway_Add(Main_Map.Selected_Tile_A.XY, Tile) Then
                    Main_Map.UndoStepCreate("Gateway Place")
                    Main_Map.Selected_Tile_A = Nothing
                    Main_Map.Selected_Tile_B = Nothing
                    Main_Map.MinimapMakeLater()
                    DrawViewLater()
                End If
            End If
        End If
    End Sub

    Public Sub OpenGL_Resize(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        GLSize.X = OpenGLControl.Width
        GLSize.Y = OpenGLControl.Height
        If GLSize.Y <> 0 Then
            GLSize_XPerY = CSng(GLSize.X / GLSize.Y)
        End If
        Viewport_Resize()
        FOV_Calc()
    End Sub

    Public Sub OpenGL_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)

        If Form.ActiveForm Is frmMainInstance Then
            OpenGLControl.Focus()
        End If
    End Sub

    Public Sub OpenGL_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Move As sXYZ_int
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim A As Integer

        For A = 0 To CInt(Math.Abs(e.Delta / 120.0#))
            Matrix3D.VectorForwardsRotationByMatrix(ViewAngleMatrix, Math.Sign(e.Delta) * Math.Max(ViewPos.Y, 512.0#) / 24.0#, XYZ_dbl)
            Move.Set_dbl(XYZ_dbl)
            ViewPosChange(Move)
        Next
    End Sub

    Public Function CreateGLFont(ByVal BaseFont As Font) As GLFont

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If
        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

    Public Sub DrawUnitRectangle(ByVal Position As Matrix3D.XYZ_dbl, ByVal Footprint As sXY_int, ByVal BorderInsideThickness As Double, ByVal InsideColour As sRGBA_sng, ByVal OutsideColour As sRGBA_sng)
        Dim PosA As sXY_dbl
        Dim PosB As sXY_dbl
        Dim PosC As sXY_dbl
        Dim PosD As sXY_dbl

        PosA.X = Position.X - (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosA.Y = Position.Z - (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosB.X = Position.X + (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosB.Y = Position.Z - (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosC.X = Position.X - (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosC.Y = Position.Z + (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosD.X = Position.X + (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosD.Y = Position.Z + (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha)
        GL.Vertex3(PosB.X, Position.Y, -PosB.Y)
        GL.Vertex3(PosA.X, Position.Y, -PosA.Y)
        GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha)
        GL.Vertex3(PosA.X + BorderInsideThickness, Position.Y, -(PosA.Y + BorderInsideThickness))
        GL.Vertex3(PosB.X - BorderInsideThickness, Position.Y, -(PosB.Y + BorderInsideThickness))

        GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha)
        GL.Vertex3(PosA.X, Position.Y, -PosA.Y)
        GL.Vertex3(PosC.X, Position.Y, -PosC.Y)
        GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha)
        GL.Vertex3(PosC.X + BorderInsideThickness, Position.Y, -(PosC.Y - BorderInsideThickness))
        GL.Vertex3(PosA.X + BorderInsideThickness, Position.Y, -(PosA.Y + BorderInsideThickness))

        GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha)
        GL.Vertex3(PosD.X, Position.Y, -PosD.Y)
        GL.Vertex3(PosB.X, Position.Y, -PosA.Y)
        GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha)
        GL.Vertex3(PosB.X - BorderInsideThickness, Position.Y, -(PosB.Y + BorderInsideThickness))
        GL.Vertex3(PosD.X - BorderInsideThickness, Position.Y, -(PosD.Y - BorderInsideThickness))

        GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha)
        GL.Vertex3(PosC.X, Position.Y, -PosC.Y)
        GL.Vertex3(PosD.X, Position.Y, -PosD.Y)
        GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha)
        GL.Vertex3(PosD.X - BorderInsideThickness, Position.Y, -(PosD.Y - BorderInsideThickness))
        GL.Vertex3(PosC.X + BorderInsideThickness, Position.Y, -(PosC.Y - BorderInsideThickness))
    End Sub

    Public Sub TerrainPicker()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Vertex As sXY_int = MouseOverTerrain.Vertex.Normal
        Dim A As Integer

        frmMainInstance.lstAutoTexture.Enabled = False
        For A = 0 To frmMainInstance.lstAutoTexture.Items.Count - 1
            If Main_Map.Painter.Terrains(A) Is Main_Map.Terrain.Vertices(Vertex.X, Vertex.Y).Terrain Then
                frmMainInstance.lstAutoTexture.SelectedIndex = A
                Exit For
            End If
        Next
        If A = frmMainInstance.lstAutoTexture.Items.Count Then
            frmMainInstance.lstAutoTexture.SelectedIndex = -1
        End If
        frmMainInstance.lstAutoTexture.Enabled = True
        SelectedTerrain = Main_Map.Terrain.Vertices(Vertex.X, Vertex.Y).Terrain
    End Sub

    Public Sub TexturePicker()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        Dim Tile As sXY_int = MouseOverTerrain.Tile.Normal

        If Main_Map.Tileset IsNot Nothing Then
            If Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.TextureNum < Main_Map.Tileset.TileCount Then
                SelectedTextureNum = Main_Map.Terrain.Tiles(Tile.X, Tile.Y).Texture.TextureNum
                frmMainInstance.TextureView.DrawViewLater()
            End If
        End If
    End Sub

    Public Sub HeightPickerL()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        frmMainInstance.txtHeightSetL.Text = CStr(Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height)
        frmMainInstance.txtHeightSetL.Focus()
        OpenGLControl.Focus()
    End Sub

    Public Sub HeightPickerR()
        Dim MouseOverTerrain As clsMouseOver.clsOverTerrain = GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            Exit Sub
        End If

        frmMainInstance.txtHeightSetR.Text = CStr(Main_Map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height)
        frmMainInstance.txtHeightSetR.Focus()
        OpenGLControl.Focus()
    End Sub

    Public Sub ObjectPicker(ByVal UnitType As clsUnitType)

        Tool = enumTool.ObjectPlace
        frmMainInstance.lstFeatures.SelectedIndex = -1
        frmMainInstance.lstStructures.SelectedIndex = -1
        frmMainInstance.lstDroids.SelectedIndex = -1
        frmMainInstance.SelectedObjectType = UnitType
        Main_Map.MinimapMakeLater() 'for unit highlight
        DrawViewLater()
    End Sub

    Public WithEvents UndoMessageTimer As Timer

    Public Sub RemoveUndoMessage(ByVal sender As Object, ByVal e As EventArgs) Handles UndoMessageTimer.Tick

        UndoMessageTimer.Enabled = False
        lblUndo.Text = ""
    End Sub

    Public Sub DisplayUndoMessage(ByVal Text As String)

        lblUndo.Text = Text
        UndoMessageTimer.Enabled = False
        UndoMessageTimer.Enabled = True
    End Sub

    Private Sub OpenGL_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)

        DrawViewLater()
    End Sub

    Private Sub OpenGL_MouseLeave(sender As Object, e As System.EventArgs)

        MouseOver = Nothing
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.ssStatus = New System.Windows.Forms.StatusStrip()
        Me.lblTile = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblVertex = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblPos = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblUndo = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.ssStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'ssStatus
        '
        Me.ssStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblTile, Me.lblVertex, Me.lblPos, Me.lblUndo})
        Me.ssStatus.Location = New System.Drawing.Point(0, 392)
        Me.ssStatus.Name = "ssStatus"
        Me.ssStatus.Size = New System.Drawing.Size(1308, 32)
        Me.ssStatus.TabIndex = 0
        Me.ssStatus.Text = "StatusStrip1"
        '
        'lblTile
        '
        Me.lblTile.AutoSize = False
        Me.lblTile.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTile.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblTile.Name = "lblTile"
        Me.lblTile.Size = New System.Drawing.Size(192, 27)
        Me.lblTile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVertex
        '
        Me.lblVertex.AutoSize = False
        Me.lblVertex.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVertex.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblVertex.Name = "lblVertex"
        Me.lblVertex.Size = New System.Drawing.Size(256, 27)
        Me.lblVertex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPos
        '
        Me.lblPos.AutoSize = False
        Me.lblPos.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPos.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblPos.Name = "lblPos"
        Me.lblPos.Size = New System.Drawing.Size(320, 27)
        Me.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUndo
        '
        Me.lblUndo.AutoSize = False
        Me.lblUndo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUndo.Name = "lblUndo"
        Me.lblUndo.Size = New System.Drawing.Size(256, 27)
        Me.lblUndo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlDraw
        '
        Me.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDraw.Location = New System.Drawing.Point(0, 0)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(1308, 392)
        Me.pnlDraw.TabIndex = 1
        '
        'ctrlMapView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.pnlDraw)
        Me.Controls.Add(Me.ssStatus)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlMapView"
        Me.Size = New System.Drawing.Size(1308, 424)
        Me.ssStatus.ResumeLayout(False)
        Me.ssStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents ssStatus As System.Windows.Forms.StatusStrip
    Public WithEvents lblTile As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblVertex As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblPos As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents pnlDraw As System.Windows.Forms.Panel
    Public WithEvents lblUndo As System.Windows.Forms.ToolStripStatusLabel
#End If
End Class