﻿// © 2015 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Parsing
{
    public interface IParser
    {
        double Priority { get; }

        bool CanParse([NotNull] IParseContext context);

        void Parse([NotNull] IParseContext context);
    }
}
