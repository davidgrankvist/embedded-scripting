using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicsSandbox.App.Service;
internal class Graphics2D
{
    private readonly ICanvas canvas;

    private DrawingVisual visual;
    private DrawingContext context;

    public Graphics2D(ICanvas canvas)
    {
        this.canvas = canvas;
    }

    public void BeginDraw()
    {
        visual = new DrawingVisual();
        context = visual.RenderOpen();

        /*
         * This is to make sure the image covers the whole canvas.
         * Otherwise the final image may be smaller than the canvas and
         * become left aligned, which will shift the entire coordinate system.
         */
        FillCanvas(Colors.White);
    }

    public void EndDraw()
    {
        context.Close();
        var drawingImage = new DrawingImage(visual.Drawing);
        var renderedImage = new Image {
            Source = drawingImage,
        };

        canvas.UpdateImage(renderedImage);
    }
    private void DrawPoints(System.Windows.Point[] points, Color color)
    {
        PathFigure pathFigure = new PathFigure
        {
            StartPoint = points[0],
            IsClosed = true
        };

        for (int i = 1; i < points.Length; i++)
        {
            pathFigure.Segments.Add(new LineSegment(points[i], true));
        }

        PathGeometry pathGeometry = new PathGeometry();
        pathGeometry.Figures.Add(pathFigure);

        var brush = new SolidColorBrush(color);
        context.DrawGeometry(brush, new Pen(brush, 0), pathGeometry);
    }

    private void FillCanvas(Color color)
    {
        var a = new System.Windows.Point(0, 0);
        var b = new System.Windows.Point(canvas.CanvasWidth, 0);
        var c = new System.Windows.Point(canvas.CanvasWidth, canvas.CanvasHeight);
        var d = new System.Windows.Point(0, canvas.CanvasHeight);
        DrawPoints([a, b, c, d], color);
    }

    public void DrawTriangle(System.Windows.Point a, System.Windows.Point b, System.Windows.Point c, Color color)
    {
        DrawPoints([a, b, c], color);
    }
}
