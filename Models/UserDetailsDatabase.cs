using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class UserDetailsDatabase
    {
        private SQLiteConnection conn;
        public UserDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<UserDetails>();
        }

        public IEnumerable<UserDetails> GetUserDetails(string Querryhere)
        {
            var list = conn.Query<UserDetails>(Querryhere);
            return list.ToList();
        }
        public string AddUserDetails(UserDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteUserDetails()
        {
            var del = conn.Query<UserDetails>("delete from UserDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<UserDetails>(query);
            return "success";
        }
    }
}
