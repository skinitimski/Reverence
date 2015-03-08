using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace Atmosphere.BattleSimulator
{
    public static class Globals
    {
        public const string SAVEGAME = @"Data\savegame.xml";

        public const string WINDOW_NAME = "Battle";

        public const int WIDTH = 800;
        public const int HEIGHT = 600;

        /// <summary>If set to Clock.TICKS_PER_MS, realtime; if less, faster; if greater, slower</summary>
        public static int BattleSpeed = 10000;

        public static XmlDocument SaveGame;

        public static Character[] Party;
        public static Character[,] Reserves;

        public static int Gil;

        public static string Err;



        private static Regex _cleanStringReplacements = new Regex(@"[ -'/!:\*]", RegexOptions.Compiled);







        static Globals()
        {
            SaveGame = new XmlDocument();
            SaveGame.Load(SAVEGAME);

            Party = new Character[3];
            Reserves = new Character[3, 3];
        }

        public static void Init()
        {
            string name;

            for (int k = 1; k <= 3; k++)
            {
                name = SaveGame.SelectSingleNode("//party/slot" + k.ToString()).InnerXml;

                if (!String.IsNullOrEmpty(name)) Party[k] = Character.Table[name];
            }


            int i = 0;
            int j = 0;

            foreach (XmlNode node in SaveGame.SelectNodes("//party//reserve"))
            {
                if (node.NodeType == XmlNodeType.Comment) continue;

                Reserves[j, i] = Character.Table[node.InnerXml];
                i++;
                if (i % 3 == 0)
                {
                    j++;
                    i = 0;
                }
            }

            Gil = Int32.Parse(SaveGame.SelectSingleNode("//gil").InnerXml);
        }

        public static int PartySize()
        {
            int size = 0;

            if (Party[0] != null) size++;
            if (Party[1] != null) size++;
            if (Party[2] != null) size++;

            return size;
        }

        public static int NormalSpeed()
        {
            int sum = 0;

            if (Party[0] != null) sum += Party[0].Dexterity;
            if (Party[1] != null) sum += Party[1].Dexterity;
            if (Party[2] != null) sum += Party[2].Dexterity;

            if (sum % 2 == 1) sum++;

            return (sum / 2) + 50;
        }

        public static int TurnTimerSpeed(Combatant c, int v_timerSpeed)
        {
            return (c.Dexterity + 50) * v_timerSpeed / Globals.NormalSpeed();
        }

        public static string CreateID(string name)
        {
            string temp = CleanString(name);

            StringBuilder id = new StringBuilder();

            for (int i = 0; i < temp.Length; i++) id.Append(Char.ToLower(temp[i]));

            return id.ToString();
        }

        public static string CleanString(string s)
        {
            foreach (string replacement in new string[] { " ", "-", "'", "/", "!", ":", "*" }) s = s.Replace(replacement, "");

            return s;            
        }
    }
}
