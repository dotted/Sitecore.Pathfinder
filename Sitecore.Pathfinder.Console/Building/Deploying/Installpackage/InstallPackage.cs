// � 2015 Sitecore Corporation A/S. All rights reserved.

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Building.Deploying.Installpackage
{
    using System.Collections.Generic;

    [Export(typeof(ITask))]
    public class InstallPackage : RequestTaskBase
    {
        public InstallPackage() : base("install-package")
        {
        }

        public override void Run(IBuildContext context)
        {
            if (context.Project.HasErrors)
            {
                context.Trace.TraceInformation(Texts.Package_contains_errors_and_will_not_be_deployed);
                context.IsAborted = true;
                return;
            }

            context.Trace.TraceInformation(Texts.Installing___);

            var packageId = Path.GetFileNameWithoutExtension(context.Configuration.Get("nuget:filename"));
            if (string.IsNullOrEmpty(packageId))
            {
                return;
            }

            var queryStringParameters = new Dictionary<string, string>
            {
                ["w"] = "0",
                ["rep"] = packageId
            };

            var url = MakeUrl(context, context.Configuration.GetString(Constants.Configuration.InstallUrl), queryStringParameters);
            if (!Request(context, url))
            {
                return;
            }

            foreach (var snapshot in context.Project.Items.SelectMany(i => i.Snapshots))
            {
                snapshot.SourceFile.IsModified = false;
            }
        }
    }
}