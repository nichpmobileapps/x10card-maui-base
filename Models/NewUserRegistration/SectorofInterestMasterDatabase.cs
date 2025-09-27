using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class SectorofInterestMasterDatabase
    {
        private SQLiteConnection conn;
        public SectorofInterestMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<SectorofInterestMaster>();
        }

        public IEnumerable<SectorofInterestMaster> GetSectorofInterestMaster(string Querryhere)
        {
            var list = conn.Query<SectorofInterestMaster>(Querryhere);
            return list.ToList();
        }
        public string AddSectorofInterestMaster(SectorofInterestMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSectorofInterestMaster()
        {
            var del = conn.Query<SectorofInterestMaster>("delete from SectorofInterestMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<SectorofInterestMaster>(query);
            return "success";
        }
    }
}
