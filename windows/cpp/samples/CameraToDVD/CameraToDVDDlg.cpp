
// CameraToDVDDlg.cpp : implementation file
//

#include "stdafx.h"
#include "CameraToDVD.h"
#include "CameraToDVDDlg.h"
#include "VideoInfoDlg.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

HWND g_mainWindow;

using namespace primo::dvdbuilder::VR;

// CameraToDVDDlg dialog

CameraToDVDDlg::CameraToDVDDlg(CWnd* pParent /*=NULL*/)
: CDialog(CameraToDVDDlg::IDD, pParent),
m_updateStatsEvent(1)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_bRecording = false;
	m_bCmdRecordBusy = false;
	m_bExiting = false;
}

void CameraToDVDDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_AUDIO_DEV, m_listAudioDev);
	DDX_Control(pDX, IDC_CMD_AUDIO_DEV_PROP, m_cmdAudioDevProp);
	DDX_Control(pDX, IDC_LIST_VIDEO_DEV, m_listVideoDev);
	DDX_Control(pDX, IDC_CMD_VIDEO_CAPTURE_PROP, m_cmdVideoCaptureProp);
	DDX_Control(pDX, IDC_CMD_DUMMY_WRITE, m_cmdSimulate);
	DDX_Control(pDX, IDC_CMD_QUERY_FINALIZE, m_cmdQueryFinalize);
	DDX_Control(pDX, IDC_CMD_RECORD, m_cmdRecord);
	DDX_Control(pDX, IDC_TXT_RECORDING, m_txtRecording);
	DDX_Control(pDX, IDC_PREVIEW, m_preview);
	DDX_Control(pDX, IDC_CMD_PREVIEW, m_cmdPreview);
	DDX_Control(pDX, IDC_CMD_ERASE_DISC, m_cmdEraseDisc);
	DDX_Control(pDX, IDC_LIST_DRIVES_2, m_listDrives);
	DDX_Control(pDX, IDC_EDIT_FOLDER, m_editFolder);
	DDX_Control(pDX, IDC_CMD_BROWSE_FOLDER, m_cmdBrowseFolder);
	DDX_Control(pDX, IDC_CMD_CLEAR_FOLDER, m_cmdClearFolder);
	DDX_Control(pDX, IDC_CMD_VIDEO_INFO, m_cmdVideoInfo);
}

BEGIN_MESSAGE_MAP(CameraToDVDDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_CMD_EXIT, &CameraToDVDDlg::OnCmdExit)
	ON_WM_SYSCOMMAND()
	ON_WM_CTLCOLOR()
	ON_CBN_SELCHANGE(IDC_LIST_AUDIO_DEV, &CameraToDVDDlg::OnCbnSelchangeListAudioDev)
	ON_CBN_SELCHANGE(IDC_LIST_VIDEO_DEV, &CameraToDVDDlg::OnCbnSelchangeListVideoDev)
	ON_BN_CLICKED(IDC_CMD_AUDIO_DEV_PROP, &CameraToDVDDlg::OnCmdAudioDevProp)
	ON_BN_CLICKED(IDC_CMD_VIDEO_DEV_PROP, &CameraToDVDDlg::OnCmdVideoDevProp)
	ON_BN_CLICKED(IDC_CMD_VIDEO_CAPTURE_PROP, &CameraToDVDDlg::OnCmdVideoCaptureProp)
	ON_BN_CLICKED(IDC_CMD_RECORD, &CameraToDVDDlg::OnCmdRecord)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDC_CMD_QUERY_FINALIZE, &CameraToDVDDlg::OnCmdFinalize)
	ON_BN_CLICKED(IDC_CMD_PREVIEW, &CameraToDVDDlg::OnCmdPreview)
	ON_BN_CLICKED(IDC_CMD_ERASE_DISC, &CameraToDVDDlg::OnCmdEraseDisc)
	ON_BN_CLICKED(IDC_CMD_BROWSE_FOLDER, &CameraToDVDDlg::OnCmdBrowseFolder)
	ON_BN_CLICKED(IDC_CMD_DUMMY_WRITE, &CameraToDVDDlg::OnCmdSimulate)
	ON_BN_CLICKED(IDC_CMD_CLEAR_FOLDER, &CameraToDVDDlg::OnCmdClearFolder)
	ON_BN_CLICKED(IDC_CMD_VIDEO_INFO, &CameraToDVDDlg::OnCmdVideoInfo)
END_MESSAGE_MAP()


// CameraToDVDDlg message handlers

BOOL CameraToDVDDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

#ifdef WIN64
	CString title;
	GetWindowText(title);
	title += L" (64-bit)";
	SetWindowText(title);
#endif

	EnumInputDev(CLSID_AudioInputDeviceCategory, m_listAudioDev, m_audioDevices);
	EnumInputDev(CLSID_VideoInputDeviceCategory, m_listVideoDev, m_videoDevices);

	if (m_audioDevices.size() > 0)
		m_listAudioDev.SetCurSel(0);

	if (m_videoDevices.size() > 0)
		m_listVideoDev.SetCurSel(0);

	m_cmdRecord.SetWindowTextW(L"Start Recording");          
	GetDlgItem(IDC_TXT_RECORDING)->ShowWindow(SW_HIDE);
	ResetStats();

	int audioItem = m_listAudioDev.GetCurSel();
	int videoItem = m_listVideoDev.GetCurSel();

	int hr = InitInputDev(m_ms, videoItem, audioItem);
	if (FAILED(hr))
	{
		MessageBox(_T("Cannot use the selected capture devices"));
		return TRUE;
	}

	AddGraphToRot(m_ms.pGraph,&m_ms.dwROT);

	primo::dvdbuilder::VR::VideoRecorder* pRecorder =  primo::dvdbuilder::Library::createVideoRecorder();

	ATLASSERT(pRecorder);

	std::wstring pbPlugin = L"VRPBDevice.dll";
	std::wstring fsPlugin = L"VRFSDevice.dll";
#if WIN64
	pbPlugin = L"VRPBDevice64.dll";
	fsPlugin = L"VRFSDevice64.dll";
#endif
	std::wstring pluginPath = L"..\\..\\..\\..\\..\\..\\..\\..\\DVDBuilder.CPP\\DVDBuilderSDK.windows\\lib\\";

	// load PrimoBurner plugin
	m_pDvdPlugin = pRecorder->loadDevicePlugin(pbPlugin.c_str());
	if (!m_pDvdPlugin)
	{
		std::wstring plugin = pluginPath + pbPlugin;
		m_pDvdPlugin = pRecorder->loadDevicePlugin(plugin.c_str());
		if (!m_pDvdPlugin)
		{
			MessageBox(L"Cannot load DVD device plugin");
		}
	}

	// load FileSystem plugin
	m_pFsPlugin = pRecorder->loadDevicePlugin(fsPlugin.c_str());
	if (!m_pFsPlugin)
	{
		std::wstring plugin = pluginPath + fsPlugin;
		m_pFsPlugin = pRecorder->loadDevicePlugin(plugin.c_str());
		if (!m_pFsPlugin)
		{
			MessageBox(L"Cannot load File System plugin");
		}
	}

	pRecorder->release();

	RECT r;
	m_preview.GetWindowRect(&r);
	LONG width = r.right-r.left+1;
	LONG height = 3 * width / 4;
	m_preview.SetWindowPos(0, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
	
	m_cmdPreview.SetCheck(BST_CHECKED);

	g_mainWindow = m_hWnd;
	UpdateDrivesAsync();

	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CameraToDVDDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CameraToDVDDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CameraToDVDDlg::OnCmdExit()
{
	CloseApp();
}

void CameraToDVDDlg::OnOK()
{}

void CameraToDVDDlg::OnCancel()
{}

void CameraToDVDDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	switch(nID) {
	  case SC_CLOSE:

		  CloseApp();
		  break;

	  default:
		  CDialog::OnSysCommand(nID, lParam);
	}
}

