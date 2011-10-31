Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public ScriptPositions As New ConnectedList(Of clsScriptPosition, clsMap)(Me)
    Public ScriptAreas As New ConnectedList(Of clsScriptArea, clsMap)(Me)

    Public Function GetDefaultScriptLabel(ByVal Prefix As String) As String
        Dim Number As Integer = 1
        Dim Valid As sResult
        Dim Label As String

        Do
            Label = Prefix & InvariantToString_int(Number)
            Valid = ScriptLabelIsValid(Label)
            If Valid.Success Then
                Return Label
            End If
            Number += 1
            If Number >= 16384 Then
                MsgBox("Error: Unable to set default script label.")
                Return ""
            End If
        Loop
    End Function

    Public Function ScriptLabelIsValid(ByVal Text As String) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        If Text Is Nothing Then
            ReturnResult.Problem = "Label cannot be nothing."
            Return ReturnResult
        End If

        Dim LCaseText As String = Text.ToLower

        If LCaseText.Length < 1 Then
            ReturnResult.Problem = "Label cannot be nothing."
            Return ReturnResult
        End If

        Dim A As Integer
        Dim tmpChar As Char

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

        For A = 0 To UnitCount - 1
            If Units(A).Label IsNot Nothing Then
                If LCaseText = Units(A).Label.ToLower Then
                    ReturnResult.Problem = "Label text is already in use."
                    Return ReturnResult
                End If
            End If
        Next
        For A = 0 To ScriptPositions.ItemCount - 1
            If LCaseText = ScriptPositions.Item(A).Label.ToLower Then
                ReturnResult.Problem = "Label text is already in use."
                Return ReturnResult
            End If
        Next
        For A = 0 To ScriptAreas.ItemCount - 1
            If LCaseText = ScriptAreas.Item(A).Label.ToLower Then
                ReturnResult.Problem = "Label text is already in use."
                Return ReturnResult
            End If
        Next

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Class clsScriptPosition

        Private _ParentMap As New ConnectedListLink(Of clsScriptPosition, clsMap)(Me)
        Public ReadOnly Property ParentMap As ConnectedListLink(Of clsScriptPosition, clsMap)
            Get
                Return _ParentMap
            End Get
        End Property

        Private _Label As String
        Public ReadOnly Property Label As String
            Get
                Return _Label
            End Get
        End Property

        Private _Pos As sXY_int
        Public Property PosX As Integer
            Get
                Return _Pos.X
            End Get
            Set(ByVal value As Integer)
                _Pos.X = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.X * TerrainGridSpacing - 1)
            End Set
        End Property
        Public Property PosY As Integer
            Get
                Return _Pos.Y
            End Get
            Set(ByVal value As Integer)
                _Pos.Y = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            End Set
        End Property

        Private Sub New()

        End Sub

        Public Shared Function Create(ByVal Map As clsMap) As clsScriptPosition
            Dim Result As New clsScriptPosition

            Result._Label = Map.GetDefaultScriptLabel("Position")

            Result._ParentMap.Connect(Map.ScriptPositions)

            Return Result
        End Function

        Public Sub GLDraw()

            Dim Drawer As New clsMap.clsDrawHorizontalPosOnTerrain
            Drawer.Map = _ParentMap.Source
            Drawer.Horizontal = _Pos
            If frmMainInstance.SelectedScriptMarker Is Me Then
                GL.LineWidth(4.5F)
                Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.5F, 1.0F)
            Else
                GL.LineWidth(3.0F)
                Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.0F, 0.75F)
            End If
            Drawer.ActionPerform()
        End Sub

        Public Sub MapResizing(ByVal PosOffset As sXY_int)

            PosX = _Pos.X - PosOffset.X
            PosY = _Pos.Y - PosOffset.Y
        End Sub

        Public Sub WriteWZ(File As clsINIWrite)

            File.SectionName_Append("position_" & InvariantToString_int(_ParentMap.ArrayPosition))
            File.Property_Append("pos", InvariantToString_int(_Pos.X) & ", " & InvariantToString_int(_Pos.Y))
            File.Property_Append("label", _Label)
            File.Gap_Append()
        End Sub

        Public Function SetLabel(ByVal Text As String) As sResult
            Dim Result As sResult

            Result = _ParentMap.Source.ScriptLabelIsValid(Text)
            If Result.Success Then
                _Label = Text
            End If
            Return Result
        End Function

        Public Sub Deallocate()

            _ParentMap.Deallocate()
        End Sub
    End Class

    Public Class clsScriptArea

        Private _ParentMap As New ConnectedListLink(Of clsScriptArea, clsMap)(Me)
        Public ReadOnly Property ParentMap As ConnectedListLink(Of clsScriptArea, clsMap)
            Get
                Return _ParentMap
            End Get
        End Property

        Private _Label As String
        Public ReadOnly Property Label As String
            Get
                Return _Label
            End Get
        End Property

        Private _PosA As sXY_int
        Private _PosB As sXY_int
        Public WriteOnly Property PosA As sXY_int
            Set(ByVal value As sXY_int)
                Dim Map As clsMap = _ParentMap.Source
                _PosA.X = Clamp_int(value.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                _PosA.Y = Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public WriteOnly Property PosB As sXY_int
            Set(ByVal value As sXY_int)
                Dim Map As clsMap = _ParentMap.Source
                _PosB.X = Clamp_int(value.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
                _PosB.Y = Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosAX As Integer
            Get
                Return _PosA.X
            End Get
            Set(ByVal value As Integer)
                _PosA.X = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.X * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosAY As Integer
            Get
                Return _PosA.Y
            End Get
            Set(ByVal value As Integer)
                _PosA.Y = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosBX As Integer
            Get
                Return _PosB.X
            End Get
            Set(ByVal value As Integer)
                _PosB.X = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.X * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property
        Public Property PosBY As Integer
            Get
                Return _PosB.Y
            End Get
            Set(ByVal value As Integer)
                _PosB.Y = Clamp_int(value, 0, _ParentMap.Source.Terrain.TileSize.Y * TerrainGridSpacing - 1)
                XY_Reorder(_PosA, _PosB, _PosA, _PosB)
            End Set
        End Property

        Protected Sub New()

        End Sub

        Public Shared Function Create(ByVal Map As clsMap) As clsScriptArea
            Dim Result As New clsScriptArea

            Result._Label = Map.GetDefaultScriptLabel("Area")

            Result._ParentMap.Connect(Map.ScriptAreas)

            Return Result
        End Function

        Public Sub SetPositions(ByVal PosA As sXY_int, ByVal PosB As sXY_int)
            Dim Map As clsMap = _ParentMap.Source

            PosA.X = Clamp_int(PosA.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            PosA.Y = Clamp_int(PosA.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            PosB.X = Clamp_int(PosB.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            PosB.Y = Clamp_int(PosB.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)

            XY_Reorder(PosA, PosB, _PosA, _PosB)
        End Sub

        Public Sub GLDraw()

            Dim Drawer As New clsMap.clsDrawTerrainLine
            Drawer.Map = _ParentMap.Source
            If frmMainInstance.SelectedScriptMarker Is Me Then
                GL.LineWidth(4.5F)
                Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.5F, 0.75F)
            Else
                GL.LineWidth(3.0F)
                Drawer.Colour = New sRGBA_sng(1.0F, 1.0F, 0.0F, 0.5F)
            End If

            Drawer.StartXY = _PosA
            Drawer.FinishXY.X = _PosB.X
            Drawer.FinishXY.Y = _PosA.Y
            Drawer.ActionPerform()

            Drawer.StartXY = _PosA
            Drawer.FinishXY.X = _PosA.X
            Drawer.FinishXY.Y = _PosB.Y
            Drawer.ActionPerform()

            Drawer.StartXY.X = _PosB.X
            Drawer.StartXY.Y = _PosA.Y
            Drawer.FinishXY = _PosB
            Drawer.ActionPerform()

            Drawer.StartXY.X = _PosA.X
            Drawer.StartXY.Y = _PosB.Y
            Drawer.FinishXY = _PosB
            Drawer.ActionPerform()
        End Sub

        Public Sub MapResizing(ByVal PosOffset As sXY_int)

            SetPositions(New sXY_int(_PosA.X - PosOffset.X, _PosA.Y - PosOffset.Y), New sXY_int(_PosB.X - PosOffset.X, _PosB.Y - PosOffset.Y))
        End Sub

        Public Sub WriteWZ(File As clsINIWrite)

            File.SectionName_Append("area_" & InvariantToString_int(_ParentMap.ArrayPosition))
            File.Property_Append("pos1", InvariantToString_int(_PosA.X) & ", " & InvariantToString_int(_PosA.Y))
            File.Property_Append("pos2", InvariantToString_int(_PosB.X) & ", " & InvariantToString_int(_PosB.Y))
            File.Property_Append("label", _Label)
            File.Gap_Append()
        End Sub

        Public Function SetLabel(ByVal Text As String) As sResult
            Dim Result As sResult

            Result = _ParentMap.Source.ScriptLabelIsValid(Text)
            If Result.Success Then
                _Label = Text
            End If
            Return Result
        End Function

        Public Sub Deallocate()

            _ParentMap.Deallocate()
        End Sub
    End Class
End Class