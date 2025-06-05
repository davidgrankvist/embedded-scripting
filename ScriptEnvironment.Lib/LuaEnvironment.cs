using System.Runtime.InteropServices;

namespace ScriptEnvironment.Lib;
public class LuaEnvironment : IDisposable
{
    private readonly IntPtr luaState;

    private static LuaBindings.LuaCFunction SampleFunctionDelegate;

    public LuaEnvironment()
    {
        luaState = LuaBindings.luaL_newstate();

        LoadLibraries();
        RegisterFunctions();
    }

    private void LoadLibraries()
    {
        LuaBindings.luaopen_base(luaState);
        LuaBindings.luaopen_table(luaState);
        LuaBindings.luaopen_string(luaState);
        LuaBindings.luaopen_math(luaState);
    }

    private void RegisterFunctions()
    {
        SampleFunctionDelegate = new LuaBindings.LuaCFunction(SampleFunction);
        var fnPtr = Marshal.GetFunctionPointerForDelegate(SampleFunctionDelegate);
        LuaBindings.lua_pushcfunction(luaState, fnPtr);
        LuaBindings.lua_setglobal(luaState, "CS_" + nameof(SampleFunction));
    }

    private static int SampleFunction(IntPtr luaState)
    {
        LuaBindings.lua_pushnumber(luaState, 12345);
        return 1;
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
