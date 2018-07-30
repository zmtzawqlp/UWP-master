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
    public class FillRowViewPanel : Panel
    {
        public int MinRowItemsCount
        {
            get { return (int)GetValue(MinRowItemsCountProperty); }
            set { SetValue(MinRowItemsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinRowItemsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinRowItemsCountProperty =
            DependencyProperty.Register("MinRowItemsCount", typeof(int), typeof(FillRowViewPanel), new PropertyMetadata(0));



        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double childrenWidth = 0;
            //double maxheight = double.MinValue;
            foreach (var item in Children)
            {
                if (item is ContentControl cc && cc.Content is IResizable iResizable)
                {
                    //item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var elementSize = iResizable;
                    var width = elementSize.Width * finalSize.Height / elementSize.Height;
                    //maxheight = Math.Max(elementSize.Height, maxheight);
                    childrenWidth += width;
                }
            }

            double ratio = childrenWidth / finalSize.Width;
            double x = 0;
            var count = Children.Count;
            foreach (var item in Children)
            {
                if (item is ContentControl cc && cc.Content is IResizable iResizable)
                {
                    var elementSize = iResizable;
                    var width = elementSize.Width * finalSize.Height / elementSize.Height;
                    //if children count is less than MinRowItemsCount and chidren total width less than finalwidth
                    //it don't need to stretch children
                    if (count < MinRowItemsCount && ratio < 1)
                    {
                        //to nothing
                    }
                    else
                    {
                        width /= ratio;
                    }

                    var rect = new Rect(x, 0, width, finalSize.Height);
                    item.Measure(new Size(rect.Width, finalSize.Height));
                    item.Arrange(rect);
                    x += width;
                }

            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
