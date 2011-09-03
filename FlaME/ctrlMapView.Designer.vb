<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ctrlMapView
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me.ssStatus = New System.Windows.Forms.StatusStrip()
        Me.lblTile = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblVertex = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblPos = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblUndo = New System.Windows.Forms.ToolStripStatusLabel()
        Me.pnlDraw = New System.Windows.Forms.Panel()
        Me.ssStatus.SuspendLayout()
        Me.SuspendLayout()
        '
        'ssStatus
        '
        Me.ssStatus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblTile, Me.lblVertex, Me.lblPos, Me.lblUndo})
        Me.ssStatus.Location = New System.Drawing.Point(0, 392)
        Me.ssStatus.Name = "ssStatus"
        Me.ssStatus.Size = New System.Drawing.Size(1308, 32)
        Me.ssStatus.TabIndex = 0
        Me.ssStatus.Text = "StatusStrip1"
        '
        'lblTile
        '
        Me.lblTile.AutoSize = False
        Me.lblTile.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTile.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblTile.Name = "lblTile"
        Me.lblTile.Size = New System.Drawing.Size(192, 27)
        Me.lblTile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVertex
        '
        Me.lblVertex.AutoSize = False
        Me.lblVertex.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVertex.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblVertex.Name = "lblVertex"
        Me.lblVertex.Size = New System.Drawing.Size(256, 27)
        Me.lblVertex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPos
        '
        Me.lblPos.AutoSize = False
        Me.lblPos.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPos.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.lblPos.Name = "lblPos"
        Me.lblPos.Size = New System.Drawing.Size(320, 27)
        Me.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUndo
        '
        Me.lblUndo.AutoSize = False
        Me.lblUndo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUndo.Name = "lblUndo"
        Me.lblUndo.Size = New System.Drawing.Size(256, 27)
        Me.lblUndo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlDraw
        '
        Me.pnlDraw.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDraw.Location = New System.Drawing.Point(0, 0)
        Me.pnlDraw.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlDraw.Name = "pnlDraw"
        Me.pnlDraw.Size = New System.Drawing.Size(1308, 392)
        Me.pnlDraw.TabIndex = 1
        '
        'ctrlMapView
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.Controls.Add(Me.pnlDraw)
        Me.Controls.Add(Me.ssStatus)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "ctrlMapView"
        Me.Size = New System.Drawing.Size(1308, 424)
        Me.ssStatus.ResumeLayout(False)
        Me.ssStatus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents ssStatus As System.Windows.Forms.StatusStrip
    Public WithEvents lblTile As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblVertex As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents lblPos As System.Windows.Forms.ToolStripStatusLabel
    Public WithEvents pnlDraw As System.Windows.Forms.Panel
    Public WithEvents lblUndo As System.Windows.Forms.ToolStripStatusLabel
End Class
