using System;
using System.Linq;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public static class Scheduler
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

            newDate = Scheduler.CalculateFirstDateMonthlyConfiguration(newDate, configuration.MonthlyConfiguration);
            var dailyFrecuencyCalculation = Scheduler.CalculateDailyFrecuency(newDate, configuration);
            if (dailyFrecuencyCalculation.IsDefinitive)
            {
                return dailyFrecuencyCalculation.date;
            }
            newDate = dailyFrecuencyCalculation.date;
            newDate = Scheduler.CalculateOcurrence(newDate, configuration.Occurrence, configuration.OccurrenceAmount, configuration.MonthlyConfiguration);
            newDate = Scheduler.CalculateLastDateMonthlyConfiguration(newDate, configuration.MonthlyConfiguration);
            return newDate;
        }

        private static DateTime CalculateFirstDateMonthlyConfiguration(DateTime newDate, MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration != null)
            {
                Scheduler.ValidateConfigurationMonthlyConfiguration(monthlyConfiguration);
                if (monthlyConfiguration.ConcreteDay)
                {
                    while (newDate.Day != monthlyConfiguration.DayNumber)
                    {
                        newDate = newDate.AddDays(1);
                    }
                }
                if (monthlyConfiguration.SomeDay)
                {
                    while (IsSelectedDay(monthlyConfiguration.Frecuency.Value, monthlyConfiguration.MonthlyConfigurationWeekDay.Value, newDate) == false)
                    {
                        newDate = newDate.AddDays(1);
                    }
                }
                //while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(monthlyConfiguration.WeekDays, newDate))
                //{
                //    newDate = newDate.AddDays(1);
                //}
            }
            return newDate;
        }

        private static DateTime GetFirstWeekdayOfMonth(int year, int month)
        {
            DateTime firstWeekDayOfMonth = new DateTime(year, month, 1);
            while (firstWeekDayOfMonth.DayOfWeek != DayOfWeek.Monday &&
                firstWeekDayOfMonth.DayOfWeek != DayOfWeek.Tuesday &&
                firstWeekDayOfMonth.DayOfWeek != DayOfWeek.Wednesday &&
                firstWeekDayOfMonth.DayOfWeek != DayOfWeek.Thursday &&
                firstWeekDayOfMonth.DayOfWeek != DayOfWeek.Friday)
            {
                firstWeekDayOfMonth = firstWeekDayOfMonth.AddDays(1);
            }
            return firstWeekDayOfMonth;
        }

        private static DateTime GetFirstWeekenddayOfMonth(int year, int month)
        {
            DateTime firstWeekendDayOfMonth = new DateTime(year, month, 1);
            while (firstWeekendDayOfMonth.DayOfWeek != DayOfWeek.Saturday &&
                firstWeekendDayOfMonth.DayOfWeek != DayOfWeek.Sunday)
            {
                firstWeekendDayOfMonth = firstWeekendDayOfMonth.AddDays(1);
            }
            return firstWeekendDayOfMonth;
        }

        private static bool IsSelectedDay(Enumerations.Frecuency frecuency, Enumerations.MonthlyConfigurationWeekDay weekday, DateTime day)
        {
            DateTime firstWeekDayOfMonth = GetFirstWeekdayOfMonth(day.Year, day.Month);
            DateTime firstWeekendDayOfMonth = GetFirstWeekenddayOfMonth(day.Year, day.Month);
            DayOfWeek dayOfWeekRequired;
            switch (weekday)
            {
                case MonthlyConfigurationWeekDay.Weekday:
                    if (day.DayOfWeek != DayOfWeek.Monday &&
                        day.DayOfWeek != DayOfWeek.Tuesday &&
                        day.DayOfWeek != DayOfWeek.Wednesday &&
                        day.DayOfWeek != DayOfWeek.Thursday &&
                        day.DayOfWeek != DayOfWeek.Friday)
                    {
                        return false;
                    }
                    dayOfWeekRequired = firstWeekDayOfMonth.DayOfWeek;
                    break;
                case MonthlyConfigurationWeekDay.Weekend:
                    if (day.DayOfWeek != DayOfWeek.Saturday &&
                        day.DayOfWeek != DayOfWeek.Sunday)
                    {
                        return false;
                    }
                    dayOfWeekRequired = firstWeekendDayOfMonth.DayOfWeek;
                    break;
                case MonthlyConfigurationWeekDay.Monday:
                    if (day.DayOfWeek != DayOfWeek.Monday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Monday;
                    break;
                case MonthlyConfigurationWeekDay.Tuesday:
                    if (day.DayOfWeek != DayOfWeek.Tuesday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Tuesday;
                    break;
                case MonthlyConfigurationWeekDay.Wednesday:
                    if (day.DayOfWeek != DayOfWeek.Wednesday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Wednesday;
                    break;
                case MonthlyConfigurationWeekDay.Thursday:
                    if (day.DayOfWeek != DayOfWeek.Thursday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Thursday;
                    break;
                case MonthlyConfigurationWeekDay.Friday:
                    if (day.DayOfWeek != DayOfWeek.Friday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Friday;
                    break;
                case MonthlyConfigurationWeekDay.Saturday:
                    if (day.DayOfWeek != DayOfWeek.Saturday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Saturday;
                    break;
                case MonthlyConfigurationWeekDay.Sunday:
                default:
                    if (day.DayOfWeek != DayOfWeek.Sunday) { return false; }
                    dayOfWeekRequired = DayOfWeek.Sunday;
                    break;
            }
            DateTime dateRequired;
            switch (frecuency)
            {
                case Frecuency.First:
                    dateRequired = GetDateRequired(day, 1, dayOfWeekRequired);
                    break;
                case Frecuency.Second:
                    dateRequired = GetDateRequired(day, 2, dayOfWeekRequired);
                    break;
                case Frecuency.Third:
                    dateRequired = GetDateRequired(day, 3, dayOfWeekRequired);
                    break;
                case Frecuency.Fourth:
                    dateRequired = GetDateRequired(day, 4, dayOfWeekRequired);
                    break;
                case Frecuency.Last:
                default:
                    dateRequired = new DateTime(day.Year, day.Month, DateTime.DaysInMonth(day.Year, day.Month));
                    while(dateRequired.DayOfWeek != dayOfWeekRequired)
                    {
                        dateRequired.AddDays(-1);
                    }
                    break;
            }
            dateRequired = dateRequired.AddHours(day.Hour);
            dateRequired = dateRequired.AddMinutes(day.Minute);
            dateRequired = dateRequired.AddSeconds(day.Second);
            if (dateRequired != day) { return false; }
            return true;
        }

        private static DateTime GetDateRequired(this DateTime currentDate, int occurrence, DayOfWeek dayOfWeek)
        {
            var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
            var firstOccurrence = firstDay.DayOfWeek == dayOfWeek ? firstDay : firstDay.AddDays(dayOfWeek - firstDay.DayOfWeek);
            if (firstOccurrence.Month < currentDate.Month) { occurrence = occurrence + 1; }
            return firstOccurrence.AddDays(7 * (occurrence - 1));
        }

        private static DateTime CalculateLastDateMonthlyConfiguration(DateTime newDate, MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration != null)
            {
                Scheduler.ValidateConfigurationMonthlyConfiguration(monthlyConfiguration);
                if (monthlyConfiguration.ConcreteDay)
                {
                    newDate = newDate.AddMonths(monthlyConfiguration.ConcreteDayMonthFrecuency);
                }
                if (monthlyConfiguration.SomeDay)
                {
                    newDate = newDate.AddMonths(monthlyConfiguration.SomeDayMonthFrecuency);
                }
                //if (monthlyConfiguration.WeekDays != null)
                //{
                //    while ((int)Scheduler.GetWeekDay(newDate.DayOfWeek) < (int)GetNextWeekDay(monthlyConfiguration.WeekDays, newDate))
                //    {
                //        newDate = newDate.AddDays(1);
                //    }
                //}

                //if (monthlyConfiguration.WeekAmount != 0 && Scheduler.GetWeekDay(newDate.DayOfWeek) > monthlyConfiguration.WeekDays.Last())
                //{
                //    newDate = newDate.AddDays(monthlyConfiguration.WeekAmount * 7);
                //    while (Scheduler.GetWeekDay(newDate.DayOfWeek) != monthlyConfiguration.WeekDays.First())
                //    {
                //        newDate = newDate.AddDays(-1);
                //    }
                //}
            }
            return newDate;
        }

        private static DateTime CalculateOcurrence(DateTime newDate, Occurrence occurrence, int occurrenceAmount, MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration != null) { return newDate; }
            switch (occurrence)
            {
                case Occurrence.Monthly:
                    newDate = newDate.AddMonths(occurrenceAmount);
                    break;
                case Occurrence.Weekly:
                    newDate = newDate.AddDays(occurrenceAmount * 7);
                    break;
                case Occurrence.Daily:
                default:
                    newDate = newDate.AddDays(occurrenceAmount);
                    break;
            }
            return newDate;
        }

        private static DateCalculation CalculateDailyFrecuency(DateTime newDate, Configuration configuration)
        {
            Scheduler.ValidateConfigurationDailyFrecuency(configuration);
            if (configuration.DailyFrecuencyConfiguration != null)
            {
                TimeSpan time = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
                if (configuration.DailyFrecuencyConfiguration.TimeStart.HasValue && time < configuration.DailyFrecuencyConfiguration.TimeStart)
                {
                    time = configuration.DailyFrecuencyConfiguration.TimeStart.Value;
                    return new DateCalculation()
                    {
                        date = new DateTime(newDate.Year, newDate.Month, newDate.Day, time.Hours, time.Minutes, time.Seconds),
                        IsDefinitive = true
                    };
                }
                DateCalculation calculation = Scheduler.CalculateDailyFrecuencyByType(newDate, configuration);
                if (calculation.IsDefinitive)
                {
                    return calculation;
                }
                newDate = calculation.date;
            }
            return new DateCalculation()
            {
                date = newDate,
                IsDefinitive = false
            };
        }

        private static DateCalculation CalculateDailyFrecuencyByType(DateTime newDate, Configuration configuration)
        {
            switch (configuration.DailyFrecuencyConfiguration.Type)
            {
                case Enumerations.Type.Recurring:
                    DateCalculation calculationRecurring = Scheduler.CalculateDailyOccurrenceRecurring(newDate, configuration.DailyFrecuencyConfiguration);
                    if (calculationRecurring.IsDefinitive)
                    {
                        return calculationRecurring;
                    }
                    newDate = calculationRecurring.date;
                    break;
                case Enumerations.Type.Once:
                default:
                    newDate = Scheduler.CalculateDailyOccurrenceOnce(newDate, configuration.DailyFrecuencyConfiguration.TimeFrecuency);
                    break;
            }
            return new DateCalculation()
            {
                date = newDate,
                IsDefinitive = false
            };
        }

        private static DateTime CalculateDailyOccurrenceOnce(DateTime newDate, TimeSpan? timeFrecuency)
        {
            if (timeFrecuency.HasValue)
            {
                newDate = new DateTime(
                    newDate.Year,
                    newDate.Month,
                    newDate.Day,
                    timeFrecuency.Value.Hours,
                    timeFrecuency.Value.Minutes,
                    timeFrecuency.Value.Seconds);
            }
            return newDate;
        }

        private static DateCalculation CalculateDailyOccurrenceRecurring(DateTime newDate, DailyFrecuency dailyFrecuency)
        {
            switch (dailyFrecuency.DailyOccurrence)
            {
                case DailyOccurrence.Seconds:
                    newDate = newDate.AddSeconds(dailyFrecuency.OccurrenceAmount);
                    break;
                case DailyOccurrence.Minutes:
                    newDate = newDate.AddSeconds(dailyFrecuency.OccurrenceAmount * 60);
                    break;
                case DailyOccurrence.Hours:
                default:
                    newDate = newDate.AddHours(dailyFrecuency.OccurrenceAmount);
                    break;
            }
            TimeSpan timeOfDate = new TimeSpan(newDate.Hour, newDate.Minute, newDate.Second);
            if (dailyFrecuency.TimeEnd.HasValue && timeOfDate <= dailyFrecuency.TimeEnd)
            {
                return new DateCalculation()
                {
                    date = newDate,
                    IsDefinitive = true
                };
            }
            else
            {
                if (dailyFrecuency.TimeStart.HasValue)
                {
                    newDate = new DateTime(
                        newDate.Year,
                        newDate.Month,
                        newDate.Day,
                        dailyFrecuency.TimeStart.Value.Hours,
                        dailyFrecuency.TimeStart.Value.Minutes,
                        dailyFrecuency.TimeStart.Value.Seconds);
                }
            }
            return new DateCalculation()
            {
                date = newDate,
                IsDefinitive = false
            };
        }

        public static string CalculateDescription(Configuration configuration, DateTime nextExecutionTime)
        {
            string description = string.Empty;
            if (configuration.MonthlyConfiguration != null)
            {
                description += GetDescriptionMonthlyConfiguration(configuration);
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
            description += $"Schedule will be used on {UsedDate.ToShortDateString()} at {UsedDate.ToShortTimeString()} ";
            description += Scheduler.GetDescriptionLimitDates(configuration);
            return description;
        }

        private static string GetDescriptionLimitDates(Configuration configuration)
        {
            string description = string.Empty;
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

        private static string GetDescriptionMonthlyConfiguration(Configuration configuration)
        {
            if (configuration.MonthlyConfiguration == null) { return string.Empty; }
            string description = string.Empty;
            //description = $"Occurs every {configuration.MonthlyConfiguration.WeekAmount} weeks ";
            //bool FirstPrinted = true;
            //foreach (Enumerations.Weekday WeekDay in configuration.MonthlyConfiguration.WeekDays)
            //{
            //    string separator = (WeekDay == configuration.MonthlyConfiguration.WeekDays.Last() ? " and " : ", ");
            //    description += (FirstPrinted == false ? separator : "on ") + WeekDay.ToString().ToLower();
            //    if (FirstPrinted)
            //    {
            //        FirstPrinted = false;
            //    }

            //}
            //if (configuration.MonthlyConfiguration.WeekDays.Length > 0) { description += " "; }
            return description;
        }

        private static void ValidateConfiguration(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new Exception("The parameter Configuration should not be null.");
            }
        }

        private static void ValidateConfigurationDailyFrecuency(Configuration configuration)
        {
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

        private static void ValidateConfigurationMonthlyConfiguration(MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration == null)
            {
                throw new Exception("The parameter MonthlyConfiguration should not be null.");
            }
            //if (monthlyConfiguration.WeekAmount != 0 && (monthlyConfiguration.WeekDays == null || monthlyConfiguration.WeekDays.Length == 0))
            //{
            //    throw new Exception("If you set weeks on Weekly Configuration, you should set almost one week day.");
            //}
        }

        //private static Weekday GetNextWeekDay(Weekday[] days, DateTime date)
        //{
        //    if (days == null || days.Length == 0) { throw new Exception("You should add Weekdays to get the next weekday."); }
        //    if (date == null) { throw new Exception("You should add Date to get the next weekday."); }
        //    while (true)
        //    {
        //        foreach (Weekday weekDay in days)
        //        {
        //            if (weekDay == Scheduler.GetWeekDay(date.DayOfWeek))
        //            {
        //                return weekDay;
        //            }
        //        }
        //        date = date.AddDays(1);
        //    }
        //}

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

        private class DateCalculation
        {
            public DateTime date { get; set; }
            public bool IsDefinitive { get; set; }
        }

    }
}
