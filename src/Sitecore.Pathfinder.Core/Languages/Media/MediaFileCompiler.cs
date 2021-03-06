﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using System.IO;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;
using Sitecore.Pathfinder.Text;

namespace Sitecore.Pathfinder.Languages.Media
{
    [Export(typeof(ICompiler)), Shared]
    public class MediaFileCompiler : CompilerBase
    {
        [ImportingConstructor]
        public MediaFileCompiler([NotNull] IConfiguration configuration, [NotNull] IFactory factory) : base(1000)
        {
            Configuration = configuration;
            Factory = factory;
        }

        [NotNull]
        protected IConfiguration Configuration { get; }

        [NotNull]
        protected IFactory Factory { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem) => projectItem is MediaFile;

        public override void Compile(ICompileContext context, IProjectItem projectItem)
        {
            var mediaFile = projectItem as MediaFile;
            Assert.Cast(mediaFile, nameof(mediaFile));

            var extension = Path.GetExtension(mediaFile.Snapshot.SourceFile.AbsoluteFileName).TrimStart('.').ToLowerInvariant();

            var templateIdOrPath = Configuration.GetString(Constants.Configuration.BuildProject.MediaTemplate + ":" + extension, "/sitecore/templates/System/Media/Unversioned/File");

            var project = context.Project;
            var snapshot = mediaFile.Snapshot;
            var guid = StringHelper.GetGuid(project, mediaFile.ItemPath);

            var item = Factory.Item(mediaFile.Database, guid, mediaFile.ItemName, mediaFile.ItemPath, string.Empty).With(Factory.SnapshotTextNode(snapshot));
            item.IsEmittable = false;
            item.OverwriteWhenMerging = true;
            item.MergingMatch = MergingMatch.MatchUsingSourceFile;

            item.ItemNameProperty.AddSourceTextNode(new FileNameTextNode(mediaFile.ItemName, snapshot));
            item.TemplateIdOrPathProperty.SetValue(templateIdOrPath);

            var fileInfo = new FileInfo(mediaFile.Snapshot.SourceFile.AbsoluteFileName);

            item.Fields.Add(Factory.Field(item, "Extension", mediaFile.Extension.Mid(1)).With(item.SourceTextNode));
            item.Fields.Add(Factory.Field(item, "Size", fileInfo.Length.ToString()).With(item.SourceTextNode));
            item.Fields.Add(Factory.Field(item, "Blob", mediaFile.Uri.Guid.Format()).With(item.SourceTextNode));

            foreach (var language in item.Database.Languages)
            {
                var altField = Factory.Field(item, "Alt", mediaFile.ItemName).With(item.SourceTextNode);
                altField.Language = language;
                altField.Version = Factory.Version(1);
                item.Fields.Add(altField);
            }

            var addedItem = project.AddOrMerge(item);
            mediaFile.MediaItemUri = addedItem.Uri;
        }
    }
}
