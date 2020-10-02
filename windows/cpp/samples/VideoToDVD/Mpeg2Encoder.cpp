#include "stdafx.h"
#include "Mpeg2Encoder.h"
#include "Mpeg2EncoderException.h"

using namespace primo::avblocks;

struct LocalContext {
	primo::avblocks::MediaInfo*	mediaInfo;
	MediaSocket*	inSocket;
	MediaSocket*	outSocket;
	Transcoder*		transcoder;

	LocalContext() : mediaInfo(NULL), inSocket(NULL), outSocket(NULL), transcoder(NULL) {}

	// cleanup
	~LocalContext() {
		if (mediaInfo)		mediaInfo->release();
		if (inSocket)		inSocket->release();
		if (outSocket)		outSocket->release();
		if (transcoder)	transcoder->release();
	};

} lc;

void Mpeg2Encoder::convert()
{
	LocalContext lc;

	lc.mediaInfo = primo::avblocks::Library::createMediaInfo();
	lc.mediaInfo->setInputFile(m_inputFile.c_str());
	
	if (!lc.mediaInfo->load())
		throw Mpeg2EncoderException(lc.mediaInfo->error()->facility(), lc.mediaInfo->error()->code());

	lc.transcoder = primo::avblocks::Library::createTranscoder();

	// In order to use the production release for testing (without a valid license) the transcoder demo mode must be enabled.
	lc.transcoder->setAllowDemoMode(TRUE);

	if (m_callback)
	{
		lc.transcoder->setCallback(m_callback);
	}

	lc.inSocket = primo::avblocks::Library::createMediaSocket(lc.mediaInfo);
	lc.transcoder->inputs()->add( lc.inSocket );

	lc.outSocket = primo::avblocks::Library::createMediaSocket(m_outputPreset.c_str());
	if (!lc.outSocket)
		throw Mpeg2EncoderException(-1, -1);

	lc.outSocket->setFile(m_outputFile.c_str());
	
	lc.transcoder->outputs()->add( lc.outSocket );

	if (!lc.transcoder->open())
		throw Mpeg2EncoderException(lc.transcoder->error()->facility(), lc.transcoder->error()->code());

	if (!lc.transcoder->run())
		throw Mpeg2EncoderException(lc.transcoder->error()->facility(), lc.transcoder->error()->code());
}
