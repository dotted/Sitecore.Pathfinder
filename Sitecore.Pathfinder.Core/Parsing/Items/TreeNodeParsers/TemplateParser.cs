// � 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Parsing.Items.TreeNodeParsers
{
    [Export(typeof(ITextNodeParser))]
    public class TemplateParser : TextNodeParserBase
    {
        public TemplateParser() : base(Constants.TextNodeParsers.Templates)
        {
        }

        public override bool CanParse(ItemParseContext context, ITextNode textNode)
        {
            return textNode.Name == "Template";
        }

        public override void Parse(ItemParseContext context, ITextNode textNode)
        {
            var itemNameTextNode = textNode.GetAttributeTextNode("Item-Name");
            var itemName = itemNameTextNode?.Value ?? context.ParseContext.ItemName;
            var itemIdOrPath = context.ParentItemPath + "/" + itemName;
            var projectUniqueId = textNode.GetAttributeValue("Id", itemIdOrPath);

            var template = context.ParseContext.Factory.Template(context.ParseContext.Project, projectUniqueId, textNode, context.ParseContext.DatabaseName, itemName, itemIdOrPath);
            template.ItemName.Source = itemNameTextNode ?? new FileNameTextNode(itemName, textNode.Snapshot);
            template.BaseTemplates = textNode.GetAttributeValue("BaseTemplates", Constants.Templates.StandardTemplate);
            template.Icon.SetValue(textNode.GetAttributeTextNode("Icon"));
            template.ShortHelp = textNode.GetAttributeValue("ShortHelp");
            template.LongHelp = textNode.GetAttributeValue("LongHelp");

            template.References.AddRange(ParseReferences(context, template, textNode, template.BaseTemplates));

            var sectionsTextNode = context.Snapshot.GetJsonChildTextNode(textNode, "Sections");
            if (sectionsTextNode != null)
            {
                foreach (var sectionTreeNode in sectionsTextNode.ChildNodes)
                {
                    ParseSection(context, template, sectionTreeNode);
                }
            }

            context.ParseContext.Project.AddOrMerge(context.ParseContext, template);
        }

        protected virtual void ParseField([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] TemplateSection templateSection, [NotNull] ITextNode fieldTextNode, ref int nextSortOrder)
        {
            var fieldName = fieldTextNode.GetAttributeTextNode("Name");
            if (fieldName == null)
            {
                context.ParseContext.Trace.TraceError(Texts._Field__element_must_have_a__Name__attribute, fieldTextNode.Snapshot.SourceFile.FileName, fieldTextNode.Position);
                return;
            }

            var templateField = templateSection.Fields.FirstOrDefault(f => string.Compare(f.FieldName.Value, fieldName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateField == null)
            {
                templateField = context.ParseContext.Factory.TemplateField(template);
                templateSection.Fields.Add(templateField);
                templateField.FieldName.SetValue(fieldName);
            }

            int sortOrder;
            if (!int.TryParse(fieldTextNode.GetAttributeValue("SortOrder"), out sortOrder))
            {
                sortOrder = nextSortOrder;
            }

            nextSortOrder = sortOrder + 100;

            templateField.Type = fieldTextNode.GetAttributeValue("Type", "Single-Line Text");
            templateField.Shared = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Shared", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.Unversioned = string.Compare(fieldTextNode.GetAttributeValue("Sharing"), "Unversioned", StringComparison.OrdinalIgnoreCase) == 0;
            templateField.Source = fieldTextNode.GetAttributeValue("Source");
            templateField.ShortHelp = fieldTextNode.GetAttributeValue("ShortHelp");
            templateField.LongHelp = fieldTextNode.GetAttributeValue("LongHelp");
            templateField.SortOrder = sortOrder;
            templateField.StandardValue = fieldTextNode.GetAttributeValue("StandardValue");

            template.References.AddRange(ParseReferences(context, template, fieldTextNode, templateField.Source));
        }

        protected virtual void ParseSection([NotNull] ItemParseContext context, [NotNull] Template template, [NotNull] ITextNode templateSectionTextNode)
        {
            var sectionName = templateSectionTextNode.GetAttributeTextNode("Name");
            if (sectionName == null)
            {
                context.ParseContext.Trace.TraceError(Texts._Section__element_must_have_a__Name__attribute, templateSectionTextNode);
                return;
            }

            var templateSection = template.Sections.FirstOrDefault(s => string.Compare(s.SectionName.Value, sectionName.Value, StringComparison.OrdinalIgnoreCase) == 0);
            if (templateSection == null)
            {
                templateSection = context.ParseContext.Factory.TemplateSection(templateSectionTextNode);
                template.Sections.Add(templateSection);
                templateSection.SectionName.SetValue(sectionName);
            }

            templateSection.Icon = templateSectionTextNode.GetAttributeValue("Icon");

            var fieldsTextNode = context.Snapshot.GetJsonChildTextNode(templateSectionTextNode, "Fields");
            if (fieldsTextNode == null)
            {
                return;
            }

            var nextSortOrder = 0;
            foreach (var fieldTextNode in fieldsTextNode.ChildNodes)
            {
                ParseField(context, template, templateSection, fieldTextNode, ref nextSortOrder);
            }
        }
    }
}