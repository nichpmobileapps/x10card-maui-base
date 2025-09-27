using SQLite;

namespace X10Card.Models
{
    public class AllowanceDetailsDatabase
    {
        private SQLiteConnection conn;
        public AllowanceDetailsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<AllowanceDetails>();
        }

        public IEnumerable<AllowanceDetails> GetAllowanceDetails(string Querryhere)
        {
            var list = conn.Query<AllowanceDetails>(Querryhere);
            return list.ToList();
        }
        public string AddAllowanceDetails(AllowanceDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteAllowanceDetails()
        {
            var del = conn.Query<AllowanceDetails>("delete from AllowanceDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<AllowanceDetails>(query);
            return "success";
        }
    }
}