HBRUSH CameraToDVDDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	// Change any attributes of the DC here
	if (pWnd->GetDlgCtrlID() == IDC_TXT_RECORDING)
	{
		pDC->SetTextColor(RGB(255, 0, 0));
	}

	return hbr;
}

void CameraToDVDDlg::EnumInputDev(const IID & devClass, CComboBox& list, StringList& devices)
{
	if (devClass != CLSID_AudioInputDeviceCategory &&
		devClass != CLSID_VideoInputDeviceCategory)
	{
		return;
	}

	struct Res
	{
		ICreateDevEnum *pSysDevEnum;
		IEnumMoniker *pEnumCat;
		IMoniker *pMoniker;
		IPropertyBag *pPropBag;

		Res() : pSysDevEnum(0), pEnumCat(0), pMoniker(0), pPropBag(0)
		{}

		~Res()
		{
			if (pPropBag) pPropBag->Release();
			if (pMoniker) pMoniker->Release();
			if (pEnumCat) pEnumCat->Release();
			if (pSysDevEnum) pSysDevEnum->Release();
		}
	} res;






	HRESULT hr = S_OK;
	hr = CoCreateInstance(CLSID_SystemDeviceEnum, NULL, CLSCTX_INPROC_SERVER,
		IID_ICreateDevEnum, (void **)&res.pSysDevEnum);
	if (FAILED(hr))
	{
		TRACE(_T("CoCreateInstance CLSID_SystemDeviceEnum failed"));
		return;
	}

	// Obtain a class enumerator for the capture category.

	hr = res.pSysDevEnum->CreateClassEnumerator(devClass, &res.pEnumCat, 0);

	if (FAILED(hr))
	{
		TRACE(_T("CreateClassEnumerator failed"));
		return;
	}

	if (!res.pEnumCat)
	{
		TRACE(_T("No capture devices found"));
		return;
	}

	// Enumerate the monikers.

	ULONG cFetched;
	while(res.pEnumCat->Next(1, &res.pMoniker, &cFetched) == S_OK)
	{
		hr = res.pMoniker->BindToStorage(0, 0, IID_IPropertyBag, 
			(void **)&res.pPropBag);
		if (SUCCEEDED(hr))
		{
			std::wstring friendlyName;
			std::wstring devName;

			// retrieve the filter's friendly name
			VARIANT varName;
			VariantInit(&varName);
			hr = res.pPropBag->Read(L"FriendlyName", &varName, 0);
			if (SUCCEEDED(hr))
			{
				friendlyName = (wchar_t*)varName.bstrVal;
				ATLTRACE(_T("friendly name: %s\n"),varName.bstrVal);
			}
			VariantClear(&varName);

			LPOLESTR pDisplayName;
			hr = res.pMoniker->GetDisplayName(NULL,NULL,(LPOLESTR*)&pDisplayName);
			devName = pDisplayName;
			ATLTRACE(_T("display name: %s\n"),pDisplayName);
			CoTaskMemFree(pDisplayName);

			// try to create an instance of the filter
			IBaseFilter *pFilter;
			hr = res.pMoniker->BindToObject(NULL, NULL, IID_IBaseFilter, (void**)&pFilter);
			CLSID filterCLSID;

			if (SUCCEEDED(hr))
			{
				hr = pFilter->GetClassID(&filterCLSID);
				if (SUCCEEDED(hr))
				{
					list.AddString(friendlyName.c_str());
					devices.push_back(devName);
				}
				pFilter->Release();
			}

			res.pPropBag->Release();
			res.pPropBag = 0;
		}

		res.pMoniker->Release();
		res.pMoniker = 0;

	} // while



} // EnumInputDev

void CameraToDVDDlg::ResetStats()
{
	const wchar_t* pEmpty = L"--";
	SetDlgItemText(IDC_TXT_REC_TIME, L"0:00:00");
	SetDlgItemText(IDC_TXT_REMAINING_TIME, L"--:--:--");
	SetDlgItemText(IDC_TXT_REMAINING_SPACE, L"--- MB");

	SetDlgItemText(IDC_TXT_NUM_DROPPED,pEmpty);
	SetDlgItemText(IDC_TXT_NUM_PROCESSED,pEmpty);
	SetDlgItemText(IDC_TXT_AVERAGE_FPS,pEmpty);
	SetDlgItemText(IDC_TXT_CURRENT_FPS,pEmpty);

	SetDlgItemText(IDC_TXT_ACB,pEmpty);
	SetDlgItemText(IDC_TXT_ADROP,pEmpty);
	SetDlgItemText(IDC_TXT_APROC,pEmpty);

	SetDlgItemText(IDC_TXT_VCB,pEmpty);
	SetDlgItemText(IDC_TXT_VDROP,pEmpty);
	SetDlgItemText(IDC_TXT_VPROC,pEmpty);
}


void CameraToDVDDlg::UpdateStats()
{
	const int NUMSIZE = 32;
	WCHAR number[NUMSIZE];
	DWORD now = GetTickCount();
	CTimeSpan rec((now - recStartTime)/1000);
	WCHAR recTime[10];
	swprintf_s(recTime,10,L"%.2d:%.2d:%.2d",rec.GetHours(),rec.GetMinutes(),rec.GetSeconds());
	SetDlgItemText(IDC_TXT_REC_TIME, recTime);

	if (m_cmdSimulate.GetCheck() == BST_UNCHECKED)
	{
		// format remaining space
		//DBG DVD-RW free space problem
		uint64_t remSpace = m_ms.pVideoRecorder->mediaFreeSpace();
		swprintf_s(number,NUMSIZE,L"%.2f MB",(double)(remSpace) / (1024*1024));
		SetDlgItemText(IDC_TXT_REMAINING_SPACE, number);

		uint32_t avgBitrate = m_ms.pVideoRecorder->averageBitrate();
		if (avgBitrate > 0)
			m_avgBitrate = avgBitrate;

		CTimeSpan rem(remSpace*8 / m_avgBitrate);
		swprintf_s(recTime,10,L"%.2d:%.2d:%.2d",rem.GetHours(),rem.GetMinutes(),rem.GetSeconds());
		SetDlgItemText(IDC_TXT_REMAINING_TIME, recTime);
	}

	if (m_ms.pDroppedFrames)
	{

		HRESULT hr = S_OK;
		long dropped;
		hr = m_ms.pDroppedFrames->GetNumDropped(&dropped);
		if (S_OK == hr)
		{
			swprintf_s(number,NUMSIZE,L"%d",dropped);
			SetDlgItemText(IDC_TXT_NUM_DROPPED, number);
		}

		long notDropped;
		hr = m_ms.pDroppedFrames->GetNumNotDropped(&notDropped);
		if (S_OK== hr)
		{
			swprintf_s(number,NUMSIZE,L"%d",notDropped);
			SetDlgItemText(IDC_TXT_NUM_PROCESSED, number);
			if (notDropped >= 0)
			{
				double averageFPS = (double)notDropped / rec.GetTotalSeconds();
				swprintf_s(number,NUMSIZE,L"%.3f",averageFPS);
				SetDlgItemText(IDC_TXT_AVERAGE_FPS, number);

				double fpsElapsed = ((double)(now - fpsStartTime)) / 1000;
				if (fpsElapsed > 5.0)
				{
					double curFPS = ((double)(notDropped-fpsProcessed))/ fpsElapsed;
					swprintf_s(number,NUMSIZE,L"%.3f",curFPS);
					SetDlgItemText(IDC_TXT_CURRENT_FPS, number);

					fpsStartTime = now;
					fpsProcessed = notDropped;
				}
			}
		}
	}

	swprintf_s(number,NUMSIZE,L"%d",m_audioCB.SampleIndex);
	SetDlgItemText(IDC_TXT_ACB, number);

	swprintf_s(number,NUMSIZE,L"%d",m_audioCB.SampleProcessed);
	SetDlgItemText(IDC_TXT_APROC, number);

	swprintf_s(number,NUMSIZE,L"%d",m_audioCB.SampleDropped);
	SetDlgItemText(IDC_TXT_ADROP, number);

	swprintf_s(number,NUMSIZE,L"%d",m_videoCB.SampleIndex);
	SetDlgItemText(IDC_TXT_VCB, number);

	swprintf_s(number,NUMSIZE,L"%d",m_videoCB.SampleProcessed);
	SetDlgItemText(IDC_TXT_VPROC, number);

	swprintf_s(number,NUMSIZE,L"%d",m_videoCB.SampleDropped);
	SetDlgItemText(IDC_TXT_VDROP, number);
}


