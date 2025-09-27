using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class QualificationMasterDatabase
    {
        private SQLiteConnection conn;
        public QualificationMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<QualificationMaster>();
        }

        public IEnumerable<QualificationMaster> GetQualificationMaster(string Querryhere)
        {
            var list = conn.Query<QualificationMaster>(Querryhere);
            return list.ToList();
        }
        public string AddQualificationMaster(QualificationMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteQualificationMaster()
        {
            var del = conn.Query<QualificationMaster>("delete from QualificationMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<QualificationMaster>(query);
            return "success";
        }
    }
}
