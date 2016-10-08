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
    /// default render for chart,you can 
    /// override the template or style of axis/legend/marker/series by override this
    /// </summary>
    public class ChartRender : IChartRender
    {
        public virtual void OnDrawAxis(Chart chart, CanvasDrawingSession cds)
        {

        }

        public virtual void OnDrawLegend(Chart chart, CanvasDrawingSession cds)
        {

        }

        public virtual void OnDrawMarker(Chart chart, CanvasDrawingSession cds)
        {

        }

        public virtual void OnDrawSeries(Chart chart, CanvasDrawingSession cds)
        {
            foreach (var series in chart.Data.Children)
            {
                if (series.CanDraw)
                {

                }
            }
        }
    }
}
