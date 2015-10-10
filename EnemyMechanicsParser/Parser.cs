using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Atmosphere.EnemyMechanicsParser
{
    class Parser
    {

        public static void Main(string[] args)
        {
            List<Enemy> enemies = new List<Enemy>();

            using (StreamReader reader = new StreamReader(args[0]))
            {
                string line = reader.ReadLine();

                StringBuilder record = null;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("Name: "))
                    {
                        if (record != null)
                        {
                            try
                            {
                                Enemy enemy = Enemy.Create(record.ToString());

                                enemies.Add(enemy);
                                
                                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                Console.WriteLine(record);
                                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                Console.WriteLine(enemy);
                                Console.ReadLine();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("{0} : {1}{2}{3}", e.GetType().Name, e.Message, Environment.NewLine, e.StackTrace);
                                Console.WriteLine(record);
                                Environment.Exit(1);
                            }
                        }
                        
                        record = new StringBuilder();
                    }

					if (record != null)
					{
                    	record.AppendLine(line);
					}
                }

                if (record != null)
                {
                    Enemy enemy = Enemy.Create(record.ToString());
                    
                    enemies.Add(enemy);
                }
            }
        }

    }
}
