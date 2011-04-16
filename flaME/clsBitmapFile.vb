Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class clsBitmapFile

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

    Function GLTexture(ByVal GLControlToGiveTo As OpenTK.GLControl, ByVal IsMipmapped As Boolean) As Integer

        Dim tmpData As Drawing.Imaging.BitmapData = CurrentBitmap.LockBits(New Rectangle(0, 0, CurrentBitmap.Width, CurrentBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If GraphicsContext.CurrentContext IsNot GLControlToGiveTo.Context Then
            GLControlToGiveTo.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GLTexture = 0
        GL.GenTextures(1, GLTexture)
        GL.BindTexture(TextureTarget.Texture2D, GLTexture)
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

    Sub GLTexture(ByVal GLControlToGiveTo As OpenTK.GLControl, ByVal GLTextureNum As Integer, ByVal MipMapLevel As Integer)

        Dim tmpData As Drawing.Imaging.BitmapData = CurrentBitmap.LockBits(New Rectangle(0, 0, CurrentBitmap.Width, CurrentBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If GraphicsContext.CurrentContext IsNot GLControlToGiveTo.Context Then
            GLControlToGiveTo.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.BindTexture(TextureTarget.Texture2D, GLTextureNum)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.NearestMipmapNearest)
        GL.TexImage2D(TextureTarget.Texture2D, MipMapLevel, PixelInternalFormat.Rgba, CurrentBitmap.Width, CurrentBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)

        CurrentBitmap.UnlockBits(tmpData)
    End Sub
End Class