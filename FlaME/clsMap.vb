Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public Class clsTerrain

#If Mono267 = 0.0# Then
        Public Structure Vertex
            Public Height As Byte
            Public Terrain As clsPainter.clsTerrain
        End Structure
#Else
    Public Class Vertex
        Public Height As Byte
        Public Terrain As clsPainter.clsTerrain
    End Class
#End If
#If Mono267 = 0.0# Then
        Public Structure Tile
            Public Structure sTexture
                Public TextureNum As Integer
                Public Orientation As sTileOrientation
            End Structure
            Public Texture As sTexture
            Public Tri As Boolean
            Public TriTopLeftIsCliff As Boolean
            Public TriTopRightIsCliff As Boolean
            Public TriBottomLeftIsCliff As Boolean
            Public TriBottomRightIsCliff As Boolean
            Public Terrain_IsCliff As Boolean
            Public DownSide As sTileDirection

            Public Sub Copy(ByVal TileToCopy As Tile)

                Texture = TileToCopy.Texture
                Tri = TileToCopy.Tri
                TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff
                TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff
                TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff
                TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff
                Terrain_IsCliff = TileToCopy.Terrain_IsCliff
                DownSide = TileToCopy.DownSide
            End Sub
        End Structure
#Else
    Public Class Tile
        Public Structure sTexture
            Public TextureNum As Integer
            Public Orientation As sTileOrientation
        End Structure
        Public Texture As sTexture
        Public Tri As Boolean
        Public TriTopLeftIsCliff As Boolean
        Public TriTopRightIsCliff As Boolean
        Public TriBottomLeftIsCliff As Boolean
        Public TriBottomRightIsCliff As Boolean
        Public Terrain_IsCliff As Boolean
        Public DownSide As sTileDirection

        Public Sub Copy(ByVal TileToCopy As Tile)
                Texture = TileToCopy.Texture
                Tri = TileToCopy.Tri
                TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff
                TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff
                TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff
                TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff
                Terrain_IsCliff = TileToCopy.Terrain_IsCliff
                DownSide = TileToCopy.DownSide
            End Sub
    End Class
#End If
#If Mono267 = 0.0# Then
        Public Structure Side
            Public Road As clsPainter.clsRoad
        End Structure
#Else
    Public Class Side
        Public Road As clsPainter.clsRoad
    End Class
#End If

        Public TileSize As sXY_int

        Public Vertices(,) As clsMap.clsTerrain.Vertex
        Public Tiles(,) As clsMap.clsTerrain.Tile
        Public SideH(,) As clsMap.clsTerrain.Side
        Public SideV(,) As clsMap.clsTerrain.Side

        Public Sub New(ByVal NewSize As sXY_int)

            TileSize = NewSize

            ReDim Vertices(TileSize.X, TileSize.Y)
            ReDim Tiles(TileSize.X - 1, TileSize.Y - 1)
            ReDim SideH(TileSize.X - 1, TileSize.Y)
            ReDim SideV(TileSize.X, TileSize.Y - 1)
            Dim X As Integer
			Dim Y As Integer
#If Mono267 <> 0.0# Then
            For Y = 0 To TileSize.Y - 1
                For X = 0 To TileSize.X - 1
                    Tiles(X, Y) = New Tile
                Next
            Next
            For Y = 0 To TileSize.Y
                For X = 0 To TileSize.X
                    Vertices(X, Y) = New Vertex
                Next
            Next
            For Y = 0 To TileSize.Y
                For X = 0 To TileSize.X - 1
                    SideH(X, Y) = New Side
                Next
            Next
            For Y = 0 To TileSize.Y - 1
                For X = 0 To TileSize.X
                    SideV(X, Y) = New Side
                Next
            Next
