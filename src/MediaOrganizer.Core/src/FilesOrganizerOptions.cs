using System;
using System.Collections.Generic;

namespace MediaOrganizer.Core
{
    public class FilesOrganizerOptions
    {
        public static readonly string DefaultDestinationPattern = "{Year}/{MonthName}/{Year}-{Month}-{Day}";

        public static readonly string WellKnownMediaCategoriesKey = "configuredMediaCategories";

        public List<MediaCategory> MediaCategories { get; } = new List<MediaCategory>();

        public string SourceRoot { get; set; }

        public string DestinationRoot { get; set; }

        public bool RemoveSource { get; set; }

        public bool SkipExistingFiles { get; set; }

        public bool DeleteEmptyFolders { get; set; }

        public string DestinationPattern { get; set; } = DefaultDestinationPattern;

        public override string ToString()
        {
            return $"SourceRoot: {SourceRoot}{Environment.NewLine}DestinationRoot: {DestinationRoot}{Environment.NewLine}RemoveSource: {RemoveSource}{Environment.NewLine}SkipExistingFiles: {SkipExistingFiles}{Environment.NewLine}DeleteEmptyFolders: {DeleteEmptyFolders}";
        }
    }
}
