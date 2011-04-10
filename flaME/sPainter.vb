Public Structure sPainter
    Public Class clsTerrain
        Public Num As Integer
        Public Name As String

        Public Tiles As sTileList
    End Class
    Public Terrains() As clsTerrain
    Public TerrainCount As UInteger
    Public Structure sTileList
        Public Structure sTile_Orientation_Chance
            Public TextureNum As Integer
            Public Direction As sTileDirection
            Public Chance As UInteger
        End Structure
        Public Tiles() As sTile_Orientation_Chance
        Public TileCount As Integer
        Public TileChanceTotal As Integer

        Public Sub Tile_Add(ByVal Tile_Num As Integer, ByVal Tile_Outward_Orientation As sTileDirection, ByVal Chance As UInteger)

            ReDim Preserve Tiles(TileCount)
            Tiles(TileCount).TextureNum = Tile_Num
            Tiles(TileCount).Direction = Tile_Outward_Orientation
            Tiles(TileCount).Chance = Chance
            TileCount += 1

            TileChanceTotal += Chance
        End Sub

        Public Sub Tile_Remove(ByVal Num As Integer)

            TileChanceTotal -= Tiles(Num).Chance

            TileCount -= 1
            If Num <> TileCount Then
                Tiles(Num) = Tiles(TileCount)
            End If
            ReDim Preserve Tiles(TileCount - 1)
        End Sub

        Public Function GetRandom() As sTile_Orientation_Chance
            Dim A As Integer
            Dim intRandom As Integer
            Dim Total As Integer

            intRandom = Math.Floor(Rnd() * TileChanceTotal)
            For A = 0 To TileCount - 1
                Total += Tiles(A).Chance
                If intRandom < Total Then
                    Exit For
                End If
            Next A
            If A = TileCount Then
                GetRandom.TextureNum = -1
                GetRandom.Direction = TileDirection_None
            Else
                GetRandom = Tiles(A)
            End If
        End Function
    End Structure
    Public Structure sTransition_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As sTileList
        Public Tiles_Corner_In As sTileList
        Public Tiles_Corner_Out As sTileList
    End Structure
    Public Structure sCliff_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As sTileList
        Public Tiles_Corner_In As sTileList
        Public Tiles_Corner_Out As sTileList
    End Structure
    Public TransitionBrushes() As sTransition_Brush
    Public TransitionBrushCount As UInteger
    Public CliffBrushes() As sCliff_Brush
    Public CliffBrushCount As UInteger

    Public Class clsRoad
        Public Num As Integer
        Public Name As String
    End Class
    Public Roads() As clsRoad
    Public RoadCount As UInteger
    Public Structure sRoad_Brush
        Dim Road As clsRoad
        Dim Terrain As clsTerrain
        Dim Tile_Straight As sTileList
        Dim Tile_Corner_In As sTileList
        Dim Tile_TIntersection As sTileList
        Dim Tile_CrossIntersection As sTileList
        Dim Tile_End As sTileList
    End Structure
    Public RoadBrushes() As sRoad_Brush
    Public RoadBrushCount As UInteger

    Public Sub TransitionBrush_Add(ByVal NewBrush As sTransition_Brush)

        ReDim Preserve TransitionBrushes(TransitionBrushCount)
        TransitionBrushes(TransitionBrushCount) = NewBrush
        TransitionBrushCount += 1
    End Sub

    Public Sub TransitionBrush_Remove(ByVal Num As Integer)

        TransitionBrushCount -= 1
        If Num <> TransitionBrushCount Then
            TransitionBrushes(Num) = TransitionBrushes(TransitionBrushCount)
        End If
        ReDim Preserve TransitionBrushes(TransitionBrushCount - 1)
    End Sub

    Public Sub CliffBrush_Add(ByVal NewBrush As sCliff_Brush)

        ReDim Preserve CliffBrushes(CliffBrushCount)
        CliffBrushes(CliffBrushCount) = NewBrush
        CliffBrushCount += 1
    End Sub

    Public Sub CliffBrush_Remove(ByVal Num As Integer)

        CliffBrushCount -= 1
        If Num <> CliffBrushCount Then
            CliffBrushes(Num) = CliffBrushes(CliffBrushCount)
        End If
        ReDim Preserve CliffBrushes(CliffBrushCount - 1)
    End Sub

    Public Sub Terrain_Add(ByVal NewTerrain As clsTerrain)

        NewTerrain.Num = TerrainCount
        ReDim Preserve Terrains(TerrainCount)
        Terrains(TerrainCount) = NewTerrain
        TerrainCount += 1
    End Sub

    Public Sub Terrain_Remove(ByVal Num As Integer)

        Terrains(Num).Num = -1
        TerrainCount -= 1
        If Num <> TerrainCount Then
            Terrains(Num) = Terrains(TerrainCount)
            Terrains(Num).Num = Num
        End If
        ReDim Preserve Terrains(TerrainCount - 1)
    End Sub

    Public Sub RoadBrush_Add(ByVal NewRoadBrush As sRoad_Brush)

        ReDim Preserve RoadBrushes(RoadBrushCount)
        RoadBrushes(RoadBrushCount) = NewRoadBrush
        RoadBrushCount += 1
    End Sub

    Public Sub RoadBrush_Remove(ByVal Num As Integer)

        RoadBrushCount -= 1
        If Num <> RoadBrushCount Then
            RoadBrushes(Num) = RoadBrushes(RoadBrushCount)
        End If
        ReDim Preserve RoadBrushes(RoadBrushCount - 1)
    End Sub

    Public Sub Road_Add(ByVal NewRoad As clsRoad)

        NewRoad.Num = RoadCount
        ReDim Preserve Roads(RoadCount)
        Roads(RoadCount) = NewRoad
        RoadCount += 1
    End Sub

    Public Sub Road_Remove(ByVal Num As Integer)

        Roads(Num).Num = -1
        RoadCount -= 1
        If Num <> RoadCount Then
            Roads(Num) = Roads(RoadCount)
            Roads(Num).Num = Num
        End If
        ReDim Preserve Roads(RoadCount - 1)
    End Sub
End Structure