#End If
            For Y = 0 To TileSize.Y - 1
                For X = 0 To TileSize.X - 1
                    Tiles(X, Y).Texture.TextureNum = -1
                    Tiles(X, Y).DownSide = TileDirection_None
                Next
            Next
        End Sub
    End Class

    Public Terrain As clsTerrain

    Public Class clsSector
        Public Pos As sXY_int
        Public GLList_Textured As Integer
        Public GLList_Wireframe As Integer
        Public Units(-1) As clsUnit
        Public Unit_SectorNum(-1) As Integer
        Public UnitCount As Integer

        Public Sub Deallocate()

            ReDim Units(-1)
            ReDim Unit_SectorNum(-1)
            UnitCount = 0
        End Sub

        Public Function Unit_Add(ByVal NewUnit As clsMap.clsUnit, ByVal NewUnitSectorNum As Integer) As Integer
            Dim ReturnResult As Integer

            ReDim Preserve Units(UnitCount)
            ReDim Preserve Unit_SectorNum(UnitCount)
            Units(UnitCount) = NewUnit
            Unit_SectorNum(UnitCount) = NewUnitSectorNum
            ReturnResult = UnitCount
            UnitCount += 1

            Return ReturnResult
        End Function

        Public Sub Unit_Remove(ByVal Num As Integer)

            Units(Num).Sectors_UnitNum(Unit_SectorNum(Num)) = -1

            UnitCount = UnitCount - 1
            If Num <> UnitCount Then
                Units(UnitCount).Sectors_UnitNum(Unit_SectorNum(UnitCount)) = Num
                Units(Num) = Units(UnitCount)
                Unit_SectorNum(Num) = Unit_SectorNum(UnitCount)
            End If
            ReDim Preserve Units(UnitCount - 1)
            ReDim Preserve Unit_SectorNum(UnitCount - 1)
        End Sub

        Public Sub New(ByVal NewPos As sXY_int)

            Pos = NewPos
        End Sub
    End Class
    Public Sectors(-1, -1) As clsSector
    Public SectorCount As sXY_int

    Public Class clsShadowSector
        Public Num As sXY_int
        Public Terrain As New clsTerrain(New sXY_int(SectorTileSize, SectorTileSize))
    End Class
    Public ShadowSectors(-1, -1) As clsShadowSector

    Public Structure sUnitChange
        Public Enum enumType As Byte
            Added
            Deleted
        End Enum
        Public Type As enumType
        Public Unit As clsUnit
    End Structure

    Public Class clsUndo
        Public Name As String
        Public ChangedSectors() As clsShadowSector
        Public ChangedSectorCount As Integer
        Public UnitChanges() As sUnitChange
        Public UnitChangeCount As Integer
    End Class
    Public Undos() As clsUndo
    Public UndoCount As Integer
    Public Undo_Pos As Integer

    Public UnitChanges(-1) As sUnitChange
    Public UnitChangeCount As Integer

    Public HeightMultiplier As Integer = DefaultHeightMultiplier

    Public SelectedUnits() As clsUnit
    Public SelectedUnitCount As Integer
    Public Selected_Tile_A As clsXY_int
    Public Selected_Tile_B As clsXY_int
    Public Selected_Area_VertexA As clsXY_int
    Public Selected_Area_VertexB As clsXY_int
    Public Unit_Selected_Area_VertexA As clsXY_int

    Public Class clsUnit
        Public Map_UnitNum As Integer = -1
        Public Map_SelectedUnitNum As Integer = -1
        Public Sectors(-1) As clsSector
        Public Sectors_UnitNum(-1) As Integer
        Public SectorCount As Integer

        Public ID As UInteger
        Public SavePriority As Integer
        Public Type As clsUnitType
        Public Pos As sWorldPos
        Public Rotation As Integer
        'Public Name As String = "NONAME"
        Public UnitGroup As clsMap.clsUnitGroup
        Public Health As Double = 1.0#
        Public PreferPartsOutput As Boolean = False

        Public Sub New()

        End Sub

        Public Sub New(ByVal Unit_To_Copy As clsUnit)
            Dim IsDesign As Boolean

            If Unit_To_Copy.Type.Type = clsUnitType.enumType.PlayerDroid Then
                IsDesign = Not CType(Unit_To_Copy.Type, clsDroidDesign).IsTemplate
            Else
                IsDesign = False
            End If
            If IsDesign Then
                Dim tmpDroidDesign As New clsDroidDesign
                Type = tmpDroidDesign
                tmpDroidDesign.CopyDesign(CType(Unit_To_Copy.Type, clsDroidDesign))
                tmpDroidDesign.UpdateAttachments()
            Else
                Type = Unit_To_Copy.Type
            End If
            Pos = Unit_To_Copy.Pos
            Rotation = Unit_To_Copy.Rotation
            UnitGroup = Unit_To_Copy.UnitGroup
            SavePriority = Unit_To_Copy.SavePriority
            Health = Unit_To_Copy.Health
            PreferPartsOutput = Unit_To_Copy.PreferPartsOutput
        End Sub

        Public Sub Sectors_Remove()

            SectorCount = 0
            ReDim Sectors(-1)
            ReDim Sectors_UnitNum(-1)
        End Sub

        Public Function GetINIRotation() As String
            Dim Rotation16 As Integer

            Rotation16 = CInt(Rotation * 65536.0# / 360.0#)
            If Rotation16 >= 63356 Then
                Rotation16 -= 65536
            End If

            Return Rotation16 & ", 0, 0"
        End Function

        Public Function GetPosText() As String

            Return Pos.Horizontal.X & ", " & Pos.Horizontal.Y
        End Function
    End Class
    Public Units(-1) As clsUnit
    Public UnitCount As Integer

    Public Class clsUnitGroup

        Public ParentMap As clsMap
        Public Map_UnitGroupNum As Integer = -1

        Public WZ_StartPos As Integer = -1

        Public Function GetLNDPlayerText() As String

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return "7"
            Else
                Return CStr(WZ_StartPos)
            End If
        End Function
    End Class
    Public UnitGroups(-1) As clsUnitGroup
    Public ScavengerUnitGroup As clsUnitGroup
    Public FeatureUnitGroup As clsUnitGroup

    Public Minimap_GLTexture As Integer
    Public Minimap_Texture_Size As Integer

    Public ReceivingUserChanges As Boolean = False

    Public Tileset As clsTileset

    Public Class clsPathInfo
        Private _Path As String
        Private _IsFMap As Boolean

        Public ReadOnly Property Path As String
            Get
                Return _Path
            End Get
        End Property

        Public ReadOnly Property IsFMap As Boolean
            Get
                Return _IsFMap
            End Get
        End Property

        Public Sub New(ByVal NewPath As String, ByVal NewIsFMap As Boolean)

            _Path = NewPath
            _IsFMap = NewIsFMap
        End Sub
    End Class
    Public PathInfo As clsPathInfo

    Public Class clsAutoSave
        Public ChangeCount As UInteger
        Public SavedDate As Date

        Public Sub New()

            SavedDate = Now
        End Sub
    End Class
    Public AutoSave As New clsAutoSave

    Public Painter As New clsPainter

    Public Tile_TypeNum(-1) As Byte

    Public Structure sGateway
        Public PosA As sXY_int
        Public PosB As sXY_int
    End Structure
    Public Gateways(-1) As sGateway
    Public GatewayCount As Integer

    Public Class clsPointChanges

        Public PointIsChanged(,) As Boolean
        Public ChangedPoints() As sXY_int
        Public ChangedPointCount As Integer

        Public Sub New(ByVal PointSize As sXY_int)

            ReDim PointIsChanged(PointSize.X - 1, PointSize.Y - 1)
            ReDim ChangedPoints(PointSize.X * PointSize.Y - 1)
        End Sub

        Public Sub Changed(ByVal Num As sXY_int)

            If Not PointIsChanged(Num.X, Num.Y) Then
                PointIsChanged(Num.X, Num.Y) = True
                ChangedPoints(ChangedPointCount) = Num
                ChangedPointCount += 1
            End If
        End Sub

        Public Sub SetAllChanged()
            Dim X As Integer
            Dim Y As Integer
            Dim Num As sXY_int

            For Y = 0 To PointIsChanged.GetUpperBound(1)
                Num.Y = Y
                For X = 0 To PointIsChanged.GetUpperBound(0)
                    Num.X = X
                    Changed(Num)
                Next
            Next
        End Sub

        Public Sub Clear()
            Dim A As Integer

            For A = 0 To ChangedPointCount - 1
                PointIsChanged(ChangedPoints(A).X, ChangedPoints(A).Y) = False
            Next
            ChangedPointCount = 0
        End Sub

        Public Sub PerformTool(ByVal Tool As clsMap.clsAction)
            Dim A As Integer

            For A = 0 To ChangedPointCount - 1
                Tool.PosNum = ChangedPoints(A)
                Tool.ActionPerform()
            Next
        End Sub
    End Class

    Public MustInherit Class clsMapChanges
        Inherits clsPointChanges

        Public Map As clsMap
        Public Terrain As clsTerrain

        Public Sub New(ByVal Map As clsMap, ByVal PointSize As sXY_int)
            MyBase.New(PointSize)

            Me.Map = Map
            Me.Terrain = Map.Terrain
        End Sub

        Public Sub Deallocate()

            Map = Nothing
        End Sub

        Public MustOverride Sub TileChanged(ByVal Num As sXY_int)

        Public Sub VertexChanged(ByVal Num As sXY_int)

            If Num.X > 0 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
            End If
        End Sub

        Public Sub VertexAndNormalsChanged(ByVal Num As sXY_int)

            If Num.X > 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y))
                End If
            End If
            If Num.X > 0 Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X - 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y))
                End If
            End If
        End Sub

        Public Sub SideHChanged(ByVal Num As sXY_int)

            If Num.Y > 0 Then
                TileChanged(New sXY_int(Num.X, Num.Y - 1))
            End If
            If Num.Y < Map.Terrain.TileSize.Y Then
                TileChanged(Num)
            End If
        End Sub

        Public Sub SideVChanged(ByVal Num As sXY_int)

            If Num.X > 0 Then
                TileChanged(New sXY_int(Num.X - 1, Num.Y))
            End If
            If Num.X < Map.Terrain.TileSize.X Then
                TileChanged(Num)
            End If
        End Sub
    End Class

    Public Class clsSectorChanges
        Inherits clsMap.clsMapChanges

        Public Sub New(ByVal Map As clsMap)
            MyBase.New(Map, Map.SectorCount)

        End Sub

        Public Overrides Sub TileChanged(ByVal Num As sXY_int)
            Dim SectorNum As sXY_int

            SectorNum = Map.GetTileSectorNum(Num)
            Changed(SectorNum)
        End Sub
    End Class

    Public Class clsAutoTextureChanges
        Inherits clsMap.clsMapChanges

        Public Sub New(ByVal Map As clsMap)
            MyBase.New(Map, Map.Terrain.TileSize)

        End Sub

        Public Overrides Sub TileChanged(ByVal Num As modMath.sXY_int)

            Changed(Num)
        End Sub
    End Class

    Public SectorGraphicsChanges As clsSectorChanges
    Public SectorUnitHeightsChanges As clsSectorChanges
    Public SectorTerrainUndoChanges As clsSectorChanges
    Public AutoTextureChanges As clsAutoTextureChanges

    Public Class clsTerrainUpdate

        Public Vertices As clsPointChanges
        Public Tiles As clsPointChanges
        Public SidesH As clsPointChanges
        Public SidesV As clsPointChanges

        Public Sub Deallocate()


        End Sub

        Public Sub New(ByVal TileSize As sXY_int)

            Vertices = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y + 1))
            Tiles = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y))
            SidesH = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y + 1))
            SidesV = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y))
        End Sub

        Public Sub SetAllChanged()

            Vertices.SetAllChanged()
            Tiles.SetAllChanged()
            SidesH.SetAllChanged()
            SidesV.SetAllChanged()
        End Sub

        Public Sub ClearAll()

            Vertices.Clear()
            Tiles.Clear()
            SidesH.Clear()
            SidesV.Clear()
        End Sub
    End Class

    Public TerrainInterpretChanges As clsTerrainUpdate

    Public Sub New()

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()
    End Sub

    Public Sub New(ByVal TileSize As sXY_int)

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()

        Terrain_Blank(TileSize)
        TileType_Reset()
        AfterInitialized()
    End Sub

    Public Sub New(ByVal Map_To_Copy As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int)
        Dim StartX As Integer
        Dim StartY As Integer
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer

        MakeDefaultUnitGroups()

        'make some map data for selection

        StartX = Math.Max(0 - Offset.X, 0)
        StartY = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Map_To_Copy.Terrain.TileSize.X - Offset.X, Area.X)
        EndY = Math.Min(Map_To_Copy.Terrain.TileSize.Y - Offset.Y, Area.Y)

        Terrain = New clsTerrain(Area)

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = StartY To EndY
            For X = StartX To EndX
                Terrain.Vertices(X, Y).Height = Map_To_Copy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                Terrain.Vertices(X, Y).Terrain = Map_To_Copy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX - 1
                Terrain.Tiles(X, Y).Copy(Map_To_Copy.Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = StartY To EndY
            For X = StartX To EndX - 1
                Terrain.SideH(X, Y).Road = Map_To_Copy.Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX
                Terrain.SideV(X, Y).Road = Map_To_Copy.Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        SectorCount.X = CInt(Math.Ceiling(Area.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Area.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer
        Dim NewUnit As clsUnit

        For A = 0 To Map_To_Copy.GatewayCount - 1
            If (Map_To_Copy.Gateways(A).PosA.X >= Offset.X And Map_To_Copy.Gateways(A).PosA.Y >= Offset.Y And _
              Map_To_Copy.Gateways(A).PosA.X < Offset.X + Area.X And Map_To_Copy.Gateways(A).PosA.Y < Offset.Y + Area.Y) Or _
              (Map_To_Copy.Gateways(A).PosB.X >= Offset.X And Map_To_Copy.Gateways(A).PosB.Y >= Offset.Y And _
              Map_To_Copy.Gateways(A).PosB.X < Offset.X + Area.X And Map_To_Copy.Gateways(A).PosB.Y < Offset.Y + Area.Y) Then
                Gateway_Add(New sXY_int(Map_To_Copy.Gateways(A).PosA.X - Offset.X, Map_To_Copy.Gateways(A).PosA.Y - Offset.Y), New sXY_int(Map_To_Copy.Gateways(A).PosB.X - Offset.X, Map_To_Copy.Gateways(A).PosB.Y - Offset.Y))
            End If
        Next

        PosDifX = -Offset.X * TerrainGridSpacing
        PosDifZ = -Offset.Y * TerrainGridSpacing
        For A = 0 To Map_To_Copy.UnitCount - 1
            NewUnit = New clsUnit(Map_To_Copy.Units(A))
            If NewUnit.UnitGroup.WZ_StartPos < 0 Then
                NewUnit.UnitGroup = ScavengerUnitGroup
            Else
                NewUnit.UnitGroup = UnitGroups(NewUnit.UnitGroup.WZ_StartPos)
            End If
            NewUnit.Pos.Horizontal.X += PosDifX
            NewUnit.Pos.Horizontal.Y += PosDifZ
            If Not (NewUnit.Pos.Horizontal.X < 0 _
              Or NewUnit.Pos.Horizontal.X >= Terrain.TileSize.X * TerrainGridSpacing _
              Or NewUnit.Pos.Horizontal.Y < 0 _
              Or NewUnit.Pos.Horizontal.Y >= Terrain.TileSize.Y * TerrainGridSpacing) Then
                Unit_Add(NewUnit)
            End If
        Next

        AfterInitialized()
    End Sub

    Protected Sub Terrain_Blank(ByVal TileSize As sXY_int)
        Dim X As Integer
        Dim Y As Integer

        Terrain = New clsTerrain(TileSize)
        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next
    End Sub

    Public Function GetTerrainTri(ByVal Horizontal As sXY_int) As Boolean
        Dim X1 As Integer
        Dim Y1 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Return False
            Else
                Return True
            End If
        Else
            If InTileZ <= InTileX Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Function GetTerrainSlopeAngle(ByVal Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim XYZ_dbl3 As Matrix3D.XYZ_dbl
        Dim AnglePY As Matrix3D.AnglePY

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
            End If
        End If

        XYZ_dbl.X = TerrainGridSpacing
        XYZ_dbl.Y = GradientX * HeightMultiplier
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = GradientY * HeightMultiplier
        XYZ_dbl2.Z = TerrainGridSpacing
        Matrix3D.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, XYZ_dbl3)
        If XYZ_dbl3.X <> 0.0# Or XYZ_dbl3.Z <> 0.0# Then
            Matrix3D.VectorToPY(XYZ_dbl3, AnglePY)
            Return RadOf90Deg - Math.Abs(AnglePY.Pitch)
        Else
            Return 0.0#
        End If
    End Function

    Public Function GetTerrainHeight(ByVal Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim RatioX As Double
        Dim RatioY As Double

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
                RatioX = InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = 1.0# - InTileZ
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
                RatioX = InTileX
                RatioY = 1.0# - InTileZ
            End If
        End If
        Return (Offset + GradientX * RatioX + GradientY * RatioY) * HeightMultiplier
    End Function

    Public Function TerrainVertexNormalCalc(ByVal X As Integer, ByVal Y As Integer) As sXYZ_sng
        Dim ReturnResult As sXYZ_sng
        Dim TerrainHeightX1 As Integer
        Dim TerrainHeightX2 As Integer
        Dim TerrainHeightY1 As Integer
        Dim TerrainHeightY2 As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp As Double

        X2 = Clamp_int(X - 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX2 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y - 1, 0, Terrain.TileSize.Y)
        TerrainHeightY1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y + 1, 0, Terrain.TileSize.Y)
        TerrainHeightY2 = Terrain.Vertices(X2, Y2).Height
        XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier
        XYZ_dbl.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl2.Z = (TerrainHeightY1 - TerrainHeightY2) * HeightMultiplier
        XYZ_dbl.X = XYZ_dbl.X + XYZ_dbl2.X
        XYZ_dbl.Y = XYZ_dbl.Y + XYZ_dbl2.Y
        XYZ_dbl.Z = XYZ_dbl.Z + XYZ_dbl2.Z
        dblTemp = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z)
        ReturnResult.X = CSng(XYZ_dbl.X / dblTemp)
        ReturnResult.Y = CSng(XYZ_dbl.Y / dblTemp)
        ReturnResult.Z = CSng(XYZ_dbl.Z / dblTemp)
        Return ReturnResult
    End Function

    Public Sub SectorAll_GLLists_Delete()
        Dim X As Integer
        Dim Y As Integer

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                If Sectors(X, Y).GLList_Textured > 0 Then
                    GL.DeleteLists(Sectors(X, Y).GLList_Textured, 1)
                    Sectors(X, Y).GLList_Textured = 0
                End If
                If Sectors(X, Y).GLList_Wireframe > 0 Then
                    GL.DeleteLists(Sectors(X, Y).GLList_Wireframe, 1)
                    Sectors(X, Y).GLList_Wireframe = 0
                End If
            Next
        Next
    End Sub

    Public Overridable Sub Deallocate()

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        Do While UnitCount > 0
            Unit_Remove(UnitCount - 1)
        Loop
        SectorAll_GLLists_Delete()
        If Minimap_GLTexture > 0 Then
            GL.DeleteTextures(1, Minimap_GLTexture)
        End If
        If SectorGraphicsChanges IsNot Nothing Then
            SectorGraphicsChanges.Deallocate()
        End If
        If SectorUnitHeightsChanges IsNot Nothing Then
            SectorUnitHeightsChanges.Deallocate()
        End If
        If SectorTerrainUndoChanges IsNot Nothing Then
            SectorTerrainUndoChanges.Deallocate()
        End If
        If AutoTextureChanges IsNot Nothing Then
            AutoTextureChanges.Deallocate()
        End If
        If TerrainInterpretChanges IsNot Nothing Then
            TerrainInterpretChanges.Deallocate()
        End If
        Undo_Clear()
    End Sub

    Public Sub Terrain_Resize(ByVal Offset As sXY_int, ByVal Size As sXY_int)
        Dim StartX As Integer
        Dim StartY As Integer
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(Size)

        Undo_Clear()
        SectorAll_GLLists_Delete()

        StartX = Math.Max(0 - Offset.X, 0)
        StartY = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Terrain.TileSize.X - Offset.X, Size.X)
        EndY = Math.Min(Terrain.TileSize.Y - Offset.Y, Size.Y)

        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = StartY To EndY
            For X = StartX To EndX
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX - 1
                NewTerrain.Tiles(X, Y).Copy(Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = StartY To EndY
            For X = StartX To EndX - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX
                NewTerrain.SideV(X, Y).Road = Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer

        PosDifX = -Offset.X * TerrainGridSpacing
        PosDifZ = -Offset.Y * TerrainGridSpacing
        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            Units(A).Pos.Horizontal.X += PosDifX
            Units(A).Pos.Horizontal.Y += PosDifZ
        Next
        For A = 0 To GatewayCount - 1
            Gateways(A).PosA.X -= Offset.X
            Gateways(A).PosA.Y -= Offset.Y
            Gateways(A).PosB.X -= Offset.X
            Gateways(A).PosB.Y -= Offset.Y
        Next
        If Selected_Tile_A IsNot Nothing Then
            Selected_Tile_A.X -= Offset.X
            Selected_Tile_A.Y -= Offset.Y
            If Selected_Tile_A.X < 0 _
                Or Selected_Tile_A.X >= NewTerrain.TileSize.X _
                Or Selected_Tile_A.Y < 0 _
                Or Selected_Tile_A.Y >= NewTerrain.TileSize.Y Then
                Selected_Tile_A = Nothing
                Selected_Tile_B = Nothing
            End If
        End If
        If Selected_Tile_B IsNot Nothing Then
            Selected_Tile_B.X -= Offset.X
            Selected_Tile_B.Y -= Offset.Y
            If Selected_Tile_B.X < 0 _
                Or Selected_Tile_B.X >= NewTerrain.TileSize.X _
                Or Selected_Tile_B.Y < 0 _
                Or Selected_Tile_B.Y >= NewTerrain.TileSize.Y Then
                Selected_Tile_A = Nothing
                Selected_Tile_B = Nothing
            End If
        End If
        If Selected_Area_VertexA IsNot Nothing Then
            Selected_Area_VertexA.X -= Offset.X
            Selected_Area_VertexA.Y -= Offset.Y
            If Selected_Area_VertexA.X < 0 _
                Or Selected_Area_VertexA.X > NewTerrain.TileSize.X _
                Or Selected_Area_VertexA.Y < 0 _
                Or Selected_Area_VertexA.Y > NewTerrain.TileSize.Y Then
                Selected_Area_VertexA = Nothing
                Selected_Area_VertexB = Nothing
            End If
        End If
        If Selected_Area_VertexB IsNot Nothing Then
            Selected_Area_VertexB.X -= Offset.X
            Selected_Area_VertexB.Y -= Offset.Y
            If Selected_Area_VertexB.X < 0 _
                Or Selected_Area_VertexB.X > NewTerrain.TileSize.X _
                Or Selected_Area_VertexB.Y < 0 _
                Or Selected_Area_VertexB.Y > NewTerrain.TileSize.Y Then
                Selected_Area_VertexA = Nothing
                Selected_Area_VertexB = Nothing
            End If
        End If
        If Unit_Selected_Area_VertexA IsNot Nothing Then
            Unit_Selected_Area_VertexA.X -= Offset.X
            Unit_Selected_Area_VertexA.Y -= Offset.Y
            If Unit_Selected_Area_VertexA.X < 0 _
                Or Unit_Selected_Area_VertexA.X > NewTerrain.TileSize.X _
                Or Unit_Selected_Area_VertexA.Y < 0 _
                Or Unit_Selected_Area_VertexA.Y > NewTerrain.TileSize.Y Then
                Unit_Selected_Area_VertexA = Nothing
            End If
        End If

        Sectors_Deallocate()
        SectorCount.X = CInt(Math.Ceiling(NewTerrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(NewTerrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.Horizontal.X < 0 _
                Or Units(A).Pos.Horizontal.X >= NewTerrain.TileSize.X * TerrainGridSpacing _
                Or Units(A).Pos.Horizontal.Y < 0 _
                Or Units(A).Pos.Horizontal.Y >= NewTerrain.TileSize.Y * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(Units(A))
                A += 1
            End If
        Loop
        A = 0
        Do While A < GatewayCount
            If Gateways(A).PosA.X < 0 _
                Or Gateways(A).PosA.X >= NewTerrain.TileSize.X _
                Or Gateways(A).PosA.Y < 0 _
                Or Gateways(A).PosA.Y >= NewTerrain.TileSize.Y _
                Or Gateways(A).PosB.X < 0 _
                Or Gateways(A).PosB.X >= NewTerrain.TileSize.X _
                Or Gateways(A).PosB.Y < 0 _
                Or Gateways(A).PosB.Y >= NewTerrain.TileSize.Y Then
                Gateway_Remove(A)
            Else
                A += 1
            End If
        Loop

        Terrain = NewTerrain

        AfterInitialized()
    End Sub

    Public Sub Sector_GLList_Make(ByVal X As Integer, ByVal Y As Integer)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim FinishX As Integer
        Dim FinishY As Integer
        Dim UnitNum As Integer

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        If Sectors(X, Y).GLList_Textured > 0 Then
            GL.DeleteLists(Sectors(X, Y).GLList_Textured, 1)
            Sectors(X, Y).GLList_Textured = 0
        End If
        If Sectors(X, Y).GLList_Wireframe > 0 Then
            GL.DeleteLists(Sectors(X, Y).GLList_Wireframe, 1)
            Sectors(X, Y).GLList_Wireframe = 0
        End If

        StartX = X * SectorTileSize
        StartY = Y * SectorTileSize
        FinishX = Math.Min(StartX + SectorTileSize, Terrain.TileSize.X) - 1
        FinishY = Math.Min(StartY + SectorTileSize, Terrain.TileSize.Y) - 1

        Sectors(X, Y).GLList_Textured = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Textured, ListMode.Compile)

        If frmMainInstance.View.Draw_Units Then
            Dim IsBasePlate(SectorTileSize - 1, SectorTileSize - 1) As Boolean
            Dim tmpUnit As clsUnit
            Dim BaseOffset As sXY_int
            Dim tmpStructure As clsStructureType
            Dim Footprint As sXY_int
            For UnitNum = 0 To Sectors(X, Y).UnitCount - 1
                tmpUnit = Sectors(X, Y).Units(UnitNum)
                If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(tmpUnit.Type, clsStructureType)
                    Footprint = tmpStructure.Footprint
                    If tmpStructure.StructureBasePlate IsNot Nothing And (tmpUnit.Rotation = 0 Or tmpUnit.Rotation = 180 Or (Footprint.X = Footprint.Y And (tmpUnit.Rotation = 90 Or tmpUnit.Rotation = 180 Or tmpUnit.Rotation = 270))) Then
                        BaseOffset.X = CInt((Footprint.X - 1) * TerrainGridSpacing / 2.0#) '1 is subtracted because centre of the edge-tiles are needed, not the edge of the base plate
                        BaseOffset.Y = CInt((Footprint.Y - 1) * TerrainGridSpacing / 2.0#)
                        For TileY = Math.Max(CInt(Int((tmpUnit.Pos.Horizontal.Y - BaseOffset.Y) / TerrainGridSpacing)), StartY) To Math.Min(CInt(Int((tmpUnit.Pos.Horizontal.Y + BaseOffset.Y) / TerrainGridSpacing)), FinishY)
                            For TileX = Math.Max(CInt(Int((tmpUnit.Pos.Horizontal.X - BaseOffset.X) / TerrainGridSpacing)), StartX) To Math.Min(CInt(Int((tmpUnit.Pos.Horizontal.X + BaseOffset.X) / TerrainGridSpacing)), FinishX)
                                IsBasePlate(TileX - StartX, TileY - StartY) = True
                            Next
                        Next
                    End If
                End If
            Next
            For TileY = StartY To FinishY
                For TileX = StartX To FinishX
                    If Not IsBasePlate(TileX - StartX, TileY - StartY) Then
                        DrawTile(TileX, TileY)
                    End If
                Next
            Next
        Else
            For TileY = StartY To FinishY
                For TileX = StartX To FinishX
                    DrawTile(TileX, TileY)
                Next
            Next
        End If

        GL.EndList()

        Sectors(X, Y).GLList_Wireframe = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Wireframe, ListMode.Compile)

        For TileY = StartY To FinishY
            For TileX = StartX To FinishX
                DrawTileWireframe(TileX, TileY)
            Next
        Next

        GL.EndList()
    End Sub

    Public Sub DrawTileWireframe(ByVal TileX As Integer, ByVal TileY As Integer)
        Dim TileTerrainHeight(3) As Double
        Dim Vertex0 As sXYZ_sng
        Dim Vertex1 As sXYZ_sng
        Dim Vertex2 As sXYZ_sng
        Dim Vertex3 As sXYZ_sng

        TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
        TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
        TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
        TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

        Vertex0.X = CSng(TileX * TerrainGridSpacing)
        Vertex0.Y = CSng(TileTerrainHeight(0) * HeightMultiplier)
        Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex1.Y = CSng(TileTerrainHeight(1) * HeightMultiplier)
        Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex2.X = CSng(TileX * TerrainGridSpacing)
        Vertex2.Y = CSng(TileTerrainHeight(2) * HeightMultiplier)
        Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
        Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex3.Y = CSng(TileTerrainHeight(3) * HeightMultiplier)
        Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

        GL.Begin(BeginMode.Lines)
        If Terrain.Tiles(TileX, TileY).Tri Then
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
        Else
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
        End If
        GL.End()
    End Sub

    Public Sub DrawTileOrientation(ByVal Tile As sXY_int)
        Dim TileOrientation As sTileOrientation
        Dim UnrotatedPos As sXY_int
        Dim Vertex0 As sWorldPos
        Dim Vertex1 As sWorldPos
        Dim Vertex2 As sWorldPos

        TileOrientation = Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation

        UnrotatedPos.X = 32
        UnrotatedPos.Y = 32
        Vertex0 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 32
        Vertex1 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 64
        Vertex2 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        GL.Vertex3(Vertex0.Horizontal.X, Vertex0.Altitude, Vertex0.Horizontal.Y)
        GL.Vertex3(Vertex1.Horizontal.X, Vertex1.Altitude, Vertex1.Horizontal.Y)
        GL.Vertex3(Vertex2.Horizontal.X, Vertex2.Altitude, Vertex2.Horizontal.Y)
    End Sub

    Public Sub DrawTile(ByVal TileX As Integer, ByVal TileY As Integer)
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
            A = Tileset.Tiles(Terrain.Tiles(TileX, TileY).Texture.TextureNum).MapView_GL_Texture_Num
            If A = 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            Else
                GL.BindTexture(TextureTarget.Texture2D, A)
            End If
        Else
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        End If

        TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
        TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
        TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
        TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

        GetTileRotatedTexCoords(Terrain.Tiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

        Vertex0.X = CSng(TileX * TerrainGridSpacing)
        Vertex0.Y = CSng(TileTerrainHeight(0) * HeightMultiplier)
        Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex1.Y = CSng(TileTerrainHeight(1) * HeightMultiplier)
        Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex2.X = CSng(TileX * TerrainGridSpacing)
        Vertex2.Y = CSng(TileTerrainHeight(2) * HeightMultiplier)
        Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
        Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex3.Y = CSng(TileTerrainHeight(3) * HeightMultiplier)
        Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

        Normal0 = TerrainVertexNormalCalc(TileX, TileY)
        Normal1 = TerrainVertexNormalCalc(TileX + 1, TileY)
        Normal2 = TerrainVertexNormalCalc(TileX, TileY + 1)
        Normal3 = TerrainVertexNormalCalc(TileX + 1, TileY + 1)

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

    Public Class clsInterfaceOptions
        Public CompileName As String
        Public CompileMultiPlayers As String
        Public CompileMultiXPlayers As Boolean
        Public CompileMultiAuthor As String
        Public CompileMultiLicense As String
        Public AutoScrollLimits As Boolean
        Public ScrollMin As sXY_int
        Public ScrollMax As sXY_uint
        Public CampaignGameTime As String
        Public CampaignGameType As Integer

        Public Sub New()

            'set to default
            CompileName = ""
            CompileMultiPlayers = CStr(2)
            CompileMultiXPlayers = False
            CompileMultiAuthor = ""
            CompileMultiLicense = ""
            AutoScrollLimits = True
            ScrollMin.X = 0
            ScrollMin.Y = 0
            ScrollMax.X = 0UI
            ScrollMax.Y = 0UI
            CampaignGameTime = CStr(2)
            CampaignGameType = -1
        End Sub
    End Class

    Public Class clsMinimapTexture
        Public Pixels(,,) As Byte

        Public Sub New(ByVal Size As sXY_int)

            ReDim Pixels(Size.Y - 1, Size.X - 1, 3)
        End Sub
    End Class

    Private Sub MinimapTextureFill(ByVal Texture As clsMinimapTexture)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim Low As sXY_int
        Dim High As sXY_int
        Dim Footprint As sXY_int
        Dim Flag As Boolean
        Dim RGB_sng As sRGB_sng
        Dim UnitMap(Texture.Pixels.GetUpperBound(0), Texture.Pixels.GetUpperBound(1)) As Boolean

        For Y = 0 To Texture.Pixels.GetUpperBound(0)
            For X = 0 To Texture.Pixels.GetUpperBound(1)
                Texture.Pixels(Y, X, 3) = 255
            Next
        Next
        If frmMainInstance.menuMiniShowTex.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            RGB_sng = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Average_Color
                            Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt(RGB_sng.Red * 255.0F), 255))
                            Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt(RGB_sng.Green * 255.0F), 255))
                            Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt(RGB_sng.Blue * 255.0F), 255))
                        End If
                    Next
                Next
            End If
            If frmMainInstance.menuMiniShowHeight.Checked Then
                Dim Height As Short
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        Height = CShort((CShort(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 4.0F)
                        Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt((Texture.Pixels(Y, X, 0) * 2S + Height) / 3.0F), 255))
                        Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt((Texture.Pixels(Y, X, 1) * 2S + Height) / 3.0F), 255))
                        Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt((Texture.Pixels(Y, X, 2) * 2S + Height) / 3.0F), 255))
                    Next
                Next
            End If
        ElseIf frmMainInstance.menuMiniShowHeight.Checked Then
            Dim Height As Byte
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Height = CByte((CShort(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 4.0F)
                    Texture.Pixels(Y, X, 0) = Height
                    Texture.Pixels(Y, X, 1) = Height
                    Texture.Pixels(Y, X, 2) = Height
                Next
            Next
        Else
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Texture.Pixels(Y, X, 0) = CByte(0)
                    Texture.Pixels(Y, X, 1) = CByte(0)
                    Texture.Pixels(Y, X, 2) = CByte(0)
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowCliffs.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            If Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Then
                                Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 0)) + 255S) / 2.0F), 255))
                            End If
                        End If
                    Next
                Next
            End If
        End If
        If frmMainInstance.menuMiniShowGateways.Checked Then
            For A = 0 To GatewayCount - 1
                XY_Reorder(Gateways(A).PosA, Gateways(A).PosB, Low, High)
                For Y = Low.Y To High.Y
                    For X = Low.X To High.X
                        Texture.Pixels(Y, X, 0) = CByte(255)
                        Texture.Pixels(Y, X, 1) = CByte(255)
                        Texture.Pixels(Y, X, 2) = CByte(0)
                    Next
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowUnits.Checked Then
            'units that are not selected
            For A = 0 To UnitCount - 1
                Flag = True
                If frmMainInstance.SelectedObjectType Is Units(A).Type Then
                    Flag = False
                Else
                    Footprint = Units(A).Type.GetFootprint
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Units(A).Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                If Settings.MinimapTeamColours Then
                                    If Settings.MinimapTeamColoursExceptFeatures And Units(A).Type.Type = clsUnitType.enumType.Feature Then
                                        Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt(MinimapFeatureColour.Red * 255.0F), 255))
                                        Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt(MinimapFeatureColour.Green * 255.0F), 255))
                                        Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt(MinimapFeatureColour.Blue * 255.0F), 255))
                                    Else
                                        RGB_sng = GetUnitGroupMinimapColour(Units(A).UnitGroup)
                                        Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt(RGB_sng.Red * 255.0F), 255))
                                        Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt(RGB_sng.Green * 255.0F), 255))
                                        Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt(RGB_sng.Blue * 255.0F), 255))
                                    End If
                                Else
                                    Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 0)) + 510S) / 3.0F), 255))
                                    Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 1)) + 0S) / 3.0F), 255))
                                    Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 2)) + 0S) / 3.0F), 255))
                                End If
                            End If
                        Next
                    Next
                End If
            Next
            'reset unit map
            For Y = 0 To Texture.Pixels.GetUpperBound(0)
                For X = 0 To Texture.Pixels.GetUpperBound(1)
                    UnitMap(Y, X) = False
                Next
            Next
            'units that are selected and highlighted
            For A = 0 To UnitCount - 1
                Flag = False
                If frmMainInstance.SelectedObjectType Is Units(A).Type Then
                    Flag = True
                    Footprint = Units(A).Type.GetFootprint
                    Footprint.X += 2
                    Footprint.Y += 2
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Units(A).Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                Texture.Pixels(Y, X, 0) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 0)) + 510S) / 3.0F), 255))
                                Texture.Pixels(Y, X, 1) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 1)) + 510S) / 3.0F), 255))
                                Texture.Pixels(Y, X, 2) = CByte(Math.Min(CInt((CShort(Texture.Pixels(Y, X, 2)) + 510S) / 3.0F), 255))
                            End If
                        Next
                    Next
                End If
            Next
        End If
    End Sub

