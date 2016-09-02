using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UWP.FlexGrid.Model.Cell
{
    public interface ICellFactory
    {
        FrameworkElement CreateCell(FlexGrid grid, CellType cellType, CellRange rng);
        void DisposeCell(FlexGrid grid, CellType cellType, FrameworkElement cell);
    }
}
