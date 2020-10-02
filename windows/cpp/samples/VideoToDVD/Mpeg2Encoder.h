#pragma once

#include <string>

class Mpeg2Encoder
{
private:
	std::wstring m_errorMessage; // utf-16
	std::wstring m_inputFile;	// utf-16
	std::wstring m_outputFile;	// utf-16
	std::string m_outputPreset;
	primo::avblocks::TranscoderCallback * m_callback;

public:
	Mpeg2Encoder(void) : m_callback(NULL) {}
	virtual ~Mpeg2Encoder(void) {}

	void setCallback(primo::avblocks::TranscoderCallback * callback)
	{
		m_callback = callback;
	}

	// utf-16 input
	void setInputFile(const wchar_t* filename)
	{
		if (filename)
			m_inputFile = filename;
		else
			m_inputFile.clear(); 
	}
	
	// utf-16 input
	void setOutputFile(const wchar_t* filename)
	{
		if (filename)
			m_outputFile = filename;
		else
			m_outputFile.clear();
	}
	
	void setOutputPreset(const char* preset)
	{
		m_outputPreset = preset;
	}

	void convert();
};
