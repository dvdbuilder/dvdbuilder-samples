Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices

Namespace CameraToDVD
    <Flags()> _
    Friend Enum ROTFlags
        RegistrationKeepsAlive = &H1
        AllowAnyClient = &H2
    End Enum

    Friend NotInheritable Class WinAPI
        Friend Const WM_APP As Integer = &H8000
        Friend Const WM_DEVICECHANGE As Integer = &H219
        Friend Const DBT_DEVICEARRIVAL As Integer = &H8000
        Friend Const DBT_DEVICEREMOVECOMPLETE As Integer = &H8004
        'INSTANT VB TODO TASK: There is no VB equivalent to 'unchecked' in this context:
        'ORIGINAL LINE: internal const int E_FAIL = unchecked((int)&H80004005);
        Friend Const E_FAIL As Integer = -2147467259 '0x80004005L

        <DllImport("oleaut32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True, PreserveSig:=True)> _
        Friend Shared Function OleCreatePropertyFrame(ByVal hwndOwner As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal lpszCaption As String, ByVal cObjects As Integer, <System.Runtime.InteropServices.In(), MarshalAs(UnmanagedType.Interface)> ByRef ppUnk As Object, ByVal cPages As Integer, ByVal pPageClsID As IntPtr, ByVal lcid As Integer, ByVal dwReserved As Integer, ByVal pvReserved As IntPtr) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)> _
        Friend Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

    End Class
End Namespace
