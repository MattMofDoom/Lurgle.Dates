using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Lurgle.Dates
{
    /// <summary>
    ///     Parse date strings with optional expressions
    /// </summary>
    public static class DateParse
    {
        /// <summary>
        ///     Parse a time string with optional expressions as UTC
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeUtc(string value)
        {
            return ParseDateTimeUtc(value);
        }

        /// <summary>
        ///     Parse a time string with optional expressions
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(string value)
        {
            return ParseDateTime(value);
        }

        private static DateTime? ParseDateTimeUtc(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            try
            {
                var now = DateTime.Now;

                if (value.Equals("now", StringComparison.CurrentCultureIgnoreCase))
                    return now.ToUniversalTime();

                //Parse a time string if specified
                const string timeExpression = "^((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(value, timeExpression))
                {
                    var match = Regex.Match(value, timeExpression);
                    return ParseTimeStringUtc(match.Groups[1].Value);
                }

                //Parse a date expression if specified
                const string dateExpression = "^(\\+|\\-)?(\\d+)(s|m|h|d|w|M)$";
                if (Regex.IsMatch(value, dateExpression))
                {
                    var match = Regex.Match(value, dateExpression);
                    return ParseDateExpressionUtc(now, match.Groups[1].Value, int.Parse(match.Groups[2].Value),
                        match.Groups[3].Value);
                }

                //Parse a hybrid date expression with time if specified
                const string hybridExpression =
                    "^(\\+|\\-)?(\\d+)(s|m|h|d|w|M)\\s+((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(value, hybridExpression))
                {
                    var match = Regex.Match(value, hybridExpression);
                    var relativeTime = ParseTimeStringUtc(match.Groups[4].Value);
                    return relativeTime != null
                        ? ParseDateExpressionUtc(((DateTime) relativeTime).ToLocalTime(), match.Groups[1].Value,
                            int.Parse(match.Groups[2].Value),
                            match.Groups[3].Value)
                        : null;
                }

                //Attempt to parse using culture formatting rules
                if (DateTime.TryParse(value, out var date))
                    return date.ToUniversalTime();

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DateTime? ParseDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            try
            {
                var now = DateTime.Now;

                if (value.Equals("now", StringComparison.CurrentCultureIgnoreCase))
                    return now;

                //Parse a time string if specified
                const string timeExpression = "^((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(value, timeExpression))
                {
                    var match = Regex.Match(value, timeExpression);
                    return ParseTimeString(match.Groups[1].Value);
                }

                //Parse a date expression if specified
                const string dateExpression = "^(\\+|\\-)?(\\d+)(s|m|h|d|w|M)$";
                if (Regex.IsMatch(value, dateExpression))
                {
                    var match = Regex.Match(value, dateExpression);
                    return ParseDateExpression(now, match.Groups[1].Value, int.Parse(match.Groups[2].Value),
                        match.Groups[3].Value);
                }

                //Parse a hybrid date expression with time if specified
                const string hybridExpression =
                    "^(\\+|\\-)?(\\d+)(s|m|h|d|w|M)\\s+((?:[0-1]?[0-9]|2[0-3])\\:(?:[0-5][0-9])(?:\\:[0-5][0-9])?)$";
                if (Regex.IsMatch(value, hybridExpression))
                {
                    var match = Regex.Match(value, hybridExpression);
                    var relativeTime = ParseTimeString(match.Groups[4].Value);
                    return relativeTime != null
                        ? ParseDateExpression((DateTime) relativeTime, match.Groups[1].Value,
                            int.Parse(match.Groups[2].Value),
                            match.Groups[3].Value)
                        : null;
                }

                //Attempt to parse using culture formatting rules
                if (DateTime.TryParse(value, out var date))
                    return date;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static DateTime? ParseDateExpressionUtc(DateTime startTime, string operation, int time,
            string expression)
        {
            var operationType = "-";

            if (!string.IsNullOrEmpty(operation))
                operationType = operation;

            switch (expression)
            {
                case "s":
                    return operationType == "+"
                        ? startTime.AddSeconds(time).ToUniversalTime()
                        : startTime.AddSeconds(-time).ToUniversalTime();
                case "m":
                    return operationType == "+"
                        ? startTime.AddMinutes(time).ToUniversalTime()
                        : startTime.AddMinutes(-time).ToUniversalTime();
                case "h":
                    return operationType == "+"
                        ? startTime.AddHours(time).ToUniversalTime()
                        : startTime.AddHours(-time).ToUniversalTime();
                case "d":
                    return operationType == "+"
                        ? startTime.AddDays(time).ToUniversalTime()
                        : startTime.AddDays(-time).ToUniversalTime();
                case "w":
                    return operationType == "+"
                        ? startTime.AddDays(7 * time).ToUniversalTime()
                        : startTime.AddDays(-(7 * time)).ToUniversalTime();
                case "M":
                    return operationType == "+"
                        ? startTime.AddMonths(time).ToUniversalTime()
                        : startTime.AddMonths(-time).ToUniversalTime();
                default:
                    return null;
            }
        }

        private static DateTime? ParseDateExpression(DateTime startTime, string operation, int time, string expression)
        {
            var operationType = "-";

            if (!string.IsNullOrEmpty(operation))
                operationType = operation;

            switch (expression)
            {
                case "s":
                    return operationType == "+" ? startTime.AddSeconds(time) : startTime.AddSeconds(-time);
                case "m":
                    return operationType == "+" ? startTime.AddMinutes(time) : startTime.AddMinutes(-time);
                case "h":
                    return operationType == "+" ? startTime.AddHours(time) : startTime.AddHours(-time);
                case "d":
                    return operationType == "+" ? startTime.AddDays(time) : startTime.AddDays(-time);
                case "w":
                    return operationType == "+" ? startTime.AddDays(7 * time) : startTime.AddDays(-(7 * time));
                case "M":
                    return operationType == "+" ? startTime.AddMonths(time) : startTime.AddMonths(-time);
                default:
                    return null;
            }
        }

        private static DateTime? ParseTimeStringUtc(string time)
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

        private static DateTime? ParseTimeString(string time)
        {
            var timeFormat = "H:mm:ss";
            if (DateTime.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _))
                return DateTime.ParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
            timeFormat = "H:mm";
            if (!DateTime.TryParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
                return null;

            return DateTime.ParseExact(time, timeFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None);
        }
    }
}