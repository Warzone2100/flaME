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
                    If Not InvariantParse_int(INIProperty.Value, NewTurretCount) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
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
                    If Not InvariantParse_uint(INIProperty.Value, Objects(INISectionNum).ID) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "priority"
                    If Not InvariantParse_int(INIProperty.Value, Objects(INISectionNum).Priority) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
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
                    Dim tmpXY As sXY_int
                    If Not InvariantParse_int(CommaText(0), tmpXY.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If Not InvariantParse_int(CommaText(1), tmpXY.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Try
                        Objects(INISectionNum).Pos = New clsXY_int(tmpXY)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "heading"
                    Dim dblTemp As Double
                    If Not InvariantParse_dbl(INIProperty.Value, dblTemp) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If dblTemp < 0.0# Or dblTemp >= 360.0# Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    Objects(INISectionNum).Heading = dblTemp
                Case "unitgroup"
                    Objects(INISectionNum).UnitGroup = INIProperty.Value
                Case "health"
                    Dim NewHealth As Double
                    If Not InvariantParse_dbl(INIProperty.Value, NewHealth) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
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

        Dim FileStream As IO.FileStream
        Try
            FileStream = IO.File.Create(Path)
        Catch ex As Exception
            ReturnResult.Problem_Add("Unable to create file at " & Path)
            Return ReturnResult
        End Try

        Dim WZStream As New Zip.ZipOutputStream(FileStream)
        WZStream.UseZip64 = Zip.UseZip64.Off
        If Compress Then
            WZStream.SetLevel(9)
        Else
            WZStream.SetLevel(0)
        End If

        Dim Encoding As New System.Text.UTF8Encoding(False, False)
        Dim BinaryWriter As New IO.BinaryWriter(WZStream, Encoding)
        Dim StreamWriter As New IO.StreamWriter(WZStream, Encoding)
        Dim ZipEntry As Zip.ZipEntry
        Dim ZipPath As String

        ZipPath = "Info.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Info As New clsINIWrite
            INI_Info.File = StreamWriter
            ReturnResult.Append(Data_FMap_Info(INI_Info), "Serialising: " & ZipPath)

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "VertexHeight.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_VertexHeight(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "VertexTerrain.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_VertexTerrain(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileTexture.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_TileTexture(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileOrientation.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_TileOrientation(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileCliff.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_TileCliff(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Roads.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_Roads(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Objects.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Objects As New clsINIWrite
            INI_Objects.File = StreamWriter
            ReturnResult.Append(Data_FMap_Objects(INI_Objects), "Serialising: " & ZipPath)

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "Gateways.ini"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            Dim INI_Gateways As New clsINIWrite
            INI_Gateways.File = StreamWriter
            ReturnResult.Append(Data_FMap_Gateways(INI_Gateways), "Serialising: " & ZipPath)

            StreamWriter.Flush()
            WZStream.CloseEntry()
        End If

        ZipPath = "TileTypes.dat"
        ZipEntry = ZipMakeEntry(WZStream, ZipPath, ReturnResult)
        If ZipEntry IsNot Nothing Then
            ReturnResult.Append(Data_FMap_TileTypes(BinaryWriter), "Serialising: " & ZipPath)

            BinaryWriter.Flush()
            WZStream.CloseEntry()
        End If

        WZStream.Finish()
        WZStream.Close()
        Return ReturnResult
    End Function

    Public Function Data_FMap_Info(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult

        Try
            If Tileset Is Nothing Then

            ElseIf Tileset Is Tileset_Arizona Then
                File.Property_Append("Tileset", "Arizona")
            ElseIf Tileset Is Tileset_Urban Then
                File.Property_Append("Tileset", "Urban")
            ElseIf Tileset Is Tileset_Rockies Then
                File.Property_Append("Tileset", "Rockies")
            End If

            File.Property_Append("Size", Terrain.TileSize.X & ", " & Terrain.TileSize.Y)

            File.Property_Append("AutoScrollLimits", InvariantToString_bool(InterfaceOptions.AutoScrollLimits))
            File.Property_Append("ScrollMinX", InvariantToString_int(InterfaceOptions.ScrollMin.X))
            File.Property_Append("ScrollMinY", InvariantToString_int(InterfaceOptions.ScrollMin.Y))
            File.Property_Append("ScrollMaxX", InvariantToString_sng(InterfaceOptions.ScrollMax.X))
            File.Property_Append("ScrollMaxY", InvariantToString_sng(InterfaceOptions.ScrollMax.Y))

            File.Property_Append("Name", InterfaceOptions.CompileName)
            File.Property_Append("Players", InterfaceOptions.CompileMultiPlayers)
            File.Property_Append("XPlayerLev", InvariantToString_bool(InterfaceOptions.CompileMultiXPlayers))
            File.Property_Append("Author", InterfaceOptions.CompileMultiAuthor)
            File.Property_Append("License", InterfaceOptions.CompileMultiLicense)
            File.Property_Append("CampTime", InvariantToString_int(InterfaceOptions.CampaignGameTime))
            If InterfaceOptions.CampaignGameType >= 0 Then
                File.Property_Append("CampType", InvariantToString_int(InterfaceOptions.CampaignGameType))
            End If
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Data_FMap_VertexHeight(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult
        Dim X As Integer
        Dim Y As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    File.Write(CByte(Terrain.Vertices(X, Y).Height))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Data_FMap_VertexTerrain(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        Try
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
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " vertices had an invalid painted terrain number.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_TileTexture(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim ErrorCount As Integer
        Dim Value As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = Terrain.Tiles(X, Y).Texture.TextureNum + 1
                    If Value < 0 Or Value > 255 Then
                        ErrorCount += 1
                        Value = 0
                    End If
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " tiles had an invalid texture number.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_TileOrientation(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult
        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer

        Try
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
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Data_FMap_TileCliff(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim DownSideValue As Integer
        Dim ErrorCount As Integer

        Try
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
                    If IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_None) Then
                        DownSideValue = 0
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Top) Then
                        DownSideValue = 1
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Left) Then
                        DownSideValue = 2
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Right) Then
                        DownSideValue = 3
                    ElseIf IdenticalTileDirections(Terrain.Tiles(X, Y).DownSide, TileDirection_Bottom) Then
                        DownSideValue = 4
                    Else
                        ErrorCount += 1
                        DownSideValue = 0
                    End If
                    Value += DownSideValue * 8
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        If ErrorCount > 0 Then
            ReturnResult.Warning_Add(ErrorCount & " tiles had an invalid cliff down side.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_Roads(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim ErrorCount As Integer

        Try
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
                    File.Write(CByte(Value))
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
                    File.Write(CByte(Value))
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

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

        Try
            For A = 0 To UnitCount - 1
                tmpUnit = Units(A)
                File.SectionName_Append(InvariantToString_int(A))
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
                            File.Property_Append("TurretCount", InvariantToString_byte(tmpDroid.TurretCount))
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
                File.Property_Append("ID", InvariantToString_sng(tmpUnit.ID))
                File.Property_Append("Priority", InvariantToString_int(tmpUnit.SavePriority))
                File.Property_Append("Pos", InvariantToString_int(tmpUnit.Pos.Horizontal.X) & ", " & InvariantToString_int(tmpUnit.Pos.Horizontal.Y))
                File.Property_Append("Heading", InvariantToString_int(tmpUnit.Rotation))
                If tmpUnit.UnitGroup.Map_UnitGroupNum < 0 Then
                    strTemp = "scavenger"
                Else
                    strTemp = InvariantToString_int(tmpUnit.UnitGroup.Map_UnitGroupNum)
                End If
                File.Property_Append("UnitGroup", strTemp)
                If tmpUnit.Health < 1.0# Then
                    File.Property_Append("Health", InvariantToString_dbl(tmpUnit.Health))
                End If
                File.Gap_Append()
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        If WarningCount > 0 Then
            ReturnResult.Warning_Add("Error: " & WarningCount & " units were of an unhandled type.")
        End If

        Return ReturnResult
    End Function

    Public Function Data_FMap_Gateways(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult
        Dim A As Integer

        Try
            For A = 0 To GatewayCount - 1
                File.SectionName_Append(InvariantToString_int(A))
                File.Property_Append("AX", InvariantToString_int(Gateways(A).PosA.X))
                File.Property_Append("AY", InvariantToString_int(Gateways(A).PosA.Y))
                File.Property_Append("BX", InvariantToString_int(Gateways(A).PosB.X))
                File.Property_Append("BY", InvariantToString_int(Gateways(A).PosB.Y))
                File.Gap_Append()
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Data_FMap_TileTypes(ByVal File As IO.BinaryWriter) As clsResult
        Dim ReturnResult As New clsResult
        Dim A As Integer

        Try
            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    File.Write(CByte(Tile_TypeNum(A)))
                Next
            End If
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function Load_FMap(ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult

        Dim ZipSearchResult As clsZipStreamEntry
        Dim FindPath As String

        Dim ResultInfo As clsFMapInfo = Nothing

        FindPath = "info.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Problem_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
            Return ReturnResult
        Else
            Dim Info_StreamReader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.Append(Read_FMap_Info(Info_StreamReader, ResultInfo), "Read info: ")
            Info_StreamReader.Close()
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
            Dim VertexHeight_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_VertexHeight(VertexHeight_Reader), "Read vertex height: ")
            VertexHeight_Reader.Close()
        End If

        FindPath = "vertexterrain.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim VertexTerrain_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_VertexTerrain(VertexTerrain_Reader), "Read vertex terrain: ")
            VertexTerrain_Reader.Close()
        End If

        FindPath = "tiletexture.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileTexture_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_TileTexture(TileTexture_Reader), "Read tile texture: ")
            TileTexture_Reader.Close()
        End If

        FindPath = "tileorientation.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileOrientation_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_TileOrientation(TileOrientation_Reader), "Read tile orientation: ")
            TileOrientation_Reader.Close()
        End If

        FindPath = "tilecliff.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileCliff_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_TileCliff(TileCliff_Reader), "Read tile cliff: ")
            TileCliff_Reader.Close()
        End If

        FindPath = "roads.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Roads_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_Roads(Roads_Reader), "Read roads: ")
            Roads_Reader.Close()
        End If

        FindPath = "objects.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Objects_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_Objects(Objects_Reader), "Read objects: ")
            Objects_Reader.Close()
        End If

        FindPath = "gateways.ini"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim Gateway_Reader As New IO.StreamReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_Gateways(Gateway_Reader), "Read gateways: ")
            Gateway_Reader.Close()
        End If

        FindPath = "tiletypes.dat"
        ZipSearchResult = FindZipEntryFromPath(Path, FindPath)
        If ZipSearchResult Is Nothing Then
            ReturnResult.Warning_Add("Unable to find file " & ControlChars.Quote & FindPath & ControlChars.Quote & ".")
        Else
            Dim TileTypes_Reader As New IO.BinaryReader(ZipSearchResult.Stream)
            ReturnResult.AppendAsWarning(Read_FMap_TileTypes(TileTypes_Reader), "Read tile types: ")
            TileTypes_Reader.Close()
        End If

        InterfaceOptions = ResultInfo.InterfaceOptions

        Return ReturnResult
    End Function

    Public Class clsFMapInfo
        Inherits clsINIRead.clsTranslator

        Public TerrainSize As sXY_int = New sXY_int(-1, -1)
        Public InterfaceOptions As New clsMap.clsInterfaceOptions
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
                    If Not InvariantParse_int(CommaText(0), NewSize.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If Not InvariantParse_int(CommaText(1), NewSize.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If NewSize.X < 1 Or NewSize.Y < 1 Or NewSize.X > MapMaxSize Or NewSize.Y > MapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    TerrainSize = NewSize
                Case "autoscrolllimits"
                    If Not InvariantParse_bool(INIProperty.Value, InterfaceOptions.AutoScrollLimits) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollminx"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.ScrollMin.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollminy"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.ScrollMin.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollmaxx"
                    If Not InvariantParse_uint(INIProperty.Value, InterfaceOptions.ScrollMax.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "scrollmaxy"
                    If Not InvariantParse_uint(INIProperty.Value, InterfaceOptions.ScrollMax.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "name"
                    InterfaceOptions.CompileName = INIProperty.Value
                Case "players"
                    InterfaceOptions.CompileMultiPlayers = INIProperty.Value
                Case "xplayerlev"
                    If Not InvariantParse_bool(INIProperty.Value, InterfaceOptions.CompileMultiXPlayers) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "author"
                    InterfaceOptions.CompileMultiAuthor = INIProperty.Value
                Case "license"
                    InterfaceOptions.CompileMultiLicense = INIProperty.Value
                Case "camptime"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.CampaignGameTime) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "camptype"
                    If Not InvariantParse_int(INIProperty.Value, InterfaceOptions.CampaignGameType) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Private Function Read_FMap_Info(ByVal File As IO.StreamReader, ByRef ResultInfo As clsFMapInfo) As clsResult
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

    Private Function Read_FMap_VertexHeight(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    Terrain.Vertices(X, Y).Height = File.ReadByte
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_VertexTerrain(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim byteTemp As Byte
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X
                    byteTemp = File.ReadByte
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
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " painted terrain vertices were invalid.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTexture(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim byteTemp As Byte

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    byteTemp = File.ReadByte
                    Terrain.Tiles(X, Y).Texture.TextureNum = CInt(byteTemp) - 1
                Next
            Next
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileOrientation(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim PartValue As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Value = File.ReadByte

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
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " tiles had unknown bits used.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileCliff(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim PartValue As Integer
        Dim DownSideWarningCount As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1

                    Value = File.ReadByte

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
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " tiles had unknown bits used.")
        End If
        If DownSideWarningCount > 0 Then
            ReturnResult.Warning_Add(DownSideWarningCount & " tiles had invalid down side values.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_Roads(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim X As Integer
        Dim Y As Integer
        Dim Value As Integer
        Dim WarningCount As Integer

        Try
            For Y = 0 To Terrain.TileSize.Y
                For X = 0 To Terrain.TileSize.X - 1
                    Value = File.ReadByte - 1
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
                    Value = File.ReadByte - 1
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
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If WarningCount > 0 Then
            ReturnResult.Warning_Add(WarningCount & " sides had an invalid road value.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function

    Private Function Read_FMap_Objects(ByVal File As IO.StreamReader) As clsResult
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
                                If Not InvariantParse_uint(INIObjects.Objects(A).UnitGroup, PlayerNum) Then
                                    Throw New Exception
                                End If
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
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosA.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "ay"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosA.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "bx"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosB.X) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "by"
                    If Not InvariantParse_int(INIProperty.Value, Gateways(INISectionNum).PosB.Y) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Read_FMap_Gateways(ByVal File As IO.StreamReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim GatewaysINI As New clsINIRead
        ReturnResult.Append(GatewaysINI.ReadFile(File), "")

        Dim INIGateways As New clsFMap_INIGateways(GatewaysINI.SectionCount)
        ReturnResult.Append(GatewaysINI.Translate(INIGateways), "")

        Dim A As Integer
        Dim InvalidGatewayCount As Integer = 0

        For A = 0 To INIGateways.GatewayCount - 1
            If Gateway_Create(INIGateways.Gateways(A).PosA, INIGateways.Gateways(A).PosB) Is Nothing Then
                InvalidGatewayCount += 1
            End If
        Next

        If InvalidGatewayCount > 0 Then
            ReturnResult.Warning_Add(InvalidGatewayCount & " gateways were invalid.")
        End If

        Return ReturnResult
    End Function

    Public Function Read_FMap_TileTypes(ByVal File As IO.BinaryReader) As clsResult
        Dim ReturnResult As New clsResult

        Dim A As Integer
        Dim byteTemp As Byte
        Dim InvalidTypeCount As Integer

        Try
            If Tileset IsNot Nothing Then
                For A = 0 To Tileset.TileCount - 1
                    byteTemp = File.ReadByte()
                    If byteTemp >= TileTypeCount Then
                        InvalidTypeCount += 1
                    Else
                        Tile_TypeNum(A) = byteTemp
                    End If
                Next
            End If
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
            Return ReturnResult
        End Try

        If InvalidTypeCount > 0 Then
            ReturnResult.Warning_Add(InvalidTypeCount & " tile types were invalid.")
        End If

        If File.PeekChar >= 0 Then
            ReturnResult.Warning_Add("There were unread bytes at the end of the file.")
        End If

        Return ReturnResult
    End Function
End Class