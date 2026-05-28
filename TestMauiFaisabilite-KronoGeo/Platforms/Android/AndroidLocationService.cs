using Android.Content;
using Android.Locations;
using Location = Android.Locations.Location;
using Application = Android.App.Application;

namespace TestMauiFaisabilite_KronoGeo.Platforms.Android
{
    public class AndroidLocationService
    {
        #region private properties
        private readonly LocationManager? _locationManager;
        private readonly LocationListener _locationListener = new();
        #endregion

        #region public properties

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;

        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        public AndroidLocationService()
        {
            // Récupération du service de localisation natif d'Android
            _locationManager = (LocationManager?)Application.Context.GetSystemService(Context.LocationService);

            // S'abonner au retour du listener
            _locationListener.OnLocationChangedAction = (location) =>
            {
                // Ici vous récupérez la position précise
                double latitude = location.Latitude;
                double longitude = location.Longitude;
                double altitude = location.Altitude;
                float accuracy = location.Accuracy; // Précision en mètres

                if (accuracy > 12) // Seuil de précision (12 mètres dans cet exemple)
                {
                    return; // Ignorer cette position
                }
                // -- appel de l'événement pour le code partagé --
                LocationChanged?.Invoke(this,
                    new GeolocationLocationChangedEventArgs(new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude, altitude) 
                    {
                        Accuracy = (double)accuracy,
                        Speed = (double)location.Speed,
                        Timestamp = DateTime.Now,
                        Course = (double)location.Bearing,
                        VerticalAccuracy = (double)location.VerticalAccuracyMeters
                    }));

                // TODO: Envoyer ces données à votre code partagé (via un événement ou Messenger)
            };
        }

        public void StartLocationUpdates()
        {
            if (_locationManager == null) return;

            try
            {
                // On force l'utilisation exclusive du GPS (Haute précision)
                string provider = LocationManager.GpsProvider;

                if (_locationManager.IsProviderEnabled(provider))
                {
                    // Paramètres de mise à jour :
                    // 1000 : Intervalle minimum en millisecondes (1 seconde)
                    // 1 : Distance minimale en mètres avant notification (1 mètre)
                    _locationManager.RequestLocationUpdates(
                    provider,
                    5000,
                    5,
                    _locationListener
                    );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Le fournisseur GPS n'est pas activé sur l'appareil.");
                    throw new FeatureNotEnabledException("Le fournisseur GPS n'est pas activé sur l'appareil.");
                }
            }
            catch (Java.Lang.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de permission : {ex.Message}");
                throw new PermissionException($"Permission de localisation refusée. Veuillez accorder les permissions nécessaires. {ex.Message}");
            }
        }

        public void StopLocationUpdates()
        {
            if (_locationManager != null && _locationListener != null)
            {
                // Très important pour économiser la batterie quand on n'en a plus besoin
                _locationManager.RemoveUpdates(_locationListener);
            }
        }

    }
}
