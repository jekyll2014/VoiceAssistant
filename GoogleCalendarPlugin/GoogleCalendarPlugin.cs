// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using PluginInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GoogleCalendarPlugin
{
    public partial class GoogleCalendarPlugin : PluginBase
    {
        private readonly GoogleCalendarPluginCommand[] GoogleCalendarCommands;
        private readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private readonly string ApplicationName = "GoogleCalendarPlugin";
        private readonly string GoogleApiCredentials = "";
        private readonly string credPath = "token_json";

        private static readonly string oneHour = "час";
        private static readonly string twoHours = "часа";
        private static readonly string fiveHours = "часов";

        private static readonly string oneMinute = "минута";
        private static readonly string twoMinutes = "минуты";
        private static readonly string fiveMinutes = "минут";

        private readonly string moreResultsAvailableMessage = "";
        private readonly string noEventsMessage = "";
        private readonly string NoDataMessage = "";

        public GoogleCalendarPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<GoogleCalendarPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            GoogleCalendarCommands = configBuilder.ConfigStorage.Commands;

            if (GoogleCalendarCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }
            moreResultsAvailableMessage = configBuilder.ConfigStorage.MoreResultsAvailableMessage;
            noEventsMessage = configBuilder.ConfigStorage.NoEventsMessage;
            NoDataMessage = configBuilder.ConfigStorage.NoDataMessage;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = GoogleCalendarCommands.FirstOrDefault(n => n.Name == commandName);
            if (command == null)
            {
                return;
            }

            var events = GetEvents(GoogleApiCredentials, command.CalendarId, command.DaysStart, command.DaysCount, command.MaxEvents, out var moreEvents);

            if (events == null)
            {
                AudioOut.Speak(NoDataMessage);
                return;
            }

            var eventsMessage = "";
            foreach (var eventItem in events)
            {
                eventsMessage += PluginTools.FormatStringWithClassFields(command.SingleEventMessage, eventItem);
            }

            if (string.IsNullOrEmpty(eventsMessage))
            {
                eventsMessage = noEventsMessage;
            }

            var message = string.Format(command.Response, null, eventsMessage);
            if (moreEvents)
            {
                message += moreResultsAvailableMessage;
            }

            AudioOut.Speak(message);
        }

        private CalendarEvents[] GetEvents(string apiCredentials, string calendarName, int daysStart, int daysCount, int maxEvents, out bool moreEvents)
        {
            moreEvents = false;
            UserCredential credential;

            try
            {
                //new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(apiCredentials)))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore($"{PluginPath}\\{credPath}", true)).Result;
                }

                // Create Google Calendar API service.
                using var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                if (string.IsNullOrEmpty(calendarName))
                {
                    var calendarsRequest = service.CalendarList.List();
                    var calendars = calendarsRequest.Execute();

                    if (calendars != null)
                    {
                        Console.WriteLine("Available calendars:");

                        foreach (var calendar in calendars.Items)
                        {
                            Console.WriteLine($"ID: {calendar.Id}\r\nDescription: {calendar.Description}\r\nSummary: {calendar.Summary}\r\n");
                        }
                    }

                    // set calendar name to default
                    calendarName = "primary";
                }

                // Define parameters of request
                EventsResource.ListRequest request = service.Events.List(calendarName); // "****@gmail.com", "primary", "addressbook#contacts@group.v.calendar.google.com"
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = maxEvents;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                request.TimeMax = DateTime.Today.AddDays(daysCount);
                if (daysStart == 0)
                    request.TimeMin = DateTime.Today.AddDays(daysStart);
                else
                    request.TimeMin = DateTime.Now;

                // List events
                Events events = request.Execute();
                var result = new List<CalendarEvents>();
                if (events.Items != null)
                {
                    foreach (var eventItem in events.Items)
                    {
                        result.Add(new CalendarEvents(eventItem));
                    }
                }

                // do we have more events pages? No use to list too many events at once
                moreEvents = !string.IsNullOrEmpty(events.NextPageToken);

                return result.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't get GoogleCalendar data: {ex.Message}");
            }
            return null;
        }
    }
}
