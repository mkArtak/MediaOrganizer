using MediaOrganizer.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local
{
    internal sealed class PhysicalFileMover : IFileMover
    {
        private readonly ILogger logger;

        public PhysicalFileMover(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task MoveAsync(FileMoverOptions options, string from, string to)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.Equals(from, to, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            if (options.SkipIfFileExists && File.Exists(to))
            {
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                try
                {
                    if (options.RemoveSourceAfterMove)
                    {
                        this.logger.LogInformation($"Moving {from} to {to}");
                        File.Move(from, to);
                    }
                    else
                    {
                        this.logger.LogInformation($"Copying {from} to {to}");
                        File.Copy(from, to);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning($"Failed to process file {from}. Reason: {ex.Message}");
                }
            });
        }
    }
}