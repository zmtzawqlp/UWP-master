using System;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit
{


    public delegate void CropImageSourceChangedEventHandler(object sender, CropImageSourceChangedEventArgs e);

    public class CropImageSourceChangedEventArgs:EventArgs
    {
        public ImageSource CropImageSource { get; set; }
    }
}