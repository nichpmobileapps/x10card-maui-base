using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
 public   class QualficationDetailsDatabase
    {
        private SQLiteConnection conn;
        public QualficationDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<QualficationDetails>();
        }

        public IEnumerable<QualficationDetails> GetQualficationDetails(string Querryhere)
        {
            var list = conn.Query<QualficationDetails>(Querryhere);
            return list.ToList();
        }
        public string AddQualficationDetails(QualficationDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteQualficationDetails()
        {
            var del = conn.Query<QualficationDetails>("delete from QualficationDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<QualficationDetails>(query);
            return "success";
        }
    }
}
