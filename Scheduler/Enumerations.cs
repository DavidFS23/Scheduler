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

    }
}
