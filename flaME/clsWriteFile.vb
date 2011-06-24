Imports ICSharpCode.SharpZipLib

Public Class clsWriteFile

    Public Bytes(-1) As Byte
    Public ByteCount As Integer
    Public ByteBufferLength As Integer = 8192

    Function Make_Length(ByVal Extra_Length As Integer) As Integer
        Dim Num As Integer

        Num = ByteCount + Extra_Length - 1
        If Bytes.GetUpperBound(0) < Num Then
            ReDim Preserve Bytes(Num + ByteBufferLength)
        End If
        Make_Length = ByteCount
        ByteCount += Extra_Length
    End Function

    Sub Text_Append(ByVal Text As String, Optional ByVal LengthDescriptor As Boolean = False)
        Dim Num As Integer
        Dim A As Integer

        If LengthDescriptor Then
            U32_Append(Text.Length)
        End If
        Num = Make_Length(Text.Length)
        For A = 0 To Text.Length - 1
            Bytes(Num + A) = Asc(Text.Chars(A))
        Next
    End Sub

    Sub Text_Append(ByVal Text As String, ByVal Length As Integer)
        Dim Num As Integer
        Dim A As Integer

        Num = Make_Length(Length)
        For A = 0 To Math.Min(Text.Length, Length) - 1
            Bytes(Num + A) = Asc(Text.Chars(A))
        Next
        For A = A To Length - 1
            Bytes(Num + A) = 0
        Next
    End Sub

    Sub U8_Append(ByVal Value As Byte)
        Dim Num As Integer

        Num = Make_Length(1)
        Bytes(Num) = Value
    End Sub

    Sub U16_Append(ByVal Value As UShort)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(2)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
    End Sub

    Sub U32_Append(ByVal Value As UInteger)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub S16_Append(ByVal Value As Short)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(2)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
    End Sub

    Sub S32_Append(ByVal Value As Integer)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub F32_Append(ByVal Value As Single)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(4)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
    End Sub

    Sub F64_Append(ByVal Value As Double)
        Dim tmpByte() As Byte
        Dim Num As Integer

        tmpByte = BitConverter.GetBytes(Value)

        Num = Make_Length(8)
        Bytes(Num) = tmpByte(0)
        Bytes(Num + 1) = tmpByte(1)
        Bytes(Num + 2) = tmpByte(2)
        Bytes(Num + 3) = tmpByte(3)
        Bytes(Num + 4) = tmpByte(4)
        Bytes(Num + 5) = tmpByte(5)
        Bytes(Num + 6) = tmpByte(6)
        Bytes(Num + 7) = tmpByte(7)
    End Sub

    Sub Trim_Buffer()

        If Bytes.GetUpperBound(0) <> ByteCount - 1 Then
            ReDim Preserve Bytes(ByteCount - 1)
        End If
    End Sub

    Public Function WriteFile(ByVal Path As String, ByVal Overwrite As Boolean) As clsResult
        Dim ReturnResult As New clsResult

        Trim_Buffer()
        If IO.File.Exists(Path) Then
            If Overwrite Then
                Try
                    IO.File.Delete(Path)
                Catch ex As Exception
                    ReturnResult.Problem_Add("Unable to delete existing file: " & ex.Message)
                    Return ReturnResult
                End Try
            Else
                ReturnResult.Problem_Add("A file already exists at " & Path)
                Return ReturnResult
            End If
        End If
        Try
            IO.File.WriteAllBytes(Path, Bytes)
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function

    Public Function WriteToZip(ByVal ZipOutputStream As Zip.ZipOutputStream, ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult

        Trim_Buffer()
        Try
            Dim NewZipEntry As New Zip.ZipEntry(Path)
            Dim Crc32 As New Checksums.Crc32
            NewZipEntry.DateTime = Now
            NewZipEntry.Size = ByteCount
            NewZipEntry.ExternalFileAttributes = 32
            Crc32.Reset()
            ZipOutputStream.PutNextEntry(NewZipEntry)
            ZipOutputStream.Write(Bytes, 0, ByteCount)
            Crc32.Update(Bytes, 0, ByteCount)
            NewZipEntry.Crc = Crc32.Value
        Catch ex As Exception
            ReturnResult.Problem_Add(ex.Message)
        End Try

        Return ReturnResult
    End Function
End Class