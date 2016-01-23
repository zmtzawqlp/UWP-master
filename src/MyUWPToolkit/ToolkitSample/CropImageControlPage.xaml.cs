using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class CropImageControlPage : Page
    {
        public CropImageControlPage()
        {
            this.InitializeComponent();
            
        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    FileOpenPicker openPicker = new FileOpenPicker();
        //    openPicker.ViewMode = PickerViewMode.Thumbnail;
        //    openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        //    openPicker.FileTypeFilter.Add(".jpg");
        //    openPicker.FileTypeFilter.Add(".jpeg");
        //    openPicker.FileTypeFilter.Add(".bmp");
        //    openPicker.FileTypeFilter.Add(".png");
        //    StorageFile imgFile = await openPicker.PickSingleFileAsync();
        //    if (imgFile != null)
        //    {
        //        CropImageControl.SourceImageFile = imgFile;
        //    }
        //}

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var cameraCaptureUI = new CameraCaptureUI();
            cameraCaptureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            cameraCaptureUI.PhotoSettings.AllowCropping = true;
            cameraCaptureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
            StorageFile photo = await cameraCaptureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

             if (photo != null)
            {
                CropImageControl.SourceImageFile = photo;
            }

            //BitmapImage bitmapImage = new BitmapImage();
            //using (IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read))
            //{
            //    bitmapImage.SetSource(stream);
            //}

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CropButton_Click(object sender, RoutedEventArgs e)
        {
            Popup p = new Popup();
            Image image = new Image();
            image.Source=await CropImageControl.GetCropImageSource();
            p.Child = image;
            p.IsOpen = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CropImageControl.ReSetSelectionRect();
        }
    }
}
