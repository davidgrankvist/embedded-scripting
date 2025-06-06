using GraphicsSandbox.App.UI.Framework;

namespace GraphicsSandbox.App.UI
{
    class EditorViewModel : ViewModelBase
    {
        private string code;

        public EditorViewModel()
        {
            code = string.Empty;
            RunCodeCommand = new DelegateCommand(OnRunCode);
        }

        public string Code
        {
            get => code; set
            {
                code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public DelegateCommand RunCodeCommand { get; }

        private void OnRunCode()
        {
            var vm = new SandboxViewModel();
            vm.Code = code;
            var window = new SandboxView();
            window.DataContext = vm;
            window.Show();
        }
    }
}
