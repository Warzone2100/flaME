Public Module modMath

    Public Const RadOf1Deg As Double = 0.017453292519943295#
    Public Const RadOf90Deg As Double = 1.5707963267948966#
    Public Const Pi As Double = 3.1415926535897931#
    Public Const RadOf360Deg As Double = 6.2831853071795862#

    Public Const RootTwo As Double = 1.4142135623730951#

    Public Structure sXY_dbl
        Dim X As Double
        Dim Y As Double
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

    Sub GetDist(ByVal XYZ_dbl As sXYZ_dbl, ByRef Output_Dist As Double)

        Output_Dist = Math.Sqrt(XYZ_dbl.Z * XYZ_dbl.Z + XYZ_dbl.Y * XYZ_dbl.Y + XYZ_dbl.X * XYZ_dbl.X)
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

    Function Clamp(ByVal Amount As Integer, ByVal Minimum As Integer, ByVal Maximum As Integer) As Integer

        Clamp = Amount
        If Clamp < Minimum Then
            Clamp = Minimum
        ElseIf Clamp > Maximum Then
            Clamp = Maximum
        End If
    End Function
End Module