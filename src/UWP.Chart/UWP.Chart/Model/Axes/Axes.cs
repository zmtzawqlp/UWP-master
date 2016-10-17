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
    /// <summary>
    /// Axes style properties
    /// contains Axis x/y etc 
    /// </summary>
    [ContentProperty(Name = nameof(Children))]
    public class Axes : ModelBase
    {
        #region Fields
        private AxisCollection _children;
        #endregion

        #region Internal Property
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

        internal List<Axis> AxisX
        {
            get
            {
                return Children.Where(x => x.AxisType == AxisType.X && x.CanDraw).ToList();
            }
        }

        internal List<Axis> AxisY
        {
            get
            {
                return Children.Where(x => x.AxisType == AxisType.Y && x.CanDraw).ToList();
            }
        }

        internal Axis PrimaryAxisX
        {
            get
            {
                if (AxisX.Count == 0)
                {
                    return null;
                }

                var primary = AxisX.FirstOrDefault(x => x.Primary);
                if (primary == null)
                {
                    primary = AxisX.FirstOrDefault(x => x.CanDraw);
                }
                if (primary != null)
                {
                    primary.Primary = true;
                }
                return primary;
            }
        }

        internal Axis PrimaryAxisY
        {
            get
            {
                if (AxisY.Count == 0)
                {
                    return null;
                }

                var primary = AxisY.FirstOrDefault(x => x.Primary);
                if (primary == null)
                {
                    primary = AxisY.FirstOrDefault(x => x.CanDraw);
                }
                if (primary != null)
                {
                    primary.Primary = true;
                }
                return primary;
            }
        }
        #endregion

        #region Public Property
        public AxisCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new AxisCollection();
                    _children.CollectionChanged -= _children_CollectionChanged;
                    _children.CollectionChanged += _children_CollectionChanged;
                }
                return _children;
            }
        }
        #endregion

        #region Dependency Property

        #endregion

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (Axis item in e.NewItems)
                    {
                        item.Chart = Chart;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (Axis item in e.OldItems)
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
