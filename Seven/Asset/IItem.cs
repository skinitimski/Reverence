using System;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal interface IItem
    {
        ItemType Type { get; }
        string Name { get; }
        string ID { get; }
        string Description { get; }
    }
}

