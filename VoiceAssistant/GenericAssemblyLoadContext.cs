using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace VoiceAssistant;

public class GenericAssemblyLoadContext<T> : AssemblyLoadContext where T : class
{
    private readonly AssemblyDependencyResolver _resolver;
    private readonly HashSet<string> _assembliesToNotLoadIntoContext;

    public GenericAssemblyLoadContext(string pluginPath) : base(true)
    {
        var pluginInterfaceAssembly = typeof(T).Assembly.FullName;
        _assembliesToNotLoadIntoContext = GetReferencedAssemblyFullNames(pluginInterfaceAssembly);
        _assembliesToNotLoadIntoContext?.Add(pluginInterfaceAssembly);
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    private static HashSet<string> GetReferencedAssemblyFullNames(string referencedBy)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(t => t.FullName == referencedBy)?
            .GetReferencedAssemblies()?
            .Select(t => t.FullName)?
            .ToHashSet();
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        //Do not load the Plugin Interface DLL into the adapter's context
        //otherwise IsAssignableFrom is false. 
        if (_assembliesToNotLoadIntoContext?.Contains(assemblyName.FullName) ?? true)
            return null;

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}