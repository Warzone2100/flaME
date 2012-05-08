<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmData
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnContinue = New System.Windows.Forms.Button()
        Me.btnQuit = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnContinue
        '
        Me.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnContinue.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnContinue.Location = New System.Drawing.Point(350, 6)
        Me.btnContinue.Margin = New System.Windows.Forms.Padding(6)
        Me.btnContinue.Name = "btnContinue"
        Me.btnContinue.Size = New System.Drawing.Size(148, 36)
        Me.btnContinue.TabIndex = 0
        Me.btnContinue.Text = "Continue"
        Me.btnContinue.UseCompatibleTextRendering = True
        Me.btnContinue.UseVisualStyleBackColor = True
        '
        'btnQuit
        '
        Me.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnQuit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnQuit.Location = New System.Drawing.Point(6, 6)
        Me.btnQuit.Margin = New System.Windows.Forms.Padding(6)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(148, 36)
        Me.btnQuit.TabIndex = 1
        Me.btnQuit.Text = "Quit"
        Me.btnQuit.UseCompatibleTextRendering = True
        Me.btnQuit.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel2, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(504, 422)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnQuit, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnContinue, 2, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 374)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(504, 48)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'frmData
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(504, 422)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "frmData"
        Me.Text = "Data Directories"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents btnContinue As System.Windows.Forms.Button
    Public WithEvents btnQuit As System.Windows.Forms.Button
    Public WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
End Class
