using System.Runtime.InteropServices;

namespace ScriptEnvironment.Lib;
public class LuaEnvironment : IDisposable
{
    private readonly IntPtr luaState;

    public LuaEnvironment()
    {
        luaState = LuaBindings.luaL_newstate();

        LuaBindings.luaopen_base(luaState);
        LuaBindings.luaopen_table(luaState);
        LuaBindings.luaopen_string(luaState);
        LuaBindings.luaopen_math(luaState);
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
