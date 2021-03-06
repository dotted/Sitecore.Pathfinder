﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NuGet.Packaging;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Media;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Projects.Templates;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Emitting.Emitters
{
    [Export(typeof(IProjectEmitter)), Shared]
    public class NugetProjectEmitter : ProjectEmitterBase
    {
        [ItemNotNull, NotNull]
        private readonly List<Item> _items = new List<Item>();

        [ItemNotNull, NotNull]
        private readonly List<Template> _templates = new List<Template>();

        [ImportingConstructor]
        public NugetProjectEmitter([NotNull] IConfiguration configuration, [NotNull] IFactory factory, [NotNull] ITraceService trace, [ItemNotNull, NotNull, ImportMany] IEnumerable<IEmitter> emitters, [NotNull] IFileSystem fileSystem) : base(configuration, trace, emitters)
        {
            Factory = factory;
            FileSystem = fileSystem;
            OutputDirectory = PathHelper.Combine(Configuration.GetProjectDirectory(), Configuration.GetString(Constants.Configuration.Output.Directory) + "\\content");
        }

        [NotNull]
        protected IFactory Factory { get; }

        [NotNull]
        public IFileSystem FileSystem { get; }

        [NotNull]
        public string OutputDirectory { get; }

        public override bool CanEmit(string format)
        {
            return string.Equals(format, "nuget", StringComparison.OrdinalIgnoreCase);
        }

        public override void Emit(IEmitContext context, IProject project)
        {
            base.Emit(context, project);

            var fileName = PathHelper.Combine(project.ProjectDirectory, Configuration.GetString(Constants.Configuration.Output.Directory) + "\\content.xml");
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };

                using (var writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartElement("content");

                    EmitReset(writer);
                    EmitTemplates(writer);
                    EmitItems(context, writer);
                    EmitPublish(writer);

                    writer.WriteEndElement();
                }
            }

            EmitNugetPackage(context);
        }

        protected virtual void EmitPublish([NotNull] XmlWriter writer)
        {
            var pairs = Configuration.GetSubKeys(Constants.Configuration.Output.Nuget.PublishDatabases).ToArray();
            if (!pairs.Any())
            {
                return;
            }

            writer.WriteStartElement("publish");

            foreach (var pair in pairs)
            {
                var value = Configuration.GetString(Constants.Configuration.Output.Nuget.PublishDatabases + ":" + pair.Key);
                writer.WriteAttributeString(pair.Key, value);
            }

            writer.WriteEndElement();
        }

        public virtual void EmitMediaFile([NotNull] IEmitContext context, [NotNull] MediaFile mediaFile)
        {
            var fileName = PathHelper.NormalizeFilePath(mediaFile.FilePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            var item = context.Project.Indexes.FindQualifiedItem<Item>(mediaFile.MediaItemUri);
            if (item == null)
            {
                Trace.TraceInformation(Msg.E1047, "No media item - skipping", mediaFile.Snapshot.SourceFile);
                return;
            }

            EmitItem(context, item);
        }

        public virtual void EmitFile([NotNull] IEmitContext context, [NotNull] Projects.Files.File file)
        {
            var fileName = PathHelper.NormalizeFilePath(file.FilePath);
            if (fileName.StartsWith("~\\"))
            {
                fileName = fileName.Mid(2);
            }

            Trace.TraceInformation(Msg.I1011, "Publishing", "~\\" + fileName);

            var forceUpdate = Configuration.GetBool(Constants.Configuration.BuildProject.ForceUpdate, true);
            var destinationFileName = PathHelper.Combine(OutputDirectory, fileName);

            FileSystem.CreateDirectoryFromFileName(destinationFileName);
            FileSystem.Copy(file.Snapshot.SourceFile.AbsoluteFileName, destinationFileName, forceUpdate);
        }

        public virtual void EmitItem([NotNull] IEmitContext context, [NotNull] Item item)
        {
            Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            _items.Add(item);
        }

        public virtual void EmitTemplate([NotNull] IEmitContext context, [NotNull] Template item)
        {
            Trace.TraceInformation(Msg.I1011, "Publishing", item.ItemIdOrPath);

            _templates.Add(item);
        }

        protected virtual void EmitNugetPackage([NotNull] IEmitContext context)
        {
            var packageFileName = Configuration.GetString(Constants.Configuration.Output.Nuget.FileName, "package");
            if (!packageFileName.EndsWith(".nupkg"))
            {
                packageFileName += ".nupkg";
            }

            var outputDirectory = PathHelper.Combine(context.Project.ProjectDirectory, Configuration.GetString(Constants.Configuration.Output.Directory));
            var fileName = Path.Combine(outputDirectory, packageFileName);

            var packageId = Path.GetFileNameWithoutExtension(fileName);
            var description = Configuration.GetString(Constants.Configuration.Description);
            if (string.IsNullOrEmpty(description))
            {
                description = "Generated by Sitecore Pathfinder";
            }

            var manifest = string.Empty;

            var nuspecFileName = Configuration.GetString(Constants.Configuration.Output.Nuget.NuspecFileName);
            if (!string.IsNullOrEmpty(nuspecFileName))
            {
                nuspecFileName = PathHelper.Combine(context.Project.ProjectDirectory, nuspecFileName);

                if (!FileSystem.FileExists(nuspecFileName))
                {
                    Trace.TraceError(Msg.E1044, "NuSpec file not found", nuspecFileName);
                    return;
                }

                manifest = FileSystem.ReadAllText(nuspecFileName);
            }

            if (string.IsNullOrEmpty(manifest))
            {
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<package xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\">");
                sb.AppendLine("    <metadata>");
                sb.AppendLine("        <id>" + packageId + "</id>");
                sb.AppendLine("        <title>" + Configuration.GetString(Constants.Configuration.Name, packageId) + "</title>");
                sb.AppendLine("        <version>" + Configuration.GetString(Constants.Configuration.Version, string.Empty) + "</version>");
                sb.AppendLine("        <authors>" + Configuration.GetString(Constants.Configuration.Author, string.Empty) + "</authors>");
                sb.AppendLine("        <owners>" + Configuration.GetString(Constants.Configuration.Publisher, string.Empty) + "</owners>");
                sb.AppendLine("        <requireLicenseAcceptance>false</requireLicenseAcceptance>");
                sb.AppendLine("        <description>" + description + "</description>");
                sb.AppendLine("    </metadata>");
                sb.AppendLine("</package>");

                manifest = sb.ToString();
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(manifest)))
            {
                var packageBuilder = new PackageBuilder(stream, outputDirectory);

                using (var nupkg = FileSystem.OpenWrite(fileName))
                {
                    packageBuilder.Save(nupkg);
                }
            }

            context.OutputFiles.Add(Factory.OutputFile(fileName));
        }

        protected virtual void EmitReset([NotNull] XmlWriter writer)
        {
            var pairs = Configuration.GetSubKeys(Constants.Configuration.Output.Nuget.ResetWebsite).ToArray();
            if (!pairs.Any())
            {
                return;
            }

            writer.WriteStartElement("reset");

            foreach (var pair in pairs)
            {
                var value = Configuration.GetString(Constants.Configuration.Output.Nuget.ResetWebsite + ":" + pair.Key);

                writer.WriteStartElement("item");

                writer.WriteAttributeString("database", value);
                writer.WriteAttributeString("id", pair.Key);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        protected virtual void EmitItems([NotNull] IEmitContext context, [NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("items");

            foreach (var item in _items.OrderBy(t => t.ItemPathLevel).ThenBy(t => t.ItemName))
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("id", item.Uri.Guid.Format());
                writer.WriteAttributeString("database", item.DatabaseName);
                writer.WriteAttributeString("name", item.ItemName);
                writer.WriteAttributeString("path", item.ItemIdOrPath);
                writer.WriteAttributeString("template", item.Template.Uri.Guid.Format());
                writer.WriteAttributeStringIf("icon", item.Icon);
                writer.WriteAttributeStringIf("sortorder", item.Sortorder.ToString());

                foreach (var field in item.Fields)
                {
                    writer.WriteStartElement("field");
                    writer.WriteAttributeString("id", field.FieldId.Format());
                    writer.WriteAttributeString("name", field.FieldName);

                    if (!field.TemplateField.Shared)
                    {
                        writer.WriteAttributeString("language", field.Language.LanguageName);
                    }

                    if (!field.TemplateField.Shared && !field.TemplateField.Unversioned)
                    {
                        writer.WriteAttributeString("version", field.Version.Number.ToString());
                    }

                    if (string.Equals(field.TemplateField.Type, "attachment", StringComparison.OrdinalIgnoreCase))
                    {
                        var mediaFile = context.Project.Indexes.FindQualifiedItem<MediaFile>(field.Value);
                        if (mediaFile == null)
                        {
                            Trace.TraceInformation(Msg.E1047, "No media item", item.SourceTextNode);
                        }
                        else
                        {
                            var bytes = File.ReadAllBytes(mediaFile.Snapshot.SourceFile.AbsoluteFileName);
                            var blob = Convert.ToBase64String(bytes);

                            writer.WriteAttributeString("blob", blob);
                            writer.WriteAttributeString("blobExtension", Path.GetExtension(mediaFile.Snapshot.SourceFile.AbsoluteFileName).TrimStart('.'));
                        }
                    }

                    writer.WriteValue(field.CompiledValue);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }



        protected virtual void EmitTemplates([NotNull] XmlWriter writer)
        {
            writer.WriteStartElement("templates");

            foreach (var template in _templates.OrderBy(t => t.ItemPathLevel).ThenBy(t => t.ItemName))
            {
                writer.WriteStartElement("template");
                writer.WriteAttributeString("id", template.Uri.Guid.Format());
                writer.WriteAttributeString("database", template.DatabaseName);
                writer.WriteAttributeString("name", template.ItemName);
                writer.WriteAttributeString("path", template.ItemIdOrPath);
                writer.WriteAttributeStringIf("longhelp", template.LongHelp);
                writer.WriteAttributeStringIf("shorthelp", template.ShortHelp);
                writer.WriteAttributeStringIf("basetemplates", template.BaseTemplates);
                writer.WriteAttributeStringIf("icon", template.Icon);

                if (template.StandardValuesItem != null && template.StandardValuesItem != Item.Empty)
                {
                    writer.WriteAttributeString("standardvaluesid", template.StandardValuesItem.Uri.Guid.Format());
                }

                foreach (var section in template.Sections)
                {
                    writer.WriteStartElement("section");
                    writer.WriteAttributeString("id", section.Uri.Guid.Format());
                    writer.WriteAttributeString("name", section.SectionName);
                    writer.WriteAttributeStringIf("icon", section.Icon);
                    writer.WriteAttributeStringIf("sortorder", section.Sortorder.ToString());

                    foreach (var field in section.Fields)
                    {
                        writer.WriteStartElement("field");
                        writer.WriteAttributeString("id", field.Uri.Guid.Format());
                        writer.WriteAttributeString("name", field.FieldName);
                        writer.WriteAttributeStringIf("icon", field.Icon);
                        writer.WriteAttributeStringIf("longhelp", field.LongHelp);
                        writer.WriteAttributeStringIf("shorthelp", field.ShortHelp);
                        writer.WriteAttributeStringIf("source", field.Source);
                        writer.WriteAttributeStringIf("type", field.Type);
                        writer.WriteAttributeStringIf("sharing", field.Shared ? "shared" : field.Unversioned ? "unversioned" : string.Empty);
                        writer.WriteAttributeStringIf("sortorder", field.Sortorder.ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        protected override void EmitProjectItems(IEmitContext context, IEnumerable<IProjectItem> projectItems, List<IEmitter> emitters, ICollection<Tuple<IProjectItem, Exception>> retries)
        {
            var unemittedItems = new List<IProjectItem>(projectItems);

            foreach (var projectItem in projectItems)
            {
                if (projectItem is MediaFile mediaFile)
                {
                    EmitMediaFile(context, mediaFile);
                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is Projects.Files.File file)
                {
                    EmitFile(context, file);
                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is Item item)
                {
                    if (item.IsEmittable)
                    {
                        EmitItem(context, item);
                    }

                    unemittedItems.Remove(projectItem);
                }
                else if (projectItem is Template template)
                {
                    if (template.IsEmittable)
                    {
                        EmitTemplate(context, template);
                    }

                    unemittedItems.Remove(projectItem);
                }
            }

            base.EmitProjectItems(context, unemittedItems, emitters, retries);
        }
    }
}
