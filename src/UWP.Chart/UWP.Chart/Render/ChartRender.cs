using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace UWP.Chart.Render
{
    /// <summary>
    /// default render for chart,you can override this class to 
    /// override the template or style of axis/legend/marker/series
    /// </summary>
    public class ChartRender : IChartRender
    {
        public virtual void OnDrawAxis(Chart chart, CanvasDrawingSession clds)
        {

        }

        public virtual void OnDrawLegend(Chart chart, CanvasDrawingSession clds)
        {

        }

        public virtual void OnDrawMarker(Chart chart, CanvasDrawingSession clds)
        {

        }

        public virtual void OnDrawSeries(Chart chart, CanvasDrawingSession clds)
        {
            foreach (var item in chart.Series)
            {

            }
        }
    }
}
