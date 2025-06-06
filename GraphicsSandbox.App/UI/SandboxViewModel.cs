using GraphicsSandbox.App.UI.Framework;

using ScriptEnvironment.Lib;

namespace GraphicsSandbox.App.UI
{
    class SandboxViewModel : ViewModelBase, IDisposable
    {
        private readonly LuaEnvironment lua;

        private string consoleOutput;
        private bool isReadonly;
        private bool shouldExecute;
        private Task? luaTask;

        public SandboxViewModel()
        {
            lua = new LuaEnvironment();
            consoleOutput = string.Empty;
            Code = string.Empty;
            isReadonly = true;
            shouldExecute = true;
            luaTask = null;

            lua.RegisterFunction(Print, nameof(Print));
            lua.RegisterFunction(Println, nameof(Println));
            lua.RegisterFunction((int x) => Thread.Sleep(x), "Sleep");
            lua.RegisterFunction(() => shouldExecute ? 1 : 0, "_ShouldExecute");
            // wrap with lua function to convert the int to a lua bool
            lua.TryExecute("cs.ShouldExecute = function () return cs._ShouldExecute() == 1 end", out _);
        }

        public string Code { get; set; }

        public void RunCodeInBackground(string code)
        {
            luaTask = Task.Run(() =>
            {
                if (!lua.TryExecute(code, out var errorMessage))
                {
                    Println("Lua error: " + errorMessage);
                }
            });
        }

        public string ConsoleOutput
        {
            get => consoleOutput; set
            {
                consoleOutput = value;
                OnPropertyChanged(nameof(ConsoleOutput));
            }
        }

        public bool IsReadonly
        {
            get => isReadonly; set
            {
                isReadonly = value;
                OnPropertyChanged(nameof(IsReadonly));
            }
        }

        private void Print(string message)
        {
            ConsoleOutput += message;
        }

        private void Println(string message)
        {
            Print(message + Environment.NewLine);
        }

        public void Dispose()
        {
            shouldExecute = false;
            luaTask?.Wait();
            lua.Dispose();
        }
    }
}
