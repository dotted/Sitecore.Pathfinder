﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Parsing.References;

namespace Sitecore.Pathfinder.Compiling.Compilers
{
    public interface ICompileContext
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<ICompiler> Compilers { get; }

        [NotNull]
        ICompositionService CompositionService { get; }

        [NotNull]
        IFactoryService Factory { get; }

        [NotNull]
        ITraceService Trace { get; }

        [NotNull]
        IReferenceParserService ReferenceParser { get; }
    }
}