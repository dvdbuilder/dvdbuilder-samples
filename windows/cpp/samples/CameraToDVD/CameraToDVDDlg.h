
// CameraToDVDDlg.h : header file
//

#pragma once
#include "afxwin.h"
#include "MediaState.h"
#include "SampleCallbacks.h"
#include "MuxedStream.h"
#include "DriveArray.h"

typedef std::deque<std::wstring> StringList;


// CameraToDVDDlg dialog
class CameraToDVDDlg : public CDialog
{
// Construction
public:
	CameraToDVDDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	enum { IDD = IDD_CAMERATODVD };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support


// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	CComboBox m_listAudioDev;
	CButton m_cmdAudioDevProp;
	CComboBox m_listVideoDev;
	CButton m_cmdVideoCaptureProp;
	CButton m_cmdSimulate;
	CButton m_cmdQueryFinalize;
	CButton m_cmdRecord;
	afx_msg void OnCmdExit();
	void OnOK();
	void OnCancel();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

public:
	void ResetStats();

protected:
	void EnumInputDev(const IID & devClass, CComboBox& list, StringList& devices);
	HRESULT InitInputDev(MediaState& ms, int videoItem, int audioItem);
	bool CheckInputDevice(IBaseFilter* pInputDevice);
	void ShowPropPages(IUnknown* pUnk);
	bool StartRecording();
	void StopRecording();
	void UpdateStats();
	bool StartDvdRecorder(DriveArray* pDrives);
	void CloseApp();
	DriveArray* GetSelectedDrives();
	DriveArray* GetAllDrives();
	void EnableDeviceUI(BOOL enable);
	void EnableCommandUI(BOOL enable);

	void UpdateDrivesAsync();
	void UpdateDrivesUI(DriveArray* pDrives);

	StringList m_audioDevices; // device monikors
	StringList m_videoDevices; // device monikors
	bool m_bRecording;
	bool m_bCmdRecordBusy;
	bool m_bExiting;
	vector<char> m_selDrives;
	MediaState m_ms;
	const UINT_PTR m_updateStatsEvent;

	VideoGrabberCB m_videoCB;
    AudioGrabberCB m_audioCB;
	MuxedStreamCB m_muxedCB;
	primo::dvdbuilder::VR::VRDevicePlugin* m_pDvdPlugin;
	primo::dvdbuilder::VR::VRDevicePlugin* m_pFsPlugin;

	static const uint64_t MIN_FREE_SPACE = 3 * 1385000; // bytes, 3 sec video at 1x dvd speed (max bitrate)
	DWORD recStartTime; // tick count
    DWORD fpsStartTime; // current fps
    DWORD fpsProcessed; // current fps
	uint32_t m_avgBitrate; // bits per second

	bool ConfigureTranscoderInput(WAVEFORMATEX *pWFX, VIDEOINFOHEADER* pvh);

public:
	afx_msg void OnCbnSelchangeListAudioDev();
	afx_msg void OnCbnSelchangeListVideoDev();
	afx_msg void OnCmdAudioDevProp();
	afx_msg void OnCmdVideoDevProp();
	afx_msg void OnCmdVideoCaptureProp();
	afx_msg void OnCmdRecord();
	CStatic m_txtRecording;
	CStatic m_preview;
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg BOOL OnWndMsg(UINT message, WPARAM wParam, LPARAM lParam, LRESULT* pResult);

	afx_msg void OnCmdFinalize();
	afx_msg void OnCmdPreview();
	CButton m_cmdPreview;
	CButton m_cmdEraseDisc;
	afx_msg void OnCmdEraseDisc();
	CListBox m_listDrives;
	CEdit m_editFolder;
	CButton m_cmdBrowseFolder;
	afx_msg void OnCmdBrowseFolder();
	CButton m_cmdClearFolder;
	afx_msg void OnCmdSimulate();
	afx_msg void OnCmdClearFolder();
	CButton m_cmdVideoInfo;
	afx_msg void OnCmdVideoInfo();
};
