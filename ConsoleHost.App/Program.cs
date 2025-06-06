using ScriptEnvironment.Lib;

using var lua = new LuaEnvironment();

lua.RegisterFunction((double x, int y, string s) =>
{
    Console.WriteLine($"Hello! This C# code was called from Lua running in C#. That's nice. It can even interpolate with these arguments: {x}, {y}, {s}");
    return "And look at this, there are return values that can be used inside Lua!";
}, "hello");

while (true)
{
    PrintPrompt();

    var line = string.Empty;
    while ((line =  Console.ReadLine()) != null)
    {
        if(!lua.TryExecute(line, out var errorMessage))
        {
            Console.Error.WriteLine("Lua error: " + errorMessage);
        }
        PrintPrompt();
    }
}

static void PrintPrompt()
{
    Console.Write("> ");
}