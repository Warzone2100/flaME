
Public Class SimpleList(Of ItemType)

    Private _Items(0) As ItemType
    Private _ItemCount As Integer = 0
    Private _IsBusy As Boolean = False

    Public MaintainOrder As Boolean = False
    Public MinSize As Integer = 1

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

    Default Public Property Item(number As Integer) As ItemType
        Get
            If number < 0 Or number >= _ItemCount Then
                Stop
                Return Nothing
            End If
            Return _Items(number)
        End Get
        Set(value As ItemType)
            If number < 0 Or number >= _ItemCount Then
                Stop
                Exit Property
            End If
            _Items(number) = value
        End Set
    End Property

    Public Overridable Sub Add(ByVal NewItem As ItemType)

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        Dim Position As Integer = _ItemCount

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

        If UBound(_Items) < _ItemCount Then
            ReDim Preserve _Items(_ItemCount * 2 + 1)
        End If
        Dim LastNum As Integer = _ItemCount
        _ItemCount += 1
        If MaintainOrder Then
            Dim A As Integer
            Dim NewPos As Integer
            For A = LastNum - 1 To Position Step -1
                NewPos = A + 1
                _Items(NewPos) = _Items(A)
                AfterMoveAction(NewPos)
            Next
        Else
            _Items(LastNum) = _Items(Position)
            AfterMoveAction(LastNum)
        End If
        _Items(Position) = NewItem

        _IsBusy = False
    End Sub

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

        _ItemCount -= 1
        If MaintainOrder Then
            Dim A As Integer
            Dim NewPos As Integer
            For A = Position + 1 To _ItemCount
                NewPos = A - 1
                _Items(NewPos) = _Items(A)
                AfterMoveAction(NewPos)
            Next
        Else
            If Position < _ItemCount Then
                Dim LastItem As ItemType = _Items(_ItemCount)
                _Items(Position) = LastItem
                AfterMoveAction(Position)
            End If
        End If
        _Items(_ItemCount) = Nothing
        Dim ArraySize As Integer = UBound(_Items) + 1
        If _ItemCount * 3 < ArraySize And ArraySize > MinSize Then
            ReDim Preserve _Items(Math.Max(_ItemCount * 2, MinSize) - 1)
        End If

        _IsBusy = False
    End Sub

    Public Sub Swap(ByVal SwapPositionA As Integer, ByVal SwapPositionB As Integer)

        If _IsBusy Then
            Stop
            Exit Sub
        End If

        If SwapPositionA = SwapPositionB Then
            Stop
            Exit Sub
        End If

        If SwapPositionA < 0 Or SwapPositionA >= _ItemCount Then
            Stop
            Exit Sub
        End If
        If SwapPositionB < 0 Or SwapPositionB >= _ItemCount Then
            Stop
            Exit Sub
        End If

        _IsBusy = True

        Dim tmpItem As ItemType = _Items(SwapPositionA)
        _Items(SwapPositionA) = _Items(SwapPositionB)
        _Items(SwapPositionB) = tmpItem
        AfterMoveAction(SwapPositionA)
        AfterMoveAction(SwapPositionB)

        _IsBusy = False
    End Sub

    Public Sub Clear()

        If UBound(_Items) + 1 <> MinSize Then
            ReDim _Items(MinSize - 1)
        End If
        _ItemCount = 0
    End Sub

    Public Sub Deallocate()

        Clear()
        _Items = Nothing

        _IsBusy = True
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

    Public Sub SendItemsShuffled(ByVal OtherList As SimpleList(Of ItemType), ByVal NumberGenerator As Random)
        Dim A As Integer
        Dim Copy As New SimpleList(Of ItemType)
        Dim Position As Integer

        SendItems(Copy)
        For A = 0 To _ItemCount - 1
            Position = Math.Min(CInt(Int(NumberGenerator.NextDouble * Copy.ItemCount)), Copy.ItemCount - 1)
            OtherList.Add(Copy.Item(Position))
            Copy.Remove(Position)
        Next
    End Sub

    Public Sub RemoveBuffer()

        ReDim Preserve _Items(_ItemCount - 1)
    End Sub

    Protected Overridable Sub AfterMoveAction(position As Integer)


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

    Public Overrides Sub Add(ByVal NewItem As ItemType)

        Select Case AddNothingAction
            Case SimpleClassList_AddNothingAction.Allow
                MyBase.Add(NewItem)
            Case SimpleClassList_AddNothingAction.DisallowIgnore
                If NewItem IsNot Nothing Then
                    MyBase.Add(NewItem)
                End If
            Case SimpleClassList_AddNothingAction.DisallowError
                If NewItem Is Nothing Then
                    Stop
                Else
                    MyBase.Add(NewItem)
                End If
            Case Else
                Stop
        End Select
    End Sub

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

