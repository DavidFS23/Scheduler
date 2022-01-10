using Scheduler.Languages;
using System.Globalization;

namespace Scheduler
{
    public class Language
    {
        private readonly Culture _language;
        private Resources _resources;

        public Language()
        {
            _language = Culture.esES;
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
                case Culture.enGB:
                    _resources = new enGB();
                    CultureInfo.CurrentCulture = new CultureInfo("en-GB");
                    CultureInfo.CurrentUICulture = new CultureInfo("en-GB");
                    break;
                case Culture.enUS:
                    _resources = new enUS();
                    CultureInfo.CurrentCulture = new CultureInfo("en-US");
                    CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                    break;
                case Culture.esES:
                default:
                    _resources = new esES();
                    CultureInfo.CurrentCulture = new CultureInfo("es-ES");
                    CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
                    break;
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
        string TheParameterConfigurationShouldNotBeNull { get; }

        string ShouldLimitEndDate { get; }
    }
}
