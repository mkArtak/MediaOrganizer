using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaOrganizer.Core
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

            return Task.Run(() =>
            {
                if (!string.Equals(from, to, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (File.Exists(to))
                    {
                        if (options.RemoveSourceAfterMove)
                        {
                            File.Delete(from);
                        }
                    }
                    else
                    {
                        File.Move(from, to);
                    }
                }
            });
        }
    }
}
