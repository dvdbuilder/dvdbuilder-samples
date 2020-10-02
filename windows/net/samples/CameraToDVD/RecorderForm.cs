using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Diagnostics;
using System.Threading;

using DirectShowLib;
using PrimoSoftware.DVDBuilder.VR;
using PrimoSoftware.AVBlocks;

namespace CameraToDVD
{
    public partial class RecorderForm : Form
    {
        private MediaState ms = new MediaState();

        private bool bIgnoreDeviceSelection;
        private bool bRecording;
        private bool bCmdRecordBusy;
        const string sStartRecording = "Start Recording";
        const string sStopRecording = "Stop Recording";

        private System.Windows.Forms.Timer statsTimer;
        DateTime recStartTime;
        DateTime fpsStartTime; // current fps
        int fpsNotDropped; // current fps
        const long MIN_FREE_SPACE = 3 * 1385000; // bytes, 3 sec video at 1x dvd speed (max bitrate)
        int m_avgBitrate; // bits per second

        private VideoGrabberCB m_videoCB = new VideoGrabberCB("VideoGrabberCB");
        private AudioGrabberCB m_audioCB = new AudioGrabberCB("AudioGrabberCB");
        private MuxedStreamCB m_muxedCB = new MuxedStreamCB();
        private VRDevicePlugin m_dvdPlugin;
        private VRDevicePlugin m_fsPlugin;
        private List<char> m_selDrives = new List<char>();
        private IntPtr m_mainWindow = IntPtr.Zero;

        public RecorderForm()
        {
            InitializeComponent();
#if WIN64
            this.Text += " (64-bit)";
#endif

            EnumInputDev(FilterCategory.AudioInputDevice, listAudioDev);
            EnumInputDev(FilterCategory.VideoInputDevice, listVideoDev);

            statsTimer = new System.Windows.Forms.Timer();
            statsTimer.Tick += new EventHandler(UpdateStats);
            statsTimer.Interval = 500;

            bIgnoreDeviceSelection = false;
            bRecording = false;
            cmdRecord.Text = sStartRecording;
            txtRecording.Visible = false;

            ResetStats();

            int h = previewBox.Size.Height;
            int w = previewBox.Size.Width;
            previewBox.Size = new Size(w, w * 3 / 4);
        }

        void UpdateStats(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan rec = now - recStartTime;
            txtRecTime.Text = string.Format("{0}:{1:d2}:{2:d2}", rec.Hours, rec.Minutes, rec.Seconds);

            if (!cmdSimulate.Checked)
            {
                // format remaining space
                long remSpace = ms.dvdRecorder.MediaFreeSpace;
                txtRemainingSpace.Text = string.Format("{0:f2} MB", (double)(remSpace) / (1024 * 1024));

                int avgBitrate = ms.dvdRecorder.AverageBitrate;
                if (avgBitrate > 0)
                    m_avgBitrate = avgBitrate;

                if (m_avgBitrate > 0)
                {
                    TimeSpan rem = new TimeSpan((long)(remSpace * 8 / m_avgBitrate) * 10000000 /* convert seconds to ticks */);
                    txtRemainingTime.Text = string.Format("{0:d2}:{1:d2}:{2:d2}", rem.Hours, rem.Minutes, rem.Seconds);
                }
            }

            if (ms.droppedFrames != null)
            {
                int hr = 0;
                int dropped;
                hr = ms.droppedFrames.GetNumDropped(out dropped);
                if (0 == hr)
                    txtNumDropped.Text = dropped.ToString();

                int notDropped;
                hr = ms.droppedFrames.GetNumNotDropped(out notDropped);

                if (0 == hr)
                {
                    txtNumNotDropped.Text = notDropped.ToString();
                    if (notDropped >= 0)
                    {
                        double averageFPS = (double)notDropped / rec.TotalSeconds;
                        txtAverageFPS.Text = averageFPS.ToString("F3");

                        TimeSpan tsfps = now - fpsStartTime;
                        double fpsElapsed = tsfps.TotalSeconds;
                        if (fpsElapsed > 5.0)
                        {
                            double curFPS = (double)(notDropped - fpsNotDropped) / fpsElapsed;
                            txtCurrentFPS.Text = curFPS.ToString("F3");

                            fpsStartTime = now;
                            fpsNotDropped = notDropped;
                        }
                    }
                }
            }

            txtACallbacks.Text = m_audioCB.SampleIndex.ToString();
            txtAProcessed.Text = m_audioCB.ProcessedSamples.ToString();
            txtADropped.Text = m_audioCB.DroppedSamples.ToString();

            txtVCallbacks.Text = m_videoCB.SampleIndex.ToString();
            txtVProcessed.Text = m_videoCB.ProcessedSamples.ToString();
            txtVDropped.Text = m_videoCB.DroppedSamples.ToString();

        }

        void ResetStats()
        {
            string sEmpty = "--";

            txtRecTime.Text = "--:--:--";
            txtRemainingTime.Text = "--:--:--";
            txtRemainingSpace.Text = "--- MB";

            txtNumDropped.Text = sEmpty;
            txtNumNotDropped.Text = sEmpty;
            txtAverageFPS.Text = sEmpty;
            txtCurrentFPS.Text = sEmpty;

            txtACallbacks.Text = sEmpty;
            txtADropped.Text = sEmpty;
            txtAProcessed.Text = sEmpty;

            txtVCallbacks.Text = sEmpty;
            txtVDropped.Text = sEmpty;
            txtVProcessed.Text = sEmpty;
        }

