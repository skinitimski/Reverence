using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Atmosphere.Reverence.Seven.State;
using Atmosphere.Reverence.Seven.Battle.Time;
using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Formation
    {       
        private static readonly Random RANDOM = new Random();


        private class EnemyRecord
        {
            public EnemyRecord(XmlNode node)
            {
                Name = node.SelectSingleNode("@name").Value;
                X = Int32.Parse(node.SelectSingleNode("@x").Value);
                Y = Int32.Parse(node.SelectSingleNode("@y").Value);
                Row = Int32.Parse(node.SelectSingleNode("@row").Value);
            }

            public string Name { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Row { get; private set; }
            
            public string Designation { get; set; }
        }



        private Formation() 
        {
            Enemies = new List<EnemyRecord>();
        }

        public Formation(XmlNode node)
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
                string id = enemyNode.SelectSingleNode("@name").Value;

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

                if (counts[record.Name] > 1)
                {
                    int index = Enemies.Where(e => e.Name == record.Name).Count();

                    designation = ((char)('A' + index)).ToString();
                }

                record.Designation = designation;

                Enemies.Add(record);
            }
        }



        public List<Enemy> GetEnemyList(BattleState battle)
        {
            int[] e = new int[Enemies.Count];
            
            for (int i = 0; i < Enemies.Count; i++)
            {
                e[i] = RANDOM.Next(0, (int)TurnTimer.TURN_TIMER_MAX_VALUE / 2);
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
                
                XmlDocument gamedata = Resource.GetXmlFromResource("data.enemies.xml", battle.Seven.Data.Assembly);

                XmlNode node = gamedata.SelectSingleNode(String.Format("//enemy[name = '{0}']", record.Name));

                if (node == null)
                {
                    throw new GameDataException("Formation '{0}' references nonexistent enemy '{1}'", ID, record.Name);
                }

                Enemy enemy = new Enemy(battle, node, record.X, record.Y, e[i], record.Designation);

                enemies.Add(enemy);
            }

            return enemies;
        }

        public int[] GetAllyTurnTimersElapsed(Party party)
        {
            int[] e = new int[Party.PARTY_SIZE];
            
            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (party[i] != null)
                {
                    e[i] = RANDOM.Next(0, (int)TurnTimer.TURN_TIMER_MAX_VALUE / 2);
                }
            }

            switch (Type)
            {
                case FormationType.Normal:

                    int max = e.Max();

                    int increase = (((int)TurnTimer.TURN_TIMER_MAX_VALUE * 87) / 100) - max;

                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] += increase;
                    }

                    break;
                    
                case FormationType.PreEmptive:
                case FormationType.Side:

                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        e[i] = (int)TurnTimer.TURN_TIMER_MAX_VALUE - 1;
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

