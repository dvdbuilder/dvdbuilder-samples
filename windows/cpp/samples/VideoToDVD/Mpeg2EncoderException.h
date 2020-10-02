#pragma once

#include "BaseException.h"


class Mpeg2EncoderException : public BaseException
{
public:

	Mpeg2EncoderException(int error, int errorFacility)
	{
        m_error = error;
        m_errorFacility = errorFacility;

		CString str;
		str.Format(_T("ErrorFacility: %d, Error: %d"), m_errorFacility, m_error);
		m_message = str;
	}

protected:

    int m_error;
    int m_errorFacility;
};
