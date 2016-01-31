using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace MyUWPToolkit
{
    public class CropSelection : INotifyPropertyChanged
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
                    OnPropertyChanged("HorizontalLineCanvasTop");
                    OnPropertyChanged("VerticalLineCanvasLeft");
                    OnPropertyChanged("HorizontalLine1CanvasTop");
                    OnPropertyChanged("VerticalLine1CanvasLeft");
                }
            }
        }


        public double HorizontalLineCanvasTop
        {
            get
            {
                return (SelectedRect.Bottom - SelectedRect.Top) / 3 * 1 + SelectedRect.Top;
            }

        }
        public double HorizontalLine1CanvasTop
        {
            get
            {
                return (SelectedRect.Bottom - SelectedRect.Top) / 3 * 2 + SelectedRect.Top;
            }

        }

        public double VerticalLineCanvasLeft
        {
            get
            {
                return (SelectedRect.Right - SelectedRect.Left) / 3 * 1 + SelectedRect.Left;
            }
        }

        public double VerticalLine1CanvasLeft
        {
            get
            {
                return (SelectedRect.Right - SelectedRect.Left) / 3 * 2 + SelectedRect.Left;
            }
        }


        private Visibility _cropSelectionVisibility=Visibility.Collapsed;

        public Visibility CropSelectionVisibility
        {
            get { return _cropSelectionVisibility; }
            set
            {
                _cropSelectionVisibility = value;
                this.OnPropertyChanged("CropSelectionVisibility");
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
            var rect = new Rect() { X = SelectedRect.X + x, Y = SelectedRect.Y + y, Width = SelectedRect.Width * scale, Height = SelectedRect.Height * scale };
            var leftTop = new Point(rect.Left, rect.Top);
            var leftBottom = new Point(rect.Left, rect.Bottom);
            var rightTop = new Point(rect.Right, rect.Top);
            var rightBottom = new Point(rect.Right, rect.Bottom);

            if (OuterRect.Contains(leftTop)
                && OuterRect.Contains(leftBottom)
                && OuterRect.Contains(rightTop)
                && OuterRect.Contains(rightBottom)
                && rect.Width >= 2 * MinSelectRegionSize
                && rect.Height >= 2 * MinSelectRegionSize)
            {
                SelectedRect = rect;
            }

        }

        internal void UpdateSelectedRect(string ThumbName, double xUpdate, double yUpdate,Rect? outerRect=null)
        {


            var left = SelectedRect.Left;
            var top = SelectedRect.Top;

            var right = SelectedRect.Right;
            var bottom = SelectedRect.Bottom;


            if (ThumbName == "topLeftThumb")
            {
                left += xUpdate;
                top += yUpdate;
            }
            else if (ThumbName == "topRightThumb")
            {
                right += xUpdate;
                top += yUpdate;
            }
            else if (ThumbName == "bottomLeftThumb")
            {
                left += xUpdate;
                bottom += yUpdate;
            }
            else if (ThumbName == "bottomRightThumb")
            {
                right += xUpdate;
                bottom += yUpdate;
            }

            var rect = new Rect(new Point(left, top), new Point(right, bottom));
            var leftTop = new Point(rect.Left, rect.Top);
            var leftBottom = new Point(rect.Left, rect.Bottom);
            var rightTop = new Point(rect.Right, rect.Top);
            var rightBottom = new Point(rect.Right, rect.Bottom);

            var outerRect1 = outerRect!=null ? outerRect.Value: OuterRect;

            if (outerRect1.Contains(leftTop)
                && outerRect1.Contains(leftBottom)
                && outerRect1.Contains(rightTop)
                && outerRect1.Contains(rightBottom)
                && rect.Width >= 2 * MinSelectRegionSize
                && rect.Height >= 2 * MinSelectRegionSize)
            {
                SelectedRect = rect;
            }
        }


        internal void ResizeSelectedRect(double scale)
        {
            var rect = new Rect() { X = SelectedRect.X * scale, Y = SelectedRect.Y * scale, Width = SelectedRect.Width * scale, Height = SelectedRect.Height * scale };
            var leftTop = new Point(rect.Left, rect.Top);
            var leftBottom = new Point(rect.Left, rect.Bottom);
            var rightTop = new Point(rect.Right, rect.Top);
            var rightBottom = new Point(rect.Right, rect.Bottom);

            if (OuterRect.Contains(leftTop)
                && OuterRect.Contains(leftBottom)
                && OuterRect.Contains(rightTop)
                && OuterRect.Contains(rightBottom)
                && rect.Width >= 2 * MinSelectRegionSize
                && rect.Height >= 2 * MinSelectRegionSize)
            {
                SelectedRect = rect;
            }
        }
        #endregion
    }
}
