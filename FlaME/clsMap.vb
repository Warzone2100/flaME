Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public TerrainSize As sXY_int
    Structure sTerrainVertex
        Public Height As Byte
        Public Terrain As clsPainter.clsTerrain
    End Structure
    Structure sTerrainTile
        Structure sTexture
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
    End Structure
    Structure sTerrainSide
        Public Road As clsPainter.clsRoad
    End Structure
    Public TerrainVertex(-1, -1) As sTerrainVertex
    Public TerrainTiles(-1, -1) As sTerrainTile
    Public TerrainSideH(-1, -1) As sTerrainSide
    Public TerrainSideV(-1, -1) As sTerrainSide

    Class clsSector
        Public Pos As sXY_int
        Public GLList_Textured As Integer
        Public GLList_Wireframe As Integer
        Public Units(-1) As clsUnit
        Public Unit_SectorNum(-1) As Integer
        Public UnitCount As Integer
        Public Changed As Boolean

        Sub Deallocate()

            ReDim Units(-1)
            ReDim Unit_SectorNum(-1)
            UnitCount = 0
        End Sub

        Function Unit_Add(ByVal NewUnit As clsMap.clsUnit, ByVal NewUnitSectorNum As Integer) As Integer

            ReDim Preserve Units(UnitCount)
            ReDim Preserve Unit_SectorNum(UnitCount)
            Units(UnitCount) = NewUnit
            Unit_SectorNum(UnitCount) = NewUnitSectorNum
            Unit_Add = UnitCount
            UnitCount += 1
        End Function

        Sub Unit_Remove(ByVal Num As Integer)

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

        Sub New(ByVal NewPos As sXY_int)

            Pos = NewPos
        End Sub
    End Class
    Public Sectors(-1, -1) As clsSector
    Public SectorCount As sXY_int

    Class clsShadowSector
        Public Num As sXY_int
        Public TerrainVertex(-1, -1) As sTerrainVertex
        Public TerrainTile(-1, -1) As sTerrainTile
        Public TerrainSideH(-1, -1) As sTerrainSide
        Public TerrainSideV(-1, -1) As sTerrainSide
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

    Class clsUndo
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

    Class clsUnit
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

        Sub New()

        End Sub

        Sub New(ByVal Unit_To_Copy As clsUnit)
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

        Sub Sectors_Remove()

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
                Return WZ_StartPos
            End If
        End Function
    End Class
    Public UnitGroups(-1) As clsUnitGroup
    Public ScavengerUnitGroup As clsUnitGroup
    Public FeatureUnitGroup As clsUnitGroup

    Public Minimap_GLTexture As Integer
    Public Minimap_Texture_Size As Integer

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

    Class clsAutoSave
        Public ChangeCount As UInteger
        Public SavedDate As Date

        Sub New()

            SavedDate = Now
        End Sub
    End Class
    Public AutoSave As New clsAutoSave

    Public Painter As New clsPainter

    Public Tile_TypeNum(-1) As Byte

    Structure sGateway
        Public PosA As sXY_int
        Public PosB As sXY_int
    End Structure
    Public Gateways(-1) As sGateway
    Public GatewayCount As Integer

    Class clsSectorGraphicsChange
        Public Parent_Map As clsMap

        Public SectorIsChanged(,) As Boolean
        Public ChangedSectors() As sXY_int
        Public ChangedSectorCount As Integer

        Sub New(ByVal NewMap As clsMap)

            Parent_Map = NewMap

            ReDim SectorIsChanged(Parent_Map.SectorCount.X - 1, Parent_Map.SectorCount.Y - 1)
            ReDim ChangedSectors(Parent_Map.SectorCount.X * Parent_Map.SectorCount.Y - 1)
        End Sub

        Public Sub TerrainUndoChanged(ByVal SectorNum As sXY_int)

            Parent_Map.Sectors(SectorNum.X, SectorNum.Y).Changed = True 'do this every time. it can be changed by units without having this set
            SectorChanged(SectorNum)
        End Sub

        Public Sub SectorChanged(ByVal SectorNum As sXY_int)

            If Not SectorIsChanged(SectorNum.X, SectorNum.Y) Then
                SectorIsChanged(SectorNum.X, SectorNum.Y) = True
                ChangedSectors(ChangedSectorCount) = SectorNum
                ChangedSectorCount += 1
            End If
        End Sub

        Sub Tile_Set_Changed(ByVal TileNum As sXY_int)
            Dim SectorNum As sXY_int

            SectorNum = Parent_Map.GetTileSectorNum(TileNum)
            TerrainUndoChanged(SectorNum)
        End Sub

        Sub Vertex_Set_Changed(ByVal VertexNum As sXY_int)

            If VertexNum.X > 0 Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y))
                End If
            End If
            If VertexNum.X < Parent_Map.TerrainSize.X Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(VertexNum)
                End If
            End If
        End Sub

        Sub Vertex_And_Normals_Changed(ByVal VertexNum As sXY_int)

            If VertexNum.X > 1 Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 2, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 2, VertexNum.Y))
                End If
            End If
            If VertexNum.X > 0 Then
                If VertexNum.Y > 1 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y - 2))
                End If
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y - 1 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y + 1))
                End If
            End If
            If VertexNum.X < Parent_Map.TerrainSize.X Then
                If VertexNum.Y > 1 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X, VertexNum.Y - 2))
                End If
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(VertexNum)
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y - 1 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X, VertexNum.Y + 1))
                End If
            End If
            If VertexNum.X < Parent_Map.TerrainSize.X - 1 Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X + 1, VertexNum.Y - 1))
                End If
                If VertexNum.Y < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X + 1, VertexNum.Y))
                End If
            End If
        End Sub

        Sub SideH_Set_Changed(ByVal SideHNum As sXY_int)

            If SideHNum.Y > 0 Then
                Tile_Set_Changed(New sXY_int(SideHNum.X, SideHNum.Y - 1))
            End If
            Tile_Set_Changed(SideHNum)
        End Sub

        Sub SideV_Set_Changed(ByVal SideVNum As sXY_int)

            If SideVNum.X > 0 Then
                Tile_Set_Changed(New sXY_int(SideVNum.X - 1, SideVNum.Y))
            End If
            Tile_Set_Changed(SideVNum)
        End Sub

        Sub Update_Graphics()

            If ChangedSectorCount = 0 Then
                Exit Sub
            End If

            Dim A As Integer
            Dim X As Integer
            Dim Y As Integer

            For A = 0 To ChangedSectorCount - 1
                X = ChangedSectors(A).X
                Y = ChangedSectors(A).Y
                Parent_Map.Sector_GLList_Make(X, Y)
                SectorIsChanged(X, Y) = False
            Next
            ChangedSectorCount = 0
            Parent_Map.MinimapMakeLater()
        End Sub

        Sub Update_Graphics_And_UnitHeights()

            If ChangedSectorCount = 0 Then
                Exit Sub
            End If

            Dim A As Integer
            Dim B As Integer
            Dim X As Integer
            Dim Y As Integer
            Dim NewUnit As clsUnit
            Dim ID As UInteger
            Dim tmpUnit As clsUnit
            Dim C As Integer
            Dim OldUnits(Parent_Map.UnitCount - 1) As clsUnit
            Dim OldUnitCount As Integer = 0

            For A = 0 To ChangedSectorCount - 1
                X = ChangedSectors(A).X
                Y = ChangedSectors(A).Y
                For B = 0 To Parent_Map.Sectors(X, Y).UnitCount - 1
                    tmpUnit = Parent_Map.Sectors(X, Y).Units(B)
                    'units can be in multiple sectors, so dont include multiple times
                    For C = 0 To OldUnitCount - 1
                        If OldUnits(C) Is tmpUnit Then
                            Exit For
                        End If
                    Next
                    If C = OldUnitCount Then
                        OldUnits(OldUnitCount) = tmpUnit
                        OldUnitCount += 1
                    End If
                Next
            Next
            For A = 0 To OldUnitCount - 1
                tmpUnit = OldUnits(A)
                NewUnit = New clsUnit(tmpUnit)
                ID = tmpUnit.ID
                NewUnit.Pos.Altitude = Parent_Map.GetTerrainHeight(NewUnit.Pos.Horizontal)
                'these create changed sectors and must be done before drawing the new sectors
                Parent_Map.Unit_Remove_StoreChange(tmpUnit.Map_UnitNum)
                Parent_Map.Unit_Add_StoreChange(NewUnit, ID)
                ErrorIDChange(ID, NewUnit, "SectorChange->UpdateGraphicsAndUnitHeights")
            Next
            Update_Graphics()
        End Sub
    End Class
    Public SectorGraphicsChange As clsSectorGraphicsChange

    Public Class clsAutoTextureChange
        Public ParentMap As clsMap

        Public TileIsChanged(,) As Boolean
        Public ChangedTiles() As sXY_int
        Public ChangedTileCount As Integer

        Sub New(ByVal NewParentMap As clsMap)

            ParentMap = NewParentMap
            ReDim TileIsChanged(NewParentMap.TerrainSize.X - 1, NewParentMap.TerrainSize.Y - 1)
            ReDim ChangedTiles(NewParentMap.TerrainSize.X * NewParentMap.TerrainSize.Y - 1)
        End Sub

        Sub Tile_Set_Changed(ByVal TileNum As sXY_int)

            If Not TileIsChanged(TileNum.X, TileNum.Y) Then
                TileIsChanged(TileNum.X, TileNum.Y) = True
                ChangedTiles(ChangedTileCount) = TileNum
                ChangedTileCount += 1
            End If
        End Sub

        Sub Vertex_Set_Changed(ByVal VertexNum As sXY_int)

            If VertexNum.X > 0 Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y - 1))
                End If
                If VertexNum.Y < ParentMap.TerrainSize.Y Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X - 1, VertexNum.Y))
                End If
            End If
            If VertexNum.X < ParentMap.TerrainSize.X Then
                If VertexNum.Y > 0 Then
                    Tile_Set_Changed(New sXY_int(VertexNum.X, VertexNum.Y - 1))
                End If
                If VertexNum.Y < ParentMap.TerrainSize.Y Then
                    Tile_Set_Changed(VertexNum)
                End If
            End If
        End Sub

        Sub SideH_Set_Changed(ByVal SideHNum As sXY_int)

            If SideHNum.Y > 0 Then
                Tile_Set_Changed(New sXY_int(SideHNum.X, SideHNum.Y - 1))
            End If
            Tile_Set_Changed(SideHNum)
        End Sub

        Sub SideV_Set_Changed(ByVal SideVNum As sXY_int)

            If SideVNum.X > 0 Then
                Tile_Set_Changed(New sXY_int(SideVNum.X - 1, SideVNum.Y))
            End If
            Tile_Set_Changed(SideVNum)
        End Sub

        Sub Update_AutoTexture()
            Dim A As Integer
            Dim X As Integer
            Dim Y As Integer

            For A = 0 To ChangedTileCount - 1
                X = ChangedTiles(A).X
                Y = ChangedTiles(A).Y
                ParentMap.Tile_AutoTexture_Changed(X, Y, frmMainInstance.cbxInvalidTiles.Checked)
                TileIsChanged(X, Y) = False
            Next
            ChangedTileCount = 0
        End Sub
    End Class
    Public AutoTextureChange As clsAutoTextureChange

    Sub New()

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()
    End Sub

    Sub New(ByVal TileSizeX As Integer, ByVal TileSizeZ As Integer)

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()

        Terrain_Blank(TileSizeX, TileSizeZ)
        TileType_Reset()
        AfterInitialized()
    End Sub

    Sub New(ByVal Map_To_Copy As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int)
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim EndX As Integer
        Dim EndZ As Integer
        Dim X As Integer
        Dim Y As Integer

        MakeDefaultUnitGroups()

        'make some map data for selection

        StartX = Math.Max(0 - Offset.X, 0)
        StartZ = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Map_To_Copy.TerrainSize.X - Offset.X, Area.X)
        EndZ = Math.Min(Map_To_Copy.TerrainSize.Y - Offset.Y, Area.Y)

        TerrainSize.X = Area.X
        TerrainSize.Y = Area.Y
        ReDim TerrainVertex(TerrainSize.X, TerrainSize.Y)
        ReDim TerrainTiles(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim TerrainSideH(TerrainSize.X - 1, TerrainSize.Y)
        ReDim TerrainSideV(TerrainSize.X, TerrainSize.Y - 1)

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = StartZ To EndZ
            For X = StartX To EndX
                TerrainVertex(X, Y) = Map_To_Copy.TerrainVertex(Offset.X + X, Offset.Y + Y)
            Next
        Next
        For Y = StartZ To EndZ - 1
            For X = StartX To EndX - 1
                TerrainTiles(X, Y) = Map_To_Copy.TerrainTiles(Offset.X + X, Offset.Y + Y)
            Next
        Next
        For Y = StartZ To EndZ
            For X = StartX To EndX - 1
                TerrainSideH(X, Y) = Map_To_Copy.TerrainSideH(Offset.X + X, Offset.Y + Y)
            Next
        Next
        For Y = StartZ To EndZ - 1
            For X = StartX To EndX
                TerrainSideV(X, Y) = Map_To_Copy.TerrainSideV(Offset.X + X, Offset.Y + Y)
            Next
        Next

        SectorCount.X = Math.Ceiling(Area.X / SectorTileSize)
        SectorCount.Y = Math.Ceiling(Area.Y / SectorTileSize)
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
              Or NewUnit.Pos.Horizontal.X >= TerrainSize.X * TerrainGridSpacing _
              Or NewUnit.Pos.Horizontal.Y < 0 _
              Or NewUnit.Pos.Horizontal.Y >= TerrainSize.Y * TerrainGridSpacing) Then
                Unit_Add(NewUnit)
            End If
        Next

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Private Sub Terrain_Blank(ByVal TileSizeX As Integer, ByVal TileSizeY As Integer)
        Dim X As Integer
        Dim Y As Integer

        TerrainSize.X = TileSizeX
        TerrainSize.Y = TileSizeY
        SectorCount.X = Math.Ceiling(TileSizeX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileSizeY / SectorTileSize)
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next
        ReDim TerrainVertex(TerrainSize.X, TerrainSize.Y)
        ReDim TerrainTiles(TerrainSize.X - 1, TerrainSize.Y - 1)
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTiles(X, Y).DownSide = TileDirection_None
            Next
        Next
        ReDim TerrainSideH(TerrainSize.X - 1, TerrainSize.Y)
        ReDim TerrainSideV(TerrainSize.X, TerrainSize.Y - 1)
        Clear_Textures()
    End Sub

    Function GetTerrainSlopeAngle(ByVal Horizontal As sXY_int) As Double
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
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_dbl2 As sXYZ_dbl
        Dim XYZ_dbl3 As sXYZ_dbl
        Dim AnglePY As sAnglePY

        XG = Int(Horizontal.X / TerrainGridSpacing)
        YG = Int(Horizontal.Y / TerrainGridSpacing)
        InTileX = Clamp(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp(XG, 0, TerrainSize.X - 1)
        Y1 = Clamp(YG, 0, TerrainSize.Y - 1)
        X2 = Clamp(XG + 1, 0, TerrainSize.X)
        Y2 = Clamp(YG + 1, 0, TerrainSize.Y)
        If TerrainTiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = TerrainVertex(X1, Y1).Height
                GradientX = TerrainVertex(X2, Y1).Height - Offset
                GradientY = TerrainVertex(X1, Y2).Height - Offset
            Else
                Offset = TerrainVertex(X2, Y2).Height
                GradientX = TerrainVertex(X1, Y2).Height - Offset
                GradientY = TerrainVertex(X2, Y1).Height - Offset
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = TerrainVertex(X2, Y1).Height
                GradientX = TerrainVertex(X1, Y1).Height - Offset
                GradientY = TerrainVertex(X2, Y2).Height - Offset
            Else
                Offset = TerrainVertex(X1, Y2).Height
                GradientX = TerrainVertex(X2, Y2).Height - Offset
                GradientY = TerrainVertex(X1, Y1).Height - Offset
            End If
        End If

        XYZ_dbl.X = TerrainGridSpacing
        XYZ_dbl.Y = GradientX * HeightMultiplier
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = GradientY * HeightMultiplier
        XYZ_dbl2.Z = TerrainGridSpacing
        VectorCrossProduct(XYZ_dbl, XYZ_dbl2, XYZ_dbl3)
        If XYZ_dbl3.X <> 0.0# Or XYZ_dbl3.Z <> 0.0# Then
            GetAnglePY(XYZ_dbl3, AnglePY)
            Return RadOf90Deg - Math.Abs(AnglePY.Pitch)
        Else
            Return 0.0#
        End If
    End Function

    Function GetTerrainHeight(ByVal Horizontal As sXY_int) As Double
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

        XG = Int(Horizontal.X / TerrainGridSpacing)
        YG = Int(Horizontal.Y / TerrainGridSpacing)
        InTileX = Clamp(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp(XG, 0, TerrainSize.X - 1)
        Y1 = Clamp(YG, 0, TerrainSize.Y - 1)
        X2 = Clamp(XG + 1, 0, TerrainSize.X)
        Y2 = Clamp(YG + 1, 0, TerrainSize.Y)
        If TerrainTiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = TerrainVertex(X1, Y1).Height
                GradientX = TerrainVertex(X2, Y1).Height - Offset
                GradientY = TerrainVertex(X1, Y2).Height - Offset
                RatioX = InTileX
                RatioY = InTileZ
            Else
                Offset = TerrainVertex(X2, Y2).Height
                GradientX = TerrainVertex(X1, Y2).Height - Offset
                GradientY = TerrainVertex(X2, Y1).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = 1.0# - InTileZ
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = TerrainVertex(X2, Y1).Height
                GradientX = TerrainVertex(X1, Y1).Height - Offset
                GradientY = TerrainVertex(X2, Y2).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = InTileZ
            Else
                Offset = TerrainVertex(X1, Y2).Height
                GradientX = TerrainVertex(X2, Y2).Height - Offset
                GradientY = TerrainVertex(X1, Y1).Height - Offset
                RatioX = InTileX
                RatioY = 1.0# - InTileZ
            End If
        End If
        Return (Offset + GradientX * RatioX + GradientY * RatioY) * HeightMultiplier
    End Function

    Function TerrainVertexNormalCalc(ByVal X As Integer, ByVal Z As Integer) As sXYZ_sng
        Dim TerrainHeightX1 As Integer
        Dim TerrainHeightX2 As Integer
        Dim TerrainHeightZ1 As Integer
        Dim TerrainHeightZ2 As Integer
        Dim X2 As Integer
        Dim Z2 As Integer
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_dbl2 As sXYZ_dbl
        Dim dblTemp As Double

        X2 = Clamp(X - 1, 0, TerrainSize.X)
        Z2 = Clamp(Z, 0, TerrainSize.Y)
        TerrainHeightX1 = TerrainVertex(X2, Z2).Height
        X2 = Clamp(X + 1, 0, TerrainSize.X)
        Z2 = Clamp(Z, 0, TerrainSize.Y)
        TerrainHeightX2 = TerrainVertex(X2, Z2).Height
        X2 = Clamp(X, 0, TerrainSize.X)
        Z2 = Clamp(Z - 1, 0, TerrainSize.Y)
        TerrainHeightZ1 = TerrainVertex(X2, Z2).Height
        X2 = Clamp(X, 0, TerrainSize.X)
        Z2 = Clamp(Z + 1, 0, TerrainSize.Y)
        TerrainHeightZ2 = TerrainVertex(X2, Z2).Height
        XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier
        XYZ_dbl.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl2.Z = (TerrainHeightZ1 - TerrainHeightZ2) * HeightMultiplier
        XYZ_dbl.X = XYZ_dbl.X + XYZ_dbl2.X
        XYZ_dbl.Y = XYZ_dbl.Y + XYZ_dbl2.Y
        XYZ_dbl.Z = XYZ_dbl.Z + XYZ_dbl2.Z
        dblTemp = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z)
        TerrainVertexNormalCalc.X = XYZ_dbl.X / dblTemp
        TerrainVertexNormalCalc.Y = XYZ_dbl.Y / dblTemp
        TerrainVertexNormalCalc.Z = XYZ_dbl.Z / dblTemp
    End Function

    Sub SectorAll_GLLists_Delete()
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
        If AutoTextureChange IsNot Nothing Then
            AutoTextureChange.ParentMap = Nothing
            AutoTextureChange = Nothing
        End If
        If SectorGraphicsChange IsNot Nothing Then
            SectorGraphicsChange.Parent_Map = Nothing
            SectorGraphicsChange = Nothing
        End If
        Undo_Clear()
    End Sub

    Sub Terrain_Resize(ByVal TileOffsetX As Integer, ByVal TileOffsetZ As Integer, ByVal TileCountX As Integer, ByVal TileCountZ As Integer)
        Dim StartX As Integer
        Dim StartY As Integer
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide

        Undo_Clear()
        SectorAll_GLLists_Delete()

        StartX = Math.Max(0 - TileOffsetX, 0)
        StartY = Math.Max(0 - TileOffsetZ, 0)
        EndX = Math.Min(TerrainSize.X - TileOffsetX, TileCountX)
        EndY = Math.Min(TerrainSize.Y - TileOffsetZ, TileCountZ)

        ReDim tmpTerrainVertex(TileCountX, TileCountZ)
        ReDim tmpTerrainTile(TileCountX - 1, TileCountZ - 1)
        ReDim tmpTerrainSideH(TileCountX - 1, TileCountZ)
        ReDim tmpTerrainSideV(TileCountX, TileCountZ - 1)

        For Y = 0 To TileCountZ - 1
            For X = 0 To TileCountX - 1
                tmpTerrainTile(X, Y).Texture.TextureNum = -1
                tmpTerrainTile(X, Y).DownSide = TileDirection_None
            Next
        Next

        For Y = StartY To EndY
            For X = StartX To EndX
                tmpTerrainVertex(X, Y) = TerrainVertex(TileOffsetX + X, TileOffsetZ + Y)
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX - 1
                tmpTerrainTile(X, Y) = TerrainTiles(TileOffsetX + X, TileOffsetZ + Y)
            Next
        Next
        For Y = StartY To EndY
            For X = StartX To EndX - 1
                tmpTerrainSideH(X, Y) = TerrainSideH(TileOffsetX + X, TileOffsetZ + Y)
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX
                tmpTerrainSideV(X, Y) = TerrainSideV(TileOffsetX + X, TileOffsetZ + Y)
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer

        PosDifX = -TileOffsetX * TerrainGridSpacing
        PosDifZ = -TileOffsetZ * TerrainGridSpacing
        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            Units(A).Pos.Horizontal.X += PosDifX
            Units(A).Pos.Horizontal.Y += PosDifZ
        Next
        For A = 0 To GatewayCount - 1
            Gateways(A).PosA.X -= TileOffsetX
            Gateways(A).PosA.Y -= TileOffsetZ
            Gateways(A).PosB.X -= TileOffsetX
            Gateways(A).PosB.Y -= TileOffsetZ
        Next
        If Selected_Tile_A IsNot Nothing Then
            Selected_Tile_A.X -= TileOffsetX
            Selected_Tile_A.Y -= TileOffsetZ
            If Selected_Tile_A.X < 0 _
                Or Selected_Tile_A.X >= TileCountX _
                Or Selected_Tile_A.Y < 0 _
                Or Selected_Tile_A.Y >= TileCountZ Then
                Selected_Tile_A = Nothing
                Selected_Tile_B = Nothing
            End If
        End If
        If Selected_Tile_B IsNot Nothing Then
            Selected_Tile_B.X -= TileOffsetX
            Selected_Tile_B.Y -= TileOffsetZ
            If Selected_Tile_B.X < 0 _
                Or Selected_Tile_B.X >= TileCountX _
                Or Selected_Tile_B.Y < 0 _
                Or Selected_Tile_B.Y >= TileCountZ Then
                Selected_Tile_A = Nothing
                Selected_Tile_B = Nothing
            End If
        End If
        If Selected_Area_VertexA IsNot Nothing Then
            Selected_Area_VertexA.X -= TileOffsetX
            Selected_Area_VertexA.Y -= TileOffsetZ
            If Selected_Area_VertexA.X < 0 _
                Or Selected_Area_VertexA.X > TileCountX _
                Or Selected_Area_VertexA.Y < 0 _
                Or Selected_Area_VertexA.Y > TileCountZ Then
                Selected_Area_VertexA = Nothing
                Selected_Area_VertexB = Nothing
            End If
        End If
        If Selected_Area_VertexB IsNot Nothing Then
            Selected_Area_VertexB.X -= TileOffsetX
            Selected_Area_VertexB.Y -= TileOffsetZ
            If Selected_Area_VertexB.X < 0 _
                Or Selected_Area_VertexB.X > TileCountX _
                Or Selected_Area_VertexB.Y < 0 _
                Or Selected_Area_VertexB.Y > TileCountZ Then
                Selected_Area_VertexA = Nothing
                Selected_Area_VertexB = Nothing
            End If
        End If
        If Unit_Selected_Area_VertexA IsNot Nothing Then
            Unit_Selected_Area_VertexA.X -= TileOffsetX
            Unit_Selected_Area_VertexA.Y -= TileOffsetZ
            If Unit_Selected_Area_VertexA.X < 0 _
                Or Unit_Selected_Area_VertexA.X > TileCountX _
                Or Unit_Selected_Area_VertexA.Y < 0 _
                Or Unit_Selected_Area_VertexA.Y > TileCountZ Then
                Unit_Selected_Area_VertexA = Nothing
            End If
        End If

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCountX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCountZ / SectorTileSize)
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.Horizontal.X < 0 _
                Or Units(A).Pos.Horizontal.X >= TileCountX * TerrainGridSpacing _
                Or Units(A).Pos.Horizontal.Y < 0 _
                Or Units(A).Pos.Horizontal.Y >= TileCountZ * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(A)
                A += 1
            End If
        Loop
        A = 0
        Do While A < GatewayCount
            If Gateways(A).PosA.X < 0 _
                Or Gateways(A).PosA.X >= TileCountX _
                Or Gateways(A).PosA.Y < 0 _
                Or Gateways(A).PosA.Y >= TileCountZ _
                Or Gateways(A).PosB.X < 0 _
                Or Gateways(A).PosB.X >= TileCountX _
                Or Gateways(A).PosB.Y < 0 _
                Or Gateways(A).PosB.Y >= TileCountZ Then
                Gateway_Remove(A)
            Else
                A += 1
            End If
        Loop

        TerrainSize.X = TileCountX
        TerrainSize.Y = TileCountZ
        TerrainVertex = tmpTerrainVertex
        TerrainTiles = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.ParentMap = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange.Parent_Map = Nothing
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Sub Sector_GLList_Make(ByVal X As Integer, ByVal Y As Integer)
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
        FinishX = Math.Min(StartX + SectorTileSize, TerrainSize.X) - 1
        FinishY = Math.Min(StartY + SectorTileSize, TerrainSize.Y) - 1

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

    Sub DrawTileWireframe(ByVal TileX As Integer, ByVal TileY As Integer)
        Dim TileTerrainHeight(3) As Double
        Dim Vertex0 As sXYZ_sng
        Dim Vertex1 As sXYZ_sng
        Dim Vertex2 As sXYZ_sng
        Dim Vertex3 As sXYZ_sng

        TileTerrainHeight(0) = TerrainVertex(TileX, TileY).Height
        TileTerrainHeight(1) = TerrainVertex(TileX + 1, TileY).Height
        TileTerrainHeight(2) = TerrainVertex(TileX, TileY + 1).Height
        TileTerrainHeight(3) = TerrainVertex(TileX + 1, TileY + 1).Height

        Vertex0.X = TileX * TerrainGridSpacing
        Vertex0.Y = TileTerrainHeight(0) * HeightMultiplier
        Vertex0.Z = -TileY * TerrainGridSpacing
        Vertex1.X = (TileX + 1) * TerrainGridSpacing
        Vertex1.Y = TileTerrainHeight(1) * HeightMultiplier
        Vertex1.Z = -TileY * TerrainGridSpacing
        Vertex2.X = TileX * TerrainGridSpacing
        Vertex2.Y = TileTerrainHeight(2) * HeightMultiplier
        Vertex2.Z = -(TileY + 1) * TerrainGridSpacing
        Vertex3.X = (TileX + 1) * TerrainGridSpacing
        Vertex3.Y = TileTerrainHeight(3) * HeightMultiplier
        Vertex3.Z = -(TileY + 1) * TerrainGridSpacing

        GL.Begin(BeginMode.Lines)
        If TerrainTiles(TileX, TileY).Tri Then
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

    Sub DrawTileOrientation(ByVal Tile As sXY_int)
        Dim TileOrientation As sTileOrientation
        Dim UnrotatedPos As sXY_int
        Dim Vertex0 As sWorldPos
        Dim Vertex1 As sWorldPos
        Dim Vertex2 As sWorldPos

        TileOrientation = TerrainTiles(Tile.X, Tile.Y).Texture.Orientation

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

    Sub DrawTile(ByVal TileX As Integer, ByVal TileY As Integer)
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

        If TerrainTiles(TileX, TileY).Texture.TextureNum < 0 Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_NoTile)
        ElseIf Tileset Is Nothing Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        ElseIf TerrainTiles(TileX, TileY).Texture.TextureNum < Tileset.TileCount Then
            A = Tileset.Tiles(TerrainTiles(TileX, TileY).Texture.TextureNum).MapView_GL_Texture_Num
            If A = 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            Else
                GL.BindTexture(TextureTarget.Texture2D, A)
            End If
        Else
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        End If

        TileTerrainHeight(0) = TerrainVertex(TileX, TileY).Height
        TileTerrainHeight(1) = TerrainVertex(TileX + 1, TileY).Height
        TileTerrainHeight(2) = TerrainVertex(TileX, TileY + 1).Height
        TileTerrainHeight(3) = TerrainVertex(TileX + 1, TileY + 1).Height

        GetTileRotatedTexCoords(TerrainTiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

        Vertex0.X = TileX * TerrainGridSpacing
        Vertex0.Y = TileTerrainHeight(0) * HeightMultiplier
        Vertex0.Z = -TileY * TerrainGridSpacing
        Vertex1.X = (TileX + 1) * TerrainGridSpacing
        Vertex1.Y = TileTerrainHeight(1) * HeightMultiplier
        Vertex1.Z = -TileY * TerrainGridSpacing
        Vertex2.X = TileX * TerrainGridSpacing
        Vertex2.Y = TileTerrainHeight(2) * HeightMultiplier
        Vertex2.Z = -(TileY + 1) * TerrainGridSpacing
        Vertex3.X = (TileX + 1) * TerrainGridSpacing
        Vertex3.Y = TileTerrainHeight(3) * HeightMultiplier
        Vertex3.Z = -(TileY + 1) * TerrainGridSpacing

        Normal0 = TerrainVertexNormalCalc(TileX, TileY)
        Normal1 = TerrainVertexNormalCalc(TileX + 1, TileY)
        Normal2 = TerrainVertexNormalCalc(TileX, TileY + 1)
        Normal3 = TerrainVertexNormalCalc(TileX + 1, TileY + 1)

        GL.Begin(BeginMode.Triangles)
        If TerrainTiles(TileX, TileY).Tri Then
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
            CompileMultiPlayers = 2
            CompileMultiXPlayers = False
            CompileMultiAuthor = ""
            CompileMultiLicense = ""
            AutoScrollLimits = True
            ScrollMin.X = 0
            ScrollMin.Y = 0
            ScrollMax.X = 0UI
            ScrollMax.Y = 0UI
            CampaignGameTime = 2
            CampaignGameType = -1
        End Sub
    End Class

    Private Sub MinimapTextureFill(ByRef Texture(,,) As Byte)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim Low As sXY_int
        Dim High As sXY_int
        Dim Footprint As sXY_int
        Dim Flag As Boolean
        Dim RGB_sng As sRGB_sng
        Dim UnitMap(Texture.GetUpperBound(0), Texture.GetUpperBound(1)) As Boolean

        For Y = 0 To Texture.GetUpperBound(0)
            For X = 0 To Texture.GetUpperBound(1)
                Texture(Y, X, 3) = 255
            Next
        Next
        If frmMainInstance.menuMiniShowTex.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If TerrainTiles(X, Y).Texture.TextureNum >= 0 And TerrainTiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            RGB_sng = Tileset.Tiles(TerrainTiles(X, Y).Texture.TextureNum).Average_Color
                            Texture(Y, X, 0) = Math.Min(CInt(RGB_sng.Red * 255.0F), 255)
                            Texture(Y, X, 1) = Math.Min(CInt(RGB_sng.Green * 255.0F), 255)
                            Texture(Y, X, 2) = Math.Min(CInt(RGB_sng.Blue * 255.0F), 255)
                        End If
                    Next
                Next
            End If
            If frmMainInstance.menuMiniShowHeight.Checked Then
                Dim Height As Short
                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        Height = (CShort(TerrainVertex(X, Y).Height) + TerrainVertex(X + 1, Y).Height + TerrainVertex(X, Y + 1).Height + TerrainVertex(X + 1, Y + 1).Height) / 4.0#
                        Texture(Y, X, 0) = (Texture(Y, X, 0) * 2S + Height) / 3.0#
                        Texture(Y, X, 1) = (Texture(Y, X, 1) * 2S + Height) / 3.0#
                        Texture(Y, X, 2) = (Texture(Y, X, 2) * 2S + Height) / 3.0#
                    Next
                Next
            End If
        ElseIf frmMainInstance.menuMiniShowHeight.Checked Then
            Dim Height As Short
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Height = (CShort(TerrainVertex(X, Y).Height) + TerrainVertex(X + 1, Y).Height + TerrainVertex(X, Y + 1).Height + TerrainVertex(X + 1, Y + 1).Height) / 4.0#
                    Texture(Y, X, 0) = Height
                    Texture(Y, X, 1) = Height
                    Texture(Y, X, 2) = Height
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowCliffs.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If TerrainTiles(X, Y).Texture.TextureNum >= 0 And TerrainTiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            If Tileset.Tiles(TerrainTiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Then
                                Texture(Y, X, 0) = (CShort(Texture(Y, X, 0)) + 255S) / 2.0#
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
                        Texture(Y, X, 0) = 255
                        Texture(Y, X, 1) = 255
                        Texture(Y, X, 2) = 0
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
                                Texture(Y, X, 0) = (CShort(Texture(Y, X, 0)) + 510S) / 3.0#
                                Texture(Y, X, 1) = (CShort(Texture(Y, X, 1)) + 0S) / 3.0#
                                Texture(Y, X, 2) = (CShort(Texture(Y, X, 2)) + 0S) / 3.0#
                            End If
                        Next
                    Next
                End If
            Next
            'reset unit map
            For Y = 0 To Texture.GetUpperBound(0)
                For X = 0 To Texture.GetUpperBound(1)
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
                                Texture(Y, X, 0) = (CShort(Texture(Y, X, 0)) + 510S) / 3.0#
                                Texture(Y, X, 1) = (CShort(Texture(Y, X, 1)) + 510S) / 3.0#
                                Texture(Y, X, 2) = (CShort(Texture(Y, X, 2)) + 510S) / 3.0#
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

        Dim NewTextureSize As Integer = Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(TerrainSize.X, TerrainSize.Y)) / Math.Log(2.0#)))

        If NewTextureSize <> Minimap_Texture_Size Then
            Minimap_Texture_Size = NewTextureSize
#If Mono <> 0.0# Then
                MinimapBitmap = New Bitmap(Minimap_Texture_Size, Minimap_Texture_Size)
#End If
        End If

        Dim Size As Integer = Minimap_Texture_Size - 1

        Dim Pixels(Size, Size, 3) As Byte

        MinimapTextureFill(Pixels)

#If Mono <> 0.0# Then
        Dim Texture As Bitmap

        Texture = MinimapBitmap

        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Size
            For X = 0 To Size
                Texture.SetPixel(X, Y, ColorTranslator.FromOle(OSRGB(Pixels(Y, X, 0), Pixels(Y, X, 1), Pixels(Y, X, 2))))
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
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Pixels)
#Else
        Minimap_GLTexture = BitmapGLTexture(Texture, frmMainInstance.View.OpenGLControl, False, False)
#End If

        frmMainInstance.View_DrawViewLater()
    End Sub

    Public Sub Tile_AutoTexture_Changed(ByVal X As Integer, ByVal Z As Integer, ByVal MakeInvalidTiles As Boolean)
        Dim Terrain_Inner As clsPainter.clsTerrain
        Dim Terrain_Outer As clsPainter.clsTerrain
        Dim Road As clsPainter.clsRoad
        Dim A As Integer
        Dim Brush_Num As Integer
        Dim RoadTop As Boolean
        Dim RoadLeft As Boolean
        Dim RoadRight As Boolean
        Dim RoadBottom As Boolean

        'apply centre brushes
        If Not TerrainTiles(X, Z).Terrain_IsCliff Then
            For Brush_Num = 0 To Painter.TerrainCount - 1
                Terrain_Inner = Painter.Terrains(Brush_Num)
                If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i i i i
                                TerrainTiles(X, Z).Texture = OrientateTile(Terrain_Inner.Tiles.GetRandom(), TileDirection_None)
                            End If
                        End If
                    End If
                End If
            Next Brush_Num
        End If

        'apply transition brushes
        If Not TerrainTiles(X, Z).Terrain_IsCliff Then
            For Brush_Num = 0 To Painter.TransitionBrushCount - 1
                Terrain_Inner = Painter.TransitionBrushes(Brush_Num).Terrain_Inner
                Terrain_Outer = Painter.TransitionBrushes(Brush_Num).Terrain_Outer
                If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i i i i
                                'nothing to do here
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i i i o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i i o i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i i o o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                                Exit For
                            End If
                        End If
                    ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i o i i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopRight)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i o i o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i o o i
                                If MakeInvalidTiles Then
                                    TerrainTiles(X, Z).Texture.TextureNum = -1
                                End If
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i o o o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        End If
                    End If
                ElseIf TerrainVertex(X, Z).Terrain Is Terrain_Outer Then
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o i i i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopLeft)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o i i o
                                If MakeInvalidTiles Then
                                    TerrainTiles(X, Z).Texture.TextureNum = -1
                                End If
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o i o i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o i o o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            End If
                        End If
                    ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o o i i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o o i o
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o o o i
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopLeft)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o o o o
                                'nothing to do here
                                Exit For
                            End If
                        End If
                    End If
                End If
            Next Brush_Num
        End If

        'set cliff tiles
        If TerrainTiles(X, Z).Tri Then
            If TerrainTiles(X, Z).TriTopLeftIsCliff Then
                If TerrainTiles(X, Z).TriBottomRightIsCliff Then
                    For Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain_Inner Is Terrain_Outer Then
                            A = 0
                            If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 3 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TerrainTiles(X, Z).DownSide)
                                Exit For
                            End If
                        End If
                        If ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                            Exit For
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        If MakeInvalidTiles Then
                            TerrainTiles(X, Z).Texture.TextureNum = -1
                        End If
                    End If
                Else
                    For Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then
                            A = 0
                            If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopLeft)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z).Terrain Is Terrain_Inner Then
                            A = 0
                            If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        If MakeInvalidTiles Then
                            TerrainTiles(X, Z).Texture.TextureNum = -1
                        End If
                    End If
                End If
            ElseIf TerrainTiles(X, Z).TriBottomRightIsCliff Then
                For Brush_Num = 0 To Painter.CliffBrushCount - 1
                    Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                    If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                        If A >= 2 Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomRight)
                            Exit For
                        End If
                    ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                        If A >= 2 Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopLeft)
                            Exit For
                        End If
                    End If
                Next Brush_Num
                If Brush_Num = Painter.CliffBrushCount Then
                    If MakeInvalidTiles Then
                        TerrainTiles(X, Z).Texture.TextureNum = -1
                    End If
                End If
            Else
                'no cliff
            End If
        Else
            'default tri orientation
            If TerrainTiles(X, Z).TriTopRightIsCliff Then
                If TerrainTiles(X, Z).TriBottomLeftIsCliff Then
                    For Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain_Inner Is Terrain_Outer Then
                            A = 0
                            If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 3 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TerrainTiles(X, Z).DownSide)
                                Exit For
                            End If
                        End If
                        If ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                            Exit For
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        If MakeInvalidTiles Then
                            TerrainTiles(X, Z).Texture.TextureNum = -1
                        End If
                    End If
                Else
                    For Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                            A = 0
                            If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                            A = 0
                            If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            End If
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        If MakeInvalidTiles Then
                            TerrainTiles(X, Z).Texture.TextureNum = -1
                        End If
                    End If
                End If
            ElseIf TerrainTiles(X, Z).TriBottomLeftIsCliff Then
                For Brush_Num = 0 To Painter.CliffBrushCount - 1
                    Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                    If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                        If A >= 2 Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomLeft)
                            Exit For
                        End If
                    ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                        If A >= 2 Then
                            TerrainTiles(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopRight)
                            Exit For
                        End If
                    End If
                Next Brush_Num
                If Brush_Num = Painter.CliffBrushCount Then
                    If MakeInvalidTiles Then
                        TerrainTiles(X, Z).Texture.TextureNum = -1
                    End If
                End If
            Else
                'no cliff
            End If
        End If

        'apply roads
        Road = Nothing
        If TerrainSideH(X, Z).Road IsNot Nothing Then
            Road = TerrainSideH(X, Z).Road
        ElseIf TerrainSideH(X, Z + 1).Road IsNot Nothing Then
            Road = TerrainSideH(X, Z + 1).Road
        ElseIf TerrainSideV(X + 1, Z).Road IsNot Nothing Then
            Road = TerrainSideV(X + 1, Z).Road
        ElseIf TerrainSideV(X, Z).Road IsNot Nothing Then
            Road = TerrainSideV(X, Z).Road
        End If
        If Road IsNot Nothing Then
            For Brush_Num = 0 To Painter.RoadBrushCount - 1
                If Painter.RoadBrushes(Brush_Num).Road Is Road Then
                    Terrain_Outer = Painter.RoadBrushes(Brush_Num).Terrain
                    A = 0
                    If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then
                        A += 1
                    End If
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                        A += 1
                    End If
                    If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                        A += 1
                    End If
                    If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                        A += 1
                    End If
                    If A >= 2 Then Exit For
                End If
            Next

            If MakeInvalidTiles Then
                TerrainTiles(X, Z).Texture.TextureNum = -1
            End If

            If Brush_Num < Painter.RoadBrushCount Then
                RoadTop = (TerrainSideH(X, Z).Road Is Road)
                RoadLeft = (TerrainSideV(X, Z).Road Is Road)
                RoadRight = (TerrainSideV(X + 1, Z).Road Is Road)
                RoadBottom = (TerrainSideH(X, Z + 1).Road Is Road)
                'do cross intersection
                If RoadTop And RoadLeft And RoadRight And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_CrossIntersection.GetRandom(), TileDirection_None)
                    'do T intersection
                ElseIf RoadTop And RoadLeft And RoadRight Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Top)
                ElseIf RoadTop And RoadLeft And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Left)
                ElseIf RoadTop And RoadRight And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Right)
                ElseIf RoadLeft And RoadRight And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Bottom)
                    'do straight
                ElseIf RoadTop And RoadBottom Then
                    If Rnd() >= 0.5F Then
                        TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Top)
                    Else
                        TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Bottom)
                    End If
                ElseIf RoadLeft And RoadRight Then
                    If Rnd() >= 0.5F Then
                        TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Left)
                    Else
                        TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Right)
                    End If
                    'do corner
                ElseIf RoadTop And RoadLeft Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_TopLeft)
                ElseIf RoadTop And RoadRight Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_TopRight)
                ElseIf RoadLeft And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_BottomLeft)
                ElseIf RoadRight And RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_BottomRight)
                    'do end
                ElseIf RoadTop Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Top)
                ElseIf RoadLeft Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Left)
                ElseIf RoadRight Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Right)
                ElseIf RoadBottom Then
                    TerrainTiles(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Bottom)
                End If
            End If
        End If
    End Sub

    Sub Clear_Textures()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTiles(X, Y).Texture.TextureNum = -1
            Next
        Next
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                TerrainVertex(X, Y).Terrain = Nothing
            Next
        Next
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                Tile_AutoTexture_Changed(X, Y, frmMainInstance.cbxInvalidTiles.Checked)
            Next
        Next
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X - 1
                TerrainSideH(X, Y).Road = Nothing
            Next
        Next
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X
                TerrainSideV(X, Y).Road = Nothing
            Next
        Next
    End Sub

    Function Unit_Add_StoreChange(ByVal NewUnit As clsUnit) As Integer

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Added
        UnitChanges(UnitChangeCount).Unit = NewUnit
        UnitChangeCount += 1

        Unit_Add_StoreChange = Unit_Add(NewUnit)
    End Function

    Function Unit_Add_StoreChange(ByVal NewUnit As clsUnit, ByVal ID As UInteger) As Integer

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Added
        UnitChanges(UnitChangeCount).Unit = NewUnit
        UnitChangeCount += 1

        Unit_Add_StoreChange = Unit_Add(NewUnit, ID)
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

    Function Unit_Add(ByVal NewUnit As clsUnit) As Integer

        Return Unit_Add(NewUnit, GetAvailableID)
    End Function

    Function Unit_Add(ByVal NewUnit As clsUnit, ByVal ID As UInteger) As Integer
        Dim A As Integer

        If NewUnit.UnitGroup Is Nothing Then
            Stop
            NewUnit.UnitGroup = ScavengerUnitGroup
        End If
        If NewUnit.UnitGroup.ParentMap IsNot Me Then
            Stop
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

        NewUnit.Pos.Horizontal.X = Clamp(NewUnit.Pos.Horizontal.X, 0, TerrainSize.X * TerrainGridSpacing - 1)
        NewUnit.Pos.Horizontal.Y = Clamp(NewUnit.Pos.Horizontal.Y, 0, TerrainSize.Y * TerrainGridSpacing - 1)
        NewUnit.Pos.Altitude = GetTerrainHeight(NewUnit.Pos.Horizontal)

        If NewUnit.UnitGroup Is Nothing Then
            NewUnit.UnitGroup = ScavengerUnitGroup
        End If

        If Units.GetUpperBound(0) < UnitCount Then
            ReDim Preserve Units(UnitCount * 2 + 1)
        End If
        Units(UnitCount) = NewUnit
        Unit_Add = UnitCount
        UnitCount += 1

        Unit_Sectors_Calc(UnitCount - 1)

        UnitSectors_GLList(NewUnit)
    End Function

    Sub Unit_Remove_StoreChange(ByVal Num As Integer)

        ReDim Preserve UnitChanges(UnitChangeCount)
        UnitChanges(UnitChangeCount).Type = sUnitChange.enumType.Deleted
        UnitChanges(UnitChangeCount).Unit = Units(Num)
        UnitChangeCount += 1

        Unit_Remove(Num)
    End Sub

    Sub Unit_Remove(ByVal Num As Integer)
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

        Result.X = Int(Tile.X / SectorTileSize)
        Result.Y = Int(Tile.Y / SectorTileSize)

        Return Result
    End Function

    Public Sub GetTileSectorRange(ByVal StartTile As sXY_int, ByVal FinishTile As sXY_int, ByRef ResultSectorStart As sXY_int, ByRef ResultSectorFinish As sXY_int)

        ResultSectorStart = GetTileSectorNum(StartTile)
        ResultSectorFinish = GetTileSectorNum(FinishTile)
        ResultSectorStart.X = Clamp(ResultSectorStart.X, 0, SectorCount.X - 1)
        ResultSectorStart.Y = Clamp(ResultSectorStart.Y, 0, SectorCount.Y - 1)
        ResultSectorFinish.X = Clamp(ResultSectorFinish.X, 0, SectorCount.X - 1)
        ResultSectorFinish.Y = Clamp(ResultSectorFinish.Y, 0, SectorCount.Y - 1)
    End Sub

    Sub UnitHeight_Update_All()
        Dim A As Integer
        Dim NewUnit As clsUnit
        Dim ID As UInteger

        Dim OldUnits() As clsUnit = Units.Clone
        Dim OldUnitCount As Integer = UnitCount

        For A = 0 To OldUnitCount - 1
            NewUnit = New clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            NewUnit.Pos.Altitude = GetTerrainHeight(NewUnit.Pos.Horizontal)
            Unit_Remove_StoreChange(OldUnits(A).Map_UnitNum)
            Unit_Add_StoreChange(NewUnit, ID)
            ErrorIDChange(ID, NewUnit, "UnitHeight_Update_All")
        Next
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Sub SectorAll_GL_Update()
        Dim X As Integer
        Dim Y As Integer

        For X = 0 To SectorCount.X - 1
            For Y = 0 To SectorCount.Y - 1
                Sector_GLList_Make(X, Y)
            Next
        Next

        MinimapMakeLater()
    End Sub

    Sub SectorAll_Set_Changed()
        Dim X As Integer
        Dim Y As Integer

        For X = 0 To SectorCount.X - 1
            For Y = 0 To SectorCount.Y - 1
                Sectors(X, Y).Changed = True
            Next
        Next
    End Sub

    Sub SectorAll_Set_NotChanged()
        Dim X As Integer
        Dim Y As Integer

        For X = 0 To SectorCount.X - 1
            For Y = 0 To SectorCount.Y - 1
                Sectors(X, Y).Changed = False
            Next
        Next
    End Sub

    Function TileAligned_Pos_From_MapPos(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = (Math.Round((Horizontal.X - Footprint.X * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.X / 2.0#) * TerrainGridSpacing
        Result.Horizontal.Y = (Math.Round((Horizontal.Y - Footprint.Y * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.Y / 2.0#) * TerrainGridSpacing
        Result.Altitude = GetTerrainHeight(Result.Horizontal)

        Return Result
    End Function

    Sub Unit_Sectors_Calc(ByVal Num As Integer)
        Dim tmpUnit As clsUnit = Units(Num)
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim TileStart As sXY_int
        Dim TileFinish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer

        GetFootprintTileRangeClamped(tmpUnit.Pos.Horizontal, tmpUnit.Type.GetFootprint, TileStart, TileFinish)
        Start = GetTileSectorNum(TileStart)
        Finish = GetTileSectorNum(TileFinish)
        Start.X = Clamp(Start.X, 0, SectorCount.X - 1)
        Start.Y = Clamp(Start.Y, 0, SectorCount.Y - 1)
        Finish.X = Clamp(Finish.X, 0, SectorCount.X - 1)
        Finish.Y = Clamp(Finish.Y, 0, SectorCount.Y - 1)
        tmpUnit.SectorCount = (Finish.X - Start.X + 1) * (Finish.Y - Start.Y + 1)
        ReDim tmpUnit.Sectors(tmpUnit.SectorCount - 1)
        ReDim tmpUnit.Sectors_UnitNum(tmpUnit.SectorCount - 1)
        A = 0
        For Y = Start.Y To Finish.Y
            For X = Start.X To Finish.X
                tmpUnit.Sectors(A) = Sectors(X, Y)
                tmpUnit.Sectors_UnitNum(A) = Sectors(X, Y).Unit_Add(tmpUnit, A)
                A += 1
            Next
        Next
    End Sub

    Sub UndoStepCreate(ByVal StepName As String)
        Dim X As Integer
        Dim Y As Integer
        Dim NewUndo As New clsUndo

        NewUndo.Name = StepName

        AutoSave.ChangeCount += 1UI
        AutoSave_Test()
        frmMainInstance.tsbSave.Enabled = True

        ReDim NewUndo.ChangedSectors(SectorCount.X * SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                If Sectors(X, Y).Changed Then
                    Sectors(X, Y).Changed = False
                    NewUndo.ChangedSectors(NewUndo.ChangedSectorCount) = ShadowSectors(X, Y)
                    NewUndo.ChangedSectorCount += 1
                    ShadowSector_Create(X, Y)
                End If
            Next
        Next
        ReDim Preserve NewUndo.ChangedSectors(NewUndo.ChangedSectorCount - 1)

        NewUndo.UnitChanges = UnitChanges.Clone
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

    Sub ShadowSector_Create(ByVal SectorX As Integer, ByVal SectorZ As Integer)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim tmpShadowSector As clsShadowSector
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        tmpShadowSector = New clsShadowSector
        ShadowSectors(SectorX, SectorZ) = tmpShadowSector
        tmpShadowSector.Num.X = SectorX
        tmpShadowSector.Num.Y = SectorZ
        ReDim tmpShadowSector.TerrainVertex(SectorTileSize, SectorTileSize)
        ReDim tmpShadowSector.TerrainTile(SectorTileSize - 1, SectorTileSize - 1)
        ReDim tmpShadowSector.TerrainSideH(SectorTileSize - 1, SectorTileSize)
        ReDim tmpShadowSector.TerrainSideV(SectorTileSize, SectorTileSize - 1)
        StartX = SectorX * SectorTileSize
        StartZ = SectorZ * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, TerrainSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, TerrainSize.Y - StartZ)
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                tmpShadowSector.TerrainVertex(X, Y) = TerrainVertex(TileX, TileZ)
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                tmpShadowSector.TerrainTile(X, Y) = TerrainTiles(TileX, TileZ)
            Next
        Next
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                tmpShadowSector.TerrainSideH(X, Y) = TerrainSideH(TileX, TileZ)
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                tmpShadowSector.TerrainSideV(X, Y) = TerrainSideV(TileX, TileZ)
            Next
        Next
    End Sub

    Sub ShadowSector_CreateAll()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                ShadowSector_Create(X, Y)
            Next
        Next
    End Sub

    Sub Undo_Clear()

        UndoCount = 0
        ReDim Undos(-1)
        Undo_Pos = UndoCount
        SectorAll_Set_NotChanged()
    End Sub

    Sub Undo_Perform()
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
            ShadowSector_Create(X, Y)
            'add old state to the redo step (that was this undo step)
            Undos(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            SectorGraphicsChange.SectorChanged(Undos(Undo_Pos).ChangedSectors(A).Num)
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
                    Unit_Add(tmpUnit, ID)
                    ErrorIDChange(ID, tmpUnit, "Undo_Perform")
                Case Else
                    Stop
            End Select
        Next

        SectorGraphicsChange.Update_Graphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Sub Redo_Perform()
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
            ShadowSector_Create(X, Y)
            'add old state to the undo step (that was this redo step)
            Undos(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undos(Undo_Pos).ChangedSectorCount - 1
            SectorGraphicsChange.SectorChanged(Undos(Undo_Pos).ChangedSectors(A).Num)
        Next

        For A = 0 To Undos(Undo_Pos).UnitChangeCount - 1
            Select Case Undos(Undo_Pos).UnitChanges(A).Type
                Case sUnitChange.enumType.Added
                    'add the unit back on to the map
                    tmpUnit = Undos(Undo_Pos).UnitChanges(A).Unit
                    ID = tmpUnit.ID
                    Unit_Add(tmpUnit, ID)
                    ErrorIDChange(ID, tmpUnit, "Redo_Perform")
                Case sUnitChange.enumType.Deleted
                    'remove the unit from the map
                    Unit_Remove(Undos(Undo_Pos).UnitChanges(A).Unit.Map_UnitNum)
                Case Else
                    Stop
            End Select
        Next

        Undo_Pos += 1

        SectorGraphicsChange.Update_Graphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Sub Undo_Sector_Rejoin(ByVal Shadow_Sector_To_Rejoin As clsShadowSector)
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
        LastTileX = Math.Min(SectorTileSize, TerrainSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, TerrainSize.Y - StartZ)
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                TerrainVertex(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainVertex(X, Y)
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                TerrainTiles(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainTile(X, Y)
            Next
        Next
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                TerrainSideH(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainSideH(X, Y)
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                TerrainSideV(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainSideV(X, Y)
            Next
        Next
    End Sub

    Function Undo_Append(ByVal NewUndo As clsUndo) As Integer

        ReDim Preserve Undos(UndoCount)
        Undos(UndoCount) = NewUndo
        Undo_Append = UndoCount
        UndoCount += 1
    End Function

    Sub Undo_Insert(ByVal Pos As Integer, ByVal NewUndo As clsUndo)
        Dim A As Integer

        ReDim Preserve Undos(UndoCount)
        For A = UndoCount - 1 To Pos
            Undos(A + 1) = Undos(A)
        Next
        Undos(Pos) = NewUndo
        UndoCount += 1
    End Sub

    Sub Map_Insert(ByVal Map_To_Insert As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int, ByVal Insert_Heights As Boolean, ByVal Insert_Textures As Boolean, ByVal Insert_Units As Boolean, ByVal Delete_Units As Boolean, ByVal Insert_Gateways As Boolean, ByVal Delete_Gateways As Boolean)
        Dim Finish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim AreaAdjusted As sXY_int

        Finish.X = Math.Min(Offset.X + Math.Min(Area.X, Map_To_Insert.TerrainSize.X), TerrainSize.X)
        Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, Map_To_Insert.TerrainSize.Y), TerrainSize.Y)
        AreaAdjusted.X = Finish.X - Offset.X
        AreaAdjusted.Y = Finish.Y - Offset.Y

        GetTileSectorRange(New sXY_int(Offset.X - 1, Offset.Y - 1), Finish, SectorStart, SectorFinish)
        For Y = SectorStart.Y To SectorFinish.Y
            For X = SectorStart.X To SectorFinish.X
                SectorGraphicsChange.SectorChanged(New sXY_int(X, Y))
            Next
        Next

        If Insert_Heights Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    TerrainVertex(Offset.X + X, Offset.Y + Y).Height = Map_To_Insert.TerrainVertex(X, Y).Height
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    TerrainTiles(Offset.X + X, Offset.Y + Y).Tri = Map_To_Insert.TerrainTiles(X, Y).Tri
                Next
            Next
        End If
        If Insert_Textures Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    TerrainVertex(Offset.X + X, Offset.Y + Y).Terrain = Map_To_Insert.TerrainVertex(X, Y).Terrain
                Next
            Next
            Dim tmpTri As Boolean
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    tmpTri = TerrainTiles(Offset.X + X, Offset.Y + Y).Tri
                    TerrainTiles(Offset.X + X, Offset.Y + Y) = Map_To_Insert.TerrainTiles(X, Y)
                    TerrainTiles(Offset.X + X, Offset.Y + Y).Tri = tmpTri
                Next
            Next
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X - 1
                    TerrainSideH(Offset.X + X, Offset.Y + Y) = Map_To_Insert.TerrainSideH(X, Y)
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X
                    TerrainSideV(Offset.X + X, Offset.Y + Y) = Map_To_Insert.TerrainSideV(X, Y)
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

        SectorGraphicsChange.Update_Graphics_And_UnitHeights()
        MinimapMakeLater()
    End Sub

    Sub Gateway_Remove(ByVal Num As Integer)

        GatewayCount -= 1
        If Num <> GatewayCount Then
            Gateways(Num) = Gateways(GatewayCount)
        End If
        ReDim Preserve Gateways(GatewayCount - 1)
    End Sub

    Public Function Gateway_Add(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As Boolean

        If PosA.X >= 0 And PosA.X < TerrainSize.X And _
            PosA.Y >= 0 And PosA.Y < TerrainSize.Y And _
            PosB.X >= 0 And PosB.X < TerrainSize.X And _
            PosB.Y >= 0 And PosB.Y < TerrainSize.Y Then 'is on map
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

    Sub Sectors_Deallocate()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).Deallocate()
            Next
        Next
    End Sub

    Sub TileType_Reset()

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

    Sub AutoSave_Test()

        If Not frmMainInstance.menuAutosaveEnabled.Checked Then
            Exit Sub
        End If
        If AutoSave.ChangeCount < AutoSave_MinChanges Then
            Exit Sub
        End If
        If DateDiff("s", AutoSave.SavedDate, Now) < AutoSave_MinInterval_s Then
            Exit Sub
        End If

        AutoSave.ChangeCount = 0UI
        AutoSave.SavedDate = Now

        AutoSave_Perform()
    End Sub

    Sub AutoSave_Perform()

        If Not IO.Directory.Exists(AutoSavePath) Then
            If Not IO.Directory.CreateDirectory(AutoSavePath).Exists Then
                MsgBox("Failed to create autosave directory during autosave.", vbOKOnly + MsgBoxStyle.Exclamation, "")
            End If
        End If

        Dim DateNow As Date = Now
        Dim Path As String

        Path = AutoSavePath & "autosaved-" & DateNow.Year & "-" & MinDigits(DateNow.Month, 2) & "-" & MinDigits(DateNow.Day, 2) & "-" & MinDigits(DateNow.Hour, 2) & "-" & MinDigits(DateNow.Minute, 2) & "-" & MinDigits(DateNow.Second, 2) & "-" & MinDigits(DateNow.Millisecond, 3) & ".fmap"

        Dim Result As clsResult = Write_FMap(Path, False, frmMainInstance.menuAutosaveCompress.Checked)

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

    Public Sub SelectedUnit_Add(ByVal NewSelectedUnits() As clsUnit)
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
        Dim A As Integer

        If SectorGraphicsChange IsNot Nothing Then
            For A = 0 To UnitToUpdateFor.SectorCount - 1
                SectorGraphicsChange.SectorChanged(UnitToUpdateFor.Sectors(A).Pos)
            Next
        End If
    End Sub

    Public Function GetTileOffsetRotatedWorldPos(ByVal Tile As sXY_int, ByVal TileOffsetToRotate As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Dim RotatedOffset As sXY_int

        RotatedOffset = GetTileRotatedOffset(TerrainTiles(Tile.X, Tile.Y).Texture.Orientation, TileOffsetToRotate)
        Result.Horizontal.X = Tile.X * TerrainGridSpacing + RotatedOffset.X
        Result.Horizontal.Y = Tile.Y * TerrainGridSpacing + RotatedOffset.Y
        Result.Altitude = GetTerrainHeight(Result.Horizontal)

        Return Result
    End Function

    Public Sub GetFootprintTileRangeClamped(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        ResultStart.X = Clamp(CInt(Math.Floor(Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#)), 0, TerrainSize.X - 1)
        ResultStart.Y = Clamp(CInt(Math.Floor(Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#)), 0, TerrainSize.Y - 1)
        ResultFinish.X = Clamp(CInt(Math.Floor((Horizontal.X - 1) / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#)), 0, TerrainSize.X - 1)
        ResultFinish.Y = Clamp(CInt(Math.Floor((Horizontal.Y - 1) / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#)), 0, TerrainSize.Y - 1)
    End Sub

    Public Sub GetFootprintTileRange(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        ResultStart.X = CInt(Math.Floor(Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#))
        ResultStart.Y = CInt(Math.Floor(Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#))
        ResultFinish.X = CInt(Math.Floor((Horizontal.X - 1) / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#))
        ResultFinish.Y = CInt(Math.Floor((Horizontal.Y - 1) / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#))
    End Sub

    Public Function GetPosTileNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Math.Floor(Horizontal.X / TerrainGridSpacing)
        Result.Y = Math.Floor(Horizontal.Y / TerrainGridSpacing)

        Return Result
    End Function

    Public Function GetPosSectorNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result = GetTileSectorNum(GetPosTileNum(Horizontal))

        Return Result
    End Function

    Public Function GetSectorNumClamped(ByVal SectorNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp(SectorNum.X, 0, SectorCount.X - 1)
        Result.Y = Clamp(SectorNum.Y, 0, SectorCount.Y - 1)

        Return Result
    End Function

    Public Function GetVertexAltitude(ByVal VertexNum As sXY_int) As Integer

        Return TerrainVertex(VertexNum.X, VertexNum.Y).Height * HeightMultiplier
    End Function

    Public Function PosIsOnMap(ByVal Horizontal As sXY_int) As Boolean

        Return PosIsWithinTileArea(Horizontal, New sXY_int(0, 0), TerrainSize)
    End Function

    Public Function TileNumClampToMap(ByVal TileNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp(TileNum.X, 0, TerrainSize.X - 1)
        Result.Y = Clamp(TileNum.Y, 0, TerrainSize.Y - 1)

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
            Return PlayerColour(ColourUnitGroup.WZ_StartPos)
        End If
    End Function

    Public Sub AfterInitialized()

        ShadowSector_CreateAll()
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Public Function GetDirectory() As String

        If PathInfo Is Nothing Then
            Return My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            Return (New sSplitPath(Main_Map.PathInfo.Path)).FilePath
        End If
    End Function
End Class