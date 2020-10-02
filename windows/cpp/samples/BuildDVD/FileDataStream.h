class FileDataStream: public primo::Stream
{
	mutable int m_ref;
	FILE* m_fh;
	tstring m_streamID;
public:
	FileDataStream(const char_t* pStreamID)
	{
		m_ref = 0;
		m_fh = NULL;
		m_streamID = pStreamID;
	}

	virtual ~FileDataStream()
	{
		close();
	}

	bool_t open()
	{
		close();
		m_fh = _tfopen(m_streamID.c_str(), _T("rb"));
		return (m_fh != NULL);
	}

	void close()
	{
		if(NULL != m_fh)
		{
			fclose(m_fh);
			m_fh = NULL;
		}
	}

	bool_t isOpen() const
	{
		return NULL != m_fh;
	}

	bool_t canRead() const
	{
		return TRUE;
	}

	bool_t canWrite() const
	{
		return FALSE;
	}

	bool_t canSeek() const
	{
		return FALSE;
	}

	bool_t read(void* buffer, int32_t bufferSize, int32_t* totalRead)
	{
		if (!isOpen())
			return FALSE;

		if(!totalRead)
			return FALSE;

		*totalRead = (uint32_t)fread(buffer, sizeof(uint8_t), bufferSize, m_fh);
		return TRUE;
	}

	bool_t write(const void* buffer, int32_t dataSize)
	{
		// not implemented
		return FALSE;
	}

	int64_t size() const
	{
		// not implemented
		return 0;
	}

	int64_t position() const
	{
		// not implemented
		return -1;
	}

	bool_t seek(int64_t position)
	{
		// not implemented
		return FALSE;
	}

	int32_t retain() const 
	{ 
		m_ref += 1;
		return m_ref; 
	}

	int32_t release() const 
	{ 
		m_ref -= 1;

		if(0 == m_ref)
		{
			delete this;
		}

		return m_ref; 
	}

	int32_t retainCount() const { return m_ref; }
};


class FileDataStreamFactory: public InputDataStreamFactory
{
public:
	virtual primo::Stream* create(const char_t* pFile)
	{
		FileDataStream *pImpl = new FileDataStream(pFile);
		pImpl->retain();
		return pImpl;
	}
};
