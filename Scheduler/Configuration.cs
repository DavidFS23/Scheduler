using System;

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

        public int NumberOfDates
        {
            get
            {
                switch (this.Type)
                {
                    case Enumerations.Type.Recurring:
                        if (this.LimitEndDate.HasValue == false) { return int.MaxValue; }
                        switch (this.Occurrence)
                        {
                            case Enumerations.Occurrence.Monthly:
                                return ((this.LimitEndDate.Value.Year - this.CurrentDate.Year) * 12) + this.LimitEndDate.Value.Month - this.CurrentDate.Month;
                            case Enumerations.Occurrence.Weekly:
                                return (int)(this.LimitEndDate - this.CurrentDate).Value.TotalDays / 7;
                            case Enumerations.Occurrence.Daily:
                            default:
                                return (int)(this.LimitEndDate - this.CurrentDate).Value.TotalDays;
                        }
                    case Enumerations.Type.Once:
                    default:
                        return 1;
                }
            }
        }

        public DailyFrecuency DailyFrecuencyConfiguration { get; set; }



    }

    public class DailyFrecuency
    {
        public Enumerations.Type Type { get; set; }
        public TimeSpan TimeFrecuency { get; set; }
        public int OccurrenceAmount { get; set; }
        public Enumerations.DailyOccurrence DailyOccurrence { get; set; }

        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }

    }

    public class WeeklyConfiguration
    {
        public int WeekAmount { get; set; }
        public Enumerations.Weekday[] WeekDays { get; set; }
    }

}
