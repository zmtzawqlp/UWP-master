using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace MyUWPToolkit.RadialMenu
{
    public class RadialMenuPanel : Panel
    {
        private RadialMenuItemsPresenter _radialMenuItemsPresenter = null;
        private RadialMenuItemsPresenter RadialMenuItemsPresenter
        {
            get
            {
                if (_radialMenuItemsPresenter == null)
                {
                    _radialMenuItemsPresenter = ItemsControl.GetItemsOwner(this) as RadialMenuItemsPresenter;
                }
                return _radialMenuItemsPresenter;
            }
        }

        private RadialMenu Menu
        {
            get
            {
                return RadialMenuItemsPresenter?.Menu;
            }
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var item in this.Children)
            {
                item.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeChildren(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        private void ArrangeChildren(Size finalSize)
        {
            double radius = Math.Min(finalSize.Width, finalSize.Height) * 0.5;
            if (radius <= 0)
            {
                return;
            }

            int count = Children.Count;
            int sectorCount = -1;
            double startAngle = 0.0;

            RadialNumericMenuItem radialNumericMenuItem = Menu.CurrentItem as RadialNumericMenuItem;

            if (radialNumericMenuItem != null && radialNumericMenuItem.Items.Count > 0 && count == 0)
            {
                List<RadialMenuItem> internalItems = new List<RadialMenuItem>();
                foreach (var item in radialNumericMenuItem.Items)
                {
                    var newItem = new RadialNumericMenuChildrenItem() { Content = item, IsSelected = item == radialNumericMenuItem.Value };
                    newItem.SetMenu(Menu);
                    internalItems.Add(newItem);
                    this.Children.Add(newItem);
                }
                radialNumericMenuItem.InternalItems = internalItems;
                return;
            }

            if (radialNumericMenuItem == null)
            {
                if (Menu.CurrentItem != null && Menu.CurrentItem is RadialMenuItem raitem && raitem.SectorCount > 0)
                {
                    sectorCount = raitem.SectorCount;
                }
                else if (Menu.SectorCount > 0)
                {
                    sectorCount = Menu.SectorCount;
                }
            }

            startAngle = Menu.StartAngle;

            List<RadialMenuItem> items = new List<RadialMenuItem>();
            foreach (var item in this.Children)
            {
                if (item is RadialMenuItem radialMenuItem && radialMenuItem.Visibility == Visibility.Visible)
                {
                    items.Add(radialMenuItem);
                }
            }

            count = (sectorCount > 0 && sectorCount > items.Count) ? sectorCount : items.Count;

            double childAngle = 360.0 / (Math.Max((double)count, 2));

            double rate = radialNumericMenuItem != null ? 1.0 : 0.99;
            //rate = 0.99;
            //leave some marign form sector to sector
            var sin = Sin((childAngle * rate) / 2.0);
            var cos = Cos((childAngle) / 2.0);

            var sectorRect = new Rect() { Width = 2 * sin * radius, Height = radius };
            sectorRect.X = radius - sectorRect.Width / 2.0;


            var pointerOverElementRadius = radius;

            var expandAreaRadius = radius - Menu.ExpandAreaThickness / 2.0;

            var selectedElementRadius = radius - Menu.ExpandAreaThickness - Menu.SelectedElementThickness / 2.0;

            var navigationButtonSize = Math.Min(Menu._navigationButton.ActualWidth, Menu._navigationButton.ActualHeight);

            var colorElementStrokeThickness = radius - Menu.ExpandAreaThickness - Menu.SelectedElementThickness - navigationButtonSize * 0.5;

            var colorElementRadius = radius - Menu.ExpandAreaThickness - Menu.SelectedElementThickness - colorElementStrokeThickness / 2.0;


            Thickness radialNumericMenuItemPading = new Thickness();
            Line line1 = null;
            Line line2 = null;
            if (radialNumericMenuItem != null)
            {
                colorElementRadius = (radius - Menu.ExpandAreaThickness - Menu.SelectedElementThickness) * 0.7;

                var markTotalLength = colorElementRadius * 1.1;
                line1 = new Line();
                line2 = new Line();
                line1.StartPoint = new Point(sectorRect.Width / 2.0, radius - markTotalLength);
                line1.EndPoint = new Point(sectorRect.Width / 2.0, radius - navigationButtonSize * 0.5);

                line2.StartPoint = new Point(sectorRect.Width / 2.0, radius - colorElementRadius);
                line2.EndPoint = new Point(sectorRect.Width / 2.0, radius - navigationButtonSize * 0.5);

                radialNumericMenuItemPading = new Thickness(0, Menu.ExpandAreaThickness + 2, 0, 0);
            }

            var hitTestElementStrokeThickness = radius - Menu.ExpandAreaThickness - Math.Min(Menu._navigationButton.DesiredSize.Width, Menu._navigationButton.DesiredSize.Height) * 0.5;

            var hitTestElementRadius = radius - Menu.ExpandAreaThickness - hitTestElementStrokeThickness / 2.0;

            var pointerOverElement = new ArcSegmentItem();
            SetArcSegmentItem(pointerOverElement, pointerOverElementRadius, sin, cos, sectorRect);

            var expandArea = new ArcSegmentItem();
            SetArcSegmentItem(expandArea, expandAreaRadius, sin, cos, sectorRect);
            expandArea.ExpandIconY = radius - expandArea.Size.Height;

            var selectedElement = new ArcSegmentItem();
            SetArcSegmentItem(selectedElement, selectedElementRadius, sin, cos, sectorRect);

            var colorElement = new ArcSegmentItem();
            SetArcSegmentItem(colorElement, colorElementRadius, sin, cos, sectorRect);
            colorElement.StrokeThickness = colorElementStrokeThickness;

            var hitTestElement = new ArcSegmentItem();
            SetArcSegmentItem(hitTestElement, hitTestElementRadius, sin, cos, sectorRect);
            hitTestElement.StrokeThickness = hitTestElementStrokeThickness;

            int i = 0;
            bool first = true;
            int j = items.Count;
            if (Menu?.CurrentItem is RadialMenuItem item1 && item1.Items.Count < count)
            {
                startAngle = -(childAngle * item1.Items.Count / 2.0 + item1.ContentAngle) + childAngle / 2.0;
            }

            if (Menu.FillEmptyPlaces)
            {
                if (j < count)
                {
                    while (j < count)
                    {
                        var newItem = new RadialMenuItem();
                        newItem.SetMenu(Menu);
                        //items.Add(newItem);
                        this.Children.Add(newItem);
                        j++;
                    }
                    return;
                }

            }

            for (int k = 0; k < items.Count; k++)
            {
                var radialMenuItem = items[k];

                var angle = startAngle + childAngle * k;

                if (first && radialMenuItem.HasItems)
                {
                    if (radialMenuItem.ExpandIcon.DesiredSize.Height == 0)
                    {
                        radialMenuItem.ExpandIcon.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    expandArea.ExpandIconY -= radialMenuItem.ExpandIcon.DesiredSize.Height / 2.0;
                    first = false;
                }

                radialMenuItem.ArcSegments.ExpandArea = expandArea;
                radialMenuItem.ArcSegments.PointerOverElement = pointerOverElement;
                radialMenuItem.ArcSegments.SelectedElement = selectedElement;
                radialMenuItem.ArcSegments.HitTestElement = hitTestElement;
                if (radialMenuItem is RadialColorMenuItem)
                {
                    radialMenuItem.ArcSegments.ColorElement = colorElement;
                }

                if (radialNumericMenuItem != null)
                {
                    if (radialMenuItem is RadialNumericMenuChildrenItem radialNumericMenuChildrenItem)
                    {
                        radialNumericMenuChildrenItem.ArcSegments.ColorElement = colorElement;
                        if (k == 0 || k == count - 1)
                        {
                            if (k == 0)
                            {
                                radialNumericMenuChildrenItem.ColorElement.Clip = new RectangleGeometry() { Rect = new Rect((colorElement.EndPoint.X - colorElement.StartPoint.X) / 2.0, 0, colorElement.EndPoint.X - colorElement.StartPoint.X, colorElement.Size.Height) };
                            }
                            else
                            {
                                radialNumericMenuChildrenItem.ColorElement.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, (colorElement.EndPoint.X - colorElement.StartPoint.X) / 2.0, colorElement.Size.Height) };
                            }
                            radialNumericMenuChildrenItem.Line2 = line2;
                        }
                        radialMenuItem.Padding = radialNumericMenuItemPading;
                        radialNumericMenuChildrenItem.Line1 = line1;
                    }
                }

                RotateTransform transform = new RotateTransform();
                transform.CenterX = sectorRect.Width / 2.0;
                transform.CenterY = sectorRect.Height;
                transform.Angle = angle;
                radialMenuItem.ContentAngle = -angle;
                radialMenuItem.RenderTransform = transform;
                radialMenuItem.Arrange(sectorRect);
            }
        }

        public void SetArcSegmentItem(ArcSegmentItem item, double radius, double sin, double cos, Rect sectorRect)
        {
            item.Size = new Size(radius, radius);
            item.StartPoint = new Point(sectorRect.Width / 2.0 - sin * radius, sectorRect.Height - cos * radius);
            item.EndPoint = new Point(sectorRect.Width / 2.0 + sin * radius, item.StartPoint.Y);
        }

        private double Sin(double angle)
        {
            return Math.Round(Math.Sin(angle / 180 * Math.PI), 5);
        }

        private double Cos(double angle)
        {
            return Math.Round(Math.Cos(angle / 180 * Math.PI), 5);
        }
    }
}
