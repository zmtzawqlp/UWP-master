using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.Chart
{
    public interface ISeries
    {
        /// <summary>
        /// Gets values names.
        /// </summary>
        /// <returns></returns>
        string[] GetItemNames();

        /// <summary>
        /// Gets 2D array containing values.
        /// </summary>
        /// <returns></returns>
        double[,] GetValues();


    }
}
