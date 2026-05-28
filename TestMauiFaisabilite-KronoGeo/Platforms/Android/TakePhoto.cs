#if ANDROID
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using Java.Nio;
using Java.Util.Concurrent;
using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;


namespace TestMauiFaisabilite_KronoGeo.Platforms.Android
{

    public class TakePhotoAndroid(Context context)
    {
        #region private properties
        private readonly Context _context = context;

        private const string _version = "android24.0";
        #endregion

        /// <summary>
        /// method qui prend la photo et l'enregistre en mémoire,
        /// elle utilise Camera2 API pour capturer une image JPEG et retourne les octets de l'image
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TimeoutException"></exception>
        [SupportedOSPlatform(_version)]
        public async Task<byte[]?> TakePhotoAndroidAsync()
        {
            var tcs = new TaskCompletionSource<byte[]>();
            var mgr = _context.GetSystemService(Context.CameraService) as CameraManager;

            // Choisir la caméra arrière
            string? cameraId = null;

            if (mgr?.GetCameraIdList() is null) return null;

            foreach (var id in mgr.GetCameraIdList())
            {
                var chars = mgr.GetCameraCharacteristics(id);
                Integer? facing = chars.Get(CameraCharacteristics.LensFacing) as Integer;
                if (facing != null && facing.IntValue() == (int)LensFacing.Back)
                {
                    cameraId = id;
                    break;
                }
            }
            if (cameraId == null) throw new InvalidOperationException("No back camera found");

            // Préparer ImageReader pour JPEG
            int width = 1920, height = 1080;
            var imageReader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);

            // Handler thread pour callbacks caméra
            var handlerThread = new HandlerThread("Camera2Thread");
            handlerThread.Start();
            var handler = new Handler(handlerThread.Looper!);

            // Récupérer l'image quand disponible
            imageReader.SetOnImageAvailableListener(new ImageReaderListener(tcs), handler);

            CameraDevice? camera = null;
            CameraCaptureSession? session = null;

            var stateCb = new CameraStateCallback(
                opened: dev => camera = dev,
                disconnected: dev => { dev?.Close(); },
                error: (dev, err) => { dev?.Close(); if (!tcs.Task.IsCompleted) tcs.SetException(new System.Exception("Camera error: " + err)); }
            );

            // Ouvrir la caméra (assurez-vous d'avoir la permission CAMERA)
            mgr.OpenCamera(cameraId, stateCb, handler);

            // Attendre ouverture
            await Task.Run(async () =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (camera == null && sw.ElapsedMilliseconds < 5000) await Task.Delay(50);
                if (camera == null) throw new System.TimeoutException("Timeout waiting for camera to open");
            });

            if (imageReader.Surface is null) return null;
            // Créer CaptureRequest pour still capture
            var captureRequestBuilder = camera?.CreateCaptureRequest(CameraTemplate.StillCapture);
            captureRequestBuilder?.AddTarget(imageReader.Surface);
            if (CaptureRequest.ControlAeMode is null)
                return null;

            captureRequestBuilder?.Set(CaptureRequest.ControlAeMode, (int)ControlAEMode.On);

            // Créer session
            var sessionCb = new CameraCaptureStateCallback(
                configured: s =>
                {
                    session = s;
                    try
                    {
                        if (captureRequestBuilder is null) return;
                        var captureCallback = new CameraCaptureCallback((result) => { /* no-op */ });
                        session.Capture(captureRequestBuilder.Build(), captureCallback, handler);
                    }
                    catch (System.Exception ex) { if (!tcs.Task.IsCompleted) tcs.SetException(ex); }
                },
                failed: reason => { if (!tcs.Task.IsCompleted) tcs.SetException(new System.Exception("Session failed: " + reason)); }
            );

            // ... préparer surface ...
            /*var outputs = new List<OutputConfiguration> { new OutputConfiguration(imageReader.Surface) };
            var executor = Executors.NewSingleThreadExecutor();
            // Java.Util.Concurrent.IExecutor
            if (executor is null) return null;
            var sessionConfig = new SessionConfiguration(0, outputs, executor, new MyStateCallback());
            camera?.CreateCaptureSession(sessionConfig);*/

