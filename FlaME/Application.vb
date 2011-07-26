Option Strict On

Namespace My

    Partial Friend Class MyApplication
        
        Public Sub New()
            MyBase.New(Global.Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            Me.IsSingleInstance = False
            Me.EnableVisualStyles = True
            Me.SaveMySettingsOnExit = False
            Me.ShutdownStyle = Global.Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses
        End Sub
        
        Protected Overrides Sub OnCreateMainForm()
            Me.MainForm = frmMainInstance
        End Sub
    End Class
End Namespace