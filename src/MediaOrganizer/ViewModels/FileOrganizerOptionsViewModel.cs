using MediaOrganizer.Core;
using Prism.Mvvm;

namespace MediaOrganizer.ViewModels
{
    public class FileOrganizerOptionsViewModel : BindableBase
    {
        private string videoSubfolderName = FilesOrganizerOptions.DefaultVideoSubfolderName;
        private string photosSubfolderName = FilesOrganizerOptions.DefaultPhotosSubfolderName;
        private string sourceRoot;
        private string destinationRoot;
        private bool removeSource;
        private bool skipExistingFiles;
        private string destinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";

        public bool SkipExistingFiles { get => skipExistingFiles; set => SetProperty(ref skipExistingFiles, value); }

        public bool RemoveSource { get => removeSource; set => SetProperty(ref this.removeSource, value); }

        public string DestinationRoot { get => destinationRoot; set => SetProperty(ref this.destinationRoot, value); }

        public string SourceRoot { get => sourceRoot; set => SetProperty(ref this.sourceRoot, value); }

        public string PhotosSubfolderName { get => photosSubfolderName; set => SetProperty(ref this.photosSubfolderName, value); }

        public string VideoSubfolderName { get => videoSubfolderName; set => SetProperty(ref this.videoSubfolderName, value); }

        public string DestinationPattern { get => destinationPattern; set => SetProperty(ref destinationPattern, value); }

        public FileOrganizerOptionsViewModel() : base()
        {
        }

        public FilesOrganizerOptions GetOptions()
        {
            return new FilesOrganizerOptions
            {
                SourceRoot = this.SourceRoot,
                DestinationRoot = this.DestinationRoot,
                PhotosSubfolderName = this.PhotosSubfolderName,
                VideoSubfolderName = this.VideoSubfolderName,
                RemoveSource = this.RemoveSource,
                SkipExistingFiles = this.SkipExistingFiles,
                DestinationPattern = this.DestinationPattern
            };
        }
    }
}
