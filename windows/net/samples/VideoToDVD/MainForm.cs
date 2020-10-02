using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using PrimoSoftware.DVDBuilder;
using PrimoSoftware.AVBlocks;
using PrimoSoftware.Burner;
using System.IO;

namespace VideoToDVD
{
    public partial class MainForm : Form
    {
        #region Fields
        private Burner m_Burner = new Burner();

        enum Task
        {
            CreateDVDProject,
            CreateVideoTs,
            CreateDiscImage,
            BurnDisc
        }

        class VideoToDVDSettings
        {
            public Task Task;
            public int DeviceIndex;
            public string ImageFileName;

            public string VolumeName;
            public TVSystem TVSystem;
            public string DvdPrjFolder;
            public string VideoTsFolder;
            public List<string> InputFiles = new List<string>();

            public string EncodingPreset
            {
                get
                {
                    return (TVSystem == TVSystem.PAL) ? Preset.Video.DVD.PAL_4x3_MP2 : Preset.Video.DVD.NTSC_4x3_PCM;
                }
            }
        }

        VideoToDVDSettings m_settings = new VideoToDVDSettings();

        #endregion

        #region ctor/dtor
        public MainForm()
        {
            InitializeComponent();
            cbTVSystem.SelectedIndex = 0;

            m_Burner.Status += new EventHandler<Burner.StatusEventArgs>(m_Burner_Status);
            m_Burner.Continue += new EventHandler<CancelEventArgs>(m_Burner_Continue);
            m_Burner.ImageProgress += new EventHandler<Burner.ImageProgressEventArgs>(m_Burner_ImageProgress);
            m_Burner.FileProgress += new EventHandler<Burner.FileProgressEventArgs>(m_Burner_FileProgress);
            m_Burner.FormatProgress += new EventHandler<Burner.FormatEventArgs>(m_Burner_FormatProgress);
            m_Burner.EraseProgress += new EventHandler<Burner.EraseEventArgs>(m_Burner_EraseProgress);

            try
            {
                m_Burner.Open();

                DeviceInfo [] devices = m_Burner.EnumerateDevices();

                if (devices.Length == 0)
                {
                    throw new Exception("No devices found.");
                }

                for (int i = 0; i < devices.Length; i++) 
                {
                    DeviceInfo dev = devices[i];
                    cbDevice.Items.Add(dev);
                }

                // Device combo
                cbDevice.SelectedIndex = 0;
                UpdateDeviceInformation();
            }
            catch (BurnerException bme)
            {
                ShowError(bme.Message);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                // Close Burner
                m_Burner.Close();
                m_Burner = null;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Burner event handlers
        void m_Burner_EraseProgress(object sender, Burner.EraseEventArgs e)
        {
            SetProgress((int)e.Percent);
        }

        void m_Burner_FormatProgress(object sender, Burner.FormatEventArgs e)
        {
            SetProgress((int)e.Percent);
        }

        void m_Burner_FileProgress(object sender, Burner.FileProgressEventArgs e)
        {
            // no progress report
        }

        void m_Burner_ImageProgress(object sender, Burner.ImageProgressEventArgs e)
        {
            if (e.All > 0)
                SetProgress((int)((100 * e.Pos) / e.All));
        }

        void m_Burner_Continue(object sender, CancelEventArgs e)
        {
            e.Cancel = backgroundWorker.CancellationPending;
        }

        void m_Burner_Status(object sender, Burner.StatusEventArgs e)
        {
            LogEvent(e.Message);
        }

        private const int WM_DEVICECHANGE = 0x0219;
        protected override void WndProc(ref Message msg)
        {
            if (WM_DEVICECHANGE == msg.Msg)
                UpdateDeviceInformation();

            base.WndProc(ref msg);
        }

        private void UpdateDeviceInformation()
        {
            if (-1 == cbDevice.SelectedIndex)
                return;

            try
            {
                DeviceInfo dev = (DeviceInfo)cbDevice.SelectedItem;

                // Select device. Exclusive access is not required.
                m_Burner.SelectDevice(dev.Index, false);

                long freeSpace = m_Burner.MediaFreeSpace * (int)BlockSize.Dvd;
                lblFreeSpace.Text = String.Format("{0}GB", ((double)freeSpace / (1e9)).ToString("0.00"));
                lblMediaType.Text = m_Burner.MediaProfileString;

                m_Burner.ReleaseDevice();
            }
            catch (BurnerException bme)
            {
                // Ignore the error when it is DEVICE_ALREADY_SELECTED
                if (Burner.DEVICE_ALREADY_SELECTED == bme.ErrorCode)
                    return;

                m_Burner.ReleaseDevice();
                ShowError(bme.Message);
            }
        }

        private void cbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (-1 != cbDevice.SelectedIndex)
                UpdateDeviceInformation();
        }
        #endregion

        #region helper
        delegate void ShowErrorDelegate(string msg);
        void ShowError(string msg)
        {
            if(InvokeRequired)
            {
                ShowErrorDelegate d = new ShowErrorDelegate(ShowError);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        delegate void LogEventDelegate(string message);
        void LogEvent(string message)
        {
            if (InvokeRequired)
            {
                LogEventDelegate d = new LogEventDelegate(LogEvent);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                ListViewItem lvi = new ListViewItem(DateTime.Now.ToLongTimeString());
                lvi.SubItems.Add(message);
                lvLog.Items.Add(lvi);
                lvLog.EnsureVisible(lvLog.Items.Count - 1);
            }
        }

        void ClearLogEvents()
        {
            lvLog.Items.Clear();
        }

        delegate void SetProgressDelegate(int percent);
        void SetProgress(int percent)
        {
            if(InvokeRequired)
            {
                SetProgressDelegate d = new SetProgressDelegate(SetProgress);
                this.Invoke(d, new object[] { percent });
            }
            else
            {
                percent = Math.Min(percent, 100);
                percent = Math.Max(percent, 0);
                progressBar.Value = percent;
            }
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                e.Cancel = true;
                MessageBox.Show("An operation is in progress. Plese stop the program.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void UpdateUI()
        {
            bool busy = backgroundWorker.IsBusy;
            gbInputFiles.Enabled = !busy;
            gbBurnSettings.Enabled = !busy;
            gbDVDVideoSettings.Enabled = !busy;
            gbIntermediateFiles.Enabled = !busy;
            btnStop.Enabled = busy && !backgroundWorker.CancellationPending;
            btnCreateDVDVideoDisc.Enabled = !busy;
            btnCreateDiscImage.Enabled = !busy;
            btnCreateDVDBProject.Enabled = !busy;
            btnCreateVideoTsFolder.Enabled = !busy;
        }

        private PrimoSoftware.AVBlocks.VideoStreamInfo GetVideoStream(PrimoSoftware.AVBlocks.MediaInfo avinfo)
        {
            foreach (var stream in avinfo.Streams)
            {
                if (stream.MediaType == PrimoSoftware.AVBlocks.MediaType.Video)
                    return (PrimoSoftware.AVBlocks.VideoStreamInfo)stream;
            }
            return null;
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Video files (*.mp4,*.mpg,*.mpeg,*.mod,*.avi,*.wmv,*.mts,*.m2t,*.ts,*.tod,*.m2v,*.m4v,*.webm,*.dat,*.mpe,*.mpeg4,*.ogm)|*.mp4;*.mpg;*.mpeg;*.mod;*.avi;*.wmv;*.mts;*.m2t;*.ts;*.tod;*.m2v;*.m4v;*.webm;*.dat;*.mpe;*.mpeg4;*.ogm|All files (*.*)|*.*";
                dlg.Multiselect = true;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var avinfo = new PrimoSoftware.AVBlocks.MediaInfo();
                    
                        foreach (string fileName in dlg.FileNames)
                        {
                            avinfo.InputFile = fileName;

                            if (avinfo.Load())
                            {
                                var videoInfo = GetVideoStream(avinfo);

                                if (videoInfo != null)
                                {
                                    m_settings.InputFiles.Add(fileName);
                                    ListViewItem lvi = new ListViewItem(fileName);
                                    lvi.SubItems.Add(FormatVideoInfo(videoInfo));
                                    lvFiles.Items.Add(lvi);
                                }
                            }
                        }
                }
            }
        }

        string FormatVideoInfo(PrimoSoftware.AVBlocks.VideoStreamInfo vsi)
        {
            return string.Format("{0}x{1} {2:.00}fps {3:.0}sec.", vsi.FrameWidth, vsi.FrameHeight, vsi.FrameRate, vsi.Duration);
        }

        private void btnClearFiles_Click(object sender, EventArgs e)
        {
            m_settings.InputFiles.Clear();
            lvFiles.Items.Clear();
        }

        private void btnBrowseVideoTsFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtVideoTsFolder.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnBrowseDvdProjectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtDvdProjectFolder.Text = dlg.SelectedPath;
                }
            }
        }

