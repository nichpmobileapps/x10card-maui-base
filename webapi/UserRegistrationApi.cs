using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Web;
using X10Card.Models.NewUserRegistration;

namespace X10Card.Models
{
    public class UserRegistrationApi
    {
        /*   public string baseurluserregapi = "https://eemis.hp.nic.in/NewUserRegistration.svc/";
           public string pdfurluserregapi = "https://eemis.hp.nic.in/ViewPDF/ViewPDF.aspx?";*/
        public string baseurluserregapi = "http://10.146.2.67/eemis/NewUserRegistration.svc/";
        public string pdfurluserregapi = "http://10.146.2.67/eemis/ViewPDF/ViewPDF.aspx?";

        string UserID = "";
        DistrictMasterDatabase districtMasterDatabase = new DistrictMasterDatabase();
        ExchangeNameDatabase exchangeNameDatabase = new ExchangeNameDatabase();
        ReligionMasterDatabase religionMasterDatabase = new ReligionMasterDatabase();
        MaritalStatusMasterDatabase maritalStatusMasterDatabase = new MaritalStatusMasterDatabase();
        CategoryMasterDatabase categoryMasterDatabase = new CategoryMasterDatabase();
        TehsilMasterDatabase tehsilMasterDatabase = new TehsilMasterDatabase();
        VillageMasterDatabase villageMasterDatabase = new VillageMasterDatabase();
        QualificationMasterDatabase qualificationMasterDatabase = new QualificationMasterDatabase();
        BoardMasterDatabase boardMasterDatabase = new BoardMasterDatabase();
        SectorofInterestMasterDatabase sectorofInterestMasterDatabase = new SectorofInterestMasterDatabase();
        LanguageMasterDatabase languageMasterDatabase = new LanguageMasterDatabase();
        EmploymentStatusDatabase employmentStatusDatabase = new EmploymentStatusDatabase();
        SubEmploymentStatusDatabase subEmploymentStatusDatabase = new SubEmploymentStatusDatabase();
        OrganisationMasterDatabase organisationMasterDatabase = new OrganisationMasterDatabase();
        SubCategoryDatabase subCategoryDatabase = new SubCategoryDatabase();
        PhysicallyHandicappedDatabase physicallyHandicappedDatabase = new PhysicallyHandicappedDatabase();
        ForceMasterDatabase forceMasterDatabase = new ForceMasterDatabase();
        RankMasterDatabase rankMasterDatabase = new RankMasterDatabase();
        MedicalMasterDatabase medicalMasterDatabase = new MedicalMasterDatabase();
        CharacterMasterDatabase characterMasterDatabase = new CharacterMasterDatabase();
        ReasonMasterDatabase reasonMasterDatabase = new ReasonMasterDatabase();
        NCODatabase ncoDatabase = new NCODatabase();
        ApplicantDashboardDatabase applicantDashboardDatabase = new ApplicantDashboardDatabase();
        ExistingUserDataDatabase existingUserDataDatabase = new ExistingUserDataDatabase();
        AlreadyRegisteredDatabase alreadyRegisteredDatabase = new AlreadyRegisteredDatabase();
        PersonalDetailsDatabase personalDetailsDatabase = new PersonalDetailsDatabase();
        ContactDetailsDatabase contactDetailsDatabase = new ContactDetailsDatabase();
        QualficationDetailsDatabase qualficationDetailsDatabase = new QualficationDetailsDatabase();
        MiscellaneousDetailsDatabase miscellaneousDetailsDatabase = new MiscellaneousDetailsDatabase();
        GetLangDetailsDatabase getLangDetailsDatabase = new GetLangDetailsDatabase();
        SubCategoryDetailsDatabase subCategoryDetailsDatabase = new SubCategoryDetailsDatabase();
        EmployedDetailsDatabase employedDetailsDatabase = new EmployedDetailsDatabase();
        PHDetailsDatabase pHDetailsDatabase = new PHDetailsDatabase();
        XservicemenDetailsDatabase xservicemenDetailsDatabase = new XservicemenDetailsDatabase();
        GetNCODetailsDatabase getNCODetailsDatabase = new GetNCODetailsDatabase();
        SubmittedFormsDatabase submittedFormsDatabase = new SubmittedFormsDatabase();

