using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class CharacterMasterDatabase
    {
        private SQLiteConnection conn;
        public CharacterMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<CharacterMaster>();
        }

        public IEnumerable<CharacterMaster> GetCharacterMaster(string Querryhere)
        {
            var list = conn.Query<CharacterMaster>(Querryhere);
            return list.ToList();
        }
        public string AddCharacterMaster(CharacterMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteCharacterMaster()
        {
            var del = conn.Query<CharacterMaster>("delete from CharacterMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<CharacterMaster>(query);
            return "success";
        }
    }
}
