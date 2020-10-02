#include "StdAfx.h"
#include "DriveArray.h"

using namespace std;
using namespace primo::dvdbuilder::VR;

DriveArray::DriveArray(void)
{
}

DriveArray::~DriveArray(void)
{
	for (drives_t::iterator it = Items.begin(); it != Items.end(); ++it)
	{
		delete (*it);
	}
	Items.clear();
}

void DriveArray::DetachDevices()
{
	for (drives_t::iterator it = Items.begin(); it != Items.end(); ++it)
	{
		if((*it)->pDevice)
		{
			(*it)->pDevice->release();
			(*it)->pDevice = NULL;
		}
	}
}

unsigned __stdcall InitializeDevice(void* args)
{
	DriveItem* pArgs = (DriveItem*)args;
	pArgs->Result = pArgs->pDevice->initialize();
	pArgs->IsInitialized = pArgs->Result;
	_endthreadex( 0 );
	return 0;			
}

unsigned __stdcall EraseDevice(void* args)
{
	DriveItem* pArgs = (DriveItem*)args;
	pArgs->Result = pArgs->pDevice->eraseMedia();
	_endthreadex( 0 );
	return 0;			
}

unsigned __stdcall QueryDevice(void* args)
{
	DriveItem* pDrive = (DriveItem*)args;
	
	VRDevice* pDevice = pDrive->pDevice;

	if (pDrive->IsInitialized)
	{
		pDrive->IsBlank = pDevice->mediaIsBlank();
		pDrive->IsVideo = false;
		pDrive->VolumeLabel.clear();

		if (pDevice->type() == VRDeviceType::OpticalDisc)
		{
			OpticalDiscDeviceConfig* pConfig = (OpticalDiscDeviceConfig*)pDevice->config();
			const char_t* pLabel = pConfig->volumeLabel();
			if (pLabel && wcslen(pLabel) > 0)
				pDrive->VolumeLabel = pLabel;
		}
						
		VideoRecorder* pRecorder = Library::createVideoRecorder();
		ATLASSERT(pRecorder);

		pRecorder->devices()->add(pDevice);
		TitleEnum* pTitles = pRecorder->titles(0);
		if (pTitles)
		{
			if (pTitles->count() > 0)
				pDrive->IsVideo = true;

			pTitles->release();
		}

		pRecorder->release();
	}

	_endthreadex( 0 );
	return 0;			
}

unsigned __stdcall NotifyChanges(void* args)
{
	DriveItem* pArgs = (DriveItem*)args;
	pArgs->Result = pArgs->pDevice->notifyOSFileSystemChanged();
	_endthreadex( 0 );
	return 0;			
}

bool DriveArray::Initialize()
{
	return Parallel(::InitializeDevice);
}

bool DriveArray::Erase()
{
	return Parallel(::EraseDevice);
}
	
bool DriveArray::Query()
{
	return Parallel(::QueryDevice);
}

bool DriveArray::NotifyChanges()
{
	return Parallel(::NotifyChanges);
}


bool DriveArray::Parallel(DEV_FUNC func)
{
	if (Items.size() == 0)
		return false;

	vector<HANDLE> handles(Items.size());

	int i = 0;
	for (drives_t::iterator it = Items.begin(); it != Items.end(); ++it)
	{
		(*it)->Result = false;
		handles[i] = (HANDLE)_beginthreadex(NULL,0, func, *it, 0, NULL);
		++i;
	}

	WaitForMultipleObjects(i, &handles[0], TRUE, INFINITE);

	bool result = true;
	
	i = 0;
	for (drives_t::iterator it = Items.begin(); it != Items.end(); ++it)
	{
		if (!(*it)->Result)
		{
			result = false;
		}
		CloseHandle(handles[i]);
		++i;
	}
	
	return result;
}

bool DriveArray::IsErasable(VRDevice* pDevice)
{
	if (pDevice->mediaIsReWritable() && (pDevice->error()->facility() == ErrorFacility::Success))
	{
		if (!pDevice->mediaIsBlank() && (pDevice->error()->facility() == ErrorFacility::Success))
		{
			return true;
		}
	}
	return false;
}

int DriveArray::GetErasableCount()
{
	int erasable = 0;
	for (drives_t::const_iterator it = Items.begin(); it != Items.end(); ++it)
	{
		if (IsErasable((*it)->pDevice))
			erasable++;
	}
	return erasable;
}

