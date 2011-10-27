Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public ViewInfo As clsViewInfo

    Public Sub GLDraw()
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
        Dim DrawCentreSector As clsBrush.sPosNum
        Dim SelectionLabel As New clsTextLabel
        Dim light_position(3) As Single
        Dim matrixA As New Matrix3D.Matrix3D
        Dim matrixB As New Matrix3D.Matrix3D
        Dim MapAction As clsMap.clsAction
        Dim ZNearFar As Single
        Dim MapView As ctrlMapView = ViewInfo.MapView
        Dim GLSize As sXY_int = ViewInfo.MapView.GLSize
        Dim DrawCentre As sXY_dbl
        Dim dblTemp2 As Double

        If Not ViewInfo.ScreenXY_Get_ViewPlanePos(New sXY_int(CInt(GLSize.X / 2.0#), CInt(GLSize.Y / 2.0#)), dblTemp, DrawCentre) Then
            Matrix3D.VectorForwardsRotationByMatrix(ViewInfo.ViewAngleMatrix, XYZ_dbl)
            dblTemp2 = VisionRadius * 2.0# / Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z)
            DrawCentre.X = ViewInfo.ViewPos.X + XYZ_dbl.X * dblTemp2
            DrawCentre.Y = ViewInfo.ViewPos.Z + XYZ_dbl.Z * dblTemp2
        End If
        DrawCentre.X = Clamp_dbl(DrawCentre.X, 0.0#, Terrain.TileSize.X * TerrainGridSpacing - 1.0#)
        DrawCentre.Y = Clamp_dbl(-DrawCentre.Y, 0.0#, Terrain.TileSize.Y * TerrainGridSpacing - 1.0#)
        DrawCentreSector.Normal = GetPosSectorNum(New sXY_int(CInt(DrawCentre.X), CInt(DrawCentre.Y)))
        DrawCentreSector.Alignment = GetPosSectorNum(New sXY_int(CInt(DrawCentre.X - SectorTileSize * TerrainGridSpacing / 2.0#), CInt(DrawCentre.Y - SectorTileSize * TerrainGridSpacing / 2.0#)))

        Dim DrawObjects As New clsMap.clsDrawSectorObjects
        DrawObjects.Map = Me
        DrawObjects.UnitTextLabels = New clsTextLabels(64)
        DrawObjects.Start()

        XYZ_dbl.X = DrawCentre.X - ViewInfo.ViewPos.X
        XYZ_dbl.Y = 128 - ViewInfo.ViewPos.Y
        XYZ_dbl.Z = -DrawCentre.Y - ViewInfo.ViewPos.Z
        ZNearFar = CSng(XYZ_dbl.GetMagnitude)

        GL.Enable(EnableCap.DepthTest)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreatePerspectiveFieldOfView(ViewInfo.FieldOfViewY, MapView.GLSize_XPerY, ZNearFar / 128.0F, ZNearFar * 128.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        Matrix3D.MatrixRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, SunAngleMatrix, matrixB)
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

        dblTemp = 127.5# * HeightMultiplier
        If ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, 0, dblTemp, ViewCorner0) _
        And ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, 0, dblTemp, ViewCorner1) _
        And ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, GLSize.Y, dblTemp, ViewCorner2) _
        And ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, GLSize.Y, dblTemp, ViewCorner3) Then
            ShowMinimapViewPosBox = True
        Else
            ShowMinimapViewPosBox = False
        End If

        GL.Rotate(AngleClamp(-ViewInfo.ViewAngleRPY.Roll) / RadOf1Deg, 0.0F, 0.0F, 1.0F)
        GL.Rotate(ViewInfo.ViewAngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
        GL.Rotate(ViewInfo.ViewAngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
        GL.Translate(-ViewInfo.ViewPos.X, -ViewInfo.ViewPos.Y, ViewInfo.ViewPos.Z)

        GL.Enable(EnableCap.CullFace)

        If Draw_TileTextures Then
            GL.Color3(1.0F, 1.0F, 1.0F)
            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
            MapAction = New clsMap.clsDrawCallTerrain
            MapAction.Map = Me
            VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector)
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)
        GL.Disable(EnableCap.Lighting)

        If Draw_TileWireframe Then
            GL.Color3(0.0F, 1.0F, 0.0F)
            GL.LineWidth(1.0F)
            Dim DrawCallTerrainWireframe As New clsMap.clsDrawCallTerrainWireframe
            DrawCallTerrainWireframe.Map = Me
            VisionSectors.PerformActionMapSectors(DrawCallTerrainWireframe, DrawCentreSector)
        End If

        'draw tile orientation markers

        If DisplayTileOrientation Then

            GL.Disable(EnableCap.CullFace)

            GL.Begin(BeginMode.Triangles)
            GL.Color3(1.0F, 1.0F, 0.0F)
            MapAction = New clsMap.clsDrawTileOrientation
            MapAction.Map = Me
            VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector)
            GL.End()

            GL.Enable(EnableCap.CullFace)
        End If

        'draw painted texture terrain type markers

        Dim RGB_sng As sRGB_sng

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = ViewInfo.GetMouseOverTerrain()

        If Draw_VertexTerrain Then
            GL.LineWidth(1.0F)
            Dim DrawVertexTerran As New clsMap.clsDrawVertexTerrain
            DrawVertexTerran.Map = Me
            DrawVertexTerran.ViewAngleMatrix = ViewInfo.ViewAngleMatrix
            VisionSectors.PerformActionMapSectors(DrawVertexTerran, DrawCentreSector)
        End If

        SelectionLabel.Text = ""

        If Selected_Area_VertexA IsNot Nothing Then
            DrawIt = False
            If Selected_Area_VertexB IsNot Nothing Then
                'area is selected
                XY_Reorder(Selected_Area_VertexA.XY, Selected_Area_VertexB.XY, StartXY, FinishXY)
                XYZ_dbl.X = Selected_Area_VertexB.X * TerrainGridSpacing - ViewInfo.ViewPos.X
                XYZ_dbl.Z = -Selected_Area_VertexB.Y * TerrainGridSpacing - ViewInfo.ViewPos.Z
                XYZ_dbl.Y = GetVertexAltitude(Selected_Area_VertexB.XY) - ViewInfo.ViewPos.Y
                DrawIt = True
            ElseIf Tool = enumTool.Terrain_Select Then
                If MouseOverTerrain IsNot Nothing Then
                    'selection is changing under pointer
                    XY_Reorder(Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                    XYZ_dbl.X = MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing - ViewInfo.ViewPos.X
                    XYZ_dbl.Z = -MouseOverTerrain.Vertex.Normal.Y * TerrainGridSpacing - ViewInfo.ViewPos.Z
                    XYZ_dbl.Y = GetVertexAltitude(MouseOverTerrain.Vertex.Normal) - ViewInfo.ViewPos.Y
                    DrawIt = True
                End If
            End If
            If DrawIt Then
                Matrix3D.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                If ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos) Then
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
                Dim DrawSelection As New clsMap.clsDrawAreaOutline
                DrawSelection.Map = Me
                DrawSelection.StartXY = StartXY
                DrawSelection.FinishXY = FinishXY
                DrawSelection.Colour = New sRGBA_sng(1.0F, 1.0F, 1.0F, 1.0F)
                DrawSelection.ActionPerform()
            End If
        End If

        If Tool = enumTool.Terrain_Select Then
            If MouseOverTerrain IsNot Nothing Then
                'draw mouseover vertex
                GL.LineWidth(3.0F)

                Vertex0.X = MouseOverTerrain.Vertex.Normal.X * TerrainGridSpacing
                Vertex0.Y = Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * HeightMultiplier
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
            For A = 0 To GatewayCount - 1
                If Gateways(A).PosA.X = Gateways(A).PosB.X Then
                    If Gateways(A).PosA.Y <= Gateways(A).PosB.Y Then
                        C = Gateways(A).PosA.Y
                        D = Gateways(A).PosB.Y
                    Else
                        C = Gateways(A).PosB.Y
                        D = Gateways(A).PosA.Y
                    End If
                    X2 = Gateways(A).PosA.X
                    For Y2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                        Vertex0.Z = -Y2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                        Vertex1.Z = -Y2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                        Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
                        Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.LineLoop)
                        GL.Color3(0.75F, 1.0F, 0.0F)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                        GL.End()
                    Next
                ElseIf Gateways(A).PosA.Y = Gateways(A).PosB.Y Then
                    If Gateways(A).PosA.X <= Gateways(A).PosB.X Then
                        C = Gateways(A).PosA.X
                        D = Gateways(A).PosB.X
                    Else
                        C = Gateways(A).PosB.X
                        D = Gateways(A).PosA.X
                    End If
                    Y2 = Gateways(A).PosA.Y
                    For X2 = C To D
                        Vertex0.X = X2 * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                        Vertex0.Z = -Y2 * TerrainGridSpacing
                        Vertex1.X = (X2 + 1) * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                        Vertex1.Z = -Y2 * TerrainGridSpacing
                        Vertex2.X = X2 * TerrainGridSpacing
                        Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                        Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                        Vertex3.X = (X2 + 1) * TerrainGridSpacing
                        Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
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
                    X2 = Gateways(A).PosA.X
                    Y2 = Gateways(A).PosA.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(1.0F, 0.0F, 0.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    X2 = Gateways(A).PosB.X
                    Y2 = Gateways(A).PosB.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
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
                If Unit_Selected_Area_VertexA IsNot Nothing Then
                    'selection is changing under pointer
                    XY_Reorder(Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                    GL.LineWidth(2.0F)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(X, StartXY.Y).Height * HeightMultiplier
                        Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(X + 1, StartXY.Y).Height * HeightMultiplier
                        Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For X = StartXY.X To FinishXY.X - 1
                        Vertex0.X = X * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(X, FinishXY.Y).Height * HeightMultiplier
                        Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                        Vertex1.X = (X + 1) * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(X + 1, FinishXY.Y).Height * HeightMultiplier
                        Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Y = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = StartXY.X * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(StartXY.X, Y).Height * HeightMultiplier
                        Vertex0.Z = -Y * TerrainGridSpacing
                        Vertex1.X = StartXY.X * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(StartXY.X, Y + 1).Height * HeightMultiplier
                        Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                        GL.Begin(BeginMode.Lines)
                        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                        GL.End()
                    Next
                    For Y = StartXY.Y To FinishXY.Y - 1
                        Vertex0.X = FinishXY.X * TerrainGridSpacing
                        Vertex0.Y = Terrain.Vertices(FinishXY.X, Y).Height * HeightMultiplier
                        Vertex0.Z = -Y * TerrainGridSpacing
                        Vertex1.X = FinishXY.X * TerrainGridSpacing
                        Vertex1.Y = Terrain.Vertices(FinishXY.X, Y + 1).Height * HeightMultiplier
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
                    Vertex0.Y = Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y).Height * HeightMultiplier
                    Vertex0.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = MouseOverTerrain.Side_Num.X * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y + 1).Height * HeightMultiplier
                    Vertex1.Z = -(MouseOverTerrain.Side_Num.Y + 1) * TerrainGridSpacing
                Else
                    Vertex0.X = MouseOverTerrain.Side_Num.X * TerrainGridSpacing
                    Vertex0.Y = Terrain.Vertices(MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y).Height * HeightMultiplier
                    Vertex0.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                    Vertex1.X = (MouseOverTerrain.Side_Num.X + 1) * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(MouseOverTerrain.Side_Num.X + 1, MouseOverTerrain.Side_Num.Y).Height * HeightMultiplier
                    Vertex1.Z = -MouseOverTerrain.Side_Num.Y * TerrainGridSpacing
                End If

                GL.Begin(BeginMode.Lines)
                GL.Color3(0.0F, 1.0F, 1.0F)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.End()
            ElseIf Tool = enumTool.AutoRoad_Line Or Tool = enumTool.Gateways Then
                GL.LineWidth(2.0F)

                If Selected_Tile_A IsNot Nothing Then
                    X2 = Selected_Tile_A.X
                    Y2 = Selected_Tile_A.Y

                    Vertex0.X = X2 * TerrainGridSpacing
                    Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
                    Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                    GL.Begin(BeginMode.LineLoop)
                    GL.Color3(0.0F, 1.0F, 1.0F)
                    GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                    GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                    GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                    GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                    GL.End()

                    If MouseOverTerrain.Tile.Normal.X = Selected_Tile_A.X Then
                        If MouseOverTerrain.Tile.Normal.Y <= Selected_Tile_A.Y Then
                            A = MouseOverTerrain.Tile.Normal.Y
                            B = Selected_Tile_A.Y
                        Else
                            A = Selected_Tile_A.Y
                            B = MouseOverTerrain.Tile.Normal.Y
                        End If
                        X2 = Selected_Tile_A.X
                        For Y2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                            Vertex0.Z = -Y2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                            Vertex1.Z = -Y2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                            Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
                            Vertex3.Z = -(Y2 + 1) * TerrainGridSpacing
                            GL.Begin(BeginMode.LineLoop)
                            GL.Color3(0.0F, 1.0F, 1.0F)
                            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                            GL.End()
                        Next
                    ElseIf MouseOverTerrain.Tile.Normal.Y = Selected_Tile_A.Y Then
                        If MouseOverTerrain.Tile.Normal.X <= Selected_Tile_A.X Then
                            A = MouseOverTerrain.Tile.Normal.X
                            B = Selected_Tile_A.X
                        Else
                            A = Selected_Tile_A.X
                            B = MouseOverTerrain.Tile.Normal.X
                        End If
                        Y2 = Selected_Tile_A.Y
                        For X2 = A To B
                            Vertex0.X = X2 * TerrainGridSpacing
                            Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                            Vertex0.Z = -Y2 * TerrainGridSpacing
                            Vertex1.X = (X2 + 1) * TerrainGridSpacing
                            Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                            Vertex1.Z = -Y2 * TerrainGridSpacing
                            Vertex2.X = X2 * TerrainGridSpacing
                            Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                            Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                            Vertex3.X = (X2 + 1) * TerrainGridSpacing
                            Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
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
                    Vertex0.Y = Terrain.Vertices(X2, Y2).Height * HeightMultiplier
                    Vertex0.Z = -Y2 * TerrainGridSpacing
                    Vertex1.X = (X2 + 1) * TerrainGridSpacing
                    Vertex1.Y = Terrain.Vertices(X2 + 1, Y2).Height * HeightMultiplier
                    Vertex1.Z = -Y2 * TerrainGridSpacing
                    Vertex2.X = X2 * TerrainGridSpacing
                    Vertex2.Y = Terrain.Vertices(X2, Y2 + 1).Height * HeightMultiplier
                    Vertex2.Z = -(Y2 + 1) * TerrainGridSpacing
                    Vertex3.X = (X2 + 1) * TerrainGridSpacing
                    Vertex3.Y = Terrain.Vertices(X2 + 1, Y2 + 1).Height * HeightMultiplier
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
                DrawTileOutline.Map = Me
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
                Vertex0.Y = Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * HeightMultiplier
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
                DrawVertexMarker.Map = Me
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
        GL.Rotate(AngleClamp(-ViewInfo.ViewAngleRPY.Roll) / RadOf1Deg, 0.0F, 0.0F, 1.0F)
        GL.Rotate(ViewInfo.ViewAngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
        GL.Rotate(ViewInfo.ViewAngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)

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
                    WorldPos = TileAligned_Pos_From_MapPos(MouseOverTerrain.Pos.Horizontal, frmMainInstance.SelectedObjectType.GetFootprint)
                    GL.PushMatrix()
                    GL.Translate(WorldPos.Horizontal.X - ViewInfo.ViewPos.X, WorldPos.Altitude - ViewInfo.ViewPos.Y + 2.0#, ViewInfo.ViewPos.Z + WorldPos.Horizontal.Y)
                    frmMainInstance.SelectedObjectType.GLDraw(0.0F)
                    GL.PopMatrix()
                End If
            End If
            GL.Disable(EnableCap.Texture2D)
        End If

        GL.Disable(EnableCap.DepthTest)

        GL.PushMatrix()
        GL.Translate(-ViewInfo.ViewPos.X, -ViewInfo.ViewPos.Y, ViewInfo.ViewPos.Z)
        For A = 0 To ScriptMarkerCount - 1
            ScriptMarkers(A).GLDraw()
        Next
        GL.PopMatrix()

        'draw unit selection

        GL.Begin(BeginMode.Quads)
        For A = 0 To SelectedUnits.UnitCount - 1
            tmpUnit = SelectedUnits.Units(A)
            XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - ViewInfo.ViewPos.X
            XYZ_dbl.Y = tmpUnit.Pos.Altitude - ViewInfo.ViewPos.Y
            XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z
            Footprint = tmpUnit.Type.GetFootprint
            RGB_sng = GetUnitGroupColour(tmpUnit.UnitGroup)
            ColourA = New sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F)
            ColourB = New sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.75F)
            DrawUnitRectangle(XYZ_dbl, Footprint, 8.0#, ColourA, ColourB)
        Next
        If MouseOverTerrain IsNot Nothing Then
            For A = 0 To MouseOverTerrain.UnitCount - 1
                If MouseOverTerrain.Units(A) IsNot Nothing And Tool = enumTool.None Then
                    tmpUnit = MouseOverTerrain.Units(A)
                    XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - ViewInfo.ViewPos.X
                    XYZ_dbl.Y = tmpUnit.Pos.Altitude - ViewInfo.ViewPos.Y
                    XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z
                    RGB_sng = GetUnitGroupColour(tmpUnit.UnitGroup)
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
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, CSng(GLSize.X), CSng(0.0F), CSng(GLSize.Y), -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        dblTemp = Settings.MinimapSize
        ViewInfo.Tiles_Per_Minimap_Pixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) / (RootTwo * dblTemp)

        If Minimap_Texture_Size > 0 And ViewInfo.Tiles_Per_Minimap_Pixel > 0.0# Then

            MinimapSizeXY.X = CInt(Terrain.TileSize.X / ViewInfo.Tiles_Per_Minimap_Pixel)
            MinimapSizeXY.Y = CInt(Terrain.TileSize.Y / ViewInfo.Tiles_Per_Minimap_Pixel)

            GL.Translate(0.0F, GLSize.Y - MinimapSizeXY.Y, 0.0F)

            XYZ_dbl.X = Terrain.TileSize.X / Minimap_Texture_Size
            XYZ_dbl.Z = Terrain.TileSize.Y / Minimap_Texture_Size

            If Minimap_GLTexture > 0 Then

                GL.Enable(EnableCap.Texture2D)
                GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Decal)
                GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture)

                GL.Begin(BeginMode.Quads)

                GL.TexCoord2(0.0F, 0.0F)
                GL.Vertex2(0, MinimapSizeXY.Y)

                GL.TexCoord2(CSng(XYZ_dbl.X), 0.0F)
                GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y)

                GL.TexCoord2(CSng(XYZ_dbl.X), CSng(XYZ_dbl.Z))
                GL.Vertex2(MinimapSizeXY.X, 0)

                GL.TexCoord2(0.0F, CSng(XYZ_dbl.Z))
                GL.Vertex2(0, 0)

                GL.End()

                GL.Disable(EnableCap.Texture2D)
            End If

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
                dblTemp = TerrainGridSpacing * ViewInfo.Tiles_Per_Minimap_Pixel

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

            If Selected_Area_VertexA IsNot Nothing Then
                DrawIt = False
                If Selected_Area_VertexB IsNot Nothing Then
                    'area is selected
                    XY_Reorder(Selected_Area_VertexA.XY, Selected_Area_VertexB.XY, StartXY, FinishXY)
                    DrawIt = True
                ElseIf Tool = enumTool.Terrain_Select Then
                    If MouseOverTerrain IsNot Nothing Then
                        'selection is changing under mouse
                        XY_Reorder(Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY)
                        DrawIt = True
                    End If
                End If
                If DrawIt Then
                    GL.LineWidth(1.0F)
                    PosA.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosA.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosB.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosB.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosC.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosC.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosD.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel
                    PosD.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel
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
    End Sub

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

    Public Class clsDrawTileOutline
        Inherits clsMap.clsAction

        Public Colour As sRGBA_sng

        Private Vertex0 As sXYZ_int
        Private Vertex1 As sXYZ_int
        Private Vertex2 As sXYZ_int
        Private Vertex3 As sXYZ_int

        Public Overrides Sub ActionPerform()

            Vertex0.X = PosNum.X * TerrainGridSpacing
            Vertex0.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y).Height * Map.HeightMultiplier
            Vertex0.Z = -PosNum.Y * TerrainGridSpacing
            Vertex1.X = (PosNum.X + 1) * TerrainGridSpacing
            Vertex1.Y = Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height * Map.HeightMultiplier
            Vertex1.Z = -PosNum.Y * TerrainGridSpacing
            Vertex2.X = PosNum.X * TerrainGridSpacing
            Vertex2.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height * Map.HeightMultiplier
            Vertex2.Z = -(PosNum.Y + 1) * TerrainGridSpacing
            Vertex3.X = (PosNum.X + 1) * TerrainGridSpacing
            Vertex3.Y = Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height * Map.HeightMultiplier
            Vertex3.Z = -(PosNum.Y + 1) * TerrainGridSpacing
            GL.Begin(BeginMode.LineLoop)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.End()
        End Sub
    End Class

    Public Class clsDrawAreaOutline
        'does not inherit action

        Public Map As clsMap
        Public Colour As sRGBA_sng
        Public StartXY As sXY_int
        Public FinishXY As sXY_int

        Private Vertex0 As sXYZ_int
        Private Vertex1 As sXYZ_int
        Private Vertex2 As sXYZ_int
        Private Vertex3 As sXYZ_int

        Public Sub ActionPerform()

            GL.Begin(BeginMode.Lines)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            For X = StartXY.X To FinishXY.X - 1
                Vertex0.X = X * TerrainGridSpacing
                Vertex0.Y = Map.Terrain.Vertices(X, StartXY.Y).Height * Map.HeightMultiplier
                Vertex0.Z = -StartXY.Y * TerrainGridSpacing
                Vertex1.X = (X + 1) * TerrainGridSpacing
                Vertex1.Y = Map.Terrain.Vertices(X + 1, StartXY.Y).Height * Map.HeightMultiplier
                Vertex1.Z = -StartXY.Y * TerrainGridSpacing
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            Next
            For X = StartXY.X To FinishXY.X - 1
                Vertex0.X = X * TerrainGridSpacing
                Vertex0.Y = Map.Terrain.Vertices(X, FinishXY.Y).Height * Map.HeightMultiplier
                Vertex0.Z = -FinishXY.Y * TerrainGridSpacing
                Vertex1.X = (X + 1) * TerrainGridSpacing
                Vertex1.Y = Map.Terrain.Vertices(X + 1, FinishXY.Y).Height * Map.HeightMultiplier
                Vertex1.Z = -FinishXY.Y * TerrainGridSpacing
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            Next
            For Y = StartXY.Y To FinishXY.Y - 1
                Vertex0.X = StartXY.X * TerrainGridSpacing
                Vertex0.Y = Map.Terrain.Vertices(StartXY.X, Y).Height * Map.HeightMultiplier
                Vertex0.Z = -Y * TerrainGridSpacing
                Vertex1.X = StartXY.X * TerrainGridSpacing
                Vertex1.Y = Map.Terrain.Vertices(StartXY.X, Y + 1).Height * Map.HeightMultiplier
                Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            Next
            For Y = StartXY.Y To FinishXY.Y - 1
                Vertex0.X = FinishXY.X * TerrainGridSpacing
                Vertex0.Y = Map.Terrain.Vertices(FinishXY.X, Y).Height * Map.HeightMultiplier
                Vertex0.Z = -Y * TerrainGridSpacing
                Vertex1.X = FinishXY.X * TerrainGridSpacing
                Vertex1.Y = Map.Terrain.Vertices(FinishXY.X, Y + 1).Height * Map.HeightMultiplier
                Vertex1.Z = -(Y + 1) * TerrainGridSpacing
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            Next
            GL.End()
        End Sub
    End Class

    Public Class clsDrawCallTerrain
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            GL.CallList(Map.Sectors(PosNum.X, PosNum.Y).GLList_Textured)
        End Sub
    End Class

    Public Class clsDrawCallTerrainWireframe
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            GL.CallList(Map.Sectors(PosNum.X, PosNum.Y).GLList_Wireframe)
        End Sub
    End Class

    Public Class clsDrawTileOrientation
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()
            Dim X As Integer
            Dim Y As Integer

            For Y = PosNum.Y * SectorTileSize To Math.Min((PosNum.Y + 1) * SectorTileSize - 1, Map.Terrain.TileSize.Y - 1)
                For X = PosNum.X * SectorTileSize To Math.Min((PosNum.X + 1) * SectorTileSize - 1, Map.Terrain.TileSize.X - 1)
                    Map.DrawTileOrientation(New sXY_int(X, Y))
                Next
            Next
        End Sub
    End Class

    Public Class clsDrawVertexTerrain
        Inherits clsMap.clsAction

        Public ViewAngleMatrix As Matrix3D.Matrix3D

        Private RGB_sng As sRGB_sng
        Private RGB_sng2 As sRGB_sng
        Private XYZ_dbl As Matrix3D.XYZ_dbl
        Private XYZ_dbl2 As Matrix3D.XYZ_dbl
        Private XYZ_dbl3 As Matrix3D.XYZ_dbl
        Private Vertex0 As Matrix3D.XYZ_dbl
        Private Vertex1 As Matrix3D.XYZ_dbl
        Private Vertex2 As Matrix3D.XYZ_dbl
        Private Vertex3 As Matrix3D.XYZ_dbl

        Public Overrides Sub ActionPerform()
            Dim X As Integer
            Dim Y As Integer
            Dim A As Integer

            For Y = PosNum.Y * SectorTileSize To Math.Min((PosNum.Y + 1) * SectorTileSize - 1, Map.Terrain.TileSize.Y)
                For X = PosNum.X * SectorTileSize To Math.Min((PosNum.X + 1) * SectorTileSize - 1, Map.Terrain.TileSize.X)
                    If Map.Terrain.Vertices(X, Y).Terrain IsNot Nothing Then
                        A = Map.Terrain.Vertices(X, Y).Terrain.Num
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
                                XYZ_dbl.X = X * TerrainGridSpacing
                                XYZ_dbl.Y = Map.Terrain.Vertices(X, Y).Height * Map.HeightMultiplier
                                XYZ_dbl.Z = -Y * TerrainGridSpacing
                                XYZ_dbl2.X = 10.0#
                                XYZ_dbl2.Y = 10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex0.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex0.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex0.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = -10.0#
                                XYZ_dbl2.Y = 10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex1.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex1.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex1.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = -10.0#
                                XYZ_dbl2.Y = -10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex2.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex2.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex2.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = 10.0#
                                XYZ_dbl2.Y = -10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
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
        End Sub
    End Class

    Public Class clsDrawVertexMarker
        Inherits clsMap.clsAction

        Public Colour As sRGBA_sng

        Private Vertex0 As sXYZ_int

        Public Overrides Sub ActionPerform()

            Vertex0.X = PosNum.X * TerrainGridSpacing
            Vertex0.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y).Height * Map.HeightMultiplier
            Vertex0.Z = -PosNum.Y * TerrainGridSpacing
            GL.Begin(BeginMode.Lines)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            GL.Vertex3(Vertex0.X - 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X + 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8)
            GL.End()
        End Sub
    End Class

    Public Class clsDrawHorizontalPosOnTerrain
        'does not inherit action

        Public Map As clsMap

        Public Horizontal As sXY_int
        Public Colour As sRGBA_sng

        Private Vertex0 As sXYZ_int

        Public Sub ActionPerform()

            Vertex0.X = Horizontal.X
            Vertex0.Y = CInt(Map.GetTerrainHeight(Horizontal))
            Vertex0.Z = -Horizontal.Y
            GL.Begin(BeginMode.Lines)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            GL.Vertex3(Vertex0.X - 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X + 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8)
            GL.End()
        End Sub
    End Class

    Public Class clsDrawSectorObjects
        Inherits clsMap.clsAction

        Public UnitTextLabels As clsTextLabels

        Private UnitDrawn() As Boolean
        Private Started As Boolean

        Public Sub Start()

            ReDim UnitDrawn(Map.UnitCount - 1)

            Started = True
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer
            Dim tmpUnit As clsUnit
            Dim tmpSector As clsSector = Map.Sectors(PosNum.X, PosNum.Y)
            Dim DrawUnitLabel As Boolean
            Dim ViewInfo As clsViewInfo = Map.ViewInfo
            Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = ViewInfo.GetMouseOverTerrain
            Dim TextLabel As clsTextLabel
            Dim XYZ_dbl As Matrix3D.XYZ_dbl
            Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
            Dim ScreenPos As sXY_int

            For A = 0 To tmpSector.UnitCount - 1
                tmpUnit = tmpSector.Units(A)
                If Not UnitDrawn(tmpUnit.Map_UnitNum) Then
                    UnitDrawn(tmpUnit.Map_UnitNum) = True
                    XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - ViewInfo.ViewPos.X
                    XYZ_dbl.Y = tmpUnit.Pos.Altitude - ViewInfo.ViewPos.Y
                    XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z
                    DrawUnitLabel = False
                    If tmpUnit.Type.IsUnknown Then
                        DrawUnitLabel = True
                    Else
                        GL.PushMatrix()
                        GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, -XYZ_dbl.Z)
                        tmpUnit.Type.GLDraw(tmpUnit.Rotation)
                        GL.PopMatrix()
                        If tmpUnit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                            If CType(tmpUnit.Type, clsDroidDesign).AlwaysDrawTextLabel Then
                                DrawUnitLabel = True
                            End If
                        End If
                        If MouseOverTerrain IsNot Nothing Then
                            If MouseOverTerrain.UnitCount > 0 Then
                                If MouseOverTerrain.Units(0) Is tmpUnit Then
                                    DrawUnitLabel = True
                                End If
                            End If
                        End If
                    End If
                    If DrawUnitLabel And Not UnitTextLabels.AtMaxCount Then
                        Matrix3D.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                        If ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos) Then
                            If ScreenPos.X >= 0 And ScreenPos.X <= ViewInfo.MapView.GLSize.X And ScreenPos.Y >= 0 And ScreenPos.Y <= ViewInfo.MapView.GLSize.Y Then
                                TextLabel = New clsTextLabel
                                With TextLabel
                                    .TextFont = UnitLabelFont
                                    .SizeY = Settings.DisplayFont.SizeInPoints
                                    .Colour.Red = 1.0F
                                    .Colour.Green = 1.0F
                                    .Colour.Blue = 1.0F
                                    .Colour.Alpha = 1.0F
                                    .Pos.X = ScreenPos.X + 32
                                    .Pos.Y = ScreenPos.Y
                                    .Text = tmpUnit.Type.GetDisplayText
                                End With
                                UnitTextLabels.Add(TextLabel)
                            End If
                        End If
                    End If
                End If
            Next
        End Sub
    End Class
End Class