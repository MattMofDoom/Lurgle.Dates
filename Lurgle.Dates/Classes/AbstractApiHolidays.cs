using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
// ReSharper disable ClassNeverInstantiated.Global

namespace Lurgle.Dates.Classes
{
    /// <summary>
    ///     AbstractAPI Holidays API format
    /// </summary>
    public class AbstractApiHolidays
    {
        /// <summary>
        ///     AbstractAPI Holidays API format
        /// </summary>
        /// <param name="name"></param>
        /// <param name="name_local"></param>
        /// <param name="language"></param>
        /// <param name="description"></param>
        /// <param name="country"></param>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="date_year"></param>
        /// <param name="date_month"></param>
        /// <param name="date_day"></param>
        /// <param name="week_day"></param>
        // ReSharper disable InconsistentNaming
        public AbstractApiHolidays(string name, string name_local, string language, string description,
            string country,
            string location, string type, string date, string date_year, string date_month, string date_day,
            string week_day)
        {
            Name = name;
            Name_Local = name_local;
            Language = language;
            Description = description;
            Location = location;
            if (Location.Contains(" - "))
                Locations = Location
                    .Substring(Location.IndexOf(" - ", StringComparison.Ordinal) + 3,
                        Location.Length - Location.IndexOf(" - ", StringComparison.Ordinal) - 3)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList();
            else
                Locations = Location.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

            Country = country;
            Type = type;
            Date = date;
            DateYear = date_year;
            DateMonth = date_month;
            DateDay = date_day;
            WeekDay = week_day;
            LocalStart = DateTime.ParseExact(Date, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
            UtcStart = LocalStart.ToUniversalTime();
            UtcEnd = LocalStart.AddDays(1).ToUniversalTime();
        }

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        ///     Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Local Name
        /// </summary>
        public string Name_Local { get; }

        /// <summary>
        ///     Language
        /// </summary>
        public string Language { get; }

        /// <summary>
        ///     Description
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Country
        /// </summary>
        public string Country { get; }

        /// <summary>
        ///     Location
        /// </summary>
        public string Location { get; }

        /// <summary>
        ///     List of locations
        /// </summary>
        public List<string> Locations { get; }

        /// <summary>
        ///     Holiday type
        /// </summary>
        public string Type { get; }

        /// <summary>
        ///     Local start time
        /// </summary>
        public DateTime LocalStart { get; }

        /// <summary>
        ///     UTC start time
        /// </summary>
        public DateTime UtcStart { get; }

        /// <summary>
        ///     UTC end time
        /// </summary>
        public DateTime UtcEnd { get; }

        /// <summary>
        ///     Date in MM/dd/yy format
        /// </summary>
        public string Date { get; }

        /// <summary>
        ///     Year of holiday
        /// </summary>
        public string DateYear { get; }

        /// <summary>
        ///     Month of holiday
        /// </summary>
        public string DateMonth { get; }

        /// <summary>
        ///     Day of holiday
        /// </summary>
        public string DateDay { get; }

        /// <summary>
        ///     Weekday of holiday
        /// </summary>
        public string WeekDay { get; }
    }
}