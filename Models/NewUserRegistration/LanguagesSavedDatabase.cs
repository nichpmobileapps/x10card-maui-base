using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class LanguagesSavedDatabase
    {
        private SQLiteConnection conn;
        public LanguagesSavedDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<LanguagesSaved>();
        }

        public IEnumerable<LanguagesSaved> GetLanguagesSaved(string Querryhere)
        {
            var list = conn.Query<LanguagesSaved>(Querryhere);
            return list.ToList();
        }
        public string AddLanguagesSaved(LanguagesSaved service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteLanguagesSaved()
        {
            var del = conn.Query<LanguagesSaved>("delete from LanguagesSaved");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<LanguagesSaved>(query);
            return "success";
        }
    }
}
