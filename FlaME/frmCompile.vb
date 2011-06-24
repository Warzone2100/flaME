Public Class frmCompile
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Public Sub New()
        InitializeComponent() 'required for monodevelop too

    End Sub

    Private Sub btnCompile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompileMultiplayer.Click
        Dim ReturnResult As New clsResult
        Dim A As Integer

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.Warning_Add(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If
        ReturnResult.Append(ValidateMap, "")

        Dim PlayerCount As Integer
        Dim MapName As String
        Dim IsBetaPlayerFormat As Boolean = cbxNewPlayerFormat.Checked
        Dim License As String = cboLicense.Text
        Dim B As Integer

        ReturnResult.Append(ValidateMap_UnitPositions, "")

        PlayerCount = CInt(Clamp(Val(txtMultiPlayers.Text), 0, CDbl(Integer.MaxValue)))

        If PlayerCount < 2 Or PlayerCount > 10 Then
            ReturnResult.Problem_Add("The number of players must be from 2 to 10.")
        End If
        If Not IsBetaPlayerFormat Then
            If Not (PlayerCount = 2 Or PlayerCount = 4 Or PlayerCount = 8) Then
                ReturnResult.Problem_Add("You must enable support for this number of players.")
            End If
        End If

        ReturnResult.Append(ValidateMap_Multiplayer(PlayerCount), "")

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
        WriteWZArgs.Multiplayer.IsBetaPlayerFormat = IsBetaPlayerFormat
        WriteWZArgs.Multiplayer.License = License
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer
        ReturnResult.Append(Main_Map.Write_WZ(WriteWZArgs), "")
        ShowWarnings(ReturnResult, "Compile Multiplayer")
        If Not ReturnResult.HasWarnings Then
            Hide()
        End If
    End Sub

    Private Sub frmCompile_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        e.Cancel = True
        Hide()
    End Sub

    Private Function ValidateMap_UnitPositions() As clsResult
        Dim Result As New clsResult

        'check unit positions

        Dim A As Integer
        Dim TileHasUnit(Main_Map.TerrainSize.X - 1, Main_Map.TerrainSize.Y - 1) As Boolean
        Dim TileStructureType(Main_Map.TerrainSize.X - 1, Main_Map.TerrainSize.Y - 1) As clsStructureType
        Dim TileFeatureType(Main_Map.TerrainSize.X - 1, Main_Map.TerrainSize.Y - 1) As clsFeatureType
        Dim X As Integer
        Dim Z As Integer
        Dim StartPos As sXY_int
        Dim FinishPos As sXY_int
        Dim CentrePos As sXY_int
        Dim StructureType As clsStructureType.enumStructureType
        Dim tmpStructure As clsStructureType
        Dim Footprint As sXY_int
        Dim UnitIsStructureModule(Main_Map.UnitCount - 1) As Boolean
        For A = 0 To Main_Map.UnitCount - 1
            If Main_Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(Main_Map.Units(A).Type, clsStructureType)
                StructureType = tmpStructure.StructureType
                UnitIsStructureModule(A) = (StructureType = clsStructureType.enumStructureType.FactoryModule _
                   Or StructureType = clsStructureType.enumStructureType.PowerModule _
                   Or StructureType = clsStructureType.enumStructureType.ResearchModule _
                   Or StructureType = clsStructureType.enumStructureType.ResourceExtractor)
            End If
        Next
        'check and store non-module units first. modules need to check for the underlying unit.
        For A = 0 To Main_Map.UnitCount - 1
            If Not UnitIsStructureModule(A) Then
                Footprint = Main_Map.Units(A).Type.GetFootprint
                Main_Map.GetFootprintTileRange(Main_Map.Units(A).Pos.Horizontal, Footprint, StartPos, FinishPos)
                If StartPos.X < 0 Or FinishPos.X >= Main_Map.TerrainSize.X _
                  Or StartPos.Y < 0 Or FinishPos.Y >= Main_Map.TerrainSize.Y Then
                    Result.Problem_Add("Unit off map at position " & Main_Map.Units(A).GetPosText & ".")
                Else
                    For Z = StartPos.Y To FinishPos.Y
                        For X = StartPos.X To FinishPos.X
                            If TileHasUnit(X, Z) Then
                                Result.Problem_Add("Bad unit overlap on tile " & X & ", " & Z & ".")
                            Else
                                TileHasUnit(X, Z) = True
                                If Main_Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                                    TileStructureType(X, Z) = CType(Main_Map.Units(A).Type, clsStructureType)
                                ElseIf Main_Map.Units(A).Type.Type = clsUnitType.enumType.Feature Then
                                    TileFeatureType(X, Z) = CType(Main_Map.Units(A).Type, clsFeatureType)
                                End If
                            End If
                        Next
                    Next
                End If
            End If
        Next
        'check modules and extractors
        For A = 0 To Main_Map.UnitCount - 1
            If UnitIsStructureModule(A) Then
                StructureType = CType(Main_Map.Units(A).Type, clsStructureType).StructureType
                CentrePos.X = Math.Floor(Main_Map.Units(A).Pos.Horizontal.X / TerrainGridSpacing)
                CentrePos.Y = Math.Floor(Main_Map.Units(A).Pos.Horizontal.Y / TerrainGridSpacing)
                If CentrePos.X < 0 Or CentrePos.X >= Main_Map.TerrainSize.X _
                  Or CentrePos.Y < 0 Or CentrePos.Y >= Main_Map.TerrainSize.Y Then
                    Result.Problem_Add("Module off map at position " & Main_Map.Units(A).GetPosText & ".")
                Else
                    If TileStructureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If StructureType = clsStructureType.enumStructureType.FactoryModule Then
                            If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Factory _
                                  Or TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.VTOLFactory Then

                            Else
                                Result.Problem_Add("Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                            End If
                        ElseIf StructureType = clsStructureType.enumStructureType.PowerModule Then
                            If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.PowerGenerator Then

                            Else
                                Result.Problem_Add("Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                            End If
                        ElseIf StructureType = clsStructureType.enumStructureType.ResearchModule Then
                            If TileStructureType(CentrePos.X, CentrePos.Y).StructureType = clsStructureType.enumStructureType.Research Then

                            Else
                                Result.Problem_Add("Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                            End If
                        End If
                    ElseIf TileFeatureType(CentrePos.X, CentrePos.Y) IsNot Nothing Then
                        If StructureType = clsStructureType.enumStructureType.ResourceExtractor Then
                            If TileFeatureType(CentrePos.X, CentrePos.Y).FeatureType = clsFeatureType.enumFeatureType.OilResource Then

                            Else
                                'allow derrick without resource
                                'Result.Problem_Add("Bad extractor on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                            End If
                        End If
                    Else
                        Result.Problem_Add("Bad module on tile " & CentrePos.X & ", " & CentrePos.Y & ".")
                    End If
                End If
            End If
        Next

        Return Result
    End Function

    Private Function ValidateMap_Multiplayer(ByVal PlayerCount As Integer) As clsResult
        Dim Result As New clsResult

        If PlayerCount < 2 Or PlayerCount > 10 Then
            Result.Problem_Add("Unable to evaluate for multiplayer due to bad number of players.")
            Return Result
        End If

        'check HQs, Trucks and unit counts

        Dim PlayerHQCount(PlayerCountMax - 1) As Integer
        Dim Player23TruckCount(PlayerCountMax - 1) As Integer
        Dim PlayerMasterTruckCount(PlayerCountMax - 1) As Integer
        Dim ScavPlayerNum As Integer
        Dim tmpDroid As clsDroidDesign
        Dim tmpStructure As clsStructureType
        Dim UnusedPlayerUnitWarningCount As Integer = 0
        Dim A As Integer

        ScavPlayerNum = Math.Max(PlayerCount, 7)

        For A = 0 To Main_Map.UnitCount - 1
            If Main_Map.Units(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                tmpDroid = CType(Main_Map.Units(A).Type, clsDroidDesign)
                If tmpDroid.Body IsNot Nothing And tmpDroid.Propulsion IsNot Nothing And tmpDroid.Turret1 IsNot Nothing And tmpDroid.TurretCount = 1 Then
                    If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Construct Then
                        PlayerMasterTruckCount(Main_Map.Units(A).PlayerNum) += 1
                        If tmpDroid.IsTemplate Then
                            Player23TruckCount(Main_Map.Units(A).PlayerNum) += 1
                        End If
                    End If
                End If
            ElseIf Main_Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(Main_Map.Units(A).Type, clsStructureType)
                If tmpStructure.Code = "A0CommandCentre" Then
                    PlayerHQCount(Main_Map.Units(A).PlayerNum) += 1
                End If
            End If
            If Main_Map.Units(A).Type.Type <> clsUnitType.enumType.Feature Then
                If Main_Map.Units(A).PlayerNum >= PlayerCount And Main_Map.Units(A).PlayerNum <> ScavPlayerNum Then
                    If UnusedPlayerUnitWarningCount < 32 Then
                        UnusedPlayerUnitWarningCount += 1
                        Result.Problem_Add("An unused player (" & Main_Map.Units(A).PlayerNum & ") has a unit at " & Main_Map.Units(A).GetPosText & ".")
                    End If
                End If
            End If
        Next

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
        ValidateMap = New clsResult

        If Main_Map.Tileset Is Nothing Then
            ValidateMap.Problem_Add("No tileset selected.")
        End If

        Dim A As Integer
        Dim B As Integer
        'Dim PlayerFactoryCount(FactionCountMax - 1) As Integer
        Dim PlayerStructureTypeCount(PlayerCountMax - 1, UnitTypeCount - 1) As Integer
        Dim tmpStructure As clsStructureType

        For A = 0 To Main_Map.UnitCount - 1
            If Main_Map.Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(Main_Map.Units(A).Type, clsStructureType)
                PlayerStructureTypeCount(Main_Map.Units(A).PlayerNum, tmpStructure.StructureNum) += 1
            End If
        Next

        For A = 0 To PlayerCountMax - 1
            For B = 0 To UnitTypeCount - 1
                If UnitTypes(B).Type = clsUnitType.enumType.PlayerStructure Then
                    If PlayerStructureTypeCount(A, B) > 255 Then
                        ValidateMap.Problem_Add("Player " & A & " has too many (" & PlayerStructureTypeCount(A, B) & ") of structure " & ControlChars.Quote & CType(UnitTypes(B), clsStructureType).Code & ControlChars.Quote & ". The limit is 255 of any one structure type.")
                    End If
                End If
            Next
        Next
ExitLoop:
    End Function

    Private Function ValidateMap_WaterTris() As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim Count As Integer

        If Main_Map.Tileset Is Nothing Then
            Return 0
        End If

        For Y = 0 To Main_Map.TerrainSize.Y - 1
            For X = 0 To Main_Map.TerrainSize.X - 1
                If Main_Map.TerrainTiles(X, Y).Tri Then
                    If Main_Map.TerrainTiles(X, Y).Texture.TextureNum >= 0 And Main_Map.TerrainTiles(X, Y).Texture.TextureNum < Main_Map.Tileset.TileCount Then
                        If Main_Map.Tileset.Tiles(Main_Map.TerrainTiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                            Count += 1
                        End If
                    End If
                End If
            Next
        Next
        Return Count
    End Function

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCompile))
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.rdoMulti = New System.Windows.Forms.RadioButton()
        Me.rdoCamp = New System.Windows.Forms.RadioButton()
        Me.txtMultiPlayers = New System.Windows.Forms.TextBox()
        Me.btnCompile = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCampTime = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtCampMaxX = New System.Windows.Forms.TextBox()
        Me.txtCampMaxY = New System.Windows.Forms.TextBox()
        Me.txtCampMinY = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtCampMinX = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.txtAuthor = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cmbLicense = New System.Windows.Forms.ComboBox()
        Me.chkNewPlayerFormat = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cmbCampType = New System.Windows.Forms.ComboBox()
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
        Me.Label1.Size = New System.Drawing.Size(49, 17)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(21, 95)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(59, 17)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Players:"
        '
        'rdoMulti
        '
        Me.rdoMulti.AutoSize = True
        Me.rdoMulti.Location = New System.Drawing.Point(16, 63)
        Me.rdoMulti.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoMulti.Name = "rdoMulti"
        Me.rdoMulti.Size = New System.Drawing.Size(97, 21)
        Me.rdoMulti.TabIndex = 3
        Me.rdoMulti.TabStop = True
        Me.rdoMulti.Text = "Multiplayer"
        Me.rdoMulti.UseVisualStyleBackColor = True
        '
        'rdoCamp
        '
        Me.rdoCamp.AutoSize = True
        Me.rdoCamp.Location = New System.Drawing.Point(16, 191)
        Me.rdoCamp.Margin = New System.Windows.Forms.Padding(4)
        Me.rdoCamp.Name = "rdoCamp"
        Me.rdoCamp.Size = New System.Drawing.Size(92, 21)
        Me.rdoCamp.TabIndex = 4
        Me.rdoCamp.TabStop = True
        Me.rdoCamp.Text = "Campaign"
        Me.rdoCamp.UseVisualStyleBackColor = True
        '
        'txtMultiPlayers
        '
        Me.txtMultiPlayers.Location = New System.Drawing.Point(84, 91)
        Me.txtMultiPlayers.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMultiPlayers.Name = "txtMultiPlayers"
        Me.txtMultiPlayers.Size = New System.Drawing.Size(68, 22)
        Me.txtMultiPlayers.TabIndex = 5
        '
        'btnCompile
        '
        Me.btnCompile.Enabled = False
        Me.btnCompile.Location = New System.Drawing.Point(255, 366)
        Me.btnCompile.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCompile.Name = "btnCompile"
        Me.btnCompile.Size = New System.Drawing.Size(128, 30)
        Me.btnCompile.TabIndex = 10
        Me.btnCompile.Text = "Compile"
        Me.btnCompile.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(32, 226)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(43, 17)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "Time:"
        '
        'txtCampTime
        '
        Me.txtCampTime.Location = New System.Drawing.Point(84, 223)
        Me.txtCampTime.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampTime.Name = "txtCampTime"
        Me.txtCampTime.Size = New System.Drawing.Size(68, 22)
        Me.txtCampTime.TabIndex = 11
        Me.txtCampTime.Text = "2"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(168, 226)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(44, 17)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Type:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(13, 270)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 17)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Scroll Limits:"
        '
        'txtCampMaxX
        '
        Me.txtCampMaxX.Location = New System.Drawing.Point(217, 298)
        Me.txtCampMaxX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMaxX.Name = "txtCampMaxX"
        Me.txtCampMaxX.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMaxX.TabIndex = 15
        Me.txtCampMaxX.Text = "maxX"
        '
        'txtCampMaxY
        '
        Me.txtCampMaxY.Location = New System.Drawing.Point(217, 329)
        Me.txtCampMaxY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMaxY.Name = "txtCampMaxY"
        Me.txtCampMaxY.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMaxY.TabIndex = 18
        Me.txtCampMaxY.Text = "maxY"
        '
        'txtCampMinY
        '
        Me.txtCampMinY.Location = New System.Drawing.Point(136, 329)
        Me.txtCampMinY.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMinY.Name = "txtCampMinY"
        Me.txtCampMinY.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMinY.TabIndex = 22
        Me.txtCampMinY.Text = "minY"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(108, 298)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(18, 17)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "x:"
        '
        'txtCampMinX
        '
        Me.txtCampMinX.Location = New System.Drawing.Point(136, 298)
        Me.txtCampMinX.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCampMinX.Name = "txtCampMinX"
        Me.txtCampMinX.Size = New System.Drawing.Size(61, 22)
        Me.txtCampMinX.TabIndex = 20
        Me.txtCampMinX.Text = "minX"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(108, 270)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(67, 17)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "Minimum:"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(193, 270)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(70, 17)
        Me.Label12.TabIndex = 25
        Me.Label12.Text = "Maximum:"
        '
        'txtAuthor
        '
        Me.txtAuthor.Location = New System.Drawing.Point(84, 123)
        Me.txtAuthor.Margin = New System.Windows.Forms.Padding(4)
        Me.txtAuthor.Name = "txtAuthor"
        Me.txtAuthor.Size = New System.Drawing.Size(123, 22)
        Me.txtAuthor.TabIndex = 27
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(25, 127)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(54, 17)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Author:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(17, 159)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(61, 17)
        Me.Label13.TabIndex = 28
        Me.Label13.Text = "License:"
        '
        'cmbLicense
        '
        Me.cmbLicense.FormattingEnabled = True
        Me.cmbLicense.Items.AddRange(New Object() {"GPL 2+", "CC BY 3.0 + GPL v2+", "CC BY-SA 3.0 + GPL v2+", "CC0"})
        Me.cmbLicense.Location = New System.Drawing.Point(84, 155)
        Me.cmbLicense.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbLicense.Name = "cmbLicense"
        Me.cmbLicense.Size = New System.Drawing.Size(172, 24)
        Me.cmbLicense.TabIndex = 29
        '
        'chkNewPlayerFormat
        '
        Me.chkNewPlayerFormat.Location = New System.Drawing.Point(161, 90)
        Me.chkNewPlayerFormat.Margin = New System.Windows.Forms.Padding(4)
        Me.chkNewPlayerFormat.Name = "chkNewPlayerFormat"
        Me.chkNewPlayerFormat.Size = New System.Drawing.Size(221, 25)
        Me.chkNewPlayerFormat.TabIndex = 30
        Me.chkNewPlayerFormat.Text = "X Player Support (Beta)"
        Me.chkNewPlayerFormat.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(108, 333)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(19, 17)
        Me.Label7.TabIndex = 31
        Me.Label7.Text = "y:"
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(265, 155)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(132, 41)
        Me.Label8.TabIndex = 32
        Me.Label8.Text = "Select from the list or type another."
        '
        'cmbCampType
        '
        Me.cmbCampType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCampType.FormattingEnabled = True
        Me.cmbCampType.Items.AddRange(New Object() {"Initial scenario state", "Scenario scroll area expansion", "Stand alone mission"})
        Me.cmbCampType.Location = New System.Drawing.Point(221, 223)
        Me.cmbCampType.Margin = New System.Windows.Forms.Padding(4)
        Me.cmbCampType.Name = "cmbCampType"
        Me.cmbCampType.Size = New System.Drawing.Size(160, 24)
        Me.cmbCampType.TabIndex = 33
        '
        'frmCompile
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(399, 402)
        Me.Controls.Add(Me.cmbCampType)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.chkNewPlayerFormat)
        Me.Controls.Add(Me.cmbLicense)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtAuthor)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.txtCampMinY)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtCampMinX)
        Me.Controls.Add(Me.txtCampMaxY)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.txtCampMaxX)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtCampTime)
        Me.Controls.Add(Me.btnCompile)
        Me.Controls.Add(Me.txtMultiPlayers)
        Me.Controls.Add(Me.rdoCamp)
        Me.Controls.Add(Me.rdoMulti)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        'Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmCompile"
        Me.Text = "Compile Map"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents rdoMulti As System.Windows.Forms.RadioButton
    Friend WithEvents rdoCamp As System.Windows.Forms.RadioButton
    Friend WithEvents txtMultiPlayers As System.Windows.Forms.TextBox
    Friend WithEvents btnCompile As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtCampTime As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtCampMaxX As System.Windows.Forms.TextBox
    Friend WithEvents txtCampMaxY As System.Windows.Forms.TextBox
    Friend WithEvents txtCampMinY As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtCampMinX As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents txtAuthor As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents cmbLicense As System.Windows.Forms.ComboBox
    Friend WithEvents chkNewPlayerFormat As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cmbCampType As System.Windows.Forms.ComboBox
