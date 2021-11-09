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
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Once;
            configuration.OccurrenceAmount = occurrenceAmount;
            CalculationResult result = Scheduler.GenerateDate(configuration);
            Assert.Equal(result.NextExecutionTime, dateTime.AddDays(occurrenceAmount));
        }

        [Fact]
        public void calculate_description_without_limit_dates()
        {
            DateTime dateTime = new DateTime(2021, 1, 1, 14, 0, 0);
            Configuration configuration = new Configuration();
            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Once;
            string description = Scheduler.CalculateDescription(configuration, new DateTime(2021, 1, 1, 14, 0, 0));
            string ExpectedText = "Occurs once. Schedule will be used on 01/01/2021 at 14:00";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void calculate_description_with_limit_dates()
        {
            DateTime dateTime = new DateTime(2021, 1, 5, 16, 0, 0);
            Configuration configuration = new Configuration();
            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.LimitStartDate = new DateTime(2021, 1, 20);
            configuration.LimitEndDate = new DateTime(2021, 1, 25);
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
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = occurrenceAmount;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, dateTime.AddDays(occurrenceAmount));
            Assert.Equal(result2.NextExecutionTime, dateTime.AddDays(occurrenceAmount * 2));
            Assert.Equal(result3.NextExecutionTime, dateTime.AddDays(occurrenceAmount * 3));
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
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = occurrenceAmount;
            configuration.Occurrence = Enumerations.Occurrence.Weekly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            Assert.Equal(result1.NextExecutionTime, dateTime.AddDays(occurrenceAmount * 7));
            Assert.Equal(result2.NextExecutionTime, dateTime.AddDays((occurrenceAmount * 7) * 2));
            Assert.Equal(result3.NextExecutionTime, dateTime.AddDays((occurrenceAmount * 7) * 3));
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
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = occurrenceAmount;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, dateTime.AddMonths(occurrenceAmount));
            Assert.Equal(result2.NextExecutionTime, dateTime.AddMonths(occurrenceAmount * 2));
            Assert.Equal(result3.NextExecutionTime, dateTime.AddMonths(occurrenceAmount * 3));
        }

        [Fact]
        public void calculate_next_date_without_config()
        {
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(null));
            Assert.Equal("The parameter Configuration should not be null.", exception.Message);
        }

        [Fact]
        public void calculate_next_date_recurring_without_limit_date()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Recurring, you should add Limit End Date.", exception.Message);
        }

        [Fact]
        public void calculate_next_date_recurring_with_high_limit_date()
        {
            DateTime dateTime = new DateTime(2000, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.LimitEndDate = new DateTime(5000, 1, 1);

            CalculationResult result = Scheduler.GenerateDate(configuration);
            for (int i = 1; i < 1000; i++)
            {
                Assert.Equal(dateTime.AddDays(i), result.NextExecutionTime);
                configuration.CurrentDate = result.NextExecutionTime;
                result = Scheduler.GenerateDate(configuration);
            }
        }

        [Fact]
        public void dailyfrecuency_ocurs_once()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Once;
            configuration.OccurrenceAmount = 2;

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Once;
            configuration.DailyFrecuencyConfiguration.TimeFrecuency = new TimeSpan(14, 30, 0);

            CalculationResult result = Scheduler.GenerateDate(configuration);
            Assert.Equal(new DateTime(2021, 1, 3, 14, 30, 0), result.NextExecutionTime);
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_hours()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.OccurrenceAmount = 2;
            configuration.LimitEndDate = new DateTime(2021, 1, 10);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(17, 0, 0);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result9.NextExecutionTime;
            CalculationResult result10 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2021, 1, 1, 15, 0, 0));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2021, 1, 1, 17, 0, 0));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2021, 1, 3, 15, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2021, 1, 3, 17, 0, 0));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2021, 1, 5, 15, 0, 0));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2021, 1, 5, 17, 0, 0));
            Assert.Equal(result10.NextExecutionTime, new DateTime(2021, 1, 7, 13, 0, 0));
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_minutes()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.OccurrenceAmount = 2;
            configuration.LimitEndDate = new DateTime(2021, 1, 10);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Minutes;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(13, 6, 0);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result9.NextExecutionTime;
            CalculationResult result10 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2021, 1, 1, 13, 2, 0));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2021, 1, 1, 13, 4, 0));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2021, 1, 1, 13, 6, 0));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2021, 1, 3, 13, 2, 0));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2021, 1, 3, 13, 4, 0));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2021, 1, 3, 13, 6, 0));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(result10.NextExecutionTime, new DateTime(2021, 1, 5, 13, 2, 0));
        }

        [Fact]
        public void dailyfrecuency_ocurs_every_x_seconds()
        {
            DateTime dateTime = new DateTime(2021, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.OccurrenceAmount = 2;
            configuration.LimitEndDate = new DateTime(2021, 1, 10);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Seconds;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(13, 0, 6);

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result9.NextExecutionTime;
            CalculationResult result10 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 2));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 4));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 6));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 2));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 4));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 6));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(result10.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 2));
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
            Assert.Equal("The parameter Configuration should not be null.", exception.Message);
        }

        [Fact]
        public void validation_configuration_recurring_without_limit_date()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();
            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Recurring, you should add Limit End Date.", exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_start()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(8, 0, 0);
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Daily Frecuency, you should add Start and End Time.", exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_end()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(4, 0, 0);
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Daily Frecuency, you should add Start and End Time.", exception.Message);
        }

        [Fact]
        public void validation_configuration_daily_frecuency_without_time_start_and_time_end()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Daily;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("If the configuration is Daily Frecuency, you should add Start and End Time.", exception.Message);
        }

        [Fact]
        public void validation_monthly_configuration_day_8_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(4, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(8, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.ConcreteDay = true;
            configuration.MonthlyConfiguration.DayNumber = 8;
            configuration.MonthlyConfiguration.ConcreteDayMonthFrecuency = 3;

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2020, 1, 8, 4, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2020, 1, 8, 6, 0, 0));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2020, 1, 8, 8, 0, 0));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2020, 4, 8, 4, 0, 0));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2020, 4, 8, 6, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2020, 4, 8, 8, 0, 0));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2020, 7, 8, 4, 0, 0));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2020, 7, 8, 6, 0, 0));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2020, 7, 8, 8, 0, 0));
        }

        [Fact]
        public void validation_monthly_configuration_the_first_thursday_of_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.First;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 3;

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2020, 1, 2, 3, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2020, 1, 2, 4, 0, 0));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2020, 1, 2, 5, 0, 0));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2020, 1, 2, 6, 0, 0));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2020, 4, 2, 3, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2020, 4, 2, 4, 0, 0));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2020, 4, 2, 5, 0, 0));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2020, 4, 2, 6, 0, 0));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2020, 7, 2, 3, 0, 0));
        }

        [Fact]
        public void validation_monthly_configuration_the_second_weekend_day_of_every_month()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.Second;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 1;

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result1.NextExecutionTime;
            CalculationResult result2 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result2.NextExecutionTime;
            CalculationResult result3 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result3.NextExecutionTime;
            CalculationResult result4 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result4.NextExecutionTime;
            CalculationResult result5 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result5.NextExecutionTime;
            CalculationResult result6 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result6.NextExecutionTime;
            CalculationResult result7 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result7.NextExecutionTime;
            CalculationResult result8 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result8.NextExecutionTime;
            CalculationResult result9 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result9.NextExecutionTime;
            CalculationResult result10 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result10.NextExecutionTime;
            CalculationResult result11 = Scheduler.GenerateDate(configuration);
            configuration.CurrentDate = result11.NextExecutionTime;
            CalculationResult result12 = Scheduler.GenerateDate(configuration);

            Assert.Equal(result1.NextExecutionTime, new DateTime(2020, 1, 5, 3, 0, 0));
            Assert.Equal(result2.NextExecutionTime, new DateTime(2020, 1, 5, 4, 0, 0));
            Assert.Equal(result3.NextExecutionTime, new DateTime(2020, 1, 5, 5, 0, 0));
            Assert.Equal(result4.NextExecutionTime, new DateTime(2020, 1, 5, 6, 0, 0));
            Assert.Equal(result5.NextExecutionTime, new DateTime(2020, 2, 2, 3, 0, 0));
            Assert.Equal(result6.NextExecutionTime, new DateTime(2020, 2, 2, 4, 0, 0));
            Assert.Equal(result7.NextExecutionTime, new DateTime(2020, 2, 2, 5, 0, 0));
            Assert.Equal(result8.NextExecutionTime, new DateTime(2020, 2, 2, 6, 0, 0));
            Assert.Equal(result9.NextExecutionTime, new DateTime(2020, 3, 7, 3, 0, 0));
            Assert.Equal(result10.NextExecutionTime, new DateTime(2020, 3, 7, 4, 0, 0));
            Assert.Equal(result11.NextExecutionTime, new DateTime(2020, 3, 7, 5, 0, 0));
            Assert.Equal(result12.NextExecutionTime, new DateTime(2020, 3, 7, 6, 0, 0));
        }

        [Fact]
        public void description_monthly_configuration_the_first_thursday_of_every_3_months()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.First;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 3;

            CalculationResult result1 = Scheduler.GenerateDate(configuration);
            string description = result1.Description;
            string ExpectedText = "Occurs the first thursday of every 3 months every 1 hour between 3:00 am and 6:00 am starting on 01/01/2020";
            Assert.Equal(ExpectedText, description);
        }

        [Fact]
        public void validate_not_concrete_day_and_some_day_same_time()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.ConcreteDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.Second;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Weekend;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 1;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should not select Concrete Day and Some Day at the same time.", exception.Message);
        }

        [Fact]
        public void validate_concrete_day_filled_properties_day_number()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.ConcreteDay = true;
            configuration.MonthlyConfiguration.ConcreteDayMonthFrecuency = 5;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert a positive Day Number if you set Concrete Day.", exception.Message);
        }

        [Fact]
        public void validate_concrete_day_filled_properties_month_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.ConcreteDay = true;
            configuration.MonthlyConfiguration.DayNumber = 1;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert Month Frecuency if you set Concrete Day.", exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 3;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert Frecuency if you set Some Day.", exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_weekday()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.First;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = 3;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert Weekday if you set Some Day.", exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_month_frecuency()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.First;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert Month Frecuency if you set Some Day.", exception.Message);
        }

        [Fact]
        public void validate_some_day_filled_properties_month_frecuency_negative()
        {
            DateTime dateTime = new DateTime(2020, 1, 1);
            Configuration configuration = new Configuration();

            configuration.CurrentDate = dateTime;
            configuration.Type = Enumerations.Type.Recurring;
            configuration.OccurrenceAmount = 1;
            configuration.Occurrence = Enumerations.Occurrence.Monthly;
            configuration.LimitEndDate = new DateTime(2099, 1, 1);

            configuration.DailyFrecuencyConfiguration = new DailyFrecuency();
            configuration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            configuration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            configuration.DailyFrecuencyConfiguration.OccurrenceAmount = 1;
            configuration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(3, 0, 0);
            configuration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(6, 0, 0);

            configuration.MonthlyConfiguration = new MonthlyConfiguration();
            configuration.MonthlyConfiguration.SomeDay = true;
            configuration.MonthlyConfiguration.Frecuency = Enumerations.Frecuency.First;
            configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay = Enumerations.MonthlyConfigurationWeekDay.Thursday;
            configuration.MonthlyConfiguration.SomeDayMonthFrecuency = -3;

            var exception = Assert.Throws<Exception>(() => Scheduler.GenerateDate(configuration));
            Assert.Equal("You should insert positive Month Frecuency.", exception.Message);
        }
    }
}
