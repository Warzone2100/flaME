
Public Class clsPainter
    Public Class clsTerrain
        Public Num As Integer
        Public Name As String

        Public Tiles As New clsTileList
    End Class
    Public Terrains As New SimpleList(Of clsTerrain)
    Public Class clsTileList
        Public Class clsTileOrientationChance
            Public TextureNum As Integer
            Public Direction As sTileDirection
            Public Chance As UInteger
        End Class
        Public Tiles As New SimpleList(Of clsTileOrientationChance)
        Public TileChanceTotal As Integer

        Public Sub Tile_Add(TileNum As Integer, TileOutwardOrientation As sTileDirection, Chance As UInteger)

            Dim tile As New clsTileOrientationChance
            tile.TextureNum = TileNum
            tile.Direction = TileOutwardOrientation
            tile.Chance = Chance
            Tiles.Add(tile)
            TileChanceTotal += CInt(Chance)
        End Sub

        Public Function GetRandom() As clsTileOrientationChance

            Dim result As New clsTileOrientationChance
            Dim randomNumber As Integer = CInt(Int(Rnd() * TileChanceTotal))
            Dim limit As Integer = 0
            For i As Integer = 0 To Tiles.Count - 1
                limit += CInt(Tiles(i).Chance)
                If randomNumber < limit Then
                    result.TextureNum = Tiles(i).TextureNum
                    result.Direction = Tiles(i).Direction
                    Return result
                End If
            Next
            result.TextureNum = -1
            result.Direction = TileDirection_None
            Return result
        End Function
    End Class
    Public Class clsTransition_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As New clsTileList
        Public Tiles_Corner_In As New clsTileList
        Public Tiles_Corner_Out As New clsTileList
    End Class
    Public TransitionBrushes As New SimpleList(Of clsTransition_Brush)
    Public Class clsCliff_Brush
        Public Name As String
        Public Terrain_Inner As clsTerrain
        Public Terrain_Outer As clsTerrain
        Public Tiles_Straight As New clsTileList
        Public Tiles_Corner_In As New clsTileList
        Public Tiles_Corner_Out As New clsTileList
    End Class
    Public CliffBrushes As New SimpleList(Of clsCliff_Brush)

    Public Class clsRoad
        Public Num As Integer
        Public Name As String
    End Class
    Public Roads As New SimpleList(Of clsRoad)
    Public Class clsRoad_Brush
        Public Road As clsRoad
        Public Terrain As clsTerrain
        Public Tile_Straight As New clsTileList
        Public Tile_Corner_In As New clsTileList
        Public Tile_TIntersection As New clsTileList
        Public Tile_CrossIntersection As New clsTileList
        Public Tile_End As New clsTileList
    End Class
    Public RoadBrushes As New SimpleList(Of clsRoad_Brush)

    Public Sub TransitionBrush_Add(NewBrush As clsTransition_Brush)

        TransitionBrushes.Add(NewBrush)
    End Sub

    Public Sub CliffBrush_Add(NewBrush As clsCliff_Brush)

        CliffBrushes.Add(NewBrush)
    End Sub

    Public Sub Terrain_Add(NewTerrain As clsTerrain)

        NewTerrain.Num = Terrains.Count
        Terrains.Add(NewTerrain)
    End Sub

    Public Sub RoadBrush_Add(NewRoadBrush As clsRoad_Brush)

        RoadBrushes.Add(NewRoadBrush)
    End Sub

    Public Sub Road_Add(NewRoad As clsRoad)

        NewRoad.Num = Roads.Count
        Roads.Add(NewRoad)
    End Sub
End Class
