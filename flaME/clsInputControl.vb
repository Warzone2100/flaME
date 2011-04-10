Public Class clsInputControl

    Class clsKeyCombo
        Public Keys(-1) As Integer
        Public KeyCount As Integer
        Public UnlessKey(-1) As Integer
        Public UnlessKeyCount As Integer

        Function IsPressed(ByRef IsKeyDown() As Boolean) As Boolean
            Static A As Integer
            Static B As Integer

            For A = 0 To KeyCount - 1
                If Not IsKeyDown(Keys(A)) Then
                    Exit For
                End If
            Next
            For B = 0 To UnlessKeyCount - 1
                If IsKeyDown(UnlessKey(B)) Then
                    Exit For
                End If
            Next
            IsPressed = ((A = KeyCount) And (B = UnlessKeyCount))
        End Function

        Sub New(ByVal KeyA As Integer)

            KeyCount = 1
            ReDim Keys(0)
            Keys(0) = KeyA
        End Sub

        Sub New(ByVal KeyA As Integer, ByVal KeyB As Integer)

            KeyCount = 2
            ReDim Keys(1)
            Keys(0) = KeyA
            Keys(1) = KeyB
        End Sub

        Sub New(ByVal KeyA As Integer, ByVal KeyB As Integer, ByVal KeyC As Integer)

            KeyCount = 3
            ReDim Keys(2)
            Keys(0) = KeyA
            Keys(1) = KeyB
            Keys(2) = KeyC
        End Sub

        Sub New(ByVal Key_Combo_To_Copy As clsKeyCombo)
            Dim A As Integer

            KeyCount = Key_Combo_To_Copy.KeyCount
            ReDim Keys(KeyCount - 1)
            For A = 0 To KeyCount - 1
                Keys(A) = Key_Combo_To_Copy.Keys(A)
            Next
            UnlessKeyCount = Key_Combo_To_Copy.UnlessKeyCount
            ReDim UnlessKey(UnlessKeyCount - 1)
            For A = 0 To UnlessKeyCount - 1
                UnlessKey(A) = Key_Combo_To_Copy.UnlessKey(A)
            Next
        End Sub

        Sub Unless_Key_Add(ByVal Key As Integer)

            ReDim Preserve UnlessKey(UnlessKeyCount)
            UnlessKey(UnlessKeyCount) = Key
            UnlessKeyCount += 1
        End Sub
    End Class
    Public DefaultKeys As clsKeyCombo

    Public KeyCombo As clsKeyCombo

    Public Active As Boolean

    Sub SetToDefault()

        If DefaultKeys IsNot Nothing Then
            KeyCombo = New clsKeyCombo(DefaultKeys)
        End If
    End Sub

    Sub KeysChanged(ByRef IsKeyDown() As Boolean)

        Active = False
        If KeyCombo IsNot Nothing Then
            If KeyCombo.IsPressed(IsKeyDown) Then
                Active = True
            End If
        End If
    End Sub
End Class