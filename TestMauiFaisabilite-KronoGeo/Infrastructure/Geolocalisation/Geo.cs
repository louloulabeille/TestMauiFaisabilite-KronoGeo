using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Devices.Sensors;


namespace TestMauiFaisabilite_KronoGeo.Infrastructure.Geolocalisation
{
    public class Geo : IDisposable
    {
        #region public properties
        public Location? DefaultPoint { get; set; }
        public Location? ActualPoint { get; set; }
        public string Status { get; set; } = string.Empty;
        #endregion

        #region private properties
        private CancellationTokenSource _cancelTokenSource = new();
        private bool _isCheckingLocation = false;
        private bool _isListeningLocation = false;
        private List<Location> _geos { get; set; } = [];
        private GeolocationAccuracy _accuracy { get; set; } = GeolocationAccuracy.Best;
        private int _delay { get; set; } = 1000;
        #endregion

        #region public constructeur
        public Geo()
        {
        }

        public Geo(GeolocationAccuracy accuracy)
        {
            _accuracy = accuracy;
        }
        public Geo(GeolocationAccuracy accuracy, int delay)
        {
            _accuracy = accuracy;
            _delay = delay;
        }

        public Geo(int delay)
        {
            _delay = delay;
        }
        #endregion


        #region pulic methods
        /// <summary>
        /// retourne la dernière localisation connue de l'appareil, si disponible. Cette méthode est rapide 
        /// et consomme peu de ressources,
        /// mais elle peut retourner une localisation obsolète ou null si aucune localisation n'est disponible.
        /// </summary>
        /// <returns></returns>
        public async Task GetCachedLocation()
        {
            try
            {
                DefaultPoint = await Geolocation.Default.GetLastKnownLocationAsync();

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                throw;
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                throw;
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
                // Unable to get location
            }
        }

        /// <summary>
        /// Donne la localisation en cours
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new (GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                ActualPoint = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public async Task<Location?> GetCurrentLocationReturn(CancellationToken token)
        {
            try
            {
                GeolocationRequest request = new(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                return await Geolocation.Default.GetLocationAsync(request, token);

            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                _isListeningLocation = false;
                throw;
            }
        }


        /// <summary>
        /// Method d'écoute GPS en mode foreground, elle vérifie la localisation toutes les secondes
        /// et l'ajoute à la liste des géolocalisations.
        /// StartListeningForegroundAsync ne marche pas à cause de l'occurency trop mauvaise
        /// sous Android, qui fait que les mises à jour de localisation ne sont pas reçues à temps, ce qui rend l'utilisation de cet API peu fiable 
        /// pour les applications nécessitant des mises à jour fréquentes de la localisation ou précise.
        /// </summary>
        public async void OnStartCheckingLocation()
        {
            _isListeningLocation = true;
            _cancelTokenSource = new CancellationTokenSource();
            _geos.Clear();

            var token = _cancelTokenSource.Token;
            await Task.Run(async () =>
            {
                
                while (_isListeningLocation)
                {
                    var location = await GetCurrentLocationReturn(token);
                    location?.Timestamp = DateTime.Now; // - on prend le timestamp de réception de la location pas UtcNow pour éviter les problèmes de fuseau horaire
                    if (location is not null && !_geos.Contains(location))
                    {
                        _geos.Add(location);
                    }
                        
                    await Task.Delay(_delay); // Attendre le delais seconde avant de vérifier à nouveau 1 seconde minimun
                }
            }, token);
            _isListeningLocation = false;
        }

        /// <summary>
        /// Permet d'annuler une demande de localisation en cours
        /// </summary>
        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

        /// <summary>
        /// Début de l'écoute des changements de localisation en mode foreground.
        /// Les mises à jour de localisation seront reçues via l'événement LocationChanged.
        /// </summary>
        public async void OnStartListening()
        {
            try
            {
                _geos.Clear();
                Geolocation.LocationChanged += Geolocation_LocationChanged;
                // Using GeolocationAccuracy.Medium as a balance between accuracy and power consumption.
                // Developers can adjust this value to High or Low based on their specific requirements.
                // GeolocationAccuracy.High le mettre en high pour une meilleure précision,
                // mais cela peut consommer plus de batterie.
                var request = new GeolocationListeningRequest(_accuracy, TimeSpan.FromSeconds(1));
                
                var success = await Geolocation.StartListeningForegroundAsync(request);

                Status = success
                    ? "Started listening for foreground location updates"
                    : "Couldn't start listening";
            }
            catch (Exception ex)
            {
                // Unable to start listening for location changes
                throw;  //- gestion des exceptions à faire au niveau de l'appelant
            }
        }

        /// <summary>
        /// enregistrement des enregistrements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            // Process e.Location to get the new location
            e.Location.Timestamp = DateTime.Now; // - on prend le timestamp de réception de la location pas UtcNow pour éviter les problèmes de fuseau horaire
            _geos.Add(e.Location);
        }

        /// <summary>
        /// arrete les écoutes de changements de positions en mode foreground.
        /// Les mises à jour de localisation ne seront plus reçues après l'appel de cette méthode.
        /// </summary>
        public void OnStopListening()
        {
            try
            {
                Geolocation.LocationChanged -= Geolocation_LocationChanged;
                Geolocation.StopListeningForeground();
                Status = "Stopped listening for foreground location updates";
            }
            catch (Exception ex)
            {
                // Unable to stop listening for location changes
                throw;  //- gestion des exceptions à faire au niveau de l'appelant
            }
        }

        /// <summary>
        /// retourne si les services d’emplacement ont été activés sur l’appareil.
        /// </summary>
        /// <returns></returns>
        public static bool IsGeoListening() => Geolocation.IsEnabled;

        /// <summary>
        /// retourne la dernière location
        /// </summary>
        /// <returns></returns>
        public Location LastLocation()
        {
            return _geos.LastOrDefault() ?? new();
        }

        /// <summary>
        /// retourne toutes les locations enregistrées depuis le début de l'écoute en mode foreground.
        /// </summary>
        /// <returns></returns>
        public string AllLocations()
        {
            if (_geos.Count == 0)
                return string.Empty;

            StringBuilder sb = new();
            foreach (var geo in _geos)
            {
                sb.AppendLine(geo.ToString());
            }
            return sb.ToString();
        }


        #endregion

        #region Medthod Interface Idisposable
        public void Dispose()
        {
            CancelRequest();
            if (Geolocation.IsListeningForeground)
            {
                Geolocation.StopListeningForeground();
            }
            GC.SuppressFinalize(this);
        }
        #endregion



        

       
    }
}
