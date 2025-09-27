using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class ReligionMasterDatabase
    {
        private SQLiteConnection conn;
        public ReligionMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<ReligionMaster>();
        }

        public IEnumerable<ReligionMaster> GetReligionMaster(string Querryhere)
        {
            var list = conn.Query<ReligionMaster>(Querryhere);
            return list.ToList();
        }
        public string AddReligionMaster(ReligionMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteReligionMaster()
        {
            var del = conn.Query<ReligionMaster>("delete from ReligionMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ReligionMaster>(query);
            return "success";
        }
    }
}