        private void btnCreateDVDVideoDisc_Click(object sender, EventArgs e)
        {
            ClearLogEvents();

            SetSettings();
            m_settings.Task = Task.BurnDisc;

            if (!ValidateTask())
                return;
            
            backgroundWorker.RunWorkerAsync();
            UpdateUI();
        }

        private void btnCreateDiscImage_Click(object sender, EventArgs e)
        {
            ClearLogEvents();

            SetSettings();
            m_settings.ImageFileName = string.Empty;
            m_settings.Task = Task.CreateDiscImage;

            if (!ValidateTask())
                return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "ISO files (*.iso)|*.iso|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    m_settings.ImageFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }

            backgroundWorker.RunWorkerAsync();
            UpdateUI();
        }

        private void btnCreateDVDBProject_Click(object sender, EventArgs e)
        {
            ClearLogEvents();

            SetSettings();
            m_settings.Task = Task.CreateDVDProject;

            if (!ValidateTask())
                return;

            backgroundWorker.RunWorkerAsync();
            UpdateUI();
        }

        private void btnCreateVideoTsFolder_Click(object sender, EventArgs e)
        {
            ClearLogEvents();

            SetSettings();
            m_settings.Task = Task.CreateVideoTs;

            if (!ValidateTask())
                return;

            backgroundWorker.RunWorkerAsync();
            UpdateUI();
        }

