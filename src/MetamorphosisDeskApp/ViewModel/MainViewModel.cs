using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Metamorphosis;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

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
        public RelayCommand ChangeColorsCommand { get; }

        public FileInfo SelectedDatabase { get; set; }

        Model.SQLDBUtilities SQLDB { get; set; }

        public string FilePath { get; set; }

        public ObservableCollection<Model.RevitBase> CategoriesAndCount { get; set; }
        public List<Model.RevitBase> BackupCategoriesAndCount { get; set; }
        public Model.RevitBase SelectedRevitElement { get; set; }
        public string ViewTitle { get; internal set; }
        public ObservableCollection<Model.RevitBase> revitElements { get; set; }

        

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

                CategoriesAndCount = new ObservableCollection<Model.RevitBase>();

                revitElements = new ObservableCollection<Model.RevitBase>();

                LoadDatabaseCommand = new RelayCommand(() => LoadRevitElements());

                SummaryCommand = new RelayCommand(() => Summary());

                LoadAllCommand = new RelayCommand(() => LoadAllRevitElementsCommand());

                ClearCommand = new RelayCommand(() => ClearDBList());

                FilterCategoryCommand = new RelayCommand(() => FilterCategory());

                UndoFilterCommand = new RelayCommand(() => UndoFilter());

                ChangeColorsCommand = new RelayCommand(() => ChangeColors());
            }

        }

        private void ChangeColors()
        {

            List<Model.RevitBase> currentList = CategoriesAndCount.ToList();

            CategoriesAndCount.Clear();

            //var random = new Random();
            //var color = String.Format("#{0:X6}", random.Next(0x1000000));

            var groupByColor = currentList.GroupBy(item => item.ColorSet).Select(group => group );

            string[] color = new string[]{ "#4287f5", "#0e9c44", "#adb837", "#b84837" };

            for (int i = 0; i < groupByColor.Count(); i++)
            {
                foreach (var per in groupByColor.ElementAt(i))
                {
                    per.ColorSet = color[i];
                    CategoriesAndCount.Add(per);
                }

            }
        }

        private void Summary()
        {
            GroupByCategory.Execute(SQLDB, SelectedDatabase).ForEach( cat => CategoriesAndCount.Add(cat));
            
        }

        private void UndoFilter()
        {
            CategoriesAndCount.Clear();
            CategoriesAndCount = new ObservableCollection<Model.RevitBase>(BackupCategoriesAndCount);

        }

        private void FilterCategory()
        {
            BackupCategoriesAndCount = new List<Model.RevitBase>(CategoriesAndCount);
            
            try
            {
                for (int i = CategoriesAndCount.Count - 1; i >= 0; i--)
                {
                    //Model.RevitElement rl = CategoriesAndCount[i] as Model.RevitElement; either do a casting or move the property to the base class

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

        private void LoadAllRevitElementsCommand()
        {
            LoadAllRevitElements.Execute(SQLDB, SelectedDatabase).ForEach(cat => CategoriesAndCount.Add(cat));
        }

        private void ClearDBList()
        {
            CategoriesAndCount.Clear();
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