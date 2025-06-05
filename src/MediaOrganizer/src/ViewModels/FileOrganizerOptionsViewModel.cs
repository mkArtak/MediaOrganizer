using MediaOrganizer.Core;
using MediaOrganizer.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MediaOrganizer.ViewModels;

public class FileOrganizerOptionsViewModel : BindableBase
{
    private string sourceRoot;
    private string destinationRoot;
    private bool removeSource;
    private bool skipExistingFiles;
    private string destinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";
    private bool deleteEmptyFolders;
    private ObservableCollection<MediaCategory> mediaCategories;
    private Window _currentDialog;

    public bool SkipExistingFiles { get => skipExistingFiles; set => SetProperty(ref skipExistingFiles, value); }

    public bool DeleteEmptyFolders { get => deleteEmptyFolders; set => SetProperty(ref deleteEmptyFolders, value); }

    public bool RemoveSource { get => removeSource; set => SetProperty(ref this.removeSource, value); }

    public string DestinationRoot { get => destinationRoot; set => SetProperty(ref this.destinationRoot, value); }

    public string SourceRoot { get => sourceRoot; set => SetProperty(ref this.sourceRoot, value); }

    public string DestinationPattern { get => destinationPattern; set => SetProperty(ref destinationPattern, value); }

    public ObservableCollection<MediaCategory> MediaCategories { get => mediaCategories; set => SetProperty(ref mediaCategories, value); }

    public DelegateCommand AddCategoryCommand { get; init; }

    public DelegateCommand<MediaCategory> EditCategoryCommand { get; init; }

    public FileOrganizerOptionsViewModel() : base()
    {
        AddCategoryCommand = new DelegateCommand(OnAddCategory);
        EditCategoryCommand = new DelegateCommand<MediaCategory>(OnEditCategory);
    }

    public FileOrganizerOptionsViewModel(FilesOrganizerOptions options) : this()
    {
        this.SourceRoot = options.SourceRoot;
        this.DestinationRoot = options.DestinationRoot;
        this.RemoveSource = options.RemoveSource;
        this.SkipExistingFiles = options.SkipExistingFiles;
        this.DestinationPattern = options.DestinationPattern;
        this.DeleteEmptyFolders = options.DeleteEmptyFolders;
        this.MediaCategories = new ObservableCollection<MediaCategory>(options.MediaCategories);
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

        result.MediaCategories.AddRange(this.MediaCategories ?? new ObservableCollection<MediaCategory>());

        return result;
    }

    private void OnAddCategory()
    {
        ShowCategoryEditorDialog(null);
    }

    private void OnEditCategory(MediaCategory category)
    {
        if (category != null)
        {
            ShowCategoryEditorDialog(category);
        }
    }

    private void ShowCategoryEditorDialog(MediaCategory category)
    {
        var vm = new MediaCategoryEditorViewModel(category, ValidateCategory,
            onSave: OnSaveCategory,
            onDelete: cat => { MediaCategories.Remove(MediaCategories.Single(c => c.CategoryName == cat.CategoryName)); _currentDialog.Close(); }
            );

        _currentDialog = new MediaCategoryEditorWindow
        {
            DataContext = vm
        };
        _currentDialog.ShowDialog();
        _currentDialog = null;
    }

    private void OnSaveCategory(MediaCategory existingCategory, MediaCategory newCategory)
    {
        if (existingCategory is not null)
            MediaCategories.Remove(existingCategory);

        MediaCategories.Add(newCategory);

        _currentDialog.Close();
    }

    private bool ValidateCategory(MediaCategory originalCategory, MediaCategory newCategory)
    {
        // The validation is done in two steps:
        // 1. Check if another category with the same name already exists
        // 2. Check if any other category has an extension defined by this category
        if (string.IsNullOrWhiteSpace(newCategory.CategoryName))
            return false;

        // The expectations is that there is only a small number of categories and each will have only a small number of extensions.
        // Hence, these iterative brute-force approach is ok.
        foreach (var cat in mediaCategories)
        {
            if (cat == originalCategory)
                continue; // Skip the original category being edited

            if (cat.CategoryName.Equals(newCategory.CategoryName, StringComparison.InvariantCultureIgnoreCase))
                return false; // Duplicate category name found

            foreach (var existingExtension in cat.FileExtensions)
            {
                foreach (var newExtension in newCategory.FileExtensions)
                {
                    if (existingExtension.Equals(newExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return false; // Duplicate extension found
                    }
                }
            }
        }

        return true;
    }
}
