﻿using PendingOrdersGoalTracking.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PendingOrdersGoalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel model;

                try
                {
                    if (File.Exists(Settings.Default.SalesFileName))
                        model = MainViewModel.LoadSalesData();
                    else
                    {
                        model = new MainViewModel();
                        // Test data
                        model.SalesGoal = 300;
                        model.DaysInMonth = 20;
                        model.DaysComplete = 13;
                        model.CurrentSales = 100;
                        model.PendingSales.Add(new PendingSale() { Customer = "Customer A", Amount = 20 });
                        model.PendingSales.Add(new PendingSale() { Customer = "Customer B", Amount = 2 });
                        model.PendingSales.Add(new PendingSale() { Customer = "Customer C", Amount = 7 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                        model.PendingSales.Add(new PendingSale() { Customer = "", Amount = 0 });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Unable to load the sales data from the file {0}. {1}", Settings.Default.SalesFileName, ex.ToString()));
                    model = new MainViewModel();
                }
                DataContext = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to load the sales data. {0}", ex.ToString()));
            }
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
                MainViewModel model = MainViewModel.LoadSalesData();
                DataContext = model;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("The save operation failed to save to the file {0}. {1}", Settings.Default.SalesFileName, ex.ToString()));
            }
        }
    }
}
