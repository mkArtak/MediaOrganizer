using MediaOrganizer.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediaOrganizer.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<FilesOrganizerOptions> _options;
        private readonly IFilesOrganizer _organizer;
        private readonly TimeSpan OrganizerRunPeriod = TimeSpan.FromMinutes(60);

        public Worker(ILogger<Worker> logger, IOptions<FilesOrganizerOptions> options, IFilesOrganizer organizer)
        {
            _logger = logger;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Worker started with options: {_options.Value}");

                await _organizer.OrganizeAsync(stoppingToken);

                await Task.Delay(OrganizerRunPeriod);
            }
        }
    }
}
