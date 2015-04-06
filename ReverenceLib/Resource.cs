using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Atmosphere.Reverence
{
    public static class Resource
    {        
        public static readonly XmlWriterSettings XmlWriterSettings;

        static Resource()
        {
            XmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                OmitXmlDeclaration = true,
                NewLineChars = Environment.NewLine
            };
        }






        public static XmlDocument GetXmlFromResource(string id, Assembly assembly)
        {
            XmlDocument doc = null;
            
            using (Stream resource = assembly.GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    doc = new XmlDocument();
                    doc.LoadXml(reader.ReadToEnd());
                }
            }
            
            return doc;
        }     

        public static XmlDocument GetXmlFromResource(string id)
        {
            return GetXmlFromResource(id, typeof(Resource).Assembly);
        }









        
        public static StreamReader GetStreamReaderFromResource(string id)
        {       
            return GetStreamReaderFromResource(id, typeof(Resource).Assembly);
        }

        public static StreamReader GetStreamReaderFromResource(string id, Assembly assembly)
        {       
            Stream resource = assembly.GetManifestResourceStream(id);
            
            return new StreamReader(resource);
        }



        public static string GetTextFromResource(string id)
        {
            return GetTextFromResource(id, typeof(Resource).Assembly);
        }
        
        public static string GetTextFromResource(string id, Assembly assembly)
        {
            string text = null;
            
            using (Stream resource = assembly.GetManifestResourceStream(id))
            {
                using (StreamReader reader = new StreamReader(resource))
                {
                    text = reader.ReadToEnd();
                }
            }
            
            return text;
        }





        public static string CleanString(string s)
        {
            foreach (string replacement in new string[] { " ", "-", "'", "/", "!", ":", "*" }) s = s.Replace(replacement, "");
            
            return s;            
        }
        
        public static string CreateID(string name)
        {
            string temp = CleanString(name);
            
            StringBuilder id = new StringBuilder();
            
            for (int i = 0; i < temp.Length; i++) id.Append(Char.ToLower(temp[i]));
            
            return id.ToString();
        }
    }
}

