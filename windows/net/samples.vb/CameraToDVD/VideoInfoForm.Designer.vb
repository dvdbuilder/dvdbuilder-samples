Imports Microsoft.VisualBasic
Imports System
Namespace CameraToDVD
    Partial Public Class VideoInfoForm
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.txtInfo = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            ' 
            ' txtInfo
            ' 
            Me.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill
            Me.txtInfo.Location = New System.Drawing.Point(0, 0)
            Me.txtInfo.Multiline = True
            Me.txtInfo.Name = "txtInfo"
            Me.txtInfo.ReadOnly = True
            Me.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.txtInfo.Size = New System.Drawing.Size(325, 339)
            Me.txtInfo.TabIndex = 0
            ' 
            ' VideoInfoForm
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(325, 339)
            Me.Controls.Add(Me.txtInfo)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "VideoInfoForm"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Video Info"
            '			Me.Load += New System.EventHandler(Me.VideoInfoForm_Load);
            '			Me.Shown += New System.EventHandler(Me.VideoInfoForm_Shown);
            '			Me.KeyDown += New System.Windows.Forms.KeyEventHandler(Me.VideoInfoForm_KeyDown);
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Private txtInfo As System.Windows.Forms.TextBox
    End Class
End Namespace