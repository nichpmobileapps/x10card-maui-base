using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class NCODetailsDatabase
    {
        private SQLiteConnection conn;
        public NCODetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<NCODetails>();
        }

        public IEnumerable<NCODetails> GetNCODetails(string Querryhere)
        {
            var list = conn.Query<NCODetails>(Querryhere);
            return list.ToList();
        }
        public string AddNCODetails(NCODetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteNCODetails()
        {
            var del = conn.Query<NCODetails>("delete from NCODetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<NCODetails>(query);
            return "success";
        }
    }
}
