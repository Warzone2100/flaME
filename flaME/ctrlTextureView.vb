Imports OpenTK.Graphics.OpenGL

Public Class ctrlTextureView
#If OS <> "Windows" Then
    Inherits UserControl
#End If

    Public DrawPending As Boolean

    Public GL_Num As Byte = 44

    Public OpenGL As OpenTK.GLControl

    Public GLSize As sXY_int

    Public ScreenSize_X_Per_Y As Double

    Public View_Pos As sXY_int

    Public TextureCount As sXY_int
    Public TextureYOffset As Integer

    Public DrawView_Enabled As Boolean = False

    Public DisplayTileTypes As Boolean = False
    Public DisplayTileNumbers As Boolean = False

    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OpenGL = New OpenTK.GLControl(New OpenTK.Graphics.GraphicsMode(New OpenTK.Graphics.ColorFormat(32), 0, 0, 1))
        GLInitialize()
        OpenGL_Size_Calc()
        pnlDraw.Controls.Add(OpenGL)

        AddHandler OpenGL.MouseDown, AddressOf OpenGL_MouseDown
        AddHandler OpenGL.Resize, AddressOf OpenGL_Resize
        AddHandler OpenGL.Paint, AddressOf OpenGL_Paint
    End Sub

    Sub OpenGL_Size_Calc()

        OpenGL.Width = pnlDraw.Width
        OpenGL.Height = pnlDraw.Height

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
            DrawPending = False
            DrawView()
            tmrDraw_Delay.Enabled = True
        End If
    End Sub

    Private Sub GLInitialize()

        If GL_Current <> GL_Num Then
            OpenGL.MakeCurrent()
            GL_Current = GL_Num
        End If

        GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
    End Sub

    Sub Viewport_Resize()

        If GL_Current <> GL_Num Then
            OpenGL.MakeCurrent()
            GL_Current = GL_Num
        End If
        GL.Viewport(0, 0, GLSize.X, GLSize.Y)

        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Flush()
        OpenGL.SwapBuffers()
        Refresh()

        DrawViewLater()
    End Sub

    Private Sub DrawView()
        Static X As Integer
        Static Y As Integer
        Static Num As Integer
        Static XY_int As sXY_int
        Static A As Integer
        Static Vertex0 As sXY_sng
        Static Vertex1 As sXY_sng
        Static Vertex2 As sXY_sng
        Static Vertex3 As sXY_sng
        Static UnrotatedPos As sXY_sng

        If Not DrawView_Enabled Then
            Exit Sub
        End If

        If GL_Current <> GL_Num Then
            OpenGL.MakeCurrent()
            GL_Current = GL_Num
        End If

        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, CSng(GLSize.X), CSng(GLSize.Y), 0.0F, -1.0F, 1.0F))
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)

        If Map.Tileset IsNot Nothing Then

            UnrotatedPos.X = 0.0F
            UnrotatedPos.Y = 0.0F
            Vertex0 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)
            UnrotatedPos.X = 1.0F
            UnrotatedPos.Y = 0.0F
            Vertex1 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)
            UnrotatedPos.X = 0.0F
            UnrotatedPos.Y = 1.0F
            Vertex2 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)
            UnrotatedPos.X = 1.0F
            UnrotatedPos.Y = 1.0F
            Vertex3 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)

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
                    GL.TexCoord2(0.0F, 0.0F)
                    GL.Vertex2(X * 64.0# + Vertex0.X * 64.0#, Y * 64.0# + Vertex0.Y * 64.0#)
                    GL.TexCoord2(1.0F, 0.0F)
                    GL.Vertex2(X * 64.0# + Vertex1.X * 64.0#, Y * 64.0# + Vertex1.Y * 64.0#)
                    GL.TexCoord2(1.0F, 1.0F)
                    GL.Vertex2(X * 64.0# + Vertex3.X * 64.0#, Y * 64.0# + Vertex3.Y * 64.0#)
                    GL.TexCoord2(0.0F, 1.0F)
                    GL.Vertex2(X * 64.0# + Vertex2.X * 64.0#, Y * 64.0# + Vertex2.Y * 64.0#)
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
                        GL.Vertex2(X * 64.0# + 40.0#, Y * 64.0# + 24.0#)
                        GL.Vertex2(X * 64.0# + 40.0#, Y * 64.0# + 40.0#)
                        GL.Vertex2(X * 64.0# + 24.0#, Y * 64.0# + 40.0#)
                    Next
                Next
EndOfTextures2:
                GL.End()
            End If

            If DisplayTileOrientation Then
                UnrotatedPos.X = 0.25F
                UnrotatedPos.Y = 0.25F
                Vertex0 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)
                UnrotatedPos.X = 0.5F
                UnrotatedPos.Y = 0.25F
                Vertex1 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)
                UnrotatedPos.X = 0.5F
                UnrotatedPos.Y = 0.5F
                Vertex2 = GetTileRotatedPos(TextureOrientation, UnrotatedPos)

                GL.Begin(BeginMode.Triangles)
                GL.Color3(1.0F, 1.0F, 0.0F)
                For Y = 0 To TextureCount.Y - 1
                    For X = 0 To TextureCount.X - 1
                        Num = (TextureYOffset + Y) * TextureCount.X + X
                        If Num >= Map.Tileset.TileCount Then
                            GoTo EndOfTextures3
                        End If
                        GL.Vertex2(X * 64.0# + Vertex0.X * 64.0#, Y * 64.0# + Vertex0.Y * 64.0#)
                        GL.Vertex2(X * 64.0# + Vertex1.X * 64.0#, Y * 64.0# + Vertex1.Y * 64.0#)
                        GL.Vertex2(X * 64.0# + Vertex2.X * 64.0#, Y * 64.0# + Vertex2.Y * 64.0#)
                    Next
                Next
EndOfTextures3:
                GL.End()
            End If

            If DisplayTileNumbers And TextureViewFont IsNot Nothing Then
                Dim TextLabel As sTextLabel
                GL.Enable(EnableCap.Texture2D)
                For Y = 0 To TextureCount.Y - 1
                    For X = 0 To TextureCount.X - 1
                        Num = (TextureYOffset + Y) * TextureCount.X + X
                        If Num >= Map.Tileset.TileCount Then
                            GoTo EndOfTextures4
                        End If
                        TextLabel = New sTextLabel
                        TextLabel.Text = Num
                        TextLabel.SizeY = 24.0F
                        TextLabel.Colour.Red = 1.0F
                        TextLabel.Colour.Green = 1.0F
                        TextLabel.Colour.Blue = 0.0F
                        TextLabel.Colour.Alpha = 1.0F
                        TextLabel.Pos.X = X * 64.0#
                        TextLabel.Pos.Y = Y * 64.0#
                        TextLabel.Font = TextureViewFont
                        Draw_TextLabel(TextLabel)
                    Next
                Next
EndOfTextures4:
                GL.Disable(EnableCap.Texture2D)
            End If

            If SelectedTexture >= 0 And TextureCount.X > 0 Then
                A = SelectedTexture - TextureYOffset * TextureCount.X
                XY_int.X = A - Int(A / TextureCount.X) * TextureCount.X
                XY_int.Y = Int(A / TextureCount.X)
                GL.Begin(BeginMode.LineLoop)
                GL.Color3(1.0F, 1.0F, 0.0F)
                GL.Vertex2(XY_int.X * 64.0#, XY_int.Y * 64.0#)
                GL.Vertex2(XY_int.X * 64.0# + 64.0#, XY_int.Y * 64.0#)
                GL.Vertex2(XY_int.X * 64.0# + 64.0#, XY_int.Y * 64.0# + 64.0#)
                GL.Vertex2(XY_int.X * 64.0#, XY_int.Y * 64.0# + 64.0#)
                GL.End()
            End If
        End If

        GL.Flush()
        OpenGL.SwapBuffers()

        Refresh()
    End Sub

    Sub OpenGL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        If Map Is Nothing Then
            SelectedTexture = -1
        ElseIf Map.Tileset Is Nothing Then
            SelectedTexture = -1
        ElseIf e.X >= 0 And e.X < TextureCount.X * 64 _
          And e.Y >= 0 And e.Y < TextureCount.Y * 64 Then
            SelectedTexture = (TextureYOffset + Int(e.Y / 64.0#)) * TextureCount.X + Int(e.X / 64.0#)
            If SelectedTexture >= Map.Tileset.TileCount Then
                SelectedTexture = -1
            Else
                Tool = enumTool.Texture_Brush
            End If
        Else
            SelectedTexture = -1
        End If

        If SelectedTexture >= 0 Then
            frmMainInstance.cmbTileType.Enabled = False
            frmMainInstance.cmbTileType.SelectedIndex = Map.Tile_TypeNum(SelectedTexture)
            frmMainInstance.cmbTileType.Enabled = True
        Else
            frmMainInstance.cmbTileType.Enabled = False
            frmMainInstance.cmbTileType.SelectedIndex = -1
        End If

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

        If OpenGL IsNot Nothing Then
            OpenGL_Size_Calc()
            TextureCount.X = Math.Floor(GLSize.X / 64.0#)
            TextureCount.Y = Math.Ceiling(GLSize.Y / 64.0#)
        Else
            TextureCount.X = 0
            TextureCount.Y = 0
        End If

        ScrollUpdate()
    End Sub

    Sub ScrollUpdate()
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

    Sub OpenGL_Resize(ByVal sender As Object, ByVal e As System.EventArgs)

        GLSize.X = OpenGL.Width
        GLSize.Y = OpenGL.Height
        ScreenSize_X_Per_Y = GLSize.X / GLSize.Y
        Viewport_Resize()
    End Sub

    Private Sub TextureScroll_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextureScroll.ValueChanged

        TextureYOffset = TextureScroll.Value

        DrawViewLater()
    End Sub

    Public Function CreateGLFont(ByVal BaseFont As Font) As GLFont

        If GL_Current <> GL_Num Then
            OpenGL.MakeCurrent()
            GL_Current = GL_Num
        End If

        Return New GLFont(New Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel))
    End Function

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

    Private Sub OpenGL_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)

        DrawViewLater()
    End Sub

#If OS <> "Windows" Then
    Private Sub InitializeComponent()
        Me.tmrDraw = New System.Windows.Forms.Timer()
        Me.tmrDraw_Delay = New System.Windows.Forms.Timer()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TextureScroll = New System.Windows.Forms.VScrollBar()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel1.SuspendLayout()
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
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(284, 388)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'TextureScroll
        '
        Me.TextureScroll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextureScroll.Location = New System.Drawing.Point(263, 0)
        Me.TextureScroll.Name = "TextureScroll"
        Me.TextureScroll.Size = New System.Drawing.Size(21, 388)
        Me.TextureScroll.TabIndex = 1
        '
        'pnlDraw
        '
        Me.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDraw.Location = New System.Drawing.Point(0, 0)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(263, 388)
        Me.pnlDraw.TabIndex = 2
        '
        'ctrlTextureView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlTextureView"
        Me.Size = New System.Drawing.Size(284, 388)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tmrDraw As System.Windows.Forms.Timer
    Friend WithEvents tmrDraw_Delay As System.Windows.Forms.Timer
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TextureScroll As System.Windows.Forms.VScrollBar
    Friend WithEvents pnlDraw As System.Windows.Forms.Panel
#End If
End Class