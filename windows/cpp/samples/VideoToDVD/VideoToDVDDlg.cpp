#include "stdafx.h"
#include "VideoToDVD.h"
#include "VideoToDVDDlg.h"
#include "DVDBuilderProject.h"
#include "DVDBuilderException.h"
#include "Mpeg2Encoder.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

void EncoderCallbackImpl::onProgress(double currentTime, double totalTime) 
{ 
	m_pDlg->EncoderOnProgress((int)currentTime, (int)totalTime); 
}

bool_t EncoderCallbackImpl::onContinue(double currentTime) 
{ 
	return m_pDlg->EncoderOnContinue(); 
}

void DVDBuilderCallbackImpl::onProgress(int32_t percentDone)
{ 
	m_pDlg->DVDBOnProgress(percentDone); 
}

void DVDBuilderCallbackImpl::onStatus(primo::dvdbuilder::DVDBuilderStatus::Enum status)
{ 
	m_pDlg->DVDBOnStatus(status); 
}

bool_t DVDBuilderCallbackImpl::onContinue() 
{ 
	return m_pDlg->DVDBOnContinue(); 
}

void BurnerCallbackImpl::OnStatus(const tstring& message) 
{ 
	m_pDlg->BurnerOnStatus(message); 
}

void BurnerCallbackImpl::OnImageProgress(DWORD64 ddwPos, DWORD64 ddwAll) 
{ 
	m_pDlg->BurnerOnImageProgress(ddwPos, ddwAll); 
}

void BurnerCallbackImpl::OnFileProgress(int file, const tstring& fileName, int percentCompleted) 
{ 
	m_pDlg->BurnerOnFileProgress(file, fileName, percentCompleted); 
}

void BurnerCallbackImpl::OnFormatProgress(double percentCompleted) 
{ 
	m_pDlg->BurnerOnFormatProgress(percentCompleted); 
}

void BurnerCallbackImpl::OnEraseProgress(double percentCompleted) 
{ 
	m_pDlg->BurnerOnEraseProgress(percentCompleted); 
}

bool BurnerCallbackImpl::OnContinue()
{ 
	return m_pDlg->BurnerOnContinue(); 
}


// VideoToDVDDlg dialog

VideoToDVDDlg::VideoToDVDDlg(CWnd* pParent /*=NULL*/)
	: CDialog(VideoToDVDDlg::IDD, pParent)
	, m_strDVDBProjectFolder(_T(""))
	, m_strVideoTsFolder(_T(""))
	, m_strVolumeName(_T("DVDVIDEO"))
	, m_AVEncoderCallbackImpl(this)
	, m_DVDBuilderCallbackImpl(this)
	, m_BurnerCallbackImpl(this)
{
	m_bCancellationPending = false;
	m_bWorking = false;

	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void VideoToDVDDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT_DVDB_PROJECT_FOLDER, m_strDVDBProjectFolder);
	DDX_Text(pDX, IDC_EDIT_VIDEO_TS_FOLDER, m_strVideoTsFolder);
	DDX_Text(pDX, IDC_EDIT_VOLUME_NAME, m_strVolumeName);
	DDX_Control(pDX, IDC_LIST_INPUT_FILES, m_lvInputFiles);
	DDX_Control(pDX, IDC_COMBO_DEVICES, m_cbDevices);
	DDX_Control(pDX, IDC_STATIC_MEDIA, m_staticMedia);
	DDX_Control(pDX, IDC_STATIC_FREE_SPACE, m_staticFreeSpace);
	DDX_Control(pDX, IDC_COMBO_TV_SYSTEM, m_cbTVSystem);
	DDX_Control(pDX, IDC_LIST_EVENT, m_lvLog);
	DDX_Control(pDX, IDC_PROGRESS_BAR, m_progressBar);
}

