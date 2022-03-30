using PluginInterface;

namespace BrowserPlugin
{
    public class BrowserPluginCommand : PluginCommand
    {
        public string Response = "";
        public string URL = "";
        public bool isStopCommand = false;
        public bool useStandAloneBrowser = false;
    }
}
