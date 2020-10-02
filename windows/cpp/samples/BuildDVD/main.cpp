// DVDAuthorApp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "DVDBuilder.h"
using namespace primo::dvdbuilder;

#include "FileDataStream.h"

class MyDVDCallback: public DVDBuilderCallback
{
public:
	MyDVDCallback()
	{
	}

	virtual void onProgress(int32_t percentDone)
	{
		_tprintf(_T("\rProgress: %d"), percentDone);
	}

	virtual void onStatus(DVDBuilderStatus::Enum eStatus)
	{
		switch(eStatus)
		{
		case DVDBuilderStatus::WritingVOB:
			_tprintf(_T("\r\nStatus: WritingVOB\r\n"));
			break;
		case DVDBuilderStatus::WritingIFO:
			_tprintf(_T("\r\nStatus: WritingIFO\r\n"));
			break;
		}
	}

	virtual bool_t onContinue()
	{
		return TRUE;
	}
};

const char_t* GetErrorMessage(int dvdbError)
{
	//DVDBuilderError::Enum dvdbError = (DVDBuilderError::Enum)error;
	switch(dvdbError)
	{
	case DVDBuilderError::Interrupted:
		return _T("Interrupted.");

	case DVDBuilderError::InternalError:
		return _T("Unexpected error.");

	case DVDBuilderError::NoProject:
		return _T("No project is specified.");

	case DVDBuilderError::InvalidProjectXml:
		return _T("Malformed project XML.");

	case DVDBuilderError::ProjectVersionMissing:
		return _T("Project version is not specified in the project.");

	case DVDBuilderError::UnsupportedProjectVersion:
		return _T("The project version is higher then maximum recognized by DVDBuilder.");

	case DVDBuilderError::VideoManagerMissing:
		return _T("There's no video manager in the project file. A <videoManager> element is required.");

	case DVDBuilderError::VideoManagerFirstPlayMissing:
		return _T("The video manager needs a first play command. The command is set by the attribute 'firstPlayNavigate' of the <videoManager> element.");

	case DVDBuilderError::InvalidButton:
		return _T("A <button> element is not defined correctly.");

	case DVDBuilderError::InvalidMenu:
		return _T("A <menu> element is not defined correctly.");

	case DVDBuilderError::MenuIdMissing:
		return _T("A <menu> element has no 'id' attribute.");

	case DVDBuilderError::DuplicateMenuId:
		return _T("A menu has the same id as another menu and the conflict cannot be resolved.");

	case DVDBuilderError::NoTitlesets:
		return _T("A DVD must have at least one video titleset.");

	case DVDBuilderError::NoTitles:
		return _T("There are no titles in a in a titleset.");

	case DVDBuilderError::NoMenus:
		return _T("The <menus> element is empty. When used it must contain at least one <menu> element.");

	case DVDBuilderError::TitleIdMissing:
		return _T("A <title> element has no 'id' attribute.");

	case DVDBuilderError::DuplicateTitleId:
		return _T("The attribute 'id' with the same value is used in more than one <title>.");

	case DVDBuilderError::NoChapters:
		return _T("A title must have at least one chapter.");

	case DVDBuilderError::NoVob:
		return _T("A title must have at least one video object.");

	case DVDBuilderError::VobFileMissing:
		return _T("A <videoObject> element has no 'file' attribute or it's empty.");

	case DVDBuilderError::TooManyTitles:
		return _T("A DVD disc can contain up to 99 titles.");

	case DVDBuilderError::FirstChapterNotZeroTime:
		return _T("The first chapter in a title must start from 00:00:00.");

	case DVDBuilderError::ChaptersOutOfOrder:
		return _T("The defined title chapters must be succesive in time.");

	case DVDBuilderError::InvalidChapterTime:
		return _T("The chapter start time must be before the end of the video object and must follow the pattern hh:mm:ss.");

	case DVDBuilderError::InvalidProjectVobAspectRatio:
		return _T("The video aspect ratio is not specified correctly in the project file.");

	case DVDBuilderError::InvalidProjectVobResolution:
		return _T("The video resolution is not specified correctly in the project file.");

	case DVDBuilderError::InvalidProjectVobFrameRate:
		return _T("The video framerate is not specified correctly in the project file.");

	case DVDBuilderError::MenuVobTooBig:
		return _T("The menu VOB cannot be bigger than 1GB.");

	case DVDBuilderError::InvalidVideoFrameRate:
		return _T("The frame rate of the input video must be either 29.97 fps for NTSC or 25 fps for PAL.");

	case DVDBuilderError::InvalidVideoFormat:
		return _T("The format of the input video must be either MPEG-1 or MPEG-2.");

	case DVDBuilderError::InvalidVideoAspectRatio:
		return _T("The aspect ratio of the input video must be either 4:3 or 16:9.");

	case DVDBuilderError::InvalidVideoResolution:
		return _T("The resolution of the input video must be one of the following:\r\n")
			_T("NTSC: 720x480 | 704x480 | 352x480 | 352x240\r\n")
			_T("PAL:  720x576 | 704x576 | 352x576 | 352x288");

	case DVDBuilderError::InconsistentVideoStreams:
		return _T("A specific group of videos is required to have the same video parameters. The requirement applies to: 1. all video objects in a titleset; 2. all menu backrgounds in a titleset or in the video manager.");

	case DVDBuilderError::InvalidAudioFormat:
		return _T("The format of the input audio streams must be one of the following: LCPM, AC3, MPEG-1 Layer II or DTS.");

	case DVDBuilderError::InvalidAudioFrequency:
		return _T("The frequency (sampling rate) of the input audio must be either 48000 Hz or 96000 Hz.");

	case DVDBuilderError::SubpictureEncodingError:
		return _T("Subpicture encoding error. The encoded subpicture size exceeds 52KB.");

	case DVDBuilderError::InvalidBitmapDimensions:
		return _T("The bitmap width and height must be positive and divisible by 2 (even numbers).");

	case DVDBuilderError::UnexpectedBitmapColor:
		return _T("The bitmap contains a color that is different from the 4 colors described in the project: pattern, background, emphasis1 and emphasis2.");

	case DVDBuilderError::InvalidBitmap:
		return _T("The input file is not recognized as a bitmap.");

	case DVDBuilderError::UnsupportedBitmapCompression:
		return _T("The input file is a compressed bitmap. An uncompressed bitmap is required.");

	case DVDBuilderError::UnsupportedBitmapColorDepth:
		return _T("The input bitmap does not have the supported color depth. The supported color depth is: 8-bit, 24-bit or 32-bit.");

	case DVDBuilderError::MultiplexerError:
		return _T("A general MPEG multiplexer error.");

	case DVDBuilderError::MultiplexerParams:
		return _T("Invalid parameters (e.g. no audio/video streams, or streams with invalid format) are passed to the multiplexer.");

	case DVDBuilderError::MultiplexerUnderrun:
		return _T("The multiplexer has run out of input data.");

	case DVDBuilderError::DataStreamError:
		return _T("A call to IDataStream method failed.");

	case DVDBuilderError::InvalidNavigationCommand:
		return _T("Invalid navigation command. Wrong title ID, menu ID or chapter number found in a navigational attribute.");

	case DVDBuilderError::ElementaryStreamFileMissing:
		return _T("An elementary stream (<videoStream\\>, <audioStream\\> or <subpictureStream\\>) element has no 'file' attribute or it's empty.");

	case DVDBuilderError::InvalidAudioEsFormat:
		return _T("The audio format is invalid. The audio format must be MPA, AC3 or DTS.");

	case DVDBuilderError::InvalidElementaryStream:
		return _T("The specified elemetary stream file is invalid.");

	default:
		return _T("Unknown error.");
	}
}

