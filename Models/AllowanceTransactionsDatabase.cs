using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models
{
    public class AllowanceTransactionsDatabase
    {
        private SQLiteConnection conn;
        public AllowanceTransactionsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<AllowanceTransactions>();
        }

        public IEnumerable<AllowanceTransactions> GetAllowanceTransactions(string Querryhere)
        {
            var list = conn.Query<AllowanceTransactions>(Querryhere);
            return list.ToList();
        }
        public string AddAllowanceTransactions(AllowanceTransactions service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteAllowanceTransactions()
        {
            var del = conn.Query<AllowanceTransactions>("delete from AllowanceTransactions");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<AllowanceTransactions>(query);
            return "success";
        }
    }
}
