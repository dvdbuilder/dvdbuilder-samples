#pragma once
#include "Burner.h"
#include "afxcmn.h"
#include "afxwin.h"
#include "DVDBuilderProject.h"

class VideoToDVDDlg;

enum
{
	WM_UPDATE_PROGRESS  = (WM_APP + 1),
	WM_WORKER_FINISHED  = (WM_APP + 2)
};

enum eTask
{
    TaskCreateDVDProject,
    TaskCreateVideoTs,
    TaskCreateDiscImage,
    TaskBurnDisc
};

class VideoToDVDSettings
{
public:
    eTask Task;
    int DeviceIndex;
    CString ImageFileName;

    CString VolumeName;
	TVSystem::Enum TVSystem;
    CString DvdPrjFolder;
    CString VideoTsFolder;
	std::vector<CString> InputFiles;

	const char* transcoderPreset()
	{
		return (TVSystem == TVSystem::PAL) ? 
					primo::avblocks::Preset::Video::DVD::PAL_4x3_MP2 : primo::avblocks::Preset::Video::DVD::NTSC_4x3_PCM;

	}
};

class ProgressContext
{
public:
    tstring LogMessage;
    int Progress;
};

class EncoderCallbackImpl : public primo::avblocks::TranscoderCallback
{
	VideoToDVDDlg *m_pDlg;
public:
	EncoderCallbackImpl(VideoToDVDDlg *pDlg):m_pDlg(pDlg) {}
	void onProgress(double currentTime, double totalTime);
	bool_t onContinue(double currentTime); 
};

class DVDBuilderCallbackImpl: public primo::dvdbuilder::DVDBuilderCallback
{
	VideoToDVDDlg *m_pDlg;
public:
	DVDBuilderCallbackImpl(VideoToDVDDlg *pDlg):m_pDlg(pDlg) {}

	void onProgress(int32_t percentDone);
	void onStatus(primo::dvdbuilder::DVDBuilderStatus::Enum status);
	bool_t onContinue();
};
	
class BurnerCallbackImpl: public IBurnerCallback
{
	VideoToDVDDlg *m_pDlg;
public:
	BurnerCallbackImpl(VideoToDVDDlg *pDlg):m_pDlg(pDlg) {}

	void OnStatus(const tstring& message);
	void OnImageProgress(DWORD64 ddwPos, DWORD64 ddwAll);
	void OnFileProgress(int file, const tstring& fileName, int percentCompleted);
	void OnFormatProgress(double percentCompleted);
	void OnEraseProgress(double percentCompleted);
	bool OnContinue();
};

class VideoToDVDDlg: public CDialog
{
	 Burner m_burner;
	 VideoToDVDSettings m_settings;

	 CString FormatAVInfo(primo::avblocks::MediaInfo *pInfo);
	 int GetVideoStreamsCount(primo::avblocks::MediaInfo *pInfo);

	ProgressContext m_progressContext;

public:
	void EncoderOnProgress(int encodedTime, int totalTime);
	bool_t EncoderOnContinue();

	void DVDBOnProgress(int32_t percentDone);
	void DVDBOnStatus(primo::dvdbuilder::DVDBuilderStatus::Enum status);
	bool DVDBOnContinue();

	void BurnerOnStatus(const tstring& message);
	void BurnerOnImageProgress(DWORD64 ddwPos, DWORD64 ddwAll);
	void BurnerOnFileProgress(int file, const tstring& fileName, int percentCompleted);
	void BurnerOnFormatProgress(double percentCompleted);
	void BurnerOnEraseProgress(double percentCompleted);
	bool BurnerOnContinue();

protected:
	EncoderCallbackImpl m_AVEncoderCallbackImpl;
	DVDBuilderCallbackImpl m_DVDBuilderCallbackImpl;
	BurnerCallbackImpl m_BurnerCallbackImpl;

	void SetProgress(int percent);
	void LogEvent(const CString &message);
	void ClearLogEvents();

	void UpdateUI();

	bool m_bCancellationPending;
	bool m_bWorking;

// Construction
public:
	VideoToDVDDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_VIDEOTODVD_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg void OnDestroy();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg LRESULT OnDeviceChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnUpdateProgress(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnWorkerFinished(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()

	static DWORD WINAPI DoWorkThreadProc(LPVOID pVoid);
	void DoWork();

	void RunWorkerAsync();
	void CancleWorkerAsync();

	CString CreateDVDBProject(const std::vector<CString> &encodedFiles, const CString &encodedFilesTmpFolder);
	void CreateVideoTs(const CString &projectFile, const CString &outputFolder);
	void EncodeInputFiles(const CString &tmpFolder, std::vector<CString> &encodedFiles);
	void CreateDiscImage(const CString &videoTsFolder);
	void BurnDisc(const CString &videoTsFolder);
	bool ValidateMedia();
	void SetSettings();
	void CleanDirectory(const CString &path, bool removeDir);
	bool IsDirectoryEmpty(const CString &path);
	bool ValidateTask();
	void UpdateDeviceInformation();
	void ShowErrorMessage(const CString &strError);
	BOOL DirectoryExists(const CString &path);
	CString RemoveFolderEndingSlash(CString str);

public:
	afx_msg void OnBnClickedButtonAddFiles();
	afx_msg void OnBnClickedButtonClearFiles();
	afx_msg void OnBnClickedButtonCreateDvdbProject();
	afx_msg void OnBnClickedButtonCreateVideoTsFolder();
	afx_msg void OnBnClickedButtonCreateDiscImage();
	afx_msg void OnBnClickedButtonCreateDvdvideoDisc();
	afx_msg void OnBnClickedButtonStop();
	CString m_strDVDBProjectFolder;
	CString m_strVideoTsFolder;
	CString m_strVolumeName;
	afx_msg void OnBnClickedButtonBrowseDvdbProjectFolder();
	afx_msg void OnBnClickedButtonBrowseVideoTsFolder();
	CListCtrl m_lvInputFiles;
	CComboBox m_cbDevices;
	CStatic m_staticMedia;
	CStatic m_staticFreeSpace;
	afx_msg void OnCbnSelchangeComboDevices();
	CComboBox m_cbTVSystem;
	CListCtrl m_lvLog;
	CProgressCtrl m_progressBar;
};
