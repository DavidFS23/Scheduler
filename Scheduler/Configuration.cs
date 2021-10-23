using System;
using System.Linq;

namespace Scheduler
{
    public class Configuration
    {
        public DateTime CurrentDate { get; set; }
        public Enumerations.Type Type { get; set; }
        public DateTime? DateTime { get; set; }
        public Enumerations.Occurrence Occurrence { get; set; }
        public int OccurrenceAmount { get; set; }

        public DateTime? LimitStartDate { get; set; }
        public DateTime? LimitEndDate { get; set; }

        public DailyFrecuency DailyFrecuencyConfiguration { get; set; }

        public WeeklyConfiguration WeeklyConfiguration { get; set; }

    }

    public class DailyFrecuency
    {
        public Enumerations.Type Type { get; set; }
        public TimeSpan? TimeFrecuency { get; set; }
        public int OccurrenceAmount { get; set; }
        public Enumerations.DailyOccurrence DailyOccurrence { get; set; }

        public TimeSpan? TimeStart { get; set; }
        public TimeSpan? TimeEnd { get; set; }

    }

    public class WeeklyConfiguration
    {
        public int WeekAmount { get; set; }

        private Enumerations.Weekday[] weekDays;
        public Enumerations.Weekday[] WeekDays 
        { 
            get
            {
                return this.weekDays.OrderBy(D => ((int)D)).ToArray();
            }
            set
            {
                this.weekDays = value;
            }
        }

        public static Enumerations.Weekday GetWeekDay(DayOfWeek TheDayOfWeek)
        {
            switch (TheDayOfWeek)
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
