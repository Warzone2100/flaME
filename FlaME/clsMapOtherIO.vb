Partial Public Class clsMap

    Public Structure sFMEUnit
        Public Code As String
        Public ID As UInteger
        Public SavePriority As Integer
        Public LNDType As Byte
        Public X As UInteger
        Public Y As UInteger
        Public Z As UInteger
        Public Rotation As UShort
        Public Name As String
        Public Player As Byte
    End Structure

    Public Function Load_FME(ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult

        Dim File As IO.BinaryReader

        Try
            File = New IO.BinaryReader(New IO.FileStream(Path, IO.FileMode.Open))
        Catch ex As Exception
            ReturnResult.Problem_Add("Open FME: " & ex.Message)
            Return ReturnResult
        End Try
        ReturnResult.Append(Read_FME(File), "Load FME: ")
        File.Close()

        Return ReturnResult
    End Function

    Private Function Read_FME(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim Version As UInteger

        Dim ResultInfo As New clsInterfaceOptions

        Dim UnitAdd As New clsMap.clsUnitAdd
        UnitAdd.Map = Me

        Try

            Version = File.ReadUInt32

            If Version <= 4UI Then
                ReturnResult.Problem_Add("Version " & Version & " is not supported.")
                Return ReturnResult
            ElseIf Version = 5UI Or Version = 6UI Or Version = 7UI Then

                Dim byteTemp As Byte

                'tileset
                byteTemp = File.ReadByte
                If byteTemp = 0 Then
                    Tileset = Nothing
                ElseIf byteTemp = 1 Then
                    Tileset = Tileset_Arizona
                ElseIf byteTemp = 2 Then
                    Tileset = Tileset_Urban
                ElseIf byteTemp = 3 Then
                    Tileset = Tileset_Rockies
                Else
                    ReturnResult.Warning_Add("Tileset value out of range.")
                    Tileset = Nothing
                End If

                SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.

                Dim MapWidth As UShort
                Dim MapHeight As UShort

                MapWidth = File.ReadUInt16
                MapHeight = File.ReadUInt16

                If MapWidth < 1US Or MapHeight < 1US Or MapWidth > MapMaxSize Or MapHeight > MapMaxSize Then
                    ReturnResult.Problem_Add("Map size is invalid.")
                    Return ReturnResult
                End If

                Terrain_Blank(New sXY_int(MapWidth, MapHeight))
                TileType_Reset()

                Dim X As Integer
                Dim Y As Integer
                Dim A As Integer
                Dim B As Integer
                Dim intTemp As Integer
                Dim WarningCount As Integer

                WarningCount = 0
                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X
                        Terrain.Vertices(X, Y).Height = File.ReadByte
                        byteTemp = File.ReadByte
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            Terrain.Vertices(X, Y).Terrain = Nothing
                        ElseIf intTemp >= Painter.TerrainCount Then
                            WarningCount += 1
                            Terrain.Vertices(X, Y).Terrain = Nothing
                        Else
                            Terrain.Vertices(X, Y).Terrain = Painter.Terrains(intTemp)
                        End If
                    Next
                Next
                If WarningCount > 0 Then
                    ReturnResult.Warning_Add(WarningCount & " painted ground vertices were out of range.")
                End If
                WarningCount = 0
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        byteTemp = File.ReadByte
                        Terrain.Tiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1

                        byteTemp = File.ReadByte

                        intTemp = 128
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        Terrain.Tiles(X, Y).Terrain_IsCliff = (A = 1)

                        intTemp = 64
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        Terrain.Tiles(X, Y).Texture.Orientation.SwitchedAxes = (A = 1)

                        intTemp = 32
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        Terrain.Tiles(X, Y).Texture.Orientation.ResultXFlip = (A = 1)

                        intTemp = 16
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        Terrain.Tiles(X, Y).Texture.Orientation.ResultYFlip = (A = 1)

                        intTemp = 4
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        Terrain.Tiles(X, Y).Tri = (A = 1)

                        intTemp = 2
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        If Terrain.Tiles(X, Y).Tri Then
                            Terrain.Tiles(X, Y).TriTopLeftIsCliff = (A = 1)
                        Else
                            Terrain.Tiles(X, Y).TriBottomLeftIsCliff = (A = 1)
                        End If

                        intTemp = 1
                        A = CInt(Int(byteTemp / intTemp))
                        byteTemp -= CByte(A * intTemp)
                        If Terrain.Tiles(X, Y).Tri Then
                            Terrain.Tiles(X, Y).TriBottomRightIsCliff = (A = 1)
                        Else
                            Terrain.Tiles(X, Y).TriTopRightIsCliff = (A = 1)
                        End If

                        'attributes2
                        byteTemp = File.ReadByte

                        Select Case byteTemp
                            Case 0
                                Terrain.Tiles(X, Y).DownSide = TileDirection_None
                            Case 1
                                Terrain.Tiles(X, Y).DownSide = TileDirection_Top
                            Case 2
                                Terrain.Tiles(X, Y).DownSide = TileDirection_Left
                            Case 3
                                Terrain.Tiles(X, Y).DownSide = TileDirection_Right
                            Case 4
                                Terrain.Tiles(X, Y).DownSide = TileDirection_Bottom
                            Case Else
                                WarningCount += 1
                        End Select
                    Next
                Next
                If WarningCount > 0 Then
                    ReturnResult.Warning_Add(WarningCount & " tile cliff down-sides were out of range.")
                End If
                WarningCount = 0
                For Y = 0 To Terrain.TileSize.Y
                    For X = 0 To Terrain.TileSize.X - 1
                        byteTemp = File.ReadByte
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            Terrain.SideH(X, Y).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            WarningCount += 1
                            Terrain.SideH(X, Y).Road = Nothing
                        Else
                            Terrain.SideH(X, Y).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X
                        byteTemp = File.ReadByte
                        intTemp = CInt(byteTemp) - 1
                        If intTemp < 0 Then
                            Terrain.SideV(X, Y).Road = Nothing
                        ElseIf intTemp >= Painter.RoadCount Then
                            WarningCount += 1
                            Terrain.SideV(X, Y).Road = Nothing
                        Else
                            Terrain.SideV(X, Y).Road = Painter.Roads(intTemp)
                        End If
                    Next
                Next
                If WarningCount > 0 Then
                    ReturnResult.Warning_Add(WarningCount & " roads were out of range.")
                End If
                Dim TempUnitCount As UInteger
                TempUnitCount = File.ReadUInt32
                Dim TempUnit(CInt(TempUnitCount) - 1) As sFMEUnit
                For A = 0 To CInt(TempUnitCount) - 1
                    TempUnit(A).Code = New String(File.ReadChars(40))
                    B = Strings.InStr(TempUnit(A).Code, Chr(0))
                    If B > 0 Then
                        TempUnit(A).Code = Strings.Left(TempUnit(A).Code, B - 1)
                    End If
                    TempUnit(A).LNDType = File.ReadByte
                    TempUnit(A).ID = File.ReadUInt32
                    If Version = 6UI Then
                        TempUnit(A).SavePriority = File.ReadInt32
                    End If
                    TempUnit(A).X = File.ReadUInt32
                    TempUnit(A).Z = File.ReadUInt32
                    TempUnit(A).Y = File.ReadUInt32
                    TempUnit(A).Rotation = File.ReadUInt16
                    TempUnit(A).Name = ReadOldText(File)
                    TempUnit(A).Player = File.ReadByte
                Next

                Dim NewUnit As clsUnit
                Dim tmpUnitType As clsUnitType = Nothing
                Dim AvailableID As UInteger

                AvailableID = 1UI
                For A = 0 To CInt(TempUnitCount) - 1
                    If TempUnit(A).ID >= AvailableID Then
                        AvailableID = TempUnit(A).ID + 1UI
                    End If
                Next
                WarningCount = 0
                For A = 0 To CInt(TempUnitCount) - 1
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
                            NewUnit.UnitGroup = UnitGroups.Item(TempUnit(A).Player)
                        End If
                        NewUnit.Pos.Horizontal.X = CInt(TempUnit(A).X)
                        'NewUnit.Pos.Altitude = TempUnit(A).Y
                        NewUnit.Pos.Horizontal.Y = CInt(TempUnit(A).Z)
                        NewUnit.Rotation = Math.Min(CInt(TempUnit(A).Rotation), 359)
                        If TempUnit(A).ID = 0UI Then
                            TempUnit(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, TempUnit(A).ID)
                        End If
                        UnitAdd.ID = TempUnit(A).ID
                        UnitAdd.NewUnit = NewUnit
                        UnitAdd.Perform()
                        ErrorIDChange(TempUnit(A).ID, NewUnit, "Read_FMEv5+")
                        If AvailableID = TempUnit(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                    Else
                        WarningCount += 1
                    End If
                Next
                If WarningCount > 0 Then
                    ReturnResult.Warning_Add(WarningCount & " types of units were invalid. That many units were ignored.")
                End If

                Dim NewGatewayCount As UInteger
                Dim NewGateStart As sXY_int
                Dim NewGateFinish As sXY_int

                NewGatewayCount = File.ReadUInt32
                WarningCount = 0
                For A = 0 To CInt(NewGatewayCount) - 1
                    NewGateStart.X = File.ReadUInt16
                    NewGateStart.Y = File.ReadUInt16
                    NewGateFinish.X = File.ReadUInt16
                    NewGateFinish.Y = File.ReadUInt16
                    If Gateway_Create(NewGateStart, NewGateFinish) Is Nothing Then
                        WarningCount += 1
                    End If
                Next
                If WarningCount > 0 Then
                    ReturnResult.Warning_Add(WarningCount & " gateways were invalid.")
                End If

                If Tileset IsNot Nothing Then
                    For A = 0 To Tileset.TileCount - 1
                        byteTemp = File.ReadByte
                        Tile_TypeNum(A) = byteTemp
                    Next
                End If

                'scroll limits
                ResultInfo.ScrollMin.X = File.ReadInt32
                ResultInfo.ScrollMin.Y = File.ReadInt32
                ResultInfo.ScrollMax.X = File.ReadUInt32
                ResultInfo.ScrollMax.Y = File.ReadUInt32

                'other compile info

                Dim strTemp As String = Nothing

                ResultInfo.CompileName = ReadOldText(File)
                byteTemp = File.ReadByte
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
                ResultInfo.CompileMultiPlayers = ReadOldText(File)
                byteTemp = File.ReadByte
                Select Case byteTemp
                    Case 0
                        ResultInfo.CompileMultiXPlayers = False
                    Case 1
                        ResultInfo.CompileMultiXPlayers = True
                    Case Else
                        ReturnResult.Warning_Add("Compile player format out of range.")
                End Select
                ResultInfo.CompileMultiAuthor = ReadOldText(File)
                ResultInfo.CompileMultiLicense = ReadOldText(File)
                strTemp = ReadOldText(File) 'game time
                ResultInfo.CampaignGameType = File.ReadInt32
                If ResultInfo.CampaignGameType < -1 Or ResultInfo.CampaignGameType >= GameTypeCount Then
                    ReturnResult.Warning_Add("Compile campaign type out of range.")
                    ResultInfo.CampaignGameType = -1
                End If

                If File.PeekChar >= 0 Then
                    ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
                End If
            Else
                ReturnResult.Problem_Add("File version number not recognised.")
            End If

            InterfaceOptions = ResultInfo

        Catch ex As Exception
            ReturnResult.Problem_Add("Read error: " & ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Structure sLNDTile
        Public Vertex0Height As Short
        Public Vertex1Height As Short
        Public Vertex2Height As Short
        Public Vertex3Height As Short
        Public TID As Short
        Public VF As Short
        Public TF As Short
        Public F As Short
    End Structure

    Public Structure sLNDObject
        Public ID As UInteger
        Public TypeNum As Integer
        Public Code As String
        Public PlayerNum As Integer
        Public Name As String
        Public Pos As sXYZ_sng
        Public Rotation As sXYZ_int
    End Structure

    Public Function Load_LND(ByVal Path As String) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Problem = ""
        ReturnResult.Success = False

        Try

            Dim strTemp As String
            Dim strTemp2 As String
            Dim X As Integer
            Dim Y As Integer
            Dim A As Integer
            Dim B As Integer
            Dim Tile_Num As Integer
            Dim LineData As New sLines
            Dim LineCount As Integer
            Dim Bytes As sBytes
            Dim ByteCount As Integer
            Dim Line_Num As Integer
            Dim LNDTile() As sLNDTile
            Dim LNDObject(-1) As sLNDObject
            Dim UnitAdd As New clsMap.clsUnitAdd

            UnitAdd.Map = Me

            'load all bytes
            Bytes.Bytes = IO.File.ReadAllBytes(Path)
            ByteCount = Bytes.Bytes.GetUpperBound(0) + 1

            BytesToLines(Bytes, LineData)
            LineCount = LineData.Lines.GetUpperBound(0) + 1

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
            Dim LNDGate(-1) As clsGateway
            Dim LNDGateCount As Integer
            Dim C As Integer
            Dim D As Integer
            Dim GotText As Boolean
            Dim FlipX As Boolean
            Dim FlipZ As Boolean
            Dim Rotation As Byte
            Dim NewTileSize As sXY_int
            Dim dblTemp As Double

            Line_Num = 0
            Do While Line_Num < LineCount
                strTemp = LineData.Lines(Line_Num)

                A = InStr(1, strTemp, "TileWidth ")
                If A = 0 Then
                Else
                End If

                A = InStr(1, strTemp, "TileHeight ")
                If A = 0 Then
                Else
                End If

                A = InStr(1, strTemp, "MapWidth ")
                If A = 0 Then
                Else
                    InvariantParse_int(Right(strTemp, Len(strTemp) - (A + 8)), NewTileSize.X)
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "MapHeight ")
                If A = 0 Then
                Else
                    InvariantParse_int(Right(strTemp, Len(strTemp) - (A + 9)), NewTileSize.Y)
                    GoTo LineDone
                End If

                A = InStr(1, strTemp, "Textures {")
                If A = 0 Then
                Else
                    Line_Num += 1
                    strTemp = LineData.Lines(Line_Num)

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
                        strTemp = LineData.Lines(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            A = InStr(1, strTemp, "TID ")
                            If A = 0 Then
                                ReturnResult.Success = False
                                ReturnResult.Problem = "Tile ID missing"
                                Return ReturnResult
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 3)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                InvariantParse_short(strTemp2, LNDTile(Tile_Num).TID)
                            End If

                            A = InStr(1, strTemp, "VF ")
                            If A = 0 Then
                                ReturnResult.Success = False
                                ReturnResult.Problem = "Tile VF missing"
                                Return ReturnResult
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                InvariantParse_short(strTemp2, LNDTile(Tile_Num).VF)
                            End If

                            A = InStr(1, strTemp, "TF ")
                            If A = 0 Then
                                ReturnResult.Success = False
                                ReturnResult.Problem = "Tile TF missing"
                                Return ReturnResult
                            Else
                                strTemp2 = Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                InvariantParse_short(strTemp2, LNDTile(Tile_Num).TF)
                            End If

                            A = InStr(1, strTemp, " F ")
                            If A = 0 Then
                                ReturnResult.Success = False
                                ReturnResult.Problem = "Tile flip missing"
                                Return ReturnResult
                            Else
                                strTemp2 = Strings.Right(strTemp, strTemp.Length - A - 2)
                                A = InStr(1, strTemp2, " ")
                                If A > 0 Then
                                    strTemp2 = Left(strTemp2, A - 1)
                                End If
                                InvariantParse_short(strTemp2, LNDTile(Tile_Num).F)
                            End If

                            A = InStr(1, strTemp, " VH ")
                            If A = 0 Then
                                ReturnResult.Success = False
                                ReturnResult.Problem = "Tile height is missing"
                                Return ReturnResult
                            Else
                                strTemp3 = Right(strTemp, Len(strTemp) - A - 3)
                                For A = 0 To 2
                                    B = InStr(1, strTemp3, " ")
                                    If B = 0 Then
                                        ReturnResult.Success = False
                                        ReturnResult.Problem = "A tile height value is missing"
                                        Return ReturnResult
                                    End If
                                    strTemp2 = Left(strTemp3, B - 1)
                                    strTemp3 = Right(strTemp3, Len(strTemp3) - B)

                                    If A = 0 Then
                                        InvariantParse_short(strTemp2, LNDTile(Tile_Num).Vertex0Height)
                                    ElseIf A = 1 Then
                                        InvariantParse_short(strTemp2, LNDTile(Tile_Num).Vertex1Height)
                                    ElseIf A = 2 Then
                                        InvariantParse_short(strTemp2, LNDTile(Tile_Num).Vertex2Height)
                                    End If
                                Next
                                InvariantParse_short(strTemp3, LNDTile(Tile_Num).Vertex3Height)
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
                        strTemp = LineData.Lines(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            ObjectText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    ObjectText(C) &= strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 11 Then
                                            ReturnResult.Problem = "Too many fields for an object, or a space at the end."
                                            Return ReturnResult
                                        End If
                                        ObjectText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            ReDim Preserve LNDObject(ObjectCount)
                            With LNDObject(ObjectCount)
                                InvariantParse_uint(ObjectText(0), .ID)
                                InvariantParse_int(ObjectText(1), .TypeNum)
                                .Code = Mid(ObjectText(2), 2, ObjectText(2).Length - 2) 'remove quotes
                                InvariantParse_int(ObjectText(3), .PlayerNum)
                                .Name = Mid(ObjectText(4), 2, ObjectText(4).Length - 2) 'remove quotes
                                InvariantParse_sng(ObjectText(5), .Pos.X)
                                InvariantParse_sng(ObjectText(6), .Pos.Y)
                                InvariantParse_sng(ObjectText(7), .Pos.Z)
                                If InvariantParse_dbl(ObjectText(8), dblTemp) Then
                                    .Rotation.X = CInt(Clamp_dbl(dblTemp, 0.0#, 359.0#))
                                End If
                                If InvariantParse_dbl(ObjectText(9), dblTemp) Then
                                    .Rotation.Y = CInt(Clamp_dbl(dblTemp, 0.0#, 359.0#))
                                End If
                                If InvariantParse_dbl(ObjectText(10), dblTemp) Then
                                    .Rotation.Z = CInt(Clamp_dbl(dblTemp, 0.0#, 359.0#))
                                End If
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
                        strTemp = LineData.Lines(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            GateText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    GateText(C) &= strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 4 Then
                                            ReturnResult.Problem = "Too many fields for a gateway, or a space at the end."
                                            Return ReturnResult
                                        End If
                                        GateText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            ReDim Preserve LNDGate(LNDGateCount)
                            LNDGate(LNDGateCount) = New clsGateway
                            With LNDGate(LNDGateCount)
                                InvariantParse_int(GateText(0), .PosA.X)
                                .PosA.X = Math.Max(.PosA.X, 0)
                                InvariantParse_int(GateText(1), .PosA.Y)
                                .PosA.Y = Math.Max(.PosA.Y, 0)
                                InvariantParse_int(GateText(2), .PosB.X)
                                .PosB.X = Math.Max(.PosB.X, 0)
                                InvariantParse_int(GateText(3), .PosB.Y)
                                .PosB.Y = Math.Max(.PosB.Y, 0)
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
                        strTemp = LineData.Lines(Line_Num)

                        A = InStr(1, strTemp, "}")
                        If A = 0 Then

                            C = 0
                            TileTypeText(0) = ""
                            GotText = False
                            For B = 0 To strTemp.Length - 1
                                If strTemp.Chars(B) <> " " And strTemp.Chars(B) <> Chr(9) Then
                                    GotText = True
                                    TileTypeText(C) &= strTemp.Chars(B)
                                Else
                                    If GotText Then
                                        C += 1
                                        If C = 256 Then
                                            ReturnResult.Problem = "Too many fields for tile types."
                                            Return ReturnResult
                                        End If
                                        TileTypeText(C) = ""
                                        GotText = False
                                    End If
                                End If
                            Next

                            If TileTypeText(C) = "" Or TileTypeText(C) = " " Then C -= 1

                            For D = 0 To C
                                ReDim Preserve LNDTileType(LNDTileTypeCount)
                                LNDTileType(LNDTileTypeCount) = Math.Min(CByte(TileTypeText(D)), CByte(11))
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

            If NewTileSize.X < 1 Or NewTileSize.Y < 1 Then
                ReturnResult.Success = False
                ReturnResult.Problem = "The LND's terrain dimensions are missing or invalid."
                Return ReturnResult
            End If

            Terrain_Blank(NewTileSize)
            TileType_Reset()

            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Tile_Num = Y * Terrain.TileSize.X + X
                    'lnd uses different order! (3 = 2, 2 = 3), this program goes left to right, lnd goes clockwise around each tile
                    Terrain.Vertices(X, Y).Height = CByte(LNDTile(Tile_Num).Vertex0Height)
                Next
            Next

            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Tile_Num = Y * Terrain.TileSize.X + X

                    Terrain.Tiles(X, Y).Texture.TextureNum = LNDTile(Tile_Num).TID - 1

                    'ignore higher values
                    A = CInt(Int(LNDTile(Tile_Num).F / 64.0#))
                    LNDTile(Tile_Num).F = CShort(LNDTile(Tile_Num).F - A * 64)

                    A = CInt(Int(LNDTile(Tile_Num).F / 16.0#))
                    LNDTile(Tile_Num).F = CShort(LNDTile(Tile_Num).F - A * 16)
                    If A < 0 Or A > 3 Then
                        ReturnResult.Problem = "Invalid flip value."
                        Return ReturnResult
                    End If
                    Rotation = CByte(A)

                    A = CInt(Int(LNDTile(Tile_Num).F / 8.0#))
                    LNDTile(Tile_Num).F -= CShort(A * 8)
                    FlipZ = (A = 1)

                    A = CInt(Int(LNDTile(Tile_Num).F / 4.0#))
                    LNDTile(Tile_Num).F -= CShort(A * 4)
                    FlipX = (A = 1)

                    A = CInt(Int(LNDTile(Tile_Num).F / 2.0#))
                    LNDTile(Tile_Num).F -= CShort(A * 2)
                    Terrain.Tiles(X, Y).Tri = (A = 1)

                    'vf, tf, ignore

                    OldOrientation_To_TileOrientation(Rotation, FlipX, FlipZ, Terrain.Tiles(X, Y).Texture.Orientation)
                Next
            Next

            Dim NewUnit As clsUnit
            Dim XYZ_int As sXYZ_int
            Dim NewType As clsUnitType
            Dim AvailableID As UInteger

            AvailableID = 1UI
            For A = 0 To ObjectCount - 1
                If LNDObject(A).ID >= AvailableID Then
                    AvailableID = LNDObject(A).ID + 1UI
                End If
            Next
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
                    If LNDObject(A).PlayerNum < 0 Or LNDObject(A).PlayerNum >= PlayerCountMax Then
                        NewUnit.UnitGroup = ScavengerUnitGroup
                    Else
                        NewUnit.UnitGroup = UnitGroups.Item(LNDObject(A).PlayerNum)
                    End If
                    XYZ_int.X = CInt(LNDObject(A).Pos.X)
                    XYZ_int.Y = CInt(LNDObject(A).Pos.Y)
                    XYZ_int.Z = CInt(LNDObject(A).Pos.Z)
                    NewUnit.Pos = MapPos_From_LNDPos(XYZ_int)
                    NewUnit.Rotation = LNDObject(A).Rotation.Y
                    If LNDObject(A).ID = 0UI Then
                        LNDObject(A).ID = AvailableID
                        ZeroIDWarning(NewUnit, LNDObject(A).ID)
                    End If
                    UnitAdd.NewUnit = NewUnit
                    UnitAdd.ID = LNDObject(A).ID
                    UnitAdd.Perform()
                    ErrorIDChange(LNDObject(A).ID, NewUnit, "Load_LND")
                    If AvailableID = LNDObject(A).ID Then
                        AvailableID = NewUnit.ID + 1UI
                    End If
                End If
            Next

            For A = 0 To LNDGateCount - 1
                Gateway_Create(LNDGate(A).PosA, LNDGate(A).PosB)
            Next

            If Tileset IsNot Nothing Then
                For A = 0 To Math.Min(LNDTileTypeCount - 1, Tileset.TileCount) - 1
                    Tile_TypeNum(A) = LNDTileType(A + 1) 'lnd value 0 is ignored
                Next
            End If

        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function LNDPos_From_MapPos(ByVal Horizontal As sXY_int) As sXYZ_int
        Dim Result As sXYZ_int

        Result.X = Horizontal.X - CInt(Terrain.TileSize.X * TerrainGridSpacing / 2.0#)
        Result.Z = CInt(Terrain.TileSize.Y * TerrainGridSpacing / 2.0#) - Horizontal.Y
        Result.Y = CInt(GetTerrainHeight(Horizontal))

        Return Result
    End Function

    Public Function MapPos_From_LNDPos(ByVal Pos As sXYZ_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = Pos.X + CInt(Terrain.TileSize.X * TerrainGridSpacing / 2.0#)
        Result.Horizontal.Y = CInt(Terrain.TileSize.Y * TerrainGridSpacing / 2.0#) - Pos.Z
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Function Write_LND(ByVal Path As String, ByVal Overwrite As Boolean) As clsResult
        Dim ReturnResult As New clsResult

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                ReturnResult.Problem_Add("The selected file already exists.")
                Return ReturnResult
            End If
        End If

        Dim File As IO.StreamWriter = Nothing

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

            File = New IO.StreamWriter(New IO.FileStream(Path, IO.FileMode.CreateNew), New System.Text.UTF8Encoding(False, False))

            If Tileset Is Tileset_Arizona Then
                Text = "DataSet WarzoneDataC1.eds" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "DataSet WarzoneDataC2.eds" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "DataSet WarzoneDataC3.eds" & EndChar
            Else
                Text = "DataSet " & EndChar
            End If
            File.Write(Text)
            Text = "GrdLand {" & EndChar
            File.Write(Text)
            Text = "    Version 4" & EndChar
            File.Write(Text)
            Text = "    3DPosition 0.000000 3072.000000 0.000000" & EndChar
            File.Write(Text)
            Text = "    3DRotation 80.000000 0.000000 0.000000" & EndChar
            File.Write(Text)
            Text = "    2DPosition 0 0" & EndChar
            File.Write(Text)
            Text = "    CustomSnap 16 16" & EndChar
            File.Write(Text)
            Text = "    SnapMode 0" & EndChar
            File.Write(Text)
            Text = "    Gravity 1" & EndChar
            File.Write(Text)
            Text = "    HeightScale " & InvariantToString_int(HeightMultiplier) & EndChar
            File.Write(Text)
            Text = "    MapWidth " & InvariantToString_int(Terrain.TileSize.X) & EndChar
            File.Write(Text)
            Text = "    MapHeight " & InvariantToString_int(Terrain.TileSize.Y) & EndChar
            File.Write(Text)
            Text = "    TileWidth 128" & EndChar
            File.Write(Text)
            Text = "    TileHeight 128" & EndChar
            File.Write(Text)
            Text = "    SeaLevel 0" & EndChar
            File.Write(Text)
            Text = "    TextureWidth 64" & EndChar
            File.Write(Text)
            Text = "    TextureHeight 64" & EndChar
            File.Write(Text)
            Text = "    NumTextures 1" & EndChar
            File.Write(Text)
            Text = "    Textures {" & EndChar
            File.Write(Text)
            If Tileset Is Tileset_Arizona Then
                Text = "        texpages\tertilesc1.pcx" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "        texpages\tertilesc2.pcx" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "        texpages\tertilesc3.pcx" & EndChar
            Else
                Text = "        " & EndChar
            End If
            File.Write(Text)
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "    NumTiles " & InvariantToString_int(Terrain.TileSize.X * Terrain.TileSize.Y) & EndChar
            File.Write(Text)
            Text = "    Tiles {" & EndChar
            File.Write(Text)
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    TileOrientation_To_OldOrientation(Terrain.Tiles(X, Y).Texture.Orientation, Rotation, FlipX)
                    Flip = 0
                    If Terrain.Tiles(X, Y).Tri Then
                        Flip += CByte(2)
                    End If
                    If FlipX Then
                        Flip += CByte(4)
                    End If
                    Flip += CByte(Rotation * 16)

                    If Terrain.Tiles(X, Y).Tri Then
                        VF = 1
                    Else
                        VF = 0
                    End If
                    If FlipX Then
                        TF = 1
                    Else
                        TF = 0
                    End If

                    Text = "        TID " & Terrain.Tiles(X, Y).Texture.TextureNum + 1 & " VF " & InvariantToString_int(VF) & " TF " & InvariantToString_int(TF) & " F " & InvariantToString_int(Flip) & " VH " & InvariantToString_byte(Terrain.Vertices(X, Y).Height) & " " & InvariantToString_byte(Terrain.Vertices(X + 1, Y).Height) & " " & Terrain.Vertices(X + 1, Y + 1).Height & " " & InvariantToString_byte(Terrain.Vertices(X, Y + 1).Height) & EndChar
                    File.Write(Text)
                Next
            Next
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "ObjectList {" & EndChar
            File.Write(Text)
            Text = "    Version 3" & EndChar
            File.Write(Text)
            If Tileset Is Tileset_Arizona Then
                Text = "	FeatureSet WarzoneDataC1.eds" & EndChar
            ElseIf Tileset Is Tileset_Urban Then
                Text = "	FeatureSet WarzoneDataC2.eds" & EndChar
            ElseIf Tileset Is Tileset_Rockies Then
                Text = "	FeatureSet WarzoneDataC3.eds" & EndChar
            Else
                Text = "	FeatureSet " & EndChar
            End If
            File.Write(Text)
            Text = "    NumObjects " & InvariantToString_int(Units.ItemCount) & EndChar
            File.Write(Text)
            Text = "    Objects {" & EndChar
            File.Write(Text)
            Dim XYZ_int As sXYZ_int
            Dim strTemp As String = Nothing
            Dim CustomDroidCount As Integer = 0
            Dim tmpUnit As clsMap.clsUnit
            For A = 0 To Units.ItemCount - 1
                tmpUnit = Units.Item(A)
                Select Case tmpUnit.Type.Type
                    Case clsUnitType.enumType.Feature
                        B = 0
                    Case clsUnitType.enumType.PlayerStructure
                        B = 1
                    Case clsUnitType.enumType.PlayerDroid
                        If CType(tmpUnit.Type, clsDroidDesign).IsTemplate Then
                            B = 2
                        Else
                            B = -1
                        End If
                    Case Else
                        B = -1
                        ReturnResult.Warning_Add("Unit type classification not accounted for.")
                End Select
                XYZ_int = LNDPos_From_MapPos(Units.Item(A).Pos.Horizontal)
                If B >= 0 Then
                    If tmpUnit.Type.GetCode(strTemp) Then
                        Text = "        " & InvariantToString_uint(tmpUnit.ID) & " " & B & " " & Quote & strTemp & Quote & " " & tmpUnit.UnitGroup.GetLNDPlayerText & " " & Quote & "NONAME" & Quote & " " & InvariantToString_int(XYZ_int.X) & ".00 " & InvariantToString_int(XYZ_int.Y) & ".00 " & InvariantToString_int(XYZ_int.Z) & ".00 0.00 " & InvariantToString_int(tmpUnit.Rotation) & ".00 0.00" & EndChar
                        File.Write(Text)
                    Else
                        ReturnResult.Warning_Add("Error. Code not found.")
                    End If
                Else
                    CustomDroidCount += 1
                End If
            Next
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "ScrollLimits {" & EndChar
            File.Write(Text)
            Text = "    Version 1" & EndChar
            File.Write(Text)
            Text = "    NumLimits 1" & EndChar
            File.Write(Text)
            Text = "    Limits {" & EndChar
            File.Write(Text)
            Text = "        " & Quote & "Entire Map" & Quote & " 0 0 0 " & InvariantToString_int(Terrain.TileSize.X) & " " & InvariantToString_int(Terrain.TileSize.Y) & EndChar
            File.Write(Text)
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "Gateways {" & EndChar
            File.Write(Text)
            Text = "    Version 1" & EndChar
            File.Write(Text)
            Text = "    NumGateways " & InvariantToString_int(Gateways.ItemCount) & EndChar
            File.Write(Text)
            Text = "    Gates {" & EndChar
            File.Write(Text)
            Dim tmpGateway As clsGateway
            For A = 0 To Gateways.ItemCount - 1
                tmpGateway = Gateways.Item(A)
                Text = "        " & InvariantToString_int(tmpGateway.PosA.X) & " " & InvariantToString_int(tmpGateway.PosA.Y) & " " & InvariantToString_int(tmpGateway.PosB.X) & " " & InvariantToString_int(tmpGateway.PosB.Y) & EndChar
                File.Write(Text)
            Next
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "TileTypes {" & EndChar
            File.Write(Text)
            Text = "    NumTiles " & Tileset.TileCount & EndChar
            File.Write(Text)
            Text = "    Tiles {" & EndChar
            File.Write(Text)
            For A = 0 To CInt(Math.Ceiling((Tileset.TileCount + 1) / 16.0#)) - 1 '+1 because the first number is not a tile type
                Text = "        "
                C = A * 16 - 1 '-1 because the first number is not a tile type
                For B = 0 To Math.Min(16, Tileset.TileCount - C) - 1
                    If C + B < 0 Then
                        Text = Text & "2 "
                    Else
                        Text = Text & InvariantToString_byte(Tile_TypeNum(C + B)) & " "
                    End If
                Next
                Text = Text & EndChar
                File.Write(Text)
            Next
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "TileFlags {" & EndChar
            File.Write(Text)
            Text = "    NumTiles 90" & EndChar
            File.Write(Text)
            Text = "    Flags {" & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "        0 0 0 0 0 0 0 0 0 0 " & EndChar
            File.Write(Text)
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)
            Text = "Brushes {" & EndChar
            File.Write(Text)
            Text = "    Version 2" & EndChar
            File.Write(Text)
            Text = "    NumEdgeBrushes 0" & EndChar
            File.Write(Text)
            Text = "    NumUserBrushes 0" & EndChar
            File.Write(Text)
            Text = "    EdgeBrushes {" & EndChar
            File.Write(Text)
            Text = "    }" & EndChar
            File.Write(Text)
            Text = "}" & EndChar
            File.Write(Text)

        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try
        If File IsNot Nothing Then
            File.Close()
        End If

        Return ReturnResult
    End Function

    Public Function Write_MinimapFile(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim ReturnResult As sResult
        Dim X As Integer
        Dim Y As Integer

        Dim MinimapBitmap As New Bitmap(Terrain.TileSize.X, Terrain.TileSize.Y)

        Dim Texture As New clsMap.clsMinimapTexture(Terrain.TileSize)

        MinimapTextureFill(Texture)

        For Y = 0 To Texture.Pixels.GetUpperBound(0)
            For X = 0 To Texture.Pixels.GetUpperBound(1)
                MinimapBitmap.SetPixel(X, Y, Drawing.ColorTranslator.FromOle(OSRGB(Texture.Pixels(Y, X, 0), Texture.Pixels(Y, X, 1), Texture.Pixels(Y, X, 2))))
            Next
        Next

        ReturnResult = SaveBitmap(Path, Overwrite, MinimapBitmap)

        Return ReturnResult
    End Function

    Public Function Write_Heightmap(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim ReturnResult As sResult
        Dim HeightmapBitmap As New Bitmap(Terrain.TileSize.X + 1, Terrain.TileSize.Y + 1)
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                HeightmapBitmap.SetPixel(X, Y, Drawing.ColorTranslator.FromOle(OSRGB(Terrain.Vertices(X, Y).Height, Terrain.Vertices(X, Y).Height, Terrain.Vertices(X, Y).Height)))
            Next
        Next

        ReturnResult = SaveBitmap(Path, Overwrite, HeightmapBitmap)
        Return ReturnResult
    End Function

    Public Function Write_TTP(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                ReturnResult.Problem = "File already exists."
                Return ReturnResult
            End If
        End If

        Dim File_TTP As IO.BinaryWriter

        Try
            File_TTP = New IO.BinaryWriter(New IO.FileStream(Path, IO.FileMode.CreateNew), ASCIIEncoding)
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        Dim A As Integer

        WriteText(File_TTP, False, "ttyp")

        File_TTP.Write(8UI)
        If Tileset Is Nothing Then
            File_TTP.Write(0UI)
        Else
            File_TTP.Write(CUInt(Tileset.TileCount))
            For A = 0 To Tileset.TileCount - 1
                File_TTP.Write(CUShort(Tile_TypeNum(A)))
            Next
        End If
        File_TTP.Close()

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function Load_TTP(ByVal Path As String) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""
        Dim File As IO.BinaryReader

        Try
            File = New IO.BinaryReader(New IO.FileStream(Path, IO.FileMode.Open))
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try
        ReturnResult = Read_TTP(File)
        File.Close()

        Return ReturnResult
    End Function

    Public Function Write_FME(ByVal Path As String, ByVal Overwrite As Boolean, ByVal ScavengerPlayerNum As Byte) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        If IO.File.Exists(Path) Then
            If Overwrite Then
                IO.File.Delete(Path)
            Else
                ReturnResult.Problem = "The selected file already exists."
                Return ReturnResult
            End If
        End If

        Dim File As IO.BinaryWriter = Nothing

        Try

            File = New IO.BinaryWriter(New IO.FileStream(Path, IO.FileMode.CreateNew))

            Dim X As Integer
            Dim Z As Integer

            File.Write(6UI)

            If Tileset Is Nothing Then
                File.Write(CByte(0))
            ElseIf Tileset Is Tileset_Arizona Then
                File.Write(CByte(1))
            ElseIf Tileset Is Tileset_Urban Then
                File.Write(CByte(2))
            ElseIf Tileset Is Tileset_Rockies Then
                File.Write(CByte(3))
            End If

            File.Write(CUShort(Terrain.TileSize.X))
            File.Write(CUShort(Terrain.TileSize.Y))

            Dim TileAttributes As Byte
            Dim DownSideData As Byte

            For Z = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    File.Write(Terrain.Vertices(X, Z).Height)
                    If Terrain.Vertices(X, Z).Terrain Is Nothing Then
                        File.Write(CByte(0))
                    ElseIf Terrain.Vertices(X, Z).Terrain.Num < 0 Then
                        ReturnResult.Problem = "Terrain number out of range."
                        Return ReturnResult
                    Else
                        File.Write(CByte(Terrain.Vertices(X, Z).Terrain.Num + 1))
                    End If
                Next
            Next
            For Z = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    File.Write(CByte(Terrain.Tiles(X, Z).Texture.TextureNum + 1))

                    TileAttributes = 0
                    If Terrain.Tiles(X, Z).Terrain_IsCliff Then
                        TileAttributes += CByte(128)
                    End If
                    If Terrain.Tiles(X, Z).Texture.Orientation.SwitchedAxes Then
                        TileAttributes += CByte(64)
                    End If
                    If Terrain.Tiles(X, Z).Texture.Orientation.ResultXFlip Then
                        TileAttributes += CByte(32)
                    End If
                    If Terrain.Tiles(X, Z).Texture.Orientation.ResultYFlip Then
                        TileAttributes += CByte(16)
                    End If
                    '8 is free
                    If Terrain.Tiles(X, Z).Tri Then
                        TileAttributes += CByte(4)
                        If Terrain.Tiles(X, Z).TriTopLeftIsCliff Then
                            TileAttributes += CByte(2)
                        End If
                        If Terrain.Tiles(X, Z).TriBottomRightIsCliff Then
                            TileAttributes += CByte(1)
                        End If
                    Else
                        If Terrain.Tiles(X, Z).TriBottomLeftIsCliff Then
                            TileAttributes += CByte(2)
                        End If
                        If Terrain.Tiles(X, Z).TriTopRightIsCliff Then
                            TileAttributes += CByte(1)
                        End If
                    End If
                    File.Write(TileAttributes)
                    If IdenticalTileDirections(Terrain.Tiles(X, Z).DownSide, TileDirection_Top) Then
                        DownSideData = 1
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Z).DownSide, TileDirection_Left) Then
                        DownSideData = 2
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Z).DownSide, TileDirection_Right) Then
                        DownSideData = 3
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Z).DownSide, TileDirection_Bottom) Then
                        DownSideData = 4
                    Else
                        DownSideData = 0
                    End If
                    File.Write(DownSideData)
                Next
            Next
            For Z = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X - 1
                    If Terrain.SideH(X, Z).Road Is Nothing Then
                        File.Write(CByte(0))
                    ElseIf Terrain.SideH(X, Z).Road.Num < 0 Then
                        ReturnResult.Problem = "Road number out of range."
                        Return ReturnResult
                    Else
                        File.Write(CByte(Terrain.SideH(X, Z).Road.Num + 1))
                    End If
                Next
            Next
            For Z = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X
                    If Terrain.SideV(X, Z).Road Is Nothing Then
                        File.Write(CByte(0))
                    ElseIf Terrain.SideV(X, Z).Road.Num < 0 Then
                        ReturnResult.Problem = "Road number out of range."
                        Return ReturnResult
                    Else
                        File.Write(CByte(Terrain.SideV(X, Z).Road.Num + 1))
                    End If
                Next
            Next

            Dim OutputUnits(Units.ItemCount - 1) As clsUnit
            Dim OutputUnitCode(Units.ItemCount - 1) As String
            Dim OutputUnitCount As Integer = 0
            Dim tmpObject As clsUnit
            Dim A As Integer

            For A = 0 To Units.ItemCount - 1
                tmpObject = Units.Item(A)
                If tmpObject.Type.GetCode(OutputUnitCode(OutputUnitCount)) Then
                    OutputUnits(OutputUnitCount) = tmpObject
                    OutputUnitCount += 1
                End If
            Next

            File.Write(CUInt(OutputUnitCount))

            For A = 0 To OutputUnitCount - 1
                tmpObject = OutputUnits(A)
                WriteTextOfLength(File, 40, OutputUnitCode(A))
                Select Case tmpObject.Type.Type
                    Case clsUnitType.enumType.Feature
                        File.Write(CByte(0))
                    Case clsUnitType.enumType.PlayerStructure
                        File.Write(CByte(1))
                    Case clsUnitType.enumType.PlayerDroid
                        File.Write(CByte(2))
                End Select
                File.Write(tmpObject.ID)
                File.Write(tmpObject.SavePriority)
                File.Write(CUInt(tmpObject.Pos.Horizontal.X))
                File.Write(CUInt(tmpObject.Pos.Horizontal.Y))
                File.Write(CUInt(tmpObject.Pos.Altitude))
                File.Write(CUShort(tmpObject.Rotation))
                WriteText(File, True, "")
                If tmpObject.UnitGroup Is ScavengerUnitGroup Then
                    File.Write(ScavengerPlayerNum)
                Else
                    File.Write(CByte(tmpObject.UnitGroup.WZ_StartPos))
                End If
            Next

            File.Write(CUInt(Gateways.ItemCount))

            Dim tmpGateway As clsGateway
            For A = 0 To Gateways.ItemCount - 1
                tmpGateway = Gateways.Item(A)
                File.Write(CUShort(tmpGateway.PosA.X))
                File.Write(CUShort(tmpGateway.PosA.Y))
                File.Write(CUShort(tmpGateway.PosB.X))
                File.Write(CUShort(tmpGateway.PosB.Y))
            Next

            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    File.Write(Tile_TypeNum(A))
                Next
            End If

            'scroll limits
            File.Write(InterfaceOptions.ScrollMin.X)
            File.Write(InterfaceOptions.ScrollMin.Y)
            File.Write(InterfaceOptions.ScrollMax.X)
            File.Write(InterfaceOptions.ScrollMax.Y)

            'other compile info
            WriteText(File, True, InterfaceOptions.CompileName)
            File.Write(CByte(0)) 'multiplayer/campaign. 0 = neither
            WriteText(File, True, InterfaceOptions.CompileMultiPlayers)
            File.Write(InterfaceOptions.CompileMultiXPlayers)
            WriteText(File, True, InterfaceOptions.CompileMultiAuthor)
            WriteText(File, True, InterfaceOptions.CompileMultiLicense)
            WriteText(File, True, "0") 'game time
            Dim intTemp As Integer = InterfaceOptions.CampaignGameType
            File.Write(intTemp)

        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        If File IsNot Nothing Then
            File.Close()
        End If

        ReturnResult.Success = True
        Return ReturnResult
    End Function
End Class