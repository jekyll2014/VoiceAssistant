// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Google.Apis.Calendar.v3.Data;

using System;

namespace GoogleCalendarPlugin
{
    public partial class GoogleCalendarPlugin
    {
        private class CalendarEvents
        {
            public string Attendees = string.Empty;
            public string Description = string.Empty;
            public string Location = string.Empty;
            public string Summary = string.Empty;

            private readonly DateTime? Start;
            public string StartYear => Start?.Year.ToString() ?? string.Empty;
            public string StartMonth => Start?.Month.ToString() ?? string.Empty;
            public string StartDay => Start?.Day.ToString() ?? string.Empty;

            public string StartDate
            {
                get
                {
                    return ((DateTime)Start).ToString("dd MMMM");
                }
            }

            public string StartTime
            {
                get
                {
                    string result = NumberToString(Start?.Hour ?? 0, oneHour, twoHours, fiveHours);
                    result += NumberToString(Start?.Minute ?? 0, oneMinute, twoMinutes, fiveMinutes);

                    return result;
                }
            }

            private readonly DateTime? End;
            public string EndYear => End?.Year.ToString() ?? string.Empty;
            public string EndMonth => End?.Month.ToString() ?? string.Empty;
            public string EndDay => End?.Day.ToString() ?? string.Empty;

            public string EndDate
            {
                get
                {
                    return ((DateTime)End).ToString("dd MMMM");
                }
            }

            public string EndTime
            {
                get
                {
                    string result = NumberToString(End?.Hour ?? 0, oneHour, twoHours, fiveHours);
                    result += NumberToString(End?.Minute ?? 0, oneMinute, twoMinutes, fiveMinutes);

                    return result;
                }
            }

            private int _lengthHour = -1;
            private int LengthHour
            {
                get
                {
                    if (_lengthHour < 0)
                    {
                        if (Start != null && End != null)
                        {
                            var diff = ((DateTime)End).Subtract((DateTime)Start);
                            _lengthHour = (int)diff.TotalHours;
                        }
                        else
                        {
                            _lengthHour = 0;
                        }
                    }

                    return _lengthHour;
                }
            }

            private int _lengthMinute = -1;
            private int LengthMinute
            {
                get
                {
                    if (_lengthMinute < 0)
                    {
                        if (Start != null && End != null)
                        {
                            var diff = ((DateTime)End).Subtract((DateTime)Start);
                            _lengthMinute = (int)diff.TotalMinutes;
                        }
                        else
                        {
                            _lengthMinute = 0;
                        }
                    }

                    return _lengthMinute;
                }
            }

            public CalendarEvents(Event calendarEvent)
            {
                if (calendarEvent.Attendees != null)
                {
                    foreach (var attendee in calendarEvent.Attendees)
                    {
                        Attendees = attendee + ",";
                    }
                }
                else
                {
                    Attendees = string.Empty;
                }

                Description = calendarEvent.Description ?? string.Empty;
                Location = calendarEvent.Location ?? string.Empty;
                Summary = calendarEvent.Summary ?? string.Empty;
                Start = calendarEvent.Start.DateTime ?? default;
                End = calendarEvent.End.DateTime ?? default;
            }

            public string Length
            {
                get
                {
                    string result = NumberToString(LengthHour, oneHour, twoHours, fiveHours);
                    result += NumberToString(LengthMinute, oneMinute, twoMinutes, fiveMinutes);

                    return result;
                }
            }

            private string NumberToString(int number, string one, string two, string five)
            {
                string result = "";

                if (number > 0)
                {
                    int lastNumber = number % 10;
                    result += number.ToString() + " ";
                    if (lastNumber == 1)
                        result += one + " ";
                    else if (lastNumber > 1 && lastNumber < 5)
                        result += two + " ";
                    else if (lastNumber > 5)
                        result += five + " ";
                }

                return result;

            }
        }
    }
}
