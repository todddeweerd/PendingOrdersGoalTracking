using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace PendingOrdersGoalTracking
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
                MessageBox.Show(string.Format("Application error {0}", e.Exception));
            e.Handled = true;
        }
    }
}
