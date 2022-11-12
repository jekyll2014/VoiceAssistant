// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using PluginInterface;

namespace VoiceAssistant
{
    public class GenericPluginLoader<T> where T : PluginBase
    {
        private readonly List<GenericAssemblyLoadContext<T>> _loadContexts = new();

        public List<T> LoadAll(string pluginPath, string filter, params object[] constructorArgs)
        {
            var plugins = new List<T>();

            foreach (var filePath in Directory.EnumerateFiles(pluginPath, filter, SearchOption.AllDirectories))
            {
                try
                {
                    var plugin = Load(filePath, constructorArgs);

                    if (plugin != null)
                    {
                        plugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading plugin {filePath}: {ex}");
                }
            }

            return plugins;
        }

        private T Load(string pluginPath, params object[] constructorArgs)
        {
            var loadContext = new GenericAssemblyLoadContext<T>(pluginPath);

            _loadContexts.Add(loadContext);

            var assembly = loadContext.LoadFromAssemblyPath(pluginPath);

            var type = assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t));
            if (type == null) return null;

            var newArgs = new List<object>();
            newArgs.AddRange(constructorArgs);
            newArgs.Add(pluginPath);

            return (T)Activator.CreateInstance(type, newArgs.ToArray());
        }

        public void UnloadAll()
        {
            foreach (var loadContext in _loadContexts) loadContext.Unload();
        }
    }

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
}
