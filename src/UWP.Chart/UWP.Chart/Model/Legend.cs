using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.UI.Xaml;

namespace UWP.Chart
{
    /// <summary>
    /// Legend style properties
    /// </summary>
    public class Legend
    {
        public GridLength Width { get; set; }

        public Legend()
        {
            Width = new GridLength(0.2, GridUnitType.Star);
        }
    }
}
