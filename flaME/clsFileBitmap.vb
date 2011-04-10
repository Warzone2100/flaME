Imports OpenTK.Graphics.OpenGL

Public Class clsFileBitmap

    Public CurrentBitmap As Bitmap

    Sub New()

    End Sub

    Sub New(ByVal Width As Integer, ByVal Height As Integer)

        CurrentBitmap = New Bitmap(Width, Height)
    End Sub

    Function Load(ByVal Path As String) As sResult
        Load.Problem = ""
        Load.Success = False

        Try
            CurrentBitmap = New Bitmap(Path)
        Catch ex As Exception
            Load.Problem = ex.Message
            Exit Function
        End Try

        Load.Success = True
    End Function

    Function Save(ByVal Path As String, ByVal Overwrite As Boolean) As sResult
        Save.Problem = ""
        Save.Success = False

        Try

            Dim SaveFormat As Drawing.Imaging.ImageFormat
            Select Case Strings.LCase(Strings.Right(Path, 4))
                Case ".bmp"
                    SaveFormat = Drawing.Imaging.ImageFormat.Bmp
                Case Else
                    Save.Problem = "Unrecognised file extension."
                    Exit Function
            End Select

            If IO.File.Exists(Path) Then
                If Overwrite Then
                    IO.File.Delete(Path)
                Else
                    Save.Problem = "File already exists."
                    Exit Function
                End If
            End If
            CurrentBitmap.Save(Path, SaveFormat)

        Catch ex As Exception
            Save.Problem = ex.Message
            Exit Function
        End Try

        Save.Success = True
    End Function

    Function GL_Texture_Create() As Integer

        Dim tmpData As Drawing.Imaging.BitmapData = CurrentBitmap.LockBits(New Rectangle(0, 0, CurrentBitmap.Width, CurrentBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL_Texture_Create = 0
        GL.GenTextures(1, GL_Texture_Create)
        GL.BindTexture(TextureTarget.Texture2D, GL_Texture_Create)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, CurrentBitmap.Width, CurrentBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)

        CurrentBitmap.UnlockBits(tmpData)
    End Function

    Function GL_Texture_Create2(ByVal GLNum As Integer, ByVal IsMipmapped As Boolean) As Integer

        Dim tmpData As Drawing.Imaging.BitmapData = CurrentBitmap.LockBits(New Rectangle(0, 0, CurrentBitmap.Width, CurrentBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If GLNum = frmMainInstance.View.GL_Num Then
            If GL_Current <> frmMainInstance.View.GL_Num Then
                frmMainInstance.View.OpenGL.MakeCurrent()
                GL_Current = frmMainInstance.View.GL_Num
            End If
        ElseIf GLNum = frmMainInstance.TextureView.GL_Num Then
            If GL_Current <> frmMainInstance.TextureView.GL_Num Then
                frmMainInstance.TextureView.OpenGL.MakeCurrent()
                GL_Current = frmMainInstance.TextureView.GL_Num
            End If
        Else
            Return 0
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL_Texture_Create2 = 0
        GL.GenTextures(1, GL_Texture_Create2)
        GL.BindTexture(TextureTarget.Texture2D, GL_Texture_Create2)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        If IsMipmapped Then
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.NearestMipmapNearest)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, CurrentBitmap.Width, CurrentBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)
        Else
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, CurrentBitmap.Width, CurrentBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)
        End If

        CurrentBitmap.UnlockBits(tmpData)
    End Function

    Sub GL_Texture_Create3(ByVal GLTextureNum As Integer, ByVal Level As Integer)

        Dim tmpData As Drawing.Imaging.BitmapData = CurrentBitmap.LockBits(New Rectangle(0, 0, CurrentBitmap.Width, CurrentBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If GL_Current <> frmMainInstance.View.GL_Num Then
            frmMainInstance.View.OpenGL.MakeCurrent()
            GL_Current = frmMainInstance.View.GL_Num
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.BindTexture(TextureTarget.Texture2D, GLTextureNum)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.NearestMipmapNearest)
        GL.TexImage2D(TextureTarget.Texture2D, Level, PixelInternalFormat.Rgba, CurrentBitmap.Width, CurrentBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)

        CurrentBitmap.UnlockBits(tmpData)
    End Sub
End Class