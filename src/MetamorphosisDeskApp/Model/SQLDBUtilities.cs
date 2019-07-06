using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetamorphosisDeskApp.Model
{
    public class SQLDBUtilities
    {

        public ObservableCollection<Model.RevitElement> observableRevitElements { get; set; }

        public SQLDBUtilities()
        {
            observableRevitElements = new ObservableCollection<RevitElement>();
            
        }

        private SQLiteConnection createConnection(string _dbFilename)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + _dbFilename + ";Version=3;");

            return conn;
        }

        private SQLiteCommand LaunchCommand(SQLiteConnection conn, string text)
        {
            var cmd = conn.CreateCommand();

            cmd = conn.CreateCommand();
            cmd.CommandText = text;

            return cmd;
        }

        public void readElements(string _dbFilename)
        {

            using (SQLiteConnection conn = createConnection(_dbFilename))
            {
                conn.Open();

                 var cmd = LaunchCommand(conn, "select id,external_id,category,isType FROM _objects_id");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        RevitElement revitElement = new RevitElement() { ElementId = id };

                        string guid = reader.GetString(1);
                        string cat = reader.GetString(2);
                        int isType = reader.GetInt32(3);

                        revitElement.Category = cat;
                        revitElement.UniqueId = guid;
                        revitElement.IsType = (isType == 1);

                        observableRevitElements.Add(revitElement);
                    }
                }
            }
        }

        public Dictionary<string, int> GetCategoryCount(string _dbFilename)
        {
            Dictionary<string, int> CountCategory = new Dictionary<string, int>();

            using (SQLiteConnection conn = createConnection(_dbFilename))
            {
                conn.Open();

                var cmd = LaunchCommand(conn, "select category, count(category) from _objects_id group by category");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString(0);
                        int count = reader.GetInt32(1);
                        CountCategory[category] = count;
                    }
                }
            }
            return CountCategory;
        }

        public List<RevitElement> GetCategoryCount(string dbFilePath, string dbFileName)
        {
            List<RevitElement> CountCategory = new List<RevitElement>();

            using (SQLiteConnection conn = createConnection(dbFilePath))
            {
                conn.Open();

                var cmd = LaunchCommand(conn, "select category, count(category) from _objects_id group by category");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader.GetString(0);
                        int count = reader.GetInt32(1);
                        RevitElement element = new RevitElement() { Category = category, DBFileName = dbFileName, ElementId=count};
                        CountCategory.Add(element);
                    }
                }
            }
            return CountCategory;
        }
    }
}
