﻿namespace MediaOrganizer.Core
{
    public sealed class FileMoverOptions
    {
        public bool RemoveSourceAfterMove { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file should be skipped, if a file in the destination with the same name already exists.
        /// </summary>
        public bool SkipIfFileExists { get; set; } = true;
    }
}
