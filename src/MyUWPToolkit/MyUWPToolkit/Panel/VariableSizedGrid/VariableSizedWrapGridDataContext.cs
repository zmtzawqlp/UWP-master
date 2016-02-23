using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyUWPToolkit
{
    public class VariableSizedWrapGridDataContext : INotifyPropertyChanged
    {
        private double _itemHeight;

        public double ItemHeight
        {
            get { return _itemHeight; }
            set
            {
                _itemHeight = value;
                OnPropertyChanged("ItemHeight");
            }
        }

        private double _itemWidth;

        public double ItemWidth
        {
            get { return _itemWidth; }
            set
            {
                _itemWidth = value;
                OnPropertyChanged("ItemWidth");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion

    }

    public class VariableSizedItemDataContext : DependencyObject
    {
        public DataTemplate VariableSizedItemTemplate
        {
            get { return (DataTemplate)GetValue(VariableSizedItemTemplateProperty); }
            set { SetValue(VariableSizedItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VariableSizedItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VariableSizedItemTemplateProperty =
            DependencyProperty.Register("VariableSizedItemTemplate", typeof(DataTemplate), typeof(VariableSizedItemDataContext), new PropertyMetadata(null));

        public ObservableCollection<Object> Items
        {
            get { return (ObservableCollection<Object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Object>), typeof(VariableSizedItemDataContext), new PropertyMetadata(null));
    }
}
