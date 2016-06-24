﻿// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.IO;
using System.Text;
using NuGet;
using Sitecore.Pathfinder.IO;

namespace Sitecore.Pathfinder.NuGet
{
    public class NuGetPackageBuilder
    {
        public NuGetPackageBuilder([NotNull] IFileSystemService fileSystem)
        {
            FileSystem = fileSystem;
        }

        [NotNull]
        protected IFileSystemService FileSystem { get; }

        public virtual void CreateNugetPackage([NotNull] string tempDirectory, [NotNull] string fileName, [NotNull] string sourceFileName)
        {
            var packageId = Path.GetFileNameWithoutExtension(fileName);

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<package xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\">");
            sb.AppendLine("    <metadata>");
            sb.AppendLine("        <id>" + packageId + "</id>");
            sb.AppendLine("        <title>" + packageId + "</title>");
            sb.AppendLine("        <version>1.0.0</version>");
            sb.AppendLine("        <authors>Sitecore Pathfinder</authors>");
            sb.AppendLine("        <requireLicenseAcceptance>false</requireLicenseAcceptance>");
            sb.AppendLine("        <description>Generated by Sitecore Pathfinder</description>");
            sb.AppendLine("    </metadata>");
            sb.AppendLine("    <files>");
            sb.AppendLine("        <file src=\"" + sourceFileName + "\" target=\"project\\sitecore.project\\build\\exports.xml\" />");
            sb.AppendLine("    </files>");
            sb.AppendLine("</package>");

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
            {
                var packageBuilder = new PackageBuilder(stream, tempDirectory);

                using (var nupkg = FileSystem.OpenWrite(fileName))
                {
                    packageBuilder.Save(nupkg);
                }
            }
        }
    }
}
