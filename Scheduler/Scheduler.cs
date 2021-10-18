using System;
using System.Collections.Generic;
using System.Linq;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public class Scheduler
    {

        public static CalculationResult GenerateDate(Configuration TheConfiguration)
        {
            Scheduler.ValidateConfiguration(TheConfiguration);
            return Scheduler.CalculateNextDate(TheConfiguration);
        }

        private static CalculationResult CalculateNextDate(Configuration TheConfiguration)
        {
            if (TheConfiguration == null)
            {
                throw new ArgumentNullException("The parameter Configuration should not be null.");
            }
            CalculationResult TheCalculation = new CalculationResult();
            TheCalculation.NextExecutionTime = Scheduler.CalculateExecutionTime(TheConfiguration.Occurrence, TheConfiguration.OccurrenceAmount, TheConfiguration.CurrentDate, TheConfiguration.DailyFrecuencyConfiguration);
            TheCalculation.Description = Scheduler.CalculateDescription(TheConfiguration.Type, TheCalculation.NextExecutionTime, TheConfiguration.DateTime, TheConfiguration.LimitStartDate, TheConfiguration.LimitEndDate);
            return TheCalculation;
        }

        private static DateTime CalculateExecutionTime(Occurrence TheOccurrence, int TheOccurenceAmount, DateTime TheCurrentDate, DailyFrecuency DailyFrecuencyConfiguration)
        {
            DateTime TheNewDate = TheCurrentDate;
            TimeSpan TheOriginalTime = new TimeSpan(TheNewDate.Hour, TheNewDate.Minute, TheNewDate.Second);
            if (DailyFrecuencyConfiguration != null)
            {
                TimeSpan LaHora = new TimeSpan(TheNewDate.Hour, TheNewDate.Minute, TheNewDate.Second);
                if (LaHora < DailyFrecuencyConfiguration.TimeStart)
                {
                    LaHora = DailyFrecuencyConfiguration.TimeStart;
                    return new DateTime(TheCurrentDate.Year, TheCurrentDate.Month, TheCurrentDate.Day, LaHora.Hours, LaHora.Minutes, LaHora.Seconds);
                }
                switch (DailyFrecuencyConfiguration.Type)
                {
                    case Enumerations.Type.Recurring:
                        switch (DailyFrecuencyConfiguration.DailyOccurrence)
                        {
                            case DailyOccurrence.Seconds:
                                TheNewDate = TheCurrentDate.AddSeconds(DailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                            case DailyOccurrence.Minutes:
                                TheNewDate = TheCurrentDate.AddSeconds(DailyFrecuencyConfiguration.OccurrenceAmount * 60);
                                break;
                            case DailyOccurrence.Hours:
                            default:
                                TheNewDate = TheCurrentDate.AddHours(DailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                        }
                        LaHora = new TimeSpan(TheNewDate.Hour, TheNewDate.Minute, TheNewDate.Second);
                        if (LaHora <= DailyFrecuencyConfiguration.TimeEnd)
                        {
                            return TheNewDate;
                        }
                        else
                        {
                            TheNewDate = new DateTime(TheNewDate.Year, TheNewDate.Month, TheNewDate.Day, DailyFrecuencyConfiguration.TimeStart.Hours, DailyFrecuencyConfiguration.TimeStart.Minutes, DailyFrecuencyConfiguration.TimeStart.Seconds);
                        }
                        break;
                    case Enumerations.Type.Once:
                    default:
                        TheNewDate = new DateTime(TheCurrentDate.Year, TheCurrentDate.Month, TheCurrentDate.Day, DailyFrecuencyConfiguration.TimeFrecuency.Hours, DailyFrecuencyConfiguration.TimeFrecuency.Minutes, DailyFrecuencyConfiguration.TimeFrecuency.Seconds);
                        break;
                }
            }
            switch (TheOccurrence)
            {
                case Occurrence.Monthly:
                    TheNewDate = TheNewDate.AddMonths(TheOccurenceAmount);
                    break;
                case Occurrence.Weekly:
                    TheNewDate = TheNewDate.AddDays(TheOccurenceAmount * 7);
                    break;
                case Occurrence.Daily:
                default:
                    TheNewDate = TheNewDate.AddDays(TheOccurenceAmount);
                    break;
            }
            return TheNewDate;
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

            // Validar que si tiene Daily Frecuencia con Occurs Every X hours, tiene que tener fecha de inicio y fecha fin.
        }

    }
}
