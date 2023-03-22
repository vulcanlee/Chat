using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public class PerformanceMeter<T> : IDisposable
    {
        private readonly ILogger<T> logger;
        private readonly LoggerHelper loggerHelper;
        private readonly string message;

        public PerformanceMeter(ILogger<T> logger, LoggerHelper loggerHelper, 
            string message)
        {
            this.logger = logger;
            this.loggerHelper = loggerHelper;
            this.message = message;
        }

        public static PerformanceMeter<T> StartWatch(ILogger<T> logger, LoggerHelper loggerHelper,
            string message)
        {
            return new PerformanceMeter<T>(logger, loggerHelper, message); 
        }

        public DateTime Begin { get; set; } = DateTime.Now;
        public void Dispose()
        {
            DateTime completion = DateTime.Now;
            TimeSpan estimatedTime = completion - Begin;

            loggerHelper.SendLog(() =>
            {
                logger.LogDebug($"{message} Estimated Time: {estimatedTime.TotalMilliseconds} ({Begin}-{completion}");
            }, EngineerModeCodeEnum.Method執行速度);
        }
    }
}
