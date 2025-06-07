using System.IO;

using GraphicsSandbox.App.View;
using GraphicsSandbox.App.ViewModel.Framework;

using Microsoft.Win32;

namespace GraphicsSandbox.App.ViewModel
{
    class EditorViewModel : ViewModelBase
    {
        private string code;

        public EditorViewModel()
        {
            code = string.Empty;
            RunCodeCommand = new DelegateCommand(OnRunCode);
            OpenLuaFileCommand = new DelegateCommand(() => OnOpenLuaFile(Environment.CurrentDirectory));
            OpenLuaSampleCommand = new DelegateCommand(() =>
            {
                var exeDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var samplesDirectory = Path.Combine(exeDirectory, "LuaSamples");
                OnOpenLuaFile(samplesDirectory);
            });

            SaveLuaFileAsCommand = new DelegateCommand(() => OnSaveLuaFileAs(Environment.CurrentDirectory));
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

        public DelegateCommand OpenLuaFileCommand { get; }

        public DelegateCommand OpenLuaSampleCommand { get; }

        public DelegateCommand SaveLuaFileAsCommand { get; }

        private void OnRunCode()
        {
            var vm = new SandboxViewModel();
            vm.Code = code;
            var window = new SandboxView();
            window.DataContext = vm;
            window.Show();
        }

        private void OnOpenLuaFile(string? initialDirectory = null)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Lua files (*.lua)|*.lua";
            dialog.DefaultExt = ".lua";

            if (initialDirectory != null)
            {
                dialog.InitialDirectory = initialDirectory;
            }

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                var code = File.ReadAllText(filePath);
                Code = code;
            }
        }

        private void OnSaveLuaFileAs(string? initialDirectory)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Lua files (*.lua)|*.lua";
            dialog.DefaultExt = ".lua";

            if (initialDirectory != null)
            {
                dialog.InitialDirectory = initialDirectory;
            }

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                File.WriteAllText(path, code);
            }
        }
    }
}
