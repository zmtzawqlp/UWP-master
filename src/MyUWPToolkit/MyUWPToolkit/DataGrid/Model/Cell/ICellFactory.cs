using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyUWPToolkit.DataGrid.Model.Cell
{
    public interface ICellFactory
    {
        FrameworkElement CreateCell(DataGrid grid, CellType cellType, CellRange rng);
        void DisposeCell(DataGrid grid, CellType cellType, FrameworkElement cell);
    }
}