        private static string SafeDecrypt(JToken node, string key)
        {
            return AesCryptography.Decrypt(node?[key]?.ToString() ?? string.Empty);
        }
        public async Task<int> Login_SSO_ID(string sso_id)
        {
            string msg = string.Empty;
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;

                try
                {
                    var jsonData = new
                    {
                        SSOID = AesCryptography.Encrypt(sso_id),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"SSODATA", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            var message = parsed["message"]?.ToObject<JObject>();
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            if (status == 200)
                            {
                                string regno = message?["RegNo"]?.ToString() ?? string.Empty;
                                string UserID = message?["UserID"]?.ToString() ?? string.Empty;
                                Preferences.Set("UserID", UserID);
                                Preferences.Set("RegNo", regno);
                            }
                            else
                            {
                                await App.ShowAlertBox(App.AppName, msg);
                            }                           
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, msg);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }
        public async Task<int> Login(string userid, string password)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;

                try
                {
                    var jsonData = new
                    {
                        Userid = AesCryptography.Encrypt(userid),
                        Password = AesCryptography.GetSha256FromString(password),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"Login", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;

                            var message = parsed["message"]?.ToObject<JObject>();
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            string regno = message?["RegNo"]?.ToString() ?? string.Empty;
                            string UserID = message?["UserID"]?.ToString() ?? string.Empty;


                            Preferences.Set("UserID", UserID);
                            Preferences.Set("RegNo", regno);

                            if (status == 500 || status == 201)
                            {
                                await App.ShowAlertBox(App.AppName, msg);
                            }
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, "Error while retrieving data.");
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }
    

        public async Task<int> GetDistrict()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"Districtslist?";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    districtMasterDatabase = new DistrictMasterDatabase();
                    districtMasterDatabase.DeleteDistrictMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "DistrictsList")
                        {
                            var nodes = pair.Value;
                            var item = new DistrictMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.DistrictID = node["DistrictID"]?.ToString() ?? string.Empty;
                                    item.DistrictName = node["DistrictName"]?.ToString() ?? string.Empty;

                                    districtMasterDatabase.AddDistrictMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetExchange(string DisCd)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetExchangeList?"
                + $"DistCd={HttpUtility.UrlEncode(DisCd)}";
                //+$"&packageid={HttpUtility.UrlEncode(AesCryptography.Encrypt(AppInfo.PackageName))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    exchangeNameDatabase.DeleteExchangeNameMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "ExchangeList")
                        {
                            var nodes = pair.Value;
                            var item = new ExchangeNameMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.ExchangeID = node["ExchangeID"]?.ToString() ?? string.Empty;
                                    item.ExchangeName = node["ExchangeName"]?.ToString() ?? string.Empty;

                                    exchangeNameDatabase.AddExchangeNameMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetReligion()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetReligionListMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    religionMasterDatabase.DeleteReligionMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "ReligionList")
                        {
                            var nodes = pair.Value;
                            var item = new ReligionMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.ReligionID = node["ReligionID"]?.ToString() ?? string.Empty;
                                    item.ReligionName = node["ReligionName"]?.ToString() ?? string.Empty;

                                    religionMasterDatabase.AddReligionMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetMaritalStatus()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetMaritalStatusMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    maritalStatusMasterDatabase.DeleteMaritalStatusMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "MaritalStatus")
                        {
                            var nodes = pair.Value;
                            var item = new MaritalStatusMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.MaritalStatusListID = node["MaritalStatusListID"]?.ToString() ?? string.Empty;
                                    item.MaritalStatusListName = node["MaritalStatusListName"]?.ToString() ?? string.Empty;

                                    maritalStatusMasterDatabase.AddMaritalStatusMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetCategory()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetCategoryMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    categoryMasterDatabase.DeleteCategoryMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "Category")
                        {
                            var nodes = pair.Value;
                            var item = new CategoryMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.CategoryID = node["CategoryID"]?.ToString() ?? string.Empty;
                                    item.CategoryName = node["CategoryName"]?.ToString() ?? string.Empty;

                                    categoryMasterDatabase.AddCategoryMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetTehsil(string DisCd)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetTehsilMaster?"
                + $"DistCd={HttpUtility.UrlEncode(DisCd)}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    tehsilMasterDatabase.DeleteTehsilMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "Tehsil")
                        {
                            var nodes = pair.Value;
                            var item = new TehsilMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.TehsilID = node["TehsilID"]?.ToString() ?? string.Empty;
                                    item.TehsilName = node["TehsilName"]?.ToString() ?? string.Empty;

                                    tehsilMasterDatabase.AddTehsilMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetVillageMaster(string DisCd, string TehsilCd)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetVillageMaster?"
                + $"DistCd={HttpUtility.UrlEncode(DisCd)}"
                + $"&TehsilCd={HttpUtility.UrlEncode(TehsilCd)}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    villageMasterDatabase.DeleteVillageMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "Village")
                        {
                            var nodes = pair.Value;
                            var item = new VillageMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.VillageID = node["VillageID"]?.ToString() ?? string.Empty;
                                    item.VillageName = node["VillageName"]?.ToString() ?? string.Empty;

                                    villageMasterDatabase.AddVillageMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetQualificationMaster()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetQualificationMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    qualificationMasterDatabase.DeleteQualificationMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "QualificationList")
                        {
                            var nodes = pair.Value;
                            var item = new QualificationMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.QualificationID = node["QualificationID"]?.ToString() ?? string.Empty;
                                    item.QualificationName = node["QualificationName"]?.ToString() ?? string.Empty;

                                    qualificationMasterDatabase.AddQualificationMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetBoardMaster()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetBoardMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    boardMasterDatabase.DeleteBoardMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "BoardList")
                        {
                            var nodes = pair.Value;
                            var item = new BoardMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.BoardID = node["BoardID"]?.ToString() ?? string.Empty;
                                    item.BoardName = node["BoardName"]?.ToString() ?? string.Empty;

                                    boardMasterDatabase.AddBoardMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetSectorofInterestMaster()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetSectorofInterestMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    sectorofInterestMasterDatabase.DeleteSectorofInterestMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "SectorofInterestList")
                        {
                            var nodes = pair.Value;
                            var item = new SectorofInterestMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.SectorID = node["SectorID"]?.ToString() ?? string.Empty;
                                    item.SectorName = node["SectorName"]?.ToString() ?? string.Empty;

                                    sectorofInterestMasterDatabase.AddSectorofInterestMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetLanguagesKnownMaster()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetLanguagesKnownMaster?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    languageMasterDatabase.DeleteLanguageMaster();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "LanguageKnownList")
                        {
                            var nodes = pair.Value;
                            var item = new LanguageMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.LanguageID = node["LanguageID"]?.ToString() ?? string.Empty;
                                    item.LanguageName = node["LanguageName"]?.ToString() ?? string.Empty;

                                    languageMasterDatabase.AddLanguageMaster(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetEmploymentStatus()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetEmploymentStatus?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    employmentStatusDatabase.DeleteEmploymentStatus();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "EmploymentStatus")
                        {
                            var nodes = pair.Value;
                            var item = new EmploymentStatus();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.EmpStatCd = node["EmpStatCd"]?.ToString() ?? string.Empty;
                                    item.EmpStatDesc = node["EmpStatDesc"]?.ToString() ?? string.Empty;

                                    employmentStatusDatabase.AddEmploymentStatus(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetSubEmploymentStatus()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetSubEmploymentStatus?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    subEmploymentStatusDatabase.DeleteSubEmploymentStatus();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "SubEmploymentStatus")
                        {
                            var nodes = pair.Value;
                            var item = new SubEmploymentStatus();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.SubEmpStatCd = node["SubEmpStatCd"]?.ToString() ?? string.Empty;
                                    item.SubEmpStatDesc = node["SubEmpStatDesc"]?.ToString() ?? string.Empty;
                                    item.SSubEmpStatCd = node["SSubEmpStatCd"]?.ToString() ?? string.Empty;
                                    item.SSubEmpStatDesc = node["SSubEmpStatDesc"]?.ToString() ?? string.Empty;

                                    subEmploymentStatusDatabase.AddSubEmploymentStatus(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetOrganisationName()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetOrganisationName?";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "OrganisationNamelist")
                        {
                            organisationMasterDatabase.DeleteOrganisationMaster();
                            /*int i = 0;
                            string insertintorg = null;*/

                            var nodes = pair.Value;
                            var item = new OrganisationMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.OrgId = node["OrgId"]?.ToString() ?? string.Empty;
                                    item.OrgName = node["OrgName"]?.ToString() ?? string.Empty;

                                    organisationMasterDatabase.AddOrganisationMaster(item);

                                }
                            }
                           
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetSubCategory()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetSubCategory?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    subCategoryDatabase.DeleteSubCategory();

                    foreach (var pair in parsed)
                    {

                        if (pair.Key == "SubCategorylist")
                        {
                            var nodes = pair.Value;
                            var item = new SubCategory();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.SubCategoryId = node["SubCategoryId"]?.ToString() ?? string.Empty;
                                    item.SubCategoryName = node["SubCategoryName"]?.ToString() ?? string.Empty;

                                    subCategoryDatabase.AddSubCategory(item);
                                }
                            }
                        }
                    }

                }
                return (int)response.StatusCode;

            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetPH()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetPH?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    physicallyHandicappedDatabase.DeletePhysicallyHandicapped();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "PHlist")
                        {
                            var nodes = pair.Value;
                            var item = new PhysicallyHandicapped();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.PHId = node["PHId"]?.ToString() ?? string.Empty;
                                    item.PHName = node["PHName"]?.ToString() ?? string.Empty;

                                    physicallyHandicappedDatabase.AddPhysicallyHandicapped(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetNCO()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetNCO?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    ncoDatabase.DeleteNCO();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "NCOlist")
                        {
                            var nodes = pair.Value;
                            var item = new NCO();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.NCOId = node["NCOId"]?.ToString() ?? string.Empty;
                                    item.NCOName = node["NCOName"]?.ToString() ?? string.Empty;

                                    ncoDatabase.AddNCO(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetForce()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetForce?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    forceMasterDatabase.DeleteForceMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "Forcelist")
                        {
                            var nodes = pair.Value;
                            var item = new ForceMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.ForceId = node["ForceId"]?.ToString() ?? string.Empty;
                                    item.ForceName = node["ForceName"]?.ToString() ?? string.Empty;

                                    forceMasterDatabase.AddForceMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetRank(string ForceCd)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetRank?"
                + $"ForceCd={HttpUtility.UrlEncode(ForceCd)}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    rankMasterDatabase.DeleteRankMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "Ranklist")
                        {
                            var nodes = pair.Value;
                            var item = new RankMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.RankId = node["RankId"]?.ToString() ?? string.Empty;
                                    item.RankName = node["RankName"]?.ToString() ?? string.Empty;

                                    rankMasterDatabase.AddRankMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {               
                return 500;
            }
        }

        public async Task<int> GetMedical()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetMedical?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    medicalMasterDatabase.DeleteMedicalMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "Medicallist")
                        {
                            var nodes = pair.Value;
                            var item = new MedicalMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.MedicalId = node["MedicalId"]?.ToString() ?? string.Empty;
                                    item.MedicalName = node["MedicalName"]?.ToString() ?? string.Empty;

                                    medicalMasterDatabase.AddMedicalMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetCharacter()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetCharacter?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    characterMasterDatabase.DeleteCharacterMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "Characterlist")
                        {
                            var nodes = pair.Value;
                            var item = new CharacterMaster();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.CharacterId = node["CharacterId"]?.ToString() ?? string.Empty;
                                    item.CharacterName = node["CharacterName"]?.ToString() ?? string.Empty;

                                    characterMasterDatabase.AddCharacterMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetReason()
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetReason?";


                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    reasonMasterDatabase.DeleteReasonMaster();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "Reasonlist")
                        {
                            var nodes = pair.Value;
                            if (nodes != null)
                            {
                                var item = new ReasonMaster();
                                foreach (var node in nodes)
                                {
                                    item.ReasonId = node["ReasonId"]?.ToString() ?? string.Empty;
                                    item.ReasonName = node["ReasonName"]?.ToString() ?? string.Empty;

                                    reasonMasterDatabase.AddReasonMaster(item);
                                }
                            }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> SavePersonalDetails(string RegNo, string XchangeCode, string UserName,
            string FAtherHusbandSelection, string TXTFHNAME, string txtmother, string ddlmarital,
            string GENDER, string DOB, string ddlreligion, string Category, string issuedate, string certificateno, string _file, string UserID)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    string ipaddress = await GetIPAddress();
                    if (string.IsNullOrEmpty(ipaddress))
                    {
                        ipaddress = "192.168.1.1";
                    }

                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        XchangeCode = AesCryptography.Encrypt(XchangeCode),
                        UserName = AesCryptography.Encrypt(UserName),
                        FAtherHusbandSelection = AesCryptography.Encrypt(FAtherHusbandSelection),
                        TXTFHNAME = AesCryptography.Encrypt(TXTFHNAME),
                        txtmother = txtmother != null ? AesCryptography.Encrypt(txtmother) : "",
                        ddlmarital = AesCryptography.Encrypt(ddlmarital),
                        GENDER = AesCryptography.Encrypt(GENDER),
                        DOB = AesCryptography.Encrypt(DOB),
                        ddlreligion = AesCryptography.Encrypt(ddlreligion),
                        Category = AesCryptography.Encrypt(Category),
                        GetIPAddress = AesCryptography.Encrypt(ipaddress),
                        issuedate = issuedate != null ? AesCryptography.Encrypt(issuedate) : "",
                        certificateno = certificateno != null ? AesCryptography.Encrypt(certificateno) : "",

                        document = _file != null ? _file : "",
                        UserID = UserID != null ? AesCryptography.Encrypt(UserID) : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"PersonsalInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"]?.ToObject<JObject>();

                            if (message != null && message.HasValues)
                            {
                                status = int.TryParse(message["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                                msg = message["message"]?.ToString() ?? string.Empty;

                                if (status == 200)
                                {
                                    string regno = message["RegNo"]?.ToString() ?? string.Empty;
                                    Preferences.Set("RegNo", regno);
                                    await App.ShowAlertBox(App.AppName, msg);
                                }
                                else
                                {
                                    await App.ShowAlertBox(App.AppName, msg);
                                }
                            }
                            else
                            {
                                await App.ShowAlertBox(App.AppName, parsed["message"]?.ToString() ?? string.Empty);
                            }

                        }

                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveContactDetails(string regno, string mobileno, string email,
         string district, string areatype, string tehsil, string village,
         string PO, string street, string pincode, string alterphone, string permntaddress, string corresaddress,
         string issuedate, string certificateno, string _file)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        Regno = AesCryptography.Encrypt(regno),
                        txtmobile = AesCryptography.Encrypt(mobileno),
                        txtemail = AesCryptography.Encrypt(email),
                        DistrictCode = AesCryptography.Encrypt(district),
                        Areatype = AesCryptography.Encrypt(areatype),
                        ddltehsilcode = AesCryptography.Encrypt(tehsil),
                        ddlvillage = AesCryptography.Encrypt(village),
                        txtpo = PO != null ? AesCryptography.Encrypt(PO) : "",
                        txtstreet = street != null ? AesCryptography.Encrypt(street) : "",
                        txtpincode = AesCryptography.Encrypt(pincode),
                        txtphone = alterphone != null ? AesCryptography.Encrypt(alterphone) : "",
                        addressmain = AesCryptography.Encrypt(permntaddress),
                        txtaddressco = AesCryptography.Encrypt(corresaddress),
                        IssueDt = issuedate != null ? AesCryptography.Encrypt(issuedate) : "",
                        DocCertificateNo = certificateno != null ? AesCryptography.Encrypt(certificateno) : "",
                        document = _file != null ? _file : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"ContactInfoInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;

                            msg = message?["message"]?.ToString() ?? string.Empty;

                            await App.ShowAlertBox(App.AppName, msg);

                        }

                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SavEducationDetails(string regno, string QualCd, string board,
         string tmarks, string omarks, string perMarks, string year, string regisdt, string issuedt, string certificateno, string _file)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (string.IsNullOrEmpty(issuedt))
                {
                    issuedt = "";
                    certificateno = "";
                    _file = "";

                }

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegistrationNo = AesCryptography.Encrypt(regno),
                        QualCd = AesCryptography.Encrypt(QualCd),
                        ddlboard = AesCryptography.Encrypt(board),
                        year = AesCryptography.Encrypt(year),
                        omarks = AesCryptography.Encrypt(omarks),
                        tmarks = AesCryptography.Encrypt(tmarks),
                        perMarks = AesCryptography.Encrypt(perMarks),
                        issuedt = issuedt != null ? AesCryptography.Encrypt(issuedt) : "",
                        validuptodt = AesCryptography.Encrypt(""),
                        DocCertNo = certificateno != null ? AesCryptography.Encrypt(certificateno) : "",
                        RegDt = AesCryptography.Encrypt(regisdt),
                        document = _file != null ? _file : "",

                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"EducationInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;

                            msg = message?["message"]?.ToString() ?? string.Empty;
                            if (status == 200)
                            {
                                //get qualificationdetails
                                await App.ShowAlertBox(App.AppName, msg);
                            }
                            else
                            {
                                await App.ShowAlertBox(App.AppName, msg);
                            }
                        }

                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveMiscaleneousDetails(string regno, string EYESIGHT, string HEIGHT, string WEIGHT,
          string CHESTNORMAL, string CHESTEXPANDED, string SALARYINHOMEDISTRICT, string SALARYINHP, string SALARYOUTSIDEHP, string SectorOfInterestId)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegistrationNo = AesCryptography.Encrypt(regno),
                        Eyesight = EYESIGHT != null ? AesCryptography.Encrypt(EYESIGHT) : AesCryptography.Encrypt("0.00"),
                        Height = HEIGHT != null ? AesCryptography.Encrypt(HEIGHT) : AesCryptography.Encrypt("0.00"),
                        Weight = WEIGHT != null ? AesCryptography.Encrypt(WEIGHT) : AesCryptography.Encrypt("0.00"),
                        ChestNormal = CHESTNORMAL != null ? AesCryptography.Encrypt(CHESTNORMAL) : AesCryptography.Encrypt("0.00"),
                        ChastExpended = CHESTEXPANDED != null ? AesCryptography.Encrypt(CHESTEXPANDED) : AesCryptography.Encrypt("0.00"),
                        SalaryHomeDist = SALARYINHOMEDISTRICT != null ? AesCryptography.Encrypt(SALARYINHOMEDISTRICT) : AesCryptography.Encrypt("0.00"),
                        SalaryInHP = SALARYINHP != null ? AesCryptography.Encrypt(SALARYINHP) : AesCryptography.Encrypt("0.00"),
                        SalaryOutHP = SALARYOUTSIDEHP != null ? AesCryptography.Encrypt(SALARYOUTSIDEHP) : AesCryptography.Encrypt("0.00"),
                        Sector = SectorOfInterestId != null ? AesCryptography.Encrypt(SectorOfInterestId) : AesCryptography.Encrypt("0"),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"MiscellaneousInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message1"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;

                            msg = message?["message"]?.ToString() ?? string.Empty;
                            if (status == 200)
                            {
                                //get qualificationdetails
                                await App.ShowAlertBox(App.AppName, msg);
                            }
                            else
                            {
                                await App.ShowAlertBox(App.AppName, msg);
                            }
                        }

                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveMiscaleneousLanguages(string regno, string Langcd, string Read, string Write, string Speak)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegistrationNo = AesCryptography.Encrypt(regno),
                        Langcd = AesCryptography.Encrypt(Langcd),
                        Read = AesCryptography.Encrypt(Read),
                        Write = AesCryptography.Encrypt(Write),
                        Speak = AesCryptography.Encrypt(Speak),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"AddLanguage", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message1"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveEmploymentStatus(string regno, string EmploymentStatusCode, string EmploymentSectorCode, string EmploymenttypeCode, string organisationid,
           string OrganizationName, string RegisteredOrganisationName)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = AesCryptography.Encrypt(regno),
                        EmploymentStatus = EmploymentStatusCode != null ? AesCryptography.Encrypt(EmploymentStatusCode) : "",
                        EmploymentSector = EmploymentSectorCode != null ? AesCryptography.Encrypt(EmploymentSectorCode) : "",
                        EmploymentType = EmploymenttypeCode != null ? AesCryptography.Encrypt(EmploymenttypeCode) : "",
                        RegisteredOrganisationID = organisationid != null ? AesCryptography.Encrypt(organisationid) : "",
                        OrganisationName = OrganizationName != null ? AesCryptography.Encrypt(OrganizationName) : "",
                        RegisteredOrganisationName = RegisteredOrganisationName != null ? AesCryptography.Encrypt(RegisteredOrganisationName) : "",
                    };


                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"EmploymentStausInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveSubCategory(string regno, string UserID, string SubCategory, string issuedt, string validuptodt, string DocCertNo, string _file)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                try
                {
                    var jsonData = new
                    {
                        RegNo = AesCryptography.Encrypt(regno),
                        UserID = UserID != null ? AesCryptography.Encrypt(UserID) : "",
                        SubCategory = SubCategory != null ? AesCryptography.Encrypt(SubCategory) : "",
                        issuedt = issuedt != null ? AesCryptography.Encrypt(issuedt) : "",
                        validuptodt = validuptodt != null ? AesCryptography.Encrypt(validuptodt) : "",
                        DocCertNo = DocCertNo != null ? AesCryptography.Encrypt(DocCertNo) : "",
                        IPAddress = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",
                        document = _file != null ? _file : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"SubCategoryInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SavePhysicallyHandicappedDetails(string regno, string PHType, string Percentage, string Remarks, string PhRegDate,
            string issuedt, string validuptodt, string DocCertNo, string _file)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                try
                {
                    var jsonData = new
                    {
                        RegNo = AesCryptography.Encrypt(regno),
                        UserID = UserID != null ? AesCryptography.Encrypt(UserID) : "",
                        PHType = PHType != null ? AesCryptography.Encrypt(PHType) : "",
                        Percentage = Percentage != null ? AesCryptography.Encrypt(Percentage) : "",
                        Remarks = Remarks != null ? AesCryptography.Encrypt(Remarks) : "",
                        PhRegDate = PhRegDate != null ? AesCryptography.Encrypt(PhRegDate) : "",
                        issuedt = issuedt != null ? AesCryptography.Encrypt(issuedt) : "",
                        validuptodt = validuptodt != null ? AesCryptography.Encrypt(validuptodt) : "",
                        DocCertNo = DocCertNo != null ? AesCryptography.Encrypt(DocCertNo) : "",
                        document = _file != null ? _file : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"PHInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveExServicemenDetails(string regno, string ForceCd, string RankCd, string EnrollDate, string DischargeDate,
           string RegimentName, string ServiceNo, string MedicalCatCd, string CharacterCd, string RemarkS, string ReasonCd, string issuedt, string validuptodt, string certno, string _file)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                try
                {
                    var jsonData = new
                    {
                        RegNo = AesCryptography.Encrypt(regno),
                        ForceCd = ForceCd != null ? AesCryptography.Encrypt(ForceCd) : "",
                        RankCd = RankCd != null ? AesCryptography.Encrypt(RankCd) : "",
                        EnrollDate = EnrollDate != null ? AesCryptography.Encrypt(EnrollDate) : "",
                        DischargeDate = DischargeDate != null ? AesCryptography.Encrypt(DischargeDate) : "",
                        RegimentName = RegimentName != null ? AesCryptography.Encrypt(RegimentName) : "",
                        ServiceNo = ServiceNo != null ? AesCryptography.Encrypt(ServiceNo) : "",
                        MedicalCatCd = MedicalCatCd != null ? AesCryptography.Encrypt(MedicalCatCd) : "",
                        CharacterCd = CharacterCd != null ? AesCryptography.Encrypt(CharacterCd) : "",
                        RemarkS = RemarkS != null ? AesCryptography.Encrypt(RemarkS) : "",
                        ReasonCd = ReasonCd != null ? AesCryptography.Encrypt(ReasonCd) : "",
                        issuedt = issuedt != null ? AesCryptography.Encrypt(issuedt) : "",
                        validuptodt = validuptodt != null ? AesCryptography.Encrypt(validuptodt) : "",
                        certno = certno != null ? AesCryptography.Encrypt(certno) : "",
                        document = _file != null ? _file : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"EXServicemenInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> SaveNCO(string regno, string NCOCd, string Period, string RegDt)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }
                string Remark = string.Empty;
                string SrNo = string.Empty;

                try
                {
                    var jsonData = new
                    {
                        RegNo = AesCryptography.Encrypt(regno),
                        NCOCd = NCOCd != null ? AesCryptography.Encrypt(NCOCd) : "",
                        Period = Period != null ? AesCryptography.Encrypt(Period) : "",
                        RegDt = RegDt != null ? AesCryptography.Encrypt(RegDt) : "",
                        Remark = Remark != null ? AesCryptography.Encrypt(Remark) : "",
                        SrNo = SrNo != null ? AesCryptography.Encrypt(SrNo) : "",
                        UserCd = UserID != null ? AesCryptography.Encrypt(UserID) : "",

                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"NCOInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> Signup(string MobileNo, string Email, string Password, string ConfirmPassword)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }
                try
                {
                    var jsonData = new
                    {
                        MobileNo = MobileNo != null ? AesCryptography.Encrypt(MobileNo) : "",
                        Email = Email != null ? AesCryptography.Encrypt(Email) : "",
                        Ipaddress = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",
                        Password = AesCryptography.GetSha256FromString(Password),
                        ConfirmPassword = AesCryptography.GetSha256FromString(ConfirmPassword),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"Signup", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

       

        public async Task<int> ResendOTP(string mobileno)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;

                try
                {
                    var jsonData = new
                    {
                        MobileNo = AesCryptography.Encrypt(mobileno),

                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"OtpResend", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;

                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;


                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> ReSendActivationLink(string Email)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;

                try
                {
                    var jsonData = new
                    {
                        Email = AesCryptography.Encrypt(Email),

                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"ReSendActivationLink", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;

                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;


                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> ActivateOTP(string userid, string otp)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;

                try
                {
                    var jsonData = new
                    {
                        Userid = AesCryptography.Encrypt(userid),
                        Otp = AesCryptography.Encrypt(otp),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"ActivateOTP", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;

                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;


                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> GetApplicantDashboardData(string UserID)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetApplicantDashboardData?"
                + $"UserID={HttpUtility.UrlEncode(UserID)}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    applicantDashboardDatabase.DeleteApplicantDashboard();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "DashboardList")
                        {
                            var nodes = pair.Value;
                            var item = new ApplicantDashboard();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.UserImage = node["DocFileLink"]?.ToString() ?? string.Empty;
                                    item.EmailID = node["EmailID"]?.ToString() ?? string.Empty;
                                    item.Exchange = node["Exchange"]?.ToString() ?? string.Empty;
                                    item.MobileNo = node["MobileNo"]?.ToString() ?? string.Empty;
                                    item.Name = node["Name"]?.ToString() ?? string.Empty;
                                    item.RegistrationNo = node["RegistrationNo"]?.ToString() ?? string.Empty;
                                    item.userStatus = node["Stat"]?.ToString() ?? string.Empty;
                                    item.Submissiondate = node["date"]?.ToString() ?? string.Empty;
                                    item.ValidUptodate = node["renewaldate"]?.ToString() ?? string.Empty;
                                    applicantDashboardDatabase.AddApplicantDashboard(item);
                                }
                            }

                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> GetExistUserData(string DOB, string FHNameDesc, string UserNM)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetExistUserData?"
                + $"DOB={HttpUtility.UrlEncode(AesCryptography.Encrypt(DOB))}"
                + $"&FHNameDesc={HttpUtility.UrlEncode(AesCryptography.Encrypt(FHNameDesc))}"
                + $"&UserNM={HttpUtility.UrlEncode(AesCryptography.Encrypt(UserNM))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    existingUserDataDatabase.DeleteExistingUserData();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "GetExistUserList")
                        {
                            var nodes = pair.Value;
                            var item = new ExistingUserData();
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    item.Address = SafeDecrypt(node, "Address");
                                    item.DOB = SafeDecrypt(node, "DOB");
                                    item.DistrictCode = SafeDecrypt(node, "DistrictCode");
                                    item.Exchange = SafeDecrypt(node, "Exchange");
                                    item.FAtherHusbandSelection = SafeDecrypt(node, "FAtherHusbandSelection");
                                    item.RegistrationNo = SafeDecrypt(node, "RegistrationNo");
                                    item.XchangeCode = SafeDecrypt(node, "XchangeCode");
                                    item.UserName = SafeDecrypt(node, "UserName");


                                    existingUserDataDatabase.AddExistingUserData(item);
                                }
                            }

                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {
                //await App.ShowAlertBox("Exception", ey.Message);
                return 500;
            }
        }

        public async Task<int> SaveUserMapping(string XchCd, string DOB, string RegNo, string UserID)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    string ipaddress = await GetIPAddress();
                    if (string.IsNullOrEmpty(ipaddress))
                    {
                        ipaddress = "192.168.1.1";
                    }

                    var jsonData = new
                    {
                        XchCd = AesCryptography.Encrypt(XchCd),
                        DOB = AesCryptography.Encrypt(DOB),
                        RegNo = AesCryptography.Encrypt(RegNo),
                        UserID = AesCryptography.Encrypt(UserID),
                        IPAddress = AesCryptography.Encrypt(ipaddress),
                    };

                    string json = JsonConvert.SerializeObject(jsonData);
                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"MapRegistration", content);

                      var result = await response.Content.ReadAsStringAsync();
                        var parsed = JObject.Parse(result);
                    if ((int)response.StatusCode == 200)
                    {
                     

                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"]?.ToObject<JObject>();

                            if (message != null && message.HasValues)
                            {
                                status = int.TryParse(message["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;

                                msg = message["message"]?.ToString() ?? string.Empty;

                                await App.ShowAlertBox(App.AppName, msg);
                            }
                            else
                            {
                                await App.ShowAlertBox(App.AppName, parsed["message"]?.ToString() ?? string.Empty);
                            }
                        }


                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> GetAlreadyRegisteredUserData(string DOB, string ExchangeID, string REGNO, string Districtcd, string UserID)
        {
            int status = 0;
            try
            {
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                var client = new HttpClient();
                string parameters = baseurluserregapi + $"AlreadyRegisteredUserData?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(REGNO))}"
                + $"&DOB={HttpUtility.UrlEncode(AesCryptography.Encrypt(DOB))}"
                + $"&XchCd={HttpUtility.UrlEncode(AesCryptography.Encrypt(ExchangeID))}"
                + $"&UserID={HttpUtility.UrlEncode(AesCryptography.Encrypt(UserID))}"
                + $"&ipaddress={HttpUtility.UrlEncode(AesCryptography.Encrypt(ipaddress))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    if (parsed.HasValues)
                    {
                        string msg = string.Empty;
                        var message = parsed["message"];
                        status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;

                        if (status != 200)
                        {
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                        else
                        {
                            foreach (var pair in parsed)
                            {
                                if (pair.Key == "GetAlreadyregisteredList")
                                {
                                    alreadyRegisteredDatabase.DeleteAlreadyRegistered();
                                    var nodes = pair.Value;
                                    if (nodes != null)
                                    {
                                        var item = new AlreadyRegistered();
                                        foreach (var node in nodes)
                                        {
                                            item.RegistrationNo = REGNO;
                                            item.DOB = DOB;
                                            item.XchangeCode = ExchangeID;
                                            item.RegName = SafeDecrypt(node, "RegName");
                                            item.F_HNameCd = SafeDecrypt(node, "F_HNameCd");
                                            item.F_HNameDesc = SafeDecrypt(node, "F_HNameDesc");
                                            item.RegDt = SafeDecrypt(node, "RegDt");
                                            item.RenewalDt = SafeDecrypt(node, "RenewalDt");
                                            item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                            item.Email = SafeDecrypt(node, "Email");
                                            item.MobileNo = SafeDecrypt(node, "MobileNo");
                                            item.ActivationStatus = SafeDecrypt(node, "ActivationStatus");

                                            alreadyRegisteredDatabase.AddAlreadyRegistered(item);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

                return status;
            }
            catch (Exception ex)
            {
#if DEBUG
                await App.ShowAlertBox("Exception", ex.Message);
                return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif

            }
        }

        //get saved user data details
        public async Task<int> GetPersonalDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetPersonalDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    personalDetailsDatabase.DeletePersonalDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "PersonalDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new PersonalDetails();
                                foreach (var node in nodes)
                                {
                                    item.DistrictCode = SafeDecrypt(node, "DistrictCode");
                                    item.XchangeCode = SafeDecrypt(node, "XchangeCode");
                                    item.UserName = SafeDecrypt(node, "UserName");
                                    item.FAtherHusbandSelection = SafeDecrypt(node, "FAtherHusbandSelection");
                                    item.TXTFHNAME = SafeDecrypt(node, "TXTFHNAME");
                                    item.txtmother = SafeDecrypt(node, "txtmother");
                                    item.GENDER = SafeDecrypt(node, "GENDER");
                                    item.ddlmarital = SafeDecrypt(node, "ddlmarital");
                                    item.DOB = SafeDecrypt(node, "DOB");
                                    item.Category = SafeDecrypt(node, "Category");
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.RegistrationNo = SafeDecrypt(node, "RegistrationNo");
                                    item.issuedt = SafeDecrypt(node, "issuedt");
                                    item.DocCertNo = SafeDecrypt(node, "DocCertNo");
                                    item.ddlreligion = SafeDecrypt(node, "ddlreligion");
                                    item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                    item.RejectionRemarks = SafeDecrypt(node, "RejectionRemarks");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;
                                    item.RegDate = SafeDecrypt(node, "RegDate");
                                    item.RenewalMonth = SafeDecrypt(node, "RenewalMonth");
                                    item.RenewalDate = SafeDecrypt(node, "RenewalDate");

                                    personalDetailsDatabase.AddPersonalDetails(item);
                                }
                                return 200;
                            }
                            else
                            {
                                return 300;
                            }


                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetContactDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetContactDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    contactDetailsDatabase.DeleteContactDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "ContactDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new ContactDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.ddltehsilcode = SafeDecrypt(node, "ddltehsilcode");
                                    item.ddlvillage = SafeDecrypt(node, "ddlvillage");
                                    item.txtpo = SafeDecrypt(node, "txtpo");
                                    item.txtstreet = SafeDecrypt(node, "txtstreet");
                                    item.txtpincode = SafeDecrypt(node, "txtpincode");
                                    item.addressmain = SafeDecrypt(node, "addressmain");
                                    item.txtaddressco = SafeDecrypt(node, "txtaddressco");
                                    item.txtmobile = SafeDecrypt(node, "txtmobile");
                                    item.txtphone = SafeDecrypt(node, "txtphone");
                                    item.txtemail = SafeDecrypt(node, "txtemail");
                                    item.DocCertificateNo = SafeDecrypt(node, "DocCertificateNo");
                                    item.IssueDt = SafeDecrypt(node, "IssueDt");
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.DistrictCode = SafeDecrypt(node, "DistrictCode");
                                    item.Areatype = SafeDecrypt(node, "Areatype");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;
                                    item.sameasabove = SafeDecrypt(node, "sameasabove");
                                    item.DistrictCode = SafeDecrypt(node, "DistrictCode");                             
                                    contactDetailsDatabase.AddContactDetails(item);
                                }
                                return 200;
                            }
                            else
                            {
                                return 300;

                            }


                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetEducationDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetEducationDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    qualficationDetailsDatabase.DeleteQualficationDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "EducationDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new QualficationDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.QualNm = SafeDecrypt(node, "QualNm");
                                    item.ddlboard = SafeDecrypt(node, "ddlboard");
                                    item.omarks = SafeDecrypt(node, "omarks");
                                    item.tmarks = SafeDecrypt(node, "tmarks");
                                    item.perMarks = SafeDecrypt(node, "perMarks");
                                    item.year = SafeDecrypt(node, "year");
                                    item.issuedt = SafeDecrypt(node, "issuedt");
                                    item.validuptodt = SafeDecrypt(node, "validuptodt");
                                    item.DocCertNo = SafeDecrypt(node, "DocCertNo");
                                    item.RegDt = SafeDecrypt(node, "RegDt");
                                    item.QualCd = SafeDecrypt(node, "QualCd");
                                    item.VerifyStatus = SafeDecrypt(node, "VerifyStatus");
                                    item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                    item.VerifiedDt = SafeDecrypt(node, "VerifiedDt");
                                    item.Remarks = SafeDecrypt(node, "Remarks");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");                                 
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;


                                    qualficationDetailsDatabase.AddQualficationDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }



                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetMiscelleaneousDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetMiscelleaneousDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    miscellaneousDetailsDatabase.DeleteMiscellaneousDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "MiscelleaneousDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new MiscellaneousDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.EyeSight = SafeDecrypt(node, "EyeSight");
                                    item.Height = SafeDecrypt(node, "Height");
                                    item.Weight = SafeDecrypt(node, "Weight");
                                    item.ChestNormal = SafeDecrypt(node, "ChestNormal");
                                    item.ChastExpended = SafeDecrypt(node, "ChastExpended");
                                    item.SalaryHomeDist = SafeDecrypt(node, "SalaryHomeDist");
                                    item.SalaryInHP = SafeDecrypt(node, "SalaryInHP");
                                    item.SalaryOutHP = SafeDecrypt(node, "SalaryOutHP");
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.Sector = SafeDecrypt(node, "Sector");

                                    miscellaneousDetailsDatabase.AddMiscellaneousDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }



                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetLangDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetLangDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    getLangDetailsDatabase.DeleteGetLangDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "LangDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new GetLangDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.Langcd = SafeDecrypt(node, "Langcd");
                                    item.LangName = SafeDecrypt(node, "LangName");
                                    item.Read = SafeDecrypt(node, "Read");
                                    item.Write = SafeDecrypt(node, "Write");
                                    item.Speak = SafeDecrypt(node, "Speak");

                                    getLangDetailsDatabase.AddGetLangDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }



                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetSubcategoryDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetSubcategoryDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    subCategoryDetailsDatabase.DeleteSubCategoryDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "SubcategoryDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new SubCategoryDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.SubCategory = SafeDecrypt(node, "SubCategory");
                                    item.SubCategoryNm = SafeDecrypt(node, "SubCategoryNm");
                                    item.issuedt = SafeDecrypt(node, "issuedt");
                                    item.validuptodt = SafeDecrypt(node, "validuptodt");
                                    item.DocCertNo = SafeDecrypt(node, "DocCertNo");
                                    item.VerifyStatus = SafeDecrypt(node, "VerifyStatus");
                                    item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                    item.VerifiedDt = SafeDecrypt(node, "VerifiedDt");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;
                                    subCategoryDetailsDatabase.AddSubCategoryDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetEmployedDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetEmployedDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    employedDetailsDatabase.DeleteEmployedDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "EmployedDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new EmployedDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.EmploymentStatus = SafeDecrypt(node, "EmploymentStatus");
                                    item.EmploymentSector = SafeDecrypt(node, "EmploymentSector");
                                    item.EmploymentType = SafeDecrypt(node, "EmploymentType");
                                    item.OrganisationName = SafeDecrypt(node, "OrganisationName");
                                    item.RegisteredOrganisationName = SafeDecrypt(node, "RegisteredOrganisationName");
                                    item.RegisteredOrganisationID = SafeDecrypt(node, "RegisteredOrganisationID");
                                    item.Stat = SafeDecrypt(node, "Stat");

                                    employedDetailsDatabase.AddEmployedDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetPHDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetPHDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    pHDetailsDatabase.DeletePHDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "PHDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new PHDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.PHType = SafeDecrypt(node, "PHType");
                                    item.PHTypeNm = SafeDecrypt(node, "PHTypeNm");
                                    item.Percentage = SafeDecrypt(node, "Percentage");
                                    item.Remarks = SafeDecrypt(node, "Remarks");
                                    item.PhRegDate = SafeDecrypt(node, "PhRegDate");
                                    item.issuedt = SafeDecrypt(node, "issuedt");
                                    item.validuptodt = SafeDecrypt(node, "validuptodt");
                                    item.DocCertNo = SafeDecrypt(node, "DocCertNo");
                                    item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                    item.VerifiedDt = SafeDecrypt(node, "VerifiedDt");
                                    item.VerifyStatus = SafeDecrypt(node, "VerifyStatus");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");                                 
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;

                                    pHDetailsDatabase.AddPHDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetXservicemenDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetXservicemenDetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    xservicemenDetailsDatabase.DeleteXservicemenDetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "XservicemenDetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new XservicemenDetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.ddlforce = SafeDecrypt(node, "ddlforce");
                                    item.ddlrank = SafeDecrypt(node, "ddlrank");
                                    item.txtservicenumber = SafeDecrypt(node, "txtservicenumber");
                                    item.txtregimentname = SafeDecrypt(node, "txtregimentname");
                                    item.ddlmedical = SafeDecrypt(node, "ddlmedical");
                                    item.ddlcharacter = SafeDecrypt(node, "ddlcharacter");
                                    item.txtdischargedate = SafeDecrypt(node, "txtdischargedate");
                                    item.txtenrolmentdate = SafeDecrypt(node, "txtenrolmentdate");
                                    item.ddlreason = SafeDecrypt(node, "ddlreason");
                                    item.txtremarks = SafeDecrypt(node, "txtremarks");
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.ValidUptoDt = SafeDecrypt(node, "ValidUptoDt");
                                    item.IssueDt = SafeDecrypt(node, "IssueDt");
                                    item.DocCertificateNo = SafeDecrypt(node, "DocCertificateNo");
                                    item.DocFileNm = SafeDecrypt(node, "DocFileNm");                                    
                                    item.DocFileLink = node["DocFileLink"]?.ToString() ?? string.Empty;

                                    xservicemenDetailsDatabase.AddXservicemenDetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetNCODetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetNCODetails?"
                + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    getNCODetailsDatabase.DeleteGetNCODetails();

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "NCODetailsList")
                        {
                            var nodes = pair.Value;
                            if (nodes!=null)
                            {
                                var item = new GetNCODetails();
                                foreach (var node in nodes)
                                {
                                    item.RegistrationNo = RegNo;
                                    item.Stat = SafeDecrypt(node, "Stat");
                                    item.NCOCd = SafeDecrypt(node, "NCOCd");
                                    item.NCONm = SafeDecrypt(node, "NCONm");
                                    item.SrNo = SafeDecrypt(node, "SrNo");
                                    item.StatusDesc = SafeDecrypt(node, "StatusDesc");
                                    item.VerifiedDt = SafeDecrypt(node, "VerifiedDt");
                                    item.VerifyStatus = SafeDecrypt(node, "VerifyStatus");
                                    item.experience = SafeDecrypt(node, "experience");
                                    item.regdt = SafeDecrypt(node, "regdt");
                                    item.remarks = SafeDecrypt(node, "remarks");


                                    getNCODetailsDatabase.AddGetNCODetails(item);
                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> GetRegDetailsLabels(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string parameters = baseurluserregapi + $"GetRegFormsData?"
                + $"RegistrationNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}";

                HttpResponseMessage response = await client.GetAsync(parameters);
                if ((int)response.StatusCode == 200)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    foreach (var pair in parsed)
                    {
                        if (pair.Key == "SubmittedForms")
                        {
                            var nodes = pair.Value;
                            if (nodes != null)
                            {
                                submittedFormsDatabase.DeleteSubmittedFormsDetails();
                                var item = new SubmittedFormsDetails();
                                foreach (var node in nodes)
                                {
                                  
                                    {
                                        item.RegistrationNo = RegNo;
                                        item.ContactDetailsYN = SafeDecrypt(node, "ContactDetails");
                                        item.PersonalDetailsYN = SafeDecrypt(node, "PersonalDetails");
                                        item.PersonalDetails = SafeDecrypt(node, "PersonalDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.ContactDetails = SafeDecrypt(node, "ContactDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.EducationDetails = SafeDecrypt(node, "EducationDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.EmployedDetails = SafeDecrypt(node, "EmployedDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.ExDetails = SafeDecrypt(node, "ExDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.NCODetails = SafeDecrypt(node, "NCODetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.MiscDetails = SafeDecrypt(node, "OtherDetails") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.PH = SafeDecrypt(node, "PH") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.SubCat = SafeDecrypt(node, "SubCat") == "N" ? "#c0c0c0" : "#FF006400";
                                        item.ActiveForm = SafeDecrypt(node, "ActiveForm");
                                        item.Stat = SafeDecrypt(node, "Stat");
                                    };

                                    submittedFormsDatabase.AddSubmittedFormsDetails(item);

                                }
                                return 200;

                            }
                            else { return 300; }
                        }
                    }
                }
                return (int)response.StatusCode;
            }
            catch
            {

                return 500;
            }
        }

        public async Task<int> FinalSubmit(string RegNo)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    string ipaddress = await GetIPAddress();
                    if (string.IsNullOrEmpty(ipaddress))
                    {
                        ipaddress = "192.168.1.1";
                    }
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        IPAddress = AesCryptography.Encrypt(ipaddress),
                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"SubmitRegistration", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> TakeLive(string RegNo, string MappedByUser, string XchangeCode)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    string ipaddress = await GetIPAddress();
                    if (string.IsNullOrEmpty(ipaddress))
                    {
                        ipaddress = "192.168.1.1";
                    }
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        MappedByUser = MappedByUser != null ? AesCryptography.Encrypt(MappedByUser) : "",
                        MappedByIP = AesCryptography.Encrypt(ipaddress),
                        XchangeCode = XchangeCode != null ? AesCryptography.Encrypt(XchangeCode) : "",
                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"TakeLiveReg", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> DelEducationDetails(string RegNo, string QualCd)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        QualCd = QualCd != null ? AesCryptography.Encrypt(QualCd) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"DelEducationDetails", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> DelSubcatDetails(string RegNo, string SubcatCd)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        SubcatCd = SubcatCd != null ? AesCryptography.Encrypt(SubcatCd) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"DelSubcatDetails", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> DelPHDetails(string RegNo, string PHCd)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        PHCd = PHCd != null ? AesCryptography.Encrypt(PHCd) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"DelPHDetails", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> DelNCODetails(string RegNo, string NCONo, string SrNo)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        NCONo = NCONo != null ? AesCryptography.Encrypt(NCONo) : "",
                        SrNo = SrNo != null ? AesCryptography.Encrypt(SrNo) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"DelNCODetails", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> UploadImageInsUpd(string _file, string UserID)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                int status = 0;
                try
                {

                    var jsonData = new
                    {

                        document = _file != null ? _file : "",
                        UserID = UserID != null ? AesCryptography.Encrypt(UserID) : "",
                    };

                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"UploadImageInsUpd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];

                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }

                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }

                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> RenewReg(string RegNo, string UserID)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        RegNo = RegNo != null ? AesCryptography.Encrypt(RegNo) : "",
                        UserID = UserID != null ? AesCryptography.Encrypt(UserID) : "",
                        ModifiedByIP = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"RenewReg", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> ForgotPasslink(string LoginId)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        LoginId = LoginId != null ? AesCryptography.Encrypt(LoginId) : "",
                        IpAddress = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"ForgotPasslink", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            string RecoveryId = message?["RegNo"]?.ToString() ?? string.Empty;
                            Preferences.Set("RecoveryId", RecoveryId);
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> VerifyForgotPassCd(string LoginId, string RecoveryId, string OTP)
        {

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        LoginId = LoginId != null ? AesCryptography.Encrypt(LoginId) : "",
                        RecoveryId = RecoveryId != null ? AesCryptography.Encrypt(RecoveryId) : "",
                        OTP = OTP != null ? AesCryptography.Encrypt(OTP) : "",
                        IpAddress = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"VerifyForgotPassCd", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);
                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public async Task<int> ChangePassword(string LoginId, string RecoveryId, string Password, string ConfirmPassword)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                string ipaddress = await GetIPAddress();
                if (string.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "192.168.1.1";
                }

                int status = 0;
                try
                {
                    var jsonData = new
                    {
                        LoginId = LoginId != null ? AesCryptography.Encrypt(LoginId) : "",
                        RecoveryId = RecoveryId != null ? AesCryptography.Encrypt(RecoveryId) : "",
                        Password = Password != null ? AesCryptography.Encrypt(AesCryptography.GetSha256FromString(Password)) : "",
                        ConfirmPassword = ConfirmPassword != null ? AesCryptography.Encrypt(AesCryptography.GetSha256FromString(ConfirmPassword)) : "",
                        IpAddress = ipaddress != null ? AesCryptography.Encrypt(ipaddress) : "",

                    };
                    string json = JsonConvert.SerializeObject(jsonData);

                    var client = new HttpClient();
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(baseurluserregapi + $"ChangePassword", content);

                    JObject parsed = new JObject();
                    if ((int)response.StatusCode == 200)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {
                            string msg = string.Empty;
                            var message = parsed["message"];
                            status = int.TryParse(message?["status"]?.ToString(), out var tmpStatus) ? tmpStatus : 0;
                            msg = message?["message"]?.ToString() ?? string.Empty;
                            await App.ShowAlertBox(App.AppName, msg);
                        }
                    }
                    else
                    {
                        await App.ShowAlertBox(App.AppName, parsed["Message"]?.ToString() ?? string.Empty);

                    }
                    return status;

                }
                catch (Exception ex)
                {
#if DEBUG
                    await App.ShowAlertBox("Exception", ex.Message);
                    return 500;
#else
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                    return 500;
#endif
                }
            }
            else
            {
                await App.ShowAlertBox(App.AppName, App.NoInternet_);
                return 101;
            }
        }

        public void getpdf(string RegNo, string filefor, string fileforcode)
        {
            string url1 = pdfurluserregapi
            + $"RegNo={HttpUtility.UrlEncode(AesCryptography.Encrypt(RegNo))}"
            + $"&filefor={HttpUtility.UrlEncode(AesCryptography.Encrypt(filefor))}"
            + $"&fileforcode={HttpUtility.UrlEncode(AesCryptography.Encrypt(fileforcode))}";
            Launcher.OpenAsync(url1);
        }

        public string getusername(string RegNo)
        {
            string username = string.Empty;
            string query = $"Select * from PersonalDetails where RegistrationNo= '{RegNo}'";
            var m = personalDetailsDatabase.GetPersonalDetails(query).ToList();
            if (m.Count > 0)
            {
                username = m.ElementAt(0).UserName ?? "";
            }

            return username;
        }

        public async Task<string> GetIPAddress()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    var response = await client.GetAsync("https://api.myip.com");
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    var m = parsed["ip"];
                    return m?.ToString() ?? string.Empty;

                }
                catch
                {
                    var ip = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();
                    return ip?.ToString() ?? string.Empty;
                }
            }
            else
            {
                var ip = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();
                return ip?.ToString() ?? string.Empty;
            }
        }

    }
}
