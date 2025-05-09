﻿using MediaOrganizer.Core;
using Prism.Mvvm;

namespace MediaOrganizer.ViewModels
{
    public class FileOrganizerOptionsViewModel : BindableBase
    {
        private string videoSubfolderName = FilesOrganizerOptions.DefaultVideoSubfolderName;
        private string photosSubfolderName = FilesOrganizerOptions.DefaultPhotosSubfolderName;

        private string[] photoExtensions = FilesOrganizerOptions.DefaultPhotoFileFormatPatterns;
        private string[] videoExtensions = FilesOrganizerOptions.DefaultVideoFileFormatPatterns;

        private string sourceRoot;
        private string destinationRoot;
        private bool removeSource;
        private bool skipExistingFiles;
        private string destinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";
        private bool deleteEmptyFolders;

        public bool SkipExistingFiles { get => skipExistingFiles; set => SetProperty(ref skipExistingFiles, value); }

        public bool DeleteEmptyFolders { get => deleteEmptyFolders; set => SetProperty(ref deleteEmptyFolders, value); }

        public bool RemoveSource { get => removeSource; set => SetProperty(ref this.removeSource, value); }

        public string DestinationRoot { get => destinationRoot; set => SetProperty(ref this.destinationRoot, value); }

        public string SourceRoot { get => sourceRoot; set => SetProperty(ref this.sourceRoot, value); }

        public string PhotosSubfolderName { get => photosSubfolderName; set => SetProperty(ref this.photosSubfolderName, value); }

        public string VideoSubfolderName { get => videoSubfolderName; set => SetProperty(ref this.videoSubfolderName, value); }

        public string DestinationPattern { get => destinationPattern; set => SetProperty(ref destinationPattern, value); }

        public string[] PhotoExtensions { get => photoExtensions; set => SetProperty(ref photoExtensions, value); }

        public string[] VideoExtensions { get => videoExtensions; set => SetProperty(ref videoExtensions, value); }

        public FileOrganizerOptionsViewModel() : base()
        {

        }

        public FileOrganizerOptionsViewModel(FilesOrganizerOptions options) : base()
        {
            this.SourceRoot = options.SourceRoot;
            this.DestinationRoot = options.DestinationRoot;
            this.PhotosSubfolderName = options.PhotosSubfolderName;
            this.VideoSubfolderName = options.VideoSubfolderName;
            this.RemoveSource = options.RemoveSource;
            this.SkipExistingFiles = options.SkipExistingFiles;
            this.DestinationPattern = options.DestinationPattern;
            this.PhotoExtensions = options.ImageFileFormatPatterns;
            this.VideoExtensions = options.VideoFileFormatPatterns;
            this.DeleteEmptyFolders = options.DeleteEmptyFolders;
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
                DestinationPattern = this.DestinationPattern,
                ImageFileFormatPatterns = this.PhotoExtensions,
                VideoFileFormatPatterns = this.VideoExtensions,
                DeleteEmptyFolders = this.DeleteEmptyFolders
            };
        }
    }
}
