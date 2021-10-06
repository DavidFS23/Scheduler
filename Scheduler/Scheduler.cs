using System;
using System.Collections.Generic;
using System.Linq;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public class Scheduler
    {

        public static CalculationResult[] GenerateDates(Configuration TheConfiguration)
        {
            Scheduler.ValidateConfiguration(TheConfiguration);
            switch (TheConfiguration.Type)
            {
                case Enumerations.Type.Recurring:
                    List<CalculationResult> TheDates = new List<CalculationResult>();
                    int NumberOfDates = TheConfiguration.NumberOfDates;
                    for (int i = 0; i < NumberOfDates; i++)
                    {
                        TheDates.Add(Scheduler.CalculateNextDate(TheConfiguration));
                        TheConfiguration.CurrentDate = TheDates.Last().NextExecutionTime;
                    }
                    return TheDates.ToArray();
                case Enumerations.Type.Once:
                default:
                    return new CalculationResult[] { Scheduler.CalculateNextDate(TheConfiguration) };
            }
        }

        private static CalculationResult CalculateNextDate(Configuration TheConfiguration)
        {
            if (TheConfiguration == null)
            {
                throw new ArgumentNullException("The parameter Configuration should not be null.");
            }
            CalculationResult TheCalculation = new CalculationResult();
            TheCalculation.NextExecutionTime = Scheduler.CalculateExecutionTime(TheConfiguration.Occurrence, TheConfiguration.OccurrenceAmount, TheConfiguration.CurrentDate);
            TheCalculation.Description = Scheduler.CalculateDescription(TheConfiguration.Type, TheCalculation.NextExecutionTime, TheConfiguration.DateTime, TheConfiguration.LimitStartDate, TheConfiguration.LimitEndDate);
            return TheCalculation;
        }

        private static DateTime CalculateExecutionTime(Occurrence TheOccurrence, int TheOccurenceAmount, DateTime TheCurrentDate)
        {
            switch (TheOccurrence)
            {
                case Occurrence.Monthly:
                    return TheCurrentDate.AddMonths(TheOccurenceAmount);
                case Occurrence.Weekly:
                    return TheCurrentDate.AddDays(TheOccurenceAmount * 7);
                case Occurrence.Daily:
                default:
                    return TheCurrentDate.AddDays(TheOccurenceAmount);
            }
        }

        public static string CalculateDescription(Enumerations.Type TheType, DateTime TheNextExecutionTime, DateTime? TheDateTime, DateTime? TheLimitStartDate, DateTime? TheLimitEndDate)
        {
            string TheDescription = string.Empty;
            switch (TheType)
            {
                case Enumerations.Type.Once:
                    TheDescription += "Occurs once. ";
                    break;
                case Enumerations.Type.Recurring:
                    TheDescription += "Occurs every day. ";
                    break;
            }

            DateTime UsedDate = TheNextExecutionTime;
            if (TheDateTime.HasValue)
            {
                UsedDate = TheDateTime.Value;
            }

            TheDescription += $"Schedule will be used on {UsedDate.ToShortDateString()} at {UsedDate.ToString("HH:mm")} ";
            if (TheLimitStartDate.HasValue)
            {
                TheDescription += $"starting on {TheLimitStartDate.Value.ToShortDateString()} ";
            }
            if (TheLimitEndDate.HasValue)
            {
                TheDescription += $"ending on {TheLimitEndDate.Value.ToShortDateString()} ";
            }
            return TheDescription.Trim();
        }

        private static void ValidateConfiguration(Configuration TheConfiguration)
        {
            if (TheConfiguration == null)
            {
                throw new ArgumentNullException("The parameter Configuration should not be null.");
            }
            if (TheConfiguration.Type == Enumerations.Type.Recurring && TheConfiguration.LimitEndDate == null)
            {
                throw new ArgumentNullException("If the configuration is Recurring, you should add Limit End Date.");
            }
        }

    }
}
