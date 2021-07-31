using System;
using System.Collections.Generic;
using System.Globalization;
using Lurgle.Dates.Classes;
using Xunit;
using Xunit.Abstractions;

namespace Lurgle.Dates.Tests
{
    public class DateTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DateTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void UtcDaysOfMonth()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                //Test last day of month
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                var dates = Dates.GetUtcDaysOfMonth("last", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  Last {0:MMMM} (UTC) : {1}", timeNow, string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.ToUniversalTime().Day);

                //Test first day of month
                dates = Dates.GetUtcDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First {1:MMMM} (UTC) : {2}", timeNow.ToUniversalTime(),
                    timeNow.AddMonths(1).ToUniversalTime(), string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.AddDays(1).ToUniversalTime().Day);


                //Test first day of month on 2nd last day of month
                dates = Dates.GetUtcDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value - 1} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First - 1 {1:MMMM} (UTC) : {2}", DateTime.ParseExact(
                        $"{DateTime.Today.Year}-{date.Key}-{date.Value - 1} 8:00",
                        "yyyy-M-d H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime(),
                    timeNow.AddMonths(1), string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.AddDays(1).ToUniversalTime().Day);
            }
        }


        [Fact]
        public void UtcDaysOfMonthDay1()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                //Test last day of month
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{1} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                var dates = Dates.GetUtcDaysOfMonth("last", $"{DateTime.Today.Year}-{date.Key}-{1} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  Last {0:MMMM} (UTC) : {1}", timeNow, string.Join(',', dates));
                Assert.True(dates[0].Day == DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None).ToUniversalTime().Day);

                //Test first day of month
                dates = Dates.GetUtcDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{1} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First {1:MMMM} (UTC) : {2}", timeNow.ToUniversalTime(),
                    timeNow.AddMonths(1).ToUniversalTime(), string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.ToUniversalTime().Day);
            }
        }

        [Fact]
        public void UtcDaysOfMonthWeekday1()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                var dates = Dates.GetUtcDaysOfMonth("first weekday",
                    $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First {1:MMMM} (UTC) : {2:F}", timeNow.ToUniversalTime(),
                    timeNow.AddMonths(1), dates[0]);
                Assert.True(dates[0].DayOfWeek >= DayOfWeek.Sunday && dates[0].DayOfWeek < DayOfWeek.Friday);
            }
        }

        [Fact]
        public void DaysOfMonthWeekday1()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                var dates = Dates.GetDaysOfMonth("first weekday",
                    $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First {1:MMMM} (UTC) : {2:F}", timeNow,
                    timeNow.AddMonths(1), dates[0]);
                Assert.True(dates[0].DayOfWeek >= DayOfWeek.Monday && dates[0].DayOfWeek < DayOfWeek.Saturday);
            }
        }

        [Fact]
        public void UtcFirstDayName()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                foreach (var m in new List<DayOfWeek>
                {
                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday
                })
                {
                    var dates = Dates.GetUtcDaysOfMonth("first " + m,
                        $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                        timeNow);

                    _testOutputHelper.WriteLine("{0:F}: First {1}, {2:F}", timeNow.ToUniversalTime(), m,
                        dates[0]);

                    Assert.True(dates[0].DayOfWeek == m - 1 ||
                                m == DayOfWeek.Sunday && dates[0].DayOfWeek == DayOfWeek.Saturday);
                }
            }
        }

        [Fact]
        public void UtcFirstDayNameDay1()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-1 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                foreach (var m in new List<DayOfWeek>
                {
                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday
                })
                {
                    var dates = Dates.GetUtcDaysOfMonth("first " + m,
                        $"{DateTime.Today.Year}-{date.Key}-1 9:00", "yyyy-M-d H:mm",
                        timeNow);

                    _testOutputHelper.WriteLine("{0:F}: First {1}, {2:F}", timeNow.ToUniversalTime(), m,
                        dates[0]);

                    Assert.True(dates[0].DayOfWeek == m - 1 ||
                                m == DayOfWeek.Sunday && dates[0].DayOfWeek == DayOfWeek.Saturday);
                }
            }
        }

        [Fact]
        public void FirstDayName()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                foreach (var m in new List<DayOfWeek>
                {
                    DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
                    DayOfWeek.Friday, DayOfWeek.Saturday
                })
                {
                    var dates = Dates.GetDaysOfMonth("first " + m,
                        $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                        timeNow);
                    _testOutputHelper.WriteLine("{0}, First {1} in {2:MMMM} : {3}", timeNow, m,
                        timeNow.AddMonths(1), string.Join(',', dates));

                    Assert.True(dates[0].DayOfWeek == m);
                }
            }
        }

        [Fact]
        public void DaysOfMonth()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            var monthEnds = new Dictionary<int, int>
            {
                {1, 31},
                {2, feb},
                {3, 31},
                {4, 30},
                {5, 31},
                {6, 30},
                {7, 31},
                {8, 31},
                {9, 30},
                {10, 31},
                {11, 30},
                {12, 31}
            };

            foreach (var date in monthEnds)
            {
                //Test last day of month
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----", timeNow);

                var dates = Dates.GetDaysOfMonth("last", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  Last {0:MMMM} : {1}", timeNow, string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.Day);

                dates = Dates.GetDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  First {1:MMMM} : {2}", timeNow, timeNow.AddMonths(1),
                    string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.AddDays(1).Day);

                dates = Dates.GetDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value - 1} 9:00",
                    "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  First - 1 {1:MMMM} : {2}", timeNow, timeNow.AddMonths(1),
                    string.Join(',', dates));
                Assert.True(dates[0].Day == timeNow.AddDays(1).Day);
            }
        }

        [Fact]
        public void HolidaysMatch()
        {
            var holiday = new AbstractApiHolidays("Lurgle.Date Day", "", "AU", "", "AU", "Australia - New South Wales",
                "Local holiday", DateTime.Today.ToString("MM/dd/yyyy"), DateTime.Today.Year.ToString(),
                DateTime.Today.Month.ToString(), DateTime.Today.Day.ToString(), DateTime.Today.DayOfWeek.ToString());

            _testOutputHelper.WriteLine("{0} - {1} {2} - {3}", holiday.Name, holiday.Type, holiday.Location,
                holiday.Date);
            Assert.True(Holidays.ValidateHolidays(new List<AbstractApiHolidays> {holiday},
                new List<string> {"National", "Local"}, new List<string> {"Australia", "New South Wales"}, false,
                true).Count > 0);
        }

        [Fact]
        public void DatesExpressed()
        {
            _testOutputHelper.WriteLine(string.Join(",",
                Dates.GetDaysOfMonth("first,last,first weekday,last weekday,first monday", "12:00", "H:mm",
                    DateTime.Now).ToArray()));
            Assert.True(Dates
                .GetDaysOfMonth("first,last,first weekday,last weekday,first monday", "12:00", "H:mm", DateTime.Now)
                .Count > 0);
        }

        [Fact]
        public void DateTokenParse()
        {
            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{d} {M} {yy}"),
                $"{DateTime.Today.Day} {DateTime.Today.Month} {DateTime.Today:yy}");
            Assert.True(DateTokens.HandleTokens("{d} {M} {yy}") ==
                        $"{DateTime.Today.Day} {DateTime.Today.Month} {DateTime.Today:yy}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{dddd-1} {d-1} {MMMM} {yyyy}"),
                string.Format("{0:dddd} {1} {2:MMMM} {2:yyyy}", DateTime.Today.AddDays(-1),
                    DateTime.Today.AddDays(-1).Day, DateTime.Today));
            Assert.True(DateTokens.HandleTokens("{dddd-1} {d-1} {MMMM} {yyyy}") == string.Format(
                "{0:dddd} {1} {2:MMMM} {2:yyyy}", DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1).Day,
                DateTime.Today));

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{dddd+2} {d+2} {MMMM} {yyyy}"),
                string.Format("{0:dddd} {1} {2:MMMM} {2:yyyy}", DateTime.Today.AddDays(2),
                    DateTime.Today.AddDays(2).Day, DateTime.Today));
            Assert.True(DateTokens.HandleTokens("{dddd+2} {d+2} {MMMM} {yyyy}") == string.Format(
                "{0:dddd} {1} {2:MMMM} {2:yyyy}", DateTime.Today.AddDays(2), DateTime.Today.AddDays(2).Day,
                DateTime.Today));

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{d+10} {M+10} {yy-10}"),
                $"{DateTime.Today.AddDays(10).Day} {DateTime.Today.AddMonths(10).Month} {DateTime.Today.AddYears(-10):yy}");
            Assert.True(DateTokens.HandleTokens("{d+10} {M+10} {yy-10}") ==
                        $"{DateTime.Today.AddDays(10).Day} {DateTime.Today.AddMonths(10).Month} {DateTime.Today.AddYears(-10):yy}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{d M y+10d}"),
                $"{DateTime.Today.AddDays(10):d M yyyy}");
            Assert.True(DateTokens.HandleTokens("{d M y+10d}") == $"{DateTime.Today.AddDays(10):d M y}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{dd MM yy+10m}"),
                $"{DateTime.Today.AddMonths(10):dd MM yy}");
            Assert.True(DateTokens.HandleTokens("{dd MM yy+10m}") == $"{DateTime.Today.AddMonths(10):dd MM yy}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{dd MMM yyy+10y}"),
                $"{DateTime.Today.AddYears(10):dd MMM yyy}");
            Assert.True(DateTokens.HandleTokens("{dd MMM yyy+10y}") == $"{DateTime.Today.AddYears(10):dd MMM yyy}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{MMMM yyyy-1m}"),
                $"{DateTime.Today.AddMonths(-1):MMMM yyyy}");
            Assert.True(DateTokens.HandleTokens("{MMMM yyyy-1m}") == $"{DateTime.Today.AddMonths(-1):MMMM yyyy}");

            _testOutputHelper.WriteLine("Compare {0} with {1}", DateTokens.HandleTokens("{MMMM yyyy-1m} {MMMM yyyy}"),
                $"{DateTime.Today.AddMonths(-1):MMMM yyyy} {DateTime.Today:MMMM yyyy}");
            Assert.True(DateTokens.HandleTokens("{MMMM yyyy-1m} {MMMM yyyy}") ==
                        $"{DateTime.Today.AddMonths(-1):MMMM yyyy} {DateTime.Today:MMMM yyyy}");
        }

        [Fact]
        public void TokenHandling()
        {
            var x = new KeyValuePair<string, string>("POWERS", "Superior Power");
            _testOutputHelper.WriteLine("Compare {0} with {1}",
                DateTokens.HandleTokens("This is a test with {LogToken}! {LogTokenLong} for logging events!", x),
                $"This is a test with {x.Key}! {x.Value} for logging events!");
            Assert.True(
                DateTokens.HandleTokens("This is a test with {LogToken}! {LogTokenLong} for logging events!", x) ==
                $"This is a test with {x.Key}! {x.Value} for logging events!");
        }

        [Fact]
        public void ValidDateExpression()
        {
            Assert.True(DateTokens.SetValidExpression("1d1h1m") == "1d 1h 1m");
            Assert.True(DateTokens.SetValidExpression("1d") == "1d");
            Assert.True(DateTokens.SetValidExpression("1d1m") == "1d 1m");
            Assert.True(DateTokens.SetValidExpression("1m") == "1m");
            Assert.True(DateTokens.SetValidExpression("1h") == "1h");
        }
    }
}