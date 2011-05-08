Public Class clsResult

    Public Warning(-1) As String
    Public WarningCount As Integer
    Public Problem(-1) As String
    Public ProblemCount As Integer

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return (WarningCount > 0 Or ProblemCount > 0)
        End Get
    End Property

    Public ReadOnly Property HasProblems As Boolean
        Get
            Return (ProblemCount > 0)
        End Get
    End Property

    Public Sub Append(ByVal Result_To_Add As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To Result_To_Add.WarningCount - 1
            Warning_Add(Prefix & Result_To_Add.Warning(A))
        Next
        For A = 0 To Result_To_Add.ProblemCount - 1
            Problem_Add(Prefix & Result_To_Add.Problem(A))
        Next
    End Sub

    Public Sub AppendAsWarning(ByRef Result_To_Add As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To Result_To_Add.WarningCount - 1
            Warning_Add(Prefix & Result_To_Add.Warning(A))
        Next
        For A = 0 To Result_To_Add.ProblemCount - 1
            Warning_Add(Prefix & Result_To_Add.Problem(A))
        Next
    End Sub

    Public Sub Warning_Add(ByRef Text As String)

        ReDim Preserve Warning(WarningCount)
        Warning(WarningCount) = Text
        WarningCount += 1
    End Sub

    Public Sub Problem_Add(ByRef Text As String)

        ReDim Preserve Problem(ProblemCount)
        Problem(ProblemCount) = Text
        ProblemCount += 1
    End Sub
End Class