﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;

namespace Sitecore.Pathfinder.Projects
{
    public class ProjectOptions
    {
        [NotNull]
        public static readonly ProjectOptions Empty = new ProjectOptions(string.Empty);

        [FactoryConstructor]
        public ProjectOptions([NotNull] string databaseName)
        {
            DatabaseName = databaseName;
        }

        [NotNull]
        public string DatabaseName { get; }

        [NotNull]
        public IDictionary<string, string> Tokens { get; } = new Dictionary<string, string>();

        public virtual void LoadTokens([NotNull] IConfiguration configuration)
        {
            foreach (var pair in configuration.GetSubKeys(Constants.Configuration.SearchAndReplaceTokens))
            {
                var value = configuration.GetString(Constants.Configuration.SearchAndReplaceTokens + ":" + pair.Key);
                Tokens[pair.Key] = value;
            }
        }
    }
}
