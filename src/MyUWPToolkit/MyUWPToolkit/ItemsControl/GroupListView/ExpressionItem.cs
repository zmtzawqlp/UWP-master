using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyUWPToolkit
{
    internal class ExpressionItem
    {
        public ContentControl Element { get; set; }
        public ScrollViewer ScrollViewer { get; private set; }
        public Visual Visual { get; private set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public CompositionPropertySet ScrollViewerManipProps { get; set; }


        public void StartAnimation()
        {

        }

        public void StopAnimation()
        {

        }

        public void SetToTop()
        {

        }

        public bool IsAllInViewPort()
        {
            return false;
        }
    }

}
