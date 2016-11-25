using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyUWPToolkit
{
    /// <summary>
    /// to solve issue that can't open two flyouts in 10586.
    /// </summary>
    [ContentProperty(Name = nameof(Content))]
    public class AdvancedFlyout : AdvancedFlyoutBase
    {
        public UIElement Content { get; set; }
        /// <summary>
        /// FlyoutPresenter Style
        /// </summary>
        public Style FlyoutPresenterStyle { get; set; }

        protected override Control CreatePresenter()
        {
            var fp = base.CreatePresenter() as FlyoutPresenter;
            if (FlyoutPresenterStyle != null)
            {
                fp.Style = FlyoutPresenterStyle;
            }
            fp.Content = Content;
            return fp;
        }
    }
}
