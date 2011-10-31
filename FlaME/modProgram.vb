Imports ICSharpCode.SharpZipLib
Imports OpenTK.Graphics.OpenGL
Imports System.Globalization

Public Module modProgram

    Public Const ProgramName As String = "FlaME"

    Public Const ProgramVersionNumber As String = "1.24"

#If MonoDevelop = 0.0# Then
    Public Const ProgramPlatform As String = "Windows"
#Else
    #If Mono <> 0.0# Then
        #If Mono267 <> 0.0# Then
            Public Const ProgramPlatform As String = "Mono 2.6.7"
        #Else
            Public Const ProgramPlatform As String = "Mono 2.10"
        #End If 
    #Else
        Public Const ProgramPlatform As String = "MonoDevelop Microsoft .NET"
    #End If
#End If

    Public Const PlayerCountMax As Integer = 10

    Public Const GameTypeCount As Integer = 3

    Public Const DefaultHeightMultiplier As Integer = 2

#If Mono = 0.0# Then
    Public Const MinimapDelay As Integer = 100
#Else
    Public Const MinimapDelay As Integer = 4000
#End If

    Public Const SectorTileSize As Integer = 8

    Public Const MaxDroidWeapons As Integer = 3

    Public Const WZMapMaxSize As Integer = 250
    Public Const MapMaxSize As Integer = 512

    Public Const MinimapMaxSize As Integer = 512
    Public MinimapFeatureColour As sRGB_sng

    Public PlatformPathSeparator As Char

    Public MyDocumentsProgramPath As String

    Public SettingsPath As String
    Public AutoSavePath As String
    Public InterfaceImagesPath As String

    Public Sub SetProgramSubDirs()

        MyDocumentsProgramPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments & PlatformPathSeparator & ".flaME"
#If Portable = 0.0# Then
        SettingsPath = MyDocumentsProgramPath & PlatformPathSeparator & "settings.ini"
        AutoSavePath = MyDocumentsProgramPath & PlatformPathSeparator & "autosave" & PlatformPathSeparator
#Else
        SettingsPath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "settings.ini"
        AutoSavePath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "autosave" & PlatformPathSeparator
#End If
        InterfaceImagesPath = My.Application.Info.DirectoryPath & PlatformPathSeparator & "interface" & PlatformPathSeparator
    End Sub

    Public ProgramInitialized As Boolean = False

    Public ProgramIcon As Icon

    Public CommandLinePaths(-1) As String

    Public GLTexture_NoTile As Integer
    Public GLTexture_OverflowTile As Integer

    Public frmMainInstance As New frmMain
#If MonoDevelop = 0.0# Then
    Public frmSplashInstance As New frmSplash
