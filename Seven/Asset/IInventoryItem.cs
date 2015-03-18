using System;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal interface IInventoryItem
    {
        string Name { get; }
        string ID { get; }
        string Description { get; }

        bool CanUseInField { get; }
    }
}

