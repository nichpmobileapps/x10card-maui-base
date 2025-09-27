using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class VillageMasterDatabase
    {
        private SQLiteConnection conn;
        public VillageMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<VillageMaster>();
        }

        public IEnumerable<VillageMaster> GetVillageMaster(string Querryhere)
        {
            var list = conn.Query<VillageMaster>(Querryhere);
            return list.ToList();
        }
        public string AddVillageMaster(VillageMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteVillageMaster()
        {
            var del = conn.Query<VillageMaster>("delete from VillageMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<VillageMaster>(query);
            return "success";
        }
    }
}
