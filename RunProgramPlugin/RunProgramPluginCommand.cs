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
