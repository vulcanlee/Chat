using System.Text;
using System.Threading.Tasks;
using Business.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace Chat.Api.Helpers;

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

public class RequestResponseLoggingMiddleware 
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestResponseLoggingMiddleware> logger;
    private readonly LoggerHelper loggerHelper;

    public RequestResponseLoggingMiddleware(RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger, LoggerHelper loggerHelper)
    {
        this.next = next;
        this.logger = logger;
        this.loggerHelper = loggerHelper;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 記錄 HTTP 請求
            await LogRequest(context);

            // 呼叫下一個 Middleware
            await next(context);

            // 記錄 HTTP 回應
            await LogResponse(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"發生例外異常 {ex.Message}");
        }
    }
    private async Task LogRequest(HttpContext context)
    {
        // 取得 HTTP 請求的詳細資訊
        var request = context.Request;
        var body = request.Body;
        var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        var method = request.Method;
        var headers = request.Headers;

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"[{method}] {url}{Environment.NewLine}");
        stringBuilder.Append($"Headers{Environment.NewLine}");
        foreach (var item in headers)
        {
            stringBuilder.Append($"{item.Key} : {item.Value}{Environment.NewLine}");
        }
        var bodyContent = await new StreamReader(body).ReadToEndAsync();
        stringBuilder.Append($"{Environment.NewLine}");
        stringBuilder.Append($"{bodyContent}{Environment.NewLine}");
        stringBuilder.Append($"{Environment.NewLine}");

        loggerHelper.SendLog(() =>
        {
            logger.LogDebug($"{stringBuilder.ToString()}");
        }, EngineerModeCodeEnum.HTTP請求與回應);
    }

    private async Task LogResponse(HttpContext context)
    {
        // 取得 HTTP 回應的詳細資訊
        var request = context.Request;
        var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        var method = request.Method;
        var response = context.Response;
        var body = response.Body;
        var headers = context.Response.Headers;

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"[{method}] {url}{Environment.NewLine}");
        stringBuilder.Append($"Response Status Code: {response.StatusCode}{Environment.NewLine}");

        //if(context.Response.)

        stringBuilder.Append($"Headers{Environment.NewLine}");
        foreach (var item in headers)
        {
            stringBuilder.Append($"{item.Key} : {item.Value}{Environment.NewLine}");
        }

        response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        stringBuilder.Append($"{responseBody}{Environment.NewLine}");
        stringBuilder.Append($"{Environment.NewLine}");

        loggerHelper.SendLog(() =>
        {
            logger.LogDebug($"{stringBuilder.ToString()}");
        }, EngineerModeCodeEnum.HTTP請求與回應);
    }
}
