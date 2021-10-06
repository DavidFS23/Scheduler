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
            CalculationResult[] TheResult = Scheduler.GenerateDates(TheConfiguration);
            Assert.Single(TheResult);
            Assert.Equal(TheResult[0].NextExecutionTime, TheDateTime.AddDays(TheOccurrenceAmount));
        }

        [Fact]
        public void CALCULATE_DESCRIPTION_WITHOUT_LIMIT_DATES()
        {
            string TheDescription = Scheduler.CalculateDescription(Enumerations.Type.Once, new DateTime(2021, 1, 1, 14, 0, 0), null, null, null);
            string ExpectedText = "Occurs once. Schedule will be used on 01/01/2021 at 14:00";
            Assert.Equal(ExpectedText, TheDescription);
        }

        [Fact]
        public void CALCULATE_DESCRIPTION_WITH_LIMIT_DATES()
        {
            string TheDescription = Scheduler.CalculateDescription(
                Enumerations.Type.Recurring, 
                new DateTime(2021, 1, 1, 14, 0, 0), 
                new DateTime(2021, 1, 5, 16, 0, 0), 
                new DateTime(2021, 1, 20, 18, 0, 0),
                new DateTime(2021, 1, 25, 20, 0, 0));
            string ExpectedText = "Occurs every day. Schedule will be used on 05/01/2021 at 16:00 starting on 20/01/2021 ending on 25/01/2021";
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
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            int TheActualDay = TheOccurrenceAmount;
            foreach (CalculationResult TheResult in TheResults)
            {
                Assert.Equal(TheResult.NextExecutionTime, TheDateTime.AddDays(TheActualDay));
                TheActualDay = TheActualDay + TheOccurrenceAmount;
            }
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
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            int TheActualDay = TheOccurrenceAmount;
            foreach (CalculationResult TheResult in TheResults)
            {
                Assert.Equal(TheResult.NextExecutionTime, TheDateTime.AddDays(TheActualDay * 7));
                TheActualDay = TheActualDay + TheOccurrenceAmount;
            }
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
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            int TheActualDay = TheOccurrenceAmount;
            foreach (CalculationResult TheResult in TheResults)
            {
                Assert.Equal(TheResult.NextExecutionTime, TheDateTime.AddMonths(TheActualDay));
                TheActualDay = TheActualDay + TheOccurrenceAmount;
            }
        }

        [Fact]
        public void CALCULATE_NEXT_DATE_WITHOUT_CONFIG()
        {
            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDates(null));
        }

        [Fact]
        public void CALCULATE_NEXT_DATE_RECURRING_WITHOUT_LIMIT_DATE()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;

            Assert.Throws<ArgumentNullException>(() => Scheduler.GenerateDates(TheConfiguration));
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
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            int TheActualDay = 1;
            foreach (CalculationResult TheResult in TheResults)
            {
                Assert.Equal(TheResult.NextExecutionTime, TheDateTime.AddDays(TheActualDay));
                TheActualDay = TheActualDay + 1;
            }
        }

        [Fact]
        public void NUMBER_OF_DAYS_ONCE()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Once;
            TheConfiguration.OccurrenceAmount = 1;
            CalculationResult[] TheResult = Scheduler.GenerateDates(TheConfiguration);
            Assert.Single(TheResult);
        }

        [Fact]
        public void NUMBER_OF_DAYS_RECURRING_DAILY()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 31);
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            Assert.Equal(30, TheResults.Length);
        }

        [Fact]
        public void NUMBER_OF_DAYS_RECURRING_WEEKLY()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Weekly;
            TheConfiguration.LimitEndDate = new DateTime(2021, 1, 31);
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            Assert.Equal(4, TheResults.Length);
        }

        [Fact]
        public void NUMBER_OF_DAYS_RECURRING_MONTHLY()
        {
            DateTime TheDateTime = new DateTime(2021, 1, 1);
            Configuration TheConfiguration = new Configuration();

            TheConfiguration.CurrentDate = TheDateTime;
            TheConfiguration.Type = Enumerations.Type.Recurring;
            TheConfiguration.OccurrenceAmount = 1;
            TheConfiguration.Occurrence = Enumerations.Occurrence.Monthly;
            TheConfiguration.LimitEndDate = new DateTime(2021, 2, 1);
            CalculationResult[] TheResults = Scheduler.GenerateDates(TheConfiguration);
            Assert.Single(TheResults);
        }

    }
}
