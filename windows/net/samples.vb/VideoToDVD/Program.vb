Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace VideoToDVD
    Friend NotInheritable Class Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        Private Sub New()
        End Sub
        <STAThread()> _
        Shared Sub Main()
            ' Call EnableVisualStyles before initializing PrimoSoftware libraries
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)

            PrimoSoftware.AVBlocks.Library.Initialize()
            PrimoSoftware.DVDBuilder.Library.Initialize()
            PrimoSoftware.Burner.Library.Initialize()

            ' Replace with your DVDBuilder license
            PrimoSoftware.DVDBuilder.Library.SetLicense("PRIMOSOFTWARE-LICENSE")

            ' Replace with your AVBlocks license
            PrimoSoftware.AVBlocks.Library.SetLicense("PRIMOSOFTWARE-LICENSE")

            ' Replace with your PrimoBurner license
            PrimoSoftware.Burner.Library.SetLicense("PRIMOSOFTWARE-LICENSE")

            PrimoSoftware.Burner.Library.EnableTraceLog(Nothing, True)

            Application.Run(New MainForm())

            PrimoSoftware.Burner.Library.DisableTraceLog()

            PrimoSoftware.Burner.Library.Shutdown()
            PrimoSoftware.DVDBuilder.Library.Shutdown()
            PrimoSoftware.AVBlocks.Library.Shutdown()
        End Sub
    End Class
End Namespace
