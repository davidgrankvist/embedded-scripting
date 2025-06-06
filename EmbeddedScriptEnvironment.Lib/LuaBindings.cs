﻿using System.Runtime.InteropServices;

namespace ScriptEnvironment.Lib;

/// <summary>
/// Subset of the Lua C API. For example usage, see https://www.lua.org/pil/24.1.html.
/// </summary>
internal static class LuaBindings
{
    private const string LuaLib = "lua.dll";

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr luaL_newstate();

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_close(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int luaL_loadstring(IntPtr luaState, [MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int lua_pcallk(IntPtr luaState, int nargs, int nresults, int errfunc, int ctx, IntPtr kfn);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int luaopen_base(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int luaopen_table(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int luaopen_string(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int luaopen_math(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_settop(IntPtr luaState, int n);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr lua_tolstring(IntPtr luaState, int n, UIntPtr length);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_pushcclosure(IntPtr luaState, IntPtr fn, int n);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_setglobal(IntPtr luaState, string name);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_pushnumber(IntPtr luaState, double d);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_createtable(IntPtr luaState, int narr, int nrec);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int lua_getglobal(IntPtr luaState, [MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int lua_setfield(IntPtr luaState, int n, [MarshalAs(UnmanagedType.LPStr)] string name);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCFunction(IntPtr luaState);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern double lua_tonumberx(IntPtr luaState, int n, IntPtr isnum);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern int lua_tointegerx(IntPtr luaState, int n, IntPtr isnum);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_pushinteger(IntPtr luaState, int d);

    [DllImport(LuaLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void lua_pushstring(IntPtr luaState, [MarshalAs(UnmanagedType.LPStr)] string name);

    public static int LUA_MULTRET = -1;

    public static int lua_pcall(IntPtr luaState, int nargs, int nresults, int errfunc)
    {
        return lua_pcallk(luaState, nargs, nresults, errfunc, 0, IntPtr.Zero);
    }

    public static int luaL_dostring(IntPtr luaState, string s)
    {
        var status = luaL_loadstring(luaState, s);
        if (status == 0)
        {
            return lua_pcall(luaState, 0, LUA_MULTRET, 0);
        }
        else
        {
            return status;
        }
    }

    public static void lua_pop(IntPtr luaState, int n)
    {
        lua_settop(luaState, -n - 1);
    }

    public static IntPtr lua_tostring(IntPtr luaState, int n)
    {
        return lua_tolstring(luaState, n, UIntPtr.Zero);
    }

    public static void lua_pushcfunction(IntPtr luaState, IntPtr fn)
    {
        lua_pushcclosure(luaState, fn, 0);
    }

    public static void lua_newtable(IntPtr luaState)
    {
        lua_createtable(luaState, 0, 0);
    }

    public static double lua_tonumber(IntPtr luaState, int n)
    {
        return lua_tonumberx(luaState, n, IntPtr.Zero);
    }

    public static int lua_tointeger(IntPtr luaState, int n)
    {
        return lua_tointegerx(luaState, n, IntPtr.Zero);
    }
}
