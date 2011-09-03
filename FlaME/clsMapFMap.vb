Imports ICSharpCode.SharpZipLib

Partial Public Class clsMap

    Public Class clsFMap_INIObjects
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sObject
            Public ID As UInteger
            Public Type As clsUnitType.enumType
            Public IsTemplate As Boolean
            Public Code As String
            Public UnitGroup As String
            Public GotAltitude As Boolean
            Public Pos As clsXY_int
            Public Heading As Double
            Public Health As Double
            Public TemplateDroidType As clsDroidDesign.clsTemplateDroidType
            Public BodyCode As String
            Public PropulsionCode As String
            Public TurretTypes() As clsTurret.enumTurretType
            Public TurretCodes() As String
            Public TurretCount As Integer
            Public Priority As Integer
        End Structure
        Public Objects() As sObject
        Public ObjectCount As Integer

        Public Sub New(ByVal NewObjectCount As Integer)
            Dim A As Integer
            Dim B As Integer

            ObjectCount = NewObjectCount
            ReDim Objects(ObjectCount - 1)
            For A = 0 To ObjectCount - 1
                Objects(A).Type = clsUnitType.enumType.Unspecified
                Objects(A).Health = 1.0#
                ReDim Objects(A).TurretCodes(MaxDroidWeapons - 1)
                ReDim Objects(A).TurretTypes(MaxDroidWeapons - 1)
                For B = 0 To MaxDroidWeapons - 1
                    Objects(A).TurretTypes(B) = clsTurret.enumTurretType.Unknown
                Next
            Next
        End Sub

        Public Overrides Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "type"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 1 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Select Case CommaText(0).ToLower
                        Case "feature"
                            Objects(INISectionNum).Type = clsUnitType.enumType.Feature
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "structure"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerStructure
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "droidtemplate"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerDroid
                            Objects(INISectionNum).IsTemplate = True
                            Objects(INISectionNum).Code = CommaText(1)
                        Case "droiddesign"
                            Objects(INISectionNum).Type = clsUnitType.enumType.PlayerDroid
                        Case Else
                            Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Select
                Case "droidtype"
                    Dim tmpType As clsDroidDesign.clsTemplateDroidType = GetTemplateDroidTypeFromTemplateCode(INIProperty.Value)
                    If tmpType Is Nothing Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).TemplateDroidType = tmpType
                Case "body"
                    Objects(INISectionNum).BodyCode = INIProperty.Value
                Case "propulsion"
                    Objects(INISectionNum).PropulsionCode = INIProperty.Value
                Case "turretcount"
                    Dim NewTurretCount As Integer
                    Try
                        NewTurretCount = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If NewTurretCount < 0 Or NewTurretCount > MaxDroidWeapons Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).TurretCount = NewTurretCount
                Case "turret1"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim tmpTurretType As clsTurret.enumTurretType
                    tmpTurretType = GetTurretTypeFromName(CommaText(0))
                    If tmpTurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(0) = tmpTurretType
                        Objects(INISectionNum).TurretCodes(0) = CommaText(1)
                    End If
                Case "turret2"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim tmpTurretType As clsTurret.enumTurretType
                    tmpTurretType = GetTurretTypeFromName(CommaText(0))
                    If tmpTurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(1) = tmpTurretType
                        Objects(INISectionNum).TurretCodes(1) = CommaText(1)
                    End If
                Case "turret3"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Dim tmpTurretType As clsTurret.enumTurretType
                    tmpTurretType = GetTurretTypeFromName(CommaText(0))
                    If tmpTurretType <> clsTurret.enumTurretType.Unknown Then
                        Objects(INISectionNum).TurretTypes(2) = tmpTurretType
                        Objects(INISectionNum).TurretCodes(2) = CommaText(1)
                    End If
                Case "id"
                    Try
                        Objects(INISectionNum).ID = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "priority"
                    Try
                        Objects(INISectionNum).Priority = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "pos"
                    Dim CommaText() As String
                    Dim CommaTextCount As Integer
                    Dim A As Integer
                    CommaText = INIProperty.Value.Split(","c)
                    CommaTextCount = CommaText.GetUpperBound(0) + 1
                    If CommaTextCount < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    For A = 0 To CommaTextCount - 1
                        CommaText(A) = CommaText(A).Trim()
                    Next
                    Try
                        Objects(INISectionNum).Pos = New clsXY_int(New sXY_int(CInt(CommaText(0)), CInt(CommaText(1))))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "heading"
                    Dim dblTemp As Double
                    Try
                        dblTemp = CDbl(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If dblTemp < 0.0# Or dblTemp >= 360.0# Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).Heading = dblTemp
                Case "unitgroup"
                    Objects(INISectionNum).UnitGroup = INIProperty.Value
                Case "health"
                    Dim NewHealth As Double
                    Try
                        NewHealth = CDbl(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If NewHealth < 0.0# Or NewHealth >= 1.0# Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).Health = NewHealth
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Write_FMap(ByVal Path As String, ByVal Overwrite As Boolean, ByVal Compress As Boolean) As clsResult
        Dim ReturnResult As New clsResult

        If Not Overwrite Then
            If IO.File.Exists(Path) Then
                ReturnResult.Problem_Add("A file already exists at " & Path)
                Return ReturnResult
            End If
        End If

        Dim INI_Info As clsINIWrite = CreateINIWriteFile()
        Data_FMap_Info(INI_Info)

        Dim File_VertexHeight As New clsWriteFile
        Data_FMap_VertexHeight(File_VertexHeight)

        Dim File_VertexTerrain As New clsWriteFile
        ReturnResult.Append(Data_FMap_VertexTerrain(File_VertexTerrain), "Vertex terrain file: ")

        Dim File_TileTexture As New clsWriteFile
        ReturnResult.Append(Data_FMap_TileTexture(File_TileTexture), "Tile textures file: ")

        Dim File_TileOrientation As New clsWriteFile
        Data_FMap_TileOrientation(File_TileOrientation)

        Dim File_TileCliff As New clsWriteFile
        ReturnResult.Append(Data_FMap_TileCliff(File_TileCliff), "Tile cliffs file: ")

        Dim File_Roads As New clsWriteFile
        ReturnResult.Append(Data_FMap_Roads(File_Roads), "Roads file: ")

        Dim INI_Objects As clsINIWrite = CreateINIWriteFile()
        ReturnResult.Append(Data_FMap_Objects(INI_Objects), "Objects file: ")

        Dim INI_Gateways As clsINIWrite = CreateINIWriteFile()
        Data_FMap_Gateways(INI_Gateways)

        Dim File_TileTypes As New clsWriteFile
        Data_FMap_TileTypes(File_TileTypes)

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        Dim FileStream As IO.FileStream

        Try
            FileStream = IO.File.Create(Path)
        Catch ex As Exception
            ReturnResult.Problem_Add("Unable to create file at " & Path)
            Return ReturnResult
        End Try

        Dim WZStream As Zip.ZipOutputStream = New Zip.ZipOutputStream(FileStream)
        Dim ZipPath As String

        If Compress Then
            WZStream.SetLevel(9)
        Else
            WZStream.SetLevel(0)
        End If

        ZipPath = "Info.ini"
        ReturnResult.Append(INI_Info.File.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "VertexHeight.dat"
        ReturnResult.Append(File_VertexHeight.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "VertexTerrain.dat"
        ReturnResult.Append(File_VertexTerrain.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "TileTexture.dat"
        ReturnResult.Append(File_TileTexture.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "TileOrientation.dat"
        ReturnResult.Append(File_TileOrientation.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "TileCliff.dat"
        ReturnResult.Append(File_TileCliff.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "Roads.dat"
        ReturnResult.Append(File_Roads.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "Objects.ini"
        ReturnResult.Append(INI_Objects.File.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "Gateways.ini"
        ReturnResult.Append(INI_Gateways.File.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        ZipPath = "TileTypes.dat"
        ReturnResult.Append(File_TileTypes.WriteToZip(WZStream, ZipPath), "Zipping file " & ZipPath)

        If ReturnResult.HasProblems Then
            WZStream.Close()
            Return ReturnResult
        End If

        WZStream.Finish()
        WZStream.Close()

        Return ReturnResult
    End Function

    Public Sub Data_FMap_Info(ByVal File As clsINIWrite)

        If Tileset Is Nothing Then

        ElseIf Tileset Is Tileset_Arizona Then
            File.Property_Append("Tileset", "Arizona")
        ElseIf Tileset Is Tileset_Urban Then
            File.Property_Append("Tileset", "Urban")
        ElseIf Tileset Is Tileset_Rockies Then
            File.Property_Append("Tileset", "Rockies")
        End If

        File.Property_Append("Size", Terrain.TileSize.X & ", " & Terrain.TileSize.Y)

        File.Property_Append("AutoScrollLimits", CStr(frmCompileInstance.cbxAutoScrollLimits.Checked))
        File.Property_Append("ScrollMinX", frmCompileInstance.txtScrollMinX.Text)
        File.Property_Append("ScrollMinY", frmCompileInstance.txtScrollMinY.Text)
        File.Property_Append("ScrollMaxX", frmCompileInstance.txtScrollMaxX.Text)
        File.Property_Append("ScrollMaxY", frmCompileInstance.txtScrollMaxY.Text)

        File.Property_Append("Name", frmCompileInstance.txtName.Text)
        File.Property_Append("Players", frmCompileInstance.txtMultiPlayers.Text)
        File.Property_Append("XPlayerLev", CStr(frmCompileInstance.cbxNewPlayerFormat.Checked))
        File.Property_Append("Author", frmCompileInstance.txtAuthor.Text)
        File.Property_Append("License", frmCompileInstance.cboLicense.Text)
        File.Property_Append("CampTime", frmCompileInstance.txtCampTime.Text)
        If frmCompileInstance.cboCampType.SelectedIndex >= 0 Then
            File.Property_Append("CampType", CStr(frmCompileInstance.cboCampType.SelectedIndex))
        End If
    End Sub

    Public Sub Data_FMap_VertexHeight(ByVal File As clsWriteFile)
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                File.U8_Append(Terrain.Vertices(X, Y).Height)
            Next
        Next
    End Sub

    Public Function Data_FMap_VertexTerrain(ByVal File As clsWriteFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                If Terrain.Vertices(X, Y).Terrain Is Nothing Then
                    Value = 0
                ElseIf Terrain.Vertices(X, Y).Terrain.Num < 0 Then
                    ErrorCount += 1
                    Value = 0
                Else
                    Value = Terrain.Vertices(X, Y).Terrain.Num + 1
                    If Value > 255 Then
                        ErrorCount += 1
                        Value = 0
                    End If
                End If
                File.U8_Append(CByte(Value))
            Next
        Next

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " vertices had an invalid painted terrain number.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_TileTexture(ByVal File As clsWriteFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Value = Terrain.Tiles(X, Y).Texture.TextureNum + 1
                If Value < 0 Or Value > 255 Then
                    ErrorCount += 1
                    Value = 0
                End If
                File.U8_Append(CByte(Value))
            Next
        Next

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " tiles had an invalid texture number.")
        End If

        Return ReturnResult
    End Function

    Public Sub Data_FMap_TileOrientation(ByVal File As clsWriteFile)
        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Value = 0
                If Terrain.Tiles(X, Y).Texture.Orientation.SwitchedAxes Then
                    Value += 8
                End If
                If Terrain.Tiles(X, Y).Texture.Orientation.ResultXFlip Then
                    Value += 4
                End If
                If Terrain.Tiles(X, Y).Texture.Orientation.ResultYFlip Then
                    Value += 2
                End If
                If Terrain.Tiles(X, Y).Tri Then
                    Value += 1
                End If
                File.U8_Append(CByte(Value))
            Next
        Next
    End Sub

    Public Function Data_FMap_TileCliff(ByVal File As clsWriteFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim DownSideValue As Integer
        Dim ErrorCount As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Value = 0
                If Terrain.Tiles(X, Y).Tri Then
                    If Terrain.Tiles(X, Y).TriTopLeftIsCliff Then
                        Value += 2
                    End If
                    If Terrain.Tiles(X, Y).TriBottomRightIsCliff Then
                        Value += 1
                    End If
                Else
                    If Terrain.Tiles(X, Y).TriBottomLeftIsCliff Then
                        Value += 2
                    End If
                    If Terrain.Tiles(X, Y).TriTopRightIsCliff Then
                        Value += 1
                    End If
                End If
                If Terrain.Tiles(X, Y).Terrain_IsCliff Then
                    Value += 4
                End If
                If IdenticalTileOrientations(Terrain.Tiles(X, Y).DownSide, TileDirection_None) Then
                    DownSideValue = 0
                ElseIf IdenticalTileOrientations(Terrain.Tiles(X, Y).DownSide, TileDirection_Top) Then
                    DownSideValue = 1
                ElseIf IdenticalTileOrientations(Terrain.Tiles(X, Y).DownSide, TileDirection_Left) Then
                    DownSideValue = 2
                ElseIf IdenticalTileOrientations(Terrain.Tiles(X, Y).DownSide, TileDirection_Right) Then
                    DownSideValue = 3
                ElseIf IdenticalTileOrientations(Terrain.Tiles(X, Y).DownSide, TileDirection_Bottom) Then
                    DownSideValue = 4
                Else
                    ErrorCount += 1
                    DownSideValue = 0
                End If
                Value += DownSideValue * 8
                File.U8_Append(CByte(Value))
            Next
        Next

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " tiles had an invalid cliff down side.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_Roads(ByVal File As clsWriteFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim ErrorCount As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X - 1
                If Terrain.SideH(X, Y).Road Is Nothing Then
                    Value = 0
                ElseIf Terrain.SideH(X, Y).Road.Num < 0 Then
                    ErrorCount += 1
                    Value = 0
                Else
                    Value = Terrain.SideH(X, Y).Road.Num + 1
                    If Value > 255 Then
                        ErrorCount += 1
                        Value = 0
                    End If
                End If
                File.U8_Append(CByte(Value))
            Next
        Next
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X
                If Terrain.SideV(X, Y).Road Is Nothing Then
                    Value = 0
                ElseIf Terrain.SideV(X, Y).Road.Num < 0 Then
                    ErrorCount += 1
                    Value = 0
                Else
                    Value = Terrain.SideV(X, Y).Road.Num + 1
                    If Value > 255 Then
                        ErrorCount += 1
                        Value = 0
                    End If
                End If
                File.U8_Append(CByte(Value))
            Next
        Next

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " sides had an invalid road number.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_Objects(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult

        Dim A As Integer
        Dim tmpUnit As clsUnit
        Dim tmpDroid As clsDroidDesign
        Dim WarningCount As Integer
        Dim strTemp As String = Nothing

        For A = 0 To UnitCount - 1
            tmpUnit = Units(A)
            File.SectionName_Append(CStr(A))
            Select Case tmpUnit.Type.Type
                Case clsUnitType.enumType.Feature
                    File.Property_Append("Type", "Feature, " & CType(tmpUnit.Type, clsFeatureType).Code)
                Case clsUnitType.enumType.PlayerStructure
                    File.Property_Append("Type", "Structure, " & CType(tmpUnit.Type, clsStructureType).Code)
                Case clsUnitType.enumType.PlayerDroid
                    tmpDroid = CType(tmpUnit.Type, clsDroidDesign)
                    If tmpDroid.IsTemplate Then
                        File.Property_Append("Type", "DroidTemplate, " & CType(tmpUnit.Type, clsDroidTemplate).Code)
                    Else
                        File.Property_Append("Type", "DroidDesign")
                        If tmpDroid.TemplateDroidType IsNot Nothing Then
                            File.Property_Append("DroidType", tmpDroid.TemplateDroidType.TemplateCode)
                        End If
                        If tmpDroid.Body IsNot Nothing Then
                            File.Property_Append("Body", tmpDroid.Body.Code)
                        End If
                        If tmpDroid.Propulsion IsNot Nothing Then
                            File.Property_Append("Propulsion", tmpDroid.Propulsion.Code)
                        End If
                        File.Property_Append("TurretCount", CStr(tmpDroid.TurretCount))
                        If tmpDroid.Turret1 IsNot Nothing Then
                            If tmpDroid.Turret1.GetTurretTypeName(strTemp) Then
                                File.Property_Append("Turret1", strTemp & ", " & tmpDroid.Turret1.Code)
                            End If
                        End If
                        If tmpDroid.Turret2 IsNot Nothing Then
                            If tmpDroid.Turret2.GetTurretTypeName(strTemp) Then
                                File.Property_Append("Turret2", strTemp & ", " & tmpDroid.Turret2.Code)
                            End If
                        End If
                        If tmpDroid.Turret3 IsNot Nothing Then
                            If tmpDroid.Turret3.GetTurretTypeName(strTemp) Then
                                File.Property_Append("Turret3", strTemp & ", " & tmpDroid.Turret3.Code)
                            End If
                        End If
                    End If
                Case Else
                    WarningCount += 1
            End Select
            File.Property_Append("ID", CStr(tmpUnit.ID))
            File.Property_Append("Priority", CStr(tmpUnit.SavePriority))
            File.Property_Append("Pos", tmpUnit.Pos.Horizontal.X & ", " & tmpUnit.Pos.Horizontal.Y)
            File.Property_Append("Heading", CStr(tmpUnit.Rotation))
            If tmpUnit.UnitGroup.Map_UnitGroupNum < 0 Then
                strTemp = "scavenger"
            Else
                strTemp = CStr(tmpUnit.UnitGroup.Map_UnitGroupNum)
            End If
            File.Property_Append("UnitGroup", strTemp)
            If tmpUnit.Health < 1.0# Then
                File.Property_Append("Health", CStr(tmpUnit.Health))
            End If
            File.Gap_Append()
        Next

        If WarningCount > 0 Then
            ReturnResult.Warning_Add("Error: " & WarningCount & " units were of an unhandled type.")
        End If

        Return ReturnResult
    End Function

    Public Sub Data_FMap_Gateways(ByVal File As clsINIWrite)
        Dim A As Integer

        For A = 0 To GatewayCount - 1
            File.SectionName_Append(CStr(A))
            File.Property_Append("AX", CStr(Gateways(A).PosA.X))
            File.Property_Append("AY", CStr(Gateways(A).PosA.Y))
            File.Property_Append("BX", CStr(Gateways(A).PosB.X))
            File.Property_Append("BY", CStr(Gateways(A).PosB.Y))
            File.Gap_Append()
        Next
    End Sub

    Public Sub Data_FMap_TileTypes(ByVal File As clsWriteFile)
        Dim A As Integer

        If Tileset IsNot Nothing Then
            For A = 0 To Tileset.TileCount - 1
                File.U8_Append(Tile_TypeNum(A))
            Next
        End If
    End Sub

    Public Function Load_FMap(ByVal Path As String, ByRef InterfaceOptions As clsInterfaceOptions) As clsResult
        Dim ReturnResult As New clsResult

        InterfaceOptions = Nothing

        Dim File As New clsReadFile
        Dim ZipSearchResult As clsZipStreamEntry
        Dim FindPath As String

        Dim ResultInfo As clsFMapInfo = Nothing

        FindPath = "info.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Problem_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
            Return ReturnResult
        Else
            ReturnResult.Append(Read_FMap_Info(ZipSearchResult.BeginNewReadFile, ResultInfo), "Read info: ")
            ZipSearchResult.Stream.Close()
            If ReturnResult.HasProblems Then
                Return ReturnResult
            End If
        End If

        Dim NewTerrainSize As sXY_int = ResultInfo.TerrainSize
        Tileset = ResultInfo.Tileset

        If NewTerrainSize.X <= 0 Or NewTerrainSize.X > MapMaxSize Then
            ReturnResult.Problem_Add("Map width of " & NewTerrainSize.X & " is not valid.")
        End If
        If NewTerrainSize.Y <= 0 Or NewTerrainSize.Y > MapMaxSize Then
            ReturnResult.Problem_Add("Map height of " & NewTerrainSize.Y & " is not valid.")
        End If
        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        SetPainterToDefaults() 'depends on tileset. must be called before loading the terrains.
        Terrain_Blank(NewTerrainSize)
        TileType_Reset()

        FindPath = "vertexheight.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_VertexHeight(ZipSearchResult.BeginNewReadFile), "Read vertex height: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "vertexterrain.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_VertexTerrain(ZipSearchResult.BeginNewReadFile), "Read vertex terrain: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "tiletexture.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_TileTexture(ZipSearchResult.BeginNewReadFile), "Read tile texture: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "tileorientation.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_TileOrientation(ZipSearchResult.BeginNewReadFile), "Read tile orientation: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "tilecliff.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_TileCliff(ZipSearchResult.BeginNewReadFile), "Read tile cliff: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "roads.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_Roads(ZipSearchResult.BeginNewReadFile), "Read roads: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "objects.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_Objects(ZipSearchResult.BeginNewReadFile), "Read objects: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "gateways.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_Gateways(ZipSearchResult.BeginNewReadFile), "Read gateways: ")
            ZipSearchResult.Stream.Close()
        End If

        FindPath = "tiletypes.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            ReturnResult.AppendAsWarning(Read_FMap_TileTypes(ZipSearchResult.BeginNewReadFile), "Read tile types: ")
            ZipSearchResult.Stream.Close()
        End If

        InterfaceOptions = ResultInfo.InterfaceOptions

        AfterInitialized()

        Return ReturnResult
    End Function

    Public Class clsFMapInfo
        Inherits clsINIRead.clsTranslator

        Public TerrainSize As sXY_int = New sXY_int(-1, -1)
        Public InterfaceOptions As New clsInterfaceOptions
        Public Tileset As clsTileset

        Public Overrides Function Translate(ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "tileset"
                    Select Case INIProperty.Value.ToLower
                        Case "arizona"
                            Tileset = Tileset_Arizona
                        Case "urban"
                            Tileset = Tileset_Urban
                        Case "rockies"
                            Tileset = Tileset_Rockies
                        Case Else
                            Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Select
                Case "size"
                    Dim CommaText() As String = INIProperty.Value.Split(","c)
                    If CommaText.GetUpperBound(0) + 1 < 2 Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Dim A As Integer
                    For A = 0 To CommaText.GetUpperBound(0)
                        CommaText(A) = CommaText(A).Trim
                    Next
                    Dim NewSize As sXY_int
                    Try
                        NewSize.X = CInt(CommaText(0))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    Try
                        NewSize.Y = CInt(CommaText(1))
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If NewSize.X < 1 Or NewSize.Y < 1 Or NewSize.X > MapMaxSize Or NewSize.Y > MapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    TerrainSize = NewSize
                Case "autoscrolllimits"
                    Try
                        InterfaceOptions.AutoScrollLimits = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "scrollminx"
                    Try
                        InterfaceOptions.ScrollMin.X = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "scrollminy"
                    Try
                        InterfaceOptions.ScrollMin.Y = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "scrollmaxx"
                    Try
                        InterfaceOptions.ScrollMax.X = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "scrollmaxy"
                    Try
                        InterfaceOptions.ScrollMax.Y = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "name"
                    InterfaceOptions.CompileName = INIProperty.Value
                Case "players"
                    InterfaceOptions.CompileMultiPlayers = INIProperty.Value
                Case "xplayerlev"
                    Try
                        InterfaceOptions.CompileMultiXPlayers = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "author"
                    InterfaceOptions.CompileMultiAuthor = INIProperty.Value
                Case "license"
                    InterfaceOptions.CompileMultiLicense = INIProperty.Value
                Case "camptime"
                    InterfaceOptions.CampaignGameTime = INIProperty.Value
                Case "camptype"
                    InterfaceOptions.CampaignGameType = CInt(INIProperty.Value)
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_FMap_Info(ByVal File As clsReadFile, ByRef ResultInfo As clsFMapInfo) As clsResult
        Dim ReturnResult As New clsResult

        Dim InfoINI As New clsINIRead.clsSection
        ReturnResult.Append(InfoINI.ReadFile(File), "")

        ResultInfo = New clsFMapInfo
        ReturnResult.Append(InfoINI.Translate(ResultInfo), "")

        If ResultInfo.TerrainSize.X < 0 Or ResultInfo.TerrainSize.Y < 0 Then
            ReturnResult.Problem_Add("Map size was not specified or was invalid.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_VertexHeight(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                If Not File.Get_U8(Terrain.Vertices(X, Y).Height) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
            Next
        Next

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_VertexTerrain(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim byteTemp As Byte
        Dim WarningCount As Integer

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
                Value = CInt(byteTemp) - 1
                If Value < 0 Then
                    Terrain.Vertices(X, Y).Terrain = Nothing
                ElseIf Value >= Painter.TerrainCount Then
                    If WarningCount < 16 Then
                        ReturnResult.Warning_Add("Painted terrain at vertex " & X & ", " & Y & " was invalid.")
                    End If
                    WarningCount += 1
                    Terrain.Vertices(X, Y).Terrain = Nothing
                Else
                    Terrain.Vertices(X, Y).Terrain = Painter.Terrains(Value)
                End If
            Next
        Next

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " painted terrain vertices were invalid.")
        End If

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTexture(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim byteTemp As Byte

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
                Terrain.Tiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1
            Next
        Next

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileOrientation(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim byteTemp As Byte
        Dim Value As Integer
        Dim PartValue As Integer
        Dim WarningCount As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If

                Value = byteTemp

                PartValue = CInt(Math.Floor(Value / 16))
                If PartValue > 0 Then
                    If WarningCount < 16 Then
                        ReturnResult.Warning_Add("Unknown bits used for tile " & X & ", " & Y & ".")
                    End If
                    WarningCount += 1
                End If
                Value -= PartValue * 16

                PartValue = CInt(Int(Value / 8.0#))
                Terrain.Tiles(X, Y).Texture.Orientation.SwitchedAxes = (PartValue > 0)
                Value -= PartValue * 8

                PartValue = CInt(Int(Value / 4.0#))
                Terrain.Tiles(X, Y).Texture.Orientation.ResultXFlip = (PartValue > 0)
                Value -= PartValue * 4

                PartValue = CInt(Int(Value / 2.0#))
                Terrain.Tiles(X, Y).Texture.Orientation.ResultYFlip = (PartValue > 0)
                Value -= PartValue * 2

                PartValue = Value
                Terrain.Tiles(X, Y).Tri = (PartValue > 0)
            Next
        Next

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " tiles had unknown bits used.")
        End If

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileCliff(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim byteTemp As Byte
        Dim PartValue As Integer
        Dim DownSideWarningCount As Integer
        Dim WarningCount As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If

                Value = byteTemp

                PartValue = CInt(Int(Value / 64.0#))
                If PartValue > 0 Then
                    If WarningCount < 16 Then
                        ReturnResult.Warning_Add("Unknown bits used for tile " & X & ", " & Y & ".")
                    End If
                    WarningCount += 1
                End If
                Value -= PartValue * 64

                PartValue = CInt(Int(Value / 8.0#))
                Select Case PartValue
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
                        Terrain.Tiles(X, Y).DownSide = TileDirection_None
                        If DownSideWarningCount < 16 Then
                            ReturnResult.Warning_Add("Down side value for tile " & X & ", " & Y & " was invalid.")
                        End If
                        DownSideWarningCount += 1
                End Select
                Value -= PartValue * 8

                PartValue = CInt(Int(Value / 4.0#))
                Terrain.Tiles(X, Y).Terrain_IsCliff = (PartValue > 0)
                Value -= PartValue * 4

                PartValue = CInt(Int(Value / 2.0#))
                If Terrain.Tiles(X, Y).Tri Then
                    Terrain.Tiles(X, Y).TriTopLeftIsCliff = (PartValue > 0)
                Else
                    Terrain.Tiles(X, Y).TriBottomLeftIsCliff = (PartValue > 0)
                End If
                Value -= PartValue * 2

                PartValue = Value
                If Terrain.Tiles(X, Y).Tri Then
                    Terrain.Tiles(X, Y).TriBottomRightIsCliff = (PartValue > 0)
                Else
                    Terrain.Tiles(X, Y).TriTopRightIsCliff = (PartValue > 0)
                End If
            Next
        Next

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " tiles had unknown bits used.")
        End If
        If DownSideWarningCount > 0 Then
            ReturnResult.Warning_Add(DownSideWarningCount & " tiles had invalid down side values.")
        End If

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_Roads(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim WarningCount As Integer
        Dim byteTemp As Byte

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
                Value = CInt(byteTemp) - 1
                If Value < 0 Then
                    Terrain.SideH(X, Y).Road = Nothing
                ElseIf Value >= Painter.RoadCount Then
                    If WarningCount < 16 Then
                        ReturnResult.Warning_Add("Invalid road value for horizontal side " & X & ", " & Y & ".")
                    End If
                    WarningCount += 1
                    Terrain.SideH(X, Y).Road = Nothing
                Else
                    Terrain.SideH(X, Y).Road = Painter.Roads(Value)
                End If
            Next
        Next
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
                Value = CInt(byteTemp) - 1
                If Value < 0 Then
                    Terrain.SideV(X, Y).Road = Nothing
                ElseIf Value >= Painter.RoadCount Then
                    If WarningCount < 16 Then
                        ReturnResult.Warning_Add("Invalid road value for vertical side " & X & ", " & Y & ".")
                    End If
                    WarningCount += 1
                    Terrain.SideV(X, Y).Road = Nothing
                Else
                    Terrain.SideV(X, Y).Road = Painter.Roads(Value)
                End If
            Next
        Next

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " sides had an invalid road value.")
        End If

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_Objects(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim A As Integer

        Dim ObjectsINI As New clsINIRead
        ReturnResult.Append(ObjectsINI.ReadFile(File), "")

        Dim INIObjects As New clsFMap_INIObjects(ObjectsINI.SectionCount)
        ReturnResult.Append(ObjectsINI.Translate(INIObjects), "")

        Dim DroidComponentUnknownCount As Integer
        Dim ObjectTypeMissingCount As Integer
        Dim ObjectPlayerNumInvalidCount As Integer
        Dim ObjectPosInvalidCount As Integer
        Dim DesignTypeUnspecifiedCount As Integer
        Dim UnknownUnitTypeCount As Integer
        Dim MaxUnknownUnitTypeWarningCount As Integer = 16

        Dim tmpDroidDesign As clsDroidDesign
        Dim NewObject As clsUnit
        Dim tmpUnitType As clsUnitType
        Dim IsDesign As Boolean
        Dim tmpUnitGroup As clsUnitGroup
        Dim ZeroPos As New sXY_int(0, 0)
        Dim AvailableID As UInteger

        AvailableID = 1UI
        For A = 0 To INIObjects.ObjectCount - 1
            If INIObjects.Objects(A).ID >= AvailableID Then
                AvailableID = INIObjects.Objects(A).ID + 1UI
            End If
        Next
        For A = 0 To INIObjects.ObjectCount - 1
            If INIObjects.Objects(A).Pos Is Nothing Then
                ObjectPosInvalidCount += 1
            ElseIf Not PosIsWithinTileArea(INIObjects.Objects(A).Pos.XY, ZeroPos, Terrain.TileSize) Then
                ObjectPosInvalidCount += 1
            Else
                tmpUnitType = Nothing
                If INIObjects.Objects(A).Type <> clsUnitType.enumType.Unspecified Then
                    IsDesign = False
                    If INIObjects.Objects(A).Type = clsUnitType.enumType.PlayerDroid Then
                        If Not INIObjects.Objects(A).IsTemplate Then
                            IsDesign = True
                        End If
                    End If
                    If IsDesign Then
                        tmpDroidDesign = New clsDroidDesign
                        tmpDroidDesign.TemplateDroidType = INIObjects.Objects(A).TemplateDroidType
                        If tmpDroidDesign.TemplateDroidType Is Nothing Then
                            tmpDroidDesign.TemplateDroidType = TemplateDroidType_Droid
                            DesignTypeUnspecifiedCount += 1
                        End If
                        If INIObjects.Objects(A).BodyCode <> "" Then
                            tmpDroidDesign.Body = FindOrCreateBody(INIObjects.Objects(A).BodyCode)
                            If tmpDroidDesign.Body.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).PropulsionCode <> "" Then
                            tmpDroidDesign.Propulsion = FindOrCreatePropulsion(INIObjects.Objects(A).PropulsionCode)
                            If tmpDroidDesign.Propulsion.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        tmpDroidDesign.TurretCount = CByte(INIObjects.Objects(A).TurretCount)
                        If INIObjects.Objects(A).TurretCodes(0) <> "" Then
                            tmpDroidDesign.Turret1 = FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(0), INIObjects.Objects(A).TurretCodes(0))
                            If tmpDroidDesign.Turret1.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).TurretCodes(1) <> "" Then
                            tmpDroidDesign.Turret2 = FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(1), INIObjects.Objects(A).TurretCodes(1))
                            If tmpDroidDesign.Turret2.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        If INIObjects.Objects(A).TurretCodes(2) <> "" Then
                            tmpDroidDesign.Turret3 = FindOrCreateTurret(INIObjects.Objects(A).TurretTypes(2), INIObjects.Objects(A).TurretCodes(2))
                            If tmpDroidDesign.Turret3.IsUnknown Then
                                DroidComponentUnknownCount += 1
                            End If
                        End If
                        tmpDroidDesign.UpdateAttachments()
                        tmpUnitType = tmpDroidDesign
                    Else
                        tmpUnitType = FindOrCreateUnitType(INIObjects.Objects(A).Code, INIObjects.Objects(A).Type)
                        If tmpUnitType.IsUnknown Then
                            If UnknownUnitTypeCount < MaxUnknownUnitTypeWarningCount Then
                                ReturnResult.Warning_Add(ControlChars.Quote & INIObjects.Objects(A).Code & ControlChars.Quote & " is not a loaded object.")
                            End If
                            UnknownUnitTypeCount += 1
                        End If
                    End If
                End If
                If tmpUnitType Is Nothing Then
                    ObjectTypeMissingCount += 1
                Else
                    NewObject = New clsUnit
                    NewObject.Type = tmpUnitType
                    NewObject.Pos.Horizontal.X = INIObjects.Objects(A).Pos.X
                    NewObject.Pos.Horizontal.Y = INIObjects.Objects(A).Pos.Y
                    NewObject.Health = INIObjects.Objects(A).Health
                    NewObject.SavePriority = INIObjects.Objects(A).Priority
                    NewObject.Rotation = CInt(INIObjects.Objects(A).Heading)
                    If NewObject.Rotation >= 360 Then
                        NewObject.Rotation -= 360
                    End If
                    If INIObjects.Objects(A).UnitGroup = Nothing Or INIObjects.Objects(A).UnitGroup = "" Then
                        If INIObjects.Objects(A).Type <> clsUnitType.enumType.Feature Then
                            ObjectPlayerNumInvalidCount += 1
                        End If
                        NewObject.UnitGroup = UnitGroups(0)
                    Else
                        If INIObjects.Objects(A).UnitGroup.ToLower = "scavenger" Then
                            NewObject.UnitGroup = ScavengerUnitGroup
                        Else
                            Dim PlayerNum As UInteger
                            Try
                                PlayerNum = CUInt(INIObjects.Objects(A).UnitGroup)
                                If PlayerNum < PlayerCountMax Then
                                    tmpUnitGroup = UnitGroups(CInt(PlayerNum))
                                Else
                                    tmpUnitGroup = ScavengerUnitGroup
                                    ObjectPlayerNumInvalidCount += 1
                                End If
                            Catch ex As Exception
                                ObjectPlayerNumInvalidCount += 1
                                tmpUnitGroup = ScavengerUnitGroup
                            End Try
                            NewObject.UnitGroup = tmpUnitGroup
                        End If
                    End If
                    If INIObjects.Objects(A).ID = 0UI Then
                        INIObjects.Objects(A).ID = AvailableID
                        ZeroIDWarning(NewObject, INIObjects.Objects(A).ID)
                    End If
                    UnitID_Add(NewObject, INIObjects.Objects(A).ID)
                    ErrorIDChange(INIObjects.Objects(A).ID, NewObject, "Read_FMap_Objects")
                    If AvailableID = INIObjects.Objects(A).ID Then
                        AvailableID = NewObject.ID + 1UI
                    End If
                End If
            End If
        Next

        If UnknownUnitTypeCount > MaxUnknownUnitTypeWarningCount Then
            ReturnResult.Warning_Add(UnknownUnitTypeCount & " objects were not in the loaded object data.")
        End If
        If ObjectTypeMissingCount > 0 Then
            ReturnResult.Warning_Add(ObjectTypeMissingCount & " objects did not specify a type and were ignored.")
        End If
        If DroidComponentUnknownCount > 0 Then
            ReturnResult.Warning_Add(DroidComponentUnknownCount & " components used by droids were loaded as unknowns.")
        End If
        If ObjectPlayerNumInvalidCount > 0 Then
            ReturnResult.Warning_Add(ObjectPlayerNumInvalidCount & " objects had an invalid player number and were set to player 0.")
        End If
        If ObjectPosInvalidCount > 0 Then
            ReturnResult.Warning_Add(ObjectPosInvalidCount & " objects had a position that was off-map and were ignored.")
        End If
        If DesignTypeUnspecifiedCount > 0 Then
            ReturnResult.Warning_Add(DesignTypeUnspecifiedCount & " designed droids did not specify a template droid type and were set to droid.")
        End If

        Return ReturnResult
    End Function

    Public Class clsFMap_INIGateways
        Inherits clsINIRead.clsSectionTranslator

        Public Structure sGateway
            Public PosA As sXY_int
            Public PosB As sXY_int
        End Structure
        Public Gateways() As sGateway
        Public GatewayCount As Integer

        Public Sub New(ByVal NewGatewayCount As Integer)
            Dim A As Integer

            GatewayCount = NewGatewayCount
            ReDim Gateways(GatewayCount - 1)
            For A = 0 To GatewayCount - 1
                Gateways(A).PosA.X = -1
                Gateways(A).PosA.Y = -1
                Gateways(A).PosB.X = -1
                Gateways(A).PosB.Y = -1
            Next
        End Sub

        Public Overrides Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "ax"
                    Try
                        Gateways(INISectionNum).PosA.X = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "ay"
                    Try
                        Gateways(INISectionNum).PosA.Y = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "bx"
                    Try
                        Gateways(INISectionNum).PosB.X = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "by"
                    Try
                        Gateways(INISectionNum).PosB.Y = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Read_FMap_Gateways(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim GatewaysINI As New clsINIRead
        ReturnResult.Append(GatewaysINI.ReadFile(File), "")

        Dim INIGateways As New clsFMap_INIGateways(GatewaysINI.SectionCount)
        ReturnResult.Append(GatewaysINI.Translate(INIGateways), "")

        Dim A As Integer
        Dim InvalidGatewayCount As Integer = 0

        For A = 0 To INIGateways.GatewayCount - 1
            If Not Gateway_Add(INIGateways.Gateways(A).PosA, INIGateways.Gateways(A).PosB) Then
                InvalidGatewayCount += 1
            End If
        Next

        If InvalidGatewayCount > 0 Then
            ReturnResult.Warning_Add(InvalidGatewayCount & " gateways were invalid.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTypes(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim A As Integer
        Dim byteTemp As Byte
        Dim InvalidTypeCount As Integer

        If Tileset IsNot Nothing Then
            For A = 0 To Tileset.TileCount - 1
                If Not File.Get_U8(byteTemp) Then
                    ReturnResult.Problem_Add("Read error.")
                    Return ReturnResult
                End If
                If byteTemp >= TileTypeCount Then
                    InvalidTypeCount += 1
                Else
                    Tile_TypeNum(A) = byteTemp
                End If
            Next
        End If

        If InvalidTypeCount > 0 Then
            ReturnResult.Warning_Add(InvalidTypeCount & " tile types were invalid.")
        End If

        If Not File.IsEOF Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function
End Class