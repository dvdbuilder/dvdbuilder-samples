#pragma once
#include "afxwin.h"


// VideoInfo dialog

class CVideoInfoDlg : public CDialog
{
	DECLARE_DYNAMIC(CVideoInfoDlg)

public:
	CVideoInfoDlg(CWnd* pParent = NULL);   // standard constructor
	virtual ~CVideoInfoDlg();
	CString Info;

private:
	bool m_bInit;

// Dialog Data
	enum { IDD = IDD_VIDEOINFO };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	afx_msg LRESULT OnKickIdle(WPARAM, LPARAM);

	DECLARE_MESSAGE_MAP()
public:
	CEdit m_txtVideoInfo;
};
