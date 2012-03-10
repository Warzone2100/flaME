
Public Class clsObjectData

    Public UnitTypes As New ConnectedList(Of clsUnitType, clsObjectData)(Me)

    Public FeatureTypes As New ConnectedList(Of clsFeatureType, clsObjectData)(Me)
    Public StructureTypes As New ConnectedList(Of clsStructureType, clsObjectData)(Me)
    Public DroidTemplates As New ConnectedList(Of clsDroidTemplate, clsObjectData)(Me)

    Public WallTypes As New ConnectedList(Of clsWallType, clsObjectData)(Me)
End Class
