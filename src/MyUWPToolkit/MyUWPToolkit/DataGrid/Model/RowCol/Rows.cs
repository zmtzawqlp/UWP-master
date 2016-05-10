using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUWPToolkit.DataGrid.Model.Cell;

namespace MyUWPToolkit.DataGrid.Model.RowCol
{
    public class Rows : RowCols<Row>
    {

        internal Rows(DataGridPanel panel, int defaultSize) : base(panel, defaultSize)
        {
        }

      
        internal override void Update()
        {
            base.Update();
        }

       
    }
}
