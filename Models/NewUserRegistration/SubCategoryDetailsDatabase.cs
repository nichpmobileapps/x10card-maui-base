using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class SubCategoryDetailsDatabase
    {
        private SQLiteConnection conn;
        public SubCategoryDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<SubCategoryDetails>();
        }

        public IEnumerable<SubCategoryDetails> GetSubCategoryDetails(string Querryhere)
        {
            var list = conn.Query<SubCategoryDetails>(Querryhere);
            return list.ToList();
        }
        public string AddSubCategoryDetails(SubCategoryDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSubCategoryDetails()
        {
            var del = conn.Query<SubCategoryDetails>("delete from SubCategoryDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<SubCategoryDetails>(query);
            return "success";
        }
    }
}
