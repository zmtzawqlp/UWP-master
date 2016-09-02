using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.FlexGrid.Model.RowCol
{
    public class BoundRow:Row
    {

        public BoundRow(object dataItem)
        {
            this.DataItem = dataItem;
        }
    }
}
