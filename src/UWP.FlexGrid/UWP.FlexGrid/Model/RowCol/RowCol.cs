using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.FlexGrid
{
    public abstract class RowCol
    {
        double _size = -1;
        double _position = -1;
        int _index = -1;
        int _dataIndex = -1;
        int _visIndex = -1;
        private IList _list;
        private bool _isVisible=true;
        public int DataIndex { get; internal set; }
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public int ItemIndex { get; internal set; }
        public double Position { get; internal set; }
        public int VisibleIndex { get; internal set; }
        internal double Size
        {
            get { return _size; }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    OnPropertyChanged("Size");
                }
            }
        }

        internal void SetSize(double size)
        {
            _size = size;
        }

        public void SetIsVisible(bool isVisible)
        {
            _isVisible = isVisible;
        }

        abstract protected void OnPropertyChanged(string name);

        public abstract FlexGrid Grid { get; }

        public abstract FlexGridPanel GridPanel { get; }

        internal IList List
        {
            get { return _list; }
            set { _list = value; }
        }


    }
}
