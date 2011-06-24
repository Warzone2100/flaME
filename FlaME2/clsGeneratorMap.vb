Public Class clsGeneratorMap
    Inherits clsMap

    Public TilePathMap As PathfinderNetwork
    Public VertexPathMap As PathfinderNetwork

    Public GenerateTileset As clsGeneratorTileset

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
    Public GenerateTerrainVertex(,) As sGenerateTerrainVertex

    Structure sGenerateTerrainTile
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
        Public OilTolerance As Single
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
        Dim TagA As clsNodeTag = NodeA.Tag
        Dim TagB As clsNodeTag = NodeB.Tag

        Return CSng(GetDist(TagA.Pos, TagB.Pos))
    End Function

    Public Sub CalcNodePos(ByVal Node As PathfinderNode, ByRef Pos As sXY_dbl, ByRef SampleCount As Integer)

        If Node.GetLayer.GetNetwork_LayerNum = 0 Then
            Dim NodeTag As clsNodeTag
            NodeTag = Node.Tag
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
        GenerateLayout.Success = False
        GenerateLayout.Problem = ""

        Dim X As Integer
        Dim Y As Integer

        _PlayerCount = Args.PlayerCount * Args.SymmetryBlockCount
        TerrainSize = Args.Size
        _SymmetryBlockCount = Args.SymmetryBlockCount

        Dim SymmetrySize As sXY_dbl

        SymmetrySize.X = TerrainSize.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X
        SymmetrySize.Y = TerrainSize.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y

        ReDim TerrainTiles(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim TerrainVertex(TerrainSize.X, TerrainSize.Y)
        ReDim GenerateTerrainTiles(TerrainSize.X - 1, TerrainSize.Y - 1)
        ReDim GenerateTerrainVertex(TerrainSize.X, TerrainSize.Y)

        Dim tmpNodeA As PathfinderNode
        Dim tmpNodeB As PathfinderNode
        Dim NodeTag As clsNodeTag

        VertexPathMap = New PathfinderNetwork

        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                GenerateTerrainVertex(X, Y).Node = New PathfinderNode(VertexPathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int(X * 128, Y * 128)
                GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
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
                    If X < TerrainSize.X Then
                        tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
                        GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < TerrainSize.X Then
                    tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
                    GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < TerrainSize.Y Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
                        GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
                    GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < TerrainSize.X Then
                        tmpNodeB = GenerateTerrainVertex(X + 1, Y + 1).Node
                        GenerateTerrainVertex(X, Y).BottomRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
            Next
        Next

        VertexPathMap.LargeArraysResize()
        VertexPathMap.FindCalc()

        TilePathMap = New PathfinderNetwork

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                GenerateTerrainTiles(X, Y).Node = New PathfinderNode(TilePathMap)
                NodeTag = New clsNodeTag
                NodeTag.Pos = New sXY_int((X + 0.5#) * 128.0#, (Y + 0.5#) * 128.0#)
                GenerateTerrainTiles(X, Y).Node.Tag = NodeTag
            Next
        Next
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
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
                    If X < TerrainSize.X - 1 Then
                        tmpNodeB = GenerateTerrainTiles(X + 1, Y - 1).Node
                        GenerateTerrainTiles(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                End If
                If X < TerrainSize.X - 1 Then
                    tmpNodeB = GenerateTerrainTiles(X + 1, Y).Node
                    GenerateTerrainTiles(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                End If
                If Y < TerrainSize.Y - 1 Then
                    If X > 0 Then
                        tmpNodeB = GenerateTerrainTiles(X - 1, Y + 1).Node
                        GenerateTerrainTiles(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    End If
                    tmpNodeB = GenerateTerrainTiles(X, Y + 1).Node
                    GenerateTerrainTiles(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
                    If X < TerrainSize.X - 1 Then
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
        Dim BaseNodeLevel(BaseLayer.GetNodeCount - 1) As Single
        Dim JitterLayer As PathfinderLayer = VertexPathMap.GetNodeLayer(Args.JitterScale)
        A = JitterLayer.GetNodeCount - 1
        Dim NodeLevel(A) As Integer

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
                NodeTag.Pos.X = XY_dbl.X / C
                NodeTag.Pos.Y = XY_dbl.Y / C
                tmpNodeA.Tag = NodeTag
            Next
        End If

        'create passage nodes

        Dim PassageRadius As Integer = 128.0F * Args.NodeScale
        Dim MaxLikelyPassageNodeCount As Integer
        MaxLikelyPassageNodeCount = Math.Ceiling(2.0# * Args.Size.X * 128 * Args.Size.Y * 128 / (Math.PI * PassageRadius * PassageRadius))

        ReDim PassageNodes(Args.SymmetryBlockCount - 1, MaxLikelyPassageNodeCount - 1)
        Dim LoopCount As Integer
        Dim EdgeOffset As Integer = 4 * 128
        Dim PointIsValid As Boolean
        Dim EdgeSections As sXY_int
        Dim EdgeSectionSize As sXY_dbl
        Dim NewPointPos As sXY_int

        If Args.SymmetryBlockCountXY.X = 1 Then
            EdgeSections.X = Int((Args.Size.X * TerrainGridSpacing - EdgeOffset * 2.0#) / (Args.NodeScale * TerrainGridSpacing * 2.0F))
            EdgeSectionSize.X = (Args.Size.X * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.X
            EdgeSections.X -= 1
        Else
            EdgeSections.X = Int((Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#)
            EdgeSectionSize.X = (Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Int((Args.Size.X * TerrainGridSpacing / Args.SymmetryBlockCountXY.X - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If
        If Args.SymmetryBlockCountXY.Y = 1 Then
            EdgeSections.Y = Int((Args.Size.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / (Args.NodeScale * TerrainGridSpacing * 2.0F))
            EdgeSectionSize.Y = (Args.Size.Y * TerrainGridSpacing - EdgeOffset * 2.0#) / EdgeSections.Y
            EdgeSections.Y -= 1
        Else
            EdgeSections.Y = Int((Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#)
            EdgeSectionSize.Y = (Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Int((Args.Size.Y * TerrainGridSpacing / Args.SymmetryBlockCountXY.Y - EdgeOffset) / (Args.NodeScale * TerrainGridSpacing * 2.0F) - 0.5#) + 0.5#)
        End If

        PassageNodeCount = 0
        For Y = 1 To EdgeSections.Y
            If Not MakePassageNodes(New sXY_int(EdgeOffset, EdgeOffset + Y * EdgeSectionSize.Y), True, Args) Then
                GenerateLayout.Problem = "Error; Bad border node."
                Exit Function
            End If
            If Args.SymmetryBlockCountXY.X = 1 Then
                If Not MakePassageNodes(New sXY_int(Args.Size.X * TerrainGridSpacing - EdgeOffset, EdgeOffset + Y * EdgeSectionSize.Y), True, Args) Then
                    GenerateLayout.Problem = "Error; Bad border node."
                    Exit Function
                End If
            End If
        Next
        For X = 1 To EdgeSections.X
            If Not MakePassageNodes(New sXY_int(EdgeOffset + X * EdgeSectionSize.X, EdgeOffset), True, Args) Then
                GenerateLayout.Problem = "Error; Bad border node."
                Exit Function
            End If
            If Args.SymmetryBlockCountXY.Y = 1 Then
                If Not MakePassageNodes(New sXY_int(EdgeOffset + X * EdgeSectionSize.X, Args.Size.Y * TerrainGridSpacing - EdgeOffset), True, Args) Then
                    GenerateLayout.Problem = "Error; Bad border node."
                    Exit Function
                End If
            End If
        Next
        Do
            LoopCount = 0
            Do
                PointIsValid = True
                If Args.SymmetryBlockCountXY.X = 1 Then
                    NewPointPos.X = EdgeOffset + Int(Rnd() * (SymmetrySize.X - EdgeOffset * 2.0# + 1.0#))
                Else
                    NewPointPos.X = EdgeOffset + Int(Rnd() * (SymmetrySize.X - EdgeOffset + 1.0#))
                End If
                If Args.SymmetryBlockCountXY.Y = 1 Then
                    NewPointPos.Y = EdgeOffset + Int(Rnd() * (SymmetrySize.Y - EdgeOffset * 2.0# + 1.0#))
                Else
                    NewPointPos.Y = EdgeOffset + Int(Rnd() * (SymmetrySize.Y - EdgeOffset + 1.0#))
                End If
                For A = 0 To PassageNodeCount - 1
                    For B = 0 To Args.SymmetryBlockCount - 1
                        If GetDist(PassageNodes(B, A).Pos, NewPointPos) < PassageRadius * 2 Then
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

        Dim AngleA As Single
        Dim AngleB As Single
        Dim AngleC As Single

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
        Dim PassageNodesMinLevel(PassageNodeCount - 1) As Integer
        Dim PassageNodesMaxLevel(PassageNodeCount - 1) As Integer

        For A = 0 To PassageNodeCount - 1
            PassageNodesMinLevel(A) = 0
            PassageNodesMaxLevel(A) = Args.LevelCount - 1
        Next

        'create bases
        ReDim BasePassageNodes(_PlayerCount - 1)
        For B = 0 To Args.PlayerCount - 1
            BestDist = Single.MaxValue
            tmpPassageNodeB = Nothing
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodes(0, A)
                If Not tmpPassageNodeA.IsOnBorder Then
                    Dist = GetDist(tmpPassageNodeA.Pos, Args.PlayerBasePos(B))
                    If Dist < BestDist Then
                        BestDist = Dist
                        tmpPassageNodeB = tmpPassageNodeA
                    End If
                End If
            Next

            If Args.BaseLevel < 0 Then
                D = Int(Rnd() * Args.LevelCount)
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
            GenerateLayout.Problem = "All height level behaviors are zero"
            Exit Function
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
                    If PassageNodesMinLevel(tmpPassageNodeA.Connections(B).GetOther.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If CanDoFlatsAroundWater And ((WaterCount = 0 And WaterSpawns < Args.WaterSpawnQuantity) Or (WaterCount = 1 And Args.TotalWaterQuantity - TotalWater > Args.WaterSpawnQuantity - WaterSpawns)) And PassageNodesMinLevel(tmpPassageNodeA.Num) = 0 And TotalWater < Args.TotalWaterQuantity Then
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
                    ElseIf PassageNodesMinLevel(tmpPassageNodeA.Num) > PassageNodesMaxLevel(tmpPassageNodeA.Num) Then
                        F = PassageNodesMinLevel(tmpPassageNodeA.Num) - PassageNodesMaxLevel(tmpPassageNodeA.Num)
                        Math.DivRem(F, 2, D)
                        If D = 1 Then
                            E = PassageNodesMinLevel(tmpPassageNodeA.Num) + CInt((F - 1) / 2.0F) + CInt(Int(Rnd() * 2.0F))
                        Else
                            E = PassageNodesMinLevel(tmpPassageNodeA.Num) + F / 2.0F
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
                            RandomAction = Int(Rnd() * ActionTotal)
                            If RandomAction < FlatCutoff Then
                                'extend the level that surrounds this most
                                C = 0
                                EligableCount = 0
                                For B = PassageNodesMinLevel(tmpPassageNodeA.Num) To PassageNodesMaxLevel(tmpPassageNodeA.Num)
                                    If LevelCounts(B) > C Then
                                        C = LevelCounts(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf LevelCounts(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(Int(Rnd() * EligableCount))
                            ElseIf RandomAction < PassageCutoff Then
                                'extend any level that surrounds only once, or closest to
                                EligableCount = 0
                                For B = PassageNodesMinLevel(tmpPassageNodeA.Num) To PassageNodesMaxLevel(tmpPassageNodeA.Num)
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
                                    For B = PassageNodesMinLevel(tmpPassageNodeA.Num) To PassageNodesMaxLevel(tmpPassageNodeA.Num)
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
                                BestNum = Eligables(Int(Rnd() * EligableCount))
                            ElseIf RandomAction < VariationCutoff Then
                                'extend the most uncommon surrounding
                                C = Integer.MaxValue
                                EligableCount = 0
                                For B = PassageNodesMinLevel(tmpPassageNodeA.Num) To PassageNodesMaxLevel(tmpPassageNodeA.Num)
                                    If LevelCounts(B) < C Then
                                        C = LevelCounts(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf LevelCounts(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(Int(Rnd() * EligableCount))
                            ElseIf RandomAction < RandomCutoff Then
                                BestNum = PassageNodesMinLevel(tmpPassageNodeA.Num) + CInt(Int(Rnd() * (PassageNodesMaxLevel(tmpPassageNodeA.Num) - PassageNodesMinLevel(tmpPassageNodeA.Num) + 1)))
                            ElseIf RandomAction < EqualizeCutoff Then
                                'switch to the most uncommon level on the map
                                C = Integer.MaxValue
                                EligableCount = 0
                                For B = PassageNodesMinLevel(tmpPassageNodeA.Num) To PassageNodesMaxLevel(tmpPassageNodeA.Num)
                                    If MapLevelCount(B) < C Then
                                        C = MapLevelCount(B)
                                        Eligables(0) = B
                                        EligableCount = 1
                                    ElseIf MapLevelCount(B) = C Then
                                        Eligables(EligableCount) = B
                                        EligableCount += 1
                                    End If
                                Next
                                BestNum = Eligables(Int(Rnd() * EligableCount))
                            Else
                                GenerateLayout.Problem = "Error; Random number out of range."
                                Exit Function
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
                    GenerateLayout.Problem = "Error; Border has had its height set."
                    Exit Function
                End If
                'If tmpPassageNodeA.ConnectionCount <> 1 Then
                '    GenerateLayout.Problem = "Error; Border has incorrect connections."
                '    Exit Function
                'End If
                tmpPassageNodeC = Nothing
                CanDoFlatsAroundWater = True
                For B = 0 To tmpPassageNodeA.ConnectionCount - 1
                    tmpPassageNodeB = tmpPassageNodeA.Connections(B).GetOther
                    If Not tmpPassageNodeB.IsOnBorder And PassageNodesMinLevel(tmpPassageNodeA.Num) <= tmpPassageNodeB.Level And PassageNodesMaxLevel(tmpPassageNodeA.Num) >= tmpPassageNodeB.Level Then
                        tmpPassageNodeC = tmpPassageNodeB
                    End If
                    If PassageNodesMinLevel(tmpPassageNodeB.Num) > 0 Then
                        CanDoFlatsAroundWater = False
                    End If
                Next
                If tmpPassageNodeC Is Nothing Then
                    GenerateLayout.Problem = "Error; No connection for border node"
                    Exit Function
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
            NodeTag = JitterLayer.GetNode(A).Tag
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
                    Dist = GetDist(XY_int, NodeTag.Pos)
                    If Dist < BestDist Then
                        BestDist = Dist
                        If GetDist(NodeTag.Pos, Connections(B).PassageNodeA.Pos) <= GetDist(NodeTag.Pos, Connections(B).PassageNodeB.Pos) Then
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
                    Dist = GetDist(NodeTag.Pos, PassageNodes(D, C).Pos)
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
                GenerateLayout.Problem = "Error; Node height is not set."
                Exit Function
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

        Dim tmpConnectionB As clsConnection
        Dim tmpPathConnection(3) As PathfinderConnection
        Dim Value As Single
        Dim NodeConnectedness(PassageNodeCount - 1) As Single
        Dim PassageNodeUpdated(PassageNodeCount - 1) As Boolean
        Dim BestDistB As Single
        Dim PassageNodeVisited(Args.SymmetryBlockCount - 1, PassageNodeCount - 1) As Boolean
        Dim BaseDist As Single
        Dim RampDist As Single

        For A = 0 To PassageNodeCount - 1
            NodeConnectedness(A) = 0.0F
            For B = 0 To PassageNodeCount - 1
                For C = 0 To Args.SymmetryBlockCount - 1
                    PassageNodeVisited(C, B) = False
                Next
            Next
            UpdateNodeConnectedness(PassageNodes(0, A), PassageNodes(0, A), NodeConnectedness, PassageNodeVisited, PassageNodePathMap, PassageNodePathNodes)
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
                    AngleA = GetAngle(XY_int)
                    For C = 0 To Connections(B).PassageNodeA.ConnectionCount - 1
                        tmpConnectionB = Connections(B).PassageNodeA.Connections(C).Connection
                        If tmpConnectionB IsNot Connections(B) Then
                            If tmpConnectionB.IsRamp Then
                                XY_int.X = tmpConnectionB.PassageNodeB.Pos.X - tmpConnectionB.PassageNodeA.Pos.X
                                XY_int.Y = tmpConnectionB.PassageNodeB.Pos.Y - tmpConnectionB.PassageNodeA.Pos.Y
                                AngleB = GetAngle(XY_int)
                                AngleC = AngleClamp(AngleA - AngleB)
                                If tmpConnectionB.PassageNodeA IsNot Connections(B).PassageNodeA Then
                                    AngleC = AngleClamp(AngleC - Pi)
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
                                    AngleB = GetAngle(XY_int)
                                    AngleC = AngleClamp(AngleA - AngleB)
                                    If tmpConnectionB.PassageNodeA Is Connections(B).PassageNodeB Then
                                        AngleC = AngleClamp(AngleC - Pi)
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
                            XY_int.X = (Connections(B).PassageNodeA.Pos.X + Connections(B).PassageNodeB.Pos.X) / 2.0#
                            XY_int.Y = (Connections(B).PassageNodeA.Pos.Y + Connections(B).PassageNodeB.Pos.Y) / 2.0#
                            For E = 0 To Args.PlayerCount - 1
                                Dist = GetDist(BasePassageNodes(E).Pos, XY_int)
                                If Dist < BaseDist Then
                                    BaseDist = Dist
                                End If
                            Next
                            RampDist = Math.Max(Args.MaxDisconnectionDist * Args.RampBase ^ (BaseDist / 1024.0#), 1.0F)
                            If ResultPaths Is Nothing Then
                                Value = NodeConnectedness(Connections(B).PassageNodeA.Num) + NodeConnectedness(Connections(B).PassageNodeB.Num)
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
                                GenerateLayout.Problem = "Error; Invalid number of routes returned."
                                Exit Function
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
                BestNum = Int(Rnd() * PossibleRampCount)
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
                    PassageNodeUpdated(E) = False
                Next
                If PossibleRamps(BestNum).PassageNodeA.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(PossibleRamps(BestNum).PassageNodeA, NodeConnectedness, PassageNodeUpdated, PassageNodeVisited, Args.SymmetryBlockCount, PassageNodePathMap, PassageNodePathNodes)
                ElseIf PossibleRamps(BestNum).PassageNodeB.MirrorNum = 0 Then
                    UpdateNetworkConnectedness(PossibleRamps(BestNum).PassageNodeB, NodeConnectedness, PassageNodeUpdated, PassageNodeVisited, Args.SymmetryBlockCount, PassageNodePathMap, PassageNodePathNodes)
                Else
                    GenerateLayout.Problem = "Error; Initial ramp not in area 0."
                    Exit Function
                End If
            Else
                Exit Do
            End If
        Loop

        Dim RampLength As Integer
        Dim MinRampLength As Integer = CInt(LevelHeight * HeightMultiplier * 2.0#) + 128

        For B = 0 To ConnectionCount - 1
            For A = 0 To JitterLayer.GetNodeCount - 1
                RampLength = Math.Max(CInt(GetDist(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos)) * 0.75#, MinRampLength * Math.Abs(Connections(B).PassageNodeA.Level - Connections(B).PassageNodeB.Level))
                If Connections(B).IsRamp Then
                    NodeTag = JitterLayer.GetNode(A).Tag
                    XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, NodeTag.Pos)
                    Dist = GetDist(XY_int, NodeTag.Pos)
                    If Dist < RampLength * 2.0F Then
                        SetBaseLevelRamp(JitterLayer.GetNode(A), Connections(B), BaseNodeLevel, RampLength)
                    End If
                End If
            Next
        Next

        For A = 0 To BaseLayer.GetNodeCount - 1
            NodeTag = BaseLayer.GetNode(A).Tag
            TerrainVertex(NodeTag.Pos.X / 128.0F, NodeTag.Pos.Y / 128.0F).Height = BaseNodeLevel(A) * LevelHeight
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
                                GenerateLayout.Problem = "Map is not all connected."
                                Exit Function
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
        Dim PlayerOilScore(Args.PlayerCount - 1) As Single
        Dim ExtraOilCount As Integer
        Dim BestValue As Single
        Dim BestPlayerOilScoreAddition(Args.PlayerCount - 1) As Single
        Dim MaxBestNodeCount As Integer
        MaxBestNodeCount = 1
        For A = 0 To Args.OilAtATime - 1
            MaxBestNodeCount *= PassageNodeCount
        Next
        Dim BestNodes(,) As clsPassageNode
        Dim BestNodeScores() As Single
        Dim BestNodeCount As Integer
        Dim OilClusterSizes(Args.OilAtATime - 1) As Integer
        Dim OilNodes(Args.OilAtATime - 1) As clsPassageNode
        Dim LoopNum As Integer

        'balanced oil
        Do While ExtraOilCount < Args.ExtraOilCount
            'place oil farthest away from other oil and where it best balances the player oil score
            For A = 0 To Args.OilAtATime - 1
                OilClusterSizes(A) = Math.Min(Args.ExtraOilClusterSizeMin + CInt(Int(Rnd() * (Args.ExtraOilClusterSizeMax - Args.ExtraOilClusterSizeMin + 1))), Math.Max(CInt(Math.Ceiling((Args.ExtraOilCount - ExtraOilCount) / Args.SymmetryBlockCount)), 1))
            Next
            BestValue = Single.MaxValue
            LoopNum = 0
            ReDim BestNodes(MaxBestNodeCount - 1, Args.OilAtATime - 1)
            ReDim BestNodeScores(MaxBestNodeCount - 1)
            BestNodeCount = 0
            OilBalanceLoop(OilNodes, OilClusterSizes, BestNodes, BestNodeScores, BestNodeCount, BestPlayerOilScoreAddition, BestValue, PlayerOilScore, LoopNum, Args)

            BestValue = Single.MaxValue
            For A = 0 To BestNodeCount - 1
                If BestNodeScores(A) < BestValue Then
                    BestValue = BestNodeScores(A)
                End If
            Next
            If BestValue < Single.MaxValue Then
                C = 0
                For A = 0 To BestNodeCount - 1
                    If BestValue >= Args.OilTolerance * BestNodeScores(A) Then
                        For B = 0 To Args.OilAtATime - 1
                            BestNodes(C, B) = BestNodes(A, B)
                        Next
                        C += 1
                    End If
                Next
                D = Int(Rnd() * C)
                For B = 0 To Args.OilAtATime - 1
                    For A = 0 To Args.SymmetryBlockCount - 1
                        PassageNodes(A, BestNodes(D, B).Num).OilCount += OilClusterSizes(B)
                    Next
                    ExtraOilCount += OilClusterSizes(B) * Args.SymmetryBlockCount
                Next
                For A = 0 To Args.PlayerCount - 1
                    PlayerOilScore(A) += BestPlayerOilScoreAddition(A)
                Next
            Else
                GenerateLayout.Problem = "Map is too small for that number of oil clusters"
                Exit Function
            End If
        Loop

        'base oil
        For A = 0 To _PlayerCount - 1
            BasePassageNodes(A).OilCount += Args.BaseOilCount
        Next

        PassageNodePathMap.Deallocate()

        LevelCount = Args.LevelCount

        GenerateLayout.Success = True
    End Function

    Private Sub PassageNodesMinLevelSet(ByVal PassageNode As clsPassageNode, ByRef PassageNodesMinLevel() As Integer, ByVal Level As Integer, ByVal LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level > PassageNodesMinLevel(PassageNode.Num) Then
            PassageNodesMinLevel(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If tmpPassageNode.MirrorNum = 0 Then
                    PassageNodesMinLevelSet(tmpPassageNode, PassageNodesMinLevel, Level - LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Sub PassageNodesMaxLevelSet(ByVal PassageNode As clsPassageNode, ByRef PassageNodesMaxLevel() As Integer, ByVal Level As Integer, ByVal LevelChange As Integer)
        Dim A As Integer
        Dim tmpPassageNode As clsPassageNode

        If Level < PassageNodesMaxLevel(PassageNode.Num) Then
            PassageNodesMaxLevel(PassageNode.Num) = Level
            For A = 0 To PassageNode.ConnectionCount - 1
                tmpPassageNode = PassageNode.Connections(A).GetOther
                If tmpPassageNode.MirrorNum = 0 Then
                    PassageNodesMaxLevelSet(tmpPassageNode, PassageNodesMaxLevel, Level + LevelChange, LevelChange)
                End If
            Next
        End If
    End Sub

    Private Sub UpdateNodeConnectedness(ByVal OriginalNode As clsPassageNode, ByVal PassageNode As clsPassageNode, ByRef NodeConnectedness() As Single, ByRef VisitedB(,) As Boolean, ByVal PassageNodePathMap As PathfinderNetwork, ByRef PassageNodePathNodes(,) As PathfinderNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim PassableCount As Integer

        VisitedB(PassageNode.MirrorNum, PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not VisitedB(tmpOtherNode.MirrorNum, tmpOtherNode.Num) Then
                    UpdateNodeConnectedness(OriginalNode, tmpOtherNode, NodeConnectedness, VisitedB, PassageNodePathMap, PassageNodePathNodes)
                End If
                PassableCount += 1
            End If
        Next

        Dim Paths() As PathfinderNetwork.PathList
        Dim StartNodes(0) As PathfinderNode
        StartNodes(0) = PassageNodePathNodes(0, OriginalNode.Num)
        Paths = PassageNodePathMap.GetPath(StartNodes, PassageNodePathNodes(PassageNode.MirrorNum, PassageNode.Num), -1, 0)
        NodeConnectedness(OriginalNode.Num) += PassableCount * 0.999# ^ Paths(0).Paths(0).Value
    End Sub

    Private Sub UpdateNetworkConnectedness(ByVal PassageNode As clsPassageNode, ByRef NodeConnectedness() As Single, ByRef Visited() As Boolean, ByRef VisitedB(,) As Boolean, ByVal SymmetryBlockCount As Integer, ByVal PassageNodePathMap As PathfinderNetwork, ByRef PassageNodePathNodes(,) As PathfinderNode)
        Dim A As Integer
        Dim tmpConnection As clsConnection
        Dim tmpOtherNode As clsPassageNode
        Dim B As Integer
        Dim C As Integer

        Visited(PassageNode.Num) = True

        For A = 0 To PassageNode.ConnectionCount - 1
            tmpConnection = PassageNode.Connections(A).Connection
            If Not (tmpConnection.PassageNodeA.IsOnBorder Or tmpConnection.PassageNodeB.IsOnBorder Or tmpConnection.PassageNodeA.IsWater Or tmpConnection.PassageNodeB.IsWater) And (tmpConnection.IsRamp Or tmpConnection.PassageNodeA.Level = tmpConnection.PassageNodeB.Level) Then
                tmpOtherNode = PassageNode.Connections(A).GetOther
                If Not Visited(tmpOtherNode.Num) And tmpOtherNode.MirrorNum = 0 Then
                    For B = 0 To PassageNodeCount - 1
                        For C = 0 To SymmetryBlockCount - 1
                            VisitedB(C, B) = False
                        Next
                    Next
                    UpdateNodeConnectedness(PassageNode, PassageNode, NodeConnectedness, VisitedB, PassageNodePathMap, PassageNodePathNodes)
                End If
            End If
        Next
    End Sub

    Private Sub OilBalanceLoop(ByRef OilNodes() As clsPassageNode, ByRef OilClusterSizes() As Integer, ByRef BestNodes(,) As clsPassageNode, ByRef BestNodeScores() As Single, ByRef BestNodeCount As Integer, ByRef BestPlayerScoreAddition() As Single, ByRef BestValue As Single, ByRef PlayerOilScore() As Single, ByVal LoopNum As Integer, ByRef GenerateLayoutArgs As sGenerateLayoutArgs)
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim NextLoopNum As Integer = LoopNum + 1
        Dim tmpPassageNodeA As clsPassageNode

        If NextLoopNum < GenerateLayoutArgs.OilAtATime Then
            For A = 0 To PassageNodeCount - 1
                tmpPassageNodeA = PassageNodes(0, A)
                For B = 0 To _PlayerCount - 1
                    If BasePassageNodes(B) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                For C = 0 To LoopNum - 1
                    If OilNodes(C) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                If tmpPassageNodeA.OilCount = 0 And Not tmpPassageNodeA.IsOnBorder And B = _PlayerCount And C = LoopNum And Not tmpPassageNodeA.IsWater Then
                    OilNodes(LoopNum) = tmpPassageNodeA
                    OilBalanceLoop(OilNodes, OilClusterSizes, BestNodes, BestNodeScores, BestNodeCount, BestPlayerScoreAddition, BestValue, PlayerOilScore, NextLoopNum, GenerateLayoutArgs)
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
                    If OilNodes(C) Is tmpPassageNodeA Then
                        Exit For
                    End If
                Next
                If tmpPassageNodeA.OilCount = 0 And Not tmpPassageNodeA.IsOnBorder And B = _PlayerCount And C = LoopNum And Not tmpPassageNodeA.IsWater Then
                    OilNodes(LoopNum) = tmpPassageNodeA
                    OilValueCalc(OilNodes, OilClusterSizes, BestNodes, BestNodeScores, BestNodeCount, BestPlayerScoreAddition, BestValue, PlayerOilScore, GenerateLayoutArgs)
                End If
            Next
        End If
    End Sub

    Private Sub OilValueCalc(ByRef OilNodes() As clsPassageNode, ByRef OilClusterSizes() As Integer, ByRef BestNodes(,) As clsPassageNode, ByRef BestNodeScores() As Single, ByRef BestNodeCount As Integer, ByRef BestPlayerScoreAddition() As Single, ByRef BestValue As Single, ByRef PlayerOilScore() As Single, ByRef GenerateLayoutArgs As sGenerateLayoutArgs)
        Dim OilDistScore As Single
        Dim OilStraightDistScore As Single
        Dim LowestScore As Single
        Dim HighestScore As Single
        Dim EnemiesOilScore As Single
        Dim PlayerOilScoreAddition() As Single
        Dim UnbalancedScore As Single
        Dim sngTemp As Single
        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim E As Integer
        Dim F As Integer
        Dim Value As Single
        Dim BaseOilScore() As Single

        ReDim PlayerOilScoreAddition(GenerateLayoutArgs.PlayerCount - 1)
        ReDim BaseOilScore(GenerateLayoutArgs.PlayerCount - 1)

        OilDistScore = 0.0F
        OilStraightDistScore = 0.0F
        For B = 0 To GenerateLayoutArgs.PlayerCount - 1
            PlayerOilScoreAddition(B) = 0.0F
        Next
        For E = 0 To GenerateLayoutArgs.OilAtATime - 1
            A = OilNodes(E).Num
            'other oil to be placed in the first area
            For F = A + 1 To GenerateLayoutArgs.OilAtATime - 1
                B = OilNodes(F).Num
                OilDistScore += OilClusterSizes(E) * 384.0F * OilClusterSizes(F) / Math.Max(384.0F, PassageNodeDists(0, A, 0, B))
                OilStraightDistScore += OilClusterSizes(E) * 384.0F * OilClusterSizes(F) / Math.Max(384.0F, GetDist(PassageNodes(0, A).Pos, PassageNodes(0, B).Pos))
            Next
            'other oil to be placed in symmetrical areas
            For F = 0 To GenerateLayoutArgs.OilAtATime - 1
                B = OilNodes(F).Num
                For C = 1 To GenerateLayoutArgs.SymmetryBlockCount - 1
                    OilDistScore += OilClusterSizes(E) * 384.0F * OilClusterSizes(F) / Math.Max(384.0F, PassageNodeDists(0, A, C, B))
                    OilStraightDistScore += OilClusterSizes(E) * 384.0F * OilClusterSizes(F) / Math.Max(384.0F, GetDist(PassageNodes(0, A).Pos, PassageNodes(C, B).Pos))
                Next
            Next
            'oil on the map
            For B = 0 To PassageNodeCount - 1
                For C = 0 To GenerateLayoutArgs.SymmetryBlockCount - 1
                    If PassageNodes(C, B).OilCount > 0 Then
                        OilDistScore += OilClusterSizes(E) * 384.0F * PassageNodes(C, B).OilCount / Math.Max(384.0F, PassageNodeDists(0, A, C, B))
                        OilStraightDistScore += OilClusterSizes(E) * 384.0F * PassageNodes(C, B).OilCount / Math.Max(384.0F, GetDist(PassageNodes(0, A).Pos, PassageNodes(C, B).Pos))
                    End If
                Next
            Next
            'extra oil score for players
            For B = 0 To GenerateLayoutArgs.PlayerCount - 1
                BaseOilScore(B) = 0.0F
                For C = 0 To GenerateLayoutArgs.SymmetryBlockCount - 1
                    BaseOilScore(B) += Math.Max(384.0F, PassageNodeDists(0, BasePassageNodes(B).Num, C, A)) * 2.0F + GetDist(BasePassageNodes(B).Pos, PassageNodes(C, A).Pos)
                Next
            Next
            For B = 0 To GenerateLayoutArgs.PlayerCount - 1
                For D = 0 To GenerateLayoutArgs.SymmetryBlockCount - 1
                    EnemiesOilScore = 0.0F
                    For C = 0 To GenerateLayoutArgs.PlayerCount - 1
                        If C <> B Then
                            EnemiesOilScore += BaseOilScore(C)
                        End If
                    Next
                    PlayerOilScoreAddition(B) += OilClusterSizes(E) * EnemiesOilScore / (BaseOilScore(B) * BaseOilScore(B) * _PlayerCount)
                Next
            Next
        Next

        LowestScore = Single.MaxValue
        HighestScore = Single.MinValue
        For B = 0 To GenerateLayoutArgs.PlayerCount - 1
            sngTemp = PlayerOilScore(B) + PlayerOilScoreAddition(B)
            If sngTemp < LowestScore Then
                LowestScore = sngTemp
            End If
            If sngTemp > HighestScore Then
                HighestScore = sngTemp
            End If
        Next
        UnbalancedScore = HighestScore - LowestScore

        C = 0
        For B = 0 To GenerateLayoutArgs.OilAtATime - 1
            C += OilClusterSizes(B)
        Next
        'divide all dists by the number of oil resources placed. does not include other symmetries, since they were never added in, and are exactly the same.
        Value = GenerateLayoutArgs.OilDispersion * (OilDistScore * 3.0F + OilStraightDistScore) / C + UnbalancedScore * 20000.0#
        'If Value < BestValue Then
        '    BestValue = Value
        '    For B = 0 To GenerateLayoutArgs.OilAtATime - 1
        '        BestNodes(B) = OilNodes(B)
        '    Next
        '    For B = 0 To GenerateLayoutArgs.PlayerCount - 1
        '        BestPlayerScoreAddition(B) = PlayerOilScoreAddition(B)
        '    Next
        'End If
        BestNodeScores(BestNodeCount) = Value
        For B = 0 To GenerateLayoutArgs.OilAtATime - 1
            BestNodes(BestNodeCount, B) = OilNodes(B)
        Next
        BestNodeCount += 1
    End Sub

    Private Sub SetBaseLevel(ByVal Node As PathfinderNode, ByVal NewLevel As Integer, ByRef BaseLevel() As Single)

        If Node.GetChildNodeCount = 0 Then
            Dim A As Integer
            Dim Height As Single
            Dim Lowest As Single = NewLevel
            For A = 0 To Node.GetConnectionCount - 1
                Height = BaseLevel(Node.GetConnection(A).GetOtherNode(Node).GetLayer_NodeNum)
                If Height < Lowest Then
                    Lowest = Height
                End If
            Next
            If NewLevel - Lowest > 1.0F Then
                BaseLevel(Node.GetLayer_NodeNum) = Lowest + 1.0F
            Else
                BaseLevel(Node.GetLayer_NodeNum) = NewLevel
            End If
        Else
            Dim A As Integer
            For A = 0 To Node.GetChildNodeCount - 1
                SetBaseLevel(Node.GetChildNode(A), NewLevel, BaseLevel)
            Next
        End If
    End Sub

    Private Sub SetBaseLevelRamp(ByVal Node As PathfinderNode, ByVal Connection As clsConnection, ByRef BaseLevel() As Single, ByVal RampLength As Integer)

        If Node.GetChildNodeCount = 0 Then
            Dim NodeTag As clsNodeTag = Node.Tag
            Dim XY_int As sXY_int = PointGetClosestPosOnLine(Connection.PassageNodeA.Pos, Connection.PassageNodeB.Pos, NodeTag.Pos)
            Dim ConnectionLength As Single = GetDist(Connection.PassageNodeA.Pos, Connection.PassageNodeB.Pos)
            Dim Extra As Single = ConnectionLength - RampLength
            Dim ConnectionPos As Single = GetDist(XY_int, Connection.PassageNodeA.Pos)
            Dim RampPos As Single = Clamp((ConnectionPos - Extra / 2.0F) / RampLength, 0.0F, 1.0F)
            Dim Layer_NodeNum As Integer = Node.GetLayer_NodeNum
            RampPos = 1.0# - (Math.Cos(RampPos * Pi) + 1.0#) / 2.0#
            If RampPos > 0.0F And RampPos < 1.0F Then
                Dim Dist2 As Single = GetDist(NodeTag.Pos, XY_int)
                If Dist2 < 320.0F Then
                    Dim Dist2Factor As Single = 1.0F 'Math.Min(3.0F - 3.0F * Dist2 / 384.0F, 1.0F) 'distance fading
                    If BaseLevel(Layer_NodeNum) = Int(BaseLevel(Layer_NodeNum)) Then
                        BaseLevel(Layer_NodeNum) = BaseLevel(Layer_NodeNum) * (1.0F - Dist2Factor) + (Connection.PassageNodeA.Level * (1.0F - RampPos) + Connection.PassageNodeB.Level * RampPos) * Dist2Factor
                    Else
                        BaseLevel(Layer_NodeNum) = (BaseLevel(Layer_NodeNum) * (2.0F - Dist2Factor) + (Connection.PassageNodeA.Level * (1.0F - RampPos) + Connection.PassageNodeB.Level * RampPos) * Dist2Factor) / 2.0F
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
                    If GetDist(XY_int, PassageNodes(B, A).Pos) < Args.MinConDist Then
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
                    ReflectionCount = Args.Args.SymmetryBlockCount / 2.0#
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
                        ReflectionCount = Args.Args.SymmetryBlockCount / 2.0#
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

        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To TerrainSize.X - 1
                If TerrainTiles(X, Y).Texture.TextureNum >= 0 Then
                    If GenerateTileset.Tileset.Tiles(TerrainTiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Cliff Or GenerateTileset.Tileset.Tiles(TerrainTiles(X, Y).Texture.TextureNum).Default_Type = TileTypeNum_Water Then
                        TileNodeBlock(X, Y)
                    End If
                End If
            Next
        Next
        TilePathMap.FindCalc()
    End Sub

    Public Function GetWaterMap() As clsBooleanMap
        Dim BestDist As Single
        Dim BestIsWater As Boolean
        Dim Pos As sXY_int
        Dim Dist As Single
        Dim B As Integer
        Dim C As Integer
        Dim XY_int As sXY_int
        Dim X As Integer
        Dim Y As Integer

        GetWaterMap = New clsBooleanMap
        GetWaterMap.Blank(TerrainSize.X + 1, TerrainSize.Y + 1)
        For Y = 0 To TerrainSize.Y
            For X = 0 To TerrainSize.X
                BestDist = Single.MaxValue
                Pos = New sXY_int(X * TerrainGridSpacing, Y * TerrainGridSpacing)
                For B = 0 To ConnectionCount - 1
                    'If Not (Connections(B).PassageNodeA.IsOnBorder Or Connections(B).PassageNodeB.IsOnBorder) Then
                    If Connections(B).PassageNodeA.IsWater = Connections(B).PassageNodeB.IsWater Then
                        'only do this if the waters are the same
                        'this is to make sure nodes that are connected are actually connected as water
                        XY_int = PointGetClosestPosOnLine(Connections(B).PassageNodeA.Pos, Connections(B).PassageNodeB.Pos, Pos)
                        Dist = GetDist(XY_int, Pos)
                        If Dist < BestDist Then
                            BestDist = Dist
                            If GetDist(Pos, Connections(B).PassageNodeA.Pos) <= GetDist(Pos, Connections(B).PassageNodeB.Pos) Then
                                BestIsWater = Connections(B).PassageNodeA.IsWater
                            Else
                                BestIsWater = Connections(B).PassageNodeB.IsWater
                            End If
                        End If
                    End If
                Next
                For C = 0 To PassageNodeCount - 1
                    For B = 0 To _SymmetryBlockCount - 1
                        Dist = GetDist(Pos, PassageNodes(B, C).Pos)
                        If Dist < BestDist Then
                            BestDist = Dist
                            BestIsWater = PassageNodes(B, C).IsWater
                        End If
                    Next
                Next
                GetWaterMap.ValueData.Value(Y, X) = BestIsWater
            Next
        Next
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
                tmpNodeTag = tmpNode.Tag
                Dist = GetDist(tmpNodeTag.Pos, Pos)
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
            NodeTag = PosNode.Tag
            If GetDist(Pos, NodeTag.Pos) <= MaxDistFromPos Then

                Dim NewUnit As New clsMap.clsUnit
                NewUnit.Type = Type
                NewUnit.PlayerNum = PlayerNum

                FinalTilePos.X = Int(NodeTag.Pos.X / TerrainGridSpacing)
                FinalTilePos.Y = Int(NodeTag.Pos.Y / TerrainGridSpacing)
                Footprint = Type.GetFootprint
                Math.DivRem(Footprint.X, 2, Remainder)
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.X = NodeTag.Pos.X
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X - TerrainGridSpacing / 2.0#
                    Else
                        NewUnit.Pos.Horizontal.X = NodeTag.Pos.X + TerrainGridSpacing / 2.0#
                    End If
                End If
                Math.DivRem(Footprint.Y, 2, Remainder)
                If Remainder > 0 Then
                    NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y
                Else
                    If Rnd() >= 0.5F Then
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y - TerrainGridSpacing / 2.0#
                    Else
                        NewUnit.Pos.Horizontal.Y = NodeTag.Pos.Y + TerrainGridSpacing / 2.0#
                    End If
                End If
                NewUnit.Pos.Altitude = GetTerrainHeight(NewUnit.Pos.Horizontal)
                TilePosA.X = Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#)
                TilePosA.Y = Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#)
                TilePosB.X = Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#)
                TilePosB.Y = Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#)
                NewUnit.Rotation = Rotation

                Unit_Add(NewUnit)

                For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, TerrainSize.Y - 1)
                    For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, TerrainSize.X - 1)
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
        NewUnit.PlayerNum = PlayerNum

        FinalTilePos.X = Int(Pos.Horizontal.X / TerrainGridSpacing)
        FinalTilePos.Y = Int(Pos.Horizontal.Y / TerrainGridSpacing)

        Footprint = Type.GetFootprint

        NewUnit.Pos = Pos
        TilePosA.X = Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing - Footprint.X / 2.0# + 0.5#)
        TilePosA.Y = Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing - Footprint.Y / 2.0# + 0.5#)
        TilePosB.X = Int(NewUnit.Pos.Horizontal.X / TerrainGridSpacing + Footprint.X / 2.0# - 0.5#)
        TilePosB.Y = Int(NewUnit.Pos.Horizontal.Y / TerrainGridSpacing + Footprint.Y / 2.0# - 0.5#)
        NewUnit.Rotation = Rotation

        Unit_Add(NewUnit)

        For Y2 = Math.Max(TilePosA.Y, 0) To Math.Min(TilePosB.Y, TerrainSize.Y - 1)
            For X2 = Math.Max(TilePosA.X, 0) To Math.Min(TilePosB.X, TerrainSize.X - 1)
                TileNodeBlock(X2, Y2)
            Next
        Next

        TilePathMap.FindCalc()

        Return NewUnit
    End Function

    Public Sub TileNodeBlock(ByVal X As Integer, ByVal Y As Integer)
        Dim X2 As Integer
        Dim Y2 As Integer

        For Y2 = Math.Max(Y - 6, 0) To Math.Min(Y + 6, TerrainSize.Y - 1)
            For X2 = Math.Max(X - 6, 0) To Math.Min(X + 6, TerrainSize.X - 1)
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

    Public Structure sGenerateUnitsArgs
        Public FeatureClusterChance As Single
        Public FeatureClusterMinUnits As Integer
        Public FeatureClusterMaxUnits As Integer
        Public FeatureScatterCount As Integer
        Public TruckCount As Integer
    End Structure

    Public Function GenerateUnits(ByRef Args As sGenerateUnitsArgs) As sResult
        GenerateUnits.Success = False
        GenerateUnits.Problem = ""

        Dim A As Integer
        Dim B As Integer
        Dim C As Integer
        Dim D As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim tmpUnit As clsMap.clsUnit
        Dim Count As Integer
        Dim FeaturePlaceRange As Integer = 6 * 128
        Dim BasePlaceRange As Integer = 16 * 128
        Dim TilePos As sXY_int
        Dim AverageHeight As Byte

        'block edge tiles
        For Y = 0 To TerrainSize.Y - 1
            For X = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For X = TerrainSize.X - 4 To TerrainSize.X - 1
                TileNodeBlock(X, Y)
            Next
        Next
        For X = 3 To TerrainSize.X - 5
            For Y = 0 To 2
                TileNodeBlock(X, Y)
            Next
            For Y = TerrainSize.Y - 4 To TerrainSize.Y - 1
                TileNodeBlock(X, Y)
            Next
        Next
        TilePathMap.FindCalc()

        For A = 0 To _PlayerCount - 1
            If PlaceUnitNear(UnitType_CommandCentre, BasePassageNodes(A).Pos, A, 3, 0, BasePlaceRange) Is Nothing Then
                GenerateUnits.Problem = "No room for base structures"
                Exit Function
            End If
            tmpUnit = PlaceUnitNear(UnitType_PowerGenerator, BasePassageNodes(A).Pos, A, 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                GenerateUnits.Problem = "No room for base structures."
                Exit Function
            End If
            tmpUnit = PlaceUnit(UnitType_PowerModule, tmpUnit.Pos, A, 0)
            If tmpUnit Is Nothing Then
                GenerateUnits.Problem = "No room for placing module."
                Exit Function
            End If
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_ResearchFacility, BasePassageNodes(A).Pos, A, 3, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    GenerateUnits.Problem = "No room for base structures"
                    Exit Function
                End If
                tmpUnit = PlaceUnit(UnitType_ResearchModule, tmpUnit.Pos, A, 0)
                If tmpUnit Is Nothing Then
                    GenerateUnits.Problem = "No room for module."
                    Exit Function
                End If
            Next
            For B = 1 To 2
                tmpUnit = PlaceUnitNear(UnitType_Factory, BasePassageNodes(A).Pos, A, 4, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    GenerateUnits.Problem = "No room for base structures"
                    Exit Function
                End If
                tmpUnit = PlaceUnit(UnitType_FactoryModule, tmpUnit.Pos, A, 0)
                If tmpUnit Is Nothing Then
                    GenerateUnits.Problem = "No room for module."
                    Exit Function
                End If
            Next
            tmpUnit = PlaceUnitNear(UnitType_CyborgFactory, BasePassageNodes(A).Pos, A, 3, 0, BasePlaceRange)
            If tmpUnit Is Nothing Then
                GenerateUnits.Problem = "No room for base structures"
                Exit Function
            End If
            For B = 1 To Args.TruckCount
                tmpUnit = PlaceUnitNear(UnitType_Truck, BasePassageNodes(A).Pos, A, 2, 0, BasePlaceRange)
                If tmpUnit Is Nothing Then
                    GenerateUnits.Problem = "No room for trucks"
                    Exit Function
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
                        GenerateUnits.Problem = "No room for base oil."
                        Exit Function
                    End If
                    'flatten ground underneath
                    TilePos.X = Int(tmpUnit.Pos.Horizontal.X / TerrainGridSpacing)
                    TilePos.Y = Int(tmpUnit.Pos.Horizontal.Y / TerrainGridSpacing)
                    AverageHeight = CByte((CInt(TerrainVertex(TilePos.X, TilePos.Y).Height) + CInt(TerrainVertex(TilePos.X + 1, TilePos.Y).Height) + CInt(TerrainVertex(TilePos.X, TilePos.Y + 1).Height) + CInt(TerrainVertex(TilePos.X + 1, TilePos.Y + 1).Height)) / 4.0#)
                    TerrainVertex(TilePos.X, TilePos.Y).Height = AverageHeight
                    TerrainVertex(TilePos.X + 1, TilePos.Y).Height = AverageHeight
                    TerrainVertex(TilePos.X, TilePos.Y + 1).Height = AverageHeight
                    TerrainVertex(TilePos.X + 1, TilePos.Y + 1).Height = AverageHeight
                    tmpUnit.Pos.Altitude = AverageHeight * HeightMultiplier
                    If C < _PlayerCount Then
                        'place base derrick
                        tmpUnit = PlaceUnit(UnitType_Derrick, tmpUnit.Pos, C, 0)
                        If tmpUnit Is Nothing Then
                            GenerateUnits.Problem = "No room for derrick."
                            Exit Function
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
                        Count = Args.FeatureClusterMinUnits + Int(Rnd() * (Args.FeatureClusterMaxUnits - Args.FeatureClusterMinUnits + 1))
                        For B = 1 To Count
                            RandNum = Int(CDbl(Rnd()) * GenerateTileset.ClusteredUnitChanceTotal)
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
                                GenerateUnits.Problem = "Not enough space for a clustered unit"
                                Exit Function
                            End If
                        Next
                    End If
                Next
            Next
        End If

        If TilePathMap.GetNodeLayer(TilePathMap.GetNodeLayerCount - 1).GetNodeCount <> 1 Then
            GenerateUnits.Problem = "Error; bad node count on top layer!"
            Exit Function
        End If

        If GenerateTileset.ScatteredUnitChanceTotal > 0 Then
            For A = 1 To Args.FeatureScatterCount
                RandNum = Int(CDbl(Rnd()) * GenerateTileset.ScatteredUnitChanceTotal)
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
                    GenerateUnits.Problem = "Not enough space for a scattered unit"
                    Exit Function
                Else
                    Dim NodeTag As clsNodeTag = tmpNode.Tag
                    If PlaceUnitNear(GenerateTileset.ScatteredUnits(C).Type, NodeTag.Pos, 0, B, 0, FeaturePlaceRange) Is Nothing Then
                        GenerateUnits.Problem = "Not enough space for a scattered unit"
                        Exit Function
                    End If
                End If
            Next
        End If

        GenerateUnits.Success = True
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
                A = Int(Rnd() * InputNode.GetChildNodeCount)
            Loop While InputNode.GetChildNode(A).GetClearance < MinClearance
            Return GetRandomChildNode(InputNode.GetChildNode(A), MinClearance)
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
        GenerateGateways.Success = False
        GenerateGateways.Problem = ""

        'must be done before units otherwise the units will be treated as gateway obstacles

        Dim X As Integer
        Dim SpaceCount As Integer
        Dim Y As Integer
        Dim PossibleGateways(TerrainSize.Y * TerrainSize.X * 2 - 1) As sPossibleGateway
        Dim PossibleGatewayCount As Integer

        For Y = 0 To TerrainSize.Y - 1
            SpaceCount = 0
            X = 0
            Do While X < TerrainSize.X
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
                X += 1
            Loop
        Next
        For X = 0 To TerrainSize.X - 1
            SpaceCount = 0
            Y = 0
            Do While Y < TerrainSize.Y
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
        Dim TileIsGateway(TerrainSize.X - 1, TerrainSize.Y - 1) As Boolean
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
                    If GetDist(InvalidPos, PossibleGateways(A).MiddlePos) < InvalidDist Then
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

        GenerateGateways.Success = True
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
            RotatedPos = GetTileRotatedPos(Args.SymmetryBlocks(A).Orientation, RatioPos)
            Positions(A).X = (Args.SymmetryBlocks(A).XYNum.X + RotatedPos.X) * SymmetrySize.X
            Positions(A).Y = (Args.SymmetryBlocks(A).XYNum.Y + RotatedPos.Y) * SymmetrySize.Y
            For B = 0 To A - 1
                If GetDist(Positions(A), Positions(B)) < Args.NodeScale * TerrainGridSpacing * 2.0F Then
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


    '    For Y = 0 To TerrainSize.Y
    '        For X = 0 To TerrainSize.X
    '            GenerateTerrainVertex(X, Y).Node = New PathfinderNode(RoadPathMap)
    '            NodeTag = New clsNodeTag
    '            NodeTag.Pos = New sXY_int(X * 128, Y * 128)
    '            GenerateTerrainVertex(X, Y).Node.Tag = NodeTag
    '        Next
    '    Next
    '    For Y = 0 To TerrainSize.Y
    '        For X = 0 To TerrainSize.X
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
    '                If X < TerrainSize.X Then
    '                    tmpNodeB = GenerateTerrainVertex(X + 1, Y - 1).Node
    '                    GenerateTerrainVertex(X, Y).TopRightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '            End If
    '            If X < TerrainSize.X Then
    '                tmpNodeB = GenerateTerrainVertex(X + 1, Y).Node
    '                GenerateTerrainVertex(X, Y).RightLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '            End If
    '            If Y < TerrainSize.Y Then
    '                If X > 0 Then
    '                    tmpNodeB = GenerateTerrainVertex(X - 1, Y + 1).Node
    '                    GenerateTerrainVertex(X, Y).BottomLeftLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                End If
    '                tmpNodeB = GenerateTerrainVertex(X, Y + 1).Node
    '                GenerateTerrainVertex(X, Y).BottomLink = tmpNodeA.GetOrCreateConnection(tmpNodeB, GetNodePosDist(tmpNodeA, tmpNodeB))
    '                If X < TerrainSize.X Then
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

    Public Sub New(ByVal Size As sXY_int)
        MyBase.New(Size.X, Size.Y)

    End Sub
End Class