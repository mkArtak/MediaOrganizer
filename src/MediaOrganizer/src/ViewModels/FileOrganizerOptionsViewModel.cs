using MediaOrganizer.Core;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
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
        private ObservableCollection<MediaCategory> mediaCategories = new();
        
        public DelegateCommand AddCategoryCommand { get; }
        public DelegateCommand<MediaCategory> RemoveCategoryCommand { get; }

        public bool SkipExistingFiles { get => skipExistingFiles; set => SetProperty(ref skipExistingFiles, value); }

        public bool DeleteEmptyFolders { get => deleteEmptyFolders; set => SetProperty(ref deleteEmptyFolders, value); }

        public bool RemoveSource { get => removeSource; set => SetProperty(ref this.removeSource, value); }

        public string DestinationRoot { get => destinationRoot; set => SetProperty(ref this.destinationRoot, value); }

        public string SourceRoot { get => sourceRoot; set => SetProperty(ref this.sourceRoot, value); }

        public string DestinationPattern { get => destinationPattern; set => SetProperty(ref destinationPattern, value); }

        public ObservableCollection<MediaCategory> MediaCategories { get => mediaCategories; set => SetProperty(ref mediaCategories, value); }

        public FileOrganizerOptionsViewModel() : base()
        {
            this.AddCategoryCommand = new DelegateCommand(AddCategory);
            this.RemoveCategoryCommand = new DelegateCommand<MediaCategory>(RemoveCategory);
        }

        public FileOrganizerOptionsViewModel(FilesOrganizerOptions options) : base()
        {
            this.SourceRoot = options.SourceRoot;
            this.DestinationRoot = options.DestinationRoot;
            this.RemoveSource = options.RemoveSource;
            this.SkipExistingFiles = options.SkipExistingFiles;
            this.DestinationPattern = options.DestinationPattern;
            this.DeleteEmptyFolders = options.DeleteEmptyFolders;
            this.MediaCategories = new ObservableCollection<MediaCategory>(options.MediaCategories);

            this.AddCategoryCommand = new DelegateCommand(AddCategory);
            this.RemoveCategoryCommand = new DelegateCommand<MediaCategory>(RemoveCategory);
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

        private void AddCategory()
        {
            MediaCategories.Add(new MediaCategory
            {
                CategoryName = "New Category",
                CategoryRoot = string.Empty,
                FileExtensions = []
            });
        }

        private void RemoveCategory(MediaCategory category)
        {
            if (category is not null)
            {
                MediaCategories.Remove(category);
            }
        }
    }
}
