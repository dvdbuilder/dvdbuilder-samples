using System;
using System.ComponentModel;
using System.IO;
using System.Collections;
using PrimoSoftware.Burner;

namespace VideoToDVD
{
    /// <summary>
    /// Container for device information
    /// </summary>
	public struct DeviceInfo
	{
        /// <summary>
        /// Device index
        /// </summary>
		public int Index;

        /// <summary>
        /// Device description
        /// </summary>
		public string Title;
        
        /// <summary>
        /// Returns string representation of this object
        /// </summary>
		public override string ToString() 
		{ 
			return Title; 
		}
	};

    /// <summary>
    /// Container for speed information
    /// </summary>
	public struct SpeedInfo
	{
		public double TransferRateKB;
		public double TransferRate1xKB;
		public override string ToString() 
		{ 
			return string.Format("{0}x", Math.Round((double)TransferRateKB / TransferRate1xKB, 1));
		}
	};

	public class Burner
	{
		#region Construct / Finalize
		public Burner()
		{
		}

		~Burner()
		{
			// Close
			if (m_isOpen)
				Close();
		}
		#endregion

		#region Public Events
        public class StatusEventArgs: EventArgs
        {
            public StatusEventArgs(string message)
            {
                Message = message;
            }

            public string Message;
        }

        public class ImageProgressEventArgs: EventArgs
        {
            public ImageProgressEventArgs(long pos, long all)
            {
                Pos = pos;
                All = all;
            }

            public long Pos;
            public long All;
        }

        public class FileProgressEventArgs: EventArgs
        {
            public FileProgressEventArgs(int file, string fileName, int percentCompleted)
            {
                FileNumber = file;
                FileName = fileName;
                PpercentCompleted = percentCompleted;
            }

            public int FileNumber;
            public string FileName;
            public int PpercentCompleted;
        }


        public class FormatEventArgs: EventArgs
        {
            public FormatEventArgs(double percent)
            {
                Percent = percent;
            }

            public double Percent;
        }

        public class EraseEventArgs: EventArgs
        {
            public EraseEventArgs(double percent)
            {
                Percent = percent;
            }

            public double Percent;
        }

        public event EventHandler<StatusEventArgs> Status;
        public event EventHandler<ImageProgressEventArgs> ImageProgress;
        public event EventHandler<FileProgressEventArgs> FileProgress;
        public event EventHandler<FormatEventArgs> FormatProgress;
        public event EventHandler<EraseEventArgs>  EraseProgress;
        public event EventHandler<CancelEventArgs> Continue;

        void FireStatus(string message)
        {
            if (Status != null)
                Status(this, new StatusEventArgs(message));
        }

        void FireImageProgress(long pos, long all)
        {
            if (ImageProgress != null)
                ImageProgress(this, new ImageProgressEventArgs(pos, all));
        }

        void FireFileProgress(int file, string fileName, int percentCompleted)
        {
            if (FileProgress != null)
                FileProgress(this, new FileProgressEventArgs(file, fileName, percentCompleted));
        }

        void FireFormatProgress(double percent)
        {
            if (FormatProgress != null)
                FormatProgress(this, new FormatEventArgs(percent));
        }

        void FireEraseProgress(double percent)
        {
            if (EraseProgress != null)
                EraseProgress(this, new EraseEventArgs(percent));
        }
		#endregion
		
		#region Public Properties
		public bool IsOpen 
		{ 
			get 
			{ 
					return m_isOpen; 
			}	
		}