BEGIN_MESSAGE_MAP(VideoToDVDDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_DESTROY()
	ON_WM_QUERYDRAGICON()
	ON_MESSAGE( WM_DEVICECHANGE, OnDeviceChange)
	ON_MESSAGE(WM_UPDATE_PROGRESS, OnUpdateProgress)
	ON_MESSAGE(WM_WORKER_FINISHED, OnWorkerFinished)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BUTTON_ADD_FILES, &VideoToDVDDlg::OnBnClickedButtonAddFiles)
	ON_BN_CLICKED(IDC_BUTTON_CLEAR_FILES, &VideoToDVDDlg::OnBnClickedButtonClearFiles)
	ON_BN_CLICKED(IDC_BUTTON_CREATE_DVDB_PROJECT, &VideoToDVDDlg::OnBnClickedButtonCreateDvdbProject)
	ON_BN_CLICKED(IDC_BUTTON_CREATE_VIDEO_TS_FOLDER, &VideoToDVDDlg::OnBnClickedButtonCreateVideoTsFolder)
	ON_BN_CLICKED(IDC_BUTTON_CREATE_DISC_IMAGE, &VideoToDVDDlg::OnBnClickedButtonCreateDiscImage)
	ON_BN_CLICKED(IDC_BUTTON_CREATE_DVDVIDEO_DISC, &VideoToDVDDlg::OnBnClickedButtonCreateDvdvideoDisc)
	ON_BN_CLICKED(IDC_BUTTON_STOP, &VideoToDVDDlg::OnBnClickedButtonStop)
	ON_BN_CLICKED(IDC_BUTTON_BROWSE_DVDB_PROJECT_FOLDER, &VideoToDVDDlg::OnBnClickedButtonBrowseDvdbProjectFolder)
	ON_BN_CLICKED(IDC_BUTTON_BROWSE_VIDEO_TS_FOLDER, &VideoToDVDDlg::OnBnClickedButtonBrowseVideoTsFolder)
	ON_CBN_SELCHANGE(IDC_COMBO_DEVICES, &VideoToDVDDlg::OnCbnSelchangeComboDevices)
END_MESSAGE_MAP()


// VideoToDVDDlg message handlers

BOOL VideoToDVDDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	CRect rect;
	m_lvInputFiles.GetClientRect(&rect);
	int nColInterval = rect.Width() / 10;

	m_lvInputFiles.InsertColumn(0, _T("File"), LVCFMT_LEFT, nColInterval * 6);
	m_lvInputFiles.InsertColumn(1, _T("Info"), LVCFMT_LEFT, nColInterval * 3);

	m_lvLog.GetClientRect(&rect);
	nColInterval = rect.Width() / 10;

	m_lvLog.InsertColumn(0, _T("Time"), LVCFMT_LEFT, nColInterval * 2);
	m_lvLog.InsertColumn(1, _T("Event"), LVCFMT_LEFT, nColInterval * 7);

	m_progressBar.SetRange(0, 100);
	
	try
	{
		int nDeviceCount = 0;

		m_burner.set_Callback(&m_BurnerCallbackImpl);
		
		m_burner.Open();

		const DeviceVector& devices = m_burner.EnumerateDevices();
		for (size_t i = 0; i < devices.size(); i++) 
		{
			const DeviceInfo& dev = devices[i];
			if (dev.IsWriter) 
			{
				m_cbDevices.AddString(dev.Title.c_str());
				m_cbDevices.SetItemData(m_cbDevices.GetCount() - 1, i);
			}
		}

		if (0 == m_cbDevices.GetCount())
			throw BurnerException(NO_WRITER_DEVICES, NO_WRITER_DEVICES_TEXT);

		// Device combo
		m_cbDevices.SetCurSel(0);
		UpdateDeviceInformation();
		m_cbTVSystem.SetCurSel(0);
	}
	catch(BurnerException& bme)
	{
		ShowErrorMessage(bme.get_Message().c_str());

		EndModalLoop(-1);
		return FALSE;
	}

	UpdateUI();


	return TRUE;  // return TRUE  unless you set the focus to a control
}

void VideoToDVDDlg::OnOK()
{

}

