using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
  public  class MiscellaneousDetailsDatabase
    {
        private SQLiteConnection conn;
        public MiscellaneousDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<MiscellaneousDetails>();
        }

        public IEnumerable<MiscellaneousDetails> GetMiscellaneousDetails(string Querryhere)
        {
            var list = conn.Query<MiscellaneousDetails>(Querryhere);
            return list.ToList();
        }
        public string AddMiscellaneousDetails(MiscellaneousDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteMiscellaneousDetails()
        {
            var del = conn.Query<MiscellaneousDetails>("delete from MiscellaneousDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<MiscellaneousDetails>(query);
            return "success";
        }
    }
}
