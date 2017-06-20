using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    /// <summary>
    /// Children will avg height/width of AvgPanel base on Orientation property
    /// </summary>
    public class AvgPanel : Panel
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(AvgPanel), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AvgPanel).UpdateLayout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var item in this.Children)
            {
                if (Orientation == Orientation.Vertical)
                {
                    item.Measure(new Size(availableSize.Width, availableSize.Height / this.Children.Count));
                }
                else
                {
                    item.Measure(new Size(availableSize.Width / this.Children.Count, availableSize.Height));
                }
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double y = 0;
            double x = 0;
            Rect rect = Rect.Empty;
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (Orientation == Orientation.Vertical)
                {
                    rect = new Rect(x, y, finalSize.Width, finalSize.Height / this.Children.Count);
                    y += rect.Height;
                }
                else
                {
                    rect = new Rect(x, y, finalSize.Width / this.Children.Count, finalSize.Height);
                    x += rect.Width;
                }

                this.Children[i].Arrange(rect);
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
