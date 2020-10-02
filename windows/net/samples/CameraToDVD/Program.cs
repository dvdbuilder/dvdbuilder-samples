using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CameraToDVD
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

            
            
            PrimoSoftware.DVDBuilder.Library.Initialize();
            PrimoSoftware.AVBlocks.Library.Initialize();

            // Replace the values with your name, company and license key for DVDBuilder.NET
            PrimoSoftware.DVDBuilder.Library.SetLicense("PRIMOSOFTWARE-LICENSE");

            // Replace the values with your name, company and license key for AVBlocks.NET
            PrimoSoftware.AVBlocks.Library.SetLicense("PRIMOSOFTWARE-LICENSE");

            Application.Run(new RecorderForm());

            PrimoSoftware.DVDBuilder.Library.Shutdown();
            PrimoSoftware.AVBlocks.Library.Shutdown();
        }
    }
}