HRESULT CameraToDVDDlg::InitInputDev(MediaState& ms, int videoItem, int audioItem)
{
	HRESULT hr = S_OK;
	// create Filter Graph Manager
	if (!ms.pGraph)
	{
		hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, IID_IFilterGraph2,
			(void **)&ms.pGraph);

		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot create FilterGraph"));
			return hr;
		}

		// Create the Capture Graph Builder.
		hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER, IID_ICaptureGraphBuilder2, 
			(void **)&ms.pCaptureGraph);

		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot create CaptureGraphBuilder2"));
			return hr;
		}

		hr = ms.pCaptureGraph->SetFiltergraph(ms.pGraph);
		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot set Filtergraph"));
			return hr;
		}
	}


	if  (audioItem >= 0)
	{
		// remove the old audio input
		if (ms.pAudioInput)
		{
			hr = ms.pGraph->RemoveFilter(ms.pAudioInput);
			SafeRelease(ms.pAudioInput);
			if (FAILED(hr))
			{
				ATLTRACE(_T("Cannot remove audio input filter"));
				return hr;
			}
		}

		// create audio input
		hr = CoGetObject(m_audioDevices[audioItem].c_str(),NULL, IID_IBaseFilter, (void**)&ms.pAudioInput);
		if (FAILED(hr))
		{
			ATLTRACE(_T("CoGetObject AudioInput failed"));
			return hr;
		}

		// add audio input to the graph
		TCHAR friendlyName[256];
		m_listAudioDev.GetLBText(audioItem, friendlyName);
		hr = ms.pGraph->AddFilter(ms.pAudioInput, friendlyName);
		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot add audio input filter"));
			return hr;
		}
	}


	if (videoItem >= 0)
	{
		// remove the old video input
		if (ms.pVideoInput)
		{
			hr = ms.pGraph->RemoveFilter(ms.pVideoInput);
			SafeRelease(ms.pVideoInput);
			if (FAILED(hr))
			{
				ATLTRACE(_T("Cannot remove video input filter"));
				return hr;
			}
		}

		// create video input
		hr = CoGetObject(m_videoDevices[videoItem].c_str(),NULL, IID_IBaseFilter, (void**)&ms.pVideoInput);
		if (FAILED(hr))
		{
			ATLTRACE(_T("CoGetObject AudioInput failed"));
			return hr;
		}

		// add video input to the graph
		TCHAR friendlyName[256];
		m_listVideoDev.GetLBText(videoItem, friendlyName);
		hr = ms.pGraph->AddFilter(ms.pVideoInput, friendlyName);
		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot add video input filter"));
			return hr;
		}
	}


	return hr;
}
void CameraToDVDDlg::OnCbnSelchangeListAudioDev()
{
	int sel = m_listAudioDev.GetCurSel();
	if (sel < 0)
		return;

	HRESULT hr = InitInputDev(m_ms,-1,sel);
	if (FAILED(hr))
	{
		MessageBox(_T("Cannot use the selected input devices"));
	}
}

void CameraToDVDDlg::OnCbnSelchangeListVideoDev()
{
	int sel = m_listVideoDev.GetCurSel();
	if (sel < 0)
		return;

	HRESULT hr = InitInputDev(m_ms,sel,-1);
	if (FAILED(hr))
	{
		MessageBox(_T("Cannot use the selected input devices"));
	}
}

void CameraToDVDDlg::OnCmdAudioDevProp()
{
	if (!CheckInputDevice(m_ms.pAudioInput))
		return;

	ShowPropPages(m_ms.pAudioInput);
}

void CameraToDVDDlg::OnCmdVideoDevProp()
{
	if (!CheckInputDevice(m_ms.pVideoInput))
		return;

	ShowPropPages(m_ms.pVideoInput);
}

void CameraToDVDDlg::OnCmdVideoCaptureProp()
{
	if (!CheckInputDevice(m_ms.pVideoInput))
		return;

	IAMStreamConfig *pStreamConfig;

	HRESULT hr = m_ms.pCaptureGraph->FindInterface(&PIN_CATEGORY_CAPTURE, &MEDIATYPE_Video,
		m_ms.pVideoInput, IID_IAMStreamConfig, (void **)&pStreamConfig);

	if (FAILED(hr))
	{
		TRACE(_T("FindInterface IID_IAMStreamConfig failed"));
		return;
	}

	ShowPropPages(pStreamConfig);

	pStreamConfig->Release();
}

bool CameraToDVDDlg::CheckInputDevice(IBaseFilter* pInputDevice)
{
	if (!pInputDevice)
	{
		MessageBox(_T("No input device!"));
		return false;
	}
	return true;
}

void CameraToDVDDlg::ShowPropPages(IUnknown* pUnk)
{
	ISpecifyPropertyPages* pSpecPropPages = NULL;

	HRESULT hr = pUnk->QueryInterface<ISpecifyPropertyPages>(&pSpecPropPages);
	if (FAILED(hr))
	{
		MessageBox(_T("Property pages not available"));
		return;
	}

	CAUUID pages;
	hr = pSpecPropPages->GetPages(&pages);

	if (S_OK == hr && pages.cElems > 0)
	{
		// show property pages
		hr = OleCreatePropertyFrame(m_hWnd,
			30, 30, NULL, 1,
			&pUnk, pages.cElems,
			pages.pElems, 0, 0, NULL);

		CoTaskMemFree(pages.pElems);

	}

	pSpecPropPages->Release();

}

