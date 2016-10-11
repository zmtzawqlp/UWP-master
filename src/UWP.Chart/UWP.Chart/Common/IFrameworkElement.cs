using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.Chart.Common
{
    public interface IFrameworkElement
    {
        Double ActualHeight { get; }
        Double ActualWidth { get; }
    }
}
