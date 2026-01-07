// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            if (type == null)
                return null;

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
}
