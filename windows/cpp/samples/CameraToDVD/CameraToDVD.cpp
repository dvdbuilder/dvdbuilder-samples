
// CameraToDVD.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "CameraToDVD.h"
#include "CameraToDVDDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CameraToDVDApp

BEGIN_MESSAGE_MAP(CameraToDVDApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CameraToDVDApp construction

CameraToDVDApp::CameraToDVDApp()
{
}


// The one and only CameraToDVDApp object

CameraToDVDApp theApp;


// CameraToDVDApp initialization

BOOL CameraToDVDApp::InitInstance()
{
	CoInitialize(NULL);

	// InitCommonControlsEx() is required on Windows XP if an application
	// manifest specifies use of ComCtl32.dll version 6 or later to enable
	// visual styles.  Otherwise, any window creation will fail.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// Set this to include all the common control classes you want to use
	// in your application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	// Replace the values with your license for DVDBuilder.
	primo::dvdbuilder::Library::setLicense("YOUR DVDBULDER LICENSE");

	primo::avblocks::Library::initialize();

	// Replace the values with your license for AVBlocks.
	primo::avblocks::Library::setLicense("YOUR AVBLOCKS LICENSE");

	/*
	if (primo::dvdbuilder::Library::GetIsLicensed())
		ATLTRACE(L"DVDBuilder: FULL mode");
	else
		ATLTRACE(L"DVDBuilder: DEMO mode");
	*/

	CameraToDVDDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	
	primo::avblocks::Library::shutdown();

	CoUninitialize();

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
