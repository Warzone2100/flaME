Public Class clsHeightmap

    Public HeightScale As Double = 0.0001#

    Structure sMinMax
        Public Min As Long
        Public Max As Long
    End Structure

    Class clsHeightData
        Public SizeX As Integer
        Public SizeY As Integer
        Public Height(,) As Long
    End Class
    Public HeightData As New clsHeightData

    Sub Blank(ByVal SizeY As Integer, ByVal SizeX As Integer)

        HeightData.SizeX = SizeX
        HeightData.SizeY = SizeY
        ReDim HeightData.Height(SizeY - 1, SizeX - 1)
    End Sub

    Sub Randomize(ByVal HeightMultiplier As Double)
        Dim X As Integer
        Dim Y As Integer
        Dim HeightMultiplierHalved As Long

        HeightMultiplierHalved = HeightMultiplier / 2.0#
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = Rnd() * HeightMultiplier - HeightMultiplierHalved
            Next X
        Next Y
    End Sub

    Sub GenerateNew(ByVal SizeY As Integer, ByVal SizeX As Integer, ByVal Inflations As Integer, ByVal NoiseFactor As Double, ByVal HeightMultiplier As Double)
        Dim Temp As New clsHeightmap

        Blank(SizeY, SizeX)
        Randomize(HeightMultiplier / HeightScale)
        Temp.HeightScale = HeightScale
        Temp.Generate(Me, Inflations, NoiseFactor, HeightMultiplier / HeightScale)
        HeightData = Temp.HeightData 'steal the temporary heightmap's data
    End Sub

    Sub Generate(ByVal Source As clsHeightmap, ByVal Inflations As Integer, ByVal NoiseFactor As Double, ByVal HeightMultiplier As Double)
        Dim Temp As New clsHeightmap
        Dim A As Integer

        If Inflations >= 1 Then
            Temp.Inflate(Source, NoiseFactor, HeightMultiplier, 1)
            HeightData = Temp.HeightData
            Temp.HeightData = New clsHeightmap.clsHeightData
            For A = 2 To Inflations
                Temp.Inflate(Me, NoiseFactor, HeightMultiplier, A)
                HeightData = Temp.HeightData
                Temp.HeightData = New clsHeightmap.clsHeightData
            Next A
        ElseIf Inflations = 0 Then
            Copy(Source)
        Else
            Exit Sub
        End If
    End Sub

    Sub Inflate(ByVal Source As clsHeightmap, ByVal NoiseFactor As Double, ByVal HeightMultiplier As Double, ByVal VariationReduction As Integer)

        Dim A As Integer
        Dim Y As Integer
        Dim X As Integer

        Dim Variation As Double
        Dim VariationHalved As Long
        Dim Mean As Long
        Dim Dist As Double
        Dim LayerFactor As Double

        'make a larger copy of heightmap
        If Source.HeightData.SizeY = 0 Or Source.HeightData.SizeX = 0 Then Exit Sub
        Blank((Source.HeightData.SizeY - 1) * 2 + 1, (Source.HeightData.SizeX - 1) * 2 + 1)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height((Y + 1) * 2 - 2, (X + 1) * 2 - 2) = Source.HeightData.Height(Y, X)
            Next X
        Next Y

        If NoiseFactor = 0.0# Then
            LayerFactor = 0.0#
        Else
            LayerFactor = (2.0# / NoiseFactor) ^ (-VariationReduction)
        End If

        'centre points
        Dist = RootTwo
        Variation = Dist * LayerFactor * HeightMultiplier
        VariationHalved = Variation / 2.0#
        For Y = 1 To HeightData.SizeY - 2 Step 2
            For X = 1 To HeightData.SizeX - 2 Step 2
                Mean = (HeightData.Height(Y - 1, X - 1) + HeightData.Height(Y - 1, X + 1) + HeightData.Height(Y + 1, X - 1) + HeightData.Height(Y + 1, X + 1)) / 4.0#
                HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
            Next X
        Next Y

        'side points
        Dist = 1.0#
        Variation = Dist * LayerFactor * HeightMultiplier
        VariationHalved = Variation / 2.0#
        'inner side points
        For Y = 1 To HeightData.SizeY - 2
            A = Y - (Int(Y / 2) * 2)
            For X = 1 + A To HeightData.SizeX - 2 - A Step 2
                Mean = (HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 4
                HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
            Next X
        Next Y
        'top side points
        Y = 0
        For X = 1 To HeightData.SizeX - 2 Step 2
            Mean = (HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 3
            HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
        Next X
        'left side points
        X = 0
        For Y = 1 To HeightData.SizeY - 2 Step 2
            Mean = (HeightData.Height(Y - 1, X) + HeightData.Height(Y, X + 1) + HeightData.Height(Y + 1, X)) / 3
            HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
        Next Y
        'right side points
        X = HeightData.SizeX - 1
        For Y = 1 To HeightData.SizeY - 2 Step 2
            Mean = (HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y + 1, X)) / 3
            HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
        Next Y
        'bottom side points
        Y = HeightData.SizeY - 1
        For X = 1 To HeightData.SizeX - 2 Step 2
            Mean = (HeightData.Height(Y - 1, X) + HeightData.Height(Y, X - 1) + HeightData.Height(Y, X + 1)) / 3
            HeightData.Height(Y, X) = Mean + Rnd() * Variation - VariationHalved
        Next X
    End Sub

    Sub MinMaxGet(ByRef MinMax_Output As sMinMax)

        Dim HeightMin As Long
        Dim HeightMax As Long
        Dim lngTemp As Long
        Dim Y As Integer
        Dim X As Integer

        If Not (HeightData.SizeY = 0 Or HeightData.SizeX = 0) Then
            HeightMin = HeightData.Height(0, 0)
            HeightMax = HeightData.Height(0, 0)
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    lngTemp = HeightData.Height(Y, X)
                    If lngTemp < HeightMin Then HeightMin = lngTemp
                    If lngTemp > HeightMax Then HeightMax = lngTemp
                Next X
            Next Y
        End If
        MinMax_Output.Min = HeightMin
        MinMax_Output.Max = HeightMax
    End Sub

    Sub Smooth2(ByVal Source As clsHeightmap, ByVal Radius As Integer, ByVal Ratio As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim Y2 As Integer
        Dim X2 As Integer
        Dim HorzRadius As Double
        Dim Samples As Double
        Dim Y3 As Integer
        Dim X3 As Integer
        Dim Y4 As Integer
        Dim X4 As Integer
        Dim DistFactor As Double
        Dim XInt1() As Integer
        Dim XInt2() As Integer
        Dim CircDist(,) As Double
        Dim CircDistFactor(,) As Double
        Dim TempHeight As Long
        Dim AntiRatio As Double = 1.0# - Ratio

        ReDim XInt1(2 * Radius)
        ReDim XInt2(2 * Radius)
        For Y2 = -Radius To Radius
            HorzRadius = Math.Sqrt(Radius * Radius - Y2 * Y2)
            XInt1(Y2 + Radius) = Int(-HorzRadius)
            XInt2(Y2 + Radius) = -Int(-HorzRadius)
        Next Y2
        ReDim CircDist(2 * Radius, 2 * Radius)
        ReDim CircDistFactor(2 * Radius, 2 * Radius)
        For Y2 = -Radius To Radius
            For X2 = XInt1(Y2 + Radius) To XInt2(Y2 + Radius)
                CircDist(Y2 + Radius, X2 + Radius) = Math.Sqrt(Y2 * Y2 + X2 * X2)
                If CircDist(Y2 + Radius, X2 + Radius) <= Radius Then
                    CircDistFactor(Y2 + Radius, X2 + Radius) = 1.0# - CircDist(Y2 + Radius, X2 + Radius) / Radius
                Else
                    CircDistFactor(Y2 + Radius, X2 + Radius) = -1.0#
                End If
            Next X2
        Next Y2
        SizeCopy(Source)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                TempHeight = 0
                Samples = 0
                For Y2 = -Radius To Radius
                    Y4 = Y2 + Radius
                    Y3 = Y + Y2
                    If Y3 >= 0 And Y3 <= Source.HeightData.SizeY - 1 Then
                        For X2 = XInt1(Y4) To XInt2(Y4)
                            X4 = X2 + Radius
                            X3 = X + X2
                            If X3 >= 0 And X3 <= Source.HeightData.SizeX - 1 Then
                                DistFactor = CircDistFactor(Y4, X4)
                                If DistFactor > 0 Then
                                    TempHeight = TempHeight + Source.HeightData.Height(Y3, X3)
                                    Samples = Samples + 1
                                End If
                            End If
                        Next X2
                    End If
                Next Y2
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X) * AntiRatio + TempHeight / Samples * Ratio
            Next X
        Next Y
    End Sub

    Sub Copy(ByVal Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer

        HeightScale = Source.HeightScale
        SizeCopy(Source)
        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X)
            Next
        Next
    End Sub

    Function IsSizeSame(ByVal Source As clsHeightmap) As Boolean

        IsSizeSame = ((HeightData.SizeX = Source.HeightData.SizeX) And (HeightData.SizeY = Source.HeightData.SizeY))
    End Function

    Sub Multiply2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = SourceA.HeightData.Height(Y, X) * SourceA.HeightScale * SourceB.HeightData.Height(Y, X) * SourceB.HeightScale / HeightScale
            Next X
        Next Y
    End Sub

    Sub Multiply(ByVal Source As clsHeightmap, ByVal Multiplier As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale * Multiplier / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X) * dblTemp
            Next X
        Next Y
    End Sub

    Sub Divide2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = SourceA.HeightScale / (SourceB.HeightScale * HeightScale)

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = SourceA.HeightData.Height(Y, X) / SourceB.HeightData.Height(Y, X) * dblTemp
            Next X
        Next Y
    End Sub

    Sub Divide(ByVal Source As clsHeightmap, ByVal Denominator As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / (Denominator * HeightScale)

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X) * dblTemp
            Next X
        Next Y
    End Sub

    Sub Intervalise(ByVal Source As clsHeightmap, ByVal Interval As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / Interval
        Dim dblTemp2 As Double = Interval / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = Int(Source.HeightData.Height(Y, X) * dblTemp) * dblTemp2
            Next X
        Next Y
    End Sub

    Sub Add2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double = SourceA.HeightScale / HeightScale
        Dim dblTempB As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = SourceA.HeightData.Height(Y, X) * dblTempA + SourceB.HeightData.Height(Y, X) * dblTempB
            Next X
        Next Y
    End Sub

    Sub Add(ByVal Source As clsHeightmap, ByVal Amount As Double)
        Dim Y As Integer
        Dim X As Integer

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = (Source.HeightData.Height(Y, X) * Source.HeightScale + Amount) / HeightScale
            Next X
        Next Y
    End Sub

    Sub Subtract2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double = SourceA.HeightScale / HeightScale
        Dim dblTempB As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = SourceA.HeightData.Height(Y, X) * dblTempA - SourceB.HeightData.Height(Y, X) * dblTempB
            Next X
        Next Y
    End Sub

    Sub Subtract(ByVal Source As clsHeightmap, ByVal Amount As Double)
        Dim Y As Integer
        Dim X As Integer

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = (Source.HeightData.Height(Y, X) * Source.HeightScale - Amount) / HeightScale
            Next X
        Next Y
    End Sub

    Sub Highest2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double
        Dim dblTempB As Double
        Dim dblTempC As Double = SourceA.HeightScale / HeightScale
        Dim dblTempD As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTempA = SourceA.HeightData.Height(Y, X) * dblTempC
                dblTempB = SourceB.HeightData.Height(Y, X) * dblTempD
                If dblTempA >= dblTempB Then
                    HeightData.Height(Y, X) = dblTempA
                Else
                    HeightData.Height(Y, X) = dblTempB
                End If
            Next X
        Next Y
    End Sub

    Sub Highest(ByVal Source As clsHeightmap, ByVal Value As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / HeightScale
        Dim dblTemp2 As Double = Value / HeightScale
        Dim dblTemp3 As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp3 = Source.HeightData.Height(Y, X) * dblTemp
                If dblTemp3 >= dblTemp2 Then
                    HeightData.Height(Y, X) = dblTemp3
                Else
                    HeightData.Height(Y, X) = dblTemp2
                End If
            Next X
        Next Y
    End Sub

    Sub Lowest2(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTempA As Double
        Dim dblTempB As Double
        Dim dblTempC As Double = SourceA.HeightScale / HeightScale
        Dim dblTempD As Double = SourceB.HeightScale / HeightScale

        If Not SourceA.IsSizeSame(SourceB) Then Stop
        SizeCopy(SourceA)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTempA = SourceA.HeightData.Height(Y, X) * dblTempC
                dblTempB = SourceB.HeightData.Height(Y, X) * dblTempD
                If dblTempA <= dblTempB Then
                    HeightData.Height(Y, X) = dblTempA
                Else
                    HeightData.Height(Y, X) = dblTempB
                End If
            Next X
        Next Y
    End Sub

    Sub Lowest(ByVal Source As clsHeightmap, ByVal Value As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = Source.HeightScale / HeightScale
        Dim dblTemp2 As Double = Value / HeightScale
        Dim dblTemp3 As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp3 = Source.HeightData.Height(Y, X) * dblTemp
                If dblTemp3 <= dblTemp2 Then
                    HeightData.Height(Y, X) = dblTemp3
                Else
                    HeightData.Height(Y, X) = dblTemp2
                End If
            Next X
        Next Y
    End Sub

    Sub Swap3(ByVal SourceA As clsHeightmap, ByVal SourceB As clsHeightmap, ByVal Swapper As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim Ratio As Double

        If Not (Swapper.IsSizeSame(SourceA) And Swapper.IsSizeSame(SourceB)) Then
            Stop
        End If
        SizeCopy(Swapper)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                Ratio = Swapper.HeightData.Height(Y, X) * Swapper.HeightScale
                HeightData.Height(Y, X) = (SourceA.HeightData.Height(Y, X) * SourceA.HeightScale * (1.0# - Ratio) + SourceB.HeightData.Height(Y, X) * Ratio * SourceB.HeightScale) / HeightScale
            Next X
        Next Y
    End Sub

    Sub Clamp(ByVal Source As clsHeightmap, ByVal HeightMin As Double, ByVal HeightMax As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                dblTemp = Source.HeightData.Height(Y, X) * Source.HeightScale
                If dblTemp < HeightMin Then
                    HeightData.Height(Y, X) = HeightMin / HeightScale
                ElseIf dblTemp > HeightMax Then
                    HeightData.Height(Y, X) = HeightMax / HeightScale
                Else
                    HeightData.Height(Y, X) = dblTemp / HeightScale
                End If
            Next X
        Next Y
    End Sub

    Sub Invert(ByVal Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim dblTemp As Double = -Source.HeightScale / HeightScale

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = Source.HeightData.Height(Y, X) * dblTemp
            Next X
        Next Y
    End Sub

    Sub WaveLow(ByVal Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim HeightRange As Long
        Dim HeightMin As Long
        Dim MinMax As sMinMax = New sMinMax

        Source.MinMaxGet(MinMax)
        HeightRange = MinMax.Max - MinMax.Min
        HeightMin = MinMax.Min

        If HeightRange = 0.0# Then Exit Sub

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = ((1.0# - Math.Sin((1.0# - (Source.HeightData.Height(Y, X) - HeightMin) / HeightRange) * RadOf90Deg)) * HeightRange + HeightMin) * Source.HeightScale / HeightScale
            Next X
        Next Y
    End Sub

    Sub WaveHigh(ByVal Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim HeightRange As Long
        Dim HeightMin As Long
        Dim MinMax As sMinMax = New sMinMax

        Source.MinMaxGet(MinMax)
        HeightRange = MinMax.Max - MinMax.Min
        HeightMin = MinMax.Min

        If HeightRange = 0.0# Then Exit Sub

        SizeCopy(Source)
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = (Math.Sin((Source.HeightData.Height(Y, X) - HeightMin) / HeightRange * RadOf90Deg) * HeightRange + HeightMin) * Source.HeightScale / HeightScale
            Next X
        Next Y
    End Sub

    Sub Rescale(ByVal Source As clsHeightmap, ByVal HeightMin As Double, ByVal HeightMax As Double)
        Dim Y As Integer
        Dim X As Integer
        Dim MinMax As sMinMax = New sMinMax

        Source.MinMaxGet(MinMax)

        Dim HeightRange As Long
        Dim Offset As Long
        Dim Ratio As Double
        Dim lngTemp As Long

        SizeCopy(Source)
        HeightRange = MinMax.Max - MinMax.Min
        Offset = 0L - MinMax.Min
        If HeightRange > 0L Then
            Ratio = (HeightMax - HeightMin) / (HeightRange * HeightScale)
            lngTemp = HeightMin / HeightScale
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    HeightData.Height(Y, X) = lngTemp + (Offset + Source.HeightData.Height(Y, X)) * Ratio
                Next X
            Next Y
        Else
            lngTemp = (HeightMin + HeightMax) / 2.0#
            For Y = 0 To HeightData.SizeY - 1
                For X = 0 To HeightData.SizeX - 1
                    HeightData.Height(Y, X) = lngTemp
                Next X
            Next Y
        End If
    End Sub

    Sub ShiftToZero(ByVal Source As clsHeightmap)
        Dim Y As Integer
        Dim X As Integer
        Dim MinMax As sMinMax = New sMinMax
        Dim dblTemp As Double = Source.HeightScale / HeightScale

        Source.MinMaxGet(MinMax)

        Dim Offset As Long
        SizeCopy(Source)
        Offset = 0L - MinMax.Min
        For Y = 0 To HeightData.SizeY - 1
            For X = 0 To HeightData.SizeX - 1
                HeightData.Height(Y, X) = (Offset + Source.HeightData.Height(Y, X)) * dblTemp
            Next X
        Next Y
    End Sub

    Sub Resize(ByVal Source As clsHeightmap, ByVal OffsetY As Integer, ByVal OffsetX As Integer, ByVal SizeY As Integer, ByVal SizeX As Integer)
        Dim StartY As Integer
        Dim StartX As Integer
        Dim EndY As Integer
        Dim EndX As Integer
        Dim Y As Integer
        Dim X As Integer

        Blank(SizeY, SizeX)
        StartX = Math.Max(0 - OffsetX, 0)
        StartY = Math.Max(0 - OffsetY, 0)
        EndX = Math.Min(Source.HeightData.SizeX - OffsetX, HeightData.SizeX) - 1
        EndY = Math.Min(Source.HeightData.SizeY - OffsetY, HeightData.SizeY) - 1
        For Y = StartY To EndY
            For X = StartX To EndX
                HeightData.Height(Y, X) = Source.HeightData.Height(OffsetY + Y, OffsetX + X)
            Next X
        Next Y
    End Sub

    Sub SizeCopy(ByVal Source As clsHeightmap)

        HeightData.SizeX = Source.HeightData.SizeX
        HeightData.SizeY = Source.HeightData.SizeY
        ReDim HeightData.Height(HeightData.SizeY - 1, HeightData.SizeX - 1)
    End Sub

    Sub Insert(ByVal Source As clsHeightmap, ByVal Y1 As Integer, ByVal X1 As Integer)
        Dim Y As Integer
        Dim X As Integer

        For Y = 0 To Source.HeightData.SizeY - 1
            For X = 0 To Source.HeightData.SizeX - 1
                HeightData.Height(Y1 + Y, X1 + X) = Source.HeightData.Height(Y, X)
            Next X
        Next Y
    End Sub

    Function Load_Image(ByVal Path As String) As sResult
        Load_Image.Success = False
        Load_Image.Problem = ""

        Try

            Dim HeightmapBitmap As New clsFileBitmap()

            HeightmapBitmap.Load(Path)

            Blank(HeightmapBitmap.CurrentBitmap.Height, HeightmapBitmap.CurrentBitmap.Width)
            Dim Y As Integer
            Dim X As Integer
            For Y = 0 To HeightmapBitmap.CurrentBitmap.Height - 1
                For X = 0 To HeightmapBitmap.CurrentBitmap.Width - 1
                    With HeightmapBitmap.CurrentBitmap.GetPixel(X, Y)
                        HeightData.Height(Y, X) = (CShort(.R) + .G + .B) / (3.0# * HeightScale)
                    End With
                Next
            Next

        Catch ex As Exception
            Load_Image.Problem = ex.Message
            Exit Function
        End Try

        Load_Image.Success = True
    End Function

    Friend Sub GenerateNewOfSize(ByVal Final_SizeY As Integer, ByVal Final_SizeX As Integer, ByVal Scale As Single, ByVal HeightMultiplier As Double)
        Dim Inflations As Integer
        Dim SizeY As Integer
        Dim SizeX As Integer
        Dim Log2 As Double
        Dim intTemp As Integer
        Dim hmTemp As New clsHeightmap
        Dim Ratio As Double

        Log2 = Math.Log(2.0#)
        If Final_SizeX > Final_SizeY Then
            Inflations = CInt(Math.Ceiling(Math.Log(Final_SizeX - 1) / Log2))
        Else
            Inflations = CInt(Math.Ceiling(Math.Log(Final_SizeY - 1) / Log2))
        End If
        Inflations = Math.Ceiling(Scale)
        If Inflations < 0 Then Stop
        Ratio = 2.0# ^ Scale / 2.0# ^ Inflations
        intTemp = CInt(2.0# ^ Inflations)
        SizeX = CInt(Math.Ceiling((Final_SizeX / Ratio - 1) / intTemp)) + 1
        SizeY = CInt(Math.Ceiling((Final_SizeY / Ratio - 1) / intTemp)) + 1

        GenerateNew(SizeY, SizeX, Inflations, 1.0#, HeightMultiplier)
        If Inflations > Scale Then
            hmTemp.Stretch(Me, HeightData.SizeX * Ratio, HeightData.SizeY * Ratio)
            HeightData = hmTemp.HeightData
            hmTemp.HeightData = New clsHeightData
        End If
        If HeightData.SizeX <> Final_SizeX Or HeightData.SizeY <> Final_SizeY Then
            'If HeightData.SizeX / Final_SizeX > HeightData.SizeY / Final_SizeY Then
            '    hmTemp.Resize(Me, 0, 0, HeightData.SizeY, Final_SizeX * HeightData.SizeY / Final_SizeY)
            'Else
            '    hmTemp.Resize(Me, 0, 0, Final_SizeY * HeightData.SizeX / Final_SizeX, HeightData.SizeX)
            'End If
            'StretchPixelated(hmTemp, Final_SizeX, Final_SizeY)
            hmTemp.Resize(Me, 0, 0, Final_SizeY, Final_SizeX)
            HeightData = hmTemp.HeightData
        End If
    End Sub

    Friend Sub Stretch(ByVal hmSource As clsHeightmap, ByVal SizeX As Integer, ByVal SizeY As Integer)
        Dim OldSizeY As Integer
        Dim OldSizeX As Integer
        Dim New_Per_OldY As Single
        Dim New_Per_OldX As Single
        Dim OldPixStartY As Single
        Dim OldPixStartX As Single
        Dim OldPixEndY As Single
        Dim OldPixEndX As Single
        Dim Ratio As Single
        Dim NewPixelY As Integer
        Dim NewPixelX As Integer
        Dim OldPixelY As Integer
        Dim OldPixelX As Integer
        Dim YTemp As Single
        Dim XTemp As Single
        Dim Temp As Single = hmSource.HeightScale / HeightScale

        OldSizeY = hmSource.HeightData.SizeY
        OldSizeX = hmSource.HeightData.SizeX

        Blank(SizeY, SizeX)
        'new ratios convert original image positions into new image positions
        New_Per_OldY = SizeY / OldSizeY
        New_Per_OldX = SizeX / OldSizeX
        'cycles through each pixel in the new image
        For OldPixelY = 0 To OldSizeY - 1
            For OldPixelX = 0 To OldSizeX - 1
                'find where the old pixel goes on the new image
                OldPixStartY = OldPixelY * New_Per_OldY
                OldPixStartX = OldPixelX * New_Per_OldX
                OldPixEndY = (OldPixelY + 1) * New_Per_OldY
                OldPixEndX = (OldPixelX + 1) * New_Per_OldX
                'cycles through each new image pixel that is to be influenced
                For NewPixelY = Int(OldPixStartY) To -Int(-OldPixEndY)
                    If NewPixelY >= SizeY Then Exit For
                    For NewPixelX = Int(OldPixStartX) To -Int(-OldPixEndX)
                        If NewPixelX >= SizeX Then Exit For
                        'ensure that the original pixel imposes on the new pixel
                        If Not (OldPixEndY > NewPixelY And OldPixStartY < NewPixelY + 1 And OldPixEndX > NewPixelX And OldPixStartX < NewPixelX + 1) Then
                            'Stop
                        Else
                            'measure the amount of original pixel in the new pixel
                            YTemp = 1.0# : XTemp = 1.0#
                            If OldPixStartY > NewPixelY Then YTemp = YTemp - (OldPixStartY - NewPixelY)
                            If OldPixStartX > NewPixelX Then XTemp = XTemp - (OldPixStartX - NewPixelX)
                            If OldPixEndY < NewPixelY + 1 Then YTemp = YTemp - ((NewPixelY + 1) - OldPixEndY)
                            If OldPixEndX < NewPixelX + 1 Then XTemp = XTemp - ((NewPixelX + 1) - OldPixEndX)
                            Ratio = YTemp * XTemp
                            'add the neccessary fraction of the original pixel's color into the new pixel
                            HeightData.Height(NewPixelY, NewPixelX) = HeightData.Height(NewPixelY, NewPixelX) + hmSource.HeightData.Height(OldPixelY, OldPixelX) * Ratio * Temp
                        End If
                    Next NewPixelX
                Next NewPixelY
            Next OldPixelX
        Next OldPixelY
    End Sub
End Class