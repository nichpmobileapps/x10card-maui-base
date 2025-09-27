using SQLite;
using System.Collections.Generic;
using System.Linq;



namespace X10Card.Models.NewUserRegistration
{
   public class ContactDetailsDatabase
    {
        private SQLiteConnection conn;
        public ContactDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<ContactDetails>();
        }

        public IEnumerable<ContactDetails> GetContactDetails(string Querryhere)
        {
            var list = conn.Query<ContactDetails>(Querryhere);
            return list.ToList();
        }
        public string AddContactDetails(ContactDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteContactDetails()
        {
            var del = conn.Query<ContactDetails>("delete from ContactDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<ContactDetails>(query);
            return "success";
        }
    }
}
