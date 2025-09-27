using SQLite;
using System.Collections.Generic;
using System.Linq;



namespace X10Card.Models.NewUserRegistration
{
    public class PersonalDetailsDatabase
    {
        private SQLiteConnection conn;
        public PersonalDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<PersonalDetails>();
        }

        public IEnumerable<PersonalDetails> GetPersonalDetails(string Querryhere)
        {
            var list = conn.Query<PersonalDetails>(Querryhere);
            return list.ToList();
        }
        public string AddPersonalDetails(PersonalDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeletePersonalDetails()
        {
            var del = conn.Query<PersonalDetails>("delete from PersonalDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<PersonalDetails>(query);
            return "success";
        }

    }
}