#If Mono <> 0.0# Then
        Private MinimapBitmap As Bitmap
#End If
    Private MinimapPending As Boolean
    Private WithEvents MakeMinimapTimer As Timer
    Public SuppressMinimap As Boolean

    Private Sub MinimapTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles MakeMinimapTimer.Tick

        If MinimapPending Then
            If SuppressMinimap Then
                'should restart the timer here, but i cant find a good way to
            Else
                MinimapPending = False
                MinimapMake()
            End If
        Else
            MakeMinimapTimer.Enabled = False
        End If
    End Sub

    Public Sub MinimapMakeLater()

        If MakeMinimapTimer.Enabled Then
            MinimapPending = True
        Else
            MakeMinimapTimer.Enabled = True
            If SuppressMinimap Then
                MinimapPending = True
            Else
                MinimapMake()
            End If
        End If
    End Sub

    Public Sub MinimapMake()

        Dim NewTextureSize As Integer = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(Terrain.TileSize.X, Terrain.TileSize.Y)) / Math.Log(2.0#))))

        If NewTextureSize <> Minimap_Texture_Size Then
            Minimap_Texture_Size = NewTextureSize
#If Mono <> 0.0# Then
                MinimapBitmap = New Bitmap(Minimap_Texture_Size, Minimap_Texture_Size)
#End If
        End If

        Dim Texture As New clsMap.clsMinimapTexture(New sXY_int(Minimap_Texture_Size, Minimap_Texture_Size))

        MinimapTextureFill(Texture)

#If Mono <> 0.0# Then
        Dim TextureB As Bitmap

        TextureB = MinimapBitmap

        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Minimap_Texture_Size - 1
            For X = 0 To Minimap_Texture_Size - 1
                TextureB.SetPixel(X, Y, ColorTranslator.FromOle(OSRGB(Texture.Pixels(Y, X, 0), Texture.Pixels(Y, X, 1), Texture.Pixels(Y, X, 2))))
            Next
        Next
#End If

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        If Minimap_GLTexture > 0 Then
            GL.DeleteTextures(1, Minimap_GLTexture)
            Minimap_GLTexture = 0
        End If

#If Mono = 0.0# Then
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.GenTextures(1, Minimap_GLTexture)
        GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Texture.Pixels)
