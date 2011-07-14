Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsModel

    Public GLTextureNum As Integer

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

        GL.BindTexture(TextureTarget.Texture2D, GLTextureNum)

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
            Dim TexCoord() As sXY_sng
            Dim PointCount As Integer
        End Structure
        Dim Polygon() As sPolygon
        Dim PolygonCount As Integer
        Dim Point() As sXYZ_sng
        Dim PointCount As Integer
    End Structure

    Function LoadPIE(ByVal File As clsReadFile) As clsResult
        Dim ReturnResult As New clsResult

        Dim TerminatorChars(1) As Char
        TerminatorChars(0) = Chr(10)
        TerminatorChars(1) = Chr(13)

        Dim A As Integer
        Dim B As Integer
        Dim strTemp As String
        Dim SplitText() As String
        Dim LevelCount As Integer
        Dim NewQuadCount As Integer
        Dim NewTriCount As Integer
        Dim C As Integer
        Dim TextureName As String = ""
        Dim Levels() As sPIELevel
        Dim LevelNum As Integer
        Dim GotText As Boolean
        Dim strTemp2 As String
        Dim D As Integer
        Dim PIEVersion As Integer
        Dim Count As Integer

        ReDim Levels(-1)
        LevelNum = -1
        Do
            strTemp = Nothing
            If Not File.Get_Text_Terminated(TerminatorChars, False, True, strTemp) Then
                GoTo FileFinished
            End If
