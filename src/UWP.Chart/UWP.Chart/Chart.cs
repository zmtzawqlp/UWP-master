using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Render;
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
            SizeChanged += Chart_SizeChanged;
        }

        #region Override
        protected override void OnApplyTemplate()
        {
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            base.OnApplyTemplate();
            CreateCanvas();
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

        private void _series_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Series item in e.NewItems)
                    {
                        item.Chart = this;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (Series item in e.OldItems)
                    {
                        item.Chart = null;
                        //todo
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void Chart_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Dispose();
        }

        private void Chart_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void Chart_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {

        }
        #endregion

        #region View
        private void _view_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            using (CanvasCommandList cl = new CanvasCommandList(sender))
            {
                using (CanvasDrawingSession cds = cl.CreateDrawingSession())
                {
                    OnDraw(cds);
                }
                args.DrawingSession.DrawImage(cl);
            }
            if (ForceRedrawn)
            {
                _view.Invalidate();
            }
        }

        private void OnDraw(CanvasDrawingSession cds)
        {
            if (Axis != null && Axis.CanDraw)
            {
                Render.OnDrawAxis(this, cds);
            }

            if (Data != null)
            {
                Render.OnDrawSeries(this, cds);
            }

            if (Marker != null && Marker.CanDraw)
            {
                Render.OnDrawMarker(this, cds);
            }

            if (Legend != null && Legend.CanDraw)
            {
                Render.OnDrawLegend(this, cds);
            }

        }

        private void _view_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CreateResources();
        }


        private void CreateResources()
        {
            if (Data.Children.Count > 0)
            {
                foreach (ISeries item in Data.Children)
                {
                    item.CreateDataResources();
                }
            }
            //else if (ItemsSource != null)
            //{
            //    IEnumerable list = ItemsSource as IEnumerable;

            //    if (list == null)
            //    {
            //        list = DataContext as IEnumerable;
            //    }

            //    if (list != null)
            //    {
            //        DataBindingHelper.AutoCreateSeries(this, list);

            //        List<object> names = null;
            //        BindingBase bndName = Data.ItemNameBinding;

            //        if (bndName == null && Bindings != null)
            //            bndName = Bindings.ItemNameBinding;

            //        if (bndName != null && Data.ItemNames == null)
            //            names = new List<object>();

            //        int nser = Data.Children.Count;
            //        Dictionary<IDataSeriesInfo, BindingBase[]> dict =
            //            new Dictionary<IDataSeriesInfo, BindingBase[]>();
            //        int lcnt = 0;

            //        for (int i = 0; i < nser; i++)
            //        {
            //            IDataSeriesInfo sinfo = Data.Children[i];
            //            BindingBase[] paths = sinfo.MemberPaths;

            //            if (paths != null)
            //            {
            //                if (Data.Children[i].ItemsSource == null)
            //                {
            //                    dict.Add(sinfo, paths);
            //                    lcnt += paths.Length;
            //                }
            //            }
            //        }

            //        IEnumerator en = list.GetEnumerator();

            //        DataUtils.TryReset(en);
            //        List<object>[] als = new List<object>[lcnt];
            //        int l = 0;
            //        for (l = 0; l < als.Length; l++)
            //            als[l] = new List<object>();

            //        DataBindingProxy dbp = new DataBindingProxy();

            //        // todo handle IList's
            //        while (en.MoveNext())
            //        {
            //            object current = en.Current;
            //            l = 0;

            //            dbp.DataContext = current;

            //            // item names
            //            if (names != null)
            //                names.Add(dbp.GetValue(bndName));

            //            foreach (KeyValuePair<IDataSeriesInfo, BindingBase[]> pair in dict)
            //            {
            //                BindingBase[] bnds = pair.Value;

            //                for (int ib = 0; ib < bnds.Length; ib++)
            //                {
            //                    if (bnds[ib] != null)
            //                        als[l++].Add(dbp.GetValue(bnds[ib]));
            //                }
            //            }
            //            // release data context
            //            dbp.DataContext = null;

            //            if (current is INotifyPropertyChanged)
            //                AddINP((INotifyPropertyChanged)current);
            //        }

            //        l = 0;
            //        foreach (KeyValuePair<IDataSeriesInfo, BindingBase[]> pair in dict)
            //        {
            //            BindingBase[] bnds = pair.Value;
            //            for (int ib = 0; ib < bnds.Length; ib++)
            //            {
            //                if (bnds[ib] != null)
            //                    pair.Key.SetResolvedValues(ib, als[l++].ToArray());
            //            }
            //        }

            //        if (names != null && names.Count > 0)
            //            Data.ItemNamesInternal = names.ToArray();
            //    }

            //    Data.Aggregate = Aggregate;
            //    for (int i = 0; i < Data.Children.Count; i++)
            //    {
            //        DataSeries ds = Data.Children[i];
            //        if (ds.ItemsSource != null)
            //            ds.PerformBinding(AddINP);
            //        renderer.AddSeries(ds);
            //    }
            //}

        }
        #endregion

        #region Public Methods
        //Indicates that the contents of the Chart need to be redrawn. 
        public void Invalidate()
        {
            CreateResources();
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
            }
        }
        #endregion

    }
}
