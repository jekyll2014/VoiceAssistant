// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace RunProgramPlugin
{
    public class RunProgramPluginCommand : PluginCommand
    {
        public string Response = "";
        public string CommandLine = "";
        public bool IsStopCommand = false;
        public bool AllowMultiple = false;
    }
}
