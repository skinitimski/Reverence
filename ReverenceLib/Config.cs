using System;
using System.Xml;

using Cairo;

namespace Atmosphere.Reverence
{
    public sealed class Config
    {
        public Config(string configPath)
        {
            //XmlDocument config = Resource.GetXmlFromResource(id);

            Doc = new XmlDocument();
            Doc.Load(configPath);


            SavePath = Doc.SelectSingleNode("/config/savePath").InnerText;

            WindowTitle = Doc.SelectSingleNode("/config/window/title").InnerText;
            WindowWidth = Int32.Parse(Doc.SelectSingleNode("/config/window/width").InnerText);
            WindowHeight = Int32.Parse(Doc.SelectSingleNode("/config/window/height").InnerText);
            RefreshRate = UInt32.Parse(Doc.SelectSingleNode("/config/window/fps").InnerText);
            
            SplashScreenColor = new Color(
                Double.Parse(Doc.SelectSingleNode("/config/window/splashScreenColor/@r").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/splashScreenColor/@g").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/splashScreenColor/@b").Value));
            
            BackgroundColor = new Color(
                Double.Parse(Doc.SelectSingleNode("/config/window/bgColor/@r").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/bgColor/@g").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/bgColor/@b").Value));
            
            Grid = new Color(
                Double.Parse(Doc.SelectSingleNode("/config/window/gridColor/@r").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/gridColor/@g").Value),
                Double.Parse(Doc.SelectSingleNode("/config/window/gridColor/@b").Value));

            for (int i = 0; i < 4; i++)
            {
                XmlNode cornerNode = Doc.SelectSingleNode("/config/window/menu/corner" + i);

                int r = Int32.Parse(cornerNode.Attributes["r"].Value);
                int g = Int32.Parse(cornerNode.Attributes["g"].Value);
                int b = Int32.Parse(cornerNode.Attributes["b"].Value);

                Menu.Menu.SetCornerColor(i, r, g, b);
            }
        }

        public string WindowTitle { get; private set; }
        
        public int WindowWidth { get; private set; }
        
        public int WindowHeight { get; private set; }

        public uint RefreshRate { get; private set; }
        
        public Color SplashScreenColor { get; private set; }
        
        public Color BackgroundColor { get; private set; }
        
        public Color Grid { get; private set; }

        public string SavePath { get; private set; }

        private XmlDocument Doc { get; set; }
    }
}

