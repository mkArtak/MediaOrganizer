using MediaOrganizer.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaOrganizer.Storage.Local
{
    internal sealed class PhysicalFileMover : IFileMover
    {
        public PhysicalFileMover()
        {
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
                if (options.RemoveSourceAfterMove)
                {
                    File.Move(from, to);
                }
                else
                {
                    File.Copy(from, to);
                }
            });
        }
    }
}