#Else
        Minimap_GLTexture = BitmapGLTexture(TextureB, frmMainInstance.View.OpenGLControl, False, False)
#End If

        frmMainInstance.View_DrawViewLater()
    End Sub

    Public Function Unit_Add_StoreChange(ByVal NewUnit As clsUnit) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Added
        UnitChanges(UnitChangeCount).Unit = NewUnit
        UnitChangeCount += 1

        ReturnResult = Unit_Add(NewUnit)
        Return ReturnResult
    End Function

    Public Function UnitID_Add_StoreChange(ByVal NewUnit As clsUnit, ByVal ID As UInteger) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Added
        UnitChanges(UnitChangeCount).Unit = NewUnit
        UnitChangeCount += 1

        ReturnResult = UnitID_Add(NewUnit, ID)
        Return ReturnResult
    End Function

    Public Function GetAvailableID() As UInteger
        Dim A As Integer
        Dim ID As UInteger

        ID = 1UI
        For A = 0 To UnitCount - 1
            If Units(A).ID >= ID Then
                ID = Units(A).ID + 1UI
            End If
        Next

        Return ID
    End Function

    Public Function Unit_Add(ByVal NewUnit As clsUnit) As Integer

        Return UnitID_Add(NewUnit, GetAvailableID)
    End Function

    Public Function UnitID_Add(ByVal NewUnit As clsUnit, ByVal ID As UInteger) As Integer
        Dim ReturnResult As Integer
        Dim A As Integer

        If NewUnit.UnitGroup Is Nothing Then
            MsgBox("Error: Added object has no group.")
            NewUnit.UnitGroup = ScavengerUnitGroup
            Return -1
        End If
        If NewUnit.UnitGroup.ParentMap IsNot Me Then
            MsgBox("Error: Something terrible happened.")
            Return -1
        End If

        If ID <= 0UI Then
            ID = 1UI
        End If

        For A = 0 To UnitCount - 1
            If ID = Units(A).ID Then
                Exit For
            End If
        Next
        If A < UnitCount Then
            ID = GetAvailableID()
        End If

        NewUnit.ID = ID

        NewUnit.Map_UnitNum = UnitCount

        NewUnit.Pos.Horizontal.X = Clamp_int(NewUnit.Pos.Horizontal.X, 0, Terrain.TileSize.X * TerrainGridSpacing - 1)
        NewUnit.Pos.Horizontal.Y = Clamp_int(NewUnit.Pos.Horizontal.Y, 0, Terrain.TileSize.Y * TerrainGridSpacing - 1)
        NewUnit.Pos.Altitude = CInt(Math.Ceiling(GetTerrainHeight(NewUnit.Pos.Horizontal)))

        If NewUnit.UnitGroup Is Nothing Then
            NewUnit.UnitGroup = ScavengerUnitGroup
        End If

        If Units.GetUpperBound(0) < UnitCount Then
            ReDim Preserve Units(UnitCount * 2 + 1)
        End If
        Units(UnitCount) = NewUnit
        ReturnResult = UnitCount
        UnitCount += 1

        Unit_Sectors_Calc(NewUnit)

        UnitSectors_GLList(NewUnit)

        Return ReturnResult
    End Function

    Public Sub Unit_Remove_StoreChange(ByVal Num As Integer)

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Deleted
        UnitChanges(UnitChangeCount).Unit = Units(Num)
        UnitChangeCount += 1

        Unit_Remove(Num)
    End Sub

    Public Sub Unit_Remove(ByVal Num As Integer)
        Dim A As Integer

        UnitSectors_GLList(Units(Num))

        Dim MouseOverTerrain As ctrlMapView.clsMouseOver.clsOverTerrain = frmMainInstance.View.GetMouseOverTerrain
        If MouseOverTerrain IsNot Nothing Then
            MouseOverTerrain.Unit_FindRemove(Units(Num))
        End If

        A = 0
        Do While A < SelectedUnitCount
            If SelectedUnits(A) Is Units(Num) Then
                SelectedUnit_Remove(A)
            Else
                A += 1
            End If
        Loop

        If Units(Num).SectorCount > 0 Then
            For A = 0 To Units(Num).SectorCount - 1
                Units(Num).Sectors(A).Unit_Remove(Units(Num).Sectors_UnitNum(A))
            Next
            Units(Num).Sectors_Remove()
        End If

        Units(Num).Map_UnitNum = -1

        UnitCount -= 1
        If Num < UnitCount Then
            Units(Num) = Units(UnitCount)
            Units(Num).Map_UnitNum = Num
        End If
        Units(UnitCount) = Nothing
        If Units.GetUpperBound(0) + 1 > UnitCount * 3 Then
            ReDim Preserve Units(UnitCount - 1)
        End If
    End Sub

    Public Function GetTileSectorNum(ByVal Tile As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Tile.X / SectorTileSize))
        Result.Y = CInt(Int(Tile.Y / SectorTileSize))

        Return Result
    End Function

    Public Sub GetTileSectorRange(ByVal StartTile As sXY_int, ByVal FinishTile As sXY_int, ByRef ResultSectorStart As sXY_int, ByRef ResultSectorFinish As sXY_int)

        ResultSectorStart = GetTileSectorNum(StartTile)
        ResultSectorFinish = GetTileSectorNum(FinishTile)
        ResultSectorStart.X = Clamp_int(ResultSectorStart.X, 0, SectorCount.X - 1)
        ResultSectorStart.Y = Clamp_int(ResultSectorStart.Y, 0, SectorCount.Y - 1)
        ResultSectorFinish.X = Clamp_int(ResultSectorFinish.X, 0, SectorCount.X - 1)
        ResultSectorFinish.Y = Clamp_int(ResultSectorFinish.Y, 0, SectorCount.Y - 1)
    End Sub

    Public Function TileAligned_Pos_From_MapPos(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = CInt((Math.Round((Horizontal.X - Footprint.X * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.X / 2.0#) * TerrainGridSpacing)
        Result.Horizontal.Y = CInt((Math.Round((Horizontal.Y - Footprint.Y * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.Y / 2.0#) * TerrainGridSpacing)
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub Unit_Sectors_Calc(ByVal Unit As clsUnit)
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim TileStart As sXY_int
        Dim TileFinish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer

        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.GetFootprint, TileStart, TileFinish)
        Start = GetTileSectorNum(TileStart)
        Finish = GetTileSectorNum(TileFinish)
        Start.X = Clamp_int(Start.X, 0, SectorCount.X - 1)
        Start.Y = Clamp_int(Start.Y, 0, SectorCount.Y - 1)
        Finish.X = Clamp_int(Finish.X, 0, SectorCount.X - 1)
        Finish.Y = Clamp_int(Finish.Y, 0, SectorCount.Y - 1)
        Unit.SectorCount = (Finish.X - Start.X + 1) * (Finish.Y - Start.Y + 1)
        ReDim Unit.Sectors(Unit.SectorCount - 1)
        ReDim Unit.Sectors_UnitNum(Unit.SectorCount - 1)
        A = 0
        For Y = Start.Y To Finish.Y
            For X = Start.X To Finish.X
                Unit.Sectors(A) = Sectors(X, Y)
                Unit.Sectors_UnitNum(A) = Sectors(X, Y).Unit_Add(Unit, A)
                A += 1
            Next
        Next
    End Sub

    Public Sub UndoStepCreate(ByVal StepName As String)
        Dim A As Integer
        Dim SectorNum As sXY_int
        Dim NewUndo As New clsUndo

        NewUndo.Name = StepName

        AutoSave.ChangeCount += 1UI
        AutoSave_Test()
        frmMainInstance.tsbSave.Enabled = True

        NewUndo.ChangedSectorCount = SectorTerrainUndoChanges.ChangedPointCount
        ReDim NewUndo.ChangedSectors(NewUndo.ChangedSectorCount - 1)
        For A = 0 To SectorTerrainUndoChanges.ChangedPointCount - 1
            SectorNum = SectorTerrainUndoChanges.ChangedPoints(A)
            NewUndo.ChangedSectors(A) = ShadowSectors(SectorNum.X, SectorNum.Y)
            ShadowSector_Create(SectorNum)
        Next
        SectorTerrainUndoChanges.Clear()

        ReDim NewUndo.UnitChanges(UnitChanges.GetUpperBound(0))
        For A = 0 To UnitChanges.GetUpperBound(0)
            NewUndo.UnitChanges(A) = UnitChanges(A)
        Next
        NewUndo.UnitChangeCount = UnitChangeCount

        UnitChangeCount = 0
        ReDim UnitChanges(-1)

        If NewUndo.ChangedSectorCount + NewUndo.UnitChangeCount > 0 Then
            UndoCount = Undo_Pos
            ReDim Preserve Undos(UndoCount - 1) 'a new line has been started so remove redos

            Undo_Append(NewUndo)
            Undo_Pos = UndoCount
        End If
    End Sub

    Public Sub ShadowSector_Create(ByVal SectorNum As sXY_int)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim tmpShadowSector As clsShadowSector
        Dim LastTileX As Integer
        Dim LastTileY As Integer

        tmpShadowSector = New clsShadowSector
        ShadowSectors(SectorNum.X, SectorNum.Y) = tmpShadowSector
        tmpShadowSector.Num = SectorNum
        StartX = SectorNum.X * SectorTileSize
        StartY = SectorNum.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileY = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartY)
        For Y = 0 To LastTileY
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.Vertices(X, Y).Height = Terrain.Vertices(TileX, TileY).Height
                tmpShadowSector.Terrain.Vertices(X, Y).Terrain = Terrain.Vertices(TileX, TileY).Terrain
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.Tiles(X, Y).Copy(Terrain.Tiles(TileX, TileY))
            Next
        Next
        For Y = 0 To LastTileY
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.SideH(X, Y).Road = Terrain.SideH(TileX, TileY).Road
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.SideV(X, Y).Road = Terrain.SideV(TileX, TileY).Road
            Next
        Next
    End Sub

    Public Sub ShadowSectors_CreateAll()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                ShadowSector_Create(New sXY_int(X, Y))
            Next
        Next
    End Sub

    Public Sub Undo_Clear()

        UndoCount = 0
        ReDim Undos(-1)
        Undo_Pos = UndoCount
        SectorTerrainUndoChanges.Clear()
    End Sub

    Public Sub Undo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Y As Integer
        Dim tmpUnit As clsUnit
        Dim ID As UInteger

        UndoStepCreate("Incomplete Action") 'make another redo step incase something has changed, such as if user presses undo while still dragging a tool

        Undo_Pos -= 1

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            X = Undos(Undo_Pos).ChangedSectors(A).Num.X
            Y = Undos(Undo_Pos).ChangedSectors(A).Num.Y
            'store existing state for redo
            tmpShadow = ShadowSectors(X, Y)
            'remove graphics from sector
            If Sectors(X, Y).GLList_Textured > 0 Then
                GL.DeleteLists(Sectors(X, Y).GLList_Textured, 1)
                Sectors(X, Y).GLList_Textured = 0
            End If
            If Sectors(X, Y).GLList_Wireframe > 0 Then
                GL.DeleteLists(Sectors(X, Y).GLList_Wireframe, 1)
                Sectors(X, Y).GLList_Wireframe = 0
            End If
            'perform the undo
            Undo_Sector_Rejoin(Undos(Undo_Pos).ChangedSectors(A))
            'update the backup
            ShadowSector_Create(New sXY_int(X, Y))
            'add old state to the redo step (that was this undo step)
            Undos(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            SectorGraphicsChanges.Changed(Undos(Undo_Pos).ChangedSectors(A).Num)
        Next

        For A = Undos(Undo_Pos).UnitChangeCount - 1 To 0 Step -1 'must do in reverse order, otherwise may try to delete units that havent been added yet
            Select Case Undos(Undo_Pos).UnitChanges(A).Type
                Case sUnitChange.enumType.Added
                    'remove the unit from the map
                    Unit_Remove(Undos(Undo_Pos).UnitChanges(A).Unit.Map_UnitNum)
                Case sUnitChange.enumType.Deleted
                    'add the unit back on to the map
                    tmpUnit = Undos(Undo_Pos).UnitChanges(A).Unit
                    ID = tmpUnit.ID
                    UnitID_Add(tmpUnit, ID)
                    ErrorIDChange(ID, tmpUnit, "Undo_Perform")
                Case Else
                    Stop
            End Select
        Next

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub Redo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Y As Integer
        Dim tmpUnit As clsUnit
        Dim ID As UInteger

        If GraphicsContext.CurrentContext IsNot frmMainInstance.View.OpenGLControl.Context Then
            frmMainInstance.View.OpenGLControl.MakeCurrent()
        End If

        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            X = Undos(Undo_Pos).ChangedSectors(A).Num.X
            Y = Undos(Undo_Pos).ChangedSectors(A).Num.Y
            'store existing state for undo
            tmpShadow = ShadowSectors(X, Y)
            'remove graphics from sector
            If Sectors(X, Y).GLList_Textured > 0 Then
                GL.DeleteLists(Sectors(X, Y).GLList_Textured, 1)
                Sectors(X, Y).GLList_Textured = 0
            End If
            If Sectors(X, Y).GLList_Wireframe > 0 Then
                GL.DeleteLists(Sectors(X, Y).GLList_Wireframe, 1)
                Sectors(X, Y).GLList_Wireframe = 0
            End If
            'perform the redo
            Undo_Sector_Rejoin(Undos(Undo_Pos).ChangedSectors(A))
            'update the backup
            ShadowSector_Create(New sXY_int(X, Y))
            'add old state to the undo step (that was this redo step)
            Undos(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            SectorGraphicsChanges.Changed(Undos(Undo_Pos).ChangedSectors(A).Num)
        Next

        For A = 0 To Undos(Undo_Pos).UnitChangeCount - 1
            Select Case Undos(Undo_Pos).UnitChanges(A).Type
                Case sUnitChange.enumType.Added
                    'add the unit back on to the map
                    tmpUnit = Undos(Undo_Pos).UnitChanges(A).Unit
                    ID = tmpUnit.ID
                    UnitID_Add(tmpUnit, ID)
                    ErrorIDChange(ID, tmpUnit, "Redo_Perform")
                Case sUnitChange.enumType.Deleted
                    'remove the unit from the map
                    Unit_Remove(Undos(Undo_Pos).UnitChanges(A).Unit.Map_UnitNum)
                Case Else
                    Stop
            End Select
        Next

        Undo_Pos += 1

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub Undo_Sector_Rejoin(ByVal Shadow_Sector_To_Rejoin As clsShadowSector)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        StartX = Shadow_Sector_To_Rejoin.Num.X * SectorTileSize
        StartZ = Shadow_Sector_To_Rejoin.Num.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartZ)
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Vertices(TileX, TileZ).Height = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Height
                Terrain.Vertices(TileX, TileZ).Terrain = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Terrain
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Tiles(TileX, TileZ).Copy(Shadow_Sector_To_Rejoin.Terrain.Tiles(X, Y))
            Next
        Next
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideH(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideH(X, Y).Road
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideV(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideV(X, Y).Road
            Next
        Next
    End Sub

    Public Function Undo_Append(ByVal NewUndo As clsUndo) As Integer
        Dim ReturnResult As Integer

        ReDim Preserve Undos(UndoCount)
        Undos(UndoCount) = NewUndo
        ReturnResult = UndoCount
        UndoCount += 1

        Return ReturnResult
    End Function

    Public Sub Undo_Insert(ByVal Pos As Integer, ByVal NewUndo As clsUndo)
        Dim A As Integer

        ReDim Preserve Undos(UndoCount)
        For A = UndoCount - 1 To Pos
            Undos(A + 1) = Undos(A)
        Next
        Undos(Pos) = NewUndo
        UndoCount += 1
    End Sub

    Public Sub Map_Insert(ByVal Map_To_Insert As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int, ByVal Insert_Heights As Boolean, ByVal Insert_Textures As Boolean, ByVal Insert_Units As Boolean, ByVal Delete_Units As Boolean, ByVal Insert_Gateways As Boolean, ByVal Delete_Gateways As Boolean)
        Dim Finish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim AreaAdjusted As sXY_int
        Dim SectorNum As sXY_int

        Finish.X = Math.Min(Offset.X + Math.Min(Area.X, Map_To_Insert.Terrain.TileSize.X), Terrain.TileSize.X)
        Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, Map_To_Insert.Terrain.TileSize.Y), Terrain.TileSize.Y)
        AreaAdjusted.X = Finish.X - Offset.X
        AreaAdjusted.Y = Finish.Y - Offset.Y

        GetTileSectorRange(New sXY_int(Offset.X - 1, Offset.Y - 1), Finish, SectorStart, SectorFinish)
        For Y = SectorStart.Y To SectorFinish.Y
            SectorNum.Y = Y
            For X = SectorStart.X To SectorFinish.X
                SectorNum.X = X
                SectorGraphicsChanges.Changed(SectorNum)
                SectorUnitHeightsChanges.Changed(SectorNum)
                SectorTerrainUndoChanges.Changed(SectorNum)
            Next
        Next

        If Insert_Heights Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height = Map_To_Insert.Terrain.Vertices(X, Y).Height
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = Map_To_Insert.Terrain.Tiles(X, Y).Tri
                Next
            Next
        End If
        If Insert_Textures Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain = Map_To_Insert.Terrain.Vertices(X, Y).Terrain
                Next
            Next
            Dim tmpTri As Boolean
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    tmpTri = Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Copy(Map_To_Insert.Terrain.Tiles(X, Y))
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = tmpTri
                Next
            Next
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.SideH(Offset.X + X, Offset.Y + Y).Road = Map_To_Insert.Terrain.SideH(X, Y).Road
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X
                    Terrain.SideV(Offset.X + X, Offset.Y + Y).Road = Map_To_Insert.Terrain.SideV(X, Y).Road
                Next
            Next
        End If

        If Delete_Gateways Then
            Dim A As Integer
            A = 0
            Do While A < GatewayCount
                If (Gateways(A).PosA.X >= Offset.X And Gateways(A).PosA.Y >= Offset.Y And _
                    Gateways(A).PosA.X < Offset.X + AreaAdjusted.X And Gateways(A).PosA.Y < Offset.Y + AreaAdjusted.Y) Or _
                    (Gateways(A).PosB.X >= Offset.X And Gateways(A).PosB.Y >= Offset.Y And _
                    Gateways(A).PosB.X < Offset.X + AreaAdjusted.X And Gateways(A).PosB.Y < Offset.Y + AreaAdjusted.Y) Then
                    Gateway_Remove(A)
                Else
                    A += 1
                End If
            Loop
        End If
        If Insert_Gateways Then
            Dim A As Integer
            Dim GateStart As sXY_int
            Dim GateFinish As sXY_int
            For A = 0 To Map_To_Insert.GatewayCount - 1
                GateStart.X = Offset.X + Map_To_Insert.Gateways(A).PosA.X
                GateStart.Y = Offset.Y + Map_To_Insert.Gateways(A).PosA.Y
                GateFinish.X = Offset.X + Map_To_Insert.Gateways(A).PosB.X
                GateFinish.Y = Offset.Y + Map_To_Insert.Gateways(A).PosB.Y
                If (GateStart.X >= Offset.X And GateStart.Y >= Offset.Y And _
                    GateStart.X < Offset.X + AreaAdjusted.X And GateStart.Y < Offset.Y + AreaAdjusted.Y) Or _
                    (GateFinish.X >= Offset.X And GateFinish.Y >= Offset.Y And _
                    GateFinish.X < Offset.X + AreaAdjusted.X And GateFinish.Y < Offset.Y + AreaAdjusted.Y) Then
                    Gateway_Add(GateStart, GateFinish)
                End If
            Next
        End If

        If Delete_Units Then
            Dim A As Integer
            Dim TempUnit As clsUnit
            Dim UnitsToDelete(-1) As clsUnit
            Dim UnitToDeleteCount As Integer = 0
            For Y = SectorStart.Y To SectorFinish.Y
                For X = SectorStart.X To SectorFinish.X
                    For A = 0 To Sectors(X, Y).UnitCount - 1
                        TempUnit = Sectors(X, Y).Units(A)
                        If PosIsWithinTileArea(TempUnit.Pos.Horizontal, Offset, Finish) Then
                            ReDim Preserve UnitsToDelete(UnitToDeleteCount)
                            UnitsToDelete(UnitToDeleteCount) = TempUnit
                            UnitToDeleteCount += 1
                        End If
                    Next
                Next
            Next
            For A = 0 To UnitToDeleteCount - 1
                If UnitsToDelete(A).Map_UnitNum >= 0 Then 'units may be in the list multiple times and already be deleted
                    Unit_Remove_StoreChange(UnitsToDelete(A).Map_UnitNum)
                End If
            Next
        End If
        If Insert_Units Then
            Dim PosDif As sXY_int
            Dim A As Integer
            Dim NewUnit As clsUnit
            Dim tmpUnit As clsUnit
            Dim ZeroPos As New sXY_int(0, 0)

            PosDif.X = Offset.X * TerrainGridSpacing
            PosDif.Y = Offset.Y * TerrainGridSpacing
            For A = 0 To Map_To_Insert.UnitCount - 1
                tmpUnit = Map_To_Insert.Units(A)
                If PosIsWithinTileArea(tmpUnit.Pos.Horizontal, ZeroPos, AreaAdjusted) Then
                    NewUnit = New clsUnit(Map_To_Insert.Units(A))
                    If NewUnit.UnitGroup.WZ_StartPos < 0 Then
                        NewUnit.UnitGroup = ScavengerUnitGroup
                    Else
                        NewUnit.UnitGroup = UnitGroups(NewUnit.UnitGroup.WZ_StartPos)
                    End If
                    NewUnit.Pos.Horizontal.X += PosDif.X
                    NewUnit.Pos.Horizontal.Y += PosDif.Y
                    Unit_Add_StoreChange(NewUnit)
                End If
            Next
        End If

        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
        MinimapMakeLater()
    End Sub

    Public Sub Gateway_Remove(ByVal Num As Integer)

        GatewayCount -= 1
        If Num <> GatewayCount Then
            Gateways(Num) = Gateways(GatewayCount)
        End If
        ReDim Preserve Gateways(GatewayCount - 1)
    End Sub

    Public Function Gateway_Add(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As Boolean

        If PosA.X >= 0 And PosA.X < Terrain.TileSize.X And _
            PosA.Y >= 0 And PosA.Y < Terrain.TileSize.Y And _
            PosB.X >= 0 And PosB.X < Terrain.TileSize.X And _
            PosB.Y >= 0 And PosB.Y < Terrain.TileSize.Y Then 'is on map
            If PosA.X = PosB.X Or PosA.Y = PosB.Y Then 'is straight

                ReDim Preserve Gateways(GatewayCount)
                Gateways(GatewayCount).PosA = PosA
                Gateways(GatewayCount).PosB = PosB
                GatewayCount += 1

                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Sub Sectors_Deallocate()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).Deallocate()
            Next
        Next
    End Sub

    Public Sub TileType_Reset()

        If Tileset Is Nothing Then
            ReDim Tile_TypeNum(-1)
        Else
            Dim A As Integer

            ReDim Tile_TypeNum(Tileset.TileCount - 1)
            For A = 0 To Tileset.TileCount - 1
                Tile_TypeNum(A) = Tileset.Tiles(A).Default_Type
            Next
        End If
    End Sub

    Public Sub AutoSave_Test()

        If Not Settings.AutoSaveEnabled Then
            Exit Sub
        End If
        If AutoSave.ChangeCount < Settings.AutoSave_MinChanges Then
            Exit Sub
        End If
        If DateDiff("s", AutoSave.SavedDate, Now) < Settings.AutoSave_MinInterval_s Then
            Exit Sub
        End If

        AutoSave.ChangeCount = 0UI
        AutoSave.SavedDate = Now

        AutoSave_Perform()
    End Sub

    Public Sub AutoSave_Perform()

        If Not IO.Directory.Exists(AutoSavePath) Then
            If Not IO.Directory.CreateDirectory(AutoSavePath).Exists Then
                MsgBox("Failed to create autosave directory during autosave.", CType(MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, MsgBoxStyle), "")
            End If
        End If

        Dim DateNow As Date = Now
        Dim Path As String

        Path = AutoSavePath & "autosaved-" & DateNow.Year & "-" & MinDigits(DateNow.Month, 2) & "-" & MinDigits(DateNow.Day, 2) & "-" & MinDigits(DateNow.Hour, 2) & "-" & MinDigits(DateNow.Minute, 2) & "-" & MinDigits(DateNow.Second, 2) & "-" & MinDigits(DateNow.Millisecond, 3) & ".fmap"

        Dim Result As clsResult = Write_FMap(Path, False, Settings.AutoSaveCompress)

        ShowWarnings(Result, "Autosave")
    End Sub

    Public Sub SelectedUnit_Add(ByVal NewSelectedUnit As clsUnit)

        If NewSelectedUnit.Map_SelectedUnitNum >= 0 Then
            Exit Sub
        End If

        NewSelectedUnit.Map_SelectedUnitNum = SelectedUnitCount

        ReDim Preserve SelectedUnits(SelectedUnitCount)
        SelectedUnits(SelectedUnitCount) = NewSelectedUnit
        SelectedUnitCount += 1
    End Sub

    Public Sub SelectedUnits_Add(ByVal NewSelectedUnits() As clsUnit)
        Dim A As Integer
        Dim Count As Integer

        ReDim Preserve SelectedUnits(SelectedUnitCount + NewSelectedUnits.GetUpperBound(0))

        Count = 0
        For A = 0 To NewSelectedUnits.GetUpperBound(0)
            If NewSelectedUnits(A).Map_SelectedUnitNum < 0 Then
                NewSelectedUnits(A).Map_SelectedUnitNum = SelectedUnitCount + Count
                SelectedUnits(SelectedUnitCount + Count) = NewSelectedUnits(A)
                Count += 1
            End If
        Next

        SelectedUnitCount += Count
        ReDim Preserve SelectedUnits(SelectedUnitCount - 1)
    End Sub

    Public Sub SelectedUnit_Remove(ByVal Num As Integer)

        SelectedUnits(Num).Map_SelectedUnitNum = -1

        SelectedUnitCount -= 1
        If Num <> SelectedUnitCount Then
            SelectedUnits(Num) = SelectedUnits(SelectedUnitCount)
            SelectedUnits(Num).Map_SelectedUnitNum = Num
        End If
        ReDim Preserve SelectedUnits(SelectedUnitCount - 1)
    End Sub

    Public Sub SelectedUnits_Clear()
        Dim A As Integer

        For A = 0 To SelectedUnitCount - 1
            SelectedUnits(A).Map_SelectedUnitNum = -1
        Next

        ReDim SelectedUnits(-1)
        SelectedUnitCount = 0
    End Sub

    Public Structure sUnitArray
        Public Units() As clsMap.clsUnit
    End Structure

    Public Sub SelectedUnits_Copy(ByRef NewArray As sUnitArray)
        Dim A As Integer

        ReDim NewArray.Units(SelectedUnitCount - 1)
        For A = 0 To SelectedUnitCount - 1
            NewArray.Units(A) = SelectedUnits(A)
        Next
    End Sub

    Public Sub SetPainterToDefaults()

        If Tileset Is Tileset_Arizona Then
            Painter = Painter_Arizona
        ElseIf Tileset Is Tileset_Urban Then
            Painter = Painter_Urban
        ElseIf Tileset Is Tileset_Rockies Then
            Painter = Painter_Rockies
        Else
            Painter = New clsPainter
        End If
    End Sub

    Private Sub UnitSectors_GLList(ByVal UnitToUpdateFor As clsUnit)

        If Not ReceivingUserChanges Then
            Exit Sub
        End If

        If SectorGraphicsChanges Is Nothing Then
            Stop
            Exit Sub
        End If

        Dim A As Integer

        For A = 0 To UnitToUpdateFor.SectorCount - 1
            SectorGraphicsChanges.Changed(UnitToUpdateFor.Sectors(A).Pos)
        Next
    End Sub

    Public Function GetTileOffsetRotatedWorldPos(ByVal Tile As sXY_int, ByVal TileOffsetToRotate As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Dim RotatedOffset As sXY_int

        RotatedOffset = GetTileRotatedOffset(Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation, TileOffsetToRotate)
        Result.Horizontal.X = Tile.X * TerrainGridSpacing + RotatedOffset.X
        Result.Horizontal.Y = Tile.Y * TerrainGridSpacing + RotatedOffset.Y
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub GetFootprintTileRangeClamped(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        ResultStart.X = Clamp_int(CInt(Int(Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.25#)), 0, Terrain.TileSize.X - 1)
        ResultStart.Y = Clamp_int(CInt(Int(Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.25#)), 0, Terrain.TileSize.Y - 1)
        ResultFinish.X = Clamp_int(CInt(Int(Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.25#)), 0, Terrain.TileSize.X - 1)
        ResultFinish.Y = Clamp_int(CInt(Int(Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.25#)), 0, Terrain.TileSize.Y - 1)
    End Sub

    Public Sub GetFootprintTileRange(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        ResultStart.X = CInt(Int(Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.25#))
        ResultStart.Y = CInt(Int(Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.25#))
        ResultFinish.X = CInt(Int(Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.25#))
        ResultFinish.Y = CInt(Int(Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.25#))
    End Sub

    Public Function GetPosTileNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Horizontal.X / TerrainGridSpacing))
        Result.Y = CInt(Int(Horizontal.Y / TerrainGridSpacing))

        Return Result
    End Function

    Public Function GetPosSectorNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result = GetTileSectorNum(GetPosTileNum(Horizontal))

        Return Result
    End Function

    Public Function GetSectorNumClamped(ByVal SectorNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(SectorNum.X, 0, SectorCount.X - 1)
        Result.Y = Clamp_int(SectorNum.Y, 0, SectorCount.Y - 1)

        Return Result
    End Function

    Public Function GetVertexAltitude(ByVal VertexNum As sXY_int) As Integer

        Return Terrain.Vertices(VertexNum.X, VertexNum.Y).Height * HeightMultiplier
    End Function

    Public Function PosIsOnMap(ByVal Horizontal As sXY_int) As Boolean

        Return PosIsWithinTileArea(Horizontal, New sXY_int(0, 0), Terrain.TileSize)
    End Function

    Public Function TileNumClampToMap(ByVal TileNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(TileNum.X, 0, Terrain.TileSize.X - 1)
        Result.Y = Clamp_int(TileNum.Y, 0, Terrain.TileSize.Y - 1)

        Return Result
    End Function

    Public Sub MakeDefaultUnitGroups()
        Dim A As Integer

        ReDim UnitGroups(PlayerCountMax - 1)
        For A = 0 To PlayerCountMax - 1
            UnitGroups(A) = New clsUnitGroup
            UnitGroups(A).ParentMap = Me
            UnitGroups(A).Map_UnitGroupNum = A
            UnitGroups(A).WZ_StartPos = A
        Next
        ScavengerUnitGroup = New clsUnitGroup
        ScavengerUnitGroup.ParentMap = Me
        FeatureUnitGroup = New clsUnitGroup
        FeatureUnitGroup.ParentMap = Me
        FeatureUnitGroup.WZ_StartPos = 0
    End Sub

    Public Function GetUnitGroupColour(ByVal ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup Is ScavengerUnitGroup Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        ElseIf ColourUnitGroup Is FeatureUnitGroup Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).Colour
        End If
    End Function

    Public Function GetUnitGroupMinimapColour(ByVal ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup Is ScavengerUnitGroup Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        ElseIf ColourUnitGroup Is FeatureUnitGroup Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).MinimapColour
        End If
    End Function

    Public Sub AfterInitialized()

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSectors_CreateAll()

        If SectorGraphicsChanges IsNot Nothing Then
            SectorGraphicsChanges.Deallocate()
        End If
        SectorGraphicsChanges = New clsSectorChanges(Me)

        If SectorUnitHeightsChanges IsNot Nothing Then
            SectorUnitHeightsChanges.Deallocate()
        End If
        SectorUnitHeightsChanges = New clsSectorChanges(Me)

        If SectorTerrainUndoChanges IsNot Nothing Then
            SectorTerrainUndoChanges.Deallocate()
        End If
        SectorTerrainUndoChanges = New clsSectorChanges(Me)

        If AutoTextureChanges IsNot Nothing Then
            AutoTextureChanges.Deallocate()
        End If
        AutoTextureChanges = New clsAutoTextureChanges(Me)

        If TerrainInterpretChanges IsNot Nothing Then
            TerrainInterpretChanges.Deallocate()
        End If
        TerrainInterpretChanges = New clsMap.clsTerrainUpdate(Terrain.TileSize)

        Undo_Clear()

        ReceivingUserChanges = True
    End Sub

    Public Function GetDirectory() As String

        If PathInfo Is Nothing Then
            Return My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            Dim SplitPath As New sSplitPath(Main_Map.PathInfo.Path)
            Return SplitPath.FilePath
        End If
    End Function

    Public Class clsUpdateSectorGraphics
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            Map.Sector_GLList_Make(PosNum.X, PosNum.Y)
            Map.MinimapMakeLater()
        End Sub
    End Class

    Public Sub Update()

        UpdateAutoTextures()
        TerrainInterpretUpdate()
        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
    End Sub

    Public Sub SectorsUpdateUnitHeights()
        Dim UpdateSectorUnitHeights As New clsUpdateSectorUnitHeights
        UpdateSectorUnitHeights.Map = Me

        UpdateSectorUnitHeights.Start()
        SectorUnitHeightsChanges.PerformTool(UpdateSectorUnitHeights)
        UpdateSectorUnitHeights.Finish()
        SectorUnitHeightsChanges.Clear()
    End Sub

    Public Sub SectorsUpdateGraphics()
        Dim UpdateSectorGraphics As New clsUpdateSectorGraphics
        UpdateSectorGraphics.Map = Me

        SectorGraphicsChanges.PerformTool(UpdateSectorGraphics)
        SectorGraphicsChanges.Clear()
    End Sub

    Public Sub UpdateAutoTextures()
        Dim UpdateAutotextures As New clsUpdateAutotexture
        UpdateAutotextures.Map = Me
        UpdateAutotextures.MakeInvalidTiles = frmMainInstance.cbxInvalidTiles.Checked

        AutoTextureChanges.PerformTool(UpdateAutotextures)
        AutoTextureChanges.Clear()
    End Sub

    Public Sub TerrainInterpretUpdate()
        Dim ApplyVertexInterpret As New clsMap.clsAction.clsApplyVertexTerrainInterpret
        Dim ApplyTileInterpret As New clsMap.clsAction.clsApplyTileTerrainInterpret
        Dim ApplySideHInterpret As New clsMap.clsAction.clsApplySideHTerrainInterpret
        Dim ApplySideVInterpret As New clsMap.clsAction.clsApplySideVTerrainInterpret
        ApplyVertexInterpret.Map = Me
        ApplyTileInterpret.Map = Me
        ApplySideHInterpret.Map = Me
        ApplySideVInterpret.Map = Me

        TerrainInterpretChanges.Vertices.PerformTool(ApplyVertexInterpret)
        TerrainInterpretChanges.Tiles.PerformTool(ApplyTileInterpret)
        TerrainInterpretChanges.SidesH.PerformTool(ApplySideHInterpret)
        TerrainInterpretChanges.SidesV.PerformTool(ApplySideVInterpret)
        TerrainInterpretChanges.ClearAll()
    End Sub

    Public Class clsUpdateSectorUnitHeights
        Inherits clsMap.clsAction

        Private B As Integer
        Private C As Integer
        Private NewUnit As clsUnit
        Private ID As UInteger
        Private tmpUnit As clsUnit
        Private OldUnits() As clsUnit
        Private OldUnitCount As Integer = 0
        Private NewAltitude As Integer
        Private Started As Boolean
        Private tmpSector As clsMap.clsSector

        Public Sub Start()

            ReDim OldUnits(Map.UnitCount - 1)

            Started = True
        End Sub

        Public Sub Finish()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer

            For A = 0 To OldUnitCount - 1
                tmpUnit = OldUnits(A)
                NewAltitude = CInt(Map.GetTerrainHeight(tmpUnit.Pos.Horizontal))
                If NewAltitude <> tmpUnit.Pos.Altitude Then
                    NewUnit = New clsUnit(tmpUnit)
                    ID = tmpUnit.ID
                    'NewUnit.Pos.Altitude = NewAltitude
                    'these create changed sectors and must be done before drawing the new sectors
                    Map.Unit_Remove_StoreChange(tmpUnit.Map_UnitNum)
                    Map.UnitID_Add_StoreChange(NewUnit, ID)
                    ErrorIDChange(ID, NewUnit, "UpdateSectorUnitHeights")
                End If
            Next

            Started = False
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            tmpSector = Map.Sectors(PosNum.X, PosNum.Y)
            For Me.B = 0 To tmpSector.UnitCount - 1
                tmpUnit = tmpSector.Units(B)
                'units can be in multiple sectors, so dont include multiple times
                For Me.C = 0 To OldUnitCount - 1
                    If OldUnits(C) Is tmpUnit Then
                        Exit For
                    End If
                Next
                If C = OldUnitCount Then
                    OldUnits(OldUnitCount) = tmpUnit
                    OldUnitCount += 1
                End If
            Next
        End Sub
    End Class

    Public Class clsUpdateAutotexture
        Inherits clsMap.clsAction

        Public MakeInvalidTiles As Boolean

        Private Terrain_Inner As clsPainter.clsTerrain
        Private Terrain_Outer As clsPainter.clsTerrain
        Private Road As clsPainter.clsRoad
        Private A As Integer
        Private Brush_Num As Integer
        Private RoadTop As Boolean
        Private RoadLeft As Boolean
        Private RoadRight As Boolean
        Private RoadBottom As Boolean
        Private Painter As clsPainter
        Private Terrain As clsTerrain
        Private ResultTiles As clsPainter.clsTileList
        Private ResultDirection As sTileDirection
        Private ResultTexture As clsPainter.clsTileList.sTile_Orientation_Chance

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Painter = Map.Painter

            ResultTiles = Nothing
            ResultDirection = TileDirection_None

            'apply centre brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For Me.Brush_Num = 0 To Painter.TerrainCount - 1
                    Terrain_Inner = Painter.Terrains(Brush_Num)
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    ResultTiles = Terrain_Inner.Tiles
                                    ResultDirection = TileDirection_None
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'apply transition brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For Me.Brush_Num = 0 To Painter.TransitionBrushCount - 1
                    Terrain_Inner = Painter.TransitionBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.TransitionBrushes(Brush_Num).Terrain_Outer
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    'nothing to do here
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Bottom
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Right
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o o i
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        End If
                    ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i i o
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Left
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Top
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o o o
                                    'nothing to do here
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'set cliff tiles
            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                                A = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                    For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomRight
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopLeft
                                Exit For
                            End If
                        End If
                    Next
                    If Brush_Num = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            Else
                'default tri orientation
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                    For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomLeft
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopRight
                                Exit For
                            End If
                        End If
                    Next
                    If Brush_Num = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            End If

            'apply roads
            Road = Nothing
            If Terrain.SideH(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y).Road
            ElseIf Terrain.SideH(PosNum.X, PosNum.Y + 1).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y + 1).Road
            ElseIf Terrain.SideV(PosNum.X + 1, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X + 1, PosNum.Y).Road
            ElseIf Terrain.SideV(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X, PosNum.Y).Road
            End If
            If Road IsNot Nothing Then
                For Me.Brush_Num = 0 To Painter.RoadBrushCount - 1
                    If Painter.RoadBrushes(Brush_Num).Road Is Road Then
                        Terrain_Outer = Painter.RoadBrushes(Brush_Num).Terrain
                        A = 0
                        If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If A >= 2 Then Exit For
                    End If
                Next

                ResultTiles = Nothing
                ResultDirection = TileDirection_None

                If Brush_Num < Painter.RoadBrushCount Then
                    RoadTop = (Terrain.SideH(PosNum.X, PosNum.Y).Road Is Road)
                    RoadLeft = (Terrain.SideV(PosNum.X, PosNum.Y).Road Is Road)
                    RoadRight = (Terrain.SideV(PosNum.X + 1, PosNum.Y).Road Is Road)
                    RoadBottom = (Terrain.SideH(PosNum.X, PosNum.Y + 1).Road Is Road)
                    'do cross intersection
                    If RoadTop And RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_CrossIntersection
                        ResultDirection = TileDirection_None
                        'do T intersection
                    ElseIf RoadTop And RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Top
                    ElseIf RoadTop And RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Left
                    ElseIf RoadTop And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Right
                    ElseIf RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Bottom
                        'do straight
                    ElseIf RoadTop And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Top
                        Else
                            ResultDirection = TileDirection_Bottom
                        End If
                    ElseIf RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Left
                        Else
                            ResultDirection = TileDirection_Right
                        End If
                        'do corner
                    ElseIf RoadTop And RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_TopLeft
                    ElseIf RoadTop And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_TopRight
                    ElseIf RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_BottomLeft
                    ElseIf RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_BottomRight
                        'do end
                    ElseIf RoadTop Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Top
                    ElseIf RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Left
                    ElseIf RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Right
                    ElseIf RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Bottom
                    End If
                End If
            End If

            If ResultTiles Is Nothing Then
                ResultTexture.TextureNum = -1
                ResultTexture.Direction = TileDirection_None
            Else
                ResultTexture = ResultTiles.GetRandom()
            End If
            If ResultTexture.TextureNum < 0 Then
                If MakeInvalidTiles Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
                End If
            Else
                Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
            End If

            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Sub TileNeedsInterpreting(ByVal Pos As sXY_int)

        TerrainInterpretChanges.Tiles.Changed(Pos)
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y + 1))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X + 1, Pos.Y))
    End Sub

    Public Sub TileTextureChangeTerrainAction(ByVal Pos As sXY_int, ByVal Action As enumTextureTerrainAction)

        Select Case Action
            Case enumTextureTerrainAction.Ignore

            Case enumTextureTerrainAction.Reinterpret
                TileNeedsInterpreting(Pos)
            Case enumTextureTerrainAction.Remove
                Terrain.Vertices(Pos.X, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X, Pos.Y + 1).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y + 1).Terrain = Nothing
        End Select
    End Sub

    Public Class clsDrawTileOutline
        Inherits clsMap.clsAction

        Public Colour As sRGBA_sng

        Private Vertex0 As sXYZ_int
        Private Vertex1 As sXYZ_int
        Private Vertex2 As sXYZ_int
        Private Vertex3 As sXYZ_int

        Public Overrides Sub ActionPerform()

            Vertex0.X = PosNum.X * TerrainGridSpacing
            Vertex0.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y).Height * Map.HeightMultiplier
            Vertex0.Z = -PosNum.Y * TerrainGridSpacing
            Vertex1.X = (PosNum.X + 1) * TerrainGridSpacing
            Vertex1.Y = Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y).Height * Map.HeightMultiplier
            Vertex1.Z = -PosNum.Y * TerrainGridSpacing
            Vertex2.X = PosNum.X * TerrainGridSpacing
            Vertex2.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y + 1).Height * Map.HeightMultiplier
            Vertex2.Z = -(PosNum.Y + 1) * TerrainGridSpacing
            Vertex3.X = (PosNum.X + 1) * TerrainGridSpacing
            Vertex3.Y = Map.Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Height * Map.HeightMultiplier
            Vertex3.Z = -(PosNum.Y + 1) * TerrainGridSpacing
            GL.Begin(BeginMode.LineLoop)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.End()
        End Sub
    End Class

    Public Class clsDrawCallTerrain
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            GL.CallList(Map.Sectors(PosNum.X, PosNum.Y).GLList_Textured)
        End Sub
    End Class

    Public Class clsDrawCallTerrainWireframe
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            GL.CallList(Map.Sectors(PosNum.X, PosNum.Y).GLList_Wireframe)
        End Sub
    End Class

    Public Class clsDrawTileOrientation
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()
            Dim X As Integer
            Dim Y As Integer

            For Y = PosNum.Y * SectorTileSize To Math.Min((PosNum.Y + 1) * SectorTileSize - 1, Map.Terrain.TileSize.Y - 1)
                For X = PosNum.X * SectorTileSize To Math.Min((PosNum.X + 1) * SectorTileSize - 1, Map.Terrain.TileSize.X - 1)
                    Map.DrawTileOrientation(New sXY_int(X, Y))
                Next
            Next
        End Sub
    End Class

    Public Class clsDrawVertexTerrain
        Inherits clsMap.clsAction

        Public ViewAngleMatrix As Matrix3D.Matrix3D

        Private RGB_sng As sRGB_sng
        Private RGB_sng2 As sRGB_sng
        Private XYZ_dbl As Matrix3D.XYZ_dbl
        Private XYZ_dbl2 As Matrix3D.XYZ_dbl
        Private XYZ_dbl3 As Matrix3D.XYZ_dbl
        Private Vertex0 As Matrix3D.XYZ_dbl
        Private Vertex1 As Matrix3D.XYZ_dbl
        Private Vertex2 As Matrix3D.XYZ_dbl
        Private Vertex3 As Matrix3D.XYZ_dbl

        Public Overrides Sub ActionPerform()
            Dim X As Integer
            Dim Y As Integer
            Dim A As Integer

            For Y = PosNum.Y * SectorTileSize To Math.Min((PosNum.Y + 1) * SectorTileSize - 1, Map.Terrain.TileSize.Y - 1)
                For X = PosNum.X * SectorTileSize To Math.Min((PosNum.X + 1) * SectorTileSize - 1, Map.Terrain.TileSize.X - 1)
                    If Map.Terrain.Vertices(X, Y).Terrain IsNot Nothing Then
                        A = Map.Terrain.Vertices(X, Y).Terrain.Num
                        If A < Map.Painter.TerrainCount Then
                            If Map.Painter.Terrains(A).Tiles.TileCount >= 1 Then
                                RGB_sng = Map.Tileset.Tiles(Map.Painter.Terrains(A).Tiles.Tiles(0).TextureNum).Average_Color
                                If RGB_sng.Red + RGB_sng.Green + RGB_sng.Blue < 1.5F Then
                                    RGB_sng2.Red = (RGB_sng.Red + 1.0F) / 2.0F
                                    RGB_sng2.Green = (RGB_sng.Green + 1.0F) / 2.0F
                                    RGB_sng2.Blue = (RGB_sng.Blue + 1.0F) / 2.0F
                                Else
                                    RGB_sng2.Red = RGB_sng.Red / 2.0F
                                    RGB_sng2.Green = RGB_sng.Green / 2.0F
                                    RGB_sng2.Blue = RGB_sng.Blue / 2.0F
                                End If
                                XYZ_dbl.X = X * TerrainGridSpacing
                                XYZ_dbl.Y = Map.Terrain.Vertices(X, Y).Height * Map.HeightMultiplier
                                XYZ_dbl.Z = -Y * TerrainGridSpacing
                                XYZ_dbl2.X = 10.0#
                                XYZ_dbl2.Y = 10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex0.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex0.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex0.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = -10.0#
                                XYZ_dbl2.Y = 10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex1.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex1.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex1.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = -10.0#
                                XYZ_dbl2.Y = -10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex2.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex2.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex2.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                XYZ_dbl2.X = 10.0#
                                XYZ_dbl2.Y = -10.0#
                                XYZ_dbl2.Z = 0.0#
                                Matrix3D.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, XYZ_dbl3)
                                Vertex3.X = XYZ_dbl.X + XYZ_dbl3.X
                                Vertex3.Y = XYZ_dbl.Y + XYZ_dbl3.Y
                                Vertex3.Z = XYZ_dbl.Z + XYZ_dbl3.Z
                                GL.Begin(BeginMode.Quads)
                                GL.Color3(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue)
                                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                                GL.End()
                                GL.Begin(BeginMode.LineLoop)
                                GL.Color3(RGB_sng2.Red, RGB_sng2.Green, RGB_sng2.Blue)
                                GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
                                GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
                                GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
                                GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
                                GL.End()
                            End If
                        End If
                    End If
                Next
            Next
        End Sub
    End Class

    Public Class clsDrawVertexMarker
        Inherits clsMap.clsAction

        Public Colour As sRGBA_sng

        Private Vertex0 As sXYZ_int

        Public Overrides Sub ActionPerform()

            Vertex0.X = PosNum.X * TerrainGridSpacing
            Vertex0.Y = Map.Terrain.Vertices(PosNum.X, PosNum.Y).Height * Map.HeightMultiplier
            Vertex0.Z = -PosNum.Y * TerrainGridSpacing
            GL.Begin(BeginMode.Lines)
            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha)
            GL.Vertex3(Vertex0.X - 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X + 8, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z - 8)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z + 8)
            GL.End()
        End Sub
    End Class

    Public Class clsDrawSectorObjects
        Inherits clsMap.clsAction

        Public MapView As ctrlMapView
        Public UnitTextLabels As clsTextLabels

        Private TextLabel As clsTextLabel
        Private UnitDrawn() As Boolean
        Private XYZ_dbl As Matrix3D.XYZ_dbl
        Private XYZ_dbl2 As Matrix3D.XYZ_dbl
        Private ScreenPos As sXY_int
        Private Started As Boolean

        Public Sub Start()

            ReDim UnitDrawn(Map.UnitCount - 1)

            Started = True
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer
            Dim tmpUnit As clsUnit
            Dim tmpSector As clsSector = Map.Sectors(PosNum.X, PosNum.Y)
            Dim DrawUnitLabel As Boolean
            Dim MouseOverTerrain As ctrlMapView.clsMouseOver.clsOverTerrain = MapView.GetMouseOverTerrain

            For A = 0 To tmpSector.UnitCount - 1
                tmpUnit = tmpSector.Units(A)
                If Not UnitDrawn(tmpUnit.Map_UnitNum) Then
                    UnitDrawn(tmpUnit.Map_UnitNum) = True
                    XYZ_dbl.X = tmpUnit.Pos.Horizontal.X - MapView.ViewPos.X
                    XYZ_dbl.Y = tmpUnit.Pos.Altitude - MapView.ViewPos.Y
                    XYZ_dbl.Z = -tmpUnit.Pos.Horizontal.Y - MapView.ViewPos.Z
                    DrawUnitLabel = False
                    If tmpUnit.Type.IsUnknown Then
                        DrawUnitLabel = True
                    Else
                        GL.PushMatrix()
                        GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, -XYZ_dbl.Z)
                        tmpUnit.Type.GLDraw(tmpUnit.Rotation)
                        GL.PopMatrix()
                        If tmpUnit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                            If CType(tmpUnit.Type, clsDroidDesign).AlwaysDrawTextLabel Then
                                DrawUnitLabel = True
                            End If
                        End If
                        If MouseOverTerrain IsNot Nothing Then
                            If MouseOverTerrain.UnitCount > 0 Then
                                If MouseOverTerrain.Units(0) Is tmpUnit Then
                                    DrawUnitLabel = True
                                End If
                            End If
                        End If
                    End If
                    If DrawUnitLabel And Not UnitTextLabels.AtMaxCount Then
                        Matrix3D.VectorRotationByMatrix(MapView.ViewAngleMatrix_Inverted, XYZ_dbl, XYZ_dbl2)
                        If MapView.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos) Then
                            If ScreenPos.X >= 0 And ScreenPos.X <= MapView.GLSize.X And ScreenPos.Y >= 0 And ScreenPos.Y <= MapView.GLSize.Y Then
                                TextLabel = New clsTextLabel
                                With TextLabel
                                    .TextFont = UnitLabelFont
                                    .SizeY = Settings.DisplayFont.SizeInPoints
                                    .Colour.Red = 1.0F
                                    .Colour.Green = 1.0F
                                    .Colour.Blue = 1.0F
                                    .Colour.Alpha = 1.0F
                                    .Pos.X = ScreenPos.X + 32
                                    .Pos.Y = ScreenPos.Y
                                    .Text = tmpUnit.Type.GetDisplayText
                                End With
                                UnitTextLabels.Add(TextLabel)
                            End If
                        End If
                    End If
                End If
            Next
        End Sub
    End Class
End Class