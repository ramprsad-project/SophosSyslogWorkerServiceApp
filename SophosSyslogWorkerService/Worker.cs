using Microsoft.Extensions.Configuration;

namespace SophosSyslogWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public IConfiguration _config;
        public string? _token { get; set; }

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger; 
            _config = config;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _token = new BearerToken().GetBearerToken(_config);
            }
            catch (Exception ex) { _logger.LogError(ex.ToString()); }
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    DBLogging dbobj = new DBLogging(_config);
                    dbobj.ExecuteSaveSystemEventsToDB(_token);
                    await Task.Delay(10000, stoppingToken);
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