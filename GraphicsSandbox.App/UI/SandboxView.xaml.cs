using System.Windows;

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
