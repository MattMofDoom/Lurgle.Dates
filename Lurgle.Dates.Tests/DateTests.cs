using System;
using System.Collections.Generic;
using System.Globalization;
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
            Dictionary<int, int> monthEnds = new Dictionary<int, int>()
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
                {12, 31},
            };
            
            foreach (var date in monthEnds)
            {
                //Test last day of month
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----",timeNow);

                var dates = Dates.GetDaysOfMonth("last", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  Last {0:MMMM} (UTC) : {1}", timeNow,string.Join(',', dates));
                Assert.True(dates[0] == timeNow.Day);

                //Test first day of month
                dates = Dates.GetUtcDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First {1:MMMM} (UTC) : {2}", timeNow.ToUniversalTime(),timeNow.AddMonths(1).ToUniversalTime(),string.Join(',', dates));
                Assert.True(dates[0] == timeNow.AddDays(1).ToUniversalTime().Day);


                //Test first day of month on 2nd last day of month
                dates = Dates.GetUtcDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value -1} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0}, First - 1 {1:MMMM} (UTC) : {2}", DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value - 1} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime(),timeNow.AddMonths(1), string.Join(',', dates));
                Assert.True(dates[0] == timeNow.AddDays(1).ToUniversalTime().Day);
            }
        }

                [Fact]
        public void DaysOfMonth()
        {
            var feb = 28;
            if (DateTime.IsLeapYear(DateTime.Today.Year))
                feb = 29;
            Dictionary<int, int> monthEnds = new Dictionary<int, int>()
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
                {12, 31},
            };
            
            foreach (var date in monthEnds)
            {
                //Test last day of month
                var timeNow = DateTime.ParseExact($"{DateTime.Today.Year}-{date.Key}-{date.Value} 8:00",
                    "yyyy-M-d H:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                _testOutputHelper.WriteLine("{0:MMM}\n----",timeNow);

                var dates = Dates.GetDaysOfMonth("last", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  Last {0:MMMM} : {1}", timeNow,string.Join(',', dates));
                Assert.True(dates[0] == timeNow.Day);

                dates = Dates.GetDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  First {1:MMMM} : {2}", timeNow,timeNow.AddMonths(1),string.Join(',', dates));
                Assert.True(dates[0] == timeNow.AddDays(1).Day);
                
                dates = Dates.GetDaysOfMonth("first", $"{DateTime.Today.Year}-{date.Key}-{date.Value - 1} 9:00", "yyyy-M-d H:mm",
                    timeNow);
                _testOutputHelper.WriteLine("{0},  First - 1 {1:MMMM} : {2}", timeNow,timeNow.AddMonths(1),string.Join(',', dates));
                Assert.True(dates[0] == timeNow.AddDays(1).Day);
                
            }
        }
    }
}
