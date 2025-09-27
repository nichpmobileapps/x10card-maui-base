using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;
using X10Card.Models.NewUserRegistration;

namespace X10Card
{
    public partial class App : Application
    {
        public static Page[]? pages;
        public static string DBName = "employment.db";
        public static string base64Image = "";
        public static string PhotoBase64 = "";
        public static FileResult? imagememroystreamsource;
        public static string Btn_Close = "Close";
        public static string AppName = "X10 Card";
        public static string NoInternet_ = "No Internet Connection Found";
        public static List<PersonalDetails> personalDetailsList = new List<PersonalDetails>();

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (!string.IsNullOrEmpty(Preferences.Get("LoggedIn", "")))
            {
                if (Preferences.Get("LoggedIn", "").Equals("Y"))
                {
                    return new Window(new NavigationPage(new PostLoginDashboardPage()));
                }
                else
                {
                    return new Window(new NavigationPage(new MainPage()));
                }
            }
            else
            {
                return new Window(new NavigationPage(new MainPage()));
            }
        }

        public static bool isAlphabetonly(string strtocheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s]+$");
            return rg.IsMatch(strtocheck);
        }
        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,.@()/-]*$");
            return rg.IsMatch(strToCheck);
        }
        public static bool isNumeric(string strToCheck)
        {
            Regex rg = new Regex("^[0-9]+$");
            return rg.IsMatch(strToCheck);
        }

        public static bool isDecimal(string strToCheck)
        {
            Regex rg = new Regex("^\\d+(\\.\\d{1,2})?$");
            return rg.IsMatch(strToCheck);
        }

        public static bool ValidateEmail(string strToCheck)
        {

            Regex rg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return rg.IsMatch(strToCheck);
        }

        public bool isvalidpassword(string strToCheck)
        {
            Regex rg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return rg.IsMatch(strToCheck);
        }
        public static string ConvertToBase64(Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }
        public static async Task<string> GetIPAddress()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                try
                {
                    var client = new HttpClient();
                    var response = await client.GetAsync("https://api.myip.com");
                    var result = await response.Content.ReadAsStringAsync();
                    JObject parsed = JObject.Parse(result);
                    var m = parsed["ip"];
                    return m?.ToString() ?? string.Empty;

                }
                catch
                {
                    var ip = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();
                    return ip?.ToString() ?? string.Empty;
                }
            }
            else
            {
                var ip = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();
                return ip?.ToString() ?? string.Empty;
            }
        }
        public static async Task<string[]> UploadPhoto(Image image)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await ShowAlertBox(AppName, "No Camera Found");
                    return new string[3] { string.Empty, string.Empty, string.Empty };

                }

                Preferences.Default.Set("INeedCamera", "Y");

                var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"Camera{DateTime.Now:yyyyMMddHHmmss}.jpg"
                });

                if (photo == null)
                {
                    return Array.Empty<string>();
                }

                // Save to stream
                await using var stream = await photo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                imagememroystreamsource = photo;

                byte[] byteArray = memoryStream.ToArray();
                PhotoBase64 = Convert.ToBase64String(byteArray);

                string extension = Path.GetExtension(photo.FullPath)?.TrimStart('.').ToLower() ?? "jpg";

                image.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));

                return new string[3] { PhotoBase64, extension, "Camera" };
            }
            catch (Exception ex)
            {
                await ShowAlertBox(AppName, ex.Message);
                return Array.Empty<string>();
            }
        }



        public static async Task ShowAlertBox(string title, string message)
        {
            if (Current != null && Current.Windows.Any())
            {
                var currentPage = Current.Windows[0].Page; // Get the root page of the first window
                if (currentPage != null)
                {
                    await currentPage.DisplayAlert(title, message, Btn_Close);
                }
                else
                {
                    // Handle cases where the page might not be ready (e.g., very early app startup)
                    System.Diagnostics.Debug.WriteLine("Warning: No active page found to display alert.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Warning: Application.Current or its Windows collection is null.");
            }
        }



        public static async Task ShowUpdateAppAlertBox(string title, string message, string updbtn)
        {
            if (Current != null && Current.Windows.Any())
            {
                var currentPage = Current.Windows[0].Page; // Get the root page of the first window
                if (currentPage != null)
                {
                    await currentPage.DisplayAlert(title, message, updbtn);
                }
                else
                {
                    // Handle cases where the page might not be ready (e.g., very early app startup)
                    System.Diagnostics.Debug.WriteLine("Warning: No active page found to display alert.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Warning: Application.Current or its Windows collection is null.");
            }
        }
    }
}
