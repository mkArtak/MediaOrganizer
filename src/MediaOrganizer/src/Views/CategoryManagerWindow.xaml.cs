using MediaOrganizer.Core;
using MediaOrganizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MediaOrganizer.Views;

public partial class CategoryManagerWindow : Window
{
    public CategoryManagerWindow()
    {
        InitializeComponent();
    }

    private void AddCategory_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not FileOrganizerOptionsViewModel vm)
            return;

        var categories = vm.MediaCategories ?? new List<MediaCategory>();
        categories = new List<MediaCategory>(categories)
        {
            new MediaCategory { CategoryName = "New Category", CategoryRoot = string.Empty, FileExtensions = Array.Empty<string>() }
        };
        vm.MediaCategories = categories;
    }
}

