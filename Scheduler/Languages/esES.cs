namespace Scheduler.Languages
{
    public class esES : Resources
    {
        public esES() { }

        public string TheParameterConfigurationShouldNotBeNull => "El parámetro Configuración no debe ser nulo.";

        public string ShouldLimitEndDate => "Si la configuración es Recurrencia, debes añadir Fecha Fin Límite";

        public string DailyFrecuencyShouldAddStartAndEndTime => "Si la configuración es Frecuencia Diaria, debes añadir Hora Inicio y Fin.";

        public string MonthlyConfigurationShouldNotBeNull => "The parameter MonthlyConfiguration should not be null.";

        public string ShouldNotSelectConcreteDayAndSomeDayAtSameTime => "No debes seleccionar Dia Concreto y Algún Día al mismo tiempo.";

        public string SouldInsertPositiveDayNumber => "Debes insertar un Dia positivo si indicas un Día Concreto.";

        public string ShouldInsertMonthFrecuency => "Debes insertar Frecuencia Mensual si indicas Día Concreto.";

        public string ShouldInsertFrecuencySomeDay =>   "You should insert Frecuency if you set Some Day.";

        public string ShouldInsertWeekDaySomeDay => "Debes insertar un Día de la Semana si indicas Algún Día.";

        public string ShouldInsertMonthFrecuencySomeDay => "Debes insertar Frecuencia Mensual si indicas Algún Día.";

        public string ShouldInsertPositiveMonthFrecuency => "Debes insertar una Frecuencia Mensual positiva.";

        public string OccursOnce => "Ocurrencia Única.";

        public string OccursEveryDay => "Ocurrencia Todos los Días.";

        public string ScheduleWillBeUsed => "Calendario utilizado el {0} a las {1}";

        public string StartingOn => "empezando en";

        public string EndingOn => "terminando el";

        public string Every => "cada";

        public string On => "a las";

        public string BetweenAnd => "entre las {0} y las {1}";

        public string Occurs => "Con ocurrencia";

        public string TheXYOfEveryZMonths => "el {0} {1} de cada {2} meses";

        public string First => "Primer";

        public string Second => "Segundo";

        public string Third => "Tercer";

        public string Fourth => "Cuarto";

        public string Last => "Último";

        public string Monday => "Lunes";

        public string Tuesday => "Martes";

        public string Wednesday => "Miércoles";

        public string Thursday => "Jueves";

        public string Friday => "Viernes";

        public string Saturday => "Sábado";

        public string Sunday => "Domingo";

        public string Day => "Día";

        public string Weekday => "Día de la semana";

        public string Weekend => "Día de fin de semana";

        public string Hours => "Horas";

        public string Minutes => "Minutos";

        public string Seconds => "Segundos";
    }
}
