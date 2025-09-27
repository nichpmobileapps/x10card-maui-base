using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class AlreadyRegisteredDatabase
    {
        private SQLiteConnection conn;
        public AlreadyRegisteredDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<AlreadyRegistered>();
        }

        public IEnumerable<AlreadyRegistered> GetAlreadyRegistered(string Querryhere)
        {
            var list = conn.Query<AlreadyRegistered>(Querryhere);
            return list.ToList();
        }
        public string AddAlreadyRegistered(AlreadyRegistered service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteAlreadyRegistered()
        {
            var del = conn.Query<AlreadyRegistered>("delete from AlreadyRegistered");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<AlreadyRegistered>(query);
            return "success";
        }
    }
}
