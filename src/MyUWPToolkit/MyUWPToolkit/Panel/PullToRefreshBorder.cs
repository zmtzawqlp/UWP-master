using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    class PullToRefreshBorder : Panel
    {
        public Size myAvailableSize { get; set; }

        public Size myFinalSize { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.myAvailableSize = availableSize;
            // Children[0] is the outer ScrollViewer
            this.Children[0].Measure(availableSize);
            return this.Children[0].DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.myFinalSize = finalSize;
            // Children[0] is the outer ScrollViewer
            this.Children[0].Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }
}