Reeval:
            If Left(strTemp, 3) = "PIE" Then
                PIEVersion = Val(Right(strTemp, strTemp.Length - 4))
                If PIEVersion <> 2 And PIEVersion <> 3 Then
                    ReturnResult.Problem_Add("Version is unknown.")
                    Return ReturnResult
                End If
            ElseIf Left(strTemp, 4) = "TYPE" Then
            ElseIf Left(strTemp, 7) = "TEXTURE" Then
                TextureName = Right(strTemp, strTemp.Length - 10)
                A = InStrRev(TextureName, " ")
                If A > 0 Then
                    A = InStrRev(TextureName, " ", A - 1)
                Else
                    ReturnResult.Problem_Add("Bad texture name.")
                    Return ReturnResult
                End If
                If A > 0 Then
                    TextureName = Left(TextureName, A - 1)
                Else
                    ReturnResult.Problem_Add("Bad texture name.")
                    Return ReturnResult
                End If
            ElseIf Left(strTemp, 6) = "LEVELS" Then
                LevelCount = Right(strTemp, strTemp.Length - 7)
                ReDim Levels(LevelCount - 1)
            ElseIf Left(strTemp, 6) = "LEVEL " Then
                LevelNum = Right(strTemp, strTemp.Length - 6) - 1
                If LevelNum >= LevelCount Then
                    ReturnResult.Problem_Add("Level number >= number of levels.")
                    Return ReturnResult
                End If
            ElseIf Left(strTemp, 6) = "POINTS" Then
                Levels(LevelNum).PointCount = Right(strTemp, strTemp.Length - 7)
                ReDim Levels(LevelNum).Point(Levels(LevelNum).PointCount - 1)
                A = 0
                Do
                    strTemp = Nothing
                    If Not File.Get_Text_Terminated(TerminatorChars, False, True, strTemp) Then
                        GoTo FileFinished
                    End If

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        ReDim SplitText(2)
                        C = 0
                        SplitText(0) = ""
                        GotText = False
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) <> " "c And strTemp.Chars(B) <> ControlChars.Tab Then
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

                        Try
                            Levels(LevelNum).Point(A).X = SplitText(0)
                            Levels(LevelNum).Point(A).Y = SplitText(1)
                            Levels(LevelNum).Point(A).Z = SplitText(2)
                        Catch ex As Exception
                            ReturnResult.Problem_Add("Bad point " & A)
                            Return ReturnResult
                        End Try
                        A += 1
                    ElseIf strTemp2 = "" Then

                    Else
                        GoTo Reeval
                    End If
                Loop
            ElseIf Left(strTemp, 8) = "POLYGONS" Then
                Levels(LevelNum).PolygonCount = Right(strTemp, strTemp.Length - 9)
                ReDim Levels(LevelNum).Polygon(Levels(LevelNum).PolygonCount - 1)
                A = 0
                Do
                    strTemp = Nothing
                    If Not File.Get_Text_Terminated(TerminatorChars, False, True, strTemp) Then
                        GoTo FileFinished
                    End If

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        C = 0
                        ReDim SplitText(C)
                        SplitText(C) = ""
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) = " "c Or strTemp.Chars(B) = ControlChars.Tab Then
                                If SplitText(C).Length > 0 Then
                                    C += 1
                                    ReDim Preserve SplitText(C)
                                    SplitText(C) = ""
                                End If
                            Else
                                SplitText(C) &= strTemp.Chars(B)
                            End If
                        Next
                        If SplitText(C).Length = 0 Then
                            ReDim Preserve SplitText(C - 1)
                        Else
                            C += 1
                        End If

                        If PIEVersion = 3 Then
                            '200, pointcount, points, texcoords
                            If C < 2 Then
                                ReturnResult.Problem_Add("Too few fields for polygon " & A)
                                Return ReturnResult
                            End If
                            Try
                                Count = SplitText(1)
                            Catch ex As Exception
                                ReturnResult.Problem_Add("Bad polygon point count: " & ex.Message)
                                Return ReturnResult
                            End Try
                            Levels(LevelNum).Polygon(A).PointCount = Count
                            ReDim Levels(LevelNum).Polygon(A).PointNum(Count - 1)
                            ReDim Levels(LevelNum).Polygon(A).TexCoord(Count - 1)
                            If Count = 3 Then
                                NewTriCount += 1
                            ElseIf Count = 4 Then
                                NewQuadCount += 1
                            End If
                            Select Case SplitText.GetUpperBound(0) + 1
                                Case 0
                                    GoTo Reeval
                                Case Is <> 2 + Count * 3
                                    ReturnResult.Problem_Add("Wrong number of fields (" & SplitText.GetUpperBound(0) + 1 & ") for polygon " & A)
                                    Return ReturnResult
                            End Select
                            For B = 0 To Count - 1
                                Try
                                    Levels(LevelNum).Polygon(A).PointNum(B) = SplitText(2 + B)
                                Catch ex As Exception
                                    ReturnResult.Problem_Add("Bad polygon point: " & ex.Message)
                                    Return ReturnResult
                                End Try

                                Try
                                    Levels(LevelNum).Polygon(A).TexCoord(B).X = SplitText(2 + Count + 2 * B)
                                Catch ex As Exception
                                    ReturnResult.Problem_Add("Bad polygon x tex coord: " & ex.Message)
                                    Return ReturnResult
                                End Try
                                Try
                                    Levels(LevelNum).Polygon(A).TexCoord(B).Y = SplitText(2 + Count + 2 * B + 1)
                                Catch ex As Exception
                                    ReturnResult.Problem_Add("Bad polygon y tex coord: " & ex.Message)
                                    Return ReturnResult
                                End Try
                            Next
                            A += 1
                        ElseIf PIEVersion = 2 Then
                            D = 0
                            Do
                                'flag, numpoints, points[], x4 ignore if animated, texcoord[]xy
                                Levels(LevelNum).Polygon(A).PointCount = Val(SplitText(D + 1))
                                ReDim Levels(LevelNum).Polygon(A).PointNum(Levels(LevelNum).Polygon(A).PointCount - 1)
                                ReDim Levels(LevelNum).Polygon(A).TexCoord(Levels(LevelNum).Polygon(A).PointCount - 1)
                                If Levels(LevelNum).Polygon(A).PointCount = 3 Then
                                    NewTriCount += 1
                                ElseIf Levels(LevelNum).Polygon(A).PointCount = 4 Then
                                    NewQuadCount += 1
                                End If
                                For B = 0 To Levels(LevelNum).Polygon(A).PointCount - 1
                                    Levels(LevelNum).Polygon(A).PointNum(B) = Val(SplitText(D + 2 + B))
                                Next
                                C = D + 2 + Levels(LevelNum).Polygon(A).PointCount
                                If SplitText(D) = "4200" Or SplitText(D) = "4000" Or SplitText(D) = "6a00" Or SplitText(D) = "4a00" Or SplitText(D) = "6200" Or SplitText(D) = "14200" Or SplitText(D) = "14a00" Or SplitText(D) = "16a00" Then
                                    C += 4
                                End If
                                For B = 0 To Levels(LevelNum).Polygon(A).PointCount - 1
                                    Levels(LevelNum).Polygon(A).TexCoord(B).X = Val(SplitText(C))
                                    Levels(LevelNum).Polygon(A).TexCoord(B).Y = Val(SplitText(C + 1))
                                    C += 2
                                Next
                                D = C
                                A += 1
                            Loop While D < SplitText.GetUpperBound(0)
                        End If
                    ElseIf strTemp2 = "" Then

                    Else
                        GoTo Reeval
                    End If
                Loop
            ElseIf Left(strTemp, 10) = "CONNECTORS" Then
                ConnectorCount = Right(strTemp, strTemp.Length - 11)
                ReDim Connectors(ConnectorCount - 1)
                A = 0
                Do
                    strTemp = Nothing
                    If Not File.Get_Text_Terminated(TerminatorChars, False, True, strTemp) Then
                        GoTo FileFinished
                    End If

                    strTemp2 = Strings.Left(strTemp, 1)
                    If strTemp2 = Chr(9) Or strTemp2 = " " Then

                        ReDim SplitText(2)
                        C = 0
                        SplitText(0) = ""
                        GotText = False
                        For B = 0 To strTemp.Length - 1
                            If strTemp.Chars(B) <> " "c And strTemp.Chars(B) <> ControlChars.Tab Then
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

                        Try
                            Connectors(A).X = SplitText(0)
                            Connectors(A).Y = SplitText(2)
                            Connectors(A).Z = SplitText(1)
                        Catch ex As Exception
                            ReturnResult.Problem_Add("Bad connector " & A)
                            Return ReturnResult
                        End Try
                        A += 1
                    ElseIf strTemp2 = "" Then

                    Else
                        GoTo Reeval
                    End If
                Loop
            Else

            End If
        Loop
