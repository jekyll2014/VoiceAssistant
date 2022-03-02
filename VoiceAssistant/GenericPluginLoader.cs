using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace VoiceAssistant
{
    public class GenericPluginLoader<T> where T : class
    {
        private readonly List<GenericAssemblyLoadContext<T>> loadContexts = new List<GenericAssemblyLoadContext<T>>();
        public List<T> LoadAll(string pluginPath, string filter = "*Plugin.dll", params object[] constructorArgs)
        {
            List<T> plugins = new List<T>();

            foreach (var filePath in Directory.EnumerateFiles(pluginPath, filter, SearchOption.AllDirectories))
            {
                var plugin = Load(filePath, constructorArgs);

                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }

            return plugins;
        }
        private T Load(string pluginPath, params object[] constructorArgs)
        {
            var loadContext = new GenericAssemblyLoadContext<T>(pluginPath);

            loadContexts.Add(loadContext);

            var assembly = loadContext.LoadFromAssemblyPath(pluginPath);

            var type = assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t));
            if (type == null)
            {
                return null;
            }

            return (T)Activator.CreateInstance(type, constructorArgs);
        }
        public void UnloadAll()
        {
            foreach (var loadContext in loadContexts)
            {
                loadContext.Unload();
            }
        }
    }

    public class GenericAssemblyLoadContext<T> : AssemblyLoadContext where T : class
    {
        private AssemblyDependencyResolver _resolver;
        private HashSet<string> assembliesToNotLoadIntoContext;

        public GenericAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {

            var pluginInterfaceAssembly = typeof(T).Assembly.FullName;
            assembliesToNotLoadIntoContext = GetReferencedAssemblyFullNames(pluginInterfaceAssembly);
            assembliesToNotLoadIntoContext.Add(pluginInterfaceAssembly);

            _resolver = new AssemblyDependencyResolver(pluginPath);
        }
        private HashSet<string> GetReferencedAssemblyFullNames(string ReferencedBy)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies().FirstOrDefault(t => t.FullName == ReferencedBy)
                .GetReferencedAssemblies()
                .Select(t => t.FullName)
                .ToHashSet();
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            //Do not load the Plugin Interface DLL into the adapter's context
            //otherwise IsAssignableFrom is false. 
            if (assembliesToNotLoadIntoContext.Contains(assemblyName.FullName))
            {
                return null;
            }

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