void VideoToDVDDlg::OnCancel()
{
	if(m_bWorking)
	{
		AfxMessageBox(_T("An operation is in progress. Plese stop the program."));
	}
	else
	{
		CDialog::OnCancel();
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void VideoToDVDDlg::OnPaint()
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

void VideoToDVDDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	m_burner.Close();
}

LRESULT VideoToDVDDlg::OnDeviceChange(WPARAM wParam, LPARAM lParam) 
{
	UpdateDeviceInformation();
	return 0;
}

// The system calls this function to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR VideoToDVDDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

LRESULT VideoToDVDDlg::OnUpdateProgress(WPARAM wParam, LPARAM lParam)
{
	m_progressBar.SetPos(m_progressContext.Progress);

	if(m_progressContext.LogMessage.size() > 0)
	{
		char dateTime[128] = {0};
		const time_t now = time(NULL);
		strftime(dateTime, sizeof(dateTime), "%H:%M:%S", localtime(&now));

		CString strTime(dateTime);

		const int index = m_lvLog.GetItemCount();
		m_lvLog.InsertItem(index, strTime); 
		m_lvLog.SetItemText(index, 1, m_progressContext.LogMessage.c_str());
		m_lvLog.EnsureVisible(index, TRUE);

		m_progressContext.LogMessage.clear();
	}
	return 0;
}

LRESULT VideoToDVDDlg::OnWorkerFinished(WPARAM wParam, LPARAM lParam)
{
	m_bWorking = false;
	m_bCancellationPending = false;

	UpdateUI();
	UpdateDeviceInformation();
	return 0;
}

void VideoToDVDDlg::OnBnClickedButtonBrowseDvdbProjectFolder()
{
	UpdateData(TRUE);

	TCHAR szPath[MAX_PATH];
	memset(szPath, 0, sizeof(szPath));

	BROWSEINFO bi = {0};
	bi.hwndOwner = m_hWnd;
	bi.pidlRoot = NULL;
	bi.lpszTitle = _TEXT("Select DVDB project intermediate folder");
	bi.ulFlags = BIF_EDITBOX | BIF_NEWDIALOGSTYLE;
	bi.lpfn = NULL;
	bi.lParam = NULL;
	bi.pszDisplayName = szPath;

	LPITEMIDLIST pidl = ::SHBrowseForFolder(&bi);

	if (NULL == pidl)
		return;

	::SHGetPathFromIDList(pidl, szPath);
	CoTaskMemFree(pidl);
	pidl = NULL;

	m_strDVDBProjectFolder = szPath;
	UpdateData(FALSE);
}

void VideoToDVDDlg::OnBnClickedButtonBrowseVideoTsFolder()
{
	UpdateData(TRUE);

	TCHAR szPath[MAX_PATH];
	memset(szPath, 0, sizeof(szPath));

	BROWSEINFO bi = {0};
	bi.hwndOwner = m_hWnd;
	bi.pidlRoot = NULL;
	bi.lpszTitle = _TEXT("Select VIDEO_TS intermediate folder");
	bi.ulFlags = BIF_EDITBOX | BIF_NEWDIALOGSTYLE;
	bi.lpfn = NULL;
	bi.lParam = NULL;
	bi.pszDisplayName = szPath;

	LPITEMIDLIST pidl = ::SHBrowseForFolder(&bi);

	if (NULL == pidl)
		return;

	::SHGetPathFromIDList(pidl, szPath);
	CoTaskMemFree(pidl);
	pidl = NULL;

	m_strVideoTsFolder = szPath;
	UpdateData(FALSE);
}


void VideoToDVDDlg::OnBnClickedButtonAddFiles()
{
   TCHAR szFilters[]= _TEXT("Video files (*.mp4,*.mpg,*.mpeg,*.mod,*.avi,*.wmv,*.mts,*.m2t,*.ts,*.tod,*.m2v,*.m4v,*.webm,*.dat,*.mpe,*.mpeg4,*.ogm)|*.mp4;*.mpg;*.mpeg;*.mod;*.avi;*.wmv;*.mts;*.m2t;*.ts;*.tod;*.m2v;*.m4v;*.webm;*.dat;*.mpe;*.mpeg4;*.ogm|")
					  _TEXT("All files (*.*)|*.*||");

   CFileDialog dlg(TRUE, 0, 0, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY, szFilters, this);
   OPENFILENAME& ofn = dlg.m_ofn;
   ofn.lpstrTitle = _T("Open video file");
   ofn.Flags |= OFN_ALLOWMULTISELECT;
   const DWORD bufSize = 99 * (_MAX_PATH + 1) + 1;
   TCHAR buf[bufSize];
   ofn.lpstrFile = buf;
   ofn.nMaxFile = bufSize;
   buf[0] = 0;
   buf[bufSize - 1] = 0;

   if(dlg.DoModal() == IDOK)
   {
	   POSITION pos = dlg.GetStartPosition();

	   while( pos )
	   {
		   CString filePath(dlg.GetNextPathName(pos));

		   primo::avblocks::MediaInfo *pInfo = primo::avblocks::Library::createMediaInfo();

		   pInfo->setInputFile(filePath);

		   if(pInfo->load())
		   {
			   if(GetVideoStreamsCount(pInfo) == 1)
			   {
				   m_settings.InputFiles.push_back(filePath);
				   const int index = m_lvInputFiles.GetItemCount();
				   m_lvInputFiles.InsertItem(index, filePath); 
				   m_lvInputFiles.SetItemText(index, 1, FormatAVInfo(pInfo));
			   }
		   }

		   pInfo->release();
	   }
   }
}

int VideoToDVDDlg::GetVideoStreamsCount(primo::avblocks::MediaInfo *pInfo)
{
	primo::codecs::StreamInfoEnum* pStreams = pInfo->streams();
	int32_t streamsCount = pStreams->count();
	int videoStreamsCount = 0;;

	for (int i=0; i < streamsCount; ++i)
	{
		primo::codecs::StreamInfo* psi = pStreams->at(i);

		if (primo::codecs::MediaType::Video == psi->mediaType()) 
		{
			videoStreamsCount += 1;
		}
	}

	return videoStreamsCount;
}

CString VideoToDVDDlg::FormatAVInfo(primo::avblocks::MediaInfo *pInfo)
{
	primo::codecs::StreamInfoEnum* pStreams = pInfo->streams();
	int32_t streamsCount = pStreams->count();
	CString str;

	for (int i=0; i < streamsCount; ++i)
	{
		primo::codecs::StreamInfo* psi = pStreams->at(i);

		if (primo::codecs::MediaType::Video == psi->mediaType()) 
		{
			primo::codecs::VideoStreamInfo* vsi = static_cast<primo::codecs::VideoStreamInfo*>(psi);
			str.Format(_T("%dx%d %.2ffps %.1fsec."), vsi->frameWidth(), vsi->frameHeight(), vsi->frameRate(), vsi->duration());
			break;
		}
	}

	return str;
}

void VideoToDVDDlg::OnBnClickedButtonClearFiles()
{
	UpdateData(TRUE);
    m_settings.InputFiles.clear();
	m_lvInputFiles.DeleteAllItems();
	UpdateData(FALSE);
}

void VideoToDVDDlg::ShowErrorMessage(const CString &strError)
{
	::MessageBox(NULL, strError, _T("Error"), MB_OK | MB_ICONERROR);
}

void VideoToDVDDlg::UpdateDeviceInformation()
{
	int nCurSel = m_cbDevices.GetCurSel();
	if (-1 == nCurSel)
		return;

	const int deviceIndex = m_cbDevices.GetItemData(nCurSel);
	try
	{
		m_burner.SelectDevice(deviceIndex, false);
		DWORD64 freeSpace = (DWORD64)m_burner.get_MediaFreeSpace() * BlockSize::DVD;

		CString str;
		str.Format(_TEXT("%.2fGB"), ((double)(__int64)freeSpace) / 1e9);
		m_staticFreeSpace.SetWindowText(str);

		m_staticMedia.SetWindowText(m_burner.get_MediaProfileString().c_str());

		m_burner.ReleaseDevice();
	}
	catch(BurnerException& bme)
	{
		// Ignore the error when it is DEVICE_ALREADY_SELECTED
		if (DEVICE_ALREADY_SELECTED == bme.get_Error())
			return;
		
		// Report all other errors
		m_burner.ReleaseDevice();
		ShowErrorMessage(bme.get_Message().c_str());
	}
}

void VideoToDVDDlg::OnCbnSelchangeComboDevices()
{
	UpdateDeviceInformation();
}


void VideoToDVDDlg::OnBnClickedButtonCreateDvdbProject()
{
    ClearLogEvents();

    SetSettings();
    m_settings.Task = TaskCreateDVDProject;

    if (!ValidateTask())
        return;

    RunWorkerAsync();
    UpdateUI();
}

void VideoToDVDDlg::OnBnClickedButtonCreateVideoTsFolder()
{
    ClearLogEvents();

    SetSettings();
    m_settings.Task = TaskCreateVideoTs;

    if (!ValidateTask())
        return;

    RunWorkerAsync();
    UpdateUI();
}

void VideoToDVDDlg::OnBnClickedButtonCreateDiscImage()
{
    ClearLogEvents();

    SetSettings();
    m_settings.ImageFileName = _T("");
    m_settings.Task = TaskCreateDiscImage;

    if (!ValidateTask())
        return;

	CFileDialog dlg(FALSE, _TEXT("*.iso"), NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT | OFN_NONETWORKBUTTON,
								_TEXT("Image File (*.iso)|*.iso||"), NULL );
	if(IDOK != dlg.DoModal())
		return;

	m_settings.ImageFileName = dlg.m_ofn.lpstrFile;

    RunWorkerAsync();
    UpdateUI();
}

void VideoToDVDDlg::OnBnClickedButtonCreateDvdvideoDisc()
{
    ClearLogEvents();

    SetSettings();
    m_settings.Task = TaskBurnDisc;

    if (!ValidateTask())
        return;
    
    RunWorkerAsync();
    UpdateUI();
}

void VideoToDVDDlg::OnBnClickedButtonStop()
{
    CancleWorkerAsync();
    UpdateUI();
}

bool VideoToDVDDlg::ValidateTask()
{
    try
    {
        if (m_settings.InputFiles.size() == 0)
        {
            ShowErrorMessage(_T("No input files specified."));
            return false;
        }

        if (m_settings.Task == TaskBurnDisc)
        {
            if (!ValidateMedia())
                return false;
        }

        if ((m_settings.DvdPrjFolder.GetLength() == 0) || !DirectoryExists(m_settings.DvdPrjFolder))
        {
            ShowErrorMessage(_T("Please select DVDB project folder."));
            return false;
        }

        if ((m_settings.Task == TaskCreateVideoTs) ||
            (m_settings.Task == TaskCreateDiscImage) ||
            (m_settings.Task == TaskBurnDisc))
        {
            if ((m_settings.VideoTsFolder.GetLength() == 0) || !DirectoryExists(m_settings.VideoTsFolder))
            {
                ShowErrorMessage(_T("Please select VIDEO_TS project folder."));
                return false;
            }

            if (!IsDirectoryEmpty(m_settings.VideoTsFolder))
            {
				CString msg;
                msg.Format(_T("The folder  %s  is not empty. All files and folders within that folder will be deleted. Do you want to continue?"), m_settings.VideoTsFolder);
				if (::MessageBox(this->GetSafeHwnd(), msg, _T("Delete Files"), MB_YESNO) != IDYES)
                    return false;

                CleanDirectory(m_settings.VideoTsFolder, false);
            }
        }
    }
    catch (BaseException &ex)
    {
        ShowErrorMessage(ex.get_Message().c_str());
        return false;
    }

    return true;
}

BOOL VideoToDVDDlg::DirectoryExists(const CString &path)
{
	return (_taccess(path, 0) != -1);
}

bool VideoToDVDDlg::IsDirectoryEmpty(const CString &path)
{
	CString sSearchFor = path + _TEXT("/*") ;

	WIN32_FIND_DATA FindFileData;
	HANDLE hFind = FindFirstFile(sSearchFor, &FindFileData);

	if (hFind == INVALID_HANDLE_VALUE)
	{
		throw BaseException(_T("File not found."));
	}

	int filesCount = 0;
	int dirsCount = 0;

	do 
	{
		// Keep the original file name
		CString sFileName = FindFileData.cFileName; 
		
		if (FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			// Skip the curent folder . and the parent folder .. 
			if (sFileName == _TEXT(".")  || sFileName == _TEXT(".."))
				continue;

			dirsCount++;
		} 
		else
		{
			filesCount++;
		}
	} 
	while (FindNextFile (hFind, &FindFileData));

	FindClose(hFind);
	hFind = INVALID_HANDLE_VALUE;

	return (filesCount + dirsCount) == 0;
}

void VideoToDVDDlg::CleanDirectory(const CString &path, bool removeDir)
{ 
	CString sSearchFor = path + _TEXT("\\*") ;

	WIN32_FIND_DATA FindFileData;
	HANDLE hFind = FindFirstFile(sSearchFor, &FindFileData);

	if (hFind == INVALID_HANDLE_VALUE)
	{
		throw BaseException(_T("File not found."));
	}

	do 
	{
		// Keep the original file name
		CString sFileName = FindFileData.cFileName; 
		CString sFullPath = path + _T("\\") + sFileName;

		if (FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			// Skip the curent folder . and the parent folder .. 
			if (sFileName == _TEXT(".")  || sFileName == _TEXT(".."))
				continue;

			CleanDirectory(sFullPath, true);
		} 
		else
		{
			if(!DeleteFile(sFullPath))
			{
				throw BaseException(_T("File delete failed."));
			}
		}
	} 
	while (FindNextFile (hFind, &FindFileData));

	FindClose(hFind);
	hFind = INVALID_HANDLE_VALUE;

	if(removeDir)
	{
		if(!RemoveDirectory(path))
		{
			throw BaseException(_T("Directory delete failed."));
		}
	}
}

CString VideoToDVDDlg::RemoveFolderEndingSlash(CString str)
{
	str.Trim();

	if ((_T('\\') == str.Right(1)) || 
		(_T('/') == str.Right(1))) 
	{
		str = str.Left(str.GetLength() - 1);
	}

	return str;
}

void VideoToDVDDlg::SetSettings()
{
	UpdateData(TRUE);

	const int nCurSel = m_cbDevices.GetCurSel();
	if (-1 != nCurSel)
	{
		m_settings.DeviceIndex = m_cbDevices.GetItemData(nCurSel);
	}

    m_settings.VolumeName = m_strVolumeName;
    m_settings.VideoTsFolder = m_strVideoTsFolder;
    m_settings.DvdPrjFolder = m_strDVDBProjectFolder;

	m_settings.VideoTsFolder = RemoveFolderEndingSlash(m_settings.VideoTsFolder);
	m_settings.DvdPrjFolder = RemoveFolderEndingSlash(m_settings.DvdPrjFolder);
	
    if (m_cbTVSystem.GetCurSel() == 0)
    {
		m_settings.TVSystem = TVSystem::NTSC;
    }
    else
    {
		m_settings.TVSystem = TVSystem::PAL;
    }
}

bool VideoToDVDDlg::ValidateMedia()
{
	const int nCurSel = m_cbDevices.GetCurSel();
	if (-1 == nCurSel)
		return false;

    try
    {
		int deviceIndex = m_cbDevices.GetItemData(nCurSel);

        m_burner.SelectDevice(deviceIndex, true);
		MediaProfile::Enum mediaProfile = m_burner.get_MediaProfile();
		bool mediaIsBlank = m_burner.get_MediaIsBlank();
		m_burner.ReleaseDevice();

        switch (mediaProfile)
        {
		case MediaProfile::DVDPlusR:
		case MediaProfile::DVDPlusRDL:
		case MediaProfile::DVDMinusRSeq:
		case MediaProfile::DVDMinusRWSeq:
		case MediaProfile::DVDMinusRDLJump:
		case MediaProfile::DVDMinusRDLSeq:
                if (!mediaIsBlank)
                {
                    ShowErrorMessage(_T("The media is not blank. Please insert a blank DVD media."));
                    return false;
                }
                break;

		case MediaProfile::DVDPlusRW:
		case MediaProfile::DVDMinusRWRO:
		case MediaProfile::DVDRam:
            // random rewriteable, no MediaIsBlank validation
            break;

        default:
            ShowErrorMessage(_T("There is no DVD media into the drive. Please insert a DVD media (DVD-R, DVD+R, DVD-RW, DVD+RW, DVD-RAM)."));
            return false;
        }
    }
    catch (BaseException &ex)
    {
		m_burner.ReleaseDevice();
        ShowErrorMessage(ex.get_Message().c_str());
        return false;
    }

    return true;
}

void VideoToDVDDlg::UpdateUI()
{
	GetDlgItem(IDC_LIST_INPUT_FILES)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_ADD_FILES)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_CLEAR_FILES)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_EDIT_DVDB_PROJECT_FOLDER)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_EDIT_VIDEO_TS_FOLDER)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_BROWSE_DVDB_PROJECT_FOLDER)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_BROWSE_VIDEO_TS_FOLDER)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_COMBO_TV_SYSTEM)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_COMBO_DEVICES)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_EDIT_VOLUME_NAME)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_CREATE_DVDB_PROJECT)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_CREATE_VIDEO_TS_FOLDER)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_CREATE_DISC_IMAGE)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_CREATE_DVDVIDEO_DISC)->EnableWindow(!m_bWorking);
	GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(m_bWorking && !m_bCancellationPending);
}

