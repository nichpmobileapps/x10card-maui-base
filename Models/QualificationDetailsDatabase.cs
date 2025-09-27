using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class QualificationDetailsDatabase
    {
        private SQLiteConnection conn;
        public QualificationDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<QualificationDetails>();
        }

        public IEnumerable<QualificationDetails> GetQualificationDetails(string Querryhere)
        {
            var list = conn.Query<QualificationDetails>(Querryhere);
            return list.ToList();
        }
        public string AddQualificationDetails(QualificationDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteQualificationDetails()
        {
            var del = conn.Query<QualificationDetails>("delete from QualificationDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<QualificationDetails>(query);
            return "success";
        }
    }
}
