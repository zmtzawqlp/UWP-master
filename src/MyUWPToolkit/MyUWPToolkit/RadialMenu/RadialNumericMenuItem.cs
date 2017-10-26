using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MyUWPToolkit.RadialMenu
{
    [ContentProperty(Name = "NumericItems")]
    public class RadialNumericMenuItem : RadialMenuItem
    {
        private ObservableCollection<double> _numericItems;
        /// <summary>
        /// notice in debug mode ContentProperty can't override base class
        /// please set it directly
        /// </summary>
        public ObservableCollection<double> NumericItems
        {
            get
            {
                return _numericItems;
            }
        }
        private RadialMenuItemCollection _items;
        public override RadialMenuItemCollection Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new RadialMenuItemCollection();
                    foreach (var item in NumericItems)
                    {
                        var newItem = new RadialNumericMenuChildrenItem() { Content = item, IsSelected = item == this.Value };
                        newItem.SetMenu(Menu);
                        _items.Add(newItem);
                    }
                }
                else
                {
                    if (_items.Count != NumericItems.Count)
                    {
                        _items.Clear();
                        foreach (var item in NumericItems)
                        {
                            var newItem = new RadialNumericMenuChildrenItem() { Content = item, IsSelected = item == this.Value };
                            newItem.SetMenu(Menu);
                            _items.Add(newItem);
                        }
                    }
                    else
                    {
                        foreach (var item in _items)
                        {
                            item.IsSelected = (double)item.Content == this.Value;
                            item.SetMenu(Menu);
                        }
                    }
                   
                }
                return _items;
            }
        }

        //internal IEnumerable<RadialMenuItem> InternalItems { get; set; }
        public override IEnumerable<RadialMenuItem> SelectedItems
        {
            get
            {
                return Items.Where(x => x.IsSelected);
            }
        }

        public override bool HasItems
        {
            get { return _numericItems.Count > 0; }
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
            (d as RadialNumericMenuItem).OnValueChanged(e);
        }
        public event DependencyPropertyChangedEventHandler ValueChanged;
        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Items != null)
            {
                var item = Items.FirstOrDefault(x => (double)x.Content == Value);
                if (item != null && !item.IsSelected)
                {
                    item.UpdateIsSelectedState();
                }
                ValueChanged?.Invoke(this, e);
            }
        }
        public RadialNumericMenuItem()
        {
            _numericItems = new ObservableCollection<double>();
            SelectionMode = RadialMenuSelectionMode.Single;
        }
    }
}
