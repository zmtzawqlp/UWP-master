using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;

namespace UWP.Chart.Render
{
    public interface IChartRender
    {
        void OnDrawAxis(Chart chart, CanvasDrawingSession clds);
        void OnDrawSeries(Chart chart, CanvasDrawingSession clds);
        void OnDrawMarker(Chart chart, CanvasDrawingSession clds);
        void OnDrawLegend(Chart chart, CanvasDrawingSession clds);
    }
}
