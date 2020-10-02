using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CameraToDVD
{
    [Flags]
    internal enum ROTFlags
    {
        RegistrationKeepsAlive = 0x1,
        AllowAnyClient = 0x2
    }

    static class WinAPI
    {
        internal const int WM_APP = 0x8000;
        internal const int WM_DEVICECHANGE = 0x0219;
        internal const int DBT_DEVICEARRIVAL = 0x8000;
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        internal const int E_FAIL = unchecked((int)0x80004005); // -2147467259 == 0x80004005L

         [DllImport("oleaut32.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig = true)]
		internal static extern int OleCreatePropertyFrame( IntPtr hwndOwner, int x, int y,
			string lpszCaption, int cObjects,
			[In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
			int cPages,	IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved );

         [return: MarshalAs(UnmanagedType.Bool)]
         [DllImport("user32.dll", SetLastError = true)]
         internal static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam,
            IntPtr lParam);

    }
}
