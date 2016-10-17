using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using UWP.Chart.Render;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UWP.Chart
{
    /// <summary>
    /// 
    /// </summary>

    public partial class Chart : Control, IDisposable
    {
        public Chart()
        {
            this.DefaultStyleKey = typeof(Chart);
            InitializeComponent();
            Loaded += Chart_Loaded;
            Unloaded += Chart_Unloaded;
        }

        #region Override
        protected override void OnApplyTemplate()
        {
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            base.OnApplyTemplate();
            CreateCanvas();

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            //ArrangeChilden(finalSize);
            return base.ArrangeOverride(finalSize);
        }


        #endregion

        #region Private Method
        private void InitializeComponent()
        {
            forceReCreateResources = true;
            _defaultRender = new ChartRender();
            Data = new SeriesData();
            //_data.CollectionChanged += _series_CollectionChanged;
            //_axis = new Axis();
            //_legend = new Legend();
            //_marker = new Marker();
        }

        private void Chart_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Dispose();
        }

        private void Chart_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }


        internal void ArrangeChilden()
        {
            if (_view != null)
            {
                ArrangeChilden(_view.Size, true);
            }
        }

        private void ArrangeChilden(Size finalSize, bool forceArrangeChilden = false)
        {
            if (finalSize == preViewSize && !forceArrangeChilden)
            {
                return;
            }

            preViewSize = finalSize;

            double height = finalSize.Height;
            double width = finalSize.Width;

            if (height == 0 || width == 0 || double.IsInfinity(height) || double.IsInfinity(width))
            {
                return;
            }
            FrameworkElementBase defaultData = new FrameworkElementBase();
            defaultData.Width = new GridLength(1, GridUnitType.Star);
            defaultData.Height = new GridLength(1, GridUnitType.Star);
            var data = DataCanDraw ? Data : defaultData;
            ArrangeChildenSize(height, width, data);
            ArrangeChildenCorpRect(data);
        }

        private void ArrangeChildenCorpRect(FrameworkElementBase data)
        {
            if (LegendCanDraw)
            {
                switch (Legend.Position)
                {
                    case LegendPosition.Left:
                        Legend.CropRect = GetCorpRect(0, 0, Legend);
                        if (AxesCanDraw)
                        {
                            if (Axes.AxisY.Count > 0)
                            {
                                var py = Axes.PrimaryAxisY;
                                py.CropRect = GetCorpRect(Legend.ActualWidth, 0, py);

                                data.CropRect = GetCorpRect(py.CropRect.Right, 0, data);

                                var otherY = Axes.AxisY.Except(new List<Axis>() { py });
                                if (otherY != null)
                                {
                                    var offset = data.CropRect.Right;
                                    foreach (var item in otherY)
                                    {
                                        item.CropRect = GetCorpRect(offset, 0, item);
                                        offset = item.CropRect.Right;
                                    }
                                }
                            }
                            else
                            {
                                data.CropRect = GetCorpRect(Legend.CropRect.Right, 0, data);
                            }

                            //
                            if (Axes.AxisX.Count > 0)
                            {
                                var px = Axes.PrimaryAxisX;
                                px.CropRect = GetCorpRect(data.CropRect.Left, data.CropRect.Bottom, px);


                                var otherX = Axes.AxisX.Except(new List<Axis>() { px });
                                if (otherX != null)
                                {
                                    var offset = px.CropRect.Bottom;
                                    foreach (var item in otherX)
                                    {
                                        item.CropRect = GetCorpRect(px.CropRect.Left, offset, item);
                                        offset = item.CropRect.Bottom;
                                    }
                                }
                            }
                        }
                        else
                        {
                            data.CropRect = GetCorpRect(Legend.CropRect.Right, 0, data);
                        }

                        break;
                    case LegendPosition.Right:
                        if (AxesCanDraw)
                        {
                            if (Axes.AxisY.Count > 0)
                            {
                                var py = Axes.PrimaryAxisY;
                                py.CropRect = GetCorpRect(0, 0, py);

                                data.CropRect = GetCorpRect(py.CropRect.Right, 0, data);

                                var otherY = Axes.AxisY.Except(new List<Axis>() { py });
                                var offset = data.CropRect.Right;
                                if (otherY != null)
                                {
                                    foreach (var item in otherY)
                                    {
                                        item.CropRect = GetCorpRect(offset, 0, item);
                                        offset = item.CropRect.Right;
                                    }
                                }
                                Legend.CropRect = GetCorpRect(offset, 0, Legend);
                            }
                            else
                            {
                                data.CropRect = GetCorpRect(0, 0, data);
                                Legend.CropRect = GetCorpRect(data.CropRect.Right, 0, Legend);
                            }

                            //
                            if (Axes.AxisX.Count > 0)
                            {
                                var px = Axes.PrimaryAxisX;
                                px.CropRect = GetCorpRect(data.CropRect.Left, data.CropRect.Bottom, px);

                                var otherX = Axes.AxisX.Except(new List<Axis>() { px });
                                if (otherX != null)
                                {
                                    var offset = px.CropRect.Bottom;
                                    foreach (var item in otherX)
                                    {
                                        item.CropRect = GetCorpRect(px.CropRect.Left, offset, item);
                                        offset = item.CropRect.Bottom;
                                    }
                                }
                            }

                        }
                        else
                        {
                            data.CropRect = GetCorpRect(0, 0, data);
                            Legend.CropRect = GetCorpRect(data.CropRect.Right, 0, Legend);
                        }

                        break;
                    case LegendPosition.Top:
                        if (AxesCanDraw)
                        {
                            if (Axes.AxisY.Count > 0)
                            {
                                var py = Axes.PrimaryAxisY;
                                py.CropRect = GetCorpRect(0, Legend._actualHeight, py);

                                Legend.CropRect = GetCorpRect(py.CropRect.Right, 0, Legend);

                                data.CropRect = GetCorpRect(py.CropRect.Right, Legend.CropRect.Bottom, data);

                                var otherY = Axes.AxisY.Except(new List<Axis>() { py });

                                if (otherY != null)
                                {
                                    var offset = data.CropRect.Right;
                                    foreach (var item in otherY)
                                    {
                                        item.CropRect = GetCorpRect(offset, py.CropRect.Top, item);
                                        offset = item.CropRect.Right;
                                    }
                                }

                            }
                            else
                            {
                                Legend.CropRect = GetCorpRect(0, 0, Legend);
                                data.CropRect = GetCorpRect(0, Legend.CropRect.Bottom, data);
                            }

                            //
                            if (Axes.AxisX.Count > 0)
                            {
                                var px = Axes.PrimaryAxisX;
                                px.CropRect = GetCorpRect(data.CropRect.Left, data.CropRect.Bottom, px);

                                var otherX = Axes.AxisX.Except(new List<Axis>() { px });
                                if (otherX != null)
                                {
                                    var offset = px.CropRect.Bottom;
                                    foreach (var item in otherX)
                                    {
                                        item.CropRect = GetCorpRect(px.CropRect.Left, offset, item);
                                        offset = item.CropRect.Bottom;
                                    }
                                }
                            }

                        }
                        else
                        {
                            Legend.CropRect = GetCorpRect(0, 0, Legend);
                            data.CropRect = GetCorpRect(0, Legend.CropRect.Bottom, data);
                        }
                        break;
                    case LegendPosition.Bottom:
                        if (AxesCanDraw)
                        {
                            if (Axes.AxisY.Count > 0)
                            {
                                var py = Axes.PrimaryAxisY;
                                py.CropRect = GetCorpRect(0, 0, py);

                                data.CropRect = GetCorpRect(py.CropRect.Right, 0, data);

                                var otherY = Axes.AxisY.Except(new List<Axis>() { py });
                                if (otherY != null)
                                {
                                    var offsetY = data.CropRect.Right;
                                    foreach (var item in otherY)
                                    {
                                        item.CropRect = GetCorpRect(offsetY, py.CropRect.Top, item);
                                        offsetY = item.CropRect.Right;
                                    }
                                }

                            }
                            else
                            {
                                data.CropRect = GetCorpRect(0, 0, data);
                            }

                            var offset = data.CropRect.Bottom;
                            //
                            if (Axes.AxisX.Count > 0)
                            {
                                var px = Axes.PrimaryAxisX;
                                px.CropRect = GetCorpRect(data.CropRect.Left, data.CropRect.Bottom, px);
                                offset = px.CropRect.Bottom;
                                var otherX = Axes.AxisX.Except(new List<Axis>() { px });
                                if (otherX != null)
                                {
                                    foreach (var item in otherX)
                                    {
                                        item.CropRect = GetCorpRect(px.CropRect.Left, offset, item);
                                        offset = item.CropRect.Bottom;
                                    }
                                }
                            }

                            Legend.CropRect = GetCorpRect(data.CropRect.Left, offset, Legend);
                        }
                        else
                        {
                            data.CropRect = GetCorpRect(0, 0, data);
                            Legend.CropRect = GetCorpRect(0, data.CropRect.Bottom, Legend);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {

                if (AxesCanDraw)
                {
                    if (Axes.AxisY.Count > 0)
                    {
                        var py = Axes.PrimaryAxisY;
                        py.CropRect = GetCorpRect(0, 0, py);

                        data.CropRect = GetCorpRect(py.CropRect.Right, 0, data);

                        var otherY = Axes.AxisY.Except(new List<Axis>() { py });
                        if (otherY != null)
                        {
                            var offsetY = data.CropRect.Right;
                            foreach (var item in otherY)
                            {
                                item.CropRect = GetCorpRect(offsetY, py.CropRect.Top, item);
                                offsetY = item.CropRect.Right;
                            }
                        }

                    }
                    else
                    {
                        data.CropRect = GetCorpRect(0, 0, data);
                    }


                    //
                    if (Axes.AxisX.Count > 0)
                    {
                        var px = Axes.PrimaryAxisX;
                        px.CropRect = GetCorpRect(data.CropRect.Left, data.CropRect.Bottom, px);
                        var offset = px.CropRect.Bottom;
                        var otherX = Axes.AxisX.Except(new List<Axis>() { px });
                        if (otherX != null)
                        {
                            foreach (var item in otherX)
                            {
                                item.CropRect = GetCorpRect(px.CropRect.Left, offset, item);
                                offset = item.CropRect.Bottom;
                            }
                        }
                    }
                }
            }
        }

        private void ArrangeChildenSize(double height, double width, FrameworkElementBase data)
        {
            List<FrameworkElementBase> listX = new List<FrameworkElementBase>();
            List<FrameworkElementBase> listY = new List<FrameworkElementBase>();

            if (LegendCanDraw)
            {
                switch (Legend.Position)
                {
                    case LegendPosition.Left:
                    case LegendPosition.Right:
                        listX.Add(Legend);
                        break;
                    case LegendPosition.Top:
                    case LegendPosition.Bottom:
                        listY.Add(Legend);
                        break;
                    default:
                        break;
                }
            }

            if (AxesCanDraw)
            {
                if (Axes.AxisX.Count > 0)
                {
                    listY.AddRange(Axes.AxisX);
                }

                if (Axes.AxisY.Count > 0)
                {
                    listX.AddRange(Axes.AxisY);
                }
            }

            //if (DataCanDraw)
            //{
            //    listX.Add(Data);
            //    listY.Add(Data);
            //}
            //else
            //{
            //    listX.Add(defaultData);
            //    listY.Add(defaultData);
            //}
            listX.Add(data);
            listY.Add(data);

            double startCount = 0;

            double requiredSize = 0;

            foreach (var item in listX)
            {
                if (item.Width.IsStar)
                {
                    startCount += item.Width.Value;
                }
                else
                {
                    requiredSize += item.Width.Value;
                    item._actualWidth = item.Width.Value;
                }
            }

            if (startCount > 0)
            {
                var totalSize = width - requiredSize;
                var starValue = totalSize > 0 ? totalSize / startCount : 0;

                foreach (var item in listX)
                {
                    if (item.Width.IsStar)
                    {
                        item._actualWidth = starValue * item.Width.Value;
                    }
                }
            }

            startCount = 0;

            requiredSize = 0;

            foreach (var item in listY)
            {
                if (item.Height.IsStar)
                {
                    startCount += item.Height.Value;
                }
                else
                {
                    requiredSize += item.Height.Value;
                    item._actualHeight = item.Height.Value;
                }
            }

            if (startCount > 0)
            {
                var totalSize = height - requiredSize;
                var starValue = totalSize > 0 ? totalSize / startCount : 0;

                foreach (var item in listY)
                {
                    if (item.Height.IsStar)
                    {
                        item._actualHeight = starValue * item.Height.Value;
                    }
                }
            }


            if (LegendCanDraw)
            {
                switch (Legend.Position)
                {
                    case LegendPosition.Left:
                    case LegendPosition.Right:
                        Legend._actualHeight = data._actualHeight;
                        break;
                    case LegendPosition.Top:
                    case LegendPosition.Bottom:
                        Legend._actualWidth = data._actualWidth;
                        break;
                    default:
                        break;
                }
            }

            if (AxesCanDraw)
            {
                if (Axes.AxisX.Count > 0)
                {
                    foreach (var item in Axes.AxisX)
                    {
                        item._actualWidth = data._actualWidth;
                    }
                }

                if (Axes.AxisY.Count > 0)
                {
                    foreach (var item in Axes.AxisY)
                    {
                        item._actualHeight = data._actualHeight;
                    }
                }
            }
        }

        private Rect GetCorpRect(double x, double y, FrameworkElementBase febase)
        {
            return new Rect(x, y, febase.ActualWidth, febase.ActualHeight);
        }

        #endregion

        #region View


        private void _view_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {

            ArrangeChilden(_view.Size);

            using (CanvasCommandList cl = new CanvasCommandList(sender))
            {
                using (CanvasDrawingSession cds = cl.CreateDrawingSession())
                {
                    OnDraw(cds);
                }
                args.DrawingSession.DrawImage(cl);
            }
        }

        private void OnDraw(CanvasDrawingSession cds)
        {
            if (AxesCanDraw)
            {
                Render.OnDrawAxis(this, cds);
            }

            if (DataCanDraw)
            {
                Render.OnDrawSeries(this, cds);
            }

            if (MarkerCanDraw)
            {
                Render.OnDrawMarker(this, cds);
            }

            if (LegendCanDraw)
            {
                Render.OnDrawLegend(this, cds);
            }
        }

        private void _view_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CreateDataResources();
        }


        /// <summary>
        /// Create data mode for render
        /// </summary>
        private void CreateDataResources()
        {
            if (Data != null)
            {
                foreach (ISeries item in Data.Children)
                {
                    item.CreateDataResources();
                }
            }

        }
        #endregion

        #region Internal Methods


        #endregion

        #region Public Methods
        //Indicates that the contents of the Chart need to be redrawn. 
        public void Invalidate()
        {
            CreateDataResources();
            if (_view != null)
            {
                _view.Invalidate();
            }
        }
        #endregion

        #region Canvas

        public void CreateCanvas()
        {
            if (_view == null)
            {
                var view = View;
            }

            if (_view.Parent == null && _rootGrid != null)
            {
                _rootGrid.Children.Add(_view);
            }
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.Draw -= _view_Draw;
                _view.CreateResources -= _view_CreateResources;
                _view.RemoveFromVisualTree();
                _view = null;
                preViewSize = Size.Empty;
            }
        }
        #endregion

        #region PropertyChanged
        internal static void OnDependencyPropertyChangedToInvalidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (e.Property.ToString() == "VisibilityProperty")
            {

                var chart = d as Chart;
                if (e.Property == DataProperty)
                {
                    chart.Data.Chart = chart;
                }

                chart.Invalidate();
            }
        }

        internal void OnPropertyChangedToInvalidate()
        {
            Invalidate();
        }
        #endregion

    }
}