void CameraToDVDDlg::OnCmdRecord()
{
	if (m_bCmdRecordBusy)
		return;

	m_bCmdRecordBusy = true;
	m_cmdRecord.EnableWindow(0);

	if (m_bRecording)
	{
		// Stop Recording
		StopRecording();

		m_txtRecording.ShowWindow(SW_HIDE);
		m_cmdRecord.SetWindowTextW(L"Start Recording");

		EnableCommandUI(1);
		EnableDeviceUI(1);

		if (!m_bExiting)
			UpdateDrivesAsync();

		m_cmdPreview.EnableWindow(1);
		m_bRecording = false;
	}
	else
	{
		// Start Recording
		EnableCommandUI(0);
		EnableDeviceUI(0);
		if (StartRecording())
		{
			if (m_cmdPreview.GetCheck() == BST_UNCHECKED)
			{
				m_cmdPreview.EnableWindow(0);
			}
			m_txtRecording.ShowWindow(SW_SHOW);
			m_cmdRecord.SetWindowTextW(L"Stop Recording");
			m_bRecording = true;
		}
		else
		{
			EnableCommandUI(1);
			EnableDeviceUI(1);
		}
	}

	m_cmdRecord.EnableWindow(1);
	m_bCmdRecordBusy = false;
}

bool CameraToDVDDlg::StartRecording()
{
	HRESULT hr = S_OK;

	if (!m_ms.pAudioInput || !m_ms.pVideoInput)
	{
		MessageBox(_T("No audio or video input!"));
		return false;
	}

	struct CLEANUP
	{
		bool invoke;
		CLEANUP(bool b) :
		ms(0), videoCB(0), audioCB(0), muxedCB(0),
			invoke(b),
			audioConfig(0),pMediaType(0),videoWindow(0)
		{}

		MediaState* ms;
		VideoGrabberCB* videoCB;
		AudioGrabberCB* audioCB;
		MuxedStreamCB* muxedCB;
		IAMStreamConfig* audioConfig;
		AM_MEDIA_TYPE* pMediaType;
		IVideoWindow* videoWindow;

		~CLEANUP()
		{
			if (invoke)
			{
				if (ms) ms->Reset(false);
				if (videoCB) videoCB->Reset();
				if (audioCB) audioCB->Reset();
				if (muxedCB) muxedCB->Reset();
			}

			if (audioConfig) audioConfig->Release();
			if (videoWindow) videoWindow->Release();
			if (pMediaType) DeleteMediaType(pMediaType);
		}

	} cleanup(true);


	cleanup.ms = &m_ms;

	// Create the Video Sample Grabber
	hr = CoCreateInstance(CLSID_SampleGrabber, NULL, CLSCTX_INPROC_SERVER,
		IID_IBaseFilter, (void**)&m_ms.pVideoGrabberFilter);

	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot create SampleGrabber"));
		return false;
	}

	hr = m_ms.pGraph->AddFilter(m_ms.pVideoGrabberFilter, L"Video SampleGrabber");
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot Add Filter"));
		return false;
	}

	// get the ISampleGrabber interface.
	hr = m_ms.pVideoGrabberFilter->QueryInterface<ISampleGrabber>(&m_ms.pVideoGrabber);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot obtain ISampleGrabber"));
		return false;
	}

	// Create the Audio Sample Grabber
	hr = CoCreateInstance(CLSID_SampleGrabber, NULL, CLSCTX_INPROC_SERVER,
		IID_IBaseFilter, (void**)&m_ms.pAudioGrabberFilter);

	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot create SampleGrabber"));
		return false;
	}

	hr = m_ms.pGraph->AddFilter(m_ms.pAudioGrabberFilter, L"Audio SampleGrabber");
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot Add Filter"));
		return false;
	}

	// get the ISampleGrabber interface.
	hr = m_ms.pAudioGrabberFilter->QueryInterface<ISampleGrabber>(&m_ms.pAudioGrabber);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot obtain ISampleGrabber"));
		return false;
	}

	// Create and add the audio null renderer in the graph
	hr = CoCreateInstance(CLSID_NullRenderer, NULL, CLSCTX_INPROC_SERVER,
		IID_IBaseFilter, (void**)&m_ms.pAudioNullRenderer);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot create NullRenderer"));
		return false;
	}

	hr = m_ms.pGraph->AddFilter(m_ms.pAudioNullRenderer, L"Audio Null Renderer");
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot add NullRenderer"));
		return false;
	}

	// Create and add the video null renderer in the graph
	hr = CoCreateInstance(CLSID_NullRenderer, NULL, CLSCTX_INPROC_SERVER,
		IID_IBaseFilter, (void**)&m_ms.pVideoNullRenderer);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot create NullRenderer"));
		return false;
	}

	hr = m_ms.pGraph->AddFilter(m_ms.pVideoNullRenderer, L"Video Null Renderer");
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot add NullRenderer"));
		return false;
	}

	// manually connect the filters
	hr = ConnectFilters(m_ms.pGraph, m_ms.pAudioInput, m_ms.pAudioGrabberFilter);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot connect filters"));
		return false;
	}

	hr = ConnectFilters(m_ms.pGraph, m_ms.pAudioGrabberFilter, m_ms.pAudioNullRenderer);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot connect filters"));
		return false;
	}

	// add the smart tee if preview is required

	if (BST_CHECKED == m_cmdPreview.GetCheck() && !m_ms.pSmartTee)
	{
		hr = CoCreateInstance(CLSID_SmartTee, NULL, CLSCTX_INPROC_SERVER,
			IID_IBaseFilter, (void**)&m_ms.pSmartTee);

		hr = m_ms.pGraph->AddFilter(m_ms.pSmartTee, L"Smart Tee");

		if (FAILED(hr))
		{
			ATLTRACE(_T("Cannot add Smart Tee"));
			return false;
		}
	}

	if (m_ms.pSmartTee)
	{
		// connect the video input to the smart tee
		ConnectFilters(m_ms.pGraph, m_ms.pVideoInput, m_ms.pSmartTee);

		// connect smart tee capture to video grabber
		IPin* pCapturePin = NULL;
		hr = GetPin(m_ms.pSmartTee, PINDIR_OUTPUT, L"Capture", &pCapturePin);
		if (FAILED(hr))
		{
			return false;
		}

		IPin* pVideoGrabberPin = NULL;
		hr = GetUnconnectedPin(m_ms.pVideoGrabberFilter, PINDIR_INPUT, &pVideoGrabberPin);
		if (FAILED(hr))
		{
			return false;
		}

		hr = m_ms.pGraph->ConnectDirect(pCapturePin, pVideoGrabberPin, NULL);
		if (FAILED(hr))
		{
			return false;
		}

		// connect smart tee preview to video renderer
		hr = CoCreateInstance(CLSID_VideoRendererDefault, NULL, CLSCTX_INPROC_SERVER,
			IID_IBaseFilter, (void**)&m_ms.pPreviewRenderer);
		if (FAILED(hr))
		{
			return false;
		}

		hr = m_ms.pGraph->AddFilter(m_ms.pPreviewRenderer, L"Preview Renderer");
		if (FAILED(hr))
		{
			return false;
		}

		IPin* pPreviewPin;
		hr = GetPin(m_ms.pSmartTee, PINDIR_OUTPUT, L"Preview", &pPreviewPin);
		if (FAILED(hr))
		{
			return false;
		}

		IPin* pVideoRendererPin;
		hr = GetUnconnectedPin(m_ms.pPreviewRenderer, PINDIR_INPUT, &pVideoRendererPin);
		if (FAILED(hr))
		{
			return false;
		}

		hr = m_ms.pGraph->Connect(pPreviewPin, pVideoRendererPin);
		if (FAILED(hr))
		{
			return false;
		}
	}
	else
	{
		hr = ConnectFilters(m_ms.pGraph, m_ms.pVideoInput, m_ms.pVideoGrabberFilter);
		if (FAILED(hr))
		{
			return false;
		}
	}

	hr = ConnectFilters(m_ms.pGraph, m_ms.pVideoGrabberFilter, m_ms.pVideoNullRenderer);
	if (FAILED(hr))
	{
		return false;
	}

	hr = m_ms.pGraph->QueryInterface<IMediaControl>(&m_ms.pMediaControl);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot obtain IMediaControl"));
		return false;
	}

	hr = m_ms.pAudioGrabber->SetCallback(&m_audioCB, CBMethod::Sample);
	if (FAILED(hr))
	{
		return false;
	}

	hr = m_ms.pVideoGrabber->SetCallback(&m_videoCB, CBMethod::Sample);
	if (FAILED(hr))
	{
		return false;
	}

	// Store the video media type for later use.
	hr = m_ms.pVideoGrabber->GetConnectedMediaType(&m_ms.VideoType);
	if (FAILED(hr))
	{
		return false;
	}

	// pass the media state to the callbacks
	m_audioCB.pMediaState = &m_ms;
	m_videoCB.pMediaState = &m_ms;
	m_audioCB.MainWindow = m_hWnd;
	m_videoCB.MainWindow = m_hWnd;

	// grab raw input in file
	//m_audioCB.SetOutputFile(L"d:\\tmp\\audio.dat");
	//m_videoCB.SetOutputFile(L"d:\\tmp\\video.dat");

	cleanup.audioCB = &m_audioCB;
	cleanup.videoCB = &m_videoCB;

	IAMStreamConfig* pAudioConfig;
	hr = m_ms.pCaptureGraph->FindInterface(&PIN_CATEGORY_CAPTURE, NULL, m_ms.pAudioInput, IID_IAMStreamConfig, (void**)&pAudioConfig);
	if (FAILED(hr))
	{
		ATLTRACE(_T("Cannot obtain IAMStreamConfig"));
		return false;
	}
	cleanup.audioConfig = pAudioConfig;

	AM_MEDIA_TYPE* pAudioType = NULL;
	pAudioConfig->GetFormat(&pAudioType);
	if (FAILED(hr))
	{
		return false;
	}
	cleanup.pMediaType = pAudioType;

	// set audio capture parameters
	WAVEFORMATEX *pWFX = (WAVEFORMATEX*)pAudioType->pbFormat;
	pWFX->nSamplesPerSec = 48000;
	pWFX->nChannels = 2;
	pWFX->wBitsPerSample = 16;
	pWFX->nBlockAlign = pWFX->nChannels * pWFX->wBitsPerSample / 8;
	pWFX->nAvgBytesPerSec = pWFX->nSamplesPerSec * pWFX->nBlockAlign;
	pWFX->wFormatTag = 1; // PCM
	hr = pAudioConfig->SetFormat(pAudioType);
	if (FAILED(hr))
	{
		return false;
	}

	hr = m_ms.pVideoInput->QueryInterface<IAMDroppedFrames>(&m_ms.pDroppedFrames);
	//the video capture device may not support IAMDroppedFrames

	VIDEOINFOHEADER* pvh = (VIDEOINFOHEADER*)m_ms.VideoType.pbFormat;

	m_ms.pMpeg2Enc = primo::avblocks::Library::createTranscoder();

	// In order to use the OEM release for testing (without a valid license) the transcoder demo mode must be enabled.
	m_ms.pMpeg2Enc->setAllowDemoMode(TRUE);

	if(!ConfigureTranscoderInput(pWFX, pvh))
		return false;

	m_muxedCB.Reset();
	// grab recorded dvd video in file
	//res = m_muxedCB.SetOutputFile(L"d:\\tmp\\av.mpg");
	m_muxedCB.MainWindow = m_hWnd;

	const int videoBitrate = 3500000; // max? 6000000
	// configure output
	{
		double videoFps = (double)10000000 / pvh->AvgTimePerFrame;

		char const * encoderPreset = videoFps < (25.1) ?
			primo::avblocks::Preset::Video::DVD::PAL_4x3_MP2 : primo::avblocks::Preset::Video::DVD::NTSC_4x3_PCM;

		primo::avblocks::MediaSocket * outSocket = primo::avblocks::Library::createMediaSocket(encoderPreset);

		ASSERT(outSocket);
		if (!outSocket)
			return false;

		outSocket->setStream(&m_muxedCB);

		// set video bitrate
		{
			primo::avblocks::MediaPinList *pPins = outSocket->pins();
			for(int k = 0; k < pPins->count(); k++)
			{
				primo::avblocks::MediaPin *pPin = pPins->at(k);
				if(pPin->streamInfo()->mediaType() == primo::codecs::MediaType::Video)
				{
					pPin->streamInfo()->setBitrate(videoBitrate);
				}
			}
		}

		m_ms.pMpeg2Enc->outputs()->add(outSocket);
		outSocket->release();
	}

	if(!m_ms.pMpeg2Enc->open())
	{
		const primo::error::ErrorInfo* error = m_ms.pMpeg2Enc->error();
		
		int code = error->code();
		int facility = error->facility();
		const char_t* hint = error->hint();

		ATLTRACE(_T("Cannot open AVBlocks Transcoder"));
		return false;
	}

	bool writeDvd = (m_cmdSimulate.GetCheck() == BST_UNCHECKED);
	if (writeDvd)
	{
		DriveArray* pDrives = GetSelectedDrives();
		if (!pDrives)
			return false;

		if (!StartDvdRecorder(pDrives))
		{
			MessageBox(L"Cannot start dvd recording");
			return false;
		}
	}
	m_muxedCB.pVideoRecorder = writeDvd ? m_ms.pVideoRecorder : 0;

	IVideoWindow* pVideoWindow = NULL;
	m_ms.pGraph->QueryInterface<IVideoWindow>(&pVideoWindow);
	if (pVideoWindow)
	{
		cleanup.videoWindow = pVideoWindow;

		pVideoWindow->put_Owner((OAHWND)m_preview.m_hWnd);
		pVideoWindow->put_WindowStyle(WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS);
		pVideoWindow->put_Visible(OATRUE);

		RECT r;
		m_preview.GetClientRect(&r);					
		pVideoWindow->SetWindowPosition(0,0,r.right,r.bottom);
	}

	hr = m_ms.pMediaControl->Pause();
	if (FAILED(hr))
	{
		return false;
	}

	Sleep(3000);
	hr = m_ms.pMediaControl->Run();
	if (FAILED(hr))
	{
		return false;
	}

	ResetStats();

	m_avgBitrate = videoBitrate + (48000 * 2 * 2 * 8); // video + audio (samples per sec * bytes per channel * channels * 8bits)
	recStartTime = GetTickCount();
	fpsStartTime = recStartTime;
	fpsProcessed = 0;
	SetTimer(m_updateStatsEvent,500,NULL);

	cleanup.invoke = false;

	return true;
}

