Public Class clsByteWriteFile

    Public Bytes(-1) As Byte
    Public ByteCount As Integer
    Public ByteBufferLength As Integer = 8192

    Function Make_Length(ByVal Extra_Length As Integer) As Integer
        Static Num As Integer

        Num = ByteCount + Extra_Length - 1
        If Bytes.GetUpperBound(0) < Num Then
            ReDim Preserve Bytes(Num + ByteBufferLength)
        End If
        Make_Length = ByteCount
        ByteCount += Extra_Length
    End Function

    Sub Text_Append(ByVal Text As String, Optional ByVal LengthDescriptor As Boolean = False)
        Static Num As Integer
        Static A As Integer

        If LengthDescriptor Then
            U32_Append(Text.Length)
        End If
        Num = Make_Length(Text.Length)
        For A = 0 To Text.Length - 1
            Bytes(Num + A) = Asc(Text.Chars(A))
        Next
    End Sub

    Sub Text_Append(ByVal Text As String, ByVal Length As Integer)
        Static Num As Integer
        Static A As Integer

        Num = Make_Length(Length)
        For A = 0 To Math.Min(Text.Length, Length) - 1
            Bytes(Num + A) = Asc(Text.Chars(A))
        Next
        For A = A To Length - 1
            Bytes(Num + A) = 0
        Next
    End Sub

    Sub U8_Append(ByVal Value As Byte)
        Static Num As Integer

        Num = Make_Length(1)
        Bytes(Num) = Value
    End Sub

    Sub U16_Append(ByVal Value As UShort)
        Static tmpByte() As Byte
        Static Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(2)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
    End Sub

    Sub U32_Append(ByVal Value As UInteger)
        Static tmpByte() As Byte
        Static Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub S16_Append(ByVal Value As Short)
        Static tmpByte() As Byte
        Static Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(2)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
    End Sub

    Sub S32_Append(ByVal Value As Integer)
        Static tmpByte() As Byte
        Static Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub F32_Append(ByVal Value As Single)
        Static tmpByte() As Byte
        Static Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub Trim_Buffer()

        ReDim Preserve Bytes(ByteCount - 1)
    End Sub
End Class