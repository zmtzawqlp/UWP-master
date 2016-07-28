using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyUWPToolkit.FlexGrid
{
    public class FlexGridItem : ListViewItem
    {
        // Summary:
        //     Occurs after a control changes into a different state.
        public event VisualStateChangedEventHandler CurrentStateChanged;
        //
        // Summary:
        //     Occurs when a control begins changing into a different state.
        public event VisualStateChangedEventHandler CurrentStateChanging;

        public FlexGridItem()
        {
            this.DefaultStyleKey = typeof(FlexGridItem);
            //Loaded += FlexGridItem_Loaded;
            //Unloaded += FlexGridItem_Unloaded;
        }

        //private void FlexGridItem_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    foreach (var item in VisualStateManager.GetVisualStateGroups(this))
        //    {
        //        item.CurrentStateChanging -= Item_CurrentStateChanging;
        //        item.CurrentStateChanged -= Item_CurrentStateChanged;
        //    }
        //}

        //private void FlexGridItem_Loaded(object sender, RoutedEventArgs e)
        //{
        //    foreach (var item in VisualStateManager.GetVisualStateGroups(this))
        //    {
        //        item.CurrentStateChanging += Item_CurrentStateChanging;
        //        item.CurrentStateChanged += Item_CurrentStateChanged;
        //    }
        //}

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            //if (border != null)
            //{
            //    foreach (var item in VisualStateManager.GetVisualStateGroups(border))
            //    {
            //        item.CurrentStateChanging += Item_CurrentStateChanging;
            //        item.CurrentStateChanged += Item_CurrentStateChanged;
            //    }
            //}
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            //if (border != null)
            //{
            //    foreach (var item in VisualStateManager.GetVisualStateGroups(border))
            //    {
            //        item.CurrentStateChanging -= Item_CurrentStateChanging;
            //        item.CurrentStateChanged -= Item_CurrentStateChanged;
            //    }
            //}
            base.OnPointerExited(e);
        }

        Border border;
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            border = GetTemplateChild("OuterContainer") as Border;
            if (border != null)
            {
                foreach (var item in VisualStateManager.GetVisualStateGroups(border))
                {
                    item.CurrentStateChanging += Item_CurrentStateChanging;
                    item.CurrentStateChanged += Item_CurrentStateChanged;
                }
            }
        }

        VisualState currentVisualState;
        private void Item_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            currentVisualState = e.NewState;
            if (CurrentStateChanged != null)
            {
                CurrentStateChanged(sender, e);
            }
        }

        private void Item_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (CurrentStateChanging != null)
            {
                CurrentStateChanging(sender, e);
            }
        }

        public void GoToState(string stateName)
        {
            if (currentVisualState != null && currentVisualState.Name == stateName)
            {
                return;
            }

            VisualStateManager.GoToState(this, stateName, true);

        }
    }
}
