using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Atmosphere.BattleSimulator
{
    public struct Summon
    {
        public const int TOTAL_SUMMONS = 16;

        private string _name;
        private Spell[] _spell;
        private int _allCount;
        private int _qmagicCount;
        private List<AddedAbility> _addedAbilities;

        public Summon(string name, SummonMateria s)
        {
            _name = name;
            _spell = s.GetSpells;
            _allCount = 0;
            _qmagicCount = 0;
            _addedAbilities = new List<AddedAbility>();
        }

        public void AddAbility(SupportMateria s)
        {
            _addedAbilities.Add(s.GetAbility());
            if (s.ID == "all")
                _allCount = s.Level + 1;
            if (s.ID == "quadramagic")
                if (_name == "Knights of Round")
                    _addedAbilities.Remove(s.GetAbility());
                else
                    _qmagicCount = s.Level + 1;
        }

        public static int Compare(Summon left, Summon right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }

        public Spell Spell { get { return _spell[0]; } }
        public string Name { get { return _name; } }
        public int AllCount { get { return _allCount; } }
        public int QMagicCount { get { return _qmagicCount; } }
        public int Order { get { return _spell[0].Order; } }
        public string ID { get { return _name == null ? "" : Globals.CreateID(Name); } }
        public List<AddedAbility> AddedAbility { get { return _addedAbilities; } }
    }



    public struct MagicSpell
    {
        public const int TOTAL_SPELLS = 54;

        private Spell _spell;
        private int _allCount;
        private int _qmagicCount;
        private int _mpTurboFactor;
        private List<AddedAbility> _addedAbilities;

        public MagicSpell(Spell s)
        {
            _spell = s;
            _allCount = 0;
            _qmagicCount = 0;
            _mpTurboFactor = 0;
            _addedAbilities = new List<AddedAbility>();
        }

        public void AddAbility(SupportMateria s)
        {
            _addedAbilities.Add(s.GetAbility());
            if (s.ID == "all")
                _allCount = Math.Min(s.Level + 1, 5);
            if (s.ID == "quadramagic")
                _qmagicCount = Math.Min(s.Level + 1, 5);
            if (s.ID == "mpturbo")
                _mpTurboFactor = Math.Min(s.Level + 1, 5);
        }

        public static int Compare(MagicSpell left, MagicSpell right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }

        public Spell Spell { get { return _spell; } }
        public int AllCount { get { return _allCount; } }
        public int QMagicCount { get { return _qmagicCount; } }
        public int MPTurboFactor { get { return _mpTurboFactor; } }
        public string ID { get { return _spell.ID; } }
        public List<AddedAbility> AddedAbility { get { return _addedAbilities; } }
    }



    public struct Spell
    {


        #region Member Data

        public const string DATAFILE = @"Data\spells.xml";

        private string _name;
        private string _desc;
        private int _cost;
        private int _matp;
        private int _order;
        private Element[] _element;

        private static Dictionary<string, Spell> _table;

        #endregion Member Data


        public static void Init()
        {
            _table = new Dictionary<string, Spell>();

            XmlDocument gamedata = new XmlDocument();
            gamedata.Load(DATAFILE);

            foreach (XmlNode node in gamedata.SelectSingleNode("//spells").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                string name = xml.SelectSingleNode("//name").InnerText;
                string id = Globals.CreateID(name);
                string dispatch = xml.SelectSingleNode("//dispatch").InnerText;
                string action = xml.SelectSingleNode("//action").InnerText;
                if (dispatch == "")
                    dispatch = "function () end";
                if (action == "")
                    action = "function (state) end";

                if (id == "????")
                {
                    Game.Lua.DoString("dispatchqqqq" + " = " + dispatch);
                    Game.Lua.DoString("actionqqqq" + " = " + action);
                }
                else
                {
                    Game.Lua.DoString("dispatch" + id + " = " + dispatch);
                    Game.Lua.DoString("action" + id + " = " + action);
                }

                _table.Add(id, new Spell(node.OuterXml));
            }

        }

        public Spell(string xmlstring)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)));

            _name = xml.SelectSingleNode("//name").InnerText;
            _desc = xml.SelectSingleNode("//desc").InnerText;
            _cost = Int32.Parse(xml.SelectSingleNode("//cost").InnerText);
            _matp = Int32.Parse(xml.SelectSingleNode("//matp").InnerText);
            _order = Int32.Parse(xml.SelectSingleNode("//order").InnerText);

            XmlNodeList nodes = xml.SelectSingleNode("//elements").ChildNodes;
            _element = new Element[nodes.Count];
            
            for(int i = 0;i < _element.Length;i++)
            {
                if (nodes[i].NodeType == XmlNodeType.Comment)
                    throw new GamedataException("Remove XML comment from spell element list goddammit!!");

                _element[i] = (Element)Enum.Parse(typeof(Element), nodes[i].InnerText);
                i++;
            }
        }


        public void Dispatch()
        {
            if (ID == "????")
                Game.Lua.GetFunction("dispatch" + "qqqq").Call();
            else
                Game.Lua.GetFunction("dispatch" + ID).Call();
        }
        public void Action()
        {
            if (ID == "????")
                Game.Lua.GetFunction("action" + "qqqq").Call(Game.Battle.ActiveAbility);
            else
                Game.Lua.GetFunction("action" + ID).Call(Game.Battle.ActiveAbility);
        }

        public static int Compare(Spell left, Spell right)
        {
            return left._order.CompareTo(right._order);
        }


        public string Name { get { return _name; } }
        public string Desc { get { return _desc; } }
        public string ID { get { return Globals.CreateID(_name == null ? "" : _name); } }
        public Element[] Element { get { return _element; } }
        public int Cost { get { return _cost; } }
        public int Matp { get { return _matp; } }
        public int Order { get { return _order; } }

        public static Dictionary<string, Spell> SpellTable { get { return _table; } }

    }
}
