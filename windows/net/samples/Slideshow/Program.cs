using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using PrimoSoftware.DVDBuilder;
using PrimoSoftware.AVBlocks;


namespace Slideshow
{
    class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string imagesFolder = Path.Combine(appPath, "..\\sample-resources\\img");
            string projectFolder = Path.Combine(appPath, "..\\sample-projects\\Slideshow");
            string outputFolder = Path.Combine(appPath, "SlideshowDVD");

            string encodingPreset = Preset.Video.DVD.PAL_4x3_MP2;

            PrimoSoftware.AVBlocks.Library.Initialize();

            // Set license information. To run AVBlocks in demo mode, comment the next line out
            // PrimoSoftware.AVBlocks.Library.SetLicense("<license-xml-string>");

            PrimoSoftware.DVDBuilder.Library.Initialize();
            
            // Set license information. To run DVDBuilder in demo mode, comment the next line out
            // PrimoSoftware.DVDBuilder.Library.SetLicense("<license-xml-string>");
            
            // Encode the input images as mpeg2 videos. Also encode the menu background bitmap as video.
            if (!EncodeImagesToVideos(encodingPreset, imagesFolder, projectFolder))
                return;

            // Create the DVD files (VIDEO_TS and AUDIO_TS folders)
            string projectFile = Path.Combine(projectFolder, "project.xml");
            BuildDVD(projectFile, outputFolder);

