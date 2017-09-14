using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Media;

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

        private RadialMenu _menu;
        private RadialMenu Menu
        {
            get
            {
                if (_menu == null && RadialMenuItemsPresenter != null)
                {
                    _menu = RadialMenuItemsPresenter.Menu;
                }
                return _menu;
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
            //double arcThickness = ParentItemsControl != null ? GetArcThickness(ParentItemsControl) : 0;
            //double arcRadius = radius * (ParentItemsControl != null ? GetArcRelativeRadius(ParentItemsControl) : 1); // only honor parent setting here

            UIElementCollection children = Children;
            int count = children.Count;
            int sectorCount = -1;
            List<UIElement> elements = new List<UIElement>(count);
            double startAngle = 0.0;

            if (Menu != null)
            {
                if (Menu.CurrentItem != null && Menu.CurrentItem is RadialMenuItem raitem && raitem.SectorCount > 0)
                {
                    sectorCount = raitem.SectorCount;
                }
                else if (Menu.SectorCount > 0)
                {
                    sectorCount = Menu.SectorCount;
                }
                startAngle = Menu.StartAngle;
            }

            count = (sectorCount > 0 && sectorCount > Children.Count) ? sectorCount : Children.Count;

            double childAngle = 360.0 / (Math.Max((double)count, 2));

            var sin = Sin(childAngle / 2.0);
            var cos = Cos(childAngle / 2.0);

            var sectorRect = new Rect() { Width = 2 * sin * radius, Height = radius };
            sectorRect.X = radius - sectorRect.Width / 2.0;


            var pointerOverElementRadius = radius;

            var expandAreaRadius = radius - Menu.ExpandAreaThickness / 2.0;

            var checkElementRadius = radius - Menu.ExpandAreaThickness - Menu.CheckElementThickness / 2.0;


            var pointerOverElement = new ArcSegmentItem();
            SetArcSegmentItem(pointerOverElement, pointerOverElementRadius, sin, cos, sectorRect);
            var expandArea = new ArcSegmentItem();
            SetArcSegmentItem(expandArea, expandAreaRadius, sin, cos, sectorRect);
            expandArea.ExpandIconY = radius - expandArea.Size.Height;
            var checkElement = new ArcSegmentItem();
            SetArcSegmentItem(checkElement, checkElementRadius, sin, cos, sectorRect);


            int i = 0;
            bool first = true;
            foreach (var item in this.Children)
            {
                if (item is RadialMenuItem radialMenuItem && radialMenuItem.Visibility == Visibility.Visible)
                {
                    var angle = startAngle + childAngle * i;
                    i++;
                    if (first)
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
                    radialMenuItem.ArcSegments.CheckElement = checkElement;

                    RotateTransform transform = new RotateTransform();
                    transform.CenterX = sectorRect.Width / 2.0;
                    transform.CenterY = sectorRect.Height;
                    transform.Angle = angle;
                    radialMenuItem.ContentAngle = -angle;
                    radialMenuItem.RenderTransform = transform;
                    radialMenuItem.Arrange(sectorRect);
                }

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
