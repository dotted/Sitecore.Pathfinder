﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.Snapshots
{
    [DebuggerDisplay("{GetType().Name}: FileName={AbsoluteFileName}")]
    public class SourceFile : ISourceFile
    {
        [CanBeNull]
        private string _fileNameWithoutExtensions;

        [FactoryConstructor]
        public SourceFile([NotNull] IConfiguration configuration, [NotNull] IFileSystem fileSystem, [NotNull] string absoluteFileName)
        {
            FileSystem = fileSystem;
            AbsoluteFileName = absoluteFileName;

            var projectDirectory = configuration.GetProjectDirectory();

            RelativeFileName = PathHelper.NormalizeFilePath(PathHelper.UnmapPath(projectDirectory, absoluteFileName)).TrimStart('\\');
            ProjectFileName = "~/" + PathHelper.NormalizeItemPath(PathHelper.UnmapPath(projectDirectory, PathHelper.GetDirectoryAndFileNameWithoutExtensions(absoluteFileName))).TrimStart('/');

            LastWriteTimeUtc = FileSystem.GetLastWriteTimeUtc(AbsoluteFileName);
        }

        public string AbsoluteFileName { get; }

        [NotNull]
        public static ISourceFile Empty { get; } = new EmptySourceFile();

        public DateTime LastWriteTimeUtc { get; }

        public string ProjectFileName { get; }

        public string RelativeFileName { get; }

        [NotNull]
        protected IFileSystem FileSystem { get; }

        public virtual string GetDirectoryAndFileNameWithoutExtensions() => _fileNameWithoutExtensions ?? (_fileNameWithoutExtensions = PathHelper.GetDirectoryAndFileNameWithoutExtensions(AbsoluteFileName));

        public virtual string GetFileNameWithoutExtensions()
        {
            var fileName = PathHelper.NormalizeItemPath(AbsoluteFileName);

            var s = fileName.LastIndexOf('/') + 1;
            var e = fileName.IndexOf('.', s);

            if (e < 0)
            {
                return fileName.Mid(s);
            }

            return fileName.Mid(s, e - s);
        }

        public virtual string[] ReadAsLines() => FileSystem.ReadAllLines(AbsoluteFileName);

        public virtual string[] ReadAsLines(IDictionary<string, string> tokens)
        {
            var lines = ReadAsLines();

            for (var index = 0; index < lines.Length; index++)
            {
                lines[index] = ReplaceTokens(lines[index], tokens);
            }

            return lines;
        }

        public virtual string ReadAsText() => FileSystem.ReadAllText(AbsoluteFileName);

        public virtual string ReadAsText(IDictionary<string, string> tokens) => ReplaceTokens(ReadAsText(), tokens);

        [NotNull]
        protected virtual string ReplaceTokens([NotNull] string text, [NotNull] IDictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                text = text.Replace("$" + token.Key, token.Value);
            }

            return text;
        }
    }
}
