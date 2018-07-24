using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace i1Sharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application startup function.
        /// Creates the application model, starts searching for instruments and shows the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup (object sender, StartupEventArgs e)
        {
            I1SharpModel model = new I1SharpModel ();

            MainWindow mainWindow = new MainWindow (model);

            model.SearchDevices ();
            mainWindow.Show ();
        }
    }
}
