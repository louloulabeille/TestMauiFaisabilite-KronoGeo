using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Location = Android.Locations.Location;

namespace TestMauiFaisabilite_KronoGeo.Platforms.Android
{
    public class LocationListener : Java.Lang.Object, ILocationListener
    {
        // Action à appeler lorsque la localisation change - event
        public Action<Location>? OnLocationChangedAction { get; set; }

        public void OnLocationChanged(global::Android.Locations.Location location)
        {
            if (location is not null)
            {
                /*double latitude = location.Latitude;
                double longitude = location.Longitude;
                double altitude = location.Altitude;
                float accuracy = location.Accuracy;
                Console.WriteLine($"Location changed: Latitude={latitude}, Longitude={longitude}, Altitude={altitude}, Accuracy={accuracy}");*/
                OnLocationChangedAction?.Invoke(location);
            }


        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
        {
            throw new NotImplementedException();
        }
    }
}
