using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.FlexGrid.Model.Cell;

namespace UWP.FlexGrid.Model.RowCol
{
    public class Rows : RowCols<Row>
    {

        internal Rows(FlexGridPanel panel, int defaultSize) : base(panel, defaultSize)
        {
        }

      
        internal override void Update()
        {
            base.Update();
        }

       
    }
}
