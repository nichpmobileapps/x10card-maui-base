using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class SubEmploymentStatusDatabase
    {
        private SQLiteConnection conn;
        public SubEmploymentStatusDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<SubEmploymentStatus>();
        }

        public IEnumerable<SubEmploymentStatus> GetSubEmploymentStatus(string Querryhere)
        {
            var list = conn.Query<SubEmploymentStatus>(Querryhere);
            return list.ToList();
        }
        public string AddSubEmploymentStatus(SubEmploymentStatus service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSubEmploymentStatus()
        {
            var del = conn.Query<SubEmploymentStatus>("delete from SubEmploymentStatus");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<SubEmploymentStatus>(query);
            return "success";
        }
    }
}
