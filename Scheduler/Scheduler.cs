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
            if (configuration == null)
            {
                throw new Exception("The parameter Configuration should not be null.");
            }
            CalculationResult calculation = new CalculationResult();
            calculation.NextExecutionTime = Scheduler.CalculateExecutionTime(configuration.Occurrence, configuration.OccurrenceAmount, configuration.CurrentDate, configuration.DailyFrecuencyConfiguration, configuration.WeeklyConfiguration);
            calculation.Description = Scheduler.CalculateDescription(
                configuration.CurrentDate, 
                configuration.Type, 
                calculation.NextExecutionTime, 
                configuration.DateTime, 
                configuration.LimitStartDate, 
                configuration.LimitEndDate, 
                configuration.WeeklyConfiguration, 
                configuration.DailyFrecuencyConfiguration);
            return calculation;
        }
        
        private static DateTime CalculateExecutionTime(Occurrence occurrence, int occurenceAmount, DateTime currentDate, DailyFrecuency dailyFrecuencyConfiguration, WeeklyConfiguration weeklyConfiguration)
        {
            DateTime newDate = currentDate;

            if (weeklyConfiguration != null && weeklyConfiguration.WeekDays != null)
            {
                while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(weeklyConfiguration.WeekDays, newDate))
                {
                    newDate = newDate.AddDays(1);
                }
            }

            if (dailyFrecuencyConfiguration != null)
            {
                TimeSpan LaHora = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
                if (dailyFrecuencyConfiguration.TimeStart.HasValue && LaHora < dailyFrecuencyConfiguration.TimeStart)
                {
                    LaHora = dailyFrecuencyConfiguration.TimeStart.Value;
                    return new DateTime(newDate.Year, newDate.Month, newDate.Day, LaHora.Hours, LaHora.Minutes, LaHora.Seconds);
                }
                switch (dailyFrecuencyConfiguration.Type)
                {
                    case Enumerations.Type.Recurring:
                        switch (dailyFrecuencyConfiguration.DailyOccurrence)
                        {
                            case DailyOccurrence.Seconds:
                                newDate = newDate.AddSeconds(dailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                            case DailyOccurrence.Minutes:
                                newDate = newDate.AddSeconds(dailyFrecuencyConfiguration.OccurrenceAmount * 60);
                                break;
                            case DailyOccurrence.Hours:
                            default:
                                newDate = newDate.AddHours(dailyFrecuencyConfiguration.OccurrenceAmount);
                                break;
                        }
                        LaHora = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
                        if (dailyFrecuencyConfiguration.TimeEnd.HasValue && LaHora <= dailyFrecuencyConfiguration.TimeEnd)
                        {
                            return newDate;
                        }
                        else
                        {
                            if (dailyFrecuencyConfiguration.TimeStart.HasValue)
                            {
                                newDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, dailyFrecuencyConfiguration.TimeStart.Value.Hours, dailyFrecuencyConfiguration.TimeStart.Value.Minutes, dailyFrecuencyConfiguration.TimeStart.Value.Seconds);
                            }
                        }
                        break;
                    case Enumerations.Type.Once:
                    default:
                        if (dailyFrecuencyConfiguration.TimeFrecuency.HasValue)
                        {
                            newDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, dailyFrecuencyConfiguration.TimeFrecuency.Value.Hours, dailyFrecuencyConfiguration.TimeFrecuency.Value.Minutes, dailyFrecuencyConfiguration.TimeFrecuency.Value.Seconds);
                        }
                        break;
                }
            }

            switch (occurrence)
            {
                case Occurrence.Monthly:
                    newDate = newDate.AddMonths(occurenceAmount);
                    break;
                case Occurrence.Weekly:
                    newDate = newDate.AddDays(occurenceAmount * 7);
                    break;
                case Occurrence.Daily:
                default:
                    newDate = newDate.AddDays(occurenceAmount);
                    break;
            }

            if (weeklyConfiguration != null)
            {
                if (weeklyConfiguration.WeekDays != null)
                {
                    while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(weeklyConfiguration.WeekDays, newDate))
                    {
                        newDate = newDate.AddDays(1);
                    }
                }

                if (weeklyConfiguration.WeekAmount != 0 && Scheduler.GetWeekDay(newDate.DayOfWeek) > weeklyConfiguration.WeekDays.Last())
                {
                    newDate = newDate.AddDays(weeklyConfiguration.WeekAmount * 7);
                    while (Scheduler.GetWeekDay(newDate.DayOfWeek) != weeklyConfiguration.WeekDays.First())
                    {
                        newDate = newDate.AddDays(-1);
                    }
                }
            }
            return newDate;
        }

        public static string CalculateDescription(DateTime currentDate, Enumerations.Type type, DateTime nextExecutionTime, DateTime? dateTime, DateTime? limitStartDate, DateTime? limitEndDate, WeeklyConfiguration weeklyConfiguration, DailyFrecuency dailyFrecuency)
        {
            //  Occurs every 2 weeks on monday, thursday and friday every 2 hours between 4:00 am and 8:00 am starting on 01/01/2020
            string description = string.Empty;
            if (weeklyConfiguration != null)
            {
                description += $"Occurs every {weeklyConfiguration.WeekAmount} weeks ";
                bool FirstPrinted = true;
                foreach (Enumerations.Weekday WeekDay in weeklyConfiguration.WeekDays)
                {
                    string separator = (WeekDay == weeklyConfiguration.WeekDays.Last() ? " and " : ", ");
                    description += (FirstPrinted == false ? separator : "on ") + WeekDay.ToString().ToLower();
                    if (FirstPrinted)
                    {
                        FirstPrinted = false;
                    }
                    
                }
                if (weeklyConfiguration.WeekDays.Length > 0) { description += " "; }
            }
            else
            {
                switch (type)
                {
                    case Enumerations.Type.Once:
                        description += "Occurs once. ";
                        break;
                    case Enumerations.Type.Recurring:
                        description += "Occurs every day. ";
                        break;
                }
                DateTime UsedDate = nextExecutionTime;
                if (dateTime.HasValue)
                {
                    UsedDate = dateTime.Value;
                }

                description += $"Schedule will be used on {UsedDate.ToShortDateString()} at {UsedDate.ToString("HH:mm")} ";
                if (limitStartDate.HasValue)
                {
                    description += $"starting on {limitStartDate.Value.ToShortDateString()} ";
                }
                if (limitEndDate.HasValue)
                {
                    description += $"ending on {limitEndDate.Value.ToShortDateString()} ";
                }
            }
            if (dailyFrecuency != null)
            {
                if (dailyFrecuency.OccurrenceAmount != 0)
                {
                    description += $"every {dailyFrecuency.OccurrenceAmount} {dailyFrecuency.DailyOccurrence.ToString().ToLower()} ";
                }
                switch (dailyFrecuency.Type)
                {
                    case Enumerations.Type.Recurring:
                        if (dailyFrecuency.TimeStart.HasValue && dailyFrecuency.TimeEnd.HasValue)
                        {
                            DateTime TimeStartDT = DateTime.Today.Add(dailyFrecuency.TimeStart.Value);
                            string TimeStartStr = Scheduler.DeleteFirstZero(TimeStartDT.ToString("hh:mm tt").ToLower());
                            DateTime TimeEndDT = DateTime.Today.Add(dailyFrecuency.TimeEnd.Value);
                            string TimeEndStr = Scheduler.DeleteFirstZero(TimeEndDT.ToString("hh:mm tt").ToLower());

                            description += $"between {TimeStartStr} and {TimeEndStr} ";
                        }
                        break;
                    case Enumerations.Type.Once:
                    default:
                        if (dailyFrecuency.TimeFrecuency.HasValue)
                        {
                            DateTime TimeFrecuencyDT = DateTime.Today.Add(dailyFrecuency.TimeFrecuency.Value);
                            string TimeFrecuencyStr = Scheduler.DeleteFirstZero(TimeFrecuencyDT.ToString("hh:mm tt").ToLower());
                            description += $"on {TimeFrecuencyStr} ";
                        }
                        break;
                }
                description += $"starting on {currentDate.ToShortDateString()}";
            }
            return description.Trim();
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
