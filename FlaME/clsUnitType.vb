Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsUnitType

    Public IsUnknown As Boolean = False

    Enum enumType As Byte
        Unspecified
        Feature
        PlayerStructure
        PlayerDroid
    End Enum
    Public Type As enumType

    Public Class clsAttachment
        Public Pos_Offset As sXYZ_sng
        Public AngleOffsetMatrix As New Matrix3D.Matrix3D
        Public Models() As clsModel
        Public ModelCount As Integer
        Public Attachments() As clsAttachment
        Public AttachmentCount As Integer

        Public Sub New()

            Matrix3D.MatrixSetToIdentity(AngleOffsetMatrix)
        End Sub

        Public Sub GLDraw()
            Dim AngleRPY As Matrix3D.AngleRPY
            Dim matrixA As New Matrix3D.Matrix3D
            Dim A As Integer

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
                Matrix3D.MatrixInvert(Attachments(A).AngleOffsetMatrix, matrixA)
                Matrix3D.MatrixToRPY(matrixA, AngleRPY)
                GL.Translate(Attachments(A).Pos_Offset.X, Attachments(A).Pos_Offset.Y, -Attachments(A).Pos_Offset.Z)
                GL.Rotate(-AngleRPY.Roll / RadOf1Deg, 0.0F, 0.0F, 1.0F)
                GL.Rotate(AngleRPY.Pitch / RadOf1Deg, 1.0F, 0.0F, 0.0F)
                GL.Rotate(AngleRPY.Yaw / RadOf1Deg, 0.0F, 1.0F, 0.0F)
                Attachments(A).GLDraw()
                GL.PopMatrix()
            Next
        End Sub

        Public Function CreateAttachment() As clsAttachment

            ReDim Preserve Attachments(AttachmentCount)
            CreateAttachment = New clsAttachment
            Attachments(AttachmentCount) = CreateAttachment
            AttachmentCount += 1
        End Function

        Public Function AddCopyOfAttachment(ByVal AttachmentToCopy As clsAttachment) As clsAttachment
            Dim tmpAttachment As New clsAttachment
            Dim A As Integer

            ReDim Preserve Attachments(AttachmentCount)
            Matrix3D.MatrixCopy(AttachmentToCopy.AngleOffsetMatrix, tmpAttachment.AngleOffsetMatrix)
            For A = 0 To AttachmentToCopy.ModelCount - 1
                tmpAttachment.AddModel(AttachmentToCopy.Models(A))
            Next
            For A = 0 To AttachmentToCopy.AttachmentCount - 1
                tmpAttachment.AddCopyOfAttachment(AttachmentToCopy.Attachments(A))
            Next
            Attachments(AttachmentCount) = tmpAttachment
            AttachmentCount += 1

            Return tmpAttachment
        End Function

        Public Sub AddModel(ByVal NewModel As clsModel)

            If NewModel Is Nothing Then
                Exit Sub
            End If

            ReDim Preserve Models(ModelCount)
            Models(ModelCount) = NewModel
            ModelCount += 1
        End Sub
    End Class

    Public Sub GLDraw(ByVal RotationDegrees As Single)

        Select Case Draw_Lighting
            Case enumDrawLighting.Off
                GL.Color3(1.0F, 1.0F, 1.0F)
            Case enumDrawLighting.Half
                GL.Color3(0.875F, 0.875F, 0.875F)
            Case enumDrawLighting.Normal
                GL.Color3(0.75F, 0.75F, 0.75F)
        End Select
        'GL.Rotate(x, 1.0F, 0.0F, 0.0F)
        GL.Rotate(RotationDegrees, 0.0F, 1.0F, 0.0F)
        'GL.Rotate(z, 0.0F, 0.0F, -1.0F)

        TypeGLDraw()
    End Sub

    Protected Overridable Sub TypeGLDraw()

    End Sub

    Public ReadOnly Property GetFootprint As sXY_int
        Get
            Select Case Type
                Case enumType.Feature
                    Return CType(Me, clsFeatureType).Footprint
                Case enumType.PlayerStructure
                    Return CType(Me, clsStructureType).Footprint
                Case Else
                    Dim XY_int As New sXY_int(1, 1)
                    Return XY_int
            End Select
        End Get
    End Property

    Public Function GetCode(ByRef Result As String) As Boolean

        Select Case Type
            Case enumType.Feature
                Result = CType(Me, clsFeatureType).Code
                Return True
            Case enumType.PlayerStructure
                Result = CType(Me, clsStructureType).Code
                Return True
            Case enumType.PlayerDroid
                Dim tmpDroid As clsDroidDesign = CType(Me, clsDroidDesign)
                If tmpDroid.IsTemplate Then
                    Result = CType(Me, clsDroidTemplate).Code
                    Return True
                Else
                    Result = Nothing
                    Return False
                End If
            Case Else
                Result = Nothing
                Return False
        End Select
    End Function

    Public Function GetDisplayText() As String

        Select Case Type
            Case enumType.Feature
                Dim tmpFeature As clsFeatureType = CType(Me, clsFeatureType)
                Return tmpFeature.Code & " (" & tmpFeature.Name & ")"
            Case enumType.PlayerStructure
                Dim tmpStructure As clsStructureType = CType(Me, clsStructureType)
                Return tmpStructure.Code & " (" & tmpStructure.Name & ")"
            Case enumType.PlayerDroid
                Dim tmpDroid As clsDroidDesign = CType(Me, clsDroidDesign)
                If tmpDroid.IsTemplate Then
                    Dim tmpTemplate As clsDroidTemplate = CType(Me, clsDroidTemplate)
                    Return tmpTemplate.Code & " (" & tmpTemplate.Name & ")"
                Else
                    Return "<droid> (" & tmpDroid.GenerateName & ")"
                End If
            Case Else
                Return ""
        End Select
    End Function
