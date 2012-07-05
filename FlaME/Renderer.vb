Imports OpenTK.Graphics.OpenGL

Public Class clsRendererTerrain

    Public Number As Integer = -1
    Public Name As String
    Public GLTexture As Integer
    Public Span As Single
    Public IsCliff As Boolean
    Public IsWater As Boolean
End Class

Public Module modRenderer

    Private RendererTextures As New SimpleList(Of clsObjectData.clsTexturePage)

    Public StoreRendererTerrain As Boolean = False
    Public StoreOldTerrain As Boolean = True

    Private Function GetRendererGLTexture(FileTitle As String) As Integer
        Dim LCaseTitle As String = FileTitle.ToLower
        Dim TexPage As clsObjectData.clsTexturePage
        Dim length As Integer = LCaseTitle.Length

        For Each TexPage In RendererTextures
            If Strings.Left(TexPage.FileTitle, length).ToLower = LCaseTitle Then
                Return TexPage.GLTextureNum
            End If
        Next
        Return 0
    End Function

    Public Class clsRendererTilesetLoad

        Public Name As String
        Public TerrainsEnumFile As String
        Public TerrainTexturesFile As String
        Public TileCornersFile As String
        Public TileDecalsFile As String
        Public DestinationTileset As clsTileset
        Public DataSet As clsObjectData
        Public TexturesPath As String

        Public Function Load(path As String) As clsResult
            Dim ReturnResult As New clsResult(Name)

            path = EndWithPathSeparator(path)

            ReturnResult.Add(EnumFile(path))
            ReturnResult.Add(TexturesFile(path))
            ReturnResult.Add(CornersFile(path))
            ReturnResult.Add(DecalsFile(path))

            Return ReturnResult
        End Function

        Private Function EnumFile(Path As String) As clsResult
            Dim returnResult As New clsResult("Enum")

            Dim reader As IO.StreamReader = Nothing
            returnResult.Take(OpenStreamReader(Path & TerrainsEnumFile, reader))
            If reader Is Nothing Then
                Return returnResult
            End If

            Try
                Dim firstLine() As String = reader.ReadLine.Split(","c)
                Dim count As Integer
                InvariantParse_int(firstLine(1), count)
                For i As Integer = 0 To count - 1
                    Dim newTerrain As New clsRendererTerrain
                    newTerrain.Name = reader.ReadLine
                    newTerrain.IsCliff = (Strings.Right(newTerrain.Name, 6) = "_cliff")
                    newTerrain.IsWater = (Strings.Right(newTerrain.Name, 6) = "_water")
                    DestinationTileset.RendererTerrains.Add(newTerrain)
                Next
            Catch ex As Exception
                reader.Close()
                returnResult.ProblemAdd(ex.Message)
                Return returnResult
            End Try

            reader.Close()

            Return returnResult
        End Function

        Private Function TexturesFile(Path As String) As clsResult
            Dim returnResult As New clsResult("Textures")

            Dim reader As IO.StreamReader = Nothing
            returnResult.Take(OpenStreamReader(Path & TerrainTexturesFile, reader))
            If reader Is Nothing Then
                Return returnResult
            End If

            Try
                Dim firstLine() As String = reader.ReadLine.Split(","c)
                Dim count As Integer
                Dim decalsFolder As String = firstLine(0)
                InvariantParse_int(firstLine(1), count)
                For i As Integer = 0 To count - 1
                    Dim parts() As String = reader.ReadLine.Split(","c)
                    Dim terrainName As String = parts(0)
                    Dim texPageName As String = parts(1)
                    Dim terrain As clsRendererTerrain = DestinationTileset.FindRendererTerrain(terrainName)
                    terrain.GLTexture = GetRendererGLTexture(texPageName)
                    If terrain.GLTexture = 0 Then
                        returnResult.WarningAdd(ControlChars.Quote & texPageName & ControlChars.Quote & " texture not found.")
                    End If
                    If InvariantParse_sng(parts(2), terrain.Span) Then
                        terrain.Span *= TerrainGridSpacing
                    Else

                    End If
                    For j As Integer = 0 To DestinationTileset.TileCount - 1
                        Dim bitmap As Bitmap = Nothing
                        If LoadBitmap(EndWithPathSeparator(TexturesPath) & decalsFolder & "-128" & PlatformPathSeparator & "tile-" & MinDigits(j, 2) & ".png", bitmap).Success Then
                            returnResult.Take(BitmapIsGLCompatible(bitmap))
                            Dim bitmapTextureArgs As New clsBitmapGLTexture
                            bitmapTextureArgs.MagFilter = TextureMagFilter.Linear
                            bitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
                            bitmapTextureArgs.TextureNum = 0
                            bitmapTextureArgs.MipMapLevel = 0
                            bitmapTextureArgs.Texture = bitmap
                            bitmapTextureArgs.Perform()
                            DestinationTileset.Tiles(j).DecalGLTexture = bitmapTextureArgs.TextureNum
                        End If
                    Next
                Next
            Catch ex As Exception
                reader.Close()
                returnResult.ProblemAdd(ex.Message)
                Return returnResult
            End Try

            reader.Close()

            Return returnResult
        End Function

        Private Function CornersFile(Path As String) As clsResult
            Dim returnResult As New clsResult("Textures")

            Dim reader As IO.StreamReader = Nothing
            returnResult.Take(OpenStreamReader(Path & TileCornersFile, reader))
            If reader Is Nothing Then
                Return returnResult
            End If

            Try
                Dim firstLine() As String = reader.ReadLine.Split(","c)
                Dim count As Integer
                InvariantParse_int(firstLine(1), count)
                For i As Integer = 0 To count - 1
                    Dim parts() As String = reader.ReadLine.Split(","c)
                    DestinationTileset.Tiles(i).CornerTerrains(0, 0) = DestinationTileset.FindRendererTerrain(parts(0))
                    DestinationTileset.Tiles(i).CornerTerrains(1, 0) = DestinationTileset.FindRendererTerrain(parts(1))
                    DestinationTileset.Tiles(i).CornerTerrains(0, 1) = DestinationTileset.FindRendererTerrain(parts(2))
                    DestinationTileset.Tiles(i).CornerTerrains(1, 1) = DestinationTileset.FindRendererTerrain(parts(3))
                Next
            Catch ex As Exception
                reader.Close()
                returnResult.ProblemAdd(ex.Message)
                Return returnResult
            End Try

            reader.Close()

            Return returnResult
        End Function

        Private Function DecalsFile(Path As String) As clsResult
            Dim returnResult As New clsResult("Textures")

            Dim reader As IO.StreamReader = Nothing
            returnResult.Take(OpenStreamReader(Path & TileDecalsFile, reader))
            If reader Is Nothing Then
                Return returnResult
            End If

            Try
                Dim firstLine() As String = reader.ReadLine.Split(","c)
                Dim count As Integer
                InvariantParse_int(firstLine(1), count)
                For i As Integer = 0 To count - 1
                    Dim tileNum As Integer
                    If Not InvariantParse_int(reader.ReadLine, tileNum) Then
                        Continue For
                    End If
                    If tileNum < 0 And tileNum >= DestinationTileset.TileCount Then
                        Continue For
                    End If
                    DestinationTileset.Tiles(tileNum).UseDecal = True
                Next
            Catch ex As Exception
                reader.Close()
                returnResult.ProblemAdd(ex.Message)
                Return returnResult
            End Try

            reader.Close()

            Return returnResult
        End Function
    End Class

    Public Function LoadRendererTilesets(Path As String) As clsResult
        Dim ReturnResult As New clsResult("Loading v3 terrain data from " & ControlChars.Quote & Path & ControlChars.Quote)

        'todo, merge this part with objectdata texpages method

        Path = EndWithPathSeparator(Path)

        Dim texturesPath As String = Path & "texpages"
        Dim texFiles() As String
        Try
            texFiles = IO.Directory.GetFiles(texturesPath)
        Catch ex As Exception
            ReturnResult.WarningAdd("Unable to access texture pages.")
            ReDim texFiles(-1)
        End Try

        Dim text As String
        Dim bitmap As Bitmap = Nothing
        Dim instrPos2 As Integer
        Dim bitmapTextureArgs As New clsBitmapGLTexture
        Dim bitmapResult As sResult

        For Each text In texFiles
            If Right(text, 4).ToLower = ".png" Then
                Dim Result As New clsResult("Loading texture page " & ControlChars.Quote & text & ControlChars.Quote)
                If IO.File.Exists(text) Then
                    bitmapResult = LoadBitmap(text, bitmap)
                    Dim NewPage As New clsObjectData.clsTexturePage
                    If bitmapResult.Success Then
                        Result.Take(BitmapIsGLCompatible(bitmap))
                        bitmapTextureArgs.MagFilter = TextureMagFilter.Linear
                        bitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
                        bitmapTextureArgs.WrapMode = TextureWrapMode.Repeat
                        bitmapTextureArgs.TextureNum = 0
                        bitmapTextureArgs.MipMapLevel = 0
                        bitmapTextureArgs.Texture = bitmap
                        bitmapTextureArgs.Perform()
                        NewPage.GLTextureNum = bitmapTextureArgs.TextureNum
                    Else
                        Result.WarningAdd(bitmapResult.Problem)
                    End If
                    instrPos2 = InStrRev(text, PlatformPathSeparator)
                    NewPage.FileTitle = Strings.Mid(text, instrPos2 + 1, text.Length - 4 - instrPos2)
                    RendererTextures.Add(NewPage)
                End If
                ReturnResult.Add(Result)
            End If
        Next

        Dim RendererTilesetLoads As New SimpleList(Of clsRendererTilesetLoad)

        If Tileset_Arizona IsNot Nothing Then
            Dim Arizona As New clsRendererTilesetLoad
            Arizona.Name = "Arizona"
            Arizona.TexturesPath = texturesPath
            Arizona.DestinationTileset = Tileset_Arizona
            Arizona.DataSet = ObjectData
            Arizona.TerrainsEnumFile = "arizona_enum.txt"
            Arizona.TerrainTexturesFile = "tertilesc1hwGtype.txt"
            Arizona.TileCornersFile = "arizonaground.txt"
            Arizona.TileDecalsFile = "arizonadecals.txt"
            RendererTilesetLoads.Add(Arizona)
        End If

        If Tileset_Urban IsNot Nothing Then
            Dim Urban As New clsRendererTilesetLoad
            Urban.Name = "Urban"
            Urban.TexturesPath = texturesPath
            Urban.DestinationTileset = Tileset_Urban
            Urban.DataSet = ObjectData
            Urban.TerrainsEnumFile = "urban_enum.txt"
            Urban.TerrainTexturesFile = "tertilesc2hwGtype.txt"
            Urban.TileCornersFile = "urbanground.txt"
            Urban.TileDecalsFile = "urbandecals.txt"
            RendererTilesetLoads.Add(Urban)
        End If

        If Tileset_Rockies IsNot Nothing Then
            Dim Rockies As New clsRendererTilesetLoad
            Rockies.Name = "Rockies"
            Rockies.TexturesPath = texturesPath
            Rockies.DestinationTileset = Tileset_Rockies
            Rockies.DataSet = ObjectData
            Rockies.TerrainsEnumFile = "rockie_enum.txt"
            Rockies.TerrainTexturesFile = "tertilesc3hwGtype.txt"
            Rockies.TileCornersFile = "rockieground.txt"
            Rockies.TileDecalsFile = "rockiedecals.txt"
            RendererTilesetLoads.Add(Rockies)
        End If

        For Each tileset As clsRendererTilesetLoad In RendererTilesetLoads
            ReturnResult.Add(tileset.Load(Path & "tileset"))
        Next

        Return ReturnResult
    End Function

    Public Function RendererNormalLighting(normal As Matrix3D.XYZ_dbl) As Double

        Return Clamp_dbl(1.5# - Matrix3D.MatrixAngleToVector(SunAngleMatrix, normal) / RadOf90Deg, 0.33333333333333331#, 1.0#)
    End Function
End Module
