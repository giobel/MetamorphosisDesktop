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
        public RelayCommand ClearCommand { get; }
        public RelayCommand FilterCategoryCommand { get; }
        public RelayCommand UndoFilterCommand { get; }
        public RelayCommand LoadAllCommand { get; }

        public FileInfo SelectedDatabase { get; set; }

        Model.SQLDBUtilities SQLDB { get; set; }

        public string FilePath { get; set; }

        public ObservableCollection<Model.IRevitBase> CategoriesAndCount { get; set; }
        public List<Model.IRevitBase> BackupCategoriesAndCount { get; set; }
        public Model.IRevitBase SelectedRevitElement { get; set; }

        public string ViewTitle { get; internal set; }

        public ObservableCollection<Model.IRevitBase> revitElements { get; set; }

        

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
                FilePath = @"C:\Users\giovanni.brogiolo\Documents";

                DatabaseList = new ObservableCollection<FileInfo>();

                SQLDB = new Model.SQLDBUtilities();

                CategoriesAndCount = new ObservableCollection<Model.IRevitBase>();

                

                revitElements = new ObservableCollection<Model.IRevitBase>();

                LoadDatabaseCommand = new RelayCommand(() => LoadRevitElements());

                SummaryCommand = new RelayCommand(() => GroupByCategory());

                LoadAllCommand = new RelayCommand(() => LoadAllRevitElements());

                ClearCommand = new RelayCommand(() => ClearDBList());

                FilterCategoryCommand = new RelayCommand(() => FilterCategory());

                UndoFilterCommand = new RelayCommand(() => UndoFilter());
            }

        }

        private void UndoFilter()
        {
            CategoriesAndCount.Clear();
            CategoriesAndCount = new ObservableCollection<Model.IRevitBase>(BackupCategoriesAndCount);

        }

        private void FilterCategory()
        {
            BackupCategoriesAndCount = new List<Model.IRevitBase>(CategoriesAndCount);
            
            try
            {
                for (int i = CategoriesAndCount.Count - 1; i >= 0; i--)
                {
                    if (CategoriesAndCount[i].CategoryName != SelectedRevitElement.CategoryName)
                    {
                        CategoriesAndCount.RemoveAt(i);
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LoadAllRevitElements()
        {
            try
            {
                List<Model.RevitElement> revitElements = SQLDB.readElementsFromDB(SelectedDatabase.FullName, SelectedDatabase.Name);
                foreach (var item in revitElements)
                {
                    var random = new Random();
                    var color = String.Format("#{0:X6}", random.Next(0x1000000));
                    item.ColorSet = color;
                    CategoriesAndCount.Add(item);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearDBList()
        {
            CategoriesAndCount.Clear();
        }

        private void GroupByCategory()
        {
            try
            {
                    List<Model.RevitSummary> revitElements = SQLDB.GetCategoryCount(SelectedDatabase.FullName, SelectedDatabase.Name);

                var random = new Random();
                var color = String.Format("#{0:X6}", random.Next(0x1000000));

                foreach (Model.RevitSummary item in revitElements)
                {
                    item.ColorSet = color;
                    CategoriesAndCount.Add(item);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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