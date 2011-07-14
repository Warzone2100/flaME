Partial Public Class clsMap

    Structure sFMEUnit
        Dim Code As String
        Dim ID As UInteger
        Dim SavePriority As Integer
        Dim LNDType As Byte
        Dim X As UInteger
        Dim Y As UInteger
        Dim Z As UInteger
        Dim Rotation As UShort
        Dim Name As String
        Dim Player As Byte
    End Structure

    'Public Structure sFME_Object
    '    Public TypeType As Byte
    '    Public IsDroidParts As Byte
    '    Public PreferPartsOutput As Byte
    '    Public Code As String
    '    Public DroidType As Byte
    '    Public BodyExists As Byte
    '    Public BodyCode As String
    '    Public PropulsionExists As Byte
    '    Public PropulsionCode As String
    '    Public TurretCount As Byte
    '    Public Turret1Exists As Byte
    '    Public Turret1Type As Byte
    '    Public Turret1Code As String
    '    Public Turret2Exists As Byte
    '    Public Turret2Type As Byte
    '    Public Turret2Code As String
    '    Public Turret3Exists As Byte
    '    Public Turret3Type As Byte
    '    Public Turret3Code As String
    '    Public ID As UInteger
    '    Public SavePriority As Integer
    '    Public X As UInteger
    '    Public Y As UInteger
    '    Public Z As UInteger
    '    Public Rotation As UShort
    '    Public Name As String
    '    Public Player As Byte
    '    Public Health As Double
    'End Structure

    Function Load_FME(ByVal Path As String, ByRef ResultInfo As clsInterfaceOptions) As clsResult
        Load_FME = New clsResult

        Dim File As New clsReadFile
        Dim Result As sResult

        Result = File.Begin(Path)
        If Not Result.Success Then
            Load_FME.Problem_Add("Load FME: " & Result.Problem)
            Exit Function
        End If
        Load_FME.Append(Read_FME(File, ResultInfo), "Load FME: ")
        File.Close()
        If Load_FME.HasProblems Then
            Exit Function
        End If

        AfterInitialized
    End Function

    Private Function Read_FME(ByVal File As clsReadFile, ByRef ResultInfo As clsInterfaceOptions) As clsResult
        Read_FME = New clsResult

        ResultInfo = New clsInterfaceOptions

        Dim Version As UInteger

        If Not File.Get_U32(Version) Then Read_FME.Problem_Add("Read error.") : Exit Function

        If Version = 1UI Then
            Read_FME.Problem_Add("Version 1 is not supported.")
            Exit Function
        ElseIf Version = 2UI Then
            Read_FME.Problem_Add("Version 2 is not supported.")
            Exit Function
        ElseIf Version = 3UI Or Version = 4UI Then

            Dim byteTemp As Byte

            'tileset
            If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If byteTemp = 0 Then
                Tileset = Nothing
            ElseIf byteTemp = 1 Then
                Tileset = Tileset_Arizona
            ElseIf byteTemp = 2 Then
                Tileset = Tileset_Urban
            ElseIf byteTemp = 3 Then
                Tileset = Tileset_Rockies
            Else
                Read_FME.Warning_Add("Tileset value was out of range.")
                Tileset = Nothing
            End If

            SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.

            Dim MapWidth As UShort
            Dim MapHeight As UShort

            If Not File.Get_U16(MapWidth) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U16(MapHeight) Then Read_FME.Problem_Add("Read error.") : Exit Function

            If MapWidth < 1US Or MapHeight < 1US Or MapWidth > MapMaxSize Or MapHeight > MapMaxSize Then
                Read_FME.Problem_Add("Map size is invalid.")
                Exit Function
            End If

            Terrain_Blank(MapWidth, MapHeight)
            TileType_Reset()

            Dim X As Integer
            Dim Y As Integer
            Dim A As Integer
            Dim B As Integer
            Dim intTemp As Integer
            Dim Rotation As Byte
            Dim FlipX As Boolean
            Dim FlipZ As Boolean
            Dim MakeWarning As Boolean

            MakeWarning = False
            For Y = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X
                    If Not File.Get_U8(TerrainVertex(X, Y).Height) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainVertex(X, Y).Terrain = Nothing
                    ElseIf intTemp >= Painter.TerrainCount Then
                        MakeWarning = True
                        TerrainVertex(X, Y).Terrain = Nothing
                    Else
                        TerrainVertex(X, Y).Terrain = Painter.Terrains(intTemp)
                    End If
                Next
            Next
            If MakeWarning Then
                Read_FME.Warning_Add("A painted ground type value was out of range.")
            End If
            MakeWarning = False
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    TerrainTiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1

                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function

                    intTemp = 128
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Terrain_IsCliff = (A = 1)

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

                    OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, TerrainTiles(X, Y).Texture.Orientation)

                    intTemp = 4
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Tri = (A = 1)

                    intTemp = 2
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    If TerrainTiles(X, Y).Tri Then
                        TerrainTiles(X, Y).TriTopLeftIsCliff = (A = 1)
                    Else
                        TerrainTiles(X, Y).TriBottomLeftIsCliff = (A = 1)
                    End If

                    intTemp = 1
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    If TerrainTiles(X, Y).Tri Then
                        TerrainTiles(X, Y).TriBottomRightIsCliff = (A = 1)
                    Else
                        TerrainTiles(X, Y).TriTopRightIsCliff = (A = 1)
                    End If

                    'attributes2
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function

                    'ignore large values - nothing should be stored there
                    intTemp = 16
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp

                    intTemp = 1
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    Select Case A
                        Case 1
                            TerrainTiles(X, Y).DownSide = TileDirection_Top
                        Case 3
                            TerrainTiles(X, Y).DownSide = TileDirection_Right
                        Case 5
                            TerrainTiles(X, Y).DownSide = TileDirection_Bottom
                        Case 7
                            TerrainTiles(X, Y).DownSide = TileDirection_Left
                        Case 8
                            TerrainTiles(X, Y).DownSide = TileDirection_None
                        Case Else
                            TerrainTiles(X, Y).DownSide = TileDirection_None
                            MakeWarning = True
                    End Select
                Next
            Next
            If MakeWarning Then
                Read_FME.Warning_Add("A tile cliff down-side was out of range.")
            End If
            MakeWarning = False
            For Y = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X - 1
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainSideH(X, Y).Road = Nothing
                    ElseIf intTemp >= Painter.RoadCount Then
                        MakeWarning = True
                        TerrainSideH(X, Y).Road = Nothing
                    Else
                        TerrainSideH(X, Y).Road = Painter.Roads(intTemp)
                    End If
                Next
            Next
            If MakeWarning Then
                Read_FME.Warning_Add("A horizontal road value was out of range.")
            End If
            MakeWarning = False
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainSideV(X, Y).Road = Nothing
                    ElseIf intTemp >= Painter.RoadCount Then
                        MakeWarning = True
                        TerrainSideV(X, Y).Road = Nothing
                    Else
                        TerrainSideV(X, Y).Road = Painter.Roads(intTemp)
                    End If
                Next
            Next
            If MakeWarning Then
                Read_FME.Warning_Add("A vertical road value was out of range.")
            End If
            Dim TempUnitCount As UInteger
            File.Get_U32(TempUnitCount)

            Dim TempUnit(TempUnitCount - 1) As sFMEUnit
            For A = 0 To TempUnitCount - 1
                If Not File.Get_Text(40, TempUnit(A).Code) Then Read_FME.Problem_Add("Read error.") : Exit Function
                B = Strings.InStr(TempUnit(A).Code, Chr(0))
                If B > 0 Then
                    TempUnit(A).Code = Strings.Left(TempUnit(A).Code, B - 1)
                End If
                If Not File.Get_U8(TempUnit(A).LNDType) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).ID) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).X) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).Z) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).Y) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(TempUnit(A).Rotation) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_Text_VariableLength(TempUnit(A).Name) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U8(TempUnit(A).Player) Then Read_FME.Problem_Add("Read error.") : Exit Function
            Next

            Dim NewUnit As clsUnit
            Dim tmpUnitType As clsUnitType = Nothing
            Dim WarningCount As Integer
            WarningCount = 0
            For A = 0 To TempUnitCount - 1
                Select Case TempUnit(A).LNDType
                    Case 0
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.Feature)
                    Case 1
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.PlayerStructure)
                    Case 2
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.PlayerDroid)
                    Case Else
                        tmpUnitType = Nothing
                End Select
                If tmpUnitType IsNot Nothing Then
                    NewUnit = New clsUnit
                    NewUnit.Type = tmpUnitType
                    NewUnit.ID = TempUnit(A).ID
                    'NewUnit.Name = TempUnit(A).Name
                    If TempUnit(A).Player >= PlayerCountMax Then
                        NewUnit.UnitGroup = ScavengerUnitGroup
                    Else
                        NewUnit.UnitGroup = UnitGroups(TempUnit(A).Player)
                    End If
                    NewUnit.Pos.Horizontal.X = TempUnit(A).X
                    NewUnit.Pos.Altitude = TempUnit(A).Y
                    NewUnit.Pos.Horizontal.Y = TempUnit(A).Z
                    NewUnit.Rotation = Math.Min(TempUnit(A).Rotation, 359)
                    If TempUnit(A).ID = 0UI Then
                        TempUnit(A).ID = ZeroResetID
                        ZeroIDWarning(NewUnit)
                    End If
                    Unit_Add(NewUnit, TempUnit(A).ID)
                    ErrorIDChange(TempUnit(A).ID, NewUnit, "Read_FMEv3+")
                Else
                    WarningCount += 1
                End If
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " types of a unit were out of range. That many units were ignored.")
            End If

            File.Get_U32(GatewayCount)
            ReDim Gateways(GatewayCount - 1)
            For A = 0 To GatewayCount - 1
                If Not File.Get_U16(Gateways(A).PosA.X) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(Gateways(A).PosA.Y) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(Gateways(A).PosB.X) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(Gateways(A).PosB.Y) Then Read_FME.Problem_Add("Read error.") : Exit Function
            Next

            If Version = 4UI And Tileset IsNot Nothing Then
                For A = 0 To 89
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    If A < Tileset.TileCount Then
                        Tile_TypeNum(A) = byteTemp
                    End If
                Next
            End If

            If Not File.IsEOF Then
                Read_FME.Warning_Add("There were unread bytes at the end of the file.")
            End If
        ElseIf Version = 5UI Or Version = 6UI Or Version = 7UI Then

            Dim byteTemp As Byte

            'tileset
            If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If byteTemp = 0 Then
                Tileset = Nothing
            ElseIf byteTemp = 1 Then
                Tileset = Tileset_Arizona
            ElseIf byteTemp = 2 Then
                Tileset = Tileset_Urban
            ElseIf byteTemp = 3 Then
                Tileset = Tileset_Rockies
            Else
                Read_FME.Warning_Add("Tileset value out of range.")
                Tileset = Nothing
            End If

            SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.

            Dim MapWidth As UShort
            Dim MapHeight As UShort

            If Not File.Get_U16(MapWidth) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U16(MapHeight) Then Read_FME.Problem_Add("Read error.") : Exit Function

            If MapWidth < 1US Or MapHeight < 1US Or MapWidth > MapMaxSize Or MapHeight > MapMaxSize Then
                Read_FME.Problem_Add("Map size is invalid.")
                Exit Function
            End If

            Terrain_Blank(MapWidth, MapHeight)
            TileType_Reset()

            Dim X As Integer
            Dim Y As Integer
            Dim A As Integer
            Dim B As Integer
            Dim intTemp As Integer
            Dim WarningCount As Integer

            WarningCount = 0
            For Y = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X
                    If Not File.Get_U8(TerrainVertex(X, Y).Height) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainVertex(X, Y).Terrain = Nothing
                    ElseIf intTemp >= Painter.TerrainCount Then
                        WarningCount += 1
                        TerrainVertex(X, Y).Terrain = Nothing
                    Else
                        TerrainVertex(X, Y).Terrain = Painter.Terrains(intTemp)
                    End If
                Next
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " painted ground vertices were out of range.")
            End If
            WarningCount = 0
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    TerrainTiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1

                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function

                    intTemp = 128
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Terrain_IsCliff = (A = 1)

                    intTemp = 64
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Texture.Orientation.SwitchedAxes = (A = 1)

                    intTemp = 32
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Texture.Orientation.ResultXFlip = (A = 1)

                    intTemp = 16
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Texture.Orientation.ResultZFlip = (A = 1)

                    intTemp = 4
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    TerrainTiles(X, Y).Tri = (A = 1)

                    intTemp = 2
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    If TerrainTiles(X, Y).Tri Then
                        TerrainTiles(X, Y).TriTopLeftIsCliff = (A = 1)
                    Else
                        TerrainTiles(X, Y).TriBottomLeftIsCliff = (A = 1)
                    End If

                    intTemp = 1
                    A = CInt(Int(byteTemp / intTemp))
                    byteTemp -= A * intTemp
                    If TerrainTiles(X, Y).Tri Then
                        TerrainTiles(X, Y).TriBottomRightIsCliff = (A = 1)
                    Else
                        TerrainTiles(X, Y).TriTopRightIsCliff = (A = 1)
                    End If

                    'attributes2
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function

                    Select Case byteTemp
                        Case 0
                            TerrainTiles(X, Y).DownSide = TileDirection_None
                        Case 1
                            TerrainTiles(X, Y).DownSide = TileDirection_Top
                        Case 2
                            TerrainTiles(X, Y).DownSide = TileDirection_Left
                        Case 3
                            TerrainTiles(X, Y).DownSide = TileDirection_Right
                        Case 4
                            TerrainTiles(X, Y).DownSide = TileDirection_Bottom
                        Case Else
                            WarningCount += 1
                    End Select
                Next
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " tile cliff down-sides were out of range.")
            End If
            WarningCount = 0
            For Y = 0 To TerrainSize.Y
                For X = 0 To TerrainSize.X - 1
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainSideH(X, Y).Road = Nothing
                    ElseIf intTemp >= Painter.RoadCount Then
                        WarningCount += 1
                        TerrainSideH(X, Y).Road = Nothing
                    Else
                        TerrainSideH(X, Y).Road = Painter.Roads(intTemp)
                    End If
                Next
            Next
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    intTemp = CInt(byteTemp) - 1
                    If intTemp < 0 Then
                        TerrainSideV(X, Y).Road = Nothing
                    ElseIf intTemp >= Painter.RoadCount Then
                        WarningCount += 1
                        TerrainSideV(X, Y).Road = Nothing
                    Else
                        TerrainSideV(X, Y).Road = Painter.Roads(intTemp)
                    End If
                Next
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " roads were out of range.")
            End If
            Dim TempUnitCount As UInteger
            File.Get_U32(TempUnitCount)
            Dim TempUnit(TempUnitCount - 1) As sFMEUnit
            For A = 0 To TempUnitCount - 1
                If Not File.Get_Text(40, TempUnit(A).Code) Then Read_FME.Problem_Add("Read error.") : Exit Function
                B = Strings.InStr(TempUnit(A).Code, Chr(0))
                If B > 0 Then
                    TempUnit(A).Code = Strings.Left(TempUnit(A).Code, B - 1)
                End If
                If Not File.Get_U8(TempUnit(A).LNDType) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).ID) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Version = 6UI Then
                    If Not File.Get_S32(TempUnit(A).SavePriority) Then Read_FME.Problem_Add("Read error.") : Exit Function
                End If
                If Not File.Get_U32(TempUnit(A).X) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).Z) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U32(TempUnit(A).Y) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(TempUnit(A).Rotation) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_Text_VariableLength(TempUnit(A).Name) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U8(TempUnit(A).Player) Then Read_FME.Problem_Add("Read error.") : Exit Function
            Next

            Dim NewUnit As clsUnit
            Dim tmpUnitType As clsUnitType = Nothing
            WarningCount = 0
            For A = 0 To TempUnitCount - 1
                Select Case TempUnit(A).LNDType
                    Case 0
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.Feature)
                    Case 1
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.PlayerStructure)
                    Case 2
                        tmpUnitType = FindOrCreateUnitType(TempUnit(A).Code, clsUnitType.enumType.PlayerDroid)
                    Case Else
                        tmpUnitType = Nothing
                End Select
                If tmpUnitType IsNot Nothing Then
                    NewUnit = New clsUnit
                    NewUnit.Type = tmpUnitType
                    NewUnit.ID = TempUnit(A).ID
                    NewUnit.SavePriority = TempUnit(A).SavePriority
                    'NewUnit.Name = TempUnit(A).Name
                    If TempUnit(A).Player >= PlayerCountMax Then
                        NewUnit.UnitGroup = ScavengerUnitGroup
                    Else
                        NewUnit.UnitGroup = UnitGroups(TempUnit(A).Player)
                    End If
                    NewUnit.Pos.Horizontal.X = TempUnit(A).X
                    NewUnit.Pos.Altitude = TempUnit(A).Y
                    NewUnit.Pos.Horizontal.Y = TempUnit(A).Z
                    NewUnit.Rotation = Math.Min(CInt(TempUnit(A).Rotation), 359)
                    If TempUnit(A).ID = 0UI Then
                        TempUnit(A).ID = ZeroResetID
                        ZeroIDWarning(NewUnit)
                    End If
                    Unit_Add(NewUnit, TempUnit(A).ID)
                    ErrorIDChange(TempUnit(A).ID, NewUnit, "Read_FMEv5+")
                Else
                    WarningCount += 1
                End If
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " types of units were invalid. That many units were ignored.")
            End If

            Dim NewGatewayCount As UInteger
            Dim NewGateStartX As UShort
            Dim NewGateStartY As UShort
            Dim NewGateFinishX As UShort
            Dim NewGateFinishY As UShort
            Dim NewGateStart As sXY_int
            Dim NewGateFinish As sXY_int

            File.Get_U32(NewGatewayCount)
            WarningCount = 0
            For A = 0 To CInt(NewGatewayCount) - 1
                If Not File.Get_U16(NewGateStartX) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(NewGateStartY) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(NewGateFinishX) Then Read_FME.Problem_Add("Read error.") : Exit Function
                If Not File.Get_U16(NewGateFinishY) Then Read_FME.Problem_Add("Read error.") : Exit Function
                NewGateStart.X = NewGateStartX
                NewGateStart.Y = NewGateStartY
                NewGateFinish.X = NewGateFinishX
                NewGateFinish.Y = NewGateFinishY
                If Not Gateway_Add(NewGateStart, NewGateFinish) Then
                    WarningCount += 1
                End If
            Next
            If WarningCount > 0 Then
                Read_FME.Warning_Add(WarningCount & " gateways were invalid.")
            End If

            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
                    Tile_TypeNum(A) = byteTemp
                Next
            End If

            'scroll limits
            If Not File.Get_S32(ResultInfo.ScrollMin.X) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_S32(ResultInfo.ScrollMin.Y) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U32(ResultInfo.ScrollMax.X) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U32(ResultInfo.ScrollMax.Y) Then Read_FME.Problem_Add("Read error.") : Exit Function

            'other compile info

            Dim strTemp As String = Nothing

            If Not File.Get_Text_VariableLength(ResultInfo.CompileName) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
            Select Case byteTemp
                Case 0
                    'no compile type
                Case 1
                    'compile multi
                Case 2
                    'compile campaign
                Case Else
                    'error
            End Select
            If Not File.Get_Text_VariableLength(ResultInfo.CompileMultiPlayers) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_U8(byteTemp) Then Read_FME.Problem_Add("Read error.") : Exit Function
            Select Case byteTemp
                Case 0
                    ResultInfo.CompileMultiXPlayers = False
                Case 1
                    ResultInfo.CompileMultiXPlayers = True
                Case Else
                    Read_FME.Warning_Add("Compile player format out of range.")
            End Select
            If Not File.Get_Text_VariableLength(ResultInfo.CompileMultiAuthor) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_Text_VariableLength(ResultInfo.CompileMultiLicense) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_Text_VariableLength(ResultInfo.CampaignGameTime) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If Not File.Get_S32(ResultInfo.CampaignGameType) Then Read_FME.Problem_Add("Read error.") : Exit Function
            If ResultInfo.CampaignGameType < -1 Or ResultInfo.CampaignGameType >= frmCompileInstance.cboCampType.Items.Count Then
                Read_FME.Warning_Add("Compile campaign type out of range.")
                ResultInfo.CampaignGameType = -1
            End If

            If Not File.IsEOF Then
                Read_FME.Warning_Add("There were unread bytes at the end of the file.")
            End If
        Else
            Read_FME.Problem_Add("File version number not recognised.")
        End If
    End Function

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
            Dim Y As Integer
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
                    Line_Num += 1
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
                    Line_Num += 1
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

            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Tile_Num = Y * TerrainSize.X + X
                    'lnd uses different order! (3 = 2, 2 = 3), this program goes left to right, lnd goes clockwise around each tile
                    TerrainVertex(X, Y).Height = LNDTile(Tile_Num).Vertex0Height
                Next X
            Next Y

            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    Tile_Num = Y * TerrainSize.X + X

                    TerrainTiles(X, Y).Texture.TextureNum = LNDTile(Tile_Num).TID - 1

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
                    TerrainTiles(X, Y).Tri = (A = 1)

                    'vf, tf, ignore

                    OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, TerrainTiles(X, Y).Texture.Orientation)
                Next
            Next

            Dim NewUnit As clsUnit
            Dim XYZ_int As sXYZ_int
            Dim NewType As clsUnitType

            For A = 0 To ObjectCount - 1
                Select Case LNDObject(A).TypeNum
                    Case 0
                        NewType = FindOrCreateUnitType(LNDObject(A).Code, clsUnitType.enumType.Feature)
                    Case 1
                        NewType = FindOrCreateUnitType(LNDObject(A).Code, clsUnitType.enumType.PlayerStructure)
                    Case 2
                        NewType = FindOrCreateUnitType(LNDObject(A).Code, clsUnitType.enumType.PlayerDroid)
                    Case Else
                        NewType = Nothing
                End Select
                If NewType IsNot Nothing Then
                    NewUnit = New clsUnit
                    NewUnit.Type = NewType
                    NewUnit.ID = LNDObject(A).ID
                    'NewUnit.Name = LNDObject(A).Name
                    If LNDObject(A).PlayerNum < 0 Or LNDObject(A).PlayerNum >= PlayerCountMax Then
                        NewUnit.UnitGroup = ScavengerUnitGroup
                    Else
                        NewUnit.UnitGroup = UnitGroups(LNDObject(A).PlayerNum)
                    End If
                    XYZ_int.X = LNDObject(A).Pos.X
                    XYZ_int.Y = LNDObject(A).Pos.Y
                    XYZ_int.Z = LNDObject(A).Pos.Z
                    NewUnit.Pos = MapPos_From_LNDPos(XYZ_int)
                    NewUnit.Rotation = LNDObject(A).Rotation.Y
                    Unit_Add(NewUnit, LNDObject(A).ID)
                    'todo: warn if id is changed
                End If
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

            AfterInitialized

        Catch ex As Exception
            Load_LND.Problem = ex.Message
            Exit Function
        End Try

        Load_LND.Success = True
    End Function

    Function LNDPos_From_MapPos(ByVal Horizontal As sXY_int) As sXYZ_int
        Dim Result As sXYZ_int

        Result.X = Horizontal.X - TerrainSize.X * TerrainGridSpacing / 2.0#
        Result.Z = TerrainSize.Y * TerrainGridSpacing / 2.0# - Horizontal.Y
        Result.Y = GetTerrainHeight(Horizontal)

        Return Result
    End Function

    Function MapPos_From_LNDPos(ByVal Pos As sXYZ_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = Pos.X + TerrainSize.X * TerrainGridSpacing / 2.0#
        Result.Horizontal.Y = TerrainSize.Y * TerrainGridSpacing / 2.0# - Pos.Z
        Result.Altitude = GetTerrainHeight(Result.Horizontal)

        Return Result
    End Function

    Function Write_LND(ByVal Path As String, ByVal Overwrite As Boolean) As clsResult
        Dim ReturnResult As New clsResult

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                ReturnResult.Problem_Add("The selected file already exists.")
                Return ReturnResult
            End If
        End If

        Try

            Dim Text As String
            Dim EndChar As Char
            Dim Quote As Char
            Dim A As Integer
            Dim X As Integer
            Dim Y As Integer
            Dim Flip As Byte
            Dim B As Integer
            Dim VF As Integer
            Dim TF As Integer
            Dim C As Integer
            Dim Rotation As Byte
            Dim FlipX As Boolean

            Quote = ControlChars.Quote
            EndChar = Chr(10)

            Dim ByteFile As New clsWriteFile

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
            For Y = 0 To TerrainSize.Y - 1
                For X = 0 To TerrainSize.X - 1
                    TileOrientation_To_OldOrientation(TerrainTiles(X, Y).Texture.Orientation, Rotation, FlipX)
                    Flip = 0
                    If TerrainTiles(X, Y).Tri Then
                        Flip += 2
                    End If
                    If FlipX Then
                        Flip += 4
                    End If
                    'If TerrainTile(X, Z).Texture.FlipZ Then
                    '    Flip += 8
                    'End If
                    Flip += Rotation * 16

                    If TerrainTiles(X, Y).Tri Then
                        VF = 1
                    Else
                        VF = 0
                    End If
                    If FlipX Then
                        TF = 1
                    Else
                        TF = 0
                    End If

                    Text = "        TID " & TerrainTiles(X, Y).Texture.TextureNum + 1 & " VF " & VF & " TF " & TF & " F " & Flip & " VH " & TerrainVertex(X, Y).Height & " " & TerrainVertex(X + 1, Y).Height & " " & TerrainVertex(X + 1, Y + 1).Height & " " & TerrainVertex(X, Y + 1).Height & EndChar
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
            Dim strTemp As String = Nothing
            Dim CustomDroidCount As Integer = 0
            For A = 0 To UnitCount - 1
                Select Case Units(A).Type.Type
                    Case clsUnitType.enumType.Feature
                        B = 0
                    Case clsUnitType.enumType.PlayerStructure
                        B = 1
                    Case clsUnitType.enumType.PlayerDroid
                        If CType(Units(A).Type, clsDroidDesign).IsTemplate Then
                            B = 2
                        Else
                            B = -1
                        End If
                    Case Else
                        B = -1
                        ReturnResult.Warning_Add("Unit type classification not accounted for.")
                End Select
                XYZ_int = LNDPos_From_MapPos(Units(A).Pos.Horizontal)
                If B >= 0 Then
                    If Units(A).Type.GetCode(strTemp) Then
                        Text = "        " & Units(A).ID & " " & B & " " & Quote & strTemp & Quote & " " & Units(A).UnitGroup.GetLNDPlayerText & " " & Quote & "NONAME" & Quote & " " & Strings.FormatNumber(XYZ_int.X, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(XYZ_int.Y, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(XYZ_int.Z, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(0, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(Units(A).Rotation, 2, TriState.True, TriState.False, TriState.False) & " " & Strings.FormatNumber(0, 2, TriState.True, TriState.False, TriState.False) & EndChar
                        ByteFile.Text_Append(Text)
                    Else
                        ReturnResult.Warning_Add("Error. Code not found.")
                    End If
                Else
                    CustomDroidCount += 1
                End If
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
                        Text = Text & Tile_TypeNum(C + B) & " "
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
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Function Write_MinimapFile(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim X As Integer
        Dim Y As Integer

        Dim MinimapBitmap As New Bitmap(TerrainSize.X, TerrainSize.Y)

        Dim Texture(TerrainSize.Y - 1, TerrainSize.X - 1, 3) As Byte

        MinimapTextureFill(Texture)

        For Y = 0 To Texture.GetUpperBound(0)
            For X = 0 To Texture.GetUpperBound(1)
                MinimapBitmap.SetPixel(X, Y, Drawing.ColorTranslator.FromOle(OSRGB(Texture(Y, X, 0), Texture(Y, X, 1), Texture(Y, X, 2))))
            Next
        Next

        Write_MinimapFile = SaveBitmap(Path, Overwrite, MinimapBitmap)
    End Function

    Function Write_Heightmap(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim HeightmapBitmap As New Bitmap(TerrainSize.X + 1, TerrainSize.Y + 1)
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                HeightmapBitmap.SetPixel(X, Y, Drawing.ColorTranslator.FromOle(OSRGB(TerrainVertex(X, Y).Height, TerrainVertex(X, Y).Height, TerrainVertex(X, Y).Height)))
            Next
        Next

        Write_Heightmap = SaveBitmap(Path, Overwrite, HeightmapBitmap)
    End Function

    Function Write_TTP(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Write_TTP.Success = False
        Write_TTP.Problem = ""

        Dim File_TTP As New clsWriteFile
        Dim A As Integer

        File_TTP.Text_Append("ttyp")
        File_TTP.U32_Append(8UI)
        If Tileset Is Nothing Then
            File_TTP.U32_Append(0UI)
        Else
            File_TTP.U32_Append(Tileset.TileCount)
            For A = 0 To Tileset.TileCount - 1
                File_TTP.U16_Append(Tile_TypeNum(A))
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
        Dim File As New clsReadFile

        Load_TTP = File.Begin(Path)
        If Not Load_TTP.Success Then
            Exit Function
        End If
        Load_TTP = Read_TTP(File)
        File.Close()
    End Function

    Function Write_FME(ByVal Path As String, ByVal Overwrite As Boolean, ByVal ScavengerPlayerNum As Byte) As sResult
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
            Dim ByteFile As New clsWriteFile

            ByteFile.U32_Append(6UI)

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
                    ByteFile.U8_Append(TerrainTiles(X, Z).Texture.TextureNum + 1)

                    TileAttributes = 0
                    If TerrainTiles(X, Z).Terrain_IsCliff Then
                        TileAttributes += 128
                    End If
                    If TerrainTiles(X, Z).Texture.Orientation.SwitchedAxes Then
                        TileAttributes += 64
                    End If
                    If TerrainTiles(X, Z).Texture.Orientation.ResultXFlip Then
                        TileAttributes += 32
                    End If
                    If TerrainTiles(X, Z).Texture.Orientation.ResultZFlip Then
                        TileAttributes += 16
                    End If
                    '8 is free
                    If TerrainTiles(X, Z).Tri Then
                        TileAttributes += 4
                        If TerrainTiles(X, Z).TriTopLeftIsCliff Then
                            TileAttributes += 2
                        End If
                        If TerrainTiles(X, Z).TriBottomRightIsCliff Then
                            TileAttributes += 1
                        End If
                    Else
                        If TerrainTiles(X, Z).TriBottomLeftIsCliff Then
                            TileAttributes += 2
                        End If
                        If TerrainTiles(X, Z).TriTopRightIsCliff Then
                            TileAttributes += 1
                        End If
                    End If
                    ByteFile.U8_Append(TileAttributes)
                    If IdenticalTileOrientations(TerrainTiles(X, Z).DownSide, TileDirection_Top) Then
                        DownSideData = 1
                    ElseIf IdenticalTileOrientations(TerrainTiles(X, Z).DownSide, TileDirection_Left) Then
                        DownSideData = 2
                    ElseIf IdenticalTileOrientations(TerrainTiles(X, Z).DownSide, TileDirection_Right) Then
                        DownSideData = 3
                    ElseIf IdenticalTileOrientations(TerrainTiles(X, Z).DownSide, TileDirection_Bottom) Then
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

            Dim OutputUnits(UnitCount - 1) As clsUnit
            Dim OutputUnitCode(UnitCount - 1) As String
            Dim OutputUnitCount As Integer = 0
            Dim tmpObject As clsUnit
            Dim A As Integer

            For A = 0 To UnitCount - 1
                tmpObject = Units(A)
                If tmpObject.Type.GetCode(OutputUnitCode(OutputUnitCount)) Then
                    OutputUnits(OutputUnitCount) = tmpObject
                    OutputUnitCount += 1
                End If
            Next

            ByteFile.U32_Append(OutputUnitCount)

            For A = 0 To OutputUnitCount - 1
                tmpObject = OutputUnits(A)
                ByteFile.Text_Append(OutputUnitCode(A), 40)
                Select Case tmpObject.Type.Type
                    Case clsUnitType.enumType.Feature
                        ByteFile.U8_Append(0)
                    Case clsUnitType.enumType.PlayerStructure
                        ByteFile.U8_Append(1)
                    Case clsUnitType.enumType.PlayerDroid
                        ByteFile.U8_Append(2)
                End Select
                ByteFile.U32_Append(tmpObject.ID)
                ByteFile.S32_Append(tmpObject.SavePriority)
                ByteFile.U32_Append(tmpObject.Pos.Horizontal.X)
                ByteFile.U32_Append(tmpObject.Pos.Horizontal.Y)
                ByteFile.U32_Append(tmpObject.Pos.Altitude)
                ByteFile.U16_Append(tmpObject.Rotation)
                ByteFile.Text_Append("", True)
                If tmpObject.UnitGroup Is ScavengerUnitGroup Then
                    ByteFile.U8_Append(ScavengerPlayerNum)
                Else
                    ByteFile.U8_Append(tmpObject.UnitGroup.Map_UnitGroupNum)
                End If
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
                    ByteFile.U8_Append(Tile_TypeNum(A))
                Next
            End If

            'scroll limits
            ByteFile.S32_Append(CInt(Clamp(Val(frmCompileInstance.txtScrollMinX.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))))
            ByteFile.S32_Append(CInt(Clamp(Val(frmCompileInstance.txtScrollMinY.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))))
            ByteFile.U32_Append(CUInt(Clamp(Val(frmCompileInstance.txtScrollMaxX.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))))
            ByteFile.U32_Append(CUInt(Clamp(Val(frmCompileInstance.txtScrollMaxY.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))))

            'other compile info
            ByteFile.Text_Append(frmCompileInstance.txtName.Text, True)
            ByteFile.U8_Append(0) 'multiplayer/campaign. 0 = neither
            ByteFile.Text_Append(frmCompileInstance.txtMultiPlayers.Text, True)
            If frmCompileInstance.cbxNewPlayerFormat.Checked Then
                ByteFile.U8_Append(1)
            Else
                ByteFile.U8_Append(0)
            End If
            ByteFile.Text_Append(frmCompileInstance.txtAuthor.Text, True)
            ByteFile.Text_Append(frmCompileInstance.cboLicense.Text, True)
            ByteFile.Text_Append(frmCompileInstance.txtCampTime.Text, True)
            Dim intTemp As Integer = frmCompileInstance.cboCampType.SelectedIndex
            ByteFile.S32_Append(intTemp)

            ByteFile.WriteFile(Path, Overwrite)

        Catch ex As Exception
            Write_FME.Problem = ex.Message
            Exit Function
        End Try

        Write_FME.Success = True
    End Function
End Class