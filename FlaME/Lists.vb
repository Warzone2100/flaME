Public Class SimpleList(Of ItemType)

    Private _Items(0) As ItemType
    Private _ItemCount As Integer = 0
    Private _IsBusy As Boolean = False

    Public MaintainOrder As Boolean = False

    Protected _tmpItem As ItemType

    Public ReadOnly Property IsBusy As Boolean
        Get
            Return _IsBusy
        End Get
    End Property

    Public ReadOnly Property ItemCount As Integer
        Get
            Return _ItemCount
        End Get
    End Property

    Public ReadOnly Property Item(ByVal Number As Integer) As ItemType
        Get
            Return _Items(Number)
        End Get
    End Property

    Public Sub Add(ByVal NewItem As ItemType)

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        Dim Position As Integer = _ItemCount

        _tmpitem = NewItem
        If Not _BeforeAdd(Position) Then
            _tmpItem = Nothing
            _IsBusy = False
            Exit Sub
        End If
        _tmpItem = Nothing

        If UBound(_Items) < _ItemCount Then
            ReDim Preserve _Items(_ItemCount * 2 + 1)
        End If
        _Items(Position) = NewItem
        _ItemCount += 1

        _IsBusy = False
    End Sub

    Public Sub Insert(ByVal NewItem As ItemType, ByVal Position As Integer)

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        If Position < 0 Or Position > _ItemCount Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        _tmpItem = NewItem
        If Not _BeforeAdd(Position) Then
            _tmpItem = Nothing
            _IsBusy = False
            Exit Sub
        End If
        _tmpItem = Nothing

        If UBound(_Items) < _ItemCount Then
            ReDim Preserve _Items(_ItemCount * 2 + 1)
        End If
        If MaintainOrder Then
            Dim A As Integer
            Dim NewPos As Integer
            For A = _ItemCount - 1 To Position Step -1
                NewPos = A + 1
                _Items(NewPos) = _Items(A)
                _AfterMove(NewPos)
            Next
        Else
            _Items(_ItemCount) = _Items(Position)
            _AfterMove(_ItemCount)
        End If
        _Items(Position) = NewItem
        _ItemCount += 1

        _IsBusy = False
    End Sub

    Protected Overridable Function _BeforeAdd(ByVal Position As Integer) As Boolean

        Return True
    End Function

    Public Sub Remove(ByVal Position As Integer)

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        If Position < 0 Or Position >= _ItemCount Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        _BeforeRemove(Position)

        _ItemCount -= 1
        If MaintainOrder Then
            Dim A As Integer
            Dim NewPos As Integer
            For A = Position + 1 To _ItemCount
                NewPos = A - 1
                _Items(NewPos) = _Items(A)
                _AfterMove(NewPos)
            Next
        Else
            If Position < _ItemCount Then
                Dim LastItem As ItemType = _Items(_ItemCount)
                _Items(Position) = LastItem
                _AfterMove(Position)
            End If
        End If
        _Items(_ItemCount) = Nothing
        If _ItemCount * 3 < UBound(_Items) + 1 Then
            ReDim Preserve _Items(_ItemCount * 2 - 1)
        End If

        _IsBusy = False

        _AfterRemove()
    End Sub

    Protected Overridable Sub _BeforeRemove(ByVal Position As Integer)


    End Sub

    Protected Overridable Sub _AfterRemove()


    End Sub

    Protected Overridable Sub _AfterMove(ByVal Position As Integer)


    End Sub

    Public Sub Clear()

        Do While _ItemCount > 0
            Remove(_ItemCount - 1)
        Loop
    End Sub

    Public Sub Deallocate()

        _Deallocate()

        _IsBusy = True
    End Sub

    Protected Overridable Sub _Deallocate()

        Clear()
        Erase _Items
    End Sub

    Public Sub PerformTool(ByVal Tool As SimpleListTool(Of ItemType))

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        Dim A As Integer

        For A = 0 To _ItemCount - 1
            Tool.SetItem(_Items(A))
            Tool.ActionPerform()
        Next

        _IsBusy = False
    End Sub

    Public Sub SendItems(ByVal OtherList As SimpleList(Of ItemType))
        Dim A As Integer

        For A = 0 To _ItemCount - 1
            OtherList.Add(_Items(A))
        Next
    End Sub
