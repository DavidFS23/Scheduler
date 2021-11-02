using System;
using System.Collections.ObjectModel;
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
        

    }

}
