using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyUWPToolkit.RadialMenu
{
    public class RadialNumericMenuItem : RadialMenuItem
    {
        private ObservableCollection<double> _items;
        public new ObservableCollection<double> Items
        {
            get
            {
                return _items;
            }
        }

        internal IEnumerable<RadialMenuItem> InternalItems { get; set; }
        public override IEnumerable<RadialMenuItem> SelectedItems
        {
            get
            {
                return InternalItems.Where(x => x.IsSelected);
            }
        }

        public override bool HasItems
        {
            get { return _items.Count > 0; }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(RadialNumericMenuItem), new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadialNumericMenuItem).OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (InternalItems != null)
            {
                var item = InternalItems.FirstOrDefault(x => (double)x.Content == Value);
                if (item != null && !item.IsSelected)
                {
                    item.UpdateIsSelectedState();
                }
            }
        }
        public RadialNumericMenuItem()
        {
            _items = new ObservableCollection<double>();
            SelectionMode = RadialMenuSelectionMode.Single;
        }
    }
}
