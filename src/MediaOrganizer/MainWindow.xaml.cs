using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeMediaOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string VideoSubfolderName = "Movies";
        private const string PhotosSubfolderName = "Photos";

        private static readonly string[] VideoFileFormatPatterns = new[] { ".mov", ".mp4", ".avi", ".3gp", ".mpg" };

        private static readonly string[] ImageFileFormatPatterns = new[] { ".jpg", ".jpeg" };

        private bool isRunning = false;

        public bool IsRunning
        {
            get { return this.isRunning; }
            set
            {
                if (value != this.isRunning)
                {
                    this.isRunning = value;
                }
            }
        }

        private void OnStateChanged()
        {
            this.txtDestination.IsEnabled = !this.IsRunning;
            this.txtSource.IsEnabled = !this.IsRunning;
            this.btnStart.Content = this.IsRunning ? "Stop" : "Start";
            this.prgProgress.Visibility = this.IsRunning ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private string SourceDirectory
        {
            get
            {
                return this.txtSource.Text;
            }
        }

        private string DestinationDirectory
        {
            get
            {
                return this.txtDestination.Text;
            }
        }

        private bool DeleteOnImport
        {
            get
            {
                return true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(this.SourceDirectory))
            {
                throw new DirectoryNotFoundException("Source not found");
            }

            if (this.IsRunning = !this.IsRunning)
            {
                this.OnStateChanged();
            }
            else
            {
                return;
            }

            //await MoveVideoFilesToMoviesSubfolder();
            await OrganizeFilesByDatesAndTypes(this.DestinationDirectory, this.SourceDirectory);

            this.IsRunning = false;
            this.OnStateChanged();
        }

        private async Task OrganizeFilesByDatesAndTypes(string mediaRoot, string sourceRoot)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(mediaRoot))
                {
                    Debug.WriteLine("Creating directory '{0}'", mediaRoot);
                    Directory.CreateDirectory(mediaRoot);
                }

                foreach (string file in Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories))
                {
                    FileType type = GetFileType(file);
                    if (type == FileType.Unknown)
                    {
                        Debug.WriteLine("Unknown file type: '{0}'. Skipping.", file);
                        continue;
                    }

                    string destinationPath = GetDestinationDirectoryForFile(mediaRoot, file, type);
                    Debug.WriteLine("Moving file '{0}' to '{1}'.", file, destinationPath);
                    if (!string.Equals(file, destinationPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (File.Exists(destinationPath))
                        {
                            Debug.WriteLine("Skipping file '{0}' as it already exists in destination.", file);

                            if (this.DeleteOnImport)
                            {
                                File.Delete(file);
                            }
                        }
                        else
                        {
                            File.Move(file, destinationPath);
                        }
                    }
                }
            });
        }

        private async Task MoveVideoFilesToMoviesSubfolder()
        {
            if (!Directory.Exists(this.DestinationDirectory))
            {
                Debug.WriteLine("Creating directory '{0}'", this.DestinationDirectory);
                Directory.CreateDirectory(this.DestinationDirectory);
            }

            foreach (string sourceDateDirectory in Directory.GetDirectories(this.SourceDirectory))
            {
                foreach (string format in VideoFileFormatPatterns)
                {
                    foreach (string sourceFile in Directory.EnumerateFiles(sourceDateDirectory, format))
                    {
                        if (!this.IsRunning)
                        {
                            Debug.WriteLine("Stopping processing per user request");
                            break;
                        }

                        string matchingDestinationDirectory = Path.Combine(this.DestinationDirectory, new DirectoryInfo(sourceDateDirectory).Name);
                        if (!Directory.Exists(matchingDestinationDirectory))
                        {
                            Debug.WriteLine("Creating destination directory '{0}'", matchingDestinationDirectory);
                            await IOExtensions.CreateDirectoryAsync(matchingDestinationDirectory);
                        }

                        string destinationFile = Path.Combine(matchingDestinationDirectory, Path.GetFileName(sourceFile));
                        if (File.Exists(destinationFile))
                        {
                            Debug.WriteLine("File '{0}' exinsts. Deleting source file '{1}'", destinationFile, sourceFile);
                            await IOExtensions.DeleteFileAsync(sourceFile);
                        }
                        else
                        {
                            Debug.WriteLine("Moving file '{0}' to '{1}'", sourceFile, destinationFile);
                            await IOExtensions.FastMoveFileAsync(sourceFile, destinationFile);
                        }
                    }

                    if (!Directory.EnumerateFileSystemEntries(sourceDateDirectory).Any())
                    {
                        Debug.WriteLine("Deleting directory '{0}'.", sourceDateDirectory);
                        await IOExtensions.DeleteDirectoryAsync(sourceDateDirectory);
                        break;
                    }
                }
            }

            if (!Directory.EnumerateFileSystemEntries(this.DestinationDirectory).Any())
            {
                await IOExtensions.DeleteDirectoryAsync(this.DestinationDirectory);
            }
        }

        private string GetDestinationDirectoryForFile(string root, string path, FileType fileType)
        {
            FileInfo info = new FileInfo(path);
            DateTime dateTaken = info.CreationTimeUtc > info.LastWriteTimeUtc ? info.LastWriteTimeUtc : info.CreationTimeUtc;
            string typeSubfolder;
            switch (fileType)
            {
                case FileType.Image:
                    typeSubfolder = PhotosSubfolderName;
                    break;

                case FileType.Movie:
                    typeSubfolder = VideoSubfolderName;
                    break;

                default:
                    throw new ApplicationException("Unexpected file type");
            }

            string directory = Path.Combine(root, typeSubfolder, dateTaken.Year.ToString(), dateTaken.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, Path.GetFileName(path));
        }

        private static FileType GetFileType(string filename)
        {
            FileType result = FileType.Unknown;
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            if (ImageFileFormatPatterns.Contains(extension))
            {
                result = FileType.Image;
            }
            else if (VideoFileFormatPatterns.Contains(extension))
            {
                result = FileType.Movie;
            }
            else
            {
                result = FileType.Unknown;
            }

            return result;
        }

        private enum FileType
        {
            Image,
            Movie,
            Unknown
        }
    }
}
