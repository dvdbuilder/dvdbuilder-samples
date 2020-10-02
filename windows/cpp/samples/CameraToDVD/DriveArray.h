#pragma once

using namespace std;
using namespace primo::dvdbuilder::VR;

class DriveItem
{
public:
	VRDevice* pDevice;
	bool IsInitialized;
	bool Result;

	// attributes
	bool IsBlank;
	bool IsVideo;
	wstring VolumeLabel;

	DriveItem(VRDevice* p) : pDevice(p), IsInitialized(false), IsBlank(true), IsVideo(false), Result(false)
	{
		ATLASSERT(p);
	}

	~DriveItem()
	{
		if (pDevice)
		{
			pDevice->release();
			pDevice = NULL;
		}
	}
};

typedef list<DriveItem*> drives_t;
typedef unsigned (__stdcall * DEV_FUNC)(void *);

class DriveArray
{
public:
	DriveArray(void);
	~DriveArray(void);

	drives_t Items;
    int GetErasableCount();
	void DetachDevices();

	//parallel operations
	bool Initialize();
	bool Query();
	bool Erase();
	bool NotifyChanges();

private:
	bool IsErasable(VRDevice* device);
	bool Parallel(DEV_FUNC func);

};
