Imports ICSharpCode.SharpZipLib

Public Class clsReadFile

    Private Const DefaultBufferLength As Integer = 524288
    Private _ByteBufferLength As Integer = DefaultBufferLength
    Private Enum enumStreamType As Byte
        None
        FileStream
        ZipStream
        FixedBytes
    End Enum
    Private Type As enumStreamType = enumStreamType.None
    Private FileStream As IO.FileStream
    Private FilePosition As Long
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
            If Type <> enumStreamType.None Then
                RedimBytesToBufferLength()
            End If
        End Set
    End Property

    Private Sub RedimBytesToBufferLength()

        ReDim Bytes(_ByteBufferLength - 1)
    End Sub

    Public ReadOnly Property Position As Long
        Get
            Return FilePosition + BytesPosition
        End Get
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
        Try
            Length = uintTemp
        Catch ex As Exception
            Return False
        End Try
        Return Get_Text(Length, Output)
    End Function

    Function Get_Text(ByVal Length As Integer, ByRef Output As String) As Boolean
        Static Chars() As Char
        Static CharOffset As Integer
        Static CharsLeft As Integer
        Static ReadLength As Integer
        Static ReadNum As Integer

        'read in buffer length blocks, for long strings
        ReDim Chars(Length - 1)
        CharOffset = 0
        Do While CharOffset < Length
            CharsLeft = Length - CharOffset
            ReadLength = Math.Min(CharsLeft, _ByteBufferLength - BytesPosition)
            If ReadLength = 0 Then
                ReadLength = Math.Min(CharsLeft, _ByteBufferLength)
                If Not FindLength(ReadLength) Then
                    Return False
                End If
            End If
            For ReadNum = 0 To ReadLength - 1
                Chars(CharOffset + ReadNum) = Chr(Bytes(BytesPosition + ReadNum))
            Next
            BytesPosition += ReadLength
            CharOffset += ReadLength
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
        FilePosition = 0L
        ByteCount = 0
        RedimBytesToBufferLength()
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
        FilePosition = 0L
        BufferLength = ReadLength
        ByteCount = Stream.Read(Bytes, 0, _ByteBufferLength)
        BytesPosition = 0
    End Sub

    Private Function ReadBlock() As Boolean

        If Type <> enumStreamType.FileStream Then ' And Type <> enumStreamType.ZipStream Then
            Return False
        End If

        FilePosition += BytesPosition
        BytesPosition = 0
        Try
            ByteCount = FileStream.Read(Bytes, 0, _ByteBufferLength)
            Return (ByteCount > 0)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub Close()

        If Type = enumStreamType.FileStream Then
            FileStream.Close()
            FileStream = Nothing
        End If

        Type = enumStreamType.None
        BufferLength = DefaultBufferLength
        Erase Bytes
    End Sub

    Public Function Seek(ByVal NewPosition As Long) As Boolean

        If NewPosition < FilePosition Or NewPosition >= FilePosition + ByteCount Then
            If Type <> enumStreamType.FileStream Then
                Return False
            End If
            FilePosition = FileStream.Seek(NewPosition, IO.SeekOrigin.Begin)
            BytesPosition = 0
            Return ReadBlock()
        Else
            BytesPosition = NewPosition - FilePosition
            Return True
        End If
    End Function
End Class