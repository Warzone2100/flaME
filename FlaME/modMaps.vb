Public Module modMaps

    Private _LoadedMaps(-1) As clsMap
#If Mono267 <> 0.0# Then
    Public LoadedMaps() As clsMap
#End If
    Private _LoadedMapCount As Integer

    Private _MainMap As clsMap

    Public Sub NewMainMap(ByVal NewMap As clsMap)

        LoadedMap_Add(NewMap)
        SetMainMap(NewMap)
    End Sub

    Public Sub UpdateMapTabs()

        frmMainInstance.MapView.UpdateTabs()
    End Sub

    Public Sub LoadedMap_Add(ByVal NewMap As clsMap)

        If NewMap.LoadedMap_Num >= 0 Then
            Stop
            Exit Sub
        End If

        If Not NewMap.ReadyForUserInput Then
            NewMap.InitializeUserInput()
        End If

        NewMap.LoadedMap_Num = _LoadedMapCount

        NewMap.MapView_TabPage = New TabPage
        NewMap.MapView_TabPage.Tag = NewMap
        NewMap.SetTabText()

        If _LoadedMaps.GetUpperBound(0) < _LoadedMapCount Then
            ReDim Preserve _LoadedMaps(_LoadedMapCount * 2 + 1)
        End If
        _LoadedMaps(_LoadedMapCount) = NewMap
        _LoadedMapCount += 1

#If Mono267 <> 0.0# Then
        LoadedMaps = _LoadedMaps
#End If
    End Sub

    Public Sub LoadedMap_Remove(ByVal Num As Integer)
        Dim Map As clsMap = _LoadedMaps(Num)
        Dim OldNum As Integer = Map.LoadedMap_Num
        Dim A As Integer

        Map.LoadedMap_Num = -1

        Map.MapView_TabPage.Tag = Nothing
        Map.MapView_TabPage = Nothing

        _LoadedMapCount -= 1
        For A = Num To _LoadedMapCount - 1
            _LoadedMaps(A) = _LoadedMaps(A + 1)
            _LoadedMaps(A).LoadedMap_Num = A
        Next
        _LoadedMaps(_LoadedMapCount) = Nothing
        If _LoadedMapCount * 3 < _LoadedMaps.GetUpperBound(0) + 1 Then
            ReDim Preserve _LoadedMaps(_LoadedMapCount * 2 - 1)
        End If

        If _MainMap Is Map Then
            If _LoadedMapCount > 0 Then
                SetMainMap(_LoadedMaps(Math.Max(OldNum - 1, 0)))
            Else
                SetMainMap(Nothing)
            End If
        End If

#If Mono267 <> 0.0# Then
        LoadedMaps = _LoadedMaps
#End If
    End Sub

    Public ReadOnly Property MainMap As clsMap
        Get
            Return _MainMap
        End Get
    End Property

    Public Sub SetMainMap(ByVal Map As clsMap)

        If Map Is Nothing Then

        ElseIf Map.LoadedMap_Num < 0 Then
            MsgBox("Error: Invalid main map.")
            Exit Sub
        End If
        If _MainMap Is Map Then
            Exit Sub
        End If
        frmMainInstance.MainMapBeforeChanged()
        _MainMap = Map
        frmMainInstance.MainMapAfterChanged()
    End Sub

    Public ReadOnly Property LoadedMapCount As Integer
        Get
            Return _LoadedMapCount
        End Get
    End Property

#If Mono267 = 0.0# Then
    Public ReadOnly Property LoadedMaps(ByVal Num As Integer) As clsMap
        Get
            Return _LoadedMaps(Num)
        End Get
    End Property
#End If
End Module