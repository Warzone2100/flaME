
Public Class clsResult

    Private _Text As String

    Public ReadOnly Property Text As String
        Get
            Return _Text
        End Get
    End Property

    Public Class clsProblem
        Public Text As String
    End Class
    Public Class clsWarning
        Public Text As String
    End Class

    Private Items As New SimpleList(Of Object)
    Private Bad As Boolean = False

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return (Items.Count > 0)
        End Get
    End Property

    Public ReadOnly Property HasProblems As Boolean
        Get
            Return Bad
        End Get
    End Property

    Public Sub AddBypass(ResultToAdd As clsResult)

        If ResultToAdd.HasWarnings Then
            Items.Add(ResultToAdd)
        End If
    End Sub

    Public Sub Add(ResultToAdd As clsResult)

        If ResultToAdd.HasProblems Then
            Bad = True
        End If
        If ResultToAdd.HasWarnings Then
            Items.Add(ResultToAdd)
        End If
    End Sub

    Public Sub Take(ResultToMerge As clsResult)

        If ResultToMerge.HasProblems Then
            Bad = True
        End If
        Items.AddSimpleList(ResultToMerge.Items)
    End Sub

    Public Sub ProblemAdd(Text As String)

        Bad = True
        Dim Problem As New clsProblem
        Problem.Text = Text
        Items.Add(Problem)
    End Sub

    Public Sub WarningAdd(Text As String)

        Dim Warning As New clsWarning
        Warning.Text = Text
        Items.Add(Warning)
    End Sub

    Public Sub New(Text As String)

        Items.MaintainOrder = True

        Me._Text = Text
    End Sub

    Public Function MakeNodes(Owner As TreeNodeCollection) As TreeNode
        Dim Node As New TreeNode
        Node.Text = _Text
        Owner.Add(Node)
        Dim A As Integer
        Dim Item As Object
        For A = 0 To Items.Count - 1
            Item = Items(A)
            Dim ChildNode As New TreeNode
            If TypeOf Item Is clsProblem Then
                ChildNode.Text = CType(Item, clsProblem).Text
                Node.Nodes.Add(ChildNode)
                ChildNode.StateImageKey = "problem"
            ElseIf TypeOf Item Is clsWarning Then
                ChildNode.Text = CType(Item, clsWarning).Text
                Node.Nodes.Add(ChildNode)
                ChildNode.StateImageKey = "warning"
            ElseIf TypeOf Item Is clsResult Then
                ChildNode = CType(Item, clsResult).MakeNodes(Node.Nodes)
            End If
        Next
        Return Node
    End Function
End Class
