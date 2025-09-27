using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class LanguageMasterDatabase
    {
        private SQLiteConnection conn;
        public LanguageMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<LanguageMaster>();
        }

        public IEnumerable<LanguageMaster> GetLanguageMaster(string Querryhere)
        {
            var list = conn.Query<LanguageMaster>(Querryhere);
            return list.ToList();
        }
        public string AddLanguageMaster(LanguageMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteLanguageMaster()
        {
            var del = conn.Query<LanguageMaster>("delete from LanguageMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<LanguageMaster>(query);
            return "success";
        }
    }
}
