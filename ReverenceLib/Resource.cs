using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Atmosphere.Reverence
{
    public static class Resource
    {        
        public static XmlDocument GetXmlFromResource(string id)
        {
            XmlDocument doc = null;
            
            using (Stream resource = typeof(Resource).Assembly.GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    doc = new XmlDocument();
                    doc.LoadXml(reader.ReadToEnd());
                }
            }
            
            return doc;
        }
        
        public static StreamReader GetStreamReaderFromResource(string id)
        {       
            Stream resource = typeof(Resource).Assembly.GetManifestResourceStream(id);
            
            return new StreamReader(resource);
        }
        
        public static string GetTextFromResource(string id)
        {
            string text = null;

            using (Stream resource = typeof(Resource).Assembly.GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    text = reader.ReadToEnd();
                }
            }
            
            return text;
        }
    }
}

