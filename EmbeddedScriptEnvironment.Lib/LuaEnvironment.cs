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

    public void RegisterFunction(Delegate fn, string name)
    {
        var cFn = ToLuaCFunction(fn);
        pinnedFunctions.Add(cFn);
        RegisterLuaCFunction(cFn, name);
    }

    private LuaBindings.LuaCFunction ToLuaCFunction(Delegate fn)
    {
        /*
         * Here is the idea:
         *
         * Take a generic delegate and use reflection to figure out the parameter/return types.
         * Pop arguments from the Lua stack. Push results to the Lua stack.
         */
        LuaBindings.LuaCFunction cFn = (state) =>
        {
            var method = fn.Method;
            var parameters = method.GetParameters();
            var args = new object[parameters.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var par = parameters[i];

                if (par.ParameterType == typeof(double))
                {
                    args[i] = LuaBindings.lua_tonumber(luaState, i + 1);
                }
                else if (par.ParameterType == typeof(int))
                {
                    args[i] = LuaBindings.lua_tointeger(luaState, i + 1);
                }
                else if (par.ParameterType == typeof(string))
                {
                    args[i] = Marshal.PtrToStringAnsi(LuaBindings.lua_tostring(luaState, i + 1));
                }
                else
                {
                    throw new NotSupportedException($"Arguments of type {par.ParameterType} are not supported");
                }
            }

            var result = method.Invoke(fn.Target, args);

            if (method.ReturnType == typeof(double))
            {
                LuaBindings.lua_pushnumber(luaState, (double)result);
                return 1;
            }
            else if (method.ReturnType == typeof(int))
            {
                LuaBindings.lua_pushinteger(luaState, (int)result);
                return 1;
            }
            else if (method.ReturnType == typeof(string))
            {
                LuaBindings.lua_pushstring(luaState, (string)result);
                return 1;
            }
            else if (method.ReturnType == typeof(void))
            {
                return 0;
            }
            else
            {
                throw new NotSupportedException($"Return type {method.ReturnType} not supported");
            }
        };
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
