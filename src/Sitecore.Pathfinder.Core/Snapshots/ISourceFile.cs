﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Snapshots
{
    public interface ISourceFile
    {
        [NotNull]
        string AbsoluteFileName { get; }

        DateTime LastWriteTimeUtc { get; }

        [NotNull]
        string ProjectFileName { get; }

        [NotNull]
        string RelativeFileName { get; }

        [NotNull]
        string GetDirectoryAndFileNameWithoutExtensions();

        [NotNull]
        string GetFileNameWithoutExtensions();

        [NotNull, ItemNotNull]
        string[] ReadAsLines();

        [NotNull, ItemNotNull]
        string[] ReadAsLines([NotNull] IDictionary<string, string> tokens);

        [NotNull]
        string ReadAsText();

        [NotNull]
        string ReadAsText([NotNull] IDictionary<string, string> tokens);
    }
}
