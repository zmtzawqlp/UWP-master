using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ToolkitSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WIN2DPage : Page
    {
        public WIN2DPage()
        {
            this.InitializeComponent();
            this.Unloaded += WIN2DPage_Unloaded;
        }

        private void WIN2DPage_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }

        private CanvasBitmap bitmapImg;
        private Rect bitmapRect;
        private const double baseImageSize = 300;
        private double maxImageSize = 300;

        private async void pickImgBtn_Click(object sender, RoutedEventArgs e)
        {
            //select file
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await loadBitmapImage(file);
            }
        }

        private async Task loadBitmapImage(StorageFile sourceFile)
        {
            using (var sourceStream = await sourceFile.OpenAsync(FileAccessMode.Read))
            {
                //resize if needed
                if (useLogicChk.IsChecked.HasValue && !useLogicChk.IsChecked.Value)
                {
                    var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    if (scale > 1)
                    {
                        maxImageSize = baseImageSize / scale;
                        canvas.Height = maxImageSize;
                        canvas.Width = maxImageSize;
                    }
                }

                //load image
                bitmapImg = await CanvasBitmap.LoadAsync(canvas, sourceStream);
                bitmapRect = new Rect();

                //determine width and height
                var width = bitmapImg.Size.Width;
                var height = bitmapImg.Size.Height;
                if (width > height)
                {
                    bitmapRect.Width = Math.Round(width * maxImageSize / height);
                    bitmapRect.Height = maxImageSize;
                    bitmapRect.X = -((bitmapRect.Width - bitmapRect.Height) / 2);
                    bitmapRect.Y = 0;
                }
                else if (height > width)
                {
                    bitmapRect.Width = maxImageSize;
                    bitmapRect.Height = Math.Round(height * maxImageSize / width);
                    bitmapRect.X = 0;
                    bitmapRect.Y = -((bitmapRect.Height - bitmapRect.Width) / 2);
                }
                else
                {
                    bitmapRect.Width = maxImageSize;
                    bitmapRect.Height = maxImageSize;
                    bitmapRect.X = 0;
                    bitmapRect.Y = 0;
                }
                //canvas.Visibility = Visibility.Visible;
                //force canvas to redraw
                canvas.Invalidate();
            }

        }

        private void canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            //var mybitmap = new CanvasRenderTarget(sender, 200, 200);
            //using (var ds=mybitmap.CreateDrawingSession())
            //{
                
            //}

            //only draw if image is loaded
            if (bitmapImg != null)
            {
                bitmapRect = new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight);
                //args.DrawingSession.DrawImage(bitmapImg, bitmapRect,new Rect(0, 0, 100, 100) );
                //sender.Invalidate();
                args.DrawingSession.DrawImage(bitmapImg);
            }
        }

        private async void saveImgBtn_Click(object sender, RoutedEventArgs e)
        {
            // render canvas to bitmap
            var bitmap = new RenderTargetBitmap();
            await bitmap.RenderAsync(canvas);

            // create file 
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.DefaultFileExtension = ".png";
            savePicker.SuggestedFileName = "resizedImage";
            savePicker.FileTypeChoices.Add("PNG", new string[] { ".png" });
            var saveFile = await savePicker.PickSaveFileAsync();

            if (saveFile!=null)
            {
                //save bitmap to file
                using (var stream = await saveFile.OpenStreamForWriteAsync())
                {
                    var pixelBuffer = await bitmap.GetPixelsAsync();
                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    var randomAccessStream = stream.AsRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth,
                        (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixelBuffer.ToArray());
                    await encoder.FlushAsync();
                }
            }
          
        }
    }
}
