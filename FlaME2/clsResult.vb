Public Class clsResult

    Public Warnings(0) As String
    Public WarningCount As Integer
    Public Problems(0) As String
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

    Public Sub Append(ByVal ResultToAdd As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To ResultToAdd.WarningCount - 1
            Warning_Add(Prefix & ResultToAdd.Warnings(A))
        Next
        For A = 0 To ResultToAdd.ProblemCount - 1
            Problem_Add(Prefix & ResultToAdd.Problems(A))
        Next
    End Sub

    Public Sub AppendAsWarning(ByVal ResultToAdd As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To ResultToAdd.WarningCount - 1
            Warning_Add(Prefix & ResultToAdd.Warnings(A))
        Next
        For A = 0 To ResultToAdd.ProblemCount - 1
            Warning_Add(Prefix & ResultToAdd.Problems(A))
        Next
    End Sub

    Public Sub Warning_Add(ByVal Text As String)

        If Warnings.GetUpperBound(0) < WarningCount Then
            ReDim Preserve Warnings(WarningCount * 2 + 1)
        End If
        Warnings(WarningCount) = Text
        WarningCount += 1
    End Sub

    Public Sub Problem_Add(ByVal Text As String)

        If Problems.GetUpperBound(0) < ProblemCount Then
            ReDim Preserve Problems(ProblemCount * 2 + 1)
        End If
        Problems(ProblemCount) = Text
        ProblemCount += 1
    End Sub
End Class