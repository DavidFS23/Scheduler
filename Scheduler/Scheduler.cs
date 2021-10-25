using System;
using System.Linq;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public class Scheduler
    {
        public static CalculationResult GenerateDate(Configuration configuration)
        {
            Scheduler.ValidateConfiguration(configuration);
            return Scheduler.CalculateNextDate(configuration);
        }

        private static CalculationResult CalculateNextDate(Configuration configuration)
        {
            
            CalculationResult calculation = new CalculationResult();
            calculation.NextExecutionTime = Scheduler.CalculateExecutionTime(configuration);
            calculation.Description = Scheduler.CalculateDescription(configuration, calculation.NextExecutionTime);
            return calculation;
        }
        
        private static DateTime CalculateExecutionTime(Configuration configuration)
        {
            DateTime newDate = configuration.CurrentDate;

            if (configuration.WeeklyConfiguration != null && configuration.WeeklyConfiguration.WeekDays != null)
            {
                while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(configuration.WeeklyConfiguration.WeekDays, newDate))
                {
                    newDate = newDate.AddDays(1);
                }
            }

            if (configuration.DailyFrecuencyConfiguration != null)
            {
                TimeSpan LaHora = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
                if (configuration.DailyFrecuencyConfiguration.TimeStart.HasValue && LaHora < configuration.DailyFrecuencyConfiguration.TimeStart)
                {
                    LaHora = configuration.DailyFrecuencyConfiguration.TimeStart.Value;
                    return new DateTime(newDate.Year, newDate.Month, newDate.Day, LaHora.Hours, LaHora.Minutes, LaHora.Seconds);
                }
                switch (configuration.DailyFrecuencyConfiguration.Type)
                {
                    case Enumerations.Type.Recurring:
                        switch (configuration.DailyFrecuencyConfiguration.DailyOccurrence)
                        {
                            case DailyOccurrence.Seconds:
                                newDate = newDate.AddSeconds(configuration.DailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                            case DailyOccurrence.Minutes:
                                newDate = newDate.AddSeconds(configuration.DailyFrecuencyConfiguration.OccurrenceAmount * 60);
                                break;
                            case DailyOccurrence.Hours:
                            default:
                                newDate = newDate.AddHours(configuration.DailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                        }
                        LaHora = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
                        if (configuration.DailyFrecuencyConfiguration.TimeEnd.HasValue && LaHora <= configuration.DailyFrecuencyConfiguration.TimeEnd)
                        {
                            return newDate;
                        }
                        else
                        {
                            if (configuration.DailyFrecuencyConfiguration.TimeStart.HasValue)
                            {
                                newDate = new DateTime(
                                    newDate.Year, 
                                    newDate.Month, 
                                    newDate.Day, 
                                    configuration.DailyFrecuencyConfiguration.TimeStart.Value.Hours, 
                                    configuration.DailyFrecuencyConfiguration.TimeStart.Value.Minutes, 
                                    configuration.DailyFrecuencyConfiguration.TimeStart.Value.Seconds);
                            }
                        }
                        break;
                    case Enumerations.Type.Once:
                    default:
                        if (configuration.DailyFrecuencyConfiguration.TimeFrecuency.HasValue)
                        {
                            newDate = new DateTime(
                                newDate.Year, 
                                newDate.Month, 
                                newDate.Day,
                                configuration.DailyFrecuencyConfiguration.TimeFrecuency.Value.Hours,
                                configuration.DailyFrecuencyConfiguration.TimeFrecuency.Value.Minutes,
                                configuration.DailyFrecuencyConfiguration.TimeFrecuency.Value.Seconds);
                        }
                        break;
                }
            }

            switch (configuration.Occurrence)
            {
                case Occurrence.Monthly:
                    newDate = newDate.AddMonths(configuration.OccurrenceAmount);
                    break;
                case Occurrence.Weekly:
                    newDate = newDate.AddDays(configuration.OccurrenceAmount * 7);
                    break;
                case Occurrence.Daily:
                default:
                    newDate = newDate.AddDays(configuration.OccurrenceAmount);
                    break;
            }

            if (configuration.WeeklyConfiguration != null)
            {
                if (configuration.WeeklyConfiguration.WeekDays != null)
                {
                    while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(configuration.WeeklyConfiguration.WeekDays, newDate))
                    {
                        newDate = newDate.AddDays(1);
                    }
                }

                if (configuration.WeeklyConfiguration.WeekAmount != 0 && Scheduler.GetWeekDay(newDate.DayOfWeek) > configuration.WeeklyConfiguration.WeekDays.Last())
                {
                    newDate = newDate.AddDays(configuration.WeeklyConfiguration.WeekAmount * 7);
                    while (Scheduler.GetWeekDay(newDate.DayOfWeek) != configuration.WeeklyConfiguration.WeekDays.First())
                    {
                        newDate = newDate.AddDays(-1);
                    }
                }
            }
            return newDate;
        }

        public static string CalculateDescription(Configuration configuration, DateTime nextExecutionTime)
        {
            string description = string.Empty;
            if (configuration.WeeklyConfiguration != null)
            {
                description += GetDescriptionWeeklyConfiguration(configuration);
            }
            else
            {
                description += GetDescriptionBaseConfiguration(configuration, nextExecutionTime);
                
            }
            if (configuration.DailyFrecuencyConfiguration != null)
            {
                description += GetDescriptionDailyFrecuencyConfiguration(configuration);
            }
            return description.Trim();
        }

        private static string GetDescriptionBaseConfiguration(Configuration configuration, DateTime nextExecutionTime)
        {
            string description = string.Empty;
            switch (configuration.Type)
            {
                case Enumerations.Type.Once:
                    description += "Occurs once. ";
                    break;
                case Enumerations.Type.Recurring:
                    description += "Occurs every day. ";
                    break;
            }
            DateTime UsedDate = nextExecutionTime;
            if (configuration.DateTime.HasValue)
            {
                UsedDate = configuration.DateTime.Value;
            }

            description += $"Schedule will be used on {UsedDate.ToShortDateString()} at {UsedDate.ToString("HH:mm")} ";
            if (configuration.LimitStartDate.HasValue)
            {
                description += $"starting on {configuration.LimitStartDate.Value.ToShortDateString()} ";
            }
            if (configuration.LimitEndDate.HasValue)
            {
                description += $"ending on {configuration.LimitEndDate.Value.ToShortDateString()} ";
            }
            return description;
        }

        private static string GetDescriptionDailyFrecuencyConfiguration(Configuration configuration)
        {
            if (configuration.DailyFrecuencyConfiguration == null) { return string.Empty; }
            string description = string.Empty;
            if (configuration.DailyFrecuencyConfiguration.OccurrenceAmount != 0)
            {
                description += $"every {configuration.DailyFrecuencyConfiguration.OccurrenceAmount} {configuration.DailyFrecuencyConfiguration.DailyOccurrence.ToString().ToLower()} ";
            }
            switch (configuration.DailyFrecuencyConfiguration.Type)
            {
                case Enumerations.Type.Recurring:
                    description += GetDescriptionDailyFrecuencyRecurring(configuration);
                    break;
                case Enumerations.Type.Once:
                default:
                    description += GetDescriptionDailyFrecuencyOnce(configuration);
                    break;
            }
            description += $"starting on {configuration.CurrentDate.ToShortDateString()}";
            return description;
        }

        private static string GetDescriptionDailyFrecuencyOnce(Configuration configuration)
        {
            string description = string.Empty;
            if (configuration.DailyFrecuencyConfiguration.TimeFrecuency.HasValue)
            {
                DateTime TimeFrecuencyDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeFrecuency.Value);
                string TimeFrecuencyStr = Scheduler.DeleteFirstZero(TimeFrecuencyDT.ToString("hh:mm tt").ToLower());
                description += $"on {TimeFrecuencyStr} ";
            }
            return description;
        }

        private static string GetDescriptionDailyFrecuencyRecurring(Configuration configuration)
        {
            string description = string.Empty;
            if (configuration.DailyFrecuencyConfiguration.TimeStart.HasValue && configuration.DailyFrecuencyConfiguration.TimeEnd.HasValue)
            {
                DateTime TimeStartDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeStart.Value);
                string TimeStartStr = Scheduler.DeleteFirstZero(TimeStartDT.ToString("hh:mm tt").ToLower());
                DateTime TimeEndDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeEnd.Value);
                string TimeEndStr = Scheduler.DeleteFirstZero(TimeEndDT.ToString("hh:mm tt").ToLower());

                description += $"between {TimeStartStr} and {TimeEndStr} ";
            }
            return description;
        }

        private static string GetDescriptionWeeklyConfiguration(Configuration configuration)
        {
            if (configuration.WeeklyConfiguration == null) { return string.Empty; }
            string description = $"Occurs every {configuration.WeeklyConfiguration.WeekAmount} weeks ";
            bool FirstPrinted = true;
            foreach (Enumerations.Weekday WeekDay in configuration.WeeklyConfiguration.WeekDays)
            {
                string separator = (WeekDay == configuration.WeeklyConfiguration.WeekDays.Last() ? " and " : ", ");
                description += (FirstPrinted == false ? separator : "on ") + WeekDay.ToString().ToLower();
                if (FirstPrinted)
                {
                    FirstPrinted = false;
                }

            }
            if (configuration.WeeklyConfiguration.WeekDays.Length > 0) { description += " "; }
            return description;
        }

        private static void ValidateConfiguration(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new Exception("The parameter Configuration should not be null.");
            }
            if (configuration.Type == Enumerations.Type.Recurring && configuration.LimitEndDate == null)
            {
                throw new Exception("If the configuration is Recurring, you should add Limit End Date.");
            }
            if (configuration.DailyFrecuencyConfiguration != null && configuration.DailyFrecuencyConfiguration.OccurrenceAmount != 0 &&
                (configuration.DailyFrecuencyConfiguration.TimeStart == null || configuration.DailyFrecuencyConfiguration.TimeEnd == null))
            {
                throw new Exception("If the configuration is Daily Frecuency, you should add Start and End Time.");
            }
        }

        private static Weekday GetNextWeekDay(Weekday[] days, DateTime date)
        {
            if (days == null || days.Length == 0) { throw new Exception("You should add Weekdays to get the next weekday."); }
            if (date == null) { throw new Exception("You should add Date to get the next weekday."); }
            while (true)
            {
                foreach (Weekday weekDay in days)
                {
                    if (weekDay == Scheduler.GetWeekDay(date.DayOfWeek))
                    {
                        return weekDay;
                    }
                }
                date = date.AddDays(1);
            }
        }

        private static string DeleteFirstZero(string chars)
        {
            if (chars.StartsWith("0"))
            {
                chars = chars.Substring(1, chars.Length - 1);
            }
            return chars;
        }

        public static Enumerations.Weekday GetWeekDay(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return Enumerations.Weekday.Monday;
                case DayOfWeek.Tuesday:
                    return Enumerations.Weekday.Tuesday;
                case DayOfWeek.Wednesday:
                    return Enumerations.Weekday.Wednesday;
                case DayOfWeek.Thursday:
                    return Enumerations.Weekday.Thursday;
                case DayOfWeek.Friday:
                    return Enumerations.Weekday.Friday;
                case DayOfWeek.Saturday:
                    return Enumerations.Weekday.Saturday;
                case DayOfWeek.Sunday:
                default:
                    return Enumerations.Weekday.Sunday;
            }
        }

    }
}
