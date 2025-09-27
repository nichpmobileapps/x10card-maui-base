using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class GetNCODetailsDatabase
    {
        private SQLiteConnection conn;
        public GetNCODetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<GetNCODetails>();
        }

        public IEnumerable<GetNCODetails> GetGetNCODetails(string Querryhere)
        {
            var list = conn.Query<GetNCODetails>(Querryhere);
            return list.ToList();
        }
        public string AddGetNCODetails(GetNCODetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteGetNCODetails()
        {
            var del = conn.Query<GetNCODetails>("delete from GetNCODetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<GetNCODetails>(query);
            return "success";
        }
    }
}
