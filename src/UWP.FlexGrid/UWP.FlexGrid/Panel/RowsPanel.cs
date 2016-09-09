using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.FlexGrid
{
    public class CellPanel : FlexGridPanel
    {

        #region fields
        RowsDictionary _rows;
        #endregion

        public CellPanel(FlexGrid grid, CellType cellType, int rowHeight, int colWidth) : base(grid, cellType, rowHeight, colWidth)
        {
            _rows = new RowsDictionary();
        }
    }
}
