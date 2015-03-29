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
            List<Enemy> enemies = new List<Enemy>();

            foreach (EnemyRecord record in Enemies)
            {
                enemies.Add(Enemy.CreateEnemy(record.ID, record.X, record.Y, record.Designation));
            }

            return enemies;
        }
                
        public string ID { get; private set; }

        private List<EnemyRecord> Enemies { get; set; }
    }
}

