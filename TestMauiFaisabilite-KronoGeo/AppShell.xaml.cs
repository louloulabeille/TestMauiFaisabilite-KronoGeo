namespace TestMauiFaisabilite_KronoGeo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(GeolocalisationPage), typeof(GeolocalisationPage));
            Routing.RegisterRoute(nameof(TestPhotoVideo), typeof(TestPhotoVideo));
            Routing.RegisterRoute(nameof(TakePhoto), typeof(TakePhoto));
        }

    }
}
