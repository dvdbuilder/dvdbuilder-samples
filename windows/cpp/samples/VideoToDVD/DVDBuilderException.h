#pragma once

class DVDBuilderException: public BaseException
{
public:

	DVDBuilderException(int error, int errorFacility, const tstring &hint)
	{
		m_error = error;
		m_errorFacility = errorFacility;
		m_hint = hint;

		switch(m_errorFacility)
		{
		case primo::dvdbuilder::ErrorFacility::Success:
			m_message = _T("Success.");
			break;

		case primo::dvdbuilder::ErrorFacility::SystemWindows:
			{
				CString str;
				str.Format(_T("System error: %d"), m_error);
				m_message = str;
			}
			break;

		case primo::dvdbuilder::ErrorFacility::DVDBuilder:
			{
				CString str;
				str.Format(_T("DVDBuilder error: %d"), m_error);
				m_message = str;
			}
			return;
			
		default:
			m_message = _T("Unknown error.");
			return;
			
		}
	}

protected:

    int m_error;
    int m_errorFacility;
    tstring m_hint;
};
