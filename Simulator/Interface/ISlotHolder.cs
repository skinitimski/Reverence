using System;

namespace Atmosphere.BattleSimulator
{
    public interface ISlotHolder
    {
        Materia[] Slots { get; }
        int Links { get; }
    }
}