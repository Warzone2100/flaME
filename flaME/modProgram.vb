Public Module modProgram

#If OS = 0.0# Then
    Public Const ProgramVersion As String = "1.17 Windows"
#Else
    Public Const ProgramVersion As String = "1.17 Mono"
#End If

    Public Const SaveVersion As UInteger = 6UI

    Public Const FactionCountMax As Integer = 11

#If OS = 0.0# Then
    Public Const MinimapDelay As Integer = 100
#Else
    Public Const MinimapDelay As Integer = 1000
#End If

    Public OSPathSeperator As Char

    Public TilesetPath As String

    Public MyDocumentsPath As String

    Public SettingsPath As String
    Public AutoSavePath As String

#If OS <> 0.0# Then
    Public InterfaceImagesPath As String
#End If

    Public Sub SetProgramSubDirs()

        TilesetPath = EndWithPathSeperator(My.Application.Info.DirectoryPath) & "tilesets" & OSPathSeperator
        MyDocumentsPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments & OSPathSeperator & ".flaME"
        SettingsPath = MyDocumentsPath & OSPathSeperator & "settings"
        AutoSavePath = MyDocumentsPath & OSPathSeperator & "autosave" & OSPathSeperator
#If OS <> 0.0# Then
        InterfaceImagesPath = My.Application.Info.DirectoryPath & OSPathSeperator & "interface" & OSPathSeperator
#End If
    End Sub

    Public Const SectorTileSize As Integer = 8

    Public Undo_Limit As UInteger = 256UI

    Public AutoSave_MinInterval_s As UInteger = 180UI
    Public AutoSave_MinChanges As UInteger = 20UI

    Public DirectPointer As Boolean = True

    Public GLTexture_NoTile As Integer
    'Public GLTexture_BlackTile As Integer
    'Public GLTexture_WhiteTile As Integer
    Public GLTexture_OverflowTile As Integer

    Public SunHeading As Double = 157.5# * RadOf1Deg
    Public SunPitch As Double = 22.5# * RadOf1Deg

    Public frmMainInstance As New frmMain
#If OS = 0.0# Then
    Public frmSplashInstance As New frmSplash
