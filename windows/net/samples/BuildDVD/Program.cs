using System;
using System.Runtime.InteropServices;
using System.IO;
using PrimoSoftware.DVDBuilder;

namespace BuildDVD
{
    class FileDataStreamFactory : InputDataStreamFactory
    {
        public System.IO.Stream Create(string file)
        {
            try
            {
                return new FileStream(file, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }
    };

    class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Library.Initialize();

            // Replace license string with your license
            Library.SetLicense("PRIMO-SOFTWARE-LICENSE-XML");
			

			if (args.Length != 2)
			{
                Console.WriteLine("Usage: BuildDVD project_file output_dir");

				Library.Shutdown();
				return;
			}

			using(DVDBuilder dvdBuilder = new DVDBuilder())
			{
				dvdBuilder.ProjectFile = args[0];
				dvdBuilder.OutputFolder = args[1];

				FileDataStreamFactory dataStreamFactory = new FileDataStreamFactory();
				dvdBuilder.InputDataStreamFactory = dataStreamFactory;

                dvdBuilder.OnContinue += new EventHandler<DVDBuilderContinueEventArgs>(dvdBuilder_OnContinue);
                dvdBuilder.OnProgress += new EventHandler<DVDBuilderProgressEventArgs>(dvdBuilder_OnProgress);
                dvdBuilder.OnStatus += new EventHandler<DVDBuilderStatusEventArgs>(dvdBuilder_OnStatus);

				dvdBuilder.Build();
                
                PrintResult(dvdBuilder);
			}

			Library.Shutdown();
		}

