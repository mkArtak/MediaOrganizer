using MediaOrganizer.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MediaOrganizer.ViewModels;

public class MediaCategoryEditorViewModel : BindableBase
{
    private readonly MediaCategory _mediaCategory;

    private string _categoryName;
    private string _categoryRoot;
    private ObservableCollection<string> _extensions = new();

    private readonly Action<MediaCategory> _onSave;
    private readonly Action<MediaCategory> _onDelete;
    private readonly Predicate<MediaCategory> _validator;

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
                RaisePropertyChanged(nameof(ExtensionsText));
        }
    }

    public string ExtensionsText
    {
        get => string.Join(", ", Extensions);
        set
        {
            var cleaned = value?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToList() ?? new();

            Extensions = new ObservableCollection<string>(cleaned);
        }
    }

    public MediaCategoryEditorViewModel() : this(new MediaCategory(), cat => true, cat => { }, catToDelete => { })
    {
    }

    public MediaCategoryEditorViewModel(MediaCategory category, Predicate<MediaCategory> validator, Action<MediaCategory> onSave, Action<MediaCategory> onDelete)
    {
        _mediaCategory = category ?? throw new ArgumentNullException(nameof(category));

        _categoryName = category.CategoryName ?? string.Empty;
        _extensions = category.FileExtensions == null ? new ObservableCollection<string>() : new ObservableCollection<string>(category.FileExtensions);

        _onSave = onSave ?? throw new ArgumentNullException(nameof(onSave));
        _onDelete = onDelete ?? throw new ArgumentNullException(nameof(onDelete));
        _validator = validator ?? (_ => true);

        SaveCommand = new DelegateCommand(Save, CanSave)
            .ObservesProperty(() => CategoryName)
            .ObservesProperty(() => ExtensionsText);

        DeleteCommand = new DelegateCommand(Delete)
            .ObservesCanExecute(() => ShowDeleteButton);
    }

    public bool ShowDeleteButton => !string.IsNullOrWhiteSpace(CategoryName);

    public ICommand SaveCommand { get; }

    public ICommand DeleteCommand { get; }

    private void Save()
    {
        MediaCategory category = GetCategory();

        if (_validator(category))
        {
            _onSave(category);
        }
    }

    private void Delete()
    {
        _onDelete(this.GetCategory());
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(CategoryName) && Extensions.Any();
    }

    private MediaCategory GetCategory() => new MediaCategory
    {
        CategoryName = CategoryName,
        CategoryRoot = CategoryRoot,
        FileExtensions = Extensions.ToArray()
    };
}