Public Class ConnectedList(Of ItemType As Class, SourceType As Class)

    Private _Owner As SourceType
    Private _Items(0) As ConnectedListItem(Of ItemType, SourceType)
    Private _ItemCount As Integer = 0

    Public Sub New(ByVal Owner As SourceType)

        _Owner = Owner
    End Sub

    Public ReadOnly Property Owner As SourceType
        Get
            Return _Owner
        End Get
    End Property

    Public ReadOnly Property ItemCount As Integer
        Get
            Return _ItemCount
        End Get
    End Property

    Public ReadOnly Property ItemContainer(ByVal Number As Integer) As ConnectedListItem(Of ItemType, SourceType)
        Get
            Return _Items(Number)
        End Get
    End Property

    Public ReadOnly Property Item(ByVal Number As Integer) As ItemType
        Get
            Return _Items(Number).Item
        End Get
    End Property

    Public Sub Add(ByVal NewItem As ConnectedListItem(Of ItemType, SourceType))

        NewItem.BeforeAdd(ItemCount)
        If _Items.GetUpperBound(0) < ItemCount Then
            ReDim Preserve _Items(ItemCount * 2 + 1)
        End If
        _Items(ItemCount) = NewItem
        _ItemCount += 1
    End Sub

    Public Sub Remove(ByVal Number As Integer)

        If Number < 0 Or Number >= ItemCount Then
            Stop
            Exit Sub
        End If

        _Items(Number).BeforeRemove()

        _ItemCount -= 1
        If Number < _ItemCount Then
            Dim LastItem As ConnectedListItem(Of ItemType, SourceType) = _Items(_ItemCount)
            _Items(Number) = LastItem
            LastItem.AfterMove(Number)
        End If
        _Items(_ItemCount) = Nothing
        If _ItemCount * 3 < _Items.GetUpperBound(0) + 1 Then
            ReDim Preserve _Items(_ItemCount * 2 - 1)
        End If
    End Sub

    Public Function FindLink(ByVal Item As ItemType) As ConnectedListItem(Of ItemType, SourceType)
        Dim A As Integer
        Dim Link As ConnectedListItem(Of ItemType, SourceType)

        For A = 0 To _ItemCount - 1
            Link = _Items(A)
            If Link.Item Is Item Then
                Return Link
            End If
        Next
        Return Nothing
    End Function

    Public Sub Clear()

        While _ItemCount > 0
            Remove(_ItemCount - 1)
        End While
    End Sub

    Public Sub Deallocate()

        Clear()
        _Owner = Nothing
        Erase _Items
    End Sub
End Class

Public Interface ConnectedListItem(Of ItemType As Class, SourceType As Class)

    ReadOnly Property Item As ItemType
    ReadOnly Property Source As SourceType
    Sub BeforeAdd(ByVal NewPosition As Integer)
    Sub BeforeRemove()
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

    Public Sub BeforeAdd(ByVal NewPosition As Integer) Implements ConnectedListItem(Of ItemType, SourceType).BeforeAdd

        _ArrayPosition = NewPosition
    End Sub

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

        If _ArrayPosition >= 0 Then
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

        If _ArrayPosition >= 0 Then
            Disconnect()
        End If
        _Item = Nothing
    End Sub
End Class