void VideoToDVDDlg::BurnDisc(const CString &videoTsFolder)
{
    LogEvent(_T("Burn started."));

    m_burner.SelectDevice(m_settings.DeviceIndex, true);

    BurnSettings bs;
    bs.SourceFolder = videoTsFolder;
    bs.VolumeLabel = m_settings.VolumeName;

	bs.ImageType = ImageTypeFlags::UdfIso;
    bs.VideoDVD = true;
	bs.WriteMethod = WriteMethod::DVDIncremental;
    bs.WriteSpeedKb = m_burner.get_MaxWriteSpeedKB();

    bs.Simulate = false;
    bs.CloseDisc = true;
    bs.Eject = true;

    m_burner.Burn(bs);

    m_burner.ReleaseDevice();

    LogEvent(_T("Burn completed."));
}

void VideoToDVDDlg::CreateDiscImage(const CString &videoTsFolder)
{
    LogEvent(_T("Create image started."));

    CreateImageSettings cis;
    cis.SourceFolder = videoTsFolder;
    cis.VolumeLabel = m_settings.VolumeName;

    cis.ImageType = ImageTypeFlags::UdfIso;
    cis.VideoDVD = true;
    cis.ImageFile = m_settings.ImageFileName;

    m_burner.CreateImage(cis);

    LogEvent(_T("Create image completed."));
}

