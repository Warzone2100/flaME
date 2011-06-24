Partial Public Class clsMap

    Sub Rotate_Clockwise(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Y As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim tmpGateways() As sGateway
        Dim TileCount As New sXY_int(TerrainSize.Y, TerrainSize.X)
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCount.X, TileCount.Y)
        ReDim tmpTerrainTile(TileCount.X - 1, TileCount.Y - 1)
        ReDim tmpTerrainSideH(TileCount.X - 1, TileCount.Y)
        ReDim tmpTerrainSideV(TileCount.X, TileCount.Y - 1)

        For Y = 0 To TileCount.Y
            For X = 0 To TileCount.X
                tmpTerrainVertex(X, Y) = TerrainVertex(Y, TerrainSize.Y - X)
            Next
        Next
        For Y = 0 To TileCount.Y - 1
            For X = 0 To TileCount.X - 1
                X2 = TerrainSize.Y - X - 1
                tmpTerrainTile(X, Y).Texture = TerrainTiles(Y, X2).Texture
                tmpTerrainTile(X, Y).Texture.Orientation.RotateClockwise()
                tmpTerrainTile(X, Y).DownSide = TerrainTiles(Y, X2).DownSide
                tmpTerrainTile(X, Y).DownSide.RotateClockwise()
                tmpTerrainTile(X, Y).Tri = Not TerrainTiles(Y, X2).Tri
                tmpTerrainTile(X, Y).TriTopLeftIsCliff = TerrainTiles(Y, X2).TriBottomLeftIsCliff
                tmpTerrainTile(X, Y).TriBottomLeftIsCliff = TerrainTiles(Y, X2).TriBottomRightIsCliff
                tmpTerrainTile(X, Y).TriBottomRightIsCliff = TerrainTiles(Y, X2).TriTopRightIsCliff
                tmpTerrainTile(X, Y).TriTopRightIsCliff = TerrainTiles(Y, X2).TriTopLeftIsCliff
            Next
        Next
        For Y = 0 To TileCount.Y
            For X = 0 To TileCount.X - 1
                tmpTerrainSideH(X, Y) = TerrainSideV(Y, TerrainSize.Y - X - 1)
            Next
        Next
        For Y = 0 To TileCount.Y - 1
            For X = 0 To TileCount.X
                tmpTerrainSideV(X, Y) = TerrainSideH(Y, TerrainSize.Y - X)
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
            Units(A).Pos.Horizontal.X = TerrainSize.Y * TerrainGridSpacing - Units(A).Pos.Horizontal.Y
            Units(A).Pos.Horizontal.Y = intTemp
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCount.X / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCount.Y / SectorTileSize)
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, TileCount) Then
                Unit_Sectors_Calc(A)
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        TerrainSize = TileCount
        TerrainVertex = tmpTerrainVertex
        TerrainTiles = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV
        Gateways = tmpGateways

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.ParentMap = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange.Parent_Map = Nothing
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Sub Rotate_CounterClockwise(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim X As Integer
        Dim Z As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim tmpGateways() As sGateway
        Dim TileCount As New sXY_int(TerrainSize.Y, TerrainSize.X)
        Dim Z2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCount.X, TileCount.Y)
        ReDim tmpTerrainTile(TileCount.X - 1, TileCount.Y - 1)
        ReDim tmpTerrainSideH(TileCount.X - 1, TileCount.Y)
        ReDim tmpTerrainSideV(TileCount.X, TileCount.Y - 1)

        For Z = 0 To TileCount.Y
            For X = 0 To TileCount.X
                tmpTerrainVertex(X, Z) = TerrainVertex(TerrainSize.X - Z, X)
            Next
        Next
        For Z = 0 To TileCount.Y - 1
            Z2 = TerrainSize.X - Z - 1
            For X = 0 To TileCount.X - 1
                tmpTerrainTile(X, Z).Texture = TerrainTiles(Z2, X).Texture
                tmpTerrainTile(X, Z).Texture.Orientation.RotateAnticlockwise()
                tmpTerrainTile(X, Z).DownSide = TerrainTiles(Z2, X).DownSide
                tmpTerrainTile(X, Z).DownSide.RotateAnticlockwise()
                tmpTerrainTile(X, Z).Tri = Not TerrainTiles(Z2, X).Tri
                tmpTerrainTile(X, Z).TriTopLeftIsCliff = TerrainTiles(Z2, X).TriTopRightIsCliff
                tmpTerrainTile(X, Z).TriBottomLeftIsCliff = TerrainTiles(Z2, X).TriTopLeftIsCliff
                tmpTerrainTile(X, Z).TriBottomRightIsCliff = TerrainTiles(Z2, X).TriBottomLeftIsCliff
                tmpTerrainTile(X, Z).TriTopRightIsCliff = TerrainTiles(Z2, X).TriBottomRightIsCliff
            Next
        Next
        For Z = 0 To TileCount.Y
            For X = 0 To TileCount.X - 1
                tmpTerrainSideH(X, Z) = TerrainSideV(TerrainSize.X - Z, X)
            Next
        Next
        For Z = 0 To TileCount.Y - 1
            For X = 0 To TileCount.X
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
            Units(A).Pos.Horizontal.Y = TerrainSize.X * TerrainGridSpacing - Units(A).Pos.Horizontal.X
            Units(A).Pos.Horizontal.X = intTemp
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCount.X / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCount.Y / SectorTileSize)
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Z) = New clsSector(New sXY_int(X, Z))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, TileCount) Then
                Unit_Sectors_Calc(A)
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        TerrainSize = TileCount
        TerrainVertex = tmpTerrainVertex
        TerrainTiles = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV
        Gateways = tmpGateways

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.ParentMap = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange.Parent_Map = Nothing
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Sub Rotate_FlipX(ByVal ObjectRotateMode As enumObjectRotateMode)
        Dim Z As Integer
        Dim X As Integer
        Dim tmpTerrainVertex(,) As sTerrainVertex
        Dim tmpTerrainTile(,) As sTerrainTile
        Dim tmpTerrainSideH(,) As sTerrainSide
        Dim tmpTerrainSideV(,) As sTerrainSide
        Dim tmpGateways() As sGateway
        Dim TileCount As sXY_int = TerrainSize
        Dim X2 As Integer

        Undo_Clear()
        SectorAll_GLLists_Delete()

        ReDim tmpTerrainVertex(TileCount.X, TileCount.Y)
        ReDim tmpTerrainTile(TileCount.X - 1, TileCount.Y - 1)
        ReDim tmpTerrainSideH(TileCount.X - 1, TileCount.Y)
        ReDim tmpTerrainSideV(TileCount.X, TileCount.Y - 1)

        For Z = 0 To TileCount.Y
            For X = 0 To TileCount.X
                tmpTerrainVertex(X, Z) = TerrainVertex(TerrainSize.X - X, Z)
            Next
        Next
        For Z = 0 To TileCount.Y - 1
            For X = 0 To TileCount.X - 1
                X2 = TerrainSize.X - X - 1
                tmpTerrainTile(X, Z).Texture = TerrainTiles(X2, Z).Texture
                tmpTerrainTile(X, Z).Texture.Orientation.ResultXFlip = Not tmpTerrainTile(X, Z).Texture.Orientation.ResultXFlip
                tmpTerrainTile(X, Z).DownSide = TerrainTiles(X2, Z).DownSide
                tmpTerrainTile(X2, Z).DownSide.FlipX()
                tmpTerrainTile(X, Z).Tri = Not TerrainTiles(X2, Z).Tri
                tmpTerrainTile(X, Z).TriTopLeftIsCliff = TerrainTiles(X2, Z).TriTopRightIsCliff
                tmpTerrainTile(X, Z).TriBottomLeftIsCliff = TerrainTiles(X2, Z).TriBottomRightIsCliff
                tmpTerrainTile(X, Z).TriBottomRightIsCliff = TerrainTiles(X2, Z).TriBottomLeftIsCliff
                tmpTerrainTile(X, Z).TriTopRightIsCliff = TerrainTiles(X2, Z).TriTopLeftIsCliff
            Next
        Next
        For Z = 0 To TileCount.Y
            For X = 0 To TileCount.X - 1
                tmpTerrainSideH(X, Z) = TerrainSideH(TerrainSize.X - X - 1, Z)
            Next
        Next
        For Z = 0 To TileCount.Y - 1
            For X = 0 To TileCount.X
                tmpTerrainSideV(X, Z) = TerrainSideV(TerrainSize.X - X, Z)
            Next
        Next

        Dim A As Integer

        ReDim tmpGateways(GatewayCount - 1)

        For A = 0 To GatewayCount - 1
            tmpGateways(A).PosA.X = TileCount.X - Gateways(A).PosA.X - 1
            tmpGateways(A).PosA.Y = Gateways(A).PosA.Y
            tmpGateways(A).PosB.X = TileCount.X - Gateways(A).PosB.X - 1
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
            Units(A).Pos.Horizontal.X = TerrainSize.X * TerrainGridSpacing - Units(A).Pos.Horizontal.X
        Next
        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing

        Sectors_Deallocate()
        SectorCount.X = Math.Ceiling(TileCount.X / SectorTileSize)
        SectorCount.Y = Math.Ceiling(TileCount.Y / SectorTileSize)
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Z) = New clsSector(New sXY_int(X, Z))
            Next
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        A = 0
        Do While A < UnitCount
            If PosIsWithinTileArea(Units(A).Pos.Horizontal, ZeroPos, TileCount) Then
                Unit_Sectors_Calc(A)
                A += 1
            Else
                Unit_Remove(A)
            End If
        Loop

        TerrainSize.X = TileCount.X
        TerrainSize.Y = TileCount.Y
        TerrainVertex = tmpTerrainVertex
        TerrainTiles = tmpTerrainTile
        TerrainSideH = tmpTerrainSideH
        TerrainSideV = tmpTerrainSideV
        Gateways = tmpGateways

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        ShadowSector_CreateAll()
        AutoTextureChange.ParentMap = Nothing
        AutoTextureChange = New clsAutoTextureChange(Me)
        SectorGraphicsChange.Parent_Map = Nothing
        SectorGraphicsChange = New clsSectorGraphicsChange(Me)
    End Sub

    Public Sub Apply_Cliff(ByVal Centre As sXY_int, ByVal Tiles As sBrushTiles, ByVal Angle As Double, ByVal SetTris As Boolean)
        Dim A As Integer
        Dim difA As Double
        Dim difB As Double
        Dim HeightA As Double
        Dim HeightB As Double
        Dim TriTopLeftMaxSlope As Double
        Dim TriTopRightMaxSlope As Double
        Dim TriBottomLeftMaxSlope As Double
        Dim TriBottomRightMaxSlope As Double
        Dim CliffChanged As Boolean
        Dim TriChanged As Boolean
        Dim NewVal As Boolean
        Dim X As Integer
        Dim Y As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim XLimit As Integer = TerrainSize.X - 1
        Dim YLimit As Integer = TerrainSize.Y - 1
        Dim TilePos As sXY_int
        Dim Pos As sXY_int

        For Y = Clamp(Tiles.ZMin + Centre.Y, 0, YLimit) - Centre.Y To Clamp(Tiles.ZMax + Centre.Y, 0, YLimit) - Centre.Y
            Y2 = Centre.Y + Y
            For X = Clamp(Tiles.XMin(Y - Tiles.ZMin) + Centre.X, 0, XLimit) - Centre.X To Clamp(Tiles.XMax(Y - Tiles.ZMin) + Centre.X, 0, XLimit) - Centre.X
                X2 = Centre.X + X

                HeightA = (CDbl(TerrainVertex(X2, Y2).Height) + TerrainVertex(X2 + 1, Y2).Height) / 2.0#
                HeightB = (CDbl(TerrainVertex(X2, Y2 + 1).Height) + TerrainVertex(X2 + 1, Y2 + 1).Height) / 2.0#
                difA = HeightB - HeightA
                HeightA = (CDbl(TerrainVertex(X2, Y2).Height) + TerrainVertex(X2, Y2 + 1).Height) / 2.0#
                HeightB = (CDbl(TerrainVertex(X2 + 1, Y2).Height) + TerrainVertex(X2 + 1, Y2 + 1).Height) / 2.0#
                difB = HeightB - HeightA
                If Math.Abs(difA) = Math.Abs(difB) Then
                    A = Int(Rnd() * 4.0F)
                    If A = 0 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Top
                    ElseIf A = 1 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Right
                    ElseIf A = 2 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Bottom
                    ElseIf A = 3 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Left
                    End If
                ElseIf Math.Abs(difA) > Math.Abs(difB) Then
                    If difA < 0 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Bottom
                    ElseIf difA > 0 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Top
                    End If
                Else
                    If difB < 0 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Right
                    ElseIf difB > 0 Then
                        TerrainTiles(X2, Y2).DownSide = TileDirection_Left
                    End If
                End If

                CliffChanged = False
                TriChanged = False

                If SetTris Then
                    difA = Math.Abs(CDbl(TerrainVertex(X2 + 1, Y2 + 1).Height) - TerrainVertex(X2, Y2).Height)
                    difB = Math.Abs(CDbl(TerrainVertex(X2, Y2 + 1).Height) - TerrainVertex(X2 + 1, Y2).Height)
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
                    If TerrainTiles(X2, Y2).Tri <> NewVal Then
                        TerrainTiles(X2, Y2).Tri = NewVal
                        TriChanged = True
                    End If
                End If

                If TerrainTiles(X2, Y2).Tri Then
                    Pos.X = (X2 + 0.25#) * TerrainGridSpacing
                    Pos.Y = (Y2 + 0.25#) * TerrainGridSpacing
                    TriTopLeftMaxSlope = GetTerrainSlopeAngle(Pos)
                    Pos.X = (X2 + 0.75#) * TerrainGridSpacing
                    Pos.Y = (Y2 + 0.75#) * TerrainGridSpacing
                    TriBottomRightMaxSlope = GetTerrainSlopeAngle(Pos)

                    If TerrainTiles(X2, Y2).TriTopRightIsCliff Then
                        TerrainTiles(X2, Y2).TriTopRightIsCliff = False
                        CliffChanged = True
                    End If
                    If TerrainTiles(X2, Y2).TriBottomLeftIsCliff Then
                        TerrainTiles(X2, Y2).TriBottomLeftIsCliff = False
                        CliffChanged = True
                    End If

                    NewVal = (TriTopLeftMaxSlope >= Angle)
                    CliffChanged = (CliffChanged Or Not TerrainTiles(X2, Y2).TriTopLeftIsCliff = NewVal)
                    TerrainTiles(X2, Y2).TriTopLeftIsCliff = NewVal

                    NewVal = (TriBottomRightMaxSlope >= Angle)
                    CliffChanged = (CliffChanged Or Not TerrainTiles(X2, Y2).TriBottomRightIsCliff = NewVal)
                    TerrainTiles(X2, Y2).TriBottomRightIsCliff = NewVal

                    If TerrainTiles(X2, Y2).TriTopLeftIsCliff Or TerrainTiles(X2, Y2).TriBottomRightIsCliff Then
                        TerrainTiles(X2, Y2).Terrain_IsCliff = True
                    Else
                        TerrainTiles(X2, Y2).Terrain_IsCliff = False
                    End If
                Else
                    Pos.X = (X2 + 0.75#) * TerrainGridSpacing
                    Pos.Y = (Y2 + 0.25#) * TerrainGridSpacing
                    TriTopRightMaxSlope = GetTerrainSlopeAngle(Pos)
                    Pos.X = (X2 + 0.25#) * TerrainGridSpacing
                    Pos.Y = (Y2 + 0.75#) * TerrainGridSpacing
                    TriBottomLeftMaxSlope = GetTerrainSlopeAngle(Pos)

                    If TerrainTiles(X2, Y2).TriBottomRightIsCliff Then
                        TerrainTiles(X2, Y2).TriBottomRightIsCliff = False
                        CliffChanged = True
                    End If
                    If TerrainTiles(X2, Y2).TriTopLeftIsCliff Then
                        TerrainTiles(X2, Y2).TriTopLeftIsCliff = False
                        CliffChanged = True
                    End If

                    NewVal = (TriTopRightMaxSlope >= Angle)
                    CliffChanged = (CliffChanged Or Not TerrainTiles(X2, Y2).TriTopRightIsCliff = NewVal)
                    TerrainTiles(X2, Y2).TriTopRightIsCliff = NewVal

                    NewVal = (TriBottomLeftMaxSlope >= Angle)
                    CliffChanged = (CliffChanged Or Not TerrainTiles(X2, Y2).TriBottomLeftIsCliff = NewVal)
                    TerrainTiles(X2, Y2).TriBottomLeftIsCliff = NewVal

                    If TerrainTiles(X2, Y2).TriTopRightIsCliff Or TerrainTiles(X2, Y2).TriBottomLeftIsCliff Then
                        TerrainTiles(X2, Y2).Terrain_IsCliff = True
                    Else
                        TerrainTiles(X2, Y2).Terrain_IsCliff = False
                    End If
                End If

                TilePos.X = X2
                TilePos.Y = Y2

                If CliffChanged Then
                    AutoTextureChange.Tile_Set_Changed(TilePos)
                End If
                If TriChanged Or CliffChanged Then
                    SectorGraphicsChange.Tile_Set_Changed(TilePos)
                End If
            Next
        Next
    End Sub

    Public Sub Apply_CliffRemove(ByVal Centre As sXY_int, ByVal Tiles As sBrushTiles)
        Dim X As Integer
        Dim Y As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim XLimit As Integer = TerrainSize.X - 1
        Dim YLimit As Integer = TerrainSize.Y - 1
        Dim TilePos As sXY_int

        For Y = Clamp(Tiles.ZMin + Centre.Y, 0, YLimit) - Centre.Y To Clamp(Tiles.ZMax + Centre.Y, 0, YLimit) - Centre.Y
            Y2 = Centre.Y + Y
            For X = Clamp(Tiles.XMin(Y - Tiles.ZMin) + Centre.X, 0, XLimit) - Centre.X To Clamp(Tiles.XMax(Y - Tiles.ZMin) + Centre.X, 0, XLimit) - Centre.X
                X2 = Centre.X + X

                If TerrainTiles(X2, Y2).Terrain_IsCliff Or TerrainTiles(X2, Y2).TriBottomLeftIsCliff Or TerrainTiles(X2, Y2).TriBottomRightIsCliff Or TerrainTiles(X2, Y2).TriTopLeftIsCliff Or TerrainTiles(X2, Y2).TriTopRightIsCliff Then

                    TerrainTiles(X2, Y2).Terrain_IsCliff = False
                    TerrainTiles(X2, Y2).TriBottomLeftIsCliff = False
                    TerrainTiles(X2, Y2).TriBottomRightIsCliff = False
                    TerrainTiles(X2, Y2).TriTopLeftIsCliff = False
                    TerrainTiles(X2, Y2).TriTopRightIsCliff = False

                    TilePos.X = X2
                    TilePos.Y = Y2

                    Main_Map.AutoTextureChange.Tile_Set_Changed(TilePos)

                    SectorGraphicsChange.Tile_Set_Changed(TilePos)
                End If
            Next
        Next
    End Sub

    Public Sub RandomizeHeights(ByVal LevelCount As Integer)
        Dim hmSource As New clsHeightmap
        Dim hmA As New clsHeightmap
        Dim hmB As New clsHeightmap
        Dim IntervalCount As Integer
        Dim AlterationLevel() As Double
        Dim hmAlteration() As clsHeightmap
        Dim LevelHeight As Double
        Dim HeightRange As Double
        Dim Level As Integer
        Dim IntervalHeight As Double
        Dim Variation As Double
        Dim X As Integer
        Dim Y As Integer

        IntervalCount = LevelCount - 1

        ReDim AlterationLevel(IntervalCount)
        Dim MinMax As New clsHeightmap.sMinMax
        ReDim hmAlteration(IntervalCount)
        ReDim hmSource.HeightData.Height(TerrainSize.Y, TerrainSize.X)
        hmSource.HeightData.SizeX = TerrainSize.X + 1
        hmSource.HeightData.SizeY = TerrainSize.Y + 1
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                hmSource.HeightData.Height(Y, X) = TerrainVertex(X, Y).Height / hmSource.HeightScale
            Next
        Next
        hmSource.MinMaxGet(MinMax)
        HeightRange = 255.0#
        IntervalHeight = HeightRange / IntervalCount
        Variation = IntervalHeight / 4.0#
        For Level = 0 To IntervalCount
            LevelHeight = (MinMax.Min + Level * MinMax.Max / IntervalCount) * hmSource.HeightScale
            AlterationLevel(Level) = LevelHeight
            hmB.GenerateNewOfSize(TerrainSize.Y + 1, TerrainSize.X + 1, 2.0F, 10000.0#)
            hmAlteration(Level) = New clsHeightmap
            hmAlteration(Level).Rescale(hmB, LevelHeight - Variation, LevelHeight + Variation)
        Next
        hmA.FadeMultiple(hmSource, hmAlteration, AlterationLevel)
        hmB.Rescale(hmA, Math.Max(MinMax.Min * hmSource.HeightScale - Variation, 0.0#), Math.Min(MinMax.Max * hmSource.HeightScale + Variation, 255.9#))
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                TerrainVertex(X, Y).Height = Int(hmB.HeightData.Height(Y, X) * hmB.HeightScale)
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

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TextureNum = TerrainTiles(X, Y).Texture.TextureNum
                If TextureNum >= 0 And TextureNum < Tileset.TileCount Then
                    If Tileset.Tiles(TextureNum).Default_Type = TileTypeNum_Water Then
                        TerrainVertex(X, Y).Height = 0
                        TerrainVertex(X + 1, Y).Height = 0
                        TerrainVertex(X, Y + 1).Height = 0
                        TerrainVertex(X + 1, Y + 1).Height = 0
                    End If
                End If
            Next
        Next
    End Sub

    Public Structure sGenerateMasterTerrainArgs
        Public Tileset As clsGeneratorTileset
        Public LevelCount As Integer
        Class clsLayer
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
        Dim Z As Integer
        Dim A As Integer
        Dim Terrain(,) As Integer
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

        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                difA = Math.Abs(CDbl(TerrainVertex(X + 1, Z + 1).Height) - TerrainVertex(X, Z).Height)
                difB = Math.Abs(CDbl(TerrainVertex(X, Z + 1).Height) - TerrainVertex(X + 1, Z).Height)
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
                If Not TerrainTiles(X, Z).Tri = NewTri Then
                    TerrainTiles(X, Z).Tri = NewTri
                End If
            Next X
        Next Z

        For A = 0 To Args.LayerCount - 1
            Args.Layers(A).Terrainmap = New clsBooleanMap
            If Args.Layers(A).TerrainmapDensity = 1.0F Then
                ReDim Args.Layers(A).Terrainmap.ValueData.Value(TerrainSize.Y - 1, TerrainSize.X - 1)
                Args.Layers(A).Terrainmap.ValueData.Size = TerrainSize
                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        Args.Layers(A).Terrainmap.ValueData.Value(Y, X) = True
                    Next
                Next
            Else
                hmB.GenerateNewOfSize(TerrainSize.Y, TerrainSize.X, Args.Layers(A).TerrainmapScale, 1.0#)
                hmC.Rescale(hmB, 0.0#, 1.0#)
                Args.Layers(A).Terrainmap.Convert_Heightmap(hmC, (1.0F - Args.Layers(A).TerrainmapDensity) / hmC.HeightScale)
            End If
        Next

        Dim Pos As sXY_int

        ReDim Terrain(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim Slope(TerrainSize.X - 1, TerrainSize.Y - 1)
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = (X + 0.25#) * TerrainGridSpacing
                Pos.Y = (Y + 0.25#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.75#) * TerrainGridSpacing
                Pos.Y = (Y + 0.25#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.25#) * TerrainGridSpacing
                Pos.Y = (Y + 0.75#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.75#) * TerrainGridSpacing
                Pos.Y = (Y + 0.75#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = BestSlope
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
                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            If TerrainVertex(X, Y).Height < Args.Layers(Layer_Num).HeightMin _
                            Or TerrainVertex(X, Y).Height > Args.Layers(Layer_Num).HeightMax Then
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

                For Y = 0 To TerrainSize.Y - 1
                    For X = 0 To TerrainSize.X - 1
                        If LayerResult(Layer_Num).ValueData.Value(Y, X) Then
                            Terrain(X, Y) = TerrainNum
                        End If
                    Next
                Next
            End If
        Next

        'set water tiles

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                If Args.Watermap.ValueData.Value(Y, X) Then
                    If Slope(X, Y) < CliffSlope Then
                        Terrain(X, Y) = 17
                    End If
                End If
            Next
        Next

        'set border tiles to cliffs
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To 2
                Terrain(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For X = TerrainSize.X - 4 To TerrainSize.X - 1
                Terrain(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next
        For X = 3 To TerrainSize.X - 5
            For Y = 0 To 2
                Terrain(X, Y) = Args.Tileset.BorderTextureNum
            Next
            For Y = TerrainSize.Y - 4 To TerrainSize.Y - 1
                Terrain(X, Y) = Args.Tileset.BorderTextureNum
            Next
        Next

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTiles(X, Y).Texture.TextureNum = Terrain(X, Y)
            Next
        Next
    End Sub

    Public Sub RandomizeTileOrientations()
        Dim X As Integer
        Dim Z As Integer

        For Z = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                TerrainTiles(X, Z).Texture.Orientation = New sTileOrientation(Rnd() >= 0.5F, Rnd() >= 0.5F, Rnd() >= 0.5F)
            Next
        Next
        For Z = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                SectorGraphicsChange.SectorChanged(New sXY_int(X, Z))
            Next
        Next
    End Sub

    Public Sub MapTexturer(ByRef LayerList As frmMapTexturer.sLayerList)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim Terrain(,) As clsPainter.clsTerrain
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

        ReDim Terrain(TerrainSize.X, TerrainSize.Y)
        ReDim Slope(TerrainSize.X - 1, TerrainSize.Y - 1)
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                'get slope
                BestSlope = 0.0#

                Pos.X = (X + 0.25#) * TerrainGridSpacing
                Pos.Y = (Y + 0.25#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.75#) * TerrainGridSpacing
                Pos.Y = (Y + 0.25#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.25#) * TerrainGridSpacing
                Pos.Y = (Y + 0.75#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Pos.X = (X + 0.75#) * TerrainGridSpacing
                Pos.Y = (Y + 0.75#) * TerrainGridSpacing
                CurrentSlope = GetTerrainSlopeAngle(Pos)
                If CurrentSlope > BestSlope Then
                    BestSlope = CurrentSlope
                End If

                Slope(X, Y) = BestSlope
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
                For Y = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            If TerrainVertex(X, Y).Height < LayerList.Layers(LayerNum).HeightMin _
                            Or TerrainVertex(X, Y).Height > LayerList.Layers(LayerNum).HeightMax Then
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
                                    If Y < TerrainSize.Y Then
                                        If Slope(X - 1, Y) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X - 1, Y) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                End If
                                If X < TerrainSize.X Then
                                    If Y > 0 Then
                                        If Slope(X, Y - 1) < LayerList.Layers(LayerNum).SlopeMin _
                                        Or Slope(X, Y - 1) > LayerList.Layers(LayerNum).SlopeMax Then
                                            AllowSlope = False
                                        End If
                                    End If
                                    If Y < TerrainSize.Y Then
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

                For Y = 0 To TerrainSize.Y
                    For X = 0 To TerrainSize.X
                        If LayerResult(LayerNum).ValueData.Value(Y, X) Then
                            Terrain(X, Y) = tmpTerrain
                        End If
                    Next
                Next
            End If
        Next

        'set vertex terrain by terrain map
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                If Terrain(X, Y) IsNot Nothing Then
                    TerrainVertex(X, Y).Terrain = Terrain(X, Y)
                End If
            Next
        Next
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                Tile_AutoTexture_Changed(X, Y, frmMainInstance.cbxInvalidTiles.Checked)
            Next
        Next
    End Sub

    Public Function GenerateTerrainMap(ByVal Scale As Single, ByVal Density As Single) As clsBooleanMap
        Dim hmB As New clsHeightmap
        Dim hmC As New clsHeightmap

        hmB.GenerateNewOfSize(TerrainSize.Y + 1, TerrainSize.X + 1, Scale, 1.0#)
        hmC.Rescale(hmB, 0.0#, 1.0#)
        GenerateTerrainMap = New clsBooleanMap
        GenerateTerrainMap.Convert_Heightmap(hmC, (1.0# - Density) / hmC.HeightScale)
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
        For Y = Math.Max(Start.Y - 1, 0) To Math.Min(Finish.Y + 1, TerrainSize.Y - 1)
            Offset.Y = Y - Start.Y + 1
            For X = Math.Max(Start.X - 1, 0) To Math.Min(Finish.X + 1, TerrainSize.X - 1)
                Offset.X = X - Start.X + 1
                If TerrainTiles(X, Y).Texture.TextureNum >= 0 Then
                    For A = 0 To Painter.TerrainCount - 1
                        With Painter.Terrains(A)
                            For B = 0 To .Tiles.TileCount - 1
                                If .Tiles.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
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
                                If .Tiles_Straight.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Straight.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tiles_Corner_In.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_In.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_Out.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tiles_Straight.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Straight.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                    If TerrainTiles(X, Y).Tri Then
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = True
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = True
                                    Else
                                        TerrainTiles(X, Y).TriTopRightIsCliff = True
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = True
                                    End If
                                    TerrainTiles(X, Y).Terrain_IsCliff = True
                                    TerrainTiles(X, Y).DownSide = Orientation
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_In.TileCount - 1
                                If .Tiles_Corner_In.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_In.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = True
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = True
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = True
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = True
                                    End If
                                    TerrainTiles(X, Y).Terrain_IsCliff = True
                                End If
                            Next
                            For B = 0 To .Tiles_Corner_Out.TileCount - 1
                                If .Tiles_Corner_Out.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    OrientationToDirection(.Tiles_Corner_Out.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
                                    If IdenticalTileOrientations(Orientation, TileDirection_TopLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = True
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_TopRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = True
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomRight) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = True
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = False
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    ElseIf IdenticalTileOrientations(Orientation, TileDirection_BottomLeft) Then
                                        VertexTerrainCount(Offset.X, Offset.Y, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain_Inner.Num) += 1
                                        VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain_Outer.Num) += 1
                                        TerrainTiles(X, Y).TriTopLeftIsCliff = False
                                        TerrainTiles(X, Y).TriBottomRightIsCliff = False
                                        TerrainTiles(X, Y).TriTopRightIsCliff = True
                                        TerrainTiles(X, Y).TriBottomLeftIsCliff = False
                                    End If
                                    TerrainTiles(X, Y).Terrain_IsCliff = True
                                End If
                            Next
                        End With
                    Next
                    For A = 0 To Painter.RoadBrushCount - 1
                        With Painter.RoadBrushes(A)
                            For B = 0 To .Tile_Corner_In.TileCount - 1
                                If .Tile_Corner_In.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_Corner_In.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tile_CrossIntersection.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
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
                                If .Tile_End.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_End.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tile_Straight.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_Straight.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                                If .Tile_TIntersection.Tiles(B).TextureNum = TerrainTiles(X, Y).Texture.TextureNum Then
                                    VertexTerrainCount(Offset.X, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X, Offset.Y + 1, .Terrain.Num) += 1
                                    VertexTerrainCount(Offset.X + 1, Offset.Y + 1, .Terrain.Num) += 1
                                    OrientationToDirection(.Tile_TIntersection.Tiles(B).Direction, TerrainTiles(X, Y).Texture, Orientation)
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
                    TerrainVertex(X, Y).Terrain = Painter.Terrains(Best_Num)
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
                    TerrainSideH(X, Y).Road = Painter.Roads(Best_Num)
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
                    TerrainSideV(X, Y).Road = Painter.Roads(Best_Num)
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

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                If TerrainTiles(X, Y).Tri Then
                    If TerrainTiles(X, Y).Texture.TextureNum >= 0 Then
                        If Tileset.Tiles(TerrainTiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            TerrainTiles(X, Y).Tri = False
                            SectorGraphicsChange.Tile_Set_Changed(New sXY_int(X, Y))
                        End If
                    End If
                End If
            Next
        Next
    End Sub
End Class