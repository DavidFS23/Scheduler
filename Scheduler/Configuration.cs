using System;
using System.Collections.ObjectModel;
using System.Linq;
using static Scheduler.Enumerations;

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

        public MonthlyConfiguration MonthlyConfiguration { get; set; }

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

    public class MonthlyConfiguration
    {
        public bool ConcreteDay { get; set; }
        public int DayNumber { get; set; }
        public int ConcreteDayMonthFrecuency { get; set; }

        public bool SomeDay { get; set; }
        public Frecuency? Frecuency { get; set; }
        public MonthlyConfigurationWeekDay? MonthlyConfigurationWeekDay { get; set; }
        public int SomeDayMonthFrecuency { get; set; }

    }

}
