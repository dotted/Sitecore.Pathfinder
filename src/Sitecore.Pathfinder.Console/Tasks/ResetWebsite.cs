// � 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    public class ResetWebsite : WebBuildTaskBase
    {
        public ResetWebsite() : base("reset-website")
        {
        }

        public override void Run(IBuildContext context)
        {
            context.Trace.TraceInformation(Msg.M1009, Texts.Resetting_website___);

            var webRequest = GetWebRequest(context).AsTask("ResetWebsite");

            Post(context, webRequest);
        }

        public override void WriteHelp(HelpWriter helpWriter)
        {
            helpWriter.Summary.Write("Resets the website.");
            helpWriter.Remarks.WriteLine("Deletes items and files from the website.");
        }
    }
}