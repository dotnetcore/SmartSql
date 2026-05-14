using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SmartSql.Middlewares.Filters;

namespace SmartSql.Test.Integration.Fixtures;

public class TestPrepareStatementFilter : IPrepareStatementFilter, ISetupSmartSql
{
    private ILogger<TestPrepareStatementFilter> _logger;

    public void OnInvoking(ExecutionContext context) => _logger.LogDebug("TestPrepareStatementFilter.OnInvoking");
    public void OnInvoked(ExecutionContext context) => _logger.LogDebug("TestPrepareStatementFilter.OnInvoked");
    public Task OnInvokingAsync(ExecutionContext context) { _logger.LogDebug("TestPrepareStatementFilter.OnInvokingAsync"); return Task.CompletedTask; }
    public Task OnInvokedAsync(ExecutionContext context) { _logger.LogDebug("TestPrepareStatementFilter.OnInvokedAsync"); return Task.CompletedTask; }
    public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder) => _logger = smartSqlBuilder.LoggerFactory.CreateLogger<TestPrepareStatementFilter>();
}
