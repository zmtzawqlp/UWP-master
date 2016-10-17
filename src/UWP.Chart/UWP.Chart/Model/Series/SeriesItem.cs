using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Chart.Model.Series
{
    public class SeriesItem
    {
        public Point Position { get; private set; }

        public double Value { get; private set; }

        public object XValue { get; private set; }

        public Rect CropRect { get; private set; }

        public Brush Fill { get; set; }

        public int Index { get; private set; }

    }

    public class ColumnSeriesItem : SeriesItem
    {
        public double Width { get; set; }

        public Thickness Padding { get; set; }
    }
}
