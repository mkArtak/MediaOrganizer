using MediaOrganizer.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace MediaOrganizer.Storage.Local;

public sealed class PhysicalFileEnumerator : IFileEnumerator
{
    public PhysicalFileEnumerator()
    {
    }

    public IEnumerable<string> GetFiles(string root)
    {
        if (root == null)
        {
            throw new ArgumentNullException(nameof(root));
        }

        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories);
    }
}
