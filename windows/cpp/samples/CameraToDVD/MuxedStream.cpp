#include "stdafx.h"
#include "MuxedStream.h"

using namespace primo::dvdbuilder::VR;

MuxedStreamCB::MuxedStreamCB()
{
	filename[0] = L'\0';
	file = NULL;
	MainWindow = NULL;
	
	Reset();
}

MuxedStreamCB::~MuxedStreamCB()
{
	Reset();
}

void MuxedStreamCB::Reset()
{
	if (file)
	{
		fclose(file);
		file = NULL;
	}
	writeCounter = 0;
	pVideoRecorder = NULL;
	bProcess = true;
}

bool MuxedStreamCB::WriteData(uint8_t *pBuf, uint32_t bufSize)
{
	if (!bProcess)
         return false;

	++writeCounter;

	bool fileResult = true;
	if (file)
	{
		size_t size = fwrite(pBuf,1,bufSize,file);
		fileResult = (size == bufSize);
	}

	bool dvdResult = true;
	int stopReason = -1;
	if (pVideoRecorder && !pVideoRecorder->write(pBuf, bufSize))
	{
		ATLTRACE(L"VideoRecorder::Write() failed");
		
		DumpErrorState(pVideoRecorder);

		int activeCount, failedCount, noSpaceCount;
		CheckRecorderDevices(pVideoRecorder, &activeCount, &failedCount, &noSpaceCount);
		
		ATLASSERT((activeCount + failedCount + noSpaceCount) == pVideoRecorder->devices()->count());
		
		if (0 == activeCount)
		{
			dvdResult = false;
			if (0 == failedCount && noSpaceCount > 0)
				stopReason = -2;
		}
	}

	if (fileResult && dvdResult)
                return true;

        //STOP_CAPTURE
        PostMessage(MainWindow, WM_STOP_CAPTURE, stopReason, 0);
        bProcess = false;
        return false;
}

bool MuxedStreamCB::SetOutputFile(PCWSTR filename)
{
	if (file)
	{
		fclose(file);
		file = NULL;
	}

	if (!filename)
	{
		this->filename[0] = L'\0';
		return true;
	}

	wcscpy_s(this->filename,256, filename);

	file = _wfopen(this->filename, L"wb");

	return file != NULL;
}


// primo::Stream
bool_t MuxedStreamCB::open()
{
	return TRUE;
}

void MuxedStreamCB::close()
{

}

bool_t MuxedStreamCB::isOpen() const
{
	return TRUE;
}

bool_t MuxedStreamCB::canRead() const
{
	return FALSE;
}

bool_t MuxedStreamCB::canWrite() const
{
	return TRUE;
}

bool_t MuxedStreamCB::canSeek() const
{
	return FALSE;
}

bool_t MuxedStreamCB::read(void* buffer, int32_t bufferSize, int32_t* totalRead)
{
	return FALSE;
}

bool_t MuxedStreamCB::write(const void* buffer, int32_t dataSize)
{
	return WriteData((uint8_t*)buffer, dataSize);
}

int64_t MuxedStreamCB::size() const
{
	return -1;
}

int64_t MuxedStreamCB::position() const
{
	return -1;
}

bool_t MuxedStreamCB::seek(int64_t position)
{
	return FALSE;
}
