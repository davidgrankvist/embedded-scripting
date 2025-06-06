using GraphicsSandbox.App.UI.Framework;

using ScriptEnvironment.Lib;

namespace GraphicsSandbox.App.UI
{
    class SandboxViewModel : ViewModelBase, IDisposable
    {
        private readonly LuaEnvironment lua;

        private string consoleText;
        private bool isReadOnly;
        private bool shouldExecute;
        private Task? luaTask;

        private SemaphoreSlim consoleReadSem;
        private int consoleReadStart;

        public SandboxViewModel()
        {
            lua = new LuaEnvironment();
            consoleText = string.Empty;
            Code = string.Empty;
            isReadOnly = true;
            shouldExecute = true;
            luaTask = null;

            lua.RegisterFunction(Print, nameof(Print));
            lua.RegisterFunction(Println, nameof(Println));
            lua.RegisterFunction((int x) => Thread.Sleep(x), "Sleep");
            lua.RegisterFunction(() => shouldExecute ? 1 : 0, "_ShouldExecute");
            // wrap with lua function to convert the int to a lua bool
            lua.TryExecute("cs.ShouldExecute = function () return cs._ShouldExecute() == 1 end", out _);

            lua.RegisterFunction(() =>
            {
                BeginConsoleRead();
                var subStrLen = consoleText.Length - consoleReadStart - Environment.NewLine.Length;
                if (subStrLen <= 0)
                {
                    return string.Empty;
                }

                var result = consoleText.Substring(consoleReadStart, subStrLen);
                return result;
            }, "Readln");
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

        public string ConsoleText
        {
            get => consoleText; set
            {
                consoleText = value;
                OnPropertyChanged(nameof(ConsoleText));

                if (!IsReadOnly && consoleText.EndsWith(Environment.NewLine))
                {
                    EndConsoleRead();
                }
            }
        }

        public bool IsReadOnly
        {
            get => isReadOnly; set
            {
                isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        private void Print(string message)
        {
            ConsoleText += message;
        }

        private void Println(string message)
        {
            Print(message + Environment.NewLine);
        }

        private void BeginConsoleRead()
        {
            consoleReadSem = new SemaphoreSlim(0);
            consoleReadStart = consoleText.Length;
            IsReadOnly = false;
            consoleReadSem.Wait();
        }

        public void EndConsoleRead()
        {
            IsReadOnly = true;
            consoleReadSem.Release();
        }

        public void Dispose()
        {
            if (!IsReadOnly)
            {
                EndConsoleRead();
            }
            shouldExecute = false;
            luaTask?.Wait();
            lua.Dispose();
        }
    }
}
