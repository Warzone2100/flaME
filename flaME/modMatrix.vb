Module modMatrix

    Function VectorDotProduct(ByVal VectorA As sXYZ_dbl, ByVal VectorB As sXYZ_dbl) As Double

        VectorDotProduct = VectorA.Z * VectorB.Z + VectorA.Y * VectorB.Y + VectorA.X * VectorB.X
    End Function

    Sub VectorCrossProduct(ByVal VectorA As sXYZ_dbl, ByVal VectorB As sXYZ_dbl, ByRef Vector_Output As sXYZ_dbl)

        Vector_Output.X = VectorA.Y * VectorB.Z - VectorB.Y * VectorA.Z
        Vector_Output.Y = VectorA.Z * VectorB.X - VectorB.Z * VectorA.X
        Vector_Output.Z = VectorA.X * VectorB.Y - VectorB.X * VectorA.Y
    End Sub

    Sub VectorRotationByMatrix(ByRef Matrix() As Double, ByVal Vector As sXYZ_dbl, ByRef Vector_Output As sXYZ_dbl)

        Vector_Output.X = Vector.X * Matrix(0) + Vector.Y * Matrix(1) + Vector.Z * Matrix(2)
        Vector_Output.Y = Vector.X * Matrix(3) + Vector.Y * Matrix(4) + Vector.Z * Matrix(5)
        Vector_Output.Z = Vector.X * Matrix(6) + Vector.Y * Matrix(7) + Vector.Z * Matrix(8)
    End Sub

    Sub VectorUpRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(1)
        Output_Vector.Y = Input_Matrix(4)
        Output_Vector.Z = Input_Matrix(7)
    End Sub

    Sub VectorUpRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(1) * Scale
        Output_Vector.Y = Input_Matrix(4) * Scale
        Output_Vector.Z = Input_Matrix(7) * Scale
    End Sub

    Sub VectorDownRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = -Input_Matrix(1)
        Output_Vector.Y = -Input_Matrix(4)
        Output_Vector.Z = -Input_Matrix(7)
    End Sub

    Sub VectorDownRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(1) * -Scale
        Output_Vector.Y = Input_Matrix(4) * -Scale
        Output_Vector.Z = Input_Matrix(7) * -Scale
    End Sub

    Sub VectorLeftRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = -Input_Matrix(0)
        Output_Vector.Y = -Input_Matrix(3)
        Output_Vector.Z = -Input_Matrix(6)
    End Sub

    Sub VectorLeftRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(0) * -Scale
        Output_Vector.Y = Input_Matrix(3) * -Scale
        Output_Vector.Z = Input_Matrix(6) * -Scale
    End Sub

    Sub VectorRightRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(0)
        Output_Vector.Y = Input_Matrix(3)
        Output_Vector.Z = Input_Matrix(6)
    End Sub

    Sub VectorRightRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(0) * Scale
        Output_Vector.Y = Input_Matrix(3) * Scale
        Output_Vector.Z = Input_Matrix(6) * Scale
    End Sub

    Sub VectorForwardRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(2)
        Output_Vector.Y = Input_Matrix(5)
        Output_Vector.Z = Input_Matrix(8)
    End Sub

    Sub VectorForwardRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(2) * Scale
        Output_Vector.Y = Input_Matrix(5) * Scale
        Output_Vector.Z = Input_Matrix(8) * Scale
    End Sub

    Sub VectorBackwardRotationByMatrix(ByRef Input_Matrix() As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = -Input_Matrix(2)
        Output_Vector.Y = -Input_Matrix(5)
        Output_Vector.Z = -Input_Matrix(8)
    End Sub

    Sub VectorBackwardRotationByMatrix(ByRef Input_Matrix() As Double, ByVal Scale As Double, ByRef Output_Vector As sXYZ_dbl)

        Output_Vector.X = Input_Matrix(2) * -Scale
        Output_Vector.Y = Input_Matrix(5) * -Scale
        Output_Vector.Z = Input_Matrix(8) * -Scale
    End Sub

    Sub MatrixRotationByMatrix(ByRef Rotation_Matrix() As Double, ByRef Base_Matrix() As Double, ByRef Output_Matrix() As Double)

        Output_Matrix(0) = Rotation_Matrix(0) * Base_Matrix(0) + Rotation_Matrix(1) * Base_Matrix(3) + Rotation_Matrix(2) * Base_Matrix(6)
        Output_Matrix(1) = Rotation_Matrix(0) * Base_Matrix(1) + Rotation_Matrix(1) * Base_Matrix(4) + Rotation_Matrix(2) * Base_Matrix(7)
        Output_Matrix(2) = Rotation_Matrix(0) * Base_Matrix(2) + Rotation_Matrix(1) * Base_Matrix(5) + Rotation_Matrix(2) * Base_Matrix(8)
        Output_Matrix(3) = Rotation_Matrix(3) * Base_Matrix(0) + Rotation_Matrix(4) * Base_Matrix(3) + Rotation_Matrix(5) * Base_Matrix(6)
        Output_Matrix(4) = Rotation_Matrix(3) * Base_Matrix(1) + Rotation_Matrix(4) * Base_Matrix(4) + Rotation_Matrix(5) * Base_Matrix(7)
        Output_Matrix(5) = Rotation_Matrix(3) * Base_Matrix(2) + Rotation_Matrix(4) * Base_Matrix(5) + Rotation_Matrix(5) * Base_Matrix(8)
        Output_Matrix(6) = Rotation_Matrix(6) * Base_Matrix(0) + Rotation_Matrix(7) * Base_Matrix(3) + Rotation_Matrix(8) * Base_Matrix(6)
        Output_Matrix(7) = Rotation_Matrix(6) * Base_Matrix(1) + Rotation_Matrix(7) * Base_Matrix(4) + Rotation_Matrix(8) * Base_Matrix(7)
        Output_Matrix(8) = Rotation_Matrix(6) * Base_Matrix(2) + Rotation_Matrix(7) * Base_Matrix(5) + Rotation_Matrix(8) * Base_Matrix(8)
    End Sub

    Sub MatrixSetToZAngle(ByRef Matrix() As Double, ByVal Z As Double)

        Matrix(0) = Math.Cos(Z)
        Matrix(1) = -Math.Sin(Z)
        Matrix(2) = 0.0#
        Matrix(3) = Math.Sin(Z)
        Matrix(4) = Math.Cos(Z)
        Matrix(5) = 0.0#
        Matrix(6) = 0.0#
        Matrix(7) = 0.0#
        Matrix(8) = 1.0#
    End Sub

    Sub MatrixSetToYAngle(ByRef Matrix() As Double, ByVal Y As Double)

        Matrix(0) = Math.Cos(Y)
        Matrix(1) = 0.0#
        Matrix(2) = Math.Sin(Y)
        Matrix(3) = 0.0#
        Matrix(4) = 1.0#
        Matrix(5) = 0.0#
        Matrix(6) = -Math.Sin(Y)
        Matrix(7) = 0.0#
        Matrix(8) = Math.Cos(Y)
    End Sub

    Sub MatrixSetToXAngle(ByRef Matrix() As Double, ByVal X As Double)

        Matrix(0) = 1.0#
        Matrix(1) = 0.0#
        Matrix(2) = 0.0#
        Matrix(3) = 0.0#
        Matrix(4) = Math.Cos(X)
        Matrix(5) = -Math.Sin(X)
        Matrix(6) = 0.0#
        Matrix(7) = Math.Sin(X)
        Matrix(8) = Math.Cos(X)
    End Sub

    Sub MatrixSetToIdentity(ByRef Matrix() As Double)

        Matrix(0) = 1.0#
        Matrix(1) = 0.0#
        Matrix(2) = 0.0#
        Matrix(3) = 0.0#
        Matrix(4) = 1.0#
        Matrix(5) = 0.0#
        Matrix(6) = 0.0#
        Matrix(7) = 0.0#
        Matrix(8) = 1.0#
    End Sub

    Sub MatrixToRPY(ByRef Matrix() As Double, ByRef Output_RPY As sAngleRPY)
        Dim XYZ_dbl As sXYZ_dbl
        Dim XYZ_dbl2 As sXYZ_dbl
        Dim AnglePY As sAnglePY
        Dim matrixX(8) As Double
        Dim matrixY(8) As Double

        VectorForwardRotationByMatrix(Matrix, XYZ_dbl2)
        GetAnglePY(XYZ_dbl2, AnglePY)
        Output_RPY.Pitch = AnglePY.Pitch
        Output_RPY.Yaw = AnglePY.Yaw
        VectorRightRotationByMatrix(Matrix, XYZ_dbl2)
        MatrixSetToXAngle(matrixX, -AnglePY.Pitch)
        MatrixSetToYAngle(matrixY, -AnglePY.Yaw)
        VectorRotationByMatrix(matrixY, XYZ_dbl2, XYZ_dbl)
        VectorRotationByMatrix(matrixX, XYZ_dbl, XYZ_dbl2)
        Output_RPY.Roll = Math.Atan2(XYZ_dbl2.Y, XYZ_dbl2.X)
    End Sub

    Sub MatrixToPY(ByRef Matrix() As Double, ByRef Output_PY As sAnglePY)
        Dim XYZ_dbl As sXYZ_dbl

        VectorForwardRotationByMatrix(Matrix, XYZ_dbl)
        GetAnglePY(XYZ_dbl, Output_PY)
    End Sub

    Sub MatrixCopy(ByRef Matrix_Input() As Double, ByRef Matrix_Output() As Double)

        Matrix_Output(0) = Matrix_Input(0)
        Matrix_Output(1) = Matrix_Input(1)
        Matrix_Output(2) = Matrix_Input(2)
        Matrix_Output(3) = Matrix_Input(3)
        Matrix_Output(4) = Matrix_Input(4)
        Matrix_Output(5) = Matrix_Input(5)
        Matrix_Output(6) = Matrix_Input(6)
        Matrix_Output(7) = Matrix_Input(7)
        Matrix_Output(8) = Matrix_Input(8)
    End Sub

    Sub MatrixInvert(ByRef matrixInput() As Double, ByRef matrixOutput() As Double)
        Dim A As Double
        Dim B As Double
        Dim C As Double
        Dim D As Double
        Dim E As Double
        Dim F As Double
        Dim G As Double
        Dim H As Double
        Dim I As Double
        Dim A2 As Double
        Dim B2 As Double
        Dim C2 As Double
        Dim D2 As Double
        Dim E2 As Double
        Dim F2 As Double
        Dim G2 As Double
        Dim H2 As Double
        Dim I2 As Double
        Dim ID As Double

        A = matrixInput(0)
        B = matrixInput(1)
        C = matrixInput(2)
        D = matrixInput(3)
        E = matrixInput(4)
        F = matrixInput(5)
        G = matrixInput(6)
        H = matrixInput(7)
        I = matrixInput(8)

        A2 = E * I - H * F
        B2 = G * F - D * I
        C2 = D * H - G * E
        D2 = H * C - B * I
        E2 = A * I - G * C
        F2 = G * B - A * H
        G2 = B * F - E * C
        H2 = D * C - A * F
        I2 = A * E - D * B

        ID = 1.0# / (A * A2 + B * B2 + C * C2)

        matrixOutput(0) = ID * A2
        matrixOutput(1) = ID * D2
        matrixOutput(2) = ID * G2
        matrixOutput(3) = ID * B2
        matrixOutput(4) = ID * E2
        matrixOutput(5) = ID * H2
        matrixOutput(6) = ID * C2
        matrixOutput(7) = ID * F2
        matrixOutput(8) = ID * I2
    End Sub

    Sub MatrixRotationAroundAxis(ByRef MatrixInput() As Double, ByRef MatrixRotationAxis() As Double, ByVal RotationAngle As Double, ByRef MatrixOutput() As Double)
        Dim matrixA(8) As Double
        Dim matrixB(8) As Double
        Dim matrixC(8) As Double

        MatrixInvert(MatrixRotationAxis, matrixA)
        MatrixRotationByMatrix(matrixA, MatrixInput, matrixB)
        MatrixSetToZAngle(matrixA, RotationAngle)
        MatrixRotationByMatrix(matrixA, matrixB, matrixC)
        MatrixRotationByMatrix(MatrixRotationAxis, matrixC, MatrixOutput)
    End Sub

    Sub MatrixSetToRPY(ByRef Matrix_Output() As Double, ByRef AngleRPY As sAngleRPY)
        Dim matrixZ(8) As Double
        Dim matrixX(8) As Double
        Dim matrixY(8) As Double
        Dim matrixA(8) As Double

        MatrixSetToZAngle(matrixZ, AngleRPY.Roll)
        MatrixSetToXAngle(matrixX, AngleRPY.Pitch)
        MatrixSetToYAngle(matrixY, AngleRPY.Yaw)
        MatrixRotationByMatrix(matrixX, matrixZ, matrixA)
        MatrixRotationByMatrix(matrixY, matrixA, Matrix_Output)
    End Sub

    Sub MatrixSetToPY(ByRef Matrix_Output() As Double, ByVal AnglePY As sAnglePY)
        Dim matrixX(8) As Double
        Dim matrixY(8) As Double

        MatrixSetToXAngle(matrixX, AnglePY.Pitch)
        MatrixSetToYAngle(matrixY, AnglePY.Yaw)
        MatrixRotationByMatrix(matrixY, matrixX, Matrix_Output)
    End Sub

    Sub MatrixNormalize(ByRef Matrix() As Double)
        Dim AngleRPY As sAngleRPY

        MatrixToRPY(Matrix, AngleRPY)
        MatrixSetToRPY(Matrix, AngleRPY)
    End Sub
End Module