void PrintResult(DVDBuilder* dvdb)
{
	using namespace primo::dvdbuilder;
	using namespace std;

	const char_t* msg = NULL;
	const char_t* sysMsg = NULL;

	int code = dvdb->error()->code();
	int facility = dvdb->error()->facility();

	switch(facility)
	{
		case ErrorFacility::Success:
			break;

		case ErrorFacility::SystemWindows:

			msg = _T("System error.");
			FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | 
							FORMAT_MESSAGE_FROM_SYSTEM | 
							FORMAT_MESSAGE_IGNORE_INSERTS,
							NULL,
							code,
							MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
							(LPTSTR) &sysMsg,
							0,
							NULL); 
			break;

		case ErrorFacility::DVDBuilder:
			msg = GetErrorMessage(code);
			break;

		default:
			msg = _T("Unknown error.");
			break;
	}

	if (msg) 
	{
		wcout << endl << msg << endl;

		const char_t* hint = dvdb->error()->hint();
		wcout << _T("hint: ") << hint << endl;

		if (sysMsg)
		{
			wcout << _T("system message: ") << sysMsg << endl;
			LocalFree((LPVOID*)sysMsg);
		}
	} 
	else 
	{
		wcout << _T("\nSuccess.") << endl;
	}

}

int _tmain(int argc, _TCHAR* argv[])
{
	if (argc != 3)
	{
		_tprintf(_T("\r\nUsage: BuildDVD project_file output_dir\r\n"));
		return 1;
	}

	// Replace with your DVDBuilder license
	primo::dvdbuilder::Library::setLicense("YOUR DVDBULDER LICENSE XML");

	DVDBuilder* dvdBuilder = Library::createDVDBuilder();

	FileDataStreamFactory dataStreamFactory;
	dvdBuilder->setInputDataStreamFactory(&dataStreamFactory);

	MyDVDCallback callback;
	dvdBuilder->setCallback(&callback);

	dvdBuilder->setProjectFile(argv[1]);
	dvdBuilder->setOutputFolder(argv[2]);

	bool_t result = dvdBuilder->build();

	PrintResult(dvdBuilder);

	dvdBuilder->release();

	return 0;
}
