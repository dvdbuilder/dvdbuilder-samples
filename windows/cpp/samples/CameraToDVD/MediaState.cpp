#include "StdAfx.h"
#include "MediaState.h"

MediaState::MediaState(void)
{
	pMediaControl = NULL;
	pGraph = NULL;
	pCaptureGraph = NULL;
	pAudioInput = NULL;
	pVideoInput = NULL;
	pSmartTee = NULL;
	pPreviewRenderer = NULL;

	pAudioGrabberFilter = NULL;
	pVideoGrabberFilter = NULL;
	pAudioGrabber = NULL;
	pVideoGrabber = NULL;

	pAudioNullRenderer = NULL;
	pVideoNullRenderer = NULL;

	dwROT = 0;

	VideoType.pbFormat = NULL;
	VideoType.pUnk = NULL;

	pDroppedFrames = NULL;

	pMpeg2Enc = NULL;
	pVideoRecorder = NULL;

	InitializeCriticalSection(&transcoderSync);
}

MediaState::~MediaState(void)
{ 
	Reset(true);

	DeleteCriticalSection(&transcoderSync);
}

void MediaState::Reset(bool full)
{
	if (pMediaControl)
	{
		pMediaControl->StopWhenReady();
		SafeRelease(pMediaControl);
	}

	FreeMediaType(VideoType);

	if(pMpeg2Enc)
	{
		pMpeg2Enc->release();
		pMpeg2Enc = NULL;
	}

	if (pPreviewRenderer)
	{
		IVideoWindow* pWindow = NULL;
		pGraph->QueryInterface<IVideoWindow>(&pWindow);
		if (pWindow)
		{
			pWindow->put_Owner(NULL);
			pWindow->put_Visible(OAFALSE);
			pWindow->Release();
		}
	}

	if (pVideoInput)
	{
		NukeDownstream(pGraph, pVideoInput);
	}

	if (pAudioInput)
	{
		NukeDownstream(pGraph, pAudioInput);
	}

	SafeRelease(pAudioNullRenderer);
	SafeRelease(pVideoNullRenderer);
	pAudioGrabber = NULL;
	pVideoGrabber = NULL;
	SafeRelease(pAudioGrabberFilter);
	SafeRelease(pVideoGrabberFilter);
	SafeRelease(pDroppedFrames);
	SafeRelease(pPreviewRenderer);
	SafeRelease(pSmartTee);

	if (full)
	{
		SafeRelease(pVideoInput);
		SafeRelease(pAudioInput);
		SafeRelease(pGraph);
		SafeRelease(pCaptureGraph);

		if (dwROT)
		{
			RemoveGraphFromRot(dwROT);
			dwROT = 0;
		}
	}

	if (pVideoRecorder)
	{
		bool_t res = pVideoRecorder->stop();
		if (!res)
		{
			ATLTRACE(L"VideoRecorder::Stop() failed");
			CheckRecorderDevices(pVideoRecorder);
		}

		//TODO: put the recorder in a drive array; notify in parallel and destroy devices
		for (int i = 0; i < pVideoRecorder->devices()->count(); i++)
		{
			if (pVideoRecorder->deviceError(i)->facility() == ErrorFacility::Success)
				pVideoRecorder->devices()->item(i)->notifyOSFileSystemChanged();
		}

		pVideoRecorder->release();
		pVideoRecorder = NULL;
	}
}

