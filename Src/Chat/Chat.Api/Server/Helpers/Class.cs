using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Chat.Api.Helpers;

public class RequestResponseLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Log request
        _logger.LogInformation($"Received {context.Request.Method} request for {context.Request.Path}");

        // Call next middleware
        await next(context);

        // Log response
        _logger.LogInformation($"Sent {context.Response.StatusCode} response for {context.Request.Path}");
    }
}

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 記錄 HTTP 請求
        LogRequest(context);

        // 呼叫下一個 Middleware
        await _next(context);

        // 記錄 HTTP 回應
        LogResponse(context);
    }

    private void LogRequest(HttpContext context)
    {
        // 取得 HTTP 請求的詳細資訊
        var request = context.Request;
        var body = request.Body;

        // 記錄 HTTP 請求的詳細訊息，包括傳輸的 body payload
        // 這裡只是一個範例，實際實作中需要使用適當的日誌庫
        Console.WriteLine($"Request Method: {request.Method}");
        Console.WriteLine($"Request Path: {request.Path}");
        Console.WriteLine($"Request Body: {new StreamReader(body).ReadToEnd()}");
    }

    private void LogResponse(HttpContext context)
    {
        // 取得 HTTP 回應的詳細資訊
        var response = context.Response;
        var body = response.Body;

        // 記錄 HTTP 回應的詳細訊息，包括傳輸的 body payload
        // 這裡只是一個範例，實際實作中需要使用適當的日誌庫
        Console.WriteLine($"Response Status Code: {response.StatusCode}");
        Console.WriteLine($"Response Body: {new StreamReader(body).ReadToEnd()}");
    }
}


public class LoggingMiddleware2
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware2(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestBodyStream = new MemoryStream();
        var originalRequestBody = context.Request.Body;

        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);

        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();

        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        // Log the request details
        var headers = context.Request.Headers;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString;
        var body = requestBodyText;

        // Call the next middleware in the pipeline
        await _next(context);

        // Reset the request body stream
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        await requestBodyStream.CopyToAsync(originalRequestBody);
        context.Request.Body = originalRequestBody;
    }

}
