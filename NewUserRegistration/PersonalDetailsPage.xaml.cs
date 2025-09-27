using System.Globalization;
using X10Card.Models;
using X10Card.Models.NewUserRegistration;

namespace X10Card.NewUserRegistration;

public partial class PersonalDetailsPage : ContentPage
{
    DistrictMasterDatabase districtMasterDatabase = new DistrictMasterDatabase();
    List<DistrictMaster> districtMasterslist=new List<DistrictMaster>();
    string DistrictCode="";

    ExchangeNameDatabase exchangeNameDatabase = new ExchangeNameDatabase();
    List<ExchangeNameMaster> exchangeMasterslist=new List<ExchangeNameMaster>();
    string ExchangeID="";

    ReligionMasterDatabase religionMasterDatabase = new ReligionMasterDatabase();
    List<ReligionMaster> religionMasterslist=new List<ReligionMaster>();
    string RelgionId = "";

    MaritalStatusMasterDatabase maritalStatusMasterDatabase = new MaritalStatusMasterDatabase();
    List<MaritalStatusMaster> maritalStatusMasterslist=new List<MaritalStatusMaster>();
    string Maritalstatusid = "";

    CategoryMasterDatabase categoryMasterDatabase = new CategoryMasterDatabase();
    List<CategoryMaster> categoryMasterslist=new List<CategoryMaster>();
    string categoryid = "";

    ExistingUserDataDatabase existingUserDataDatabase = new ExistingUserDataDatabase();
    List<ExistingUserData> existingUserDatalist = new List<ExistingUserData>();

    PersonalDetailsDatabase personalDetailsDatabase = new PersonalDetailsDatabase();
    List<PersonalDetails> personalDetailsList = new List<PersonalDetails>();

    string fatherhusbandid = "";
    string malefemaleid = "";
    string dateselected = "", issuedateselected = "";  
    string doc="";
    string issuedate = "", certificateno = "";
    string UserID = "", RegNo = "";
    DateTime Dateofbirth = DateTime.Now;

    public PersonalDetailsPage()
	{
		InitializeComponent();
      

        fatherhusbandid = "F";
        malefemaleid = "M";
    }

   /* protected override void OnAppearing()
    {
        base.OnAppearing();
        getvalues();
        loaddata(); 
    }
*/

