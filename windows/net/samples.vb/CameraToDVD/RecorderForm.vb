Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.IO
Imports System.Diagnostics
Imports System.Threading

Imports DirectShowLib
Imports PrimoSoftware.DVDBuilder.VR
Imports PrimoSoftware.AVBlocks

Namespace CameraToDVD
    Partial Public Class RecorderForm
        Inherits Form
        Private ms As New MediaState()

        Private bIgnoreDeviceSelection As Boolean
        Private bRecording As Boolean
        Private bCmdRecordBusy As Boolean
        Private Const sStartRecording As String = "Start Recording"
        Private Const sStopRecording As String = "Stop Recording"

        Private statsTimer As System.Windows.Forms.Timer
        Private recStartTime As DateTime
        Private fpsStartTime As DateTime ' current fps
        Private fpsNotDropped As Integer ' current fps
        Private Const MIN_FREE_SPACE As Long = 3 * 1385000 ' bytes, 3 sec video at 1x dvd speed (max bitrate)
        Private m_avgBitrate As Integer ' bits per second

        Private m_videoCB As New VideoGrabberCB("VideoGrabberCB")
        Private m_audioCB As New AudioGrabberCB("AudioGrabberCB")
        Private m_muxedCB As New MuxedStreamCB()
        Private m_dvdPlugin As VRDevicePlugin
        Private m_fsPlugin As VRDevicePlugin
        Private m_selDrives As New List(Of Char)()
        Private m_mainWindow As IntPtr = IntPtr.Zero

        Public Sub New()
            InitializeComponent()
#If WIN64 Then
            Me.Text &= " (64-bit)"
