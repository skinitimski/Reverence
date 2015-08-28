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


            SavePath = GetNode("/config/savePath").InnerText;

            WindowTitle = GetNode("/config/window/title").InnerText;
            WindowWidth = Int32.Parse(GetNode("/config/window/width").InnerText);
            WindowHeight = Int32.Parse(GetNode("/config/window/height").InnerText);
            RefreshRate = UInt32.Parse(GetNode("/config/window/fps").InnerText);
            
            SplashScreenColor = new Color(
                Double.Parse(GetNode("/config/window/splashScreenColor/@r").Value),
                Double.Parse(GetNode("/config/window/splashScreenColor/@g").Value),
                Double.Parse(GetNode("/config/window/splashScreenColor/@b").Value));
            
            BackgroundColor = new Color(
                Double.Parse(GetNode("/config/window/bgColor/@r").Value),
                Double.Parse(GetNode("/config/window/bgColor/@g").Value),
                Double.Parse(GetNode("/config/window/bgColor/@b").Value));
            
            Grid = new Color(
                Double.Parse(GetNode("/config/window/gridColor/@r").Value),
                Double.Parse(GetNode("/config/window/gridColor/@g").Value),
                Double.Parse(GetNode("/config/window/gridColor/@b").Value));

            ResetMenuColors();
        }


        public XmlNode GetNode(string xpath)
        {
            return Doc.SelectSingleNode(xpath);
        }

        public void ResetMenuColors()
        {              
            for (int i = 0; i < 4; i++)
            {
                XmlNode cornerNode = GetNode("/config/window/menu/corner" + i);
                
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
        
        public int RefreshPeriod { get { return 1000 / (int)RefreshRate; } }
        
        public Color SplashScreenColor { get; private set; }
        
        public Color BackgroundColor { get; private set; }
        
        public Color Grid { get; private set; }

        public string SavePath { get; private set; }

        private XmlDocument Doc { get; set; }
    }
}

