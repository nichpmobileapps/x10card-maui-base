namespace X10Card
{
    public partial class MainPage : ContentPage
    {   
        public MainPage()
        {
            InitializeComponent();
        }

        private void btn_submit_Clicked(object sender, EventArgs e)
        {            
            if (Application.Current?.Windows.FirstOrDefault() is Window mainWindow)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                   // mainWindow.Page = new NavigationPage(new SsoLoginPage());
                    Navigation.PushAsync(new SsoLoginPage());
                });
            }
        }
    }
}
