Public Module modMath

    Public Const RadOf1Deg As Double = 0.017453292519943295#
    Public Const RadOf90Deg As Double = 1.5707963267948966#
    Public Const Pi As Double = 3.1415926535897931#
    Public Const RadOf360Deg As Double = 6.2831853071795862#

    Public Const RootTwo As Double = 1.4142135623730951#

    Public Structure sXY_dbl
        Dim X As Double
        Dim Y As Double

        Public Sub New(ByVal NewX As Double, ByVal NewY As Double)
            X = NewX
            Y = NewY
        End Sub
    End Structure
    Public Structure sXY_lng
        Dim X As Long
        Dim Y As Long
    End Structure
    Public Structure sXY_int
        Dim X As Integer
        Dim Y As Integer

        Public Sub New(ByVal NewX As Integer, ByVal NewY As Integer)
            X = NewX
            Y = NewY
        End Sub
    End Structure
    Public Structure sXY_uint
        Dim X As UInteger
        Dim Y As UInteger
    End Structure
    Public Structure sXY_byte
        Dim X As Byte
        Dim Y As Byte
    End Structure
    Public Structure sXY_sng
        Dim X As Single
        Dim Y As Single
    End Structure

    Public Structure sXYZ_int
        Dim X As Integer
        Dim Y As Integer
        Dim Z As Integer
    End Structure

    Public Structure sXYZ_dbl
        Dim X As Double
        Dim Y As Double
        Dim Z As Double
    End Structure

    Public Structure sXYZ_sng
        Dim X As Single
        Dim Y As Single
        Dim Z As Single
    End Structure

    Public Structure sXYZ_lng
        Dim X As Long
        Dim Y As Long
        Dim Z As Long
    End Structure

    Public Structure sAnglePY
        Dim Pitch As Double
        Dim Yaw As Double

        Public Sub New(ByVal NewPitch As Double, ByVal NewYaw As Double)
            Pitch = NewPitch
            Yaw = NewYaw
        End Sub
    End Structure

    Public Structure sAngleRPY
        Dim Roll As Double
        Dim Pitch As Double
        Dim Yaw As Double

        Public Sub New(ByVal NewRoll As Double, ByVal NewPitch As Double, ByVal NewYaw As Double)
            Roll = NewRoll
            Pitch = NewPitch
            Yaw = NewYaw
        End Sub
    End Structure

    Function AngleClamp(ByVal Angle As Double) As Double

        AngleClamp = Angle
        If AngleClamp < -Pi Then
            AngleClamp += RadOf360Deg
        ElseIf AngleClamp >= Pi Then
            AngleClamp -= RadOf360Deg
        End If
    End Function

    Public Function GetDist(ByVal PosA As sXY_int, ByVal PosB As sXY_int) As Double
        Static XY_dbl As sXY_dbl

        XY_dbl.X = PosB.X - PosA.X
        XY_dbl.Y = PosB.Y - PosA.Y
        Return Math.Sqrt(XY_dbl.X * XY_dbl.X + XY_dbl.Y * XY_dbl.Y)
    End Function

    Public Function GetDist(ByVal PosA As sXYZ_dbl, ByVal PosB As sXYZ_dbl) As Double
        Static XYZ_dbl As sXYZ_dbl

        XYZ_dbl.X = PosB.X - PosA.X
        XYZ_dbl.Y = PosB.Y - PosA.Y
        XYZ_dbl.Z = PosB.z - PosA.z
        Return Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.Z * XYZ_dbl.Z)
    End Function

    Sub GetDist(ByVal XYZ_dbl As sXYZ_dbl, ByRef ResultDist As Double)

        ResultDist = Math.Sqrt(XYZ_dbl.Z * XYZ_dbl.Z + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.X * XYZ_dbl.X)
    End Sub

    Sub GetAnglePY(ByVal XYZ_dbl As sXYZ_dbl, ByRef Output_AnglePY As sAnglePY)

        Output_AnglePY.Pitch = Math.Atan2(-XYZ_dbl.Y, Math.Sqrt(XYZ_dbl.Z * XYZ_dbl.Z + XYZ_dbl.X * XYZ_dbl.X))
        If Output_AnglePY.Pitch > RadOf90Deg Then
            Output_AnglePY.Pitch = Pi - Output_AnglePY.Pitch
        ElseIf Output_AnglePY.Pitch < -RadOf90Deg Then
            Output_AnglePY.Pitch = -Output_AnglePY.Pitch - Pi
        End If
        Output_AnglePY.Yaw = Math.Atan2(XYZ_dbl.X, XYZ_dbl.Z)
    End Sub

    Function Clamp(ByVal Amount As Double, ByVal Minimum As Double, ByVal Maximum As Double) As Double

        Clamp = Amount
        If Clamp < Minimum Then
            Clamp = Minimum
        ElseIf Clamp > Maximum Then
            Clamp = Maximum
        End If
    End Function

    Function Clamp(ByVal Amount As Single, ByVal Minimum As Single, ByVal Maximum As Single) As Single

        Clamp = Amount
        If Clamp < Minimum Then
            Clamp = Minimum
        ElseIf Clamp > Maximum Then
            Clamp = Maximum
        End If
    End Function

    Function Clamp(ByVal Amount As Integer, ByVal Minimum As Integer, ByVal Maximum As Integer) As Integer

        Clamp = Amount
        If Clamp < Minimum Then
            Clamp = Minimum
        ElseIf Clamp > Maximum Then
            Clamp = Maximum
        End If
    End Function

    Public Structure sIntersectPos
        Dim Exists As Boolean
        Dim Pos As sXY_int
    End Structure

    Public Function GetLinesIntersectBetween(ByVal A1 As sXY_int, ByVal A2 As sXY_int, ByVal B1 As sXY_int, ByVal B2 As sXY_int) As sIntersectPos

        If (A1.X = A2.X And A1.Y = A2.Y) Or (B1.X = B2.X And B1.Y = B2.Y) Then
            GetLinesIntersectBetween.Exists = False
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

            y1dif = A1.Y - B1.Y
            x1dif = B1.X - A1.X
            adifx = A2.X - A1.X
            adify = A2.Y - A1.Y
            bdifx = B2.X - B1.X
            bdify = B2.Y - B1.Y
            m = adifx * bdify - adify * bdifx
            If m = 0.0# Then
                GetLinesIntersectBetween.Exists = False
            Else
                ar = (x1dif * bdify + y1dif * bdifx) / m
                br = (x1dif * adify + y1dif * adifx) / m
                If ar <= 0.0# Or ar >= 1.0# Or br <= 0.0# Or br >= 1.0# Then
                    GetLinesIntersectBetween.Exists = False
                Else
                    GetLinesIntersectBetween.Pos.X = A1.X + CInt(ar * adifx)
                    GetLinesIntersectBetween.Pos.Y = A1.Y + CInt(ar * adify)
                    GetLinesIntersectBetween.Exists = True
                End If
            End If
        End If
    End Function

    Public Function PointGetClosestPosOnLine(ByVal LinePointA As sXY_int, ByVal LinePointB As sXY_int, ByVal Point As sXY_int) As sXY_int
        Dim x1dif As Double = Point.X - LinePointA.X
        Dim y1dif As Double = LinePointA.Y - Point.Y
        Dim adifx As Double = LinePointB.X - LinePointA.X
        Dim adify As Double = LinePointB.Y - LinePointA.Y
        Dim m As Double

        m = adifx * adifx + adify * adify
        If m = 0.0# Then
            PointGetClosestPosOnLine = LinePointA
        Else
            Dim ar As Double
            ar = (x1dif * adifx - y1dif * adify) / m
            If ar <= 0.0# Then
                PointGetClosestPosOnLine = LinePointA
            ElseIf ar >= 1.0# Then
                PointGetClosestPosOnLine = LinePointB
            Else
                PointGetClosestPosOnLine.X = LinePointA.X + CInt(adifx * ar)
                PointGetClosestPosOnLine.Y = LinePointA.Y + CInt(adify * ar)
            End If
        End If
    End Function

    Public Function GetAngle(ByVal Length As sXY_int) As Double

        Return Math.Atan2(CDbl(Length.Y), CDbl(Length.X))
    End Function
End Module