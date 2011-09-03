Public Class clsGeneratorMap
    Inherits clsMap

    Public TilePathMap As PathfinderNetwork
    Public VertexPathMap As PathfinderNetwork

    Public GenerateTileset As clsGeneratorTileset

#If Mono267 = 0.0# Then
    Public Structure sGenerateTerrainVertex
        Public Node As PathfinderNode
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
        Public TopLeftLink As PathfinderConnection
    End Structure
#Else
     Public Class sGenerateTerrainVertex
        Public Node As PathfinderNode
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
        Public TopLeftLink As PathfinderConnection
    End Class
#End If
    Public GenerateTerrainVertex(,) As sGenerateTerrainVertex

#If Mono267 = 0.0# Then
    Public Structure sGenerateTerrainTile
        Public Node As PathfinderNode
        Public TopLeftLink As PathfinderConnection
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
    End Structure
#Else
    Public Class sGenerateTerrainTile
        Public Node As PathfinderNode
        Public TopLeftLink As PathfinderConnection
        Public TopLink As PathfinderConnection
        Public TopRightLink As PathfinderConnection
        Public RightLink As PathfinderConnection
        Public BottomRightLink As PathfinderConnection
        Public BottomLink As PathfinderConnection
        Public BottomLeftLink As PathfinderConnection
        Public LeftLink As PathfinderConnection
    End Class
