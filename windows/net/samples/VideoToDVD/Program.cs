using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VideoToDVD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Call EnableVisualStyles before initializing PrimoSoftware libraries
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            PrimoSoftware.AVBlocks.Library.Initialize();
            PrimoSoftware.DVDBuilder.Library.Initialize();
            PrimoSoftware.Burner.Library.Initialize();

            // Replace with your DVDBuilder license
            PrimoSoftware.DVDBuilder.Library.SetLicense("PRIMOSOFTWARE-LICENSE");

            // Replace with your AVBlocks license
            PrimoSoftware.AVBlocks.Library.SetLicense("PRIMOSOFTWARE-LICENSE");

            // Replace with your PrimoBurner license
            PrimoSoftware.Burner.Library.SetLicense("PRIMOSOFTWARE-LICENSE");

            PrimoSoftware.Burner.Library.EnableTraceLog(null, true);

            Application.Run(new MainForm());

            PrimoSoftware.Burner.Library.DisableTraceLog();

            PrimoSoftware.Burner.Library.Shutdown();
            PrimoSoftware.DVDBuilder.Library.Shutdown();
            PrimoSoftware.AVBlocks.Library.Shutdown();
        }
    }
}
