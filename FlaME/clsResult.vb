Public Class clsResult

    Public Warnings As New SimpleClassList(Of String)
    Public Problems As New SimpleClassList(Of String)

    Public Sub New()

        'Warnings.MaintainOrder = True
        'Problems.MaintainOrder = True
    End Sub

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return (Warnings.ItemCount > 0 Or Problems.ItemCount > 0)
        End Get
    End Property

    Public ReadOnly Property HasProblems As Boolean
        Get
            Return (Problems.ItemCount > 0)
        End Get
    End Property

    Public Sub Append(ByVal ResultToAdd As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To ResultToAdd.Problems.ItemCount - 1
            Problems.Add(Prefix & ResultToAdd.Problems.Item(A))
        Next
        For A = 0 To ResultToAdd.Warnings.ItemCount - 1
            Warnings.Add(Prefix & ResultToAdd.Warnings.Item(A))
        Next
    End Sub

    Public Sub AppendAsWarning(ByVal ResultToAdd As clsResult, ByVal Prefix As String)
        Dim A As Integer

        For A = 0 To ResultToAdd.Problems.ItemCount - 1
            Warnings.Add(Prefix & ResultToAdd.Problems.Item(A))
        Next
        For A = 0 To ResultToAdd.Warnings.ItemCount - 1
            Warnings.Add(Prefix & ResultToAdd.Warnings.Item(A))
        Next
    End Sub

    Public Sub Problem_Add(ByVal Text As String)

        Problems.Add(Text)
    End Sub

    Public Sub Warning_Add(ByVal Text As String)

        Warnings.Add(Text)
    End Sub
End Class