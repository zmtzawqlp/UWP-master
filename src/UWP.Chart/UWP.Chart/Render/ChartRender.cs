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

        #region Axis
        public virtual void OnDrawAxis(Chart chart, CanvasDrawingSession cds)
        {
            foreach (var item in chart.Axes.Children)
            {
                cds.FillRectangle(item.CropRect, Colors.Blue);

            }
        }
        #endregion


        #region Legend
        public virtual void OnDrawLegend(Chart chart, CanvasDrawingSession cds)
        {
            cds.FillRectangle(chart.Legend.CropRect, Colors.Yellow);
        }

        #endregion

        #region Marker
        public virtual void OnDrawMarker(Chart chart, CanvasDrawingSession cds)
        {

        }
        #endregion


        #region Series
        public virtual void OnDrawSeries(Chart chart, CanvasDrawingSession cds)
        {
            cds.FillRectangle(chart.Data.CropRect, Colors.Red);

            foreach (var series in chart.Data.Children)
            {
                if (series.CanDraw)
                {

                }
            }
        }
        #endregion


    }
}
