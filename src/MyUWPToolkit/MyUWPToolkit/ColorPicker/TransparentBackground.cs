using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit
{
    public class TransparentBackground : Grid
    {

        public double SquareWidth
        {
            get { return (double)GetValue(SquareWidthProperty); }
            set { SetValue(SquareWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SquareWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SquareWidthProperty =
            DependencyProperty.Register("SquareWidth", typeof(double), typeof(TransparentBackground), new PropertyMetadata(4.0, new PropertyChangedCallback(OnUpdateSquares)));

        private static void OnUpdateSquares(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TransparentBackground).UpdateSquares();
        }

        public Brush SquareBrush
        {
            get { return (Brush)GetValue(SquareBrushProperty); }
            set { SetValue(SquareBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SquareBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SquareBrushProperty =
            DependencyProperty.Register("SquareBrush", typeof(Brush), typeof(TransparentBackground), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xd7, 0xd7, 0xd7)), new PropertyChangedCallback(OnUpdateSquares)));




        public Brush AlternatingSquareBrush
        {
            get { return (Brush)GetValue(AlternatingSquareBrushProperty); }
            set { SetValue(AlternatingSquareBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlternatingSquareBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlternatingSquareBrushProperty =
            DependencyProperty.Register("AlternatingSquareBrush", typeof(Brush), typeof(TransparentBackground), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnUpdateSquares)));

        public TransparentBackground()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            //this.SizeChanged += (s, e) =>
            //{

            //    if (e.NewSize != e.PreviousSize)
            //    {
            //        UpdateSquares();
            //    }
            //};
        }

        Size pre = Size.Empty;
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (pre != finalSize)
            {
                UpdateSquares(finalSize);
                pre = finalSize;
            }

            return base.ArrangeOverride(finalSize);
        }

        private void UpdateSquares(Size? finalSize = null)
        {

            Size size = finalSize == null ? new Size(this.ActualWidth, this.ActualHeight) : finalSize.Value;
            //size = new Size(this.ActualWidth, this.ActualHeight);
            this.Children.Clear();
            for (int x = 0; x < size.Width / SquareWidth; x++)
            {
                for (int y = 0; y < size.Height / SquareWidth; y++)
                {
                    var rectangle = new Rectangle();
                    rectangle.Fill = ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1)) ? SquareBrush : AlternatingSquareBrush;
                    rectangle.Width = Math.Max(0, Math.Min(SquareWidth, size.Width - x * SquareWidth));
                    rectangle.Height = Math.Max(0, Math.Min(SquareWidth, size.Height - y * SquareWidth));

                    rectangle.Margin = new Thickness(x * SquareWidth, y * SquareWidth, 0, 0);
                    rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                    rectangle.VerticalAlignment = VerticalAlignment.Top;
                    this.Children.Add(rectangle);
                }
            }

        }
    }
}