		public bool MediaIsBlank
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.MediaIsBlank;
			}
		}

		public bool MediaIsFullyFormatted
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				// Get media profile
				MediaProfile mp = m_device.MediaProfile;

				// DVD+RW
				if (MediaProfile.DvdPlusRw == mp)
					return (BgFormatStatus.Completed == m_device.BgFormatStatus);

				// DVD-RW for Restricted Overwrite
				if (MediaProfile.DvdMinusRwRo == mp)
					return (m_device.MediaFreeSpace == m_device.MediaCapacity);

				return false;
			}
		}

		public int DeviceCacheSize
		{
			get
			{
                if (null == m_device)
                    throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.InternalCacheCapacity;
			}
		}

		public int DeviceCacheUsedSpace
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.InternalCacheUsedSpace;
			}
		}

		public int WriteTransferKB
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.WriteTransferRate;
			}
		}

		public long MediaFreeSpace
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.MediaFreeSpace;
			}
		}

		public int MaxWriteSpeedKB
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.MaxWriteSpeedKB;
			}
		}

		public PrimoSoftware.Burner.MediaProfile MediaProfile
		{
			get
			{
				if (null == m_device)
					throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

				return m_device.MediaProfile;
			}
		}

		public string MediaProfileString
		{
			get
			{
				PrimoSoftware.Burner.MediaProfile profile = this.MediaProfile;
				switch(profile)
				{
					case MediaProfile.CdRom:
						return "CD-ROM. Read only CD.";

					case MediaProfile.CdR:
						return "CD-R. Write once CD.";

					case MediaProfile.CdRw:
						return "CD-RW. Re-writable CD.";

					case MediaProfile.DvdRom:
						return "DVD-ROM. Read only DVD.";

					case MediaProfile.DvdMinusRSeq:
						return "DVD-R Sequential Recording. Write once DVD.";

					case MediaProfile.DvdMinusRDLSeq:
						return "DVD-R DL 8.54GB for Sequential Recording. Write once DVD.";

					case MediaProfile.DvdMinusRDLJump:
						return "DVD-R DL 8.54GB for Layer Jump Recording. Write once DVD.";

					case MediaProfile.DvdRam:
						return "DVD-RAM ReWritable DVD.";

					case MediaProfile.DvdMinusRwRo:
						return "DVD-RW Restricted Overwrite ReWritable. ReWritable DVD using restricted overwrite.";

					case MediaProfile.DvdMinusRwSeq:
						return "DVD-RW Sequential Recording ReWritable. ReWritable DVD using sequential recording.";

					case MediaProfile.DvdPlusRw:
					{
						BgFormatStatus fmt = m_device.BgFormatStatus;
						switch(fmt)
						{
							case BgFormatStatus.NotFormatted:
								return "DVD+RW ReWritable DVD. Not formatted.";
							case BgFormatStatus.Partial:
								return "DVD+RW ReWritable DVD. Partially formatted.";
							case BgFormatStatus.Pending:
								return "DVD+RW ReWritable DVD. Background formatting is pending ...";
							case BgFormatStatus.Completed:
								return "DVD+RW ReWritable DVD. Formatted.";
						}
						return "DVD+RW ReWritable DVD.";
					}

					case MediaProfile.DvdPlusR:
						return "DVD+R. Write once DVD.";

					case MediaProfile.DvdPlusRDL:
						return "DVD+R DL 8.5GB. Write once DVD.";

					default:
						return "Unknown Profile.";
				}
			}
		}

		#endregion

		#region Public Methods
		public void Open()
		{	
			if (m_isOpen)
				return;

			m_engine = new Engine();
			if (!m_engine.Initialize()) 
			{
				m_engine.Dispose();
				m_engine = null;

				throw new BurnerException(ENGINE_INITIALIZATION, ENGINE_INITIALIZATION_TEXT);
			}

			m_isOpen = true;
		}

		public void Close()
		{
			if (null != m_device)
				m_device.Dispose();
			m_device = null;

			if (null != m_engine)
			{
				m_engine.Shutdown();
				m_engine.Dispose();
			}
			m_engine = null;

			m_isOpen = false;
		}

		public DeviceInfo[] EnumerateDevices()
		{
			if (!m_isOpen)
				throw new BurnerException(BURNER_NOT_OPEN, BURNER_NOT_OPEN_TEXT);

			m_deviceArray.Clear();

            using (var enumerator = m_engine.CreateDeviceEnumerator())
            {
                int devices = enumerator.Count;
                if (0 == devices)
                {
                    throw new BurnerException(NO_DEVICES, NO_DEVICES_TEXT);
                }

                for (int i = 0; i < devices; i++)
                {
                    using (var device = enumerator.CreateDevice(i))
                    {
                        if (null != device)
                        {
                            DeviceInfo dev = new DeviceInfo();
                            dev.Index = i;
                            dev.Title = GetDeviceTitle(device);
                            m_deviceArray.Add(dev);
                        }
                    }
                }
            }

			return (DeviceInfo[])m_deviceArray.ToArray(typeof(DeviceInfo));
		}

		public void SelectDevice(int deviceIndex, bool exclusive)
		{
            if (null != m_device)
                throw new BurnerException(DEVICE_ALREADY_SELECTED, DEVICE_ALREADY_SELECTED_TEXT);

            using (var enumerator = m_engine.CreateDeviceEnumerator())
            {
                Device dev = enumerator.CreateDevice(deviceIndex, exclusive);
                if (null == dev)
                    throw new BurnerException(INVALID_DEVICE_INDEX, INVALID_DEVICE_INDEX_TEXT);

                m_device = dev;
            }
		}

		public void ReleaseDevice()
		{
			if (null != m_device)
				m_device.Dispose();

			m_device = null;
		}

		public SpeedInfo [] EnumerateWriteSpeeds()
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

			m_speedArray.Clear();

            var speeds = m_device.GetWriteSpeeds();

            for (int i = 0; i < speeds.Count; i++)
            {
                SpeedDescriptor speed = speeds[i];

                SpeedInfo speedInfo = new SpeedInfo();
                speedInfo.TransferRateKB = speed.TransferRateKB;
                if (m_device.MediaIsDVD)
                {
                    speedInfo.TransferRate1xKB = Speed1xKB.DVD;
                }
                else if (m_device.MediaIsCD)
                {
                    speedInfo.TransferRate1xKB = Speed1xKB.CD;
                }
                else
                {
                    speedInfo.TransferRate1xKB = Speed1xKB.BD;
                }

                m_speedArray.Add(speedInfo);

            }

			return (SpeedInfo[])m_speedArray.ToArray(typeof(SpeedInfo));
		}

        public long CalculateImageSize(BurnSettings settings)
		{
            using (var dataDisc = new DataDisc())
            {
                dataDisc.ImageType = settings.ImageType;
                SetVolumeProperties(dataDisc, settings.VolumeLabel);
                SetImageLayoutFromFolder(dataDisc, settings.VideoDVD, settings.SourceFolder);
                return dataDisc.ImageSizeInBytes;
            }
		}

		public void Eject()
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

			m_device.Eject(true);
		}

		public void CloseTray()
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

			m_device.Eject(false);
		}

		public void Burn(BurnSettings settings) 
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

            using (var dataDisc = new DataDisc())
            {
                // Add event handlers
                dataDisc.OnStatus += new EventHandler<DataDiscStatusEventArgs>(DataDisc_OnStatus);
                dataDisc.OnFileStatus += new EventHandler<DataDiscFileStatusEventArgs>(DataDisc_OnFileStatus);
                dataDisc.OnProgress += new EventHandler<DataDiscProgressEventArgs>(DataDisc_OnProgress);
                dataDisc.OnContinueBurn += new EventHandler<DataDiscContinueEventArgs>(DataDisc_OnContinueBurn);

                FormatMedia(m_device);

                m_device.WriteSpeedKB = settings.WriteSpeedKB;

                dataDisc.Device = m_device;
                dataDisc.SimulateBurn = settings.Simulate;
                dataDisc.WriteMethod = settings.WriteMethod;
                dataDisc.CloseDisc = settings.CloseDisc;

                // Set the session start address. This must be done before intializing the file system.
                dataDisc.SessionStartAddress = m_device.NewSessionStartAddress;

                // Set burning parameters
                dataDisc.ImageType = settings.ImageType;
                SetVolumeProperties(dataDisc, settings.VolumeLabel);
                // Set image layout
                SetImageLayoutFromFolder(dataDisc, settings.VideoDVD, settings.SourceFolder);

                if (!dataDisc.WriteToDisc(true))
                    throw new BurnerException(dataDisc.Error);

                if (settings.Eject)
                    m_device.Eject(true);
            }
		}

        public void CreateImage(CreateImageSettings settings)
        {
            using (var data = new DataDisc())
            {
                // Add event handlers
                data.OnStatus += new EventHandler<DataDiscStatusEventArgs>(DataDisc_OnStatus);
                data.OnFileStatus += new EventHandler<DataDiscFileStatusEventArgs>(DataDisc_OnFileStatus);
                data.OnProgress += new EventHandler<DataDiscProgressEventArgs>(DataDisc_OnProgress);
                data.OnContinueBurn += new EventHandler<DataDiscContinueEventArgs>(DataDisc_OnContinueBurn);

                data.ImageType = settings.ImageType;
                SetVolumeProperties(data, settings.VolumeLabel);
                // Create image file system
                SetImageLayoutFromFolder(data, settings.VideoDVD, settings.SourceFolder);

                // Create the image file
                if (!data.CreateImageFile(settings.ImageFile))
                    throw new BurnerException(data.Error);
            }
        }

		public void Erase(EraseSettings settings) 
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

			MediaProfile mp = m_device.MediaProfile;

			if (MediaProfile.DvdMinusRwSeq != mp && MediaProfile.DvdMinusRwRo != mp && MediaProfile.CdRw != mp)
				throw new BurnerException(ERASE_NOT_SUPPORTED, ERASE_NOT_SUPPORTED_TEXT);
		
			if (m_device.MediaIsBlank && !settings.Force)
				return;

            m_device.OnErase += new EventHandler<DeviceEraseEventArgs>(Device_Erase);

            EraseType et = EraseType.Minimal;

            if (settings.Quick)
            {
                et = EraseType.Minimal;
            }
            else
            {
                et = EraseType.Disc;
            }

            bool bRes = m_device.Erase(et);

			m_device.OnErase -= Device_Erase;

			if (!bRes)
				throw new BurnerException(m_device.Error);

			// Refresh to reload disc information
			m_device.Refresh();
		}

		public void Format(FormatSettings settings) 
		{
			if (null == m_device)
				throw new BurnerException(NO_DEVICE, NO_DEVICE_TEXT);

			MediaProfile mp = m_device.MediaProfile;

			if (MediaProfile.DvdMinusRwSeq != mp && MediaProfile.DvdMinusRwRo != mp && MediaProfile.DvdPlusRw != mp)
				throw new BurnerException(FORMAT_NOT_SUPPORTED, FORMAT_NOT_SUPPORTED_TEXT);

            m_device.OnFormat += new EventHandler<DeviceFormatEventArgs>(Device_Format);

			bool bRes = true;
			switch(mp)
			{
				case MediaProfile.DvdMinusRwRo:
                    if (settings.Quick)
                    {
                        bRes = m_device.Format(FormatType.DvdMinusRwQuick);
                    }
                    else
                    {
                        bRes = m_device.Format(FormatType.DvdMinusRwFull);
                    }
				break;
				case MediaProfile.DvdMinusRwSeq:
                    if (settings.Quick)
                    {
                        bRes = m_device.Format(FormatType.DvdMinusRwQuick);
                    }
                    else
                    {
                        bRes = m_device.Format(FormatType.DvdMinusRwFull);
                    }
				break;

				case MediaProfile.DvdPlusRw:
				{
					BgFormatStatus fmt = m_device.BgFormatStatus;
					switch(fmt)
					{
						case BgFormatStatus.Completed:
							if (settings.Force)
								bRes = m_device.Format(FormatType.DvdPlusRwFull, 0, !settings.Quick);
						break;
						case BgFormatStatus.NotFormatted:
							bRes = m_device.Format(FormatType.DvdPlusRwFull, 0, !settings.Quick);
						break;
						case BgFormatStatus.Partial:
							bRes = m_device.Format(FormatType.DvdPlusRwRestart, 0, !settings.Quick);
						break;
					}
				}
				break;
			}

			m_device.OnFormat -= Device_Format;

			if (!bRes)
				throw new BurnerException(m_device.Error);

			// Refresh to reload disc information
			m_device.Refresh();
		}
		#endregion
		
		#region Device Event Handlers

        void Device_Format(object sender, DeviceFormatEventArgs e)
        {
            FireFormatProgress(e.Progress);
        }

        public void Device_Erase(Object sender, DeviceEraseEventArgs e)
		{
            FireEraseProgress(e.Progress);
		}

		#endregion

		#region DataDisc Event Handlers
		public void DataDisc_OnStatus(Object sender, DataDiscStatusEventArgs e)
		{
            FireStatus(GetDataDiscStatusString(e.Status));
		}

		public void DataDisc_OnFileStatus(Object sender, DataDiscFileStatusEventArgs e)
		{
            FireFileProgress(e.FileNumber, e.FileName, e.PercentWritten);
		}

        public void DataDisc_OnProgress(Object sender, DataDiscProgressEventArgs e)
		{
            FireImageProgress(e.Position, e.All);
		}

		public void DataDisc_OnContinueBurn(Object sender, DataDiscContinueEventArgs e)
		{
			if (null == Continue)
				return;

            CancelEventArgs args = new CancelEventArgs();
            Continue(this, args);

            e.Continue = !args.Cancel;
		}
		#endregion

		#region Private Methods
			private string GetDeviceTitle(Device device)
			{
				return String.Format("({0}:) - {1}", device.DriveLetter, device.Description);
			}


			private void CreateFileTree(DataFile currentDirectory, string currentPath)
			{
                const ImageType allImageTypes = (ImageType.Iso9660 | ImageType.Joliet | ImageType.Udf);

				ArrayList filesAndDirs = new ArrayList();
				filesAndDirs.AddRange(Directory.GetFiles(currentPath, "*"));
				filesAndDirs.AddRange(Directory.GetDirectories(currentPath, "*"));

				foreach (string path in filesAndDirs)
				{
					if (Directory.Exists(path))
					{
						// Get directory information
						DirectoryInfo di = new DirectoryInfo(path);

						// Create a new directory
						DataFile newDirectory = new DataFile(); 

							newDirectory.IsDirectory = true;
							newDirectory.LongFilename = di.Name;
							
							newDirectory.FilePath = di.Name;
							newDirectory.FileTime = di.CreationTime;
                            
                            if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                                newDirectory.HiddenMask = (int)allImageTypes;
                                                   
							// Call CreateFileTree recursively to process all the files from the new directory
							CreateFileTree(newDirectory, di.FullName);

							// Add the new directory to the image tree
							currentDirectory.Children.Add(newDirectory);
					} 
					else
					{
						// Get file information
						FileInfo fi = new FileInfo(path);
						// Create a new file
						DataFile newFile = new DataFile();
                        newFile.IsDirectory = false;
                        newFile.LongFilename = fi.Name;

                        newFile.FilePath = fi.FullName;
                        newFile.FileTime = fi.CreationTime;

                        if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                            newFile.HiddenMask = (int)allImageTypes;

                        // Add the new file to the image tree
                        currentDirectory.Children.Add(newFile);
					}
				}
			}

			private void SetImageLayoutFromFolder(DataDisc data, bool isVideoDVD, string sourceFolder)
			{
				if (isVideoDVD)
				{
                    data.DvdVideo = true;
					data.CloseDisc = true;
				}

				DataFile fileSystemRoot = new DataFile();

                // Create directory structure

                // Set up the root of the file system
                fileSystemRoot.IsDirectory = true;
                fileSystemRoot.LongFilename = "\\";
                fileSystemRoot.FilePath = "\\";

                // Import all files
                CreateFileTree(fileSystemRoot, sourceFolder);

                // Set image layout
                if (isVideoDVD)
                {
                    using (var dvd = new VideoDVD())
                    {
                        // Pass the raw layout to VideoDVD
                        if (!dvd.SetImageLayout(fileSystemRoot))
                        {
                            throw new BurnerException(dvd.Error);
                        }

                        // Get the correct dvd layout
                        if (!data.SetImageLayout(dvd.ImageLayout))
                        {
                            throw new BurnerException(data.Error);
                        }
                    }
                }
                else
                {
                    if (!data.SetImageLayout(fileSystemRoot))
                        throw new BurnerException(data.Error);
                }
			}

			private void SetVolumeProperties(DataDisc data, string volumeLabel)
			{
				// Sample settings. Replace with your own data or leave empty

                data.UdfVolumeProps.VolumeLabel = volumeLabel;
                data.IsoVolumeProps.VolumeLabel = volumeLabel;
                data.JolietVolumeProps.VolumeLabel = volumeLabel;

                data.UdfVolumeProps.VolumeSet = "SET";
                data.IsoVolumeProps.VolumeSet = "SET";
                data.JolietVolumeProps.VolumeSet = "SET";

                data.IsoVolumeProps.SystemID = "WINDOWS";
                data.JolietVolumeProps.SystemID = "WINDOWS";

                data.IsoVolumeProps.Publisher = "PUBLISHER";
                data.JolietVolumeProps.Publisher = "PUBLISHER";

                data.IsoVolumeProps.DataPreparer = "PREPARER";
                data.JolietVolumeProps.DataPreparer = "PREPARER";

                data.IsoVolumeProps.Application = "DVDBURNER";
                data.JolietVolumeProps.Application = "DVDBURNER";

                var now = DateTime.Now;
                data.UdfVolumeProps.CreationTime = now;
                data.IsoVolumeProps.CreationTime = now;
                data.JolietVolumeProps.CreationTime = now;
			}

			private string GetDataDiscStatusString(DataDiscStatus status)
			{
				switch(status)
				{
					case DataDiscStatus.BuildingFileSystem:
						return "Building filesystem...";
					case DataDiscStatus.LoadingImageLayout:
						return "Loading image layout...";
					case DataDiscStatus.WritingFileSystem:
						return "Writing filesystem...";
					case DataDiscStatus.WritingImage:
						return "Writing image...";
					case DataDiscStatus.CachingSmallFiles:
						return "Caching small files...";
					case DataDiscStatus.CachingNetworkFiles:
						return "Caching network files...";
					case DataDiscStatus.CachingCDRomFiles:
						return "Caching CDROM files...";
					case DataDiscStatus.Initializing:
						return "Initializing and writing lead-in...";
					case DataDiscStatus.Writing:
						return "Writing...";
					case DataDiscStatus.WritingLeadOut:
						return "Writing lead-out and flushing cache...";
				}

				return "Unknown status...";
			}

			bool FormatMedia(Device dev)
			{
				dev.OnErase += Device_Erase;
				dev.OnFormat += Device_Format;

				switch(dev.MediaProfile)
				{
					// DVD+RW (needs to be formatted before the disc can be used)
					case MediaProfile.DvdPlusRw:
					{
                        FireStatus("Formatting...");

						switch(dev.BgFormatStatus)
						{
							case BgFormatStatus.NotFormatted:
								dev.Format(FormatType.DvdPlusRwFull);
							break;

							case BgFormatStatus.Partial:
								dev.Format(FormatType.DvdPlusRwRestart);
							break;
						}
					}
					break;
				}

				dev.OnErase -= Device_Erase;
				dev.OnFormat -= Device_Format;

				// Must be DVD-R, DVD+R
				return true;
			}
		#endregion
	
		#region Private Property Members
			private bool m_isOpen = false;
			private ArrayList m_deviceArray = new ArrayList();
			private ArrayList m_speedArray = new ArrayList();
		#endregion
	 
		#region Private Members
			private Engine m_engine = null;
			private Device m_device = null;
		#endregion

		#region Error Definitions
		// User Errors
		public const int ENGINE_INITIALIZATION				= (-1);
		public const string ENGINE_INITIALIZATION_TEXT		= "PrimoBurner engine initialization error.";

		public const int BURNER_NOT_OPEN					= (-2);
		public const string BURNER_NOT_OPEN_TEXT			= "Burner not open.";

		public const int NO_DEVICES							= (-3);
		public const string NO_DEVICES_TEXT					= "No CD/DVD/BD devices are available.";

		public const int NO_DEVICE							= (-4);
		public const string NO_DEVICE_TEXT					= "No device selected.";

        public const int DEVICE_ALREADY_SELECTED            = (-5);
        public const string DEVICE_ALREADY_SELECTED_TEXT    = "Device already selected.";

		public const int INVALID_DEVICE_INDEX				= (-6);
		public const string INVALID_DEVICE_INDEX_TEXT		= "Invalid device index.";

		public const int ERASE_NOT_SUPPORTED				= (-7);
		public const string ERASE_NOT_SUPPORTED_TEXT		= "Erasing is supported only for CD-RW and DVD-RW media.";

		public const int FORMAT_NOT_SUPPORTED				= (-8);
		public const string FORMAT_NOT_SUPPORTED_TEXT		= "Format is supported only for DVD-RW and DVD+RW media.";

		public const int NO_WRITER_DEVICES					= (-10);
		public const string NO_WRITER_DEVICES_TEXT			= "No CD/DVD/BD writers are available.";

		// Special Errors
		public const int DEVICE_ERROR		= (-100);
		public const int DATADISC_ERROR		= (-200);
		public const int VIDEODVD_ERROR		= (-300);
		#endregion
    }

    #region Settings
    // Burn Settings
    public class BurnSettings
    {
        public string SourceFolder;
        public string VolumeLabel;

        public PrimoSoftware.Burner.ImageType ImageType = ImageType.None;
        public bool VideoDVD = false;

        public PrimoSoftware.Burner.WriteMethod WriteMethod = WriteMethod.DvdIncremental;
        public int WriteSpeedKB = 0;

        public bool Simulate = false;
        public bool CloseDisc = true;
        public bool Eject = true;
    };

    public class CreateImageSettings
    {
        public string ImageFile = "";
        public string SourceFolder = "";

        public string VolumeLabel = "";
        public PrimoSoftware.Burner.ImageType ImageType = PrimoSoftware.Burner.ImageType.None;
        public bool VideoDVD = false;
    };

    // Format Settings
    public class FormatSettings
    {
        public bool Quick = true; 		// Quick format
        public bool Force = false;		// Format even if disc is already formatted
    };

    // Erase Settings
    public class EraseSettings
    {
        public bool Quick = true; 		// Quick erase
        public bool Force = false;		// Erase even if disc is already blank
    };
