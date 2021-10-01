using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Lurgle.Dates
{
    /// <summary>
    ///     Process date tokens
    /// </summary>
    public static class DateTokens
    {
        /// <summary>
        ///     Validate that a valid date has been passed in yyyy-MM-dd format
        /// </summary>
        /// <param name="value">Date in yyyy-MM-dd format</param>
        /// <returns></returns>
        public static bool ValidDate(string value)
        {
            return Regex.IsMatch(value, "^(([12]\\d{3})-(0[1-9]|1[0-2])-(0[1-9]|[12]\\d|3[01]))$");
        }

        /// <summary>
        ///     Validate that a valid date expression has been passed as Jira-style data expressions (eg. Ww Xd Yh Zm)
        /// </summary>
        /// <param name="value">Date expression as any of Ww Xd Yh Zm</param>
        /// <returns></returns>
        public static bool ValidDateExpression(string value)
        {
            return Regex.IsMatch(value, "^((?:(\\d+)w\\s?)?(?:(\\d+)d\\s?)?(?:(\\d+)h\\s?)?(?:(\\d+)m)?)$",
                RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     Ensure that a date expression passed is valid for Jira=style date expressions including spaces to separate (Ww Xd
        ///     Yh Zm)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SetValidExpression(string value)
        {
            var match = Regex.Match(value, "^((?:(\\d+)w\\s?)?(?:(\\d+)d\\s?)?(?:(\\d+)h\\s?)?(?:(\\d+)m)?)$",
                RegexOptions.IgnoreCase);
            var s = new StringBuilder();
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                s.AppendFormat("{0}w ", match.Groups[2].Value);
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
                s.AppendFormat("{0}d ", match.Groups[3].Value);
            if (!string.IsNullOrEmpty(match.Groups[4].Value))
                s.AppendFormat("{0}h ", match.Groups[4].Value);
            if (!string.IsNullOrEmpty(match.Groups[5].Value))
                s.AppendFormat("{0}m", match.Groups[5].Value);

            return s.ToString().Trim();
        }

        /// <summary>
        ///     Return a valid date time from a date expression of Jira-style date expressions
        /// </summary>
        /// <param name="value">Date expression as any of Ww Xd Yh Zm</param>
        /// <param name="startDate">Optional date time, defaults to DateTime.Now</param>
        /// <returns></returns>
        public static DateTime CalculateDateExpression(string value, DateTime? startDate = null)
        {
            var date = DateTime.Now;
            if (startDate != null)
                date = (DateTime)startDate;

            var match = Regex.Match(value, "^((?:(\\d+)w\\s?)?(?:(\\d+)d\\s?)?(?:(\\d+)h\\s?)?(?:(\\d+)m)?)$",
                RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                date = date.AddDays(int.Parse(match.Groups[2].Value) * 7);
            if (!string.IsNullOrEmpty(match.Groups[3].Value))
                date = date.AddDays(int.Parse(match.Groups[3].Value));
            if (!string.IsNullOrEmpty(match.Groups[4].Value))
                date = date.AddHours(int.Parse(match.Groups[4].Value));
            if (!string.IsNullOrEmpty(match.Groups[5].Value))
                date = date.AddMinutes(int.Parse(match.Groups[5].Value));
            return date;
        }

        /// <summary>
        ///     Handle a list of tokens - simple date tokens, date expression tokens, LogToken/LogTokenLong keypair
        /// </summary>
        /// <param name="values"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IEnumerable<string> HandleTokens(IEnumerable<string> values,
            KeyValuePair<string, string>? token = null)
        {
            return values.Select(x => HandleTokens(x, token)).ToList();
        }

        /// <summary>
        ///     Handle date token replacement - simple date tokens, date expression tokens, LogToken/LogTokenLong keypair
        /// </summary>
        /// <param name="value"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string HandleTokens(string value, KeyValuePair<string, string>? token = null)
        {
            var replaceValue = GetDateExpressionToken(value);
            var replaceParams = new List<string>
            {
                "{d}",
                "{dd}",
                "{ddd}",
                "{dddd}",
                "{M}",
                "{MM}",
                "{MMM}",
                "{MMMM}",
                "{yy}",
                "{yyyy}"
            };

            foreach (var replaceToken in replaceParams)
            {
                var tokenMatch = Regex.Match(replaceToken, "\\{([dmy]+)\\}", RegexOptions.IgnoreCase);
                replaceValue = GetDateToken(replaceValue, tokenMatch, replaceToken);
            }

            if (token == null) return replaceValue;
            var tokenPair = (KeyValuePair<string, string>)token;
            replaceValue = Regex.Replace(replaceValue, "{LogToken}", tokenPair.Key);
            replaceValue = Regex.Replace(replaceValue, "{LogTokenLong}", tokenPair.Value);

            return replaceValue;
        }

        private static string GetDateToken(string value, Match tokenMatch, string token)
        {
            var replaceValue = value;


            var matches = Regex.Matches(value, "(\\{(" + tokenMatch.Groups[1].Value + ")(\\+|\\-)?(\\d+)?\\})",
                RegexOptions.IgnoreCase);
            if (matches.Count > 0)
                for (var i = 0; i < matches.Count; i++)
                {
                    var dateAdd = 0;
                    switch (matches[i].Groups[3].Value)
                    {
                        case "+" when !string.IsNullOrEmpty(matches[i].Groups[4].Value):
                            dateAdd = int.Parse(matches[i].Groups[4].Value);
                            break;
                        case "-" when !string.IsNullOrEmpty(matches[i].Groups[4].Value):
                            dateAdd = -int.Parse(matches[i].Groups[4].Value);
                            break;
                    }

                    replaceValue = Regex.Replace(replaceValue,
                        matches[i].Groups[1].Value.Replace("{", "\\{").Replace("}", "\\}").Replace("+", "\\+")
                            .Replace("-", "\\-"),
                        GetDateValue(token, dateAdd), RegexOptions.IgnoreCase);
                }
            else
                replaceValue = Regex.Replace(replaceValue, token, GetDateValue(token));

            return replaceValue;
        }

        private static string GetDateExpressionToken(string value)
        {
            var replaceValue = value;
            const string matchString =
                "(\\{(d{3}|d{4})?(\\s+)?(d{1,2})?(\\s+|\\/|\\-)?(M{1,4})(\\s+|\\/|\\-)?(y{1,4})(\\+|\\-)?(\\d+)?(d|m|y)?\\})";
            var matches = Regex.Matches(replaceValue,
                matchString, RegexOptions.IgnoreCase);

            for (var matchCount = 0; matchCount < matches.Count; matchCount++)
            {
                var s = new StringBuilder();
                for (var group = 2; group < 9; group++)
                    if (!string.IsNullOrEmpty(matches[matchCount].Groups[group].Value))
                        s.Append(matches[matchCount].Groups[group].Value.Replace("D", "d").Replace("m", "M")
                            .Replace("Y", "y"));
                var dateAdd = 0;

                switch (matches[matchCount].Groups[9].Value)
                {
                    case "+" when !string.IsNullOrEmpty(matches[matchCount].Groups[10].Value):
                        dateAdd = int.Parse(matches[matchCount].Groups[10].Value);
                        break;
                    case "-" when !string.IsNullOrEmpty(matches[matchCount].Groups[10].Value):
                        dateAdd = -int.Parse(matches[matchCount].Groups[10].Value);
                        break;
                }

                var date = DateTime.Today;
                if (!string.IsNullOrEmpty(matches[matchCount].Groups[11].Value))
                    switch (matches[matchCount].Groups[11].Value.ToLower()[0])
                    {
                        case 'd':
                            date = date.AddDays(dateAdd);
                            break;
                        case 'm':
                            date = date.AddMonths(dateAdd);
                            break;
                        case 'y':
                            date = date.AddYears(dateAdd);
                            break;
                    }

                replaceValue = Regex.Replace(replaceValue,
                    matches[matchCount].Groups[1].Value.Replace("{", "\\{").Replace("+", "\\+").Replace("-", "\\-")
                        .Replace("}", "\\}"), date.ToString(s.ToString()), RegexOptions.IgnoreCase);
            }

            return replaceValue;
        }

        private static string GetDateValue(string matchType, int dateAdd = 0)
        {
            var dateValue = string.Empty;
            switch (matchType.ToLower())
            {
                case "{d}":
                    dateValue = DateTime.Today.AddDays(dateAdd).Day.ToString();
                    break;
                case "{dd}":
                    dateValue = DateTime.Today.AddDays(dateAdd).ToString("dd");
                    break;
                case "{ddd}":
                    dateValue = DateTime.Today.AddDays(dateAdd).ToString("ddd");
                    break;
                case "{dddd}":
                    dateValue = DateTime.Today.AddDays(dateAdd).ToString("dddd");
                    break;
                case "{m}":
                    dateValue = DateTime.Today.AddMonths(dateAdd).Month.ToString();
                    break;
                case "{mm}":
                    dateValue = DateTime.Today.AddMonths(dateAdd).ToString("MM");
                    break;
                case "{mmm}":
                    dateValue = DateTime.Today.AddMonths(dateAdd).ToString("MMM");
                    break;
                case "{mmmm}":
                    dateValue = DateTime.Today.AddMonths(dateAdd).ToString("MMMM");
                    break;
                case "{yy}":
                    dateValue = DateTime.Today.AddYears(dateAdd).ToString("yy");
                    break;
                case "{yyyy}":
                    dateValue = DateTime.Today.AddYears(dateAdd).ToString("yyyy");
                    break;
            }

            return dateValue;
        }
    }
}