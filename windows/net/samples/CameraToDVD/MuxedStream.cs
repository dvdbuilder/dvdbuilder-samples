using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PrimoSoftware.DVDBuilder.VR;

namespace CameraToDVD
{

    internal class MuxedStreamCB: System.IO.Stream
    {
        #region System.IO.Stream
        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }

        public override long Length { get { throw new NotImplementedException(); } }
        public override long Position { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ((offset == 0) && (buffer.Length == count))
            {
                WriteData(buffer);
            }
            else
            {
                byte[] b = new byte[count];
                Array.Copy(buffer, offset, b, 0, count);
                WriteData(b);
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
#endregion

        public MuxedStreamCB()
        {

        }


        public bool WriteData(byte[] buffer)
        {
            if (!bProcess)
                return false;

            ++writeCounter;
            if (writeCounter % 100 == 0)
            {
                string msg = string.Format("MuxedStreamCB::WriteData {0}", writeCounter);
                System.Diagnostics.Trace.WriteLine(msg);
            }

            bool fileResult = true;
            if (file != null)
            {
                try
                {
                    file.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    fileResult = false;
                }
            }

            bool dvdResult = true;
            int stopReason = -1; // -1:general failure -2:out of space
            if (DvdRecorder != null && !DvdRecorder.Write(buffer))
            {
                System.Diagnostics.Trace.WriteLine("VideoRecorder.Write() FAILED");

                int activeCount, failedCount, noSpaceCount;
                Util.CheckRecorderDevices(DvdRecorder, out activeCount, out failedCount, out noSpaceCount);

                System.Diagnostics.Debug.Assert((activeCount + failedCount + noSpaceCount) == DvdRecorder.Devices.Count);

                if (activeCount == 0)
                {
                    dvdResult = false;
                    if (failedCount == 0 && noSpaceCount > 0)
                        stopReason = -2;
                }
            }

            if (fileResult && dvdResult)
                return true;

            //STOP_CAPTURE
            bProcess = false;
            System.Diagnostics.Trace.WriteLine("WriteData: Before Post STOP_CAPTURE");
            WinAPI.PostMessage(MainWindow, Util.WM_STOP_CAPTURE, new IntPtr(stopReason), IntPtr.Zero);
            System.Diagnostics.Trace.WriteLine("WriteData: After Post STOP_CAPTURE");

            return false;
        }

        public bool SetOutputFile(string filename)
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }

            if (filename == null)
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

        public void Reset()
        {
            if (file != null)
            {
                file.Close();
                file = null;
            }

            writeCounter = 0;
            DvdRecorder = null;
            bProcess = true;
        }

        public PrimoSoftware.DVDBuilder.VR.VideoRecorder DvdRecorder;

        private string filename;
        private System.IO.FileStream file;
        private int writeCounter;
        public IntPtr MainWindow;
        protected bool bProcess = true;
    }
}