bool CameraToDVDDlg::ConfigureTranscoderInput(WAVEFORMATEX *pWFX, VIDEOINFOHEADER* pvh)
{
	// audio
	{
		primo::avblocks::MediaSocket * inSocket = primo::avblocks::Library::createMediaSocket();
		ASSERT(inSocket);
		if (!inSocket)
			return false;

		primo::avblocks::MediaPin *pin = primo::avblocks::Library::createMediaPin();
		pin->setConnection(primo::avblocks::PinConnection::Auto);

		primo::codecs::AudioStreamInfo *streamInfo = primo::avblocks::Library::createAudioStreamInfo();

		streamInfo->setStreamType( primo::codecs::StreamType::LPCM );
		streamInfo->setSampleRate( pWFX->nSamplesPerSec );
		streamInfo->setChannels( pWFX->nChannels );
		streamInfo->setBitsPerSample( pWFX->wBitsPerSample );
		streamInfo->setBytesPerFrame( (pWFX->wBitsPerSample / 8) * pWFX->nChannels );
		
		if(streamInfo->bitsPerSample() <= 8)
		{
			streamInfo->setPcmFlags(primo::codecs::PcmFlags::Unsigned);
		}
		
		pin->setStreamInfo(streamInfo);
		streamInfo->release();

		inSocket->pins()->add(pin);
		inSocket->setStreamType(pin->streamInfo()->streamType());
		pin->release();

		m_ms.pMpeg2Enc->inputs()->add(inSocket);
		inSocket->release();
	}

	// video
	{
		primo::avblocks::MediaSocket * inSocket = primo::avblocks::Library::createMediaSocket();
		ASSERT(inSocket);
		if (!inSocket)
			return false;

		primo::avblocks::MediaPin *pin = primo::avblocks::Library::createMediaPin();
		pin->setConnection(primo::avblocks::PinConnection::Auto);

		primo::codecs::VideoStreamInfo *streamInfo = primo::avblocks::Library::createVideoStreamInfo();

		streamInfo->setFrameRate((double)10000000 / pvh->AvgTimePerFrame);
		streamInfo->setBitrate(0);

		streamInfo->setFrameWidth(pvh->bmiHeader.biWidth);
		streamInfo->setFrameHeight(abs(pvh->bmiHeader.biHeight));

		streamInfo->setDisplayRatioWidth(pvh->bmiHeader.biWidth);
		streamInfo->setDisplayRatioHeight(abs(pvh->bmiHeader.biHeight));
		
		if (m_ms.VideoType.subtype == MEDIASUBTYPE_MJPG)
		{
			streamInfo->setStreamType(primo::codecs::StreamType::MJPEG);
			streamInfo->setColorFormat(primo::codecs::ColorFormat::YUV422);
		}
		else
		{
			streamInfo->setStreamType(primo::codecs::StreamType::UncompressedVideo);
			streamInfo->setColorFormat(GetColorFormat(m_ms.VideoType.subtype));
		}

		// unsupported capture format
		if (streamInfo->colorFormat() == primo::codecs::ColorFormat::Unknown)
			return false;

		streamInfo->setDuration(0.0);
		streamInfo->setScanType(primo::codecs::ScanType::Progressive);

		switch (streamInfo->colorFormat())
		{
		case primo::codecs::ColorFormat::BGR32:
		case primo::codecs::ColorFormat::BGR24:
		case primo::codecs::ColorFormat::BGR444:
		case primo::codecs::ColorFormat::BGR555:
		case primo::codecs::ColorFormat::BGR565:
			streamInfo->setFrameBottomUp(pvh->bmiHeader.biHeight > 0);
			break;
		}

		pin->setStreamInfo(streamInfo);
		streamInfo->release();

		inSocket->pins()->add(pin);
		inSocket->setStreamType(pin->streamInfo()->streamType());
		pin->release();

		m_ms.pMpeg2Enc->inputs()->add(inSocket);
		inSocket->release();
	}

	return true;
}

