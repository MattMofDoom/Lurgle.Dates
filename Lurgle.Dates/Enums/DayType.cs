namespace Lurgle.Dates.Enums
{
    /// <summary>
    ///     Day type within a date expression
    /// </summary>
    public enum DayType
    {
        /// <summary>
        /// Day
        /// </summary>
        Day = 0,
        /// <summary>
        /// Day of Week
        /// </summary>
        DayOfWeek = 1,
        /// <summary>
        /// Day of Month
        /// </summary>
        DayOfMonth = 2,
        /// <summary>
        /// Weekday
        /// </summary>
        Weekday = 3,
        /// <summary>
        /// No match
        /// </summary>
        NoMatch = -1
    }
}