FileFinished:

        GLTextureNum = Get_TexturePage_GLTexture(Left(TextureName, TextureName.Length - 4))
        If GLTextureNum = 0 Then
            ReturnResult.Warning_Add("PIE texture " & ControlChars.Quote & TextureName & ControlChars.Quote & " was not loaded.")
        End If

        TriangleCount = NewTriCount
        QuadCount = NewQuadCount
        ReDim Triangles(TriangleCount - 1)
        ReDim Quads(QuadCount - 1)
        NewTriCount = 0
        NewQuadCount = 0
        For LevelNum = 0 To LevelCount - 1
            For A = 0 To Levels(LevelNum).PolygonCount - 1
                If Levels(LevelNum).Polygon(A).PointCount = 3 Then
                    Triangles(NewTriCount).PosA = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(0))
                    Triangles(NewTriCount).PosB = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(1))
                    Triangles(NewTriCount).PosC = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(2))
                    If PIEVersion = 2 Then
                        Triangles(NewTriCount).TexCoordA.X = Levels(LevelNum).Polygon(A).TexCoord(0).X / 255.0#
                        Triangles(NewTriCount).TexCoordA.Y = Levels(LevelNum).Polygon(A).TexCoord(0).Y / 255.0#
                        Triangles(NewTriCount).TexCoordB.X = Levels(LevelNum).Polygon(A).TexCoord(1).X / 255.0#
                        Triangles(NewTriCount).TexCoordB.Y = Levels(LevelNum).Polygon(A).TexCoord(1).Y / 255.0#
                        Triangles(NewTriCount).TexCoordC.X = Levels(LevelNum).Polygon(A).TexCoord(2).X / 255.0#
                        Triangles(NewTriCount).TexCoordC.Y = Levels(LevelNum).Polygon(A).TexCoord(2).Y / 255.0#
                    ElseIf PIEVersion = 3 Then
                        Triangles(NewTriCount).TexCoordA = Levels(LevelNum).Polygon(A).TexCoord(0)
                        Triangles(NewTriCount).TexCoordB = Levels(LevelNum).Polygon(A).TexCoord(1)
                        Triangles(NewTriCount).TexCoordC = Levels(LevelNum).Polygon(A).TexCoord(2)
                    End If
                    NewTriCount += 1
                ElseIf Levels(LevelNum).Polygon(A).PointCount = 4 Then
                    Quads(NewQuadCount).PosA = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(0))
                    Quads(NewQuadCount).PosB = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(1))
                    Quads(NewQuadCount).PosC = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(2))
                    Quads(NewQuadCount).PosD = Levels(LevelNum).Point(Levels(LevelNum).Polygon(A).PointNum(3))
                    If PIEVersion = 2 Then
                        Quads(NewQuadCount).TexCoordA.X = Levels(LevelNum).Polygon(A).TexCoord(0).X / 255.0#
                        Quads(NewQuadCount).TexCoordA.Y = Levels(LevelNum).Polygon(A).TexCoord(0).Y / 255.0#
                        Quads(NewQuadCount).TexCoordB.X = Levels(LevelNum).Polygon(A).TexCoord(1).X / 255.0#
                        Quads(NewQuadCount).TexCoordB.Y = Levels(LevelNum).Polygon(A).TexCoord(1).Y / 255.0#
                        Quads(NewQuadCount).TexCoordC.X = Levels(LevelNum).Polygon(A).TexCoord(2).X / 255.0#
                        Quads(NewQuadCount).TexCoordC.Y = Levels(LevelNum).Polygon(A).TexCoord(2).Y / 255.0#
                        Quads(NewQuadCount).TexCoordD.X = Levels(LevelNum).Polygon(A).TexCoord(3).X / 255.0#
                        Quads(NewQuadCount).TexCoordD.Y = Levels(LevelNum).Polygon(A).TexCoord(3).Y / 255.0#
                    ElseIf PIEVersion = 3 Then
                        Quads(NewQuadCount).TexCoordA = Levels(LevelNum).Polygon(A).TexCoord(0)
                        Quads(NewQuadCount).TexCoordB = Levels(LevelNum).Polygon(A).TexCoord(1)
                        Quads(NewQuadCount).TexCoordC = Levels(LevelNum).Polygon(A).TexCoord(2)
                        Quads(NewQuadCount).TexCoordD = Levels(LevelNum).Polygon(A).TexCoord(3)
                    End If
                    NewQuadCount += 1
                End If
            Next
        Next

        Return ReturnResult
    End Function
End Class