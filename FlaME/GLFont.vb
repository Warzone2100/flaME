Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL

Public Class GLFont

    Public BaseFont As Font
    Public Structure sCharacter
        Public GLTexture As Integer
        Public TexSize As Integer
        Public Width As Integer
    End Structure
    Public Character(255) As sCharacter
    Public Height As Integer

    Public Sub New(ByVal BaseFont As Font)

        GLTextures_Generate(BaseFont)
    End Sub

    Private Sub GLTextures_Generate(ByVal BaseFont As Font)
        Dim A As Integer
        Dim X As Integer
        Dim Y As Integer
        Dim TempBitmap As Bitmap
        Dim gfx As Graphics
        Dim tmpData As Drawing.Imaging.BitmapData
        Dim NewSizeX As Integer
        Dim StartX As Integer
        Dim FinishX As Integer
        Dim TexBitmap As Bitmap
        Dim tmpString As String

        BaseFont = BaseFont
        Height = BaseFont.Height
        For A = 0 To 255
            tmpString = Chr(A)
            TempBitmap = New Bitmap(Height * 2, Height, Imaging.PixelFormat.Format32bppArgb)
            gfx = Graphics.FromImage(TempBitmap)
            gfx.Clear(Color.Transparent)
            gfx.DrawString(tmpString, BaseFont, Brushes.White, 0.0F, 0.0F)
            gfx.Dispose()
            For X = 0 To TempBitmap.Width - 1
                For Y = 0 To TempBitmap.Height - 1
                    If TempBitmap.GetPixel(X, Y).A > 0 Then
                        Exit For
                    End If
                Next
                If Y < TempBitmap.Height Then Exit For
            Next
            StartX = X
            For X = TempBitmap.Width - 1 To 0 Step -1
                For Y = 0 To TempBitmap.Height - 1
                    If TempBitmap.GetPixel(X, Y).A > 0 Then
                        Exit For
                    End If
                Next
                If Y < TempBitmap.Height Then Exit For
            Next
            FinishX = X
            NewSizeX = FinishX - StartX + 1
            If NewSizeX <= 0 Then
                NewSizeX = Math.Max(CInt(Math.Round(Height / 4.0F)), 1)
                Character(A).TexSize = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0#))))
                TexBitmap = New Bitmap(Character(A).TexSize, Character(A).TexSize, Imaging.PixelFormat.Format32bppArgb)
                gfx = Graphics.FromImage(TexBitmap)
                gfx.Clear(Color.Transparent)
                gfx.Dispose()
                tmpData = TexBitmap.LockBits(New Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                GL.GenTextures(1, Character(A).GLTexture)
                GL.BindTexture(TextureTarget.Texture2D, Character(A).GLTexture)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)
                TexBitmap.UnlockBits(tmpData)
                Character(A).Width = NewSizeX
            Else
                Character(A).TexSize = CInt(Math.Round(2.0# ^ Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0#))))
                TexBitmap = New Bitmap(Character(A).TexSize, Character(A).TexSize, Imaging.PixelFormat.Format32bppArgb)
                gfx = Graphics.FromImage(TexBitmap)
                gfx.Clear(Color.Transparent)
                gfx.Dispose()
                For Y = 0 To TempBitmap.Height - 1
                    For X = StartX To FinishX
                        TexBitmap.SetPixel(X - StartX, Y, TempBitmap.GetPixel(X, Y))
                    Next
                Next
                tmpData = TexBitmap.LockBits(New Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                GL.GenTextures(1, Character(A).GLTexture)
                GL.BindTexture(TextureTarget.Texture2D, Character(A).GLTexture)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, tmpData.Scan0)
                TexBitmap.UnlockBits(tmpData)
                Character(A).Width = NewSizeX
            End If
            TempBitmap.Dispose()
            TexBitmap.Dispose()
        Next
    End Sub

    Public Sub Deallocate()
        Dim A As Integer

        For A = 0 To 255
            GL.DeleteTexture(Character(A).GLTexture)
        Next
    End Sub
End Class