End Class

Public Class clsFeatureType
    Inherits clsUnitType

    Public Code As String = ""
    Public Name As String = "Unknown"
    Public Footprint As sXY_int
    Public Enum enumFeatureType As Byte
        Unknown
        OilResource
    End Enum
    Public FeatureType As enumFeatureType = enumFeatureType.Unknown

    Public BaseAttachment As clsUnitType.clsAttachment

    Public Sub New()

        Type = enumType.Feature
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
    End Sub
End Class

Public Class clsStructureType
    Inherits clsUnitType

    Public StructureNum As Integer = -1

    Public Code As String = ""
    Public Name As String = "Unknown"
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
    End Enum
    Public StructureType As enumStructureType = enumStructureType.Unknown

    Public BaseAttachment As New clsUnitType.clsAttachment
    Public StructureBasePlate As clsModel

    Public Sub New()

        Type = enumType.PlayerStructure
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
        If StructureBasePlate IsNot Nothing Then
            StructureBasePlate.GLDraw()
        End If
    End Sub
End Class

Public Class clsDroidDesign
    Inherits clsUnitType

    Public IsTemplate As Boolean

    Public Name As String = ""

    Public Class clsTemplateDroidType

        Public Num As Integer = -1

        Public Name As String

        Public TemplateCode As String

        Public Sub New(ByVal NewName As String, ByVal NewTemplateCode As String)

            Name = NewName
            TemplateCode = NewTemplateCode
        End Sub
    End Class
    Public TemplateDroidType As clsTemplateDroidType

    Public Body As clsBody
    Public Propulsion As clsPropulsion
    Public TurretCount As Byte
    Public Turret1 As clsTurret
    Public Turret2 As clsTurret
    Public Turret3 As clsTurret

    Public BaseAttachment As New clsUnitType.clsAttachment

    Public AlwaysDrawTextLabel As Boolean

    Public Sub New()

        Type = enumType.PlayerDroid
    End Sub

    Public Sub CopyDesign(ByVal DroidTypeToCopy As clsDroidDesign)

        TemplateDroidType = DroidTypeToCopy.TemplateDroidType
        Body = DroidTypeToCopy.Body
        Propulsion = DroidTypeToCopy.Propulsion
        TurretCount = DroidTypeToCopy.TurretCount
        Turret1 = DroidTypeToCopy.Turret1
        Turret2 = DroidTypeToCopy.Turret2
        Turret3 = DroidTypeToCopy.Turret3
    End Sub

    Protected Overrides Sub TypeGLDraw()

        If BaseAttachment IsNot Nothing Then
            BaseAttachment.GLDraw()
        End If
    End Sub

    Public Sub UpdateAttachments()

        BaseAttachment = New clsUnitType.clsAttachment

        If Body Is Nothing Then
            AlwaysDrawTextLabel = True
            Exit Sub
        End If

        Dim NewBody As clsUnitType.clsAttachment = BaseAttachment.AddCopyOfAttachment(Body.Attachment)

        AlwaysDrawTextLabel = (NewBody.ModelCount = 0)

        If Propulsion IsNot Nothing Then
            If Body.Num >= 0 Then
                BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies(Body.Num).LeftAttachment)
                BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies(Body.Num).RightAttachment)
            End If
        End If

        If NewBody.ModelCount = 0 Then
            Exit Sub
        End If

        If NewBody.Models(0).ConnectorCount <= 0 Then
            Exit Sub
        End If

        Dim TurretConnector As sXYZ_sng

        TurretConnector = Body.Attachment.Models(0).Connectors(0)

        If TurretCount >= 1 Then
            If Turret1 IsNot Nothing Then
                Dim NewTurret As clsUnitType.clsAttachment = NewBody.AddCopyOfAttachment(Turret1.Attachment)
                NewTurret.Pos_Offset = TurretConnector
            End If
        End If

        If Body.Attachment.Models(0).ConnectorCount <= 1 Then
            Exit Sub
        End If

        TurretConnector = Body.Attachment.Models(0).Connectors(1)

        If TurretCount >= 2 Then
            If Turret2 IsNot Nothing Then
                Dim NewTurret As clsUnitType.clsAttachment = NewBody.AddCopyOfAttachment(Turret2.Attachment)
                NewTurret.Pos_Offset = TurretConnector
            End If
        End If
    End Sub

    Public Function GetMaxHitPoints() As Integer
        Dim Result As Integer

        'this is inaccurate

        If Body Is Nothing Then
            Return 0
        End If
        Result = Body.Hitpoints
        If Propulsion Is Nothing Then
            Return Result
        End If
        Result += CInt(Body.Hitpoints * Propulsion.Hitpoints / 100.0#)
        If Turret1 Is Nothing Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret1.HitPoints
        If TurretCount < 2 Or Turret2 Is Nothing Then
            Return Result
        End If
        If Turret2.TurretType <> clsTurret.enumTurretType.Weapon Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret2.HitPoints
        If TurretCount < 3 Or Turret3 Is Nothing Then
            Return Result
        End If
        If Turret3.TurretType <> clsTurret.enumTurretType.Weapon Then
            Return Result
        End If
        Result += Body.Hitpoints + Turret3.HitPoints
        Return Result
    End Function

    Public Structure sLoadPartsArgs
        Public Body As clsBody
        Public Propulsion As clsPropulsion
        Public Construct As clsConstruct
        Public Sensor As clsSensor
        Public Repair As clsRepair
        Public Brain As clsBrain
        Public ECM As clsECM
        Public Weapon1 As clsWeapon
        Public Weapon2 As clsWeapon
        Public Weapon3 As clsWeapon
    End Structure

    Public Function LoadParts(ByVal Args As sLoadPartsArgs) As Boolean
        Dim TurretConflict As Boolean

        Body = Args.Body
        Propulsion = Args.Propulsion

        TurretConflict = False
        If Args.Construct IsNot Nothing Then
            If Args.Construct.Code <> "ZNULLCONSTRUCT" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Construct
            End If
        End If
        If Args.Repair IsNot Nothing Then
            If Args.Repair.Code <> "ZNULLREPAIR" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Repair
            End If
        End If
        If Args.Brain IsNot Nothing Then
            If Args.Brain.Code <> "ZNULLBRAIN" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Brain
            End If
        End If
        If Args.Weapon1 IsNot Nothing Then
            Dim UseWeapon As Boolean
            If Turret1 IsNot Nothing Then
                If Turret1.TurretType = clsTurret.enumTurretType.Brain Then
                    UseWeapon = False
                Else
                    UseWeapon = True
                    TurretConflict = True
                End If
            Else
                UseWeapon = True
            End If
            If UseWeapon Then
                TurretCount = 1
                Turret1 = Args.Weapon1
                If Args.Weapon2 IsNot Nothing Then
                    Turret2 = Args.Weapon2
                    TurretCount += CByte(1)
                    If Args.Weapon3 IsNot Nothing Then
                        Turret3 = Args.Weapon3
                        TurretCount += CByte(1)
                    End If
                End If
            End If
        End If
        If Args.Sensor IsNot Nothing Then
            If Args.Sensor.Code <> "ZNULLSENSOR" And Args.Sensor.Code <> "DefaultSensor1Mk1" And Args.Sensor.Code <> "NavGunSensor" Then
                If Turret1 IsNot Nothing Then
                    TurretConflict = True
                End If
                TurretCount = 1
                Turret1 = Args.Sensor
            End If
        End If
        UpdateAttachments()

        Return (Not TurretConflict) 'return if all is ok
    End Function

    Public Function GenerateName() As String
        Dim Result As String = ""

        If Propulsion IsNot Nothing Then
            If Result.Length > 0 Then
                Result = " "c & Result
            End If
            Result = Propulsion.Name & Result
        End If

        If Body IsNot Nothing Then
            If Result.Length > 0 Then
                Result = " "c & Result
            End If
            Result = Body.Name & Result
        End If

        If TurretCount >= 3 Then
            If Turret3 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret3.Name & Result
            End If
        End If

        If TurretCount >= 2 Then
            If Turret2 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret2.Name & Result
            End If
        End If

        If TurretCount >= 1 Then
            If Turret1 IsNot Nothing Then
                If Result.Length > 0 Then
                    Result = " "c & Result
                End If
                Result = Turret1.Name & Result
            End If
        End If

        Return Result
    End Function

    Public Function GetDroidType() As enumDroidType
        Dim Result As enumDroidType

        If TemplateDroidType Is TemplateDroidType_Null Then
            Result = enumDroidType.Default_
        ElseIf TemplateDroidType Is TemplateDroidType_Person Then
            Result = enumDroidType.Person
        ElseIf TemplateDroidType Is TemplateDroidType_Cyborg Then
            Result = enumDroidType.Cyborg
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgSuper Then
            Result = enumDroidType.Cyborg_Super
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgConstruct Then
            Result = enumDroidType.Cyborg_Construct
        ElseIf TemplateDroidType Is TemplateDroidType_CyborgRepair Then
            Result = enumDroidType.Cyborg_Repair
        ElseIf TemplateDroidType Is TemplateDroidType_Transporter Then
            Result = enumDroidType.Transporter
        ElseIf Turret1 Is Nothing Then
            Result = enumDroidType.Default_
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Brain Then
            Result = enumDroidType.Command
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Sensor Then
            Result = enumDroidType.Sensor
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.ECM Then
            Result = enumDroidType.ECM
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Construct Then
            Result = enumDroidType.Construct
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Repair Then
            Result = enumDroidType.Repair
        ElseIf Turret1.TurretType = clsTurret.enumTurretType.Weapon Then
            Result = enumDroidType.Weapon
        Else
            Result = enumDroidType.Default_
        End If
        Return Result
    End Function

    Public Function SetDroidType(ByVal DroidType As enumDroidType) As Boolean

        Select Case DroidType
            Case enumDroidType.Weapon
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Sensor
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.ECM
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Construct
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Person
                TemplateDroidType = TemplateDroidType_Person
            Case enumDroidType.Cyborg
                TemplateDroidType = TemplateDroidType_Cyborg
            Case enumDroidType.Transporter
                TemplateDroidType = TemplateDroidType_Transporter
            Case enumDroidType.Command
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Repair
                TemplateDroidType = TemplateDroidType_Droid
            Case enumDroidType.Default_
                TemplateDroidType = TemplateDroidType_Null
            Case enumDroidType.Cyborg_Construct
                TemplateDroidType = TemplateDroidType_CyborgConstruct
            Case enumDroidType.Cyborg_Repair
                TemplateDroidType = TemplateDroidType_CyborgRepair
            Case enumDroidType.Cyborg_Super
                TemplateDroidType = TemplateDroidType_CyborgSuper
            Case Else
                TemplateDroidType = Nothing
                Return False
        End Select
        Return True
    End Function

    Public Function GetConstructCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Construct Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLCONSTRUCT"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetRepairCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Repair Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLREPAIR"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetSensorCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Sensor Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLSENSOR"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetBrainCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.Brain Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLBRAIN"
        Else
            Return Turret1.Code
        End If
    End Function

    Public Function GetECMCode() As String
        Dim NotThis As Boolean

        If TurretCount >= 1 Then
            If Turret1 Is Nothing Then
                NotThis = True
            ElseIf Turret1.TurretType <> clsTurret.enumTurretType.ECM Then
                NotThis = True
            Else
                NotThis = False
            End If
        Else
            NotThis = True
        End If

        If NotThis Then
            Return "ZNULLECM"
        Else
            Return Turret1.Code
        End If
    End Function
End Class

Public Class clsDroidTemplate
    Inherits clsDroidDesign

    Public Code As String = ""

    Public Sub New()

        IsTemplate = True
        Name = "Unknown"
    End Sub
End Class