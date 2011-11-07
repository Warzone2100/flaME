Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Public Sub FirstInstanceStart(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            If e.CommandLine.Count = 0 Then
                Exit Sub
            End If

            Dim A As Integer
            Dim OldCount As Integer

            OldCount = CommandLinePaths.GetUpperBound(0) + 1
            ReDim Preserve CommandLinePaths(OldCount + e.CommandLine.Count - 1)
            For A = 0 To e.CommandLine.Count - 1
                CommandLinePaths(OldCount + A) = e.CommandLine(A)
            Next
        End Sub

        Public Sub NextInstanceStart(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs) Handles Me.StartupNextInstance

            If e.CommandLine.Count = 0 Then
                Exit Sub
            End If

            If ProgramInitialized Then
                Dim LoadResult As New clsResult
                Dim A As Integer
                For A = 0 To e.CommandLine.Count - 1
                    LoadResult.Append(frmMainInstance.LoadMap(e.CommandLine.Item(A)), New sSplitPath(e.CommandLine.Item(A)).FileTitle & ": ")
                Next
                ShowWarnings(LoadResult, "Load Command-line Map")
            Else
                Dim A As Integer
                Dim OldCount As Integer
                OldCount = CommandLinePaths.GetUpperBound(0) + 1
                ReDim Preserve CommandLinePaths(OldCount + e.CommandLine.Count - 1)
                For A = 0 To e.CommandLine.Count - 1
                    CommandLinePaths(OldCount + A) = e.CommandLine.Item(A)
                Next
            End If
        End Sub

    End Class

End Namespace