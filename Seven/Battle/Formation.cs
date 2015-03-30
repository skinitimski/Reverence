using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Formation
    {       
        private static Dictionary<string, Formation> _table;
                

        private class EnemyRecord
        {
            public EnemyRecord(XmlNode node)
            {
                ID = node.SelectSingleNode("@id").Value;
                X = Int32.Parse(node.SelectSingleNode("@x").Value);
                Y = Int32.Parse(node.SelectSingleNode("@y").Value);
                Row = Int32.Parse(node.SelectSingleNode("@row").Value);
            }

            public string ID { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Row { get; private set; }
            
            public string Designation { get; set; }
        }

        
        public static void LoadFormations()
        {
            _table = new Dictionary<string, Formation>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.formations.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//formations/formation"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                Formation formation = new Formation(node);
                
                _table.Add(formation.ID, formation);
            }
        }

        private Formation() 
        {
            Enemies = new List<EnemyRecord>();
        }

        private Formation(XmlNode node)
            : this()
        {
            ID = node.SelectSingleNode("@id").Value;

            XmlNode typeNode = node.SelectSingleNode("@type");

            if (typeNode != null)
            {
                Type = (FormationType)Enum.Parse(typeof(FormationType), typeNode.Value);
            }
            else
            {
                Type = FormationType.Normal;
            }

            Dictionary<string, int> counts = new Dictionary<string, int>();
            
            foreach (XmlNode enemyNode in node.SelectNodes("enemy"))
            {
                string id = enemyNode.SelectSingleNode("@id").Value;

                if (!counts.ContainsKey(id))
                {
                    counts.Add(id, 1);
                }
                else
                {
                    counts[id] = counts[id] + 1;
                }
            }

            foreach (XmlNode enemyNode in node.SelectNodes("enemy"))
            {
                EnemyRecord record = new EnemyRecord(enemyNode);

                string designation = "";

                if (counts[record.ID] > 1)
                {
                    int index = Enemies.Where(e => e.ID == record.ID).Count();

                    designation = ((char)('A' + index)).ToString();
                }

                record.Designation = designation;

                Enemies.Add(record);
            }
        }

        public static Formation Get(string id)
        {
            return _table[id];
        }


        public List<Enemy> GetEnemyList()
        {
            int[] e = new int[Enemies.Count];
            
            for (int i = 0; i < Enemies.Count; i++)
            {
                e[i] = Seven.Party.Random.Next(0, Combatant.TURN_TIMER_TIMEOUT / 2);
            }
                        
            switch (Type)
            {                    
                case FormationType.PreEmptive:
                case FormationType.Side:
                    
                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] /= 8;
                    }
                    
                    break;
            }

            List<Enemy> enemies = new List<Enemy>();

            for (int i = 0; i < Enemies.Count; i++)
            {
                EnemyRecord record = Enemies[i];

                enemies.Add(Enemy.CreateEnemy(record.ID, record.X, record.Y, e[i], record.Designation));
            }

            return enemies;
        }

        public int[] GetAllyTurnTimersElapsed()
        {
            int[] e = new int[Party.PARTY_SIZE];
            
            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (Seven.Party[i] != null)
                {
                    e[i] = Seven.Party.Random.Next(0, Combatant.TURN_TIMER_TIMEOUT / 2);
                }
            }

            switch (Type)
            {
                case FormationType.Normal:

                    int max = e.Max();

                    int increase = ((Combatant.TURN_TIMER_TIMEOUT * 87) / 100) - max;

                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] += increase;
                    }

                    break;
                    
                case FormationType.PreEmptive:
                case FormationType.Side:

                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] = Combatant.TURN_TIMER_TIMEOUT - 1;
                    }
                    
                    break;
                    
                case FormationType.Back:
                case FormationType.Pincer:

                    // This is NOT what TFerguson's guide says to do, I couldn't make
                    //  sense of what he was saying. This is just the same as what happens
                    //  to enemy timers in PreEmptive/Side attacks
                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] /= 8;
                    }
                    
                    break;
            }

            return e;
        }
                
        public string ID { get; private set; }

        public FormationType Type { get; private set; }

        private List<EnemyRecord> Enemies { get; set; }
    }
}

