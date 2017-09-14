using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MyUWPToolkit.RadialMenu
{
    public class ArcSegments : INotifyPropertyChanged
    {

        private ArcSegmentItem checkElement;

        public ArcSegmentItem CheckElement
        {
            get { return checkElement; }
            set
            {
                checkElement = value;
                OnPropertyChanged("CheckElement");
            }
        }

        private ArcSegmentItem _pointerOverElement;
        public ArcSegmentItem PointerOverElement
        {
            get { return _pointerOverElement; }
            set
            {
                _pointerOverElement = value;
                OnPropertyChanged("PointerOverElement");
            }
        }

        private ArcSegmentItem expandArea;

        public ArcSegmentItem ExpandArea
        {
            get { return expandArea; }
            set
            {
                expandArea = value;
                OnPropertyChanged("ExpandArea");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }

    public class ArcSegmentItem : INotifyPropertyChanged
    {

        private Point _startPoint;

        public Point StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                OnPropertyChanged("StartPoint");
            }
        }

        private Point _endPoint;

        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                OnPropertyChanged("EndPoint");
            }
        }

        private Size _size;

        public Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        private double _expandIconY;

        public double ExpandIconY
        {
            get { return _expandIconY; }
            set
            {
                _expandIconY = value;
                OnPropertyChanged("ExpandIconY");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
