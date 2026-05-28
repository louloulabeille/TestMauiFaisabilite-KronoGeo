/*using System;
using System.Collections.Generic;
using System.Text;


#if ANDROID
using Android.Gms.Location;
using Android.OS;
#endif

namespace TestMauiFaisabilite_KronoGeo.Infrastructure.Geolocalisation
{
    public class GeoAndroid
    {
#if ANDROID
        public async Task StartHighAccuracyTracking()
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (activity == null) return;

            var fusedLocationClient = LocationServices.GetFusedLocationProviderClient(activity);

            // Configuration moderne pour forcer le GPS haute précision
            var locationRequest = new LocationRequest.Builder(Android.Gms.Location.Priority.PriorityHighAccuracy, 2000) // 2 secondes
            .SetMinUpdateIntervalMillis(1000) // Rafraîchissement ultra-rapide
            .SetMaxUpdateDelayMillis(2000)
            .Build();

            var callback = new PreciseLocationCallback();

            try
            {
                await fusedLocationClient.RequestLocationUpdatesAsync(locationRequest, callback, Looper.MainLooper!);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur GPS : {ex.Message}");
            }
        }
#endif
    }

#if ANDROID
    public class PreciseLocationCallback : LocationCallback
    {
        public override void OnLocationResult(LocationResult result)
        {
            if (result?.LastLocation == null) return;

            var loc = result.LastLocation;
            // Précision en mètres
            System.Diagnostics.Debug.WriteLine($"Précision: {loc.Accuracy}m | Lat: {loc.Latitude} | Long: {loc.Longitude}");
        }
    }
#endif
}
*/