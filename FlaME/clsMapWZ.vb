Imports ICSharpCode.SharpZipLib

Partial Public Class clsMap

    Public Class clsWZBJOUnits
        Public Structure sUnit
            Public Code As String
            Public ID As UInteger
            Public Pos As sWorldPos
            Public Rotation As UInteger
            Public Player As UInteger
            Public ObjectType As clsUnitType.enumType
        End Structure
        Public Units(0) As sUnit
        Public UnitCount As Integer = 0

        Public Sub Unit_Add(ByVal NewUnit As sUnit)

            If Units.GetUpperBound(0) < UnitCount Then
                ReDim Preserve Units(UnitCount * 2 + 1)
            End If
            Units(UnitCount) = NewUnit
            UnitCount += 1
        End Sub
    End Class
   

    Public Function Load_WZ(ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult
        Dim SubResult As sResult
        Dim Quote As String = ControlChars.Quote
        Dim ZipEntry As Zip.ZipEntry
        Dim Bytes As sBytes
        Dim LineData As New sLines
        Dim GameFound As Boolean
        Dim DatasetFound As Boolean
        Dim MapName As frmWZLoad.sMapNameList
        ReDim MapName.Names(-1)
        Dim MapTileset(-1) As clsTileset
        Dim GameTileset As clsTileset = Nothing
        Dim MapCount As Integer
        Dim GameName As String = ""
        Dim strTemp As String = ""
        Dim SplitPath As sZipSplitPath
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer

        Dim ZipStream As Zip.ZipInputStream

        'get all usable lev entries
        ZipStream = New Zip.ZipInputStream(IO.File.OpenRead(Path))
        Do
            ZipEntry = ZipStream.GetNextEntry
            If ZipEntry Is Nothing Then
                Exit Do
            End If

            SplitPath = New sZipSplitPath(ZipEntry.Name)

            If SplitPath.FileExtension = "lev" And SplitPath.PartCount = 1 Then
                If ZipEntry.Size > 10 * 1024 * 1024 Then
                    ReturnResult.Problem_Add("lev file is too large.")
                    ZipStream.Close()
                    Return ReturnResult
                End If
                ReDim Bytes.Bytes(CInt(ZipEntry.Size - 1))
                ZipStream.Read(Bytes.Bytes, 0, CInt(ZipEntry.Size))
                BytesToLines(Bytes, LineData)
                LineData.RemoveComments()
                'find each level block
                For A = 0 To LineData.Lines.GetUpperBound(0)
                    If Strings.LCase(Strings.Left(LineData.Lines(A), 5)) = "level" Then
                        'find each levels game file
                        GameFound = False
                        B = 1
                        Do While A + B <= LineData.Lines.GetUpperBound(0)
                            If Strings.LCase(Strings.Left(LineData.Lines(A + B), 4)) = "game" Then
                                C = Strings.InStr(LineData.Lines(A + B), Quote)
                                D = Strings.InStrRev(LineData.Lines(A + B), Quote)
                                If C > 0 And D > 0 And D - C > 1 Then
                                    GameName = Strings.LCase(Strings.Mid(LineData.Lines(A + B), C + 1, D - C - 1))
                                    'see if map is already counted
                                    For C = 0 To MapCount - 1
                                        If GameName = MapName.Names(C) Then Exit For
                                    Next
                                    If C = MapCount Then
                                        GameFound = True
                                    End If
                                End If
                                Exit Do
                            ElseIf Strings.LCase(Strings.Left(LineData.Lines(A + B), 5)) = "level" Then
                                Exit Do
                            End If
                            B += 1
                        Loop
                        If GameFound Then
                            'find the dataset (determines tileset)
                            DatasetFound = False
                            B = 1
                            Do While A + B <= LineData.Lines.GetUpperBound(0)
                                If Strings.LCase(Strings.Left(LineData.Lines(A + B), 7)) = "dataset" Then
                                    strTemp = Strings.LCase(Strings.Right(LineData.Lines(A + B), 1))
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
                                ElseIf Strings.LCase(Strings.Left(LineData.Lines(A + B), 5)) = "level" Then
                                    Exit Do
                                End If
                                B += 1
                            Loop
                            If DatasetFound Then
                                ReDim Preserve MapName.Names(MapCount)
                                ReDim Preserve MapTileset(MapCount)
                                MapName.Names(MapCount) = GameName
                                MapTileset(MapCount) = GameTileset
                                MapCount += 1
                            End If
                        End If
                    End If
                Next
            End If
        Loop
        ZipStream.Close()

        Dim MapLoadName As String

        'prompt user for which of the entries to load
        If MapCount < 1 Then
            ReturnResult.Problem_Add("No maps found in file.")
            Return ReturnResult
        ElseIf MapCount = 1 Then
            MapLoadName = MapName.Names(0)
            Tileset = MapTileset(0)
        Else
            Dim SelectToLoadResult As New frmWZLoad.clsOutput
            Dim SelectToLoadForm As New frmWZLoad(MapName, SelectToLoadResult, "Select a map from " & New sSplitPath(Path).FileTitle)
            SelectToLoadForm.ShowDialog()
            If SelectToLoadResult.Result < 0 Then
                ReturnResult.Problem_Add("No map selected.")
                Return ReturnResult
            End If
            MapLoadName = MapName.Names(SelectToLoadResult.Result)
            Tileset = MapTileset(SelectToLoadResult.Result)
        End If

        TileType_Reset()
        SetPainterToDefaults()

        Dim GameSplitPath As New sZipSplitPath(MapLoadName)
        Dim GameFilesPath As String = GameSplitPath.FilePath & GameSplitPath.FileTitleWithoutExtension & "/"

        Dim File As New clsReadFile
        Dim ZipSearchResult As clsZipStreamEntry

        'load map files

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "game.map")
        If ZipSearchResult Is Nothing Then
            ReturnResult.Problem_Add("game.map file not found.")
            Return ReturnResult
        Else
            SubResult = Read_WZ_Game(ZipSearchResult.BeginNewReadFile)
            ZipSearchResult.Stream.Close()

            If Not SubResult.Success Then
                ReturnResult.Problem_Add(SubResult.Problem)
                Return ReturnResult
            End If
        End If

        Dim BJOUnits As New clsMap.clsWZBJOUnits
        Dim NewUnit As clsUnit

        Dim INIFeatures As clsINIFeatures = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "feature.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim FeaturesINI As New clsINIRead
            ReturnResult.AppendAsWarning(FeaturesINI.ReadFile(ZipSearchResult.BeginNewReadFile), "Features INI: ")
            ZipSearchResult.Stream.Close()
            INIFeatures = New clsINIFeatures(FeaturesINI.SectionCount)
            ReturnResult.AppendAsWarning(FeaturesINI.Translate(INIFeatures), "Features INI: ")
        End If

        If INIFeatures Is Nothing Then
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "feat.bjo")
            If ZipSearchResult Is Nothing Then
                ReturnResult.Warning_Add("feat.bjo file not found.")
            Else
                SubResult = Read_WZ_Features(ZipSearchResult.BeginNewReadFile, BJOUnits)
                ZipSearchResult.Stream.Close()
                If Not SubResult.Success Then
                    ReturnResult.Warning_Add(SubResult.Problem)
                End If
            End If
        End If

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "ttypes.ttp")
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("ttypes.ttp file not found.")
        Else
            SubResult = Read_WZ_TileTypes(ZipSearchResult.BeginNewReadFile)
            ZipSearchResult.Stream.Close()
            If Not SubResult.Success Then
                ReturnResult.Warning_Add(SubResult.Problem)
            End If
        End If

        Dim INIStructures As clsINIStructures = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "struct.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim StructuresINI As New clsINIRead
            ReturnResult.AppendAsWarning(StructuresINI.ReadFile(ZipSearchResult.BeginNewReadFile), "Structures INI: ")
            ZipSearchResult.Stream.Close()
            INIStructures = New clsINIStructures(StructuresINI.SectionCount, Me)
            ReturnResult.AppendAsWarning(StructuresINI.Translate(INIStructures), "Structures INI: ")
        End If

        If INIStructures Is Nothing Then
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "struct.bjo")
            If ZipSearchResult Is Nothing Then
                ReturnResult.Warning_Add("struct.bjo file not found.")
            Else
                SubResult = Read_WZ_Structures(ZipSearchResult.BeginNewReadFile, BJOUnits)
                ZipSearchResult.Stream.Close()
                If Not SubResult.Success Then
                    ReturnResult.Warning_Add(SubResult.Problem)
                End If
            End If
        End If

        Dim INIDroids As clsINIDroids = Nothing

        ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "droid.ini")
        If ZipSearchResult Is Nothing Then

        Else
            Dim DroidsINI As New clsINIRead
            ReturnResult.AppendAsWarning(DroidsINI.ReadFile(ZipSearchResult.BeginNewReadFile), "Droids INI: ")
            INIDroids = New clsINIDroids(DroidsINI.SectionCount, Me)
            ReturnResult.AppendAsWarning(DroidsINI.Translate(INIDroids), "Droids INI: ")
        End If

        If INIDroids Is Nothing Then
            ZipSearchResult = FindZipEntryFromPath(Path, GameFilesPath & "dinit.bjo")
            If ZipSearchResult Is Nothing Then
                ReturnResult.Warning_Add("dinit.bjo file not found.")
            Else
                SubResult = Read_WZ_Droids(ZipSearchResult.BeginNewReadFile, BJOUnits)
                ZipSearchResult.Stream.Close()
                If Not SubResult.Success Then
                    ReturnResult.Warning_Add(SubResult.Problem)
                End If
            End If
        End If

        Dim AvailableID As UInteger

        AvailableID = 1UI
        For A = 0 To BJOUnits.UnitCount - 1
            If BJOUnits.Units(A).ID >= AvailableID Then
                AvailableID = BJOUnits.Units(A).ID + 1UI
            End If
        Next
        If INIStructures IsNot Nothing Then
            For A = 0 To INIStructures.StructureCount - 1
                If INIStructures.Structures(A).ID >= AvailableID Then
                    AvailableID = INIStructures.Structures(A).ID + 1UI
                End If
            Next
        End If
        If INIFeatures IsNot Nothing Then
            For A = 0 To INIFeatures.FeatureCount - 1
                If INIFeatures.Features(A).ID >= AvailableID Then
                    AvailableID = INIFeatures.Features(A).ID + 1UI
                End If
            Next
        End If
        If INIDroids IsNot Nothing Then
            For A = 0 To INIDroids.DroidCount - 1
                If INIDroids.Droids(A).ID >= AvailableID Then
                    AvailableID = INIDroids.Droids(A).ID + 1UI
                End If
            Next
        End If

        For A = 0 To BJOUnits.UnitCount - 1
            NewUnit = New clsUnit
            NewUnit.ID = BJOUnits.Units(A).ID
            NewUnit.Type = FindOrCreateUnitType(BJOUnits.Units(A).Code, BJOUnits.Units(A).ObjectType)
            If Not SubResult.Success Then
                ReturnResult.Problem_Add(SubResult.Problem)
                Return ReturnResult
            End If
            If BJOUnits.Units(A).Player >= PlayerCountMax Then
                NewUnit.UnitGroup = ScavengerUnitGroup
            Else
                NewUnit.UnitGroup = UnitGroups(CInt(BJOUnits.Units(A).Player))
            End If
            NewUnit.Pos = BJOUnits.Units(A).Pos
            NewUnit.Rotation = CInt(Math.Min(BJOUnits.Units(A).Rotation, 359UI))
            If BJOUnits.Units(A).ID = 0UI Then
                BJOUnits.Units(A).ID = AvailableID
                ZeroIDWarning(NewUnit, BJOUnits.Units(A).ID)
            End If
            UnitID_Add(NewUnit, BJOUnits.Units(A).ID)
            ErrorIDChange(BJOUnits.Units(A).ID, NewUnit, "Load_WZ->BJOObjects")
            If AvailableID = BJOUnits.Units(A).ID Then
                AvailableID = NewUnit.ID + 1UI
            End If
        Next

        Dim tmpDroidType As clsDroidDesign
        Dim tmpFeatureType As clsFeatureType
        Dim LoadPartsArgs As clsDroidDesign.sLoadPartsArgs
        Dim tmpUnitType As clsUnitType = Nothing
        Dim ErrorCount As Integer = 0
        Dim UnknownDroidComponentCount As Integer = 0
        Dim UnknownDroidTypeCount As Integer = 0
        Dim DroidBadPositionCount As Integer = 0
        Dim StructureBadPositionCount As Integer = 0
        Dim StructureBadModulesCount As Integer = 0
        Dim FeatureBadPositionCount As Integer = 0
        Dim ModuleLimit As Integer
        Dim ZeroPos As New sXY_int(0, 0)
        Dim tmpStructureType As clsStructureType
        Dim tmpModuleType As clsStructureType
        Dim NewModule As clsUnit

        Dim FactoryModule As clsStructureType = FindFirstStructureType(clsStructureType.enumStructureType.FactoryModule)
        Dim ResearchModule As clsStructureType = FindFirstStructureType(clsStructureType.enumStructureType.ResearchModule)
        Dim PowerModule As clsStructureType = FindFirstStructureType(clsStructureType.enumStructureType.PowerModule)

        If FactoryModule Is Nothing Then
            ReturnResult.Warning_Add("No factory module loaded.")
        End If
        If ResearchModule Is Nothing Then
            ReturnResult.Warning_Add("No research module loaded.")
        End If
        If PowerModule Is Nothing Then
            ReturnResult.Warning_Add("No power module loaded.")
        End If

        If INIStructures IsNot Nothing Then
            For A = 0 To INIStructures.StructureCount - 1
                If INIStructures.Structures(A).Pos Is Nothing Then
                    StructureBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIStructures.Structures(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    StructureBadPositionCount += 1
                Else
                    tmpUnitType = FindOrCreateUnitType(INIStructures.Structures(A).Code, clsUnitType.enumType.PlayerStructure)
                    If tmpUnitType.Type = clsUnitType.enumType.PlayerStructure Then
                        tmpStructureType = CType(tmpUnitType, clsStructureType)
                    Else
                        tmpStructureType = Nothing
                    End If
                    If tmpStructureType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = tmpStructureType
                        If INIStructures.Structures(A).UnitGroup Is Nothing Then
                            NewUnit.UnitGroup = ScavengerUnitGroup
                        Else
                            NewUnit.UnitGroup = INIStructures.Structures(A).UnitGroup
                        End If
                        NewUnit.Pos = INIStructures.Structures(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIStructures.Structures(A).Rotation.Direction * 360.0# / 65536.0#)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIStructures.Structures(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIStructures.Structures(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIStructures.Structures(A).ID = 0UI Then
                            INIStructures.Structures(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIStructures.Structures(A).ID)
                        End If
                        UnitID_Add(NewUnit, INIStructures.Structures(A).ID)
                        ErrorIDChange(INIStructures.Structures(A).ID, NewUnit, "Load_WZ->INIStructures")
                        If AvailableID = INIStructures.Structures(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                        'create modules
                        Select Case tmpStructureType.StructureType
                            Case clsStructureType.enumStructureType.Factory
                                ModuleLimit = 2
                                tmpModuleType = FactoryModule
                            Case clsStructureType.enumStructureType.PowerGenerator
                                ModuleLimit = 1
                                tmpModuleType = PowerModule
                            Case clsStructureType.enumStructureType.Research
                                ModuleLimit = 1
                                tmpModuleType = ResearchModule
                            Case Else
                                ModuleLimit = 0
                                tmpModuleType = Nothing
                        End Select
                        If INIStructures.Structures(A).ModuleCount > ModuleLimit Then
                            INIStructures.Structures(A).ModuleCount = ModuleLimit
                            StructureBadModulesCount += 1
                        ElseIf INIStructures.Structures(A).ModuleCount < 0 Then
                            INIStructures.Structures(A).ModuleCount = 0
                            StructureBadModulesCount += 1
                        End If
                        If tmpModuleType IsNot Nothing Then
                            For B = 0 To INIStructures.Structures(A).ModuleCount - 1
                                NewModule = New clsUnit
                                NewModule.Type = tmpModuleType
                                NewModule.UnitGroup = NewUnit.UnitGroup
                                NewModule.Pos = NewUnit.Pos
                                NewModule.Rotation = NewUnit.Rotation
                                UnitID_Add(NewModule, AvailableID)
                                AvailableID = NewModule.ID + 1UI
                            Next
                        End If
                    End If
                End If
            Next
            If StructureBadPositionCount > 0 Then
                ReturnResult.Warning_Add(StructureBadPositionCount & " structures had an invalid position and were removed.")
            End If
            If StructureBadModulesCount > 0 Then
                ReturnResult.Warning_Add(StructureBadModulesCount & " structures had an invalid number of modules.")
            End If
        End If
        If INIFeatures IsNot Nothing Then
            For A = 0 To INIFeatures.FeatureCount - 1
                If INIFeatures.Features(A).Pos Is Nothing Then
                    FeatureBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIFeatures.Features(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    FeatureBadPositionCount += 1
                Else
                    tmpUnitType = FindOrCreateUnitType(INIFeatures.Features(A).Code, clsUnitType.enumType.Feature)
                    If tmpUnitType.Type = clsUnitType.enumType.Feature Then
                        tmpFeatureType = CType(tmpUnitType, clsFeatureType)
                    Else
                        tmpFeatureType = Nothing
                    End If
                    If tmpFeatureType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = tmpFeatureType
                        NewUnit.UnitGroup = FeatureUnitGroup
                        NewUnit.Pos = INIFeatures.Features(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIFeatures.Features(A).Rotation.Direction * 360.0# / 65536.0#)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIFeatures.Features(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIFeatures.Features(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIFeatures.Features(A).ID = 0UI Then
                            INIFeatures.Features(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIFeatures.Features(A).ID)
                        End If
                        UnitID_Add(NewUnit, INIFeatures.Features(A).ID)
                        ErrorIDChange(INIFeatures.Features(A).ID, NewUnit, "Load_WZ->INIFeatures")
                        If AvailableID = INIFeatures.Features(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                    End If
                End If
            Next
            If FeatureBadPositionCount > 0 Then
                ReturnResult.Warning_Add(FeatureBadPositionCount & " features had an invalid position and were removed.")
            End If
        End If
        If INIDroids IsNot Nothing Then
            For A = 0 To INIDroids.DroidCount - 1
                If INIDroids.Droids(A).Pos Is Nothing Then
                    DroidBadPositionCount += 1
                ElseIf Not PosIsWithinTileArea(INIDroids.Droids(A).Pos.WorldPos.Horizontal, ZeroPos, Terrain.TileSize) Then
                    DroidBadPositionCount += 1
                Else
                    If INIDroids.Droids(A).Template = Nothing Or INIDroids.Droids(A).Template = "" Then
                        tmpDroidType = New clsDroidDesign
                        If Not tmpDroidType.SetDroidType(CType(INIDroids.Droids(A).DroidType, enumDroidType)) Then
                            UnknownDroidTypeCount += 1
                        End If
                        LoadPartsArgs.Body = FindOrCreateBody(INIDroids.Droids(A).Body)
                        If LoadPartsArgs.Body Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Body.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Propulsion = FindOrCreatePropulsion(INIDroids.Droids(A).Propulsion)
                        If LoadPartsArgs.Propulsion Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Propulsion.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Construct = FindOrCreateConstruct(INIDroids.Droids(A).Construct)
                        If LoadPartsArgs.Construct Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Construct.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Repair = FindOrCreateRepair(INIDroids.Droids(A).Repair)
                        If LoadPartsArgs.Repair Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Repair.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Sensor = FindOrCreateSensor(INIDroids.Droids(A).Sensor)
                        If LoadPartsArgs.Sensor Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Sensor.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Brain = FindOrCreateBrain(INIDroids.Droids(A).Brain)
                        If LoadPartsArgs.Brain Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Brain.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.ECM = FindOrCreateECM(INIDroids.Droids(A).ECM)
                        If LoadPartsArgs.ECM Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.ECM.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon1 = FindOrCreateWeapon(INIDroids.Droids(A).Weapons(0))
                        If LoadPartsArgs.Weapon1 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon1.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon2 = FindOrCreateWeapon(INIDroids.Droids(A).Weapons(1))
                        If LoadPartsArgs.Weapon2 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon2.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        LoadPartsArgs.Weapon3 = FindOrCreateWeapon(INIDroids.Droids(A).Weapons(2))
                        If LoadPartsArgs.Weapon3 Is Nothing Then
                            UnknownDroidComponentCount += 1
                        Else
                            If LoadPartsArgs.Weapon3.IsUnknown Then
                                UnknownDroidComponentCount += 1
                            End If
                        End If
                        tmpDroidType.LoadParts(LoadPartsArgs)
                    Else
                        tmpUnitType = FindOrCreateUnitType(INIDroids.Droids(A).Template, clsUnitType.enumType.PlayerDroid)
                        If tmpUnitType Is Nothing Then
                            tmpDroidType = Nothing
                        Else
                            If tmpUnitType.Type = clsUnitType.enumType.PlayerDroid Then
                                tmpDroidType = CType(tmpUnitType, clsDroidDesign)
                            Else
                                tmpDroidType = Nothing
                            End If
                        End If
                    End If
                    If tmpDroidType Is Nothing Then
                        ErrorCount += 1
                    Else
                        NewUnit = New clsUnit
                        NewUnit.Type = tmpDroidType
                        If INIDroids.Droids(A).UnitGroup Is Nothing Then
                            NewUnit.UnitGroup = ScavengerUnitGroup
                        Else
                            NewUnit.UnitGroup = INIDroids.Droids(A).UnitGroup
                        End If
                        NewUnit.Pos = INIDroids.Droids(A).Pos.WorldPos
                        NewUnit.Rotation = CInt(INIDroids.Droids(A).Rotation.Direction * 360.0# / 65536.0#)
                        If NewUnit.Rotation = 360 Then
                            NewUnit.Rotation = 0
                        End If
                        If INIDroids.Droids(A).HealthPercent >= 0 Then
                            NewUnit.Health = Clamp_dbl(INIDroids.Droids(A).HealthPercent / 100.0#, 0.01#, 1.0#)
                        End If
                        If INIDroids.Droids(A).ID = 0UI Then
                            INIDroids.Droids(A).ID = AvailableID
                            ZeroIDWarning(NewUnit, INIDroids.Droids(A).ID)
                        End If
                        UnitID_Add(NewUnit, INIDroids.Droids(A).ID)
                        ErrorIDChange(INIDroids.Droids(A).ID, NewUnit, "Load_WZ->INIDroids")
                        If AvailableID = INIDroids.Droids(A).ID Then
                            AvailableID = NewUnit.ID + 1UI
                        End If
                    End If
                End If
            Next
            If DroidBadPositionCount > 0 Then
                ReturnResult.Warning_Add(DroidBadPositionCount & " droids had an invalid position and were removed.")
            End If
            If UnknownDroidTypeCount > 0 Then
                ReturnResult.Warning_Add(UnknownDroidTypeCount & " droid designs had an unrecognised droidType and were removed.")
            End If
            If UnknownDroidComponentCount > 0 Then
                ReturnResult.Warning_Add(UnknownDroidComponentCount & " droid designs had components that are not loaded.")
            End If
        End If

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add("Object Create Error.")
        End If

        AfterInitialized()

        Return ReturnResult
    End Function

    Public Class clsINIStructures
        Inherits clsINIRead.clsSectionTranslator

        Private ParentMap As clsMap

        Public Structure sStructure
            Public ID As UInteger
            Public Code As String
            Public UnitGroup As clsMap.clsUnitGroup
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public ModuleCount As Integer
            Public HealthPercent As Integer
        End Structure
        Public Structures() As sStructure
        Public StructureCount As Integer

        Public Sub New(ByVal NewStructureCount As Integer, ByVal NewParentMap As clsMap)
            Dim A As Integer

            ParentMap = NewParentMap

            StructureCount = NewStructureCount
            ReDim Structures(StructureCount - 1)
            For A = 0 To StructureCount - 1
                Structures(A).HealthPercent = -1
            Next
        End Sub

        Public Overrides Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Try
                        If CUInt(INIProperty.Value) > 0 Then
                            Structures(INISectionNum).ID = CUInt(INIProperty.Value)
                        End If
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "name"
                    Structures(INISectionNum).Code = INIProperty.Value
                Case "startpos"
                    Dim StartPos As Integer
                    Try
                        StartPos = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If StartPos < 0 Or StartPos >= PlayerCountMax Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).UnitGroup = ParentMap.UnitGroups(StartPos)
                Case "player"
                    If INIProperty.Value.ToLower <> "scavenger" Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Structures(INISectionNum).UnitGroup = ParentMap.ScavengerUnitGroup
                Case "position"
                    Dim VectorText() As String
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        Structures(INISectionNum).Pos = New clsWorldPos(New sWorldPos(New sXY_int(CInt(VectorText(0)), CInt(VectorText(1))), CInt(VectorText(2))))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "rotation"
                    Dim VectorText() As String
                    Dim WZAngle As sWZAngle
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        WZAngle.Direction = CUShort(VectorText(0))
                        WZAngle.Pitch = CUShort(VectorText(1))
                        WZAngle.Roll = CUShort(VectorText(2))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    Structures(INISectionNum).Rotation = WZAngle
                Case "modules"
                    Try
                        If CInt(INIProperty.Value) >= 0 Then
                            Structures(INISectionNum).ModuleCount = CInt(INIProperty.Value)
                        Else
                            Return clsINIRead.enumTranslatorResult.ValueInvalid
                        End If
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "health"
                    Dim A As Integer
                    A = INIProperty.Value.IndexOf("%"c)
                    If A < 0 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    INIProperty.Value = INIProperty.Value.Replace("%", "")
                    Try
                        Structures(INISectionNum).HealthPercent = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Class clsINIDroids
        Inherits clsINIRead.clsSectionTranslator

        Private ParentMap As clsMap

        Public Structure sDroid
            Public ID As UInteger
            Public Template As String
            Public UnitGroup As clsMap.clsUnitGroup
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public HealthPercent As Integer
            Public DroidType As Integer
            Public Body As String
            Public Propulsion As String
            Public Brain As String
            Public Repair As String
            Public ECM As String
            Public Sensor As String
            Public Construct As String
            Public Weapons() As String
            Public WeaponCount As Integer
        End Structure
        Public Droids() As sDroid
        Public DroidCount As Integer

        Public Sub New(ByVal NewDroidCount As Integer, ByVal NewParentMap As clsMap)
            Dim A As Integer

            ParentMap = NewParentMap

            DroidCount = NewDroidCount
            ReDim Droids(DroidCount - 1)
            For A = 0 To DroidCount - 1
                Droids(A).HealthPercent = -1
                Droids(A).DroidType = -1
            Next
        End Sub

        Public Overrides Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Try
                        If CUInt(INIProperty.Value) > 0 Then
                            Droids(INISectionNum).ID = CUInt(INIProperty.Value)
                        End If
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "template"
                    Droids(INISectionNum).Template = INIProperty.Value
                Case "startpos"
                    Dim StartPos As Integer
                    Try
                        StartPos = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If StartPos < 0 Or StartPos >= PlayerCountMax Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Droids(INISectionNum).UnitGroup = ParentMap.UnitGroups(StartPos)
                Case "player"
                    If INIProperty.Value.ToLower <> "scavenger" Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Droids(INISectionNum).UnitGroup = ParentMap.ScavengerUnitGroup
                Case "name"
                    'ignore
                Case "position"
                    Dim VectorText() As String
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        Droids(INISectionNum).Pos = New clsWorldPos(New sWorldPos(New sXY_int(CInt(VectorText(0)), CInt(VectorText(1))), CInt(VectorText(2))))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "rotation"
                    Dim VectorText() As String
                    Dim WZAngle As sWZAngle
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        WZAngle.Direction = CUShort(VectorText(0))
                        WZAngle.Pitch = CUShort(VectorText(1))
                        WZAngle.Roll = CUShort(VectorText(2))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    Droids(INISectionNum).Rotation = WZAngle
                Case "health"
                    Dim A As Integer
                    A = INIProperty.Value.IndexOf("%"c)
                    If A < 0 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    INIProperty.Value = INIProperty.Value.Replace("%", "")
                    Try
                        Droids(INISectionNum).HealthPercent = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "droidtype"
                    Try
                        Droids(INISectionNum).DroidType = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "weapons"
                    Try
                        Droids(INISectionNum).WeaponCount = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "parts\body"
                    Droids(INISectionNum).Body = INIProperty.Value
                Case "parts\propulsion"
                    Droids(INISectionNum).Propulsion = INIProperty.Value
                Case "parts\brain"
                    Droids(INISectionNum).Brain = INIProperty.Value
                Case "parts\repair"
                    Droids(INISectionNum).Repair = INIProperty.Value
                Case "parts\ecm"
                    Droids(INISectionNum).ECM = INIProperty.Value
                Case "parts\sensor"
                    Droids(INISectionNum).Sensor = INIProperty.Value
                Case "parts\construct"
                    Droids(INISectionNum).Construct = INIProperty.Value
                Case "parts\weapon\1"
                    Droids(INISectionNum).Weapons(0) = INIProperty.Value
                Case "parts\weapon\2"
                    Droids(INISectionNum).Weapons(1) = INIProperty.Value
                Case "parts\weapon\3"
                    Droids(INISectionNum).Weapons(2) = INIProperty.Value
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Class clsINIFeatures
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sFeatures
            Public ID As UInteger
            Public Code As String
            Public Pos As clsWorldPos
            Public Rotation As sWZAngle
            Public HealthPercent As Integer
        End Structure
        Public Features() As sFeatures
        Public FeatureCount As Integer

        Public Sub New(ByVal NewFeatureCount As Integer)

            FeatureCount = NewFeatureCount
            ReDim Features(FeatureCount - 1)
        End Sub

        Public Overrides Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "id"
                    Try
                        If CUInt(INIProperty.Value) > 0 Then
                            Features(INISectionNum).ID = CUInt(INIProperty.Value)
                        End If
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "name"
                    Features(INISectionNum).Code = INIProperty.Value
                Case "position"
                    Dim VectorText() As String
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        Features(INISectionNum).Pos = New clsWorldPos(New sWorldPos(New sXY_int(CInt(VectorText(0)), CInt(VectorText(1))), CInt(VectorText(2))))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "rotation"
                    Dim VectorText() As String
                    Dim WZAngle As sWZAngle
                    Dim A As Integer
                    VectorText = INIProperty.Value.Split(","c)
                    If VectorText.GetUpperBound(0) + 1 <> 3 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To VectorText.GetUpperBound(0)
                        VectorText(A) = VectorText(A).Trim()
                    Next
                    Try
                        WZAngle.Direction = CUShort(VectorText(0))
                        WZAngle.Pitch = CUShort(VectorText(1))
                        WZAngle.Roll = CUShort(VectorText(2))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    Features(INISectionNum).Rotation = WZAngle
                Case "health"
                    Dim A As Integer
                    Dim Health As Integer
                    A = INIProperty.Value.IndexOf("%"c)
                    If A < 0 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    INIProperty.Value = INIProperty.Value.Replace("%", "")
                    Try
                        Health = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If Health < 0 Or Health > 100 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Features(INISectionNum).HealthPercent = Health
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_WZ_Game(ByVal File As clsReadFile) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim MapWidth As UInteger
        Dim MapHeight As UInteger
        Dim uintTemp As UInteger
        Dim byteTemp As Byte
        Dim Flip As Byte
        Dim FlipX As Boolean
        Dim FlipZ As Boolean
        Dim Rotate As Byte
        Dim TextureNum As Byte
        Dim A As Integer
        Dim X As Integer
        Dim Y As Integer

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If strTemp <> "map " Then
            ReturnResult.Problem = "Unknown game.map identifier."
            Return ReturnResult
        End If

        If Not File.Get_U32(Version) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Version <> 10UI Then
            'Load_WZ.Problem = "Unknown game.map version."
            'Exit Function
            If MsgBox("game.map version is unknown. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle)) <> MsgBoxResult.Ok Then
                ReturnResult.Problem = "Aborted."
                Return ReturnResult
            End If
        End If
        If Not File.Get_U32(MapWidth) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Not File.Get_U32(MapHeight) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If MapWidth < 1UI Or MapWidth > MapMaxSize Or MapHeight < 1UI Or MapHeight > MapMaxSize Then
            ReturnResult.Problem = "Map size out of range."
            Return ReturnResult
        End If

        Terrain_Blank(New sXY_int(CInt(MapWidth), CInt(MapHeight)))

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Not File.Get_U8(TextureNum) Then
                    ReturnResult.Problem = "Tile data read error."
                    Return ReturnResult
                End If
                Terrain.Tiles(X, Y).Texture.TextureNum = TextureNum
                If Not File.Get_U8(Flip) Then
                    ReturnResult.Problem = "Tile data read error."
                    Return ReturnResult
                End If
                If Not File.Get_U8(Terrain.Vertices(X, Y).Height) Then
                    ReturnResult.Problem = "Tile data read error."
                    Return ReturnResult
                End If
                'get flipx
                A = CInt(Int(Flip / 128.0#))
                Flip -= CByte(A * 128)
                FlipX = (A = 1)
                'get flipy
                A = CInt(Int(Flip / 64.0#))
                Flip -= CByte(A * 64)
                FlipZ = (A = 1)
                'get rotation
                A = CInt(Int(Flip / 16.0#))
                Flip -= CByte(A * 16)
                Rotate = CByte(A)
                OldOrientation_To_TileOrientation(Rotate, FlipX, FlipZ, Terrain.Tiles(X, Y).Texture.Orientation)
                'get tri direction
                A = CInt(Int(Flip / 8.0#))
                Flip -= CByte(A * 8)
                Terrain.Tiles(X, Y).Tri = (A = 1)
            Next
        Next

        If Version <> 2UI Then
            If Not File.Get_U32(uintTemp) Then
                ReturnResult.Problem = "Gateway version read error."
                Return ReturnResult
            End If
            If uintTemp <> 1 Then
                ReturnResult.Problem = "Bad gateway version number."
                Return ReturnResult
            End If

            If Not File.Get_U32(uintTemp) Then
                ReturnResult.Problem = "Gateway read error."
                Return ReturnResult
            End If
            GatewayCount = CInt(uintTemp)
            ReDim Gateways(GatewayCount - 1)

            For A = 0 To GatewayCount - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem = "Gateway read error."
                    Return ReturnResult
                End If
                Gateways(A).PosA.X = byteTemp
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem = "Gateway read error."
                    Return ReturnResult
                End If
                Gateways(A).PosA.Y = byteTemp
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem = "Gateway read error."
                    Return ReturnResult
                End If
                Gateways(A).PosB.X = byteTemp
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem = "Gateway read error."
                    Return ReturnResult
                End If
                Gateways(A).PosB.Y = byteTemp
            Next
        End If

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Features(ByVal File As clsReadFile, ByRef WZUnits As clsMap.clsWZBJOUnits) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim uintTemp2 As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsMap.clsWZBJOUnits.sUnit

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If strTemp <> "feat" Then
            ReturnResult.Problem = "Unknown feat.bjo identifier."
            Return ReturnResult
        End If

        If Not File.Get_U32(Version) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Version <> 8UI Then
            'Load_WZ.Problem = "Unknown feat.bjo version."
            'Exit Function
            If MsgBox("feat.bjo version is unknown. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle)) <> MsgBoxResult.Ok Then
                ReturnResult.Problem = "Aborted."
                Return ReturnResult
            End If
        End If

        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If

        For A = 0 To CInt(uintTemp) - 1
            WZBJOUnit = New clsMap.clsWZBJOUnits.sUnit
            WZBJOUnit.ObjectType = clsUnitType.enumType.Feature
            If Not File.Get_Text(40, WZBJOUnit.Code) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            B = Strings.InStr(WZBJOUnit.Code, Chr(0))
            If B > 0 Then
                WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
            End If
            If Not File.Get_U32(WZBJOUnit.ID) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.X = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.Y = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Altitude = CInt(uintTemp2)
            If Not File.Get_U32(WZBJOUnit.Rotation) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(WZBJOUnit.Player) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            File.Seek(File.Position + 12L)
            WZUnits.Unit_Add(WZBJOUnit)
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_TileTypes(ByVal File As clsReadFile) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim ushortTemp As UShort
        Dim A As Integer

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If strTemp <> "ttyp" Then
            ReturnResult.Problem = "Unknown ttypes.ttp identifier."
            Return ReturnResult
        End If

        If Not File.Get_U32(Version) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Version <> 8UI Then
            'Load_WZ.Problem = "Unknown ttypes.ttp version."
            'Exit Function
            If MsgBox("ttypes.ttp version is unknown. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle)) <> MsgBoxResult.Ok Then
                ReturnResult.Problem = "Aborted."
                Return ReturnResult
            End If
        End If

        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If

        If Tileset IsNot Nothing Then
            For A = 0 To Math.Min(CInt(uintTemp), Tileset.TileCount) - 1
                If Not File.Get_U16(ushortTemp) Then
                    ReturnResult.Problem = "Read error."
                    Return ReturnResult
                End If
                If ushortTemp > 11US Then
                    ReturnResult.Problem = "Unknown tile type."
                    Return ReturnResult
                End If
                Tile_TypeNum(A) = CByte(ushortTemp)
            Next
        End If

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Structures(ByVal File As clsReadFile, ByRef WZUnits As clsMap.clsWZBJOUnits) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim uintTemp2 As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsMap.clsWZBJOUnits.sUnit

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If strTemp <> "stru" Then
            ReturnResult.Problem = "Unknown struct.bjo identifier."
            Return ReturnResult
        End If

        If Not File.Get_U32(Version) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Version <> 8UI Then
            'Load_WZ.Problem = "Unknown struct.bjo version."
            'Exit Function
            If MsgBox("struct.bjo version is unknown. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle)) <> MsgBoxResult.Ok Then
                ReturnResult.Problem = "Aborted."
                Return ReturnResult
            End If
        End If

        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If

        For A = 0 To CInt(uintTemp) - 1
            WZBJOUnit = New clsMap.clsWZBJOUnits.sUnit
            WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerStructure
            If Not File.Get_Text(40, WZBJOUnit.Code) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            B = Strings.InStr(WZBJOUnit.Code, Chr(0))
            If B > 0 Then
                WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
            End If
            If Not File.Get_U32(WZBJOUnit.ID) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.X = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.Y = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Altitude = CInt(uintTemp2)
            If Not File.Get_U32(WZBJOUnit.Rotation) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(WZBJOUnit.Player) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            File.Seek(File.Position + 56L)
            WZUnits.Unit_Add(WZBJOUnit)
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function Read_WZ_Droids(ByVal File As clsReadFile, ByVal WZUnits As clsMap.clsWZBJOUnits) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = Nothing
        Dim Version As UInteger
        Dim uintTemp As UInteger
        Dim uintTemp2 As UInteger
        Dim A As Integer
        Dim B As Integer
        Dim WZBJOUnit As clsMap.clsWZBJOUnits.sUnit

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If

        If strTemp <> "dint" Then
            ReturnResult.Problem = "Unknown dinit.bjo identifier."
            Return ReturnResult
        End If

        If Not File.Get_U32(Version) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If
        If Version > 19UI Then
            'Load_WZ.Problem = "Unknown dinit.bjo version."
            'Exit Function
            If MsgBox("dinit.bjo version is unknown. Continue?", CType(MsgBoxStyle.OkCancel + MsgBoxStyle.Question, MsgBoxStyle)) <> MsgBoxResult.Ok Then
                ReturnResult.Problem = "Aborted."
                Return ReturnResult
            End If
        End If

        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Read error."
            Return ReturnResult
        End If

        For A = 0 To CInt(uintTemp) - 1
            WZBJOUnit = New clsMap.clsWZBJOUnits.sUnit
            WZBJOUnit.ObjectType = clsUnitType.enumType.PlayerDroid
            If Not File.Get_Text(40, WZBJOUnit.Code) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            B = Strings.InStr(WZBJOUnit.Code, Chr(0))
            If B > 0 Then
                WZBJOUnit.Code = Strings.Left(WZBJOUnit.Code, B - 1)
            End If
            If Not File.Get_U32(WZBJOUnit.ID) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.X = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Horizontal.Y = CInt(uintTemp2)
            If Not File.Get_U32(uintTemp2) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            WZBJOUnit.Pos.Altitude = CInt(uintTemp2)
            If Not File.Get_U32(WZBJOUnit.Rotation) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            If Not File.Get_U32(WZBJOUnit.Player) Then
                ReturnResult.Problem = "Read error."
                Return ReturnResult
            End If
            File.Seek(File.Position + 12L)
            WZUnits.Unit_Add(WZBJOUnit)
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function Data_WZ_StructuresINI(ByVal File As clsINIWrite, ByVal PlayerCount As Integer) As clsResult
        Dim ReturnResult As New clsResult

        Dim tmpStructure As clsStructureType
        Dim EndChar As Char = Chr(10)
        Dim tmpUnit As clsUnit
        Dim A As Integer
        Dim UnitIsModule(UnitCount - 1) As Boolean
        Dim UnitModuleCount(UnitCount - 1) As Integer
        Dim SectorNum As sXY_int
        Dim OtherStructureType As clsStructureType
        Dim OtherUnit As clsUnit
        Dim ModuleMin As sXY_int
        Dim ModuleMax As sXY_int
        Dim Footprint As sXY_int
        Dim B As Integer
        Dim C As Integer
        Dim UnderneathTypes(1) As clsStructureType.enumStructureType
        Dim UnderneathTypeCount As Integer
        Dim BadModuleCount As Integer = 0

        For A = 0 To UnitCount - 1
            tmpUnit = Units(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                tmpStructure = CType(tmpUnit.Type, clsStructureType)
                Select Case tmpStructure.StructureType
                    Case clsStructureType.enumStructureType.FactoryModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.Factory
                        UnderneathTypes(1) = clsStructureType.enumStructureType.VTOLFactory
                        UnderneathTypeCount = 2
                    Case clsStructureType.enumStructureType.PowerModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.PowerGenerator
                        UnderneathTypeCount = 1
                    Case clsStructureType.enumStructureType.ResearchModule
                        UnderneathTypes(0) = clsStructureType.enumStructureType.Research
                        UnderneathTypeCount = 1
                    Case Else
                        UnderneathTypeCount = 0
                End Select
                If UnderneathTypeCount > 0 Then
                    UnitIsModule(A) = True
                    SectorNum = GetPosSectorNum(tmpUnit.Pos.Horizontal)
                    For B = 0 To Sectors(SectorNum.X, SectorNum.Y).UnitCount - 1
                        OtherUnit = Sectors(SectorNum.X, SectorNum.Y).Units(B)
                        If OtherUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                            OtherStructureType = CType(OtherUnit.Type, clsStructureType)
                            If OtherUnit.UnitGroup Is tmpUnit.UnitGroup Then
                                For C = 0 To UnderneathTypeCount - 1
                                    If OtherStructureType.StructureType = UnderneathTypes(C) Then
                                        Exit For
                                    End If
                                Next
                                If C < UnderneathTypeCount Then
                                    Footprint = OtherStructureType.GetFootprint
                                    ModuleMin.X = OtherUnit.Pos.Horizontal.X - CInt(Footprint.X * TerrainGridSpacing / 2.0#)
                                    ModuleMin.Y = OtherUnit.Pos.Horizontal.Y - CInt(Footprint.Y * TerrainGridSpacing / 2.0#)
                                    ModuleMax.X = OtherUnit.Pos.Horizontal.X + CInt(Footprint.X * TerrainGridSpacing / 2.0#)
                                    ModuleMax.Y = OtherUnit.Pos.Horizontal.Y + CInt(Footprint.Y * TerrainGridSpacing / 2.0#)
                                    If tmpUnit.Pos.Horizontal.X >= ModuleMin.X And tmpUnit.Pos.Horizontal.X < ModuleMax.X And _
                                      tmpUnit.Pos.Horizontal.Y >= ModuleMin.Y And tmpUnit.Pos.Horizontal.Y < ModuleMax.Y Then
                                        UnitModuleCount(OtherUnit.Map_UnitNum) += 1
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                    Next
                    If B = Sectors(SectorNum.X, SectorNum.Y).UnitCount Then
                        BadModuleCount += 1
                    End If
                End If
            End If
        Next

        If BadModuleCount > 0 Then
            ReturnResult.Warning_Add(BadModuleCount & " modules had no underlying structure.")
        End If

        Dim TooManyModulesWarningCount As Integer
        Dim TooManyModulesWarningMaxCount As Integer = 16
        Dim ModuleCount As Integer
        Dim ModuleLimit As Integer

        For A = 0 To UnitCount - 1
            tmpUnit = Units(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure And Not UnitIsModule(A) Then
                tmpStructure = CType(tmpUnit.Type, clsStructureType)
                If tmpUnit.ID <= 0 Then
                    ReturnResult.Warning_Add("Error. A structure's ID was zero. It was NOT saved. Delete and replace it to allow save.")
                Else
                    File.SectionName_Append("structure_" & tmpUnit.ID)
                    File.Property_Append("id", CStr(tmpUnit.ID))
                    If tmpUnit.UnitGroup Is ScavengerUnitGroup Or (PlayerCount >= 0 And tmpUnit.UnitGroup.WZ_StartPos >= PlayerCount) Then
                        File.Property_Append("player", "scavenger")
                    Else
                        File.Property_Append("startpos", CStr(tmpUnit.UnitGroup.WZ_StartPos))
                    End If
                    File.Property_Append("name", tmpStructure.Code)
                    File.Property_Append("position", tmpUnit.Pos.Horizontal.X & ", " & tmpUnit.Pos.Horizontal.Y & ", 0")
                    File.Property_Append("rotation", tmpUnit.GetINIRotation)
                    If tmpUnit.Health < 1.0# Then
                        File.Property_Append("health", CInt(Clamp_dbl(tmpUnit.Health * 100.0#, 1.0#, 100.0#)) & "%")
                    End If
                    Select Case tmpStructure.StructureType
                        Case clsStructureType.enumStructureType.Factory
                            ModuleLimit = 2
                        Case clsStructureType.enumStructureType.VTOLFactory
                            ModuleLimit = 2
                        Case clsStructureType.enumStructureType.PowerGenerator
                            ModuleLimit = 1
                        Case clsStructureType.enumStructureType.Research
                            ModuleLimit = 1
                        Case Else
                            ModuleLimit = 0
                    End Select
                    If UnitModuleCount(A) > ModuleLimit Then
                        ModuleCount = ModuleLimit
                        If TooManyModulesWarningCount < TooManyModulesWarningMaxCount Then
                            ReturnResult.Warning_Add("Structure " & tmpStructure.GetDisplayText & " at " & tmpUnit.GetPosText & " has too many modules (" & UnitModuleCount(A) & ").")
                        End If
                        TooManyModulesWarningCount += 1
                    Else
                        ModuleCount = UnitModuleCount(A)
                    End If
                    File.Property_Append("modules", CStr(UnitModuleCount(A)))
                    File.Gap_Append()
                End If
            End If
        Next

        If TooManyModulesWarningCount > TooManyModulesWarningMaxCount Then
            ReturnResult.Warning_Add(TooManyModulesWarningCount & " structures had too many modules.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_WZ_DroidsINI(ByVal File As clsINIWrite, ByVal PlayerCount As Integer) As clsResult
        Dim ReturnResult As New clsResult

        Dim tmpDroid As clsDroidDesign
        Dim tmpTemplate As clsDroidTemplate
        Dim Text As String
        Dim EndChar As Char = Chr(10)
        Dim tmpUnit As clsUnit
        Dim AsPartsNotTemplate As Boolean
        Dim ValidDroid As Boolean
        Dim InvalidPartCount As Integer = 0
        Dim A As Integer
        Dim tmpBrain As clsBrain

        For A = 0 To UnitCount - 1
            tmpUnit = Units(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.PlayerDroid Then
                tmpDroid = CType(tmpUnit.Type, clsDroidDesign)
                ValidDroid = True
                If tmpUnit.ID <= 0 Then
                    ValidDroid = False
                    ReturnResult.Warning_Add("Error. A droid's ID was zero. It was NOT saved. Delete and replace it to allow save.")
                End If
                If tmpDroid.Body Is Nothing Then
                    ValidDroid = False
                    InvalidPartCount += 1
                ElseIf tmpDroid.Propulsion Is Nothing Then
                    ValidDroid = False
                    InvalidPartCount += 1
                ElseIf tmpDroid.TurretCount >= 1 Then
                    If tmpDroid.Turret1 Is Nothing Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    End If
                ElseIf tmpDroid.TurretCount >= 2 Then
                    If tmpDroid.Turret2 Is Nothing Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    ElseIf tmpDroid.Turret2.TurretType <> clsTurret.enumTurretType.Weapon Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    End If
                ElseIf tmpDroid.TurretCount >= 3 And tmpDroid.Turret3 Is Nothing Then
                    If tmpDroid.Turret3 Is Nothing Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    ElseIf tmpDroid.Turret3.TurretType <> clsTurret.enumTurretType.Weapon Then
                        ValidDroid = False
                        InvalidPartCount += 1
                    End If
                End If
                If ValidDroid Then
                    File.SectionName_Append("droid_" & tmpUnit.ID)
                    File.Property_Append("id", CStr(tmpUnit.ID))
                    If tmpUnit.UnitGroup Is ScavengerUnitGroup Or (PlayerCount >= 0 And tmpUnit.UnitGroup.WZ_StartPos >= PlayerCount) Then
                        File.Property_Append("player", "scavenger")
                    Else
                        File.Property_Append("startpos", CStr(tmpUnit.UnitGroup.WZ_StartPos))
                    End If
                    If tmpDroid.IsTemplate Then
                        tmpTemplate = CType(tmpDroid, clsDroidTemplate)
                        AsPartsNotTemplate = tmpUnit.PreferPartsOutput
                    Else
                        AsPartsNotTemplate = True
                    End If
                    If AsPartsNotTemplate Then
                        File.Property_Append("name", tmpDroid.GenerateName)
                    Else
                        tmpTemplate = CType(tmpDroid, clsDroidTemplate)
                        File.Property_Append("template", tmpTemplate.Code)
                    End If
                    File.Property_Append("position", tmpUnit.Pos.Horizontal.X & ", " & tmpUnit.Pos.Horizontal.Y & ", 0")
                    File.Property_Append("rotation", tmpUnit.GetINIRotation)
                    If tmpUnit.Health < 1.0# Then
                        File.Property_Append("health", CInt(Clamp_dbl(tmpUnit.Health * 100.0#, 1.0#, 100.0#)) & "%")
                    End If
                    If AsPartsNotTemplate Then
                        File.Property_Append("droidType", CStr(CInt(tmpDroid.GetDroidType)))
                        If tmpDroid.TurretCount = 0 Then
                            Text = "0"
                        Else
                            If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                                If CType(tmpDroid.Turret1, clsBrain).Weapon Is Nothing Then
                                    Text = "0"
                                Else
                                    Text = "1"
                                End If
                            Else
                                If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
                                    Text = CStr(tmpDroid.TurretCount)
                                Else
                                    Text = "0"
                                End If
                            End If
                        End If
                        File.Property_Append("weapons", Text)
                        File.Property_Append("parts\body", tmpDroid.Body.Code)
                        File.Property_Append("parts\propulsion", tmpDroid.Propulsion.Code)
                        File.Property_Append("parts\sensor", tmpDroid.GetSensorCode)
                        File.Property_Append("parts\construct", tmpDroid.GetConstructCode)
                        File.Property_Append("parts\repair", tmpDroid.GetRepairCode)
                        File.Property_Append("parts\brain", tmpDroid.GetBrainCode)
                        File.Property_Append("parts\ecm", tmpDroid.GetECMCode)
                        If tmpDroid.TurretCount >= 1 Then
                            If tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
                                File.Property_Append("parts\weapon\1", tmpDroid.Turret1.Code)
                                If tmpDroid.TurretCount >= 2 Then
                                    If tmpDroid.Turret2.TurretType = clsTurret.enumTurretType.Weapon Then
                                        File.Property_Append("parts\weapon\2", tmpDroid.Turret2.Code)
                                        If tmpDroid.TurretCount >= 3 Then
                                            If tmpDroid.Turret3.TurretType = clsTurret.enumTurretType.Weapon Then
                                                File.Property_Append("parts\weapon\3", tmpDroid.Turret3.Code)
                                            End If
                                        End If
                                    End If
                                End If
                            ElseIf tmpDroid.Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                                tmpBrain = CType(tmpDroid.Turret1, clsBrain)
                                If tmpBrain.Weapon Is Nothing Then
                                    Text = "ZNULLWEAPON"
                                Else
                                    Text = tmpBrain.Weapon.Code
                                End If
                                File.Property_Append("parts\weapon\1", Text)
                            End If
                        End If
                    End If
                    File.Gap_Append()
                End If
            End If
        Next

        If InvalidPartCount > 0 Then
            ReturnResult.Warning_Add("There were " & InvalidPartCount & " droids with parts missing. They were not saved.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_WZ_FeaturesINI(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult
        Dim tmpFeature As clsFeatureType
        Dim EndChar As Char = Chr(10)
        Dim tmpUnit As clsUnit
        Dim Valid As Boolean
        Dim A As Integer

        For A = 0 To UnitCount - 1
            tmpUnit = Units(A)
            If tmpUnit.Type.Type = clsUnitType.enumType.Feature Then
                tmpFeature = CType(tmpUnit.Type, clsFeatureType)
                Valid = True
                If tmpUnit.ID <= 0 Then
                    Valid = False
                    ReturnResult.Warning_Add("Error. A features's ID was zero. It was NOT saved. Delete and replace it to allow save.")
                End If
                If Valid Then
                    File.SectionName_Append("feature_" & tmpUnit.ID)
                    File.Property_Append("id", CStr(tmpUnit.ID))
                    File.Property_Append("position", tmpUnit.Pos.Horizontal.X & ", " & tmpUnit.Pos.Horizontal.Y & ", 0")
                    File.Property_Append("rotation", tmpUnit.GetINIRotation)
                    File.Property_Append("name", tmpFeature.Code)
                    If tmpUnit.Health < 1.0# Then
                        File.Property_Append("health", CInt(Clamp_dbl(tmpUnit.Health * 100.0#, 1.0#, 100.0#)) & "%")
                    End If
                    File.Gap_Append()
                End If
            End If
        Next

        Return ReturnResult
    End Function

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

    Public Function Write_WZ(ByVal Args As sWrite_WZ_Args) As clsResult
        Dim ReturnResult As New clsResult

        Try

            Select Case Args.CompileType
                Case sWrite_WZ_Args.enumCompileType.Multiplayer
                    If Args.Multiplayer Is Nothing Then
                        ReturnResult.Problem_Add("Multiplayer arguments were not passed.")
                        Return ReturnResult
                    End If
                    If Args.Multiplayer.PlayerCount < 2 Or Args.Multiplayer.PlayerCount > 10 Then
                        ReturnResult.Problem_Add("Number of players was below 2 or above 10.")
                        Return ReturnResult
                    End If
                    If Not Args.Multiplayer.IsBetaPlayerFormat Then
                        If Not (Args.Multiplayer.PlayerCount = 2 Or Args.Multiplayer.PlayerCount = 4 Or Args.Multiplayer.PlayerCount = 8) Then
                            ReturnResult.Problem_Add("Number of players was not 2, 4 or 8 in original format.")
                            Return ReturnResult
                        End If
                    End If
                Case sWrite_WZ_Args.enumCompileType.Campaign
                    If Args.Campaign Is Nothing Then
                        ReturnResult.Problem_Add("Campaign arguments were not passed.")
                        Return ReturnResult
                    End If
                Case Else
                    ReturnResult.Problem_Add("Unknown compile method.")
                    Return ReturnResult
            End Select

            If Not Args.Overwrite Then
                If IO.File.Exists(Args.Path) Then
                    ReturnResult.Problem_Add("The selected file already exists.")
                    Return ReturnResult
                End If
            End If

            Dim Quote As Char = ControlChars.Quote
            Dim EndChar As Char = Chr(10)
            Dim Text As String

            Dim File_LEV As New clsWriteFile
            Dim File_MAP As New clsWriteFile
            Dim File_GAM As New clsWriteFile
            Dim File_featBJO As New clsWriteFile
            Dim INI_feature As clsINIWrite = CreateINIWriteFile()
            Dim File_TTP As New clsWriteFile
            Dim File_structBJO As New clsWriteFile
            Dim INI_struct As clsINIWrite = CreateINIWriteFile()
            Dim File_droidBJO As New clsWriteFile
            Dim INI_droid As clsINIWrite = CreateINIWriteFile()

            Dim PlayersPrefix As String = ""

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                PlayersPrefix = Args.Multiplayer.PlayerCount & "c-"
                Dim fog As String
                Dim TilesetNum As String
                If Tileset Is Nothing Then
                    ReturnResult.Problem_Add("Map must have a tileset.")
                    Return ReturnResult
                ElseIf Tileset Is Tileset_Arizona Then
                    fog = "fog1.wrf"
                    TilesetNum = "1"
                ElseIf Tileset Is Tileset_Urban Then
                    fog = "fog2.wrf"
                    TilesetNum = "2"
                ElseIf Tileset Is Tileset_Rockies Then
                    fog = "fog3.wrf"
                    TilesetNum = "3"
                Else
                    ReturnResult.Problem_Add("Unknown tileset selected.")
                    Return ReturnResult
                End If

                Text = "// Made with " & ProgramName & " " & ProgramVersionNumber & " " & ProgramPlatform & EndChar
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

            File_GAM.U8_Append(Asc("g"c))
            File_GAM.U8_Append(Asc("a"c))
            File_GAM.U8_Append(Asc("m"c))
            File_GAM.U8_Append(Asc("e"c))
            File_GAM.U32_Append(8)
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                File_GAM.U32_Append(0)
                File_GAM.U32_Append(0)
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

            File_MAP.U8_Append(Asc("m"c))
            File_MAP.U8_Append(Asc("a"c))
            File_MAP.U8_Append(Asc("p"c))
            File_MAP.U8_Append(Asc(" "c))
            File_MAP.U32_Append(10)
            File_MAP.U32_Append(CUInt(Terrain.TileSize.X))
            File_MAP.U32_Append(CUInt(Terrain.TileSize.Y))
            Dim Flip As Byte
            Dim Rotation As Byte
            Dim DoFlipX As Boolean
            Dim InvalidTileCount As Integer
            Dim TextureNum As Integer
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    TileOrientation_To_OldOrientation(Terrain.Tiles(X, Y).Texture.Orientation, Rotation, DoFlipX)
                    Flip = 0
                    If Terrain.Tiles(X, Y).Tri Then
                        Flip += CByte(8)
                    End If
                    Flip += CByte(Rotation * 16)
                    If DoFlipX Then
                        Flip += CByte(128)
                    End If
                    TextureNum = Terrain.Tiles(X, Y).Texture.TextureNum
                    If TextureNum < 0 Or TextureNum > 255 Then
                        TextureNum = 0
                        If InvalidTileCount < 16 Then
                            ReturnResult.Warning_Add("Tile texture number " & Terrain.Tiles(X, Y).Texture.TextureNum & " is invalid on tile " & X & ", " & Y & " and was compiled as texture number " & TextureNum & ".")
                        End If
                        InvalidTileCount += 1
                    End If
                    File_MAP.U8_Append(CByte(TextureNum))
                    File_MAP.U8_Append(Flip)
                    File_MAP.U8_Append(Terrain.Vertices(X, Y).Height)
                Next
            Next
            If InvalidTileCount > 0 Then
                ReturnResult.Warning_Add(InvalidTileCount & " tile texture numbers were invalid.")
            End If
            File_MAP.U32_Append(1) 'gateway version
            File_MAP.U32_Append(CUInt(GatewayCount))
            For A = 0 To GatewayCount - 1
                File_MAP.U8_Append(CByte(Clamp_int(Gateways(A).PosA.X, 0, 255)))
                File_MAP.U8_Append(CByte(Clamp_int(Gateways(A).PosA.Y, 0, 255)))
                File_MAP.U8_Append(CByte(Clamp_int(Gateways(A).PosB.X, 0, 255)))
                File_MAP.U8_Append(CByte(Clamp_int(Gateways(A).PosB.Y, 0, 255)))
            Next

            Dim tmpFeature As clsFeatureType
            Dim tmpStructure As clsStructureType
            Dim tmpDroid As clsDroidDesign
            Dim tmpTemplate As clsDroidTemplate

            File_featBJO.U8_Append(Asc("f"c))
            File_featBJO.U8_Append(Asc("e"c))
            File_featBJO.U8_Append(Asc("a"c))
            File_featBJO.U8_Append(Asc("t"c))
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
            File_featBJO.U32_Append(CUInt(FeatureCount))
            For B = 0 To FeatureCount - 1
                A = Features(B)
                tmpFeature = CType(Units(A).Type, clsFeatureType)
                File_featBJO.Text_Append(tmpFeature.Code, 40)
                File_featBJO.U32_Append(Units(A).ID)
                File_featBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.X))
                File_featBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.Y))
                File_featBJO.U32_Append(CUInt(Units(A).Pos.Altitude))
                File_featBJO.U32_Append(CUInt(Units(A).Rotation))
                If Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                    If Units(A).UnitGroup.WZ_StartPos >= 0 Then
                        File_featBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    Else
                        File_featBJO.U32_Append(7UI)
                    End If
                ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                    If Units(A).UnitGroup Is ScavengerUnitGroup Or Units(A).UnitGroup.WZ_StartPos >= Args.Multiplayer.PlayerCount Then
                        File_featBJO.U32_Append(CUInt(Math.Max(7, Args.Multiplayer.PlayerCount)))
                    Else
                        File_featBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    End If
                Else
                    Stop
                End If
                File_featBJO.Make_Length(12)
            Next

            File_TTP.Text_Append("ttyp")
            File_TTP.U32_Append(8UI)
            File_TTP.U32_Append(CUInt(Tileset.TileCount))
            For A = 0 To Tileset.TileCount - 1
                File_TTP.U16_Append(Tile_TypeNum(A))
            Next

            File_structBJO.U8_Append(Asc("s"c))
            File_structBJO.U8_Append(Asc("t"c))
            File_structBJO.U8_Append(Asc("r"c))
            File_structBJO.U8_Append(Asc("u"c))
            File_structBJO.U32_Append(8)
            Dim StructureOrder(UnitCount - 1) As Integer
            Dim StructureCount As Integer = 0
            'non-module structures
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(Units(A).Type, clsStructureType)
                    If Not (tmpStructure.StructureType = clsStructureType.enumStructureType.FactoryModule _
                        Or tmpStructure.StructureType = clsStructureType.enumStructureType.PowerModule _
                        Or tmpStructure.StructureType = clsStructureType.enumStructureType.ResearchModule) Then
                        For B = 0 To StructureCount - 1
                            If Units(StructureOrder(B)).SavePriority < Units(A).SavePriority Then
                                Exit For
                            End If
                        Next
                        For C = StructureCount - 1 To B Step -1
                            StructureOrder(C + 1) = StructureOrder(C)
                        Next
                        StructureOrder(B) = A
                        StructureCount += 1
                    End If
                End If
            Next
            'module structures
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(Units(A).Type, clsStructureType)
                    If tmpStructure.StructureType = clsStructureType.enumStructureType.FactoryModule _
                            Or tmpStructure.StructureType = clsStructureType.enumStructureType.PowerModule _
                        Or tmpStructure.StructureType = clsStructureType.enumStructureType.ResearchModule Then
                        For B = 0 To StructureCount - 1
                            If Units(StructureOrder(B)).SavePriority < Units(A).SavePriority Then
                                Exit For
                            End If
                        Next
                        For C = StructureCount - 1 To B Step -1
                            StructureOrder(C + 1) = StructureOrder(C)
                        Next
                        StructureOrder(B) = A
                        StructureCount += 1
                    End If
                End If
            Next
            File_structBJO.U32_Append(CUInt(StructureCount))
            For B = 0 To StructureCount - 1
                A = StructureOrder(B)
                tmpStructure = CType(Units(A).Type, clsStructureType)
                File_structBJO.Text_Append(tmpStructure.Code, 40)
                File_structBJO.U32_Append(Units(A).ID)
                File_structBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.X))
                File_structBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.Y))
                File_structBJO.U32_Append(CUInt(Units(A).Pos.Altitude))
                File_structBJO.U32_Append(CUInt(Units(A).Rotation))
                If Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                    If Units(A).UnitGroup.WZ_StartPos >= 0 Then
                        File_structBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    Else
                        File_structBJO.U32_Append(7UI)
                    End If
                ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                    If Units(A).UnitGroup Is ScavengerUnitGroup Or Units(A).UnitGroup.WZ_StartPos >= Args.Multiplayer.PlayerCount Then
                        File_structBJO.U32_Append(CUInt(Math.Max(7, Args.Multiplayer.PlayerCount)))
                    Else
                        File_structBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    End If
                Else
                    Stop
                End If
                File_structBJO.Make_Length(12)
                File_structBJO.U8_Append(1)
                File_structBJO.U8_Append(26)
                File_structBJO.U8_Append(127)
                File_structBJO.U8_Append(0)
                File_structBJO.Make_Length(40)
            Next

            File_droidBJO.U8_Append(Asc("d"c))
            File_droidBJO.U8_Append(Asc("i"c))
            File_droidBJO.U8_Append(Asc("n"c))
            File_droidBJO.U8_Append(Asc("t"c))
            File_droidBJO.U32_Append(8)
            Dim Droids(UnitCount - 1) As Integer
            Dim DroidCount As Integer = 0
            For A = 0 To UnitCount - 1
                If Units(A).Type.Type = clsUnitType.enumType.PlayerDroid Then
                    tmpDroid = CType(Units(A).Type, clsDroidDesign)
                    If tmpDroid.IsTemplate Then
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
                End If
            Next
            File_droidBJO.U32_Append(CUInt(DroidCount))
            For B = 0 To DroidCount - 1
                A = Droids(B)
                tmpTemplate = CType(Units(A).Type, clsDroidTemplate)
                File_droidBJO.Text_Append(tmpTemplate.Code, 40)
                File_droidBJO.U32_Append(Units(A).ID)
                File_droidBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.X))
                File_droidBJO.U32_Append(CUInt(Units(A).Pos.Horizontal.Y))
                File_droidBJO.U32_Append(CUInt(Units(A).Pos.Altitude))
                File_droidBJO.U32_Append(CUInt(Units(A).Rotation))
                If Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                    If Units(A).UnitGroup.WZ_StartPos >= 0 Then
                        File_droidBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    Else
                        File_droidBJO.U32_Append(7UI)
                    End If
                ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                    If Units(A).UnitGroup Is ScavengerUnitGroup Or Units(A).UnitGroup.WZ_StartPos >= Args.Multiplayer.PlayerCount Then
                        File_droidBJO.U32_Append(CUInt(Math.Max(7, Args.Multiplayer.PlayerCount)))
                    Else
                        File_droidBJO.U32_Append(CUInt(Units(A).UnitGroup.WZ_StartPos))
                    End If
                Else
                    Stop
                End If
                File_droidBJO.Make_Length(12)
            Next

            ReturnResult.Append(Data_WZ_FeaturesINI(INI_feature), "Features INI: ")
            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then
                ReturnResult.Append(Data_WZ_StructuresINI(INI_struct, Args.Multiplayer.PlayerCount), "Structures INI: ")
                ReturnResult.Append(Data_WZ_DroidsINI(INI_droid, Args.Multiplayer.PlayerCount), "Droids INI: ")
            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then
                ReturnResult.Append(Data_WZ_StructuresINI(INI_struct, -1), "Structures INI: ")
                ReturnResult.Append(Data_WZ_DroidsINI(INI_droid, -1), "Droids INI: ")
            End If

            If Args.CompileType = sWrite_WZ_Args.enumCompileType.Multiplayer Then

                If Not Args.Overwrite Then
                    If IO.File.Exists(Args.Path) Then
                        ReturnResult.Problem_Add("A file already exists at: " & Args.Path)
                        Return ReturnResult
                    End If
                Else
                    If IO.File.Exists(Args.Path) Then
                        Try
                            IO.File.Delete(Args.Path)
                        Catch ex As Exception
                            ReturnResult.Problem_Add("Unable to delete existing file: " & ex.Message)
                            Return ReturnResult
                        End Try
                    End If
                End If

                Dim WZStream As Zip.ZipOutputStream = New Zip.ZipOutputStream(IO.File.Create(Args.Path))

                Try

                    Dim ZippedFile As Zip.ZipEntry
                    Dim ZipPath As String

                    WZStream.SetLevel(9)

                    If Args.Multiplayer.IsBetaPlayerFormat Then
                        ZipPath = PlayersPrefix & Args.MapName & ".xplayers.lev"
                    Else
                        ZipPath = PlayersPrefix & Args.MapName & ".addon.lev"
                    End If
                    ReturnResult.Append(File_LEV.WriteToZip(WZStream, ZipPath), "Zip lev file: ")

                    ZippedFile = New Zip.ZipEntry("multiplay/")
                    WZStream.PutNextEntry(ZippedFile)
                    ZippedFile = New Zip.ZipEntry("multiplay/maps/")
                    WZStream.PutNextEntry(ZippedFile)
                    ZippedFile = New Zip.ZipEntry("multiplay/maps/" & PlayersPrefix & Args.MapName & "/")
                    WZStream.PutNextEntry(ZippedFile)

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & ".gam"
                    ReturnResult.Append(File_GAM.WriteToZip(WZStream, ZipPath), "Zip game file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "dinit.bjo"
                    ReturnResult.Append(File_droidBJO.WriteToZip(WZStream, ZipPath), "Zip droid bjo file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "droid.ini"
                    ReturnResult.Append(INI_droid.File.WriteToZip(WZStream, ZipPath), "Zip droid ini file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "feat.bjo"
                    ReturnResult.Append(File_featBJO.WriteToZip(WZStream, ZipPath), "Zip feature bjo file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "feature.ini"
                    ReturnResult.Append(INI_feature.File.WriteToZip(WZStream, ZipPath), "Zip feature ini file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "game.map"
                    ReturnResult.Append(File_MAP.WriteToZip(WZStream, ZipPath), "Zip map file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "struct.bjo"
                    ReturnResult.Append(File_structBJO.WriteToZip(WZStream, ZipPath), "Zip structure bjo file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "struct.ini"
                    ReturnResult.Append(INI_struct.File.WriteToZip(WZStream, ZipPath), "Zip structure ini file: ")

                    ZipPath = "multiplay/maps/" & PlayersPrefix & Args.MapName & "/" & "ttypes.ttp"
                    ReturnResult.Append(File_TTP.WriteToZip(WZStream, ZipPath), "Zip tile types file: ")

                    If ReturnResult.HasProblems Then
                        WZStream.Close()
                        Return ReturnResult
                    End If

                    WZStream.Finish()
                Catch ex As Exception
                    WZStream.Close()
                    ReturnResult.Problem_Add(ex.Message)
                    Return ReturnResult
                Finally
                    WZStream.Close()
                End Try

            ElseIf Args.CompileType = sWrite_WZ_Args.enumCompileType.Campaign Then

                Dim tmpPath As String = EndWithPathSeperator(Args.Path)

                If Not IO.Directory.Exists(tmpPath) Then
                    ReturnResult.Problem_Add("Directory " & tmpPath & " does not exist.")
                    Return ReturnResult
                End If

                Dim tmpFilePath As String
                tmpFilePath = tmpPath & Args.MapName & ".gam"
                ReturnResult.Append(File_GAM.WriteFile(tmpFilePath, False), "Write game file: ")

                tmpPath &= Args.MapName & OSPathSeperator
                Try
                    IO.Directory.CreateDirectory(tmpPath)
                Catch ex As Exception
                    ReturnResult.Problem_Add("Unable to create directory " & tmpPath)
                    Return ReturnResult
                End Try

                tmpFilePath = tmpPath & "dinit.bjo"
                ReturnResult.Append(File_droidBJO.WriteFile(tmpFilePath, False), "Write droids bjo file: ")

                tmpFilePath = tmpPath & "droid.ini"
                ReturnResult.Append(INI_droid.File.WriteFile(tmpFilePath, False), "Write droids ini file: ")

                tmpFilePath = tmpPath & "feat.bjo"
                ReturnResult.Append(File_featBJO.WriteFile(tmpFilePath, False), "Write features bjo file: ")

                tmpFilePath = tmpPath & "feature.ini"
                ReturnResult.Append(INI_feature.File.WriteFile(tmpFilePath, False), "Write features ini file: ")

                tmpFilePath = tmpPath & "game.map"
                ReturnResult.Append(File_MAP.WriteFile(tmpFilePath, False), "Write map file: ")

                tmpFilePath = tmpPath & "struct.bjo"
                ReturnResult.Append(File_structBJO.WriteFile(tmpFilePath, False), "Write structures bjo file: ")

                tmpFilePath = tmpPath & "struct.ini"
                ReturnResult.Append(INI_struct.File.WriteFile(tmpFilePath, False), "Write structures ini file: ")

                tmpFilePath = tmpPath & "ttypes.ttp"
                ReturnResult.Append(File_TTP.WriteFile(tmpFilePath, False), "Write tile types file: ")
            End If

        Catch ex As Exception
            Stop
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        Return ReturnResult
    End Function

    Private Function Read_TTP(ByVal File As clsReadFile) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim strTemp As String = ""
        Dim uintTemp As UInteger
        Dim ushortTemp As UShort
        Dim A As Integer

        If Not File.Get_Text(4, strTemp) Then
            ReturnResult.Problem = "Unable to read identifier."
            Return ReturnResult
        End If
        If strTemp <> "ttyp" Then
            ReturnResult.Problem = "Incorrect identifier."
            Return ReturnResult
        End If
        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Unable to read version."
            Return ReturnResult
        End If
        If uintTemp <> 8UI Then
            ReturnResult.Problem = "Unknown version."
            Return ReturnResult
        End If
        If Not File.Get_U32(uintTemp) Then
            ReturnResult.Problem = "Unable to read tile count."
            Return ReturnResult
        End If
        For A = 0 To CInt(Math.Min(uintTemp, CUInt(Tileset.TileCount))) - 1
            If Not File.Get_U16(ushortTemp) Then
                ReturnResult.Problem = "Unable to read tile type number."
                Return ReturnResult
            End If
            If ushortTemp > 11 Then
                ReturnResult.Problem = "Unknown tile type number."
                Return ReturnResult
            End If
            Tile_TypeNum(A) = CByte(ushortTemp)
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function
End Class