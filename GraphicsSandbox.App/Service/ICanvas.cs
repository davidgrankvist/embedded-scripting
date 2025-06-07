using System.Windows.Controls;

namespace GraphicsSandbox.App.Service;
internal interface ICanvas
{
    public double CanvasWidth { get; }

    public double CanvasHeight { get; }

    public void UpdateImage(Image image);
}
