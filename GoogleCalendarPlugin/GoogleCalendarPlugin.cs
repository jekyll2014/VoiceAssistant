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
        private string[] Scopes = { CalendarService.Scope.Calendar };
        private string ApplicationName = "GoogleCalendarPlugin";
        private string GoogleApiCredentials = "{\"installed\":{\"client_id\":\"830242905552-f9ob7qhv6l21gv4vn8u7d9efo03jglkp.apps.googleusercontent.com\",\"project_id\":\"voiceassistant-345916\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"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\",\"redirect_uris\":[\"http://localhost\"]}}";
        private string credPath = "token.json";

        private static string oneHour = "час";
        private static string twoHours = "часа";
        private static string fiveHours = "часов";

        private static string oneMinute = "минута";
        private static string twoMinutes = "минуты";
        private static string fiveMinutes = "минут";

        private string moreResultsAvailableMessage = "";
        private string noEventsMessage = "";

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
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = GoogleCalendarCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            var events = GetEvents(GoogleApiCredentials, command.CalendarId, command.DaysStart, command.DaysCount, command.MaxEvents, out var moreEvents);

            var eventsMessage = "";
            foreach (var eventItem in events)
            {
                eventsMessage += FormatStringWithClassFields(command.SingleEventMessage, eventItem);
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

            using (var stream =
                //new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                new MemoryStream(Encoding.UTF8.GetBytes(apiCredentials)))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
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
                    foreach (var calendar in calendars.Items)
                    {
                        Console.WriteLine($"ID: {calendar.Id}\r\nDEscription: {calendar.Description}\r\nSummary: {calendar.Summary}");
                    }
                }

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

            moreEvents = !string.IsNullOrEmpty(events.NextPageToken);

            return result.ToArray();
        }

        private string FormatStringWithClassFields(string sample, object sourceClass)
        {
            var result = new StringBuilder();
            var openBracketPosition = sample.IndexOf('{');

            if (openBracketPosition < 0)
            {
                return sample;
            }

            if (openBracketPosition > 0)
            {
                result.Append(sample.Substring(0, openBracketPosition));
            }

            while (openBracketPosition >= 0)
            {
                var closeBracketPosition = sample.IndexOf('}', openBracketPosition + 1);
                if (closeBracketPosition < 0)
                {
                    result.Append(sample.Substring(openBracketPosition));

                    return result.ToString();
                }

                var propertyName = sample.Substring(openBracketPosition + 1, closeBracketPosition - openBracketPosition - 1);

                object propertyValue = null;
                try
                {
                    propertyValue = sourceClass.GetType()?.GetField(propertyName)?.GetValue(sourceClass);

                    if (propertyValue == null)
                    {
                        propertyValue = sourceClass.GetType()?.GetProperty(propertyName)?.GetValue(sourceClass);
                    }
                }
                catch
                {

                }

                if (propertyValue != null)
                {
                    result.Append(propertyValue.ToString());
                }
                else
                {
                    result.Append("{" + propertyName + "}");
                }

                openBracketPosition = sample.IndexOf('{', closeBracketPosition + 1);

                if (openBracketPosition > 0)
                {
                    result.Append(sample.Substring(closeBracketPosition + 1, openBracketPosition - closeBracketPosition - 1));
                }
                else
                {
                    result.Append(sample.Substring(closeBracketPosition + 1));
                }
            }

            return result.ToString();
        }
    }
}
