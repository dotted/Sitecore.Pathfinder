﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder
{
    public static class Constants
    {
        public const string ExtensionsAssemblyFileName = "Sitecore.Pathfinder.Checker.dll";

        public const string RenderingIdsFastQuery = "@@templateid='{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}' or @@templateid='{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}' or @@templateid='{86776923-ECA5-4310-8DC0-AE65FE88D078}' or @@templateid='{39587D7D-F06D-4CB4-A25E-AA7D847EDDD0}' or @@templateid='{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}' or @@templateid='{83E993C5-C0FC-4472-86A9-2F6CFED694E4}' or @@templateid='{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}' or @@templateid='{F1F1D639-4F54-40C2-8BE0-81266B392CEB}'";

        [NotNull]
        public static readonly char[] Comma =
        {
            ','
        };

        [NotNull]
        public static readonly char[] Pipe =
        {
            '|'
        };

        [NotNull]
        public static readonly char[] Semicolon =
        {
            ';'
        };

        [NotNull]
        public static readonly char[] Slash =
        {
            '/'
        };

        [NotNull]
        public static readonly char[] Space =
        {
            ' '
        };

        public static class Configuration
        {
            public const string BuildProject = "build-project:tasks";

            public const string CheckBinFileVersion = "install-package:check-bin-file-version";

            public const string CodeGen = "codegen";

            public const string ContentFiles = "build-project:content-files";

            public const string Database = "database";

            public const string DataDirectoryName = "copy-package:data-directory-name";

            public const string Debug = "system:debug";

            public const string ExternalDirectory = "build-project:external-directory";

            public const string Files = "build-project:files";

            public const string HostName = "host-name";

            public const string IgnoreDirectories = "build-project:ignore-directories";

            public const string IgnoreFileNames = "build-project:ignore-filenames";

            public const string InstallUrl = "install-package:install-url";

            public const string LocalTestDirectory = "run-unittests:local-test-directory";

            public const string PackageDirectory = "copy-package:package-directory";

            public const string PackagesDirectory = "copy-dependencies:packages-directory";

            public const string PackNugetFileName = "pack-nuget:filename";

            public const string Password = "password";

            public const string Pathfinder = "Pathfinder";

            public const string ProjectConfigFileName = "project";

            public const string ProjectDirectory = "projectdirectory";

            public const string ProjectUniqueId = "project-unique-id";

            public const string PublishUrl = "publish-databases:publish-url";

            public const string RemapFileDirectories = "remap-file-directories";

            public const string SolutionDirectory = "solutiondirectory";

            public const string StandardTemplateFields = "standard-template-fields";

            public const string SystemConfigFileName = "system:config";

            public const string ToolsDirectory = "system:toolspath";

            public const string UninstallDirectory = "install-package:uninstall-directory";

            public const string UpdateResourcesUrl = "sync-website:sync-url";

            public const string UserName = "user-name";

            public const string WebsiteDirectoryName = "run-unittests:website-directory-name";

            public const string WebTestRunnerName = "run-unittests:web-test-runner-name";

            public const string WebTestRunnerUrl = "run-unittests:web-test-runner-url";

            public const string Wwwroot = "wwwroot";

            public static class ValidateWebsite
            {
                public const string InactiveValidations = "validate-website:inactive-validations";

                public const string Languages = "validate-website:languages";

                public const string ProcessSiteValidation = "validate-website:process-site-validations";

                public const string RootItemPath = "validate-website:root-item-path";

                public const string Site = "validate-website:site";

                public const string Timeout = "validate-website:timeout";

                public const string Url = "validate-website:url";
            }
        }

        public static class Emitters
        {
            public const double BinFiles = 9999;

            public const double ContentFiles = 4000;

            public const double Items = 2000;

            public const double Layouts = 2500;

            public const double MediaFiles = 1500;

            public const double Templates = 1000;
        }

        public static class FieldResolvers
        {
            public const double High = 500;

            public const double Low = 2000;

            public const double Normal = 1000;
        }

        public static class Fields
        {
            public const string InsertOptionsFieldId = "{1172F251-DAD4-4EFB-A329-0C63500E4F1E}";
        }

        public static class Parsers
        {
            public const double BinFiles = 9999;

            public const double ContentFiles = 9000;

            public const double Items = 3000;

            public const double Media = 2000;

            public const double Renderings = 5000;

            public const double System = 100;

            public const double Templates = 1000;
        }

        public static class Templates
        {
            public const string Layout = "{3A45A723-64EE-4919-9D41-02FD40FD1466}";

            public const string StandardTemplate = "{1930BBEB-7805-471A-A3BE-4858AC7CF696}";

            public const string Sublayout = "{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}";

            public const string ViewRendering = "{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}";
        }

        public static class TextNodeParsers
        {
            public const double Content = 9999;

            public const double Items = 1000;

            public const double Layouts = 1000;

            public const double Templates = 1000;
        }
    }
}