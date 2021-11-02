namespace Scheduler
{
    public class Enumerations
    {

        public enum Type
        {
            Once = 1,
            Recurring = 2
        }

        public enum Occurrence
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3
        }

        public enum DailyOccurrence
        {
            Hours = 1,
            Minutes = 2,
            Seconds = 3
        }

        public enum Weekday
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7
        }

        public enum MonthlyConfigurationWeekDay
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 3,
            Thursday = 4,
            Friday = 5,
            Saturday = 6,
            Sunday = 7,
            Day = 8,
            Weekday = 9,
            Weekend = 10
        }

        public enum Frecuency
        {
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Last = 5
        }

    }
}
