using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Metamorphosis;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;

namespace MetamorphosisDeskApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<FileInfo> DatabaseList {get; set;}
        public RelayCommand LoadDatabaseCommand { get; }
        public RelayCommand SummaryCommand { get; }

        Model.SQLDBUtilities SQLDB { get; set; }

        public string FilePath { get; set; }

        public Dictionary<string, int> CategoriesAndCount { get; set; }

        public string ViewTitle { get; internal set; }

        public ObservableCollection<Model.RevitElement> revitElements { get; set; }

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                ViewTitle = "Design Mode";
                FilePath = "Enter the database file path";
            }
            else
            {
                ViewTitle = "Metamorphosis Desktop App";
                FilePath = @"C:\Users\giovanni.brogiolo\Documents\190614_Arch.sdb";

                DatabaseList = new ObservableCollection<FileInfo>();

                SQLDB = new Model.SQLDBUtilities();

                revitElements = new ObservableCollection<Model.RevitElement>();

                LoadDatabaseCommand = new RelayCommand(() => LoadRevitElements());

                SummaryCommand = new RelayCommand(() => GroupByCategory());
            }

        }

        private void GroupByCategory()
        {
            CategoriesAndCount = SQLDB.GetCategoryCount(FilePath);
        }

        private void LoadRevitElements()
        {
            try
            {
                DirectoryInfo dinfo = new DirectoryInfo(FilePath);

                FileInfo[] files = dinfo.GetFiles("*.sdb");

                foreach (FileInfo file in files)
                {
                    DatabaseList.Add(file);
                }

            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
                    }


            //SQLDB.readElements(FilePath);

            //revitElements = SQLDB.observableRevitElements;

            //MessageBox.Show($"load {FilePath}");
        }
    }
}