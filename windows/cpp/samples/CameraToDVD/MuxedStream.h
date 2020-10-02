#pragma once

class MuxedStreamCB: public primo::Stream
{
	bool WriteData(uint8_t *pBuf, uint32_t bufSize);

public:
	MuxedStreamCB();
	~MuxedStreamCB();
	bool SetOutputFile(PCWSTR filename);
	void Reset();
	
	primo::dvdbuilder::VR::VideoRecorder* pVideoRecorder;
	HWND MainWindow;

	// primo::Stream
	virtual bool_t open();
	virtual void close();
	virtual bool_t isOpen() const;
	virtual bool_t canRead() const;
	virtual bool_t canWrite() const;
	virtual bool_t canSeek() const;
	virtual bool_t read(void* buffer, int32_t bufferSize, int32_t* totalRead);
	virtual bool_t write(const void* buffer, int32_t dataSize);
	virtual int64_t size() const;
	virtual int64_t position() const;
	virtual bool_t seek(int64_t position);


private:
	WCHAR filename[256];
	FILE* file;
	int writeCounter;
	bool bProcess;
};