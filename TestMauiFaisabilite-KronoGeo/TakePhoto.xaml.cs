using TestMauiFaisabilite_KronoGeo.Infrastructure.Photo_Video;
using TestMauiFaisabilite_KronoGeo.ViewModels;

namespace TestMauiFaisabilite_KronoGeo;

public partial class TakePhoto : ContentPage
{
	public TakePhoto()
	{
		InitializeComponent();
		BindingContext = new TakePhotoViewModel();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Photo.PermissionPhoto();
    }
}