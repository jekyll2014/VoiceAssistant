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
