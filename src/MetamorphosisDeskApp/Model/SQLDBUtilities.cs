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

        

        public SQLDBUtilities()
        {
        
            
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

        public List<RevitElement> readElementsFromDB(string dbFilePath, string dbFileName)
        {
            List<RevitElement> revitElements = new List<RevitElement>();

            using (SQLiteConnection conn = createConnection(dbFilePath))
            {
                conn.Open();

                //var cmd = LaunchCommand(conn, "select id,external_id,category,isType FROM _objects_id");
                var cmd = LaunchCommand(conn, "SELECT _objects_id.id, _objects_id.external_id, _objects_id.category,_objects_id.isType, _objects_geom.BoundingBoxMax, _objects_geom.BoundingBoxMin FROM _objects_geom JOIN _objects_id ON _objects_geom.id = _objects_id.id");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        RevitElement revitElement = new RevitElement() { ElementId = id };

                        string guid = reader.GetString(1);
                        string cat = reader.GetString(2);
                        int isType = reader.GetInt32(3);

                        
                        PointXYZ bboxMaxPt = PointXYZ.ParsePoint(reader.GetString(4));
                        PointXYZ bboxMinPt = PointXYZ.ParsePoint(reader.GetString(5));

                        //string[] bboxMax = reader.GetBlob(4,true).ToString().Split(',');
                        //string[] bboxMin = reader.GetBlob(5,true).ToString().Split(',');

                        //PointXYZ bboxMaxPt = new PointXYZ(double.Parse(bboxMax[0]), double.Parse(bboxMax[1]), double.Parse(bboxMax[2]));
                        //PointXYZ bboxMinPt = new PointXYZ(double.Parse(bboxMin[0]), double.Parse(bboxMin[1]), double.Parse(bboxMin[2]));

                        revitElement.DBFileName = dbFileName;
                        revitElement.CategoryCount = 1;
                        revitElement.CategoryName = cat;
                        revitElement.UniqueId = guid;
                        revitElement.IsType = (isType == 1);

                        PointXYZ centroid = new PointXYZ(0, 0, -1);

                        if (bboxMaxPt != null && bboxMinPt != null)
                        {
                            centroid = PointXYZ.Center(bboxMaxPt, bboxMinPt);
                        }

                        //revitElement.LocationPoint = PointXYZ.ToString(centroid);
                        revitElement.LocationPoint = centroid;

                        revitElements.Add(revitElement);
                    }
                }
            }
            return revitElements;
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

        public List<RevitCategories> GetCategoryCount(string dbFilePath, string dbFileName)
        {
            List<RevitCategories> CountCategory = new List<RevitCategories>();

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
                        RevitCategories element = new RevitCategories() { CategoryName = category, DBFileName = dbFileName, CategoryCount=count, VariationOnPrevious=count};
                        CountCategory.Add(element);
                    }
                }
            }
            return CountCategory;
        }
    }
}
