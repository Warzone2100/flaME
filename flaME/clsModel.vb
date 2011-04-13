Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsModel

    Public GL_Texture As Integer
    Structure sTriangle
        Dim PosA As sXYZ_sng
        Dim PosB As sXYZ_sng
        Dim PosC As sXYZ_sng
        Dim TexCoordA As sXY_sng
        Dim TexCoordB As sXY_sng
        Dim TexCoordC As sXY_sng
    End Structure
    Public Triangles() As sTriangle
    Public TriangleCount As Integer

    Structure sQuad
        Dim PosA As sXYZ_sng
        Dim PosB As sXYZ_sng
        Dim PosC As sXYZ_sng
        Dim PosD As sXYZ_sng
        Dim TexCoordA As sXY_sng
        Dim TexCoordB As sXY_sng
        Dim TexCoordC As sXY_sng
        Dim TexCoordD As sXY_sng
    End Structure
    Public Quads() As sQuad
    Public QuadCount As Integer

    Function GLList_Create() As Integer

        GLList_Create = GL.GenLists(1)

        If GLList_Create = 0 Then
            Exit Function
        End If

        GL.NewList(GLList_Create, ListMode.Compile)

        GLDraw()

        GL.EndList()
    End Function

    Sub GLDraw()
        Dim A As Integer

        GL.BindTexture(TextureTarget.Texture2D, GL_Texture)

        GL.Begin(BeginMode.Triangles)
        For A = 0 To TriangleCount - 1
            With Triangles(A)
                GL.TexCoord2(.TexCoordA.X, .TexCoordA.Y)
                GL.Vertex3(.PosA.X, .PosA.Y, -.PosA.Z)
                GL.TexCoord2(.TexCoordB.X, .TexCoordB.Y)
                GL.Vertex3(.PosB.X, .PosB.Y, -.PosB.Z)
                GL.TexCoord2(.TexCoordC.X, .TexCoordC.Y)
                GL.Vertex3(.PosC.X, .PosC.Y, -.PosC.Z)
            End With
        Next A
        GL.End()
        GL.Begin(BeginMode.Quads)
        For A = 0 To QuadCount - 1
            With Quads(A)
                GL.TexCoord2(.TexCoordA.X, .TexCoordA.Y)
                GL.Vertex3(.PosA.X, .PosA.Y, -.PosA.Z)
                GL.TexCoord2(.TexCoordB.X, .TexCoordB.Y)
                GL.Vertex3(.PosB.X, .PosB.Y, -.PosB.Z)
                GL.TexCoord2(.TexCoordC.X, .TexCoordC.Y)
                GL.Vertex3(.PosC.X, .PosC.Y, -.PosC.Z)
                GL.TexCoord2(.TexCoordD.X, .TexCoordD.Y)
                GL.Vertex3(.PosD.X, .PosD.Y, -.PosD.Z)
            End With
        Next A
        GL.End()
    End Sub

    Public Connectors(-1) As sXYZ_sng
    Public ConnectorCount As Integer

    Structure sPIELevel
        Structure sPolygon
            Dim PointNum() As Integer
            Dim TexCoord() As sXY_int
            Dim PointCount As Integer
        End Structure
        Dim Polygon() As sPolygon
        Dim PolygonCount As Integer
        Dim Point() As sXYZ_sng
        Dim PointCount As Integer
    End Structure

    Function LoadPIE(ByVal Path As String) As sResult
        LoadPIE.Success = False
        LoadPIE.Problem = ""

        Dim A As Integer
        Dim B As Integer
        Dim LineCount As Integer

        Dim LineData(-1) As String

        If Not IO.File.Exists(Path) Then
            LoadPIE.Problem = "PIE missing."
            Exit Function
        End If

        Dim tmpBytes() As Byte
        Try
            tmpBytes = IO.File.ReadAllBytes(Path)
        Catch ex As Exception
            LoadPIE.Problem = ex.Message
            Exit Function
        End Try
        BytesToLines(tmpBytes, LineData)
        LineCount = LineData.GetUpperBound(0) + 1

        Dim strTemp As String
        Dim SplitText() As String
        Dim LevelCount As Integer
        Dim QuadCount As Integer
        Dim TriCount As Integer
        Dim C As Integer
        Dim TextureName As String = ""
        Dim LineNum As Integer
        Dim Level() As sPIELevel
        Dim LevelNum As Integer
        Dim GotText As Boolean
        Dim strTemp2 As String
        Dim D As Integer

        ReDim Level(-1)
        LineNum = -1
        LevelNum = -1
        Do
            LineNum += 1
            If LineNum >= LineCount Then
                Exit Do
            End If
            strTemp = LineData(LineNum)

