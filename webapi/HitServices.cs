using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using X10Card.Models;

namespace X10Card.webapi
{
    public class HitServices
    {

        public string access_key = "1E0762F5768C727AC462C80967AA80F4";
        public string access_id = "NICHPSC";
        public string AppName = "X10 Card";
        public string close = "Close";
        public string nointernet = "No Internet Connection Found!";
        //public string privacypolicyurl = "https://himachal.nic.in/mobile-app-privacy-policy/X10.html";
        public string privacypolicyurl = "https://mobileappshp.nic.in/assets/pdf/mobile-app-privacy-policy/X10.html";
        public string GetAppVersionDetailsUrl = "https://mobileappshp.nic.in/MyDiary/MobileAppVersions.svc/GetAppVersion?";

        public string baseurlx10api = "http://10.146.2.67/eemis/X10Service.svc/";

        /* public string baseurlx10api = "https://eemis.hp.nic.in/X10Service.svc/";*/

        public static UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
        public static List<UserDetails> userDetailslist = new List<UserDetails>();
        public static NCODetailsDatabase nCODetailsDatabase = new NCODetailsDatabase();
        public static QualificationDetailsDatabase qualificationDetailsDatabase = new QualificationDetailsDatabase();
        public static EmploymentStatusDatabase employmentStatusDatabase = new EmploymentStatusDatabase();
        public static SubEmploymentStatusDatabase subEmploymentStatusDatabase = new SubEmploymentStatusDatabase();
        public static AllowanceDetailsDatabase allowanceDetailsDatabase = new AllowanceDetailsDatabase();
        public static AllowanceTransactionsDatabase allowanceTransactionsDatabase = new AllowanceTransactionsDatabase();
        public static AllVacancyDetailsDatabase allVacancyDetailsDatabase = new AllVacancyDetailsDatabase();
        public static SponsorshipDetailsDatabase sponsorshipDetailsDatabase = new SponsorshipDetailsDatabase();
        public static JobFairDetailsDatabase jobFairDetailsDatabase = new JobFairDetailsDatabase();
        public static JobfairsEmployersDatabase jobfairsEmployersDatabase = new JobfairsEmployersDatabase();
        public static JobFairEmployersVacanciesDatabase jobFairEmployersVacanciesDatabase = new JobFairEmployersVacanciesDatabase();

        private static string SafeDecrypt(JToken node, string key)
        {
            return AesCryptography.Decrypt(node?[key]?.ToString() ?? string.Empty);
        }

        public async void update()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    double installedVersionNumber = double.Parse(VersionTracking.CurrentVersion);


