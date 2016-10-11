using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.UI.Xaml;

namespace UWP.Chart
{
    public class Axis : FrameworkElementBase, IFrameworkElement
    {

        #region Internal Property
        #endregion

        #region Public Property

        #endregion

        #region Dependency Property
        public AxisType AxisType
        {
            get { return (AxisType)GetValue(AxisTypeProperty); }
            set { SetValue(AxisTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AxisType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AxisTypeProperty =
            DependencyProperty.Register("AxisType", typeof(AxisType), typeof(Axis), new PropertyMetadata(AxisType.Y));

        public HorizontalAlignment HorizontalTitleAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTitleAlignmentProperty); }
            set { SetValue(HorizontalTitleAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalTitleAlignmentProperty =
            DependencyProperty.Register("HorizontalTitleAlignment", typeof(HorizontalAlignment), typeof(Axis), new PropertyMetadata(HorizontalAlignment.Stretch, OnDependencyPropertyChangedToInvalidate));



        public VerticalAlignment VerticalTitleAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTitleAlignmentProperty); }
            set { SetValue(VerticalTitleAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalTitleAlignmentProperty =
            DependencyProperty.Register("VerticalTitleAlignment", typeof(VerticalAlignment), typeof(Axis), new PropertyMetadata(HorizontalAlignment.Stretch, OnDependencyPropertyChangedToInvalidate));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Axis), new PropertyMetadata(null, OnDependencyPropertyChangedToInvalidate));

        #endregion

    }
}
