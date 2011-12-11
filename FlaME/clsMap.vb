Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Partial Public Class clsMap

    Public frmMainLink As New ConnectedListLink(Of clsMap, frmMain)(Me)

    Public Class clsTerrain

#If Mono267 = 0.0# Then
        Public Structure Vertex
#Else
        Public Class Vertex
#End If
            Public Height As Byte
            Public Terrain As clsPainter.clsTerrain
#If Mono267 = 0.0# Then
        End Structure
#Else
        End Class
#End If

#If Mono267 = 0.0# Then
        Public Structure Tile
#Else
        Public Class Tile
#End If
            Public Structure sTexture
                Public TextureNum As Integer
                Public Orientation As sTileOrientation
            End Structure
            Public Texture As sTexture
            Public Tri As Boolean
            Public TriTopLeftIsCliff As Boolean
            Public TriTopRightIsCliff As Boolean
            Public TriBottomLeftIsCliff As Boolean
            Public TriBottomRightIsCliff As Boolean
            Public Terrain_IsCliff As Boolean
            Public DownSide As sTileDirection

            Public Sub Copy(ByVal TileToCopy As Tile)

                Texture = TileToCopy.Texture
                Tri = TileToCopy.Tri
                TriTopLeftIsCliff = TileToCopy.TriTopLeftIsCliff
                TriTopRightIsCliff = TileToCopy.TriTopRightIsCliff
                TriBottomLeftIsCliff = TileToCopy.TriBottomLeftIsCliff
                TriBottomRightIsCliff = TileToCopy.TriBottomRightIsCliff
                Terrain_IsCliff = TileToCopy.Terrain_IsCliff
                DownSide = TileToCopy.DownSide
            End Sub

            Public Sub TriCliffAddDirection(ByVal Direction As sTileDirection)

                If Direction.X = 0 Then
                    If Direction.Y = 0 Then
                        TriTopLeftIsCliff = True
                    ElseIf Direction.Y = 2 Then
                        TriBottomLeftIsCliff = True
                    Else
                        Stop
                    End If
                ElseIf Direction.X = 2 Then
                    If Direction.Y = 0 Then
                        TriTopRightIsCliff = True
                    ElseIf Direction.Y = 2 Then
                        TriBottomRightIsCliff = True
                    Else
                        Stop
                    End If
                Else
                    Stop
                End If
            End Sub
#If Mono267 = 0.0# Then
        End Structure
#Else
        End Class
#End If

#If Mono267 = 0.0# Then
        Public Structure Side
#Else
        Public Class Side
#End If
            Public Road As clsPainter.clsRoad
#If Mono267 = 0.0# Then
        End Structure
#Else
        End Class
#End If

        Public TileSize As sXY_int

        Public Vertices(,) As clsMap.clsTerrain.Vertex
        Public Tiles(,) As clsMap.clsTerrain.Tile
        Public SideH(,) As clsMap.clsTerrain.Side
        Public SideV(,) As clsMap.clsTerrain.Side

        Public Sub New(ByVal NewSize As sXY_int)

            TileSize = NewSize

            ReDim Vertices(TileSize.X, TileSize.Y)
            ReDim Tiles(TileSize.X - 1, TileSize.Y - 1)
            ReDim SideH(TileSize.X - 1, TileSize.Y)
            ReDim SideV(TileSize.X, TileSize.Y - 1)
            Dim X As Integer
            Dim Y As Integer
#If Mono267 <> 0.0# Then
                For Y = 0 To TileSize.Y - 1
                    For X = 0 To TileSize.X - 1
                        Tiles(X, Y) = New Tile
                    Next
                Next
                For Y = 0 To TileSize.Y
                    For X = 0 To TileSize.X
                        Vertices(X, Y) = New Vertex
                    Next
                Next
                For Y = 0 To TileSize.Y
                    For X = 0 To TileSize.X - 1
                        SideH(X, Y) = New Side
                    Next
                Next
                For Y = 0 To TileSize.Y - 1
                    For X = 0 To TileSize.X
                        SideV(X, Y) = New Side
                    Next
                Next
