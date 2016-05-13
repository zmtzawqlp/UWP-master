using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.Common
{
    internal enum ManipulationStatus
    {
        None,
        CrossSlideLeft,
        CrossSlideRight,
        PullToRefresh,
        Scrolling
    }

    public enum CrossSlideMode
    {
        Left,
        Right
    }
}