            PrimoSoftware.AVBlocks.Library.Shutdown();
            PrimoSoftware.DVDBuilder.Library.Shutdown();
		}

        #region Encode Images to MPEG2 Video
        static bool EncodeImagesToVideos(string encodingPreset, string imagesFolder, string projectFolder)
        {
            // Create a video file from images. Each image will be encoded as one video frame.
            if (!EncodeImagesToMpeg2Video(imagesFolder, Path.Combine(projectFolder, "movie1.mpg"), 250, 1, 1, encodingPreset))
                return false;

            // Create a video file from images. Each image will be encoded as 25 video frames (25 video frames duration is 1 second).
            if (!EncodeImagesToMpeg2Video(imagesFolder, Path.Combine(projectFolder, "movie2.mpg"), 15, 25, 15, encodingPreset))
                return false;

            // Encode the menu background image as Mpeg2 video with one frame.
            if(!EncodeImageToMpeg2Video(Path.Combine(projectFolder, "menu.bmp"), Path.Combine(projectFolder, "menu.mpg"), encodingPreset))
                return false;

            return true;
        }

        static bool EncodeImagesToMpeg2Video(string imagesFolder, string outputFile, int imagesCount, int videoFramesPerImage, int step, string encodingPreset)
        {
            double frameRate = -1;

            switch (encodingPreset)
            {
                case Preset.Video.DVD.PAL_4x3_MP2:
                case Preset.Video.DVD.PAL_16x9_MP2:
                    frameRate = 25.0; 
                    break;

                case Preset.Video.DVD.NTSC_4x3_MP2:
                case Preset.Video.DVD.NTSC_16x9_MP2:
                    frameRate = 30000.0 / 1001; // 29.97
                    break;

                default:
                    return false;
            }

            Transcoder transcoder = new Transcoder();

            // In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
            transcoder.AllowDemoMode = true;

            try
            {
                File.Delete(outputFile);

                // Configure Input
                {
                    MediaInfo info = new MediaInfo();
                    info.InputFile = GetImagePath(imagesFolder, 0);
                    if (!info.Load())
                    {
                        PrintError("MediaInfo load", info.Error);
                        return false;
                    }

                    MediaPin pin = new MediaPin();
                    VideoStreamInfo vInfo = (VideoStreamInfo)info.Streams[0];
                    vInfo.FrameRate = frameRate;
                    pin.StreamInfo = vInfo;

                    MediaSocket socket = new MediaSocket();
                    socket.Pins.Add(pin);

                    transcoder.Inputs.Add(socket);
                }

                // Configure Output
                {
                    MediaSocket socket = MediaSocket.FromPreset(encodingPreset);
                    socket.File = outputFile;
                    transcoder.Outputs.Add(socket);
                }

                // Encode Images
                if (!transcoder.Open())
                {
                    PrintError("Transcoder open", transcoder.Error);
                    return false;
                }

                int totalVideoFrames = imagesCount * videoFramesPerImage;

                for (int i = 0; i < totalVideoFrames; i++)
                {
                    string imagePath = GetImagePath(imagesFolder, i / videoFramesPerImage * step);
                    MediaBuffer mediaBuffer = new MediaBuffer(File.ReadAllBytes(imagePath));
                    MediaSample mediaSample = new MediaSample();
                    mediaSample.Buffer = mediaBuffer;

                    mediaSample.StartTime = i / frameRate;

                    if (!transcoder.Push(0, mediaSample))
                    {
                        PrintError("Transcoder write", transcoder.Error);
                        return false;
                    }
                }

                if (!transcoder.Flush())
                {
                    PrintError("Transcoder flush", transcoder.Error);
                    return false;
                }

                transcoder.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                transcoder.Dispose();
            }

            return true;
        }

        static bool EncodeImageToMpeg2Video(string imageFile, string outputFile, string encodingPreset)
        {
            Transcoder transcoder = new Transcoder();

            // In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
            transcoder.AllowDemoMode = true;

            try
            {
                File.Delete(outputFile);

                // Configure Input
                {
                    MediaInfo info = new MediaInfo();
                    info.InputFile = imageFile;
                    if (!info.Load())
                    {
                        PrintError("MediaInfo load", info.Error);
                        return false;
                    }

                    MediaSocket socket = MediaSocket.FromMediaInfo(info);
                    transcoder.Inputs.Add(socket);
                }

                // Configure Output
                {
                    MediaSocket socket = MediaSocket.FromPreset(encodingPreset);
                    socket.File = outputFile;
                    transcoder.Outputs.Add(socket);
                }

                // Encode Images
                if (!transcoder.Open())
                {
                    PrintError("Transcoder open", transcoder.Error);
                    return false;
                }

                if (!transcoder.Run())
                {
                    PrintError("Transcoder run", transcoder.Error);
                    return false;
                }

                transcoder.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                transcoder.Dispose();
            }

            return true;
        }

        static void PrintError(string action, PrimoSoftware.AVBlocks.ErrorInfo e)
        {
            if (PrimoSoftware.AVBlocks.ErrorFacility.Success == e.Facility)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("{0}:  {1}, facility:{2} code:{3}", action, e.Message, e.Facility, e.Code);
            }
            Console.WriteLine();
        }


        static string GetImagePath(string imagesFolder, int imageNumber)
        {
            return Path.Combine(imagesFolder, string.Format("cube{0:0000}.jpeg", imageNumber));
        }

        #endregion

        #region Build DVD
        private static void BuildDVD(string projectFilePath, string outputFolder)
        {
            using (DVDBuilder dvdBuilder = new DVDBuilder())
            {
                dvdBuilder.ProjectFile = projectFilePath;
                dvdBuilder.OutputFolder = outputFolder;

                dvdBuilder.OnProgress += new EventHandler<DVDBuilderProgressEventArgs>(dvdBuilder_OnProgress);
                dvdBuilder.OnStatus += new EventHandler<DVDBuilderStatusEventArgs>(dvdBuilder_OnStatus);

                dvdBuilder.Build();

                PrintResult(dvdBuilder);
            }
        }

		private static void dvdBuilder_OnProgress(object sender, DVDBuilderProgressEventArgs e)
		{
			Console.Write(string.Format("\rProgress: {0}", e.Percent));
		}

		private static void dvdBuilder_OnStatus(object sender, DVDBuilderStatusEventArgs e)
		{
			Console.WriteLine("");
			switch(e.Status)
			{
				case DVDBuilderStatus.WritingVOB:
					Console.WriteLine("Status: WritingVOB");
					break;
				case DVDBuilderStatus.WritingIFO:
					Console.WriteLine("Status: WritingIFO");
					break;
			}
		}

        static void PrintResult(DVDBuilder dvdb)
		{
            PrimoSoftware.DVDBuilder.ErrorFacility facility = (PrimoSoftware.DVDBuilder.ErrorFacility)(dvdb.Error.Facility);
            string msg = null;

            try
            {
                switch (facility)
                {
                    case PrimoSoftware.DVDBuilder.ErrorFacility.Success:
                        // Success.
                        return;

                    case PrimoSoftware.DVDBuilder.ErrorFacility.SystemWindows:
                        System.ComponentModel.Win32Exception sysex = new System.ComponentModel.Win32Exception(dvdb.Error.Code);
                        msg = "System error: " + sysex.Message;
                        return;

                    case PrimoSoftware.DVDBuilder.ErrorFacility.DVDBuilder:
                        msg = ((DVDBuilderError)dvdb.Error.Code).ToString();
                        return;

                    default:
                        msg = "Unknown error.";
                        return;
                }
            }
            finally
            {
                Console.WriteLine("");
                if (msg != null)
                {
                    Console.WriteLine(msg);
                    Console.WriteLine("hint: {0}", dvdb.Error.Hint);
                }
                else
                {
                    Console.WriteLine("Success");
                }
                
            }
        }
        #endregion
    }
}
