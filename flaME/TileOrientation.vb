Public Module TileOrientation

    Public Structure sTileOrientation
        Public ResultXFlip As Boolean
        Public ResultZFlip As Boolean
        Public SwitchedAxes As Boolean

        Public Sub New(ByVal New_ResultXFlip As Boolean, ByVal New_ResultZFlip As Boolean, ByVal New_SwitchedAxes As Boolean)

            ResultXFlip = New_ResultXFlip
            ResultZFlip = New_ResultZFlip
            SwitchedAxes = New_SwitchedAxes
        End Sub

        Public Sub RotateClockwise()

            SwitchedAxes = Not SwitchedAxes
            If ResultXFlip Xor ResultZFlip Then
                ResultZFlip = Not ResultZFlip
            Else
                ResultXFlip = Not ResultXFlip
            End If
        End Sub

        Public Sub RotateAnticlockwise()

            SwitchedAxes = Not SwitchedAxes
            If ResultXFlip Xor ResultZFlip Then
                ResultXFlip = Not ResultXFlip
            Else
                ResultZFlip = Not ResultZFlip
            End If
        End Sub
    End Structure

    Public Structure sTileDirection
        Public X As Byte '0-2, 1=middle
        Public Z As Byte '0-2, 1=middle

        Public Sub New(ByVal NewX As Byte, ByVal NewZ As Byte)

            X = NewX
            Z = NewZ
        End Sub

        Public Sub FlipX()

            X = 2 - X
        End Sub

        Public Sub FlipZ()

            Z = 2 - Z
        End Sub

        Public Sub RotateClockwise()
            Dim byteTemp As Byte

            byteTemp = X
            X = 2 - Z
            Z = byteTemp
        End Sub

        Public Sub RotateAnticlockwise()
            Dim byteTemp As Byte

            byteTemp = X
            X = Z
            Z = 2 - byteTemp
        End Sub

        Public Sub SwitchAxes()
            Dim byteTemp As Byte

            byteTemp = X
            X = Z
            Z = byteTemp
        End Sub
    End Structure

    Public TileDirection_TopLeft As New sTileDirection(0, 0)
    Public TileDirection_Top As New sTileDirection(1, 0)
    Public TileDirection_TopRight As New sTileDirection(2, 0)
    Public TileDirection_Right As New sTileDirection(2, 1)
    Public TileDirection_BottomRight As New sTileDirection(2, 2)
    Public TileDirection_Bottom As New sTileDirection(1, 2)
    Public TileDirection_BottomLeft As New sTileDirection(0, 2)
    Public TileDirection_Left As New sTileDirection(0, 1)
    Public TileDirection_None As New sTileDirection(1, 1)

    Public Function OrientateTile(ByRef TileChance As sPainter.sTileList.sTile_Orientation_Chance, ByVal NewDirection As sTileDirection) As clsMap.sTerrainTile.sTexture
        Static IsDiagonal As Boolean

        'use random for empty tiles
        If TileChance.TextureNum < 0 Then
            OrientateTile.Orientation.ResultXFlip = (Rnd() >= 0.5F)
            OrientateTile.Orientation.ResultZFlip = (Rnd() >= 0.5F)
            OrientateTile.Orientation.SwitchedAxes = (Rnd() >= 0.5F)
            OrientateTile.TextureNum = -1
            Exit Function
        End If
        'stop invalid numbers
        If TileChance.Direction.X > 2 Or TileChance.Direction.Z > 2 Or NewDirection.X > 2 Or NewDirection.Z > 2 Then
            Stop
            Exit Function
        End If
        'stop different direction types
        If (NewDirection.X = 1 Xor NewDirection.Z = 1) Xor (TileChance.Direction.X = 1 Xor TileChance.Direction.Z = 1) Then
            Stop
            Exit Function
        End If

        OrientateTile.TextureNum = TileChance.TextureNum

        'if a direction is neutral then give a random orientation
        If (NewDirection.X = 1 And NewDirection.Z = 1) Or (TileChance.Direction.X = 1 And TileChance.Direction.Z = 1) Then
            OrientateTile.Orientation.SwitchedAxes = (Rnd() >= 0.5F)
            OrientateTile.Orientation.ResultXFlip = (Rnd() >= 0.5F)
            OrientateTile.Orientation.ResultZFlip = (Rnd() >= 0.5F)
            Exit Function
        End If

        IsDiagonal = (NewDirection.X <> 1 And NewDirection.Z <> 1)
        If IsDiagonal Then
            OrientateTile.Orientation.SwitchedAxes = False
            'use flips to match the directions
            If TileChance.Direction.X = 0 Xor NewDirection.X = 0 Then
                OrientateTile.Orientation.ResultXFlip = True
            Else
                OrientateTile.Orientation.ResultXFlip = False
            End If
            If TileChance.Direction.Z = 0 Xor NewDirection.Z = 0 Then
                OrientateTile.Orientation.ResultZFlip = True
            Else
                OrientateTile.Orientation.ResultZFlip = False
            End If
            'randomly switch to the alternate orientation
            If Rnd() >= 0.5F Then
                OrientateTile.Orientation.SwitchedAxes = Not OrientateTile.Orientation.SwitchedAxes
                If (NewDirection.X = 0 Xor NewDirection.Z = 0) Xor (OrientateTile.Orientation.ResultXFlip Xor OrientateTile.Orientation.ResultZFlip) Then
                    OrientateTile.Orientation.ResultXFlip = Not OrientateTile.Orientation.ResultXFlip
                    OrientateTile.Orientation.ResultZFlip = Not OrientateTile.Orientation.ResultZFlip
                End If
            End If
        Else
            'switch axes if the directions are on different axes
            OrientateTile.Orientation.SwitchedAxes = (TileChance.Direction.X = 1 Xor NewDirection.X = 1)
            'use a flip to match the directions
            If OrientateTile.Orientation.SwitchedAxes Then
                If TileChance.Direction.Z <> NewDirection.X Then
                    OrientateTile.Orientation.ResultXFlip = True
                Else
                    OrientateTile.Orientation.ResultXFlip = False
                End If
                If TileChance.Direction.X <> NewDirection.Z Then
                    OrientateTile.Orientation.ResultZFlip = True
                Else
                    OrientateTile.Orientation.ResultZFlip = False
                End If
            Else
                If TileChance.Direction.X <> NewDirection.X Then
                    OrientateTile.Orientation.ResultXFlip = True
                Else
                    OrientateTile.Orientation.ResultXFlip = False
                End If
                If TileChance.Direction.Z <> NewDirection.Z Then
                    OrientateTile.Orientation.ResultZFlip = True
                Else
                    OrientateTile.Orientation.ResultZFlip = False
                End If
            End If
            'randomly switch to the alternate orientation
            If Rnd() >= 0.5F Then
                If NewDirection.X = 1 Then
                    OrientateTile.Orientation.ResultXFlip = Not OrientateTile.Orientation.ResultXFlip
                Else
                    OrientateTile.Orientation.ResultZFlip = Not OrientateTile.Orientation.ResultZFlip
                End If
            End If
        End If
    End Function

    Public Sub OrientationToDirection(ByVal InitialDirection As sTileDirection, ByVal TileTexture As clsMap.sTerrainTile.sTexture, ByRef ResultDirection As sTileDirection)

        ResultDirection = InitialDirection
        If TileTexture.Orientation.SwitchedAxes Then
            ResultDirection.SwitchAxes()
        End If
        If TileTexture.Orientation.ResultXFlip Then
            ResultDirection.FlipX()
        End If
        If TileTexture.Orientation.ResultZFlip Then
            ResultDirection.FlipZ()
        End If
    End Sub

    Public Function GetTileRotatedPos(ByVal TileOrientation As sTileOrientation, ByVal Pos As sXY_sng) As sXY_sng

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                GetTileRotatedPos.X = 1.0F - Pos.Y
            Else
                GetTileRotatedPos.X = Pos.Y
            End If
            If TileOrientation.ResultZFlip Then
                GetTileRotatedPos.Y = 1.0F - Pos.X
            Else
                GetTileRotatedPos.Y = Pos.X
            End If
        Else
            If TileOrientation.ResultXFlip Then
                GetTileRotatedPos.X = 1.0F - Pos.X
            Else
                GetTileRotatedPos.X = Pos.X
            End If
            If TileOrientation.ResultZFlip Then
                GetTileRotatedPos.Y = 1.0F - Pos.Y
            Else
                GetTileRotatedPos.Y = Pos.Y
            End If
        End If
    End Function

    Public Sub GetTileRotatedTexCoords(ByVal TileOrientation As sTileOrientation, ByRef CoordA As sXY_sng, ByRef CoordB As sXY_sng, ByRef CoordC As sXY_sng, ByRef CoordD As sXY_sng)
        Static XFlip As Boolean
        Static ZFlip As Boolean

        XFlip = TileOrientation.ResultXFlip
        ZFlip = TileOrientation.ResultZFlip

        'texcoords are reverse of normal, so reverse any rotation. flip is the reverse of itself so nothing needs to be done there.
        If TileOrientation.SwitchedAxes Then
            If XFlip Xor ZFlip Then
                XFlip = Not XFlip
                ZFlip = Not ZFlip
            End If
        End If

        If TileOrientation.SwitchedAxes Then
            If XFlip Then
                CoordA.X = 1.0F
                CoordB.X = 1.0F
                CoordC.X = 0.0F
                CoordD.X = 0.0F
            Else
                CoordA.X = 0.0F
                CoordB.X = 0.0F
                CoordC.X = 1.0F
                CoordD.X = 1.0F
            End If
            If ZFlip Then
                CoordA.Y = 1.0F
                CoordB.Y = 0.0F
                CoordC.Y = 1.0F
                CoordD.Y = 0.0F
            Else
                CoordA.Y = 0.0F
                CoordB.Y = 1.0F
                CoordC.Y = 0.0F
                CoordD.Y = 1.0F
            End If
        Else
            If XFlip Then
                CoordA.X = 1.0F
                CoordB.X = 0.0F
                CoordC.X = 1.0F
                CoordD.X = 0.0F
            Else
                CoordA.X = 0.0F
                CoordB.X = 1.0F
                CoordC.X = 0.0F
                CoordD.X = 1.0F
            End If
            If ZFlip Then
                CoordA.Y = 1.0F
                CoordB.Y = 1.0F
                CoordC.Y = 0.0F
                CoordD.Y = 0.0F
            Else
                CoordA.Y = 0.0F
                CoordB.Y = 0.0F
                CoordC.Y = 1.0F
                CoordD.Y = 1.0F
            End If
        End If
    End Sub

    Public Sub TileOrientation_To_OldOrientation(ByVal TileOrientation As sTileOrientation, ByRef OutputRotation As Byte, ByRef OutputFlipX As Boolean)

        If TileOrientation.SwitchedAxes Then
            If TileOrientation.ResultXFlip Then
                OutputRotation = 1
            Else
                OutputRotation = 3
            End If
            OutputFlipX = Not (TileOrientation.ResultXFlip Xor TileOrientation.ResultZFlip)
        Else
            If TileOrientation.ResultZFlip Then
                OutputRotation = 2
            Else
                OutputRotation = 0
            End If
            OutputFlipX = (TileOrientation.ResultXFlip Xor TileOrientation.ResultZFlip)
        End If
    End Sub

    Public Sub OldOrientation_To_TileOrientation(ByVal OldRotation As Byte, ByVal OldFlipX As Boolean, ByVal OldFlipZ As Boolean, ByRef Output_TileOrientation As sTileOrientation)

        If OldRotation = 0 Then
            Output_TileOrientation.SwitchedAxes = False
            Output_TileOrientation.ResultXFlip = False
            Output_TileOrientation.ResultZFlip = False
        ElseIf OldRotation = 1 Then
            Output_TileOrientation.SwitchedAxes = True
            Output_TileOrientation.ResultXFlip = True
            Output_TileOrientation.ResultZFlip = False
        ElseIf OldRotation = 2 Then
            Output_TileOrientation.SwitchedAxes = False
            Output_TileOrientation.ResultXFlip = True
            Output_TileOrientation.ResultZFlip = True
        ElseIf OldRotation = 3 Then
            Output_TileOrientation.SwitchedAxes = True
            Output_TileOrientation.ResultXFlip = False
            Output_TileOrientation.ResultZFlip = True
        End If
        If OldFlipX Then
            If Output_TileOrientation.SwitchedAxes Then
                Output_TileOrientation.ResultZFlip = Not Output_TileOrientation.ResultZFlip
            Else
                Output_TileOrientation.ResultXFlip = Not Output_TileOrientation.ResultXFlip
            End If
        End If
        If OldFlipZ Then
            If Output_TileOrientation.SwitchedAxes Then
                Output_TileOrientation.ResultXFlip = Not Output_TileOrientation.ResultXFlip
            Else
                Output_TileOrientation.ResultZFlip = Not Output_TileOrientation.ResultZFlip
            End If
        End If
    End Sub

    Public Function IdenticalTileOrientations(ByVal TileOrientationA As sTileDirection, ByVal TileOrientationB As sTileDirection) As Boolean

        IdenticalTileOrientations = (TileOrientationA.X = TileOrientationB.X And TileOrientationA.Z = TileOrientationB.Z)
    End Function
End Module