void CameraToDVDDlg::StopRecording()
{
	KillTimer(m_updateStatsEvent);

	m_ms.Reset(false); // leave the input devices in the graph
	m_audioCB.Reset();
	m_videoCB.Reset();
	m_muxedCB.Reset();
}
void CameraToDVDDlg::OnTimer(UINT_PTR nIDEvent)
{
	if (nIDEvent == m_updateStatsEvent)
	{
		UpdateStats();
	}

	CDialog::OnTimer(nIDEvent);
}

BOOL CameraToDVDDlg::OnWndMsg(UINT message, WPARAM wParam, LPARAM lParam, LRESULT* pResult)
{
	if (message == WM_STOP_CAPTURE)
	{
		OnCmdRecord();

		int stopReason = (int)wParam;
		if (-1 == stopReason)
		{

			MessageBox(L"An error occurred while recording to the disc. The recording has been stopped.",
				L"DVDBuilder Error", MB_OK | MB_ICONERROR);
		}
		else if (-2 == stopReason)
		{
			MessageBox(L"The disc is full. The recording has been stopped.",
				L"Device Out of space", MB_OK | MB_ICONEXCLAMATION);
		}
		else if (stopReason >= 0)
		{
			MessageBox(L"An error occurred encoding captured data. The recording has been stopped.",
				L"AVBlocks",MB_OK | MB_ICONERROR);
		}
		else
		{
			MessageBox(L"An error occurred while recording. The recording has been stopped.",
				L"Unexpected Error", MB_OK | MB_ICONERROR);

		}

		*pResult = 0;
		return TRUE;
	}
	else if (message == WM_DEVICECHANGE && !m_bRecording && !m_bCmdRecordBusy)
	{
		switch (wParam)
		{
		case DBT_DEVICEARRIVAL:
		case DBT_DEVICEREMOVECOMPLETE:
			UpdateDrivesAsync();
		}
	}
	else if (message == WM_QUERY_DRIVES_COMPLETE)
	{
		UpdateDrivesUI((DriveArray*)wParam);
	}

	return CDialog::OnWndMsg(message, wParam, lParam, pResult);
}

void CameraToDVDDlg::OnCmdFinalize()
{
	DriveArray* pDrives = GetSelectedDrives();
	if (!pDrives)
		return;

	VideoRecorder* pRecorder = primo::dvdbuilder::Library::createVideoRecorder();
	ATLASSERT(pRecorder);

	bool bUpdateDrives = false;

	do
	{
		m_cmdQueryFinalize.EnableWindow(0);

		if (!pDrives->Initialize())
		{
			MessageBox(L"Cannot access disc(s)", NULL, MB_OK | MB_ICONERROR);
			break;
		}

		__time64_t tnow;
		_time64(&tnow);
		tm* now = _localtime64(&tnow);
		CString label;
		label.Format(L"%.4d%.2d%.2d_%.2d%.2d",
			now->tm_year+1900,now->tm_mon+1,now->tm_mday,now->tm_hour,now->tm_min);


		for (drives_t::iterator it = pDrives->Items.begin(); it != pDrives->Items.end(); ++it)
		{
			VRDevice* pDevice = (*it)->pDevice;
			pRecorder->devices()->add(pDevice);
			if (pDevice->type() == VRDeviceType::OpticalDisc)
			{
				OpticalDiscDeviceConfig* pConfig = (OpticalDiscDeviceConfig*)pDevice->config();
				pConfig->setVolumeLabel(label);
			}
		}

		if (!pRecorder->finalizeSupported())
		{

			MessageBox(L"Video cannot be finalized");
			break;
		}

		if (pRecorder->finalized())
		{
			MessageBox(L"Video already finalized");
			break;
		}

		if (IDYES != MessageBox(L"Do you want to finalize the video now?",
			0, MB_YESNO | MB_ICONQUESTION | MB_DEFBUTTON2))
			break;

		bool res = pRecorder->finalizeMedia();
		bUpdateDrives = true;

		if (res)
		{
			pDrives->NotifyChanges();

			if (pRecorder->finalized())
			{
				MessageBox(L"Video finalized successfully");
				break;
			}
		}
		else
		{
			ATLTRACE("FinalizeMedia failed");
			CheckRecorderDevices(pRecorder);
		}

		MessageBox(L"An error occurred while finalizing video.", NULL, MB_OK | MB_ICONERROR);

	} while(true);

	delete pDrives;
	pRecorder->release();
	if (bUpdateDrives)
		UpdateDrivesAsync();
	m_cmdQueryFinalize.EnableWindow(1);
}

