using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class SubCategoryDatabase
    {
        private SQLiteConnection conn;
        public SubCategoryDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<SubCategory>();
        }

        public IEnumerable<SubCategory> GetSubCategory(string Querryhere)
        {
            var list = conn.Query<SubCategory>(Querryhere);
            return list.ToList();
        }
        public string AddSubCategory(SubCategory service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSubCategory()
        {
            var del = conn.Query<SubCategory>("delete from SubCategory");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<SubCategory>(query);
            return "success";
        }
    }
}
