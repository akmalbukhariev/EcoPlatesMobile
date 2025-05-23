namespace EcoPlatesMobile.Views.Company.Pages;

[QueryProperty(nameof(CompanyImage), nameof(CompanyImage))]
[QueryProperty(nameof(CompanyName), nameof(CompanyName))]
[QueryProperty(nameof(CompanyPhone), nameof(CompanyPhone))]
public partial class CompanyProfileInfoPage : BasePage
{
    public string CompanyImage { get; set; }
    public string CompanyName { get; set; }
    public string CompanyPhone { get; set; }

    private Stream? imageStream = null;
    private bool isNewImageSelected = false;

    public CompanyProfileInfoPage()
    {
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        imCompany.Source = CompanyImage;
        fullImage.Source = CompanyImage;
        entryCompanyName.Text = CompanyName;
        lbPhoneNUmber.Text = CompanyPhone;
    }

    private async void BorderImage_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(borderCompanyImage);

        fullImage.TranslationY = -100;
        fullImage.Opacity = 0;
        fullImage.IsVisible = true;
        boxFullImage.IsVisible = true;

        await Task.WhenAll(
            fullImage.TranslateTo(0, 0, 250, Easing.SinIn),
            fullImage.FadeTo(1, 250, Easing.SinIn)
        );
    }

    private async void ChangeImage_Clicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet("Choose an option", "Cancel", null, "Select from Gallery", "Take a Photo");

        FileResult result = null;

        if (action == "Select from Gallery")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.PickPhotoAsync();
            }
        }
        else if (action == "Take a Photo")
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.CapturePhotoAsync();
            }
        }

        if (result != null)
        {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

            using (Stream sourceStream = await result.OpenReadAsync())
            using (FileStream localFileStream = File.OpenWrite(localFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream);
            }

            imCompany.Source = ImageSource.FromFile(localFilePath);
            fullImage.Source = imCompany.Source;
            imageStream = await result.OpenReadAsync();
            isNewImageSelected = true;
        }
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void Done_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", true);
    }

    private async void PhoneNumber_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(grdPhoneNumber);
        await Shell.Current.GoToAsync(nameof(PhoneNumberChangePage));
    }

    private async void OnImage_Swiped(object sender, SwipedEventArgs e)
    {
        await Task.WhenAll(
            fullImage.TranslateTo(0, -100, 250, Easing.SinOut),
            fullImage.FadeTo(0, 250, Easing.SinOut)
        );

        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
        fullImage.Opacity = 1;
        fullImage.TranslationY = 0;
    }

    private void OnImage_Tapped(object sender, TappedEventArgs e)
    {
        boxFullImage.IsVisible = false;
        fullImage.IsVisible = false;
    }
}