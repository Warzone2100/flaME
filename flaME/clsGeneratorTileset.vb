Public Class clsGeneratorTileset

    Public Tileset As clsTileset

    Public Structure sUnitChance
        Public Type As clsUnitType
        Public Chance As UInteger

        Public Sub New(ByVal NewType As clsUnitType, ByVal NewChance As UInteger)

            Type = NewType
            Chance = NewChance
        End Sub
    End Structure
    Public ScatteredUnits(-1) As sUnitChance
    Public ScatteredUnitCount As Integer
    Public ScatteredUnitChanceTotal As Integer
    Public ClusteredUnits(-1) As sUnitChance
    Public ClusteredUnitCount As Integer
    Public ClusteredUnitChanceTotal As Integer

    Public BorderTextureNum As Integer = -1

    Public OldTextureLayers As frmMapTexturer.sLayerList

    Public Sub ScatteredUnit_Add(ByVal NewUnit As sUnitChance)

        If NewUnit.Type Is Nothing Then
            Exit Sub
        End If

        ScatteredUnitChanceTotal += NewUnit.Chance

        ReDim Preserve ScatteredUnits(ScatteredUnitCount)
        ScatteredUnits(ScatteredUnitCount) = NewUnit
        ScatteredUnitCount += 1
    End Sub

    Public Sub ClusteredUnit_Add(ByVal NewUnit As sUnitChance)

        If NewUnit.Type Is Nothing Then
            Exit Sub
        End If

        ClusteredUnitChanceTotal += NewUnit.Chance

        ReDim Preserve ClusteredUnits(ClusteredUnitCount)
        ClusteredUnits(ClusteredUnitCount) = NewUnit
        ClusteredUnitCount += 1
    End Sub
End Class