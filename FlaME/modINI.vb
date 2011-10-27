Public Module modINI

    Public Class clsINIRead
        Public Class clsSection
            Public Name As String
            Public Structure sProperty
                Public Name As String
                Public Value As String
            End Structure
            Public Properties(0) As sProperty
            Public PropertyCount As Integer

            Public Sub CreateProperty(ByVal Name As String, ByVal Value As String)

                If Properties.GetUpperBound(0) < PropertyCount Then
                    ReDim Preserve Properties(PropertyCount * 2 + 1)
                End If
                Properties(PropertyCount).Name = Name
                Properties(PropertyCount).Value = Value
                PropertyCount += 1
            End Sub

            Public Function ReadFile(ByVal File As System.IO.StreamReader) As clsResult
                Dim ReturnResult As New clsResult

                Dim InvalidLineCount As Integer = 0
                Dim CurrentEntryNum As Integer = -1
                Dim LineText As String = Nothing
                Dim A As Integer
                Dim TrimChars(2) As Char
                TrimChars(0) = Chr(10)
                TrimChars(1) = Chr(13)
                TrimChars(2) = " "c

                Do
                    LineText = File.ReadLine
                    If LineText Is Nothing Then
                        Exit Do
                    End If
                    LineText.Trim(TrimChars)
                    If LineText.Length >= 1 Then
                        A = LineText.IndexOf("="c)
                        If A >= 0 Then
                            CreateProperty(LineText.Substring(0, A).ToLower.Trim, LineText.Substring(A + 1, LineText.Length - A - 1).Trim)
                        Else
                            InvalidLineCount += 1
                        End If
                    ElseIf LineText.Length > 0 Then
                        InvalidLineCount += 1
                    End If
                Loop

                ReDim Preserve Properties(PropertyCount - 1)

                If InvalidLineCount > 0 Then
                    ReturnResult.Warning_Add("There were " & InvalidLineCount & " invalid lines that were ignored.")
                End If

                Return ReturnResult
            End Function

            Public Function Translate(ByVal SectionNum As Integer, ByVal Translator As clsINIRead.clsSectionTranslator, ByRef ErrorCount As sErrorCount) As clsResult
                Dim ReturnResult As New clsResult

                Dim A As Integer
                Dim TranslatorResult As enumTranslatorResult

                For A = 0 To PropertyCount - 1
                    TranslatorResult = Translator.Translate(SectionNum, Properties(A))
                    Select Case TranslatorResult
                        Case enumTranslatorResult.NameUnknown
                            If ErrorCount.NameErrorCount < 16 Then
                                ReturnResult.Warning_Add("Property name " & ControlChars.Quote & Properties(A).Name & ControlChars.Quote & " is unknown.")
                            End If
                            ErrorCount.NameErrorCount += 1
                        Case enumTranslatorResult.ValueInvalid
                            If ErrorCount.ValueErrorCount < 16 Then
                                ReturnResult.Warning_Add("Value " & ControlChars.Quote & Properties(A).Value & ControlChars.Quote & " for property name " & ControlChars.Quote & Properties(A).Name & ControlChars.Quote & " is not valid.")
                            End If
                            ErrorCount.ValueErrorCount += 1
                    End Select
                Next

                Return ReturnResult
            End Function

            Public Function Translate(ByVal Translator As clsINIRead.clsTranslator) As clsResult
                Dim ReturnResult As New clsResult

                Dim A As Integer
                Dim TranslatorResult As enumTranslatorResult
                Dim ErrorCount As sErrorCount

                ErrorCount.NameWarningCountMax = 16
                ErrorCount.ValueWarningCountMax = 16

                For A = 0 To PropertyCount - 1
                    TranslatorResult = Translator.Translate(Properties(A))
                    Select Case TranslatorResult
                        Case enumTranslatorResult.NameUnknown
                            If ErrorCount.NameErrorCount < 16 Then
                                ReturnResult.Warning_Add("Property name " & ControlChars.Quote & Properties(A).Name & ControlChars.Quote & " is unknown.")
                            End If
                            ErrorCount.NameErrorCount += 1
                        Case enumTranslatorResult.ValueInvalid
                            If ErrorCount.ValueErrorCount < 16 Then
                                ReturnResult.Warning_Add("Value " & ControlChars.Quote & Properties(A).Value & ControlChars.Quote & " for property name " & ControlChars.Quote & Properties(A).Name & ControlChars.Quote & " is not valid.")
                            End If
                            ErrorCount.ValueErrorCount += 1
                    End Select
                Next

                If ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax Then
                    ReturnResult.Warning_Add("There were " & ErrorCount.NameErrorCount & " unknown property names that were ignored.")
                End If
                If ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax Then
                    ReturnResult.Warning_Add("There were " & ErrorCount.ValueErrorCount & " invalid values that were ignored.")
                End If

                Return ReturnResult
            End Function
        End Class
        Public Sections(0) As clsSection
        Public SectionCount As Integer

        Public Sub CreateSection(ByVal Name As String)

            If Sections.GetUpperBound(0) < SectionCount Then
                ReDim Preserve Sections(SectionCount * 2 + 1)
            End If
            Sections(SectionCount) = New clsSection
            Sections(SectionCount).Name = Name
            SectionCount += 1
        End Sub

        Public Function ReadFile(ByVal File As IO.StreamReader) As clsResult
            Dim ReturnResult As New clsResult

            Dim InvalidLineCount As Integer = 0
            Dim CurrentEntryNum As Integer = -1
            Dim LineText As String = Nothing
            Dim A As Integer
            Dim TrimChars(2) As Char
            Dim SectionName As String
            TrimChars(0) = Chr(10)
            TrimChars(1) = Chr(13)
            TrimChars(2) = " "c

            Do
                LineText = File.ReadLine
                If LineText Is Nothing Then
                    Exit Do
                End If
                LineText.Trim(TrimChars)
                If LineText.Length >= 2 Then
                    If LineText.Chars(0) = "["c Then
                        If LineText.Chars(LineText.Length - 1) = "]"c Then
                            SectionName = LineText.Substring(1, LineText.Length - 2)
                            For A = 0 To SectionCount - 1
                                If Sections(A).Name = SectionName Then
                                    Exit For
                                End If
                            Next
                            CurrentEntryNum = A
                            If CurrentEntryNum = SectionCount Then
                                CreateSection(SectionName)
                            End If
                        Else
                            InvalidLineCount += 1
                        End If
                    ElseIf CurrentEntryNum >= 0 Then
                        A = LineText.IndexOf("="c)
                        If A >= 0 Then
                            Sections(CurrentEntryNum).CreateProperty(LineText.Substring(0, A).ToLower.Trim, LineText.Substring(A + 1, LineText.Length - A - 1).Trim)
                        Else
                            InvalidLineCount += 1
                        End If
                    Else
                        InvalidLineCount += 1
                    End If
                ElseIf LineText.Length > 0 Then
                    InvalidLineCount += 1
                End If
            Loop

            ReDim Preserve Sections(SectionCount - 1)
            For A = 0 To SectionCount - 1
                ReDim Preserve Sections(A).Properties(Sections(A).PropertyCount - 1)
            Next

            If InvalidLineCount > 0 Then
                ReturnResult.Warning_Add("There were " & InvalidLineCount & " invalid lines that were ignored.")
            End If

            Return ReturnResult
        End Function

        Public Enum enumTranslatorResult As Byte
            NameUnknown
            ValueInvalid
            Translated
        End Enum

        Public Structure sErrorCount
            Public NameErrorCount As Integer
            Public ValueErrorCount As Integer
            Public NameWarningCountMax As Integer
            Public ValueWarningCountMax As Integer
        End Structure

        Public Function Translate(ByVal Translator As clsINIRead.clsSectionTranslator) As clsResult
            Dim ReturnResult As New clsResult

            Dim A As Integer
            Dim ErrorCount As sErrorCount

            ErrorCount.NameWarningCountMax = 16
            ErrorCount.ValueWarningCountMax = 16

            For A = 0 To SectionCount - 1
                ReturnResult.Append(Sections(A).Translate(A, Translator, ErrorCount), "")
            Next

            If ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax Then
                ReturnResult.Warning_Add("There were " & ErrorCount.NameErrorCount & " unknown property names that were ignored.")
            End If
            If ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax Then
                ReturnResult.Warning_Add("There were " & ErrorCount.ValueErrorCount & " invalid values that were ignored.")
            End If

            Return ReturnResult
        End Function

        Public MustInherit Class clsSectionTranslator

            Public MustOverride Function Translate(ByVal INISectionNum As Integer, ByVal INIProperty As clsINIRead.clsSection.sProperty) As enumTranslatorResult
        End Class

        Public MustInherit Class clsTranslator

            Public MustOverride Function Translate(ByVal INIProperty As clsINIRead.clsSection.sProperty) As enumTranslatorResult
        End Class
    End Class

    Public Class clsINIWrite

        Public File As IO.StreamWriter
        Public LineEndChar As Char = Chr(10)
        Public EqualsChar As Char = "="c

        Public Sub SectionName_Append(ByVal Name As String)

            Name = Name.Replace(LineEndChar, "")

            File.Write("["c & Name & "]"c & LineEndChar)
        End Sub

        Public Sub Property_Append(ByVal Name As String, ByVal Value As String)

            Name = Name.Replace(LineEndChar, "")
            Name = Name.Replace(EqualsChar, "")
            Value = Value.Replace(LineEndChar, "")

            File.Write(Name & " " & EqualsChar & " " & Value & LineEndChar)
        End Sub

        Public Sub Gap_Append()

            File.Write(LineEndChar)
        End Sub
    End Class

    Public Function CreateINIWriteFile(ByVal Output As IO.Stream) As clsINIWrite
        Dim NewINI As New clsINIWrite
        Dim Encoding As New System.Text.UTF8Encoding(False, False)

        NewINI.File = New IO.StreamWriter(Output, Encoding)

        Return NewINI
    End Function
End Module