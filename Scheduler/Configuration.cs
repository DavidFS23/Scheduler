using System;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public class Configuration
    {
        public Configuration()
        {
            Language = new Language();
        }

        public Language Language { get; set; }
        public Resources Resources 
        { 
            get
            {
                return Language.Resources;               
            } 
        }

        public DateTime CurrentDate { get; set; }
        public Enumerations.Type Type { get; set; }
        public DateTime? DateTime { get; }
        public Enumerations.Occurrence Occurrence { get; set; }
        public int OccurrenceAmount { get; set; }

        public DateTime? LimitStartDate { get; set; }
        public DateTime? LimitEndDate { get; set; }

        private DailyFrecuency dailyFrecuencyConfiguration;
        public DailyFrecuency DailyFrecuencyConfiguration 
        {
            get
            {
                return dailyFrecuencyConfiguration;
            }
            set
            {
                dailyFrecuencyConfiguration = value;
                dailyFrecuencyConfiguration.BaseConfiguration = this;
            }
        }

        private MonthlyConfiguration monthlyConfiguration;
        public MonthlyConfiguration MonthlyConfiguration 
        {
            get
            {
                return monthlyConfiguration;
            }
            set
            {
                monthlyConfiguration = value;
                monthlyConfiguration.BaseConfiguration = this;
            }
        }

    }

    public class DailyFrecuency
    {
        public Enumerations.Type Type { get; set; }
        public TimeSpan? TimeFrecuency { get; set; }
        public int OccurrenceAmount { get; set; }
        public Enumerations.DailyOccurrence DailyOccurrence { get; set; }

        public TimeSpan? TimeStart { get; set; }
        public TimeSpan? TimeEnd { get; set; }

        public Configuration BaseConfiguration { get; set; }
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

        public Configuration BaseConfiguration { get; set; }
    }

}
