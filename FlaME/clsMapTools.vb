Partial Public Class clsMap

    Public Sub Rotate_Clockwise(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(New sXY_int(Terrain.TileSize.Y, Terrain.TileSize.X))
        Dim tmpGateways() As sGateway
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Y, Terrain.TileSize.Y - X).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Y, Terrain.TileSize.Y - X).Terrain
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                X2 = Terrain.TileSize.Y - X - 1
                NewTerrain.Tiles(X, Y).Texture = Terrain.Tiles(Y, X2).Texture
                NewTerrain.Tiles(X, Y).Texture.Orientation.RotateClockwise()
                NewTerrain.Tiles(X, Y).DownSide = Terrain.Tiles(Y, X2).DownSide
                NewTerrain.Tiles(X, Y).DownSide.RotateClockwise()
                NewTerrain.Tiles(X, Y).Tri = Not Terrain.Tiles(Y, X2).Tri
                NewTerrain.Tiles(X, Y).TriTopLeftIsCliff = Terrain.Tiles(Y, X2).TriBottomLeftIsCliff
                NewTerrain.Tiles(X, Y).TriBottomLeftIsCliff = Terrain.Tiles(Y, X2).TriBottomRightIsCliff
                NewTerrain.Tiles(X, Y).TriBottomRightIsCliff = Terrain.Tiles(Y, X2).TriTopRightIsCliff
                NewTerrain.Tiles(X, Y).TriTopRightIsCliff = Terrain.Tiles(Y, X2).TriTopLeftIsCliff
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideV(Y, Terrain.TileSize.Y - X - 1).Road
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.SideV(X, Y).Road = Terrain.SideH(Y, Terrain.TileSize.Y - X).Road
            Next
        Next

        Dim A As Integer
        Dim intTemp As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.X = Terrain.TileSize.Y - Gateways(A).PosA.Y - 1
            tmpGateways(A).PosA.Y = Gateways(A).PosA.X
            tmpGateways(A).PosB.X = Terrain.TileSize.Y - Gateways(A).PosB.Y - 1
            tmpGateways(A).PosB.Y = Gateways(A).PosB.X
        Next

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            If ObjectRotateMode = enumObjectRotateMode.All Then
                Units(A).Rotation -= 90
                If Units(A).Rotation < 0 Then
                    Units(A).Rotation += 360
                End If
            ElseIf ObjectRotateMode = enumObjectRotateMode.Walls Then
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    If CType(Units(A).Type, clsStructureType).StructureType = clsStructureType.enumStructureType.Wall Then
                        Units(A).Rotation -= 90
                        If Units(A).Rotation < 0 Then
                            Units(A).Rotation += 360
                        End If
                        If Units(A).Rotation = 180 Then
                            Units(A).Rotation = 0
                        ElseIf Units(A).Rotation = 270 Then
                            Units(A).Rotation = 90
                        End If
                    End If
                End If
            End If
            intTemp = Units(A).Pos.Horizontal.X
            Units(A).Pos.Horizontal.X = Terrain.TileSize.Y * TerrainGridSpacing - Units(A).Pos.Horizontal.Y
            Units(A).Pos.Horizontal.Y = intTemp
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                Unit_Sectors_Calc(Units(A))
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        Terrain = NewTerrain
        Gateways = tmpGateways

        AfterInitialized()
    End Sub

    Public Sub Rotate_CounterClockwise(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(New sXY_int(Terrain.TileSize.Y, Terrain.TileSize.X))
        Dim tmpGateways() As sGateway
        Dim Y2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Terrain.TileSize.X - Y, X).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Terrain.TileSize.X - Y, X).Terrain
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            Y2 = Terrain.TileSize.X - Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.Tiles(X, Y).Texture = Terrain.Tiles(Y2, X).Texture
                NewTerrain.Tiles(X, Y).Texture.Orientation.RotateAnticlockwise()
                NewTerrain.Tiles(X, Y).DownSide = Terrain.Tiles(Y2, X).DownSide
                NewTerrain.Tiles(X, Y).DownSide.RotateAnticlockwise()
                NewTerrain.Tiles(X, Y).Tri = Not Terrain.Tiles(Y2, X).Tri
                NewTerrain.Tiles(X, Y).TriTopLeftIsCliff = Terrain.Tiles(Y2, X).TriTopRightIsCliff
                NewTerrain.Tiles(X, Y).TriBottomLeftIsCliff = Terrain.Tiles(Y2, X).TriTopLeftIsCliff
                NewTerrain.Tiles(X, Y).TriBottomRightIsCliff = Terrain.Tiles(Y2, X).TriBottomLeftIsCliff
                NewTerrain.Tiles(X, Y).TriTopRightIsCliff = Terrain.Tiles(Y2, X).TriBottomRightIsCliff
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideV(Terrain.TileSize.X - Y, X).Road
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.SideV(X, Y).Road = Terrain.SideH(Terrain.TileSize.X - Y - 1, X).Road
            Next
        Next

        Dim A As Integer
        Dim intTemp As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.Y = Terrain.TileSize.X - Gateways(A).PosA.X - 1
            tmpGateways(A).PosA.X = Gateways(A).PosA.Y
            tmpGateways(A).PosB.Y = Terrain.TileSize.X - Gateways(A).PosB.X - 1
            tmpGateways(A).PosB.X = Gateways(A).PosB.Y
        Next

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            If ObjectRotateMode = enumObjectRotateMode.All Then
                Units(A).Rotation += 90
                If Units(A).Rotation >= 360 Then
                    Units(A).Rotation -= 360
                End If
            ElseIf ObjectRotateMode = enumObjectRotateMode.Walls Then
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    If CType(Units(A).Type, clsStructureType).StructureType = clsStructureType.enumStructureType.Wall Then
                        Units(A).Rotation += 90
                        If Units(A).Rotation >= 360 Then
                            Units(A).Rotation -= 360
                        End If
                        If Units(A).Rotation = 180 Then
                            Units(A).Rotation = 0
                        ElseIf Units(A).Rotation = 270 Then
                            Units(A).Rotation = 90
                        End If
                    End If
                End If
            End If
            intTemp = Units(A).Pos.Horizontal.Y
            Units(A).Pos.Horizontal.Y = Terrain.TileSize.X * TerrainGridSpacing - Units(A).Pos.Horizontal.X
            Units(A).Pos.Horizontal.X = intTemp
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = CInt(Math.Ceiling(NewTerrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(NewTerrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                Unit_Sectors_Calc(Units(A))
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        Terrain = NewTerrain
        Gateways = tmpGateways

        AfterInitialized()
    End Sub

    Public Sub Rotate_FlipX(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(Terrain.TileSize)
        Dim tmpGateways() As sGateway
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Terrain.TileSize.X - X, Y).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Terrain.TileSize.X - X, Y).Terrain
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                X2 = Terrain.TileSize.X - X - 1
                NewTerrain.Tiles(X, Y).Texture = Terrain.Tiles(X2, Y).Texture
                NewTerrain.Tiles(X, Y).Texture.Orientation.ResultXFlip = Not NewTerrain.Tiles(X, Y).Texture.Orientation.ResultXFlip
                NewTerrain.Tiles(X, Y).DownSide = Terrain.Tiles(X2, Y).DownSide
                NewTerrain.Tiles(X2, Y).DownSide.FlipX()
                NewTerrain.Tiles(X, Y).Tri = Not Terrain.Tiles(X2, Y).Tri
                NewTerrain.Tiles(X, Y).TriTopLeftIsCliff = Terrain.Tiles(X2, Y).TriTopRightIsCliff
                NewTerrain.Tiles(X, Y).TriBottomLeftIsCliff = Terrain.Tiles(X2, Y).TriBottomRightIsCliff
                NewTerrain.Tiles(X, Y).TriBottomRightIsCliff = Terrain.Tiles(X2, Y).TriBottomLeftIsCliff
                NewTerrain.Tiles(X, Y).TriTopRightIsCliff = Terrain.Tiles(X2, Y).TriTopLeftIsCliff
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideH(Terrain.TileSize.X - X - 1, Y).Road
            Next
        Next
        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X
                NewTerrain.SideV(X, Y).Road = Terrain.SideV(Terrain.TileSize.X - X, Y).Road
            Next
        Next

        Dim A As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.X = NewTerrain.TileSize.X - Gateways(A).PosA.X - 1
            tmpGateways(A).PosA.Y = Gateways(A).PosA.Y
            tmpGateways(A).PosB.X = NewTerrain.TileSize.X - Gateways(A).PosB.X - 1
            tmpGateways(A).PosB.Y = Gateways(A).PosB.Y
        Next

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            If ObjectRotateMode = enumObjectRotateMode.All Then
                Units(A).Rotation -= 180
                If Units(A).Rotation < 0 Then
                    Units(A).Rotation += 360
                End If
            ElseIf ObjectRotateMode = enumObjectRotateMode.Walls Then
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    If CType(Units(A).Type, clsStructureType).StructureType = clsStructureType.enumStructureType.Wall Then
                        Units(A).Rotation -= 180
                        If Units(A).Rotation < 0 Then
                            Units(A).Rotation += 360
                        End If
                        If Units(A).Rotation = 180 Then
                            Units(A).Rotation = 0
                        ElseIf Units(A).Rotation = 270 Then
                            Units(A).Rotation = 90
                        End If
                    End If
                End If
            End If
            Units(A).Pos.Horizontal.X = Terrain.TileSize.X * TerrainGridSpacing - Units(A).Pos.Horizontal.X
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = CInt(Math.Ceiling(NewTerrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(NewTerrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                Unit_Sectors_Calc(Units(A))
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        Terrain = NewTerrain
        Gateways = tmpGateways

        AfterInitialized()
    End Sub

    Public Sub RandomizeHeights(ByVal LevelCount As Integer)
        Dim hmSource As New clsHeightmap
        Dim hmA As New clsHeightmap
        Dim hmB As New clsHeightmap
        Dim IntervalCount As Integer
        Dim AlterationLevels As clsHeightmap.sHeights
        Dim hmAlteration As sHeightmaps
        Dim LevelHeight As Single
        Dim HeightRange As Double
        Dim Level As Integer
        Dim IntervalHeight As Double
        Dim Variation As Double
        Dim X As Integer
        Dim Y As Integer

        IntervalCount = LevelCount - 1

        ReDim AlterationLevels.Heights(IntervalCount)
        Dim MinMax As New clsHeightmap.sMinMax
        ReDim hmAlteration.Heightmaps(IntervalCount)
        ReDim hmSource.HeightData.Height(Terrain.TileSize.Y, Terrain.TileSize.X)
        hmSource.HeightData.SizeX = Terrain.TileSize.X + 1
        hmSource.HeightData.SizeY = Terrain.TileSize.Y + 1
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                hmSource.HeightData.Height(Y, X) = CLng(Terrain.Vertices(X, Y).Height / hmSource.HeightScale)
            Next
        Next
        hmSource.MinMaxGet(MinMax)
        HeightRange = 255.0#
        IntervalHeight = HeightRange / IntervalCount
        Variation = IntervalHeight / 4.0#
        For Level = 0 To IntervalCount
            LevelHeight = CSng((MinMax.Min + Level * MinMax.Max / IntervalCount) * hmSource.HeightScale)
            AlterationLevels.Heights(Level) = LevelHeight
            hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, 2.0F, 10000.0#)
            hmAlteration.Heightmaps(Level) = New clsHeightmap
            hmAlteration.Heightmaps(Level).Rescale(hmB, LevelHeight - Variation, LevelHeight + Variation)
        Next
        hmA.FadeMultiple(hmSource, hmAlteration, AlterationLevels)
        hmB.Rescale(hmA, Math.Max(MinMax.Min * hmSource.HeightScale - Variation, 0.0#), Math.Min(MinMax.Max * hmSource.HeightScale + Variation, 255.9#))
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                Terrain.Vertices(X, Y).Height = CByte(Int(hmB.HeightData.Height(Y, X) * hmB.HeightScale))
            Next
        Next
    End Sub

    Public Sub LevelWater()
        Dim X As Integer
        Dim Y As Integer
        Dim TextureNum As Integer

        If Tileset Is Nothing Then
            Exit Sub
        End If

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum
                If TextureNum >= 0 And TextureNum < Tileset.TileCount Then
                    If Tileset.Tiles(TextureNum).Default_Type = TileTypeNum_Water Then
                        Terrain.Vertices(X, Y).Height = 0
                        Terrain.Vertices(X + 1, Y).Height = 0
                        Terrain.Vertices(X, Y + 1).Height = 0
                        Terrain.Vertices(X + 1, Y + 1).Height = 0
                    End If
                End If
            Next
        Next
    End Sub

    Public Structure sGenerateMasterTerrainArgs
        Public Tileset As clsGeneratorTileset
        Public LevelCount As Integer
        Public Class clsLayer
            Public WithinLayer As Integer
            Public AvoidLayers() As Boolean
            Public TileNum As Integer
            Public Terrainmap As clsBooleanMap
            Public TerrainmapScale As Single
            Public TerrainmapDensity As Single
            Public HeightMin As Single
            Public HeightMax As Single
            Public IsCliff As Boolean
        End Class
        Public Layers() As clsLayer
        Public LayerCount As Integer
        Public Watermap As clsBooleanMap
    End Structure

    Public Sub GenerateMasterTerrain(ByRef Args As sGenerateMasterTerrainArgs)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim TerrainType(,) As Integer
        Dim Slope(,) As Single

        Dim TerrainNum As Integer

        Dim bmA As New clsBooleanMap
        Dim Layer_Num As Integer
        Dim LayerResult(Args.LayerCount - 1) As clsBooleanMap
        Dim bmB As New clsBooleanMap
        Dim BestSlope As Double
        Dim CurrentSlope As Double
        Dim hmB As New clsHeightmap
        Dim hmC As New clsHeightmap

        Dim difA As Double
        Dim difB As Double
        Dim NewTri As Boolean
        Dim CliffSlope As Double = Math.Atan(255.0# * DefaultHeightMultiplier / (2.0# * (Args.LevelCount - 1.0#) * TerrainGridSpacing)) - RadOf1Deg 'divided by 2 due to the terrain height randomization

        Tileset = Args.Tileset.Tileset

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                difA = Math.Abs(CDbl(Terrain.Vertices(X + 1, Y + 1).Height) - Terrain.Vertices(X, Y).Height)
                difB = Math.Abs(CDbl(Terrain.Vertices(X, Y + 1).Height) - Terrain.Vertices(X + 1, Y).Height)
                If difA = difB Then
                    If Rnd() >= 0.5F Then
                        NewTri = False
                    Else
                        NewTri = True
                    End If
                ElseIf difA < difB Then
                    NewTri = False
                Else
                    NewTri = True
                End If
                If Not Terrain.Tiles(X, Y).Tri = NewTri Then
                    Terrain.Tiles(X, Y).Tri = NewTri
                End If
            Next
        Next

        For A = 0 To Args.LayerCount - 1
            Args.Layers(A).Terrainmap = New clsBooleanMap
            If Args.Layers(A).TerrainmapDensity = 1.0F Then
                ReDim Args.Layers(A).Terrainmap.ValueData.Value(Terrain.TileSize.Y - 1, Terrain.TileSize.X - 1)
                Args.Layers(A).Terrainmap.ValueData.Size = Terrain.TileSize
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        Args.Layers(A).Terrainmap.ValueData.Value(Y, X) = True
                    Next
                Next
            Else
                hmB.GenerateNewOfSize(Terrain.TileSize.Y, Terrain.TileSize.X, Args.Layers(A).TerrainmapScale, 1.0#)
                hmC.Rescale(hmB, 0.0#, 1.0#)
                Args.Layers(A).Terrainmap.Convert_Heightmap(hmC, CLng((1.0F - Args.Layers(A).TerrainmapDensity) / hmC.HeightScale))
            End If
        Next

        Dim Pos As sXY_int

        ReDim TerrainType(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        ReDim Slope(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = CSng(BestSlope)
            Next
        Next
        For Layer_Num = 0 To Args.LayerCount - 1
            TerrainNum = Args.Layers(Layer_Num).TileNum
            If TerrainNum >= 0 Then
                'do other layer constraints
                LayerResult(Layer_Num) = New clsBooleanMap
                LayerResult(Layer_Num).Copy(Args.Layers(Layer_Num).Terrainmap)
                If Args.Layers(Layer_Num).WithinLayer >= 0 Then
                    If Args.Layers(Layer_Num).WithinLayer < Layer_Num Then
                        bmA.Within(LayerResult(Layer_Num), LayerResult(Args.Layers(Layer_Num).WithinLayer))
                        LayerResult(Layer_Num).ValueData = bmA.ValueData
                        bmA.ValueData = New clsBooleanMap.clsValueData
                    End If
                End If
                For A = 0 To Layer_Num - 1
                    If Args.Layers(Layer_Num).AvoidLayers(A) Then
                        bmA.Expand_One_Tile(LayerResult(A))
                        bmB.Remove(LayerResult(Layer_Num), bmA)
                        LayerResult(Layer_Num).ValueData = bmB.ValueData
                        bmB.ValueData = New clsBooleanMap.clsValueData
                    End If
                Next
                'do height and slope constraints
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            If Terrain.Vertices(X, Y).Height < Args.Layers(Layer_Num).HeightMin _
                            Or Terrain.Vertices(X, Y).Height > Args.Layers(Layer_Num).HeightMax Then
                                LayerResult(Layer_Num).ValueData.Value(Y, X) = False
                            End If
                            If Args.Layers(Layer_Num).IsCliff Then
                                If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                                    If Slope(X, Y) < CliffSlope Then
                                        LayerResult(Layer_Num).ValueData.Value(Y, X) = False
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next

                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            TerrainType(X, Y) = TerrainNum
                        End If
                    Next
                Next
            End If
        Next

        'set water tiles

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Args.Watermap.ValueData.Value(Y, X) Then
                    If Slope(X, Y) < CliffSlope Then
                        TerrainType(X, Y) = 17
                    End If
                End If
            Next
        Next

        'set border tiles to cliffs
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To 2
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For X = Terrain.TileSize.X - 4 To Terrain.TileSize.X - 1
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next
        For X = 3 To Terrain.TileSize.X - 5
            For Y = 0 To 2
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For Y = Terrain.TileSize.Y - 4 To Terrain.TileSize.Y - 1
                TerrainType(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.TextureNum = TerrainType(X, Y)
            Next
        Next
    End Sub

    Public Sub RandomizeTileOrientations()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.Orientation = New sTileOrientation(Rnd() >= 0.5F, Rnd() >= 0.5F, Rnd() >= 0.5F)
            Next
        Next
        SectorGraphicsChanges.SetAllChanged()
    End Sub

    Public Sub MapTexturer(ByRef LayerList As frmMapTexturer.sLayerList)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim TerrainType(,) As clsPainter.clsTerrain
        Dim Slope(,) As Single
        Dim tmpTerrain As clsPainter.clsTerrain
        Dim bmA As New clsBooleanMap
        Dim bmB As New clsBooleanMap
        Dim LayerNum As Integer
        Dim LayerResult(LayerList.LayerCount - 1) As clsBooleanMap
        Dim BestSlope As Double
        Dim CurrentSlope As Double
        Dim AllowSlope As Boolean
        Dim Pos As sXY_int

        ReDim TerrainType(Terrain.TileSize.X, Terrain.TileSize.Y)
        ReDim Slope(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.25#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.25#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = CInt((X + 0.75#) * TerrainGridSpacing)
                Pos.Y = CInt((Y + 0.75#) * TerrainGridSpacing)
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = CSng(BestSlope)
            Next
        Next
        For LayerNum = 0 To LayerList.LayerCount - 1
            tmpTerrain = LayerList.Layers(LayerNum).Terrain
            If tmpTerrain IsNot Nothing Then
                'do other layer constraints
                LayerResult(LayerNum) = New clsBooleanMap
                LayerResult(LayerNum).Copy(LayerList.Layers(LayerNum).Terrainmap)
                If LayerList.Layers(LayerNum).WithinLayer >= 0 Then
                    If LayerList.Layers(LayerNum).WithinLayer < LayerNum Then
                        bmA.Within(LayerResult(LayerNum), LayerResult(LayerList.Layers(LayerNum).WithinLayer))
                        LayerResult(LayerNum).ValueData = bmA.ValueData
                        bmA.ValueData = New clsBooleanMap.clsValueData
                    End If
                End If
                For A = 0 To LayerNum - 1
                    If LayerList.Layers(LayerNum).AvoidLayers(A) Then
                        bmA.Expand_One_Tile(LayerResult(A))
                        bmB.Remove(LayerResult(LayerNum), bmA)
                        LayerResult(LayerNum).ValueData = bmB.ValueData
                        bmB.ValueData = New clsBooleanMap.clsValueData
                    End If
                Next
                'do height and slope constraints
                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            If Terrain.Vertices(X, Y).Height < LayerList.Layers(LayerNum).HeightMin _
                            Or Terrain.Vertices(X, Y).Height > LayerList.Layers(LayerNum).HeightMax Then
                                LayerResult(LayerNum).ValueData.Value(Y, X) = False
                            End If
                            If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                                AllowSlope = True
                                If X > 0 Then
                                    If Y > 0 Then
                                        If Slope(X - 1, Y - 1) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X - 1, Y - 1) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                    If Y < Terrain.TileSize.Y Then
                                        If Slope(X - 1, Y) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X - 1, Y) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                End If
                                If X < Terrain.TileSize.X Then
                                    If Y > 0 Then
                                        If Slope(X, Y - 1) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X, Y - 1) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                    If Y < Terrain.TileSize.Y Then
                                        If Slope(X, Y) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X, Y) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                End If
                                If Not AllowSlope Then
                                    LayerResult(LayerNum).ValueData.Value(Y, X) = False
                                End If
                            End If
                        End If
                    Next
                Next

                LayerResult(LayerNum).Remove_Diagonals()

                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            TerrainType(X, Y) = tmpTerrain
                        End If
                    Next
                Next
            End If
        Next

        'set vertex terrain by terrain map
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                If TerrainType(X, Y) IsNot Nothing Then
                    Terrain.Vertices(X, Y).Terrain = TerrainType(X, Y)
                End If
            Next
        Next
        AutoTextureChanges.SetAllChanged()
        UpdateAutoTextures()
    End Sub

    Public Function GenerateTerrainMap(ByVal Scale As Single, ByVal Density As Single) As clsBooleanMap
        Dim ReturnResult As clsBooleanMap
        Dim hmB As New clsHeightmap
        Dim hmC As New clsHeightmap

        hmB.GenerateNewOfSize(Terrain.TileSize.Y + 1, Terrain.TileSize.X + 1, Scale, 1.0#)
        hmC.Rescale(hmB, 0.0#, 1.0#)
        ReturnResult = New clsBooleanMap
        ReturnResult.Convert_Heightmap(hmC, CLng((1.0# - Density) / hmC.HeightScale))
        Return ReturnResult
    End Function

    Public Sub Terrain_Interpret(ByVal Start As sXY_int, ByVal Finish As sXY_int)

        If Tileset Is Nothing Then
            Exit Sub
        End If

        Dim Area As sXY_int

        Area.X = Finish.X - Start.X + 1
        Area.Y = Finish.Y - Start.Y + 1

        Dim VertexTerrainCount(Area.X + 2, Area.Y + 2, Painter.TerrainCount - 1) As Integer
        Dim SideH_Road_Count(Area.X + 2 - 1, Area.Y + 2, Painter.RoadCount - 1) As Integer
        Dim SideV_Road_Count(Area.X + 2, Area.Y + 2 - 1, Painter.RoadCount - 1) As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim B As Integer
        Dim Orientation As sTileDirection
        Dim Offset As sXY_int

        'count the terrains of influencing tiles
        For Y = Math.Max(Start.Y - 1, 0) To Math.Min(Finish.Y + 1, Terrain.TileSize.Y - 1)
            Offset.Y = Y - Start.Y + 1
            For X = Math.Max(Start.X - 1, 0) To Math.Min(Finish.X + 1, Terrain.TileSize.X - 1)
                Offset.X = X - Start.X + 1
                If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 Then
                    For A = 0 To Painter.TerrainCount - 1
                        With Painter.Terrains(A)
                            For B = 0 To .Tiles.TileCount - 1
                                If .Tiles.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Num) += 1
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.TransitionBrushCount - 1
                        With Painter.TransitionBrushes(A)
                            For B = 0 To .Tiles_Straight.TileCount - 1
                                If .Tiles_Straight.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Straight.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_In.TileCount - 1
                                If .Tiles_Corner_In.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Corner_In.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_Out.TileCount - 1
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Corner_Out.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    End If
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.CliffBrushCount - 1
                        With Painter.CliffBrushes(A)
                            For B = 0 To .Tiles_Straight.TileCount - 1
                                If .Tiles_Straight.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Straight.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                    End If
                                    If Terrain.Tiles(X, Y).Tri Then
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = True
                                    Else
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = True
                                    End If
                                    Terrain.Tiles(X, Y).Terrain_IsCliff = True
                                    Terrain.Tiles(X, Y).DownSide = Orientation
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_In.TileCount - 1
                                If .Tiles_Corner_In.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Corner_In.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = True
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = True
                                    End If
                                    Terrain.Tiles(X, Y).Terrain_IsCliff = True
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_Out.TileCount - 1
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    RotateDirection(.Tiles_Corner_Out.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = True
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = True
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        Terrain.Tiles(X, Y).TriTopLeftIsCliff = False
                                        Terrain.Tiles(X, Y).TriBottomRightIsCliff = False
                                        Terrain.Tiles(X, Y).TriTopRightIsCliff = True
                                        Terrain.Tiles(X, Y).TriBottomLeftIsCliff = False
                                    End If
                                    Terrain.Tiles(X, Y).Terrain_IsCliff = True
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.RoadBrushCount - 1
                        With Painter.RoadBrushes(A)
                            For B = 0 To .Tile_Corner_In.TileCount - 1
                                If .Tile_Corner_In.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    RotateDirection(.Tile_Corner_In.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_CrossIntersection.TileCount - 1
                                If .Tile_CrossIntersection.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                End If
                            Next
                            For B = 0 To .Tile_End.TileCount - 1
                                If .Tile_End.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    RotateDirection(.Tile_End.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_Straight.TileCount - 1
                                If .Tile_Straight.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    RotateDirection(.Tile_Straight.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_TIntersection.TileCount - 1
                                If .Tile_TIntersection.Tiles(B).TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    RotateDirection(.Tile_TIntersection.Tiles(B).Direction, Terrain.Tiles(X, Y).Texture.Orientation, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideV_Road_Count(Offset.X + 1, Offset.Y, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y, .Road.Num) += 1
                                        SideH_Road_Count(Offset.X, Offset.Y + 1, .Road.Num) += 1
                                    End If
                                End If
                            Next
                        End With
                    Next
                End If
            Next
        Next

        'designate terrains
        Dim Best_Num As Integer
        Dim Best_Count As Integer
        For Y = Start.Y To Finish.Y + 1
            Offset.Y = Y - Start.Y + 1
            For X = Start.X To Finish.X + 1
                Offset.X = X - Start.X + 1
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.TerrainCount - 1
                    If VertexTerrainCount(Offset.X, Offset.Y, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = VertexTerrainCount(Offset.X, Offset.Y, A)
                    End If
                Next
                If Best_Count > 0 Then
                    Terrain.Vertices(X, Y).Terrain = Painter.Terrains(Best_Num)
                End If
            Next
        Next
        'designate h roads
        For Y = Start.Y To Finish.Y + 1
            Offset.Y = Y - Start.Y + 1
            For X = Start.X To Finish.X
                Offset.X = X - Start.X + 1
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.RoadCount - 1
                    If SideH_Road_Count(Offset.X, Offset.Y, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = SideH_Road_Count(Offset.X, Offset.Y, A)
                    End If
                Next
                If Best_Count > 0 Then
                    Terrain.SideH(X, Y).Road = Painter.Roads(Best_Num)
                End If
            Next
        Next
        'designate v roads
        For Y = Start.Y To Finish.Y
            Offset.Y = Y - Start.Y + 1
            For X = Start.X To Finish.X + 1
                Offset.X = X - Start.X + 1
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.RoadCount - 1
                    If SideV_Road_Count(Offset.X, Offset.Y, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = SideV_Road_Count(Offset.X, Offset.Y, A)
                    End If
                Next
                If Best_Count > 0 Then
                    Terrain.SideV(X, Y).Road = Painter.Roads(Best_Num)
                End If
            Next
        Next
    End Sub

    Public Sub WaterTriCorrection()

        If Tileset Is Nothing Then
            Exit Sub
        End If

        Dim X As Integer
        Dim Y As Integer
        Dim TileNum As sXY_int

        For Y = 0 To Terrain.TileSize.Y - 1
            TileNum.Y = Y
            For X = 0 To Terrain.TileSize.X - 1
                TileNum.X = X
                If Terrain.Tiles(X, Y).Tri Then
                    If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 Then
                        If Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            Terrain.Tiles(X, Y).Tri = False
                            SectorGraphicsChanges.TileChanged(TileNum)
                            SectorTerrainUndoChanges.TileChanged(TileNum)
                        End If
                    End If
                End If
            Next
        Next
    End Sub

    Public MustInherit Class clsAction

        Public Map As clsMap
        Public PosNum As sXY_int
        Public UseEffect As Boolean
        Public Effect As Double

        Public MustOverride Sub ActionPerform()

        Public Class clsApplyCliff
            Inherits clsMap.clsAction

            Public Angle As Double
            Public SetTris As Boolean

            Private A As Integer
            Private difA As Double
            Private difB As Double
            Private HeightA As Double
            Private HeightB As Double
            Private TriTopLeftMaxSlope As Double
            Private TriTopRightMaxSlope As Double
            Private TriBottomLeftMaxSlope As Double
            Private TriBottomRightMaxSlope As Double
            Private CliffChanged As Boolean
            Private TriChanged As Boolean
            Private NewVal As Boolean
            Private Terrain As clsTerrain
            Private tmpPos As sXY_int

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) / 2.0#
                HeightB = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
                difA = HeightB - HeightA
                HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) / 2.0#
                HeightB = (CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
                difB = HeightB - HeightA
                If Math.Abs(difA) = Math.Abs(difB) Then
                    A = CInt(Int(Rnd() * 4.0F))
                    If A = 0 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                    ElseIf A = 1 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                    ElseIf A = 2 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                    End If
                ElseIf Math.Abs(difA) > Math.Abs(difB) Then
                    If difA < 0.0# Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                    End If
                Else
                    If difB < 0.0# Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                    End If
                End If

                CliffChanged = False
                TriChanged = False

                If SetTris Then
                    difA = Math.Abs(CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) - Terrain.Vertices(PosNum.X, PosNum.Y).Height)
                    difB = Math.Abs(CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) - Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height)
                    If difA = difB Then
                        If Rnd() >= 0.5F Then
                            NewVal = False
                        Else
                            NewVal = True
                        End If
                    ElseIf difA < difB Then
                        NewVal = False
                    Else
                        NewVal = True
                    End If
                    If Terrain.Tiles(PosNum.X, PosNum.Y).Tri <> NewVal Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).Tri = NewVal
                        TriChanged = True
                    End If
                End If

                If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                    tmpPos.X = CInt((PosNum.X + 0.25#) * TerrainGridSpacing)
                    tmpPos.Y = CInt((PosNum.Y + 0.25#) * TerrainGridSpacing)
                    TriTopLeftMaxSlope = Map.GetTerrainSlopeAngle(tmpPos)
                    tmpPos.X = CInt((PosNum.X + 0.75#) * TerrainGridSpacing)
                    tmpPos.Y = CInt((PosNum.Y + 0.75#) * TerrainGridSpacing)
                    TriBottomRightMaxSlope = Map.GetTerrainSlopeAngle(tmpPos)

                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                        CliffChanged = True
                    End If
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                        CliffChanged = True
                    End If

                    NewVal = (TriTopLeftMaxSlope >= Angle)
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff <> NewVal Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = NewVal
                        CliffChanged = True
                    End If

                    NewVal = (TriBottomRightMaxSlope >= Angle)
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff <> NewVal Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = NewVal
                        CliffChanged = True
                    End If

                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                    End If
                Else
                    tmpPos.X = CInt((PosNum.X + 0.75#) * TerrainGridSpacing)
                    tmpPos.Y = CInt((PosNum.Y + 0.25#) * TerrainGridSpacing)
                    TriTopRightMaxSlope = Map.GetTerrainSlopeAngle(tmpPos)
                    tmpPos.X = CInt((PosNum.X + 0.25#) * TerrainGridSpacing)
                    tmpPos.Y = CInt((PosNum.Y + 0.75#) * TerrainGridSpacing)
                    TriBottomLeftMaxSlope = Map.GetTerrainSlopeAngle(tmpPos)

                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                        CliffChanged = True
                    End If
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                        CliffChanged = True
                    End If

                    NewVal = (TriTopRightMaxSlope >= Angle)
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff <> NewVal Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = NewVal
                        CliffChanged = True
                    End If

                    NewVal = (TriBottomLeftMaxSlope >= Angle)
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff <> NewVal Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = NewVal
                        CliffChanged = True
                    End If

                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                    End If
                End If

                If CliffChanged Then
                    Map.AutoTextureChanges.TileChanged(PosNum)
                End If
                If TriChanged Or CliffChanged Then
                    Map.SectorGraphicsChanges.TileChanged(PosNum)
                    Map.SectorTerrainUndoChanges.TileChanged(PosNum)
                End If
            End Sub
        End Class

        Public Class clsApplyCliffRemove
            Inherits clsMap.clsAction

            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                If Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Or Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False

                    Map.AutoTextureChanges.TileChanged(PosNum)
                    Map.SectorGraphicsChanges.TileChanged(PosNum)
                    Map.SectorTerrainUndoChanges.TileChanged(PosNum)
                End If
            End Sub
        End Class

        Public Class clsApplyCliffTriangle
            Inherits clsMap.clsAction

            Public Triangle As Boolean

            Private Terrain As clsTerrain
            Private CliffChanged As Boolean

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                CliffChanged = False
                If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                    If Triangle Then
                        If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                            CliffChanged = True
                        End If
                    Else
                        If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                            CliffChanged = True
                        End If
                    End If
                Else
                    If Triangle Then
                        If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                            CliffChanged = True
                        End If
                    Else
                        If Not Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                            CliffChanged = True
                        End If
                    End If
                End If

                If Not CliffChanged Then
                    Exit Sub
                End If

                Dim HeightA As Double
                Dim HeightB As Double
                Dim difA As Double
                Dim difB As Double
                Dim A As Integer

                Map.AutoTextureChanges.TileChanged(PosNum)
                Map.SectorGraphicsChanges.TileChanged(PosNum)
                Map.SectorTerrainUndoChanges.TileChanged(PosNum)

                HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) / 2.0#
                HeightB = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
                difA = HeightB - HeightA
                HeightA = (CDbl(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height) / 2.0#
                HeightB = (CDbl(Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height) + Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height) / 2.0#
                difB = HeightB - HeightA
                If Math.Abs(difA) = Math.Abs(difB) Then
                    A = CInt(Int(Rnd() * 4.0F))
                    If A = 0 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                    ElseIf A = 1 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                    ElseIf A = 2 Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                    End If
                ElseIf Math.Abs(difA) > Math.Abs(difB) Then
                    If difA < 0.0# Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Bottom
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Top
                    End If
                Else
                    If difB < 0.0# Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Right
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_Left
                    End If
                End If
            End Sub
        End Class

        Public Class clsApplyCliffTriangleRemove
            Inherits clsMap.clsAction

            Public Triangle As Boolean

            Private Terrain As clsTerrain
            Private CliffChanged As Boolean

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                CliffChanged = False
                If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                    If Triangle Then
                        If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                            CliffChanged = True
                        End If
                    Else
                        If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                            CliffChanged = True
                        End If
                    End If
                Else
                    If Triangle Then
                        If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                            CliffChanged = True
                        End If
                    Else
                        If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                            Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                            CliffChanged = True
                        End If
                    End If
                End If

                If Not CliffChanged Then
                    Exit Sub
                End If

                Map.AutoTextureChanges.TileChanged(PosNum)
                Map.SectorGraphicsChanges.TileChanged(PosNum)
                Map.SectorTerrainUndoChanges.TileChanged(PosNum)
            End Sub
        End Class

        Public Class clsApplyHeightChange
            Inherits clsMap.clsAction

            Public Rate As Double

            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                Terrain.Vertices(PosNum.X, PosNum.Y).Height = CByte(Clamp_int(CInt(Terrain.Vertices(PosNum.X, PosNum.Y).Height) + CInt(Rate * Effect), Byte.MinValue, Byte.MaxValue))

                Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum)
                Map.SectorTerrainUndoChanges.VertexAndNormalsChanged(PosNum)
            End Sub
        End Class

        Public Class clsApplyHeightSet
            Inherits clsMap.clsAction

            Public Height As Byte

            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                If Terrain.Vertices(PosNum.X, PosNum.Y).Height <> Height Then
                    Terrain.Vertices(PosNum.X, PosNum.Y).Height = Height
                    Map.SectorGraphicsChanges.VertexAndNormalsChanged(PosNum)
                    Map.SectorUnitHeightsChanges.VertexAndNormalsChanged(PosNum)
                    Map.SectorTerrainUndoChanges.VertexAndNormalsChanged(PosNum)
                End If
            End Sub
        End Class

        Public Class clsApplyHeightSmoothing
            Inherits clsMap.clsAction

            Public Ratio As Double
            Public Offset As sXY_int
            Public AreaTileSize As sXY_int

            Private NewHeight(,) As Byte
            Private Started As Boolean
            Private TempHeight As Integer
            Private Samples As Integer
            Private LimitX As Integer
            Private LimitY As Integer
            Private XNum As Integer
            Private VertexNum As sXY_int
            Private Terrain As clsTerrain

            Public Sub Start()
                Dim X As Integer
                Dim Y As Integer

                Terrain = Map.Terrain

                ReDim NewHeight(AreaTileSize.X, AreaTileSize.Y)
                For Y = 0 To AreaTileSize.Y
                    For X = 0 To AreaTileSize.X
                        NewHeight(X, Y) = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                    Next
                Next

                Started = True
            End Sub

            Public Sub Finish()

                If Not Started Then
                    Stop
                    Exit Sub
                End If

                Dim X As Integer
                Dim Y As Integer

                Terrain = Map.Terrain

                For Y = 0 To AreaTileSize.Y
                    VertexNum.Y = Offset.Y + Y
                    For X = 0 To AreaTileSize.X
                        VertexNum.X = Offset.X + X
                        Terrain.Vertices(VertexNum.X, VertexNum.Y).Height = NewHeight(X, Y)

                        Map.SectorGraphicsChanges.VertexAndNormalsChanged(VertexNum)
                        Map.SectorTerrainUndoChanges.VertexAndNormalsChanged(VertexNum)
                    Next
                Next

                Started = False
            End Sub

            Public Overrides Sub ActionPerform()

                If Not Started Then
                    Stop
                    Exit Sub
                End If

                Dim X As Integer
                Dim Y As Integer
                Dim X2 As Integer
                Dim Y2 As Integer

                Terrain = Map.Terrain

                LimitX = Terrain.TileSize.X
                LimitY = Terrain.TileSize.Y
                TempHeight = 0
                Samples = 0
                For Y = Clamp_int(SmoothRadius.Tiles.YMin + PosNum.Y, 0, LimitY) - PosNum.Y To Clamp_int(SmoothRadius.Tiles.YMax + PosNum.Y, 0, LimitY) - PosNum.Y
                    Y2 = PosNum.Y + Y
                    XNum = Y - SmoothRadius.Tiles.YMin
                    For X = Clamp_int(SmoothRadius.Tiles.XMin(XNum) + PosNum.X, 0, LimitX) - PosNum.X To Clamp_int(SmoothRadius.Tiles.XMax(XNum) + PosNum.X, 0, LimitX) - PosNum.X
                        X2 = PosNum.X + X
                        TempHeight += Terrain.Vertices(X2, Y2).Height
                        Samples += 1
                    Next
                Next
                NewHeight(PosNum.X - Offset.X, PosNum.Y - Offset.Y) = CByte(Math.Min(CInt(Terrain.Vertices(PosNum.X, PosNum.Y).Height * (1.0# - Ratio) + TempHeight / Samples * Ratio), Byte.MaxValue))
            End Sub
        End Class

        Public Class clsApplyRoadRemove
            Inherits clsMap.clsAction

            Private Terrain As clsTerrain

            Private Sub ToolPerformSideH(ByVal SideNum As sXY_int)

                Terrain = Map.Terrain

                If Terrain.SideH(SideNum.X, SideNum.Y).Road IsNot Nothing Then
                    Terrain.SideH(SideNum.X, SideNum.Y).Road = Nothing
                    Map.AutoTextureChanges.SideHChanged(SideNum)
                    Map.SectorGraphicsChanges.SideHChanged(SideNum)
                    Map.SectorTerrainUndoChanges.SideHChanged(SideNum)
                End If
            End Sub

            Private Sub ToolPerformSideV(ByVal SideNum As sXY_int)

                Terrain = Map.Terrain

                If Terrain.SideV(SideNum.X, SideNum.Y).Road IsNot Nothing Then
                    Terrain.SideV(SideNum.X, SideNum.Y).Road = Nothing
                    Map.AutoTextureChanges.SideVChanged(SideNum)
                    Map.SectorGraphicsChanges.SideVChanged(SideNum)
                    Map.SectorTerrainUndoChanges.SideVChanged(SideNum)
                End If
            End Sub

            Public Overrides Sub ActionPerform()

                ToolPerformSideH(PosNum)
                ToolPerformSideH(New sXY_int(PosNum.X, PosNum.Y + 1))
                ToolPerformSideV(PosNum)
                ToolPerformSideV(New sXY_int(PosNum.X + 1, PosNum.Y))
            End Sub
        End Class

        Public Class clsApplyVertexTerrain
            Inherits clsMap.clsAction

            Public VertexTerrain As clsPainter.clsTerrain

            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain IsNot VertexTerrain Then
                    Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = VertexTerrain
                    Map.SectorGraphicsChanges.VertexChanged(PosNum)
                    Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
                    Map.AutoTextureChanges.VertexChanged(PosNum)
                End If
            End Sub
        End Class

        Public Class clsApplyTexture
            Inherits clsMap.clsAction

            Public TextureNum As Integer
            Public SetTexture As Boolean
            Public Orientation As sTileOrientation
            Public SetOrientation As Boolean
            Public RandomOrientation As Boolean
            Public TerrainAction As enumTextureTerrainAction

            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False

                If SetTexture Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture.TextureNum = TextureNum
                End If
                If SetOrientation Then
                    If RandomOrientation Then
                        Terrain.Tiles(PosNum.X, PosNum.Y).Texture.Orientation = New sTileOrientation(Rnd() < 0.5F, Rnd() < 0.5F, Rnd() < 0.5F)
                    Else
                        Terrain.Tiles(PosNum.X, PosNum.Y).Texture.Orientation = Orientation
                    End If
                End If

                Map.TileTextureChangeTerrainAction(PosNum, TerrainAction)

                Map.SectorGraphicsChanges.TileChanged(PosNum)
                Map.SectorTerrainUndoChanges.TileChanged(PosNum)
            End Sub
        End Class

        Public Class clsApplyVertexTerrainInterpret
            Inherits clsMap.clsAction

            Private TerrainCount() As Integer
            Private VertexDirection As sTileDirection
            Private Painter As clsPainter
            Private PainterTerrainA As clsPainter.clsTerrain
            Private PainterTerrainB As clsPainter.clsTerrain
            Private Texture As clsTerrain.Tile.sTexture
            Private ResultDirection As sTileDirection
            Private PainterTexture As clsPainter.clsTileList.sTile_Orientation_Chance
            Private OppositeDirection As sTileDirection
            Private BestNum As Integer
            Private BestCount As Integer
            Private Tile As clsTerrain.Tile
            Private Terrain As clsTerrain

            Private Sub ToolPerformTile()
                Dim PainterBrushNum As Integer
                Dim A As Integer

                For PainterBrushNum = 0 To Painter.TerrainCount - 1
                    PainterTerrainA = Painter.Terrains(PainterBrushNum)
                    For A = 0 To PainterTerrainA.Tiles.TileCount - 1
                        PainterTexture = PainterTerrainA.Tiles.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                Next
                For PainterBrushNum = 0 To Painter.TransitionBrushCount - 1
                    PainterTerrainA = Painter.TransitionBrushes(PainterBrushNum).Terrain_Inner
                    PainterTerrainB = Painter.TransitionBrushes(PainterBrushNum).Terrain_Outer
                    For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                        PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If DirectionsOnSameSide(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainB.Num) += 1
                            Else
                                TerrainCount(PainterTerrainA.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                        PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If IdenticalTileOrientations(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainB.Num) += 1
                            Else
                                TerrainCount(PainterTerrainA.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                        PainterTexture = Painter.TransitionBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            OppositeDirection = PainterTexture.Direction
                            OppositeDirection.FlipX()
                            OppositeDirection.FlipY()
                            RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                            If IdenticalTileOrientations(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainA.Num) += 1
                            Else
                                TerrainCount(PainterTerrainB.Num) += 1
                            End If
                        End If
                    Next
                Next

                For PainterBrushNum = 0 To Painter.CliffBrushCount - 1
                    PainterTerrainA = Painter.CliffBrushes(PainterBrushNum).Terrain_Inner
                    PainterTerrainB = Painter.CliffBrushes(PainterBrushNum).Terrain_Outer
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If DirectionsOnSameSide(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainB.Num) += 1
                            Else
                                TerrainCount(PainterTerrainA.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If IdenticalTileOrientations(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainA.Num) += 1
                            Else
                                TerrainCount(PainterTerrainB.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            OppositeDirection = PainterTexture.Direction
                            OppositeDirection.FlipX()
                            OppositeDirection.FlipY()
                            RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                            If IdenticalTileOrientations(VertexDirection, ResultDirection) Then
                                TerrainCount(PainterTerrainA.Num) += 1
                            Else
                                TerrainCount(PainterTerrainB.Num) += 1
                            End If
                        End If
                    Next
                Next

                For PainterBrushNum = 0 To Painter.RoadBrushCount - 1
                    PainterTerrainA = Painter.RoadBrushes(PainterBrushNum).Terrain
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_End.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_End.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Straight.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Straight.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            TerrainCount(PainterTerrainA.Num) += 1
                        End If
                    Next
                Next
            End Sub

            Public Overrides Sub ActionPerform()
                Dim A As Integer

                Terrain = Map.Terrain

                Painter = Map.Painter
                ReDim TerrainCount(Painter.TerrainCount - 1)

                If PosNum.Y > 0 Then
                    If PosNum.X > 0 Then
                        VertexDirection = TileDirection_BottomRight
                        Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y - 1)
                        Texture = Tile.Texture
                        ToolPerformTile()
                    End If
                    If PosNum.X < Terrain.TileSize.X Then
                        VertexDirection = TileDirection_BottomLeft
                        Tile = Terrain.Tiles(PosNum.X, PosNum.Y - 1)
                        Texture = Tile.Texture
                        ToolPerformTile()
                    End If
                End If
                If PosNum.Y < Terrain.TileSize.Y Then
                    If PosNum.X > 0 Then
                        VertexDirection = TileDirection_TopRight
                        Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y)
                        Texture = Tile.Texture
                        ToolPerformTile()
                    End If
                    If PosNum.X < Terrain.TileSize.X Then
                        VertexDirection = TileDirection_TopLeft
                        Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                        Texture = Tile.Texture
                        ToolPerformTile()
                    End If
                End If

                BestNum = -1
                BestCount = 0
                For A = 0 To Painter.TerrainCount - 1
                    If TerrainCount(A) > BestCount Then
                        BestNum = A
                        BestCount = TerrainCount(A)
                    End If
                Next
                If BestCount > 0 Then
                    Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = Painter.Terrains(BestNum)
                Else
                    Terrain.Vertices(PosNum.X, PosNum.Y).Terrain = Nothing
                End If

                Map.SectorTerrainUndoChanges.VertexChanged(PosNum)
            End Sub
        End Class

        Public Class clsApplyTileTerrainInterpret
            Inherits clsMap.clsAction

            Private Painter As clsPainter
            Private PainterTerrainA As clsPainter.clsTerrain
            Private PainterTerrainB As clsPainter.clsTerrain
            Private Texture As clsTerrain.Tile.sTexture
            Private ResultDirection As sTileDirection
            Private PainterTexture As clsPainter.clsTileList.sTile_Orientation_Chance
            Private OppositeDirection As sTileDirection
            Private Tile As clsTerrain.Tile
            Private Terrain As clsTerrain

            Public Overrides Sub ActionPerform()
                Dim PainterBrushNum As Integer
                Dim A As Integer

                Terrain = Map.Terrain

                Painter = Map.Painter

                Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                Texture = Tile.Texture

                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = False
                Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = TileDirection_None

                For PainterBrushNum = 0 To Painter.CliffBrushCount - 1
                    PainterTerrainA = Painter.CliffBrushes(PainterBrushNum).Terrain_Inner
                    PainterTerrainB = Painter.CliffBrushes(PainterBrushNum).Terrain_Outer
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Straight.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            If Tile.Tri Then
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                            Else
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                                Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                            End If
                            Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            Terrain.Tiles(PosNum.X, PosNum.Y).DownSide = ResultDirection
                        End If
                    Next
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_In.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If Tile.Tri Then
                                If IdenticalTileOrientations(ResultDirection, TileDirection_TopLeft) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                ElseIf IdenticalTileOrientations(ResultDirection, TileDirection_BottomRight) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                End If
                            Else
                                If IdenticalTileOrientations(ResultDirection, TileDirection_TopRight) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                ElseIf IdenticalTileOrientations(ResultDirection, TileDirection_BottomLeft) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                End If
                            End If
                        End If
                    Next
                    For A = 0 To Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.TileCount - 1
                        PainterTexture = Painter.CliffBrushes(PainterBrushNum).Tiles_Corner_Out.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            OppositeDirection = PainterTexture.Direction
                            OppositeDirection.FlipX()
                            OppositeDirection.FlipY()
                            RotateDirection(OppositeDirection, Texture.Orientation, ResultDirection)
                            If Tile.Tri Then
                                If IdenticalTileOrientations(ResultDirection, TileDirection_TopLeft) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                ElseIf IdenticalTileOrientations(ResultDirection, TileDirection_BottomRight) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                End If
                            Else
                                If IdenticalTileOrientations(ResultDirection, TileDirection_TopRight) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                ElseIf IdenticalTileOrientations(ResultDirection, TileDirection_BottomLeft) Then
                                    Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff = True
                                    Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff = True
                                End If
                            End If
                        End If
                    Next
                Next

                Map.SectorTerrainUndoChanges.TileChanged(PosNum)
            End Sub
        End Class

        Public MustInherit Class clsApplySideTerrainInterpret
            Inherits clsMap.clsAction

            Protected Painter As clsPainter
            Protected PainterTerrain As clsPainter.clsTerrain
            Protected PainterRoad As clsPainter.clsRoad
            Protected Texture As clsTerrain.Tile.sTexture
            Protected ResultDirection As sTileDirection
            Protected PainterTexture As clsPainter.clsTileList.sTile_Orientation_Chance
            Protected OppositeDirection As sTileDirection
            Protected Tile As clsTerrain.Tile
            Protected RoadCount() As Integer
            Protected SideDirection As sTileDirection
            Protected BestNum As Integer
            Protected BestCount As Integer
            Protected Terrain As clsTerrain

            Protected Sub ToolPerformTile()
                Dim PainterBrushNum As Integer
                Dim A As Integer

                For PainterBrushNum = 0 To Painter.RoadBrushCount - 1
                    PainterRoad = Painter.RoadBrushes(PainterBrushNum).Road
                    PainterTerrain = Painter.RoadBrushes(PainterBrushNum).Terrain
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Corner_In.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If DirectionsOnSameSide(SideDirection, ResultDirection) Then
                                RoadCount(PainterRoad.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_CrossIntersection.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RoadCount(PainterRoad.Num) += 1
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_End.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_End.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If IdenticalTileOrientations(SideDirection, ResultDirection) Then
                                RoadCount(PainterRoad.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_Straight.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_Straight.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If DirectionsAreInLine(SideDirection, ResultDirection) Then
                                RoadCount(PainterRoad.Num) += 1
                            End If
                        End If
                    Next
                    For A = 0 To Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.TileCount - 1
                        PainterTexture = Painter.RoadBrushes(PainterBrushNum).Tile_TIntersection.Tiles(A)
                        If PainterTexture.TextureNum = Texture.TextureNum Then
                            RotateDirection(PainterTexture.Direction, Texture.Orientation, ResultDirection)
                            If Not DirectionsOnSameSide(SideDirection, ResultDirection) Then
                                RoadCount(PainterRoad.Num) += 1
                            End If
                        End If
                    Next
                Next
            End Sub

            Public Overrides Sub ActionPerform()

                Terrain = Map.Terrain

                Painter = Map.Painter
                ReDim RoadCount(Painter.RoadCount - 1)
            End Sub
        End Class

        Public Class clsApplySideHTerrainInterpret
            Inherits clsMap.clsAction.clsApplySideTerrainInterpret

            Public Overrides Sub ActionPerform()
                MyBase.ActionPerform()

                Dim A As Integer

                If PosNum.Y > 0 Then
                    SideDirection = TileDirection_Bottom
                    Tile = Terrain.Tiles(PosNum.X, PosNum.Y - 1)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
                If PosNum.Y < Terrain.TileSize.Y Then
                    SideDirection = TileDirection_Top
                    Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If

                BestNum = -1
                BestCount = 0
                For A = 0 To Painter.RoadCount - 1
                    If RoadCount(A) > BestCount Then
                        BestNum = A
                        BestCount = RoadCount(A)
                    End If
                Next
                If BestCount > 0 Then
                    Terrain.SideH(PosNum.X, PosNum.Y).Road = Painter.Roads(BestNum)
                Else
                    Terrain.SideH(PosNum.X, PosNum.Y).Road = Nothing
                End If

                Map.SectorTerrainUndoChanges.SideHChanged(PosNum)
            End Sub
        End Class

        Public Class clsApplySideVTerrainInterpret
            Inherits clsMap.clsAction.clsApplySideTerrainInterpret

            Public Overrides Sub ActionPerform()
                MyBase.ActionPerform()

                Dim A As Integer

                If PosNum.X > 0 Then
                    SideDirection = TileDirection_Right
                    Tile = Terrain.Tiles(PosNum.X - 1, PosNum.Y)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If
                If PosNum.X < Terrain.TileSize.X Then
                    SideDirection = TileDirection_Left
                    Tile = Terrain.Tiles(PosNum.X, PosNum.Y)
                    Texture = Tile.Texture
                    ToolPerformTile()
                End If

                BestNum = -1
                BestCount = 0
                For A = 0 To Painter.RoadCount - 1
                    If RoadCount(A) > BestCount Then
                        BestNum = A
                        BestCount = RoadCount(A)
                    End If
                Next
                If BestCount > 0 Then
                    Terrain.SideV(PosNum.X, PosNum.Y).Road = Painter.Roads(BestNum)
                Else
                    Terrain.SideV(PosNum.X, PosNum.Y).Road = Nothing
                End If

                Map.SectorTerrainUndoChanges.SideVChanged(PosNum)
            End Sub
        End Class
    End Class
End Class