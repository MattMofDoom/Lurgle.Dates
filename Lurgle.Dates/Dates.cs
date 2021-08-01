using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lurgle.Dates.Classes;
using Lurgle.Dates.Enums;

// ReSharper disable UnusedMember.Global

namespace Lurgle.Dates
{
    /// <summary>
    ///     Date calculations for inclusions/exclusions
    /// </summary>
    public static class Dates
    {
        /// <summary>
        ///     Return a DateExpression based on expressed day order, day type, day match type, and the next local day and month
        ///     interval start
        /// </summary>
        /// <param name="dayOrder"></param>
        /// <param name="dayType"></param>
        /// <param name="matchDay"></param>
        /// <param name="dateRef"></param>
        /// <returns></returns>
        private static DateExpression GetDayOfMonth(DayOrder dayOrder, DayType dayType, DayMatch matchDay,
            DateTime dateRef)
        {
            var firstDay = new DateTime(dateRef.Year, dateRef.Month, 1, dateRef.Hour, dateRef.Minute, dateRef.Second);
            var lastDay = new DateTime(dateRef.Year, dateRef.Month,
                DateTime.DaysInMonth(dateRef.Year, dateRef.Month), dateRef.Hour, dateRef.Minute, dateRef.Second);

            // ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
            // ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (dayType)
            {
                case DayType.DayOfMonth:
                    //Only first or last days of month are handled
                    switch (dayOrder)
                    {
                        case DayOrder.First:
                            return new DateExpression(dayOrder, dayType, firstDay.DayOfWeek, firstDay);
                        case DayOrder.Last:
                            return new DateExpression(dayOrder, dayType, lastDay.DayOfWeek, lastDay);
                        default:
                            return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateRef);
                    }
                case DayType.Weekday:
                    //Only first or last weekday expressions are handled
                    switch (dayOrder)
                    {
                        case DayOrder.First:
                            //Find the first weekday
                            switch (firstDay.DayOfWeek)
                            {
                                case DayOfWeek.Sunday:
                                    firstDay = firstDay.AddDays(1);
                                    break;
                                case DayOfWeek.Saturday:
                                    firstDay = firstDay.AddDays(2);
                                    break;
                            }

                            return new DateExpression(dayOrder, dayType, firstDay.DayOfWeek, firstDay);
                        case DayOrder.Last:
                            //Find the last weekday
                            switch (lastDay.DayOfWeek)
                            {
                                case DayOfWeek.Sunday:
                                    lastDay = lastDay.AddDays(-2);
                                    break;
                                case DayOfWeek.Saturday:
                                    lastDay = lastDay.AddDays(-1);
                                    break;
                            }

                            return new DateExpression(dayOrder, dayType, lastDay.DayOfWeek, lastDay);
                        default:
                            return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateRef);
                    }
                case DayType.DayOfWeek:
                    //Calculate the day of week in the month, according to the DayExpression (First, Second, Third, Fourth, Fifth, Last)
                    if (dayOrder == DayOrder.Last)
                    {
                        while ((int) lastDay.DayOfWeek != (int) matchDay) lastDay = lastDay.AddDays(-1);

                        return new DateExpression(dayOrder, dayType, lastDay.DayOfWeek, lastDay);
                    }
                    else if (dayOrder != DayOrder.None)
                    {
                        //Match the first day of month for this DayOfWeek
                        while ((int) firstDay.DayOfWeek != (int) matchDay) firstDay = firstDay.AddDays(1);

                        //Calculate the nth DayOfWeek
                        firstDay = firstDay.AddDays((int) dayOrder * 7);

                        //Make sure the nth DayOfWeek is still in the same month
                        return dateRef.Month == firstDay.Month
                            ? new DateExpression(dayOrder, dayType, firstDay.DayOfWeek, firstDay)
                            : new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateRef);
                    }

