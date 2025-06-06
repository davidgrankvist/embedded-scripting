# embedded-scripting

Embed Lua in C#.

## About

This is a project to learn more about embedding Lua. The idea is to host a Lua environment in a C# application
and register C# functions that are available from the Lua code.

### Code example

Here's an example of calling a C# hello world function from Lua.

The hosting C# application sets up an environment, registers functions and runs the Lua code.
```cs
var luaCode = File.ReadAllText(pathToSomeLuaFile);

using var lua = new LuaEnvironment();
lua.RegisterFunction(() => {
    Console.WriteLine("Hello! This C# code was called from Lua."); 
}, "Hello");

if(!lua.TryExecute(luaCode, out var errorMessage))
{
    Console.Error.WriteLine("Lua error: " + errorMessage);
}
```

The Lua code can access the registered functions via the `cs` table
```lua
cs.Hello()
```

### How does it work?

Lua can be built either as an executable to run Lua or as a library with functions to manually control a Lua environment. To embed Lua in C#
you can use the C library DLL and access the functions via interop calls.

The Lua C API supports registering custom C functions, so if you use interop you can register C# code that's available from Lua.


### Code structure

#### EmbeddedScriptEnvironment.Lib 

This library handles the Lua embedding and interop.

#### ConsoleHost.App 

This is a Lua REPL that runs in C#.

#### GraphicsSandbox.App 

This is is a graphical editor where you can write and launch Lua programs. The launched program
receives its own window with a canvas to draw graphics in as well as a virtual console.

For some examples of Lua programs running in this particular environment, see [LuaSamples/](./GraphicsSandbox.App/LuaSamples/).
