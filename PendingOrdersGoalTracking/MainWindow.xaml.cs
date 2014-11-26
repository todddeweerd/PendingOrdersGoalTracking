using PendingOrdersGoalTracking.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PendingOrdersGoalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                MessageBox.Show("There was a problem initializing the application. " + ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel model = LoadMainViewModel();
                DataContext = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to load the sales data. {0}", ex.ToString()));
            }
        }

        private MainViewModel LoadMainViewModel()
        {
            MainViewModel model;

            try
            {
                model = MainViewModel.LoadSalesData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to load the sales data from the file {0}. {1}", Settings.Default.SalesFileName, ex.ToString()));
                model = new MainViewModel();
                // Test data
                //model.SalesGoal = 300;
                //model.DaysInMonth = 20;
                //model.DaysComplete = 13;
                //model.CurrentSales = 100;
                //model.PendingSales.Add(new PendingSale() { Customer = "Customer A", Amount = 20 });
                //model.PendingSales.Add(new PendingSale() { Customer = "Customer B", Amount = 2 });
                //model.PendingSales.Add(new PendingSale() { Customer = "Customer C", Amount = 7 });
            }
            // Default to 10
            while (model.PendingSales.Count < 10)
                model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
            return model;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel.SaveSalesData(DataContext as MainViewModel);
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("The save operation failed to save to the file {0}. {1}", Settings.Default.SalesFileName, ex.ToString()));
            }
        }

        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel model = LoadMainViewModel();
                DataContext = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The save operation failed to save to the file {0}. {1}", Settings.Default.SalesFileName, ex.ToString()));
            }
        }
    }
}
