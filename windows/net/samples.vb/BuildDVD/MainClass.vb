Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices
Imports System.IO
Imports PrimoSoftware.DVDBuilder

Namespace BuildDVD
    Friend Class FileDataStreamFactory
        Implements InputDataStreamFactory
        Public Function Create(ByVal file As String) As System.IO.Stream Implements InputDataStreamFactory.Create
            Try
                Return New FileStream(file, FileMode.Open, FileAccess.Read)
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine(ex.Message)
                Return Nothing
            End Try
        End Function
    End Class

    Friend Class MainClass
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()> _
        Shared Sub Main(ByVal args() As String)
            Library.Initialize()

            ' Replace license string
            Library.SetLicense("PRIMO-SOFTWARE-LICENSE")


            If args.Length <> 2 Then
                Console.WriteLine("Usage: DVDAuthorCmd project_file output_dir")

                Library.Shutdown()
                Return
            End If

            Using dvdBuilder As New DVDBuilder()
                dvdBuilder.ProjectFile = args(0)
                dvdBuilder.OutputFolder = args(1)

                Dim dataStreamFactory As New FileDataStreamFactory()
                dvdBuilder.InputDataStreamFactory = dataStreamFactory

                AddHandler dvdBuilder.OnContinue, AddressOf dvdBuilder_OnContinue
                AddHandler dvdBuilder.OnProgress, AddressOf dvdBuilder_OnProgress
                AddHandler dvdBuilder.OnStatus, AddressOf dvdBuilder_OnStatus

                dvdBuilder.Build()

                PrintResult(dvdBuilder)
            End Using

            Library.Shutdown()
        End Sub

        Private Shared Sub dvdBuilder_OnContinue(ByVal sender As Object, ByVal e As DVDBuilderContinueEventArgs)
            e.Continue = True
        End Sub

        Private Shared Sub dvdBuilder_OnProgress(ByVal sender As Object, ByVal e As DVDBuilderProgressEventArgs)
            Console.Write(String.Format(Constants.vbCr & "Progress: {0}", e.Percent))
        End Sub

        Private Shared Sub dvdBuilder_OnStatus(ByVal sender As Object, ByVal e As DVDBuilderStatusEventArgs)
            Console.WriteLine("")
            Select Case e.Status
                Case DVDBuilderStatus.WritingVOB
                    Console.WriteLine("Status: WritingVOB")
                Case DVDBuilderStatus.WritingIFO
                    Console.WriteLine("Status: WritingIFO")
            End Select
        End Sub

        Private Shared Function GetErrorMessage(ByVal dvdbError As DVDBuilderError) As String
            Select Case dvdbError
                Case DVDBuilderError.Interrupted
                    Return "Interrupted."

                Case DVDBuilderError.InternalError
                    Return "Unexpected error."

                Case DVDBuilderError.NoProject
                    Return "No project is specified."

                Case DVDBuilderError.InvalidProjectXml
                    Return "Malformed project XML."

                Case DVDBuilderError.ProjectVersionMissing
                    Return "Project version is not specified in the project."

                Case DVDBuilderError.UnsupportedProjectVersion
                    Return "The project version is higher then maximum recognized by DVDBuilder."

                Case DVDBuilderError.VideoManagerMissing
                    Return "There's no video manager in the project file. A <videoManager> element is required."

                Case DVDBuilderError.VideoManagerFirstPlayMissing
                    Return "The video manager needs a first play command. The command is set by the attribute 'firstPlayNavigate' of the <videoManager> element."

                Case DVDBuilderError.InvalidButton
                    Return "A <button> element is not defined correctly."

                Case DVDBuilderError.InvalidMenu
                    Return "A <menu> element is not defined correctly."

                Case DVDBuilderError.MenuIdMissing
                    Return "A <menu> element has no 'id' attribute."

                Case DVDBuilderError.DuplicateMenuId
                    Return "A menu has the same id as another menu and the conflict cannot be resolved."

                Case DVDBuilderError.NoTitlesets
                    Return "A DVD must have at least one video titleset."

                Case DVDBuilderError.NoTitles
                    Return "There are no titles in a in a titleset."

                Case DVDBuilderError.NoMenus
                    Return "The <menus> element is empty. When used it must contain at least one <menu> element."

                Case DVDBuilderError.TitleIdMissing
                    Return "A <title> element has no 'id' attribute."

                Case DVDBuilderError.DuplicateTitleId
                    Return "The attribute 'id' with the same value is used in more than one <title>."

                Case DVDBuilderError.NoChapters
                    Return "A title must have at least one chapter."

                Case DVDBuilderError.NoVob
                    Return "A title must have at least one video object."

                Case DVDBuilderError.VobFileMissing
                    Return "A <videoObject> element has no 'file' attribute or it's empty."

                Case DVDBuilderError.TooManyTitles
                    Return "A DVD disc can contain up to 99 titles."

                Case DVDBuilderError.FirstChapterNotZeroTime
                    Return "The first chapter in a title must start from 00:00:00."

                Case DVDBuilderError.ChaptersOutOfOrder
                    Return "The defined title chapters must be succesive in time."

                Case DVDBuilderError.InvalidChapterTime
                    Return "The chapter start time must be before the end of the video object and must follow the pattern hh:mm:ss."

                Case DVDBuilderError.InvalidProjectVobAspectRatio
                    Return "The video aspect ratio is not specified correctly in the project file."

                Case DVDBuilderError.InvalidProjectVobResolution
                    Return "The video resolution is not specified correctly in the project file."

                Case DVDBuilderError.InvalidProjectVobFrameRate
                    Return "The video framerate is not specified correctly in the project file."

                Case DVDBuilderError.MenuVobTooBig
                    Return "The menu VOB cannot be bigger than 1GB."

                Case DVDBuilderError.InvalidVideoFrameRate
                    Return "The frame rate of the input video must be either 29.97 fps for NTSC or 25 fps for PAL."

                Case DVDBuilderError.InvalidVideoFormat
                    Return "The format of the input video must be either MPEG-1 or MPEG-2."

                Case DVDBuilderError.InvalidVideoAspectRatio
                    Return "The aspect ratio of the input video must be either 4:3 or 16:9."

                Case DVDBuilderError.InvalidVideoResolution
                    Return "The resolution of the input video must be one of the following:" & Constants.vbCrLf & "NTSC: 720x480 | 704x480 | 352x480 | 352x240" & Constants.vbCrLf & "PAL:  720x576 | 704x576 | 352x576 | 352x288"

                Case DVDBuilderError.InconsistentVideoStreams
                    Return "A specific group of videos is required to have the same video parameters. The requirement applies to: 1. all video objects in a titleset; 2. all menu backrgounds in a titleset or in the video manager."

                Case DVDBuilderError.InvalidAudioFormat
                    Return "The format of the input audio streams must be one of the following: LCPM, AC3, MPEG-1 Layer II or DTS."

                Case DVDBuilderError.InvalidAudioFrequency
                    Return "The frequency (sampling rate) of the input audio must be either 48000 Hz or 96000 Hz."

                Case DVDBuilderError.SubpictureEncodingError
                    Return "Subpicture encoding error. The encoded subpicture size exceeds 52KB."

                Case DVDBuilderError.InvalidBitmapDimensions
                    Return "The bitmap width and height must be positive and divisible by 2 (even numbers)."

                Case DVDBuilderError.UnexpectedBitmapColor
                    Return "The bitmap contains a color that is different from the 4 colors described in the project: pattern, background, emphasis1 and emphasis2."

                Case DVDBuilderError.InvalidBitmap
                    Return "The input file is not recognized as a bitmap."

                Case DVDBuilderError.UnsupportedBitmapCompression
                    Return "The input file is a compressed bitmap. An uncompressed bitmap is required."

                Case DVDBuilderError.UnsupportedBitmapColorDepth
                    Return "The input bitmap does not have the supported color depth. The supported color depth is: 8-bit, 24-bit or 32-bit."

                Case DVDBuilderError.MultiplexerError
                    Return "A general MPEG multiplexer error."

                Case DVDBuilderError.MultiplexerParams
                    Return "Invalid parameters (e.g. no audio/video streams, or streams with invalid format) are passed to the multiplexer."

                Case DVDBuilderError.MultiplexerUnderrun
                    Return "The multiplexer has run out of input data."

                Case DVDBuilderError.DataStreamError
                    Return "A call to IDataStream method failed."

                Case DVDBuilderError.InvalidNavigationCommand
                    Return "Invalid navigation command. Wrong title ID, menu ID or chapter number found in a navigational attribute."

                Case DVDBuilderError.ElementaryStreamFileMissing
                    Return "An elementary stream (<videoStream\>, <audioStream\> or <subpictureStream\>) element has no 'file' attribute or it's empty."

                Case DVDBuilderError.InvalidAudioEsFormat
                    Return "The audio format is invalid. The audio format must be MPA, AC3 or DTS."

                Case DVDBuilderError.InvalidElementaryStream
                    Return "The specified elemetary stream file is invalid."

                Case Else
                    Return "Unknown error."
            End Select
        End Function

        Private Shared Sub PrintResult(ByVal dvdb As DVDBuilder)
            Dim facility As ErrorFacility = CType(dvdb.Error.Facility, ErrorFacility)
            Dim msg As String = Nothing

            Try
                Select Case facility
                    Case ErrorFacility.Success
                        ' Success.
                        Return

                    Case ErrorFacility.SystemWindows
                        Dim sysex As New System.ComponentModel.Win32Exception(dvdb.Error.Code)
                        msg = "System error: " & sysex.Message
                        Return

                    Case ErrorFacility.DVDBuilder
                        msg = GetErrorMessage(CType(dvdb.Error.Code, DVDBuilderError))
                        Return

                    Case Else
                        msg = "Unknown error."
                        Return
                End Select
            Finally
                Console.WriteLine("")
                If msg IsNot Nothing Then
                    Console.WriteLine(msg)
                    Console.WriteLine("hint: {0}", dvdb.Error.Hint)
                Else
                    Console.WriteLine("Success")
                End If

            End Try
        End Sub
    End Class
End Namespace
