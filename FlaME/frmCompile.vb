Public Class frmCompile
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Private Map As clsMap

    Public Sub New(ByVal Map As clsMap)
        InitializeComponent() 'required for monodevelop too

        Icon = ProgramIcon

        If Map Is Nothing Then
            Stop
        End If

        Me.Map = Map

        UpdateControls()
    End Sub

    Private Sub UpdateControls()

        txtName.Text = Map.InterfaceOptions.CompileName

        txtMultiPlayers.Text = Map.InterfaceOptions.CompileMultiPlayers
        cbxNewPlayerFormat.Checked = Map.InterfaceOptions.CompileMultiXPlayers
        txtAuthor.Text = Map.InterfaceOptions.CompileMultiAuthor
        cboLicense.Text = Map.InterfaceOptions.CompileMultiLicense

        txtCampTime.Text = InvariantToString_int(Map.InterfaceOptions.CampaignGameTime)
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

        Try
            Map.InterfaceOptions.CampaignGameTime = CInt(txtCampTime.Text)
        Catch ex As Exception
            Map.InterfaceOptions.CampaignGameTime = 2
        End Try
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

        ReturnResult.Append(ValidateMap_UnitPositions, "")

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
        SaveFileDialog.FileName = PlayerCount & "c-" & MapName
        SaveFileDialog.Filter = "WZ Files (*.wz)|*.wz"
        If SaveFileDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = SaveFileDialog.FileName
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
            Hide()
        End If
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
        Dim UnitIsStructureModule(Map.UnitCount - 1) As Boolean
        Dim IsValid As Boolean
        For A = 0 To Map.UnitCount - 1
            If Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(Map.Units(A).Type, clsStructureType)
                StructureType = tmpStructure.StructureType
                UnitIsStructureModule(A) = (StructureType = clsStructureType.enumStructureType.FactoryModule _
                   Or StructureType = clsStructureType.enumStructureType.PowerModule _
                   Or StructureType = clsStructureType.enumStructureType.ResearchModule _
                   Or StructureType = clsStructureType.enumStructureType.ResourceExtractor)
            End If
        Next
        'check and store non-module units first. modules need to check for the underlying unit.
        For A = 0 To Map.UnitCount - 1
            If Not UnitIsStructureModule(A) Then
                Footprint = Map.Units(A).Type.GetFootprint
                Map.GetFootprintTileRange(Map.Units(A).Pos.Horizontal, Footprint, StartPos, FinishPos)
                If StartPos.X < 0 Or FinishPos.X >= Map.Terrain.TileSize.X _
                  Or StartPos.Y < 0 Or FinishPos.Y >= Map.Terrain.TileSize.Y Then
                    Result.Problem_Add("Unit off map at position " & Map.Units(A).GetPosText & ".")
                Else
                    For Y = StartPos.Y To FinishPos.Y
                        For X = StartPos.X To FinishPos.X
                            If TileHasUnit(X, Y) Then
                                Result.Problem_Add("Bad unit overlap on tile " & X & ", " & Y & ".")
                            Else
                                TileHasUnit(X, Y) = True
                                If Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                                    TileStructureType(X, Y) = CType(Map.Units(A).Type, clsStructureType)
                                ElseIf Map.Units(A).Type.Type = clsUnitType.enumType.Feature Then
                                    TileFeatureType(X, Y) = CType(Map.Units(A).Type, clsFeatureType)
                                End If
                                TileObjectGroup(X, Y) = Map.Units(A).UnitGroup
                            End If
                        Next
                    Next
                End If
            End If
        Next
        'check modules and extractors
        For A = 0 To Map.UnitCount - 1
            If UnitIsStructureModule(A) Then
                StructureType = CType(Map.Units(A).Type, clsStructureType).StructureType
                CentrePos.X = CInt(Int(Map.Units(A).Pos.Horizontal.X / TerrainGridSpacing))
                CentrePos.Y = CInt(Int(Map.Units(A).Pos.Horizontal.Y / TerrainGridSpacing))
                If CentrePos.X < 0 Or CentrePos.X >= Map.Terrain.TileSize.X _
                  Or CentrePos.Y < 0 Or CentrePos.Y >= Map.Terrain.TileSize.Y Then
                    Result.Problem_Add("Module off map at position " & Map.Units(A).GetPosText & ".")
                Else
                    If TileStructureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If TileObjectGroup(CentrePos.X, CentrePos.Y) Is Map.Units(A).UnitGroup Then
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
        Dim A As Integer

        ScavPlayerNum = Math.Max(PlayerCount, 7)

        For A = 0 To Map.UnitCount - 1
            If Map.Units(A).UnitGroup Is Map.ScavengerUnitGroup Then

            Else
                If Map.Units(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    tmpDroid = CType(Map.Units(A).Type, clsDroidDesign)
                    If tmpDroid.Body IsNot Nothing And tmpDroid.Propulsion IsNot Nothing And tmpDroid.Turret1 IsNot Nothing And tmpDroid.TurretCount = 1 Then
                        If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Construct Then
                            PlayerMasterTruckCount(Map.Units(A).UnitGroup.WZ_StartPos) += 1
                            If tmpDroid.IsTemplate Then
                                Player23TruckCount(Map.Units(A).UnitGroup.WZ_StartPos) += 1
                            End If
                        End If
                    End If
                ElseIf Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(Map.Units(A).Type, clsStructureType)
                    If tmpStructure.Code = "A0CommandCentre" Then
                        PlayerHQCount(Map.Units(A).UnitGroup.WZ_StartPos) += 1
                    End If
                End If
            End If
            If Map.Units(A).Type.Type <> clsUnitType.enumType.Feature Then
                If Map.Units(A).UnitGroup.WZ_StartPos = ScavPlayerNum Or Map.Units(A).UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavObjectCount += 1
                ElseIf Map.Units(A).UnitGroup.WZ_StartPos >= PlayerCount Then
                    If UnusedPlayerUnitWarningCount < 32 Then
                        UnusedPlayerUnitWarningCount += 1
                        Result.Problem_Add("An unused player (" & Map.Units(A).UnitGroup.WZ_StartPos & ") has a unit at " & Map.Units(A).GetPosText & ".")
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
        Dim PlayerStructureTypeCount(PlayerCountMax - 1, UnitTypeCount - 1) As Integer
        Dim ScavStructureTypeCount(UnitTypeCount - 1) As Integer
        Dim tmpStructure As clsStructureType

        For A = 0 To Map.UnitCount - 1
            If Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(Map.Units(A).Type, clsStructureType)
                If Map.Units(A).UnitGroup Is Map.ScavengerUnitGroup Then
                    ScavStructureTypeCount(tmpStructure.StructureNum) += 1
                Else
                    PlayerStructureTypeCount(Map.Units(A).UnitGroup.WZ_StartPos, tmpStructure.StructureNum) += 1
                End If
            End If
        Next

        For B = 0 To UnitTypeCount - 1
            If UnitTypes(B).Type = clsUnitType.enumType.PlayerStructure Then
                For A = 0 To PlayerCountMax - 1
                    If PlayerStructureTypeCount(A, B) > 255 Then
                        ReturnResult.Problem_Add("Player " & A & " has too many (" & PlayerStructureTypeCount(A, B) & ") of structure " & ControlChars.Quote & CType(UnitTypes(B), clsStructureType).Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                    End If
                Next
                If ScavStructureTypeCount(B) > 255 Then
                    ReturnResult.Problem_Add("Scavengers have too many (" & ScavStructureTypeCount(B) & ") of structure " & ControlChars.Quote & CType(UnitTypes(B), clsStructureType).Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                End If
            End If
        Next
ExitLoop:
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
        Dim dblTemp As Double

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
        If FolderBrowserDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = FolderBrowserDialog.SelectedPath
        WriteWZArgs.Overwrite = False
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Campaign = New clsMap.sWrite_WZ_Args.clsCampaign
        If InvariantParse_dbl(txtCampTime.Text, dblTemp) Then
            WriteWZArgs.Campaign.GAMTime = CUInt(Clamp_dbl(dblTemp, UInteger.MinValue, UInteger.MaxValue))
        Else
            WriteWZArgs.Campaign.GAMTime = UInteger.MinValue
        End If
        WriteWZArgs.Campaign.GAMType = CUInt(TypeNum)
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign
        ReturnResult.Append(Map.Write_WZ(WriteWZArgs), "")
        ShowWarnings(ReturnResult, "Compile Campaign")
        If Not ReturnResult.HasWarnings Then
            Hide()
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

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtMultiPlayers = New System.Windows.Forms.TextBox()
        Me.btnCompileMultiplayer = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCampTime = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtScrollMaxX = New System.Windows.Forms.TextBox()
        Me.txtScrollMaxY = New System.Windows.Forms.TextBox()
        Me.txtScrollMinY = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtScrollMinX = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.txtAuthor = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cboLicense = New System.Windows.Forms.ComboBox()
        Me.cbxNewPlayerFormat = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cboCampType = New System.Windows.Forms.ComboBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnCompileCampaign = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cbxAutoScrollLimits = New System.Windows.Forms.CheckBox()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(84, 15)
        Me.txtName.Margin = New System.Windows.Forms.Padding(4)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(140, 22)
        Me.txtName.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(25, 18)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 20)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Name:"
        Me.Label1.UseCompatibleTextRendering = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(18, 20)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Players:"
        Me.Label2.UseCompatibleTextRendering = True
        '
        'txtMultiPlayers
        '
        Me.txtMultiPlayers.Location = New System.Drawing.Point(81, 16)
        Me.txtMultiPlayers.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMultiPlayers.Name = "txtMultiPlayers"
        Me.txtMultiPlayers.Size = New System.Drawing.Size(68, 22)
        Me.txtMultiPlayers.TabIndex = 5
        '
        'btnCompileMultiplayer
        '
        Me.btnCompileMultiplayer.Location = New System.Drawing.Point(251, 128)
        Me.btnCompileMultiplayer.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCompileMultiplayer.Name = "btnCompileMultiplayer"
        Me.btnCompileMultiplayer.Size = New System.Drawing.Size(128, 30)
        Me.btnCompileMultiplayer.TabIndex = 10
        Me.btnCompileMultiplayer.Text = "Compile"
        Me.btnCompileMultiplayer.UseCompatibleTextRendering = True
        Me.btnCompileMultiplayer.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 22)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 20)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Time:"
        Me.Label3.UseCompatibleTextRendering = True
        '
        'txtCampTime
        '
        Me.txtCampTime.Location = New System.Drawing.Point(70, 19)
        Me.txtCampTime.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampTime.Name = "txtCampTime"
        Me.txtCampTime.Size = New System.Drawing.Size(68, 22)
        Me.txtCampTime.TabIndex = 11
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(154, 22)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 20)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Type:"
        Me.Label5.UseCompatibleTextRendering = True
        '
        'txtScrollMaxX
        '
        Me.txtScrollMaxX.Location = New System.Drawing.Point(82, 88)
        Me.txtScrollMaxX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtScrollMaxX.Name = "txtScrollMaxX"
        Me.txtScrollMaxX.Size = New System.Drawing.Size(61, 22)
        Me.txtScrollMaxX.TabIndex = 15
        '
        'txtScrollMaxY
        '
        Me.txtScrollMaxY.Location = New System.Drawing.Point(151, 88)
        Me.txtScrollMaxY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtScrollMaxY.Name = "txtScrollMaxY"
        Me.txtScrollMaxY.Size = New System.Drawing.Size(61, 22)
        Me.txtScrollMaxY.TabIndex = 18
        '
        'txtScrollMinY
        '
        Me.txtScrollMinY.Location = New System.Drawing.Point(151, 48)
        Me.txtScrollMinY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtScrollMinY.Name = "txtScrollMinY"
        Me.txtScrollMinY.Size = New System.Drawing.Size(61, 22)
        Me.txtScrollMinY.TabIndex = 22
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(82, 27)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(15, 20)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "x:"
        Me.Label10.UseCompatibleTextRendering = True
        '
        'txtScrollMinX
        '
        Me.txtScrollMinX.Location = New System.Drawing.Point(82, 48)
        Me.txtScrollMinX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtScrollMinX.Name = "txtScrollMinX"
        Me.txtScrollMinX.Size = New System.Drawing.Size(61, 22)
        Me.txtScrollMinX.TabIndex = 20
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(10, 51)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(63, 20)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Minimum:"
        Me.Label11.UseCompatibleTextRendering = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(7, 88)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(67, 20)
        Me.Label12.TabIndex = 25
        Me.Label12.Text = "Maximum:"
        Me.Label12.UseCompatibleTextRendering = True
        '
        'txtAuthor
        '
        Me.txtAuthor.Location = New System.Drawing.Point(81, 48)
        Me.txtAuthor.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAuthor.Name = "txtAuthor"
        Me.txtAuthor.Size = New System.Drawing.Size(123, 22)
        Me.txtAuthor.TabIndex = 27
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(22, 52)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 20)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Author:"
        Me.Label4.UseCompatibleTextRendering = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(14, 84)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(55, 20)
        Me.Label13.TabIndex = 28
        Me.Label13.Text = "License:"
        Me.Label13.UseCompatibleTextRendering = True
        '
        'cboLicense
        '
        Me.cboLicense.FormattingEnabled = True
        Me.cboLicense.Items.AddRange(New Object() {"GPL 2+", "CC BY 3.0 + GPL v2+", "CC BY-SA 3.0 + GPL v2+", "CC0"})
        Me.cboLicense.Location = New System.Drawing.Point(81, 80)
        Me.cboLicense.Margin = New System.Windows.Forms.Padding(4)
        Me.cboLicense.Name = "cboLicense"
        Me.cboLicense.Size = New System.Drawing.Size(172, 24)
        Me.cboLicense.TabIndex = 29
        '
        'cbxNewPlayerFormat
        '
        Me.cbxNewPlayerFormat.Location = New System.Drawing.Point(158, 15)
        Me.cbxNewPlayerFormat.Margin = New System.Windows.Forms.Padding(4)
        Me.cbxNewPlayerFormat.Name = "cbxNewPlayerFormat"
        Me.cbxNewPlayerFormat.Size = New System.Drawing.Size(221, 25)
        Me.cbxNewPlayerFormat.TabIndex = 30
        Me.cbxNewPlayerFormat.Text = "X Player Support"
        Me.cbxNewPlayerFormat.UseCompatibleTextRendering = True
        Me.cbxNewPlayerFormat.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(148, 27)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(15, 20)
        Me.Label7.TabIndex = 31
        Me.Label7.Text = "y:"
        Me.Label7.UseCompatibleTextRendering = True
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(262, 80)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(132, 41)
        Me.Label8.TabIndex = 32
        Me.Label8.Text = "Select from the list or type another."
        Me.Label8.UseCompatibleTextRendering = True
        '
        'cboCampType
        '
        Me.cboCampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCampType.FormattingEnabled = True
        Me.cboCampType.Items.AddRange(New Object() {"Initial scenario state", "Scenario scroll area expansion", "Stand alone mission"})
        Me.cboCampType.Location = New System.Drawing.Point(207, 19)
        Me.cboCampType.Margin = New System.Windows.Forms.Padding(4)
        Me.cboCampType.Name = "cboCampType"
        Me.cboCampType.Size = New System.Drawing.Size(160, 24)
        Me.cboCampType.TabIndex = 33
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 53)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(415, 194)
        Me.TabControl1.TabIndex = 34
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.txtMultiPlayers)
        Me.TabPage1.Controls.Add(Me.Label8)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.txtAuthor)
        Me.TabPage1.Controls.Add(Me.cbxNewPlayerFormat)
        Me.TabPage1.Controls.Add(Me.Label13)
        Me.TabPage1.Controls.Add(Me.cboLicense)
        Me.TabPage1.Controls.Add(Me.btnCompileMultiplayer)
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(407, 165)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Multiplayer"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnCompileCampaign)
        Me.TabPage2.Controls.Add(Me.cboCampType)
        Me.TabPage2.Controls.Add(Me.txtCampTime)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(407, 165)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Campaign"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnCompileCampaign
        '
        Me.btnCompileCampaign.Location = New System.Drawing.Point(253, 128)
        Me.btnCompileCampaign.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCompileCampaign.Name = "btnCompileCampaign"
        Me.btnCompileCampaign.Size = New System.Drawing.Size(128, 30)
        Me.btnCompileCampaign.TabIndex = 11
        Me.btnCompileCampaign.Text = "Compile"
        Me.btnCompileCampaign.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.txtScrollMaxX)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.txtScrollMaxY)
        Me.GroupBox1.Controls.Add(Me.txtScrollMinX)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.txtScrollMinY)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 280)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(257, 132)
        Me.GroupBox1.TabIndex = 35
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scroll Limits"
        Me.GroupBox1.UseCompatibleTextRendering = True
        '
        'cbxAutoScrollLimits
        '
        Me.cbxAutoScrollLimits.AutoSize = True
        Me.cbxAutoScrollLimits.Location = New System.Drawing.Point(12, 253)
        Me.cbxAutoScrollLimits.Name = "cbxAutoScrollLimits"
        Me.cbxAutoScrollLimits.Size = New System.Drawing.Size(162, 21)
        Me.cbxAutoScrollLimits.TabIndex = 32
        Me.cbxAutoScrollLimits.Text = "Automatic Scroll Limits"
        Me.cbxAutoScrollLimits.UseCompatibleTextRendering = True
        Me.cbxAutoScrollLimits.UseVisualStyleBackColor = True
        '
        'frmCompile
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(436, 429)
        Me.Controls.Add(Me.cbxAutoScrollLimits)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.Name = "frmCompile"
        Me.Text = "Compile Map"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents txtName As System.Windows.Forms.TextBox
    Public WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents Label2 As System.Windows.Forms.Label
    Public WithEvents txtMultiPlayers As System.Windows.Forms.TextBox
    Public WithEvents btnCompileMultiplayer As System.Windows.Forms.Button
    Public WithEvents Label3 As System.Windows.Forms.Label
    Public WithEvents txtCampTime As System.Windows.Forms.TextBox
    Public WithEvents Label5 As System.Windows.Forms.Label
    Public WithEvents txtScrollMaxX As System.Windows.Forms.TextBox
    Public WithEvents txtScrollMaxY As System.Windows.Forms.TextBox
    Public WithEvents txtScrollMinY As System.Windows.Forms.TextBox
    Public WithEvents Label10 As System.Windows.Forms.Label
    Public WithEvents txtScrollMinX As System.Windows.Forms.TextBox
    Public WithEvents Label11 As System.Windows.Forms.Label
    Public WithEvents Label12 As System.Windows.Forms.Label
    Public WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Public WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Public WithEvents txtAuthor As System.Windows.Forms.TextBox
    Public WithEvents Label4 As System.Windows.Forms.Label
    Public WithEvents Label13 As System.Windows.Forms.Label
    Public WithEvents cboLicense As System.Windows.Forms.ComboBox
    Public WithEvents cbxNewPlayerFormat As System.Windows.Forms.CheckBox
    Public WithEvents Label7 As System.Windows.Forms.Label
    Public WithEvents Label8 As System.Windows.Forms.Label
    Public WithEvents cboCampType As System.Windows.Forms.ComboBox
    Public WithEvents TabControl1 As System.Windows.Forms.TabControl
    Public WithEvents TabPage1 As System.Windows.Forms.TabPage
    Public WithEvents TabPage2 As System.Windows.Forms.TabPage
    Public WithEvents btnCompileCampaign As System.Windows.Forms.Button
    Public WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Public WithEvents cbxAutoScrollLimits As System.Windows.Forms.CheckBox
#End If
End Class