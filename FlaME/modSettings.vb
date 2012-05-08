
Public Module modSettings

    Public InitializeINISettings As clsINISettings

    Public Class clsSettings

        Public AutoSaveEnabled As Boolean = True
        Public AutoSaveCompress As Boolean = False
        Public AutoSaveMinInterval_s As UInteger = 180UI
        Public AutoSaveMinChanges As UInteger = 20UI
        Public UndoLimit As UInteger = 256UI
        Public DirectoriesPrompt As Boolean = True
        Public DirectPointer As Boolean = True
        Public DisplayFont As Font 'set by INI settings class
        Public MinimapSize As Integer = 160
        Public MinimapTeamColours As Boolean = True
        Public MinimapTeamColoursExceptFeatures As Boolean = True
        Public MinimapCliffColour As New clsRGBA_sng(1.0F, 0.25F, 0.25F, 0.5F)
        Public MinimapSelectedObjectsColour As New clsRGBA_sng(1.0F, 1.0F, 1.0F, 0.75F)
        Public FOVDefault As Double = 30.0# / (50.0# * 900.0#) ' screen_vertical_size / ( screen_dist * screen_vertical_pixels )
        Public Mipmaps As Boolean = True
        Public MipmapsHardware As Boolean = False
        Public OpenPath As String = Nothing
        Public SavePath As String = Nothing
        Public MapViewBPP As Integer = 32
        Public TextureViewBPP As Integer = 32
        Public MapViewDepth As Integer = 24
        Public TextureViewDepth As Integer = 24
    End Class

    Public Class clsINISettings
        Inherits clsINIRead.clsTranslator

        Public FontFamily As String = "Verdana"
        Public FontBold As Boolean = True
        Public FontItalic As Boolean = False
        Public FontSize As Single = 20.0F

        Public NewSettings As New clsSettings

        Public TilesetsPaths As New SimpleList(Of String)
        Public ObjectDataPaths As New SimpleList(Of String)

        Public DefaultTilesetPathNum As Integer = -1
        Public DefaultObjectDataPathNum As Integer = -1

        Public Overrides Function Translate(INIProperty As clsINIRead.clsSection.sProperty) As clsINIRead.enumTranslatorResult

            Select Case INIProperty.Name
                Case "directpointer"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.DirectPointer) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "fontfamily"
                    FontFamily = INIProperty.Value
                Case "fontbold"
                    If Not InvariantParse_bool(INIProperty.Value, FontBold) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "fontitalic"
                    If Not InvariantParse_bool(INIProperty.Value, FontItalic) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "fontsize"
                    Dim sngTemp As Single
                    If Not InvariantParse_sng(INIProperty.Value, sngTemp) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If sngTemp <= 0.0F Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    FontSize = sngTemp
                Case "minimapsize"
                    Dim Size As Integer
                    If Not InvariantParse_int(INIProperty.Value, Size) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    If Size < 0 Or Size > MinimapMaxSize Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                    NewSettings.MinimapSize = Size
                Case "minimapteamcolours"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.MinimapTeamColours) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "minimapteamcoloursexceptfeatures"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.MinimapTeamColoursExceptFeatures) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "minimapcliffcolour"
                    If Not NewSettings.MinimapCliffColour.ReadINIText(New clsSplitCommaText(INIProperty.Value)) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "minimapselectedobjectscolour"
                    If Not NewSettings.MinimapSelectedObjectsColour.ReadINIText(New clsSplitCommaText(INIProperty.Value)) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "undolimit"
                    If Not InvariantParse_uint(INIProperty.Value, NewSettings.UndoLimit) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "autosave"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.AutoSaveEnabled) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "autosavemininterval"
                    If Not InvariantParse_uint(INIProperty.Value, NewSettings.AutoSaveMinInterval_s) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "autosaveminchanges"
                    If Not InvariantParse_uint(INIProperty.Value, NewSettings.AutoSaveMinChanges) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "autosavecompress"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.AutoSaveCompress) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "tilesetspath"
                    TilesetsPaths.Add(INIProperty.Value)
                Case "objectdatapath"
                    ObjectDataPaths.Add(INIProperty.Value)
                Case "defaulttilesetspathnum"
                    If Not InvariantParse_int(INIProperty.Value, DefaultTilesetPathNum) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "defaultobjectdatapathnum"
                    If Not InvariantParse_int(INIProperty.Value, DefaultObjectDataPathNum) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "directoriesprompt"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.DirectoriesPrompt) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "fovdefault"
                    If Not InvariantParse_dbl(INIProperty.Value, NewSettings.FOVDefault) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "mipmaps"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.Mipmaps) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "mipmapshardware"
                    If Not InvariantParse_bool(INIProperty.Value, NewSettings.MipmapsHardware) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "openpath"
                    NewSettings.OpenPath = INIProperty.Value
                Case "savepath"
                    NewSettings.SavePath = INIProperty.Value
                Case "mapviewbpp"
                    If Not InvariantParse_int(INIProperty.Value, NewSettings.MapViewBPP) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "textureviewbpp"
                    If Not InvariantParse_int(INIProperty.Value, NewSettings.TextureViewBPP) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "mapviewdepth"
                    If Not InvariantParse_int(INIProperty.Value, NewSettings.MapViewDepth) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case "textureviewdepth"
                    If Not InvariantParse_int(INIProperty.Value, NewSettings.TextureViewDepth) Then
                        Return clsINIRead.enumTranslatorResult.ValueInvalid
                    End If
                Case Else
                    Return clsINIRead.enumTranslatorResult.NameUnknown
            End Select
            Return clsINIRead.enumTranslatorResult.Translated
        End Function
    End Class

    Public Function Read_Settings(File As IO.StreamReader, ByRef Result As clsINISettings) As clsResult
        Dim ReturnResult As New clsResult("Reading settings")

        Dim INISection As New clsINIRead.clsSection
        ReturnResult.Take(INISection.ReadFile(File))
        Result = New clsINISettings
        ReturnResult.Take(INISection.Translate(Result))

        Return ReturnResult
    End Function

    Public Sub UpdateINISettings(NewSettingsINI As clsINISettings)

        Dim FontStyle As Drawing.FontStyle = FontStyle.Regular
        If NewSettingsINI.FontBold Then
            FontStyle = (FontStyle Or FontStyle.Bold)
        End If
        If NewSettingsINI.FontItalic Then
            FontStyle = (FontStyle Or FontStyle.Italic)
        End If
        NewSettingsINI.NewSettings.DisplayFont = New Font(NewSettingsINI.FontFamily, Math.Max(NewSettingsINI.FontSize, 1.0F), FontStyle)

        UpdateSettings(NewSettingsINI.NewSettings)

        frmDataInstance.TilesetsPathSet.SetPaths(NewSettingsINI.TilesetsPaths)
        If NewSettingsINI.DefaultTilesetPathNum >= -1 And NewSettingsINI.DefaultTilesetPathNum < NewSettingsINI.TilesetsPaths.Count Then
            frmDataInstance.TilesetsPathSet.SelectedNum = NewSettingsINI.DefaultTilesetPathNum
        End If
        frmDataInstance.ObjectDataPathSet.SetPaths(NewSettingsINI.ObjectDataPaths)
        If NewSettingsINI.DefaultObjectDataPathNum >= -1 And NewSettingsINI.DefaultObjectDataPathNum < NewSettingsINI.ObjectDataPaths.Count Then
            frmDataInstance.ObjectDataPathSet.SelectedNum = NewSettingsINI.DefaultObjectDataPathNum
        End If
    End Sub

    Public Sub UpdateSettings(NewSettings As clsSettings)
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

    Private Sub SetFont(NewFont As Font)

        If UnitLabelFont IsNot Nothing Then
            UnitLabelFont.Deallocate()
        End If
        'If TextureViewFont IsNot Nothing Then
        '    TextureViewFont.Deallocate()
        'End If
        UnitLabelFont = frmMainInstance.MapView.CreateGLFont(NewFont)
        'TextureViewFont = frmMainInstance.TextureView.CreateGLFont(NewFont)
    End Sub

    Public Function Settings_Write() As clsResult
        Dim ReturnResult As New clsResult("Writing settings to " & ControlChars.Quote & SettingsPath & ControlChars.Quote)

