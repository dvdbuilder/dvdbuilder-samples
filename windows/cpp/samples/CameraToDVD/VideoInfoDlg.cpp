// VideoInfo.cpp : implementation file
//

#include "stdafx.h"
#include "CameraToDVD.h"
#include "VideoInfoDlg.h"


// VideoInfo dialog

IMPLEMENT_DYNAMIC(CVideoInfoDlg, CDialog)

CVideoInfoDlg::CVideoInfoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CVideoInfoDlg::IDD, pParent)
{
	m_bInit = false;
}

CVideoInfoDlg::~CVideoInfoDlg()
{
}

void CVideoInfoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TXT_VIDEO_INFO, m_txtVideoInfo);
}


BEGIN_MESSAGE_MAP(CVideoInfoDlg, CDialog)
	ON_MESSAGE(WM_KICKIDLE, OnKickIdle)
END_MESSAGE_MAP()


// VideoInfo message handlers
BOOL CVideoInfoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	m_txtVideoInfo.SetWindowTextW(Info);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

LRESULT CVideoInfoDlg::OnKickIdle(WPARAM wp, LPARAM lp)
{
	if (!m_bInit)
	{
		m_bInit = true;
		m_txtVideoInfo.SetSel(-1,-1);
	}
    
    return TRUE;
}
