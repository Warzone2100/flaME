Imports OpenTK.Graphics.OpenGL

Public Class clsUnitType

    Public Num As Integer

    Enum enumType As Byte
        Feature
        PlayerStructure
        PlayerDroidTemplate
    End Enum
    Public Type As enumType

    Public Code As String
    Public LoadedInfo As clsLoadedInfo
    Public Class clsLoadedInfo
        Public Num As Integer = -1

        Public Name As String
        Public Footprint As sXY_int
        Public Enum enumStructureType As Byte
            Unknown
            Demolish
            Wall
            CornerWall
            Factory
            CyborgFactory
            VTOLFactory
            Command
            HQ
            Defense
            PowerGenerator
            PowerModule
            Research
            ResearchModule
            FactoryModule
            DOOR
            RepairFacility
            SatUplink
            RearmPad
            MissileSilo
            ResourceExtractor
            OilResource
        End Enum
        Public StructureType As enumStructureType = enumStructureType.Unknown

        Public BaseAttachment As clsAttachment
        Public StructureBasePlate As clsModel

        Class clsAttachment
            Public Pos_Offset As sXYZ_sng
            Public Angle_Offset_Matrix(8) As Double 'rotated X axis, then Y axis, then Z axis
            Public Models() As clsModel
            Public ModelCount As Integer
            Public Attachments() As clsAttachment
            Public AttachmentCount As Integer

            Sub New()
                Matrix_Set_Identity(Angle_Offset_Matrix)
            End Sub

            Sub GLDraw()
                Static AngleRPY As sAngleRPY
                Static matrixA(8) As Double
                Static A As Integer

                'Matrix_Invert(Angle_Matrix, matrixA)
                'Matrix_Get_RPY(matrixA, AngleRPY)
                'gl.Rotatef(-AngleRPY.Roll / Rad1Deg, 0.0F, 0.0F, 1.0F)
                'gl.Rotatef(AngleRPY.Pitch / Rad1Deg, 1.0F, 0.0F, 0.0F)
                'gl.Rotatef(AngleRPY.Yaw / Rad1Deg, 0.0F, 1.0F, 0.0F)
               
                For A = 0 To ModelCount - 1
                    Models(A).GLDraw()
                Next

                For A = 0 To AttachmentCount - 1
                    GL.PushMatrix()
                    Matrix_Invert(Attachments(A).Angle_Offset_Matrix, matrixA)
                    Matrix_Get_RPY(matrixA, AngleRPY)
                    GL.Translate(Attachments(A).Pos_Offset.X, Attachments(A).Pos_Offset.Y, -Attachments(A).Pos_Offset.Z)
                    GL.Rotate(-AngleRPY.Roll / RadOf1Deg, 0.0F, 0.0F, 1.0F)
                    GL.Rotate(AngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
                    GL.Rotate(AngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
                    Attachments(A).GLDraw()
                    GL.PopMatrix()
                Next
            End Sub

            Function CreateAttachment() As clsAttachment

                ReDim Preserve Attachments(AttachmentCount)
                CreateAttachment = New clsAttachment
                Attachments(AttachmentCount) = CreateAttachment
                AttachmentCount += 1
            End Function

            Sub AddModel(ByVal NewModel As clsModel)

                If NewModel Is Nothing Then
                    Exit Sub
                End If

                ReDim Preserve Models(ModelCount)
                Models(ModelCount) = NewModel
                ModelCount += 1
            End Sub
        End Class

        Sub GLDraw(ByVal RotationDegrees As Single)

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, TextureEnvMode.Modulate)
            GL.Color3(1.0F, 1.0F, 1.0F)
            'GL.Rotate(x, 1.0F, 0.0F, 0.0F)
            GL.Rotate(RotationDegrees, 0.0F, 1.0F, 0.0F)
            'GL.Rotate(z, 0.0F, 0.0F, -1.0F)
            If BaseAttachment IsNot Nothing Then
                BaseAttachment.GLDraw()
            End If
            If StructureBasePlate IsNot Nothing Then
                StructureBasePlate.GLDraw()
            End If
        End Sub
    End Class

    Public Sub New(ByVal NewNum As Integer)

        Num = NewNum
    End Sub
End Class