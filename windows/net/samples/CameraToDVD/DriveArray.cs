using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using PrimoSoftware.DVDBuilder.VR;

namespace CameraToDVD
{
    class DriveItem
    {
        public VRDevice Device;
        public bool IsInitialized = false;
        public bool Result = false;

        // attributes
        public bool IsBlank = true;
        public bool IsVideo = false;
        public string VolumeLabel = string.Empty;

        public DriveItem(VRDevice device)
        {
            Debug.Assert(device != null);
            Device = device;
        }
    }

    class DriveArray
    {
        delegate void DeviceCaller(DriveItem drive);
        void InitializeDevice(DriveItem drive)
        {
            drive.Result = drive.Device.Initialize();
            drive.IsInitialized = drive.Result;
        }

        void EraseDevice(DriveItem drive)
        {
            drive.Result = drive.Device.EraseMedia();
        }

        void NotifyDeviceChanges(DriveItem drive)
        {
            drive.Result = drive.Device.NotifyOSFileSystemChanged();
        }

        void QueryDevice(DriveItem drive)
        {
            VRDevice device = drive.Device;

            if (drive.IsInitialized)
            {
                drive.IsBlank = device.MediaIsBlank;
                drive.IsVideo = false;
                drive.VolumeLabel = String.Empty;

                if (device.Type == VRDeviceType.OpticalDisc)
                {
                    OpticalDiscDeviceConfig config = (OpticalDiscDeviceConfig)device.Config;
                    if (config.VolumeLabel != null)
                        drive.VolumeLabel = config.VolumeLabel;
                    else
                        drive.VolumeLabel = String.Empty;
                }

                VideoRecorder recorder = new VideoRecorder();
                Debug.Assert(recorder != null);

                recorder.Devices.Add(device);
                IList<Title> titles = recorder.GetTitles(0);
                if (titles != null)
                {
                    if (titles.Count > 0)
                        drive.IsVideo = true;
                }

                recorder.Dispose();
            }
        }

        public List<DriveItem> Items = new List<DriveItem>(1);
        public void Dispose()
        {
            foreach (DriveItem drive in Items)
            {
                if (drive.Device != null)
                    drive.Device.Dispose();
            }
            Items.Clear();
        }

        public bool Initialize()
        {
            return Parallel(InitializeDevice);
        }

        public bool Erase()
        {
            return Parallel(EraseDevice);
        }

        public bool Query()
        {
            return Parallel(QueryDevice);
        }

        public bool NotifyChanges()
        {
            return Parallel(NotifyDeviceChanges);
        }

        private bool Parallel(DeviceCaller caller)
        {
            if (Items.Count == 0)
                return false;

            IAsyncResult[] asyncResults = new IAsyncResult[Items.Count];
            
            int i = 0;
            foreach (DriveItem drive in Items)
            {
                asyncResults[i] = caller.BeginInvoke(drive, null, null);
                i++;
            }

            bool result = true;
            foreach (IAsyncResult asyncResult in asyncResults)
            {
                caller.EndInvoke(asyncResult);
            }

            foreach (DriveItem drive in Items)
            {
                if (!drive.Result)
                {
                    result = false;
                }
            }

            return result;
        }

        private bool IsErasable(VRDevice device)
        {
            bool rw = device.MediaIsReWritable;
            if (rw && device.Error.Facility == PrimoSoftware.DVDBuilder.ErrorFacility.Success)
            {
                bool blank = device.MediaIsBlank;
                if (!blank && (device.Error.Facility == PrimoSoftware.DVDBuilder.ErrorFacility.Success))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetErasableCount()
        {
            int erasable = 0;

            foreach (DriveItem drive in Items)
            {
                if (IsErasable(drive.Device))
                    erasable++;
            }

            return erasable;
        }
    }
}
