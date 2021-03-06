﻿// © 2015-2017 Sitecore Corporation A/S. All rights reserved.

using System.Composition;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Languages.Serialization
{
    [Export]
    public class SerializationTextSnapshot : TextSnapshot
    {
        [FactoryConstructor(typeof(ISnapshot))]
        [ImportingConstructor]
        public SerializationTextSnapshot([NotNull] ISnapshotService snapshotService) : base(snapshotService)
        {
        }
    }
}
