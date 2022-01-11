using System;
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
        public void calculate_once_daily_10_times()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Daily,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 1, 2, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 1, 3, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 1, 4, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 1, 5, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 1, 6, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 1, 7, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 1, 8, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 1, 9, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 1, 10, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 1, 11, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs once. Schedule will be used on 02/01/2022 at 00:00 ending on 01/12/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void calculate_once_monthly_10_times()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 2, 1, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 3, 1, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 4, 1, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 5, 1, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 6, 1, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 7, 1, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 8, 1, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 9, 1, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 10, 1, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 11, 1, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs once. Schedule will be used on 01/02/2022 at 00:00 ending on 01/12/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void calculate_once_weekly_10_times()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Weekly,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 1, 8, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 1, 15, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 1, 22, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 1, 29, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 2, 5, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 2, 12, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 2, 19, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 2, 26, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 3, 5, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 3, 12, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs once. Schedule will be used on 08/01/2022 at 00:00 ending on 01/12/2022";
            Assert.Equal(expectedText, description);
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

        [Fact]
        public void calculate_recurring_monthly_and_daily_monday_every_2_hours_starting_at_4am_to_8am()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2022, 2, 1),
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
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.Second,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Monday,
                    SomeDayMonthFrecuency = 2
                }
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 1, 10, 4, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 1, 10, 6, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 1, 10, 8, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 3, 14, 4, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 3, 14, 6, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 3, 14, 8, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 5, 9, 4, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 5, 9, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 5, 9, 8, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 7, 11, 4, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs the second monday of every 2 months every 2 hours between 4:00 am and 8:00 am starting on 01/01/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void calculate_recurring_weekly_and_daily_saturday_every_4_hours_starting_at_4am_to_4pm()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                CurrentDate = dateTime,
                Type = Enumerations.Type.Recurring,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Weekly,
                LimitEndDate = new DateTime(2022, 2, 1),
                DailyFrecuencyConfiguration = new DailyFrecuency()
                {
                    Type = Enumerations.Type.Recurring,
                    DailyOccurrence = Enumerations.DailyOccurrence.Hours,
                    OccurrenceAmount = 4,
                    TimeStart = new TimeSpan(4, 0, 0),
                    TimeEnd = new TimeSpan(16, 0, 0)
                },
                MonthlyConfiguration = new MonthlyConfiguration()
                {
                    SomeDay = true,
                    Frecuency = Enumerations.Frecuency.Second,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Saturday,
                    SomeDayMonthFrecuency = 2
                }
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 1, 1, 4, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 1, 1, 8, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 1, 1, 12, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 1, 1, 16, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 3, 12, 4, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 3, 12, 8, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 3, 12, 12, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 3, 12, 16, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 5, 14, 4, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 5, 14, 8, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs the second saturday of every 2 months every 4 hours between 4:00 am and 4:00 pm starting on 01/01/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_last_weekend_day_of_every_month()
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
                    Frecuency = Enumerations.Frecuency.Last,
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend,
                    SomeDayMonthFrecuency = 1
                }
            };

            CalculationResult[] results = GetResults(configuration, 12);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 26, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 26, 3, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 26, 4, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 26, 5, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 1, 26, 6, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 1, 26, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 1, 26, 3, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 1, 26, 4, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 1, 26, 5, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2020, 1, 26, 6, 0, 0));
            Assert.Equal(results[10].NextExecutionTime, new DateTime(2020, 1, 26, 0, 0, 0));
            Assert.Equal(results[11].NextExecutionTime, new DateTime(2020, 1, 26, 3, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs the last weekend of every 1 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void description_monthly_configuration_the_first_thursday_of_every_3_months_with_extreme_number_repetitions()
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

            CalculationResult[] results = GetResults(configuration, 1000);
            Assert.Equal(results[999].NextExecutionTime, new DateTime(2082, 4, 2, 6, 0, 0));

            string description = results[999].Description;
            string expectedText = "Occurs the first thursday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 02/04/2082";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_monday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Monday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 6, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 6, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 6, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 6, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 6, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 6, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 6, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 6, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 6, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first monday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
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
        public void validation_monthly_configuration_the_first_wednesday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Wednesday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 1, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 1, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 1, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 1, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 1, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 1, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 1, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 1, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 1, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first wednesday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_Tuesday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Tuesday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 7, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 7, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 7, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 7, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 7, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 7, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 7, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 7, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 7, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first tuesday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_friday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Friday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 3, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 3, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 3, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 3, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 3, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 3, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 3, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 3, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 3, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first friday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_saturday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Saturday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 4, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 4, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 4, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 4, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 4, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 4, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 4, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 4, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 4, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first saturday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_sunday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Sunday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 5, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 5, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 5, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 5, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 5, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 5, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 5, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 5, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 5, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first sunday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_weekday_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 1, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 1, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 1, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 1, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 1, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 1, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 1, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 1, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 1, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first weekday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_monthly_configuration_the_first_weekend_of_every_3_months()
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 4, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 4, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 4, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 4, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 4, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 4, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 4, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 4, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 4, 3, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs the first weekend of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void culture_month_order_en_GB()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.enGB),
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 2, 1, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 3, 1, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 4, 1, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 5, 1, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 6, 1, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 7, 1, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 8, 1, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 9, 1, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 10, 1, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 11, 1, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs once. Schedule will be used on 01/02/2022 at 00:00 ending on 01/12/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void culture_month_order_en_US()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.enUS),
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 2, 1, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 3, 1, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 4, 1, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 5, 1, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 6, 1, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 7, 1, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 8, 1, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 9, 1, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 10, 1, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 11, 1, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Occurs once. Schedule will be used on 2/1/2022 at 12:00 AM ending on 12/1/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void culture_month_order_es_ES()
        {
            DateTime dateTime = new DateTime(2022, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.esES),
                CurrentDate = dateTime,
                Type = Enumerations.Type.Once,
                OccurrenceAmount = 1,
                Occurrence = Enumerations.Occurrence.Monthly,
                LimitEndDate = new DateTime(2022, 12, 1)
            };

            CalculationResult[] results = GetResults(configuration, 10);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2022, 2, 1, 0, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2022, 3, 1, 0, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2022, 4, 1, 0, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2022, 5, 1, 0, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2022, 6, 1, 0, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2022, 7, 1, 0, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2022, 8, 1, 0, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2022, 9, 1, 0, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2022, 10, 1, 0, 0, 0));
            Assert.Equal(results[9].NextExecutionTime, new DateTime(2022, 11, 1, 0, 0, 0));

            string description = results[0].Description;
            string expectedText = "Ocurrencia Única. Calendario utilizado el 01/02/2022 a las 0:00 terminando el 01/12/2022";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_culture_same_validation_different_cultures_en_GB()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.enGB),
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

        [Fact]
        public void validation_culture_same_validation_different_cultures_en_US()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.enUS),
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

        [Fact]
        public void validation_culture_same_validation_different_cultures_es_ES()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.esES),
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
            string expectedText = "Debes insertar una Frecuencia Mensual positiva.";
            Assert.Equal(expectedText, exception.Message);
        }

        [Fact]
        public void validation_message_with_default_culture()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                //Language = new Language(Culture.enGB),
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

        [Fact]
        public void validation_culture_with_enumeration_frecuency_es_ES()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.esES),
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Sunday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 5, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 5, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 5, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 5, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 5, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 5, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 5, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 5, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 5, 3, 0, 0));

            string description = results[0].Description;
            string expectedText = "Con ocurrencia el primer domingo de cada 3 meses cada 1 hora entre las 3:00  y las 6:00  empezando en 01/01/2020";
            Assert.Equal(expectedText, description);
        }

        [Fact]
        public void validation_culture_with_enumeration_frecuency_en_GB()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);

            Configuration configuration = new Configuration()
            {
                Language = new Language(Culture.enGB),
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
                    MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Sunday,
                    SomeDayMonthFrecuency = 3
                }
            };

            CalculationResult[] results = GetResults(configuration, 9);
            Assert.Equal(results[0].NextExecutionTime, new DateTime(2020, 1, 5, 3, 0, 0));
            Assert.Equal(results[1].NextExecutionTime, new DateTime(2020, 1, 5, 4, 0, 0));
            Assert.Equal(results[2].NextExecutionTime, new DateTime(2020, 1, 5, 5, 0, 0));
            Assert.Equal(results[3].NextExecutionTime, new DateTime(2020, 1, 5, 6, 0, 0));
            Assert.Equal(results[4].NextExecutionTime, new DateTime(2020, 4, 5, 3, 0, 0));
            Assert.Equal(results[5].NextExecutionTime, new DateTime(2020, 4, 5, 4, 0, 0));
            Assert.Equal(results[6].NextExecutionTime, new DateTime(2020, 4, 5, 5, 0, 0));
            Assert.Equal(results[7].NextExecutionTime, new DateTime(2020, 4, 5, 6, 0, 0));
            Assert.Equal(results[8].NextExecutionTime, new DateTime(2020, 7, 5, 3, 0, 0));


            string description = results[0].Description;
            string expectedText = "Occurs the first sunday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(expectedText, description);
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
