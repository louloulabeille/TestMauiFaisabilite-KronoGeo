using System;
using System.Collections.Generic;
using System.Text;

namespace TestMauiFaisabilite_KronoGeo.Infrastructure.Photo_Video
{
    public class Photo
    {
        /// <summary>
        /// demande de permission pour accéder à la caméra du téléphone
        /// pour faire des photos
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> PermissionPhoto()
        {
            var cameraPermissionsRequest = await Permissions.RequestAsync<Permissions.Camera>();
            return cameraPermissionsRequest == PermissionStatus.Granted;
        } 
    }
}
