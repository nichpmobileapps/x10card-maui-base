using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class NCODatabase
    {
        private SQLiteConnection conn;
        public NCODatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<NCO>();
        }

        public IEnumerable<NCO> GetNCO(string Querryhere)
        {
            var list = conn.Query<NCO>(Querryhere);
            return list.ToList();
        }
        public string AddNCO(NCO service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteNCO()
        {
            var del = conn.Query<NCO>("delete from NCO");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<NCO>(query);
            return "success";
        }
    }
}