Reeval:
            If Left(strTemp, 3) = "PIE" Then
            ElseIf Left(strTemp, 4) = "TYPE" Then
            ElseIf Left(strTemp, 7) = "TEXTURE" Then
                TextureName = Right(strTemp, strTemp.Length - 10)
                A = InStr(TextureName, " ")
                TextureName = Left(TextureName, A - 1)
            ElseIf Left(strTemp, 6) = "LEVELS" Then
                LevelCount = Right(strTemp, strTemp.Length - 7)
                ReDim Level(LevelCount - 1)
            ElseIf Left(strTemp, 6) = "LEVEL " Then
                LevelNum = Right(strTemp, strTemp.Length - 6) - 1
            ElseIf Left(strTemp, 6) = "POINTS" Then
                Level(LevelNum).PointCount = Right(strTemp, strTemp.Length - 7)
                ReDim Level(LevelNum).Point(Level(LevelNum).PointCount - 1)
                A = 0
                Do
                    LineNum += 1
                    If LineNum >= LineCount Then
                        Exit Do
                    End If
                    strTemp = LineData(LineNum)

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        ReDim SplitText(2)
                        C = 0
                        SplitText(0) = ""
                        GotText = False
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) <> " "c And strTemp.Chars(B) <> Chr(9) Then
                                GotText = True
                                SplitText(C) &= strTemp.Chars(B)
                            Else
                                If GotText Then
                                    C += 1
                                    If C = 3 Then
                                        Exit For
                                    End If
                                    SplitText(C) = ""
                                    GotText = False
                                End If
                            End If
                        Next

                        Level(LevelNum).Point(A).X = Val(SplitText(0))
                        Level(LevelNum).Point(A).Y = Val(SplitText(1))
                        Level(LevelNum).Point(A).Z = Val(SplitText(2))
                        A += 1
                    Else
                        GoTo Reeval
                    End If
                Loop
            ElseIf Left(strTemp, 8) = "POLYGONS" Then
                Level(LevelNum).PolygonCount = Right(strTemp, strTemp.Length - 9)
                ReDim Level(LevelNum).Polygon(Level(LevelNum).PolygonCount - 1)
                A = 0
                Do
                    LineNum += 1
                    If LineNum >= LineCount Then
                        Exit Do
                    End If
                    strTemp = LineData(LineNum)

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        C = 0
                        ReDim SplitText(0)
                        SplitText(0) = ""
                        GotText = False
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) <> " "c And strTemp.Chars(B) <> Chr(9) Then
                                GotText = True
                                SplitText(C) &= strTemp.Chars(B)
                            Else
                                If GotText Then
                                    C += 1
                                    ReDim Preserve SplitText(C)
                                    SplitText(C) = ""
                                    GotText = False
                                End If
                            End If
                        Next

                        D = 0
                        Do
                            'flag, numpoints, points[], x4 ignore if animated, texcoord[]xy
                            Level(LevelNum).Polygon(A).PointCount = SplitText(D + 1)
                            ReDim Level(LevelNum).Polygon(A).PointNum(Level(LevelNum).Polygon(A).PointCount - 1)
                            ReDim Level(LevelNum).Polygon(A).TexCoord(Level(LevelNum).Polygon(A).PointCount - 1)
                            If Level(LevelNum).Polygon(A).PointCount = 3 Then
                                TriCount += 1
                            ElseIf Level(LevelNum).Polygon(A).PointCount = 4 Then
                                QuadCount += 1
                            End If
                            For B = 0 To Level(LevelNum).Polygon(A).PointCount - 1
                                Level(LevelNum).Polygon(A).PointNum(B) = SplitText(D + 2 + B)
                            Next
                            C = D + 2 + Level(LevelNum).Polygon(A).PointCount
                            If SplitText(D) = "4200" Or SplitText(D) = "4000" Or SplitText(D) = "6a00" Or SplitText(D) = "4a00" Or SplitText(D) = "6200" Then
                                C += 4
                            End If
                            For B = 0 To Level(LevelNum).Polygon(A).PointCount - 1
                                Level(LevelNum).Polygon(A).TexCoord(B).X = Val(SplitText(C))
                                Level(LevelNum).Polygon(A).TexCoord(B).Y = Val(SplitText(C + 1))
                                C += 2
                            Next
                            D = C
                            A += 1
                        Loop While D < SplitText.GetUpperBound(0)
                    Else
                        GoTo Reeval
                    End If
                Loop
            ElseIf Left(strTemp, 10) = "CONNECTORS" Then
                ConnectorCount = Right(strTemp, strTemp.Length - 11)
                ReDim Connectors(ConnectorCount - 1)
                A = 0
                Do
                    LineNum += 1
                    If LineNum >= LineCount Then
                        Exit Do
                    End If
                    strTemp = LineData(LineNum)

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        ReDim SplitText(2)
                        C = 0
                        SplitText(0) = ""
                        GotText = False
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) <> " "c And strTemp.Chars(B) <> Chr(9) Then
                                GotText = True
                                SplitText(C) &= strTemp.Chars(B)
                            Else
                                If GotText Then
                                    C += 1
                                    If C = 3 Then
                                        Exit For
                                    End If
                                    SplitText(C) = ""
                                    GotText = False
                                End If
                            End If
                        Next

                        Connectors(A).X = Val(SplitText(0))
                        Connectors(A).Y = Val(SplitText(2))
                        Connectors(A).Z = Val(SplitText(1))
                        A += 1
                    Else
                        GoTo Reeval
                    End If
                Loop
            Else
            End If
        Loop

        GL_Texture = Get_TexturePage_GLTexture(Left(TextureName, TextureName.Length - 4))

        'If GL_Texture = 0 Then
        '   MsgBox("PIE (" & Path & ") cant find OpenGL texture for " & TextureName & ".")
        'End If

        TriangleCount = TriCount
        QuadCount = QuadCount
        ReDim Triangles(TriangleCount - 1)
        ReDim Quads(QuadCount - 1)
        TriCount = 0
        QuadCount = 0
        For LevelNum = 0 To LevelCount - 1
            For A = 0 To Level(LevelNum).PolygonCount - 1
                If Level(LevelNum).Polygon(A).PointCount = 3 Then
                    Triangles(TriCount).PosA = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(0))
                    Triangles(TriCount).PosB = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(1))
                    Triangles(TriCount).PosC = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(2))
                    Triangles(TriCount).TexCoordA.X = Level(LevelNum).Polygon(A).TexCoord(0).X / 255.0#
                    Triangles(TriCount).TexCoordA.Y = Level(LevelNum).Polygon(A).TexCoord(0).Y / 255.0#
                    Triangles(TriCount).TexCoordB.X = Level(LevelNum).Polygon(A).TexCoord(1).X / 255.0#
                    Triangles(TriCount).TexCoordB.Y = Level(LevelNum).Polygon(A).TexCoord(1).Y / 255.0#
                    Triangles(TriCount).TexCoordC.X = Level(LevelNum).Polygon(A).TexCoord(2).X / 255.0#
                    Triangles(TriCount).TexCoordC.Y = Level(LevelNum).Polygon(A).TexCoord(2).Y / 255.0#
                    TriCount += 1
                ElseIf Level(LevelNum).Polygon(A).PointCount = 4 Then
                    Quads(QuadCount).PosA = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(0))
                    Quads(QuadCount).PosB = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(1))
                    Quads(QuadCount).PosC = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(2))
                    Quads(QuadCount).PosD = Level(LevelNum).Point(Level(LevelNum).Polygon(A).PointNum(3))
                    Quads(QuadCount).TexCoordA.X = Level(LevelNum).Polygon(A).TexCoord(0).X / 255.0#
                    Quads(QuadCount).TexCoordA.Y = Level(LevelNum).Polygon(A).TexCoord(0).Y / 255.0#
                    Quads(QuadCount).TexCoordB.X = Level(LevelNum).Polygon(A).TexCoord(1).X / 255.0#
                    Quads(QuadCount).TexCoordB.Y = Level(LevelNum).Polygon(A).TexCoord(1).Y / 255.0#
                    Quads(QuadCount).TexCoordC.X = Level(LevelNum).Polygon(A).TexCoord(2).X / 255.0#
                    Quads(QuadCount).TexCoordC.Y = Level(LevelNum).Polygon(A).TexCoord(2).Y / 255.0#
                    Quads(QuadCount).TexCoordD.X = Level(LevelNum).Polygon(A).TexCoord(3).X / 255.0#
                    Quads(QuadCount).TexCoordD.Y = Level(LevelNum).Polygon(A).TexCoord(3).Y / 255.0#
                    QuadCount += 1
                End If
            Next
        Next

        LoadPIE.Success = True
    End Function
End Class