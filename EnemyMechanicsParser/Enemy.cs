using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Atmosphere.EnemyMechanicsParser
{
    public class Enemy
    {
		public class Item
		{
			public string Name;
			public string Chance;
		}

        private static XmlWriterSettings _xmlWriterSettings;

        public string name;
        public string lvl;
        public string hp;
        public string mp;
        public string exp;
        public string ap;
        public string gil;
        
        // win, steal, morph
        
        // elements
        // weak, absorb, half, void
        
		public List<string> weak = new List<string>();
		public List<string> half = new List<string>();
		public List<string> @void = new List<string>();
		public List<string> absorb = new List<string>();
		
		public List<string> immunities = new List<string>();
		
		public List<Item> win = new List<Item>();
		public List<Item> steal = new List<Item>();
		public String morph;
        
        public string atk;
        public string def;
        public string defp;
        public string dex;
        public string mat;
        public string mdf;
        public string lck;
        
        static Enemy()
        {
            _xmlWriterSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "    "
            };
        }
        
        
        public static Enemy Create(String record)
        {
            Enemy enemy = new Enemy();
            
            enemy.name = GetValueFromRegex(record, @"Name: (.+)");
            enemy.lvl = GetValueFromRegex(record, @" Lvl: +(\d{1,2}) ");
            enemy.hp = GetValueFromRegex(record, @"  HP: +(\d{1,4}) ");
            enemy.mp = GetValueFromRegex(record, @"  MP: +(\d{1,4}) ");
            enemy.exp = GetValueFromRegex(record, @" EXP: +(\d{1,5}) ");
            enemy.ap = GetValueFromRegex(record, @" AP: +(\d{1,5}) ");
            enemy.gil = GetValueFromRegex(record, @" Gil: +(\d{1,6}) ");
            
            enemy.weak.AddRange(GetValuesFromRegex(record, @"Weak: +(.+)"));
            enemy.half.AddRange(GetValuesFromRegex(record, @"Half: +(.+)"));
            enemy.@void.AddRange(GetValuesFromRegex(record, @"Void: +(.+)"));
            enemy.absorb.AddRange(GetValuesFromRegex(record, @"Asrb: +(.+)"));
			enemy.immunities.AddRange(GetValuesFromRegex(record, @"Immune: +(.+)"));

            enemy.morph = GetValueFromRegex(record, @" Morph: +(.+)");
            
            enemy.atk = GetValueFromRegex(record, @" Att: +(\d{1,3}) ");
            enemy.def = GetValueFromRegex(record, @" Def: +(\d{1,3}) ");
            enemy.defp = GetValueFromRegex(record, @" Df%: +(\d{1,3}) ");
            enemy.dex = GetValueFromRegex(record, @" Dex: +(\d{1,3})");
            enemy.mat = GetValueFromRegex(record, @" MAt: +(\d{1,3}) ");
            enemy.mdf = GetValueFromRegex(record, @" MDf: +(\d{1,3}) ");
            enemy.lck = GetValueFromRegex(record, @" Lck: +(\d{1,3})");
                        
            return enemy;
        }
        
        private static string GetValueFromRegex(String record, String regex)
        {
            string value = "";
            
            Match m = Regex.Match(record, regex);
            
            if (m.Success)
            {
                value = m.Groups[1].Value;
            }
            
            return value;
        }
        
        private static List<string> GetValuesFromRegex(String record, String regex)
        {
            List<String> values = new List<string>();
            
            string list = GetValueFromRegex(record, regex);
            
            if (list.Length > 0)
            {
                int startIndex = 0;
                int index = 0;
                
                while ((index = list.IndexOf(',', startIndex)) >= 0)
                {
                    string value = list.Substring(startIndex, index - startIndex);
                    values.Add(value.Trim());
                    
                    startIndex = index + 1;
                }
                
                string lastValue = list.Substring(startIndex).Trim();

                if (!String.IsNullOrEmpty(lastValue))
                {
                    values.Add(lastValue.Trim());
                }
            }
            
            return values;
        }
        
        
        public override string ToString()
        {
            StringWriter representation = new StringWriter();
            
            using (XmlWriter writer = XmlWriter.Create(representation, _xmlWriterSettings))
            {
                writer.WriteStartElement("enemy");
                {
                    writer.WriteElementString("name", name);
                    writer.WriteElementString("lvl", lvl);
                    writer.WriteElementString("hp", hp);
                    writer.WriteElementString("mp", mp);
                    writer.WriteElementString("exp", exp);
                    writer.WriteElementString("ap", ap);
                    writer.WriteElementString("gil", gil);
                    writer.WriteElementString("atk", atk);
                    writer.WriteElementString("def", def);
                    writer.WriteElementString("defp", defp);
                    writer.WriteElementString("dex", dex);
                    writer.WriteElementString("mat", mat);
                    writer.WriteElementString("mdf", mdf);
                    writer.WriteElementString("lck", lck);
					
							
					if (weak.Count > 0)
					{
					    writer.WriteStartElement("weaks");
						{
							foreach (String s in weak)
							{
								writer.WriteElementString("weak", s);
							}
						}
						writer.WriteEndElement(); // weaks
					}

					if (half.Count > 0)
					{
						writer.WriteStartElement("halves");
						{
	                        foreach (String s in half)
							{
								writer.WriteElementString("halve", s);
							}
						}
						writer.WriteEndElement(); // halves
					}

					if (@void.Count > 0)
					{
						writer.WriteStartElement("voids");
						{
	                        foreach (String s in @void)
							{
								writer.WriteElementString("void", s);
							}
						}
						writer.WriteEndElement(); // voids
					}

					if (absorb.Count > 0)
					{
						writer.WriteStartElement("absorbs");
						{
	                        foreach (String s in absorb)
							{
								writer.WriteElementString("absorb", s);
							}
						}
						writer.WriteEndElement(); // absorbs
					}

					if (immunities.Count > 0)
					{
						writer.WriteStartElement("immunities");
						{
	                        foreach (String s in immunities)
							{
								writer.WriteElementString("immunity", s);
							}
						}
						writer.WriteEndElement(); // immunities
					}

					if (!String.IsNullOrWhiteSpace(morph))
					{
						writer.WriteStartElement("morph");
						writer.WriteAttributeString("id", morph);
						writer.WriteEndElement();
					}
                }
                writer.WriteEndElement(); // enemy
            }
            
            return representation.ToString();
        }
    }
}

