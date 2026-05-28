using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
#if ANDROID21_0_OR_GREATER
using TestMauiFaisabilite_KronoGeo.Platforms.Android;
#endif





#if ANDROID
using Android.Hardware.Camera2;

#endif

namespace TestMauiFaisabilite_KronoGeo.ViewModels
{
    public partial class TakePhotoViewModel : ObservableObject
    {
        #region properties public
        [ObservableProperty]
        public partial ImageSource? Photo { get; set; }
        [ObservableProperty]
        public partial bool DisabledViewPhoto { get; set; } = false;

        [ObservableProperty]
        public partial bool DisabledPhoto { get; set; } = true;
        #endregion


        #region public method
        [RelayCommand]
        public async Task TakePhotoAsync()
        {
            byte[]? jpeg = null;
            DisabledViewPhoto = false;
            DisabledPhoto = true;
#if ANDROID
            // demander la permission CAMERA (runtime)
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                return;

            // instancier le service Android (Camera2Service doit être dans Platforms/Android)
            var service = new TakePhotoAndroid(Android.App.Application.Context);

            // obtenir les octets JPEG
            
            try
            {
                jpeg = await service.TakePhotoAndroidAsync();
            }
            catch (Exception ex)
            {
                // gestion d'erreur simple (adapter selon besoins)
                Console.WriteLine("Erreur lors de la prise de photo \n" + ex.Message);
            }


        #endif
            if (jpeg != null && jpeg.Length > 0)
            {
                Photo = ImageSource.FromStream(() => new MemoryStream(jpeg));
                DisabledViewPhoto = true;
                DisabledPhoto = false;
            }
        }
        #endregion

    }
}
