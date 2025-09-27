using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class JobfairsEmployersDatabase
    {
        private SQLiteConnection conn;
        public JobfairsEmployersDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<JobfairsEmployers>();
        }

        public IEnumerable<JobfairsEmployers> GetJobfairsEmployers(string Querryhere)
        {
            var list = conn.Query<JobfairsEmployers>(Querryhere);
            return list.ToList();
        }
        public string AddJobfairsEmployers(JobfairsEmployers service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteJobfairsEmployers()
        {
            var del = conn.Query<JobfairsEmployers>("delete from JobfairsEmployers");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<JobfairsEmployers>(query);
            return "success";
        }
    }
}
