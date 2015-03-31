using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class MasterMagicMateria : MagicMateria
    {        
        internal static readonly MateriaBase.MateriaRecord MasterMagic;

        static MasterMagicMateria()
        {
            MasterMagic = new MateriaBase.MateriaRecord("Master Magic", "Equips all magical spells", "All Magic");
        }
        /*
         * 
    <materia>
        <name>Restore</name>
        <desc>[Restorative] elemental magic</desc>
        <hpp>-2</hpp>
        <mpp>2</mpp>
        <str>-1</str>
        <vit>0</vit>
        <dex>0</dex>
        <mag>1</mag>
        <spr>0</spr>
        <lck>0</lck>
        <type>Magic</type>
        <tiers>
            <tier>0</tier>
            <tier>2500</tier>
            <tier>17000</tier>
            <tier>25000</tier>
            <tier>40000</tier>
        </tiers>
        <order>75</order>
        <abilities>Cure,Cure2,Regen,Cure3</abilities>
    </materia>
    
         */

        public MasterMagicMateria() : base(MasterMagic.ID, 0) { }


        public override List<Spell> GetSpells
        {
            get
            {
                List<Spell> sp = new List<Spell>();
                
                foreach (Spell s in Spell.GetMagicSpells())
                {
                    sp.Add(s);
                }
                
                return sp;
            }
        }
    }
}

