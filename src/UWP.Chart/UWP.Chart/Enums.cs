using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.Chart
{
    public enum ChartType
    {
        Bar,
        Column,
        Pie,
    }

    /// <summary>
    /// Specifies which coordinate corresponds to the data values.
    /// </summary>
    public enum ValueCoordinate
    {
        /// <summary>
        /// None or default.
        /// </summary>
        None,
        /// <summary>
        /// X-coordinate.
        /// </summary>
        X,
        /// <summary>
        /// Y-coordinate.
        /// </summary>
        Y
    }

    /// <summary>
    /// Specifies legend position relative to Series.
    /// </summary>
    public enum LegendPosition
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum AxisType
    {
        /// <summary>
        /// X
        /// </summary>
        X,
        /// <summary>
        /// Y
        /// </summary>
        Y
    }

}
