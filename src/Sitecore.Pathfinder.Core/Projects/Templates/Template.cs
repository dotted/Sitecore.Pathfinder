// � 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Projects.Templates
{
    public class Template : ItemBase
    {
        [NotNull]
        public static readonly Template Empty = new Template(Projects.Project.Empty, new Guid("{7A3E077F-D985-453F-8773-348ADFEAF2FD}"), TextNode.Empty, string.Empty, string.Empty, string.Empty);

        public Template([NotNull] IProject project, Guid guid, [NotNull] ITextNode textNode, [NotNull] string databaseName, [NotNull] string itemName, [NotNull] string itemIdOrPath) : base(project, guid, textNode, databaseName, itemName, itemIdOrPath)
        {
        }

        [NotNull]
        public string BaseTemplates
        {
            get { return BaseTemplatesProperty.GetValue(); }
            set { BaseTemplatesProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> BaseTemplatesProperty { get; } = new SourceProperty<string>("BaseTemplates", string.Empty);

        [NotNull]
        public string LongHelp
        {
            get { return LongHelpProperty.GetValue(); }
            set { LongHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> LongHelpProperty { get; } = new SourceProperty<string>("LongHelp", string.Empty);

        [NotNull]
        [ItemNotNull]
        public IList<TemplateSection> Sections { get; } = new List<TemplateSection>();

        [NotNull]
        public string ShortHelp
        {
            get { return ShortHelpProperty.GetValue(); }
            set { ShortHelpProperty.SetValue(value); }
        }

        [NotNull]
        public SourceProperty<string> ShortHelpProperty { get; } = new SourceProperty<string>("ShortHelp", string.Empty);

        [CanBeNull]
        public Item StandardValuesItem { get; set; }

        public void Merge([NotNull] Template newTemplate)
        {
            Merge(newTemplate, true);
        }

        protected override void Merge(IProjectItem newProjectItem, bool overwrite)
        {
            base.Merge(newProjectItem, overwrite);

            var newTemplate = newProjectItem as Template;
            if (newTemplate == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(newTemplate.BaseTemplates))
            {
                // todo: join base templates
                BaseTemplatesProperty.SetValue(newTemplate.BaseTemplatesProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.Icon))
            {
                IconProperty.SetValue(newTemplate.IconProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.ShortHelp))
            {
                ShortHelpProperty.SetValue(newTemplate.ShortHelpProperty, SetValueOptions.DisableUpdates);
            }

            if (!string.IsNullOrEmpty(newTemplate.LongHelp))
            {
                LongHelpProperty.SetValue(newTemplate.LongHelpProperty, SetValueOptions.DisableUpdates);
            }

            foreach (var newSection in newTemplate.Sections)
            {
                var section = Sections.FirstOrDefault(s => string.Compare(s.SectionName, newSection.SectionName, StringComparison.OrdinalIgnoreCase) == 0);
                if (section == null)
                {
                    Sections.Add(newSection);
                    continue;
                }

                section.Merge(newSection, overwrite);
            }
        }
    }
}
