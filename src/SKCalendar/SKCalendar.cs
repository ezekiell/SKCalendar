using System;

namespace SKCalendar
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class SKCalendar
    {
        private static List<SKCalendarDate> _dates;
        private readonly Regex _tokenRegex = new Regex("{([^}]*)}", RegexOptions.Compiled);

        private static List<SKCalendarDate> Dates
        {
            get
            {
                if (_dates != null)
                {
                    return _dates;
                }

                _dates = XMLSerializer.Deserialize<SKCalendarDates>().Dates;
                return _dates;
            }
        }

        /// <summary>
        /// Renders calendar information string according specified format for current day. In the format you can use this tokens:
        /// 
        /// {longday} - renders date in longdate format
        /// {nameday} - renders information about namedays
        /// {holiday} - renders information about official holidays
        /// {worldDay} - renders information about world days, memorial days and other days which are not official holidays
        /// 
        /// Every token can have format specified after semicolon. Format has to have inner token "[value]" to be able render the value of the token:
        /// <example>
        /// {nameday:Meniny má [value].}
        /// </example>
        /// </summary>
        /// <param name="format">Format of the rendering string.
        /// <example>
        /// "Dnes je {longdate}{nameday:, Meniny má [value]}.<br /><strong>{holiday}</strong> {worldday}
        /// </example>
        /// </param>
        /// <returns>Rendered calendar information string.</returns>
        public string RenderCalendarInfoForToday(string format)
        {
            return RenderCalendarInfo(DateTime.Today, format);
        }

        /// <summary>
        /// Renders calendar information string according specified format. In the format you can use this tokens:
        /// 
        /// {longday} - renders date in longdate format
        /// {nameday} - renders information about namedays
        /// {holiday} - renders information about official holidays
        /// {worldDay} - renders information about world days, memorial days and other days which are not official holidays
        /// 
        /// Every token can have format specified after semicolon. Format has to have inner token "[value]" to be able render the value of the token:
        /// <example>
        /// {nameday:Meniny má [value].}
        /// </example>
        /// </summary>
        /// <param name="date">Date which the calendar info should be rendered for.</param>
        /// <param name="format">Format of the rendering string.
        /// <example>
        /// "Dnes je {longdate}{nameday:, Meniny má [value]}.<br /><strong>{holiday}</strong> {worldday}
        /// </example>
        /// </param>
        /// <returns>Rendered calendar information string.</returns>
        public string RenderCalendarInfo(DateTime date, string format)
        {
            var tokensDictionary = new Dictionary<string, string> { { "longdate", date.ToLongDateString() }, { "nameday", GetNamedayForDate(date) }, { "holiday", GetHolidayForDate(date) }, { "worldday", GetWorlddayForDate(date) } };

            var tokens = _tokenRegex.Matches(format);
            var result = format;

            foreach (Match token in tokens)
            {
                var tokenExpressions = token.Groups[1].Value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                var tokenKey = tokenExpressions[0];

                // if this token is not supported, remove it
                if (!tokensDictionary.ContainsKey(tokenKey) || string.IsNullOrWhiteSpace(tokensDictionary[tokenKey]))
                {
                    result = result.Replace(token.Value, string.Empty);
                    continue;
                }

                // token doesn't contain format, we can replace it directly
                if (tokenExpressions.Length == 1)
                {
                    result = result.Replace(token.Value, tokensDictionary[tokenKey]);
                    continue;
                }

                // evaluate token formatting and replace it in result
                var tokenFormat = tokenExpressions[1];
                var tokenValue = tokenFormat.Replace("[value]", tokensDictionary[tokenKey]);

                result = result.Replace(token.Value, tokenValue);
            }

            return result;
        }

        private string GetNamedayForDate(DateTime date)
        {
            var daykey = $"{date.Month:00}{date.Day:00}";
            var calendarDate = Dates.FirstOrDefault(x => x.Date == daykey);

            return !string.IsNullOrEmpty(calendarDate?.Namedays) ? calendarDate.Namedays : string.Empty;
        }

        private string GetHolidayForDate(DateTime date)
        {
            var result = string.Empty;
            var daykey = $"{date.Month:00}{date.Day:00}";

            var easterdays = CountEasterHolidays(date.Year);
            if (easterdays.ContainsKey(daykey))
            {
                result = easterdays[daykey];
            }

            var calendarDate = Dates.FirstOrDefault(x => x.Date == daykey);
            if (calendarDate == null)
            {
                return result;    
            }

            if (!string.IsNullOrEmpty(calendarDate.Holiday))
            {
                result += string.IsNullOrEmpty(result) ? calendarDate.Holiday : ", " + calendarDate.Holiday;
            }

            return result;
        }

        private string GetWorlddayForDate(DateTime date)
        {
            var daykey = $"{date.Month:00}{date.Day:00}";
            var calendarDate = Dates.FirstOrDefault(x => x.Date == daykey);

            return !string.IsNullOrEmpty(calendarDate?.WorldDay) ? calendarDate.WorldDay : string.Empty;
        }

        private Dictionary<string, string> CountEasterHolidays(int year)
        {
            var d = ((19 * (year % 19)) + 24) % 30;
            var e = (5 + (2 * (year % 4)) + (4 * (year % 7)) + (6 * d)) % 7;

            var march = 22 + d + e;
            var april = d + e - 9;

            if (april > 25)
            {
                april = april - 7;
            }

            var sunday = new DateTime(year, march > 30 ? 4 : 3, march > 30 ? april : march);

            return new Dictionary<string, string>
                       {
                           { sunday.AddDays(-2).ToString("MMdd"), "Veľký piatok" },
                           { sunday.AddDays(-1).ToString("MMdd"), "Biela sobota" },
                           { sunday.ToString("MMdd"), "Veľkonočná nedeľa" },
                           { sunday.AddDays(1).ToString("MMdd"), "Veľkonočný pondelok" }
                       };
        }
    }
}
