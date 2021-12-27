using System;
using System.Collections.Generic;
using Xunit;

namespace Scheduler.Tests.xUnit
{
    public class SchedulerTests
    {

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void calculate_next_date_once(int year, int month, int day, int occurrenceAmount)
        {
            DateTime dateTime = new DateTime(year, month, day);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = occurrenceAmount
            };
            CalculationResult result = Scheduler.GenerateDate(configuration);
            Assert.Equal(result.NextExecutionTime, dateTime.AddDays(occurrenceAmount));
        }

        [Fact]
        public void calculate_description_without_limit_dates()
        {
            DateTime dateTime = new DateTime(2021, 1, 1, 14, 0, 0);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once
            };
            string description = Scheduler.CalculateDescription(configuration, new DateTime(2021, 1, 1, 14, 0, 0));
            string ExpectedText = "Occurs once. Schedule will be used on 01/01/2021 at 14:00";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void calculate_description_with_limit_dates()
        {
            DateTime dateTime = new DateTime(2021, 1, 5, 16, 0, 0);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                LimitStartDate = new DateTime(2021, 1, 20),
                LimitEndDate = new DateTime(2021, 1, 25)
            };
            string description = Scheduler.CalculateDescription(configuration, dateTime);
            string ExpectedText = "Occurs every day. Schedule will be used on 05/01/2021 at 16:00 starting on 20/01/2021 ending on 25/01/2021";
            Assert.Equal(ExpectedText, description);
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void calculate_next_date_recurring_daily(int year, int month, int day, int occurrenceAmount)
        {
            DateTime dateTime = new DateTime(year, month, day);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = occurrenceAmount,
                LimitEndDate = new DateTime(2099, 1, 1)
            };

            CalculationResult[] results = GetResults(configuration, 3);
            for (int i = 1; i <= 3; i++)
            {
                Assert.Equal(results[i - 1].NextExecutionTime, dateTime.AddDays(occurrenceAmount * i));
            }
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void calculate_next_date_recurring_weekly(int year, int month, int day, int occurrenceAmount)
        {
            DateTime dateTime = new DateTime(year, month, day);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = occurrenceAmount,
                Occurrence = Enumerations.Occurrence.Weekly,
                LimitEndDate = new DateTime(2099, 1, 1)
            };

