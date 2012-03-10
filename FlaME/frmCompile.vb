Public Class frmCompile

    Private Map As clsMap

    Public Shared Function Create(ByVal Map As clsMap) As frmCompile

        If Map Is Nothing Then
            Stop
            Return Nothing
        End If

        If Map.CompileScreen IsNot Nothing Then
            Stop
            Return Nothing
        End If

        Return New frmCompile(Map)
    End Function

    Private Sub New(ByVal Map As clsMap)

        InitializeComponent()

        Icon = ProgramIcon

        Me.Map = Map
        Map.CompileScreen = Me

        UpdateControls()
    End Sub

    Private Sub UpdateControls()

        txtName.Text = Map.InterfaceOptions.CompileName

        txtMultiPlayers.Text = Map.InterfaceOptions.CompileMultiPlayers
        cbxNewPlayerFormat.Checked = Map.InterfaceOptions.CompileMultiXPlayers
        txtAuthor.Text = Map.InterfaceOptions.CompileMultiAuthor
        cboLicense.Text = Map.InterfaceOptions.CompileMultiLicense

        cboCampType.SelectedIndex = Map.InterfaceOptions.CampaignGameType

        cbxAutoScrollLimits.Checked = Map.InterfaceOptions.AutoScrollLimits
        AutoScrollLimits_Update()
        txtScrollMinX.Text = InvariantToString_int(Map.InterfaceOptions.ScrollMin.X)
        txtScrollMinY.Text = InvariantToString_int(Map.InterfaceOptions.ScrollMin.Y)
        txtScrollMaxX.Text = InvariantToString_sng(Map.InterfaceOptions.ScrollMax.X)
        txtScrollMaxY.Text = InvariantToString_sng(Map.InterfaceOptions.ScrollMax.Y)
    End Sub

    Private Sub SaveToMap()

        Map.InterfaceOptions.CompileName = txtName.Text

        Map.InterfaceOptions.CompileMultiPlayers = txtMultiPlayers.Text
        Map.InterfaceOptions.CompileMultiXPlayers = cbxNewPlayerFormat.Checked
        Map.InterfaceOptions.CompileMultiAuthor = txtAuthor.Text
        Map.InterfaceOptions.CompileMultiLicense = cboLicense.Text

        Map.InterfaceOptions.CampaignGameType = cboCampType.SelectedIndex

        Dim Invalid As Boolean = False

        Try
            Map.InterfaceOptions.ScrollMin.X = CInt(txtScrollMinX.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMin.X = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMin.Y = CInt(txtScrollMinY.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMin.Y = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMax.X = CUInt(txtScrollMaxX.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMax.X = 0
        End Try
        Try
            Map.InterfaceOptions.ScrollMax.Y = CUInt(txtScrollMaxY.Text)
        Catch ex As Exception
            Invalid = True
            Map.InterfaceOptions.ScrollMax.Y = 0
        End Try
        Map.InterfaceOptions.AutoScrollLimits = (cbxAutoScrollLimits.Checked Or Invalid)

        Map.SetChanged()

        UpdateControls() 'display to show any changes
    End Sub

    Private Sub btnCompile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompileMultiplayer.Click
        Dim ReturnResult As New clsResult
        Dim A As Integer

        SaveToMap()

        Dim MapName As String
        Dim License As String = cboLicense.Text
        Dim B As Integer
        Dim PlayerCount As Integer
        If Not InvariantParse_int(txtMultiPlayers.Text, PlayerCount) Then
            PlayerCount = 0
        End If
        Dim IsXPlayerFormat As Boolean = cbxNewPlayerFormat.Checked
        If PlayerCount < 2 Or PlayerCount > 10 Then
            ReturnResult.Problem_Add("The number of players must be from 2 to " & PlayerCountMax)
        End If
        If Not IsXPlayerFormat Then
            If PlayerCount <> 2 And PlayerCount <> 4 And PlayerCount <> 8 Then
                ReturnResult.Problem_Add("You must enable support for this number of players.")
            End If
        End If

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.Warning_Add(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If
        ReturnResult.Append(ValidateMap, "")

        ReturnResult.AppendAsWarning(ValidateMap_UnitPositions, "")

        ReturnResult.Append(ValidateMap_Multiplayer(PlayerCount, IsXPlayerFormat), "")

        MapName = txtName.Text
        For A = 0 To MapName.Length - 1
            B = Asc(MapName.Chars(A))
            If Not ((B >= 97 And B <= 122) Or (B >= 65 And B <= 90) Or (A >= 1 And ((B >= 48 And B <= 57) Or MapName.Chars(A) = "-"c Or MapName.Chars(A) = "_"c))) Then
                Exit For
            End If
        Next
        If A < MapName.Length Then
            ReturnResult.Problem_Add("The map's name must contain only letters, numbers, underscores and hyphens, and must begin with a letter.")
        End If
        If MapName.Length < 1 Or MapName.Length > 16 Then
            ReturnResult.Problem_Add("Map name must be from 1 to 16 characters.")
        End If
        If License = "" Then
            ReturnResult.Problem_Add("Enter a valid license.")
        End If
        If ReturnResult.HasProblems Then
            ShowWarnings(ReturnResult, "Compile Multiplayer")
            Exit Sub
        End If
        Dim CompileMultiDialog As New SaveFileDialog
        CompileMultiDialog.FileName = PlayerCount & "c-" & MapName
        CompileMultiDialog.Filter = "WZ Files (*.wz)|*.wz"
        If CompileMultiDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = CompileMultiDialog.FileName
        WriteWZArgs.Overwrite = True
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Multiplayer = New clsMap.sWrite_WZ_Args.clsMultiplayer
        WriteWZArgs.Multiplayer.AuthorName = txtAuthor.Text
        WriteWZArgs.Multiplayer.PlayerCount = PlayerCount
        WriteWZArgs.Multiplayer.IsBetaPlayerFormat = IsXPlayerFormat
        WriteWZArgs.Multiplayer.License = License
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer
        ReturnResult.Append(Map.Write_WZ(WriteWZArgs), "")
        ShowWarnings(ReturnResult, "Compile Multiplayer")
        If Not ReturnResult.HasWarnings Then
            Close()
        End If
    End Sub

    Private Sub frmCompile_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        Map.CompileScreen = Nothing
        Map = Nothing
    End Sub

    Private Sub frmCompile_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        SaveToMap()
    End Sub

    Private Function ValidateMap_UnitPositions() As clsResult
        Dim Result As New clsResult

        'check unit positions

        Dim A As Integer
        Dim TileHasUnit(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As Boolean
        Dim TileStructureType(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsStructureType
        Dim TileFeatureType(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsFeatureType
        Dim TileObjectGroup(Map.Terrain.TileSize.X - 1, Map.Terrain.TileSize.Y - 1) As clsMap.clsUnitGroup
        Dim X As Integer
        Dim Y As Integer
        Dim StartPos As sXY_int
        Dim FinishPos As sXY_int
        Dim CentrePos As sXY_int
        Dim StructureType As clsStructureType.enumStructureType
        Dim tmpStructure As clsStructureType
        Dim Footprint As sXY_int
        Dim UnitIsStructureModule(Map.Units.ItemCount - 1) As Boolean
        Dim IsValid As Boolean
        Dim tmpUnit As clsMap.clsUnit
        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(tmpUnit.Type, clsStructureType)
                StructureType = tmpStructure.StructureType
                UnitIsStructureModule(A) = (tmpStructure.IsModule Or StructureType = clsStructureType.enumStructureType.ResourceExtractor)
            End If
        Next
        'check and store non-module units first. modules need to check for the underlying unit.
        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If Not UnitIsStructureModule(A) Then
                Footprint = tmpUnit.Type.GetFootprint
                Map.GetFootprintTileRange(tmpUnit.Pos.Horizontal, Footprint, StartPos, FinishPos)
                If StartPos.X < 0 Or FinishPos.X >= Map.Terrain.TileSize.X _
                  Or StartPos.Y < 0 Or FinishPos.Y >= Map.Terrain.TileSize.Y Then
                    Result.Problem_Add("Unit off map at position " & tmpUnit.GetPosText & ".")
                Else
                    For Y = StartPos.Y To FinishPos.Y
                        For X = StartPos.X To FinishPos.X
                            If TileHasUnit(X, Y) Then
                                Result.Problem_Add("Bad unit overlap on tile " & X & ", " & Y & ".")
                            Else
                                TileHasUnit(X, Y) = True
                                If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                                    TileStructureType(X, Y) = CType(tmpUnit.Type, clsStructureType)
                                ElseIf tmpUnit.Type.Type = clsUnitType.enumType.Feature Then
                                    TileFeatureType(X, Y) = CType(tmpUnit.Type, clsFeatureType)
                                End If
                                TileObjectGroup(X, Y) = tmpUnit.UnitGroup
                            End If
                        Next
                    Next
                End If
            End If
        Next
        'check modules and extractors
        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If UnitIsStructureModule(A) Then
                StructureType = CType(tmpUnit.Type, clsStructureType).StructureType
                CentrePos.X = CInt(Int(tmpUnit.Pos.Horizontal.X / TerrainGridSpacing))
                CentrePos.Y = CInt(Int(tmpUnit.Pos.Horizontal.Y / TerrainGridSpacing))
                If CentrePos.X < 0 Or CentrePos.X >= Map.Terrain.TileSize.X _
                  Or CentrePos.Y < 0 Or CentrePos.Y >= Map.Terrain.TileSize.Y Then
                    Result.Problem_Add("Module off map at position " & tmpUnit.GetPosText & ".")
                Else
                    If TileStructureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If TileObjectGroup(CentrePos.X, CentrePos.Y) Is tmpUnit.UnitGroup Then
                            If StructureType = clsStructureType.enumStructureType.FactoryModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Factory _
                                  Or TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.VTOLFactory Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            ElseIf StructureType = clsStructureType.enumStructureType.PowerModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.PowerGenerator Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            ElseIf StructureType = clsStructureType.enumStructureType.ResearchModule Then
                                If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Research Then
                                    IsValid = True
                                Else
                                    IsValid = False
                                End If
                            Else
                                IsValid = False
                            End If
                        Else
                            IsValid = False
                        End If
                    ElseIf TileFeatureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If StructureType = clsStructureType.enumStructureType.ResourceExtractor Then
                            If TileFeatureType(CentrePos.X, CentrePos.Y).FeatureType = clsFeatureType.enumFeatureType.OilResource Then
                                IsValid = True
                            Else
                                IsValid = False
                            End If
                        Else
                            IsValid = False
                        End If
                    ElseIf StructureType = clsStructureType.enumStructureType.ResourceExtractor Then
                        IsValid = True
                    Else
                        IsValid = False
                    End If
                    If Not IsValid Then
                        Result.Problem_Add("Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                    End If
                End If
            End If
        Next

        Return Result
    End Function

    Private Function ValidateMap_Multiplayer(ByVal PlayerCount As Integer, ByVal IsXPlayerFormat As Boolean) As clsResult
        Dim Result As New clsResult

        If PlayerCount < 2 Or PlayerCount > PlayerCountMax Then
            Result.Problem_Add("Unable to evaluate for multiplayer due to bad number of players.")
            Return Result
        End If

        'check HQs, Trucks and unit counts

        Dim PlayerHQCount(PlayerCountMax - 1) As Integer
        Dim Player23TruckCount(PlayerCountMax - 1) As Integer
        Dim PlayerMasterTruckCount(PlayerCountMax - 1) As Integer
        Dim ScavPlayerNum As Integer
        Dim ScavObjectCount As Integer = 0
        Dim tmpDroid As clsDroidDesign
        Dim tmpStructure As clsStructureType
        Dim UnusedPlayerUnitWarningCount As Integer = 0
        Dim tmpUnit As clsMap.clsUnit
        Dim A As Integer

        ScavPlayerNum = Math.Max(PlayerCount, 7)

        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If tmpUnit.UnitGroup Is Map.ScavengerUnitGroup Then

            Else
                If tmpUnit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                    tmpDroid = CType(tmpUnit.Type, clsDroidDesign)
                    If tmpDroid.Body IsNot Nothing And tmpDroid.Propulsion IsNot Nothing And tmpDroid.Turret1 IsNot Nothing And tmpDroid.TurretCount = 1 Then
                        If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Construct Then
                            PlayerMasterTruckCount(tmpUnit.UnitGroup.WZ_StartPos) += 1
                            If tmpDroid.IsTemplate Then
                                Player23TruckCount(tmpUnit.UnitGroup.WZ_StartPos) += 1
                            End If
                        End If
                    End If
                ElseIf tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(tmpUnit.Type, clsStructureType)
                    If tmpStructure.Code = "A0CommandCentre" Then
                        PlayerHQCount(tmpUnit.UnitGroup.WZ_StartPos) += 1
                    End If
                End If
            End If
            If tmpUnit.Type.Type <> clsUnitType.enumType.Feature Then
                If tmpUnit.UnitGroup.WZ_StartPos = ScavPlayerNum Or tmpUnit.UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavObjectCount += 1
                ElseIf tmpUnit.UnitGroup.WZ_StartPos >= PlayerCount Then
                    If UnusedPlayerUnitWarningCount < 32 Then
                        UnusedPlayerUnitWarningCount += 1
                        Result.Problem_Add("An unused player (" & tmpUnit.UnitGroup.WZ_StartPos & ") has a unit at " & tmpUnit.GetPosText & ".")
                    End If
                End If
            End If
        Next

        If ScavPlayerNum <= 7 Or IsXPlayerFormat Then

        ElseIf ScavObjectCount > 0 Then 'only counts non-features
            Result.Problem_Add("Scavengers are not supported on a map with this number of players without enabling X player support.")
        End If

        For A = 0 To PlayerCount - 1
            If PlayerHQCount(A) = 0 Then
                Result.Problem_Add("There is no Command Centre for player " & A & ".")
            End If
            If PlayerMasterTruckCount(A) = 0 Then
                Result.Problem_Add("There are no constructor units for player " & A & ".")
            ElseIf Player23TruckCount(A) = 0 Then
                Result.Warning_Add("All constructor units for player " & A & " will only exist in master.")
            End If
        Next

        Return Result
    End Function

    Private Function ValidateMap() As clsResult
        Dim ReturnResult As New clsResult

        If Map.Terrain.TileSize.X > WZMapMaxSize Then
            ReturnResult.Warning_Add("Map width is too large. The maximum is " & WZMapMaxSize & ".")
        End If
        If Map.Terrain.TileSize.Y > WZMapMaxSize Then
            ReturnResult.Warning_Add("Map height is too large. The maximum is " & WZMapMaxSize & ".")
        End If

        If Map.Tileset Is Nothing Then
            ReturnResult.Problem_Add("No tileset selected.")
        End If

        Dim A As Integer
        Dim B As Integer
        'Dim PlayerFactoryCount(FactionCountMax - 1) As Integer
        Dim PlayerStructureTypeCount(PlayerCountMax - 1, ObjectData.StructureTypes.ItemCount - 1) As Integer
        Dim ScavStructureTypeCount(ObjectData.StructureTypes.ItemCount - 1) As Integer
        Dim tmpStructure As clsStructureType
        Dim tmpUnit As clsMap.clsUnit
        Dim UnitType As clsUnitType

        For A = 0 To Map.Units.ItemCount - 1
            tmpUnit = Map.Units.Item(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(tmpUnit.Type, clsStructureType)
                If tmpUnit.UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavStructureTypeCount(tmpStructure.StructureType_ObjectDataLink.ArrayPosition) += 1
                Else
                    PlayerStructureTypeCount(tmpUnit.UnitGroup.WZ_StartPos, tmpStructure.StructureType_ObjectDataLink.ArrayPosition) += 1
                End If
            End If
        Next

        For B = 0 To ObjectData.UnitTypes.ItemCount - 1
            UnitType = ObjectData.UnitTypes(B)
            If UnitType.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(UnitType, clsStructureType)
                For A = 0 To PlayerCountMax - 1
                    If PlayerStructureTypeCount(A, B) > 255 Then
                        ReturnResult.Problem_Add("Player " & A & " has too many (" & PlayerStructureTypeCount(A, B) & ") of structure " & ControlChars.Quote & tmpStructure.Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                    End If
                Next
                If ScavStructureTypeCount(B) > 255 Then
                    ReturnResult.Problem_Add("Scavengers have too many (" & ScavStructureTypeCount(B) & ") of structure " & ControlChars.Quote & tmpStructure.Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                End If
            End If
        Next

        Return ReturnResult
    End Function

    Private Function ValidateMap_WaterTris() As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Count As Integer

        If Map.Tileset Is Nothing Then
            Return 0
        End If

        For Y = 0 To Map.Terrain.TileSize.Y - 1
            For X = 0 To Map.Terrain.TileSize.X - 1
                If Map.Terrain.Tiles(X, Y).Tri Then
                    If Map.Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Map.Terrain.Tiles(X, Y).Texture.TextureNum < Map.Tileset.TileCount Then
                        If Map.Tileset.Tiles(Map.Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            Count += 1
                        End If
                    End If
                End If
            Next
        Next
        Return Count
    End Function

    Private Sub btnCompileCampaign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompileCampaign.Click
        Dim ReturnResult As New clsResult
        Dim A As Integer

        SaveToMap()

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.Warning_Add(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If
        ReturnResult.Append(ValidateMap, "")

        ReturnResult.AppendAsWarning(ValidateMap_UnitPositions, "")

        Dim MapName As String
        Dim TypeNum As Integer

        MapName = txtName.Text
        If MapName.Length < 1 Then
            ReturnResult.Problem_Add("Enter a name for the campaign files.")
        End If
        TypeNum = cboCampType.SelectedIndex
        If TypeNum < 0 Or TypeNum > 2 Then
            ReturnResult.Problem_Add("Select a campaign type.")
        End If
        If ReturnResult.HasProblems Then
            ShowWarnings(ReturnResult, "Compile Campaign")
            Exit Sub
        End If
        Dim CompileCampDialog As New FolderBrowserDialog
        If CompileCampDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = CompileCampDialog.SelectedPath
        WriteWZArgs.Overwrite = False
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Campaign = New clsMap.sWrite_WZ_Args.clsCampaign
        'If InvariantParse_dbl(txtCampTime.Text, dblTemp) Then
        '    WriteWZArgs.Campaign.GAMTime = CUInt(Clamp_dbl(dblTemp, UInteger.MinValue, UInteger.MaxValue))
        'Else
        '    WriteWZArgs.Campaign.GAMTime = UInteger.MinValue
        'End If
        WriteWZArgs.Campaign.GAMType = CUInt(TypeNum)
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign
        ReturnResult.Append(Map.Write_WZ(WriteWZArgs), "")
        ShowWarnings(ReturnResult, "Compile Campaign")
        If Not ReturnResult.HasWarnings Then
            Close()
        End If
    End Sub

    Public Sub AutoScrollLimits_Update()

        If cbxAutoScrollLimits.Checked Then
            txtScrollMinX.Enabled = False
            txtScrollMaxX.Enabled = False
            txtScrollMinY.Enabled = False
            txtScrollMaxY.Enabled = False
        Else
            txtScrollMinX.Enabled = True
            txtScrollMaxX.Enabled = True
            txtScrollMinY.Enabled = True
            txtScrollMaxY.Enabled = True
        End If
    End Sub

    Private Sub cbxAutoScrollLimits_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbxAutoScrollLimits.CheckedChanged
        If Not cbxAutoScrollLimits.Enabled Then
            Exit Sub
        End If

        AutoScrollLimits_Update()
    End Sub

    Private Sub SetScrollLimits(ByRef Min As sXY_int, ByRef Max As sXY_uint)

        Min.X = 0
        Min.Y = 0
        Max.X = CUInt(Map.Terrain.TileSize.X)
        Max.Y = CUInt(Map.Terrain.TileSize.Y)
        If Not cbxAutoScrollLimits.Checked Then
            InvariantParse_int(txtScrollMinX.Text, Min.X)
            InvariantParse_int(txtScrollMinY.Text, Min.Y)
            InvariantParse_uint(txtScrollMaxX.Text, Max.X)
            InvariantParse_uint(txtScrollMaxY.Text, Max.Y)
        End If
    End Sub
End Class