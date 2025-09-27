using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class BoardMasterDatabase
    {
        private SQLiteConnection conn;
        public BoardMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<BoardMaster>();
        }

        public IEnumerable<BoardMaster> GetBoardMaster(string Querryhere)
        {
            var list = conn.Query<BoardMaster>(Querryhere);
            return list.ToList();
        }
        public string AddBoardMaster(BoardMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteBoardMaster()
        {
            var del = conn.Query<BoardMaster>("delete from BoardMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<BoardMaster>(query);
            return "success";
        }
    }
}