#End If
    Public frmCompileInstance As New frmCompile
    Public frmMapTexturerInstance As New frmMapTexturer

    Public IsViewKeyDown(255) As Boolean

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
    Public Control_View_Move_Type As clsInputControl
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
    Public Control_Anticlockwise As clsInputControl
    Public Control_Texture_Flip As clsInputControl
    Public Control_Tri_Flip As clsInputControl
    'gateway controls
    Public Control_Gateway_Delete As clsInputControl
    'undo controls
    Public Control_Undo As clsInputControl
    Public Control_Redo As clsInputControl

    Public TextureBrushRadius As sBrushTiles
    Public AutoTextureBrushRadius As sBrushTiles
    Public HeightBrushRadius As sBrushTiles

    Public SmoothRadius As sBrushTiles
    Public AutoCliffBrushRadius As sBrushTiles

    Public InputControls() As clsInputControl
    Public InputControlCount As Integer

    Public Const FOVDefault As Double = 30.0# / (50.0# * 900.0#) ' screen_vertical_size / ( screen_dist * screen_vertical_pixels )

    Public DisplayTileOrientation As Boolean

    Public Enum enumTool As Byte
        None
        Texture_Brush
        AutoTexture_Place
        AutoTexture_Fill
        AutoRoad_Place
        AutoRoad_Line
        AutoCliff
        Height_Set_Brush
        Height_Change_Brush
        Height_Smooth_Brush
        Object_Feature
        Object_Structure
        Object_Unit
        Terrain_Select
        Gateways
    End Enum
    Public Tool As enumTool = enumTool.Texture_Brush

    Public SelectedTexture As Integer = -1
    Public TextureOrientation As New sTileOrientation(False, False, False)

    Public SelectedTerrain As sPainter.clsTerrain
    Public SelectedRoad As sPainter.clsRoad

    Public Structure sTileType
        Dim Name As String
        Dim DisplayColour As sRGB_sng
    End Structure
    Public TileTypes(-1) As sTileType
    Public TileTypeCount As Integer

    Public TileType_WaterNum As Integer = 7

    Public Structure sResult
        Dim Success As Boolean
        Dim Problem As String
    End Structure

    Public Structure sTextLabel
        Dim Text As String
        Dim Font As GLFont
        Dim SizeY As Single
        Dim Colour As sRGBA_sng
        Dim Pos As sXY_int
    End Structure

    Public Structure sRGB_sng
        Dim Red As Single
        Dim Green As Single
        Dim Blue As Single
    End Structure

    Public Structure sRGBA_sng
        Dim Red As Single
        Dim Green As Single
        Dim Blue As Single
        Dim Alpha As Single
    End Structure

    Public Structure sBrushTiles
        Dim XMin() As Integer
        Dim XMax() As Integer
        Dim ZMin As Integer
        Dim ZMax As Integer
    End Structure

    Public Structure sSplitPath

        Public Parts() As String
        Public PartCount As Integer
        Public FilePath As String
        Public FileTitle As String
        Public FileTitleWithoutExtension As String
        Public FileExtension As String

        Sub New(ByVal Path As String)
            Dim tmpPath As String = Path
            Dim A As Integer

            Parts = Strings.Split(tmpPath, OSPathSeperator)
            PartCount = Parts.GetUpperBound(0) + 1
            FilePath = ""
            For A = 0 To PartCount - 2
                FilePath &= Parts(A) & OSPathSeperator
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

        Sub New(ByVal Path As String)
            Dim tmpPath As String = Strings.LCase(Path)
            Dim A As Integer

            tmpPath = tmpPath.Replace("\"c, "/"c)

            Parts = Strings.Split(tmpPath, "/")
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

    Public TerrainGridSpacing As Integer = 128

    Public VisionRadius_2E As Integer
    Public VisionRadius As Double

    Public Map As clsMap

    Public Copied_Map As clsMap

    Public Tilesets() As clsTileset
    Public TilesetCount As Integer

    Public Tileset_Arizona As clsTileset
    Public Tileset_Urban As clsTileset
    Public Tileset_Rockies As clsTileset

    Public UnitTypes(-1) As clsUnitType
    Public UnitTypeCount As Integer

    Public Structure sTexturePage
        Dim FileTitle As String
        Dim GLTexture_Num As Integer
    End Structure
    Public TexturePages() As sTexturePage
    Public TexturePageCount As Integer

    Public UnitLabelFont As GLFont
    Public TextureViewFont As GLFont
    Public UnitLabelFontSize As Single

    Public GL_Current As Byte = 0

    Public PlayerColour(15) As sRGB_sng

    Public Sub VisionRadius_2E_Changed()

        VisionRadius = 256.0# * 2.0# ^ (VisionRadius_2E / 2.0#)
        If frmMainInstance.View IsNot Nothing Then
            frmMainInstance.View.View_Radius_Set(VisionRadius)
            frmMainInstance.View.DrawViewLater()
        End If
    End Sub

    Public Sub CircleTiles_Create(ByVal Radius As Double, ByRef Output As sBrushTiles, ByVal TileSize As Double)
        Dim X As Double
        Dim Z As Integer
        Dim Radius2 As Double
        Dim Radius3 As Double

        Radius2 = Radius / TileSize
        Output.ZMax = Math.Floor(Radius2)
        Output.ZMin = -Output.ZMax
        ReDim Output.XMin(Output.ZMax - Output.ZMin)
        ReDim Output.XMax(Output.ZMax - Output.ZMin)
        Radius3 = Radius2 * Radius2
        For Z = Output.ZMin To Output.ZMax
            X = Math.Sqrt(Radius3 - Z * Z)
            Output.XMax(Z - Output.ZMin) = Math.Floor(X)
            Output.XMin(Z - Output.ZMin) = -Output.XMax(Z - Output.ZMin)
        Next
    End Sub

    Public Sub SquareTiles_Create(ByVal Radius As Double, ByRef Output As sBrushTiles, ByVal TileSize As Double)
        Dim Z As Integer
        Dim A As Integer
        Dim B As Integer

        A = Math.Floor(Radius / TileSize)
        Output.ZMin = -A
        Output.ZMax = A
        B = A * 2
        ReDim Output.XMin(B)
        ReDim Output.XMax(B)
        For Z = 0 To B
            Output.XMin(Z) = -A
            Output.XMax(Z) = A
        Next
    End Sub

    Public Function Get_TexturePage_GLTexture(ByVal FileTitle As String) As Integer
        Dim LCaseTitle As String = LCase(FileTitle)
        Dim A As Integer

        For A = 0 To TexturePageCount - 1
            If LCase(TexturePages(A).FileTitle) = LCaseTitle Then
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

        If Strings.Right(Text, 1) = OSPathSeperator Then
            EndWithPathSeperator = Text
        Else
            EndWithPathSeperator = Text & OSPathSeperator
        End If
    End Function

    Public Function TileType_Add(ByVal NewTileType As sTileType) As Integer

        ReDim Preserve TileTypes(TileTypeCount)
        TileTypes(TileTypeCount) = NewTileType
        TileType_Add = TileTypeCount
        TileTypeCount += 1
    End Function

    Public Function MinDigits(ByVal Number As Integer, ByVal Digits As Integer) As String
        Dim A As Integer

        MinDigits = Number
        A = Digits - MinDigits.Length
        If A > 0 Then
            MinDigits = Strings.StrDup(A, "0"c) & MinDigits
        End If
    End Function

    Public Sub ViewKeyDown_Clear()
        Dim A As Integer

        For A = 0 To 255
            IsViewKeyDown(A) = False
        Next A

        For A = 0 To InputControlCount - 1
            InputControls(A).KeysChanged(IsViewKeyDown)
        Next
    End Sub

    Public Function OSRGB(ByVal Red As Integer, ByVal Green As Integer, ByVal Blue As Integer) As Integer
#If OS = 0.0# Then
        OSRGB = RGB(Red, Green, Blue)
#Else
        OSRGB = RGB(Blue, Green, Red)
#End If
    End Function
End Module