#End If
            For Y = 0 To TileSize.Y - 1
                For X = 0 To TileSize.X - 1
                    Tiles(X, Y).Texture.TextureNum = -1
                    Tiles(X, Y).DownSide = TileDirection_None
                Next
            Next
        End Sub
    End Class

    Public Terrain As clsTerrain

    Public Class clsSector
        Public Pos As sXY_int
        Public GLList_Textured As Integer
        Public GLList_Wireframe As Integer
        Public Units As New ConnectedList(Of clsMap.clsUnitSectorConnection, clsMap.clsSector)(Me)

        Public Sub DeleteLists()

            If GLList_Textured <> 0 Then
                GL.DeleteLists(GLList_Textured, 1)
                GLList_Textured = 0
            End If
            If GLList_Wireframe <> 0 Then
                GL.DeleteLists(GLList_Wireframe, 1)
                GLList_Wireframe = 0
            End If
        End Sub

        Public Sub Deallocate()

            Units.Deallocate()
        End Sub

        Public Sub New(ByVal NewPos As sXY_int)

            Pos = NewPos
        End Sub
    End Class
    Public Sectors(-1, -1) As clsSector
    Public SectorCount As sXY_int

    Public Class clsShadowSector
        Public Num As sXY_int
        Public Terrain As New clsTerrain(New sXY_int(SectorTileSize, SectorTileSize))
    End Class
    Public ShadowSectors(-1, -1) As clsShadowSector

    Public Class clsUnitChange
        Public Enum enumType As Byte
            Added
            Deleted
        End Enum
        Public Type As enumType
        Public Unit As clsUnit
    End Class

    Public Class clsGatewayChange
        Public Enum enumType As Byte
            Added
            Deleted
        End Enum
        Public Type As enumType
        Public Gateway As clsGateway
    End Class

    Public Class clsUndo
        Public Name As String
        Public ChangedSectors() As clsShadowSector
        Public ChangedSectorCount As Integer
        Public UnitChanges As SimpleClassList(Of clsMap.clsUnitChange)
        Public GatewayChanges As SimpleClassList(Of clsMap.clsGatewayChange)
    End Class
    Public Undos As SimpleClassList(Of clsMap.clsUndo)
    Public UndoPosition As Integer

    Public UnitChanges As SimpleClassList(Of clsMap.clsUnitChange)
    Public GatewayChanges As SimpleClassList(Of clsMap.clsGatewayChange)

    Public HeightMultiplier As Integer = DefaultHeightMultiplier

    Public ReadOnly Property ReadyForUserInput As Boolean
        Get
            Return _ReadyForUserInput
        End Get
    End Property
    Private _ReadyForUserInput As Boolean = False

    Public SelectedUnits As ConnectedList(Of clsMap.clsUnit, clsMap)
    Public Selected_Tile_A As clsXY_int
    Public Selected_Tile_B As clsXY_int
    Public Selected_Area_VertexA As clsXY_int
    Public Selected_Area_VertexB As clsXY_int
    Public Unit_Selected_Area_VertexA As clsXY_int

    Public Class clsUnit
        Public MapLink As New ConnectedListLink(Of clsUnit, clsMap)(Me)
        Public MapSelectedUnitLink As New ConnectedListLink(Of clsUnit, clsMap)(Me)
        Public Sectors As New ConnectedList(Of clsMap.clsUnitSectorConnection, clsMap.clsUnit)(Me)

        Public ID As UInteger
        Public Type As clsUnitType
        Public Pos As sWorldPos
        Public Rotation As Integer
        Public UnitGroup As clsMap.clsUnitGroup
        Public SavePriority As Integer
        Public Health As Double = 1.0#
        Public PreferPartsOutput As Boolean = False

        Private _Label As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal UnitToCopy As clsUnit, ByVal TargetMap As clsMap)
            Dim IsDesign As Boolean

            If UnitToCopy.Type.Type = clsUnitType.enumType.PlayerDroid Then
                IsDesign = Not CType(UnitToCopy.Type, clsDroidDesign).IsTemplate
            Else
                IsDesign = False
            End If
            If IsDesign Then
                Dim tmpDroidDesign As New clsDroidDesign
                Type = tmpDroidDesign
                tmpDroidDesign.CopyDesign(CType(UnitToCopy.Type, clsDroidDesign))
                tmpDroidDesign.UpdateAttachments()
            Else
                Type = UnitToCopy.Type
            End If
            Pos = UnitToCopy.Pos
            Rotation = UnitToCopy.Rotation
            Dim tmpUnitGroup As clsMap.clsUnitGroup
            tmpUnitGroup = UnitToCopy.UnitGroup
            If tmpUnitGroup.WZ_StartPos < 0 Then
                UnitGroup = TargetMap.ScavengerUnitGroup
            Else
                UnitGroup = TargetMap.UnitGroups.Item(tmpUnitGroup.WZ_StartPos)
            End If
            SavePriority = UnitToCopy.SavePriority
            Health = UnitToCopy.Health
            PreferPartsOutput = UnitToCopy.PreferPartsOutput
        End Sub

        Public ReadOnly Property Label As String
            Get
                Return _Label
            End Get
        End Property

        Public Function GetINIPosition() As String

            Return InvariantToString_int(Pos.Horizontal.X) & ", " & InvariantToString_int(Pos.Horizontal.Y) & ", 0"
        End Function

        Public Function GetINIRotation() As String
            Dim Rotation16 As Integer

            Rotation16 = CInt(Rotation * INIRotationMax / 360.0#)
            If Rotation16 >= INIRotationMax Then
                Rotation16 -= INIRotationMax
            End If

            Return InvariantToString_int(Rotation16) & ", 0, 0"
        End Function

        Public Function GetINIHealthPercent() As String

            Return InvariantToString_int(CInt(Clamp_dbl(Health * 100.0#, 1.0#, 100.0#))) & "%"
        End Function

        Public Function GetPosText() As String

            Return InvariantToString_int(Pos.Horizontal.X) & ", " & InvariantToString_int(Pos.Horizontal.Y)
        End Function

        Public Function SetLabel(ByVal Text As String) As sResult
            Dim Result As sResult

            If Type.Type = clsUnitType.enumType.PlayerStructure Then
                Dim tmpStructure As clsStructureType = CType(Type, clsStructureType)
                Dim tmpType As clsStructureType.enumStructureType = tmpStructure.StructureType
                If tmpType = clsStructureType.enumStructureType.FactoryModule _
                    Or tmpType = clsStructureType.enumStructureType.PowerModule _
                    Or tmpType = clsStructureType.enumStructureType.ResearchModule Then
                    Result.Problem = "Error: Trying to assign label to structure module."
                    Return Result
                End If
            End If

            If Not MapLink.IsConnected Then
                Stop
                Result.Problem = "Error: Unit not on a map."
                Return Result
            End If

            If Text Is Nothing Then
                _Label = Nothing
                Result.Success = True
                Result.Problem = ""
                Return Result
            Else
                Result = MapLink.Source.ScriptLabelIsValid(Text)
                If Result.Success Then
                    _Label = Text
                End If
                Return Result
            End If
        End Function

        Public Sub WriteWZLabel(ByVal File As clsINIWrite, ByVal PlayerCount As Integer)

            If _Label IsNot Nothing Then
                Dim TypeNum As Integer
                Select Case Type.Type
                    Case clsUnitType.enumType.PlayerDroid
                        TypeNum = 0
                    Case clsUnitType.enumType.PlayerStructure
                        TypeNum = 1
                    Case clsUnitType.enumType.Feature
                        TypeNum = 2
                    Case Else
                        Exit Sub
                End Select
                File.SectionName_Append("object_" & InvariantToString_int(MapLink.ArrayPosition))
                File.Property_Append("id", InvariantToString_uint(ID))
                If PlayerCount >= 0 Then 'not an FMap
                    File.Property_Append("type", InvariantToString_int(TypeNum))
                    File.Property_Append("player", InvariantToString_int(UnitGroup.GetPlayerNum(PlayerCount)))
                End If
                File.Property_Append("label", _Label)
                File.Gap_Append()
            End If
        End Sub

        Public Function GetBJOMultiplayerPlayerNum(ByVal PlayerCount As Integer) As UInteger
            Dim PlayerNum As Integer

            If UnitGroup Is MapLink.Source.ScavengerUnitGroup Or UnitGroup.WZ_StartPos < 0 Then
                PlayerNum = Math.Max(PlayerCount, 7)
            Else
                PlayerNum = UnitGroup.WZ_StartPos
            End If
            Return CUInt(PlayerNum)
        End Function

        Public Function GetBJOCampaignPlayerNum() As UInteger
            Dim PlayerNum As Integer

            If UnitGroup Is MapLink.Source.ScavengerUnitGroup Or UnitGroup.WZ_StartPos < 0 Then
                PlayerNum = 7
            Else
                PlayerNum = UnitGroup.WZ_StartPos
            End If
            Return CUInt(PlayerNum)
        End Function

        Public Sub MapSelect()

            If MapSelectedUnitLink.IsConnected Then
                Stop
                Exit Sub
            End If

            MapSelectedUnitLink.Connect(MapLink.Source.SelectedUnits)
        End Sub

        Public Sub MapDeselect()

            If Not MapSelectedUnitLink.IsConnected Then
                Stop
                Exit Sub
            End If

            MapSelectedUnitLink.Disconnect()
        End Sub

        Public Sub DisconnectFromMap()

            If MapLink.IsConnected Then
                MapLink.Disconnect()
            End If
            If MapSelectedUnitLink.IsConnected Then
                MapSelectedUnitLink.Disconnect()
            End If
            Sectors.Clear()
        End Sub

        Public Sub Deallocate()

            MapLink.Deallocate()
            MapSelectedUnitLink.Deallocate()
            Sectors.Deallocate()
        End Sub
    End Class
    Public Units As New ConnectedList(Of clsMap.clsUnit, clsMap)(Me)

    Public Class clsUnitSectorConnection

        Protected Class Link(Of SourceType As Class)
            Inherits ConnectedListLink(Of clsUnitSectorConnection, SourceType)

            Public Sub New(ByVal Owner As clsUnitSectorConnection)
                MyBase.New(Owner)

            End Sub

            Public Overrides Sub AfterRemove()
                MyBase.AfterRemove()

                Item.Deallocate()
            End Sub
        End Class

        Protected _UnitLink As New Link(Of clsMap.clsUnit)(Me)
        Protected _SectorLink As New Link(Of clsMap.clsSector)(Me)

        Public Overridable ReadOnly Property Unit As clsMap.clsUnit
            Get
                Return _UnitLink.Source
            End Get
        End Property

        Public Overridable ReadOnly Property Sector As clsMap.clsSector
            Get
                Return _SectorLink.Source
            End Get
        End Property

        Public Shared Function Create(ByVal Unit As clsMap.clsUnit, ByVal Sector As clsMap.clsSector) As clsUnitSectorConnection

            If Unit Is Nothing Then
                Return Nothing
            End If
            If Unit.Sectors Is Nothing Then
                Return Nothing
            End If
            If Unit.Sectors.IsBusy Then
                Return Nothing
            End If
            If Sector Is Nothing Then
                Return Nothing
            End If
            If Sector.Units Is Nothing Then
                Return Nothing
            End If
            If Sector.Units.IsBusy Then
                Return Nothing
            End If

            Dim Result As New clsUnitSectorConnection
            Result._UnitLink.Connect(Unit.Sectors)
            Result._SectorLink.Connect(Sector.Units)
            Return Result
        End Function

        Protected Sub New()


        End Sub

        Public Sub Deallocate()

            _UnitLink.Deallocate()
            _SectorLink.Deallocate()
        End Sub
    End Class

    Public Class clsUnitGroupContainer

        Private _Item As clsUnitGroup

        Public Property Item As clsUnitGroup
            Get
                Return _Item
            End Get
            Set(value As clsUnitGroup)
                If value Is _Item Then
                    Exit Property
                End If
                _Item = value
                RaiseEvent Changed()
            End Set
        End Property

        Public Event Changed()
    End Class

    Private _SelectedUnitGroup As clsUnitGroupContainer
    Public ReadOnly Property SelectedUnitGroup As clsUnitGroupContainer
        Get
            Return _SelectedUnitGroup
        End Get
    End Property

    Public Class clsUnitGroup

        Public MapLink As New ConnectedListLink(Of clsUnitGroup, clsMap)(Me)

        Public WZ_StartPos As Integer = -1

        Public Function GetFMapINIPlayerText() As String

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return "scavenger"
            Else
                Return InvariantToString_int(WZ_StartPos)
            End If
        End Function

        Public Function GetLNDPlayerText() As String

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return InvariantToString_int(7)
            Else
                Return InvariantToString_int(WZ_StartPos)
            End If
        End Function

        Public Function GetPlayerNum(ByVal PlayerCount As Integer) As Integer

            If WZ_StartPos < 0 Or WZ_StartPos >= PlayerCountMax Then
                Return Math.Max(PlayerCount, 7)
            Else
                Return WZ_StartPos
            End If
        End Function
    End Class
    Public UnitGroups As New ConnectedList(Of clsUnitGroup, clsMap)(Me)
    Public ScavengerUnitGroup As clsUnitGroup

    Public Minimap_GLTexture As Integer
    Public Minimap_Texture_Size As Integer

    Public Class clsMessage
        Public Text As String
        Private _CreatedDate As Date = Now

        Public ReadOnly Property CreatedDate As Date
            Get
                Return _CreatedDate
            End Get
        End Property
    End Class
    Public Messages As SimpleClassList(Of clsMessage)

    Public Event TilesetChanged(ByVal sender As clsMap)

    Private Sub RaiseTilesetChanged()

        RaiseEvent TilesetChanged(Me)
    End Sub

    Private Structure sTileset
        Private Item As clsTileset

        Public Function GetItem() As clsTileset

            Return Item
        End Function

        Public Sub SetItem(ByVal Map As clsMap, ByVal Item As clsTileset)

            If Item Is Me.Item Then
                Exit Sub
            End If
            Me.Item = Item
            Map.RaiseTilesetChanged()
        End Sub
    End Structure
    Private _Tileset As sTileset

    Public Property Tileset As clsTileset
        Get
            Return _Tileset.GetItem
        End Get
        Set(value As clsTileset)
            _Tileset.SetItem(Me, value)
        End Set
    End Property

    Public Class clsPathInfo
        Private _Path As String
        Private _IsFMap As Boolean

        Public ReadOnly Property Path As String
            Get
                Return _Path
            End Get
        End Property

        Public ReadOnly Property IsFMap As Boolean
            Get
                Return _IsFMap
            End Get
        End Property

        Public Sub New(ByVal Path As String, ByVal IsFMap As Boolean)

            _Path = Path
            _IsFMap = IsFMap
        End Sub
    End Class
    Public PathInfo As clsPathInfo

    Public ChangedSinceSave As Boolean = False
    Public Event Changed()

    Public Class clsAutoSave
        Public ChangeCount As Integer
        Public SavedDate As Date

        Public Sub New()

            SavedDate = Now
        End Sub
    End Class
    Public AutoSave As New clsAutoSave

    Public Painter As New clsPainter

    Public Tile_TypeNum(-1) As Byte

    Public Class clsGateway
        Public MapLink As New ConnectedListLink(Of clsGateway, clsMap)(Me)
        Public PosA As sXY_int
        Public PosB As sXY_int

        Public Function IsOffMap() As Boolean
            Dim TerrainSize As sXY_int = MapLink.Source.Terrain.TileSize

            Return PosA.X < 0 _
                Or PosA.X >= TerrainSize.X _
                Or PosA.Y < 0 _
                Or PosA.Y >= TerrainSize.Y _
                Or PosB.X < 0 _
                Or PosB.X >= TerrainSize.X _
                Or PosB.Y < 0 _
                Or PosB.Y >= TerrainSize.Y
        End Function

        Public Sub Deallocate()

            MapLink.Deallocate()
        End Sub
    End Class
    Public Gateways As New ConnectedList(Of clsGateway, clsMap)(Me)

    Public Class clsPointChanges

        Public PointIsChanged(,) As Boolean
        Public ChangedPoints() As sXY_int
        Public ChangedPointCount As Integer

        Public Sub New(ByVal PointSize As sXY_int)

            ReDim PointIsChanged(PointSize.X - 1, PointSize.Y - 1)
            ReDim ChangedPoints(PointSize.X * PointSize.Y - 1)
        End Sub

        Public Sub Changed(ByVal Num As sXY_int)

            If Not PointIsChanged(Num.X, Num.Y) Then
                PointIsChanged(Num.X, Num.Y) = True
                ChangedPoints(ChangedPointCount) = Num
                ChangedPointCount += 1
            End If
        End Sub

        Public Sub SetAllChanged()
            Dim X As Integer
            Dim Y As Integer
            Dim Num As sXY_int

            For Y = 0 To PointIsChanged.GetUpperBound(1)
                Num.Y = Y
                For X = 0 To PointIsChanged.GetUpperBound(0)
                    Num.X = X
                    Changed(Num)
                Next
            Next
        End Sub

        Public Sub Clear()
            Dim A As Integer

            For A = 0 To ChangedPointCount - 1
                PointIsChanged(ChangedPoints(A).X, ChangedPoints(A).Y) = False
            Next
            ChangedPointCount = 0
        End Sub

        Public Sub PerformTool(ByVal Tool As clsMap.clsAction)
            Dim A As Integer

            For A = 0 To ChangedPointCount - 1
                Tool.PosNum = ChangedPoints(A)
                Tool.ActionPerform()
            Next
        End Sub
    End Class

    Public MustInherit Class clsMapTileChanges
        Inherits clsPointChanges

        Public Map As clsMap
        Public Terrain As clsTerrain

        Public Sub New(ByVal Map As clsMap, ByVal PointSize As sXY_int)
            MyBase.New(PointSize)

            Me.Map = Map
            Me.Terrain = Map.Terrain
        End Sub

        Public Sub Deallocate()

            Map = Nothing
        End Sub

        Public MustOverride Sub TileChanged(ByVal Num As sXY_int)

        Public Sub VertexChanged(ByVal Num As sXY_int)

            If Num.X > 0 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
            End If
        End Sub

        Public Sub VertexAndNormalsChanged(ByVal Num As sXY_int)

            If Num.X > 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 2, Num.Y))
                End If
            End If
            If Num.X > 0 Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y))
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X - 1, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X Then
                If Num.Y > 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 2))
                End If
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(Num)
                End If
                If Num.Y < Terrain.TileSize.Y - 1 Then
                    TileChanged(New sXY_int(Num.X, Num.Y + 1))
                End If
            End If
            If Num.X < Terrain.TileSize.X - 1 Then
                If Num.Y > 0 Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y - 1))
                End If
                If Num.Y < Terrain.TileSize.Y Then
                    TileChanged(New sXY_int(Num.X + 1, Num.Y))
                End If
            End If
        End Sub

        Public Sub SideHChanged(ByVal Num As sXY_int)

            If Num.Y > 0 Then
                TileChanged(New sXY_int(Num.X, Num.Y - 1))
            End If
            If Num.Y < Map.Terrain.TileSize.Y Then
                TileChanged(Num)
            End If
        End Sub

        Public Sub SideVChanged(ByVal Num As sXY_int)

            If Num.X > 0 Then
                TileChanged(New sXY_int(Num.X - 1, Num.Y))
            End If
            If Num.X < Map.Terrain.TileSize.X Then
                TileChanged(Num)
            End If
        End Sub
    End Class

    Public Class clsSectorChanges
        Inherits clsMap.clsMapTileChanges

        Public Sub New(ByVal Map As clsMap)
            MyBase.New(Map, Map.SectorCount)

        End Sub

        Public Overrides Sub TileChanged(ByVal Num As sXY_int)
            Dim SectorNum As sXY_int

            SectorNum = Map.GetTileSectorNum(Num)
            Changed(SectorNum)
        End Sub
    End Class

    Public Class clsAutoTextureChanges
        Inherits clsMap.clsMapTileChanges

        Public Sub New(ByVal Map As clsMap)
            MyBase.New(Map, Map.Terrain.TileSize)

        End Sub

        Public Overrides Sub TileChanged(ByVal Num As modMath.sXY_int)

            Changed(Num)
        End Sub
    End Class

    Public SectorGraphicsChanges As clsSectorChanges
    Public SectorUnitHeightsChanges As clsSectorChanges
    Public SectorTerrainUndoChanges As clsSectorChanges
    Public AutoTextureChanges As clsAutoTextureChanges

    Public Class clsTerrainUpdate

        Public Vertices As clsPointChanges
        Public Tiles As clsPointChanges
        Public SidesH As clsPointChanges
        Public SidesV As clsPointChanges

        Public Sub Deallocate()


        End Sub

        Public Sub New(ByVal TileSize As sXY_int)

            Vertices = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y + 1))
            Tiles = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y))
            SidesH = New clsPointChanges(New sXY_int(TileSize.X, TileSize.Y + 1))
            SidesV = New clsPointChanges(New sXY_int(TileSize.X + 1, TileSize.Y))
        End Sub

        Public Sub SetAllChanged()

            Vertices.SetAllChanged()
            Tiles.SetAllChanged()
            SidesH.SetAllChanged()
            SidesV.SetAllChanged()
        End Sub

        Public Sub ClearAll()

            Vertices.Clear()
            Tiles.Clear()
            SidesH.Clear()
            SidesV.Clear()
        End Sub
    End Class

    Public TerrainInterpretChanges As clsTerrainUpdate

    Public Sub New()

        Initialize()
    End Sub

    Public Sub New(ByVal TileSize As sXY_int)

        Initialize()

        Terrain_Blank(TileSize)
        TileType_Reset()
    End Sub

    Public Sub Initialize()

        MakeMinimapTimer = New Timer
        MakeMinimapTimer.Interval = MinimapDelay

        MakeDefaultUnitGroups()
        ScriptPositions.MaintainOrder = True
        ScriptAreas.MaintainOrder = True
    End Sub

    Public Sub New(ByVal Map_To_Copy As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int)
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer

        Initialize()

        'make some map data for selection

        EndX = Math.Min(Map_To_Copy.Terrain.TileSize.X - Offset.X, Area.X)
        EndY = Math.Min(Map_To_Copy.Terrain.TileSize.Y - Offset.Y, Area.Y)

        Terrain = New clsTerrain(Area)

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Terrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = 0 To EndY
            For X = 0 To EndX
                Terrain.Vertices(X, Y).Height = Map_To_Copy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                Terrain.Vertices(X, Y).Terrain = Map_To_Copy.Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = 0 To EndY - 1
            For X = 0 To EndX - 1
                Terrain.Tiles(X, Y).Copy(Map_To_Copy.Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = 0 To EndY
            For X = 0 To EndX - 1
                Terrain.SideH(X, Y).Road = Map_To_Copy.Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = 0 To EndY - 1
            For X = 0 To EndX
                Terrain.SideV(X, Y).Road = Map_To_Copy.Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        SectorCount.X = CInt(Math.Ceiling(Area.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Area.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer
        Dim NewUnitAdd As New clsMap.clsUnitAdd
        NewUnitAdd.Map = Me
        Dim NewUnit As clsMap.clsUnit

        For A = 0 To Map_To_Copy.Gateways.ItemCount - 1
            Gateway_Create(New sXY_int(Map_To_Copy.Gateways.Item(A).PosA.X - Offset.X, Map_To_Copy.Gateways.Item(A).PosA.Y - Offset.Y), New sXY_int(Map_To_Copy.Gateways.Item(A).PosB.X - Offset.X, Map_To_Copy.Gateways.Item(A).PosB.Y - Offset.Y))
        Next

        PosDifX = -Offset.X * TerrainGridSpacing
        PosDifZ = -Offset.Y * TerrainGridSpacing
        For A = 0 To Map_To_Copy.Units.ItemCount - 1
            NewUnit = New clsUnit(Map_To_Copy.Units.Item(A), Me)
            NewUnit.Pos.Horizontal.X += PosDifX
            NewUnit.Pos.Horizontal.Y += PosDifZ
            If Not (NewUnit.Pos.Horizontal.X < 0 _
              Or NewUnit.Pos.Horizontal.X >= Terrain.TileSize.X * TerrainGridSpacing _
              Or NewUnit.Pos.Horizontal.Y < 0 _
              Or NewUnit.Pos.Horizontal.Y >= Terrain.TileSize.Y * TerrainGridSpacing) Then
                NewUnitAdd.NewUnit = NewUnit
                NewUnitAdd.Label = Map_To_Copy.Units.Item(A).Label
                NewUnitAdd.Perform()
            End If
        Next
    End Sub

    Protected Sub Terrain_Blank(ByVal TileSize As sXY_int)
        Dim X As Integer
        Dim Y As Integer

        Terrain = New clsTerrain(TileSize)
        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next
    End Sub

    Public Function GetTerrainTri(ByVal Horizontal As sXY_int) As Boolean
        Dim X1 As Integer
        Dim Y1 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Return False
            Else
                Return True
            End If
        Else
            If InTileZ <= InTileX Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Function GetTerrainSlopeAngle(ByVal Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim XYZ_dbl3 As Matrix3D.XYZ_dbl
        Dim AnglePY As Matrix3D.AnglePY

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
            End If
        End If

        XYZ_dbl.X = TerrainGridSpacing
        XYZ_dbl.Y = GradientX * HeightMultiplier
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = GradientY * HeightMultiplier
        XYZ_dbl2.Z = TerrainGridSpacing
        Matrix3D.VectorCrossProduct(XYZ_dbl, XYZ_dbl2, XYZ_dbl3)
        If XYZ_dbl3.X <> 0.0# Or XYZ_dbl3.Z <> 0.0# Then
            Matrix3D.VectorToPY(XYZ_dbl3, AnglePY)
            Return RadOf90Deg - Math.Abs(AnglePY.Pitch)
        Else
            Return 0.0#
        End If
    End Function

    Public Function GetTerrainHeight(ByVal Horizontal As sXY_int) As Double
        Dim X1 As Integer
        Dim X2 As Integer
        Dim Y1 As Integer
        Dim Y2 As Integer
        Dim InTileX As Double
        Dim InTileZ As Double
        Dim XG As Integer
        Dim YG As Integer
        Dim GradientX As Double
        Dim GradientY As Double
        Dim Offset As Double
        Dim RatioX As Double
        Dim RatioY As Double

        XG = CInt(Int(Horizontal.X / TerrainGridSpacing))
        YG = CInt(Int(Horizontal.Y / TerrainGridSpacing))
        InTileX = Clamp_dbl(Horizontal.X / TerrainGridSpacing - XG, 0.0#, 1.0#)
        InTileZ = Clamp_dbl(Horizontal.Y / TerrainGridSpacing - YG, 0.0#, 1.0#)
        X1 = Clamp_int(XG, 0, Terrain.TileSize.X - 1)
        Y1 = Clamp_int(YG, 0, Terrain.TileSize.Y - 1)
        X2 = Clamp_int(XG + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(YG + 1, 0, Terrain.TileSize.Y)
        If Terrain.Tiles(X1, Y1).Tri Then
            If InTileZ <= 1.0# - InTileX Then
                Offset = Terrain.Vertices(X1, Y1).Height
                GradientX = Terrain.Vertices(X2, Y1).Height - Offset
                GradientY = Terrain.Vertices(X1, Y2).Height - Offset
                RatioX = InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X2, Y2).Height
                GradientX = Terrain.Vertices(X1, Y2).Height - Offset
                GradientY = Terrain.Vertices(X2, Y1).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = 1.0# - InTileZ
            End If
        Else
            If InTileZ <= InTileX Then
                Offset = Terrain.Vertices(X2, Y1).Height
                GradientX = Terrain.Vertices(X1, Y1).Height - Offset
                GradientY = Terrain.Vertices(X2, Y2).Height - Offset
                RatioX = 1.0# - InTileX
                RatioY = InTileZ
            Else
                Offset = Terrain.Vertices(X1, Y2).Height
                GradientX = Terrain.Vertices(X2, Y2).Height - Offset
                GradientY = Terrain.Vertices(X1, Y1).Height - Offset
                RatioX = InTileX
                RatioY = 1.0# - InTileZ
            End If
        End If
        Return (Offset + GradientX * RatioX + GradientY * RatioY) * HeightMultiplier
    End Function

    Public Function TerrainVertexNormalCalc(ByVal X As Integer, ByVal Y As Integer) As sXYZ_sng
        Dim ReturnResult As sXYZ_sng
        Dim TerrainHeightX1 As Integer
        Dim TerrainHeightX2 As Integer
        Dim TerrainHeightY1 As Integer
        Dim TerrainHeightY2 As Integer
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim XYZ_dbl As Matrix3D.XYZ_dbl
        Dim XYZ_dbl2 As Matrix3D.XYZ_dbl
        Dim dblTemp As Double

        X2 = Clamp_int(X - 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X + 1, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y, 0, Terrain.TileSize.Y)
        TerrainHeightX2 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y - 1, 0, Terrain.TileSize.Y)
        TerrainHeightY1 = Terrain.Vertices(X2, Y2).Height
        X2 = Clamp_int(X, 0, Terrain.TileSize.X)
        Y2 = Clamp_int(Y + 1, 0, Terrain.TileSize.Y)
        TerrainHeightY2 = Terrain.Vertices(X2, Y2).Height
        XYZ_dbl.X = (TerrainHeightX1 - TerrainHeightX2) * HeightMultiplier
        XYZ_dbl.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl.Z = 0.0#
        XYZ_dbl2.X = 0.0#
        XYZ_dbl2.Y = TerrainGridSpacing * 2.0#
        XYZ_dbl2.Z = (TerrainHeightY1 - TerrainHeightY2) * HeightMultiplier
        XYZ_dbl.X = XYZ_dbl.X + XYZ_dbl2.X
        XYZ_dbl.Y = XYZ_dbl.Y + XYZ_dbl2.Y
        XYZ_dbl.Z = XYZ_dbl.Z + XYZ_dbl2.Z
        dblTemp = Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z)
        ReturnResult.X = CSng(XYZ_dbl.X / dblTemp)
        ReturnResult.Y = CSng(XYZ_dbl.Y / dblTemp)
        ReturnResult.Z = CSng(XYZ_dbl.Z / dblTemp)
        Return ReturnResult
    End Function

    Public Sub SectorAll_GLLists_Delete()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).DeleteLists()
            Next
        Next
    End Sub

    Public Overridable Sub Deallocate()

        CancelUserInput()

        MakeMinimapTimer.Enabled = False
        MakeMinimapTimer.Dispose()
        MakeMinimapTimer = Nothing

        frmMainLink.Deallocate()
        frmMainLink = Nothing

        UnitGroups.Deallocate()
        UnitGroups = Nothing

        Do While Units.ItemCount > 0
            Units.Item(0).Deallocate()
        Loop
        Units.Deallocate()
        Units = Nothing

        Do While Gateways.ItemCount > 0
            Gateways.Item(0).Deallocate()
        Loop
        Gateways.Deallocate()
        Gateways = Nothing

        Do While ScriptPositions.ItemCount > 0
            ScriptPositions.Item(0).Deallocate()
        Loop
        ScriptPositions.Deallocate()
        ScriptPositions = Nothing

        Do While ScriptAreas.ItemCount > 0
            ScriptAreas.Item(0).Deallocate()
        Loop
        ScriptAreas.Deallocate()
        ScriptAreas = Nothing
    End Sub

    Public Sub Terrain_Resize(ByVal Offset As sXY_int, ByVal Size As sXY_int)
        Dim StartX As Integer
        Dim StartY As Integer
        Dim EndX As Integer
        Dim EndY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim NewTerrain As New clsTerrain(Size)

        StartX = Math.Max(0 - Offset.X, 0)
        StartY = Math.Max(0 - Offset.Y, 0)
        EndX = Math.Min(Terrain.TileSize.X - Offset.X, Size.X)
        EndY = Math.Min(Terrain.TileSize.Y - Offset.Y, Size.Y)

        For Y = 0 To NewTerrain.TileSize.Y - 1
            For X = 0 To NewTerrain.TileSize.X - 1
                NewTerrain.Tiles(X, Y).Texture.TextureNum = -1
            Next
        Next

        For Y = StartY To EndY
            For X = StartX To EndX
                NewTerrain.Vertices(X, Y).Height = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height
                NewTerrain.Vertices(X, Y).Terrain = Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX - 1
                NewTerrain.Tiles(X, Y).Copy(Terrain.Tiles(Offset.X + X, Offset.Y + Y))
            Next
        Next
        For Y = StartY To EndY
            For X = StartX To EndX - 1
                NewTerrain.SideH(X, Y).Road = Terrain.SideH(Offset.X + X, Offset.Y + Y).Road
            Next
        Next
        For Y = StartY To EndY - 1
            For X = StartX To EndX
                NewTerrain.SideV(X, Y).Road = Terrain.SideV(Offset.X + X, Offset.Y + Y).Road
            Next
        Next

        Dim PosDifX As Integer
        Dim PosDifZ As Integer
        Dim A As Integer
        Dim tmpUnit As clsUnit
        Dim tmpGateway As clsGateway

        PosDifX = -Offset.X * TerrainGridSpacing
        PosDifZ = -Offset.Y * TerrainGridSpacing
        For A = 0 To Units.ItemCount - 1
            tmpUnit = Units.Item(A)
            tmpUnit.Pos.Horizontal.X += PosDifX
            tmpUnit.Pos.Horizontal.Y += PosDifZ
        Next
        For A = 0 To Gateways.ItemCount - 1
            tmpGateway = Gateways.Item(A)
            tmpGateway.PosA.X -= Offset.X
            tmpGateway.PosA.Y -= Offset.Y
            tmpGateway.PosB.X -= Offset.X
            tmpGateway.PosB.Y -= Offset.Y
        Next

        Dim ZeroPos As New sXY_int(0, 0)

        Dim tmpUnits As SimpleClassList(Of clsMap.clsUnit) = Units.GetItemsAsSimpleClassList
        Dim Position As Integer

        For A = 0 To tmpUnits.ItemCount - 1
            tmpUnit = tmpUnits.Item(A)
            Position = tmpUnit.MapLink.ArrayPosition
            If Not PosIsWithinTileArea(Units.Item(Position).Pos.Horizontal, ZeroPos, NewTerrain.TileSize) Then
                Unit_Remove(Position)
            End If
        Next

        Terrain = NewTerrain

        Dim tmpGateways As SimpleClassList(Of clsMap.clsGateway) = Gateways.GetItemsAsSimpleClassList
        For A = 0 To tmpGateways.ItemCount - 1
            tmpGateway = tmpGateways.Item(A)
            If tmpGateway.IsOffMap Then
                tmpGateway.Deallocate()
            End If
        Next

        Dim PosOffset As New sXY_int(Offset.X * TerrainGridSpacing, Offset.Y * TerrainGridSpacing)

        Dim tmpScriptPositions As SimpleClassList(Of clsMap.clsScriptPosition) = ScriptPositions.GetItemsAsSimpleClassList
        For A = 0 To tmpScriptPositions.ItemCount - 1
            tmpScriptPositions.Item(A).MapResizing(PosOffset)
        Next

        Dim tmpScriptareas As SimpleClassList(Of clsMap.clsScriptArea) = ScriptAreas.GetItemsAsSimpleClassList
        For A = 0 To tmpScriptareas.ItemCount - 1
            tmpScriptareas.Item(A).MapResizing(PosOffset)
        Next

        If _ReadyForUserInput Then
            CancelUserInput()
            InitializeUserInput()
        End If
    End Sub

    Public Sub Sector_GLList_Make(ByVal X As Integer, ByVal Y As Integer)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim FinishX As Integer
        Dim FinishY As Integer
        Dim UnitNum As Integer

        Sectors(X, Y).DeleteLists()

        StartX = X * SectorTileSize
        StartY = Y * SectorTileSize
        FinishX = Math.Min(StartX + SectorTileSize, Terrain.TileSize.X) - 1
        FinishY = Math.Min(StartY + SectorTileSize, Terrain.TileSize.Y) - 1

        Sectors(X, Y).GLList_Textured = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Textured, ListMode.Compile)

        If Draw_Units Then
            Dim IsBasePlate(SectorTileSize - 1, SectorTileSize - 1) As Boolean
            Dim tmpUnit As clsUnit
            Dim BaseOffset As sXY_int
            Dim tmpStructure As clsStructureType
            Dim Footprint As sXY_int
            For UnitNum = 0 To Sectors(X, Y).Units.ItemCount - 1
                tmpUnit = Sectors(X, Y).Units.Item(UnitNum).Unit
                If tmpUnit.Type.Type = clsUnitType.enumType.PlayerStructure Then
                    tmpStructure = CType(tmpUnit.Type, clsStructureType)
                    Footprint = tmpStructure.Footprint
                    If tmpStructure.StructureBasePlate IsNot Nothing And (tmpUnit.Rotation = 0 Or tmpUnit.Rotation = 180 Or (Footprint.X = Footprint.Y And (tmpUnit.Rotation = 90 Or tmpUnit.Rotation = 180 Or tmpUnit.Rotation = 270))) Then
                        BaseOffset.X = CInt((Footprint.X - 1) * TerrainGridSpacing / 2.0#) '1 is subtracted because centre of the edge-tiles are needed, not the edge of the base plate
                        BaseOffset.Y = CInt((Footprint.Y - 1) * TerrainGridSpacing / 2.0#)
                        For TileY = Math.Max(CInt(Int((tmpUnit.Pos.Horizontal.Y - BaseOffset.Y) / TerrainGridSpacing)), StartY) To Math.Min(CInt(Int((tmpUnit.Pos.Horizontal.Y + BaseOffset.Y) / TerrainGridSpacing)), FinishY)
                            For TileX = Math.Max(CInt(Int((tmpUnit.Pos.Horizontal.X - BaseOffset.X) / TerrainGridSpacing)), StartX) To Math.Min(CInt(Int((tmpUnit.Pos.Horizontal.X + BaseOffset.X) / TerrainGridSpacing)), FinishX)
                                IsBasePlate(TileX - StartX, TileY - StartY) = True
                            Next
                        Next
                    End If
                End If
            Next
            For TileY = StartY To FinishY
                For TileX = StartX To FinishX
                    If Not IsBasePlate(TileX - StartX, TileY - StartY) Then
                        DrawTile(TileX, TileY)
                    End If
                Next
            Next
        Else
            For TileY = StartY To FinishY
                For TileX = StartX To FinishX
                    DrawTile(TileX, TileY)
                Next
            Next
        End If

        GL.EndList()

        Sectors(X, Y).GLList_Wireframe = GL.GenLists(1)
        GL.NewList(Sectors(X, Y).GLList_Wireframe, ListMode.Compile)

        For TileY = StartY To FinishY
            For TileX = StartX To FinishX
                DrawTileWireframe(TileX, TileY)
            Next
        Next

        GL.EndList()
    End Sub

    Public Sub DrawTileWireframe(ByVal TileX As Integer, ByVal TileY As Integer)
        Dim TileTerrainHeight(3) As Double
        Dim Vertex0 As sXYZ_sng
        Dim Vertex1 As sXYZ_sng
        Dim Vertex2 As sXYZ_sng
        Dim Vertex3 As sXYZ_sng

        TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
        TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
        TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
        TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

        Vertex0.X = CSng(TileX * TerrainGridSpacing)
        Vertex0.Y = CSng(TileTerrainHeight(0) * HeightMultiplier)
        Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex1.Y = CSng(TileTerrainHeight(1) * HeightMultiplier)
        Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex2.X = CSng(TileX * TerrainGridSpacing)
        Vertex2.Y = CSng(TileTerrainHeight(2) * HeightMultiplier)
        Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
        Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex3.Y = CSng(TileTerrainHeight(3) * HeightMultiplier)
        Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

        GL.Begin(BeginMode.Lines)
        If Terrain.Tiles(TileX, TileY).Tri Then
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
        Else
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)

            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
        End If
        GL.End()
    End Sub

    Public Sub DrawTileOrientation(ByVal Tile As sXY_int)
        Dim TileOrientation As sTileOrientation
        Dim UnrotatedPos As sXY_int
        Dim Vertex0 As sWorldPos
        Dim Vertex1 As sWorldPos
        Dim Vertex2 As sWorldPos

        TileOrientation = Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation

        UnrotatedPos.X = 32
        UnrotatedPos.Y = 32
        Vertex0 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 32
        Vertex1 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        UnrotatedPos.X = 64
        UnrotatedPos.Y = 64
        Vertex2 = GetTileOffsetRotatedWorldPos(Tile, UnrotatedPos)

        GL.Vertex3(Vertex0.Horizontal.X, Vertex0.Altitude, Vertex0.Horizontal.Y)
        GL.Vertex3(Vertex1.Horizontal.X, Vertex1.Altitude, Vertex1.Horizontal.Y)
        GL.Vertex3(Vertex2.Horizontal.X, Vertex2.Altitude, Vertex2.Horizontal.Y)
    End Sub

    Public Sub DrawTile(ByVal TileX As Integer, ByVal TileY As Integer)
        Dim TileTerrainHeight(3) As Double
        Dim Vertex0 As sXYZ_sng
        Dim Vertex1 As sXYZ_sng
        Dim Vertex2 As sXYZ_sng
        Dim Vertex3 As sXYZ_sng
        Dim Normal0 As sXYZ_sng
        Dim Normal1 As sXYZ_sng
        Dim Normal2 As sXYZ_sng
        Dim Normal3 As sXYZ_sng
        Dim TexCoord0 As sXY_sng
        Dim TexCoord1 As sXY_sng
        Dim TexCoord2 As sXY_sng
        Dim TexCoord3 As sXY_sng
        Dim A As Integer

        If Terrain.Tiles(TileX, TileY).Texture.TextureNum < 0 Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_NoTile)
        ElseIf Tileset Is Nothing Then
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        ElseIf Terrain.Tiles(TileX, TileY).Texture.TextureNum < Tileset.TileCount Then
            A = Tileset.Tiles(Terrain.Tiles(TileX, TileY).Texture.TextureNum).MapView_GL_Texture_Num
            If A = 0 Then
                GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
            Else
                GL.BindTexture(TextureTarget.Texture2D, A)
            End If
        Else
            GL.BindTexture(TextureTarget.Texture2D, GLTexture_OverflowTile)
        End If
        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)

        TileTerrainHeight(0) = Terrain.Vertices(TileX, TileY).Height
        TileTerrainHeight(1) = Terrain.Vertices(TileX + 1, TileY).Height
        TileTerrainHeight(2) = Terrain.Vertices(TileX, TileY + 1).Height
        TileTerrainHeight(3) = Terrain.Vertices(TileX + 1, TileY + 1).Height

        GetTileRotatedTexCoords(Terrain.Tiles(TileX, TileY).Texture.Orientation, TexCoord0, TexCoord1, TexCoord2, TexCoord3)

        Vertex0.X = CSng(TileX * TerrainGridSpacing)
        Vertex0.Y = CSng(TileTerrainHeight(0) * HeightMultiplier)
        Vertex0.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex1.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex1.Y = CSng(TileTerrainHeight(1) * HeightMultiplier)
        Vertex1.Z = CSng(-TileY * TerrainGridSpacing)
        Vertex2.X = CSng(TileX * TerrainGridSpacing)
        Vertex2.Y = CSng(TileTerrainHeight(2) * HeightMultiplier)
        Vertex2.Z = CSng(-(TileY + 1) * TerrainGridSpacing)
        Vertex3.X = CSng((TileX + 1) * TerrainGridSpacing)
        Vertex3.Y = CSng(TileTerrainHeight(3) * HeightMultiplier)
        Vertex3.Z = CSng(-(TileY + 1) * TerrainGridSpacing)

        Normal0 = TerrainVertexNormalCalc(TileX, TileY)
        Normal1 = TerrainVertexNormalCalc(TileX + 1, TileY)
        Normal2 = TerrainVertexNormalCalc(TileX, TileY + 1)
        Normal3 = TerrainVertexNormalCalc(TileX + 1, TileY + 1)

        GL.Begin(BeginMode.Triangles)
        If Terrain.Tiles(TileX, TileY).Tri Then
            GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
            GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
            GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
            GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)

            GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
            GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
            GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
            GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
            GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
        Else
            GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
            GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Normal3(Normal2.X, Normal2.Y, -Normal2.Z)
            GL.TexCoord2(TexCoord2.X, TexCoord2.Y)
            GL.Vertex3(Vertex2.X, Vertex2.Y, -Vertex2.Z)
            GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
            GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)

            GL.Normal3(Normal0.X, Normal0.Y, -Normal0.Z)
            GL.TexCoord2(TexCoord0.X, TexCoord0.Y)
            GL.Vertex3(Vertex0.X, Vertex0.Y, -Vertex0.Z)
            GL.Normal3(Normal3.X, Normal3.Y, -Normal3.Z)
            GL.TexCoord2(TexCoord3.X, TexCoord3.Y)
            GL.Vertex3(Vertex3.X, Vertex3.Y, -Vertex3.Z)
            GL.Normal3(Normal1.X, Normal1.Y, -Normal1.Z)
            GL.TexCoord2(TexCoord1.X, TexCoord1.Y)
            GL.Vertex3(Vertex1.X, Vertex1.Y, -Vertex1.Z)
        End If
        GL.End()
    End Sub

    Public Class clsMinimapTexture
        Public Pixels(,,) As Byte

        Public Sub New(ByVal Size As sXY_int)

            ReDim Pixels(Size.Y - 1, Size.X - 1, 3)
        End Sub
    End Class

    Protected Sub MinimapTextureFill(ByVal Texture As clsMinimapTexture)
        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer
        Dim Low As sXY_int
        Dim High As sXY_int
        Dim Footprint As sXY_int
        Dim Flag As Boolean
        Dim UnitMap(Texture.Pixels.GetUpperBound(0), Texture.Pixels.GetUpperBound(1)) As Boolean
        Dim sngTexture(Texture.Pixels.GetUpperBound(0), Texture.Pixels.GetUpperBound(1), 2) As Single
        Dim Alpha As Single
        Dim AntiAlpha As Single
        Dim RGB_sng As sRGB_sng

        If frmMainInstance.menuMiniShowTex.Checked Then
            If Tileset IsNot Nothing Then
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            sngTexture(Y, X, 0) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Red
                            sngTexture(Y, X, 1) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Green
                            sngTexture(Y, X, 2) = Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).AverageColour.Blue
                        End If
                    Next
                Next
            End If
            If frmMainInstance.menuMiniShowHeight.Checked Then
                Dim Height As Single
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        Height = (CInt(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 1020.0F
                        sngTexture(Y, X, 0) = (sngTexture(Y, X, 0) * 2.0F + Height) / 3.0F
                        sngTexture(Y, X, 1) = (sngTexture(Y, X, 1) * 2.0F + Height) / 3.0F
                        sngTexture(Y, X, 2) = (sngTexture(Y, X, 2) * 2.0F + Height) / 3.0F
                    Next
                Next
            End If
        ElseIf frmMainInstance.menuMiniShowHeight.Checked Then
            Dim Height As Single
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    Height = (CInt(Terrain.Vertices(X, Y).Height) + Terrain.Vertices(X + 1, Y).Height + Terrain.Vertices(X, Y + 1).Height + Terrain.Vertices(X + 1, Y + 1).Height) / 1020.0F
                    sngTexture(Y, X, 0) = Height
                    sngTexture(Y, X, 1) = Height
                    sngTexture(Y, X, 2) = Height
                Next
            Next
        Else
            For Y = 0 To Terrain.TileSize.Y - 1
                For X = 0 To Terrain.TileSize.X - 1
                    sngTexture(Y, X, 0) = 0.0F
                    sngTexture(Y, X, 1) = 0.0F
                    sngTexture(Y, X, 2) = 0.0F
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowCliffs.Checked Then
            If Tileset IsNot Nothing Then
                Alpha = Settings.MinimapCliffColour.Alpha
                AntiAlpha = 1.0F - Alpha
                For Y = 0 To Terrain.TileSize.Y - 1
                    For X = 0 To Terrain.TileSize.X - 1
                        If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 And Terrain.Tiles(X, Y).Texture.TextureNum < Tileset.TileCount Then
                            If Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Then
                                sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * AntiAlpha + Settings.MinimapCliffColour.Red * Alpha
                                sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * AntiAlpha + Settings.MinimapCliffColour.Green * Alpha
                                sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * AntiAlpha + Settings.MinimapCliffColour.Blue * Alpha
                            End If
                        End If
                    Next
                Next
            End If
        End If
        If frmMainInstance.menuMiniShowGateways.Checked Then
            For A = 0 To Gateways.ItemCount - 1
                XY_Reorder(Gateways.Item(A).PosA, Gateways.Item(A).PosB, Low, High)
                For Y = Low.Y To High.Y
                    For X = Low.X To High.X
                        sngTexture(Y, X, 0) = 1.0F
                        sngTexture(Y, X, 1) = 1.0F
                        sngTexture(Y, X, 2) = 0.0F
                    Next
                Next
            Next
        End If
        If frmMainInstance.menuMiniShowUnits.Checked Then
            'units that are not selected
            For A = 0 To Units.ItemCount - 1
                Flag = True
                If frmMainInstance.SelectedObjectType Is Units.Item(A).Type Then
                    Flag = False
                Else
                    Footprint = Units.Item(A).Type.GetFootprint
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Units.Item(A).Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                If Settings.MinimapTeamColours Then
                                    If Settings.MinimapTeamColoursExceptFeatures And Units.Item(A).Type.Type = clsUnitType.enumType.Feature Then
                                        sngTexture(Y, X, 0) = MinimapFeatureColour.Red
                                        sngTexture(Y, X, 1) = MinimapFeatureColour.Green
                                        sngTexture(Y, X, 2) = MinimapFeatureColour.Blue
                                    Else
                                        RGB_sng = GetUnitGroupMinimapColour(Units.Item(A).UnitGroup)
                                        sngTexture(Y, X, 0) = RGB_sng.Red
                                        sngTexture(Y, X, 1) = RGB_sng.Green
                                        sngTexture(Y, X, 2) = RGB_sng.Blue
                                    End If
                                Else
                                    sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * 0.6666667F + 0.333333343F
                                    sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * 0.6666667F
                                    sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * 0.6666667F
                                End If
                            End If
                        Next
                    Next
                End If
            Next
            'reset unit map
            For Y = 0 To Texture.Pixels.GetUpperBound(0)
                For X = 0 To Texture.Pixels.GetUpperBound(1)
                    UnitMap(Y, X) = False
                Next
            Next
            'units that are selected and highlighted
            Alpha = Settings.MinimapSelectedObjectsColour.Alpha
            AntiAlpha = 1.0F - Alpha
            For A = 0 To Units.ItemCount - 1
                Flag = False
                If frmMainInstance.SelectedObjectType Is Units.Item(A).Type Then
                    Flag = True
                    Footprint = Units.Item(A).Type.GetFootprint
                    Footprint.X += 2
                    Footprint.Y += 2
                End If
                If Flag Then
                    GetFootprintTileRangeClamped(Units.Item(A).Pos.Horizontal, Footprint, Low, High)
                    For Y = Low.Y To High.Y
                        For X = Low.X To High.X
                            If Not UnitMap(Y, X) Then
                                UnitMap(Y, X) = True
                                sngTexture(Y, X, 0) = sngTexture(Y, X, 0) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Red * Alpha
                                sngTexture(Y, X, 1) = sngTexture(Y, X, 1) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Green * Alpha
                                sngTexture(Y, X, 2) = sngTexture(Y, X, 2) * AntiAlpha + Settings.MinimapSelectedObjectsColour.Blue * Alpha
                            End If
                        Next
                    Next
                End If
            Next
        End If
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                Texture.Pixels(Y, X, 0) = CByte(Clamp_dbl(sngTexture(Y, X, 0) * 255.0F, 0.0#, 255.0#))
                Texture.Pixels(Y, X, 1) = CByte(Clamp_dbl(sngTexture(Y, X, 1) * 255.0F, 0.0#, 255.0#))
                Texture.Pixels(Y, X, 2) = CByte(Clamp_dbl(sngTexture(Y, X, 2) * 255.0F, 0.0#, 255.0#))
                Texture.Pixels(Y, X, 3) = CByte(255)
            Next
        Next
    End Sub

#If Mono <> 0.0# Then
            Private MinimapBitmap As Bitmap
#End If
    Private MinimapPending As Boolean
    Private WithEvents MakeMinimapTimer As Timer
    Public SuppressMinimap As Boolean

    Private Sub MinimapTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles MakeMinimapTimer.Tick

        If MainMap IsNot Me Then
            MinimapPending = False
        End If
        If MinimapPending Then
            If SuppressMinimap Then
                'should restart the timer here, but i cant find a good way to
            Else
                MinimapPending = False
                MinimapMake()
            End If
        Else
            MakeMinimapTimer.Enabled = False
        End If
    End Sub

    Public Sub MinimapMakeLater()

        If MainMap IsNot Me Then
            MsgBox("Error: Hidden minimap is redrawing")
            Exit Sub
        End If

        If MakeMinimapTimer.Enabled Then
            MinimapPending = True
        Else
            MakeMinimapTimer.Enabled = True
            If SuppressMinimap Then
                MinimapPending = True
            Else
                MinimapMake()
            End If
        End If
    End Sub

    Private Sub MinimapMake()

        Dim NewTextureSize As Integer = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(Terrain.TileSize.X, Terrain.TileSize.Y)) / Math.Log(2.0#))))

        If NewTextureSize <> Minimap_Texture_Size Then
            Minimap_Texture_Size = NewTextureSize
#If Mono <> 0.0# Then
                    MinimapBitmap = New Bitmap(Minimap_Texture_Size, Minimap_Texture_Size)
#End If
        End If

        Dim Texture As New clsMap.clsMinimapTexture(New sXY_int(Minimap_Texture_Size, Minimap_Texture_Size))

        MinimapTextureFill(Texture)

#If Mono <> 0.0# Then
            Dim TextureB As Bitmap

            TextureB = MinimapBitmap

            Dim X As Integer
            Dim Y As Integer

            For Y = 0 To Minimap_Texture_Size - 1
                For X = 0 To Minimap_Texture_Size - 1
                    TextureB.SetPixel(X, Y, ColorTranslator.FromOle(OSRGB(Texture.Pixels(Y, X, 0), Texture.Pixels(Y, X, 1), Texture.Pixels(Y, X, 2))))
                Next
            Next
#End If

        Minimap_GLDelete()

#If Mono = 0.0# Then
        GL.GenTextures(1, Minimap_GLTexture)
        GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Minimap_Texture_Size, Minimap_Texture_Size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Texture.Pixels)
#Else
        Dim BitmapTextureArgs As sBitmapGLTexture
        BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
        BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
        BitmapTextureArgs.TextureNum = 0
        BitmapTextureArgs.MipMapLevel = 0
        BitmapTextureArgs.Texture = TextureB
        BitmapTextureArgs.Perform()
        Minimap_GLTexture = BitmapTextureArgs.TextureNum
#End If

        frmMainInstance.View_DrawViewLater()
    End Sub

    Public Sub Minimap_GLDelete()

        If Minimap_GLTexture <> 0 Then
            GL.DeleteTextures(1, Minimap_GLTexture)
            Minimap_GLTexture = 0
        End If
    End Sub

    Public Function GetAvailableID() As UInteger
        Dim A As Integer
        Dim ID As UInteger

        ID = 1UI
        For A = 0 To Units.ItemCount - 1
            If Units.Item(A).ID >= ID Then
                ID = Units.Item(A).ID + 1UI
            End If
        Next

        Return ID
    End Function

    Public Class clsUnitAdd
        Public Map As clsMap
        Public NewUnit As clsUnit
        Public ID As UInteger = 0UI
        Public Label As String = Nothing
        Public StoreChange As Boolean = False

        Public Function Perform() As Boolean
            Dim A As Integer

            If Map Is Nothing Then
                Stop
                Return False
            End If
            If NewUnit Is Nothing Then
                Stop
                Return False
            End If

            If NewUnit.MapLink.IsConnected Then
                MsgBox("Error: Added object already has a map assigned.")
                Return False
            End If
            If NewUnit.UnitGroup Is Nothing Then
                MsgBox("Error: Added object has no group.")
                NewUnit.UnitGroup = Map.ScavengerUnitGroup
                Return False
            End If
            If NewUnit.UnitGroup.MapLink.Source IsNot Map Then
                MsgBox("Error: Something terrible happened.")
                Return False
            End If

            If StoreChange Then
                Dim UnitChange As New clsMap.clsUnitChange
                UnitChange.Type = clsUnitChange.enumType.Added
                UnitChange.Unit = NewUnit
                Map.UnitChanges.Add(UnitChange)
            End If

            If ID <= 0UI Then
                ID = Map.GetAvailableID
            Else
                For A = 0 To Map.Units.ItemCount - 1
                    If ID = Map.Units.Item(A).ID Then
                        Exit For
                    End If
                Next
                If A < Map.Units.ItemCount Then
                    ID = Map.GetAvailableID
                End If
            End If

            NewUnit.ID = ID

            NewUnit.MapLink.Connect(Map.Units)

            NewUnit.Pos.Horizontal.X = Clamp_int(NewUnit.Pos.Horizontal.X, 0, Map.Terrain.TileSize.X * TerrainGridSpacing - 1)
            NewUnit.Pos.Horizontal.Y = Clamp_int(NewUnit.Pos.Horizontal.Y, 0, Map.Terrain.TileSize.Y * TerrainGridSpacing - 1)
            NewUnit.Pos.Altitude = CInt(Math.Ceiling(Map.GetTerrainHeight(NewUnit.Pos.Horizontal)))

            If Label IsNot Nothing Then
                NewUnit.SetLabel(Label)
            End If

            Map.UnitSectorsCalc(NewUnit)

            If Map.SectorGraphicsChanges IsNot Nothing Then
                Map.UnitSectors_GLList(NewUnit)
            End If

            Return True
        End Function
    End Class

    Public Sub Unit_Remove_StoreChange(ByVal Num As Integer)

        Dim UnitChange As New clsMap.clsUnitChange
        UnitChange.Type = clsUnitChange.enumType.Deleted
        UnitChange.Unit = Units.Item(Num)
        UnitChanges.Add(UnitChange)

        Unit_Remove(Num)
    End Sub

    Public Sub Unit_Remove(ByVal Num As Integer)
        Dim tmpUnit As clsMap.clsUnit

        tmpUnit = Units.Item(Num)

        If SectorGraphicsChanges IsNot Nothing Then
            UnitSectors_GLList(tmpUnit)
        End If

        If ViewInfo IsNot Nothing Then
            Dim MouseOverTerrain As clsViewInfo.clsMouseOver.clsOverTerrain = ViewInfo.GetMouseOverTerrain
            If MouseOverTerrain IsNot Nothing Then
                Dim Pos As Integer = MouseOverTerrain.Units.FindFirstItemPosition(tmpUnit)
                If Pos >= 0 Then
                    MouseOverTerrain.Units.Remove(Pos)
                End If
            End If
        End If

        tmpUnit.DisconnectFromMap()
    End Sub

    Public Sub UnitSwap(ByVal OldUnit As clsMap.clsUnit, ByVal NewUnit As clsMap.clsUnit)

        If OldUnit.MapLink.Source IsNot Me Then
            Stop
            Exit Sub
        End If

        Unit_Remove_StoreChange(OldUnit.MapLink.ArrayPosition)
        Dim UnitAdd As New clsMap.clsUnitAdd
        UnitAdd.Map = Me
        UnitAdd.StoreChange = True
        UnitAdd.ID = OldUnit.ID
        UnitAdd.NewUnit = NewUnit
        UnitAdd.Label = OldUnit.Label
        UnitAdd.Perform()
        ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap")
    End Sub

    Public Function GetTileSectorNum(ByVal Tile As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Tile.X / SectorTileSize))
        Result.Y = CInt(Int(Tile.Y / SectorTileSize))

        Return Result
    End Function

    Public Sub GetTileSectorRange(ByVal StartTile As sXY_int, ByVal FinishTile As sXY_int, ByRef ResultSectorStart As sXY_int, ByRef ResultSectorFinish As sXY_int)

        ResultSectorStart = GetTileSectorNum(StartTile)
        ResultSectorFinish = GetTileSectorNum(FinishTile)
        ResultSectorStart.X = Clamp_int(ResultSectorStart.X, 0, SectorCount.X - 1)
        ResultSectorStart.Y = Clamp_int(ResultSectorStart.Y, 0, SectorCount.Y - 1)
        ResultSectorFinish.X = Clamp_int(ResultSectorFinish.X, 0, SectorCount.X - 1)
        ResultSectorFinish.Y = Clamp_int(ResultSectorFinish.Y, 0, SectorCount.Y - 1)
    End Sub

    Public Function TileAligned_Pos_From_MapPos(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Result.Horizontal.X = CInt((Math.Round((Horizontal.X - Footprint.X * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.X / 2.0#) * TerrainGridSpacing)
        Result.Horizontal.Y = CInt((Math.Round((Horizontal.Y - Footprint.Y * TerrainGridSpacing / 2.0#) / TerrainGridSpacing) + Footprint.Y / 2.0#) * TerrainGridSpacing)
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub UnitSectorsCalc(ByVal Unit As clsUnit)
        Dim Start As sXY_int
        Dim Finish As sXY_int
        Dim TileStart As sXY_int
        Dim TileFinish As sXY_int
        Dim tmpConnection As clsMap.clsUnitSectorConnection
        Dim X As Integer
        Dim Y As Integer

        GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.GetFootprint, TileStart, TileFinish)
        Start = GetTileSectorNum(TileStart)
        Finish = GetTileSectorNum(TileFinish)
        Start.X = Clamp_int(Start.X, 0, SectorCount.X - 1)
        Start.Y = Clamp_int(Start.Y, 0, SectorCount.Y - 1)
        Finish.X = Clamp_int(Finish.X, 0, SectorCount.X - 1)
        Finish.Y = Clamp_int(Finish.Y, 0, SectorCount.Y - 1)
        Unit.Sectors.Clear()
        For Y = Start.Y To Finish.Y
            For X = Start.X To Finish.X
                tmpConnection = clsMap.clsUnitSectorConnection.Create(Unit, Sectors(X, Y))
            Next
        Next
    End Sub

    Public Sub AutoSave_Test()

        If Not Settings.AutoSaveEnabled Then
            Exit Sub
        End If
        If AutoSave.ChangeCount < Settings.AutoSave_MinChanges Then
            Exit Sub
        End If
        If DateDiff("s", AutoSave.SavedDate, Now) < Settings.AutoSave_MinInterval_s Then
            Exit Sub
        End If

        AutoSave.ChangeCount = 0
        AutoSave.SavedDate = Now

        AutoSave_Perform()
    End Sub

    Public Sub AutoSave_Perform()

        If Not IO.Directory.Exists(AutoSavePath) Then
            Try
                IO.Directory.CreateDirectory(AutoSavePath)
            Catch ex As Exception
                MsgBox("Unable to create autosave directory during autosave.", CType(MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, MsgBoxStyle), "")
            End Try
        End If

        Dim DateNow As Date = Now
        Dim Path As String

        Path = AutoSavePath & "autosaved-" & InvariantToString_int(DateNow.Year) & "-" & MinDigits(DateNow.Month, 2) & "-" & MinDigits(DateNow.Day, 2) & "-" & MinDigits(DateNow.Hour, 2) & "-" & MinDigits(DateNow.Minute, 2) & "-" & MinDigits(DateNow.Second, 2) & "-" & MinDigits(DateNow.Millisecond, 3) & ".fmap"

        Dim Result As clsResult = Write_FMap(Path, False, Settings.AutoSaveCompress)

        ShowWarnings(Result, "Autosave")
    End Sub

    Public Sub UndoStepCreate(ByVal StepName As String)
        Dim A As Integer
        Dim SectorNum As sXY_int
        Dim NewUndo As New clsUndo

        NewUndo.Name = StepName

        NewUndo.ChangedSectorCount = SectorTerrainUndoChanges.ChangedPointCount
        ReDim NewUndo.ChangedSectors(NewUndo.ChangedSectorCount - 1)
        For A = 0 To SectorTerrainUndoChanges.ChangedPointCount - 1
            SectorNum = SectorTerrainUndoChanges.ChangedPoints(A)
            NewUndo.ChangedSectors(A) = ShadowSectors(SectorNum.X, SectorNum.Y)
            ShadowSector_Create(SectorNum)
        Next
        SectorTerrainUndoChanges.Clear()

        NewUndo.UnitChanges = New SimpleClassList(Of clsMap.clsUnitChange)
        UnitChanges.SendItems(NewUndo.UnitChanges)
        UnitChanges.Clear()

        NewUndo.GatewayChanges = New SimpleClassList(Of clsMap.clsGatewayChange)
        GatewayChanges.SendItems(NewUndo.GatewayChanges)
        GatewayChanges.Clear()

        If NewUndo.ChangedSectorCount + NewUndo.UnitChanges.ItemCount + NewUndo.GatewayChanges.ItemCount > 0 Then
            Do While Undos.ItemCount > UndoPosition 'a new line has been started so remove redos
                Undos.Remove(Undos.ItemCount - 1)
            Loop

            Undos.Add(NewUndo)
            UndoPosition = Undos.ItemCount

            SetChanged()
        End If
    End Sub

    Public Sub ShadowSector_Create(ByVal SectorNum As sXY_int)
        Dim TileX As Integer
        Dim TileY As Integer
        Dim StartX As Integer
        Dim StartY As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim tmpShadowSector As clsShadowSector
        Dim LastTileX As Integer
        Dim LastTileY As Integer

        tmpShadowSector = New clsShadowSector
        ShadowSectors(SectorNum.X, SectorNum.Y) = tmpShadowSector
        tmpShadowSector.Num = SectorNum
        StartX = SectorNum.X * SectorTileSize
        StartY = SectorNum.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileY = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartY)
        For Y = 0 To LastTileY
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.Vertices(X, Y).Height = Terrain.Vertices(TileX, TileY).Height
                tmpShadowSector.Terrain.Vertices(X, Y).Terrain = Terrain.Vertices(TileX, TileY).Terrain
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.Tiles(X, Y).Copy(Terrain.Tiles(TileX, TileY))
            Next
        Next
        For Y = 0 To LastTileY
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.SideH(X, Y).Road = Terrain.SideH(TileX, TileY).Road
            Next
        Next
        For Y = 0 To LastTileY - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileY = StartY + Y
                tmpShadowSector.Terrain.SideV(X, Y).Road = Terrain.SideV(TileX, TileY).Road
            Next
        Next
    End Sub

    Public Sub Undo_Clear()

        UndoStepCreate("") 'absorb current changes
        Dim A As Integer
        Dim B As Integer
        For A = 0 To Undos.ItemCount - 1
            For B = 0 To Undos.Item(A).UnitChanges.ItemCount - 1
                Undos.Item(A).UnitChanges.Item(B).Unit.Deallocate()
            Next
        Next
        Undos.Clear()
        UndoPosition = Undos.ItemCount
    End Sub

    Public Sub Undo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Y As Integer
        Dim tmpUnit As clsUnit
        Dim ID As UInteger
        Dim UnitAdd As New clsMap.clsUnitAdd
        Dim tmpUndo As clsMap.clsUndo

        UnitAdd.Map = Me

        UndoStepCreate("Incomplete Action") 'make another redo step incase something has changed, such as if user presses undo while still dragging a tool

        UndoPosition -= 1

        tmpUndo = Undos.Item(UndoPosition)
        For A = 0 To tmpUndo.ChangedSectorCount - 1
            X = tmpUndo.ChangedSectors(A).Num.X
            Y = tmpUndo.ChangedSectors(A).Num.Y
            'store existing state for redo
            tmpShadow = ShadowSectors(X, Y)
            'remove graphics from sector
            Sectors(X, Y).DeleteLists()
            'perform the undo
            Undo_Sector_Rejoin(tmpUndo.ChangedSectors(A))
            'update the backup
            ShadowSector_Create(New sXY_int(X, Y))
            'add old state to the redo step (that was this undo step)
            tmpUndo.ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To tmpUndo.ChangedSectorCount - 1
            SectorGraphicsChanges.Changed(tmpUndo.ChangedSectors(A).Num)
        Next

        For A = tmpUndo.UnitChanges.ItemCount - 1 To 0 Step -1 'must do in reverse order, otherwise may try to delete units that havent been added yet
            tmpUnit = tmpUndo.UnitChanges.Item(A).Unit
            Select Case tmpUndo.UnitChanges.Item(A).Type
                Case clsUnitChange.enumType.Added
                    'remove the unit from the map
                    Unit_Remove(tmpUnit.MapLink.ArrayPosition)
                Case clsUnitChange.enumType.Deleted
                    'add the unit back on to the map
                    ID = tmpUnit.ID
                    UnitAdd.ID = ID
                    UnitAdd.NewUnit = tmpUnit
                    UnitAdd.Perform()
                    ErrorIDChange(ID, tmpUnit, "Undo_Perform")
                Case Else
                    Stop
            End Select
        Next

        For A = tmpUndo.GatewayChanges.ItemCount - 1 To 0 Step -1
            Select Case tmpUndo.GatewayChanges.Item(A).Type
                Case clsGatewayChange.enumType.Added
                    'remove the unit from the map
                    tmpUndo.GatewayChanges.Item(A).Gateway.MapLink.Disconnect()
                Case clsGatewayChange.enumType.Deleted
                    'add the unit back on to the map
                    tmpUndo.GatewayChanges.Item(A).Gateway.MapLink.Connect(Gateways)
                Case Else
                    Stop
            End Select
        Next

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub Redo_Perform()
        Dim A As Integer
        Dim tmpShadow As clsShadowSector
        Dim X As Integer
        Dim Y As Integer
        Dim tmpUnit As clsUnit
        Dim ID As UInteger
        Dim UnitAdd As New clsMap.clsUnitAdd
        Dim tmpUndo As clsMap.clsUndo

        tmpUndo = Undos.Item(UndoPosition)

        UnitAdd.Map = Me

        For A = 0 To tmpUndo.ChangedSectorCount - 1
            X = tmpUndo.ChangedSectors(A).Num.X
            Y = tmpUndo.ChangedSectors(A).Num.Y
            'store existing state for undo
            tmpShadow = ShadowSectors(X, Y)
            'remove graphics from sector
            Sectors(X, Y).DeleteLists()
            'perform the redo
            Undo_Sector_Rejoin(tmpUndo.ChangedSectors(A))
            'update the backup
            ShadowSector_Create(New sXY_int(X, Y))
            'add old state to the undo step (that was this redo step)
            tmpUndo.ChangedSectors(A) = tmpShadow
        Next
        For A = 0 To tmpUndo.ChangedSectorCount - 1
            SectorGraphicsChanges.Changed(tmpUndo.ChangedSectors(A).Num)
        Next

        For A = 0 To tmpUndo.UnitChanges.ItemCount - 1
            tmpUnit = tmpUndo.UnitChanges.Item(A).Unit
            Select Case tmpUndo.UnitChanges.Item(A).Type
                Case clsUnitChange.enumType.Added
                    'add the unit back on to the map
                    ID = tmpUnit.ID
                    UnitAdd.ID = ID
                    UnitAdd.NewUnit = tmpUnit
                    UnitAdd.Perform()
                    ErrorIDChange(ID, tmpUnit, "Redo_Perform")
                Case clsUnitChange.enumType.Deleted
                    'remove the unit from the map
                    Unit_Remove(tmpUnit.MapLink.ArrayPosition)
                Case Else
                    Stop
            End Select
        Next

        For A = 0 To tmpUndo.GatewayChanges.ItemCount - 1
            Select Case tmpUndo.GatewayChanges.Item(A).Type
                Case clsGatewayChange.enumType.Added
                    'add the unit back on to the map
                    tmpUndo.GatewayChanges.Item(A).Gateway.MapLink.Connect(Gateways)
                Case clsGatewayChange.enumType.Deleted
                    'remove the unit from the map
                    tmpUndo.GatewayChanges.Item(A).Gateway.MapLink.Disconnect()
                Case Else
                    Stop
            End Select
        Next

        UndoPosition += 1

        SectorsUpdateGraphics()
        MinimapMakeLater()
        frmMainInstance.SelectedObject_Changed()
    End Sub

    Public Sub Undo_Sector_Rejoin(ByVal Shadow_Sector_To_Rejoin As clsShadowSector)
        Dim TileX As Integer
        Dim TileZ As Integer
        Dim StartX As Integer
        Dim StartZ As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim LastTileX As Integer
        Dim LastTileZ As Integer

        StartX = Shadow_Sector_To_Rejoin.Num.X * SectorTileSize
        StartZ = Shadow_Sector_To_Rejoin.Num.Y * SectorTileSize
        LastTileX = Math.Min(SectorTileSize, Terrain.TileSize.X - StartX)
        LastTileZ = Math.Min(SectorTileSize, Terrain.TileSize.Y - StartZ)
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Vertices(TileX, TileZ).Height = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Height
                Terrain.Vertices(TileX, TileZ).Terrain = Shadow_Sector_To_Rejoin.Terrain.Vertices(X, Y).Terrain
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.Tiles(TileX, TileZ).Copy(Shadow_Sector_To_Rejoin.Terrain.Tiles(X, Y))
            Next
        Next
        For Y = 0 To LastTileZ
            For X = 0 To LastTileX - 1
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideH(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideH(X, Y).Road
            Next
        Next
        For Y = 0 To LastTileZ - 1
            For X = 0 To LastTileX
                TileX = StartX + X
                TileZ = StartZ + Y
                Terrain.SideV(TileX, TileZ).Road = Shadow_Sector_To_Rejoin.Terrain.SideV(X, Y).Road
            Next
        Next
    End Sub

    Public Sub Map_Insert(ByVal Map_To_Insert As clsMap, ByVal Offset As sXY_int, ByVal Area As sXY_int, ByVal Insert_Heights As Boolean, ByVal Insert_Textures As Boolean, ByVal Insert_Units As Boolean, ByVal Delete_Units As Boolean, ByVal Insert_Gateways As Boolean, ByVal Delete_Gateways As Boolean)
        Dim Finish As sXY_int
        Dim X As Integer
        Dim Y As Integer
        Dim SectorStart As sXY_int
        Dim SectorFinish As sXY_int
        Dim AreaAdjusted As sXY_int
        Dim SectorNum As sXY_int

        Finish.X = Math.Min(Offset.X + Math.Min(Area.X, Map_To_Insert.Terrain.TileSize.X), Terrain.TileSize.X)
        Finish.Y = Math.Min(Offset.Y + Math.Min(Area.Y, Map_To_Insert.Terrain.TileSize.Y), Terrain.TileSize.Y)
        AreaAdjusted.X = Finish.X - Offset.X
        AreaAdjusted.Y = Finish.Y - Offset.Y

        GetTileSectorRange(New sXY_int(Offset.X - 1, Offset.Y - 1), Finish, SectorStart, SectorFinish)
        For Y = SectorStart.Y To SectorFinish.Y
            SectorNum.Y = Y
            For X = SectorStart.X To SectorFinish.X
                SectorNum.X = X
                SectorGraphicsChanges.Changed(SectorNum)
                SectorUnitHeightsChanges.Changed(SectorNum)
                SectorTerrainUndoChanges.Changed(SectorNum)
            Next
        Next

        If Insert_Heights Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Height = Map_To_Insert.Terrain.Vertices(X, Y).Height
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = Map_To_Insert.Terrain.Tiles(X, Y).Tri
                Next
            Next
        End If
        If Insert_Textures Then
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X
                    Terrain.Vertices(Offset.X + X, Offset.Y + Y).Terrain = Map_To_Insert.Terrain.Vertices(X, Y).Terrain
                Next
            Next
            Dim tmpTri As Boolean
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X - 1
                    tmpTri = Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Copy(Map_To_Insert.Terrain.Tiles(X, Y))
                    Terrain.Tiles(Offset.X + X, Offset.Y + Y).Tri = tmpTri
                Next
            Next
            For Y = 0 To AreaAdjusted.Y
                For X = 0 To AreaAdjusted.X - 1
                    Terrain.SideH(Offset.X + X, Offset.Y + Y).Road = Map_To_Insert.Terrain.SideH(X, Y).Road
                Next
            Next
            For Y = 0 To AreaAdjusted.Y - 1
                For X = 0 To AreaAdjusted.X
                    Terrain.SideV(Offset.X + X, Offset.Y + Y).Road = Map_To_Insert.Terrain.SideV(X, Y).Road
                Next
            Next
        End If

        If Delete_Gateways Then
            Dim A As Integer
            A = 0
            Do While A < Gateways.ItemCount
                If (Gateways.Item(A).PosA.X >= Offset.X And Gateways.Item(A).PosA.Y >= Offset.Y And _
                    Gateways.Item(A).PosA.X < Offset.X + AreaAdjusted.X And Gateways.Item(A).PosA.Y < Offset.Y + AreaAdjusted.Y) Or _
                    (Gateways.Item(A).PosB.X >= Offset.X And Gateways.Item(A).PosB.Y >= Offset.Y And _
                    Gateways.Item(A).PosB.X < Offset.X + AreaAdjusted.X And Gateways.Item(A).PosB.Y < Offset.Y + AreaAdjusted.Y) Then
                    Gateway_Remove_StoreChange(A)
                Else
                    A += 1
                End If
            Loop
        End If
        If Insert_Gateways Then
            Dim A As Integer
            Dim GateStart As sXY_int
            Dim GateFinish As sXY_int
            For A = 0 To Map_To_Insert.Gateways.ItemCount - 1
                GateStart.X = Offset.X + Map_To_Insert.Gateways.Item(A).PosA.X
                GateStart.Y = Offset.Y + Map_To_Insert.Gateways.Item(A).PosA.Y
                GateFinish.X = Offset.X + Map_To_Insert.Gateways.Item(A).PosB.X
                GateFinish.Y = Offset.Y + Map_To_Insert.Gateways.Item(A).PosB.Y
                If (GateStart.X >= Offset.X And GateStart.Y >= Offset.Y And _
                    GateStart.X < Offset.X + AreaAdjusted.X And GateStart.Y < Offset.Y + AreaAdjusted.Y) Or _
                    (GateFinish.X >= Offset.X And GateFinish.Y >= Offset.Y And _
                    GateFinish.X < Offset.X + AreaAdjusted.X And GateFinish.Y < Offset.Y + AreaAdjusted.Y) Then
                    Gateway_Create_StoreChange(GateStart, GateFinish)
                End If
            Next
        End If

        If Delete_Units Then
            Dim A As Integer
            Dim TempUnit As clsUnit
            Dim UnitsToDelete(-1) As clsUnit
            Dim UnitToDeleteCount As Integer = 0
            For Y = SectorStart.Y To SectorFinish.Y
                For X = SectorStart.X To SectorFinish.X
                    For A = 0 To Sectors(X, Y).Units.ItemCount - 1
                        TempUnit = Sectors(X, Y).Units.Item(A).Unit
                        If PosIsWithinTileArea(TempUnit.Pos.Horizontal, Offset, Finish) Then
                            ReDim Preserve UnitsToDelete(UnitToDeleteCount)
                            UnitsToDelete(UnitToDeleteCount) = TempUnit
                            UnitToDeleteCount += 1
                        End If
                    Next
                Next
            Next
            For A = 0 To UnitToDeleteCount - 1
                If UnitsToDelete(A).MapLink.IsConnected Then 'units may be in the list multiple times and already be deleted
                    Unit_Remove_StoreChange(UnitsToDelete(A).MapLink.ArrayPosition)
                End If
            Next
        End If
        If Insert_Units Then
            Dim PosDif As sXY_int
            Dim A As Integer
            Dim NewUnit As clsUnit
            Dim tmpUnit As clsUnit
            Dim ZeroPos As New sXY_int(0, 0)
            Dim UnitAdd As New clsMap.clsUnitAdd

            UnitAdd.Map = Me
            UnitAdd.StoreChange = True

            PosDif.X = Offset.X * TerrainGridSpacing
            PosDif.Y = Offset.Y * TerrainGridSpacing
            For A = 0 To Map_To_Insert.Units.ItemCount - 1
                tmpUnit = Map_To_Insert.Units.Item(A)
                If PosIsWithinTileArea(tmpUnit.Pos.Horizontal, ZeroPos, AreaAdjusted) Then
                    NewUnit = New clsUnit(Map_To_Insert.Units.Item(A), Me)
                    NewUnit.Pos.Horizontal.X += PosDif.X
                    NewUnit.Pos.Horizontal.Y += PosDif.Y
                    UnitAdd.NewUnit = NewUnit
                    UnitAdd.Label = tmpUnit.Label
                    UnitAdd.Perform()
                End If
            Next
        End If

        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
        MinimapMakeLater()
    End Sub

    Public Function Gateway_Create(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As clsGateway

        If PosA.X >= 0 And PosA.X < Terrain.TileSize.X And _
            PosA.Y >= 0 And PosA.Y < Terrain.TileSize.Y And _
            PosB.X >= 0 And PosB.X < Terrain.TileSize.X And _
            PosB.Y >= 0 And PosB.Y < Terrain.TileSize.Y Then 'is on map
            If PosA.X = PosB.X Or PosA.Y = PosB.Y Then 'is straight

                Dim tmpGateway As New clsGateway

                tmpGateway.PosA = PosA
                tmpGateway.PosB = PosB

                tmpGateway.MapLink.Connect(Gateways)

                Return tmpGateway
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function Gateway_Create_StoreChange(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As clsGateway
        Dim tmpGateway As clsGateway

        tmpGateway = Gateway_Create(PosA, PosB)

        Dim GatewayChange As New clsGatewayChange
        GatewayChange.Type = clsGatewayChange.enumType.Added
        GatewayChange.Gateway = tmpGateway
        GatewayChanges.Add(GatewayChange)

        Return tmpGateway
    End Function

    Public Sub Gateway_Remove_StoreChange(ByVal Num As Integer)

        Dim GatewayChange As New clsGatewayChange
        GatewayChange.Type = clsGatewayChange.enumType.Deleted
        GatewayChange.Gateway = Gateways.Item(Num)
        GatewayChanges.Add(GatewayChange)

        Gateways.Item(Num).MapLink.Disconnect()
    End Sub

    Public Sub TileType_Reset()

        If Tileset Is Nothing Then
            ReDim Tile_TypeNum(-1)
        Else
            Dim A As Integer

            ReDim Tile_TypeNum(Tileset.TileCount - 1)
            For A = 0 To Tileset.TileCount - 1
                Tile_TypeNum(A) = Tileset.Tiles(A).Default_Type
            Next
        End If
    End Sub

    Public Sub SetPainterToDefaults()

        If Tileset Is Nothing Then
            Painter = New clsPainter
        ElseIf Tileset Is Tileset_Arizona Then
            Painter = Painter_Arizona
        ElseIf Tileset Is Tileset_Urban Then
            Painter = Painter_Urban
        ElseIf Tileset Is Tileset_Rockies Then
            Painter = Painter_Rockies
        Else
            Painter = New clsPainter
        End If
    End Sub

    Private Sub UnitSectors_GLList(ByVal UnitToUpdateFor As clsUnit)

        If SectorGraphicsChanges Is Nothing Then
            Stop
            Exit Sub
        End If

        Dim A As Integer

        For A = 0 To UnitToUpdateFor.Sectors.ItemCount - 1
            SectorGraphicsChanges.Changed(UnitToUpdateFor.Sectors.Item(A).Sector.Pos)
        Next
    End Sub

    Public Function GetTileOffsetRotatedWorldPos(ByVal Tile As sXY_int, ByVal TileOffsetToRotate As sXY_int) As sWorldPos
        Dim Result As sWorldPos

        Dim RotatedOffset As sXY_int

        RotatedOffset = GetTileRotatedOffset(Terrain.Tiles(Tile.X, Tile.Y).Texture.Orientation, TileOffsetToRotate)
        Result.Horizontal.X = Tile.X * TerrainGridSpacing + RotatedOffset.X
        Result.Horizontal.Y = Tile.Y * TerrainGridSpacing + RotatedOffset.Y
        Result.Altitude = CInt(GetTerrainHeight(Result.Horizontal))

        Return Result
    End Function

    Public Sub GetFootprintTileRangeClamped(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)
        Dim Remainder As Integer
        Dim Centre As sXY_int = GetPosTileNum(Horizontal)
        Dim Half As Integer

        Half = Math.DivRem(Footprint.X, 2, Remainder)
        ResultStart.X = Clamp_int(Centre.X - Half, 0, Terrain.TileSize.X - 1)
        ResultFinish.X = Clamp_int(ResultStart.X + Footprint.X - 1, 0, Terrain.TileSize.X - 1)
        Half = Math.DivRem(Footprint.Y, 2, Remainder)
        ResultStart.Y = Clamp_int(Centre.Y - Half, 0, Terrain.TileSize.Y - 1)
        ResultFinish.Y = Clamp_int(ResultStart.Y + Footprint.Y - 1, 0, Terrain.TileSize.Y - 1)
    End Sub

    Public Sub GetFootprintTileRange(ByVal Horizontal As sXY_int, ByVal Footprint As sXY_int, ByRef ResultStart As sXY_int, ByRef ResultFinish As sXY_int)

        Dim Remainder As Integer
        Dim Centre As sXY_int = GetPosTileNum(Horizontal)
        Dim Half As Integer

        Half = Math.DivRem(Footprint.X, 2, Remainder)
        ResultStart.X = Centre.X - Half
        ResultFinish.X = ResultStart.X + Footprint.X - 1
        Half = Math.DivRem(Footprint.Y, 2, Remainder)
        ResultStart.Y = Centre.Y - Half
        ResultFinish.Y = ResultStart.Y + Footprint.Y - 1
    End Sub

    Public Function GetPosTileNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Int(Horizontal.X / TerrainGridSpacing))
        Result.Y = CInt(Int(Horizontal.Y / TerrainGridSpacing))

        Return Result
    End Function

    Public Function GetPosVertexNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = CInt(Math.Round(Horizontal.X / TerrainGridSpacing))
        Result.Y = CInt(Math.Round(Horizontal.Y / TerrainGridSpacing))

        Return Result
    End Function

    Public Function GetPosSectorNum(ByVal Horizontal As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result = GetTileSectorNum(GetPosTileNum(Horizontal))

        Return Result
    End Function

    Public Function GetSectorNumClamped(ByVal SectorNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(SectorNum.X, 0, SectorCount.X - 1)
        Result.Y = Clamp_int(SectorNum.Y, 0, SectorCount.Y - 1)

        Return Result
    End Function

    Public Function GetVertexAltitude(ByVal VertexNum As sXY_int) As Integer

        Return Terrain.Vertices(VertexNum.X, VertexNum.Y).Height * HeightMultiplier
    End Function

    Public Function PosIsOnMap(ByVal Horizontal As sXY_int) As Boolean

        Return PosIsWithinTileArea(Horizontal, New sXY_int(0, 0), Terrain.TileSize)
    End Function

    Public Function TileNumClampToMap(ByVal TileNum As sXY_int) As sXY_int
        Dim Result As sXY_int

        Result.X = Clamp_int(TileNum.X, 0, Terrain.TileSize.X - 1)
        Result.Y = Clamp_int(TileNum.Y, 0, Terrain.TileSize.Y - 1)

        Return Result
    End Function

    Public Sub MakeDefaultUnitGroups()
        Dim A As Integer
        Dim NewGroup As clsMap.clsUnitGroup

        UnitGroups.Clear()
        For A = 0 To PlayerCountMax - 1
            NewGroup = New clsUnitGroup
            NewGroup.WZ_StartPos = A
            NewGroup.MapLink.Connect(UnitGroups)
        Next
        ScavengerUnitGroup = New clsUnitGroup
        ScavengerUnitGroup.MapLink.Connect(UnitGroups)
        ScavengerUnitGroup.WZ_StartPos = -1
    End Sub

    Public Function GetUnitGroupColour(ByVal ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup.WZ_StartPos < 0 Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).Colour
        End If
    End Function

    Public Function GetUnitGroupMinimapColour(ByVal ColourUnitGroup As clsUnitGroup) As sRGB_sng

        If ColourUnitGroup.WZ_StartPos < 0 Then
            Return New sRGB_sng(1.0F, 1.0F, 1.0F)
        Else
            Return PlayerColour(ColourUnitGroup.WZ_StartPos).MinimapColour
        End If
    End Function

    Public CompileScreen As frmCompile

    Public Sub CancelUserInput()

        If Not _ReadyForUserInput Then
            Exit Sub
        End If

        _ReadyForUserInput = False

        Dim X As Integer
        Dim Y As Integer

        If CompileScreen IsNot Nothing Then
            CompileScreen.Close()
            CompileScreen = Nothing
        End If

        SectorAll_GLLists_Delete()
        Minimap_GLDelete()

        ShadowSectors = Nothing
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y).Deallocate()
            Next
        Next
        Sectors = Nothing
        SectorGraphicsChanges.Deallocate()
        SectorGraphicsChanges = Nothing
        SectorUnitHeightsChanges.Deallocate()
        SectorUnitHeightsChanges = Nothing
        SectorTerrainUndoChanges.Deallocate()
        SectorTerrainUndoChanges = Nothing
        AutoTextureChanges.Deallocate()
        AutoTextureChanges = Nothing
        TerrainInterpretChanges.Deallocate()
        TerrainInterpretChanges = Nothing

        UnitChanges = Nothing
        GatewayChanges = Nothing
        Undos = Nothing

        SelectedUnits.Deallocate()
        SelectedUnits = Nothing

        Selected_Tile_A = Nothing
        Selected_Tile_B = Nothing
        Selected_Area_VertexA = Nothing
        Selected_Area_VertexB = Nothing
        Unit_Selected_Area_VertexA = Nothing

        ViewInfo = Nothing

        _SelectedUnitGroup = Nothing

        Messages = Nothing
    End Sub

    Public Sub InitializeUserInput()

        If _ReadyForUserInput Then
            Exit Sub
        End If

        _ReadyForUserInput = True

        Dim X As Integer
        Dim Y As Integer
        Dim A As Integer

        SectorCount.X = CInt(Math.Ceiling(Terrain.TileSize.X / SectorTileSize))
        SectorCount.Y = CInt(Math.Ceiling(Terrain.TileSize.Y / SectorTileSize))
        ReDim Sectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                Sectors(X, Y) = New clsSector(New sXY_int(X, Y))
            Next
        Next

        For A = 0 To Units.ItemCount - 1
            UnitSectorsCalc(Units.Item(A))
        Next

        ReDim ShadowSectors(SectorCount.X - 1, SectorCount.Y - 1)
        For Y = 0 To SectorCount.Y - 1
            For X = 0 To SectorCount.X - 1
                ShadowSector_Create(New sXY_int(X, Y))
            Next
        Next

        SectorGraphicsChanges = New clsSectorChanges(Me)
        SectorGraphicsChanges.SetAllChanged()
        SectorUnitHeightsChanges = New clsSectorChanges(Me)
        SectorTerrainUndoChanges = New clsSectorChanges(Me)
        AutoTextureChanges = New clsAutoTextureChanges(Me)
        TerrainInterpretChanges = New clsMap.clsTerrainUpdate(Terrain.TileSize)

        UnitChanges = New SimpleClassList(Of clsMap.clsUnitChange)
        UnitChanges.MaintainOrder = True
        GatewayChanges = New SimpleClassList(Of clsMap.clsGatewayChange)
        GatewayChanges.MaintainOrder = True
        Undos = New SimpleClassList(Of clsMap.clsUndo)
        Undos.MaintainOrder = True
        UndoPosition = 0

        SelectedUnits = New ConnectedList(Of clsMap.clsUnit, clsMap)(Me)

        If InterfaceOptions Is Nothing Then
            InterfaceOptions = New clsInterfaceOptions
        End If

        ViewInfo = New clsViewInfo(Me, frmMainInstance.MapView)

        _SelectedUnitGroup = New clsMap.clsUnitGroupContainer
        SelectedUnitGroup.Item = ScavengerUnitGroup

        Messages = New SimpleClassList(Of clsMessage)
        Messages.MaintainOrder = True
    End Sub

    Public Function GetDirectory() As String

        If PathInfo Is Nothing Then
            Return My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            Dim SplitPath As New sSplitPath(PathInfo.Path)
            Return SplitPath.FilePath
        End If
    End Function

    Public Class clsUpdateSectorGraphics
        Inherits clsMap.clsAction

        Public Overrides Sub ActionPerform()

            Map.Sector_GLList_Make(PosNum.X, PosNum.Y)
            Map.MinimapMakeLater()
        End Sub
    End Class

    Public Sub Update()
        Dim PrevSuppress As Boolean = SuppressMinimap

        SuppressMinimap = True
        UpdateAutoTextures()
        TerrainInterpretUpdate()
        SectorsUpdateGraphics()
        SectorsUpdateUnitHeights()
        SuppressMinimap = PrevSuppress
    End Sub

    Public Sub SectorsUpdateUnitHeights()
        Dim UpdateSectorUnitHeights As New clsUpdateSectorUnitHeights
        UpdateSectorUnitHeights.Map = Me

        UpdateSectorUnitHeights.Start()
        SectorUnitHeightsChanges.PerformTool(UpdateSectorUnitHeights)
        UpdateSectorUnitHeights.Finish()
        SectorUnitHeightsChanges.Clear()
    End Sub

    Public Sub SectorsUpdateGraphics()
        Dim UpdateSectorGraphics As New clsUpdateSectorGraphics
        UpdateSectorGraphics.Map = Me

        If MainMap Is Me Then
            SectorGraphicsChanges.PerformTool(UpdateSectorGraphics)
        End If
        SectorGraphicsChanges.Clear()
    End Sub

    Public Sub UpdateAutoTextures()
        Dim UpdateAutotextures As New clsUpdateAutotexture
        UpdateAutotextures.Map = Me
        UpdateAutotextures.MakeInvalidTiles = frmMainInstance.cbxInvalidTiles.Checked

        AutoTextureChanges.PerformTool(UpdateAutotextures)
        AutoTextureChanges.Clear()
    End Sub

    Public Sub TerrainInterpretUpdate()
        Dim ApplyVertexInterpret As New clsMap.clsApplyVertexTerrainInterpret
        Dim ApplyTileInterpret As New clsMap.clsApplyTileTerrainInterpret
        Dim ApplySideHInterpret As New clsMap.clsApplySideHTerrainInterpret
        Dim ApplySideVInterpret As New clsMap.clsApplySideVTerrainInterpret
        ApplyVertexInterpret.Map = Me
        ApplyTileInterpret.Map = Me
        ApplySideHInterpret.Map = Me
        ApplySideVInterpret.Map = Me

        TerrainInterpretChanges.Vertices.PerformTool(ApplyVertexInterpret)
        TerrainInterpretChanges.Tiles.PerformTool(ApplyTileInterpret)
        TerrainInterpretChanges.SidesH.PerformTool(ApplySideHInterpret)
        TerrainInterpretChanges.SidesV.PerformTool(ApplySideVInterpret)
        TerrainInterpretChanges.ClearAll()
    End Sub

    Public Class clsUpdateSectorUnitHeights
        Inherits clsMap.clsAction

        Private NewUnit As clsUnit
        Private ID As UInteger
        Private tmpUnit As clsUnit
        Private OldUnits() As clsUnit
        Private OldUnitCount As Integer = 0
        Private NewAltitude As Integer
        Private Started As Boolean
        Private tmpSector As clsMap.clsSector

        Public Sub Start()

            ReDim OldUnits(Map.Units.ItemCount - 1)

            Started = True
        End Sub

        Public Sub Finish()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer
            Dim UnitAdd As New clsMap.clsUnitAdd

            UnitAdd.Map = Map
            UnitAdd.StoreChange = True

            For A = 0 To OldUnitCount - 1
                tmpUnit = OldUnits(A)
                NewAltitude = CInt(Map.GetTerrainHeight(tmpUnit.Pos.Horizontal))
                If NewAltitude <> tmpUnit.Pos.Altitude Then
                    NewUnit = New clsUnit(tmpUnit, Map)
                    ID = tmpUnit.ID
                    'NewUnit.Pos.Altitude = NewAltitude
                    'these create changed sectors and must be done before drawing the new sectors
                    Map.Unit_Remove_StoreChange(tmpUnit.MapLink.ArrayPosition)
                    UnitAdd.NewUnit = NewUnit
                    UnitAdd.ID = ID
                    UnitAdd.Perform()
                    ErrorIDChange(ID, NewUnit, "UpdateSectorUnitHeights")
                End If
            Next

            Started = False
        End Sub

        Public Overrides Sub ActionPerform()

            If Not Started Then
                Stop
                Exit Sub
            End If

            Dim A As Integer
            Dim B As Integer

            tmpSector = Map.Sectors(PosNum.X, PosNum.Y)
            For A = 0 To tmpSector.Units.ItemCount - 1
                tmpUnit = tmpSector.Units.Item(A).Unit
                'units can be in multiple sectors, so dont include multiple times
                For B = 0 To OldUnitCount - 1
                    If OldUnits(B) Is tmpUnit Then
                        Exit For
                    End If
                Next
                If B = OldUnitCount Then
                    OldUnits(OldUnitCount) = tmpUnit
                    OldUnitCount += 1
                End If
            Next
        End Sub
    End Class

    Public Class clsUpdateAutotexture
        Inherits clsMap.clsAction

        Public MakeInvalidTiles As Boolean

        Private Terrain_Inner As clsPainter.clsTerrain
        Private Terrain_Outer As clsPainter.clsTerrain
        Private Road As clsPainter.clsRoad
        Private A As Integer
        Private Brush_Num As Integer
        Private RoadTop As Boolean
        Private RoadLeft As Boolean
        Private RoadRight As Boolean
        Private RoadBottom As Boolean
        Private Painter As clsPainter
        Private Terrain As clsTerrain
        Private ResultTiles As clsPainter.clsTileList
        Private ResultDirection As sTileDirection
        Private ResultTexture As clsPainter.clsTileList.sTileOrientationChance

        Public Overrides Sub ActionPerform()

            Terrain = Map.Terrain

            Painter = Map.Painter

            ResultTiles = Nothing
            ResultDirection = TileDirection_None

            'apply centre brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For Me.Brush_Num = 0 To Painter.TerrainCount - 1
                    Terrain_Inner = Painter.Terrains(Brush_Num)
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    ResultTiles = Terrain_Inner.Tiles
                                    ResultDirection = TileDirection_None
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'apply transition brushes
            If Not Terrain.Tiles(PosNum.X, PosNum.Y).Terrain_IsCliff Then
                For Me.Brush_Num = 0 To Painter.TransitionBrushCount - 1
                    Terrain_Inner = Painter.TransitionBrushes(Brush_Num).Terrain_Inner
                    Terrain_Outer = Painter.TransitionBrushes(Brush_Num).Terrain_Outer
                    If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i i i
                                    'nothing to do here
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i i o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i i o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Bottom
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Right
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'i o o i
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'i o o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        End If
                    ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i i o
                                    ResultTiles = Nothing
                                    ResultDirection = TileDirection_None
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o i o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Left
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o i o o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o i i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = TileDirection_Top
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o i o
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                                    'o o o i
                                    ResultTiles = Painter.TransitionBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                                    'o o o o
                                    'nothing to do here
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'set cliff tiles
            If Terrain.Tiles(PosNum.X, PosNum.Y).Tri Then
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopLeftIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopLeft
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then
                                A = 0
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomRight
                                    Exit For
                                End If
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomRightIsCliff Then
                    For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomRight
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopLeft
                                Exit For
                            End If
                        End If
                    Next
                    If Brush_Num = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            Else
                'default tri orientation
                If Terrain.Tiles(PosNum.X, PosNum.Y).TriTopRightIsCliff Then
                    If Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain_Inner Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 3 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                    ResultDirection = Terrain.Tiles(PosNum.X, PosNum.Y).DownSide
                                    Exit For
                                End If
                            End If
                            If ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Bottom
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Left
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer) And (Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Top
                                Exit For
                            ElseIf ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner And Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Or Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Or ((Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Or Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner) And (Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer And Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer)) Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Straight
                                ResultDirection = TileDirection_Right
                                Exit For
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    Else
                        For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                            Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                            Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                    ResultDirection = TileDirection_TopRight
                                    Exit For
                                End If
                            ElseIf Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then
                                A = 0
                                If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                                If A >= 2 Then
                                    ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                    ResultDirection = TileDirection_BottomLeft
                                    Exit For
                                End If
                            End If
                        Next
                        If Brush_Num = Painter.CliffBrushCount Then
                            ResultTiles = Nothing
                            ResultDirection = TileDirection_None
                        End If
                    End If
                ElseIf Terrain.Tiles(PosNum.X, PosNum.Y).TriBottomLeftIsCliff Then
                    For Me.Brush_Num = 0 To Painter.CliffBrushCount - 1
                        Terrain_Inner = Painter.CliffBrushes(Brush_Num).Terrain_Inner
                        Terrain_Outer = Painter.CliffBrushes(Brush_Num).Terrain_Outer
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Inner Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Inner Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_In
                                ResultDirection = TileDirection_BottomLeft
                                Exit For
                            End If
                        ElseIf Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Inner Then
                            A = 0
                            If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then A += 1
                            If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then A += 1
                            If A >= 2 Then
                                ResultTiles = Painter.CliffBrushes(Brush_Num).Tiles_Corner_Out
                                ResultDirection = TileDirection_TopRight
                                Exit For
                            End If
                        End If
                    Next
                    If Brush_Num = Painter.CliffBrushCount Then
                        ResultTiles = Nothing
                        ResultDirection = TileDirection_None
                    End If
                Else
                    'no cliff
                End If
            End If

            'apply roads
            Road = Nothing
            If Terrain.SideH(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y).Road
            ElseIf Terrain.SideH(PosNum.X, PosNum.Y + 1).Road IsNot Nothing Then
                Road = Terrain.SideH(PosNum.X, PosNum.Y + 1).Road
            ElseIf Terrain.SideV(PosNum.X + 1, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X + 1, PosNum.Y).Road
            ElseIf Terrain.SideV(PosNum.X, PosNum.Y).Road IsNot Nothing Then
                Road = Terrain.SideV(PosNum.X, PosNum.Y).Road
            End If
            If Road IsNot Nothing Then
                For Me.Brush_Num = 0 To Painter.RoadBrushCount - 1
                    If Painter.RoadBrushes(Brush_Num).Road Is Road Then
                        Terrain_Outer = Painter.RoadBrushes(Brush_Num).Terrain
                        A = 0
                        If Terrain.Vertices(PosNum.X, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If Terrain.Vertices(PosNum.X + 1, PosNum.Y + 1).Terrain Is Terrain_Outer Then
                            A += 1
                        End If
                        If A >= 2 Then Exit For
                    End If
                Next

                ResultTiles = Nothing
                ResultDirection = TileDirection_None

                If Brush_Num < Painter.RoadBrushCount Then
                    RoadTop = (Terrain.SideH(PosNum.X, PosNum.Y).Road Is Road)
                    RoadLeft = (Terrain.SideV(PosNum.X, PosNum.Y).Road Is Road)
                    RoadRight = (Terrain.SideV(PosNum.X + 1, PosNum.Y).Road Is Road)
                    RoadBottom = (Terrain.SideH(PosNum.X, PosNum.Y + 1).Road Is Road)
                    'do cross intersection
                    If RoadTop And RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_CrossIntersection
                        ResultDirection = TileDirection_None
                        'do T intersection
                    ElseIf RoadTop And RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Top
                    ElseIf RoadTop And RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Left
                    ElseIf RoadTop And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Right
                    ElseIf RoadLeft And RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_TIntersection
                        ResultDirection = TileDirection_Bottom
                        'do straight
                    ElseIf RoadTop And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Top
                        Else
                            ResultDirection = TileDirection_Bottom
                        End If
                    ElseIf RoadLeft And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Straight
                        If Rnd() >= 0.5F Then
                            ResultDirection = TileDirection_Left
                        Else
                            ResultDirection = TileDirection_Right
                        End If
                        'do corner
                    ElseIf RoadTop And RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_TopLeft
                    ElseIf RoadTop And RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_TopRight
                    ElseIf RoadLeft And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_BottomLeft
                    ElseIf RoadRight And RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_Corner_In
                        ResultDirection = TileDirection_BottomRight
                        'do end
                    ElseIf RoadTop Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Top
                    ElseIf RoadLeft Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Left
                    ElseIf RoadRight Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Right
                    ElseIf RoadBottom Then
                        ResultTiles = Painter.RoadBrushes(Brush_Num).Tile_End
                        ResultDirection = TileDirection_Bottom
                    End If
                End If
            End If

            If ResultTiles Is Nothing Then
                ResultTexture.TextureNum = -1
                ResultTexture.Direction = TileDirection_None
            Else
                ResultTexture = ResultTiles.GetRandom()
            End If
            If ResultTexture.TextureNum < 0 Then
                If MakeInvalidTiles Then
                    Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
                End If
            Else
                Terrain.Tiles(PosNum.X, PosNum.Y).Texture = OrientateTile(ResultTexture, ResultDirection)
            End If

            Map.SectorGraphicsChanges.TileChanged(PosNum)
            Map.SectorTerrainUndoChanges.TileChanged(PosNum)
        End Sub
    End Class

    Public Sub TileNeedsInterpreting(ByVal Pos As sXY_int)

        TerrainInterpretChanges.Tiles.Changed(Pos)
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.Vertices.Changed(New sXY_int(Pos.X + 1, Pos.Y + 1))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesH.Changed(New sXY_int(Pos.X, Pos.Y + 1))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X, Pos.Y))
        TerrainInterpretChanges.SidesV.Changed(New sXY_int(Pos.X + 1, Pos.Y))
    End Sub

    Public Sub TileTextureChangeTerrainAction(ByVal Pos As sXY_int, ByVal Action As enumTextureTerrainAction)

        Select Case Action
            Case enumTextureTerrainAction.Ignore

            Case enumTextureTerrainAction.Reinterpret
                TileNeedsInterpreting(Pos)
            Case enumTextureTerrainAction.Remove
                Terrain.Vertices(Pos.X, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y).Terrain = Nothing
                Terrain.Vertices(Pos.X, Pos.Y + 1).Terrain = Nothing
                Terrain.Vertices(Pos.X + 1, Pos.Y + 1).Terrain = Nothing
        End Select
    End Sub

    Public Class clsInterfaceOptions
        Public CompileName As String
        Public CompileMultiPlayers As String
        Public CompileMultiXPlayers As Boolean
        Public CompileMultiAuthor As String
        Public CompileMultiLicense As String
        Public AutoScrollLimits As Boolean
        Public ScrollMin As sXY_int
        Public ScrollMax As sXY_uint
        Public CampaignGameType As Integer

        Public Sub New()

            'set to default
            CompileName = ""
            CompileMultiPlayers = InvariantToString_int(2)
            CompileMultiXPlayers = False
            CompileMultiAuthor = ""
            CompileMultiLicense = ""
            AutoScrollLimits = True
            ScrollMin.X = 0
            ScrollMin.Y = 0
            ScrollMax.X = 0UI
            ScrollMax.Y = 0UI
            CampaignGameType = -1
        End Sub
    End Class
    Public InterfaceOptions As clsInterfaceOptions

    Public Function GetTitle() As String
        Dim ReturnResult As String

        If PathInfo Is Nothing Then
            ReturnResult = "Unsaved map"
        Else
            Dim SplitPath As New sSplitPath(PathInfo.Path)
            If PathInfo.IsFMap Then
                ReturnResult = SplitPath.FileTitleWithoutExtension
            Else
                ReturnResult = SplitPath.FileTitle
            End If
        End If
        Return ReturnResult
    End Function

    Public Sub SetChanged()

        ChangedSinceSave = True
        RaiseEvent Changed()

        AutoSave.ChangeCount += 1
        AutoSave_Test()
    End Sub

    Public MapView_TabPage As TabPage

    Public Sub SetTabText()
        Const MaxLength As Integer = 24

        Dim Result As String
        Result = GetTitle()
        If Result.Length > MaxLength Then
            Result = Strings.Left(Result, MaxLength - 3) & "..."
        End If
        MapView_TabPage.Text = Result
    End Sub

    Public Function SideHIsCliffOnBothSides(ByVal SideNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If SideNum.Y > 0 Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y - 1
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomRightIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomLeftIsCliff Then
                    Return False
                End If
            End If
        End If

        If SideNum.Y < Terrain.TileSize.Y Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopLeftIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopRightIsCliff Then
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Public Function SideVIsCliffOnBothSides(ByVal SideNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If SideNum.X > 0 Then
            TileNum.X = SideNum.X - 1
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomRightIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopRightIsCliff Then
                    Return False
                End If
            End If
        End If

        If SideNum.X < Terrain.TileSize.X Then
            TileNum.X = SideNum.X
            TileNum.Y = SideNum.Y
            If Terrain.Tiles(TileNum.X, TileNum.Y).Tri Then
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriTopLeftIsCliff Then
                    Return False
                End If
            Else
                If Not Terrain.Tiles(TileNum.X, TileNum.Y).TriBottomLeftIsCliff Then
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Public Function VertexIsCliffEdge(ByVal VertexNum As sXY_int) As Boolean
        Dim TileNum As sXY_int

        If VertexNum.X > 0 Then
            If VertexNum.Y > 0 Then
                TileNum.X = VertexNum.X - 1
                TileNum.Y = VertexNum.Y - 1
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
            If VertexNum.Y < Terrain.TileSize.Y Then
                TileNum.X = VertexNum.X - 1
                TileNum.Y = VertexNum.Y
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
        End If
        If VertexNum.X < Terrain.TileSize.X Then
            If VertexNum.Y > 0 Then
                TileNum.X = VertexNum.X
                TileNum.Y = VertexNum.Y - 1
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
            If VertexNum.Y < Terrain.TileSize.Y Then
                TileNum.X = VertexNum.X
                TileNum.Y = VertexNum.Y
                If Terrain.Tiles(TileNum.X, TileNum.Y).Terrain_IsCliff Then
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Public Sub SelectedUnitsAction(ByVal Tool As clsObjectAction)
        Dim SelectAction As New clsObjectSelect

        SelectedUnits.GetItemsAsSimpleClassList.PerformTool(Tool)
        SelectedUnits.Clear()
        Tool.ResultUnits.PerformTool(SelectAction)
    End Sub

    Public Function CheckMessages() As Boolean
        Dim A As Integer
        Dim DateNow As Date = Now
        Dim Changed As Boolean = False

        A = 0
        Do While A < Messages.ItemCount
            If DateDiff(DateInterval.Second, Messages.Item(A).CreatedDate, DateNow) >= 6L Then
                Messages.Remove(A)
                Changed = True
            Else
                A += 1
            End If
        Loop
        Return Changed
    End Function

    Public ReadOnly Property MainMap As clsMap
        Get
            If Not frmMainLink.IsConnected Then
                Return Nothing
            Else
                Return frmMainLink.Source.MainMap
            End If
        End Get
    End Property
End Class