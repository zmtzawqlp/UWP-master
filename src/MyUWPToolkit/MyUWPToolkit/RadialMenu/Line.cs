using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MyUWPToolkit.RadialMenu
{
    public class Line : INotifyPropertyChanged
    {
        private Point startPoint;

        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                startPoint = value;
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        private Point endPoint;

        public Point EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;
                OnPropertyChanged(nameof(EndPoint));
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
