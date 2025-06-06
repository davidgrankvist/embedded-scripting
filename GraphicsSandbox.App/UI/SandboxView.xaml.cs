using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GraphicsSandbox.App.UI
{
    /// <summary>
    /// Interaction logic for SandboxView.xaml
    /// </summary>
    public partial class SandboxView : Window
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
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is SandboxViewModel vm)
            {
                vm.Dispose();
            }
        }
    }
}