                    return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateRef);
                case DayType.Day:
                    //Just return a date expression for the date passed in
                    return new DateExpression(dayOrder, dayType, dateRef.DayOfWeek, dateRef);
            }

            return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateRef);
        }

        /// <summary>
        ///     Return a date expression given the type of day, using the the next local day and month interval start
        /// </summary>
        /// <param name="day"></param>
        /// <param name="localStart"></param>
        /// <param name="dateNow"></param>
        /// <returns></returns>
        private static DateExpression GetDayType(string day, DateTime localStart, DateTime dateNow)
        {
            if (string.IsNullOrEmpty(day))
                return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateNow);

            var firstDay = new DateTime(dateNow.Year, dateNow.Month, 1, localStart.Hour, localStart.Minute, localStart.Second);
            var nextFirstDay = firstDay.AddMonths(1);
            var lastDay = new DateTime(firstDay.Year, firstDay.Month,
                DateTime.DaysInMonth(firstDay.Year, firstDay.Month), localStart.Hour, localStart.Minute, localStart.Second);
            var nextLastDay = new DateTime(nextFirstDay.Year, nextFirstDay.Month,
                DateTime.DaysInMonth(nextFirstDay.Year, nextFirstDay.Month), localStart.Hour, localStart.Minute, localStart.Second);
            //If it's a simple integer, we can just return the day
            if (int.TryParse(day, out var dayResult) && dayResult > 0)
            {
                var dayOfMonth = GetDayOfMonth(DayOrder.None, DayType.Day, DayMatch.None,  
                    new DateTime(firstDay.Year, firstDay.Month, dayResult, localStart.Hour, localStart.Minute, localStart.Second));
                return dayOfMonth.Date < localStart
                    ? GetDayOfMonth(DayOrder.None, DayType.Day, DayMatch.None,
                        new DateTime(nextFirstDay.Year, nextFirstDay.Month, dayResult, localStart.Hour,
                            localStart.Minute, localStart.Second))
                    : dayOfMonth;
            }

            switch (day.ToLower())
            {
                case "first":
                    var firstOfMonth = GetDayOfMonth(DayOrder.First, DayType.DayOfMonth, DayMatch.None, firstDay);
                    return firstOfMonth.Date < localStart ? GetDayOfMonth(DayOrder.First, DayType.DayOfMonth, DayMatch.None, nextFirstDay) : firstOfMonth;
                case "last":
                    var lastOfMonth = GetDayOfMonth(DayOrder.Last, DayType.DayOfMonth, DayMatch.None, lastDay);
                    return lastOfMonth.Date < localStart ? GetDayOfMonth(DayOrder.Last, DayType.DayOfMonth, DayMatch.None, nextLastDay) : lastOfMonth;
                case "first weekday":
                    var firstWeekday = GetDayOfMonth(DayOrder.First, DayType.Weekday, DayMatch.None, firstDay);
                    return firstWeekday.Date < localStart
                        ? GetDayOfMonth(DayOrder.First, DayType.Weekday, DayMatch.None, nextFirstDay)
                        : firstWeekday;
                case "last weekday":
                    var lastWeekday = GetDayOfMonth(DayOrder.Last, DayType.Weekday, DayMatch.None, lastDay);
                    return lastWeekday.Date < localStart
                        ? GetDayOfMonth(DayOrder.Last, DayType.Weekday, DayMatch.None, nextLastDay)
                        : lastWeekday;
                default:
                    var dayExpressionString = day.Split(' ');
                    if (dayExpressionString.Length != 2)
                        return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateNow);
                    if (!Enum.TryParse(dayExpressionString[0], true, out DayOrder dayExpression) ||
                        dayExpression == DayOrder.None)
                        return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateNow);
                    if (!Enum.TryParse(dayExpressionString[1], true, out DayMatch dayMatch))
                        return new DateExpression(DayOrder.None, DayType.NoMatch, DayOfWeek.Sunday, dateNow);
                    //Month rollover - if the next local start is the last day of the month, ensure that we are returning next month's first day
                    var dayOfMonth = GetDayOfMonth(dayExpression, DayType.DayOfWeek, dayMatch, firstDay);
                    return dayOfMonth.Date < localStart ? GetDayOfMonth(dayExpression, DayType.DayOfWeek, dayMatch, nextFirstDay) : dayOfMonth;
            }
        }

        /// <summary>
        ///     Return the UTC days of month that are included/excluded, given a date expression and start time
        /// </summary>
        /// <param name="dateExpression"></param>
        /// <param name="startTime"></param>
        /// <param name="startFormat"></param>
        /// <param name="timeNow"></param>
        /// <returns></returns>
        public static List<DateTime> GetUtcDaysOfMonth(string dateExpression, string startTime, string startFormat,
            DateTime timeNow)
        {
            var dayResult = new List<DateTime>();
            if (!string.IsNullOrEmpty(dateExpression))
            {
                var dayList = dateExpression.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToList();

                var localStart = DateTime.ParseExact(startTime, startFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                //Always calculate based on next start
                if (localStart < timeNow) localStart = localStart.AddDays(1);
                
                foreach (var dateResult in dayList.Select(resultDay => GetDayType(resultDay, localStart, timeNow)).Where(dateResult => dateResult.DayType != DayType.NoMatch && !dayResult.Contains(dateResult.Date.ToUniversalTime())))
                {
                    dayResult.Add(dateResult.Date.ToUniversalTime());
                }
            }

            dayResult.Sort();
            return dayResult;
        }


        /// <summary>
        ///     Return the UTC days of month that are included/excluded, given a date expression and start time
        /// </summary>
        /// <param name="dateExpression"></param>
        /// <param name="startTime"></param>
        /// <param name="startFormat"></param>
        /// <param name="timeNow"></param>
        /// <returns></returns>
        public static List<DateTime> GetDaysOfMonth(string dateExpression, string startTime, string startFormat,
            DateTime timeNow)
        {
            var dayResult = new List<DateTime>();
            if (!string.IsNullOrEmpty(dateExpression))
            {
                var dayList = dateExpression.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToList();

                var localStart = DateTime.ParseExact(startTime, startFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                
                foreach (var dateResult in dayList.Select(resultDay => GetDayType(resultDay, localStart, timeNow)).Where(dateResult => dateResult.DayType != DayType.NoMatch && !dayResult.Contains(dateResult.Date)))
                {
                    dayResult.Add(dateResult.Date);
                }
            }
            
            dayResult.Sort();
            return dayResult;
        }

        /// <summary>
        ///     Return a list of DayOfWeek, given a comma-delimited string and start time />
        /// </summary>
        /// <param name="daysOfWeek"></param>
        /// <param name="startTime"></param>
        /// <param name="startFormat"></param>
        /// <returns></returns>
        public static List<DayOfWeek> GetUtcDaysOfWeek(string daysOfWeek, string startTime, string startFormat)
        {
            var dayResult = new List<DayOfWeek>();
            var localStart =
                DateTime.ParseExact(startTime, startFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
            var utcStart = localStart.ToUniversalTime();

            //Always calculate based on next start
            if (localStart < DateTime.Now) localStart = localStart.AddDays(1);

            if (!string.IsNullOrEmpty(daysOfWeek))
            {
                var dayArray = daysOfWeek.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToArray();
                if (dayArray.Length > 0)
                    //Calculate days of week based on UTC start times
                    foreach (var day in dayArray)
                        if (Enum.TryParse(day, out DayOfWeek dayOfWeek))
                        {
                            if (localStart.ToUniversalTime().DayOfWeek < localStart.DayOfWeek ||
                                localStart.DayOfWeek == 0 && (int) utcStart.DayOfWeek == 6)
                            {
                                if (dayOfWeek - 1 >= 0)
                                    dayResult.Add(dayOfWeek - 1);
                                else
                                    dayResult.Add(DayOfWeek.Saturday);
                            }
                            else
                            {
                                dayResult.Add(dayOfWeek);
                            }
                        }
            }

            if (dayResult.Count == 0)
                dayResult = new List<DayOfWeek>
                {
                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday
                };

            return dayResult;
        }

        /// <summary>
        ///     Return a list of DayOfWeek, given a comma-delimited string and start time />
        /// </summary>
        /// <param name="daysOfWeek"></param>
        /// <param name="startTime"></param>
        /// <param name="startFormat"></param>
        /// <returns></returns>
        public static List<DayOfWeek> GetDaysOfWeek(string daysOfWeek, string startTime, string startFormat)
        {
            var dayResult = new List<DayOfWeek>();
            var localStart =
                DateTime.ParseExact(startTime, startFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);

            //Always calculate based on next start
            if (localStart < DateTime.Now) localStart = localStart.AddDays(1);

            if (!string.IsNullOrEmpty(daysOfWeek))
            {
                var dayArray = daysOfWeek.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToArray();
                if (dayArray.Length > 0)
                    //Calculate days of week based on UTC start times
                    foreach (var day in dayArray)
                        if (Enum.TryParse(day, out DayOfWeek dayOfWeek))
                        {
                            if (localStart.ToUniversalTime().DayOfWeek < localStart.DayOfWeek)
                            {
                                if (dayOfWeek - 1 >= 0)
                                    dayResult.Add(dayOfWeek - 1);
                                else
                                    dayResult.Add(DayOfWeek.Saturday);
                            }
                            else
                            {
                                dayResult.Add(dayOfWeek);
                            }
                        }
            }

            if (dayResult.Count == 0)
                dayResult = new List<DayOfWeek>
                {
                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday
                };

            return dayResult;
        }
    }
}