Public MustInherit Class clsComponent

    Public IsUnknown As Boolean = False

    Public Name As String = "Unknown"
    Public Code As String
    Public Enum enumComponentType As Byte
        Unspecified
        Body
        Propulsion
        Turret
    End Enum
    Public ComponentType As enumComponentType = enumComponentType.Unspecified

    Public Designable As Boolean
End Class

Public Class clsBody
    Inherits clsComponent

    Public Num As Integer = -1

    Public Attachment As New clsUnitType.clsAttachment
    Public Hitpoints As Integer

    Public Sub New()

        ComponentType = enumComponentType.Body
    End Sub
End Class

Public Class clsPropulsion
    Inherits clsComponent

    Public Structure sBody
        Public LeftAttachment As clsUnitType.clsAttachment
        Public RightAttachment As clsUnitType.clsAttachment
    End Structure
    Public Bodies(-1) As sBody
    Public Hitpoints As Integer

    Public Sub New(ByVal BodyCount As Integer)

        ComponentType = enumComponentType.Propulsion

        Dim A As Integer

        ReDim Bodies(BodyCount - 1)
        For A = 0 To BodyCount - 1
            Bodies(A).LeftAttachment = New clsUnitType.clsAttachment
            Bodies(A).RightAttachment = New clsUnitType.clsAttachment
        Next
    End Sub
End Class

Public Class clsTurret
    Inherits clsComponent

    Public Attachment As New clsUnitType.clsAttachment
    Public HitPoints As Integer
    Public Enum enumTurretType As Byte
        Unknown
        Weapon
        Construct
        Repair
        Sensor
        Brain
        ECM
    End Enum
    Public TurretType As enumTurretType = enumTurretType.Unknown

    Public Function GetTurretTypeName(ByRef Result As String) As Boolean

        Select Case TurretType
            Case enumTurretType.Weapon
                Result = "Weapon"
                Return True
            Case enumTurretType.Construct
                Result = "Construct"
                Return True
            Case enumTurretType.Repair
                Result = "Repair"
                Return True
            Case enumTurretType.Sensor
                Result = "Sensor"
                Return True
            Case enumTurretType.Brain
                Result = "Brain"
                Return True
            Case enumTurretType.ECM
                Result = "ECM"
                Return True
            Case Else
            	Result = Nothing
                Return False
        End Select
    End Function
End Class

Public Class clsWeapon
    Inherits clsTurret

    Public Sub New()

        TurretType = enumTurretType.Weapon
    End Sub
End Class

Public Class clsConstruct
    Inherits clsTurret

    Public Sub New()

        TurretType = enumTurretType.Construct
    End Sub
End Class

Public Class clsRepair
    Inherits clsTurret

    Public Sub New()

        TurretType = enumTurretType.Repair
    End Sub
End Class

Public Class clsSensor
    Inherits clsTurret

    Public Sub New()

        TurretType = enumTurretType.Sensor
    End Sub
End Class

Public Class clsBrain
    Inherits clsTurret

    Public Weapon As clsWeapon

    Public Sub New()

        TurretType = enumTurretType.Brain
    End Sub
End Class

Public Class clsECM
    Inherits clsTurret

    Public Sub New()

        TurretType = enumTurretType.ECM
    End Sub
End Class