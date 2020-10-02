using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using DirectShowLib;
using PrimoSoftware.AVBlocks;

namespace CameraToDVD
{
    class SampleGrabberCB
    {
        protected long lastMediaTime = -1;
        protected double lastSampleTime = 0;
        protected DateTime startTime = DateTime.MinValue;

        protected string name;
        protected string filename;
        protected FileStream file;
        protected MediaState mediaState;
        protected bool bProcess = true;
        
        // how many times the callback has been called
        protected long sampleIndex;

        // how many samples are processed (based on the media time)
        protected long sampleProcessed;

        // how many samples are dropped (based on the media time)
        protected long sampleDropped;

        protected int streamNumber;
        protected IntPtr mainWindow;

        protected enum StreamType
        {
            Audio = 0,
            Video = 1
        };

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            ++sampleIndex;
            string msg = string.Format("BufferCB ({0}) {1}. time:{2} length:{3}",
                name, sampleIndex, SampleTime, BufferLen);

            System.Diagnostics.Trace.WriteLine(msg);

            if (file != null)
            {
                byte[] buf = new byte[BufferLen];
                Marshal.Copy(pBuffer, buf, 0, BufferLen);
                file.Write(buf, 0, BufferLen);
            }

            return 0;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            if (!bProcess)
            {
                lastSampleTime = SampleTime;
                return WinAPI.E_FAIL;
            }

            // internal stats
            ++sampleIndex;
            long tStart, tEnd;

            pSample.GetMediaTime(out tStart, out tEnd);
            Debug.Assert(tStart < tEnd);
            Debug.Assert(tStart > lastMediaTime);
            sampleProcessed += tEnd - tStart;
            sampleDropped += tStart - lastMediaTime - 1;
            lastMediaTime = tEnd - 1;

            int dataLen = pSample.GetActualDataLength();
            IntPtr bufPtr;
            int hr = pSample.GetPointer(out bufPtr);
            Debug.Assert(0 == hr);

            // BEGIN TRACE
            
            int bufSize = pSample.GetSize();

            long timeStart, timeEnd;
            pSample.GetTime(out timeStart, out timeEnd);

            string msg = string.Format(
                "SampleCB ({0}) {1}, sampleTime:{2} datalen:{3} bufsize:{4} mediaTime:{5}-{6} time:{7}-{8}",
                name, sampleIndex, SampleTime, dataLen, bufSize, tStart, tEnd, timeStart, timeEnd);

            Trace.WriteLine(msg);

            if (tStart - lastMediaTime - 1 > 0)
            {
                msg = string.Format("!!! Frame drop: {0}", tStart - lastMediaTime - 1 > 0);
                Trace.WriteLine(msg);
            }
              
            //END TRACE

            byte[] buf = new byte[dataLen];
            Marshal.Copy(bufPtr, buf, 0, dataLen);

            if (file != null)
            {
                file.Write(buf, 0, dataLen);
            }

            //DBG - simulate encoding error
            //if (sampleIndex > 100)
            //    goto STOP_CAPTURE;

            if (mediaState != null && mediaState.mpeg2Enc != null)
            {
                PrimoSoftware.AVBlocks.Transcoder enc = mediaState.mpeg2Enc;
                MediaSample inputSample = new MediaSample();
                inputSample.Buffer = new MediaBuffer(buf);
                inputSample.StartTime = Math.Max(SampleTime, 0);
                //TODO: end time

                try
                {
                    bool pushed = false;

                    // transcoder.Push() is not threads safe.
                    // lock (enc){ } ensure that only one thread is calling transcoder.Push()
                    lock (enc)
                    {
                        pushed = enc.Push(StreamNumber, inputSample);
                    }

                    if (pushed)
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.ToString());
                }

                Trace.WriteLine("PushSample FAILED");

            }
            
        //STOP_CAPTURE:

