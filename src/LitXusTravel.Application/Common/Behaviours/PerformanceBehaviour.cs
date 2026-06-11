using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LitXusTravel.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
            logger.LogWarning("Slow request: {RequestName} took {ElapsedMs}ms",
                typeof(TRequest).Name, sw.ElapsedMilliseconds);

        return response;
    }
}
