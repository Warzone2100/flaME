Imports OpenTK.Graphics.OpenGL
Imports ICSharpCode.SharpZipLib

Public Class clsMap

    Public TerrainSize As sXY_int
    Structure sTerrainVertex
        Public Height As Byte
        Public Terrain As sPainter.clsTerrain
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
        Public Road As sPainter.clsRoad
    End Structure
    Public TerrainVertex(-1, -1) As sTerrainVertex
    Public TerrainTile(-1, -1) As sTerrainTile
    Public TerrainSideH(-1, -1) As sTerrainSide
    Public TerrainSideV(-1, -1) As sTerrainSide

    Class clsSector
        Public GLList_Textured As Integer
        Public GLList_Wireframe As Integer
        Public Unit(-1) As clsUnit
        Public Unit_SectorNum(-1) As Integer
        Public UnitCount As Integer
        Public Changed As Boolean

        Sub Deallocate()

            ReDim Unit(-1)
            ReDim Unit_SectorNum(-1)
            UnitCount = 0
        End Sub

        Function Unit_Add(ByVal NewUnit As clsMap.clsUnit, ByVal NewUnitSectorNum As Integer) As Integer

            ReDim Preserve Unit(UnitCount)
            ReDim Preserve Unit_SectorNum(UnitCount)
            Unit(UnitCount) = NewUnit
            Unit_SectorNum(UnitCount) = NewUnitSectorNum
            Unit_Add = UnitCount
            UnitCount += 1
        End Function

        Sub Unit_Remove(ByVal Num As Integer)

            Unit(Num).Sectors_UnitNum(Unit_SectorNum(Num)) = -1

            UnitCount = UnitCount - 1
            If Num <> UnitCount Then
                Unit(UnitCount).Sectors_UnitNum(Unit_SectorNum(UnitCount)) = Num
                Unit(Num) = Unit(UnitCount)
                Unit_SectorNum(Num) = Unit_SectorNum(UnitCount)
            End If
            ReDim Preserve Unit(UnitCount - 1)
            ReDim Preserve Unit_SectorNum(UnitCount - 1)
        End Sub

        Sub New()

        End Sub
    End Class
    Public Sector(-1, -1) As clsSector
    Public SectorCount As sXY_int

    Class clsShadowSector
        Public Num As sXY_int
        Public TerrainVertex(-1, -1) As sTerrainVertex
        Public TerrainTile(-1, -1) As sTerrainTile
        Public TerrainSideH(-1, -1) As sTerrainSide
        Public TerrainSideV(-1, -1) As sTerrainSide
    End Class
    Public ShadowSector(-1, -1) As clsShadowSector

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
    Public Undo() As clsUndo
    Public UndoCount As Integer
    Public Undo_Pos As Integer

    Public UnitChanges(-1) As sUnitChange
    Public UnitChangeCount As Integer

    Public HeightMultiplier As Integer = 2

    Public SelectedUnits() As clsUnit
    Public SelectedUnitCount As Integer
    Public Selected_Tile_A_Exists As Boolean
    Public Selected_Tile_A As sXY_int
    Public Selected_Tile_B_Exists As Boolean
    Public Selected_Tile_B As sXY_int
    Public Selected_Area_VertexA As sXY_int
    Public Selected_Area_VertexA_Exists As Boolean
    Public Selected_Area_VertexB As sXY_int
    Public Selected_Area_VertexB_Exists As Boolean

    Public Unrecognised_UnitTypes(-1) As clsUnitType
    Public Unrecognised_UnitType_Count As Integer

    Class clsUnit
        Public ID As UInteger
        Public SavePriority As Integer
        Public Type As clsUnitType
        Public Pos As sXYZ_int
        Public Rotation As Integer
        Public Name As String = "NONAME"
        Public PlayerNum As Byte
        Public Num As Integer = -1
        Public SelectedUnitNum As Integer = -1
        Public Sectors(-1) As clsSector
        Public Sectors_UnitNum(-1) As Integer
        Public SectorCount As Integer

        Sub New()

        End Sub

        Sub New(ByVal Unit_To_Copy As clsUnit)

            Type = Unit_To_Copy.Type
            Pos = Unit_To_Copy.Pos
            Rotation = Unit_To_Copy.Rotation
            PlayerNum = Unit_To_Copy.PlayerNum
            SavePriority = Unit_To_Copy.SavePriority
        End Sub

        Sub Sectors_Remove()

            SectorCount = 0
            ReDim Sectors(-1)
            ReDim Sectors_UnitNum(-1)
        End Sub
    End Class
    Public Units(-1) As clsUnit
    Public UnitCount As Integer

    Public Minimap_Texture As Integer
    Public Minimap_Texture_Size As Integer

    Public Tileset As clsTileset

    Public QuickSave_Path As String = ""

    Class clsAutoSave
        Public ChangeCount As UInteger
        Public SavedDate As Date

        Sub New()

            SavedDate = Now
        End Sub
    End Class
    Public AutoSave As New clsAutoSave

    Public Painter As New sPainter

    Public Tile_TypeNum(-1) As Byte

    Structure sGateway
        Public PosA As sXY_int
        Public PosB As sXY_int
    End Structure
    Public Gateways(-1) As sGateway
    Public GatewayCount As Integer

    Class clsSectorChange
        Public Parent_Map As clsMap

        Public SectorIsChanged(,) As Boolean
        Public ChangedSectors() As sXY_int
        Public ChangedSectorsCount As Integer

        Sub New(ByVal Map_New As clsMap)

            Parent_Map = Map_New

            ReDim SectorIsChanged(Parent_Map.SectorCount.X - 1, Parent_Map.SectorCount.Y - 1)
            ReDim ChangedSectors(Parent_Map.SectorCount.X * Parent_Map.SectorCount.Y - 1)
        End Sub

        Sub Tile_Set_Changed(ByVal X As Integer, ByVal Z As Integer)
            Static SectorNum As sXY_int

            Parent_Map.Tile_Get_Sector(X, Z, SectorNum)
            If Not SectorIsChanged(SectorNum.X, SectorNum.Y) Then
                Parent_Map.Sector(SectorNum.X, SectorNum.Y).Changed = True
                SectorIsChanged(SectorNum.X, SectorNum.Y) = True
                ChangedSectors(ChangedSectorsCount) = SectorNum
                ChangedSectorsCount = ChangedSectorsCount + 1
            End If
        End Sub

        Sub Vertex_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If X > 0 Then
                If Z > 0 Then
                    Tile_Set_Changed(X - 1, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X - 1, Z)
                End If
            End If
            If X < Parent_Map.TerrainSize.X Then
                If Z > 0 Then
                    Tile_Set_Changed(X, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X, Z)
                End If
            End If
        End Sub

        Sub Vertex_And_Normals_Changed(ByVal X As Integer, ByVal Z As Integer)

            If X > 1 Then
                If Z > 0 Then
                    Tile_Set_Changed(X - 2, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X - 2, Z)
                End If
            End If
            If X > 0 Then
                If Z > 1 Then
                    Tile_Set_Changed(X - 1, Z - 2)
                End If
                If Z > 0 Then
                    Tile_Set_Changed(X - 1, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X - 1, Z)
                End If
                If Z < Parent_Map.TerrainSize.Y - 1 Then
                    Tile_Set_Changed(X - 1, Z + 1)
                End If
            End If
            If X < Parent_Map.TerrainSize.X Then
                If Z > 1 Then
                    Tile_Set_Changed(X, Z - 2)
                End If
                If Z > 0 Then
                    Tile_Set_Changed(X, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X, Z)
                End If
                If Z < Parent_Map.TerrainSize.Y - 1 Then
                    Tile_Set_Changed(X, Z + 1)
                End If
            End If
            If X < Parent_Map.TerrainSize.X - 1 Then
                If Z > 0 Then
                    Tile_Set_Changed(X + 1, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X + 1, Z)
                End If
            End If
        End Sub

        Sub SideH_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If Z > 0 Then
                Tile_Set_Changed(X, Z - 1)
            End If
            Tile_Set_Changed(X, Z)
        End Sub

        Sub SideV_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If X > 0 Then
                Tile_Set_Changed(X - 1, Z)
            End If
            Tile_Set_Changed(X, Z)
        End Sub

        Sub Update_Graphics()
            Static A As Integer
            Static X As Integer
            Static Z As Integer

            If ChangedSectorsCount > 0 Then
                For A = 0 To ChangedSectorsCount - 1
                    X = ChangedSectors(A).X
                    Z = ChangedSectors(A).Y
                    Parent_Map.Sector_GLList_Make(X, Z)
                    SectorIsChanged(X, Z) = False
                Next
                ChangedSectorsCount = 0
                Parent_Map.Minimap_Make()
            End If
        End Sub

        Sub Update_Graphics_And_UnitHeights()
            Static A As Integer
            Static B As Integer
            Static X As Integer
            Static Z As Integer
            Static Unit_New As clsUnit
            Static ID As UInteger
            Static tmpUnit As clsUnit
            Static C As Integer

            Dim OldUnits(Parent_Map.UnitCount - 1) As clsUnit
            Dim OldUnitCount As Integer = 0

            If ChangedSectorsCount > 0 Then
                For A = 0 To ChangedSectorsCount - 1
                    X = ChangedSectors(A).X
                    Z = ChangedSectors(A).Y
                    Parent_Map.Sector_GLList_Make(X, Z)
                    For B = 0 To Parent_Map.Sector(X, Z).UnitCount - 1
                        tmpUnit = Parent_Map.Sector(X, Z).Unit(B)
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
                    SectorIsChanged(X, Z) = False
                Next
                For A = 0 To OldUnitCount - 1
                    tmpUnit = OldUnits(A)
                    Unit_New = New clsUnit(tmpUnit)
                    ID = tmpUnit.ID
                    Unit_New.Pos.Y = Parent_Map.GetTerrainHeight(Unit_New.Pos.X, Unit_New.Pos.Z)
                    Parent_Map.Unit_Remove_StoreChange(tmpUnit.Num)
                    Parent_Map.Unit_Add_StoreChange(Unit_New, ID)
                Next
                ChangedSectorsCount = 0
                Parent_Map.Minimap_Make()
            End If
        End Sub
    End Class
    Public SectorChange As clsSectorChange

    Public Class clsAutoTextureChange
        Public Parent_Map As clsMap

        Public TileIsChanged(,) As Boolean
        Public ChangedTiles() As sXY_int
        Public ChangedTileCount As Integer

        Sub New(ByVal Parent_Map_New As clsMap)

            Parent_Map = Parent_Map_New
            ReDim TileIsChanged(Parent_Map_New.TerrainSize.X - 1, Parent_Map_New.TerrainSize.Y - 1)
            ReDim ChangedTiles(Parent_Map_New.TerrainSize.X * Parent_Map_New.TerrainSize.Y - 1)
        End Sub

        Sub Tile_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If Not TileIsChanged(X, Z) Then
                TileIsChanged(X, Z) = True
                ChangedTiles(ChangedTileCount).X = X
                ChangedTiles(ChangedTileCount).Y = Z
                ChangedTileCount += 1
            End If
        End Sub

        Sub Vertex_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If X > 0 Then
                If Z > 0 Then
                    Tile_Set_Changed(X - 1, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X - 1, Z)
                End If
            End If
            If X < Parent_Map.TerrainSize.X Then
                If Z > 0 Then
                    Tile_Set_Changed(X, Z - 1)
                End If
                If Z < Parent_Map.TerrainSize.Y Then
                    Tile_Set_Changed(X, Z)
                End If
            End If
        End Sub

        Sub SideH_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If Z > 0 Then
                Tile_Set_Changed(X, Z - 1)
            End If
            Tile_Set_Changed(X, Z)
        End Sub

        Sub SideV_Set_Changed(ByVal X As Integer, ByVal Z As Integer)

            If X > 0 Then
                Tile_Set_Changed(X - 1, Z)
            End If
            Tile_Set_Changed(X, Z)
        End Sub

        Sub Update_AutoTexture()
            Dim A As Integer
            Dim X As Integer
            Dim Z As Integer

            For A = 0 To ChangedTileCount - 1
                X = ChangedTiles(A).X
                Z = ChangedTiles(A).Y
                Parent_Map.Tile_AutoTexture_Changed(X, Z)
                TileIsChanged(X, Z) = False
            Next
            ChangedTileCount = 0
        End Sub
    End Class
    Public AutoTextureChange As clsAutoTextureChange

    Sub New()

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = 1000
    End Sub

    Sub New(ByVal TileSizeX As Integer, ByVal TileSizeZ As Integer)

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = 1000

        Terrain_Blank(TileSizeX, TileSizeZ)
        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange = New clsSectorChange(Me)
        TileType_Reset()
    End Sub

    Sub New(ByVal Map_To_Copy As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int)
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim EndX As Integer
        Dim EndZ As Integer
        Dim X As Integer
        Dim Z As Integer

        'MakeMinimapTimer = New Timer
        'MakeMinimapTimer.Interval = 1000

        'make some map data for selection

        Unrecognised_UnitTypes = Map_To_Copy.Unrecognised_UnitTypes.Clone
        Unrecognised_UnitType_Count = Map_To_Copy.Unrecognised_UnitType_Count

        StartX = Math.Max(0 - Offset.X, 0)
        StartZ = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Map_To_Copy.TerrainSize.X - Offset.X, Area.X)
        EndZ = Math.Min(Map_To_Copy.TerrainSize.Y - Offset.Y, Area.Y)

        TerrainSize.X = Area.X
        TerrainSize.Y = Area.Y
        ReDim TerrainVertex(TerrainSize.X, TerrainSize.Y)
        ReDim TerrainTile(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim TerrainSideH(TerrainSize.X - 1, TerrainSize.Y)
        ReDim TerrainSideV(TerrainSize.X, TerrainSize.Y - 1)

        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTile(X, Z).Texture.TextureNum = -1
            Next
        Next

        For Z = StartZ To EndZ
            For X = StartX To EndX
                TerrainVertex(X, Z) = Map_To_Copy.TerrainVertex(Offset.X + X, Offset.Y + Z)
            Next
        Next
        For Z = StartZ To EndZ - 1
            For X = StartX To EndX - 1
                TerrainTile(X, Z) = Map_To_Copy.TerrainTile(Offset.X + X, Offset.Y + Z)
            Next
        Next
        For Z = StartZ To EndZ
            For X = StartX To EndX - 1
                TerrainSideH(X, Z) = Map_To_Copy.TerrainSideH(Offset.X + X, Offset.Y + Z)
            Next
        Next
        For Z = StartZ To EndZ - 1
            For X = StartX To EndX
                TerrainSideV(X, Z) = Map_To_Copy.TerrainSideV(Offset.X + X, Offset.Y + Z)
            Next
        Next

        SectorCount.X = Math.Ceiling(Area.X / SectorTileSize)
        SectorCount.Y = Math.Ceiling(Area.Y / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
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
            NewUnit.Pos.X += PosDifX
            NewUnit.Pos.Z += PosDifZ
            If Not (NewUnit.Pos.X < 0 _
                    Or NewUnit.Pos.X >= TerrainSize.X * TerrainGridSpacing _
                    Or NewUnit.Pos.Z < 0 _
                    Or NewUnit.Pos.Z >= TerrainSize.Y * TerrainGridSpacing) Then
                Unit_Add(NewUnit)
            End If
        Next

        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange = New clsSectorChange(Me)
    End Sub

    Sub Terrain_Blank(ByVal TileSizeX As Integer, ByVal TileSizeZ As Integer)
        Dim X As Integer
        Dim Z As Integer

        TerrainSize.X = TileSizeX
        TerrainSize.Y = TileSizeZ
        SectorCount.X = Math.Ceiling(TileSizeX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileSizeZ / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
            Next
        Next
        ReDim TerrainVertex(TerrainSize.X, TerrainSize.Y)
        ReDim TerrainTile(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim TerrainSideH(TerrainSize.X - 1, TerrainSize.Y)
        ReDim TerrainSideV(TerrainSize.X, TerrainSize.Y - 1)
        Clear_Textures()
    End Sub

    Function GetTerrainSlopeAngle(ByVal X As Integer, ByVal Z As Integer) As Double
        Static X1 As Integer
        Static X2 As Integer
        Static Z1 As Integer
        Static Z2 As Integer
        Static InTileX As Double
        Static InTileZ As Double
        Static XG As Integer
        Static ZG As Integer
        Static GradientX As Double
        Static GradientZ As Double
        Static Offset As Double
        XG = Int(X / TerrainGridSpacing)
        ZG = Int(Z / TerrainGridSpacing)
        InTileX = Clamp(X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp(Z / TerrainGridSpacing - ZG, 0.0#, 1.0#)
        X1 = Clamp(XG, 0, TerrainSize.X - 1)
        Z1 = Clamp(ZG, 0, TerrainSize.Y - 1)
        X2 = Clamp(XG + 1, 0, TerrainSize.X)
        Z2 = Clamp(ZG + 1, 0, TerrainSize.Y)
        If TerrainTile(X1, Z1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = TerrainVertex(X1, Z1).Height
                GradientX = TerrainVertex(X2, Z1).Height - Offset
                GradientZ = TerrainVertex(X1, Z2).Height - Offset
            Else
                Offset = TerrainVertex(X2, Z2).Height
                GradientX = TerrainVertex(X1, Z2).Height - Offset
                GradientZ = TerrainVertex(X2, Z1).Height - Offset
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = TerrainVertex(X2, Z1).Height
                GradientX = TerrainVertex(X1, Z1).Height - Offset
                GradientZ = TerrainVertex(X2, Z2).Height - Offset
            Else
                Offset = TerrainVertex(X1, Z2).Height
                GradientX = TerrainVertex(X2, Z2).Height - Offset
                GradientZ = TerrainVertex(X1, Z1).Height - Offset
            End If
        End If

        Static XYZ_dbl As sXYZ_dbl
        Static XYZ_dbl2 As sXYZ_dbl
        Static XYZ_dbl3 As sXYZ_dbl
        Static AnglePY As sAnglePY

        XYZ_dbl.X = TerrainGridSpacing
        XYZ_dbl.Y = GradientX * HeightMultiplier
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = GradientZ * HeightMultiplier
        XYZ_dbl2.Z = TerrainGridSpacing
        CrossProduct(XYZ_dbl, XYZ_dbl2, XYZ_dbl3)
        If XYZ_dbl3.X <> 0.0# Or XYZ_dbl3.Z <> 0.0# Then
            GetAnglePY(XYZ_dbl3, AnglePY)
            GetTerrainSlopeAngle = RadOf90Deg - Math.Abs(AnglePY.Pitch)
        Else
            GetTerrainSlopeAngle = 0.0#
        End If
    End Function

    Function GetTerrainHeight(ByVal X As Integer, ByVal Z As Integer) As Double
        Static X1 As Integer
        Static X2 As Integer
        Static Z1 As Integer
        Static Z2 As Integer
        Static InTileX As Double
        Static InTileZ As Double
        Static XG As Integer
        Static ZG As Integer
        Static GradientX As Double
        Static GradientZ As Double
        Static Offset As Double
        Static RatioX As Double
        Static RatioZ As Double
        XG = Int(X / TerrainGridSpacing)
        ZG = Int(Z / TerrainGridSpacing)
        InTileX = Clamp(X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp(Z / TerrainGridSpacing - ZG, 0.0#, 1.0#)
        X1 = Clamp(XG, 0, TerrainSize.X - 1)
        Z1 = Clamp(ZG, 0, TerrainSize.Y - 1)
        X2 = Clamp(XG + 1, 0, TerrainSize.X)
        Z2 = Clamp(ZG + 1, 0, TerrainSize.Y)
        If TerrainTile(X1, Z1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = TerrainVertex(X1, Z1).Height
                GradientX = TerrainVertex(X2, Z1).Height - Offset
                GradientZ = TerrainVertex(X1, Z2).Height - Offset
                RatioX = InTileX
                RatioZ = InTileZ
            Else
                Offset = TerrainVertex(X2, Z2).Height
                GradientX = TerrainVertex(X1, Z2).Height - Offset
                GradientZ = TerrainVertex(X2, Z1).Height - Offset
                RatioX = 1.0# - InTileX
                RatioZ = 1.0# - InTileZ
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = TerrainVertex(X2, Z1).Height
                GradientX = TerrainVertex(X1, Z1).Height - Offset
                GradientZ = TerrainVertex(X2, Z2).Height - Offset
                RatioX = 1.0# - InTileX
                RatioZ = InTileZ
            Else
                Offset = TerrainVertex(X1, Z2).Height
                GradientX = TerrainVertex(X2, Z2).Height - Offset
                GradientZ = TerrainVertex(X1, Z1).Height - Offset
                RatioX = InTileX
                RatioZ = 1.0# - InTileZ
            End If
        End If
        GetTerrainHeight = (Offset + GradientX * RatioX + GradientZ * RatioZ) * HeightMultiplier
    End Function

    Function TerrainVertexNormalCalc(ByVal X As Integer, ByVal Z As Integer) As sXYZ_sng
        Static TerrainHeightX1 As Integer
        Static TerrainHeightX2 As Integer
        Static TerrainHeightZ1 As Integer
        Static TerrainHeightZ2 As Integer
        Static X2 As Integer
        Static Z2 As Integer
        Static XYZ_dbl As sXYZ_dbl
        Static XYZ_dbl2 As sXYZ_dbl
        Static dblTemp As Double

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
        Dim Z As Integer

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                If Sector(X, Z).GLList_Textured > 0 Then
                    GL.DeleteLists(Sector(X, Z).GLList_Textured, 1)
                    Sector(X, Z).GLList_Textured = 0
                End If
                If Sector(X, Z).GLList_Wireframe > 0 Then
                    GL.DeleteLists(Sector(X, Z).GLList_Wireframe, 1)
                    Sector(X, Z).GLList_Wireframe = 0
                End If
            Next
        Next
    End Sub

    Sub Deallocate()

        Do While UnitCount > 0
            Unit_Remove(UnitCount - 1)
        Loop
        SectorAll_GLLists_Delete()
        If Minimap_Texture > 0 Then
            GL.DeleteTextures(1, Minimap_Texture)
        End If
        If AutoTextureChange IsNot Nothing Then
            AutoTextureChange.Parent_Map = Nothing
            AutoTextureChange = Nothing
        End If
        If SectorChange IsNot Nothing Then
            SectorChange.Parent_Map = Nothing
            SectorChange = Nothing
        End If
        Undo_Clear()
    End Sub

    Sub Terrain_Resize(ByVal TileOffsetX As Integer, ByVal TileOffsetZ As Integer, ByVal TileCountX As Integer, ByVal TileCountZ As Integer)
        Dim StartZ As Integer
        Dim StartX As Integer
        Dim EndZ As Integer
        Dim EndX As Integer
        Dim Z As Integer
        Dim X As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide

        Undo_Clear()
        SectorAll_GLLists_Delete()

        StartX = Math.Max(0 - TileOffsetX, 0)
        StartZ = Math.Max(0 - TileOffsetZ, 0)
        EndX = Math.Min(TerrainSize.X - TileOffsetX, TileCountX)
        EndZ = Math.Min(TerrainSize.Y - TileOffsetZ, TileCountZ)

        ReDim tmpTerrainVertex(TileCountX, TileCountZ)
        ReDim tmpTerrainTile(TileCountX - 1, TileCountZ - 1)
        ReDim tmpTerrainSideH(TileCountX - 1, TileCountZ)
        ReDim tmpTerrainSideV(TileCountX, TileCountZ - 1)

        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX - 1
                tmpTerrainTile(X, Z).Texture.TextureNum = -1
            Next
        Next

        For Z = StartZ To EndZ
            For X = StartX To EndX
                tmpTerrainVertex(X, Z) = TerrainVertex(TileOffsetX + X, TileOffsetZ + Z)
            Next
        Next
        For Z = StartZ To EndZ - 1
            For X = StartX To EndX - 1
                tmpTerrainTile(X, Z) = TerrainTile(TileOffsetX + X, TileOffsetZ + Z)
            Next
        Next
        For Z = StartZ To EndZ
            For X = StartX To EndX - 1
                tmpTerrainSideH(X, Z) = TerrainSideH(TileOffsetX + X, TileOffsetZ + Z)
            Next
        Next
        For Z = StartZ To EndZ - 1
            For X = StartX To EndX
                tmpTerrainSideV(X, Z) = TerrainSideV(TileOffsetX + X, TileOffsetZ + Z)
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer

        PosDifX = -TileOffsetX * TerrainGridSpacing
        PosDifZ = -TileOffsetZ * TerrainGridSpacing
        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            Units(A).Pos.X = Units(A).Pos.X + PosDifX
            Units(A).Pos.Z = Units(A).Pos.Z + PosDifZ
        Next
        For A = 0 To GatewayCount - 1
            Gateways(A).PosA.X = Gateways(A).PosA.X - TileOffsetX
            Gateways(A).PosA.Y = Gateways(A).PosA.Y - TileOffsetZ
            Gateways(A).PosB.X = Gateways(A).PosB.X - TileOffsetX
            Gateways(A).PosB.Y = Gateways(A).PosB.Y - TileOffsetZ
        Next
        If Selected_Tile_A_Exists Then
            Selected_Tile_A.X = Selected_Tile_A.X - TileOffsetX
            Selected_Tile_A.Y = Selected_Tile_A.Y - TileOffsetZ
            If Selected_Tile_A.X < 0 _
              Or Selected_Tile_A.X >= TileCountX _
              Or Selected_Tile_A.Y < 0 _
              Or Selected_Tile_A.Y >= TileCountZ Then
                Selected_Tile_A_Exists = False
            End If
        End If
        If Selected_Tile_B_Exists Then
            Selected_Tile_B.X = Selected_Tile_B.X - TileOffsetX
            Selected_Tile_B.Y = Selected_Tile_B.Y - TileOffsetZ
            If Selected_Tile_B.X < 0 _
              Or Selected_Tile_B.X >= TileCountX _
              Or Selected_Tile_B.Y < 0 _
              Or Selected_Tile_B.Y >= TileCountZ Then
                Selected_Tile_B_Exists = False
            End If
        End If
        If Selected_Area_VertexA_Exists Then
            Selected_Area_VertexA.X = Selected_Area_VertexA.X - TileOffsetX
            Selected_Area_VertexA.Y = Selected_Area_VertexA.Y - TileOffsetZ
            If Selected_Area_VertexA.X < 0 _
              Or Selected_Area_VertexA.X > TileCountX _
              Or Selected_Area_VertexA.Y < 0 _
              Or Selected_Area_VertexA.Y > TileCountZ Then
                Selected_Area_VertexA_Exists = False
            End If
        End If
        If Selected_Area_VertexB_Exists Then
            Selected_Area_VertexB.X = Selected_Area_VertexB.X - TileOffsetX
            Selected_Area_VertexB.Y = Selected_Area_VertexB.Y - TileOffsetZ
            If Selected_Area_VertexB.X < 0 _
              Or Selected_Area_VertexB.X > TileCountX _
              Or Selected_Area_VertexB.Y < 0 _
              Or Selected_Area_VertexB.Y > TileCountZ Then
                Selected_Area_VertexB_Exists = False
            End If
        End If

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCountX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCountZ / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.X < 0 _
              Or Units(A).Pos.X >= TileCountX * TerrainGridSpacing _
              Or Units(A).Pos.Z < 0 _
              Or Units(A).Pos.Z >= TileCountZ * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(A)
                A = A + 1
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
                A = A + 1
            End If
        Loop

        TerrainSize.X = TileCountX
        TerrainSize.Y = TileCountZ
        TerrainVertex = tmpTerrainVertex
        TerrainTile = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV

        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.Parent_Map = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange.Parent_Map = Nothing
        SectorChange = New clsSectorChange(Me)
    End Sub

    Sub Sector_GLList_Make(ByVal X As Integer, ByVal Z As Integer)
        Static TileX As Integer
        Static TileZ As Integer
        Static StartX As Integer
        Static StartZ As Integer
        Static FinishX As Integer
        Static FinishZ As Integer

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        If Sector(X, Z).GLList_Textured > 0 Then
            GL.DeleteLists(Sector(X, Z).GLList_Textured, 1)
            Sector(X, Z).GLList_Textured = 0
        End If
        If Sector(X, Z).GLList_Wireframe > 0 Then
            GL.DeleteLists(Sector(X, Z).GLList_Wireframe, 1)
            Sector(X, Z).GLList_Wireframe = 0
        End If

        StartX = X * SectorTileSize
        StartZ = Z * SectorTileSize
        FinishX = Math.Min(StartX + SectorTileSize, TerrainSize.X) - 1
        FinishZ = Math.Min(StartZ + SectorTileSize, TerrainSize.Y) - 1

        Sector(X, Z).GLList_Textured = GL.GenLists(1)
        GL.NewList(Sector(X, Z).GLList_Textured, ListMode.Compile)

        For TileZ = StartZ To FinishZ
            For TileX = StartX To FinishX
                DrawTile(TileX, TileZ)
            Next
        Next

        GL.EndList()

        Sector(X, Z).GLList_Wireframe = GL.GenLists(1)
        GL.NewList(Sector(X, Z).GLList_Wireframe, ListMode.Compile)

        For TileZ = StartZ To FinishZ
            For TileX = StartX To FinishX
                DrawTileWireframe(TileX, TileZ)
            Next
        Next

        GL.EndList()
    End Sub

    Sub DrawTileWireframe(ByVal TileX As Integer, ByVal TileZ As Integer)
        Static TileTerrainHeight(3) As Double
        Static Vertex0 As sXYZ_sng
        Static Vertex1 As sXYZ_sng
        Static Vertex2 As sXYZ_sng
        Static Vertex3 As sXYZ_sng

        TileTerrainHeight(0) = TerrainVertex(TileX, TileZ).Height
        TileTerrainHeight(1) = TerrainVertex(TileX + 1, TileZ).Height
        TileTerrainHeight(2) = TerrainVertex(TileX, TileZ + 1).Height
        TileTerrainHeight(3) = TerrainVertex(TileX + 1, TileZ + 1).Height

        Vertex0.X = TileX * TerrainGridSpacing
        Vertex0.Y = TileTerrainHeight(0) * HeightMultiplier
        Vertex0.Z = -TileZ * TerrainGridSpacing
        Vertex1.X = (TileX + 1) * TerrainGridSpacing
        Vertex1.Y = TileTerrainHeight(1) * HeightMultiplier
        Vertex1.Z = -TileZ * TerrainGridSpacing
        Vertex2.X = TileX * TerrainGridSpacing
        Vertex2.Y = TileTerrainHeight(2) * HeightMultiplier
        Vertex2.Z = -(TileZ + 1) * TerrainGridSpacing
        Vertex3.X = (TileX + 1) * TerrainGridSpacing
        Vertex3.Y = TileTerrainHeight(3) * HeightMultiplier
        Vertex3.Z = -(TileZ + 1) * TerrainGridSpacing

        GL.Begin(BeginMode.Lines)
        GL.Color3(0.0F, 1.0F, 0.0F)
        If TerrainTile(TileX, TileZ).Tri Then
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

    Sub DrawTileOrientation(ByVal TileX As Integer, ByVal TileZ As Integer)
        Static TileOrientation As sTileOrientation
        Static UnrotatedPos As sXY_sng
        Static RotatedPos As sXY_sng
        Static Vertex0 As sXYZ_int
        Static Vertex1 As sXYZ_int
        Static Vertex2 As sXYZ_int

        TileOrientation = TerrainTile(TileX, TileZ).Texture.Orientation

        UnrotatedPos.X = 0.25F
        UnrotatedPos.Y = 0.25F
        RotatedPos = GetTileRotatedPos(TileOrientation, UnrotatedPos)
        Vertex0.X = TileX * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.X
        Vertex0.Z = TileZ * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.Y
        Vertex0.Y = GetTerrainHeight(Vertex0.X, Vertex0.Z)
        Vertex0.Z = -Vertex0.Z

        UnrotatedPos.X = 0.5F
        UnrotatedPos.Y = 0.25F
        RotatedPos = GetTileRotatedPos(TileOrientation, UnrotatedPos)
        Vertex1.X = TileX * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.X
        Vertex1.Z = TileZ * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.Y
        Vertex1.Y = GetTerrainHeight(Vertex1.X, Vertex1.Z)
        Vertex1.Z = -Vertex1.Z

        UnrotatedPos.X = 0.5F
        UnrotatedPos.Y = 0.5F
        RotatedPos = GetTileRotatedPos(TileOrientation, UnrotatedPos)
        Vertex2.X = TileX * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.X
        Vertex2.Z = TileZ * TerrainGridSpacing + TerrainGridSpacing * RotatedPos.Y
        Vertex2.Y = GetTerrainHeight(Vertex2.X, Vertex2.Z)
        Vertex2.Z = -Vertex2.Z

        GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
        GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
        GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
    End Sub

    Sub DrawTile(ByVal TileX As Integer, ByVal TileZ As Integer)
        Static TileTerrainHeight(3) As Double
        Static Vertex0 As sXYZ_sng
        Static Vertex1 As sXYZ_sng
        Static Vertex2 As sXYZ_sng
        Static Vertex3 As sXYZ_sng
        Static Normal0 As sXYZ_sng
        Static Normal1 As sXYZ_sng
        Static Normal2 As sXYZ_sng
        Static Normal3 As sXYZ_sng
        Static TexCoord0 As sXY_sng
        Static TexCoord1 As sXY_sng
        Static TexCoord2 As sXY_sng
        Static TexCoord3 As sXY_sng
        Static A As Integer

        If TerrainTile(TileX, TileZ).Texture.TextureNum < 0 Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_NoTile)
        ElseIf Tileset Is Nothing Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        ElseIf TerrainTile(TileX, TileZ).Texture.TextureNum < Tileset.TileCount Then
            A = Tileset.Tiles(TerrainTile(TileX, TileZ).Texture.TextureNum).MapView_GL_Texture_Num
            If A = 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            Else
                GL.BindTexture(TextureTarget.Texture2D, A)
            End If
        Else
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        End If

        TileTerrainHeight(0) = TerrainVertex(TileX, TileZ).Height
        TileTerrainHeight(1) = TerrainVertex(TileX + 1, TileZ).Height
        TileTerrainHeight(2) = TerrainVertex(TileX, TileZ + 1).Height
        TileTerrainHeight(3) = TerrainVertex(TileX + 1, TileZ + 1).Height

        GetTileRotatedTexCoords(TerrainTile(TileX, TileZ).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

        Vertex0.X = TileX * TerrainGridSpacing
        Vertex0.Y = TileTerrainHeight(0) * HeightMultiplier
        Vertex0.Z = -TileZ * TerrainGridSpacing
        Vertex1.X = (TileX + 1) * TerrainGridSpacing
        Vertex1.Y = TileTerrainHeight(1) * HeightMultiplier
        Vertex1.Z = -TileZ * TerrainGridSpacing
        Vertex2.X = TileX * TerrainGridSpacing
        Vertex2.Y = TileTerrainHeight(2) * HeightMultiplier
        Vertex2.Z = -(TileZ + 1) * TerrainGridSpacing
        Vertex3.X = (TileX + 1) * TerrainGridSpacing
        Vertex3.Y = TileTerrainHeight(3) * HeightMultiplier
        Vertex3.Z = -(TileZ + 1) * TerrainGridSpacing

        Normal0 = TerrainVertexNormalCalc(TileX, TileZ)
        Normal1 = TerrainVertexNormalCalc(TileX + 1, TileZ)
        Normal2 = TerrainVertexNormalCalc(TileX, TileZ + 1)
        Normal3 = TerrainVertexNormalCalc(TileX + 1, TileZ + 1)

        GL.Begin(BeginMode.Triangles)
        If TerrainTile(TileX, TileZ).Tri Then
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

    Structure sLNDTile
        Dim Vertex0Height As Short
        Dim Vertex1Height As Short
        Dim Vertex2Height As Short
        Dim Vertex3Height As Short
        Dim TID As Short
        Dim VF As Short
        Dim TF As Short
        Dim F As Short
    End Structure

    Structure sLNDObject
        Dim ID As Integer
        Dim TypeNum As Integer
        Dim Code As String
        Dim PlayerNum As Integer
        Dim Name As String
        Dim Pos As sXYZ_sng
        Dim Rotation As sXYZ_int
    End Structure

    Function Load_LND(ByVal Path As String) As sResult
        Load_LND.Success = False
        Load_LND.Problem = ""

        Try

            Dim strTemp As String
            Dim strTemp2 As String
            Dim X As Integer
            Dim Z As Integer
            Dim A As Integer
            Dim B As Integer
            Dim Tile_Num As Integer
            Dim LineData(-1) As String
            Dim LineCount As Integer
            Dim Bytes() As Byte
            Dim ByteCount As Integer
            Dim Line_Num As Integer
            Dim LNDTile() As sLNDTile
            Dim LNDObject(-1) As sLNDObject

            'load all bytes
            Bytes = IO.File.ReadAllBytes(Path)
            ByteCount = Bytes.GetUpperBound(0) + 1

            BytesToLines(Bytes, LineData)
            LineCount = LineData.GetUpperBound(0) + 1

            ReDim Preserve LNDTile(LineCount - 1)

            Dim strTemp3 As String
            Dim GotTiles As Boolean
            Dim GotObjects As Boolean
            Dim GotGates As Boolean
            Dim GotTileTypes As Boolean
            Dim LNDTileType(-1) As Byte
            Dim ObjectCount As Integer
            Dim ObjectText(10) As String
            Dim GateText(3) As String
            Dim TileTypeText(255) As String
            Dim LNDTileTypeCount As Integer
            Dim LNDGate(-1) As sGateway
            Dim LNDGateCount As Integer
            Dim C As Integer
            Dim D As Integer
            Dim GotText As Boolean
            Dim FlipX As Boolean
            Dim FlipZ As Boolean
            Dim Rotation As Byte

            Line_Num = 0
            Do While Line_Num < LineCount
                strTemp = LineData(Line_Num)

                A = InStr(1, strTemp, "HeightScale ")
                If A = 0 Then
                Else
                    'HeightMultiplier = Val(Right(strTemp, Len(strTemp) - (A + 11)))
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "TileWidth ")
                If A = 0 Then
                Else
                End If

                A = InStr(1, strTemp, "TileHeight ")
                If A = 0 Then
                Else
                End If

                A = InStr(1, strTemp, "MapHeight ")
                If A = 0 Then
                Else
                    TerrainSize.Y = Val(Right(strTemp, Len(strTemp) - (A + 9)))
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "MapWidth ")
                If A = 0 Then
                Else
                    TerrainSize.X = Val(Right(strTemp, Len(strTemp) - (A + 8)))
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Textures {")
                If A = 0 Then
                Else
                    Line_Num = Line_Num + 1
                    strTemp = LineData(Line_Num)

                    strTemp2 = LCase(strTemp)
                    If InStr(1, strTemp2, "tertilesc1") > 0 Then
                        Tileset = Tileset_Arizona

                        GoTo LineDone
                    ElseIf InStr(1, strTemp2, "tertilesc2") > 0 Then
                        Tileset = Tileset_Urban

                        GoTo LineDone
                    ElseIf InStr(1, strTemp2, "tertilesc3") > 0 Then
                        Tileset = Tileset_Rockies

                        GoTo LineDone
                    End If

                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Tiles {")
                If A = 0 Or GotTiles Then
                Else
                    Line_Num = Line_Num + 1
                    Do While Line_Num < LineCount
                        strTemp = LineData(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            A = InStr(1, strTemp, "TID ")
                            If A = 0 Then
                                Load_LND.Success = False
                                Load_LND.Problem = "Tile ID missing"
                                Exit Function
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 3)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                LNDTile(Tile_Num).TID = Val(strTemp2)
                            End If

                            A = InStr(1, strTemp, "VF ")
                            If A = 0 Then
                                Load_LND.Success = False
                                Load_LND.Problem = "Tile VF missing"
                                Exit Function
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                LNDTile(Tile_Num).VF = Val(strTemp2)
                            End If

                            A = InStr(1, strTemp, "TF ")
                            If A = 0 Then
                                Load_LND.Success = False
                                Load_LND.Problem = "Tile TF missing"
                                Exit Function
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                LNDTile(Tile_Num).TF = Val(strTemp2)
                            End If

                            A = InStr(1, strTemp, " F ")
                            If A = 0 Then
                                Load_LND.Success = False
                                Load_LND.Problem = "Tile flip missing"
                                Exit Function
                            Else
                                strTemp2 = Strings.Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                LNDTile(Tile_Num).F = Val(strTemp2)
                            End If

                            A = InStr(1, strTemp, " VH ")
                            If A = 0 Then
                                Load_LND.Success = False
                                Load_LND.Problem = "Tile height is missing"
                                Exit Function
                            Else
                                strTemp3 = Right(strTemp, Len(strTemp) - A - 3)
                                For A = 0 To 2
                                    B = InStr(1, strTemp3, " ")
                                    If B = 0 Then
                                        Load_LND.Success = False
                                        Load_LND.Problem = "A tile height value is missing"
                                        Exit Function
                                    End If
                                    strTemp2 = Left(strTemp3, B - 1)
                                    strTemp3 = Right(strTemp3, Len(strTemp3) - B)

                                    If A = 0 Then
                                        LNDTile(Tile_Num).Vertex0Height = Val(strTemp2)
                                    ElseIf A = 1 Then
                                        LNDTile(Tile_Num).Vertex1Height = Val(strTemp2)
                                    ElseIf A = 2 Then
                                        LNDTile(Tile_Num).Vertex2Height = Val(strTemp2)
                                    End If
                                Next A
                                LNDTile(Tile_Num).Vertex3Height = Val(strTemp3)
                            End If

                            Tile_Num += 1
                        Else
                            GotTiles = True
                            GoTo LineDone
                        End If

                        Line_Num += 1
                    Loop

                    GotTiles = True
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Objects {")
                If A = 0 Or GotObjects Then
                Else
                    Line_Num += 1
                    Do While Line_Num < LineCount
                        strTemp = LineData(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            ObjectText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    ObjectText(C) = ObjectText(C) & strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 11 Then
                                            Load_LND.Problem = "Too many fields for an object, or a space at the end."
                                            Exit Function
                                        End If
                                        ObjectText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            ReDim Preserve LNDObject(ObjectCount)
                            With LNDObject(ObjectCount)
                                .ID = Val(ObjectText(0))
                                .TypeNum = Val(ObjectText(1))
                                .Code = Mid(ObjectText(2), 2, ObjectText(2).Length - 2) 'remove quotes
                                .PlayerNum = Val(ObjectText(3))
                                .Name = Mid(ObjectText(4), 2, ObjectText(4).Length - 2) 'remove quotes
                                .Pos.X = Val(ObjectText(5))
                                .Pos.Y = Val(ObjectText(6))
                                .Pos.Z = Val(ObjectText(7))
                                .Rotation.X = Clamp(Val(ObjectText(8)), 0.0#, 359.0#)
                                .Rotation.Y = Clamp(Val(ObjectText(9)), 0.0#, 359.0#)
                                .Rotation.Z = Clamp(Val(ObjectText(10)), 0.0#, 359.0#)
                            End With

                            ObjectCount += 1
                        Else
                            GotObjects = True
                            GoTo LineDone
                        End If

                        Line_Num += 1
                    Loop

                    GotObjects = True
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Gates {")
                If A = 0 Or GotGates Then
                Else
                    Line_Num += 1
                    Do While Line_Num < LineCount
                        strTemp = LineData(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            GateText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    GateText(C) = GateText(C) & strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 4 Then
                                            Load_LND.Problem = "Too many fields for a gateway, or a space at the end."
                                            Exit Function
                                        End If
                                        GateText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            ReDim Preserve LNDGate(LNDGateCount)
                            With LNDGate(LNDGateCount)
                                .PosA.X = Clamp(Val(GateText(0)), 0.0#, Integer.MaxValue)
                                .PosA.Y = Clamp(Val(GateText(1)), 0.0#, Integer.MaxValue)
                                .PosB.X = Clamp(Val(GateText(2)), 0.0#, Integer.MaxValue)
                                .PosB.Y = Clamp(Val(GateText(3)), 0.0#, Integer.MaxValue)
                            End With

                            LNDGateCount += 1
                        Else
                            GotGates = True
                            GoTo LineDone
                        End If

                        Line_Num += 1
                    Loop

                    GotGates = True
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Tiles {")
                If A = 0 Or GotTileTypes Or Not GotTiles Then
                Else
                    Line_Num += 1
                    Do While Line_Num < LineCount
                        strTemp = LineData(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            TileTypeText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    TileTypeText(C) = TileTypeText(C) & strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 256 Then
                                            Load_LND.Problem = "Too many fields for tile types."
                                            Exit Function
                                        End If
                                        TileTypeText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            If TileTypeText(C) = "" Or TileTypeText(C) = " " Then C = C - 1

                            For D = 0 To C
                                ReDim Preserve LNDTileType(LNDTileTypeCount)
                                LNDTileType(LNDTileTypeCount) = Clamp(Val(TileTypeText(D)), 0.0#, 11.0#)
                                LNDTileTypeCount += 1
                            Next
                        Else
                            GotTileTypes = True
                            GoTo LineDone
                        End If

                        Line_Num += 1
                    Loop

                    GotTileTypes = True
                    GoTo LineDone
                End If

LineDone:
                Line_Num += 1
            Loop

            ReDim Preserve LNDTile(Tile_Num - 1)

            SetPainterToDefaults()

            'If .HeightScale = -1 Or .TileSize = -1 Or .SizeY = -1 Or .SizeX = -1 Then Stop

            If TerrainSize.X < 1 Or TerrainSize.Y < 1 Then
                Load_LND.Success = False
                Load_LND.Problem = "The LND's terrain dimensions are missing or invalid."
                Exit Function
            End If

            Terrain_Blank(TerrainSize.X, TerrainSize.Y)
            TileType_Reset()

            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Tile_Num = Z * TerrainSize.X + X
                    'lnd uses different order! (3 = 2, 2 = 3), this program goes left to right, lnd goes clockwise around each tile
                    TerrainVertex(X, Z).Height = LNDTile(Tile_Num).Vertex0Height
                Next X
            Next Z

            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Tile_Num = Z * TerrainSize.X + X

                    TerrainTile(X, Z).Texture.TextureNum = LNDTile(Tile_Num).TID - 1

                    'ignore higher values
                    A = Int(LNDTile(Tile_Num).F / 64.0#)
                    LNDTile(Tile_Num).F = LNDTile(Tile_Num).F - A * 64

                    A = Int(LNDTile(Tile_Num).F / 16.0#)
                    LNDTile(Tile_Num).F = LNDTile(Tile_Num).F - A * 16
                    If A < 0 Or A > 3 Then
                        Load_LND.Problem = "Invalid flip value."
                        Exit Function
                    End If
                    Rotation = A

                    A = Int(LNDTile(Tile_Num).F / 8.0#)
                    LNDTile(Tile_Num).F -= A * 8
                    FlipZ = (A = 1)

                    A = Int(LNDTile(Tile_Num).F / 4.0#)
                    LNDTile(Tile_Num).F -= A * 4
                    FlipX = (A = 1)

                    A = Int(LNDTile(Tile_Num).F / 2.0#)
                    LNDTile(Tile_Num).F -= A * 2
                    TerrainTile(X, Z).Tri = (A = 1)

                    'vf, tf, ignore

                    OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, TerrainTile(X, Z).Texture.Orientation)
                Next
            Next

            Dim Unit_New As clsUnit
            Dim XYZ_int As sXYZ_int
            Dim Result As sResult

            For A = 0 To ObjectCount - 1
                Unit_New = New clsUnit
                Unit_New.ID = LNDObject(A).ID
                Result = FindUnitType(LNDObject(A).Code, LNDObject(A).TypeNum, Unit_New.Type)
                If Not Result.Success Then
                    Load_LND.Problem = Result.Problem
                    Exit Function
                End If
                Unit_New.Name = LNDObject(A).Name
                Unit_New.PlayerNum = LNDObject(A).PlayerNum
                XYZ_int.X = LNDObject(A).Pos.X
                XYZ_int.Y = LNDObject(A).Pos.Y
                XYZ_int.Z = LNDObject(A).Pos.Z
                Unit_New.Pos = MapPos_From_LNDPos(XYZ_int)
                Unit_New.Rotation = LNDObject(A).Rotation.Y
                Unit_Add(Unit_New, LNDObject(A).ID)
            Next

            GatewayCount = LNDGateCount
            ReDim Gateways(GatewayCount - 1)
            For A = 0 To LNDGateCount - 1
                Gateways(A).PosA.X = Clamp(LNDGate(A).PosA.X, 0, TerrainSize.X - 1)
                Gateways(A).PosA.Y = Clamp(LNDGate(A).PosA.Y, 0, TerrainSize.Y - 1)
                Gateways(A).PosB.X = Clamp(LNDGate(A).PosB.X, 0, TerrainSize.X - 1)
                Gateways(A).PosB.Y = Clamp(LNDGate(A).PosB.Y, 0, TerrainSize.Y - 1)
            Next

            If Tileset IsNot Nothing Then
                For A = 0 To Math.Min(LNDTileTypeCount - 1, Tileset.TileCount) - 1
                    Tile_TypeNum(A) = LNDTileType(A + 1) 'lnd value 0 is ignored
                Next
            End If

            ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
            ShadowSector_CreateAll()
            AutoTextureChange = New clsAutoTextureChange(Me)
            SectorChange = New clsSectorChange(Me)

        Catch ex As Exception
            Load_LND.Problem = ex.Message
            Exit Function
        End Try

        Load_LND.Success = True
    End Function

    Function FindUnitType(ByVal Code As String, ByVal TypeNum As Integer, ByRef OutputType As clsUnitType) As sResult
        FindUnitType.Success = False
        FindUnitType.Problem = ""

        Dim A As Integer

        For A = 0 To UnitTypeCount - 1
            If UnitTypes(A).Code = Code Then
                Exit For
            End If
        Next
        If A < UnitTypeCount Then
            OutputType = UnitTypes(A)
        Else
            For A = 0 To Unrecognised_UnitType_Count - 1
                If Unrecognised_UnitTypes(A).Code = Code Then
                    Exit For
                End If
            Next A
            If A < Unrecognised_UnitType_Count Then
                OutputType = Unrecognised_UnitTypes(A)
            Else
                OutputType = New clsUnitType(Unrecognised_UnitType_Count)

                ReDim Preserve Unrecognised_UnitTypes(Unrecognised_UnitType_Count)
                Unrecognised_UnitTypes(Unrecognised_UnitType_Count) = OutputType
                Unrecognised_UnitType_Count += 1

                With OutputType
                    .Code = Code
                    Select Case TypeNum
                        Case 0
                            .Type = clsUnitType.enumType.Feature
                        Case 1
                            .Type = clsUnitType.enumType.PlayerStructure
                        Case 2
                            .Type = clsUnitType.enumType.PlayerDroidTemplate
                        Case Else
                            FindUnitType.Problem = "Invalid object type number."
                            Exit Function
                    End Select
                End With
            End If
        End If
        FindUnitType.Success = True
    End Function

    Function Load_FME(ByVal Path As String) As sResult
        Load_FME.Success = False
        Load_FME.Problem = ""

        Try

            Dim Version_Val As UInteger

            Dim ByteFile As New clsByteReadFile
            Dim Result As sResult = ByteFile.File_Read(Path)
            If Not Result.Success Then Load_FME.Problem = Result.Problem : Exit Function

            If Not ByteFile.Get_U32(Version_Val) Then Load_FME.Problem = "Read error." : Exit Function

            If Version_Val = 1UI Then
                Load_FME.Problem = "Version 1 is no longer supported."
                Exit Function
            ElseIf Version_Val = 2UI Then
                Load_FME.Problem = "Version 2 is no longer supported."
                Exit Function
            ElseIf Version_Val = 3UI Or Version_Val = 4UI Then

                Dim byteTemp As Byte

                'tileset
                If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                If byteTemp = 0 Then
                    Tileset = Nothing
                ElseIf byteTemp = 1 Then
                    Tileset = Tileset_Arizona
                ElseIf byteTemp = 2 Then
                    Tileset = Tileset_Urban
                ElseIf byteTemp = 3 Then
                    Tileset = Tileset_Rockies
                Else
                    Load_FME.Problem = "Tileset value out of range."
                    Exit Function
                End If

                SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.

                Dim MapWidth As UShort
                Dim MapHeight As UShort

                If Not ByteFile.Get_U16(MapWidth) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_U16(MapHeight) Then Load_FME.Problem = "Read error." : Exit Function

                If MapWidth < 1US Or MapHeight < 1US Or MapWidth > 1024US Or MapHeight > 1024US Then
                    Load_FME.Problem = "Map size is invalid."
                    Exit Function
                End If

                Terrain_Blank(MapWidth, MapHeight)
                TileType_Reset()

                Dim X As Integer
                Dim Z As Integer
                Dim A As Integer
                Dim B As Integer
                Dim intTemp As Integer
                Dim Rotation As Byte
                Dim FlipX As Boolean
                Dim FlipZ As Boolean

                For Z = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X
                        If Not ByteFile.Get_U8(TerrainVertex(X, Z).Height) Then Load_FME.Problem = "Read error." : Exit Function
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainVertex(X, Z).Terrain = Nothing
                        ElseIf intTemp >= Painter.TerrainCount Then
                            Load_FME.Problem = "Terrain value out of range."
                            Exit Function
                        Else
                            TerrainVertex(X, Z).Terrain = Painter.Terrains(intTemp)
                        End If
                    Next
                Next
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        TerrainTile(X, Z).Texture.TextureNum = CInt(byteTemp) - 1

                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function

                        intTemp = 128
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Terrain_IsCliff = (A = 1)

                        intTemp = 32
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        Rotation = A

                        intTemp = 16
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        FlipX = (A = 1)

                        intTemp = 8
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        FlipZ = (A = 1)

                        OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, TerrainTile(X, Z).Texture.Orientation)

                        intTemp = 4
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Tri = (A = 1)

                        intTemp = 2
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        If TerrainTile(X, Z).Tri Then
                            TerrainTile(X, Z).TriTopLeftIsCliff = (A = 1)
                        Else
                            TerrainTile(X, Z).TriBottomLeftIsCliff = (A = 1)
                        End If

                        intTemp = 1
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        If TerrainTile(X, Z).Tri Then
                            TerrainTile(X, Z).TriBottomRightIsCliff = (A = 1)
                        Else
                            TerrainTile(X, Z).TriTopRightIsCliff = (A = 1)
                        End If

                        'attributes2
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function

                        'ignore large values - nothing should be stored there
                        intTemp = 16
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp

                        intTemp = 1
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        Select Case A
                            Case 1
                                TerrainTile(X, Z).DownSide = TileDirection_Top
                            Case 3
                                TerrainTile(X, Z).DownSide = TileDirection_Right
                            Case 5
                                TerrainTile(X, Z).DownSide = TileDirection_Bottom
                            Case 7
                                TerrainTile(X, Z).DownSide = TileDirection_Left
                            Case 8
                                TerrainTile(X, Z).DownSide = TileDirection_None
                            Case Else
                                TerrainTile(X, Z).DownSide = TileDirection_None
                                'Load_FME.Problem = "Cliff down-side is out of range."
                                'Exit Function
                        End Select
                    Next
                Next
                For Z = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X - 1
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainSideH(X, Z).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            Load_FME.Problem = "Road value out of range."
                            Exit Function
                        Else
                            TerrainSideH(X, Z).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainSideV(X, Z).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            Load_FME.Problem = "Road value out of range."
                            Exit Function
                        Else
                            TerrainSideV(X, Z).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                Dim TempUnitCount As UInteger
                ByteFile.Get_U32(TempUnitCount)
                Dim TempUnit(TempUnitCount - 1) As sWZME3Unit
                For A = 0 To TempUnitCount - 1
                    If Not ByteFile.Get_Text(40, TempUnit(A).Code) Then Load_FME.Problem = "Read error." : Exit Function
                    B = Strings.InStr(TempUnit(A).Code, Chr(0))
                    If B > 0 Then
                        TempUnit(A).Code = Strings.Left(TempUnit(A).Code, B - 1)
                    End If
                    If Not ByteFile.Get_U8(TempUnit(A).LNDType) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).ID) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).Z) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).Y) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(TempUnit(A).Rotation) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_Text_VariableLength(TempUnit(A).Name) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U8(TempUnit(A).Player) Then Load_FME.Problem = "Read error." : Exit Function
                Next

                Dim Unit_New As clsUnit
                For A = 0 To TempUnitCount - 1
                    Unit_New = New clsUnit
                    Result = FindUnitType(TempUnit(A).Code, TempUnit(A).LNDType, Unit_New.Type)
                    If Not Result.Success Then
                        Load_FME.Problem = Result.Problem
                        Exit Function
                    End If
                    Unit_New.ID = TempUnit(A).ID
                    Unit_New.Name = TempUnit(A).Name
                    Unit_New.PlayerNum = TempUnit(A).Player
                    Unit_New.Pos.X = TempUnit(A).X
                    Unit_New.Pos.Y = TempUnit(A).Y
                    Unit_New.Pos.Z = TempUnit(A).Z
                    Unit_New.Rotation = Math.Min(TempUnit(A).Rotation, 359)
                    Unit_Add(Unit_New, TempUnit(A).ID)
                Next

                ByteFile.Get_U32(GatewayCount)
                ReDim Gateways(GatewayCount - 1)
                For A = 0 To GatewayCount - 1
                    If Not ByteFile.Get_U16(Gateways(A).PosA.X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosA.Y) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosB.X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosB.Y) Then Load_FME.Problem = "Read error." : Exit Function
                Next

                If Version_Val = 4UI And Tileset IsNot Nothing Then
                    For A = 0 To 89
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        If A < Tileset.TileCount Then
                            Tile_TypeNum(A) = byteTemp
                        End If
                    Next
                End If
            ElseIf Version_Val = 5UI Or Version_Val = 6UI Then

                Dim byteTemp As Byte
                Dim uintTemp As UInteger

                'tileset
                If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                If byteTemp = 0 Then
                    Tileset = Nothing
                ElseIf byteTemp = 1 Then
                    Tileset = Tileset_Arizona
                ElseIf byteTemp = 2 Then
                    Tileset = Tileset_Urban
                ElseIf byteTemp = 3 Then
                    Tileset = Tileset_Rockies
                Else
                    Load_FME.Problem = "Tileset value out of range."
                    Exit Function
                End If

                SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.

                Dim MapWidth As UShort
                Dim MapHeight As UShort

                If Not ByteFile.Get_U16(MapWidth) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_U16(MapHeight) Then Load_FME.Problem = "Read error." : Exit Function

                If MapWidth < 1US Or MapHeight < 1US Or MapWidth > 1024US Or MapHeight > 1024US Then
                    Load_FME.Problem = "Map size is invalid."
                    Exit Function
                End If

                Terrain_Blank(MapWidth, MapHeight)
                TileType_Reset()

                Dim X As Integer
                Dim Z As Integer
                Dim A As Integer
                Dim B As Integer
                Dim intTemp As Integer

                For Z = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X
                        If Not ByteFile.Get_U8(TerrainVertex(X, Z).Height) Then Load_FME.Problem = "Read error." : Exit Function
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainVertex(X, Z).Terrain = Nothing
                        ElseIf intTemp >= Painter.TerrainCount Then
                            Load_FME.Problem = "Terrain value out of range."
                            Exit Function
                        Else
                            TerrainVertex(X, Z).Terrain = Painter.Terrains(intTemp)
                        End If
                    Next
                Next
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        TerrainTile(X, Z).Texture.TextureNum = CInt(byteTemp) - 1

                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function

                        intTemp = 128
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Terrain_IsCliff = (A = 1)

                        intTemp = 64
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Texture.Orientation.SwitchedAxes = (A = 1)

                        intTemp = 32
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Texture.Orientation.ResultXFlip = (A = 1)

                        intTemp = 16
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Texture.Orientation.ResultZFlip = (A = 1)

                        intTemp = 4
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        TerrainTile(X, Z).Tri = (A = 1)

                        intTemp = 2
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        If TerrainTile(X, Z).Tri Then
                            TerrainTile(X, Z).TriTopLeftIsCliff = (A = 1)
                        Else
                            TerrainTile(X, Z).TriBottomLeftIsCliff = (A = 1)
                        End If

                        intTemp = 1
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= A * intTemp
                        If TerrainTile(X, Z).Tri Then
                            TerrainTile(X, Z).TriBottomRightIsCliff = (A = 1)
                        Else
                            TerrainTile(X, Z).TriTopRightIsCliff = (A = 1)
                        End If

                        'attributes2
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function

                        Select Case byteTemp
                            Case 0
                                TerrainTile(X, Z).DownSide = TileDirection_None
                            Case 1
                                TerrainTile(X, Z).DownSide = TileDirection_Top
                            Case 2
                                TerrainTile(X, Z).DownSide = TileDirection_Left
                            Case 3
                                TerrainTile(X, Z).DownSide = TileDirection_Right
                            Case 4
                                TerrainTile(X, Z).DownSide = TileDirection_Bottom
                            Case Else
                                Load_FME.Problem = "Cliff down-side value out of range."
                                Exit Function
                        End Select
                    Next
                Next
                For Z = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X - 1
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainSideH(X, Z).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            Load_FME.Problem = "Road value out of range."
                            Exit Function
                        Else
                            TerrainSideH(X, Z).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            TerrainSideV(X, Z).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            Load_FME.Problem = "Road value out of range."
                            Exit Function
                        Else
                            TerrainSideV(X, Z).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                Dim TempUnitCount As UInteger
                ByteFile.Get_U32(TempUnitCount)
                Dim TempUnit(TempUnitCount - 1) As sWZME3Unit
                For A = 0 To TempUnitCount - 1
                    If Not ByteFile.Get_Text(40, TempUnit(A).Code) Then Load_FME.Problem = "Read error." : Exit Function
                    B = Strings.InStr(TempUnit(A).Code, Chr(0))
                    If B > 0 Then
                        TempUnit(A).Code = Strings.Left(TempUnit(A).Code, B - 1)
                    End If
                    If Not ByteFile.Get_U8(TempUnit(A).LNDType) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).ID) Then Load_FME.Problem = "Read error." : Exit Function
                    If Version_Val = 6UI Then
                        If Not ByteFile.Get_S32(TempUnit(A).SavePriority) Then Load_FME.Problem = "Read error." : Exit Function
                    End If
                    If Not ByteFile.Get_U32(TempUnit(A).X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).Z) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U32(TempUnit(A).Y) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(TempUnit(A).Rotation) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_Text_VariableLength(TempUnit(A).Name) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U8(TempUnit(A).Player) Then Load_FME.Problem = "Read error." : Exit Function
                Next

                Dim Unit_New As clsUnit
                For A = 0 To TempUnitCount - 1
                    Unit_New = New clsUnit
                    Result = FindUnitType(TempUnit(A).Code, TempUnit(A).LNDType, Unit_New.Type)
                    If Not Result.Success Then
                        Load_FME.Problem = Result.Problem
                        Exit Function
                    End If
                    Unit_New.ID = TempUnit(A).ID
                    Unit_New.SavePriority = TempUnit(A).SavePriority
                    Unit_New.Name = TempUnit(A).Name
                    Unit_New.PlayerNum = TempUnit(A).Player
                    Unit_New.Pos.X = TempUnit(A).X
                    Unit_New.Pos.Y = TempUnit(A).Y
                    Unit_New.Pos.Z = TempUnit(A).Z
                    Unit_New.Rotation = Math.Min(CInt(TempUnit(A).Rotation), 359)
                    Unit_Add(Unit_New, TempUnit(A).ID)
                Next

                ByteFile.Get_U32(GatewayCount)
                ReDim Gateways(GatewayCount - 1)
                For A = 0 To GatewayCount - 1
                    If Not ByteFile.Get_U16(Gateways(A).PosA.X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosA.Y) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosB.X) Then Load_FME.Problem = "Read error." : Exit Function
                    If Not ByteFile.Get_U16(Gateways(A).PosB.Y) Then Load_FME.Problem = "Read error." : Exit Function
                Next

                If Tileset IsNot Nothing Then
                    For A = 0 To Tileset.TileCount - 1
                        If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                        Tile_TypeNum(A) = byteTemp
                    Next
                End If

                'scroll limits
                If Not ByteFile.Get_S32(intTemp) Then Load_FME.Problem = "Read error." : Exit Function
                frmCompileInstance.txtCampMinX.Text = intTemp
                If Not ByteFile.Get_S32(intTemp) Then Load_FME.Problem = "Read error." : Exit Function
                frmCompileInstance.txtCampMinY.Text = intTemp
                If Not ByteFile.Get_U32(uintTemp) Then Load_FME.Problem = "Read error." : Exit Function
                frmCompileInstance.txtCampMaxX.Text = uintTemp
                If Not ByteFile.Get_U32(uintTemp) Then Load_FME.Problem = "Read error." : Exit Function
                frmCompileInstance.txtCampMaxY.Text = uintTemp

                'other compile info

                If Not ByteFile.Get_Text_VariableLength(frmCompileInstance.txtName.Text) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                Select Case byteTemp
                    Case 0
                        frmCompileInstance.rdoMulti.Checked = False
                        frmCompileInstance.rdoCamp.Checked = False
                    Case 1
                        frmCompileInstance.rdoMulti.Checked = True
                    Case 2
                        frmCompileInstance.rdoCamp.Checked = True
                    Case Else
                        Load_FME.Problem = "Compile type out of range."
                        Exit Function
                End Select
                If Not ByteFile.Get_Text_VariableLength(frmCompileInstance.txtMultiPlayers.Text) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_U8(byteTemp) Then Load_FME.Problem = "Read error." : Exit Function
                Select Case byteTemp
                    Case 0
                        frmCompileInstance.chkNewPlayerFormat.Checked = False
                    Case 1
                        frmCompileInstance.chkNewPlayerFormat.Checked = True
                    Case Else
                        Load_FME.Problem = "Compile player format out of range."
                        Exit Function
                End Select
                If Not ByteFile.Get_Text_VariableLength(frmCompileInstance.txtAuthor.Text) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_Text_VariableLength(frmCompileInstance.cmbLicense.Text) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_Text_VariableLength(frmCompileInstance.txtCampTime.Text) Then Load_FME.Problem = "Read error." : Exit Function
                If Not ByteFile.Get_S32(intTemp) Then Load_FME.Problem = "Read error." : Exit Function
                If intTemp < -1 Or intTemp >= frmCompileInstance.cmbCampType.Items.Count Then
                    Load_FME.Problem = "Compile campaign type is out of range."
                    Exit Function
                End If
                frmCompileInstance.cmbCampType.SelectedIndex = intTemp
            Else
                Load_FME.Problem = "File version number is not recognised."
                Exit Function
            End If

            ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
            ShadowSector_CreateAll()
            AutoTextureChange = New clsAutoTextureChange(Me)
            SectorChange = New clsSectorChange(Me)

        Catch ex As Exception
            Load_FME.Problem = ex.Message
            Exit Function
        End Try

        Load_FME.Success = True
    End Function

    Sub Minimap_Texture_Fill(ByRef Texture(,,) As Byte)
        Static X As Integer
        Static Z As Integer
        Static A As Integer
        Static Low As sXY_int
        Static High As sXY_int
        Static Footprint As sXY_int
        Static Flag As Boolean
        Static RGB_sng As sRGB_sng

        For Z = 0 To Texture.GetUpperBound(0)
            For X = 0 To Texture.GetUpperBound(1)
                Texture(Z, X, 3) = 255
            Next
        Next
        If frmMainInstance.menuMiniShowTex.Checked Then
            If Tileset IsNot Nothing Then
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If TerrainTile(X, Z).Texture.TextureNum >= 0 And TerrainTile(X, Z).Texture.TextureNum < Tileset.TileCount Then
                            RGB_sng = Tileset.Tiles(TerrainTile(X, Z).Texture.TextureNum).Average_Color
                            Texture(Z, X, 0) = Math.Min(CInt(RGB_sng.Red * 255.0F), 255)
                            Texture(Z, X, 1) = Math.Min(CInt(RGB_sng.Green * 255.0F), 255)
                            Texture(Z, X, 2) = Math.Min(CInt(RGB_sng.Blue * 255.0F), 255)
                        End If
                    Next
                Next
            End If
            If frmMainInstance.menuMiniShowHeight.Checked Then
                Dim Height As Short
                For Z = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        Height = (CShort(TerrainVertex(X, Z).Height) + TerrainVertex(X + 1, Z).Height + TerrainVertex(X, Z + 1).Height + TerrainVertex(X + 1, Z + 1).Height) / 4.0#
                        Texture(Z, X, 0) = (Texture(Z, X, 0) * 2S + Height) / 3.0#
                        Texture(Z, X, 1) = (Texture(Z, X, 1) * 2S + Height) / 3.0#
                        Texture(Z, X, 2) = (Texture(Z, X, 2) * 2S + Height) / 3.0#
                    Next
                Next
            End If
        ElseIf frmMainInstance.menuMiniShowHeight.Checked Then
            Dim Height As Short
            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Height = (CShort(TerrainVertex(X, Z).Height) + TerrainVertex(X + 1, Z).Height + TerrainVertex(X, Z + 1).Height + TerrainVertex(X + 1, Z + 1).Height) / 4.0#
                    Texture(Z, X, 0) = Height
                    Texture(Z, X, 1) = Height
                    Texture(Z, X, 2) = Height
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowGateways.Checked Then
            For A = 0 To GatewayCount - 1
                XY_Reorder(Gateways(A).PosA, Gateways(A).PosB, Low, High)
                For Z = Low.Y To High.Y
                    For X = Low.X To High.X
                        Texture(Z, X, 0) = 255
                        Texture(Z, X, 1) = 255
                        Texture(Z, X, 2) = 0
                    Next
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowUnits.Checked Then
            For A = 0 To UnitCount - 1
                Flag = False
                If Units(A).Type.LoadedInfo IsNot Nothing Then
                    Footprint = Units(A).Type.LoadedInfo.Footprint
                    If Footprint.X < 1 Then Footprint.X = 1
                    If Footprint.Y < 1 Then Footprint.Y = 1
                    'highlight unit if selected
                    If frmMainInstance.lstFeatures.SelectedIndex >= 0 Then
                        If frmMainInstance.lstFeatures_Unit(frmMainInstance.lstFeatures.SelectedIndex) Is Units(A).Type Then
                            Flag = True
                        End If
                    ElseIf frmMainInstance.lstStructures.SelectedIndex >= 0 Then
                        If frmMainInstance.lstStructures_Unit(frmMainInstance.lstStructures.SelectedIndex) Is Units(A).Type Then
                            Flag = True
                        End If
                    ElseIf frmMainInstance.lstDroids.SelectedIndex >= 0 Then
                        If frmMainInstance.lstDroids_Unit(frmMainInstance.lstDroids.SelectedIndex) Is Units(A).Type Then
                            Flag = True
                        End If
                    End If
                    If Flag Then
                        Footprint.X += 2
                        Footprint.Y += 2
                    End If
                Else
                    Footprint.X = 1
                    Footprint.Y = 1
                End If
                Low.X = Math.Max(Int(Units(A).Pos.X / TerrainGridSpacing - Footprint.X / 2.0#), 0)
                Low.Y = Math.Max(Int(Units(A).Pos.Z / TerrainGridSpacing - Footprint.Y / 2.0#), 0)
                High.X = Math.Min(Int((Units(A).Pos.X - 1) / TerrainGridSpacing + Footprint.X / 2.0#), TerrainSize.X - 1)
                High.Y = Math.Min(Int((Units(A).Pos.Z - 1) / TerrainGridSpacing + Footprint.Y / 2.0#), TerrainSize.Y - 1)
                If Flag Then
                    For Z = Low.Y To High.Y
                        For X = Low.X To High.X
                            Texture(Z, X, 0) = (CShort(Texture(Z, X, 0)) + 510S) / 3.0#
                            Texture(Z, X, 1) = (CShort(Texture(Z, X, 1)) + 510S) / 3.0#
                            Texture(Z, X, 2) = (CShort(Texture(Z, X, 2)) + 510S) / 3.0#
                        Next
                    Next
                Else
                    For Z = Low.Y To High.Y
                        For X = Low.X To High.X
                            Texture(Z, X, 0) = (CShort(Texture(Z, X, 0)) + 510S) / 3.0#
                            Texture(Z, X, 1) = (CShort(Texture(Z, X, 1)) + 0S) / 3.0#
                            Texture(Z, X, 2) = (CShort(Texture(Z, X, 2)) + 0S) / 3.0#
                        Next
                    Next
                End If
            Next
        End If
    End Sub

#If OS <> 0.0# Then
    Private MinimapBitmap As Bitmap
#End If
    Private MinimapPending As Boolean
    Private WithEvents MakeMinimapTimer As Timer
    Public SuppressMinimap As Boolean

    Sub MinimapTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Makeminimaptimer.tick

        If MinimapPending Then
            If SuppressMinimap Then
                'should restart the timer here, but i cant find a good way to
            Else
                MinimapPending = False
                Minimap_Make_Now()
            End If
        Else
            MakeMinimapTimer.Enabled = False
        End If
    End Sub

    Sub Minimap_Make()

        If MakeMinimapTimer.enabled Then
            MinimapPending = True
        Else
            MakeMinimapTimer.enabled = True
            If SuppressMinimap Then
                MinimapPending = True
            Else
                Minimap_Make_Now()
            End If
        End If
    End Sub

    Sub Minimap_Make_Now()

        Dim NewTextureSize As Integer = Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(TerrainSize.X, TerrainSize.Y)) / Math.Log(2.0#)))

        If NewTextureSize <> Minimap_Texture_Size Then
            Minimap_Texture_Size = NewTextureSize
#If OS <> 0.0# Then
            MinimapBitmap = New Bitmap(Minimap_Texture_Size, Minimap_Texture_Size)
#End If
        End If

        Dim Size As Integer = Minimap_Texture_Size - 1

        Dim Pixels(Size, Size, 3) As Byte

        Minimap_Texture_Fill(Pixels)

#If OS <> 0.0# Then
        Dim Texture As New clsFileBitmap

        Texture.CurrentBitmap = MinimapBitmap

        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Size
            For X = 0 To Size
                Texture.CurrentBitmap.SetPixel(X, Y, ColorTranslator.FromOle(OSRGB(Pixels(Y, X, 0), Pixels(Y, X, 1), Pixels(Y, X, 2))))
            Next
        Next
#End If

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        If Minimap_Texture > 0 Then
            GL.DeleteTextures(1, Minimap_Texture)
            Minimap_Texture = 0
        End If

#If OS = 0.0# Then
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.GenTextures(1, Minimap_Texture)
        GL.BindTexture(TextureTarget.Texture2D, Minimap_Texture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Pixels)
#Else
        Minimap_Texture = Texture.GL_Texture_Create
#End If

        frmMainInstance.DrawView()
    End Sub

    Sub Tile_AutoTexture_Changed(ByVal X As Integer, ByVal Z As Integer)
        Static Terrain_Inner As sPainter.clsTerrain
        Static Terrain_Outer As sPainter.clsTerrain
        Static Road As sPainter.clsRoad
        Static A As Integer
        Static Brush_Num As Integer
        Static RoadTop As Boolean
        Static RoadLeft As Boolean
        Static RoadRight As Boolean
        Static RoadBottom As Boolean

        'apply centre brushes
        If Not TerrainTile(X, Z).Terrain_IsCliff Then
            For Brush_Num = 0 To Painter.TerrainCount - 1
                Terrain_Inner = Painter.Terrains(Brush_Num)
                If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i i i i
                                TerrainTile(X, Z).Texture = OrientateTile(Terrain_Inner.Tiles.GetRandom(), TileDirection_None)
                            End If
                        End If
                    End If
                End If
            Next Brush_Num
        End If

        'apply transition brushes
        If Not TerrainTile(X, Z).Terrain_IsCliff Then
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
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i i o i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i i o o
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                                Exit For
                            End If
                        End If
                    ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i o i i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopRight)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i o i o
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'i o o i
                                TerrainTile(X, Z).Texture.TextureNum = -1
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'i o o o
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        End If
                    End If
                ElseIf TerrainVertex(X, Z).Terrain Is Terrain_Outer Then
                    If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o i i i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopLeft)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o i i o
                                TerrainTile(X, Z).Texture.TextureNum = -1
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o i o i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o i o o
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            End If
                        End If
                    ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o o i i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                                Exit For
                            ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                                'o o i o
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                                'o o o i
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopLeft)
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
        If TerrainTile(X, Z).Tri Then
            If TerrainTile(X, Z).TriTopLeftIsCliff Then
                If TerrainTile(X, Z).TriBottomRightIsCliff Then
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
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TerrainTile(X, Z).DownSide)
                                Exit For
                            End If
                        End If
                        If ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                            Exit For
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        TerrainTile(X, Z).Texture.TextureNum = -1
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
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopLeft)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X, Z).Terrain Is Terrain_Inner Then
                            A = 0
                            If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomRight)
                                Exit For
                            End If
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        TerrainTile(X, Z).Texture.TextureNum = -1
                    End If
                End If
            ElseIf TerrainTile(X, Z).TriBottomRightIsCliff Then
                For Brush_Num = 0 To Painter.CliffBrushCount - 1
                    Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                    If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then A += 1
                        If A >= 2 Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomRight)
                            Exit For
                        End If
                    ElseIf TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                        If A >= 2 Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopLeft)
                            Exit For
                        End If
                    End If
                Next Brush_Num
                If Brush_Num = Painter.CliffBrushCount Then
                    TerrainTile(X, Z).Texture.TextureNum = -1
                End If
            Else
                'no cliff
            End If
        Else
            'default tri orientation
            If TerrainTile(X, Z).TriTopRightIsCliff Then
                If TerrainTile(X, Z).TriBottomLeftIsCliff Then
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
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TerrainTile(X, Z).DownSide)
                                Exit For
                            End If
                        End If
                        If ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Bottom)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Left)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer) And (TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Top)
                            Exit For
                        ElseIf ((TerrainVertex(X, Z).Terrain Is Terrain_Inner And TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Or TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Or ((TerrainVertex(X, Z).Terrain Is Terrain_Inner Or TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner) And (TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer And TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer)) Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Straight.GetRandom(), TileDirection_Right)
                            Exit For
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        TerrainTile(X, Z).Texture.TextureNum = -1
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
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_TopRight)
                                Exit For
                            End If
                        ElseIf TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then
                            A = 0
                            If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_BottomLeft)
                                Exit For
                            End If
                        End If
                    Next Brush_Num
                    If Brush_Num = Painter.CliffBrushCount Then
                        TerrainTile(X, Z).Texture.TextureNum = -1
                    End If
                End If
            ElseIf TerrainTile(X, Z).TriBottomLeftIsCliff Then
                For Brush_Num = 0 To Painter.CliffBrushCount - 1
                    Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                    If TerrainVertex(X, Z + 1).Terrain Is Terrain_Outer Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Inner Then A += 1
                        If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Inner Then A += 1
                        If A >= 2 Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_In.GetRandom(), TileDirection_BottomLeft)
                            Exit For
                        End If
                    ElseIf TerrainVertex(X, Z + 1).Terrain Is Terrain_Inner Then
                        A = 0
                        If TerrainVertex(X, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z).Terrain Is Terrain_Outer Then A += 1
                        If TerrainVertex(X + 1, Z + 1).Terrain Is Terrain_Outer Then A += 1
                        If A >= 2 Then
                            TerrainTile(X, Z).Texture = OrientateTile(Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out.GetRandom(), TileDirection_TopRight)
                            Exit For
                        End If
                    End If
                Next Brush_Num
                If Brush_Num = Painter.CliffBrushCount Then
                    TerrainTile(X, Z).Texture.TextureNum = -1
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

            TerrainTile(X, Z).Texture.TextureNum = -1

            If Brush_Num < Painter.RoadBrushCount Then
                RoadTop = (TerrainSideH(X, Z).Road Is Road)
                RoadLeft = (TerrainSideV(X, Z).Road Is Road)
                RoadRight = (TerrainSideV(X + 1, Z).Road Is Road)
                RoadBottom = (TerrainSideH(X, Z + 1).Road Is Road)
                'do cross intersection
                If RoadTop And RoadLeft And RoadRight And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_CrossIntersection.GetRandom(), TileDirection_None)
                    'do T intersection
                ElseIf RoadTop And RoadLeft And RoadRight Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Top)
                ElseIf RoadTop And RoadLeft And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Left)
                ElseIf RoadTop And RoadRight And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Right)
                ElseIf RoadLeft And RoadRight And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_TIntersection.GetRandom(), TileDirection_Bottom)
                    'do straight
                ElseIf RoadTop And RoadBottom Then
                    If Rnd() >= 0.5F Then
                        TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Top)
                    Else
                        TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Bottom)
                    End If
                ElseIf RoadLeft And RoadRight Then
                    If Rnd() >= 0.5F Then
                        TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Left)
                    Else
                        TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Straight.GetRandom(), TileDirection_Right)
                    End If
                    'do corner
                ElseIf RoadTop And RoadLeft Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_TopLeft)
                ElseIf RoadTop And RoadRight Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_TopRight)
                ElseIf RoadLeft And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_BottomLeft)
                ElseIf RoadRight And RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_Corner_In.GetRandom(), TileDirection_BottomRight)
                    'do end
                ElseIf RoadTop Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Top)
                ElseIf RoadLeft Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Left)
                ElseIf RoadRight Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Right)
                ElseIf RoadBottom Then
                    TerrainTile(X, Z).Texture = OrientateTile(Painter.RoadBrushes(Brush_Num).Tile_End.GetRandom(), TileDirection_Bottom)
                End If
            End If
        End If
    End Sub

    Sub Clear_Textures()
        Dim X As Integer
        Dim Z As Integer

        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTile(X, Z).Texture.TextureNum = -1
            Next
        Next
        For Z = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                TerrainVertex(X, Z).Terrain = Nothing
            Next
        Next
        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                Tile_AutoTexture_Changed(X, Z)
            Next
        Next
        For Z = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X - 1
                TerrainSideH(X, Z).Road = Nothing
            Next
        Next
        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X
                TerrainSideV(X, Z).Road = Nothing
            Next
        Next
    End Sub

    Public Structure sWrite_WZ_Args
        Public Path As String
        Public Overwrite As Boolean
        Public MapName As String
        Public Class clsMultiplayer
            Public PlayerCount As Integer
            Public AuthorName As String
            Public License As String
            Public IsBetaPlayerFormat As Boolean
        End Class
        Public Multiplayer As clsMultiplayer
        Public Class clsCampaign
            Public GAMTime As UInteger
            Public GAMType As UInteger
        End Class
        Public Campaign As clsCampaign
        Enum enumCompileType As Byte
            Multiplayer
            Campaign
        End Enum
        Public ScrollMin As sXY_int
        Public ScrollMax As sXY_uint
        Public CompileType As enumCompileType
    End Structure

    Function Write_WZ(ByVal Args As sWrite_WZ_Args) As sResult
        Write_WZ.Success = False
        Write_WZ.Problem = ""

        Try

            Select Case Args.CompileType
                Case sWrite_WZ_Args.enumCompileType.Multiplayer
                    If Args.Multiplayer Is Nothing Then
                        Write_WZ.Problem = "Multiplayer arguments were not passed."
                        Exit Function
                    End If
                    If Args.Multiplayer.PlayerCount < 2 Or Args.Multiplayer.PlayerCount > 10 Then
                        Write_WZ.Problem = "Number of players was below 2 or above 10."
                        Exit Function
                    End If
                    If Not Args.Multiplayer.IsBetaPlayerFormat Then
                        If Not (Args.Multiplayer.PlayerCount = 2 Or Args.Multiplayer.PlayerCount = 4 Or Args.Multiplayer.PlayerCount = 8) Then
                            Write_WZ.Problem = "Number of players was not 2, 4 or 8 in original format."
                            Exit Function
                        End If
                    End If
                Case sWrite_WZ_Args.enumCompileType.Campaign
                    If Args.Campaign Is Nothing Then
                        Write_WZ.Problem = "Campaign arguments were not passed."
                        Exit Function
                    End If
                Case Else
                    Write_WZ.Problem = "Unknown compile method."
                    Exit Function
            End Select

            If Not Args.Overwrite Then
                If IO.File.Exists(Args.Path) Then
                    Write_WZ.Problem = "The selected file already exists."
                    Exit Function
                End If
            End If

            Dim Quote As String = Chr(34)
            Dim EndChar As String = Chr(10)
            Dim Text As String

            Dim File_LEV As New clsByteWriteFile
            Dim File_MAP As New clsByteWriteFile
            Dim File_GAM As New clsByteWriteFile
            Dim File_featBJO As New clsByteWriteFile
            Dim File_TTP As New clsByteWriteFile
            Dim File_structBJO As New clsByteWriteFile
            Dim File_droidBJO As New clsByteWriteFile

            Dim PlayersPrefix As String = ""

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                PlayersPrefix = Args.Multiplayer.PlayerCount & "c-"
                Dim fog As String
                Dim TilesetNum As String
                If Tileset Is Tileset_Arizona Then
                    fog = "fog1.wrf"
                    TilesetNum = "1"
                ElseIf Tileset Is Tileset_Urban Then
                    fog = "fog2.wrf"
                    TilesetNum = "2"
                ElseIf Tileset Is Tileset_Rockies Then
                    fog = "fog3.wrf"
                    TilesetNum = "3"
                Else
                    Write_WZ.Problem = "Unknown tileset selected."
                    Exit Function
                End If

                Text = "// Made with flaME v" & ProgramVersion & EndChar
                File_LEV.Text_Append(Text)
                Dim DateNow As Date = Now
                Text = "// Date: " & DateNow.Year & "/" & MinDigits(DateNow.Month, 2) & "/" & MinDigits(DateNow.Day, 2) & " " & MinDigits(DateNow.Hour, 2) & ":" & MinDigits(DateNow.Minute, 2) & ":" & MinDigits(DateNow.Second, 2) & EndChar
                File_LEV.Text_Append(Text)
                Text = "// Author: " & Args.Multiplayer.AuthorName & EndChar
                File_LEV.Text_Append(Text)
                Text = "// License: " & Args.Multiplayer.License & EndChar
                File_LEV.Text_Append(Text)
                Text = EndChar
                File_LEV.Text_Append(Text)
                Text = "level   " & Args.MapName & "-T1" & EndChar
                File_LEV.Text_Append(Text)
                Text = "players " & Args.Multiplayer.PlayerCount & EndChar
                File_LEV.Text_Append(Text)
                Text = "type    14" & EndChar
                File_LEV.Text_Append(Text)
                Text = "dataset MULTI_CAM_" & TilesetNum & EndChar
                File_LEV.Text_Append(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/skirmish" & Args.Multiplayer.PlayerCount & ".wrf" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = EndChar
                File_LEV.Text_Append(Text)
                Text = "level   " & Args.MapName & "-T2" & EndChar
                File_LEV.Text_Append(Text)
                Text = "players " & Args.Multiplayer.PlayerCount & EndChar
                File_LEV.Text_Append(Text)
                Text = "type    18" & EndChar
                File_LEV.Text_Append(Text)
                Text = "dataset MULTI_T2_C" & TilesetNum & EndChar
                File_LEV.Text_Append(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/t2-skirmish" & Args.Multiplayer.PlayerCount & ".wrf" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = EndChar
                File_LEV.Text_Append(Text)
                Text = "level   " & Args.MapName & "-T3" & EndChar
                File_LEV.Text_Append(Text)
                Text = "players " & Args.Multiplayer.PlayerCount & EndChar
                File_LEV.Text_Append(Text)
                Text = "type    19" & EndChar
                File_LEV.Text_Append(Text)
                Text = "dataset MULTI_T3_C" & TilesetNum & EndChar
                File_LEV.Text_Append(Text)
                Text = "game    " & Quote & "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/t3-skirmish" & Args.Multiplayer.PlayerCount & ".wrf" & Quote & EndChar
                File_LEV.Text_Append(Text)
                Text = "data    " & Quote & "wrf/multi/" & fog & Quote & EndChar
                File_LEV.Text_Append(Text)

            End If

            File_GAM.U8_Append(Asc("g"))
            File_GAM.U8_Append(Asc("a"))
            File_GAM.U8_Append(Asc("m"))
            File_GAM.U8_Append(Asc("e"))
            File_GAM.U32_Append(8)
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                File_GAM.U32_Append(0)
                File_GAM.U32_Append(0)
                'File_GAM.S32_Append(0)
                'File_GAM.S32_Append(0)
                'File_GAM.U32_Append(TerrainSize.X)
                'File_GAM.U32_Append(TerrainSize.Y)
            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                File_GAM.U32_Append(Args.Campaign.GAMTime)
                File_GAM.U32_Append(Args.Campaign.GAMType)
            End If
            File_GAM.S32_Append(Args.ScrollMin.X)
            File_GAM.S32_Append(Args.ScrollMin.Y)
            File_GAM.U32_Append(Args.ScrollMax.X)
            File_GAM.U32_Append(Args.ScrollMax.Y)
            File_GAM.Make_Length(20)

            Dim A As Integer
            Dim B As Integer
            Dim X As Integer
            Dim Y As Integer

            File_MAP.U8_Append(Asc("m"))
            File_MAP.U8_Append(Asc("a"))
            File_MAP.U8_Append(Asc("p"))
            File_MAP.U8_Append(Asc(" "))
            File_MAP.U32_Append(10)
            File_MAP.U32_Append(TerrainSize.X)
            File_MAP.U32_Append(TerrainSize.Y)
            Dim Flip As Byte
            Dim Rotation As Byte
            Dim DoFlipX As Boolean
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    TileOrientation_To_OldOrientation(TerrainTile(X, Y).Texture.Orientation, Rotation, DoFlipX)
                    Flip = 0
                    If TerrainTile(X, Y).Tri Then
                        Flip += 8
                    End If
                    Flip += Rotation * 16
                    If DoFlipX Then
                        Flip += 128
                    End If
                    File_MAP.U8_Append(Clamp(TerrainTile(X, Y).Texture.TextureNum, 0, 255))
                    File_MAP.U8_Append(Flip)
                    File_MAP.U8_Append(TerrainVertex(X, Y).Height)
                Next
            Next
            File_MAP.U32_Append(1) 'gateway version
            File_MAP.U32_Append(GatewayCount)
            For A = 0 To GatewayCount - 1
                File_MAP.U8_Append(Clamp(Gateways(A).PosA.X, 0, 255))
                File_MAP.U8_Append(Clamp(Gateways(A).PosA.Y, 0, 255))
                File_MAP.U8_Append(Clamp(Gateways(A).PosB.X, 0, 255))
                File_MAP.U8_Append(Clamp(Gateways(A).PosB.Y, 0, 255))
            Next

            File_featBJO.U8_Append(Asc("f"))
            File_featBJO.U8_Append(Asc("e"))
            File_featBJO.U8_Append(Asc("a"))
            File_featBJO.U8_Append(Asc("t"))
            File_featBJO.U32_Append(8)
            Dim Features(UnitCount - 1) As Integer
            Dim FeatureCount As Integer = 0
            Dim C As Integer
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.Feature Then
                    For B = 0 To FeatureCount - 1
                        If Units(Features(B)).SavePriority < Units(A).SavePriority Then
                            Exit For
                        End If
                    Next
                    For C = FeatureCount - 1 To B Step -1
                        Features(C + 1) = Features(C)
                    Next
                    Features(B) = A
                    FeatureCount += 1
                End If
            Next
            File_featBJO.U32_Append(FeatureCount)
            For B = 0 To FeatureCount - 1
                A = Features(B)
                File_featBJO.Text_Append(Units(A).Type.Code, 40)
                File_featBJO.U32_Append(Units(A).ID)
                File_featBJO.U32_Append(Units(A).Pos.X)
                File_featBJO.U32_Append(Units(A).Pos.Z)
                File_featBJO.U32_Append(Units(A).Pos.Y)
                File_featBJO.U32_Append(Units(A).Rotation)
                File_featBJO.U32_Append(Units(A).PlayerNum)
                File_featBJO.Make_Length(12)
            Next

            File_TTP.Text_Append("ttyp")
            File_TTP.U32_Append(8UI)
            File_TTP.U32_Append(Map.Tileset.TileCount)
            For A = 0 To Map.Tileset.TileCount - 1
                File_TTP.U16_Append(Map.Tile_TypeNum(A))
            Next

            File_structBJO.U8_Append(Asc("s"))
            File_structBJO.U8_Append(Asc("t"))
            File_structBJO.U8_Append(Asc("r"))
            File_structBJO.U8_Append(Asc("u"))
            File_structBJO.U32_Append(8)
            Dim TempStructs(UnitCount - 1) As Integer
            Dim TempStructCount As Integer = 0
            Dim AddToList As Boolean
            'non-module structures
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    AddToList = False
                    If Units(A).Type.LoadedInfo IsNot Nothing Then
                        If Not (Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.FactoryModule _
                          Or Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.PowerModule _
                          Or Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.ResearchModule) Then
                            AddToList = True
                        End If
                    Else
                        AddToList = True
                    End If
                    If AddToList Then
                        For B = 0 To TempStructCount - 1
                            If Units(TempStructs(B)).SavePriority < Units(A).SavePriority Then
                                Exit For
                            End If
                        Next
                        For C = TempStructCount - 1 To B Step -1
                            TempStructs(C + 1) = TempStructs(C)
                        Next
                        TempStructs(B) = A
                        TempStructCount += 1
                    End If
                End If
            Next
            'module structures
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    If Units(A).Type.LoadedInfo IsNot Nothing Then
                        If Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.FactoryModule _
                          Or Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.PowerModule _
                          Or Units(A).Type.LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.ResearchModule Then
                            For B = 0 To TempStructCount - 1
                                If Units(TempStructs(B)).SavePriority < Units(A).SavePriority Then
                                    Exit For
                                End If
                            Next
                            For C = TempStructCount - 1 To B Step -1
                                TempStructs(C + 1) = TempStructs(C)
                            Next
                            TempStructs(B) = A
                            TempStructCount += 1
                        End If
                    End If
                End If
            Next
            File_structBJO.U32_Append(TempStructCount)
            For B = 0 To TempStructCount - 1
                A = TempStructs(B)
                File_structBJO.Text_Append(Units(A).Type.Code, 40)
                File_structBJO.U32_Append(Units(A).ID)
                File_structBJO.U32_Append(Units(A).Pos.X)
                File_structBJO.U32_Append(Units(A).Pos.Z)
                File_structBJO.U32_Append(Units(A).Pos.Y)
                File_structBJO.U32_Append(Units(A).Rotation)
                File_structBJO.U32_Append(Units(A).PlayerNum)
                File_structBJO.Make_Length(12)
                File_structBJO.U8_Append(1)
                File_structBJO.U8_Append(26)
                File_structBJO.U8_Append(127)
                File_structBJO.U8_Append(0)
                File_structBJO.Make_Length(40)
            Next

            File_droidBJO.U8_Append(Asc("d"))
            File_droidBJO.U8_Append(Asc("i"))
            File_droidBJO.U8_Append(Asc("n"))
            File_droidBJO.U8_Append(Asc("t"))
            File_droidBJO.U32_Append(8)
            Dim Droids(UnitCount - 1) As Integer
            Dim DroidCount As Integer = 0
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerDroidTemplate Then
                    For B = 0 To DroidCount - 1
                        If Units(Droids(B)).SavePriority < Units(A).SavePriority Then
                            Exit For
                        End If
                    Next
                    For C = DroidCount - 1 To B Step -1
                        Droids(C + 1) = Droids(C)
                    Next
                    Droids(B) = A
                    DroidCount += 1
                End If
            Next
            File_droidBJO.U32_Append(DroidCount)
            For B = 0 To DroidCount - 1
                A = Droids(B)
                File_droidBJO.Text_Append(Units(A).Type.Code, 40)
                File_droidBJO.U32_Append(Units(A).ID)
                File_droidBJO.U32_Append(Units(A).Pos.X)
                File_droidBJO.U32_Append(Units(A).Pos.Z)
                File_droidBJO.U32_Append(Units(A).Pos.Y)
                File_droidBJO.U32_Append(Units(A).Rotation)
                File_droidBJO.U32_Append(Units(A).PlayerNum)
                File_droidBJO.Make_Length(12)
            Next

            File_LEV.Trim_Buffer()
            File_GAM.Trim_Buffer()
            File_droidBJO.Trim_Buffer()
            File_featBJO.Trim_Buffer()
            File_MAP.Trim_Buffer()
            File_structBJO.Trim_Buffer()
            File_TTP.Trim_Buffer()

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                If Not Args.Overwrite Then
                    If IO.File.Exists(Args.Path) Then
                        Write_WZ.Problem = "File already exists. Will not overwrite."
                        Exit Function
                    End If
                Else
                    If IO.File.Exists(Args.Path) Then
                        IO.File.Delete(Args.Path)
                    End If
                End If

                Dim WZStream As Zip.ZipOutputStream = New Zip.ZipOutputStream(IO.File.Create(Args.Path))

                Try

                    Dim Crc32 As New ICSharpCode.SharpZipLib.Checksums.Crc32
                    Dim ZippedFile As Zip.ZipEntry

                    WZStream.SetLevel(9)

                    If Args.Multiplayer.IsBetaPlayerFormat Then
                        ZippedFile = New Zip.ZipEntry(PlayersPrefix & Args.MapName & ".xplayers.lev")
                    Else
                        ZippedFile = New Zip.ZipEntry(PlayersPrefix & Args.MapName & ".addon.lev")
                    End If
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_LEV.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_LEV.Bytes, 0, File_LEV.ByteCount)
                    Crc32.Update(File_LEV.Bytes, 0, File_LEV.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/")
                    WZStream.PutNextEntry(ZippedFile)
                    ZippedFile = New Zip.ZipEntry("multiplay/maps/")
                    WZStream.PutNextEntry(ZippedFile)
                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/")
                    WZStream.PutNextEntry(ZippedFile)

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_GAM.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_GAM.Bytes, 0, File_GAM.ByteCount)
                    Crc32.Update(File_GAM.Bytes, 0, File_GAM.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "dinit.bjo")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_droidBJO.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_droidBJO.Bytes, 0, File_droidBJO.ByteCount)
                    Crc32.Update(File_droidBJO.Bytes, 0, File_droidBJO.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "feat.bjo")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_featBJO.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_featBJO.Bytes, 0, File_featBJO.ByteCount)
                    Crc32.Update(File_featBJO.Bytes, 0, File_featBJO.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "game.map")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_MAP.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_MAP.Bytes, 0, File_MAP.ByteCount)
                    Crc32.Update(File_MAP.Bytes, 0, File_MAP.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "struct.bjo")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_structBJO.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_structBJO.Bytes, 0, File_structBJO.ByteCount)
                    Crc32.Update(File_structBJO.Bytes, 0, File_structBJO.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "ttypes.ttp")
                    ZippedFile.DateTime = Now
                    ZippedFile.Size = File_TTP.ByteCount
                    ZippedFile.ExternalFileAttributes = 32
                    Crc32.Reset()
                    WZStream.PutNextEntry(ZippedFile)
                    WZStream.Write(File_TTP.Bytes, 0, File_TTP.ByteCount)
                    Crc32.Update(File_TTP.Bytes, 0, File_TTP.ByteCount)
                    ZippedFile.Crc = Crc32.Value

                    WZStream.Finish()
                Catch ex As Exception
                    WZStream.Close()
                    Write_WZ.Problem = ex.Message
                    Exit Function
                Finally
                    WZStream.Close()
                End Try

            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then

                Dim tmpPath As String = EndWithPathSeperator(Args.Path)

                If Not IO.Directory.Exists(tmpPath) Then
                    Write_WZ.Problem = "Folder does not exist."
                    Exit Function
                End If

                Dim tmpFilePath As String
                tmpFilePath = tmpPath & Args.MapName & ".gam"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_GAM.Bytes)
                tmpPath &= Args.MapName & OSPathSeperator
                IO.Directory.CreateDirectory(tmpPath)
                tmpFilePath = tmpPath & "dinit.bjo"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_droidBJO.Bytes)
                tmpFilePath = tmpPath & "feat.bjo"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_featBJO.Bytes)
                tmpFilePath = tmpPath & "game.map"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_MAP.Bytes)
                tmpFilePath = tmpPath & "struct.bjo"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_structBJO.Bytes)
                tmpFilePath = tmpPath & "ttypes.ttp"
                If IO.File.Exists(tmpFilePath) Then
                    Write_WZ.Problem = tmpFilePath & " already exists."
                    Exit Function
                End If
                IO.File.WriteAllBytes(tmpFilePath, File_TTP.Bytes)
            End If

        Catch ex As Exception
            Write_WZ.Problem = ex.Message
            Exit Function
        End Try

        Write_WZ.Success = True
    End Function

    Function Write_LND(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Write_LND.Success = False
        Write_LND.Problem = ""

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                Write_LND.Problem = "The selected file already exists."
                Exit Function
            End If
        End If

        Try

            Dim Text As String
            Dim EndChar As String
            Dim Quote As String
            Dim A As Integer
            Dim X As Integer
            Dim Z As Integer
            Dim Flip As Byte
            Dim B As Integer
            Dim VF As Integer
            Dim TF As Integer
            Dim C As Integer
            Dim Rotation As Byte
            Dim FlipX As Boolean

            Quote = Chr(34)
            EndChar = Chr(10)

            Dim ByteFile As clsByteWriteFile = New clsByteWriteFile

            If Tileset Is Tileset_Arizona Then
                Text = "DataSet WarzoneDataC1.eds" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "DataSet WarzoneDataC2.eds" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "DataSet WarzoneDataC3.eds" & EndChar
            Else
                Text = "DataSet " & EndChar
            End If
            ByteFile.Text_Append(Text)
            Text = "GrdLand {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Version 4" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    3DPosition 0.000000 3072.000000 0.000000" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    3DRotation 80.000000 0.000000 0.000000" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    2DPosition 0 0" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    CustomSnap 16 16" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    SnapMode 0" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Gravity 1" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    HeightScale " & HeightMultiplier & EndChar
            ByteFile.Text_Append(Text)
            Text = "    MapWidth " & TerrainSize.X & EndChar
            ByteFile.Text_Append(Text)
            Text = "    MapHeight " & TerrainSize.Y & EndChar
            ByteFile.Text_Append(Text)
            Text = "    TileWidth 128" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    TileHeight 128" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    SeaLevel 0" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    TextureWidth 64" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    TextureHeight 64" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumTextures 1" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Textures {" & EndChar
            ByteFile.Text_Append(Text)
            If Tileset Is Tileset_Arizona Then
                Text = "        texpages\tertilesc1.pcx" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "        texpages\tertilesc2.pcx" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "        texpages\tertilesc3.pcx" & EndChar
            Else
                Text = "        " & EndChar
            End If
            ByteFile.Text_Append(Text)
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumTiles " & TerrainSize.X * TerrainSize.Y & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Tiles {" & EndChar
            ByteFile.Text_Append(Text)
            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    TileOrientation_To_OldOrientation(TerrainTile(X, Z).Texture.Orientation, Rotation, FlipX)
                    Flip = 0
                    If TerrainTile(X, Z).Tri Then
                        Flip += 2
                    End If
                    If FlipX Then
                        Flip += 4
                    End If
                    'If TerrainTile(X, Z).Texture.FlipZ Then
                    '    Flip += 8
                    'End If
                    Flip += Rotation * 16

                    If TerrainTile(X, Z).Tri Then
                        VF = 1
                    Else
                        VF = 0
                    End If
                    If FlipX Then
                        TF = 1
                    Else
                        TF = 0
                    End If

                    Text = "        TID " & TerrainTile(X, Z).Texture.TextureNum + 1 & " VF " & VF & " TF " & TF & " F " & Flip & " VH " & TerrainVertex(X, Z).Height & " " & TerrainVertex(X + 1, Z).Height & " " & TerrainVertex(X + 1, Z + 1).Height & " " & TerrainVertex(X, Z + 1).Height & EndChar
                    ByteFile.Text_Append(Text)
                Next
            Next
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "ObjectList {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Version 3" & EndChar
            ByteFile.Text_Append(Text)
            If Tileset Is Tileset_Arizona Then
                Text = "	FeatureSet WarzoneDataC1.eds" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "	FeatureSet WarzoneDataC2.eds" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "	FeatureSet WarzoneDataC3.eds" & EndChar
            Else
                Text = "	FeatureSet " & EndChar
            End If
            ByteFile.Text_Append(Text)
            Text = "    NumObjects " & UnitCount & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Objects {" & EndChar
            ByteFile.Text_Append(Text)
            Dim XYZ_int As sXYZ_int
            For A = 0 To UnitCount - 1
                Select Case Units(A).Type.Type
                    Case clsUnitType.enumType.Feature
                        B = 0
                    Case clsUnitType.enumType.PlayerStructure
                        B = 1
                    Case clsUnitType.enumType.PlayerDroidTemplate
                        B = 2
                    Case Else
                        Write_LND.Problem = "Unit type classification not accounted for."
                        Exit Function
                End Select
                XYZ_int = LNDPos_From_MapPos(Units(A).Pos.X, Units(A).Pos.Z)
                Text = "        " & Units(A).ID & " " & B & " " & Quote & Units(A).Type.Code & Quote & " " & Units(A).PlayerNum & " " & Quote & Units(A).Name & Quote & " " & Strings.FormatNumber(XYZ_int.X, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(XYZ_int.Y, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(XYZ_int.Z, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(0, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(Units(A).Rotation, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(0, 2, TriState.True, TriState.False, TriState.False) & EndChar
                ByteFile.Text_Append(Text)
            Next
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "ScrollLimits {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Version 1" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumLimits 1" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Limits {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "        " & Quote & "Entire Map" & Quote & " 0 0 0 " & TerrainSize.X & " " & TerrainSize.Y & EndChar
            ByteFile.Text_Append(Text)
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "Gateways {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Version 1" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumGateways " & GatewayCount & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Gates {" & EndChar
            ByteFile.Text_Append(Text)
            For A = 0 To GatewayCount - 1
                Text = "        " & Gateways(A).PosA.X & " " & Gateways(A).PosA.Y & " " & Gateways(A).PosB.X & " " & Gateways(A).PosB.Y & EndChar
                ByteFile.Text_Append(Text)
            Next
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "TileTypes {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumTiles " & Tileset.TileCount & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Tiles {" & EndChar
            ByteFile.Text_Append(Text)
            For A = 0 To Math.Ceiling((Tileset.TileCount + 1) / 16.0#) - 1 '+1 because the first number is not a tile type
                Text = "        "
                C = A * 16 - 1 '-1 because the first number is not a tile type
                For B = 0 To Math.Min(16, Tileset.TileCount - C) - 1
                    If C + B < 0 Then
                        Text = Text & "2 "
                    Else
                        Text = Text & Map.Tile_TypeNum(C + B) & " "
                    End If
                Next
                Text = Text & EndChar
                ByteFile.Text_Append(Text)
            Next
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "TileFlags {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumTiles 90" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Flags {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 " & EndChar
            ByteFile.Text_Append(Text)
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)
            Text = "Brushes {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    Version 2" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumEdgeBrushes 0" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    NumUserBrushes 0" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    EdgeBrushes {" & EndChar
            ByteFile.Text_Append(Text)
            Text = "    }" & EndChar
            ByteFile.Text_Append(Text)
            Text = "}" & EndChar
            ByteFile.Text_Append(Text)

            ByteFile.Trim_Buffer()

            IO.File.WriteAllBytes(Path, ByteFile.Bytes)

        Catch ex As Exception
            Write_LND.Problem = ex.Message
            Exit Function
        End Try

        Write_LND.Success = True
    End Function

    Function Write_FME(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Write_FME.Success = False
        Write_FME.Problem = ""

        If Not Overwrite Then
            If IO.File.Exists(Path) Then
                Write_FME.Problem = "The selected file already exists."
                Exit Function
            End If
        End If

        Try

            Dim X As Integer
            Dim Z As Integer
            Dim ByteFile As New clsByteWriteFile

            ByteFile.U32_Append(SaveVersion)

            If Tileset Is Nothing Then
                ByteFile.U8_Append(0)
            ElseIf Tileset Is Tileset_Arizona Then
                ByteFile.U8_Append(1)
            ElseIf Tileset Is Tileset_Urban Then
                ByteFile.U8_Append(2)
            ElseIf Tileset Is Tileset_Rockies Then
                ByteFile.U8_Append(3)
            End If

            ByteFile.U16_Append(TerrainSize.X)
            ByteFile.U16_Append(TerrainSize.Y)

            Dim TileAttributes As Byte
            Dim DownSideData As Byte

            For Z = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X
                    ByteFile.U8_Append(TerrainVertex(X, Z).Height)
                    If TerrainVertex(X, Z).Terrain Is Nothing Then
                        ByteFile.U8_Append(0)
                    ElseIf TerrainVertex(X, Z).Terrain.Num < 0 Then
                        Write_FME.Problem = "Terrain number out of range."
                        Exit Function
                    Else
                        ByteFile.U8_Append(TerrainVertex(X, Z).Terrain.Num + 1)
                    End If
                Next
            Next
            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    ByteFile.U8_Append(TerrainTile(X, Z).Texture.TextureNum + 1)

                    TileAttributes = 0
                    If TerrainTile(X, Z).Terrain_IsCliff Then
                        TileAttributes += 128
                    End If
                    If TerrainTile(X, Z).Texture.Orientation.SwitchedAxes Then
                        TileAttributes += 64
                    End If
                    If TerrainTile(X, Z).Texture.Orientation.ResultXFlip Then
                        TileAttributes += 32
                    End If
                    If TerrainTile(X, Z).Texture.Orientation.ResultZFlip Then
                        TileAttributes += 16
                    End If
                    '8 is free
                    If TerrainTile(X, Z).Tri Then
                        TileAttributes += 4
                        If TerrainTile(X, Z).TriTopLeftIsCliff Then
                            TileAttributes += 2
                        End If
                        If TerrainTile(X, Z).TriBottomRightIsCliff Then
                            TileAttributes += 1
                        End If
                    Else
                        If TerrainTile(X, Z).TriBottomLeftIsCliff Then
                            TileAttributes += 2
                        End If
                        If TerrainTile(X, Z).TriTopRightIsCliff Then
                            TileAttributes += 1
                        End If
                    End If
                    ByteFile.U8_Append(TileAttributes)
                    If IdenticalTileOrientations(TerrainTile(X, Z).DownSide, TileDirection_Top) Then
                        DownSideData = 1
                    ElseIf IdenticalTileOrientations(TerrainTile(X, Z).DownSide, TileDirection_Left) Then
                        DownSideData = 2
                    ElseIf IdenticalTileOrientations(TerrainTile(X, Z).DownSide, TileDirection_Right) Then
                        DownSideData = 3
                    ElseIf IdenticalTileOrientations(TerrainTile(X, Z).DownSide, TileDirection_Bottom) Then
                        DownSideData = 4
                    Else
                        DownSideData = 0
                    End If
                    ByteFile.U8_Append(DownSideData)
                Next
            Next
            For Z = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X - 1
                    If TerrainSideH(X, Z).Road Is Nothing Then
                        ByteFile.U8_Append(0)
                    ElseIf TerrainSideH(X, Z).Road.Num < 0 Then
                        Write_FME.Problem = "Road number out of range."
                        Exit Function
                    Else
                        ByteFile.U8_Append(TerrainSideH(X, Z).Road.Num + 1)
                    End If
                Next
            Next
            For Z = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X
                    If TerrainSideV(X, Z).Road Is Nothing Then
                        ByteFile.U8_Append(0)
                    ElseIf TerrainSideV(X, Z).Road.Num < 0 Then
                        Write_FME.Problem = "Road number out of range."
                        Exit Function
                    Else
                        ByteFile.U8_Append(TerrainSideV(X, Z).Road.Num + 1)
                    End If
                Next
            Next

            Dim A As Integer

            ByteFile.U32_Append(UnitCount)

            For A = 0 To UnitCount - 1
                ByteFile.Text_Append(Units(A).Type.Code, 40)
                Select Case Units(A).Type.Type
                    Case clsUnitType.enumType.Feature
                        ByteFile.U8_Append(0)
                    Case clsUnitType.enumType.PlayerStructure
                        ByteFile.U8_Append(1)
                    Case clsUnitType.enumType.PlayerDroidTemplate
                        ByteFile.U8_Append(2)
                End Select
                ByteFile.U32_Append(Units(A).ID)
                ByteFile.S32_Append(Units(A).SavePriority)
                ByteFile.U32_Append(Units(A).Pos.X)
                ByteFile.U32_Append(Units(A).Pos.Z)
                ByteFile.U32_Append(Units(A).Pos.Y)
                ByteFile.U16_Append(Units(A).Rotation)
                ByteFile.Text_Append(Units(A).Name, True)
                ByteFile.U8_Append(Units(A).PlayerNum)
            Next

            ByteFile.U32_Append(GatewayCount)

            For A = 0 To GatewayCount - 1
                ByteFile.U16_Append(Gateways(A).PosA.X)
                ByteFile.U16_Append(Gateways(A).PosA.Y)
                ByteFile.U16_Append(Gateways(A).PosB.X)
                ByteFile.U16_Append(Gateways(A).PosB.Y)
            Next

            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    ByteFile.U8_Append(Map.Tile_TypeNum(A))
                Next
            End If

            'scroll limits
            ByteFile.S32_Append(CInt(Clamp(Val(frmCompileInstance.txtCampMinX.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))))
            ByteFile.S32_Append(CInt(Clamp(Val(frmCompileInstance.txtCampMinY.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))))
            ByteFile.U32_Append(CUInt(Clamp(Val(frmCompileInstance.txtCampMaxX.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))))
            ByteFile.U32_Append(CUInt(Clamp(Val(frmCompileInstance.txtCampMaxY.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))))

            'other compile info
            ByteFile.Text_Append(frmCompileInstance.txtName.Text, True)
            If frmCompileInstance.rdoMulti.Checked Then
                ByteFile.U8_Append(1)
            ElseIf frmCompileInstance.rdoCamp.Checked Then
                ByteFile.U8_Append(2)
            Else
                ByteFile.U8_Append(0)
            End If
            ByteFile.Text_Append(frmCompileInstance.txtMultiPlayers.Text, True)
            If frmCompileInstance.chkNewPlayerFormat.Checked Then
                ByteFile.U8_Append(1)
            Else
                ByteFile.U8_Append(0)
            End If
            ByteFile.Text_Append(frmCompileInstance.txtAuthor.Text, True)
            ByteFile.Text_Append(frmCompileInstance.cmbLicense.Text, True)
            ByteFile.Text_Append(frmCompileInstance.txtCampTime.Text, True)
            Dim intTemp As Integer = frmCompileInstance.cmbCampType.SelectedIndex
            ByteFile.S32_Append(intTemp)

            If IO.File.Exists(Path) Then
                IO.File.Delete(Path)
            End If

            ByteFile.Trim_Buffer()
            IO.File.WriteAllBytes(Path, ByteFile.Bytes)

        Catch ex As Exception
            Write_FME.Problem = ex.Message
            Exit Function
        End Try

        Write_FME.Success = True
    End Function

    Function Write_MinimapFile(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Write_MinimapFile.Problem = ""
        Write_MinimapFile.Success = False

        Dim X As Integer
        Dim Z As Integer

        Dim MinimapBitmap As New clsFileBitmap(TerrainSize.X, TerrainSize.Y)

        Dim Texture(TerrainSize.Y - 1, TerrainSize.X - 1, 3) As Byte

        Map.Minimap_Texture_Fill(Texture)

        For Z = 0 To Texture.GetUpperBound(0)
            For X = 0 To Texture.GetUpperBound(1)
                MinimapBitmap.CurrentBitmap.SetPixel(X, Z, Drawing.ColorTranslator.FromOle(OSRGB(Texture(Z, X, 0), Texture(Z, X, 1), Texture(Z, X, 2))))
            Next
        Next

        Write_MinimapFile = MinimapBitmap.Save(Path, Overwrite)
    End Function

    Function Write_HeightmapBMP(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim HeightmapBitmap As New clsFileBitmap(TerrainSize.X + 1, TerrainSize.Y + 1)
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                HeightmapBitmap.CurrentBitmap.SetPixel(X, Y, Drawing.ColorTranslator.FromOle(OSRGB(TerrainVertex(X, Y).Height, TerrainVertex(X, Y).Height, TerrainVertex(X, Y).Height)))
            Next
        Next

        Write_HeightmapBMP = HeightmapBitmap.Save(Path, Overwrite)
    End Function

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

    Function Unit_Add(ByVal NewUnit As clsUnit) As Integer

        Dim A As Integer
        Dim ID As UInteger

        ID = 0
        For A = 0 To UnitCount - 1
            If Units(A).ID >= ID Then
                ID = Units(A).ID + 1
            End If
        Next
        Unit_Add = Unit_Add(NewUnit, ID)
    End Function

    Function Unit_Add(ByVal NewUnit As clsUnit, ByVal ID As UInteger) As Integer
        Dim A As Integer

        For A = 0 To UnitCount - 1
            If ID = Units(A).ID Then
                Exit For
            End If
        Next
        If A <> UnitCount Then
            Unit_Add = Unit_Add(NewUnit)
            Exit Function
        End If

        NewUnit.ID = ID

        NewUnit.Num = UnitCount

        NewUnit.Pos.X = Clamp(NewUnit.Pos.X, 0, TerrainSize.X * TerrainGridSpacing - 1)
        NewUnit.Pos.Z = Clamp(NewUnit.Pos.Z, 0, TerrainSize.Y * TerrainGridSpacing - 1)
        NewUnit.Pos.Y = GetTerrainHeight(NewUnit.Pos.X, NewUnit.Pos.Z)

        If Units.GetUpperBound(0) < UnitCount Then
            ReDim Preserve Units((UnitCount + 1) * 2 - 1)
        End If
        Units(UnitCount) = NewUnit
        Unit_Add = UnitCount
        UnitCount += 1

        Unit_Sectors_Calc(UnitCount - 1)
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

        A = 0
        Do While A < frmMainInstance.View.MouseOver_UnitCount
            If frmMainInstance.View.MouseOver_Units(A) Is Units(Num) Then
                frmMainInstance.View.MouseOverUnit_Remove(A)
            Else
                A += 1
            End If
        Loop

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

        Units(Num).Num = -1

        UnitCount -= 1
        If Num <> UnitCount Then
            Units(UnitCount).Num = Num
            Units(Num) = Units(UnitCount)
        End If
        ReDim Preserve Units(UnitCount - 1)
    End Sub

    Sub Pos_Get_Sector(ByVal X As Integer, ByVal Z As Integer, ByRef SectorNum As sXY_int)
        Dim intTemp As Integer

        intTemp = SectorTileSize * TerrainGridSpacing
        SectorNum.X = Int(X / intTemp)
        SectorNum.Y = Int(Z / intTemp)
    End Sub

    Sub Tile_Get_Sector(ByVal X As Integer, ByVal Z As Integer, ByRef SectorNum As sXY_int)

        SectorNum.X = Int(X / SectorTileSize)
        SectorNum.Y = Int(Z / SectorTileSize)
    End Sub

    Sub UnitHeight_Update_All()
        Dim A As Integer
        Dim Unit_New As clsUnit
        Dim ID As UInteger

        Dim OldUnits() As clsUnit = Units.Clone
        Dim OldUnitCount As Integer = UnitCount

        For A = 0 To OldUnitCount - 1
            Unit_New = New clsUnit(OldUnits(A))
            ID = OldUnits(A).ID
            Unit_New.Pos.Y = GetTerrainHeight(Unit_New.Pos.X, Unit_New.Pos.Z)
            Unit_Remove_StoreChange(OldUnits(A).Num)
            Unit_Add_StoreChange(Unit_New, ID)
        Next
        frmMainInstance.Selected_Object_Changed()
    End Sub

    Sub SectorAll_GL_Update()
        Dim X As Integer
        Dim Z As Integer

        For X = 0 To SectorCount.X - 1
            For Z = 0 To SectorCount.Y - 1
                Sector_GLList_Make(X, Z)
            Next
        Next

        Minimap_Make()
    End Sub

    Sub SectorAll_Set_Changed()
        Dim X As Integer
        Dim Z As Integer

        For X = 0 To SectorCount.X - 1
            For Z = 0 To SectorCount.Y - 1
                Sector(X, Z).Changed = True
            Next
        Next
    End Sub

    Sub SectorAll_Set_NotChanged()
        Dim X As Integer
        Dim Z As Integer

        For X = 0 To SectorCount.X - 1
            For Z = 0 To SectorCount.Y - 1
                Sector(X, Z).Changed = False
            Next
        Next
    End Sub

    Function LNDPos_From_MapPos(ByVal X As Integer, ByVal Z As Integer) As sXYZ_int

        LNDPos_From_MapPos.X = X - TerrainSize.X * TerrainGridSpacing / 2.0#
        LNDPos_From_MapPos.Z = TerrainSize.Y * TerrainGridSpacing / 2.0# - Z
        LNDPos_From_MapPos.Y = GetTerrainHeight(X, Z)
    End Function

    Function MapPos_From_LNDPos(ByVal Pos As sXYZ_int) As sXYZ_int

        MapPos_From_LNDPos.X = Pos.X + TerrainSize.X * TerrainGridSpacing / 2.0#
        MapPos_From_LNDPos.Z = TerrainSize.Y * TerrainGridSpacing / 2.0# - Pos.Z
        MapPos_From_LNDPos.Y = GetTerrainHeight(MapPos_From_LNDPos.X, MapPos_From_LNDPos.Z)
    End Function

    Function TileAligned_Pos_From_MapPos(ByVal X As Integer, ByVal Z As Integer, ByVal Footprint As sXY_int) As sXYZ_int

        TileAligned_Pos_From_MapPos.X = (Math.Round((X - Footprint.X * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.X / 2.0#) * TerrainGridSpacing
        TileAligned_Pos_From_MapPos.Z = (Math.Round((Z - Footprint.Y * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.Y / 2.0#) * TerrainGridSpacing
        TileAligned_Pos_From_MapPos.Y = GetTerrainHeight(TileAligned_Pos_From_MapPos.X, TileAligned_Pos_From_MapPos.Z)
    End Function

    Sub Unit_Sectors_Calc(ByVal Num As Integer)
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim Footprint As sXY_int
        Dim X As Integer
        Dim Z As Integer
        Dim A As Integer

        If Units(Num).Type.LoadedInfo IsNot Nothing Then
            Footprint = Units(Num).Type.LoadedInfo.Footprint
        Else
            Footprint.X = 1
            Footprint.Y = 1
        End If

        Pos_Get_Sector(Units(Num).Pos.X - Footprint.X * TerrainGridSpacing / 2.0#, Units(Num).Pos.Z - Footprint.Y * TerrainGridSpacing / 2.0#, Start)
        Pos_Get_Sector(Units(Num).Pos.X + Footprint.X * TerrainGridSpacing / 2.0#, Units(Num).Pos.Z + Footprint.Y * TerrainGridSpacing / 2.0#, Finish)
        Start.X = Clamp(Start.X, 0, SectorCount.X - 1)
        Start.Y = Clamp(Start.Y, 0, SectorCount.Y - 1)
        Finish.X = Clamp(Finish.X, 0, SectorCount.X - 1)
        Finish.Y = Clamp(Finish.Y, 0, SectorCount.Y - 1)
        Units(Num).SectorCount = (Finish.X - Start.X + 1) * (Finish.Y - Start.Y + 1)
        ReDim Units(Num).Sectors(Units(Num).SectorCount - 1)
        ReDim Units(Num).Sectors_UnitNum(Units(Num).SectorCount - 1)
        A = 0
        For Z = Start.Y To Finish.Y
            For X = Start.X To Finish.X
                Units(Num).Sectors(A) = Sector(X, Z)
                Units(Num).Sectors_UnitNum(A) = Sector(X, Z).Unit_Add(Units(Num), A)
                A += 1
            Next
        Next
    End Sub

    Sub UndoStepCreate(ByVal StepName As String)
        Dim X As Integer
        Dim Z As Integer
        Dim Undo_New As New clsUndo

        Undo_New.Name = StepName

        AutoSave.ChangeCount += 1UI
        AutoSave_Test()
        frmMainInstance.tsbSave.Enabled = True

        ReDim Undo_New.ChangedSectors(SectorCount.X * SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                If Sector(X, Z).Changed Then
                    Sector(X, Z).Changed = False
                    Undo_New.ChangedSectors(Undo_New.ChangedSectorCount) = ShadowSector(X, Z)
                    Undo_New.ChangedSectorCount = Undo_New.ChangedSectorCount + 1
                    ShadowSector_Create(X, Z)
                End If
            Next
        Next
        ReDim Preserve Undo_New.ChangedSectors(Undo_New.ChangedSectorCount - 1)

        Undo_New.UnitChanges = UnitChanges.Clone
        Undo_New.UnitChangeCount = UnitChangeCount

        UnitChangeCount = 0
        ReDim UnitChanges(-1)

        If Undo_New.ChangedSectorCount + Undo_New.UnitChangeCount > 0 Then
            UndoCount = Undo_Pos
            ReDim Preserve Undo(UndoCount - 1) 'a new line has been started so remove redos

            Undo_Append(Undo_New)
            Undo_Pos = UndoCount
        End If
    End Sub

    Sub ShadowSector_Create(ByVal SectorX As Integer, ByVal SectorZ As Integer)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Z As Integer
        Dim tmpShadowSector As clsShadowSector
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        tmpShadowSector = New clsShadowSector
        ShadowSector(SectorX, SectorZ) = tmpShadowSector
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
        For Z = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Z
                tmpShadowSector.TerrainVertex(X, Z) = TerrainVertex(TileX, TileZ)
            Next
        Next
        For Z = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Z
                tmpShadowSector.TerrainTile(X, Z) = TerrainTile(TileX, TileZ)
            Next
        Next
        For Z = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Z
                tmpShadowSector.TerrainSideH(X, Z) = TerrainSideH(TileX, TileZ)
            Next
        Next
        For Z = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Z
                tmpShadowSector.TerrainSideV(X, Z) = TerrainSideV(TileX, TileZ)
            Next
        Next
    End Sub

    Sub ShadowSector_CreateAll()
        Dim X As Integer
        Dim Z As Integer

        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                ShadowSector_Create(X, Z)
            Next
        Next
    End Sub

    Sub Undo_Clear()

        UndoCount = 0
        ReDim Undo(-1)
        Undo_Pos = UndoCount
        SectorAll_Set_NotChanged()
    End Sub

    Sub Undo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Z As Integer

        UndoStepCreate("Incomplete Action") 'make another redo step incase something has changed, such as if user presses undo while still dragging a tool

        Undo_Pos = Undo_Pos - 1

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        For A = 0 To Undo(Undo_Pos).ChangedSectorCount - 1
            X = Undo(Undo_Pos).ChangedSectors(A).Num.X
            Z = Undo(Undo_Pos).ChangedSectors(A).Num.Y
            'store existing state for redo
            tmpShadow = ShadowSector(X, Z)
            'remove graphics from sector
            If Sector(X, Z).GLList_Textured > 0 Then
                GL.DeleteLists(Sector(X, Z).GLList_Textured, 1)
                Sector(X, Z).GLList_Textured = 0
            End If
            If Sector(X, Z).GLList_Wireframe > 0 Then
                GL.DeleteLists(Sector(X, Z).GLList_Wireframe, 1)
                Sector(X, Z).GLList_Wireframe = 0
            End If
            'perform the undo
            Undo_Sector_Rejoin(Undo(Undo_Pos).ChangedSectors(A))
            'update the backup
            ShadowSector_Create(X, Z)
            'add old state to the redo step (that was this undo step)
            Undo(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undo(Undo_Pos).ChangedSectorCount - 1
            X = Undo(Undo_Pos).ChangedSectors(A).Num.X
            Z = Undo(Undo_Pos).ChangedSectors(A).Num.Y
            'update graphics on changed sector
            Sector_GLList_Make(X, Z)
        Next

        For A = Undo(Undo_Pos).UnitChangeCount - 1 To 0 Step -1 'must do in reverse order, otherwise may try to delete units that havent been added yet
            Select Case Undo(Undo_Pos).UnitChanges(A).Type
                Case sUnitChange.enumType.Added
                    'remove the unit from the map
                    Unit_Remove(Undo(Undo_Pos).UnitChanges(A).Unit.Num)
                Case sUnitChange.enumType.Deleted
                    'add the unit back on to the map
                    Unit_Add(Undo(Undo_Pos).UnitChanges(A).Unit, Undo(Undo_Pos).UnitChanges(A).Unit.ID)
                Case Else
                    Stop
            End Select
        Next

        Minimap_Make()
        frmMainInstance.Selected_Object_Changed()
    End Sub

    Sub Redo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Z As Integer

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        For A = 0 To Undo(Undo_Pos).ChangedSectorCount - 1
            X = Undo(Undo_Pos).ChangedSectors(A).Num.X
            Z = Undo(Undo_Pos).ChangedSectors(A).Num.Y
            'store existing state for undo
            tmpShadow = ShadowSector(X, Z)
            'remove graphics from sector
            If Sector(X, Z).GLList_Textured > 0 Then
                GL.DeleteLists(Sector(X, Z).GLList_Textured, 1)
                Sector(X, Z).GLList_Textured = 0
            End If
            If Sector(X, Z).GLList_Wireframe > 0 Then
                GL.DeleteLists(Sector(X, Z).GLList_Wireframe, 1)
                Sector(X, Z).GLList_Wireframe = 0
            End If
            'perform the redo
            Undo_Sector_Rejoin(Undo(Undo_Pos).ChangedSectors(A))
            'update the backup
            ShadowSector_Create(X, Z)
            'add old state to the undo step (that was this redo step)
            Undo(Undo_Pos).ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To Undo(Undo_Pos).ChangedSectorCount - 1
            X = Undo(Undo_Pos).ChangedSectors(A).Num.X
            Z = Undo(Undo_Pos).ChangedSectors(A).Num.Y
            'update graphics on changed sector
            Sector_GLList_Make(X, Z)
        Next

        For A = 0 To Undo(Undo_Pos).UnitChangeCount - 1
            Select Case Undo(Undo_Pos).UnitChanges(A).Type
                Case sUnitChange.enumType.Added
                    'add the unit back on to the map
                    Unit_Add(Undo(Undo_Pos).UnitChanges(A).Unit, Undo(Undo_Pos).UnitChanges(A).Unit.ID)
                Case sUnitChange.enumType.Deleted
                    'remove the unit from the map
                    Unit_Remove(Undo(Undo_Pos).UnitChanges(A).Unit.Num)
                Case Else
                    Stop
            End Select
        Next

        Undo_Pos += 1

        Minimap_Make()
        frmMainInstance.Selected_Object_Changed()
    End Sub

    Sub Undo_Sector_Rejoin(ByVal Shadow_Sector_To_Rejoin As clsShadowSector)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Z As Integer
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        StartX = Shadow_Sector_To_Rejoin.Num.X * SectorTileSize
        StartZ = Shadow_Sector_To_Rejoin.Num.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, TerrainSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, TerrainSize.Y - StartZ)
        For Z = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Z
                TerrainVertex(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainVertex(X, Z)
            Next
        Next
        For Z = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Z
                TerrainTile(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainTile(X, Z)
            Next
        Next
        For Z = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Z
                TerrainSideH(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainSideH(X, Z)
            Next
        Next
        For Z = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Z
                TerrainSideV(TileX, TileZ) = Shadow_Sector_To_Rejoin.TerrainSideV(X, Z)
            Next
        Next
    End Sub

    Function Undo_Append(ByVal NewUndo As clsUndo) As Integer

        ReDim Preserve Undo(UndoCount)
        Undo(UndoCount) = NewUndo
        Undo_Append = UndoCount
        UndoCount += 1
    End Function

    Sub Undo_Insert(ByVal Pos As Integer, ByVal NewUndo As clsUndo)
        Dim A As Integer

        ReDim Preserve Undo(UndoCount)
        For A = UndoCount - 1 To Pos
            Undo(A + 1) = Undo(A)
        Next
        Undo(Pos) = NewUndo
        UndoCount += 1
    End Sub

    Sub Map_Insert(ByVal Map_To_Insert As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int, ByVal Insert_Heights As Boolean, ByVal Insert_Textures As Boolean, ByVal Insert_Units As Boolean, ByVal Delete_Units As Boolean, ByVal Insert_Gateways As Boolean, ByVal Delete_Gateways As Boolean)
        Dim Finish As sXY_int
        Dim X As Integer
        Dim Z As Integer
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim AreaAdjusted As sXY_int

        Finish.X = Math.Min(Offset.X + Math.Min(Area.X, Map_To_Insert.TerrainSize.X), TerrainSize.X)
        Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, Map_To_Insert.TerrainSize.Y), TerrainSize.Y)
        AreaAdjusted.X = Finish.X - Offset.X
        AreaAdjusted.Y = Finish.Y - Offset.Y

        Tile_Get_Sector(Math.Max(Offset.X - 1, 0), Math.Max(Offset.Y - 1, 0), SectorStart)
        Tile_Get_Sector(Finish.X, Finish.Y, SectorFinish)
        If SectorFinish.X >= SectorCount.X Then
            SectorFinish.X = SectorCount.X - 1
        End If
        If SectorFinish.Y >= SectorCount.Y Then
            SectorFinish.Y = SectorCount.Y - 1
        End If
        For Z = SectorStart.Y To SectorFinish.Y
            For X = SectorStart.X To SectorFinish.X
                Sector(X, Z).Changed = True
            Next
        Next

        If Insert_Heights Then
            For Z = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    TerrainVertex(Offset.X + X, Offset.Y + Z).Height = Map_To_Insert.TerrainVertex(X, Z).Height
                Next
            Next
            For Z = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    TerrainTile(Offset.X + X, Offset.Y + Z).Tri = Map_To_Insert.TerrainTile(X, Z).Tri
                Next
            Next
        End If
        If Insert_Textures Then
            For Z = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    TerrainVertex(Offset.X + X, Offset.Y + Z).Terrain = Map_To_Insert.TerrainVertex(X, Z).Terrain
                Next
            Next
            Dim tmpTri As Boolean
            For Z = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    tmpTri = TerrainTile(Offset.X + X, Offset.Y + Z).Tri
                    TerrainTile(Offset.X + X, Offset.Y + Z) = Map_To_Insert.TerrainTile(X, Z)
                    TerrainTile(Offset.X + X, Offset.Y + Z).Tri = tmpTri
                Next
            Next
            For Z = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X - 1
                    TerrainSideH(Offset.X + X, Offset.Y + Z) = Map_To_Insert.TerrainSideH(X, Z)
                Next
            Next
            For Z = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X
                    TerrainSideV(Offset.X + X, Offset.Y + Z) = Map_To_Insert.TerrainSideV(X, Z)
                Next
            Next
        End If

        If Delete_Gateways Then
            Dim A As Integer
            A = 0
            Do While A < GatewayCount
                If (Gateways(A).PosA.X >= Offset.X And Gateways(A).PosA.Y >= Offset.Y And _
                    Gateways(A).PosA.X < Offset.X + Area.X And Gateways(A).PosA.Y < Offset.Y + Area.Y) Or _
                    (Gateways(A).PosB.X >= Offset.X And Gateways(A).PosB.Y >= Offset.Y And _
                    Gateways(A).PosB.X < Offset.X + Area.X And Gateways(A).PosB.Y < Offset.Y + Area.Y) Then
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
                 GateStart.X < Offset.X + Area.X And GateStart.Y < Offset.Y + Area.Y) Or _
                 (GateFinish.X >= Offset.X And GateFinish.Y >= Offset.Y And _
                 GateFinish.X < Offset.X + Area.X And GateFinish.Y < Offset.Y + Area.Y) Then
                    If GateStart.X >= 0 And GateFinish.X >= 0 And _
                      GateFinish.X < TerrainSize.X And GateFinish.Y < TerrainSize.Y Then
                        Gateway_Add(GateStart, GateFinish)
                    End If
                End If
            Next
        End If

        If Delete_Units Then
            Dim A As Integer
            Dim TempUnit As clsUnit
            Dim UnitsToDelete(-1) As clsUnit
            Dim UnitToDeleteCount As Integer = 0
            For Z = SectorStart.Y To SectorFinish.Y
                For X = SectorStart.X To SectorFinish.X
                    For A = 0 To Sector(X, Z).UnitCount - 1
                        TempUnit = Sector(X, Z).Unit(A)
                        If TempUnit.Pos.X >= Offset.X * TerrainGridSpacing And _
                        TempUnit.Pos.X < Finish.X * TerrainGridSpacing And _
                        TempUnit.Pos.Z >= Offset.Y * TerrainGridSpacing And _
                        TempUnit.Pos.Z < Finish.Y * TerrainGridSpacing Then
                            ReDim Preserve UnitsToDelete(UnitToDeleteCount)
                            UnitsToDelete(UnitToDeleteCount) = TempUnit
                            UnitToDeleteCount += 1
                        End If
                    Next
                Next
            Next
            For A = 0 To UnitToDeleteCount - 1
                If UnitsToDelete(A).Num >= 0 Then 'units may be in the list multiple times and already be deleted
                    Unit_Remove_StoreChange(UnitsToDelete(A).Num)
                End If
            Next
        End If
        If Insert_Units Then
            Dim PosDifX As Integer
            Dim PosDifZ As Integer
            Dim A As Integer
            Dim Unit_New As clsUnit
            Dim tmpUnit As clsUnit

            PosDifX = Offset.X * TerrainGridSpacing
            PosDifZ = Offset.Y * TerrainGridSpacing
            For A = 0 To Map_To_Insert.UnitCount - 1
                tmpUnit = Map_To_Insert.Units(A)
                If tmpUnit.Pos.X < AreaAdjusted.X * TerrainGridSpacing And _
                    tmpUnit.Pos.Z < AreaAdjusted.Y * TerrainGridSpacing Then
                    Unit_New = New clsUnit(Map_To_Insert.Units(A))
                    Unit_New.Pos.X += PosDifX
                    Unit_New.Pos.Z += PosDifZ
                    Unit_Add_StoreChange(Unit_New)
                End If
            Next
        End If

        For Z = SectorStart.Y To SectorFinish.Y
            For X = SectorStart.X To SectorFinish.X
                Sector_GLList_Make(X, Z)
            Next
        Next

        Minimap_Make()
    End Sub

    Sub Rotate_Clockwise(ByVal RotateUnits As Boolean)
        Dim Z As Integer
        Dim X As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim tmpGateways() As sGateway
        Dim TileCountX As Integer = TerrainSize.Y
        Dim TileCountZ As Integer = TerrainSize.X
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCountX, TileCountZ)
        ReDim tmpTerrainTile(TileCountX - 1, TileCountZ - 1)
        ReDim tmpTerrainSideH(TileCountX - 1, TileCountZ)
        ReDim tmpTerrainSideV(TileCountX, TileCountZ - 1)

        For Z = 0 To TileCountZ
            For X = 0 To TileCountX
                tmpTerrainVertex(X, Z) = TerrainVertex(Z, TerrainSize.Y - X)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX - 1
                X2 = TerrainSize.Y - X - 1
                tmpTerrainTile(X, Z).Texture = TerrainTile(Z, X2).Texture
                tmpTerrainTile(X, Z).Texture.Orientation.RotateClockwise()
                tmpTerrainTile(X, Z).DownSide = TerrainTile(Z, X2).DownSide
                tmpTerrainTile(X, Z).DownSide.RotateClockwise()
                tmpTerrainTile(X, Z).Tri = Not TerrainTile(Z, X2).Tri
                tmpTerrainTile(X, Z).TriTopLeftIsCliff = TerrainTile(Z, X2).TriBottomLeftIsCliff
                tmpTerrainTile(X, Z).TriBottomLeftIsCliff = TerrainTile(Z, X2).TriBottomRightIsCliff
                tmpTerrainTile(X, Z).TriBottomRightIsCliff = TerrainTile(Z, X2).TriTopRightIsCliff
                tmpTerrainTile(X, Z).TriTopRightIsCliff = TerrainTile(Z, X2).TriTopLeftIsCliff
            Next
        Next
        For Z = 0 To TileCountZ
            For X = 0 To TileCountX - 1
                tmpTerrainSideH(X, Z) = TerrainSideV(Z, TerrainSize.Y - X - 1)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX
                tmpTerrainSideV(X, Z) = TerrainSideH(Z, TerrainSize.Y - X)
            Next
        Next

        Dim A As Integer
        Dim intTemp As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.X = TerrainSize.Y - Gateways(A).PosA.Y - 1
            tmpGateways(A).PosA.Y = Gateways(A).PosA.X
            tmpGateways(A).PosB.X = TerrainSize.Y - Gateways(A).PosB.Y - 1
            tmpGateways(A).PosB.Y = Gateways(A).PosB.X
        Next

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            If RotateUnits Then
                Units(A).Rotation -= 90
                If Units(A).Rotation < 0 Then
                    Units(A).Rotation += 360
                End If
            End If
            intTemp = Units(A).Pos.X
            Units(A).Pos.X = TerrainSize.Y * TerrainGridSpacing - Units(A).Pos.Z
            Units(A).Pos.Z = intTemp
        Next
        If Selected_Tile_A_Exists Then
            Selected_Tile_A_Exists = False
        End If
        If Selected_Tile_B_Exists Then
            Selected_Tile_B_Exists = False
        End If
        If Selected_Area_VertexA_Exists Then
            Selected_Area_VertexA_Exists = False
        End If
        If Selected_Area_VertexB_Exists Then
            Selected_Area_VertexB_Exists = False
        End If

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCountX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCountZ / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.X < 0 _
              Or Units(A).Pos.X >= TileCountX * TerrainGridSpacing _
              Or Units(A).Pos.Z < 0 _
              Or Units(A).Pos.Z >= TileCountZ * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(A)
                A += 1
            End If
        Loop

        TerrainSize.X = TileCountX
        TerrainSize.Y = TileCountZ
        TerrainVertex = tmpTerrainVertex
        TerrainTile = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV
        Gateways = tmpGateways

        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.Parent_Map = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange.Parent_Map = Nothing
        SectorChange = New clsSectorChange(Me)
    End Sub

    Sub Rotate_Anticlockwise(ByVal RotateUnits As Boolean)
        Dim Z As Integer
        Dim X As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim tmpGateways() As sGateway
        Dim TileCountX As Integer = TerrainSize.Y
        Dim TileCountZ As Integer = TerrainSize.X
        Dim Z2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCountX, TileCountZ)
        ReDim tmpTerrainTile(TileCountX - 1, TileCountZ - 1)
        ReDim tmpTerrainSideH(TileCountX - 1, TileCountZ)
        ReDim tmpTerrainSideV(TileCountX, TileCountZ - 1)

        For Z = 0 To TileCountZ
            For X = 0 To TileCountX
                tmpTerrainVertex(X, Z) = TerrainVertex(TerrainSize.X - Z, X)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            Z2 = TerrainSize.X - Z - 1
            For X = 0 To TileCountX - 1
                tmpTerrainTile(X, Z).Texture = TerrainTile(Z2, X).Texture
                tmpTerrainTile(X, Z).Texture.Orientation.RotateAnticlockwise()
                tmpTerrainTile(X, Z).DownSide = TerrainTile(Z2, X).DownSide
                tmpTerrainTile(X, Z).DownSide.RotateAnticlockwise()
                tmpTerrainTile(X, Z).Tri = Not TerrainTile(Z2, X).Tri
                tmpTerrainTile(X, Z).TriTopLeftIsCliff = TerrainTile(Z2, X).TriTopRightIsCliff
                tmpTerrainTile(X, Z).TriBottomLeftIsCliff = TerrainTile(Z2, X).TriTopLeftIsCliff
                tmpTerrainTile(X, Z).TriBottomRightIsCliff = TerrainTile(Z2, X).TriBottomLeftIsCliff
                tmpTerrainTile(X, Z).TriTopRightIsCliff = TerrainTile(Z2, X).TriBottomRightIsCliff
            Next
        Next
        For Z = 0 To TileCountZ
            For X = 0 To TileCountX - 1
                tmpTerrainSideH(X, Z) = TerrainSideV(TerrainSize.X - Z, X)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX
                tmpTerrainSideV(X, Z) = TerrainSideH(TerrainSize.X - Z - 1, X)
            Next
        Next

        Dim A As Integer
        Dim intTemp As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.Y = TerrainSize.X - Gateways(A).PosA.X - 1
            tmpGateways(A).PosA.X = Gateways(A).PosA.Y
            tmpGateways(A).PosB.Y = TerrainSize.X - Gateways(A).PosB.X - 1
            tmpGateways(A).PosB.X = Gateways(A).PosB.Y
        Next

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            If RotateUnits Then
                Units(A).Rotation += 90
                If Units(A).Rotation > 359 Then
                    Units(A).Rotation -= 360
                End If
            End If
            intTemp = Units(A).Pos.Z
            Units(A).Pos.Z = TerrainSize.X * TerrainGridSpacing - Units(A).Pos.X
            Units(A).Pos.X = intTemp
        Next
        If Selected_Tile_A_Exists Then
            Selected_Tile_A_Exists = False
        End If
        If Selected_Tile_B_Exists Then
            Selected_Tile_B_Exists = False
        End If
        If Selected_Area_VertexA_Exists Then
            Selected_Area_VertexA_Exists = False
        End If
        If Selected_Area_VertexB_Exists Then
            Selected_Area_VertexB_Exists = False
        End If

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCountX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCountZ / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.X < 0 _
              Or Units(A).Pos.X >= TileCountX * TerrainGridSpacing _
              Or Units(A).Pos.Z < 0 _
              Or Units(A).Pos.Z >= TileCountZ * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(A)
                A += 1
            End If
        Loop

        TerrainSize.X = TileCountX
        TerrainSize.Y = TileCountZ
        TerrainVertex = tmpTerrainVertex
        TerrainTile = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV
        Gateways = tmpGateways

        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.Parent_Map = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange.Parent_Map = Nothing
        SectorChange = New clsSectorChange(Me)
    End Sub

    Sub FlipX()
        Dim Z As Integer
        Dim X As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim TileCountX As Integer = TerrainSize.X
        Dim TileCountZ As Integer = TerrainSize.Y
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCountX, TileCountZ)
        ReDim tmpTerrainTile(TileCountX - 1, TileCountZ - 1)
        ReDim tmpTerrainSideH(TileCountX - 1, TileCountZ)
        ReDim tmpTerrainSideV(TileCountX, TileCountZ - 1)

        For Z = 0 To TileCountZ
            For X = 0 To TileCountX
                tmpTerrainVertex(X, Z) = TerrainVertex(TerrainSize.X - X, Z)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX - 1
                X2 = TerrainSize.X - X - 1
                tmpTerrainTile(X, Z).Texture = TerrainTile(X2, Z).Texture
                tmpTerrainTile(X, Z).Texture.Orientation.ResultXFlip = Not tmpTerrainTile(X, Z).Texture.Orientation.ResultXFlip
                tmpTerrainTile(X, Z).DownSide = TerrainTile(X2, Z).DownSide
                tmpTerrainTile(X2, Z).DownSide.FlipX()
                tmpTerrainTile(X, Z).Tri = Not TerrainTile(X2, Z).Tri
                tmpTerrainTile(X, Z).TriTopLeftIsCliff = TerrainTile(X2, Z).TriTopRightIsCliff
                tmpTerrainTile(X, Z).TriBottomLeftIsCliff = TerrainTile(X2, Z).TriBottomRightIsCliff
                tmpTerrainTile(X, Z).TriBottomRightIsCliff = TerrainTile(X2, Z).TriBottomLeftIsCliff
                tmpTerrainTile(X, Z).TriTopRightIsCliff = TerrainTile(X2, Z).TriTopLeftIsCliff
            Next
        Next
        For Z = 0 To TileCountZ
            For X = 0 To TileCountX - 1
                tmpTerrainSideH(X, Z) = TerrainSideH(TerrainSize.X - X - 1, Z)
            Next
        Next
        For Z = 0 To TileCountZ - 1
            For X = 0 To TileCountX
                tmpTerrainSideV(X, Z) = TerrainSideV(TerrainSize.X - X, Z)
            Next
        Next

        Dim A As Integer

        For A = 0 To UnitCount - 1
            Units(A).Sectors_Remove()
            Units(A).Rotation -= 180
            If Units(A).Rotation < 0 Then
                Units(A).Rotation += 360
            End If
            Units(A).Pos.X = TerrainSize.X * TerrainGridSpacing - Units(A).Pos.X
            Units(A).Pos.Z = Units(A).Pos.Z
        Next
        If Selected_Tile_A_Exists Then
            Selected_Tile_A_Exists = False
        End If
        If Selected_Tile_B_Exists Then
            Selected_Tile_B_Exists = False
        End If
        If Selected_Area_VertexA_Exists Then
            Selected_Area_VertexA_Exists = False
        End If
        If Selected_Area_VertexB_Exists Then
            Selected_Area_VertexB_Exists = False
        End If

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCountX / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCountZ / SectorTileSize)
        ReDim Sector(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z) = New clsSector
            Next
        Next

        A = 0
        Do While A < UnitCount
            If Units(A).Pos.X < 0 _
              Or Units(A).Pos.X >= TileCountX * TerrainGridSpacing _
              Or Units(A).Pos.Z < 0 _
              Or Units(A).Pos.Z >= TileCountZ * TerrainGridSpacing Then
                Unit_Remove(A)
            Else
                Unit_Sectors_Calc(A)
                A += 1
            End If
        Loop

        TerrainSize.X = TileCountX
        TerrainSize.Y = TileCountZ
        TerrainVertex = tmpTerrainVertex
        TerrainTile = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV

        ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.Parent_Map = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorChange.Parent_Map = Nothing
        SectorChange = New clsSectorChange(Me)
    End Sub

    Structure sWZUnit
        Dim Code As String
        Dim ID As UInteger
        Dim Pos As sXYZ_int
        Dim Rotation As UInteger
        Dim Player As UInteger
        Dim LNDType As Byte
    End Structure

    Function Load_WZ(ByVal Path As String) As sResult
        Load_WZ.Success = False
        Load_WZ.Problem = ""

        Dim Result As sResult
        Dim Quote As String = Chr(34)
        Dim ZipEntry As Zip.ZipEntry
        Dim Buffer(-1) As Byte
        Dim LineData(-1) As String
        Dim GameFound As Boolean
        Dim DatasetFound As Boolean
        Dim MapName(-1) As String
        Dim MapTileset(-1) As clsTileset
        Dim GameTileset As clsTileset = Nothing
        Dim MapCount As Integer
        Dim GameName As String = ""
        Dim strTemp As String = ""
        Dim SplitPath As sZipSplitPath
        Dim GotMapFile As Boolean = False
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer

        Dim ZipStream As Zip.ZipInputStream = Nothing

        Try
            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))

            'get all usable lev entries
            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then
                    Exit Do
                End If

                SplitPath = New sZipSplitPath(ZipEntry.Name)

                If SplitPath.FileExtension = "lev" And SplitPath.PartCount = 1 Then
                    ReDim Buffer(ZipEntry.Size - 1)
                    ZipStream.Read(Buffer, 0, ZipEntry.Size)
                    BytesToLines(Buffer, LineData)
                    Lines_Remove_Comments(LineData)
                    'find each level block
                    For A = 0 To LineData.GetUpperBound(0)
                        If Strings.LCase(Strings.Left(LineData(A), 5)) = "level" Then
                            'find each levels game file
                            GameFound = False
                            B = 1
                            Do While A + B <= LineData.GetUpperBound(0)
                                If Strings.LCase(Strings.Left(LineData(A + B), 4)) = "game" Then
                                    C = Strings.InStr(LineData(A + B), Quote)
                                    D = Strings.InStrRev(LineData(A + B), Quote)
                                    If C > 0 And D > 0 And D - C > 1 Then
                                        GameName = Strings.LCase(Strings.Mid(LineData(A + B), C + 1, D - C - 1))
                                        'see if map is already counted
                                        For C = 0 To MapCount - 1
                                            If GameName = MapName(C) Then Exit For
                                        Next
                                        If C = MapCount Then
                                            GameFound = True
                                        End If
                                    End If
                                    Exit Do
                                ElseIf Strings.LCase(Strings.Left(LineData(A + B), 5)) = "level" Then
                                    Exit Do
                                End If
                                B += 1
                            Loop
                            If GameFound Then
                                'find the dataset (determines tileset)
                                DatasetFound = False
                                B = 1
                                Do While A + B <= LineData.GetUpperBound(0)
                                    If Strings.LCase(Strings.Left(LineData(A + B), 7)) = "dataset" Then
                                        strTemp = Strings.LCase(Strings.Right(LineData(A + B), 1))
                                        If strTemp = "1" Then
                                            GameTileset = Tileset_Arizona
                                            DatasetFound = True
                                        ElseIf strTemp = "2" Then
                                            GameTileset = Tileset_Urban
                                            DatasetFound = True
                                        ElseIf strTemp = "3" Then
                                            GameTileset = Tileset_Rockies
                                            DatasetFound = True
                                        End If
                                        Exit Do
                                    ElseIf Strings.LCase(Strings.Left(LineData(A + B), 5)) = "level" Then
                                        Exit Do
                                    End If
                                    B += 1
                                Loop
                                If DatasetFound Then
                                    ReDim Preserve MapName(MapCount)
                                    ReDim Preserve MapTileset(MapCount)
                                    MapName(MapCount) = GameName
                                    MapTileset(MapCount) = GameTileset
                                    MapCount += 1
                                End If
                            End If
                        End If
                    Next
                End If
            Loop

            Dim MapLoadName As String

            'prompt user for which of the entries to load
            If MapCount < 1 Then
                Load_WZ.Problem = "No maps found in file."
                Exit Function
            ElseIf MapCount = 1 Then
                MapLoadName = MapName(0)
                Tileset = MapTileset(0)
            Else
                Dim frmWZLoad_Output As New frmWZLoad.clsOutput
                Dim frmWZLoad_New As New frmWZLoad(MapName, frmWZLoad_Output)
                frmWZLoad_New.ShowDialog()
                If frmWZLoad_Output.Result < 0 Then
                    Load_WZ.Problem = "No map selected."
                    Exit Function
                End If
                MapLoadName = MapName(frmWZLoad_Output.Result)
                Tileset = MapTileset(frmWZLoad_Output.Result)
            End If

            TileType_Reset()
            SetPainterToDefaults()

            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))

            Dim GameSplitPath As New sZipSplitPath(MapLoadName)
            Dim GameFilesPath As String = GameSplitPath.FilePath & GameSplitPath.FileTitleWithoutExtension & "/"

            Dim Version As UInteger
            Dim MapWidth As UInteger
            Dim MapHeight As UInteger
            Dim ByteLoadFile As clsByteReadFile
            Dim uintTemp As UInteger
            Dim ushortTemp As UShort
            Dim X As Integer
            Dim Z As Integer
            Dim Flip As Byte
            Dim FlipX As Boolean
            Dim FlipZ As Boolean
            Dim Rotate As Byte

            'load map files

            ByteLoadFile = New clsByteReadFile

            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then
                    Exit Do
                End If

                SplitPath = New sZipSplitPath(ZipEntry.Name)
                If SplitPath.FilePath = GameFilesPath Then
                    If SplitPath.FileTitle = "game.map" Then
                        ReDim ByteLoadFile.Bytes(ZipEntry.Size)
                        ByteLoadFile.ByteCount = ZipEntry.Size
                        ZipStream.Read(ByteLoadFile.Bytes, 0, ZipEntry.Size)

                        If Not ByteLoadFile.Get_Text(4, strTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                        If strTemp <> "map " Then
                            Load_WZ.Problem = "Unknown game.map identifier."
                            Exit Function
                        End If

                        If Not ByteLoadFile.Get_U32(Version) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Version <> 10UI Then
                            'Load_WZ.Problem = "Unknown game.map version."
                            'Exit Function
                            If MsgBox("game.map version is unknown. Continue?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) <> MsgBoxResult.Ok Then
                                Load_WZ.Problem = "Aborted."
                                Exit Function
                            End If
                        End If
                        If Not ByteLoadFile.Get_U32(MapWidth) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Not ByteLoadFile.Get_U32(MapHeight) Then Load_WZ.Problem = "Read error." : Exit Function
                        If MapWidth < 1 Or MapWidth > 1024 Or MapHeight < 1 Or MapHeight > 1024 Then Load_WZ.Problem = "Map size out of range." : Exit Function

                        Terrain_Blank(MapWidth, MapHeight)

                        For Z = 0 To MapHeight - 1
                            For X = 0 To MapWidth - 1
                                If Not ByteLoadFile.Get_U8(uintTemp) Then Load_WZ.Problem = "Tile data read error." : Exit Function
                                TerrainTile(X, Z).Texture.TextureNum = uintTemp
                                If Not ByteLoadFile.Get_U8(Flip) Then Load_WZ.Problem = "Tile data read error." : Exit Function
                                If Not ByteLoadFile.Get_U8(TerrainVertex(X, Z).Height) Then Load_WZ.Problem = "Tile data read error." : Exit Function
                                'get flipx
                                A = Int(Flip / 128.0#)
                                Flip -= A * 128
                                FlipX = (A = 1)
                                'get flipy
                                A = Int(Flip / 64.0#)
                                Flip -= A * 64
                                FlipZ = (A = 1)
                                'get rotation
                                A = Int(Flip / 16.0#)
                                Flip -= A * 16
                                Rotate = A
                                OldOrientation_To_TileOrientation(Rotate, FlipX, FlipZ, TerrainTile(X, Z).Texture.Orientation)
                                'get tri direction
                                A = Int(Flip / 8.0#)
                                Flip -= A * 8
                                TerrainTile(X, Z).Tri = (A = 1)
                            Next
                        Next

                        If Version <> 2UI Then
                            If Not ByteLoadFile.Get_U32(uintTemp) Then Load_WZ.Problem = "Gateway version read error." : Exit Function
                            If uintTemp <> 1 Then Load_WZ.Problem = "Bad gateway version number." : Exit Function

                            If Not ByteLoadFile.Get_U32(GatewayCount) Then Load_WZ.Problem = "Gateway read error." : Exit Function
                            ReDim Gateways(GatewayCount - 1)

                            For A = 0 To GatewayCount - 1
                                If Not ByteLoadFile.Get_U8(Gateways(A).PosA.X) Then Load_WZ.Problem = "Gateway read error." : Exit Function
                                If Not ByteLoadFile.Get_U8(Gateways(A).PosA.Y) Then Load_WZ.Problem = "Gateway read error." : Exit Function
                                If Not ByteLoadFile.Get_U8(Gateways(A).PosB.X) Then Load_WZ.Problem = "Gateway read error." : Exit Function
                                If Not ByteLoadFile.Get_U8(Gateways(A).PosB.Y) Then Load_WZ.Problem = "Gateway read error." : Exit Function
                            Next
                        End If

                        GotMapFile = True
                        Exit Do
                    End If
                End If
            Loop

            If Not GotMapFile Then
                Load_WZ.Problem = "game.map file not found."
                Exit Function
            End If

            Dim WZUnits(-1) As sWZUnit
            Dim WZUnitCount As Integer = 0
            Dim Unit_New As clsUnit

            ZipStream.Close()
            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
            ByteLoadFile = New clsByteReadFile

            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then Exit Do

                SplitPath = New sZipSplitPath(ZipEntry.Name)
                If SplitPath.FilePath = GameFilesPath Then
                    If SplitPath.FileTitle = "feat.bjo" Then
                        ReDim ByteLoadFile.Bytes(ZipEntry.Size)
                        ByteLoadFile.ByteCount = ZipEntry.Size
                        ZipStream.Read(ByteLoadFile.Bytes, 0, ZipEntry.Size)

                        If Not ByteLoadFile.Get_Text(4, strTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                        If strTemp <> "feat" Then
                            Load_WZ.Problem = "Unknown feat.bjo identifier."
                            Exit Function
                        End If

                        If Not ByteLoadFile.Get_U32(Version) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Version <> 8UI Then
                            'Load_WZ.Problem = "Unknown feat.bjo version."
                            'Exit Function
                            If MsgBox("feat.bjo version is unknown. Continue?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) <> MsgBoxResult.Ok Then
                                Load_WZ.Problem = "Aborted."
                                Exit Function
                            End If
                        End If

                        If Not ByteLoadFile.Get_U32(uintTemp) Then Load_WZ.Problem = "Read error." : Exit Function

                        ReDim Preserve WZUnits(WZUnitCount + uintTemp - 1)
                        For A = 0 To uintTemp - 1
                            WZUnits(WZUnitCount).LNDType = 0
                            If Not ByteLoadFile.Get_Text(40, WZUnits(WZUnitCount).Code) Then Load_WZ.Problem = "Read error." : Exit Function
                            B = Strings.InStr(WZUnits(WZUnitCount).Code, Chr(0))
                            If B > 0 Then
                                WZUnits(WZUnitCount).Code = Strings.Left(WZUnits(WZUnitCount).Code, B - 1)
                            End If
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).ID) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.X) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Z) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Y) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Rotation) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Player) Then Load_WZ.Problem = "Read error." : Exit Function
                            ByteLoadFile.Position += 12
                            WZUnitCount += 1
                        Next

                        Exit Do
                    End If
                End If
            Loop

            ZipStream.Close()
            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
            ByteLoadFile = New clsByteReadFile

            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then Exit Do

                SplitPath = New sZipSplitPath(ZipEntry.Name)
                If SplitPath.FilePath = GameFilesPath Then
                    If SplitPath.FileTitle = "ttypes.ttp" Then
                        ReDim ByteLoadFile.Bytes(ZipEntry.Size)
                        ByteLoadFile.ByteCount = ZipEntry.Size
                        ZipStream.Read(ByteLoadFile.Bytes, 0, ZipEntry.Size)

                        If Not ByteLoadFile.Get_Text(4, strTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                        If strTemp <> "ttyp" Then
                            Load_WZ.Problem = "Unknown ttypes.ttp identifier."
                            Exit Function
                        End If

                        If Not ByteLoadFile.Get_U32(Version) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Version <> 8UI Then
                            'Load_WZ.Problem = "Unknown ttypes.ttp version."
                            'Exit Function
                            If MsgBox("ttypes.ttp version is unknown. Continue?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) <> MsgBoxResult.Ok Then
                                Load_WZ.Problem = "Aborted."
                                Exit Function
                            End If
                        End If

                        If Not ByteLoadFile.Get_U32(uintTemp) Then Load_WZ.Problem = "Read error." : Exit Function

                        If Tileset IsNot Nothing Then
                            For A = 0 To Math.Min(uintTemp, Tileset.TileCount) - 1
                                If Not ByteLoadFile.Get_U16(ushortTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                                If ushortTemp > 11US Then
                                    Load_WZ.Problem = "Unknown tile type."
                                    Exit Function
                                End If
                                Tile_TypeNum(A) = ushortTemp
                            Next
                        End If

                        Exit Do
                    End If
                End If
            Loop

            ZipStream.Close()
            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
            ByteLoadFile = New clsByteReadFile

            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then Exit Do

                SplitPath = New sZipSplitPath(ZipEntry.Name)
                If SplitPath.FilePath = GameFilesPath Then
                    If SplitPath.FileTitle = "struct.bjo" Then
                        ReDim ByteLoadFile.Bytes(ZipEntry.Size)
                        ByteLoadFile.ByteCount = ZipEntry.Size
                        ZipStream.Read(ByteLoadFile.Bytes, 0, ZipEntry.Size)

                        If Not ByteLoadFile.Get_Text(4, strTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                        If strTemp <> "stru" Then
                            Load_WZ.Problem = "Unknown struct.bjo identifier."
                            Exit Function
                        End If

                        If Not ByteLoadFile.Get_U32(Version) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Version <> 8UI Then
                            'Load_WZ.Problem = "Unknown struct.bjo version."
                            'Exit Function
                            If MsgBox("struct.bjo version is unknown. Continue?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) <> MsgBoxResult.Ok Then
                                Load_WZ.Problem = "Aborted."
                                Exit Function
                            End If
                        End If

                        If Not ByteLoadFile.Get_U32(uintTemp) Then Load_WZ.Problem = "Read error." : Exit Function

                        ReDim Preserve WZUnits(WZUnitCount + uintTemp - 1)
                        For A = 0 To uintTemp - 1
                            WZUnits(WZUnitCount).LNDType = 1
                            If Not ByteLoadFile.Get_Text(40, WZUnits(WZUnitCount).Code) Then Load_WZ.Problem = "Read error." : Exit Function
                            B = Strings.InStr(WZUnits(WZUnitCount).Code, Chr(0))
                            If B > 0 Then
                                WZUnits(WZUnitCount).Code = Strings.Left(WZUnits(WZUnitCount).Code, B - 1)
                            End If
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).ID) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.X) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Z) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Y) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Rotation) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Player) Then Load_WZ.Problem = "Read error." : Exit Function
                            ByteLoadFile.Position += 56
                            WZUnitCount += 1
                        Next

                        Exit Do
                    End If
                End If
            Loop

            ZipStream.Close()
            ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
            ByteLoadFile = New clsByteReadFile

            Do
                ZipEntry = ZipStream.GetNextEntry
                If ZipEntry Is Nothing Then Exit Do

                SplitPath = New sZipSplitPath(ZipEntry.Name)
                If SplitPath.FilePath = GameFilesPath Then
                    If SplitPath.FileTitle = "dinit.bjo" Then
                        ReDim ByteLoadFile.Bytes(ZipEntry.Size)
                        ByteLoadFile.ByteCount = ZipEntry.Size
                        ZipStream.Read(ByteLoadFile.Bytes, 0, ZipEntry.Size)

                        If Not ByteLoadFile.Get_Text(4, strTemp) Then Load_WZ.Problem = "Read error." : Exit Function
                        If strTemp <> "dint" Then
                            Load_WZ.Problem = "Unknown dinit.bjo identifier."
                            Exit Function
                        End If

                        If Not ByteLoadFile.Get_U32(Version) Then Load_WZ.Problem = "Read error." : Exit Function
                        If Version > 19UI Then
                            'Load_WZ.Problem = "Unknown dinit.bjo version."
                            'Exit Function
                            If MsgBox("dinit.bjo version is unknown. Continue?", MsgBoxStyle.OkCancel + MsgBoxStyle.Question) <> MsgBoxResult.Ok Then
                                Load_WZ.Problem = "Aborted."
                                Exit Function
                            End If
                        End If

                        If Not ByteLoadFile.Get_U32(uintTemp) Then Load_WZ.Problem = "Read error." : Exit Function

                        ReDim Preserve WZUnits(WZUnitCount + uintTemp - 1)
                        For A = 0 To uintTemp - 1
                            WZUnits(WZUnitCount).LNDType = 2
                            If Not ByteLoadFile.Get_Text(40, WZUnits(WZUnitCount).Code) Then Load_WZ.Problem = "Read error." : Exit Function
                            B = Strings.InStr(WZUnits(WZUnitCount).Code, Chr(0))
                            If B > 0 Then
                                WZUnits(WZUnitCount).Code = Strings.Left(WZUnits(WZUnitCount).Code, B - 1)
                            End If
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).ID) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.X) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Z) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Pos.Y) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Rotation) Then Load_WZ.Problem = "Read error." : Exit Function
                            If Not ByteLoadFile.Get_U32(WZUnits(WZUnitCount).Player) Then Load_WZ.Problem = "Read error." : Exit Function
                            ByteLoadFile.Position += 12
                            WZUnitCount += 1
                        Next

                        Exit Do
                    End If
                End If
            Loop

            ZipStream.Close()

            For A = 0 To WZUnitCount - 1
                Unit_New = New clsUnit
                Unit_New.ID = WZUnits(A).ID
                Result = FindUnitType(WZUnits(A).Code, WZUnits(A).LNDType, Unit_New.Type)
                If Not Result.Success Then
                    Load_WZ.Problem = Result.Problem
                    Exit Function
                End If
                Unit_New.PlayerNum = Math.Min(WZUnits(A).Player, FactionCountMax - 1)
                Unit_New.Pos = WZUnits(A).Pos
                Unit_New.Rotation = Math.Min(WZUnits(A).Rotation, 359UI)
                Unit_Add(Unit_New, WZUnits(A).ID)
            Next

            ReDim ShadowSector(SectorCount.X - 1, SectorCount.Y - 1)
            ShadowSector_CreateAll()
            AutoTextureChange = New clsAutoTextureChange(Me)
            SectorChange = New clsSectorChange(Me)

        Catch ex As Exception
            If ZipStream IsNot Nothing Then
                ZipStream.Close()
            End If
            Load_WZ.Problem = ex.Message
            Exit Function
        End Try

        Load_WZ.Success = True
    End Function

    Sub Gateway_Remove(ByVal Num As Integer)

        GatewayCount -= 1
        If Num <> GatewayCount Then
            Gateways(Num) = Gateways(GatewayCount)
        End If
        ReDim Preserve Gateways(GatewayCount - 1)
    End Sub

    Sub Gateway_Add(ByVal PosA As sXY_int, ByVal PosB As sXY_int)

        ReDim Preserve Gateways(GatewayCount)
        Gateways(GatewayCount).PosA = PosA
        Gateways(GatewayCount).PosB = PosB
        GatewayCount += 1
    End Sub

    Sub Sectors_Deallocate()
        Dim X As Integer
        Dim Z As Integer

        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sector(X, Z).Deallocate()
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

    Function Write_TTP(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Write_TTP.Success = False
        Write_TTP.Problem = ""

        Dim File_TTP As New clsByteWriteFile

        File_TTP.Text_Append("ttyp")
        File_TTP.U32_Append(8UI)
        If Map.Tileset Is Nothing Then
            File_TTP.U32_Append(0UI)
        Else
            File_TTP.U32_Append(Map.Tileset.TileCount)
            For A = 0 To Map.Tileset.TileCount - 1
                File_TTP.U16_Append(Map.Tile_TypeNum(A))
            Next
        End If

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                Write_TTP.Problem = "File already exists."
                Exit Function
            End If
        End If

        File_TTP.Trim_Buffer()
        Try
            IO.File.WriteAllBytes(Path, File_TTP.Bytes)
        Catch ex As Exception
            Write_TTP.Problem = ex.Message
            Exit Function
        End Try

        Write_TTP.Success = True
    End Function

    Function Load_TTP(ByVal Path As String) As sResult
        Load_TTP.Success = False
        Load_TTP.Problem = ""

        Dim File_TTP As New clsByteReadFile

        File_TTP.Bytes = IO.File.ReadAllBytes(Path)
        File_TTP.ByteCount = File_TTP.Bytes.GetUpperBound(0) + 1
        File_TTP.Position = 0

        Dim strTemp As String = ""
        Dim uintTemp As UInteger
        Dim ushortTemp As UShort
        Dim A As Integer

        If Not File_TTP.Get_Text(4, strTemp) Then Load_TTP.Problem = "Read error." : Exit Function
        If strTemp <> "ttyp" Then Load_TTP.Problem = "Incorrect identifier." : Exit Function
        If Not File_TTP.Get_U32(uintTemp) Then Load_TTP.Problem = "Read error." : Exit Function
        If uintTemp <> 8UI Then Load_TTP.Problem = "Unknown version." : Exit Function
        If Not File_TTP.Get_U32(uintTemp) Then Load_TTP.Problem = "Read error." : Exit Function
        For A = 0 To Math.Min(uintTemp, CUInt(Tileset.TileCount)) - 1
            If Not File_TTP.Get_U16(ushortTemp) Then Load_TTP.Problem = "Read error." : Exit Function
            If ushortTemp > 11 Then Load_TTP.Problem = "Unknown tile type number." : Exit Function
            Tile_TypeNum(A) = ushortTemp
        Next

        Load_TTP.Success = True
    End Function

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

        Path = AutoSavePath & "autosaved-" & DateNow.Year & "-" & MinDigits(DateNow.Month, 2) & "-" & MinDigits(DateNow.Day, 2) & "-" & MinDigits(DateNow.Hour, 2) & "-" & MinDigits(DateNow.Minute, 2) & "-" & MinDigits(DateNow.Second, 2) & "-" & MinDigits(DateNow.Millisecond, 3) & ".fme"

        Dim Result As sResult = Write_FME(Path, False)

        If Not Result.Success Then
            MsgBox("Autosave failed to write file; " & Result.Problem)
        End If
    End Sub

    Sub Terrain_Interpret()
        If Tileset Is Nothing Then Exit Sub

        Dim X As Integer
        Dim Z As Integer
        Dim A As Integer
        Dim B As Integer
        Dim Vertex_Terrain_Count(TerrainSize.X, TerrainSize.Y, Painter.TerrainCount - 1) As Integer
        Dim SideH_Road_Count(TerrainSize.X - 1, TerrainSize.Y, Painter.RoadCount - 1) As Integer
        Dim SideV_Road_Count(TerrainSize.X, TerrainSize.Y - 1, Painter.RoadCount - 1) As Integer
        Dim Orientation As sTileDirection

        'count the terrains of influencing tiles
        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                If TerrainTile(X, Z).Texture.TextureNum >= 0 Then
                    For A = 0 To Painter.TerrainCount - 1
                        With Painter.Terrains(A)
                            For B = 0 To .Tiles.TileCount - 1
                                If .Tiles.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Num) += 1
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.TransitionBrushCount - 1
                        With Painter.TransitionBrushes(A)
                            For B = 0 To .Tiles_Straight.TileCount - 1
                                If .Tiles_Straight.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Straight.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_In.TileCount - 1
                                If .Tiles_Corner_In.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_In.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_Out.TileCount - 1
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_Out.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    End If
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.CliffBrushCount - 1
                        With Painter.CliffBrushes(A)
                            For B = 0 To .Tiles_Straight.TileCount - 1
                                If .Tiles_Straight.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Straight.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                    End If
                                    If TerrainTile(X, Z).Tri Then
                                        TerrainTile(X, Z).TriTopLeftIsCliff = True
                                        TerrainTile(X, Z).TriBottomRightIsCliff = True
                                    Else
                                        TerrainTile(X, Z).TriTopRightIsCliff = True
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = True
                                    End If
                                    TerrainTile(X, Z).Terrain_IsCliff = True
                                    TerrainTile(X, Z).DownSide = Orientation
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_In.TileCount - 1
                                If .Tiles_Corner_In.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_In.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = True
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = True
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = True
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = True
                                    End If
                                    TerrainTile(X, Z).Terrain_IsCliff = True
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_Out.TileCount - 1
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_Out.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Inner.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = True
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = True
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = True
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = False
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        Vertex_Terrain_Count(X, Z, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z, .Terrain_Inner.Num) += 1
                                        Vertex_Terrain_Count(X, Z + 1, .Terrain_Outer.Num) += 1
                                        Vertex_Terrain_Count(X + 1, Z + 1, .Terrain_Outer.Num) += 1
                                        TerrainTile(X, Z).TriTopLeftIsCliff = False
                                        TerrainTile(X, Z).TriBottomRightIsCliff = False
                                        TerrainTile(X, Z).TriTopRightIsCliff = True
                                        TerrainTile(X, Z).TriBottomLeftIsCliff = False
                                    End If
                                    TerrainTile(X, Z).Terrain_IsCliff = True
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.RoadBrushCount - 1
                        With Painter.RoadBrushes(A)
                            For B = 0 To .Tile_Corner_In.TileCount - 1
                                If .Tile_Corner_In.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_Corner_In.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_CrossIntersection.TileCount - 1
                                If .Tile_CrossIntersection.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Terrain.Num) += 1
                                    SideH_Road_Count(X, Z, .Road.Num) += 1
                                    SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                    SideV_Road_Count(X, Z, .Road.Num) += 1
                                    SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                End If
                            Next
                            For B = 0 To .Tile_End.TileCount - 1
                                If .Tile_End.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_End.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_Straight.TileCount - 1
                                If .Tile_Straight.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_Straight.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    End If
                                End If
                            Next
                            For B = 0 To .Tile_TIntersection.TileCount - 1
                                If .Tile_TIntersection.Tiles(B).TextureNum = TerrainTile(X, Z).Texture.TextureNum Then
                                    Vertex_Terrain_Count(X, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X, Z + 1, .Terrain.Num) += 1
                                    Vertex_Terrain_Count(X + 1, Z + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_TIntersection.Tiles(B).Direction, TerrainTile(X, Z).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_Top) Then
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Right) Then
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Bottom) Then
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                        SideV_Road_Count(X + 1, Z, .Road.Num) += 1
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_Left) Then
                                        SideV_Road_Count(X, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z, .Road.Num) += 1
                                        SideH_Road_Count(X, Z + 1, .Road.Num) += 1
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
        For Z = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.TerrainCount - 1
                    If Vertex_Terrain_Count(X, Z, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = Vertex_Terrain_Count(X, Z, A)
                    End If
                Next
                If Best_Count > 0 Then
                    TerrainVertex(X, Z).Terrain = Painter.Terrains(Best_Num)
                End If
            Next
        Next
        'designate h roads
        For Z = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X - 1
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.RoadCount - 1
                    If SideH_Road_Count(X, Z, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = SideH_Road_Count(X, Z, A)
                    End If
                Next
                If Best_Count > 0 Then
                    TerrainSideH(X, Z).Road = Painter.Roads(Best_Num)
                End If
            Next
        Next
        'designate v roads
        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X
                Best_Num = -1
                Best_Count = 0
                For A = 0 To Painter.RoadCount - 1
                    If SideV_Road_Count(X, Z, A) > Best_Count Then
                        Best_Num = A
                        Best_Count = SideV_Road_Count(X, Z, A)
                    End If
                Next
                If Best_Count > 0 Then
                    TerrainSideV(X, Z).Road = Painter.Roads(Best_Num)
                End If
            Next
        Next
    End Sub

    Public Sub SelectedUnit_Add(ByVal NewSelectedUnit As clsUnit)

        If NewSelectedUnit.SelectedUnitNum >= 0 Then Exit Sub

        NewSelectedUnit.SelectedUnitNum = SelectedUnitCount

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
            If NewSelectedUnits(A).SelectedUnitNum < 0 Then
                NewSelectedUnits(A).SelectedUnitNum = SelectedUnitCount + Count
                SelectedUnits(SelectedUnitCount + Count) = NewSelectedUnits(A)
                Count += 1
            End If
        Next

        SelectedUnitCount += Count
        ReDim Preserve SelectedUnits(SelectedUnitCount - 1)
    End Sub

    Public Sub SelectedUnit_Remove(ByVal Num As Integer)

        SelectedUnits(Num).SelectedUnitNum = -1

        SelectedUnitCount -= 1
        If Num <> SelectedUnitCount Then
            SelectedUnits(Num) = SelectedUnits(SelectedUnitCount)
            SelectedUnits(Num).SelectedUnitNum = Num
        End If
        ReDim Preserve SelectedUnits(SelectedUnitCount - 1)
    End Sub

    Public Sub SelectedUnits_Clear()
        Dim A As Integer

        For A = 0 To SelectedUnitCount - 1
            SelectedUnits(A).SelectedUnitNum = -1
        Next

        ReDim SelectedUnits(-1)
        SelectedUnitCount = 0
    End Sub

    Public Sub WaterTriCorrection()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                If TerrainTile(X, Y).Tri Then
                    If TerrainTile(X, Y).Texture.TextureNum >= 0 Then
                        If Tileset.Tiles(TerrainTile(X, Y).Texture.TextureNum).Default_Type = TileType_WaterNum Then
                            TerrainTile(X, Y).Tri = False
                            SectorChange.Tile_Set_Changed(X, Y)
                        End If
                    End If
                End If
            Next
        Next
    End Sub

    Public Sub SetPainterToDefaults()
        Dim NewBrushCliff As sPainter.sCliff_Brush
        Dim NewBrush As sPainter.sTransition_Brush
        Dim NewRoadBrush As sPainter.sRoad_Brush

        Painter = New sPainter

        If Tileset Is Tileset_Arizona Then
            'arizona

            Dim Terrain_Red As New sPainter.clsTerrain
            Terrain_Red.Name = "Red"
            Painter.Terrain_Add(Terrain_Red)

            Dim Terrain_Yellow As New sPainter.clsTerrain
            Terrain_Yellow.Name = "Yellow"
            Painter.Terrain_Add(Terrain_Yellow)

            Dim Terrain_Sand As New sPainter.clsTerrain
            Terrain_Sand.Name = "Sand"
            Painter.Terrain_Add(Terrain_Sand)

            Dim Terrain_Brown As New sPainter.clsTerrain
            Terrain_Brown.Name = "Brown"
            Painter.Terrain_Add(Terrain_Brown)

            Dim Terrain_Green As New sPainter.clsTerrain
            Terrain_Green.Name = "Green"
            Painter.Terrain_Add(Terrain_Green)

            Dim Terrain_Concrete As New sPainter.clsTerrain
            Terrain_Concrete.Name = "Concrete"
            Painter.Terrain_Add(Terrain_Concrete)

            Dim Terrain_Water As New sPainter.clsTerrain
            Terrain_Water.Name = "Water"
            Painter.Terrain_Add(Terrain_Water)

            'red centre brush
            Terrain_Red.Tiles.Tile_Add(48, TileDirection_None, 1)
            Terrain_Red.Tiles.Tile_Add(53, TileDirection_None, 1)
            Terrain_Red.Tiles.Tile_Add(54, TileDirection_None, 1)
            Terrain_Red.Tiles.Tile_Add(76, TileDirection_None, 1)
            'yellow centre brushTerrain_yellow
            Terrain_Yellow.Tiles.Tile_Add(9, TileDirection_None, 1)
            Terrain_Yellow.Tiles.Tile_Add(11, TileDirection_None, 1)
            'sand centre brush
            Terrain_Sand.Tiles.Tile_Add(12, TileDirection_None, 1)
            'brown centre brush
            Terrain_Brown.Tiles.Tile_Add(5, TileDirection_None, 1)
            Terrain_Brown.Tiles.Tile_Add(6, TileDirection_None, 1)
            Terrain_Brown.Tiles.Tile_Add(7, TileDirection_None, 1)
            Terrain_Brown.Tiles.Tile_Add(8, TileDirection_None, 1)
            'green centre brush
            Terrain_Green.Tiles.Tile_Add(23, TileDirection_None, 1)
            'concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
            'water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
            'red cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Red Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Red
            NewBrushCliff.Terrain_Outer = Terrain_Red
            NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(75, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'water to sand transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water->Sand"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Sand
            NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'water to green transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water->Green"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Green
            NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(33, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(32, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'yellow to red transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Yellow->Red"
            NewBrush.Terrain_Inner = Terrain_Yellow
            NewBrush.Terrain_Outer = Terrain_Red
            NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(28, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'sand to red transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Sand->Red"
            NewBrush.Terrain_Inner = Terrain_Sand
            NewBrush.Terrain_Outer = Terrain_Red
            NewBrush.Tiles_Straight.Tile_Add(43, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(42, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(41, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'sand to yellow transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Sand->Yellow"
            NewBrush.Terrain_Inner = Terrain_Sand
            NewBrush.Terrain_Outer = Terrain_Yellow
            NewBrush.Tiles_Straight.Tile_Add(10, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(1, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(0, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'brown to red transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Brown->Red"
            NewBrush.Terrain_Inner = Terrain_Brown
            NewBrush.Terrain_Outer = Terrain_Red
            NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(36, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'brown to yellow transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Brown->Yellow"
            NewBrush.Terrain_Inner = Terrain_Brown
            NewBrush.Terrain_Outer = Terrain_Yellow
            NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(39, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(40, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'brown to sand transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Brown->Sand"
            NewBrush.Terrain_Inner = Terrain_Brown
            NewBrush.Terrain_Outer = Terrain_Sand
            NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'brown to green transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Brown->Green"
            NewBrush.Terrain_Inner = Terrain_Brown
            NewBrush.Terrain_Outer = Terrain_Green
            NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(26, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(25, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'concrete to red transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Concrete->Red"
            NewBrush.Terrain_Inner = Terrain_Concrete
            NewBrush.Terrain_Outer = Terrain_Red
            NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)


            Dim Road_Road As New sPainter.clsRoad

            Road_Road = New sPainter.clsRoad
            Road_Road.Name = "Road"
            Painter.Road_Add(Road_Road)

            Dim Road_Track As New sPainter.clsRoad

            Road_Track = New sPainter.clsRoad
            Road_Track.Name = "Track"
            Painter.Road_Add(Road_Track)

            'road
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Red
            NewRoadBrush.Tile_TIntersection.Tile_Add(57, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(47, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'track
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Track
            NewRoadBrush.Terrain = Terrain_Red
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(73, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
            NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
            NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
        ElseIf Tileset Is Tileset_Urban Then
            'urban

            Dim Terrain_Green As New sPainter.clsTerrain
            Terrain_Green.Name = "Green"
            Painter.Terrain_Add(Terrain_Green)

            Dim Terrain_Blue As New sPainter.clsTerrain
            Terrain_Blue.Name = "Blue"
            Painter.Terrain_Add(Terrain_Blue)

            Dim Terrain_Gray As New sPainter.clsTerrain
            Terrain_Gray.Name = "Gray"
            Painter.Terrain_Add(Terrain_Gray)

            Dim Terrain_Orange As New sPainter.clsTerrain
            Terrain_Orange.Name = "Orange"
            Painter.Terrain_Add(Terrain_Orange)

            Dim Terrain_Concrete As New sPainter.clsTerrain
            Terrain_Concrete.Name = "Concrete"
            Painter.Terrain_Add(Terrain_Concrete)

            Dim Terrain_Water As New sPainter.clsTerrain
            Terrain_Water.Name = "Water"
            Painter.Terrain_Add(Terrain_Water)

            'green centre brush
            Terrain_Green.Tiles.Tile_Add(50, TileDirection_None, 1)
            'blue centre brush
            Terrain_Blue.Tiles.Tile_Add(0, TileDirection_None, 14)
            Terrain_Blue.Tiles.Tile_Add(2, TileDirection_None, 1) 'line
            'gray centre brush
            Terrain_Gray.Tiles.Tile_Add(5, TileDirection_None, 1)
            Terrain_Gray.Tiles.Tile_Add(7, TileDirection_None, 4)
            Terrain_Gray.Tiles.Tile_Add(8, TileDirection_None, 4)
            Terrain_Gray.Tiles.Tile_Add(78, TileDirection_None, 4)
            'orange centre brush
            Terrain_Orange.Tiles.Tile_Add(31, TileDirection_None, 1) 'pipe
            Terrain_Orange.Tiles.Tile_Add(22, TileDirection_None, 50)
            'concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(51, TileDirection_None, 200)
            'water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
            'cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Gray
            NewBrushCliff.Terrain_Outer = Terrain_Gray
            NewBrushCliff.Tiles_Straight.Tile_Add(69, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Straight.Tile_Add(70, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(68, TileDirection_TopRight, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(68, TileDirection_BottomLeft, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'water to gray transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water->Gray"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Gray
            NewBrush.Tiles_Straight.Tile_Add(23, TileDirection_Left, 1)
            NewBrush.Tiles_Straight.Tile_Add(24, TileDirection_Top, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(26, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'water to concrete transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water->Concrete"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Concrete
            NewBrush.Tiles_Straight.Tile_Add(13, TileDirection_Left, 1)
            NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'gray to blue transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Gray->Blue"
            NewBrush.Terrain_Inner = Terrain_Gray
            NewBrush.Terrain_Outer = Terrain_Blue
            NewBrush.Tiles_Straight.Tile_Add(6, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(4, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(3, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'concrete to gray transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Concrete->Gray"
            NewBrush.Terrain_Inner = Terrain_Concrete
            NewBrush.Terrain_Outer = Terrain_Gray
            NewBrush.Tiles_Straight.Tile_Add(9, TileDirection_Left, 1)
            NewBrush.Tiles_Straight.Tile_Add(27, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(30, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(10, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(29, TileDirection_BottomLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'orange to blue transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Orange->Blue"
            NewBrush.Terrain_Inner = Terrain_Orange
            NewBrush.Terrain_Outer = Terrain_Blue
            NewBrush.Tiles_Straight.Tile_Add(33, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(34, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(35, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'orange to green transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Orange->Green"
            NewBrush.Terrain_Inner = Terrain_Orange
            NewBrush.Terrain_Outer = Terrain_Green
            NewBrush.Tiles_Straight.Tile_Add(39, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(38, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(37, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'orange to gray transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Orange->Gray"
            NewBrush.Terrain_Inner = Terrain_Orange
            NewBrush.Terrain_Outer = Terrain_Gray
            NewBrush.Tiles_Straight.Tile_Add(60, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(73, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(72, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'orange to concrete transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Orange->Concrete"
            NewBrush.Terrain_Inner = Terrain_Orange
            NewBrush.Terrain_Outer = Terrain_Concrete
            NewBrush.Tiles_Straight.Tile_Add(71, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(76, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(75, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'gray to green transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Gray->Green"
            NewBrush.Terrain_Inner = Terrain_Gray
            NewBrush.Terrain_Outer = Terrain_Green
            NewBrush.Tiles_Straight.Tile_Add(77, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(58, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(79, TileDirection_BottomLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)

            'road
            Dim Road_Road As New sPainter.clsRoad
            Road_Road.Name = "Road"
            Painter.Road_Add(Road_Road)
            'road green
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Green
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(45, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'road blue
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Blue
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(41, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'road gray
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Gray
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(43, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(44, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'road orange
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Orange
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'road concrete
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Concrete
            NewRoadBrush.Tile_CrossIntersection.Tile_Add(49, TileDirection_None, 1)
            NewRoadBrush.Tile_TIntersection.Tile_Add(40, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(42, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
        ElseIf Tileset Is Tileset_Rockies Then
            'rockies

            Dim Terrain_Grass As New sPainter.clsTerrain
            Terrain_Grass.Name = "Grass"
            Painter.Terrain_Add(Terrain_Grass)

            Dim Terrain_Gravel As New sPainter.clsTerrain
            Terrain_Gravel.Name = "Gravel"
            Painter.Terrain_Add(Terrain_Gravel)

            Dim Terrain_Dirt As New sPainter.clsTerrain
            Terrain_Dirt.Name = "Dirt"
            Painter.Terrain_Add(Terrain_Dirt)

            Dim Terrain_GrassSnow As New sPainter.clsTerrain
            Terrain_GrassSnow.Name = "Grass Snow"
            Painter.Terrain_Add(Terrain_GrassSnow)

            Dim Terrain_GravelSnow As New sPainter.clsTerrain
            Terrain_GravelSnow.Name = "Gravel Snow"
            Painter.Terrain_Add(Terrain_GravelSnow)

            Dim Terrain_Snow As New sPainter.clsTerrain
            Terrain_Snow.Name = "Snow"
            Painter.Terrain_Add(Terrain_Snow)

            Dim Terrain_Concrete As New sPainter.clsTerrain
            Terrain_Concrete.Name = "Concrete"
            Painter.Terrain_Add(Terrain_Concrete)

            Dim Terrain_Water As New sPainter.clsTerrain
            Terrain_Water.Name = "Water"
            Painter.Terrain_Add(Terrain_Water)

            'grass centre brush
            Terrain_Grass.Tiles.Tile_Add(0, TileDirection_None, 1)
            'gravel centre brush
            Terrain_Gravel.Tiles.Tile_Add(5, TileDirection_None, 1)
            Terrain_Gravel.Tiles.Tile_Add(6, TileDirection_None, 1)
            Terrain_Gravel.Tiles.Tile_Add(7, TileDirection_None, 1)
            'dirt centre brush
            Terrain_Dirt.Tiles.Tile_Add(53, TileDirection_None, 1)
            'grass snow centre brush
            Terrain_GrassSnow.Tiles.Tile_Add(23, TileDirection_None, 1)
            'gravel snow centre brush
            Terrain_GravelSnow.Tiles.Tile_Add(41, TileDirection_None, 1)
            'snow centre brush
            Terrain_Snow.Tiles.Tile_Add(64, TileDirection_None, 1)
            'concrete centre brush
            Terrain_Concrete.Tiles.Tile_Add(22, TileDirection_None, 1)
            'water centre brush
            Terrain_Water.Tiles.Tile_Add(17, TileDirection_None, 1)
            'gravel to gravel cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Gravel Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Gravel
            NewBrushCliff.Terrain_Outer = Terrain_Gravel
            NewBrushCliff.Tiles_Straight.Tile_Add(46, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Straight.Tile_Add(71, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(45, TileDirection_TopRight, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(45, TileDirection_BottomLeft, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'gravel snow to gravel cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Gravel Snow -> Gravel Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
            NewBrushCliff.Terrain_Outer = Terrain_Gravel
            NewBrushCliff.Tiles_Straight.Tile_Add(29, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'snow to gravel cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Snow -> Gravel Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Snow
            NewBrushCliff.Terrain_Outer = Terrain_Gravel
            NewBrushCliff.Tiles_Straight.Tile_Add(68, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(42, TileDirection_BottomLeft, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'gravel snow cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Gravel Snow Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_GravelSnow
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
            NewBrushCliff.Tiles_Straight.Tile_Add(44, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(9, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'snow to gravel snow cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Snow -> Gravel Snow Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Snow
            NewBrushCliff.Terrain_Outer = Terrain_GravelSnow
            NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(9, TileDirection_BottomRight, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'snow to snow cliff brush
            NewBrushCliff = New sPainter.sCliff_Brush
            NewBrushCliff.Name = "Snow -> Snow Cliff"
            NewBrushCliff.Terrain_Inner = Terrain_Snow
            NewBrushCliff.Terrain_Outer = Terrain_Snow
            NewBrushCliff.Tiles_Straight.Tile_Add(78, TileDirection_Bottom, 1)
            NewBrushCliff.Tiles_Corner_In.Tile_Add(63, TileDirection_TopLeft, 1)
            NewBrushCliff.Tiles_Corner_Out.Tile_Add(63, TileDirection_BottomRight, 1)
            Painter.CliffBrush_Add(NewBrushCliff)
            'water to grass transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water -> Grass"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Grass
            NewBrush.Tiles_Straight.Tile_Add(14, TileDirection_Bottom, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(16, TileDirection_BottomLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(15, TileDirection_BottomLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'water to gravel transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Water -> Gravel"
            NewBrush.Terrain_Inner = Terrain_Water
            NewBrush.Terrain_Outer = Terrain_Gravel
            NewBrush.Tiles_Straight.Tile_Add(31, TileDirection_Top, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(32, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(33, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'grass to gravel transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Grass -> Gravel"
            NewBrush.Terrain_Inner = Terrain_Grass
            NewBrush.Terrain_Outer = Terrain_Gravel
            NewBrush.Tiles_Straight.Tile_Add(2, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(3, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(4, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'grass to grass snow transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Grass -> Grass Snow"
            NewBrush.Terrain_Inner = Terrain_Grass
            NewBrush.Terrain_Outer = Terrain_GrassSnow
            NewBrush.Tiles_Straight.Tile_Add(26, TileDirection_Top, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(25, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(24, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'grass to dirt transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Grass -> Dirt"
            NewBrush.Terrain_Inner = Terrain_Grass
            NewBrush.Terrain_Outer = Terrain_Dirt
            NewBrush.Tiles_Straight.Tile_Add(34, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(35, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(36, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'gravel snow to gravel transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Gravel Snow -> Gravel"
            NewBrush.Terrain_Inner = Terrain_GravelSnow
            NewBrush.Terrain_Outer = Terrain_Gravel
            NewBrush.Tiles_Straight.Tile_Add(12, TileDirection_Bottom, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(10, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(11, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'snow to gravel snow transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Snow -> Gravel Snow"
            NewBrush.Terrain_Inner = Terrain_Snow
            NewBrush.Terrain_Outer = Terrain_GravelSnow
            NewBrush.Tiles_Straight.Tile_Add(67, TileDirection_Bottom, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(65, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(66, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'concrete to dirt transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Concrete -> Dirt"
            NewBrush.Terrain_Inner = Terrain_Concrete
            NewBrush.Terrain_Outer = Terrain_Dirt
            NewBrush.Tiles_Straight.Tile_Add(21, TileDirection_Right, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(19, TileDirection_BottomRight, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(20, TileDirection_BottomRight, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'gravel to dirt transition brush
            NewBrush = New sPainter.sTransition_Brush
            NewBrush.Name = "Gravel -> Dirt"
            NewBrush.Terrain_Inner = Terrain_Gravel
            NewBrush.Terrain_Outer = Terrain_Dirt
            NewBrush.Tiles_Straight.Tile_Add(38, TileDirection_Left, 1)
            NewBrush.Tiles_Corner_In.Tile_Add(40, TileDirection_TopLeft, 1)
            NewBrush.Tiles_Corner_Out.Tile_Add(39, TileDirection_TopLeft, 1)
            Painter.TransitionBrush_Add(NewBrush)
            'road
            Dim Road_Road As New sPainter.clsRoad
            Road_Road.Name = "Road"
            Painter.Road_Add(Road_Road)
            'road brown
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Road
            NewRoadBrush.Terrain = Terrain_Dirt
            NewRoadBrush.Tile_TIntersection.Tile_Add(13, TileDirection_Bottom, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(59, TileDirection_Left, 1)
            NewRoadBrush.Tile_End.Tile_Add(60, TileDirection_Left, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
            'track
            Dim Road_Track As New sPainter.clsRoad
            Road_Track.Name = "Track"
            Painter.Road_Add(Road_Track)
            'track brown
            NewRoadBrush = New sPainter.sRoad_Brush
            NewRoadBrush.Road = Road_Track
            NewRoadBrush.Terrain = Terrain_Dirt
            NewRoadBrush.Tile_TIntersection.Tile_Add(72, TileDirection_Right, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(49, TileDirection_Top, 1)
            NewRoadBrush.Tile_Straight.Tile_Add(51, TileDirection_Top, 2)
            NewRoadBrush.Tile_Corner_In.Tile_Add(50, TileDirection_BottomRight, 1)
            NewRoadBrush.Tile_End.Tile_Add(52, TileDirection_Bottom, 1)
            Painter.RoadBrush_Add(NewRoadBrush)
        End If
    End Sub
End Class