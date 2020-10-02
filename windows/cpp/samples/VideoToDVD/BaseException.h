#pragma once


class BaseException
{
protected:
	tstring m_message;

public:
	BaseException(){}
	BaseException(const tstring &msg):m_message(msg){}

	virtual const tstring& get_Message() const { return m_message; }
};
