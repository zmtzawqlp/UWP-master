using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public RadialNumericMenuItem()
        {
            this.DefaultStyleKey = typeof(RadialNumericMenuItem);
            _items = new ObservableCollection<double>();
        }
    }
}
