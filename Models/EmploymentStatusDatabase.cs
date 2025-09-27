using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class EmploymentStatusDatabase
    {

        private SQLiteConnection conn;
        public EmploymentStatusDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<EmploymentStatus>();
        }

        public IEnumerable<EmploymentStatus> GetEmploymentStatus(string Querryhere)
        {
            var list = conn.Query<EmploymentStatus>(Querryhere);
            return list.ToList();
        }
        public string AddEmploymentStatus(EmploymentStatus service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteEmploymentStatus()
        {
            var del = conn.Query<EmploymentStatus>("delete from EmploymentStatus");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<EmploymentStatus>(query);
            return "success";
        }
    }
}
