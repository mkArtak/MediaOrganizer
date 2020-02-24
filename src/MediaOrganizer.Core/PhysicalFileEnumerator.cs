using System;
using System.Collections.Generic;
using System.IO;

namespace MediaOrganizer.Core
{
    public sealed class PhysicalFileEnumerator : IFileEnumerator
    {
        public PhysicalFileEnumerator()
        {
        }

        public IEnumerable<string> GetFilesAsync(string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories);
        }
    }
}
