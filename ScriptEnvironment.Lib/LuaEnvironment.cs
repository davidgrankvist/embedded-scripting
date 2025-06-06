using System.Runtime.InteropServices;

namespace ScriptEnvironment.Lib;

/// <summary>
/// Runs Lua code and keeps track of registered C# functions.
/// Registered functions are accessed via a global "cs" table.
/// </summary>
public class LuaEnvironment : IDisposable
{
    private const string CsFunctionTableName = "cs";

    private readonly IntPtr luaState;

    private readonly List<LuaBindings.LuaCFunction> pinnedFunctions;

    public LuaEnvironment()
    {
        luaState = LuaBindings.luaL_newstate();
        pinnedFunctions = [];

        LoadStandardLibraries();
        AddGlobalCsTable();
    }

    private void LoadStandardLibraries()
    {
        LuaBindings.luaopen_base(luaState);
        LuaBindings.luaopen_table(luaState);
        LuaBindings.luaopen_string(luaState);
        LuaBindings.luaopen_math(luaState);
    }

    private void AddGlobalCsTable()
    {
        LuaBindings.lua_newtable(luaState);
        LuaBindings.lua_setglobal(luaState, CsFunctionTableName);
    }

    public void RegisterFunction(Action fn, string name)
    {
        var cFn = ToPinnedFunction(fn);
        RegisterLuaCFunction(cFn, name);
    }

    private LuaBindings.LuaCFunction ToPinnedFunction(Action fn)
    {
        LuaBindings.LuaCFunction cFn = (state) =>
        {
            fn();
            return 0;
        };
        pinnedFunctions.Add(cFn);
        return cFn;
    }

    private void RegisterLuaCFunction(LuaBindings.LuaCFunction fn, string name)
    {
        var fnPtr = Marshal.GetFunctionPointerForDelegate(fn);
        LuaBindings.lua_getglobal(luaState, CsFunctionTableName);
        LuaBindings.lua_pushcfunction(luaState, fnPtr);
        LuaBindings.lua_setfield(luaState, -2, name);
    }

    public bool TryExecute(string code, out string errorMessage)
    {
        errorMessage = null;
        var status = LuaBindings.luaL_dostring(luaState, code);
        if (status != 0)
        {
            var strPtr = LuaBindings.lua_tostring(luaState, -1);
            var str = Marshal.PtrToStringAnsi(strPtr);
            errorMessage = str;
        }

        return status == 0;
    }

    public void Dispose()
    {
        LuaBindings.lua_close(luaState);
    }
}
