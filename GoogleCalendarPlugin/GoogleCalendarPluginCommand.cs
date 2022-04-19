// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace GoogleCalendarPlugin
{
    public class GoogleCalendarPluginCommand : PluginCommand
    {
        public string SingleEventMessage = "";
        public string Response = "";
        public string CalendarId = "";
        public int DaysStart = 0;
        public int DaysCount = 7;
        public int MaxEvents = 10;
    }
}
