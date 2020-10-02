#include "stdafx.h"
#include "SampleCallbacks.h"
#include "MediaState.h"

/// SampleGrabber Callback

bool SampleGrabberCB::ProcessSample(uint8_t *pBuf, uint32_t bufSize, double sampleTime)
{
	bool fileResult = true;
	if (file)
	{
		size_t size = fwrite(pBuf,1,bufSize,file);
		fileResult = (size == bufSize);
	}

	//DBG - simulate encoding error
	//if (SampleIndex > 100)
	//    goto STOP_CAPTURE;

	bool pushResult = true;
	if (pMediaState && pMediaState->pMpeg2Enc)
	{
		
		primo::avblocks::Transcoder* pEnc = pMediaState->pMpeg2Enc;

		if (sampleTime < 0)
			sampleTime = 0;


		if(!m_mediaSample)
		{
			m_mediaSample = primo::avblocks::Library::createMediaSample();
		}

		if(m_mediaBuffer)
		{
			if(m_mediaBuffer->capacity() < bufSize)
			{
				m_mediaBuffer->release();
				m_mediaBuffer = NULL;
			}
		}

		if(!m_mediaBuffer)
		{
			m_mediaBuffer = primo::avblocks::Library::createMediaBuffer(bufSize);
			m_mediaSample->setBuffer(m_mediaBuffer);
		}

		memcpy(m_mediaBuffer->start(), pBuf, bufSize);
		m_mediaBuffer->setData(0, bufSize);
		m_mediaSample->setStartTime(sampleTime);

		// transcoder->push() is not threads safe.
		// EnterCriticalSection/LeaveCriticalSection ensure that only one thread is calling transcoder->push()

		EnterCriticalSection(&(pMediaState->transcoderSync));

		pushResult = pEnc->push(StreamNumber, m_mediaSample);

		LeaveCriticalSection(&(pMediaState->transcoderSync));

		if (!pushResult)
		{
			ATLTRACE(L"Write sample FAILED");
		}
	}

	if (fileResult && pushResult)
		return true;

	//STOP_CAPTURE:
	bProcess = false;
	PostMessage(MainWindow, WM_STOP_CAPTURE, StreamNumber, 0);

	return false;	

}

HRESULT SampleGrabberCB::BufferCB(double SampleTime, BYTE *pBuffer, long BufferLen)
{
	if (!bProcess)
	{
		return E_FAIL;
	}
	++SampleIndex;

	bool processed = ProcessSample(pBuffer, BufferLen, SampleTime);

	return processed ? S_OK : E_FAIL;
}

HRESULT SampleGrabberCB::SampleCB(double SampleTime, IMediaSample *pSample)
{
	if (!bProcess)
	{
		return E_FAIL;
	}

	// internal stats
	++SampleIndex;

	LONGLONG tStart, tEnd;
	pSample->GetMediaTime(&tStart, &tEnd);

	ATLASSERT(tStart < tEnd);
	ATLASSERT(tStart > lastMediaTime);

	SampleProcessed += tEnd - tStart;
	SampleDropped += tStart - lastMediaTime - 1;
	lastMediaTime = tEnd - 1;

	int dataLen = pSample->GetActualDataLength();
	BYTE*  pBuf;
	HRESULT hr = pSample->GetPointer(&pBuf);
	ATLASSERT(S_OK == hr);

	bool processed = ProcessSample(pBuf, dataLen, SampleTime);

	return processed ? S_OK : E_FAIL;
}

HRESULT SampleGrabberCB::QueryInterface(REFIID riid, __RPC__deref_out void __RPC_FAR *__RPC_FAR *ppvObject)
{
	return S_FALSE;
}

ULONG SampleGrabberCB::AddRef(void)
{
	return 1;
}

ULONG SampleGrabberCB::Release(void)
{
	return 1;
}

SampleGrabberCB::SampleGrabberCB(PCWSTR name) : 
SampleIndex(0),
pMediaState(NULL),
StreamNumber(0),
MainWindow(NULL),
bProcess(true),
lastMediaTime(-1),
SampleProcessed(0),
SampleDropped(0),
ProcSampleMin(0),
ProcSampleMax(0),
ProcSampleAvg(0),
ProcSampleTotal(0),
m_mediaSample(NULL),
m_mediaBuffer(NULL)
{
	if (name)
	{
		wcscpy_s(this->name, 100, name);
	}
	else
	{
		wcscpy_s(this->name,100, L"SampleGrabberCB");
	}

	filename[0] = L'\0';
	file = NULL;
}

void SampleGrabberCB::Reset()
{
	bProcess = true;

	if(m_mediaSample)
	{
		m_mediaSample->release();
		m_mediaSample = NULL;
	}

	if(m_mediaBuffer)
	{
		m_mediaBuffer->release();
		m_mediaBuffer = NULL;
	}

	if (file)
	{
		fclose(file);
		file = NULL;
	}

	SampleIndex = 0;
	pMediaState = NULL;

	SampleProcessed = 0;
	SampleDropped = 0;

	lastMediaTime = -1;

	ProcSampleMin = 0;
	ProcSampleMax = 0;
	ProcSampleAvg = 0;
	ProcSampleTotal = 0;
}

SampleGrabberCB::~SampleGrabberCB()
{
	Reset();
}

bool SampleGrabberCB::SetOutputFile(PCWSTR filename)
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


/// AudioGrabber Callback

AudioGrabberCB::AudioGrabberCB() : 
SampleGrabberCB(L"AudioGrabberCB")
{
	StreamNumber = 0;
}

HRESULT AudioGrabberCB::SampleCB(double SampleTime, IMediaSample *pSample)
{
	return SampleGrabberCB::SampleCB(SampleTime, pSample);
}

/// VideoGrabber Callback

VideoGrabberCB::VideoGrabberCB() : 
SampleGrabberCB(L"VideoGrabberCB")
{
	StreamNumber = 1;
}

HRESULT VideoGrabberCB::SampleCB(double SampleTime, IMediaSample *pSample)
{
	return SampleGrabberCB::SampleCB(SampleTime, pSample);
}
