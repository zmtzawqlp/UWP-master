using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MyUWPToolkit
{
    public class CropSelection1 : INotifyPropertyChanged
    {

        #region Property

        /// <summary>
        /// The minimun size of the seleced region
        /// </summary>
        public double MinSelectRegionSize { get; set; }

        public AspectRatio CropAspectRatio { get; set; }

        private Rect outerRect;

        /// <summary>
        /// The outer rect. The non-selected region can be represented by the 
        /// OuterRect and the SelectedRect.
        /// </summary>
        public Rect OuterRect
        {
            get { return outerRect; }
            set
            {
                if (outerRect != value)
                {
                    outerRect = value;

                    this.OnPropertyChanged("OuterRect");
                }
            }
        }

        private Rect selectedRect;

        /// <summary>
        /// The selected region, which is represented by the four Thumbs.
        /// </summary>
        public Rect SelectedRect
        {
            get { return selectedRect; }
            set
            {
                if (selectedRect != value)
                {
                    selectedRect = value;

                    this.OnPropertyChanged("SelectedRect");
                }
            }
        }

        #endregion




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

        #region Method
        internal void UpdateSelectedRect(float scale, double x, double y)
        {
            //double width = SelectedRect.Width;
            //double height = SelectedRect.Height;

            //double scaledLeftUpdate = width * (scale - 1) / 2;
            //double scaledTopUpdate = height * (scale - 1) / 2;

            //double minWidth = Math.Max(this.MinSelectRegionSize, width * scale);
            //double minHeight = Math.Max(this.MinSelectRegionSize, height * scale);

            var rect = new Rect() { X = SelectedRect.X + x, Y = SelectedRect.Y + y, Width = SelectedRect.Width, Height = SelectedRect.Height };
            var leftTop = new Point(rect.Left, rect.Top);
            var leftBottom = new Point(rect.Left, rect.Bottom);
            var rightTop = new Point(rect.Right, rect.Top);
            var rightBottom = new Point(rect.Right, rect.Bottom);

            if (OuterRect.Contains(leftTop)&& OuterRect.Contains(leftBottom) && OuterRect.Contains(rightTop) && OuterRect.Contains(rightBottom))
            {
                SelectedRect = rect;
            }
            else
            {

            }

        }

        internal void UpdateThumb(string v, double xUpdate, double yUpdate)
        {
            
        }

        #endregion
    }
}
