using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace X10Card.Models.NewUserRegistration
{
    public class SubmittedFormsDatabase
    {
        private SQLiteConnection conn;
        public SubmittedFormsDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<SubmittedFormsDetails>();
        }

        public IEnumerable<SubmittedFormsDetails> GetSubmittedFormsDetails(string Querryhere)
        {
            var list = conn.Query<SubmittedFormsDetails>(Querryhere);
            return list.ToList();
        }
        public string AddSubmittedFormsDetails(SubmittedFormsDetails service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteSubmittedFormsDetails()
        {
            var del = conn.Query<SubmittedFormsDetails>("delete from SubmittedFormsDetails");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<SubmittedFormsDetails>(query);
            return "success";
        }
    }
}
