using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TestMauiFaisabilite_KronoGeo.Infrastructure.Geolocalisation;
#if ANDROID
using TestMauiFaisabilite_KronoGeo.Platforms.Android;
#endif
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace TestMauiFaisabilite_KronoGeo;

public partial class GeolocalisationPage : ContentPage
{
    #region private properties
    private readonly Geo _geolocalisation = new();
    private bool _isGeolocalisation = false;

    private CancellationTokenSource? _geoCts;
    private Task? _geoTask;

#if ANDROID
    // - appel d'une classe spécifique à android
    private readonly AndroidLocationService _androidLocationService = new();
#endif
    #endregion

    //public string Geolocation { get; set; } = string.Empty;

    public GeolocalisationPage()
	{
        InitializeComponent();
    }


    #region private methods

    private async void OnMapLoad(object sender, EventArgs e)
    {
        /*var mapControl = new MapControl();
        mapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());*/
        //Content = mapControl;

        //MainThread.BeginInvokeOnMainThread(async () =>
        //{
        await _geolocalisation.GetCurrentLocation();
        // - actualisation sur le thread principal
        if (_geolocalisation.ActualPoint is not null)
        {
            MapSpan mapSpan = MapSpan.FromCenterAndRadius(_geolocalisation.ActualPoint, Distance.FromMeters(500));
            googleMap.MoveToRegion(mapSpan);
        }
        //});
        
    }

    private void ChargeMapOpenStreetMap() 
    {
        
        /*mapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

        //var map = mapView.Map;

        var bottomLeft = SphericalMercator.FromLonLat(-122.514926, 37.708075);
        var topRight = SphericalMercator.FromLonLat(-122.357031, 37.832371);

        //var region = new MRect(bottomLeft.x, bottomLeft.y,topRight.x, topRight.y);
        var region = new MRect(20, 40);

        //map.Navigator.PanLimits = region;
        mapView.Map.Navigator.ZoomToBox(region);
        if (_geolocalisation.ActualPoint is not null)
            mapView.Map.Navigator.CenterOn(_geolocalisation.ActualPoint.Longitude, _geolocalisation.ActualPoint.Latitude);
        */
    }

    private async void OnGetLocationClicked(object? sender, EventArgs e)
	{
        // - vérification des permissions de suivi de localisation
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted) return;

        if (Geo.IsGeoListening())
        {
#if ANDROID21_0_OR_GREATER
            
            _androidLocationService?.LocationChanged += (s, args) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LocationLabel.Text += args.Location.ToString() + "\n";
                    if (googleMap.MapElements.Count == 0)
                    {
                        var polyne = new Polyline()
                        {
                            StrokeColor = Colors.Blue,
                            StrokeWidth = 12,
                        };
                        polyne.Geopath.Add(args.Location);
                        googleMap.MapElements.Add(polyne);
                    }
                    else
                    {
                        var element = googleMap.MapElements.FirstOrDefault() as Polyline;
                        element?.Geopath.Add(args.Location);
                    }

                });
            };
            _androidLocationService?.StartLocationUpdates();
#else
            _geolocalisation.OnStartCheckingLocation();
            _isGeolocalisation = true;

            await Task.Run(async () =>
            {
                while (_isGeolocalisation)
                {
                    await Task.Delay(1000);
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LocationLabel.Text = $"location: \n {_geolocalisation.AllLocations()}";
                        if (googleMap.MapElements.Count == 0)
                        {
                            googleMap.MapElements.Add(new Polyline()
                            {
                                StrokeColor = Colors.Blue,
                                StrokeWidth = 12,
                                Geopath =
                            {
                                _geolocalisation.LastLocation()
                            }
                            });
                        }
                        else
                        {
                            var element = googleMap.MapElements.FirstOrDefault() as Polyline;
                            element?.Geopath.Add(_geolocalisation.LastLocation());
                        }
                    });
                }
            });

            // - start suivi geolocalisation
            /*_geolocalisation.OnStartListening();

            _isGeolocalisation = true;
            Geolocation.LocationChanged += (s, args) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LocationLabel.Text = $"location: \n {_geolocalisation.AllLocations()}";
                    if (googleMap.MapElements.Count == 0)
                    {
                        googleMap.MapElements.Add(new Polyline()
                        {
                            StrokeColor = Colors.Blue,
                            StrokeWidth = 12,
                            Geopath =
                            {
                                _geolocalisation.LastLocation()
                            }
                        });
                    }
                    else
                    {
                        var element = googleMap.MapElements.FirstOrDefault() as Polyline;
                        element?.Geopath.Add(_geolocalisation.LastLocation());
                    }
                        
                });
            };*/

            /*_geoCts = new CancellationTokenSource();
            var token = _geoCts.Token;

            // on stocke la tâche au lieu de la disposer immédiatement
            _geoTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && _isGeolocalisation)
                {
                    try
                    {
                        await Task.Delay(1000, token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LocationLabel.Text = $"location: \n {_geolocalisation.AllLocations()}";
                    });
                }
            }, token);*/
#endif
        }
    }

    private void OnStopLocationClicked(object? sender, EventArgs e)
    {
        if (Geo.IsGeoListening())
        {
        #if ANDROID
            _androidLocationService?.StopLocationUpdates();
        #endif
            // - stop suivi geolocalisation
            //_geolocalisation.OnStopListening();
            _geolocalisation.Dispose();
            _isGeolocalisation = false;
            /*_geoCts?.Cancel();
            _geoCts?.Dispose();
            _geoCts = null;
            _geoTask = null;*/

            /*LocationLabel.Text += "\n" + "************";
            LocationLabel.Text += $"{_geolocalisation.AllLocations()}";*/
        }
    }
    #endregion
}