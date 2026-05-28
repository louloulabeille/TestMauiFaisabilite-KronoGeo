using TestMauiFaisabilite_KronoGeo.Infrastructure.Photo_Video;
using TestMauiFaisabilite_KronoGeo.ViewModels;

namespace TestMauiFaisabilite_KronoGeo;

public partial class TestPhotoVideo : ContentPage
{
	public TestPhotoVideo(TestPhotoVideoViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Photo.PermissionPhoto();
    }

    #region public methods
    public async void HandleCaptureButtonTapped(object? sender, EventArgs e)
    {
        try
        {
            // Use the Camera field defined above in XAML (`<toolkit:CameraView x:Name="Camera" />`)
            var captureImageCTS = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Stream stream = await Camera.CaptureImage(captureImageCTS.Token);
        }
        catch (Exception ex)
        {
            // Handle Exception
            Console.WriteLine(ex.Message);
        }
    }

 
    #endregion

}