﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Documents;
using Sitecore.Pathfinder.Projects.Items;

namespace Sitecore.Pathfinder.Projects.References
{
    public class LayoutRenderingReference : Reference
    {
        public LayoutRenderingReference([NotNull] IProjectItem owner, [NotNull] Attribute<string> sourceAttribute, [NotNull] string targetQualifiedName) : base(owner, sourceAttribute, targetQualifiedName)
        {
        }

        public override IProjectItem Resolve()
        {
            foreach (var projectItem in Owner.Project.Items.Where(i => string.Compare(i.ShortName, TargetQualifiedName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                var item = projectItem as Item;
                if (item == null)
                {
                    continue;
                }

                var templateIdOrPath = item.TemplateIdOrPath.Value;
                //if (templateIdOrPath != Constants.Templates.ViewRendering && templateIdOrPath != Constants.Templates.Sublayout)
                //{
                //    continue;
                //}

                IsResolved = true;
                IsValid = true;
                return projectItem;
            }

            return null;
        }
    }
}