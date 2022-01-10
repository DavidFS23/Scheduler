using System;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public static class Scheduler
    {
        public static CalculationResult GenerateDate(Configuration configuration)
        {
            if (configuration.Language == null) { configuration.Language = new Language(); }
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

            var firstDateMonthlyConfiguration = Scheduler.CalculateFirstDateMonthlyConfiguration(newDate, configuration);
            if (firstDateMonthlyConfiguration.IsDefinitive)
            {
                return firstDateMonthlyConfiguration.date;
            }
            newDate = firstDateMonthlyConfiguration.date;
            if (firstDateMonthlyConfiguration.DailyFrecuencyCalculated == false)
            {
                var dailyFrecuencyCalculation = Scheduler.CalculateDailyFrecuency(newDate, configuration);
                if (dailyFrecuencyCalculation.IsDefinitive)
                {
                    return dailyFrecuencyCalculation.date;
                }
                newDate = dailyFrecuencyCalculation.date;
            }
            newDate = Scheduler.CalculateOcurrence(newDate, configuration.Occurrence, configuration.OccurrenceAmount, configuration.MonthlyConfiguration);
            newDate = Scheduler.CalculateLastDateMonthlyConfiguration(newDate, configuration.MonthlyConfiguration, false);
            return newDate;
        }

        private static DateCalculation CalculateFirstDateMonthlyConfiguration(DateTime newDate, Configuration configuration)
        {
            bool DailyFrecuencyCalculated = false;
            if (configuration.MonthlyConfiguration != null)
            {
                Scheduler.ValidateConfigurationMonthlyConfiguration(configuration.MonthlyConfiguration);
                if (configuration.MonthlyConfiguration.ConcreteDay)
                {
                    while (newDate.Day != configuration.MonthlyConfiguration.DayNumber)
                    {
                        newDate = newDate.AddDays(1);
                    }
                }
                if (configuration.MonthlyConfiguration.SomeDay)
                {
                    return Scheduler.CalculateMonthlyConfigurationSomeDay(newDate, configuration);
                }
            }
            return new DateCalculation() { date = newDate, IsDefinitive = false, DailyFrecuencyCalculated = DailyFrecuencyCalculated };
        }

        private static DateCalculation CalculateMonthlyConfigurationSomeDay(DateTime newDate, Configuration configuration)
        {
            var dailyFrecuencyCalculation = Scheduler.CalculateDailyFrecuency(newDate, configuration);
            bool dailyFrecuencyCalculated = true;
            if (IsSelectedDay(configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay.Value, dailyFrecuencyCalculation.date) && dailyFrecuencyCalculation.IsDefinitive)
            {
                dailyFrecuencyCalculation.DailyFrecuencyCalculated = true;
                return dailyFrecuencyCalculation;
            }
            newDate = dailyFrecuencyCalculation.date;
            if (configuration.MonthlyConfiguration.Frecuency == Frecuency.Last)
            {
                newDate = new DateTime(newDate.Year, newDate.Month, DateTime.DaysInMonth(newDate.Year, newDate.Month));
                while (IsSelectedDay(configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay, newDate) == false)
                {
                    newDate = newDate.AddDays(-1);
                }
                return new DateCalculation() { date = newDate, IsDefinitive = false, DailyFrecuencyCalculated = dailyFrecuencyCalculated };
            }
            int actualOccurence = 1;
            int numberOfOccurrences = Scheduler.GetNumberOfOccurrences(configuration.MonthlyConfiguration.Frecuency);
            bool isSelectedDay = IsSelectedDay(configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay.Value, newDate);
            if (isSelectedDay)
            {
                newDate = Scheduler.CalculateLastDateMonthlyConfiguration(newDate, configuration.MonthlyConfiguration, true);
                isSelectedDay = IsSelectedDay(configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay.Value, newDate);
            }
            while (isSelectedDay == false || (numberOfOccurrences > actualOccurence))
            {
                if (isSelectedDay)
                {
                    actualOccurence++;
                }
                newDate = newDate.AddDays(1);
                isSelectedDay = IsSelectedDay(configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay.Value, newDate);
            }
            return new DateCalculation() { date = newDate, IsDefinitive = false, DailyFrecuencyCalculated = dailyFrecuencyCalculated };
        }

        private static int GetNumberOfOccurrences(Frecuency? frecuency)
        {
            switch (frecuency)
            {
                case Frecuency.First:
                    return 1;
                case Frecuency.Second:
                    return 2;
                case Frecuency.Third:
                    return 3;
                case Frecuency.Fourth:
                    return 4;
            }
            return 1;
        }

        private static bool IsSelectedDay(Enumerations.MonthlyConfigurationWeekDay? weekday, DateTime day)
        {
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
                    break;
                case MonthlyConfigurationWeekDay.Weekend:
                    if (day.DayOfWeek != DayOfWeek.Saturday &&
                        day.DayOfWeek != DayOfWeek.Sunday)
                    {
                        return false;
                    }
                    break;
                case MonthlyConfigurationWeekDay.Monday:
                    if (day.DayOfWeek != DayOfWeek.Monday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Tuesday:
                    if (day.DayOfWeek != DayOfWeek.Tuesday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Wednesday:
                    if (day.DayOfWeek != DayOfWeek.Wednesday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Thursday:
                    if (day.DayOfWeek != DayOfWeek.Thursday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Friday:
                    if (day.DayOfWeek != DayOfWeek.Friday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Saturday:
                    if (day.DayOfWeek != DayOfWeek.Saturday) { return false; }
                    break;
                case MonthlyConfigurationWeekDay.Sunday:
                    if (day.DayOfWeek != DayOfWeek.Sunday) { return false; }
                    break;
            }
            return true;
        }


        private static DateTime CalculateLastDateMonthlyConfiguration(DateTime newDate, MonthlyConfiguration monthlyConfiguration, bool exec)
        {
            if (monthlyConfiguration != null)
            {
                Scheduler.ValidateConfigurationMonthlyConfiguration(monthlyConfiguration);
                if (monthlyConfiguration.ConcreteDay)
                {
                    newDate = newDate.AddMonths(monthlyConfiguration.ConcreteDayMonthFrecuency);
                }
                if (monthlyConfiguration.SomeDay && exec)
                {
                    newDate = newDate.AddMonths(monthlyConfiguration.SomeDayMonthFrecuency);
                    while (newDate.Day != 1)
                    {
                        newDate = newDate.AddDays(-1);
                    }
                }
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
                string dailyOccurrence = configuration.DailyFrecuencyConfiguration.DailyOccurrence.ToString().ToLower();
                if (configuration.DailyFrecuencyConfiguration.OccurrenceAmount == 1)
                {
                    dailyOccurrence = dailyOccurrence.Substring(0, dailyOccurrence.Length - 1);
                }
                description += $"every {configuration.DailyFrecuencyConfiguration.OccurrenceAmount} {dailyOccurrence} ";
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
                DateTime timeFrecuencyDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeFrecuency.Value);
                string timeFrecuencyStr = Scheduler.DeleteFirstZero(timeFrecuencyDT.ToString("hh:mm tt").ToLower());
                description += $"on {timeFrecuencyStr} ";
            }
            return description;
        }

        private static string GetDescriptionDailyFrecuencyRecurring(Configuration configuration)
        {
            string description = string.Empty;
            if (configuration.DailyFrecuencyConfiguration.TimeStart.HasValue && configuration.DailyFrecuencyConfiguration.TimeEnd.HasValue)
            {
                DateTime timeStartDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeStart.Value);
                string timeStartStr = Scheduler.DeleteFirstZero(timeStartDT.ToString("hh:mm tt").ToLower());
                DateTime timeEndDT = DateTime.Today.Add(configuration.DailyFrecuencyConfiguration.TimeEnd.Value);
                string timeEndStr = Scheduler.DeleteFirstZero(timeEndDT.ToString("hh:mm tt").ToLower());

                description += $"between {timeStartStr} and {timeEndStr} ";
            }
            return description;
        }

        private static string GetDescriptionMonthlyConfiguration(Configuration configuration)
        {
            if (configuration.MonthlyConfiguration == null) { return string.Empty; }
            string description = string.Empty;

            description += "Occurs ";
            if (configuration.MonthlyConfiguration.SomeDay)
            {
                string frecuency = configuration.MonthlyConfiguration.Frecuency.ToString().ToLower();
                string weekday = configuration.MonthlyConfiguration.MonthlyConfigurationWeekDay.ToString().ToLower();
                string monthFrecuency = configuration.MonthlyConfiguration.SomeDayMonthFrecuency.ToString();
                description += $"the {frecuency} {weekday} of every {monthFrecuency} months ";
            }
            
            return description;
        }

        private static void ValidateConfiguration(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new Exception(configuration.Resources.TheParameterConfigurationShouldNotBeNull);
            }
        }

        private static void ValidateConfigurationDailyFrecuency(Configuration configuration)
        {
            if (configuration.Type == Enumerations.Type.Recurring && configuration.LimitEndDate == null)
            {
                throw new Exception(configuration.Resources.ShouldLimitEndDate);
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
            if (monthlyConfiguration.SomeDay && monthlyConfiguration.ConcreteDay)
            {
                throw new Exception("You should not select Concrete Day and Some Day at the same time.");
            }
            if (monthlyConfiguration.ConcreteDay && monthlyConfiguration.DayNumber <= 0)
            {
                throw new Exception("You should insert a positive Day Number if you set Concrete Day.");
            }
            if (monthlyConfiguration.ConcreteDay && monthlyConfiguration.ConcreteDayMonthFrecuency == 0)
            {
                throw new Exception("You should insert Month Frecuency if you set Concrete Day.");
            }
            if (monthlyConfiguration.SomeDay && monthlyConfiguration.Frecuency == null)
            {
                throw new Exception("You should insert Frecuency if you set Some Day.");
            }
            if (monthlyConfiguration.SomeDay && monthlyConfiguration.MonthlyConfigurationWeekDay == null)
            {
                throw new Exception("You should insert Weekday if you set Some Day.");
            }
            if (monthlyConfiguration.SomeDay && monthlyConfiguration.SomeDayMonthFrecuency == 0)
            {
                throw new Exception("You should insert Month Frecuency if you set Some Day.");
            }
            if (monthlyConfiguration.SomeDay && monthlyConfiguration.SomeDayMonthFrecuency < 0)
            {
                throw new Exception("You should insert positive Month Frecuency.");
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

        private class DateCalculation
        {
            public DateTime date { get; set; }
            public bool IsDefinitive { get; set; }
            public bool DailyFrecuencyCalculated { get; set; }
        }

    }
}
