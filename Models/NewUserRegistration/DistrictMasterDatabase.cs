using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class DistrictMasterDatabase
    {
        private SQLiteConnection conn;
        public DistrictMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<DistrictMaster>();
        }
        public IEnumerable<DistrictMaster> GetDistrictMaster(String Querryhere)
        {
            var list = conn.Query<DistrictMaster>(Querryhere);
            return list.ToList();
        }
        public string AddDistrictMaster(DistrictMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteDistrictMaster()
        {
            var del = conn.Query<DistrictMaster>("delete from DistrictMaster");
            return "success";
        }
    }
}