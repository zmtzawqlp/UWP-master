using UWP.DataGrid.CollectionView;
using UWP.DataGrid.Model.RowCol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWP.DataGrid.Model.Cell
{
    public class CellFactory : ICellFactory
    {
        static Thickness
           _padding = new Thickness(0),   // default padding
           _bdrError = new Thickness(4),           // error tooltip padding
           _bdrFixed = new Thickness(0, 0, 1, 1),  // fixed cells
           _bdrV = new Thickness(0, 0, 1, 0),      // vertical grid lines
           _bdrH = new Thickness(0, 0, 0, 1),      // horizontal grid lines
           _bdrA = new Thickness(0, 0, 1, 1),      // all grid lines
           _bdrN = new Thickness(0);            // no grid lines
        static Brush _brTransparent = new SolidColorBrush(Colors.Transparent);
        static Brush _brOpaque = new SolidColorBrush(Color.FromArgb(0x80, 0, 0, 0));

        public virtual FrameworkElement CreateCell(DataGrid grid, CellType cellType, CellRange rng)
        {
            // cell border
            var bdr = CreateCellBorder(grid, cellType, rng);

            // bottom right cells have no content
            if (!rng.IsValid)
            {
                return bdr;
            }

            // bind/customize cell by type
            switch (cellType)
            {
                case CellType.Cell:
                    CreateCellContent(grid, bdr, rng);
                    break;

                case CellType.ColumnHeader:
                    CreateColumnHeaderContent(grid, bdr, rng);
                    break;
            }

            // done
            return bdr;
        }

        public virtual void DisposeCell(DataGrid grid, CellType cellType, FrameworkElement cell)
        {
            var bdr = cell as Border;
            if (bdr != null)
            {
                bdr.DataContext = null;
                if (bdr.Child is ContentPresenter)
                {
                    (bdr.Child as ContentPresenter).Content = null;
                }
                bdr.Child = null;
            }
        }

        public virtual Border CreateCellBorder(DataGrid grid, CellType cellType, CellRange rng)
        {
            var bdr = new Border();
            switch (cellType)
            {
                // scrollable cells
                case CellType.Cell:
                    var row = grid.Rows[rng.Row];

                    // apply data context to border
                    bdr.DataContext = row.DataItem;

                    // apply cell background
                    var even = grid.Rows[rng.Row].VisibleIndex % 2 == 0;
                    bdr.Background = even || rng.RowSpan > 1 || grid.AlternatingRowBackground == null
                        ? grid.RowBackground
                        : grid.AlternatingRowBackground;


                    // apply borders/padding
                    bdr.BorderThickness = GetBorderThickness(grid, rng);
                    bdr.BorderBrush = grid.GridLinesBrush;
                    bdr.Padding = GetCellPadding(grid, grid.Cells, rng);
                    break;

                case CellType.ColumnHeader:
                    bdr.BorderThickness = GetColumnHeaderBorderThickness(grid);
                    bdr.BorderBrush = grid.HeaderGridLinesBrush;
                    bdr.Padding = GetCellPadding(grid, grid.ColumnHeaders, rng);

                    // apply background
                    bdr.Background = grid.ColumnHeaderBackground;
                    break;
            }

            // don't ever use null brushes for background
            if (bdr.Background == null)
            {
                bdr.Background = _brTransparent;
            }

            bdr.Language = grid.Language;

            // done
            return bdr;
        }

        public virtual Thickness GetColumnHeaderBorderThickness(DataGrid grid)
        {
            if (grid.HasHeader())
            {
                return new Thickness(0, 1, 1, 1);
            }
            return _bdrFixed;
        }

        public virtual Thickness GetBorderThickness(DataGrid grid, CellRange rng)
        {
            switch (grid.GridLinesVisibility)
            {
                case GridLinesVisibility.Horizontal:
                    return _bdrH;
                case GridLinesVisibility.Vertical:
                    return _bdrV;
                case GridLinesVisibility.None:
                    return _bdrN;
            }
            return _bdrA;
        }

        public virtual Thickness GetCellPadding(DataGrid grid, DataGridPanel panel, CellRange rng)
        {
            var p = _padding;

            return p;
        }

        public virtual void CreateCellContent(DataGrid grid, Border bdr, CellRange rng)
        {
            CreateCellContent(grid, grid.Cells, bdr, rng);
        }

        public virtual void CreateColumnHeaderContent(DataGrid grid, Border bdr, CellRange rng)
        {
            var c = grid.Columns[rng.Column];
            if (c.HeaderObject != null)
            {
                var tb = new ContentPresenter();
                tb.VerticalContentAlignment = VerticalAlignment.Center;
                tb.Content = c.HeaderObject;
                bdr.Child = tb;
                // apply global foreground color
                if (grid.ColumnHeaderForeground != null)
                {
                    tb.Foreground = grid.ColumnHeaderForeground;
                }
                if (c.HeaderForeground != null)
                {
                    tb.Foreground = c.HeaderForeground;
                }
                tb.HorizontalAlignment = c.HeaderHorizontalAlignment;
                // show sort direction (after applying styles)
                if (grid.ShowSort && IsSortSymbolRow(grid, rng))
                {
                    // get the grid that owns the row (we could be printing this)
                    var g = grid.Columns.Count > 0 ? grid.Columns[0].Grid : grid;

                    // get sort direction and show it
                    ListSortDirection? sd = g.GetColumnSort(rng.Column);
                    if (sd.HasValue)
                    {
                        var icon = GetGlyphSort(sd.Value, tb.Foreground);
                        SetBorderContent(bdr, bdr.Child, icon, 1);
                    }
                }
            }
            else
            {
                // create TextBlock content
                var tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                bdr.Child = tb;

                // get unbound value
                var value = grid.ColumnHeaders[rng.Row, rng.Column];
                if (value != null)
                {
                    tb.Text = value.ToString();
                }

                // no unbound value? show column caption on first fixed row
                if (string.IsNullOrEmpty(tb.Text) && rng.Row == 0)
                {
                    tb.Text = grid.Columns[rng.Column].GetHeader() ?? "";
                }

                // apply global foreground color
                if (grid.ColumnHeaderForeground != null)
                {
                    tb.Foreground = grid.ColumnHeaderForeground;
                }

                // honor HeaderTemplate property

                if (c.HeaderTemplate != null)
                {
                    bdr.Padding = GetTemplatePadding(bdr.Padding); // Util.ThicknessEmpty;
                    bdr.Child = GetTemplatedCell(grid, c.HeaderTemplate);
                    //bdr.Child = c.HeaderTemplate.LoadContent() as UIElement;
                }

                if (c.HeaderForeground != null)
                {
                    tb.Foreground = c.HeaderForeground;
                }
                tb.HorizontalAlignment = c.HeaderHorizontalAlignment;

                // show sort direction (after applying styles)
                if (grid.ShowSort && IsSortSymbolRow(grid, rng))
                {
                    // get the grid that owns the row (we could be printing this)
                    var g = grid.Columns.Count > 0 ? grid.Columns[0].Grid : grid;

                    // get sort direction and show it
                    ListSortDirection? sd = g.GetColumnSort(rng.Column);
                    if (sd.HasValue)
                    {
                        var icon = GetGlyphSort(sd.Value, tb.Foreground);
                        SetBorderContent(bdr, bdr.Child, icon, 1);
                    }
                }
            }
            
          
        }
        void SetBorderContent(Border bdr, UIElement e1, UIElement e2, int autoCol)
        {
            var g = new Grid();
            g.Background = _brTransparent; // make sure it's transparent
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions[autoCol].Width = new GridLength(1, GridUnitType.Auto);

            bdr.Child = null;
            g.Children.Add(e1);
            g.Children.Add(e2);
            e2.SetValue(Grid.ColumnProperty, 1);
            bdr.Child = g;
        }

        public virtual FrameworkElement GetGlyphSort(ListSortDirection dir, Brush brush)
        {
            return dir == ListSortDirection.Ascending
                ? CreatePolygon(brush, 0, 8, 12, 8, 6, 0)
                : CreatePolygon(brush, 0, 0, 12, 0, 6, 8);
        }

        public virtual bool IsSortSymbolRow(DataGrid grid, CellRange rng)
        {
            return rng.BottomRow == grid.ColumnHeaders.Rows.Count - 1;
        }

        Thickness GetTemplatePadding(Thickness padding)
        {
            padding.Left -= _padding.Left;
            padding.Right -= _padding.Right;
            return padding;
        }

        internal Polygon CreatePolygon(Brush brush, params double[] values)
        {
            var p = new Polygon();
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Margin = new Thickness(4, 0, 4, 0);
            p.Fill = brush != null ? brush : _brOpaque;
            p.Points = new PointCollection();
            for (int i = 0; i < values.Length - 1; i += 2)
            {
                p.Points.Add(new Point(values[i], values[i + 1]));
            }
            return p;
        }
        DataGrid _cacheOwner;
        Dictionary<DataTemplate, List<UIElement>> _dtCache = new Dictionary<DataTemplate, List<UIElement>>();
        Dictionary<UIElement, bool> _dtRawCache = new Dictionary<UIElement, bool>();
        UIElement GetTemplatedCell(DataGrid grid, DataTemplate dt)
        {

            // check that the grid requesting the cell is the cache owner
            if (_cacheOwner == null)
            {
                _cacheOwner = grid;
            }

            // get from cache if possible
            List<UIElement> list = null;


            // not the cache owner? don't use the cache...
            if (grid == _cacheOwner)
            {
                if (_dtCache.TryGetValue(dt, out list) && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (VisualTreeHelper.GetParent(item) == null)
                        {
                            return item;
                        }
                    }
                }
            }
            // create now
            var e = dt.LoadContent() as UIElement;
            // save in recycle list
            if (e != null)
            {
                // not the cache owner? don't use the cache...
                if (grid == _cacheOwner)
                {
                    if (list == null)
                    {
                        list = new List<UIElement>();
                        _dtCache[dt] = list;
                    }
                    list.Add(e);
                    _dtRawCache[e] = true;
                }
            }
            // return
            return e;
        }

        void CreateCellContent(DataGrid grid, DataGridPanel panel, Border bdr, CellRange rng)
        {
            // get row and column
            var r = panel.Rows[rng.Row];
            var c = panel.Columns[rng.Column];
            //var gr = r as GroupRow;

            // honor user templates (if the row has a backing data item)
            if (r.DataItem != null && c.CellTemplate != null && panel.CellType == CellType.Cell)
            {
                bdr.Padding = GetTemplatePadding(bdr.Padding);
                bdr.Child = GetTemplatedCell(grid, c.CellTemplate);
                return;
            }

            // get binding, unbound value for the cell
            var b = r.DataItem != null ? c.Binding : null;
            var ubVal = r.DataItem != null ? null : panel[rng.Row, rng.Column];

            // get foreground brush taking selected state into account
            var fore = GetForegroundBrush(grid, r, rng, grid.Foreground);

            // handle non-text types
            var type = c.DataType;
            TextBlock tb = null;
            CheckBox chk = null;
            if (// not a group, or hierarchical
                panel.CellType == CellType.Cell &&
                (type == typeof(bool) || type == typeof(bool?))) // and bool
            {
                // Boolean cells: use CheckBox
                chk = new CheckBox();
                chk.IsThreeState = type == typeof(bool?);
                chk.HorizontalAlignment = HorizontalAlignment.Center;
                chk.VerticalAlignment = VerticalAlignment.Center;
                chk.MinWidth = 32;

                chk.HorizontalAlignment = HorizontalAlignment.Stretch;
                chk.VerticalAlignment = VerticalAlignment.Stretch;
                chk.Margin = new Thickness(0, -10, 0, -10);

                chk.IsHitTestVisible = false;
                chk.IsTabStop = false;
                if (fore != null)
                {
                    chk.Foreground = fore;
                }

                bdr.Child = chk;
                if (b != null)
                {
                    chk.SetBinding(CheckBox.IsCheckedProperty, b);
                }
                else
                {
                    var value = ubVal as bool?;
                    chk.IsChecked = value;
                }
            }
            else
            {
                // use TextBlock for everything else (even empty cells)
                tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                if (fore != null)
                {
                    tb.Foreground = fore;
                }
                bdr.Child = tb;

                // bound values
                if (b != null)
                {
                    // apply binding
                    tb.SetBinding(TextBlock.TextProperty, b);
                }
                else if (ubVal != null) // unbound values
                {
                    // get column format from column and/or binding
                    tb.Text = r.GetDataFormatted(c);
                }
            }

        }

        Brush GetForegroundBrush(DataGrid grid, Row r, CellRange rng, Brush defBrush)
        {
            var fore = grid.Foreground;
            // done
            return fore != null ? fore : defBrush;
        }
    }
}
