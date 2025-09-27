using X10Card.Models;
using X10Card.webapi;

namespace X10Card;

public partial class ViewAllVacanciesPage : ContentPage
{
    public Label[] Footer_Labels;
    public string[] Footer_Image_Source;
    public Image[] Footer_Images;
    UserDetailsDatabase userDetailsDatabase =  new UserDetailsDatabase();
    List<UserDetails> userDetailslist = new List<UserDetails>();

    AllVacancyDetailsDatabase allVacancyDetailsDatabase = new AllVacancyDetailsDatabase();
    List<AllVacancyDetails> allVacancyDetailslist=new List<AllVacancyDetails>();
    List<AllVacancyDetails>  allVacancyDetailslistpopup= new List<AllVacancyDetails>();
    
    string RegNo="";
    public ViewAllVacanciesPage()
	{
		InitializeComponent();
        Footer_Labels = new Label[4] { Tab_Home_Label, Tab_UpdateEmpStatus_Label, Tab_ViewAllowances_Label, Tab_Settings_Label };
        Footer_Images = new Image[4] { Tab_Home_Image, Tab_UpdateEmpStatus_Image, Tab_ViewAllowances_Image, Tab_Settings_Image };
        Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };

        userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Footer_Image_Source = new string[4] { "ic_homeselected.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };

        Footer_Images[Preferences.Get("Active", 0)].Source = Footer_Image_Source[Preferences.Get("Active", 0)];
        Footer_Labels[Preferences.Get("Active", 0)].TextColor = Color.FromArgb("#337ab7");

        loaddata();
    }

    void loaddata()
    {
        userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
        lbl_header.Text = userDetailslist.ElementAt(0).CandiName;
        if (userDetailslist != null && userDetailslist.Any())
        {
            RegNo = userDetailslist.ElementAt(0).RegNo??"";
        }

        allVacancyDetailslist = allVacancyDetailsDatabase.GetAllVacancyDetails("Select EmpID,EmployerName,count(*) as TotalVacancy from AllVacancyDetails group by EmployerName").ToList();
        DetailedList.ItemsSource = allVacancyDetailslist;
    }

    private void DetailedList_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null) return;
        allVacancyDetailslistpopup = new List<AllVacancyDetails>();
        var currentRecord = e.Item as AllVacancyDetails;
        if (currentRecord == null) return;

        string emplyrid = currentRecord.EmpID??"";

        allVacancyDetailslistpopup = allVacancyDetailsDatabase
            .GetAllVacancyDetails($"Select * from AllVacancyDetails where EmpID = '{emplyrid}'")
            .ToList();

        // Reset ItemsSource to force refresh
      
        listview_vacancy.ItemsSource = null;
        listview_vacancy.ItemsSource = allVacancyDetailslistpopup;

        if (allVacancyDetailslistpopup.Count > 0)
            lbl_popupvacancy.Text = "Vacancy Details - " + allVacancyDetailslistpopup[0].EmployerName;
        else
            lbl_popupvacancy.Text = "No Vacancy Details Found";

        popupvacancy.IsVisible = true;

        // Clear selection so next tap always fires
        ((ListView)sender).SelectedItem = null;
    }


    private void popupvacancyCancel_Clicked(object sender, EventArgs e)
    {
        popupvacancy.IsVisible = false;
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
