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
        public void CALCULATE_NEXT_DATE_ONCE(int TheYear, int TheMonth, int TheDay, int TheOccurrenceAmount)
        {
            DateTime TheDateTime = new DateTime(TheYear, TheMonth, TheDay);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Once;
            TheConfiguration.OccurrenceAmount = TheOccurrenceAmount;
            CalculationResult TheResult = Scheduler.GenerateDate(TheConfiguration);
            Assert.Equal(TheResult.NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount));
        }

        [Fact]
        public void CALCULATE_DESCRIPTION_WITHOUT_LIMIT_DATES()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1, 14, 0, 0);
            Configuration TheConfiguration = new Configuration();
            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Once;
            string TheDescription = Scheduler.CalculateDescription(TheConfiguration.CurrentDate, TheConfiguration.Type, new DateTime(2021, 1, 1, 14, 0, 0), null, TheConfiguration.LimitStartDate, TheConfiguration.LimitEndDate, TheConfiguration.WeeklyConfiguration, TheConfiguration.DailyFrecuencyConfiguration);
            string ExpectedText = "Occurs once. Schedule will be used on 01/01/2021 at 14:00";
            Assert.Equal(ExpectedText, TheDescription);
        }

        [Fact]
        public void CALCULATE_DESCRIPTION_WITH_LIMIT_DATES()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 5, 16, 0, 0);
            Configuration TheConfiguration = new Configuration();
            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.LimitStartDate = new DateTime(2021, 1, 20);
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 25);
            string TheDescription = Scheduler.CalculateDescription(TheConfiguration.CurrentDate, TheConfiguration.Type, TheDateTime, null, TheConfiguration.LimitStartDate, TheConfiguration.LimitEndDate, TheConfiguration.WeeklyConfiguration, TheConfiguration.DailyFrecuencyConfiguration);
            string ExpectedText = "Occurs every day. Schedule will be used on 05/01/2021 at 16:00 starting on 20/01/2021 ending on 25/01/2021";
            Assert.Equal(ExpectedText, TheDescription);
        }

        [Theory]
        [InlineData(Enumerations.DailyOccurrence.Hours)]
        [InlineData(Enumerations.DailyOccurrence.Minutes)]
        [InlineData(Enumerations.DailyOccurrence.Seconds)]
        public void CALCULATE_DESCRIPTION_WITH_WEEKLY_CONFIGURATION_AND_DAILY_FRECUENCY(Enumerations.DailyOccurrence TheOccurrenceType)
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = TheOccurrenceType;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(4, 0, 0);
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(8, 0, 0);

            TheConfiguration.WeeklyConfiguration = new WeeklyConfiguration();
            TheConfiguration.WeeklyConfiguration.WeekDays = new Enumerations.Weekday[] { Enumerations.Weekday.Monday, Enumerations.Weekday.Thursday, Enumerations.Weekday.Friday };
            TheConfiguration.WeeklyConfiguration.WeekAmount = 2;

            string TheDescription = Scheduler.CalculateDescription(TheConfiguration.CurrentDate, TheConfiguration.Type, TheDateTime, null, TheConfiguration.LimitStartDate, TheConfiguration.LimitEndDate, TheConfiguration.WeeklyConfiguration, TheConfiguration.DailyFrecuencyConfiguration);
            string ExpectedText = $"Occurs every 2 weeks on monday, thursday and friday every 2 {TheOccurrenceType.ToString().ToLower()} between 4:00 am and 8:00 am starting on 01/01/2020";
            Assert.Equal(ExpectedText, TheDescription);
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void CALCULATE_NEXT_DATE_RECURRING_DAILY(int TheYear, int TheMonth, int TheDay, int TheOccurrenceAmount)
        {
            DateTime TheDateTime = new DateTime(TheYear, TheMonth, TheDay);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = TheOccurrenceAmount;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);

            Assert.Equal(TheResult1.NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount));
            Assert.Equal(TheResult2.NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount * 2));
            Assert.Equal(TheResult3.NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount * 3));
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void CALCULATE_NEXT_DATE_RECURRING_WEEKLY(int TheYear, int TheMonth, int TheDay, int TheOccurrenceAmount)
        {
            DateTime TheDateTime = new DateTime(TheYear, TheMonth, TheDay);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = TheOccurrenceAmount;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Weekly;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);
            Assert.Equal(TheResult1.NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount * 7));
            Assert.Equal(TheResult2.NextExecutionTime, TheDateTime.AddDays((TheOccurrenceAmount * 7) * 2));
            Assert.Equal(TheResult3.NextExecutionTime, TheDateTime.AddDays((TheOccurrenceAmount * 7) * 3));
        }

        [Theory]
        [InlineData(2021, 1, 1, 2)]
        [InlineData(1996, 2, 1, 7)]
        [InlineData(2005, 1, 15, 15)]
        [InlineData(2040, 7, 11, 30)]
        [InlineData(2020, 4, 26, 80)]
        public void CALCULATE_NEXT_DATE_RECURRING_MONTHLY(int TheYear, int TheMonth, int TheDay, int TheOccurrenceAmount)
        {
            DateTime TheDateTime = new DateTime(TheYear, TheMonth, TheDay);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = TheOccurrenceAmount;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Monthly;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);

            Assert.Equal(TheResult1.NextExecutionTime, TheDateTime.AddMonths(TheOccurrenceAmount));
            Assert.Equal(TheResult2.NextExecutionTime, TheDateTime.AddMonths(TheOccurrenceAmount * 2));
            Assert.Equal(TheResult3.NextExecutionTime, TheDateTime.AddMonths(TheOccurrenceAmount * 3));
        }

        [Fact]
        public void CALCULATE_NEXT_DATE_WITHOUT_CONFIG()
        {
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(null));
        }

        [Fact]
        public void CALCULATE_NEXT_DATE_RECURRING_WITHOUT_LIMIT_DATE()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;

            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(TheConfiguration));
        }

        [Fact]
        public void CALCULATE_NEXT_DATE_RECURRING_WITH_HIGH_LIMIT_DATE()
        {
            DateTime TheDateTime = new DateTime(2000, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.LimitEndDate = new DateTime(5000, 1, 1);

            CalculationResult TheResult = Scheduler.GenerateDate(TheConfiguration);
            for (int i = 1; i < 1000; i++)
            {
                Assert.Equal(TheDateTime.AddDays(i), TheResult.NextExecutionTime);
                TheConfiguration.CurrentDate = TheResult.NextExecutionTime;
                TheResult = Scheduler.GenerateDate(TheConfiguration);
            }
        }

        [Fact]
        public void DAILYFRECUENCY_OCURS_ONCE()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Once;
            TheConfiguration.OccurrenceAmount = 2;

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Once;
            TheConfiguration.DailyFrecuencyConfiguration.TimeFrecuency = new TimeSpan(14, 30, 0);

            CalculationResult TheResult = Scheduler.GenerateDate(TheConfiguration);
            Assert.Equal(new DateTime(2021, 1, 3, 14, 30, 0), TheResult.NextExecutionTime);
        }

        [Fact]
        public void DAILYFRECUENCY_OCURS_EVERY_X_HOURS()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.OccurrenceAmount = 2;
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 10);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(17, 0, 0);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult3.NextExecutionTime;
            CalculationResult TheResult4 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult4.NextExecutionTime;
            CalculationResult TheResult5 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult5.NextExecutionTime;
            CalculationResult TheResult6 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult6.NextExecutionTime;
            CalculationResult TheResult7 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult7.NextExecutionTime;
            CalculationResult TheResult8 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult8.NextExecutionTime;
            CalculationResult TheResult9 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult9.NextExecutionTime;
            CalculationResult TheResult10 = Scheduler.GenerateDate(TheConfiguration);

            Assert.Equal(TheResult1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(TheResult2.NextExecutionTime, new DateTime(2021, 1, 1, 15, 0, 0));
            Assert.Equal(TheResult3.NextExecutionTime, new DateTime(2021, 1, 1, 17, 0, 0));
            Assert.Equal(TheResult4.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(TheResult5.NextExecutionTime, new DateTime(2021, 1, 3, 15, 0, 0));
            Assert.Equal(TheResult6.NextExecutionTime, new DateTime(2021, 1, 3, 17, 0, 0));
            Assert.Equal(TheResult7.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(TheResult8.NextExecutionTime, new DateTime(2021, 1, 5, 15, 0, 0));
            Assert.Equal(TheResult9.NextExecutionTime, new DateTime(2021, 1, 5, 17, 0, 0));
            Assert.Equal(TheResult10.NextExecutionTime, new DateTime(2021, 1, 7, 13, 0, 0));
        }

        [Fact]
        public void DAILYFRECUENCY_OCURS_EVERY_X_MINUTES()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.OccurrenceAmount = 2;
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 10);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Minutes;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(13, 6, 0);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult3.NextExecutionTime;
            CalculationResult TheResult4 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult4.NextExecutionTime;
            CalculationResult TheResult5 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult5.NextExecutionTime;
            CalculationResult TheResult6 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult6.NextExecutionTime;
            CalculationResult TheResult7 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult7.NextExecutionTime;
            CalculationResult TheResult8 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult8.NextExecutionTime;
            CalculationResult TheResult9 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult9.NextExecutionTime;
            CalculationResult TheResult10 = Scheduler.GenerateDate(TheConfiguration);

            Assert.Equal(TheResult1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(TheResult2.NextExecutionTime, new DateTime(2021, 1, 1, 13, 2, 0));
            Assert.Equal(TheResult3.NextExecutionTime, new DateTime(2021, 1, 1, 13, 4, 0));
            Assert.Equal(TheResult4.NextExecutionTime, new DateTime(2021, 1, 1, 13, 6, 0));
            Assert.Equal(TheResult5.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(TheResult6.NextExecutionTime, new DateTime(2021, 1, 3, 13, 2, 0));
            Assert.Equal(TheResult7.NextExecutionTime, new DateTime(2021, 1, 3, 13, 4, 0));
            Assert.Equal(TheResult8.NextExecutionTime, new DateTime(2021, 1, 3, 13, 6, 0));
            Assert.Equal(TheResult9.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(TheResult10.NextExecutionTime, new DateTime(2021, 1, 5, 13, 2, 0));
        }

        [Fact]
        public void DAILYFRECUENCY_OCURS_EVERY_X_SECONDS()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.OccurrenceAmount = 2;
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 10);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Seconds;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(13, 0, 0);
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(13, 0, 6);

            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult3.NextExecutionTime;
            CalculationResult TheResult4 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult4.NextExecutionTime;
            CalculationResult TheResult5 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult5.NextExecutionTime;
            CalculationResult TheResult6 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult6.NextExecutionTime;
            CalculationResult TheResult7 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult7.NextExecutionTime;
            CalculationResult TheResult8 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult8.NextExecutionTime;
            CalculationResult TheResult9 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult9.NextExecutionTime;
            CalculationResult TheResult10 = Scheduler.GenerateDate(TheConfiguration);

            Assert.Equal(TheResult1.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 0));
            Assert.Equal(TheResult2.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 2));
            Assert.Equal(TheResult3.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 4));
            Assert.Equal(TheResult4.NextExecutionTime, new DateTime(2021, 1, 1, 13, 0, 6));
            Assert.Equal(TheResult5.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 0));
            Assert.Equal(TheResult6.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 2));
            Assert.Equal(TheResult7.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 4));
            Assert.Equal(TheResult8.NextExecutionTime, new DateTime(2021, 1, 3, 13, 0, 6));
            Assert.Equal(TheResult9.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 0));
            Assert.Equal(TheResult10.NextExecutionTime, new DateTime(2021, 1, 5, 13, 0, 2));
        }

        [Fact]
        public void DAILY_CONFIGURATION_EVERY_MONDAY_THRSDAY_FRIDAY()
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(4, 0, 0);
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(8, 0, 0);

            TheConfiguration.WeeklyConfiguration = new WeeklyConfiguration();
            TheConfiguration.WeeklyConfiguration.WeekDays = new Enumerations.Weekday[] { Enumerations.Weekday.Monday, Enumerations.Weekday.Thursday, Enumerations.Weekday.Friday };
            TheConfiguration.WeeklyConfiguration.WeekAmount = 2;

            #region Calculate Date Calls
            CalculationResult TheResult1 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult1.NextExecutionTime;
            CalculationResult TheResult2 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult2.NextExecutionTime;
            CalculationResult TheResult3 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult3.NextExecutionTime;
            CalculationResult TheResult4 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult4.NextExecutionTime;
            CalculationResult TheResult5 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult5.NextExecutionTime;
            CalculationResult TheResult6 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult6.NextExecutionTime;
            CalculationResult TheResult7 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult7.NextExecutionTime;
            CalculationResult TheResult8 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult8.NextExecutionTime;
            CalculationResult TheResult9 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult9.NextExecutionTime;
            CalculationResult TheResult10 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult10.NextExecutionTime;
            CalculationResult TheResult11 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult11.NextExecutionTime;
            CalculationResult TheResult12 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult12.NextExecutionTime;
            CalculationResult TheResult13 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult13.NextExecutionTime;
            CalculationResult TheResult14 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult14.NextExecutionTime;
            CalculationResult TheResult15 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult15.NextExecutionTime;
            CalculationResult TheResult16 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult16.NextExecutionTime;
            CalculationResult TheResult17 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult17.NextExecutionTime;
            CalculationResult TheResult18 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult18.NextExecutionTime;
            CalculationResult TheResult19 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult19.NextExecutionTime;
            CalculationResult TheResult20 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult20.NextExecutionTime;
            CalculationResult TheResult21 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult21.NextExecutionTime;
            CalculationResult TheResult22 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult22.NextExecutionTime;
            CalculationResult TheResult23 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult23.NextExecutionTime;
            CalculationResult TheResult24 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult24.NextExecutionTime;
            CalculationResult TheResult25 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult25.NextExecutionTime;
            CalculationResult TheResult26 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult26.NextExecutionTime;
            CalculationResult TheResult27 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult27.NextExecutionTime;
            CalculationResult TheResult28 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult28.NextExecutionTime;
            CalculationResult TheResult29 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult29.NextExecutionTime;
            CalculationResult TheResult30 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult30.NextExecutionTime;
            CalculationResult TheResult31 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult31.NextExecutionTime;
            CalculationResult TheResult32 = Scheduler.GenerateDate(TheConfiguration);
            TheConfiguration.CurrentDate = TheResult32.NextExecutionTime;
            CalculationResult TheResult33 = Scheduler.GenerateDate(TheConfiguration);
            #endregion
            #region Asserts
            Assert.Equal(TheResult1.NextExecutionTime, new DateTime(2020, 1, 2, 4, 0, 0));
            Assert.Equal(TheResult2.NextExecutionTime, new DateTime(2020, 1, 2, 6, 0, 0));
            Assert.Equal(TheResult3.NextExecutionTime, new DateTime(2020, 1, 2, 8, 0, 0));
            Assert.Equal(TheResult4.NextExecutionTime, new DateTime(2020, 1, 3, 4, 0, 0));
            Assert.Equal(TheResult5.NextExecutionTime, new DateTime(2020, 1, 3, 6, 0, 0));
            Assert.Equal(TheResult6.NextExecutionTime, new DateTime(2020, 1, 3, 8, 0, 0));
            Assert.Equal(TheResult7.NextExecutionTime, new DateTime(2020, 1, 13, 4, 0, 0));
            Assert.Equal(TheResult8.NextExecutionTime, new DateTime(2020, 1, 13, 6, 0, 0));
            Assert.Equal(TheResult9.NextExecutionTime, new DateTime(2020, 1, 13, 8, 0, 0));
            Assert.Equal(TheResult10.NextExecutionTime, new DateTime(2020, 1, 16, 4, 0, 0));
            Assert.Equal(TheResult11.NextExecutionTime, new DateTime(2020, 1, 16, 6, 0, 0));
            Assert.Equal(TheResult12.NextExecutionTime, new DateTime(2020, 1, 16, 8, 0, 0));
            Assert.Equal(TheResult13.NextExecutionTime, new DateTime(2020, 1, 17, 4, 0, 0));
            Assert.Equal(TheResult14.NextExecutionTime, new DateTime(2020, 1, 17, 6, 0, 0));
            Assert.Equal(TheResult15.NextExecutionTime, new DateTime(2020, 1, 17, 8, 0, 0));
            Assert.Equal(TheResult16.NextExecutionTime, new DateTime(2020, 1, 27, 4, 0, 0));
            Assert.Equal(TheResult17.NextExecutionTime, new DateTime(2020, 1, 27, 6, 0, 0));
            Assert.Equal(TheResult18.NextExecutionTime, new DateTime(2020, 1, 27, 8, 0, 0));
            Assert.Equal(TheResult19.NextExecutionTime, new DateTime(2020, 1, 30, 4, 0, 0));
            Assert.Equal(TheResult20.NextExecutionTime, new DateTime(2020, 1, 30, 6, 0, 0));
            Assert.Equal(TheResult21.NextExecutionTime, new DateTime(2020, 1, 30, 8, 0, 0));
            Assert.Equal(TheResult22.NextExecutionTime, new DateTime(2020, 1, 31, 4, 0, 0));
            Assert.Equal(TheResult23.NextExecutionTime, new DateTime(2020, 1, 31, 6, 0, 0));
            Assert.Equal(TheResult24.NextExecutionTime, new DateTime(2020, 1, 31, 8, 0, 0));
            Assert.Equal(TheResult25.NextExecutionTime, new DateTime(2020, 2, 10, 4, 0, 0));
            Assert.Equal(TheResult26.NextExecutionTime, new DateTime(2020, 2, 10, 6, 0, 0));
            Assert.Equal(TheResult27.NextExecutionTime, new DateTime(2020, 2, 10, 8, 0, 0));
            Assert.Equal(TheResult28.NextExecutionTime, new DateTime(2020, 2, 13, 4, 0, 0));
            Assert.Equal(TheResult29.NextExecutionTime, new DateTime(2020, 2, 13, 6, 0, 0));
            Assert.Equal(TheResult30.NextExecutionTime, new DateTime(2020, 2, 13, 8, 0, 0));
            Assert.Equal(TheResult31.NextExecutionTime, new DateTime(2020, 2, 14, 4, 0, 0));
            Assert.Equal(TheResult32.NextExecutionTime, new DateTime(2020, 2, 14, 6, 0, 0));
            Assert.Equal(TheResult33.NextExecutionTime, new DateTime(2020, 2, 14, 8, 0, 0));
            #endregion
        }

        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public void GETWEEKDAY_METHOD_TEST(DayOfWeek TheDayOfWeek)
        {
            var TheWeekDay = WeeklyConfiguration.GetWeekDay(TheDayOfWeek);
            switch (TheDayOfWeek)
            {
                case DayOfWeek.Monday:
                    Assert.Equal(Enumerations.Weekday.Monday, TheWeekDay);
                    break;
                case DayOfWeek.Tuesday:
                    Assert.Equal(Enumerations.Weekday.Tuesday, TheWeekDay);
                    break;
                case DayOfWeek.Wednesday:
                    Assert.Equal(Enumerations.Weekday.Wednesday, TheWeekDay);
                    break;
                case DayOfWeek.Thursday:
                    Assert.Equal(Enumerations.Weekday.Thursday, TheWeekDay);
                    break;
                case DayOfWeek.Friday:
                    Assert.Equal(Enumerations.Weekday.Friday, TheWeekDay);
                    break;
                case DayOfWeek.Saturday:
                    Assert.Equal(Enumerations.Weekday.Saturday, TheWeekDay);
                    break;
                case DayOfWeek.Sunday:
                default:
                    Assert.Equal(Enumerations.Weekday.Sunday, TheWeekDay);
                    break;
            }
        }

        [Fact]
        public void VALIDATION_CONFIGURATION_NOT_NULL()
        {
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(null));
        }

        [Fact]
        public void VALIDATION_CONFIGURATION_RECURRING_WITHOUT_LIMIT_DATE()
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();
            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(TheConfiguration));
        }

        [Fact]
        public void VALIDATION_CONFIGURATION_DAILY_FRECUENCY_WITHOUT_TIME_START()
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeEnd = new TimeSpan(8, 0, 0);
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(TheConfiguration));
        }

        [Fact]
        public void VALIDATION_CONFIGURATION_DAILY_FRECUENCY_WITHOUT_TIME_END()
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            TheConfiguration.DailyFrecuencyConfiguration.TimeStart = new TimeSpan(4, 0, 0);
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(TheConfiguration));
        }

        [Fact]
        public void VALIDATION_CONFIGURATION_DAILY_FRECUENCY_WITHOUT_TIME_START_AND_TIME_END()
        {
            DateTime TheDateTime = new DateTime(2020, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Daily;
            TheConfiguration.LimitEndDate = new DateTime(2099, 1, 1);

            TheConfiguration.DailyFrecuencyConfiguration = new DailyFrecuency();
            TheConfiguration.DailyFrecuencyConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.DailyFrecuencyConfiguration.DailyOccurrence = Enumerations.DailyOccurrence.Hours;
            TheConfiguration.DailyFrecuencyConfiguration.OccurrenceAmount = 2;
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDate(TheConfiguration));
        }

    }
}
