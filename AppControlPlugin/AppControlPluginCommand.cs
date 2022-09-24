// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace AppControlPlugin
{
    public class AppControlPluginCommand : PluginCommand
    {
        public string Response = "";
        public string ApplicationId = "";
        public string[] KeyNames = { "" };
    }
}
