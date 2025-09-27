using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class PhysicallyHandicappedDatabase
    {
        private SQLiteConnection conn;
        public PhysicallyHandicappedDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<PhysicallyHandicapped>();
        }

        public IEnumerable<PhysicallyHandicapped> GetPhysicallyHandicapped(string Querryhere)
        {
            var list = conn.Query<PhysicallyHandicapped>(Querryhere);
            return list.ToList();
        }
        public string AddPhysicallyHandicapped(PhysicallyHandicapped service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeletePhysicallyHandicapped()
        {
            var del = conn.Query<PhysicallyHandicapped>("delete from PhysicallyHandicapped");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<PhysicallyHandicapped>(query);
            return "success";
        }
    }
}
