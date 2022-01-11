using Scheduler.Languages;
using System.Globalization;
using static Scheduler.Enumerations;

namespace Scheduler
{
    public class Language
    {
        private readonly Culture _language;
        private Resources _resources;

        public Language()
        {
            _language = Culture.enGB;
            LoadResources();
        }

        public Language(Culture language)
        {
            _language = language;
            LoadResources();
        }

        public Resources Resources
        {
            get
            {
                return _resources;
            }
        }

        private void LoadResources()
        {
            switch (_language)
            {
                
                case Culture.enUS:
                    _resources = new enUS();
                    CultureInfo.CurrentCulture = new CultureInfo("en-US");
                    CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                    break;
                case Culture.esES:
                    _resources = new esES();
                    CultureInfo.CurrentCulture = new CultureInfo("es-ES");
                    CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
                    break;
                default:
                case Culture.enGB:
                    _resources = new enGB();
                    CultureInfo.CurrentCulture = new CultureInfo("en-GB");
                    CultureInfo.CurrentUICulture = new CultureInfo("en-GB");
                    break;
            }
        }

        public string GetEnumFrecuencyTranslated(Frecuency? frecuency)
        {
            switch (frecuency)
            {
                case Frecuency.First:
                    return _resources.First;
                case Frecuency.Second:
                    return _resources.Second;
                case Frecuency.Third:
                    return _resources.Third;
                case Frecuency.Fourth:
                    return _resources.Fourth;
                case Frecuency.Last:
                    return _resources.Last;
                default:
                    return string.Empty;
            }
        }

        public string GetEnumMonthlyConfigurationWeekDayTranslated(MonthlyConfigurationWeekDay? weekday)
        {
            switch (weekday)
            {
                case MonthlyConfigurationWeekDay.Monday:
                    return _resources.Monday;
                case MonthlyConfigurationWeekDay.Tuesday:
                    return _resources.Tuesday;
                case MonthlyConfigurationWeekDay.Wednesday:
                    return _resources.Wednesday;
                case MonthlyConfigurationWeekDay.Thursday:
                    return _resources.Thursday;
                case MonthlyConfigurationWeekDay.Friday:
                    return _resources.Friday;
                case MonthlyConfigurationWeekDay.Saturday:
                    return _resources.Saturday;
                case MonthlyConfigurationWeekDay.Sunday:
                    return _resources.Sunday;
                case MonthlyConfigurationWeekDay.Day:
                    return _resources.Day;
                case MonthlyConfigurationWeekDay.Weekday:
                    return _resources.Weekday;
                case MonthlyConfigurationWeekDay.Weekend:
                    return _resources.Weekend;
                default:
                    return string.Empty;
            }
        }

        public string GetEnumDailyOccurrenceTranslated(Enumerations.DailyOccurrence dailyOccurrence)
        {
            switch (dailyOccurrence)
            {
                case DailyOccurrence.Hours:
                    return _resources.Hours;
                case DailyOccurrence.Minutes:
                    return _resources.Minutes;
                case DailyOccurrence.Seconds:
                    return _resources.Seconds;
                default:
                    return string.Empty;
            }
        }

    }

    public enum Culture
    {
        esES = 0,
        enGB = 1,
        enUS = 2
    }

    public interface Resources
    {
        string OccursOnce { get; }

        string OccursEveryDay { get; }

        string ScheduleWillBeUsed { get; }

        string StartingOn { get; }

        string EndingOn { get; }

        string Every { get; }

        string On { get; }

        string BetweenAnd { get; }

        string Occurs { get; }

        string TheXYOfEveryZMonths { get; }

        string TheParameterConfigurationShouldNotBeNull { get; }

        string ShouldLimitEndDate { get; }

        string DailyFrecuencyShouldAddStartAndEndTime { get; }

        string MonthlyConfigurationShouldNotBeNull { get; }

        string ShouldNotSelectConcreteDayAndSomeDayAtSameTime { get; }

        string SouldInsertPositiveDayNumber { get; }

        string ShouldInsertMonthFrecuency { get; }

        string ShouldInsertFrecuencySomeDay { get; }

        string ShouldInsertWeekDaySomeDay { get; }

        string ShouldInsertMonthFrecuencySomeDay { get; }

        string ShouldInsertPositiveMonthFrecuency { get; }

        string First { get; }

        string Second { get; }

        string Third { get; }

        string Fourth { get; }

        string Last { get; }

        string Monday { get; }
        string Tuesday { get; }
        string Wednesday { get; }
        string Thursday { get; }
        string Friday { get; }
        string Saturday { get; }
        string Sunday { get; }
        string Day { get; }
        string Weekday { get; }
        string Weekend { get; }

        string Hours { get; }
        string Minutes { get; }
        string Seconds { get; }

    }
}
