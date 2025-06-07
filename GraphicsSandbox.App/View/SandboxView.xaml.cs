using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using GraphicsSandbox.App.Service;
using GraphicsSandbox.App.ViewModel;

namespace GraphicsSandbox.App.View
{
    /// <summary>
    /// Interaction logic for SandboxView.xaml
    /// </summary>
    public partial class SandboxView : Window, ICanvas
    {
        public SandboxView()
        {
            InitializeComponent();

            var dpd = DependencyPropertyDescriptor.FromProperty(TextBox.IsReadOnlyProperty, typeof(TextBox));
            dpd.AddValueChanged(ConsoleTextBox, OnIsReadOnlyChanged);
        }

        private void OnIsReadOnlyChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsReadOnly)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (DataContext is SandboxViewModel vm)
            {
                vm.RunCodeInBackground(vm.Code);
            }

            var graphics = new Graphics2D(this);
            graphics.BeginDraw();
            graphics.DrawTriangle(new Point(0, 0), new Point(100, 0), new Point(0, 100), Colors.LightBlue);
            graphics.EndDraw();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is SandboxViewModel vm)
            {
                vm.Dispose();
            }
        }

        public double CanvasWidth => ActualWidth;

        public double CanvasHeight => ActualHeight;

        public void UpdateImage(Image image)
        {
            GraphicsCanvas.Children.Clear();
            GraphicsCanvas.Children.Add(image);
        }
    }
}