            CalculationResult[] results = GetResults(configuration, 3);
            for (int i = 1; i <= 3; i++)
            {
                Assert.Equal(results[i - 1].NextExecutionTime, dateTime.AddDays((occurrenceAmount * 7) * i));
            }
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void calculate_next_date_recurring_monthly(int year, int month, int day, int occurrenceAmount)
        {
            DateTime dateTime = new DateTime(year, month, day);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = occurrenceAmount,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1)
            };

            CalculationResult[] results = GetResults(configuration, 3);
            for (int i = 1; i <= 3; i++)
            {
                Assert.Equal(results[i - 1].NextExecutionTime, dateTime.AddMonths(occurrenceAmount * i));
            }
        }

        [Fact]
        public void calculate_next_date_without_config()
        {
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(null));
            string expected = "The parameter Configuration should not be null.";
            Assert.Equal(expected, exception.Message);
        }

        [Fact]
        public void calculate_next_date_recurring_without_limit_date()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expected = "If the configuration is Recurring, you should add Limit End Date.";
            Assert.Equal(expected, exception.Message);
        }

        [Fact]
        public void calculate_next_date_recurring_with_high_limit_date()
        {
            DateTime dateTime = new DateTime(2000, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                LimitEndDate = new DateTime(5000, 1, 1)
            };

            CalculationResult[] results = GetResults(configuration, 1000);
            for (int i = 1; i <= 3; i++)
            {
                Assert.Equal(dateTime.AddDays(i), results[i - 1].NextExecutionTime);
            }
            string description = results[0].Description;
            string ExpectedText = "Occurs every day. Schedule will be used on 02/01/2000 at 00:00 ending on 01/01/5000";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void dailyfrecuency_ocurs_once()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 2,
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Once,
                    TimeFrecuency = new TimeSpan(14, 30, 0)
                }
            };

            CalculationResult[] results = GetResults(configuration, 1);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2021, 1, 3, 14, 30, 0));
            string description = results[0].Description;
            string ExpectedText = "Occurs once. Schedule will be used on 03/01/2021 at 14:30 on 2:30 pm starting on 01/01/2021";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_hours()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                Occurrence = Enumerations.Occurrence.Daily,
                OccurrenceAmount = 2,
                LimitEndDate = new DateTime(2021, 1, 10),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 2,
                    TimeStart = new TimeSpan(13, 0, 0),
                    TimeEnd = new TimeSpan(17, 0, 0)
                }
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2021, 1, 1, 15, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2021, 1, 1, 17, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2021, 1, 3, 15, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2021, 1, 3, 17, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2021, 1, 5, 15, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2021, 1, 5, 17, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2021, 1, 7, 13, 0, 0));

            string description = results[0].Description;
            string ExpectedText = "Occurs every day. Schedule will be used on 01/01/2021 at 13:00 ending on 10/01/2021 every 2 hours between 1:00 pm and 5:00 pm starting on 01/01/2021";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_minutes()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                Occurrence = Enumerations.Occurrence.Daily,
                OccurrenceAmount = 2,
                LimitEndDate = new DateTime(2021, 1, 10),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Minutes,
                    OccurrenceAmount = 2,
                    TimeStart = new TimeSpan(13, 0, 0),
                    TimeEnd = new TimeSpan(13, 6, 0)
                }
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2021, 1, 1, 13, 2, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2021, 1, 1, 13, 4, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2021, 1, 1, 13, 6, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2021, 1, 3, 13, 2, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2021, 1, 3, 13, 4, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2021, 1, 3, 13, 6, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2021, 1, 5, 13, 2, 0));

            string description = results[0].Description;
            string ExpectedText = "Occurs every day. Schedule will be used on 01/01/2021 at 13:00 ending on 10/01/2021 every 2 minutes between 1:00 pm and 1:06 pm starting on 01/01/2021";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_seconds()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                Occurrence = Enumerations.Occurrence.Daily,
                OccurrenceAmount = 2,
                LimitEndDate = new DateTime(2021, 1, 10),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Seconds,
                    OccurrenceAmount = 2,
                    TimeStart = new TimeSpan(13, 0, 0),
                    TimeEnd = new TimeSpan(13, 0, 6)
                }
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 2));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 4));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 6));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 2));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 4));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 6));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 2));


            string description = results[0].Description;
            string ExpectedText = "Occurs every day. Schedule will be used on 01/01/2021 at 13:00 ending on 10/01/2021 every 2 seconds between 1:00 pm and 1:00 pm starting on 01/01/2021";
            Assert.Equal(ExpectedText, description);
        }

        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public void getweekday_method_test(DayOfWeek dayOfWeek)
        {
            var weekDay = Scheduler.GetWeekDay(dayOfWeek);
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    Assert.Equal(Enumerations.Weekday.Monday, weekDay);
                    break;
                case DayOfWeek.Tuesday:
                    Assert.Equal(Enumerations.Weekday.Tuesday, weekDay);
                    break;
                case DayOfWeek.Wednesday:
                    Assert.Equal(Enumerations.Weekday.Wednesday, weekDay);
                    break;
                case DayOfWeek.Thursday:
                    Assert.Equal(Enumerations.Weekday.Thursday, weekDay);
                    break;
                case DayOfWeek.Friday:
                    Assert.Equal(Enumerations.Weekday.Friday, weekDay);
                    break;
                case DayOfWeek.Saturday:
                    Assert.Equal(Enumerations.Weekday.Saturday, weekDay);
                    break;
                case DayOfWeek.Sunday:
                default:
                    Assert.Equal(Enumerations.Weekday.Sunday, weekDay);
                    break;
            }
        }

        [Fact]
        public void validation_configuration_not_null()
        {
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(null));
            string expectedText = "The parameter Configuration should not be null.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validation_configuration_recurring_without_limit_date()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Daily
            };
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "If the configuration is Recurring, you should add Limit End Date.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_start()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Daily,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 2,
                    TimeEnd = new TimeSpan(8, 0, 0)
                }
            };
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Daily Frecuency, you should add Start and End Time.", exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_end()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Daily,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 2,
                    TimeStart = new TimeSpan(4, 0, 0)
                }
            };
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "If the configuration is Daily Frecuency, you should add Start and End Time.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_start_and_time_end()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Daily,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 2
                }
            };
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "If the configuration is Daily Frecuency, you should add Start and End Time.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validation_monthly_configuration_day_8_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 2,
                    TimeStart = new TimeSpan(4, 0, 0),
                    TimeEnd = new TimeSpan(8, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    ConcreteDay = true,
                    DayNumber = 8,
                    ConcreteDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 8, 4, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 8, 6, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 8, 8, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 4, 8, 4, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 8, 6, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 8, 8, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 7, 8, 4, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 7, 8, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 8, 8, 0, 0));


            string description = results[0].Description;
            string ExpectedText = "Occurs every 2 hours between 4:00 am and 8:00 am starting on 01/01/2020";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_thursday_of_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.First,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 2, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 2, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 2, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 2, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 2, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 2, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 2, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 2, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 2, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first thursday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_second_weekend_day_of_every_month()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.Second,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend,
                    SomeDayMonthFrecuency = 1
                }
            };

            CalculationResult[] results = GetResults(configuration, 12);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 5, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 5, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 5, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 5, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 2, 2, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 2, 2, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 2, 2, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 2, 2, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 3, 7, 3, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2020, 3, 7, 4, 0, 0));
            Assert.Equal(results[10].NextExecutionTime, new DateTime(2020, 3, 7, 5, 0, 0));
            Assert.Equal(results[11].NextExecutionTime, new DateTime(2020, 3, 7, 6, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the second weekend of every 1 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void description_monthly_configuration_the_first_thursday_of_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.First,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            string description = result1.Description;
            string expectedText = "Occurs the first thursday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validate_not_concrete_day_and_some_day_same_time()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    ConcreteDay = true,
                    Frecuency = Enumerations.Frecuency.Second,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend,
                    SomeDayMonthFrecuency = 1
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should not select Concrete Day and Some Day at the same time.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_concrete_day_filled_properties_day_number()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    ConcreteDay = true,
                    ConcreteDayMonthFrecuency = 5
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert a positive Day Number if you set Concrete Day.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_concrete_day_filled_properties_month_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    ConcreteDay = true,
                    DayNumber = 1
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert Month Frecuency if you set Concrete Day.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday,
                    SomeDayMonthFrecuency = 3
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert Frecuency if you set Some Day.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_weekday()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.First,
                    SomeDayMonthFrecuency = 3
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert Weekday if you set Some Day.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_month_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.First,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert Month Frecuency if you set Some Day.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_month_frecuency_negative()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2099, 1, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 1,
                    TimeStart = new TimeSpan(3, 0, 0),
                    TimeEnd = new TimeSpan(6, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.First,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday,
                    SomeDayMonthFrecuency = -3
                }
            };

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            string expectedText = "You should insert positive Month Frecuency.";
            Assert.Equal(expectedText, exception.Message);
        }

        private CalculationResult[] GetResults(Configuration configuration, int numberOfResults)
        {
            CalculationResult[] results = new CalculationResult[numberOfResults];
            for (int i = 1; i <= numberOfResults; i++)
            {
                results[i - 1] = Scheduler.GenerateDate(configuration);
                configuration.CurrentDate = results[i - 1].NextExecutionTime;
            }
            return results;
        }
    }
}
