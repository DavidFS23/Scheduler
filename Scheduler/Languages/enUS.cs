namespace Scheduler.Languages
{
    public class enUS : Resources
    {
        public enUS() { }

        public string TheParameterConfigurationShouldNotBeNull => "The parameter Configuration should not be null.";

        public string ShouldLimitEndDate => "If the configuration is Recurring, you should add Limit End Date.";

        public string DailyFrecuencyShouldAddStartAndEndTime => "If the configuration is Daily Frecuency, you should add Start and End Time.";

        public string MonthlyConfigurationShouldNotBeNull => "The parameter MonthlyConfiguration should not be null.";

        public string ShouldNotSelectConcreteDayAndSomeDayAtSameTime => "You should not select Concrete Day and Some Day at the same time.";

        public string SouldInsertPositiveDayNumber => "You should insert a positive Day Number if you set Concrete Day.";

        public string ShouldInsertMonthFrecuency => "You should insert Month Frecuency if you set Concrete Day.";

        public string ShouldInsertFrecuencySomeDay => "You should insert Frecuency if you set Some Day.";

        public string ShouldInsertWeekDaySomeDay => "You should insert Weekday if you set Some Day.";

        public string ShouldInsertMonthFrecuencySomeDay => "You should insert Month Frecuency if you set Some Day.";

        public string ShouldInsertPositiveMonthFrecuency => "You should insert positive Month Frecuency.";

        public string OccursOnce => "Occurs once.";

        public string OccursEveryDay => "Occurs every day.";

        public string ScheduleWillBeUsed => "Schedule will be used on {0} at {1}";

        public string StartingOn => "starting on";

        public string EndingOn => "ending on";

        public string Every => "every";

        public string On => "on";

        public string BetweenAnd => "between {0} and {1}";

        public string Occurs => "Occurs";

        public string TheXYOfEveryZMonths => "the {0} {1} of every {2} months";

        public string First => "First";

        public string Second => "Second";

        public string Third => "Third";

        public string Fourth => "Fourth";

        public string Last => "Last";

        public string Monday => "Monday";

        public string Tuesday => "Tuesday";

        public string Wednesday => "Wednesday";

        public string Thursday => "Thursday";

        public string Friday => "Friday";

        public string Saturday => "Saturday";

        public string Sunday => "Sunday";

        public string Day => "Day";

        public string Weekday => "Weekday";

        public string Weekend => "Weekend";

        public string Hours => "Hours";

        public string Minutes => "Minutes";

        public string Seconds => "Seconds";
    }
}
