using MediaOrganizer.Core;
using Prism.Mvvm;
using System.Collections.Generic;

namespace MediaOrganizer.ViewModels
{
    public class FileOrganizerOptionsViewModel : BindableBase
    {
        private string sourceRoot;
        private string destinationRoot;
        private bool removeSource;
        private bool skipExistingFiles;
        private string destinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";
        private bool deleteEmptyFolders;
        private List<MediaCategory> mediaCategories;

        public bool SkipExistingFiles { get => skipExistingFiles; set => SetProperty(ref skipExistingFiles, value); }

        public bool DeleteEmptyFolders { get => deleteEmptyFolders; set => SetProperty(ref deleteEmptyFolders, value); }

        public bool RemoveSource { get => removeSource; set => SetProperty(ref this.removeSource, value); }

        public string DestinationRoot { get => destinationRoot; set => SetProperty(ref this.destinationRoot, value); }

        public string SourceRoot { get => sourceRoot; set => SetProperty(ref this.sourceRoot, value); }

        public string DestinationPattern { get => destinationPattern; set => SetProperty(ref destinationPattern, value); }

        public List<MediaCategory> MediaCategories { get => mediaCategories; set => SetProperty(ref mediaCategories, value); }

        public FileOrganizerOptionsViewModel() : base()
        {

        }

        public FileOrganizerOptionsViewModel(FilesOrganizerOptions options) : base()
        {
            this.SourceRoot = options.SourceRoot;
            this.DestinationRoot = options.DestinationRoot;
            this.RemoveSource = options.RemoveSource;
            this.SkipExistingFiles = options.SkipExistingFiles;
            this.DestinationPattern = options.DestinationPattern;
            this.DeleteEmptyFolders = options.DeleteEmptyFolders;
            this.MediaCategories = options.MediaCategories;
        }

        public FilesOrganizerOptions GetOptions()
        {
            var result = new FilesOrganizerOptions
            {
                SourceRoot = this.SourceRoot,
                DestinationRoot = this.DestinationRoot,
                RemoveSource = this.RemoveSource,
                SkipExistingFiles = this.SkipExistingFiles,
                DestinationPattern = this.DestinationPattern,
                DeleteEmptyFolders = this.DeleteEmptyFolders,
            };

            result.MediaCategories.AddRange(this.MediaCategories ?? new List<MediaCategory>());

            return result;
        }
    }
}
