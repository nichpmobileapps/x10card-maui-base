using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;


namespace X10Card.Models
{
    public class InvalidLoginAttemptDatabase
    {
        private SQLiteConnection conn;
        public InvalidLoginAttemptDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<InvalidLoginAttempt>();
        }

        public IEnumerable<InvalidLoginAttempt> GetInvalidLoginAttempt(string Querryhere)
        {
            var list = conn.Query<InvalidLoginAttempt>(Querryhere);
            return list.ToList();
        }
        public string AddInvalidLoginAttempt(InvalidLoginAttempt service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteInvalidLoginAttempt()
        {
            var del = conn.Query<InvalidLoginAttempt>("delete from InvalidLoginAttempt");
            return "success";
        }
        public string customDeleteInvalidLoginAttempt(string userid)
        {
            var del = conn.Query<InvalidLoginAttempt>($"delete from InvalidLoginAttempt where userid='{userid}'");
            return "success";
        }
        public string DeleteBefore15Minutes()
        {
            string que = $"DELETE from InvalidLoginAttempt where AttemptedDateTime < '{DateTime.Now.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss")}'";
            var del = conn.Query<InvalidLoginAttempt>(que);
            return "success";
        }
    }
}
