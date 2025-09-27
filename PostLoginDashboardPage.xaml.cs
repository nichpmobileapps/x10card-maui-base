using X10Card.Models;
using X10Card.Models.NewUserRegistration;
using X10Card.NewUserRegistration;
using X10Card.webapi;

namespace X10Card;

public partial class PostLoginDashboardPage : ContentPage
{
    string UserID = "", RegNo = "";
    ApplicantDashboardDatabase applicantDashboardDatabase = new ApplicantDashboardDatabase();
    List<ApplicantDashboard> applicantDashboardlist = new List<ApplicantDashboard>();


    public PostLoginDashboardPage()
    {
        InitializeComponent();
        var service = new HitServices();
        //service.update();
    }
    protected override void OnAppearing()
    {
        getvalues();

        GetApplicantDashboardData();

        if (!string.IsNullOrEmpty(RegNo))
        {
            stack_newuserdata.IsVisible = false;
            btn_viewdetails.IsVisible = true;
            stack_candidatedata.IsVisible = true;
            btn_newregister.Text = "Update/Modify Registration";
            btn_alreadyregister.IsVisible = false;
        }
        else
        {
            stack_candidatedata.IsVisible = false;
            btn_viewdetails.IsVisible = false;
            stack_newuserdata.IsVisible = true;
            btn_newregister.Text = "New Registration";
            btn_alreadyregister.IsVisible = true;

        }
    }

    async void GetApplicantDashboardData()
    {
        var service = new UserRegistrationApi();
        Loading_activity.IsVisible = true;
        int reposnse_GetApplicantDashboardData = await service.GetApplicantDashboardData(UserID);
        if (reposnse_GetApplicantDashboardData == 200)
        {
            applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();
            if (applicantDashboardlist != null && applicantDashboardlist.Any())
            {
                var applicant = applicantDashboardlist[0];

                var regNoEncrypted = applicant?.RegistrationNo;

                if (!string.IsNullOrEmpty(regNoEncrypted))
                {
                    RegNo = AesCryptography.Decrypt(regNoEncrypted);
                }
                else
                {
                    RegNo = string.Empty;
                }

                loadphoto();
                if (!string.IsNullOrEmpty(RegNo))
                {

                    stack_newuserdata.IsVisible = false;

                    stack_candidatedata.IsVisible = true;
                    btn_newregister.Text = "View/Update Registration Details";
                    btn_alreadyregister.IsVisible = false;

                    if (applicant != null)
                    {
                        lbl_header.Text = AesCryptography.Decrypt(applicant.Name ?? string.Empty);
                        Label_registrationnumber.Text = AesCryptography.Decrypt(applicant.RegistrationNo ?? string.Empty);
                        Label_XchName.Text = AesCryptography.Decrypt(applicant.Exchange ?? string.Empty);
                        Label_RegDate.Text = AesCryptography.Decrypt(applicant.Submissiondate ?? string.Empty);
                        Label_ValidUpto.Text = AesCryptography.Decrypt(applicant.ValidUptodate ?? string.Empty);
                        string userstatus = AesCryptography.Decrypt(applicant.userStatus ?? string.Empty);


                        if (userstatus.Equals("-1"))
                        {
                            btn_viewdetails.IsVisible = false;
                            Label_ApplicationStatus.Text = "Your Registration Details Are Pending and Application Status is Draft";
                            Label_ApplicationStatus.TextColor = Color.FromArgb("#eb7c0e");
                        }
                        else if (userstatus.Equals("0"))
                        {
                            btn_viewdetails.IsVisible = false;
                            Label_ApplicationStatus.Text = "Application Submitted";
                            Label_ApplicationStatus.TextColor = Colors.OrangeRed;
                        }
                        else if (userstatus.Equals("1"))
                        {
                            btn_viewdetails.IsVisible = true;
                            Label_ApplicationStatus.Text = "Accepted/Approved";
                            Label_ApplicationStatus.TextColor = Colors.DarkGreen;
                        }
                        else if (userstatus.Equals("2"))
                        {
                            btn_viewdetails.IsVisible = false;
                            Label_ApplicationStatus.Text = "Dead Registration";
                            Label_ApplicationStatus.TextColor = Colors.Red;
                        }
                        else if (userstatus.Equals("3"))
                        {
                            btn_viewdetails.IsVisible = false;
                            Label_ApplicationStatus.Text = "Refered Back";
                            Label_ApplicationStatus.TextColor = Colors.Red;
                        }
                    }

                }
                else
                {
                    stack_candidatedata.IsVisible = false;
                    btn_viewdetails.IsVisible = false;
                    stack_newuserdata.IsVisible = true;
                    btn_newregister.Text = "New Registration";
                    btn_alreadyregister.IsVisible = true;

                    string mobileNo = AesCryptography.Decrypt(applicant?.MobileNo ?? string.Empty);
                    string emailId = AesCryptography.Decrypt(applicant?.EmailID ?? string.Empty);

                    Label_mob.Text = string.IsNullOrEmpty(mobileNo) ? "Not Available" : mobileNo;
                    Label_email.Text = string.IsNullOrEmpty(emailId) ? "Not Available" : emailId;

                    
                }
            }

        }


        Loading_activity.IsVisible = false;
    }

