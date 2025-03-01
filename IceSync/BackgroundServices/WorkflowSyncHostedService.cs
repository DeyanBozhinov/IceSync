using IceSync.Services;

public class WorkflowSyncHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public WorkflowSyncHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
         while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedService = scope.ServiceProvider.GetRequiredService<IWorkflowService>();

                    await scopedService.WorkflowTableSync(stoppingToken);
                }
            } catch(Exception e)
            {
                // log error here
            }
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }
}