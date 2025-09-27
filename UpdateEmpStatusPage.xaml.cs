using X10Card.Models;
using X10Card.webapi;

namespace X10Card;

public partial class UpdateEmpStatusPage : ContentPage
{
    public Label[] Footer_Labels;
    public string[] Footer_Image_Source;
    public Image[] Footer_Images;
    UserDetailsDatabase userDetailsDatabase= new UserDetailsDatabase();
    List<UserDetails> userDetailslist=new List<UserDetails>();
    EmploymentStatusDatabase employmentStatusDatabase= new EmploymentStatusDatabase();
    List<EmploymentStatus> employmentStatuslist=new List<EmploymentStatus>();
    SubEmploymentStatusDatabase subEmploymentStatusDatabase=new SubEmploymentStatusDatabase();
    List<SubEmploymentStatus> empsectorlist=new List<SubEmploymentStatus>();
    List<SubEmploymentStatus> emptypelist=new List<SubEmploymentStatus>();
    string RegNo="";
    string EmploymentStatusName="", EmploymentStatusCode="", EmploymentSectorCode = "", EmploymenttypeCode="";
    public UpdateEmpStatusPage()
	{
		InitializeComponent();


     

        Footer_Labels = new Label[4] { Tab_Home_Label, Tab_UpdateEmpStatus_Label, Tab_ViewAllowances_Label, Tab_Settings_Label };
        Footer_Images = new Image[4] { Tab_Home_Image, Tab_UpdateEmpStatus_Image, Tab_ViewAllowances_Image, Tab_Settings_Image };
        Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };
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
        loadData();
    }

    void loadData()
    {
        userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();
        RegNo = userDetailslist.ElementAt(0).RegNo ?? "";
        string username = userDetailslist.ElementAt(0).CandiName ?? "";
        lbl_header.Text = username;

        employmentStatuslist = employmentStatusDatabase.GetEmploymentStatus("Select * from EmploymentStatus order by EmpStatDesc Asc").ToList();
        Picker_EmploymentStatus.Title = "Select Employment Status";
        Picker_EmploymentStatus.ItemsSource = employmentStatuslist;
        Picker_EmploymentStatus.ItemDisplayBinding = new Binding("EmpStatDesc");
    }

    private void Picker_EmploymentStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Picker_EmploymentStatus.SelectedIndex != -1)
        {

            EmploymentStatusCode = employmentStatuslist.ElementAt(Picker_EmploymentStatus.SelectedIndex).EmpStatCd??"";
            EmploymentStatusName = employmentStatuslist.ElementAt(Picker_EmploymentStatus.SelectedIndex).EmpStatDesc ?? "";

            if (EmploymentStatusName.Equals("Employed"))
            {
                stack_sector.IsVisible = true;
                stack_type.IsVisible = true;
                stack_sector.IsVisible = true;
                stack_organisationr.IsVisible = false;

                empsectorlist = subEmploymentStatusDatabase.GetSubEmploymentStatus("Select distinct SubEmpStatDesc,SubEmpStatCd  from SubEmploymentStatus order by SubEmpStatDesc Asc").ToList();
                Picker_EmploymentSector.Title = "Select Employment Sector";
                Picker_EmploymentSector.ItemsSource = empsectorlist;
                Picker_EmploymentSector.ItemDisplayBinding = new Binding("SubEmpStatDesc");
            }
            else
            {
                stack_sector.IsVisible = false;
                stack_type.IsVisible = false;
                stack_sector.IsVisible = false;
                stack_organisationr.IsVisible = false;
                EmploymentSectorCode = string.Empty;
                EmploymenttypeCode = string.Empty;
                editor_organisation.Text = "";
            }
        }
    }

    private void Picker_EmploymentSector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Picker_EmploymentSector.SelectedIndex != -1)
        {

            EmploymentSectorCode = empsectorlist.ElementAt(Picker_EmploymentSector.SelectedIndex).SubEmpStatCd ?? "";

            emptypelist = subEmploymentStatusDatabase.GetSubEmploymentStatus($"Select distinct SSubEmpStatDesc ,SSubEmpStatCd " +
                $"from SubEmploymentStatus where SubEmpStatCd='{EmploymentSectorCode}' order by SSubEmpStatDesc ASC").ToList();
            Picker_employmentype.Title = "Select Employment Type";
            Picker_employmentype.ItemsSource = emptypelist;
            Picker_employmentype.ItemDisplayBinding = new Binding("SSubEmpStatDesc");
        }
    }

    private void Picker_employmentype_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Picker_employmentype.SelectedIndex != -1)
        {

            EmploymenttypeCode = emptypelist.ElementAt(Picker_employmentype.SelectedIndex).SSubEmpStatCd ?? "";
        }
    }

    private async void btn_updatestatus_Clicked(object sender, EventArgs e)
    {

        if (await checkvalidation())
        {
            var service=new HitServices();
            Loading_activity.IsVisible = true;
            await service.UpdateEmployementstatus(RegNo, EmploymentStatusCode, EmploymentSectorCode, EmploymenttypeCode);
            Loading_activity.IsVisible = false;
            Preferences.Set("Active", 0);            
            if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    mainWindow.Page = new NavigationPage(new HomePage());

                });
            }
        }
    }

    async Task<bool> checkvalidation()
    {
        try
        {

            if (Picker_EmploymentStatus.SelectedIndex == -1)
            {
                await DisplayAlert(App.AppName, "Select Employment Status", "Close");
                Picker_EmploymentStatus.Focus();
                return false;
            }
            if (EmploymentStatusName.Equals("Employed"))
            {

                if (Picker_EmploymentSector.SelectedIndex == -1)
                {
                    await DisplayAlert(App.AppName, "Select Employment Sector", "Close");
                    Picker_EmploymentSector.Focus();
                    return false;
                }


                if (Picker_employmentype.SelectedIndex == -1)
                {
                    await DisplayAlert(App.AppName, "Select Employment Type", "Close");
                    Picker_employmentype.Focus();
                    return false;
                }
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
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
}