#End If
    Public frmGeneratorInstance As New frmGenerator
    Public frmDataInstance As New frmData
    Public frmOptionsInstance As frmOptions

    Public IsViewKeyDown As New clsKeysActive

    'interface controls
    Public Control_Deselect As clsInputControl
    'selected unit controls
    Public Control_Unit_Move As clsInputControl
    Public Control_Unit_Delete As clsInputControl
    Public Control_Unit_Multiselect As clsInputControl
    'generalised controls
    Public Control_Slow As clsInputControl
    Public Control_Fast As clsInputControl
    'picker controls
    Public Control_Picker As clsInputControl
    'view controls
    Public Control_View_Textures As clsInputControl
    Public Control_View_Lighting As clsInputControl
    Public Control_View_Wireframe As clsInputControl
    Public Control_View_Units As clsInputControl
    Public Control_View_ScriptMarkers As clsInputControl
    Public Control_View_Move_Type As clsInputControl
    Public Control_View_Rotate_Type As clsInputControl
    Public Control_View_Move_Left As clsInputControl
    Public Control_View_Move_Right As clsInputControl
    Public Control_View_Move_Forward As clsInputControl
    Public Control_View_Move_Backward As clsInputControl
    Public Control_View_Move_Up As clsInputControl
    Public Control_View_Move_Down As clsInputControl
    Public Control_View_Zoom_In As clsInputControl
    Public Control_View_Zoom_Out As clsInputControl
    Public Control_View_Left As clsInputControl
    Public Control_View_Right As clsInputControl
    Public Control_View_Forward As clsInputControl
    Public Control_View_Backward As clsInputControl
    Public Control_View_Up As clsInputControl
    Public Control_View_Down As clsInputControl
    Public Control_View_Reset As clsInputControl
    Public Control_View_Roll_Left As clsInputControl
    Public Control_View_Roll_Right As clsInputControl
    'texture controls
    Public Control_Clockwise As clsInputControl
    Public Control_CounterClockwise As clsInputControl
    Public Control_Texture_Flip As clsInputControl
    Public Control_Tri_Flip As clsInputControl
    'gateway controls
    Public Control_Gateway_Delete As clsInputControl
    'undo controls
    Public Control_Undo As clsInputControl
    Public Control_Redo As clsInputControl
    'script marker controls
    Public Control_ScriptPosition As clsInputControl

    Public TextureBrush As New clsBrush(0.125#, clsBrush.enumShape.Circle)
    Public TerrainBrush As New clsBrush(1.875#, clsBrush.enumShape.Circle)
    Public HeightBrush As New clsBrush(1.875#, clsBrush.enumShape.Circle)
    Public CliffBrush As New clsBrush(1.875#, clsBrush.enumShape.Circle)

    Public SmoothRadius As New clsBrush(1.0#, clsBrush.enumShape.Square)

    Public InputControls() As clsInputControl
    Public InputControlCount As Integer

    Public DisplayTileOrientation As Boolean

    Public Settings As clsSettings

    Public Enum enumTool As Byte
        None
        Texture_Brush
        AutoTexture_Place
        AutoTexture_Fill
        AutoRoad_Place
        AutoRoad_Line
        AutoRoad_Remove
        CliffTriangle
        AutoCliff
        AutoCliffRemove
        Height_Set_Brush
        Height_Change_Brush
        Height_Smooth_Brush
        ObjectPlace
        Terrain_Select
        Gateways
    End Enum
    Public Tool As enumTool = enumTool.Texture_Brush

    Public SelectedTextureNum As Integer = -1
    Public TextureOrientation As New sTileOrientation(False, False, False)

    Public SelectedTerrain As clsPainter.clsTerrain
    Public SelectedRoad As clsPainter.clsRoad

    Public Structure sTileType
        Public Name As String
        Public DisplayColour As sRGB_sng
    End Structure
    Public TileTypes(-1) As sTileType
    Public TileTypeCount As Integer

    Public Const TileTypeNum_Water As Integer = 7
    Public Const TileTypeNum_Cliff As Integer = 8

    Public TemplateDroidTypes(-1) As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidTypeCount As Integer

    Public Const INIRotationMax As Integer = 65536

    Public Enum enumObjectRotateMode As Byte
        None
        Walls
        All
    End Enum

    Public Enum enumTextureTerrainAction As Byte
        Ignore
        Reinterpret
        Remove
    End Enum

    Public Enum enumFillCliffAction As Byte
        Ignore
        StopBefore
        StopAfter
    End Enum

    Public Structure sResult
        Public Success As Boolean
        Public Problem As String
    End Structure

    Public Class clsTextLabels
        Public Items() As clsTextLabel
        Public ItemCount As Integer = 0
        Public MaxCount As Integer

        Public Sub New(ByVal MaxItemCount As Integer)

            MaxCount = MaxItemCount
            ReDim Items(MaxCount - 1)
        End Sub

        Public Function AtMaxCount() As Boolean

            Return (ItemCount >= MaxCount)
        End Function

        Public Sub Add(ByVal NewItem As clsTextLabel)

            If ItemCount = MaxCount Then
                Stop
                Exit Sub
            End If

            Items(ItemCount) = NewItem
            ItemCount += 1
        End Sub

        Public Sub Draw()
            Dim A As Integer

            For A = 0 To ItemCount - 1
                Items(A).Draw()
            Next
        End Sub
    End Class

    Public Class clsTextLabel
        Public Text As String
        Public TextFont As GLFont
        Public SizeY As Single
        Public Colour As sRGBA_sng
        Public Pos As sXY_int

        Public Function GetSizeX() As Single
            Dim SizeX As Single
            Dim CharWidth As Single
            Dim CharSpacing As Single = SizeY / 10.0F
            Dim CharSize As Single = SizeY / TextFont.Height
            Dim A As Integer

            For A = 0 To Text.Length - 1
                CharWidth = TextFont.Character(Asc(Text.Chars(A))).Width * CharSize
                SizeX += CharWidth
            Next
            SizeX += CharSpacing * (Text.Length - 1)

            Return SizeX
        End Function

        Public Sub Draw()

            If Text Is Nothing Then
                Exit Sub
            End If
            If Text.Length = 0 Then
                Exit Sub
            End If
            If TextFont Is Nothing Then
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

            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            PosY1 = Pos.Y
            PosY2 = Pos.Y + SizeY
            CharSpacing = SizeY / 10.0F
            LetterPosA = Pos.X
            For A = 0 To Text.Length - 1
                CharCode = Asc(Text(A))
                If CharCode >= 0 And CharCode <= 255 Then
                    CharWidth = SizeY * TextFont.Character(CharCode).Width / TextFont.Height
                    TexRatio.X = CSng(TextFont.Character(CharCode).Width / TextFont.Character(CharCode).TexSize)
                    TexRatio.Y = CSng(TextFont.Height / TextFont.Character(CharCode).TexSize)
                    LetterPosB = LetterPosA + CharWidth
                    GL.BindTexture(TextureTarget.Texture2D, TextFont.Character(CharCode).GLTexture)
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
                    GL.Begin(BeginMode.Quads)
                    GL.TexCoord2(0.0F, 0.0F)
                    GL.Vertex2(LetterPosA, PosY1)
                    GL.TexCoord2(0.0F, TexRatio.Y)
                    GL.Vertex2(LetterPosA, PosY2)
                    GL.TexCoord2(TexRatio.X, TexRatio.Y)
                    GL.Vertex2(LetterPosB, PosY2)
                    GL.TexCoord2(TexRatio.X, 0.0F)
                    GL.Vertex2(LetterPosB, PosY1)
                    GL.End()
                    LetterPosA = LetterPosB + CharSpacing
                End If
            Next
        End Sub
    End Class

    Public Structure sWZAngle
        Public Direction As UShort
        Public Pitch As UShort
        Public Roll As UShort
    End Structure

    Public Structure sRGB_sng
        Public Red As Single
        Public Green As Single
        Public Blue As Single

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single)

            Me.Red = Red
            Me.Green = Green
            Me.Blue = Blue
        End Sub
    End Structure

    Public Structure sRGBA_sng
        Public Red As Single
        Public Green As Single
        Public Blue As Single
        Public Alpha As Single

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single, ByVal Alpha As Single)

            Me.Red = Red
            Me.Green = Green
            Me.Blue = Blue
            Me.Alpha = Alpha
        End Sub
    End Structure

    Public Class clsRGB_sng
        Public Red As Single
        Public Green As Single
        Public Blue As Single
        Public Enum enumType As Byte
            RGB
            RGBA
        End Enum
        Protected Type As enumType
        Public ReadOnly Property ColourType As enumType
            Get
                Return Type
            End Get
        End Property

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single)

            Me.Red = Red
            Me.Green = Green
            Me.Blue = Blue
            Type = enumType.RGB
        End Sub

        Public Overridable Function GetINIOutput() As String

            Return InvariantToString_sng(Red) & ", " & InvariantToString_sng(Green) & ", " & InvariantToString_sng(Blue)
        End Function

        Public Overridable Function ReadINIText(ByVal SplitText As clsSplitCommaText) As Boolean

            If SplitText.PartCount < 3 Then
                Return False
            End If

            Dim Colour As sRGB_sng

            Try
                InvariantParse_sng(SplitText.Parts(0), Colour.Red)
                InvariantParse_sng(SplitText.Parts(1), Colour.Green)
                InvariantParse_sng(SplitText.Parts(2), Colour.Blue)
            Catch ex As Exception
                Return False
            End Try

            Red = Colour.Red
            Green = Colour.Green
            Blue = Colour.Blue

            Return True
        End Function
    End Class

    Public Class clsRGBA_sng
        Inherits clsRGB_sng

        Public Alpha As Single

        Public Sub New(ByVal Red As Single, ByVal Green As Single, ByVal Blue As Single, ByVal Alpha As Single)
            MyBase.New(Red, Green, Blue)

            Me.Alpha = Alpha
            Type = enumType.RGBA
        End Sub

        Public Sub New(ByVal CopyItem As clsRGBA_sng)
            MyBase.New(CopyItem.Red, CopyItem.Green, CopyItem.Blue)

            Alpha = CopyItem.Alpha
            Type = enumType.RGBA
        End Sub

        Public Overrides Function GetINIOutput() As String

            Return MyBase.GetINIOutput() & ", " & InvariantToString_sng(Alpha)
        End Function

        Public Overrides Function ReadINIText(ByVal SplitText As clsSplitCommaText) As Boolean

            If Not MyBase.ReadINIText(SplitText) Then
                Return False
            End If

            If SplitText.PartCount < 4 Then
                Return False
            End If

            Try
                Alpha = CSng(SplitText.Parts(3))
            Catch ex As Exception
                Alpha = 1.0F
                Return False
            End Try

            Return True
        End Function
    End Class

    Public Class clsBrush

        Public Structure sPosNum
            Public Normal As sXY_int
            Public Alignment As sXY_int
        End Structure

        Private _Radius As Double
        Public Enum enumShape As Byte
            Circle
            Square
        End Enum
        Private _Shape As enumShape = enumShape.Circle

        Public Tiles As sBrushTiles

        Public ReadOnly Property Alignment As Boolean
            Get
                Return _Alignment
            End Get
        End Property
        Private _Alignment As Boolean

        Public Property Radius As Double
            Get
                Return _Radius
            End Get
            Set(ByVal value As Double)
                If _Radius = value Then
                    Exit Property
                End If
                _Radius = value
                CreateTiles()
            End Set
        End Property

        Public Property Shape As enumShape
            Get
                Return _Shape
            End Get
            Set(ByVal value As enumShape)
                If _Shape = value Then
                    Exit Property
                End If
                _Shape = value
                CreateTiles()
            End Set
        End Property

        Private Sub CreateTiles()
            Dim AlignmentOffset As Double = _Radius - Int(_Radius)

            _Alignment = (AlignmentOffset >= 0.25# And AlignmentOffset < 0.75#)
            Select Case _Shape
                Case enumShape.Circle
                    Tiles.CreateCircle(_Radius, 1.0#, _Alignment)
                Case enumShape.Square
                    Tiles.CreateSquare(_Radius, 1.0#, _Alignment)
            End Select
        End Sub

        Public Sub New(ByVal InitialRadius As Double, ByVal InitialShape As enumShape)

            _Radius = InitialRadius
            _Shape = InitialShape
            CreateTiles()
        End Sub

        Public Sub PerformActionMapTiles(ByVal Tool As clsMap.clsAction, ByVal Centre As sPosNum)

            PerformAction(Tool, Centre, New sXY_int(Tool.Map.Terrain.TileSize.X - 1, Tool.Map.Terrain.TileSize.Y - 1))
        End Sub

        Public Sub PerformActionMapVertices(ByVal Tool As clsMap.clsAction, ByVal Centre As sPosNum)

            PerformAction(Tool, Centre, Tool.Map.Terrain.TileSize)
        End Sub

        Public Sub PerformActionMapSectors(ByVal Tool As clsMap.clsAction, ByVal Centre As sPosNum)

            PerformAction(Tool, Centre, New sXY_int(Tool.Map.SectorCount.X - 1, Tool.Map.SectorCount.Y - 1))
        End Sub

        Public Function GetPosNum(ByVal PosNum As sPosNum) As sXY_int

            If _Alignment Then
                Return PosNum.Alignment
            Else
                Return PosNum.Normal
            End If
        End Function

        Private Sub PerformAction(ByVal Action As clsMap.clsAction, ByVal PosNum As sPosNum, ByVal LastValidNum As sXY_int)
            Dim XNum As Integer
            Dim X As Integer
            Dim Y As Integer
            Dim Centre As sXY_int

            If Action.Map Is Nothing Then
                Stop
                Exit Sub
            End If

            Centre = GetPosNum(PosNum)

            Action.Effect = 1.0#
            For Y = Clamp_int(Tiles.YMin + Centre.Y, 0, LastValidNum.Y) - Centre.Y To Clamp_int(Tiles.YMax + Centre.Y, 0, LastValidNum.Y) - Centre.Y
                Action.PosNum.Y = Centre.Y + Y
                XNum = Y - Tiles.YMin
                For X = Clamp_int(Tiles.XMin(XNum) + Centre.X, 0, LastValidNum.X) - Centre.X To Clamp_int(Tiles.XMax(XNum) + Centre.X, 0, LastValidNum.X) - Centre.X
                    Action.PosNum.X = Centre.X + X
                    If Action.UseEffect Then
                        If Tiles.ResultRadius > 0.0# Then
                            Select Case _Shape
                                Case clsBrush.enumShape.Circle
                                    If _Alignment Then
                                        Action.Effect = 1.0# - GetDist_XY_dbl(New sXY_dbl(Centre.X - 0.5#, Centre.Y - 0.5#), New sXY_dbl(Action.PosNum)) / (Tiles.ResultRadius + 0.5#)
                                    Else
                                        Action.Effect = 1.0# - GetDist_XY_int(Centre, Action.PosNum) / (Tiles.ResultRadius + 0.5#)
                                    End If
                                Case clsBrush.enumShape.Square
                                    If _Alignment Then
                                        Action.Effect = 1.0# - Math.Max(Math.Abs(Action.PosNum.X - (Centre.X - 0.5#)), Math.Abs(Action.PosNum.Y - (Centre.Y - 0.5#))) / (Tiles.ResultRadius + 0.5#)
                                    Else
                                        Action.Effect = 1.0# - Math.Max(Math.Abs(Action.PosNum.X - Centre.X), Math.Abs(Action.PosNum.Y - Centre.Y)) / (Tiles.ResultRadius + 0.5#)
                                    End If
                            End Select
                        End If
                    End If
                    Action.ActionPerform()
                Next
            Next
        End Sub
    End Class

    Public Structure sBrushTiles
        Public XMin() As Integer
        Public XMax() As Integer
        Public YMin As Integer
        Public YMax As Integer
        Public ResultRadius As Double

        Public Sub CreateCircle(ByVal Radius As Double, ByVal TileSize As Double, ByVal Alignment As Boolean)
            Dim X As Integer
            Dim Y As Integer
            Dim dblX As Double
            Dim dblY As Double
            Dim RadiusB As Double
            Dim RadiusC As Double
            Dim A As Integer
            Dim B As Integer

            RadiusB = Radius / TileSize + 0.5#
            If Alignment Then
                Y = CInt(Math.Round(RadiusB))
                YMin = -Y
                YMax = Y - 1
                B = YMax - YMin
                ReDim XMin(B)
                ReDim XMax(B)
                RadiusC = RadiusB * RadiusB
                For Y = YMin To YMax
                    dblY = Y + 0.5#
                    dblX = Math.Sqrt(RadiusC - dblY * dblY) + 0.5#
                    A = Y - YMin
                    X = CInt(Int(dblX))
                    XMin(A) = -X
                    XMax(A) = X - 1
                Next
            Else
                Y = CInt(Int(RadiusB))
                YMin = -Y
                YMax = Y
                B = YMax - YMin
                ReDim XMin(B)
                ReDim XMax(B)
                RadiusC = RadiusB * RadiusB
                For Y = YMin To YMax
                    dblX = Math.Sqrt(RadiusC - Y * Y)
                    A = Y - YMin
                    X = CInt(Int(dblX))
                    XMin(A) = -X
                    XMax(A) = X
                Next
            End If

            ResultRadius = B / 2.0#
        End Sub

        Public Sub CreateSquare(ByVal Radius As Double, ByVal TileSize As Double, ByVal Alignment As Boolean)
            Dim Y As Integer
            Dim A As Integer
            Dim B As Integer
            Dim RadiusB As Double

            RadiusB = Radius / TileSize + 0.5#
            If Alignment Then
                RadiusB += 0.5#
                A = CInt(Int(RadiusB))
                YMin = -A
                YMax = A - 1
            Else
                A = CInt(Int(RadiusB))
                YMin = -A
                YMax = A
            End If
            B = YMax - YMin
            ReDim XMin(B)
            ReDim XMax(B)
            For Y = 0 To B
                XMin(Y) = YMin
                XMax(Y) = YMax
            Next

            ResultRadius = B / 2.0#
        End Sub
    End Structure

    Public Const TerrainGridSpacing As Integer = 128

    Public VisionRadius_2E As Integer
    Public VisionRadius As Double

    Public Copied_Map As clsMap

    Public Tilesets() As clsTileset
    Public TilesetCount As Integer

    Public Tileset_Arizona As clsTileset
    Public Tileset_Urban As clsTileset
    Public Tileset_Rockies As clsTileset

    Public Painter_Arizona As clsPainter
    Public Painter_Urban As clsPainter
    Public Painter_Rockies As clsPainter

    Public UnitTypes(-1) As clsUnitType
    Public UnitTypeCount As Integer

    Public Structure sTexturePage
        Public FileTitle As String
        Public GLTexture_Num As Integer
    End Structure
    Public TexturePages() As sTexturePage
    Public TexturePageCount As Integer

    Public UnitLabelFont As GLFont
    'Public TextureViewFont As GLFont

    Public Class clsPlayer
        Public Colour As sRGB_sng
        Public MinimapColour As sRGB_sng

        Public Sub CalcMinimapColour()

            MinimapColour.Red = Math.Min(Colour.Red * 0.6666667F + 0.333333343F, 1.0F)
            MinimapColour.Green = Math.Min(Colour.Green * 0.6666667F + 0.333333343F, 1.0F)
            MinimapColour.Blue = Math.Min(Colour.Blue * 0.6666667F + 0.333333343F, 1.0F)
        End Sub
    End Class
    Public PlayerColour(15) As clsPlayer

    Public Structure sSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Public Sub New(ByVal Path As String)
            Dim A As Integer

            Parts = Path.Split(PlatformPathSeparator)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & PlatformPathSeparator
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Structure sZipSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Public Sub New(ByVal Path As String)
            Dim tmpPath As String = Path.ToLower.Replace("\"c, "/"c)
            Dim A As Integer

            Parts = tmpPath.Split("/"c)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & "/"
            Next
            FileTitle = Parts(A)
            A = InStrRev(FileTitle, ".")
            If A > 0 Then
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A)
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1)
            Else
                FileExtension = ""
                FileTitleWithoutExtension = FileTitle
            End If
        End Sub
    End Structure

    Public Sub VisionRadius_2E_Changed()

        VisionRadius = 256.0# * 2.0# ^ (VisionRadius_2E / 2.0#)
        If frmMainInstance.MapView IsNot Nothing Then
            View_Radius_Set(VisionRadius)
            frmMainInstance.View_DrawViewLater()
        End If
    End Sub

    Public Function Get_TexturePage_GLTexture(ByVal FileTitle As String) As Integer
        Dim LCaseTitle As String = FileTitle.ToLower
        Dim A As Integer

        For A = 0 To TexturePageCount - 1
            If TexturePages(A).FileTitle.ToLower = LCaseTitle Then
                Return TexturePages(A).GLTexture_Num
            End If
        Next
        Return 0
    End Function

    Public Sub XY_Reorder(ByVal XY_A_Input As sXY_int, ByVal XY_B_Input As sXY_int, ByRef XY_Low_Output As sXY_int, ByRef XY_High_Output As sXY_int)

        If XY_A_Input.X <= XY_B_Input.X Then
            XY_Low_Output.X = XY_A_Input.X
            XY_High_Output.X = XY_B_Input.X
        Else
            XY_Low_Output.X = XY_B_Input.X
            XY_High_Output.X = XY_A_Input.X
        End If
        If XY_A_Input.Y <= XY_B_Input.Y Then
            XY_Low_Output.Y = XY_A_Input.Y
            XY_High_Output.Y = XY_B_Input.Y
        Else
            XY_Low_Output.Y = XY_B_Input.Y
            XY_High_Output.Y = XY_A_Input.Y
        End If
    End Sub

    Public Function EndWithPathSeperator(ByVal Text As String) As String

        If Strings.Right(Text, 1) = PlatformPathSeparator Then
            Return Text
        Else
            Return Text & PlatformPathSeparator
        End If
    End Function

    Public Function TileType_Add(ByVal NewTileType As sTileType) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve TileTypes(TileTypeCount)
        TileTypes(TileTypeCount) = NewTileType
        ReturnResult = TileTypeCount
        TileTypeCount += 1

        Return ReturnResult
    End Function

    Public Function MinDigits(ByVal Number As Integer, ByVal Digits As Integer) As String
        Dim ReturnResult As String
        Dim A As Integer

        ReturnResult = InvariantToString_int(Number)
        A = Digits - ReturnResult.Length
        If A > 0 Then
            ReturnResult = Strings.StrDup(A, "0"c) & ReturnResult
        End If
        Return ReturnResult
    End Function

    Public Sub ViewKeyDown_Clear()
        Dim A As Integer

        IsViewKeyDown.Deactivate()

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Public Function OSRGB(ByVal Red As Integer, ByVal Green As Integer, ByVal Blue As Integer) As Integer

#If Mono = 0.0# And Mono267 = 0.0# Then
        Return RGB(Red, Green, Blue)
#Else
        Return RGB(Blue, Green, Red)
#End If
    End Function

    Public Class clsZipStreamEntry
        Public Stream As Zip.ZipInputStream
        Public Entry As Zip.ZipEntry
    End Class

    Public Function FindZipEntryFromPath(ByVal Path As String, ByVal ZipPathToFind As String) As clsZipStreamEntry
        Dim ZipStream As Zip.ZipInputStream
        Dim ZipEntry As Zip.ZipEntry
        Dim FindPath As String = ZipPathToFind.ToLower.Replace("\"c, "/"c)
        Dim tmpPath As String

        ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
        Do
            Try
                ZipEntry = ZipStream.GetNextEntry
            Catch ex As Exception
                Exit Do
            End Try
            If ZipEntry Is Nothing Then
                Exit Do
            End If

            tmpPath = ZipEntry.Name.ToLower.Replace("\"c, "/"c)
            If tmpPath = FindPath Then
                Dim Result As New clsZipStreamEntry
                Result.Stream = ZipStream
                Result.Entry = ZipEntry
                Return Result
            End If
        Loop
        ZipStream.Close()

        Return Nothing
    End Function

    Public TemplateDroidType_Droid As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Cyborg As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgConstruct As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgRepair As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_CyborgSuper As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Transporter As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Person As clsDroidDesign.clsTemplateDroidType
    Public TemplateDroidType_Null As clsDroidDesign.clsTemplateDroidType

    Public Sub CreateTemplateDroidTypes()

        TemplateDroidType_Droid = New clsDroidDesign.clsTemplateDroidType("Droid", "DROID")
        TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid)

        TemplateDroidType_Cyborg = New clsDroidDesign.clsTemplateDroidType("Cyborg", "CYBORG")
        TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg)

        TemplateDroidType_CyborgConstruct = New clsDroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT")
        TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct)

        TemplateDroidType_CyborgRepair = New clsDroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR")
        TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair)

        TemplateDroidType_CyborgSuper = New clsDroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER")
        TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper)

        TemplateDroidType_Transporter = New clsDroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER")
        TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter)

        TemplateDroidType_Person = New clsDroidDesign.clsTemplateDroidType("Person", "PERSON")
        TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person)

        TemplateDroidType_Null = New clsDroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID")
        TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null)
    End Sub

    Public Function GetTemplateDroidTypeFromTemplateCode(ByVal Code As String) As clsDroidDesign.clsTemplateDroidType
        Dim LCaseCode As String = Code.ToLower
        Dim A As Integer

        For A = 0 To TemplateDroidTypeCount - 1
            If TemplateDroidTypes(A).TemplateCode.ToLower = LCaseCode Then
                Return TemplateDroidTypes(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function TemplateDroidType_Add(ByVal NewDroidType As clsDroidDesign.clsTemplateDroidType) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve TemplateDroidTypes(TemplateDroidTypeCount)
        TemplateDroidTypes(TemplateDroidTypeCount) = NewDroidType
        ReturnResult = TemplateDroidTypeCount
        TemplateDroidTypeCount += 1

        Return ReturnResult
    End Function

    Public Enum enumDroidType As Byte
        Weapon = 0
        Sensor = 1
        ECM = 2
        Construct = 3
        Person = 4
        Cyborg = 5
        Transporter = 6
        Command = 7
        Repair = 8
        Default_ = 9
        Cyborg_Construct = 10
        Cyborg_Repair = 11
        Cyborg_Super = 12
    End Enum

    Public Sub ShowWarnings(ByVal Result As clsResult, ByVal Title As String)

        If Not Result.HasWarnings Then
            Exit Sub
        End If

        Dim WarningsForm As New frmWarnings(Result, Title)
        WarningsForm.Show()
        WarningsForm.Activate()
    End Sub

    Public Function GetTurretFromTypeAndCode(ByVal TurretTypeName As String, ByVal TurretCode As String) As clsTurret
        Dim LCaseTypeName As String = TurretTypeName.ToLower

        Select Case LCaseTypeName
            Case "weapon"
                Return FindWeaponCode(TurretCode)
            Case "construct"
                Return FindConstructCode(TurretCode)
            Case "repair"
                Return FindRepairCode(TurretCode)
            Case "sensor"
                Return FindSensorCode(TurretCode)
            Case "brain"
                Return FindBrainCode(TurretCode)
            Case "ecm"
                Return FindECMCode(TurretCode)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function GetTurretTypeFromName(ByVal TurretTypeName As String) As clsTurret.enumTurretType

        Select Case TurretTypeName.ToLower
            Case "weapon"
                Return clsTurret.enumTurretType.Weapon
            Case "construct"
                Return clsTurret.enumTurretType.Construct
            Case "repair"
                Return clsTurret.enumTurretType.Repair
            Case "sensor"
                Return clsTurret.enumTurretType.Sensor
            Case "brain"
                Return clsTurret.enumTurretType.Brain
            Case "ecm"
                Return clsTurret.enumTurretType.ECM
            Case Else
                Return clsTurret.enumTurretType.Unknown
        End Select
    End Function

    Public Function FindOrCreateWeapon(ByVal Code As String) As clsWeapon
        Dim Result As clsWeapon

        Result = FindWeaponCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsWeapon
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateConstruct(ByVal Code As String) As clsConstruct
        Dim Result As clsConstruct

        Result = FindConstructCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsConstruct
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateRepair(ByVal Code As String) As clsRepair
        Dim Result As clsRepair

        Result = FindRepairCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsRepair
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateSensor(ByVal Code As String) As clsSensor
        Dim Result As clsSensor

        Result = FindSensorCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsSensor
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateBrain(ByVal Code As String) As clsBrain
        Dim Result As clsBrain

        Result = FindBrainCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsBrain
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateECM(ByVal Code As String) As clsECM
        Dim Result As clsECM

        Result = FindECMCode(Code)
        If Result IsNot Nothing Then
            Return Result
        End If
        Result = New clsECM
        Result.IsUnknown = True
        Result.Code = Code
        Return Result
    End Function

    Public Function FindOrCreateTurret(ByVal TurretType As clsTurret.enumTurretType, ByVal TurretCode As String) As clsTurret

        Select Case TurretType
            Case clsTurret.enumTurretType.Weapon
                Return FindOrCreateWeapon(TurretCode)
            Case clsTurret.enumTurretType.Construct
                Return FindOrCreateConstruct(TurretCode)
            Case clsTurret.enumTurretType.Repair
                Return FindOrCreateRepair(TurretCode)
            Case clsTurret.enumTurretType.Sensor
                Return FindOrCreateSensor(TurretCode)
            Case clsTurret.enumTurretType.Brain
                Return FindOrCreateBrain(TurretCode)
            Case clsTurret.enumTurretType.ECM
                Return FindOrCreateECM(TurretCode)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function FindOrCreateBody(ByVal Code As String) As clsBody
        Dim tmpBody As clsBody

        tmpBody = FindBodyCode(Code)
        If tmpBody IsNot Nothing Then
            Return tmpBody
        End If
        tmpBody = New clsBody
        tmpBody.IsUnknown = True
        tmpBody.Code = Code
        Return tmpBody
    End Function

    Public Function FindOrCreatePropulsion(ByVal Code As String) As clsPropulsion
        Dim tmpPropulsion As clsPropulsion

        tmpPropulsion = FindPropulsionCode(Code)
        If tmpPropulsion IsNot Nothing Then
            Return tmpPropulsion
        End If
        tmpPropulsion = New clsPropulsion(BodyCount)
        tmpPropulsion.IsUnknown = True
        tmpPropulsion.Code = Code
        Return tmpPropulsion
    End Function

    Public Function FindOrCreateUnitType(ByVal Code As String, ByVal Type As clsUnitType.enumType) As clsUnitType
        Dim A As Integer

        Select Case Type
            Case clsUnitType.enumType.Feature
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.Feature Then
                        If CType(UnitTypes(A), clsFeatureType).Code = Code Then
                            Return UnitTypes(A)
                        End If
                    End If
                Next
                Dim tmpFeatureType As New clsFeatureType
                tmpFeatureType.IsUnknown = True
                tmpFeatureType.Code = Code
                tmpFeatureType.Footprint.X = 1
                tmpFeatureType.Footprint.Y = 1
                Return tmpFeatureType
            Case clsUnitType.enumType.PlayerStructure
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.PlayerStructure Then
                        If CType(UnitTypes(A), clsStructureType).Code = Code Then
                            Return UnitTypes(A)
                        End If
                    End If
                Next
                Dim tmpStructureType As New clsStructureType
                tmpStructureType.IsUnknown = True
                tmpStructureType.Code = Code
                tmpStructureType.Footprint.X = 1
                tmpStructureType.Footprint.Y = 1
                Return tmpStructureType
            Case clsUnitType.enumType.PlayerDroid
                Dim tmpDroidType As clsDroidDesign
                For A = 0 To UnitTypeCount - 1
                    If UnitTypes(A).Type = clsUnitType.enumType.PlayerDroid Then
                        tmpDroidType = CType(UnitTypes(A), clsDroidDesign)
                        If tmpDroidType.IsTemplate Then
                            If CType(tmpDroidType, clsDroidTemplate).Code = Code Then
                                Return UnitTypes(A)
                            End If
                        End If
                    End If
                Next
                Dim tmpDroidTemplate As New clsDroidTemplate
                tmpDroidTemplate.IsUnknown = True
                tmpDroidTemplate.Code = Code
                Return tmpDroidTemplate
            Case Else
                Return Nothing
        End Select
    End Function

    Public ShowIDErrorMessage As Boolean = True

    Public Sub ErrorIDChange(ByVal IntendedID As UInteger, ByVal IDUnit As clsMap.clsUnit, ByVal NameOfErrorSource As String)

        If Not ShowIDErrorMessage Then
            Exit Sub
        End If

        If IDUnit.ID = IntendedID Then
            Exit Sub
        End If

        Dim MessageText As String

        MessageText = "An object's ID has been changed unexpectedly. The error was in " & ControlChars.Quote & NameOfErrorSource & ControlChars.Quote & "." & ControlChars.CrLf & ControlChars.CrLf & "The object is of type " & IDUnit.Type.GetDisplayText & " and is at map position " & IDUnit.GetPosText & ". It's ID was " & InvariantToString_uint(IntendedID) & ", but is now " & InvariantToString_uint(IDUnit.ID) & "." & ControlChars.CrLf & ControlChars.CrLf & "Click Cancel to stop seeing this message. Otherwise, click OK."

        If MsgBox(MessageText, MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then
            ShowIDErrorMessage = False
        End If
    End Sub

    Public Sub ZeroIDWarning(ByVal IDUnit As clsMap.clsUnit, ByVal NewID As UInteger)
        Dim MessageText As String

        MessageText = "An object's ID has been changed from 0 to " & InvariantToString_uint(NewID) & ". Zero is not a valid ID. The object is of type " & IDUnit.Type.GetDisplayText & " and is at map position " & IDUnit.GetPosText & "."

        MsgBox(MessageText, MsgBoxStyle.OkOnly)
    End Sub

    Public Structure sWorldPos
        Public Horizontal As sXY_int
        Public Altitude As Integer

        Public Sub New(ByVal NewHorizontal As sXY_int, ByVal NewAltitude As Integer)

            Horizontal = NewHorizontal
            Altitude = NewAltitude
        End Sub
    End Structure

    Public Class clsWorldPos
        Public WorldPos As sWorldPos

        Public Sub New(ByVal NewWorldPos As sWorldPos)

            WorldPos = NewWorldPos
        End Sub
    End Class

    Public Function PosIsWithinTileArea(ByVal WorldHorizontal As sXY_int, ByVal StartTile As sXY_int, ByVal FinishTile As sXY_int) As Boolean

        Return (WorldHorizontal.X >= StartTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y >= StartTile.Y * TerrainGridSpacing And _
            WorldHorizontal.X < FinishTile.X * TerrainGridSpacing And _
            WorldHorizontal.Y < FinishTile.Y * TerrainGridSpacing)
    End Function

    Public Function SizeIsPowerOf2(ByVal Size As Integer) As Boolean

        Dim Power As Double = Math.Log(Size) / Math.Log(2.0#)
        Return (Power = CInt(Power))
    End Function

    Public Function FindFirstStructureType(ByVal FindType As clsStructureType.enumStructureType) As clsStructureType
        Dim A As Integer
        Dim tmpObjectType As clsUnitType
        Dim tmpStructureType As clsStructureType

        For A = 0 To UnitTypeCount - 1
            tmpObjectType = UnitTypes(A)
            If tmpObjectType.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructureType = CType(tmpObjectType, clsStructureType)
                If tmpStructureType.StructureType = FindType Then
                    Return tmpStructureType
                End If
            End If
        Next

        Return Nothing
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
        If Dialog.ShowDialog(frmMainInstance) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim Result As clsResult = LoadMap(Dialog.FileName)
        ShowWarnings(Result, "Load Map")
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
            ResultMap.InitializeForUserInput()
            NewMainMap(ResultMap)
            UpdateMapTabs()
        End If

        Return ReturnResult
    End Function

    Public Class clsKeysActive
        Public Keys(255) As Boolean

        Public Sub Deactivate()
            Dim A As Integer

            For A = 0 To 255
                Keys(A) = False
            Next
        End Sub
    End Class

    Public Class clsSplitCommaText

        Public Parts() As String
        Public PartCount As Integer

        Public Sub New(ByVal Text As String)
            Dim A As Integer

            Parts = Text.Split(","c)
            PartCount = Parts.GetUpperBound(0) + 1
            For A = 0 To PartCount - 1
                Parts(A) = Parts(A).Trim()
            Next
        End Sub
    End Class

    Public Function LoadTilesets(ByVal TilesetsPath As String) As clsResult
        Dim ReturnResult As New clsResult

        Dim TilesetDirs() As String
        Try
            TilesetDirs = IO.Directory.GetDirectories(TilesetsPath)
        Catch ex As Exception
            ReturnResult.Problem_Add("Error reading tilesets directory: " & ex.Message)
            Return ReturnResult
        End Try
        Dim A As Integer

        If TilesetDirs Is Nothing Then
            TilesetCount = 0
            ReDim Tilesets(-1)
            Return ReturnResult
        End If

        TilesetCount = 0
        ReDim Tilesets(TilesetDirs.GetUpperBound(0))

        Dim Result As clsResult
        Dim Path As String

        For A = 0 To TilesetDirs.GetUpperBound(0)
            Path = TilesetDirs(A)
            Tilesets(TilesetCount) = New clsTileset
            Result = Tilesets(TilesetCount).LoadDirectory(Path)
            ReturnResult.AppendAsWarning(Result, "Loading tileset directory " & ControlChars.Quote & Path & ControlChars.Quote & ": ")
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
                tmpTileset.BGColour = New sRGB_sng(204.0# / 255.0#, 149.0# / 255.0#, 70.0# / 255.0#)
            ElseIf tmpTileset.Name = "tertilesc2hw" Then
                tmpTileset.Name = "Urban"
                Tileset_Urban = tmpTileset
                tmpTileset.IsOriginal = True
                tmpTileset.BGColour = New sRGB_sng(118.0# / 255.0#, 165.0# / 255.0#, 203.0# / 255.0#)
            ElseIf tmpTileset.Name = "tertilesc3hw" Then
                tmpTileset.Name = "Rocky Mountains"
                Tileset_Rockies = tmpTileset
                tmpTileset.IsOriginal = True
                tmpTileset.BGColour = New sRGB_sng(182.0# / 255.0#, 225.0# / 255.0#, 236.0# / 255.0#)
            End If
        Next

        If Tileset_Arizona Is Nothing Then
            ReturnResult.Warning_Add("Arizona tileset is missing.")
        End If
        If Tileset_Urban Is Nothing Then
            ReturnResult.Warning_Add("Urban tileset is missing.")
        End If
        If Tileset_Rockies Is Nothing Then
            ReturnResult.Warning_Add("Rocky Mountains tileset is missing.")
        End If

        Return ReturnResult
    End Function

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
    Public Draw_ScriptMarkers As Boolean = True

    Enum enumView_Move_Type As Byte
        Free
        RTS
    End Enum
    Public ViewMoveType As enumView_Move_Type = enumView_Move_Type.RTS
    Public RTSOrbit As Boolean = True

    Public SunAngleMatrix As New Matrix3D.Matrix3D
    Public VisionSectors As New clsBrush(0.0#, clsBrush.enumShape.Circle)

    Public Sub View_Radius_Set(ByVal Radius As Double)

        VisionSectors.Radius = Radius / (TerrainGridSpacing * SectorTileSize)
    End Sub

    Public Structure sLayerList
        Public Class clsLayer
            Public WithinLayer As Integer
            Public AvoidLayers() As Boolean
            Public Terrain As clsPainter.clsTerrain
            Public Terrainmap As clsBooleanMap
            Public HeightMin As Single
            Public HeightMax As Single
            Public SlopeMin As Single
            Public SlopeMax As Single
            'for generator only
            Public Scale As Single
            Public Density As Single
        End Class
        Public Layers() As clsLayer
        Public LayerCount As Integer

        Public Sub Layer_Insert(ByVal PositionNum As Integer, ByVal NewLayer As clsLayer)
            Dim A As Integer
            Dim B As Integer

            ReDim Preserve Layers(LayerCount)
            'shift the ones below down
            For A = LayerCount - 1 To PositionNum Step -1
                Layers(A + 1) = Layers(A)
            Next
            'insert the new entry
            Layers(PositionNum) = NewLayer
            LayerCount += 1

            For A = 0 To LayerCount - 1
                If Layers(A).WithinLayer >= PositionNum Then
                    Layers(A).WithinLayer = Layers(A).WithinLayer + 1
                End If
                ReDim Preserve Layers(A).AvoidLayers(LayerCount - 1)
                For B = LayerCount - 2 To PositionNum Step -1
                    Layers(A).AvoidLayers(B + 1) = Layers(A).AvoidLayers(B)
                Next
                Layers(A).AvoidLayers(PositionNum) = False
            Next
        End Sub

        Public Sub Layer_Remove(ByVal Layer_Num As Integer)
            Dim A As Integer
            Dim B As Integer

            LayerCount = LayerCount - 1
            For A = Layer_Num To LayerCount - 1
                Layers(A) = Layers(A + 1)
            Next
            ReDim Preserve Layers(LayerCount - 1)

            For A = 0 To LayerCount - 1
                If Layers(A).WithinLayer = Layer_Num Then
                    Layers(A).WithinLayer = -1
                ElseIf Layers(A).WithinLayer > Layer_Num Then
                    Layers(A).WithinLayer = Layers(A).WithinLayer - 1
                End If
                For B = Layer_Num To LayerCount - 1
                    Layers(A).AvoidLayers(B) = Layers(A).AvoidLayers(B + 1)
                Next
                ReDim Preserve Layers(A).AvoidLayers(LayerCount - 1)
            Next
        End Sub

        Public Sub Layer_Move(ByVal Layer_Num As Integer, ByVal Layer_Dest_Num As Integer)
            Dim Layer_Temp As clsLayer
            Dim boolTemp As Boolean
            Dim A As Integer
            Dim B As Integer

            If Layer_Dest_Num < Layer_Num Then
                'move the variables
                Layer_Temp = Layers(Layer_Num)
                For A = Layer_Num - 1 To Layer_Dest_Num Step -1
                    Layers(A + 1) = Layers(A)
                Next
                Layers(Layer_Dest_Num) = Layer_Temp
                'update the layer nums
                For A = 0 To LayerCount - 1
                    If Layers(A).WithinLayer = Layer_Num Then
                        Layers(A).WithinLayer = Layer_Dest_Num
                    ElseIf Layers(A).WithinLayer >= Layer_Dest_Num And Layers(A).WithinLayer < Layer_Num Then
                        Layers(A).WithinLayer = Layers(A).WithinLayer + 1
                    End If
                    boolTemp = Layers(A).AvoidLayers(Layer_Num)
                    For B = Layer_Num - 1 To Layer_Dest_Num Step -1
                        Layers(A).AvoidLayers(B + 1) = Layers(A).AvoidLayers(B)
                    Next
                    Layers(A).AvoidLayers(Layer_Dest_Num) = boolTemp
                Next
            ElseIf Layer_Dest_Num > Layer_Num Then
                'move the variables
                Layer_Temp = Layers(Layer_Num)
                For A = Layer_Num To Layer_Dest_Num - 1
                    Layers(A) = Layers(A + 1)
                Next
                Layers(Layer_Dest_Num) = Layer_Temp
                'update the layer nums
                For A = 0 To LayerCount - 1
                    If Layers(A).WithinLayer = Layer_Num Then
                        Layers(A).WithinLayer = Layer_Dest_Num
                    ElseIf Layers(A).WithinLayer > Layer_Num And Layers(A).WithinLayer <= Layer_Dest_Num Then
                        Layers(A).WithinLayer = Layers(A).WithinLayer - 1
                    End If
                    boolTemp = Layers(A).AvoidLayers(Layer_Num)
                    For B = Layer_Num To Layer_Dest_Num - 1
                        Layers(A).AvoidLayers(B) = Layers(A).AvoidLayers(B + 1)
                    Next
                    Layers(A).AvoidLayers(Layer_Dest_Num) = boolTemp
                Next
            End If
        End Sub
    End Structure
    Public LayerList As sLayerList

    Public Function ZipMakeEntry(ByVal ZipOutputStream As Zip.ZipOutputStream, ByVal Path As String, ByVal Result As clsResult) As Zip.ZipEntry

        Try
            Dim NewZipEntry As New Zip.ZipEntry(Path)
            NewZipEntry.DateTime = Now
            ZipOutputStream.PutNextEntry(NewZipEntry)
            Return NewZipEntry
        Catch ex As Exception
            Result.Problem_Add("Zip entry " & ControlChars.Quote & Path & ControlChars.Quote & " failed: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function InvariantToString_bool(ByVal Value As Boolean) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_byte(ByVal Value As Byte) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_short(ByVal Value As Short) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_int(ByVal Value As Integer) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_uint(ByVal Value As UInteger) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_sng(ByVal Value As Single) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantToString_dbl(ByVal Value As Double) As String

        Return Value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function InvariantParse_bool(ByVal Text As String, ByRef Result As Boolean) As Boolean

        Return Boolean.TryParse(Text, Result)
    End Function

    Public Function InvariantParse_byte(ByVal Text As String, ByRef Result As Byte) As Boolean

        Return Byte.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_short(ByVal Text As String, ByRef Result As Short) As Boolean

        Return Short.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_ushort(ByVal Text As String, ByRef Result As UShort) As Boolean

        Return UShort.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_int(ByVal Text As String, ByRef Result As Integer) As Boolean

        Return Integer.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_intB(ByVal Text As String, ByRef Succeeded As Boolean) As Integer
        Dim Result As Integer

        Succeeded = Integer.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
        Return Result
    End Function

    Public Function InvariantParse_uint(ByVal Text As String, ByRef Result As UInteger) As Boolean

        Return UInteger.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_sng(ByVal Text As String, ByRef Result As Single) As Boolean

        Return Single.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function InvariantParse_dbl(ByVal Text As String, ByRef Result As Double) As Boolean

        Return Double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, Result)
    End Function

    Public Function ReadOldText(ByVal File As IO.BinaryReader) As String
        Dim Result As String = ""
        Dim A As Integer
        Dim Length As Integer = CInt(File.ReadUInt32)

        For A = 0 To Length - 1
            Result &= Chr(File.ReadByte)
        Next
        Return Result
    End Function

    Public Function ReadOldTextOfLength(ByVal File As IO.BinaryReader, ByVal Length As Integer) As String
        Dim Result As String = ""
        Dim A As Integer

        For A = 0 To Length - 1
            Result &= Chr(File.ReadByte)
        Next
        Return Result
    End Function

    Public Sub WriteText(ByVal File As IO.BinaryWriter, ByVal WriteLength As Boolean, ByVal Text As String)

        If WriteLength Then
            File.Write(CUInt(Text.Length))
        End If
        Dim A As Integer
        For A = 0 To Text.Length - 1
            File.Write(CByte(Asc(Text(A))))
        Next
    End Sub

    Public Sub WriteTextOfLength(ByVal File As IO.BinaryWriter, ByVal Length As Integer, ByVal Text As String)

        Dim A As Integer
        For A = 0 To Math.Min(Text.Length, Length) - 1
            File.Write(CByte(Asc(Text(A))))
        Next
        For A = Text.Length To Length - 1
            File.Write(CByte(0))
        Next
    End Sub

    Public Function WriteMemoryToNewFile(ByVal Memory As IO.MemoryStream, ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult

        Dim NewFile As IO.FileStream
        Try
            NewFile = New IO.FileStream(Path, IO.FileMode.CreateNew)
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try
        Try
            Memory.WriteTo(NewFile)
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try
        
        Memory.Close()
        NewFile.Close()

        Return ReturnResult
    End Function

    Public Function WriteMemoryToZipEntryAndFlush(ByVal Memory As IO.MemoryStream, ByVal Stream As Zip.ZipOutputStream) As clsResult
        Dim ReturnResult As New clsResult

        Try
            Memory.WriteTo(Stream)
            Memory.Flush()
            Stream.Flush()
            Stream.CloseEntry()
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        Return ReturnResult
    End Function

    Public Function TryOpenFileStream(ByVal Path As String, ByRef Output As IO.FileStream) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Try
            Output = New IO.FileStream(Path, IO.FileMode.Open)
        Catch ex As Exception
            Output = Nothing
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function WZAngleFromINIText(ByVal Text As String, ByRef Result As sWZAngle) As Boolean
        Dim VectorText As New clsSplitCommaText(Text)
        Dim WZAngle As sWZAngle

        If VectorText.PartCount <> 3 Then
            Return False
        End If
        If Not InvariantParse_ushort(VectorText.Parts(0), WZAngle.Direction) Then
            Return False
        End If
        If Not InvariantParse_ushort(VectorText.Parts(1), WZAngle.Pitch) Then
            Return False
        End If
        If Not InvariantParse_ushort(VectorText.Parts(2), WZAngle.Roll) Then
            Return False
        End If
        Result = WZAngle
        Return True
    End Function

    Public Function HealthFromINIText(ByVal Text As String, ByRef Result As Integer) As Boolean
        Dim A As Integer
        Dim Health As Integer

        A = Text.IndexOf("%"c)
        If A < 0 Then
            Return False
        End If
        Text = Text.Replace("%", "")
        If Not InvariantParse_int(Text, Health) Then
            Return False
        End If
        If Health < 0 Or Health > 100 Then
            Return False
        End If
        Result = Health
        Return True
    End Function

    Public Function WorldPosFromINIText(ByVal Text As String, ByRef Result As clsWorldPos) As Boolean
        Dim VectorText As New clsSplitCommaText(Text)
        Dim A As Integer
        Dim Success As Boolean
        Dim B As Integer

        If VectorText.PartCount <> 3 Then
            Return False
        End If
        Dim tmpPositions(2) As Integer
        For A = 0 To 2
            B = InvariantParse_intB(VectorText.Parts(A), Success)
            If Success Then
                tmpPositions(A) = B
            Else
                Return False
            End If
        Next
        Result = New clsWorldPos(New sWorldPos(New sXY_int(tmpPositions(0), tmpPositions(1)), tmpPositions(2)))
        Return True
    End Function

    Public Class clsPositionFromText

        Public Pos As sXY_int

        Public Function Translate(ByVal Text As String) As Boolean
            Dim A As Integer
            Dim ParseSuccess As Boolean
            Dim tmpPositions As New clsSplitCommaText(Text)

            If tmpPositions.PartCount < 2 Then
                Return False
            End If
            A = InvariantParse_intB(tmpPositions.Parts(0), ParseSuccess)
            If ParseSuccess Then
                Pos.X = A
            Else
                Return False
            End If
            A = InvariantParse_intB(tmpPositions.Parts(1), ParseSuccess)
            If ParseSuccess Then
                Pos.Y = A
            Else
                Return False
            End If
            Return True
        End Function
    End Class
End Module