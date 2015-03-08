using System;
using System.Xml;

using Cairo;

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
            RefreshRate = UInt32.Parse(config.SelectSingleNode("/config/window/fps").InnerText);
            
            BackgroundColor = new Color(
                Double.Parse(config.SelectSingleNode("/config/window/bgColor/@r").Value),
                Double.Parse(config.SelectSingleNode("/config/window/bgColor/@g").Value),
                Double.Parse(config.SelectSingleNode("/config/window/bgColor/@b").Value));
            
            Grid = new Color(
                Double.Parse(config.SelectSingleNode("/config/window/gridColor/@r").Value),
                Double.Parse(config.SelectSingleNode("/config/window/gridColor/@g").Value),
                Double.Parse(config.SelectSingleNode("/config/window/gridColor/@b").Value));
        }

        public string WindowTitle { get; private set; }
        
        public int WindowWidth { get; private set; }
        
        public int WindowHeight { get; private set; }

        public uint RefreshRate { get; private set; }
        
        public Color BackgroundColor { get; private set; }
        
        public Color Grid { get; private set; }
    }
}

