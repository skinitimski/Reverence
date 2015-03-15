using System;

using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal interface ISlotHolder
    {
        MateriaBase[] Slots { get; }
        int Links { get; }
    }
}

