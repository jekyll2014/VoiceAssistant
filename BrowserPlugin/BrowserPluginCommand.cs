// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
