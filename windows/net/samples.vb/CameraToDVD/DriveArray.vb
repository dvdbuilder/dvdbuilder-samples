Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports System.Diagnostics
Imports PrimoSoftware.DVDBuilder.VR

Namespace CameraToDVD
    Friend Class DriveItem
        Public Device As VRDevice
        Public IsInitialized As Boolean = False
        Public Result As Boolean = False

        ' attributes
        Public IsBlank As Boolean = True
        Public IsVideo As Boolean = False
        Public VolumeLabel As String = String.Empty

        Public Sub New(ByVal device As VRDevice)
            Debug.Assert(device IsNot Nothing)
            Me.Device = device
        End Sub
    End Class

    Friend Class DriveArray
        Private Delegate Sub DeviceCaller(ByVal drive As DriveItem)
        Private Sub InitializeDevice(ByVal drive As DriveItem)
            drive.Result = drive.Device.Initialize()
            drive.IsInitialized = drive.Result
        End Sub

        Private Sub EraseDevice(ByVal drive As DriveItem)
            drive.Result = drive.Device.EraseMedia()
        End Sub

        Private Sub NotifyDeviceChanges(ByVal drive As DriveItem)
            drive.Result = drive.Device.NotifyOSFileSystemChanged()
        End Sub

        Private Sub QueryDevice(ByVal drive As DriveItem)
            Dim device As VRDevice = drive.Device

            If drive.IsInitialized Then
                drive.IsBlank = device.MediaIsBlank
                drive.IsVideo = False
                drive.VolumeLabel = String.Empty

                If device.Type = VRDeviceType.OpticalDisc Then
                    Dim config As OpticalDiscDeviceConfig = CType(device.Config, OpticalDiscDeviceConfig)
                    If config.VolumeLabel IsNot Nothing Then
                        drive.VolumeLabel = config.VolumeLabel
                    Else
                        drive.VolumeLabel = String.Empty
                    End If
                End If

                Dim recorder As New VideoRecorder()
                Debug.Assert(recorder IsNot Nothing)

                recorder.Devices.Add(device)
                Dim titles As IList(Of Title) = recorder.GetTitles(0)
                If titles IsNot Nothing Then
                    If titles.Count > 0 Then
                        drive.IsVideo = True
                    End If
                End If

                recorder.Dispose()
            End If
        End Sub

        Public Items As New List(Of DriveItem)(1)
        Public Sub Dispose()
            For Each drive As DriveItem In Items
                If drive.Device IsNot Nothing Then
                    drive.Device.Dispose()
                End If
            Next drive
            Items.Clear()
        End Sub

        Public Function Initialize() As Boolean
            Return Parallel(AddressOf InitializeDevice)
        End Function

        Public Function [Erase]() As Boolean
            Return Parallel(AddressOf EraseDevice)
        End Function

        Public Function Query() As Boolean
            Return Parallel(AddressOf QueryDevice)
        End Function

        Public Function NotifyChanges() As Boolean
            Return Parallel(AddressOf NotifyDeviceChanges)
        End Function

        Private Function Parallel(ByVal caller As DeviceCaller) As Boolean
            If Items.Count = 0 Then
                Return False
            End If

            Dim asyncResults(Items.Count - 1) As IAsyncResult

            Dim i As Integer = 0
            For Each drive As DriveItem In Items
                asyncResults(i) = caller.BeginInvoke(drive, Nothing, Nothing)
                i += 1
            Next drive

            Dim result As Boolean = True
            For Each asyncResult As IAsyncResult In asyncResults
                caller.EndInvoke(asyncResult)
            Next asyncResult

            For Each drive As DriveItem In Items
                If (Not drive.Result) Then
                    result = False
                End If
            Next drive

            Return result
        End Function

        Private Function IsErasable(ByVal device As VRDevice) As Boolean
            Dim rw As Boolean = device.MediaIsReWritable
            If rw AndAlso device.Error.Facility = PrimoSoftware.DVDBuilder.ErrorFacility.Success Then
                Dim blank As Boolean = device.MediaIsBlank
                If (Not blank) AndAlso (device.Error.Facility = PrimoSoftware.DVDBuilder.ErrorFacility.Success) Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Function GetErasableCount() As Integer
            Dim erasable As Integer = 0

            For Each drive As DriveItem In Items
                If IsErasable(drive.Device) Then
                    erasable += 1
                End If
            Next drive

            Return erasable
        End Function
    End Class
End Namespace
