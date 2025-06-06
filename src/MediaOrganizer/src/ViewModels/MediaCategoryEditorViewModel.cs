using MediaOrganizer.Core;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace MediaOrganizer.ViewModels;

public delegate bool MediaCategoryValidateEventHandler(MediaCategory existingCategory, MediaCategory newCategory, out Dictionary<string, string> errors);

public delegate void SaveMediaCategoryEventHandler(MediaCategory existingCategory, MediaCategory newCategory);

public class MediaCategoryEditorViewModel : ValidatableObjectBase
{
    private static readonly HashSet<char> invalidCharacters = new HashSet<char>(Path.GetInvalidFileNameChars());

    private readonly MediaCategory _mediaCategory;

    private string _categoryName;
    private string _categoryRoot;
    private ObservableCollection<string> _extensions = new();
    private readonly SaveMediaCategoryEventHandler _onSave;
    private readonly Action<MediaCategory> _onDelete;
    private readonly MediaCategoryValidateEventHandler _validateCategory;

    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }

    public string CategoryRoot
    {
        get => _categoryRoot;
        set => SetProperty(ref _categoryRoot, value);
    }

    public ObservableCollection<string> Extensions
    {
        get => _extensions;
        set
        {
            if (SetProperty(ref _extensions, value))
            {
                RaisePropertyChanged(nameof(ExtensionsText));
                ValidateExtensions();
            }
        }
    }

    public string ExtensionsText
    {
        get => string.Join(", ", Extensions);
        set
        {
            var cleaned = value?
                .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToList() ?? new();
            Extensions = new ObservableCollection<string>(cleaned);
        }
    }

    private static bool NoOpValidator(MediaCategory old, MediaCategory @new, out Dictionary<string, string> errors)
    {
        errors = null;
        return true;
    }

    public MediaCategoryEditorViewModel() : this(new MediaCategory(), null, (old, @new) => { }, catToDelete => { })
    {
    }

    public MediaCategoryEditorViewModel(MediaCategory category, MediaCategoryValidateEventHandler validator, SaveMediaCategoryEventHandler onSave, Action<MediaCategory> onDelete)
    {
        _mediaCategory = category;

        _categoryName = _mediaCategory?.CategoryName ?? string.Empty;
        _categoryRoot = _mediaCategory?.CategoryRoot ?? string.Empty;
        _extensions = _mediaCategory?.FileExtensions is null ? new ObservableCollection<string>() : new ObservableCollection<string>(category.FileExtensions);

        _onSave = onSave ?? throw new ArgumentNullException(nameof(onSave));
        _onDelete = onDelete ?? throw new ArgumentNullException(nameof(onDelete));
        _validateCategory = validator ?? NoOpValidator;

        SaveCommand = new DelegateCommand(Save);

        DeleteCommand = new DelegateCommand(Delete)
            .ObservesCanExecute(() => ShowDeleteButton);
    }

    public bool ShowDeleteButton => _mediaCategory is not null;

    public ICommand SaveCommand { get; }

    public ICommand DeleteCommand { get; }

    private void Save()
    {
        if (ValidateAll())
        {
            MediaCategory category = GetCategory();

            if (_validateCategory(_mediaCategory, category, out var errors))
                _onSave(_mediaCategory, category);
            else
                ReportExternalErrors(errors);
        }
    }

    private void ReportExternalErrors(Dictionary<string, string> errors)
    {
        foreach (var error in errors)
        {
            var key = error.Key;
            if (key == nameof(MediaCategory.FileExtensions))
                key = nameof(ExtensionsText);

            ClearErrors(key);
            AddError(key, error.Value);
        }
    }

    private bool ValidateAll()
    {
        ValidateCategoryName();
        ValidateCategoryRoot();
        ValidateExtensions();

        return !HasErrors;
    }

    private void ValidateCategoryName()
    {
        ClearErrors(nameof(CategoryName));

        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            AddError(nameof(CategoryName), "Category name cannot be empty");
        }
    }

    private void ValidateCategoryRoot()
    {
        ClearErrors(nameof(CategoryRoot));

        if (!UsesValidCharacters(CategoryRoot))
            AddError(nameof(CategoryRoot), $"Category root contains one or more invalid characters");
    }

    private void ValidateExtensions()
    {
        ClearErrors(nameof(ExtensionsText));

        if (Extensions.Count == 0)
            AddError(nameof(ExtensionsText), "At least one file extension is required.");
        else
        {
            foreach (var extension in Extensions)
                if (!UsesValidCharacters(extension))
                    AddError(nameof(ExtensionsText), $"Extension {extension} contains one or more invalid characters");
        }
    }

    private bool UsesValidCharacters(string path)
    {
        foreach (char c in path)
        {
            if (invalidCharacters.Contains(c))
                return false;
        }

        return true;
    }

    private void Delete()
    {
        _onDelete(this.GetCategory());
    }

    private MediaCategory GetCategory() => new MediaCategory
    {
        CategoryName = CategoryName,
        CategoryRoot = CategoryRoot,
        FileExtensions = Extensions.ToArray()
    };
}