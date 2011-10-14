Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class ctrlTextureView
#If MonoDevelop <> 0.0# Then
    Inherits UserControl
#End If

    Public DrawPending As Boolean

    Public OpenGLControl As OpenTK.GLControl

    Public GLSize As sXY_int
    Public GLSize_XPerY As Double

    Public View_Pos As sXY_int

    Public TextureCount As sXY_int
    Public TextureYOffset As Integer

    Public DrawView_Enabled As Boolean = False

    Public DisplayTileTypes As Boolean = False
    Public DisplayTileNumbers As Boolean = False

    Private GLInitializeDelayTimer As Timer
    Public IsGLInitialized As Boolean = False

    Private WithEvents tmrDraw As Timer
    Private WithEvents tmrDrawDelay As Timer

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OpenGLControl = New OpenTK.GLControl(New GraphicsMode(New ColorFormat(32), 0, 0))
        OpenGLControl.MakeCurrent() 'mono version fails without this
        pnlDraw.Controls.Add(OpenGLControl)

        GLInitializeDelayTimer = New Timer
        GLInitializeDelayTimer.Interval = 1
        AddHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Enabled = True

        tmrDraw = New Timer
        tmrDraw.Interval = 1

        tmrDrawDelay = New Timer
        tmrDrawDelay.Interval = 30
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
            DrawPending = False
            DrawView()
            tmrDrawDelay.Enabled = True
        End If
    End Sub

    Private Sub GLInitialize(ByVal sender As Object, ByVal e As EventArgs)

        IsGLInitialized = True

        GLInitializeDelayTimer.Enabled = False
        RemoveHandler GLInitializeDelayTimer.Tick, AddressOf GLInitialize
        GLInitializeDelayTimer.Dispose()
        GLInitializeDelayTimer = Nothing

        OpenGL_Size_Calc()

        AddHandler OpenGLControl.MouseDown, AddressOf OpenGL_MouseDown
        AddHandler OpenGLControl.Resize, AddressOf OpenGL_Resize
        AddHandler OpenGLControl.Paint, AddressOf OpenGL_Paint

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
        GL.Enable(EnableCap.CullFace)
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

        GL.Clear(ClearBufferMask.ColorBufferBit)

        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            GL.Flush()
            OpenGLControl.SwapBuffers()
            Refresh()
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim Num As Integer
        Dim XY_int As sXY_int
        Dim A As Integer
        Dim Vertex0 As sXY_sng
        Dim Vertex1 As sXY_sng
        Dim Vertex2 As sXY_sng
        Dim UnrotatedPos As sXY_sng
        Dim TexCoord0 As sXY_sng
        Dim TexCoord1 As sXY_sng
        Dim TexCoord2 As sXY_sng
        Dim TexCoord3 As sXY_sng

        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, CSng(GLSize.X), CSng(GLSize.Y), 0.0F, -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        If Map.Tileset IsNot Nothing Then

            GetTileRotatedTexCoords(TextureOrientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

            GL.Enable(EnableCap.Texture2D)
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Decal)
            GL.Color4(0.0F, 0.0F, 0.0F, 1.0F)

            For Y = 0 To TextureCount.Y - 1
                For X = 0 To TextureCount.X - 1
                    Num = (TextureYOffset + Y) * TextureCount.X + X
                    If Num >= Map.Tileset.TileCount Then
                        GoTo EndOfTextures1
                    End If
                    A = Map.Tileset.Tiles(Num).TextureView_GL_Texture_Num
                    If A = 0 Then
                        GL.BindTexture(TextureTarget.Texture2D, 0)
                    Else
                        GL.BindTexture(TextureTarget.Texture2D, A)
                    End If
                    GL.Begin(BeginMode.Quads)
                    GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                    GL.Vertex2(X * 64.0#, Y * 64.0#)
                    GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                    GL.Vertex2(X * 64.0#, Y * 64.0# + 64.0#)
                    GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                    GL.Vertex2(X * 64.0# + 64.0#, Y * 64.0# + 64.0#)
                    GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                    GL.Vertex2(X * 64.0# + 64.0#, Y * 64.0#)
                    GL.End()
                Next
            Next

EndOfTextures1:

            GL.Disable(EnableCap.Texture2D)

            If DisplayTileTypes Then
                GL.Begin(BeginMode.Quads)
                For Y = 0 To TextureCount.Y - 1
                    For X = 0 To TextureCount.X - 1
                        Num = (TextureYOffset + Y) * TextureCount.X + X
                        If Num >= Map.Tileset.TileCount Then
                            GoTo EndOfTextures2
                        End If
                        A = Map.Tile_TypeNum(Num)
                        GL.Color3(TileTypes(A).DisplayColour.Red, TileTypes(A).DisplayColour.Green, TileTypes(A).DisplayColour.Blue)
                        GL.Vertex2(X * 64.0# + 24.0#, Y * 64.0# + 24.0#)
                        GL.Vertex2(X * 64.0# + 24.0#, Y * 64.0# + 40.0#)
                        GL.Vertex2(X * 64.0# + 40.0#, Y * 64.0# + 40.0#)
                        GL.Vertex2(X * 64.0# + 40.0#, Y * 64.0# + 24.0#)
                    Next
                Next
EndOfTextures2:
                GL.End()
            End If

            If DisplayTileOrientation Then
                GL.Disable(EnableCap.CullFace)

                UnrotatedPos.X = 0.25F
                UnrotatedPos.Y = 0.25F
                Vertex0 = GetTileRotatedPos_sng(TextureOrientation, UnrotatedPos)
                UnrotatedPos.X = 0.5F
                UnrotatedPos.Y = 0.25F
                Vertex1 = GetTileRotatedPos_sng(TextureOrientation, UnrotatedPos)
                UnrotatedPos.X = 0.5F
                UnrotatedPos.Y = 0.5F
                Vertex2 = GetTileRotatedPos_sng(TextureOrientation, UnrotatedPos)

                GL.Begin(BeginMode.Triangles)
                GL.Color3(1.0F, 1.0F, 0.0F)
                For Y = 0 To TextureCount.Y - 1
                    For X = 0 To TextureCount.X - 1
                        Num = (TextureYOffset + Y) * TextureCount.X + X
                        If Num >= Map.Tileset.TileCount Then
                            GoTo EndOfTextures3
                        End If
                        GL.Vertex2(X * 64.0# + Vertex0.X * 64.0#, Y * 64.0# + Vertex0.Y * 64.0#)
                        GL.Vertex2(X * 64.0# + Vertex2.X * 64.0#, Y * 64.0# + Vertex2.Y * 64.0#)
                        GL.Vertex2(X * 64.0# + Vertex1.X * 64.0#, Y * 64.0# + Vertex1.Y * 64.0#)
                    Next
                Next
EndOfTextures3:
                GL.End()

                GL.Enable(EnableCap.CullFace)
            End If

            If DisplayTileNumbers And TextureViewFont IsNot Nothing Then
                Dim TextLabel As clsTextLabel
                GL.Enable(EnableCap.Texture2D)
                For Y = 0 To TextureCount.Y - 1
                    For X = 0 To TextureCount.X - 1
                        Num = (TextureYOffset + Y) * TextureCount.X + X
                        If Num >= Map.Tileset.TileCount Then
                            GoTo EndOfTextures4
                        End If
                        TextLabel = New clsTextLabel
                        TextLabel.Text = InvariantToString_int(Num)
                        TextLabel.SizeY = 24.0F
                        TextLabel.Colour.Red = 1.0F
                        TextLabel.Colour.Green = 1.0F
                        TextLabel.Colour.Blue = 0.0F
                        TextLabel.Colour.Alpha = 1.0F
                        TextLabel.Pos.X = X * 64
                        TextLabel.Pos.Y = Y * 64
                        TextLabel.TextFont = TextureViewFont
                        TextLabel.Draw()
                    Next
                Next
EndOfTextures4:
                GL.Disable(EnableCap.Texture2D)
            End If

            If SelectedTextureNum >= 0 And TextureCount.X > 0 Then
                A = SelectedTextureNum - TextureYOffset * TextureCount.X
                XY_int.X = A - CInt(Int(A / TextureCount.X)) * TextureCount.X
                XY_int.Y = CInt(Int(A / TextureCount.X))
                GL.Begin(BeginMode.LineLoop)
                GL.Color3(1.0F, 1.0F, 0.0F)
                GL.Vertex2(XY_int.X * 64.0#, XY_int.Y * 64.0#)
                GL.Vertex2(XY_int.X * 64.0#, XY_int.Y * 64.0# + 64.0#)
                GL.Vertex2(XY_int.X * 64.0# + 64.0#, XY_int.Y * 64.0# + 64.0#)
                GL.Vertex2(XY_int.X * 64.0# + 64.0#, XY_int.Y * 64.0#)
                GL.End()
            End If
        End If

        GL.Flush()
        OpenGLControl.SwapBuffers()

        Refresh()
    End Sub

    Public Sub OpenGL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        If Map Is Nothing Then
            SelectedTextureNum = -1
        ElseIf Map.Tileset Is Nothing Then
            SelectedTextureNum = -1
        ElseIf e.X >= 0 And e.X < TextureCount.X * 64 _
          And e.Y >= 0 And e.Y < TextureCount.Y * 64 Then
            SelectedTextureNum = (TextureYOffset + CInt(Int(e.Y / 64.0#))) * TextureCount.X + CInt(Int(e.X / 64.0#))
            If SelectedTextureNum >= Map.Tileset.TileCount Then
                SelectedTextureNum = -1
            Else
                Tool = enumTool.Texture_Brush
            End If
        Else
            SelectedTextureNum = -1
        End If

        If SelectedTextureNum >= 0 Then
            frmMainInstance.cboTileType.Enabled = False
            frmMainInstance.cboTileType.SelectedIndex = Map.Tile_TypeNum(SelectedTextureNum)
            frmMainInstance.cboTileType.Enabled = True
        Else
            frmMainInstance.cboTileType.Enabled = False
            frmMainInstance.cboTileType.SelectedIndex = -1
        End If

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
            TextureCount.X = CInt(Math.Floor(GLSize.X / 64.0#))
            TextureCount.Y = CInt(Math.Ceiling(GLSize.Y / 64.0#))
        Else
            TextureCount.X = 0
            TextureCount.Y = 0
        End If

        ScrollUpdate()
    End Sub

    Public Sub ScrollUpdate()
        Dim Map As clsMap = MainMap

        If Map Is Nothing Then
            Exit Sub
        End If

        Dim Flag As Boolean

        If TextureCount.X > 0 And TextureCount.Y > 0 Then
            If Map Is Nothing Then
                Flag = True
            ElseIf Map.Tileset Is Nothing Then
                Flag = True
            Else
                Flag = False
            End If
        Else
            Flag = True
        End If
        If Flag Then
            TextureScroll.Maximum = 0
            TextureScroll.LargeChange = 0
            TextureScroll.Enabled = False
        Else
            TextureScroll.Maximum = CInt(Math.Ceiling(Map.Tileset.TileCount / TextureCount.X))
            TextureScroll.LargeChange = TextureCount.Y
            TextureScroll.Enabled = True
        End If
    End Sub

    Public Sub OpenGL_Resize(ByVal sender As Object, ByVal e As System.EventArgs)

        GLSize.X = OpenGLControl.Width
        GLSize.Y = OpenGLControl.Height
        If GLSize.Y <> 0 Then
            GLSize_XPerY = GLSize.X / GLSize.Y
        End If
        Viewport_Resize()
    End Sub

    Private Sub TextureScroll_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextureScroll.ValueChanged

        TextureYOffset = TextureScroll.Value

        DrawViewLater()
    End Sub

    Public Function CreateGLFont(ByVal BaseFont As Font) As GLFont

        If GraphicsContext.CurrentContext IsNot OpenGLControl.Context Then
            OpenGLControl.MakeCurrent()
        End If

        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

    Private Sub OpenGL_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)

        DrawViewLater()
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TextureScroll = New System.Windows.Forms.VScrollBar()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TextureScroll, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.pnlDraw, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(280, 384)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'TextureScroll
        '
        Me.TextureScroll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextureScroll.Location = New System.Drawing.Point(259, 0)
        Me.TextureScroll.Name = "TextureScroll"
        Me.TextureScroll.Size = New System.Drawing.Size(21, 384)
        Me.TextureScroll.TabIndex = 1
        '
        'pnlDraw
        '
        Me.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDraw.Location = New System.Drawing.Point(0, 0)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(259, 384)
        Me.pnlDraw.TabIndex = 2
        '
        'ctrlTextureView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlTextureView"
        Me.Size = New System.Drawing.Size(280, 384)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents TextureScroll As System.Windows.Forms.VScrollBar
    Public WithEvents pnlDraw As System.Windows.Forms.Panel
#End If
End Class