    private async void btn_viewdetails_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(RegNo))
        {
            Loading_activity.IsVisible = true;
            var service = new HitServices();
            int response = await service.validatelogin(RegNo);
            if (response == 200)
            {
                Preferences.Set("Active", 0);
                await service.validatelogin(RegNo);
                await service.GetAllVacancies();
                await service.GetSponsorship(RegNo);
                Loading_activity.IsVisible = false;                

                if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new HomePage());

                    });
                }
            }
            Loading_activity.IsVisible = false;
        }
        else
        {
            await App.ShowAlertBox(App.AppName, "No Mapped Registration Number Found.\nKindly register/map.");

        }

    }

    private async void btn_newregister_Clicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new PersonalDetailsPage());
        var service = new UserRegistrationApi();
        Loading_activity.IsVisible = true;
        if (!string.IsNullOrEmpty(RegNo))
        {
            Preferences.Set("Indexer_Reg", RegNo);

            int reposne_GetRegDetailsLabels = await service.GetRegDetailsLabels(RegNo);
            int response_GetPersonalDetail = await service.GetPersonalDetails(RegNo);
            Loading_activity.IsVisible = false;
            PersonalDetailsDatabase personalDetailsDatabase = new PersonalDetailsDatabase();

            App.personalDetailsList = personalDetailsDatabase.GetPersonalDetails($"Select * from PersonalDetails where RegistrationNo= '{RegNo}'").ToList();
            Loading_activity.IsVisible = false;
        }
        else
        {
            App.personalDetailsList = new List<PersonalDetails>();
        }
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new NewUserRegistrationMasterPage(1));

            });
        }

        
    }

    private void btn_alreadyregister_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AlreadyRegisteredPage());
    }

    private void loadphoto()
    {
        try
        {
            applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();

            string empphoto = applicantDashboardlist.ElementAt(0).UserImage??"";
            if (!string.IsNullOrEmpty(empphoto))
            {
                var bytes = Convert.FromBase64String(empphoto);
                candidateimage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            else
            {
                candidateimage.Source = "ic_usericon.png";
            }
        }
        catch
        {
            candidateimage.Source = "ic_usericon.png";
        }
    }

    private void getvalues()
    {
        UserID = Preferences.Get("UserID", "");
        RegNo = Preferences.Get("RegNo", "");
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();

        bool m;
        var applicant = applicantDashboardlist[0];
        string Name = AesCryptography.Decrypt(applicant?.Name ?? string.Empty);
        string MobileNo = AesCryptography.Decrypt(applicant?.MobileNo??string.Empty);
        string EmailID = AesCryptography.Decrypt(applicant?.EmailID ?? string.Empty);
        if (!string.IsNullOrEmpty(Name))
        {
            m = await DisplayAlert(App.AppName, "Are you sure " + Name + " you want to logout.", "Logout", "Cancel");

        }
        else
        {
            if (!string.IsNullOrEmpty(MobileNo))
            {
                m = await DisplayAlert(App.AppName, "Are you sure " + MobileNo + " you want to logout.", "Logout", "Cancel");
            }
            else
            {
                m = await DisplayAlert(App.AppName, "Are you sure " + EmailID + " you want to logout.", "Logout", "Cancel");
            }
        }
        if (m)
        {
            SecureStorage.RemoveAll();
            Preferences.Clear();
            applicantDashboardDatabase.DeleteApplicantDashboard();
            if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    mainWindow.Page = new NavigationPage(new MainPage());
                });
            }
        }
    }

    private async void imgbtn_uploadimage_Clicked(object sender, EventArgs e)
    {
        candidateimage.Source = null;
        var m = await App.UploadPhoto(candidateimage);
        if (candidateimage.Source != null)
        {
            candidateimage.IsVisible = true;
            Loading_activity.IsVisible = true;
            var service = new UserRegistrationApi();
            int reposne_uploadphoto = await service.UploadImageInsUpd(App.PhotoBase64, UserID);
            if (reposne_uploadphoto == 200)
            {
                Loading_activity.IsVisible = false;
                loadphoto();
            }

            Loading_activity.IsVisible = false;
        }
    }



}