bool CameraToDVDDlg::StartDvdRecorder(DriveArray* pDrives)
{
	if (!pDrives)
		return false;

	struct CLEANUP
	{
		DriveArray* pDrives;
		VideoRecorder* pRecorder;
		bool success;

		CLEANUP() : pDrives(0), pRecorder(0), success(false)
		{}

		~CLEANUP()
		{
			if (success) 
				pDrives->DetachDevices();
			else if (pRecorder) 
				pRecorder->release();

			delete pDrives;
		}
	} cleanup;
	
	cleanup.pDrives = pDrives;
	

	if (!pDrives->Initialize())
		return false;

	VideoRecorder* pRecorder = primo::dvdbuilder::Library::createVideoRecorder();
	ATLASSERT(pRecorder);
	
	cleanup.pRecorder = pRecorder;

	for (drives_t::iterator it = pDrives->Items.begin(); it != pDrives->Items.end(); ++it)
	{
		pRecorder->devices()->add((*it)->pDevice);
	}

	if (pRecorder->mediaFreeSpace() < MIN_FREE_SPACE)
	{
		MessageBox(L"Not enough space on the disc(s).");
		return false;
	}

	if (pRecorder->finalized())
	{
		MessageBox(L"Disc(s) already finalized and cannot be written to.");
		return false;
	}

	if (pRecorder->startAsync())
	{
		m_ms.pVideoRecorder = pRecorder;
		cleanup.success = true;
	}

	return cleanup.success;
}


void CameraToDVDDlg::CloseApp()
{
	m_bExiting = true;
	if (m_bRecording)
	{
		OnCmdRecord();
		Sleep(300);
	}

	EndDialog(IDCANCEL);
}
void CameraToDVDDlg::OnCmdPreview()
{
	if (m_bRecording && m_ms.pPreviewRenderer)
	{
		IVideoWindow* pVideoWindow = NULL;
		m_ms.pGraph->QueryInterface<IVideoWindow>(&pVideoWindow);
		if (pVideoWindow)
		{
			long visible = m_cmdPreview.GetCheck() == BST_CHECKED ? OATRUE : OAFALSE;
			pVideoWindow->put_Visible(visible);
			pVideoWindow->Release();
		}
	}
}

void CameraToDVDDlg::OnCmdEraseDisc()
{
	DriveArray* pDrives = GetSelectedDrives();
	if (!pDrives)
		return;

	m_cmdEraseDisc.EnableWindow(0);
	bool bUpdateDrives = false;

	while(1)
	{
		if (!pDrives->Initialize())
		{
			MessageBox(L"Cannot access disc(s)", NULL, MB_OK | MB_ICONERROR);
			break;
		}

		int erasableCount= pDrives->GetErasableCount();

		if (erasableCount > 0)
		{

			if (IDYES != MessageBox(L"Erase disc(s)?",L"Confirmation", MB_YESNO | MB_ICONQUESTION | MB_DEFBUTTON2))
				break;

			bool res = pDrives->Erase();
			bUpdateDrives = true;

			if (res)
			{
				MessageBox(L"Disc(s) erased successfully");
			}
			else
			{
				MessageBox(L"Could not erase disc(s)", NULL, MB_OK | MB_ICONERROR);
			}
		}

		break;
	}

	delete pDrives;
	if (bUpdateDrives)
		UpdateDrivesAsync();
	m_cmdEraseDisc.EnableWindow(1);
}

DriveArray* CameraToDVDDlg::GetSelectedDrives()
{
	int	selCount = m_listDrives.GetSelCount();
	if (selCount > 0 && !m_pDvdPlugin)
	{
		MessageBox(L"DVD plugin not loaded");
		return NULL;
	}
	bool bDVD = selCount > 0;
	
	CStringW folder;
	m_editFolder.GetWindowTextW(folder);

	if (!folder.IsEmpty())
	{
		if (!PathIsDirectory(folder))
		{
			MessageBox(L"Invalid folder");
			return NULL;
		}

		if (!m_pFsPlugin)
		{
			MessageBox(L"File system plugin not loaded");
			return NULL;
		}
	}
	bool bFolder = !folder.IsEmpty();

	DriveArray* pDrives = NULL;

	// DVD Drives
	if (bDVD)
	{
		vector<int> selItems(selCount);
		selCount = m_listDrives.GetSelItems(selCount, &selItems[0]);

		if (!pDrives) pDrives = new DriveArray;
		for (int i=0; i < selCount; i++)
		{
			char letter = (char)m_listDrives.GetItemData(selItems[i]);

			VRDevice* pDevice = m_pDvdPlugin->createOpticalDiscDevice(letter, TRUE);
			if(pDevice)
			{
				pDrives->Items.push_back(new DriveItem(pDevice));
			}
			// the device is not initialized, so it may not be accessible!
		}
	}
	
	// Folder
	if (bFolder)
	{
		if (!pDrives) pDrives = new DriveArray;

		VRDevice* pDevice = m_pFsPlugin->createFileSystemDevice(folder);
		ATLASSERT(pDevice);

		pDrives->Items.push_back(new DriveItem(pDevice));
		// the device is not initialized, so it may not be accessible!
	}

	return pDrives;
}

// Get all optical drives in the system; put them in a drive array for a parallel work
DriveArray* CameraToDVDDlg::GetAllDrives()
{
	if (!m_pDvdPlugin)
	{
		return NULL;
	}

	DriveArray* pDrives = new DriveArray;

	const int size = 2048;
	WCHAR drives[size];
	memset(drives, 0, size); 
	GetLogicalDriveStrings(size - sizeof(WCHAR),drives);
	WCHAR* p = drives;
	while (*p)
	{
		if (DRIVE_CDROM == GetDriveType(p))
		{
			char letter = (char)p[0];
			VRDevice* pDevice = m_pDvdPlugin->createOpticalDiscDevice(letter, FALSE);
			if(pDevice)
			{
				pDrives->Items.push_back(new DriveItem(pDevice));
			}
		}
		p += 4;
	}

	if (pDrives->Items.size() == 0)
	{
		delete pDrives;
		pDrives = NULL;
	}

	return pDrives;
}


int CALLBACK BrowseFolderCB(HWND hwnd,UINT msg,LPARAM lp, LPARAM folder)
{
	switch(msg)
	{
	// select the last directory when the browse folder dialog opens
	case BFFM_INITIALIZED:
		if (folder)
		{
			SendMessage(hwnd, BFFM_SETSELECTION, TRUE, folder);
		}
		break;                                      
	}

	return 0;
}

