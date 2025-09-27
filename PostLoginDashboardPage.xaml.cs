using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X10Card.Models;
using X10Card.Models.NewUserRegistration;
using X10Card.NewUserRegistration;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace X10Card
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostLoginDashboardPage : ContentPage
    {
        string UserID, RegNo;
        ApplicantDashboardDatabase applicantDashboardDatabase = new ApplicantDashboardDatabase();
        List<ApplicantDashboard> applicantDashboardlist;


        public PostLoginDashboardPage()
        {
            InitializeComponent();
            lbl_navigation_header.Text = App.AppName;
           

            App.update();
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
                RegNo = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).RegistrationNo);
                
                loadphoto();
                if (!string.IsNullOrEmpty(RegNo))
                {

                    stack_newuserdata.IsVisible = false;

                    stack_candidatedata.IsVisible = true;
                    btn_newregister.Text = "View/Update Registration Details";
                    btn_alreadyregister.IsVisible = false;

                    lbl_header.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).Name);
                    Label_registrationnumber.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).RegistrationNo);
                    Label_XchName.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).Exchange);
                    Label_RegDate.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).Submissiondate);
                    Label_ValidUpto.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).ValidUptodate);
                    string userstatus = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).userStatus);
                   
                    if (userstatus.Equals("-1"))
                    {
                        btn_viewdetails.IsVisible = false;
                        Label_ApplicationStatus.Text = "Your Registration Details Are Pending and Application Status is Draft";
                        Label_ApplicationStatus.TextColor = Color.FromHex("#eb7c0e");
                    }
                    else if (userstatus.Equals("0"))
                    {
                        btn_viewdetails.IsVisible = false;
                        Label_ApplicationStatus.Text = "Application Submitted";
                        Label_ApplicationStatus.TextColor = Color.OrangeRed;
                    }
                    else if (userstatus.Equals("1"))
                    {
                        btn_viewdetails.IsVisible = true;
                        Label_ApplicationStatus.Text = "Accepted/Approved";
                        Label_ApplicationStatus.TextColor = Color.DarkGreen;
                    }
                    else if (userstatus.Equals("2"))
                    {
                        btn_viewdetails.IsVisible = false;
                        Label_ApplicationStatus.Text = "Dead Registration";
                        Label_ApplicationStatus.TextColor = Color.Red;
                    }
                    else if (userstatus.Equals("3"))
                    {
                        btn_viewdetails.IsVisible = false;
                        Label_ApplicationStatus.Text = "Refered Back";
                        Label_ApplicationStatus.TextColor = Color.Red;
                    }

                   
                }
                else
                {
                    stack_candidatedata.IsVisible = false;
                    btn_viewdetails.IsVisible = false;
                    stack_newuserdata.IsVisible = true;
                    btn_newregister.Text = "New Registration";
                    btn_alreadyregister.IsVisible = true;

                    string MobileNo = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).MobileNo);
                    string emailid = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).EmailID);
                    if (!string.IsNullOrEmpty(MobileNo))
                    {
                        Label_mob.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).MobileNo);
                    }
                    else
                    {
                        Label_mob.Text = "Not Available";
                    }

                    if (!string.IsNullOrEmpty(emailid))
                    {
                        Label_email.Text = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).EmailID);
                    }
                    else
                    {
                        Label_email.Text = "Not Available";
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
                int response = await App.validatelogin(RegNo);
                if (response == 200)
                {
                    Preferences.Set("Active", 0);
                    await App.GetAllVacancies();
                    await App.GetSponsorship(RegNo);
                    await App.GetAllowanceDetails(RegNo);
                    Loading_activity.IsVisible = false;
                    Application.Current.MainPage = new NavigationPage(new HomePage());
                }
                Loading_activity.IsVisible = false;
            }
            else
            {
                await DisplayAlert(App.AppName, "No Mapped Registration Number Found.\nKindly register/map.", App.close);
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
                PersonalDetailsDatabase personalDetailsDatabase = new PersonalDetailsDatabase();    
              
                App.personalDetailsList = personalDetailsDatabase.GetPersonalDetails($"Select * from PersonalDetails where RegistrationNo= '{RegNo}'").ToList();
                Loading_activity.IsVisible = false;
            }
            else
            {
                App.personalDetailsList = null;
            }
            Application.Current.MainPage = new NewUserRegistrationMasterPage(1);
        }

        private void btn_alreadyregister_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AlreadyRegisteredPage());
        }

        private void loadphoto()
        {
            //  string DummyImg = "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";
            try
            {
                applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();

                string empphoto = applicantDashboardlist.ElementAt(0).UserImage;
                if (!string.IsNullOrEmpty(empphoto))
                {
                    var bytes = Convert.FromBase64String(empphoto);
                    candidateimage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                else
                {

                    /* var bytes = Convert.FromBase64String(DummyImg);
                     candidateimage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));*/
                    candidateimage.Source = "ic_usericon.png";
                }
            }
            catch
            {
                /* var bytes = Convert.FromBase64String(DummyImg);
                 candidateimage.Source = ImageSource.FromStream(() => new MemoryStream(bytes));*/
                candidateimage.Source = "ic_usericon.png";

            }
        }

        private  void getvalues()
        {

            
                UserID = Preferences.Get("UserID", "");
                RegNo = Preferences.Get("RegNo", "");
            
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();

            bool m;

            string Name = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).Name);
            string MobileNo = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).MobileNo);
            string EmailID = CommonClass.Decrypt(applicantDashboardlist.ElementAt(0).EmailID);
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

                Application.Current.MainPage = new NavigationPage(new MainPage());

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
                   // Application.Current.MainPage = new NavigationPage(new PostLoginDashboardPage());
                }

                Loading_activity.IsVisible = false;
            }
        }



    }
}