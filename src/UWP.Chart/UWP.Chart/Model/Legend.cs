using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.Chart.Common;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace UWP.Chart
{
    /// <summary>
    /// Legend style properties
    /// </summary>
    public class Legend : FrameworkElementBase
    {

        #region Internal Property
        
        #endregion

        #region Public Property

        #endregion

        #region Dependency Property

        public LegendPosition Position
        {
            get { return (LegendPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(LegendPosition), typeof(Legend), new PropertyMetadata(LegendPosition.Right, OnDependencyPropertyChangedToInvalidate));


        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(Legend), new PropertyMetadata(HorizontalAlignment.Stretch, OnDependencyPropertyChangedToInvalidate));



        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalContentAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(Legend), new PropertyMetadata(HorizontalAlignment.Stretch, OnDependencyPropertyChangedToInvalidate));



        #endregion

        public Legend()
        {
            Width = new GridLength(100);
            Height = new GridLength(100);
        }
    }
}
