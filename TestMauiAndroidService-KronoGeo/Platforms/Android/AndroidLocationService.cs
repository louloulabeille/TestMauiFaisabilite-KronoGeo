using Android.Content;
using Android.Locations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Location = Android.Locations.Location;

using Application = Android.App.Application;

namespace TestMauiAndroidService_KronoGeo.Platforms.Android
{
    public class AndroidLocationService
    {
        private LocationManager? _locationManager;
        private LocationListener _locationListener;

        public List<Location> Locations { get; private set; } = new List<Location>();

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;

        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;

        public AndroidLocationService()
        {
            // Récupération du service de localisation natif d'Android
            _locationManager = (LocationManager?)Application.Context.GetSystemService(Context.LocationService);
            _locationListener = new();

            

        // S'abonner au retour du listener
        _locationListener.OnLocationChangedAction = (location) =>
            {
                // Ici vous récupérez la position précise
                double latitude = location.Latitude;
                double longitude = location.Longitude;
                float accuracy = location.Accuracy; // Précision en mètres

                // -- appel de l'événement pour le code partagé --
                LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude, accuracy)));

                Locations.Add(location);
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
                    2000,
                    4,
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
