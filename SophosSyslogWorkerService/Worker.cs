namespace SophosSyslogWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try { }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            try { }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
            return base.StopAsync(cancellationToken);
        }
    }
}