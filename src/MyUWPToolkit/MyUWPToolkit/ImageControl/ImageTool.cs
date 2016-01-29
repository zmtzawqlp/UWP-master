using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit
{
    [TemplatePart(Name = ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = ImageGrid, Type = typeof(Grid))]
    [TemplatePart(Name = ImageCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = SourceImage, Type = typeof(Image))]
    [TemplatePart(Name = EditImage, Type = typeof(Image))]
    [TemplatePart(Name = TopLeftThumb, Type = typeof(Ellipse))]
    [TemplatePart(Name = TopRightThumb, Type = typeof(Ellipse))]
    [TemplatePart(Name = BottomLeftThumb, Type = typeof(Ellipse))]
    [TemplatePart(Name = BottomRightThumb, Type = typeof(Ellipse))]
    public class ImageTool : Control
    {
        #region Fields
        private const string ScrollViewer = "scrollViewer";
        private const string ImageGrid = "imageGrid";
        private const string ImageCanvas = "imageCanvas";
        private const string SourceImage = "sourceImage";
        private const string EditImage = "editImage";
        private const string TopLeftThumb = "topLeftThumb";
        private const string TopRightThumb = "topRightThumb";
        private const string BottomLeftThumb = "bottomLeftThumb";
        private const string BottomRightThumb = "bottomRightThumb";

        private Grid imageGrid;
        private ScrollViewer scrollViewer;
        private Canvas imageCanvas;
        private Image sourceImage;
        private Image editImage;
        private Ellipse topLeftThumb;
        private Ellipse topRightThumb;
        private Ellipse bottomLeftThumb;
        private Ellipse bottomRightThumb;

        private bool _isTemplateLoaded = false;

        private uint sourceImagePixelHeight;
        private uint sourceImagePixelWidth;
        /// <summary>
        /// The previous points of all the pointers.
        /// </summary>
        Dictionary<uint, Point?> pointerPositionHistory = new Dictionary<uint, Point?>();

        #endregion

        #region Property
        private CropSelection _cropSelection;

        public CropSelection CropSelection
        {
            get { return _cropSelection; }
            private set { _cropSelection = value; }
        }

        private AspectRatio _cropAspectRatio;

        public AspectRatio CropAspectRatio
        {
            get { return _cropAspectRatio; }
            set { _cropAspectRatio = value; }
        }

        private CropSelectionSize _defaultCropSelectionSize;

        public CropSelectionSize DefaultCropSelectionSize
        {
            get { return _defaultCropSelectionSize; }
            set { _defaultCropSelectionSize = value; }
        }


        public event CropImageSourceChangedEventHandler CropImageSourceChanged;
        #endregion

        #region DependencyProperty

        public StorageFile SourceImageFile
        {
            get { return (StorageFile)GetValue(SourceImageFileProperty); }
            set { SetValue(SourceImageFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceImageFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceImageFileProperty =
            DependencyProperty.Register("SourceImageFile", typeof(StorageFile), typeof(ImageTool), new PropertyMetadata(null, new PropertyChangedCallback(OnSourceImageFileChanged)));

        private static void OnSourceImageFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageTool;
            if (control != null)
            {
                control.OnSourceImageFileChanged();
            }
        }

        public StorageFile TempImageFile
        {
            get { return (StorageFile)GetValue(TempImageFileProperty); }
            private set { SetValue(TempImageFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TempImageFile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TempImageFileProperty =
            DependencyProperty.Register("TempImageFile", typeof(StorageFile), typeof(ImageTool), new PropertyMetadata(null));





        public WriteableBitmap EditImageSource
        {
            get { return (WriteableBitmap)GetValue(EditImageSourceProperty); }
            private set { SetValue(EditImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditImageSourceProperty =
            DependencyProperty.Register("EditImageSource", typeof(WriteableBitmap), typeof(ImageTool), new PropertyMetadata(null));




        #endregion
        public ImageTool()
        {
            this.DefaultStyleKey = typeof(ImageTool);
        }

        protected override void OnApplyTemplate()
        {
            scrollViewer = GetTemplateChild(ScrollViewer) as ScrollViewer;
            imageGrid = GetTemplateChild(ImageGrid) as Grid;
            imageCanvas = GetTemplateChild(ImageCanvas) as Canvas;
            sourceImage = GetTemplateChild(SourceImage) as Image;
            editImage = GetTemplateChild(EditImage) as Image;

            topLeftThumb = GetTemplateChild(TopLeftThumb) as Ellipse;
            topRightThumb = GetTemplateChild(TopRightThumb) as Ellipse;
            bottomLeftThumb = GetTemplateChild(BottomLeftThumb) as Ellipse;
            bottomRightThumb = GetTemplateChild(BottomRightThumb) as Ellipse;
            base.OnApplyTemplate();
            _isTemplateLoaded = true;
            Initialize();
            AttachEvents();

        }
        private void Initialize()
        {
            topLeftThumb.ManipulationMode = topRightThumb.ManipulationMode = bottomLeftThumb.ManipulationMode = bottomRightThumb.ManipulationMode =
            ManipulationModes.TranslateX | ManipulationModes.TranslateY;

            //Thumb width and height is 20.
            CropSelection = new CropSelection { MinSelectRegionSize = 2 * 30, CropAspectRatio = CropAspectRatio };
            imageCanvas.DataContext = CropSelection;
            scrollViewer.DataContext = CropSelection;
        }

        private void AttachEvents()
        {
            sourceImage.SizeChanged += SourceImage_SizeChanged;
            editImage.SizeChanged += EditImage_SizeChanged;
            // Handle the pointer events of the corners. 
            AddThumbEvents(topLeftThumb);
            AddThumbEvents(topRightThumb);
            AddThumbEvents(bottomLeftThumb);
            AddThumbEvents(bottomRightThumb);

            // Handle the manipulation events of the selectRegion
            //selectRegion.ManipulationDelta += selectRegion_ManipulationDelta;
            //selectRegion.ManipulationCompleted += selectRegion_ManipulationCompleted;
            scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        private void EditImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
            scrollViewer.ChangeView(e.NewSize.Width, e.NewSize.Height, null, true);
            //});
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Debug.WriteLine("X:" + (sender as ScrollViewer).HorizontalOffset + ", Y:" + (sender as ScrollViewer).VerticalOffset);
        }

        private void AddThumbEvents(Ellipse thumb)
        {
            //thumb.PointerPressed += Thumb_PointerPressed;
            //thumb.PointerMoved += Thumb_PointerMoved;
            //thumb.PointerReleased += Thumb_PointerReleased;
            //thumb.ManipulationDelta += Thumb_ManipulationDelta;  
        }

        private void SourceImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (CropSelection == null || CropSelection.CropSelectionVisibility == Visibility.Collapsed)
            //{
            //    return;
            //}
            if (e.NewSize.IsEmpty || double.IsNaN(e.NewSize.Height) || e.NewSize.Height <= 0)
            {
                //this.imageCanvas.Visibility = Visibility.Collapsed;
                //CropSelection.OuterRect = Rect.Empty;

                //CropSelection.SelectedRect = new Rect(0, 0, 0, 0);
            }
            else
            {
                if (imageGrid != null)
                {
                    imageGrid.Width = this.ActualWidth + e.NewSize.Width * 2;
                    imageGrid.Height = this.ActualHeight + e.NewSize.Height * 2;
                }


                if (scrollViewer != null)
                {
                    scrollViewer.Width = this.ActualWidth;
                    scrollViewer.Height = this.ActualHeight;

                }
                if (editImage != null)
                {
                    editImage.Width = e.NewSize.Width;
                    editImage.Height = e.NewSize.Height;
                    editImage.Source = sourceImage.Source;
                }

                //this.imageCanvas.Visibility = Visibility.Visible;

                //this.imageCanvas.Height = e.NewSize.Height;
                //this.imageCanvas.Width = e.NewSize.Width;
                //CropSelection.OuterRect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

                //if (e.PreviousSize.IsEmpty || double.IsNaN(e.PreviousSize.Height) || e.PreviousSize.Height <= 0)
                //{
                //    var rect = new Rect();
                //    if (CropAspectRatio == AspectRatio.Custom)
                //    {
                //        rect.Width = e.NewSize.Width / (int)DefaultCropSelectionSize;
                //        rect.Height = e.NewSize.Height / (int)DefaultCropSelectionSize;
                //    }
                //    else
                //    {
                //        var min = Math.Min(e.NewSize.Width, e.NewSize.Height);
                //        rect.Width = rect.Height = min / (int)DefaultCropSelectionSize;
                //    }

                //    rect.X = (e.NewSize.Width - rect.Width) / (int)DefaultCropSelectionSize;
                //    rect.Y = (e.NewSize.Height - rect.Height) / (int)DefaultCropSelectionSize;

                //    CropSelection.SelectedRect = rect;
                //}
                //else
                //{
                //    double scale = e.NewSize.Height / e.PreviousSize.Height;
                //    //todo
                //    CropSelection.ResizeSelectedRect(scale);

                //}

            }
        }

        private async void OnSourceImageFileChanged()
        {
            if (_isTemplateLoaded && SourceImageFile != null)
            {

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

                TempImageFile = await storageFolder.CreateFileAsync("Temp_" + SourceImageFile.Name, CreationCollisionOption.ReplaceExisting);

                await BitmapHelper.CloneBitmapAsync(this.SourceImageFile, TempImageFile);

                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await SourceImageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);

                    this.sourceImagePixelHeight = decoder.PixelHeight;
                    this.sourceImagePixelWidth = decoder.PixelWidth;
                }

                if (this.sourceImagePixelHeight < 2 * 30 ||
                    this.sourceImagePixelWidth < 2 * 30)
                {

                }
                else
                {
                    double sourceImageScale = 1;

                    if (this.sourceImagePixelHeight < this.ActualHeight &&
                        this.sourceImagePixelWidth < this.ActualWidth)
                    {
                        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                    }
                    else
                    {
                        sourceImageScale = Math.Min(this.ActualWidth / this.sourceImagePixelWidth,
                        this.ActualHeight / this.sourceImagePixelHeight);
                        this.sourceImage.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                    }
                    this.sourceImage.Source = await BitmapHelper.GetCroppedBitmapAsync(
                        this.SourceImageFile,
                        new Point(0, 0),
                        new Size(this.sourceImagePixelWidth, this.sourceImagePixelHeight),
                        sourceImageScale);
                }
            }
        }

        public void StartEidtCrop()
        {
            if (scrollViewer != null)
            {
                scrollViewer.IsHitTestVisible = true;
            }


        }

        public void FinishEditCrop()
        {
            if (scrollViewer != null)
            {
                scrollViewer.IsHitTestVisible = false;
            }
        }

        public async Task RotateAsync(RotationAngle angle)
        {
            if (SourceImageFile == null)
            {
                return;
            }

            if (EditImageSource!=null)
            {
               await BitmapHelper.RotateAsync(this.SourceImageFile,EditImageSource,angle);
            }
        }

    }
}
