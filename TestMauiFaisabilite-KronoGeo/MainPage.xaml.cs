using Microsoft.Maui.Devices.Sensors;
using TestMauiFaisabilite_KronoGeo.Infrastructure.Geolocalisation;

namespace TestMauiFaisabilite_KronoGeo
{
    public partial class MainPage : ContentPage
    {
        #region public properties
        public Geo Geolocalisation { get; set; } = new Geo();
        #endregion

        #region constructeur 
        public MainPage()
        {
            InitializeComponent();
        }
        #endregion


        #region public methods
        public async void OnGetLocationClicked(object sender, EventArgs e)
        {
            try
            {
                await Geolocalisation.GetCachedLocation();
                locationLabel.Text = Geolocalisation.DefaultPoint?.ToString();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                locationLabel.Text = $"Permission denied {pEx.Message}";
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public async void OnGetActualLocationClicked(object sender, EventArgs e)
        {
            try
            {
                await Geolocalisation.GetCurrentLocation();
                actualLabel.Text = Geolocalisation.ActualPoint?.ToString();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                actualLabel.Text = $"Permission denied {pEx.Message}";
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }


        public async void OnGetAllLocationsClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(GeolocalisationPage));
            }
            catch (Exception ex)
            {
                // Unable to cancel location request
            }
        }

        #endregion

        #region method evenhandler clicked
        /// <summary>
        /// Event à programmer Clicked sur la ToolBar de la MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            // ajoute une fenêtre au dessus de l'autre
            //await this.Navigation.PushAsync(new TestPhotoVideo());
            await Shell.Current.GoToAsync(nameof(TakePhoto));
        }

        #endregion
    }
}
