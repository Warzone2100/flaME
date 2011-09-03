Public Module modSettings

    Public Class clsSettings

        Public AutoSaveEnabled As Boolean = True
        Public AutoSaveCompress As Boolean = False
        Public AutoSave_MinInterval_s As UInteger = 180UI
        Public AutoSave_MinChanges As UInteger = 20UI
        Public Undo_Limit As UInteger = 256UI
        Public DirectoriesPrompt As Boolean = True
        Public DirectPointer As Boolean = True
        Public DisplayFont As Font 'set by INI settings class
        Public MinimapSize As Integer = 160
        Public MinimapTeamColours As Boolean = True
        Public MinimapTeamColoursExceptFeatures As Boolean = True
        Public FOVDefault As Double = 30.0# / (50.0# * 900.0#) ' screen_vertical_size / ( screen_dist * screen_vertical_pixels )
    End Class

    Public Class clsINISettings
        Inherits clsINIRead.clsTranslator

        Public FontFamily As String = "Verdana"
        Public FontBold As Boolean = True
        Public FontItalic As Boolean = False
        Public FontSize As Single = 20.0F

        Public NewSettings As New clsSettings

        Public TilesetsPaths(-1) As String
        Public TilesetsPathCount As Integer = 0
        Public ObjectDataPaths(-1) As String
        Public ObjectDataPathCount As Integer = 0

        Public DefaultTilesetPathNum As Integer = -1
        Public DefaultObjectDataPathNum As Integer = -1

        Public Overrides Function Translate(ByVal INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "directpointer"
                    Try
                        NewSettings.DirectPointer = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontfamily"
                    FontFamily = INIProperty.Value
                Case "fontbold"
                    Try
                        FontBold = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontitalic"
                    Try
                        FontItalic = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fontsize"
                    Dim sngTemp As Single
                    Try
                        sngTemp = CSng(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If sngTemp <= 0.0F Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    FontSize = sngTemp
                Case "minimapsize"
                    Dim Size As Integer
                    Try
                        Size = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                    If Size < 0 Or Size > MinimapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    NewSettings.MinimapSize = Size
                Case "minimapteamcolours"
                    Try
                        NewSettings.MinimapTeamColours = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "minimapteamcoloursexceptfeatures"
                    Try
                        NewSettings.MinimapTeamColoursExceptFeatures = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "undolimit"
                    Try
                        NewSettings.Undo_Limit = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosave"
                    Try
                        NewSettings.AutoSaveEnabled = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosavemininterval"
                    Try
                        NewSettings.AutoSave_MinInterval_s = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosaveminchanges"
                    Try
                        NewSettings.AutoSave_MinChanges = CUInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "autosavecompress"
                    Try
                        NewSettings.AutoSaveCompress = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "tilesetspath"
                    ReDim Preserve TilesetsPaths(TilesetsPathCount)
                    TilesetsPaths(TilesetsPathCount) = INIProperty.Value
                    TilesetsPathCount += 1
                Case "objectdatapath"
                    ReDim Preserve ObjectDataPaths(ObjectDataPathCount)
                    ObjectDataPaths(ObjectDataPathCount) = INIProperty.Value
                    ObjectDataPathCount += 1
                Case "defaulttilesetspathnum"
                    Try
                        DefaultTilesetPathNum = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "defaultobjectdatapathnum"
                    Try
                        DefaultObjectDataPathNum = CInt(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "directoriesprompt"
                    Try
                        NewSettings.DirectoriesPrompt = CBool(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case "fovdefault"
                    Try
                        NewSettings.FOVDefault = CDbl(INIProperty.Value)
                    Catch ex As Exception
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End Try
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Read_Settings(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim INISection As New clsINIRead.clsSection
        ReturnResult.Append(INISection.ReadFile(File), "")
        Dim NewSettingsINI As New clsINISettings
        ReturnResult.Append(INISection.Translate(NewSettingsINI), "")

        UpdateINISettings(NewSettingsINI)

        Return ReturnResult
    End Function

    Private Sub UpdateINISettings(ByVal NewSettingsINI As clsINISettings)

        Dim tmpFontStyle As Drawing.FontStyle = FontStyle.Regular
        If NewSettingsINI.FontBold Then
            tmpFontStyle = CType(tmpFontStyle + FontStyle.Bold, FontStyle)
        End If
        If NewSettingsINI.FontItalic Then
            tmpFontStyle = CType(tmpFontStyle + FontStyle.Italic, FontStyle)
        End If
        NewSettingsINI.NewSettings.DisplayFont = New Font(NewSettingsINI.FontFamily, Math.Max(NewSettingsINI.FontSize, 1.0F), tmpFontStyle)

        UpdateSettings(NewSettingsINI.NewSettings)

        frmDataInstance.TilesetsPathSet.SetPaths(NewSettingsINI.TilesetsPaths)
        If NewSettingsINI.DefaultTilesetPathNum >= -1 And NewSettingsINI.DefaultTilesetPathNum < NewSettingsINI.TilesetsPathCount Then
            frmDataInstance.TilesetsPathSet.SelectedNum = NewSettingsINI.DefaultTilesetPathNum
        End If
        frmDataInstance.ObjectDataPathSet.SetPaths(NewSettingsINI.ObjectDataPaths)
        If NewSettingsINI.DefaultObjectDataPathNum >= -1 And NewSettingsINI.DefaultObjectDataPathNum < NewSettingsINI.ObjectDataPathCount Then
            frmDataInstance.ObjectDataPathSet.SelectedNum = NewSettingsINI.DefaultObjectDataPathNum
        End If
    End Sub

    Public Sub UpdateSettings(ByVal NewSettings As clsSettings)
        Dim FontChanged As Boolean

        If Settings Is Nothing Then
            FontChanged = True
        Else
            If Settings.DisplayFont Is Nothing Then
                If NewSettings.DisplayFont Is Nothing Then
                    FontChanged = False
                Else
                    FontChanged = True
                End If
            Else
                If NewSettings.DisplayFont Is Nothing Then
                    FontChanged = True
                Else
                    If Settings.DisplayFont.FontFamily.Name = NewSettings.DisplayFont.FontFamily.Name _
                        And Settings.DisplayFont.Style = NewSettings.DisplayFont.Style _
                        And Settings.DisplayFont.SizeInPoints = NewSettings.DisplayFont.SizeInPoints Then
                        FontChanged = False
                    Else
                        FontChanged = True
                    End If
                End If
            End If
        End If
        If FontChanged Then
            SetFont(NewSettings.DisplayFont)
        End If

        Settings = NewSettings
    End Sub

    Public Function Read_OldSettings(ByVal File As clsReadFile) As Boolean
        Dim ReturnResult As Boolean = False

        Dim NewSettingsINI As New clsINISettings
        Dim uintTemp As UInteger
        Dim byteTemp As Byte
        Dim strTemp As String = ""
        Dim Version As UInteger
        Dim sngTemp As Single

        If Not File.Get_U32(Version) Then
            Return ReturnResult
        End If
        If Version <> 4UI And Version <> 5UI Then
            Return ReturnResult
        End If
        If Not File.Get_U32(uintTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.Undo_Limit = uintTemp
        If Not File.Get_U32(uintTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.AutoSave_MinInterval_s = uintTemp
        If Not File.Get_U32(uintTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.AutoSave_MinChanges = uintTemp
        If Not File.Get_U8(byteTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.AutoSaveEnabled = (byteTemp > 0)
        If Not File.Get_U8(byteTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.DirectPointer = (byteTemp > 0)
        If Not File.Get_Text_VariableLength(strTemp) Then
            Return ReturnResult
        End If
        Dim BoldByte As Byte
        Dim ItalicByte As Byte
        If Version = 5UI Then
            If Not File.Get_U8(BoldByte) Then
                Return ReturnResult
            End If
            If Not File.Get_U8(ItalicByte) Then
                Return ReturnResult
            End If
        End If
        Dim tmpFontStyle As Drawing.FontStyle = FontStyle.Regular
        If BoldByte > 0 Then
            tmpFontStyle = CType(tmpFontStyle + FontStyle.Bold, FontStyle)
        End If
        If ItalicByte > 0 Then
            tmpFontStyle = CType(tmpFontStyle + FontStyle.Italic, FontStyle)
        End If
        If Not File.Get_F32(sngTemp) Then
            Return ReturnResult
        End If
        NewSettingsINI.NewSettings.DisplayFont = New Font(strTemp, Math.Max(sngTemp, 1.0F), tmpFontStyle)

        ReturnResult = True
        Return ReturnResult
    End Function

    Private Sub SetFont(ByVal NewFont As Font)

        If UnitLabelFont IsNot Nothing Then
            UnitLabelFont.Deallocate()
        End If
        If TextureViewFont IsNot Nothing Then
            TextureViewFont.Deallocate()
        End If
        UnitLabelFont = frmMainInstance.View.CreateGLFont(NewFont)
        TextureViewFont = frmMainInstance.TextureView.CreateGLFont(NewFont)
    End Sub

    Public Function Settings_Write() As clsResult
        Dim ReturnResult As New clsResult

#If Portable = 0.0# Then
        If Not IO.Directory.Exists(MyDocumentsProgramPath) Then
            Try
                IO.Directory.CreateDirectory(MyDocumentsProgramPath)
            Catch ex As Exception
                ReturnResult.Problem_Add("Unable to create folder " & ControlChars.Quote & MyDocumentsProgramPath & ControlChars.Quote & ": " & ex.Message)
                Return ReturnResult
            End Try
        End If
#End If

        Dim INI_Settings As clsINIWrite = CreateINIWriteFile()

        ReturnResult.Append(Data_Settings(INI_Settings), "Compile settings.ini: ")

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        ReturnResult.Append(INI_Settings.File.WriteFile(SettingsPath, True), "Write settings.ini: ")

        Return ReturnResult
    End Function

    Private Function Data_Settings(ByVal File As clsINIWrite) As clsResult
        Dim ReturnResult As New clsResult

        File.Property_Append("DirectPointer", CStr(Settings.DirectPointer))
        If UnitLabelFont IsNot Nothing Then
            File.Property_Append("FontFamily", Settings.DisplayFont.FontFamily.Name)
            File.Property_Append("FontBold", CStr(Settings.DisplayFont.Bold))
            File.Property_Append("FontItalic", CStr(Settings.DisplayFont.Italic))
            File.Property_Append("FontSize", CStr(Settings.DisplayFont.SizeInPoints))
        End If
        File.Property_Append("MinimapSize", CStr(Settings.MinimapSize))
        File.Property_Append("MinimapTeamColours", CStr(Settings.MinimapTeamColours))
        File.Property_Append("MinimapTeamColoursExceptFeatures", CStr(Settings.MinimapTeamColoursExceptFeatures))
        File.Property_Append("UndoLimit", CStr(Settings.Undo_Limit))
        File.Property_Append("AutoSave", CStr(Settings.AutoSaveEnabled))
        File.Property_Append("AutoSaveMinInterval", CStr(Settings.AutoSave_MinInterval_s))
        File.Property_Append("AutoSaveMinChanges", CStr(Settings.AutoSave_MinChanges))
        File.Property_Append("AutoSaveCompress", CStr(Settings.AutoSaveCompress))
        File.Property_Append("DirectoriesPrompt", CStr(Settings.DirectoriesPrompt))
        File.Property_Append("FOVDefault", CStr(Settings.FOVDefault))
        Dim A As Integer
        Dim Paths() As String
        Paths = frmDataInstance.TilesetsPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("TilesetsPath", Paths(A))
        Next
        A = frmDataInstance.TilesetsPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultTilesetsPathNum", CStr(A))
        End If
        Paths = frmDataInstance.ObjectDataPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("ObjectDataPath", Paths(A))
        Next
        A = frmDataInstance.ObjectDataPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultObjectDataPathNum", CStr(A))
        End If

        Return ReturnResult
    End Function

    Public Function Settings_Load() As clsResult
        Dim ReturnResult As New clsResult

        Dim File_Settings As New clsReadFile
        If File_Settings.Begin(SettingsPath).Success Then
            ReturnResult.Append(Read_Settings(File_Settings), "Read settings.ini: ")
            File_Settings.Close()
        Else
#If Portable = 0.0# Then
            Dim File_OldSettings As New clsReadFile
            If File_OldSettings.Begin(OldSettingsPath).Success Then
                Read_OldSettings(File_OldSettings)
                File_OldSettings.Close()
            Else
                Dim NewINISettings As New clsINISettings
                UpdateINISettings(NewINISettings)
            End If
#Else
            Dim NewINISettings As New clsINISettings
            UpdateINISettings(NewINISettings)
#End If
        End If

        Return ReturnResult
    End Function
End Module