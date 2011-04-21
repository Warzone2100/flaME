Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class ctrlMapView
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public DrawPending As Boolean

    Public OpenGLControl As OpenTK.GLControl

    Public GLSize As sXY_int
    Public GLSize_XPerY As Double

    Public MouseOver_Exists As Boolean
    Public MouseOver_ScreenPos As sXY_int
    Public MouseOver_Pos_Exists As Boolean
    Public MouseOver_Pos As sXYZ_lng
    Public MouseOver_Units() As clsMap.clsUnit
    Public MouseOver_UnitCount As Integer
    Public MouseLeftIsDown As Boolean
    Public MouseRightIsDown As Boolean
    Public MouseOver_Tile As sXY_int
    Public MouseOver_Vertex As sXY_int
    Public MouseOver_Side_IsV As Boolean
    Public MouseOver_Side_Num As sXY_int
    Public Tiles_Per_Minimap_Pixel As Double

    Public IsMinimap_MouseDown As Boolean

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

    Public BGColor As sRGB_sng

    Enum enumView_Move_Type As Byte
        Free
        RTS
    End Enum
    Public ViewMoveType As enumView_Move_Type = enumView_Move_Type.RTS
    Public RTSOrbit As Boolean = True
    Public ViewPos As sXYZ_int
    Public ViewAngleMatrix(8) As Double
    Public ViewAngleMatrix_Inverted(8) As Double
    Public ViewAngleRPY As sAngleRPY
    Public FieldOfViewY As Double

    Public FOVMultiplierExponent As Double
    Public FOVMultiplier As Double

    Public DrawView_Enabled As Boolean = False

    Public VisionSectors As sBrushTiles

    Private GLInitializeDelayTimer As Timer
    Public IsGLInitialized As Boolean = False

    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ListSelect = New ContextMenuStrip
        UndoMessageTimer = New Timer

        ' Add any initialization after the InitializeComponent() call.
        OpenGLControl = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(32), 24, 0))
        OpenGLControl.MakeCurrent() 'mono version fails without this
        pnlDraw.Controls.Add(OpenGLControl)

        GLInitializeDelayTimer = New Timer
        GLInitializeDelayTimer.Interval = 1
        AddHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Enabled = True

        UndoMessageTimer.Interval = 4000
    End Sub

    Sub OpenGL_Size_Calc()

        OpenGLControl.Width = pnlDraw.Width
        OpenGLControl.Height = pnlDraw.Height
        Viewport_Resize()
    End Sub

    Sub DrawView_SetEnabled(ByVal Value As Boolean)

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

    Sub DrawViewLater()

        DrawPending = True
        If Not tmrDraw_Delay.Enabled Then
            tmrDraw.Enabled = True
        End If
    End Sub

    Private Sub tmrDraw_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDraw.Tick

        tmrDraw.Enabled = False
        If DrawPending Then
            DrawView()
            DrawPending = False
            tmrDraw_Delay.Enabled = True
        End If
    End Sub

    Sub FOV_Calc()

        FieldOfViewY = Clamp(Math.Atan(GLSize.Y * FOVMultiplier / 2.0#) * 2.0#, RadOf1Deg, 179.0# * RadOf1Deg)

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

        FOV_Multiplier_Set(FOVDefault)
    End Sub

    Sub Viewport_Resize()

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

    Private Sub Draw_TextLabel(ByRef TextLabel() As sTextLabel)
        Static A As Integer

        For A = 0 To TextLabel.GetUpperBound(0)
            Draw_TextLabel(TextLabel(A))
        Next
    End Sub

    Private Sub Draw_TextLabel(ByRef TextLabel As sTextLabel)
        If TextLabel.Text Is Nothing Then
            Exit Sub
        End If
        If TextLabel.Text.Length = 0 Then
            Exit Sub
        End If
        If TextLabel.Font Is Nothing Then
            Exit Sub
        End If

        Dim CharCode As Integer
        Dim CharWidth As Single
        Dim TexRatio As sXY_sng
        Dim LetterPosA As Single
        Dim LetterPosB As Single
        Dim PosY1 As Single
        Dim PosY2 As Single
        Dim CharSpacing As Single
        Dim A As Integer

        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
        GL.Color4(TextLabel.Colour.Red, TextLabel.Colour.Green, TextLabel.Colour.Blue, TextLabel.Colour.Alpha)
        PosY1 = TextLabel.Pos.Y
        PosY2 = TextLabel.Pos.Y + TextLabel.SizeY
        CharSpacing = TextLabel.SizeY / 10.0F
        LetterPosA = TextLabel.Pos.X
        For A = 0 To TextLabel.Text.Length - 1
            CharCode = Asc(TextLabel.Text(A))
            If CharCode >= 0 And CharCode <= 255 Then
                CharWidth = TextLabel.SizeY * TextLabel.Font.Character(CharCode).Width / TextLabel.Font.Height
                TexRatio.X = TextLabel.Font.Character(CharCode).Width / TextLabel.Font.Character(CharCode).TexSize
                TexRatio.Y = TextLabel.Font.Height / TextLabel.Font.Character(CharCode).TexSize
                LetterPosB = LetterPosA + CharWidth
                GL.BindTexture(TextureTarget.Texture2D, TextLabel.Font.Character(CharCode).GLTexture)
                GL.Begin(BeginMode.Quads)
                GL.TexCoord2(0.0F, 0.0F)
                GL.Vertex2(LetterPosA, PosY1)
                GL.TexCoord2(TexRatio.X, 0.0F)
                GL.Vertex2(LetterPosB, PosY1)
                GL.TexCoord2(TexRatio.X, TexRatio.Y)
                GL.Vertex2(LetterPosB, PosY2)
                GL.TexCoord2(0.0F, TexRatio.Y)
                GL.Vertex2(LetterPosA, PosY2)
                GL.End()
                LetterPosA = LetterPosB + CharSpacing
            End If
        Next
    End Sub

    Sub FOV_Scale_2E_Set(ByVal Power As Double)

        FOVMultiplierExponent = Power
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent
        FOV_Calc()
    End Sub

    Sub FOV_Scale_2E_Change(ByVal Power_Change As Double)

        FOVMultiplierExponent += Power_Change
        FOVMultiplier = 2.0# ^ FOVMultiplierExponent
        FOV_Calc()
    End Sub

    Sub FOV_Set(ByVal Radians As Double)

        FOVMultiplier = Math.Tan(Radians / 2.0#) / GLSize.Y * 2.0#
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
        FOV_Calc()
    End Sub

    Sub FOV_Multiplier_Set(ByVal Value As Double)

        FOVMultiplier = Value
        FOVMultiplierExponent = Math.Log(FOVMultiplier) / Math.Log(2.0#)
        FOV_Calc()
    End Sub

    Private Sub DrawView()
        Static XYZ_dbl As sXYZ_dbl
        Static XY_int As sXY_int
        Static X As Integer
        Static Z As Integer
        Static X2 As Integer
        Static Z2 As Integer
        Static A As Integer
        Static B As Integer
        Static C As Integer
        Static D As Integer
        Static ShowMinimapViewPosBox As Boolean
        Static ViewCorner0 As sXY_dbl
        Static ViewCorner1 As sXY_dbl
        Static ViewCorner2 As sXY_dbl
        Static ViewCorner3 As sXY_dbl
        Static dblTemp As Double
        Static dblTemp2 As Double
        Static Vertex0 As sXYZ_dbl
        Static Vertex1 As sXYZ_dbl
        Static Vertex2 As sXYZ_dbl
        Static Vertex3 As sXYZ_dbl
        Static ScreenX As Integer
        Static ScreenY As Integer
        Static XYZ_dbl2 As sXYZ_dbl
        'Static XYZ_dbl3 As sXYZ_dbl
        Static XYZ_int As sXYZ_int
        Static NewUnitType As clsUnitType
        Static PosA As sXY_dbl
        Static PosB As sXY_dbl
        Static PosC As sXY_dbl
        Static PosD As sXY_dbl
        Static MinimapSize As sXY_int
        Static Draw_Unit_Label As Boolean
        Static tmpUnit As clsMap.clsUnit
        Static StartXY As sXY_int
        Static FinishXY As sXY_int
        Static DrawIt As Boolean
        Static DrawCentre As sXY_dbl
        Static DrawCentreSector As sXY_int
        Static UnitTextLabels() As sTextLabel
        Static UnitTextLabelCount As Integer
        Static SelectionLabel As sTextLabel
        'Static RGB_sng As sRGB_sng
        Static light_position(3) As Single
        Static matrixA(8) As Double
        Static matrixB(8) As Double
        Static X3 As Integer
        Static Z3 As Integer

        If Not (DrawView_Enabled And IsGLInitialized) Then
            Exit Sub
        End If

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

        MatrixSetToPY(matrixA, New sAnglePY(-SunPitch, SunHeading))
        MatrixRotationByMatrix(ViewAngleMatrix_Inverted, matrixA, matrixB)
        VectorForwardRotationByMatrix(matrixB, XYZ_dbl)
        light_position(0) = XYZ_dbl.X
        light_position(1) = XYZ_dbl.Y
        light_position(2) = -XYZ_dbl.Z
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

        dblTemp = 127.5# * Map.HeightMultiplier
        If ScreenXY_Get_PlanePos_ForwardDownOnly(0, 0, dblTemp, ViewCorner0) _
        And ScreenXY_Get_PlanePos_ForwardDownOnly(GLSize.X, 0, dblTemp, ViewCorner1) _
        And ScreenXY_Get_PlanePos_ForwardDownOnly(GLSize.X, GLSize.Y, dblTemp, ViewCorner2) _
        And ScreenXY_Get_PlanePos_ForwardDownOnly(0, GLSize.Y, dblTemp, ViewCorner3) Then
            ShowMinimapViewPosBox = True
        Else
            ShowMinimapViewPosBox = False
        End If

        If ScreenXY_Get_PlanePos(GLSize.X / 2.0#, GLSize.Y / 2.0#, dblTemp, DrawCentre) Then
            XYZ_dbl.X = DrawCentre.X - ViewPos.X
            XYZ_dbl.Z = DrawCentre.Y - ViewPos.Z
            dblTemp2 = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z)
            If dblTemp2 > VisionRadius * 2.0# Then
                DrawCentre.X = ViewPos.X + XYZ_dbl.X * VisionRadius * 2.0# / dblTemp2
                DrawCentre.Y = ViewPos.Z + XYZ_dbl.Z * VisionRadius * 2.0# / dblTemp2
            End If
        Else
            VectorForwardRotationByMatrix(ViewAngleMatrix, XYZ_dbl)
            dblTemp2 = VisionRadius * 2.0# / Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z)
            DrawCentre.X = ViewPos.X + XYZ_dbl.X * dblTemp2
            DrawCentre.Y = ViewPos.Z + XYZ_dbl.Z * dblTemp2
        End If
        DrawCentre.X = Clamp(DrawCentre.X, 0.0#, Map.TerrainSize.X * TerrainGridSpacing - 1.0#)
        DrawCentre.Y = Clamp(-DrawCentre.Y, 0.0#, Map.TerrainSize.Y * TerrainGridSpacing - 1.0#)
        Map.Pos_Get_Sector(DrawCentre.X, DrawCentre.Y, DrawCentreSector)

        GL.Rotate(AngleClamp(-ViewAngleRPY.Roll) / RadOf1Deg, 0.0F, 0.0F, 1.0F)
        GL.Rotate(ViewAngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
        GL.Rotate(ViewAngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
        GL.Translate(-ViewPos.X, -ViewPos.Y, ViewPos.Z)

        X2 = DrawCentreSector.X
        Z2 = DrawCentreSector.Y

        GL.Enable(EnableCap.CullFace)

        If Draw_TileTextures Then
            GL.Color3(1.0F, 1.0F, 1.0F)
            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
            For Z = Clamp(Z2 + VisionSectors.ZMin, 0, Map.SectorCount.Y - 1) To Clamp(Z2 + VisionSectors.ZMax, 0, Map.SectorCount.Y - 1)
                For X = Clamp(X2 + VisionSectors.XMin(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1) To Clamp(X2 + VisionSectors.XMax(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1)
                    GL.CallList(Map.Sectors(X, Z).GLList_Textured)
                Next
            Next
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)
        GL.Disable(EnableCap.Lighting)

        If Draw_TileWireframe Then
            GL.Color3(0.0F, 1.0F, 0.0F)
            GL.LineWidth(1.0F)
            For Z = Clamp(Z2 + VisionSectors.ZMin, 0, Map.SectorCount.Y - 1) To Clamp(Z2 + VisionSectors.ZMax, 0, Map.SectorCount.Y - 1)
                For X = Clamp(X2 + VisionSectors.XMin(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1) To Clamp(X2 + VisionSectors.XMax(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1)
                    GL.CallList(Map.Sectors(X, Z).GLList_Wireframe)
                Next
            Next
        End If

        'draw tile orientation markers

        If DisplayTileOrientation Then

            GL.Disable(EnableCap.CullFace)

            GL.Begin(BeginMode.Triangles)
            GL.Color3(1.0F, 1.0F, 0.0F)
            For Z = Clamp(Z2 + VisionSectors.ZMin, 0, Map.SectorCount.Y - 1) To Clamp(Z2 + VisionSectors.ZMax, 0, Map.SectorCount.Y)
                For X = Clamp(X2 + VisionSectors.XMin(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1) To Clamp(X2 + VisionSectors.XMax(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X)
                    For Z3 = Z * SectorTileSize To Math.Min((Z + 1) * SectorTileSize - 1, Map.TerrainSize.Y - 1)
                        For X3 = X * SectorTileSize To Math.Min((X + 1) * SectorTileSize - 1, Map.TerrainSize.X - 1)
                            Map.DrawTileOrientation(X3, Z3)
                        Next
                    Next
                Next
            Next
            GL.End()

            GL.Enable(EnableCap.CullFace)
        End If

        'draw autotexture terrain type markers

        Static RGB_sng As sRGB_sng
        Static XYZ_dbl3 As sXYZ_dbl
        Static RGB_sng2 As sRGB_sng
        If Draw_VertexTerrain Then
            GL.LineWidth(1.0F)
            For Z = Clamp(Z2 + VisionSectors.ZMin, 0, Map.SectorCount.Y - 1) To Clamp(Z2 + VisionSectors.ZMax, 0, Map.SectorCount.Y)
                For X = Clamp(X2 + VisionSectors.XMin(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1) To Clamp(X2 + VisionSectors.XMax(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X)
                    For Z3 = Z * SectorTileSize To Math.Min((Z + 1) * SectorTileSize - 1, Map.TerrainSize.Y)
                        For X3 = X * SectorTileSize To Math.Min((X + 1) * SectorTileSize - 1, Map.TerrainSize.X)
                            If Map.TerrainVertex(X3, Z3).Terrain IsNot Nothing And Map.Tileset IsNot Nothing Then
                                A = Map.TerrainVertex(X3, Z3).Terrain.Num
                                If A < Map.Painter.TerrainCount Then
                                    If Map.Painter.Terrains(A).Tiles.TileCount >= 1 Then
                                        RGB_sng = Map.Tileset.Tiles(Map.Painter.Terrains(A).Tiles.Tiles(0).TextureNum).Average_Color
                                        If RGB_sng.Red + RGB_sng.Green + RGB_sng.Blue < 1.5F Then
                                            RGB_sng2.Red = (RGB_sng.Red + 1.0F) / 2.0F
                                            RGB_sng2.Green = (RGB_sng.Green + 1.0F) / 2.0F
                                            RGB_sng2.Blue = (RGB_sng.Blue + 1.0F) / 2.0F
                                        Else
                                            RGB_sng2.Red = RGB_sng.Red / 2.0F
                                            RGB_sng2.Green = RGB_sng.Green / 2.0F
                                            RGB_sng2.Blue = RGB_sng.Blue / 2.0F
                                        End If
                                        XYZ_dbl.X = X3 * TerrainGridSpacing
                                        XYZ_dbl.Y = Map.TerrainVertex(X3, Z3).Height * Map.HeightMultiplier
                                        XYZ_dbl.Z = -Z3 * TerrainGridSpacing
                                        XYZ_dbl2.X = 10.0#
                                        XYZ_dbl2.Y = 10.0#
                                        XYZ_dbl2.Z = 0.0#
                                        VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                        Vertex0.X = XYZ_dbl.X + XYZ_dbl3.X
                                        Vertex0.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                        Vertex0.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                        XYZ_dbl2.X = -10.0#
                                        XYZ_dbl2.Y = 10.0#
                                        XYZ_dbl2.Z = 0.0#
                                        VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                        Vertex1.X = XYZ_dbl.X + XYZ_dbl3.X
                                        Vertex1.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                        Vertex1.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                        XYZ_dbl2.X = -10.0#
                                        XYZ_dbl2.Y = -10.0#
                                        XYZ_dbl2.Z = 0.0#
                                        VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                        Vertex2.X = XYZ_dbl.X + XYZ_dbl3.X
                                        Vertex2.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                        Vertex2.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                        XYZ_dbl2.X = 10.0#
                                        XYZ_dbl2.Y = -10.0#
                                        XYZ_dbl2.Z = 0.0#
                                        VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                        Vertex3.X = XYZ_dbl.X + XYZ_dbl3.X
                                        Vertex3.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                        Vertex3.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                        GL.Begin(BeginMode.Quads)
                                        GL.Color3(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue)
                                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                                        GL.End()
                                        GL.Begin(BeginMode.LineLoop)
                                        GL.Color3(RGB_sng2.Red, RGB_sng2.Green, RGB_sng2.Blue)
                                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                                        GL.End()
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next
            Next
        End If

        SelectionLabel.Text = ""

        If Map.Selected_Area_VertexA_Exists Then
            DrawIt = False
            If Map.Selected_Area_VertexB_Exists Then
                'area is selected
                XY_Reorder(Map.Selected_Area_VertexA, Map.Selected_Area_VertexB, StartXY, FinishXY)
                XYZ_dbl.X = Map.Selected_Area_VertexB.X * TerrainGridSpacing - ViewPos.X
                XYZ_dbl.Z = -Map.Selected_Area_VertexB.Y * TerrainGridSpacing - ViewPos.Z
                XYZ_dbl.Y = Map.GetTerrainHeight(Map.Selected_Area_VertexB.X * TerrainGridSpacing, Map.Selected_Area_VertexB.Y * TerrainGridSpacing) - ViewPos.Y
                DrawIt = True
            ElseIf Tool = enumTool.Terrain_Select Then
                If MouseOver_Pos_Exists Then
                    'selection is changing under pointer
                    XY_Reorder(Map.Selected_Area_VertexA, MouseOver_Vertex, StartXY, FinishXY)
                    XYZ_dbl.X = MouseOver_Vertex.X * TerrainGridSpacing - ViewPos.X
                    XYZ_dbl.Z = -MouseOver_Vertex.Y * TerrainGridSpacing - ViewPos.Z
                    XYZ_dbl.Y = Map.GetTerrainHeight(MouseOver_Vertex.X * TerrainGridSpacing, MouseOver_Vertex.Y * TerrainGridSpacing) - ViewPos.Y
                    DrawIt = True
                End If
            End If
            If DrawIt Then
                VectorRotationByMatrix(ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                If Pos_Get_Screen_XY(XYZ_dbl2, ScreenX, ScreenY) Then
                    If ScreenX >= 0 And ScreenX <= GLSize.X And ScreenY >= 0 And ScreenY <= GLSize.Y Then
                        SelectionLabel.Colour.Red = 1.0F
                        SelectionLabel.Colour.Green = 1.0F
                        SelectionLabel.Colour.Blue = 1.0F
                        SelectionLabel.Colour.Alpha = 1.0F
                        SelectionLabel.Font = UnitLabelFont
                        SelectionLabel.SizeY = UnitLabelFontSize
                        SelectionLabel.Pos.X = ScreenX
                        SelectionLabel.Pos.Y = ScreenY
                        SelectionLabel.Text = FinishXY.X - StartXY.X & "x" & FinishXY.Y - StartXY.Y
                    End If
                End If
                GL.LineWidth(3.0F)
                For X = StartXY.X To FinishXY.X - 1
                    Vertex0.X = X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X, StartXY.Y).Height * Map.HeightMultiplier
                    Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                    Vertex1.X = (X + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X + 1, StartXY.Y).Height * Map.HeightMultiplier
                    Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For X = StartXY.X To FinishXY.X - 1
                    Vertex0.X = X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X, FinishXY.Y).Height * Map.HeightMultiplier
                    Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                    Vertex1.X = (X + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X + 1, FinishXY.Y).Height * Map.HeightMultiplier
                    Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For Z = StartXY.Y To FinishXY.Y - 1
                    Vertex0.X = StartXY.X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(StartXY.X, Z).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z * TerrainGridSpacing
                    Vertex1.X = StartXY.X * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(StartXY.X, Z + 1).Height * Map.HeightMultiplier
                    Vertex1.Z = -(Z + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
                For Z = StartXY.Y To FinishXY.Y - 1
                    Vertex0.X = FinishXY.X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(FinishXY.X, Z).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z * TerrainGridSpacing
                    Vertex1.X = FinishXY.X * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(FinishXY.X, Z + 1).Height * Map.HeightMultiplier
                    Vertex1.Z = -(Z + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(1.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.End()
                Next
            End If
        End If

        If Tool = enumTool.Terrain_Select Then
            If MouseOver_Pos_Exists Then
                'draw mouseover vertex
                GL.LineWidth(3.0F)

                Vertex0.X = MouseOver_Vertex.X * TerrainGridSpacing
                Vertex0.Y = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height * Map.HeightMultiplier
                Vertex0.Z = -MouseOver_Vertex.Y * TerrainGridSpacing
                GL.Begin(BeginMode.Lines)
                GL.Color3(1.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
                GL.End()
            End If
        ElseIf Tool = enumTool.Gateways Then
            GL.LineWidth(2.0F)
            For A = 0 To Map.GatewayCount - 1
                If Map.Gateways(A).PosA.X = Map.Gateways(A).PosB.X Then
                    If Map.Gateways(A).PosA.Y <= Map.Gateways(A).PosB.Y Then
                        C = Map.Gateways(A).PosA.Y
                        D = Map.Gateways(A).PosB.Y
                    Else
                        C = Map.Gateways(A).PosB.Y
                        D = Map.Gateways(A).PosA.Y
                    End If
                    X2 = Map.Gateways(A).PosA.X
                    For Z2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                        Vertex0.Z = -Z2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                        Vertex1.Z = -Z2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                        Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                        Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.LineLoop)
                        GL.Color3(0.75F, 1.0F, 0.0F)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                        GL.End()
                    Next
                ElseIf Map.Gateways(A).PosA.Y = Map.Gateways(A).PosB.Y Then
                    If Map.Gateways(A).PosA.X <= Map.Gateways(A).PosB.X Then
                        C = Map.Gateways(A).PosA.X
                        D = Map.Gateways(A).PosB.X
                    Else
                        C = Map.Gateways(A).PosB.X
                        D = Map.Gateways(A).PosA.X
                    End If
                    Z2 = Map.Gateways(A).PosA.Y
                    For X2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                        Vertex0.Z = -Z2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                        Vertex1.Z = -Z2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                        Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                        Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
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
                    X2 = Map.Gateways(A).PosA.X
                    Z2 = Map.Gateways(A).PosA.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(1.0F, 0.0F, 0.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    X2 = Map.Gateways(A).PosB.X
                    Z2 = Map.Gateways(A).PosB.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
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

        If MouseOver_Pos_Exists Then

            If Tool = enumTool.None Then
                If Map.Unit_Selected_Area_VertexA_Exists Then
                    'selection is changing under pointer
                    XY_Reorder(Map.Unit_Selected_Area_VertexA, MouseOver_Vertex, StartXY, FinishXY)
                    GL.LineWidth(2.0F)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(X, StartXY.Y).Height * Map.HeightMultiplier
                        Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(X + 1, StartXY.Y).Height * Map.HeightMultiplier
                        Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(X, FinishXY.Y).Height * Map.HeightMultiplier
                        Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(X + 1, FinishXY.Y).Height * Map.HeightMultiplier
                        Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Z = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = StartXY.X * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(StartXY.X, Z).Height * Map.HeightMultiplier
                        Vertex0.Z = -Z * TerrainGridSpacing
                        Vertex1.X = StartXY.X * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(StartXY.X, Z + 1).Height * Map.HeightMultiplier
                        Vertex1.Z = -(Z + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Z = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = FinishXY.X * TerrainGridSpacing
                        Vertex0.Y = Map.TerrainVertex(FinishXY.X, Z).Height * Map.HeightMultiplier
                        Vertex0.Z = -Z * TerrainGridSpacing
                        Vertex1.X = FinishXY.X * TerrainGridSpacing
                        Vertex1.Y = Map.TerrainVertex(FinishXY.X, Z + 1).Height * Map.HeightMultiplier
                        Vertex1.Z = -(Z + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                Else
                    GL.LineWidth(2.0F)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Begin(BeginMode.Lines)
                    GL.Vertex3(MouseOver_Pos.X - 16.0#, MouseOver_Pos.Y, -MouseOver_Pos.Z - 16.0#)
                    GL.Vertex3(MouseOver_Pos.X + 16.0#, MouseOver_Pos.Y, -MouseOver_Pos.Z + 16.0#)
                    GL.Vertex3(MouseOver_Pos.X + 16.0#, MouseOver_Pos.Y, -MouseOver_Pos.Z - 16.0#)
                    GL.Vertex3(MouseOver_Pos.X - 16.0#, MouseOver_Pos.Y, -MouseOver_Pos.Z + 16.0#)
                    GL.End()
                End If
            End If

            If Tool = enumTool.AutoRoad_Place Then
                GL.LineWidth(2.0F)

                If MouseOver_Side_IsV Then
                    Vertex0.X = MouseOver_Side_Num.X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Height * Map.HeightMultiplier
                    Vertex0.Z = -MouseOver_Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = MouseOver_Side_Num.X * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(MouseOver_Side_Num.X, MouseOver_Side_Num.Y + 1).Height * Map.HeightMultiplier
                    Vertex1.Z = -(MouseOver_Side_Num.Y + 1) * TerrainGridSpacing
                Else
                    Vertex0.X = MouseOver_Side_Num.X * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Height * Map.HeightMultiplier
                    Vertex0.Z = -MouseOver_Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = (MouseOver_Side_Num.X + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(MouseOver_Side_Num.X + 1, MouseOver_Side_Num.Y).Height * Map.HeightMultiplier
                    Vertex1.Z = -MouseOver_Side_Num.Y * TerrainGridSpacing
                End If

                GL.Begin(BeginMode.Lines)
                GL.Color3(0.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.End()
            ElseIf Tool = enumTool.AutoRoad_Line Or Tool = enumTool.Gateways Then
                GL.LineWidth(2.0F)

                If Map.Selected_Tile_A_Exists Then
                    X2 = Map.Selected_Tile_A.X
                    Z2 = Map.Selected_Tile_A.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    If MouseOver_Tile.X = Map.Selected_Tile_A.X Then
                        If MouseOver_Tile.Y <= Map.Selected_Tile_A.Y Then
                            A = MouseOver_Tile.Y
                            B = Map.Selected_Tile_A.Y
                        Else
                            A = Map.Selected_Tile_A.Y
                            B = MouseOver_Tile.Y
                        End If
                        X2 = Map.Selected_Tile_A.X
                        For Z2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                            Vertex0.Z = -Z2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                            Vertex1.Z = -Z2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                            Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                            Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                            GL.Begin(BeginMode.LineLoop)
                            GL.Color3(0.0F, 1.0F, 1.0F)
                            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                            GL.End()
                        Next
                    ElseIf MouseOver_Tile.Y = Map.Selected_Tile_A.Y Then
                        If MouseOver_Tile.X <= Map.Selected_Tile_A.X Then
                            A = MouseOver_Tile.X
                            B = Map.Selected_Tile_A.X
                        Else
                            A = Map.Selected_Tile_A.X
                            B = MouseOver_Tile.X
                        End If
                        Z2 = Map.Selected_Tile_A.Y
                        For X2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                            Vertex0.Z = -Z2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                            Vertex1.Z = -Z2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                            Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                            Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
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
                    X2 = MouseOver_Tile.X
                    Z2 = MouseOver_Tile.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
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
        If Tool = enumTool.Texture_Brush Then
            GL.LineWidth(2.0F)

            For Z = Clamp(TextureBrushRadius.ZMin + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y To Clamp(TextureBrushRadius.ZMax + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y
                Z2 = MouseOver_Tile.Y + Z
                For X = Clamp(TextureBrushRadius.XMin(Z - TextureBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X To Clamp(TextureBrushRadius.XMax(Z - TextureBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X
                    X2 = MouseOver_Tile.X + X

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()
                Next
            Next
        End If

        If Tool = enumTool.AutoCliff Then
            GL.LineWidth(2.0F)

            For Z = Clamp(AutoCliffBrushRadius.ZMin + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y To Clamp(AutoCliffBrushRadius.ZMax + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y
                Z2 = MouseOver_Tile.Y + Z
                For X = Clamp(AutoCliffBrushRadius.XMin(Z - AutoCliffBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X To Clamp(AutoCliffBrushRadius.XMax(Z - AutoCliffBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X
                    X2 = MouseOver_Tile.X + X

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Map.TerrainVertex(X2 + 1, Z2).Height * Map.HeightMultiplier
                    Vertex1.Z = -Z2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Map.TerrainVertex(X2, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex2.Z = -(Z2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Map.TerrainVertex(X2 + 1, Z2 + 1).Height * Map.HeightMultiplier
                    Vertex3.Z = -(Z2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()
                Next
            Next
        End If

        'draw mouseover vertex
        If Tool = enumTool.AutoTexture_Fill Then
            GL.LineWidth(2.0F)

            Vertex0.X = MouseOver_Vertex.X * TerrainGridSpacing
            Vertex0.Y = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height * Map.HeightMultiplier
            Vertex0.Z = -MouseOver_Vertex.Y * TerrainGridSpacing
            GL.Begin(BeginMode.Lines)
            GL.Color3(0.0F, 1.0F, 1.0F)
            GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
            GL.End()
        End If

        'draw mouseover vertices
        If Tool = enumTool.AutoTexture_Place Then
            GL.LineWidth(2.0F)

            For Z = Clamp(AutoTextureBrushRadius.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(AutoTextureBrushRadius.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
                Z2 = MouseOver_Vertex.Y + Z
                For X = Clamp(AutoTextureBrushRadius.XMin(Z - AutoTextureBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(AutoTextureBrushRadius.XMax(Z - AutoTextureBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                    X2 = MouseOver_Vertex.X + X

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
                    GL.End()
                Next
            Next
        End If

        If Tool = enumTool.Height_Set_Brush _
        Or Tool = enumTool.Height_Smooth_Brush _
        Or Tool = enumTool.Height_Change_Brush Then
            GL.LineWidth(2.0F)

            For Z = Clamp(HeightBrushRadius.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(HeightBrushRadius.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
                Z2 = MouseOver_Vertex.Y + Z
                For X = Clamp(HeightBrushRadius.XMin(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(HeightBrushRadius.XMax(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                    X2 = MouseOver_Vertex.X + X

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Map.TerrainVertex(X2, Z2).Height * Map.HeightMultiplier
                    Vertex0.Z = -Z2 * TerrainGridSpacing
                    GL.Begin(BeginMode.Lines)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X - 8.0#, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex0.X + 8.0#, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8.0#)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8.0#)
                    GL.End()
                Next
            Next
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

        ReDim UnitTextLabels(63)
        UnitTextLabelCount = 0

        If Draw_Units Then
            GL.Color3(1.0F, 1.0F, 1.0F)
            GL.Enable(EnableCap.Texture2D)
            Dim UnitDrawn(Map.UnitCount - 1) As Boolean
            X2 = DrawCentreSector.X 'Clamp(Int(View_Pos.X / (TerrainGridSpacing * Tiles_Per_Sector)), 0, Map.numSectors.X - 1)
            Z2 = DrawCentreSector.Y 'Clamp(Int(-View_Pos.Z / (TerrainGridSpacing * Tiles_Per_Sector)), 0, Map.numSectors.Y - 1)
            For Z = Clamp(Z2 + VisionSectors.ZMin, 0, Map.SectorCount.Y - 1) To Clamp(Z2 + VisionSectors.ZMax, 0, Map.SectorCount.Y - 1)
                For X = Clamp(X2 + VisionSectors.XMin(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1) To Clamp(X2 + VisionSectors.XMax(Z - Z2 - VisionSectors.ZMin), 0, Map.SectorCount.X - 1)
                    For A = 0 To Map.Sectors(X, Z).UnitCount - 1
                        tmpUnit = Map.Sectors(X, Z).Units(A)
                        If Not UnitDrawn(tmpUnit.Num) Then
                            UnitDrawn(tmpUnit.Num) = True
                            XYZ_dbl.X = tmpUnit.Pos.X - ViewPos.X
                            XYZ_dbl.Y = tmpUnit.Pos.Y - ViewPos.Y
                            XYZ_dbl.Z = -tmpUnit.Pos.Z - ViewPos.Z
                            Draw_Unit_Label = False
                            If tmpUnit.Type.LoadedInfo IsNot Nothing Then
                                GL.PushMatrix()
                                GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, -XYZ_dbl.Z)
                                tmpUnit.Type.LoadedInfo.GLDraw(tmpUnit.Rotation)
                                GL.PopMatrix()
                                If MouseOver_UnitCount > 0 Then
                                    If MouseOver_Units(0) Is tmpUnit Then
                                        Draw_Unit_Label = True
                                    End If
                                End If
                            Else
                                Draw_Unit_Label = True
                            End If
                            If Draw_Unit_Label And UnitTextLabelCount <= 63 Then
                                VectorRotationByMatrix(ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                                If Pos_Get_Screen_XY(XYZ_dbl2, ScreenX, ScreenY) Then
                                    If ScreenX >= 0 And ScreenX <= GLSize.X And ScreenY >= 0 And ScreenY <= GLSize.Y Then
                                        UnitTextLabels(UnitTextLabelCount) = New sTextLabel
                                        With UnitTextLabels(UnitTextLabelCount)
                                            .Font = UnitLabelFont
                                            .SizeY = UnitLabelFontSize
                                            .Colour.Red = 1.0F
                                            .Colour.Green = 1.0F
                                            .Colour.Blue = 1.0F
                                            .Colour.Alpha = 1.0F
                                            .Pos.X = ScreenX
                                            .Pos.Y = ScreenY
                                            If tmpUnit.Type.LoadedInfo IsNot Nothing Then
                                                .Text = tmpUnit.Type.Code & " (" & tmpUnit.Type.LoadedInfo.Name & ")"
                                            Else
                                                .Text = tmpUnit.Type.Code
                                            End If
                                        End With
                                        UnitTextLabelCount += 1
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next
            Next
            GL.Disable(EnableCap.Texture2D)
        End If

        ReDim Preserve UnitTextLabels(UnitTextLabelCount - 1)

        If MouseOver_Pos_Exists Then
            GL.Enable(EnableCap.Texture2D)
            If Tool = enumTool.Object_Unit Then
                If frmMainInstance.lstDroids.SelectedIndex >= 0 Then
                    NewUnitType = frmMainInstance.lstDroids_Unit(frmMainInstance.lstDroids.SelectedIndex)
                    XYZ_int = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                    GL.PushMatrix()
                    GL.Translate(XYZ_int.X - ViewPos.X, XYZ_int.Y - ViewPos.Y + 2.0#, ViewPos.Z + XYZ_int.Z)
                    NewUnitType.LoadedInfo.GLDraw(0.0F)
                    GL.PopMatrix()
                End If
            End If
            If Tool = enumTool.Object_Structure Then
                If frmMainInstance.lstStructures.SelectedIndex >= 0 Then
                    NewUnitType = frmMainInstance.lstStructures_Unit(frmMainInstance.lstStructures.SelectedIndex)
                    XYZ_int = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                    GL.PushMatrix()
                    GL.Translate(XYZ_int.X - ViewPos.X, XYZ_int.Y - ViewPos.Y + 2.0#, ViewPos.Z + XYZ_int.Z)
                    NewUnitType.LoadedInfo.GLDraw(0.0F)
                    GL.PopMatrix()
                End If
            End If
            If Tool = enumTool.Object_Feature Then
                If frmMainInstance.lstFeatures.SelectedIndex >= 0 Then
                    NewUnitType = frmMainInstance.lstFeatures_Unit(frmMainInstance.lstFeatures.SelectedIndex)
                    XYZ_int = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                    GL.PushMatrix()
                    GL.Translate(XYZ_int.X - ViewPos.X, XYZ_int.Y - ViewPos.Y + 2.0#, ViewPos.Z + XYZ_int.Z)
                    NewUnitType.LoadedInfo.GLDraw(0.0F)
                    GL.PopMatrix()
                End If
            End If
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)

        'draw unit selection

        GL.Begin(BeginMode.Quads)
        For A = 0 To Map.SelectedUnitCount - 1
            tmpUnit = Map.SelectedUnits(A)
            XYZ_dbl.X = tmpUnit.Pos.X - ViewPos.X
            XYZ_dbl.Y = tmpUnit.Pos.Y - ViewPos.Y
            XYZ_dbl.Z = -tmpUnit.Pos.Z - ViewPos.Z
            GL.Color4((1.0F + PlayerColour(tmpUnit.PlayerNum).Red) / 2.0F, (1.0F + PlayerColour(tmpUnit.PlayerNum).Green) / 2.0F, (1.0F + PlayerColour(tmpUnit.PlayerNum).Blue) / 2.0F, 0.875F)
            If tmpUnit.Type.LoadedInfo IsNot Nothing Then
                XY_int = tmpUnit.Type.LoadedInfo.Footprint
            Else
                XY_int.X = 1
                XY_int.Y = 1
            End If
            DrawUnitRectangle(XYZ_dbl, XY_int, 8.0#)
        Next
        For A = 0 To MouseOver_UnitCount - 1
            If MouseOver_Units(A) IsNot Nothing And Tool = enumTool.None Then
                tmpUnit = MouseOver_Units(A)
                XYZ_dbl.X = tmpUnit.Pos.X - ViewPos.X
                XYZ_dbl.Y = tmpUnit.Pos.Y - ViewPos.Y
                XYZ_dbl.Z = -tmpUnit.Pos.Z - ViewPos.Z
                GL.Color4((0.5F + PlayerColour(tmpUnit.PlayerNum).Red) / 1.5F, (0.5F + PlayerColour(tmpUnit.PlayerNum).Green) / 1.5F, (0.5F + PlayerColour(tmpUnit.PlayerNum).Blue) / 1.5F, 0.875F)
                If tmpUnit.Type.LoadedInfo IsNot Nothing Then
                    XY_int = tmpUnit.Type.LoadedInfo.Footprint
                Else
                    XY_int.X = 1
                    XY_int.Y = 1
                End If
                DrawUnitRectangle(XYZ_dbl, XY_int, 16.0#)
            End If
        Next
        GL.End()

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, CSng(GLSize.X), CSng(GLSize.Y), 0.0F, -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        GL.Enable(EnableCap.Texture2D)

        Draw_TextLabel(UnitTextLabels)

        Draw_TextLabel(SelectionLabel)

        GL.Disable(EnableCap.Texture2D)

        GL.Disable(EnableCap.Blend)

        'draw minimap

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, CSng(0.0F), CSng(GLSize.Y), -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        If Map.Minimap_Texture_Size > 0 And Map.Minimap_Texture > 0 Then
            dblTemp = Clamp(Val(frmMainInstance.menuMinimapSize.Text), 0.0#, 512.0#)
            Tiles_Per_Minimap_Pixel = Math.Sqrt(Map.TerrainSize.X * Map.TerrainSize.X + Map.TerrainSize.Y * Map.TerrainSize.Y) / (RootTwo * dblTemp)

            MinimapSize.X = Map.TerrainSize.X / Tiles_Per_Minimap_Pixel
            MinimapSize.Y = Map.TerrainSize.Y / Tiles_Per_Minimap_Pixel

            GL.Translate(0.0F, GLSize.Y - MinimapSize.Y, 0.0F)

            XYZ_dbl.X = Map.TerrainSize.X / Map.Minimap_Texture_Size
            XYZ_dbl.Z = Map.TerrainSize.Y / Map.Minimap_Texture_Size

            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Decal)
            GL.BindTexture(TextureTarget.Texture2D, Map.Minimap_Texture)

            GL.Begin(BeginMode.Quads)

            GL.TexCoord2(0.0F, 0.0F)
            GL.Vertex2(0.0F, MinimapSize.Y)

            GL.TexCoord2(XYZ_dbl.X, 0.0F)
            GL.Vertex2(MinimapSize.X, MinimapSize.Y)

            GL.TexCoord2(XYZ_dbl.X, XYZ_dbl.Z)
            GL.Vertex2(MinimapSize.X, 0.0F)

            GL.TexCoord2(0.0F, XYZ_dbl.Z)
            GL.Vertex2(0.0F, 0.0F)

            GL.End()

            GL.Disable(EnableCap.Texture2D)

            'draw minimap border

            GL.LineWidth(1.0F)
            GL.Begin(BeginMode.Lines)
            GL.Color3(0.75F, 0.75F, 0.75F)
            GL.Vertex2(MinimapSize.X, 0.0F)
            GL.Vertex2(MinimapSize.X, MinimapSize.Y)
            GL.Vertex2(0.0F, 0.0F)
            GL.Vertex2(MinimapSize.X, 0.0F)
            GL.End()

            'draw minimap view pos box

            If ShowMinimapViewPosBox Then
                'PosA.X = Clamp(ViewCorner0.X / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.X))
                'PosA.Y = Clamp(MinimapSize.Y + ViewCorner0.Y / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.Y))
                'PosB.X = Clamp(ViewCorner1.X / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.X))
                'PosB.Y = Clamp(MinimapSize.Y + ViewCorner1.Y / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.Y))
                'PosC.X = Clamp(ViewCorner2.X / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.X))
                'PosC.Y = Clamp(MinimapSize.Y + ViewCorner2.Y / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.Y))
                'PosD.X = Clamp(ViewCorner3.X / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.X))
                'PosD.Y = Clamp(MinimapSize.Y + ViewCorner3.Y / TerrainGridSpacing / Tiles_Per_Minimap_Pixel, 0.0#, CDbl(MinimapSize.Y))

                dblTemp = TerrainGridSpacing * Tiles_Per_Minimap_Pixel

                PosA.X = ViewCorner0.X / dblTemp
                PosA.Y = MinimapSize.Y + ViewCorner0.Y / dblTemp
                PosB.X = ViewCorner1.X / dblTemp
                PosB.Y = MinimapSize.Y + ViewCorner1.Y / dblTemp
                PosC.X = ViewCorner2.X / dblTemp
                PosC.Y = MinimapSize.Y + ViewCorner2.Y / dblTemp
                PosD.X = ViewCorner3.X / dblTemp
                PosD.Y = MinimapSize.Y + ViewCorner3.Y / dblTemp

                GL.LineWidth(1.0F)
                GL.Begin(BeginMode.LineLoop)
                GL.Color3(1.0F, 1.0F, 1.0F)
                GL.Vertex2(PosA.X, PosA.Y)
                GL.Vertex2(PosB.X, PosB.Y)
                GL.Vertex2(PosC.X, PosC.Y)
                GL.Vertex2(PosD.X, PosD.Y)
                GL.End()
            End If

            If Map.Selected_Area_VertexA_Exists Then
                DrawIt = False
                If Map.Selected_Area_VertexB_Exists Then
                    'area is selected
                    XY_Reorder(Map.Selected_Area_VertexA, Map.Selected_Area_VertexB, StartXY, FinishXY)
                    DrawIt = True
                ElseIf Tool = enumTool.Terrain_Select Then
                    If MouseOver_Pos_Exists Then
                        'selection is changing under mouse
                        XY_Reorder(Map.Selected_Area_VertexA, MouseOver_Vertex, StartXY, FinishXY)
                        DrawIt = True
                    End If
                End If
                If DrawIt Then
                    GL.LineWidth(1.0F)
                    PosA.X = StartXY.X / Tiles_Per_Minimap_Pixel
                    PosA.Y = MinimapSize.Y - StartXY.Y / Tiles_Per_Minimap_Pixel
                    PosB.X = FinishXY.X / Tiles_Per_Minimap_Pixel
                    PosB.Y = MinimapSize.Y - StartXY.Y / Tiles_Per_Minimap_Pixel
                    PosC.X = FinishXY.X / Tiles_Per_Minimap_Pixel
                    PosC.Y = MinimapSize.Y - FinishXY.Y / Tiles_Per_Minimap_Pixel
                    PosD.X = StartXY.X / Tiles_Per_Minimap_Pixel
                    PosD.Y = MinimapSize.Y - FinishXY.Y / Tiles_Per_Minimap_Pixel
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

    Sub ViewPosSet(ByVal NewViewPos As sXYZ_int)

        ViewPos.X = Clamp(NewViewPos.X, -Map.TerrainSize.X * TerrainGridSpacing - 2048, Map.TerrainSize.X * TerrainGridSpacing * 2 + 2048)
        ViewPos.Z = Clamp(NewViewPos.Z, -Map.TerrainSize.Y * TerrainGridSpacing * 2 - 2048, Map.TerrainSize.X * TerrainGridSpacing + 2048)
        ViewPos.Y = Clamp(NewViewPos.Y, CInt(Math.Ceiling(Map.GetTerrainHeight(ViewPos.X, -ViewPos.Z))) + 16, 49152)

        DrawViewLater()
    End Sub

    Sub ViewPosChange(ByVal Displacement As sXYZ_int)

        ViewPos.X = Clamp(ViewPos.X + Displacement.X, -Map.TerrainSize.X * TerrainGridSpacing - 2048, Map.TerrainSize.X * TerrainGridSpacing * 2 + 2048)
        ViewPos.Z = Clamp(ViewPos.Z + Displacement.Z, -Map.TerrainSize.Y * TerrainGridSpacing * 2 - 2048, Map.TerrainSize.X * TerrainGridSpacing + 2048)
        ViewPos.Y = Clamp(ViewPos.Y + Displacement.Y, CInt(Math.Ceiling(Map.GetTerrainHeight(ViewPos.X, -ViewPos.Z))) + 16, 49152)

        DrawViewLater()
    End Sub

    Sub ViewAngleSet(ByRef NewMatrix() As Double)

        MatrixCopy(NewMatrix, ViewAngleMatrix)
        MatrixNormalize(ViewAngleMatrix)
        MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)
        MatrixToRPY(ViewAngleMatrix, ViewAngleRPY)
    End Sub

    Sub ViewAngleSet_Rotate(ByRef NewMatrix() As Double)
        Dim Flag As Boolean
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_dbl2 As sXYZ_dbl
        'Dim XYZ_lng As sXYZ_lng
        Dim XY_dbl As sXY_dbl

        If ViewMoveType = enumView_Move_Type.RTS And RTSOrbit Then
            Flag = True
            'If ScreenXY_Get_TerrainPos(CInt(Int(GLSize.X / 2.0#)), CInt(Int(GLSize.Y / 2.0#)), XYZ_lng) Then
            '    XYZ_dbl.X = XYZ_lng.X
            '    XYZ_dbl.Y = XYZ_lng.Y
            '    XYZ_dbl.Z = XYZ_lng.Z
            'Else
            If ScreenXY_Get_PlanePos_ForwardDownOnly(CInt(Int(GLSize.X / 2.0#)), CInt(Int(GLSize.Y / 2.0#)), 127.5#, XY_dbl) Then
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

        MatrixToRPY(NewMatrix, ViewAngleRPY)
        If Flag Then
            If ViewAngleRPY.Pitch < RadOf1Deg * 10.0# Then
                ViewAngleRPY.Pitch = RadOf1Deg * 10.0#
            End If
        End If
        MatrixSetToRPY(ViewAngleMatrix, ViewAngleRPY)
        MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)

        If Flag Then
            XYZ_dbl2.X = ViewPos.X
            XYZ_dbl2.Y = ViewPos.Y
            XYZ_dbl2.Z = -ViewPos.Z
            MoveToViewTerrainPosFromDistance(XYZ_dbl, GetDist(XYZ_dbl, XYZ_dbl2))
        End If

        DrawViewLater()
    End Sub

    Sub View_Rotate(ByRef ChangeMatrix() As Double)
        Static matrixA(8) As Double

        MatrixRotationByMatrix(ViewAngleMatrix, ChangeMatrix, matrixA)
        MatrixCopy(matrixA, ViewAngleMatrix)
        MatrixInvert(ViewAngleMatrix, ViewAngleMatrix_Inverted)

        DrawViewLater()
    End Sub

    Sub OpenGL_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        MouseOver_Exists = True
        MouseOver_ScreenPos.X = e.X
        MouseOver_ScreenPos.Y = e.Y

        MouseOver_Pos_Calc()
    End Sub

    Sub MouseOver_Pos_Calc()
        Static XY_dbl As sXY_dbl
        Static A As Integer
        Static Flag As Boolean
        Static TerrainPos As sXYZ_lng

        MouseOverUnit_Clear()
        'commented to allow dragging beyond the borders of the view
        'If MouseOver_ScreenPos.X < 0 Or MouseOver_ScreenPos.Y < 0 Then
        '    MouseOver_Pos_Exists = False
        'Else
        If IsViewPosOverMinimap(MouseOver_ScreenPos) Then
            MouseOver_Pos_Exists = False

            If IsMinimap_MouseDown Then
                LookAtTile(Int(MouseOver_ScreenPos.X * Tiles_Per_Minimap_Pixel), Int(MouseOver_ScreenPos.Y * Tiles_Per_Minimap_Pixel))
            End If
        ElseIf IsMinimap_MouseDown Then
            MouseOver_Pos_Exists = False
        Else
            MouseOver_Pos_Exists = False
            Flag = False
            If DirectPointer Then
                If ScreenXY_Get_TerrainPos(MouseOver_ScreenPos.X, MouseOver_ScreenPos.Y, TerrainPos) Then
                    MouseOver_Pos.X = TerrainPos.X
                    MouseOver_Pos.Y = TerrainPos.Y
                    MouseOver_Pos.Z = -TerrainPos.Z
                    If MouseOver_Pos.X >= 0L And MouseOver_Pos.X < Map.TerrainSize.X * TerrainGridSpacing _
                    And -MouseOver_Pos.Z >= 0L And -MouseOver_Pos.Z < Map.TerrainSize.Y * TerrainGridSpacing Then
                        Flag = True
                    End If
                End If
            Else
                MouseOver_Pos.Y = 255.0# / 2.0# * Map.HeightMultiplier
                If ScreenXY_Get_PlanePos(MouseOver_ScreenPos.X, MouseOver_ScreenPos.Y, MouseOver_Pos.Y, XY_dbl) Then
                    MouseOver_Pos.X = XY_dbl.X
                    MouseOver_Pos.Z = XY_dbl.Y
                    If MouseOver_Pos.X >= 0L And MouseOver_Pos.X < Map.TerrainSize.X * TerrainGridSpacing _
                    And -MouseOver_Pos.Z >= 0L And -MouseOver_Pos.Z < Map.TerrainSize.Y * TerrainGridSpacing Then
                        MouseOver_Pos.Y = Map.GetTerrainHeight(MouseOver_Pos.X, -MouseOver_Pos.Z)
                        Flag = True
                    End If
                End If
            End If
            If Flag Then
                MouseOver_Pos_Exists = True
                MouseOverUnit_Clear()
                MouseOver_Tile.X = Math.Floor(MouseOver_Pos.X / TerrainGridSpacing)
                MouseOver_Tile.Y = Math.Floor(-MouseOver_Pos.Z / TerrainGridSpacing)
                MouseOver_Vertex.X = Math.Round(MouseOver_Pos.X / TerrainGridSpacing)
                MouseOver_Vertex.Y = Math.Round(-MouseOver_Pos.Z / TerrainGridSpacing)
                XY_dbl.X = MouseOver_Pos.X - MouseOver_Vertex.X * TerrainGridSpacing
                XY_dbl.Y = -MouseOver_Pos.Z - MouseOver_Vertex.Y * TerrainGridSpacing
                If Math.Abs(XY_dbl.Y) <= Math.Abs(XY_dbl.X) Then
                    MouseOver_Side_IsV = False
                    MouseOver_Side_Num.X = MouseOver_Tile.X
                    MouseOver_Side_Num.Y = MouseOver_Vertex.Y
                Else
                    MouseOver_Side_IsV = True
                    MouseOver_Side_Num.X = MouseOver_Vertex.X
                    MouseOver_Side_Num.Y = MouseOver_Tile.Y
                End If
                Dim SectorNum As sXY_int
                Map.Pos_Get_Sector(MouseOver_Pos.X, -MouseOver_Pos.Z, SectorNum)
                Dim tmpUnit As clsMap.clsUnit
                For A = 0 To Map.Sectors(SectorNum.X, SectorNum.Y).UnitCount - 1
                    tmpUnit = Map.Sectors(SectorNum.X, SectorNum.Y).Units(A)
                    XY_dbl.X = tmpUnit.Pos.X - MouseOver_Pos.X
                    XY_dbl.Y = tmpUnit.Pos.Z + MouseOver_Pos.Z
                    If tmpUnit.Type.LoadedInfo IsNot Nothing Then
                        If Math.Abs(XY_dbl.X) <= Math.Max(tmpUnit.Type.LoadedInfo.Footprint.X / 2.0#, 0.5#) * TerrainGridSpacing _
                        And Math.Abs(XY_dbl.Y) <= Math.Max(tmpUnit.Type.LoadedInfo.Footprint.Y / 2.0#, 0.5#) * TerrainGridSpacing Then
                            MouseOverUnit_Add(tmpUnit)
                        End If
                    Else
                        If Math.Abs(XY_dbl.X) <= TerrainGridSpacing / 2.0# _
                        And Math.Abs(XY_dbl.Y) <= TerrainGridSpacing / 2.0# Then
                            MouseOverUnit_Add(tmpUnit)
                        End If
                    End If
                Next
            End If

            If MouseOver_Pos_Exists Then
                If MouseLeftIsDown Then
                    If Tool = enumTool.AutoTexture_Place Then
                        Apply_Terrain()
                        If frmMainInstance.chkAutoTexSetHeight.Checked Then
                            Apply_Height_Set(AutoTextureBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                        End If
                    ElseIf Tool = enumTool.Height_Set_Brush Then
                        Apply_Height_Set(HeightBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                    ElseIf Tool = enumTool.Texture_Brush Then
                        Apply_Texture()
                    ElseIf Tool = enumTool.AutoCliff Then
                        Apply_Cliff()
                    ElseIf Tool = enumTool.AutoRoad_Place Then
                        Apply_Road()
                    End If
                End If
                If MouseRightIsDown Then
                    If Tool = enumTool.Height_Set_Brush Then
                        If Not MouseLeftIsDown Then
                            Apply_Height_Set(HeightBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                        End If
                    End If
                End If
            End If
        End If
        Pos_Display_Update()
        DrawViewLater()
    End Sub

    Sub Pos_Display_Update()

        If MouseOver_Pos_Exists Then
            lblTile.Text = "Tile x:" & MouseOver_Tile.X & ", z:" & MouseOver_Tile.Y
            lblVertex.Text = "Vertex  x:" & MouseOver_Vertex.X & ", z:" & MouseOver_Vertex.Y & ", alt:" & Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height * Map.HeightMultiplier & " (" & Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height & "x" & Map.HeightMultiplier & ")"
            lblPos.Text = "Pos x:" & MouseOver_Pos.X & ", z:" & -MouseOver_Pos.Z & ", alt:" & MouseOver_Pos.Y & ", slope: " & Math.Round(Map.GetTerrainSlopeAngle(MouseOver_Pos.X, -MouseOver_Pos.Z) / RadOf1Deg * 10.0#) / 10.0# & "°"
        Else
            lblTile.Text = ""
            lblVertex.Text = ""
            lblPos.Text = ""
        End If
    End Sub

    Sub OpenGL_LostFocus(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        Map.SuppressMinimap = False

        MouseOver_Exists = False
        MouseLeftIsDown = False
        MouseRightIsDown = False
        IsMinimap_MouseDown = False
        MouseOver_ScreenPos.X = -1
        MouseOver_ScreenPos.Y = -1
        MouseOver_Pos_Calc()

        ViewKeyDown_Clear()
    End Sub

    Sub OpenGL_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        MouseOver_Exists = False
        MouseOver_Pos_Exists = False
    End Sub

    Sub LookAtTile(ByVal X As Integer, ByVal Z As Integer)
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_int As sXYZ_int
        Dim dblTemp As Double
        Dim A As Integer
        Dim matrixA(8) As Double
        Dim AnglePY As sAnglePY

        VectorForwardRotationByMatrix(ViewAngleMatrix, XYZ_dbl)
        dblTemp = Map.GetTerrainHeight((X + 0.5#) * TerrainGridSpacing, (Z + 0.5#) * TerrainGridSpacing)
        A = Math.Ceiling(dblTemp) + 128
        If ViewPos.Y < A Then
            ViewPos.Y = A
        End If
        If XYZ_dbl.Y > -0.33333333333333331# Then
            XYZ_dbl.Y = -0.33333333333333331#
            GetAnglePY(XYZ_dbl, AnglePY)
            MatrixSetToPY(matrixA, AnglePY)
            ViewAngleSet(matrixA)
        End If
        dblTemp = (ViewPos.Y - dblTemp) / XYZ_dbl.Y

        XYZ_int.X = (X + 0.5#) * TerrainGridSpacing + dblTemp * XYZ_dbl.X
        XYZ_int.Y = ViewPos.Y
        XYZ_int.Z = -(Z + 0.5#) * TerrainGridSpacing + dblTemp * XYZ_dbl.Z

        ViewPosSet(XYZ_int)
    End Sub

    Public Sub MoveToViewTerrainPosFromDistance(ByVal TerrainPos As sXYZ_dbl, ByVal Distance As Double)
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_int As sXYZ_int

        VectorForwardRotationByMatrix(ViewAngleMatrix, XYZ_dbl)

        XYZ_int.X = TerrainPos.X - XYZ_dbl.X * Distance
        XYZ_int.Y = TerrainPos.Y - XYZ_dbl.Y * Distance
        XYZ_int.Z = -TerrainPos.Z - XYZ_dbl.Z * Distance

        ViewPosSet(XYZ_int)
    End Sub

    Sub Apply_Terrain()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer
        Dim AutoTextureChanged As clsMap.clsAutoTextureChange = Map.AutoTextureChange
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        For Z = Clamp(AutoTextureBrushRadius.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(AutoTextureBrushRadius.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
            Z2 = MouseOver_Vertex.Y + Z
            For X = Clamp(AutoTextureBrushRadius.XMin(Z - AutoTextureBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(AutoTextureBrushRadius.XMax(Z - AutoTextureBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                X2 = MouseOver_Vertex.X + X

                If Map.TerrainVertex(X2, Z2).Terrain IsNot SelectedTerrain Then

                    Map.TerrainVertex(X2, Z2).Terrain = SelectedTerrain

                    SectorChange.Vertex_Set_Changed(X2, Z2)
                    AutoTextureChanged.Vertex_Set_Changed(X2, Z2)
                End If
            Next
        Next

        AutoTextureChanged.Update_AutoTexture()
        SectorChange.Update_Graphics()

        DrawViewLater()
    End Sub

    Sub Apply_Road()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        If MouseOver_Side_IsV Then
            If Map.TerrainSideV(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Road IsNot SelectedRoad Then

                Map.TerrainSideV(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Road = SelectedRoad

                If MouseOver_Side_Num.X > 0 Then
                    Map.Tile_AutoTexture_Changed(MouseOver_Side_Num.X - 1, MouseOver_Side_Num.Y)
                    SectorChange.Tile_Set_Changed(MouseOver_Side_Num.X - 1, MouseOver_Side_Num.Y)
                End If
                If MouseOver_Side_Num.X < Map.TerrainSize.X Then
                    Map.Tile_AutoTexture_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y)
                    SectorChange.Tile_Set_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y)
                End If

                SectorChange.Update_Graphics()

                Map.UndoStepCreate("Road Side")

                DrawViewLater()
            End If
        Else
            If Map.TerrainSideH(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Road IsNot SelectedRoad Then

                Map.TerrainSideH(MouseOver_Side_Num.X, MouseOver_Side_Num.Y).Road = SelectedRoad

                If MouseOver_Side_Num.Y > 0 Then
                    Map.Tile_AutoTexture_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y - 1)
                    SectorChange.Tile_Set_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y - 1)
                End If
                If MouseOver_Side_Num.Y < Map.TerrainSize.X Then
                    Map.Tile_AutoTexture_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y)
                    SectorChange.Tile_Set_Changed(MouseOver_Side_Num.X, MouseOver_Side_Num.Y)
                End If

                SectorChange.Update_Graphics()

                Map.UndoStepCreate("Road Side")

                DrawViewLater()
            End If
        End If
    End Sub

    Sub Apply_Terrain_Fill()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim FillType As clsPainter.clsTerrain
        Dim ReplaceType As clsPainter.clsTerrain

        FillType = SelectedTerrain
        ReplaceType = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Terrain
        If FillType Is ReplaceType Then
            Exit Sub 'otherwise will cause endless loop
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim A As Integer
        Dim SourceOfFill(511) As sXY_int
        Dim SourceOfFillCount As Integer
        Dim SourceOfFill_Num As Integer
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange
        Dim AutoTextureChange As clsMap.clsAutoTextureChange = Map.AutoTextureChange

        SourceOfFill(0).X = MouseOver_Vertex.X
        SourceOfFill(0).Y = MouseOver_Vertex.Y
        SourceOfFillCount = 1
        SourceOfFill_Num = 0

        X = SourceOfFill(SourceOfFill_Num).X
        Z = SourceOfFill(SourceOfFill_Num).Y
        Map.TerrainVertex(X, Z).Terrain = FillType
        SectorChange.Vertex_Set_Changed(X, Z)
        AutoTextureChange.Vertex_Set_Changed(X, Z)
        Do While SourceOfFill_Num < SourceOfFillCount
            X = SourceOfFill(SourceOfFill_Num).X + 1
            Z = SourceOfFill(SourceOfFill_Num).Y
            If X >= 0 And X <= Map.TerrainSize.X _
            And Z >= 0 And Z <= Map.TerrainSize.Y Then
                If Map.TerrainVertex(X, Z).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount + 512)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Z
                    SourceOfFillCount += 1

                    Map.TerrainVertex(X, Z).Terrain = FillType
                    SectorChange.Vertex_Set_Changed(X, Z)
                    AutoTextureChange.Vertex_Set_Changed(X, Z)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X - 1
            Z = SourceOfFill(SourceOfFill_Num).Y
            If X >= 0 And X <= Map.TerrainSize.X _
            And Z >= 0 And Z <= Map.TerrainSize.Y Then
                If Map.TerrainVertex(X, Z).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount + 512)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Z
                    SourceOfFillCount += 1

                    Map.TerrainVertex(X, Z).Terrain = FillType
                    SectorChange.Vertex_Set_Changed(X, Z)
                    AutoTextureChange.Vertex_Set_Changed(X, Z)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X
            Z = SourceOfFill(SourceOfFill_Num).Y + 1
            If X >= 0 And X <= Map.TerrainSize.X _
            And Z >= 0 And Z <= Map.TerrainSize.Y Then
                If Map.TerrainVertex(X, Z).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount + 512)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Z
                    SourceOfFillCount += 1

                    Map.TerrainVertex(X, Z).Terrain = FillType
                    SectorChange.Vertex_Set_Changed(X, Z)
                    AutoTextureChange.Vertex_Set_Changed(X, Z)
                End If
            End If

            X = SourceOfFill(SourceOfFill_Num).X
            Z = SourceOfFill(SourceOfFill_Num).Y - 1
            If X >= 0 And X <= Map.TerrainSize.X _
            And Z >= 0 And Z <= Map.TerrainSize.Y Then
                If Map.TerrainVertex(X, Z).Terrain Is ReplaceType Then
                    If SourceOfFill.GetUpperBound(0) < SourceOfFillCount Then
                        ReDim Preserve SourceOfFill(SourceOfFillCount + 512)
                    End If
                    SourceOfFill(SourceOfFillCount).X = X
                    SourceOfFill(SourceOfFillCount).Y = Z
                    SourceOfFillCount += 1

                    Map.TerrainVertex(X, Z).Terrain = FillType
                    SectorChange.Vertex_Set_Changed(X, Z)
                    AutoTextureChange.Vertex_Set_Changed(X, Z)
                End If
            End If

            SourceOfFill_Num += 1

            If SourceOfFill_Num > 131072 Then
                SourceOfFillCount -= SourceOfFill_Num
                For A = 0 To SourceOfFillCount - 1
                    SourceOfFill(A) = SourceOfFill(SourceOfFill_Num + A)
                Next
                ReDim Preserve SourceOfFill(SourceOfFillCount + 512)
                SourceOfFill_Num = 0
            End If
        Loop

        AutoTextureChange.Update_AutoTexture()
        SectorChange.Update_Graphics()

        Map.UndoStepCreate("Ground Fill")

        DrawViewLater()
    End Sub

    Sub Apply_Texture()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        For Z = Clamp(TextureBrushRadius.ZMin + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y To Clamp(TextureBrushRadius.ZMax + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y
            Z2 = MouseOver_Tile.Y + Z
            For X = Clamp(TextureBrushRadius.XMin(Z - TextureBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X To Clamp(TextureBrushRadius.XMax(Z - TextureBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X
                X2 = MouseOver_Tile.X + X

                'Map.TerrainVertex(X2, Z2).Terrain = Nothing
                'Map.TerrainVertex(X2 + 1, Z2).Terrain = Nothing
                'Map.TerrainVertex(X2, Z2 + 1).Terrain = Nothing
                'Map.TerrainVertex(X2 + 1, Z2 + 1).Terrain = Nothing

                If frmMainInstance.chkSetTexture.Checked Then
                    Map.TerrainTiles(X2, Z2).Texture.TextureNum = SelectedTexture
                End If
                If frmMainInstance.chkSetTextureOrientation.Checked Then
                    If frmMainInstance.chkTextureOrientationRandomize.Checked Then
                        Map.TerrainTiles(X2, Z2).Texture.Orientation = New sTileOrientation(Rnd() >= 0.5F, Rnd() >= 0.5F, Rnd() >= 0.5F)
                    Else
                        Map.TerrainTiles(X2, Z2).Texture.Orientation = TextureOrientation
                    End If
                End If

                SectorChange.Tile_Set_Changed(X2, Z2)
            Next
        Next

        SectorChange.Update_Graphics()

        DrawViewLater()
    End Sub

    Sub Apply_Cliff()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        If frmMainInstance.rdoAutoCliffRemove.Checked Then
            For Z = Clamp(AutoCliffBrushRadius.ZMin + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y To Clamp(AutoCliffBrushRadius.ZMax + MouseOver_Tile.Y, 0, Map.TerrainSize.Y - 1) - MouseOver_Tile.Y
                Z2 = MouseOver_Tile.Y + Z
                For X = Clamp(AutoCliffBrushRadius.XMin(Z - AutoCliffBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X To Clamp(AutoCliffBrushRadius.XMax(Z - AutoCliffBrushRadius.ZMin) + MouseOver_Tile.X, 0, Map.TerrainSize.X - 1) - MouseOver_Tile.X
                    X2 = MouseOver_Tile.X + X

                    If Map.TerrainTiles(X2, Z2).Terrain_IsCliff Or Map.TerrainTiles(X2, Z2).TriBottomLeftIsCliff Or Map.TerrainTiles(X2, Z2).TriBottomRightIsCliff Or Map.TerrainTiles(X2, Z2).TriTopLeftIsCliff Or Map.TerrainTiles(X2, Z2).TriTopRightIsCliff Then

                        Map.TerrainTiles(X2, Z2).Terrain_IsCliff = False
                        Map.TerrainTiles(X2, Z2).TriBottomLeftIsCliff = False
                        Map.TerrainTiles(X2, Z2).TriBottomRightIsCliff = False
                        Map.TerrainTiles(X2, Z2).TriTopLeftIsCliff = False
                        Map.TerrainTiles(X2, Z2).TriTopRightIsCliff = False

                        Map.Tile_AutoTexture_Changed(X2, Z2)

                        SectorChange.Tile_Set_Changed(X2, Z2)
                    End If
                Next
            Next

            SectorChange.Update_Graphics()

            DrawViewLater()
        ElseIf frmMainInstance.rdoAutoCliffBrush.Checked Then
            Dim MinCliffAngle As Double = Clamp(Val(frmMainInstance.txtAutoCliffSlope.Text) * RadOf1Deg, 0.0#, RadOf90Deg)
            Map.Apply_Cliff(MouseOver_Tile, AutoCliffBrushRadius, MinCliffAngle, frmMainInstance.chkCliffTris.Checked)
            Map.SectorChange.Update_Graphics()
            DrawViewLater()
        End If
    End Sub

    Sub Apply_Texture_Clockwise()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.Orientation.RotateClockwise()

        SectorChange.Tile_Set_Changed(MouseOver_Tile.X, MouseOver_Tile.Y)

        SectorChange.Update_Graphics()

        Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Sub Apply_Texture_Anticlockwise()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.Orientation.RotateAnticlockwise()

        SectorChange.Tile_Set_Changed(MouseOver_Tile.X, MouseOver_Tile.Y)

        SectorChange.Update_Graphics()

        Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Sub Apply_Texture_FlipX()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.Orientation.ResultXFlip = Not Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.Orientation.ResultXFlip

        SectorChange.Tile_Set_Changed(MouseOver_Tile.X, MouseOver_Tile.Y)

        SectorChange.Update_Graphics()

        Map.UndoStepCreate("Texture Rotate")

        DrawViewLater()
    End Sub

    Sub Apply_Tri_Flip()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Tri = Not Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Tri

        'to update any cliffs
        Map.Tile_AutoTexture_Changed(MouseOver_Tile.X, MouseOver_Tile.Y)

        SectorChange.Tile_Set_Changed(MouseOver_Tile.X, MouseOver_Tile.Y)

        SectorChange.Update_Graphics()

        Map.UndoStepCreate("Triangle Flip")

        DrawViewLater()
    End Sub

    Sub Apply_HeightSmoothing(ByVal Ratio As Double)
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer
        Dim X3 As Integer
        Dim Z3 As Integer
        Dim X4 As Integer
        Dim Z4 As Integer

        Dim TempHeight As Integer
        Dim Samples As Integer

        Dim NewHeight(,) As Byte
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        ReDim NewHeight(Map.TerrainSize.X, Map.TerrainSize.Y)

        For Z = 0 To Map.TerrainSize.Y
            For X = 0 To Map.TerrainSize.X
                NewHeight(X, Z) = Map.TerrainVertex(X, Z).Height
            Next
        Next

        For Z = Clamp(HeightBrushRadius.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(HeightBrushRadius.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
            Z2 = MouseOver_Vertex.Y + Z
            For X = Clamp(HeightBrushRadius.XMin(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(HeightBrushRadius.XMax(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                X2 = MouseOver_Vertex.X + X

                TempHeight = 0
                Samples = 0

                For Z3 = Clamp(SmoothRadius.ZMin + Z2, 0, Map.TerrainSize.Y) - Z2 To Clamp(SmoothRadius.ZMax + Z2, 0, Map.TerrainSize.Y) - Z2
                    Z4 = Z2 + Z3
                    For X3 = Clamp(SmoothRadius.XMin(Z3 - SmoothRadius.ZMin) + X2, 0, Map.TerrainSize.X) - X2 To Clamp(SmoothRadius.XMax(Z3 - SmoothRadius.ZMin) + X2, 0, Map.TerrainSize.X) - X2
                        X4 = X2 + X3
                        TempHeight = TempHeight + Map.TerrainVertex(X4, Z4).Height
                        Samples = Samples + 1
                    Next
                Next

                NewHeight(X2, Z2) = Map.TerrainVertex(X2, Z2).Height * (1.0# - Ratio) + TempHeight / Samples * Ratio

                SectorChange.Vertex_And_Normals_Changed(X2, Z2)
            Next
        Next

        For Z = 0 To Map.TerrainSize.Y
            For X = 0 To Map.TerrainSize.X
                Map.TerrainVertex(X, Z).Height = NewHeight(X, Z)
            Next
        Next

        SectorChange.Update_Graphics_And_UnitHeights()

        DrawViewLater()
    End Sub

    Sub Apply_Height_Change(ByVal Rate As Double)
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        Dim intRate As Integer

        intRate = Math.Round(Rate)

        For Z = Clamp(HeightBrushRadius.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(HeightBrushRadius.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
            Z2 = MouseOver_Vertex.Y + Z
            For X = Clamp(HeightBrushRadius.XMin(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(HeightBrushRadius.XMax(Z - HeightBrushRadius.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                X2 = MouseOver_Vertex.X + X

                Map.TerrainVertex(X2, Z2).Height = Clamp(CInt(Map.TerrainVertex(X2, Z2).Height) + intRate, 0, 255)

                SectorChange.Vertex_And_Normals_Changed(X2, Z2)
            Next
        Next

        SectorChange.Update_Graphics_And_UnitHeights()

        DrawViewLater()
    End Sub

    Sub Apply_Height_Set(ByRef CircleTiles As sBrushTiles, ByVal HeightNew As Byte)
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Z As Integer
        Dim X2 As Integer
        Dim Z2 As Integer

        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange

        For Z = Clamp(CircleTiles.ZMin + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y To Clamp(CircleTiles.ZMax + MouseOver_Vertex.Y, 0, Map.TerrainSize.Y) - MouseOver_Vertex.Y
            Z2 = MouseOver_Vertex.Y + Z
            For X = Clamp(CircleTiles.XMin(Z - CircleTiles.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X To Clamp(CircleTiles.XMax(Z - CircleTiles.ZMin) + MouseOver_Vertex.X, 0, Map.TerrainSize.X) - MouseOver_Vertex.X
                X2 = MouseOver_Vertex.X + X

                If Map.TerrainVertex(X2, Z2).Height <> HeightNew Then

                    Map.TerrainVertex(X2, Z2).Height = HeightNew

                    SectorChange.Vertex_And_Normals_Changed(X2, Z2)
                End If
            Next
        Next

        SectorChange.Update_Graphics_And_UnitHeights()

        DrawViewLater()
    End Sub

    Private WithEvents ListSelect As ContextMenuStrip
    Private ListSelectIsPicker As Boolean
    Private ListSelectItems(-1) As ToolStripItem

    Private Sub ListSelect_Click(ByVal Sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles ListSelect.ItemClicked
        Dim tmpButton As ToolStripItem = e.ClickedItem
        Dim tmpUnit As clsMap.clsUnit = tmpButton.Tag

        If ListSelectIsPicker Then
            ObjectPicker(tmpUnit.Type)
        Else
            If tmpUnit.SelectedUnitNum < 0 Then
                Map.SelectedUnit_Add(tmpUnit)
            Else
                Map.SelectedUnit_Remove(tmpUnit.SelectedUnitNum)
            End If
            frmMainInstance.Selected_Object_Changed()
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

    Sub OpenGL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Static XY_int As sXY_int
        Static NewUnitType As clsUnitType
        Static NewUnit As clsMap.clsUnit
        Static A As Integer

        Map.SuppressMinimap = True

        XY_int.X = e.X
        XY_int.Y = e.Y
        If e.Button = Windows.Forms.MouseButtons.Left Then
            MouseLeftIsDown = True

            If IsViewPosOverMinimap(XY_int) Then
                IsMinimap_MouseDown = True
                LookAtTile(Int(XY_int.X * Tiles_Per_Minimap_Pixel), Int(XY_int.Y * Tiles_Per_Minimap_Pixel))
            Else
                If MouseOver_Pos_Exists Then
                    If Tool = enumTool.None Then
                        If Control_Picker.Active Then
                            If MouseOver_UnitCount > 0 Then
                                If MouseOver_UnitCount = 1 Then
                                    ObjectPicker(MouseOver_Units(0).Type)
                                Else
                                    ListSelect.Close()
                                    ListSelect.Items.Clear()
                                    ReDim ListSelectItems(MouseOver_UnitCount - 1)
                                    For A = 0 To MouseOver_UnitCount - 1
                                        If MouseOver_Units(A).Type Is Nothing Then
                                            ListSelectItems(A) = New ToolStripButton("<nothing>")
                                        Else
                                            ListSelectItems(A) = New ToolStripButton(MouseOver_Units(A).Type.Code)
                                        End If
                                        ListSelectItems(A).Tag = MouseOver_Units(A)
                                        ListSelect.Items.Add(ListSelectItems(A))
                                    Next
                                    ListSelectIsPicker = True
                                    ListSelect.Show(Me, New Drawing.Point(MouseOver_ScreenPos.X, MouseOver_ScreenPos.Y))
                                End If
                            End If
                        Else
                            If Not Control_Unit_Multiselect.Active Then
                                Map.SelectedUnits_Clear()
                            End If
                            frmMainInstance.Selected_Object_Changed()
                            Map.Unit_Selected_Area_VertexA = MouseOver_Vertex
                            Map.Unit_Selected_Area_VertexA_Exists = True
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.AutoTexture_Place Then
                        If Map.Tileset IsNot Nothing Then
                            If Control_Picker.Active Then
                                TerrainPicker()
                            Else
                                Apply_Terrain()
                                If frmMainInstance.chkAutoTexSetHeight.Checked Then
                                    Apply_Height_Set(AutoTextureBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                                End If
                            End If
                        End If
                    ElseIf Tool = enumTool.Height_Set_Brush Then
                        If Control_Picker.Active Then
                            HeightPickerL()
                        Else
                            Apply_Height_Set(HeightBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetL.SelectedIndex))
                        End If
                    ElseIf Tool = enumTool.Texture_Brush Then
                        If Map.Tileset IsNot Nothing Then
                            If Control_Picker.Active Then
                                TexturePicker()
                            Else
                                Apply_Texture()
                            End If
                        End If
                    ElseIf Tool = enumTool.AutoCliff Then
                        Apply_Cliff()
                        DrawViewLater()
                    ElseIf Tool = enumTool.AutoTexture_Fill Then
                        If Map.Tileset IsNot Nothing Then
                            If Control_Picker.Active Then
                                TerrainPicker()
                            Else
                                Apply_Terrain_Fill()
                                DrawViewLater()
                            End If
                        End If
                    ElseIf Tool = enumTool.AutoRoad_Place Then
                        If Map.Tileset IsNot Nothing Then
                            Apply_Road()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.AutoRoad_Line Then
                        If Map.Tileset IsNot Nothing Then
                            Apply_Road_Line_Selection()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.Object_Unit Then
                        If frmMainInstance.lstDroids.SelectedIndex >= 0 Then
                            NewUnitType = frmMainInstance.lstDroids_Unit(frmMainInstance.lstDroids.SelectedIndex)
                            NewUnit = New clsMap.clsUnit
                            NewUnit.PlayerNum = frmMainInstance.NewPlayerNum.SelectedPlayerNum
                            NewUnit.Pos = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                            NewUnit.Rotation = 0
                            NewUnit.Type = NewUnitType
                            Map.Unit_Add_StoreChange(NewUnit)
                            Map.UndoStepCreate("Place Droid")
                            Map.SectorChange.Update_Graphics()
                            Map.MinimapMakeLater()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.Object_Structure Then
                        If frmMainInstance.lstStructures.SelectedIndex >= 0 Then
                            NewUnitType = frmMainInstance.lstStructures_Unit(frmMainInstance.lstStructures.SelectedIndex)
                            NewUnit = New clsMap.clsUnit
                            NewUnit.PlayerNum = frmMainInstance.NewPlayerNum.SelectedPlayerNum
                            NewUnit.Pos = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                            NewUnit.Rotation = 0
                            NewUnit.Type = NewUnitType
                            Map.Unit_Add_StoreChange(NewUnit)
                            Map.UndoStepCreate("Place Structure")
                            Map.SectorChange.Update_Graphics()
                            Map.MinimapMakeLater()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.Object_Feature Then
                        If frmMainInstance.lstFeatures.SelectedIndex >= 0 Then
                            NewUnitType = frmMainInstance.lstFeatures_Unit(frmMainInstance.lstFeatures.SelectedIndex)
                            NewUnit = New clsMap.clsUnit
                            NewUnit.PlayerNum = frmMainInstance.NewPlayerNum.SelectedPlayerNum
                            NewUnit.Pos = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X, -MouseOver_Pos.Z, NewUnitType.LoadedInfo.Footprint)
                            NewUnit.Rotation = 0
                            NewUnit.Type = NewUnitType
                            Map.Unit_Add_StoreChange(NewUnit)
                            Map.UndoStepCreate("Place Feature")
                            Map.SectorChange.Update_Graphics()
                            Map.MinimapMakeLater()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.Terrain_Select Then
                        If Not Map.Selected_Area_VertexA_Exists Then
                            Map.Selected_Area_VertexA = MouseOver_Vertex
                            Map.Selected_Area_VertexA_Exists = True
                            DrawViewLater()
                        ElseIf Not Map.Selected_Area_VertexB_Exists Then
                            Map.Selected_Area_VertexB = MouseOver_Vertex
                            Map.Selected_Area_VertexB_Exists = True
                            DrawViewLater()
                            frmMainInstance.Terrain_Selection_Changed()
                        Else
                            Map.Selected_Area_VertexA_Exists = False
                            Map.Selected_Area_VertexB_Exists = False
                            frmMainInstance.Terrain_Selection_Changed()
                            DrawViewLater()
                        End If
                    ElseIf Tool = enumTool.Gateways Then
                        Apply_Gateway()
                        DrawViewLater()
                    End If
                ElseIf Tool = enumTool.None Then
                    Map.SelectedUnits_Clear()
                    frmMainInstance.Selected_Object_Changed()
                End If
            End If
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            MouseRightIsDown = True

            Select Case Tool
                Case enumTool.AutoRoad_Line
                    Map.Selected_Tile_A_Exists = False
                    DrawViewLater()
                Case enumTool.Terrain_Select
                    Map.Selected_Area_VertexA_Exists = False
                    Map.Selected_Area_VertexB_Exists = False
                    frmMainInstance.Terrain_Selection_Changed()
                    DrawViewLater()
                Case enumTool.Gateways
                    Map.Selected_Tile_A_Exists = False
                    Map.Selected_Tile_B_Exists = False
                    DrawViewLater()
                Case enumTool.Height_Set_Brush
                    If Control_Picker.Active Then
                        HeightPickerR()
                    Else
                        Apply_Height_Set(HeightBrushRadius, frmMainInstance.HeightSetPalette(frmMainInstance.tabHeightSetR.SelectedIndex))
                    End If
            End Select
        End If
    End Sub

    Public Sub OpenGL_KeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
        Static matrixA(8) As Double
        Static A As Integer

        IsViewKeyDown(e.KeyCode) = True

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next

        If e.KeyCode = Keys.F12 Then InputBox("", "", My.Application.Info.DirectoryPath)
        If Control_Undo.Active Then
            If Map.Undo_Pos > 0 Then
                DisplayUndoMessage("Undid: " & Map.Undos(Map.Undo_Pos - 1).Name)
                Map.Undo_Perform()
                DrawViewLater()
            Else
                DisplayUndoMessage("Nothing to undo")
            End If
        End If
        If Control_Redo.Active Then
            If Map.Undo_Pos < Map.UndoCount Then
                DisplayUndoMessage("Redid: " & Map.Undos(Map.Undo_Pos).Name)
                Map.Redo_Perform()
                DrawViewLater()
            Else
                DisplayUndoMessage("Nothing to redo")
            End If
        End If
        If IsViewKeyDown(Keys.ControlKey) Then
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
            FOV_Multiplier_Set(FOVDefault)
            If ViewMoveType = ctrlMapView.enumView_Move_Type.Free Then
                MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                ViewAngleSet_Rotate(matrixA)
            ElseIf ViewMoveType = ctrlMapView.enumView_Move_Type.RTS Then
                MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
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
            Map.SectorAll_GL_Update()
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
            If Control_Clockwise.Active Then
                If MouseOver_Pos_Exists Then
                    Apply_Texture_Clockwise()
                End If
            End If
            If Control_Anticlockwise.Active Then
                If MouseOver_Pos_Exists Then
                    Apply_Texture_Anticlockwise()
                End If
            End If
            If Control_Texture_Flip.Active Then
                If MouseOver_Pos_Exists Then
                    Apply_Texture_FlipX()
                End If
            End If
            If Control_Tri_Flip.Active Then
                If MouseOver_Pos_Exists Then
                    Apply_Tri_Flip()
                End If
            End If
        End If
        If Tool = enumTool.None Then
            If Control_Unit_Delete.Active Then
                If Map.SelectedUnitCount > 0 Then
                    Dim OldUnits() As clsMap.clsUnit = Map.SelectedUnits.Clone
                    For A = 0 To OldUnits.GetUpperBound(0)
                        Map.Unit_Remove_StoreChange(OldUnits(A).Num)
                    Next
                    Map.SelectedUnits_Clear()
                    frmMainInstance.Selected_Object_Changed()
                    Map.UndoStepCreate("Object Deleted")
                    Map.SectorChange.Update_Graphics()
                    Map.MinimapMakeLater()
                    DrawViewLater()
                End If
            End If
            If Control_Unit_Move.Active Then
                If MouseOver_Pos_Exists Then
                    If Map.SelectedUnitCount > 0 Then
                        Dim Centre As sXY_dbl
                        For A = 0 To Map.SelectedUnitCount - 1
                            Centre.X += Map.SelectedUnits(A).Pos.X
                            Centre.Y += Map.SelectedUnits(A).Pos.Z
                        Next
                        Centre.X /= Map.SelectedUnitCount
                        Centre.Y /= Map.SelectedUnitCount
                        Dim UnitsToMove(Map.SelectedUnitCount - 1) As clsMap.clsUnit
                        Dim UnitsToMoveID(Map.SelectedUnitCount - 1) As UInteger
                        For A = 0 To Map.SelectedUnitCount - 1
                            UnitsToMove(A) = Map.SelectedUnits(A)
                            UnitsToMoveID(A) = UnitsToMove(A).ID
                        Next
                        Dim NewUnits(Map.SelectedUnitCount - 1) As clsMap.clsUnit
                        Dim tmpUnit As clsMap.clsUnit
                        For A = 0 To Map.SelectedUnitCount - 1
                            tmpUnit = New clsMap.clsUnit(UnitsToMove(A))
                            NewUnits(A) = tmpUnit
                            If tmpUnit.Type.LoadedInfo Is Nothing Then
                                tmpUnit.Pos = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X + UnitsToMove(A).Pos.X - Centre.X, -MouseOver_Pos.Z + UnitsToMove(A).Pos.Z - Centre.Y, New sXY_int(1, 1))
                            Else
                                tmpUnit.Pos = Map.TileAligned_Pos_From_MapPos(MouseOver_Pos.X + UnitsToMove(A).Pos.X - Centre.X, -MouseOver_Pos.Z + UnitsToMove(A).Pos.Z - Centre.Y, tmpUnit.Type.LoadedInfo.Footprint)
                            End If
                            Map.Unit_Remove_StoreChange(UnitsToMove(A).Num)
                            Map.Unit_Add_StoreChange(tmpUnit, UnitsToMoveID(A))
                        Next
                        Map.SelectedUnit_Add(NewUnits)
                        Map.UndoStepCreate("Objects Moved")
                        Map.SectorChange.Update_Graphics()
                        Map.MinimapMakeLater()
                        frmMainInstance.Selected_Object_Changed()
                        DrawViewLater()
                    End If
                End If
            End If
            If Control_Clockwise.Active Then
                If Map.SelectedUnitCount = 1 Then
                    Dim NewUnit As New clsMap.clsUnit(Map.SelectedUnits(0))
                    Dim ID As UInteger = Map.SelectedUnits(0).ID
                    Map.Unit_Remove_StoreChange(Map.SelectedUnits(0).Num)
                    NewUnit.Rotation -= 90
                    If NewUnit.Rotation < 0 Then
                        NewUnit.Rotation += 360
                    End If
                    Map.Unit_Add_StoreChange(NewUnit, ID)
                    Map.SelectedUnit_Add(NewUnit)
                    frmMainInstance.Selected_Object_Changed()
                    Map.UndoStepCreate("Object Rotated")
                    DrawViewLater()
                End If
            End If
            If Control_Anticlockwise.Active Then
                If Map.SelectedUnitCount = 1 Then
                    Dim NewUnit As New clsMap.clsUnit(Map.SelectedUnits(0))
                    Dim ID As UInteger = Map.SelectedUnits(0).ID
                    Map.Unit_Remove_StoreChange(Map.SelectedUnits(0).Num)
                    NewUnit.Rotation += 90
                    If NewUnit.Rotation > 359 Then
                        NewUnit.Rotation -= 360
                    End If
                    Map.Unit_Add_StoreChange(NewUnit, ID)
                    Map.SelectedUnit_Add(NewUnit)
                    frmMainInstance.Selected_Object_Changed()
                    Map.UndoStepCreate("Object Rotated")
                    DrawViewLater()
                End If
            End If
        End If

        If Control_Deselect.Active Then
            Tool = enumTool.None
            DrawViewLater()
        End If
    End Sub

    Public Sub OpenGL_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        Dim A As Integer

        IsViewKeyDown(e.KeyCode) = False

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Sub Apply_Road_Line_Selection()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        Dim Num As Integer
        Dim A As Integer
        Dim B As Integer
        Dim SectorChange As clsMap.clsSectorChange = Map.SectorChange
        Dim AutoTextureChange As clsMap.clsAutoTextureChange = Map.AutoTextureChange

        If Map.Selected_Tile_A_Exists Then
            If MouseOver_Tile.X = Map.Selected_Tile_A.X Then
                If MouseOver_Tile.Y <= Map.Selected_Tile_A.Y Then
                    A = MouseOver_Tile.Y
                    B = Map.Selected_Tile_A.Y
                Else
                    A = Map.Selected_Tile_A.Y
                    B = MouseOver_Tile.Y
                End If
                For Num = A + 1 To B
                    Map.TerrainSideH(Map.Selected_Tile_A.X, Num).Road = SelectedRoad
                    AutoTextureChange.SideH_Set_Changed(Map.Selected_Tile_A.X, Num)
                    SectorChange.SideH_Set_Changed(Map.Selected_Tile_A.X, Num)
                Next

                AutoTextureChange.Update_AutoTexture()
                SectorChange.Update_Graphics()

                Map.UndoStepCreate("Road Line")

                Map.Selected_Tile_A_Exists = False
                DrawViewLater()
            ElseIf MouseOver_Tile.Y = Map.Selected_Tile_A.Y Then
                If MouseOver_Tile.X <= Map.Selected_Tile_A.X Then
                    A = MouseOver_Tile.X
                    B = Map.Selected_Tile_A.X
                Else
                    A = Map.Selected_Tile_A.X
                    B = MouseOver_Tile.X
                End If
                For Num = A + 1 To B
                    Map.TerrainSideV(Num, Map.Selected_Tile_A.Y).Road = SelectedRoad
                    AutoTextureChange.SideV_Set_Changed(Num, Map.Selected_Tile_A.Y)
                    SectorChange.SideV_Set_Changed(Num, Map.Selected_Tile_A.Y)
                Next

                AutoTextureChange.Update_AutoTexture()
                SectorChange.Update_Graphics()

                Map.UndoStepCreate("Road Line")

                Map.Selected_Tile_A_Exists = False
                DrawViewLater()
            Else

            End If
        Else
            Map.Selected_Tile_A.X = MouseOver_Tile.X
            Map.Selected_Tile_A.Y = MouseOver_Tile.Y
            Map.Selected_Tile_A_Exists = True
        End If
    End Sub

    Function IsViewPosOverMinimap(ByVal Pos As sXY_int) As Boolean

        If Pos.X >= 0 And Pos.X < Map.TerrainSize.X / Tiles_Per_Minimap_Pixel _
            And Pos.Y >= 0 And Pos.Y < Map.TerrainSize.Y / Tiles_Per_Minimap_Pixel Then
            IsViewPosOverMinimap = True
        Else
            IsViewPosOverMinimap = False
        End If
    End Function

    Sub OpenGL_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Map.SuppressMinimap = False

        If e.Button = Windows.Forms.MouseButtons.Left Then
            MouseLeftIsDown = False

            If IsMinimap_MouseDown Then
                IsMinimap_MouseDown = False
            Else
                Select Case Tool
                    Case enumTool.AutoTexture_Place
                        Map.UndoStepCreate("Ground Painted")
                    Case enumTool.AutoCliff
                        Map.UndoStepCreate("Cliff Brush")
                    Case enumTool.Height_Change_Brush
                        Map.UndoStepCreate("Height Change")
                    Case enumTool.Height_Set_Brush
                        Map.UndoStepCreate("Height Set")
                    Case enumTool.Height_Smooth_Brush
                        Map.UndoStepCreate("Height Smooth")
                    Case enumTool.Texture_Brush
                        Map.UndoStepCreate("Texture")
                    Case enumTool.None
                        If Map.Unit_Selected_Area_VertexA_Exists Then
                            SelectUnits(Map.Unit_Selected_Area_VertexA, MouseOver_Vertex)
                            Map.Unit_Selected_Area_VertexA_Exists = False
                        End If
                End Select
            End If
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            MouseRightIsDown = False

            If Not IsMinimap_MouseDown Then
                Select Case Tool
                    Case enumTool.Height_Change_Brush
                        Map.UndoStepCreate("Height Change")
                End Select
            End If
        End If
    End Sub

    Private Sub SelectUnits(ByVal VertexA As sXY_int, ByVal VertexB As sXY_int)
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
            Math.Abs(VertexA.Y - VertexB.Y) <= 1 Then
            If MouseOver_UnitCount > 0 Then
                If MouseOver_UnitCount = 1 Then
                    If MouseOver_Units(0).SelectedUnitNum < 0 Then
                        Map.SelectedUnit_Add(MouseOver_Units(0))
                    Else
                        Map.SelectedUnit_Remove(MouseOver_Units(0).SelectedUnitNum)
                    End If
                Else
                    ListSelect.Close()
                    ListSelect.Items.Clear()
                    ReDim ListSelectItems(MouseOver_UnitCount - 1)
                    For A = 0 To MouseOver_UnitCount - 1
                        If MouseOver_Units(A).Type Is Nothing Then
                            ListSelectItems(A) = New ToolStripButton("<nothing>")
                        Else
                            ListSelectItems(A) = New ToolStripButton(MouseOver_Units(A).Type.Code)
                        End If
                        ListSelectItems(A).Tag = MouseOver_Units(A)
                        ListSelect.Items.Add(ListSelectItems(A))
                    Next
                    ListSelectIsPicker = False
                    ListSelect.Show(Me, New Drawing.Point(MouseOver_ScreenPos.X, MouseOver_ScreenPos.Y))
                End If
            End If
        Else
            XY_Reorder(VertexA, VertexB, StartVertex, FinishVertex)
            StartPos.X = StartVertex.X * TerrainGridSpacing
            StartPos.Y = StartVertex.Y * TerrainGridSpacing
            FinishPos.X = FinishVertex.X * TerrainGridSpacing
            FinishPos.Y = FinishVertex.Y * TerrainGridSpacing
            SectorStart.X = Math.Min(CInt(Int(StartVertex.X / SectorTileSize)), Map.SectorCount.X - 1)
            SectorStart.Y = Math.Min(CInt(Int(StartVertex.Y / SectorTileSize)), Map.SectorCount.Y - 1)
            SectorFinish.X = Math.Min(CInt(Int(FinishVertex.X / SectorTileSize)), Map.SectorCount.X - 1)
            SectorFinish.Y = Math.Min(CInt(Int(FinishVertex.Y / SectorTileSize)), Map.SectorCount.Y - 1)
            For SectorNum.Y = SectorStart.Y To SectorFinish.Y
                For SectorNum.X = SectorStart.X To SectorFinish.X
                    For A = 0 To Map.Sectors(SectorNum.X, SectorNum.Y).UnitCount - 1
                        tmpUnit = Map.Sectors(SectorNum.X, SectorNum.Y).Units(A)
                        If tmpUnit.Pos.X >= StartPos.X And tmpUnit.Pos.Z >= StartPos.Y And _
                            tmpUnit.Pos.X <= FinishPos.X And tmpUnit.Pos.Z <= FinishPos.Y Then
                            Map.SelectedUnit_Add(tmpUnit)
                        End If
                    Next
                Next
            Next
        End If
        frmMainInstance.Selected_Object_Changed()
        DrawViewLater()
    End Sub

    Private Sub tmrDraw_Delay_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrDraw_Delay.Tick

        If DrawPending Then
            DrawPending = False
            DrawView()
        Else
            tmrDraw_Delay.Enabled = False
        End If
    End Sub

    Private Sub pnlDraw_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlDraw.Resize

        If OpenGLControl IsNot Nothing Then
            OpenGL_Size_Calc()
        End If
    End Sub

    Sub View_Radius_Set(ByVal Radius As Double)

        CircleTiles_Create(Radius, VisionSectors, TerrainGridSpacing * SectorTileSize)
    End Sub

    Public Function Pos_Get_Screen_XY(ByVal Pos As sXYZ_dbl, ByRef X_Output As Integer, ByRef Y_Output As Integer) As Boolean
        Static RatioZ_px As Double

        Try
            If Pos.Z > 0.0# Then
                RatioZ_px = 1.0# / (FOVMultiplier * Pos.Z)
                Y_Output = GLSize.Y / 2.0# - (Pos.Y * RatioZ_px)
                X_Output = GLSize.X / 2.0# + (Pos.X * RatioZ_px)
                Return True
            End If
        Catch

        End Try
        Return False
    End Function

    Public Function ScreenXY_Get_PlanePos(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByVal PlaneHeight As Double, ByRef ResultPos As sXY_dbl) As Boolean
        Static dblTemp As Double
        Static XYZ_dbl As sXYZ_dbl
        Static XYZ_dbl2 As sXYZ_dbl
        Static dblTemp2 As Double

        Try
            'convert screen pos to vector of one pos unit
            dblTemp2 = FOVMultiplier
            XYZ_dbl.X = (ScreenX - GLSize.X / 2.0#) * dblTemp2
            XYZ_dbl.Y = (GLSize.Y / 2.0# - ScreenY) * dblTemp2
            XYZ_dbl.Z = 1.0#
            'factor in the view angle
            VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
            'get distance to cover the height
            dblTemp = (PlaneHeight - ViewPos.Y) / XYZ_dbl2.Y
            ResultPos.X = ViewPos.X + XYZ_dbl2.X * dblTemp
            ResultPos.Y = ViewPos.Z + XYZ_dbl2.Z * dblTemp
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_TerrainPos(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByRef ResultPos As sXYZ_lng) As Boolean
        Static dblTemp As Double
        Static XYZ_dbl As sXYZ_dbl
        Static TerrainViewVector As sXYZ_dbl
        Static X As Integer
        Static Z As Integer
        Static LimitA As sXY_dbl
        Static LimitB As sXY_dbl
        Static Min As sXY_int
        Static Max As sXY_int
        Static TriGradientX As Double
        Static TriGradientZ As Double
        Static TriHeightOffset As Double
        Static Dist As Double
        Static BestPos As sXYZ_dbl
        Static BestDist As Double
        Static Dif As sXYZ_dbl
        Static InTileX As Double
        Static InTileZ As Double
        Static TilePos As sXY_dbl
        Static TerrainViewPos As sXYZ_dbl

        Try

            TerrainViewPos.X = ViewPos.X
            TerrainViewPos.Y = ViewPos.Y
            TerrainViewPos.Z = -ViewPos.Z

            'convert screen pos to vector of one pos unit
            XYZ_dbl.X = (ScreenX - GLSize.X / 2.0#) * FOVMultiplier
            XYZ_dbl.Y = (GLSize.Y / 2.0# - ScreenY) * FOVMultiplier
            XYZ_dbl.Z = 1.0#
            'rotate the vector so that it points forward and level
            VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, TerrainViewVector)
            TerrainViewVector.Y = -TerrainViewVector.Y 'get the amount of looking down, not up
            TerrainViewVector.Z = -TerrainViewVector.Z 'convert to terrain coordinates from view coordinates
            'get range of possible tiles
            dblTemp = (TerrainViewPos.Y - 255.0# * Map.HeightMultiplier) / TerrainViewVector.Y
            LimitA.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitA.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            dblTemp = TerrainViewPos.Y / TerrainViewVector.Y
            LimitB.X = TerrainViewPos.X + TerrainViewVector.X * dblTemp
            LimitB.Y = TerrainViewPos.Z + TerrainViewVector.Z * dblTemp
            Min.X = Math.Max(Math.Floor(Math.Min(LimitA.X, LimitB.X) / TerrainGridSpacing), 0)
            Min.Y = Math.Max(Math.Floor(Math.Min(LimitA.Y, LimitB.Y) / TerrainGridSpacing), 0)
            Max.X = Math.Min(Math.Floor(Math.Max(LimitA.X, LimitB.X) / TerrainGridSpacing), Map.TerrainSize.X - 1)
            Max.Y = Math.Min(Math.Floor(Math.Max(LimitA.Y, LimitB.Y) / TerrainGridSpacing), Map.TerrainSize.Y - 1)
            'find the nearest valid tile to the view
            BestDist = Double.MaxValue
            BestPos.X = Double.NaN
            BestPos.Y = Double.NaN
            BestPos.Z = Double.NaN
            For Z = Min.Y To Max.Y
                For X = Min.X To Max.X

                    TilePos.X = X * TerrainGridSpacing
                    TilePos.Y = Z * TerrainGridSpacing

                    If Map.TerrainTiles(X, Z).Tri Then
                        TriHeightOffset = Map.TerrainVertex(X, Z).Height * Map.HeightMultiplier
                        TriGradientX = Map.TerrainVertex(X + 1, Z).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.TerrainVertex(X, Z + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Z
                        If InTileZ <= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            GetDist(Dif, Dist)
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Map.TerrainVertex(X + 1, Z + 1).Height * Map.HeightMultiplier
                        TriGradientX = Map.TerrainVertex(X, Z + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.TerrainVertex(X + 1, Z).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + TriGradientZ + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X + TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Z
                        If InTileZ >= 1.0# - InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            GetDist(Dif, Dist)
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                    Else

                        TriHeightOffset = Map.TerrainVertex(X + 1, Z).Height * Map.HeightMultiplier
                        TriGradientX = Map.TerrainVertex(X, Z).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.TerrainVertex(X + 1, Z + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientX + (TriGradientX * (TilePos.X - TerrainViewPos.X) + TriGradientZ * (TerrainViewPos.Z - TilePos.Y) - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# - (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Z
                        If InTileZ <= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            GetDist(Dif, Dist)
                            If Dist < BestDist Then
                                BestDist = Dist
                                BestPos = XYZ_dbl
                            End If
                        End If

                        TriHeightOffset = Map.TerrainVertex(X, Z + 1).Height * Map.HeightMultiplier
                        TriGradientX = Map.TerrainVertex(X + 1, Z + 1).Height * Map.HeightMultiplier - TriHeightOffset
                        TriGradientZ = Map.TerrainVertex(X, Z).Height * Map.HeightMultiplier - TriHeightOffset
                        XYZ_dbl.Y = (TriHeightOffset + TriGradientZ + (TriGradientX * (TerrainViewPos.X - TilePos.X) + TriGradientZ * (TilePos.Y - TerrainViewPos.Z) + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) * TerrainViewPos.Y / TerrainViewVector.Y) / TerrainGridSpacing) / (1.0# + (TriGradientX * TerrainViewVector.X - TriGradientZ * TerrainViewVector.Z) / (TerrainViewVector.Y * TerrainGridSpacing))
                        XYZ_dbl.X = TerrainViewPos.X + TerrainViewVector.X * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        XYZ_dbl.Z = TerrainViewPos.Z + TerrainViewVector.Z * (TerrainViewPos.Y - XYZ_dbl.Y) / TerrainViewVector.Y
                        InTileX = XYZ_dbl.X / TerrainGridSpacing - X
                        InTileZ = XYZ_dbl.Z / TerrainGridSpacing - Z
                        If InTileZ >= InTileX And InTileX >= 0.0# And InTileZ >= 0.0# And InTileX <= 1.0# And InTileZ <= 1.0# Then
                            Dif.X = XYZ_dbl.X - TerrainViewPos.X
                            Dif.Y = XYZ_dbl.Y - TerrainViewPos.Y
                            Dif.Z = XYZ_dbl.Z - TerrainViewPos.Z
                            GetDist(Dif, Dist)
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

            ResultPos.X = BestPos.X
            ResultPos.Y = BestPos.Y
            ResultPos.Z = BestPos.Z
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Function ScreenXY_Get_PlanePos_ForwardDownOnly(ByVal ScreenX As Integer, ByVal ScreenY As Integer, ByVal PlaneHeight As Double, ByRef ResultPos As sXY_dbl) As Boolean
        Static dblTemp As Double
        Static XYZ_dbl As sXYZ_dbl
        Static XYZ_dbl2 As sXYZ_dbl
        Static dblTemp2 As Double

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
            VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl, XYZ_dbl2)
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

    Sub Apply_Gateway()
        If Not MouseOver_Pos_Exists Then
            Exit Sub
        End If

        If Control_Gateway_Delete.Active Then
            Dim A As Integer
            Dim Low As sXY_int
            Dim High As sXY_int
            A = 0
            Do While A < Map.GatewayCount
                XY_Reorder(Map.Gateways(A).PosA, Map.Gateways(A).PosB, Low, High)
                If Low.X <= MouseOver_Tile.X _
                And High.X >= MouseOver_Tile.X _
                And Low.Y <= MouseOver_Tile.Y _
                And High.Y >= MouseOver_Tile.Y Then
                    Map.Gateway_Remove(A)
                    Map.UndoStepCreate("Gateway Delete")
                    Map.MinimapMakeLater()
                    DrawViewLater()
                    Exit Do
                End If
                A += 1
            Loop
        Else
            If Not Map.Selected_Tile_A_Exists Then
                Map.Selected_Tile_A = MouseOver_Tile
                Map.Selected_Tile_A_Exists = True
                DrawViewLater()
            ElseIf MouseOver_Tile.X = Map.Selected_Tile_A.X Or MouseOver_Tile.Y = Map.Selected_Tile_A.Y Then
                Map.Gateway_Add(Map.Selected_Tile_A, MouseOver_Tile)
                Map.UndoStepCreate("Gateway Place")
                Map.Selected_Tile_A_Exists = False
                Map.Selected_Tile_B_Exists = False
                Map.MinimapMakeLater()
                DrawViewLater()
            End If
        End If
    End Sub

    Sub OpenGL_Resize(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        GLSize.X = OpenGLControl.Width
        GLSize.Y = OpenGLControl.Height
        If GLSize.Y <> 0 Then
            GLSize_XPerY = GLSize.X / GLSize.Y
        End If
        Viewport_Resize()
        FOV_Calc()
    End Sub

    Sub OpenGL_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)

        If Form.ActiveForm Is frmMainInstance Then
            OpenGLControl.Focus()
        End If
    End Sub

    Sub OpenGL_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Move As sXYZ_int
        Dim XYZ_dbl As sXYZ_dbl
        Dim A As Integer

        For A = 0 To Math.Abs(e.Delta / 120)
            VectorForwardRotationByMatrix(ViewAngleMatrix, Math.Sign(e.Delta) * Math.Max(ViewPos.Y, 512.0#) / 24.0#, XYZ_dbl)
            Move.X = XYZ_dbl.X
            Move.Y = XYZ_dbl.Y
            Move.Z = XYZ_dbl.Z
            ViewPosChange(Move)
        Next
    End Sub

    Public Function CreateGLFont(ByVal BaseFont As Font) As GLFont

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

    Public Sub DrawUnitRectangle(ByVal Position As sXYZ_dbl, ByVal Footprint As sXY_int, ByVal BorderInsideThickness As Double)
        Static PosA As sXY_dbl
        Static PosB As sXY_dbl
        Static PosC As sXY_dbl
        Static PosD As sXY_dbl

        PosA.X = Position.X - (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosA.Y = Position.Z - (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosB.X = Position.X + (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosB.Y = Position.Z - (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosC.X = Position.X - (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosC.Y = Position.Z + (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        PosD.X = Position.X + (Footprint.X - 0.125#) * TerrainGridSpacing / 2.0#
        PosD.Y = Position.Z + (Footprint.Y - 0.125#) * TerrainGridSpacing / 2.0#

        GL.Vertex3(PosA.X, Position.Y, -PosA.Y)
        GL.Vertex3(PosA.X + BorderInsideThickness, Position.Y, -(PosA.Y + BorderInsideThickness))
        GL.Vertex3(PosB.X - BorderInsideThickness, Position.Y, -(PosB.Y + BorderInsideThickness))
        GL.Vertex3(PosB.X, Position.Y, -PosB.Y)

        GL.Vertex3(PosA.X, Position.Y, -PosA.Y)
        GL.Vertex3(PosC.X, Position.Y, -PosC.Y)
        GL.Vertex3(PosC.X + BorderInsideThickness, Position.Y, -(PosC.Y - BorderInsideThickness))
        GL.Vertex3(PosA.X + BorderInsideThickness, Position.Y, -(PosA.Y + BorderInsideThickness))

        GL.Vertex3(PosB.X - BorderInsideThickness, Position.Y, -(PosB.Y + BorderInsideThickness))
        GL.Vertex3(PosD.X - BorderInsideThickness, Position.Y, -(PosD.Y - BorderInsideThickness))
        GL.Vertex3(PosD.X, Position.Y, -PosD.Y)
        GL.Vertex3(PosB.X, Position.Y, -PosA.Y)

        GL.Vertex3(PosC.X + BorderInsideThickness, Position.Y, -(PosC.Y - BorderInsideThickness))
        GL.Vertex3(PosC.X, Position.Y, -PosC.Y)
        GL.Vertex3(PosD.X, Position.Y, -PosD.Y)
        GL.Vertex3(PosD.X - BorderInsideThickness, Position.Y, -(PosD.Y - BorderInsideThickness))
    End Sub

    Public Sub TerrainPicker()
        Dim A As Integer

        frmMainInstance.lstAutoTexture.Enabled = False
        For A = 0 To frmMainInstance.lstAutoTexture.Items.Count - 1
            If Map.Painter.Terrains(A) Is Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Terrain Then
                frmMainInstance.lstAutoTexture.SelectedIndex = A
                Exit For
            End If
        Next
        If A = frmMainInstance.lstAutoTexture.Items.Count Then
            frmMainInstance.lstAutoTexture.SelectedIndex = -1
        End If
        frmMainInstance.lstAutoTexture.Enabled = True
        SelectedTerrain = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Terrain
    End Sub

    Public Sub TexturePicker()

        If Map.Tileset IsNot Nothing Then
            If Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.TextureNum < Map.Tileset.TileCount Then
                SelectedTexture = Map.TerrainTiles(MouseOver_Tile.X, MouseOver_Tile.Y).Texture.TextureNum
                frmMainInstance.TextureView.DrawViewLater()
            End If
        End If
    End Sub

    Public Sub HeightPickerL()

        frmMainInstance.txtHeightSetL.Text = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height
        frmMainInstance.txtHeightSetL.Focus()
        OpenGLControl.Focus()
    End Sub

    Public Sub HeightPickerR()

        frmMainInstance.txtHeightSetR.Text = Map.TerrainVertex(MouseOver_Vertex.X, MouseOver_Vertex.Y).Height
        frmMainInstance.txtHeightSetR.Focus()
        OpenGLControl.Focus()
    End Sub

    Public Sub ObjectPicker(ByVal UnitType As clsUnitType)
        Dim A As Integer

        If UnitType Is Nothing Then
            Exit Sub
        End If

        For A = 0 To frmMainInstance.lstFeatures.Items.Count - 1
            If frmMainInstance.lstFeatures_Unit(A) IsNot Nothing Then
                If frmMainInstance.lstFeatures_Unit(A) Is UnitType Then
                    frmMainInstance.lstFeatures.SelectedIndex = A
                    Tool = enumTool.Object_Unit
                    Exit For
                End If
            End If
        Next
        For A = 0 To frmMainInstance.lstStructures.Items.Count - 1
            If frmMainInstance.lstStructures_Unit(A) IsNot Nothing Then
                If frmMainInstance.lstStructures_Unit(A) Is UnitType Then
                    frmMainInstance.lstStructures.SelectedIndex = A
                    Tool = enumTool.Object_Structure
                    Exit For
                End If
            End If
        Next
        For A = 0 To frmMainInstance.lstDroids.Items.Count - 1
            If frmMainInstance.lstDroids_Unit(A) IsNot Nothing Then
                If frmMainInstance.lstDroids_Unit(A) Is UnitType Then
                    frmMainInstance.lstDroids.SelectedIndex = A
                    Tool = enumTool.Object_Unit
                    Exit For
                End If
            End If
        Next
    End Sub

    Public Sub MouseOverUnit_Clear()

        ReDim Preserve MouseOver_Units(-1)
        MouseOver_UnitCount = 0
    End Sub

    Public Sub MouseOverUnit_Add(ByVal UnitToAdd As clsMap.clsUnit)

        ReDim Preserve MouseOver_Units(MouseOver_UnitCount)
        MouseOver_Units(MouseOver_UnitCount) = UnitToAdd
        MouseOver_UnitCount += 1
    End Sub

    Public Sub MouseOverUnit_Remove(ByVal Num As Integer)

        MouseOver_UnitCount -= 1
        If Num <> MouseOver_UnitCount Then
            MouseOver_Units(Num) = MouseOver_Units(MouseOver_UnitCount)
        End If
        ReDim Preserve MouseOver_Units(MouseOver_UnitCount - 1)
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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.tmrDraw = New System.Windows.Forms.Timer()
        Me.tmrDraw_Delay = New System.Windows.Forms.Timer()
        Me.ssStatus = New System.Windows.Forms.StatusStrip()
        Me.lblTile = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblVertex = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblPos = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.lblUndo = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ssStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'tmrDraw
        '
        Me.tmrDraw.Interval = 1
        '
        'tmrDraw_Delay
        '
        Me.tmrDraw_Delay.Interval = 40
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
        'pnlDraw
        '
        Me.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDraw.Location = New System.Drawing.Point(0, 0)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(1308, 392)
        Me.pnlDraw.TabIndex = 1
        '
        'lblUndo
        '
        Me.lblUndo.AutoSize = False
        Me.lblUndo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUndo.Name = "lblUndo"
        Me.lblUndo.Size = New System.Drawing.Size(256, 27)
        Me.lblUndo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
    Friend WithEvents tmrDraw_Delay As System.Windows.Forms.Timer
    Friend WithEvents tmrDraw As System.Windows.Forms.Timer
    Friend WithEvents ssStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents lblTile As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblVertex As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblPos As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pnlDraw As System.Windows.Forms.Panel
    Friend WithEvents lblUndo As System.Windows.Forms.ToolStripStatusLabel
#End If
End Class