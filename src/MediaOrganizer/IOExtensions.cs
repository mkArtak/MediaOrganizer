using System.IO;
using System.Threading.Tasks;

namespace HomeMediaOrganizer
{
    public static class IOExtensions
    {
        public static async Task CopyFileAsync(string source, string destination)
        {
            using (FileStream sourceStream = File.OpenRead(source))
            {
                using (FileStream destinationStream = File.Create(destination))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        public static async Task MoveFileAsync(string source, string destination)
        {
            await CopyFileAsync(source, destination);

            try
            {
                File.Delete(source);
            }
            catch
            {
                File.Delete(destination);
                throw;
            }
        }

        public static async Task CreateDirectoryAsync(string path)
        {
            await Task.Factory.StartNew(() => Directory.CreateDirectory(path));
        }

        public static async Task DeleteFileAsync(string path)
        {
            await Task.Factory.StartNew(() => File.Delete(path));
        }

        public static async Task FastMoveFileAsync(string source, string destination)
        {
            await Task.Factory.StartNew(() => File.Move(source, destination));
        }

        internal static async Task DeleteDirectoryAsync(string sourceDateDirectory)
        {
            await Task.Factory.StartNew(() => Directory.Delete(sourceDateDirectory));
        }
    }
}