void VideoToDVDDlg::EncodeInputFiles(const CString &tmpFolder, std::vector<CString> &encodedFiles)
{
	// COM must be initialized in order to decode WMV files
	CoInitialize(NULL);

    LogEvent(_T("Encoding started."));

	encodedFiles.clear();
	
	Mpeg2Encoder encoder;
	encoder.setCallback(&m_AVEncoderCallbackImpl);
	
	encoder.setOutputPreset(m_settings.transcoderPreset());

	for(int i = 0; i < (int)m_settings.InputFiles.size(); i++)
	{
        CString inputFile = m_settings.InputFiles[i];

		TCHAR fileName[_MAX_FNAME + 1];
		memset(fileName, 0, sizeof(fileName));

		_tsplitpath(inputFile, NULL, NULL, fileName, NULL);

        CString outputFile = tmpFolder + _T("\\") + fileName + _T(".mpg");

		CFileStatus status;
		if (CFile::GetStatus(outputFile, status))
			CFile::Remove(outputFile);

		LogEvent(CString(_T("Encoding ")) + outputFile);

		encoder.setInputFile(inputFile);
		encoder.setOutputFile(outputFile);

		encoder.convert();

		encodedFiles.push_back(outputFile);
	}

    LogEvent(_T("Encoding completed."));
}