                    var client = new HttpClient();
                    string CurrentPlateform = "A";

                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        CurrentPlateform = "I";
                    }
                    if (DeviceInfo.Platform == DevicePlatform.macOS)
                    {
                        CurrentPlateform = "I";
                    }
                    if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                    {
                        CurrentPlateform = "I";
                    }
                    if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        CurrentPlateform = "U";
                    }
                    else
                    {
                        CurrentPlateform = "A";
                    }

                    var responce = await client.GetAsync(GetAppVersionDetailsUrl + $"&Platform={CurrentPlateform}&packageid={AppInfo.PackageName}");
                    var MyJson = await responce.Content.ReadAsStringAsync();

                    JObject parsed = JObject.Parse(MyJson);
                    var ServiceStatusCode = parsed?["message"]?["status"]?.ToString();
                    if (ServiceStatusCode == "200")
                    {
                        if (MyJson.Contains("Mandatory"))
                        {
                            double latestVersionNumber = double.Parse(parsed?["appVersionDetails"]?[0]?["VersionNumber"]?.ToString() ?? "1.0");
                            string isMandatory = parsed?["appVersionDetails"]?[0]?["Mandatory"]?.ToString() ?? "N";
                            string whatsNew = parsed?["appVersionDetails"]?[0]?["WhatsNew"]?.ToString() ?? "";
                            string url = parsed?["appVersionDetails"]?[0]?["Url"]?.ToString() ?? "https://play.google.com/";

                            if (installedVersionNumber < latestVersionNumber)
                            {
                                if (isMandatory == "Y")
                                {
                                    await App.ShowUpdateAppAlertBox("New Version", $"There is a new version (v{latestVersionNumber}) of this app available.\nWhatsNew: {whatsNew}", "Update");
                                    await Launcher.OpenAsync(url);
                                    return;
                                }
                                else
                                {

                                    //var updat = await Application.App.ShowAlertBox("New Version", $"There is a new version (v{latestVersionNumber}) of this app available.\nWhatsNew: {whatsNew}\nWould you like to update now?", "Yes", "No");                                  
                                    if (Application.Current != null && Application.Current.Windows.Any())
                                    {
                                        var currentPage = Application.Current.Windows[0].Page; // Get the root page of the first window
                                        if (currentPage != null)
                                        {
                                            var updat = await currentPage.DisplayAlert("New Version", $"There is a new version (v{latestVersionNumber}) of this app available.\nWhatsNew: {whatsNew}\nWould you like to update now?", "Yes", "No");
                                            if (updat)
                                            {
                                                await Launcher.OpenAsync(url);
                                            }
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine("Warning: No active page found to display alert.");
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }


        public async Task<int> validatelogin(string regno/*, string password, string dob*/)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        key = AesCryptography.Encrypt(access_key),
                        id = AesCryptography.Encrypt(access_id),
                        RegNo = AesCryptography.Encrypt(regno.Trim()),
                        /* NM = AesCryptography.Encrypt(password.Trim()),
                         DOB = AesCryptography.Encrypt(dob.Trim()),*/
                    });
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //var responce = await client.PostAsync($"{LoginUrl}", content);
                    HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"ValidateApplicant2024", content);
                    if (!responce.IsSuccessStatusCode)
                    {
                        await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                        return 500;
                    }
                    else
                    {
                        var result = await responce.Content.ReadAsStringAsync();
                        JObject parsed = JObject.Parse(result);

                        string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                        string status = parsed["message"]?["status"]?.ToString() ?? string.Empty;
                        if (status.Equals("True"))
                        {
                            foreach (var pair in parsed)
                            {
                                if (pair.Key == "applicantDetails")
                                {
                                    var nodes = pair.Value;
                                    if (nodes != null)
                                    {
                                        userDetailsDatabase.DeleteUserDetails();
                                        foreach (var node in nodes)
                                        {
                                            var item = new UserDetails();
                                            item.CandiName = SafeDecrypt(node, "CandiName");
                                            item.RegNo = SafeDecrypt(node, "RegNo");
                                            item.RegDt = SafeDecrypt(node, "RegDt");
                                            item.XchName = SafeDecrypt(node, "XchName");
                                            item.RenewalDt = SafeDecrypt(node, "RenewalDt");
                                            item.Address = SafeDecrypt(node, "Address");
                                            item.Category = SafeDecrypt(node, "Category");
                                            item.DOB = SafeDecrypt(node, "DOB");
                                            item.MaritalStatus = SafeDecrypt(node, "MaritalStatus");
                                            item.MobileNo = SafeDecrypt(node, "MobileNo");
                                            item.EmpStatus = SafeDecrypt(node, "EmpStatus");
                                            item.eMailId = SafeDecrypt(node, "eMailId");

                                            item.islogin = "Y";
                                            item.lastupdated = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                            userDetailsDatabase.AddUserDetails(item);
                                        }
                                    }
                                }
                                if (pair.Key == "nCODetails")
                                {
                                    var nodes = pair.Value;
                                    if (nodes != null)
                                    {
                                        nCODetailsDatabase.DeleteNCODetails();
                                        foreach (var node in nodes)
                                        {
                                            var item = new NCODetails();
                                            item.NCODesc = SafeDecrypt(node, "NCODesc");
                                            nCODetailsDatabase.AddNCODetails(item);
                                        }
                                    }
                                }
                                if (pair.Key == "qualificationDetails")
                                {
                                    var nodes = pair.Value;

                                    if (nodes != null)
                                    {
                                        qualificationDetailsDatabase.DeleteQualificationDetails();
                                        foreach (var node in nodes)
                                        {
                                            var item = new QualificationDetails();
                                            item.QualDesc = SafeDecrypt(node, "QualDesc");
                                            qualificationDetailsDatabase.AddQualificationDetails(item);
                                        }
                                    }
                                }
                                /*if (pair.Key == "vacancyDetails")
                                {
                                    var nodes = pair.Value;
                                    vacancyDetailsDatabase.DeleteVacancyDetails();
                                    foreach (var node in nodes)
                                    {
                                        var item = new VacancyDetails();
                                        item.InterviewDateTime = AesCryptography.Decrypt(node["InterviewDateTime"].ToString());
                                        item.Post = AesCryptography.Decrypt(node["Post"].ToString());
                                        vacancyDetailsDatabase.AddVacancyDetails(item);
                                    }
                                }*/
                                if (pair.Key == "employmentStatus")
                                {
                                    var nodes = pair.Value;

                                    if (nodes != null)
                                    {
                                        employmentStatusDatabase.DeleteEmploymentStatus();
                                        foreach (var node in nodes)
                                        {
                                            var item = new EmploymentStatus();
                                            item.EmpStatCd = SafeDecrypt(node, "EmpStatCd");
                                            item.EmpStatDesc = SafeDecrypt(node, "EmpStatDesc");

                                            employmentStatusDatabase.AddEmploymentStatus(item);

                                        }
                                    }
                                }
                                if (pair.Key == "subEmploymentStatus")
                                {
                                    var nodes = pair.Value;

                                    if (nodes != null)
                                    {
                                        subEmploymentStatusDatabase.DeleteSubEmploymentStatus();
                                        foreach (var node in nodes)
                                        {
                                            var item = new SubEmploymentStatus();
                                            item.SubEmpStatCd = SafeDecrypt(node, "SubEmpStatCd");
                                            item.SubEmpStatDesc = SafeDecrypt(node, "SubEmpStatDesc");
                                            item.SSubEmpStatCd = SafeDecrypt(node, "SSubEmpStatCd");
                                            item.SSubEmpStatDesc = SafeDecrypt(node, "SSubEmpStatDesc");

                                            subEmploymentStatusDatabase.AddSubEmploymentStatus(item);
                                        }
                                    }
                                }
                            }
                            return 200;
                        }
                        else
                        {
                            await App.ShowAlertBox(AppName, msg);
                            return 300;
                        }
                    }
                }
                catch
                {
                    await App.ShowAlertBox("Exception", "Something went wrong. Pls try after some time");
                    return 400;
                }
            }
            return 500;
        }

        public async Task UpdateEmployementstatus(string RegNo, string EmpStatus, string SubEmpStatus, string SSubEmpStatus)
        {

            userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();

            string Registration_No = userDetailslist.ElementAt(0).RegNo??"";


            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {

                    string jsonData = JsonConvert.SerializeObject(new
                    {
                        RegNo = AesCryptography.Encrypt(RegNo),
                        EmpStatus = AesCryptography.Encrypt(EmpStatus),
                        SubEmpStatus = AesCryptography.Encrypt(SubEmpStatus),
                        SSubEmpStatus = AesCryptography.Encrypt(SSubEmpStatus),

                    });
                    var client = new HttpClient();
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    //var response = await client.PostAsync($"{UpdEmpStatUrl}", content);
                    HttpResponseMessage response = await client.PostAsync(baseurlx10api + $"UpdEmpStat", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        await App.ShowAlertBox(AppName, $"[{response.StatusCode}] Something went wrong\nPlease try again!");
                        return;
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        JObject parsed = JObject.Parse(result);
                        if (parsed.HasValues)
                        {

                            string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                            string status = parsed["message"]?["status"]?.ToString() ?? string.Empty;

                            
                            if (status.Equals("True"))
                            {
                                await validatelogin(Registration_No);
                                await App.ShowAlertBox(AppName, "Employment Status Successfully Updated.");

                            }
                            else
                            {
                                await App.ShowAlertBox(AppName, msg);
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time.");
                }
            }
            else
            {
                await App.ShowAlertBox(AppName, nointernet);
            }
        }

        public async Task<int> GetAllowanceDetails(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    RegNo = AesCryptography.Encrypt(RegNo),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var responce = await client.PostAsync($"{AllowanceDetailsUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetAllowanceDetails", content);

                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {
                        allowanceDetailsDatabase.DeleteAllowanceDetails();
                        allowanceTransactionsDatabase.DeleteAllowanceTransactions();
                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "allowanceDetails")
                            {

                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new AllowanceDetails();
                                        item.AllowanceDesc = SafeDecrypt(node, "AllowanceDesc");
                                        item.AllowanceId = SafeDecrypt(node, "AllowanceId");
                                        item.AmtPaid = SafeDecrypt(node, "AmtPaid");
                                        item.ApplicationNo = SafeDecrypt(node, "ApplicationNo");
                                        item.ApplicationStatus = SafeDecrypt(node, "ApplicationStatus");
                                        item.ApplicationStatusCd = SafeDecrypt(node, "ApplicationStatusCd");
                                        item.EndDt = SafeDecrypt(node, "EndDt");
                                        item.InstallmentAmt = SafeDecrypt(node, "InstallmentAmt");
                                        item.InstallmentsPaid = SafeDecrypt(node, "InstallmentsPaid");
                                        item.LastInstallmentPaidOn = SafeDecrypt(node, "LastInstallmentPaidOn");
                                        item.RegNo = SafeDecrypt(node, "RegNo");
                                        item.StartDt = SafeDecrypt(node, "StartDt");
                                        item.TotInstallments = SafeDecrypt(node, "TotInstallments");
                                        item.XchNm = SafeDecrypt(node, "XchNm");
                                        item.ApplicationStatusTexcolor = SafeDecrypt(node, "ApplicationStatusTexcolor");
                                        allowanceDetailsDatabase.AddAllowanceDetails(item);
                                    }
                                }
                            }

                            if (pair.Key == "allowanceTransactions")
                            {

                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new AllowanceTransactions();
                                        item.AccountNo = SafeDecrypt(node, "AccountNo");
                                        item.AllowanceId = SafeDecrypt(node, "AllowanceId");
                                        item.AllowanceDesc = SafeDecrypt(node, "AllowanceDesc");
                                        item.ApplicationNo = SafeDecrypt(node, "ApplicationNo");
                                        item.Finyear = SafeDecrypt(node, "Finyear");
                                        item.GrossAmount = SafeDecrypt(node, "GrossAmount");
                                        item.IFSC = SafeDecrypt(node, "IFSC");
                                        item.MonthYY = SafeDecrypt(node, "MonthYY");
                                        item.PayeeCode = SafeDecrypt(node, "PayeeCode");
                                        item.TreaBillStatus = SafeDecrypt(node, "TreaBillStatus");
                                        item.XBillNo = SafeDecrypt(node, "XBillNo");
                                        item.XTBillNo = SafeDecrypt(node, "XTBillNo");
                                        item.YearNm = SafeDecrypt(node, "YearNm");


                                        item.paymentdate = SafeDecrypt(node, "paymentdate");
                                        item.voucherno = SafeDecrypt(node, "voucherno");

                                        allowanceTransactionsDatabase.AddAllowanceTransactions(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try after some time!");
                return 500;
            }
        }

        public async Task<int> GetAllVacancies()
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    RegNo = AesCryptography.Encrypt(""),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var responce = await client.PostAsync($"{AllVacanciesUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetPublishedVacancies", content);

                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {

                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "GetPublishedVacanciesRequestData")
                            {
                                allVacancyDetailsDatabase.DeleteAllVacancyDetails();
                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new AllVacancyDetails();
                                        item.RegNo = SafeDecrypt(node, "RegNo");
                                        item.EmployerName = SafeDecrypt(node, "EmployerName");
                                        item.vacdesc = SafeDecrypt(node, "vacdesc");
                                        item.NoofPost = SafeDecrypt(node, "NoofPost");
                                        item.VacDesgOfPost = SafeDecrypt(node, "VacDesgOfPost");
                                        item.agebw = SafeDecrypt(node, "agebw");
                                        item.intdttm = SafeDecrypt(node, "intdttm");
                                        item.IntPlace = SafeDecrypt(node, "IntPlace");
                                        item.PublishDt = SafeDecrypt(node, "PublishDt");
                                        item.JobfairID = SafeDecrypt(node, "JobfairID");
                                        item.EmpID = SafeDecrypt(node, "EmpID");


                                        allVacancyDetailsDatabase.AddAllVacancyDetails(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        await App.ShowAlertBox("Exception", msg);
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try again after some time.");
                return 500;
            }
        }

        public async Task<int> GetSponsorship(string RegNo)
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    RegNo = AesCryptography.Encrypt(RegNo),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                // var responce = await client.PostAsync($"{AllVacanciesUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetPublishedVacancies", content);
                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {

                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "GetPublishedVacanciesRequestData")
                            {
                                sponsorshipDetailsDatabase.DeleteSponsorshipDetails();
                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new SponsorshipDetails();
                                        item.RegNo = SafeDecrypt(node, "RegNo");
                                        item.EmployerName = SafeDecrypt(node, "EmployerName");
                                        item.vacdesc = SafeDecrypt(node, "vacdesc");
                                        item.NoofPost = SafeDecrypt(node, "NoofPost");
                                        item.VacDesgOfPost = SafeDecrypt(node, "VacDesgOfPost");
                                        item.agebw = SafeDecrypt(node, "agebw");
                                        item.intdttm = SafeDecrypt(node, "intdttm");
                                        item.IntPlace = SafeDecrypt(node, "IntPlace");
                                        item.PublishDt = SafeDecrypt(node, "PublishDt");
                                        item.JobfairID = SafeDecrypt(node, "JobfairID");
                                        item.EmpID = SafeDecrypt(node, "EmpID");
                                        item.ApplyDt = SafeDecrypt(node, "ApplyDt");
                                        item.Attend_Dt = SafeDecrypt(node, "Attend_Dt");
                                        item.Selected_Dt = SafeDecrypt(node, "Selected_Dt");
                                        item.joindt = SafeDecrypt(node, "joindt");


                                        sponsorshipDetailsDatabase.AddSponsorshipDetails(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        await App.ShowAlertBox("Exception", msg);
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try again after some time.");
                return 500;
            }
        }

        public async Task<int> GetPublishedjobfairs()
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    JobfairID = AesCryptography.Encrypt(""),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var responce = await client.PostAsync($"{GetPublishedjobfairsUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetPublishedjobfairs", content);

                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {

                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "GetPublishedVacanciesRequestData")
                            {

                                jobFairDetailsDatabase.DeleteJobFairDetails();
                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new JobFairDetails();
                                        item.Distt = SafeDecrypt(node, "Distt");
                                        item.XchName = SafeDecrypt(node, "XchName");
                                        item.JobfairDate = SafeDecrypt(node, "JobfairDate");
                                        item.venue = SafeDecrypt(node, "venue");
                                        item.remarks = SafeDecrypt(node, "remarks");
                                        item.officer_incharge = SafeDecrypt(node, "officer_incharge");
                                        item.officer_mobile = SafeDecrypt(node, "officer_mobile");
                                        item.officer_email = SafeDecrypt(node, "officer_email");
                                        item.PublishDt = SafeDecrypt(node, "PublishDt");
                                        item.JobfairID = SafeDecrypt(node, "JobfairID");


                                        jobFairDetailsDatabase.AddJobFairDetails(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        await App.ShowAlertBox("Exception", msg);
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try again after some time.");
                return 500;
            }
        }

        public async Task<int> GetPublishedjobfairsEmployers(string JobfairID)
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    JobfairID = AesCryptography.Encrypt(JobfairID),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var responce = await client.PostAsync($"{GetPublishedjobfairsUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetPublishedjobfairs", content);

                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {

                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "GetPublishedVacanciesRequestData")
                            {
                                jobfairsEmployersDatabase.DeleteJobfairsEmployers();
                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {
                                        var item = new JobfairsEmployers();
                                        item.EmployerName = SafeDecrypt(node, "EmployerName");
                                        item.EmpID = SafeDecrypt(node, "EmpID");
                                        item.NatureofWork = SafeDecrypt(node, "NatureofWork");
                                        item.EmployerAddress = SafeDecrypt(node, "EmployerAddress");
                                        item.EmployereMail = SafeDecrypt(node, "EmployereMail");
                                        item.EmployerTel = SafeDecrypt(node, "EmployerTel");
                                        item.JobfairID = SafeDecrypt(node, "JobfairID");


                                        jobfairsEmployersDatabase.AddJobfairsEmployers(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        await App.ShowAlertBox("Exception", msg);
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try again after some time.");
                return 500;
            }
        }

        public async Task<int> GetJobFairEmployersVacancies(string JobfairID, string EmpID)
        {
            try
            {
                var client = new HttpClient();
                string jsonData = JsonConvert.SerializeObject(new
                {
                    JobfairID = AesCryptography.Encrypt(JobfairID),
                    EmpID = AesCryptography.Encrypt(EmpID),
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var responce = await client.PostAsync($"{GetPublishedJOBFAIRVacanciesUrl}", content);
                HttpResponseMessage responce = await client.PostAsync(baseurlx10api + $"GetPublishedJOBFAIRVacancies", content);


                if (!responce.IsSuccessStatusCode)
                {
                    await App.ShowAlertBox(AppName, $"[{responce.StatusCode}] Something went wrong\nPlease try again!");
                    return 500;
                }
                else
                {
                    var result = await responce.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);

                    string msg = parsed["message"]?["message"]?.ToString() ?? string.Empty;
                    int status = Convert.ToInt32(parsed["message"]?["status"]?.ToString() ?? string.Empty);
                    if (status == 200)
                    {

                        foreach (var pair in parsed)
                        {
                            if (pair.Key == "GetPublishedVacanciesRequestData")
                            {
                                jobFairEmployersVacanciesDatabase.DeleteJobFairEmployersVacancies();
                                var nodes = pair.Value;
                                if (nodes != null)
                                {
                                    foreach (var node in nodes)
                                    {

                                        var item = new JobFairEmployersVacancies();
                                        item.EmployerName = SafeDecrypt(node, "EmployerName");
                                        item.vacdesc = SafeDecrypt(node, "vacdesc");
                                        item.NoofPost = SafeDecrypt(node, "NoofPost");
                                        item.VacDesgOfPost = SafeDecrypt(node, "VacDesgOfPost");
                                        item.agebw = SafeDecrypt(node, "agebw");
                                        item.intdttm = SafeDecrypt(node, "intdttm");
                                        item.IntPlace = SafeDecrypt(node, "IntPlace");
                                        item.PublishDt = SafeDecrypt(node, "PublishDt");
                                        item.JobfairID = SafeDecrypt(node, "JobfairID");
                                        item.EmpID = SafeDecrypt(node, "EmpID");
                                        item.XchName = SafeDecrypt(node, "XchName");
                                        item.NcoCd = SafeDecrypt(node, "NcoCd");

                                        item.ApplyDt = SafeDecrypt(node, "ApplyDt");
                                        item.Attend_Dt = SafeDecrypt(node, "Attend_Dt");
                                        item.Selected_Dt = SafeDecrypt(node, "Selected_Dt");
                                        item.joindt = SafeDecrypt(node, "joindt");



                                        jobFairEmployersVacanciesDatabase.AddJobFairEmployersVacancies(item);
                                    }
                                }
                            }
                        }
                        return 200;
                    }
                    else
                    {
                        await App.ShowAlertBox("Exception", msg);
                        return 300;
                    }
                }
            }
            catch
            {
                await App.ShowAlertBox("Exception", "Something went wrong. Please try again after some time.");
                return 500;
            }
        }
    }
}