        private bool ValidateTask()
        {
            try
            {
                if (m_settings.InputFiles.Count == 0)
                {
                    ShowError("No input files specified.");
                    return false;
                }

                if (m_settings.Task == Task.BurnDisc)
                {
                    if (!ValidateMedia())
                        return false;
                }

                if (string.IsNullOrEmpty(m_settings.DvdPrjFolder) || !System.IO.Directory.Exists(m_settings.DvdPrjFolder))
                {
                    ShowError("Please select DVDB project folder.");
                    return false;
                }

                if ((m_settings.Task == Task.CreateVideoTs) ||
                    (m_settings.Task == Task.CreateDiscImage) ||
                    (m_settings.Task == Task.BurnDisc))
                {
                    if (string.IsNullOrEmpty(m_settings.VideoTsFolder) || !System.IO.Directory.Exists(m_settings.VideoTsFolder))
                    {
                        ShowError("Please select VIDEO_TS project folder.");
                        return false;
                    }

                    if (!IsDirectoryEmpty(m_settings.VideoTsFolder))
                    {
                        string msg = string.Format("The folder  {0}  is not empty. All files and folders within that folder will be deleted. Do you want to continue?", m_settings.VideoTsFolder);
                        if (MessageBox.Show(this, msg, "Delete Files!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            return false;

                        CleanDirectory(m_settings.VideoTsFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return false;
            }

            return true;
        }

        bool IsDirectoryEmpty(string path)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
            return (di.GetFiles().Length == 0) && (di.GetDirectories().Length == 0);
        }

        void CleanDirectory(string path)
        { 
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }

            foreach (System.IO.DirectoryInfo sdi in di.GetDirectories())
            {
                sdi.Delete(true);
            }
        }

        private void SetSettings()
        {
            if (cbDevice.SelectedIndex >= 0)
                m_settings.DeviceIndex = ((DeviceInfo)cbDevice.SelectedItem).Index;

            m_settings.VolumeName = txtVolumeName.Text;
            m_settings.VideoTsFolder = txtVideoTsFolder.Text;
            m_settings.DvdPrjFolder = txtDvdProjectFolder.Text;

            if (cbTVSystem.SelectedIndex == 0)
            {
                m_settings.TVSystem = TVSystem.NTSC;
            }
            else
            {
                m_settings.TVSystem = TVSystem.PAL;
            }
        }

        bool ValidateMedia()
        {
            if (-1 == cbDevice.SelectedIndex)
                return false;

            try
            {
                DeviceInfo dev = (DeviceInfo)cbDevice.SelectedItem;
                m_Burner.SelectDevice(dev.Index, true);

                switch (m_Burner.MediaProfile)
                {
                    case MediaProfile.DvdPlusR:
                    case MediaProfile.DvdPlusRDL:
                    case MediaProfile.DvdMinusRSeq:
                    case MediaProfile.DvdMinusRwSeq:
                    case MediaProfile.DvdMinusRDLJump:
                    case MediaProfile.DvdMinusRDLSeq:
                        if (!m_Burner.MediaIsBlank)
                        {
                            ShowError("The media is not blank. Please insert a blank DVD media.");
                            return false;
                        }
                        break;

                    case MediaProfile.DvdPlusRw:
                    case MediaProfile.DvdMinusRwRo:
                    case MediaProfile.DvdRam:
                        // random rewriteable, no MediaIsBlank validation
                        break;

                    default:
                        ShowError("There is no DVD media into the drive. Please insert a DVD media (DVD-R, DVD+R, DVD-RW, DVD+RW, DVD-RAM).");
                        return false;
                }
            }
            catch (BurnerException bme)
            {
                ShowError(bme.Message);
                return false;
            }
            finally
            {
                m_Burner.ReleaseDevice();
            }

            return true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            UpdateUI();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateUI();
            UpdateDeviceInformation();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<string> encodedFiles = EncodeInputFiles(m_settings.DvdPrjFolder);
                string dvdbProjectFile = CreateDVDBProject(encodedFiles, m_settings.DvdPrjFolder);

                if (backgroundWorker.CancellationPending)
                    return;

                switch(m_settings.Task)
                {
                    case Task.CreateVideoTs:
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);
                        break;

                    case Task.CreateDiscImage:
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);

                        if (backgroundWorker.CancellationPending)
                            return;

                        CreateDiscImage(m_settings.VideoTsFolder);
                        break;

                    case Task.BurnDisc:
                        CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);

                        if (backgroundWorker.CancellationPending)
                            return;

                        BurnDisc(m_settings.VideoTsFolder);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                LogEvent(ex.Message);
            }
        }