void VideoToDVDDlg::CreateVideoTs(const CString &projectFile, const CString &outputFolder)
{
    LogEvent(_T("DVD-Video building started."));

	primo::dvdbuilder::DVDBuilder *pDVDB = primo::dvdbuilder::Library::createDVDBuilder();
	pDVDB->setCallback(&m_DVDBuilderCallbackImpl);
	pDVDB->setOutputFolder(outputFolder);
	pDVDB->setProjectFile(projectFile);

	bool_t result = pDVDB->build();
	uint32_t error = pDVDB->error()->code();
	uint32_t errorFacility = pDVDB->error()->facility();
	tstring errorHint = pDVDB->error()->hint();
	pDVDB->release();

	if(!result)
	{
		throw DVDBuilderException(error, errorFacility, errorHint);
	}

    LogEvent(_T("DVD-Video building completed."));
}

CString VideoToDVDDlg::CreateDVDBProject(const std::vector<CString> &encodedFiles, const CString &encodedFilesTmpFolder)
{
	CString projectFile = encodedFilesTmpFolder + _T("\\") + _T("project.xml");
	
	DVDBuilderProject project;
	project.setTvSystem(m_settings.TVSystem);
	project.create(projectFile, encodedFiles, encodedFilesTmpFolder);
	
	return projectFile;
}

