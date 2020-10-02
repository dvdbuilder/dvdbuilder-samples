using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using PrimoSoftware.DVDBuilder.VR;
using System.Diagnostics;
using PrimoSoftware.AVBlocks;

namespace CameraToDVD
{
   

    static class Util
    {
        public const int WM_STOP_CAPTURE = WinAPI.WM_APP + 1;

        struct ColorSpaceEntry
        {
            public Guid videoSubType;
            public ColorFormat colorFormat;

            public ColorSpaceEntry(Guid videoSubType, ColorFormat colorFormat)
            {
                this.videoSubType = videoSubType;
                this.colorFormat = colorFormat;
            }
        };

        private static ColorSpaceEntry[] ColorSpaceTab =
        {
            new ColorSpaceEntry(MediaSubType.RGB24,	ColorFormat.BGR24),
            new ColorSpaceEntry(MediaSubType.ARGB32,ColorFormat.BGR32), // with alpha
            new ColorSpaceEntry(MediaSubType.RGB32,	ColorFormat.BGR32), // with alpha
            new ColorSpaceEntry(MediaSubType.RGB565,ColorFormat.BGR565),
            new ColorSpaceEntry(MediaSubType.ARGB1555,ColorFormat.BGR555),// with alpha
            new ColorSpaceEntry(MediaSubType.RGB555,ColorFormat.BGR555),// with alpha
            new ColorSpaceEntry(MediaSubType.ARGB4444,ColorFormat.BGR444),// with alpha

            // tested mappings
            new ColorSpaceEntry(MediaSubType.YV12, ColorFormat.YV12),
            new ColorSpaceEntry(MediaSubType.I420, ColorFormat.YUV420),
            new ColorSpaceEntry(MediaSubType.IYUV, ColorFormat.YUV420),
            new ColorSpaceEntry(MediaSubType.YUY2, ColorFormat.YUY2),

            // possible mappings
            new ColorSpaceEntry(MediaSubType.NV12, ColorFormat.NV12),
            new ColorSpaceEntry(MediaSubType.UYVY, ColorFormat.UYVY),
            new ColorSpaceEntry(MediaSubType.Y411, ColorFormat.Y411),
            new ColorSpaceEntry(MediaSubType.Y41P, ColorFormat.Y41P),
            new ColorSpaceEntry(MediaSubType.YVU9, ColorFormat.YVU9),

            // unknown mappings
		//{ ?,		ColorFormat::YUV411		},
		//{ ?,		ColorFormat::YUV420A	},
		//{ ?,		ColorFormat::YUV422		},
		//{ ?,		ColorFormat::YUV422A	},
		//{ ?,		ColorFormat::YUV444		},
		//{ ?,		ColorFormat::YUV444A	},
		//{ ?,		ColorFormat::YUV_VC1	},

        };
        
        public static void ReleaseComObject<T>(ref T comObject)
        {
            if (comObject != null)
            {
                Marshal.ReleaseComObject(comObject);

                comObject = default(T);
            }
        }

