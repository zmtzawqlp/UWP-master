using UWP.FlexGrid.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.FlexGrid
{
    public class Row : RowCol
    {
        static FrameworkElement _tbh = new TextBlock();
        private object _item;

        public object DataItem
        {
            get { return _item; }
            set { _item = value; }
        }

        public override FlexGrid Grid
        {
            get { return Rows != null ? Rows.Grid : null; }
        }

        public override FlexGridPanel GridPanel
        {
            get
            {
                return Rows != null ? Rows.GridPanel : null;
            }
        }

        public int Index
        {
            get
            {
                this.Rows.Update();
                return this.ItemIndex;
            }
        }
        internal Rows Rows
        {
            get { return List as Rows; }
        }


        public double Height
        {
            get { return Size; }
            set { Size = value; }
        }


        void InvalidateCell(Column c)
        {
            if (GridPanel != null)
            {
                // invalidate if the range is in view (big perf impact!)
                var rng = new CellRange(this.Index, c.Index);
                GridPanel.Invalidate(rng);
            }
        }
        protected override void OnPropertyChanged(string name)
        {
            if (Rows != null)
            {
                Rows.OnPropertyChanged(this, name);
            }
        }
    }
}