void VideoToDVDDlg::RunWorkerAsync()
{
	m_progressContext.Progress = 0;
	m_progressContext.LogMessage.clear();

	m_bCancellationPending = false;
	m_bWorking = true;

	DWORD dwThreadID = 0;
	CreateThread(NULL, NULL, DoWorkThreadProc, this, NULL, &dwThreadID);
}

void VideoToDVDDlg::CancleWorkerAsync()
{
	m_bCancellationPending = true;
}

DWORD VideoToDVDDlg::DoWorkThreadProc(LPVOID pVoid)
{
	VideoToDVDDlg* pDlg = ((VideoToDVDDlg*)pVoid);
	pDlg->DoWork();
	::PostMessage(pDlg->GetSafeHwnd(), WM_WORKER_FINISHED, 0, 0);
	return 0;
}

void VideoToDVDDlg::DoWork()
{
    try
    {
		std::vector<CString> encodedFiles;

		EncodeInputFiles(m_settings.DvdPrjFolder, encodedFiles);
        
        CString dvdbProjectFile = CreateDVDBProject(encodedFiles, m_settings.DvdPrjFolder);

        if (m_bCancellationPending)
            return;

        switch(m_settings.Task)
        {
            case TaskCreateVideoTs:
                CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);
                break;

            case TaskCreateDiscImage:
                CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);

                if (m_bCancellationPending)
                    return;

                CreateDiscImage(m_settings.VideoTsFolder);
                break;

            case TaskBurnDisc:
                CreateVideoTs(dvdbProjectFile, m_settings.VideoTsFolder);

                if (m_bCancellationPending)
                    return;

                BurnDisc(m_settings.VideoTsFolder);
                break;
        }
    }
    catch (BaseException &ex)
    {
        ShowErrorMessage(ex.get_Message().c_str());
        LogEvent(ex.get_Message().c_str());
    }
}

