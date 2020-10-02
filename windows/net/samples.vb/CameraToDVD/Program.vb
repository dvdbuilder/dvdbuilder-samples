Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Namespace CameraToDVD
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



            PrimoSoftware.DVDBuilder.Library.Initialize()
            PrimoSoftware.AVBlocks.Library.Initialize()

            ' Replace the values with your name, company and license key for DVDBuilder.NET
            PrimoSoftware.DVDBuilder.Library.SetLicense("PRIMOSOFTWARE-LICENSE")

            ' Replace the values with your name, company and license key for AVBlocks.NET
            PrimoSoftware.AVBlocks.Library.SetLicense("PRIMOSOFTWARE-LICENSE")

            Application.Run(New RecorderForm())

            PrimoSoftware.DVDBuilder.Library.Shutdown()
            PrimoSoftware.AVBlocks.Library.Shutdown()
        End Sub
    End Class
End Namespace