            camera?.CreateCaptureSession( [imageReader.Surface], sessionCb!, handler);

            // Récupérer octets JPEG depuis tcs (déclenché par ImageReaderListener)
            var jpeg = await tcs.Task;

            // Nettoyage
            try { session?.Close(); } catch { }
            try { camera?.Close(); } catch { }
            imageReader.Close();
            handlerThread.QuitSafely();

            return RotateImage(jpeg,90f);
        }

        /// <summary>
        /// method de rotation d'une image
        /// </summary>
        /// <param name="sourceBytes"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public byte[]? RotateImage(byte[] sourceBytes, float degrees)
        {
            // 1. Créer le Bitmap à partir du flux de la caméra
            Bitmap? sourceBitmap = BitmapFactory.DecodeByteArray(sourceBytes, 0, sourceBytes.Length);

            // 2. Configurer la matrice de rotation
            Matrix matrix = new();
            matrix.PostRotate(degrees); // Souvent 90 ou 270 degrés sur Android

            if (sourceBitmap is null) return null;
            // 3. Créer le nouveau Bitmap pivoté
            Bitmap rotatedBitmap = Bitmap.CreateBitmap(
            sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height, matrix, true
            );

            // 4. Recycler et convertir en flux pour MAUI
            using var stream = new System.IO.MemoryStream();

            var format = Bitmap.CompressFormat.Png;

            if (format is null) return null;

            rotatedBitmap.Compress(format, 100, stream);
            return stream.ToArray();
            
        }
    }
    // Listener pour ImageReader
    /// <summary>
    /// methode appelée quand une image est disponible dans ImageReader,
    /// elle lit les octets JPEG et les retourne via le TaskCompletionSource
    /// </summary>
    internal class ImageReaderListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        readonly TaskCompletionSource<byte[]> _tcs;
        public ImageReaderListener(TaskCompletionSource<byte[]> tcs) => _tcs = tcs;
        public void OnImageAvailable(ImageReader? reader)
        {
            if (reader is null) return;
            using var image = reader.AcquireLatestImage();

            if (image == null) return;

            var plane = image.GetPlanes()?[0];
            var buffer = plane?.Buffer;
            var bytes = new byte[buffer?.Remaining() ?? 0];
            buffer?.Get(bytes);

            if (!_tcs.Task.IsCompleted) _tcs.SetResult(bytes);

        }
    }

    // Callbacks d'état simplifiés
    internal class CameraStateCallback(Action<CameraDevice> opened, Action<CameraDevice> disconnected, Action<CameraDevice, CameraError> error) : CameraDevice.StateCallback
    {
        readonly Action<CameraDevice> _opened = opened;
        readonly Action<CameraDevice> _disconnected = disconnected;
        readonly Action<CameraDevice, CameraError> _error = error;

        public override void OnOpened(CameraDevice camera) => _opened?.Invoke(camera);
        public override void OnDisconnected(CameraDevice camera) => _disconnected?.Invoke(camera);
        public override void OnError(CameraDevice camera, CameraError error) => _error?.Invoke(camera, error);
    }

    internal class CameraCaptureStateCallback(Action<CameraCaptureSession> configured, Action<CameraCaptureSession> failed) : CameraCaptureSession.StateCallback
    {
        readonly Action<CameraCaptureSession> _configured = configured;
        readonly Action<CameraCaptureSession> _failed = failed;
        public override void OnConfigured(CameraCaptureSession session) => _configured?.Invoke(session);
        public override void OnConfigureFailed(CameraCaptureSession session) => _failed?.Invoke(session);
    }

    internal class CameraCaptureCallback(Action<CaptureResult> onCompleted) : CameraCaptureSession.CaptureCallback
    {
        readonly Action<CaptureResult> _onCompleted = onCompleted;
        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result) => _onCompleted?.Invoke(result);
    }

    public class MyStateCallback : CameraCaptureSession.StateCallback
    {
        public override void OnConfigured(CameraCaptureSession session)
        {
            // session prête
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // échec
        }
    }
}

#endif