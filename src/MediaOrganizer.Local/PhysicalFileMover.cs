using MediaOrganizer.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaOrganizer.Local
{
    internal sealed class PhysicalFileMover : IFileMover
    {
        public PhysicalFileMover()
        {
        }

        public async Task MoveAsync(FileMoverOptions options, string from, string to)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.Equals(from, to, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (File.Exists(to) && options.SkipIfFileExists)
            {
                return;
            }

            await Task.Run(() =>
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