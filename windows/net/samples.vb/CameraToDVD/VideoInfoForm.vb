Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace CameraToDVD
    Partial Public Class VideoInfoForm
        Inherits Form
        Public Sub New()
            InitializeComponent()
        End Sub

        Public Info As String

        Private Sub VideoInfoForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            txtInfo.Text = Info
        End Sub

        Private Sub VideoInfoForm_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
            txtInfo.Select(Info.Length, 0)
        End Sub

        Private Sub VideoInfoForm_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles MyBase.KeyDown
            If e.KeyCode = Keys.Escape Then
                Close()
            End If
        End Sub
    End Class
End Namespace