        private void BurnDisc(string videoTsFolder)
        {
            LogEvent("Burn started.");

            m_Burner.SelectDevice(m_settings.DeviceIndex, true);

            BurnSettings bs = new BurnSettings();
            bs.SourceFolder = videoTsFolder;
            bs.VolumeLabel = m_settings.VolumeName;

            bs.ImageType = ImageType.UdfIso;
            bs.VideoDVD = true;
            bs.WriteMethod = WriteMethod.DvdIncremental;
            bs.WriteSpeedKB = m_Burner.MaxWriteSpeedKB;

            bs.Simulate = false;
            bs.CloseDisc = true;
            bs.Eject = true;

            m_Burner.Burn(bs);

            m_Burner.ReleaseDevice();

            LogEvent("Burn completed.");
        }

        private void CreateDiscImage(string videoTsFolder)
        {
            LogEvent("Create image started.");

            CreateImageSettings cis = new CreateImageSettings();
            cis.SourceFolder = videoTsFolder;
            cis.VolumeLabel = m_settings.VolumeName;

            cis.ImageType = ImageType.UdfIso;
            cis.VideoDVD = true;
            cis.ImageFile = m_settings.ImageFileName;

            m_Burner.CreateImage(cis);

            LogEvent("Create image completed.");
        }

        #region EncoderException
        public class EncoderException: Exception
        {
            PrimoSoftware.AVBlocks.ErrorInfo m_error;
            

            public EncoderException(PrimoSoftware.AVBlocks.ErrorInfo errorInfo)
            {
                m_error = errorInfo;
            }

            public override string Message
            {
                get
                {
                    return m_error.Message;
                }
            }
        }
        #endregion

        private List<string> EncodeInputFiles(string tmpFolder)
        {
            LogEvent("Encoding started.");

            List<string> encodedFiles = new List<string>();

            for (int i = 0; i < m_settings.InputFiles.Count; i++)
            {
                using (var transcoder = new Transcoder())
                {
                    // In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
                    transcoder.AllowDemoMode = true;

                    transcoder.OnContinue += new EventHandler<TranscoderContinueEventArgs>(transcoder_OnContinue);
                    transcoder.OnProgress += new EventHandler<TranscoderProgressEventArgs>(transcoder_OnProgress);

                    string inputFile = m_settings.InputFiles[i];
                    string outputFile = System.IO.Path.Combine(tmpFolder, System.IO.Path.GetFileNameWithoutExtension(inputFile) + ".mpg");

                    if (File.Exists(outputFile))
                        File.Delete(outputFile);

                    LogEvent("Encoding " + outputFile);

                    var outputSocket = MediaSocket.FromPreset(m_settings.EncodingPreset);
                    outputSocket.File = outputFile;
                    transcoder.Outputs.Add(outputSocket);

                    var inputSocket = new MediaSocket();
                    inputSocket.File = inputFile;
                    transcoder.Inputs.Add(inputSocket);

                    if (!transcoder.Open())
                        throw new EncoderException(transcoder.Error);

                    if (!transcoder.Run())
                        throw new EncoderException(transcoder.Error);

                    encodedFiles.Add(outputFile);
                }
            }

            LogEvent("Encoding completed.");

            return encodedFiles;
        }

