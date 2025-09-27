using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class ExistingUserDataDatabase
    {
        private SQLiteConnection conn;
        public ExistingUserDataDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<ExistingUserData>();
        }

        public IEnumerable<ExistingUserData> GetExistingUserData(string Querryhere)
        {
            var list = conn.Query<ExistingUserData>(Querryhere);
            return list.ToList();
        }
        public string AddExistingUserData(ExistingUserData service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteExistingUserData()
        {
            var del = conn.Query<ExistingUserData>("delete from ExistingUserData");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ExistingUserData>(query);
            return "success";
        }
    }
}
