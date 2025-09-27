using System.Collections.ObjectModel;
using System.Globalization;
using X10Card.Models;
using X10Card.Models.NewUserRegistration;

namespace X10Card.NewUserRegistration;

public partial class FlyoutMenuPage : ContentPage
{
    string UserID="", RegNo="";
    ObservableCollection<FlyoutPageItem> flyoutPageItems = new ObservableCollection<FlyoutPageItem>();
    public ObservableCollection<FlyoutPageItem> FlyoutPageItems { get { return flyoutPageItems; } }

    SubmittedFormsDatabase submittedFormsDatabase = new SubmittedFormsDatabase();
    List<SubmittedFormsDetails> submittedFormsDetailslist=new List<SubmittedFormsDetails>();

    string dob = "";
    DateTime Dateofbirth =DateTime.Now;
    string personalcolor="";

    ApplicantDashboardDatabase applicantDashboardDatabase = new ApplicantDashboardDatabase();
    List<ApplicantDashboard> applicantDashboardlist = new List<ApplicantDashboard>();

    public FlyoutMenuPage()
	{
		InitializeComponent();
        getvalues();

        var service = new UserRegistrationApi();
        if (!string.IsNullOrEmpty(RegNo))
        {
            lbl_username.Text = service.getusername(RegNo);
        }
        else
        {
            lbl_username.Text = "";
        }
        loadphoto();

        var m = App.personalDetailsList;
        if (m != null)
        {
            if (App.personalDetailsList.Any())
            {
                dob = App.personalDetailsList.ElementAt(0).DOB??"";
                try
                {
                    Dateofbirth = DateTime.ParseExact(dob + " 12:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Preferences.Set("DOB", Dateofbirth.Date.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                catch
                {
                    Dateofbirth = DateTime.ParseExact(dob + " 12:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Preferences.Set("DOB", Dateofbirth.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await loadcolors();
            });
           
        }
        else
        {
            flyoutPageItems.Add(new FlyoutPageItem { Id = 1, Title = "Personal", MenuIcon = "Home.png", Textcolor = personalcolor });
            collectionViewFlyout.ItemsSource = flyoutPageItems;
        }

    }

    async Task loadcolors()
    {
        var service = new UserRegistrationApi();
        int reposne_GetRegDetailsLabels = await service.GetRegDetailsLabels(Preferences.Get("Indexer_Reg", "0"));
        if (reposne_GetRegDetailsLabels == 200)
        {
            submittedFormsDetailslist = submittedFormsDatabase.GetSubmittedFormsDetails("Select * from SubmittedFormsDetails").ToList();
            personalcolor = submittedFormsDetailslist.ElementAt(0).PersonalDetails ?? "";

            if (App.personalDetailsList.Any())
            {
                string contactcolor = submittedFormsDetailslist.ElementAt(0).ContactDetails ?? "";
                string Educationcolor = submittedFormsDetailslist.ElementAt(0).EducationDetails ?? "";
                string Miscellaneouscolor = submittedFormsDetailslist.ElementAt(0).MiscDetails ?? "";
                string Employmentcolor = submittedFormsDetailslist.ElementAt(0).EmployedDetails ?? "";
                string SubCategorycolor = submittedFormsDetailslist.ElementAt(0).SubCat ?? "";
                string phcolor = submittedFormsDetailslist.ElementAt(0).PH ?? "";
                string ExServiceMancolor = submittedFormsDetailslist.ElementAt(0).ExDetails ?? "";
                string NCOcolor = submittedFormsDetailslist.ElementAt(0).NCODetails ?? "";
                string PersonalDetailsYN = submittedFormsDetailslist.ElementAt(0).PersonalDetailsYN ?? "";
                string ContactDetailsYN = submittedFormsDetailslist.ElementAt(0).ContactDetailsYN ?? "";

                flyoutPageItems.Add(new FlyoutPageItem { Id = 1, Title = "Personal", MenuIcon = "ic_personal.png", Textcolor = personalcolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 2, Title = "Contact", MenuIcon = "ic_contact.png", Textcolor = contactcolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 3, Title = "Qualification", MenuIcon = "ic_education.png", Textcolor = Educationcolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 4, Title = "Miscellaneous", MenuIcon = "ic_misc.png", Textcolor = Miscellaneouscolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 5, Title = "Employment", MenuIcon = "ic_employment.png", Textcolor = Employmentcolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 6, Title = "Sub-Category", MenuIcon = "ic_category.png", Textcolor = SubCategorycolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 7, Title = "Physically Handicapped", MenuIcon = "ic_handicapped.png", Textcolor = phcolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 8, Title = "Ex-ServiceMen", MenuIcon = "ic_exserviceman.png", Textcolor = ExServiceMancolor });
                flyoutPageItems.Add(new FlyoutPageItem { Id = 9, Title = "NCO", MenuIcon = "ic_category.png", Textcolor = NCOcolor });
                string userstatus = App.personalDetailsList.ElementAt(0).Stat ?? "";
                if (userstatus.Equals("-1") || userstatus.Equals("3"))
                {
                    if (PersonalDetailsYN.ToUpper().Equals("Y") && ContactDetailsYN.ToUpper().Equals("Y"))
                    {
                        flyoutPageItems.Add(new FlyoutPageItem { Id = 10, Title = "Final Submit", MenuIcon = "ic_finalsubmit.png", Textcolor = "#d70f3d" });
                        lbl_finalsubmit.IsVisible = false;
                    }
                    else
                    {
                        lbl_finalsubmit.IsVisible = true;
                    }
                }
            }
            else
            {
                flyoutPageItems.Add(new FlyoutPageItem { Id = 1, Title = "Personal", MenuIcon = "ic_personal.png", Textcolor = personalcolor });
            }
        }
        else
        {
            flyoutPageItems.Add(new FlyoutPageItem { Id = 1, Title = "Personal", MenuIcon = "ic_personal.png", Textcolor = personalcolor });
        }
        collectionViewFlyout.ItemsSource = flyoutPageItems;
    }

    private void getvalues()
    {
        UserID = Preferences.Get("UserID", "");
        RegNo = Preferences.Get("RegNo", "");
    }

    private void loadphoto()
    {
        
        try
        {
            applicantDashboardlist = applicantDashboardDatabase.GetApplicantDashboard("Select * from ApplicantDashboard").ToList();

            string empphoto = applicantDashboardlist.ElementAt(0).UserImage ?? "";
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

}
