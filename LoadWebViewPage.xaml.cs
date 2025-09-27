using System;
using X10Card.webapi;

namespace X10Card;

public partial class LoadWebViewPage : ContentPage
{
	public LoadWebViewPage(string url)
	{
		InitializeComponent();
        

        if (!string.IsNullOrEmpty(url))
        {
            Loading_activity.IsVisible = true;
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                webview_loaddata.Source = url;
                Loading_activity.IsVisible = false;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.ShowAlertBox(App.AppName, "No Internet Connection Found");

                });
            }
            Loading_activity.IsVisible = false;
        }
        else
        {
            lbl_heading.Text = "Privacy Policy";
            Loading_activity.IsVisible = true;
            /*var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    double version = double.Parse(DeviceInfo.VersionString);
                    if (version >= 7.1)
                    {
                        var service=new HitServices();
                        webview_loaddata.Source = service.privacypolicyurl;
                    }
                    else
                    {
                        webview_loaddata.Source = "file:///android_asset/X10CardPrivacyPolicy.html";
                    }
                }
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    webview_loaddata.Source = "file:///android_asset/X10CardPrivacyPolicy.html";
                }
                else
                {
                    webview_loaddata.Source = "X10CardPrivacyPolicy.html";
                }
            }*/
            var service = new HitServices();
            webview_loaddata.Source = service.privacypolicyurl;
            Loading_activity.IsVisible = false;
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("Active", 0);
        Application.Current.MainPage = new NavigationPage(new HomePage());
    }
}
