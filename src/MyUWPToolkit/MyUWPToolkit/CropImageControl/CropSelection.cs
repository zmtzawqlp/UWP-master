using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MyUWPToolkit
{
    public class CropSelection : INotifyPropertyChanged
    {
        private const string TopLeftThumbCanvasLeftPropertyName = "TopLeftThumbCanvasLeft";
        private const string TopLeftThumbCanvasTopPropertyName = "TopLeftThumbCanvasTop";
        private const string BottomRightThumbCanvasLeftPropertyName = "BottomRightThumbCanvasLeft";
        private const string BottomRightThumbCanvasTopPropertyName = "BottomRightThumbCanvasTop";
        private const string OutterRectPropertyName = "OuterRect";
        private const string SelectedRectPropertyName = "SelectedRect";

        public const string TopLeftThumbName = "topLeftThumb";
        public const string TopRightThumbName = "topRightThumb";
        public const string BottomLeftThumbName = "bottomLeftThumb";
        public const string BottomRightThumbName = "bottomRightThumb";

        #region Property
        /// <summary>
        /// The minimun size of the seleced region
        /// </summary>
        public double MinSelectRegionSize { get; set; }

        public AspectRatio CropAspectRatio { get; set; }

        #region TopLeftThumb

        private double _topLeftThumbCanvasLeft;
        /// <summary>
        /// The Canvas.Left property of the TopLeft Thumb.
        /// </summary>
        public double TopLeftThumbCanvasLeft
        {
            get { return _topLeftThumbCanvasLeft; }
            set
            {

                if (_topLeftThumbCanvasLeft != value)
                {
                    _topLeftThumbCanvasLeft = value;
                    OnPropertyChanged(TopLeftThumbCanvasLeftPropertyName);
                }
            }
        }

        private double _topLeftThumbCanvasTop;
        /// <summary>
        /// The Canvas.Top property of the TopLeft Thumb.
        /// </summary>
        public double TopLeftThumbCanvasTop
        {
            get { return _topLeftThumbCanvasTop; }
            set
            {

                if (_topLeftThumbCanvasTop != value)
                {
                    _topLeftThumbCanvasTop = value;
                    OnPropertyChanged(TopLeftThumbCanvasTopPropertyName);
                }
            }
        }
        #endregion

        #region BottomRightThumb

        private double _bottomRightThumbCanvasTop;

        /// <summary>
        /// The Canvas.Top property of the BottomRight Thumb.
        /// </summary>
        public double BottomRightThumbCanvasTop
        {
            get { return _bottomRightThumbCanvasTop; }
            protected set
            {
                if (_bottomRightThumbCanvasTop != value)
                {
                    _bottomRightThumbCanvasTop = value;
                    this.OnPropertyChanged(BottomRightThumbCanvasTopPropertyName);
                }
            }
        }

        private double _bottomRightThumbCanvasLeft;

        /// <summary>
        /// The Canvas.Left property of the BottomRight Thumb.
        /// </summary>
        public double BottomRightThumbCanvasLeft
        {
            get { return _bottomRightThumbCanvasLeft; }
            set
            {
                if (_bottomRightThumbCanvasLeft != value)
                {
                    _bottomRightThumbCanvasLeft = value;

                    this.OnPropertyChanged(BottomRightThumbCanvasLeftPropertyName);
                }
            }
        }


        #endregion


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

                    this.OnPropertyChanged(OutterRectPropertyName);
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
            protected set
            {
                if (selectedRect != value)
                {
                    selectedRect = value;

                    this.OnPropertyChanged(SelectedRectPropertyName);
                }
            }
        }


        #endregion

        #region Method

        public void ResetThumb(double topLeftThumbCanvasLeft, double topLeftThumbCanvasTop,
            double bottomRightThumbCanvasLeft, double bottomRightThumbCanvasTop)
        {
            this.BottomRightThumbCanvasLeft = bottomRightThumbCanvasLeft;
            this.BottomRightThumbCanvasTop = bottomRightThumbCanvasTop;
            this.TopLeftThumbCanvasLeft = topLeftThumbCanvasLeft;
            this.TopLeftThumbCanvasTop = topLeftThumbCanvasTop;
        }

        /// <summary>
        /// Update the Canvas.Top and Canvas.Left of the Thumb.
        /// </summary>
        public void UpdateThumb(string ThumbName, double leftUpdate, double topUpdate)
        {
            if (leftUpdate==0 && topUpdate==0)
            {
                return;
            }
            UpdateThumb(ThumbName, leftUpdate, topUpdate,
                this.MinSelectRegionSize, this.MinSelectRegionSize);
        }

        /// <summary>
        /// Update the Canvas.Top and Canvas.Left of the Thumb.
        /// </summary>
        public void UpdateThumb(string ThumbName, double leftUpdate, double topUpdate,
            double minWidthSize, double minHeightSize)
        {

         
            var left = 0.0;
            var top = 0.0;
            switch (ThumbName)
            {
                case CropSelection.TopLeftThumbName:


                    left = ValidateValue(_topLeftThumbCanvasLeft + leftUpdate,
                       0, _bottomRightThumbCanvasLeft - minWidthSize);
                    top = ValidateValue(_topLeftThumbCanvasTop + topUpdate,
                        0, _bottomRightThumbCanvasTop - minHeightSize);

                    if (CropAspectRatio == AspectRatio.Square && ((left==0 && TopLeftThumbCanvasLeft == 0) ||(top==0 && TopLeftThumbCanvasTop == 0)))
                    {
                        return;
                    }
                    TopLeftThumbCanvasLeft = left;
                    TopLeftThumbCanvasTop = top;
                    break;
                case CropSelection.TopRightThumbName:
                    left = ValidateValue(_bottomRightThumbCanvasLeft + leftUpdate,
                        _topLeftThumbCanvasLeft + minWidthSize, outerRect.Width);
                    top = ValidateValue(_topLeftThumbCanvasTop + topUpdate,
                        0, _bottomRightThumbCanvasTop - minHeightSize);
                    Debug.WriteLine(left + "*******************" + top);
                    if (CropAspectRatio == AspectRatio.Square && ((left == outerRect.Width && BottomRightThumbCanvasLeft == outerRect.Width) || (top == 0 && TopLeftThumbCanvasTop == 0)))
                    {
                        return;
                    }
                    BottomRightThumbCanvasLeft = left;
                    TopLeftThumbCanvasTop = top;

                    break;
                case CropSelection.BottomLeftThumbName:

                    left = ValidateValue(_topLeftThumbCanvasLeft + leftUpdate,
                        0, _bottomRightThumbCanvasLeft - minWidthSize);
                    top = ValidateValue(_bottomRightThumbCanvasTop + topUpdate,
                        _topLeftThumbCanvasTop + minHeightSize, outerRect.Height);
            
                    if (CropAspectRatio == AspectRatio.Square && ((left ==0 && TopLeftThumbCanvasLeft == 0) || (top== outerRect.Height && BottomRightThumbCanvasTop == outerRect.Height)))
                    {
                        return;
                    }
                    TopLeftThumbCanvasLeft = left;
                    BottomRightThumbCanvasTop = top;
                    break;
                case CropSelection.BottomRightThumbName:
                    left = ValidateValue(_bottomRightThumbCanvasLeft + leftUpdate,
                        _topLeftThumbCanvasLeft + minWidthSize, outerRect.Width);
                    top = ValidateValue(_bottomRightThumbCanvasTop + topUpdate,
                        _topLeftThumbCanvasTop + minHeightSize, outerRect.Height);
                    if (CropAspectRatio == AspectRatio.Square && ((left == outerRect.Width && BottomRightThumbCanvasLeft == outerRect.Width) ||(top == outerRect.Height && BottomRightThumbCanvasTop == outerRect.Height)))
                    {
                        return;
                    }
                    BottomRightThumbCanvasLeft = left;
                    BottomRightThumbCanvasTop = top;
                    break;
                default:
                    throw new ArgumentException("ThumbName: " + ThumbName + "  is not recognized.");
            }

            var width = BottomRightThumbCanvasLeft - TopLeftThumbCanvasLeft;
            var height = BottomRightThumbCanvasTop - TopLeftThumbCanvasTop;
            if (CropAspectRatio == AspectRatio.Square && width != height)
            {

            }
            //OnPropertyChanged(TopLeftThumbCanvasLeftPropertyName);
            //OnPropertyChanged(TopLeftThumbCanvasTopPropertyName);
            //OnPropertyChanged(BottomRightThumbCanvasLeftPropertyName);
            //OnPropertyChanged(BottomRightThumbCanvasTopPropertyName);


        }

        private double ValidateValue(double tempValue, double from, double to)
        {
            if (tempValue < from)
            {
                tempValue = from;
            }

            if (tempValue > to)
            {
                tempValue = to;
            }

            return tempValue;
        }

        /// <summary>
        /// Update the SelectedRect when it is moved or scaled.
        /// </summary>
        public void UpdateSelectedRect(double scale, double leftUpdate, double topUpdate)
        {
            double width = _bottomRightThumbCanvasLeft - _topLeftThumbCanvasLeft;
            double height = _bottomRightThumbCanvasTop - _topLeftThumbCanvasTop;

            double scaledLeftUpdate = (_bottomRightThumbCanvasLeft - _topLeftThumbCanvasLeft) * (scale - 1) / 2;
            double scaledTopUpdate = (_bottomRightThumbCanvasTop - _topLeftThumbCanvasTop) * (scale - 1) / 2;

            double minWidth = Math.Max(this.MinSelectRegionSize, width * scale);
            double minHeight = Math.Max(this.MinSelectRegionSize, height * scale);



            if (scale != 1)
            {
                this.UpdateThumb(CropSelection.TopLeftThumbName, -scaledLeftUpdate, -scaledTopUpdate);
                this.UpdateThumb(CropSelection.BottomRightThumbName, scaledLeftUpdate, scaledTopUpdate);
            }

            // Move towards BottomRight: Move BottomRightThumb first, and then move TopLeftThumb.
            if (leftUpdate >= 0 && topUpdate >= 0)
            {
                this.UpdateThumb(CropSelection.BottomRightThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                this.UpdateThumb(CropSelection.TopLeftThumbName, leftUpdate, topUpdate,
                                        minWidth, minHeight);
            }

            // Move towards TopRight: Move TopRightThumb first, and then move BottomLeftThumb.
            else if (leftUpdate >= 0 && topUpdate < 0)
            {
                this.UpdateThumb(CropSelection.TopRightThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                this.UpdateThumb(CropSelection.BottomLeftThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }

            // Move towards BottomLeft: Move BottomLeftThumb first, and then move TopRightThumb.
            else if (leftUpdate < 0 && topUpdate >= 0)
            {
                this.UpdateThumb(CropSelection.BottomLeftThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                this.UpdateThumb(CropSelection.TopRightThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }

            // Move towards TopLeft: Move TopLeftThumb first, and then move BottomRightThumb.
            else if (leftUpdate < 0 && topUpdate < 0)
            {
                this.UpdateThumb(CropSelection.TopLeftThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
                this.UpdateThumb(CropSelection.BottomRightThumbName, leftUpdate, topUpdate,
                    minWidth, minHeight);
            }
        }

        /// <summary>
        /// Resize the SelectedRect
        /// </summary>
        /// <param name="scale"></param>
        public void ResizeSelectedRect(double scale)
        {
            if (scale > 1)
            {
                this.BottomRightThumbCanvasLeft = this.BottomRightThumbCanvasLeft * scale;
                this.BottomRightThumbCanvasTop = this.BottomRightThumbCanvasTop * scale;
                this.TopLeftThumbCanvasLeft = this.TopLeftThumbCanvasLeft * scale;
                this.TopLeftThumbCanvasTop = this.TopLeftThumbCanvasTop * scale;
            }
            else
            {
                this.TopLeftThumbCanvasLeft = this.TopLeftThumbCanvasLeft * scale;
                this.TopLeftThumbCanvasTop = this.TopLeftThumbCanvasTop * scale;
                this.BottomRightThumbCanvasLeft = this.BottomRightThumbCanvasLeft * scale;
                this.BottomRightThumbCanvasTop = this.BottomRightThumbCanvasTop * scale;
            }

        }

        #endregion



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            // When the Thumb is moved, update the SelectedRect.
            if (propertyName == TopLeftThumbCanvasLeftPropertyName ||
                propertyName == TopLeftThumbCanvasTopPropertyName ||
                propertyName == BottomRightThumbCanvasLeftPropertyName ||
                propertyName == BottomRightThumbCanvasTopPropertyName)
            {
                var width = BottomRightThumbCanvasLeft - TopLeftThumbCanvasLeft;
                var height = BottomRightThumbCanvasTop - TopLeftThumbCanvasTop;
               
                SelectedRect = new Rect(
                    TopLeftThumbCanvasLeft,
                    TopLeftThumbCanvasTop,
                    width,
                    height);
               
                Debug.WriteLine(SelectedRect.Width + "," + SelectedRect.Height);
            }
        }
        #endregion
    }
}
