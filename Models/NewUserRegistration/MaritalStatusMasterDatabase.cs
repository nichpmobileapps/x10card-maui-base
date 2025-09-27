using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class MaritalStatusMasterDatabase
    {
        private SQLiteConnection conn;
        public MaritalStatusMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<MaritalStatusMaster>();
        }

        public IEnumerable<MaritalStatusMaster> GetMaritalStatusMaster(string Querryhere)
        {
            var list = conn.Query<MaritalStatusMaster>(Querryhere);
            return list.ToList();
        }
        public string AddMaritalStatusMaster(MaritalStatusMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteMaritalStatusMaster()
        {
            var del = conn.Query<MaritalStatusMaster>("delete from MaritalStatusMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<MaritalStatusMaster>(query);
            return "success";
        }
    }
}
