using System;

namespace Scheduler
{
    public struct CalculationResult
    {
        public DateTime NextExecutionTime { get; set; }
        public string Description { get; set; }
    }
}
