using System;
using System.Xml;

namespace Atmosphere.Reverence
{
    public sealed class Config
    {
        public static readonly Config Instance;

        static Config()
        {
            Instance = new Config();
            Instance.LoadConfiguration("data.config.xml");
        }



        private Config()
        {
        }


        private void LoadConfiguration(string id)
        {
            XmlDocument config = Resource.GetXmlFromResource(id);
            
            WindowTitle = config.SelectSingleNode("/config/window/title").InnerText;
            WindowWidth = Int32.Parse(config.SelectSingleNode("/config/window/width").InnerText);
            WindowHeight = Int32.Parse(config.SelectSingleNode("/config/window/height").InnerText);
        }

        public string WindowTitle { get; private set; }
        
        public int WindowWidth { get; private set; }
        
        public int WindowHeight { get; private set; }
    }
}

