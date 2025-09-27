using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using X10Card.Models;

namespace X10Card;

public partial class SsoLoginPage : ContentPage
{
    string serviceId = "10000088";
    string transactionidfromaddhaar = "";

    string devicePlatform = DeviceInfo.Platform.ToString();
    string appversiono;

   
    string verifiedaadhaarotpflagValue = "";
    string chosenUserName = "";
    string FetchedSSOId = "";
    string FetchedMobileNo = "";
/*    string otpStatus = "";
    string FetchedEncryptedstring = "";
    string FetchedDencryptedstring = "";
    string sso_id = "";
    string mobile = "";
    string encryptedStr = "";
    string decryptedstr = "";
    string IPaddress = "";
    string username = "";
    string name = "";
    string email = "";*/
    string UserID = "", RegNo = "", otpStatus = "";


    public SsoLoginPage()
    {
        InitializeComponent();
        rb_himaccessid.IsChecked = true;
        stack_himaccessid.IsVisible = true;
        VersionTracking.Track();
        var currentVersion = VersionTracking.CurrentVersion;
        appversiono = currentVersion;

    }

    private void rb_loginoptions_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (rb_himaccessid.IsChecked)
        {
            entry_HimAccessId.Text = "";
            entry_HimAccessPassword.Text = "";
            entry_HimAccessId.IsReadOnly = false;
            entry_HimAccessPassword.IsReadOnly = false;
            btn_getotphimaccess.IsVisible = true;
            stack_enterotphimaccess.IsVisible = false;
            entry_OTPhimaccess.Text = "";
            stack_himaccessid.IsVisible = true;
            stack_mobile.IsVisible = false;
            stack_others.IsVisible = false;
        }
        else if (rb_mobile.IsChecked)
        {
            entry_mobile.Text = "";
            entry_OTP.Text = "";
            btn_getotp.IsVisible = true;
            entry_mobile.IsReadOnly = false;
            stack_enterotp.IsVisible = false;
            stack_himaccessid.IsVisible = false;
            stack_mobile.IsVisible = true;
            stack_others.IsVisible = false;

        }
        else if (rb_others.IsChecked)
        {
            stack_himaccessid.IsVisible = false;
            stack_mobile.IsVisible = false;
            entry_otherUsername.Text = "";
            entry_otherPassword.Text = "";
            entry_aadhaarno.Text = "";
            entry_Email.Text = "";
            entry_emailPassword.Text = "";


            stack_others.IsVisible = true;

            bindpicker();
        }
    }
    void bindpicker()
    {
        var loginTypes = new List<string>
            {
                "User Name",
                "Aadhar Number",
                "Email"
            };
        picker_others.Title = "Select Login Type";
        picker_others.ItemsSource = loginTypes;
        picker_others.SelectedIndex = 0;

    }
    private void Picker_others_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_others.SelectedIndex != -1)
        {
            string selected = picker_others.SelectedItem.ToString() ?? "";
            if (selected.Equals("User Name"))
            {
                entry_otherUsername.IsEnabled = true;
                entry_otherPassword.IsEnabled = true;
                stack_othersusername.IsVisible = true;
                stack_aadhaar.IsVisible = false;
                stack_emailidpassword.IsVisible = false;

            }
            else if (selected.Equals("Aadhar Number"))
            {
                stack_othersusername.IsVisible = false;
                entry_aadhaarno.IsEnabled = true;
                stack_aadhaar.IsVisible = true;
                stack_emailidpassword.IsVisible = false;

            }
            else if (selected.Equals("Email"))
            {
                entry_Email.IsEnabled = true;
                entry_emailPassword.IsEnabled = true;
                stack_othersusername.IsVisible = false;
                stack_aadhaar.IsVisible = false;
                stack_emailidpassword.IsVisible = true;
            }
        }
    }


    //--------------------------------------------------------HIMACCESS--------------------------------------------------

    private async void sendOTPHimaccess_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationHimaccessid())
        {
            Loading_activity.IsVisible = true;
            int response_HimAccessIDPassword = await HimAccessIDPassword();
            Loading_activity.IsVisible = false;
            if (response_HimAccessIDPassword == 200)
            {
                string SSO = FetchedSSOId;

                entry_HimAccessId.IsReadOnly = true;
                entry_HimAccessPassword.IsReadOnly = true;
                btn_getotphimaccess.IsVisible = false;
                stack_enterotphimaccess.IsVisible = true;

                /* if (himaccessotpStatus.Equals("true"))
                 {
                     entry_HimAccessId.IsReadOnly = true;
                     entry_HimAccessPassword.IsReadOnly = true;
                     btn_getotphimaccess.IsVisible = false;
                     stack_enterotphimaccess.IsVisible = true;
                 }
                 else
                 {
                     await App.ShowAlertBox("OTP", "Web API unable to send OTP");
                 }*/
            }
        }
    }

    async Task<bool> checkvalidationHimaccessid()
    {
        try
        {
            if (string.IsNullOrEmpty(entry_HimAccessId.Text))
            {
                await DisplayAlert(App.AppName, "Enter Him Access Id", "Close");
                return false;
            }
            if (entry_HimAccessId.Text.Length < 10)
            {
                await DisplayAlert(App.AppName, "Enter 10 digit Him Access Id", "Close");
                return false;
            }
            if (!App.isNumeric(entry_HimAccessId.Text))
            {
                await DisplayAlert(App.AppName, "Him Access Id should be Numeric", "Close");
                return false;
            }
            if (string.IsNullOrEmpty(entry_HimAccessPassword.Text))
            {
                await DisplayAlert(App.AppName, "Enter Him Access Password", "Close");
                return false;
            }
            if (!App.isAlphaNumeric(entry_HimAccessPassword.Text))
            {
                await DisplayAlert(App.AppName, "Him Access Password Should be AlphaNumeric", "Close");
                return false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }

    private async void btn_submitotphimaccess_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationotpHimaccessid())
        {
            Loading_activity.IsVisible = true;
            int response_verifyOtpAsync = await verifyOtpAsync(FetchedMobileNo, entry_OTPhimaccess.Text);
            Loading_activity.IsVisible = false;
            if (response_verifyOtpAsync == 200)
            {
                Loading_activity.IsVisible = true;
                var service = new UserRegistrationApi();
                int response_login = await service.Login_SSO_ID(FetchedSSOId);
                Loading_activity.IsVisible = false;
                if (response_login == 200)
                {
                    Preferences.Set("LoggedIn", "Y");
                    getvalues();
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
    }
    private void getvalues()
    {

        UserID = Preferences.Get("UserID", "");
        RegNo = Preferences.Get("RegNo", "");
    }
    async Task<bool> checkvalidationotpHimaccessid()
    {
        try
        {


            if (string.IsNullOrEmpty(entry_OTPhimaccess.Text))
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }
            if (entry_OTPhimaccess.Text.Length == 0)
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }



            if (!App.isNumeric(entry_OTPhimaccess.Text))
            {
                await DisplayAlert(App.AppName, "OTP Should be Numeric", "Close");
                return false;
            }



        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }

    //-------------------------------------------------------------Mobile
    private async void sendOTP_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationmobile())
        {
            Loading_activity.IsVisible = true;
            int resposne_sendotp = await SendOtpAsync(entry_mobile.Text);
            if (resposne_sendotp == 200)
            {
                btn_getotp.IsVisible = false;
                stack_enterotp.IsVisible = true;
                entry_mobile.IsReadOnly = true;
            }
            Loading_activity.IsVisible = false;
        }
    }

    async Task<bool> checkvalidationmobile()
    {
        try
        {


            if (string.IsNullOrEmpty(entry_mobile.Text))
            {
                await DisplayAlert(App.AppName, "Enter Mobile Number", "Close");
                return false;
            }
            if (entry_mobile.Text.Length == 0)
            {
                await DisplayAlert(App.AppName, "Enter 10 digit Mobile Number", "Close");
                return false;
            }
            if (entry_mobile.Text.Length < 10)
            {
                await DisplayAlert(App.AppName, "Enter 10 digit Mobile Number", "Close");
                return false;
            }
            if (long.Parse(entry_mobile.Text) < 6000000000)
            {
                await DisplayAlert(App.AppName, "Invalid Mobile No. Mobile no. must start from 6,7,8,9.", "Close");
                return false;
            }

            if (!App.isNumeric(entry_mobile.Text))
            {
                await DisplayAlert(App.AppName, "Mobile No. Should be Numeric", "Close");
                return false;
            }



        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }

    private async void VerifyOTP_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationotp())
        {
            entry_OTP.IsReadOnly = true;
            Loading_activity.IsVisible = true;
            int response_verifyOtpAsync = await verifyOtpAsync(entry_mobile.Text, entry_OTP.Text);
            if (response_verifyOtpAsync == 200)
            {
                btn_submitotp.IsVisible = false;
                stack_usernamemobile.IsVisible = true;
            }
            Loading_activity.IsVisible = false;
        }
    }

    private void picker_usernamesmobile_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_usernamesmobile.SelectedIndex != -1)
        {
            var selectedUser = picker_usernamesmobile.SelectedItem as MultipleUser;
            if (selectedUser != null)
            {
                FetchedSSOId = selectedUser.id ?? "";
                //selectedusernamemobile = selectedUser.userName??"";
            }
        }
    }

    private async void btn_submitusernamemobile_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(FetchedSSOId))
        {
            // selectedSSOIdmobile  //pass ssoid to get registration details 

            Loading_activity.IsVisible = true;
            var service = new UserRegistrationApi();
            int response_login = await service.Login_SSO_ID(FetchedSSOId);
            Loading_activity.IsVisible = false;
            if (response_login == 200)
            {
                Preferences.Set("LoggedIn", "Y");
                getvalues();
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

    async Task<bool> checkvalidationotp()
    {
        try
        {


            if (string.IsNullOrEmpty(entry_OTP.Text))
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }
            if (entry_OTP.Text.Length == 0)
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }



            if (!App.isNumeric(entry_OTP.Text))
            {
                await DisplayAlert(App.AppName, "OTP Should be Numeric", "Close");
                return false;
            }



        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }
    //-------------------------------------------------------username password------------------
    private async void btnUsernamePassword_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationUSerName())
        {
            Loading_activity.IsVisible = true;
            int response_LoginwithUsernamePassword = await LoginwithUsernamePassword();
            Loading_activity.IsVisible = false;
            if (response_LoginwithUsernamePassword == 200)
            {
                Loading_activity.IsVisible = true;
                var service = new UserRegistrationApi();
                int response_login = await service.Login_SSO_ID(FetchedSSOId);
                Loading_activity.IsVisible = false;
                if (response_login == 200)
                {
                    Preferences.Set("LoggedIn", "Y");
                    getvalues();
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
    }

    async Task<bool> checkvalidationUSerName()
    {
        try
        {
            if (string.IsNullOrEmpty(entry_otherUsername.Text))
            {
                await DisplayAlert(App.AppName, "Enter Username", "Close");
                return false;
            }
            if (!App.isAlphaNumeric(entry_otherUsername.Text))
            {
                await DisplayAlert(App.AppName, "Username Should be AlphaNumeric", "Close");
                return false;
            }
            if (string.IsNullOrEmpty(entry_otherPassword.Text))
            {
                await DisplayAlert(App.AppName, "Enter Password", "Close");
                return false;
            }
            if (!App.isAlphaNumeric(entry_otherPassword.Text))
            {
                await DisplayAlert(App.AppName, "Password Should be AlphaNumeric", "Close");
                return false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }

    private async void btnaadharno_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationaadhaarno())
        {
            Loading_activity.IsVisible = true;
            int response_getotpaadhaar = await LoginWithAadhaarNumber();
            Loading_activity.IsVisible = false;
            if (response_getotpaadhaar == 200)
            {
                stack_enterotpaadhaar.IsVisible = true;
                entry_aadhaarno.IsReadOnly = true;
            }
        }
    }

    async Task<bool> checkvalidationaadhaarno()
    {
        try
        {

            if (string.IsNullOrEmpty(entry_aadhaarno.Text))
            {
                await DisplayAlert(App.AppName, "Enter Aadhaar Number", "Close");
                return false;
            }
            if (entry_aadhaarno.Text.Length == 0)
            {
                await DisplayAlert(App.AppName, "Enter 12 digit Aadhaar Number", "Close");
                return false;
            }
            if (entry_aadhaarno.Text.Length < 12)
            {
                await DisplayAlert(App.AppName, "Enter 12 digit Aadhaar Number", "Close");
                return false;
            }


            if (!App.isNumeric(entry_aadhaarno.Text))
            {
                await DisplayAlert(App.AppName, "Aadhaar Number should be Numeric", "Close");
                return false;
            }



        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }

    private async void btnVerifyAadhaarOTP_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationotpaadhaar())
        {
            Loading_activity.IsVisible = true;
            int resposne_VerifyAadhaarOTP = await VerifyAadhaarOTP();
            Loading_activity.IsVisible = false;
            if (resposne_VerifyAadhaarOTP == 200)
            {
                if (verifiedaadhaarotpflagValue.Equals("true"))
                {

                    Loading_activity.IsVisible = true;
                    var service = new UserRegistrationApi();
                    int response_login = await service.Login_SSO_ID(FetchedSSOId);
                    Loading_activity.IsVisible = false;
                    if (response_login == 200)
                    {
                        Preferences.Set("LoggedIn", "Y");
                        getvalues();
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
        }
    }

    async Task<bool> checkvalidationotpaadhaar()
    {
        try
        {
            if (string.IsNullOrEmpty(entry_aadhaarOTP.Text))
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }
            if (entry_aadhaarOTP.Text.Length == 0)
            {
                await DisplayAlert(App.AppName, "Enter OTP", "Close");
                return false;
            }

            if (!App.isNumeric(entry_aadhaarOTP.Text))
            {
                await DisplayAlert(App.AppName, "OTP Should be Numeric", "Close");
                return false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }


    private async void imgbtn_searchusername_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationEmailonly())
        {
            Loading_activity.IsVisible = true;
            int resposne_Checkmultipleuser = await CheckEmailMultipleUser();
            Loading_activity.IsVisible = false;
            if (resposne_Checkmultipleuser == 200)
            {
                entry_Email.IsReadOnly = true;
                stack_otheremailpassword.IsVisible = true;

            }
        }
    }

    private void Picker_email_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (picker_usernames.SelectedIndex != -1)
        {
            /* var selectedUser = picker_usernames.SelectedItem as MultipleUser;
             if (selectedUser != null)
             {
                 selectedSSOIdmobile = selectedUser.id ?? "";
                 selectedusernamemobile = selectedUser.userName ?? "";
             }*/
            var selectedUser = picker_usernames.SelectedItem as EmailUser;
            if (selectedUser != null)
            {
                chosenUserName = selectedUser.userName ?? "";
                //chosenEmail = selectedUser.email;
            }
        }

    }

    async Task<bool> checkvalidationEmailID()
    {
        try
        {

            if (string.IsNullOrEmpty(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Enter Email", "Close");
                return false;
            }

            if (!App.isAlphaNumeric(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Email should be AlphaNumeric", "Close");
                return false;
            }
            if (!App.ValidateEmail(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Invalid Email Id.", "Close");
                return false;
            }
            if (string.IsNullOrEmpty(entry_emailPassword.Text))
            {
                await DisplayAlert(App.AppName, "Enter Password", "Close");
                return false;
            }

            if (!App.isAlphaNumeric(entry_emailPassword.Text))
            {
                await DisplayAlert(App.AppName, "Password should be AlphaNumeric", "Close");
                return false;
            }


        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }
    async Task<bool> checkvalidationEmailonly()
    {
        try
        {

            if (string.IsNullOrEmpty(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Enter Email", "Close");
                return false;
            }

            if (!App.isAlphaNumeric(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Email should be AlphaNumeric", "Close");
                return false;
            }
            if (!App.ValidateEmail(entry_Email.Text))
            {
                await DisplayAlert(App.AppName, "Invalid Email Id.", "Close");
                return false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.AppName, ex.Message, "Close");
            return false;
        }
        return true;
    }
    private async void btnEmailIDPassword_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationEmailID())
        {
            Loading_activity.IsVisible = true;
            int response_LoginWithEmailIDPassword = await LoginWithEmailIDPassword();
            Loading_activity.IsVisible = false;
            if (response_LoginWithEmailIDPassword == 200)
            {
                Loading_activity.IsVisible = true;
                int resposne_sendotp = await SendOtpAsync(FetchedMobileNo);
                if (resposne_sendotp == 200)
                {
                    btn_submitemailpassword.IsVisible = false;
                    stack_enterotpemail.IsVisible = true;
                    picker_usernames.IsEnabled = false;
                    entry_Email.IsReadOnly = true;
                    entry_emailPassword.IsReadOnly = true;
                }
                Loading_activity.IsVisible = false;
            }
        }
    }

    private async void VerifyOTPemail_Clicked(object sender, EventArgs e)
    {
        if (await checkvalidationotpHimaccessid())
        {
            Loading_activity.IsVisible = true;
            int response_verifyOtpAsync = await verifyOtpAsync(FetchedMobileNo, entry_OTPemail.Text);
            Loading_activity.IsVisible = false;
            if (response_verifyOtpAsync == 200)
            {
                Loading_activity.IsVisible = true;
                var service = new UserRegistrationApi();
                int response_login = await service.Login_SSO_ID(FetchedSSOId);
                Loading_activity.IsVisible = false;
                if (response_login == 200)
                {
                    Preferences.Set("LoggedIn", "Y");
                    getvalues();
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
    }

    //---------------------------------------------Himaccess---------------------------
    private async Task<int> HimAccessIDPassword()
    {
        try
        {
            var client = new HttpClient();
            string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/user/mobile-app-login";
            var obj = new AESCrypto();
            var otpRequestData = new
            {
                userName = entry_HimAccessId.Text,//"8817015340"
                email = "",
                aadhaar = "",
                login_type = "himaccessid",
                service_id = serviceId,
                browserDetails = new
                {
                    userAgent = "MobileApp",
                    clientIp = await App.GetIPAddress(),
                    browserName = "MobileApp",
                    browserVersion = appversiono,
                    osName = devicePlatform,
                    location = new { city = "NA", lat = 0, lng = 0 }
                },
                password = obj.Encrypt(entry_HimAccessPassword.Text)//"Password@123456"
            };

            var objFinalData = new finalData
            {
                data = obj.Encrypt(JsonConvert.SerializeObject(otpRequestData)),
                service_id = serviceId
            };
            string requestJson = JsonConvert.SerializeObject(objFinalData);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(otpUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                return (int)response.StatusCode;
            }
            else
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(responseJson);

                string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                var result = JObject.Parse(responsedata);
                FetchedSSOId = result["sso_id"]?.ToString() ?? "";
                FetchedMobileNo = result["mobileNo"]?.ToString() ?? "";
                otpStatus = result["otpStatus"]?.ToString().ToLower() ?? "";
                //sms_api = result["sms_api"]?.ToString().ToLower() ?? "";
                return 200;
            }
        }
        catch
        {
            return 500;
        }
    }

    //-----------------------------------------------MobileNo---------------------------

    private async Task<int> SendOtpAsync(string mobileno)
    {
        try
        {
            using (var client = new HttpClient())
            {

                string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/send-mobile-otp-new";

                AESCrypto Obj = new AESCrypto();

                var otpRequestData = new
                {
                    mobile = mobileno
                };

                var objfinaldata = new finalData();
                objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
                objfinaldata.service_id = serviceId;

                string contjson = JsonConvert.SerializeObject(objfinaldata);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(contjson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(otpUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    await DisplayAlert(App.AppName, jsonString, "Close");
                    return 200;
                }
                else
                {
                    await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                    return (int)response.StatusCode;
                }
            }
        }
        catch
        {

            return 500;
        }

    }

    private async Task<int> verifyOtpAsync(string mobileno, string otp)
    {
        try
        {
            using (var client = new HttpClient())
            {

                string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/user-check-mobile-email-Otp";

                AESCrypto Obj = new AESCrypto();

                var otpRequestData = new
                {
                    username = mobileno,
                    otp = Obj.Encrypt(otp),
                    service_id = serviceId
                };

                var objfinaldata = new finalData();
                objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
                objfinaldata.service_id = serviceId;


                string contjson = JsonConvert.SerializeObject(objfinaldata);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(contjson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(otpUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(jsonString);

                    string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                    var apiObj = JsonConvert.DeserializeObject<ApiResponseVerifyMobileOTP>(responsedata);
                    //{"id":1867,"userName":"nityanandverma9379","email":"nityanand.verma777@gmail.com","mobile":"8219211012","name":"Nityanand Verma"}]}
                    List<MultipleUser> multipleuserhimaccesslist = apiObj?.multipleUser ?? new List<MultipleUser>();
                    picker_usernamesmobile.ItemsSource = multipleuserhimaccesslist;
                    picker_usernamesmobile.ItemDisplayBinding = new Binding("userName");
                    picker_usernamesmobile.SelectedIndex = 0;

                    return 200;
                }
                else
                {
                    await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                    return (int)response.StatusCode;
                }
            }
        }
        catch
        {
            return 500;
        }
    }

    public class ApiResponseVerifyMobileOTP
    {
        public List<MultipleUser>? multipleUser { get; set; }
    }

    //-----------------------------------------------Username------------------------------
    private async Task<int> LoginwithUsernamePassword()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/user/login";
                AESCrypto Obj = new AESCrypto();
                var otpRequestData = new
                {
                    username = entry_otherUsername.Text,
                    password = Obj.Encrypt(entry_otherPassword.Text),
                };
                /* var otpRequestData = new
                 {

                     username = "akhilshyam5999",
                     password = Obj.Encrypt("Test@123"),

                 };*/
                var objfinaldata = new finalData();
                objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
                objfinaldata.service_id = serviceId;
                string contjson = JsonConvert.SerializeObject(objfinaldata);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(contjson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(otpUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(jsonString);
                    string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                    var obj = JObject.Parse(responsedata);
                    FetchedSSOId = obj["sso_id"]?.ToString() ?? "";
                    FetchedMobileNo = obj["mobileNo"]?.ToString() ?? "";
                    otpStatus = obj["otpStatus"]?.ToString() ?? "";
                    //{ "sso_id":2005,"mobileNo":"7018134985","otpStatus":false,"sms_api":false}
                    //await DisplayAlert(App.AppName, responsedata, "Close");
                    return 200;
                }
                else
                {
                    return (int)response.StatusCode;

                }
            }
        }
        catch
        {
            return 500;
        }
    }

    //-----------------------------------------------Aadhaar--------------------------------

    private async Task<int> LoginWithAadhaarNumber()
    {
        try
        {
            var client = new HttpClient();
            string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/user/mobile-app-login";
            AESCrypto Obj = new AESCrypto();
            var otpRequestData = new
            {


                userName = entry_aadhaarno.Text,
                email = "",
                aadhaar = entry_aadhaarno.Text,
                login_type = "Aadhaar",
                service_id = serviceId,
                browserDetails = new
                {
                    userAgent = "MobileApp",
                    clientIp = await App.GetIPAddress(),
                    browserName = "MobileApp",
                    browserVersion = appversiono,
                    osName = devicePlatform,
                    location = new { city = "NA", lat = 0, lng = 0 }
                },
                password = ""
            };

            var objfinaldata = new finalData();
            objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
            objfinaldata.service_id = serviceId;


            string contjson = JsonConvert.SerializeObject(objfinaldata);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(contjson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(otpUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                JObject parsed = JObject.Parse(jsonString);
                string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                var obj = JObject.Parse(responsedata);

                transactionidfromaddhaar = obj["transactionId"]?.ToString() ?? "";
                FetchedSSOId = obj["sso_id"]?.ToString() ?? "";
                FetchedMobileNo = obj["mobileNo"]?.ToString() ?? "";

                return 200;
            }
            else
            {
                await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                return (int)response.StatusCode;
            }

        }
        catch
        {
            return 500;
        }
    }
    private async Task<int> VerifyAadhaarOTP()
    {
        try
        {
            using (var client = new HttpClient())
            {

                string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/aadhaar-otp-authentication";

                AESCrypto Obj = new AESCrypto();

                var otpRequestData = new
                {


                    aadhaarNumber = entry_aadhaarno.Text,
                    otp = Obj.Encrypt(entry_aadhaarOTP.Text),
                    transactionId = transactionidfromaddhaar,

                };

                var objfinaldata = new finalData();
                objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
                objfinaldata.service_id = serviceId;


                string contjson = JsonConvert.SerializeObject(objfinaldata);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(contjson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(otpUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(jsonString);
                    verifiedaadhaarotpflagValue = parsed["flag"]?.ToString().ToLower() ?? "";
                    // string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");

                    //await DisplayAlert(App.AppName, flagValue, "Close");
                    return 200;
                }
                else
                {
                    await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                    return (int)(int)response.StatusCode;
                }
            }
        }
        catch
        {
            return 500;
        }
    }

    //------------------------------------------------Email Id and password---------------------
    private async Task<int> CheckEmailMultipleUser()
    {
        try
        {
            var client = new HttpClient();
            string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/check-email";
            AESCrypto Obj = new AESCrypto();
            var otpRequestData = new
            {
                email = entry_Email.Text,
            };
            /*            var otpRequestData = new
                        {
                            email = "nityanand.verma77@gmail.com",
                        };*/

            var objfinaldata = new finalData();
            objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
            objfinaldata.service_id = serviceId;

            string contjson = JsonConvert.SerializeObject(objfinaldata);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(contjson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(otpUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                JObject parsed = JObject.Parse(jsonString);
                string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                var apiObj = JsonConvert.DeserializeObject<ApiResponseEmail>(responsedata);

                List<EmailUser> multipleuseremaillist = apiObj?.emailUsers ?? new List<EmailUser>();

                if (multipleuseremaillist.Any())
                {
                    picker_usernames.ItemsSource = multipleuseremaillist;
                    picker_usernames.ItemDisplayBinding = new Binding("userName");
                    picker_usernames.SelectedIndex = 0;
                }
                return 200;
            }
            else
            {
                await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                return (int)response.StatusCode;
            }
        }
        catch
        {
            return 500;
        }
    }
    private async Task<int> LoginWithEmailIDPassword()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string otpUrl = "https://himstaging2.hp.gov.in/nodeapi/user/mobile-app-login";

                AESCrypto Obj = new AESCrypto();

                var otpRequestData = new
                {
                    userName = chosenUserName,
                    email = entry_Email.Text,
                    aadhaar = "",
                    login_type = "Email",
                    service_id = serviceId,
                    browserDetails = new
                    {
                        userAgent = "MobileApp",
                        clientIp = await App.GetIPAddress(),
                        browserName = "MobileApp",
                        browserVersion = appversiono,
                        osName = devicePlatform,
                        location = new { city = "NA", lat = 0, lng = 0 }
                    },
                    password = Obj.Encrypt(entry_emailPassword.Text),

                };
                /* var otpRequestData = new
                 {


                     userName = "vermanityanand5928",
                     email = "nityanand.verma77@gmail.com",
                     aadhaar = "",
                     login_type = "Email",
                     service_id = serviceId,
                     browserDetails = new
                     {
                         userAgent = "MobileApp",
                         clientIp = await  App.GetIPAddress(),
                         browserName = "MobileApp",
                         browserVersion = appversiono,
                         osName = devicePlatform,
                         location = new { city = "NA", lat = 0, lng = 0 }
                     },
                     password = Obj.Encrypt("Password@123456"),

                 };*/

                var objfinaldata = new finalData();
                objfinaldata.data = Obj.Encrypt(JsonConvert.SerializeObject(otpRequestData));
                objfinaldata.service_id = serviceId;


                string contjson = JsonConvert.SerializeObject(objfinaldata);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(contjson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(otpUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(jsonString);
                    string responsedata = AESCrypto.Decrypt(parsed["data"]?.ToString() ?? "");
                    var result = JObject.Parse(responsedata);
                    FetchedSSOId = result["sso_id"]?.ToString() ?? "";
                    FetchedMobileNo = result["mobileNo"]?.ToString() ?? "";
                    otpStatus = result["otpStatus"]?.ToString().ToLower() ?? "";
                    // sms_api = result["sms_api"]?.ToString().ToLower() ?? "";

                    return 200;
                }
                else
                {
                    await DisplayAlert(App.AppName, response.StatusCode.ToString(), "Close");
                    return (int)response.StatusCode;
                }
            }
        }
        catch
        {
            return 500;
        }
    }

    public class ApiResponseEmail
    {
        public int emailCount { get; set; }
        public List<EmailUser>? emailUsers { get; set; }
    }


    //------------------------------------------------------------------------------------------------------
    public class finalData()
    {
        public string? data { get; set; }
        public string? service_id { get; set; }
    }
    //web api parameters

}



