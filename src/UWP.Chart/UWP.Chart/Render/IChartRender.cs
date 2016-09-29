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
        void OnDrawAxis(Chart chart, CanvasDrawingSession cds);
        void OnDrawSeries(Chart chart, CanvasDrawingSession cds);
        void OnDrawMarker(Chart chart, CanvasDrawingSession cds);
        void OnDrawLegend(Chart chart, CanvasDrawingSession cds);
    }
}
