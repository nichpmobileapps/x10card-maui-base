using X10Card.Models;
using X10Card.webapi;

namespace X10Card;

public partial class ViewJobFairEmployersVacanciesPage : ContentPage
{
    JobFairEmployersVacanciesDatabase jobFairEmployersVacanciesDatabase = new JobFairEmployersVacanciesDatabase();
    List<JobFairEmployersVacancies> jobfairsEmployersVacancieslists;
    public Label[] Footer_Labels;
    public string[] Footer_Image_Source;
    public Image[] Footer_Images;

    UserDetailsDatabase userDetailsDatabase = new UserDetailsDatabase();
    List<UserDetails> userDetailslist;
    string distt, exchange, employer, RegNo="";
    public ViewJobFairEmployersVacanciesPage(string district, string xchangename, string employername)
	{
		InitializeComponent();
        userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
        distt = district;
        exchange = xchangename;
        employer = employername;

         RegNo = userDetailslist.ElementAt(0).RegNo??"";
        string username = userDetailslist.ElementAt(0).CandiName??"";
        lbl_header.Text = "Job Fair Details - " + username;

        Footer_Labels = new Label[4] { Tab_Home_Label, Tab_UpdateEmpStatus_Label, Tab_ViewAllowances_Label, Tab_Settings_Label };
        Footer_Images = new Image[4] { Tab_Home_Image, Tab_UpdateEmpStatus_Image, Tab_ViewAllowances_Image, Tab_Settings_Image };
        Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };

        string query = $"Select *" +
            $", (case when ApplyDt <> '' then 'true' else 'false' end)ApplyDtVisibility " +
            $", (case when Attend_Dt <> '' then 'true' else 'false' end)Attend_DtVisibility " +
            $", (case when Selected_Dt <> '' then 'true' else 'false' end)Selected_DtVisibility " +
            $", (case when joindt <> '' then 'true' else 'false' end)joindtVisibility " +
            $" from JobFairEmployersVacancies";
        jobfairsEmployersVacancieslists = jobFairEmployersVacanciesDatabase.GetJobFairEmployersVacancies(query).ToList();
        listview_jobfairdetails.ItemsSource = jobfairsEmployersVacancieslists;

        lbl_header.Text = "Job Fair Details \nDistrict - " + distt + "\nExchange - " + exchange + "\nEmployer - " + employer;

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Preferences.Get("Active", 0) == 0)
        {
            Footer_Image_Source = new string[4] { "ic_homeselected.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };
            App.pages = new Page[] { new HomePage(), new UpdateEmpStatusPage(), new ViewAllowances(), new MorePage() };
        }
        else if (Preferences.Get("Active", 0) == 1)
        {
            Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };
            App.pages = new Page[] { new HomePage(), new UpdateEmpStatusPage(), new ViewAllowances(), new MorePage() };

        }
        else if (Preferences.Get("Active", 0) == 2)
        {
            Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowanceselected.png", "ic_more.png" };
            App.pages = new Page[] { new HomePage(), new UpdateEmpStatusPage(), new ViewAllowances(), new MorePage() };

        }
        else if (Preferences.Get("Active", 0) == 3)
        {
            Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_moreselected.png" };
            App.pages = new Page[] { new HomePage(), new UpdateEmpStatusPage(), new ViewAllowances(), new MorePage() };

        }

        Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
        Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#337ab7");

    }
    private void Tab_Home_Tapped(object sender, EventArgs e)
    {
        Preferences.Set("Active", 0);
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mainWindow.Page = new NavigationPage(new HomePage());

            });
        }
    }

    private void Tab_UpdateEmpStatus_Tapped(object sender, EventArgs e)
    {
        Preferences.Set("Active", 1);
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mainWindow.Page = new NavigationPage(new UpdateEmpStatusPage());

            });
        }
    }

    private void Tab_ViewAllowances_Tapped(object sender, EventArgs e)
    {
        Preferences.Set("Active", 2);

        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Loading_activity.IsVisible = true;
                var service = new HitServices();
                int resposne_GetAllowanceDetails = await service.GetAllowanceDetails(RegNo);
                Loading_activity.IsVisible = false;
                if (resposne_GetAllowanceDetails == 200)
                {
                    mainWindow.Page = new NavigationPage(new ViewAllowances());
                }
                else
                {
                    await App.ShowAlertBox(App.AppName, "No Allowances Found!");
                }

            });
        }
    }

    private void Tab_Settings_Tapped(object sender, EventArgs e)
    {
        Preferences.Set("Active", 3);
        if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                mainWindow.Page = new NavigationPage(new MorePage());
            });
        }
    }

}
