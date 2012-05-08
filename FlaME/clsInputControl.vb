
Public Class clsInputControl

    Public Class clsKeyCombo
        Public Keys() As Keys
        Public UnlessKeys() As Keys

        Public Function IsPressed(KeysDown As clsKeysActive) As Boolean
            Dim Key As Keys

            For Each Key In Keys
                If Not KeysDown.Keys(Key) Then
                    Return False
                End If
            Next
            For Each Key In UnlessKeys
                If KeysDown.Keys(Key) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Sub New(Keys() As Keys)

            Me.Keys = Keys
            ReDim UnlessKeys(-1)
        End Sub

        Public Sub New(Keys() As Keys, UnlessKeys() As Keys)

            Me.Keys = Keys
            Me.UnlessKeys = UnlessKeys
        End Sub
    End Class

    Public DefaultKeys As clsKeyCombo

    Public KeyCombo As clsKeyCombo

    Public Active As Boolean

    Public Sub SetToDefault()

        If DefaultKeys IsNot Nothing Then
            KeyCombo = DefaultKeys
        End If
    End Sub

    Public Sub KeysChanged(KeysDown As clsKeysActive)

        Active = False
        If KeyCombo IsNot Nothing Then
            If KeyCombo.IsPressed(KeysDown) Then
                Active = True
            End If
        End If
    End Sub
End Class
