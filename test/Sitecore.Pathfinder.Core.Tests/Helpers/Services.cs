﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Microsoft.Framework.ConfigurationModel;
using Sitecore.Pathfinder.Checking;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Querying;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Helpers
{
    public class Services
    {
        [NotNull]
        public ICheckerService CheckerService { get; set; }

        [NotNull]
        public CompositionContainer CompositionService { get; private set; }

        [NotNull]
        public IConfigurationSourceRoot Configuration { get; private set; }

        [NotNull]
        public IConfigurationService ConfigurationService { get; private set; }

        [NotNull]
        public IFileSystemService FileSystem { get; private set; }

        [NotNull]
        public IParseService ParseService { get; private set; }

        [NotNull]
        public IProjectService ProjectService { get; private set; }

        [NotNull]
        public IQueryService QueryService { get; set; }

        [NotNull]
        public ISnapshotService SnapshotService { get; set; }

        [NotNull]
        public ITraceService Trace { get; private set; }

        [NotNull]
        public CompositionContainer RegisterCompositionService([NotNull] IConfiguration configuration)
        {
            var extensionCompiler = new CsharpCompiler();

            var extensionsDirectory = Path.Combine(configuration.Get(Constants.Configuration.ToolsDirectory), "files\\extensions");
            var projectExtensionsDirectory = PathHelper.Combine(configuration.Get(Constants.Configuration.ToolsDirectory), "..\\sitecore.project\\extensions");
            var directories = new[]
            {
                extensionsDirectory,
                projectExtensionsDirectory
            };

            var extensionsAssembly = extensionCompiler.GetExtensionsAssembly(extensionsDirectory, directories);
            if (extensionsAssembly == null)
            {
                // todo: not nice
                return null;
            }

            var conventions = new ExtensibilityConventions().GetConventions();

            var coreExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Constants).Assembly, conventions));
            var applicationExportProvider = new CatalogExportProvider(new AssemblyCatalog(typeof(Services).Assembly, conventions));
            var extensionsExportProvider = new CatalogExportProvider(new AssemblyCatalog(extensionsAssembly, conventions));

            // plugin directory exports takes precedence over application exports
            var compositionContainer = new CompositionContainer(extensionsExportProvider, applicationExportProvider, coreExportProvider);

            coreExportProvider.SourceProvider = compositionContainer;
            applicationExportProvider.SourceProvider = compositionContainer;
            extensionsExportProvider.SourceProvider = compositionContainer;

            // register the composition service itself for DI
            compositionContainer.ComposeExportedValue<ICompositionService>(compositionContainer);
            compositionContainer.ComposeExportedValue(configuration);

            return compositionContainer;
        }

        public void Start(string projectDirectory, [CanBeNull] Action mock = null)
        {
            var configuration = ConfigurationStartup.RegisterConfiguration(projectDirectory, ConfigurationOptions.Noninteractive);
            if (configuration == null)
            {
                throw new ConfigurationException("Configuration failed");
            }

            Configuration = configuration;
            CompositionService = RegisterCompositionService(Configuration);

            mock?.Invoke();

            Trace = CompositionService.Resolve<ITraceService>();
            FileSystem = CompositionService.Resolve<IFileSystemService>();
            ParseService = CompositionService.Resolve<IParseService>();
            ProjectService = CompositionService.Resolve<IProjectService>();
            ConfigurationService = CompositionService.Resolve<IConfigurationService>();
            SnapshotService = CompositionService.Resolve<ISnapshotService>();
            CheckerService = CompositionService.Resolve<ICheckerService>();
            QueryService = CompositionService.Resolve<IQueryService>();
        }
    }
}