        void EnumInputDev(Guid filterCategory, ComboBox list)
        {
            if (list == null)
                return;

            ICreateDevEnum devEnum = null;
            IEnumMoniker enumCat = null;
            IMoniker[] moniker = new IMoniker[1] { null };
            IPropertyBag propBag = null;

            int hr = 0;
            try
            {

                devEnum = new CreateDevEnum() as ICreateDevEnum;
                if (devEnum == null)
                    throw new COMException("Cannot create CLSID_SystemDeviceEnum");

                // Obtain enumerator for the capture category.
                hr = devEnum.CreateClassEnumerator(filterCategory, out enumCat, 0);
                DsError.ThrowExceptionForHR(hr);

                if (enumCat == null)
                {
                    MessageBox.Show("No capture devices found");
                    return;
                }

                // Enumerate the monikers.
                IntPtr fetchedCount = new IntPtr(0);
                while (0 == enumCat.Next(1, moniker, fetchedCount))
                {

                    Guid bagId = typeof(IPropertyBag).GUID;
                    object bagObj;
                    moniker[0].BindToStorage(null, null, ref bagId, out bagObj);

                    propBag = bagObj as IPropertyBag;

                    if (propBag != null)
                    {
                        object val;
                        string friendlyName = null;
                        string displayName = null;

                        hr = propBag.Read("FriendlyName", out val, null);
                        if (hr == 0)
                        {
                            friendlyName = val as string;
                        }
                        Util.ReleaseComObject(ref propBag);

                        moniker[0].GetDisplayName(null, null, out displayName);

                        // create an instance of the filter
                        Guid baseFilterId = typeof(IBaseFilter).GUID;
                        object filter;
                        bool addFilter = false;
                        try
                        {
                            moniker[0].BindToObject(null, null, ref baseFilterId, out filter);
                            if (filter != null)
                            {
                                addFilter = true;
                                Util.ReleaseComObject(ref filter);
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Trace.WriteLine("Cannot use input device " + friendlyName);
                        }

                        if (addFilter == true &&
                            friendlyName != null &&
                            displayName != null)
                        {
                            FilterItem fi = new FilterItem(friendlyName, displayName);
                            list.Items.Add(fi);
                        }
                    } // if IPropertyBag

                    Util.ReleaseComObject(ref moniker[0]);
                } // while enum devices
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Util.ReleaseComObject(ref propBag);
                Util.ReleaseComObject(ref moniker[0]);
                Util.ReleaseComObject(ref enumCat);
                Util.ReleaseComObject(ref devEnum);
            }

        } // EnumInputDev

        private int InitInputDev(MediaState ms, FilterItem videoItem, FilterItem audioItem)
        {
            int hr = 0;
            // create Filter Graph Manager
            if (ms.graph == null)
            {
                //ms.graph = new FilterGraph() as IGraphBuilder;
                ms.graph = new FilterGraph() as IFilterGraph2;
                if (ms.graph == null)
                {
                    throw new COMException("Cannot create FilterGraph");
                }

                ms.captureGraph = new CaptureGraphBuilder2() as ICaptureGraphBuilder2;
                if (ms.captureGraph == null)
                {
                    throw new COMException("Cannot create CaptureGraphBuilder2");
                }

                hr = ms.captureGraph.SetFiltergraph(ms.graph);
                DsError.ThrowExceptionForHR(hr);
            }


            if (audioItem != null)
            {
                // remove the old audio input
                if (ms.audioInput != null)
                {
                    hr = ms.graph.RemoveFilter(ms.audioInput);
                    Util.ReleaseComObject(ref ms.audioInput);
                    DsError.ThrowExceptionForHR(hr);
                }

                // create audio input

                // using BindToMoniker
                ms.audioInput = Marshal.BindToMoniker(audioItem.DisplayName) as IBaseFilter;

                // add audio input to the graph
                hr = ms.graph.AddFilter(ms.audioInput, audioItem.FriendlyName);
                DsError.ThrowExceptionForHR(hr);
            }


            if (videoItem != null)
            {
                // remove the old video input
                if (ms.videoInput != null)
                {
                    hr = ms.graph.RemoveFilter(ms.videoInput);
                    Util.ReleaseComObject(ref ms.videoInput);
                    DsError.ThrowExceptionForHR(hr);
                }

                // create video input

                // Using BindToMoniker
                ms.videoInput = Marshal.BindToMoniker(videoItem.DisplayName) as IBaseFilter;

                // add video input to the graph
                hr = ms.graph.AddFilter(ms.videoInput, videoItem.FriendlyName);
                DsError.ThrowExceptionForHR(hr);


            }


            return hr;
        }

        private void RecorderForm_Load(object sender, EventArgs e)
        {
            try
            {
                bIgnoreDeviceSelection = true;

                if (listAudioDev.Items.Count > 0)
                    listAudioDev.SelectedIndex = 0;

                if (listVideoDev.Items.Count > 0)
                    listVideoDev.SelectedIndex = 0;

                bIgnoreDeviceSelection = false;

                FilterItem videoItem = (FilterItem)listVideoDev.SelectedItem;
                FilterItem audioItem = (FilterItem)listAudioDev.SelectedItem;

                int hr = InitInputDev(ms, videoItem, audioItem);
                DsError.ThrowExceptionForHR(hr);

                if (hr != 0)
                    Trace.WriteLine("Cannot use the selected capture devices");

                ms.rot = new DsROTEntry(ms.graph);

                VideoRecorder dvdRecorder = new VideoRecorder();
                Debug.Assert(dvdRecorder != null);

                string pbPlugin = "VRPBDevice.dll";
                string fsPlugin = "VRFSDevice.dll";
#if WIN64
                pbPlugin = "VRPBDevice64.dll";
	            fsPlugin = "VRFSDevice64.dll";
#endif


                string pluginPath = @"..\..\..\..\..\..\..\..\DVDBuilder.CPP\DVDBuilderSDK\lib\";

                // load PrimoBurner plugin
                m_dvdPlugin = dvdRecorder.LoadDevicePlugin(pbPlugin);
                if (m_dvdPlugin == null)
                {
                    string plugin = pluginPath + pbPlugin;
                    m_dvdPlugin = dvdRecorder.LoadDevicePlugin(plugin);
                    if (m_dvdPlugin == null)
                    {
                        MessageBox.Show("Cannot load DVD device plugin");
                    }
                }


                // load FileSystem plugin
                m_fsPlugin = dvdRecorder.LoadDevicePlugin(fsPlugin);
                if (m_fsPlugin == null)
                {
                    string plugin = pluginPath + fsPlugin;
                    m_fsPlugin = dvdRecorder.LoadDevicePlugin(plugin);
                    if (m_fsPlugin == null)
                    {
                        MessageBox.Show("Cannot load File System plugin");
                    }
                }

                dvdRecorder.Dispose();
                m_mainWindow = this.Handle;
                UpdateDrivesAsync();
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listAudioDev_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItem item = (FilterItem)listAudioDev.SelectedItem;
            int hr;
            if (item != null)
            {
                if (!bIgnoreDeviceSelection)
                {
                    try
                    {
                        hr = InitInputDev(ms, null, item);
                    }
                    catch (COMException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void listVideoDev_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItem item = (FilterItem)listVideoDev.SelectedItem;
            int hr;
            if (item != null)
            {
                if (!bIgnoreDeviceSelection)
                {
                    try
                    {
                        hr = InitInputDev(ms, item, null);
                    }
                    catch (COMException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void cmdAudioDevProp_Click(object sender, EventArgs e)
        {
            if (!CheckInputDevice(ms.audioInput))
                return;

            ShowPropPages(ms.audioInput);
        }

        private void cmdAudioCaptureProp_Click(object sender, EventArgs e)
        {
            if (!CheckInputDevice(ms.audioInput))
                return;

            int hr = 0;
            Guid streaConfigId = typeof(IAMStreamConfig).GUID;
            object streamConfigObj = null;

            try
            {
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, DirectShowLib.MediaType.Audio,
                                                    ms.audioInput, streaConfigId, out streamConfigObj);

                DsError.ThrowExceptionForHR(hr);

                ShowPropPages(streamConfigObj);
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Util.ReleaseComObject(ref streamConfigObj);
            }
        }

        private bool CheckInputDevice(IBaseFilter inputDevice)
        {
            if (inputDevice == null)
            {
                MessageBox.Show("No input device!");
                return false;
            }
            return true;
        }

        private void ShowPropPages(object obj)
        {
            ISpecifyPropertyPages specPropPages = null;

            try
            {
                specPropPages = obj as ISpecifyPropertyPages;
                if (null == specPropPages)
                {
                    MessageBox.Show("Property pages not available");
                    return;
                }

                DsCAUUID cauuid;
                int hr = specPropPages.GetPages(out cauuid);
                DsError.ThrowExceptionForHR(hr);

                if (hr == 0 && cauuid.cElems > 0)
                {
                    // show property pages
                    hr = WinAPI.OleCreatePropertyFrame(this.Handle,
                        30, 30, null, 1,
                        ref obj, cauuid.cElems,
                        cauuid.pElems, 0, 0, IntPtr.Zero);

                    Marshal.FreeCoTaskMem(cauuid.pElems);

                }
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //do not release interfaces obtained with a cast (as), the primary interface is also released
        }

        private void cmdVideoDevProp_Click(object sender, EventArgs e)
        {
            if (!CheckInputDevice(ms.videoInput))
                return;

            ShowPropPages(ms.videoInput);
        }

        private void cmdVideoCaptureProp_Click(object sender, EventArgs e)
        {
            if (!CheckInputDevice(ms.videoInput))
                return;

            int hr = 0;
            Guid streamConfigId = typeof(IAMStreamConfig).GUID;
            object streamConfigObj = null;

            try
            {
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, DirectShowLib.MediaType.Video,
                                                    ms.videoInput, streamConfigId, out streamConfigObj);

                DsError.ThrowExceptionForHR(hr);

                ShowPropPages(streamConfigObj);
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Util.ReleaseComObject(ref streamConfigObj);
            }
        }

        private bool StartRecording()
        {
            int hr = 0;
            if (null == ms.audioInput || null == ms.videoInput)
            {
                MessageBox.Show("No audio or video input!");
                return false;
            }

            bool cleanup = true;
            object audioConfigObj = null;
            IAMStreamConfig audioConfig = null;

            try
            {
                ms.videoSampleGrabber = new SampleGrabber() as IBaseFilter;
                if (ms.videoSampleGrabber == null)
                {
                    throw new COMException("Cannot create SampleGrabber");
                }

                hr = ms.graph.AddFilter(ms.videoSampleGrabber, "Video SampleGrabber");
                DsError.ThrowExceptionForHR(hr);

                ms.videoGrabber = ms.videoSampleGrabber as ISampleGrabber;
                if (ms.videoGrabber == null)
                {
                    throw new COMException("Cannot obtain ISampleGrabber");
                }
                // Create the Audio Sample Grabber.
                ms.audioSampleGrabber = new SampleGrabber() as IBaseFilter;
                if (ms.audioSampleGrabber == null)
                {
                    throw new COMException("Cannot create SampleGrabber");
                }

                hr = ms.graph.AddFilter(ms.audioSampleGrabber, "Audio SampleGrabber");
                DsError.ThrowExceptionForHR(hr);

                ms.audioGrabber = ms.audioSampleGrabber as ISampleGrabber;
                if (ms.audioGrabber == null)
                {
                    throw new COMException("Cannot obtain ISampleGrabber");
                }


                // Create and add the audio null renderer in the graph
                ms.audioNullRenderer = new NullRenderer() as IBaseFilter;
                if (ms.audioNullRenderer == null)
                {
                    throw new COMException("Cannot create NullRenderer");
                }

                hr = ms.graph.AddFilter(ms.audioNullRenderer, "Audio NullRenderer");
                DsError.ThrowExceptionForHR(hr);



                // Create and add the video null renderer in the graph
                ms.videoNullRenderer = new NullRenderer() as IBaseFilter;
                if (ms.videoNullRenderer == null)
                {
                    throw new COMException("Cannot create NullRenderer");
                }

                hr = ms.graph.AddFilter(ms.videoNullRenderer, "Video NullRenderer");
                DsError.ThrowExceptionForHR(hr);

                // manually connect the filters
                hr = Util.ConnectFilters(ms.graph, ms.audioInput, ms.audioSampleGrabber);
                DsError.ThrowExceptionForHR(hr);

                hr = Util.ConnectFilters(ms.graph, ms.audioSampleGrabber, ms.audioNullRenderer);
                DsError.ThrowExceptionForHR(hr);

                // add the smart tee if preview is required
                if (cmdPreview.Checked && ms.smartTee == null)
                {
                    ms.smartTee = new SmartTee() as IBaseFilter;
                    hr = ms.graph.AddFilter(ms.smartTee, "Smart Tee");
                    DsError.ThrowExceptionForHR(hr);
                }

                if (ms.smartTee != null)
                {
                    // connect the video input to the smart tee
                    Util.ConnectFilters(ms.graph, ms.videoInput, ms.smartTee);

                    // connect smart tee capture to video grabber
                    IPin capturePin;
                    hr = Util.GetPin(ms.smartTee, PinDirection.Output, "Capture", out capturePin);
                    DsError.ThrowExceptionForHR(hr);

                    IPin videoGrabberPin;
                    hr = Util.GetUnconnectedPin(ms.videoSampleGrabber, PinDirection.Input, out videoGrabberPin);
                    DsError.ThrowExceptionForHR(hr);

                    hr = ms.graph.ConnectDirect(capturePin, videoGrabberPin, null);
                    DsError.ThrowExceptionForHR(hr);

                    // connect smart tee preview to video renderer
                    ms.previewRenderer = new VideoRendererDefault() as IBaseFilter;
                    hr = ms.graph.AddFilter(ms.previewRenderer, "Preview Renderer");
                    DsError.ThrowExceptionForHR(hr);

                    IPin previewPin;
                    hr = Util.GetPin(ms.smartTee, PinDirection.Output, "Preview", out previewPin);
                    DsError.ThrowExceptionForHR(hr);

                    IPin videoRendererPin;
                    hr = Util.GetUnconnectedPin(ms.previewRenderer, PinDirection.Input, out videoRendererPin);
                    DsError.ThrowExceptionForHR(hr);

                    hr = ms.graph.Connect(previewPin, videoRendererPin);
                    DsError.ThrowExceptionForHR(hr);
                }
                else
                {
                    hr = Util.ConnectFilters(ms.graph, ms.videoInput, ms.videoSampleGrabber);
                    DsError.ThrowExceptionForHR(hr);
                }

                hr = Util.ConnectFilters(ms.graph, ms.videoSampleGrabber, ms.videoNullRenderer);
                DsError.ThrowExceptionForHR(hr);


                // auto connect filters
                /*
                hr = ms.captureGraph.RenderStream(
                    PinCategory.Capture, MediaType.Audio, ms.audioInput,
                    ms.audioSampleGrabber, ms.audioNullRenderer);
                DsError.ThrowExceptionForHR(hr);

                hr = ms.captureGraph.RenderStream(
                    PinCategory.Capture, MediaType.Video, ms.videoInput,
                    ms.videoSampleGrabber, ms.videoNullRenderer);
                DsError.ThrowExceptionForHR(hr);
                 */

                ms.mediaControl = ms.graph as IMediaControl;
                if (null == ms.mediaControl)
                {
                    throw new COMException("Cannot obtain IMediaControl");
                }


                hr = ms.audioGrabber.SetCallback(m_audioCB, (int)CBMethod.Sample);
                DsError.ThrowExceptionForHR(hr);

                hr = ms.videoGrabber.SetCallback(m_videoCB, (int)CBMethod.Sample);
                DsError.ThrowExceptionForHR(hr);

                // grab raw input in file
                //m_audioCB.SetOutputFile("d:\\tmp\\audio.dat");
                //m_videoCB.SetOutputFile("d:\\tmp\\video.dat");

                // Store the video media type for later use.

                hr = ms.videoGrabber.GetConnectedMediaType(ms.videoType);
                DsError.ThrowExceptionForHR(hr);

                // pass the media state to the callbacks

                m_audioCB.MediaState = ms;
                m_videoCB.MediaState = ms;
                m_audioCB.MainWindow = m_mainWindow;
                m_videoCB.MainWindow = m_mainWindow;

                Guid streamConfigId = typeof(IAMStreamConfig).GUID;
                hr = ms.captureGraph.FindInterface(PinCategory.Capture, null, ms.audioInput,
                    streamConfigId, out audioConfigObj);
                DsError.ThrowExceptionForHR(hr);

                audioConfig = audioConfigObj as IAMStreamConfig;
                if (null == audioConfig)
                    throw new COMException("Cannot obtain IAMStreamConfig");

                AMMediaType audioType;
                hr = audioConfig.GetFormat(out audioType);
                DsError.ThrowExceptionForHR(hr);

                WaveFormatEx wfx = new WaveFormatEx();
                Marshal.PtrToStructure(audioType.formatPtr, wfx);

                // set audio capture parameters
                wfx.nSamplesPerSec = 48000;
                wfx.nChannels = 2;
                wfx.wBitsPerSample = 16;
                wfx.nBlockAlign = (short)(wfx.nChannels * wfx.wBitsPerSample / 8);
                wfx.nAvgBytesPerSec = wfx.nSamplesPerSec * wfx.nBlockAlign;
                //wfx.wFormatTag = 1; // PCM
                Marshal.StructureToPtr(wfx, audioType.formatPtr, false);
                hr = audioConfig.SetFormat(audioType);
                DsUtils.FreeAMMediaType(audioType);

                DsError.ThrowExceptionForHR(hr);

                try
                {
                    ms.droppedFrames = ms.videoInput as IAMDroppedFrames;
                    //the video capture device may not support IAMDroppedFrames
                }
                catch { }


                ms.mpeg2Enc = new PrimoSoftware.AVBlocks.Transcoder();

                // In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                ms.mpeg2Enc.AllowDemoMode = true;


                // set audio input pin
                {
                    AudioStreamInfo audioInfo = new AudioStreamInfo();
                    audioInfo.BitsPerSample = wfx.wBitsPerSample;
                    audioInfo.Channels = wfx.nChannels;
                    audioInfo.SampleRate = wfx.nSamplesPerSec;
                    audioInfo.StreamType = StreamType.LPCM;

                    MediaSocket inputSocket = new MediaSocket();
                    MediaPin inputPin = new MediaPin();
                    inputPin.StreamInfo = audioInfo;
                    inputSocket.Pins.Add(inputPin);
                    inputSocket.StreamType = StreamType.LPCM;

                    ms.mpeg2Enc.Inputs.Add(inputSocket);
                }

                // set video input pin
                {
                    VideoStreamInfo videoInfo = new VideoStreamInfo();
                    VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(ms.videoType.formatPtr, typeof(VideoInfoHeader));
                    videoInfo.FrameRate = (double)10000000 / vih.AvgTimePerFrame;
                    videoInfo.Bitrate = 0; //vih.BitRate;
                    videoInfo.FrameHeight = Math.Abs(vih.BmiHeader.Height);
                    videoInfo.FrameWidth = vih.BmiHeader.Width;
                    videoInfo.DisplayRatioWidth = videoInfo.FrameWidth;
                    videoInfo.DisplayRatioHeight = videoInfo.FrameHeight;
                    videoInfo.ColorFormat = Util.GetColorFormat(ref ms.videoType.subType);
                    videoInfo.Duration = 0;
                    //TODO: it's possible for the video device to have a compressed format
                    videoInfo.StreamType =  StreamType.UncompressedVideo;
                    videoInfo.ScanType = ScanType.Progressive;

                    switch (videoInfo.ColorFormat)
                    {
                        case ColorFormat.BGR32:
                        case ColorFormat.BGR24:
                        case ColorFormat.BGR444:
                        case ColorFormat.BGR555:
                        case ColorFormat.BGR565:
                            videoInfo.FrameBottomUp = (vih.BmiHeader.Height > 0);
                            break;
                    }

                    MediaSocket inputSocket = new MediaSocket();
                    MediaPin inputPin = new MediaPin();
                    inputPin.StreamInfo = videoInfo;
                    inputSocket.Pins.Add(inputPin);
                    inputSocket.StreamType = StreamType.UncompressedVideo;

                    ms.mpeg2Enc.Inputs.Add(inputSocket);
                }

                var outputSocket = MediaSocket.FromPreset(Preset.Video.DVD.PAL_4x3_MP2);

                outputSocket.Stream = m_muxedCB;

                ms.mpeg2Enc.Outputs.Add(outputSocket);

                if (!ms.mpeg2Enc.Open())
                {
                    MessageBox.Show("Cannot open transcoder.");
                    return false;
                }

                m_muxedCB.Reset();
                // grab recorded dvd video in file
                //res = m_muxedCB.SetOutputFile("d:\\tmp\\av.mpg");
                m_muxedCB.MainWindow = m_mainWindow;
                
                if (!cmdSimulate.Checked)
                {
                    DriveArray drives = GetSelectedDrives();
                    if (drives == null)
                        return false;

                    if (!StartDvdRecorder(drives))
                    {
                        MessageBox.Show("Cannot start dvd recording");
                        return false;
                    }
                }
                if (cmdSimulate.Checked)
                    m_muxedCB.DvdRecorder = null;
                else
                    m_muxedCB.DvdRecorder = ms.dvdRecorder;

                IVideoWindow ivw = ms.graph as IVideoWindow;
                try
                {
                    hr = ivw.put_Owner(previewBox.Handle);
                    DsError.ThrowExceptionForHR(hr);

                    hr = ivw.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
                    DsError.ThrowExceptionForHR(hr);

                    hr = ivw.put_Visible(OABool.True);
                    DsError.ThrowExceptionForHR(hr);

                    Rectangle rc = previewBox.ClientRectangle;
                    hr = ivw.SetWindowPosition(0, 0, rc.Right, rc.Bottom);
                    DsError.ThrowExceptionForHR(hr);

                }
                catch { }

                hr = ms.mediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);
                System.Threading.Thread.Sleep(3000);
                hr = ms.mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);

                ResetStats();
                
                //TODO: video bitrate is unknown
                //m_avgBitrate = videoBitrate + (48000 * 2 * 2 * 8); //video + audio: samples per sec * bytes per channel * channels * 8bits

                recStartTime = DateTime.Now;
                fpsStartTime = recStartTime;
                fpsNotDropped = 0;
                statsTimer.Start();

                // recording has started suceffully, do not cleanup
                cleanup = false;
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            finally
            {
                Util.ReleaseComObject(ref audioConfigObj);
                if (cleanup)
                {
                    ms.Reset(false);
                    m_audioCB.Reset();
                    m_videoCB.Reset();
                    m_muxedCB.Reset();
                }
            }

            return true;
        }

        private void StopRecording()
        {
            statsTimer.Stop();

            ms.Reset(false); // leave the input devices in the graph

            m_audioCB.Reset();
            m_videoCB.Reset();
            m_muxedCB.Reset();
        }

        private void cmdRecord_Click(object sender, EventArgs e)
        {
            if (bCmdRecordBusy)
                return;

            bCmdRecordBusy = true;
            cmdRecord.Enabled = false;

            if (bRecording)
            {
                // stop recording
                StopRecording();

                txtRecording.Visible = false;
                cmdRecord.Text = sStartRecording;
                EnableCommandUI(true);
                EnableDeviceUI(true);
                cmdPreview.Enabled = true;
                UpdateDrivesAsync();
                bRecording = false;
            }
            else
            {
                // start recording
                try
                {
                    EnableCommandUI(false);
                    EnableDeviceUI(false);
                    if (StartRecording())
                    {
                        if (!cmdPreview.Checked)
                        {
                            cmdPreview.Enabled = false;
                        }

                        txtRecording.Visible = true;
                        cmdRecord.Text = sStopRecording;

                        bRecording = true;
                    }
                    else
                    {
                        EnableCommandUI(true);
                        EnableDeviceUI(true);
                    }
                }
                catch (COMException ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            cmdRecord.Enabled = true;
            bCmdRecordBusy = false;
        }

        private bool StartDvdRecorder(DriveArray drives)
        {
            if (drives == null)
                return false;

            VideoRecorder recorder = null;
            bool success = false;

            try
            {
                if (!drives.Initialize())
                {
                    return false;
                }

                recorder = new VideoRecorder();
                Debug.Assert(recorder != null);

                foreach (DriveItem drive in drives.Items)
                {
                    recorder.Devices.Add(drive.Device);
                }

                if (recorder.MediaFreeSpace < MIN_FREE_SPACE)
                {
                    MessageBox.Show("Not enough space on the disc(s).");
                    return false;
                }

                if (recorder.IsFinalized)
                {
                    MessageBox.Show("Disc(s) already finalized and cannot be written to.");
                    return false;
                }

                if (recorder.StartAsync())
                {
                    success = true;
                    ms.dvdRecorder = recorder;
                }

                return success;
            }
            finally
            {
                if (!success)
                {
                    drives.Dispose();
                    
                    if (recorder != null)
                        recorder.Dispose();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Util.WM_STOP_CAPTURE)
            {

                System.Diagnostics.Trace.WriteLine("WM_STOP_CAPTURE WParam:" + m.WParam.ToInt32().ToString());

                cmdRecord_Click(null, null);

                int stopReason = m.WParam.ToInt32();

                if (stopReason == -1)
                {
                    MessageBox.Show(this,
                "An error occurred while recording to the disc. The recording has been stopped.",
                "DVDBuilder Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (stopReason == -2)
                {
                    MessageBox.Show(this,
                "The disc is full. The recording has been stopped.", "Device Out of space",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (stopReason >= 0)
                {
                    MessageBox.Show(this,
                "An error occurred encoding captured data. The recording has been stopped.",
                "AVBlocks",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(this,
                "An error occurred while recording. The recording has been stopped.",
                "Unexpected Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else if (m.Msg == WinAPI.WM_DEVICECHANGE && !bRecording && !bCmdRecordBusy)
            {
                int devEvent = m.WParam.ToInt32();
                switch (devEvent)
                {
                    case WinAPI.DBT_DEVICEARRIVAL:
                    case WinAPI.DBT_DEVICEREMOVECOMPLETE:
                        UpdateDrivesAsync();
                        break;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void RecorderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bRecording)
            {
                statsTimer.Stop();

                ms.Reset(true);

                m_audioCB.Reset();
                m_videoCB.Reset();
                m_muxedCB.Reset();

                Thread.Sleep(300);
            }
            else
            {
                ms.Reset(true);
            }
        }

        private void cmdFinalize_Click(object sender, EventArgs e)
        {
            DriveArray drives = GetSelectedDrives();
            if (drives == null)
                return;

            VideoRecorder recorder = new VideoRecorder();
            Debug.Assert(recorder != null);

            bool bUpdateDrives = false;

            try
            {
                cmdFinalize.Enabled = false;

                if (!drives.Initialize())
                {
                    MessageBox.Show("Cannot access disc(s)", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime now = DateTime.Now;

                string label = string.Format("{0}{1:d2}{2:d2}_{3:d2}{4:d2}", now.Year, now.Month, now.Day, now.Hour, now.Minute);

                foreach (DriveItem drive in drives.Items)
                {
                    VRDevice device = drive.Device;
                    recorder.Devices.Add(device);
                    if (device.Type == VRDeviceType.OpticalDisc)
                    {
                        OpticalDiscDeviceConfig config = device.Config as OpticalDiscDeviceConfig;
                        Trace.WriteLine(String.Format("drive {0} label: {1}", config.DriveLetter, config.VolumeLabel));
                        config.VolumeLabel = label;
                    }
                }

                if (!recorder.IsFinalizeSupported)
                {
                    MessageBox.Show("Video cannot be finalized");
                    return;
                }

                if (recorder.IsFinalized)
                {
                    MessageBox.Show("Video already finalized");
                    return;
                }

                if (DialogResult.Yes != MessageBox.Show("Do you want to finalize the video now?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                    return;

                bool res = recorder.FinalizeMedia();
                bUpdateDrives = true;

                if (res)
                {
                    drives.NotifyChanges();

                    if (recorder.IsFinalized)
                    {
                        MessageBox.Show("Video finalized successfully");
                        return;
                    }
                }
                else
                {
                    Trace.WriteLine("FinalizeMedia failed");
                    Util.CheckRecorderDevices(recorder);
                }

                MessageBox.Show("An error occurred while finalizing disc(s).", null,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                drives.Dispose();
                recorder.Dispose();
                if (bUpdateDrives)
                    UpdateDrivesAsync();
                cmdFinalize.Enabled = true;
            }
        }

        private void cmdPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (bRecording && ms.previewRenderer != null)
            {
                IVideoWindow window = ms.graph as IVideoWindow;
                if (cmdPreview.Checked)
                    window.put_Visible(OABool.True);
                else
                    window.put_Visible(OABool.False);
            }
        }

        private DriveArray GetSelectedDrives()
        {
            int selCount = listDrives.CheckedIndices.Count;
            if (selCount > 0 && m_dvdPlugin == null)
            {
                MessageBox.Show("DVD plugin not loaded");
                return null;
            }

            bool bDVD = selCount > 0;
            string folder = editFolder.Text;

            if (folder.Length > 0)
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    MessageBox.Show("Invalid folder");
                    return null;
                }

                if (m_fsPlugin == null)
                {
                    MessageBox.Show("File system plugin not loaded");
                    return null;
                }
            }

            bool bFolder = false;
            if (folder.Length > 0)
                bFolder = true;

            DriveArray drives = null;

            // DVD Drives
            if (bDVD)
            {
                if (drives == null)
                    drives = new DriveArray();

                foreach (int index in listDrives.CheckedIndices)
                {
                    DriveListItem di = (DriveListItem)listDrives.Items[index];

                    VRDevice device = m_dvdPlugin.CreateOpticalDiscDevice(di.Letter, true);
                    Debug.Assert(device != null);

                    drives.Items.Add(new DriveItem(device));
                    // the device is not initialized, so it may not be accessible!
                }
            }

            // Folder
            if (bFolder)
            {
                if (drives == null)
                    drives = new DriveArray();


                VRDevice device = m_fsPlugin.CreateFileSystemDevice(folder);
                Debug.Assert(device != null);

                drives.Items.Add(new DriveItem(device));
                // the device is not initialized, so it may not be accessible!
            }

            return drives;

        }

        private void cmdEraseDisc_Click(object sender, EventArgs e)
        {
            DriveArray drives = GetSelectedDrives();
            if (drives == null)
                return;

            cmdErase.Enabled = false;
            bool bUpdateDrives = false;

            try
            {
                if (!drives.Initialize())
                {
                    MessageBox.Show("Cannot access disc(s)", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int erasableCount = drives.GetErasableCount();

                if (erasableCount > 0)
                {
                    if (DialogResult.Yes != MessageBox.Show("Erase disc(s)?", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        return;

                    bool res = drives.Erase();
                    bUpdateDrives = true;
                    if (res)
                    {
                        MessageBox.Show("Disc(s) erased successfully");
                    }
                    else
                    {
                        MessageBox.Show("Could not erase disc(s)", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                drives.Dispose();
                if (bUpdateDrives)
                    UpdateDrivesAsync();
                cmdErase.Enabled = true;
            }
        }

        class DriveListItem
        {
            public DriveListItem(char letter, string fullName)
            {
                Letter = letter;
                FullName = fullName;
            }

            // single drive
            public char Letter;

            // display text
            public string FullName;

            public override string ToString()
            {
                return FullName;
            }
        }

        class FilterItem
        {
            public FilterItem() { }
            public FilterItem(string friendlyName, string displayName)
            {
                FriendlyName = friendlyName;
                DisplayName = displayName;
            }

            public string FriendlyName;
            public string DisplayName;

            public override string ToString()
            {
                return FriendlyName;
            }
        };

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (editFolder.Text.Length > 0)
                dlg.SelectedPath = editFolder.Text;

            dlg.Description = "Select video output folder";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                editFolder.Text = dlg.SelectedPath;
            }

        }

        private void cmdClearFolder_Click(object sender, EventArgs e)
        {
            editFolder.Text = string.Empty;
        }

        private void cmdVideoInfo_Click(object sender, EventArgs e)
        {
            DriveArray drives = GetSelectedDrives();
            if (drives == null)
                return;

            VideoRecorder recorder = new VideoRecorder();
            Debug.Assert(recorder != null);


            try
            {
                cmdVideoInfo.Enabled = false;

                if (!drives.Initialize())
                {
                    MessageBox.Show("Cannot access disc(s)", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                StringBuilder info = new StringBuilder();

                foreach (DriveItem drive in drives.Items)
                {
                    VRDevice device = drive.Device;
                    recorder.Devices.Add(device);

                    if (device.Type == VRDeviceType.OpticalDisc)
                    {
                        OpticalDiscDeviceConfig config = (OpticalDiscDeviceConfig)device.Config;
                        info.AppendFormat("\r\nDVD Drive {0}:, Volume Label:{1}\r\n",
                            char.ToUpper(config.DriveLetter), config.VolumeLabel);
                    }
                    else if (device.Type == VRDeviceType.FileSystem)
                    {
                        FileSystemDeviceConfig config = (FileSystemDeviceConfig)device.Config;
                        info.AppendFormat("\r\nDVD Video, Folder: {0}\r\n", config.Folder);
                    }

                    IList<Title> titles = recorder.GetTitles(0);
                    if (titles != null)
                    {
                        TimeSpan total = new TimeSpan(0);
                        for (int i = 0; i < titles.Count; ++i)
                        {
                            double sec = titles[i].Duration;
                            TimeSpan dur = new TimeSpan(Convert.ToInt64(sec) * 10000000 /* convert seconds to ticks */);
                            info.AppendFormat("Title {0:d2}, {1:f2} sec, {2}h {3:d2}m {4:d2}s\r\n",
                                i + 1, sec, dur.Hours, dur.Minutes, dur.Seconds);

                            total += dur;
                        }

                        info.AppendFormat("Total: {0}h {1:d2}m {2:d2}s\r\n", total.Hours, total.Minutes, total.Seconds);
                    }
                    else
                    {
                        info.Append("Cannot read titles\r\n");
                    }

                    recorder.Devices.Clear();
                }

                info.Append("\r\n");

                VideoInfoForm vi = new VideoInfoForm();
                vi.Info = info.ToString();
                vi.ShowDialog();
            }
            finally
            {
                drives.Dispose();
                recorder.Dispose();
                cmdVideoInfo.Enabled = true;
            }
        }

        private void EnableCommandUI(bool enable)
        {
            cmdFinalize.Enabled = enable;
            cmdErase.Enabled = enable;
            cmdVideoInfo.Enabled = enable;
            cmdSimulate.Enabled = enable;
        }

        private void EnableDeviceUI(bool enable)
        {
	        listDrives.Enabled = enable;
	        editFolder.Enabled = enable;
	        cmdBrowseFolder.Enabled = enable;
	        cmdClearFolder.Enabled = enable;
        }

        private void cmdSimulate_CheckedChanged(object sender, EventArgs e)
        {
            bool simulate = cmdSimulate.Checked;
            EnableDeviceUI(!simulate);
        }

        // Get all optical drives in the system; put them in a drive array for a parallel work
        private DriveArray GetAllDrives()
        {
            if (m_dvdPlugin == null)
            {
                return null;
            }

            DriveArray drives = new DriveArray();

            DriveInfo[] drivesInfo = DriveInfo.GetDrives();

            foreach (DriveInfo di in drivesInfo)
            {
                if (di.DriveType == DriveType.CDRom)
                {
                    char letter = (char)di.Name[0];
                    VRDevice device = m_dvdPlugin.CreateOpticalDiscDevice(letter, false);
                    Debug.Assert(device != null);

                    drives.Items.Add(new DriveItem(device));
                }
            }

            if (drives.Items.Count == 0)
            {
                return null;
            }

            return drives;
        }

        
        void QueryDrivesProc(object args)
        {
            DriveArray drives = (DriveArray)args;

            drives.Initialize();

            drives.Query();

            UpdateDrivesUI(drives);
        }

        private void UpdateDrivesAsync()
        {
	        DriveArray drives = GetAllDrives();
	        if (drives == null)
		        return;

            // save drive selection
            m_selDrives.Clear();
            foreach (int index in listDrives.CheckedIndices)
            {
                DriveListItem di = (DriveListItem)listDrives.Items[index];

               m_selDrives.Add(di.Letter);
            }

	        // start updating drive information
            txtUpdatingDrives.Visible = true;
	        listDrives.Enabled = false;

            Thread t = new Thread(QueryDrivesProc);
            t.Start(drives);
        }

        delegate void UpdateDrivesUICaller(DriveArray drives);

        private void UpdateDrivesUI(DriveArray drives)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateDrivesUICaller(UpdateDrivesUI), drives);
                return;
            }

            listDrives.Items.Clear();
            txtUpdatingDrives.Visible = false;

            foreach (DriveItem drive in drives.Items)
            {
                VRDevice device = drive.Device;

                StringBuilder driveInfo = new StringBuilder();

                OpticalDiscDeviceConfig config = (OpticalDiscDeviceConfig)device.Config;
                char letter = config.DriveLetter;
                driveInfo.AppendFormat("{0}: ", letter, char.ToUpper(letter));

                if (drive.IsInitialized)
                {
                    // successfully initialized. device attributes
                    if (drive.IsBlank)
                    {
                        driveInfo.Append("Blank");
                    }
                    else
                    {
                        if (drive.VolumeLabel.Length > 0)
                            driveInfo.AppendFormat("{0}", drive.VolumeLabel);
                        else
                            driveInfo.Append("No Label");

                        if (drive.IsVideo)
                        {
                            driveInfo.Append("  (Video)");
                        }
                    }
                }

                listDrives.Items.Add(new DriveListItem(letter, driveInfo.ToString()));

                // restore drive selection
                if (m_selDrives.Contains(letter))
                    listDrives.SetItemChecked(listDrives.Items.Count - 1, true);

            }

            listDrives.Enabled = true;
            drives.Dispose();
        }
    }
}
