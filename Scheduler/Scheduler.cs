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
            TheCalculation.NextExecutionTime = Scheduler.CalculateExecutionTime(TheConfiguration.Occurrence, TheConfiguration.OccurrenceAmount, TheConfiguration.CurrentDate, TheConfiguration.DailyFrecuencyConfiguration, TheConfiguration.WeeklyConfiguration);
            TheCalculation.Description = Scheduler.CalculateDescription(
                TheConfiguration.CurrentDate, 
                TheConfiguration.Type, 
                TheCalculation.NextExecutionTime, 
                TheConfiguration.DateTime, 
                TheConfiguration.LimitStartDate, 
                TheConfiguration.LimitEndDate, 
                TheConfiguration.WeeklyConfiguration, 
                TheConfiguration.DailyFrecuencyConfiguration);
            return TheCalculation;
        }
        
        private static Weekday GetNextWeekDay(Weekday[] TheDays, DateTime TheDate)
        {
            if (TheDays == null || TheDays.Length == 0) { throw new ArgumentException("You should add Weekdays to get the next weekday."); }
            if (TheDate == null) { throw new ArgumentNullException(); }
            while (true)
            {
                foreach(Weekday TheWeekDay in TheDays)
                {
                    if (TheWeekDay == WeeklyConfiguration.GetWeekDay(TheDate.DayOfWeek))
                    {
                        return TheWeekDay;
                    }
                }
                TheDate = TheDate.AddDays(1);
            }
        }

        private static DateTime CalculateExecutionTime(Occurrence TheOccurrence, int TheOccurenceAmount, DateTime TheCurrentDate, DailyFrecuency DailyFrecuencyConfiguration, WeeklyConfiguration WeeklyConfiguration)
        {
            DateTime TheNewDate = TheCurrentDate;

            if (WeeklyConfiguration != null && WeeklyConfiguration.WeekDays != null)
            {
                while ((int)WeeklyConfiguration.GetWeekDay(TheNewDate.DayOfWeek) < (int)GetNextWeekDay(WeeklyConfiguration.WeekDays, TheNewDate))
                {
                    TheNewDate = TheNewDate.AddDays(1);
                }
            }

            if (DailyFrecuencyConfiguration != null)
            {
                TimeSpan LaHora = new TimeSpan(TheNewDate.Hour, TheNewDate.Minute, TheNewDate.Second);
                if (LaHora < DailyFrecuencyConfiguration.TimeStart)
                {
                    LaHora = DailyFrecuencyConfiguration.TimeStart;
                    return new DateTime(TheNewDate.Year, TheNewDate.Month, TheNewDate.Day, LaHora.Hours, LaHora.Minutes, LaHora.Seconds);
                }
                switch (DailyFrecuencyConfiguration.Type)
                {
                    case Enumerations.Type.Recurring:
                        switch (DailyFrecuencyConfiguration.DailyOccurrence)
                        {
                            case DailyOccurrence.Seconds:
                                TheNewDate = TheNewDate.AddSeconds(DailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                            case DailyOccurrence.Minutes:
                                TheNewDate = TheNewDate.AddSeconds(DailyFrecuencyConfiguration.OccurrenceAmount * 60);
                                break;
                            case DailyOccurrence.Hours:
                            default:
                                TheNewDate = TheNewDate.AddHours(DailyFrecuencyConfiguration.OccurrenceAmount);
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
                        TheNewDate = new DateTime(TheNewDate.Year, TheNewDate.Month, TheNewDate.Day, DailyFrecuencyConfiguration.TimeFrecuency.Hours, DailyFrecuencyConfiguration.TimeFrecuency.Minutes, DailyFrecuencyConfiguration.TimeFrecuency.Seconds);
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

            if (WeeklyConfiguration != null)
            {
                if (WeeklyConfiguration.WeekDays != null)
                {
                    while ((int)WeeklyConfiguration.GetWeekDay(TheNewDate.DayOfWeek) < (int)GetNextWeekDay(WeeklyConfiguration.WeekDays, TheNewDate))
                    {
                        TheNewDate = TheNewDate.AddDays(1);
                    }
                }

                if (WeeklyConfiguration.WeekAmount != 0 && WeeklyConfiguration.GetWeekDay(TheNewDate.DayOfWeek) > WeeklyConfiguration.WeekDays.Last())
                {
                    TheNewDate = TheNewDate.AddDays(WeeklyConfiguration.WeekAmount * 7);
                    while (WeeklyConfiguration.GetWeekDay(TheNewDate.DayOfWeek) != WeeklyConfiguration.WeekDays.First())
                    {
                        TheNewDate = TheNewDate.AddDays(-1);
                    }
                }
            }
            return TheNewDate;
        }

        public static string CalculateDescription(DateTime TheCurrentDate, Enumerations.Type TheType, DateTime TheNextExecutionTime, DateTime? TheDateTime, DateTime? TheLimitStartDate, DateTime? TheLimitEndDate, WeeklyConfiguration TheWeeklyConfiguration, DailyFrecuency TheDailyFrecuency)
        {
            //  Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00 am and 8:00 am starting on 01/01/2020
            string TheDescription = string.Empty;
            if (TheWeeklyConfiguration != null)
            {
                TheDescription += $"Occurs every {TheWeeklyConfiguration.WeekAmount} weeks ";
                bool FirstPrinted = true;
                foreach (Enumerations.Weekday WeekDay in TheWeeklyConfiguration.WeekDays)
                {
                    string TheSeparator = (WeekDay == TheWeeklyConfiguration.WeekDays.Last() ? " and " : ", ");
                    TheDescription += (FirstPrinted == false ? TheSeparator : "on ") + WeekDay.ToString().ToLower();
                    if (FirstPrinted)
                    {
                        FirstPrinted = false;
                    }
                    
                }
                if (TheWeeklyConfiguration.WeekDays.Length > 0) { TheDescription += " "; }
            }
            else
            {
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
            }
            if (TheDailyFrecuency != null)
            {
                if (TheDailyFrecuency.OccurrenceAmount != 0)
                {
                    TheDescription += $"every {TheDailyFrecuency.OccurrenceAmount} {TheDailyFrecuency.DailyOccurrence.ToString().ToLower()} ";
                }
                switch (TheDailyFrecuency.Type)
                {
                    case Enumerations.Type.Recurring:
                        DateTime TimeStartDT = DateTime.Today.Add(TheDailyFrecuency.TimeStart);
                        string TimeStartStr = Scheduler.DeleteFirstZero(TimeStartDT.ToString("hh:mm tt").ToLower());
                        DateTime TimeEndDT = DateTime.Today.Add(TheDailyFrecuency.TimeEnd);
                        string TimeEndStr = Scheduler.DeleteFirstZero(TimeEndDT.ToString("hh:mm tt").ToLower());

                        TheDescription += $"between {TimeStartStr} and {TimeEndStr} ";
                        break;
                    case Enumerations.Type.Once:
                    default:
                        DateTime TimeFrecuencyDT = DateTime.Today.Add(TheDailyFrecuency.TimeFrecuency);
                        string TimeFrecuencyStr = Scheduler.DeleteFirstZero(TimeFrecuencyDT.ToString("hh:mm tt").ToLower());
                        TheDescription += $"on {TimeFrecuencyStr} ";
                        break;
                }
                TheDescription += $"starting on {TheCurrentDate.ToShortDateString()}";
            }
            return TheDescription.Trim();
        }

        private static string DeleteFirstZero(string TheString)
        {
            if (TheString.StartsWith("0"))
            {
                TheString = TheString.Substring(1, TheString.Length - 1);
            }
            return TheString;
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
