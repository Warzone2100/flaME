Module modWZME

    Structure sWZMEv2
        Structure sHeader
            Dim TerrainSize As sXY_int
            Dim Tileset_Val As Byte
            Dim HeightMultiplier As Byte
        End Structure
        Dim Header As sHeader
        Structure sVertex
            Dim Height As Byte
            Dim AutoTexture As Byte
        End Structure
        Dim Vertex(,) As sVertex
        Structure sTile
            Dim Texture As Byte
            Dim Attributes As Byte
            Dim Attributes2 As Byte
        End Structure
        Dim Tile(,) As sTile
        Structure sSide
            Dim AutoRoad As Byte
        End Structure
        Dim SideH(,) As sSide
        Dim SideV(,) As sSide
        Structure sUnit
            Dim Code As String
            Dim ID As Integer
            Dim Type As Byte
            Dim Pos As sXYZ_int
            Dim Rotation As sXYZ_int
            Dim Name As String
            Dim Player As Byte
        End Structure
        Dim Units() As sUnit
        Dim UnitCount As Integer
    End Structure

    Structure sWZME3Unit
        Dim Code As String
        Dim ID As UInteger
        Dim SavePriority As Integer
        Dim LNDType As Byte
        Dim X As UInteger
        Dim Y As UInteger
        Dim Z As UInteger
        Dim Rotation As UShort
        Dim Name As String
        Dim Player As Byte
    End Structure
End Module