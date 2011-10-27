Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Private Sub ScriptMarker_Add(ByVal Item As clsScriptMarker)

        If Item.Map_MarkerNum >= 0 Then
            Stop
            Exit Sub
        End If

        Item.Map_MarkerNum = ScriptMarkerCount

        If ScriptMarkers.GetUpperBound(0) < ScriptMarkerCount Then
            ReDim Preserve ScriptMarkers(ScriptMarkerCount * 2 + 1)
        End If
        ScriptMarkers(ScriptMarkerCount) = Item
        ScriptMarkerCount += 1
    End Sub

    Private Sub ScriptMarker_Remove(ByVal Num As Integer)

        If ScriptMarkers(Num).Map_MarkerNum <> Num Then
            Stop
        End If

        ScriptMarkers(Num).Map_MarkerNum = -1

        ScriptMarkerCount -= 1
        If Num < ScriptMarkerCount Then
            ScriptMarkers(Num) = ScriptMarkers(ScriptMarkerCount)
            ScriptMarkers(Num).Map_MarkerNum = Num
        End If
        ScriptMarkers(ScriptMarkerCount) = Nothing
        If ScriptMarkerCount * 3 < ScriptMarkers.GetUpperBound(0) + 1 Then
            ReDim Preserve ScriptMarkers(ScriptMarkerCount * 2 - 1)
        End If
    End Sub

    Public MustInherit Class clsScriptMarker

        Protected _Map As clsMap
        Public Map_MarkerNum As Integer = -1

        Protected _Label As String

        Public ReadOnly Property Label As String
            Get
                Return _Label
            End Get
        End Property

        Protected Sub New(ByVal Map As clsMap)

            _Map = Map
            Map.ScriptMarker_Add(Me)
        End Sub

        Public Sub Deallocate()

            If Map_MarkerNum < 0 Then
                Stop
                Exit Sub
            End If

            _Deallocate()
        End Sub

        Protected Overridable Sub _Deallocate()

            _Map.ScriptMarker_Remove(Map_MarkerNum)
            _Map = Nothing
            _Label = Nothing
        End Sub

        Public Sub GLDraw()

            _GLDraw()
        End Sub

        Protected Overridable Sub _GLDraw()

        End Sub

        Public Sub SetDefaultName(ByVal Prefix As String)
            Dim Number As Integer = 1
            Dim Result As sResult

            Do
                Result = SetLabel(Prefix & Number)
                If Result.Success Then
                    Exit Do
                End If
                Number += 1
                If Number >= 16384 Then
                    MsgBox("Error: Unable to set default script label.")
                    Exit Do
                End If
            Loop
        End Sub

        Public Function SetLabel(ByVal Text As String) As sResult
            Dim ReturnResult As sResult
            ReturnResult.Success = False
            ReturnResult.Problem = ""
            Dim A As Integer
            Dim tmpChar As Char
            Dim LCaseText As String = Text.ToLower

            If LCaseText.Length < 1 Then
                ReturnResult.Problem = "Label cannot be nothing."
                Return ReturnResult
            End If

            For A = 0 To LCaseText.Length - 1
                tmpChar = LCaseText.Chars(A)
                If Not ((tmpChar >= "a"c And tmpChar <= "z"c) Or (tmpChar >= "0"c And tmpChar <= "9"c) Or tmpChar = "_"c) Then
                    Exit For
                End If
            Next
            If A < LCaseText.Length Then
                ReturnResult.Problem = "Label contains invalid characters. Use only letters, numbers or underscores."
                Return ReturnResult
            End If

            For A = 0 To _Map.ScriptMarkerCount - 1
                If A <> Map_MarkerNum Then
                    If LCaseText = _Map.ScriptMarkers(A).Label.ToLower Then
                        Exit For
                    End If
                End If
            Next

            If A = _Map.ScriptMarkerCount Then
                _Label = Text
            Else
                ReturnResult.Problem = "Label text is already in use."
                Return ReturnResult
            End If

            ReturnResult.Success = True
            Return ReturnResult
        End Function

        Public Sub MapResizing(ByVal PosOffset As sXY_int)

            _MapResizing(PosOffset)
        End Sub

        Protected Overridable Sub _MapResizing(ByVal PosOffset As sXY_int)


        End Sub

        Public Sub WriteFMap(ByVal File As clsINIWrite)

            _WriteFMap(File)
        End Sub

        Protected Overridable Sub _WriteFMap(ByVal File As clsINIWrite)

        End Sub

        Public Sub WriteWZ(ByVal File As clsINIWrite)

            _WriteWZ(File)
        End Sub

        Protected Overridable Sub _WriteWZ(ByVal File As clsINIWrite)

        End Sub
    End Class

    Public Class clsScriptPosition
        Inherits clsScriptMarker

        Private _Pos As sXY_int
        Public Property PosX As Integer
            Get
                Return _Pos.X
            End Get
            Set(ByVal value As Integer)
                _Pos.X = Clamp_int(value, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            End Set
        End Property
        Public Property PosY As Integer
            Get
                Return _Pos.Y
            End Get
            Set(ByVal value As Integer)
                _Pos.Y = Clamp_int(value, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            End Set
        End Property

        Protected Sub New(ByVal Map As clsMap)
            MyBase.New(Map)

        End Sub

        Public Shared Function Create(ByVal Map As clsMap) As clsScriptPosition
            Dim Result As New clsScriptPosition(Map)

            If Result.Map_MarkerNum < 0 Then
                Return Nothing
            End If

            Result.SetDefaultName("Position")

            Return Result
        End Function

        Protected Overrides Sub _GLDraw()

            Dim Drawer As New clsMap.clsDrawHorizontalPosOnTerrain
            Drawer.Map = _Map
            Drawer.Horizontal = _Pos
            Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.0F, 0.75F)
            GL.LineWidth(3.0F)
            Drawer.ActionPerform()
        End Sub

        Protected Overrides Sub _MapResizing(ByVal PosOffset As sXY_int)
            MyBase._MapResizing(PosOffset)

            PosX = _Pos.X - PosOffset.X
            PosY = _Pos.Y - PosOffset.Y
        End Sub

        Protected Overrides Sub _WriteFMap(File As modINI.clsINIWrite)
            MyBase._WriteFMap(File)

            File.SectionName_Append(InvariantToString_int(Map_MarkerNum))
            File.Property_Append("Type", "Position")
            File.Property_Append("Label", _Label)
            File.Property_Append("Pos", InvariantToString_int(_Pos.X) & ", " & InvariantToString_int(_Pos.Y))
            File.Gap_Append()
        End Sub

        Protected Overrides Sub _WriteWZ(File As modINI.clsINIWrite)
            MyBase._WriteWZ(File)

            File.SectionName_Append("position_" & InvariantToString_int(Map_MarkerNum))
            File.Property_Append("pos", InvariantToString_int(_Pos.X) & ", " & InvariantToString_int(_Pos.Y))
            File.Property_Append("label", _Label)
            File.Gap_Append()
        End Sub
    End Class

    Public Class clsScriptArea
        Inherits clsScriptMarker

        Private _PosA As sXY_int
        Private _PosB As sXY_int
        Public WriteOnly Property PosA As sXY_int
            Set(ByVal value As sXY_int)
                _PosA.X = Clamp_int(value.X, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                _PosA.Y = Clamp_int(value.Y, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public WriteOnly Property PosB As sXY_int
            Set(ByVal value As sXY_int)
                _PosB.X = Clamp_int(value.X, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                _PosB.Y = Clamp_int(value.Y, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosAX As Integer
            Get
                Return _PosA.X
            End Get
            Set(ByVal value As Integer)
                _PosA.X = Clamp_int(value, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosAY As Integer
            Get
                Return _PosA.Y
            End Get
            Set(ByVal value As Integer)
                _PosA.Y = Clamp_int(value, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosBX As Integer
            Get
                Return _PosB.X
            End Get
            Set(ByVal value As Integer)
                _PosB.X = Clamp_int(value, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosBY As Integer
            Get
                Return _PosB.Y
            End Get
            Set(ByVal value As Integer)
                _PosB.Y = Clamp_int(value, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property

        Protected Sub New(ByVal Map As clsMap)
            MyBase.New(Map)

        End Sub

        Public Shared Function Create(ByVal Map As clsMap) As clsScriptArea
            Dim Result As New clsScriptArea(Map)

            If Result.Map_MarkerNum < 0 Then
                Return Nothing
            End If

            Result.SetDefaultName("Area")

            Return Result
        End Function

        Public Sub SetPositions(ByVal PosA As sXY_int, ByVal PosB As sXY_int)

            PosA.X = Clamp_int(PosA.X, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            PosA.Y = Clamp_int(PosA.Y, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            PosB.X = Clamp_int(PosB.X, 0, _Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            PosB.Y = Clamp_int(PosB.Y, 0, _Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)

            XY_Reorder(PosA, PosB, _PosA, _PosB)
        End Sub

        Protected Overrides Sub _GLDraw()

            Dim Drawer As New clsMap.clsDrawAreaOutline
            Drawer.Map = _Map
            Drawer.StartXY = _Map.GetPosVertexNum(_PosA)
            Drawer.FinishXY = _Map.GetPosVertexNum(_PosB)
            Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.0F, 0.5F)
            GL.LineWidth(3.0F)
            Drawer.ActionPerform()

            Dim PosDrawer As New clsMap.clsDrawHorizontalPosOnTerrain
            PosDrawer.Map = _Map
            PosDrawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.0F, 0.75F)
            GL.LineWidth(3.0F)

            PosDrawer.Horizontal = _PosA
            PosDrawer.ActionPerform()

            PosDrawer.Horizontal = _PosB
            PosDrawer.ActionPerform()
        End Sub

        Protected Overrides Sub _MapResizing(ByVal PosOffset As sXY_int)
            MyBase._MapResizing(PosOffset)

            SetPositions(New sXY_int(_PosA.X - PosOffset.X, _PosA.Y - PosOffset.Y), New sXY_int(_PosB.X - PosOffset.X, _PosB.Y - PosOffset.Y))
        End Sub

        Protected Overrides Sub _WriteFMap(File As modINI.clsINIWrite)
            MyBase._WriteFMap(File)

            File.SectionName_Append(InvariantToString_int(Map_MarkerNum))
            File.Property_Append("Type", "Area")
            File.Property_Append("Label", _Label)
            File.Property_Append("Pos", InvariantToString_int(_PosA.X) & ", " & InvariantToString_int(_PosA.Y) & ", " & InvariantToString_int(_PosB.X) & ", " & InvariantToString_int(_PosB.Y))
            File.Gap_Append()
        End Sub

        Protected Overrides Sub _WriteWZ(File As modINI.clsINIWrite)
            MyBase._WriteWZ(File)

            File.SectionName_Append("area_" & InvariantToString_int(Map_MarkerNum))
            File.Property_Append("pos", InvariantToString_int(_PosA.X) & ", " & InvariantToString_int(_PosA.Y) & ", " & InvariantToString_int(_PosB.X) & ", " & InvariantToString_int(_PosB.Y))
            File.Property_Append("label", _Label)
            File.Gap_Append()
        End Sub
    End Class

End Class