using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Exceptions;
using NLua;

namespace Atmosphere.Reverence.Seven
{
    internal class Data
    {
        private Data()
        {
        }

        public Data(Assembly assembly)
            : this()
        {
            Assembly = assembly;

            Lua = Seven.GetLua();

            LoadItems();
            LoadWeapons();
            LoadArmor();
            LoadAccessories();
            LoadMagicSpells();
            LoadSummonSpells();
            LoadEnemySkills();
        }





        private void LoadItems()
        {
            Items = new Dictionary<string, Item>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.items.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//items/item"))
            {
                Item i = new Item(node, Lua);
                
                Items.Add(i.ID, i);
            }
        }

        private void LoadWeapons()
        {    
            Weapons = new Dictionary<string, Weapon.WeaponData>();

            XmlDocument gamedata = Resource.GetXmlFromResource("data.weapons.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("./weapons/weapon"))
            {
                Weapon.WeaponData weapon = new Weapon.WeaponData(node, Lua);

                Weapons.Add(weapon.ID, weapon);
            }
        }


        

        private void LoadArmor()
        {
            Armor = new Dictionary<string, Armor.ArmorData>();

            XmlDocument gamedata = Resource.GetXmlFromResource("data.armour.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//armour/armor"))
            {                
                Armor.ArmorData armor = new Armor.ArmorData(node, Lua);
                
                Armor.Add(armor.ID, armor);
            }
        }
        
        
        private void LoadAccessories()
        {            
            Accessories = new Dictionary<string, Equipment.EquipmentData>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.accessories.xml", Assembly);

            foreach (XmlNode node in gamedata.SelectNodes("//accessories/accessory"))
            {
                Equipment.EquipmentData a = new Equipment.EquipmentData(node, Lua);
                
                Accessories.Add(a.ID, a);
            }
        }





        
        public void LoadMagicSpells()
        {
            MagicSpells = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.spells.magic.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {                
                Spell spell = new Spell(node, Lua);
                
                MagicSpells.Add(spell.Name, spell);
            } 
        }
        
        public void LoadSummonSpells()
        {
            SummonSpells = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.spells.summon.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {
                Spell spell = new Spell(node, Lua);
                
                SummonSpells.Add(spell.Name, spell);
            } 
        }
        
        public void LoadEnemySkills()
        {
            EnemySkills = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.spells.enemyskill.xml", Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {                
                Spell spell = new Spell(node, Lua);
                
                EnemySkills.Add(spell.Name, spell);
            } 
        }








        
        public Item GetItem(string id)
        {
            if (!Items.ContainsKey(id))
            {
                throw new GameDataException("Could not find item with id " + id);
            }
                    
            return Items[id];
        }
        
        public Weapon GetWeapon(string id)
        {                    
            if (!Weapons.ContainsKey(id))
            {
                throw new GameDataException("Could not find weapon with id " + id);
            }

            return new Weapon(Weapons[id]);
        }
        
        public Armor GetArmor(string id)
        {
            if (!Armor.ContainsKey(id))
            {
                throw new GameDataException("Could not find armor with id " + id);
            }

            return new Armor(Armor[id]);
        }
        
        public Accessory GetAccessory(string id)
        {
            if (!Accessories.ContainsKey(id))
            {
                throw new GameDataException("Could not find accessory with id " + id);
            }

            return new Accessory(Accessories[id]);
        }
        
        public Spell GetMagicSpell(string id)
        {
            if (!MagicSpells.ContainsKey(id))
            {
                throw new GameDataException("Could not find magic spell with id " + id);
            }
            
            return MagicSpells[id];
        }
        
        public IEnumerable<Spell> GetMagicSpells()
        {
            return MagicSpells.Select(x => x.Value).OrderBy(x => x.Order);
        }
        
        public Spell GetSummonSpell(string id)
        {
            if (!SummonSpells.ContainsKey(id))
            {
                throw new GameDataException("Could not find summon spell with id " + id);
            }
            
            return SummonSpells[id];
        }
        
        public IEnumerable<Spell> GetSummonSpells()
        {
            return SummonSpells.Select(x => x.Value).OrderBy(x => x.Order);
        }
        
        public Spell GetEnemySkill(string id)
        {
            if (!EnemySkills.ContainsKey(id))
            {
                throw new GameDataException("Could not find enemy skill with id " + id);
            }
            
            return EnemySkills[id];
        }

        public IEnumerable<Spell> GetEnemySkills()
        {
            return EnemySkills.Select(x => x.Value).OrderBy(x => x.Order);
        }

        
        public MateriaOrb GetMateria(string name, int ap)
        {
            MateriaOrb materia = null;
            
            if (name == EnemySkillMateria.NAME)
            {
                materia = new EnemySkillMateria(ap, this);
            }
            else if (name == "Master Magic")
            {
                materia = new MasterMateria(MateriaType.Magic, this);
            }
            else if (name == "Master Command")
            {
                materia = new MasterMateria(MateriaType.Command, this);
            }
            else if (name == "Master Summon")
            {
                materia = new MasterMateria(MateriaType.Summon, this);
            }
            else
            {
                XmlDocument gamedata = Resource.GetXmlFromResource("data.materia.xml", Assembly);
                
                XmlNode node = gamedata.SelectSingleNode(String.Format("//materia[name = '{0}']", name));
                
                if (node == null)
                {
                    throw new GameDataException("Could not find materia with name " + name);
                }
                
                materia = new MateriaOrb(node, Lua);
                materia.AddAP(ap);
            }
            
            return materia;
        }

        
        
        
        public IInventoryItem GetInventoryItem(string id, InventoryItemType type)
        {
            IInventoryItem item = null;
            
            switch (type)
            {
                case InventoryItemType.item:
                    
                    item = GetItem(id);
                    
                    break;
                    
                case InventoryItemType.weapon:
                    
                    item = GetWeapon(id);
                    
                    break;
                    
                case InventoryItemType.armor:
                    
                    item = GetArmor(id);
                    
                    break;
                    
                case InventoryItemType.accessory:
                    
                    item = GetAccessory(id);
                    
                    break;
            }
            
            return item;
        }




        public Assembly Assembly { get; private set; }
        
        public int MagicSpellCount { get { return MagicSpells.Count; } }
        
        public int SummonSpellCount { get { return SummonSpells.Count; } }
        
        public int EnemySkillCount { get { return EnemySkills.Count; } }

        private Dictionary<string, Item> Items { get; set; }

        private Dictionary<string, Weapon.WeaponData> Weapons { get; set; }

        private Dictionary<string, Armor.ArmorData> Armor { get; set; }

        private Dictionary<string, Equipment.EquipmentData> Accessories { get; set; }
        
        private Dictionary<string, Spell> MagicSpells { get; set; }
        
        private Dictionary<string, Spell> SummonSpells { get; set; }
        
        private Dictionary<string, Spell> EnemySkills { get; set; }

        private Lua Lua { get; set; }
    }
}

