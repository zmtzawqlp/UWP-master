using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Util;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Chart
{
    public class SeriesItem
    {
        static object NoValue = double.NaN;

        public Point Position { get; private set; }

        /// <summary>
        /// actual value
        /// </summary>
        public object DataValue { get; private set; }

        /// <summary>
        /// actual  x value
        /// </summary>
        public object XDataValue { get; private set; }

        /// <summary>
        /// calculate value
        /// </summary>
        public double Value
        {
            get
            {
                if (DataValue != null)
                {
                    object o = DataValue;
                    if (o is double)
                        return (double)o;
                    else if (o is DateTime)
                        return ((DateTime)o).ToOADate();
                    else
                        return (double)NoValue;
                }
                else
                    return (double)NoValue;
            }
        }

        public Rect CropRect { get; private set; }

        public Brush Fill { get; set; }

        public int Index { get; private set; }

        public object DataContext { get; private set; }

    }

    public class ColumnSeriesItem : SeriesItem
    {
        public double Width { get; set; }

        public Thickness Padding { get; set; }
    }
}