        void transcoder_OnProgress(object sender, PrimoSoftware.AVBlocks.TranscoderProgressEventArgs e)
        {
            if (e.TotalTime > 0)
            {
                int percent = (int)((e.CurrentTime * 100) / e.TotalTime);
                SetProgress(percent);
            }
        }

        void transcoder_OnContinue(object sender, PrimoSoftware.AVBlocks.TranscoderContinueEventArgs e)
        {
            e.Continue = !backgroundWorker.CancellationPending;
        }
        
        private void CreateVideoTs(string projectFile, string outputFolder)
        {
            LogEvent("DVD-Video building started.");

            using (DVDBuilder dvdBuilder = new DVDBuilder())
            {
                dvdBuilder.ProjectFile = projectFile;
                dvdBuilder.OutputFolder = outputFolder;

                dvdBuilder.OnContinue += new EventHandler<PrimoSoftware.DVDBuilder.DVDBuilderContinueEventArgs>(dvdBuilder_OnContinue);
                dvdBuilder.OnProgress += new EventHandler<PrimoSoftware.DVDBuilder.DVDBuilderProgressEventArgs>(dvdBuilder_OnProgress);
                dvdBuilder.OnStatus += new EventHandler<PrimoSoftware.DVDBuilder.DVDBuilderStatusEventArgs>(dvdBuilder_OnStatus);

                if (!dvdBuilder.Build())
                    throw new DVDBuilderException(dvdBuilder.Error.Code, (int)dvdBuilder.Error.Facility, dvdBuilder.Error.Hint);
            }

            LogEvent("DVD-Video building completed.");
        }

        private string CreateDVDBProject(List<string> encodedFiles, string encodedFilesTmpFolder)
        {
            string projectFile = System.IO.Path.Combine(encodedFilesTmpFolder, "project.xml");

            DVDBuilderProject project = new DVDBuilderProject();
            project.TVSystem = m_settings.TVSystem;

            project.Create(projectFile, encodedFiles, encodedFilesTmpFolder);
            return projectFile;
        }

        #region DVDBuilderException
        public class DVDBuilderException: Exception
        {
            string m_message;
            int m_error;
            int m_errorFacility;
            string m_hint;

            public DVDBuilderException(int error, int errorFacility, string hint)
            {
                m_error = error;
                m_errorFacility = errorFacility;
                m_hint = hint;

                switch (m_errorFacility)
                {
                    case (int)PrimoSoftware.DVDBuilder.ErrorFacility.Success:
                        m_message = "Success.";
                        return;

                    case (int)PrimoSoftware.DVDBuilder.ErrorFacility.SystemWindows:
                        System.ComponentModel.Win32Exception sysex = new System.ComponentModel.Win32Exception(m_error);
                        m_message = "System error: " + sysex.Message;
                        return;

                    case (int)PrimoSoftware.DVDBuilder.ErrorFacility.DVDBuilder:
                        m_message = "DVDBuilderError: " + ((PrimoSoftware.DVDBuilder.DVDBuilderError)m_error).ToString();
                        return;

                    default:
                        m_message = "Unknown error.";
                        return;
                }
            }

            public override string Message
            {
                get
                {
                    return m_message;
                }
            }

            public int ErrorFacility
            {
                get
                {
                    return m_errorFacility;
                }
            }

            public int Error
            {
                get
                {
                    return m_error;
                }
            }
        }
        #endregion

        void dvdBuilder_OnStatus(object sender, PrimoSoftware.DVDBuilder.DVDBuilderStatusEventArgs e)
        {
            switch (e.Status)
            {
                case PrimoSoftware.DVDBuilder.DVDBuilderStatus.WritingVOB:
                    LogEvent("WritingVOB");
                    break;
                case PrimoSoftware.DVDBuilder.DVDBuilderStatus.WritingIFO:
                    LogEvent("WritingIFO");
                    break;
            }
        }

        void dvdBuilder_OnProgress(object sender, PrimoSoftware.DVDBuilder.DVDBuilderProgressEventArgs e)
        {
            SetProgress(e.Percent);
        }

        void dvdBuilder_OnContinue(object sender, PrimoSoftware.DVDBuilder.DVDBuilderContinueEventArgs e)
        {
            e.Continue = !backgroundWorker.CancellationPending;
        }
    }
}
