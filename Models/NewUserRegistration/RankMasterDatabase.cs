using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class RankMasterDatabase
    {
        private SQLiteConnection conn;
        public RankMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<RankMaster>();
        }

        public IEnumerable<RankMaster> GetRankMaster(string Querryhere)
        {
            var list = conn.Query<RankMaster>(Querryhere);
            return list.ToList();
        }
        public string AddRankMaster(RankMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteRankMaster()
        {
            var del = conn.Query<RankMaster>("delete from RankMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<RankMaster>(query);
            return "success";
        }
    }
}