void VideoToDVDDlg::SetProgress(int percent)
{
	if(percent > 100)
		percent = 100;

	if(percent < 0)
		percent = 0;

	m_progressContext.Progress = percent;
	::SendMessage(this->GetSafeHwnd(), WM_UPDATE_PROGRESS, 0, 0);
}

void VideoToDVDDlg::LogEvent(const CString &message)
{
	m_progressContext.LogMessage = message;
	::SendMessage(this->GetSafeHwnd(), WM_UPDATE_PROGRESS, 0, 0);
}

void VideoToDVDDlg::ClearLogEvents()
{
	UpdateData(TRUE);
	m_lvLog.DeleteAllItems();
	UpdateData(FALSE);
}

void VideoToDVDDlg::EncoderOnProgress(int encodedTime, int totalTime) 
{ 
    if(totalTime > 0)
	{
        SetProgress((encodedTime * 100) / totalTime);
	}
};

bool_t VideoToDVDDlg::EncoderOnContinue() 
{ 
	return !m_bCancellationPending; 
};

void VideoToDVDDlg::DVDBOnProgress(int32_t percentDone)
{ 
	SetProgress(percentDone);
};

void VideoToDVDDlg::DVDBOnStatus(primo::dvdbuilder::DVDBuilderStatus::Enum status)
{ 
    switch (status)
    {
		case primo::dvdbuilder::DVDBuilderStatus::WritingVOB :
            LogEvent(_T("WritingVOB"));
            break;
        case primo::dvdbuilder::DVDBuilderStatus::WritingIFO:
            LogEvent(_T("WritingIFO"));
            break;
    }
};

bool VideoToDVDDlg::DVDBOnContinue()
{ 
	return !m_bCancellationPending;
};

void VideoToDVDDlg::BurnerOnStatus(const tstring& message) 
{ 
	LogEvent(message.c_str());
};

void VideoToDVDDlg::BurnerOnImageProgress(DWORD64 ddwPos, DWORD64 ddwAll) 
{ 
    if (ddwAll > 0)
	{
        SetProgress((int)((100 * ddwPos) / ddwAll));
	}
};

void VideoToDVDDlg::BurnerOnFileProgress(int file, const tstring& fileName, int percentCompleted) 
{ 
	// no progress report
};

void VideoToDVDDlg::BurnerOnFormatProgress(double percentCompleted) 
{ 
	SetProgress((int)percentCompleted);
};

void VideoToDVDDlg::BurnerOnEraseProgress(double percentCompleted)
{ 
	SetProgress((int)percentCompleted);
};

bool VideoToDVDDlg::BurnerOnContinue()
{ 
	return !m_bCancellationPending; 
}

