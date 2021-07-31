using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lurgle.Dates
{
    /// <summary>
    /// Parse date strings with optional expressions
    /// </summary>
    public static class DateParse
    {
        /// <summary>
        /// Parse a time string with optional expressions
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(string configValue)
        {
            return ParseDateTime(configValue);
        }

        private static DateTime? ParseDateTime(string configValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return null;
            try
            {
                var now = DateTime.Now;

                if (configValue.Equals("now", StringComparison.CurrentCultureIgnoreCase))
                    return now.ToUniversalTime();

                //Parse a time string if specified
                const string timeExpression = "^((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(configValue, timeExpression))
                {
                    var match = Regex.Match(configValue, timeExpression);
                    return ParseTimeString(match.Groups[1].Value);
                }

                //Parse a date expression if specified
                const string dateExpression = "^(\\d+)(s|m|h|d|w|M)$";
                if (Regex.IsMatch(configValue, dateExpression))
                {
                    var match = Regex.Match(configValue, dateExpression);
                    return ParseDateExpression(now, int.Parse(match.Groups[1].Value), match.Groups[2].Value);
                }

                //Parse a hybrid date expression with time if specified
                const string hybridExpression =
                    "^(\\d+)(s|m|h|d|w|M)\\s+((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(configValue, hybridExpression))
                {
                    var match = Regex.Match(configValue, hybridExpression);
                    var relativeTime = ParseTimeString(match.Groups[3].Value);
                    return relativeTime != null
                        ? ParseDateExpression(((DateTime) relativeTime).ToLocalTime(), int.Parse(match.Groups[1].Value),
                            match.Groups[2].Value)
                        : null;
                }

                //Attempt to parse using culture formatting rules
                if (DateTime.TryParse(configValue, out var date))
                    return date.ToUniversalTime();

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DateTime? ParseDateExpression(DateTime startTime, int time, string expression)
        {
            switch (expression)
            {
                case "s":
                    return startTime.AddSeconds(-time).ToUniversalTime();
                case "m":
                    return startTime.AddMinutes(-time).ToUniversalTime();
                case "h":
                    return startTime.AddHours(-time).ToUniversalTime();
                case "d":
                    return startTime.AddDays(-time).ToUniversalTime();
                case "w":
                    return startTime.AddDays(-(7 * time)).ToUniversalTime();
                case "M":
                    return startTime.AddMonths(-time).ToUniversalTime();
                default:
                    return null;
            }
        }

        private static DateTime? ParseTimeString(string time)
        {
            var timeFormat = "H:mm:ss";
            if (DateTime.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _))
                return DateTime.ParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None).ToUniversalTime();
            timeFormat = "H:mm";
            if (!DateTime.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return null;

            return DateTime.ParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None).ToUniversalTime();
        }
    }
}
