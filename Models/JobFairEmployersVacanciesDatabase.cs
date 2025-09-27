using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class JobFairEmployersVacanciesDatabase
    {
        private SQLiteConnection conn;
        public JobFairEmployersVacanciesDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<JobFairEmployersVacancies>();
        }

        public IEnumerable<JobFairEmployersVacancies> GetJobFairEmployersVacancies(string Querryhere)
        {
            var list = conn.Query<JobFairEmployersVacancies>(Querryhere);
            return list.ToList();
        }
        public string AddJobFairEmployersVacancies(JobFairEmployersVacancies service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteJobFairEmployersVacancies()
        {
            var del = conn.Query<JobFairEmployersVacancies>("delete from JobFairEmployersVacancies");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<JobFairEmployersVacancies>(query);
            return "success";
        }
    }
}
