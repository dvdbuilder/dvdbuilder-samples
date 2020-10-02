Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DirectShowLib
Imports PrimoSoftware.DVDBuilder.VR
Imports System.Diagnostics

Namespace CameraToDVD
    Friend Class MediaState
        Public Sub New()
        End Sub

        Public mediaControl As IMediaControl
        'public IGraphBuilder graph;
        Public graph As IFilterGraph2
        Public captureGraph As ICaptureGraphBuilder2

        Public audioInput As IBaseFilter
        Public videoInput As IBaseFilter
        Public smartTee As IBaseFilter
        Public previewRenderer As IBaseFilter

        Public audioSampleGrabber As IBaseFilter
        Public videoSampleGrabber As IBaseFilter
        Public audioGrabber As ISampleGrabber
        Public videoGrabber As ISampleGrabber

        Public audioNullRenderer As IBaseFilter
        Public videoNullRenderer As IBaseFilter

        Public rot As DsROTEntry

        Public videoType As New AMMediaType()

        Public droppedFrames As IAMDroppedFrames

        Public mpeg2Enc As PrimoSoftware.AVBlocks.Transcoder
        Public dvdRecorder As VideoRecorder

        Public Sub Reset(ByVal full As Boolean)
            '            
            '             * NOTE:
            '             * Interfaces obtained with a cast (as) should not be released with Marshal.ReleaseComObject.
            '             

            If mediaControl IsNot Nothing Then
                mediaControl.StopWhenReady()
                mediaControl = Nothing
            End If

            If full AndAlso rot IsNot Nothing Then
                rot.Dispose()
                rot = Nothing
            End If

            DsUtils.FreeAMMediaType(videoType)

            Util.DisposeObject(mpeg2Enc)

            If previewRenderer IsNot Nothing Then
                Dim window As IVideoWindow = TryCast(graph, IVideoWindow)
                window.put_Owner(IntPtr.Zero)
                window.put_Visible(OABool.False)
            End If

            If videoInput IsNot Nothing Then
                Util.NukeDownstream(graph, videoInput)
            End If

            If audioInput IsNot Nothing Then
                Util.NukeDownstream(graph, audioInput)
            End If

            audioNullRenderer = Nothing
            videoNullRenderer = Nothing
            audioGrabber = Nothing
            videoGrabber = Nothing
            audioSampleGrabber = Nothing
            videoSampleGrabber = Nothing
            droppedFrames = Nothing
            previewRenderer = Nothing
            smartTee = Nothing

            If full Then
                Util.ReleaseComObject(videoInput)
                Util.ReleaseComObject(audioInput)
                Util.ReleaseComObject(graph)
                Util.ReleaseComObject(captureGraph)
            End If

            If dvdRecorder IsNot Nothing Then
                Dim res As Boolean = dvdRecorder.Stop()
                If (Not res) Then
                    Trace.WriteLine("VideoRecorder.Stop() failed")

                    Util.CheckRecorderDevices(dvdRecorder)
                End If

                For i As Integer = 0 To dvdRecorder.Devices.Count - 1
                    If dvdRecorder.GetDeviceError(i).Facility = PrimoSoftware.DVDBuilder.ErrorFacility.Success Then
                        dvdRecorder.Devices(i).NotifyOSFileSystemChanged()
                    End If
                    dvdRecorder.Devices(i).Dispose()
                Next i
            End If

            Util.DisposeObject(dvdRecorder)
        End Sub


    End Class
End Namespace
