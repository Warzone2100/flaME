Option Strict On

Namespace My

    Partial Friend Class MyApplication
        
        Public Sub New()
            MyBase.New(Global.Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            Me.IsSingleInstance = false
            Me.EnableVisualStyles = true
            Me.SaveMySettingsOnExit = false
            Me.ShutDownStyle = Global.Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses
        End Sub
        
        Protected Overrides Sub OnCreateMainForm()
            Me.MainForm = frmMainInstance
        End Sub
    End Class
End Namespace