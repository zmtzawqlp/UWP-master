using MyUWPToolkit;
using MyUWPToolkit.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageToolPage : Page
    {
        public ImageToolPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            StorageFile photo = null;
            if (btn.Content.ToString() == "Take a photo")
            {
                photo = await GetPhotoByCameraCapture();
            }
            else
            {
                photo = await GetPhotoByPictureLibrary();
            }

            if (photo != null)
            {
                imageTool.SourceImageFile = photo;
                //imageTool.StartEidtCrop();
            }

        }

        private async Task<StorageFile> GetPhotoByPictureLibrary()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            return await openPicker.PickSingleFileAsync();
        }

        private async Task<StorageFile> GetPhotoByCameraCapture()
        {
            var cameraCaptureUI = new CameraCaptureUI();
            cameraCaptureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            cameraCaptureUI.PhotoSettings.AllowCropping = false;

            var photo = await cameraCaptureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (PlatformIndependent.IsWindowsPhoneDevice)
            {
                if (photo != null)
                {
                    using (var stream = await photo.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        //旋转图片
                        await BitmapHelper.RotateCaptureImageByDisplayInformationAutoRotationPreferences(stream, stream);
                    }
                }
            }


            return photo;
        }



        private void CropButton_Click(object sender, RoutedEventArgs e)
        {

            imageTool.StartEidtCrop();
            CropButton.Visibility = Visibility.Collapsed;
            RotateButton.Visibility = Visibility.Collapsed;
            CancelEditButton.Visibility = Visibility.Collapsed;
            OkButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            imageTool.CancelEditCrop();
            CropButton.Visibility = Visibility.Visible;
            RotateButton.Visibility = Visibility.Visible;
            CancelEditButton.Visibility = Visibility.Visible;
            OkButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await imageTool.FinishEditCrop();
            if (result)
            {
                CropButton.Visibility = Visibility.Visible;
                RotateButton.Visibility = Visibility.Visible;
                CancelEditButton.Visibility = Visibility.Visible;
                OkButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
            }

        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            imageTool.RotateAsync(RotationAngle.Clockwise90Degrees);
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();

            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;


            savePicker.FileTypeChoices["jpg"] = new List<string> { ".jpg" };
            savePicker.FileTypeChoices["jpeg"] = new List<string> { ".jpeg" };
            savePicker.FileTypeChoices["png"] = new List<string> { ".png" };

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                imageTool.SaveBitmap(file);
            }

        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            imageTool.CancelEdit();
        }
    }



}