void CameraToDVDDlg::OnCmdBrowseFolder()
{
	BROWSEINFO bi = { 0 };
    bi.lpszTitle = L"Select video output folder";
	WCHAR folder[MAX_PATH];
	int folderLen = m_editFolder.GetWindowText(folder,MAX_PATH-1);
	bi.lParam = folderLen == 0 ? NULL : (LPARAM)folder;
	bi.ulFlags = BIF_USENEWUI | BIF_RETURNONLYFSDIRS;
	bi.lpfn = BrowseFolderCB;
	
    LPITEMIDLIST pidl = SHBrowseForFolder(&bi);
    if (pidl)
    {
        // get the name of the folder
        WCHAR path[MAX_PATH];
        if (SHGetPathFromIDList(pidl, path))
        {
			m_editFolder.SetWindowTextW(path);
        }

        // free memory used
        IMalloc* pMalloc = 0;
        if ( SUCCEEDED(SHGetMalloc(&pMalloc)) )
        {
            pMalloc->Free(pidl);
            pMalloc->Release();
        }
    }
}

void CameraToDVDDlg::EnableDeviceUI(BOOL enable)
{
	m_listDrives.EnableWindow(enable);
	m_editFolder.EnableWindow(enable);
	m_cmdBrowseFolder.EnableWindow(enable);
	m_cmdClearFolder.EnableWindow(enable);
}

void CameraToDVDDlg::EnableCommandUI(BOOL enable)
{
	m_cmdQueryFinalize.EnableWindow(enable);
	m_cmdEraseDisc.EnableWindow(enable);
	m_cmdVideoInfo.EnableWindow(enable);
	m_cmdSimulate.EnableWindow(enable);
}

void CameraToDVDDlg::OnCmdSimulate()
{
	BOOL simulate = m_cmdSimulate.GetCheck()	== BST_CHECKED;
	EnableDeviceUI(!simulate);
}

void CameraToDVDDlg::OnCmdClearFolder()
{
	m_editFolder.SetWindowTextW(L"");
}

void CameraToDVDDlg::OnCmdVideoInfo()
{
	DriveArray* pDrives = GetSelectedDrives();
	if (!pDrives)
		return;

	VideoRecorder* pRecorder = primo::dvdbuilder::Library::createVideoRecorder();
	ATLASSERT(pRecorder);

	do
	{
		m_cmdVideoInfo.EnableWindow(0);

		if (!pDrives->Initialize())
		{
			MessageBox(L"Cannot access disc(s)", NULL, MB_OK | MB_ICONERROR);
			break;
		}

		CString info;

		for (drives_t::const_iterator it = pDrives->Items.begin(); it != pDrives->Items.end(); ++it)
		{
			VRDevice* pDevice = (*it)->pDevice;
			
			pRecorder->devices()->add(pDevice);

			if (pDevice->type() == VRDeviceType::OpticalDisc)
			{
				OpticalDiscDeviceConfig* pConfig = (OpticalDiscDeviceConfig*)pDevice->config();
				info.AppendFormat(L"\r\nDVD Drive %C:, Volume Label:%s\r\n", 
					pConfig->driveLetter(), pConfig->volumeLabel());
			}
			else if (pDevice->type() == VRDeviceType::FileSystem)
			{
				FileSystemDeviceConfig* pConfig = (FileSystemDeviceConfig*)pDevice->config();
				info.AppendFormat(L"\r\nDVD Video, Folder: %s\r\n", pConfig->folder());
			}

			TitleEnum* pTitles = pRecorder->titles(0);
			if (pTitles)
			{
				CTimeSpan total(0);
				int titleCount = pTitles->count();
				for (int i=0; i < titleCount; ++i)
				{
					Title* pTitle = pTitles->item(i);
					double sec = pTitle->duration();
					CTimeSpan dur((int)sec);
					info.AppendFormat(L"Title %.2d, %.2f sec, %dh %.2dm %.2ds\r\n",
						i+1,sec, dur.GetHours(), dur.GetMinutes(), dur.GetSeconds());
					
					total += dur;
				}

				pTitles->release();

				info.AppendFormat(L"Total: %dh %.2dm %.2ds\r\n", total.GetHours(), total.GetMinutes(), total.GetSeconds());
			}
			else
			{
				info.Append(L"Cannot read titles\r\n");
			}

			pRecorder->devices()->clear();
		}

		info.Append(L"\r\n");

		CVideoInfoDlg vi;
		vi.Info = info;
		vi.DoModal();

		break;

	} while(1);

	delete pDrives;
	pRecorder->release();
	m_cmdVideoInfo.EnableWindow(1);
}

void QueryDrivesProc(void* args)
{
	DriveArray* pDrives = (DriveArray*)args;
	
	pDrives->Initialize();

	pDrives->Query();
	
	//Notify main thread to update the UI
	PostMessage(g_mainWindow,WM_QUERY_DRIVES_COMPLETE, (WPARAM)args,0);

	_endthread();
}

void CameraToDVDDlg::UpdateDrivesUI(DriveArray* pDrives)
{
	m_listDrives.ResetContent();

	for (drives_t::const_iterator it = pDrives->Items.begin(); it != pDrives->Items.end(); ++it)
	{
		DriveItem* pDrive = (*it);
		VRDevice* pDevice = pDrive->pDevice;

		CString drive;

		OpticalDiscDeviceConfig* pConfig = (OpticalDiscDeviceConfig*)pDevice->config();
		char letter = pConfig->driveLetter();
		drive.AppendFormat(L"%C: ",letter);

		if (pDrive->IsInitialized)
		{
			// successfully initialized. device attributes
			if (pDrive->IsBlank)
			{
				drive.Append(L"Blank");
			}
			else
			{
				if (!pDrive->VolumeLabel.empty())
					drive.AppendFormat(L"%s",pDrive->VolumeLabel.c_str());
				else
					drive.Append(L"No Label");

				if (pDrive->IsVideo)
				{
					drive.Append(L"  (Video)");
				}
			}
		}
		
		m_listDrives.AddString(drive);
		m_listDrives.SetItemData(m_listDrives.GetCount()-1,(DWORD_PTR)letter);
		
		// restore drive selection
		if (find(m_selDrives.begin(), m_selDrives.end(), letter) != m_selDrives.end())
			m_listDrives.SetSel(m_listDrives.GetCount()-1,1);
	}

	m_listDrives.EnableWindow(1);
	delete pDrives;
}

void CameraToDVDDlg::UpdateDrivesAsync()
{
	DriveArray* pDrives = GetAllDrives();
	if (!pDrives)
		return;

	// save drive selection
	int	selCount = m_listDrives.GetSelCount();
	vector<int> selItems(selCount);
	if (selCount > 0)
	{
		selCount = m_listDrives.GetSelItems(selCount, &selItems[0]);
		m_selDrives.resize(selCount);
		for (int i = 0; i < selCount; i++)
		{
			 char letter = (char)m_listDrives.GetItemData(selItems[i]);
			 m_selDrives[i] = letter;
		}
	}

	// start updating drive information
	m_listDrives.ResetContent();
	m_listDrives.AddString(L"Updating...");
	m_listDrives.EnableWindow(0);

	_beginthread(QueryDrivesProc,0,pDrives);
}
