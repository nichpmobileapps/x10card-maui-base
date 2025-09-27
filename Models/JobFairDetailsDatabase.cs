using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace X10Card.Models
{
    public class JobFairDetailsDatabase
    {
        private SQLiteConnection conn;
        public JobFairDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<JobFairDetails>();
        }

        public IEnumerable<JobFairDetails> GetJobFairDetails(string Querryhere)
        {
            var list = conn.Query<JobFairDetails>(Querryhere);
            return list.ToList();
        }
        public string AddJobFairDetails(JobFairDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteJobFairDetails()
        {
            var del = conn.Query<JobFairDetails>("delete from JobFairDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<JobFairDetails>(query);
            return "success";
        }
    }
}