#End If
    Public GenerateTerrainTiles(-1, -1) As sGenerateTerrainTile

    Public LevelCount As Integer

    Private Connections() As clsConnection
    Private ConnectionCount As Integer
    Private PassageNodes(,) As clsPassageNode
    Private PassageNodeCount As Integer
    Private PassageNodeDists(,,,) As Single
    Private Nearests() As clsNearest
    Private NearestCount As Integer
    Private BasePassageNodes() As clsPassageNode
    Private _PlayerCount As Integer
    Private _SymmetryBlockCount As Integer
    Public ReadOnly Property PlayerCount As Integer
        Get
            Return _PlayerCount
        End Get
    End Property

    Public Structure sGenerateLayoutArgs
        Public Size As sXY_int
        Public SymmetryIsRotational As Boolean
        Public Structure sSymmetryBlock
            Public XYNum As sXY_int
            Public Orientation As sTileOrientation
            Public ReflectToNum() As Integer
        End Structure
        Public SymmetryBlocks() As sSymmetryBlock
        Public SymmetryBlockCountXY As sXY_int
        Public SymmetryBlockCount As Integer
        Public LevelCount As Integer
        Public JitterScale As Integer
        Public MaxLevelTransition As Integer
        Public NodeScale As Single
        Public BaseLevel As Integer
        Public FlatBases As Boolean
        Public PlayerBasePos() As sXY_int
        Public PlayerCount As Integer
        Public PassageChance As Integer
        Public VariationChance As Integer
        Public FlatChance As Integer
        Public RandomChance As Integer
        Public EqualizeChance As Integer
        Public MaxDisconnectionDist As Single
        Public RampBase As Double
        Public BaseOilCount As Integer
        Public ExtraOilCount As Integer
        Public ExtraOilClusterSizeMin As Integer
        Public ExtraOilClusterSizeMax As Integer
        Public OilDispersion As Single
        Public OilAtATime As Integer
        Public WaterSpawnQuantity As Integer
        Public TotalWaterQuantity As Integer
    End Structure

    Public Class clsPassageNode
        Public Num As Integer = -1

        Public MirrorNum As Integer = -1

        Public Level As Integer = -1

        Public Pos As sXY_int

        Public IsOnBorder As Boolean

        Public OilCount As Integer

        Public HasFeatureCluster As Boolean

        Public IsWater As Boolean

        Public Structure sConnection
            Public Connection As clsConnection
            Public IsB As Boolean

            Public Function GetOther() As clsPassageNode
                If IsB Then
                    Return Connection.PassageNodeA
                Else
                    Return Connection.PassageNodeB
                End If
            End Function
        End Structure
        Public Connections() As sConnection
        Public ConnectionCount As Integer

        Public Sub Connection_Add(ByVal NewConnection As sConnection)

            If NewConnection.IsB Then
                NewConnection.Connection.PassageNodeB_ConnectionNum = ConnectionCount
            Else
                NewConnection.Connection.PassageNodeA_ConnectionNum = ConnectionCount
            End If

            ReDim Preserve Connections(ConnectionCount)
            Connections(ConnectionCount) = NewConnection
            ConnectionCount += 1
        End Sub

        Public Sub Connection_Remove(ByVal Num As Integer)

            If Connections(Num).IsB Then
                Connections(Num).Connection.PassageNodeB_ConnectionNum = -1
            Else
                Connections(Num).Connection.PassageNodeA_ConnectionNum = -1
            End If

            ConnectionCount -= 1
            If Num <> ConnectionCount Then
                Connections(Num) = Connections(ConnectionCount)
                If Connections(Num).IsB Then
                    Connections(Num).Connection.PassageNodeB_ConnectionNum = Num
                Else
                    Connections(Num).Connection.PassageNodeA_ConnectionNum = Num
                End If
            End If
        End Sub

        Public Function FindConnection(ByVal PassageNode As PathfinderNode) As clsConnection
            Dim A As Integer

            For A = 0 To ConnectionCount - 1
                If Connections(A).GetOther Is PassageNode Then
                    Return Connections(A).Connection
                End If
            Next
            Return Nothing
        End Function
    End Class

    Public Class clsConnection
        Public PassageNodeA As clsPassageNode
        Public PassageNodeA_ConnectionNum As Integer = -1
        Public PassageNodeB As clsPassageNode
        Public PassageNodeB_ConnectionNum As Integer = -1
        Public IsRamp As Boolean
        Public Reflections() As clsConnection
        Public ReflectionCount As Integer

        Public Sub New(ByVal NewPassageNodeA As clsPassageNode, ByVal NewPassageNodeB As clsPassageNode)
            Dim NewConnection As clsPassageNode.sConnection

            PassageNodeA = NewPassageNodeA
            NewConnection.Connection = Me
            NewConnection.IsB = False
            PassageNodeA.Connection_Add(NewConnection)

            PassageNodeB = NewPassageNodeB
            NewConnection.Connection = Me
            NewConnection.IsB = True
            PassageNodeB.Connection_Add(NewConnection)
        End Sub
    End Class

    Public Class clsNodeTag
        Public Pos As sXY_int
    End Class

    Public Function GetNodePosDist(ByVal NodeA As PathfinderNode, ByVal NodeB As PathfinderNode) As Single
        Dim TagA As clsNodeTag = CType(NodeA.Tag, clsNodeTag)
        Dim TagB As clsNodeTag = CType(NodeB.Tag, clsNodeTag)

        Return CSng(GetDist_XY_int(TagA.Pos, TagB.Pos))
    End Function

    Public Sub CalcNodePos(ByVal Node As PathfinderNode, ByRef Pos As sXY_dbl, ByRef SampleCount As Integer)

        If Node.GetLayer.GetNetwork_LayerNum = 0 Then
            Dim NodeTag As clsNodeTag
            NodeTag = CType(Node.Tag, clsNodeTag)
            Pos.X += NodeTag.Pos.X
            Pos.Y += NodeTag.Pos.Y
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                CalcNodePos(Node.GetChildNode(A), Pos, SampleCount)
            Next
            SampleCount += Node.GetChildNodeCount
        End If
    End Sub

    Public Function GenerateLayout(ByVal Args As sGenerateLayoutArgs) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim X As Integer
        Dim Y As Integer

        _PlayerCount = Args.PlayerCount * Args.SymmetryBlockCount
        Terrain.TileSize = Args.Size
        _SymmetryBlockCount = Args.SymmetryBlockCount

        Dim SymmetrySize As sXY_dbl

        SymmetrySize.X = Terrain.TileSize.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X
        SymmetrySize.Y = Terrain.TileSize.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y

        'ReDim Terrain.Tiles(Terrain.Size.X - 1, Terrain.Size.Y - 1)
        'ReDim Terrain.Vertices(Terrain.Size.X, Terrain.Size.Y)
        Terrain_Blank(Terrain.TileSize)
        ReDim GenerateTerrainTiles(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1)
        ReDim GenerateTerrainVertex(Terrain.TileSize.X, Terrain.TileSize.Y)

        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim NodeTag As clsNodeTag

        VertexPathMap = New PathfinderNetwork

        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                GenerateTerrainVertex(X, Y) = New sGenerateTerrainVertex
                GenerateTerrainVertex(X, Y).Node = New PathfinderNode(VertexPathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int(X * 128, Y * 128)
                GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                tmpNodeA = GenerateTerrainVertex(X, Y).Node
                If X > 0 Then
                    tmpNodeB = GenerateTerrainVertex(X - 1, Y).Node
                    GenerateTerrainVertex(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y > 0 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainVertex(X - 1, Y - 1).Node
                        GenerateTerrainVertex(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainVertex(X, Y - 1).Node
                    GenerateTerrainVertex(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Terrain.TileSize.X Then
                        tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
                        GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < Terrain.TileSize.X Then
                    tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
                    GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < Terrain.TileSize.Y Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
                        GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
                    GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Terrain.TileSize.X Then
                        tmpNodeB = GenerateTerrainVertex(X + 1, Y + 1).Node
                        GenerateTerrainVertex(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
            Next
        Next

        VertexPathMap.LargeArraysResize()
        VertexPathMap.FindCalc()

        TilePathMap = New PathfinderNetwork

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                GenerateTerrainTiles(X, Y) = New sGenerateTerrainTile
                GenerateTerrainTiles(X, Y).Node = New PathfinderNode(TilePathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int(CInt((X + 0.5#) * 128.0#), CInt((Y + 0.5#) * 128.0#))
                GenerateTerrainTiles(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                tmpNodeA = GenerateTerrainTiles(X, Y).Node
                If X > 0 Then
                    tmpNodeB = GenerateTerrainTiles(X - 1, Y).Node
                    GenerateTerrainTiles(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y > 0 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainTiles(X - 1, Y - 1).Node
                        GenerateTerrainTiles(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainTiles(X, Y - 1).Node
                    GenerateTerrainTiles(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Terrain.TileSize.X - 1 Then
                        tmpNodeB = GenerateTerrainTiles(X + 1, Y - 1).Node
                        GenerateTerrainTiles(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < Terrain.TileSize.X - 1 Then
                    tmpNodeB = GenerateTerrainTiles(X + 1, Y).Node
                    GenerateTerrainTiles(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < Terrain.TileSize.Y - 1 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainTiles(X - 1, Y + 1).Node
                        GenerateTerrainTiles(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainTiles(X, Y + 1).Node
                    GenerateTerrainTiles(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < Terrain.TileSize.X - 1 Then
                        tmpNodeB = GenerateTerrainTiles(X + 1, Y + 1).Node
                        GenerateTerrainTiles(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
            Next
        Next

        TilePathMap.LargeArraysResize()
        TilePathMap.FindCalc()

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim E As Integer
        Dim F As Integer
        Dim G As Integer
        Dim H As Integer
        Dim BaseLayer As PathfinderLayer = VertexPathMap.GetNodeLayer(0)
        Dim JitterLayer As PathfinderLayer = VertexPathMap.GetNodeLayer(Args.JitterScale)
        A = JitterLayer.GetNodeCount - 1
        Dim NodeLevel(A) As Integer
        Dim BaseNodeLevel As sBaseNodeLevels
        ReDim BaseNodeLevel.NodeLevels(BaseLayer.GetNodeCount - 1)

        'set position of jitter layer nodes

        Dim XY_dbl As sXY_dbl

        If A > 0 Then
            For B = 0 To A
                tmpNodeA = JitterLayer.GetNode(B)
                C = 0
                XY_dbl.X = 0.0#
                XY_dbl.Y = 0.0#
                CalcNodePos(tmpNodeA, XY_dbl, C)
                NodeTag = New clsNodeTag
                NodeTag.Pos.X = CInt(XY_dbl.X / C)
                NodeTag.Pos.Y = CInt(XY_dbl.Y / C)
                tmpNodeA.Tag = NodeTag
            Next
        End If

        'create passage nodes

        Dim PassageRadius As Integer = CInt(128.0F * Args.NodeScale)
        Dim MaxLikelyPassageNodeCount As Integer
        MaxLikelyPassageNodeCount = CInt(Math.Ceiling(2.0# * Args.Size.X * 128 * Args.Size.Y * 128 / (Math.PI * PassageRadius * PassageRadius)))

        ReDim PassageNodes(Args.SymmetryBlockCount - 1, MaxLikelyPassageNodeCount - 1)
        Dim LoopCount As Integer
        Dim EdgeOffset As Integer = 4 * 128
        Dim PointIsValid As Boolean
        Dim EdgeSections As sXY_int
        Dim EdgeSectionSize As sXY_dbl
        Dim NewPointPos As sXY_int

        If Args.SymmetryBlockCountXY.X = 1 Then
            EdgeSections.X = CInt(Int((Args.Size.X * TerrainGridSpacing - EdgeOffset * 2.0#) / (Args.NodeScale * TerrainGridSpacing * 2.0F)))
            EdgeSectionSize.X = (Args.Size.X * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.X
            EdgeSections.X -= 1
        Else
            EdgeSections.X = CInt(Int((Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#))
            EdgeSectionSize.X = (Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Int((Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If
        If Args.SymmetryBlockCountXY.Y = 1 Then
            EdgeSections.Y = CInt(Int((Args.Size.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / (Args.NodeScale * TerrainGridSpacing * 2.0F)))
            EdgeSectionSize.Y = (Args.Size.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.Y
            EdgeSections.Y -= 1
        Else
            EdgeSections.Y = CInt(Int((Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#))
            EdgeSectionSize.Y = (Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Int((Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If

        PassageNodeCount = 0
        For Y = 1 To EdgeSections.Y
            If Not MakePassageNodes(New sXY_int(EdgeOffset, EdgeOffset + CInt(Y * EdgeSectionSize.Y)), True, Args) Then
                ReturnResult.Problem = "Error: Bad border node."
                Return ReturnResult
            End If
            If Args.SymmetryBlockCountXY.X = 1 Then
                If Not MakePassageNodes(New sXY_int(Args.Size.X * TerrainGridSpacing - EdgeOffset, EdgeOffset + CInt(Y * EdgeSectionSize.Y)), True, Args) Then
                    ReturnResult.Problem = "Error: Bad border node."
                    Return ReturnResult
                End If
            End If
        Next
        For X = 1 To EdgeSections.X
            If Not MakePassageNodes(New sXY_int(EdgeOffset + CInt(X * EdgeSectionSize.X), EdgeOffset), True, Args) Then
                ReturnResult.Problem = "Error: Bad border node."
                Return ReturnResult
            End If
            If Args.SymmetryBlockCountXY.Y = 1 Then
                If Not MakePassageNodes(New sXY_int(EdgeOffset + CInt(X * EdgeSectionSize.X), Args.Size.Y * TerrainGridSpacing - EdgeOffset), True, Args) Then
                    ReturnResult.Problem = "Error: Bad border node."
                    Return ReturnResult
                End If
            End If
        Next
        Do
            LoopCount = 0
            Do
                PointIsValid = True
                If Args.SymmetryBlockCountXY.X = 1 Then
                    NewPointPos.X = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.X - EdgeOffset * 2.0# + 1.0#)))
                Else
                    NewPointPos.X = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.X - EdgeOffset + 1.0#)))
                End If
                If Args.SymmetryBlockCountXY.Y = 1 Then
                    NewPointPos.Y = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.Y - EdgeOffset * 2.0# + 1.0#)))
                Else
                    NewPointPos.Y = EdgeOffset + CInt(Int(Rnd() * (SymmetrySize.Y - EdgeOffset + 1.0#)))
                End If
                For A = 0 To PassageNodeCount - 1
                    For B = 0 To Args.SymmetryBlockCount - 1
                        If GetDist_XY_int(PassageNodes(B, A).Pos, NewPointPos) < PassageRadius * 2 Then
                            GoTo PointTooClose
                        End If
                    Next
                Next
PointTooClose:
                If A = PassageNodeCount Then
                    If MakePassageNodes(NewPointPos, False, Args) Then
                        Exit Do
                    End If
                End If
                LoopCount += 1
                If LoopCount >= Args.Size.X * Args.Size.Y / 4.0F Then
                    GoTo PointMakingFinished
                End If
            Loop
        Loop
PointMakingFinished:
        ReDim Preserve PassageNodes(Args.SymmetryBlockCount - 1, PassageNodeCount - 1)

        'connect until all are connected without intersecting

        Dim IntersectPos As sIntersectPos
        Dim MaxConDist2 As Integer = PassageRadius * 2 * 4
        MaxConDist2 *= MaxConDist2
        Dim NearestA As clsNearest
        ReDim Nearests(PassageNodeCount * 64 - 1)
        Dim tmpPassageNodeA As clsPassageNode
        Dim tmpPassageNodeB As clsPassageNode
        Dim XY_int As sXY_int
        Dim NearestArgs As sTestNearestArgs
        Dim MinConDist As Integer = 5 * 128

        NearestArgs.Args = Args
        NearestArgs.MaxConDist2 = MaxConDist2
        NearestArgs.MinConDist = MinConDist

        For A = 0 To PassageNodeCount - 1
            NearestArgs.tmpPassageNodeA = PassageNodes(0, A)
            For B = A To PassageNodeCount - 1
                For C = 0 To Args.SymmetryBlockCount - 1
                    NearestArgs.tmpPassageNodeB = PassageNodes(C, B)
                    If NearestArgs.tmpPassageNodeA IsNot NearestArgs.tmpPassageNodeB Then
                        TestNearest(NearestArgs)
                    End If
                Next
            Next
        Next

        Dim NearestB As clsNearest
        Dim Flag As Boolean

        For G = 0 To NearestCount - 1
            NearestA = Nearests(G)
            For A = 0 To NearestA.NodeCount - 1
                tmpPassageNodeA = NearestA.NodeA(A)
                tmpPassageNodeB = NearestA.NodeB(A)
                For H = 0 To NearestCount - 1
                    NearestB = Nearests(H)
                    If NearestB IsNot NearestA Then
                        If NearestB.Dist2 < NearestA.Dist2 Then
                            Flag = True
                        ElseIf NearestB.Dist2 = NearestA.Dist2 Then
                            Flag = (NearestA.Num > NearestB.Num)
                        Else
                            Flag = False
                        End If
                        If Flag Then
                            For B = 0 To NearestB.NodeCount - 1
                                IntersectPos = GetLinesIntersectBetween(tmpPassageNodeA.Pos, tmpPassageNodeB.Pos, NearestB.NodeA(B).Pos, NearestB.NodeB(B).Pos)
                                If IntersectPos.Exists Then
                                    Exit For
                                End If
                            Next
                            If B < NearestB.NodeCount Then
                                NearestA.BlockedCount += 1
                                With NearestB
                                    .BlockedNearests(.BlockedNearestCount) = NearestA
                                    .BlockedNearestCount += 1
                                End With
                            End If
                        End If
                    End If
                Next
            Next
        Next

        Dim ChangeCount As Integer
        'Dim ConnectionsA(PassageNodeCount * 16 - 1) As clsConnection
        ReDim Connections(PassageNodeCount * 16 - 1)
        'Dim ConnectionsB(PassageNodeCount * 16 - 1) As clsConnection
        'Dim ConnectionsC(PassageNodeCount * 16 - 1) As clsConnection
        'Dim ConnectionsD(PassageNodeCount * 16 - 1) As clsConnection
        'Dim ConnectionCount As Integer

        Dim AngleA As Double
        Dim AngleB As Double
        Dim AngleC As Double

        Do
            'create valid connections
            ChangeCount = 0
            G = 0
            Do While G < NearestCount
                NearestA = Nearests(G)
                'XY_int.X = NearestA.NodeA(0).Pos.X - NearestA.NodeB(0).Pos.X
                'XY_int.Y = NearestA.NodeA(0).Pos.Y - NearestA.NodeB(0).Pos.Y
                'AngleA = Get_Angle(XY_int)
                Flag = True
                'If NearestA.NodeA(0).IsOnBorder Xor NearestA.NodeB(0).IsOnBorder Then
                '    If Flag Then
                '        If NearestA.NodeA(0).IsOnBorder Then
                '            For A = 0 To NearestA.NodeA(0).ConnectionCount - 1
                '                If Not NearestA.NodeA(0).Connections(A).GetOther.IsOnBorder Then
                '                    Flag = False
                '                    Exit For
                '                End If
                '            Next
                '        End If
                '    End If
                '    If Flag Then
                '        If NearestA.NodeB(0).IsOnBorder Then
                '            For A = 0 To NearestA.NodeB(0).ConnectionCount - 1
                '                If Not NearestA.NodeB(0).Connections(A).GetOther.IsOnBorder Then
                '                    Flag = False
                '                    Exit For
                '                End If
                '            Next
                '        End If
                '    End If
                'End If
                'If Flag Then
                '    If NearestA.NodeA(0).IsOnBorder Then
                '        If NearestA.NodeA(0).ConnectionCount > 0 Then
                '            Flag = False
                '        End If
                '    End If
                'End If
                'If Flag Then
                '    If NearestA.NodeB(0).IsOnBorder Then
                '        If NearestA.NodeB(0).ConnectionCount > 0 Then
                '            Flag = False
                '        End If
                '    End If
                'End If
                'If Flag Then
                '    For D = 0 To NearestA.NodeA(0).ConnectionCount - 1
                '        XY_int.X = NearestA.NodeA(0).Connections(D).Connection.PassageNodeA.Pos.X - NearestA.NodeA(0).Connections(D).Connection.PassageNodeB.Pos.X
                '        XY_int.Y = NearestA.NodeA(0).Connections(D).Connection.PassageNodeA.Pos.Y - NearestA.NodeA(0).Connections(D).Connection.PassageNodeB.Pos.Y
                '        AngleB = Get_Angle(XY_int)
                '        AngleC = Angle_Clamp(AngleA - AngleB)
                '        If NearestA.NodeA(0).Connections(D).Connection.PassageNodeA IsNot NearestA.NodeA(0) Then
                '            AngleC = Angle_Clamp(AngleC - Pi)
                '        End If
                '        If Math.Abs(AngleC) < 30.0# * RadOf1Deg Then
                '            Exit For
                '        End If
                '    Next
                '    For E = 0 To NearestA.NodeB(0).ConnectionCount - 1
                '        XY_int.X = NearestA.NodeB(0).Connections(E).Connection.PassageNodeA.Pos.X - NearestA.NodeB(0).Connections(E).Connection.PassageNodeB.Pos.X
                '        XY_int.Y = NearestA.NodeB(0).Connections(E).Connection.PassageNodeA.Pos.Y - NearestA.NodeB(0).Connections(E).Connection.PassageNodeB.Pos.Y
                '        AngleB = Get_Angle(XY_int)
                '        AngleC = Angle_Clamp(AngleA - AngleB)
                '        If NearestA.NodeB(0).Connections(E).Connection.PassageNodeB IsNot NearestA.NodeB(0) Then
                '            AngleC = Angle_Clamp(AngleC - Pi)
                '        End If
                '        If Math.Abs(AngleC) < 30.0# * RadOf1Deg Then
                '            Exit For
                '        End If
                '    Next
                '    If D < NearestA.NodeA(0).ConnectionCount Or E < NearestA.NodeB(0).ConnectionCount Then
                '        NearestA.Invalid = True
                '        Flag = False
                '    End If
                'End If
                If NearestA.BlockedCount = 0 And Flag Then
                    F = ConnectionCount
                    For D = 0 To NearestA.NodeCount - 1
                        Connections(ConnectionCount) = New clsConnection(NearestA.NodeA(D), NearestA.NodeB(D))
                        ConnectionCount += 1
                    Next
                    For D = 0 To NearestA.NodeCount - 1
                        A = F + D
                        Connections(A).ReflectionCount = NearestA.NodeCount - 1
                        ReDim Connections(A).Reflections(Connections(A).ReflectionCount - 1)
                        B = 0
                        For E = 0 To NearestA.NodeCount - 1
                            If E <> D Then
                                Connections(A).Reflections(B) = Connections(F + E)
                                B += 1
                            End If
                        Next
                    Next
                    For C = 0 To NearestA.BlockedNearestCount - 1
                        NearestA.BlockedNearests(C).Invalid = True
                    Next
                    NearestCount -= 1
                    H = NearestA.Num
                    NearestA.Num = -1
                    If H <> NearestCount Then
                        Nearests(H) = Nearests(NearestCount)
                        Nearests(H).Num = H
                    End If
                    ChangeCount += 1
                Else
                    If Not Flag Then
                        NearestA.Invalid = True
                    End If
                    G += 1
                End If
            Loop
            'remove blocked ones and their blocking effect
            G = 0
            Do While G < NearestCount
                NearestA = Nearests(G)
                If NearestA.Invalid Then
                    NearestA.Num = -1
                    For D = 0 To NearestA.BlockedNearestCount - 1
                        NearestA.BlockedNearests(D).BlockedCount -= 1
                    Next
                    NearestCount -= 1
                    If G <> NearestCount Then
                        Nearests(G) = Nearests(NearestCount)
                        Nearests(G).Num = G
                    End If
                Else
                    G += 1
                End If
            Loop
        Loop While ChangeCount > 0

        'get nodes in random order

        Dim PassageNodeListOrder(PassageNodeCount - 1) As clsPassageNode
        Dim PassageNodeListOrderCount As Integer = 0
        Dim PassageNodeOrder(PassageNodeCount - 1) As clsPassageNode
        For A = 0 To PassageNodeCount - 1
            PassageNodeListOrder(PassageNodeListOrderCount) = PassageNodes(0, A)
            PassageNodeListOrderCount += 1
        Next
        B = 0
        Do While PassageNodeListOrderCount > 0
            A = CInt(Int(Rnd() * PassageNodeListOrderCount))
            PassageNodeOrder(B) = PassageNodeListOrder(A)
            B += 1
            PassageNodeListOrderCount -= 1
            PassageNodeListOrder(A) = PassageNodeListOrder(PassageNodeListOrderCount)
        Loop

        'designate height levels

        Dim LevelHeight As Single = 255.0F / (Args.LevelCount - 1)
        Dim LevelCounts(Args.LevelCount - 1) As Integer
        Dim BestNum As Integer
        Dim Eligables(Args.LevelCount - 1) As Integer
        Dim EligableCount As Integer
        Dim Changed As Boolean
        Dim MapLevelCount(Args.LevelCount - 1) As Integer
        Dim Dist As Single
        Dim BestDist As Single
        Dim PassageNodesMinLevel As sPassageNodeLevels
        ReDim PassageNodesMinLevel.Nodes(PassageNodeCount - 1)
        Dim PassageNodesMaxLevel As sPassageNodeLevels
        ReDim PassageNodesMaxLevel.Nodes(PassageNodeCount - 1)

        For A = 0 To PassageNodeCount - 1
            PassageNodesMinLevel.Nodes(A) = 0
            PassageNodesMaxLevel.Nodes(A) = Args.LevelCount - 1
        Next

        'create bases
        ReDim BasePassageNodes(_PlayerCount - 1)
        For B = 0 To Args.PlayerCount - 1
            BestDist = Single.MaxValue
            tmpPassageNodeB = Nothing
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodes(0, A)
                If Not tmpPassageNodeA.IsOnBorder Then
                    Dist = CSng(GetDist_XY_int(tmpPassageNodeA.Pos, Args.PlayerBasePos(B)))
                    If Dist < BestDist Then
                        BestDist = Dist
                        tmpPassageNodeB = tmpPassageNodeA
                    End If
                End If
            Next

            If Args.BaseLevel < 0 Then
                D = CInt(Int(Rnd() * Args.LevelCount))
            Else
                D = Args.BaseLevel
            End If

            PassageNodesMinLevelSet(tmpPassageNodeB, PassageNodesMinLevel, D, Args.MaxLevelTransition)
            PassageNodesMaxLevelSet(tmpPassageNodeB, PassageNodesMaxLevel, D, Args.MaxLevelTransition)
            MapLevelCount(D) += 1
            For A = 0 To Args.SymmetryBlockCount - 1
                E = A * Args.PlayerCount + B
                BasePassageNodes(E) = PassageNodes(A, tmpPassageNodeB.Num)
                BasePassageNodes(E).Level = D
            Next
            If Args.FlatBases Then
                For E = 0 To tmpPassageNodeB.ConnectionCount - 1
                    tmpPassageNodeA = tmpPassageNodeB.Connections(E).GetOther
                    If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder Then
                        PassageNodesMinLevelSet(tmpPassageNodeA, PassageNodesMinLevel, D, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeA, PassageNodesMaxLevel, D, Args.MaxLevelTransition)
                        For A = 0 To Args.SymmetryBlockCount - 1
                            PassageNodes(A, tmpPassageNodeA.Num).Level = D
                        Next
                    End If
                Next
            End If
        Next

        Dim WaterCount As Integer
        Dim ActionTotal As Integer = Args.FlatChance + Args.PassageChance + Args.VariationChance + Args.RandomChance + Args.EqualizeChance

        If ActionTotal <= 0 Then
            ReturnResult.Problem = "All height level behaviors are zero"
            Return ReturnResult
        End If

        Dim RandomAction As Integer
        Dim FlatCutoff As Integer = Args.FlatChance
        Dim PassageCutoff As Integer = Args.FlatChance + Args.PassageChance
        Dim VariationCutoff As Integer = Args.FlatChance + Args.PassageChance + Args.VariationChance
        Dim RandomCutoff As Integer = Args.FlatChance + Args.PassageChance + Args.VariationChance + Args.RandomChance
        Dim EqualizeCutoff As Integer = Args.FlatChance + Args.PassageChance + Args.VariationChance + Args.RandomChance + Args.EqualizeChance

        Dim Min As Integer
        Dim Max As Integer
        Dim CanDoFlatsAroundWater As Boolean
        Dim TotalWater As Integer
        Dim WaterSpawns As Integer

        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodeOrder(A)
            If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder Then
                WaterCount = 0
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If tmpPassageNodeB.IsWater Then
                        WaterCount += 1
                    End If
                Next
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    If PassageNodesMinLevel.Nodes(tmpPassageNodeA.Connections(B).GetOther.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If CanDoFlatsAroundWater And ((WaterCount = 0 And WaterSpawns < Args.WaterSpawnQuantity) Or (WaterCount = 1 And Args.TotalWaterQuantity - TotalWater > Args.WaterSpawnQuantity - WaterSpawns)) And PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) = 0 And TotalWater < Args.TotalWaterQuantity Then
                    If WaterCount = 0 Then
                        WaterSpawns += 1
                    End If
                    TotalWater += 1
                    C = tmpPassageNodeA.Num
                    For D = 0 To Args.SymmetryBlockCount - 1
                        PassageNodes(D, C).IsWater = True
                        PassageNodes(D, C).Level = 0
                    Next
                    PassageNodesMinLevelSet(tmpPassageNodeA, PassageNodesMinLevel, 0, Args.MaxLevelTransition)
                    PassageNodesMaxLevelSet(tmpPassageNodeA, PassageNodesMaxLevel, 0, Args.MaxLevelTransition)
                    MapLevelCount(0) += 1
                    For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                        tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                        PassageNodesMinLevelSet(tmpPassageNodeB, PassageNodesMinLevel, 0, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeB, PassageNodesMaxLevel, 0, Args.MaxLevelTransition)
                    Next
                    Changed = True
                End If
            End If
        Next

        Do
            Changed = False
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodeOrder(A)
                If tmpPassageNodeA.Level < 0 And Not tmpPassageNodeA.IsOnBorder Then
                    For B = 0 To Args.LevelCount - 1
                        LevelCounts(B) = 0
                    Next
                    WaterCount = 0
                    Flag = False
                    For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                        tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                        If tmpPassageNodeB.Level >= 0 Then
                            LevelCounts(tmpPassageNodeB.Level) += 1
                            Flag = True
                        End If
                        If tmpPassageNodeB.IsWater Then
                            WaterCount += 1
                        End If
                    Next
                    If WaterCount > 0 Then
                        C = tmpPassageNodeA.Num
                        For D = 0 To Args.SymmetryBlockCount - 1
                            PassageNodes(D, C).Level = 0
                        Next
                        PassageNodesMinLevelSet(tmpPassageNodeA, PassageNodesMinLevel, 0, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeA, PassageNodesMaxLevel, 0, Args.MaxLevelTransition)
                        MapLevelCount(0) += 1
                        Changed = True
                    ElseIf PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) > PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num) Then
                        F = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) - PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                        Math.DivRem(F, 2, D)
                        If D = 1 Then
                            E = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) + CInt((F - 1) / 2.0F) + CInt(Int(Rnd() * 2.0F))
                        Else
                            E = CInt(PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) + F / 2.0F)
                        End If
                        For D = 0 To Args.SymmetryBlockCount - 1
                            PassageNodes(D, tmpPassageNodeA.Num).Level = E
                        Next
                        PassageNodesMinLevelSet(tmpPassageNodeA, PassageNodesMinLevel, E, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeA, PassageNodesMaxLevel, E, Args.MaxLevelTransition)
                        Changed = True
                    ElseIf Flag Then
                        'Min = PassageNodesMinLevel(tmpPassageNodeA.Num) + 1
                        'Max = PassageNodesMaxLevel(tmpPassageNodeA.Num) - 1
                        Min = -1
                        For B = 0 To Args.LevelCount - 1
                            If LevelCounts(B) > 0 Then
                                Min = B
                                Exit For
                            End If
                        Next
                        Max = -1
                        For B = Args.LevelCount - 1 To 0 Step -1
                            If LevelCounts(B) > 0 Then
                                Max = B
                                Exit For
                            End If
                        Next
                        C = Max - Min
                        If C >= Args.MaxLevelTransition Then
                            'if there is a low ground and a high ground nearby, make a medium ground
                            Math.DivRem(C, 2, D)
                            If D = 1 Then
                                BestNum = Min + CInt((C - 1) / 2.0F) + CInt(Int(Rnd() * 2.0F))
                            Else
                                BestNum = Min + CInt(C / 2.0F)
                            End If
                        Else
                            RandomAction = CInt(Int(Rnd() * ActionTotal))
                            If RandomAction < FlatCutoff Then
                                'extend the level that surrounds this most
                                C = 0
                                EligableCount = 0
                                For B = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) To PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                                    If LevelCounts(B) > C Then
                                        C = LevelCounts(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf LevelCounts(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(CInt(Int(Rnd() * EligableCount)))
                            ElseIf RandomAction < PassageCutoff Then
                                'extend any level that surrounds only once, or closest to
                                EligableCount = 0
                                For B = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) To PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                                    If LevelCounts(B) = 1 Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                If EligableCount = 0 Then
                                    'cant form a passage, so make a new height level to increase passage forming
                                    'extend the most uncommon surrounding
                                    C = Integer.MaxValue
                                    EligableCount = 0
                                    For B = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) To PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                                        If LevelCounts(B) < C Then
                                            C = LevelCounts(B)
                                            Eligables(0) = B
                                            EligableCount = 1
                                        ElseIf LevelCounts(B) = C Then
                                            Eligables(EligableCount) = B
                                            EligableCount += 1
                                        End If
                                    Next
                                End If
                                BestNum = Eligables(CInt(Int(Rnd() * EligableCount)))
                            ElseIf RandomAction < VariationCutoff Then
                                'extend the most uncommon surrounding
                                C = Integer.MaxValue
                                EligableCount = 0
                                For B = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) To PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                                    If LevelCounts(B) < C Then
                                        C = LevelCounts(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf LevelCounts(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(CInt(Int(Rnd() * EligableCount)))
                            ElseIf RandomAction < RandomCutoff Then
                                BestNum = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) + CInt(Int(Rnd() * (PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num) - PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) + 1)))
                            ElseIf RandomAction < EqualizeCutoff Then
                                'switch to the most uncommon level on the map
                                C = Integer.MaxValue
                                EligableCount = 0
                                For B = PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) To PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num)
                                    If MapLevelCount(B) < C Then
                                        C = MapLevelCount(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf MapLevelCount(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(CInt(Int(Rnd() * EligableCount)))
                            Else
                                ReturnResult.Problem = "Error: Random number out of range."
                                Return ReturnResult
                            End If
                        End If
                        C = tmpPassageNodeA.Num
                        For D = 0 To Args.SymmetryBlockCount - 1
                            PassageNodes(D, C).Level = BestNum
                        Next
                        PassageNodesMinLevelSet(tmpPassageNodeA, PassageNodesMinLevel, BestNum, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeA, PassageNodesMaxLevel, BestNum, Args.MaxLevelTransition)
                        MapLevelCount(BestNum) += 1
                        Changed = True
                    End If
                End If
            Next
        Loop While Changed
        Dim tmpPassageNodeC As clsPassageNode
        'set edge points to the level of their neighbour
        For A = 0 To PassageNodeCount - 1
            tmpPassageNodeA = PassageNodes(0, A)
            If tmpPassageNodeA.IsOnBorder Then
                If tmpPassageNodeA.Level >= 0 Then
                    ReturnResult.Problem = "Error: Border has had its height set."
                    Return ReturnResult
                End If
                'If tmpPassageNodeA.ConnectionCount <> 1 Then
                '    ReturnResult.Problem = "Error: Border has incorrect connections."
                '    Exit Function
                'End If
                tmpPassageNodeC = Nothing
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If Not tmpPassageNodeB.IsOnBorder And PassageNodesMinLevel.Nodes(tmpPassageNodeA.Num) <= tmpPassageNodeB.Level And PassageNodesMaxLevel.Nodes(tmpPassageNodeA.Num) >= tmpPassageNodeB.Level Then
                        tmpPassageNodeC = tmpPassageNodeB
                    End If
                    If PassageNodesMinLevel.Nodes(tmpPassageNodeB.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If tmpPassageNodeC Is Nothing Then
                    ReturnResult.Problem = "Error: No connection for border node"
                    Return ReturnResult
                End If
                PassageNodesMinLevelSet(tmpPassageNodeC, PassageNodesMinLevel, tmpPassageNodeC.Level, Args.MaxLevelTransition)
                PassageNodesMaxLevelSet(tmpPassageNodeC, PassageNodesMaxLevel, tmpPassageNodeC.Level, Args.MaxLevelTransition)
                For D = 0 To Args.SymmetryBlockCount - 1
                    PassageNodes(D, A).IsWater = (tmpPassageNodeC.IsWater And CanDoFlatsAroundWater)
                    PassageNodes(D, A).Level = tmpPassageNodeC.Level
                Next
                If tmpPassageNodeA.IsWater Then
                    For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                        tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                        PassageNodesMinLevelSet(tmpPassageNodeB, PassageNodesMinLevel, tmpPassageNodeA.Level, Args.MaxLevelTransition)
                        PassageNodesMaxLevelSet(tmpPassageNodeB, PassageNodesMaxLevel, tmpPassageNodeA.Level, Args.MaxLevelTransition)
                    Next
                End If
            End If
        Next

        'set node heights

        Dim BestConnection As clsConnection
        Dim BestNode As clsPassageNode

        For A = 0 To JitterLayer.GetNodeCount - 1
            NodeTag = CType(JitterLayer.GetNode(A).Tag, clsNodeTag)
            NodeLevel(A) = -1
            BestDist = Single.MaxValue
            BestConnection = Nothing
            BestNode = Nothing
            For B = 0 To ConnectionCount - 1
                'If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                If Connections(B).PassageNodeA.Level = Connections(B).PassageNodeB.Level Then
                    'only do this if the levels are the same
                    'this is to make sure nodes that are connected are actually connected on the terrain
                    XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, NodeTag.Pos)
                    Dist = CSng(GetDist_XY_int(XY_int, NodeTag.Pos))
                    If Dist < BestDist Then
                        BestDist = Dist
                        If GetDist_XY_int(NodeTag.Pos, Connections(B).PassageNodeA.Pos) <= GetDist_XY_int(NodeTag.Pos, Connections(B).PassageNodeB.Pos) Then
                            BestNode = Connections(B).PassageNodeA
                        Else
                            BestNode = Connections(B).PassageNodeB
                        End If
                        Flag = True
                    End If
                End If
            Next
            For C = 0 To PassageNodeCount - 1
                'If Not PassageNodesA(C).IsOnBorder Then
                For D = 0 To Args.SymmetryBlockCount - 1
                    Dist = CSng(GetDist_XY_int(NodeTag.Pos, PassageNodes(D, C).Pos))
                    If Dist < BestDist Then
                        BestDist = Dist
                        BestNode = PassageNodes(D, C)
                        Flag = True
                    End If
                Next
                'End If
            Next
            If Flag Then
                NodeLevel(A) = BestNode.Level
            Else
                NodeLevel(A) = BestConnection.PassageNodeA.Level
            End If
            If NodeLevel(A) < 0 Then
                ReturnResult.Problem = "Error: Node height is not set."
                Return ReturnResult
            End If
        Next

        For A = 0 To Args.LevelCount - 1
            For B = 0 To JitterLayer.GetNodeCount - 1
                If NodeLevel(B) >= A Then
                    SetBaseLevel(JitterLayer.GetNode(B), A, BaseNodeLevel)
                End If
            Next
        Next

        'make ramps

        Dim PassageNodePathNodes(Args.SymmetryBlockCount - 1, PassageNodeCount - 1) As PathfinderNode
        Dim PassageNodePathMap As New PathfinderNetwork
        Dim NewConnection As PathfinderConnection
        For A = 0 To PassageNodeCount - 1
            For B = 0 To Args.SymmetryBlockCount - 1
                PassageNodePathNodes(B, A) = New PathfinderNode(PassageNodePathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = PassageNodes(B, A).Pos
                PassageNodePathNodes(B, A).Tag = NodeTag
            Next
        Next
        For A = 0 To ConnectionCount - 1
            If Connections(A).PassageNodeA.Level = Connections(A).PassageNodeB.Level Then
                If Not (Connections(A).PassageNodeA.IsWater Or Connections(A).PassageNodeB.IsWater) Then
                    tmpNodeA = PassageNodePathNodes(Connections(A).PassageNodeA.MirrorNum, Connections(A).PassageNodeA.Num)
                    tmpNodeB = PassageNodePathNodes(Connections(A).PassageNodeB.MirrorNum, Connections(A).PassageNodeB.Num)
                    NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
            End If
        Next
        PassageNodePathMap.LargeArraysResize()
        PassageNodePathMap.FindCalc()

        Dim PossibleRamps(ConnectionCount - 1) As clsConnection
        Dim PossibleRampCount As Integer
        Dim GetPathStartNodes(0) As PathfinderNode
        Dim ResultPaths() As PathfinderNetwork.PathList

        'ramp connections whose points are too far apart

        Dim ConnectionsCanRamp(ConnectionCount - 1) As Boolean

        For B = 0 To ConnectionCount - 1
            'Or Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level) = 2) _
            If Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level) = 1 _
            And Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) _
            And Connections(B).PassageNodeA.MirrorNum = 0 _
            And Connections(B).PassageNodeA.Num <> Connections(B).PassageNodeB.Num Then
                ConnectionsCanRamp(B) = True
            Else
                ConnectionsCanRamp(B) = False
            End If
        Next

        Dim Connectedness As New clsNodeConnectedness
        ReDim Connectedness.NodeConnectedness(PassageNodeCount - 1)
        ReDim Connectedness.PassageNodeVisited(Args.SymmetryBlockCount - 1, PassageNodeCount - 1)
        Connectedness.PassageNodePathNodes = PassageNodePathNodes
        Connectedness.PassageNodePathMap = PassageNodePathMap

        Dim tmpConnectionB As clsConnection
        Dim tmpPathConnection(3) As PathfinderConnection
        Dim Value As Single
        Dim BestDistB As Single
        Dim BaseDist As Single
        Dim RampDist As Single
        Dim UpdateNodeConnectednessArgs As sUpdateNodeConnectednessArgs
        Dim UpdateNetworkConnectednessArgs As sUpdateNetworkConnectednessArgs

        UpdateNodeConnectednessArgs.Args = Connectedness
        UpdateNetworkConnectednessArgs.Args = Connectedness
        ReDim UpdateNetworkConnectednessArgs.PassageNodeUpdated(PassageNodeCount - 1)
        UpdateNetworkConnectednessArgs.SymmetryBlockCount = Args.SymmetryBlockCount

        For A = 0 To PassageNodeCount - 1
            Connectedness.NodeConnectedness(A) = 0.0F
            For B = 0 To PassageNodeCount - 1
                For C = 0 To Args.SymmetryBlockCount - 1
                    Connectedness.PassageNodeVisited(C, B) = False
                Next
            Next
            UpdateNodeConnectednessArgs.OriginalNode = PassageNodes(0, A)
            UpdateNodeConnectedness(UpdateNodeConnectednessArgs, PassageNodes(0, A))
        Next

        Do
            BestNum = -1
            BestDist = 1.0F 'for connections that can already reach the other side
            BestDistB = 0.0F 'for connections that cant
            PossibleRampCount = 0
            For B = 0 To ConnectionCount - 1
                If ConnectionsCanRamp(B) And Not Connections(B).IsRamp Then
                    XY_int.X = Connections(B).PassageNodeB.Pos.X - Connections(B).PassageNodeA.Pos.X
                    XY_int.Y = Connections(B).PassageNodeB.Pos.Y - Connections(B).PassageNodeA.Pos.Y
                    AngleA = XY_int.GetAngle
                    For C = 0 To Connections(B).PassageNodeA.ConnectionCount - 1
                        tmpConnectionB = Connections(B).PassageNodeA.Connections(C).Connection
                        If tmpConnectionB IsNot Connections(B) Then
                            If tmpConnectionB.IsRamp Then
                                XY_int.X = tmpConnectionB.PassageNodeB.Pos.X - tmpConnectionB.PassageNodeA.Pos.X
                                XY_int.Y = tmpConnectionB.PassageNodeB.Pos.Y - tmpConnectionB.PassageNodeA.Pos.Y
                                AngleB = XY_int.GetAngle
                                AngleC = AngleClamp(AngleA - AngleB)
                                If tmpConnectionB.PassageNodeA IsNot Connections(B).PassageNodeA Then
                                    AngleC = AngleClamp(AngleC - Math.PI)
                                End If
                                If Math.Abs(AngleC) < 80.0# * RadOf1Deg Then
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                    If C = Connections(B).PassageNodeA.ConnectionCount Then
                        For D = 0 To Connections(B).PassageNodeB.ConnectionCount - 1
                            tmpConnectionB = Connections(B).PassageNodeB.Connections(D).Connection
                            If tmpConnectionB IsNot Connections(B) Then
                                If tmpConnectionB.IsRamp Then
                                    XY_int.X = tmpConnectionB.PassageNodeB.Pos.X - tmpConnectionB.PassageNodeA.Pos.X
                                    XY_int.Y = tmpConnectionB.PassageNodeB.Pos.Y - tmpConnectionB.PassageNodeA.Pos.Y
                                    AngleB = XY_int.GetAngle
                                    AngleC = AngleClamp(AngleA - AngleB)
                                    If tmpConnectionB.PassageNodeA Is Connections(B).PassageNodeB Then
                                        AngleC = AngleClamp(AngleC - Math.PI)
                                    End If
                                    If Math.Abs(AngleC) < 80.0# * RadOf1Deg Then
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                        If D = Connections(B).PassageNodeB.ConnectionCount Then
                            GetPathStartNodes(0) = PassageNodePathNodes(Connections(B).PassageNodeA.MirrorNum, Connections(B).PassageNodeA.Num)
                            ResultPaths = PassageNodePathMap.GetPath(GetPathStartNodes, PassageNodePathNodes(Connections(B).PassageNodeB.MirrorNum, Connections(B).PassageNodeB.Num), -1, 0)
                            BaseDist = Single.MaxValue
                            XY_int.X = CInt((Connections(B).PassageNodeA.Pos.X + Connections(B).PassageNodeB.Pos.X) / 2.0#)
                            XY_int.Y = CInt((Connections(B).PassageNodeA.Pos.Y + Connections(B).PassageNodeB.Pos.Y) / 2.0#)
                            For E = 0 To Args.PlayerCount - 1
                                Dist = CSng(GetDist_XY_int(BasePassageNodes(E).Pos, XY_int))
                                If Dist < BaseDist Then
                                    BaseDist = Dist
                                End If
                            Next
                            RampDist = Math.Max(CSng(Args.MaxDisconnectionDist * Args.RampBase ^ (BaseDist / 1024.0#)), 1.0F)
                            If ResultPaths Is Nothing Then
                                Value = Connectedness.NodeConnectedness(Connections(B).PassageNodeA.Num) + Connectedness.NodeConnectedness(Connections(B).PassageNodeB.Num)
                                If Single.MaxValue > BestDist Then
                                    BestDist = Single.MaxValue
                                    BestDistB = Value
                                    PossibleRamps(0) = Connections(B)
                                    PossibleRampCount = 1
                                Else
                                    If Value < BestDistB Then
                                        BestDistB = Value
                                        PossibleRamps(0) = Connections(B)
                                        PossibleRampCount = 1
                                    ElseIf Value = BestDistB Then
                                        PossibleRamps(PossibleRampCount) = Connections(B)
                                        PossibleRampCount += 1
                                    End If
                                End If
                            ElseIf ResultPaths(0).PathCount <> 1 Then
                                ReturnResult.Problem = "Error: Invalid number of routes returned."
                                Return ReturnResult
                            ElseIf ResultPaths(0).Paths(0).Value / RampDist > BestDist Then
                                BestDist = ResultPaths(0).Paths(0).Value / RampDist
                                PossibleRamps(0) = Connections(B)
                                PossibleRampCount = 1
                            ElseIf ResultPaths(0).Paths(0).Value / RampDist = BestDist Then
                                PossibleRamps(PossibleRampCount) = Connections(B)
                                PossibleRampCount += 1
                            ElseIf ResultPaths(0).Paths(0).Value <= RampDist Then
                                ConnectionsCanRamp(B) = False
                            End If
                        Else
                            ConnectionsCanRamp(B) = False
                        End If
                    Else
                        ConnectionsCanRamp(B) = False
                    End If
                Else
                    ConnectionsCanRamp(B) = False
                End If
            Next
            If PossibleRampCount > 0 Then
                BestNum = CInt(Int(Rnd() * PossibleRampCount))
                PossibleRamps(BestNum).IsRamp = True
                tmpPassageNodeA = PossibleRamps(BestNum).PassageNodeA
                tmpPassageNodeB = PossibleRamps(BestNum).PassageNodeB
                tmpNodeA = PassageNodePathNodes(tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num)
                tmpNodeB = PassageNodePathNodes(tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num)
                NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                For C = 0 To PossibleRamps(BestNum).ReflectionCount - 1
                    PossibleRamps(BestNum).Reflections(C).IsRamp = True
                    tmpPassageNodeA = PossibleRamps(BestNum).Reflections(C).PassageNodeA
                    tmpPassageNodeB = PossibleRamps(BestNum).Reflections(C).PassageNodeB
                    tmpNodeA = PassageNodePathNodes(tmpPassageNodeA.MirrorNum, tmpPassageNodeA.Num)
                    tmpNodeB = PassageNodePathNodes(tmpPassageNodeB.MirrorNum, tmpPassageNodeB.Num)
                    NewConnection = tmpNodeA.CreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                Next
                PassageNodePathMap.FindCalc()
                For E = 0 To PassageNodeCount - 1
                    UpdateNetworkConnectednessArgs.PassageNodeUpdated(E) = False
                Next
                If PossibleRamps(BestNum).PassageNodeA.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps(BestNum).PassageNodeA)
                ElseIf PossibleRamps(BestNum).PassageNodeB.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(UpdateNetworkConnectednessArgs, PossibleRamps(BestNum).PassageNodeB)
                Else
                    ReturnResult.Problem = "Error: Initial ramp not in area 0."
                    Return ReturnResult
                End If
            Else
                Exit Do
            End If
        Loop

        Dim RampLength As Integer
        Dim MinRampLength As Integer = CInt(LevelHeight * HeightMultiplier * 2.0#) + 128

        For B = 0 To ConnectionCount - 1
            For A = 0 To JitterLayer.GetNodeCount - 1
                RampLength = Math.Max(CInt(GetDist_XY_int(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos) * 0.75#), MinRampLength * Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level))
                If Connections(B).IsRamp Then
                    NodeTag = CType(JitterLayer.GetNode(A).Tag, clsNodeTag)
                    XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, NodeTag.Pos)
                    Dist = CSng(GetDist_XY_int(XY_int, NodeTag.Pos))
                    If Dist < RampLength * 2.0F Then
                        SetBaseLevelRamp(JitterLayer.GetNode(A), Connections(B), BaseNodeLevel, RampLength)
                    End If
                End If
            Next
        Next

        For A = 0 To BaseLayer.GetNodeCount - 1
            NodeTag = CType(BaseLayer.GetNode(A).Tag, clsNodeTag)
            Terrain.Vertices(CInt(NodeTag.Pos.X / 128.0F), CInt(NodeTag.Pos.Y / 128.0F)).Height = CByte(BaseNodeLevel.NodeLevels(A) * LevelHeight)
        Next

        'unique connections test
        'For A = 0 To ConnectionCount - 1
        '    For B = 0 To ConnectionCount - 1
        '        If A <> B Then
        '            If ConnectionsA(A).PassageNodeA Is ConnectionsA(B).PassageNodeA And ConnectionsA(A).PassageNodeB Is ConnectionsA(B).PassageNodeB Then
        '                Stop
        '            End If
        '        End If
        '    Next
        'Next

        'store passage node route distances

        ReDim PassageNodeDists(Args.SymmetryBlockCount - 1, PassageNodeCount - 1, Args.SymmetryBlockCount - 1, PassageNodeCount - 1)
        For A = 0 To PassageNodeCount - 1
            For D = 0 To Args.SymmetryBlockCount - 1
                PassageNodeDists(D, A, D, A) = 0.0F
                For B = A + 1 To PassageNodeCount - 1
                    For C = 0 To Args.SymmetryBlockCount - 1
                        If PassageNodes(0, A).IsWater Or PassageNodes(C, B).IsWater Or (C <> 0 And D <> 0) Then
                            PassageNodeDists(D, A, C, B) = Single.MaxValue
                            PassageNodeDists(C, B, D, A) = Single.MaxValue
                        Else
                            GetPathStartNodes(0) = PassageNodePathNodes(D, A)
                            ResultPaths = PassageNodePathMap.GetPath(GetPathStartNodes, PassageNodePathNodes(C, B), -1, 0)
                            If ResultPaths Is Nothing Then
                                ReturnResult.Problem = "Map is not all connected."
                                Return ReturnResult
                            Else
                                If ResultPaths(0).PathCount <> 1 Then
                                    Stop
                                End If
                                PassageNodeDists(D, A, C, B) = ResultPaths(0).Paths(0).Value
                                PassageNodeDists(C, B, D, A) = ResultPaths(0).Paths(0).Value
                            End If
                        End If
                    Next
                Next
            Next
        Next

        'place oil
        Dim ExtraOilCount As Integer
        Dim MaxBestNodeCount As Integer
        MaxBestNodeCount = 1
        For A = 0 To Args.OilAtATime - 1
            MaxBestNodeCount *= PassageNodeCount
        Next
        Dim BestPossibility As clsGeneratorMap.clsOilPossibilities.clsPossibility
        Dim OilArgs As clsGeneratorMap.sOilBalanceLoopArgs
        ReDim OilArgs.OilClusterSizes(Args.OilAtATime - 1)
        ReDim OilArgs.PlayerOilScore(Args.PlayerCount - 1)
        ReDim OilArgs.OilNodes(Args.OilAtATime - 1)
        OilArgs.GenerateLayoutArgs = Args

        'balanced oil
        Do While ExtraOilCount < Args.ExtraOilCount
            'place oil farthest away from other oil and where it best balances the player oil score
            For A = 0 To Args.OilAtATime - 1
                OilArgs.OilClusterSizes(A) = Math.Min(Args.ExtraOilClusterSizeMin + CInt(Int(Rnd() * (Args.ExtraOilClusterSizeMax - Args.ExtraOilClusterSizeMin + 1))), Math.Max(CInt(Math.Ceiling((Args.ExtraOilCount - ExtraOilCount) / Args.SymmetryBlockCount)), 1))
            Next
            OilArgs.OilPossibilities = New clsGeneratorMap.clsOilPossibilities
            OilBalanceLoop(OilArgs, 0)

            BestPossibility = OilArgs.OilPossibilities.BestPossibility
            If BestPossibility IsNot Nothing Then
                For B = 0 To Args.OilAtATime - 1
                    For A = 0 To Args.SymmetryBlockCount - 1
                        PassageNodes(A, BestPossibility.Nodes(B).Num).OilCount += OilArgs.OilClusterSizes(B)
                    Next
                    ExtraOilCount += OilArgs.OilClusterSizes(B) * Args.SymmetryBlockCount
                Next
                For A = 0 To Args.PlayerCount - 1
                    OilArgs.PlayerOilScore(A) += BestPossibility.PlayerOilScoreAddition(A)
                Next
            Else
                ReturnResult.Problem = "Map is too small for that number of oil clusters"
                Return ReturnResult
            End If
        Loop

        'base oil
        For A = 0 To _PlayerCount - 1
            BasePassageNodes(A).OilCount += Args.BaseOilCount
        Next

        PassageNodePathMap.Deallocate()

        LevelCount = Args.LevelCount

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Structure sPassageNodeLevels
        Public Nodes() As Integer
    End Structure

    Private Sub PassageNodesMinLevelSet(ByVal PassageNode As clsPassageNode, ByRef PassageNodesMinLevel As sPassageNodeLevels, ByVal Level As Integer, ByVal LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level > PassageNodesMinLevel.Nodes(PassageNode.Num) Then
            PassageNodesMinLevel.Nodes(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If tmpPassageNode.MirrorNum = 0 Then
                    PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Sub PassageNodesMaxLevelSet(ByVal PassageNode As clsPassageNode, ByRef PassageNodesMaxLevel As sPassageNodeLevels, ByVal Level As Integer, ByVal LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level < PassageNodesMaxLevel.Nodes(PassageNode.Num) Then
            PassageNodesMaxLevel.Nodes(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If tmpPassageNode.MirrorNum = 0 Then
                    PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Class clsNodeConnectedness
        Public NodeConnectedness() As Single
        Public PassageNodeVisited(,) As Boolean
        Public PassageNodePathMap As PathfinderNetwork
        Public PassageNodePathNodes(,) As PathfinderNode
    End Class

    Private Structure sUpdateNodeConnectednessArgs
        Public OriginalNode As clsPassageNode
        Public Args As clsNodeConnectedness
    End Structure

    Private Sub UpdateNodeConnectedness(ByRef Args As sUpdateNodeConnectednessArgs, ByVal PassageNode As clsPassageNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim PassableCount As Integer

        Args.Args.PassageNodeVisited(PassageNode.MirrorNum, PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not Args.Args.PassageNodeVisited(tmpOtherNode.MirrorNum, tmpOtherNode.Num) Then
                    UpdateNodeConnectedness(Args, tmpOtherNode)
                End If
                PassableCount += 1
            End If
        Next

        Dim Paths() As PathfinderNetwork.PathList
        Dim StartNodes(0) As PathfinderNode
        StartNodes(0) = Args.Args.PassageNodePathNodes(0, Args.OriginalNode.Num)
        Paths = Args.Args.PassageNodePathMap.GetPath(StartNodes, Args.Args.PassageNodePathNodes(PassageNode.MirrorNum, PassageNode.Num), -1, 0)
        Args.Args.NodeConnectedness(Args.OriginalNode.Num) += CSng(PassableCount * 0.999# ^ Paths(0).Paths(0).Value)
    End Sub

    Private Structure sUpdateNetworkConnectednessArgs
        Public PassageNodeUpdated() As Boolean
        Public SymmetryBlockCount As Integer
        Public Args As clsNodeConnectedness
    End Structure

    Private Sub UpdateNetworkConnectedness(ByRef Args As sUpdateNetworkConnectednessArgs, ByVal PassageNode As clsPassageNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim NodeConnectednessArgs As sUpdateNodeConnectednessArgs
        Dim B As Integer
        Dim C As Integer

        Args.PassageNodeUpdated(PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not Args.PassageNodeUpdated(tmpOtherNode.Num) And tmpOtherNode.MirrorNum = 0 Then
                    For B = 0 To PassageNodeCount - 1
                        For C = 0 To Args.SymmetryBlockCount - 1
                            Args.Args.PassageNodeVisited(C, B) = False
                        Next
                    Next
                    NodeConnectednessArgs.OriginalNode = PassageNode
                    NodeConnectednessArgs.Args = Args.Args
                    UpdateNodeConnectedness(NodeConnectednessArgs, PassageNode)
                End If
            End If
        Next
    End Sub

    Private Structure sOilBalanceLoopArgs
        Public OilNodes() As clsPassageNode
        Public OilClusterSizes() As Integer
        Public OilPossibilities As clsGeneratorMap.clsOilPossibilities
        Public PlayerOilScore() As Double
        Public GenerateLayoutArgs As sGenerateLayoutArgs
    End Structure

    Private Sub OilBalanceLoop(ByRef Args As sOilBalanceLoopArgs, ByVal LoopNum As Integer)
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim NextLoopNum As Integer = LoopNum + 1
        Dim tmpPassageNodeA As clsPassageNode

        If NextLoopNum < Args.GenerateLayoutArgs.OilAtATime Then
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodes(0, A)
                For B = 0 To _PlayerCount - 1
                    If BasePassageNodes(B) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                For C = 0 To LoopNum - 1
                    If Args.OilNodes(C) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                If tmpPassageNodeA.OilCount = 0 And Not tmpPassageNodeA.IsOnBorder And B = _PlayerCount And C = LoopNum And Not tmpPassageNodeA.IsWater Then
                    Args.OilNodes(LoopNum) = tmpPassageNodeA
                    OilBalanceLoop(Args, NextLoopNum)
                End If
            Next
        Else
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodes(0, A)
                For B = 0 To _PlayerCount - 1
                    If BasePassageNodes(B) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                For C = 0 To LoopNum - 1
                    If Args.OilNodes(C) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                If tmpPassageNodeA.OilCount = 0 And Not tmpPassageNodeA.IsOnBorder And B = _PlayerCount And C = LoopNum And Not tmpPassageNodeA.IsWater Then
                    Args.OilNodes(LoopNum) = tmpPassageNodeA
                    OilValueCalc(Args)
                End If
            Next
        End If
    End Sub

    Public Class clsOilPossibilities
        Public Class clsPossibility
            Public Nodes() As clsPassageNode
            Public Score As Double
            Public PlayerOilScoreAddition() As Double
        End Class
        Public BestPossibility As clsPossibility

        Public Sub NewPossibility(ByVal Possibility As clsPossibility)

            If BestPossibility Is Nothing Then
                BestPossibility = Possibility
            ElseIf Possibility.Score < BestPossibility.Score Then
                BestPossibility = Possibility
            End If
        End Sub
    End Class

    Private Sub OilValueCalc(ByRef Args As sOilBalanceLoopArgs)
        Dim OilDistScore As Double
        Dim OilStraightDistScore As Double
        Dim LowestScore As Double
        Dim HighestScore As Double
        'Dim TotalOilScore As Double
        Dim UnbalancedScore As Double
        Dim dblTemp As Double
        Dim Value As Double
        Dim NewPossibility As New clsGeneratorMap.clsOilPossibilities.clsPossibility
        Dim BaseOilScore(Args.GenerateLayoutArgs.PlayerCount - 1) As Double

        ReDim NewPossibility.PlayerOilScoreAddition(Args.GenerateLayoutArgs.PlayerCount - 1)

        Dim NewOilNum As Integer
        Dim OtherOilNum As Integer
        Dim NewOilNodeNum As Integer
        Dim OtherOilNodeNum As Integer
        Dim SymmetryBlockNum As Integer
        Dim MapNodeNum As Integer
        Dim PlayerNum As Integer
        'Dim NewOilCount As Integer
        Dim OilMassMultiplier As Double

        OilDistScore = 0.0#
        OilStraightDistScore = 0.0#
        For PlayerNum = 0 To Args.GenerateLayoutArgs.PlayerCount - 1
            NewPossibility.PlayerOilScoreAddition(PlayerNum) = 0.0#
        Next
        For NewOilNum = 0 To Args.GenerateLayoutArgs.OilAtATime - 1
            NewOilNodeNum = Args.OilNodes(NewOilNum).Num
            'other oil to be placed in the first area
            For OtherOilNum = NewOilNum + 1 To Args.GenerateLayoutArgs.OilAtATime - 1
                OtherOilNodeNum = Args.OilNodes(OtherOilNum).Num
                OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, 0, OtherOilNodeNum)
                OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(0, OtherOilNodeNum).Pos)
            Next
            'other oil to be placed in symmetrical areas
            For OtherOilNum = 0 To Args.GenerateLayoutArgs.OilAtATime - 1
                OtherOilNodeNum = Args.OilNodes(OtherOilNum).Num
                OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * Args.OilClusterSizes(OtherOilNum)
                For SymmetryBlockNum = 1 To Args.GenerateLayoutArgs.SymmetryBlockCount - 1
                    OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, OtherOilNodeNum)
                    OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, OtherOilNodeNum).Pos)
                Next
            Next
            'oil on the map
            For MapNodeNum = 0 To PassageNodeCount - 1
                For SymmetryBlockNum = 0 To Args.GenerateLayoutArgs.SymmetryBlockCount - 1
                    If PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount > 0 Then
                        OilMassMultiplier = Args.OilClusterSizes(NewOilNum) * PassageNodes(SymmetryBlockNum, MapNodeNum).OilCount
                        OilDistScore += OilMassMultiplier / PassageNodeDists(0, NewOilNodeNum, SymmetryBlockNum, MapNodeNum)
                        OilStraightDistScore += OilMassMultiplier / GetDist_XY_int(PassageNodes(0, NewOilNodeNum).Pos, PassageNodes(SymmetryBlockNum, MapNodeNum).Pos)
                    End If
                Next
            Next
            'extra oil score for players
            For PlayerNum = 0 To Args.GenerateLayoutArgs.PlayerCount - 1
                BaseOilScore(PlayerNum) = 0.0#
                For SymmetryBlockNum = 0 To Args.GenerateLayoutArgs.SymmetryBlockCount - 1
                    dblTemp = PassageNodeDists(0, BasePassageNodes(PlayerNum).Num, SymmetryBlockNum, NewOilNodeNum) * 2.0# + GetDist_XY_int(BasePassageNodes(PlayerNum).Pos, PassageNodes(SymmetryBlockNum, NewOilNodeNum).Pos)
                    BaseOilScore(PlayerNum) += 100.0# / dblTemp
                Next
            Next
            'TotalOilScore = 0.0#
            'For PlayerNum = 0 To Args.GenerateLayoutArgs.PlayerCount - 1
            '    TotalOilScore += BaseOilScore(PlayerNum)
            'Next
            For PlayerNum = 0 To Args.GenerateLayoutArgs.PlayerCount - 1
                NewPossibility.PlayerOilScoreAddition(PlayerNum) += Args.OilClusterSizes(NewOilNum) * BaseOilScore(PlayerNum)
            Next
        Next

        LowestScore = Double.MaxValue
        HighestScore = Double.MinValue
        For PlayerNum = 0 To Args.GenerateLayoutArgs.PlayerCount - 1
            dblTemp = Args.PlayerOilScore(PlayerNum) + NewPossibility.PlayerOilScoreAddition(PlayerNum)
            If dblTemp < LowestScore Then
                LowestScore = dblTemp
            End If
            If dblTemp > HighestScore Then
                HighestScore = dblTemp
            End If
        Next
        UnbalancedScore = HighestScore - LowestScore

        'NewOilCount = 0
        'For NewOilNum = 0 To Args.GenerateLayoutArgs.OilAtATime - 1
        '    NewOilCount += Args.OilClusterSizes(NewOilNum)
        'Next
        'divide all dists by the number of oil resources placed. does not include other symmetries, since they were never added in, and are exactly the same.
        Value = Args.GenerateLayoutArgs.OilDispersion * (OilDistScore * 3.0# + OilStraightDistScore) + UnbalancedScore
        NewPossibility.Score = Value
        ReDim NewPossibility.Nodes(Args.GenerateLayoutArgs.OilAtATime - 1)
        For NewOilNum = 0 To Args.GenerateLayoutArgs.OilAtATime - 1
            NewPossibility.Nodes(NewOilNum) = Args.OilNodes(NewOilNum)
        Next
        Args.OilPossibilities.NewPossibility(NewPossibility)
    End Sub

    Private Structure sBaseNodeLevels
        Public NodeLevels() As Single
    End Structure

    Private Sub SetBaseLevel(ByVal Node As PathfinderNode, ByVal NewLevel As Integer, ByRef BaseLevel As sBaseNodeLevels)

        If Node.GetChildNodeCount = 0 Then
            Dim A As Integer
            Dim Height As Single
            Dim Lowest As Single = NewLevel
            For A = 0 To Node.GetConnectionCount - 1
                Height = BaseLevel.NodeLevels(Node.GetConnection(A).GetOtherNode(Node).GetLayer_NodeNum)
                If Height < Lowest Then
                    Lowest = Height
                End If
            Next
            If NewLevel - Lowest > 1.0F Then
                BaseLevel.NodeLevels(Node.GetLayer_NodeNum) = Lowest + 1.0F
            Else
                BaseLevel.NodeLevels(Node.GetLayer_NodeNum) = NewLevel
            End If
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                SetBaseLevel(Node.GetChildNode(A), NewLevel, BaseLevel)
            Next
        End If
    End Sub

    Private Sub SetBaseLevelRamp(ByVal Node As PathfinderNode, ByVal Connection As clsConnection, ByRef BaseLevel As sBaseNodeLevels, ByVal RampLength As Integer)

        If Node.GetChildNodeCount = 0 Then
            Dim NodeTag As clsNodeTag = CType(Node.Tag, clsNodeTag)
            Dim XY_int As sXY_int = PointGetClosestPosOnLine(Connection.PassageNodeA.Pos, Connection.PassageNodeB.Pos, NodeTag.Pos)
            Dim ConnectionLength As Single = CSng(GetDist_XY_int(Connection.PassageNodeA.Pos, Connection.PassageNodeB.Pos))
            Dim Extra As Single = ConnectionLength - RampLength
            Dim ConnectionPos As Single = CSng(GetDist_XY_int(XY_int, Connection.PassageNodeA.Pos))
            Dim RampPos As Single = Clamp_sng((ConnectionPos - Extra / 2.0F) / RampLength, 0.0F, 1.0F)
            Dim Layer_NodeNum As Integer = Node.GetLayer_NodeNum
            RampPos = CSng(1.0# - (Math.Cos(RampPos * Math.PI) + 1.0#) / 2.0#)
            If RampPos > 0.0F And RampPos < 1.0F Then
                Dim Dist2 As Single = CSng(GetDist_XY_int(NodeTag.Pos, XY_int))
                If Dist2 < 320.0F Then
                    Dim Dist2Factor As Single = 1.0F 'Math.Min(3.0F - 3.0F * Dist2 / 384.0F, 1.0F) 'distance fading
                    If BaseLevel.NodeLevels(Layer_NodeNum) = Int(BaseLevel.NodeLevels(Layer_NodeNum)) Then
                        BaseLevel.NodeLevels(Layer_NodeNum) = BaseLevel.NodeLevels(Layer_NodeNum) * (1.0F - Dist2Factor) + (Connection.PassageNodeA.Level * (1.0F - RampPos) + Connection.PassageNodeB.Level * RampPos) * Dist2Factor
                    Else
                        BaseLevel.NodeLevels(Layer_NodeNum) = (BaseLevel.NodeLevels(Layer_NodeNum) * (2.0F - Dist2Factor) + (Connection.PassageNodeA.Level * (1.0F - RampPos) + Connection.PassageNodeB.Level * RampPos) * Dist2Factor) / 2.0F
                    End If
                End If
            End If
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                SetBaseLevelRamp(Node.GetChildNode(A), Connection, BaseLevel, RampLength)
            Next
        End If
    End Sub

    Private Class clsNearest
        Public Num As Integer = -1
        Public NodeA() As clsPassageNode
        Public NodeB() As clsPassageNode
        Public NodeCount As Integer
        Public Dist2 As Single
        Public BlockedCount As Integer
        Public BlockedNearests() As clsNearest
        Public BlockedNearestCount As Integer
        Public Invalid As Boolean
    End Class

    Private Structure sTestNearestArgs
        Public Args As sGenerateLayoutArgs
        Public MaxConDist2 As Integer
        Public MinConDist As Integer
        Public tmpPassageNodeA As clsPassageNode
        Public tmpPassageNodeB As clsPassageNode
    End Structure

    Private Function TestNearest(ByRef Args As sTestNearestArgs) As Boolean
        Dim XY_int As sXY_int
        Dim NearestA As clsNearest
        Dim Dist2 As Integer
        Dim A As Integer
        Dim B As Integer
        Dim ReflectionNum As Integer
        Dim ReflectionCount As Integer

        If Args.tmpPassageNodeA.MirrorNum <> 0 Then
            Stop
            Return False
        End If

        XY_int.X = Args.tmpPassageNodeB.Pos.X - Args.tmpPassageNodeA.Pos.X
        XY_int.Y = Args.tmpPassageNodeB.Pos.Y - Args.tmpPassageNodeA.Pos.Y
        Dist2 = XY_int.X * XY_int.X + XY_int.Y * XY_int.Y
        If Dist2 > Args.MaxConDist2 Then
            Return False
        End If
        For A = 0 To PassageNodeCount - 1
            For B = 0 To Args.Args.SymmetryBlockCount - 1
                If PassageNodes(B, A) IsNot Args.tmpPassageNodeA And PassageNodes(B, A) IsNot Args.tmpPassageNodeB Then
                    XY_int = PointGetClosestPosOnLine(Args.tmpPassageNodeA.Pos, Args.tmpPassageNodeB.Pos, PassageNodes(B, A).Pos)
                    If GetDist_XY_int(XY_int, PassageNodes(B, A).Pos) < Args.MinConDist Then
                        Return False
                    End If
                End If
            Next
        Next

        NearestA = New clsNearest
        With NearestA
            .Num = NearestCount
            .Dist2 = Dist2
            If Args.tmpPassageNodeA.MirrorNum = Args.tmpPassageNodeB.MirrorNum Then
                ReDim .NodeA(Args.Args.SymmetryBlockCount - 1)
                ReDim .NodeB(Args.Args.SymmetryBlockCount - 1)
                For A = 0 To Args.Args.SymmetryBlockCount - 1
                    .NodeA(A) = PassageNodes(A, Args.tmpPassageNodeA.Num)
                    .NodeB(A) = PassageNodes(A, Args.tmpPassageNodeB.Num)
                Next
                .NodeCount = Args.Args.SymmetryBlockCount
            Else
                If Args.Args.SymmetryIsRotational Then
                    ReDim .NodeA(Args.Args.SymmetryBlockCount - 1)
                    ReDim .NodeB(Args.Args.SymmetryBlockCount - 1)
                    ReflectionCount = CInt(Args.Args.SymmetryBlockCount / 2.0#)
                    For ReflectionNum = 0 To ReflectionCount - 1
                        If Args.Args.SymmetryBlocks(0).ReflectToNum(ReflectionNum) = Args.tmpPassageNodeB.MirrorNum Then
                            Exit For
                        End If
                    Next
                    If ReflectionNum = ReflectionCount Then
                        Return False
                    End If
                    For A = 0 To Args.Args.SymmetryBlockCount - 1
                        .NodeA(A) = PassageNodes(A, Args.tmpPassageNodeA.Num)
                        .NodeB(A) = PassageNodes(Args.Args.SymmetryBlocks(A).ReflectToNum(ReflectionNum), Args.tmpPassageNodeB.Num)
                    Next
                    .NodeCount = Args.Args.SymmetryBlockCount
                Else
                    If Args.tmpPassageNodeA.Num <> Args.tmpPassageNodeB.Num Then
                        Return False
                    End If
                    If Args.Args.SymmetryBlockCount = 4 Then
                        ReDim .NodeA(1)
                        ReDim .NodeB(1)
                        ReflectionCount = CInt(Args.Args.SymmetryBlockCount / 2.0#)
                        For ReflectionNum = 0 To ReflectionCount - 1
                            If Args.Args.SymmetryBlocks(0).ReflectToNum(ReflectionNum) = Args.tmpPassageNodeB.MirrorNum Then
                                Exit For
                            End If
                        Next
                        If ReflectionNum = ReflectionCount Then
                            Return False
                        End If
                        .NodeA(0) = Args.tmpPassageNodeA
                        .NodeB(0) = Args.tmpPassageNodeB
                        B = Args.Args.SymmetryBlocks(0).ReflectToNum(1 - ReflectionNum)
                        .NodeA(1) = PassageNodes(B, Args.tmpPassageNodeA.Num)
                        .NodeB(1) = PassageNodes(Args.Args.SymmetryBlocks(B).ReflectToNum(ReflectionNum), Args.tmpPassageNodeB.Num)
                        .NodeCount = 2
                    Else
                        ReDim .NodeA(0)
                        ReDim .NodeB(0)
                        .NodeA(0) = Args.tmpPassageNodeA
                        .NodeB(0) = Args.tmpPassageNodeB
                        .NodeCount = 1
                    End If
                End If
            End If

            ReDim .BlockedNearests(511)
        End With
        Nearests(NearestCount) = NearestA
        NearestCount += 1

        Return True
    End Function

    Public Sub TerrainBlockPaths()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To Terrain.TileSize.X - 1
                If Terrain.Tiles(X, Y).Texture.TextureNum >= 0 Then
                    If GenerateTileset.Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Or GenerateTileset.Tileset.Tiles(Terrain.Tiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                        TileNodeBlock(X, Y)
                    End If
                End If
            Next
        Next
        TilePathMap.FindCalc()
    End Sub

    Public Function GetWaterMap() As clsBooleanMap
        Dim ReturnResult As New clsBooleanMap
        Dim BestDist As Single
        Dim BestIsWater As Boolean
        Dim Pos As sXY_int
        Dim Dist As Single
        Dim B As Integer
        Dim C As Integer
        Dim XY_int As sXY_int
        Dim X As Integer
        Dim Y As Integer

        ReturnResult.Blank(Terrain.TileSize.X + 1, Terrain.TileSize.Y + 1)
        For Y = 0 To Terrain.TileSize.Y
            For X = 0 To Terrain.TileSize.X
                BestDist = Single.MaxValue
                Pos = New sXY_int(X * TerrainGridSpacing, Y * TerrainGridSpacing)
                For B = 0 To ConnectionCount - 1
                    'If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                    If Connections(B).PassageNodeA.IsWater = Connections(B).PassageNodeB.IsWater Then
                        'only do this if the waters are the same
                        'this is to make sure nodes that are connected are actually connected as water
                        XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, Pos)
                        Dist = CSng(GetDist_XY_int(XY_int, Pos))
                        If Dist < BestDist Then
                            BestDist = Dist
                            If GetDist_XY_int(Pos, Connections(B).PassageNodeA.Pos) <= GetDist_XY_int(Pos, Connections(B).PassageNodeB.Pos) Then
                                BestIsWater = Connections(B).PassageNodeA.IsWater
                            Else
                                BestIsWater = Connections(B).PassageNodeB.IsWater
                            End If
                        End If
                    End If
                Next
                For C = 0 To PassageNodeCount - 1
                    For B = 0 To _SymmetryBlockCount - 1
                        Dist = CSng(GetDist_XY_int(Pos, PassageNodes(B, C).Pos))
                        If Dist < BestDist Then
                            BestDist = Dist
                            BestIsWater = PassageNodes(B, C).IsWater
                        End If
                    Next
                Next
                ReturnResult.ValueData.Value(Y, X) = BestIsWater
            Next
        Next
        Return ReturnResult
    End Function

    Public Function GetNearestNode(ByVal Network As PathfinderNetwork, ByVal Pos As sXY_int, ByVal MinClearance As Integer) As PathfinderNode
        Dim A As Integer
        Dim Dist As Double
        Dim tmpNode As PathfinderNode
        Dim BestNode As PathfinderNode
        Dim BestDist As Double
        Dim tmpNodeTag As clsNodeTag

        BestDist = Double.MaxValue
        BestNode = Nothing
        For A = 0 To Network.GetNodeLayer(0).GetNodeCount - 1
            tmpNode = Network.GetNodeLayer(0).GetNode(A)
            If tmpNode.GetClearance >= MinClearance Then
                tmpNodeTag = CType(tmpNode.Tag, clsNodeTag)
                Dist = GetDist_XY_int(tmpNodeTag.Pos, Pos)
                If Dist < BestDist Then
                    BestDist = Dist
                    BestNode = tmpNode
                End If
            End If
        Next
        Return BestNode
    End Function

    Public Function PlaceUnitNear(ByVal Type As clsUnitType, ByVal Pos As sXY_int, ByVal PlayerNum As Byte, ByVal Clearance As Integer, ByVal Rotation As Integer, ByVal MaxDistFromPos As Integer) As clsMap.clsUnit
        Dim PosNode As PathfinderNode
        Dim NodeTag As clsNodeTag
        Dim FinalTilePos As sXY_int
        Dim TilePosA As sXY_int
        Dim TilePosB As sXY_int
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim Remainder As Integer
        Dim Footprint As sXY_int

        PosNode = GetNearestNode(TilePathMap, Pos, Clearance)
        If PosNode IsNot Nothing Then
            NodeTag = CType(PosNode.Tag, clsNodeTag)
            If GetDist_XY_int(Pos, NodeTag.Pos) <= MaxDistFromPos Then

                Dim NewUnit As New clsMap.clsUnit
                NewUnit.Type = Type
                NewUnit.UnitGroup = UnitGroups(PlayerNum)

                FinalTilePos.X = CInt(Int(NodeTag.Pos.X / TerrainGridSpacing))
                FinalTilePos.Y = CInt(Int(NodeTag.Pos.Y / TerrainGridSpacing))
                Footprint = Type.GetFootprint
                Math.DivRem(Footprint.X, 2, Remainder)
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.X = NodeTag.Pos.X
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X - CInt(TerrainGridSpacing / 2.0#)
                    Else
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X + CInt(TerrainGridSpacing / 2.0#)
                    End If
                End If
                Math.DivRem(Footprint.Y, 2, Remainder)
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y - CInt(TerrainGridSpacing / 2.0#)
                    Else
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y + CInt(TerrainGridSpacing / 2.0#)
                    End If
                End If
                'NewUnit.Pos.Altitude = GetTerrainHeight(NewUnit.Pos.Horizontal)
                TilePosA.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#))
                TilePosA.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#))
                TilePosB.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#))
                TilePosB.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#))
                NewUnit.Rotation = Rotation

                Unit_Add(NewUnit)

                For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, Terrain.TileSize.Y - 1)
                    For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, Terrain.TileSize.X - 1)
                        TileNodeBlock(X2, Y2)
                    Next
                Next

                TilePathMap.FindCalc()

                Return NewUnit
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Function PlaceUnit(ByVal Type As clsUnitType, ByVal Pos As sWorldPos, ByVal PlayerNum As Byte, ByVal Rotation As Integer) As clsMap.clsUnit
        Dim TilePosA As sXY_int
        Dim TilePosB As sXY_int
        Dim X2 As Integer
        Dim Y2 As Integer
        Dim FinalTilePos As sXY_int
        Dim Footprint As sXY_int

        Dim NewUnit As New clsMap.clsUnit
        NewUnit.Type = Type
        NewUnit.UnitGroup = UnitGroups(PlayerNum)

        FinalTilePos.X = CInt(Int(Pos.Horizontal.X / TerrainGridSpacing))
        FinalTilePos.Y = CInt(Int(Pos.Horizontal.Y / TerrainGridSpacing))

        Footprint = Type.GetFootprint

        NewUnit.Pos = Pos
        TilePosA.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#))
        TilePosA.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#))
        TilePosB.X = CInt(Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#))
        TilePosB.Y = CInt(Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#))
        NewUnit.Rotation = Rotation

        Unit_Add(NewUnit)

        For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, Terrain.TileSize.Y - 1)
            For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, Terrain.TileSize.X - 1)
                TileNodeBlock(X2, Y2)
            Next
        Next

        TilePathMap.FindCalc()

        Return NewUnit
    End Function

    Public Sub TileNodeBlock(ByVal X As Integer, ByVal Y As Integer)
        Dim X2 As Integer
        Dim Y2 As Integer

        For Y2 = Math.Max(Y - 6, 0) To Math.Min(Y + 6, Terrain.TileSize.Y - 1)
            For X2 = Math.Max(X - 6, 0) To Math.Min(X + 6, Terrain.TileSize.X - 1)
                GenerateTerrainTiles(X2, Y2).Node.ClearanceSet(Math.Min(GenerateTerrainTiles(X2, Y2).Node.GetClearance, Math.Max(Math.Abs(Y2 - Y), Math.Abs(X2 - X))))
            Next
        Next

        If GenerateTerrainTiles(X, Y).TopLeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopLeftLink.Destroy()
            GenerateTerrainTiles(X, Y).TopLeftLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).TopLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopLink.Destroy()
            GenerateTerrainTiles(X, Y).TopLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).TopRightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).TopRightLink.Destroy()
            GenerateTerrainTiles(X, Y).TopRightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).RightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).RightLink.Destroy()
            GenerateTerrainTiles(X, Y).RightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomRightLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomRightLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomRightLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).BottomLeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).BottomLeftLink.Destroy()
            GenerateTerrainTiles(X, Y).BottomLeftLink = Nothing
        End If
        If GenerateTerrainTiles(X, Y).LeftLink IsNot Nothing Then
            GenerateTerrainTiles(X, Y).LeftLink.Destroy()
            GenerateTerrainTiles(X, Y).LeftLink = Nothing
        End If
    End Sub

    Public Sub BlockEdgeTiles()
        Dim X As Integer
        Dim Y As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            For X = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For X = Terrain.TileSize.X - 4 To Terrain.TileSize.X - 1
                TileNodeBlock(X, Y)
            Next
        Next
        For X = 3 To Terrain.TileSize.X - 5
            For Y = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For Y = Terrain.TileSize.Y - 4 To Terrain.TileSize.Y - 1
                TileNodeBlock(X, Y)
            Next
        Next
        TilePathMap.FindCalc()
    End Sub

    Public Structure sGenerateUnitsArgs
        Public FeatureClusterChance As Single
        Public FeatureClusterMinUnits As Integer
        Public FeatureClusterMaxUnits As Integer
        Public FeatureScatterCount As Integer
        Public TruckCount As Integer
    End Structure

    Public Function GenerateUnits(ByRef Args As sGenerateUnitsArgs) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim tmpUnit As clsMap.clsUnit
        Dim Count As Integer
        Dim FeaturePlaceRange As Integer = 6 * 128
        Dim BasePlaceRange As Integer = 16 * 128
        Dim TilePos As sXY_int
        Dim AverageHeight As Byte
        Dim PlayerNum As Byte

        For A = 0 To _PlayerCount - 1
            PlayerNum = CByte(A)
            If PlaceUnitNear(UnitType_CommandCentre, BasePassageNodes(A).Pos, PlayerNum, 3, 0, BasePlaceRange) Is Nothing Then
                ReturnResult.Problem = "No room for base structures"
                Return ReturnResult
            End If
            tmpUnit = PlaceUnitNear(UnitType_PowerGenerator, BasePassageNodes(A).Pos, PlayerNum, 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                ReturnResult.Problem = "No room for base structures."
                Return ReturnResult
            End If
            tmpUnit = PlaceUnit(UnitType_PowerModule, tmpUnit.Pos, PlayerNum, 0)
            If tmpUnit Is Nothing Then
                ReturnResult.Problem = "No room for placing module."
                Return ReturnResult
            End If
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_ResearchFacility, BasePassageNodes(A).Pos, PlayerNum, 3, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.Problem = "No room for base structures"
                    Return ReturnResult
                End If
                tmpUnit = PlaceUnit(UnitType_ResearchModule, tmpUnit.Pos, PlayerNum, 0)
                If tmpUnit Is Nothing Then
                    ReturnResult.Problem = "No room for module."
                    Return ReturnResult
                End If
            Next
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_Factory, BasePassageNodes(A).Pos, PlayerNum, 4, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.Problem = "No room for base structures"
                    Return ReturnResult
                End If
                tmpUnit = PlaceUnit(UnitType_FactoryModule, tmpUnit.Pos, PlayerNum, 0)
                If tmpUnit Is Nothing Then
                    ReturnResult.Problem = "No room for module."
                    Return ReturnResult
                End If
            Next
            tmpUnit = PlaceUnitNear(UnitType_CyborgFactory, BasePassageNodes(A).Pos, PlayerNum, 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                ReturnResult.Problem = "No room for base structures"
                Return ReturnResult
            End If
            For B = 1 To Args.TruckCount
                tmpUnit = PlaceUnitNear(UnitType_Truck, BasePassageNodes(A).Pos, PlayerNum, 2, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    ReturnResult.Problem = "No room for trucks"
                    Return ReturnResult
                End If
            Next
        Next
        For A = 0 To PassageNodeCount - 1
            For D = 0 To _SymmetryBlockCount - 1
                For C = 0 To _PlayerCount - 1
                    If BasePassageNodes(C) Is PassageNodes(D, A) Then
                        Exit For
                    End If
                Next
                For B = 0 To PassageNodes(D, A).OilCount - 1
                    If C < _PlayerCount Then
                        tmpUnit = PlaceUnitNear(UnitType_OilResource, PassageNodes(D, A).Pos, 0, 2, 0, BasePlaceRange)
                    Else
                        tmpUnit = PlaceUnitNear(UnitType_OilResource, PassageNodes(D, A).Pos, 0, 2, 0, FeaturePlaceRange)
                    End If
                    If tmpUnit Is Nothing Then
                        ReturnResult.Problem = "No room for base oil."
                        Return ReturnResult
                    End If
                    'flatten ground underneath
                    TilePos.X = CInt(Int(tmpUnit.Pos.Horizontal.X / TerrainGridSpacing))
                    TilePos.Y = CInt(Int(tmpUnit.Pos.Horizontal.Y / TerrainGridSpacing))
                    AverageHeight = CByte((CInt(Terrain.Vertices(TilePos.X, TilePos.Y).Height) + CInt(Terrain.Vertices(TilePos.X + 1, TilePos.Y).Height) + CInt(Terrain.Vertices(TilePos.X, TilePos.Y + 1).Height) + CInt(Terrain.Vertices(TilePos.X + 1, TilePos.Y + 1).Height)) / 4.0#)
                    Terrain.Vertices(TilePos.X, TilePos.Y).Height = AverageHeight
                    Terrain.Vertices(TilePos.X + 1, TilePos.Y).Height = AverageHeight
                    Terrain.Vertices(TilePos.X, TilePos.Y + 1).Height = AverageHeight
                    Terrain.Vertices(TilePos.X + 1, TilePos.Y + 1).Height = AverageHeight
                    'tmpUnit.Pos.Altitude = AverageHeight * HeightMultiplier
                    If C < _PlayerCount Then
                        'place base derrick
                        tmpUnit = PlaceUnit(UnitType_Derrick, tmpUnit.Pos, CByte(C), 0)
                        If tmpUnit Is Nothing Then
                            ReturnResult.Problem = "No room for derrick."
                            Return ReturnResult
                        End If
                    End If
                Next
            Next
        Next

        'feature clusters
        For A = 0 To PassageNodeCount - 1
            For D = 0 To _SymmetryBlockCount - 1
                For B = 0 To _PlayerCount - 1
                    If PassageNodes(D, A) Is BasePassageNodes(B) Then
                        Exit For
                    End If
                Next
                If B = _PlayerCount And Not PassageNodes(D, A).IsOnBorder Then
                    PassageNodes(D, A).HasFeatureCluster = (Rnd() < Args.FeatureClusterChance)
                End If
            Next
        Next

        Dim RandNum As UInteger
        Dim uintTemp As UInteger
        Dim tmpNode As PathfinderNode
        Dim E As Integer
        Dim Footprint As sXY_int

        If GenerateTileset.ClusteredUnitChanceTotal > 0 Then
            For A = 0 To PassageNodeCount - 1
                For D = 0 To _SymmetryBlockCount - 1
                    If PassageNodes(D, A).HasFeatureCluster Then
                        Count = Args.FeatureClusterMinUnits + CInt(Int(Rnd() * (Args.FeatureClusterMaxUnits - Args.FeatureClusterMinUnits + 1)))
                        For B = 1 To Count
                            RandNum = CUInt(Int(Rnd() * CDbl(GenerateTileset.ClusteredUnitChanceTotal)))
                            uintTemp = 0
                            For C = 0 To GenerateTileset.ClusteredUnitCount - 1
                                uintTemp += GenerateTileset.ClusteredUnits(C).Chance
                                If RandNum < uintTemp Then
                                    Exit For
                                End If
                            Next
                            Footprint = GenerateTileset.ClusteredUnits(C).Type.GetFootprint
                            E = CInt(Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F)) + 1
                            tmpUnit = PlaceUnitNear(GenerateTileset.ClusteredUnits(C).Type, PassageNodes(D, A).Pos, 0, E, 0, FeaturePlaceRange)
                            If tmpUnit Is Nothing Then
                                ReturnResult.Problem = "Not enough space for a clustered unit"
                                Return ReturnResult
                            End If
                        Next
                    End If
                Next
            Next
        End If

        If TilePathMap.GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNodeCount <> 1 Then
            ReturnResult.Problem = "Error: bad node count on top layer!"
            Return ReturnResult
        End If

        If GenerateTileset.ScatteredUnitChanceTotal > 0 Then
            For A = 1 To Args.FeatureScatterCount
                RandNum = CUInt(Int(Rnd() * CDbl(GenerateTileset.ScatteredUnitChanceTotal)))
                uintTemp = 0
                For C = 0 To GenerateTileset.ScatteredUnitCount - 1
                    uintTemp += GenerateTileset.ScatteredUnits(C).Chance
                    If RandNum < uintTemp Then
                        Exit For
                    End If
                Next
                Footprint = GenerateTileset.ScatteredUnits(C).Type.GetFootprint
                B = 2 + CInt(Math.Ceiling(Math.Max(Footprint.X, Footprint.Y) / 2.0F))
                tmpNode = GetRandomChildNode(TilePathMap.GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNode(0), B + 2)
                If tmpNode Is Nothing Then
                    ReturnResult.Problem = "Not enough space for a scattered unit"
                    Return ReturnResult
                Else
                    Dim NodeTag As clsNodeTag = CType(tmpNode.Tag, clsNodeTag)
                    If PlaceUnitNear(GenerateTileset.ScatteredUnits(C).Type, NodeTag.Pos, 0, B, 0, FeaturePlaceRange) Is Nothing Then
                        ReturnResult.Problem = "Not enough space for a scattered unit"
                        Return ReturnResult
                    End If
                End If
            Next
        End If

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function GetRandomChildNode(ByVal InputNode As PathfinderNode, ByVal MinClearance As Integer) As PathfinderNode

        If InputNode.GetClearance < MinClearance Then
            Return Nothing
        End If

        If InputNode.GetChildNodeCount = 0 Then
            Return InputNode
        Else
            Dim A As Integer
            Do
                A = CInt(Int(Rnd() * InputNode.GetChildNodeCount))
            Loop While InputNode.GetChildNode(A).GetClearance < MinClearance

            Dim ReturnResult As PathfinderNode = GetRandomChildNode(InputNode.GetChildNode(A), MinClearance)
            Return ReturnResult
        End If
    End Function

    Public Structure sGenerateGatewaysArgs
        Public LayoutArgs As sGenerateLayoutArgs
    End Structure

    Private Structure sPossibleGateway
        Public StartPos As sXY_int
        Public MiddlePos As sXY_int
        Public IsVertical As Boolean
        Public Length As Integer
    End Structure

    Public Function GenerateGateways(ByRef Args As sGenerateGatewaysArgs) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        'must be done before units otherwise the units will be treated as gateway obstacles

        Dim X As Integer
        Dim SpaceCount As Integer
        Dim Y As Integer
        Dim PossibleGateways(Terrain.TileSize.X * Terrain.TileSize.Y * 2 - 1) As sPossibleGateway
        Dim PossibleGatewayCount As Integer

        For Y = 0 To Terrain.TileSize.Y - 1
            SpaceCount = 0
            For X = 0 To Terrain.TileSize.X - 1
                If GenerateTerrainTiles(X, Y).Node.GetClearance < 1 Then

                ElseIf GenerateTerrainTiles(X, Y).Node.GetClearance = 1 Then
                    If SpaceCount > 3 And SpaceCount <= 9 Then
                        PossibleGateways(PossibleGatewayCount).StartPos.X = X - SpaceCount
                        PossibleGateways(PossibleGatewayCount).StartPos.Y = Y
                        PossibleGateways(PossibleGatewayCount).Length = SpaceCount + 1
                        PossibleGateways(PossibleGatewayCount).IsVertical = False
                        PossibleGateways(PossibleGatewayCount).MiddlePos.X = PossibleGateways(PossibleGatewayCount).StartPos.X * 128 + PossibleGateways(PossibleGatewayCount).Length * 64
                        PossibleGateways(PossibleGatewayCount).MiddlePos.Y = PossibleGateways(PossibleGatewayCount).StartPos.Y * 128
                        PossibleGatewayCount += 1
                    End If
                    SpaceCount = 1
                Else
                    SpaceCount += 1
                End If
            Next
        Next
        For X = 0 To Terrain.TileSize.X - 1
            SpaceCount = 0
            Y = 0
            Do While Y < Terrain.TileSize.Y
                If GenerateTerrainTiles(X, Y).Node.GetClearance < 1 Then

                ElseIf GenerateTerrainTiles(X, Y).Node.GetClearance = 1 Then
                    If SpaceCount >= 3 And SpaceCount <= 9 Then
                        PossibleGateways(PossibleGatewayCount).StartPos.X = X
                        PossibleGateways(PossibleGatewayCount).StartPos.Y = Y - SpaceCount
                        PossibleGateways(PossibleGatewayCount).Length = SpaceCount + 1
                        PossibleGateways(PossibleGatewayCount).IsVertical = True
                        PossibleGateways(PossibleGatewayCount).MiddlePos.X = PossibleGateways(PossibleGatewayCount).StartPos.X * 128
                        PossibleGateways(PossibleGatewayCount).MiddlePos.Y = PossibleGateways(PossibleGatewayCount).StartPos.Y * 128 + PossibleGateways(PossibleGatewayCount).Length * 64
                        PossibleGatewayCount += 1
                    End If
                    SpaceCount = 1
                Else
                    SpaceCount += 1
                End If
                Y += 1
            Loop
        Next

        'add the best gateways

        Dim A As Integer
        Dim Value As Single
        Dim BestValue As Single
        Dim BestNum As Integer
        Dim TileIsGateway(Terrain.TileSize.X - 1, Terrain.TileSize.Y - 1) As Boolean
        Dim Valid As Boolean
        Dim InvalidPos As sXY_int
        Dim InvalidDist As Double

        Do While PossibleGatewayCount > 0
            BestNum = -1
            BestValue = Single.MaxValue
            For A = 0 To PossibleGatewayCount - 1
                'Value = 0.0F
                'For B = 0 To PossibleGatewayCount - 1
                '    Value += GetDist(PossibleGateways(A).MiddlePos, PossibleGateways(B).MiddlePos)
                'Next
                Value = PossibleGateways(A).Length
                If Value < BestValue Then
                    BestValue = Value
                    BestNum = A
                End If
            Next
            ReDim Preserve Gateways(GatewayCount)
            Gateways(GatewayCount).PosA = PossibleGateways(BestNum).StartPos
            If PossibleGateways(BestNum).IsVertical Then
                Gateways(GatewayCount).PosB = New sXY_int(PossibleGateways(BestNum).StartPos.X, PossibleGateways(BestNum).StartPos.Y + PossibleGateways(BestNum).Length - 1)
                For Y = PossibleGateways(BestNum).StartPos.Y To PossibleGateways(BestNum).StartPos.Y + PossibleGateways(BestNum).Length - 1
                    TileIsGateway(PossibleGateways(BestNum).StartPos.X, Y) = True
                Next
            Else
                Gateways(GatewayCount).PosB = New sXY_int(PossibleGateways(BestNum).StartPos.X + PossibleGateways(BestNum).Length - 1, PossibleGateways(BestNum).StartPos.Y)
                For X = PossibleGateways(BestNum).StartPos.X To PossibleGateways(BestNum).StartPos.X + PossibleGateways(BestNum).Length - 1
                    TileIsGateway(X, PossibleGateways(BestNum).StartPos.Y) = True
                Next
            End If
            GatewayCount += 1
            InvalidPos = PossibleGateways(BestNum).MiddlePos
            InvalidDist = PossibleGateways(BestNum).Length * 128
            A = 0
            Do While A < PossibleGatewayCount
                Valid = True
                If PossibleGateways(A).IsVertical Then
                    For Y = PossibleGateways(A).StartPos.Y To PossibleGateways(A).StartPos.Y + PossibleGateways(A).Length - 1
                        If TileIsGateway(PossibleGateways(A).StartPos.X, Y) Then
                            Valid = False
                            Exit For
                        End If
                    Next
                Else
                    For X = PossibleGateways(A).StartPos.X To PossibleGateways(A).StartPos.X + PossibleGateways(A).Length - 1
                        If TileIsGateway(X, PossibleGateways(A).StartPos.Y) Then
                            Valid = False
                            Exit For
                        End If
                    Next
                End If
                If Valid Then
                    If GetDist_XY_int(InvalidPos, PossibleGateways(A).MiddlePos) < InvalidDist Then
                        Valid = False
                    End If
                End If
                If Not Valid Then
                    PossibleGatewayCount -= 1
                    If A <> PossibleGatewayCount Then
                        PossibleGateways(A) = PossibleGateways(PossibleGatewayCount)
                    End If
                Else
                    A += 1
                End If
            Loop
        Loop

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Private Function MakePassageNodes(ByVal Pos As sXY_int, ByVal IsOnBorder As Boolean, ByRef Args As sGenerateLayoutArgs) As Boolean
        Dim A As Integer
        Dim B As Integer
        Dim tmpNode As clsPassageNode
        Dim RatioPos As sXY_dbl
        Dim RotatedPos As sXY_dbl
        Dim SymmetrySize As sXY_dbl
        Dim Positions(3) As sXY_int

        SymmetrySize.X = Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X
        SymmetrySize.Y = Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y

        RatioPos.X = Pos.X / SymmetrySize.X
        RatioPos.Y = Pos.Y / SymmetrySize.Y

        For A = 0 To Args.SymmetryBlockCount - 1
            RotatedPos = GetTileRotatedPos_dbl(Args.SymmetryBlocks(A).Orientation, RatioPos)
            Positions(A).X = CInt((Args.SymmetryBlocks(A).XYNum.X + RotatedPos.X) * SymmetrySize.X)
            Positions(A).Y = CInt((Args.SymmetryBlocks(A).XYNum.Y + RotatedPos.Y) * SymmetrySize.Y)
            For B = 0 To A - 1
                If GetDist_XY_int(Positions(A), Positions(B)) < Args.NodeScale * TerrainGridSpacing * 2.0F Then
                    Return False
                End If
            Next
        Next

        For A = 0 To Args.SymmetryBlockCount - 1
            tmpNode = New clsPassageNode
            PassageNodes(A, PassageNodeCount) = tmpNode
            tmpNode.Num = PassageNodeCount
            tmpNode.MirrorNum = A
            tmpNode.Pos = Positions(A)
            tmpNode.IsOnBorder = IsOnBorder
        Next
        PassageNodeCount += 1

        Return True
    End Function

    'Public Structure sGenerateSide
    '    Public Node As PathfinderNode
    '    Public TopLink As PathfinderNode
    '    Public TopRightLink As PathfinderNode
    '    Public RightLink As PathfinderNode
    '    Public BottomRightLink As PathfinderNode
    '    Public BottomLink As PathfinderNode
    '    Public BottomLeftLink As PathfinderNode
    '    Public LeftLink As PathfinderNode
    '    Public TopLeftLink As PathfinderNode
    'End Structure

    'Public Structure sGenerateRoadsArgs
    '    Public RoadType As sPainter.clsRoad
    '    Public MaxAlt As Byte
    '    Public Terrain As sPainter.clsTerrain
    '    Public MinLength As Integer
    '    Public MaxLength As Integer
    '    Public MinTurnRatio As Single
    '    Public MaxTurnRatio As Single
    '    Public Quantity As Integer
    'End Structure

    'Public Sub GenerateRoads(ByVal Args As sGenerateRoadsArgs)
    '    Dim RoadPathMap As New PathfinderNetwork
    '    Dim tmpNode As PathfinderNode
    '    Dim NodeTag As clsNodeTag


    '    For Y = 0 To Terrain.Size.Y
    '        For X = 0 To Terrain.Size.X
    '            GenerateTerrainVertex(X, Y).Node = New PathfinderNode(RoadPathMap)
    '            NodeTag = New clsNodeTag
    '            NodeTag.Pos = New sXY_int(X * 128, Y * 128)
    '            GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
    '        Next
    '    Next
    '    For Y = 0 To Terrain.Size.Y
    '        For X = 0 To Terrain.Size.X
    '            tmpNodeA = GenerateTerrainVertex(X, Y).Node
    '            If X > 0 Then
    '                tmpNodeB = GenerateTerrainVertex(X - 1, Y).Node
    '                GenerateTerrainVertex(X, Y).LeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '            End If
    '            If Y > 0 Then
    '                If X > 0 Then
    '                    tmpNodeB = GenerateTerrainVertex(X - 1, Y - 1).Node
    '                    GenerateTerrainVertex(X, Y).TopLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '                tmpNodeB = GenerateTerrainVertex(X, Y - 1).Node
    '                GenerateTerrainVertex(X, Y).TopLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                If X < Terrain.Size.X Then
    '                    tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
    '                    GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '            End If
    '            If X < Terrain.Size.X Then
    '                tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
    '                GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '            End If
    '            If Y < Terrain.Size.Y Then
    '                If X > 0 Then
    '                    tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
    '                    GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '                tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
    '                GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                If X < Terrain.Size.X Then
    '                    tmpNodeB = GenerateTerrainVertex(X + 1, Y + 1).Node
    '                    GenerateTerrainVertex(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '            End If
    '        Next
    '    Next

    '    RoadPathMap.LargeArraysResize()
    '    RoadPathMap.FindCalc()

    '    RoadPathMap.Deallocate()
    'End Sub

    Public Sub ClearGenerator(ByVal SymmetryBlockCount As Integer)

        Dim A As Integer
        Dim B As Integer

        TilePathMap.Deallocate()
        TilePathMap = Nothing
        VertexPathMap.Deallocate()
        VertexPathMap = Nothing

        Erase GenerateTerrainVertex
        Erase GenerateTerrainTiles

        For A = 0 To ConnectionCount - 1
            Connections(A).PassageNodeA = Nothing
            Connections(A).PassageNodeB = Nothing
            Erase Connections(A).Reflections
        Next
        Erase Connections

        For A = 0 To PassageNodeCount - 1
            For B = 0 To SymmetryBlockCount - 1
                Erase PassageNodes(B, A).Connections
            Next
        Next
        Erase PassageNodes
        Erase PassageNodeDists
        Erase Nearests
        Erase BasePassageNodes
    End Sub

    Public Sub New(ByVal TileSize As sXY_int)
        MyBase.New(TileSize)

    End Sub
End Class