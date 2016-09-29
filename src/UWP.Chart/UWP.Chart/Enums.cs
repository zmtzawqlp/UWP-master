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

}
