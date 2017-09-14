using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

namespace MyUWPToolkit.RadialMenu
{
    [ContentProperty(Name = "Items")]
    [Bindable]
    public partial class RadialMenu : Control, IRadialMenuItemsControl
    {
        public RadialMenu()
        {
            this.DefaultStyleKey = typeof(RadialMenu);
            _items = new ObservableCollection<RadialMenuItem>();
        }
        #region override
        protected override void OnApplyTemplate()
        {
            _currentItemPresenter = GetTemplateChild("CurrentItemPresenter") as RadialMenuItemsPresenter;
            _currentItemPresenter.Menu = this;
            _navigationButton = GetTemplateChild("NavigationButton") as Button;
            CurrentItem = this;
            base.OnApplyTemplate();
        }
        #endregion


    }
}
