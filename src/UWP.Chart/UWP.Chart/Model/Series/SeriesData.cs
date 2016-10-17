using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace UWP.Chart
{
    [ContentProperty(Name = nameof(Children))]
    public class SeriesData : FrameworkElementBase
    {
        #region Fields
        private SeriesCollection _children;
        #endregion

        #region Internal Property
        public override bool CanDraw
        {
            get
            {
                return base.CanDraw && Children.Count > 0;
            }
        }

        internal override Chart Chart
        {
            get
            {
                return base.Chart;
            }

            set
            {
                base.Chart = value;
                foreach (var item in Children)
                {
                    item.Chart = base.Chart;
                }
            }
        }
        #endregion

        public SeriesData()
        {
            Width = new GridLength(1, GridUnitType.Star);
            Height = new GridLength(1, GridUnitType.Star);
        }

        #region Public Properties
        public SeriesCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new SeriesCollection();
                    _children.CollectionChanged += _children_CollectionChanged;
                }
                return _children;
            }
        }

        #endregion

        //internal SeriesData(Chart chart)
        //{
        //    _chart = chart;
        //}

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Series item in e.NewItems)
                    {
                        item.Chart = Chart;
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
    }
}