        public static void DisposeObject<T>(ref T obj) where T:IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = default(T);
            }
        }

        public static int GetPin(IBaseFilter filter, PinDirection pinDir, string name, out IPin ppPin)
        {
            ppPin = null;

            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1] { null };

            try
            {
                int hr = filter.EnumPins(out enumPins);
                DsError.ThrowExceptionForHR(hr);

                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    PinInfo pi;
                    hr = pins[0].QueryPinInfo(out pi);

                    bool found = false;
                    if (hr == 0 && pi.dir == pinDir && pi.name == name)
                    {
                        found = true;
                        
                        ppPin = pins[0];
                        
                        DsUtils.FreePinInfo(pi);
                    }

                    if (found)
                        return 0;
                   
                    Util.ReleaseComObject(ref pins[0]);
                }

                // Did not find a matching pin.
            }
            catch (COMException)
            {
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }

            return WinAPI.E_FAIL;
        }

        public static int GetUnconnectedPin(IBaseFilter filter, PinDirection pinDir, out IPin ppPin)
        {
            ppPin = null;

            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1] { null };
            IPin tmpPin = null;

            try
            {
                int hr = filter.EnumPins(out enumPins);
                DsError.ThrowExceptionForHR(hr);

                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    PinDirection thisPinDir;
                    hr = pins[0].QueryDirection(out thisPinDir);

                    if (hr == 0 && thisPinDir == pinDir)
                    {
                        hr = pins[0].ConnectedTo(out tmpPin);
                        if (tmpPin != null)  // Already connected, not the pin we want.
                        {
                            Util.ReleaseComObject(ref tmpPin);
                        }
                        else  // Unconnected, this is the pin we want.
                        {
                            ppPin = pins[0];
                            return 0;
                        }
                    }

                    Util.ReleaseComObject(ref pins[0]);
                }

                // Did not find a matching pin.
            }
            catch (COMException)
            {
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }

            return WinAPI.E_FAIL;
        }


        public static int ConnectFilters(IGraphBuilder graph, IBaseFilter src, IBaseFilter dest)
        {
            if ((graph == null) || (src == null) || (dest == null))
            {
                return WinAPI.E_FAIL;
            }

            // Find an output pin on the upstream filter.
            IPin pinOut = null;
            IPin pinIn = null;

            try
            {
                int hr = GetUnconnectedPin(src, PinDirection.Output, out pinOut);
                DsError.ThrowExceptionForHR(hr);

                // Find an input pin on the downstream filter.

                hr = GetUnconnectedPin(dest, PinDirection.Input, out pinIn);
                DsError.ThrowExceptionForHR(hr);

                // Try to connect them.
                hr = graph.ConnectDirect(pinOut, pinIn, null);
                DsError.ThrowExceptionForHR(hr);

                return 0;
            }
            catch (COMException)
            {
            }
            finally
            {
                Util.ReleaseComObject(ref pinIn);
                Util.ReleaseComObject(ref pinOut);
            }

            return WinAPI.E_FAIL;
        }

        public static ColorFormat GetColorFormat(ref Guid videoSubtype)
        {
            for (int i = 0; i < ColorSpaceTab.Length; i++)
            {
                if (ColorSpaceTab[i].videoSubType == videoSubtype)
                    return ColorSpaceTab[i].colorFormat;
            }

            return ColorFormat.Unknown;
        }

        // Tear down everything downstream of a given filter
        public static int NukeDownstream(IFilterGraph graph, IBaseFilter filter)
        {
            if (filter == null)
                return WinAPI.E_FAIL;

            IEnumPins enumPins = null;
            IPin[] pins = new IPin[1] { null };

            try
            {
                int hr = filter.EnumPins(out enumPins);
                DsError.ThrowExceptionForHR(hr);
                enumPins.Reset(); // start at the first pin

                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    if (pins[0] != null)
                    {
                        PinDirection pindir;
                        pins[0].QueryDirection(out pindir);
                        if (pindir == PinDirection.Output)
                        {
                            IPin pTo = null;
                            pins[0].ConnectedTo(out pTo);
                            if (pTo != null)
                            {
                                PinInfo pi;
                                hr = pTo.QueryPinInfo(out pi);

                                if (hr == 0)
                                {
                                    NukeDownstream(graph, pi.filter);

                                    graph.Disconnect(pTo);
                                    graph.Disconnect(pins[0]);
                                    graph.RemoveFilter(pi.filter);

                                    Util.ReleaseComObject(ref pi.filter);
                                    DsUtils.FreePinInfo(pi);
                                }
                                Marshal.ReleaseComObject(pTo);
                            }
                        }
                        Marshal.ReleaseComObject(pins[0]);
                    }
                }

                return 0;
            }
            catch (COMException)
            {
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }

            return WinAPI.E_FAIL;

        }

        public static bool CheckRecorderDevices(VideoRecorder recorder, out int activeCount, out int failedCount, out int noSpaceCount)
        {
            activeCount = 0;
            failedCount = 0;
            noSpaceCount = 0;

            for (int i = 0; i < recorder.Devices.Count; i++)
            {
                PrimoSoftware.DVDBuilder.ErrorInfo err = recorder.GetDeviceError(i);
                if (err.Facility != PrimoSoftware.DVDBuilder.ErrorFacility.Success)
                {
                    if (err.Facility == PrimoSoftware.DVDBuilder.ErrorFacility.VRDevice &&
                        err.Code == (int)VRDeviceError.OutOfFreeSpace)
                    {
                        ++noSpaceCount;
                    }
                    else
                    {
                        ++failedCount;
                    }

                    Trace.WriteLine(string.Format("Device({0}) error:{1} facility:{2}", i, err.Code, err.Facility));
                }
                else
                {
                    ++activeCount;
                }
            }

            Trace.WriteLine(string.Format("Devices active:{0} failed:{1} out-of-space:{2}", activeCount, failedCount, noSpaceCount));

            return activeCount > 0;
        }

        public static bool CheckRecorderDevices(VideoRecorder recorder)
        {
            int activeCount;
            int failedCount;
            int noSpaceCount;

            return CheckRecorderDevices(recorder, out activeCount, out failedCount, out noSpaceCount);
        }
    }
}
