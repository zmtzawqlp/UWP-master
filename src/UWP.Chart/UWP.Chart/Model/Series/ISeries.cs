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
        /// Gets array containing values.
        /// </summary>
        /// <returns></returns>
        double[,] GetValues();


        /// <summary>
        /// Build data resource for draw
        /// </summary>
        void BuildDataResources(bool clearValues = false);

    }
}
