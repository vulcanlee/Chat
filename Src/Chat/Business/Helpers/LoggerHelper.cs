using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers;

public class LoggerHelper
{
    private readonly EngineerModeData engineerModeData;

    public LoggerHelper(EngineerModeData engineerModeData)
    {
        this.engineerModeData = engineerModeData;
    }

    public void SendLog(Action writeLogHandler, EngineerModeCodeEnum engineerMode)
    {
        var actual = (this.engineerModeData.EngineerCode & engineerMode) == engineerMode;
        if (actual) writeLogHandler?.Invoke();
    }

    /// <summary>
    /// 加入某個工程模式
    /// </summary>
    /// <param name="engineerMode">指定之工程模式列舉值</param>
    public void AddEngineerMode(EngineerModeCodeEnum engineerMode)
    {
        this.engineerModeData.EngineerCode = this.engineerModeData.EngineerCode | engineerMode;
    }

    /// <summary>
    /// 移除某個工程模式
    /// </summary>
    /// <param name="engineerMode">指定之工程模式列舉值</param>
    public void RemoveEngineerMode(EngineerModeCodeEnum engineerMode)
    {
        this.engineerModeData.EngineerCode = this.engineerModeData.EngineerCode & ~engineerMode;
    }

    /// <summary>
    /// 強制設定某個工程模式
    /// </summary>
    /// <param name="engineerMode">指定之工程模式列舉值</param>
    public void SetEngineerMode(EngineerModeCodeEnum engineerMode)
    {
        this.engineerModeData.EngineerCode = engineerMode;
    }

    /// <summary>
    /// 清除所有工程模式
    /// </summary>
    /// <param name="engineerMode">指定之工程模式列舉值</param>
    public void ClearEngineerMode()
    {
        this.engineerModeData.EngineerCode = EngineerModeCodeEnum.None;
    }
}
