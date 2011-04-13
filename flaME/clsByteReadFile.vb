Imports ICSharpCode.SharpZipLib

Public Class clsByteReadFile

    Private Const DefaultBufferLength As Integer = 524288
    Private _ByteBufferLength As Integer = DefaultBufferLength
    Private Enum enumStreamType As Byte
        None
        FileStream
        FixedBytes
    End Enum
    Private Type As enumStreamType = enumStreamType.None
    Private FileStream As IO.FileStream
    Private FilePosition As Integer
    Private FileNextPosition As Integer
    Private Bytes() As Byte
    Private ByteCount As Integer
    Private BytesPosition As Integer

    Public Property BufferLength As Integer
        Get
            Return _ByteBufferLength
        End Get
        Set(ByVal value As Integer)
            If value = _ByteBufferLength Or value < 8 Then
                Exit Property
            End If
            _ByteBufferLength = value
            RedimBytes()
        End Set
    End Property

    Public Sub New()

        RedimBytes()
    End Sub

    Private Sub RedimBytes()

        ReDim Bytes(_ByteBufferLength - 1)
    End Sub

    Public Property Position As Integer
        Get
            Return FilePosition + BytesPosition
        End Get
        Set(ByVal NewPosition As Integer)
            If NewPosition < FilePosition Then
                FilePosition = NewPosition
                ReadBlock()
            ElseIf NewPosition >= FilePosition + ByteCount Then
                FilePosition = NewPosition
                ReadBlock()
            Else
                BytesPosition = NewPosition - FilePosition
            End If
        End Set
    End Property

    Private Function FindLength(ByVal Length As Integer) As Boolean

        If Length < 0 Or Length > _ByteBufferLength Then
            Stop
            Return False
        End If
        If BytesPosition + Length > ByteCount Then
            Return ReadBlock()
        Else
            Return True
        End If
    End Function

    Function Get_U8(ByRef Output As Byte) As Boolean

        If FindLength(1) Then
            Output = Bytes(BytesPosition)
            BytesPosition += 1
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_U16(ByRef Output As UShort) As Boolean

        If FindLength(2) Then
            Output = BitConverter.ToUInt16(Bytes, BytesPosition)
            BytesPosition += 2
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_U32(ByRef Output As UInteger) As Boolean

        If FindLength(4) Then
            Output = BitConverter.ToUInt32(Bytes, BytesPosition)
            BytesPosition += 4
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_U64(ByRef Output As ULong) As Boolean

        If FindLength(8) Then
            Output = BitConverter.ToUInt64(Bytes, BytesPosition)
            BytesPosition += 8
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_S16(ByRef Output As Short) As Boolean

        If FindLength(2) Then
            Output = BitConverter.ToInt16(Bytes, BytesPosition)
            BytesPosition += 2
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_S32(ByRef Output As Integer) As Boolean

        If FindLength(4) Then
            Output = BitConverter.ToInt32(Bytes, BytesPosition)
            BytesPosition += 4
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_F32(ByRef Output As Single) As Boolean

        If FindLength(4) Then
            Output = BitConverter.ToSingle(Bytes, BytesPosition)
            BytesPosition += 4
            Return True
        Else
            Return False
        End If
    End Function

    Function Get_Text_VariableLength(ByRef Output As String) As Boolean
        Static Length As Integer
        Static uintTemp As UInteger

        If Not Get_U32(uintTemp) Then
            Return False
        End If
        Length = uintTemp
        Return Get_Text(Length, Output)
    End Function

    Function Get_Text(ByVal Length As Integer, ByRef Output As String) As Boolean
        Static Chars() As Char
        Static ReadLength As Integer
        Static Offset As Integer
        Static A As Integer

        'read in buffer length blocks, for long strings
        ReDim Chars(Length - 1)
        Offset = 0
        Do While Offset < Length
            ReadLength = Math.Min(Length - Offset, _ByteBufferLength)
            If Not FindLength(ReadLength) Then
                Return False
            End If
            For A = 0 To ReadLength - 1
                Chars(Offset + A) = Chr(Bytes(BytesPosition + A))
            Next
            BytesPosition += ReadLength
            Offset += ReadLength
        Loop
        Output = New String(Chars)
        Return True
    End Function

    Public Function Begin(ByVal Path As String) As sResult
        Begin.Success = False
        Begin.Problem = ""

        Close()
        Type = enumStreamType.None
        Try
            FileStream = New IO.FileStream(Path, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
        Catch ex As Exception
            Begin.Problem = ex.Message
            Exit Function
        End Try
        Type = enumStreamType.FileStream
        FilePosition = 0
        ByteCount = 0
        ReadBlock()

        Begin.Success = True
    End Function

    Public Sub Begin(ByVal Stream As Zip.ZipInputStream, ByVal ReadLength As Integer)

        Close()
        If Stream Is Nothing Then
            Type = enumStreamType.None
            Exit Sub
        End If
        Type = enumStreamType.FixedBytes
        FilePosition = 0
        BufferLength = ReadLength
        ByteCount = Stream.Read(Bytes, 0, _ByteBufferLength)
        BytesPosition = 0
    End Sub

    Private Function ReadBlock() As Boolean

        If Type <> enumStreamType.FileStream Then
            Return False
        End If

        Try
            FilePosition += BytesPosition
            FileStream.Seek(FilePosition, IO.SeekOrigin.Begin)
            ByteCount = FileStream.Read(Bytes, 0, _ByteBufferLength)
        Catch ex As Exception
            Return False
        End Try
        BytesPosition = 0

        Return True
    End Function

    Public Sub Close()

        BufferLength = DefaultBufferLength

        If Type <> enumStreamType.FileStream Then
            Exit Sub
        End If

        FileStream.Close()
        FileStream = Nothing
    End Sub
End Class