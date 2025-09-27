using X10Card.Models;
using X10Card.webapi;

namespace X10Card;

public partial class AllowanceTypeDetailsPage : ContentPage
{
    public Label[] Footer_Labels;
    public string[] Footer_Image_Source;
    public Image[] Footer_Images;

    UserDetailsDatabase userDetailsDatabase=new UserDetailsDatabase();
    List<UserDetails> userDetailslist=new List<UserDetails>();

    AllowanceTransactionsDatabase allowanceTransactionsDatabase=new AllowanceTransactionsDatabase();
    List<AllowanceTransactions> allowanceTransactionslist=new List<AllowanceTransactions>();
    List<AllowanceTransactions> finacyrlist=new List<AllowanceTransactions>();
    string AllowanceId, ApplicationNo="";
    string financeyear="";
    string financialyearquery="";
    string username = "", RegNo = "";

    public AllowanceTypeDetailsPage(string allowanceId, string Appno, string allowancename)
	{
		InitializeComponent();
        ApplicationNo = Appno;
        AllowanceId = allowanceId;

        userDetailsDatabase = new UserDetailsDatabase();

        allowanceTransactionsDatabase = new AllowanceTransactionsDatabase();
        userDetailslist = userDetailsDatabase.GetUserDetails("Select * from UserDetails").ToList();

        

        username = userDetailslist.ElementAt(0).CandiName??"";
        RegNo = userDetailslist.ElementAt(0).RegNo ?? "";
        lbl_header.Text = username + " - " + allowancename + " Details";

        Footer_Labels = new Label[4] { Tab_Home_Label, Tab_UpdateEmpStatus_Label, Tab_ViewAllowances_Label, Tab_Settings_Label };
        Footer_Images = new Image[4] { Tab_Home_Image, Tab_UpdateEmpStatus_Image, Tab_ViewAllowances_Image, Tab_Settings_Image };
        Footer_Image_Source = new string[4] { "ic_home.png", "ic_update.png", "ic_allowance.png", "ic_more.png" };


        loaddata();
    }

    void loaddata()
    {
        finacyrlist = allowanceTransactionsDatabase.GetAllowanceTransactions(
                        $"Select distinct(Finyear) from AllowanceTransactions " +
                        $"where AllowanceId='{AllowanceId}' and ApplicationNo='{ApplicationNo}' ").ToList();
        picker_financeyear.Title = "Select Financial Year";
        picker_financeyear.ItemsSource = finacyrlist;
        picker_financeyear.ItemDisplayBinding = new Binding("Finyear");
        picker_financeyear.SelectedIndex = 0;
        financeyear = finacyrlist.ElementAt(picker_financeyear.SelectedIndex).Finyear??"";

        financialyearquery = $"Select * from AllowanceTransactions" +
                             $" where AllowanceId = '{AllowanceId}' and ApplicationNo='{ApplicationNo}' and Finyear ='{financeyear}'";

        allowanceTransactionslist = allowanceTransactionsDatabase.GetAllowanceTransactions(financialyearquery).ToList();
        listview_allowancetypedetails.ItemsSource = allowanceTransactionslist;


    }

    private void picker_financeyear_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_financeyear.SelectedIndex != -1)
        {
            financeyear = finacyrlist.ElementAt(picker_financeyear.SelectedIndex).Finyear ?? "";
            financialyearquery = $"Select * from AllowanceTransactions" +
           $" where AllowanceId = '{AllowanceId}' and ApplicationNo='{ApplicationNo}' and Finyear ='{financeyear}'";

            allowanceTransactionslist = allowanceTransactionsDatabase.GetAllowanceTransactions(financialyearquery).ToList();
            listview_allowancetypedetails.ItemsSource = allowanceTransactionslist;
        }
    }


    private void listview_allowancetypedetails_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var currentRecord = e.Item as AllowanceTransactions;
        string MonthYY = currentRecord?.MonthYY?.ToString() ?? string.Empty;

        popupDetails.IsVisible = true;
        allowanceTransactionslist = allowanceTransactionsDatabase.GetAllowanceTransactions($"Select * from AllowanceTransactions " +
            $" where AllowanceId = '{AllowanceId}' and ApplicationNo='{ApplicationNo}' and MonthYY='{MonthYY}'").ToList();
        Content_DetailedList.ItemsSource = allowanceTransactionslist;
        TitleofContent.Text = username + " - " + allowanceTransactionslist.ElementAt(0).AllowanceDesc + "\nBill Details For Month-" + allowanceTransactionslist.ElementAt(0).MonthYY;
    }


    private void btn_close_Clicked(object sender, EventArgs e)
    {
        popupDetails.IsVisible = false;
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
                    await App.ShowAlertBox(App.AppName, "No Allowances FOund!");
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