    private  void loaddata()
    {

        try
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new UserRegistrationApi();
                int response_getdistrict = await service.GetDistrict();
                int response_getMaritalStatus = await service.GetMaritalStatus();

                int response_GetReligion = await service.GetReligion();
                int response_GetCategory = await service.GetCategory();
                Loading_activity.IsVisible = false;



                if (response_getdistrict == 200)
                {
                    districtMasterslist = districtMasterDatabase.GetDistrictMaster("Select * from DistrictMaster").ToList();
                    picker_district.ItemsSource = districtMasterslist;
                    picker_district.ItemDisplayBinding = new Binding("DistrictName");
                    picker_district.SelectedIndex = 0;
                }
                if (response_getMaritalStatus == 200)
                {
                    maritalStatusMasterslist = maritalStatusMasterDatabase.GetMaritalStatusMaster("Select * from MaritalStatusMaster").ToList();
                    picker_maritalstatus.ItemsSource = maritalStatusMasterslist;
                    picker_maritalstatus.ItemDisplayBinding = new Binding("MaritalStatusListName");
                    picker_maritalstatus.SelectedIndex = 0;
                }
                if (response_GetReligion == 200)
                {
                    religionMasterslist = religionMasterDatabase.GetReligionMaster("Select * from ReligionMaster").ToList();
                    picker_religion.ItemsSource = religionMasterslist;
                    picker_religion.ItemDisplayBinding = new Binding("ReligionName");
                    picker_religion.SelectedIndex = 0;
                }
                if (response_GetCategory == 200)
                {
                    categoryMasterslist = categoryMasterDatabase.GetCategoryMaster("Select * from CategoryMaster").ToList();
                    picker_category.ItemsSource = categoryMasterslist;
                    picker_category.ItemDisplayBinding = new Binding("CategoryName");
                    picker_category.SelectedIndex = 0;
                }

                if (!string.IsNullOrEmpty(RegNo))
                {
                    btn_header.Text = "Personal Details" + "\nRegistration No : " + RegNo + "\nName : " + service.getusername(RegNo);

                    Loadpreviousdata();
                }
                else
                {
                    btn_submit.IsVisible = true;
                    btn_next.IsVisible = false;
                    btn_takelive.IsVisible = false;
                }

                Loading_activity.IsVisible = false;
            
            });



        }
        catch (Exception e)
        {

        }
    }

    private async void Loadpreviousdata()
    {
        //personalDetailsList = personalDetailsDatabase.GetPersonalDetails($"Select * from PersonalDetails where RegistrationNo= '{RegNo}'").ToList();
        personalDetailsList = App.personalDetailsList;
        string userstatus = personalDetailsList.ElementAt(0).Stat??"";

        Label_ApplicationStatus.Text = "Status : " + personalDetailsList.ElementAt(0).StatusDesc;


        if (userstatus.Equals("-1"))
        {
            btn_submit.IsVisible = true;
            btn_next.IsVisible = false;
            btn_takelive.IsVisible = false;
            Label_ApplicationStatus.TextColor = Colors.Orange;
        }
        else if (userstatus.Equals("0"))
        {
            btn_submit.IsVisible = false;
            btn_next.IsVisible = true;
            btn_takelive.IsVisible = false;
            Label_ApplicationStatus.TextColor = Colors.OrangeRed;
        }
        else if (userstatus.Equals("1"))
        {
            btn_submit.IsVisible = false;
            btn_next.IsVisible = true;
            btn_takelive.IsVisible = false;
            Label_ApplicationStatus.TextColor = Colors.DarkGreen;
        }
        else if (userstatus.Equals("2"))
        {
            btn_submit.IsVisible = false;
            btn_next.IsVisible = true;
            btn_takelive.IsVisible = true;
            Label_ApplicationStatus.TextColor = Colors.Red;
        }
        else if (userstatus.Equals("3"))
        {
            btn_submit.IsVisible = true;
            btn_next.IsVisible = false;
            btn_takelive.IsVisible = false;
            Label_ApplicationStatus.TextColor = Colors.Red;
        }


        int districtindex = districtMasterslist.FindIndex(s => s.DistrictID == personalDetailsList.ElementAt(0).DistrictCode);
        if (districtindex != -1)
        {
            picker_district.SelectedIndex = districtindex;
            DistrictCode = districtMasterslist.ElementAt(districtindex).DistrictID ?? "";
            Loading_activity.IsVisible = true;
            var service = new UserRegistrationApi();
            int response_GetExchange = await service.GetExchange(DistrictCode);
            if (response_GetExchange == 200)
            {
                Loading_activity.IsVisible = false;
                exchangeMasterslist = exchangeNameDatabase.GetExchangeNameMaster("Select * from ExchangeNameMaster").ToList();

            }

        }

        int exchangeindex = exchangeMasterslist.FindIndex(s => s.ExchangeID == personalDetailsList.ElementAt(0).XchangeCode);
        if (exchangeindex != -1)
        {
            picker_exchangename.SelectedIndex = exchangeindex;
            ExchangeID = exchangeMasterslist.ElementAt(exchangeindex).ExchangeID ?? "";
        }


        /* picker_district.IsEnabled = false;
         picker_exchangename.IsEnabled = false;*/

        entry_applicantname.Text = personalDetailsList.ElementAt(0).UserName;
        string fhselection = personalDetailsList.ElementAt(0).FAtherHusbandSelection ?? "".Trim();
        if (fhselection.Equals("F"))
        {
            rd_Father.IsChecked = true;
            rd_Husband.IsChecked = false;
            fatherhusbandid = "F";
        }
        else
        {
            rd_Husband.IsChecked = true;
            rd_Father.IsChecked = false;
            fatherhusbandid = "H";

        }
        entry_fatherhusbname.Text = personalDetailsList.ElementAt(0).TXTFHNAME;
        entry_mothername.Text = personalDetailsList.ElementAt(0).txtmother;

        int maritalindex = maritalStatusMasterslist.FindIndex(s => s.MaritalStatusListID == personalDetailsList.ElementAt(0).ddlmarital);
        if (maritalindex != -1)
        {
            picker_maritalstatus.SelectedIndex = maritalindex;
            Maritalstatusid = maritalStatusMasterslist.ElementAt(maritalindex).MaritalStatusListID ?? "";
        }

        Entry_dob.Text = personalDetailsList.ElementAt(0).DOB;
        try
        {
            Dateofbirth = DateTime.ParseExact(Entry_dob.Text + " 12:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch
        {
            Dateofbirth = DateTime.ParseExact(Entry_dob.Text + " 12:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        }


        string genderselection = personalDetailsList.ElementAt(0).GENDER ?? "".Trim();
        if (genderselection.Equals("M"))
        {
            rd_Male.IsChecked = true;
            rd_Female.IsChecked = false;
            malefemaleid = "M";
        }
        else
        {
            rd_Female.IsChecked = true;
            rd_Male.IsChecked = false;
            malefemaleid = "F";
        }

        int religionlindex = religionMasterslist.FindIndex(s => s.ReligionID == personalDetailsList.ElementAt(0).ddlreligion);
        if (religionlindex != -1)
        {
            picker_religion.SelectedIndex = religionlindex;
            RelgionId = religionMasterslist.ElementAt(religionlindex).ReligionID ?? "";
        }

        int categoryindex = categoryMasterslist.FindIndex(s => s.CategoryID == personalDetailsList.ElementAt(0).Category);
        if (categoryindex != -1)
        {
            picker_category.SelectedIndex = categoryindex;
            categoryid = categoryMasterslist.ElementAt(categoryindex).CategoryID ?? "";

            if (!categoryid.Equals("4"))
            {
                stack_catg.IsVisible = true;
                Entry_issuedate.Text = personalDetailsList.ElementAt(0).issuedt;
                entry_certificateno.Text = personalDetailsList.ElementAt(0).DocCertNo;
                string pdffile = personalDetailsList.ElementAt(0).DocFileLink ?? "";
                if (!string.IsNullOrEmpty(pdffile))
                {
                    doc = pdffile;
                    btn_viewdoc.IsVisible = true;
                }
                else
                {
                    doc = "";
                    btn_viewdoc.IsVisible = false;
                }
            }
            else
            {
                stack_catg.IsVisible = false;
            }
        }
    }

    private async void picker_district_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_district.SelectedIndex != -1)
        {

            DistrictCode = districtMasterslist.ElementAt(picker_district.SelectedIndex).DistrictID ?? "";
            // EmploymentStatusName = employmentStatuslist.ElementAt(picker_district.SelectedIndex).EmpStatDesc;
            Loading_activity.IsVisible = true;
            var service = new UserRegistrationApi();
            int response_GetExchange = await service.GetExchange(DistrictCode);
            if (response_GetExchange == 200)
            {
                Loading_activity.IsVisible = false;
                exchangeMasterslist = exchangeNameDatabase.GetExchangeNameMaster("Select * from ExchangeNameMaster").ToList();
                picker_exchangename.ItemsSource = exchangeMasterslist;
                picker_exchangename.ItemDisplayBinding = new Binding("ExchangeName");
                picker_exchangename.SelectedIndex = 0;
            }
            Loading_activity.IsVisible = false;
        }
    }

    private void picker_exchangename_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_exchangename.SelectedIndex != -1)
        {

            ExchangeID = exchangeMasterslist.ElementAt(picker_exchangename.SelectedIndex).ExchangeID ?? "";

        }
    }

    private void rd_Father_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rd_Father.IsChecked)
        {
            fatherhusbandid = "F";
        }
        else
        {
            fatherhusbandid = "H";
        }
    }

    private void picker_maritalstatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_maritalstatus.SelectedIndex != -1)
        {
            Maritalstatusid = maritalStatusMasterslist.ElementAt(picker_maritalstatus.SelectedIndex).MaritalStatusListID ?? "";
        }
    }

    private void Entry_dob_Focused(object sender, FocusEventArgs e)
    {
        Entry_dob.Unfocus();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            datepicker_dob.MaximumDate = DateTime.Now.AddYears(-14);
            datepicker_dob.Focus();
        });
       
    }

    private void datepicker_dob_DateSelected(object sender, DateChangedEventArgs e)
    {
        dateselected = e.NewDate.ToString("dd/MM/yyyy HH:mm:ss");
        try
        {
            Dateofbirth = DateTime.ParseExact(dateselected, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch
        {
            Dateofbirth = DateTime.ParseExact(dateselected, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }


        Entry_dob.Text = e.NewDate.ToString("dd/MM/yyyy").Replace('-', '/');
    }

    private void rd_Male_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rd_Male.IsChecked)
        {
            malefemaleid = "M";
        }
        else
        {
            malefemaleid = "F";
        }
    }

    private void picker_religion_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_religion.SelectedIndex != -1)
        {
            RelgionId = religionMasterslist.ElementAt(picker_religion.SelectedIndex).ReligionID ?? "";
        }
    }

    private void picker_category_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_category.SelectedIndex != -1)
        {
            categoryid = categoryMasterslist.ElementAt(picker_category.SelectedIndex).CategoryID ?? "";
            if (!categoryid.Equals("4"))
            {
                stack_catg.IsVisible = true;

            }
            else
            {
                stack_catg.IsVisible = false;
            }

        }
    }

    async Task<FileResult?> PickAndShow(PickOptions options, int docnumber)
    {
        try
        {
            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                var FileNameText_ = $"File Name: {result.FileName}";
                if (result.FileName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var stream = await result.OpenReadAsync();

                    long length = stream.Length / 1024;
                    if (length >= 1024)
                    {
                        await App.ShowAlertBox(App.AppName, "Max. file size limit is 1 MB.");
                        
                    }
                    else
                    {
                        if (docnumber == 1)
                        {
                            doc = App.ConvertToBase64(stream).ToString();
                            lbl_pdf1.IsVisible = false;
                            btn_uploaddoc1.BackgroundColor = Colors.Green;
                            btn_uploaddoc1.TextColor = Colors.White;
                            btn_uploaddoc1.Text = "Successfully Uploaded";
                        }

                    }
                }
                else
                {
                    await App.ShowAlertBox(App.AppName, "Only PDF files are allowed");
                    
                }
               
            }
            return result;

        }
        catch
        {
            return null;
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {        
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mainWindow.Page = new NavigationPage(new PostLoginDashboardPage());

            });
        }
    }

    private void Entry_issuedate_Focused(object sender, FocusEventArgs e)
    {
        Entry_issuedate.Unfocus();
        MainThread.BeginInvokeOnMainThread(() => {
            datepicker_issuedate.MinimumDate = Dateofbirth;
            datepicker_issuedate.MaximumDate = DateTime.Now;
            datepicker_issuedate.Focus();
        });
       
    }

    private void popupExistingUserCancel_Clicked(object sender, EventArgs e)
    {
        popupExistingUser.IsVisible = false;
    }

    private async void popupExistingUserMap_Clicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is not null)
        {
            string regno = b.CommandParameter.ToString() ?? "";
            List<ExistingUserData> existingUserDatadetailslist;
            existingUserDatadetailslist = existingUserDataDatabase.GetExistingUserData($"Select * from ExistingUserData where RegistrationNo='{regno}'").ToList();
            string XchangeCode = existingUserDatadetailslist.ElementAt(0).XchangeCode ?? "";
            string dob = existingUserDatadetailslist.ElementAt(0).DOB ?? "";

            Loading_activity.IsVisible = true;
            var service = new UserRegistrationApi();
            int response_usermap = await service.SaveUserMapping(XchangeCode, dob, regno, UserID);
            Loading_activity.IsVisible = false;
            if (response_usermap == 200)
            {                
                popupExistingUser.IsVisible = false;
                if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        mainWindow.Page = new NavigationPage(new PostLoginDashboardPage());

                    });
                }              
            }            
        }
    }

    private void btn_viewdoc_Clicked(object sender, EventArgs e)
    {
        //App.ConvertBase64ToPdf(personalDetailsList.ElementAt(0).DocFileLink);
        var service = new UserRegistrationApi();
        service.getpdf(RegNo, "personal", "");
    }

    private void datepicker_issuedate_DateSelected(object sender, DateChangedEventArgs e)
    {
        issuedateselected = e.NewDate.ToString("dd/MM/yyyy");
        Entry_issuedate.Text = issuedateselected.Replace('-', '/');
    }

    private async void btn_uploaddoc1_Clicked(object sender, EventArgs e)
    {
        var pickOptions = new PickOptions
        {
            PickerTitle = "Please select a PDF file",
            FileTypes = FilePickerFileType.Pdf // Use built-in PDF file filter
        };
        var result = await PickAndShow(pickOptions, 1);
    }

    private void getvalues()
    {
        UserID = Preferences.Get("UserID", "");
        RegNo = Preferences.Get("RegNo", "");
    }

    async Task<bool> checkvalidation()
    {
        try
        {
            if (string.IsNullOrEmpty(entry_applicantname.Text))
            {
                await DisplayAlert(App.AppName, "Enter Applicant Name", "Close");
                return false;
            }
            else if (!App.isAlphabetonly(entry_applicantname.Text.ToString().Trim()))
            {
                await DisplayAlert(App.AppName, "Only Alphabets are allowed in Applicant Name", "Close");
                return false;
            }

            if (string.IsNullOrEmpty(entry_fatherhusbname.Text))
            {
                await DisplayAlert(App.AppName, "Enter Father/Husband Name", "Close");
                return false;
            }
            else if (!App.isAlphabetonly(entry_fatherhusbname.Text.ToString().Trim()))
            {
                await DisplayAlert(App.AppName, "Only Alphabets are allowed in Father/Husband Name", "Close");
                return false;
            }

            if (string.IsNullOrEmpty(Entry_dob.Text))
            {
                await DisplayAlert(App.AppName, "Enter Date Of Birth", "Close");
                return false;
            }


            if (!categoryid.Equals("4"))
            {
                if (string.IsNullOrEmpty(Entry_issuedate.Text))
                {
                    await DisplayAlert(App.AppName, "Enter Issue Date", "Close");
                    return false;
                }
                if (string.IsNullOrEmpty(entry_certificateno.Text))
                {
                    await DisplayAlert(App.AppName, "Enter Certificate No.", "Close");
                    return false;
                }
                else if (!App.isAlphaNumeric(entry_certificateno.Text.ToString().Trim()))
                {
                    await DisplayAlert(App.AppName, "Only Alphanumeric characters are allowed in Certificate No.", "Close");
                    return false;
                }
                if (string.IsNullOrEmpty(doc))
                {
                    await DisplayAlert(App.AppName, "Upload Document", "Close");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "OK");
            return false;
        }
        return true;
    }

    private async void btn_submit_Clicked(object sender, EventArgs e)
    {
        var service = new UserRegistrationApi();
        if (!categoryid.Equals("4"))
        {
            issuedate = Entry_issuedate.Text;
            certificateno = entry_certificateno.Text;
        }
        else
        {
            doc = string.Empty;
            issuedate = "";
            certificateno = "";
        }
     
        try
        {
            Preferences.Set("DOB", Dateofbirth.Date.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch
        {
            Preferences.Set("DOB", Dateofbirth.Date.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        if (await checkvalidation())
        {
            Loading_activity.IsVisible = true;
            int response_SavePersonalDetails = await service.SavePersonalDetails(RegNo,
                        ExchangeID, entry_applicantname.Text, fatherhusbandid, entry_fatherhusbname.Text, entry_mothername.Text,
                        Maritalstatusid, malefemaleid, Entry_dob.Text, RelgionId, categoryid, issuedate, certificateno, doc, UserID);
            if (response_SavePersonalDetails == 200)
            {
                int reposne_GetRegDetailsLabels = await service.GetRegDetailsLabels(RegNo);
                int response_GetPersonalDetail = await service.GetPersonalDetails(RegNo);
                PersonalDetailsDatabase personalDetailsDatabase = new PersonalDetailsDatabase();

                App.personalDetailsList = personalDetailsDatabase.GetPersonalDetails($"Select * from PersonalDetails where RegistrationNo= '{RegNo}'").ToList();
                Loading_activity.IsVisible = false;

               
                if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        mainWindow.Page = new NewUserRegistrationMasterPage(2);

                    });
                }               
            }
            else if (response_SavePersonalDetails == 300)
            {
                int response_Getexistinguser = await service.GetExistUserData(Entry_dob.Text, entry_fatherhusbname.Text, entry_applicantname.Text);
                if (response_Getexistinguser == 200)
                {
                    Loading_activity.IsVisible = false;
                    string query = $"Select  * from ExistingUserData";
                    existingUserDatalist = existingUserDataDatabase.GetExistingUserData(query).ToList();
                    listview_ExistingUser.ItemsSource = existingUserDatalist;
                    popupExistingUser.IsVisible = true;
                }
            }
            Loading_activity.IsVisible = false;
        }
    }

    private void btn_next_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("RegNo", RegNo);
        Preferences.Set("DOB", Entry_dob.Text);
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mainWindow.Page = new NewUserRegistrationMasterPage(2);

            });
        }
    }

    private async void btn_takelive_Clicked(object sender, EventArgs e)
    {
        Loading_activity.IsVisible = true;
        var service = new UserRegistrationApi();
        int response_TakeLive = await service.TakeLive(RegNo, UserID, ExchangeID);
        Loading_activity.IsVisible = false;
        if (response_TakeLive == 200)
        {            
            if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    mainWindow.Page = new NavigationPage(new PostLoginDashboardPage());

                });
            }
        }
     
    }

}
