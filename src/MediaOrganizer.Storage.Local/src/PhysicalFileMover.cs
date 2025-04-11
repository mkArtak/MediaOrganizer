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
        private readonly Action<string, string> moveHandler;
        private readonly Action<string, string> copyHandler;

        public PhysicalFileMover(ILogger logger) : this(File.Copy, File.Move, logger)
        {
        }

        internal PhysicalFileMover(Action<string, string> copyHandler, Action<string, string> moveHandler, ILogger logger)
        {
            this.moveHandler = moveHandler ?? throw new ArgumentNullException(nameof(moveHandler));
            this.copyHandler = copyHandler ?? throw new ArgumentNullException(nameof(copyHandler));
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
                        this.moveHandler(from, to);
                    }
                    else
                    {
                        this.logger.LogInformation($"Copying {from} to {to}");
                        this.copyHandler(from, to);
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