using MyUWPToolkit;
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

            if (photo != null)
            {
                using (var stream = await photo.OpenAsync(FileAccessMode.ReadWrite))
                {
                    //旋转图片
                    await BitmapHelper.RotateCaptureImageByDisplayInformationAutoRotationPreferences(stream, stream);
                }
            }


            return photo;
        }



        private async void CropButton_Click(object sender, RoutedEventArgs e)
        {
            //Popup p = new Popup();
            //Image image = new Image();
            //image.Source = await imageTool.GetCropImageSource();
            //p.Child = image;
            //p.IsOpen = true;
            await imageTool.RotateAsync(RotationAngle.Clockwise90Degrees);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           // imageTool.ReSetSelectionRect();
        }
    }


    
}