End Class

Public Enum SimpleClassList_AddNothingAction As Byte
    Allow
    DisallowIgnore
    DisallowError
End Enum

Public Class SimpleClassList(Of ItemType As Class)
    Inherits SimpleList(Of ItemType)

    Public AddNothingAction As SimpleClassList_AddNothingAction = SimpleClassList_AddNothingAction.Allow

    Protected Overrides Function _BeforeAdd(Position As Integer) As Boolean

        Select Case AddNothingAction
            Case SimpleClassList_AddNothingAction.Allow
                Return True
            Case SimpleClassList_AddNothingAction.DisallowIgnore
                Return (_tmpitem IsNot Nothing)
            Case SimpleClassList_AddNothingAction.DisallowError
                If _tmpitem Is Nothing Then
                    Stop
                    Return False
                Else
                    Return True
                End If
            Case Else
                Stop
                Return False
        End Select
    End Function

    Public Function FindFirstItemPosition(ByVal ItemToFind As ItemType) As Integer
        Dim Position As Integer

        For Position = 0 To ItemCount - 1
            If Item(Position) Is ItemToFind Then
                Return Position
            End If
        Next
        Return -1
    End Function
End Class

Public Interface SimpleListTool(Of ItemType)

    Sub SetItem(ByVal Item As ItemType)
    Sub ActionPerform()
End Interface

Public Class ConnectedList(Of ItemType As Class, SourceType As Class)
    Inherits SimpleClassList(Of ConnectedListItem(Of ItemType, SourceType))

    Private _Owner As SourceType

    Public Sub New(ByVal Owner As SourceType)

        _Owner = Owner
    End Sub

    Public ReadOnly Property Owner As SourceType
        Get
            Return _Owner
        End Get
    End Property

    Public ReadOnly Property ItemContainer(ByVal Position As Integer) As ConnectedListItem(Of ItemType, SourceType)
        Get
            Return MyBase.Item(Position)
        End Get
    End Property

    Public Shadows ReadOnly Property Item(ByVal Position As Integer) As ItemType
        Get
            Return MyBase.Item(Position).Item
        End Get
    End Property

    Protected Overrides Function _BeforeAdd(ByVal Position As Integer) As Boolean

        Return _tmpItem.BeforeAdd(Position)
    End Function

    Protected Overrides Sub _BeforeRemove(Position As Integer)

        _tmpItem = ItemContainer(Position)
        _tmpItem.BeforeRemove()
    End Sub

    Protected Overrides Sub _AfterRemove()

        _tmpItem.AfterRemove()
        _tmpItem = Nothing
    End Sub

    Protected Overrides Sub _AfterMove(Position As Integer)
        MyBase._AfterMove(Position)

        ItemContainer(Position).AfterMove(Position)
    End Sub

    Public Function FindLink(ByVal ItemToFind As ItemType) As ConnectedListItem(Of ItemType, SourceType)
        Dim A As Integer
        Dim Link As ConnectedListItem(Of ItemType, SourceType)

        For A = 0 To ItemCount - 1
            Link = ItemContainer(A)
            If Link.Item Is ItemToFind Then
                Return Link
            End If
        Next
        Return Nothing
    End Function

    Protected Overrides Sub _Deallocate()
        MyBase._Deallocate()

        _Owner = Nothing
    End Sub

    Public Function GetItemsAsSimpleClassList() As SimpleClassList(Of ItemType)
        Dim A As Integer
        Dim Result As New SimpleClassList(Of ItemType)

        For A = 0 To ItemCount - 1
            Result.Add(Item(A))
        Next
        Return Result
    End Function
End Class

Public Interface ConnectedListItem(Of ItemType As Class, SourceType As Class)

    ReadOnly Property Item As ItemType
    ReadOnly Property Source As SourceType
    Function BeforeAdd(ByVal NewPosition As Integer) As Boolean
    Sub BeforeRemove()
    Sub AfterRemove()
    Sub AfterMove(ByVal NewPosition As Integer)
End Interface

