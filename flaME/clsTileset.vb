Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsTileset

    Public Num As Integer = -1

    Public Name As String

    Public IsOriginal As Boolean

    Structure sTile
        Dim MapView_GL_Texture_Num As Integer
        Dim TextureView_GL_Texture_Num As Integer
        Dim Average_Color As sRGB_sng
        Dim Default_Type As Byte
    End Structure
    Public Tiles() As sTile
    Public TileCount As Integer

    Function Default_TileTypes_Load(ByVal Path As String) As sResult
        Dim File As New clsReadFile

        Default_TileTypes_Load = File.Begin(Path)
        If Not Default_TileTypes_Load.Success Then
            Exit Function
        End If
        Default_TileTypes_Load = Default_TileTypes_Read(File)
        File.Close()
    End Function

    Private Function Default_TileTypes_Read(ByVal File As clsReadFile) As sResult
        Default_TileTypes_Read.Success = False
        Default_TileTypes_Read.Problem = ""

        Dim uintTemp As UInteger
        Dim A As Integer
        Dim ushortTemp As UShort
        Dim strTemp As String = ""

        If Not File.Get_Text(4, strTemp) Then Default_TileTypes_Read.Problem = "Read Error." : Exit Function
        If strTemp <> "ttyp" Then
            Default_TileTypes_Read.Problem = "Bad identifier."
            Exit Function
        End If

        If Not File.Get_U32(uintTemp) Then Default_TileTypes_Read.Problem = "Read Error." : Exit Function
        If Not uintTemp = 8UI Then Default_TileTypes_Read.Problem = "Unknown version." : Exit Function

        If Not File.Get_U32(uintTemp) Then Default_TileTypes_Read.Problem = "Read Error." : Exit Function
        TileCount = uintTemp
        ReDim Tiles(TileCount - 1)

        For A = 0 To Math.Min(uintTemp, TileCount) - 1
            If Not File.Get_U16(ushortTemp) Then Default_TileTypes_Read.Problem = "Read Error." : Exit Function
            If ushortTemp > 11US Then Default_TileTypes_Read.Problem = "Unknown tile type." : Exit Function
            Tiles(A).Default_Type = ushortTemp
        Next

        Default_TileTypes_Read.Success = True
    End Function

    Public Function LoadDirectory(ByVal Path As String) As clsResult
        LoadDirectory = New clsResult

        Dim tmpBitmap As clsBitmapFile
        Dim tmpBitmap8 As New clsBitmapFile
        Dim tmpBitmap4 As New clsBitmapFile
        Dim tmpBitmap2 As New clsBitmapFile
        Dim tmpBitmap1 As New clsBitmapFile
        Dim SplitPath As New sSplitPath(Path)
        Dim SlashPath As String = EndWithPathSeperator(Path)
        Dim Result As sResult

        If SplitPath.FileTitle <> "" Then
            Name = SplitPath.FileTitle
        ElseIf SplitPath.PartCount >= 2 Then
            Name = SplitPath.Parts(SplitPath.PartCount - 2)
        End If

        Result = Default_TileTypes_Load(SlashPath & Name & ".ttp")
        If Not Result.Success Then
            LoadDirectory.Problem_Add("Loading tile types; " & Result.Problem)
            Exit Function
        End If

        Dim PixX As Integer
        Dim PixY As Integer
        Dim RedTotal As Integer
        Dim GreenTotal As Integer
        Dim BlueTotal As Integer
        Dim PixelColor As Color
        Dim PixelColorA As Color
        Dim PixelColorB As Color
        Dim PixelColorC As Color
        Dim PixelColorD As Color
        Dim TileNum As Integer
        Dim strTile As String
        Dim PixelCountForAverage As Single = 4177920.0F '128*128*255
        Dim X1 As Integer
        Dim Y1 As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim Red As Integer
        Dim Green As Integer
        Dim Blue As Integer

        Dim GraphicPath As String

        'tile count has been set by the ttp file

        For TileNum = 0 To TileCount - 1
            strTile = "tile-" & MinDigits(TileNum, 2) & ".png"

            '-------- 128 --------

            GraphicPath = SlashPath & Name & "-128" & OSPathSeperator & strTile

            RedTotal = 0
            GreenTotal = 0
            BlueTotal = 0

            tmpBitmap = New clsBitmapFile
            Result = tmpBitmap.Load(GraphicPath)
            If Not Result.Success Then
                'ignore and exit, since not all tile types have a corresponding tile graphic
                Exit Function
            End If

            If tmpBitmap.CurrentBitmap.Width <> 128 Or tmpBitmap.CurrentBitmap.Height <> 128 Then
                LoadDirectory.Warning_Add("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 128x128.")
                Exit Function
            End If

            For PixY = 0 To 127
                For PixX = 0 To 127
                    PixelColor = tmpBitmap.CurrentBitmap.GetPixel(PixX, PixY)
                    'Texture128(PixY, PixX, 0) = PixelColor.R
                    'Texture128(PixY, PixX, 1) = PixelColor.G
                    'Texture128(PixY, PixX, 2) = PixelColor.B
                    'If PixelColor.A < 255 Then
                    '    Texture128(PixY, PixX, 0) = 16
                    '    Texture128(PixY, PixX, 1) = 64
                    '    Texture128(PixY, PixX, 2) = 128
                    'End If
                    'Texture128(PixY, PixX, 3) = 255

                    RedTotal += PixelColor.R
                    GreenTotal += PixelColor.G
                    BlueTotal += PixelColor.B
                Next
            Next

            Tiles(TileNum).Average_Color.Red = RedTotal / PixelCountForAverage
            Tiles(TileNum).Average_Color.Green = GreenTotal / PixelCountForAverage
            Tiles(TileNum).Average_Color.Blue = BlueTotal / PixelCountForAverage

            Tiles(TileNum).TextureView_GL_Texture_Num = tmpBitmap.GLTexture(frmMainInstance.TextureView.OpenGLControl, False)
            Tiles(TileNum).MapView_GL_Texture_Num = tmpBitmap.GLTexture(frmMainInstance.View.OpenGLControl, True)

            '-------- 64 --------

            GraphicPath = SlashPath & Name & "-64" & OSPathSeperator & strTile

            tmpBitmap = New clsBitmapFile
            Result = tmpBitmap.Load(GraphicPath)
            If Not Result.Success Then
                LoadDirectory.Warning_Add("Unable to load tile graphic; " & Result.Problem)
                Exit Function
            End If

            If tmpBitmap.CurrentBitmap.Width <> 64 Or tmpBitmap.CurrentBitmap.Height <> 64 Then
                LoadDirectory.Warning_Add("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 64x64.")
                Exit Function
            End If

            tmpBitmap.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 1)

            '-------- 32 --------

            GraphicPath = SlashPath & Name & "-32" & OSPathSeperator & strTile

            tmpBitmap = New clsBitmapFile
            Result = tmpBitmap.Load(GraphicPath)
            If Not Result.Success Then
                LoadDirectory.Warning_Add("Unable to load tile graphic; " & Result.Problem)
                Exit Function
            End If

            If tmpBitmap.CurrentBitmap.Width <> 32 Or tmpBitmap.CurrentBitmap.Height <> 32 Then
                LoadDirectory.Warning_Add("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 32x32.")
                Exit Function
            End If

            tmpBitmap.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 2)

            '-------- 16 --------

            GraphicPath = SlashPath & Name & "-16" & OSPathSeperator & strTile

            tmpBitmap = New clsBitmapFile
            Result = tmpBitmap.Load(GraphicPath)
            If Not Result.Success Then
                LoadDirectory.Warning_Add("Unable to load tile graphic; " & Result.Problem)
                Exit Function
            End If

            If tmpBitmap.CurrentBitmap.Width <> 16 Or tmpBitmap.CurrentBitmap.Height <> 16 Then
                LoadDirectory.Warning_Add("Tile graphic " & GraphicPath & " from tileset " & Name & " is not 16x16.")
                Exit Function
            End If

            tmpBitmap.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 3)

            '-------- 8 --------

            tmpBitmap8.CurrentBitmap = New Bitmap(8, 8, Imaging.PixelFormat.Format32bppArgb)
            For PixY = 0 To 7
                Y1 = PixY * 2
                Y2 = Y1 + 1
                For PixX = 0 To 7
                    X1 = PixX * 2
                    X2 = X1 + 1
                    PixelColorA = tmpBitmap.CurrentBitmap.GetPixel(X1, Y1)
                    PixelColorB = tmpBitmap.CurrentBitmap.GetPixel(X2, Y1)
                    PixelColorC = tmpBitmap.CurrentBitmap.GetPixel(X1, Y2)
                    PixelColorD = tmpBitmap.CurrentBitmap.GetPixel(X2, Y2)
                    Red = (CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F
                    Green = (CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F
                    Blue = (CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F
                    tmpBitmap8.CurrentBitmap.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
                Next
            Next

            tmpBitmap8.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 4)

            '-------- 4 --------

            tmpBitmap4.CurrentBitmap = New Bitmap(4, 4, Imaging.PixelFormat.Format32bppArgb)
            For PixY = 0 To 3
                Y1 = PixY * 2
                Y2 = Y1 + 1
                For PixX = 0 To 3
                    X1 = PixX * 2
                    X2 = X1 + 1
                    PixelColorA = tmpBitmap.CurrentBitmap.GetPixel(X1, Y1)
                    PixelColorB = tmpBitmap.CurrentBitmap.GetPixel(X2, Y1)
                    PixelColorC = tmpBitmap.CurrentBitmap.GetPixel(X1, Y2)
                    PixelColorD = tmpBitmap.CurrentBitmap.GetPixel(X2, Y2)
                    Red = (CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F
                    Green = (CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F
                    Blue = (CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F
                    tmpBitmap4.CurrentBitmap.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
                Next
            Next

            tmpBitmap4.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 5)

            '-------- 2 --------

            tmpBitmap2.CurrentBitmap = New Bitmap(2, 2, Imaging.PixelFormat.Format32bppArgb)
            For PixY = 0 To 1
                Y1 = PixY * 2
                Y2 = Y1 + 1
                For PixX = 0 To 1
                    X1 = PixX * 2
                    X2 = X1 + 1
                    PixelColorA = tmpBitmap.CurrentBitmap.GetPixel(X1, Y1)
                    PixelColorB = tmpBitmap.CurrentBitmap.GetPixel(X2, Y1)
                    PixelColorC = tmpBitmap.CurrentBitmap.GetPixel(X1, Y2)
                    PixelColorD = tmpBitmap.CurrentBitmap.GetPixel(X2, Y2)
                    Red = (CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F
                    Green = (CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F
                    Blue = (CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F
                    tmpBitmap2.CurrentBitmap.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))
                Next
            Next

            tmpBitmap2.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 6)

            '-------- 1 --------

            tmpBitmap1.CurrentBitmap = New Bitmap(1, 1, Imaging.PixelFormat.Format32bppArgb)
            PixX = 0
            PixY = 0
            Y1 = PixY * 2
            Y2 = Y1 + 1
            X1 = PixX * 2
            X2 = X1 + 1
            PixelColorA = tmpBitmap.CurrentBitmap.GetPixel(X1, Y1)
            PixelColorB = tmpBitmap.CurrentBitmap.GetPixel(X2, Y1)
            PixelColorC = tmpBitmap.CurrentBitmap.GetPixel(X1, Y2)
            PixelColorD = tmpBitmap.CurrentBitmap.GetPixel(X2, Y2)
            Red = (CInt(PixelColorA.R) + PixelColorB.R + PixelColorC.R + PixelColorD.R) / 4.0F
            Green = (CInt(PixelColorA.G) + PixelColorB.G + PixelColorC.G + PixelColorD.G) / 4.0F
            Blue = (CInt(PixelColorA.B) + PixelColorB.B + PixelColorC.B + PixelColorD.B) / 4.0F
            tmpBitmap1.CurrentBitmap.SetPixel(PixX, PixY, ColorTranslator.FromOle(OSRGB(Red, Green, Blue)))

            tmpBitmap1.GLTexture(frmMainInstance.View.OpenGLControl, Tiles(TileNum).MapView_GL_Texture_Num, 7)
        Next
    End Function
End Class