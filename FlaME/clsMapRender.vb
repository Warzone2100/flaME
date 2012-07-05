Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public MustInherit Class clsDrawTile

        Public Map As clsMap
        Public TileX As Integer
        Public TileY As Integer

        Public MustOverride Sub Perform()
    End Class

    Public Class clsDrawTileOld
        Inherits clsDrawTile

        Public Overrides Sub Perform()
            Dim Terrain As clsTerrain = Map.Terrain
            Dim Tileset As clsTileset = Map.Tileset
            Dim TileTerrainHeight(3) As Double
            Dim Vertex0 As sXYZ_sng
            Dim Vertex1 As sXYZ_sng
            Dim Vertex2 As sXYZ_sng
            Dim Vertex3 As sXYZ_sng
            Dim Normal0 As sXYZ_sng
            Dim Normal1 As sXYZ_sng
            Dim Normal2 As sXYZ_sng
            Dim Normal3 As sXYZ_sng
            Dim TexCoord0 As sXY_sng
            Dim TexCoord1 As sXY_sng
            Dim TexCoord2 As sXY_sng
            Dim TexCoord3 As sXY_sng
            Dim A As Integer

            If Terrain.Tiles(TileX, TileY).Texture.TextureNum < 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_NoTile)
            ElseIf Tileset Is Nothing Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            ElseIf Terrain.Tiles(TileX, TileY).Texture.TextureNum < Tileset.TileCount Then
                A = Tileset.Tiles(Terrain.Tiles(TileX, TileY).Texture.TextureNum).MapViewGLTexture
                If A = 0 Then
                    GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
                Else
                    GL.BindTexture(TextureTarget.Texture2D, A)
                End If
            Else
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            End If
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)

            TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
            TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
            TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
            TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

            GetTileRotatedTexCoords(Terrain.Tiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

            Vertex0.X = CSng(TileX * TerrainGridSpacing)
            Vertex0.Y = CSng(TileTerrainHeight(0) * Map.HeightMultiplier)
            Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex1.Y = CSng(TileTerrainHeight(1) * Map.HeightMultiplier)
            Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex2.X = CSng(TileX * TerrainGridSpacing)
            Vertex2.Y = CSng(TileTerrainHeight(2) * Map.HeightMultiplier)
            Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
            Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex3.Y = CSng(TileTerrainHeight(3) * Map.HeightMultiplier)
            Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

            Normal0 = Map.TerrainVertexNormalCalc(TileX, TileY)
            Normal1 = Map.TerrainVertexNormalCalc(TileX + 1, TileY)
            Normal2 = Map.TerrainVertexNormalCalc(TileX, TileY + 1)
            Normal3 = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1)

            GL.Begin(BeginMode.Triangles)
            If Terrain.Tiles(TileX, TileY).Tri Then
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)

                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            Else
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)

                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            End If
            GL.End()
        End Sub
    End Class

    Public Class clsDrawTileDecal
        Inherits clsDrawTile

        Public Overrides Sub Perform()
            Dim Terrain As clsTerrain = Map.Terrain
            Dim Tileset As clsTileset = Map.Tileset
            Dim TileTerrainHeight(3) As Double
            Dim Vertex0 As sXYZ_sng
            Dim Vertex1 As sXYZ_sng
            Dim Vertex2 As sXYZ_sng
            Dim Vertex3 As sXYZ_sng
            Dim Normal0 As sXYZ_sng
            Dim Normal1 As sXYZ_sng
            Dim Normal2 As sXYZ_sng
            Dim Normal3 As sXYZ_sng
            Dim TexCoord0 As sXY_sng
            Dim TexCoord1 As sXY_sng
            Dim TexCoord2 As sXY_sng
            Dim TexCoord3 As sXY_sng

            If Terrain.Tiles(TileX, TileY).Texture.TextureNum < 0 Then
                Exit Sub
            ElseIf Tileset Is Nothing Then
                Exit Sub
            ElseIf Terrain.Tiles(TileX, TileY).Texture.TextureNum < Tileset.TileCount Then
                Dim texture As clsTileset.sTile = Tileset.Tiles(Terrain.Tiles(TileX, TileY).Texture.TextureNum)
                If Not texture.UseDecal Then
                    Exit Sub
                End If
                If texture.DecalGLTexture = 0 Then
                    Exit Sub
                End If
                GL.BindTexture(TextureTarget.Texture2D, texture.DecalGLTexture)
            Else
                Exit Sub
            End If
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)

            TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height + 1.0#
            TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height + 1.0#
            TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height + 1.0#
            TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height + 1.0#

            GetTileRotatedTexCoords(Terrain.Tiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

            Vertex0.X = CSng(TileX * TerrainGridSpacing)
            Vertex0.Y = CSng(TileTerrainHeight(0) * Map.HeightMultiplier)
            Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex1.Y = CSng(TileTerrainHeight(1) * Map.HeightMultiplier)
            Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
            Vertex2.X = CSng(TileX * TerrainGridSpacing)
            Vertex2.Y = CSng(TileTerrainHeight(2) * Map.HeightMultiplier)
            Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
            Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
            Vertex3.Y = CSng(TileTerrainHeight(3) * Map.HeightMultiplier)
            Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

            Normal0 = Map.TerrainVertexNormalCalc(TileX, TileY)
            Normal1 = Map.TerrainVertexNormalCalc(TileX + 1, TileY)
            Normal2 = Map.TerrainVertexNormalCalc(TileX, TileY + 1)
            Normal3 = Map.TerrainVertexNormalCalc(TileX + 1, TileY + 1)

            Dim normal As Matrix3D.XYZ_dbl
            normal.X += CDbl(Normal0.X)
            normal.Y += CDbl(Normal0.Y)
            normal.Z += CDbl(Normal0.Z)
            normal.X += CDbl(Normal1.X)
            normal.Y += CDbl(Normal1.Y)
            normal.Z += CDbl(Normal1.Z)
            normal.X += CDbl(Normal2.X)
            normal.Y += CDbl(Normal2.Y)
            normal.Z += CDbl(Normal2.Z)
            normal.X += CDbl(Normal3.X)
            normal.Y += CDbl(Normal3.Y)
            normal.Z += CDbl(Normal3.Z)
            normal /= 4.0#
            Dim lighting As Single = CSng(RendererNormalLighting(normal))

            GL.Begin(BeginMode.Triangles)
            GL.Color3(lighting, lighting, lighting)
            If Terrain.Tiles(TileX, TileY).Tri Then
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)

                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            Else
                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
                GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)

                GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
                GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
                GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
                GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            End If
            GL.End()
        End Sub
    End Class

    Public Class clsDrawTileNewTriangles

        Public Normals(4) As sXYZ_sng
        Public TexCoords(4) As sXY_sng
        Public Vertices(4) As sXYZ_sng
        Public VertexColour(4) As sRGB_sng
        Public VertexAlpha(4) As Single

        Public Sub Buffer(BufferData As clsBufferData)

            'top
            BufferVertex(BufferData, 1)
            BufferVertex(BufferData, 0)
            BufferVertex(BufferData, 4)

            'left
            BufferVertex(BufferData, 0)
            BufferVertex(BufferData, 2)
            BufferVertex(BufferData, 4)

            'right
            BufferVertex(BufferData, 3)
            BufferVertex(BufferData, 1)
            BufferVertex(BufferData, 4)

            'bottom
            BufferVertex(BufferData, 2)
            BufferVertex(BufferData, 3)
            BufferVertex(BufferData, 4)
        End Sub

        Private Sub BufferVertex(BufferData As clsBufferData, VertexNum As Integer)

            BufferData.Vertices(BufferData.Position).Normal = Normals(VertexNum)
            BufferData.Vertices(BufferData.Position).TexCoord = TexCoords(VertexNum)
            BufferData.Vertices(BufferData.Position).Pos = Vertices(VertexNum)
            BufferData.Vertices(BufferData.Position).Pos.Z *= -1.0F
            BufferData.Vertices(BufferData.Position).RGBA.Red = VertexColour(VertexNum).Red
            BufferData.Vertices(BufferData.Position).RGBA.Green = VertexColour(VertexNum).Green
            BufferData.Vertices(BufferData.Position).RGBA.Blue = VertexColour(VertexNum).Blue
            BufferData.Vertices(BufferData.Position).RGBA.Alpha = VertexAlpha(VertexNum)
            BufferData.Position += 1
        End Sub
    End Class

    Public Class clsDrawTileNew
        Inherits clsDrawTile

        Public BlackBuffer As clsBufferData
        Public LayerBuffers() As clsBufferData

        Public Overrides Sub Perform()
            Dim bufferTriangles As New clsDrawTileNewTriangles
            Dim layerNums(4) As Integer
            Dim vertexPos(4) As sXY_int
            Dim vertexNum(3) As sXY_int
            Dim vertexLighting(4) As Single
            Dim vertexTerrain(4) As clsRendererTerrain

            vertexNum(0).X = TileX
            vertexNum(0).Y = TileY
            vertexNum(1).X = TileX + 1
            vertexNum(1).Y = TileY
            vertexNum(2).X = TileX
            vertexNum(2).Y = TileY + 1
            vertexNum(3).X = TileX + 1
            vertexNum(3).Y = TileY + 1

            For i As Integer = 0 To 3
                vertexPos(i) = vertexNum(i) * TerrainGridSpacing
            Next
            vertexPos(4).X = CInt((TileX + 0.5#) * TerrainGridSpacing)
            vertexPos(4).Y = CInt((TileY + 0.5#) * TerrainGridSpacing)

            For i As Integer = 0 To 4
                Dim Pos As sXY_int = vertexPos(i)
                bufferTriangles.Vertices(i).X = Pos.X
                bufferTriangles.Vertices(i).Y = CSng(Map.GetTerrainHeight(Pos))
                bufferTriangles.Vertices(i).Z = -Pos.Y
                Dim DifX As Matrix3D.XYZ_dbl
                Dim DifY As Matrix3D.XYZ_dbl
                DifX.X = 32.0#
                DifX.Y = Map.GetTerrainHeight(New sXY_int(Pos.X + 16, Pos.Y)) - Map.GetTerrainHeight(New sXY_int(Pos.X - 16, Pos.Y))
                DifX.Z = 0.0#
                DifY.X = 0.0#
                DifY.Y = Map.GetTerrainHeight(New sXY_int(Pos.X, Pos.Y + 16)) - Map.GetTerrainHeight(New sXY_int(Pos.X, Pos.Y - 16))
                DifY.Z = 32.0#
                Dim Cross As Matrix3D.XYZ_dbl
                Matrix3D.VectorCrossProduct(DifX, DifY, Cross)
                Cross /= Cross.GetMagnitude
                If Cross.Y < 0.0# Then
                    Cross *= -1.0#
                End If
                bufferTriangles.Normals(i).X = CSng(Cross.X)
                bufferTriangles.Normals(i).Y = CSng(Cross.Y)
                bufferTriangles.Normals(i).Z = CSng(Cross.Z)
                'todo adjust lighting
                vertexLighting(i) = CSng(RendererNormalLighting(Cross))
            Next

            bufferTriangles.VertexAlpha(4) = 0.25F

            For i As Integer = 0 To 3
                For j As Integer = 0 To 3
                    If j = i Then
                        bufferTriangles.VertexAlpha(j) = 1.0F
                    Else
                        bufferTriangles.VertexAlpha(j) = 0.0F
                    End If
                Next
                If Map.Tileset IsNot Nothing Then
                    'the odd terrain renderer method
                    Dim bestTerrain As clsRendererTerrain
                    'Dim ground(1, 1) As clsRendererTerrain
                    'For x As Integer = -1 To 0
                    '    Dim x2 As Integer = vertexNum(i).X + x
                    '    For y As Integer = -1 To 0
                    'Dim y2 As Integer = vertexNum(i).Y + y
                    'Dim texture As clsMap.clsTerrain.Tile.sTexture
                    'If x2 >= 0 And x2 < Map.Terrain.TileSize.X And y2 >= 0 And y2 < Map.Terrain.TileSize.Y Then
                    '    texture = Map.Terrain.Tiles(x2, y2).Texture
                    'Else
                    '    texture.TextureNum = 0
                    '    texture.Orientation = New sTileOrientation(False, False, False)
                    'End If
                    'Dim wzi As Integer = 1 + x
                    'Dim wzj As Integer = 1 + y
                    ''wzi = 1 - wzi
                    ''wzj = 1 - wzj
                    'Dim wzmap(1, 1) As Integer
                    'wzmap(0, 0) = 0
                    'wzmap(1, 0) = 1
                    'wzmap(1, 1) = 2
                    'wzmap(0, 1) = 3
                    'Dim wzinvmap(3, 1) As Integer
                    'wzinvmap(0, 0) = 0
                    'wzinvmap(1, 0) = 1
                    'wzinvmap(2, 0) = 1
                    'wzinvmap(3, 0) = 0
                    'wzinvmap(0, 1) = 0
                    'wzinvmap(1, 1) = 0
                    'wzinvmap(2, 1) = 1
                    'wzinvmap(3, 1) = 1
                    'Dim rot As Integer = wzmap(wzi, wzj)
                    'wzi = wzinvmap(rot, 0)
                    'wzj = wzinvmap(rot, 1)
                    'Dim textureNum As Integer = texture.TextureNum
                    'If textureNum >= 0 And textureNum < Map.Tileset.TileCount Then
                    '    Dim terrain As clsRendererTerrain = Map.Tileset.Tiles(textureNum).CornerTerrains(wzi, wzj)
                    '    If terrain IsNot Nothing Then
                    '        If terrain.IsCliff Then
                    '            bestTerrain = terrain
                    '            GoTo UseCliff
                    '        End If
                    '    End If
                    '    'ground(1 + x, 1 + y) = terrain
                    'End If
                    '    Next
                    'Next
                    'Dim votes(1, 1) As Integer
                    'For x As Integer = 0 To 1
                    '    For y As Integer = 0 To 1
                    '        For x2 As Integer = 0 To 1
                    '            For y2 As Integer = 0 To 1
                    '                If ground(x, y) Is ground(x2, y2) Then
                    '                    votes(x, y) += 1
                    '                End If
                    '            Next
                    '        Next
                    '    Next
                    'Next
                    'bestTerrain = ground(1, 1)
                    'Dim bestVotes As Integer = -1
                    'For x As Integer = 0 To 1
                    '    For y As Integer = 0 To 1
                    '        If votes(x, y) > bestVotes Then
                    '            bestVotes = votes(x, y)
                    '            bestTerrain = ground(x, y)
                    '        End If
                    '    Next
                    'Next
                    If vertexNum(i).X < Map.Terrain.TileSize.X And vertexNum(i).Y < Map.Terrain.TileSize.Y Then
                        Dim textureNum As Integer = Map.Terrain.Tiles(vertexNum(i).X, vertexNum(i).Y).Texture.TextureNum
                        If textureNum >= 0 Then
                            bestTerrain = Map.Tileset.Tiles(textureNum).CornerTerrains(0, 0)
                        ElseIf Map.Tileset.TileCount > 0 Then
                            bestTerrain = Map.Tileset.Tiles(0).CornerTerrains(0, 0)
                        Else
                            bestTerrain = Nothing
                        End If
                    Else
                        bestTerrain = Nothing
                    End If
UseCliff:
                    If bestTerrain IsNot Nothing Then
                        For j As Integer = 0 To 4
                            bufferTriangles.TexCoords(j).X = vertexPos(j).X / bestTerrain.Span
                            bufferTriangles.TexCoords(j).Y = vertexPos(j).Y / bestTerrain.Span
                        Next
                        If bestTerrain.IsWater Then
                            For j As Integer = 0 To 4
                                bufferTriangles.VertexColour(j).Red = vertexLighting(j) * 0.333333343F
                                bufferTriangles.VertexColour(j).Green = vertexLighting(j) * 0.333333343F
                                bufferTriangles.VertexColour(j).Blue = vertexLighting(j) * 0.333333343F
                            Next
                        Else
                            For j As Integer = 0 To 4
                                bufferTriangles.VertexColour(j).Red = vertexLighting(j)
                                bufferTriangles.VertexColour(j).Green = vertexLighting(j)
                                bufferTriangles.VertexColour(j).Blue = vertexLighting(j)
                            Next
                        End If
                        bufferTriangles.Buffer(LayerBuffers(bestTerrain.Number))
                        vertexTerrain(i) = bestTerrain
                    End If
                End If
            Next

            bufferTriangles.VertexAlpha(4) = 1.0F
            Dim waterCount As Integer = 0
            For i As Integer = 0 To 3
                bufferTriangles.VertexAlpha(i) = 1.0F
                bufferTriangles.VertexColour(i).Red = 0.0F
                bufferTriangles.VertexColour(i).Green = 0.0F
                bufferTriangles.VertexColour(i).Blue = 0.0F
                If vertexTerrain(i) IsNot Nothing Then
                    If vertexTerrain(i).IsWater Then
                        waterCount += 1
                        bufferTriangles.VertexColour(i).Red = 0.166666672F
                        bufferTriangles.VertexColour(i).Green = 0.25F
                        bufferTriangles.VertexColour(i).Blue = 0.333333343F
                    End If
                End If
            Next
            bufferTriangles.VertexColour(4).Red = 0.166666672F * waterCount * 0.25F
            bufferTriangles.VertexColour(4).Green = 0.25F * waterCount * 0.25F
            bufferTriangles.VertexColour(4).Blue = 0.333333343F * waterCount * 0.25F
            bufferTriangles.Buffer(BlackBuffer)
        End Sub
    End Class

    Public Class clsBufferData

        Public Structure sVertex
            Public Pos As sXYZ_sng
            Public Normal As sXYZ_sng
            Public TexCoord As sXY_sng
            Public RGBA As sRGBA_sng
            Private PaddingA As Integer
            Private PaddingB As Integer
            Private PaddingC As Integer
            Private PaddingD As Integer
        End Structure
        Public Vertices() As sVertex

        Public Position As Integer = 0

        Public Sub SendData(GLBufferNum As Integer)

            ReDim Preserve Vertices(Position - 1)

            GL.BindBuffer(BufferTarget.ArrayBuffer, GLBufferNum)
            GL.BufferData(Of sVertex)(BufferTarget.ArrayBuffer, New IntPtr(Position * 64), Vertices, BufferUsageHint.DynamicDraw)
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)

            Vertices = Nothing
        End Sub
    End Class
End Class