            Trace.WriteLine("SampleCB: Before Post STOP_CAPTURE");
            WinAPI.PostMessage(MainWindow, Util.WM_STOP_CAPTURE, new IntPtr(streamNumber), IntPtr.Zero);
            Trace.WriteLine("SampleCB: After Post STOP_CAPTURE");
            bProcess = false;
            return WinAPI.E_FAIL;

        } // end of SampleCB

        public void Start(DateTime time)
        {
            startTime = time;
        }

        public IntPtr MainWindow
        {
            get { return mainWindow; }
            set { mainWindow = value; }
        }

        public long SampleIndex
        {
            get { return sampleIndex; }
        }

        public int StreamNumber
        {
            get { return streamNumber; }
        }

        public long ProcessedSamples
        {
            get { return sampleProcessed; }
        }

        public long DroppedSamples
        {
            get { return sampleDropped; }
        }

        public SampleGrabberCB(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "SampleGrabberCB";
            }
        }

        ~SampleGrabberCB()
        {
            Reset();
        }

        public bool SetOutputFile(string filename)
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }

            if (string.IsNullOrEmpty(filename))
            {
                this.filename = null;
                return true;
            }

            try
            {
                file = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                return false;
            }

            this.filename = filename;
            return true;
        }

        public MediaState MediaState
        {
            get { return mediaState; }
            set { mediaState = value; }
        }

        public void Reset()
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }

            sampleIndex = 0;
            sampleProcessed = 0;
            sampleDropped = 0;

            mediaState = null;
            bProcess = true;

            lastMediaTime = -1;
            lastSampleTime = 0;
        }



    } // end of SampleGrabberCB

    class AudioGrabberCB : SampleGrabberCB, ISampleGrabberCB
    {
        public AudioGrabberCB(string name) : base(name)
        {
            streamNumber = (int)StreamType.Audio;
        }

        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            return base.BufferCB(SampleTime, pBuffer, BufferLen);
        }

        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            int hr =  base.SampleCB(SampleTime, pSample);
            Marshal.ReleaseComObject(pSample);
            return hr;
        }

    }

    class VideoGrabberCB : SampleGrabberCB, ISampleGrabberCB
    {
        public VideoGrabberCB(string name) : base(name)
        {
            streamNumber = (int)StreamType.Video;
        }
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            
            base.BufferCB(SampleTime, pBuffer, BufferLen);
            

            // own processing
            if (mediaState == null)
                return 0;

            AMMediaType mt = mediaState.videoType;

            // NOTE: not all subtypes can by dumped to bmp file

            if ((mt.majorType != DirectShowLib.MediaType.Video) ||
                (mt.formatType != DirectShowLib.FormatType.VideoInfo) ||
                (mt.formatPtr == IntPtr.Zero))
            {
                System.Diagnostics.Trace.WriteLine("Invalid media type");
                return 0;
            }

            if ((sampleIndex % 10) != 0)
            	return 0;

            string sampleFilename = string.Format(
                "d:\\tmp\\capture\\{0}_{1:d5}.bmp", this.name, sampleIndex);


            FileStream fs = null;
            try
            {
                fs = new FileStream(sampleFilename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                return WinAPI.E_FAIL;
            }

            BinaryWriter bw = new BinaryWriter(fs);
            const int bmFileHeaderSize = 14;
            VideoInfoHeader vih = (VideoInfoHeader)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader));

            System.Diagnostics.Trace.WriteLine(String.Format( "BmiHeader: {0}x{1}", vih.BmiHeader.Width, vih.BmiHeader.Height));

            bw.Write((byte)'B'); // type
            bw.Write((byte)'M');
            int bmSize = bmFileHeaderSize + vih.BmiHeader.Size + BufferLen;
            bw.Write(bmSize); // size
            bw.Write((short)0); // reserved
            bw.Write((short)0); // reserved
            bw.Write((int)(bmSize - BufferLen)); // offset bits

            int vihSize = Marshal.SizeOf(vih);
            byte[] bihBuf = new byte[vih.BmiHeader.Size];
            IntPtr bihStart = new IntPtr(mt.formatPtr.ToInt64() + vihSize - vih.BmiHeader.Size);
            Marshal.Copy(bihStart, bihBuf, 0, vih.BmiHeader.Size);
            bw.Write(bihBuf);

            byte[] bmpData = new byte[BufferLen];
            Marshal.Copy(pBuffer, bmpData, 0, BufferLen);
            bw.Write(bmpData);
            //bw.Flush();
            bw.Close();

            return 0;
        }

        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            int hr = base.SampleCB(SampleTime, pSample);
            Marshal.ReleaseComObject(pSample);
            return hr;
        }

    } // end of VideoGrabberCB

    // Sample grabber callback method
    enum CBMethod
    {
        Sample = 0, // the original sample from the upstream filter
        Buffer = 1  // a copy of the sample of the upstream filter
    };

}