#endregion

    public class BurnerException : System.Exception
    {
        private string message;
        private int errorCode;

        public int ErrorCode
        {
            get
            {
                if (errorInfo != null)
                    return errorInfo.Code;

                return errorCode;
            }
        }

        public override string Message { get { return message; } }

        private PrimoSoftware.Burner.ErrorInfo errorInfo;

        public BurnerException(int errorCode, string errorMessage)
        {
            this.errorCode = errorCode;
            this.message = errorMessage;
        }

        public BurnerException(PrimoSoftware.Burner.ErrorInfo errorInfo)
        {
            if (errorInfo == null)
                return;

            this.errorInfo = (PrimoSoftware.Burner.ErrorInfo)errorInfo.Clone();

            switch (errorInfo.Facility)
            {
                case ErrorFacility.SystemWindows:
                    message = new System.ComponentModel.Win32Exception(errorInfo.Code).Message;
                    break;

                case ErrorFacility.Success:
                    message = "Success";
                    break;

                case ErrorFacility.DataDisc:
                    message = string.Format("DataDisc error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message);
                    break;

                case ErrorFacility.Device:
                    message = string.Format("Device error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message);
                    break;

                case ErrorFacility.VideoDVD:
                    message = string.Format("VideoDVD error: 0x{0:x8}: {1}", errorInfo.Code, errorInfo.Message);
                    break;

                default:
                    message = string.Format("Facility:{0} error :0x{1:x8}: {2}", errorInfo.Facility, errorInfo.Code, errorInfo.Message);
                    break;

            }
        }
    }

}
