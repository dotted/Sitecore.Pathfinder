// � 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Composition;
using System.Linq;
using Sitecore.Pathfinder.Compiling.Compilers;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Languages.Renderings;
using Sitecore.Pathfinder.Parsing.LayoutFiles;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Items;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Compiling.LayoutFileCompilers
{
    [Export(typeof(ILayoutFileCompiler)), Shared]
    public class CshtmlLayoutFileCompiler : LayoutFileCompilerBase
    {
        [ImportingConstructor]
        public CshtmlLayoutFileCompiler([NotNull] IFactory factory, [NotNull] ITraceService trace) : base(factory)
        {
            Trace = trace;
        }

        [NotNull]
        protected ITraceService Trace { get; }

        public override bool CanCompile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            if (item == null)
            {
                return false;
            }

            var sourcePropertyBag = (ISourcePropertyBag) item;
            if (!sourcePropertyBag.ContainsSourceProperty(LayoutFileItemParser.LayoutFile))
            {
                return false;
            }

            var extension = sourcePropertyBag.GetValue<string>(LayoutFileItemParser.LayoutFile);
            return !string.IsNullOrEmpty(extension) && string.Equals(PathHelper.GetExtension(extension), ".cshtml", StringComparison.OrdinalIgnoreCase);
        }

        public override void Compile(ICompileContext context, IProjectItem projectItem, SourceProperty<string> property)
        {
            var item = projectItem as Item;
            Assert.Cast(item, nameof(item));

            var value = property.GetValue().Trim();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var renderings = item.Project.Indexes.GetByFileName<Rendering>(value).Where(r => r.Database == item.Database).ToList();
            if (!renderings.Any())
            {
                Trace.TraceError(Msg.C1060, Texts.Rendering_reference_not_found, TraceHelper.GetTextNode(property), value);
                return;
            }

            if (renderings.Count > 1)
            {
                Trace.TraceError(Msg.C1062, Texts.Ambiguous_file_name, TraceHelper.GetTextNode(property), value);
                return;
            }

            var rendering = renderings.First();

            var renderingItemUri = rendering.RenderingItemUri;
            if (renderingItemUri == ProjectItemUri.Empty)
            {
                RetryCompilation(projectItem);
                return;
            }

            CreateLayout(context, item, renderingItemUri.Guid);
        }
    }
}
