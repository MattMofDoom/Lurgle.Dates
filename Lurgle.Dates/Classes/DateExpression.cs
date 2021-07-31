using System;
using Lurgle.Dates.Enums;

namespace Lurgle.Dates.Classes
{
    /// <summary>
    ///     Date Expression comprising the day order, day type, day of week, and day of month
    /// </summary>
    public class DateExpression
    {
        /// <summary>
        ///     Date Expression comprising the day order, day type, day of week, and date in month
        /// </summary>
        /// <param name="order"></param>
        /// <param name="type"></param>
        /// <param name="weekday"></param>
        /// <param name="dayofmonth"></param>
        public DateExpression(DayOrder order, DayType type, DayOfWeek weekday, DateTime dayofmonth)
        {
            DayOrder = order;
            DayType = type;
            DayOfWeek = weekday;
            Date = dayofmonth;
        } // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// Order of day in month
        /// </summary>
        public DayOrder DayOrder { get; }
        /// <summary>
        /// Type of day in month
        /// </summary>
        public DayType DayType { get; }
        /// <summary>
        /// Day of week in month
        /// </summary>
        public DayOfWeek DayOfWeek { get; }
        /// <summary>
        /// Date in month
        /// </summary>
        public DateTime Date { get; }
    }
}