Public Class AlteredSimpleClassList(Of ItemType As Class, SourceType As Class)
    Inherits SimpleClassList(Of ConnectedListItem(Of ItemType, SourceType))

    Protected Overrides Sub AfterMoveAction(position As Integer)

        Item(position).AfterMove(position)
    End Sub
End Class

Public Class ConnectedList(Of ItemType As Class, SourceType As Class)

    Private _Items As New AlteredSimpleClassList(Of ItemType, SourceType)
    Private _Owner As SourceType

    Public Sub New(ByVal Owner As SourceType)

        _Owner = Owner
        _Items.AddNothingAction = SimpleClassList_AddNothingAction.DisallowError
    End Sub

    Public ReadOnly Property Owner As SourceType
        Get
            Return _Owner
        End Get
    End Property

    Public Property MaintainOrder As Boolean
        Get
            Return _Items.MaintainOrder
        End Get
        Set(value As Boolean)
            _Items.MaintainOrder = value
        End Set
    End Property

    Public ReadOnly Property IsBusy As Boolean
        Get
            Return _Items.IsBusy
        End Get
    End Property

    Public ReadOnly Property ItemContainer(ByVal Position As Integer) As ConnectedListItem(Of ItemType, SourceType)
        Get
            Return _Items.Item(Position)
        End Get
    End Property

    Default Public ReadOnly Property Item(ByVal Position As Integer) As ItemType
        Get
            Return _Items.Item(Position).Item
        End Get
    End Property

    Public ReadOnly Property ItemCount() As Integer
        Get
            Return _Items.ItemCount
        End Get
    End Property

    Public Overridable Sub Add(ByVal NewItem As ConnectedListItem(Of ItemType, SourceType))

        If NewItem.BeforeAdd(Nothing, _Items.ItemCount) Then
            _Items.Add(NewItem)
        End If
    End Sub

    Public Overridable Sub Insert(ByVal NewItem As ConnectedListItem(Of ItemType, SourceType), ByVal Position As Integer)

        If NewItem.BeforeAdd(Nothing, Position) Then
            _Items.Insert(NewItem, Position)
        End If
    End Sub

    Public Overridable Sub Remove(ByVal Position As Integer)
        Dim tmpItem As ConnectedListItem(Of ItemType, SourceType)

        tmpItem = _Items.Item(Position)
        tmpItem.BeforeRemove()
        _Items.Remove(Position)
        tmpItem.AfterRemove()
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

    Public Sub Deallocate()

        Do While ItemCount > 0
            Remove(0)
        Loop
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

    Public Sub Clear()

        Do While _Items.ItemCount > 0
            _Items.Item(0).Disconnect()
        Loop
    End Sub
End Class

Public MustInherit Class ConnectedListItem(Of ItemType As Class, SourceType As Class)

    MustOverride ReadOnly Property Item As ItemType
    MustOverride ReadOnly Property Source As SourceType
    MustOverride Function BeforeAdd(ByVal NewList As ConnectedList(Of ItemType, SourceType), ByVal NewPosition As Integer) As Boolean
    MustOverride Sub BeforeRemove()
    MustOverride Sub AfterRemove()
    MustOverride Sub AfterMove(ByVal NewPosition As Integer)
    MustOverride Sub Disconnect()
End Class

Public Class ConnectedListLink(Of ItemType As Class, SourceType As Class)
    Inherits ConnectedListItem(Of ItemType, SourceType)

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

    Public Overrides Sub AfterMove(ByVal NewPosition As Integer)

        _ArrayPosition = NewPosition
    End Sub

    Public Overrides Sub BeforeRemove()

        _ConnectedList = Nothing
        _ArrayPosition = -1
    End Sub

    Public Overrides ReadOnly Property Item As ItemType
        Get
            Return _Item
        End Get
    End Property

    Public Overrides ReadOnly Property Source As SourceType
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

    Public Sub ConnectInsert(ByVal List As ConnectedList(Of ItemType, SourceType), ByVal Position As Integer)

        If IsConnected Then
            Stop
            Exit Sub
        End If

        List.Insert(Me, Position)
        _ConnectedList = List
    End Sub

    Public Overrides Sub Disconnect()

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

    Public Overrides Sub AfterRemove()

        _ArrayPosition = -1
        _ConnectedList = Nothing
    End Sub

    Public Overrides Function BeforeAdd(NewList As ConnectedList(Of ItemType, SourceType), NewPosition As Integer) As Boolean

        If IsConnected Then
            Stop
            Return False
        End If
        '_ConnectedList = NewList -- mono is unable to pass newlist
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