#End If

    Private Sub btnCompileCampaign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCompileCampaign.Click
        Dim ReturnResult As New clsResult
        Dim A As Integer

        A = ValidateMap_WaterTris()
        If A > 0 Then
            ReturnResult.Warning_Add(A & " water tiles have an incorrect triangle direction. There might be in-game graphical glitches on those tiles.")
        End If
        ReturnResult.Append(ValidateMap, "")

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
        If FolderBrowserDialog.ShowDialog(Me) <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        Dim WriteWZArgs As New clsMap.sWrite_WZ_Args
        WriteWZArgs.MapName = MapName
        WriteWZArgs.Path = FolderBrowserDialog.SelectedPath
        WriteWZArgs.Overwrite = False
        SetScrollLimits(WriteWZArgs.ScrollMin, WriteWZArgs.ScrollMax)
        WriteWZArgs.Campaign = New clsMap.sWrite_WZ_Args.clsCampaign
        WriteWZArgs.Campaign.GAMTime = Clamp(Val(txtCampTime.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))
        WriteWZArgs.Campaign.GAMType = TypeNum
        WriteWZArgs.CompileType = clsMap.sWrite_WZ_Args.enumCompileType.Campaign
        ReturnResult.Append(Main_Map.Write_WZ(WriteWZArgs), "")
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

        If cbxAutoScrollLimits.Checked Then
            Min.X = 0
            Min.Y = 0
            Max.X = CUInt(Main_Map.TerrainSize.X)
            Max.Y = CUInt(Main_Map.TerrainSize.Y)
        Else
            Min.X = Clamp(Val(txtScrollMinX.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))
            Min.Y = Clamp(Val(txtScrollMinY.Text), CDbl(Integer.MinValue), CDbl(Integer.MaxValue))
            Max.X = Clamp(Val(txtScrollMaxX.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))
            Max.Y = Clamp(Val(txtScrollMaxY.Text), CDbl(UInteger.MinValue), CDbl(UInteger.MaxValue))
        End If
    End Sub
End Class