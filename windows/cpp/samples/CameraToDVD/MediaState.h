#pragma once

class MediaState
{
public:
	MediaState(void);
	~MediaState(void);

	IMediaControl* pMediaControl;
	IFilterGraph2* pGraph;
	ICaptureGraphBuilder2* pCaptureGraph;
	IBaseFilter* pAudioInput;
	IBaseFilter* pVideoInput;
	IBaseFilter* pSmartTee;
	IBaseFilter* pPreviewRenderer;

	IBaseFilter* pAudioGrabberFilter;
	IBaseFilter* pVideoGrabberFilter;
	ISampleGrabber* pAudioGrabber;
	ISampleGrabber* pVideoGrabber;

	IBaseFilter* pAudioNullRenderer;
	IBaseFilter* pVideoNullRenderer;

	DWORD dwROT;

	AM_MEDIA_TYPE VideoType;
	IAMDroppedFrames* pDroppedFrames;

	CRITICAL_SECTION transcoderSync;

	primo::avblocks::Transcoder *pMpeg2Enc;
	primo::dvdbuilder::VR::VideoRecorder* pVideoRecorder;

	void Reset(bool full);

};
