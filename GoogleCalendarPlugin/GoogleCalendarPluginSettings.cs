// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace GoogleCalendarPlugin
{
    public class GoogleCalendarPluginSettings
    {
        public string[] ConfigurationNote =
    {
            "Interpolation macros:",
            "Only for \"SingleEventMessage\":",
            "{Attendees} - list of attendees",
            "{Description} - event description",
            "{Location} - event location",
            "{Summary} - event summary",
            "{Length} - event duration",
            "{StartYear} - event start year",
            "{StartMonth} - event start month",
            "{StartDay} - event start day",
            "{StarDate} - event start day and month",
            "{StarTime} - event start hour and minute",
            "{EndYear} - event end year",
            "{EndMonth} - event end month",
            "{EndDay} - event end day",
            "{EndDate} - event end day and month",
            "{EndTime} - event end hour and minute",
            "",
            "Only for \"Response\":",
            "{1} - use SingleEventMessage sample",
        };

        //[JsonProperty(Required = Required.Always)]
        public GoogleCalendarPluginCommand[] Commands =
        {
            new GoogleCalendarPluginCommand
            {
                Name = "Today's events",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Календарь", "События", "Расписание"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сегодня"}
                    }
                },
                CalendarId = "",
                DaysStart = 0,
                DaysCount = 1,
                SingleEventMessage = "{StarTime} - {Summary}",
                Response = "Расписание на сегодня: {1}",
                MaxEvents = 10
            },
            new GoogleCalendarPluginCommand
            {
                Name = "Tomorrow's events",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Календарь", "События", "Расписание"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"завтра"}
                    }
                },
                CalendarId = "",
                DaysStart = 1,
                DaysCount = 1,
                SingleEventMessage = "{StarTime} - {Summary}",
                Response = "Расписание на завтра: {1}",
                MaxEvents = 10
            },
            new GoogleCalendarPluginCommand
            {
                Name = "Today's birthdays",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Дни"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"рождения"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сегодня"}
                    }
                },
                CalendarId = "addressbook#contacts@group.v.calendar.google.com",
                DaysStart = 0,
                DaysCount = 1,
                SingleEventMessage = "{Summary}",
                Response = "Дни рождения сегодня: {1}",
                MaxEvents = 10
            },
            new GoogleCalendarPluginCommand
            {
                Name = "Tomorrow's birthdays",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Дни"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"рождения"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "завтра" }
                    }
                },
                CalendarId = "addressbook#contacts@group.v.calendar.google.com",
                DaysStart = 0,
                DaysCount = 1,
                SingleEventMessage = "{Summary}",
                Response = "Дни рождения завтра: {1}",
                MaxEvents = 10
            },
            new GoogleCalendarPluginCommand
            {
                Name = "Upcoming birthdays",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Ближайшие"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"дни"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"рождения"}
                    },
                },
                CalendarId = "addressbook#contacts@group.v.calendar.google.com",
                DaysStart = 0,
                DaysCount = 7,
                SingleEventMessage = "{StarDate} - {Summary}",
                Response = "Приближающиеся дни рождения: {1}",
                MaxEvents = 10
            }
        };

        public string MoreResultsAvailableMessage = "и дальнейшие события, не вошедшие в список";
        public string NoEventsMessage = "пусто";
        public string NoDataMessage = "не могу получить данные с сервера";
    }
}
