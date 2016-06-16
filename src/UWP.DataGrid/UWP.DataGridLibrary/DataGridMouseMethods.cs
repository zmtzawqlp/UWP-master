using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace UWP.DataGrid
{
    public partial class DataGrid
    {
        private void DataGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _cellPanel.ClearPointerPressedAnimation();
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                if (!e.Pointer.IsInContact)
                {
                    _cellPanel.HandlePointerOver(_cellPanel.currentpointerOverRow);
                }

            }
        }

        private void DataGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var pt = e.GetCurrentPoint(_cellPanel).Position;
                var row = GetRowFromPoint(pt);
                _cellPanel.HandlePointerPressed(row);

            }
        }


        private void DataGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var pt = e.GetCurrentPoint(_cellPanel).Position;
                if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                {
                    pointerOverPoint = pt;
                }
                else
                {
                    pointerOverPoint = null;
                }

                var row = -2;

                if (pt.Y <= _cellPanel.Rows.GetTotalSize())
                {
                    // get row and column at given coordinates
                    row= GetRowFromPoint(pt);
                }
                //-1 means not in cellpanel, -2 means not in rows
                if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                {
                    if (row < 0)
                    {
                        pointerOverPoint = null;
                    }

                    if (row >= 0 || row == -2)
                    {
                        VisualStateManager.GoToState(this, "MouseIndicator", true);
                    }
                }

                if (row != _cellPanel.currentPressedRow)
                {
                    _cellPanel.ClearPointerPressedAnimation();
                    if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                        _cellPanel.HandlePointerOver(row);
                }
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            //pen,mouse
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
            {
                _cellPanel.HandlePointerOver(-1);
                VisualStateManager.GoToState(this, "NoIndicator", true);
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //pen,mouse
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                VisualStateManager.GoToState(this, "MouseIndicator", true);
        }

        private void _contentGrid_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _cellPanel.ClearPointerPressedAnimation();
                PointerPoint mousePosition = e.GetCurrentPoint(sender as Grid);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var verticalOffset = ScrollPosition.Y + delta;
                ScrollPosition = new Point(ScrollPosition.X, verticalOffset);
                VisualStateManager.GoToState(this, "MouseIndicator", true);
            }
        }


        private void _horizontalScrollBar_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as ScrollBar);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var horizontalOffset = ScrollPosition.X + delta;
                ScrollPosition = new Point(horizontalOffset, ScrollPosition.Y);
            }
        }

        private void _verticalScrollBar_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint mousePosition = e.GetCurrentPoint(sender as ScrollBar);
                var delta = mousePosition.Properties.MouseWheelDelta;

                var verticalOffset = ScrollPosition.Y + delta;
                ScrollPosition = new Point(ScrollPosition.X, verticalOffset);
            }
        }
    }
}
