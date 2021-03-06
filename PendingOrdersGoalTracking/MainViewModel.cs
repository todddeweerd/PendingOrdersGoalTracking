﻿using PendingOrdersGoalTracking.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PendingOrdersGoalTracking
{
    [DataContract]
    [Serializable]
    class MainViewModel : INotifyPropertyChanged
    {
        DailyTimer _dayTimer;

        [DataMember(Name="PendingSales")]
        ObservableCollection<PendingSale> _pendingSales;

        public MainViewModel()
        {
            Initialize();
        }
        
        [OnDeserializing]  
        private void OnDeserializing(StreamingContext context)  
        {  
            Initialize();  
        }  
  
        [OnDeserialized]
        private void OnDeserializedAttribute(StreamingContext context)
        {
            foreach (PendingSale item in PendingSales)
                item.PropertyChanged += item_PropertyChanged;
        }

        private void Initialize()  
        {
            _pendingSales = new ObservableCollection<PendingSale>();
            _pendingSales.CollectionChanged += _pendingSales_CollectionChanged;
            _dayTimer = new DailyTimer();
            _dayTimer.NewDay += _dayTimer_NewDay;
            if (string.IsNullOrEmpty(QuoteText)|| QuoteText.Trim().Length == 0)
            {
                QuoteText = "\"The Best Way To Predict The Future Is To Create It.\"";
                QuoteReference = "- Peter Drucker";
            }

        }

        void _dayTimer_NewDay(DateTime Time)
        {
            OnPropertyChanged("CurrentMonth");
        }  

        void _pendingSales_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PendingSale item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Amount")
                NotifySalesChanged();
        }

        public BitmapFrame GoalIcon
        {
            get
            {
                BitmapFrame frame = BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Goal.png"));
                return frame;
                //IconBitmapDecoder ibd = new IconBitmapDecoder(new Uri("pack://application:,,,/Resources/Goal.ico")
                //    , BitmapCreateOptions.None
                //    , BitmapCacheOption.Default);
                //return ibd.Frames[0];
            }
        }

        public string CurrentMonth
        {
            get
            {
                return string.Format("  {0:MMMM yyyy}", DateTime.Today);
            }
        }

        [DataMember]
        public string QuoteText
        {
            get;
            set;
        }

        [DataMember]
        public string QuoteReference
        {
            get;
            set;
        }

        int _daysInMonth;
        [DataMember]
        public int DaysInMonth
        {
            get { return _daysInMonth; }
            set
            {
                _daysInMonth = value;
                OnPropertyChanged("DaysInMonth");
                OnPropertyChanged("DaysLeft");
                NotifySalesChanged();
            }
        }

        int _daysComplete;
        [DataMember]
        public int DaysComplete
        {
            get { return _daysComplete; }
            set
            {
                _daysComplete = value;
                OnPropertyChanged("DaysComplete");
                OnPropertyChanged("DaysLeft");
                NotifySalesChanged();
            }
        }

        public int DaysLeft
        {
            get { return DaysInMonth - DaysComplete; }
        }

        double _salesGoal;
        [DataMember]
        public double SalesGoal
        {
            get { return _salesGoal; }
            set
            {
                _salesGoal = value;
                NotifySalesChanged();
            }
        }

        double _currentSales;
        [DataMember]
        public double CurrentSales
        {
            get { return _currentSales; }
            set
            {
                _currentSales = value;
                NotifySalesChanged();
            }
        }

        public double ProjectedSales
        {
            get
            {
                if (DaysComplete * DaysInMonth == 0)
                    return 0;
                return CurrentSales / DaysComplete * DaysInMonth;
            }
        }

        public double ProjectedWithPending
        {
            get
            {
                if (DaysComplete * DaysInMonth == 0)
                    return 0;
                return (PendingSalesTotal + CurrentSales) / DaysComplete * DaysInMonth;
            }
        }

        public double ProjectedSalesPercent
        {
            get
            {
                if (SalesGoal == 0)
                    return 0;
                return ProjectedSales / SalesGoal * 100;
            }
        }

        public string ProjectedSalesPercentString
        {
            get
            {
                return string.Format("{0:0.0} %", ProjectedSalesPercent);
            }
        }

        public double CurrentSalesPercent
        {
            get
            {
                if (SalesGoal == 0)
                    return 0;
                return CurrentSales / SalesGoal * 100;
            }
        }

        public string CurrentSalesPercentString
        {
            get
            {
                return string.Format("{0:0.0} %", CurrentSalesPercent);
            }
        }

        public double ProjectedAndPendingSalesPercent
        {
            get
            {
                if (SalesGoal == 0)
                    return 0;
                return ProjectedWithPending / SalesGoal * 100;
            }
        }

        public string ProjectedAndPendingSalesPercentString
        {
            get
            {
                return string.Format("{0:0.0} %", ProjectedAndPendingSalesPercent);
            }
        }

        public double PendingSalesTotal
        {
            get
            {
                double total = 0;
                foreach (var pendingSale in _pendingSales)
                    total += pendingSale.Amount;
                return total;
            }
        }

        public ObservableCollection<PendingSale> PendingSales
        {
            get { return _pendingSales; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void NotifySalesChanged()
        {
            OnPropertyChanged("SalesGoal");
            OnPropertyChanged("CurrentSales");
            OnPropertyChanged("ProjectedSales");
            OnPropertyChanged("PendingSalesTotal");
            OnPropertyChanged("ProjectedWithPending");
            OnPropertyChanged("ProjectedSalesPercent");
            OnPropertyChanged("ProjectedSalesPercentString");
            OnPropertyChanged("ProjectedAndPendingSalesPercent");
            OnPropertyChanged("ProjectedAndPendingSalesPercentString");
            OnPropertyChanged("CurrentSalesPercent");
            OnPropertyChanged("CurrentSalesPercentString");
        }

        public static MainViewModel LoadSalesData()
        {
            MainViewModel model;
            DataContractSerializer serializer = new DataContractSerializer(typeof(MainViewModel));
            string fileName = GetSalesFileName();
            if (File.Exists(fileName))
            {
                using (FileStream fs = File.Open(fileName, FileMode.Open))
                {
                    model = serializer.ReadObject(fs) as MainViewModel;
                    if (model == null)
                        model = new MainViewModel();
                }
            }
            else
                model = new MainViewModel();
            return model;
        }

        public static void SaveSalesData(MainViewModel model)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(MainViewModel));
            using (FileStream fs = File.Open(GetSalesFileName(), FileMode.Create))
            {
                serializer.WriteObject(fs, model);
            }
        }

        private static string GetSalesFileName()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(appDataFolder, Settings.Default.SalesFileName);
        }
    }

    [DataContract]
    class PendingSale : INotifyPropertyChanged
    {
        string _customer;
        int _amount;

        [DataMember]
        public string Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }

        [DataMember]
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                OnPropertyChanged("Amount");
                OnPropertyChanged("FormattedAmount");
            }
        }

        public string FormattedAmount
        {
            get { return string.Format("{0:0;0; }", _amount); }
            set
            {
                Amount = Convert.ToInt32(value);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
