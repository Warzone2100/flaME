Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class ctrlMapView
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Private _Owner As frmMain

    Public DrawPending As Boolean

    Public OpenGLControl As OpenTK.GLControl

    Public GLSize As sXY_int
    Public GLSize_XPerY As Single

    Public DrawView_Enabled As Boolean = False

    Private GLInitializeDelayTimer As Timer
    Public IsGLInitialized As Boolean = False

    Private WithEvents tmrDraw As Timer
    Private WithEvents tmrDrawDelay As Timer

    Public Sub New(ByVal Owner As frmMain)

        _Owner = Owner

        InitializeComponent()

        ListSelect = New ContextMenuStrip
        UndoMessageTimer = New Timer

        OpenGLControl = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(32), 24, 0))
        Try
            OpenGLControl.MakeCurrent()
        Catch ex As Exception

        End Try
        pnlDraw.Controls.Add(OpenGLControl)

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

    Public Sub ResizeOpenGL()

        OpenGLControl.Width = pnlDraw.Width
        OpenGLControl.Height = pnlDraw.Height
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

    Private Sub GLInitialize(ByVal sender As Object, ByVal e As EventArgs)

        If OpenGLControl.Context Is Nothing Then
            Exit Sub
        End If

        IsGLInitialized = True

        GLInitializeDelayTimer.Enabled = False
        RemoveHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Dispose()
        GLInitializeDelayTimer = Nothing

        ResizeOpenGL()

        AddHandler OpenGLControl.MouseDown, AddressOf OpenGL_MouseDown
        AddHandler OpenGLControl.MouseUp, AddressOf OpenGL_MouseUp
        AddHandler OpenGLControl.MouseWheel, AddressOf OpenGL_MouseWheel
        AddHandler OpenGLControl.MouseMove, AddressOf OpenGL_MouseMove
        AddHandler OpenGLControl.MouseEnter, AddressOf OpenGL_MouseEnter
        AddHandler OpenGLControl.MouseLeave, AddressOf OpenGL_MouseLeave
        AddHandler OpenGLControl.Resize, AddressOf OpenGL_Resize
        AddHandler OpenGLControl.Leave, AddressOf OpenGL_LostFocus
        AddHandler OpenGLControl.PreviewKeyDown, AddressOf OpenGL_KeyDown
        AddHandler OpenGLControl.KeyUp, AddressOf OpenGL_KeyUp

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.PackAlignment, 1)
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
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

    Private Sub DrawView()

        If Not (DrawView_Enabled And IsGLInitialized) Then
            Exit Sub
        End If

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        Dim Map As clsMap = MainMap
        Dim BGColour As sRGB_sng

        If Map Is Nothing Then
            BGColour.Red = 0.5F
            BGColour.Green = 0.5F
            BGColour.Blue = 0.5F
        ElseIf Map.Tileset Is Nothing Then
            BGColour.Red = 0.5F
            BGColour.Green = 0.5F
            BGColour.Blue = 0.5F
        Else
            BGColour = Map.Tileset.BGColour
        End If

        GL.ClearColor(BGColour.Red, BGColour.Green, BGColour.Blue, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        If Map IsNot Nothing Then
            Map.GLDraw()
        End If

        GL.Flush()
        OpenGLControl.SwapBuffers()

        Refresh()
    End Sub

    Public Sub OpenGL_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.Map.ViewInfo.MouseOver = New clsViewInfo.clsMouseOver
        Map.ViewInfo.MouseOver.ScreenPos.X = e.X
        Map.ViewInfo.MouseOver.ScreenPos.Y = e.Y

        Map.ViewInfo.MouseOver_Pos_Calc()
    End Sub

    Public Sub Pos_Display_Update()
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        If MouseOverTerrain Is Nothing Then
            lblTile.Text = ""
            lblVertex.Text = ""
            lblPos.Text = ""
        Else
            lblTile.Text = "Tile x:" & MouseOverTerrain.Tile.Normal.X & ", y:" & MouseOverTerrain.Tile.Normal.Y
            lblVertex.Text = "Vertex  x:" & MouseOverTerrain.Vertex.Normal.X & ", y:" & MouseOverTerrain.Vertex.Normal.Y & ", alt:" & map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height * map.HeightMultiplier & " (" & map.Terrain.Vertices(MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y).Height & "x" & map.HeightMultiplier & ")"
            lblPos.Text = "Pos x:" & MouseOverTerrain.Pos.Horizontal.X & ", y:" & MouseOverTerrain.Pos.Horizontal.Y & ", alt:" & MouseOverTerrain.Pos.Altitude & ", slope: " & Math.Round(map.GetTerrainSlopeAngle(MouseOverTerrain.Pos.Horizontal) / RadOf1Deg * 10.0#) / 10.0# & "°"
        End If
    End Sub

    Public Sub OpenGL_LostFocus(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.SuppressMinimap = False

        Map.ViewInfo.MouseOver = Nothing
        Map.ViewInfo.MouseLeftDown = Nothing
        Map.ViewInfo.MouseRightDown = Nothing

        ViewKeyDown_Clear()
    End Sub

    Private WithEvents ListSelect As ContextMenuStrip
    Private ListSelectIsPicker As Boolean
    Private ListSelectItems(-1) As ToolStripItem

    Private Sub ListSelect_Click(ByVal Sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles ListSelect.ItemClicked
        Dim tmpButton As ToolStripItem = e.ClickedItem
        Dim tmpUnit As clsMap.clsUnit = CType(tmpButton.Tag, clsMap.clsUnit)

        If ListSelectIsPicker Then
            frmMainInstance.ObjectPicker(tmpUnit.Type)
        Else
            If tmpUnit.MapSelectedUnitLink.IsConnected Then
                tmpUnit.MapDeselect()
            Else
                tmpUnit.MapSelect()
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

    Private Sub OpenGL_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.MouseDown(e)
    End Sub

    Private Sub OpenGL_KeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim matrixA As New Matrix3D.Matrix3D
        Dim A As Integer
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        IsViewKeyDown.Keys(e.KeyCode) = True

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next

        If Control_Undo.Active Then
            Dim Message As String
            If Map.UndoPosition > 0 Then
                Message = "Undid: " & Map.Undos.Item(Map.UndoPosition - 1).Name
                Dim MapMessage As New clsMap.clsMessage
                MapMessage.Text = Message
                Map.Messages.Add(MapMessage)
                Map.Undo_Perform()
                DrawViewLater()
            Else
                Message = "Nothing to undo"
            End If
            DisplayUndoMessage(Message)
        End If
        If Control_Redo.Active Then
            Dim Message As String
            If Map.UndoPosition < Map.Undos.ItemCount Then
                Message = "Redid: " & Map.Undos.Item(Map.UndoPosition).Name
                Dim MapMessage As New clsMap.clsMessage
                MapMessage.Text = Message
                Map.Messages.Add(MapMessage)
                Map.Redo_Perform()
                DrawViewLater()
            Else
                Message = "Nothing to redo"
            End If
            DisplayUndoMessage(Message)
        End If
        If IsViewKeyDown.Keys(Keys.ControlKey) Then
            If e.KeyCode = Keys.D1 Then
                VisionRadius_2E = 6
            ElseIf e.KeyCode = Keys.D2 Then
                VisionRadius_2E = 7
            ElseIf e.KeyCode = Keys.D3 Then
                VisionRadius_2E = 8
            ElseIf e.KeyCode = Keys.D4 Then
                VisionRadius_2E = 9
            ElseIf e.KeyCode = Keys.D5 Then
                VisionRadius_2E = 10
            ElseIf e.KeyCode = Keys.D6 Then
                VisionRadius_2E = 11
            ElseIf e.KeyCode = Keys.D7 Then
                VisionRadius_2E = 12
            ElseIf e.KeyCode = Keys.D8 Then
                VisionRadius_2E = 13
            ElseIf e.KeyCode = Keys.D9 Then
                VisionRadius_2E = 14
            ElseIf e.KeyCode = Keys.D0 Then
                VisionRadius_2E = 15
            End If
            VisionRadius_2E_Changed()
        End If

        If Control_View_Move_Type.Active Then
            If ViewMoveType = enumView_Move_Type.Free Then
                ViewMoveType = enumView_Move_Type.RTS
            ElseIf ViewMoveType = enumView_Move_Type.RTS Then
                ViewMoveType = enumView_Move_Type.Free
            End If
        End If
        If Control_View_Rotate_Type.Active Then
            RTSOrbit = Not RTSOrbit
        End If
        If Control_View_Reset.Active Then
            Map.ViewInfo.FOV_Multiplier_Set(Settings.FOVDefault)
            If ViewMoveType = enumView_Move_Type.Free Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                Map.ViewInfo.ViewAngleSet_Rotate(matrixA)
            ElseIf ViewMoveType = enumView_Move_Type.RTS Then
                Matrix3D.MatrixSetToXAngle(matrixA, Math.Atan(2.0#))
                Map.ViewInfo.ViewAngleSet_Rotate(matrixA)
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
            Dim X As Integer
            Dim Y As Integer
            Dim SectorNum As sXY_int
            Dim tmpUnit As clsMap.clsUnit
            For Y = 0 To Map.SectorCount.Y - 1
                For X = 0 To Map.SectorCount.X - 1
                    For A = 0 To Map.Sectors(X, Y).Units.ItemCount - 1
                        tmpUnit = Map.Sectors(X, Y).Units.Item(A).Unit
                        If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                            If CType(tmpUnit.Type, clsStructureType).StructureBasePlate IsNot Nothing Then
                                SectorNum.X = X
                                SectorNum.Y = Y
                                Map.SectorGraphicsChanges.Changed(SectorNum)
                                Exit For
                            End If
                        End If
                    Next
                Next
            Next
            Map.Update()
            DrawViewLater()
        End If
        If Control_View_ScriptMarkers.Active Then
            Draw_ScriptMarkers = Not Draw_ScriptMarkers
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
                    Map.ViewInfo.Apply_Texture_Clockwise()
                End If
                If Control_CounterClockwise.Active Then
                    Map.ViewInfo.Apply_Texture_CounterClockwise()
                End If
                If Control_Texture_Flip.Active Then
                    Map.ViewInfo.Apply_Texture_FlipX()
                End If
                If Control_Tri_Flip.Active Then
                    Map.ViewInfo.Apply_Tri_Flip()
                End If
            End If
        End If
        If Tool = enumTool.None Then
            If Control_Unit_Delete.Active Then
                If Map.SelectedUnits.ItemCount > 0 Then
                    Dim OldUnits As SimpleClassList(Of clsMap.clsUnit) = Map.SelectedUnits.GetItemsAsSimpleClassList
                    For A = 0 To OldUnits.ItemCount - 1
                        Map.Unit_Remove_StoreChange(OldUnits.Item(A).MapLink.ArrayPosition)
                    Next
                    frmMainInstance.SelectedObject_Changed()
                    Map.UndoStepCreate("Object Deleted")
                    Map.Update()
                    Map.MinimapMakeLater()
                    DrawViewLater()
                End If
            End If
            If Control_Unit_Move.Active Then
                If MouseOverTerrain IsNot Nothing Then
                    If Map.SelectedUnits.ItemCount > 0 Then
                        Dim Centre As sXY_dbl = GetUnitsCentrePos(Map.SelectedUnits.GetItemsAsSimpleClassList)
                        Dim Offset As sXY_int
                        Offset.X = CInt(Math.Round((MouseOverTerrain.Pos.Horizontal.X - Centre.X) / TerrainGridSpacing)) * TerrainGridSpacing
                        Offset.Y = CInt(Math.Round((MouseOverTerrain.Pos.Horizontal.Y - Centre.Y) / TerrainGridSpacing)) * TerrainGridSpacing
                        Dim ObjectPosOffset As New clsMap.clsObjectPosOffset
                        ObjectPosOffset.Map = Map
                        ObjectPosOffset.Offset = Offset
                        Map.SelectedUnitsAction(ObjectPosOffset)

                        Map.UndoStepCreate("Objects Moved")
                        Map.Update()
                        Map.MinimapMakeLater()
                        frmMainInstance.SelectedObject_Changed()
                        DrawViewLater()
                    End If
                End If
            End If
            If Control_Clockwise.Active Then
                Dim ObjectRotationOffset As New clsMap.clsObjectRotationOffset
                ObjectRotationOffset.Map = Map
                ObjectRotationOffset.Offset = -90
                Map.SelectedUnitsAction(ObjectRotationOffset)
                Map.Update()
                frmMainInstance.SelectedObject_Changed()
                Map.UndoStepCreate("Object Rotated")
                DrawViewLater()
            End If
            If Control_CounterClockwise.Active Then
                Dim ObjectRotationOffset As New clsMap.clsObjectRotationOffset
                ObjectRotationOffset.Map = Map
                ObjectRotationOffset.Offset = 90
                Map.SelectedUnitsAction(ObjectRotationOffset)
                Map.Update()
                frmMainInstance.SelectedObject_Changed()
                Map.UndoStepCreate("Object Rotated")
                DrawViewLater()
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

    Private Sub OpenGL_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()

        Map.SuppressMinimap = False

        If e.Button = Windows.Forms.MouseButtons.Left Then
            If Map.ViewInfo.GetMouseLeftDownOverMinimap() IsNot Nothing Then

            Else
                Select Case Tool
                    Case enumTool.AutoTexture_Place
                        Map.UndoStepCreate("Ground Painted")
                    Case enumTool.CliffTriangle
                        Map.UndoStepCreate("Cliff Triangles")
                    Case enumTool.AutoCliff
                        Map.UndoStepCreate("Cliff Brush")
                    Case enumTool.AutoCliffRemove
                        Map.UndoStepCreate("Cliff Remove Brush")
                    Case enumTool.Height_Change_Brush
                        Map.UndoStepCreate("Height Change")
                    Case enumTool.Height_Set_Brush
                        Map.UndoStepCreate("Height Set")
                    Case enumTool.Height_Smooth_Brush
                        Map.UndoStepCreate("Height Smooth")
                    Case enumTool.Texture_Brush
                        Map.UndoStepCreate("Texture")
                    Case enumTool.AutoRoad_Remove
                        Map.UndoStepCreate("Road Remove")
                    Case enumTool.None
                        If Map.Unit_Selected_Area_VertexA IsNot Nothing Then
                            If MouseOverTerrain IsNot Nothing Then
                                SelectUnits(Map.Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal)
                            End If
                            Map.Unit_Selected_Area_VertexA = Nothing
                        End If
                End Select
            End If
            Map.ViewInfo.MouseLeftDown = Nothing
        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
            If Map.ViewInfo.GetMouseRightDownOverMinimap() IsNot Nothing Then

            Else
                Select Case Tool
                    Case enumTool.Height_Change_Brush
                        Map.UndoStepCreate("Height Change")
                    Case enumTool.Height_Set_Brush
                        Map.UndoStepCreate("Height Set")
                End Select
            End If
            Map.ViewInfo.MouseRightDown = Nothing
        End If
    End Sub

    Private Sub SelectUnits(ByVal VertexA As sXY_int, ByVal VertexB As sXY_int)
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain()
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
            If MouseOverTerrain.Units.ItemCount > 0 Then
                If MouseOverTerrain.Units.ItemCount = 1 Then
                    tmpUnit = MouseOverTerrain.Units.Item(0)
                    If tmpUnit.MapSelectedUnitLink.IsConnected Then
                        tmpUnit.MapDeselect()
                    Else
                        tmpUnit.MapSelect()
                    End If
                Else
                    ListSelect.Close()
                    ListSelect.Items.Clear()
                    ReDim ListSelectItems(MouseOverTerrain.Units.ItemCount - 1)
                    For A = 0 To MouseOverTerrain.Units.ItemCount - 1
                        If MouseOverTerrain.Units.Item(A).Type Is Nothing Then
                            ListSelectItems(A) = New ToolStripButton("<nothing>")
                        Else
                            ListSelectItems(A) = New ToolStripButton(MouseOverTerrain.Units.Item(A).Type.GetDisplayText)
                        End If
                        ListSelectItems(A).Tag = MouseOverTerrain.Units.Item(A)
                        ListSelect.Items.Add(ListSelectItems(A))
                    Next
                    ListSelectIsPicker = False
                    ListSelect.Show(Me, New Drawing.Point(Map.ViewInfo.MouseOver.ScreenPos.X, Map.ViewInfo.MouseOver.ScreenPos.Y))
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
                    For A = 0 To Map.Sectors(SectorNum.X, SectorNum.Y).Units.ItemCount - 1
                        tmpUnit = Map.Sectors(SectorNum.X, SectorNum.Y).Units.Item(A).Unit
                        If tmpUnit.Pos.Horizontal.X >= StartPos.X And tmpUnit.Pos.Horizontal.Y >= StartPos.Y And _
                            tmpUnit.Pos.Horizontal.X <= FinishPos.X And tmpUnit.Pos.Horizontal.Y <= FinishPos.Y Then
                            If Not tmpUnit.MapSelectedUnitLink.IsConnected Then
                                tmpUnit.MapSelect()
                            End If
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
            ResizeOpenGL()
        End If
    End Sub

    Public Sub OpenGL_Resize(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        Dim Map As clsMap = MainMap

        GLSize.X = OpenGLControl.Width
        GLSize.Y = OpenGLControl.Height
        If GLSize.Y <> 0 Then
            GLSize_XPerY = CSng(GLSize.X / GLSize.Y)
        End If
        Viewport_Resize()
        If Map IsNot Nothing Then
            Map.ViewInfo.FOV_Calc()
        End If
        DrawViewLater()
    End Sub

    Public Sub OpenGL_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs)

        If Form.ActiveForm Is frmMainInstance Then
            OpenGLControl.Focus()
        End If
    End Sub

    Public Sub OpenGL_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Move As sXYZ_int
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim A As Integer

        For A = 0 To CInt(Math.Abs(e.Delta / 120.0#))
            Matrix3D.VectorForwardsRotationByMatrix(Map.ViewInfo.ViewAngleMatrix, Math.Sign(e.Delta) * Math.Max(Map.ViewInfo.ViewPos.Y, 512.0#) / 24.0#, XYZ_dbl)
            Move.Set_dbl(XYZ_dbl)
            Map.ViewInfo.ViewPosChange(Move)
        Next
    End Sub

    Public Function CreateGLFont(ByVal BaseFont As Font) As GLFont

        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

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

    Private Sub OpenGL_MouseLeave(sender As Object, e As System.EventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Map.ViewInfo.MouseOver = Nothing
    End Sub

    Public Sub ListSelectBegin()
        Dim Map As clsMap = MainMap
        Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = Map.ViewInfo.GetMouseOverTerrain

        If MouseOverTerrain Is Nothing Then
            Stop
            Exit Sub
        End If

        Dim A As Integer

        ListSelect.Close()
        ListSelect.Items.Clear()
        ReDim ListSelectItems(MouseOverTerrain.Units.ItemCount - 1)
        For A = 0 To MouseOverTerrain.Units.ItemCount - 1
            ListSelectItems(A) = New ToolStripButton(MouseOverTerrain.Units.Item(A).Type.GetDisplayText)
            ListSelectItems(A).Tag = MouseOverTerrain.Units.Item(A)
            ListSelect.Items.Add(ListSelectItems(A))
        Next
        ListSelectIsPicker = True
        ListSelect.Show(Me, New Drawing.Point(Map.ViewInfo.MouseOver.ScreenPos.X, Map.ViewInfo.MouseOver.ScreenPos.Y))
    End Sub

    Private Sub tabMaps_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tabMaps.SelectedIndexChanged

        If Not tabMaps.Enabled Then
            Exit Sub
        End If

        If tabMaps.SelectedTab Is Nothing Then
            _Owner.SetMainMap(Nothing)
            Exit Sub
        End If

        Dim Map As clsMap = CType(tabMaps.SelectedTab.Tag, clsMap)

        _Owner.SetMainMap(Map)
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If
        If Not Map.frmMainLink.IsConnected Then
            MsgBox("Error: Map should be closed already.")
            Exit Sub
        End If

        If Map.ChangedSinceSave Then
            If MsgBox("Lose any unsaved changes to this map?", MsgBoxStyle.OkCancel, Map.GetTitle) <> MsgBoxResult.Ok Then
                Exit Sub
            End If
        End If

        Map.Deallocate()
    End Sub

    Public Sub UpdateTabs()
        Dim A As Integer
        Dim Map As clsMap

        tabMaps.Enabled = False
        tabMaps.TabPages.Clear()
        For A = 0 To _Owner.LoadedMaps.ItemCount - 1
            Map = _Owner.LoadedMaps.Item(A)
            tabMaps.TabPages.Add(Map.MapView_TabPage)
        Next
        Map = MainMap
        If Map IsNot Nothing Then
            tabMaps.SelectedIndex = Map.frmMainLink.ArrayPosition
        Else
            tabMaps.SelectedIndex = -1
        End If
        tabMaps.Enabled = True
    End Sub

    Private ReadOnly Property MainMap As clsMap
        Get
            Return _Owner.MainMap
        End Get
    End Property

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.ssStatus = New System.Windows.Forms.StatusStrip()
        Me.lblTile = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblVertex = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblPos = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblUndo = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.tabMaps = New System.Windows.Forms.TabControl()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.ssStatus.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
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
        Me.pnlDraw.Location = New System.Drawing.Point(0, 28)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(1308, 364)
        Me.pnlDraw.TabIndex = 1
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.pnlDraw, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1308, 392)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'tabMaps
        '
        Me.tabMaps.Appearance = System.Windows.Forms.TabAppearance.Buttons
        Me.tabMaps.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMaps.Location = New System.Drawing.Point(3, 3)
        Me.tabMaps.Name = "tabMaps"
        Me.tabMaps.SelectedIndex = -1
        Me.tabMaps.Size = New System.Drawing.Size(1270, 22)
        Me.tabMaps.TabIndex = 2
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.tabMaps, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnClose, 1, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(1308, 28)
        Me.TableLayoutPanel2.TabIndex = 2
        '
        'btnClose
        '
        Me.btnClose.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnClose.Location = New System.Drawing.Point(1276, 0)
        Me.btnClose.Margin = New System.Windows.Forms.Padding(0)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(32, 28)
        Me.btnClose.TabIndex = 3
        Me.btnClose.Text = "X"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'ctrlMapView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.ssStatus)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlMapView"
        Me.Size = New System.Drawing.Size(1308, 424)
        Me.ssStatus.ResumeLayout(False)
        Me.ssStatus.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents ssStatus As System.Windows.Forms.StatusStrip
    Public WithEvents lblTile As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblVertex As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblPos As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents pnlDraw As System.Windows.Forms.Panel
    Public WithEvents lblUndo As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tabMaps As System.Windows.Forms.TabControl
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnClose As System.Windows.Forms.Button
#End If
End Class