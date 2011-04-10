Public Class clsByteReadFile

    Public Bytes(-1) As Byte
    Public ByteCount As Integer
    Public Position As Integer

    Function Get_U8(ByRef Output As Byte) As Boolean

        If Position < ByteCount Then
            Output = Bytes(Position)
            Position += 1
            Get_U8 = True
        Else
            Get_U8 = False
        End If
    End Function

    Function Get_U16(ByRef Output As UShort) As Boolean

        If Position + 2 <= ByteCount Then
            Output = BitConverter.ToUInt16(Bytes, Position)
            Position += 2
            Get_U16 = True
        Else
            Get_U16 = False
        End If
    End Function

    Function Get_U32(ByRef Output As UInteger) As Boolean

        If Position + 4 <= ByteCount Then
            Output = BitConverter.ToUInt32(Bytes, Position)
            Position += 4
            Get_U32 = True
        Else
            Get_U32 = False
        End If
    End Function

    Function Get_U64(ByRef Output As ULong) As Boolean

        If Position + 8 <= ByteCount Then
            Output = BitConverter.ToUInt64(Bytes, Position)
            Position += 8
            Get_U64 = True
        Else
            Get_U64 = False
        End If
    End Function

    Function Get_S16(ByRef Output As Short) As Boolean

        If Position + 2 <= ByteCount Then
            Output = BitConverter.ToInt16(Bytes, Position)
            Position += 2
            Get_S16 = True
        Else
            Get_S16 = False
        End If
    End Function

    Function Get_S32(ByRef Output As Integer) As Boolean

        If Position + 4 <= ByteCount Then
            Output = BitConverter.ToInt32(Bytes, Position)
            Position += 4
            Get_S32 = True
        Else
            Get_S32 = False
        End If
    End Function

    Function Get_F32(ByRef Output As Single) As Boolean

        If Position + 4 <= ByteCount Then
            Output = BitConverter.ToSingle(Bytes, Position)
            Position += 4
            Get_F32 = True
        Else
            Get_F32 = False
        End If
    End Function

    Function Get_Text_VariableLength(ByRef Output As String) As Boolean
        Static Length As Integer
        Static uintTemp As UInteger
        Static Chars() As Char
        Static A As Integer

        If Not Get_U32(uintTemp) Then
            Get_Text_VariableLength = False
            Exit Function
        End If
        Length = uintTemp
        If Position + Length <= ByteCount Then
            ReDim Chars(Length - 1)
            For A = 0 To Length - 1
                Chars(A) = Chr(Bytes(Position + A))
            Next
            Output = New String(Chars)
            Position += Length
            Get_Text_VariableLength = True
        Else
            Get_Text_VariableLength = False
        End If
    End Function

    Function Get_Text(ByVal Length As Integer, ByRef Output As String) As Boolean
        Static A As Integer
        Static Chars() As Char

        If Position + Length <= ByteCount Then
            ReDim Chars(Length - 1)
            For A = 0 To Length - 1
                Chars(A) = Chr(Bytes(Position + A))
            Next
            Output = New String(Chars)
            Position += Length
            Get_Text = True
        Else
            Get_Text = False
        End If
    End Function

    Function File_Read(ByVal Path As String) As sResult
        File_Read.Success = False
        File_Read.Problem = ""

        Try
            Bytes = IO.File.ReadAllBytes(Path)
            ByteCount = Bytes.GetUpperBound(0) + 1
        Catch ex As Exception
            File_Read.Problem = ex.Message
            Exit Function
        End Try
        Position = 0

        File_Read.Success = True
    End Function
End Class