#End If

            EnumInputDev(FilterCategory.AudioInputDevice, listAudioDev)
            EnumInputDev(FilterCategory.VideoInputDevice, listVideoDev)

            statsTimer = New System.Windows.Forms.Timer()
            AddHandler statsTimer.Tick, AddressOf UpdateStats
            statsTimer.Interval = 500

            bIgnoreDeviceSelection = False
            bRecording = False
            cmdRecord.Text = sStartRecording
            txtRecording.Visible = False

            ResetStats()

            Dim h As Integer = previewBox.Size.Height
            Dim w As Integer = previewBox.Size.Width
            previewBox.Size = New Size(w, w * 3 \ 4)
        End Sub

        Private Sub UpdateStats(ByVal sender As Object, ByVal e As EventArgs)
            Dim now As DateTime = DateTime.Now
            Dim rec As TimeSpan = now.Subtract(recStartTime)
            txtRecTime.Text = String.Format("{0}:{1:d2}:{2:d2}", rec.Hours, rec.Minutes, rec.Seconds)

            If (Not cmdSimulate.Checked) Then
                ' format remaining space
                Dim remSpace As Long = ms.dvdRecorder.MediaFreeSpace
                txtRemainingSpace.Text = String.Format("{0:f2} MB", CDbl(remSpace) / (1024 * 1024))

                Dim avgBitrate As Integer = ms.dvdRecorder.AverageBitrate
                If avgBitrate > 0 Then
                    m_avgBitrate = avgBitrate
                End If

                If m_avgBitrate > 0 Then
                    'INSTANT VB NOTE: Embedded comments are not maintained by Instant VB
                    'ORIGINAL LINE: TimeSpan rem = new TimeSpan((long)(remSpace * 8 / m_avgBitrate) * 10000000 /* convert seconds to ticks */);
                    Dim [rem] As New TimeSpan(CLng(Fix(remSpace * 8 \ m_avgBitrate)) * 10000000)
                    txtRemainingTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", [rem].Hours, [rem].Minutes, [rem].Seconds)
                End If
            End If

            If ms.droppedFrames IsNot Nothing Then
                Dim hr As Integer = 0
                Dim dropped As Integer
                hr = ms.droppedFrames.GetNumDropped(dropped)
                If 0 = hr Then
                    txtNumDropped.Text = dropped.ToString()
                End If

                Dim notDropped As Integer
                hr = ms.droppedFrames.GetNumNotDropped(notDropped)

                If 0 = hr Then
                    txtNumNotDropped.Text = notDropped.ToString()
                    If notDropped >= 0 Then
                        Dim averageFPS As Double = CDbl(notDropped) / rec.TotalSeconds
                        txtAverageFPS.Text = averageFPS.ToString("F3")

                        Dim tsfps As TimeSpan = now.Subtract(fpsStartTime)
                        Dim fpsElapsed As Double = tsfps.TotalSeconds
                        If fpsElapsed > 5.0 Then
                            Dim curFPS As Double = CDbl(notDropped - fpsNotDropped) / fpsElapsed
                            txtCurrentFPS.Text = curFPS.ToString("F3")

                            fpsStartTime = now
                            fpsNotDropped = notDropped
                        End If
                    End If
                End If
            End If

            txtACallbacks.Text = m_audioCB.SampleIndex.ToString()
            txtAProcessed.Text = m_audioCB.ProcessedSamples.ToString()
            txtADropped.Text = m_audioCB.DroppedSamples.ToString()

            txtVCallbacks.Text = m_videoCB.SampleIndex.ToString()
            txtVProcessed.Text = m_videoCB.ProcessedSamples.ToString()
            txtVDropped.Text = m_videoCB.DroppedSamples.ToString()

        End Sub

        Private Sub ResetStats()
            Dim sEmpty As String = "--"

            txtRecTime.Text = "--:--:--"
            txtRemainingTime.Text = "--:--:--"
            txtRemainingSpace.Text = "--- MB"

            txtNumDropped.Text = sEmpty
            txtNumNotDropped.Text = sEmpty
            txtAverageFPS.Text = sEmpty
            txtCurrentFPS.Text = sEmpty

            txtACallbacks.Text = sEmpty
            txtADropped.Text = sEmpty
            txtAProcessed.Text = sEmpty

            txtVCallbacks.Text = sEmpty
            txtVDropped.Text = sEmpty
            txtVProcessed.Text = sEmpty
        End Sub

        Private Sub EnumInputDev(ByVal filterCategory As Guid, ByVal list As ComboBox)
            If list Is Nothing Then
                Return
            End If

            Dim devEnum As ICreateDevEnum = Nothing
            Dim enumCat As IEnumMoniker = Nothing
            Dim moniker() As IMoniker = {Nothing}
            Dim propBag As IPropertyBag = Nothing

            Dim hr As Integer = 0
            Try

                devEnum = TryCast(New CreateDevEnum(), ICreateDevEnum)
                If devEnum Is Nothing Then
                    Throw New COMException("Cannot create CLSID_SystemDeviceEnum")
                End If

                ' Obtain enumerator for the capture category.
                hr = devEnum.CreateClassEnumerator(filterCategory, enumCat, 0)
                DsError.ThrowExceptionForHR(hr)

                If enumCat Is Nothing Then
                    MessageBox.Show("No capture devices found")
                    Return
                End If

                ' Enumerate the monikers.
                Dim fetchedCount As New IntPtr(0)
                Do While 0 = enumCat.Next(1, moniker, fetchedCount)

                    Dim bagId As Guid = GetType(IPropertyBag).GUID
                    Dim bagObj As Object = Nothing
                    moniker(0).BindToStorage(Nothing, Nothing, bagId, bagObj)

                    propBag = TryCast(bagObj, IPropertyBag)

                    If propBag IsNot Nothing Then
                        Dim val As Object = Nothing
                        Dim friendlyName As String = Nothing
                        Dim displayName As String = Nothing

                        hr = propBag.Read("FriendlyName", val, Nothing)
                        If hr = 0 Then
                            friendlyName = TryCast(val, String)
                        End If
                        Util.ReleaseComObject(propBag)

                        moniker(0).GetDisplayName(Nothing, Nothing, displayName)

                        ' create an instance of the filter
                        Dim baseFilterId As Guid = GetType(IBaseFilter).GUID
                        Dim filter As Object = Nothing
                        Dim addFilter As Boolean = False
                        Try
                            moniker(0).BindToObject(Nothing, Nothing, baseFilterId, filter)
                            If filter IsNot Nothing Then
                                addFilter = True
                                Util.ReleaseComObject(filter)
                            End If
                        Catch
                            System.Diagnostics.Trace.WriteLine("Cannot use input device " & friendlyName)
                        End Try

                        If addFilter = True AndAlso friendlyName IsNot Nothing AndAlso displayName IsNot Nothing Then
                            Dim fi As New FilterItem(friendlyName, displayName)
                            list.Items.Add(fi)
                        End If
                    End If ' if IPropertyBag

                    Util.ReleaseComObject(moniker(0))
                Loop ' while enum devices
            Catch ex As COMException
                MessageBox.Show(ex.Message)
            Finally
                Util.ReleaseComObject(propBag)
                Util.ReleaseComObject(moniker(0))
                Util.ReleaseComObject(enumCat)
                Util.ReleaseComObject(devEnum)
            End Try

        End Sub ' EnumInputDev

        Private Function InitInputDev(ByVal ms As MediaState, ByVal videoItem As FilterItem, ByVal audioItem As FilterItem) As Integer
            Dim hr As Integer = 0
            ' create Filter Graph Manager
            If ms.graph Is Nothing Then
                'ms.graph = new FilterGraph() as IGraphBuilder;
                ms.graph = TryCast(New FilterGraph(), IFilterGraph2)
                If ms.graph Is Nothing Then
                    Throw New COMException("Cannot create FilterGraph")
                End If

                ms.captureGraph = TryCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
                If ms.captureGraph Is Nothing Then
                    Throw New COMException("Cannot create CaptureGraphBuilder2")
                End If

                hr = ms.captureGraph.SetFiltergraph(ms.graph)
                DsError.ThrowExceptionForHR(hr)
            End If


            If audioItem IsNot Nothing Then
                ' remove the old audio input
                If ms.audioInput IsNot Nothing Then
                    hr = ms.graph.RemoveFilter(ms.audioInput)
                    Util.ReleaseComObject(ms.audioInput)
                    DsError.ThrowExceptionForHR(hr)
                End If

                ' create audio input

                ' using BindToMoniker
                ms.audioInput = TryCast(Marshal.BindToMoniker(audioItem.DisplayName), IBaseFilter)

                ' add audio input to the graph
                hr = ms.graph.AddFilter(ms.audioInput, audioItem.FriendlyName)
                DsError.ThrowExceptionForHR(hr)
            End If


            If videoItem IsNot Nothing Then
                ' remove the old video input
                If ms.videoInput IsNot Nothing Then
                    hr = ms.graph.RemoveFilter(ms.videoInput)
                    Util.ReleaseComObject(ms.videoInput)
                    DsError.ThrowExceptionForHR(hr)
                End If

                ' create video input

                ' Using BindToMoniker
                ms.videoInput = TryCast(Marshal.BindToMoniker(videoItem.DisplayName), IBaseFilter)

                ' add video input to the graph
                hr = ms.graph.AddFilter(ms.videoInput, videoItem.FriendlyName)
                DsError.ThrowExceptionForHR(hr)


            End If


            Return hr
        End Function

        Private Sub RecorderForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                bIgnoreDeviceSelection = True

                If listAudioDev.Items.Count > 0 Then
                    listAudioDev.SelectedIndex = 0
                End If

                If listVideoDev.Items.Count > 0 Then
                    listVideoDev.SelectedIndex = 0
                End If

                bIgnoreDeviceSelection = False

                Dim videoItem As FilterItem = CType(listVideoDev.SelectedItem, FilterItem)
                Dim audioItem As FilterItem = CType(listAudioDev.SelectedItem, FilterItem)

                Dim hr As Integer = InitInputDev(ms, videoItem, audioItem)
                DsError.ThrowExceptionForHR(hr)

                If hr <> 0 Then
                    Trace.WriteLine("Cannot use the selected capture devices")
                End If

                ms.rot = New DsROTEntry(ms.graph)

                Dim dvdRecorder As New VideoRecorder()
                Debug.Assert(dvdRecorder IsNot Nothing)

                Dim pbPlugin As String = "VRPBDevice.dll"
                Dim fsPlugin As String = "VRFSDevice.dll"
#If WIN64 Then
                pbPlugin = "VRPBDevice64.dll"
                fsPlugin = "VRFSDevice64.dll"
