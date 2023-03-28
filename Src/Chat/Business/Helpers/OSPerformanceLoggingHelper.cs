using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public enum OSPerformanceCounterItem
    {
        Processor, AvailableMemory, AvailablePhysicalMemory
    }
    public class OSPerformanceLoggingHelper
    {
        private readonly ILogger<OSPerformanceLoggingHelper> logger;
        private readonly LoggerHelper loggerHelper;
        private readonly EngineerModeData engineerModeData;
        private bool enableOSPerformanceMeasure;
        public OSPerformanceLoggingHelper(ILogger<OSPerformanceLoggingHelper> logger,
            LoggerHelper loggerHelper, EngineerModeData engineerModeData)
        {
            this.logger = logger;
            this.loggerHelper = loggerHelper;
            this.engineerModeData = engineerModeData;
        }
        public Dictionary<OSPerformanceCounterItem, PerformanceCounter> PerformanceCounters { get; set; } = new();

        public async Task Prepare()
        {
            enableOSPerformanceMeasure = (this.engineerModeData.EngineerCode &
                EngineerModeCodeEnum.OS系統運作效能數據) == EngineerModeCodeEnum.OS系統運作效能數據;

            if (enableOSPerformanceMeasure)
            {
                string processName = Process.GetCurrentProcess().ProcessName;
                string category = string.Empty;
                string counter = string.Empty;
                string instance = string.Empty;

                PerformanceCounters.Clear();
                // 取得 CPU 使用率計數器
                category = "Processor";
                counter = "% Processor Time";
                instance = "_Total";
                PerformanceCounter cpuCounter = new PerformanceCounter(category, counter, instance);
                PerformanceCounters.Add(OSPerformanceCounterItem.Processor, cpuCounter);

                category = "Memory";
                counter = "Available Bytes";
                PerformanceCounter availableMemoryCounter = new PerformanceCounter(category, counter);
                PerformanceCounters.Add(OSPerformanceCounterItem.AvailableMemory, availableMemoryCounter);

                // 第一次先取得數據，方便之後計算平均值
                await Task.Delay(1000);

                float cpuCounterValue = cpuCounter.NextValue();
                float availableMemoryCounterValue = availableMemoryCounter.NextValue();
            }
        }

        public void LogPerformance()
        {
            if (enableOSPerformanceMeasure)
            {
                string message = string.Empty;
                float cpuCounterValue = PerformanceCounters[OSPerformanceCounterItem.Processor].NextValue();
                float availableMemoryCounterValue = PerformanceCounters[OSPerformanceCounterItem.AvailableMemory].NextValue();
                Process proc = Process.GetCurrentProcess();
                var usedMemory = proc.PrivateMemorySize64;
                message = $"CPU % Processor Time : {cpuCounterValue} % , " +
                    $"Memory used : {usedMemory / 1024 / 1024} MB , " +
                    $"Memory Available : {availableMemoryCounterValue / 1024.0 / 1024.0} MB";
                loggerHelper.SendLog(() =>
                {
                    logger.LogDebug($"系統效能資訊 {message}");
                }, EngineerModeCodeEnum.OS系統運作效能數據);
            }
        }
    }
}
