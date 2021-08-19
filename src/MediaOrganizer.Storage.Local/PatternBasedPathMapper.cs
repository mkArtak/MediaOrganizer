using MediaOrganizer.Core;
using System;

namespace MediaOrganizer.Storage.Local
{
    public class PatternBasedPathMapper : Mapper
    {
        private static readonly string[] monthNames = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private string pattern;

        public PatternBasedPathMapper(FilesOrganizerOptions options) : base(options)
        {
            this.pattern = options.DestinationPattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        protected override string GetDestinationFolder(DateTime dateTaken)
        {
            return pattern
                .Replace("{Year}", dateTaken.Year.ToString(), StringComparison.OrdinalIgnoreCase)
                .Replace("{Month}", dateTaken.Month.ToString("D2"), StringComparison.OrdinalIgnoreCase)
                .Replace("{MonthName}", monthNames[dateTaken.Month - 1], StringComparison.OrdinalIgnoreCase)
                .Replace("{Day}", dateTaken.Day.ToString("D2"), StringComparison.OrdinalIgnoreCase);
        }
    }
}
