
// VideoToDVD.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "VideoToDVD.h"
#include "VideoToDVDDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// VideoToDVDApp

BEGIN_MESSAGE_MAP(VideoToDVDApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// VideoToDVDApp construction

VideoToDVDApp::VideoToDVDApp()
{
	// Place all significant initialization in InitInstance
}


// The one and only VideoToDVDApp object

VideoToDVDApp theApp;


// VideoToDVDApp initialization

BOOL VideoToDVDApp::InitInstance()
{
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

	// Initialize GDI+.
	GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR           gdiplusToken;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	// Replace the values with your license for DVDBuilder.
	primo::dvdbuilder::Library::setLicense("YOUR DVDBULDER LICENSE XML");

	primo::avblocks::Library::initialize();
	primo::avblocks::Library::setLicense("YOUR AVBLOCKS LICENSE XML");

	// Standard initialization
	// If you are not using these features and wish to reduce the size
	// of your final executable, you should remove from the following
	// the specific initialization routines you do not need
	// Change the registry key under which our settings are stored
	// You should modify this string to be something appropriate
	// such as the name of your company or organization
	SetRegistryKey(_T("Local AppWizard-Generated Applications"));

	VideoToDVDDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// Place code here to handle when the dialog is
		//  dismissed with OK
	}
	else if (nResponse == IDCANCEL)
	{
		// Place code here to handle when the dialog is
		//  dismissed with Cancel
	}

	primo::avblocks::Library::shutdown();

	// Shutdown GDI+.
	GdiplusShutdown(gdiplusToken);

	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
