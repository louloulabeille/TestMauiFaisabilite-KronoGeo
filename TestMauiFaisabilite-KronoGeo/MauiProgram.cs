using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using TestMauiFaisabilite_KronoGeo.ViewModels;

namespace TestMauiFaisabilite_KronoGeo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseMauiMaps() // pour afficher les cartes de Microsoft.Maui.Controls.Maps
                // Initialize the .NET MAUI Community Toolkit CameraView by adding the below line of code
                .UseMauiCommunityToolkitCamera()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons-Regular");
                });


#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<TestPhotoVideo>();
            builder.Services.AddTransient<TestPhotoVideoViewModel>();


            return builder.Build();
        }
    }
}
