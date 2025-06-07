using System.Windows;
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

    public void DrawTriangle(Point a, Point b, Point c, Color color)
    {
        PathFigure pathFigure = new PathFigure
        {
            StartPoint = a,
            IsClosed = true
        };

        pathFigure.Segments.Add(new LineSegment(b, true));
        pathFigure.Segments.Add(new LineSegment(c, true));

        PathGeometry pathGeometry = new PathGeometry();
        pathGeometry.Figures.Add(pathFigure);

        var brush = new SolidColorBrush(color);
        context.DrawGeometry(brush, new Pen(brush, 0), pathGeometry);
    }
}
