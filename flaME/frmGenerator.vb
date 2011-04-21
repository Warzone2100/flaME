Public Class frmGenerator
#If MonoDevelop <> 0.0# Then
    Inherits Form
#End If

    Private PlayerCount As Integer = 4
    Private StopTrying As Boolean

    Private Function ValidateTextbox(ByRef TextBoxToValidate As TextBox, ByVal Min As Double, ByVal Max As Double, ByVal Multiplier As Double) As Integer

        ValidateTextbox = CInt(Int(Clamp(Val(TextBoxToValidate.Text), Min, Max) * Multiplier))
        TextBoxToValidate.Text = CSng(ValidateTextbox / Multiplier)
    End Function

    Private Sub btnGenerate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenerate.Click

        If Not NewMapQuestion() Then
            Exit Sub
        End If

        btnGenerate.Enabled = False
        lblProgress.Text = "Generating..."
        Application.DoEvents()

        StopTrying = False

        Dim UnitsArgs As clsGeneratorMap.sGenerateUnitsArgs
        Dim LoopCount As Integer
        Dim LayoutArgs As New clsGeneratorMap.sGenerateLayoutArgs
        Dim NewMap As clsGeneratorMap
        Dim Result As sResult

        LayoutArgs.PlayerCount = PlayerCount

        Select Case cboSymmetry.SelectedIndex
            Case 0 'none
                LayoutArgs.SymmetryBlockCountXY.X = 1
                LayoutArgs.SymmetryBlockCountXY.Y = 1
                LayoutArgs.SymmetryBlockCount = 1
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                LayoutArgs.SymmetryIsRotational = False
            Case 1 'h rotation
                LayoutArgs.SymmetryBlockCountXY.X = 2
                LayoutArgs.SymmetryBlockCountXY.Y = 1
                LayoutArgs.SymmetryBlockCount = 2
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(True, True, False)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 0
                LayoutArgs.SymmetryIsRotational = True
            Case 2 'v rotation
                LayoutArgs.SymmetryBlockCountXY.X = 1
                LayoutArgs.SymmetryBlockCountXY.Y = 2
                LayoutArgs.SymmetryBlockCount = 2
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(0, 1)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(True, True, False)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 0
                LayoutArgs.SymmetryIsRotational = True
            Case 3 'h flip
                LayoutArgs.SymmetryBlockCountXY.X = 2
                LayoutArgs.SymmetryBlockCountXY.Y = 1
                LayoutArgs.SymmetryBlockCount = 2
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, False)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 0
                LayoutArgs.SymmetryIsRotational = False
            Case 4 'v flip
                LayoutArgs.SymmetryBlockCountXY.X = 1
                LayoutArgs.SymmetryBlockCountXY.Y = 2
                LayoutArgs.SymmetryBlockCount = 2
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(0, 1)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(False, True, False)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 0
                LayoutArgs.SymmetryIsRotational = False
            Case 5 '4x rotation
                LayoutArgs.SymmetryBlockCountXY.X = 2
                LayoutArgs.SymmetryBlockCountXY.Y = 2
                LayoutArgs.SymmetryBlockCount = 4
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(1) = 2
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, True)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 3
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(1) = 0
                LayoutArgs.SymmetryBlocks(2).XYNum = New sXY_int(0, 1)
                LayoutArgs.SymmetryBlocks(2).Orientation = New sTileOrientation(False, True, True)
                ReDim LayoutArgs.SymmetryBlocks(2).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(2).ReflectToNum(0) = 0
                LayoutArgs.SymmetryBlocks(2).ReflectToNum(1) = 3
                LayoutArgs.SymmetryBlocks(3).XYNum = New sXY_int(1, 1)
                LayoutArgs.SymmetryBlocks(3).Orientation = New sTileOrientation(True, True, False)
                ReDim LayoutArgs.SymmetryBlocks(3).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(3).ReflectToNum(0) = 2
                LayoutArgs.SymmetryBlocks(3).ReflectToNum(1) = 1
                LayoutArgs.SymmetryIsRotational = True
            Case 6 'hv flip
                LayoutArgs.SymmetryBlockCountXY.X = 2
                LayoutArgs.SymmetryBlockCountXY.Y = 2
                LayoutArgs.SymmetryBlockCount = 4
                ReDim LayoutArgs.SymmetryBlocks(LayoutArgs.SymmetryBlockCount - 1)
                LayoutArgs.SymmetryBlocks(0).XYNum = New sXY_int(0, 0)
                LayoutArgs.SymmetryBlocks(0).Orientation = New sTileOrientation(False, False, False)
                ReDim LayoutArgs.SymmetryBlocks(0).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(0) = 1
                LayoutArgs.SymmetryBlocks(0).ReflectToNum(1) = 2
                LayoutArgs.SymmetryBlocks(1).XYNum = New sXY_int(1, 0)
                LayoutArgs.SymmetryBlocks(1).Orientation = New sTileOrientation(True, False, False)
                ReDim LayoutArgs.SymmetryBlocks(1).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(0) = 0
                LayoutArgs.SymmetryBlocks(1).ReflectToNum(1) = 3
                LayoutArgs.SymmetryBlocks(2).XYNum = New sXY_int(0, 1)
                LayoutArgs.SymmetryBlocks(2).Orientation = New sTileOrientation(False, True, False)
                ReDim LayoutArgs.SymmetryBlocks(2).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(2).ReflectToNum(0) = 3
                LayoutArgs.SymmetryBlocks(2).ReflectToNum(1) = 0
                LayoutArgs.SymmetryBlocks(3).XYNum = New sXY_int(1, 1)
                LayoutArgs.SymmetryBlocks(3).Orientation = New sTileOrientation(True, True, False)
                ReDim LayoutArgs.SymmetryBlocks(3).ReflectToNum(LayoutArgs.SymmetryBlockCount / 2.0# - 1)
                LayoutArgs.SymmetryBlocks(3).ReflectToNum(0) = 2
                LayoutArgs.SymmetryBlocks(3).ReflectToNum(1) = 1
                LayoutArgs.SymmetryIsRotational = False
            Case Else
                MsgBox("Select symmetry")
                btnGenerate.Enabled = True
                Exit Sub
        End Select

        If LayoutArgs.PlayerCount * LayoutArgs.SymmetryBlockCount < 2 Then
            MsgBox("That configuration only produces 1 player.")
            btnGenerate.Enabled = True
            Exit Sub
        End If
        If LayoutArgs.PlayerCount * LayoutArgs.SymmetryBlockCount > 10 Then
            MsgBox("That configuration produces more than 10 players.")
            btnGenerate.Enabled = True
            Exit Sub
        End If

        LayoutArgs.Size.X = ValidateTextbox(txtWidth, 48.0#, 250.0#, 1.0#)
        LayoutArgs.Size.Y = ValidateTextbox(txtHeight, 48.0#, 250.0#, 1.0#)
        If LayoutArgs.SymmetryBlockCount = 4 Then
            If LayoutArgs.Size.X <> LayoutArgs.Size.Y Then
                MsgBox("Width and height must be equal if map is symmetrical on two axes.")
                btnGenerate.Enabled = True
                Exit Sub
            End If
        End If
        ReDim LayoutArgs.PlayerBasePos(LayoutArgs.PlayerCount - 1)
        Dim BaseMin As Double = 12.0#
        Dim BaseMax As sXY_dbl = New sXY_dbl(Math.Min(LayoutArgs.Size.X / LayoutArgs.SymmetryBlockCountXY.X, LayoutArgs.Size.X - 12.0#), Math.Min(LayoutArgs.Size.Y / LayoutArgs.SymmetryBlockCountXY.Y, LayoutArgs.Size.Y - 12.0#))
        LayoutArgs.PlayerBasePos(0) = New sXY_int(ValidateTextbox(txt1x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt1y, BaseMin, BaseMax.Y, TerrainGridSpacing))
        If LayoutArgs.PlayerCount >= 2 Then
            LayoutArgs.PlayerBasePos(1) = New sXY_int(ValidateTextbox(txt2x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt2y, BaseMin, BaseMax.Y, TerrainGridSpacing))
            If LayoutArgs.PlayerCount >= 3 Then
                LayoutArgs.PlayerBasePos(2) = New sXY_int(ValidateTextbox(txt3x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt3y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                If LayoutArgs.PlayerCount >= 4 Then
                    LayoutArgs.PlayerBasePos(3) = New sXY_int(ValidateTextbox(txt4x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt4y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                    If LayoutArgs.PlayerCount >= 5 Then
                        LayoutArgs.PlayerBasePos(4) = New sXY_int(ValidateTextbox(txt5x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt5y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                        If LayoutArgs.PlayerCount >= 6 Then
                            LayoutArgs.PlayerBasePos(5) = New sXY_int(ValidateTextbox(txt6x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt6y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                            If LayoutArgs.PlayerCount >= 7 Then
                                LayoutArgs.PlayerBasePos(6) = New sXY_int(ValidateTextbox(txt7x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt7y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                If LayoutArgs.PlayerCount >= 8 Then
                                    LayoutArgs.PlayerBasePos(7) = New sXY_int(ValidateTextbox(txt8x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt8y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                    If LayoutArgs.PlayerCount >= 9 Then
                                        LayoutArgs.PlayerBasePos(8) = New sXY_int(ValidateTextbox(txt9x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt9y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                        If LayoutArgs.PlayerCount >= 10 Then
                                            LayoutArgs.PlayerBasePos(9) = New sXY_int(ValidateTextbox(txt10x, BaseMin, BaseMax.X, TerrainGridSpacing), ValidateTextbox(txt10y, BaseMin, BaseMax.Y, TerrainGridSpacing))
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        LayoutArgs.LevelCount = ValidateTextbox(txtLevels, 3.0#, 5.0#, 1.0#)
        LayoutArgs.BaseLevel = ValidateTextbox(txtBaseLevel, -1.0#, CDbl(LayoutArgs.LevelCount - 1), 1.0#)
        LayoutArgs.JitterScale = 1
        LayoutArgs.MaxLevelTransition = 2
        LayoutArgs.PassageChance = ValidateTextbox(txtLevelFrequency, 0.0#, 100.0#, 1.0#)
        LayoutArgs.VariationChance = ValidateTextbox(txtVariation, 0.0#, 100.0#, 1.0#)
        LayoutArgs.FlatChance = ValidateTextbox(txtFlatness, 0.0#, 100.0#, 1.0#)
        LayoutArgs.RandomChance = ValidateTextbox(txtRandom, 0.0#, 100.0#, 1.0#)
        LayoutArgs.EqualizeChance = ValidateTextbox(txtEqualize, 0.0#, 100.0#, 1.0#)
        LayoutArgs.FlatBases = cbxFlatBases.Checked
        LayoutArgs.NodeScale = 4.0F
        LayoutArgs.MaxDisconnectionDist = ValidateTextbox(txtRampDistance, 0.0#, 99999.0#, TerrainGridSpacing)
        LayoutArgs.RampBase = ValidateTextbox(txtRampBase, 10.0#, 1000.0#, 10.0#) / 1000.0#
        LayoutArgs.BaseOilCount = ValidateTextbox(txtBaseOil, 0.0#, 16.0#, 1.0#)
        LayoutArgs.ExtraOilCount = ValidateTextbox(txtOilElsewhere, 0.0#, 9999.0#, 1.0#)
        LayoutArgs.ExtraOilClusterSizeMax = ValidateTextbox(txtOilClusterMax, 0.0#, 99.0#, 1.0#)
        LayoutArgs.ExtraOilClusterSizeMin = ValidateTextbox(txtOilClusterMin, 0.0#, CDbl(LayoutArgs.ExtraOilClusterSizeMax), 1.0#)
        LayoutArgs.OilDispersion = ValidateTextbox(txtOilDispersion, 0.0#, 9999.0#, 1.0#) / 100.0F
        LayoutArgs.OilAtATime = ValidateTextbox(txtOilAtATime, 1.0#, 2.0#, 1.0#)
        LayoutArgs.OilTolerance = 1.0F - ValidateTextbox(txtOilTolerance, 0.0#, 100.0#, 10.0#) / 1000.0F
        LayoutArgs.WaterSpawnQuantity = ValidateTextbox(txtWaterQuantity, 0.0#, 9999.0#, 1.0#)
        LayoutArgs.TotalWaterQuantity = ValidateTextbox(txtConnectedWater, 0.0#, 9999.0#, 1.0#)

        UnitsArgs.FeatureClusterChance = ValidateTextbox(txtFClusterChance, 0.0#, 100.0#, 1.0#) / 100.0F
        UnitsArgs.FeatureClusterMaxUnits = ValidateTextbox(txtFClusterMax, 0.0#, 99.0#, 1.0#)
        UnitsArgs.FeatureClusterMinUnits = ValidateTextbox(txtFClusterMin, 0.0#, UnitsArgs.FeatureClusterMaxUnits, 1.0#)
        UnitsArgs.FeatureScatterCount = ValidateTextbox(txtFScatterChance, 0.0#, 99999.0#, 1.0#)
        UnitsArgs.TruckCount = ValidateTextbox(txtTrucks, 0.0#, 15.0#, 1.0#)
        Application.DoEvents()
        LoopCount = 0
MakeNewMap:
        Do
            NewMap = New clsGeneratorMap(LayoutArgs.Size)
            Result = NewMap.GenerateLayout(LayoutArgs)
            If Result.Success Then
                Exit Do
            End If
            LoopCount += 1
            lblProgress.Text = "Attempt " & LoopCount & " failed; " & Result.Problem & " Retrying..."
            Application.DoEvents()
            NewMap.Deallocate()
            If StopTrying Then
                lblProgress.Text = "Aborted."
                btnGenerate.Enabled = True
                Exit Sub
            End If
        Loop

        'pbxMap.Image = NewMap.GetConnectionsBitmap
        'Application.DoEvents()

        NewMap.RandomizeHeights(LayoutArgs.LevelCount)

        If cbxMasterTexture.Checked Then
            Select Case cboTileset.SelectedIndex
                Case 0
                    NewMap.GenerateTileset = Generator_TilesetArizona
                    TerrainStyle_Arizona.Watermap = NewMap.GetWaterMap
                    TerrainStyle_Arizona.LevelCount = NewMap.LevelCount
                    NewMap.GenerateMasterTerrain(TerrainStyle_Arizona)
                    TerrainStyle_Arizona.Watermap = Nothing
                Case 1
                    NewMap.GenerateTileset = Generator_TilesetUrban
                    TerrainStyle_Urban.Watermap = NewMap.GetWaterMap
                    TerrainStyle_Urban.LevelCount = NewMap.LevelCount
                    NewMap.GenerateMasterTerrain(TerrainStyle_Urban)
                    TerrainStyle_Urban.Watermap = Nothing
                Case 2
                    NewMap.GenerateTileset = Generator_TilesetRockies
                    TerrainStyle_Rockies.Watermap = NewMap.GetWaterMap
                    TerrainStyle_Rockies.LevelCount = NewMap.LevelCount
                    NewMap.GenerateMasterTerrain(TerrainStyle_Rockies)
                    TerrainStyle_Rockies.Watermap = Nothing
                Case Else
                    MsgBox("Error; bad tileset selection.")
                    lblProgress.Text = "Failed."
                    btnGenerate.Enabled = True
                    Exit Sub
            End Select
            NewMap.TileType_Reset()
            NewMap.SetPainterToDefaults()
        Else
            Select Case cboTileset.SelectedIndex
                Case 0
                    NewMap.Tileset = Tileset_Arizona
                    NewMap.GenerateTileset = Generator_TilesetArizona
                Case 1
                    NewMap.Tileset = Tileset_Urban
                    NewMap.GenerateTileset = Generator_TilesetUrban
                Case 2
                    NewMap.Tileset = Tileset_Rockies
                    NewMap.GenerateTileset = Generator_TilesetRockies
                Case Else
                    MsgBox("Error; bad tileset selection.")
                    lblProgress.Text = "Failed."
                    btnGenerate.Enabled = True
                    Exit Sub
            End Select
            NewMap.TileType_Reset()
            NewMap.SetPainterToDefaults()
            Dim A As Integer
            Dim CliffAngle As Double = Math.Atan(255.0# * NewMap.HeightMultiplier / (2.0# * (NewMap.LevelCount - 1.0#) * TerrainGridSpacing)) - RadOf1Deg
            Dim tmpTiles As New sBrushTiles
            SquareTiles_Create(Math.Ceiling(Math.Max(NewMap.TerrainSize.X, NewMap.TerrainSize.Y)), tmpTiles, 1.0#)
            NewMap.Apply_Cliff(New sXY_int(CInt(Int(NewMap.TerrainSize.X / 2.0#)), CInt(Int(NewMap.TerrainSize.Y / 2.0#))), tmpTiles, CliffAngle, True)
            Dim RevertSlope() As Boolean
            Dim RevertHeight() As Boolean
            Dim WaterMap As New clsBooleanMap
            Dim bmTemp As New clsBooleanMap
            WaterMap = NewMap.GetWaterMap
            With NewMap.GenerateTileset
                ReDim RevertSlope(.OldTextureLayers.LayerCount - 1)
                ReDim RevertHeight(.OldTextureLayers.LayerCount - 1)
                For A = 0 To .OldTextureLayers.LayerCount - 1
                    With .OldTextureLayers.Layers(A)
                        .Terrainmap = NewMap.GenerateTerrainMap(.Scale, .Density)
                        If .SlopeMax < 0.0F Then
                            .SlopeMax = CliffAngle - RadOf1Deg
                            If .HeightMax < 0.0F Then
                                .HeightMax = 255.0F
                                bmTemp.Within(.Terrainmap, WaterMap)
                                .Terrainmap.ValueData = bmTemp.ValueData
                                bmTemp.ValueData = New clsBooleanMap.clsValueData
                                RevertHeight(A) = True
                            End If
                            RevertSlope(A) = True
                        End If
                    End With
                Next
                NewMap.MapTexturer(.OldTextureLayers)
                For A = 0 To .OldTextureLayers.LayerCount - 1
                    With .OldTextureLayers.Layers(A)
                        .Terrainmap = Nothing
                        If RevertSlope(A) Then
                            .SlopeMax = -1.0F
                        End If
                        If RevertHeight(A) Then
                            .HeightMax = -1.0F
                        End If
                    End With
                Next
            End With
        End If
        NewMap.TerrainBlockPaths()

        NewMap.LevelWater()

        Dim GatewaysArgs As clsGeneratorMap.sGenerateGatewaysArgs
        GatewaysArgs.LayoutArgs = LayoutArgs
        NewMap.GenerateGateways(GatewaysArgs)

        Result = NewMap.GenerateUnits(UnitsArgs)
        If Not Result.Success Then
            LoopCount += 1
            lblProgress.Text = "Attempt " & LoopCount & " failed; " & Result.Problem & " Retrying..."
            Application.DoEvents()
            NewMap.Deallocate()
            If StopTrying Then
                lblProgress.Text = "Aborted."
                btnGenerate.Enabled = True
                Exit Sub
            End If
            GoTo MakeNewMap
        End If

        NewMap.WaterTriCorrection()

        Map.Deallocate()
        Map = NewMap

        Map.Undo_Clear()
        Map.ShadowSector_CreateAll()

        lblProgress.Text = "Done."
        btnGenerate.Enabled = True

        frmMainInstance.SetBackgroundColour()

        Map.SectorAll_GL_Update()

        frmMainInstance.View.LookAtTile(CInt(Int(Map.TerrainSize.X / 2.0#)), CInt(Int(Map.TerrainSize.Y / 2.0#)))

        frmMainInstance.Resize_Update()
        frmMainInstance.HeightMultiplier_Update()
        frmMainInstance.cmbTileset_Refresh()
        frmMainInstance.PainterTerrains_Refresh(-1, -1)
        frmMainInstance.Selected_Object_Changed()

        frmMainInstance.TextureView.ScrollUpdate()
        frmMainInstance.TextureView.DrawViewLater()
        frmMainInstance.DrawView()
        frmMainInstance.Title_Text_Update()

        frmCompileInstance.txtCampMinX.Text = "0"
        frmCompileInstance.txtCampMinY.Text = "0"
        frmCompileInstance.txtCampMaxX.Text = Map.TerrainSize.X
        frmCompileInstance.txtCampMaxY.Text = Map.TerrainSize.Y

        frmCompileInstance.txtMultiPlayers.Text = NewMap.PlayerCount
        frmCompileInstance.txtName.Text = ""
        frmCompileInstance.cmbLicense.Text = ""
    End Sub

    Private Sub frmGenerator_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        Hide()
        e.Cancel = True
    End Sub

    Private Sub frmWZMapGen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cboTileset.SelectedIndex = 0
        cboSymmetry.SelectedIndex = 0

        SaveFileDialog.InitialDirectory = MyDocumentsPath
    End Sub

    Private Sub rdoPlayer2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer2.CheckedChanged

        If rdoPlayer2.Checked Then
            PlayerCount = 2
            rdoPlayer1.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer3.CheckedChanged

        If rdoPlayer3.Checked Then
            PlayerCount = 3
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer4.CheckedChanged

        If rdoPlayer4.Checked Then
            PlayerCount = 4
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer5.CheckedChanged

        If rdoPlayer5.Checked Then
            PlayerCount = 5
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer6.CheckedChanged

        If rdoPlayer6.Checked Then
            PlayerCount = 6
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer7.CheckedChanged

        If rdoPlayer7.Checked Then
            PlayerCount = 7
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer8.CheckedChanged

        If rdoPlayer8.Checked Then
            PlayerCount = 8
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click

        StopTrying = True
    End Sub

    Private Sub rdoPlayer1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer1.CheckedChanged

        If rdoPlayer1.Checked Then
            PlayerCount = 1
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer9.CheckedChanged

        If rdoPlayer9.Checked Then
            PlayerCount = 9
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer10.Checked = False
        End If
    End Sub

    Private Sub rdoPlayer10_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdoPlayer10.CheckedChanged

        If rdoPlayer10.Checked Then
            PlayerCount = 10
            rdoPlayer1.Checked = False
            rdoPlayer2.Checked = False
            rdoPlayer3.Checked = False
            rdoPlayer4.Checked = False
            rdoPlayer5.Checked = False
            rdoPlayer6.Checked = False
            rdoPlayer7.Checked = False
            rdoPlayer8.Checked = False
            rdoPlayer9.Checked = False
        End If
    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent() 'and monodevelop

        ' Add any initialization after the InitializeComponent() call.
    End Sub

#If MonoDevelop <> 0.0# Then
    Private Sub InitializeComponent()
        Me.btnGenerate = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtWidth = New System.Windows.Forms.TextBox()
        Me.txtHeight = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txt1x = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.rdoPlayer2 = New System.Windows.Forms.RadioButton()
        Me.txt1y = New System.Windows.Forms.TextBox()
        Me.txt2y = New System.Windows.Forms.TextBox()
        Me.txt2x = New System.Windows.Forms.TextBox()
        Me.txt3y = New System.Windows.Forms.TextBox()
        Me.txt3x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer3 = New System.Windows.Forms.RadioButton()
        Me.txt4y = New System.Windows.Forms.TextBox()
        Me.txt4x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer4 = New System.Windows.Forms.RadioButton()
        Me.txt5y = New System.Windows.Forms.TextBox()
        Me.txt5x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer5 = New System.Windows.Forms.RadioButton()
        Me.txt6y = New System.Windows.Forms.TextBox()
        Me.txt6x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer6 = New System.Windows.Forms.RadioButton()
        Me.txt7y = New System.Windows.Forms.TextBox()
        Me.txt7x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer7 = New System.Windows.Forms.RadioButton()
        Me.txt8y = New System.Windows.Forms.TextBox()
        Me.txt8x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer8 = New System.Windows.Forms.RadioButton()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtLevels = New System.Windows.Forms.TextBox()
        Me.txtLevelFrequency = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtRampDistance = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtBaseOil = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtOilElsewhere = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtOilClusterMin = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtOilClusterMax = New System.Windows.Forms.TextBox()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.txtBaseLevel = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.txtOilDispersion = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtFScatterChance = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.txtFClusterChance = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.txtFClusterMin = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.txtFClusterMax = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.txtTrucks = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.cboTileset = New System.Windows.Forms.ComboBox()
        Me.txtFlatness = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.cbxFlatBases = New System.Windows.Forms.CheckBox()
        Me.txtWaterQuantity = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.txtVariation = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.txtRandom = New System.Windows.Forms.TextBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.txtEqualize = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.rdoPlayer1 = New System.Windows.Forms.RadioButton()
        Me.cboSymmetry = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtOilAtATime = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.txtOilTolerance = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.txtRampBase = New System.Windows.Forms.TextBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.txtConnectedWater = New System.Windows.Forms.TextBox()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.txt9y = New System.Windows.Forms.TextBox()
        Me.txt9x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer9 = New System.Windows.Forms.RadioButton()
        Me.txt10y = New System.Windows.Forms.TextBox()
        Me.txt10x = New System.Windows.Forms.TextBox()
        Me.rdoPlayer10 = New System.Windows.Forms.RadioButton()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.cbxMasterTexture = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'btnGenerate
        '
        Me.btnGenerate.Location = New System.Drawing.Point(10, 489)
        Me.btnGenerate.Margin = New System.Windows.Forms.Padding(4)
        Me.btnGenerate.Name = "btnGenerate"
        Me.btnGenerate.Size = New System.Drawing.Size(144, 50)
        Me.btnGenerate.TabIndex = 50
        Me.btnGenerate.Text = "Generate"
        Me.btnGenerate.UseCompatibleTextRendering = True
        Me.btnGenerate.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(22, 49)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 20)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Width"
        Me.Label1.UseCompatibleTextRendering = True
        '
        'txtWidth
        '
        Me.txtWidth.Location = New System.Drawing.Point(72, 46)
        Me.txtWidth.Name = "txtWidth"
        Me.txtWidth.Size = New System.Drawing.Size(46, 22)
        Me.txtWidth.TabIndex = 1
        Me.txtWidth.Text = "128"
        '
        'txtHeight
        '
        Me.txtHeight.Location = New System.Drawing.Point(174, 46)
        Me.txtHeight.Name = "txtHeight"
        Me.txtHeight.Size = New System.Drawing.Size(46, 22)
        Me.txtHeight.TabIndex = 2
        Me.txtHeight.Text = "128"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(124, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 20)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Height"
        Me.Label2.UseCompatibleTextRendering = True
        '
        'txt1x
        '
        Me.txt1x.Location = New System.Drawing.Point(122, 102)
        Me.txt1x.Name = "txt1x"
        Me.txt1x.Size = New System.Drawing.Size(46, 22)
        Me.txt1x.TabIndex = 4
        Me.txt1x.Text = "0"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 82)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 20)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Base Positions"
        Me.Label3.UseCompatibleTextRendering = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(119, 81)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(12, 20)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "x"
        Me.Label4.UseCompatibleTextRendering = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(171, 81)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(12, 20)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "y"
        Me.Label5.UseCompatibleTextRendering = True
        '
        'rdoPlayer2
        '
        Me.rdoPlayer2.AutoSize = True
        Me.rdoPlayer2.Location = New System.Drawing.Point(27, 131)
        Me.rdoPlayer2.Name = "rdoPlayer2"
        Me.rdoPlayer2.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer2.TabIndex = 54
        Me.rdoPlayer2.Text = "Player 2"
        Me.rdoPlayer2.UseCompatibleTextRendering = True
        Me.rdoPlayer2.UseVisualStyleBackColor = True
        '
        'txt1y
        '
        Me.txt1y.Location = New System.Drawing.Point(174, 101)
        Me.txt1y.Name = "txt1y"
        Me.txt1y.Size = New System.Drawing.Size(46, 22)
        Me.txt1y.TabIndex = 5
        Me.txt1y.Text = "0"
        '
        'txt2y
        '
        Me.txt2y.Location = New System.Drawing.Point(174, 129)
        Me.txt2y.Name = "txt2y"
        Me.txt2y.Size = New System.Drawing.Size(46, 22)
        Me.txt2y.TabIndex = 8
        Me.txt2y.Text = "999"
        '
        'txt2x
        '
        Me.txt2x.Location = New System.Drawing.Point(122, 130)
        Me.txt2x.Name = "txt2x"
        Me.txt2x.Size = New System.Drawing.Size(46, 22)
        Me.txt2x.TabIndex = 7
        Me.txt2x.Text = "999"
        '
        'txt3y
        '
        Me.txt3y.Location = New System.Drawing.Point(174, 157)
        Me.txt3y.Name = "txt3y"
        Me.txt3y.Size = New System.Drawing.Size(46, 22)
        Me.txt3y.TabIndex = 11
        Me.txt3y.Text = "0"
        '
        'txt3x
        '
        Me.txt3x.Location = New System.Drawing.Point(122, 158)
        Me.txt3x.Name = "txt3x"
        Me.txt3x.Size = New System.Drawing.Size(46, 22)
        Me.txt3x.TabIndex = 10
        Me.txt3x.Text = "999"
        '
        'rdoPlayer3
        '
        Me.rdoPlayer3.AutoSize = True
        Me.rdoPlayer3.Location = New System.Drawing.Point(27, 159)
        Me.rdoPlayer3.Name = "rdoPlayer3"
        Me.rdoPlayer3.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer3.TabIndex = 55
        Me.rdoPlayer3.Text = "Player 3"
        Me.rdoPlayer3.UseCompatibleTextRendering = True
        Me.rdoPlayer3.UseVisualStyleBackColor = True
        '
        'txt4y
        '
        Me.txt4y.Location = New System.Drawing.Point(174, 185)
        Me.txt4y.Name = "txt4y"
        Me.txt4y.Size = New System.Drawing.Size(46, 22)
        Me.txt4y.TabIndex = 14
        Me.txt4y.Text = "999"
        '
        'txt4x
        '
        Me.txt4x.Location = New System.Drawing.Point(122, 186)
        Me.txt4x.Name = "txt4x"
        Me.txt4x.Size = New System.Drawing.Size(46, 22)
        Me.txt4x.TabIndex = 13
        Me.txt4x.Text = "0"
        '
        'rdoPlayer4
        '
        Me.rdoPlayer4.AutoSize = True
        Me.rdoPlayer4.Checked = True
        Me.rdoPlayer4.Location = New System.Drawing.Point(27, 187)
        Me.rdoPlayer4.Name = "rdoPlayer4"
        Me.rdoPlayer4.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer4.TabIndex = 56
        Me.rdoPlayer4.TabStop = True
        Me.rdoPlayer4.Text = "Player 4"
        Me.rdoPlayer4.UseCompatibleTextRendering = True
        Me.rdoPlayer4.UseVisualStyleBackColor = True
        '
        'txt5y
        '
        Me.txt5y.Location = New System.Drawing.Point(174, 213)
        Me.txt5y.Name = "txt5y"
        Me.txt5y.Size = New System.Drawing.Size(46, 22)
        Me.txt5y.TabIndex = 17
        Me.txt5y.Text = "y"
        '
        'txt5x
        '
        Me.txt5x.Location = New System.Drawing.Point(122, 214)
        Me.txt5x.Name = "txt5x"
        Me.txt5x.Size = New System.Drawing.Size(46, 22)
        Me.txt5x.TabIndex = 16
        Me.txt5x.Text = "x"
        '
        'rdoPlayer5
        '
        Me.rdoPlayer5.AutoSize = True
        Me.rdoPlayer5.Location = New System.Drawing.Point(27, 215)
        Me.rdoPlayer5.Name = "rdoPlayer5"
        Me.rdoPlayer5.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer5.TabIndex = 57
        Me.rdoPlayer5.Text = "Player 5"
        Me.rdoPlayer5.UseCompatibleTextRendering = True
        Me.rdoPlayer5.UseVisualStyleBackColor = True
        '
        'txt6y
        '
        Me.txt6y.Location = New System.Drawing.Point(174, 241)
        Me.txt6y.Name = "txt6y"
        Me.txt6y.Size = New System.Drawing.Size(46, 22)
        Me.txt6y.TabIndex = 20
        Me.txt6y.Text = "y"
        '
        'txt6x
        '
        Me.txt6x.Location = New System.Drawing.Point(122, 242)
        Me.txt6x.Name = "txt6x"
        Me.txt6x.Size = New System.Drawing.Size(46, 22)
        Me.txt6x.TabIndex = 19
        Me.txt6x.Text = "x"
        '
        'rdoPlayer6
        '
        Me.rdoPlayer6.AutoSize = True
        Me.rdoPlayer6.Location = New System.Drawing.Point(27, 243)
        Me.rdoPlayer6.Name = "rdoPlayer6"
        Me.rdoPlayer6.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer6.TabIndex = 58
        Me.rdoPlayer6.Text = "Player 6"
        Me.rdoPlayer6.UseCompatibleTextRendering = True
        Me.rdoPlayer6.UseVisualStyleBackColor = True
        '
        'txt7y
        '
        Me.txt7y.Location = New System.Drawing.Point(174, 269)
        Me.txt7y.Name = "txt7y"
        Me.txt7y.Size = New System.Drawing.Size(46, 22)
        Me.txt7y.TabIndex = 23
        Me.txt7y.Text = "y"
        '
        'txt7x
        '
        Me.txt7x.Location = New System.Drawing.Point(122, 270)
        Me.txt7x.Name = "txt7x"
        Me.txt7x.Size = New System.Drawing.Size(46, 22)
        Me.txt7x.TabIndex = 22
        Me.txt7x.Text = "x"
        '
        'rdoPlayer7
        '
        Me.rdoPlayer7.AutoSize = True
        Me.rdoPlayer7.Location = New System.Drawing.Point(27, 271)
        Me.rdoPlayer7.Name = "rdoPlayer7"
        Me.rdoPlayer7.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer7.TabIndex = 59
        Me.rdoPlayer7.Text = "Player 7"
        Me.rdoPlayer7.UseCompatibleTextRendering = True
        Me.rdoPlayer7.UseVisualStyleBackColor = True
        '
        'txt8y
        '
        Me.txt8y.Location = New System.Drawing.Point(174, 297)
        Me.txt8y.Name = "txt8y"
        Me.txt8y.Size = New System.Drawing.Size(46, 22)
        Me.txt8y.TabIndex = 26
        Me.txt8y.Text = "y"
        '
        'txt8x
        '
        Me.txt8x.Location = New System.Drawing.Point(122, 298)
        Me.txt8x.Name = "txt8x"
        Me.txt8x.Size = New System.Drawing.Size(46, 22)
        Me.txt8x.TabIndex = 25
        Me.txt8x.Text = "x"
        '
        'rdoPlayer8
        '
        Me.rdoPlayer8.AutoSize = True
        Me.rdoPlayer8.Location = New System.Drawing.Point(27, 299)
        Me.rdoPlayer8.Name = "rdoPlayer8"
        Me.rdoPlayer8.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer8.TabIndex = 60
        Me.rdoPlayer8.Text = "Player 8"
        Me.rdoPlayer8.UseCompatibleTextRendering = True
        Me.rdoPlayer8.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(47, 428)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 20)
        Me.Label7.TabIndex = 37
        Me.Label7.Text = "Height Levels"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label7.UseCompatibleTextRendering = True
        '
        'txtLevels
        '
        Me.txtLevels.Location = New System.Drawing.Point(139, 429)
        Me.txtLevels.Name = "txtLevels"
        Me.txtLevels.Size = New System.Drawing.Size(46, 22)
        Me.txtLevels.TabIndex = 28
        Me.txtLevels.Text = "4"
        '
        'txtLevelFrequency
        '
        Me.txtLevelFrequency.Location = New System.Drawing.Point(357, 304)
        Me.txtLevelFrequency.Name = "txtLevelFrequency"
        Me.txtLevelFrequency.Size = New System.Drawing.Size(46, 22)
        Me.txtLevelFrequency.TabIndex = 31
        Me.txtLevelFrequency.Text = "3"
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(287, 303)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(64, 20)
        Me.Label8.TabIndex = 39
        Me.Label8.Text = "Passages"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label8.UseCompatibleTextRendering = True
        '
        'txtRampDistance
        '
        Me.txtRampDistance.Location = New System.Drawing.Point(579, 360)
        Me.txtRampDistance.Name = "txtRampDistance"
        Me.txtRampDistance.Size = New System.Drawing.Size(46, 22)
        Me.txtRampDistance.TabIndex = 36
        Me.txtRampDistance.Text = "80"
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(468, 334)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(147, 20)
        Me.Label9.TabIndex = 41
        Me.Label9.Text = "Ramp Distance At Base"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label9.UseCompatibleTextRendering = True
        '
        'txtBaseOil
        '
        Me.txtBaseOil.Location = New System.Drawing.Point(393, 49)
        Me.txtBaseOil.Name = "txtBaseOil"
        Me.txtBaseOil.Size = New System.Drawing.Size(46, 22)
        Me.txtBaseOil.TabIndex = 38
        Me.txtBaseOil.Text = "4"
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(317, 48)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(70, 20)
        Me.Label10.TabIndex = 43
        Me.Label10.Text = "Oil In Base"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label10.UseCompatibleTextRendering = True
        '
        'txtOilElsewhere
        '
        Me.txtOilElsewhere.Location = New System.Drawing.Point(393, 77)
        Me.txtOilElsewhere.Name = "txtOilElsewhere"
        Me.txtOilElsewhere.Size = New System.Drawing.Size(46, 22)
        Me.txtOilElsewhere.TabIndex = 39
        Me.txtOilElsewhere.Text = "40"
        '
        'Label11
        '
        Me.Label11.Location = New System.Drawing.Point(300, 76)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(87, 20)
        Me.Label11.TabIndex = 45
        Me.Label11.Text = "Oil Elsewhere"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label11.UseCompatibleTextRendering = True
        '
        'txtOilClusterMin
        '
        Me.txtOilClusterMin.Location = New System.Drawing.Point(393, 105)
        Me.txtOilClusterMin.Name = "txtOilClusterMin"
        Me.txtOilClusterMin.Size = New System.Drawing.Size(22, 22)
        Me.txtOilClusterMin.TabIndex = 40
        Me.txtOilClusterMin.Text = "1"
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(242, 104)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(97, 20)
        Me.Label12.TabIndex = 47
        Me.Label12.Text = "Oil Cluster Size"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label12.UseCompatibleTextRendering = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(357, 108)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(26, 20)
        Me.Label13.TabIndex = 49
        Me.Label13.Text = "Min"
        Me.Label13.UseCompatibleTextRendering = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(430, 108)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(33, 17)
        Me.Label14.TabIndex = 51
        Me.Label14.Text = "Max"
        '
        'txtOilClusterMax
        '
        Me.txtOilClusterMax.Location = New System.Drawing.Point(466, 105)
        Me.txtOilClusterMax.Name = "txtOilClusterMax"
        Me.txtOilClusterMax.Size = New System.Drawing.Size(22, 22)
        Me.txtOilClusterMax.TabIndex = 41
        Me.txtOilClusterMax.Text = "1"
        '
        'lblProgress
        '
        Me.lblProgress.Location = New System.Drawing.Point(237, 500)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(590, 39)
        Me.lblProgress.TabIndex = 56
        Me.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(162, 489)
        Me.btnStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(68, 50)
        Me.btnStop.TabIndex = 51
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseCompatibleTextRendering = True
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'txtBaseLevel
        '
        Me.txtBaseLevel.Location = New System.Drawing.Point(139, 455)
        Me.txtBaseLevel.Name = "txtBaseLevel"
        Me.txtBaseLevel.Size = New System.Drawing.Size(46, 22)
        Me.txtBaseLevel.TabIndex = 29
        Me.txtBaseLevel.Text = "-1"
        '
        'Label17
        '
        Me.Label17.Location = New System.Drawing.Point(20, 454)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(113, 20)
        Me.Label17.TabIndex = 58
        Me.Label17.Text = "Base Height Level"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label17.UseCompatibleTextRendering = True
        '
        'txtOilDispersion
        '
        Me.txtOilDispersion.Location = New System.Drawing.Point(393, 133)
        Me.txtOilDispersion.Name = "txtOilDispersion"
        Me.txtOilDispersion.Size = New System.Drawing.Size(46, 22)
        Me.txtOilDispersion.TabIndex = 42
        Me.txtOilDispersion.Text = "100"
        '
        'Label18
        '
        Me.Label18.Location = New System.Drawing.Point(299, 132)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(88, 20)
        Me.Label18.TabIndex = 60
        Me.Label18.Text = "Oil Dispersion"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label18.UseCompatibleTextRendering = True
        '
        'txtFScatterChance
        '
        Me.txtFScatterChance.Location = New System.Drawing.Point(664, 49)
        Me.txtFScatterChance.Name = "txtFScatterChance"
        Me.txtFScatterChance.Size = New System.Drawing.Size(46, 22)
        Me.txtFScatterChance.TabIndex = 43
        Me.txtFScatterChance.Text = "0"
        '
        'Label19
        '
        Me.Label19.Location = New System.Drawing.Point(539, 48)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(119, 20)
        Me.Label19.TabIndex = 62
        Me.Label19.Text = "Scattered Features"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label19.UseCompatibleTextRendering = True
        '
        'txtFClusterChance
        '
        Me.txtFClusterChance.Location = New System.Drawing.Point(664, 77)
        Me.txtFClusterChance.Name = "txtFClusterChance"
        Me.txtFClusterChance.Size = New System.Drawing.Size(46, 22)
        Me.txtFClusterChance.TabIndex = 44
        Me.txtFClusterChance.Text = "0"
        '
        'Label20
        '
        Me.Label20.Location = New System.Drawing.Point(496, 76)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(162, 20)
        Me.Label20.TabIndex = 64
        Me.Label20.Text = "Feature Cluster Chance %"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label20.UseCompatibleTextRendering = True
        '
        'txtFClusterMin
        '
        Me.txtFClusterMin.Location = New System.Drawing.Point(664, 129)
        Me.txtFClusterMin.Name = "txtFClusterMin"
        Me.txtFClusterMin.Size = New System.Drawing.Size(46, 22)
        Me.txtFClusterMin.TabIndex = 45
        Me.txtFClusterMin.Text = "2"
        '
        'Label21
        '
        Me.Label21.Location = New System.Drawing.Point(559, 106)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(127, 20)
        Me.Label21.TabIndex = 66
        Me.Label21.Text = "Feature Cluster Size"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label21.UseCompatibleTextRendering = True
        '
        'txtFClusterMax
        '
        Me.txtFClusterMax.Location = New System.Drawing.Point(664, 157)
        Me.txtFClusterMax.Name = "txtFClusterMax"
        Me.txtFClusterMax.Size = New System.Drawing.Size(46, 22)
        Me.txtFClusterMax.TabIndex = 46
        Me.txtFClusterMax.Text = "5"
        '
        'Label22
        '
        Me.Label22.Location = New System.Drawing.Point(632, 128)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(26, 20)
        Me.Label22.TabIndex = 68
        Me.Label22.Text = "Min"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label22.UseCompatibleTextRendering = True
        '
        'txtTrucks
        '
        Me.txtTrucks.Location = New System.Drawing.Point(664, 197)
        Me.txtTrucks.Name = "txtTrucks"
        Me.txtTrucks.Size = New System.Drawing.Size(46, 22)
        Me.txtTrucks.TabIndex = 47
        Me.txtTrucks.Text = "2"
        '
        'Label23
        '
        Me.Label23.Location = New System.Drawing.Point(579, 196)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(79, 20)
        Me.Label23.TabIndex = 70
        Me.Label23.Text = "Base Trucks"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label23.UseCompatibleTextRendering = True
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(21, 390)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(44, 20)
        Me.Label24.TabIndex = 72
        Me.Label24.Text = "Tileset"
        Me.Label24.UseCompatibleTextRendering = True
        '
        'cboTileset
        '
        Me.cboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTileset.FormattingEnabled = True
        Me.cboTileset.Items.AddRange(New Object() {"Arizona", "Urban", "Rockies"})
        Me.cboTileset.Location = New System.Drawing.Point(77, 387)
        Me.cboTileset.Name = "cboTileset"
        Me.cboTileset.Size = New System.Drawing.Size(149, 24)
        Me.cboTileset.TabIndex = 27
        '
        'txtFlatness
        '
        Me.txtFlatness.Location = New System.Drawing.Point(357, 276)
        Me.txtFlatness.Name = "txtFlatness"
        Me.txtFlatness.Size = New System.Drawing.Size(46, 22)
        Me.txtFlatness.TabIndex = 30
        Me.txtFlatness.Text = "0"
        '
        'Label25
        '
        Me.Label25.Location = New System.Drawing.Point(295, 275)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(56, 20)
        Me.Label25.TabIndex = 74
        Me.Label25.Text = "Flatness"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label25.UseCompatibleTextRendering = True
        '
        'cbxFlatBases
        '
        Me.cbxFlatBases.AutoSize = True
        Me.cbxFlatBases.Checked = True
        Me.cbxFlatBases.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbxFlatBases.Location = New System.Drawing.Point(357, 416)
        Me.cbxFlatBases.Name = "cbxFlatBases"
        Me.cbxFlatBases.Size = New System.Drawing.Size(109, 21)
        Me.cbxFlatBases.TabIndex = 37
        Me.cbxFlatBases.Text = "Flatten Bases"
        Me.cbxFlatBases.UseCompatibleTextRendering = True
        Me.cbxFlatBases.UseVisualStyleBackColor = True
        '
        'txtWaterQuantity
        '
        Me.txtWaterQuantity.Location = New System.Drawing.Point(579, 278)
        Me.txtWaterQuantity.Name = "txtWaterQuantity"
        Me.txtWaterQuantity.Size = New System.Drawing.Size(46, 22)
        Me.txtWaterQuantity.TabIndex = 35
        Me.txtWaterQuantity.Text = "0"
        '
        'Label26
        '
        Me.Label26.Location = New System.Drawing.Point(481, 277)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(92, 20)
        Me.Label26.TabIndex = 78
        Me.Label26.Text = "Water Spawns"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label26.UseCompatibleTextRendering = True
        '
        'Label27
        '
        Me.Label27.Location = New System.Drawing.Point(628, 156)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(30, 20)
        Me.Label27.TabIndex = 80
        Me.Label27.Text = "Max"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label27.UseCompatibleTextRendering = True
        '
        'txtVariation
        '
        Me.txtVariation.Location = New System.Drawing.Point(357, 332)
        Me.txtVariation.Name = "txtVariation"
        Me.txtVariation.Size = New System.Drawing.Size(46, 22)
        Me.txtVariation.TabIndex = 32
        Me.txtVariation.Text = "2"
        '
        'Label28
        '
        Me.Label28.Location = New System.Drawing.Point(293, 332)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(58, 20)
        Me.Label28.TabIndex = 81
        Me.Label28.Text = "Variation"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label28.UseCompatibleTextRendering = True
        '
        'txtRandom
        '
        Me.txtRandom.Location = New System.Drawing.Point(357, 360)
        Me.txtRandom.Name = "txtRandom"
        Me.txtRandom.Size = New System.Drawing.Size(46, 22)
        Me.txtRandom.TabIndex = 33
        Me.txtRandom.Text = "0"
        '
        'Label29
        '
        Me.Label29.Location = New System.Drawing.Point(296, 359)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(55, 20)
        Me.Label29.TabIndex = 83
        Me.Label29.Text = "Random"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label29.UseCompatibleTextRendering = True
        '
        'txtEqualize
        '
        Me.txtEqualize.Location = New System.Drawing.Point(357, 388)
        Me.txtEqualize.Name = "txtEqualize"
        Me.txtEqualize.Size = New System.Drawing.Size(46, 22)
        Me.txtEqualize.TabIndex = 34
        Me.txtEqualize.Text = "0"
        '
        'Label30
        '
        Me.Label30.Location = New System.Drawing.Point(252, 388)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(99, 20)
        Me.Label30.TabIndex = 85
        Me.Label30.Text = "Equalize Levels"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label30.UseCompatibleTextRendering = True
        '
        'rdoPlayer1
        '
        Me.rdoPlayer1.AutoSize = True
        Me.rdoPlayer1.Location = New System.Drawing.Point(27, 105)
        Me.rdoPlayer1.Name = "rdoPlayer1"
        Me.rdoPlayer1.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer1.TabIndex = 53
        Me.rdoPlayer1.Text = "Player 1"
        Me.rdoPlayer1.UseCompatibleTextRendering = True
        Me.rdoPlayer1.UseVisualStyleBackColor = True
        '
        'cboSymmetry
        '
        Me.cboSymmetry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSymmetry.FormattingEnabled = True
        Me.cboSymmetry.Items.AddRange(New Object() {"None", "Horizontal Rotation", "Vertical Rotation", "Horizontal Flip", "Vertical Flip", "Quarters Rotation", "Quarters Flip"})
        Me.cboSymmetry.Location = New System.Drawing.Point(84, 12)
        Me.cboSymmetry.Name = "cboSymmetry"
        Me.cboSymmetry.Size = New System.Drawing.Size(149, 24)
        Me.cboSymmetry.TabIndex = 0
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(8, 15)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 20)
        Me.Label6.TabIndex = 88
        Me.Label6.Text = "Symmetry"
        Me.Label6.UseCompatibleTextRendering = True
        '
        'txtOilAtATime
        '
        Me.txtOilAtATime.Location = New System.Drawing.Point(393, 161)
        Me.txtOilAtATime.Name = "txtOilAtATime"
        Me.txtOilAtATime.Size = New System.Drawing.Size(46, 22)
        Me.txtOilAtATime.TabIndex = 89
        Me.txtOilAtATime.Text = "1"
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(251, 160)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(136, 20)
        Me.Label31.TabIndex = 90
        Me.Label31.Text = "Oil Clusters At A Time"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label31.UseCompatibleTextRendering = True
        '
        'txtOilTolerance
        '
        Me.txtOilTolerance.Location = New System.Drawing.Point(393, 189)
        Me.txtOilTolerance.Name = "txtOilTolerance"
        Me.txtOilTolerance.Size = New System.Drawing.Size(46, 22)
        Me.txtOilTolerance.TabIndex = 91
        Me.txtOilTolerance.Text = "0"
        '
        'Label32
        '
        Me.Label32.Location = New System.Drawing.Point(268, 188)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(119, 20)
        Me.Label32.TabIndex = 92
        Me.Label32.Text = "Oil Randomness %"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label32.UseCompatibleTextRendering = True
        '
        'txtRampBase
        '
        Me.txtRampBase.Location = New System.Drawing.Point(579, 415)
        Me.txtRampBase.Name = "txtRampBase"
        Me.txtRampBase.Size = New System.Drawing.Size(46, 22)
        Me.txtRampBase.TabIndex = 93
        Me.txtRampBase.Text = "100"
        '
        'Label33
        '
        Me.Label33.Location = New System.Drawing.Point(473, 390)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(149, 20)
        Me.Label33.TabIndex = 94
        Me.Label33.Text = "Ramp Multiplier % Per 8"
        Me.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label33.UseCompatibleTextRendering = True
        '
        'txtConnectedWater
        '
        Me.txtConnectedWater.Location = New System.Drawing.Point(579, 306)
        Me.txtConnectedWater.Name = "txtConnectedWater"
        Me.txtConnectedWater.Size = New System.Drawing.Size(46, 22)
        Me.txtConnectedWater.TabIndex = 95
        Me.txtConnectedWater.Text = "0"
        '
        'Label34
        '
        Me.Label34.Location = New System.Drawing.Point(499, 305)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(74, 20)
        Me.Label34.TabIndex = 96
        Me.Label34.Text = "Total Water"
        Me.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Label34.UseCompatibleTextRendering = True
        '
        'txt9y
        '
        Me.txt9y.Location = New System.Drawing.Point(174, 325)
        Me.txt9y.Name = "txt9y"
        Me.txt9y.Size = New System.Drawing.Size(46, 22)
        Me.txt9y.TabIndex = 98
        Me.txt9y.Text = "y"
        '
        'txt9x
        '
        Me.txt9x.Location = New System.Drawing.Point(122, 326)
        Me.txt9x.Name = "txt9x"
        Me.txt9x.Size = New System.Drawing.Size(46, 22)
        Me.txt9x.TabIndex = 97
        Me.txt9x.Text = "x"
        '
        'rdoPlayer9
        '
        Me.rdoPlayer9.AutoSize = True
        Me.rdoPlayer9.Location = New System.Drawing.Point(27, 327)
        Me.rdoPlayer9.Name = "rdoPlayer9"
        Me.rdoPlayer9.Size = New System.Drawing.Size(75, 21)
        Me.rdoPlayer9.TabIndex = 99
        Me.rdoPlayer9.Text = "Player 9"
        Me.rdoPlayer9.UseCompatibleTextRendering = True
        Me.rdoPlayer9.UseVisualStyleBackColor = True
        '
        'txt10y
        '
        Me.txt10y.Location = New System.Drawing.Point(174, 353)
        Me.txt10y.Name = "txt10y"
        Me.txt10y.Size = New System.Drawing.Size(46, 22)
        Me.txt10y.TabIndex = 101
        Me.txt10y.Text = "y"
        '
        'txt10x
        '
        Me.txt10x.Location = New System.Drawing.Point(122, 354)
        Me.txt10x.Name = "txt10x"
        Me.txt10x.Size = New System.Drawing.Size(46, 22)
        Me.txt10x.TabIndex = 100
        Me.txt10x.Text = "x"
        '
        'rdoPlayer10
        '
        Me.rdoPlayer10.AutoSize = True
        Me.rdoPlayer10.Location = New System.Drawing.Point(27, 355)
        Me.rdoPlayer10.Name = "rdoPlayer10"
        Me.rdoPlayer10.Size = New System.Drawing.Size(82, 21)
        Me.rdoPlayer10.TabIndex = 102
        Me.rdoPlayer10.Text = "Player 10"
        Me.rdoPlayer10.UseCompatibleTextRendering = True
        Me.rdoPlayer10.UseVisualStyleBackColor = True
        '
        'cbxMasterTexture
        '
        Me.cbxMasterTexture.AutoSize = True
        Me.cbxMasterTexture.Location = New System.Drawing.Point(499, 457)
        Me.cbxMasterTexture.Name = "cbxMasterTexture"
        Me.cbxMasterTexture.Size = New System.Drawing.Size(127, 21)
        Me.cbxMasterTexture.TabIndex = 103
        Me.cbxMasterTexture.Text = "Master Texturing"
        Me.cbxMasterTexture.UseCompatibleTextRendering = True
        Me.cbxMasterTexture.UseVisualStyleBackColor = True
        '
        'frmGenerator
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(730, 555)
        Me.Controls.Add(Me.cbxMasterTexture)
        Me.Controls.Add(Me.txt10y)
        Me.Controls.Add(Me.txt10x)
        Me.Controls.Add(Me.rdoPlayer10)
        Me.Controls.Add(Me.txt9y)
        Me.Controls.Add(Me.txt9x)
        Me.Controls.Add(Me.rdoPlayer9)
        Me.Controls.Add(Me.txtConnectedWater)
        Me.Controls.Add(Me.Label34)
        Me.Controls.Add(Me.txtRampBase)
        Me.Controls.Add(Me.Label33)
        Me.Controls.Add(Me.txtOilTolerance)
        Me.Controls.Add(Me.Label32)
        Me.Controls.Add(Me.txtOilAtATime)
        Me.Controls.Add(Me.Label31)
        Me.Controls.Add(Me.cboSymmetry)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.rdoPlayer1)
        Me.Controls.Add(Me.txtEqualize)
        Me.Controls.Add(Me.Label30)
        Me.Controls.Add(Me.txtRandom)
        Me.Controls.Add(Me.Label29)
        Me.Controls.Add(Me.txtVariation)
        Me.Controls.Add(Me.Label28)
        Me.Controls.Add(Me.Label27)
        Me.Controls.Add(Me.txtWaterQuantity)
        Me.Controls.Add(Me.Label26)
        Me.Controls.Add(Me.cbxFlatBases)
        Me.Controls.Add(Me.txtFlatness)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.cboTileset)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.txtTrucks)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.txtFClusterMax)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.txtFClusterMin)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.txtFClusterChance)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.txtFScatterChance)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.txtOilDispersion)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.txtBaseLevel)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.txtOilClusterMax)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.txtOilClusterMin)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.txtOilElsewhere)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.txtBaseOil)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.txtRampDistance)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtLevelFrequency)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtLevels)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txt8y)
        Me.Controls.Add(Me.txt8x)
        Me.Controls.Add(Me.rdoPlayer8)
        Me.Controls.Add(Me.txt7y)
        Me.Controls.Add(Me.txt7x)
        Me.Controls.Add(Me.rdoPlayer7)
        Me.Controls.Add(Me.txt6y)
        Me.Controls.Add(Me.txt6x)
        Me.Controls.Add(Me.rdoPlayer6)
        Me.Controls.Add(Me.txt5y)
        Me.Controls.Add(Me.txt5x)
        Me.Controls.Add(Me.rdoPlayer5)
        Me.Controls.Add(Me.txt4y)
        Me.Controls.Add(Me.txt4x)
        Me.Controls.Add(Me.rdoPlayer4)
        Me.Controls.Add(Me.txt3y)
        Me.Controls.Add(Me.txt3x)
        Me.Controls.Add(Me.rdoPlayer3)
        Me.Controls.Add(Me.txt2y)
        Me.Controls.Add(Me.txt2x)
        Me.Controls.Add(Me.txt1y)
        Me.Controls.Add(Me.rdoPlayer2)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txt1x)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtHeight)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtWidth)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnGenerate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.Name = "frmGenerator"
        Me.Text = "Generator"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnGenerate As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtHeight As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txt1x As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents rdoPlayer2 As System.Windows.Forms.RadioButton
    Friend WithEvents txt1y As System.Windows.Forms.TextBox
    Friend WithEvents txt2y As System.Windows.Forms.TextBox
    Friend WithEvents txt2x As System.Windows.Forms.TextBox
    Friend WithEvents txt3y As System.Windows.Forms.TextBox
    Friend WithEvents txt3x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer3 As System.Windows.Forms.RadioButton
    Friend WithEvents txt4y As System.Windows.Forms.TextBox
    Friend WithEvents txt4x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer4 As System.Windows.Forms.RadioButton
    Friend WithEvents txt5y As System.Windows.Forms.TextBox
    Friend WithEvents txt5x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer5 As System.Windows.Forms.RadioButton
    Friend WithEvents txt6y As System.Windows.Forms.TextBox
    Friend WithEvents txt6x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer6 As System.Windows.Forms.RadioButton
    Friend WithEvents txt7y As System.Windows.Forms.TextBox
    Friend WithEvents txt7x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer7 As System.Windows.Forms.RadioButton
    Friend WithEvents txt8y As System.Windows.Forms.TextBox
    Friend WithEvents txt8x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer8 As System.Windows.Forms.RadioButton
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtLevels As System.Windows.Forms.TextBox
    Friend WithEvents txtLevelFrequency As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtRampDistance As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents txtBaseOil As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtOilElsewhere As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtOilClusterMin As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtOilClusterMax As System.Windows.Forms.TextBox
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents txtBaseLevel As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents txtOilDispersion As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents txtFScatterChance As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents txtFClusterChance As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents txtFClusterMin As System.Windows.Forms.TextBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents txtFClusterMax As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents txtTrucks As System.Windows.Forms.TextBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents cboTileset As System.Windows.Forms.ComboBox
    Friend WithEvents txtFlatness As System.Windows.Forms.TextBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents cbxFlatBases As System.Windows.Forms.CheckBox
    Friend WithEvents txtWaterQuantity As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents txtVariation As System.Windows.Forms.TextBox
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents txtRandom As System.Windows.Forms.TextBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents txtEqualize As System.Windows.Forms.TextBox
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents rdoPlayer1 As System.Windows.Forms.RadioButton
    Friend WithEvents cboSymmetry As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtOilAtATime As System.Windows.Forms.TextBox
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents txtOilTolerance As System.Windows.Forms.TextBox
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents txtRampBase As System.Windows.Forms.TextBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents txtConnectedWater As System.Windows.Forms.TextBox
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents txt9y As System.Windows.Forms.TextBox
    Friend WithEvents txt9x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer9 As System.Windows.Forms.RadioButton
    Friend WithEvents txt10y As System.Windows.Forms.TextBox
    Friend WithEvents txt10x As System.Windows.Forms.TextBox
    Friend WithEvents rdoPlayer10 As System.Windows.Forms.RadioButton
    Friend WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents cbxMasterTexture As System.Windows.Forms.CheckBox
#End If
End Class