Public Class ConnectedListLink(Of ItemType As Class, SourceType As Class)
    Implements ConnectedListItem(Of ItemType, SourceType)

    Private _Item As ItemType
    Private _ConnectedList As ConnectedList(Of ItemType, SourceType)
    Private _ArrayPosition As Integer = -1

    Public Sub New(ByVal Owner As ItemType)

        _Item = Owner
    End Sub

    Public ReadOnly Property ParentList As ConnectedList(Of ItemType, SourceType)
        Get
            Return _ConnectedList
        End Get
    End Property

    Public ReadOnly Property ArrayPosition As Integer
        Get
            Return _ArrayPosition
        End Get
    End Property

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return (_ArrayPosition >= 0)
        End Get
    End Property

    Public Sub AfterMove(ByVal NewPosition As Integer) Implements ConnectedListItem(Of ItemType, SourceType).AfterMove

        _ArrayPosition = NewPosition
    End Sub

    Public Sub BeforeRemove() Implements ConnectedListItem(Of ItemType, SourceType).BeforeRemove

        _ConnectedList = Nothing
        _ArrayPosition = -1
    End Sub

    Public ReadOnly Property Item As ItemType Implements ConnectedListItem(Of ItemType, SourceType).Item
        Get
            Return _Item
        End Get
    End Property

    Public ReadOnly Property Source As SourceType Implements ConnectedListItem(Of ItemType, SourceType).Source
        Get
            If IsConnected Then
                Return _ConnectedList.Owner
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Sub Connect(ByVal List As ConnectedList(Of ItemType, SourceType))

        If IsConnected Then
            Stop
            Exit Sub
        End If

        List.Add(Me)
        _ConnectedList = List
    End Sub

    Public Sub Disconnect()

        If _ConnectedList Is Nothing Then
            Stop
            Exit Sub
        End If

        _ConnectedList.Remove(_ArrayPosition)
    End Sub

    Public Sub Deallocate()

        If IsConnected Then
            Disconnect()
        End If
        _Item = Nothing
    End Sub

    Public Overridable Sub AfterRemove() Implements ConnectedListItem(Of ItemType, SourceType).AfterRemove


    End Sub

    Public Function BeforeAdd(NewPosition As Integer) As Boolean Implements ConnectedListItem(Of ItemType, SourceType).BeforeAdd

        If IsConnected Then
            Return False
        End If
        _ArrayPosition = NewPosition
        Return True
    End Function
End Class

#If False Then
	Public Class ConnectedListsConnection(Of SourceTypeA As Class, SourceTypeB As Class)
	
	    Protected Class Link(Of SourceType As Class)
	        Inherits ConnectedListLink(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceType)
	
	        Public Sub New(ByVal Owner As ConnectedListsConnection(Of SourceTypeA, SourceTypeB))
	            MyBase.New(Owner)
	
	        End Sub
	
	        Public Overrides Sub AfterRemove()
	            MyBase.AfterRemove()
	
	            Item.Deallocate()
	        End Sub
	    End Class
	
	    Protected _LinkA As New Link(Of SourceTypeA)(Me)
	    Protected _LinkB As New Link(Of SourceTypeB)(Me)
	
	    Public Overridable ReadOnly Property ItemA As SourceTypeA
	        Get
	            Return _LinkA.Source
	        End Get
	    End Property
	
	    Public Overridable ReadOnly Property ItemB As SourceTypeB
	        Get
	            Return _LinkB.Source
	        End Get
	    End Property
	
	    Public Shared Function Create(ByVal ListA As ConnectedList(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceTypeA), ByVal ListB As ConnectedList(Of ConnectedListsConnection(Of SourceTypeA, SourceTypeB), SourceTypeB)) As ConnectedListsConnection(Of SourceTypeA, SourceTypeB)
	
	        If ListA Is Nothing Then
	            Return Nothing
	        End If
	        If ListB Is Nothing Then
	            Return Nothing
	        End If
	        If ListA.IsBusy Then
	            Return Nothing
	        End If
	        If ListB.IsBusy Then
	            Return Nothing
	        End If
	
	        Dim Result As New ConnectedListsConnection(Of SourceTypeA, SourceTypeB)
	        Result._LinkA.Connect(ListA)
	        Result._LinkB.Connect(ListB)
	        Return Result
	    End Function
	
	    Protected Sub New()
	
	
	    End Sub
	
	    Public Sub Deallocate()
	
	        _LinkA.Deallocate()
	        _LinkB.Deallocate()
	    End Sub
	End Class
#End If