using SQLite;
using System.Collections.Generic;
using System.Linq;


namespace X10Card.Models.NewUserRegistration
{
    public class OrganisationMasterDatabase
    {
        private SQLiteConnection conn;
        public OrganisationMasterDatabase()
        {
            conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.DBName));
            conn.CreateTable<OrganisationMaster>();
        }

        public IEnumerable<OrganisationMaster> GetOrganisationMaster(string Querryhere)
        {
            var list = conn.Query<OrganisationMaster>(Querryhere);
            return list.ToList();
        }
        public string AddOrganisationMaster(OrganisationMaster service)
        {
            conn.Insert(service);
            return "success";
        }
        public string DeleteOrganisationMaster()
        {
            var del = conn.Query<OrganisationMaster>("delete from OrganisationMaster");
            return "success";
        }
        public string UpdateCustomquery(string query)
        {
            var update = conn.Query<OrganisationMaster>(query);
            return "success";
        }
        public string InsertOrgNameList(string insertintoorg)
        {
            string query = $"insert into OrganisationMaster ('OrgId','OrgName') " +
                 $" values {insertintoorg}";
            conn.Query<OrganisationMaster>(query);
            return "success";
        }
    }
}
