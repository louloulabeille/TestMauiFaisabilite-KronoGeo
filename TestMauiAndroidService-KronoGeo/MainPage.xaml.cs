#if ANDROID
using TestMauiAndroidService_KronoGeo.Platforms.Android;
#endif
namespace TestMauiAndroidService_KronoGeo
{
    public partial class MainPage : ContentPage
    {
#if ANDROID
        private readonly AndroidLocationService _location;
#endif
        public MainPage()
        {
            InitializeComponent();
#if ANDROID
            _location = new AndroidLocationService();
#endif
        }

        public void StartLocationService_Clicked(object sender, EventArgs e)
        {
            locationLabel.Text = string.Empty;
#if ANDROID
            _location.StartLocationUpdates();
#endif
        }

        public void StopLocationService_Clicked(object sender, EventArgs e)
        {
#if ANDROID
            foreach (var item in _location.Locations) {
                locationLabel.Text += $"Lat: {item.Latitude}, Lon: {item.Longitude}, Précision: {item.Accuracy}m\n";
            }
            
            _location.StopLocationUpdates();
#endif

        }
    }
}
