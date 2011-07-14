Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Module modBitmap

    Function LoadBitmap(ByVal Path As String, ByRef ResultBitmap As Bitmap) As sResult
        LoadBitmap.Problem = ""
        LoadBitmap.Success = False

        Dim tmpBitmap As Bitmap

        Try
            tmpBitmap = New Bitmap(Path)
        Catch ex As Exception
            LoadBitmap.Problem = ex.Message
            ResultBitmap = Nothing
            Exit Function
        End Try

        ResultBitmap = New Bitmap(tmpBitmap) 'copying the bitmap is needed so it doesn't lock access to the file

        LoadBitmap.Success = True
    End Function

    Function SaveBitmap(ByVal Path As String, ByVal Overwrite As Boolean, ByVal BitmapToSave As Bitmap) As sResult
        SaveBitmap.Problem = ""
        SaveBitmap.Success = False

        Try

            If IO.File.Exists(Path) Then
                If Overwrite Then
                    IO.File.Delete(Path)
                Else
                    SaveBitmap.Problem = "File already exists."
                    Exit Function
                End If
            End If
            BitmapToSave.Save(Path)

        Catch ex As Exception
            SaveBitmap.Problem = ex.Message
            Exit Function
        End Try

        SaveBitmap.Success = True
    End Function

    Function BitmapGLTexture(ByVal Texture As Bitmap, ByVal GLControlToGiveTo As OpenTK.GLControl, ByVal IsMipmapped As Boolean, ByVal Interpolation As Boolean) As Integer

        Dim tmpData As Drawing.Imaging.BitmapData = Texture.LockBits(New Rectangle(0, 0, Texture.Width, Texture.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If OpenTK.Graphics.GraphicsContext.CurrentContext IsNot GLControlToGiveTo.Context Then
            GLControlToGiveTo.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        BitmapGLTexture = 0
        GL.GenTextures(1, BitmapGLTexture)
        GL.BindTexture(TextureTarget.Texture2D, BitmapGLTexture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        If Interpolation Then
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
        Else
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        End If
        If IsMipmapped Then
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.NearestMipmapNearest)
        Else
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Nearest)
        End If
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Texture.Width, Texture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)

        Texture.UnlockBits(tmpData)
    End Function

    Sub BitmapGLTexture(ByVal Texture As Bitmap, ByVal GLControlToGiveTo As OpenTK.GLControl, ByVal GLTextureNum As Integer, ByVal MipMapLevel As Integer)

        Dim tmpData As Drawing.Imaging.BitmapData = Texture.LockBits(New Rectangle(0, 0, Texture.Width, Texture.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)

        If OpenTK.Graphics.GraphicsContext.CurrentContext IsNot GLControlToGiveTo.Context Then
            GLControlToGiveTo.MakeCurrent()
        End If

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1)
        GL.BindTexture(TextureTarget.Texture2D, GLTextureNum)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.NearestMipmapNearest)
        GL.TexImage2D(TextureTarget.Texture2D, MipMapLevel, PixelInternalFormat.Rgba, Texture.Width, Texture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)

        Texture.UnlockBits(tmpData)
    End Sub

    Function BitmapIsGLCompatible(ByVal BitmapToCheck As Bitmap) As clsResult
        Dim ReturnResult As New clsResult

        If Not SizeIsPowerOf2(BitmapToCheck.Width) Then
            ReturnResult.Warning_Add("Image width is not a power of 2.")
        End If
        If Not SizeIsPowerOf2(BitmapToCheck.Height) Then
            ReturnResult.Warning_Add("Image height is not a power of 2.")
        End If
        If BitmapToCheck.Width <> BitmapToCheck.Height Then
            ReturnResult.Warning_Add("Image is not square.")
        End If

        Return ReturnResult
    End Function
End Module