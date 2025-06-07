using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using GraphicsSandbox.App.Service;
using GraphicsSandbox.App.ViewModel.Framework;

using ScriptEnvironment.Lib;

namespace GraphicsSandbox.App.ViewModel
{
    class SandboxViewModel : ViewModelBase, IDisposable
    {
        private readonly LuaEnvironment lua;
        private readonly Graphics2D graphics;

        private string consoleText;
        private bool isReadOnly;
        private bool shouldExecute;
        private Task? luaTask;

        private SemaphoreSlim consoleReadSem;
        private int consoleReadStart;

        private Stopwatch sw;

        public SandboxViewModel(Graphics2D graphics)
        {
            lua = new LuaEnvironment();
            this.graphics = graphics;
            consoleText = string.Empty;
            Code = string.Empty;
            isReadOnly = true;
            shouldExecute = true;
            luaTask = null;

            RegisterCoreFunctions();
            RegisterGraphicsFunctions();
        }

        private void RegisterCoreFunctions()
        {
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

            lua.RegisterFunction(() =>
            {
                sw = new Stopwatch();
                sw.Start();
            }, "ClockStart");

            lua.RegisterFunction(() =>
            {
                return (int)sw.Elapsed.TotalMilliseconds;
            }, "ClockTicksMs");
        }

        private void RegisterGraphicsFunctions()
        {
            /*
             * The renderer is WPF based, but the Lua code is run from the thread pool.
             * The dispatcher sends back the calls to the UI thread to prevent STA issues.
             */
            var dispatcher = Dispatcher.CurrentDispatcher;
            lua.RegisterFunction(() =>
            {
                dispatcher.Invoke(() => graphics.BeginDraw());
            }, nameof(graphics.BeginDraw));
            lua.RegisterFunction(() =>
            {
                dispatcher.Invoke(() => graphics.EndDraw());
            }, nameof(graphics.EndDraw));

            // flattened args to make C interop easier
            lua.RegisterFunction((int ax, int ay, int bx, int by, int cx, int cy, int r, int g, int b, int a) =>
            {
                dispatcher.Invoke(() => graphics.DrawTriangle(new Point(ax, ay), new Point(bx, by), new Point(cx, cy), Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b)));
            }, "_" + nameof(graphics.DrawTriangle));
            // wrapper in lua that calls the flattened function
            lua.TryExecute("cs.DrawTriangle = function (a, b, c, color) return cs._DrawTriangle(a.x, a.y, b.x, b.y, c.x, c.y, color.r, color.g, color.b, color.a) end", out _);
        }

        public string Code { get; set; }

        public void RunCodeInBackground(string code)
        {
            luaTask = Task.Run(() =>
            {
                try
                {

                    if (!lua.TryExecute(code, out var errorMessage))
                    {
                        Println("Lua error: " + errorMessage);
                    }
                }
                catch (Exception e)
                {
                    Println("A C# call from Lua failed: " + e);
                    if (e.InnerException != null)
                    {
                        Println(e.InnerException.ToString());
                    }
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
