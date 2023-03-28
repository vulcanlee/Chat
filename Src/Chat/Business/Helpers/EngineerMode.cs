using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers;

public class EngineerModeData
{
    public EngineerModeCodeEnum EngineerCode { get; set; } = EngineerModeCodeEnum.All;
}

[Flags]
public enum EngineerModeCodeEnum
{
    None = 0,
    登出登入 = 1,
    資料庫存取 = 2,
    讀卡機操作 = 4,
    MQTT = 8,
    呼叫WebAPI = 16,
    HTTP請求與回應 = 32,
    Method執行速度 = 64,
    OS系統運作效能數據 = 128,
    All = 登出登入 | 資料庫存取 | 讀卡機操作 | MQTT | 呼叫WebAPI | HTTP請求與回應 |
        Method執行速度 | OS系統運作效能數據
}