#If Portable = 0.0# Then
        If Not IO.Directory.Exists(MyDocumentsProgramPath) Then
            Try
                IO.Directory.CreateDirectory(MyDocumentsProgramPath)
            Catch ex As Exception
                ReturnResult.ProblemAdd("Unable to create folder " & ControlChars.Quote & MyDocumentsProgramPath & ControlChars.Quote & ": " & ex.Message)
                Return ReturnResult
            End Try
        End If
#End If

        Dim INI_Settings As clsINIWrite

        Try
            INI_Settings = clsINIWrite.CreateFile(IO.File.Create(SettingsPath))
        Catch ex As Exception
            ReturnResult.ProblemAdd(ex.Message)
            Return ReturnResult
        End Try

        Serialize_Settings(INI_Settings)
        INI_Settings.File.Close()

        Return ReturnResult
    End Function

    Private Sub Serialize_Settings(File As clsINIWrite)
        Dim ReturnResult As New clsResult("Serializing settings")

        File.Property_Append("DirectPointer", InvariantToString_bool(Settings.DirectPointer))
        If UnitLabelFont IsNot Nothing Then
            File.Property_Append("FontFamily", Settings.DisplayFont.FontFamily.Name)
            File.Property_Append("FontBold", InvariantToString_bool(Settings.DisplayFont.Bold))
            File.Property_Append("FontItalic", InvariantToString_bool(Settings.DisplayFont.Italic))
            File.Property_Append("FontSize", InvariantToString_sng(Settings.DisplayFont.SizeInPoints))
        End If
        File.Property_Append("MinimapSize", InvariantToString_int(Settings.MinimapSize))
        File.Property_Append("MinimapTeamColours", InvariantToString_bool(Settings.MinimapTeamColours))
        File.Property_Append("MinimapTeamColoursExceptFeatures", InvariantToString_bool(Settings.MinimapTeamColoursExceptFeatures))
        File.Property_Append("MinimapCliffColour", Settings.MinimapCliffColour.GetINIOutput)
        File.Property_Append("MinimapSelectedObjectsColour", Settings.MinimapSelectedObjectsColour.GetINIOutput)
        File.Property_Append("UndoLimit", InvariantToString_sng(Settings.UndoLimit))
        File.Property_Append("AutoSave", InvariantToString_bool(Settings.AutoSaveEnabled))
        File.Property_Append("AutoSaveMinInterval", InvariantToString_sng(Settings.AutoSaveMinInterval_s))
        File.Property_Append("AutoSaveMinChanges", InvariantToString_sng(Settings.AutoSaveMinChanges))
        File.Property_Append("AutoSaveCompress", InvariantToString_bool(Settings.AutoSaveCompress))
        File.Property_Append("DirectoriesPrompt", InvariantToString_bool(Settings.DirectoriesPrompt))
        File.Property_Append("FOVDefault", InvariantToString_dbl(Settings.FOVDefault))
        Dim A As Integer
        Dim Paths() As String
        Paths = frmDataInstance.TilesetsPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("TilesetsPath", Paths(A))
        Next
        A = frmDataInstance.TilesetsPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultTilesetsPathNum", InvariantToString_int(A))
        End If
        Paths = frmDataInstance.ObjectDataPathSet.GetPaths
        For A = 0 To Paths.GetUpperBound(0)
            File.Property_Append("ObjectDataPath", Paths(A))
        Next
        A = frmDataInstance.ObjectDataPathSet.SelectedNum
        If A >= 0 Then
            File.Property_Append("DefaultObjectDataPathNum", InvariantToString_int(A))
        End If
        File.Property_Append("Mipmaps", InvariantToString_bool(Settings.Mipmaps))
        File.Property_Append("MipmapsHardware", InvariantToString_bool(Settings.MipmapsHardware))
        If Settings.OpenPath IsNot Nothing Then
            File.Property_Append("OpenPath", Settings.OpenPath)
        End If
        If Settings.SavePath IsNot Nothing Then
            File.Property_Append("SavePath", Settings.SavePath)
        End If
        File.Property_Append("MapViewBPP", InvariantToString_int(Settings.MapViewBPP))
        File.Property_Append("TextureViewBPP", InvariantToString_int(Settings.TextureViewBPP))
        File.Property_Append("MapViewDepth", InvariantToString_int(Settings.MapViewDepth))
        File.Property_Append("TextureViewDepth", InvariantToString_int(Settings.TextureViewDepth))
    End Sub

    Public Function Settings_Load(ByRef Result As clsINISettings) As clsResult
        Dim ReturnResult As New clsResult("Loading settings from " & ControlChars.Quote & SettingsPath & ControlChars.Quote)

        Dim File_Settings As IO.StreamReader
        Try
            File_Settings = New IO.StreamReader(SettingsPath)
        Catch ex As Exception
            Result = New clsINISettings
            Return ReturnResult
        End Try

        ReturnResult.Take(Read_Settings(File_Settings, Result))

        File_Settings.Close()

        Return ReturnResult
    End Function
End Module
