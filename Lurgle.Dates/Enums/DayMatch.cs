using System;

namespace Lurgle.Dates.Enums
{
    /// <summary>
    ///     Match days of week, or none
    /// </summary>
    // ReSharper disable UnusedMember.Global
    public enum DayMatch
    {
        /// <summary>
        ///     Sunday
        /// </summary>
        Sunday = DayOfWeek.Sunday,

        /// <summary>
        ///     Monday
        /// </summary>
        Monday = DayOfWeek.Monday,

        /// <summary>
        ///     Tuesday
        /// </summary>
        Tuesday = DayOfWeek.Tuesday,

        /// <summary>
        ///     Wednesday
        /// </summary>
        Wednesday = DayOfWeek.Wednesday,

        /// <summary>
        ///     Thursday
        /// </summary>
        Thursday = DayOfWeek.Thursday,

        /// <summary>
        ///     Friday
        /// </summary>
        Friday = DayOfWeek.Friday,

        /// <summary>
        ///     Saturday
        /// </summary>
        Saturday = DayOfWeek.Saturday,

        /// <summary>
        ///     Sunday
        /// </summary>
        None = -1
    }
}