        static void dvdBuilder_OnContinue(object sender, DVDBuilderContinueEventArgs e)
        {
            e.Continue = true;
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

        private static string GetErrorMessage(DVDBuilderError dvdbError)
        {
            switch (dvdbError)
            {
                case DVDBuilderError.Interrupted:
                    return "Interrupted.";

                case DVDBuilderError.InternalError:
                    return "Unexpected error.";

                case DVDBuilderError.NoProject:
                    return "No project is specified.";

                case DVDBuilderError.InvalidProjectXml:
                    return "Malformed project XML.";

                case DVDBuilderError.ProjectVersionMissing:
                    return "Project version is not specified in the project.";

                case DVDBuilderError.UnsupportedProjectVersion:
                    return "The project version is higher then maximum recognized by DVDBuilder.";

                case DVDBuilderError.VideoManagerMissing:
                    return "There's no video manager in the project file. A <videoManager> element is required.";

                case DVDBuilderError.VideoManagerFirstPlayMissing:
                    return "The video manager needs a first play command. The command is set by the attribute 'firstPlayNavigate' of the <videoManager> element.";

                case DVDBuilderError.InvalidButton:
                    return "A <button> element is not defined correctly.";

                case DVDBuilderError.InvalidMenu:
                    return "A <menu> element is not defined correctly.";

                case DVDBuilderError.MenuIdMissing:
                    return "A <menu> element has no 'id' attribute.";

                case DVDBuilderError.DuplicateMenuId:
                    return "A menu has the same id as another menu and the conflict cannot be resolved.";

                case DVDBuilderError.NoTitlesets:
                    return "A DVD must have at least one video titleset.";

                case DVDBuilderError.NoTitles:
                    return "There are no titles in a in a titleset.";

                case DVDBuilderError.NoMenus:
                    return "The <menus> element is empty. When used it must contain at least one <menu> element.";

                case DVDBuilderError.TitleIdMissing:
                    return "A <title> element has no 'id' attribute.";

                case DVDBuilderError.DuplicateTitleId:
                    return "The attribute 'id' with the same value is used in more than one <title>.";

                case DVDBuilderError.NoChapters:
                    return "A title must have at least one chapter.";

                case DVDBuilderError.NoVob:
                    return "A title must have at least one video object.";

                case DVDBuilderError.VobFileMissing:
                    return "A <videoObject> element has no 'file' attribute or it's empty.";

                case DVDBuilderError.TooManyTitles:
                    return "A DVD disc can contain up to 99 titles.";

                case DVDBuilderError.FirstChapterNotZeroTime:
                    return "The first chapter in a title must start from 00:00:00.";

                case DVDBuilderError.ChaptersOutOfOrder:
                    return "The defined title chapters must be succesive in time.";

                case DVDBuilderError.InvalidChapterTime:
                    return "The chapter start time must be before the end of the video object and must follow the pattern hh:mm:ss.";

                case DVDBuilderError.InvalidProjectVobAspectRatio:
                    return "The video aspect ratio is not specified correctly in the project file.";

                case DVDBuilderError.InvalidProjectVobResolution:
                    return "The video resolution is not specified correctly in the project file.";

                case DVDBuilderError.InvalidProjectVobFrameRate:
                    return "The video framerate is not specified correctly in the project file.";

                case DVDBuilderError.MenuVobTooBig:
                    return "The menu VOB cannot be bigger than 1GB.";

                case DVDBuilderError.InvalidVideoFrameRate:
                    return "The frame rate of the input video must be either 29.97 fps for NTSC or 25 fps for PAL.";

                case DVDBuilderError.InvalidVideoFormat:
                    return "The format of the input video must be either MPEG-1 or MPEG-2.";

                case DVDBuilderError.InvalidVideoAspectRatio:
                    return "The aspect ratio of the input video must be either 4:3 or 16:9.";

                case DVDBuilderError.InvalidVideoResolution:
                    return "The resolution of the input video must be one of the following:\r\nNTSC: 720x480 | 704x480 | 352x480 | 352x240\r\nPAL:  720x576 | 704x576 | 352x576 | 352x288";

                case DVDBuilderError.InconsistentVideoStreams:
                    return "A specific group of videos is required to have the same video parameters. The requirement applies to: 1. all video objects in a titleset; 2. all menu backrgounds in a titleset or in the video manager.";

                case DVDBuilderError.InvalidAudioFormat:
                    return "The format of the input audio streams must be one of the following: LCPM, AC3, MPEG-1 Layer II or DTS.";

                case DVDBuilderError.InvalidAudioFrequency:
                    return "The frequency (sampling rate) of the input audio must be either 48000 Hz or 96000 Hz.";

                case DVDBuilderError.SubpictureEncodingError:
                    return "Subpicture encoding error. The encoded subpicture size exceeds 52KB.";

                case DVDBuilderError.InvalidBitmapDimensions:
                    return "The bitmap width and height must be positive and divisible by 2 (even numbers).";

                case DVDBuilderError.UnexpectedBitmapColor:
                    return "The bitmap contains a color that is different from the 4 colors described in the project: pattern, background, emphasis1 and emphasis2.";

                case DVDBuilderError.InvalidBitmap:
                    return "The input file is not recognized as a bitmap.";

                case DVDBuilderError.UnsupportedBitmapCompression:
                    return "The input file is a compressed bitmap. An uncompressed bitmap is required.";

                case DVDBuilderError.UnsupportedBitmapColorDepth:
                    return "The input bitmap does not have the supported color depth. The supported color depth is: 8-bit, 24-bit or 32-bit.";

                case DVDBuilderError.MultiplexerError:
                    return "A general MPEG multiplexer error.";

                case DVDBuilderError.MultiplexerParams:
                    return "Invalid parameters (e.g. no audio/video streams, or streams with invalid format) are passed to the multiplexer.";

                case DVDBuilderError.MultiplexerUnderrun:
                    return "The multiplexer has run out of input data.";

                case DVDBuilderError.DataStreamError:
                    return "A call to IDataStream method failed.";

                case DVDBuilderError.InvalidNavigationCommand:
                    return "Invalid navigation command. Wrong title ID, menu ID or chapter number found in a navigational attribute.";

                case DVDBuilderError.ElementaryStreamFileMissing:
                    return "An elementary stream (<videoStream\\>, <audioStream\\> or <subpictureStream\\>) element has no 'file' attribute or it's empty.";

                case DVDBuilderError.InvalidAudioEsFormat:
                    return "The audio format is invalid. The audio format must be MPA, AC3 or DTS.";

                case DVDBuilderError.InvalidElementaryStream:
                    return "The specified elemetary stream file is invalid.";

                default:
                    return "Unknown error.";
            }
        }

        private static void PrintResult(DVDBuilder dvdb)
		{
            ErrorFacility facility = (ErrorFacility)(dvdb.Error.Facility);
            string msg = null;

            try
            {
                switch (facility)
                {
                    case ErrorFacility.Success:
                        // Success.
                        return;

                    case ErrorFacility.SystemWindows:
                        System.ComponentModel.Win32Exception sysex = new System.ComponentModel.Win32Exception(dvdb.Error.Code);
                        msg = "System error: " + sysex.Message;
                        return;

                    case ErrorFacility.DVDBuilder:
                        msg = GetErrorMessage((DVDBuilderError)dvdb.Error.Code);
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
	}
}
