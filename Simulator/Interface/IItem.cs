using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public interface IItem
    {
        ItemType Type { get; }
        string Name { get; }
        string ID { get; }
        string Description { get; }
    }
}