#End If


                Dim pluginPath As String = "..\..\..\..\..\..\..\..\DVDBuilder.CPP\DVDBuilderSDK\lib\"

                ' load PrimoBurner plugin
                m_dvdPlugin = dvdRecorder.LoadDevicePlugin(pbPlugin)
                If m_dvdPlugin Is Nothing Then
                    Dim plugin As String = pluginPath & pbPlugin
                    m_dvdPlugin = dvdRecorder.LoadDevicePlugin(plugin)
                    If m_dvdPlugin Is Nothing Then
                        MessageBox.Show("Cannot load DVD device plugin")
                    End If
                End If


                ' load FileSystem plugin
                m_fsPlugin = dvdRecorder.LoadDevicePlugin(fsPlugin)
                If m_fsPlugin Is Nothing Then
                    Dim plugin As String = pluginPath & fsPlugin
                    m_fsPlugin = dvdRecorder.LoadDevicePlugin(plugin)
                    If m_fsPlugin Is Nothing Then
                        MessageBox.Show("Cannot load File System plugin")
                    End If
                End If

                dvdRecorder.Dispose()
                m_mainWindow = Me.Handle
                UpdateDrivesAsync()
            Catch ex As COMException
                MessageBox.Show(ex.ToString())
            End Try
        End Sub

        Private Sub listAudioDev_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles listAudioDev.SelectedIndexChanged
            Dim item As FilterItem = CType(listAudioDev.SelectedItem, FilterItem)
            Dim hr As Integer
            If item IsNot Nothing Then
                If (Not bIgnoreDeviceSelection) Then
                    Try
                        hr = InitInputDev(ms, Nothing, item)
                    Catch ex As COMException
                        MessageBox.Show(ex.ToString())
                    End Try
                End If
            End If
        End Sub

        Private Sub listVideoDev_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles listVideoDev.SelectedIndexChanged
            Dim item As FilterItem = CType(listVideoDev.SelectedItem, FilterItem)
            Dim hr As Integer
            If item IsNot Nothing Then
                If (Not bIgnoreDeviceSelection) Then
                    Try
                        hr = InitInputDev(ms, item, Nothing)
                    Catch ex As COMException
                        MessageBox.Show(ex.ToString())
                    End Try
                End If
            End If
        End Sub

        Private Sub cmdAudioDevProp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAudioDevProp.Click
            If (Not CheckInputDevice(ms.audioInput)) Then
                Return
            End If

            ShowPropPages(ms.audioInput)
        End Sub

        Private Sub cmdAudioCaptureProp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAudioCaptureProp.Click
            If (Not CheckInputDevice(ms.audioInput)) Then
                Return
            End If

            Dim hr As Integer = 0
            Dim streaConfigId As Guid = GetType(IAMStreamConfig).GUID
            Dim streamConfigObj As Object = Nothing

            Try
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, DirectShowLib.MediaType.Audio, ms.audioInput, streaConfigId, streamConfigObj)

                DsError.ThrowExceptionForHR(hr)

                ShowPropPages(streamConfigObj)
            Catch ex As COMException
                MessageBox.Show(ex.ToString())
            Finally
                Util.ReleaseComObject(streamConfigObj)
            End Try
        End Sub

        Private Function CheckInputDevice(ByVal inputDevice As IBaseFilter) As Boolean
            If inputDevice Is Nothing Then
                MessageBox.Show("No input device!")
                Return False
            End If
            Return True
        End Function

        Private Sub ShowPropPages(ByVal obj As Object)
            Dim specPropPages As ISpecifyPropertyPages = Nothing

            Try
                specPropPages = TryCast(obj, ISpecifyPropertyPages)
                If Nothing Is specPropPages Then
                    MessageBox.Show("Property pages not available")
                    Return
                End If

                Dim cauuid As DsCAUUID
                Dim hr As Integer = specPropPages.GetPages(cauuid)
                DsError.ThrowExceptionForHR(hr)

                If hr = 0 AndAlso cauuid.cElems > 0 Then
                    ' show property pages
                    hr = WinAPI.OleCreatePropertyFrame(Me.Handle, 30, 30, Nothing, 1, obj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero)

                    Marshal.FreeCoTaskMem(cauuid.pElems)

                End If
            Catch ex As COMException
                MessageBox.Show(ex.ToString())
            End Try

            'do not release interfaces obtained with a cast (as), the primary interface is also released
        End Sub

        Private Sub cmdVideoDevProp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdVideoDevProp.Click
            If (Not CheckInputDevice(ms.videoInput)) Then
                Return
            End If

            ShowPropPages(ms.videoInput)
        End Sub

        Private Sub cmdVideoCaptureProp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdVideoCaptureProp.Click
            If (Not CheckInputDevice(ms.videoInput)) Then
                Return
            End If

            Dim hr As Integer = 0
            Dim streamConfigId As Guid = GetType(IAMStreamConfig).GUID
            Dim streamConfigObj As Object = Nothing

            Try
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, DirectShowLib.MediaType.Video, ms.videoInput, streamConfigId, streamConfigObj)

                DsError.ThrowExceptionForHR(hr)

                ShowPropPages(streamConfigObj)
            Catch ex As COMException
                MessageBox.Show(ex.ToString())
            Finally
                Util.ReleaseComObject(streamConfigObj)
            End Try
        End Sub

        Private Function StartRecording() As Boolean
            Dim hr As Integer = 0
            If Nothing Is ms.audioInput OrElse Nothing Is ms.videoInput Then
                MessageBox.Show("No audio or video input!")
                Return False
            End If

            Dim cleanup As Boolean = True
            Dim audioConfigObj As Object = Nothing
            Dim audioConfig As IAMStreamConfig = Nothing

            Try
                ms.videoSampleGrabber = TryCast(New SampleGrabber(), IBaseFilter)
                If ms.videoSampleGrabber Is Nothing Then
                    Throw New COMException("Cannot create SampleGrabber")
                End If

                hr = ms.graph.AddFilter(ms.videoSampleGrabber, "Video SampleGrabber")
                DsError.ThrowExceptionForHR(hr)

                ms.videoGrabber = TryCast(ms.videoSampleGrabber, ISampleGrabber)
                If ms.videoGrabber Is Nothing Then
                    Throw New COMException("Cannot obtain ISampleGrabber")
                End If
                ' Create the Audio Sample Grabber.
                ms.audioSampleGrabber = TryCast(New SampleGrabber(), IBaseFilter)
                If ms.audioSampleGrabber Is Nothing Then
                    Throw New COMException("Cannot create SampleGrabber")
                End If

                hr = ms.graph.AddFilter(ms.audioSampleGrabber, "Audio SampleGrabber")
                DsError.ThrowExceptionForHR(hr)

                ms.audioGrabber = TryCast(ms.audioSampleGrabber, ISampleGrabber)
                If ms.audioGrabber Is Nothing Then
                    Throw New COMException("Cannot obtain ISampleGrabber")
                End If


                ' Create and add the audio null renderer in the graph
                ms.audioNullRenderer = TryCast(New NullRenderer(), IBaseFilter)
                If ms.audioNullRenderer Is Nothing Then
                    Throw New COMException("Cannot create NullRenderer")
                End If

                hr = ms.graph.AddFilter(ms.audioNullRenderer, "Audio NullRenderer")
                DsError.ThrowExceptionForHR(hr)



                ' Create and add the video null renderer in the graph
                ms.videoNullRenderer = TryCast(New NullRenderer(), IBaseFilter)
                If ms.videoNullRenderer Is Nothing Then
                    Throw New COMException("Cannot create NullRenderer")
                End If

                hr = ms.graph.AddFilter(ms.videoNullRenderer, "Video NullRenderer")
                DsError.ThrowExceptionForHR(hr)

                ' manually connect the filters
                hr = Util.ConnectFilters(ms.graph, ms.audioInput, ms.audioSampleGrabber)
                DsError.ThrowExceptionForHR(hr)

                hr = Util.ConnectFilters(ms.graph, ms.audioSampleGrabber, ms.audioNullRenderer)
                DsError.ThrowExceptionForHR(hr)

                ' add the smart tee if preview is required
                If cmdPreview.Checked AndAlso ms.smartTee Is Nothing Then
                    ms.smartTee = TryCast(New SmartTee(), IBaseFilter)
                    hr = ms.graph.AddFilter(ms.smartTee, "Smart Tee")
                    DsError.ThrowExceptionForHR(hr)
                End If

                If ms.smartTee IsNot Nothing Then
                    ' connect the video input to the smart tee
                    Util.ConnectFilters(ms.graph, ms.videoInput, ms.smartTee)

                    ' connect smart tee capture to video grabber
                    Dim capturePin As IPin = Nothing
                    hr = Util.GetPin(ms.smartTee, PinDirection.Output, "Capture", capturePin)
                    DsError.ThrowExceptionForHR(hr)

                    Dim videoGrabberPin As IPin = Nothing
                    hr = Util.GetUnconnectedPin(ms.videoSampleGrabber, PinDirection.Input, videoGrabberPin)
                    DsError.ThrowExceptionForHR(hr)

                    hr = ms.graph.ConnectDirect(capturePin, videoGrabberPin, Nothing)
                    DsError.ThrowExceptionForHR(hr)

                    ' connect smart tee preview to video renderer
                    ms.previewRenderer = TryCast(New VideoRendererDefault(), IBaseFilter)
                    hr = ms.graph.AddFilter(ms.previewRenderer, "Preview Renderer")
                    DsError.ThrowExceptionForHR(hr)

                    Dim previewPin As IPin = Nothing
                    hr = Util.GetPin(ms.smartTee, PinDirection.Output, "Preview", previewPin)
                    DsError.ThrowExceptionForHR(hr)

                    Dim videoRendererPin As IPin = Nothing
                    hr = Util.GetUnconnectedPin(ms.previewRenderer, PinDirection.Input, videoRendererPin)
                    DsError.ThrowExceptionForHR(hr)

                    hr = ms.graph.Connect(previewPin, videoRendererPin)
                    DsError.ThrowExceptionForHR(hr)
                Else
                    hr = Util.ConnectFilters(ms.graph, ms.videoInput, ms.videoSampleGrabber)
                    DsError.ThrowExceptionForHR(hr)
                End If

                hr = Util.ConnectFilters(ms.graph, ms.videoSampleGrabber, ms.videoNullRenderer)
                DsError.ThrowExceptionForHR(hr)


                ' auto connect filters
                '                
                '                hr = ms.captureGraph.RenderStream(
                '                    PinCategory.Capture, MediaType.Audio, ms.audioInput,
                '                    ms.audioSampleGrabber, ms.audioNullRenderer);
                '                DsError.ThrowExceptionForHR(hr);
                '
                '                hr = ms.captureGraph.RenderStream(
                '                    PinCategory.Capture, MediaType.Video, ms.videoInput,
                '                    ms.videoSampleGrabber, ms.videoNullRenderer);
                '                DsError.ThrowExceptionForHR(hr);
                '                 

                ms.mediaControl = TryCast(ms.graph, IMediaControl)
                If Nothing Is ms.mediaControl Then
                    Throw New COMException("Cannot obtain IMediaControl")
                End If


                hr = ms.audioGrabber.SetCallback(m_audioCB, CInt(Fix(CBMethod.Sample)))
                DsError.ThrowExceptionForHR(hr)

                hr = ms.videoGrabber.SetCallback(m_videoCB, CInt(Fix(CBMethod.Sample)))
                DsError.ThrowExceptionForHR(hr)

                ' grab raw input in file
                'm_audioCB.SetOutputFile("d:\\tmp\\audio.dat");
                'm_videoCB.SetOutputFile("d:\\tmp\\video.dat");

                ' Store the video media type for later use.

                hr = ms.videoGrabber.GetConnectedMediaType(ms.videoType)
                DsError.ThrowExceptionForHR(hr)

                ' pass the media state to the callbacks

                m_audioCB.MediaState = ms
                m_videoCB.MediaState = ms
                m_audioCB.MainWindow = m_mainWindow
                m_videoCB.MainWindow = m_mainWindow

                Dim streamConfigId As Guid = GetType(IAMStreamConfig).GUID
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, Nothing, ms.audioInput, streamConfigId, audioConfigObj)
                DsError.ThrowExceptionForHR(hr)

                audioConfig = TryCast(audioConfigObj, IAMStreamConfig)
                If Nothing Is audioConfig Then
                    Throw New COMException("Cannot obtain IAMStreamConfig")
                End If

                Dim audioType As AMMediaType = Nothing
                hr = audioConfig.GetFormat(audioType)
                DsError.ThrowExceptionForHR(hr)

                Dim wfx As New WaveFormatEx()
                Marshal.PtrToStructure(audioType.formatPtr, wfx)

                ' set audio capture parameters
                wfx.nSamplesPerSec = 48000
                wfx.nChannels = 2
                wfx.wBitsPerSample = 16
                wfx.nBlockAlign = CShort(Fix(wfx.nChannels * wfx.wBitsPerSample / 8))
                wfx.nAvgBytesPerSec = wfx.nSamplesPerSec * wfx.nBlockAlign
                'wfx.wFormatTag = 1; // PCM
                Marshal.StructureToPtr(wfx, audioType.formatPtr, False)
                hr = audioConfig.SetFormat(audioType)
                DsUtils.FreeAMMediaType(audioType)

                DsError.ThrowExceptionForHR(hr)

                Try
                    ms.droppedFrames = TryCast(ms.videoInput, IAMDroppedFrames)
                    'the video capture device may not support IAMDroppedFrames
                Catch
                End Try


                ms.mpeg2Enc = New PrimoSoftware.AVBlocks.Transcoder()

                ' In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                ms.mpeg2Enc.AllowDemoMode = True


                ' set audio input pin
                Dim audioInfo As New AudioStreamInfo()
                audioInfo.BitsPerSample = wfx.wBitsPerSample
                audioInfo.Channels = wfx.nChannels
                audioInfo.SampleRate = wfx.nSamplesPerSec
                audioInfo.StreamType = StreamType.LPCM

                Dim inputSocketAudio As New MediaSocket()
                Dim inputPinAudio As New MediaPin()
                inputPinAudio.StreamInfo = audioInfo
                inputSocketAudio.Pins.Add(inputPinAudio)
                inputSocketAudio.StreamType = StreamType.LPCM

                ms.mpeg2Enc.Inputs.Add(inputSocketAudio)

                ' set video input pin
                Dim videoInfo As New VideoStreamInfo()
                Dim vih As VideoInfoHeader = CType(Marshal.PtrToStructure(ms.videoType.formatPtr, GetType(VideoInfoHeader)), VideoInfoHeader)
                videoInfo.FrameRate = CDbl(10000000) / vih.AvgTimePerFrame
                videoInfo.Bitrate = 0 'vih.BitRate;
                videoInfo.FrameHeight = Math.Abs(vih.BmiHeader.Height)
                videoInfo.FrameWidth = vih.BmiHeader.Width
                videoInfo.DisplayRatioWidth = videoInfo.FrameWidth
                videoInfo.DisplayRatioHeight = videoInfo.FrameHeight
                videoInfo.ColorFormat = Util.GetColorFormat(ms.videoType.subType)
                videoInfo.Duration = 0
                'TODO: it's possible for the video device to have a compressed format
                videoInfo.StreamType = StreamType.UncompressedVideo
                videoInfo.ScanType = ScanType.Progressive

                Select Case videoInfo.ColorFormat
                    Case ColorFormat.BGR32, ColorFormat.BGR24, ColorFormat.BGR444, ColorFormat.BGR555, ColorFormat.BGR565
                        videoInfo.FrameBottomUp = (vih.BmiHeader.Height > 0)
                End Select

                Dim inputSocket As New MediaSocket()
                Dim inputPin As New MediaPin()
                inputPin.StreamInfo = videoInfo
                inputSocket.Pins.Add(inputPin)
                inputSocket.StreamType = StreamType.UncompressedVideo

                ms.mpeg2Enc.Inputs.Add(inputSocket)

                Dim outputSocketVideo As MediaSocket = MediaSocket.FromPreset(Preset.Video.DVD.PAL_4x3_MP2)

                outputSocketVideo.Stream = m_muxedCB

                ms.mpeg2Enc.Outputs.Add(outputSocketVideo)

                If (Not ms.mpeg2Enc.Open()) Then
                    MessageBox.Show("Cannot open transcoder.")
                    Return False
                End If

                m_muxedCB.Reset()
                ' grab recorded dvd video in file
                'res = m_muxedCB.SetOutputFile("d:\\tmp\\av.mpg");
                m_muxedCB.MainWindow = m_mainWindow

                If (Not cmdSimulate.Checked) Then
                    Dim drives As DriveArray = GetSelectedDrives()
                    If drives Is Nothing Then
                        Return False
                    End If

                    If (Not StartDvdRecorder(drives)) Then
                        MessageBox.Show("Cannot start dvd recording")
                        Return False
                    End If
                End If
                If cmdSimulate.Checked Then
                    m_muxedCB.DvdRecorder = Nothing
                Else
                    m_muxedCB.DvdRecorder = ms.dvdRecorder
                End If

                Dim ivw As IVideoWindow = TryCast(ms.graph, IVideoWindow)
                Try
                    hr = ivw.put_Owner(previewBox.Handle)
                    DsError.ThrowExceptionForHR(hr)

                    hr = ivw.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipChildren Or WindowStyle.ClipSiblings)
                    DsError.ThrowExceptionForHR(hr)

                    hr = ivw.put_Visible(OABool.True)
                    DsError.ThrowExceptionForHR(hr)

                    Dim rc As Rectangle = previewBox.ClientRectangle
                    hr = ivw.SetWindowPosition(0, 0, rc.Right, rc.Bottom)
                    DsError.ThrowExceptionForHR(hr)

                Catch
                End Try

                hr = ms.mediaControl.Pause()
                DsError.ThrowExceptionForHR(hr)
                System.Threading.Thread.Sleep(3000)
                hr = ms.mediaControl.Run()
                DsError.ThrowExceptionForHR(hr)

                ResetStats()

                'TODO: video bitrate is unknown
                'm_avgBitrate = videoBitrate + (48000 * 2 * 2 * 8); //video + audio: samples per sec * bytes per channel * channels * 8bits

                recStartTime = DateTime.Now
                fpsStartTime = recStartTime
                fpsNotDropped = 0
                statsTimer.Start()

                ' recording has started suceffully, do not cleanup
                cleanup = False
            Catch ex As COMException
                MessageBox.Show(ex.ToString())
                Return False
            Finally
                Util.ReleaseComObject(audioConfigObj)
                If cleanup Then
                    ms.Reset(False)
                    m_audioCB.Reset()
                    m_videoCB.Reset()
                    m_muxedCB.Reset()
                End If
            End Try

            Return True
        End Function

        Private Sub StopRecording()
            statsTimer.Stop()

            ms.Reset(False) ' leave the input devices in the graph

            m_audioCB.Reset()
            m_videoCB.Reset()
            m_muxedCB.Reset()
        End Sub

        Private Sub cmdRecord_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRecord.Click
            If bCmdRecordBusy Then
                Return
            End If

            bCmdRecordBusy = True
            cmdRecord.Enabled = False

            If bRecording Then
                ' stop recording
                StopRecording()

                txtRecording.Visible = False
                cmdRecord.Text = sStartRecording
                EnableCommandUI(True)
                EnableDeviceUI(True)
                cmdPreview.Enabled = True
                UpdateDrivesAsync()
                bRecording = False
            Else
                ' start recording
                Try
                    EnableCommandUI(False)
                    EnableDeviceUI(False)
                    If StartRecording() Then
                        If (Not cmdPreview.Checked) Then
                            cmdPreview.Enabled = False
                        End If

                        txtRecording.Visible = True
                        cmdRecord.Text = sStopRecording

                        bRecording = True
                    Else
                        EnableCommandUI(True)
                        EnableDeviceUI(True)
                    End If
                Catch ex As COMException
                    MessageBox.Show(ex.ToString())
                End Try

            End If

            cmdRecord.Enabled = True
            bCmdRecordBusy = False
        End Sub

        Private Function StartDvdRecorder(ByVal drives As DriveArray) As Boolean
            If drives Is Nothing Then
                Return False
            End If

            Dim recorder As VideoRecorder = Nothing
            Dim success As Boolean = False

            Try
                If (Not drives.Initialize()) Then
                    Return False
                End If

                recorder = New VideoRecorder()
                Debug.Assert(recorder IsNot Nothing)

                For Each drive As DriveItem In drives.Items
                    recorder.Devices.Add(drive.Device)
                Next drive

                If recorder.MediaFreeSpace < MIN_FREE_SPACE Then
                    MessageBox.Show("Not enough space on the disc(s).")
                    Return False
                End If

                If recorder.IsFinalized Then
                    MessageBox.Show("Disc(s) already finalized and cannot be written to.")
                    Return False
                End If

                If recorder.StartAsync() Then
                    success = True
                    ms.dvdRecorder = recorder
                End If

                Return success
            Finally
                If (Not success) Then
                    drives.Dispose()

                    If recorder IsNot Nothing Then
                        recorder.Dispose()
                    End If
                End If
            End Try
        End Function

        Protected Overrides Sub WndProc(ByRef m As Message)
            If m.Msg = Util.WM_STOP_CAPTURE Then

                System.Diagnostics.Trace.WriteLine("WM_STOP_CAPTURE WParam:" & m.WParam.ToInt32().ToString())

                cmdRecord_Click(Nothing, Nothing)

                Dim stopReason As Integer = m.WParam.ToInt32()

                If stopReason = -1 Then
                    MessageBox.Show(Me, "An error occurred while recording to the disc. The recording has been stopped.", "DVDBuilder Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                ElseIf stopReason = -2 Then
                    MessageBox.Show(Me, "The disc is full. The recording has been stopped.", "Device Out of space", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                ElseIf stopReason >= 0 Then
                    MessageBox.Show(Me, "An error occurred encoding captured data. The recording has been stopped.", "AVBlocks", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    MessageBox.Show(Me, "An error occurred while recording. The recording has been stopped.", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

            ElseIf m.Msg = WinAPI.WM_DEVICECHANGE AndAlso (Not bRecording) AndAlso (Not bCmdRecordBusy) Then
                Dim devEvent As Integer = m.WParam.ToInt32()
                Select Case devEvent
                    Case WinAPI.DBT_DEVICEARRIVAL, WinAPI.DBT_DEVICEREMOVECOMPLETE
                        UpdateDrivesAsync()
                End Select
            Else
                MyBase.WndProc(m)
            End If
        End Sub

        Private Sub RecorderForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
            If bRecording Then
                statsTimer.Stop()

                ms.Reset(True)

                m_audioCB.Reset()
                m_videoCB.Reset()
                m_muxedCB.Reset()

                Thread.Sleep(300)
            Else
                ms.Reset(True)
            End If
        End Sub

        Private Sub cmdFinalize_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdFinalize.Click
            Dim drives As DriveArray = GetSelectedDrives()
            If drives Is Nothing Then
                Return
            End If

            Dim recorder As New VideoRecorder()
            Debug.Assert(recorder IsNot Nothing)

            Dim bUpdateDrives As Boolean = False

            Try
                cmdFinalize.Enabled = False

                If (Not drives.Initialize()) Then
                    MessageBox.Show("Cannot access disc(s)", Nothing, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim now As DateTime = DateTime.Now

                Dim label As String = String.Format("{0}{1:d2}{2:d2}_{3:d2}{4:d2}", now.Year, now.Month, now.Day, now.Hour, now.Minute)

                For Each drive As DriveItem In drives.Items
                    Dim device As VRDevice = drive.Device
                    recorder.Devices.Add(device)
                    If device.Type = VRDeviceType.OpticalDisc Then
                        Dim config As OpticalDiscDeviceConfig = TryCast(device.Config, OpticalDiscDeviceConfig)
                        Trace.WriteLine(String.Format("drive {0} label: {1}", config.DriveLetter, config.VolumeLabel))
                        config.VolumeLabel = label
                    End If
                Next drive

                If (Not recorder.IsFinalizeSupported) Then
                    MessageBox.Show("Video cannot be finalized")
                    Return
                End If

                If recorder.IsFinalized Then
                    MessageBox.Show("Video already finalized")
                    Return
                End If

                If System.Windows.Forms.DialogResult.Yes <> MessageBox.Show("Do you want to finalize the video now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then
                    Return
                End If

                Dim res As Boolean = recorder.FinalizeMedia()
                bUpdateDrives = True

                If res Then
                    drives.NotifyChanges()

                    If recorder.IsFinalized Then
                        MessageBox.Show("Video finalized successfully")
                        Return
                    End If
                Else
                    Trace.WriteLine("FinalizeMedia failed")
                    Util.CheckRecorderDevices(recorder)
                End If

                MessageBox.Show("An error occurred while finalizing disc(s).", Nothing, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                drives.Dispose()
                recorder.Dispose()
                If bUpdateDrives Then
                    UpdateDrivesAsync()
                End If
                cmdFinalize.Enabled = True
            End Try
        End Sub

        Private Sub cmdPreview_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPreview.CheckedChanged
            If bRecording AndAlso ms.previewRenderer IsNot Nothing Then
                Dim window As IVideoWindow = TryCast(ms.graph, IVideoWindow)
                If cmdPreview.Checked Then
                    window.put_Visible(OABool.True)
                Else
                    window.put_Visible(OABool.False)
                End If
            End If
        End Sub

        Private Function GetSelectedDrives() As DriveArray
            Dim selCount As Integer = listDrives.CheckedIndices.Count
            If selCount > 0 AndAlso m_dvdPlugin Is Nothing Then
                MessageBox.Show("DVD plugin not loaded")
                Return Nothing
            End If

            Dim bDVD As Boolean = selCount > 0
            Dim folder As String = editFolder.Text

            If folder.Length > 0 Then
                If (Not System.IO.Directory.Exists(folder)) Then
                    MessageBox.Show("Invalid folder")
                    Return Nothing
                End If

                If m_fsPlugin Is Nothing Then
                    MessageBox.Show("File system plugin not loaded")
                    Return Nothing
                End If
            End If

            Dim bFolder As Boolean = False
            If folder.Length > 0 Then
                bFolder = True
            End If

            Dim drives As DriveArray = Nothing

            ' DVD Drives
            If bDVD Then
                If drives Is Nothing Then
                    drives = New DriveArray()
                End If

                For Each index As Integer In listDrives.CheckedIndices
                    Dim di As DriveListItem = CType(listDrives.Items(index), DriveListItem)

                    Dim device As VRDevice = m_dvdPlugin.CreateOpticalDiscDevice(di.Letter, True)
                    Debug.Assert(device IsNot Nothing)

                    drives.Items.Add(New DriveItem(device))
                    ' the device is not initialized, so it may not be accessible!
                Next index
            End If

            ' Folder
            If bFolder Then
                If drives Is Nothing Then
                    drives = New DriveArray()
                End If


                Dim device As VRDevice = m_fsPlugin.CreateFileSystemDevice(folder)
                Debug.Assert(device IsNot Nothing)

                drives.Items.Add(New DriveItem(device))
                ' the device is not initialized, so it may not be accessible!
            End If

            Return drives

        End Function

        Private Sub cmdEraseDisc_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdErase.Click
            Dim drives As DriveArray = GetSelectedDrives()
            If drives Is Nothing Then
                Return
            End If

            cmdErase.Enabled = False
            Dim bUpdateDrives As Boolean = False

            Try
                If (Not drives.Initialize()) Then
                    MessageBox.Show("Cannot access disc(s)", Nothing, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim erasableCount As Integer = drives.GetErasableCount()

                If erasableCount > 0 Then
                    If System.Windows.Forms.DialogResult.Yes <> MessageBox.Show("Erase disc(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then
                        Return
                    End If

                    Dim res As Boolean = drives.Erase()
                    bUpdateDrives = True
                    If res Then
                        MessageBox.Show("Disc(s) erased successfully")
                    Else
                        MessageBox.Show("Could not erase disc(s)", Nothing, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
            Finally
                drives.Dispose()
                If bUpdateDrives Then
                    UpdateDrivesAsync()
                End If
                cmdErase.Enabled = True
            End Try
        End Sub

        Private Class DriveListItem
            Public Sub New(ByVal letter As Char, ByVal fullName As String)
                Me.Letter = letter
                Me.FullName = fullName
            End Sub

            ' single drive
            Public Letter As Char

            ' display text
            Public FullName As String

            Public Overrides Function ToString() As String
                Return FullName
            End Function
        End Class

        Private Class FilterItem
            Public Sub New()
            End Sub
            Public Sub New(ByVal friendlyName As String, ByVal displayName As String)
                Me.FriendlyName = friendlyName
                Me.DisplayName = displayName
            End Sub

            Public FriendlyName As String
            Public DisplayName As String

            Public Overrides Function ToString() As String
                Return FriendlyName
            End Function
        End Class

        Private Sub cmdExit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdExit.Click
            Close()
        End Sub

        Private Sub cmdBrowseFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdBrowseFolder.Click
            Dim dlg As New FolderBrowserDialog()
            If editFolder.Text.Length > 0 Then
                dlg.SelectedPath = editFolder.Text
            End If

            dlg.Description = "Select video output folder"

            If System.Windows.Forms.DialogResult.OK = dlg.ShowDialog() Then
                editFolder.Text = dlg.SelectedPath
            End If

        End Sub

        Private Sub cmdClearFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClearFolder.Click
            editFolder.Text = String.Empty
        End Sub

        Private Sub cmdVideoInfo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdVideoInfo.Click
            Dim drives As DriveArray = GetSelectedDrives()
            If drives Is Nothing Then
                Return
            End If

            Dim recorder As New VideoRecorder()
            Debug.Assert(recorder IsNot Nothing)


            Try
                cmdVideoInfo.Enabled = False

                If (Not drives.Initialize()) Then
                    MessageBox.Show("Cannot access disc(s)", Nothing, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim info As New StringBuilder()

                For Each drive As DriveItem In drives.Items
                    Dim device As VRDevice = drive.Device
                    recorder.Devices.Add(device)

                    If device.Type = VRDeviceType.OpticalDisc Then
                        Dim config As OpticalDiscDeviceConfig = CType(device.Config, OpticalDiscDeviceConfig)
                        info.AppendFormat(Constants.vbCrLf & "DVD Drive {0}:, Volume Label:{1}" & Constants.vbCrLf, Char.ToUpper(config.DriveLetter), config.VolumeLabel)
                    ElseIf device.Type = VRDeviceType.FileSystem Then
                        Dim config As FileSystemDeviceConfig = CType(device.Config, FileSystemDeviceConfig)
                        info.AppendFormat(Constants.vbCrLf & "DVD Video, Folder: {0}" & Constants.vbCrLf, config.Folder)
                    End If

                    Dim titles As IList(Of Title) = recorder.GetTitles(0)
                    If titles IsNot Nothing Then
                        Dim total As New TimeSpan(0)
                        For i As Integer = 0 To titles.Count - 1
                            Dim sec As Double = titles(i).Duration
                            'INSTANT VB NOTE: Embedded comments are not maintained by Instant VB
                            'ORIGINAL LINE: TimeSpan dur = new TimeSpan(Convert.ToInt64(sec) * 10000000 /* convert seconds to ticks */);
                            Dim dur As New TimeSpan(Convert.ToInt64(sec) * 10000000)
                            info.AppendFormat("Title {0:d2}, {1:f2} sec, {2}h {3:d2}m {4:d2}s" & Constants.vbCrLf, i + 1, sec, dur.Hours, dur.Minutes, dur.Seconds)

                            total = total.Add(dur)
                        Next i

                        info.AppendFormat("Total: {0}h {1:d2}m {2:d2}s" & Constants.vbCrLf, total.Hours, total.Minutes, total.Seconds)
                    Else
                        info.Append("Cannot read titles" & Constants.vbCrLf)
                    End If

                    recorder.Devices.Clear()
                Next drive

                info.Append(Constants.vbCrLf)

                Dim vi As New VideoInfoForm()
                vi.Info = info.ToString()
                vi.ShowDialog()
            Finally
                drives.Dispose()
                recorder.Dispose()
                cmdVideoInfo.Enabled = True
            End Try
        End Sub

        Private Sub EnableCommandUI(ByVal enable As Boolean)
            cmdFinalize.Enabled = enable
            cmdErase.Enabled = enable
            cmdVideoInfo.Enabled = enable
            cmdSimulate.Enabled = enable
        End Sub

        Private Sub EnableDeviceUI(ByVal enable As Boolean)
            listDrives.Enabled = enable
            editFolder.Enabled = enable
            cmdBrowseFolder.Enabled = enable
            cmdClearFolder.Enabled = enable
        End Sub

        Private Sub cmdSimulate_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSimulate.CheckedChanged
            Dim simulate As Boolean = cmdSimulate.Checked
            EnableDeviceUI((Not simulate))
        End Sub

        ' Get all optical drives in the system; put them in a drive array for a parallel work
        Private Function GetAllDrives() As DriveArray
            If m_dvdPlugin Is Nothing Then
                Return Nothing
            End If

            Dim drives As New DriveArray()

            Dim drivesInfo() As DriveInfo = DriveInfo.GetDrives()

            For Each di As DriveInfo In drivesInfo
                If di.DriveType = DriveType.CDRom Then
                    Dim letter As Char = CChar(di.Name(0))
                    Dim device As VRDevice = m_dvdPlugin.CreateOpticalDiscDevice(letter, False)
                    Debug.Assert(device IsNot Nothing)

                    drives.Items.Add(New DriveItem(device))
                End If
            Next di

            If drives.Items.Count = 0 Then
                Return Nothing
            End If

            Return drives
        End Function


        Private Sub QueryDrivesProc(ByVal args As Object)
            Dim drives As DriveArray = CType(args, DriveArray)

            drives.Initialize()

            drives.Query()

            UpdateDrivesUI(drives)
        End Sub

        Private Sub UpdateDrivesAsync()
            Dim drives As DriveArray = GetAllDrives()
            If drives Is Nothing Then
                Return
            End If

            ' save drive selection
            m_selDrives.Clear()
            For Each index As Integer In listDrives.CheckedIndices
                Dim di As DriveListItem = CType(listDrives.Items(index), DriveListItem)

                m_selDrives.Add(di.Letter)
            Next index

            ' start updating drive information
            txtUpdatingDrives.Visible = True
            listDrives.Enabled = False

            Dim t As New Thread(AddressOf QueryDrivesProc)
            t.Start(drives)
        End Sub

        Private Delegate Sub UpdateDrivesUICaller(ByVal drives As DriveArray)

        Private Sub UpdateDrivesUI(ByVal drives As DriveArray)
            If InvokeRequired Then
                Invoke(New UpdateDrivesUICaller(AddressOf UpdateDrivesUI), drives)
                Return
            End If

            listDrives.Items.Clear()
            txtUpdatingDrives.Visible = False

            For Each drive As DriveItem In drives.Items
                Dim device As VRDevice = drive.Device

                Dim driveInfo As New StringBuilder()

                Dim config As OpticalDiscDeviceConfig = CType(device.Config, OpticalDiscDeviceConfig)
                Dim letter As Char = config.DriveLetter
                driveInfo.AppendFormat("{0}: ", letter, Char.ToUpper(letter))

                If drive.IsInitialized Then
                    ' successfully initialized. device attributes
                    If drive.IsBlank Then
                        driveInfo.Append("Blank")
                    Else
                        If drive.VolumeLabel.Length > 0 Then
                            driveInfo.AppendFormat("{0}", drive.VolumeLabel)
                        Else
                            driveInfo.Append("No Label")
                        End If

                        If drive.IsVideo Then
                            driveInfo.Append("  (Video)")
                        End If
                    End If
                End If

                listDrives.Items.Add(New DriveListItem(letter, driveInfo.ToString()))

                ' restore drive selection
                If m_selDrives.Contains(letter) Then
                    listDrives.SetItemChecked(listDrives.Items.Count - 1, True)
                End If

            Next drive

            listDrives.Enabled = True
            drives.Dispose()
        End Sub
    End Class
End Namespace
