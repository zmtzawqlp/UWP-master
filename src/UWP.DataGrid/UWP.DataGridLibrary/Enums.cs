﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.DataGrid
{
    [Flags]
    public enum GridLinesVisibility
    {
        /// <summary>
        /// No grid lines are shown.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only horizontal grid lines, which separate rows, are shown.
        /// </summary>
        Horizontal = 1,
        /// <summary>
        /// Only vertical grid lines, which separate columns, are shown.
        /// </summary>
        Vertical = 2,
        /// <summary>
        /// Both horizontal and vertical grid lines are shown.
        /// </summary>
        All = 3,
    }


    public enum SortMode
    {
        //Handle sort by collection view
        Auto,
        //Handle sort by SortingColumn event
        Manual
    }

    public enum ScollingDirectionMode
    {
        //Horizontal and Vertical Scolling will happen at the same time.
        TwoDirection,
        //Only one direction(Horizontal or Vertical) scolling will happen at the same time.
        OneDirection,
    }

    internal enum ScollingDirection
    {
        None,
        Horizontal,
        Vertical
    }

}
