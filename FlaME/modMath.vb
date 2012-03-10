Public Module modMath

    Public Const RadOf1Deg As Double = Math.PI / 180.0#
    Public Const RadOf90Deg As Double = Math.PI / 2.0#
    Public Const RadOf360Deg As Double = Math.PI * 2.0#

    Public Const RootTwo As Double = 1.4142135623730951#

    Public Structure sXY_int
        Public X As Integer
        Public Y As Integer

        Public Sub New(ByVal X As Integer, ByVal Y As Integer)
            Me.X = X
            Me.Y = Y
        End Sub

        Public Function GetAngle() As Double

            Return Math.Atan2(CDbl(Y), CDbl(X))
        End Function
    End Structure

    Public Class clsXY_int
        Public XY As sXY_int

        Public Property X As Integer
            Get
                Return XY.X
            End Get
            Set(ByVal value As Integer)
                XY.X = value
            End Set
        End Property
        Public Property Y As Integer
            Get
                Return XY.Y
            End Get
            Set(ByVal value As Integer)
                XY.Y = value
            End Set
        End Property

        Public Sub New(ByVal XY As sXY_int)

            Me.XY = XY
        End Sub
    End Class

    Public Structure sXY_uint
        Public X As UInteger
        Public Y As UInteger

        Public Sub New(ByVal X As UInteger, ByVal Y As UInteger)
            Me.X = X
            Me.Y = Y
        End Sub
    End Structure

    Public Structure sXY_sng
        Public X As Single
        Public Y As Single

        Public Sub New(ByVal X As Single, ByVal Y As Single)
            Me.X = X
            Me.Y = Y
        End Sub
    End Structure

    Public Structure sXYZ_int
        Public X As Integer
        Public Y As Integer
        Public Z As Integer

        Public Sub New(ByVal X As Integer, ByVal Y As Integer, ByVal Z As Integer)
            Me.X = X
            Me.Y = Y
            Me.Z = Z
        End Sub

        Public Sub Add_dbl(ByVal XYZ As Matrix3D.XYZ_dbl)
            X += CInt(XYZ.X)
            Y += CInt(XYZ.Y)
            Z += CInt(XYZ.Z)
        End Sub

        Public Sub Set_dbl(ByVal XYZ As Matrix3D.XYZ_dbl)
            X = CInt(XYZ.X)
            Y = CInt(XYZ.Y)
            Z = CInt(XYZ.Z)
        End Sub
    End Structure

    Public Structure sXYZ_sng
        Public X As Single
        Public Y As Single
        Public Z As Single

        Public Sub New(ByVal X As Single, ByVal Y As Single, ByVal Z As Single)
            Me.X = X
            Me.Y = Y
            Me.Z = Z
        End Sub
    End Structure

    Public Function AngleClamp(ByVal Angle As Double) As Double
        Dim ReturnResult As Double

        ReturnResult = Angle
        If ReturnResult < -Math.PI Then
            ReturnResult += RadOf360Deg
        ElseIf ReturnResult >= Math.PI Then
            ReturnResult -= RadOf360Deg
        End If
        Return ReturnResult
    End Function

    Public Function GetDist_XY_int(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As Double
        Dim Dif As Matrix3D.XY_dbl

        Dif.X = PosB.X - PosA.X
        Dif.Y = PosB.Y - PosA.Y
        Return Math.Sqrt(Dif.X * Dif.X + Dif.Y * Dif.Y)
    End Function

    Public Function Clamp_dbl(ByVal Amount As Double, ByVal Minimum As Double, ByVal Maximum As Double) As Double
        Dim ReturnResult As Double

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Function Clamp_sng(ByVal Amount As Single, ByVal Minimum As Single, ByVal Maximum As Single) As Single
        Dim ReturnResult As Single

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Function Clamp_int(ByVal Amount As Integer, ByVal Minimum As Integer, ByVal Maximum As Integer) As Integer
        Dim ReturnResult As Integer

        ReturnResult = Amount
        If ReturnResult < Minimum Then
            ReturnResult = Minimum
        ElseIf ReturnResult > Maximum Then
            ReturnResult = Maximum
        End If
        Return ReturnResult
    End Function

    Public Structure sIntersectPos
        Public Exists As Boolean
        Public Pos As sXY_int
    End Structure

    Public Function GetLinesIntersectBetween(ByVal A1 As sXY_int, ByVal A2 As sXY_int, ByVal B1 As sXY_int, ByVal B2 As sXY_int) As sIntersectPos
        Dim Result As sIntersectPos

        If (A1.X = A2.X And A1.Y = A2.Y) Or (B1.X = B2.X And B1.Y = B2.Y) Then
            Result.Exists = False
        Else
            Dim y1dif As Double
            Dim x1dif As Double
            Dim adifx As Double
            Dim adify As Double
            Dim bdifx As Double
            Dim bdify As Double
            Dim m As Double
            Dim ar As Double
            Dim br As Double

            y1dif = B1.Y - A1.Y
            x1dif = B1.X - A1.X
            adifx = A2.X - A1.X
            adify = A2.Y - A1.Y
            bdifx = B2.X - B1.X
            bdify = B2.Y - B1.Y
            m = adifx * bdify - adify * bdifx
            If m = 0.0# Then
                Result.Exists = False
            Else
                ar = (x1dif * bdify - y1dif * bdifx) / m
                br = (x1dif * adify - y1dif * adifx) / m
                If ar <= 0.0# Or ar >= 1.0# Or br <= 0.0# Or br >= 1.0# Then
                    Result.Exists = False
                Else
                    Result.Pos.X = A1.X + CInt(ar * adifx)
                    Result.Pos.Y = A1.Y + CInt(ar * adify)
                    Result.Exists = True
                End If
            End If
        End If
        Return Result
    End Function

    Public Function PointGetClosestPosOnLine(ByVal LinePointA As sXY_int, ByVal LinePointB As sXY_int, ByVal Point As sXY_int) As sXY_int
        Dim x1dif As Double = Point.X - LinePointA.X
        Dim y1dif As Double = Point.Y - LinePointA.Y
        Dim adifx As Double = LinePointB.X - LinePointA.X
        Dim adify As Double = LinePointB.Y - LinePointA.Y
        Dim m As Double

        m = adifx * adifx + adify * adify
        If m = 0.0# Then
            Return LinePointA
        Else
            Dim ar As Double
            ar = (x1dif * adifx + y1dif * adify) / m
            If ar <= 0.0# Then
                Return LinePointA
            ElseIf ar >= 1.0# Then
                Return LinePointB
            Else
                Dim Result As sXY_int
                Result.X = LinePointA.X + CInt(adifx * ar)
                Result.Y = LinePointA.Y + CInt(adify * ar)
                Return Result
            End If
        End If
    End Function
End Module