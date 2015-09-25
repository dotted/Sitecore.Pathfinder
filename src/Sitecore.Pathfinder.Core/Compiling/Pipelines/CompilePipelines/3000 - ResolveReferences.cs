// � 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Extensibility.Pipelines;

namespace Sitecore.Pathfinder.Compiling.Pipelines.CompilePipelines
{
    public class ResolveReferences : PipelineProcessorBase<CompilePipeline>
    {
        public ResolveReferences() : base(3000)
        {
        }

        protected override void Process(CompilePipeline pipeline)
        {
            foreach (var projectItem in pipeline.Project.Items)
            {
                foreach (var reference in projectItem.References)
                {
                    reference.Resolve();
                }
            }
        }
    }
}
