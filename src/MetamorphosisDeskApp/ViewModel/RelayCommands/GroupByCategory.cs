using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace MetamorphosisDeskApp.ViewModel
{
    public static class GroupByCategory
    {
        public static List<Model.RevitBase> Execute(Model.SQLDBUtilities SQLDB, FileInfo SelectedDatabase)
        {
            List<Model.RevitBase> result = new List<Model.RevitBase>();
            try
            {
                List<Model.RevitCategories> revitElements = SQLDB.GetCategoryCount(SelectedDatabase.FullName, SelectedDatabase.Name);

                var random = new Random();
                var color = String.Format("#{0:X6}", random.Next(0x1000000));

                foreach (Model.RevitCategories item in revitElements)
                {
                    item.ColorSet = color;
                    result.Add(item);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }
    }
}
