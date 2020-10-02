Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports PrimoSoftware.DVDBuilder.VR

Namespace CameraToDVD

    Friend Class MuxedStreamCB
        Inherits System.IO.Stream
#Region "System.IO.Stream"
        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property Length() As Long
            Get
                Throw New NotImplementedException()
            End Get
        End Property
        Public Overrides Property Position() As Long
            Get
                Throw New NotImplementedException()
            End Get
            Set(ByVal value As Long)
                Throw New NotImplementedException()
            End Set
        End Property

        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            If (offset = 0) AndAlso (buffer.Length = count) Then
                WriteData(buffer)
            Else
                Dim b(count - 1) As Byte
                Array.Copy(buffer, offset, b, 0, count)
                WriteData(b)
            End If
        End Sub

        Public Overrides Sub Flush()
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
            Throw New NotImplementedException()
        End Function

        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw New NotImplementedException()
        End Function
#End Region

        Public Sub New()

        End Sub


        Public Function WriteData(ByVal buffer() As Byte) As Boolean
            If (Not bProcess) Then
                Return False
            End If

            writeCounter += 1
            If writeCounter Mod 100 = 0 Then
                Dim msg As String = String.Format("MuxedStreamCB::WriteData {0}", writeCounter)
                System.Diagnostics.Trace.WriteLine(msg)
            End If

            Dim fileResult As Boolean = True
            If file IsNot Nothing Then
                Try
                    file.Write(buffer, 0, buffer.Length)
                Catch
                    fileResult = False
                End Try
            End If

            Dim dvdResult As Boolean = True
            Dim stopReason As Integer = -1 ' -1:general failure -2:out of space
            If DvdRecorder IsNot Nothing AndAlso (Not DvdRecorder.Write(buffer)) Then
                System.Diagnostics.Trace.WriteLine("VideoRecorder.Write() FAILED")

                Dim activeCount, failedCount, noSpaceCount As Integer
                Util.CheckRecorderDevices(DvdRecorder, activeCount, failedCount, noSpaceCount)

                System.Diagnostics.Debug.Assert((activeCount + failedCount + noSpaceCount) = DvdRecorder.Devices.Count)

                If activeCount = 0 Then
                    dvdResult = False
                    If failedCount = 0 AndAlso noSpaceCount > 0 Then
                        stopReason = -2
                    End If
                End If
            End If

            If fileResult AndAlso dvdResult Then
                Return True
            End If

            'STOP_CAPTURE
            bProcess = False
            System.Diagnostics.Trace.WriteLine("WriteData: Before Post STOP_CAPTURE")
            WinAPI.PostMessage(MainWindow, Util.WM_STOP_CAPTURE, New IntPtr(stopReason), IntPtr.Zero)
            System.Diagnostics.Trace.WriteLine("WriteData: After Post STOP_CAPTURE")

            Return False
        End Function

        Public Function SetOutputFile(ByVal filename As String) As Boolean
            If file IsNot Nothing Then
                file.Close()
                file = Nothing
            End If

            If filename Is Nothing Then
                Me.filename = Nothing
                Return True
            End If

            Try
                file = New FileStream(filename, FileMode.Create, FileAccess.Write)
            Catch
                Return False
            End Try

            Me.filename = filename
            Return True
        End Function

        Public Sub Reset()
            If file IsNot Nothing Then
                file.Close()
                file = Nothing
            End If

            writeCounter = 0
            DvdRecorder = Nothing
            bProcess = True
        End Sub

        Public DvdRecorder As PrimoSoftware.DVDBuilder.VR.VideoRecorder

        Private filename As String
        Private file As System.IO.FileStream
        Private writeCounter As Integer
        Public MainWindow As IntPtr
        Protected bProcess As Boolean = True
    End Class
End Namespace
